Option Strict On
Option Explicit On

Imports System.Drawing
Imports System.Windows.Forms

Namespace Biosystems.Ax00.Controls.UserControls

    Public Class BsDoubleListOld

#Region "Attributes"

        Private leftListViewEvent As System.Windows.Forms.ColumnClickEventArgs
        Private rightListViewEvent As System.Windows.Forms.ColumnClickEventArgs
        'My delegate to handler the event to be throw when click on List elements.
        Public Delegate Sub ClickElementEventHandler()

        'declaring the event handler 
        Public Event CallBackEvent As ClickElementEventHandler

        Private SelectedElementsAllDataSet As New DataSet
        Private SelectedElementsDataSet As New DataSet

        Private ListItemsSelDataTable As New DataTable
        Private ListItemsDataTable As New DataTable
        Private dataTableList As New DataTable
        Private dataTableRight As New DataTable

        Private dataRowItem As DataRow
        Private dataRowList As DataRow
        Private dataRowRight As DataRow

        Private tooltipMessage As New ToolTip 'To display the ToolTip text Message for Clik Events

        Private rightImageList As New ImageList
        Private leftImageList As New ImageList

        Private strCode As String
        Private strPath As String

        Private imageIndexLeftListView As Integer
        Private imageIndexRightListView As Integer

        Private multiSelectOption As Boolean
        Private dbNullValue As Boolean
        Private sortedAttibute As Boolean

#End Region

