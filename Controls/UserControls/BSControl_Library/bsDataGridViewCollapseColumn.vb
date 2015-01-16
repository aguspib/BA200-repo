Imports System
Imports System.Drawing
Imports System.Windows.Forms
Imports Biosystems.Ax00.Global

Namespace Biosystems.Ax00.Controls.UserControls

    ''' <summary>
    ''' Custom column type dedicated to the bsDataGridViewCollapseCell cell type.
    ''' </summary>
    ''' <remarks>
    ''' Created by: RH 06/07/2010
    ''' </remarks>
    Public Class bsDataGridViewCollapseColumn
        Inherits DataGridViewColumn

#Region "Fields"

        Private PlusImage As Image
        Private MinusImage As Image
        Private NoImage As Image
        Private WithEvents headerPictureBox As PictureBox
        Private Enabled As Boolean
        Private Collapsed As Boolean = False
        Private RowsCountField As Integer = 0

        Public Event HeaderClickEventHandler As DataGridViewCellMouseEventHandler

#End Region

#Region "Properties"

        Public Overrides Property Frozen() As Boolean
            Get
                Return True
            End Get
            Set(ByVal value As Boolean)
                If Not value Then Throw New Exception("The Property 'bsDataGridViewCollapseColumn.Frozen' should always be True")
            End Set
        End Property

        Public Property IsEnabled() As Boolean
            Get
                Return Enabled
            End Get
            Set(ByVal value As Boolean)
                Enabled = value

                If Enabled Then
                    If Collapsed Then
                        headerPictureBox.Image = PlusImage
                    Else
                        headerPictureBox.Image = MinusImage
                    End If
                Else
                    headerPictureBox.Image = NoImage
                End If

                If Not DataGridView Is Nothing Then
                    For i As Integer = 0 To RowsCount - 1
                        CType(DataGridView(Index, i), bsDataGridViewCollapseCell).IsEnabled = Enabled
                    Next
                End If
            End Set
        End Property

        Public Property RowsCount() As Integer
            Get
                Return RowsCountField
            End Get
            Set(ByVal value As Integer)
                If value > DataGridView.Rows.Count OrElse value < 0 Then
                    RowsCountField = DataGridView.Rows.Count
                Else
                    RowsCountField = value
                End If
            End Set
        End Property

        Public Property IsCollapsed() As Boolean
            Get
                Return Collapsed
            End Get
            Set(ByVal value As Boolean)
                If value Then
                    CollapseAll()
                Else
                    ExpandAll()
                End If
            End Set
        End Property
#End Region

#Region "Public Methods"

        Public Sub New()
            MyBase.New(New bsDataGridViewCollapseCell())

            Try
                SortMode = DataGridViewColumnSortMode.NotSortable
                Resizable = DataGridViewTriState.False
                Width = 25
                MyBase.Frozen = True
                DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
                HeaderText = ""
                Enabled = True
                Collapsed = True

                ' XB 10/03/2014 - Add Try Catch section
            Catch ex As Exception
                ' Write into Log
                Dim myLogAcciones As New Biosystems.Ax00.Global.ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "bsDataGridViewCollapseColumn.New", EventLogEntryType.Error, False)
                ' Throw ex --> Do not work, no business catch this throw, just MyApplication_UnhandledException !
            End Try

        End Sub

        Public Sub CollapseAll()

            Try
                If Not Enabled Then Return

                headerPictureBox.Image = PlusImage
                Collapsed = True

                For i As Integer = 0 To RowsCount - 1
                    CType(DataGridView(Index, i), bsDataGridViewCollapseCell).IsCollapsed = Collapsed
                Next

                ' XB 10/03/2014 - Add Try Catch section
            Catch ex As Exception
                ' Write into Log
                Dim myLogAcciones As New Biosystems.Ax00.Global.ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "bsDataGridViewCollapseColumn.CollapseAll", EventLogEntryType.Error, False)
                ' Throw ex --> Do not work, no business catch this throw, just MyApplication_UnhandledException !
            End Try

        End Sub

        Public Sub ExpandAll()

            Try
                If Not Enabled Then Return

                headerPictureBox.Image = MinusImage
                Collapsed = False

                For i As Integer = 0 To RowsCount - 1
                    CType(DataGridView(Index, i), bsDataGridViewCollapseCell).IsCollapsed = Collapsed
                Next

                ' XB 10/03/2014 - Add Try Catch section
            Catch ex As Exception
                ' Write into Log
                Dim myLogAcciones As New Biosystems.Ax00.Global.ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "bsDataGridViewCollapseColumn.ExpandAll", EventLogEntryType.Error, False)
                ' Throw ex --> Do not work, no business catch this throw, just MyApplication_UnhandledException !
            End Try

        End Sub
#End Region

