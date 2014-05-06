Option Explicit On
Option Strict On

Imports System.Windows.Forms
Imports System.Drawing
Imports System.Windows.Forms.VisualStyles
Imports Biosystems.Ax00.Controls.UserControls


''' <summary>
''' User Control that allows the creation and design of an editable grid  
''' </summary>
''' <remarks></remarks>
Public Class BSGridControl

#Region "Constructor"
    'This call is required by the Windows Form Designer
    Public Sub New()
        InitializeComponent()
    End Sub
#End Region

#Region "Declarations"
    Public NAMECOLUMNBUTTON1 As String
    Public NAMECOLUMNIMAGE As String
    Public NAMECOLUMNSPARAMS As List(Of String)   ' n columns
    Public NAMECOLUMNBUTTON2 As String
    Public IDENTIFICATOR As String

    Public Const EDITABLECOLUMN As String = "EDIT"  ' this value is save into TAG field of every column

    'Events
    Public Event Button1Click(ByVal adjButton As DataGridViewDisableButtonCell, ByVal rowIndex As Integer)
    Public Event SelectedValueEvent(ByVal rowIndex As Integer, ByVal colIndex As Integer)
    Public Event Button2Click(ByVal adjButton As DataGridViewDisableButtonCell, ByVal rowIndex As Integer)
#End Region

#Region "Attibutes"
    'Attribute variables needed for the path of the different Icons used in the control
    Private OKIconNameAttribute As String = ""
    Private NOIconNameAttribute As String = ""
    Private numParamsAttribute As Integer
    Private selectedValueAttribute As String
    Private selectedRowAttribute As Integer

    'SGM 28/02/2012
    Private AdjustButtonImageAttribute As Image = Nothing
    Private TestButtonImageAttribute As Image = Nothing
    Private OkImageAttribute As Image = Nothing

#End Region

