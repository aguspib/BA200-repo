Imports Biosystems.Ax00.Global.GlobalEnumerates
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.Global

Public Class ISortingTestsAux

#Region "Declaration"
    'Global variable for the current application Language
    Private CurrentLanguage As String = ""

    'Global variable for the list of Icons for the different Test Types
    Private TestIconList As New ImageList

    'Global variable used to center the screen
    Private NewScreenLocation As New Point

    Private ChangesMade As Boolean = False
#End Region

#Region "Constructor"
    'Public Sub New(ByRef myMDI As Form)
    Public Sub New()
        'RH 12/04/2012 myMDI not needed.
        'MyBase.New()
        'SetParentMDI(myMDI)
        InitializeComponent()
    End Sub
#End Region

#Region "Methods"
    ''' <summary>
    ''' Get the default sorting for all available Test Types:
    '''    ** Standard Tests sorted by Test Position
    '''    ** Calculated Tests sorted by Calculated Test Name
    '''    ** ISE Tests sorted by ISETestID
    '''    ** OffSystem Tests sorted by OFFSTestID
    ''' </summary>
    ''' <remarks>
    ''' Created by: TR
    ''' </remarks>
    Private Sub DefaultTestSort()
        Try
            Dim myGlobalDataTO As New GlobalDataTO
            Dim myReportsTestsSortingDelegate As New ReportsTestsSortingDelegate

            myGlobalDataTO = myReportsTestsSortingDelegate.GetDefaultSortedTestList(Nothing)
            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                Dim myReportsTestsSortingDS As ReportsTestsSortingDS = DirectCast(myGlobalDataTO.SetDatos, ReportsTestsSortingDS)

                'Get the default sorting for Standard Tests
                Dim myReportsTestsSortingList As List(Of ReportsTestsSortingDS.tcfgReportsTestsSortingRow)
                myReportsTestsSortingList = (From a In myReportsTestsSortingDS.tcfgReportsTestsSorting _
                                            Where a.TestType = "STD" _
                                         Order By a.TestPosition, a.PreloadedTest Descending _
                                           Select a).ToList()

                Dim myDefaultSortedTestDS As New ReportsTestsSortingDS
                For Each testRow As ReportsTestsSortingDS.tcfgReportsTestsSortingRow In myReportsTestsSortingList
                    myDefaultSortedTestDS.tcfgReportsTestsSorting.ImportRow(testRow)
                Next

                'Get the default sorting for Calculated Tests
                myReportsTestsSortingList = (From a In myReportsTestsSortingDS.tcfgReportsTestsSorting _
                                            Where a.TestType = "CALC" _
                                         Order By a.TestName, a.TestPosition _
                                           Select a).ToList()

                For Each testRow As ReportsTestsSortingDS.tcfgReportsTestsSortingRow In myReportsTestsSortingList
                    myDefaultSortedTestDS.tcfgReportsTestsSorting.ImportRow(testRow)
                Next

                'Get the default sorting for ISE Tests
                myReportsTestsSortingList = (From a In myReportsTestsSortingDS.tcfgReportsTestsSorting _
                                            Where a.TestType = "ISE" _
                                         Order By a.TestPosition _
                                           Select a).ToList()

                For Each testRow As ReportsTestsSortingDS.tcfgReportsTestsSortingRow In myReportsTestsSortingList
                    myDefaultSortedTestDS.tcfgReportsTestsSorting.ImportRow(testRow)
                Next

                'Get the default sorting for OffSystem Tests
                myReportsTestsSortingList = (From a In myReportsTestsSortingDS.tcfgReportsTestsSorting _
                                            Where a.TestType = "OFFS" _
                                         Order By a.TestPosition _
                                           Select a).ToList()

                For Each testRow As ReportsTestsSortingDS.tcfgReportsTestsSortingRow In myReportsTestsSortingList
                    myDefaultSortedTestDS.tcfgReportsTestsSorting.ImportRow(testRow)
                Next

                'Load the ListView with the final sorted list
                FillTestListView(myDefaultSortedTestDS)
                myReportsTestsSortingList = Nothing

                'TR 13/02/2012 -Set ChangesMade value True.
                ChangesMade = True
            Else
                'Error getting the default sorting for Tests
                ShowMessage(Name & ".DefaultTestSort ", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage)
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".DefaultTestSort ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".DefaultTestSort ", Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Get all Tests (all Types) sorted in the last saved mode
    ''' </summary>
    ''' <returns>Typed DataSet ReportsTestsSortingDS containing the list of Tests in the last saved test sorting</returns>
    ''' <remarks>
    ''' Created by:  TR
    ''' </remarks>
    Public Function GetSortedTestList() As ReportsTestsSortingDS
        Dim myReportsTestsSortingDS As New ReportsTestsSortingDS
        Try
            Dim myGlobalDataTO As GlobalDataTO
            Dim myReportsTestsSortingDelegate As New ReportsTestsSortingDelegate

            myGlobalDataTO = myReportsTestsSortingDelegate.GetSortedTestList(Nothing)
            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                myReportsTestsSortingDS = DirectCast(myGlobalDataTO.SetDatos, ReportsTestsSortingDS)
            Else
                'Error getting the current sorting of Tests for Reports
                ShowMessage(Name & ".GetSortedTestList ", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage)
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".GetSortedTestList ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".GetSortedTestList ", Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
        Return myReportsTestsSortingDS
    End Function

    ''' <summary>
    ''' Load an ImageList with all icons used for the different Test Types
    ''' </summary>
    ''' <remarks>
    ''' Created by:  TR
    ''' </remarks>
    Private Sub FillTestIconList()
        Try
            Dim myGlobalDataTO As New GlobalDataTO
            Dim myPreloadedMasterDataDelegate As New PreloadedMasterDataDelegate

            myGlobalDataTO = myPreloadedMasterDataDelegate.GetList(Nothing, PreloadedMasterDataEnum.ICON_PATHS)
            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                Dim myPreMasterDataDS As PreloadedMasterDataDS = CType(myGlobalDataTO.SetDatos, PreloadedMasterDataDS)

                For Each preMasterRow As PreloadedMasterDataDS.tfmwPreloadedMasterDataRow In myPreMasterDataDS.tfmwPreloadedMasterData.Rows
                    Select Case preMasterRow.ItemID
                        Case "TESTICON", "USERTEST", "TCALC", "TISE_SYS", "TOFF_SYS"
                            If (IO.File.Exists(MyBase.IconsPath & preMasterRow.FixedItemDesc)) Then
                                TestIconList.Images.Add(preMasterRow.ItemID, Image.FromFile(MyBase.IconsPath & preMasterRow.FixedItemDesc))
                            End If
                            Exit Select
                        Case Else
                            Exit Select
                    End Select
                Next
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".FillTestIconList ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".FillTestIconList ", Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Load in the ListView, the list of sorted Tests received 
    ''' </summary>
    ''' <param name="pReportsTestsSortingDS">Typed Dataset ReportsTestsSortingDS containing the current test sorting</param>
    ''' <remarks>
    ''' Created by:  TR
    ''' </remarks>
    Private Sub FillTestListView(ByVal pReportsTestsSortingDS As ReportsTestsSortingDS)
        Try
            Dim IconName As String = String.Empty
            Dim myRowIndex As Integer = 0

            TestListView.Items.Clear()
            For Each testRow As ReportsTestsSortingDS.tcfgReportsTestsSortingRow In pReportsTestsSortingDS.tcfgReportsTestsSorting.Rows
                Select Case testRow.TestType
                    Case "STD"
                        If testRow.PreloadedTest Then
                            IconName = "TESTICON"
                        Else
                            IconName = "USERTEST"
                        End If
                        Exit Select
                    Case "CALC"
                        IconName = "TCALC"
                        Exit Select
                    Case "ISE"
                        IconName = "TISE_SYS"
                        Exit Select
                    Case "OFFS"
                        IconName = "TOFF_SYS"
                        Exit Select
                End Select

                TestListView.Items.Add(testRow.TestName, IconName).SubItems.Add(testRow.TestID)
                TestListView.Items(myRowIndex).SubItems.Add(testRow.TestType)

                myRowIndex += 1
            Next
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".FillTestListView ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".FillTestListView ", Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Get text of all labels and screen tooltips in the current language
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub GetScreenLabelsAndToolTips()
        Try
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            'For Labels.....
            TestSortingLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Test_Sorting_Reports", CurrentLanguage)

            'For Button Tooltips...
            ScreenToolTips.SetToolTip(UpPosButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Move_Up", CurrentLanguage))
            ScreenToolTips.SetToolTip(FirstPosButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Move_First", CurrentLanguage))
            ScreenToolTips.SetToolTip(LastPosButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Move_Last", CurrentLanguage))
            ScreenToolTips.SetToolTip(DownPosButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Move_Down", CurrentLanguage))

            ScreenToolTips.SetToolTip(DefaultSortingButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Default_Sorting", CurrentLanguage))
            ScreenToolTips.SetToolTip(bsAcceptButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Save&Close", CurrentLanguage))
            ScreenToolTips.SetToolTip(CloseButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Cancel&Close", CurrentLanguage))
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".GetScreenLabels ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".GetScreenLabels ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Prepare all screen controls and fill data when the screen is loaded
    ''' </summary>
    ''' <remarks>
    ''' Created by:  TR
    ''' Modified by: SA 04/01/2012 - Added code needed to center the screen 
    ''' </remarks>
    Private Sub InitializeScreen()
        Try
            'Center the screen
            Dim mySize As Size = Me.Parent.Size
            Dim myLocation As Point = Me.Parent.Location

            NewScreenLocation = New Point(myLocation.X + CInt((mySize.Width - Me.Width) / 2), myLocation.Y + CInt((mySize.Height - Me.Height) / 2) - 60)
            Me.Location = NewScreenLocation

            'Get the current application Language
            Dim currentLanguageGlobal As New GlobalBase
            CurrentLanguage = currentLanguageGlobal.GetSessionInfo().ApplicationLanguage.Trim.ToString

            'Configure all screen controls (icons, texts in current language, ListView properties)
            PrepareButtons()
            PrepareTestListView()
            GetScreenLabelsAndToolTips()
            FillTestIconList()

            'Get the list of Tests with the current sorting and fill the ListView
            Dim myReportsTestsSortingDS As ReportsTestsSortingDS = GetSortedTestList()
            FillTestListView(myReportsTestsSortingDS)
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".InitializeScreen ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".InitializeScreen ", Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Move the group of selected Tests Up or Down in the ListView
    ''' </summary>
    ''' <param name="pListView">ListView containing all Tests</param>
    ''' <param name="pMoveUp">Flag indicating if Test are moved Up (True) or Down (False)</param>
    ''' <remarks>
    ''' Created by: TR
    ''' </remarks>
    Private Sub MoveItemsInListView(ByRef pListView As ListView, ByVal pMoveUp As Boolean)
        Try
            'If tests are moving down, set limittedIndex to the last element in list
            'If tests are moving up, set limittedIndex to the first element in list
            Dim limittedIndex As Integer = (pListView.Items.Count - 1)
            If (pMoveUp) Then limittedIndex = 0

            'Define a new collection for the indexes of ListView elements to move and field it with the indexes 
            'of all selected elements
            Dim IndexesToMove As New List(Of Integer)()
            For Each SelectedItem As ListViewItem In pListView.SelectedItems
                'Add the item's index to the collection
                IndexesToMove.Add(SelectedItem.Index)

                'If this item is at the limit we defined
                If (SelectedItem.Index = limittedIndex) Then
                    'Do not attempt to move item(s) as we are at the top or bottom of the list
                    Exit Try
                End If
            Next

            'If tests are moving down
            If (Not pMoveUp) Then
                'Reverse the index list so that we move items from the bottom of the selection first
                IndexesToMove.Reverse()
            End If

            'Loop through each index we want to move
            For Each intIndex As Integer In IndexesToMove
                'Define a new ListViewItem
                Dim lviNewItem As ListViewItem = CType(pListView.Items(intIndex).Clone(), ListViewItem)

                'Remove the currently selected item from the list
                pListView.Items(intIndex).Remove()

                'Insert the new item in the new position
                If (pMoveUp) Then
                    pListView.Items.Insert(intIndex - 1, lviNewItem)
                Else
                    pListView.Items.Insert(intIndex + 1, lviNewItem)
                End If

                lviNewItem.Selected = True
                lviNewItem.EnsureVisible()
            Next
            'TR 13/02/2012 -Set ChangesMade value True 
            ChangesMade = True

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".MoveItemsInListView ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".MoveItemsInListView ", Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        Finally
            'Set the focus on the listview
            pListView.Focus()
        End Try
    End Sub

    ''' <summary>
    ''' Move the group of selected Tests to the Top or the Bottom of the ListView
    ''' </summary>
    ''' <param name="ptListView">ListView containing all Tests</param>
    ''' <param name="pMoveTop">Flag indicating if Test are moved to Top(True) or to the Bottom (False)</param>
    ''' <remarks>
    ''' Created by: TR
    ''' </remarks>
    Private Sub MoveTopOrBottomItems(ByVal ptListView As ListView, ByVal pMoveTop As Boolean)
        Try
            'If tests are moving to botton, set index to the last element in list
            'If tests are moving to top, set index to the first element in list
            Dim myIndex As Integer = 0
            If (Not pMoveTop) Then myIndex = ptListView.Items.Count - 1

            For Each selectedTestItem As ListViewItem In ptListView.SelectedItems
                'Define a new ListViewItem
                Dim lviNewItem As ListViewItem = CType(selectedTestItem.Clone(), ListViewItem)

                'Remove the currently selected item from the list
                selectedTestItem.Remove()
                If (pMoveTop) Then
                    'Insert the new item in it's new place
                    TestListView.Items.Insert(myIndex, lviNewItem)
                    myIndex += 1
                Else
                    TestListView.Items.Insert(myIndex, lviNewItem)
                    myIndex -= 1
                End If

                lviNewItem.Selected = True
                lviNewItem.EnsureVisible()
            Next

            'TR 13/02/2012 -Set ChangesMade value True 
            ChangesMade = True

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".MoveTopOrBottomItems ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".MoveTopOrBottomItems ", Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub


    ''' <summary>
    ''' Get the Icon for all graphic buttons in the screen
    ''' </summary>
    ''' <remarks>
    ''' Created by:  TR
    ''' </remarks>
    Private Sub PrepareButtons()
        Try
            Dim auxIconName As String = ""
            Dim iconPath As String = IconsPath

            'MOVE UP SELECTED TESTS Button
            auxIconName = GetIconName("UPARROW")
            If (auxIconName <> "") Then UpPosButton.Image = Image.FromFile(iconPath & auxIconName)

            'MOVE DOWN SELECTED TESTS Button
            auxIconName = GetIconName("DOWNARROW")
            If (auxIconName <> "") Then DownPosButton.Image = Image.FromFile(iconPath & auxIconName)

            'MOVE SELECTED TESTS TO FIRST POSITION Button
            auxIconName = GetIconName("TOPARROW")
            If (auxIconName <> "") Then FirstPosButton.Image = Image.FromFile(iconPath & auxIconName)

            'MOVE SELECTED TESTS TO LAST POSITION Button
            auxIconName = GetIconName("BOTTOMARROW")
            If (auxIconName <> "") Then LastPosButton.Image = Image.FromFile(iconPath & auxIconName)

            'DEFAULT SORTING RECOVERY Button
            'auxIconName = GetIconName("UPDOWNROW") 'DL 21/02/2012
            auxIconName = GetIconName("UNDO")       'DL 21/02/2012
            If (auxIconName <> "") Then DefaultSortingButton.Image = Image.FromFile(iconPath & auxIconName)

            'SAVE Button
            auxIconName = GetIconName("ACCEPT1")
            If (auxIconName <> "") Then bsAcceptButton.Image = Image.FromFile(iconPath & auxIconName)

            'CANCEL Button
            auxIconName = GetIconName("CANCEL")
            If (auxIconName <> "") Then CloseButton.Image = Image.FromFile(iconPath & auxIconName)
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".PrepareButtons ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".PrepareButtons ", Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Configure the ListView for the list of Tests
    ''' </summary>
    ''' <remarks>
    ''' Created by:  TR
    ''' </remarks>
    Private Sub PrepareTestListView()
        Try
            TestListView.Items.Clear()
            TestListView.Alignment = ListViewAlignment.Left
            TestListView.HeaderStyle = ColumnHeaderStyle.Clickable
            TestListView.MultiSelect = True
            TestListView.Scrollable = True
            TestListView.FullRowSelect = True
            TestListView.View = View.Details
            TestListView.HideSelection = False
            TestListView.SmallImageList = TestIconList

            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate
            TestListView.Columns.Add(myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_TestNames", CurrentLanguage), -2, HorizontalAlignment.Left)

            TestListView.Columns(0).Width = 215 'TestListView.Width
            TestListView.Columns(0).Name = "TestName"
            TestListView.Columns.Add("TestID", 0, HorizontalAlignment.Left)
            TestListView.Columns(1).Name = "TestID"
            TestListView.Columns.Add("TestType", 0, HorizontalAlignment.Left)
            TestListView.Columns(2).Name = "TestType"
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".PrepareTestListView ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".PrepareTestListView ", Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    Private Function ReorderTest() As ReportsTestsSortingDS
        Dim myReportSortingDS As New ReportsTestsSortingDS

        Try
            Dim myRepoSortRow As ReportsTestsSortingDS.tcfgReportsTestsSortingRow
            For Each TestItem As ListViewItem In TestListView.Items
                myRepoSortRow = myReportSortingDS.tcfgReportsTestsSorting.NewtcfgReportsTestsSortingRow
                myRepoSortRow.TestType = TestItem.SubItems(2).Text
                myRepoSortRow.TestID = CInt(TestItem.SubItems(1).Text)
                myRepoSortRow.TestPosition = TestItem.Index + 1

                myReportSortingDS.tcfgReportsTestsSorting.AddtcfgReportsTestsSortingRow(myRepoSortRow)
            Next
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".ReorderTest ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ReorderTest ", Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
        Return myReportSortingDS
    End Function

    Private Function SaveChanges(ByVal pReportsTestsSortingDS As ReportsTestsSortingDS) As Boolean
        Dim isSaveOK As Boolean = False
        Try
            Dim myGlobalDataTO As New GlobalDataTO
            Dim myReportsTestsSortingDelegate As New ReportsTestsSortingDelegate

            myGlobalDataTO = myReportsTestsSortingDelegate.UpdateTestPosition(Nothing, pReportsTestsSortingDS)
            If (Not myGlobalDataTO.HasError) Then
                isSaveOK = True
            Else
                ShowMessage(Name & ".SaveChanges ", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage)
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".SaveChanges ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".SaveChanges ", Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
        Return isSaveOK
    End Function

    ''' <summary>
    ''' Not allow moving the form and mantain the center location in center parent
    ''' </summary>
    Protected Overrides Sub WndProc(ByRef m As Message)
        Try
            If (m.Msg = WM_WINDOWPOSCHANGING) Then
                Dim pos As WINDOWPOS = DirectCast(Runtime.InteropServices.Marshal.PtrToStructure(m.LParam, GetType(WINDOWPOS)), WINDOWPOS)
                Dim myLocation As Point = Me.Parent.Location
                Dim mySize As Size = Me.Parent.Size

                pos.x = myLocation.X + CInt((mySize.Width - Me.Width) / 2)
                pos.y = myLocation.Y + CInt((mySize.Height - Me.Height) / 2) - 60
                Runtime.InteropServices.Marshal.StructureToPtr(pos, m.LParam, True)
            End If

            MyBase.WndProc(m)
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WndProc " & Me.Name, EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & "WndProc", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub
#End Region

#Region "Events"
    Private Sub ISortingTestsAux_Move(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Move
        Me.Location = NewScreenLocation
    End Sub

    Private Sub ISortingTestsAux_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles MyBase.KeyDown
        Try
            If (e.KeyCode = Keys.Escape) Then
                'Escape key should do exactly the same operations as bsExitButton_Click()
                CloseButton.PerformClick()
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".ISortingTestsAux_KeyDown ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ISortingTestsAux_KeyDown ", Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    Private Sub ISortingTestsAux_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try
            InitializeScreen()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".ISortingTestsAux_Load ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ISortingTestsAux_Load ", Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    Private Sub TestListView_ColumnWidthChanging(ByVal sender As System.Object, ByVal e As System.Windows.Forms.ColumnWidthChangingEventArgs) Handles TestListView.ColumnWidthChanging
        Try
            e.Cancel = True
            If e.ColumnIndex = 0 Then
                e.NewWidth = 215 '233 
            Else
                e.NewWidth = 0
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".TestListView_ColumnWidthChanging", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".TestListView_ColumnWidthChanging", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    Private Sub FirstPosButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles FirstPosButton.Click
        Try
            MoveTopOrBottomItems(TestListView, True)
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".FirstPosButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".FirstPosButton_Click ", Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    Private Sub UpPosButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles UpPosButton.Click
        Try
            MoveItemsInListView(TestListView, True)
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".UpPosButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".UpPosButton_Click ", Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    Private Sub DownPosButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DownPosButton.Click
        Try
            MoveItemsInListView(TestListView, False)
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".DownPosButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".DownPosButton_Click ", Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    Private Sub LastPosButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles LastPosButton.Click
        Try
            MoveTopOrBottomItems(TestListView, False)
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".LastPosButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".LastPosButton_Click ", Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    Private Sub DefaultSortingButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DefaultSortingButton.Click
        Try
            DefaultTestSort()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".DefaultSortingButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".DefaultSortingButton_Click ", Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    Private Sub CloseButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CloseButton.Click
        Try
            'TR 14/02/2012
            Dim CloseForm As Boolean = False
            If ChangesMade Then
                If (ShowMessage(Name, GlobalEnumerates.Messages.DISCARD_PENDING_CHANGES.ToString) = Windows.Forms.DialogResult.Yes) Then
                    ChangesMade = False
                    CloseForm = True
                End If
            Else
                CloseForm = True
            End If
            If CloseForm Then

                'TR 11/04/2012 -Disable form on close to avoid any button press.
                Me.Enabled = False

                If Not Me.Tag Is Nothing Then
                    'A PerformClick() method was executed
                    Me.Close()
                Else
                    'Normal button click
                    'Open the WS Monitor form and close this one
                    IAx00MainMDI.OpenMonitorForm(Me)
                End If
            End If
            'TR 14/02/2012 -END.
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".CloseButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".CloseButton_Click ", Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    Private Sub AcceptButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsAcceptButton.Click
        Try
            If (SaveChanges(ReorderTest())) Then
                ChangesMade = False
                CloseButton.PerformClick()
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".AcceptButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".AcceptButton_Click ", Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub
#End Region

End Class
