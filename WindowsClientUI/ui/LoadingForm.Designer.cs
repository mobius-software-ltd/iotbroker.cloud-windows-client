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
    partial class LoadingForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoadingForm));
            this.mainLayout = new System.Windows.Forms.TableLayoutPanel();
            this.imagesPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.logo = new System.Windows.Forms.PictureBox();
            this.title = new System.Windows.Forms.PictureBox();
            this.progressPanel = new System.Windows.Forms.Panel();
            this.progressText = new System.Windows.Forms.Label();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.mainLayout.SuspendLayout();
            this.imagesPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.logo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.title)).BeginInit();
            this.progressPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainLayout
            // 
            this.mainLayout.BackgroundImage = global::com.mobius.software.windows.iotbroker.ui.win7.Properties.Resources.iot_broker_background;
            this.mainLayout.ColumnCount = 1;
            this.mainLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.mainLayout.Controls.Add(this.imagesPanel, 0, 0);
            this.mainLayout.Controls.Add(this.progressPanel, 0, 1);
            this.mainLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainLayout.Location = new System.Drawing.Point(0, 0);
            this.mainLayout.Name = "mainLayout";
            this.mainLayout.RowCount = 2;
            this.mainLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 80F));
            this.mainLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.mainLayout.Size = new System.Drawing.Size(284, 362);
            this.mainLayout.TabIndex = 0;
            // 
            // imagesPanel
            // 
            this.imagesPanel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.imagesPanel.BackColor = System.Drawing.Color.Transparent;
            this.imagesPanel.Controls.Add(this.logo);
            this.imagesPanel.Controls.Add(this.title);
            this.imagesPanel.Location = new System.Drawing.Point(3, 19);
            this.imagesPanel.Name = "imagesPanel";
            this.imagesPanel.Size = new System.Drawing.Size(278, 250);
            this.imagesPanel.TabIndex = 2;
            // 
            // logo
            // 
            this.logo.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.logo.Image = global::com.mobius.software.windows.iotbroker.ui.win7.Properties.Resources.loading_icon;
            this.logo.InitialImage = global::com.mobius.software.windows.iotbroker.ui.win7.Properties.Resources.loading_icon;
            this.logo.Location = new System.Drawing.Point(68, 3);
            this.logo.Margin = new System.Windows.Forms.Padding(68, 3, 0, 3);
            this.logo.Name = "logo";
            this.logo.Size = new System.Drawing.Size(145, 164);
            this.logo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.logo.TabIndex = 0;
            this.logo.TabStop = false;
            // 
            // title
            // 
            this.title.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.title.Image = global::com.mobius.software.windows.iotbroker.ui.win7.Properties.Resources.loading_text;
            this.title.Location = new System.Drawing.Point(11, 170);
            this.title.Margin = new System.Windows.Forms.Padding(11, 0, 3, 3);
            this.title.Name = "title";
            this.title.Size = new System.Drawing.Size(257, 48);
            this.title.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.title.TabIndex = 1;
            this.title.TabStop = false;
            // 
            // progressPanel
            // 
            this.progressPanel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.progressPanel.BackColor = System.Drawing.Color.Transparent;
            this.progressPanel.Controls.Add(this.progressText);
            this.progressPanel.Controls.Add(this.progressBar);
            this.progressPanel.Location = new System.Drawing.Point(42, 305);
            this.progressPanel.Name = "progressPanel";
            this.progressPanel.Size = new System.Drawing.Size(200, 40);
            this.progressPanel.TabIndex = 3;
            // 
            // progressText
            // 
            this.progressText.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.progressText.BackColor = System.Drawing.Color.Transparent;
            this.progressText.Font = new System.Drawing.Font("Dotum", 8.25F);
            this.progressText.ForeColor = System.Drawing.Color.Black;
            this.progressText.Location = new System.Drawing.Point(0, 20);
            this.progressText.Name = "progressText";
            this.progressText.Size = new System.Drawing.Size(200, 20);
            this.progressText.TabIndex = 1;
            this.progressText.Text = "Checking accounts";
            this.progressText.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // progressBar
            // 
            this.progressBar.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.progressBar.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.progressBar.Location = new System.Drawing.Point(0, 0);
            this.progressBar.Margin = new System.Windows.Forms.Padding(0);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(200, 20);
            this.progressBar.TabIndex = 0;
            // 
            // LoadingForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 362);
            this.Controls.Add(this.mainLayout);
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "LoadingForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Loading";
            this.Load += new System.EventHandler(this.LoadingForm_Load);
            this.mainLayout.ResumeLayout(false);
            this.imagesPanel.ResumeLayout(false);
            this.imagesPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.logo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.title)).EndInit();
            this.progressPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel mainLayout;
        private System.Windows.Forms.FlowLayoutPanel imagesPanel;
        private System.Windows.Forms.PictureBox logo;
        private System.Windows.Forms.PictureBox title;
        private System.Windows.Forms.Panel progressPanel;
        private System.Windows.Forms.Label progressText;
        private System.Windows.Forms.ProgressBar progressBar;
    }
}