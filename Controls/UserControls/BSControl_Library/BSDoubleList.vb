Option Explicit On
Option Strict On

Imports System.Windows.Forms
Imports System.Drawing

Namespace Biosystems.Ax00.Controls.UserControls

    Public Class BSDoubleList

#Region "Declarations"
        'To control sorting of ListViews
        Private leftListViewEvent As System.Windows.Forms.ColumnClickEventArgs
        Private rightListViewEvent As System.Windows.Forms.ColumnClickEventArgs

        'To manage the ToolTips of the movement buttons
        Private BSDoubleClickToolTips As New ToolTip
#End Region

#Region "Attributes"
        Private TypeAttribute As String
        Private SelectableElementsAttribute As DataSet
        Private SelectedElementsTitleAttribute As String

        Private SelectedElementsAttribute As DataSet
        Private SelectableElementsTitleAttribute As String

        Private SortedAttribute As Boolean
        Private MultiSelectionAttribute As Boolean
        Private ListViewsIconsAttribute As ImageList
        Private DoubleClickedElementAttribute As String

        Private SelectAllToolTipAttribute As String
        Private SelectSomeToolTipAttribute As String
        Private UnselectSomeToolTipAttribute As String
        Private UnselectAllToolTipAttribute As String


        'TR 09/03/2011 -Message used to indicate the factory values
        Private FactoryValuesMessageAttribute As String = ""
#End Region

