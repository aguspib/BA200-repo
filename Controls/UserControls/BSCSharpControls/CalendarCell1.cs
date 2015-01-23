using System;
using System.Collections.Generic;
using System.Windows.Forms;


namespace Biosystems.Ax00.Controls.UserControls
{
/// <summary>
    /// Copy from http://www.c-sharpcorner.com/UploadFile/ankurmee/1388/
    /// http://msdn.microsoft.com/en-us/library/7tas5c80.aspx#Y214
/// </summary>
    public class CalendarCell1 : DataGridViewTextBoxCell
    {

        public CalendarCell1()
            : base()
        {
            // Use the short date format.
            this.Style.Format = "hh:mm tt";

        }

        public override void InitializeEditingControl(int rowIndex, object
            initialFormattedValue, DataGridViewCellStyle dataGridViewCellStyle)
        {
            // Set the value of the editing control to the current cell value.
            base.InitializeEditingControl(rowIndex, initialFormattedValue,
                dataGridViewCellStyle);
            CalendarEditingControl1 ctl = DataGridView.EditingControl as CalendarEditingControl1;
            // Use the default row value when Value property is null.

            if (this.RowIndex < 0) 
                return;
            if ((object.ReferenceEquals(this.Value, DBNull.Value))) 
                return;
            if (this.Value == null) 
                return;
            if (Convert.ToString(this.Value) == string.Empty) 
                return;
            try
            {
                ctl.Value = DateTime.Parse(this.Value.ToString());
            }
            catch (Exception)
            {

            }

            //if (this.Value == null)
            //{
            //    ctl.Value = (DateTime)this.DefaultNewRowValue;
            //}
            //else
            //{
            //    ctl.Value = (DateTime)this.Value;
            //}
        }

        public override Type EditType
        {
            get
            {
                // Return the type of the editing control that CalendarCell uses.
                return typeof(CalendarEditingControl1);
            }
        }

        public override Type ValueType
        {
            get
            {
                // Return the type of the value that CalendarCell contains.

                return typeof(DateTime);
            }
        }

        public override object DefaultNewRowValue
        {
            get
            {
                // Use the current date and time as the default value.
                return DateTime.Now;
            }
        }
    }
}

namespace Biosystems.Ax00.Controls.UserControls
{
    public class CalendarTimeColumn : DataGridViewColumn
    {
        public CalendarTimeColumn()
            : base(new CalendarCell1())
        {
        }

        public override DataGridViewCell CellTemplate
        {
            get
            {
                return base.CellTemplate;
            }
            set
            {
                // Ensure that the cell used for the template is a CalendarCell.
                if (value != null &&
                    !value.GetType().IsAssignableFrom(typeof(CalendarCell1)))
                {
                    throw new InvalidCastException("Must be a CalendarCell");
                }
                base.CellTemplate = value;
            }
        }
    }
}

namespace Biosystems.Ax00.Controls.UserControls
{
    class CalendarEditingControl1 : DateTimePicker, IDataGridViewEditingControl
    {
        private DataGridView dataGridViewControl;
        private bool valueIsChanged = false;
        private int rowIndexNum;

        public CalendarEditingControl1()
        {
            this.Format = DateTimePickerFormat.Time;
        }

        public object EditingControlFormattedValue
        {
            get
            {
                return this.Value.ToShortTimeString();
            }
            set
            {
                if (value is string)
                {
                    this.Value = DateTime.Parse(System.Convert.ToString(value));
                }
            }
        }

        public object GetEditingControlFormattedValue(DataGridViewDataErrorContexts context)
        {
            return this.Value.ToShortTimeString(); //Return Only Time from DateTime
        }

        public void ApplyCellStyleToEditingControl(DataGridViewCellStyle dataGridViewCellStyle)
        {
            this.Font = dataGridViewCellStyle.Font;
            this.ShowUpDown = true;
            this.CalendarForeColor = dataGridViewCellStyle.ForeColor;
            this.CalendarMonthBackground = dataGridViewCellStyle.BackColor;
        }

        public int EditingControlRowIndex
        {
            get
            {
                return rowIndexNum;
            }
            set
            {
                rowIndexNum = value;
            }
        }

        public bool EditingControlWantsInputKey(Keys key, bool dataGridViewWantsInputKey)
        {
            return (key == Keys.Left | key == Keys.Up | key == Keys.Down | key == Keys.Right | key == Keys.Home | key == Keys.End | key == Keys.PageDown | key == Keys.PageUp);
        }

        public void PrepareEditingControlForEdit(bool selectAll)
        {

        }

        public bool RepositionEditingControlOnValueChange
        {
            get
            {
                return false;
            }
        }

        public DataGridView EditingControlDataGridView
        {
            get
            {
                return dataGridViewControl;
            }
            set
            {
                dataGridViewControl = value;
            }
        }

        public bool EditingControlValueChanged
        {
            get
            {
                return valueIsChanged;
            }
            set
            {
                valueIsChanged = value;
            }
        }

        public Cursor EditingControlCursor
        {
            get
            {
                return base.Cursor;
            }
        }

        protected override void OnValueChanged(System.EventArgs eventargs)
        {
            valueIsChanged = true;
            this.EditingControlDataGridView.NotifyCurrentCellDirty(true);
            base.OnValueChanged(eventargs);
        }

        #region IDataGridViewEditingControl Members

        Cursor IDataGridViewEditingControl.EditingPanelCursor
        {
            get { return base.Cursor; }//throw new Exception("The method or operation is not implemented."); }
        }
        #endregion
    }
}
