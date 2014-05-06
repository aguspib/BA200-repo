﻿Imports System
Imports System.Text
Imports System.Diagnostics
Imports System.Globalization
Imports System.Windows.Forms
Imports System.ComponentModel

Namespace Biosystems.Ax00.Controls.UserControls
    ''' <summary>
    ''' Custom column type dedicated to the DataGridViewNumericUpDownCell cell type.
    ''' </summary>
    Public Class DataGridViewNumericUpDownColumn
        Inherits DataGridViewColumn
        ''' <summary>
        ''' Constructor for the DataGridViewNumericUpDownColumn class.
        ''' </summary>
        Public Sub New()
            MyBase.New(New DataGridViewNumericUpDownCell())
        End Sub

        ''' <summary>
        ''' Represents the implicit cell that gets cloned when adding rows to the grid.
        ''' </summary>
        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public Overloads Overrides Property CellTemplate() As DataGridViewCell
            Get
                Return MyBase.CellTemplate
            End Get
            Set(ByVal value As DataGridViewCell)
                Dim dataGridViewNumericUpDownCell As DataGridViewNumericUpDownCell = TryCast(value, DataGridViewNumericUpDownCell)
                If value IsNot Nothing AndAlso dataGridViewNumericUpDownCell Is Nothing Then
                    Throw New InvalidCastException("Value provided for CellTemplate must be of type DataGridViewNumericUpDownElements.DataGridViewNumericUpDownCell or derive from it.")
                End If
                MyBase.CellTemplate = value
            End Set
        End Property

        ''' <summary>
        ''' Replicates the DecimalPlaces property of the DataGridViewNumericUpDownCell cell type.
        ''' </summary>
        <Category("Appearance"), DefaultValue(DataGridViewNumericUpDownCell.DATAGRIDVIEWNUMERICUPDOWNCELL_defaultDecimalPlaces), Description("Indicates the number of decimal places to display.")> _
        Public Property DecimalPlaces() As Integer
            Get
                If Me.NumericUpDownCellTemplate Is Nothing Then
                    Throw New InvalidOperationException("Operation cannot be completed because this DataGridViewColumn does not have a CellTemplate.")
                End If
                Return Me.NumericUpDownCellTemplate.DecimalPlaces
            End Get
            Set(ByVal value As Integer)
                If Me.NumericUpDownCellTemplate Is Nothing Then
                    Throw New InvalidOperationException("Operation cannot be completed because this DataGridViewColumn does not have a CellTemplate.")
                End If
                ' Update the template cell so that subsequent cloned cells use the new value.
                Me.NumericUpDownCellTemplate.DecimalPlaces = value
                If Me.DataGridView IsNot Nothing Then
                    ' Update all the existing DataGridViewNumericUpDownCell cells in the column accordingly.
                    Dim dataGridViewRows As DataGridViewRowCollection = Me.DataGridView.Rows
                    Dim rowCount As Integer = dataGridViewRows.Count
                    For rowIndex As Integer = 0 To rowCount - 1
                        ' Be careful not to unshare rows unnecessarily.
                        ' This could have severe performance repercussions.
                        Dim dataGridViewRow As DataGridViewRow = dataGridViewRows.SharedRow(rowIndex)
                        Dim dataGridViewCell As DataGridViewNumericUpDownCell = TryCast(dataGridViewRow.Cells(Me.Index), DataGridViewNumericUpDownCell)
                        If dataGridViewCell IsNot Nothing Then
                            ' Call the internal SetDecimalPlaces method instead of the property to avoid invalidation
                            ' of each cell. The whole column is invalidated later in a single operation for better performance.
                            dataGridViewCell.SetDecimalPlaces(rowIndex, value)
                        End If
                    Next
                    ' TODO: Call the grid's autosizing methods to autosize the column, rows, column headers / row headers as needed.
                    Me.DataGridView.InvalidateColumn(Me.Index)
                End If
            End Set
        End Property

        ''' <summary>
        ''' Replicates the Increment property of the DataGridViewNumericUpDownCell cell type.
        ''' </summary>
        <Category("Data"), Description("Indicates the amount to increment or decrement on each button click.")> _
        Public Property Increment() As [Decimal]
            Get
                If Me.NumericUpDownCellTemplate Is Nothing Then
                    Throw New InvalidOperationException("Operation cannot be completed because this DataGridViewColumn does not have a CellTemplate.")
                End If
                Return Me.NumericUpDownCellTemplate.Increment
            End Get
            Set(ByVal value As [Decimal])
                If Me.NumericUpDownCellTemplate Is Nothing Then
                    Throw New InvalidOperationException("Operation cannot be completed because this DataGridViewColumn does not have a CellTemplate.")
                End If
                Me.NumericUpDownCellTemplate.Increment = value
                If Me.DataGridView IsNot Nothing Then
                    Dim dataGridViewRows As DataGridViewRowCollection = Me.DataGridView.Rows
                    Dim rowCount As Integer = dataGridViewRows.Count
                    For rowIndex As Integer = 0 To rowCount - 1
                        Dim dataGridViewRow As DataGridViewRow = dataGridViewRows.SharedRow(rowIndex)
                        Dim dataGridViewCell As DataGridViewNumericUpDownCell = TryCast(dataGridViewRow.Cells(Me.Index), DataGridViewNumericUpDownCell)
                        If dataGridViewCell IsNot Nothing Then
                            dataGridViewCell.SetIncrement(rowIndex, value)
                        End If
                    Next
                End If
            End Set
        End Property

        ''' Indicates whether the Increment property should be persisted.
        Private Function ShouldSerializeIncrement() As Boolean
            Return Not Me.Increment.Equals(DataGridViewNumericUpDownCell.DATAGRIDVIEWNUMERICUPDOWNCELL_defaultIncrement)
        End Function

        ''' <summary>
        ''' Replicates the Maximum property of the DataGridViewNumericUpDownCell cell type.
        ''' </summary>
        <Category("Data"), Description("Indicates the maximum value for the numeric up-down cells."), RefreshProperties(RefreshProperties.All)> _
        Public Property Maximum() As [Decimal]
            Get
                If Me.NumericUpDownCellTemplate Is Nothing Then
                    Throw New InvalidOperationException("Operation cannot be completed because this DataGridViewColumn does not have a CellTemplate.")
                End If
                Return Me.NumericUpDownCellTemplate.Maximum
            End Get
            Set(ByVal value As [Decimal])
                If Me.NumericUpDownCellTemplate Is Nothing Then
                    Throw New InvalidOperationException("Operation cannot be completed because this DataGridViewColumn does not have a CellTemplate.")
                End If
                Me.NumericUpDownCellTemplate.Maximum = value
                If Me.DataGridView IsNot Nothing Then
                    Dim dataGridViewRows As DataGridViewRowCollection = Me.DataGridView.Rows
                    Dim rowCount As Integer = dataGridViewRows.Count
                    For rowIndex As Integer = 0 To rowCount - 1
                        Dim dataGridViewRow As DataGridViewRow = dataGridViewRows.SharedRow(rowIndex)
                        Dim dataGridViewCell As DataGridViewNumericUpDownCell = TryCast(dataGridViewRow.Cells(Me.Index), DataGridViewNumericUpDownCell)
                        If dataGridViewCell IsNot Nothing Then
                            dataGridViewCell.SetMaximum(rowIndex, value)
                        End If
                    Next
                    ' TODO: This column and/or grid rows may need to be autosized depending on their
                    ' autosize settings. Call the autosizing methods to autosize the column, rows,
                    ' column headers / row headers as needed.
                    Me.DataGridView.InvalidateColumn(Me.Index)
                End If
            End Set
        End Property

        ''' Indicates whether the Maximum property should be persisted.
        Private Function ShouldSerializeMaximum() As Boolean
            Return Not Me.Maximum.Equals(DataGridViewNumericUpDownCell.DATAGRIDVIEWNUMERICUPDOWNCELL_defaultMaximum)
        End Function

        ''' <summary>
        ''' Replicates the Minimum property of the DataGridViewNumericUpDownCell cell type.
        ''' </summary>
        <Category("Data"), Description("Indicates the minimum value for the numeric up-down cells."), RefreshProperties(RefreshProperties.All)> _
        Public Property Minimum() As [Decimal]
            Get
                If Me.NumericUpDownCellTemplate Is Nothing Then
                    Throw New InvalidOperationException("Operation cannot be completed because this DataGridViewColumn does not have a CellTemplate.")
                End If
                Return Me.NumericUpDownCellTemplate.Minimum
            End Get
            Set(ByVal value As [Decimal])
                If Me.NumericUpDownCellTemplate Is Nothing Then
                    Throw New InvalidOperationException("Operation cannot be completed because this DataGridViewColumn does not have a CellTemplate.")
                End If
                Me.NumericUpDownCellTemplate.Minimum = value
                If Me.DataGridView IsNot Nothing Then
                    Dim dataGridViewRows As DataGridViewRowCollection = Me.DataGridView.Rows
                    Dim rowCount As Integer = dataGridViewRows.Count
                    For rowIndex As Integer = 0 To rowCount - 1
                        Dim dataGridViewRow As DataGridViewRow = dataGridViewRows.SharedRow(rowIndex)
                        Dim dataGridViewCell As DataGridViewNumericUpDownCell = TryCast(dataGridViewRow.Cells(Me.Index), DataGridViewNumericUpDownCell)
                        If dataGridViewCell IsNot Nothing Then
                            dataGridViewCell.SetMinimum(rowIndex, value)
                        End If
                    Next
                    ' TODO: This column and/or grid rows may need to be autosized depending on their
                    ' autosize settings. Call the autosizing methods to autosize the column, rows,
                    ' column headers / row headers as needed.
                    Me.DataGridView.InvalidateColumn(Me.Index)
                End If
            End Set
        End Property

        ''' Indicates whether the Maximum property should be persisted.
        Private Function ShouldSerializeMinimum() As Boolean
            Return Not Me.Minimum.Equals(DataGridViewNumericUpDownCell.DATAGRIDVIEWNUMERICUPDOWNCELL_defaultMinimum)
        End Function

        ''' <summary>
        ''' Replicates the ThousandsSeparator property of the DataGridViewNumericUpDownCell cell type.
        ''' </summary>
        <Category("Data"), DefaultValue(DataGridViewNumericUpDownCell.DATAGRIDVIEWNUMERICUPDOWNCELL_defaultThousandsSeparator), Description("Indicates whether the thousands separator will be inserted between every three decimal digits.")> _
        Public Property ThousandsSeparator() As Boolean
            Get
                If Me.NumericUpDownCellTemplate Is Nothing Then
                    Throw New InvalidOperationException("Operation cannot be completed because this DataGridViewColumn does not have a CellTemplate.")
                End If
                Return Me.NumericUpDownCellTemplate.ThousandsSeparator
            End Get
            Set(ByVal value As Boolean)
                If Me.NumericUpDownCellTemplate Is Nothing Then
                    Throw New InvalidOperationException("Operation cannot be completed because this DataGridViewColumn does not have a CellTemplate.")
                End If
                Me.NumericUpDownCellTemplate.ThousandsSeparator = value
                If Me.DataGridView IsNot Nothing Then
                    Dim dataGridViewRows As DataGridViewRowCollection = Me.DataGridView.Rows
                    Dim rowCount As Integer = dataGridViewRows.Count
                    For rowIndex As Integer = 0 To rowCount - 1
                        Dim dataGridViewRow As DataGridViewRow = dataGridViewRows.SharedRow(rowIndex)
                        Dim dataGridViewCell As DataGridViewNumericUpDownCell = TryCast(dataGridViewRow.Cells(Me.Index), DataGridViewNumericUpDownCell)
                        If dataGridViewCell IsNot Nothing Then
                            dataGridViewCell.SetThousandsSeparator(rowIndex, value)
                        End If
                    Next
                    ' TODO: This column and/or grid rows may need to be autosized depending on their
                    ' autosize settings. Call the autosizing methods to autosize the column, rows,
                    ' column headers / row headers as needed.
                    Me.DataGridView.InvalidateColumn(Me.Index)
                End If
            End Set
        End Property

        ''' <summary>
        ''' Small utility function that returns the template cell as a DataGridViewNumericUpDownCell
        ''' </summary>
        Private ReadOnly Property NumericUpDownCellTemplate() As DataGridViewNumericUpDownCell
            Get
                Return DirectCast(Me.CellTemplate, DataGridViewNumericUpDownCell)
            End Get
        End Property

        ''' <summary>
        ''' Returns a standard compact string representation of the column.
        ''' </summary>
        Public Overloads Overrides Function ToString() As String
            Dim sb As New StringBuilder(100)
            sb.Append("DataGridViewNumericUpDownColumn { Name=")
            sb.Append(Me.Name)
            sb.Append(", Index=")
            sb.Append(Me.Index.ToString(CultureInfo.CurrentCulture))
            sb.Append(" }")
            Return sb.ToString()
        End Function
    End Class
End Namespace