#Region "Properties"

        ''' <summary>
        ''' Property use to recive the factory value message.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>CREATED BY: TR 09/03/2011</remarks>
        Public Property FactoryValueMessage() As String
            Get
                Return FactoryValuesMessageAttribute
            End Get
            Set(ByVal value As String)
                FactoryValuesMessageAttribute = value
            End Set
        End Property

        ''' <summary>
        ''' Type of Elements to shown in the list of SelectableElements; once informed, load the Selectable
        ''' Elements ListView with Elements of this specific Type
        ''' </summary>
        Public WriteOnly Property TypeToShow() As String
            Set(ByVal value As String)
                TypeAttribute = value
                FillSelectableElementsListView()
            End Set
        End Property

        ''' <summary>
        ''' List of Selectable Elements (to load the left list)
        ''' </summary>
        Public Property SelectableElements() As DataSet
            Get
                Return SelectableElementsAttribute
            End Get
            Set(ByVal value As DataSet)
                If (value Is Nothing) Then
                    PrepareSelectableElements()
                Else
                    SelectableElementsAttribute = value
                End If
            End Set
        End Property


        ' DL 10/11/2010
        Public WriteOnly Property SelectAllButtonImage() As Image
            Set(ByVal value As Image)
                SelectALLSelectableElementsButton.Image = value
            End Set
        End Property

        ' DL 10/11/2010
        Public WriteOnly Property SelectChosenButtonImage() As Image
            Set(ByVal value As Image)
                SelectChosenButton.Image = value
            End Set
        End Property

        ' DL 10/11/2010
        Public WriteOnly Property UnselectChosenButtonImage() As Image
            Set(ByVal value As Image)
                UnselectChosenSelectedElementButton.Image = value
            End Set
        End Property

        ' DL 10/11/2010
        Public WriteOnly Property UnselectAllButtonImage() As Image
            Set(ByVal value As Image)
                UnselectAllSelectedElementsButton.Image = value
            End Set
        End Property


        ''' <summary>
        '''Enable or disable controls in double list except labels
        ''' </summary>
        ''' <remarks>
        ''' Created by: DL 19/07/2011
        ''' </remarks>
        Public WriteOnly Property EnableDisableControls() As Boolean
            Set(ByVal value As Boolean)
                SelectableElementsListView.Enabled = value
                SelectedElementsListView.Enabled = value
                SelectChosenButton.Enabled = value
                UnselectChosenSelectedElementButton.Enabled = value
                SelectALLSelectableElementsButton.Enabled = value
                UnselectAllSelectedElementsButton.Enabled = value
            End Set
        End Property


        ''' <summary>
        ''' Title that must be shown for the list containing the available selectable elements (left list) 
        ''' </summary>
        Public Property SelectableElementsTitle() As String
            Get
                Return SelectableElementsTitleAttribute
            End Get
            Set(ByVal value As String)
                SelectableElementsTitleAttribute = value
                SelectableElementsTitleLabel.Text = SelectableElementsTitleAttribute
            End Set
        End Property

        ''' <summary>
        ''' List of Selected Elements (to load the right list). Used also to return the final list of selected elements
        ''' </summary>
        Public Property SelectedElements() As DataSet
            Get
                Return SelectedElementsAttribute
            End Get
            Set(ByVal value As DataSet)
                If value Is Nothing Then
                    Call PrepareSelectedElements()
                Else
                    SelectedElementsAttribute = value
                    FillSelectedElementsListView()
                End If
            End Set
        End Property

        ''' <summary>
        ''' Title that must be shown for the list containing the selected elements (right list).
        ''' </summary>
        Public Property SelectedElementsTitle() As String
            Get
                Return SelectedElementsTitleAttribute
            End Get
            Set(ByVal value As String)
                SelectedElementsTitleAttribute = value
                SelectedElementsTitleLabel.Text = SelectedElementsTitleAttribute
            End Set
        End Property

        ''' <summary>
        ''' Allow activate / deactivate the sorting of elements in both lists.
        ''' </summary>
        Public Property Sorted() As Boolean
            Get
                Return SortedAttribute
            End Get
            Set(ByVal value As Boolean)
                SortedAttribute = value
                If (SortedAttribute) Then
                    'Call the ListView ColumnClick event for sorting the data
                    ElementsListView_ColumnClick(SelectableElements, leftListViewEvent)
                    ElementsListView_ColumnClick(SelectedElementsListView, rightListViewEvent)
                End If
            End Set
        End Property

        ''' <summary>
        ''' To load all Icons that can be shown in both ListViews
        ''' </summary>
        Public WriteOnly Property ListViewsIcons() As ImageList
            Set(ByVal value As ImageList)
                ListViewsIconsAttribute = value
            End Set
        End Property

        ''' <summary>
        ''' Allow activate / deactivate the functionality of multiple selection in both lists
        ''' </summary>
        Public Property MultiSelection() As Boolean
            Get
                Return MultiSelectionAttribute
            End Get
            Set(ByVal value As Boolean)
                MultiSelectionAttribute = value
                SelectableElementsListView.MultiSelect = MultiSelectionAttribute
                SelectedElementsListView.MultiSelect = MultiSelectionAttribute
            End Set
        End Property

        ''' <summary>
        ''' Multilanguage text to be shown as Tooltip of button Select All Elements
        ''' </summary>
        Public Property SelectAllToolTip() As String
            Get
                Return SelectAllToolTipAttribute
            End Get
            Set(ByVal value As String)
                SelectAllToolTipAttribute = value
                If (SelectAllToolTipAttribute <> "") Then
                    BSDoubleClickToolTips.SetToolTip(SelectALLSelectableElementsButton, SelectAllToolTipAttribute)
                End If
            End Set
        End Property

        ''' <summary>
        ''' Multilanguage text to be shown as Tooltip of  button Select Some Elements
        ''' </summary>
        Public Property SelectSomeToolTip() As String
            Get
                Return SelectSomeToolTipAttribute
            End Get
            Set(ByVal value As String)
                SelectSomeToolTipAttribute = value
                If SelectSomeToolTipAttribute <> "" Then
                    BSDoubleClickToolTips.SetToolTip(SelectChosenButton, SelectSomeToolTipAttribute)
                End If
            End Set
        End Property

        ''' <summary>
        ''' Multilanguage text to be shown as Tooltip of button Unselect Some Elements
        ''' </summary>
        Public Property UnselectSomeToolTip() As String
            Get
                Return UnselectSomeToolTipAttribute
            End Get
            Set(ByVal value As String)
                UnselectSomeToolTipAttribute = value
                If UnselectSomeToolTipAttribute <> "" Then
                    BSDoubleClickToolTips.SetToolTip(UnselectChosenSelectedElementButton, UnselectSomeToolTipAttribute)
                End If
            End Set
        End Property

        ''' <summary>
        ''' Multilanguage text to be shown as Tooltip of the button Unselect All Elements.
        ''' </summary>
        Public Property UnselectAllToolTip() As String
            Get
                Return UnselectAllToolTipAttribute
            End Get
            Set(ByVal value As String)
                UnselectAllToolTipAttribute = value
                If UnselectAllToolTipAttribute <> "" Then
                    BSDoubleClickToolTips.SetToolTip(UnselectAllSelectedElementsButton, UnselectAllToolTipAttribute)
                End If
            End Set
        End Property

        ''' <summary>
        ''' Initialize the control, cleaning all the used controls and objects
        ''' </summary>
        Public WriteOnly Property InitializeControl() As Boolean
            Set(ByVal value As Boolean)
                If (value) Then
                    PrepareSelectableElements()
                    PrepareSelectedElements()
                    SelectableElementsListView.Items.Clear()
                    SelectedElementsListView.Items.Clear()
                End If
            End Set
        End Property
