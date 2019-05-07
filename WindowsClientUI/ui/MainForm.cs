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

using com.mobius.software.windows.iotbroker.mqtt;
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
using com.mobius.software.windows.iotbroker.mqtt.avps;
using System.Data.Entity;
using com.mobius.software.windows.iotbroker.network;

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

namespace com.mobius.software.windows.iotbroker.ui.win7.ui
{
    public partial class MainForm : Form,ClientListener
    {
        private NetworkClient _client;
        private Account _account;
        private EntityFrameworkDBInterface _dbInterface;
        
        public bool UserClosing { get; set; }

        public MainForm(NetworkClient client,Account account, EntityFrameworkDBInterface dbInterface)
        {
            InitializeComponent();
            this._client = client;
            this._account = account;
            this._dbInterface = dbInterface;
            this._client.SetListener(this);
            UserClosing = true;

            if (this._account.Protocol == iotbroker.dal.Protocols.AMQP_PROTOCOL || this._account.Protocol == iotbroker.dal.Protocols.COAP_PROTOCOL)
            {
                pnlDuplicate.Hide();
                pnlRetain.Hide();
            }
            else
            {
                pnlDuplicate.Show();
                pnlRetain.Show();
            }

            if (this._account.Protocol == iotbroker.dal.Protocols.AMQP_PROTOCOL || this._account.Protocol == iotbroker.dal.Protocols.COAP_PROTOCOL)
            {
                this.cmbQOS.Items.Clear();
                this.cmbQOS.Items.AddRange(new object[] {"QOS0","QOS1"});

                this.cmbNewTopicQOS.Items.Clear();
                if (this._account.Protocol == iotbroker.dal.Protocols.AMQP_PROTOCOL)
                    this.cmbNewTopicQOS.Items.AddRange(new object[] {"QOS1"});  
                else
                    this.cmbNewTopicQOS.Items.AddRange(new object[] { "QOS0", "QOS1" });
            }                
            else
            {
                this.cmbNewTopicQOS.Items.Clear();
                this.cmbNewTopicQOS.Items.AddRange(new object[] { "QOS0", "QOS1", "QOS2" });

                this.cmbQOS.Items.Clear();
                this.cmbQOS.Items.AddRange(new object[] { "QOS0", "QOS1", "QOS2" });
            }
        }

        public void StateChanged(network.ConnectionState newState)
        {
            switch (newState)
            {
                case network.ConnectionState.CONNECTION_LOST:
                    MessageBox.Show("Connection to server lost");
                    UserClosing = false;
                    _dbInterface.UnmarkAsDefault(_account);
                    _client.SetListener(null);
                    this.Invoke((MethodInvoker)delegate { Close(); });                    
                    break;
            }
        }

        public void MessageReceived(MessageType messageType)
        {
            switch (messageType)
            {
                case MessageType.SUBACK:
                case MessageType.UNSUBACK:
                    MQTTModel _context = new MQTTModel();
                    List<dal.Topic> topics = (from t in _context.Topics.Include("Account") where t.Account.ID == _account.ID select t).ToList();
                    this.Invoke((MethodInvoker)delegate { this.topicsGrid.DataSource = topics; });
                    break;
                case MessageType.PUBLISH:
                    _context = new MQTTModel();
                    List<dal.Message> messages = (from m in _context.Messages.Include("Account") where m.Account.ID == _account.ID orderby m.ID descending select m).ToList();
                    this.Invoke((MethodInvoker)delegate { this.messagesGrid.DataSource = messages; });
                    break;
            }
        }

        private void btnTopicsList_Click(object sender, EventArgs e)
        {
            btnMessagesList.Image = Properties.Resources.ic_tab_ml;
            btnMessagesList.ForeColor = Color.FromArgb(148, 156, 161);

            btnSendMessage.Image = Properties.Resources.ic_tab_sm;
            btnSendMessage.ForeColor= Color.FromArgb(148, 156, 161);

            btnTopicsList.Image = Properties.Resources.ic_tab_tl_selected;
            btnTopicsList.ForeColor = Color.FromArgb(25, 163, 220);

            mainPanel.SelectedIndex = 0;
        }

        private void btnSendMessage_Click(object sender, EventArgs e)
        {
            btnMessagesList.Image = Properties.Resources.ic_tab_ml;
            btnMessagesList.ForeColor = Color.FromArgb(148, 156, 161);

            btnSendMessage.Image = Properties.Resources.ic_tab_sm_selected;
            btnSendMessage.ForeColor = Color.FromArgb(25, 163, 220); 

            btnTopicsList.Image = Properties.Resources.ic_tab_tl;
            btnTopicsList.ForeColor = Color.FromArgb(148, 156, 161);

            mainPanel.SelectedIndex = 1;
        }

