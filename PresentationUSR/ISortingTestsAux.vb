Imports Biosystems.Ax00.Global.GlobalEnumerates
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Controls.UserControls

Public Class ISortingTestsAux
    Inherits Biosystems.Ax00.PresentationCOM.BSBaseForm
    Implements IPermissionLevel

#Region "Declaration"
    'Global variable for the current application Language
    Private CurrentLanguage As String = ""

    'Global variable for the list of Icons for the different Test Types
    Private TestIconList As New ImageList

    'Global variable used to center the screen
    Private NewScreenLocation As New Point

    Private ChangesMade As Boolean = False

    'AG 04/09/2014 - BA-1869
    Private HeadImageSide As Integer = 12 'Set here the dimensions of the head images
    Private HeadRect As Rectangle = Nothing
    Private checkImage As Image = Nothing
    Private uncheckImage As Image = Nothing
    'AG 04/09/2014 - BA-1869

#End Region

#Region "Attributes"
    Private openModeAttribute As String = String.Empty
    Private ScreenIDAttribute As String = String.Empty
#End Region
    'AG 02/09/2014 - BA-1869

#Region "Constructor"
    'Public Sub New(ByRef myMDI As Form)
    Public Sub New()
        InitializeComponent()
    End Sub
#End Region

    'AG 02/09/2014 - BA-1869
#Region "Screen properties"
    Public Property openMode() As String
        Get
            Return openModeAttribute
        End Get
        Set(value As String)
            openModeAttribute = value
        End Set
    End Property

    Public Property screenID() As String
        Get
            Return ScreenIDAttribute
        End Get
        Set(value As String)
            ScreenIDAttribute = value
        End Set
    End Property
#End Region
    'AG 02/09/2014 - BA-1869

