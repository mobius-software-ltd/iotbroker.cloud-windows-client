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
using com.mobius.software.windows.iotbroker.mqtt.avps;
using com.mobius.software.windows.iotbroker.network;
using com.mobius.software.windows.iotbroker.ui.win7.dal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace com.mobius.software.windows.iotbroker.ui.win7.ui
{
    public partial class LoadingForm : Form,ClientListener
    {
        private NetworkClient _client;
        private Account _account;
        private EntityFrameworkDBInterface _dbInterface;
        public LoadingForm()
        {
            InitializeComponent();            
        }

        private void LoadingForm_Load(object sender, EventArgs e)
        {
            MQTTModel mqtt = new MQTTModel();
            var accounts = from a in mqtt.Accounts where a.IsDefault == true select a;
            if (accounts.Count() > 0)
            {
                this._account = accounts.First();
                initConnect();
            }
            else
            {
                DialogResult result = DialogResult.None;
                this.Hide();
                AccountsDialog dialogForm = new AccountsDialog();
                result = dialogForm.ShowDialog();
                if (result == DialogResult.Cancel)
                {
                    Application.Exit();
                    return;
                }

                if (result == DialogResult.Ignore)
                {
                    NewAccountForm newAccountForm = new NewAccountForm();
                    result = newAccountForm.ShowDialog();
                    if (result == DialogResult.Cancel)
                    {
                        Application.Exit();
                        return;
                    }
                }

                this.Show();
                accounts = from a in mqtt.Accounts where a.IsDefault == true select a;
                this._account = accounts.First();                
                initConnect();
            }
        }

        private void initConnect()
        {
            EndPoint endp = new DnsEndPoint(_account.ServerHost, _account.ServerPort);
            Will will = null;
            if (_account.Will != null && _account.Will.Length>0)
                will = new Will(new mqtt.avps.Topic(_account.WillTopic, _account.QOS), _account.Will, _account.IsRetain);

            _dbInterface = new EntityFrameworkDBInterface(_account);
            _client = new MqttClient(_dbInterface, endp, _account.UserName, _account.Pass, _account.ClientID, _account.CleanSession, _account.KeepAlive,will,this);
            
            bool channelCreated =_client.createChannel();
            if (!channelCreated)
            {
                MessageBox.Show("An error occured while trying to create channel for " + _account.ServerHost + ":" + _account.ServerPort);
                progressBar.Value = 0;
                progressText.Text = "Channel establishment failed";
                _dbInterface.UnmarkAsDefault(_account);
                LoadingForm_Load(this, null);
                return;
            }
        }

        private void updateProgressBar(Int32 value, String text)
        {
            progressBar.Value = value;
            progressText.Text = text;
        }

        private void showMainForm()
        {
            this.Hide();
            MainForm mainForm = new MainForm(_client, _account, _dbInterface);
            mainForm.Closed += new EventHandler(this.MainForm_FormClosed);
            mainForm.Show();
        }

        private void closeConnection()
        {
            _dbInterface.UnmarkAsDefault(_account);
            if (_client != null)
            {
                _client.CloseChannel();
                _client = null;
            }

            if (this.InvokeRequired)
                this.Invoke((MethodInvoker)delegate { this.LoadingForm_Load(this, null); });
            else
                LoadingForm_Load(this, null);
        }

        public void MainForm_FormClosed(object sender, EventArgs e)
        {
            if (_client != null)
            {
                if (((FormClosedEventArgs)e).CloseReason == CloseReason.UserClosing)
                    _client.SetListener(null);

                _client.CloseChannel();
                _client = null;
            }

            if (((MainForm)sender).UserClosing)
                Application.Exit();
            else
            {
                this.Show();
                LoadingForm_Load(this, null);
            }
        }

        public void StateChanged(network.ConnectionState newState)
        {
            switch (newState)
            {
                case network.ConnectionState.CHANNEL_ESTABLISHED:
                    if (this.InvokeRequired)
                        this.Invoke((MethodInvoker)delegate { this.updateProgressBar(33, "Connecting to server"); });
                    else
                        this.updateProgressBar(33, "Connecting to server");

                    _client.Connect();
                    break;
                case network.ConnectionState.CHANNEL_FAILED:
                    if (this.InvokeRequired)
                        this.Invoke((MethodInvoker)delegate { this.updateProgressBar(0, "Channel establishment failed"); });
                    else
                        this.updateProgressBar(0, "Channel establishment failed");

                    MessageBox.Show("An error occured while trying to create channel for " + _account.ServerHost + ":" + _account.ServerPort);
                    _dbInterface.UnmarkAsDefault(_account);
                    if (_client != null)
                    {
                        _client.CloseChannel();
                        _client = null;
                    }

                    if (this.InvokeRequired)
                        this.Invoke((MethodInvoker)delegate { this.LoadingForm_Load(this, null); });
                    else
                        LoadingForm_Load(this, null);

                    break;
                case network.ConnectionState.CONNECTION_ESTABLISHED:
                    if (this.InvokeRequired)
                        this.Invoke((MethodInvoker)delegate { this.updateProgressBar(100, "Connected succesfully"); });
                    else
                        this.updateProgressBar(100, "Connected succesfully");

                    if (this.InvokeRequired)
                        this.Invoke((MethodInvoker)delegate { showMainForm(); });
                    else
                        showMainForm();

                    break;
                case network.ConnectionState.CONNECTION_LOST:
                    MessageBox.Show("Connection closed by server.Server  " + _account.ServerHost + ":" + _account.ServerPort);
                    closeConnection();
                    break;
                case network.ConnectionState.CONNECTION_FAILED:
                    MessageBox.Show("Connection to server has failed  " + _account.ServerHost + ":" + _account.ServerPort);
                    closeConnection();
                    break;        
            }
        }

        public void MessageReceived(MessageType messageType)
        {
            throw new NotImplementedException();
        }
    }
}