#Region "Properties"
    Public Property ValidationImage() As String
        Get
            Return OKIconNameAttribute
        End Get
        Set(ByVal value As String)
            OKIconNameAttribute = value
        End Set
    End Property

    Public Property NoValidationImage() As String
        Get
            Return NOIconNameAttribute
        End Get
        Set(ByVal value As String)
            NOIconNameAttribute = value
        End Set
    End Property

    Public Property OkImage() As Image
        Get
            Return OkImageAttribute
        End Get
        Set(ByVal value As Image)
            OkImageAttribute = value
        End Set
    End Property


    Public Property numParams() As Integer
        Get
            Return Me.numParamsAttribute
        End Get
        Set(ByVal value As Integer)
            Me.numParamsAttribute = value

            For i As Integer = 0 To value - 1
                If NAMECOLUMNSPARAMS Is Nothing Then
                    NAMECOLUMNSPARAMS = New List(Of String)
                End If
                NAMECOLUMNSPARAMS.Add("")
            Next
        End Set
    End Property

    Public Property nameColButton1() As String
        Get
            Return NAMECOLUMNBUTTON1
        End Get
        Set(ByVal value As String)
            NAMECOLUMNBUTTON1 = value
        End Set
    End Property

    Public Property nameColImage() As String
        Get
            Return NAMECOLUMNIMAGE
        End Get
        Set(ByVal value As String)
            NAMECOLUMNIMAGE = value
        End Set
    End Property

    Public Property nameColsParams(ByVal index As Integer) As String
        Get
            Return NAMECOLUMNSPARAMS(index)
        End Get
        Set(ByVal value As String)
            NAMECOLUMNSPARAMS(index) = value
        End Set
    End Property

    Public Property nameColButton2() As String
        Get
            Return NAMECOLUMNBUTTON2
        End Get
        Set(ByVal value As String)
            NAMECOLUMNBUTTON2 = value
        End Set
    End Property

    Public Property nameIdentificator() As String
        Get
            Return IDENTIFICATOR
        End Get
        Set(ByVal value As String)
            IDENTIFICATOR = value
        End Set
    End Property

    Public Property EnableButton1(ByVal rowIndex As Integer) As Boolean
        Get
            Dim returnValue As Boolean
            If Not Me.bsAdjustsDataGridView Is Nothing Then
                If Me.bsAdjustsDataGridView.Rows.Count > rowIndex Then
                    Dim buttonCell As DataGridViewDisableButtonCell = _
                        CType(Me.bsAdjustsDataGridView.Rows(rowIndex).Cells(NAMECOLUMNBUTTON1),  _
                        DataGridViewDisableButtonCell)
                    returnValue = buttonCell.Enabled
                End If
            End If
            Return returnValue
        End Get
        Set(ByVal value As Boolean)
            If Not Me.bsAdjustsDataGridView Is Nothing Then
                If Me.bsAdjustsDataGridView.Rows.Count > rowIndex Then
                    Dim buttonCell As DataGridViewDisableButtonCell = _
                        CType(Me.bsAdjustsDataGridView.Rows(rowIndex).Cells(NAMECOLUMNBUTTON1),  _
                        DataGridViewDisableButtonCell)
                    buttonCell.Enabled = value
                End If
            End If
        End Set
    End Property

    Public Property EnableButton2(ByVal rowIndex As Integer) As Boolean
        Get
            Dim returnValue As Boolean
            If Not Me.bsAdjustsDataGridView Is Nothing Then
                If Me.bsAdjustsDataGridView.Rows.Count > rowIndex Then
                    Dim buttonCell As DataGridViewDisableButtonCell = _
                        CType(Me.bsAdjustsDataGridView.Rows(rowIndex).Cells(NAMECOLUMNBUTTON2),  _
                        DataGridViewDisableButtonCell)
                    returnValue = buttonCell.Enabled
                End If
            End If
            Return returnValue
        End Get
        Set(ByVal value As Boolean)
            If Not Me.bsAdjustsDataGridView Is Nothing Then
                If Me.bsAdjustsDataGridView.Rows.Count > rowIndex Then
                    Dim buttonCell As DataGridViewDisableButtonCell = _
                        CType(Me.bsAdjustsDataGridView.Rows(rowIndex).Cells(NAMECOLUMNBUTTON2),  _
                        DataGridViewDisableButtonCell)
                    buttonCell.Enabled = value
                    bsAdjustsDataGridView.Refresh()
                End If
            End If
        End Set
    End Property

    Public Property EnableCell(ByVal rowIndex As Integer, ByVal colName As String) As Boolean
        Get
            Dim returnValue As Boolean
            If Not Me.bsAdjustsDataGridView Is Nothing Then
                If Me.bsAdjustsDataGridView.Rows.Count > rowIndex Then
                    Dim TextBoxCell As DataGridViewDisableTextBoxCell = _
                        CType(Me.bsAdjustsDataGridView.Rows(rowIndex).Cells(colName),  _
                        DataGridViewDisableTextBoxCell)
                    returnValue = TextBoxCell.Enabled
                End If
            End If
            Return returnValue
        End Get
        Set(ByVal value As Boolean)
            If Not Me.bsAdjustsDataGridView Is Nothing Then
                If Me.bsAdjustsDataGridView.Rows.Count > rowIndex Then
                    Dim TextBoxCell As DataGridViewDisableTextBoxCell = _
                        CType(Me.bsAdjustsDataGridView.Rows(rowIndex).Cells(colName),  _
                        DataGridViewDisableTextBoxCell)
                    TextBoxCell.Enabled = value
                End If
            End If
        End Set
    End Property


    Public ReadOnly Property VisibleIcon(ByVal visible As Boolean) As Bitmap
        Get
            Dim path As String
            If visible Then
                path = Me.ValidationImage
            Else
                path = Me.NoValidationImage
            End If

            If System.IO.File.Exists(path) Then
                Dim bm_source As New Bitmap(Image.FromFile(path))
                Dim scale_factor As Integer = 20
                ' Make a bitmap for the result.
                Dim bm_dest As New Bitmap(CInt(scale_factor), CInt(scale_factor))
                ' Make a Graphics object for the result Bitmap.
                Dim gr_dest As Graphics = Graphics.FromImage(bm_dest)
                ' Copy the source image into the destination bitmap.
                gr_dest.DrawImage(bm_source, 0, 0, bm_dest.Width, bm_dest.Height)

                Return bm_dest
            Else

                Return New Bitmap(0, 0)
            End If

        End Get
    End Property

    Public Property SelectedValue() As String
        Get
            Return Me.selectedValueAttribute
        End Get
        Set(ByVal value As String)
            Me.selectedValueAttribute = value

            If Not Me.bsAdjustsDataGridView Is Nothing Then
                For Each cell As DataGridViewCell In Me.bsAdjustsDataGridView.SelectedCells
                    If cell.Selected AndAlso Not cell.Tag Is Nothing AndAlso cell.Tag.ToString = EDITABLECOLUMN Then
                        cell.Value = value
                        Me.selectedRowAttribute = cell.RowIndex
                        bsAdjustsDataGridView.Refresh()
                    End If
                Next
            End If
        End Set
    End Property

    Public Property SelectedRow() As Integer
        Get
            Return Me.selectedRowAttribute
        End Get
        Set(ByVal value As Integer)
            Me.selectedRowAttribute = value
        End Set
    End Property

    Public Property ColumnWidth(ByVal colName As String) As Integer
        Get
            Dim returnValue As Integer
            If Not Me.bsAdjustsDataGridView Is Nothing Then
                For Each col As DataGridViewColumn In Me.bsAdjustsDataGridView.Columns
                    If col.Name = colName Then
                        returnValue = col.Width
                        Exit For
                    End If
                Next
            End If
            Return returnValue
        End Get
        Set(ByVal value As Integer)
            If Not Me.bsAdjustsDataGridView Is Nothing Then
                For Each col As DataGridViewColumn In Me.bsAdjustsDataGridView.Columns
                    If col.Name = colName Then
                        col.Width = value
                        Exit For
                    End If
                Next
            End If
        End Set
    End Property

    Public Property RowHeight(ByVal rowIndex As Integer) As Integer
        Get
            Dim returnValue As Integer
            If Not Me.bsAdjustsDataGridView Is Nothing Then
                For Each row As DataGridViewRow In Me.bsAdjustsDataGridView.Rows
                    If row.Index = rowIndex Then
                        returnValue = row.Height
                        Exit For
                    End If
                Next
            End If
            Return returnValue
        End Get
        Set(ByVal value As Integer)
            If Not Me.bsAdjustsDataGridView Is Nothing Then
                For Each row As DataGridViewRow In Me.bsAdjustsDataGridView.Rows
                    If row.Index = rowIndex Then
                        row.Height = value
                        Exit For
                    End If
                Next
            End If
        End Set
    End Property

    Public ReadOnly Property ColumnsCount() As Integer
        Get
            Dim returnValue As Integer
            If Not Me.bsAdjustsDataGridView Is Nothing Then
                returnValue = Me.bsAdjustsDataGridView.Columns.Count
            End If
            Return returnValue
        End Get
    End Property

    Public ReadOnly Property RowsCount() As Integer
        Get
            Dim returnValue As Integer
            If Not Me.bsAdjustsDataGridView Is Nothing Then
                returnValue = Me.bsAdjustsDataGridView.Rows.Count
            End If
            Return returnValue
        End Get
    End Property

    Public Property ParameterCellValue(ByVal rowIndex As Integer, ByVal colIndex As Integer) As String
        Get
            Dim returnValue As String = ""
            If Not Me.bsAdjustsDataGridView Is Nothing Then
                If Me.bsAdjustsDataGridView.Rows.Count > rowIndex Then
                    returnValue = Me.bsAdjustsDataGridView.Rows(rowIndex).Cells(NAMECOLUMNSPARAMS(colIndex)).Value.ToString
                End If
            End If
            Return returnValue
        End Get
        Set(ByVal value As String)
            If Not Me.bsAdjustsDataGridView Is Nothing Then
                If Me.bsAdjustsDataGridView.Rows.Count > rowIndex Then
                    Me.bsAdjustsDataGridView.Rows(rowIndex).Cells(NAMECOLUMNSPARAMS(colIndex)).Value = value
                End If
            End If
        End Set
    End Property

    Public ReadOnly Property IdentValue(ByVal rowIndex As Integer) As String
        Get
            Dim returnValue As String = ""
            If Not Me.bsAdjustsDataGridView Is Nothing Then
                If Me.bsAdjustsDataGridView.Rows.Count > rowIndex Then
                    returnValue = Me.bsAdjustsDataGridView.Rows(rowIndex).Cells(IDENTIFICATOR).Value.ToString
                End If
            End If
            Return returnValue
        End Get
    End Property

    Public Property NameColumnButton2ByRow(ByVal rowIndex As Integer) As String
        Get
            Dim returnValue As String = ""
            If Not Me.bsAdjustsDataGridView Is Nothing Then
                If Me.bsAdjustsDataGridView.Rows.Count > rowIndex Then
                    returnValue = Me.bsAdjustsDataGridView.Rows(rowIndex).Cells(NAMECOLUMNBUTTON2).Value.ToString
                End If
            End If
            Return returnValue
        End Get
        Set(ByVal value As String)
            If Not Me.bsAdjustsDataGridView Is Nothing Then
                If Me.bsAdjustsDataGridView.Rows.Count > rowIndex Then
                    Me.bsAdjustsDataGridView.Rows(rowIndex).Cells(NAMECOLUMNBUTTON2).Value = value
                End If
            End If
        End Set
    End Property

    Public WriteOnly Property HeaderHighlight(ByVal colName As String) As Boolean
        Set(ByVal value As Boolean)
            If Not Me.bsAdjustsDataGridView Is Nothing Then
                For Each col As DataGridViewColumn In Me.bsAdjustsDataGridView.Columns
                    If col.Name = colName Then
                        If value Then
                            col.HeaderCell.Style.BackColor = Color.LightYellow
                        Else
                            col.HeaderCell.Style.BackColor = Color.Gainsboro
                        End If
                        Exit For
                    End If
                Next
            End If
        End Set
    End Property

    Public Property AdjustButtonImage() As Image
        Get
            Return AdjustButtonImageAttribute
        End Get
        Set(ByVal value As Image)
            AdjustButtonImageAttribute = value
        End Set
    End Property
    Public Property TestButtonImage() As Image
        Get
            Return TestButtonImageAttribute
        End Get
        Set(ByVal value As Image)
            TestButtonImageAttribute = value
        End Set
    End Property