#Region "Protected Methods"

        Protected Overrides Sub OnDataGridViewChanged()

            Try
                If Me.DataGridView Is Nothing Then Return

                InitializeHeaderPicBox()
                Me.DataGridView.Controls.Add(headerPictureBox)
                IsEnabled = Enabled

                ' TR 02/08/2011 -One type has instances that are disposed and directly rooted by an EventHandler. 
                'This often indicates that an EventHandler has not been properly removed and is a common cause of memory leaks.
                RemoveHandler DataGridView.Invalidated, AddressOf Validating
                AddHandler DataGridView.Invalidated, AddressOf Validating

                ' XB 10/03/2014 - Add Try Catch section
            Catch ex As Exception
                ' Write into Log
                Dim myLogAcciones As New Biosystems.Ax00.Global.ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "bsDataGridViewCollapseColumn.OnDataGridViewChanged", EventLogEntryType.Error, False)
                ' Throw ex --> Do not work, no business catch this throw, just MyApplication_UnhandledException !
            End Try

        End Sub

        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            Try
                If disposing Then
                    headerPictureBox = Nothing
                End If
            Finally
                MyBase.Dispose(disposing)
            End Try
        End Sub
#End Region

#Region "Private Methods"

        Private Sub InitializeHeaderPicBox()

            Try
                PlusImage = My.Resources.expand1 '.plus_16
                MinusImage = My.Resources.collapse1 '.minus_16
                NoImage = My.Resources.noimage

                headerPictureBox = New PictureBox()
                'headerPictureBox.BackColor = System.Drawing.Color.DarkGray
                headerPictureBox.Name = "headerPictureBox"
                headerPictureBox.SizeMode = PictureBoxSizeMode.CenterImage
                headerPictureBox.BackColor = Color.Transparent
                headerPictureBox.TabStop = False
                headerPictureBox.Image = PlusImage

                ' XB 10/03/2014 - Add Try Catch section
            Catch ex As Exception
                ' Write into Log
                Dim myLogAcciones As New Biosystems.Ax00.Global.ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "bsDataGridViewCollapseColumn.InitializeHeaderPicBox", EventLogEntryType.Error, False)
                ' Throw ex --> Do not work, no business catch this throw, just MyApplication_UnhandledException !
            End Try

        End Sub

        Private Sub Validating(ByVal sender As Object, ByVal e As InvalidateEventArgs)

            Try
                RefreshHeader()

                ' XB 10/03/2014 - Add Try Catch section
            Catch ex As Exception
                ' Write into Log
                Dim myLogAcciones As New Biosystems.Ax00.Global.ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "bsDataGridViewCollapseColumn.Validating", EventLogEntryType.Error, False)
                ' Throw ex --> Do not work, no business catch this throw, just MyApplication_UnhandledException !
            End Try

        End Sub

        Private Sub RefreshHeader()

            Try
                If DataGridView Is Nothing Then Return

                'If Displayed Then
                'headerPictureBox.Size = New Size(Width - 1, DataGridView.ColumnHeadersHeight - 2)

                headerPictureBox.Size = New Size(PlusImage.Width, PlusImage.Height)

                Dim OffsetWidth As Integer = (Width - PlusImage.Width) / 2 + 1
                Dim OffsetHeight As Integer = (DataGridView.ColumnHeadersHeight - PlusImage.Height) / 2 + 2

                Dim LeftPadding As Integer = OffsetWidth
                For i As Integer = 0 To Me.Index - 1
                    LeftPadding += DataGridView.Columns(i).Width + DataGridView.Columns(i).DividerWidth
                Next

                headerPictureBox.Location = New System.Drawing.Point(LeftPadding, OffsetHeight)
                headerPictureBox.Visible = True
                'Else
                'headerPictureBox.Visible = False
                'End If

                ' XB 10/03/2014 - Add Try Catch section
            Catch ex As Exception
                ' Write into Log
                Dim myLogAcciones As New Biosystems.Ax00.Global.ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "bsDataGridViewCollapseColumn.RefreshHeader", EventLogEntryType.Error, False)
                ' Throw ex --> Do not work, no business catch this throw, just MyApplication_UnhandledException !
            End Try

        End Sub

        Private Sub headerPictureBox_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles headerPictureBox.Click

            Try
                If Not Enabled Then Return

                Collapsed = Not Collapsed

                If Collapsed Then
                    headerPictureBox.Image = PlusImage
                Else
                    headerPictureBox.Image = MinusImage
                End If

                For i As Integer = 0 To RowsCount - 1
                    CType(DataGridView(Index, i), bsDataGridViewCollapseCell).IsCollapsed = Collapsed
                Next

                Dim mouseArg As MouseEventArgs = New MouseEventArgs(MouseButtons.Left, 1, 1, 1, 0)
                Dim cellArg As New DataGridViewCellMouseEventArgs(Index, -1, 0, 0, mouseArg)
                RaiseEvent HeaderClickEventHandler(Me.DataGridView, cellArg)

                ' XB 10/03/2014 - Add Try Catch section
            Catch ex As Exception
                ' Write into Log
                Dim myLogAcciones As New Biosystems.Ax00.Global.ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "bsDataGridViewCollapseColumn.headerPictureBox_Click", EventLogEntryType.Error, False)
                ' Throw ex --> Do not work, no business catch this throw, just MyApplication_UnhandledException !
            End Try

        End Sub

#End Region

    End Class

End Namespace