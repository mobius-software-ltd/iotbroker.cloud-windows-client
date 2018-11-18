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
 
using com.mobius.software.windows.iotbroker.ui.win7.dal;
using com.mobius.software.windows.iotbroker.ui.win7.ui.controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
    public partial class AccountsDialog : Form
    {
        public AccountsDialog()
        {
            InitializeComponent();
            AccountColumn col = new AccountColumn();
            MQTTModel mqtt = new MQTTModel();

            this.accountsList.AutoGenerateColumns = false;            
            col.DataPropertyName = "DataItem";
            col.Width = this.accountsList.Width-30;      
                              
            this.accountsList.Columns.Insert(0,col);
            this.accountsList.DataSource = mqtt.Accounts.Select(o => new ViewModel() { DataItem = o }).ToList();            
        }

        class ViewModel
        {
            public Account DataItem { get; set; }
        }

        private void accountsList_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 0)
            {
                Account currAccount = ((ViewModel)accountsList.Rows[e.RowIndex].DataBoundItem).DataItem;
                currAccount.IsDefault = true;
                MQTTModel mqtt = new MQTTModel();
                mqtt.Accounts.Attach(currAccount);
                var entry = mqtt.Entry(currAccount);
                entry.Property(a => a.IsDefault).IsModified = true;
                mqtt.SaveChanges();
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                Account currAccount = ((ViewModel)accountsList.Rows[e.RowIndex].DataBoundItem).DataItem;
                MQTTModel mqtt = new MQTTModel();
                mqtt.Accounts.Attach(currAccount);
                mqtt.Accounts.Remove(currAccount);
                mqtt.SaveChanges();
                this.accountsList.DataSource = mqtt.Accounts.Select(o => new ViewModel() { DataItem = o }).ToList();
            }
        }

        private void accountsList_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            accountsList.Cursor = Cursors.Hand;
        }

        private void accountsList_CellMouseLeave(object sender, DataGridViewCellEventArgs e)
        {
            accountsList.Cursor = Cursors.Default;
        }

        private void newAcccountButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Ignore;
            this.Close();
        }
    }
}