#End Region

#Region "Public Methods"

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="rowIndex"></param>
    ''' <remarks>Created by SGM 28/02/2012</remarks>
    Public Sub ShowIconOk(ByVal rowIndex As Integer)
        Try
            If Not Me.bsAdjustsDataGridView Is Nothing Then
                If Me.bsAdjustsDataGridView.Rows.Count > rowIndex Then
                    Me.bsAdjustsDataGridView.Rows(rowIndex).Cells(NAMECOLUMNIMAGE).Value = MyClass.OkImage
                    Me.bsAdjustsDataGridView.Rows(rowIndex).Cells(NAMECOLUMNIMAGE).Tag = "True"
                End If
            End If
        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="rowIndex"></param>
    ''' <remarks>Created by SGM 28/02/2012</remarks>
    Public Sub HideIconOk(ByVal rowIndex As Integer)
        Try
            If Not Me.bsAdjustsDataGridView Is Nothing Then
                If Me.bsAdjustsDataGridView.Rows.Count > rowIndex Then
                    Me.bsAdjustsDataGridView.Rows(rowIndex).Cells(NAMECOLUMNIMAGE).Value = Nothing
                    Me.bsAdjustsDataGridView.Rows(rowIndex).Cells(NAMECOLUMNIMAGE).Tag = "False"
                End If
            End If
        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    Public Sub HideAllIconsOk()
        Try
            For Each dr As DataGridViewRow In Me.bsAdjustsDataGridView.Rows
                dr.Cells(NAMECOLUMNIMAGE).Value = Nothing
                dr.Cells(NAMECOLUMNIMAGE).Tag = "False"
            Next
        Catch ex As Exception
            Throw ex
        End Try
    End Sub


    ''' <summary>
    ''' Display Image into Icon Cell
    ''' </summary>
    ''' <param name="rowIndex">row of the icon cell</param>
    ''' <remarks>
    ''' Created by XBC 15/12/2010
    ''' </remarks>
    Public Sub ShowIcon(ByVal rowIndex As Integer)
        Try
            If Not Me.bsAdjustsDataGridView Is Nothing Then
                If Me.bsAdjustsDataGridView.Rows.Count > rowIndex Then
                    Me.bsAdjustsDataGridView.Rows(rowIndex).Cells(NAMECOLUMNIMAGE).Value = VisibleIcon(True)
                    Me.bsAdjustsDataGridView.Rows(rowIndex).Cells(NAMECOLUMNIMAGE).Tag = "True"
                End If
            End If
        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    ''' <summary>
    ''' Hides Image into Icon Cell
    ''' </summary>
    ''' <param name="rowIndex">row of the icon cell</param>
    ''' <remarks>
    ''' Created by XBC 15/12/2010
    ''' </remarks>
    Public Sub HideIcon(ByVal rowIndex As Integer)
        Try
            If Not Me.bsAdjustsDataGridView Is Nothing Then
                If Me.bsAdjustsDataGridView.Rows.Count > rowIndex Then
                    Me.bsAdjustsDataGridView.Rows(rowIndex).Cells(NAMECOLUMNIMAGE).Value = VisibleIcon(False)
                    Me.bsAdjustsDataGridView.Rows(rowIndex).Cells(NAMECOLUMNIMAGE).Tag = "False"
                End If
            End If
        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    'SGM 04/02/2011
    Public Function AreAllRowDataValidated() As Boolean

        Dim AllValidated As Boolean = False
        Try
            For Each dr As DataGridViewRow In Me.bsAdjustsDataGridView.Rows
                'If CType(dr.Cells(NAMECOLUMNIMAGE).Value, System.Drawing.Bitmap) Is VisibleIcon(True) Then
                If Not dr.Cells(NAMECOLUMNIMAGE).Tag Is Nothing Then
                    If dr.Cells(NAMECOLUMNIMAGE).Tag.Equals("True") Then
                        AllValidated = True
                    Else
                        Return False
                    End If
                Else
                    Return False
                End If
            Next
        Catch ex As Exception
            Throw ex
        End Try
        Return AllValidated
    End Function

    'SGM 04/02/2011
    Public Function IsAnyRowDataValidated() As Boolean
        Try
            For Each dr As DataGridViewRow In Me.bsAdjustsDataGridView.Rows
                If CType(dr.Cells(NAMECOLUMNIMAGE).Value, System.Drawing.Bitmap) Is VisibleIcon(True) Then
                    Return True
                End If
            Next
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    'SGM 04/02/2011
    Public Sub SetAllRowDataInvalidated()
        Try
            For Each dr As DataGridViewRow In Me.bsAdjustsDataGridView.Rows
                dr.Cells(NAMECOLUMNIMAGE).Value = VisibleIcon(False)
            Next
        Catch ex As Exception
            Throw ex
        End Try
    End Sub