        private void btnMessagesList_Click(object sender, EventArgs e)
        {
            btnMessagesList.Image = Properties.Resources.ic_tab_ml_selected;
            btnMessagesList.ForeColor = Color.FromArgb(25, 163, 220); 

            btnSendMessage.Image = Properties.Resources.ic_tab_sm;
            btnSendMessage.ForeColor = Color.FromArgb(148, 156, 161);

            btnTopicsList.Image = Properties.Resources.ic_tab_tl;
            btnTopicsList.ForeColor = Color.FromArgb(148, 156, 161);

            mainPanel.SelectedIndex = 2;
        }

        private void txtContent_Click(object sender, EventArgs e)
        {
            String value = txtContent.Text;
            if (InputBox("Editing Message Content", ref value) == DialogResult.OK)
            {
                if (_account.Protocol == iotbroker.dal.Protocols.MQTT_SN_PROTOCOL || _account.Protocol == iotbroker.dal.Protocols.COAP_PROTOCOL)
                {
                    if (value.Length > 1400)
                    {
                        MessageBox.Show("Maximum message length for COAP and SN is 1400 characters");
                        return;
                    }
                }
                txtContent.Text = value;
            }                
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
            buttonOk.BackColor = Color.FromArgb(24, 118, 219);
            textBox.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            textBox.AcceptsReturn = true;
            buttonOk.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            buttonOk.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));

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

        private void sendButton_Click(object sender, EventArgs e)
        {
            if (txtContent.Text.Length == 0)
            {
                MessageBox.Show("Content is required");
                return;
            }

            if (txtTopic.Text.Length == 0)
            {
                MessageBox.Show("Topic is required");
                return;
            }

            if (cmbQOS.SelectedIndex == -1)
            {
                MessageBox.Show("Please choose QOS");
                return;
            }

            QOS messageQOS = QOS.AT_MOST_ONCE;
            switch (cmbQOS.SelectedIndex)
            {
                case 1:
                    messageQOS = QOS.AT_LEAST_ONCE;
                    break;
                case 2:
                    messageQOS = QOS.EXACTLY_ONCE;
                    break;
            }

            MQTTModel mqtt = new MQTTModel();
            var accounts = from a in mqtt.Accounts where a.IsDefault == true select a;
            
            dal.Message newMessage = new dal.Message();
            newMessage.Content = Encoding.UTF8.GetBytes(txtContent.Text);
            newMessage.Incoming = false;
            newMessage.QOS = messageQOS;
            newMessage.TopicName = txtTopic.Text;
            newMessage.Account= accounts.First();

            _client.Publish(new mqtt.avps.Topic(newMessage.TopicName, newMessage.QOS), newMessage.Content, chkRetain.Checked, chkDuplicate.Checked);
            mqtt.Accounts.Attach(newMessage.Account);
            mqtt.Messages.Add(newMessage);
            mqtt.Entry(newMessage.Account).State= EntityState.Unchanged;
            mqtt.SaveChanges();

            txtContent.Text = String.Empty;
            txtTopic.Text = String.Empty;
            cmbQOS.SelectedIndex = -1;
            chkDuplicate.Checked = false;
            chkRetain.Checked = false;
            MessageBox.Show("Message Sent Succesfully", "Message Sent", MessageBoxButtons.OK, MessageBoxIcon.Information);

            List<dal.Message> messages = (from m in mqtt.Messages.Include("Account") where m.Account.ID == _account.ID orderby m.ID descending select m).ToList();
            this.Invoke((MethodInvoker)delegate { this.messagesGrid.DataSource = messages; });
            mqtt.Dispose();
        }

        private void addTopicButton_Click(object sender, EventArgs e)
        {
            if (txtNewTopicName.Text.Length == 0)
            {
                MessageBox.Show("Topic is required");
                return;
            }

            if (cmbNewTopicQOS.SelectedIndex == -1)
            {
                MessageBox.Show("Please choose QOS");
                return;
            }

            QOS topicQOS = QOS.AT_MOST_ONCE;
            switch (cmbNewTopicQOS.SelectedIndex)
            {
                case 1:
                    topicQOS = QOS.AT_LEAST_ONCE;
                    break;
                case 2:
                    topicQOS = QOS.EXACTLY_ONCE;
                    break;
            }

            mqtt.avps.Topic[] topics = new mqtt.avps.Topic[] { new mqtt.avps.Topic(txtNewTopicName.Text, topicQOS) };
            txtNewTopicName.Text = String.Empty;

            cmbNewTopicQOS.SelectedIndex = -1;

            MessageBox.Show("Topic Request Sent", "Adding new topic request sent to server", MessageBoxButtons.OK, MessageBoxIcon.Information);
            _client.Subscribe(topics);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            this.topicsGrid.AutoGenerateColumns = false;
            this.messagesGrid.AutoGenerateColumns = false;
            
            MQTTModel _context = new MQTTModel();
            List<dal.Topic> topics= (from t in _context.Topics where t.Account.ID == _account.ID select t).ToList();
            this.topicsGrid.DataSource = topics;
            List<dal.Message> messages = (from m in _context.Messages.Include("Account") where m.Account.ID == _account.ID orderby m.ID ascending select m).ToList();
            this.messagesGrid.DataSource = messages;
        }

