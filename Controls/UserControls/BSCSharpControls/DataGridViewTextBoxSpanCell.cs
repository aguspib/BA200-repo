//Taken from: http://www.codeproject.com/KB/grid/SpanDataGridViewCell.aspx
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace Biosystems.Ax00.Controls.UserControls
{
    public class DataGridViewTextBoxSpanCell : DataGridViewTextBoxCell
    {
        #region Fields
        private int m_ColumnSpan = 1;
        private int m_RowSpan = 1;
        private DataGridViewTextBoxSpanCell m_OwnerCell;
        #endregion

        #region Properties
        public int ColumnSpan
        {
            get { return m_ColumnSpan; }
            set
            {
                if (DataGridView == null || m_OwnerCell != null)
                    return;
                if (value < 1 || ColumnIndex + value - 1 >= DataGridView.ColumnCount)
                    throw new System.ArgumentOutOfRangeException("value");
                if (m_ColumnSpan != value)
                    SetSpan(value, m_RowSpan);
            }
        }
        public int RowSpan
        {
            get { return m_RowSpan; }
            set
            {
                if (DataGridView == null || m_OwnerCell != null)
                    return;
                if (value < 1 || RowIndex + value - 1 >= DataGridView.RowCount)
                    throw new System.ArgumentOutOfRangeException("value");
                if (m_RowSpan != value)
                    SetSpan(m_ColumnSpan, value);
            }
        }
        public DataGridViewTextBoxSpanCell OwnerCell
        {
            get { return m_OwnerCell; }
            private set { m_OwnerCell = value; }
        }
        public override bool ReadOnly
        {
            get
            {
                return base.ReadOnly;
            }
            set
            {
                base.ReadOnly = value;

                if (m_OwnerCell == null
                    && (m_ColumnSpan > 1 || m_RowSpan > 1)
                    && DataGridView != null)
                {
                    foreach (var col in Enumerable.Range(ColumnIndex, m_ColumnSpan))
                        foreach (var row in Enumerable.Range(RowIndex, m_RowSpan))
                            if (col != ColumnIndex || row != RowIndex)
                            {
                                DataGridView[col, row].ReadOnly = value;
                            }
                }
            }
        }
        #endregion

        #region ctor
        public DataGridViewTextBoxSpanCell()
        {

        }
        #endregion

        #region Painting.
        private void InternalPaint(System.Drawing.Graphics graphics, System.Drawing.Rectangle clipBounds, System.Drawing.Rectangle cellBounds, int rowIndex, DataGridViewElementStates cellState, object value, object formattedValue, string errorText, DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle, DataGridViewPaintParts paintParts)
        {
            base.Paint(graphics, clipBounds, cellBounds, rowIndex, cellState, value, formattedValue, errorText, cellStyle, advancedBorderStyle, paintParts);
        }
        protected override void Paint(System.Drawing.Graphics graphics, System.Drawing.Rectangle clipBounds, System.Drawing.Rectangle cellBounds, int rowIndex, DataGridViewElementStates cellState, object value, object formattedValue, string errorText, DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle, DataGridViewPaintParts paintParts)
        {
            if (m_OwnerCell != null && m_OwnerCell.DataGridView == null)
                m_OwnerCell = null; //owner cell was removed.

            if (DataGridView == null
                || (m_OwnerCell == null && m_ColumnSpan == 1 && m_RowSpan == 1))
            {
                base.Paint(graphics, clipBounds, cellBounds, rowIndex, cellState, value, formattedValue, errorText, cellStyle, advancedBorderStyle,
                        paintParts);
                return;
            }

            var ownerCell = this;
            var columnIndex = ColumnIndex;
            var columnSpan = m_ColumnSpan;
            var rowSpan = m_RowSpan;
            if (m_OwnerCell != null)
            {
                ownerCell = m_OwnerCell;
                columnIndex = m_OwnerCell.ColumnIndex;
                rowIndex = m_OwnerCell.RowIndex;
                columnSpan = m_OwnerCell.ColumnSpan;
                rowSpan = m_OwnerCell.RowSpan;
                value = m_OwnerCell.GetValue(rowIndex);
                errorText = m_OwnerCell.GetErrorText(rowIndex);
                cellState = m_OwnerCell.State;
                cellStyle = m_OwnerCell.GetInheritedStyle(null, rowIndex, true);
                formattedValue = m_OwnerCell.GetFormattedValue(value,
                    rowIndex, ref cellStyle, null, null, DataGridViewDataErrorContexts.Display);
            }
            if (CellsRegionContainsSelectedCell(columnIndex, rowIndex, columnSpan, rowSpan))
                cellState |= DataGridViewElementStates.Selected;
            cellBounds = GetSpannedCellBoundsFromChildCellBounds(
                this,
                cellBounds,
                advancedBorderStyle.Left != DataGridViewAdvancedCellBorderStyle.None,
                advancedBorderStyle.Top != DataGridViewAdvancedCellBorderStyle.None);
            clipBounds = GetSpannedCellClipBounds(ownerCell, cellBounds,
                advancedBorderStyle.Left != DataGridViewAdvancedCellBorderStyle.None,
                advancedBorderStyle.Top != DataGridViewAdvancedCellBorderStyle.None);
            using (var g = DataGridView.CreateGraphics())
            {
                g.SetClip(clipBounds);
                //Paint the content.
                ownerCell.InternalPaint(g, clipBounds, cellBounds, rowIndex, cellState,
                    value, formattedValue, errorText,
                    cellStyle, advancedBorderStyle,
                    paintParts & ~DataGridViewPaintParts.Border);
                //Paint the borders.
                if ((paintParts & DataGridViewPaintParts.Border) != DataGridViewPaintParts.None)
                {
                    var leftTopCell = ownerCell;
                    DataGridViewAdvancedBorderStyle advancedBorderStyle2 = new DataGridViewAdvancedBorderStyle()
                    {
                        Left = advancedBorderStyle.Left,
                        Top = advancedBorderStyle.Top,
                        Right = DataGridViewAdvancedCellBorderStyle.None,
                        Bottom = DataGridViewAdvancedCellBorderStyle.None
                    };
                    leftTopCell.PaintBorder(g, clipBounds, cellBounds, cellStyle, advancedBorderStyle2);

                    var rightBottomCell = DataGridView[columnIndex + columnSpan - 1, rowIndex + rowSpan - 1] as DataGridViewTextBoxSpanCell;
                    if (rightBottomCell == null)
                        rightBottomCell = this;
                    DataGridViewAdvancedBorderStyle advancedBorderStyle3 = new DataGridViewAdvancedBorderStyle()
                    {
                        Left = DataGridViewAdvancedCellBorderStyle.None,
                        Top = DataGridViewAdvancedCellBorderStyle.None,
                        Right = advancedBorderStyle.Right,
                        Bottom = advancedBorderStyle.Bottom
                    };
                    rightBottomCell.PaintBorder(g, clipBounds, cellBounds, cellStyle, advancedBorderStyle3);
                }
            }
        }
        #endregion

        #region Spanning.
        private void SetSpan(int columnSpan, int rowSpan)
        {
            int prevColumnSpan = m_ColumnSpan;
            int prevRowSpan = m_RowSpan;
            m_ColumnSpan = columnSpan;
            m_RowSpan = rowSpan;

            if (DataGridView != null)
            {
                #region clearing.
                foreach (int rowIndex in Enumerable.Range(RowIndex, prevRowSpan))
                    foreach (int columnIndex in Enumerable.Range(ColumnIndex, prevColumnSpan))
                    {
                        var cell = DataGridView[columnIndex, rowIndex] as DataGridViewTextBoxSpanCell;
                        if (cell != null)
                            cell.OwnerCell = null;
                    }
                #endregion

                #region setting.
                foreach (int rowIndex in Enumerable.Range(RowIndex, m_RowSpan))
                    foreach (int columnIndex in Enumerable.Range(ColumnIndex, m_ColumnSpan))
                    {
                        var cell = DataGridView[columnIndex, rowIndex] as DataGridViewTextBoxSpanCell;
                        if (cell != null)
                            cell.OwnerCell = this;
                    }
                OwnerCell = null;
                #endregion

                DataGridView.Invalidate();
            }
        }
        #endregion

        #region Editing.
        protected override object GetValue(int rowIndex)
        {
            if (m_OwnerCell != null)
                return m_OwnerCell.GetValue(m_OwnerCell.RowIndex);
            return base.GetValue(rowIndex);
        }
        protected override bool SetValue(int rowIndex, object value)
        {
            if (m_OwnerCell != null)
                return m_OwnerCell.SetValue(m_OwnerCell.RowIndex, value);
            return base.SetValue(rowIndex, value);
        }

        public override Rectangle PositionEditingPanel(Rectangle cellBounds, Rectangle cellClip, DataGridViewCellStyle cellStyle, bool singleVerticalBorderAdded, bool singleHorizontalBorderAdded, bool isFirstDisplayedColumn, bool isFirstDisplayedRow)
        {
            if (m_OwnerCell == null
                && m_ColumnSpan == 1 && m_RowSpan == 1)
            {
                return base.PositionEditingPanel(cellBounds, cellClip, cellStyle, singleVerticalBorderAdded, singleHorizontalBorderAdded, isFirstDisplayedColumn, isFirstDisplayedRow);
            }

            int rowIndex = RowIndex;
            var ownerCell = this;
            if (m_OwnerCell != null)
            {
                rowIndex = m_OwnerCell.RowIndex;
                cellStyle = m_OwnerCell.GetInheritedStyle(null, rowIndex, true);
                m_OwnerCell.GetFormattedValue(m_OwnerCell.Value, rowIndex, ref cellStyle, null, null, DataGridViewDataErrorContexts.Formatting);
                var editingControl = DataGridView.EditingControl as IDataGridViewEditingControl;
                if (editingControl != null)
                {
                    editingControl.ApplyCellStyleToEditingControl(cellStyle);
                    var editingPanel = DataGridView.EditingControl.Parent;
                    if (editingPanel != null)
                        editingPanel.BackColor = cellStyle.BackColor;
                }
                ownerCell = m_OwnerCell;
            }
            cellBounds = GetSpannedCellBoundsFromChildCellBounds(this, cellBounds, singleVerticalBorderAdded, singleHorizontalBorderAdded);
            cellClip = GetSpannedCellClipBounds(ownerCell, cellBounds, singleVerticalBorderAdded, singleHorizontalBorderAdded);
            return base.PositionEditingPanel(cellBounds, cellClip, cellStyle, singleVerticalBorderAdded, singleHorizontalBorderAdded, isFirstDisplayedColumn, isFirstDisplayedRow);
        }
        #endregion

        #region Other overridden

        protected override void OnDataGridViewChanged()
        {
            base.OnDataGridViewChanged();

            if (DataGridView == null)
            {
                m_ColumnSpan = 1;
                m_RowSpan = 1;
            }
        }

        protected override Rectangle BorderWidths(DataGridViewAdvancedBorderStyle advancedBorderStyle)
        {
            if (m_OwnerCell == null
                && m_ColumnSpan == 1 && m_RowSpan == 1)
            {
                return base.BorderWidths(advancedBorderStyle);
            }

            if (m_OwnerCell != null)
                return m_OwnerCell.BorderWidths(advancedBorderStyle);

            var leftTop = base.BorderWidths(advancedBorderStyle);
            var rightBottomCell = DataGridView[
                ColumnIndex + ColumnSpan - 1,
                RowIndex + RowSpan - 1] as DataGridViewTextBoxSpanCell;
            var rightBottom = rightBottomCell != null
                ? rightBottomCell.InternalBorderWidths(advancedBorderStyle)
                : leftTop;
            return new Rectangle(leftTop.X, leftTop.Y, rightBottom.Width, rightBottom.Height);
        }

        #endregion

        #region Private Methods

        private bool CellsRegionContainsSelectedCell(int columnIndex, int rowIndex, int columnSpan, int rowSpan)
        {
            if (DataGridView == null)
                return false;

            foreach (int col in Enumerable.Range(columnIndex, columnSpan))
                foreach (int row in Enumerable.Range(rowIndex, rowSpan))
                    if (DataGridView[col, row].Selected)
                    {
                        return true;
                    }

            return false;
        }
        private Rectangle InternalBorderWidths(DataGridViewAdvancedBorderStyle advancedBorderStyle)
        {
            return base.BorderWidths(advancedBorderStyle);
        }
        private static Rectangle GetSpannedCellClipBounds(
            DataGridViewTextBoxSpanCell ownerCell,
            Rectangle cellBounds,
            bool singleVerticalBorderAdded,
            bool singleHorizontalBorderAdded)
        {
            DataGridView dataGridView = ownerCell.DataGridView;
            var clipBounds = cellBounds;
            //Setting X.
            foreach (int i in Enumerable.Range(ownerCell.ColumnIndex, ownerCell.m_ColumnSpan))
            {
                var columnItem = dataGridView.Columns[i];
                if (!columnItem.Visible)
                    continue;
                if (columnItem.Frozen
                    || i > dataGridView.FirstDisplayedScrollingColumnIndex)
                {
                    break;
                }
                if (i == dataGridView.FirstDisplayedScrollingColumnIndex)
                {
                    clipBounds.X += dataGridView.FirstDisplayedScrollingColumnHiddenWidth;
                    break;
                }
                clipBounds.X += columnItem.Width;
            }
            //Setting Width.
            clipBounds.Width = Enumerable.Range(ownerCell.ColumnIndex, ownerCell.m_ColumnSpan)
                .Sum(i =>
                {
                    var columnItem = dataGridView.Columns[i];
                    if (!columnItem.Visible)
                        return 0;
                    if (columnItem.Frozen
                        || i > dataGridView.FirstDisplayedScrollingColumnIndex)
                    {
                        return columnItem.Width;
                    }
                    if (i == dataGridView.FirstDisplayedScrollingColumnIndex)
                    {
                        return columnItem.Width
                            - dataGridView.FirstDisplayedScrollingColumnHiddenWidth;
                    }
                    return 0;
                });
            //Setting Y.
            foreach (var i in Enumerable.Range(ownerCell.RowIndex, ownerCell.m_RowSpan))
            {
                var rowItem = dataGridView.Rows[i];
                if (!rowItem.Visible)
                    continue;
                if (rowItem.Frozen
                    || i >= dataGridView.FirstDisplayedScrollingRowIndex)
                {
                    break;
                }
                clipBounds.Y += rowItem.Height;
            }
            //Setting Height.
            clipBounds.Height = Enumerable.Range(ownerCell.RowIndex, ownerCell.m_RowSpan)
                .Sum(i =>
                {
                    var rowItem = dataGridView.Rows[i];
                    if (!rowItem.Visible)
                        return 0;
                    if (rowItem.Frozen
                        || i >= dataGridView.FirstDisplayedScrollingRowIndex)
                    {
                        return dataGridView.Rows[i].Height;
                    }
                    return 0;
                });

            if (singleVerticalBorderAdded)
                clipBounds.Width++;
            if (singleHorizontalBorderAdded)
                clipBounds.Height++;

            //exclude borders.
            if (dataGridView.BorderStyle != BorderStyle.None)
            {
                var clientRectangle = dataGridView.ClientRectangle;
                clientRectangle.Width--;
                clientRectangle.Height--;
                clipBounds.Intersect(clientRectangle);
            }
            return clipBounds;
        }
        private static Rectangle GetSpannedCellBoundsFromChildCellBounds(
            DataGridViewTextBoxSpanCell childCell,
            Rectangle childCellBounds,
            bool singleVerticalBorderAdded,
            bool singleHorizontalBorderAdded)
        {
            var dataGridView = childCell.DataGridView;
            var ownerCell = childCell.OwnerCell == null
                ? childCell
                : childCell.OwnerCell;
            var spannedCellBounds = childCellBounds;
            //
            var firstVisibleColumnIndex = Enumerable.Range(ownerCell.ColumnIndex, ownerCell.ColumnSpan)
                .First(i => dataGridView.Columns[i].Visible);
            if (dataGridView.Columns[firstVisibleColumnIndex].Frozen)
            {
                spannedCellBounds.X = dataGridView.GetColumnDisplayRectangle(firstVisibleColumnIndex, false).X;
            }
            else
            {
                spannedCellBounds.X -= Enumerable.Range(firstVisibleColumnIndex, childCell.ColumnIndex - firstVisibleColumnIndex)
                    .Select(i => dataGridView.Columns[i])
                    .Where(columnItem => columnItem.Visible)
                    .Sum(columnItem => columnItem.Width);
            }
            //
            var firstVisibleRowIndex = Enumerable.Range(ownerCell.RowIndex, ownerCell.RowSpan)
                .First(i => dataGridView.Rows[i].Visible);
            if (dataGridView.Rows[firstVisibleRowIndex].Frozen)
            {
                spannedCellBounds.Y = dataGridView.GetRowDisplayRectangle(firstVisibleRowIndex, false).Y;
            }
            else
            {
                spannedCellBounds.Y -= Enumerable.Range(firstVisibleRowIndex, childCell.RowIndex - firstVisibleRowIndex)
                    .Select(i => dataGridView.Rows[i])
                    .Where(rowItem => rowItem.Visible)
                    .Sum(rowItem => rowItem.Height);
            }
            //
            spannedCellBounds.Width = Enumerable.Range(ownerCell.ColumnIndex, ownerCell.ColumnSpan)
                    .Select(i => dataGridView.Columns[i])
                    .Where(columnItem => columnItem.Visible)
                    .Sum(columnItem => columnItem.Width);
            //
            spannedCellBounds.Height = Enumerable.Range(ownerCell.RowIndex, ownerCell.RowSpan)
                    .Select(i => dataGridView.Rows[i])
                    .Where(rowItem => rowItem.Visible)
                    .Sum(rowItem => rowItem.Height);

            if (singleVerticalBorderAdded)
                spannedCellBounds.Width++;
            if (singleHorizontalBorderAdded)
                spannedCellBounds.Height++;

            return spannedCellBounds;
        }

        #endregion
    }

    public class DataGridViewTextBoxSpanColumn : DataGridViewColumn
    {
        #region ctor
        public DataGridViewTextBoxSpanColumn()
            : base(new DataGridViewTextBoxSpanCell())
        {
        }
        #endregion
    }
}