#Region "Property"

        ''' <summary>
        ''' For SelectableElemnts Lable Title
        ''' </summary>
        ''' <value></value>
        ''' <remarks></remarks>
        Public Property SelectableElementsTitle() As String
            Get
                Return BsLeftLabel.Text
            End Get
            Set(ByVal Value As String)
                BsLeftLabel.Text = Value
            End Set
        End Property

        ''' <summary>
        ''' To Fill SelectableElements Listview Data
        ''' </summary>
        ''' <value></value>
        ''' <remarks></remarks>
        Public WriteOnly Property SelectableElements() As DataSet
            Set(ByVal Value As DataSet)
                FillSelectableElements(Value)
            End Set
        End Property

        ''' <summary>
        ''' For SelectedElements Lable Title
        ''' </summary>
        ''' <value></value>
        ''' <remarks></remarks>
        Public WriteOnly Property SelectedElementsTitle() As String
            Set(ByVal Value As String)
                BsRightLabel.Text = Value
            End Set
        End Property

        ''' <summary>
        ''' To Fill SelectedElements Listview Data
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>Added By vanitha</remarks>
        Public Property SelectedElements() As DataSet
            Get
                SelectedElements = GetSelectedElementsDS() 'To Return the SelectedElements Datset
            End Get
            Set(ByVal Value As DataSet)
                SelectedElementsDataSet = Value
                FillSelectedElements(SelectedElementsDataSet)
            End Set
        End Property

        ''' <summary>
        ''' To set multiSelectOptionection for both Listview
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property multiSelection() As Boolean
            Get
                Return multiSelectOption
            End Get
            Set(ByVal value As Boolean)
                multiSelectOption = value
                If value = False Then
                    Me.BsLeftListView.MultiSelect = False
                    Me.BsRightListView.MultiSelect = False
                Else
                    Me.BsLeftListView.MultiSelect = True
                    Me.BsRightListView.MultiSelect = True
                End If
            End Set
        End Property

        ''' <summary>
        ''' To set Sort Order for both Listview
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Sorted() As Boolean
            Get
                Return (sortedAttibute)
            End Get
            Set(ByVal value As Boolean)
                sortedAttibute = value
                If value = True Then
                    Me.BsLeftListView.Sorting = SortOrder.Ascending
                    Me.BsRightListView.Sorting = SortOrder.Ascending
                    Call BsLeftListView_ColumnClick(BsLeftListView, leftListViewEvent)              'Call the listview cloumn click event for sorting the data
                    Call BsRightListView_ColumnClick(BsRightListView, rightListViewEvent)
                End If
            End Set
        End Property

        ''' <summary>
        ''' To set the ToolTip for BsMoveLeftToRightAll >> Button
        ''' </summary>
        ''' <value></value>
        ''' <remarks></remarks>
        Public WriteOnly Property SelectAllToolTip() As String
            Set(ByVal value As String)
                tooltipMessage.SetToolTip(Me.BsMoveLeftToRightAllButton, value)
                tooltipMessage.Active = True
            End Set
        End Property

        ''' <summary>
        ''' To set the ToolTip for BsMoveLeftToRightSelectedItem > Button
        ''' </summary>
        ''' <value></value>
        ''' <remarks></remarks>
        Public WriteOnly Property SelectSomeToolTip() As String
            Set(ByVal value As String)
                tooltipMessage.SetToolTip(Me.BsMoveLeftToRightSelectedItemButton, value)
                tooltipMessage.Active = True
            End Set
        End Property

        ''' <summary>
        ''' To set the ToolTip for BsMoveRightToLeftSelectedItem Button
        ''' </summary>
        ''' <value></value>
        ''' <remarks></remarks>
        Public WriteOnly Property UnselectSomeToolTip() As String
            Set(ByVal value As String)
                tooltipMessage.SetToolTip(Me.BsMoveRightToLeftSelectedItemButton, value)
                tooltipMessage.Active = True
            End Set
        End Property

        ''' <summary>
        ''' To set the ToolTip for BsMoveRightToLeftAll Button
        ''' </summary>
        ''' <value></value>
        ''' <remarks></remarks>
        Public WriteOnly Property UnselectAllToolTip() As String
            Set(ByVal value As String)
                tooltipMessage.SetToolTip(Me.BsMoveRightToLeftAllButton, value)
                tooltipMessage.Active = True
            End Set
        End Property

        ''' <summary>
        ''' To Get The Code For DoubleClickedElement In Listview
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property DoubleClickedElement() As String
            Get
                DoubleClickedElement = strCode           'Return value that generated from Double-Click event from the ListView
            End Get
        End Property

        ''' <summary>
        ''' To Get the Height of Listview
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property ListViewHeight() As String
            Get
                ListViewHeight = BsLeftListView.Height & "," & BsRightListView.Height
            End Get
        End Property

        ''' <summary>
        ''' To Get The Width of Listview
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property ListViewWidth() As String
            Get
                ListViewWidth = BsLeftListView.Width & "," & BsRightListView.Width
            End Get
        End Property

#End Region

#Region "Constuctor"

        Public Sub New()
            ' This call is required by the Windows Form Designer.
            InitializeComponent()
            ' Add any initialization after the InitializeComponent() call.
            InitializeSelectableLst()
            InitializeSelectedLst()

        End Sub

#End Region

#Region "Sub & Function"

        ''' <summary>
        ''' This procedure fills the Selectable list view from SelectableElements property.
        ''' </summary>
        ''' <param name="dsSelectableElements">FillSelectableElements</param>
        ''' <remarks></remarks>
        Public Sub FillSelectableElements(ByVal dsSelectableElements As DataSet)
            Try
                ' Clear the ListView control 
                BsLeftListView.Items.Clear()

                'checking whether the dataset is empty or not.
                If Not (dsSelectableElements Is Nothing) Then
                    If dsSelectableElements.Tables.Count > 0 Then
                        ListItemsSelDataTable = dsSelectableElements.Tables(0)

                        ' Display items in the ListView control 
                        For d As Integer = 0 To ListItemsSelDataTable.Rows.Count - 1

                            dataRowItem = ListItemsSelDataTable.Rows(d)
                            If dataRowItem.RowState <> DataRowState.Deleted Then

                                'Checking whether the icon column has the value or not
                                If (Not (dsSelectableElements.Tables(0).Rows(d).Item("ElementIcon").Equals(System.DBNull.Value))) And _
                                        (dsSelectableElements.Tables(0).Rows(d).Item("ElementIcon").ToString <> "") Then                     'Item(3)-Icon column
                                    'Checking imagelist count for assigning image index.
                                    If leftImageList.Images.Count = 0 Then
                                        imageIndexLeftListView = 0
                                    Else
                                        imageIndexLeftListView = imageIndexLeftListView + 1
                                    End If
                                    'Add icon to imagelist with image key.
                                    leftImageList.Images.Add(CType(imageIndexLeftListView, String), _
                                                             Image.FromFile(CStr(dsSelectableElements.Tables(0).Rows(d).Item("ElementIcon"))))
                                    dbNullValue = False
                                Else
                                    dbNullValue = True
                                End If

                                BsLeftListView.SmallImageList = leftImageList
                                BsLeftListView.View = View.Details
                                If dbNullValue = True Then
                                    BsLeftListView.Items.Add(dataRowItem("ElementCode").ToString, dataRowItem("ElementDesc").ToString, "")
                                Else
                                    'if icon exists assign icon to the listview item.(last parameter)
                                    BsLeftListView.Items.Add(dataRowItem("ElementCode").ToString, dataRowItem("ElementDesc").ToString, _
                                                             leftImageList.Images.Keys(imageIndexLeftListView))
                                End If
                                BsLeftListView.Refresh()
                            End If
                        Next
                    End If
                End If
            Catch ex As Exception
                MsgBox(ex.ToString)
            Finally
                dsSelectableElements = Nothing
            End Try
        End Sub

        ''' <summary>
        ''' This procedure fills the Selected list view from SelectedElements property.
        ''' </summary>
        ''' <param name="dsSelectedElements">FillSelectedElements</param>
        ''' <remarks></remarks>
        Public Sub FillSelectedElements(ByVal dsSelectedElements As DataSet)
            Try
                ' Clear the ListView control 
                BsRightListView.Items.Clear()

                'checking whether the dataset is empty or not.
                If Not (dsSelectedElements Is Nothing) Then
                    If dsSelectedElements.Tables.Count > 0 Then
                        ListItemsDataTable = dsSelectedElements.Tables(0)

                        ' Display items in the ListView control 
                        For d As Integer = 0 To ListItemsDataTable.Rows.Count - 1

                            dataRowItem = ListItemsDataTable.Rows(d)
                            If dataRowItem.RowState <> DataRowState.Deleted Then
                                'Checking whether the icon column has the value or not
                                If (Not (dsSelectedElements.Tables(0).Rows(d).Item("ElementIcon").Equals(System.DBNull.Value))) And _
                                        (dsSelectedElements.Tables(0).Rows(d).Item("ElementIcon").ToString <> "") Then
                                    'Checking imagelist count for assigning image index.
                                    If rightImageList.Images.Count = 0 Then
                                        imageIndexRightListView = 0
                                    Else
                                        imageIndexRightListView = imageIndexRightListView + 1
                                    End If
                                    'Add icon to imagelist with image key.
                                    rightImageList.Images.Add(CType(imageIndexRightListView, String), _
                                    Image.FromFile(CStr(dsSelectedElements.Tables(0).Rows(d).Item("ElementIcon"))))
                                    dbNullValue = False
                                Else
                                    dbNullValue = True
                                End If
                                BsRightListView.SmallImageList = rightImageList
                                BsRightListView.View = View.Details
                                If dbNullValue = True Then
                                    BsRightListView.Items.Add(dataRowItem("ElementCode").ToString, dataRowItem("ElementDesc").ToString, "")
                                Else
                                    'if icon exists assign icon to the listview item.(last parameter)
                                    BsRightListView.Items.Add(dataRowItem("ElementCode").ToString, dataRowItem("ElementDesc").ToString, rightImageList.Images.Keys(imageIndexRightListView))
                                End If
                                BsRightListView.Refresh()
                            End If
                        Next
                    End If
                End If
            Catch ex As Exception
                MsgBox(ex.ToString)
            Finally
                dsSelectedElements = Nothing
            End Try
        End Sub

        ''' <summary>
        ''' Here We initialize the list view.Add column headers for SelectableListView.
        ''' Set the property of SelectableListView
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub InitializeSelectableLst()

            BsLeftListView.View = View.Details

            ' Allow the user to rearrange columns. 
            BsLeftListView.AllowColumnReorder = True

            ' Select the item and subitems when selection is made. 
            BsLeftListView.FullRowSelect = True

            ' Sort the items in the list in ascending order. 
            BsLeftListView.Sorting = SortOrder.Ascending

            'Assign imagelist to listview.
            BsLeftListView.SmallImageList = leftImageList

            BsLeftListView.View = View.Details

            'Column width assigned as BsLeftListView.Size.Width - 4 for reducing horizontal scroll bar.
            BsLeftListView.Columns.Add("", BsLeftListView.Size.Width - 4, HorizontalAlignment.Left)

            BsLeftListView.Items.Clear()

        End Sub

        ''' <summary>
        ''' Here We initialize the list view.Add column headers for SelectedListView.
        ''' Set the property of SelectedListView
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub InitializeSelectedLst()
            BsRightListView.Items.Clear()
            BsRightListView.View = View.Details

            ' Allow the user to rearrange columns. 
            BsRightListView.AllowColumnReorder = True

            ' Select the item and subitems when selection is made. 
            BsRightListView.FullRowSelect = True

            ' Sort the items in the list in ascending order. 
            BsRightListView.Sorting = SortOrder.Ascending

            'Assign imagelist ti listview.
            BsRightListView.SmallImageList = leftImageList
            BsRightListView.View = View.Details

            'Column width assigned as BsLeftListView.Size.Width - 4 for reducing horizontal scroll bar.
            BsRightListView.Columns.Add("", BsRightListView.Size.Width - 4, HorizontalAlignment.Left)
        End Sub

        ''' <summary>
        ''' To Return the SelectedElements Dataset to the SelectedElements Property
        ''' </summary>
        ''' <returns>Dataset</returns>
        ''' <remarks>Added By Vanitha</remarks>
        Public Function GetSelectedElementsDS() As DataSet
            Dim result As DataSet = New DataSet
            Try
                'Checking whether selected list is empty or not.
                If BsRightListView.Items.Count > 0 Then
                    dataTableRight.Columns.Clear()
                    dataTableRight.Columns.Add("Code")
                    dataTableRight.Columns.Add("Description")
                    BsRightListView.Refresh()
                    dataTableRight.Rows.Clear()

                    'Assign the selected list values to dataset for returning purpose.
                    For rightLstCount As Integer = 0 To Me.BsRightListView.Items.Count - 1
                        dataRowRight = dataTableRight.NewRow
                        dataRowRight.Item(0) = BsRightListView.Items(rightLstCount).Name
                        dataRowRight.Item(1) = BsRightListView.Items(rightLstCount).Text
                        dataTableRight.Rows.Add(dataRowRight)
                    Next
                    If SelectedElementsAllDataSet.Tables.Count > 0 Then
                        SelectedElementsAllDataSet.Tables.Clear()
                    End If
                    result.Tables.Add(dataTableRight)
                    result.AcceptChanges()

                End If
            Catch ex As Exception
                MessageBox.Show(ex.Message)
            End Try
            Return result
        End Function

        ''' <summary>
        ''' To get the Path of the Image from the selectableElements/selectedElements Dataset
        ''' </summary>
        ''' <param name="code"></param>
        ''' <param name="Desc"></param>
        ''' <returns>String</returns>
        ''' <remarks>Added By Vanitha</remarks>
        Public Function GetPath(ByVal code As String, ByVal Desc As String) As String
            Dim result As String = ""
            Try
                If Me.BsRightListView.Items.Count > 0 Then

                    'Get the file path of selected item from selectable list.
                    For selItem As Integer = 0 To ListItemsSelDataTable.Rows.Count - 1
                        If ListItemsSelDataTable.Rows(selItem).Item("Code").ToString = code And ListItemsSelDataTable.Rows(selItem).Item("Description").ToString = Desc Then
                            result = ListItemsSelDataTable.Rows(selItem).Item("FilePath").ToString
                        End If
                    Next
                    'Get the file path of selected item from selected list if path not exists in selecatable list.
                    For selectedItem As Integer = 0 To ListItemsDataTable.Rows.Count - 1
                        If ListItemsDataTable.Rows(selectedItem).Item("Code").ToString = code And ListItemsDataTable.Rows(selectedItem).Item("Description").ToString = Desc Then
                            result = ListItemsDataTable.Rows(selectedItem).Item("FilePath").ToString
                        End If
                    Next
                End If
            Catch ex As Exception
                Throw ex
            End Try

            Return result
        End Function

#End Region

#Region "Events"
        ''' <summary>
        ''' While clicking this button all items moved Left to Right ListView
        ''' </summary>
        ''' <param name="sender">BsMoveLeftToRightAllButton</param>
        ''' <param name="e">Click</param>
        ''' <remarks></remarks>
        Private Sub BsMoveLeftToRightAllButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsMoveLeftToRightAllButton.Click
            Try
                If Sorted = False Then
                    BsRightListView.Sorting = SortOrder.None
                Else
                    BsRightListView.Sorting = SortOrder.Ascending
                End If
                If BsLeftListView.Items.Count > 0 Then
                    For iCount As Integer = 0 To BsLeftListView.Items.Count - 1
                        'Checking whether selectablelist having icon or not.
                        If IsNothing(leftImageList.Images.Item(BsLeftListView.Items(iCount).ImageKey)) = False Then
                            'If yes add icon to the image list.
                            rightImageList.Images.Add((rightImageList.Images.Keys.Count + 1).ToString, _
                                                      leftImageList.Images.Item(BsLeftListView.Items(iCount).ImageKey))
                            'Add icon to the selectedlist.
                            BsRightListView.Items.Add(BsLeftListView.Items(iCount).Name.ToString, BsLeftListView.Items(iCount).Text.ToString, _
                                                      rightImageList.Images.Keys(rightImageList.Images.Keys.Count - 1))
                        Else
                            'If icon not exists, set empty icon for listview item.(last parameter).
                            BsRightListView.Items.Add(BsLeftListView.Items(iCount).Name.ToString, BsLeftListView.Items(iCount).Text.ToString, "")
                        End If
                    Next
                    'clear the selectable list.
                    For iCount As Integer = 0 To BsLeftListView.Items.Count - 1
                        BsLeftListView.Items.RemoveAt(0)
                    Next
                End If

                If Sorted = True Then
                    Call BsRightListView_ColumnClick(BsRightListView, rightListViewEvent)
                End If
            Catch ex As Exception
                MsgBox(ex.ToString)
            End Try
        End Sub

        ''' <summary>
        ''' While clicking this button all selected items moved Left to Right ListView
        ''' </summary>
        ''' <param name="sender">BsMoveLeftToRightSelectedItemButton</param>
        ''' <param name="e">Click</param>
        ''' <remarks></remarks>
        Private Sub BsMoveLeftToRightSelectedItemButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsMoveLeftToRightSelectedItemButton.Click
            Try
                If (BsLeftListView.Items.Count > 0) And (BsLeftListView.SelectedItems.Count > 0) Then
                    If Sorted = False Then
                        BsRightListView.Sorting = SortOrder.None
                    Else
                        BsRightListView.Sorting = SortOrder.Ascending
                    End If
                    For iCount As Integer = 0 To BsLeftListView.SelectedItems.Count - 1
                        'Check whether list having icon or not.
                        If IsNothing(leftImageList.Images.Item(BsLeftListView.SelectedItems.Item(0).ImageKey)) = False Then
                            'Add icon to the image list.
                            rightImageList.Images.Add((rightImageList.Images.Keys.Count + 1).ToString, _
                                                       leftImageList.Images.Item(BsLeftListView.SelectedItems.Item(0).ImageKey))
                            BsRightListView.Items.Add(BsLeftListView.SelectedItems(0).Name, BsLeftListView.SelectedItems(0).Text.ToString, _
                                                      rightImageList.Images.Keys(rightImageList.Images.Keys.Count - 1))
                        Else
                            'Set icon as empty(last parameter).
                            BsRightListView.Items.Add(BsLeftListView.SelectedItems(0).Name, BsLeftListView.SelectedItems(0).Text.ToString, "")
                        End If
                        'Remove the selected item.
                        BsLeftListView.SelectedItems(0).Remove()
                    Next
                End If
                If Sorted = True Then
                    Call BsRightListView_ColumnClick(BsRightListView, rightListViewEvent)
                End If
            Catch ex As Exception
                MsgBox(ex.ToString)
            End Try
        End Sub

        ''' <summary>
        ''' While clicking this button all Selected items moved Right to Left ListView
        ''' </summary>
        ''' <param name="sender">BsMoveRightToLeftSelectedItemButton</param>
        ''' <param name="e">Click</param>
        ''' <remarks></remarks>
        Private Sub BsMoveRightToLeftSelectedItemButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsMoveRightToLeftSelectedItemButton.Click
            Try
                If Sorted = False Then
                    BsLeftListView.Sorting = SortOrder.None
                Else
                    BsLeftListView.Sorting = SortOrder.Ascending
                End If
                If (BsRightListView.Items.Count > 0) And (BsRightListView.SelectedItems.Count > 0) Then
                    For iCount As Integer = 0 To BsRightListView.SelectedItems.Count - 1
                        'Check whether list having icon or not.
                        If IsNothing(rightImageList.Images.Item(BsRightListView.SelectedItems.Item(0).ImageKey)) = False Then
                            'Add icon to the image list.
                            leftImageList.Images.Add((leftImageList.Images.Keys.Count + 1).ToString, _
                                                      rightImageList.Images.Item(BsRightListView.SelectedItems.Item(0).ImageKey))
                            BsLeftListView.Items.Add(BsRightListView.SelectedItems(0).Name, BsRightListView.SelectedItems(0).Text.ToString, _
                                                     leftImageList.Images.Keys(leftImageList.Images.Keys.Count - 1))
                        Else
                            'Set icon as empty(last parameter).
                            BsLeftListView.Items.Add(BsRightListView.SelectedItems(0).Name, BsRightListView.SelectedItems(0).Text.ToString, "")
                        End If
                        'Remove the selected item.
                        BsRightListView.SelectedItems.Item(0).Remove()
                    Next
                End If
                If Sorted = True Then
                    Call BsLeftListView_ColumnClick(BsLeftListView, leftListViewEvent)
                End If
            Catch ex As Exception
                MsgBox(ex.ToString)
            End Try
        End Sub

        ''' <summary>
        ''' While clicking this button allitems moved Right to Left ListView
        ''' </summary>
        ''' <param name="sender">BsMoveRightToLeftAllButton</param>
        ''' <param name="e">Click</param>
        ''' <remarks></remarks>
        Private Sub BsMoveRightToLeftAllButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsMoveRightToLeftAllButton.Click
            Try
                If Sorted = False Then
                    BsLeftListView.Sorting = SortOrder.None
                Else
                    BsLeftListView.Sorting = SortOrder.Ascending
                End If
                If BsRightListView.Items.Count > 0 Then
                    For iCount As Integer = 0 To BsRightListView.Items.Count - 1
                        'Check whether list having icon or not.
                        If IsNothing(rightImageList.Images.Item(BsRightListView.Items(iCount).ImageKey)) = False Then
                            'Add icon to the image list.
                            leftImageList.Images.Add((leftImageList.Images.Keys.Count + 1).ToString, _
                                                      rightImageList.Images.Item(BsRightListView.Items(iCount).ImageKey))
                            BsLeftListView.Items.Add(BsRightListView.Items(iCount).Name.ToString, BsRightListView.Items(iCount).Text.ToString, _
                                                     leftImageList.Images.Keys(leftImageList.Images.Keys.Count - 1))
                        Else
                            'Set icon as empty(last parameter).
                            BsLeftListView.Items.Add(BsRightListView.Items(iCount).Name.ToString, BsRightListView.Items(iCount).Text.ToString, "")
                        End If
                    Next
                    'clear the selected list.
                    For iCount As Integer = 0 To BsRightListView.Items.Count - 1
                        BsRightListView.Items.RemoveAt(0)
                    Next
                End If
                If Sorted = True Then
                    Call BsLeftListView_ColumnClick(BsLeftListView, leftListViewEvent)
                End If
            Catch ex As Exception
                MsgBox(ex.ToString)
            End Try
        End Sub

        ''' <summary>
        ''' By clicking Right ListView to set sortOrder Ascending
        ''' </summary>
        ''' <param name="sender">BsRightListView</param>
        ''' <param name="e">ColumnClick</param>
        ''' <remarks></remarks>
        Private Sub BsRightListView_ColumnClick(ByVal sender As Object, ByVal e As System.Windows.Forms.ColumnClickEventArgs) Handles _
                                                BsRightListView.ColumnClick
            Try
                BsRightListView.Sorting = SortOrder.Ascending
            Catch ex As Exception
                MsgBox(ex.ToString)
            End Try
        End Sub

        ''' <summary>
        ''' By clicking Left ListView to set sortOrder Ascending
        ''' </summary>
        ''' <param name="sender">BsLeftListView</param>
        ''' <param name="e">ColumnClick</param>
        ''' <remarks></remarks>
        Private Sub BsLeftListView_ColumnClick(ByVal sender As Object, ByVal e As System.Windows.Forms.ColumnClickEventArgs) Handles _
                                               BsLeftListView.ColumnClick
            Try
                BsLeftListView.Sorting = SortOrder.Ascending
            Catch ex As Exception
                MsgBox(ex.ToString)
            End Try
        End Sub

        ''' <summary>
        ''' While double clicking particular item of the Right Listview then you can get the code of the item
        ''' </summary>
        ''' <param name="sender">BsRightListView</param>
        ''' <param name="e">DoubleClick</param>
        ''' <remarks></remarks>
        Public Sub BsRightListView_DoubleClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles BsRightListView.DoubleClick
            Try
                strCode = BsRightListView.SelectedItems(0).Name             'Return the code of the selected item
                RaiseEvent CallBackEvent()
            Catch ex As Exception
                MsgBox(ex.ToString)
            End Try
        End Sub

        ''' <summary>
        ''' While double clicking particular item of the Left Listview then you can get the code of the item
        ''' </summary>
        ''' <param name="sender">BsLeftListView</param>
        ''' <param name="e">DoubleClick</param>
        ''' <remarks></remarks>
        Public Sub BsLeftListView_DoubleClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles BsLeftListView.DoubleClick
            Try
                strCode = BsLeftListView.SelectedItems(0).Name              'Return the code of the selected item
                RaiseEvent CallBackEvent()
            Catch ex As Exception
                MsgBox(ex.ToString)
            End Try
        End Sub


#End Region

    End Class

End Namespace


