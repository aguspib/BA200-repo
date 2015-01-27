Option Strict On
Option Explicit On
Option Infer On

Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global.GlobalEnumerates
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Controls.UserControls
Imports Biosystems.Ax00.PresentationCOM


Public Class UiProgTestContaminations

#Region "Declarations"
    Private EditionMode As Boolean                    'To control when the screen is in Edition Mode
    Private ChangesMade As Boolean                    'To control if there are changes pending to save
    Private ReadOnly SelectedTestID As Integer = -1   'To control which one is the Test currently selected in the list of Contaminators
    Private originalSelectedIndex As Integer = -1

    Private R1TestsDS As New TestContaminationsDS     'DS binded to grid containing Contaminated Tests (R1)
    Private R2TestsDS As New TestContaminationsDS     'DS binded to grid containing Contaminated Tests (R2)

    'TR 09/03/2011 *- USe to indicate the prev selected test.
    Private PrevSelectedTest As String = ""

    'RH 27/09/2011 The form is ReadOnly if ExistExecutions
    Private ExistsExecutions As Boolean = False
#End Region

#Region "Attributes"
    Private WorkSessionIDAttribute As String = ""
    Private WSStatusAttribute As String = ""
#End Region

#Region "Properties"
    Public WriteOnly Property ActiveWorkSession() As String
        Set(ByVal value As String)
            WorkSessionIDAttribute = value
        End Set
    End Property

    Public Property ActiveWSStatus() As String
        Get
            Return WSStatusAttribute
        End Get
        Set(ByVal value As String)
            WSStatusAttribute = value
        End Set
    End Property

#End Region

