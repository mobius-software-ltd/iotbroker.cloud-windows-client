/**
 * Mobius Software LTD
 * Copyright 2015-2017, Mobius Software LTD
 *
 * This is free software; you can redistribute it and/or modify it
 * under the terms of the GNU Lesser General Public License as
 * published by the Free Software Foundation; either version 2.1 of
 * the License, or (at your option) any later version.
 *
 * This software is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this software; if not, write to the Free
 * Software Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA
 * 02110-1301 USA, or see the FSF site: http://www.fsf.org.
 */
 
using com.mobius.software.windows.iotbroker.mqtt.avps;
using com.mobius.software.windows.iotbroker.ui.win7.dal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace com.mobius.software.windows.iotbroker.ui.win7.ui
{
    public partial class NewAccountForm : Form
    {
        public NewAccountForm()
        {
            InitializeComponent();
            cmbProtocol.SelectedIndex = 0;
        }

        private void txtWill_Click(object sender, EventArgs e)
        {
            String value=txtWill.Text;
            if (InputBox("Editing Will", ref value) == DialogResult.OK)
                txtWill.Text = value;
        }

        public static DialogResult InputBox(string title, ref string value)
        {
            Form form = new Form();
            TextBox textBox = new TextBox();            
            Button buttonOk = new Button();
            
            form.Text = title;
            textBox.Text = value;

            buttonOk.Text = "OK";
            buttonOk.DialogResult = DialogResult.OK;
            
            textBox.Multiline = true;
            textBox.SetBounds(3, 3, 294, 150);
            textBox.BorderStyle = BorderStyle.FixedSingle;
            buttonOk.SetBounds(0, 156, 300, 47);
            buttonOk.ForeColor = Color.White;
            buttonOk.FlatStyle = FlatStyle.Flat;
            buttonOk.BackColor = Color.FromArgb(24,118,219);
            textBox.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            textBox.AcceptsReturn = true;
            buttonOk.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            buttonOk.Font= new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0))); 

            form.Controls.Add(textBox);
            form.Controls.Add(buttonOk);

            form.ClientSize = new Size(300, 203);
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.StartPosition = FormStartPosition.CenterScreen;
            form.MinimizeBox = false;
            form.MaximizeBox = false;
            form.AcceptButton = buttonOk;
            
            DialogResult dialogResult = form.ShowDialog();
            value = textBox.Text;
            return dialogResult;
        }

        private void loginButton_Click(object sender, EventArgs e)
        {
            if(cmbProtocol.SelectedIndex==0 || cmbProtocol.SelectedIndex == 3 || cmbProtocol.SelectedIndex == 4)
            {
                if (txtUsername.Text.Length == 0)
                {
                    MessageBox.Show("Please enter username");
                    return;
                }

                if (txtPassword.Text.Length == 0)
                {
                    MessageBox.Show("Please enter password");
                    return;
                }
            }

            if (txtClientID.Text.Length == 0)
            {
                MessageBox.Show("Please enter client ID");
                return;
            }

            if (txtServerHost.Text.Length == 0)
            {
                MessageBox.Show("Please enter server host");
                return;
            }

            if (txtServerPort.Text.Length == 0)
            {
                MessageBox.Show("Please enter server port");
                return;
            }

            Int32 serverPort = 0;
            if (!Int32.TryParse(txtServerPort.Text, out serverPort))
            {
                MessageBox.Show("Server port has invalid value");
                return;
            }

            if (serverPort<=0 || serverPort>UInt16.MaxValue)
            {
                MessageBox.Show("Server port has invalid value");
                return;
            }

            Int32 keepalive = 0;
            if (txtKeepalive.Text.Length > 0)
            {
                if (!Int32.TryParse(txtKeepalive.Text, out keepalive))
                {
                    MessageBox.Show("Keepalive has invalid value");
                    return;
                }
            }

            if (keepalive <= 0)
            {
                MessageBox.Show("Keepalive has invalid value");
                return;
            }

            if (cmbProtocol.SelectedIndex == 0 || cmbProtocol.SelectedIndex == 4 || cmbProtocol.SelectedIndex == 1)
            {
                if (txtWill.Text.Length > 0 && txtWillTopic.Text.Length == 0)
                {
                    MessageBox.Show("Both will and will topic are required");
                    return;
                }

                if (txtWill.Text.Length == 0 && txtWillTopic.Text.Length > 0)
                {
                    MessageBox.Show("Both will and will topic are required");
                    return;
                }

                if (txtWill.Text.Length > 0 && cmbQOS.SelectedIndex == -1)
                {
                    MessageBox.Show("Please choose will QOS");
                    return;
                }
            }

            Account newAccount = new Account();
            newAccount.UserName = txtUsername.Text;
            newAccount.Pass = txtPassword.Text;
            newAccount.ClientID = txtClientID.Text;
            newAccount.ServerHost = txtServerHost.Text;
            newAccount.ServerPort = serverPort;
            newAccount.CleanSession = chkCleanSession.Checked;
            newAccount.IsRetain = chkRetain.Checked;
            newAccount.KeepAlive = keepalive;
            newAccount.Will = Encoding.UTF8.GetBytes(txtWill.Text);
            newAccount.WillTopic = txtWillTopic.Text;
            
            switch (cmbProtocol.SelectedIndex)
            {
                case 0:
                    newAccount.Protocol = iotbroker.dal.Protocols.MQTT_PROTOCOL;
                    break;
                case 1:
                    newAccount.Protocol = iotbroker.dal.Protocols.MQTT_SN_PROTOCOL;
                    break;
                case 2:
                    newAccount.Protocol = iotbroker.dal.Protocols.COAP_PROTOCOL;
                    break;
                case 3:
                    newAccount.Protocol = iotbroker.dal.Protocols.AMQP_PROTOCOL;
                    break;
                case 4:
                    newAccount.Protocol = iotbroker.dal.Protocols.WS_PROTOCOL;
                    break;
            }

            switch (cmbQOS.SelectedIndex)
            {
                case 0:
                    newAccount.QOS = QOS.AT_MOST_ONCE;
                    break;
                case 1:
                    newAccount.QOS = QOS.AT_LEAST_ONCE;
                    break;
                case 2:
                    newAccount.QOS = QOS.EXACTLY_ONCE;
                    break;
            }

            newAccount.IsDefault = true;
            MQTTModel mqtt = new MQTTModel();
            mqtt.Accounts.Add(newAccount);
            mqtt.SaveChanges();
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void cmbProtocol_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (cmbProtocol.SelectedIndex)
            {
                case 0:
                    //MQTT
                    this.pnlUsername.Show();
                    this.pnlPassword.Show();
                    
                    this.pnlCleanSession.Show();
                    this.pnlWill.Show();
                    this.pnlWillTopic.Show();
                    this.pnlRetain.Show();
                    this.pnlQOS.Show();
                    break;
                case 1:
                    //MQTT-SN
                    this.pnlUsername.Hide();
                    this.pnlPassword.Hide();
                    
                    this.pnlCleanSession.Show();
                    this.pnlWill.Show();
                    this.pnlWillTopic.Show();
                    this.pnlRetain.Show();
                    this.pnlQOS.Show();
                    break;
                case 2:
                    //COAP
                    this.pnlUsername.Hide();
                    this.pnlPassword.Hide();
                    
                    this.pnlCleanSession.Hide();
                    this.pnlWill.Hide();
                    this.pnlWillTopic.Hide();
                    this.pnlRetain.Hide();
                    this.pnlQOS.Hide();
                    break;
                case 3:
                    //AMQP
                    this.pnlUsername.Show();
                    this.pnlPassword.Show();
                    
                    this.pnlCleanSession.Hide();
                    this.pnlWill.Hide();
                    this.pnlWillTopic.Hide();
                    this.pnlRetain.Hide();
                    this.pnlQOS.Hide();
                    break;
                case 4:
                    //MQTT-WS
                    this.pnlUsername.Show();
                    this.pnlPassword.Show();

                    this.pnlCleanSession.Show();
                    this.pnlWill.Show();
                    this.pnlWillTopic.Show();
                    this.pnlRetain.Show();
                    this.pnlQOS.Show();
                    break;
            }
        }
    }
}