        private void topicsGrid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if(e.ColumnIndex==2)
            {
                dal.Topic currTopic = ((dal.Topic)topicsGrid.Rows[e.RowIndex].DataBoundItem);
                _client.Unsubscribe(new String[] { currTopic.TopicName });
                MessageBox.Show("Topic Request Sent", "Removing topic request sent to server", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            _client.SetListener(null);
            if(_client.Disconnect())
                _client.CloseConnection();

            _client = null;
            UserClosing = false;
            _dbInterface.UnmarkAsDefault(_account);
            Close();
        }

        private void topicsGrid_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {            
            for(int i=0;i< topicsGrid.Rows.Count;i++)
            {
                dal.Topic currTopic = ((dal.Topic)topicsGrid.Rows[i].DataBoundItem);
                switch (currTopic.QOS)
                {
                    case QOS.AT_MOST_ONCE:
                        ((DataGridViewImageCell)topicsGrid.Rows[i].Cells[1]).Value = Properties.Resources.icon_qos_0;
                        break;
                    case QOS.AT_LEAST_ONCE:
                        ((DataGridViewImageCell)topicsGrid.Rows[i].Cells[1]).Value = Properties.Resources.icon_qos_1;
                        break;
                    case QOS.EXACTLY_ONCE:
                        ((DataGridViewImageCell)topicsGrid.Rows[i].Cells[1]).Value = Properties.Resources.icon_qos_2;
                        break;
                }
            }
        }

        private void messagesGrid_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {            
            for(int i=0;i< messagesGrid.Rows.Count;i++)
            {
                dal.Message currMessage = ((dal.Message)messagesGrid.Rows[i].DataBoundItem);
                ((DataGridViewTextBoxCell)messagesGrid.Rows[i].Cells[0]).Value = currMessage.TopicName + Environment.NewLine + Environment.NewLine + Encoding.UTF8.GetString(currMessage.Content);
                if (currMessage.Incoming)
                {
                    switch (currMessage.QOS)
                    {
                        case QOS.AT_MOST_ONCE:
                            ((DataGridViewImageCell)messagesGrid.Rows[i].Cells[1]).Value = (System.Drawing.Image)Properties.Resources.icon_in_qos_0;
                            break;
                        case QOS.AT_LEAST_ONCE:
                            ((DataGridViewImageCell)messagesGrid.Rows[i].Cells[1]).Value = (System.Drawing.Image)Properties.Resources.icon_in_qos_1;
                            break;
                        case QOS.EXACTLY_ONCE:
                            ((DataGridViewImageCell)messagesGrid.Rows[i].Cells[1]).Value = (System.Drawing.Image)Properties.Resources.icon_in_qos_2;
                            break;
                    }
                }
                else
                {
                    switch (currMessage.QOS)
                    {
                        case QOS.AT_MOST_ONCE:
                            ((DataGridViewImageCell)messagesGrid.Rows[i].Cells[1]).Value = (System.Drawing.Image)Properties.Resources.icon_out_qos_0;
                            break;
                        case QOS.AT_LEAST_ONCE:
                            ((DataGridViewImageCell)messagesGrid.Rows[i].Cells[1]).Value = (System.Drawing.Image)Properties.Resources.icon_out_qos_1;
                            break;
                        case QOS.EXACTLY_ONCE:
                            ((DataGridViewImageCell)messagesGrid.Rows[i].Cells[1]).Value = (System.Drawing.Image)Properties.Resources.icon_out_qos_2;
                            break;
                    }
                }
            }            
        }

        private void messagesGrid_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex != -1 && e.ColumnIndex==0)
            {
                if (!e.Handled)
                {
                    e.Handled = true;
                    e.PaintBackground(e.CellBounds, messagesGrid.Rows[e.RowIndex].Cells[e.ColumnIndex].Selected);
                }

                if ((e.PaintParts & DataGridViewPaintParts.ContentForeground) != DataGridViewPaintParts.None)
                {
                    if(e.Value!=null)
                    {
                        string text = e.Value.ToString();
                        int newLinePosition = text.IndexOf(Environment.NewLine);
                        string textPart1 = text.Substring(0, newLinePosition);
                        string textPart2 = text.Substring(newLinePosition, text.Length - newLinePosition);
                        Size fullsize = TextRenderer.MeasureText(text, e.CellStyle.Font);
                        Size size1 = TextRenderer.MeasureText(textPart1, e.CellStyle.Font);
                        Size size2 = TextRenderer.MeasureText(textPart2, e.CellStyle.Font);
                        Rectangle rect1 = new Rectangle(e.CellBounds.Location, e.CellBounds.Size);
                        using (Brush cellForeBrush = new SolidBrush(e.CellStyle.ForeColor))
                            e.Graphics.DrawString(textPart1, e.CellStyle.Font, cellForeBrush, rect1);

                        rect1.Y += size1.Height - 20;
                        rect1.Height = e.CellBounds.Height;
                        e.Graphics.DrawString(textPart2, e.CellStyle.Font, new SolidBrush(Color.FromArgb(148, 156, 161)), rect1);
                    }
                }
            }
        }
    }
}