#Region "Methods"
    ''' <summary>
    ''' Get the default sorting for all available Test Types:
    '''    ** Standard Tests sorted by Test Position
    '''    ** Calculated Tests sorted by Calculated Test Name
    '''    ** ISE Tests sorted by ISETestID
    '''    ** OffSystem Tests sorted by OFFSTestID
    ''' 
    ''' In mode test selection order all tests become Available too
    ''' </summary>
    ''' <remarks>
    ''' Created by: TR
    ''' AG 03/09/2014 - BA-1869 default test selection order
    ''' </remarks>
    Private Sub DefaultTestSort()
        Try
            Dim myGlobalDataTO As New GlobalDataTO
            Dim myReportsTestsSortingDS As New ReportsTestsSortingDS
            Dim myReportsTestsSortingList As List(Of ReportsTestsSortingDS.tcfgReportsTestsSortingRow)
            Dim myDefaultSortedTestDS As New ReportsTestsSortingDS

            'AG 03/09/2014 - BA-1869
            If openModeAttribute = String.Empty Then
                Dim myReportsTestsSortingDelegate As New ReportsTestsSortingDelegate
                myGlobalDataTO = myReportsTestsSortingDelegate.GetDefaultSortedTestList(Nothing)
                If Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing Then
                    myReportsTestsSortingDS = DirectCast(myGlobalDataTO.SetDatos, ReportsTestsSortingDS)

                    '1st Get the default sorting for Standard Tests
                    myReportsTestsSortingList = (From a In myReportsTestsSortingDS.tcfgReportsTestsSorting _
                                                Where a.TestType = "STD" _
                                             Order By a.TestPosition, a.PreloadedTest Descending _
                                               Select a).ToList()
                    For Each testRow As ReportsTestsSortingDS.tcfgReportsTestsSortingRow In myReportsTestsSortingList
                        myDefaultSortedTestDS.tcfgReportsTestsSorting.ImportRow(testRow)
                    Next

                    '2on Get the default sorting for Calculated Tests
                    myReportsTestsSortingList = (From a In myReportsTestsSortingDS.tcfgReportsTestsSorting _
                                                Where a.TestType = "CALC" _
                                             Order By a.TestName, a.TestPosition _
                                               Select a).ToList()
                    For Each testRow As ReportsTestsSortingDS.tcfgReportsTestsSortingRow In myReportsTestsSortingList
                        myDefaultSortedTestDS.tcfgReportsTestsSorting.ImportRow(testRow)
                    Next

                    '3rd Get the default sorting for ISE Tests
                    myReportsTestsSortingList = (From a In myReportsTestsSortingDS.tcfgReportsTestsSorting _
                                                Where a.TestType = "ISE" _
                                             Order By a.TestPosition _
                                               Select a).ToList()
                    For Each testRow As ReportsTestsSortingDS.tcfgReportsTestsSortingRow In myReportsTestsSortingList
                        myDefaultSortedTestDS.tcfgReportsTestsSorting.ImportRow(testRow)
                    Next

                    '4th Get the default sorting for OffSystem Tests
                    myReportsTestsSortingList = (From a In myReportsTestsSortingDS.tcfgReportsTestsSorting _
                                                Where a.TestType = "OFFS" _
                                             Order By a.TestPosition _
                                               Select a).ToList()
                    For Each testRow As ReportsTestsSortingDS.tcfgReportsTestsSortingRow In myReportsTestsSortingList
                        myDefaultSortedTestDS.tcfgReportsTestsSorting.ImportRow(testRow)
                    Next
                End If

            Else
                myReportsTestsSortingDS = GetSortedTestList() 'Get all elements and apply the default order

                'Order for test selection default (same as screen)
                Select Case ScreenIDAttribute
                    Case "STD"
                        'Get the default sorting for Standard Tests
                        myReportsTestsSortingList = (From a In myReportsTestsSortingDS.tcfgReportsTestsSorting _
                                                 Order By a.PreloadedTest Descending, a.TestName _
                                                   Select a).ToList()
                    Case "CALC"
                        'Get the default sorting for Calculated Tests
                        myReportsTestsSortingList = (From a In myReportsTestsSortingDS.tcfgReportsTestsSorting _
                                                 Order By a.TestName Select a).ToList()
                    Case "ISE", "OFFS", "PROFILE"
                        'Get the default sorting for ISE, OFFS and profile Tests
                        myReportsTestsSortingList = (From a In myReportsTestsSortingDS.tcfgReportsTestsSorting _
                                                 Order By a.TestID Select a).ToList()
                End Select
                If Not myReportsTestsSortingList Is Nothing Then
                    For Each testRow As ReportsTestsSortingDS.tcfgReportsTestsSortingRow In myReportsTestsSortingList
                        If Not testRow.Available Then
                            testRow.BeginEdit()
                            testRow.Available = True
                            testRow.EndEdit()
                        End If
                        myDefaultSortedTestDS.tcfgReportsTestsSorting.ImportRow(testRow)
                    Next
                End If
            End If
            'AG 03/09/2014 - BA-1869

            'Load the ListView with the final sorted list
            'FillTestListView(myDefaultSortedTestDS)'AG 04/09/2014 - BA-1869
            FillTestDataGridView(myDefaultSortedTestDS) 'AG 03/09/2014 - BA-1869
            myReportsTestsSortingList = Nothing

            'TR 13/02/2012 -Set ChangesMade value True.
            ChangesMade = True

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
    ''' AG 02/09/2014 - BA-1869 get data for SORT test report or test selection (in this case get only the desidred the test type or profile)
    ''' </remarks>
    Private Function GetSortedTestList() As ReportsTestsSortingDS
        Dim myReportsTestsSortingDS As New ReportsTestsSortingDS
        Try
            Dim myGlobalDataTO As GlobalDataTO

            'AG 02/09/2014 - BA-1869
            'Dim myReportsTestsSortingDelegate As New ReportsTestsSortingDelegate
            'myGlobalDataTO = myReportsTestsSortingDelegate.GetSortedTestList(Nothing)
            If openModeAttribute = String.Empty Then
                'Test order for reports
                Dim myReportsTestsSortingDelegate As New ReportsTestsSortingDelegate
                myGlobalDataTO = myReportsTestsSortingDelegate.GetSortedTestList(Nothing)
            Else
                'Order for test selection
                Select Case ScreenIDAttribute
                    Case "STD"
                        Dim myDelegate As New TestsDelegate
                        myGlobalDataTO = myDelegate.GetCustomizedSortedTestSelectionList(Nothing)
                    Case "CALC"
                        Dim myDelegate As New CalculatedTestsDelegate
                        myGlobalDataTO = myDelegate.GetCustomizedSortedTestSelectionList(Nothing)

                    Case "ISE"
                        Dim myDelegate As New ISETestsDelegate
                        myGlobalDataTO = myDelegate.GetCustomizedSortedTestSelectionList(Nothing)

                    Case "OFFS"
                        Dim myDelegate As New OffSystemTestsDelegate
                        myGlobalDataTO = myDelegate.GetCustomizedSortedTestSelectionList(Nothing)

                    Case "PROFILE"
                        Dim myDelegate As New TestProfilesDelegate
                        myGlobalDataTO = myDelegate.GetCustomizedSortedTestSelectionList(Nothing)

                    Case Else
                        'List will appear empty
                End Select
            End If
            'AG 02/09/2014 - BA-1869

            If Not myGlobalDataTO Is Nothing Then
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    myReportsTestsSortingDS = DirectCast(myGlobalDataTO.SetDatos, ReportsTestsSortingDS)
                Else
                    'Error getting the current sorting of Tests for Reports
                    ShowMessage(Name & ".GetSortedTestList ", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage)
                End If

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
    ''' AG 03/09/2014 - BA-1869 add the tests profiles icon
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
                        Case "TESTICON", "USERTEST", "TCALC", "TISE_SYS", "TOFF_SYS", "TPROFILES"
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
    ''' Get text of all labels and screen tooltips in the current language
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub GetScreenLabelsAndToolTips()
        Try
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            'For Labels.....
            'AG 08/09/2014 - BA-1869
            'TestSortingLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Test_Sorting_Reports", CurrentLanguage)
            If openModeAttribute = String.Empty Then
                TestSortingLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Test_Sorting_Reports", CurrentLanguage)
            Else
                TestSortingLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_TEST_SORTING_SELECTION", CurrentLanguage)
            End If

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
    ''' AG 02/09/2014 - BA-1869 (when open in TEST SELECTION mode has no parent, use the mdi) for Size and Location
    ''' </remarks>
    Private Sub InitializeScreen()
        Try

            'AG 02/09/2014 - BA-1869: Center the screen
            'Dim mySize As Size = Me.Parent.Size
            'Dim myLocation As Point = Me.Parent.Location
            Dim mySize As Size
            Dim myLocation As Point

            If openModeAttribute = String.Empty Then 'Test sort for REPORTS
                mySize = Me.Parent.Size
                myLocation = Me.Parent.Location
            Else 'Test selection sort (popup,  use the MDI)
                mySize = IAx00MainMDI.Size
                myLocation = IAx00MainMDI.Location
            End If
            'AG 02/09/2014 - BA-1869

            NewScreenLocation = New Point(myLocation.X + CInt((mySize.Width - Me.Width) / 2), myLocation.Y + CInt((mySize.Height - Me.Height) / 2) - 60)
            Me.Location = NewScreenLocation

            'Get the current application Language
            Dim currentLanguageGlobal As New GlobalBase
            CurrentLanguage = currentLanguageGlobal.GetSessionInfo().ApplicationLanguage.Trim.ToString

            'Configure all screen controls (icons, texts in current language, ListView properties)
            PrepareButtons()

            'PrepareTestListView()'AG 04/09/2014 - BA-1869
            PrepareTestListGrid() 'AG 03/09/2014 - BA-1869

            GetScreenLabelsAndToolTips()
            FillTestIconList()

            'Get the list of Tests with the current sorting and fill the ListView
            Dim myReportsTestsSortingDS As ReportsTestsSortingDS = GetSortedTestList()

            'FillTestListView(myReportsTestsSortingDS)'AG 04/09/2014 - BA-1869
            FillTestDataGridView(myReportsTestsSortingDS) 'AG 03/09/2014 - BA-1869

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".InitializeScreen ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".InitializeScreen ", Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub


    ''' <summary>
    ''' Get the Icon for all graphic buttons in the screen
    ''' </summary>
    ''' <remarks>
    ''' Created by:  TR
    ''' AG 04/09/2014 - BA-1869 new icon check / unchecked for grid header
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

            'AG 04/09/2014 - BA-1869
            'CHECK icon in grid header
            auxIconName = GetIconName("CHECKL")
            If (auxIconName <> "") Then
                checkImage = Image.FromFile(iconPath & auxIconName)
            End If

            'UNCHECK icon in grid header
            auxIconName = GetIconName("UNCHECKL")
            If (auxIconName <> "") Then
                uncheckImage = Image.FromFile(iconPath & auxIconName)
            End If
            'AG 04/09/2014 - BA-1869

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".PrepareButtons ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".PrepareButtons ", Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub


    ''' <summary>
    ''' Saves changes in new test orders
    ''' - Original functionality test order for reports with all test types
    ''' - Functionality test order and availability for test selection only 1 test type or profiles
    ''' </summary>
    ''' <param name="pReportsTestsSortingDS"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' Created ??
    ''' AG 03/09/2014 - BA-1869 customize test order for test selection
    ''' </remarks>
    Private Function SaveChanges(ByVal pReportsTestsSortingDS As ReportsTestsSortingDS) As Boolean
        Dim isSaveOK As Boolean = False
        Try
            Dim myGlobalDataTO As New GlobalDataTO
            Dim myReportsTestsSortingDelegate As New ReportsTestsSortingDelegate

            'AG 03/09/2014 - BA-1869
            'myGlobalDataTO = myReportsTestsSortingDelegate.UpdateTestPosition(Nothing, pReportsTestsSortingDS)
            If openModeAttribute = String.Empty Then
                'Test order for reports
                myGlobalDataTO = myReportsTestsSortingDelegate.UpdateTestPosition(Nothing, pReportsTestsSortingDS)
            Else
                'Test (or profile) order for test selection
                Select Case ScreenIDAttribute
                    Case "STD"
                        Dim myDelegate As New TestsDelegate
                        myGlobalDataTO = myDelegate.UpdateCustomPositionAndAvailable(Nothing, pReportsTestsSortingDS)
                    Case "CALC"
                        Dim myDelegate As New CalculatedTestsDelegate
                        myGlobalDataTO = myDelegate.UpdateCustomPositionAndAvailable(Nothing, pReportsTestsSortingDS)

                    Case "ISE"
                        Dim myDelegate As New ISETestsDelegate
                        myGlobalDataTO = myDelegate.UpdateCustomPositionAndAvailable(Nothing, pReportsTestsSortingDS)

                    Case "OFFS"
                        Dim myDelegate As New OffSystemTestsDelegate
                        myGlobalDataTO = myDelegate.UpdateCustomPositionAndAvailable(Nothing, pReportsTestsSortingDS)

                    Case "PROFILE"
                        Dim myDelegate As New TestProfilesDelegate
                        myGlobalDataTO = myDelegate.UpdateCustomPositionAndAvailable(Nothing, pReportsTestsSortingDS)

                    Case Else
                        'List will appear empty
                End Select
            End If
            'AG 03/09/2014 - BA-1869

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
    ''' AG 02/09/2014 - BA-1869 (when open in TEST SELECTION mode has no parent, use the mdi) for Size and Location
    ''' </summary>
    Protected Overrides Sub WndProc(ByRef m As Message)
        Try
            If (m.Msg = WM_WINDOWPOSCHANGING) Then
                Dim pos As WINDOWPOS = DirectCast(Runtime.InteropServices.Marshal.PtrToStructure(m.LParam, GetType(WINDOWPOS)), WINDOWPOS)

                'AG 02/09/2014 - ba-1869
                'Dim myLocation As Point = Me.Parent.Location
                'Dim mySize As Size = Me.Parent.Size

                Dim myLocation As Point
                Dim mySize As Size
                If openModeAttribute = String.Empty Then 'Test sort for REPORTS
                    mySize = Me.Parent.Size
                    myLocation = Me.Parent.Location
                Else 'Test selection sort (popup,  use the MDI)
                    mySize = IAx00MainMDI.Size
                    myLocation = IAx00MainMDI.Location
                End If
                'AG 02/09/2014 - BA-1869

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


    Private Sub FirstPosButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles FirstPosButton.Click
        Try
            'MoveTopOrBottomItems(TestListView, True)
            MoveTopOrBottomItemsInGrid(bsTestListGrid, True) 'AG 03/09/2014 - BA-1869
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".FirstPosButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".FirstPosButton_Click ", Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    Private Sub UpPosButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles UpPosButton.Click
        Try
            'MoveItemsInListView(TestListView, True)
            MoveItemsInListInGrid(bsTestListGrid, True) 'AG 03/09/2014 - BA-1869
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".UpPosButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".UpPosButton_Click ", Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    Private Sub DownPosButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DownPosButton.Click
        Try
            'MoveItemsInListView(TestListView, False)
            MoveItemsInListInGrid(bsTestListGrid, False) 'AG 03/09/2014 - BA-1869
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".DownPosButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".DownPosButton_Click ", Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    Private Sub LastPosButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles LastPosButton.Click
        Try
            'MoveTopOrBottomItems(TestListView, False)
            MoveTopOrBottomItemsInGrid(bsTestListGrid, False) 'AG 03/09/2014 - BA-1869
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

                    'AG 04/09/2014 - BA-1869 Mode: order for test report >> opens monitor // Mode: order for test selection >> closes himself
                    'IAx00MainMDI.OpenMonitorForm(Me)
                    If openModeAttribute = String.Empty Then
                        'Open the WS Monitor form and close this one
                        IAx00MainMDI.OpenMonitorForm(Me)
                    Else
                        Me.Close()
                    End If
                    'AG 04/09/2014 - BA-1869
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


    ''' <summary>
    ''' Click on grid
    ''' - Available column row -> New value = Not current value
    ''' - Available column header -> New value: some NOT selected -> SELECT ALL // all selected -> SELECT NONE
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>AG 04/09/2014 - BA-1869</remarks>
    Private Sub bsTestListGrid_CellMouseClick(sender As Object, e As DataGridViewCellMouseEventArgs) Handles bsTestListGrid.CellMouseClick
        Try
            If openModeAttribute <> String.Empty Then
                If e.RowIndex >= 0 Then
                    'Click on item
                    If bsTestListGrid.Columns(e.ColumnIndex).Name.ToString = "Available" Then
                        Dim dgv As BSDataGridView = bsTestListGrid
                        Dim myField As String = dgv.Columns(e.ColumnIndex).Name
                        dgv(myField, e.RowIndex).Value = Not CBool(dgv(e.ColumnIndex, e.RowIndex).Value) 'Assign new value = Not current

                    End If
                Else
                    'Click on header
                    If bsTestListGrid.Columns(e.ColumnIndex).Name.ToString = "Available" Then
                        Dim dgv As BSDataGridView = bsTestListGrid

                        'Some not available new value = avaliable ALL // all available new value = available NONE
                        Dim disabledRows As List(Of DataGridViewRow) = (From a As DataGridViewRow In dgv.Rows _
                                                    Where a.Cells("Available").Value = False Select a).ToList


                        If disabledRows.Count > 0 Then
                            'Some disable ... set all available
                            For Each row As DataGridViewRow In disabledRows
                                row.Cells("Available").Value = True
                            Next

                        Else
                            'All enable ... set disable
                            For Each row As DataGridViewRow In dgv.Rows
                                If row.Cells("Available").Value Then
                                    row.Cells("Available").Value = False
                                End If
                            Next
                        End If
                        disabledRows = Nothing

                    End If
                End If
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".bsTestListGrid_CellMouseClick ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsTestListGrid_CellMouseClick ", Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub


    ''' <summary>
    ''' Draws the icon header for column Available
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>Created AG 04/09/2014 - BA-1869</remarks>
    Private Sub bsTestListGrid_CellPainting(ByVal sender As Object, ByVal e As DataGridViewCellPaintingEventArgs) Handles bsTestListGrid.CellPainting

        If openModeAttribute <> String.Empty Then
            If e.RowIndex = -1 Then
                Dim AvailableIndex As Integer = bsTestListGrid.Columns("Available").Index


                If e.ColumnIndex = AvailableIndex Then
                    HeadRect = New Rectangle(e.CellBounds.Left + (e.CellBounds.Width - HeadImageSide) / 2, _
                                             e.CellBounds.Top + (e.CellBounds.Height - HeadImageSide) / 2, _
                                             HeadImageSide, HeadImageSide)

                    e.Paint(HeadRect, DataGridViewPaintParts.All And Not DataGridViewPaintParts.ContentForeground)

                    e.Graphics.DrawImage(checkImage, HeadRect)
                    e.Handled = True
                End If
            End If
        End If
    End Sub

#End Region


#Region "Methods adapted for use DataGridView instead of ListView"
    ''' <summary>
    ''' Configure the Grid for the list of Tests
    ''' </summary>
    ''' <remarks>
    ''' Creation AG 03/09/2014 - BA-1869 Remove ListView control and use DataGridView control for the Available column functionality
    ''' </remarks>
    Private Sub PrepareTestListGrid()
        Try
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            bsTestListGrid.Columns.Clear()

            Dim witdhToLess As Integer = 0
            If openModeAttribute <> String.Empty Then
                Dim availableColumn As New DataGridViewCheckBoxColumn
                With availableColumn
                    .Name = "Available"
                    .HeaderText = ""
                    .Width = 24
                    witdhToLess += .Width
                    .ToolTipText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_AVAILABILITY", CurrentLanguage)
                End With
                bsTestListGrid.Columns.Add(availableColumn)
            End If

            Dim iconColumn As New DataGridViewImageColumn
            With iconColumn
                .Name = "Icon"
                .HeaderText = ""
                .Width = 24
                .DefaultCellStyle.SelectionBackColor = .DefaultCellStyle.BackColor
                witdhToLess += .Width
            End With
            bsTestListGrid.Columns.Add(iconColumn)

            Dim multiLanguageID As String = "LBL_TestNames"
            If openModeAttribute <> String.Empty AndAlso ScreenIDAttribute = "PROFILE" Then
                multiLanguageID = "LBL_Profiles_ListName"
            End If

            bsTestListGrid.Columns.Add("TestName", myMultiLangResourcesDelegate.GetResourceText(Nothing, multiLanguageID, CurrentLanguage))

            bsTestListGrid.Columns("TestName").Width = 215 - witdhToLess
            bsTestListGrid.Columns("TestName").SortMode = DataGridViewColumnSortMode.Programmatic  'AG 18/09/2014 - do not sort clicking in header!!!!

            bsTestListGrid.Columns.Add("TestID", "")
            bsTestListGrid.Columns("TestID").Visible = False

            bsTestListGrid.Columns.Add("TestType", "")
            bsTestListGrid.Columns("TestType").Visible = False

            bsTestListGrid.MultiSelect = True
            'Properties not found on datagridview
            'bsTestListGrid.HeaderStyle = ColumnHeaderStyle.Clickable
            'bsTestListGrid.Scrollable = True
            'bsTestListGrid.FullRowSelect = True
            'bsTestListGrid.View = View.Details
            'bsTestListGrid.HideSelection = False


        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".PrepareTestListGrid ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".PrepareTestListGrid ", Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Load in the DataGridView, the list of sorted Tests received 
    ''' </summary>
    ''' <param name="pReportsTestsSortingDS">Typed Dataset ReportsTestsSortingDS containing the current test sorting</param>
    ''' <remarks>
    ''' Creation AG 03/09/2014 - BA-1869 Remove ListView control and use DataGridView control for the Available column functionality
    ''' </remarks>
    Private Sub FillTestDataGridView(ByVal pReportsTestsSortingDS As ReportsTestsSortingDS)
        Try
            Dim IconName As String = String.Empty
            Dim myRowIndex As Integer = 0
            Dim dgv As BSDataGridView = bsTestListGrid

            dgv.Rows.Clear()
            For Each testRow As ReportsTestsSortingDS.tcfgReportsTestsSortingRow In pReportsTestsSortingDS.tcfgReportsTestsSorting.Rows
                dgv.Rows.Add() 'New row

                'Available only in TESTSELECT mode
                If openModeAttribute <> String.Empty Then
                    If testRow.IsAvailableNull Then testRow.Available = True 'Set default value is DBNULL
                    dgv("Available", myRowIndex).Value = testRow.Available
                End If

                'Test type icon
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

                        'AG 03/09/2014 - BA-1869
                    Case "PROFILE"
                        IconName = "TPROFILES"
                        Exit Select
                End Select
                dgv("Icon", myRowIndex).Value = TestIconList.Images(IconName)
                dgv("TestName", myRowIndex).Value = testRow.TestName
                dgv("TestID", myRowIndex).Value = testRow.TestID.ToString
                dgv("TestType", myRowIndex).Value = testRow.TestType.ToString

                myRowIndex += 1
            Next

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".FillTestDataGridView ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".FillTestDataGridView ", Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Build a DSataSET to update database
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>
    ''' Creation ??
    ''' AG 03/09/2014 - BA-1869 get also Available column for each item
    ''' </remarks>
    Private Function ReorderTest() As ReportsTestsSortingDS
        Dim myReportSortingDS As New ReportsTestsSortingDS

        Try
            Dim myRepoSortRow As ReportsTestsSortingDS.tcfgReportsTestsSortingRow
            'AG 03/09/2014 - BA-1869
            'Old code using ListView

            'For Each TestItem As ListViewItem In TestListView.Items
            '    myRepoSortRow = myReportSortingDS.tcfgReportsTestsSorting.NewtcfgReportsTestsSortingRow
            '    myRepoSortRow.TestType = TestItem.SubItems(2).Text
            '    myRepoSortRow.TestID = CInt(TestItem.SubItems(1).Text)

            '    'AG 03/09/2014 - BA-1869 - In mode test selection order get also Available
            '    If openMode <> String.Empty AndAlso TestItem.SubItems(3).Text.ToString <> String.Empty Then
            '        myRepoSortRow.Available = CBool(TestItem.SubItems(3).Text)
            '    End If
            '    'AG 03/09/2014 - BA-1869

            '    myRepoSortRow.TestPosition = TestItem.Index + 1

            '    myReportSortingDS.tcfgReportsTestsSorting.AddtcfgReportsTestsSortingRow(myRepoSortRow)
            'Next

            'New code using dataGridView
            Dim myIndex As Integer = 0
            For Each gridRow As DataGridViewRow In bsTestListGrid.Rows
                myRepoSortRow = myReportSortingDS.tcfgReportsTestsSorting.NewtcfgReportsTestsSortingRow
                myRepoSortRow.TestType = gridRow.Cells("TestType").Value.ToString
                myRepoSortRow.TestID = CInt(gridRow.Cells("TestID").Value.ToString)

                If openModeAttribute <> String.Empty AndAlso gridRow.Cells("Available").Value.ToString <> String.Empty Then
                    myRepoSortRow.Available = CBool(gridRow.Cells("Available").Value.ToString)
                End If
                myRepoSortRow.TestPosition = myIndex + 1
                myReportSortingDS.tcfgReportsTestsSorting.AddtcfgReportsTestsSortingRow(myRepoSortRow)
                myIndex += 1
            Next
            'AG 03/09/2014 - BA-1869

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".ReorderTest ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ReorderTest ", Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
        Return myReportSortingDS
    End Function

    ''' <summary>
    ''' Move the group of selected Tests to the Top or the Bottom of the DataGridView
    ''' </summary>
    ''' <param name="pGridView">ListView containing all Tests</param>
    ''' <param name="pMoveTop">Flag indicating if Test are moved to Top(True) or to the Bottom (False)</param>
    ''' <remarks>
    ''' AG 03/09/2014 - BA-1869
    ''' </remarks>
    Private Sub MoveTopOrBottomItemsInGrid(ByVal pGridView As DataGridView, ByVal pMoveTop As Boolean)
        Try
            Dim dgv As BSDataGridView = pGridView

            'If tests are moving to botton, set index to the last element in list
            'If tests are moving to top, set index to the first element in list
            Dim myIndex As Integer = 0
            If (Not pMoveTop) Then myIndex = dgv.Rows.Count

            'Get the list of selected positions (indexes)
            Dim selectedIndexesList As New List(Of Integer)
            For Each selectedRow As DataGridViewRow In dgv.SelectedRows
                If Not selectedIndexesList.Contains(selectedRow.Index) Then
                    selectedIndexesList.Add(selectedRow.Index)
                End If
            Next
            'Sort the indexed
            selectedIndexesList.Sort() 'Increasing (always add on bottom)
            If pMoveTop Then
                'Sort decreasing (always insert on top)
                selectedIndexesList.Reverse()
            End If

            'Insert rows to copy the selected items
            Dim loopItera As Integer = 0
            For Each item As Integer In selectedIndexesList
                If pMoveTop Then 'Insert on top
                    dgv.Rows.Insert(0, 1)
                Else 'Add to the bottom
                    dgv.Rows.Add()
                End If

                'Copy column values
                For i As Integer = 0 To dgv.Columns.Count - 1
                    If pMoveTop Then 'Already inserted on top. The selected index value incremented
                        dgv(i, 0).Value = dgv(i, item + loopItera + 1).Value
                    Else 'Added on bottom. The selected index value NOT CHANGED
                        dgv(i, dgv.Rows.Count - 1).Value = dgv(i, item).Value
                    End If
                Next
                loopItera += 1
            Next

            'Delete the selected rows that have been moved
            For Each selectedTestItem As DataGridViewRow In dgv.SelectedRows
                If dgv.Rows.Count > 1 Then
                    dgv.Rows.Remove(selectedTestItem)
                Else
                    dgv.Rows.Clear()
                End If
            Next

            'Keep the selected items in new positions
            dgv.ClearSelection()
            If pMoveTop Then
                For i As Integer = 0 To selectedIndexesList.Count - 1
                    dgv.Rows(i).Selected = True
                Next
            Else
                For i As Integer = 0 To selectedIndexesList.Count - 1
                    dgv.Rows(dgv.Rows.Count - 1 - i).Selected = True
                Next
            End If
            selectedIndexesList = Nothing

            ChangesMade = True 'TR 13/02/2012 -Set ChangesMade value True 

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".MoveTopOrBottomItemsInGrid ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".MoveTopOrBottomItemsInGrid ", Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        Finally
            'Set the focus on the gridview
            pGridView.Focus()
        End Try
    End Sub

    ''' <summary>
    ''' Move the group of selected Tests Up or Down in the DataViewGrid
    ''' </summary>
    ''' <param name="pGridView">ListView containing all Tests</param>
    ''' <param name="pMoveUp">Flag indicating if Test are moved Up (True) or Down (False)</param>
    ''' <remarks>
    ''' AG 03/09/2014 - BA-1869
    ''' </remarks>
    Private Sub MoveItemsInListInGrid(ByRef pGridView As DataGridView, ByVal pMoveUp As Boolean)
        Try
            Dim dgv As BSDataGridView = pGridView

            'Get the list of selected positions (indexes)
            Dim selectedIndexesList As New List(Of Integer)

            For Each selectedRow As DataGridViewRow In dgv.SelectedRows
                If Not selectedIndexesList.Contains(selectedRow.Index) Then
                    selectedIndexesList.Add(selectedRow.Index)
                End If
            Next

            'Sort the indexed
            selectedIndexesList.Sort() 'Increasing
            If Not pMoveUp Then
                selectedIndexesList.Reverse() 'Sort decreasing 
            End If

            Dim myOffset As Integer = 0
            Dim tmpValue As Object
            For Each item As Integer In selectedIndexesList
                If pMoveUp Then
                    myOffset = -1

                    'Do nothing if item already on top
                    If item > 0 Then
                        'If previous row selected to nothing, else change values between rows
                        If Not dgv.Rows(item + myOffset).Selected Then
                            For i As Integer = 0 To dgv.Columns.Count - 1
                                tmpValue = dgv(i, item + myOffset).Value
                                dgv(i, item + myOffset).Value = dgv(i, item).Value
                                dgv(i, item).Value = tmpValue
                            Next

                            'Keep the selected items in new positions
                            dgv.Rows(item + myOffset).Selected = True
                            dgv.Rows(item).Selected = False
                        End If
                    End If

                Else 'Move down
                    myOffset = 1

                    'Do nothing if item already on bottom
                    If item < dgv.Rows.Count - 1 Then
                        'If next row selected to nothing, else change values between rows
                        If Not dgv.Rows(item + myOffset).Selected Then
                            For i As Integer = 0 To dgv.Columns.Count - 1
                                tmpValue = dgv(i, item + myOffset).Value
                                dgv(i, item + myOffset).Value = dgv(i, item).Value
                                dgv(i, item).Value = tmpValue
                            Next

                            'Keep the selected items in new positions
                            dgv.Rows(item + myOffset).Selected = True
                            dgv.Rows(item).Selected = False
                        End If

                    End If
                End If
            Next

            selectedIndexesList = Nothing
            ChangesMade = True 'TR 13/02/2012 -Set ChangesMade value True 

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".MoveItemsInListInGrid ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".MoveItemsInListInGrid ", Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        Finally
            'Set the focus on the gridview
            pGridView.Focus()
        End Try
    End Sub

#End Region

#Region "OLD methods using ListView replaced for DataGridView"
    ' ''' <summary>
    ' ''' Configure the ListView for the list of Tests
    ' ''' </summary>
    ' ''' <remarks>
    ' ''' Created by:  TR
    ' ''' AG 03/09/2014 - BA-1869 get also Available column for each item
    ' ''' </remarks>
    'Private Sub PrepareTestListView()
    '    Try
    '        TestListView.Items.Clear()
    '        TestListView.Alignment = ListViewAlignment.Left
    '        TestListView.HeaderStyle = ColumnHeaderStyle.Clickable
    '        TestListView.MultiSelect = True
    '        TestListView.Scrollable = True
    '        TestListView.FullRowSelect = True
    '        TestListView.View = View.Details
    '        TestListView.HideSelection = False
    '        TestListView.SmallImageList = TestIconList

    '        Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate
    '        TestListView.Columns.Add(myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_TestNames", CurrentLanguage), -2, HorizontalAlignment.Left)

    '        TestListView.Columns(0).Width = 215 'TestListView.Width
    '        TestListView.Columns(0).Name = "TestName"
    '        TestListView.Columns.Add("TestID", 0, HorizontalAlignment.Left)
    '        TestListView.Columns(1).Name = "TestID"
    '        TestListView.Columns.Add("TestType", 0, HorizontalAlignment.Left)
    '        TestListView.Columns(2).Name = "TestType"

    '        'AG 03/09/2014 - BA-1869 - In mode test selection order show also Available
    '        If openModeAttribute <> String.Empty Then
    '            TestListView.CheckBoxes = True 'AG 03/09/2014 - BA-1869 - It does not work because the check has not an own header in order to check/unchech all
    '            TestListView.Columns.Add("Available", 0, HorizontalAlignment.Left)
    '            TestListView.Columns(3).Name = "Available"
    '        End If
    '        'AG 03/09/2014 - BA-1869
    '    Catch ex As Exception
    '        CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".PrepareTestListView ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
    '        ShowMessage(Name & ".PrepareTestListView ", Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
    '    End Try
    'End Sub

    ' ''' <summary>
    ' ''' Load in the ListView, the list of sorted Tests received 
    ' ''' </summary>
    ' ''' <param name="pReportsTestsSortingDS">Typed Dataset ReportsTestsSortingDS containing the current test sorting</param>
    ' ''' <remarks>
    ' ''' Created by:  TR
    ' ''' AG 03/09/2014 - BA-1869 get also Available column for each item (and profile Icon)
    ' ''' </remarks>
    'Private Sub FillTestListView(ByVal pReportsTestsSortingDS As ReportsTestsSortingDS)
    '    Try
    '        Dim IconName As String = String.Empty
    '        Dim myRowIndex As Integer = 0

    '        TestListView.Items.Clear()
    '        For Each testRow As ReportsTestsSortingDS.tcfgReportsTestsSortingRow In pReportsTestsSortingDS.tcfgReportsTestsSorting.Rows
    '            Select Case testRow.TestType
    '                Case "STD"
    '                    If testRow.PreloadedTest Then
    '                        IconName = "TESTICON"
    '                    Else
    '                        IconName = "USERTEST"
    '                    End If
    '                    Exit Select
    '                Case "CALC"
    '                    IconName = "TCALC"
    '                    Exit Select
    '                Case "ISE"
    '                    IconName = "TISE_SYS"
    '                    Exit Select
    '                Case "OFFS"
    '                    IconName = "TOFF_SYS"
    '                    Exit Select

    '                    'AG 03/09/2014 - BA-1869
    '                Case "PROFILE"
    '                    IconName = "TPROFILES"
    '                    Exit Select

    '            End Select

    '            TestListView.Items.Add(testRow.TestName, IconName).SubItems.Add(testRow.TestID)
    '            TestListView.Items(myRowIndex).SubItems.Add(testRow.TestType)

    '            'AG 03/09/2014 - BA-1869 - In mode test selection order show also Available
    '            If openModeAttribute <> String.Empty Then
    '                TestListView.Items(myRowIndex).SubItems.Add(testRow.Available)
    '            End If

    '            myRowIndex += 1
    '        Next
    '    Catch ex As Exception
    '        CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".FillTestListView ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
    '        ShowMessage(Name & ".FillTestListView ", Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
    '    End Try
    'End Sub

    ' ''' <summary>
    ' ''' Move the group of selected Tests to the Top or the Bottom of the ListView
    ' ''' </summary>
    ' ''' <param name="ptListView">ListView containing all Tests</param>
    ' ''' <param name="pMoveTop">Flag indicating if Test are moved to Top(True) or to the Bottom (False)</param>
    ' ''' <remarks>
    ' ''' Created by: TR
    ' ''' </remarks>
    'Private Sub MoveTopOrBottomItems(ByVal ptListView As ListView, ByVal pMoveTop As Boolean)
    '    Try
    '        'If tests are moving to botton, set index to the last element in list
    '        'If tests are moving to top, set index to the first element in list
    '        Dim myIndex As Integer = 0
    '        If (Not pMoveTop) Then myIndex = ptListView.Items.Count - 1

    '        For Each selectedTestItem As ListViewItem In ptListView.SelectedItems
    '            'Define a new ListViewItem
    '            Dim lviNewItem As ListViewItem = CType(selectedTestItem.Clone(), ListViewItem)

    '            'Remove the currently selected item from the list
    '            selectedTestItem.Remove()
    '            If (pMoveTop) Then
    '                'Insert the new item in it's new place
    '                TestListView.Items.Insert(myIndex, lviNewItem)
    '                myIndex += 1
    '            Else
    '                TestListView.Items.Insert(myIndex, lviNewItem)
    '                myIndex -= 1
    '            End If

    '            lviNewItem.Selected = True
    '            lviNewItem.EnsureVisible()
    '        Next

    '        'TR 13/02/2012 -Set ChangesMade value True 
    '        ChangesMade = True

    '    Catch ex As Exception
    '        CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".MoveTopOrBottomItems ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
    '        ShowMessage(Name & ".MoveTopOrBottomItems ", Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
    '    End Try
    'End Sub

    ' ''' <summary>
    ' ''' Move the group of selected Tests Up or Down in the ListView
    ' ''' </summary>
    ' ''' <param name="pListView">ListView containing all Tests</param>
    ' ''' <param name="pMoveUp">Flag indicating if Test are moved Up (True) or Down (False)</param>
    ' ''' <remarks>
    ' ''' Created by: TR
    ' ''' </remarks>
    'Private Sub MoveItemsInListView(ByRef pListView As ListView, ByVal pMoveUp As Boolean)
    '    Try
    '        'If tests are moving down, set limittedIndex to the last element in list
    '        'If tests are moving up, set limittedIndex to the first element in list
    '        Dim limittedIndex As Integer = (pListView.Items.Count - 1)
    '        If (pMoveUp) Then limittedIndex = 0

    '        'Define a new collection for the indexes of ListView elements to move and field it with the indexes 
    '        'of all selected elements
    '        Dim IndexesToMove As New List(Of Integer)()
    '        For Each SelectedItem As ListViewItem In pListView.SelectedItems
    '            'Add the item's index to the collection
    '            IndexesToMove.Add(SelectedItem.Index)

    '            'If this item is at the limit we defined
    '            If (SelectedItem.Index = limittedIndex) Then
    '                'Do not attempt to move item(s) as we are at the top or bottom of the list
    '                Exit Try
    '            End If
    '        Next

    '        'If tests are moving down
    '        If (Not pMoveUp) Then
    '            'Reverse the index list so that we move items from the bottom of the selection first
    '            IndexesToMove.Reverse()
    '        End If

    '        'Loop through each index we want to move
    '        For Each intIndex As Integer In IndexesToMove
    '            'Define a new ListViewItem
    '            Dim lviNewItem As ListViewItem = CType(pListView.Items(intIndex).Clone(), ListViewItem)

    '            'Remove the currently selected item from the list
    '            pListView.Items(intIndex).Remove()

    '            'Insert the new item in the new position
    '            If (pMoveUp) Then
    '                pListView.Items.Insert(intIndex - 1, lviNewItem)
    '            Else
    '                pListView.Items.Insert(intIndex + 1, lviNewItem)
    '            End If

    '            lviNewItem.Selected = True
    '            lviNewItem.EnsureVisible()
    '        Next
    '        'TR 13/02/2012 -Set ChangesMade value True 
    '        ChangesMade = True

    '    Catch ex As Exception
    '        CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".MoveItemsInListView ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
    '        ShowMessage(Name & ".MoveItemsInListView ", Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
    '    Finally
    '        'Set the focus on the listview
    '        pListView.Focus()
    '    End Try
    'End Sub

#End Region


#Region "Permission Level"

    Sub ReadOnlyMode()
        If (openMode = "TESTSELECTION") Then
            FirstPosButton.Enabled = False
            UpPosButton.Enabled = False
            DownPosButton.Enabled = False
            LastPosButton.Enabled = False
            DefaultSortingButton.Enabled = False
            bsAcceptButton.Enabled = False

            RemoveHandler bsTestListGrid.CellMouseClick, AddressOf bsTestListGrid_CellMouseClick
        End If
    End Sub


    Sub ValidatePermissionLevel(ByVal level As Integer) Implements IPermissionLevel.ValidatePermissionLevel

        Try

            If (IAx00MainMDI.ActiveStatus <> "EMPTY") Then
                ReadOnlyMode()
            Else
                Select Case level

                    Case USER_LEVEL.lADMINISTRATOR
                        Exit Select

                    Case USER_LEVEL.lBIOSYSTEMS
                        Exit Select

                    Case USER_LEVEL.lOPERATOR
                        ReadOnlyMode()
                        Exit Select
                End Select
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".ValidatePermissionLevel ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ValidatePermissionLevel ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try

    End Sub

#End Region

End Class
