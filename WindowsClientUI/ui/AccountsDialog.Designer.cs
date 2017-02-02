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
    partial class AccountsDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AccountsDialog));
            this.mainLayout = new System.Windows.Forms.TableLayoutPanel();
            this.newAcccountButton = new System.Windows.Forms.Button();
            this.accountsList = new System.Windows.Forms.DataGridView();
            this.dataGridViewImageColumn1 = new System.Windows.Forms.DataGridViewImageColumn();
            this.DeleteColumn = new System.Windows.Forms.DataGridViewImageColumn();
            this.mainLayout.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.accountsList)).BeginInit();
            this.SuspendLayout();
            // 
            // mainLayout
            // 
            this.mainLayout.ColumnCount = 1;
            this.mainLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.mainLayout.Controls.Add(this.newAcccountButton, 0, 1);
            this.mainLayout.Controls.Add(this.accountsList, 0, 0);
            this.mainLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainLayout.Location = new System.Drawing.Point(0, 0);
            this.mainLayout.Margin = new System.Windows.Forms.Padding(0);
            this.mainLayout.Name = "mainLayout";
            this.mainLayout.RowCount = 2;
            this.mainLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 90F));
            this.mainLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.mainLayout.Size = new System.Drawing.Size(284, 362);
            this.mainLayout.TabIndex = 0;
            // 
            // newAcccountButton
            // 
            this.newAcccountButton.AutoSize = true;
            this.newAcccountButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(118)))), ((int)(((byte)(219)))));
            this.newAcccountButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.newAcccountButton.FlatAppearance.BorderSize = 0;
            this.newAcccountButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.newAcccountButton.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.newAcccountButton.ForeColor = System.Drawing.Color.White;
            this.newAcccountButton.Location = new System.Drawing.Point(0, 325);
            this.newAcccountButton.Margin = new System.Windows.Forms.Padding(0);
            this.newAcccountButton.Name = "newAcccountButton";
            this.newAcccountButton.Size = new System.Drawing.Size(284, 37);
            this.newAcccountButton.TabIndex = 0;
            this.newAcccountButton.Text = "Add New Account";
            this.newAcccountButton.UseVisualStyleBackColor = false;
            this.newAcccountButton.Click += new System.EventHandler(this.newAcccountButton_Click);
            // 
            // accountsList
            // 
            this.accountsList.AllowUserToAddRows = false;
            this.accountsList.AllowUserToDeleteRows = false;
            this.accountsList.AllowUserToResizeColumns = false;
            this.accountsList.AllowUserToResizeRows = false;
            this.accountsList.BackgroundColor = System.Drawing.Color.White;
            this.accountsList.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.accountsList.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.accountsList.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.accountsList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.accountsList.ColumnHeadersVisible = false;
            this.accountsList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.DeleteColumn});
            this.accountsList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.accountsList.GridColor = System.Drawing.Color.White;
            this.accountsList.Location = new System.Drawing.Point(0, 0);
            this.accountsList.Margin = new System.Windows.Forms.Padding(0);
            this.accountsList.MultiSelect = false;
            this.accountsList.Name = "accountsList";
            this.accountsList.ReadOnly = true;
            this.accountsList.RowHeadersVisible = false;
            this.accountsList.RowTemplate.Height = 52;
            this.accountsList.Size = new System.Drawing.Size(284, 325);
            this.accountsList.TabIndex = 1;
            this.accountsList.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.accountsList_CellClick);
            this.accountsList.CellMouseEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.accountsList_CellMouseEnter);
            this.accountsList.CellMouseLeave += new System.Windows.Forms.DataGridViewCellEventHandler(this.accountsList_CellMouseLeave);
            // 
            // dataGridViewImageColumn1
            // 
            this.dataGridViewImageColumn1.HeaderText = "Delete";
            this.dataGridViewImageColumn1.Image = global::com.mobius.software.windows.iotbroker.ui.win7.Properties.Resources.ic_delete;
            this.dataGridViewImageColumn1.MinimumWidth = 25;
            this.dataGridViewImageColumn1.Name = "dataGridViewImageColumn1";
            this.dataGridViewImageColumn1.Width = 25;
            // 
            // DeleteColumn
            // 
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.Transparent;
            dataGridViewCellStyle1.NullValue = ((object)(resources.GetObject("dataGridViewCellStyle1.NullValue")));
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.Transparent;
            this.DeleteColumn.DefaultCellStyle = dataGridViewCellStyle1;
            this.DeleteColumn.HeaderText = "Delete";
            this.DeleteColumn.Image = ((System.Drawing.Image)(resources.GetObject("DeleteColumn.Image")));
            this.DeleteColumn.MinimumWidth = 25;
            this.DeleteColumn.Name = "DeleteColumn";
            this.DeleteColumn.ReadOnly = true;
            this.DeleteColumn.Width = 25;
            // 
            // AccountsDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 362);
            this.Controls.Add(this.mainLayout);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AccountsDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Please choose your account";
            this.mainLayout.ResumeLayout(false);
            this.mainLayout.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.accountsList)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel mainLayout;
        private System.Windows.Forms.Button newAcccountButton;
        private System.Windows.Forms.DataGridView accountsList;
        private System.Windows.Forms.DataGridViewImageColumn dataGridViewImageColumn1;
        private System.Windows.Forms.DataGridViewImageColumn DeleteColumn;
    }
}