Option Explicit On
Option Strict On
Option Infer On

Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Global.TO
Imports Biosystems.Ax00.Controls.UserControls
Imports Biosystems.Ax00.PresentationCOM

Public Class UiProgCalibrator

#Region "Attributes"
    Private AnalyzerIDAttribute As String
    Private WorkSessionIDAttribute As String

    'Attributes to manage information received/to send to Tests Programming Screen 
    Private IsTestParamWinAttribute As Boolean = False
    Private ExternalTestIDAttribute As Integer
    Private ExternalSampleTypeAttribute As String
    Private ExternalTestNameAttribute As String
    Private ExternalDecimalsAllowAttribute As Integer
    Private UpdatedTestDS As New TestsDS
    Private UpdatedCalibratorsDS As New CalibratorsDS
    Private UpdatedTestCalibratorsDS As New TestCalibratorsDS
    Private UpdatedTestCalibratorsValuesDS As New TestCalibratorValuesDS
    Private LocalChangesToSendAttribute As Boolean = False
#End Region

#Region "Properties"
    ''' <summary>
    ''' Identifier of the current connected Analyzer
    ''' </summary>
    Public WriteOnly Property AnalyzerID() As String
        Set(ByVal value As String)
            AnalyzerIDAttribute = value
        End Set
    End Property

    ''' <summary>
    ''' Identifier of the active WorkSession
    ''' </summary>
    Public WriteOnly Property WorkSessionID() As String
        Set(ByVal value As String)
            WorkSessionIDAttribute = value
        End Set
    End Property

    ''' <summary>
    ''' When TRUE, it indicates the screen has been opened from Tests Programming Screen
    ''' </summary>
    Public WriteOnly Property IsTestParammeterWindows() As Boolean
        Set(ByVal value As Boolean)
            IsTestParamWinAttribute = value
        End Set
    End Property

    ''' <summary>
    ''' When the screen has been opened from Tests Programming Screen, it contains the Identifier of the Test beign edited
    ''' </summary>
    Public WriteOnly Property TestIdentification() As Integer
        Set(ByVal value As Integer)
            ExternalTestIDAttribute = value
        End Set
    End Property

    ''' <summary>
    ''' When the screen has been opened from Tests Programming Screen, it contains the Sample Type of the Test beign edited
    ''' </summary>
    Public WriteOnly Property TestSampleType() As String
        Set(ByVal value As String)
            ExternalSampleTypeAttribute = value
        End Set
    End Property

    ''' <summary>
    ''' When the screen has been opened from Tests Programming Screen, it contains the Name of the Test beign edited
    ''' </summary>
    Public WriteOnly Property TestName() As String
        Set(ByVal value As String)
            ExternalTestNameAttribute = value
        End Set
    End Property

    ''' <summary>
    ''' When the screen has been opened from Tests Programming Screen, it contains the Number of Decimals Allowed for the Test beign edited
    ''' </summary>
    Public WriteOnly Property DecimalsAllow() As Integer
        Set(ByVal value As Integer)
            ExternalDecimalsAllowAttribute = value
        End Set
    End Property

    ''' <summary>
    ''' When the screen has been opened from Tests Programming Screen, it contains the data of the Calibrator selected
    ''' for the Test/SampleType beign edited
    ''' </summary>
    Public Property ResultCalibratorsDS() As CalibratorsDS
        Get
            Return UpdatedCalibratorsDS
        End Get
        Set(ByVal value As CalibratorsDS)
            UpdatedCalibratorsDS = value
        End Set
    End Property

    ''' <summary>
    ''' When the screen has been opened from Tests Programming Screen, it contains the data of the relation between a Calibrator 
    ''' and the Test/SampleType beign edited (Curve Calibration Values)
    ''' </summary>
    Public Property ResultTestCalibrator() As TestCalibratorsDS
        Get
            Return UpdatedTestCalibratorsDS
        End Get
        Set(ByVal value As TestCalibratorsDS)
            UpdatedTestCalibratorsDS = value
        End Set
    End Property

    ''' <summary>
    ''' When the screen has been opened from Tests Programming Screen, it contains the Theoretical Concentration Calibration  
    ''' values for the Test/SampleType beign edited 
    ''' </summary>
    Public Property ResultTestCalibratorsValue() As TestCalibratorValuesDS
        Get
            Return UpdatedTestCalibratorsValuesDS
        End Get
        Set(ByVal value As TestCalibratorValuesDS)
            UpdatedTestCalibratorsValuesDS = value
        End Set
    End Property

    ''' <summary>
    ''' Indicate if there are changes on the Calibrator screen to send to Tests Programming Screen
    ''' </summary>
    Public ReadOnly Property ChangesMade() As Boolean
        Get
            Return LocalChangesToSendAttribute
        End Get
    End Property
#End Region

#Region "Enumerates"
    Public Enum SCREEN_STATUS_TYPE
        ReadMode
        EditMode
        CreateMode
        MultiSelection
        DisableAllButtons
        ElementInUse
        SpecialCalibrator
    End Enum

    Public Enum CALIBRATOR_ACTION
        Create
        Edit
        Delete
    End Enum
#End Region

#Region "Declarations"
    Private LocalTestDS As TestsDS                                 'DataSource for Calibrator by Test/SampleType Grid
    Private LocalCalibratorDS As CalibratorsDS                     'DataSource for Calibrators Grid
    Private LocalConcentrationValuesDS As TestCalibratorValuesDS   'DataSource for Theoretical Concentration Values Grid

    Private LocalScreenStatus As SCREEN_STATUS_TYPE
    Private LocalChange As Boolean
    Private LocalCurLanguage As String = ""
    Private LocalValidateDependencies As Boolean
    Private LocalCalibratorLotModified As Boolean
    Private LocalDecimalAllow As Integer
    Private LocalConcPrevValue As Double = -1
    Private LocalFirstPointValue As Double = -1
    Private currentLanguage As String

    'TR 09/03/2011 - Used to indicate the previous selected Test
    Private PrevSelectedTest As String = ""

#End Region

#Region "Contructor"
    Public Sub New()
        'This call is required by the Windows Form Designer.
        InitializeComponent()

        'Add any initialization after the InitializeComponent() call.
        LocalChange = False
        LocalValidateDependencies = False
        LocalCalibratorLotModified = False

        LocalConcPrevValue = -1
        LocalFirstPointValue = -1
    End Sub
#End Region