#End Region

#Region "Subs and Functions"
        ''' <summary>
        ''' Initilize thes SelectableElements DataSet
        ''' </summary>      
        Private Sub PrepareSelectableElements()
            Try
                'Create the DataTable with the needed fields
                Dim SelectableElementsTable As New DataTable("SelectableElementsTable")
                SelectableElementsTable.Columns.Add("MainPos", System.Type.GetType("System.Int32"))
                SelectableElementsTable.Columns.Add("Type", System.Type.GetType("System.String"))
                SelectableElementsTable.Columns.Add("Code", System.Type.GetType("System.Int32"))
                SelectableElementsTable.Columns.Add("Description", System.Type.GetType("System.String"))
                SelectableElementsTable.Columns.Add("SecondaryPos", System.Type.GetType("System.Int32"))
                SelectableElementsTable.Columns.Add("Icon", System.Type.GetType("System.String"))
                'TR 08/03/2011 -Add Fartory calib column
                SelectableElementsTable.Columns.Add("FactoryCalib", System.Type.GetType("System.Boolean"))

                'Initialize the Dataset and add to it the created table
                SelectableElementsAttribute = New DataSet
                SelectableElementsAttribute.Tables.Add(SelectableElementsTable)
            Catch ex As Exception
                Throw ex
            End Try
        End Sub

        ''' <summary>
        ''' Initilize thes SelectedElements DataSet
        ''' </summary>
        Private Sub PrepareSelectedElements()
            Try
                'Create the DataTable with the needed fields
                Dim SelectedElementsTable As New DataTable("SelectedElementsTable")
                SelectedElementsTable.Columns.Add("MainPos", System.Type.GetType("System.Int32"))
                SelectedElementsTable.Columns.Add("Type", System.Type.GetType("System.String"))
                SelectedElementsTable.Columns.Add("Code", System.Type.GetType("System.Int32"))
                SelectedElementsTable.Columns.Add("Description", System.Type.GetType("System.String"))
                SelectedElementsTable.Columns.Add("SecondaryPos", System.Type.GetType("System.Int32"))
                SelectedElementsTable.Columns.Add("Icon", System.Type.GetType("System.String"))
                'TR 09/03/2011 -Set the Factory Calib Row
                SelectedElementsTable.Columns.Add("FactoryCalib", System.Type.GetType("System.Boolean"))

                'Initialize the Dataset and add to it the created table
                SelectedElementsAttribute = New DataSet
                SelectedElementsAttribute.Tables.Add(SelectedElementsTable)
            Catch ex As Exception
                Throw ex
            End Try
        End Sub

        ''' <summary>
        ''' Fill the SelectableElementsListView with data in SelectableElementsAttribute,
        ''' setting as SmallImageList the ListViewsIconsAttribute
        ''' </summary>
        Private Sub FillSelectableElementsListView()
            Try
                If (SelectableElementsListView.Items.Count > 0) Then
                    'Clear the ListView of Selectable Elements
                    SelectableElementsListView.Items.Clear()
                End If

                If (Not SelectableElementsAttribute Is Nothing) Then
                    If (SelectableElementsAttribute.Tables(0).Rows.Count > 0) Then
                        'Set the ImageList for both ListViews
                        SelectableElementsListView.SmallImageList = ListViewsIconsAttribute
                        SelectedElementsListView.SmallImageList = ListViewsIconsAttribute

                        'Filter by the selected Type and fill the ListView of Selectable Elements (left) 
                        SelectableElementsAttribute.Tables(0).DefaultView.RowFilter = "Type='" & TypeAttribute & "'"
                        For Each selectableElemRow As DataRowView In SelectableElementsAttribute.Tables(0).DefaultView
                            'AG 04/11/2010 - If item exits on the selected list do not add it
                            SelectedElementsAttribute.Tables(0).DefaultView.RowFilter = "Code='" & selectableElemRow("Code").ToString & "' AND " & _
                                                                                        "Type='" & selectableElemRow("Type").ToString & "'"
                            If (SelectedElementsAttribute.Tables(0).DefaultView.Count = 0) Then
                                With SelectableElementsListView.Items.Add(selectableElemRow("Description").ToString(), selectableElemRow("Icon").ToString)
                                    .SubItems.Add(selectableElemRow("Code").ToString())
                                    .SubItems.Add(selectableElemRow("Type").ToString())
                                    .SubItems.Add(selectableElemRow("MainPos").ToString())
                                    .SubItems.Add(selectableElemRow("SecondaryPos").ToString())
                                    'TR 09/03/2011 -Add Factory Calib 
                                    .SubItems.Add(selectableElemRow("FactoryCalib").ToString())

                                End With
                            End If
                        Next
                        SelectableElementsAttribute.Tables(0).DefaultView.Sort = "MainPos, SecondaryPos"
                    End If
                End If
            Catch ex As Exception
                Throw ex
            End Try
        End Sub

        ''' <summary>
        ''' Fill the SelectedElementsListView with data in SelectedElementsAttribute,
        ''' setting as SmallImageList the ListViewsIconsAttribute
        ''' </summary>
        Private Sub FillSelectedElementsListView()
            Try
                If (SelectedElementsListView.Items.Count > 0) Then
                    'Clear the ListView of Selected Elements
                    SelectedElementsListView.Items.Clear()
                End If

                If (Not SelectedElementsAttribute Is Nothing) Then
                    If (SelectedElementsAttribute.Tables(0).Rows.Count > 0) Then
                        'Set the ImageList for both ListViews
                        SelectableElementsListView.SmallImageList = ListViewsIconsAttribute
                        SelectedElementsListView.SmallImageList = ListViewsIconsAttribute

                        'Fill the ListView of Selected Elements (right)
                        SelectedElementsAttribute.Tables(0).DefaultView.RowFilter = "" 'AG 04/11/2010
                        For Each selectedElemRow As DataRowView In SelectedElementsAttribute.Tables(0).DefaultView
                            With SelectedElementsListView.Items.Add(selectedElemRow("Description").ToString(), selectedElemRow("Icon").ToString)
                                .SubItems.Add(selectedElemRow("Code").ToString())
                                .SubItems.Add(selectedElemRow("Type").ToString())
                                .SubItems.Add(selectedElemRow("MainPos").ToString())
                                .SubItems.Add(selectedElemRow("SecondaryPos").ToString())
                                'TR 09/03/2011 -Set the Factory Calib. 
                                .SubItems.Add(selectedElemRow("FactoryCalib").ToString())
                            End With
                        Next
                        SelectedElementsAttribute.Tables(0).DefaultView.Sort = "MainPos, SecondaryPos"
                    End If
                End If
            Catch ex As Exception
                Throw ex
            End Try
        End Sub

        ''' <summary>
        ''' Move a Selectable Element to the DataSet of Selected Elements
        ''' </summary>
        Private Sub FillSelectedElementsDataSet(ByVal pCode As String, ByVal pDescription As String, ByVal pIcon As String, _
                                                ByVal pType As String, ByVal pMainPos As Integer, ByVal pSecondaryPos As Integer, _
                                                                                                    ByVal pFactoryCalib As String)
            Try
                If (Not SelectedElementsAttribute Is Nothing) Then
                    If (SelectedElementsAttribute.Tables.Count > 0) Then
                        'Create a new DataRow, assign the informed values to it and add the row to SelectedElements DataSet
                        'AG 04/11/2010 - Add only if not exits (RowFilter + If + End If)
                        SelectedElementsAttribute.Tables(0).DefaultView.RowFilter = "Code='" & pCode & "' AND " & _
                                                                                    "Type='" & pType & "'"
                        If (SelectedElementsAttribute.Tables(0).DefaultView.Count = 0) Then
                            Dim mySelectedDR As DataRow = SelectedElementsAttribute.Tables(0).NewRow
                            mySelectedDR("MainPos") = pMainPos
                            mySelectedDR("Type") = pType
                            mySelectedDR("Code") = pCode
                            mySelectedDR("Description") = pDescription
                            mySelectedDR("SecondaryPos") = pSecondaryPos
                            mySelectedDR("Icon") = pIcon
                            'TR 09/03/2011-Set value to Factory Calib
                            mySelectedDR("FactoryCalib") = pFactoryCalib

                            SelectedElementsAttribute.Tables(0).Rows.Add(mySelectedDR)
                        End If

                        'Remove the moved Element from DataSet of Selectable Elements
                        SelectableElementsAttribute.Tables(0).DefaultView.RowFilter = "Code='" & pCode & "' AND " & _
                                                                                      "Type='" & pType & "'"
                        If (SelectableElementsAttribute.Tables(0).DefaultView.Count > 0) Then
                            'Delete the Element and clear the applied filter
                            SelectableElementsAttribute.Tables(0).DefaultView.Delete(0)
                            SelectableElementsAttribute.Tables(0).DefaultView.RowFilter = ""
                        End If
                    End If
                Else
                    'Prepare the Selected Elements DataSet and use recursion to move the Element
                    PrepareSelectedElements()
                    'TR 09/03/2011 -Set Factory Calib.
                    FillSelectedElementsDataSet(pCode, pDescription, pIcon, pType, pMainPos, pSecondaryPos, pFactoryCalib)
                End If
            Catch ex As Exception
                Throw ex
            End Try
        End Sub

        ''' <summary>
        ''' Move a Selected Element to the DataSet of Selectable Elements, 
        ''' </summary>
        Private Sub FillSelectableElementsDataSet(ByVal pCode As String, ByVal pDescription As String, ByVal pIcon As String, _
                                                  ByVal pType As String, ByVal pMainPos As Integer, ByVal pSecondaryPos As Integer, _
                                                                                                        ByVal pFactoryCalib As String)
            Try
                If (Not SelectableElementsAttribute Is Nothing) Then
                    If (SelectableElementsAttribute.Tables.Count > 0) Then
                        'Create a new DataRow, assign the informed values to it and add the row to SelectableElements DataSet
                        'AG 04/11/2010 - Add only if not exits (RowFilter + If + End If)
                        SelectableElementsAttribute.Tables(0).DefaultView.RowFilter = "Code='" & pCode & "' AND " & _
                                                                                      "Type='" & pType & "'"
                        If (SelectableElementsAttribute.Tables(0).DefaultView.Count = 0) Then
                            Dim mySelectedDR As DataRow = SelectableElementsAttribute.Tables(0).NewRow
                            mySelectedDR("MainPos") = pMainPos
                            mySelectedDR("Type") = pType
                            mySelectedDR("Code") = pCode
                            mySelectedDR("Description") = pDescription
                            mySelectedDR("SecondaryPos") = pSecondaryPos
                            mySelectedDR("Icon") = pIcon
                            'TR 09/03/2011-Set value to Factory Calib
                            mySelectedDR("FactoryCalib") = pFactoryCalib
                            SelectableElementsAttribute.Tables(0).Rows.Add(mySelectedDR)
                        End If

                        'Remove the moved Element from DataSet of Selected Elements
                        'SelectedElementsAttribute.Tables(0).DefaultView.RowFilter = "Code='" & pCode & "'" 'AG 04/11/2010
                        SelectedElementsAttribute.Tables(0).DefaultView.RowFilter = "Code='" & pCode & "' AND " & _
                                                                                    "Type='" & pType & "'"
                        If (SelectedElementsAttribute.Tables(0).DefaultView.Count > 0) Then
                            'Delete the Element and clear the applied filter
                            SelectedElementsAttribute.Tables(0).DefaultView.Delete(0)
                            SelectedElementsAttribute.Tables(0).DefaultView.RowFilter = ""
                        End If
                    End If
                Else
                    'Prepare the Selectable Elements DataSet and use recursion to move the Element
                    PrepareSelectableElements()
                    FillSelectableElementsDataSet(pCode, pDescription, pIcon, pType, pMainPos, pSecondaryPos, pFactoryCalib)
                End If
            Catch ex As Exception
                Throw ex
            End Try
        End Sub

        ''' <summary>
        ''' Move selected items from SelectableElementsListView to SelectedElementsListView
        ''' </summary>
        Private Sub MoveSelectionToRight()
            Try
                'TR 09/03/2011 -Create a varible to keep all test with Factory Calib.
                Dim myFactoryCalibTestList As String = ""

                If (SelectableElementsListView.SelectedItems.Count > 0) Then
                    For Each mySelectedItem As ListViewItem In SelectableElementsListView.SelectedItems
                        'Remove the Item from ListView of Selectable Elements 
                        SelectableElementsListView.Items.Remove(mySelectedItem)
                        'TR 09/03/2011 -
                        If CBool(mySelectedItem.SubItems(5).Text) Then
                            myFactoryCalibTestList &= mySelectedItem.Text & vbCrLf
                        End If

                        mySelectedItem.Selected = False
                        'TR 09/03/2011  -Set the FactoryCalib.
                        'Move the Element between the DataSets
                        FillSelectedElementsDataSet(mySelectedItem.SubItems(1).Text, mySelectedItem.Text, mySelectedItem.ImageKey.ToString(), _
                                                    mySelectedItem.SubItems(2).Text, Convert.ToInt32(mySelectedItem.SubItems(3).Text), _
                                                    Convert.ToInt32(mySelectedItem.SubItems(4).Text), mySelectedItem.SubItems(5).Text)

                    Next mySelectedItem

                    'TR 09/03/2011
                    If myFactoryCalibTestList <> "" Then
                        MessageBox.Show(FactoryValuesMessageAttribute & vbCrLf & myFactoryCalibTestList, "Warning")
                    End If
                    'TR 09/03/2011 -END

                    'Reload ListView of Selected Elements
                    FillSelectedElementsListView()
                End If
            Catch ex As Exception
                Throw ex
            End Try
        End Sub

        ''' <summary>
        ''' Move selected items from SelectedElementsListView to SelectableElementsListView
        ''' </summary>
        Private Sub MoveSelectionToLeft()
            Try
                'TR 09/03/2011 -Create a varible to keep all test with Factory Calib.
                Dim myFactoryCalibTestList As String = ""

                If (SelectedElementsListView.SelectedItems.Count > 0) Then
                    For Each mySelectedItem As ListViewItem In SelectedElementsListView.SelectedItems
                        'Remove the Item from ListView of Selected Elements 
                        SelectedElementsListView.Items.Remove(mySelectedItem)
                        mySelectedItem.Selected = False

                        'TR 09/03/2011 -
                        If CBool(mySelectedItem.SubItems(5).Text) Then
                            myFactoryCalibTestList &= mySelectedItem.Text & vbCrLf
                        End If

                        'Move the Element between the DataSets
                        FillSelectableElementsDataSet(mySelectedItem.SubItems(1).Text, mySelectedItem.Text, mySelectedItem.ImageKey.ToString, _
                                                      mySelectedItem.SubItems(2).Text, Convert.ToInt32(mySelectedItem.SubItems(3).Text), _
                                                      Convert.ToInt32(mySelectedItem.SubItems(4).Text), mySelectedItem.SubItems(5).Text)
                    Next

                    'TR 09/03/2011
                    If myFactoryCalibTestList <> "" Then
                        MessageBox.Show(FactoryValuesMessageAttribute & vbCrLf & myFactoryCalibTestList, "Warning")
                    End If
                    'TR 09/03/2011 -END

                    'Reload ListView of Selectable Elements
                    FillSelectableElementsListView()
                End If
            Catch ex As Exception
                Throw ex
            End Try
        End Sub

        ''' <summary>
        ''' Move all items from SelectableElementsListView to SelectedElementsListView
        ''' </summary>
        Private Sub MoveAllItemsRight()
            Try
                'TR 09/03/2011 -Create a varible to keep all test with Factory Calib.
                Dim myFactoryCalibTestList As String = ""

                For Each mySelectedItem As ListViewItem In SelectableElementsListView.Items
                    'Remove the Item from ListView of Selectable Elements 
                    SelectableElementsListView.Items.Remove(mySelectedItem)

                    'TR 09/03/2011 -
                    If CBool(mySelectedItem.SubItems(5).Text) Then
                        myFactoryCalibTestList &= mySelectedItem.Text & vbCrLf
                    End If


                    'Move the Element between the DataSets
                    FillSelectedElementsDataSet(mySelectedItem.SubItems(1).Text, mySelectedItem.Text, mySelectedItem.ImageKey.ToString(), _
                                                    mySelectedItem.SubItems(2).Text, Convert.ToInt32(mySelectedItem.SubItems(3).Text), _
                                                    Convert.ToInt32(mySelectedItem.SubItems(4).Text), mySelectedItem.SubItems(5).Text)
                Next

                'TR 09/03/2011 
                If myFactoryCalibTestList <> "" Then
                    MessageBox.Show(FactoryValuesMessageAttribute & vbCrLf & myFactoryCalibTestList, "Warning")
                End If
                'TR 09/03/2011 -END

                'Reload ListView of Selected Elements
                FillSelectedElementsListView()
            Catch ex As Exception
                Throw ex
            End Try
        End Sub

        ''' <summary>
        ''' Move all items from SelectedElementsListView to SelectableElementsListView
        ''' </summary>
        Private Sub MoveAllItemsLeft()
            Try
                'TR 09/03/2011 -Create a varible to keep all test with Factory Calib.
                Dim myFactoryCalibTestList As String = ""

                For Each mySelectedItem As ListViewItem In SelectedElementsListView.Items
                    'Remove the Item from ListView of Selected Elements 
                    SelectedElementsListView.Items.Remove(mySelectedItem)
                    'TR 09/03/2011 -
                    If CBool(mySelectedItem.SubItems(5).Text) Then
                        myFactoryCalibTestList &= mySelectedItem.Text & "." & vbCrLf
                    End If

                    'Move the Element between the DataSets
                    FillSelectableElementsDataSet(mySelectedItem.SubItems(1).Text, mySelectedItem.Text, mySelectedItem.ImageKey.ToString(), _
                                                  mySelectedItem.SubItems(2).Text, Convert.ToInt32(mySelectedItem.SubItems(3).Text), _
                                                  Convert.ToInt32(mySelectedItem.SubItems(4).Text), mySelectedItem.SubItems(5).Text)
                Next

                'TR 09/03/2011
                If myFactoryCalibTestList <> "" Then
                    MessageBox.Show(FactoryValuesMessageAttribute & vbCrLf & myFactoryCalibTestList, "Warning")
                End If
                'TR 09/03/2011 -END

                'Reload ListView of Selectable Elements
                FillSelectableElementsListView()
            Catch ex As Exception
                Throw ex
            End Try
        End Sub
