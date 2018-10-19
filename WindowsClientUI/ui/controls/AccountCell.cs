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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace com.mobius.software.windows.iotbroker.ui.win7.ui.controls
{    
    public class AccountCell : DataGridViewCell
    {
        private Pen selectedColorPen = new Pen(Color.FromArgb(221, 221, 221));
        private Pen whiteColorPen = new Pen(Color.White);
        private SolidBrush grayBrush = new SolidBrush(Color.FromArgb(219, 227, 229));
        private SolidBrush darkGrayBrush = new SolidBrush(Color.FromArgb(148, 156, 161));
        private Pen pen = new Pen(Color.FromArgb(219, 227, 229));

        protected override void Paint(Graphics graphics, Rectangle clipBounds, Rectangle cellBounds, int rowIndex, DataGridViewElementStates cellState, object value, object formattedValue, string errorText, DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle, DataGridViewPaintParts paintParts)
        {
            SolidBrush titleBrush = darkGrayBrush;
            
            graphics.DrawRectangle(whiteColorPen, clipBounds);
            Font titleFont = new Font(FontFamily.GenericSansSerif, 10, FontStyle.Bold);
            Font regularFont = new Font(FontFamily.GenericSansSerif, 9, FontStyle.Regular);
            if (value != null)
            {
                Account account = (Account)value;
                graphics.DrawImage(com.mobius.software.windows.iotbroker.ui.win7.Properties.Resources.icon_user_name, new PointF(cellBounds.X + 5, cellBounds.Y + 3));
                graphics.DrawString(account.Protocol.ToString().Replace("_PROTOCOL",""), titleFont, titleBrush, new PointF(cellBounds.Left + 55, cellBounds.Top + 5));
                graphics.DrawString(account.ClientID, regularFont, darkGrayBrush, new PointF(cellBounds.Left + 55, cellBounds.Top + 19));
                graphics.DrawString(account.ServerHost + ":" + account.ServerPort, regularFont, darkGrayBrush, new PointF(cellBounds.Left +55, cellBounds.Top + 33));
                graphics.DrawLine(pen, new PointF(cellBounds.Left + 5, cellBounds.Bottom - 1), new Point(cellBounds.Right + 25, cellBounds.Bottom - 1));
            }
        }

        public override Type FormattedValueType
        {
            get
            {
                return typeof(Account);
            }
        }

        public override Type ValueType
        {
            get
            {
                return typeof(Account);
            }
        }        

        protected override object GetFormattedValue(object value, int rowIndex, ref DataGridViewCellStyle cellStyle, TypeConverter valueTypeConverter, TypeConverter formattedValueTypeConverter, DataGridViewDataErrorContexts context)
        {
            return null;
        }
    }
}