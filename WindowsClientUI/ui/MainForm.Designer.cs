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
 
using com.mobius.software.windows.iotbroker.ui.win7.ui.controls;

namespace com.mobius.software.windows.iotbroker.ui.win7.ui
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.tabsLayout = new System.Windows.Forms.TableLayoutPanel();
            this.pnlSeparator = new System.Windows.Forms.Panel();
            this.dataGridViewImageColumn1 = new System.Windows.Forms.DataGridViewImageColumn();
            this.btnTopicsList = new System.Windows.Forms.Button();
            this.btnSendMessage = new System.Windows.Forms.Button();
            this.btnMessagesList = new System.Windows.Forms.Button();
            this.btnLogout = new System.Windows.Forms.Button();
            this.mainPanel = new com.mobius.software.windows.iotbroker.ui.win7.ui.controls.TabContainer();
            this.tabTopicsList = new System.Windows.Forms.TabPage();
            this.topicsLayout = new System.Windows.Forms.TableLayoutPanel();
            this.pnlNewTopicQOS = new System.Windows.Forms.Panel();
            this.cmbNewTopicQOS = new com.mobius.software.windows.iotbroker.ui.win7.ui.controls.FlatCombo();
            this.lblTopicQOS = new System.Windows.Forms.Label();
            this.iconNewTopicQOS = new System.Windows.Forms.PictureBox();
            this.pnlNewTopicName = new System.Windows.Forms.Panel();
            this.txtNewTopicName = new System.Windows.Forms.TextBox();
            this.lblNewTopicName = new System.Windows.Forms.Label();
            this.imageNewTopicName = new System.Windows.Forms.PictureBox();
            this.pnlNewTopicHeader = new System.Windows.Forms.Panel();
            this.lblNewTopicHeader = new System.Windows.Forms.Label();
            this.addTopicButton = new System.Windows.Forms.Button();
            this.pnlTopicsListHeader = new System.Windows.Forms.Panel();
            this.lblTopicsListHeader = new System.Windows.Forms.Label();
            this.topicsGrid = new System.Windows.Forms.DataGridView();
            this.TopicName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TopicQOS = new System.Windows.Forms.DataGridViewImageColumn();
            this.TopicDelete = new System.Windows.Forms.DataGridViewImageColumn();
            this.tabSendMessage = new System.Windows.Forms.TabPage();
            this.newMessageLayout = new System.Windows.Forms.TableLayoutPanel();
            this.sendButton = new System.Windows.Forms.Button();
            this.pnlDuplicate = new System.Windows.Forms.Panel();
            this.chkDuplicate = new System.Windows.Forms.CheckBox();
            this.lblDuplicate = new System.Windows.Forms.Label();
            this.imageDuplicate = new System.Windows.Forms.PictureBox();
            this.pnlRetain = new System.Windows.Forms.Panel();
            this.chkRetain = new System.Windows.Forms.CheckBox();
            this.lblRetain = new System.Windows.Forms.Label();
            this.imageRetain = new System.Windows.Forms.PictureBox();
            this.pnlQOS = new System.Windows.Forms.Panel();
            this.cmbQOS = new com.mobius.software.windows.iotbroker.ui.win7.ui.controls.FlatCombo();
            this.lblQOS = new System.Windows.Forms.Label();
            this.imageQOS = new System.Windows.Forms.PictureBox();
            this.pnlTopic = new System.Windows.Forms.Panel();
            this.txtTopic = new System.Windows.Forms.TextBox();
            this.lblWillTopic = new System.Windows.Forms.Label();
            this.imageTopic = new System.Windows.Forms.PictureBox();
            this.pnlContent = new System.Windows.Forms.Panel();
            this.txtContent = new System.Windows.Forms.TextBox();
            this.lblContent = new System.Windows.Forms.Label();
            this.imageContent = new System.Windows.Forms.PictureBox();
            this.pnlNewMessageHeader = new System.Windows.Forms.Panel();
            this.lblNewMessage = new System.Windows.Forms.Label();
            this.tabMessagesList = new System.Windows.Forms.TabPage();
            this.messagesGrid = new System.Windows.Forms.DataGridView();
            this.MessageContentColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MessageTypeColumn = new System.Windows.Forms.DataGridViewImageColumn();
            this.tabsLayout.SuspendLayout();
            this.mainPanel.SuspendLayout();
            this.tabTopicsList.SuspendLayout();
            this.topicsLayout.SuspendLayout();
            this.pnlNewTopicQOS.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.iconNewTopicQOS)).BeginInit();
            this.pnlNewTopicName.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imageNewTopicName)).BeginInit();
            this.pnlNewTopicHeader.SuspendLayout();
            this.pnlTopicsListHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.topicsGrid)).BeginInit();
            this.tabSendMessage.SuspendLayout();
            this.newMessageLayout.SuspendLayout();
            this.pnlDuplicate.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imageDuplicate)).BeginInit();
            this.pnlRetain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imageRetain)).BeginInit();
            this.pnlQOS.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imageQOS)).BeginInit();
            this.pnlTopic.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imageTopic)).BeginInit();
            this.pnlContent.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imageContent)).BeginInit();
            this.pnlNewMessageHeader.SuspendLayout();
            this.tabMessagesList.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.messagesGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // tabsLayout
            // 
            this.tabsLayout.ColumnCount = 4;
            this.tabsLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tabsLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tabsLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tabsLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tabsLayout.Controls.Add(this.btnTopicsList, 0, 0);
            this.tabsLayout.Controls.Add(this.btnSendMessage, 1, 0);
            this.tabsLayout.Controls.Add(this.btnMessagesList, 2, 0);
            this.tabsLayout.Controls.Add(this.btnLogout, 3, 0);
            this.tabsLayout.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.tabsLayout.Location = new System.Drawing.Point(0, 292);
            this.tabsLayout.Name = "tabsLayout";
            this.tabsLayout.RowCount = 1;
            this.tabsLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tabsLayout.Size = new System.Drawing.Size(284, 70);
            this.tabsLayout.TabIndex = 0;
            // 
            // pnlSeparator
            // 
            this.pnlSeparator.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(217)))), ((int)(((byte)(225)))), ((int)(((byte)(227)))));
            this.pnlSeparator.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlSeparator.Location = new System.Drawing.Point(0, 291);
            this.pnlSeparator.Name = "pnlSeparator";
            this.pnlSeparator.Size = new System.Drawing.Size(284, 1);
            this.pnlSeparator.TabIndex = 1;
            // 
            // dataGridViewImageColumn1
            // 
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.NullValue = ((object)(resources.GetObject("dataGridViewCellStyle1.NullValue")));
            this.dataGridViewImageColumn1.DefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridViewImageColumn1.FillWeight = 40F;
            this.dataGridViewImageColumn1.HeaderText = "Delete";
            this.dataGridViewImageColumn1.Image = global::com.mobius.software.windows.iotbroker.ui.win7.Properties.Resources.ic_delete;
            this.dataGridViewImageColumn1.MinimumWidth = 70;
            this.dataGridViewImageColumn1.Name = "dataGridViewImageColumn1";
            this.dataGridViewImageColumn1.Width = 70;
            // 
            // btnTopicsList
            // 
            this.btnTopicsList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnTopicsList.FlatAppearance.BorderSize = 0;
            this.btnTopicsList.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTopicsList.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(163)))), ((int)(((byte)(220)))));
            this.btnTopicsList.Image = global::com.mobius.software.windows.iotbroker.ui.win7.Properties.Resources.ic_tab_tl_selected;
            this.btnTopicsList.Location = new System.Drawing.Point(3, 3);
            this.btnTopicsList.Name = "btnTopicsList";
            this.btnTopicsList.Size = new System.Drawing.Size(65, 64);
            this.btnTopicsList.TabIndex = 0;
            this.btnTopicsList.Text = "Topics List";
            this.btnTopicsList.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnTopicsList.UseVisualStyleBackColor = true;
            this.btnTopicsList.Click += new System.EventHandler(this.btnTopicsList_Click);
            // 
            // btnSendMessage
            // 
            this.btnSendMessage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnSendMessage.FlatAppearance.BorderSize = 0;
            this.btnSendMessage.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSendMessage.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(156)))), ((int)(((byte)(161)))));
            this.btnSendMessage.Image = global::com.mobius.software.windows.iotbroker.ui.win7.Properties.Resources.ic_tab_sm;
            this.btnSendMessage.Location = new System.Drawing.Point(74, 3);
            this.btnSendMessage.Name = "btnSendMessage";
            this.btnSendMessage.Size = new System.Drawing.Size(65, 64);
            this.btnSendMessage.TabIndex = 1;
            this.btnSendMessage.Text = "Send Message";
            this.btnSendMessage.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnSendMessage.UseVisualStyleBackColor = true;
            this.btnSendMessage.Click += new System.EventHandler(this.btnSendMessage_Click);
            // 
            // btnMessagesList
            // 
            this.btnMessagesList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnMessagesList.FlatAppearance.BorderSize = 0;
            this.btnMessagesList.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMessagesList.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(156)))), ((int)(((byte)(161)))));
            this.btnMessagesList.Image = global::com.mobius.software.windows.iotbroker.ui.win7.Properties.Resources.ic_tab_ml;
            this.btnMessagesList.Location = new System.Drawing.Point(145, 3);
            this.btnMessagesList.Name = "btnMessagesList";
            this.btnMessagesList.Size = new System.Drawing.Size(65, 64);
            this.btnMessagesList.TabIndex = 2;
            this.btnMessagesList.Text = "Messages List";
            this.btnMessagesList.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnMessagesList.UseVisualStyleBackColor = true;
            this.btnMessagesList.Click += new System.EventHandler(this.btnMessagesList_Click);
            // 
            // btnLogout
            // 
            this.btnLogout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnLogout.FlatAppearance.BorderSize = 0;
            this.btnLogout.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLogout.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(156)))), ((int)(((byte)(161)))));
            this.btnLogout.Image = global::com.mobius.software.windows.iotbroker.ui.win7.Properties.Resources.icon_logout;
            this.btnLogout.Location = new System.Drawing.Point(216, 3);
            this.btnLogout.Name = "btnLogout";
            this.btnLogout.Size = new System.Drawing.Size(65, 64);
            this.btnLogout.TabIndex = 3;
            this.btnLogout.Text = "Logout";
            this.btnLogout.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnLogout.UseVisualStyleBackColor = true;
            this.btnLogout.Click += new System.EventHandler(this.btnLogout_Click);
            // 
            // mainPanel
            // 
            this.mainPanel.Appearance = System.Windows.Forms.TabAppearance.FlatButtons;
            this.mainPanel.Controls.Add(this.tabTopicsList);
            this.mainPanel.Controls.Add(this.tabSendMessage);
            this.mainPanel.Controls.Add(this.tabMessagesList);
            this.mainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainPanel.ItemSize = new System.Drawing.Size(0, 1);
            this.mainPanel.Location = new System.Drawing.Point(0, 0);
            this.mainPanel.Margin = new System.Windows.Forms.Padding(0);
            this.mainPanel.Name = "mainPanel";
            this.mainPanel.Padding = new System.Drawing.Point(0, 0);
            this.mainPanel.SelectedIndex = 0;
            this.mainPanel.Size = new System.Drawing.Size(284, 291);
            this.mainPanel.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.mainPanel.TabIndex = 2;
            // 
            // tabTopicsList
            // 
            this.tabTopicsList.BackColor = System.Drawing.Color.White;
            this.tabTopicsList.Controls.Add(this.topicsLayout);
            this.tabTopicsList.Location = new System.Drawing.Point(4, 5);
            this.tabTopicsList.Margin = new System.Windows.Forms.Padding(0);
            this.tabTopicsList.Name = "tabTopicsList";
            this.tabTopicsList.Size = new System.Drawing.Size(276, 282);
            this.tabTopicsList.TabIndex = 0;
            this.tabTopicsList.Text = "Topics List";
            // 
            // topicsLayout
            // 
            this.topicsLayout.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.topicsLayout.ColumnCount = 1;
            this.topicsLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.topicsLayout.Controls.Add(this.pnlNewTopicQOS, 0, 4);
            this.topicsLayout.Controls.Add(this.pnlNewTopicName, 0, 3);
            this.topicsLayout.Controls.Add(this.pnlNewTopicHeader, 0, 2);
            this.topicsLayout.Controls.Add(this.addTopicButton, 0, 5);
            this.topicsLayout.Controls.Add(this.pnlTopicsListHeader, 0, 0);
            this.topicsLayout.Controls.Add(this.topicsGrid, 0, 1);
            this.topicsLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.topicsLayout.Location = new System.Drawing.Point(0, 0);
            this.topicsLayout.Margin = new System.Windows.Forms.Padding(0);
            this.topicsLayout.Name = "topicsLayout";
            this.topicsLayout.RowCount = 6;
            this.topicsLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.topicsLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.topicsLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.topicsLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.topicsLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.topicsLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.topicsLayout.Size = new System.Drawing.Size(276, 282);
            this.topicsLayout.TabIndex = 0;
            // 
            // pnlNewTopicQOS
            // 
            this.pnlNewTopicQOS.Controls.Add(this.cmbNewTopicQOS);
            this.pnlNewTopicQOS.Controls.Add(this.lblTopicQOS);
            this.pnlNewTopicQOS.Controls.Add(this.iconNewTopicQOS);
            this.pnlNewTopicQOS.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlNewTopicQOS.Location = new System.Drawing.Point(2, 221);
            this.pnlNewTopicQOS.Margin = new System.Windows.Forms.Padding(1);
            this.pnlNewTopicQOS.Name = "pnlNewTopicQOS";
            this.pnlNewTopicQOS.Size = new System.Drawing.Size(272, 28);
            this.pnlNewTopicQOS.TabIndex = 52;
            // 
            // cmbNewTopicQOS
            // 
            this.cmbNewTopicQOS.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbNewTopicQOS.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbNewTopicQOS.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.cmbNewTopicQOS.FormattingEnabled = true;
            this.cmbNewTopicQOS.Items.AddRange(new object[] {
            "QOS0",
            "QOS1",
            "QOS2"});
            this.cmbNewTopicQOS.Location = new System.Drawing.Point(120, 3);
            this.cmbNewTopicQOS.Name = "cmbNewTopicQOS";
            this.cmbNewTopicQOS.Size = new System.Drawing.Size(150, 21);
            this.cmbNewTopicQOS.TabIndex = 3;
            // 
            // lblTopicQOS
            // 
            this.lblTopicQOS.AutoSize = true;
            this.lblTopicQOS.Location = new System.Drawing.Point(30, 7);
            this.lblTopicQOS.Margin = new System.Windows.Forms.Padding(3);
            this.lblTopicQOS.Name = "lblTopicQOS";
            this.lblTopicQOS.Size = new System.Drawing.Size(29, 13);
            this.lblTopicQOS.TabIndex = 23;
            this.lblTopicQOS.Text = "Qos:";
            // 
            // iconNewTopicQOS
            // 
            this.iconNewTopicQOS.Image = global::com.mobius.software.windows.iotbroker.ui.win7.Properties.Resources.icon_qos_small;
            this.iconNewTopicQOS.Location = new System.Drawing.Point(5, 0);
            this.iconNewTopicQOS.Margin = new System.Windows.Forms.Padding(1);
            this.iconNewTopicQOS.Name = "iconNewTopicQOS";
            this.iconNewTopicQOS.Size = new System.Drawing.Size(24, 25);
            this.iconNewTopicQOS.TabIndex = 0;
            this.iconNewTopicQOS.TabStop = false;
            // 
            // pnlNewTopicName
            // 
            this.pnlNewTopicName.Controls.Add(this.txtNewTopicName);
            this.pnlNewTopicName.Controls.Add(this.lblNewTopicName);
            this.pnlNewTopicName.Controls.Add(this.imageNewTopicName);
            this.pnlNewTopicName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlNewTopicName.Location = new System.Drawing.Point(2, 190);
            this.pnlNewTopicName.Margin = new System.Windows.Forms.Padding(1);
            this.pnlNewTopicName.Name = "pnlNewTopicName";
            this.pnlNewTopicName.Size = new System.Drawing.Size(272, 28);
            this.pnlNewTopicName.TabIndex = 51;
            // 
            // txtNewTopicName
            // 
            this.txtNewTopicName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtNewTopicName.Location = new System.Drawing.Point(120, 4);
            this.txtNewTopicName.Name = "txtNewTopicName";
            this.txtNewTopicName.Size = new System.Drawing.Size(155, 20);
            this.txtNewTopicName.TabIndex = 2;
            // 
            // lblNewTopicName
            // 
            this.lblNewTopicName.AutoSize = true;
            this.lblNewTopicName.Location = new System.Drawing.Point(30, 7);
            this.lblNewTopicName.Margin = new System.Windows.Forms.Padding(3);
            this.lblNewTopicName.Name = "lblNewTopicName";
            this.lblNewTopicName.Size = new System.Drawing.Size(37, 13);
            this.lblNewTopicName.TabIndex = 22;
            this.lblNewTopicName.Text = "Topic:";
            // 
            // imageNewTopicName
            // 
            this.imageNewTopicName.Image = global::com.mobius.software.windows.iotbroker.ui.win7.Properties.Resources.icon_will_topic_small;
            this.imageNewTopicName.Location = new System.Drawing.Point(5, 0);
            this.imageNewTopicName.Margin = new System.Windows.Forms.Padding(1);
            this.imageNewTopicName.Name = "imageNewTopicName";
            this.imageNewTopicName.Size = new System.Drawing.Size(24, 25);
            this.imageNewTopicName.TabIndex = 0;
            this.imageNewTopicName.TabStop = false;
            // 
            // pnlNewTopicHeader
            // 
            this.pnlNewTopicHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(215)))), ((int)(((byte)(219)))));
            this.pnlNewTopicHeader.Controls.Add(this.lblNewTopicHeader);
            this.pnlNewTopicHeader.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlNewTopicHeader.Location = new System.Drawing.Point(1, 168);
            this.pnlNewTopicHeader.Margin = new System.Windows.Forms.Padding(0);
            this.pnlNewTopicHeader.Name = "pnlNewTopicHeader";
            this.pnlNewTopicHeader.Size = new System.Drawing.Size(274, 20);
            this.pnlNewTopicHeader.TabIndex = 50;
            // 
            // lblNewTopicHeader
            // 
            this.lblNewTopicHeader.AutoSize = true;
            this.lblNewTopicHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewTopicHeader.ForeColor = System.Drawing.Color.Black;
            this.lblNewTopicHeader.Location = new System.Drawing.Point(7, 3);
            this.lblNewTopicHeader.Name = "lblNewTopicHeader";
            this.lblNewTopicHeader.Size = new System.Drawing.Size(68, 13);
            this.lblNewTopicHeader.TabIndex = 0;
            this.lblNewTopicHeader.Text = "New Topic";
            // 
            // addTopicButton
            // 
            this.addTopicButton.AutoSize = true;
            this.addTopicButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(118)))), ((int)(((byte)(219)))));
            this.addTopicButton.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.addTopicButton.FlatAppearance.BorderSize = 0;
            this.addTopicButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.addTopicButton.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.addTopicButton.ForeColor = System.Drawing.Color.White;
            this.addTopicButton.Location = new System.Drawing.Point(1, 252);
            this.addTopicButton.Margin = new System.Windows.Forms.Padding(0);
            this.addTopicButton.Name = "addTopicButton";
            this.addTopicButton.Size = new System.Drawing.Size(274, 29);
            this.addTopicButton.TabIndex = 49;
            this.addTopicButton.Text = "Add Topic";
            this.addTopicButton.UseVisualStyleBackColor = false;
            this.addTopicButton.Click += new System.EventHandler(this.addTopicButton_Click);
            // 
            // pnlTopicsListHeader
            // 
            this.pnlTopicsListHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(215)))), ((int)(((byte)(219)))));
            this.pnlTopicsListHeader.Controls.Add(this.lblTopicsListHeader);
            this.pnlTopicsListHeader.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlTopicsListHeader.Location = new System.Drawing.Point(1, 1);
            this.pnlTopicsListHeader.Margin = new System.Windows.Forms.Padding(0);
            this.pnlTopicsListHeader.Name = "pnlTopicsListHeader";
            this.pnlTopicsListHeader.Size = new System.Drawing.Size(274, 20);
            this.pnlTopicsListHeader.TabIndex = 48;
            // 
            // lblTopicsListHeader
            // 
            this.lblTopicsListHeader.AutoSize = true;
            this.lblTopicsListHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTopicsListHeader.ForeColor = System.Drawing.Color.Black;
            this.lblTopicsListHeader.Location = new System.Drawing.Point(7, 3);
            this.lblTopicsListHeader.Name = "lblTopicsListHeader";
            this.lblTopicsListHeader.Size = new System.Drawing.Size(69, 13);
            this.lblTopicsListHeader.TabIndex = 0;
            this.lblTopicsListHeader.Text = "Topics List";
            // 
            // topicsGrid
            // 
            this.topicsGrid.AllowUserToAddRows = false;
            this.topicsGrid.AllowUserToDeleteRows = false;
            this.topicsGrid.AllowUserToResizeColumns = false;
            this.topicsGrid.AllowUserToResizeRows = false;
            this.topicsGrid.BackgroundColor = System.Drawing.Color.White;
            this.topicsGrid.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.topicsGrid.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.topicsGrid.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.topicsGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.topicsGrid.ColumnHeadersVisible = false;
            this.topicsGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.TopicName,
            this.TopicQOS,
            this.TopicDelete});
            this.topicsGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.topicsGrid.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(156)))), ((int)(((byte)(161)))));
            this.topicsGrid.Location = new System.Drawing.Point(1, 22);
            this.topicsGrid.Margin = new System.Windows.Forms.Padding(0);
            this.topicsGrid.Name = "topicsGrid";
            this.topicsGrid.ReadOnly = true;
            this.topicsGrid.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.topicsGrid.RowHeadersVisible = false;
            this.topicsGrid.RowTemplate.DefaultCellStyle.BackColor = System.Drawing.Color.White;
            this.topicsGrid.RowTemplate.DefaultCellStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.topicsGrid.RowTemplate.DefaultCellStyle.ForeColor = System.Drawing.Color.Black;
            this.topicsGrid.RowTemplate.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.White;
            this.topicsGrid.RowTemplate.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.Black;
            this.topicsGrid.RowTemplate.DividerHeight = 1;
            this.topicsGrid.RowTemplate.Height = 35;
            this.topicsGrid.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.topicsGrid.ShowCellErrors = false;
            this.topicsGrid.ShowCellToolTips = false;
            this.topicsGrid.ShowEditingIcon = false;
            this.topicsGrid.Size = new System.Drawing.Size(274, 145);
            this.topicsGrid.TabIndex = 1;
            this.topicsGrid.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.topicsGrid_CellClick);
            this.topicsGrid.RowsAdded += new System.Windows.Forms.DataGridViewRowsAddedEventHandler(this.topicsGrid_RowsAdded);
            // 
            // TopicName
            // 
            this.TopicName.DataPropertyName = "TopicName";
            this.TopicName.FillWeight = 124F;
            this.TopicName.HeaderText = "Name";
            this.TopicName.MinimumWidth = 124;
            this.TopicName.Name = "TopicName";
            this.TopicName.ReadOnly = true;
            this.TopicName.Width = 124;
            // 
            // TopicQOS
            // 
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.NullValue = ((object)(resources.GetObject("dataGridViewCellStyle2.NullValue")));
            this.TopicQOS.DefaultCellStyle = dataGridViewCellStyle2;
            this.TopicQOS.HeaderText = "QOS";
            this.TopicQOS.Name = "TopicQOS";
            this.TopicQOS.ReadOnly = true;
            // 
            // TopicDelete
            // 
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle3.NullValue = ((object)(resources.GetObject("dataGridViewCellStyle3.NullValue")));
            this.TopicDelete.DefaultCellStyle = dataGridViewCellStyle3;
            this.TopicDelete.FillWeight = 60F;
            this.TopicDelete.HeaderText = "Delete";
            this.TopicDelete.Image = global::com.mobius.software.windows.iotbroker.ui.win7.Properties.Resources.ic_delete;
            this.TopicDelete.MinimumWidth = 60;
            this.TopicDelete.Name = "TopicDelete";
            this.TopicDelete.ReadOnly = true;
            this.TopicDelete.Width = 60;
            // 
            // tabSendMessage
            // 
            this.tabSendMessage.BackColor = System.Drawing.Color.White;
            this.tabSendMessage.Controls.Add(this.newMessageLayout);
            this.tabSendMessage.Location = new System.Drawing.Point(4, 5);
            this.tabSendMessage.Margin = new System.Windows.Forms.Padding(0);
            this.tabSendMessage.Name = "tabSendMessage";
            this.tabSendMessage.Size = new System.Drawing.Size(276, 282);
            this.tabSendMessage.TabIndex = 1;
            this.tabSendMessage.Text = "Send Message";
            // 
            // newMessageLayout
            // 
            this.newMessageLayout.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.newMessageLayout.ColumnCount = 1;
            this.newMessageLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.newMessageLayout.Controls.Add(this.sendButton, 0, 6);
            this.newMessageLayout.Controls.Add(this.pnlDuplicate, 0, 4);
            this.newMessageLayout.Controls.Add(this.pnlRetain, 0, 5);
            this.newMessageLayout.Controls.Add(this.pnlQOS, 0, 3);
            this.newMessageLayout.Controls.Add(this.pnlTopic, 0, 2);
            this.newMessageLayout.Controls.Add(this.pnlContent, 0, 1);
            this.newMessageLayout.Controls.Add(this.pnlNewMessageHeader, 0, 0);
            this.newMessageLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.newMessageLayout.Location = new System.Drawing.Point(0, 0);
            this.newMessageLayout.Margin = new System.Windows.Forms.Padding(0);
            this.newMessageLayout.Name = "newMessageLayout";
            this.newMessageLayout.RowCount = 7;
            this.newMessageLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.newMessageLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.newMessageLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.newMessageLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.newMessageLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.newMessageLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.newMessageLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.newMessageLayout.Size = new System.Drawing.Size(276, 282);
            this.newMessageLayout.TabIndex = 0;
            // 
            // sendButton
            // 
            this.sendButton.AutoSize = true;
            this.sendButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(118)))), ((int)(((byte)(219)))));
            this.sendButton.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.sendButton.FlatAppearance.BorderSize = 0;
            this.sendButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.sendButton.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.sendButton.ForeColor = System.Drawing.Color.White;
            this.sendButton.Location = new System.Drawing.Point(1, 244);
            this.sendButton.Margin = new System.Windows.Forms.Padding(0);
            this.sendButton.Name = "sendButton";
            this.sendButton.Size = new System.Drawing.Size(274, 37);
            this.sendButton.TabIndex = 6;
            this.sendButton.Text = "Send";
            this.sendButton.UseVisualStyleBackColor = false;
            this.sendButton.Click += new System.EventHandler(this.sendButton_Click);
            // 
            // pnlDuplicate
            // 
            this.pnlDuplicate.Controls.Add(this.chkDuplicate);
            this.pnlDuplicate.Controls.Add(this.lblDuplicate);
            this.pnlDuplicate.Controls.Add(this.imageDuplicate);
            this.pnlDuplicate.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlDuplicate.Location = new System.Drawing.Point(2, 147);
            this.pnlDuplicate.Margin = new System.Windows.Forms.Padding(1);
            this.pnlDuplicate.Name = "pnlDuplicate";
            this.pnlDuplicate.Size = new System.Drawing.Size(272, 28);
            this.pnlDuplicate.TabIndex = 45;
            // 
            // chkDuplicate
            // 
            this.chkDuplicate.AutoSize = true;
            this.chkDuplicate.ForeColor = System.Drawing.SystemColors.Control;
            this.chkDuplicate.Location = new System.Drawing.Point(259, 8);
            this.chkDuplicate.Name = "chkDuplicate";
            this.chkDuplicate.Size = new System.Drawing.Size(15, 14);
            this.chkDuplicate.TabIndex = 5;
            this.chkDuplicate.UseVisualStyleBackColor = true;
            // 
            // lblDuplicate
            // 
            this.lblDuplicate.AutoSize = true;
            this.lblDuplicate.Location = new System.Drawing.Point(30, 7);
            this.lblDuplicate.Margin = new System.Windows.Forms.Padding(3);
            this.lblDuplicate.Name = "lblDuplicate";
            this.lblDuplicate.Size = new System.Drawing.Size(55, 13);
            this.lblDuplicate.TabIndex = 25;
            this.lblDuplicate.Text = "Duplicate:";
            // 
            // imageDuplicate
            // 
            this.imageDuplicate.Image = global::com.mobius.software.windows.iotbroker.ui.win7.Properties.Resources.icon_retain_small;
            this.imageDuplicate.Location = new System.Drawing.Point(5, 0);
            this.imageDuplicate.Margin = new System.Windows.Forms.Padding(1);
            this.imageDuplicate.Name = "imageDuplicate";
            this.imageDuplicate.Size = new System.Drawing.Size(24, 25);
            this.imageDuplicate.TabIndex = 0;
            this.imageDuplicate.TabStop = false;
            // 
            // pnlRetain
            // 
            this.pnlRetain.Controls.Add(this.chkRetain);
            this.pnlRetain.Controls.Add(this.lblRetain);
            this.pnlRetain.Controls.Add(this.imageRetain);
            this.pnlRetain.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlRetain.Location = new System.Drawing.Point(2, 116);
            this.pnlRetain.Margin = new System.Windows.Forms.Padding(1);
            this.pnlRetain.Name = "pnlRetain";
            this.pnlRetain.Size = new System.Drawing.Size(272, 28);
            this.pnlRetain.TabIndex = 44;
            // 
            // chkRetain
            // 
            this.chkRetain.AutoSize = true;
            this.chkRetain.ForeColor = System.Drawing.SystemColors.Control;
            this.chkRetain.Location = new System.Drawing.Point(259, 8);
            this.chkRetain.Name = "chkRetain";
            this.chkRetain.Size = new System.Drawing.Size(15, 14);
            this.chkRetain.TabIndex = 4;
            this.chkRetain.UseVisualStyleBackColor = true;
            // 
            // lblRetain
            // 
            this.lblRetain.AutoSize = true;
            this.lblRetain.Location = new System.Drawing.Point(30, 7);
            this.lblRetain.Margin = new System.Windows.Forms.Padding(3);
            this.lblRetain.Name = "lblRetain";
            this.lblRetain.Size = new System.Drawing.Size(41, 13);
            this.lblRetain.TabIndex = 24;
            this.lblRetain.Text = "Retain:";
            // 
            // imageRetain
            // 
            this.imageRetain.Image = global::com.mobius.software.windows.iotbroker.ui.win7.Properties.Resources.icon_retain_small;
            this.imageRetain.Location = new System.Drawing.Point(5, 0);
            this.imageRetain.Margin = new System.Windows.Forms.Padding(1);
            this.imageRetain.Name = "imageRetain";
            this.imageRetain.Size = new System.Drawing.Size(24, 25);
            this.imageRetain.TabIndex = 0;
            this.imageRetain.TabStop = false;
            // 
            // pnlQOS
            // 
            this.pnlQOS.Controls.Add(this.cmbQOS);
            this.pnlQOS.Controls.Add(this.lblQOS);
            this.pnlQOS.Controls.Add(this.imageQOS);
            this.pnlQOS.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlQOS.Location = new System.Drawing.Point(2, 85);
            this.pnlQOS.Margin = new System.Windows.Forms.Padding(1);
            this.pnlQOS.Name = "pnlQOS";
            this.pnlQOS.Size = new System.Drawing.Size(272, 28);
            this.pnlQOS.TabIndex = 43;
            // 
            // cmbQOS
            // 
            this.cmbQOS.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbQOS.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbQOS.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.cmbQOS.FormattingEnabled = true;
            this.cmbQOS.Items.AddRange(new object[] {
            "QOS0",
            "QOS1",
            "QOS2"});
            this.cmbQOS.Location = new System.Drawing.Point(120, 3);
            this.cmbQOS.Name = "cmbQOS";
            this.cmbQOS.Size = new System.Drawing.Size(150, 21);
            this.cmbQOS.TabIndex = 3;
            // 
            // lblQOS
            // 
            this.lblQOS.AutoSize = true;
            this.lblQOS.Location = new System.Drawing.Point(30, 7);
            this.lblQOS.Margin = new System.Windows.Forms.Padding(3);
            this.lblQOS.Name = "lblQOS";
            this.lblQOS.Size = new System.Drawing.Size(29, 13);
            this.lblQOS.TabIndex = 23;
            this.lblQOS.Text = "Qos:";
            // 
            // imageQOS
            // 
            this.imageQOS.Image = global::com.mobius.software.windows.iotbroker.ui.win7.Properties.Resources.icon_qos_small;
            this.imageQOS.Location = new System.Drawing.Point(5, 0);
            this.imageQOS.Margin = new System.Windows.Forms.Padding(1);
            this.imageQOS.Name = "imageQOS";
            this.imageQOS.Size = new System.Drawing.Size(24, 25);
            this.imageQOS.TabIndex = 0;
            this.imageQOS.TabStop = false;
            // 
            // pnlTopic
            // 
            this.pnlTopic.Controls.Add(this.txtTopic);
            this.pnlTopic.Controls.Add(this.lblWillTopic);
            this.pnlTopic.Controls.Add(this.imageTopic);
            this.pnlTopic.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTopic.Location = new System.Drawing.Point(2, 54);
            this.pnlTopic.Margin = new System.Windows.Forms.Padding(1);
            this.pnlTopic.Name = "pnlTopic";
            this.pnlTopic.Size = new System.Drawing.Size(272, 28);
            this.pnlTopic.TabIndex = 42;
            // 
            // txtTopic
            // 
            this.txtTopic.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtTopic.Location = new System.Drawing.Point(120, 4);
            this.txtTopic.Name = "txtTopic";
            this.txtTopic.Size = new System.Drawing.Size(155, 20);
            this.txtTopic.TabIndex = 2;
            // 
            // lblWillTopic
            // 
            this.lblWillTopic.AutoSize = true;
            this.lblWillTopic.Location = new System.Drawing.Point(30, 7);
            this.lblWillTopic.Margin = new System.Windows.Forms.Padding(3);
            this.lblWillTopic.Name = "lblWillTopic";
            this.lblWillTopic.Size = new System.Drawing.Size(37, 13);
            this.lblWillTopic.TabIndex = 22;
            this.lblWillTopic.Text = "Topic:";
            // 
            // imageTopic
            // 
            this.imageTopic.Image = global::com.mobius.software.windows.iotbroker.ui.win7.Properties.Resources.icon_will_topic_small;
            this.imageTopic.Location = new System.Drawing.Point(5, 0);
            this.imageTopic.Margin = new System.Windows.Forms.Padding(1);
            this.imageTopic.Name = "imageTopic";
            this.imageTopic.Size = new System.Drawing.Size(24, 25);
            this.imageTopic.TabIndex = 0;
            this.imageTopic.TabStop = false;
            // 
            // pnlContent
            // 
            this.pnlContent.Controls.Add(this.txtContent);
            this.pnlContent.Controls.Add(this.lblContent);
            this.pnlContent.Controls.Add(this.imageContent);
            this.pnlContent.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlContent.Location = new System.Drawing.Point(2, 23);
            this.pnlContent.Margin = new System.Windows.Forms.Padding(1);
            this.pnlContent.Name = "pnlContent";
            this.pnlContent.Size = new System.Drawing.Size(272, 28);
            this.pnlContent.TabIndex = 41;
            // 
            // txtContent
            // 
            this.txtContent.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtContent.Location = new System.Drawing.Point(120, 4);
            this.txtContent.Name = "txtContent";
            this.txtContent.ReadOnly = true;
            this.txtContent.Size = new System.Drawing.Size(155, 20);
            this.txtContent.TabIndex = 1;
            this.txtContent.Click += new System.EventHandler(this.txtContent_Click);
            // 
            // lblContent
            // 
            this.lblContent.AutoSize = true;
            this.lblContent.Location = new System.Drawing.Point(30, 7);
            this.lblContent.Margin = new System.Windows.Forms.Padding(3);
            this.lblContent.Name = "lblContent";
            this.lblContent.Size = new System.Drawing.Size(47, 13);
            this.lblContent.TabIndex = 21;
            this.lblContent.Text = "Content:";
            // 
            // imageContent
            // 
            this.imageContent.Image = global::com.mobius.software.windows.iotbroker.ui.win7.Properties.Resources.icon_will_small;
            this.imageContent.Location = new System.Drawing.Point(5, 0);
            this.imageContent.Margin = new System.Windows.Forms.Padding(1);
            this.imageContent.Name = "imageContent";
            this.imageContent.Size = new System.Drawing.Size(24, 25);
            this.imageContent.TabIndex = 0;
            this.imageContent.TabStop = false;
            // 
            // pnlNewMessageHeader
            // 
            this.pnlNewMessageHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(215)))), ((int)(((byte)(219)))));
            this.pnlNewMessageHeader.Controls.Add(this.lblNewMessage);
            this.pnlNewMessageHeader.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlNewMessageHeader.Location = new System.Drawing.Point(1, 1);
            this.pnlNewMessageHeader.Margin = new System.Windows.Forms.Padding(0);
            this.pnlNewMessageHeader.Name = "pnlNewMessageHeader";
            this.pnlNewMessageHeader.Size = new System.Drawing.Size(274, 20);
            this.pnlNewMessageHeader.TabIndex = 47;
            // 
            // lblNewMessage
            // 
            this.lblNewMessage.AutoSize = true;
            this.lblNewMessage.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewMessage.ForeColor = System.Drawing.Color.Black;
            this.lblNewMessage.Location = new System.Drawing.Point(7, 3);
            this.lblNewMessage.Name = "lblNewMessage";
            this.lblNewMessage.Size = new System.Drawing.Size(86, 13);
            this.lblNewMessage.TabIndex = 0;
            this.lblNewMessage.Text = "New Message";
            // 
            // tabMessagesList
            // 
            this.tabMessagesList.BackColor = System.Drawing.Color.White;
            this.tabMessagesList.Controls.Add(this.messagesGrid);
            this.tabMessagesList.Location = new System.Drawing.Point(4, 5);
            this.tabMessagesList.Name = "tabMessagesList";
            this.tabMessagesList.Size = new System.Drawing.Size(276, 282);
            this.tabMessagesList.TabIndex = 2;
            this.tabMessagesList.Text = "Messages List";
            // 
            // messagesGrid
            // 
            this.messagesGrid.AllowUserToAddRows = false;
            this.messagesGrid.AllowUserToDeleteRows = false;
            this.messagesGrid.AllowUserToResizeColumns = false;
            this.messagesGrid.AllowUserToResizeRows = false;
            this.messagesGrid.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.messagesGrid.BackgroundColor = System.Drawing.Color.White;
            this.messagesGrid.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.messagesGrid.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.messagesGrid.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.messagesGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.messagesGrid.ColumnHeadersVisible = false;
            this.messagesGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.MessageContentColumn,
            this.MessageTypeColumn});
            this.messagesGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.messagesGrid.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(156)))), ((int)(((byte)(161)))));
            this.messagesGrid.Location = new System.Drawing.Point(0, 0);
            this.messagesGrid.MultiSelect = false;
            this.messagesGrid.Name = "messagesGrid";
            this.messagesGrid.ReadOnly = true;
            this.messagesGrid.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.messagesGrid.RowHeadersVisible = false;
            this.messagesGrid.RowTemplate.DefaultCellStyle.BackColor = System.Drawing.Color.White;
            this.messagesGrid.RowTemplate.DefaultCellStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.messagesGrid.RowTemplate.DefaultCellStyle.ForeColor = System.Drawing.Color.Black;
            this.messagesGrid.RowTemplate.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.White;
            this.messagesGrid.RowTemplate.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.Black;
            this.messagesGrid.RowTemplate.DefaultCellStyle.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.messagesGrid.RowTemplate.DividerHeight = 1;
            this.messagesGrid.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.messagesGrid.Size = new System.Drawing.Size(276, 282);
            this.messagesGrid.TabIndex = 0;
            this.messagesGrid.CellPainting += new System.Windows.Forms.DataGridViewCellPaintingEventHandler(this.messagesGrid_CellPainting);
            this.messagesGrid.RowsAdded += new System.Windows.Forms.DataGridViewRowsAddedEventHandler(this.messagesGrid_RowsAdded);
            // 
            // MessageContentColumn
            // 
            this.MessageContentColumn.FillWeight = 176F;
            this.MessageContentColumn.HeaderText = "Content";
            this.MessageContentColumn.MinimumWidth = 176;
            this.MessageContentColumn.Name = "MessageContentColumn";
            this.MessageContentColumn.ReadOnly = true;
            this.MessageContentColumn.Width = 176;
            // 
            // MessageTypeColumn
            // 
            this.MessageTypeColumn.FillWeight = 110F;
            this.MessageTypeColumn.HeaderText = "Type";
            this.MessageTypeColumn.MinimumWidth = 110;
            this.MessageTypeColumn.Name = "MessageTypeColumn";
            this.MessageTypeColumn.ReadOnly = true;
            this.MessageTypeColumn.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.MessageTypeColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.MessageTypeColumn.Width = 110;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(284, 362);
            this.Controls.Add(this.mainPanel);
            this.Controls.Add(this.pnlSeparator);
            this.Controls.Add(this.tabsLayout);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "IOTBroker.cloud demo client";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.tabsLayout.ResumeLayout(false);
            this.mainPanel.ResumeLayout(false);
            this.tabTopicsList.ResumeLayout(false);
            this.topicsLayout.ResumeLayout(false);
            this.topicsLayout.PerformLayout();
            this.pnlNewTopicQOS.ResumeLayout(false);
            this.pnlNewTopicQOS.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.iconNewTopicQOS)).EndInit();
            this.pnlNewTopicName.ResumeLayout(false);
            this.pnlNewTopicName.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imageNewTopicName)).EndInit();
            this.pnlNewTopicHeader.ResumeLayout(false);
            this.pnlNewTopicHeader.PerformLayout();
            this.pnlTopicsListHeader.ResumeLayout(false);
            this.pnlTopicsListHeader.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.topicsGrid)).EndInit();
            this.tabSendMessage.ResumeLayout(false);
            this.newMessageLayout.ResumeLayout(false);
            this.newMessageLayout.PerformLayout();
            this.pnlDuplicate.ResumeLayout(false);
            this.pnlDuplicate.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imageDuplicate)).EndInit();
            this.pnlRetain.ResumeLayout(false);
            this.pnlRetain.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imageRetain)).EndInit();
            this.pnlQOS.ResumeLayout(false);
            this.pnlQOS.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imageQOS)).EndInit();
            this.pnlTopic.ResumeLayout(false);
            this.pnlTopic.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imageTopic)).EndInit();
            this.pnlContent.ResumeLayout(false);
            this.pnlContent.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imageContent)).EndInit();
            this.pnlNewMessageHeader.ResumeLayout(false);
            this.pnlNewMessageHeader.PerformLayout();
            this.tabMessagesList.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.messagesGrid)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.TableLayoutPanel tabsLayout;
        private System.Windows.Forms.Button btnTopicsList;
        private System.Windows.Forms.Button btnSendMessage;
        private System.Windows.Forms.Button btnMessagesList;
        private System.Windows.Forms.Panel pnlSeparator;
        private TabContainer mainPanel;
        private System.Windows.Forms.TabPage tabTopicsList;
        private System.Windows.Forms.TabPage tabSendMessage;
        private System.Windows.Forms.TabPage tabMessagesList;
        private System.Windows.Forms.TableLayoutPanel newMessageLayout;
        private System.Windows.Forms.Panel pnlNewMessageHeader;
        private System.Windows.Forms.Label lblNewMessage;
        private System.Windows.Forms.Panel pnlContent;
        private System.Windows.Forms.TextBox txtContent;
        private System.Windows.Forms.Label lblContent;
        private System.Windows.Forms.PictureBox imageContent;
        private System.Windows.Forms.Panel pnlTopic;
        private System.Windows.Forms.TextBox txtTopic;
        private System.Windows.Forms.Label lblWillTopic;
        private System.Windows.Forms.PictureBox imageTopic;
        private System.Windows.Forms.Panel pnlQOS;
        private FlatCombo cmbQOS;
        private System.Windows.Forms.Label lblQOS;
        private System.Windows.Forms.PictureBox imageQOS;
        private System.Windows.Forms.Panel pnlRetain;
        private System.Windows.Forms.CheckBox chkRetain;
        private System.Windows.Forms.Label lblRetain;
        private System.Windows.Forms.PictureBox imageRetain;
        private System.Windows.Forms.Panel pnlDuplicate;
        private System.Windows.Forms.CheckBox chkDuplicate;
        private System.Windows.Forms.Label lblDuplicate;
        private System.Windows.Forms.PictureBox imageDuplicate;
        private System.Windows.Forms.Button sendButton;
        private System.Windows.Forms.TableLayoutPanel topicsLayout;
        private System.Windows.Forms.Button addTopicButton;
        private System.Windows.Forms.Panel pnlTopicsListHeader;
        private System.Windows.Forms.Label lblTopicsListHeader;
        private System.Windows.Forms.Panel pnlNewTopicHeader;
        private System.Windows.Forms.Label lblNewTopicHeader;
        private System.Windows.Forms.Panel pnlNewTopicQOS;
        private FlatCombo cmbNewTopicQOS;
        private System.Windows.Forms.Label lblTopicQOS;
        private System.Windows.Forms.PictureBox iconNewTopicQOS;
        private System.Windows.Forms.Panel pnlNewTopicName;
        private System.Windows.Forms.TextBox txtNewTopicName;
        private System.Windows.Forms.Label lblNewTopicName;
        private System.Windows.Forms.PictureBox imageNewTopicName;
        private System.Windows.Forms.Button btnLogout;
        private System.Windows.Forms.DataGridViewImageColumn dataGridViewImageColumn1;
        private System.Windows.Forms.DataGridView messagesGrid;
        private System.Windows.Forms.DataGridView topicsGrid;
        private System.Windows.Forms.DataGridViewTextBoxColumn TopicName;
        private System.Windows.Forms.DataGridViewImageColumn TopicQOS;
        private System.Windows.Forms.DataGridViewImageColumn TopicDelete;
        private System.Windows.Forms.DataGridViewTextBoxColumn MessageContentColumn;
        private System.Windows.Forms.DataGridViewImageColumn MessageTypeColumn;
    }
}