#Region "COMMON METHODS"

    ''' <summary>
    ''' Screen initialization
    ''' </summary>
    Private Sub InitializeScreen()
        Try
            'Get the current Language from the current Application Session
            'Dim myGlobalbase As New GlobalBase
            currentLanguage = GlobalBase.GetSessionInfo().ApplicationLanguage.Trim.ToString

            'LocalCurLanguage = currentLanguage

            'Get all screen labels in the active Language
            GetScreenLabels(currentLanguage)

            'Get the Icon and ToolTip in the active Language for all graphical buttons
            PrepareButtons(currentLanguage)

            'Initialize and fill the grid of Calibrators
            PrepareCalibratorListGrid(currentLanguage)
            FillCalibratorList()

            'Initialize and fill the grid of Calibrator by Tests/SampleTypes
            PrepareCalibratorTestSampleListGrid(currentLanguage)
            FillCalibratorTestSampleList(False)

            'Fill ComboBoxes for Calibrators and Calibration Curve Values
            FillCalibratorListCombo()
            FillCurveControls()

            'Initialize the grid of Theoretical Concentration Values 
            PrepareConcentrationGridView(currentLanguage)
            Application.DoEvents()

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".InitializeScreen ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".InitializeScreen ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Get texts in the current application language for all screen controls
    ''' </summary>
    ''' <param name="pLanguageID"> The current Language of Application</param>
    ''' <remarks>
    ''' Created by:  PG 08/10/2010
    ''' Modified by: TR 27/07/2011 - Changes to get resources for new screen controls
    '''              JB 01/10/2012 - Changes for Resource string unification
    ''' </remarks>
    Private Sub GetScreenLabels(ByVal pLanguageID As String)
        Try
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            'Label for the screen title
            bsCalibratorProgrammingLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Calibrator_Prog", pLanguageID)

            'Labels for Calibrators Tab
            CalibratorInfoTab.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Calibrators", pLanguageID)
            CalibratorListLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Calibrators", pLanguageID)
            CalibratorDefinitionLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Calibrator_Def", pLanguageID)
            CalibratorNameLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_CalibratorName", pLanguageID) + ":"
            LotNumberLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Lot", pLanguageID) + ":"
            ExpirationDateLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_ExpDate_Full", pLanguageID) + ":"
            NumberLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Number_Short", pLanguageID) + ":"

            'Labels for Calibrator by Test/SampleType Tab
            CalibratorTestTab.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Calib_TestSample", pLanguageID)
            TestSampleListLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_TestsSample_Types", pLanguageID)
            CalibratorLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Calibrators", pLanguageID) + ":"
            CurveValueLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Curve_Values", pLanguageID)
            CalibCurveInfoGroupBox.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_CalibrationCurve", pLanguageID)
            IncreasingRadioButton.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Increasing", pLanguageID)
            DecreasingRadioButton.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Decreasing", pLanguageID)
            XaxisLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_AxisX", pLanguageID) + ":"
            YaxisLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_AxisY", pLanguageID) + ":"
            CalibrationValuesCurveLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Concentration_Values", pLanguageID)
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".GetScreenLabels", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".GetScreenLabels", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Get icon and multilanguage text of the tooltip for all graphical buttons
    ''' </summary>
    ''' <param name="pLanguageID">Identifier of the current Language</param>
    ''' <remarks>
    ''' Created by:  TR 11/02/2011
    ''' </remarks>
    Private Sub PrepareButtons(ByVal pLanguageID As String)
        Try
            Dim auxIconName As String = ""
            Dim iconPath As String = MyBase.IconsPath

            Dim myToolTipsControl As New ToolTip
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            'NEW Calibrator
            auxIconName = GetIconName("ADD")
            If (auxIconName <> "") Then
                AddCalibButon.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
                myToolTipsControl.SetToolTip(AddCalibButon, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_AddNew", pLanguageID))
            End If

            'EDIT Calibrator and EDIT Calibrator Values
            auxIconName = GetIconName("EDIT")
            If (String.Compare(auxIconName, "", False) <> 0) Then
                EditCalibButton.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
                myToolTipsControl.SetToolTip(EditCalibButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Edit", pLanguageID))

                EditCalTestSampleButton.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
                myToolTipsControl.SetToolTip(EditCalTestSampleButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Edit", pLanguageID))
            End If

            'DELETE Calibrators
            auxIconName = GetIconName("REMOVE")
            If (auxIconName <> "") Then
                DeleteCalbButton.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
                myToolTipsControl.SetToolTip(DeleteCalbButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Delete", pLanguageID))
            End If

            'PRINT Calibrators Report
            auxIconName = GetIconName("PRINT")
            If (auxIconName <> "") Then
                PrintButton.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
                myToolTipsControl.SetToolTip(PrintButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Print", pLanguageID))
            End If

            'SAVE Changes (Calibrators and SAVE Calibrator Values)
            auxIconName = GetIconName("SAVE")
            If (String.Compare(auxIconName, "", False) <> 0) Then
                SaveCalibButton.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
                myToolTipsControl.SetToolTip(SaveCalibButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Calibs_SaveCalib", pLanguageID))

                SaveTestSampleCalValue.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
                myToolTipsControl.SetToolTip(SaveTestSampleCalValue, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Calibs_SaveCurve", pLanguageID))
            End If

            'CANCEL Changes (Calibrators and SAVE Calibrator Values)
            auxIconName = GetIconName("UNDO")
            If (String.Compare(auxIconName, String.Empty, False) <> 0) Then
                CancelCalibButton.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
                myToolTipsControl.SetToolTip(CancelCalibButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Cancel", pLanguageID))

                CancelTestSampleCalValue.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
                myToolTipsControl.SetToolTip(CancelTestSampleCalValue, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Cancel", pLanguageID))
            End If

            'CLOSE SCREEN
            auxIconName = GetIconName("CANCEL")
            If (String.Compare(auxIconName, String.Empty, False) <> 0) Then
                bsAcceptButton.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
                myToolTipsControl.SetToolTip(bsAcceptButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_CloseScreen", pLanguageID))
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".PrepareButtons ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PrepareButtons ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

#End Region

#Region "CALIBRATORS TAB"

    ''' <summary>
    ''' Prepare the Calibrators Grid control
    ''' </summary>
    ''' <param name="pLanguageID">Current application Language</param>
    ''' <remarks>
    ''' Created by: TR 11/02/2011
    ''' Modified by: JC 13/11/2012. Modified column width.
    ''' </remarks>
    Private Sub PrepareCalibratorListGrid(ByVal pLanguageID As String)
        Try
            CalibratorListGrid.Columns.Clear()
            CalibratorListGrid.AutoSize = False
            CalibratorListGrid.AutoGenerateColumns = False
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            'VISIBLE COLUMNS....
            'Calibrator Name
            CalibratorListGrid.Columns.Add("CalibratorName", myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_CalibratorName", pLanguageID))
            CalibratorListGrid.Columns("CalibratorName").Width = 148
            CalibratorListGrid.Columns("CalibratorName").DataPropertyName = "CalibratorName"

            'Calibrator Lot Number
            CalibratorListGrid.Columns.Add("LotNumber", myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Lot", pLanguageID))
            CalibratorListGrid.Columns("LotNumber").Width = 136
            CalibratorListGrid.Columns("LotNumber").DataPropertyName = "LotNumber"

            'Number of Calibrator Points
            CalibratorListGrid.Columns.Add("NumberOfCalibrators", myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Number_Short", pLanguageID))
            CalibratorListGrid.Columns("NumberOfCalibrators").Width = 70
            CalibratorListGrid.Columns("NumberOfCalibrators").DataPropertyName = "NumberOfCalibrators"
            CalibratorListGrid.Columns("NumberOfCalibrators").HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter
            CalibratorListGrid.Columns("NumberOfCalibrators").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter

            'Expiration Date
            CalibratorListGrid.Columns.Add("ExpirationDate", myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_ExpDate_Full", pLanguageID))
            CalibratorListGrid.Columns("ExpirationDate").Width = 110
            CalibratorListGrid.Columns("ExpirationDate").DataPropertyName = "ExpirationDate"
            CalibratorListGrid.Columns("ExpirationDate").HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter
            CalibratorListGrid.Columns("ExpirationDate").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter

            'HIDDEN COLUMNS...
            'Calibrator Identifier
            CalibratorListGrid.Columns.Add("CalibratorID", "CalibratorID")
            CalibratorListGrid.Columns("CalibratorID").DataPropertyName = "CalibratorID"
            CalibratorListGrid.Columns("CalibratorID").Visible = False

            'InUse Flag
            CalibratorListGrid.Columns.Add("InUse", "InUse ID")
            CalibratorListGrid.Columns("InUse").DataPropertyName = "InUse"
            CalibratorListGrid.Columns("InUse").Visible = False

            'Special Calibrator Flag
            CalibratorListGrid.Columns.Add("SpecialCalib", "SpecialCalib")
            CalibratorListGrid.Columns("SpecialCalib").DataPropertyName = "SpecialCalib"
            CalibratorListGrid.Columns("SpecialCalib").Width = 0
            CalibratorListGrid.Columns("SpecialCalib").Visible = False
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".PrepareCalibratorList ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PrepareCalibratorList ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Fill the CalibratorListGrid with all the Calibrators
    ''' </summary>
    ''' <remarks>
    ''' Created by:  TR 11/02/2011
    ''' </remarks>
    Private Sub FillCalibratorList()
        Try
            Dim myGlobalDataTO As New GlobalDataTO
            Dim myCalibratorDelegate As New CalibratorsDelegate

            'Get all defined Calibrators
            myGlobalDataTO = myCalibratorDelegate.GetAllCalibrators(Nothing)
            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                LocalCalibratorDS = DirectCast(myGlobalDataTO.SetDatos, CalibratorsDS)
            Else
                'Error getting the list of Calibrators; shown it
                ShowMessage(Me.Name & ".FillCalibratorList ", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage, Me)
            End If

            CalibratorListGrid.DataSource = LocalCalibratorDS.tparCalibrators
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".FillCalibratorList ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".FillCalibratorList ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Load controls in the details area with data of the selected Calibrator, or clean them if no Calibrator is selected 
    ''' </summary>
    ''' <param name="pCalibratorID">Calibrator Identifier</param>
    ''' <remarks>
    ''' Created by: TR 11/02/2011
    ''' </remarks>
    Private Sub BindCalibratorsControls(ByVal pCalibratorID As Integer)
        Try
            Dim qCalibratorList As List(Of CalibratorsDS.tparCalibratorsRow) = (From a As CalibratorsDS.tparCalibratorsRow In LocalCalibratorDS.tparCalibrators _
                                                                               Where a.RowState <> DataRowState.Deleted _
                                                                             AndAlso a.CalibratorID = pCalibratorID).ToList()

            If (qCalibratorList.Count > 0) Then
                CalibIDTextBox.Text = qCalibratorList.First().CalibratorID.ToString()
                CalibratorNameTextBox.Text = qCalibratorList.First().CalibratorName
                LotNumberTextBox.Text = qCalibratorList.First().LotNumber
                CalibNumberUpDown.Value = qCalibratorList.First().NumberOfCalibrators
                ExpDatePickUpCombo.Value = qCalibratorList.First().ExpirationDate
            Else
                CalibIDTextBox.Text = "0"
                CalibratorNameTextBox.Clear()
                LotNumberTextBox.Clear()
                CalibNumberUpDown.Value = CalibNumberUpDown.Minimum
                ExpDatePickUpCombo.ResetText()
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".BindCalibratorsControls ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".BindCalibratorsControls ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Verify if the name informed for the Calibrator is unique (there is not another Calibrator with the same name)
    ''' </summary>
    ''' <param name="pCalibratorName">Calibrator Name</param>
    ''' <returns>TRUE is there is another Calibrator with the same name; FALSE in other case</returns>
    ''' <remarks>
    ''' Created by:  TR 11/02/2011
    ''' Modified by: XB 04/02/2013 - Upper conversions redundants because the value is already in UpperCase must delete to avoid Regional Settings problems (Bugs tracking #1112)
    ''' </remarks>
    Private Function CalibratorExist(ByVal pCalibratorName As String, ByVal pCalibratorID As Integer) As Boolean
        Dim myResult As Boolean = False
        Try
            Dim myCalibratorList As List(Of CalibratorsDS.tparCalibratorsRow) = (From a As CalibratorsDS.tparCalibratorsRow In LocalCalibratorDS.tparCalibrators _
                                                                                Where a.CalibratorName.Trim = pCalibratorName.Trim _
                                                                              AndAlso a.CalibratorID <> pCalibratorID _
                                                                               Select a).ToList()
            'Where a.CalibratorName.Trim.ToUpper = pCalibratorName.Trim.ToUpper _
            myResult = (myCalibratorList.Count > 0)
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".CalibratorExist ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".CalibratorExist ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
        Return myResult
    End Function

    ''' <summary>
    ''' Load in a CalibratorsDS, all data informed for a new Calibrator to create 
    ''' </summary>
    ''' <returns>Typed DataSet CalibratorsDS containing the data of the Calibrator to add</returns>
    ''' <remarks>
    ''' Created by: TR 14/02/2011
    ''' </remarks>
    Private Function CreateNewCalibrator() As CalibratorsDS
        Dim myCalibratorDS As New CalibratorsDS
        Try
            Dim myCalibratorRow As CalibratorsDS.tparCalibratorsRow
            myCalibratorRow = myCalibratorDS.tparCalibrators.NewtparCalibratorsRow

            'Set a "fake" CalibratorID
            myCalibratorRow.CalibratorID = 1000
            CalibIDTextBox.Text = "1000"

            myCalibratorRow.CalibratorName = CalibratorNameTextBox.Text
            myCalibratorRow.LotNumber = LotNumberTextBox.Text
            myCalibratorRow.ExpirationDate = ExpDatePickUpCombo.Value
            myCalibratorRow.NumberOfCalibrators = CInt(CalibNumberUpDown.Value)
            myCalibratorRow.SpecialCalib = False
            myCalibratorRow.InUse = False
            myCalibratorRow.TS_User = GetApplicationInfoSession.UserName
            myCalibratorRow.TS_DateTime = DateTime.Now.Date
            myCalibratorRow.IsNew = True
            myCalibratorDS.tparCalibrators.AddtparCalibratorsRow(myCalibratorRow)
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".CreateNewCalibrator ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".CreateNewCalibrator ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
        Return myCalibratorDS
    End Function

    ''' <summary>
    ''' Load in a CalibratorsDS all data informed for an existing Calibrator to update 
    ''' </summary>
    ''' <returns>Typed DataSet CalibratorsDS containing the data of the Calibrator to update</returns>
    ''' <remarks>
    ''' Created by: TR 14/02/2011
    ''' </remarks>
    Private Function UpdatedCalibrator() As CalibratorsDS
        Dim myCalibratorDS As New CalibratorsDS
        Try
            Dim myCalibratorRow As CalibratorsDS.tparCalibratorsRow
            myCalibratorRow = myCalibratorDS.tparCalibrators.NewtparCalibratorsRow

            myCalibratorRow.CalibratorID = CInt(CalibIDTextBox.Text)
            myCalibratorRow.CalibratorName = CalibratorNameTextBox.Text
            myCalibratorRow.LotNumber = LotNumberTextBox.Text
            myCalibratorRow.ExpirationDate = ExpDatePickUpCombo.Value
            myCalibratorRow.NumberOfCalibrators = CInt(CalibNumberUpDown.Value)
            myCalibratorRow.SpecialCalib = False
            myCalibratorRow.InUse = False
            myCalibratorRow.TS_User = GetApplicationInfoSession.UserName
            myCalibratorRow.TS_DateTime = DateTime.Now.Date
            myCalibratorRow.IsNew = False
            myCalibratorDS.tparCalibrators.AddtparCalibratorsRow(myCalibratorRow)
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".UpdatedCalibrator ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".UpdatedCalibrator ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
        Return myCalibratorDS
    End Function

    ''' <summary>
    ''' Prepare the Calibrator data before updating it on database
    ''' </summary>
    ''' <returns>TRUE if the Calibrator was added/updated; otherwise, FALSE</returns>
    ''' <remarks>
    ''' Created by: TR 16/02/2011
    ''' </remarks>
    Private Function SaveCalibratorInfo() As Boolean
        Dim myOperationResult As Boolean = False

        Try
            'Validate if the calibrator data is OK
            If (Not ValidateCalibratorDataError()) Then
                Dim myCalibratorID As Integer = -1
                Dim tempCalibratorDS As New CalibratorsDS
                Dim delCalibratorValuesAndResults As Boolean = False

                Select Case (LocalScreenStatus)
                    Case SCREEN_STATUS_TYPE.CreateMode
                        '** ADD NEW CALIBRATOR **

                        'Fill a CalibratorsDS with all data informed for the Calibrator to add
                        tempCalibratorDS = CreateNewCalibrator()

                        'Save the Calibrator
                        If (SaveCalibratorChanges(tempCalibratorDS, CALIBRATOR_ACTION.Create, False, False, Nothing)) Then
                            'Add the new Calibrator to the Calibrators list, and mark it as the selected row in the Grid
                            LocalCalibratorDS.tparCalibrators.ImportRow(tempCalibratorDS.tparCalibrators(0))
                            myCalibratorID = tempCalibratorDS.tparCalibrators(0).CalibratorID

                            CalibratorListGrid.Rows(CalibratorListGrid.Rows.Count - 1).Selected = True
                            CalibratorListGrid.FirstDisplayedScrollingRowIndex = CalibratorListGrid.Rows.Count - 1
                            CalibratorListGrid.Select()

                            myOperationResult = True
                            LocalValidateDependencies = False
                            LocalCalibratorLotModified = False

                            'Reload the ComboBox of Calibrators in the details area of the Test/Samples TAB
                            FillCalibratorListCombo()
                        End If
                        Exit Select

                    Case SCREEN_STATUS_TYPE.EditMode
                        '** EDIT AN EXISTING CALIBRATOR **

                        'Fill a CalibratorsDS with all data informed for the Calibrator to update
                        tempCalibratorDS = UpdatedCalibrator()
                        myCalibratorID = tempCalibratorDS.tparCalibrators(0).CalibratorID

                        Dim myCalibratorList As New List(Of Integer)
                        myCalibratorList.Add(myCalibratorID)

                        'If LotNumber or ExpirationDate have been changed, validate if there are affected elements
                        '(open Calibration results for whatever of the Tests/SampleTypes linked to the updated Calibrator)
                        If (LocalValidateDependencies) Then
                            If (LocalCalibratorLotModified) Then
                                Dim myResultDialog As DialogResult = GetAffectedTestConcentration(myCalibratorID)
                                If (myResultDialog = Windows.Forms.DialogResult.OK) Then
                                    delCalibratorValuesAndResults = True

                                ElseIf (myResultDialog <> Windows.Forms.DialogResult.None) Then
                                    Exit Select
                                End If
                            Else
                                If (ValidateCalibratorTestSampleDependencies(myCalibratorList) = Windows.Forms.DialogResult.OK) Then
                                    delCalibratorValuesAndResults = True
                                Else
                                    Exit Try
                                End If
                            End If
                        End If

                        'Save the Calibrator
                        If (SaveCalibratorChanges(tempCalibratorDS, CALIBRATOR_ACTION.Edit, delCalibratorValuesAndResults, _
                                                  delCalibratorValuesAndResults, Nothing)) Then
                            Dim qCalibratorList As List(Of CalibratorsDS.tparCalibratorsRow) = (From a As CalibratorsDS.tparCalibratorsRow In LocalCalibratorDS.tparCalibrators _
                                                                                               Where a.CalibratorID = myCalibratorID _
                                                                                              Select a).ToList()
                            If (qCalibratorList.Count > 0) Then
                                'Update local Calibrator dataset
                                qCalibratorList.First().ItemArray = tempCalibratorDS.tparCalibrators(0).ItemArray
                            End If

                            'Reload the ComboBox of Calibrators in the details area of the Test/Samples TAB
                            FillCalibratorListCombo()

                            'If the LotNumber was changed, all Tests/SampleTypes linked to the Calibrator have to be marked as INCOMPLETE
                            If (LocalCalibratorLotModified) Then
                                If (IsTestParamWinAttribute) Then
                                    'Clean values from the UpdatedTestCalibratorsValuesDS
                                    If (UpdatedTestCalibratorsValuesDS.tparTestCalibratorValues.Count > 0) Then
                                        If (UpdatedCalibrator.tparCalibrators.Count > 0 AndAlso UpdatedCalibrator.tparCalibrators(0).CalibratorID = myCalibratorID) Then
                                            UpdatedTestCalibratorsValuesDS.tparTestCalibratorValues.Clear()
                                        End If
                                    End If
                                End If
                                FillCalibratorTestSampleList(False)

                                Dim qTestList As List(Of TestsDS.tparTestsRow) = (From a As TestsDS.tparTestsRow In LocalTestDS.tparTests _
                                                                                 Where a.CalibratorID = myCalibratorID _
                                                                                Select a).ToList()

                                For Each TestRow As TestsDS.tparTestsRow In qTestList
                                    TestRow.BeginEdit()
                                    TestRow.EnableStatus = False
                                    TestRow.EndEdit()
                                Next
                                SetTestSampleStatusIcon()

                                'Select the first Test/SampleType on the list
                                If (CalibTestSampleListGrid.Rows.Count > 0) Then
                                    CalibTestSampleListGrid.FirstDisplayedScrollingRowIndex = 0
                                    CalibTestSampleListGrid.Rows(0).Selected = True

                                    BindCalibratorValues(CInt(CalibTestSampleListGrid.Rows(0).Cells("TestCalibratorID").Value), _
                                                         CInt(CalibTestSampleListGrid.Rows(0).Cells("TestID").Value), _
                                                         CalibTestSampleListGrid.Rows(0).Cells("SampleType").Value.ToString(), _
                                                         CInt(CalibTestSampleListGrid.Rows(0).Cells("CalibratorID").Value))
                                End If
                            Else
                                'Reload the Tests/SampleTypes grid
                                FillCalibratorTestSampleList(False)
                            End If

                            myOperationResult = True
                            LocalValidateDependencies = False
                            LocalCalibratorLotModified = False
                        End If
                        Exit Select

                    Case Else
                        Exit Select
                End Select

                If (myOperationResult) Then
                    'Validate if the selected Calibrator is marked as SPECIAL 
                    If (DirectCast(CalibratorListGrid.SelectedRows(0).Cells("SpecialCalib").Value, Boolean)) Then
                        SetScreenStatusTABCalibrator(SCREEN_STATUS_TYPE.SpecialCalibrator)
                    Else
                        SetScreenStatusTABCalibrator(SCREEN_STATUS_TYPE.ReadMode)
                    End If

                    BindCalibratorsControls(myCalibratorID)
                    LocalChange = False
                End If
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".SaveCalibratorInfo ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".SaveCalibratorInfo ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
        Return myOperationResult
    End Function

    ''' <summary>
    ''' Add a new Calibrator OR Update data of an existing Calibrator OR Delete a group of selected Calibrators
    ''' </summary>
    ''' <param name="pCalibratorsDS">Typed DataSet CalibratorsDS containing the Calibrator to add/update or the group of 
    '''                              Calibrators to delete</param>
    ''' <param name="pCalAction">Action to execute: Add/Update/Delete</param>
    ''' <param name="pDeleteCalibratorResult">When TRUE, it indicates all previous results saved for the Calibrator and all its linked 
    '''                                       Tests/SampleTypes have to be deleted</param>
    ''' <param name="pDeleteTestCalibratorValue">When TRUE, it indicates all theoretical concentration values for the Calibrator and all 
    '''                                          its linked Tests/SampleTypes have to be deleted</param>
    ''' <param name="myDeleteCalibratorList">List of an object DeletedCalibratorTO containing all Calibrators selected to be deleted and,
    '''                                      for each one of them, all its linked Tests/SampleTypes</param>
    ''' <returns>TRUE if the saving was successfully executed</returns>
    ''' <remarks> 
    ''' Created by:  TR 16/02/2011
    ''' </remarks>
    Private Function SaveCalibratorChanges(ByVal pCalibratorsDS As CalibratorsDS, ByVal pCalAction As CALIBRATOR_ACTION, _
                                           ByVal pDeleteCalibratorResult As Boolean, ByVal pDeleteTestCalibratorValue As Boolean, _
                                           ByVal myDeleteCalibratorList As List(Of DeletedCalibratorTO)) As Boolean
        Dim myResults As Boolean = False
        Try
            Dim myGlobalDataTO As New GlobalDataTO
            Dim myCalibratorsDelegate As New CalibratorsDelegate

            Select Case pCalAction
                Case CALIBRATOR_ACTION.Create
                    'CREATE PROCESS
                    myGlobalDataTO = myCalibratorsDelegate.Save(Nothing, pCalibratorsDS, New TestCalibratorsDS, New TestCalibratorValuesDS, _
                                                                Nothing, False, pDeleteCalibratorResult, pDeleteTestCalibratorValue, _
                                                                AnalyzerIDAttribute, WorkSessionIDAttribute)
                    Exit Select
                Case CALIBRATOR_ACTION.Edit
                    'EDIT PROCESS
                    myGlobalDataTO = myCalibratorsDelegate.Save(Nothing, pCalibratorsDS, New TestCalibratorsDS, New TestCalibratorValuesDS, _
                                                                Nothing, False, pDeleteCalibratorResult, pDeleteTestCalibratorValue, _
                                                                AnalyzerIDAttribute, WorkSessionIDAttribute)
                    Exit Select
                Case CALIBRATOR_ACTION.Delete
                    'DELETE PROCESS
                    myGlobalDataTO = myCalibratorsDelegate.Save(Nothing, pCalibratorsDS, New TestCalibratorsDS, New TestCalibratorValuesDS, _
                                                                myDeleteCalibratorList, IsTestParamWinAttribute, pDeleteCalibratorResult, _
                                                                pDeleteTestCalibratorValue, AnalyzerIDAttribute, WorkSessionIDAttribute)
                    Exit Select
            End Select

            myResults = (Not myGlobalDataTO.HasError)
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".SaveCalibratorChanges ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".SaveCalibratorChanges ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
        Return myResults
    End Function

    ''' <summary>
    ''' Prepare data to delete all selected Calibrators
    ''' </summary>
    ''' <returns>GlobalDataTO containing</returns>
    ''' <remarks>
    ''' Created by: TR 14/02/2011
    ''' </remarks>
    Private Function DeleteCalibrator() As GlobalDataTO
        Dim myGlobalDataTO As New GlobalDataTO

        Try
            If (CalibratorListGrid.SelectedRows.Count > 0) Then
                CalibratorTabControl.TabPages(1).Show()

                If (ShowMessage("Question", GlobalEnumerates.Messages.DELETE_CONFIRMATION.ToString()) = Windows.Forms.DialogResult.Yes) Then
                    Dim myCalibratorIDList As New List(Of Integer)
                    Dim myDeleteCalibratorTO As DeletedCalibratorTO
                    Dim myDeleteCalibratorList As New List(Of DeletedCalibratorTO)

                    'Add the ID of all selected Calibrators in a list of Integer and also in a list of DeletedCalibratorTO
                    For Each CalibratorRow As DataGridViewRow In Me.CalibratorListGrid.SelectedRows
                        myCalibratorIDList.Add(CInt(CalibratorRow.Cells("CalibratorID").Value))

                        myDeleteCalibratorTO = New DeletedCalibratorTO
                        myDeleteCalibratorTO.CalibratorID = CInt(CalibratorRow.Cells("CalibratorID").Value)
                        myDeleteCalibratorList.Add(myDeleteCalibratorTO)

                        'SA 13/11/2012
                        ''On the update elements in case came from parammeters windows search the deleted calibrator
                        ''and remove concentration values.
                        'If (IsTestParamWinAttribute) Then
                        '    If (UpdatedCalibrator.tparCalibrators.Count > 0 AndAlso _
                        '        UpdatedCalibrator.tparCalibrators(0).CalibratorID = myDeleteCalibratorTO.CalibratorID) _
                        '                                    OrElse UpdatedCalibrator.tparCalibrators(0).CalibratorID = 0 Then
                        '        UpdatedCalibrator.tparCalibrators.Clear()
                        '        LocalConcentrationValuesDS.tparTestCalibratorValues.Clear()
                        '        UpdatedTestCalibratorsValuesDS.tparTestCalibratorValues.Clear()
                        '    End If
                        'End If
                    Next

                    'Search all the Tests/SampleTypes linked to all selected Calibrators and shown them in the auxiliary screen of affected elements
                    If (ValidateCalibratorTestSampleDependencies(myCalibratorIDList) = Windows.Forms.DialogResult.OK) Then
                        If (SaveCalibratorChanges(New CalibratorsDS, CALIBRATOR_ACTION.Delete, True, True, myDeleteCalibratorList)) Then
                            'Refill the grid of Calibrators
                            FillCalibratorList()

                            If (Me.CalibratorListGrid.RowCount > 0) Then
                                Me.CalibratorListGrid.Rows(0).Selected = True
                                Me.CalibratorListGrid.FirstDisplayedScrollingRowIndex = 0

                                SetScreenStatusTABCalibrator(SCREEN_STATUS_TYPE.ReadMode)
                                BindCalibratorsControls(CInt(Me.CalibratorListGrid.Rows(0).Cells("CalibratorID").Value))
                                LocalChange = False

                                'On the LocalTestDS search all the Tests/SampleTypes linked to all deleted Calibrators 
                                'and change the enable status to False to show the warning icon of incomplete programming
                                Dim qTestList As New List(Of TestsDS.tparTestsRow)
                                For Each myDelTO As DeletedCalibratorTO In myDeleteCalibratorList
                                    qTestList = (From a As TestsDS.tparTestsRow In LocalTestDS.tparTests _
                                                Where a.CalibratorID = myDelTO.CalibratorID _
                                               Select a).ToList()

                                    If (qTestList.Count > 0) Then
                                        For Each TestRow As TestsDS.tparTestsRow In qTestList
                                            TestRow.BeginEdit()
                                            TestRow.EnableStatus = False
                                            TestRow.CalibratorID = -1
                                            TestRow.TestCalibratorID = -1
                                            TestRow("CalibratorName") = ""
                                            TestRow.EndEdit()
                                        Next
                                    End If
                                Next

                                ''Refill the calibrator list.
                                'FillCalibratorList()

                                'After updating all the status on the local structure, shown the icon in the grid of Calibrator by Test/SampleType
                                SetTestSampleStatusIcon()

                                'Reload the ComboBox of Calibrators 
                                FillCalibratorListCombo()

                                'Set focus to Calibrator by Test/SampleType TAB and select the first element in the grid
                                CalibratorTabControl.TabPages(1).Show()
                                If (CalibTestSampleListGrid.Rows.Count > 0) Then
                                    CalibTestSampleListGrid.FirstDisplayedScrollingRowIndex = 0
                                    CalibTestSampleListGrid.Rows(0).Selected = True
                                    CalibTestSampleListGrid.Refresh()

                                    If (DirectCast(CalibratorListGrid.SelectedRows(0).Cells("InUse").Value, Boolean)) Then
                                        'Set screen status to Element InUse mode
                                        SetScreenStatusTABCalibrator(SCREEN_STATUS_TYPE.ElementInUse)

                                    ElseIf DirectCast(CalibratorListGrid.SelectedRows(0).Cells("SpecialCalib").Value, Boolean) Then
                                        'Set screen status to Special Calibrator mode
                                        SetScreenStatusTABCalibrator(SCREEN_STATUS_TYPE.SpecialCalibrator)
                                    Else
                                        'Set screen status to ReadOnly mode
                                        SetScreenStatusTABCalibrator(SCREEN_STATUS_TYPE.ReadMode)
                                    End If

                                    BindCalibratorValues(CInt(CalibTestSampleListGrid.Rows(0).Cells("TestCalibratorID").Value), _
                                                         CInt(CalibTestSampleListGrid.Rows(0).Cells("TestID").Value), _
                                                         CalibTestSampleListGrid.Rows(0).Cells("SampleType").Value.ToString(), _
                                                         CInt(CalibTestSampleListGrid.Rows(0).Cells("CalibratorID").Value))
                                    CalibratorTabControl.TabPages(1).Refresh()
                                End If
                            End If
                        End If
                    End If
                End If
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".DeleteCalibrator ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".DeleteCalibrator ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
        Return myGlobalDataTO
    End Function

    ''' <summary>
    ''' Enable/disable controls in the Calibrators TAB according the screen status
    ''' </summary>
    ''' <param name="pStatus">Screen Status</param>
    ''' <remarks>
    ''' Created by:  TR 
    ''' Modified by: TR 06/10/2011 - Enable/disable controls depending on if the screen was opened from Tests Programming
    '''                              Screen or from the Calibrators Menu entry
    '''              DL 19/07/2011 - Added call to new function EnableDisableCalibratorInfoGroupBox
    '''              TR 23/04/2012 - Added call to new function ScreenStatusByUserLevel to enable/disable screen actions 
    '''                              according the level of the connected User
    ''' </remarks>
    Private Sub SetScreenStatusTABCalibrator(ByVal pStatus As SCREEN_STATUS_TYPE)
        Try
            Select Case (pStatus)
                Case SCREEN_STATUS_TYPE.CreateMode
                    LocalScreenStatus = pStatus

                    'Disable Controls
                    AddCalibButon.Enabled = False
                    EditCalibButton.Enabled = False
                    DeleteCalbButton.Enabled = False
                    CalibratorListGroupBox.Enabled = False

                    'Enable Controls
                    SaveCalibButton.Enabled = True
                    CancelCalibButton.Enabled = True
                    EnableDisableCalibratorInfoGroupBox(True)
                    Exit Select

                Case SCREEN_STATUS_TYPE.ReadMode
                    LocalScreenStatus = pStatus

                    'Disable Controls
                    SaveCalibButton.Enabled = False
                    CancelCalibButton.Enabled = False
                    EnableDisableCalibratorInfoGroupBox(False)

                    'Enable Controls
                    AddCalibButon.Enabled = True
                    If (IsTestParamWinAttribute) Then
                        EditCalibButton.Enabled = False
                        DeleteCalbButton.Enabled = False
                    Else
                        EditCalibButton.Enabled = True
                        DeleteCalbButton.Enabled = True
                    End If
                    CalibratorListGroupBox.Enabled = True
                    Exit Select

                Case SCREEN_STATUS_TYPE.EditMode
                    LocalScreenStatus = pStatus

                    'Disable Controls
                    AddCalibButon.Enabled = False
                    EditCalibButton.Enabled = False
                    DeleteCalbButton.Enabled = False
                    CalibratorListGroupBox.Enabled = False

                    'Enable Controls
                    SaveCalibButton.Enabled = True
                    CancelCalibButton.Enabled = True
                    EnableDisableCalibratorInfoGroupBox(True)
                    Exit Select

                Case SCREEN_STATUS_TYPE.ElementInUse
                    LocalScreenStatus = pStatus

                    'Disable Controls
                    EditCalibButton.Enabled = False
                    DeleteCalbButton.Enabled = False
                    EnableDisableCalibratorInfoGroupBox(False)

                    'Enable Controls
                    AddCalibButon.Enabled = True
                    CalibratorListGroupBox.Enabled = True
                    Exit Select

                Case SCREEN_STATUS_TYPE.MultiSelection
                    LocalScreenStatus = pStatus

                    'Disable Controls
                    AddCalibButon.Enabled = False
                    EditCalibButton.Enabled = False
                    EnableDisableCalibratorInfoGroupBox(False)

                    'Enable Controls
                    DeleteCalbButton.Enabled = True
                    Exit Select

                Case SCREEN_STATUS_TYPE.SpecialCalibrator
                    LocalScreenStatus = pStatus

                    'Disable controls
                    SaveCalibButton.Enabled = False
                    CancelCalibButton.Enabled = False
                    EnableDisableCalibratorInfoGroupBox(False)
                    DeleteCalbButton.Enabled = False

                    'TR 01/10/2013 -Enable controls depending on IsTestParamWinAttribute.
                    If (IsTestParamWinAttribute) Then
                        EditCalibButton.Enabled = False
                        DeleteCalbButton.Enabled = False
                    Else
                        EditCalibButton.Enabled = True
                        DeleteCalbButton.Enabled = True
                    End If
                    'TR 01/10/2013 -END

                    'TR 01/10/2013 -Commented
                    'Enable Controls
                    'AddCalibButon.Enabled = True
                    'EditCalibButton.Enabled = True
                    'TR 01/10/2013 -END

                    CalibratorListGroupBox.Enabled = True
                    Exit Select

                Case Else
                    Exit Select
            End Select

            ScreenStatusByUserLevel()
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".SetScreenStatus ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".SetScreenStatus ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Enable/disable all controls in CalibratorInfoGroupBox
    ''' </summary>
    ''' <remarks>
    '''Created by: DL 19/07/2011
    ''' </remarks>
    Private Sub EnableDisableCalibratorInfoGroupBox(ByVal pState As Boolean)
        Try
            Dim BackColor As New Color
            Dim LetColor As New Color

            If (Not pState) Then
                BackColor = SystemColors.MenuBar
                LetColor = Color.DarkGray
            Else
                BackColor = Color.White
                LetColor = Color.Black
            End If

            'SA 13/11/2012
            'CalibratorListCombo.Enabled = pState
            'CalibratorListCombo.BackColor = BackColor
            'CalibratorListCombo.ForeColor = LetColor

            CalibratorNameTextBox.Enabled = pState
            CalibratorNameTextBox.BackColor = BackColor
            CalibratorNameTextBox.ForeColor = LetColor

            LotNumberTextBox.Enabled = pState
            LotNumberTextBox.BackColor = BackColor
            LotNumberTextBox.ForeColor = LetColor

            ExpDatePickUpCombo.Enabled = pState
            ExpDatePickUpCombo.BackColor = BackColor
            ExpDatePickUpCombo.ForeColor = LetColor

            CalibNumberUpDown.Enabled = pState
            CalibNumberUpDown.BackColor = BackColor
            CalibNumberUpDown.ForeColor = LetColor
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".EnableDisableCalibratorInfoGroupBox ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".EnableDisableCalibratorInfoGroupBox ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' When a Calibrator is deleted or when it is modified by changing its LotNumber or NumberOfCalibrators, this function
    ''' search the list of affected Elements (all its linked Tests/SampleTypes) and shown them in the auxiliary screen 
    ''' </summary>
    ''' <returns>DialogResult indicating if the User has accepted or cancelled the delete/update action after reviewing
    '''          the element affected</returns>
    ''' <remarks>
    ''' Created by:  TR 14/12/2010
    ''' Modified by: SA 08/11/2012 - If an error happens during the process, shown it
    ''' </remarks>
    Private Function GetAffectedTestConcentration(ByVal pCalibratorID As Integer) As DialogResult
        Dim myResult As DialogResult = Windows.Forms.DialogResult.None

        Try
            Dim myResultGlobalTO As New GlobalDataTO
            Dim myProfileMember As String = ""
            Dim myResultDelegate As New ResultsDelegate
            Dim myDependenciesElementsDS As New DependenciesElementsDS
            Dim myDepElemenRow As DependenciesElementsDS.DependenciesElementsRow
            Dim myPreloadeDelegate As New PreloadedMasterDataDelegate
            Dim myTestCalibratorDelegate As New TestCalibratorsDelegate

            myResultGlobalTO = myTestCalibratorDelegate.GetAllTestCalibratorByCalibratorID(Nothing, pCalibratorID)
            If (Not myResultGlobalTO.HasError AndAlso Not myResultGlobalTO.SetDatos Is Nothing) Then
                Dim myTestSampleCalibratorDS As TestSampleCalibratorDS = DirectCast(myResultGlobalTO.SetDatos, TestSampleCalibratorDS)

                Dim imageBytes As Byte()
                imageBytes = myPreloadeDelegate.GetIconImage("TESTICON")

                For Each TestCalibRow As TestSampleCalibratorDS.tparTestCalibratorsRow In myTestSampleCalibratorDS.tparTestCalibrators.Rows
                    myDepElemenRow = myDependenciesElementsDS.DependenciesElements.NewDependenciesElementsRow()
                    myDepElemenRow.Type = imageBytes
                    myDepElemenRow.Name = TestCalibRow.TestName

                    'Get results for the last executed Calibrator for the Test/SampleType in process
                    myResultGlobalTO = myResultDelegate.GetLastExecutedCalibrator(Nothing, TestCalibRow.TestID, TestCalibRow.SampleType, _
                                                                                  TestCalibRow.TestVersionNumber)

                    If (Not myResultGlobalTO.HasError AndAlso Not myResultGlobalTO.SetDatos Is Nothing) Then
                        'Go throught each element and set the Absorbance value into myProfileMember variable, 
                        'then add it into the myAdditionalelementsDS
                        myProfileMember = ""
                        For Each ResultRow As WSAdditionalElementsDS.WSAdditionalElementsTableRow _
                                           In DirectCast(myResultGlobalTO.SetDatos, WSAdditionalElementsDS).WSAdditionalElementsTable.Rows
                            If (ResultRow.IsCalibratorFactorNull) Then
                                myProfileMember &= ResultRow.ABSValue.ToString() & vbCrLf
                            Else
                                myProfileMember &= ResultRow.ABSValue.ToString() & " (" & ResultRow.CalibratorFactor.ToString() & ")" & vbCrLf
                            End If
                            myDepElemenRow.ResultDateTime = ResultRow.ResultDateTime
                        Next
                    Else
                        Exit For
                    End If

                    myDepElemenRow.FormProfileMember = myProfileMember
                    myDependenciesElementsDS.DependenciesElements.AddDependenciesElementsRow(myDepElemenRow)

                    If (myResultGlobalTO.HasError) Then Exit For
                Next
            End If

            If (Not myResultGlobalTO.HasError) Then
                'Show the screen of Affected Elements
                If (myDependenciesElementsDS.DependenciesElements.Count > 0) Then
                    Using myAffectedElementsWarning As New UiWarningAfectedElements()
                        myAffectedElementsWarning.AffectedElements = myDependenciesElementsDS
                        myAffectedElementsWarning.AdditionalElements = True
                        myAffectedElementsWarning.ElementsAffectedMessageDetail = "MSG_UPDATE_CONCENTRATION"

                        myAffectedElementsWarning.ShowDialog()
                        myResult = myAffectedElementsWarning.DialogResult
                    End Using
                End If
            Else
                'Error getting the list of affected Tests/SampleTypes
                ShowMessage(Me.Name & ".GetAffectedTestConcentration ", myResultGlobalTO.ErrorCode, myResultGlobalTO.ErrorMessage, Me)
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".GetAffectedTestConcentration ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".GetAffectedTestConcentration ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
        Return myResult
    End Function

#End Region

#Region "CALIBRATOR BY TEST/SAMPLE TYPE TAB"

    ''' <summary>
    ''' Enable/disable all controls in CalibCurveInfoGroupBox
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 19/07/2011
    ''' Modified by: TR 16/01/2012 - Set BackColor of ComboBoxes in Curve Values area according the informed status
    ''' </remarks>
    Private Sub EnableDisableCalibCurveInfoGroupBox(ByVal pState As Boolean)
        Try
            Dim BackColor As New Color
            Dim LetColor As New Color

            If Not pState Then
                BackColor = SystemColors.MenuBar
                LetColor = Color.DarkGray
            Else
                BackColor = Color.White
                LetColor = Color.Black
            End If

            IncreasingRadioButton.Enabled = pState
            IncreasingRadioButton.ForeColor = LetColor

            DecreasingRadioButton.Enabled = pState
            DecreasingRadioButton.ForeColor = LetColor

            CurveTypeCombo.Enabled = pState
            If (pState) Then
                If (CurveTypeCombo.SelectedIndex >= 0) Then
                    CalibratorListCombo.BackColor = BackColor
                Else
                    CurveTypeCombo.BackColor = Color.Khaki
                End If
            Else
                CurveTypeCombo.BackColor = BackColor
            End If

            XAxisCombo.Enabled = pState
            If (pState) Then
                If (XAxisCombo.SelectedIndex >= 0) Then
                    XAxisCombo.BackColor = BackColor
                Else
                    XAxisCombo.BackColor = Color.Khaki
                End If
            Else
                XAxisCombo.BackColor = BackColor
            End If

            YAxisCombo.Enabled = pState
            If (pState) Then
                If (YAxisCombo.SelectedIndex >= 0) Then
                    YAxisCombo.BackColor = BackColor
                Else
                    YAxisCombo.BackColor = Color.Khaki
                End If
            Else
                YAxisCombo.BackColor = BackColor
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".EnableDisableCalibCurveInfoGroupBox ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".EnableDisableCalibCurveInfoGroupBox ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Enable/disable all controls in details area of Calibrator by Test/SampleType
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 19/07/2011
    ''' Modified by: TR 16/01/2012 - Set BackColor of Calibrator ComboBox according the informed status
    ''' </remarks>
    Private Sub EnableDisableTestSampleCalibFrame(ByVal pState As Boolean)
        Try
            Dim BackColor As New Color
            Dim LetColor As New Color

            If (Not pState) Then
                BackColor = SystemColors.MenuBar
                LetColor = Color.DarkGray
            Else
                BackColor = Color.White
                LetColor = Color.Black
            End If

            CalibratorListCombo.Enabled = pState
            If (pState) Then
                If (CalibratorListCombo.SelectedIndex >= 0) Then
                    CalibratorListCombo.BackColor = BackColor
                Else
                    CalibratorListCombo.BackColor = Color.Khaki
                End If
            Else
                CalibratorListCombo.BackColor = BackColor
            End If
            CalibratorListCombo.ForeColor = LetColor

            ConcentrationGridView.Enabled = pState
            ConcentrationGridView.BackColor = BackColor
            ConcentrationGridView.ForeColor = LetColor

            SaveTestSampleCalValue.Enabled = pState
            CancelTestSampleCalValue.Enabled = pState
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".EnableDisableTestSampleCalibFrame ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".EnableDisableTestSampleCalibFrame ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Enable/disable controls in the Calibrator by Test/SampleType TAB according the screen status
    ''' </summary>
    ''' <param name="pTestSampleCalStatus">Screen Status</param>
    ''' <remarks>
    ''' Created by:  TR
    ''' Modified by: DL 19/07/2011 - Added calls to new functions EnableDisableTestSampleCalibFrame and 
    '''                              EnableDisableCalibCurveInfoGroupBox
    '''              TR 23/04/2012 - Added call to new function ScreenStatusByUserLevel to enable/disable screen actions 
    '''                              according the level of the connected User
    ''' </remarks>
    Private Sub SetScreenStatusTABTestSampleCalib(ByVal pTestSampleCalStatus As SCREEN_STATUS_TYPE)
        Try
            Select Case (pTestSampleCalStatus)
                Case SCREEN_STATUS_TYPE.ReadMode
                    LocalScreenStatus = pTestSampleCalStatus

                    'Enable Controls
                    CalibratorTestSampleGB.Enabled = True
                    EditCalTestSampleButton.Enabled = True

                    'Disable Controls
                    EnableDisableTestSampleCalibFrame(False)
                    EnableDisableCalibCurveInfoGroupBox(False)
                    Exit Select

                Case SCREEN_STATUS_TYPE.ElementInUse
                    LocalScreenStatus = pTestSampleCalStatus

                    'Enable Controls
                    Me.CalibratorTestSampleGB.Enabled = True

                    'Disable Controls
                    EditCalTestSampleButton.Enabled = False
                    EnableDisableTestSampleCalibFrame(False)
                    EnableDisableCalibCurveInfoGroupBox(False)
                    Exit Select

                Case SCREEN_STATUS_TYPE.EditMode
                    LocalScreenStatus = pTestSampleCalStatus

                    'Enable Controls
                    EnableDisableTestSampleCalibFrame(True)

                    'Disable Controls
                    CalibratorTestSampleGB.Enabled = False
                    EditCalTestSampleButton.Enabled = False

                    'Depending on the number of Calibrator points the Curve Values area is 
                    'enable (when multipoint) or disable (when single point)
                    If (ConcentrationGridView.Rows.Count > 1) Then
                        EnableDisableCalibCurveInfoGroupBox(True)
                    Else
                        EnableDisableCalibCurveInfoGroupBox(False)
                    End If
                    Exit Select

                Case Else
                    Exit Select
            End Select

            ScreenStatusByUserLevel()
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " SetScreenStatusTABTestSampleCalib ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Prepare the grid of Calibrator by Test/SampleType
    ''' </summary>
    ''' <remarks>
    '''Created by: TR 15/02/2011
    ''' </remarks>
    Private Sub PrepareCalibratorTestSampleListGrid(ByVal pLanguageID As String)
        Try
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            CalibTestSampleListGrid.AutoGenerateColumns = False
            CalibTestSampleListGrid.AutoSize = False
            CalibTestSampleListGrid.ReadOnly = True
            CalibTestSampleListGrid.MultiSelect = False
            CalibTestSampleListGrid.Columns.Clear()

            'Visible columns
            Dim TypeIconColumn As New DataGridViewImageColumn
            TypeIconColumn.Name = "Status"
            TypeIconColumn.HeaderText = ""
            TypeIconColumn.Width = 35
            TypeIconColumn.ReadOnly = True
            TypeIconColumn.DataPropertyName = "Type"
            CalibTestSampleListGrid.Columns.Add(TypeIconColumn)

            CalibTestSampleListGrid.Columns.Add("TestName", myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_TestName", pLanguageID))
            CalibTestSampleListGrid.Columns("TestName").Width = 140
            CalibTestSampleListGrid.Columns("TestName").DataPropertyName = "TestName"

            CalibTestSampleListGrid.Columns.Add("SampleType", myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Type", pLanguageID))
            CalibTestSampleListGrid.Columns("SampleType").Width = 100
            CalibTestSampleListGrid.Columns("SampleType").DataPropertyName = "SampleType"

            CalibTestSampleListGrid.Columns.Add("CalibratorName", myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_CalibratorName", pLanguageID))
            CalibTestSampleListGrid.Columns("CalibratorName").Width = 150
            CalibTestSampleListGrid.Columns("CalibratorName").DataPropertyName = "CalibratorName"

            'Not visible columns
            CalibTestSampleListGrid.Columns.Add("TestCalibratorID", "TestCalibratorID")
            CalibTestSampleListGrid.Columns("TestCalibratorID").Width = 145
            CalibTestSampleListGrid.Columns("TestCalibratorID").DataPropertyName = "TestCalibratorID"
            CalibTestSampleListGrid.Columns("TestCalibratorID").Visible = False

            CalibTestSampleListGrid.Columns.Add("TestID", "TestID")
            CalibTestSampleListGrid.Columns("TestID").Width = 145
            CalibTestSampleListGrid.Columns("TestID").DataPropertyName = "TestID"
            CalibTestSampleListGrid.Columns("TestID").Visible = False

            CalibTestSampleListGrid.Columns.Add("CalibratorID", "CalibratorID")
            CalibTestSampleListGrid.Columns("CalibratorID").Width = 145
            CalibTestSampleListGrid.Columns("CalibratorID").DataPropertyName = "CalibratorID"
            CalibTestSampleListGrid.Columns("CalibratorID").Visible = False

            CalibTestSampleListGrid.Columns.Add("EnableStatus", "EnableStatus")
            CalibTestSampleListGrid.Columns("EnableStatus").Width = 145
            CalibTestSampleListGrid.Columns("EnableStatus").DataPropertyName = "EnableStatus"
            CalibTestSampleListGrid.Columns("EnableStatus").Visible = False

            CalibTestSampleListGrid.Columns.Add("InUse", "InUse")
            CalibTestSampleListGrid.Columns("InUse").Width = 145
            CalibTestSampleListGrid.Columns("InUse").DataPropertyName = "InUse"
            CalibTestSampleListGrid.Columns("InUse").Visible = False

            CalibTestSampleListGrid.Columns.Add("DecimalsAllowed", "DecimalsAllowed")
            CalibTestSampleListGrid.Columns("DecimalsAllowed").DataPropertyName = "DecimalsAllowed"
            CalibTestSampleListGrid.Columns("DecimalsAllowed").Visible = False

            CalibTestSampleListGrid.Columns.Add("TestVersionNumber", "TestVersionNumber")
            CalibTestSampleListGrid.Columns("TestVersionNumber").DataPropertyName = "TestVersionNumber"
            CalibTestSampleListGrid.Columns("TestVersionNumber").Visible = False


            CalibTestSampleListGrid.Columns.Add("SpecialTest", "SpecialTest")
            CalibTestSampleListGrid.Columns("SpecialTest").DataPropertyName = "SpecialTest"
            CalibTestSampleListGrid.Columns("SpecialTest").Visible = False
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".PrepareCalibratorTestSampleList ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PrepareCalibratorTestSampleList ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Prepare the grid of Theoretical Concentration values
    ''' </summary>
    ''' <remarks>
    ''' Created by:  TR 03/06/2010
    ''' Modified by: TR 26/11/2010 - Set MaxLength = 10 the DataGridViewTextBoxColumn for the Theoretical Concentration
    ''' </remarks>
    Private Sub PrepareConcentrationGridView(ByVal pLanguageID As String)
        Try
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            ConcentrationGridView.AutoGenerateColumns = False
            ConcentrationGridView.Enabled = True
            ConcentrationGridView.ReadOnly = False
            ConcentrationGridView.AutoSize = False
            ConcentrationGridView.EditMode = DataGridViewEditMode.EditOnEnter
            ConcentrationGridView.SelectionMode = DataGridViewSelectionMode.CellSelect
            ConcentrationGridView.Columns.Clear()

            ConcentrationGridView.Columns.Add("CalibratorNum", myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Number_Short", pLanguageID))
            ConcentrationGridView.Columns("CalibratorNum").Width = 50
            ConcentrationGridView.Columns("CalibratorNum").DataPropertyName = "CalibratorNum"
            ConcentrationGridView.Columns("CalibratorNum").ReadOnly = True
            ConcentrationGridView.Columns("CalibratorNum").HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter
            ConcentrationGridView.Columns("CalibratorNum").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            ConcentrationGridView.Columns("CalibratorNum").DefaultCellStyle.BackColor = SystemColors.MenuBar
            ConcentrationGridView.Columns("CalibratorNum").DefaultCellStyle.SelectionBackColor = SystemColors.MenuBar
            ConcentrationGridView.Columns("CalibratorNum").DefaultCellStyle.ForeColor = Color.DarkGray
            ConcentrationGridView.Columns("CalibratorNum").DefaultCellStyle.SelectionForeColor = Color.DarkGray
            ConcentrationGridView.Columns("CalibratorNum").SortMode = DataGridViewColumnSortMode.NotSortable


            Dim TheoricalColumn As New DataGridViewTextBoxColumn()
            TheoricalColumn.Name = "TheoricalConcentration"
            TheoricalColumn.MaxInputLength = 10
            TheoricalColumn.HeaderText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Concentration_Long", pLanguageID)
            ConcentrationGridView.Columns.Add(TheoricalColumn)
            ConcentrationGridView.Columns("TheoricalConcentration").Width = 120
            ConcentrationGridView.Columns("TheoricalConcentration").DataPropertyName = "TheoricalConcentration"
            ConcentrationGridView.Columns("TheoricalConcentration").ReadOnly = False
            ConcentrationGridView.Columns("TheoricalConcentration").HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter
            ConcentrationGridView.Columns("TheoricalConcentration").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            ConcentrationGridView.Columns("TheoricalConcentration").SortMode = DataGridViewColumnSortMode.NotSortable

            ConcentrationGridView.Columns.Add("KitConcentrationRelation", myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_CalibFactor", pLanguageID))
            ConcentrationGridView.Columns("KitConcentrationRelation").Width = 65
            ConcentrationGridView.Columns("KitConcentrationRelation").DataPropertyName = "KitConcentrationRelation"
            ConcentrationGridView.Columns("KitConcentrationRelation").ReadOnly = True
            ConcentrationGridView.Columns("KitConcentrationRelation").HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight
            ConcentrationGridView.Columns("KitConcentrationRelation").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            ConcentrationGridView.Columns("KitConcentrationRelation").DefaultCellStyle.BackColor = SystemColors.MenuBar
            ConcentrationGridView.Columns("KitConcentrationRelation").DefaultCellStyle.ForeColor = Color.DarkGray
            ConcentrationGridView.Columns("KitConcentrationRelation").DefaultCellStyle.SelectionBackColor = SystemColors.MenuBar
            ConcentrationGridView.Columns("KitConcentrationRelation").DefaultCellStyle.SelectionForeColor = Color.DarkGray
            ConcentrationGridView.Columns("KitConcentrationRelation").SortMode = DataGridViewColumnSortMode.NotSortable

            ConcentrationGridView.Columns.Add("TestCalibratorID", "TestCalibratorID")
            ConcentrationGridView.Columns("TestCalibratorID").DataPropertyName = "TestCalibratorID"
            ConcentrationGridView.Columns("TestCalibratorID").Visible = False
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".PrepareConcentrationGridView ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PrepareConcentrationGridView ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Fill the grid of Calibrator by Test/SampleType
    ''' </summary>
    ''' <remarks>
    ''' Created by:  TR 15/02/2011
    ''' Modified by: SA 08/11/2012 - Changes to avoid some screen errors
    ''' </remarks>
    Private Sub FillCalibratorTestSampleList(ByVal pCalibratorRemoved As Boolean)
        Try
            Dim myGlobalDataTO As New GlobalDataTO
            Dim myTestDelegate As New TestsDelegate

            If (IsTestParamWinAttribute) Then
                'Get the Test/SampleType being edited
                myGlobalDataTO = myTestDelegate.GetCalibratorTestSampleList(Nothing, ExternalTestIDAttribute, ExternalSampleTypeAttribute)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    LocalTestDS = DirectCast(myGlobalDataTO.SetDatos, TestsDS)

                    If (LocalTestDS.tparTests.Count > 0) Then
                        If (UpdatedCalibratorsDS.tparCalibrators.Count > 0) Then
                            LocalTestDS.tparTests.First().CalibratorID = UpdatedCalibratorsDS.tparCalibrators(0).CalibratorID
                            LocalTestDS.tparTests.First().CalibratorName = UpdatedCalibratorsDS.tparCalibrators(0).CalibratorName

                            'Set the enable value to TRUE
                            LocalTestDS.tparTests.First().EnableStatus = True
                        End If
                    Else
                        If (UpdatedTestDS.tparTests.Count > 0) Then
                            LocalTestDS.tparTests.ImportRow(UpdatedTestDS.tparTests(0))
                        Else
                            SetReceivedTestSample()
                        End If
                    End If
                Else
                    'Error getting the Calibrator linked to the informed Test/SampleTypes; shown it
                    ShowMessage(Me.Name & ".FillCalibratorTestSampleList ", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage, Me)
                End If
            Else
                ''Get all Tests/SampleTypes not linked to any Calibrator
                Dim qTestList As New List(Of TestsDS.tparTestsRow)
                qTestList = PendingTestSampleWithOutCalib()

                'Get the list of all Calibrator by Test/SampleTypes saved in DB
                myGlobalDataTO = myTestDelegate.GetCalibratorTestSampleList(Nothing)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    LocalTestDS = DirectCast(myGlobalDataTO.SetDatos, TestsDS)

                    'Add to the list, all the Tests/SampleTypes not linked to any Calibrator
                    For Each testRow As TestsDS.tparTestsRow In qTestList
                        LocalTestDS.tparTests.ImportRow(testRow)
                    Next
                Else
                    'Error getting the list of all Calibrator by Test/SampleTypes saved in DB; shown it
                    ShowMessage(Me.Name & ".FillCalibratorTestSampleList ", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage, Me)
                End If
            End If

            'For all Tests/SampleType not linked to any Calibrator (EnableStatus is FALSE), set the Warning icon 
            SetTestSampleStatusIcon()

            LocalTestDS.tparTests.DefaultView.Sort = "EnableStatus, CalibratorID, TestName"
            CalibTestSampleListGrid.DataSource = LocalTestDS.tparTests.DefaultView


            ''Get all Tests/SampleTypes not linked to any Calibrator
            'Dim qTestList As New List(Of TestsDS.tparTestsRow)
            'qTestList = PendingTestSampleWithOutCalib()

            ''Get the list of all Calibrator by Test/SampleTypes saved in DB
            'myGlobalDataTO = myTestDelegate.GetCalibratorTestSampleList(Nothing)
            'If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
            '    LocalTestDS = DirectCast(myGlobalDataTO.SetDatos, TestsDS)

            '    'Add to the list, all the Tests/SampleTypes not linked to any Calibrator
            '    For Each testRow As TestsDS.tparTestsRow In qTestList
            '        LocalTestDS.tparTests.ImportRow(testRow)
            '    Next

            '    If (IsTestParamWinAttribute) Then
            '        'Get the Test/SampleType beign edited in the screen of Tests Programming
            '        Dim qTestsList As List(Of TestsDS.tparTestsRow) = (From a As TestsDS.tparTestsRow In LocalTestDS.tparTests _
            '                                                          Where a.TestID = ExternalTestIDAttribute _
            '                                                        AndAlso a.SampleType = ExternalSampleTypeAttribute _
            '                                                         Select a).ToList()

            '        If (qTestsList.Count > 0) Then
            '            If (UpdatedCalibratorsDS.tparCalibrators.Count > 0) Then
            '                qTestsList.First().CalibratorID = UpdatedCalibratorsDS.tparCalibrators(0).CalibratorID
            '                qTestsList.First()("CalibratorName") = UpdatedCalibratorsDS.tparCalibrators(0).CalibratorName

            '                'Set the enable value to TRUE
            '                qTestsList.First().EnableStatus = True
            '            End If
            '        Else
            '            If (UpdatedTestDS.tparTests.Count > 0) Then
            '                LocalTestDS.tparTests.ImportRow(UpdatedTestDS.tparTests(0))
            '            Else
            '                SetReceivedTestSample()
            '            End If
            '        End If
            '    End If
            'Else
            '    'Error getting the list of all Calibrator by Test/SampleTypes saved in DB; shown it
            '    ShowMessage(Me.Name & ".FillCalibratorTestSampleList ", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage, Me)
            'End If

            ''For all Tests/SampleType not linked to any Calibrator (EnableStatus is FALSE), set the Warning icon 
            'SetTestSampleStatusIcon()

            'LocalTestDS.tparTests.DefaultView.Sort = "EnableStatus, CalibratorID, TestName"
            'CalibTestSampleListGrid.DataSource = LocalTestDS.tparTests.DefaultView
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".FillCalibratorTestSampleList ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".FillCalibratorTestSampleList ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Go through all the rows on the LocalTestDS to validate the EnableStatus and set the warning icon for those 
    ''' Tests/SampleTypes not linked to any Calibrator
    ''' </summary>
    ''' <remarks>
    ''' Created by:  TR 18/02/2011
    ''' Modified BY: TR 12/12/2011 - Changed the OK Icon for the Empty Icon (reported on BugTracking #229: do not shown
    '''                              the OK Icon, shown only the Warning Icon if the Test/SampleType is incomplete)
    '''              DL 22/03/2012
    ''' </remarks>
    Private Sub SetTestSampleStatusIcon()
        Try
            Dim EmptyImageBytes As Byte()
            Dim WarningImageBytes As Byte()
            Dim myPreloadeDelegate As New PreloadedMasterDataDelegate

            LocalTestDS.AcceptChanges()
            WarningImageBytes = myPreloadeDelegate.GetIconImage("ERRPROVIDER")  '("WARNINGSMALL") DL 22/03/2012
            EmptyImageBytes = myPreloadeDelegate.GetIconImage("EMPTYIMAGE")

            'Set the incomplete icon in case Test Sample is incomplete.
            For Each TestRow As TestsDS.tparTestsRow In LocalTestDS.tparTests.Rows
                If (Not TestRow.IsEnableStatusNull) Then
                    TestRow.Type = If(TestRow.EnableStatus, EmptyImageBytes, WarningImageBytes)
                End If
            Next
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".SetTestSampleStatusIcon ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".SetTestSampleStatusIcon ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Fill the ComboBox of Calibrators in the Calibrator by Test/SampleType TAB
    ''' </summary>
    ''' <remarks>
    ''' Created by:  TR 15/02/2011
    ''' </remarks>
    Private Sub FillCalibratorListCombo()
        Try
            Dim myGlobalDataTO As New GlobalDataTO
            Dim myCalibratorDS As New CalibratorsDS
            Dim myCalibratorDelegate As New CalibratorsDelegate

            'Get all Calibrators
            myGlobalDataTO = myCalibratorDelegate.GetAllCalibrators(Nothing)
            If (Not myGlobalDataTO.HasError) Then
                myCalibratorDS = DirectCast(myGlobalDataTO.SetDatos, CalibratorsDS)
            Else
                ShowMessage("Error", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage)
            End If

            CalibratorListCombo.DataSource = myCalibratorDS.tparCalibrators
            CalibratorListCombo.DisplayMember = "CalibratorName"
            CalibratorListCombo.ValueMember = "CalibratorID"

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".FillCalibratorListCombo ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".FillCalibratorListCombo ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Fill all ComboBoxes in the Calibration Curve Values area in the Calibrator by Test/SampleType TAB
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub FillCurveControls()
        Try
            Dim myGlobalDataTO As New GlobalDataTO
            Dim myPreloadedDataDelegate As New PreloadedMasterDataDelegate
            Dim myPreloadedDataDS As New PreloadedMasterDataDS

            'Load the ComboBox of Curve Types 
            myGlobalDataTO = myPreloadedDataDelegate.GetList(Nothing, GlobalEnumerates.PreloadedMasterDataEnum.CURVE_TYPES)
            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                myPreloadedDataDS = DirectCast(myGlobalDataTO.SetDatos, PreloadedMasterDataDS)

                'Sort data by field Position
                Dim qPreloadedMasterData As List(Of PreloadedMasterDataDS.tfmwPreloadedMasterDataRow)
                qPreloadedMasterData = (From a As PreloadedMasterDataDS.tfmwPreloadedMasterDataRow In myPreloadedDataDS.tfmwPreloadedMasterData _
                                      Select a Order By a.Position).ToList()

                CurveTypeCombo.DataSource = qPreloadedMasterData
                CurveTypeCombo.DisplayMember = "FixedItemDesc"
                CurveTypeCombo.ValueMember = "ItemID"
            Else
                'Error getting the list of Curve Types; shown it
                ShowMessage(Me.Name & ".FillCurveControls ", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage)
            End If

            'Load the ComboBoxes for Curve Axis Types
            myGlobalDataTO = myPreloadedDataDelegate.GetList(Nothing, GlobalEnumerates.PreloadedMasterDataEnum.CURVE_AXIS_TYPES)
            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                myPreloadedDataDS = DirectCast(myGlobalDataTO.SetDatos, PreloadedMasterDataDS)

                Dim qPreloadedMasterData As List(Of PreloadedMasterDataDS.tfmwPreloadedMasterDataRow)
                qPreloadedMasterData = (From a As PreloadedMasterDataDS.tfmwPreloadedMasterDataRow In myPreloadedDataDS.tfmwPreloadedMasterData _
                                      Select a Order By a.Position).ToList()

                If (qPreloadedMasterData.Count > 0) Then
                    'X AXIS
                    XAxisCombo.DataSource = qPreloadedMasterData
                    XAxisCombo.DisplayMember = "FixedItemDesc"
                    XAxisCombo.ValueMember = "ItemID"

                    'Y AXIS
                    Dim qPreloadedMasterData2 As New List(Of PreloadedMasterDataDS.tfmwPreloadedMasterDataRow)
                    qPreloadedMasterData2.AddRange(qPreloadedMasterData)

                    YAxisCombo.DataSource = qPreloadedMasterData2
                    YAxisCombo.DisplayMember = "FixedItemDesc"
                    YAxisCombo.ValueMember = "ItemID"
                Else
                    'Error getting the list of Curve Axis Types; shown it
                    ShowMessage(Me.Name & ".FillCurveControls ", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage)
                End If
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".FillCurveControls ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".FillCurveControls ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Bind Curve Values controls to selected TestSample Calibrator.
    ''' </summary>
    ''' <param name="pTestID">Test Identifier</param>
    ''' <param name="pSampleType">Sample Type Code</param>
    ''' <remarks>
    ''' Created by: TR 15/02/2011
    ''' </remarks>
    Private Sub BindCalTestSampleCurveValues(ByVal pTestID As Integer, ByVal pSampleType As String)
        Try

            Dim myGlobalDataTO As New GlobalDataTO
            Dim myTestCalibratorsDS As New TestCalibratorsDS
            Dim myTestCalibratorsDelgate As New TestCalibratorsDelegate

            If IsTestParamWinAttribute Then
                'validate if is the recived test name and sample type
                If pTestID = ExternalTestIDAttribute AndAlso String.Compare(pSampleType, ExternalSampleTypeAttribute, False) = 0 Then
                    myGlobalDataTO.SetDatos = UpdatedTestCalibratorsDS
                Else
                    myGlobalDataTO = myTestCalibratorsDelgate.GetTestCalibratorByTestID(Nothing, pTestID, pSampleType)
                End If
            Else
                myGlobalDataTO = myTestCalibratorsDelgate.GetTestCalibratorByTestID(Nothing, pTestID, pSampleType)
            End If

            If Not myGlobalDataTO.HasError Then
                myTestCalibratorsDS = DirectCast(myGlobalDataTO.SetDatos, TestCalibratorsDS)
                If myTestCalibratorsDS.tparTestCalibrators.Count > 0 Then

                    Dim myLocalCalibratorDS As New CalibratorsDS
                    If Not myTestCalibratorsDS.tparTestCalibrators(0).IsCalibratorIDNull Then
                        'Get the calibrator value to validate the calibrator number.
                        myLocalCalibratorDS = GetLocalCalibratorInfo(myTestCalibratorsDS.tparTestCalibrators(0).CalibratorID)
                    End If

                    'Validate the calibrator number.
                    If myLocalCalibratorDS.tparCalibrators.Count > 0 AndAlso myLocalCalibratorDS.tparCalibrators(0).NumberOfCalibrators > 1 Then
                        'Set the Curve Type.
                        If Not myTestCalibratorsDS.tparTestCalibrators(0).IsCurveTypeNull AndAlso _
                                                String.Compare(myTestCalibratorsDS.tparTestCalibrators(0).CurveType, "", False) <> 0 Then
                            CurveTypeCombo.SelectedValue = myTestCalibratorsDS.tparTestCalibrators(0).CurveType
                        Else
                            CurveTypeCombo.SelectedIndex = -1
                        End If
                        'Set the Curve Growth .
                        Select Case myTestCalibratorsDS.tparTestCalibrators(0).CurveGrowthType
                            Case "INC"
                                IncreasingRadioButton.Checked = True
                            Case "DEC"
                                DecreasingRadioButton.Checked = True
                            Case Else
                                IncreasingRadioButton.Checked = False
                                DecreasingRadioButton.Checked = False
                        End Select
                        'Set the X Axis value. 
                        If Not myTestCalibratorsDS.tparTestCalibrators(0).IsCurveAxisXTypeNull AndAlso _
                                        myTestCalibratorsDS.tparTestCalibrators(0).CurveAxisXType <> "" Then
                            XAxisCombo.SelectedValue = myTestCalibratorsDS.tparTestCalibrators(0).CurveAxisXType
                        Else
                            XAxisCombo.SelectedIndex = -1
                        End If
                        'Set the Y Axis value.
                        If Not myTestCalibratorsDS.tparTestCalibrators(0).IsCurveAxisYTypeNull AndAlso _
                                        myTestCalibratorsDS.tparTestCalibrators(0).CurveAxisYType <> "" Then
                            YAxisCombo.SelectedValue = myTestCalibratorsDS.tparTestCalibrators(0).CurveAxisYType
                        Else
                            YAxisCombo.SelectedIndex = -1
                        End If
                    Else
                        CurveTypeCombo.SelectedIndex = -1
                        IncreasingRadioButton.Checked = False
                        DecreasingRadioButton.Checked = False
                        XAxisCombo.SelectedIndex = -1
                        YAxisCombo.SelectedIndex = -1
                    End If
                Else
                    CurveTypeCombo.SelectedIndex = -1
                    IncreasingRadioButton.Checked = False
                    DecreasingRadioButton.Checked = False
                    XAxisCombo.SelectedIndex = -1
                    YAxisCombo.SelectedIndex = -1
                End If
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".BindCalTestSampleCurveValues ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".BindCalTestSampleCurveValues ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Configure the grid of Concentration Values according the selected TestID/SampleType and Calibrator
    ''' </summary>
    ''' <param name="pTestCalibratorID">Identifier of the link between the TestID/SampleType and the Calibrator</param>
    ''' <param name="pTestID">Test Identifier</param>
    ''' <param name="pSampleType">Sample Type Code</param>
    ''' <param name="pIsSpecialTest">When TRUE, it indicates there are special settings defined for the TestID/SampleType</param>
    ''' <remarks>
    ''' Created by:  TR
    ''' Modified by: SA 09/05/2012 - Added parameter to indicate if the TestID/SampleType is marked as Special Test
    '''                              For Special Tests, verify if settings TOTAL_CAL_POINTS and CAL_POINT_USED are defined and 
    '''                              configure the Concentration grid using these values instead of the Number of Points defined
    '''                              for the selected Calibrator
    ''' </remarks>
    Private Sub BindConcentrationValuesList(ByVal pTestCalibratorID As Integer, ByVal pTestID As Integer, ByVal pSampleType As String, _
                                            ByVal pIsSpecialTest As Boolean)
        Try
            Dim myGlobalDataTO As New GlobalDataTO
            Dim myTestCalibratorValuesDelegate As New TestCalibratorValuesDelegate()

            If (IsTestParamWinAttribute AndAlso pTestID = ExternalTestIDAttribute) Then
                Dim myTemUpdateTestCalValDS As New TestCalibratorValuesDS
                UpdatedTestCalibratorsValuesDS.tparTestCalibratorValues.CopyToDataTable(myTemUpdateTestCalValDS.tparTestCalibratorValues, LoadOption.OverwriteChanges)
                myGlobalDataTO.SetDatos = myTemUpdateTestCalValDS
            Else
                'Get the test calibrator values from DB 
                myGlobalDataTO = myTestCalibratorValuesDelegate.GetTestCalibratorValuesByTestCalibratorIDAndTestID(Nothing, pTestCalibratorID, pTestID)
            End If

            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                LocalConcentrationValuesDS = DirectCast(myGlobalDataTO.SetDatos, TestCalibratorValuesDS)

                'If there is not Concentration Values saved, then the grid is prepared with a number of rows equal to the 
                'Number of Points of the selected Calibrator
                If (LocalConcentrationValuesDS.tparTestCalibratorValues.Count = 0) Then
                    Dim myPointUsed As Integer = 0
                    Dim myCalibNumber As Integer = 0

                    If (pIsSpecialTest) Then
                        'Get the list of Special Settings defined for the special TestID/SampleType
                        Dim mySpecialSettingsDelegate As New SpecialTestsSettingsDelegate

                        myGlobalDataTO = mySpecialSettingsDelegate.Read(Nothing, pTestID, pSampleType)
                        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                            Dim mySpecialSettingsDS As SpecialTestsSettingsDS = DirectCast(myGlobalDataTO.SetDatos, SpecialTestsSettingsDS)

                            If (mySpecialSettingsDS.tfmwSpecialTestsSettings.Count > 0) Then
                                'Verify if setting TOTAL_CAL_POINTS is defined for the Special Test
                                Dim mySettingRow As List(Of SpecialTestsSettingsDS.tfmwSpecialTestsSettingsRow) = (From a In mySpecialSettingsDS.tfmwSpecialTestsSettings _
                                                                                                                  Where a.SettingName = "TOTAL_CAL_POINTS" _
                                                                                                                 Select a).ToList()
                                If (mySettingRow.Count = 1) Then
                                    myCalibNumber = Convert.ToInt32(mySettingRow.First.SettingValue)

                                    'Verify if setting CAL_POINT_USED is defined for the Special Test
                                    mySettingRow = (From a In mySpecialSettingsDS.tfmwSpecialTestsSettings _
                                                   Where a.SettingName = "CAL_POINT_USED" _
                                                  Select a).ToList()
                                    If (mySettingRow.Count = 1) Then myPointUsed = Convert.ToInt32(mySettingRow.First.SettingValue)
                                End If
                            End If
                        End If
                    End If

                    'Get the Calibrator information from the local CalibratorDS
                    Dim myCalibratorID As Integer = 0
                    If (Not Me.CalibratorListCombo.SelectedValue Is Nothing) Then
                        myCalibratorID = CInt(Me.CalibratorListCombo.SelectedValue.ToString())
                    End If

                    Dim qCalibratorList As List(Of CalibratorsDS.tparCalibratorsRow) = (From a In LocalCalibratorDS.tparCalibrators _
                                                                                       Where a.CalibratorID = myCalibratorID _
                                                                                      Select a).ToList()
                    If (qCalibratorList.Count > 0) Then
                        If (myCalibNumber = 0) Then myCalibNumber = qCalibratorList.First().NumberOfCalibrators

                        Dim myConcValRow As TestCalibratorValuesDS.tparTestCalibratorValuesRow
                        For i As Integer = myCalibNumber To 1 Step -1
                            myConcValRow = LocalConcentrationValuesDS.tparTestCalibratorValues.NewtparTestCalibratorValuesRow()
                            myConcValRow.TestCalibratorID = pTestCalibratorID
                            myConcValRow.CalibratorNum = i
                            myConcValRow.IsNew = True
                            LocalConcentrationValuesDS.tparTestCalibratorValues.AddtparTestCalibratorValuesRow(myConcValRow)
                        Next

                        If (pIsSpecialTest AndAlso myCalibNumber = 1 AndAlso myPointUsed <> 0) Then
                            LocalConcentrationValuesDS.BeginInit()
                            LocalConcentrationValuesDS.tparTestCalibratorValues.First.CalibratorNum = myPointUsed
                            LocalConcentrationValuesDS.EndInit()
                        End If
                    End If
                Else
                    'Inform the local variable containing the Theoretical Concentration value for the first Calibrator Point
                    LocalFirstPointValue = LocalConcentrationValuesDS.tparTestCalibratorValues.First.TheoricalConcentration
                End If

                ConcentrationGridView.DataSource = Nothing
                ConcentrationGridView.DataSource = LocalConcentrationValuesDS.tparTestCalibratorValues
                ConcentrationGridView.ClearSelection()
            Else
                ShowMessage(Me.Name & ".BindConcentrationValuesList ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, myGlobalDataTO.ErrorCode, Me)
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".BindConcentrationValuesList ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".BindConcentrationValuesList ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Configure the edition area when the selected tab is Tests/Sample Types
    ''' </summary>
    ''' <param name="pTestCalibratorID">Identifier of the link between the TestID/SampleType and the Calibrator</param>
    ''' <param name="pTestID">Test Identifier</param>
    ''' <param name="pSampleType">Sample Type Code</param>
    ''' <param name="pCalibratorID">Calibrator Identifier</param>
    ''' <remarks>
    ''' Created by:  TR
    ''' Modified by: TR 09/03/2011 - Added verification of Test/SampleType with Factory Calibrator values to shown the Warning Msg
    '''              SA 10/05/2012 - Verify if the selected Test/SampleType is marked as Special Test before calling function BindConcentrationValuesList
    ''' </remarks>
    Private Sub BindCalibratorValues(ByVal pTestCalibratorID As Integer, ByVal pTestID As Integer, ByVal pSampleType As String, ByVal pCalibratorID As Integer)
        Try
            'Select the Calibrator on the ComboBox containing the list of available Calibrators
            CalibratorListCombo.SelectedValue = pCalibratorID

            'Inform fields in area of Calibrator Curve
            BindCalTestSampleCurveValues(pTestID, pSampleType)

            'Inform the Concentration Values; first verify if the selected TestID/SampleType is marked as special
            Dim isSpecialTest As Boolean = False
            If (CalibTestSampleListGrid.SelectedRows.Count = 1) AndAlso (String.Compare(CalibratorTabControl.SelectedTab.Name, "CalibratorTestTab", False) = 0) Then
                If (CalibTestSampleListGrid.SelectedRows(0).Cells("SpecialTest").Value Is DBNull.Value) Then
                    isSpecialTest = False
                Else
                    isSpecialTest = Convert.ToBoolean(CalibTestSampleListGrid.SelectedRows(0).Cells("SpecialTest").Value)
                End If
            End If
            BindConcentrationValuesList(pTestCalibratorID, pTestID, pSampleType, isSpecialTest)

            If (CalibTestSampleListGrid.SelectedRows.Count = 1) AndAlso (CalibratorTabControl.SelectedTab.Name = "CalibratorTestTab") AndAlso _
               (LocalScreenStatus = SCREEN_STATUS_TYPE.ReadMode) Then
                If (PrevSelectedTest <> CalibTestSampleListGrid.SelectedRows(0).Cells("TestName").Value.ToString()) Then
                    PrevSelectedTest = CalibTestSampleListGrid.SelectedRows(0).Cells("TestName").Value.ToString()
                    'TR 22/07/2013  -BUG #1229 Not need to validate factory calibrator if comming from test programming screem, 
                    'message was shown on previouws windows. Uncomment this if.
                    If Not (IsTestParamWinAttribute) Then
                        If (ValidateCalibratorFactoryValues(CInt(CalibTestSampleListGrid.SelectedRows(0).Cells("TestID").Value), _
                                                            CalibTestSampleListGrid.SelectedRows(0).Cells("SampleType").Value.ToString())) Then
                            ShowMessage(Me.Name & ".BindCalibratorValues ", GlobalEnumerates.Messages.FACTORY_VALUES.ToString())
                        End If
                    End If
                End If
            End If

            If (IsTestParamWinAttribute) Then
                If (pTestID = ExternalTestIDAttribute AndAlso pSampleType = ExternalSampleTypeAttribute) Then
                    'Get the Calibrator and assign it to the received Test/SampleType 
                    Dim qCalibList As List(Of CalibratorsDS.tparCalibratorsRow) = (From a In LocalCalibratorDS.tparCalibrators _
                                                                                  Where a.CalibratorID = pCalibratorID _
                                                                                 Select a).ToList()

                    'If calibrator found and is on update table then replace found for updated
                    If (qCalibList.Count > 0 AndAlso UpdatedCalibratorsDS.tparCalibrators.Count > 0) Then
                        qCalibList.First().ItemArray = UpdatedCalibratorsDS.tparCalibrators(0).ItemArray
                    End If
                    CalibTestSampleListGrid.Refresh()
                End If
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".BindCalibratorValues ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".BindCalibratorValues ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Validate the calibrator information 
    ''' </summary>
    ''' <returns>
    ''' Return true if there're any error and false if OK
    ''' </returns>
    ''' <remarks>
    ''' Created by: TR 14/02/2011
    ''' </remarks>
    Private Function ValidateCalibratorDataError() As Boolean
        Dim myResult As Boolean = False
        Try
            If LocalScreenStatus = SCREEN_STATUS_TYPE.CreateMode OrElse LocalScreenStatus = SCREEN_STATUS_TYPE.EditMode Then
                'clear all the error marks
                CalibratorErrorProv.Clear()

                If CalibratorNameTextBox.Text = "" Then
                    CalibratorErrorProv.SetError(CalibratorNameTextBox, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString()))
                    myResult = True
                ElseIf CalibratorExist(CalibratorNameTextBox.Text, CInt(CalibIDTextBox.Text)) Then
                    CalibratorErrorProv.SetError(CalibratorNameTextBox, GetMessageText(GlobalEnumerates.Messages.DUPLICATED_CALIB_NAME.ToString()))
                    myResult = True
                End If

                If LotNumberTextBox.Text = "" Then
                    CalibratorErrorProv.SetError(LotNumberTextBox, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString()))
                    myResult = True
                End If

                If String.Compare(CalibNumberUpDown.Text, "", False) = 0 Then
                    CalibratorErrorProv.SetError(CalibNumberUpDown, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString()))
                    myResult = True
                End If

                If ExpDatePickUpCombo.Value <= DateTime.Now Then
                    CalibratorErrorProv.SetError(ExpDatePickUpCombo, GetMessageText(GlobalEnumerates.Messages.INVALIDDATE.ToString()))
                    ExpDatePickUpCombo.Select()
                    myResult = True
                End If

            End If

        Catch ex As Exception
            'Write error SYSTEM_ERROR in the Application Log
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " ValidateCalibratorData ", EventLogEntryType.Error, _
                                                                    GetApplicationInfoSession().ActivateSystemLog)
            'Show error message
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
        Return myResult
    End Function

    ''' <summary>
    ''' Validate if there are any test-Sample Assigned to the Calibrator 
    ''' </summary>
    ''' <param name="pCalibratorIDList">Calibrator list to validate.</param>
    ''' <returns></returns>
    ''' <remarks>
    ''' Created by: TR 14/02/2011
    ''' </remarks>
    Private Function ValidateCalibratorTestSampleDependencies(ByVal pCalibratorIDList As List(Of Integer)) As DialogResult
        Dim myResult As DialogResult
        myResult = DialogResult.OK
        Try
            Dim imageBytes As Byte()
            Dim myGlobalDataTO As New GlobalDataTO
            Dim myPreloadeDelegate As New PreloadedMasterDataDelegate
            Dim myTestSampleCalibratorDS As New TestSampleCalibratorDS
            Dim myDependenciesElementsDS As New DependenciesElementsDS
            Dim myTestCalibratorDelegate As New TestCalibratorsDelegate
            Dim myDepElemenRow As DependenciesElementsDS.DependenciesElementsRow

            For Each myCalibratorID As Integer In pCalibratorIDList
                myGlobalDataTO = myTestCalibratorDelegate.GetAllTestCalibratorByCalibratorID(Nothing, myCalibratorID)

                If Not myGlobalDataTO.HasError Then
                    myTestSampleCalibratorDS = DirectCast(myGlobalDataTO.SetDatos, TestSampleCalibratorDS)

                    imageBytes = myPreloadeDelegate.GetIconImage("TESTICON")
                    For Each myTestSampleRow As TestSampleCalibratorDS.tparTestCalibratorsRow In _
                                                        myTestSampleCalibratorDS.tparTestCalibrators.Rows

                        myDepElemenRow = myDependenciesElementsDS.DependenciesElements.NewDependenciesElementsRow()
                        myDepElemenRow.Type = imageBytes
                        myDepElemenRow.Name = myTestSampleRow.TestName

                        myDependenciesElementsDS.DependenciesElements.AddDependenciesElementsRow(myDepElemenRow)
                    Next
                End If
            Next

            If myDependenciesElementsDS.DependenciesElements.Count > 0 Then
                Using myAffectedElementsWarning As New UiWarningAfectedElements()
                    myAffectedElementsWarning.AffectedElements = myDependenciesElementsDS
                    myAffectedElementsWarning.AdditionalElements = True
                    'TODO: change message to a new one.
                    myAffectedElementsWarning.ElementsAffectedMessageDetail = "MSG_UPDATE_CONCENTRATION"

                    myAffectedElementsWarning.ShowDialog()
                    myResult = myAffectedElementsWarning.DialogResult
                End Using
            End If



        Catch ex As Exception
            'Write error SYSTEM_ERROR in the Application Log
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " ValidateCalibratorTestSampleDependencies ", EventLogEntryType.Error, _
                                                                    GetApplicationInfoSession().ActivateSystemLog)
            'Show error message
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
        Return myResult
    End Function

    ''' <summary>
    ''' Validate there are previous concentration values and indicate that there values will be deleted.
    ''' </summary>
    ''' <param name="pTestID">TestID </param>
    ''' <param name="pSampleType">Sample Type </param>
    ''' <param name="pTestVersionNumber">Test version Number</param>
    ''' <returns></returns>
    ''' <remarks>
    ''' Created by: TR 21/02/2011
    ''' </remarks>
    Private Function ValidateDependecies(ByVal pTestID As Integer, ByVal pSampleType As String, ByVal pTestVersionNumber As Integer) As DialogResult

        Dim myAffectedElementsResults As DialogResult = Windows.Forms.DialogResult.None

        Try
            Dim myProfileMember As String = ""
            Dim myResultGlobalTO As New GlobalDataTO
            Dim myResultDelegate As New ResultsDelegate
            Dim myAdditionalElementsDS As New WSAdditionalElementsDS

            'Get the last executed calibrators
            myResultGlobalTO = myResultDelegate.GetLastExecutedCalibrator(Nothing, pTestID, pSampleType, pTestVersionNumber)

            If Not myResultGlobalTO.HasError Then
                'Go throught each element and set the absorbance value into myProfilemember variable, 
                'then add it into the myAdditionalelementsDS
                For Each ResultRow As WSAdditionalElementsDS.WSAdditionalElementsTableRow In _
                                  DirectCast(myResultGlobalTO.SetDatos, WSAdditionalElementsDS).WSAdditionalElementsTable.Rows

                    If ResultRow.IsCalibratorFactorNull Then
                        myProfileMember &= ResultRow.ABSValue.ToString() & vbCrLf
                    Else
                        myProfileMember &= ResultRow.ABSValue.ToString() & " (" & ResultRow.CalibratorFactor.ToString() & ")" & vbCrLf
                    End If
                    myAdditionalElementsDS.WSAdditionalElementsTable.ImportRow(ResultRow)

                Next

                Dim myDependenciesElementsDS As New DependenciesElementsDS
                'if additionale elements has row then star preparing the dependecieselementsDS.
                If myAdditionalElementsDS.WSAdditionalElementsTable.Count > 0 Then
                    Dim myDepElemenRow As DependenciesElementsDS.DependenciesElementsRow
                    Dim myPreloadeDelegate As New PreloadedMasterDataDelegate
                    Dim imageBytes As Byte()
                    imageBytes = myPreloadeDelegate.GetIconImage("CALIB")
                    Dim myCalibratorsDS As New CalibratorsDS

                    Dim qCalibratorList As New List(Of CalibratorsDS.tparCalibratorsRow)
                    For Each ResultRow As WSAdditionalElementsDS.WSAdditionalElementsTableRow In _
                                                    myAdditionalElementsDS.WSAdditionalElementsTable.Rows
                        myDepElemenRow = myDependenciesElementsDS.DependenciesElements.NewDependenciesElementsRow()
                        myDepElemenRow.Type = imageBytes

                        'TR 26/07/2013 -commented line under: The display name was not correct need the calibrator name not the sampleclass to show.
                        'myDepElemenRow.Name = ResultRow.SampleClass 

                        'TR 26/07/2013 -BUG #921 Get calibrator name to show on Affected elements warning screen.
                        qCalibratorList = (From a In LocalCalibratorDS.tparCalibrators _
                                           Where a.CalibratorID = CInt(CalibTestSampleListGrid.SelectedRows(0).Cells("CalibratorID").Value) _
                                           Select a).ToList()
                        If qCalibratorList.Count > 0 Then
                            myDepElemenRow.Name = qCalibratorList.First().CalibratorName
                        End If
                        'TR 26/07/2013 -END.

                        If String.Compare(ResultRow.SampleClass, "CALIB", False) = 0 Then
                            myDepElemenRow.FormProfileMember = myProfileMember.TrimEnd()
                            myDepElemenRow.ResultDateTime = ResultRow.ResultDateTime
                            myDependenciesElementsDS.DependenciesElements.AddDependenciesElementsRow(myDepElemenRow)
                            Exit For
                        End If

                    Next

                    Using myAffectedElementsWarning As New UiWarningAfectedElements()
                        myAffectedElementsWarning.AffectedElements = myDependenciesElementsDS
                        myAffectedElementsWarning.AdditionalElements = True

                        myAffectedElementsWarning.ShowDialog()
                        myAffectedElementsResults = myAffectedElementsWarning.DialogResult
                    End Using

                End If
            End If

        Catch ex As Exception
            'Write error SYSTEM_ERROR in the Application Log & show error message
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " ValidateDependecies ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
        Return myAffectedElementsResults
    End Function

    Private Sub PrepareNewCalibratorValuesForTestSample()
        Try
            Dim myTestCalibratorID As Integer = 0
            If CalibTestSampleListGrid.SelectedRows.Count > 0 Then
                myTestCalibratorID = CInt(CalibTestSampleListGrid.SelectedRows(0).Cells("TestCalibratorID").Value.ToString())
            End If

            Dim myCalibratorDS As New CalibratorsDS
            'Get selected calibrator information 
            myCalibratorDS = GetLocalCalibratorInfo(CInt(Me.CalibratorListCombo.SelectedValue))

            If myCalibratorDS.tparCalibrators.Count > 0 Then
                'Clean the Cuve values.
                CleanCurveValue()
                'Validate the number of point to enable or disable the curve data area
                If myCalibratorDS.tparCalibrators(0).NumberOfCalibrators > 1 Then
                    'Enable curve area.
                    'DL 19/07/2011
                    'CalibCurveInfoGroupBox.Enabled = True
                    EnableDisableCalibCurveInfoGroupBox(True)
                    'DL 19/07/2011

                    'TR 14/05/2012 -Set default values for multipoint. 
                    'Set defaults values New functionallity see bugtracking 403
                    Me.IncreasingRadioButton.Checked = True
                    Me.CurveTypeCombo.SelectedIndex = 0
                    Me.XAxisCombo.SelectedIndex = 0
                    Me.YAxisCombo.SelectedIndex = 0
                    'TR 14/05/2012 -END
                Else
                    'Disable curve area.
                    'DL 19/07/2011
                    'CalibCurveInfoGroupBox.Enabled = False
                    EnableDisableCalibCurveInfoGroupBox(False)
                    'DL 19/07/2011
                End If

                Dim myCalibratorNumber As Integer
                myCalibratorNumber = myCalibratorDS.tparCalibrators(0).NumberOfCalibrators
                'Clear all values
                LocalConcentrationValuesDS.tparTestCalibratorValues.Clear()

                'TR 08/03/2012 ' Accept change and refresh Concentration grid to avoid error on visual effects.
                LocalConcentrationValuesDS.tparTestCalibratorValues.AcceptChanges()
                ConcentrationGridView.Refresh()
                'TR 08/03/2012
                'Set the new numbers of calibrators to the concentration grid.
                Dim myCalConcNumbRow As TestCalibratorValuesDS.tparTestCalibratorValuesRow
                For i As Integer = myCalibratorNumber To 1 Step -1
                    myCalConcNumbRow = LocalConcentrationValuesDS.tparTestCalibratorValues.NewtparTestCalibratorValuesRow()
                    myCalConcNumbRow.TestCalibratorID = myTestCalibratorID
                    myCalConcNumbRow.CalibratorNum = i
                    myCalConcNumbRow.IsNew = True
                    LocalConcentrationValuesDS.tparTestCalibratorValues.AddtparTestCalibratorValuesRow(myCalConcNumbRow)
                Next
                ConcentrationGridView.CurrentRow.Selected = False
            End If
        Catch ex As Exception
            'Write error SYSTEM_ERROR in the Application Log
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " PrepareNewCalibratorValuesForTestSample ", EventLogEntryType.Error, _
                                                                GetApplicationInfoSession().ActivateSystemLog)
            'Show error message
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Search on the local calibrator table an specific calibrator by the 
    ''' calibrator id
    ''' </summary>
    ''' <param name="pCalibratorID">Calibrator ID.</param>
    ''' <returns></returns>
    ''' <remarks>
    ''' Created by: TR 18/02/2011
    ''' </remarks>
    Private Function GetLocalCalibratorInfo(ByVal pCalibratorID As Integer) As CalibratorsDS
        Dim myResultCalibraorDS As New CalibratorsDS
        Try
            Dim qCalibratorList As New List(Of CalibratorsDS.tparCalibratorsRow)
            qCalibratorList = (From a In LocalCalibratorDS.tparCalibrators _
                               Where a.CalibratorID = pCalibratorID _
                               Select a).ToList()

            If qCalibratorList.Count > 0 Then
                For Each CalibRow As CalibratorsDS.tparCalibratorsRow In qCalibratorList
                    myResultCalibraorDS.tparCalibrators.ImportRow(CalibRow)
                Next
            End If

        Catch ex As Exception
            'Write error SYSTEM_ERROR in the Application Log
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " GetLocalCalibratorInfo ", EventLogEntryType.Error, _
                                                                GetApplicationInfoSession().ActivateSystemLog)
            'Show error message
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
        Return myResultCalibraorDS
    End Function

    ''' <summary>
    ''' Clean the controls values on the Curve Values area.
    ''' </summary>
    ''' <remarks>
    ''' Created by: TR 18/02/2011
    '''             TR 14/05/2012 -Commented the value asignation and leave the control value cleaning.
    ''' </remarks>
    Private Sub CleanCurveValue()
        Try
            'TR 08/03/2012 - New functionality See BugTracking 403
            ''Before removing values validate if there are any memorize value
            'If Not IncreasingRadioButton.Checked AndAlso Not DecreasingRadioButton.Checked Then
            '    Me.IncreasingRadioButton.Checked = True
            '    Me.CurveTypeCombo.SelectedIndex = 0
            '    Me.XAxisCombo.SelectedIndex = 0
            '    Me.YAxisCombo.SelectedIndex = 0
            'Else
            Me.IncreasingRadioButton.Checked = False
            Me.DecreasingRadioButton.Checked = False
            Me.CurveTypeCombo.SelectedIndex = -1
            Me.XAxisCombo.SelectedIndex = -1
            Me.YAxisCombo.SelectedIndex = -1

            'End If
        Catch ex As Exception
            'Write error SYSTEM_ERROR in the Application Log
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " CleanCurveValue ", EventLogEntryType.Error, _
                                                                GetApplicationInfoSession().ActivateSystemLog)
            'Show error message
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try

    End Sub

    ''' <summary>
    ''' Validate if there are errors on the Calibrator Curve Values or in Concentration Values
    ''' </summary>
    ''' <returns>True if an error was found; False if all values are correct</returns>
    ''' <remarks>
    ''' Created by: TR 18/02/2011
    ''' </remarks>
    Private Function ValidateCalibCurveAndConcValuesHasError() As Boolean
        Dim myResult As Boolean = False
        Try
            myResult = ValidateCalibratorCurveValuesHasError() OrElse _
                       ValidateErrorConcValuesDescOrderGrid()
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ValidaterCalibCurveAndConcValuesHasError ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ValidaterCalibCurveAndConcValuesHasError ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
        Return myResult
    End Function

    ''' <summary>
    ''' Validate the calibrator curve values
    ''' </summary>
    ''' <returns>
    ''' True if error found or False if ok
    ''' </returns>
    ''' <remarks>
    ''' Created by:  TR 21/02/2011
    ''' </remarks>
    Private Function ValidateCalibratorCurveValuesHasError() As Boolean
        Dim myResult As Boolean = False
        Try
            CalibratorErrorProv.Clear()
            'Validate there are test selected on the test detail grid.
            If CalibTestSampleListGrid.SelectedRows.Count > 0 AndAlso CurveTypeCombo.Enabled Then ' DL 19/07/2011  CalibCurveInfoGroupBox.Enabled Then

                If Not CalibTestSampleListGrid.SelectedRows(0).Cells("SpecialTest").Value Is DBNull.Value Then

                    If Not CType(CalibTestSampleListGrid.SelectedRows(0).Cells("SpecialTest").Value, Boolean) Then

                        If Not IncreasingRadioButton.Checked And Not DecreasingRadioButton.Checked Then
                            CalibratorErrorProv.SetError(DecreasingRadioButton, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString()))
                            myResult = True
                        End If

                        If CurveTypeCombo.SelectedIndex < 0 Then
                            CalibratorErrorProv.SetError(CurveTypeCombo, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString()))
                            myResult = True
                        End If

                        'XAxisCombo
                        If XAxisCombo.SelectedIndex < 0 Then
                            CalibratorErrorProv.SetError(XAxisCombo, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString()))
                            myResult = True
                        End If

                        'YAxisCombo
                        If YAxisCombo.SelectedIndex < 0 Then
                            CalibratorErrorProv.SetError(YAxisCombo, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString()))
                            myResult = True
                        End If
                    End If

                Else
                    If Not IncreasingRadioButton.Checked And Not DecreasingRadioButton.Checked Then
                        CalibratorErrorProv.SetError(DecreasingRadioButton, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString()))
                        myResult = True
                    End If

                    If CurveTypeCombo.SelectedIndex < 0 Then
                        CalibratorErrorProv.SetError(CurveTypeCombo, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString()))
                        myResult = True
                    End If

                    'XAxisCombo
                    If XAxisCombo.SelectedIndex < 0 Then
                        CalibratorErrorProv.SetError(XAxisCombo, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString()))
                        myResult = True
                    End If

                    'YAxisCombo
                    If YAxisCombo.SelectedIndex < 0 Then
                        CalibratorErrorProv.SetError(YAxisCombo, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString()))
                        myResult = True
                    End If
                End If
            End If

        Catch ex As Exception
            'Write error SYSTEM_ERROR in the Application Log
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " ValidateCalibratorCurveValuesHasError ", EventLogEntryType.Error, _
                                                                    GetApplicationInfoSession().ActivateSystemLog)
            'Show error message
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
        Return myResult
    End Function

    ''' <summary>
    ''' Validate all Theorical Concentration values have been informed and they are in descendent order
    ''' </summary>
    ''' <remarks>
    ''' Created by:  TR 18/02/2011
    ''' Modified by: TR 09/10/2011 - Changed all convert to INT by convert to DOUBLE
    ''' </remarks>
    Private Function ValidateErrorConcValuesDescOrderGrid() As Boolean
        Dim myResult As Boolean = False
        Try
            If (LocalScreenStatus = SCREEN_STATUS_TYPE.EditMode Or LocalScreenStatus = SCREEN_STATUS_TYPE.CreateMode) Then
                CalibratorErrorProv.Clear()

                Dim myRowIndex As Integer = 0
                For Each ConcentrationRow As DataGridViewRow In ConcentrationGridView.Rows
                    'Validate the Theorical Concentration is informed for the row and also that it is numeric
                    If (Not ConcentrationRow.Cells("TheoricalConcentration").Value Is DBNull.Value AndAlso _
                        IsNumeric(ConcentrationRow.Cells("TheoricalConcentration").Value.ToString())) Then
                        If (CDbl(ConcentrationRow.Cells("TheoricalConcentration").Value) <= 0) Then
                            myResult = True
                            Exit For
                        End If

                        If (myRowIndex > 0) Then
                            'Compare value with the Theorical Concentration on the previous row
                            If (CDbl(ConcentrationGridView.Rows(myRowIndex - 1).Cells("TheoricalConcentration").FormattedValue) <= _
                                CDbl(ConcentrationRow.Cells("TheoricalConcentration").FormattedValue)) Then
                                ConcentrationGridView.Rows(myRowIndex).Cells("TheoricalConcentration").Style.Alignment = DataGridViewContentAlignment.MiddleCenter
                                ConcentrationGridView.Rows(myRowIndex).Cells("TheoricalConcentration").ErrorText = GetMessageText(GlobalEnumerates.Messages.VALIDCONCENTRATIONVALUE.ToString())
                                myResult = True
                                Exit For
                            End If
                        End If

                    ElseIf (ConcentrationRow.Cells("TheoricalConcentration").Value Is DBNull.Value) Then
                        ConcentrationRow.Cells("TheoricalConcentration").ErrorText = GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString())
                        myResult = True
                        Exit For
                    End If

                    myRowIndex += 1
                Next
                ConcentrationGridView.Refresh()
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ValidateConcentrationsValuesGrid ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ValidateConcentrationsValuesGrid ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
        Return myResult
    End Function

    ''' <summary>
    ''' Update Test Sample Calibrator values
    ''' </summary>
    ''' <param name="pTestCalibratorID">Identifier of the relation between the Test/SampleType and the experimental Calibrator
    '''                                 When that relation is new, value of this parameter is -1</param>
    ''' <param name="pTestID">Test Identifier</param>
    ''' <param name="pSampleType">Sample Type Code</param>
    ''' <returns></returns>
    ''' <remarks>
    ''' Created by:  TR
    ''' Modified by: SA 08/11/2012 - Added optional parameters pTestID and pSampleType, which have to be informed when parameter 
    '''                              pTestCalibratorID is not informed (due to relation between the Test/SampleType and the Calibrator
    '''                              does not exist yet)
    '''                            - Delete parameters pDeleteCalibratorResult and pDeleteTestCalibratorValue due to they are not used
    ''' </remarks>
    Private Function SaveTestSampleCalibratorValues(ByVal pTestCalibratorID As Integer, Optional ByVal pTestID As Integer = -1, _
                                                    Optional ByVal pSampleType As String = "") As Boolean
        Dim myResult As Boolean = True

        Try
            'Prepare CalibratorDS
            Dim myCalibratorDS As New CalibratorsDS
            Dim qCalibratorList As List(Of CalibratorsDS.tparCalibratorsRow) = (From a As CalibratorsDS.tparCalibratorsRow In LocalCalibratorDS.tparCalibrators _
                                                                               Where a.CalibratorID = CInt(Me.CalibratorListCombo.SelectedValue.ToString()) _
                                                                              Select a).ToList()
            For Each calibRow As CalibratorsDS.tparCalibratorsRow In qCalibratorList
                myCalibratorDS.tparCalibrators.ImportRow(calibRow)
            Next

            'Prepare TestCalibratorsDS
            Dim myTestCalibratorsDS As New TestCalibratorsDS
            Dim qTesList As New List(Of TestsDS.tparTestsRow)

            If (pTestCalibratorID <> -1) Then
                'Search the Test/SampleType by the informed TestCalibratorID
                qTesList = (From a As TestsDS.tparTestsRow In LocalTestDS.tparTests _
                           Where a.TestCalibratorID = pTestCalibratorID _
                          Select a).ToList()
            Else
                'Search the Test/SampleType by TestID and SampleType
                qTesList = (From a As TestsDS.tparTestsRow In LocalTestDS.tparTests _
                           Where a.TestID = pTestID _
                         AndAlso a.SampleType = pSampleType _
                          Select a).ToList()
            End If

            Dim myTestCalibRow As TestCalibratorsDS.tparTestCalibratorsRow
            If (qTesList.Count > 0) Then
                For Each testRow As TestsDS.tparTestsRow In qTesList
                    myTestCalibRow = myTestCalibratorsDS.tparTestCalibrators.NewtparTestCalibratorsRow()
                    myTestCalibRow.TestCalibratorID = pTestCalibratorID
                    myTestCalibRow.TestID = testRow.TestID
                    myTestCalibRow.SampleType = testRow.SampleType
                    myTestCalibRow.CalibratorID = qCalibratorList.First().CalibratorID
                    myTestCalibRow.TestVersionNumber = testRow.TestVersionNumber
                    myTestCalibRow.IsNew = True

                    If (CurveTypeCombo.Enabled) Then
                        'For Multipoint Calibrators, inform all parameters for the Calibration Curve
                        If (IncreasingRadioButton.Checked) Then
                            myTestCalibRow.CurveGrowthType = "INC"
                        ElseIf (DecreasingRadioButton.Checked) Then
                            myTestCalibRow.CurveGrowthType = "DEC"
                        End If

                        myTestCalibRow.CurveType = CurveTypeCombo.SelectedValue.ToString()
                        myTestCalibRow.CurveAxisXType = XAxisCombo.SelectedValue.ToString()
                        myTestCalibRow.CurveAxisYType = YAxisCombo.SelectedValue.ToString()
                    Else
                        'For Single point Calibrators, all parameters for the Calibration Curve are set to NULL
                        myTestCalibRow.SetCurveGrowthTypeNull()
                        myTestCalibRow.SetCurveTypeNull()
                        myTestCalibRow.SetCurveAxisXTypeNull()
                        myTestCalibRow.SetCurveAxisYTypeNull()
                    End If

                    myTestCalibRow.TS_User = GetApplicationInfoSession.UserName
                    myTestCalibRow.TS_DateTime = DateTime.Now
                    myTestCalibratorsDS.tparTestCalibrators.AddtparTestCalibratorsRow(myTestCalibRow)

                    'Copy data to UpdatedTestDS and set EnableStatus to TRUE
                    UpdatedTestDS.tparTests.ImportRow(testRow)
                    UpdatedTestDS.tparTests(0).EnableStatus = True
                    UpdatedTestDS.tparTests(0).CalibratorName = qCalibratorList.First().CalibratorName
                    UpdatedTestDS.tparTests(0).CalibratorID = qCalibratorList.First().CalibratorID
                Next
            End If

            Dim myGlobalDataTO As New GlobalDataTO
            Dim SaveOnDataBase As Boolean = True

            If (IsTestParamWinAttribute) Then
                'Validate if the edited element is the received from Tests Programming Screen
                If (myTestCalibratorsDS.tparTestCalibrators(0).TestID = ExternalTestIDAttribute) Then
                    SaveOnDataBase = False
                End If
            End If

            If (SaveOnDataBase) Then
                Dim myCalibratorDelegate As New CalibratorsDelegate
                myGlobalDataTO = myCalibratorDelegate.Save(Nothing, myCalibratorDS, myTestCalibratorsDS, LocalConcentrationValuesDS, _
                                                           Nothing, False, True, False, AnalyzerIDAttribute, WorkSessionIDAttribute)
                If (Not myGlobalDataTO.HasError) Then
                    'Update the Status for the Test/SampleType
                    Dim myTestSampleDelegate As New TestSamplesDelegate
                    myGlobalDataTO = myTestSampleDelegate.UpdateTestSampleEnableStatus(Nothing, qTesList.First().TestID, _
                                                                                       qTesList.First().SampleType, True)

                    'Update flag Factory Calibration to FALSE
                    If (Not myGlobalDataTO.HasError) Then
                        myGlobalDataTO = myTestSampleDelegate.UpdateTestSampleFactoryCalib(Nothing, qTesList.First().TestID, _
                                                                                           qTesList.First().SampleType, False)
                    End If
                End If

                If (Not myGlobalDataTO.HasError) Then
                    qTesList.First().EnableStatus = True
                    LocalChange = False
                    LocalValidateDependencies = False 'TR 27/09/2012

                    LocalConcPrevValue = -1
                    LocalFirstPointValue = -1
                Else
                    myResult = False
                    ShowMessage("Error", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage, Me)
                End If
            Else
                'Import updated values to external extructures
                UpdatedCalibratorsDS.tparCalibrators.Clear()
                myCalibratorDS.tparCalibrators.CopyToDataTable(UpdatedCalibratorsDS.tparCalibrators, LoadOption.OverwriteChanges)

                UpdatedTestCalibratorsDS.tparTestCalibrators.Clear()
                myTestCalibratorsDS.tparTestCalibrators.CopyToDataTable(UpdatedTestCalibratorsDS.tparTestCalibrators, LoadOption.OverwriteChanges)
                UpdatedTestCalibratorsValuesDS.tparTestCalibratorValues.Clear()
                LocalConcentrationValuesDS.tparTestCalibratorValues.CopyToDataTable(UpdatedTestCalibratorsValuesDS.tparTestCalibratorValues, _
                                                                                    LoadOption.OverwriteChanges)

                LocalChangesToSendAttribute = True
                LocalChange = False

                LocalConcPrevValue = -1
                LocalFirstPointValue = -1
            End If
        Catch ex As Exception
            myResult = False
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".SaveTestSampleCalibratorValues ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".SaveTestSampleCalibratorValues ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
        Return myResult
    End Function

    ''' <summary>
    ''' Search on the local TestDS if there're any test with 
    ''' enablestatus = false and not calibrator asigned.
    ''' </summary>
    ''' <returns>
    ''' Return a lis of test with enableStatus false if exist.
    ''' </returns>
    ''' <remarks>
    '''Created by: TR 22/02/2011
    ''' </remarks>
    Private Function PendingTestSampleWithOutCalib() As List(Of TestsDS.tparTestsRow)
        Dim qTestList As New List(Of TestsDS.tparTestsRow)
        Try
            If Not LocalTestDS Is Nothing Then
                qTestList = (From a In LocalTestDS.tparTests _
                             Where a.EnableStatus = False AndAlso _
                             String.Compare(a("CalibratorName").ToString(), "", False) = 0 _
                             Select a).ToList()
            End If

        Catch ex As Exception
            'Write error SYSTEM_ERROR in the Application Log
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " PendingTestSampleCalibData ", EventLogEntryType.Error, _
                                                                    GetApplicationInfoSession().ActivateSystemLog)
            'Show error message
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
        Return qTestList
    End Function

    ''' <summary>
    ''' Search on le local testds all the test with status disable 
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>
    ''' Created by: TR 22/02/2011
    ''' </remarks>
    Private Function TestSampleWithDisabelStatus() As List(Of TestsDS.tparTestsRow)
        Dim qTestList As New List(Of TestsDS.tparTestsRow)
        Try
            qTestList = (From a In LocalTestDS.tparTests Where Not a.IsEnableStatusNull AndAlso _
                         a.EnableStatus = False Select a).ToList()

        Catch ex As Exception
            'Write error SYSTEM_ERROR in the Application Log
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " TestSampleWithDisabelStatus ", EventLogEntryType.Error, _
                                                                    GetApplicationInfoSession().ActivateSystemLog)
            'Show error message
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
        Return qTestList
    End Function

#End Region

#Region "OTHERS"
    ''' <summary>
    ''' When the screen is opened from the Calibration Tab in Tests Programming Screen, this method search the 
    ''' Test/SampleType in edition and select it in the grid of Calibrator by Test/SampleType
    ''' </summary>
    ''' <remarks>
    ''' Created by:  TR  
    ''' Modified by: SA 07/11/2012 - Inform the global variable used to format Theoretical Concentration Cells with the number
    '''                              of decimals allowed for the Test with the value received from Tests Programming Screen
    ''' </remarks>
    Private Sub SetReceivedTestSample()
        Try
            'Set the Calibrator by Test/SampleType as the active TAB
            CalibratorTabControl.SelectedTab = CalibratorTestTab

            'Verify if the Test/SampleType exists on local structures
            Dim qTestSampleList As List(Of TestsDS.tparTestsRow) = (From a As TestsDS.tparTestsRow In LocalTestDS.tparTests _
                                                                   Where a.TestID = ExternalTestIDAttribute _
                                                                 AndAlso a.SampleType = ExternalSampleTypeAttribute _
                                                                  Select a).ToList()
            If (qTestSampleList.Count = 0) Then
                'If the Test/SampleType is not found it means it is a new one; add it to LocalTestDS
                Dim myTestRow As TestsDS.tparTestsRow
                myTestRow = LocalTestDS.tparTests.NewtparTestsRow

                myTestRow.TestID = ExternalTestIDAttribute
                myTestRow.SampleType = ExternalSampleTypeAttribute
                myTestRow.TestName = ExternalTestNameAttribute
                myTestRow.DecimalsAllowed = ExternalDecimalsAllowAttribute
                myTestRow.TestVersionNumber = 0
                myTestRow.NewTest = True
                myTestRow.TestCalibratorID = 1000
                myTestRow.EnableStatus = False
                myTestRow.InUse = False

                If (UpdatedCalibratorsDS.tparCalibrators.Count > 0) AndAlso (Not UpdatedCalibratorsDS.tparCalibrators(0).IsCalibratorNameNull) Then
                    myTestRow.CalibratorName = UpdatedCalibratorsDS.tparCalibrators(0).CalibratorName
                End If

                'To set the calibrator id have to validate if there's and update test sample to asigned the calibrator id value
                myTestRow.CalibratorID = 0
                If (UpdatedTestCalibratorsDS.tparTestCalibrators.Count > 0) AndAlso (Not UpdatedTestCalibratorsDS.tparTestCalibrators(0).IsCalibratorIDNull) Then
                    myTestRow.CalibratorID = UpdatedTestCalibratorsDS.tparTestCalibrators(0).CalibratorID
                End If
                LocalTestDS.tparTests.AddtparTestsRow(myTestRow)

                LocalTestDS.tparTests.DefaultView.Sort = "EnableStatus, CalibratorID, TestName"
                CalibTestSampleListGrid.DataSource = LocalTestDS.tparTests.DefaultView
                SetTestSampleStatusIcon()
            End If

            'Search the Test/SampleType in the grid and mark it as the selected one 
            For Each myRow As DataGridViewRow In CalibTestSampleListGrid.Rows
                If (myRow.Cells("TestName").Value.ToString() = ExternalTestNameAttribute) AndAlso _
                   (myRow.Cells("SampleType").Value.ToString() = ExternalSampleTypeAttribute) Then
                    CalibTestSampleListGrid.FirstDisplayedScrollingRowIndex = myRow.Index
                    myRow.Selected = True
                    Exit For
                End If
            Next

            'SA 07/11/2012
            'Inform the global variable for the number of decimals allowed with the value received from Tests Programming Screen
            LocalDecimalAllow = ExternalDecimalsAllowAttribute

            'Set the Screen status to Edition mode
            If (CalibTestSampleListGrid.SelectedRows.Count > 0) Then
                BindCalibratorValues(CInt(CalibTestSampleListGrid.SelectedRows(0).Cells("TestCalibratorID").Value), _
                                     CInt(CalibTestSampleListGrid.SelectedRows(0).Cells("TestID").Value), _
                                     CalibTestSampleListGrid.SelectedRows(0).Cells("SampleType").Value.ToString(), _
                                     CInt(CalibTestSampleListGrid.SelectedRows(0).Cells("CalibratorID").Value))
                SetScreenStatusTABTestSampleCalib(SCREEN_STATUS_TYPE.EditMode)
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".SetRecivedTestSample ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".SetRecivedTestSample ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Validate if calibrator values is factory value.
    ''' </summary>
    ''' <param name="pTestID"></param>
    ''' <param name="pSampleType"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' Created by: TR 08/03/2011
    ''' </remarks>
    Private Function ValidateCalibratorFactoryValues(ByVal pTestID As Integer, ByVal pSampleType As String) As Boolean
        Dim isFactoryValue As Boolean = False
        Try
            Dim myGlobalDataTO As New GlobalDataTO
            Dim myTestSampleDelegate As New TestSamplesDelegate

            'TR 08/03/2011 -Validate if Factory Calibrator value is true.
            myGlobalDataTO = myTestSampleDelegate.ValidateFactoryCalibratorValue(Nothing, pTestID, pSampleType)
            If Not myGlobalDataTO.HasError Then
                isFactoryValue = DirectCast(myGlobalDataTO.SetDatos, Boolean)
            End If

        Catch ex As Exception
            'Write error SYSTEM_ERROR in the Application Log
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " ValidateCalibratorFactoryValues ", EventLogEntryType.Error, _
                                                            GetApplicationInfoSession().ActivateSystemLog)
            'Show error message
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
        Return isFactoryValue
    End Function


#End Region

#Region "Events"

    ''' <summary>
    ''' Handle the change event on the screen.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>
    ''' Created by: TR 14/02/2011.
    ''' </remarks>
    Private Sub ChangeMade(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles LotNumberTextBox.TextChanged, _
                                                         ExpDatePickUpCombo.ValueChanged, CalibratorNameTextBox.TextChanged, _
                                                         CalibNumberUpDown.ValueChanged, YAxisCombo.SelectedIndexChanged, XAxisCombo.SelectedIndexChanged, CurveTypeCombo.SelectedIndexChanged, CalibratorListCombo.SelectedIndexChanged, IncreasingRadioButton.CheckedChanged, DecreasingRadioButton.CheckedChanged
        Try
            'firt validate the screen status to set the local changes value.
            If LocalScreenStatus = SCREEN_STATUS_TYPE.CreateMode OrElse _
                                            LocalScreenStatus = SCREEN_STATUS_TYPE.EditMode Then
                LocalChange = True ' set the local change to true

                If sender.GetType().Name = "BSTextBox" Then
                    If DirectCast(sender, BSTextBox).Name = LotNumberTextBox.Name Then
                        LocalValidateDependencies = True
                        LocalCalibratorLotModified = True
                    End If

                ElseIf sender.GetType().Name = "BSComboBox" Then
                    If DirectCast(sender, BSComboBox).Name = CurveTypeCombo.Name Or String.Compare(DirectCast(sender, BSComboBox).Name, XAxisCombo.Name, False) = 0 _
                       Or DirectCast(sender, BSComboBox).Name = YAxisCombo.Name Or DirectCast(sender, BSComboBox).Name = CurveTypeCombo.Name Then
                        LocalValidateDependencies = True
                    End If
                ElseIf sender.GetType().Name = "BSRadioButton" Then
                    LocalValidateDependencies = True

                ElseIf sender.GetType().Name = "BSNumericUpDown" Then
                    LocalValidateDependencies = True
                    LocalCalibratorLotModified = True
                End If

            End If
        Catch ex As Exception
            'Write error SYSTEM_ERROR in the Application Log
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " ChangeMade ", EventLogEntryType.Error, _
                                                                GetApplicationInfoSession().ActivateSystemLog)
            'Show error message
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub ProgCalibrator_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Try
            'TR 23/04/2012 -Get the current level
            'Dim myGlobalbase As New GlobalBase
            CurrentUserLevel = GlobalBase.GetSessionInfo.UserLevel
            'TR 23/04/2012 -END

            ''DL 28/07/2011
            If IsTestParamWinAttribute Then
                Me.StartPosition = FormStartPosition.Manual

                Dim myLocation As Point = UiAx00MainMDI.Location
                Dim mySize As Size = UiAx00MainMDI.Size

                Me.Location = New Point(myLocation.X + CInt((mySize.Width - Me.Width) / 2), myLocation.Y + CInt((mySize.Height - Me.Height) / 2))
            End If
            'end DL 28/07/2011

            'Initialize all screen controls
            InitializeScreen()
            CalibratorListGrid.Focus()

            If (IsTestParamWinAttribute) Then
                SetReceivedTestSample()
                CalibTestSampleListGrid.Focus()
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".IProgCalibrator_Load ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".IProgCalibrator_Load ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub


    ''' <summary>
    ''' Not allow move form and mantain the center location in center parent
    ''' </summary>
    ''' <remarks>
    ''' Created by: DL 27/07/2011
    ''' </remarks>
    Protected Overrides Sub WndProc(ByRef m As Message)
        Try
            If m.Msg = WM_WINDOWPOSCHANGING Then
                Dim pos As WINDOWPOS = DirectCast(Runtime.InteropServices.Marshal.PtrToStructure(m.LParam, GetType(WINDOWPOS)), WINDOWPOS)
                If IsTestParamWinAttribute Then

                    Dim myLocation As Point = UiAx00MainMDI.Location
                    Dim mySize As Size = UiAx00MainMDI.Size

                    pos.x = myLocation.X + CInt((mySize.Width - Me.Width) / 2) 'Me.Left
                    pos.y = myLocation.Y + CInt((mySize.Height - Me.Height) / 2) ' Me.Top
                    Runtime.InteropServices.Marshal.StructureToPtr(pos, m.LParam, True)
                End If
            End If

            MyBase.WndProc(m)

        Catch ex As Exception
            'Write error SYSTEM_ERROR in the Application Log
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " WndProc ", EventLogEntryType.Error, _
                                                            GetApplicationInfoSession().ActivateSystemLog)
            'Show error message
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")

        End Try

    End Sub


    Private Sub ExitButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsAcceptButton.Click
        Try
            CloseForm()

            ''TR 28/07/2011 -Validate if local changes before closing.
            'If LocalChange Then
            '    If (ShowMessage(Name, GlobalEnumerates.Messages.DISCARD_PENDING_CHANGES.ToString) = Windows.Forms.DialogResult.Yes) Then
            '        'TR 28/07/2011 -Set value to false to avoid question of changes mades (BUG this message appears when screen is closed)
            '        LocalChange = False
            '        IAx00MainMDI.OpenMonitorForm(Me)
            '    End If
            'Else
            '    'RH 04/07/2011
            '    'If Not Tag Is Nothing Then
            '    '    'A PerformClick() method was executed
            '    Close()
            '    'Else
            '    'Normal button click
            '    'Open the WS Monitor form and close this one
            '    'IAx00MainMDI.OpenMonitorForm(Me)
            '    'End If
            'End If

        Catch ex As Exception
            'Write error SYSTEM_ERROR in the Application Log
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " ExitButton_Click", EventLogEntryType.Error, _
                                                            GetApplicationInfoSession().ActivateSystemLog)
            'Show error message
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    Private Sub CalibratorListGrid_RowEnter(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles CalibratorListGrid.RowEnter, CalibratorListGrid.CellClick

        Try
            If LocalScreenStatus <> SCREEN_STATUS_TYPE.EditMode AndAlso LocalScreenStatus <> SCREEN_STATUS_TYPE.CreateMode Then
                If CalibratorListGrid.SelectedRows.Count = 1 Then
                    BindCalibratorsControls(CInt(CalibratorListGrid.SelectedRows(0).Cells("CalibratorID").Value))
                    CalibratorListGrid.Rows(CalibratorListGrid.SelectedRows(0).Index).Selected = True 'TR 30/07/2013 -Select the corresponding row on gridview
                ElseIf CalibratorListGrid.SelectedRows.Count > 1 Then
                    BindCalibratorsControls(0)
                Else
                    BindCalibratorsControls(CInt(CalibratorListGrid.Rows(0).Cells("CalibratorID").Value))
                    CalibratorListGrid.Rows(0).Selected = True 'TR 30/07/2013 -Select the corresponding row on gridview
                    CalibratorListGrid.FirstDisplayedScrollingRowIndex = CalibratorListGrid.SelectedRows(0).Index
                End If

                'Depending if the calibrator is in use or special set the screen status.
                If CalibratorListGrid.SelectedRows.Count = 1 Then
                    If DirectCast(CalibratorListGrid.SelectedRows(0).Cells("InUse").Value, Boolean) Then
                        'Set screen status to Edit mode.
                        SetScreenStatusTABCalibrator(SCREEN_STATUS_TYPE.ElementInUse)
                    ElseIf DirectCast(CalibratorListGrid.SelectedRows(0).Cells("SpecialCalib").Value, Boolean) Then
                        'Set screen status to Edit mode.
                        SetScreenStatusTABCalibrator(SCREEN_STATUS_TYPE.SpecialCalibrator)
                    Else
                        SetScreenStatusTABCalibrator(SCREEN_STATUS_TYPE.ReadMode)
                    End If

                ElseIf CalibratorListGrid.SelectedRows.Count > 1 Then

                    For Each calRow As DataGridViewRow In CalibratorListGrid.SelectedRows
                        If DirectCast(calRow.Cells("InUse").Value, Boolean) OrElse DirectCast(calRow.Cells("SpecialCalib").Value, Boolean) Then
                            SetScreenStatusTABCalibrator(SCREEN_STATUS_TYPE.ElementInUse)
                            Exit Sub
                        End If
                    Next

                    'if multiselect then  set multiselect status
                    SetScreenStatusTABCalibrator(SCREEN_STATUS_TYPE.MultiSelection)
                Else
                    'Set screen status to Edit mode.
                    SetScreenStatusTABCalibrator(SCREEN_STATUS_TYPE.ReadMode)
                End If

            End If
        Catch ex As Exception
            'Write error SYSTEM_ERROR in the Application Log
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & "CalibratorListGrid_RowEnter ", EventLogEntryType.Error, _
                                                            GetApplicationInfoSession().ActivateSystemLog)
            'Show error message
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try

    End Sub

    Private Sub AddCalibButon_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AddCalibButon.Click

        Try
            'Unselect the row on the Calibrator list.
            If CalibratorListGrid.SelectedRows.Count > 0 Then
                CalibratorListGrid.ClearSelection()
            End If
            BindCalibratorsControls(0)

            'set initial values to controls
            ExpDatePickUpCombo.Value = Now.Date.AddMonths(3)

            'Set screen status to create mode.
            SetScreenStatusTABCalibrator(SCREEN_STATUS_TYPE.CreateMode)

            LotNumberTextBox.Focus() 'TR 30/07/2013 bug dont show the kaky color.
            'Set focus
            CalibratorNameTextBox.SelectAll()
            CalibratorNameTextBox.Focus()
        Catch ex As Exception
            'Write error SYSTEM_ERROR in the Application Log
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & "AddCalibButon_Click ", EventLogEntryType.Error, _
                                                            GetApplicationInfoSession().ActivateSystemLog)
            'Show error message
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try

    End Sub

    Private Sub CancelCalibButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CancelCalibButton.Click
        Try
            Dim myDialogResult As DialogResult = Windows.Forms.DialogResult.Yes
            If LocalChange Then
                'Show warning message indication there were changes made.
                myDialogResult = ShowMessage("Question", GlobalEnumerates.Messages.DISCARD_PENDING_CHANGES.ToString, , Me)

            End If

            If myDialogResult = Windows.Forms.DialogResult.Yes Then
                CalibratorErrorProv.Clear()
                Dim myScreenStatusTemp As New SCREEN_STATUS_TYPE
                'TR 18/04/2012 VALIDATE IF SPECIAL CALIBRATOR.
                'TR 29/05/2012 -Validate there is a calibrator selected. 
                If CalibratorListGrid.SelectedRows.Count = 1 AndAlso _
                   DirectCast(CalibratorListGrid.SelectedRows(0).Cells("SpecialCalib").Value, Boolean) Then
                    'Set screen status to Edit mode.
                    myScreenStatusTemp = SCREEN_STATUS_TYPE.SpecialCalibrator
                    'myScreenStatusTemp = 
                Else
                    myScreenStatusTemp = SCREEN_STATUS_TYPE.ReadMode
                End If
                'TR 18/04/2012
                'myScreenStatusTemp = SCREEN_STATUS_TYPE.ReadMode
                'TR 18/04/2012
                If CalibratorListGrid.Rows.Count > 0 Then
                    'If there's not selected row the select the first row on grid
                    If CalibratorListGrid.SelectedRows.Count = 0 Then
                        CalibratorListGrid.Rows(0).Selected = True
                        CalibratorListGrid.FirstDisplayedScrollingRowIndex = 0
                    End If

                    BindCalibratorsControls(CInt(CalibratorListGrid.SelectedRows(0).Cells("CalibratorID").Value))
                    If DirectCast(CalibratorListGrid.SelectedRows(0).Cells("InUSe").Value, Boolean) Then
                        myScreenStatusTemp = SCREEN_STATUS_TYPE.ElementInUse
                    End If
                End If

                'Set screen status to read mode.
                SetScreenStatusTABCalibrator(myScreenStatusTemp)
                LocalChange = False ' set the value to false on local change declaration.
                'TR 27/09/2012 -Add variable require to set values to false
                LocalValidateDependencies = False
                LocalCalibratorLotModified = False

                LocalConcPrevValue = -1
                LocalFirstPointValue = -1
            End If
        Catch ex As Exception
            'Write error SYSTEM_ERROR in the Application Log
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & "CancelCalibButton_Click ", EventLogEntryType.Error, _
                                                            GetApplicationInfoSession().ActivateSystemLog)
            'Show error message
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try


    End Sub

    Private Sub CalibratorListGrid_EnabledChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CalibratorListGrid.EnabledChanged
        Try
            'Dim BackColor As New Color
            'Dim LetColor As New Color

            'RH 11/05/2012 Remove New
            Dim BackColor As Color
            Dim LetColor As Color

            If Not CalibratorListGrid.Enabled Then
                BackColor = SystemColors.MenuBar 'Color.FloralWhite dl 19/07/2011
                LetColor = Color.DarkGray 'Color.LightGray dl 19/07/2011
            Else
                BackColor = Color.White
                LetColor = Color.Black
            End If

            For Each CalRow As DataGridViewRow In CalibratorListGrid.Rows
                CalRow.DefaultCellStyle.BackColor = BackColor
                CalRow.DefaultCellStyle.ForeColor = LetColor
            Next CalRow

        Catch ex As Exception
            'Write error SYSTEM_ERROR in the Application Log & show error message
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " CalibratorListGrid_EnabledChanged ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try

    End Sub

    Private Sub CalibratorNameTextBox_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles CalibratorNameTextBox.Validating
        Try
            If LocalScreenStatus = SCREEN_STATUS_TYPE.CreateMode Or LocalScreenStatus = SCREEN_STATUS_TYPE.EditMode Then
                If CalibratorExist(CalibratorNameTextBox.Text, CInt(CalibIDTextBox.Text)) Then
                    CalibratorErrorProv.SetError(CalibratorNameTextBox, GetMessageText(GlobalEnumerates.Messages.DUPLICATED_CALIB_NAME.ToString()))
                    CalibratorNameTextBox.Focus()
                Else
                    CalibratorErrorProv.Clear()
                End If
            End If
        Catch ex As Exception
            'Write error SYSTEM_ERROR in the Application Log & show error message
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " CalibratorNameTextBox_Validating ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub EditCalibButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _
                                                    EditCalibButton.Click, CalibratorListGrid.CellDoubleClick

        Try
            If EditCalibButton.Enabled Then
                'Validate the calibrator status, if it's in use
                If CalibratorListGrid.SelectedRows.Count = 1 AndAlso _
                    Not DirectCast(CalibratorListGrid.SelectedRows(0).Cells("InUse").Value, Boolean) Then
                    'Set screen status to Edit mode.
                    SetScreenStatusTABCalibrator(SCREEN_STATUS_TYPE.EditMode)
                    'If special calibrator disable calibratorname Text box.
                    If DirectCast(CalibratorListGrid.SelectedRows(0).Cells("SpecialCalib").Value, Boolean) Then
                        CalibratorNameTextBox.Enabled = False
                        CalibNumberUpDown.Enabled = False
                        LotNumberTextBox.Focus()
                    Else
                        CalibratorNameTextBox.Enabled = True
                        'Set focus
                        CalibratorNameTextBox.Focus()
                    End If
                End If
            End If

        Catch ex As Exception
            'Write error SYSTEM_ERROR in the Application Log & show error message
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " EditCalibButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try

    End Sub

    Private Sub CalibratorListGrid_CellPainting(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellPaintingEventArgs) Handles CalibratorListGrid.CellPainting
        Try
            If Not e Is Nothing AndAlso e.RowIndex >= 0 Then
                If DirectCast(CalibratorListGrid.Rows(e.RowIndex).Cells("InUse").Value, Boolean) Then
                    e.CellStyle.ForeColor = Color.DarkGray  'Color.Gray  DL 19/07/2011
                    e.CellStyle.SelectionBackColor = Color.LightGray
                    e.CellStyle.SelectionForeColor = Color.White
                End If
            End If

        Catch ex As Exception
            'Write error SYSTEM_ERROR in the Application Log & show error message
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " CalibratorListGrid_CellPainting ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub SaveCalibButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SaveCalibButton.Click
        Try
            'Validate if there was any changes.
            If LocalChange Then
                SaveCalibratorInfo()
            Else
                If (CalibratorListGrid.SelectedRows.Count > 0) Then
                    'TR 18/04/2012 VALIDATE IF SPECIAL CALIBRATOR. 
                    If DirectCast(CalibratorListGrid.SelectedRows(0).Cells("SpecialCalib").Value, Boolean) Then
                        'Set screen status to Edit mode.
                        SetScreenStatusTABCalibrator(SCREEN_STATUS_TYPE.SpecialCalibrator)
                    Else
                        SetScreenStatusTABCalibrator(SCREEN_STATUS_TYPE.ReadMode)
                    End If
                    'TR 18/04/2012
                    'SetScreenStatusTABCalibrator(SCREEN_STATUS_TYPE.ReadMode)'TR 18/04/2012 COMMENTED.
                Else
                    'SA 22/11/2012 - Added to avoid SystemError in ADD Mode when clicking in SAVE button with fields 
                    '                Name and LotNumber empty
                    ValidateCalibratorDataError()
                End If
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " SaveCalibButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub DeleteCalbButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DeleteCalbButton.Click
        DeleteCalibrator()
    End Sub

    Private Sub ProgCalibrator_Shown(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Shown
        Try
            'Depending if the calibrator is in use set the screen status.
            If CalibratorListGrid.SelectedRows.Count = 1 Then
                If Not DirectCast(CalibratorListGrid.SelectedRows(0).Cells("InUse").Value, Boolean) Then
                    'Set screen status to Edit mode.
                    SetScreenStatusTABCalibrator(SCREEN_STATUS_TYPE.ReadMode)
                Else
                    'Set screen status to Edit mode.
                    SetScreenStatusTABCalibrator(SCREEN_STATUS_TYPE.ElementInUse)
                End If
            ElseIf CalibratorListGrid.SelectedRows.Count > 1 Then
                'if multiselect then  set multiselect status
                SetScreenStatusTABCalibrator(SCREEN_STATUS_TYPE.MultiSelection)
            Else
                'Set screen status to Edit mode.
                SetScreenStatusTABCalibrator(SCREEN_STATUS_TYPE.ReadMode)
            End If

            'TAB CALIBRATOR BY TEST-SAMPLE.
            If (Not IsTestParamWinAttribute) Then
                If CalibTestSampleListGrid.Rows.Count > 0 Then
                    CalibTestSampleListGrid.Rows(0).Selected = True
                    CalibTestSampleListGrid.FirstDisplayedScrollingRowIndex = 0

                    'load the decimals allow
                    LocalDecimalAllow = CInt(CalibTestSampleListGrid.SelectedRows(0).Cells("DecimalsAllowed").Value)

                    If CBool(CalibTestSampleListGrid.SelectedRows(0).Cells("InUse").Value) Then
                        SetScreenStatusTABTestSampleCalib(SCREEN_STATUS_TYPE.ElementInUse)
                    Else
                        SetScreenStatusTABTestSampleCalib(SCREEN_STATUS_TYPE.ReadMode)
                    End If

                    BindCalibratorValues(CInt(CalibTestSampleListGrid.SelectedRows(0).Cells("TestCalibratorID").Value), _
                                         CInt(CalibTestSampleListGrid.SelectedRows(0).Cells("TestID").Value), _
                                         CalibTestSampleListGrid.SelectedRows(0).Cells("SampleType").Value.ToString(), _
                                         CInt(CalibTestSampleListGrid.SelectedRows(0).Cells("CalibratorID").Value))

                End If

            Else
                SetScreenStatusTABTestSampleCalib(SCREEN_STATUS_TYPE.EditMode)
                SpecialTestEditionScreenEnable()
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " ProgCalibrator_Shown ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try


    End Sub

    ''' <summary>
    ''' Fill the details area with data of the selected Test/Sample Type while moving up and down arrows on rows of grid
    ''' </summary>
    ''' <remarks>
    ''' Created by: SA 21/11/2012 - Added to replace functionality of code removed in RowEvent event 
    ''' </remarks>
    Private Sub CalibTestSampleListGrid_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles CalibTestSampleListGrid.KeyUp
        Try
            Dim rowIndex As Integer = -1
            If (e.KeyCode = Keys.Up OrElse e.KeyCode = Keys.Down) Then
                rowIndex = CalibTestSampleListGrid.SelectedRows(0).Index
                'CalibTestSampleListGrid.Rows(rowIndex).Selected = True

                LocalDecimalAllow = CInt(CalibTestSampleListGrid.Rows(rowIndex).Cells("DecimalsAllowed").Value)
                BindCalibratorValues(CInt(CalibTestSampleListGrid.Rows(rowIndex).Cells("TestCalibratorID").Value), _
                                     CInt(CalibTestSampleListGrid.Rows(rowIndex).Cells("TestID").Value), _
                                     CalibTestSampleListGrid.Rows(rowIndex).Cells("SampleType").Value.ToString(), _
                                     CInt(CalibTestSampleListGrid.Rows(rowIndex).Cells("CalibratorID").Value))

                If (CBool(CalibTestSampleListGrid.Rows(rowIndex).Cells("InUse").Value)) Then
                    SetScreenStatusTABTestSampleCalib(SCREEN_STATUS_TYPE.ElementInUse)
                Else
                    SetScreenStatusTABTestSampleCalib(SCREEN_STATUS_TYPE.ReadMode)
                End If
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".CalibTestSampleListGrid_KeyUp ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".CalibTestSampleListGrid_KeyUp ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub


    ''' <summary></summary>
    ''' <remarks>
    ''' Modified by: SA 08/11/2012 - Removed call to function BindCalibratorValues: this event is executed before Click event and the
    '''                              SelectedRow has not been still updated to the clicked one
    ''' </remarks>
    Private Sub CalibTestSampleList_RowEnter(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles CalibTestSampleListGrid.RowEnter
        Try
            If LocalScreenStatus <> SCREEN_STATUS_TYPE.EditMode AndAlso LocalScreenStatus <> SCREEN_STATUS_TYPE.CreateMode Then
                If CBool(CalibTestSampleListGrid.Rows(e.RowIndex).Cells("InUse").Value) Then
                    SetScreenStatusTABTestSampleCalib(SCREEN_STATUS_TYPE.ElementInUse)
                Else
                    SetScreenStatusTABTestSampleCalib(SCREEN_STATUS_TYPE.ReadMode)
                End If

                'SA 08/11/2012 - This code should not be here. RowEnter event is executed before Click event and the SelectedRow has not been
                '                still updated to the clicked one
                ''load the decimals allow
                'LocalDecimalAllow = CInt(CalibTestSampleListGrid.Rows(e.RowIndex).Cells("DecimalsAllowed").Value)

                'BindCalibratorValues(CInt(CalibTestSampleListGrid.Rows(e.RowIndex).Cells("TestCalibratorID").Value), _
                '                     CInt(CalibTestSampleListGrid.Rows(e.RowIndex).Cells("TestID").Value), _
                '                     CalibTestSampleListGrid.Rows(e.RowIndex).Cells("SampleType").Value.ToString(), _
                '                     CInt(CalibTestSampleListGrid.Rows(e.RowIndex).Cells("CalibratorID").Value))

                'CalibTestSampleListGrid.Refresh()
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".CalibTestSampleList_RowEnter ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".CalibTestSampleList_RowEnter ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub CalibTestSampleListGrid_CellPainting(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellPaintingEventArgs) Handles CalibTestSampleListGrid.CellPainting
        Try
            If (e.RowIndex >= 0) Then
                If (Not CalibTestSampleListGrid.Rows(e.RowIndex).Cells("InUse").Value Is DBNull.Value) AndAlso _
                   (DirectCast(CalibTestSampleListGrid.Rows(e.RowIndex).Cells("InUse").Value, Boolean)) Then
                    e.CellStyle.ForeColor = Color.DarkGray 'Color.Gray DL 19/07/2011
                    e.CellStyle.SelectionBackColor = Color.LightGray
                    e.CellStyle.SelectionForeColor = Color.White
                End If
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".CalibTestSampleListGrid_CellPainting ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".CalibTestSampleListGrid_CellPainting ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub EditCalTestSampleButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles EditCalTestSampleButton.Click, _
                                                                                                                  CalibTestSampleListGrid.CellDoubleClick
        Try
            'This functionality is not available for Users with OPERATOR Level
            If (Not CurrentUserLevel = "OPERATOR") Then
                If (CalibTestSampleListGrid.SelectedRows.Count = 1 AndAlso Not CBool(CalibTestSampleListGrid.SelectedRows(0).Cells("InUse").Value)) Then
                    SetScreenStatusTABTestSampleCalib(SCREEN_STATUS_TYPE.EditMode)
                    UnSelectConcentrationGridView()
                    CalibratorListCombo.Focus()
                    SpecialTestEditionScreenEnable()
                End If
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".EditCalTestSampleButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".EditCalTestSampleButton_Click ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub CalibTestSampleListGrid_EnabledChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CalibTestSampleListGrid.EnabledChanged
        Try
            Dim BackColor As New Color
            Dim LetColor As New Color

            If Not CalibTestSampleListGrid.Enabled Then
                BackColor = SystemColors.MenuBar 'Color.FloralWhite DL 19/07/2011
                LetColor = Color.DarkGray ' Color.LightGray DL 19/07/2011
            Else
                BackColor = Color.White
                LetColor = Color.Black
            End If

            For Each CalRow As DataGridViewRow In CalibTestSampleListGrid.Rows
                CalRow.DefaultCellStyle.BackColor = BackColor
                CalRow.DefaultCellStyle.ForeColor = LetColor
            Next CalRow

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".CalibTestSampleListGrid_EnabledChanged ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".CalibTestSampleListGrid_EnabledChanged ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub CalibratorListCombo_SelectionChangeCommitted(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CalibratorListCombo.SelectionChangeCommitted
        PrepareNewCalibratorValuesForTestSample()
    End Sub

    Private Sub ConcentrationGridView_CellPainting(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellPaintingEventArgs) Handles ConcentrationGridView.CellPainting
        Try
            If (CalibratorListCombo.Enabled) Then
                If (e.ColumnIndex = 0 AndAlso (e.RowIndex > -1 AndAlso e.RowIndex <= 10)) OrElse _
                   (e.ColumnIndex = 2 AndAlso (e.RowIndex > -1 AndAlso e.RowIndex <= 10)) Then
                    e.CellStyle.BackColor = SystemColors.MenuBar
                    e.CellStyle.ForeColor = Color.DarkGray

                ElseIf (e.ColumnIndex = 1 AndAlso (e.RowIndex > -1 AndAlso e.RowIndex <= 10)) Then
                    If (e.Value Is DBNull.Value OrElse e.Value Is Nothing OrElse e.Value.ToString = String.Empty) Then
                        e.CellStyle.BackColor = Color.Khaki
                    Else
                        e.CellStyle.BackColor = Color.White
                    End If
                End If
            Else
                'If the area is not in EDIT Mode, then all cells in the DataGridView as painting as disabled
                If (LocalScreenStatus <> SCREEN_STATUS_TYPE.EditMode) Then
                    If (e.RowIndex > -1) Then
                        e.CellStyle.BackColor = SystemColors.MenuBar
                        e.CellStyle.ForeColor = Color.DarkGray
                    End If
                End If
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ConcentrationGridView_CellPainting ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ConcentrationGridView_CellPainting ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Update the Style of all cells in the grid of Concentration
    ''' </summary>
    Private Sub UnSelectConcentrationGridView()
        Try
            If (ConcentrationGridView.Rows.Count > 0) Then
                For r As Integer = 0 To ConcentrationGridView.Rows.Count - 1 Step 1
                    ConcentrationGridView.Rows(r).Selected = False
                    If (CalibratorListCombo.Enabled) Then
                        'Change to White/Black the BackColor/ForeColor of the editable cells; the rest remain as MenuBar/DarkGray
                        ConcentrationGridView.Rows(r).Cells("TheoricalConcentration").Style.BackColor = Color.White
                        ConcentrationGridView.Rows(r).Cells("TheoricalConcentration").Style.ForeColor = Color.Black
                        ConcentrationGridView.Rows(r).Cells("TheoricalConcentration").ReadOnly = False
                    Else
                        'Change to MenuBar/DarkGray the BackColor/ForeColor of the editable cells
                        ConcentrationGridView.Rows(r).Cells("TheoricalConcentration").Style.BackColor = SystemColors.MenuBar
                        ConcentrationGridView.Rows(r).Cells("TheoricalConcentration").Style.ForeColor = Color.DarkGray
                        ConcentrationGridView.Rows(r).Cells("TheoricalConcentration").ReadOnly = True
                    End If
                Next r
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".UnselectTestsSampleTypes", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".UnselectTestsSampleTypes", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    Private Sub ConcentrationGridView_CellFormatting(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellFormattingEventArgs) Handles ConcentrationGridView.CellFormatting
        Try
            If (e.ColumnIndex = 1) Then
                If (Not ConcentrationGridView.Rows(e.RowIndex).Cells("Theoricalconcentration").Value Is DBNull.Value) Then
                    e.Value = CType(e.Value, Double).ToString("F" & LocalDecimalAllow.ToString)
                End If
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ConcentrationGridView_CellFormatting ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ConcentrationGridView_CellFormatting ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub ConcentrationGridView_CellLeave(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles ConcentrationGridView.CellEndEdit 'ConcentrationGridView.CellLeave, 
        Try
            If (e.RowIndex >= 0) Then
                'Variable used to determinate if Concentration of the first point has changed
                Dim changeFirstConc As Boolean = False

                If (LocalScreenStatus = SCREEN_STATUS_TYPE.EditMode) Then
                    If (ConcentrationGridView.Rows(e.RowIndex).Cells("TheoricalConcentration").FormattedValue Is DBNull.Value) Then
                        'If Concentration Value is Null, an error is shown in the cell
                        ConcentrationGridView.Rows(e.RowIndex).Cells("TheoricalConcentration").Style.Alignment = DataGridViewContentAlignment.MiddleCenter
                        ConcentrationGridView.Rows(e.RowIndex).Cells("TheoricalConcentration").ErrorText = GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString())
                        ConcentrationGridView.Rows(e.RowIndex).Cells("KitConcentrationRelation").Value = DBNull.Value
                        LocalChange = True
                        changeFirstConc = (e.RowIndex = 0)

                    ElseIf (ConcentrationGridView.Rows(e.RowIndex).Cells("TheoricalConcentration").FormattedValue.ToString = String.Empty) Then
                        'If Concentration Value is empty, an error is shown in the cell
                        ConcentrationGridView.Rows(e.RowIndex).Cells("TheoricalConcentration").Style.Alignment = DataGridViewContentAlignment.MiddleCenter
                        ConcentrationGridView.Rows(e.RowIndex).Cells("TheoricalConcentration").ErrorText = GetMessageText(GlobalEnumerates.Messages.ZERO_NOTALLOW.ToString(), currentLanguage)
                        ConcentrationGridView.Rows(e.RowIndex).Cells("KitConcentrationRelation").Value = DBNull.Value
                        LocalChange = True
                        changeFirstConc = (e.RowIndex = 0)

                    ElseIf (CDbl(ConcentrationGridView.Rows(e.RowIndex).Cells("TheoricalConcentration").FormattedValue) = 0) Then
                        'If Concentration Value is zero, an error is shown in the cell
                        ConcentrationGridView.Rows(e.RowIndex).Cells("TheoricalConcentration").Style.Alignment = DataGridViewContentAlignment.MiddleCenter
                        ConcentrationGridView.Rows(e.RowIndex).Cells("TheoricalConcentration").ErrorText = GetMessageText(GlobalEnumerates.Messages.ZERO_NOTALLOW.ToString(), currentLanguage)
                        ConcentrationGridView.Rows(e.RowIndex).Cells("KitConcentrationRelation").Value = DBNull.Value
                        LocalChange = True
                        changeFirstConc = (e.RowIndex = 0)

                    ElseIf (e.RowIndex = 0) Then
                        'First Concentration Value is always allowed if it is greater than zero
                        ConcentrationGridView.Rows(e.RowIndex).Cells("TheoricalConcentration").ErrorText = ""
                        ConcentrationGridView.Rows(e.RowIndex).Cells("TheoricalConcentration").Style.Alignment = DataGridViewContentAlignment.MiddleRight

                        'Set 1 as Factor for the last Calibrator point
                        ConcentrationGridView.Rows(e.RowIndex).Cells("KitConcentrationRelation").Value = 1

                        If (Not ConcentrationGridView.Rows(e.RowIndex).Cells("TheoricalConcentration").IsInEditMode AndAlso _
                            LocalConcPrevValue <> CDbl(ConcentrationGridView.Rows(e.RowIndex).Cells("TheoricalConcentration").FormattedValue)) Then
                            'Save the Concentration value of the last Calibrator point in a global variable
                            LocalConcPrevValue = CDbl(ConcentrationGridView.Rows(e.RowIndex).Cells("TheoricalConcentration").FormattedValue)

                            'changeFirstConc = True
                            changeFirstConc = CDbl(ConcentrationGridView.Rows(e.RowIndex).Cells("TheoricalConcentration").Value) <> LocalFirstPointValue
                            'TR 17/07/2013 -BUG #1229 Validate the dependencies 'cause concentration value has change. Uncomment the line under.
                            LocalValidateDependencies = True
                        End If

                    ElseIf (Not ConcentrationGridView.Rows(0).Cells("TheoricalConcentration").FormattedValue Is DBNull.Value) AndAlso _
                           (ConcentrationGridView.Rows(0).Cells("TheoricalConcentration").FormattedValue.ToString <> String.Empty) AndAlso _
                           (CDbl(ConcentrationGridView.Rows(0).Cells("TheoricalConcentration").FormattedValue) > 0) Then
                        ConcentrationGridView.Rows(e.RowIndex).Cells("TheoricalConcentration").ErrorText = ""
                        ConcentrationGridView.Rows(e.RowIndex).Cells("TheoricalConcentration").Style.Alignment = DataGridViewContentAlignment.MiddleRight

                        If (LocalConcPrevValue > 0 AndAlso Not ConcentrationGridView.Rows(e.RowIndex).Cells("TheoricalConcentration").IsInEditMode AndAlso _
                            LocalConcPrevValue <> CDbl(ConcentrationGridView.Rows(e.RowIndex).Cells("TheoricalConcentration").FormattedValue)) Then
                            'Calculate the KitConcentrationRelation value
                            ConcentrationGridView.Rows(e.RowIndex).Cells("KitConcentrationRelation").Value = _
                                    (CType(ConcentrationGridView.Rows(e.RowIndex).Cells("TheoricalConcentration").Value, Double) / _
                                     CType(ConcentrationGridView.Rows(0).Cells("TheoricalConcentration").Value, Double)).ToString("F3")

                            LocalChange = True
                            LocalValidateDependencies = True

                            LocalConcPrevValue = CDbl(ConcentrationGridView.Rows(e.RowIndex).Cells("TheoricalConcentration").FormattedValue)

                        ElseIf (LocalConcPrevValue < 0) Then
                            LocalConcPrevValue = CDbl(ConcentrationGridView.Rows(e.RowIndex).Cells("TheoricalConcentration").FormattedValue)
                        End If
                    End If

                    ''Validate if value is nothing to do the calculations
                    'If (Not ConcentrationGridView.Rows(e.RowIndex).Cells("TheoricalConcentration").FormattedValue Is DBNull.Value AndAlso _
                    '    ConcentrationGridView.Rows(e.RowIndex).Cells("TheoricalConcentration").FormattedValue.ToString <> String.Empty AndAlso _
                    '    Not ConcentrationGridView.Rows(0).Cells("TheoricalConcentration").FormattedValue Is DBNull.Value AndAlso _
                    '    CDbl(ConcentrationGridView.Rows(e.RowIndex).Cells("TheoricalConcentration").FormattedValue) <> 0) Then
                    '    'Clear cell errors 
                    '    ConcentrationGridView.Rows(e.RowIndex).Cells("TheoricalConcentration").ErrorText = ""
                    '    ConcentrationGridView.Rows(e.RowIndex).Cells("TheoricalConcentration").Style.Alignment = DataGridViewContentAlignment.MiddleRight

                    '    'Set the current value to a local variable to validate if there was any changes.
                    '    ConcentrationGridView.Rows(e.RowIndex).Cells("KitConcentrationRelation").Value = _
                    '        (CType(ConcentrationGridView.Rows(e.RowIndex).Cells("TheoricalConcentration").FormattedValue, Double) / _
                    '         CType(ConcentrationGridView.Rows(0).Cells("TheoricalConcentration").Value, Double)).ToString("F3")

                    '    'Validate if the value change to update the localChanges variable.
                    '    If (LocalConcPrevValue > 0 AndAlso Not ConcentrationGridView.Rows(e.RowIndex).Cells("TheoricalConcentration").IsInEditMode AndAlso _
                    '        LocalConcPrevValue <> CType(ConcentrationGridView.Rows(e.RowIndex).Cells("TheoricalConcentration").FormattedValue, Double)) Then

                    '        LocalChange = True
                    '        LocalValidateDependencies = True
                    '        LocalConcPrevValue = CType(ConcentrationGridView.Rows(e.RowIndex).Cells("TheoricalConcentration").Value, Double)

                    '        If (e.RowIndex = 0) Then
                    '            changeFirstConc = True
                    '        End If

                    '    ElseIf (LocalConcPrevValue < 0) Then
                    '        LocalConcPrevValue = CType(ConcentrationGridView.Rows(e.RowIndex).Cells("TheoricalConcentration").Value, Double)
                    '    End If

                    'ElseIf ConcentrationGridView.Rows(e.RowIndex).Cells("theoricalconcentration").Value Is DBNull.Value Then
                    '    'if concentration value is null show error on cell
                    '    ConcentrationGridView.Rows(e.RowIndex).Cells("theoricalconcentration").Style.Alignment = DataGridViewContentAlignment.MiddleCenter
                    '    ConcentrationGridView.Rows(e.RowIndex).Cells("theoricalconcentration").ErrorText = GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString())

                    'ElseIf CDbl(ConcentrationGridView.Rows(e.RowIndex).Cells("theoricalconcentration").Value) = 0 Then
                    '    ConcentrationGridView.Rows(e.RowIndex).Cells("theoricalconcentration").Style.Alignment = DataGridViewContentAlignment.MiddleCenter
                    '    ConcentrationGridView.Rows(e.RowIndex).Cells("theoricalconcentration").ErrorText = _
                    '                            GetMessageText(GlobalEnumerates.Messages.ZERO_NOTALLOW.ToString(), currentLanguage)
                    'End If

                    If (changeFirstConc) Then
                        'Validate if the first row has been changed to delete values in the rest of rows
                        'If (ConcentrationGridView.SelectedRows.Count > 0 AndAlso ConcentrationGridView.SelectedRows(0).Index = 0) Then
                        LocalChange = True
                        If (Not ConcentrationGridView.Rows(0).Cells("TheoricalConcentration").FormattedValue Is DBNull.Value) AndAlso _
                           (ConcentrationGridView.Rows(0).Cells("TheoricalConcentration").FormattedValue.ToString <> String.Empty) Then
                            LocalFirstPointValue = CDbl(ConcentrationGridView.Rows(0).Cells("TheoricalConcentration").Value)
                        End If

                        If (ConcentrationGridView.RowCount > 1) Then
                            For myRowIndex As Integer = 1 To ConcentrationGridView.RowCount - 1
                                ConcentrationGridView.Rows(myRowIndex).Cells("TheoricalConcentration").Value = DBNull.Value
                                ConcentrationGridView.Rows(myRowIndex).Cells("KitConcentrationRelation").Value = DBNull.Value

                                ConcentrationGridView.Rows(myRowIndex).Cells("TheoricalConcentration").ErrorText = String.Empty
                                ConcentrationGridView.Rows(myRowIndex).Cells("TheoricalConcentration").Style.Alignment = DataGridViewContentAlignment.MiddleRight
                            Next
                        End If
                        'End If
                    End If

                    'Validate all the informed Concentrations
                    ValidateErrorConcValuesDescOrderGrid()
                End If
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ConcentrationGridView_CellLeave ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ConcentrationGridView_CellLeave ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try

    End Sub

    Private Sub ConcentrationGridView_RowEnter(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles ConcentrationGridView.RowEnter
        If (ConcentrationGridView.SelectedRows.Count > 0) Then
            If (Not ConcentrationGridView.Rows(e.RowIndex).Cells(1).Value Is DBNull.Value) Then
                LocalConcPrevValue = CDbl(ConcentrationGridView.Rows(e.RowIndex).Cells(1).Value)
            Else
                LocalConcPrevValue = -1
            End If
        End If
    End Sub

    Private Sub ConcentrationGridView_EditingControlShowing(ByVal sender As Object, ByVal e As DataGridViewEditingControlShowingEventArgs) Handles ConcentrationGridView.EditingControlShowing
        Try
            If (Me.ConcentrationGridView.CurrentRow.Index >= 0 AndAlso Me.ConcentrationGridView.CurrentCell.ColumnIndex = 1) Then
                'TR 01/07/2011 - Do not allow to show Copy/Paste/Cut Menu on cell
                If (e.Control.GetType().Name = "DataGridViewTextBoxEditingControl") Then
                    DirectCast(e.Control, DataGridViewTextBoxEditingControl).ShortcutsEnabled = False
                End If

                RemoveHandler e.Control.KeyPress, AddressOf CheckNumericCell
                AddHandler e.Control.KeyPress, AddressOf CheckNumericCell
            Else
                RemoveHandler e.Control.KeyPress, AddressOf CheckNumericCell
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ConcentrationGridView_EditingControlShowing ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ConcentrationGridView_EditingControlShowing ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Validate if the value enter is numeric or one of the allowed decimals separators 
    ''' </summary>
    ''' <remarks>
    ''' Created by:  TR 21/06/2010
    ''' Modified by: SA 08/11/2012 - Implementation copied from the one in Controls Programmig Screen to:
    '''                              ** Avoid more than one decimal separator in a cell
    '''                              ** If the pressed key is BackSpace, allow it (e.Handle=false) and stop
    '''                              ** Name changed from CheckCell to CheckNumericCell
    ''' </remarks>
    Private Sub CheckNumericCell(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyPressEventArgs)
        Try
            If (e.KeyChar = CChar("") OrElse e.KeyChar = ChrW(Keys.Back)) Then
                e.Handled = False
            Else
                Dim myDecimalSeparator As String = SystemInfoManager.OSDecimalSeparator
                If (e.KeyChar = CChar(".") OrElse e.KeyChar = CChar(",")) Then
                    If (e.KeyChar = CChar(".") OrElse e.KeyChar = CChar(",")) Then
                        e.KeyChar = CChar(myDecimalSeparator)
                    End If

                    If (CType(sender, TextBox).Text.Contains(".") Or CType(sender, TextBox).Text.Contains(",")) Then
                        e.Handled = True
                    Else
                        e.Handled = False
                    End If
                Else
                    If (Not IsNumeric(e.KeyChar)) Then
                        e.Handled = True
                    End If
                End If
            End If

            'Dim myDecimalSeparator As String = SystemInfoManager.OSDecimalSeparator
            ''Add the Backspace key value (TR 23/12/2010 -Add the point)
            'If e.KeyChar = myDecimalSeparator OrElse String.Compare(e.KeyChar.ToString(), CChar("").ToString(), False) = 0 OrElse e.KeyChar = CChar(".") Then
            '    If e.KeyChar = CChar(".") Then
            '        e.KeyChar = CChar(myDecimalSeparator) 'TR 23/12/2010
            '    End If
            '    e.Handled = False
            'Else
            '    If Not IsNumeric(e.KeyChar) Then
            '        e.Handled = True
            '    End If
            'End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".CheckNumericCell ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".CheckNumericCell ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub CalibratorTabControl_Selecting(ByVal sender As System.Object, ByVal e As System.Windows.Forms.TabControlCancelEventArgs) Handles CalibratorTabControl.Selecting
        Try
            CalibratorErrorProv.Clear()

            'RH 11/05/2012 Get info from the selected calibrator, not the first one
            'Code optimization
            If (CalibratorListGrid.RowCount = 0) OrElse (CalibTestSampleListGrid.RowCount = 0) Then
                Return
            End If

            If (CalibratorListGrid.SelectedRows.Count = 0) Then CalibratorListGrid.Rows(0).Selected = True
            If (CalibTestSampleListGrid.SelectedRows.Count = 0) Then CalibTestSampleListGrid.Rows(0).Selected = True

            Dim tmpDataView As DataRowView = CType(CalibratorListGrid.SelectedRows(0).DataBoundItem, DataRowView)
            Dim CurrentRow As CalibratorsDS.tparCalibratorsRow = CType(tmpDataView.Row, CalibratorsDS.tparCalibratorsRow)
            tmpDataView = CType(CalibTestSampleListGrid.SelectedRows(0).DataBoundItem, DataRowView)
            Dim CurrentTestSampleRow As TestsDS.tparTestsRow = CType(tmpDataView.Row, TestsDS.tparTestsRow)

            If (LocalChange) Then
                If (ShowMessage(Name, GlobalEnumerates.Messages.DISCARD_PENDING_CHANGES.ToString) = DialogResult.No) Then
                    e.Cancel = True
                Else
                    BindCalibratorValues(CurrentTestSampleRow.TestCalibratorID, CurrentTestSampleRow.TestID, _
                                         CurrentTestSampleRow.SampleType, CurrentTestSampleRow.CalibratorID)

                    BindCalibratorsControls(CurrentRow.CalibratorID)
                    LocalChange = False
                End If
            Else
                BindCalibratorsControls(CurrentRow.CalibratorID)
                LocalChange = False
            End If

            If (Not LocalChange) Then
                'If CalibratorTabControl.SelectedTab.Name = "CalibratorInfoTab" Then
                'RH 11/05/2012 Do not rely on changeable text strings, but on compiler-validation-capable property values
                If (CalibratorTabControl.SelectedTab.Name = CalibratorInfoTab.Name) Then
                    If (CurrentRow.SpecialCalib) Then
                        'Set screen status to the mode for Special Calibrator
                        SetScreenStatusTABCalibrator(SCREEN_STATUS_TYPE.SpecialCalibrator)
                    Else
                        'SA 21/11/2012 - To avoid status of buttons EDIT and DELETE changes to ENABLED when the selected 
                        '                Calibrator is IN USE in the active WorkSession
                        If (CurrentRow.InUse) Then
                            SetScreenStatusTABCalibrator(SCREEN_STATUS_TYPE.ElementInUse)
                        Else
                            SetScreenStatusTABCalibrator(SCREEN_STATUS_TYPE.ReadMode)
                        End If
                        'SetScreenStatusTABCalibrator(SCREEN_STATUS_TYPE.ReadMode)
                    End If
                Else
                    'SA 21/11/2012 - To avoid status of button EDIT changes to ENABLED when the selected 
                    '                Test/Sample Type is IN USE in the active WorkSession
                    If (CurrentTestSampleRow.InUse) Then
                        SetScreenStatusTABTestSampleCalib(SCREEN_STATUS_TYPE.ElementInUse)
                    Else
                        SetScreenStatusTABTestSampleCalib(SCREEN_STATUS_TYPE.ReadMode)
                    End If
                    ''If (IsTestParamWinAttribute) Then
                    ''    SetScreenStatusTABTestSampleCalib(SCREEN_STATUS_TYPE.EditMode)
                    ''Else
                    'SetScreenStatusTABTestSampleCalib(SCREEN_STATUS_TYPE.ReadMode)
                    ''End If
                End If
            End If
            'RH 11/05/2012 END

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".CalibratorTabControl_Selecting ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".CalibratorTabControl_Selecting ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub ProgCalibrator_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles MyBase.KeyDown
        Try
            'Handle the escape key press depending the tab.
            If (e.KeyCode = Keys.Escape) Then
                If CalibratorTabControl.SelectedTab.Name = "CalibratorInfoTab" AndAlso CancelCalibButton.Enabled Then
                    CancelCalibButton.PerformClick()
                ElseIf String.Compare(CalibratorTabControl.SelectedTab.Name, "CalibratorTestTab", False) = 0 AndAlso CancelTestSampleCalValue.Enabled Then
                    CancelTestSampleCalValue.PerformClick()
                Else
                    If (IsTestParamWinAttribute) Then
                        Me.Close()
                    Else
                        CloseForm()
                    End If

                End If
                'Handle the enter key press to enable edition.
            ElseIf e.KeyCode = Keys.Enter Then
                If (Not IsTestParamWinAttribute) Then
                    If CalibratorTabControl.SelectedTab.Name = "CalibratorInfoTab" AndAlso EditCalibButton.Enabled Then
                        EditCalibButton.PerformClick()
                    ElseIf CalibratorTabControl.SelectedTab.Name = "CalibratorTestTab" AndAlso EditCalTestSampleButton.Enabled Then
                        EditCalTestSampleButton.PerformClick()
                    End If
                End If
                'TR 27/07/2011 -implemented to avoid the bug when pressing enter don't allow to jump to next row
                e.Handled = True
                'TR 27/07/2011 -END.

            ElseIf e.KeyCode = Keys.Delete Then
                If String.Compare(CalibratorTabControl.SelectedTab.Name, "CalibratorInfoTab", False) = 0 AndAlso DeleteCalbButton.Enabled Then
                    DeleteCalbButton.PerformClick()
                End If

            ElseIf e.KeyCode = Keys.F1 Then
                'TR 07/11/2011 -Search the Help File and  Chapter
                'Help.ShowHelp(Me, GetHelpFilePath(HELP_FILE_TYPE.MANUAL, currentLanguage), GetScreenChapter(Me.Name))
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " CancelButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub CalibTestSampleListGrid_CellClick(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles CalibTestSampleListGrid.CellClick
        Try
            If (e.RowIndex >= 0) Then
                'Save in a global variable the number of decimals allowed for the selected Test
                LocalDecimalAllow = CInt(CalibTestSampleListGrid.Rows(e.RowIndex).Cells("DecimalsAllowed").Value)

                'Load Calibrator values for the selected TestID/SampleType in the edition area
                BindCalibratorValues(CInt(CalibTestSampleListGrid.Rows(e.RowIndex).Cells("TestCalibratorID").Value), _
                                     CInt(CalibTestSampleListGrid.Rows(e.RowIndex).Cells("TestID").Value), _
                                     CalibTestSampleListGrid.Rows(e.RowIndex).Cells("SampleType").Value.ToString(), _
                                     CInt(CalibTestSampleListGrid.Rows(e.RowIndex).Cells("CalibratorID").Value))
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".CalibTestSampleListGrid_CellClick ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".CalibTestSampleListGrid_CellClick ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Prints the Calibrators Report
    ''' </summary>
    ''' <remarks>
    ''' Created by: RH 19/12/2011
    ''' </remarks>
    Private Sub PrintButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PrintButton.Click
        Try
            If (CalibratorListGrid.SelectedRows.Count > 0) Then
                Dim Calibrators As New List(Of Integer)

                For Each row As DataGridViewRow In CalibratorListGrid.SelectedRows
                    Calibrators.Add(CInt(row.Cells("CalibratorID").Value))
                Next

                XRManager.ShowCalibratorsReport(Calibrators)
            Else
                XRManager.ShowCalibratorsReport()
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".PrintButton_Click", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".PrintButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub CalibratorListCombo_Leave(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CalibratorListCombo.Leave
        'TR 16/01/2012 -Set the back color white if selection made
        If CalibratorListCombo.SelectedIndex >= 0 Then
            CalibratorListCombo.BackColor = Color.White
        Else
            CalibratorListCombo.BackColor = Color.Khaki
        End If
        'TR 16/01/2012 -END.
    End Sub

    Private Sub CurveTypeCombo_Leave(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CurveTypeCombo.Leave
        'TR 16/01/2012 -Set the back color white if selection made
        If CurveTypeCombo.SelectedIndex >= 0 Then
            CurveTypeCombo.BackColor = Color.White
        Else
            CurveTypeCombo.BackColor = Color.Khaki
        End If
        'TR 16/01/2012 -END.
    End Sub

    Private Sub XAxisCombo_Leave(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles XAxisCombo.Leave
        'TR 16/01/2012 -Set the back color white if selection made
        If XAxisCombo.SelectedIndex >= 0 Then
            XAxisCombo.BackColor = Color.White
        Else
            XAxisCombo.BackColor = Color.Khaki
        End If
        'TR 16/01/2012 -END.
    End Sub

    Private Sub ConcentrationGridView_Enter(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ConcentrationGridView.Enter
        If ConcentrationGridView.Rows.Count > 0 Then
            ConcentrationGridView.Rows(0).Cells(1).Selected = True
        End If

    End Sub

    Private Sub YAxisCombo_Leave(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles YAxisCombo.Leave
        'TR 16/01/2012 -Set the back color white if selection made
        If YAxisCombo.SelectedIndex >= 0 Then
            YAxisCombo.BackColor = Color.White
        Else
            YAxisCombo.BackColor = Color.Khaki
        End If
        'TR 16/01/2012 -END.
    End Sub

    Private Sub IncreasingRadioButton_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles IncreasingRadioButton.Validating, DecreasingRadioButton.Validating, _
                                                                                                                                          CurveTypeCombo.Validating, YAxisCombo.Validating, XAxisCombo.Validating
        'TR 08/03/2012 -Validate when user change control.
        'ValidaterCalibCurveAndConcValuesHasError()
        'TR 15/05/2012 -is not necesary to validate the concentration values 
        ValidateCalibratorCurveValuesHasError()
    End Sub

    Private Sub CancelTestSampleCalValue_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CancelTestSampleCalValue.Click
        Try
            Dim myDialogResult As DialogResult = Windows.Forms.DialogResult.Yes
            'Validate if there was any change.
            If LocalChange Then
                'Show warning message indication there were changes made.
                myDialogResult = ShowMessage("Question", GlobalEnumerates.Messages.DISCARD_PENDING_CHANGES.ToString, , Me)
            End If

            If myDialogResult = Windows.Forms.DialogResult.Yes Then
                'TR 08/03/2012- Clear all errors.
                CalibratorErrorProv.Clear()

                LocalConcentrationValuesDS.tparTestCalibratorValues.Clear()

                Dim myScreenStatusTemp As New SCREEN_STATUS_TYPE
                myScreenStatusTemp = SCREEN_STATUS_TYPE.ReadMode

                If Me.CalibTestSampleListGrid.Rows.Count > 0 Then
                    If Me.CalibTestSampleListGrid.SelectedRows.Count = 0 Then
                        Me.CalibTestSampleListGrid.Rows(0).Selected = True
                        Me.CalibTestSampleListGrid.FirstDisplayedScrollingRowIndex = 0
                    End If

                    'Set Read mode status.
                    myScreenStatusTemp = SCREEN_STATUS_TYPE.ReadMode
                End If


                SetScreenStatusTABTestSampleCalib(myScreenStatusTemp)

                BindCalibratorValues(CInt(CalibTestSampleListGrid.SelectedRows(0).Cells("TestCalibratorID").Value), _
                                         CInt(Me.CalibTestSampleListGrid.SelectedRows(0).Cells("TestID").Value), _
                                         Me.CalibTestSampleListGrid.SelectedRows(0).Cells("SampleType").Value.ToString(), _
                                         CInt(Me.CalibTestSampleListGrid.SelectedRows(0).Cells("CalibratorID").Value))

                LocalChange = False
                LocalValidateDependencies = False

                LocalConcPrevValue = -1
                LocalFirstPointValue = -1

                UnSelectConcentrationGridView() ' dl 19/07/2011
            End If
        Catch ex As Exception
            'Write error SYSTEM_ERROR in the Application Log & show error message
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " CancelTestSampleCalValue_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Save the Calibration data (Calibrator, Curve Values and Theoretical Concentration Values) for the Test/SampleType 
    ''' selected in the grid
    ''' </summary>
    ''' <remarks>
    ''' Created by:  TR
    ''' Modified by: TR 26/09/2012 - Validate if there was a localChange to continue saving process
    '''              SA 08/11/2012 - Changed the call to function SaveTestSampleCalibratorValues due to its required 
    '''                              parameters were changed
    '''              SA 21/11/2012 - Changed the way of set the SelectedRow. Set the Test/Sample Type saved also as CurrentRow
    '''                              (in the previous code, the row marked as selected was different from the CurrentRow and KeyUp event
    '''                              does not work fine
    ''' </remarks>
    Private Sub SaveTestSampleCalValue_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SaveTestSampleCalValue.Click
        Try
            If (LocalChange AndAlso CalibTestSampleListGrid.SelectedRows.Count = 1) Then
                If (Not ValidateCalibCurveAndConcValuesHasError()) Then
                    If (CalibratorListCombo.SelectedIndex >= 0) Then
                        Dim localDeleteCalibratorsResult As Boolean = False
                        Dim myDialogResult As DialogResult = Windows.Forms.DialogResult.None

                        'Validate the dependencies if required
                        If (LocalValidateDependencies AndAlso Not IsTestParamWinAttribute) Then
                            myDialogResult = ValidateDependecies(CInt(CalibTestSampleListGrid.SelectedRows(0).Cells("TestID").Value), _
                                                                 CalibTestSampleListGrid.SelectedRows(0).Cells("SampleType").Value.ToString, _
                                                                 CInt(CalibTestSampleListGrid.SelectedRows(0).Cells("TestVersionNumber").Value))
                            Select Case (myDialogResult)
                                Case Windows.Forms.DialogResult.OK
                                    localDeleteCalibratorsResult = True
                                    Exit Select

                                Case Windows.Forms.DialogResult.Cancel
                                    localDeleteCalibratorsResult = False
                                    Exit Try

                                Case Windows.Forms.DialogResult.None
                                    localDeleteCalibratorsResult = False
                                    Exit Select
                            End Select
                        End If

                        'Set the TestID, TestName and Sample Type to search on the GridView for selecction
                        Dim myRowIndex As Integer = CalibTestSampleListGrid.SelectedRows(0).Index
                        Dim myTestID As Integer = CInt(CalibTestSampleListGrid.SelectedRows(0).Cells("TestID").Value)
                        Dim myTestName As String = CalibTestSampleListGrid.SelectedRows(0).Cells("TestName").Value.ToString()
                        Dim mySampleType As String = CalibTestSampleListGrid.SelectedRows(0).Cells("SampleType").Value.ToString()

                        CalibTestSampleListGrid.Refresh()
                        If (SaveTestSampleCalibratorValues(CInt(CalibTestSampleListGrid.SelectedRows(0).Cells("TestCalibratorID").Value), _
                                                           myTestID, mySampleType)) Then
                            'If OK all then reload the TestSamples grid
                            FillCalibratorTestSampleList(False)

                            'Get the current Test/SampleType and change status to enabled
                            Dim qtestList As List(Of TestsDS.tparTestsRow) = (From a As TestsDS.tparTestsRow In LocalTestDS.tparTests _
                                                                             Where a.TestName = myTestName _
                                                                           AndAlso a.SampleType = mySampleType _
                                                                            Select a).ToList()
                            If (qtestList.Count > 0) Then qtestList.First().EnableStatus = True
                            SetTestSampleStatusIcon()

                            SetScreenStatusTABTestSampleCalib(SCREEN_STATUS_TYPE.ReadMode)
                            'CalibTestSampleListGrid.Refresh()

                            'SA 21/11/2012'
                            CalibTestSampleListGrid.CurrentCell = CalibTestSampleListGrid.Rows(myRowIndex).Cells(0)
                            CalibTestSampleListGrid.FirstDisplayedScrollingRowIndex = myRowIndex
                            CalibTestSampleListGrid.Rows(myRowIndex).Selected = True
                            CalibTestSampleListGrid.Refresh()

                            'CalibTestSampleListGrid.Focus()

                            ''Select in the grid the row for the Test/SampleType
                            'For Each myRow As DataGridViewRow In CalibTestSampleListGrid.Rows
                            '    If (myRow.Cells("TestName").Value.ToString() = myTestName) AndAlso _
                            '       (myRow.Cells("SampleType").Value.ToString() = mySampleType) Then
                            '        CalibTestSampleListGrid.FirstDisplayedScrollingRowIndex = myRow.Index
                            '        myRow.Selected = True
                            '        Exit For
                            '    End If
                            'Next

                            BindCalibratorValues(CInt(CalibTestSampleListGrid.SelectedRows(0).Cells("TestCalibratorID").Value), _
                                                 CInt(CalibTestSampleListGrid.SelectedRows(0).Cells("TestID").Value), _
                                                 CalibTestSampleListGrid.SelectedRows(0).Cells("SampleType").Value.ToString(), _
                                                 CInt(CalibTestSampleListGrid.SelectedRows(0).Cells("CalibratorID").Value))
                        End If
                    Else
                        CalibratorErrorProv.SetError(CalibratorListCombo, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString()))
                    End If
                End If
            Else
                'TR 26/09/2012 - If not changes made, then set SCREEN_STATUS_TYPE to Read Mode
                SetScreenStatusTABTestSampleCalib(SCREEN_STATUS_TYPE.ReadMode)
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".SaveTestSampleCalValue_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".SaveTestSampleCalValue_Click ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub


    ''' <summary>
    ''' To avoid entered following characters in the Numeric Up/Down allowing only integer numbers greater than zero:
    '''   ** Minus sign
    '''   ** Dot, Apostrophe or Comma as decimal point
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 09/11/2012
    ''' </remarks>
    Private Sub IntegerNumericUpDown_KeyPress(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles CalibNumberUpDown.KeyPress
        Try
            If (e.KeyChar = CChar("-") OrElse e.KeyChar = CChar(".") OrElse e.KeyChar = CChar(",") OrElse e.KeyChar = "'") Then e.Handled = True
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".IntegerNumericUpDown_KeyPress", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".IntegerNumericUpDown_KeyPress", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub
#End Region

    Private Sub CloseForm()
        Try

            'TR 08/03/2012 -Validate if there are Test without calibrators.
            If (Not IsTestParamWinAttribute AndAlso TestSampleWithDisabelStatus().Count > 0) Then
                CalibratorErrorProv.SetError(CalibratorTestSampleGB, GetMessageText(GlobalEnumerates.Messages.INCOMPLETE_TESTSAMPLE.ToString, LocalCurLanguage)) 'INCOMPLETE_TESTSAMPLE
                Return
            End If

            'RH 21/12/2011 New version, because all forms should be closed in the same way.
            'Move here the logic from ProgCalibrator_FormClosing() in order to unify the handle of closing
            'operations in only one place and so, simplify the closing algorithm.
            If LocalChange Then
                If (ShowMessage(Name, GlobalEnumerates.Messages.DISCARD_PENDING_CHANGES.ToString()) = DialogResult.No) Then
                    Return
                Else
                    'If TestSampleWithDisabelStatus().Count > 0 Then
                    '    CalibratorErrorProv.SetError(CalibratorTestSampleGB, GetMessageText(GlobalEnumerates.Messages.INCOMPLETE_TESTSAMPLE.ToString, LocalCurLanguage)) 'INCOMPLETE_TESTSAMPLE
                    '    Return
                    'End If

                    LocalChange = False
                End If
            End If

            'TR 11/04/2012 -Disable form on close to avoid any button press.
            Me.Enabled = False

            If (Not Me.Tag Is Nothing) Then
                'A PerformClick() method was executed
                Me.Close()
            Else
                'Normal button click - Open the WS Monitor form and close this one
                UiAx00MainMDI.OpenMonitorForm(Me)
            End If
            'RH 21/12/2011 END

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " CloseForm ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Verify if the selected TestID/SampleType is an Special Test and in this case, get the Special Test Settings
    ''' needed to configure the edition area properly
    ''' </summary>
    ''' <remarks>
    ''' Created by:  TR 19/04/2012
    ''' </remarks>
    Private Sub SpecialTestEditionScreenEnable()
        Try
            If (Not CalibTestSampleListGrid.SelectedRows(0).Cells("SpecialTest").Value Is DBNull.Value) AndAlso _
               (CBool(CalibTestSampleListGrid.SelectedRows(0).Cells("SpecialTest").Value)) Then
                CalibratorListCombo.Enabled = False
                ConcentrationGridView.Enabled = True
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".SpecialTestEditionScreenEnable ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".SpecialTestEditionScreenEnable ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Enable or disable functionallity by user level.
    ''' </summary>
    ''' <remarks>
    ''' Created by: TR 20/04/2012
    ''' Modified by XB 04/02/2013 - Upper conversions redundants because the value is already in UpperCase must delete to avoid Regional Settings problems (Bugs tracking #1112)
    ''' </remarks>
    Private Sub ScreenStatusByUserLevel()
        Try
            Select Case CurrentUserLevel    '.ToUpper()
                Case "SUPERVISOR"
                    Exit Select

                Case "OPERATOR"
                    'Calibrator Tab.
                    AddCalibButon.Enabled = False
                    EditCalibButton.Enabled = False
                    DeleteCalbButton.Enabled = False
                    PrintButton.Enabled = False
                    SaveCalibButton.Enabled = False
                    CancelCalibButton.Enabled = False

                    'Test/Calibrator Tab.
                    EditCalTestSampleButton.Enabled = False
                    SaveTestSampleCalValue.Enabled = False
                    CancelTestSampleCalValue.Enabled = False

                    Exit Select
            End Select

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & "ScreenStatusByUserLevel ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & "ScreenStatusByUserLevel ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Avoid to leave the Concentration cell when the cell content is only the decimal separator (is not a valid number)
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 21/11/2012 
    ''' </remarks>
    Private Sub ConcentrationGridView_CellValidating(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellValidatingEventArgs) Handles ConcentrationGridView.CellValidating
        Try
            If (ConcentrationGridView.Columns(e.ColumnIndex).Index = 1) Then
                If (Not e.FormattedValue Is DBNull.Value AndAlso Not e.FormattedValue Is Nothing AndAlso e.FormattedValue.ToString.Trim <> "" AndAlso _
                    Not IsNumeric(e.FormattedValue)) Then
                    e.Cancel = True
                End If
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ConcentrationGridView_CellValidating ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ConcentrationGridView_CellValidating", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub
End Class