#End Region

#Region "Private Methods"
    ''' <summary>
    ''' Initialization of all controls
    ''' </summary>
    ''' <remarks>
    ''' Created by XBC 14/12/2010
    ''' </remarks>
    Public Function PrepareControls() As DataSet
        Try
            Dim ds As New DataSet
            Dim table As DataTable
            Dim column As DataColumn
            table = Nothing
            column = Nothing
            Me.selectedValueAttribute = Nothing
            '// Create new DataTable.
            table = New DataTable("BSGridControl")

            Me.bsAdjustsDataGridView.Rows.Clear()
            Me.bsAdjustsDataGridView.Columns.Clear()
            Me.bsAdjustsDataGridView.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None
            Me.bsAdjustsDataGridView.CellBorderStyle = DataGridViewCellBorderStyle.None
            Me.bsAdjustsDataGridView.SelectionMode = DataGridViewSelectionMode.CellSelect  ' FullRowSelect
            Me.bsAdjustsDataGridView.MultiSelect = False

            Dim columnName As String

            '
            'Identificator Column (Hidden)
            '
            Dim IdentCol As New DataGridViewTextBoxColumn

            columnName = IDENTIFICATOR
            IdentCol.DataPropertyName = columnName
            IdentCol.Name = columnName
            IdentCol.HeaderText = ""

            Me.bsAdjustsDataGridView.Columns.Add(IdentCol)
            Me.bsAdjustsDataGridView.Columns(columnName).HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter
            Me.bsAdjustsDataGridView.Columns(columnName).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            Me.bsAdjustsDataGridView.Columns(columnName).HeaderCell.Style.BackColor = Me.BackColor
            Me.bsAdjustsDataGridView.Columns(columnName).DefaultCellStyle.BackColor = Me.BackColor
            Me.bsAdjustsDataGridView.Columns(columnName).Width = 0
            Me.bsAdjustsDataGridView.Columns(columnName).SortMode = DataGridViewColumnSortMode.NotSortable
            Me.bsAdjustsDataGridView.Columns(columnName).Tag = ""
            Me.bsAdjustsDataGridView.Columns(columnName).Visible = False

            'creating own dataset to return
            column = New DataColumn()
            column.ColumnName = columnName
            table.Columns.Add(column)

            '
            'Action Column (Button)
            '
            Dim ActionButtonCol As New DataGridViewDisableButtonColumn ' DataGridViewButtonColumn

            columnName = NAMECOLUMNBUTTON1
            ActionButtonCol.DataPropertyName = columnName
            ActionButtonCol.Name = columnName
            ActionButtonCol.HeaderText = ""

            Me.bsAdjustsDataGridView.Columns.Add(ActionButtonCol)
            Me.bsAdjustsDataGridView.Columns(columnName).HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter
            Me.bsAdjustsDataGridView.Columns(columnName).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            Me.bsAdjustsDataGridView.Columns(columnName).HeaderCell.Style.BackColor = Me.BackColor
            Me.bsAdjustsDataGridView.Columns(columnName).DefaultCellStyle.BackColor = Me.BackColor
            Me.bsAdjustsDataGridView.Columns(columnName).Width = 75
            Me.bsAdjustsDataGridView.Columns(columnName).SortMode = DataGridViewColumnSortMode.NotSortable
            Me.bsAdjustsDataGridView.Columns(columnName).Tag = ""

            'creating own dataset to return
            column = New DataColumn()
            column.ColumnName = columnName
            table.Columns.Add(column)

            '
            'Icon Validation Column (Image)
            '
            Dim validateIconCol As New DataGridViewImageColumn

            columnName = NAMECOLUMNIMAGE
            validateIconCol.DataPropertyName = columnName
            validateIconCol.Name = columnName
            validateIconCol.Image = New Bitmap(16, 16)
            validateIconCol.ImageLayout = DataGridViewImageCellLayout.Normal

            validateIconCol.HeaderText = ""

            Me.bsAdjustsDataGridView.Columns.Add(validateIconCol)
            Me.bsAdjustsDataGridView.Columns(columnName).HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter
            Me.bsAdjustsDataGridView.Columns(columnName).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            Me.bsAdjustsDataGridView.Columns(columnName).HeaderCell.Style.BackColor = Me.BackColor
            Me.bsAdjustsDataGridView.Columns(columnName).DefaultCellStyle.BackColor = Me.BackColor
            Me.bsAdjustsDataGridView.Columns(columnName).Width = 40
            Me.bsAdjustsDataGridView.Columns(columnName).SortMode = DataGridViewColumnSortMode.NotSortable
            Me.bsAdjustsDataGridView.Columns(columnName).Tag = ""

            'creating own dataset to return
            column = New DataColumn()
            column.ColumnName = columnName
            table.Columns.Add(column)

            '
            'Parameter column(s) (Text)
            '
            If Me.numParams > 0 Then
                For i As Integer = 0 To Me.numParams - 1
                    Dim paramTextCol As New DataGridViewDisableTextBoxColumn ' DataGridViewTextBoxColumn
                    columnName = NAMECOLUMNSPARAMS(i)
                    paramTextCol.DataPropertyName = columnName
                    paramTextCol.Name = columnName
                    paramTextCol.HeaderText = columnName

                    Me.bsAdjustsDataGridView.Columns.Add(paramTextCol)
                    Me.bsAdjustsDataGridView.Columns(columnName).HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter
                    Me.bsAdjustsDataGridView.Columns(columnName).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
                    Me.bsAdjustsDataGridView.Columns(columnName).HeaderCell.Style.BackColor = Me.BackColor
                    Me.bsAdjustsDataGridView.Columns(columnName).DefaultCellStyle.BackColor = Color.White
                    Me.bsAdjustsDataGridView.Columns(columnName).Width = 65
                    Me.bsAdjustsDataGridView.Columns(columnName).SortMode = DataGridViewColumnSortMode.NotSortable
                    Me.bsAdjustsDataGridView.Columns(columnName).Tag = EDITABLECOLUMN

                    'creating own dataset to return
                    column = New DataColumn()
                    column.ColumnName = columnName
                    table.Columns.Add(column)
                Next
            End If

            '
            'Check Column (Button)
            '
            Dim CheckButtonCol As New DataGridViewDisableButtonColumn ' DataGridViewButtonColumn

            columnName = NAMECOLUMNBUTTON2
            CheckButtonCol.DataPropertyName = columnName
            CheckButtonCol.Name = columnName
            CheckButtonCol.HeaderText = ""

            Me.bsAdjustsDataGridView.Columns.Add(CheckButtonCol)
            Me.bsAdjustsDataGridView.Columns(columnName).HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter
            Me.bsAdjustsDataGridView.Columns(columnName).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            Me.bsAdjustsDataGridView.Columns(columnName).HeaderCell.Style.BackColor = Me.BackColor
            Me.bsAdjustsDataGridView.Columns(columnName).DefaultCellStyle.BackColor = Me.BackColor
            Me.bsAdjustsDataGridView.Columns(columnName).Width = 50
            Me.bsAdjustsDataGridView.Columns(columnName).SortMode = DataGridViewColumnSortMode.NotSortable
            Me.bsAdjustsDataGridView.Columns(columnName).Tag = ""

            'creating own dataset to return
            column = New DataColumn()
            column.ColumnName = columnName
            table.Columns.Add(column)

            ds.Tables.Add(table)
            Return ds
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    ''' <summary>
    ''' Populate Grid with content data from a Form
    ''' </summary>
    ''' <param name="ds">content data into dataset</param>
    ''' <remarks>
    ''' Created by XBC 14/12/2010
    ''' </remarks>
    Public Sub PopulateGrid(ByVal ds As DataSet)
        Try
            'Me.bsAdjustsDataGridView.DataSource = ds.Tables(0) - Not works with images and dynamic columns !
            Me.selectedRowAttribute = -1

            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                Dim r As Integer = Me.bsAdjustsDataGridView.Rows.Add()

                '
                'Identificator Column (Text)
                '
                Me.bsAdjustsDataGridView.Rows(r).Cells(IDENTIFICATOR).Value = ds.Tables(0).Rows(i)(0)

                '
                'Action Column (Button)
                '
                Me.bsAdjustsDataGridView.Rows(r).Cells(NAMECOLUMNBUTTON1).Value = ds.Tables(0).Rows(i)(1)
                EnableButton1(r) = False ' by default


                '
                'Icon Validation Column (Image)
                '
                Me.bsAdjustsDataGridView.Rows(r).Cells(NAMECOLUMNIMAGE).Tag = "False"

                '
                'Parameter column(s) (Text)
                '
                If Me.numParams > 0 Then
                    For j As Integer = 0 To Me.numParams - 1
                        Me.bsAdjustsDataGridView.Rows(r).Cells(NAMECOLUMNSPARAMS(j)).Value = ds.Tables(0).Rows(i)(3 + j)
                        Me.bsAdjustsDataGridView.Rows(r).Cells(NAMECOLUMNSPARAMS(j)).Tag = EDITABLECOLUMN
                        EnableCell(r, NAMECOLUMNSPARAMS(j)) = False ' by default
                    Next
                End If

                '
                'Check Column (Button)
                '
                Me.bsAdjustsDataGridView.Rows(r).Cells(NAMECOLUMNBUTTON2).Value = ds.Tables(0).Rows(i)(Me.numParams + 3)
                EnableButton2(r) = False ' by default
            Next
        Catch ex As Exception
            Throw ex
        End Try
    End Sub
#End Region

#Region "Event Handler"
    ''' <summary>
    ''' Datagrid Click event capture for any kind of cell
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>
    ''' Created by XBC 15/12/2010
    ''' </remarks>
    Public Sub CellClick(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles bsAdjustsDataGridView.CellClick
        Try
            If e.RowIndex >= 0 Then
                Dim cell As DataGridViewCell = Me.bsAdjustsDataGridView.Rows(e.RowIndex).Cells(e.ColumnIndex)
                If Me.bsAdjustsDataGridView.Columns(cell.ColumnIndex).Tag.ToString = EDITABLECOLUMN Then
                    ' Param Editable Cells 
                    If cell.ColumnIndex = e.ColumnIndex Then
                        ' Click on Text Cell
                        If cell.RowIndex = Me.selectedRowAttribute Then
                            cell.Selected = True
                            cell.Style.SelectionBackColor = Color.LightGreen
                            cell.Style.SelectionForeColor = Color.Black
                            Me.selectedValueAttribute = cell.Value.ToString

                            ' Send event to Form
                            RaiseEvent SelectedValueEvent(e.RowIndex, e.ColumnIndex)
                        Else
                            Me.bsAdjustsDataGridView.ClearSelection()
                        End If
                    End If
                Else
                    ' Button/Image Cells
                    Me.selectedValueAttribute = Nothing
                    cell.Style.SelectionBackColor = Me.BackColor
                    If cell.ColumnIndex = e.ColumnIndex Then
                        If Me.bsAdjustsDataGridView.Columns(cell.ColumnIndex).Name.Contains(nameColButton1) Then
                            ' Click on BUTTON 1
                            If EnableButton1(e.RowIndex) Then
                                Dim buttonCell As DataGridViewDisableButtonCell = CType(Me.bsAdjustsDataGridView.Rows(e.RowIndex).Cells(nameColButton1), DataGridViewDisableButtonCell)
                                ' Send event to Form
                                RaiseEvent Button1Click(buttonCell, e.RowIndex)
                            End If
                        End If
                        If Me.bsAdjustsDataGridView.Columns(cell.ColumnIndex).Name.Contains(nameColButton2) Then
                            ' Click on BUTTON 2 
                            If EnableButton2(e.RowIndex) Then
                                Me.selectedRowAttribute = -1
                                Dim buttonCell As DataGridViewDisableButtonCell = CType(Me.bsAdjustsDataGridView.Rows(e.RowIndex).Cells(nameColButton2), DataGridViewDisableButtonCell)
                                ' Send event to Form
                                RaiseEvent Button2Click(buttonCell, e.RowIndex)
                            End If
                        End If
                    End If
                End If
                Me.bsAdjustsDataGridView.Refresh()
            End If
        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    ''' <summary>
    ''' key press evaluation to ignoring everything that means displacement
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>
    ''' Created by XBC 15/12/2010
    ''' </remarks>
    Private Sub bsAdjustsDataGridView_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles bsAdjustsDataGridView.KeyDown
        Try
            'MessageBox.Show("Key pressed was " & e.KeyCode)
            If e.KeyCode = Keys.Up _
            Or e.KeyCode = Keys.Down _
            Or e.KeyCode = Keys.Right _
            Or e.KeyCode = Keys.Left _
            Or e.KeyCode = Keys.Tab _
            Or e.KeyCode = Keys.Prior _
            Or e.KeyCode = Keys.Next _
            Or e.KeyCode = Keys.Home _
            Or e.KeyCode = Keys.End Then
                e.Handled = True
            End If
        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    Private Sub BSGridControl_Resize(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Resize
        Me.bsAdjustsDataGridView.Width = MyBase.Width - 5
        Me.bsAdjustsDataGridView.Height = MyBase.Height - 5
    End Sub

    Private Sub DataGridView1_CellPainting(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellPaintingEventArgs) Handles bsAdjustsDataGridView.CellPainting
        Try
            If e.ColumnIndex >= 0 AndAlso e.RowIndex >= 0 Then

                e.Paint(e.CellBounds, DataGridViewPaintParts.All)

                If bsAdjustsDataGridView.Columns(e.ColumnIndex).Name = NAMECOLUMNBUTTON1 Then
                    If Me.AdjustButtonImage IsNot Nothing Then
                        e.Graphics.DrawImage(Me.AdjustButtonImage, e.CellBounds.X + 4, CInt(e.CellBounds.Y + (e.CellBounds.Height - 16) / 2), 16, 16)
                    End If
                End If

                If bsAdjustsDataGridView.Columns(e.ColumnIndex).Name = NAMECOLUMNBUTTON2 Then
                    If Me.TestButtonImage IsNot Nothing Then
                        e.Graphics.DrawImage(Me.TestButtonImage, e.CellBounds.X + 4, CInt(e.CellBounds.Y + (e.CellBounds.Height - 16) / 2), 16, 16)
                    End If
                End If

                e.Handled = True
            End If
            'NAMECOLUMNBUTTON1
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub

#End Region



End Class