#Region "Methods"

    Public Sub New()
        'This call is required by the Windows Form Designer.
        InitializeComponent()
    End Sub

    ''' <summary>
    ''' Method incharge to load the buttons images
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 29/11/2010
    ''' </remarks>
    Private Sub PrepareButtons()
        Try
            Dim auxIconName As String = ""
            Dim iconPath As String = IconsPath

            'EDIT Button
            auxIconName = GetIconName("EDIT")
            If (auxIconName <> "") Then
                bsEditButton.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
            End If

            'DELETE Button
            auxIconName = GetIconName("REMOVE")
            If (auxIconName <> "") Then
                bsDeleteButton.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
            End If

            'PRINT Button
            auxIconName = GetIconName("PRINT")
            If (auxIconName <> "") Then
                bsPrintButton.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
            End If

            'SAVE Button
            auxIconName = GetIconName("SAVE")
            If (auxIconName <> "") Then
                bsSaveButton.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
            End If

            'CANCEL Button
            auxIconName = GetIconName("UNDO")
            If (auxIconName <> "") Then
                bsCancelButton.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
            End If

            'SUMMARY BY TEST Button
            auxIconName = GetIconName("GRID")
            If (auxIconName <> "") Then
                bsSummaryByTestButton.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
            End If

            'EXIT Button
            auxIconName = GetIconName("CANCEL")
            If (auxIconName <> "") Then
                bsExitButton.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".PrepareButtons ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".PrepareButtons ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Screen initialization when loading
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 29/11/2010
    ''' </remarks>
    Private Sub ScreenLoad()
        Try
            'Get the current Language from the current Application Session
            'Dim myGlobalbase As New GlobalBase
            CurrentUserLevel = GlobalBase.GetSessionInfo().UserLevel
            Dim currentLanguage As String = GlobalBase.GetSessionInfo().ApplicationLanguage.Trim.ToString


            'RH 27/09/2011 Initialize ExistsExecutions
            ExistsExecutions = (UiAx00MainMDI.ActiveStatus <> "EMPTY")

            'Load the multilanguage texts for all Screen Labels and get Icons for graphical Buttons
            GetScreenLabels(currentLanguage)
            PrepareButtons()

            InitializeGrids(currentLanguage)

            'Initialize and load the ListView of Contaminators
            InitializeListView(currentLanguage)

            'RH 15/12/2010
            ResetBorder()

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".ScreenLoad ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ScreenLoad ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub


    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="pCurrentLanguage"></param>
    ''' <remarks>
    ''' AG 16/12/2010
    ''' Modified by: RH 28/06/2011 Code optimization
    ''' </remarks>
    Private Sub InitializeGrids(Optional ByVal pCurrentLanguage As String = "")
        Try
            'If pcurrentLanguage = "" Then
            '    'Get the current Language from the current Application Session
            '    'Dim currentLanguageGlobal As New GlobalBase
            '    pCurrentLanguage = GlobalBase.GetSessionInfo().ApplicationLanguage.Trim.ToString
            'End If

            ''Load the ComboBoxes of Washing Solutions (for Cuvettes Contaminations)
            'Dim resultData As New GlobalDataTO
            'resultData = LoadWashingSolutionsComboBoxes()
            'If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
            '    Dim lstSorted As New List(Of PreloadedMasterDataDS.tfmwPreloadedMasterDataRow)
            '    lstSorted = DirectCast(resultData.SetDatos, List(Of PreloadedMasterDataDS.tfmwPreloadedMasterDataRow))

            '    'Create columns in DataGridViews of R1 and R2 Contaminations; use the obtained list of Washing Solutions to load
            '    'the correspondent ComboBox column in both grids. After creation, load both grids
            '    InitializeGridViews(lstSorted, pCurrentLanguage)
            '    LoadGridViews()
            'End If

            If String.IsNullOrEmpty(pCurrentLanguage) Then
                'Get the current Language from the current Application Session
                'Dim currentLanguageGlobal As New GlobalBase
                pCurrentLanguage = GlobalBase.GetSessionInfo().ApplicationLanguage
            End If

            'Load the ComboBoxes of Washing Solutions (for Cuvettes Contaminations)
            Dim resultData As GlobalDataTO
            resultData = LoadWashingSolutionsComboBoxes()

            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                Dim lstSorted As List(Of PreloadedMasterDataDS.tfmwPreloadedMasterDataRow)
                lstSorted = DirectCast(resultData.SetDatos, List(Of PreloadedMasterDataDS.tfmwPreloadedMasterDataRow))

                'Create columns in DataGridViews of R1 and R2 Contaminations; use the obtained list of Washing Solutions to load
                'the correspondent ComboBox column in both grids. After creation, load both grids
                InitializeGridViews(lstSorted, pCurrentLanguage)
                LoadGridViews()
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".InitializeGrids ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".InitializeGrids ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", Me)

        End Try
    End Sub

    ''' <summary>
    ''' Get texts in the current application language for all screen controls
    ''' </summary>
    ''' <param name="pLanguageID">Current Application Language</param>
    ''' <remarks>
    ''' Created by: SA 29/11/2010
    ''' </remarks>
    Private Sub GetScreenLabels(ByVal pLanguageID As String)
        Try
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            'For Labels, CheckBox, RadioButtons.....
            bsContaminatorsListLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Contaminators", pLanguageID)
            bsContaminationsLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Contaminations", pLanguageID) 'PG 11/01/2011

            'bsR1GroupBox.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Contaminations_R1", pLanguageID)
            'bsR2GroupBox.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Contaminations_R2", pLanguageID)
            bsR1GroupBox.Text = ""
            bsR2GroupBox.Text = ""

            bsContaminesAllR1Checkbox.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_ContaminatesAll", pLanguageID)
            bsContaminesAllR2Checkbox.Text = bsContaminesAllR1Checkbox.Text

            bsCuvettesGroupBox.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Cuvettes", pLanguageID)
            bsContaminatesCuvettesCheckbox.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Contaminates_Cuvette", pLanguageID)
            bsWashingSolR1Label.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Step_1", pLanguageID) & ":"
            bsWashingSolR2Label.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Step_2", pLanguageID) & ":"

            'bsScreenToolTips.SetToolTip(bsSummaryByTestButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "???", pLanguageID))
            bsScreenToolTips.SetToolTip(bsEditButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Edit", pLanguageID))
            bsScreenToolTips.SetToolTip(bsDeleteButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Delete", pLanguageID))
            bsScreenToolTips.SetToolTip(bsPrintButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Print", pLanguageID))
            bsScreenToolTips.SetToolTip(bsSaveButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Save", pLanguageID))
            bsScreenToolTips.SetToolTip(bsCancelButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Cancel", pLanguageID))
            bsScreenToolTips.SetToolTip(bsExitButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_CloseScreen", pLanguageID))
            bsScreenToolTips.SetToolTip(bsSummaryByTestButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_ContaminationSummary", pLanguageID))

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".GetScreenLabels", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".GetScreenLabels", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Initialize and load the ListView containing the list of Contaminator Tests
    ''' </summary>
    ''' <param name="pLanguageID">Current Application Language</param>
    ''' <remarks>
    ''' Created by: SA 29/11/2010
    ''' </remarks>
    Private Sub InitializeListView(ByVal pLanguageID As String)
        Try
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            'Initialization of Contaminator Tests List
            bsTestContaminatorsListView.Items.Clear()

            bsTestContaminatorsListView.Alignment = ListViewAlignment.Left
            bsTestContaminatorsListView.FullRowSelect = True
            'bsTestContaminatorsListView.MultiSelect = False 'AG 16/12/2010
            bsTestContaminatorsListView.Scrollable = True
            bsTestContaminatorsListView.View = View.Details
            bsTestContaminatorsListView.HideSelection = False
            bsTestContaminatorsListView.HeaderStyle = ColumnHeaderStyle.Nonclickable

            'List columns definition  --> only column containing the Test Name will be visible
            bsTestContaminatorsListView.Columns.Add(myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_TestNames", pLanguageID), -2, HorizontalAlignment.Left)

            bsTestContaminatorsListView.Columns(0).Width = 200 'TR 23/04/2012

            bsTestContaminatorsListView.Columns.Add("TestID", 0, HorizontalAlignment.Left)
            bsTestContaminatorsListView.Columns.Add("ReagentsNumber", 0, HorizontalAlignment.Left)
            bsTestContaminatorsListView.Columns.Add("InUse", 0, HorizontalAlignment.Left)

            'Fill ListView with the list of existing Standard Tests
            LoadTestContaminatorsList()
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".InitializeListView ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".InitializeListView ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Get the list of available Standard Tests to load the ListView
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 29/11/2010
    ''' </remarks>
    Private Sub LoadTestContaminatorsList()
        Try
            Dim myIcons As New ImageList

            'Get the Icons defined for Preloaded Standard Tests that are inuse/not inuse in the current Work Session
            Dim iconName As String = GetIconName("TESTICON")
            If (iconName <> "") Then myIcons.Images.Add("TESTICON", ImageUtilities.ImageFromFile(IconsPath & iconName))

            iconName = GetIconName("INUSETEST")
            If (iconName <> "") Then myIcons.Images.Add("INUSETEST", ImageUtilities.ImageFromFile(IconsPath & iconName))

            'Get the Icon defined for User Defined Standard Tests that are not inuse/not inuse in the current Work Session
            iconName = GetIconName("USERTEST")
            If (iconName <> "") Then myIcons.Images.Add("USERTEST", ImageUtilities.ImageFromFile(IconsPath & iconName))
            'TR 10/01/2013 -Correct the icon name.
            iconName = GetIconName("INUSUSTEST")
            If (iconName <> "") Then myIcons.Images.Add("INUSUSEST", ImageUtilities.ImageFromFile(IconsPath & iconName))
            'TR 10/01/2013 -END.
            'Assign the Icons to the Contaminator Tests List View
            bsTestContaminatorsListView.Items.Clear()
            bsTestContaminatorsListView.SmallImageList = myIcons

            'Get the list of Standard Tests and load the ListView
            Dim resultData As New GlobalDataTO
            Dim myTestsDelegate As New TestsDelegate

            resultData = myTestsDelegate.GetList(Nothing)
            If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then '(1)
                Dim myTestsDS As New TestsDS
                myTestsDS = DirectCast(resultData.SetDatos, TestsDS)

                resultData = myTestsDelegate.ReadAllContaminatorsList(Nothing)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then '(2)
                    Dim myContaminatorsDS As New TestContaminationsDS
                    myContaminatorsDS = CType(resultData.SetDatos, TestContaminationsDS)

                    'Fill the List View with the list of Contaminator Tests
                    Dim i As Integer = 0
                    For Each contaminatorTest As TestsDS.tparTestsRow In myTestsDS.tparTests.Rows
                        If (contaminatorTest.PreloadedTest) Then
                            If (Not contaminatorTest.InUse) Then
                                iconName = "TESTICON"
                            Else
                                iconName = "INUSETEST"
                            End If
                        Else
                            If (Not contaminatorTest.InUse) Then
                                iconName = "USERTEST"
                            Else
                                iconName = "INUSUSEST"
                            End If
                        End If

                        bsTestContaminatorsListView.Items.Add(contaminatorTest.TestID.ToString, _
                                                              contaminatorTest.TestName.ToString, _
                                                              iconName).Tag = contaminatorTest.InUse

                        bsTestContaminatorsListView.Items(i).SubItems.Add(contaminatorTest.ReagentsNumber.ToString)
                        bsTestContaminatorsListView.Items(i).SubItems.Add(contaminatorTest.InUse.ToString)

                        'If test is contaminator then mark in different colour
                        Dim myLinQ As List(Of TestContaminationsDS.tparContaminationsRow) = _
                                        (From a As TestContaminationsDS.tparContaminationsRow In myContaminatorsDS.tparContaminations _
                                         Where a.TestID = contaminatorTest.TestID Select a).ToList
                        If myLinQ.Count > 0 Then
                            bsTestContaminatorsListView.Items(i).ForeColor = Color.Red
                        Else
                            'TR 13/12/2011 -Search if it's a cuvette Contamination
                            Dim myContaminationsDelegate As New ContaminationsDelegate
                            resultData = myContaminationsDelegate.ReadByTestID(Nothing, contaminatorTest.TestID)

                            If Not resultData.HasError Then
                                Dim myTempContaminatorDS As New ContaminationsDS
                                myTempContaminatorDS = DirectCast(resultData.SetDatos, ContaminationsDS)
                                If myTempContaminatorDS.tparContaminations.Count > 0 Then
                                    'Valida if contamination type = CUVETTES
                                    If myTempContaminatorDS.tparContaminations(0).ContaminationType = "CUVETTES" Then
                                        bsTestContaminatorsListView.Items(i).ForeColor = Color.Red 'Change Color to Red.
                                    End If
                                End If
                            Else
                                'Error getting the list of Standard Tests, show the error message
                                ShowMessage(Name & ".LoadTestContaminatorsList", resultData.ErrorCode, resultData.ErrorMessage, Me)
                            End If
                            'TR 13/12/2011 -END
                        End If

                        i += 1
                    Next

                    If Not resultData.HasError Then
                        'Load all Contaminations defined for the first Test in the list
                        If (bsTestContaminatorsListView.Items.Count > 0) Then
                            bsTestContaminatorsListView.Items(0).Selected = True

                            ReadOnlyMode()
                            LoadContaminationsByTest(CInt(bsTestContaminatorsListView.Items(0).Name))
                        End If
                    End If


                Else
                    'Error getting the list of Standard Tests, show the error message
                    ShowMessage(Name & ".LoadTestContaminatorsList", resultData.ErrorCode, resultData.ErrorMessage, Me)
                End If

            Else '(1)
                'Error getting the list of Standard Tests, show the error message
                ShowMessage(Name & ".LoadTestContaminatorsList", resultData.ErrorCode, resultData.ErrorMessage, Me)
            End If '(1)

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".LoadTestContaminatorsList ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".LoadTestContaminatorsList ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Initialization of DataGridViews for R1 Contaminations and R2 Contaminations
    ''' </summary>
    ''' <param name="pWashingSolutionsList">List of available Washing Solutions</param>
    ''' <param name="pLanguageID">Current Application Language</param>
    ''' <remarks>
    ''' Created by: SA 29/11/2010
    ''' </remarks>
    Private Sub InitializeGridViews(ByVal pWashingSolutionsList As List(Of PreloadedMasterDataDS.tfmwPreloadedMasterDataRow), _
                                    ByVal pLanguageID As String)
        Try
            Dim columnName As String
            Dim columnHeaderText As String
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            bsR1ContaminatedDataGridView.AllowUserToAddRows = False
            bsR1ContaminatedDataGridView.AllowUserToDeleteRows = False

            bsR1ContaminatedDataGridView.Rows.Clear()
            bsR1ContaminatedDataGridView.Columns.Clear()
            bsR1ContaminatedDataGridView.EditMode = DataGridViewEditMode.EditOnEnter

            bsR2ContaminatedDataGridView.AllowUserToAddRows = False
            bsR2ContaminatedDataGridView.AllowUserToDeleteRows = False

            bsR2ContaminatedDataGridView.Rows.Clear()
            bsR2ContaminatedDataGridView.Columns.Clear()
            bsR2ContaminatedDataGridView.EditMode = DataGridViewEditMode.EditOnEnter

            'Selected column
            columnName = "Selected"

            Dim checkBox1Col As New DataGridViewCheckBoxColumn() With {.Name = columnName, .HeaderText = ""}

            bsR1ContaminatedDataGridView.Columns.Add(checkBox1Col)
            bsR1ContaminatedDataGridView.Columns(columnName).Width = 20
            bsR1ContaminatedDataGridView.Columns(columnName).DataPropertyName = columnName
            bsR1ContaminatedDataGridView.Columns(columnName).Resizable = DataGridViewTriState.False
            bsR1ContaminatedDataGridView.Columns(columnName).SortMode = DataGridViewColumnSortMode.Programmatic

            Dim checkBox2Col As New DataGridViewCheckBoxColumn() With {.Name = columnName, .HeaderText = ""}

            bsR2ContaminatedDataGridView.Columns.Add(checkBox2Col)
            bsR2ContaminatedDataGridView.Columns(columnName).Width = 20
            bsR2ContaminatedDataGridView.Columns(columnName).DataPropertyName = columnName
            bsR2ContaminatedDataGridView.Columns(columnName).Resizable = DataGridViewTriState.False
            bsR2ContaminatedDataGridView.Columns(columnName).SortMode = DataGridViewColumnSortMode.Programmatic

            'ContaminationID column 
            columnName = "ContaminationID"
            bsR1ContaminatedDataGridView.Columns.Add(columnName, "")
            bsR1ContaminatedDataGridView.Columns(columnName).Width = 0
            bsR1ContaminatedDataGridView.Columns(columnName).DataPropertyName = columnName
            bsR1ContaminatedDataGridView.Columns(columnName).ReadOnly = True
            bsR1ContaminatedDataGridView.Columns(columnName).Visible = False

            bsR2ContaminatedDataGridView.Columns.Add(columnName, "")
            bsR2ContaminatedDataGridView.Columns(columnName).Width = 0
            bsR2ContaminatedDataGridView.Columns(columnName).DataPropertyName = columnName
            bsR2ContaminatedDataGridView.Columns(columnName).ReadOnly = True
            bsR2ContaminatedDataGridView.Columns(columnName).Visible = False

            'TestID column
            columnName = "TestID"
            bsR1ContaminatedDataGridView.Columns.Add(columnName, "")
            bsR1ContaminatedDataGridView.Columns(columnName).Width = 0
            bsR1ContaminatedDataGridView.Columns(columnName).Visible = False
            bsR1ContaminatedDataGridView.Columns(columnName).DataPropertyName = columnName
            bsR1ContaminatedDataGridView.Columns(columnName).ReadOnly = True

            bsR2ContaminatedDataGridView.Columns.Add(columnName, "")
            bsR2ContaminatedDataGridView.Columns(columnName).Width = 0
            bsR2ContaminatedDataGridView.Columns(columnName).Visible = False
            bsR2ContaminatedDataGridView.Columns(columnName).DataPropertyName = columnName
            bsR2ContaminatedDataGridView.Columns(columnName).ReadOnly = True

            'TestName column
            columnName = "TestName"
            columnHeaderText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Test", pLanguageID)
            bsR1ContaminatedDataGridView.Columns.Add(columnName, columnHeaderText)
            bsR1ContaminatedDataGridView.Columns(columnName).Width = 190 '150
            bsR1ContaminatedDataGridView.Columns(columnName).DataPropertyName = columnName
            bsR1ContaminatedDataGridView.Columns(columnName).ReadOnly = True
            bsR1ContaminatedDataGridView.Columns(columnName).SortMode = DataGridViewColumnSortMode.Programmatic

            bsR2ContaminatedDataGridView.Columns.Add(columnName, columnHeaderText)
            bsR2ContaminatedDataGridView.Columns(columnName).Width = 190 '150
            bsR2ContaminatedDataGridView.Columns(columnName).DataPropertyName = columnName
            bsR2ContaminatedDataGridView.Columns(columnName).ReadOnly = True
            bsR2ContaminatedDataGridView.Columns(columnName).SortMode = DataGridViewColumnSortMode.Programmatic

            'ReagentID column
            columnName = "ReagentID"
            bsR1ContaminatedDataGridView.Columns.Add(columnName, "")
            bsR1ContaminatedDataGridView.Columns(columnName).Width = 0
            bsR1ContaminatedDataGridView.Columns(columnName).DataPropertyName = columnName
            bsR1ContaminatedDataGridView.Columns(columnName).ReadOnly = True
            bsR1ContaminatedDataGridView.Columns(columnName).Visible = False

            bsR2ContaminatedDataGridView.Columns.Add(columnName, "")
            bsR2ContaminatedDataGridView.Columns(columnName).Width = 0
            bsR2ContaminatedDataGridView.Columns(columnName).DataPropertyName = columnName
            bsR2ContaminatedDataGridView.Columns(columnName).ReadOnly = True
            bsR2ContaminatedDataGridView.Columns(columnName).Visible = False

            'IsShared column 
            columnName = "IsShared"
            bsR1ContaminatedDataGridView.Columns.Add(columnName, "")
            bsR1ContaminatedDataGridView.Columns(columnName).Width = 0
            bsR1ContaminatedDataGridView.Columns(columnName).DataPropertyName = columnName
            bsR1ContaminatedDataGridView.Columns(columnName).ReadOnly = True
            bsR1ContaminatedDataGridView.Columns(columnName).Visible = False

            bsR2ContaminatedDataGridView.Columns.Add(columnName, "")
            bsR2ContaminatedDataGridView.Columns(columnName).Width = 0
            bsR2ContaminatedDataGridView.Columns(columnName).DataPropertyName = columnName
            bsR2ContaminatedDataGridView.Columns(columnName).ReadOnly = True
            bsR2ContaminatedDataGridView.Columns(columnName).Visible = False

            'Washing Solutions Column
            columnName = "WashingSolution"
            columnHeaderText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_INST_Wash", pLanguageID)

            Dim comboBox1Col As New DataGridViewComboBoxColumn() With {.DataPropertyName = columnName, .Name = columnName, .HeaderText = columnHeaderText, .DataSource = pWashingSolutionsList, .DisplayMember = "FixedItemDesc", .ValueMember = "ItemID"}

            bsR1ContaminatedDataGridView.Columns.Add(comboBox1Col)
            bsR1ContaminatedDataGridView.Columns(columnName).Width = 185 '186
            bsR1ContaminatedDataGridView.Columns(columnName).ReadOnly = False
            bsR1ContaminatedDataGridView.Columns(columnName).SortMode = DataGridViewColumnSortMode.Programmatic

            Dim comboBox2Col As New DataGridViewComboBoxColumn() With {.DataPropertyName = columnName, .Name = columnName, .HeaderText = columnHeaderText, .DataSource = pWashingSolutionsList, .DisplayMember = "FixedItemDesc", .ValueMember = "ItemID"}

            bsR2ContaminatedDataGridView.Columns.Add(comboBox2Col)
            bsR2ContaminatedDataGridView.Columns(columnName).Width = 185 '186
            bsR2ContaminatedDataGridView.Columns(columnName).ReadOnly = False
            bsR2ContaminatedDataGridView.Columns(columnName).SortMode = DataGridViewColumnSortMode.Programmatic

            'Visible column 
            columnName = "Visible"
            bsR1ContaminatedDataGridView.Columns.Add(columnName, "")
            bsR1ContaminatedDataGridView.Columns(columnName).Width = 0
            bsR1ContaminatedDataGridView.Columns(columnName).DataPropertyName = columnName
            bsR1ContaminatedDataGridView.Columns(columnName).ReadOnly = True
            bsR1ContaminatedDataGridView.Columns(columnName).Visible = False

            bsR2ContaminatedDataGridView.Columns.Add(columnName, "")
            bsR2ContaminatedDataGridView.Columns(columnName).Width = 0
            bsR2ContaminatedDataGridView.Columns(columnName).DataPropertyName = columnName
            bsR2ContaminatedDataGridView.Columns(columnName).ReadOnly = True
            bsR2ContaminatedDataGridView.Columns(columnName).Visible = False


            'Saved column (create for not move the selected rows when enter on edition mode)
            '"A" saved, "B" not saved
            columnName = "AlreadySaved"

            bsR1ContaminatedDataGridView.Columns.Add(columnName, "")
            bsR1ContaminatedDataGridView.Columns(columnName).Width = 0
            bsR1ContaminatedDataGridView.Columns(columnName).DataPropertyName = columnName
            bsR1ContaminatedDataGridView.Columns(columnName).ReadOnly = True
            bsR1ContaminatedDataGridView.Columns(columnName).Visible = False
            bsR1ContaminatedDataGridView.Columns(columnName).SortMode = DataGridViewColumnSortMode.Programmatic

            bsR2ContaminatedDataGridView.Columns.Add(columnName, "")
            bsR2ContaminatedDataGridView.Columns(columnName).Width = 0
            bsR2ContaminatedDataGridView.Columns(columnName).DataPropertyName = columnName
            bsR2ContaminatedDataGridView.Columns(columnName).ReadOnly = True
            bsR2ContaminatedDataGridView.Columns(columnName).Visible = False
            bsR2ContaminatedDataGridView.Columns(columnName).SortMode = DataGridViewColumnSortMode.Programmatic


        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".InitializeGridViews ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".InitializeGridViews ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Load ComboBoxes of available Washing Solutions
    ''' </summary>
    ''' <remarks>
    ''' Created by: SA 29/11/2010
    ''' Modified by: RH 28/06/2011 Remove ISE_WASH_SOL from WASHING_SOLUTIONS. Code optimization.
    ''' </remarks>
    Private Function LoadWashingSolutionsComboBoxes() As GlobalDataTO
        'Dim resultData As New GlobalDataTO
        'Try
        '    Dim myPMDDelegate As New PreloadedMasterDataDelegate

        '    resultData = myPMDDelegate.GetList(Nothing, PreloadedMasterDataEnum.WASHING_SOLUTIONS)
        '    If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
        '        Dim myPreloadedDataR1DS As New PreloadedMasterDataDS
        '        myPreloadedDataR1DS = DirectCast(resultData.SetDatos, PreloadedMasterDataDS)

        '        If (myPreloadedDataR1DS.tfmwPreloadedMasterData.Rows.Count > 0) Then
        '            'Add an empty Item
        '            Dim emptyWashSolRow As DataRow = myPreloadedDataR1DS.tfmwPreloadedMasterData.NewRow
        '            emptyWashSolRow(0) = PreloadedMasterDataEnum.WASHING_SOLUTIONS.ToString
        '            emptyWashSolRow(1) = " "
        '            emptyWashSolRow(2) = ""
        '            emptyWashSolRow(3) = ""
        '            emptyWashSolRow(4) = 0
        '            emptyWashSolRow(5) = True
        '            emptyWashSolRow(6) = ""

        '            myPreloadedDataR1DS.tfmwPreloadedMasterData.Rows.Add(emptyWashSolRow)
        '            myPreloadedDataR1DS.tfmwPreloadedMasterData.DefaultView.Sort = "Position"

        '            'Fill ComboBox of Washing Solutions for R1 (Cuvettes Contaminations)
        '            bsWashingSolR1ComboBox.DataSource = myPreloadedDataR1DS.tfmwPreloadedMasterData
        '            bsWashingSolR1ComboBox.DisplayMember = "FixedItemDesc"
        '            bsWashingSolR1ComboBox.ValueMember = "ItemID"

        '            'Copy all available Washing Solutions to another DS
        '            Using myPreloadedDataR2DS As New PreloadedMasterDataDS()
        '                For Each washingSol As PreloadedMasterDataDS.tfmwPreloadedMasterDataRow In myPreloadedDataR1DS.tfmwPreloadedMasterData.Rows
        '                    myPreloadedDataR2DS.tfmwPreloadedMasterData.ImportRow(washingSol)
        '                Next
        '                myPreloadedDataR2DS.tfmwPreloadedMasterData.DefaultView.Sort = "Position"
        '                'Fill ComboBox of Washing Solutions for R2 (Cuvettes Contaminations)
        '                bsWashingSolR2ComboBox.DataSource = myPreloadedDataR2DS.tfmwPreloadedMasterData
        '            End Using
        '            bsWashingSolR2ComboBox.DisplayMember = "FixedItemDesc"
        '            bsWashingSolR2ComboBox.ValueMember = "ItemID"

        '            'Create a list with all Washing Solutions and return it
        '            Dim lstSorted As New List(Of PreloadedMasterDataDS.tfmwPreloadedMasterDataRow)
        '            lstSorted = (From a In myPreloadedDataR1DS.tfmwPreloadedMasterData _
        '                     Order By a.Position _
        '                       Select a).ToList()

        '            resultData.SetDatos = lstSorted
        '            resultData.HasError = False
        '        End If
        '    Else
        '        'Error getting the list of defined Washing Solutions, show the error message
        '        ShowMessage(Name & ".LoadWashingSolutionsComboBoxes", resultData.ErrorCode, resultData.ErrorMessage, Me)
        '    End If

        Dim resultData As GlobalDataTO = Nothing

        Try
            Dim myPMDDelegate As New PreloadedMasterDataDelegate

            resultData = myPMDDelegate.GetList(Nothing, PreloadedMasterDataEnum.WASHING_SOLUTIONS)

            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                Dim ReturnedDS As PreloadedMasterDataDS = DirectCast(resultData.SetDatos, PreloadedMasterDataDS)

                If (ReturnedDS.tfmwPreloadedMasterData.Rows.Count > 0) Then
                    Dim PreloadedDataR1Table As New PreloadedMasterDataDS.tfmwPreloadedMasterDataDataTable()
                    Dim PreloadedDataR2Table As New PreloadedMasterDataDS.tfmwPreloadedMasterDataDataTable()

                    'Add an empty Item
                    Dim emptyWashSolRow As PreloadedMasterDataDS.tfmwPreloadedMasterDataRow
                    emptyWashSolRow = PreloadedDataR1Table.NewtfmwPreloadedMasterDataRow()

                    emptyWashSolRow.SubTableID = PreloadedMasterDataEnum.WASHING_SOLUTIONS.ToString()
                    emptyWashSolRow.ItemID = " "
                    emptyWashSolRow.FixedItemDesc = String.Empty
                    emptyWashSolRow.LangDescription = String.Empty
                    emptyWashSolRow.Position = 0
                    emptyWashSolRow.Status = True
                    emptyWashSolRow.Description = String.Empty

                    PreloadedDataR1Table.Rows.Add(emptyWashSolRow)
                    PreloadedDataR2Table.ImportRow(emptyWashSolRow)

                    For Each washingSol As PreloadedMasterDataDS.tfmwPreloadedMasterDataRow In ReturnedDS.tfmwPreloadedMasterData.Rows
                        If washingSol.ItemID <> "WASHSOL3" Then
                            PreloadedDataR1Table.ImportRow(washingSol)
                            PreloadedDataR2Table.ImportRow(washingSol)
                        End If
                    Next

                    PreloadedDataR1Table.DefaultView.Sort = "Position"
                    PreloadedDataR2Table.DefaultView.Sort = "Position"

                    'Fill ComboBox of Washing Solutions for R1 (Cuvettes Contaminations)
                    bsWashingSolR1ComboBox.DataSource = PreloadedDataR1Table
                    bsWashingSolR1ComboBox.DisplayMember = "FixedItemDesc"
                    bsWashingSolR1ComboBox.ValueMember = "ItemID"

                    'Fill ComboBox of Washing Solutions for R2 (Cuvettes Contaminations)
                    bsWashingSolR2ComboBox.DataSource = PreloadedDataR2Table
                    bsWashingSolR2ComboBox.DisplayMember = "FixedItemDesc"
                    bsWashingSolR2ComboBox.ValueMember = "ItemID"

                    'Create a list with all Washing Solutions and return it
                    Dim lstSorted As List(Of PreloadedMasterDataDS.tfmwPreloadedMasterDataRow)
                    lstSorted = (From a In PreloadedDataR1Table _
                                 Order By a.Position _
                                 Select a).ToList()

                    resultData.SetDatos = lstSorted
                    resultData.HasError = False
                End If
            Else
                'Error getting the list of defined Washing Solutions, show the error message
                ShowMessage(Name & ".LoadWashingSolutionsComboBoxes", resultData.ErrorCode, resultData.ErrorMessage, Me)
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".LoadWashingSolutionsComboBoxes ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".LoadWashingSolutionsComboBoxes ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", Me)

        End Try

        Return resultData
    End Function

    ''' <summary>
    ''' Load all Tests using R1 and R2
    ''' </summary>
    ''' <remarks>
    ''' Created by: SA 29/11/2010
    ''' Modified by: RH 28/06/2011 Code optimization
    ''' </remarks>
    Private Sub LoadGridViews()
        Try
            Dim resultData As GlobalDataTO
            Dim myTestReagentsDelegate As New TestReagentsDelegate

            resultData = myTestReagentsDelegate.GetByReagentNumber(Nothing, 1)
            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                R1TestsDS = DirectCast(resultData.SetDatos, TestContaminationsDS)

                Dim viewR1Tests As DataView
                viewR1Tests = R1TestsDS.tparContaminations.DefaultView
                viewR1Tests.Sort = "Selected DESC, TestName ASC"

                Dim bsR1TestsBindingSource As BindingSource = New BindingSource() With {.DataSource = viewR1Tests}
                bsR1ContaminatedDataGridView.DataSource = bsR1TestsBindingSource

                resultData = myTestReagentsDelegate.GetByReagentNumber(Nothing, 2)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    R2TestsDS = DirectCast(resultData.SetDatos, TestContaminationsDS)

                    Dim viewR2Tests As DataView
                    viewR2Tests = R2TestsDS.tparContaminations.DefaultView
                    viewR2Tests.Sort = "Selected DESC, TestName ASC"

                    Dim bsR2TestsBindingSource As BindingSource = New BindingSource() With {.DataSource = viewR2Tests}
                    bsR2ContaminatedDataGridView.DataSource = bsR2TestsBindingSource

                    ForceVisibilityFirstItem()
                Else
                    'Error getting the list of Tests using Reagent 2; shown it
                    ShowMessage(Name & ".LoadGridsByTest", resultData.ErrorCode, resultData.ErrorMessage, Me)
                End If
            Else
                'Error getting the list of Tests using Reagent 1; shown it
                ShowMessage(Name & ".LoadGridViews", resultData.ErrorCode, resultData.ErrorMessage, Me)
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".LoadGridViews ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".LoadGridViews ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", Me)

        End Try
    End Sub

    ''' <summary>
    ''' When a new Test is selected as Contaminator, controls in Contaminated area are initialized 
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 01/12/2010
    ''' </remarks>
    Private Sub InitializeContaminatedArea(Optional ByVal pTestID As Integer = -1)
        Try
            If (pTestID <> -1) Then
                R1TestsDS.tparContaminations.DefaultView.RowFilter = " TestID <> " & pTestID
                R2TestsDS.tparContaminations.DefaultView.RowFilter = " TestID <> " & pTestID
            Else
                R1TestsDS.tparContaminations.DefaultView.RowFilter = ""
                R2TestsDS.tparContaminations.DefaultView.RowFilter = ""
            End If
            bsContaminesAllR1Checkbox.Checked = False
            bsContaminesAllR2Checkbox.Checked = False

            Dim lstContaminatedR1 As New List(Of TestContaminationsDS.tparContaminationsRow)
            lstContaminatedR1 = (From a As TestContaminationsDS.tparContaminationsRow In R1TestsDS.tparContaminations _
                                Where a.Selected = True _
                               Select a).ToList

            For Each contaminatedTest As TestContaminationsDS.tparContaminationsRow In lstContaminatedR1
                contaminatedTest.BeginEdit()
                contaminatedTest.Selected = False
                contaminatedTest.AlreadySaved = "N"
                contaminatedTest.SetContaminationIDNull()
                contaminatedTest.SetWashingSolutionNull()
                contaminatedTest.EndEdit()
            Next


            Dim lstContaminatedR2 As New List(Of TestContaminationsDS.tparContaminationsRow)
            lstContaminatedR2 = (From a As TestContaminationsDS.tparContaminationsRow In R2TestsDS.tparContaminations _
                                Where a.Selected = True _
                               Select a).ToList

            For Each contaminatedTest As TestContaminationsDS.tparContaminationsRow In lstContaminatedR2
                contaminatedTest.BeginEdit()
                contaminatedTest.Selected = False
                contaminatedTest.AlreadySaved = "N"
                contaminatedTest.SetContaminationIDNull()
                contaminatedTest.SetWashingSolutionNull()
                contaminatedTest.EndEdit()
            Next

            'Initialize Cuvettes Contaminations
            bsContaminatesCuvettesCheckbox.Checked = False
            bsWashingSolR1ComboBox.SelectedIndex = 0
            bsWashingSolR2ComboBox.SelectedIndex = 0
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".InitializeContaminatedArea ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".InitializeContaminatedArea ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Load all Contaminations defined for the specified Standard Test
    ''' </summary>
    ''' <param name="pTestID">Test Identifier</param>
    ''' <remarks>
    ''' Created by: SA 29/11/2010
    ''' Modified by RH 28/06/2011 Code optimization.
    ''' </remarks>
    Private Sub LoadContaminationsByTest(ByVal pTestID As Integer)
        Try
            'Initialize the Contaminated Area
            InitializeContaminatedArea(pTestID)

            'Get all Contaminations currently defined for the Test
            Dim resultData As GlobalDataTO

            Dim myContaminationsDelegate As New ContaminationsDelegate

            resultData = myContaminationsDelegate.GetTestRNAsContaminator(Nothing, pTestID)

            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                Dim myContaminationsDS As ContaminationsDS
                myContaminationsDS = DirectCast(resultData.SetDatos, ContaminationsDS)

                For Each testContamination As ContaminationsDS.tparContaminationsRow In myContaminationsDS.tparContaminations.Rows
                    If (testContamination.ContaminationType = "R1") Then
                        Dim lstContaminatedR1 As List(Of TestContaminationsDS.tparContaminationsRow)
                        lstContaminatedR1 = (From a As TestContaminationsDS.tparContaminationsRow In R1TestsDS.tparContaminations _
                                            Where a.ReagentID = testContamination.ReagentContaminatedID _
                                           Select a).ToList

                        For Each contaminatedTest As TestContaminationsDS.tparContaminationsRow In lstContaminatedR1
                            contaminatedTest.BeginEdit()
                            contaminatedTest.Selected = True
                            contaminatedTest.AlreadySaved = "Y"
                            contaminatedTest.ContaminationID = testContamination.ContaminationID

                            If (Not testContamination.IsWashingSolutionR1Null) Then
                                contaminatedTest.WashingSolution = testContamination.WashingSolutionR1
                            End If
                            contaminatedTest.EndEdit()
                        Next
                        R1TestsDS.AcceptChanges() 'AG

                    ElseIf (testContamination.ContaminationType = "R2") Then
                        Dim lstContaminatedR2 As List(Of TestContaminationsDS.tparContaminationsRow)
                        lstContaminatedR2 = (From a As TestContaminationsDS.tparContaminationsRow In R2TestsDS.tparContaminations _
                                            Where a.ReagentID = testContamination.ReagentContaminatedID _
                                           Select a).ToList

                        For Each contaminatedTest As TestContaminationsDS.tparContaminationsRow In lstContaminatedR2
                            contaminatedTest.BeginEdit()
                            contaminatedTest.Selected = True
                            contaminatedTest.AlreadySaved = "Y"
                            contaminatedTest.ContaminationID = testContamination.ContaminationID

                            If (Not testContamination.IsWashingSolutionR2Null) Then
                                contaminatedTest.WashingSolution = testContamination.WashingSolutionR2
                            End If
                            contaminatedTest.EndEdit()
                        Next
                        R2TestsDS.AcceptChanges() 'AG

                    ElseIf (testContamination.ContaminationType = "CUVETTES") Then
                        bsContaminatesCuvettesCheckbox.Checked = True

                        'If (testContamination.ContaminatedType = "R1") Then
                        '    bsWashingSolR1ComboBox.SelectedValue = testContamination.WashingSolutionR1
                        'ElseIf (testContamination.ContaminatedType = "R2") Then
                        '    bsWashingSolR2ComboBox.SelectedValue = testContamination.WashingSolutionR2
                        'End If
                        If Not testContamination.IsWashingSolutionR1Null Then bsWashingSolR1ComboBox.SelectedValue = testContamination.WashingSolutionR1
                        If Not testContamination.IsWashingSolutionR2Null Then bsWashingSolR2ComboBox.SelectedValue = testContamination.WashingSolutionR2
                    End If
                Next

                ForceVisibilityFirstItem()
            Else
                'Error getting the list of Contaminations defined for the specified Test; shown it
                ShowMessage(Name & ".LoadContaminationsByTest", resultData.ErrorCode, resultData.ErrorMessage, Me)
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".LoadContaminationsByTest ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".LoadContaminationsByTest ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Select/unselect all Tests in the grid that corresponds to the informed Contamination Type
    ''' </summary>
    ''' <param name="pType">Contamination type: R1 or R2</param>
    ''' <param name="pCheckedState">True to select; False to unselect</param>
    ''' <remarks>
    ''' Created by:  SA 01/12/2010
    ''' </remarks>
    Private Sub TestContaminatesEverything(ByVal pType As String, ByVal pCheckedState As Boolean)
        Try
            If EditionMode Then ChangesMade = True
            If (pType = "R1") Then
                For Each test As TestContaminationsDS.tparContaminationsRow In R1TestsDS.tparContaminations.Rows
                    If (test.TestID <> CInt(bsTestContaminatorsListView.SelectedItems(0).Name)) Then
                        test.BeginEdit()
                        test.Selected = pCheckedState
                        test.EndEdit()
                    End If
                Next

            ElseIf (pType = "R2") Then
                For Each test As TestContaminationsDS.tparContaminationsRow In R2TestsDS.tparContaminations.Rows
                    If (test.TestID <> CInt(bsTestContaminatorsListView.SelectedItems(0).Name)) Then
                        test.BeginEdit()
                        test.Selected = pCheckedState
                        test.EndEdit()
                    End If
                Next
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".TestContaminatesEverything ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".TestContaminatesEverything ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Control availability of Combos for R1 and R2 Washing Solutions when the CheckBox of Cuvettes
    ''' Contamination is selected
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 01/12/2010
    ''' </remarks>
    Private Sub ChangeCuvettesSelection()
        Try
            If (bsContaminatesCuvettesCheckbox.Checked) Then
                bsWashingSolR1ComboBox.BackColor = Color.White
                bsWashingSolR1ComboBox.Enabled = True
                'TR 14/12/2011 -Validate if there is a selected item in the combo to enable the R2 Combobox.
                If (bsWashingSolR1ComboBox.SelectedIndex > 0) Then
                    bsWashingSolR2ComboBox.BackColor = Color.White
                    bsWashingSolR2ComboBox.Enabled = True
                Else
                    bsWashingSolR2ComboBox.SelectedIndex = 0
                    bsWashingSolR2ComboBox.BackColor = SystemColors.MenuBar
                    bsWashingSolR2ComboBox.Enabled = False
                End If

            Else
                bsWashingSolR1ComboBox.SelectedIndex = 0
                bsWashingSolR1ComboBox.BackColor = SystemColors.MenuBar
                bsWashingSolR1ComboBox.Enabled = False

                bsWashingSolR2ComboBox.SelectedIndex = 0
                bsWashingSolR2ComboBox.BackColor = SystemColors.MenuBar
                bsWashingSolR2ComboBox.Enabled = False
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".ChangeCuvettesSelection ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ChangeCuvettesSelection ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Activate/deactivate controls in the specified Contaminated area
    ''' </summary>
    ''' <param name="pContaminatedArea">Indicates the area that has to be enabled/disabled</param>
    ''' <param name="pStatus">Indicates if the informed area has to be enabled or disabled</param>
    ''' <remarks>
    ''' Created by:  SA 01/12/2010
    ''' </remarks>
    Private Sub ChangeContaminatedAreaStatus(ByVal pContaminatedArea As String, ByVal pStatus As Boolean)
        Try
            Dim myColor As Color = Color.White
            If (Not pStatus) Then myColor = SystemColors.MenuBar

            If (pContaminatedArea = "R1") Then
                bsContaminesAllR1Checkbox.Enabled = pStatus

                PaintGridCells(pStatus, myColor) ' dl 20/07/2011

                'bsR1ContaminatedDataGridView.AlternatingRowsDefaultCellStyle.BackColor = myColor
                'bsR1ContaminatedDataGridView.AlternatingRowsDefaultCellStyle.ForeColor = Color.DarkGray 'Color.Black
                'bsR1ContaminatedDataGridView.AlternatingRowsDefaultCellStyle.SelectionBackColor = myColor
                'bsR1ContaminatedDataGridView.AlternatingRowsDefaultCellStyle.SelectionForeColor = Color.DarkGray 'Color.Black

                'bsR1ContaminatedDataGridView.RowsDefaultCellStyle.BackColor = myColor
                'bsR1ContaminatedDataGridView.RowsDefaultCellStyle.ForeColor = Color.DarkGray 'Color.Black
                'bsR1ContaminatedDataGridView.RowsDefaultCellStyle.SelectionBackColor = myColor
                'bsR1ContaminatedDataGridView.RowsDefaultCellStyle.SelectionForeColor = Color.DarkGray ' Color.Black

                bsR1ContaminatedDataGridView.ReadOnly = (Not pStatus)
                For i As Integer = 0 To bsR1ContaminatedDataGridView.RowCount - 1
                    bsR1ContaminatedDataGridView.Rows(i).DefaultCellStyle.BackColor = myColor
                    bsR1ContaminatedDataGridView.Rows(i).ReadOnly = (Not pStatus)
                    bsR1ContaminatedDataGridView.Rows(i).Cells("Selected").ReadOnly = (Not pStatus)

                    If (CBool(bsR1ContaminatedDataGridView.Rows(i).Cells("Selected").Value)) Then
                        bsR1ContaminatedDataGridView.Rows(i).Cells("WashingSolution").ReadOnly = (Not pStatus)
                        'bsR1ContaminatedDataGridView.Rows(i).Cells("AlreadySaved").Value = "Y"
                    Else
                        bsR1ContaminatedDataGridView.Rows(i).ReadOnly = True
                        bsR1ContaminatedDataGridView.Rows(i).Cells("WashingSolution").ReadOnly = True
                        'bsR1ContaminatedDataGridView.Rows(i).Cells("AlreadySaved").Value = "N"
                    End If
                Next
                'If Not pStatus Then
                '    R1TestsDS.tparContaminations.DefaultView.Sort = "Selected DESC, TestName ASC"
                'Else
                '    R1TestsDS.tparContaminations.DefaultView.Sort = "AlreadySaved DESC, TestName ASC"
                'End If

            ElseIf (pContaminatedArea = "R2") Then
                bsContaminesAllR2Checkbox.Enabled = pStatus

                bsR2ContaminatedDataGridView.AlternatingRowsDefaultCellStyle.BackColor = myColor
                bsR2ContaminatedDataGridView.AlternatingRowsDefaultCellStyle.ForeColor = Color.Black
                bsR2ContaminatedDataGridView.AlternatingRowsDefaultCellStyle.SelectionBackColor = myColor
                bsR2ContaminatedDataGridView.AlternatingRowsDefaultCellStyle.SelectionForeColor = Color.Black

                bsR2ContaminatedDataGridView.RowsDefaultCellStyle.BackColor = myColor
                bsR2ContaminatedDataGridView.RowsDefaultCellStyle.ForeColor = Color.Black
                bsR2ContaminatedDataGridView.RowsDefaultCellStyle.SelectionBackColor = myColor
                bsR2ContaminatedDataGridView.RowsDefaultCellStyle.SelectionForeColor = Color.Black

                bsR2ContaminatedDataGridView.ReadOnly = (Not pStatus)
                For i As Integer = 0 To bsR2ContaminatedDataGridView.RowCount - 1
                    bsR2ContaminatedDataGridView.Rows(i).DefaultCellStyle.BackColor = myColor
                    bsR2ContaminatedDataGridView.Rows(i).ReadOnly = (Not pStatus)
                    bsR2ContaminatedDataGridView.Rows(i).Cells("Selected").ReadOnly = (Not pStatus)

                    If (CBool(bsR2ContaminatedDataGridView.Rows(i).Cells("Selected").Value)) Then
                        bsR2ContaminatedDataGridView.Rows(i).Cells("WashingSolution").ReadOnly = (Not pStatus)
                        'bsR1ContaminatedDataGridView.Rows(i).Cells("AlreadySaved").Value = "Y"
                    Else
                        bsR2ContaminatedDataGridView.Rows(i).Cells("WashingSolution").ReadOnly = True
                        'bsR1ContaminatedDataGridView.Rows(i).Cells("AlreadySaved").Value = "N"
                    End If
                Next

                'If Not pStatus Then
                '    R2TestsDS.tparContaminations.DefaultView.Sort = "Selected DESC, TestName ASC"
                'Else
                '    R2TestsDS.tparContaminations.DefaultView.Sort = "AlreadySaved DESC, TestName ASC"
                'End If

            ElseIf (pContaminatedArea = "CUVETTES") Then
                bsContaminatesCuvettesCheckbox.Enabled = pStatus

                bsWashingSolR1ComboBox.Enabled = (pStatus And bsContaminatesCuvettesCheckbox.Checked)
                If (bsWashingSolR1ComboBox.Enabled) Then
                    bsWashingSolR1ComboBox.BackColor = Color.White
                Else
                    bsWashingSolR1ComboBox.BackColor = SystemColors.MenuBar
                End If

                bsWashingSolR2ComboBox.Enabled = (pStatus And bsContaminatesCuvettesCheckbox.Checked And _
                                                  bsWashingSolR1ComboBox.SelectedIndex > 0)
                If (bsWashingSolR2ComboBox.Enabled) Then
                    bsWashingSolR2ComboBox.BackColor = Color.White
                Else
                    bsWashingSolR2ComboBox.BackColor = SystemColors.MenuBar
                End If
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".ChangeContaminatedAreaStatus ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ChangeContaminatedAreaStatus", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Put the Contaminated Area in Read-Only mode
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 01/12/2010
    ''' </remarks>
    Private Sub ReadOnlyMode()
        Try
            EditionMode = False
            ChangeContaminatedAreaStatus("R1", False)
            ChangeContaminatedAreaStatus("R2", False)
            ChangeContaminatedAreaStatus("CUVETTES", False)
            ForceVisibilityFirstItem()

            bsSaveButton.Enabled = False
            bsCancelButton.Enabled = False

            'AG - Allow edit contaminations only if not exist current worksession
            'If WorkSessionIDAttribute = "" Then
            If Not ExistsExecutions Then 'RH 27/09/2011
                bsEditButton.Enabled = True
                bsSummaryByTestButton.Enabled = True
                bsDeleteButton.Enabled = True
            Else
                bsEditButton.Enabled = False
                bsSummaryByTestButton.Enabled = False
                bsDeleteButton.Enabled = False
            End If
            bsPrintButton.Enabled = True

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".ReadOnlyMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ReadOnlyMode ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>AG 16/12/2010</remarks>
    Private Sub ForceVisibilityFirstItem()
        Try
            If bsR2ContaminatedDataGridView.Visible And bsR2ContaminatedDataGridView.RowCount > 0 Then
                bsR2ContaminatedDataGridView.Rows(0).Selected = True
                bsR2ContaminatedDataGridView.FirstDisplayedScrollingRowIndex = 0

                If Not EditionMode Then
                    R2TestsDS.tparContaminations.DefaultView.Sort = "Selected DESC, TestName ASC"
                Else
                    R2TestsDS.tparContaminations.DefaultView.Sort = "AlreadySaved DESC, TestName ASC"
                End If

            End If

            If bsR1ContaminatedDataGridView.Visible And bsR1ContaminatedDataGridView.RowCount > 0 Then
                bsR1ContaminatedDataGridView.Rows(0).Selected = True
                bsR1ContaminatedDataGridView.FirstDisplayedScrollingRowIndex = 0

                If Not EditionMode Then
                    R1TestsDS.tparContaminations.DefaultView.Sort = "Selected DESC, TestName ASC"
                Else
                    R1TestsDS.tparContaminations.DefaultView.Sort = "AlreadySaved DESC, TestName ASC"
                End If


            End If


        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".ForceVisibilityFirstItem ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ForceVisibilityFirstItem ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)

        End Try
    End Sub

    ''' <summary>
    ''' Put the Contaminated Area in Edition mode
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 01/12/2010
    ''' Modified by RH 28/06/2011 Cells("WashingSolution").ReadOnly.
    ''' </remarks>
    Private Sub EditMode()
        Try
            If Not CurrentUserLevel = "OPERATOR" Then
                'If String.IsNullOrEmpty(WorkSessionIDAttribute) Then
                If Not ExistsExecutions Then 'RH 27/09/2011
                    EditionMode = True
                    ChangeContaminatedAreaStatus("R1", True)
                    ChangeContaminatedAreaStatus("R2", (CInt(bsTestContaminatorsListView.SelectedItems(0).SubItems(1).Text) > 1))
                    ChangeContaminatedAreaStatus("CUVETTES", True)
                    ForceVisibilityFirstItem()

                    bsSummaryByTestButton.Enabled = False
                    bsEditButton.Enabled = False
                    bsDeleteButton.Enabled = False
                    bsPrintButton.Enabled = False

                    bsSaveButton.Enabled = True
                    bsCancelButton.Enabled = True
                End If

                'RH 28/06/2011
                For i As Integer = 0 To bsR1ContaminatedDataGridView.RowCount - 1
                    bsR1ContaminatedDataGridView.Rows(i).Cells("WashingSolution").ReadOnly = _
                            (Not CBool(bsR1ContaminatedDataGridView.Rows(i).Cells("Selected").Value))
                Next
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".EditMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".EditMode ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)

        End Try
    End Sub

    ''' <summary>
    ''' Save all Contaminations defined for the selected Contaminator Test
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 01/12/2010
    ''' </remarks>
    Private Sub SaveTestContaminations()
        Try
            ChangesMade = False

            Using testNewContaminationsDS As New ContaminationsDS()
                Dim myTestID As Integer = CInt(bsTestContaminatorsListView.SelectedItems(0).Name)
                Dim currentTestIsContaminator As Boolean = False
                'Read current contaminations defined for current selected TEST

                Dim resultData As New GlobalDataTO()
                Dim myContaminationsDelegate As New ContaminationsDelegate
                resultData = myContaminationsDelegate.GetTestRNAsContaminator(Nothing, myTestID)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then '(1)
                    Dim myContaminationsDS As New ContaminationsDS()
                    myContaminationsDS = DirectCast(resultData.SetDatos, ContaminationsDS)
                    Dim testReagentsDel As New TestReagentsDelegate()
                    resultData = testReagentsDel.GetTestReagents(Nothing, myTestID)
                    If Not resultData.HasError And Not resultData.SetDatos Is Nothing Then '(2)
                        Dim myTestReagentsDS As New TestReagentsDS()
                        myTestReagentsDS = CType(resultData.SetDatos, TestReagentsDS)
                        'Get the reagent ID (ReagentNumber = 1) for the current Contaminator test
                        Dim myReagentID As Integer = -1
                        myReagentID = (From a As TestReagentsDS.tparTestReagentsRow In myTestReagentsDS.tparTestReagents _
                        Where a.ReagentNumber = 1 _
                        Select a.ReagentID).Max
                        Dim maxReagentNumber As Integer = (From a As TestReagentsDS.tparTestReagentsRow In myTestReagentsDS.tparTestReagents _
                        Select a.ReagentNumber _
                        Distinct).Max
                        'Search for new, updated or deleted R1 contaminations
                        For i As Integer = 0 To bsR1ContaminatedDataGridView.Rows.Count - 1
                            Dim mySelectedValue = CBool(bsR1ContaminatedDataGridView.Rows(i).Cells("Selected").Value)
                            Dim myContaminatedReagentID = CInt(bsR1ContaminatedDataGridView.Rows(i).Cells("ReagentID").Value)
                            Dim mwWashSolution As String = ""
                            If bsR1ContaminatedDataGridView.Rows(i).Cells("WashingSolution").Value.ToString <> "" Then
                                mwWashSolution = CStr(bsR1ContaminatedDataGridView.Rows(i).Cells("WashingSolution").Value)
                            End If
                            If mySelectedValue Then
                                Dim newRow As ContaminationsDS.tparContaminationsRow
                                newRow = testNewContaminationsDS.tparContaminations.NewtparContaminationsRow
                                testNewContaminationsDS.tparContaminations.AddtparContaminationsRow(newRow)
                                With newRow
                                    .BeginEdit()
                                    .ContaminationType = "R1"
                                    .ReagentContaminatorID = myReagentID 'Reagents contaminated reagents
                                    .ReagentContaminatedID = myContaminatedReagentID
                                    .SetTestContaminaCuvetteIDNull()
                                    .SetWashingSolutionR1Null()
                                    If mwWashSolution <> "" Then
                                        .WashingSolutionR1 = mwWashSolution
                                    End If
                                    .SetWashingSolutionR2Null()
                                    .AcceptChanges()
                                End With
                                currentTestIsContaminator = True
                            End If
                        Next 'For i As Integer = 0 To bsR1ContaminatedDataGridView.Rows.Count - 1
                        Dim mypContaminationsDelegate As New ContaminationsDelegate
                        resultData = mypContaminationsDelegate.SaveContaminations(Nothing, "R1", testNewContaminationsDS, myReagentID)
                        If (resultData.HasError) Then '(3)
                            ShowMessage(Name & ".SaveTestContaminations", resultData.ErrorCode, resultData.ErrorMessage, Me)
                        Else
                            testNewContaminationsDS.Clear()
                            'Search for new, updated or deleted R2 contaminations - PHASE-2
                            'In the same way as R1
                            'Search for new, updated or deleted CUVETTES contaminations
                            If bsContaminatesCuvettesCheckbox.CheckState = CheckState.Checked Then '(4)
                                Dim newRow As ContaminationsDS.tparContaminationsRow
                                newRow = testNewContaminationsDS.tparContaminations.NewtparContaminationsRow
                                testNewContaminationsDS.tparContaminations.AddtparContaminationsRow(newRow)
                                With newRow
                                    .BeginEdit()
                                    .ContaminationType = "CUVETTES"
                                    '.ReagentContaminatorID = myReagentID 'Reagent that contaminates cuvette
                                    .SetReagentContaminatedIDNull()
                                    .TestContaminaCuvetteID = myTestID 'myReagentID 'Test contaminated cuvettes
                                    .SetWashingSolutionR1Null()
                                    If bsWashingSolR1ComboBox.SelectedValue.ToString.Trim <> "" Then
                                        .WashingSolutionR1 = bsWashingSolR1ComboBox.SelectedValue.ToString
                                    End If
                                    .SetWashingSolutionR2Null()
                                    If bsWashingSolR2ComboBox.SelectedValue.ToString.Trim <> "" Then
                                        .WashingSolutionR2 = bsWashingSolR2ComboBox.SelectedValue.ToString
                                    End If
                                    .AcceptChanges()
                                End With
                                currentTestIsContaminator = True
                            End If '(4)
                            Dim pContaminationsDelegate As New ContaminationsDelegate
                            resultData = pContaminationsDelegate.SaveContaminations(Nothing, "CUVETTES", testNewContaminationsDS, myTestID)
                            If (resultData.HasError) Then '(4)
                                ShowMessage(Name & ".SaveTestContaminations", resultData.ErrorCode, resultData.ErrorMessage, Me)
                            Else
                                If currentTestIsContaminator Then
                                    bsTestContaminatorsListView.Items(originalSelectedIndex).ForeColor = Color.Red
                                Else
                                    bsTestContaminatorsListView.Items(originalSelectedIndex).ForeColor = Color.Black
                                End If
                            End If '(4)
                        End If '(3)
                    End If '(2)
                End If
                '(1)
                If (Not resultData.HasError) Then
                    ReadOnlyMode()
                    LoadContaminationsByTest(CInt(bsTestContaminatorsListView.SelectedItems(0).Name))
                Else
                    'Error saving the list of Contaminations defined for the selected Test; shown it
                    ShowMessage(Name & ".SaveTestContaminations", resultData.ErrorCode, resultData.ErrorMessage, Me)
                End If
            End Using
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".SaveTestContaminations ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".SaveTestContaminations", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 01/12/2010
    ''' </remarks>
    Private Sub CancelEdition()
        Try
            Dim executeAction As Boolean = True
            If (ChangesPendingToSave()) Then
                If (ShowMessage(Name, GlobalEnumerates.Messages.DISCARD_PENDING_CHANGES.ToString) = Windows.Forms.DialogResult.No) Then
                    executeAction = False
                End If
            End If

            If (executeAction) Then
                ReadOnlyMode()
                If bsTestContaminatorsListView.SelectedItems.Count = 1 Then LoadContaminationsByTest(CInt(bsTestContaminatorsListView.SelectedItems(0).Name))
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".CancelEdition ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".CancelEdition", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 01/12/2010
    ''' </remarks>
    Private Sub ExitScreen()
        Try
            Dim executeAction As Boolean = True
            If (ChangesPendingToSave()) Then
                If (ShowMessage(Name, GlobalEnumerates.Messages.DISCARD_PENDING_CHANGES.ToString) = Windows.Forms.DialogResult.No) Then
                    executeAction = False
                End If
            End If

            If (executeAction) Then
                'TR 11/04/2012 -Disable form on close to avoid any button press.
                Me.Enabled = False

                'RH 17/12/2010
                If Not Tag Is Nothing Then
                    'A PerformClick() method was executed
                    Close()
                Else
                    'Normal button click
                    'Open the WS Monitor form and close this one
                    UiAx00MainMDI.OpenMonitorForm(Me)
                End If
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".ExitScreen ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ExitScreen", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Verify if there are changes that have not been still saved 
    ''' </summary>
    ''' <returns>True if there are changes pending to save</returns>
    ''' <remarks>
    ''' Created by:  SA 01/12/2010
    ''' </remarks>
    Private Function ChangesPendingToSave() As Boolean
        Dim unSavedChanges As Boolean = False
        Try
            unSavedChanges = ChangesMade
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".ChangesPendingToSave ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ChangesPendingToSave", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
        Return unSavedChanges
    End Function

    ''' <summary>
    ''' Delete all Contaminations defined for the selected Contaminator Test
    ''' </summary>
    ''' <param name="pTestID">Test Identifier</param>
    ''' <remarks>
    ''' Created by:  SA 01/12/2010
    ''' </remarks>
    Private Function DeleteAllContaminationsByTest(ByVal pTestID As Integer, ByVal SelectedIndex As Integer) As Boolean
        Dim hasError As Boolean = False
        Try
            'If (ShowMessage(Me.Name, GlobalEnumerates.Messages.DELETE_CONTAMINATION.ToString) = Windows.Forms.DialogResult.Yes) Then
            Dim resultData As New GlobalDataTO
            Dim myContaminationsDelegate As New ContaminationsDelegate

            resultData = myContaminationsDelegate.DeleteAllByTest(Nothing, pTestID)
            If (Not resultData.HasError) Then
                bsTestContaminatorsListView.SelectedItems(SelectedIndex).ForeColor = Color.Black
                ReadOnlyMode()
                If bsTestContaminatorsListView.SelectedItems.Count = 1 Then LoadContaminationsByTest(pTestID)
            Else
                'Error deleting all Contaminations defined for the selected Test; shown it
                ShowMessage(Name & ".DeleteAllContaminationsByTest", resultData.ErrorCode, resultData.ErrorMessage, Me)
                hasError = True
            End If
            'End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".DeleteAllContaminationsByTest ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".DeleteAllContaminationsByTest", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
        Return hasError
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>AG 16/12/2010</remarks>
    Private Sub SelectItemInTreeView()
        Try
            Dim executeAction As Boolean = True
            If EditionMode And ChangesPendingToSave() Then
                If (ShowMessage(Name, GlobalEnumerates.Messages.DISCARD_PENDING_CHANGES.ToString) = Windows.Forms.DialogResult.No) Then
                    executeAction = False
                End If
            End If

            If (executeAction) Then
                ChangesMade = False
                If bsTestContaminatorsListView.SelectedItems.Count = 1 Then

                    If bsR1ContaminatedDataGridView.DataSource Is Nothing Then
                        InitializeGrids()
                    End If
                    'If Not bsR1ContaminatedDataGridView.Visible Then 'If the last selection was multiple you must re-load grids
                    '    bsR1ContaminatedDataGridView.Visible = True
                    '    bsR2ContaminatedDataGridView.Visible = True
                    'End If

                    If bsEditButton.Enabled = False Then
                        bsEditButton.Enabled = True
                        bsSummaryByTestButton.Enabled = True
                    End If

                    originalSelectedIndex = bsTestContaminatorsListView.SelectedIndices(0)
                    ReadOnlyMode()
                    LoadContaminationsByTest(CInt(bsTestContaminatorsListView.SelectedItems(0).Name))

                Else 'Multi selection
                    CancelEdition()
                    SetEmptyValues()

                    originalSelectedIndex = -1 'No item selected
                    bsEditButton.Enabled = False 'Allow only delete or print
                    bsSummaryByTestButton.Enabled = False 'Temporal
                End If

            Else
                If (originalSelectedIndex <> -1) Then
                    bsTestContaminatorsListView.SelectedItems.Clear() 'Clear selection
                    bsTestContaminatorsListView.Items(originalSelectedIndex).Selected = True
                    bsTestContaminatorsListView.Select()
                End If

            End If

            ScreenStatusByUserLevel() 'TR 23/04/2012

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".SelectItemInTreeView ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".SelectItemInTreeView", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>AG 16/12/2010</remarks>
    Private Sub DeleteContaminations()
        Try
            If (ShowMessage(Name, GlobalEnumerates.Messages.DELETE_CONTAMINATION.ToString) = Windows.Forms.DialogResult.Yes) Then
                Dim hasError As Boolean = False
                For i As Integer = 0 To bsTestContaminatorsListView.SelectedItems.Count - 1
                    hasError = DeleteAllContaminationsByTest(CInt(bsTestContaminatorsListView.SelectedItems(i).Name), i)
                    If hasError Then Exit For
                Next
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".DeleteContaminations ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".DeleteContaminations", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>AG 16/12/2010</remarks>
    Private Sub SetEmptyValues()
        Try
            bsContaminesAllR1Checkbox.Checked = False
            bsR1ContaminatedDataGridView.DataSource = Nothing
            bsR1ContaminatedDataGridView.DataMember = Nothing
            'bsR1ContaminatedDataGridView.Visible = False

            bsContaminesAllR2Checkbox.Checked = False
            bsR2ContaminatedDataGridView.DataSource = Nothing
            bsR2ContaminatedDataGridView.DataMember = Nothing
            'bsR2ContaminatedDataGridView.Visible = False

            bsContaminatesCuvettesCheckbox.Checked = False

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".SetEmptyValues ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".SetEmptyValues", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Validate if calibrator values is factory value.
    ''' </summary>
    ''' <param name="pTestID">Test ID.</param>
    ''' <returns></returns>
    ''' <remarks>
    ''' CREATED BY: TR 08/03/2011
    ''' </remarks>
    Private Function ValidateCalibratorFactoryValues(ByVal pTestID As Integer) As Boolean
        Dim hasFactoryValue As Boolean = False
        Try
            Dim myGlobalDataTO As New GlobalDataTO
            Dim myTestSampleDelegate As New TestSamplesDelegate
            Dim myTestSampleDS As New TestSamplesDS

            'Get all the test Sample 
            myGlobalDataTO = myTestSampleDelegate.GetSampleDataByTestID(Nothing, pTestID)
            If Not myGlobalDataTO.HasError Then
                myTestSampleDS = DirectCast(myGlobalDataTO.SetDatos, TestSamplesDS)

                If myTestSampleDS.tparTestSamples.Count > 0 Then
                    Dim qTestSampleList As New List(Of TestSamplesDS.tparTestSamplesRow)
                    qTestSampleList = (From a In myTestSampleDS.tparTestSamples _
                                       Where a.FactoryCalib = True Select a).ToList()
                    If qTestSampleList.Count > 0 Then
                        hasFactoryValue = True
                    End If
                End If
            End If

        Catch ex As Exception
            'Write error SYSTEM_ERROR in the Application Log
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " ValidateCalibratorFactoryValues ", EventLogEntryType.Error, _
                                                            GetApplicationInfoSession().ActivateSystemLog)
            'Show error message
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
        Return hasFactoryValue
    End Function

    ''' <summary>
    ''' paint cells 
    ''' </summary>
    ''' <remarks>
    ''' Created by: DL 20/07/2011
    ''' </remarks>
    Private Sub PaintGridCells(ByVal pEdit As Boolean, ByVal pColor As Color)
        Try
            Dim dgvCbo As DataGridViewComboBoxColumn = TryCast(bsR1ContaminatedDataGridView.Columns("WashingSolution"), DataGridViewComboBoxColumn)
            Dim myColor As Color

            If pEdit Then
                'AG 20/10/2011 - avoid system error when multiple selection
                'dgvCbo.DisplayStyle = DataGridViewComboBoxDisplayStyle.DropDownButton
                If Not dgvCbo Is Nothing Then dgvCbo.DisplayStyle = DataGridViewComboBoxDisplayStyle.DropDownButton
                myColor = Color.Black
            Else
                'AG 20/10/2011 - avoid system error when multiple selection
                'dgvCbo.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
                If Not dgvCbo Is Nothing Then dgvCbo.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
                myColor = Color.DarkGray
            End If

            With bsR1ContaminatedDataGridView
                .AlternatingRowsDefaultCellStyle.BackColor = pColor
                .AlternatingRowsDefaultCellStyle.ForeColor = myColor
                .AlternatingRowsDefaultCellStyle.SelectionBackColor = pColor
                .AlternatingRowsDefaultCellStyle.SelectionForeColor = myColor

                .RowsDefaultCellStyle.BackColor = pColor
                .RowsDefaultCellStyle.ForeColor = myColor
                .RowsDefaultCellStyle.SelectionBackColor = pColor
                .RowsDefaultCellStyle.SelectionForeColor = myColor
            End With

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".PaintGridCells ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".PaintGridCells", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try

    End Sub

    ''' <summary>
    ''' Enable or disable functionallity by user level.
    ''' </summary>
    ''' <remarks>
    ''' CREATED BY: TR 23/04/2012
    ''' Modified by XB 04/02/2013 - Upper conversions redundants because the value is already in UpperCase must delete to avoid Regional Settings problems (Bugs tracking #1112)
    ''' </remarks>
    Private Sub ScreenStatusByUserLevel()
        Try
            Select Case CurrentUserLevel    '.ToUpper()
                Case "SUPERVISOR"
                    Exit Select

                Case "OPERATOR"
                    bsEditButton.Enabled = False
                    bsDeleteButton.Enabled = False
                    bsPrintButton.Enabled = False
                    Exit Select
            End Select

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & "ScreenStatusByUserLevel ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & "ScreenStatusByUserLevel ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub


    ''' <summary>
    ''' Release elements not handle by the GC.
    ''' </summary>
    ''' <remarks>CREATED BY: TR 02/08/2012
    ''' AG 10/02/2014 - #1496 Mark screen closing when ReleaseElements is called
    ''' </remarks>
    Private Sub ReleaseElements()

        Try
            '--- Detach variable defined using WithEvents ---
            isClosingFlag = True 'AG 10/02/2014 - #1496 Mark screen closing when ReleaseElement is called
            R1TestsDS = Nothing
            R2TestsDS = Nothing
            bsTestListGroupBox = Nothing
            bsContaminatorsListLabel = Nothing
            bsTestContaminatorsListView = Nothing
            bsPrintButton = Nothing
            bsEditButton = Nothing
            bsExitButton = Nothing
            bsContaminatedDetailsGroupBox = Nothing
            bsSaveButton = Nothing
            bsCancelButton = Nothing
            bsContaminesAllR1Checkbox = Nothing
            bsR1GroupBox = Nothing
            bsContaminationsLabel = Nothing
            bsR2GroupBox = Nothing
            bsContaminesAllR2Checkbox = Nothing
            bsCuvettesGroupBox = Nothing
            bsContaminatesCuvettesCheckbox = Nothing
            bsWashingSolR2ComboBox = Nothing
            bsWashingSolR2Label = Nothing
            bsWashingSolR1ComboBox = Nothing
            bsWashingSolR1Label = Nothing
            bsR2ContaminatedDataGridView = Nothing
            bsR1ContaminatedDataGridView = Nothing
            bsSummaryByTestButton = Nothing
            bsScreenToolTips = Nothing
            bsDeleteButton = Nothing
            '----------------------------------------------
            'GC.Collect()
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ReleaseElements ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ReleaseElements ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try

    End Sub

#End Region

#Region "Events"
    ''' <summary>
    ''' Screen loading
    ''' </summary>
    Private Sub ProgTestContaminations_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            ''TR 23/04/2012 get the current user level
            ''Dim myGlobalbase As New GlobalBase
            'CurrentUserLevel = GlobalBase.GetSessionInfo().UserLevel
            ''TR 23/04/2012 -END.

            ScreenLoad()
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".ProgTestContaminations_Load ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ProgTestContaminations_Load ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' When the screen ESC key is pressed, the screen is closed
    ''' </summary>
    Private Sub ProgTestContaminations_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyDown
        Try
            If (e.KeyCode = Keys.Escape) Then
                If (bsCancelButton.Enabled) Then
                    bsCancelButton.PerformClick()
                Else
                    bsExitButton.PerformClick()
                End If
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".ProgTestContaminations_KeyDown ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ProgTestContaminations_KeyDown", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub bsEditButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles bsEditButton.Click
        EditMode()
    End Sub

    Private Sub bsDeleteButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsDeleteButton.Click
        DeleteContaminations()
    End Sub

    Private Sub bsSaveButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles bsSaveButton.Click
        SaveTestContaminations()
    End Sub

    Private Sub bsCancelButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles bsCancelButton.Click
        CancelEdition()
    End Sub

    Private Sub bsExitButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles bsExitButton.Click
        ExitScreen()
    End Sub

    Private Sub bsTestContaminatorsListView_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles bsTestContaminatorsListView.Click
        Try
            SelectItemInTreeView()

            'TR 09/03/2011 - Validation of Calibrators Factory Values
            If (bsTestContaminatorsListView.SelectedItems.Count = 1) Then
                If (PrevSelectedTest <> bsTestContaminatorsListView.SelectedItems(0).Text) Then
                    PrevSelectedTest = bsTestContaminatorsListView.SelectedItems(0).Text
                    If (ValidateCalibratorFactoryValues(CInt(bsTestContaminatorsListView.SelectedItems(0).Name))) Then ShowMessage("Warning", GlobalEnumerates.Messages.FACTORY_VALUES.ToString)
                End If
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".bsTestContaminatorsListView_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsTestContaminatorsListView_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub bsTestContaminatorsListView_DoubleClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles bsTestContaminatorsListView.DoubleClick
        bsEditButton_Click(Nothing, Nothing)
    End Sub

    ''' <summary>
    ''' Don't allow Users to resize the visible ListView column
    ''' </summary>
    Private Sub bsTestContaminatorsListView_ColumnWidthChanging(ByVal sender As Object, ByVal e As System.Windows.Forms.ColumnWidthChangingEventArgs) Handles bsTestContaminatorsListView.ColumnWidthChanging
        Try
            e.Cancel = True
            If (e.ColumnIndex = 0) Then
                e.NewWidth = 210
            Else
                e.NewWidth = 0
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".bsTestContaminatorsListView_ColumnWidthChanging", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsTestContaminatorsListView_ColumnWidthChanging", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Allow consultation of elements in Contaminator Tests list using the keyboard
    ''' </summary>
    Private Sub bsTestContaminatorsListView_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles bsTestContaminatorsListView.KeyUp
        Try
            SelectItemInTreeView()

            'TR 09/03/2011 - Validation of Calibrators Factory Values
            If (bsTestContaminatorsListView.SelectedItems.Count = 1) Then
                If (PrevSelectedTest <> bsTestContaminatorsListView.SelectedItems(0).Text) Then
                    PrevSelectedTest = bsTestContaminatorsListView.SelectedItems(0).Text
                    If (ValidateCalibratorFactoryValues(CInt(bsTestContaminatorsListView.SelectedItems(0).Name))) Then ShowMessage("Warning", GlobalEnumerates.Messages.FACTORY_VALUES.ToString)
                End If
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".bsTestContaminatorsListView_KeyUp ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsTestContaminatorsListView_KeyUp", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub bsContaminesAllR1Checkbox_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles bsContaminesAllR1Checkbox.Click
        Try
            TestContaminatesEverything("R1", bsContaminesAllR1Checkbox.Checked)
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".bsContaminesAllR1Checkbox_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsContaminesAllR1Checkbox_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub bsContaminesAllR2Checkbox_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles bsContaminesAllR2Checkbox.Click
        Try
            TestContaminatesEverything("R2", bsContaminesAllR2Checkbox.Checked)
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".bsContaminesAllR2Checkbox_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsContaminesAllR2Checkbox_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub bsContaminatesCuvettesCheckbox_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles bsContaminatesCuvettesCheckbox.Click
        Try
            ChangeCuvettesSelection()
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".bsContaminatesCuvettesCheckbox_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsContaminatesCuvettesCheckbox_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub bsWashingSolR1ComboBox_SelectionChangeCommitted(ByVal sender As Object, ByVal e As System.EventArgs) Handles bsWashingSolR1ComboBox.SelectionChangeCommitted
        Try
            If EditionMode Then ChangesMade = True
            If (bsWashingSolR1ComboBox.SelectedIndex > 0) Then
                If EditionMode Then bsWashingSolR2ComboBox.Enabled = True
                bsWashingSolR2ComboBox.BackColor = Color.White
            Else
                bsWashingSolR2ComboBox.SelectedIndex = 0
                bsWashingSolR2ComboBox.Enabled = False
                bsWashingSolR2ComboBox.BackColor = SystemColors.MenuBar
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".bsWashingSolR1ComboBox_SelectionChangeCommitted ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsWashingSolR1ComboBox_SelectionChangeCommitted", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub bsR1ContaminatedDataGridView_CellMouseClick(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellMouseEventArgs) Handles bsR1ContaminatedDataGridView.CellMouseClick, bsR2ContaminatedDataGridView.CellMouseClick
        Try
            If sender Is Nothing Then Return
            Dim dgv As BSDataGridView = CType(sender, BSDataGridView)

            Dim myColumn As Integer = e.ColumnIndex
            Dim myRow As Integer = e.RowIndex

            If myRow < 0 Then
                'Header click
            Else
                If Not EditionMode Then Return

                If dgv.Columns(myColumn).Name = "Selected" Then
                    dgv.Rows(myRow).Cells("Selected").Value = Not CBool(dgv.Rows(myRow).Cells("Selected").Value)
                    dgv.Rows(myRow).Cells("WashingSolution").ReadOnly = Not CBool(dgv.Rows(myRow).Cells("Selected").Value)
                    ChangesMade = True
                    If Not CBool(dgv.Rows(myRow).Cells("Selected").Value) Then
                        dgv.Rows(myRow).Cells("WashingSolution").Value = ""
                    End If

                ElseIf dgv.Columns(myColumn).Name = "WashingSolution" Then
                    If CBool(dgv.Rows(myRow).Cells("Selected").Value) Then
                        ChangesMade = True
                    Else
                        dgv.Rows(myRow).Cells("WashingSolution").Value = ""
                        dgv.Rows(myRow).Cells("WashingSolution").ReadOnly = True
                    End If

                End If

            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".bsR1ContaminatedDataGridView_CellMouseClick ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsR1ContaminatedDataGridView_CellMouseClick", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)

        End Try
    End Sub

    Private Sub bsWashingSolR2ComboBox_SelectionChangeCommitted(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsWashingSolR2ComboBox.SelectionChangeCommitted
        Try
            If EditionMode Then ChangesMade = True
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".bsWashingSolR2ComboBox_SelectionChangeCommitted ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsWashingSolR2ComboBox_SelectionChangeCommitted", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub bsTestContaminatorsListView_PreviewKeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.PreviewKeyDownEventArgs) Handles bsTestContaminatorsListView.PreviewKeyDown
        Try
            If bsTestContaminatorsListView.SelectedItems.Count > 0 Then
                If e.KeyCode = Keys.Delete And Not EditionMode Then
                    If bsDeleteButton.Enabled = True Then DeleteContaminations()
                ElseIf e.KeyCode = Keys.Enter And Not EditionMode Then
                    If bsTestContaminatorsListView.SelectedItems.Count = 1 Then EditMode()
                End If
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".bsTestContaminatorsListView_PreviewKeyDown ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsTestContaminatorsListView_PreviewKeyDown", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub bsSummaryByTestButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsSummaryByTestButton.Click
        Using myProgAuxTestContaminations As New UiProgAuxTestContaminations
            'myProgAuxTestContaminations.FormBorderStyle = Windows.Forms.FormBorderStyle.FixedToolWindow

            'RH 20/06/2012 This is the right value
            'It is the used througout the application.
            'It enables the aplication to show it's icon when the user presses Alt + Tab. The other one not.
            myProgAuxTestContaminations.FormBorderStyle = FormBorderStyle.FixedDialog

            myProgAuxTestContaminations.ShowDialog()
        End Using
    End Sub

    Private Sub ProgTestContaminations_Shown(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Shown
        ''TR 09/03/2011 -Validate FactoryValue
        If bsTestContaminatorsListView.SelectedItems.Count > 0 Then
            If PrevSelectedTest <> bsTestContaminatorsListView.SelectedItems(0).Text Then
                PrevSelectedTest = bsTestContaminatorsListView.SelectedItems(0).Text
                If ValidateCalibratorFactoryValues(CInt(bsTestContaminatorsListView.SelectedItems(0).Name)) Then
                    ShowMessage("Warning", GlobalEnumerates.Messages.FACTORY_VALUES.ToString())
                End If
            End If
        End If
        ''TR 09/03/2011 -END.

        ScreenStatusByUserLevel() 'TR 23/04/2012

    End Sub

    ''' <summary>
    ''' Prints the Contaminations Report
    ''' </summary>
    ''' <remarks>
    ''' Created by: RH 21/12/2011
    ''' </remarks>
    Private Sub bsPrintButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsPrintButton.Click
        Try
            XRManager.ShowContaminationsReport()

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".bsPrintButton_Click", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsPrintButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

#End Region

End Class