#End Region

#Region "Events"
        Private Sub ElementsListView_ColumnClick(ByVal sender As System.Object, ByVal e As System.Windows.Forms.ColumnClickEventArgs) Handles SelectableElementsListView.ColumnClick, SelectedElementsListView.ColumnClick
            SelectableElementsListView.Sorting = SortOrder.Ascending
            SelectedElementsListView.Sorting = SortOrder.Ascending
        End Sub

        Private Sub SelectChosenButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SelectChosenButton.Click
            MoveSelectionToRight()
        End Sub

        Private Sub UnselectChosenSelectedElementButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles UnselectChosenSelectedElementButton.Click
            MoveSelectionToLeft()
        End Sub

        Private Sub SelectALLSelectableElementsButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SelectALLSelectableElementsButton.Click
            MoveAllItemsRight()
        End Sub

        Private Sub UnselectAllSelectedElementsButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles UnselectAllSelectedElementsButton.Click
            MoveAllItemsLeft()
        End Sub

        Private Sub SelectableElementsListView_DoubleClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles SelectableElementsListView.DoubleClick
            MoveSelectionToRight()
        End Sub

        Private Sub SelectedElementsListView_DoubleClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles SelectedElementsListView.DoubleClick
            MoveSelectionToLeft()
        End Sub

        ''' <summary>
        ''' User begings the dragging of selected items in the source list
        ''' </summary>
        Private Sub ListView_ItemDrag(ByVal sender As Object, ByVal e As System.Windows.Forms.ItemDragEventArgs) Handles SelectableElementsListView.ItemDrag, _
                                                                                                                         SelectedElementsListView.ItemDrag
            Try
                Dim myListView As New ListView
                myListView = CType(sender, ListView)

                'Loop though the SelectedItems collection for the source
                Dim i As Integer = 0
                Dim myItems(myListView.SelectedItems.Count - 1) As ListViewItem
                For Each myItem As ListViewItem In myListView.SelectedItems
                    'Add the ListViewItem to the array of ListViewItems.
                    myItems(i) = myItem
                    i = i + 1
                Next

                'Create a DataObject containg the array of ListViewItems
                myListView.DoDragDrop(New DataObject("System.Windows.Forms.ListViewItem()", myItems), DragDropEffects.Move)
            Catch ex As Exception
                Throw ex
            End Try
        End Sub

        ''' <summary>
        ''' El mouse with the list of selected elements enters in the destination list
        ''' </summary>
        Private Sub ListView_DragEnter(ByVal sender As Object, ByVal e As System.Windows.Forms.DragEventArgs) Handles SelectableElementsListView.DragEnter, _
                                                                                                                      SelectedElementsListView.DragEnter
            Try
                'Check for the custom DataFormat ListViewItem array.
                If (e.Data.GetDataPresent("System.Windows.Forms.ListViewItem()")) Then
                    e.Effect = DragDropEffects.Move
                Else
                    e.Effect = DragDropEffects.None
                End If
            Catch ex As Exception
                Throw ex
            End Try
        End Sub

        ''' <summary>
        ''' The selected elements are moved to the destination list
        ''' </summary>
        Private Sub ListView_DragDrop(ByVal sender As Object, ByVal e As System.Windows.Forms.DragEventArgs) Handles SelectableElementsListView.DragDrop, _
                                                                                                                     SelectedElementsListView.DragDrop
            Try
                Dim myListView As New ListView
                myListView = CType(sender, ListView)

                If (myListView Is SelectedElementsListView) Then
                    MoveSelectionToRight()
                Else
                    MoveSelectionToLeft()
                End If
            Catch ex As Exception
                Throw ex
            End Try
        End Sub

        Protected Overrides Sub OnPaint(ByVal e As System.Windows.Forms.PaintEventArgs)
            MyBase.OnPaint(e)
        End Sub
#End Region
    End Class
End Namespace