Option Explicit On
Option Strict On

'Imports System
'Imports System.Data
'Imports System.Windows.Forms
Imports DevExpress.XtraCharts
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Global.GlobalEnumerates
Imports Biosystems.Ax00.FwScriptsManagement
Imports Biosystems.Ax00.BL
Imports System.IO

Public Class UiPhotometryAdjustments
    Inherits PesentationLayer.BSAdjustmentBaseForm

#Region "Declarations"
    'Screen's business delegate
    Private WithEvents myScreenDelegate As PhotometryAdjustmentDelegate

    ' Language
    Private currentLanguage As String

    Private SelectedPage As PHOTOMETRY_PAGES
    'Private LimitMinLEDs As Long
    'Private LimitMaxLEDs As Long
    'Private LimitMinPhMain As Long
    'Private LimitMaxPhMain As Long
    'Private LimitMinPhRef As Long
    'Private LimitMaxPhRef As Long

    Private myClearImage As Bitmap = Nothing
    Private myWarningImage As Bitmap = Nothing

    ' XBC 16/05/2011
    Private SelectedAdjustmentsDS As New SRVAdjustmentsDS
    Private TemporalAdjustmentsDS As New SRVAdjustmentsDS
    Private TempToSendAdjustmentsDelegate As FwAdjustmentsDelegate
    ' Edited value
    Private EditedValues() As EditedValueStruct

    'Information
    Private SelectedInfoPanel As Panel
    Private SelectedAdjPanel As Panel

    ' XBC 21/02/2012
    Public Const StabilityInterval As Integer = 5  ' seconds

#End Region

#Region "Variables"
    Private ManageTabPages As Boolean
    Private UnableHandlers As Boolean
    Private RunningTest As Boolean
    Private FirstIteration As Boolean
    Private FlagExitingScreen As Boolean
    Private WarningLeds As String
#End Region

#Region "Attributes"
    'Private ChangedValueAttr As Boolean = False
    Private ActiveAnalyzerAttr As String
    Private ActiveAnalyzerModelAttr As String
#End Region

#Region "Properties"
    'Private Property ChangedValue() As Boolean
    '    Get
    '        Return Me.ChangedValueAttr
    '    End Get
    '    Set(ByVal value As Boolean)
    '        Me.BsSaveButton.Enabled = value
    '        Me.ChangedValueAttr = value
    '    End Set
    'End Property

    Public Property ActiveAnalyzer() As String
        Get
            Return Me.ActiveAnalyzerAttr
        End Get
        Set(ByVal value As String)
            Me.ActiveAnalyzerAttr = value
        End Set
    End Property

    Public Property ActiveAnalyzerModel() As String
        Get
            Return Me.ActiveAnalyzerModelAttr
        End Get
        Set(ByVal value As String)
            Me.ActiveAnalyzerModelAttr = value
        End Set
    End Property
#End Region

#Region "Enumerations"
    Private Enum PHOTOMETRY_PAGES
        STEP1
        STEP2
        STEP3
        'CHECKROTOR
    End Enum
#End Region

#Region "Structures"

    Private Structure EditedValueStruct
        ' Identifier
        Public WaveLength As String
        ' Led Position
        Public LedPos As String
        ' Current values
        Public CurrentValue As Single
        ' New values
        Public NewValue As Single
    End Structure

    Private Structure AdjustmentRowData

        Public AnalyzerID As String
        Public CodeFw As String
        Public Value As String
        Public CanSave As Boolean
        Public CanMove As Boolean
        Public AxisID As String
        Public GroupID As String
        Public InFile As Boolean


        Public Sub New(ByVal pCodeFw As String)
            CodeFw = pCodeFw
            AnalyzerID = ""
            Value = ""
            CanSave = False
            CanMove = False
            GroupID = ""
            AxisID = ""
            InFile = False
        End Sub

        Public Sub Clear()
            CodeFw = ""
            AnalyzerID = ""
            Value = ""
            CanSave = False
            CanMove = False
            GroupID = ""
            AxisID = ""
            InFile = False
        End Sub
    End Structure

#End Region

#Region "Constructor"
    Sub New()
        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        DefineScreenLayout(PHOTOMETRY_PAGES.STEP1)
    End Sub
#End Region

#Region "Event Handlers"
    ''' <summary>
    ''' Executes ManageReceptionEvent() method in the main thread
    ''' </summary>
    ''' <param name="pResponse"></param>
    ''' <param name="pData"></param>
    ''' <remarks>Created by XBC 15/09/2011</remarks>
    Public Sub ScreenReceptionLastFwScriptEvent(ByVal pResponse As RESPONSE_TYPES, ByVal pData As Object) Handles myScreenDelegate.ReceivedLastFwScriptEvent

        Me.UIThread(Function() ManageReceptionEvent(pResponse, pData))

    End Sub

    ''' <summary>
    ''' manages the response of the Analyzer after sending a FwScript List
    ''' The response can be OK, NG, Timeout or Exception
    ''' </summary>
    ''' <param name="pResponse">response type</param>
    ''' <param name="pData">data received</param>
    ''' <remarks>
    ''' Created by XBC 23/02/2011
    ''' </remarks>
    Private Function ManageReceptionEvent(ByVal pResponse As RESPONSE_TYPES, ByVal pData As Object) As Boolean
        Dim myGlobal As New GlobalDataTO
        Try
            'manage special operations according to the screen characteristics

            ' XBC 05/05/2011 - timeout limit repetitions
            If pResponse = RESPONSE_TYPES.TIMEOUT Then
                Select Case pData.ToString
                    Case AnalyzerManagerSwActionList.TRYING_CONNECTION.ToString
                        MyBase.DisplayMessage(Messages.TRY_CONNECTION.ToString)

                    Case AnalyzerManagerSwActionList.WAITING_TIME_EXPIRED.ToString
                        MyBase.CurrentMode = ADJUSTMENT_MODES.ERROR_MODE
                        MyBase.DisplayMessage(Messages.ERROR_COMM.ToString)
                        PrepareErrorMode()

                End Select
                Exit Function
            ElseIf pResponse = RESPONSE_TYPES.EXCEPTION Then
                PrepareErrorMode()
                Exit Function
            End If
            ' XBC 05/05/2011 - timeout limit repetitions

            'if needed manage the event in the Base Form
            MyBase.OnReceptionLastFwScriptEvent(pResponse, pData)

            Select Case MyBase.CurrentMode
                Case ADJUSTMENT_MODES.ADJUSTMENTS_READED
                    If pResponse = RESPONSE_TYPES.OK Then

                        MyBase.DisplayMessage(Messages.SRV_ADJUSTMENTS_READED.ToString)

                        If Not myGlobal.HasError Then
                            MyBase.myServiceMDI.AdjustmentsReaded = True
                            PrepareArea()
                        Else
                            PrepareErrorMode()
                            Exit Function
                        End If
                    End If

                Case ADJUSTMENT_MODES.LOADED
                    If pResponse = RESPONSE_TYPES.OK Then
                        PrepareArea()
                    Else
                        PrepareErrorMode()
                        Exit Function
                    End If

                Case ADJUSTMENT_MODES.TESTED
                    If pResponse = RESPONSE_TYPES.OK Or _
                       pResponse = RESPONSE_TYPES.START Then
                        PrepareArea()
                    Else
                        PrepareErrorMode()
                        Exit Function
                    End If

                Case ADJUSTMENT_MODES.TEST_EXITED
                    If pResponse = RESPONSE_TYPES.OK Then
                        PrepareArea()
                    Else
                        PrepareErrorMode()
                        Exit Function
                    End If

                Case ADJUSTMENT_MODES.SAVED
                    If pResponse = RESPONSE_TYPES.OK Then
                        MyBase.DisplayMessage(Messages.SRV_ADJUSTMENTS_SAVED.ToString)
                        PrepareArea()
                    Else
                        PrepareErrorMode()
                        Exit Function
                    End If

                    ' XBC 20/02/2012
                Case ADJUSTMENT_MODES.PARKED
                    If pResponse = RESPONSE_TYPES.OK Then
                        PrepareArea()
                    End If

                Case ADJUSTMENT_MODES.ERROR_MODE
                    MyBase.DisplayMessage(Messages.FWSCRIPT_DATA_ERROR.ToString)
                    PrepareErrorMode()
            End Select

        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".ScreenReceptionLastFwScriptEvent ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ScreenReceptionLastFwScriptEvent", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, myGlobal.ErrorMessage, Me)
        End Try

        Return True
    End Function



#End Region


#Region "Public Methods"
    ''' <summary>
    ''' Refresh this specified screen with the information received from the Instrument
    ''' </summary>
    ''' <param name="pRefreshEventType"></param>
    ''' <param name="pRefreshDS"></param>
    ''' <remarks>Created by XBC 20/02/2012</remarks>
    Public Overrides Sub RefreshScreen(ByVal pRefreshEventType As List(Of GlobalEnumerates.UI_RefreshEvents), ByVal pRefreshDS As Biosystems.Ax00.Types.UIRefreshDS)
        'Dim myGlobal As New GlobalDataTO
        Try
            myScreenDelegate.RefreshDelegate(pRefreshEventType, pRefreshDS)

            'if needed manage the event in the Base Form
            MyBase.OnReceptionLastFwScriptEvent(RESPONSE_TYPES.OK, Nothing)

            PrepareArea()

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".RefreshScreen ", EventLogEntryType.Error, _
                                                                    GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".RefreshScreen", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

#Region "Must Inherited"
    ''' <summary>
    ''' It manages the Stop of the operation(s) that are currently being performed. 
    ''' </summary>
    ''' <param name="pAlarmType"></param>
    ''' <remarks>Created by SGM 19/10/2012</remarks>
    Public Overrides Sub StopCurrentOperation(Optional ByVal pAlarmType As ManagementAlarmTypes = ManagementAlarmTypes.NONE)
        Try
            ' Stop FwScripts
            myFwScriptDelegate.StopFwScriptQueue()

            PrepareErrorMode()

            'when stop action is finished, perform final operations after alarm received
            MyBase.myServiceMDI.ManageAlarmStep2(pAlarmType)

        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".StopCurrentOperation ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".StopCurrentOperation ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

#End Region

#End Region


#Region "Private Methods"

    ''' <summary>
    ''' reset all homes
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>SGM 10/03/11</remarks>
    Private Function InitializeHomes() As GlobalDataTO
        Dim myGlobal As New GlobalDataTO
        Try
            myGlobal = myScreenDelegate.ResetAllPreliminaryHomes(MyBase.myServiceMDI.ActiveAnalyzer)

        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myGlobal.ErrorMessage = ex.Message
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".InitializeHomes ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".InitializeHomes", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
        Return myGlobal
    End Function

    Private Sub Initializations()
        Dim myGlobal As New GlobalDataTO
        Try
            Me.FirstIteration = True

            Me.BsStep2AbsMeanResult.Text = ""
            Me.BsStep2AbsSDResult.Text = ""
            Me.BsStep2AbsCVResult.Text = ""
            Me.BsStep2AbsMaxResult.Text = ""
            Me.BsStep2AbsMinResult.Text = ""
            Me.BsStep2AbsRangeResult.Text = ""
            Me.BsStep2phMainMeanResult.Text = ""
            Me.BsStep2phMainSDResult.Text = ""
            Me.BsStep2phMainCVResult.Text = ""
            Me.BsStep2phMainMaxResult.Text = ""
            Me.BsStep2phMainMinResult.Text = ""
            Me.BsStep2phMainRangeResult.Text = ""
            Me.BsStep2phRefMeanResult.Text = ""
            Me.BsStep2phRefSDResult.Text = ""
            Me.BsStep2phRefCVResult.Text = ""
            Me.BsStep2phRefMaxResult.Text = ""
            Me.BsStep2phRefMinResult.Text = ""
            Me.BsStep2phRefRangeResult.Text = ""
            Me.BsStep3AbsMeanResult.Text = ""
            Me.BsStep3AbsSDResult.Text = ""
            Me.BsStep3AbsCVResult.Text = ""
            Me.BsStep3AbsMaxResult.Text = ""
            Me.BsStep3AbsMinResult.Text = ""
            Me.BsStep3AbsRangeResult.Text = ""

            myGlobal = myScreenDelegate.Initialize()
            If myGlobal.HasError Then
                PrepareErrorMode()
                CreateLogActivity(myGlobal.ErrorCode, Me.Name & ".Initializations ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
                ShowMessage(Me.Name & ".Initializations ", myGlobal.ErrorCode, myGlobal.ErrorMessage, Me)
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".Initializations ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".Initializations ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Generic function to send FwScripts to the Instrument through own delegates
    ''' </summary>
    ''' <param name="pMode"></param>
    ''' <param name="pAdjustmentGroup"></param>
    ''' <remarks>Created by: XBC 23/02/2011</remarks>
    Private Sub SendFwScript(ByVal pMode As ADJUSTMENT_MODES, _
                             Optional ByVal pAdjustmentGroup As ADJUSTMENT_GROUPS = Nothing)
        Dim myGlobal As New GlobalDataTO
        Try
            If Not myGlobal.HasError AndAlso Ax00ServiceMainMDI.MDIAnalyzerManager.Connected Then
                myScreenDelegate.NoneInstructionToSend = True
                myGlobal = myScreenDelegate.SendFwScriptsQueueList(pMode, pAdjustmentGroup)
                If Not myGlobal.HasError Then

                    If myScreenDelegate.NoneInstructionToSend Then
                        ' Send FwScripts
                        myGlobal = myFwScriptDelegate.StartFwScriptQueue
                    End If

                End If
            Else
                myGlobal.HasError = True
            End If

            If myGlobal.HasError Then
                PrepareErrorMode()
                If myFwScriptDelegate.CurrentFwScriptsQueue IsNot Nothing Then
                    myFwScriptDelegate.CurrentFwScriptsQueue.Clear()
                End If
                CreateLogActivity(myGlobal.ErrorCode, Me.Name & ".SendFwScript ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
                ShowMessage(Me.Name & ".SendFwScript ", myGlobal.ErrorCode, myGlobal.ErrorMessage, Me)
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".SendFwScript ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".SendFwScript ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Get texts in the current application language for all screen controls
    ''' </summary>
    ''' <remarks>
    ''' Created by: XBC 23/02/2011
    ''' </remarks>
    Private Sub GetScreenLabels()
        Try
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            'For Labels, CheckBox, RadioButtons.....
            BsTabPagesControl.TabPages(0).Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_BL_DC", currentLanguage)
            BsTabPagesControl.TabPages(1).Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_REP_STA_BS", currentLanguage)
            'BsTabPagesControl.TabPages(2).Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_CheckRotor", currentLanguage)
            ' TAB 1
            BsBaselineInfoTitle.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_INFO_TITLE", currentLanguage)
            BsStep1Label.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_BL_DC", currentLanguage)
            BsStep1ConfigurationGroupBox.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_TestConfiguration", currentLanguage)
            bsStep1DACSIntensityLabel.Text = "uA" ' myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_DACS", currentLanguage) PDT ???
            bsStep1RefDACSIntensityLabel.Text = "uA Ref" ' myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_DACS", currentLanguage) PDT ???
            BsStep1LEDsGroupBox1.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_DACS", currentLanguage)
            BsStep1BaselineLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_Baseline", currentLanguage)
            BsStep1DCLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_DarknessCounts", currentLanguage)
            BsStep1ManualFillRadioButton.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_MANUALLY_FILL", currentLanguage)
            BsStep1AutoFillRadioButton.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_AUTOMATIC_FILL", currentLanguage)
            BsStep1WellLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_CurveReplicate_Well", currentLanguage)
            BsStep1BLphMainGroupBox.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_Photodiode1", currentLanguage)
            BsStep1BLphRefGroupBox.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_Photodiode2", currentLanguage)
            Me.BSStep1BLChart.Series(0).LegendText = BsStep1BLphMainGroupBox.Text
            Me.BSStep1BLChart.Series(1).LegendText = BsStep1BLphRefGroupBox.Text
            CType(BSStep1BLChart.Diagram, XYDiagram).AxisX.Title.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_Wavelength", currentLanguage)
            CType(BSStep1BLChart.Diagram, XYDiagram).AxisY.Title.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_NumberCounts", currentLanguage)
            BsStep1WLsGroupBox.Text = "λ"
            BsStep1DCphMainGroupBox.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_Photodiode1", currentLanguage)
            BsStep1DCphRefGroupBox.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_Photodiode2", currentLanguage)
            BsStep1ResultsLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "TITLE_SRV_Results", currentLanguage)
            bsSelectAllITsCheckbox.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_SELECT_ALL", currentLanguage)
            ' TAB 2
            BsStep2InfoTitle.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_INFO_TITLE", currentLanguage)
            BsStep2Label.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_REP_STA_BS", currentLanguage)
            BsStep2ConfigurationGroupBox.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_TestConfiguration", currentLanguage)
            BsStep2WavelengthLabel.Text = "λ"
            BsStep2Well2Label.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_CurveReplicate_Well", currentLanguage)
            BsStep2ResultsLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "TITLE_SRV_Results", currentLanguage)
            BsStep2phMainGroupBox.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_Photodiode1", currentLanguage)
            BsStep2phRefGroupBox.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_Photodiode2", currentLanguage)
            Me.Step2Chart.Series(0).LegendText = BsStep2phMainGroupBox.Text
            Me.Step2Chart.Series(1).LegendText = BsStep2phRefGroupBox.Text
            BsStep2AbsorbanceGroupBox.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_Results", currentLanguage)
            BsStep2MeanLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Mean", currentLanguage) 'JB 01/10/2012 - Resource String unification
            BsStep2SDevLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_SDevResult", currentLanguage)
            BsStep2CVevLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_CDevResult", currentLanguage)
            BsStep2MaxLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_MaxValue", currentLanguage)
            BsStep2MinLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_MinValue", currentLanguage)
            BsStep2RangeLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_Range", currentLanguage)
            BsStep2RepeatabilityRadioButton.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_REP_TEST", currentLanguage)
            BsStep2StabilityRadioButton.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_STA_TEST", currentLanguage)
            BsStep2AbsorbanceRadioButton.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Absorbance_Full", currentLanguage) 'JB 01/10/2012 - Resource String unification
            BsStep2ManualFillRadioButton.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_MANUALLY_FILL", currentLanguage)
            BsStep2AutoFillRadioButton.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_AUTOMATIC_FILL", currentLanguage)
            BsStep2LightRadioButton.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_LIGHT", currentLanguage)
            BsStep2DarkRadioButton.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_DARK", currentLanguage)
            '' TAB 3
            'BsStep3Label.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_CheckRotor", currentLanguage)
            'BsFillModeLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_FillMode", currentLanguage) & ":"
            'BsWavelengthLabel2.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_Wavelength", currentLanguage) & ":"
            'BsResultsLabel2.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "TITLE_Results", currentLanguage)
            'BsStep3AbsorbanceGroupBox.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Absorbance_Full", currentLanguage)

            ' Tooltips
            GetScreenTooltip()

        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".GetScreenLabels", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".GetScreenLabels", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Get texts in the current application language for all screen controls
    ''' </summary>
    ''' <remarks>
    ''' Created by: XBC 26/07/11
    ''' </remarks>
    Private Sub GetScreenTooltip()
        Try
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            ' For Tooltips...
            MyBase.bsScreenToolTipsControl.SetToolTip(BsStep1DacsReferenceButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Edit", currentLanguage))
            MyBase.bsScreenToolTipsControl.SetToolTip(BsStep1ITSaveButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Save", currentLanguage))
            MyBase.bsScreenToolTipsControl.SetToolTip(BsStep1ITExitButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Cancel", currentLanguage))

            MyBase.bsScreenToolTipsControl.SetToolTip(BsTestButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "SRV_BTN_Test", currentLanguage))
            MyBase.bsScreenToolTipsControl.SetToolTip(BsExitButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_CloseScreen", currentLanguage))

            WarningLeds = myMultiLangResourcesDelegate.GetResourceText(Nothing, "SRV_LEDS_WARNINGS", currentLanguage)

        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".GetScreenTooltip ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".GetScreenTooltip ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Loads the icons and tooltips used for painting the buttons
    ''' </summary>
    ''' <remarks>Created by: XBC 23/02/2011</remarks>
    Private Sub PrepareButtons()
        Dim auxIconName As String = ""
        Dim iconPath As String = MyBase.IconsPath
        'Dim Utilities As New Utilities

        Try

            ' Hidden buttons for these tests
            Me.BsAdjustButton.Visible = False
            Me.BsSaveButton.Visible = False
            ' Hide Verification Rotor TAB because will not use for the moment
            Me.BsTabPagesControl.TabPages.Remove(TabVerificationRotor)

            'MyBase.SetButtonImage(BsExitButton, "CANCEL")
            'MyBase.SetButtonImage(BsTestButton, "ADJUSTMENT")
            'MyBase.SetButtonImage(BsStep1ITSaveButton, "SAVE")
            'MyBase.SetButtonImage(BsStep1ITExitButton, "UNDO")
            'MyBase.SetButtonImage(BsStep1UpDownWSButton, "UPDOWN", 20, 20)
            'MyBase.SetButtonImage(BsStep2UpDownWSButton, "UPDOWN", 20, 20)

            'DL 20/04/2012. Substitute icons
            auxIconName = GetIconName("EDIT")
            If (auxIconName <> "") Then
                BsStep1DacsReferenceButton.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
            End If

            auxIconName = GetIconName("CANCEL")
            If (auxIconName <> "") Then
                BsExitButton.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
            End If

            auxIconName = GetIconName("ADJUSTMENT")
            If (auxIconName <> "") Then
                BsTestButton.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
            End If

            auxIconName = GetIconName("SAVE")
            If (auxIconName <> "") Then
                BsStep1ITSaveButton.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
            End If

            auxIconName = GetIconName("UNDO")
            If (auxIconName <> "") Then
                BsStep1ITExitButton.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
            End If

            auxIconName = GetIconName("UPDOWN")
            If (auxIconName <> "") Then
                BsStep1UpDownWSButton.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
                BsStep2UpDownWSButton.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
            End If
            'DL 20/04/2012

            '
            ' STEP 1
            ' 
            ''ADJUST Button
            'auxIconName = GetIconName("VOLUME")
            'If System.IO.File.Exists(iconPath & auxIconName) Then
            '    BsAdjustButton.BackgroundImage = ImageUtilities.ImageFromFile(iconPath & auxIconName)
            '    BsAdjustButton.BackgroundImageLayout = ImageLayout.Center
            'End If

            ''SAVE Button
            'auxIconName = GetIconName("SAVE")
            'If System.IO.File.Exists(iconPath & auxIconName) Then
            '    BsSaveButton.BackgroundImage = ImageUtilities.ImageFromFile(iconPath & auxIconName)
            '    BsSaveButton.BackgroundImageLayout = ImageLayout.Center
            'End If

            ''CANCEL Button
            'auxIconName = GetIconName("UNDO") 'CANCEL
            'If System.IO.File.Exists(iconPath & auxIconName) Then
            '    BsCancelButton.BackgroundImage = ImageUtilities.ImageFromFile(iconPath & auxIconName)
            '    BsCancelButton.BackgroundImageLayout = ImageLayout.Center
            'End If

            ''EXIT Button
            'auxIconName = GetIconName("CANCEL")
            'If System.IO.File.Exists(iconPath & auxIconName) Then
            '    Dim myImage As Image = Image.FromFile(iconPath & auxIconName)
            '    myImage = CType(Utilities.ResizeImage(myImage, New Size(28, 28)).SetDatos, Image)
            '    BsExitButton.Image = myImage
            '    BsExitButton.ImageAlign = ContentAlignment.MiddleCenter
            'End If

            ''BASELINE TEST Button
            'auxIconName = GetIconName("ADJUSTMENT")
            'If System.IO.File.Exists(iconPath & auxIconName) Then
            '    Dim myImage As Image = Image.FromFile(iconPath & auxIconName)
            '    myImage = CType(Utilities.ResizeImage(myImage, New Size(28, 28)).SetDatos, Image)
            '    BsTestButton.Image = myImage
            '    BsTestButton.ImageAlign = ContentAlignment.MiddleCenter
            'End If

            ''CHANGE IT REFERENCES Button
            'auxIconName = GetIconName("EDIT")
            'If System.IO.File.Exists(iconPath & auxIconName) Then
            '    Dim myImage As Image = Image.FromFile(iconPath & auxIconName)
            '    myImage = CType(Utilities.ResizeImage(myImage, New Size(28, 28)).SetDatos, Image)
            '    BsStep1DacsReferenceButton.Image = myImage
            '    BsStep1DacsReferenceButton.ImageAlign = ContentAlignment.MiddleCenter
            'End If

            ''IT REFERENCES SAVE Button
            'auxIconName = GetIconName("SAVE")
            'If System.IO.File.Exists(iconPath & auxIconName) Then
            '    Dim myImage As Image = Image.FromFile(iconPath & auxIconName)
            '    myImage = CType(Utilities.ResizeImage(myImage, New Size(28, 28)).SetDatos, Image)
            '    BsStep1ITSaveButton.Image = myImage
            '    BsStep1ITSaveButton.ImageAlign = ContentAlignment.MiddleCenter
            'End If

            ''IT REFERENCES CANCEL Button
            'auxIconName = GetIconName("UNDO")
            'If System.IO.File.Exists(iconPath & auxIconName) Then
            '    Dim myImage As Image = Image.FromFile(iconPath & auxIconName)
            '    myImage = CType(Utilities.ResizeImage(myImage, New Size(28, 28)).SetDatos, Image)
            '    BsStep1ITExitButton.Image = myImage
            '    BsStep1ITExitButton.ImageAlign = ContentAlignment.MiddleCenter
            '    'BsStep1ITExitButton.BackgroundImageLayout = ImageLayout.Stretch
            'End If

            '
            ' STEP 2
            '

            ' XBC 27-04-2011 - Place test buttons outside testing area into buttons below area
            'STEP2 TEST Button
            'auxIconName = GetIconName("VOLUME")
            'If System.IO.File.Exists(iconPath & auxIconName) Then
            '    BsStep2TestButtonTODELETE.BackgroundImage = ImageUtilities.ImageFromFile(iconPath & auxIconName)
            '    BsStep2TestButtonTODELETE.BackgroundImageLayout = ImageLayout.Center
            'End If

            ''STEP3 TEST Button
            'auxIconName = GetIconName("VOLUME")
            'If System.IO.File.Exists(iconPath & auxIconName) Then
            '    BsStep3TestButton.BackgroundImage = ImageUtilities.ImageFromFile(iconPath & auxIconName)
            '    BsStep3TestButton.BackgroundImageLayout = ImageLayout.Center
            'End If

            Dim myNewNGImage As Bitmap = Nothing
            Dim myNGImage As Image = Nothing
            ''Dim myUtil As New Utilities.
            Dim myGlobal As New GlobalDataTO

            ' Icons used to informate about warning Limits into ListViews PhMain & PhRef and also Intesities of the LEDs
            Me.BsphMainWarningsListView.StateImageList = Me.ImageList1
            Me.BsphRefWarningsListView.StateImageList = Me.ImageList1

            auxIconName = GetIconName("FREECELL")
            If System.IO.File.Exists(iconPath & auxIconName) Then
                myNGImage = ImageUtilities.ImageFromFile(iconPath & auxIconName)

                myGlobal = Utilities.ResizeImage(myNGImage, New Size(20, 20))
                If Not myGlobal.HasError And myGlobal.SetDatos IsNot Nothing Then
                    myClearImage = CType(myGlobal.SetDatos, Bitmap)
                Else
                    myClearImage = CType(myNGImage, Bitmap)
                End If
            Else
                myClearImage = Nothing
            End If

            auxIconName = GetIconName("STUS_WITHERRS") 'WARNING") dl 23/03/2012
            If System.IO.File.Exists(iconPath & auxIconName) Then
                myNGImage = ImageUtilities.ImageFromFile(iconPath & auxIconName)

                myGlobal = Utilities.ResizeImage(myNGImage, New Size(20, 20))
                If Not myGlobal.HasError And myGlobal.SetDatos IsNot Nothing Then
                    myWarningImage = CType(myGlobal.SetDatos, Bitmap)
                Else
                    myWarningImage = CType(myNGImage, Bitmap)
                End If
            Else
                myWarningImage = Nothing
            End If


            '' XBC 20/02/2012
            'auxIconName = GetIconName("UPDOWN") ' UPDOWNROW
            'If System.IO.File.Exists(iconPath & auxIconName) Then
            '    Dim myImage As Image = Image.FromFile(iconPath & auxIconName)
            '    myGlobal = Utilities.ResizeImage(myImage, New Size(20, 20))
            '    If Not myGlobal.HasError And myGlobal.SetDatos IsNot Nothing Then
            '        myImage = CType(myGlobal.SetDatos, Image)
            '    End If
            '    'Me.BsStep1UpDownWSButton.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
            '    'Me.BsStep2UpDownWSButton.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
            '    Me.BsStep1UpDownWSButton.Image = myImage
            '    Me.BsStep1UpDownWSButton.ImageAlign = ContentAlignment.MiddleCenter
            '    Me.BsStep2UpDownWSButton.Image = myImage
            '    Me.BsStep2UpDownWSButton.ImageAlign = ContentAlignment.MiddleCenter
            'End If

        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".PrepareButtons", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PrepareButtons", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Set all the screen elements to disabled
    ''' </summary>
    ''' <remarks>Created by: XBC 23/02/2011</remarks>
    Private Sub DisableAll()
        Try
            Select Case SelectedPage
                Case PHOTOMETRY_PAGES.STEP1
                    Me.BsStep1ManualFillRadioButton.Enabled = False
                    Me.BsStep1AutoFillRadioButton.Enabled = False
                    Me.BsStep1WellUpDown.Enabled = False
                    Me.BsTestButton.Enabled = False
                    Me.BsStep1DacsReferenceButton.Enabled = False
                    Me.bsSelectAllITsCheckbox.Enabled = False
                    Me.BsStep1ITSaveButton.Enabled = False
                    Me.BsStep1ITExitButton.Enabled = False
                Case PHOTOMETRY_PAGES.STEP2
                    Me.BsStep2RepeatabilityRadioButton.Enabled = False
                    Me.BsStep2StabilityRadioButton.Enabled = False
                    Me.BsStep2AbsorbanceRadioButton.Enabled = False
                    Me.BsStep2WLCombo.Enabled = False
                    Me.BsStep2WellUpDown.Enabled = False
                    Me.BsTestButton.Enabled = False
                    Me.BsStep2AutoFillRadioButton.Enabled = False
                    Me.BsStep2ManualFillRadioButton.Enabled = False
                    Me.BsStep2DarkRadioButton.Enabled = False
                    Me.BsStep2LightRadioButton.Enabled = False

                    'Case PHOTOMETRY_PAGES.CHECKROTOR
                    '    Me.BsFillModeCombo.Enabled = False
                    '    Me.BsStep3WLCombo.Enabled = False
                    '    Me.BsStep3TestButton.Enabled = False

            End Select

            ' XBC 20/02/2012
            Me.BsStep1UpDownWSButton.Enabled = False
            Me.BsStep2UpDownWSButton.Enabled = False

            ' Disable Area Buttons
            'Me.BsAdjustButton.Enabled = False
            'Me.BsSaveButton.Enabled = False
            'Me.BsCancelButton.Enabled = False
            Me.BsExitButton.Enabled = False

            'disable MDI Menus and Buttons
            MyBase.ActivateMDIMenusButtons(False)

            ' Disable Tabs 
            Me.ManageTabPages = False

        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".DisableAll ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".DisableAll ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Prepare Tab Area according with current operations
    ''' </summary>
    ''' <remarks>Created by: XBC 23/02/2011</remarks>
    Private Sub PrepareArea()
        Try
            Application.DoEvents()

            ' Enabling/desabling form components to this child screen
            Select Case MyBase.CurrentMode

                Case ADJUSTMENT_MODES.ADJUSTMENTS_READING
                    MyBase.DisplayMessage(Messages.SRV_READ_ADJUSTMENTS.ToString)

                Case ADJUSTMENT_MODES.ADJUSTMENTS_READED
                    MyBase.DisplayMessage(Messages.SRV_ADJUSTMENTS_READED.ToString)
                    MyClass.PrepareLoadingMode()
                    MyClass.LoadAdjustmentGroupData()
                    'Me.ManageTabPages = True
                    MyBase.ActivateMDIMenusButtons(True)

                Case ADJUSTMENT_MODES.LOADED
                    MyClass.PrepareLoadedMode()

                Case ADJUSTMENT_MODES.TESTING
                    MyClass.PrepareTestingMode()

                Case ADJUSTMENT_MODES.TESTED
                    MyClass.PrepareTestedMode()

                Case ADJUSTMENT_MODES.TEST_EXITING
                    MyClass.PrepareTestExitingMode()

                Case ADJUSTMENT_MODES.TEST_EXITED
                    MyClass.PrepareTestExitedMode()

                Case ADJUSTMENT_MODES.SAVING
                    MyClass.PrepareSavingMode()

                Case ADJUSTMENT_MODES.SAVED
                    MyClass.PrepareSavedMode()

                    ' XBC 20/02/2012
                Case ADJUSTMENT_MODES.PARKING
                    Me.PrepareParkingMode()

                    ' XBC 20/02/2012
                Case ADJUSTMENT_MODES.PARKED
                    Me.PrepareParkedMode()

                Case ADJUSTMENT_MODES.ERROR_MODE
                    MyClass.PrepareErrorMode()
            End Select

            If Not MyBase.SimulationMode And Ax00ServiceMainMDI.MDIAnalyzerManager.AnalyzerStatus = AnalyzerManagerStatus.SLEEPING Then
                MyClass.PrepareErrorMode()
                MyBase.DisplayMessage("")
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".PrepareArea ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PrepareArea ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Prepare GUI for Loading Mode
    ''' </summary>
    ''' <remarks>Created by XBC 23/02/2011</remarks>
    Private Sub PrepareLoadingMode()
        Dim myResultData As New GlobalDataTO
        'Dim myGlobalbase As New GlobalBase
        Dim myPhotometryDataTO As PhotometryDataTO
        Try
            DisableAll()

            ' Get limits values
            myResultData = GetLimitValues()

            ' Get common Parameters
            If Not myResultData.HasError Then
                Me.ActiveAnalyzerModel = Ax00ServiceMainMDI.ActiveAnalyzerModel
                Me.ActiveAnalyzer = Ax00ServiceMainMDI.ActiveAnalyzer
                myResultData = GetParameters(Me.ActiveAnalyzerModel)
            End If

            ' Prepare main TAB - Baseline & Darnkness Counts tests
            If Not myResultData.HasError Then
                myResultData = PrepareScreen()

                ' Check Previous completed BLDC tests
                Dim myPathBLFile As String = Application.StartupPath & GlobalBase.PhotometryTestsFile
                If IO.File.Exists(myPathBLFile) Then
                    ' Recover BLDC test previously completed
                    myResultData = myScreenDelegate.GetLastBLDCCompletedTest(myPathBLFile)
                    If (Not myResultData.HasError And Not myResultData.SetDatos Is Nothing) Then
                        myPhotometryDataTO = DirectCast(myResultData.SetDatos, PhotometryDataTO)

                        If Me.ActiveAnalyzer = myPhotometryDataTO.AnalyzerID Then
                            ' same Intrument

                            ' XBC 02/09/2011
                            If myPhotometryDataTO.PositionLed.Count = Me.BsDataGridViewLeds.Columns.Count Then
                                ' same number of active Leds

                                ' Populate results of the readed WaveLengths to screen
                                PopulatephMainBL(myPhotometryDataTO.CountsMainBaseline)
                                PopulatephRefBL(myPhotometryDataTO.CountsRefBaseline)

                                ' XBC 24/11/2011
                                'BsphMainDCLabel.Text = myPhotometryDataTO.CountsMainDarkness(0).ToString("#,##0")
                                'BsphRefDCLabel.Text = myPhotometryDataTO.CountsRefDarkness(0).ToString("#,##0")
                                BsphMainDCLabel.Text = myPhotometryDataTO.CountsMainDarkness.Average.ToString("#,##0")
                                BsphRefDCLabel.Text = myPhotometryDataTO.CountsRefDarkness.Average.ToString("#,##0")
                                ' XBC 24/11/2011

                                PopulateBLChart(myPhotometryDataTO.CountsMainBaseline, myPhotometryDataTO.CountsRefBaseline)

                                myResultData = myScreenDelegate.AcceptBLResults()
                                If Not myResultData.HasError Then
                                    myScreenDelegate.TestBaseLineDone = True
                                End If

                            End If

                        End If

                    End If

                End If

            End If

            If Not myResultData.HasError Then
                Initializations()
                MyBase.CurrentMode = ADJUSTMENT_MODES.LOADED
                PrepareArea()
            Else
                PrepareErrorMode()
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".PrepareLoadingMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PrepareLoadingMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Prepare GUI for Loaded Mode
    ''' </summary>
    ''' <remarks>Created by XBC 23/02/2011</remarks>
    Private Sub PrepareLoadedMode()
        Dim myResultData As New GlobalDataTO
        Dim myPhotometryDataTO As PhotometryDataTO
        Try
            ' Step 1
            Me.BsStep1ManualFillRadioButton.Enabled = True
            Me.BsStep1AutoFillRadioButton.Enabled = True
            Me.BsStep1WellUpDown.Enabled = True
            Me.BsTestButton.Enabled = True
            ' Step 2
            Me.BsStep2RepeatabilityRadioButton.Enabled = True
            Me.BsStep2StabilityRadioButton.Enabled = True
            Me.BsStep2AbsorbanceRadioButton.Enabled = True
            Me.BsStep2WLCombo.Enabled = True
            Me.BsStep2phMainGroupBox.Visible = False
            Me.BsStep2phRefGroupBox.Visible = False
            Me.BsStep2AbsorbanceGroupBox.Visible = True
            Me.BsStep2AbsorbanceGroupBox.Size = New Size(100, 241)
            Me.BsTestButton.Enabled = True
            Me.BsStep2AutoFillRadioButton.Enabled = True
            Me.BsStep2ManualFillRadioButton.Enabled = True
            'If Me.BsStep2AutoFillRadioButton.Checked Then
            '    Me.BsStep2WellUpDown.Value = 1
            '    Me.BsStep2WellUpDown.Enabled = False
            'Else
            Me.BsStep2WellUpDown.Enabled = True
            'End If
            Me.BsStep2DarkRadioButton.Enabled = True
            Me.BsStep2LightRadioButton.Enabled = True

            '' Step 3
            'Me.BsFillModeCombo.Enabled = True
            'Me.BsStep3WLCombo.Enabled = True
            'Me.BsStep3TestButton.Enabled = True

            Select Case Me.SelectedPage

                Case PHOTOMETRY_PAGES.STEP1
                    'If Me.BsStep1AutoFillRadioButton.Checked Then
                    '    Me.BsStep1WellUpDown.Value = 1
                    '    Me.BsStep1WellUpDown.Enabled = False
                    'Else
                    '    Me.BsStep1WellUpDown.Enabled = True
                    'End If

                    ' Manage results of the readed WaveLengths from the Instrument
                    If myScreenDelegate.TestBaseLineDone Then
                        ' Instrument returns leds currents in conjunction with BL & DC readings
                        myResultData = myAnalyzerManager.ReadPhotometryData()
                        If (Not myResultData.HasError And Not myResultData.SetDatos Is Nothing) Then
                            myPhotometryDataTO = DirectCast(myResultData.SetDatos, PhotometryDataTO)

                            PopulateLEDSIntensity(myPhotometryDataTO.LEDsIntensities)
                        End If
                    End If

                Case PHOTOMETRY_PAGES.STEP2

                    RefreshResultLists(Me.BsStep2WLCombo.SelectedIndex)

                    'Case PHOTOMETRY_PAGES.CHECKROTOR

                    '    RefreshResultLists()

            End Select

            ' XBC 20/02/2012
            Me.BsStep1UpDownWSButton.Enabled = True
            Me.BsStep2UpDownWSButton.Enabled = True

            'Me.BsSaveButton.Enabled = False
            'Me.BsCancelButton.Enabled = False
            Me.BsExitButton.Enabled = True

            If myScreenDelegate.TestBaseLineDone And Not Me.RunningTest Then
                Me.ManageTabPages = True
                MyBase.ActivateMDIMenusButtons(True) 'SGM 27/09/2011
            Else
                Me.ManageTabPages = False
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".PrepareLoadedMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PrepareLoadedMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Prepare GUI for Testing Mode
    ''' </summary>
    ''' <remarks>Created by XBC 28/02/2011</remarks>
    Private Sub PrepareTestingMode()
        Try
            DisableAll()
            ManageTabPages = False

        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".PrepareTestingMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PrepareTestingMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Prepare GUI for Tested Mode
    ''' </summary>
    ''' <remarks>Created by XBC 23/02/2011</remarks>
    Private Sub PrepareTestedMode()
        Dim myResultData As New GlobalDataTO
        'Dim myPhotometryDataTO As PhotometryDataTO
        'Dim myGlobalbase As New GlobalBase
        Dim myPath As String
        Try
            Select Case Me.SelectedPage

                Case PHOTOMETRY_PAGES.STEP1

                    Select Case myScreenDelegate.CurrentTest
                        Case ADJUSTMENT_GROUPS.PHOTOMETRY
                            '
                            ' BASELINE & DARKNESS COUNTS
                            '
                            If myScreenDelegate.TestBaseLineDone Then
                                Me.ProgressBar1.Value = 0
                                Me.ProgressBar1.Visible = False
                                Me.ProgressBar1.Refresh()

                                Me.Cursor = Cursors.Default
                                Me.RunningTest = False
                                Me.FirstIteration = False
                                Me.BsStep1ManualFillRadioButton.Enabled = True
                                Me.BsStep1AutoFillRadioButton.Enabled = True
                                Me.BsStep1WellUpDown.Enabled = True
                                Me.BsTestButton.Enabled = True
                                ' XBC 20/02/2012
                                Me.BsStep1UpDownWSButton.Enabled = True
                                Me.BsStep2UpDownWSButton.Enabled = True


                                If MyBase.CurrentUserNumericalLevel = USER_LEVEL.lOPERATOR Then
                                    Me.BsStep1DacsReferenceButton.Enabled = False
                                Else
                                    Me.BsStep1DacsReferenceButton.Enabled = True
                                End If

                                'If Me.BsStep1AutoFillRadioButton.Checked Then
                                '    Me.BsStep1WellUpDown.Value = 1
                                '    Me.BsStep1WellUpDown.Enabled = False
                                'Else
                                '    Me.BsStep1WellUpDown.Enabled = True
                                'End If
                                ' Buttons Area
                                'Me.BsSaveButton.Enabled = True
                                'Me.BsCancelButton.Enabled = True
                                Me.BsExitButton.Enabled = True

                                ' Save BLDC Test
                                myPath = Application.StartupPath & GlobalBase.PhotometryTestsFile
                                myResultData = myScreenDelegate.SaveBLDCFile(myPath, Me.ActiveAnalyzer)

                                MyBase.DisplayMessage(Messages.SRV_COMPLETED.ToString)

                                ' Populate results of the readed WaveLengths to screen
                                PopulateLEDSIntensity(myScreenDelegate.CurrentLEDsIntensities)
                                PopulatephMainBL(myScreenDelegate.BaseLineMainCounts)
                                PopulatephRefBL(myScreenDelegate.BaseLineRefCounts)
                                BsphMainDCLabel.Text = myScreenDelegate.DarkphMainCount.ToString("#,##0")
                                BsphRefDCLabel.Text = myScreenDelegate.DarkphRefCount.ToString("#,##0")
                                PopulateBLChart(myScreenDelegate.BaseLineMainCounts, myScreenDelegate.BaseLineRefCounts)

                                ' Write Results into output file
                                myPath = Application.StartupPath & GlobalBase.PhotometryBLFileOut
                                CreateBLResultsFileOutput(myPath)
                            Else
                                If myScreenDelegate.HomesDone Then
                                    ' homes are done for current adjust
                                    myScreenDelegate.HomesDone = False

                                    myResultData = myScreenDelegate.SetPreliminaryHomesAsDone(ADJUSTMENT_GROUPS.PHOTOMETRY)
                                    If myResultData.HasError Then
                                        PrepareErrorMode()
                                        Exit Sub
                                    End If

                                    Me.CurrentOperationTimer.Enabled = False
                                    Me.ProgressBar1.Value = 0
                                    Me.ProgressBar1.Visible = False
                                    Me.ProgressBar1.Refresh()

                                    ' Continue with the Test until finish it
                                    myResultData = MyBase.Test
                                    Me.RunningTest = True
                                    If myResultData.HasError Then
                                        PrepareErrorMode()
                                        Exit Sub
                                    Else
                                        If MyBase.SimulationMode Then
                                            ' simulating...
                                            'MyBase.DisplaySimulationMessage("Baseline Testing...")
                                            Me.Cursor = Cursors.WaitCursor
                                            System.Threading.Thread.Sleep(SimulationProcessTime)
                                            MyBase.myServiceMDI.Focus()
                                            Me.Cursor = Cursors.Default
                                            MyClass.CurrentMode = ADJUSTMENT_MODES.TESTED
                                            PrepareArea()
                                        Else
                                            ' Fill Rotor is already filled by user and then proceed to send Read Photometric Counts to Instrument
                                            If Not myResultData.HasError AndAlso Ax00ServiceMainMDI.MDIAnalyzerManager.Connected Then
                                                Me.Cursor = Cursors.WaitCursor
                                                myResultData = myScreenDelegate.SendREAD_COUNTS(ADJUSTMENT_GROUPS.PHOTOMETRY)
                                            End If
                                        End If
                                    End If
                                ElseIf myScreenDelegate.HomesDoing Then
                                    ' Continue with the Test until finish it
                                    'Me.ProgressBar1.Maximum = myScreenDelegate.CurrentTimeOperation
                                    'Me.ProgressBar1.Value = 0
                                    'Me.ProgressBar1.Visible = True
                                    'Me.CurrentOperationTimer.Interval = 1000 ' 1 second
                                    'Me.CurrentOperationTimer.Enabled = True
                                End If
                            End If

                    End Select


                Case PHOTOMETRY_PAGES.STEP2

                    Select Case myScreenDelegate.CurrentTest
                        Case ADJUSTMENT_GROUPS.REPEATABILITY
                            '
                            ' REPEATABILITY
                            '
                            If myScreenDelegate.TestRepeatabilityDone Then
                                Me.ProgressBar1.Value = 0
                                Me.ProgressBar1.Visible = False
                                Me.ProgressBar1.Refresh()

                                Me.Cursor = Cursors.Default
                                Me.RunningTest = False
                                Me.FirstIteration = False
                                Me.BsStep2RepeatabilityRadioButton.Enabled = True
                                Me.BsStep2StabilityRadioButton.Enabled = True
                                Me.BsStep2AbsorbanceRadioButton.Enabled = True
                                Me.BsStep2WLCombo.Enabled = True
                                'If Me.BsStep2ManualFillRadioButton.Checked Then
                                Me.BsStep2WellUpDown.Enabled = True
                                'End If
                                Me.BsTestButton.Enabled = True
                                Me.BsExitButton.Enabled = True
                                Me.BsStep2AutoFillRadioButton.Enabled = True
                                Me.BsStep2ManualFillRadioButton.Enabled = True
                                Me.BsStep2DarkRadioButton.Enabled = True
                                Me.BsStep2LightRadioButton.Enabled = True
                                ' XBC 20/02/2012
                                Me.BsStep1UpDownWSButton.Enabled = True
                                Me.BsStep2UpDownWSButton.Enabled = True

                                ' Populate results of the readed WaveLengths to screen
                                MyBase.DisplayMessage(Messages.SRV_COMPLETED.ToString)
                                If Me.BsStep2DarkRadioButton.Checked Then
                                    ' DARK
                                    Me.BsStep2AbsCVResult.Visible = False
                                    Me.BsStep2CVevLabel.Visible = False
                                    PopulatephMainStep2(Me.BsStep2WLCombo.SelectedIndex)
                                    PopulatephRefStep2(Me.BsStep2WLCombo.SelectedIndex)
                                    PopulateStep2Chart(myScreenDelegate.MeasuresRepeatabilityphMainCountsDKByLed(Me.BsStep2WLCombo.SelectedIndex), _
                                                       myScreenDelegate.MeasuresRepeatabilityphRefCountsDKByLed(Me.BsStep2WLCombo.SelectedIndex))
                                    'myScreenDelegate.GetMaxValueRepeatabilityResult(0), _
                                    'myScreenDelegate.GetMinValueRepeatabilityResult(0))
                                    Step2Chart.Legend.Visibility = DevExpress.Utils.DefaultBoolean.True
                                ElseIf Me.BsStep2LightRadioButton.Checked Then
                                    ' LIGHT
                                    Me.BsStep2AbsCVResult.Visible = True
                                    Me.BsStep2CVevLabel.Visible = True
                                    PopulateABSStep2(Me.BsStep2WLCombo.SelectedIndex)
                                    PopulateStep2Chart(myScreenDelegate.MeasuresRepeatabilityAbsorbances(Me.BsStep2WLCombo.SelectedIndex))
                                    Step2Chart.Legend.Visibility = DevExpress.Utils.DefaultBoolean.False
                                End If

                                ' Write Results into output file
                                myPath = Application.StartupPath & GlobalBase.PhotometryRepeatabilityFileOut
                                CreateRepeatabilityResultsFileOutput(myPath)

                            Else
                                ' Continue with the Test until finish it
                                Me.ProgressBar1.Value += 1
                                Me.ProgressBar1.Refresh()

                                myResultData = MyBase.Test
                                Me.RunningTest = True
                                If myResultData.HasError Then
                                    PrepareErrorMode()
                                    Exit Sub
                                Else
                                    If MyBase.SimulationMode Then
                                        ' simulating
                                        'MyBase.DisplaySimulationMessage("Repeatability Testing...")
                                        Me.Cursor = Cursors.WaitCursor
                                        System.Threading.Thread.Sleep(SimulationProcessTime)
                                        MyBase.myServiceMDI.Focus()
                                        Me.Cursor = Cursors.Default
                                        MyBase.CurrentMode = ADJUSTMENT_MODES.TESTED
                                        PrepareArea()
                                    Else
                                        If Not myResultData.HasError AndAlso Ax00ServiceMainMDI.MDIAnalyzerManager.Connected Then
                                            Me.Cursor = Cursors.WaitCursor

                                            ' XBC 30/10/2012
                                            System.Threading.Thread.Sleep(50)

                                            ' XBC 21/02/2012 - next measures have well already placed and need to execute BLIGHT fastest
                                            myResultData = myScreenDelegate.SendREAD_COUNTS(ADJUSTMENT_GROUPS.REPEATABILITY, True)
                                        End If
                                        If myResultData.HasError Then
                                            PrepareErrorMode()
                                            Exit Sub
                                        End If
                                    End If
                                End If
                            End If

                        Case ADJUSTMENT_GROUPS.STABILITY
                            '
                            ' STABILITY
                            '
                            If myScreenDelegate.TestStabilityDone Then
                                Me.ProgressBar1.Value = 0
                                Me.ProgressBar1.Visible = False
                                Me.ProgressBar1.Refresh()

                                Me.Cursor = Cursors.Default
                                Me.RunningTest = False
                                Me.FirstIteration = False
                                Me.BsStep2RepeatabilityRadioButton.Enabled = True
                                Me.BsStep2StabilityRadioButton.Enabled = True
                                Me.BsStep2AbsorbanceRadioButton.Enabled = True
                                Me.BsStep2WLCombo.Enabled = True
                                'If Me.BsStep2ManualFillRadioButton.Checked Then
                                Me.BsStep2WellUpDown.Enabled = True
                                'End If
                                Me.BsTestButton.Enabled = True
                                Me.BsExitButton.Enabled = True
                                Me.BsStep2AutoFillRadioButton.Enabled = True
                                Me.BsStep2ManualFillRadioButton.Enabled = True
                                Me.BsStep2DarkRadioButton.Enabled = True
                                Me.BsStep2LightRadioButton.Enabled = True
                                ' XBC 20/02/2012
                                Me.BsStep1UpDownWSButton.Enabled = True
                                Me.BsStep2UpDownWSButton.Enabled = True

                                'myResultData = myAnalyzerManager.ReadPhotometryData()
                                'If (Not myResultData.HasError And Not myResultData.SetDatos Is Nothing) Then
                                '    myPhotometryDataTO = DirectCast(myResultData.SetDatos, PhotometryDataTO)

                                ' Populate results of the readed WaveLengths to screen
                                MyBase.DisplayMessage(Messages.SRV_COMPLETED.ToString)
                                If Me.BsStep2DarkRadioButton.Checked Then
                                    ' DARK
                                    Me.BsStep2AbsCVResult.Visible = False
                                    Me.BsStep2CVevLabel.Visible = False
                                    PopulatephMainStep2(Me.BsStep2WLCombo.SelectedIndex)
                                    PopulatephRefStep2(Me.BsStep2WLCombo.SelectedIndex)
                                    PopulateStep2Chart(myScreenDelegate.MeasuresStabilityphMainCountsDKByLed(Me.BsStep2WLCombo.SelectedIndex), _
                                                       myScreenDelegate.MeasuresStabilityphRefCountsDKByLed(Me.BsStep2WLCombo.SelectedIndex))
                                    'myScreenDelegate.GetMaxValueStabilityResult(0), _
                                    'myScreenDelegate.GetMinValueStabilityResult(0))
                                    Step2Chart.Legend.Visibility = DevExpress.Utils.DefaultBoolean.True
                                ElseIf Me.BsStep2LightRadioButton.Checked Then
                                    ' LIGHT
                                    Me.BsStep2AbsCVResult.Visible = True
                                    Me.BsStep2CVevLabel.Visible = True
                                    PopulateABSStep2(Me.BsStep2WLCombo.SelectedIndex)
                                    PopulateStep2Chart(myScreenDelegate.MeasuresStabilityAbsorbances(Me.BsStep2WLCombo.SelectedIndex))
                                    Step2Chart.Legend.Visibility = DevExpress.Utils.DefaultBoolean.False
                                End If
                                'End If

                                ' Write Results into output file
                                myPath = Application.StartupPath & GlobalBase.PhotometryStabilityFileOut
                                CreateStabilityResultsFileOutput(myPath)

                            Else
                                ' Continue with the Test until finish it
                                Me.ProgressBar1.Value += 1
                                Me.ProgressBar1.Refresh()

                                myResultData = MyBase.Test
                                Me.RunningTest = True
                                If myResultData.HasError Then
                                    PrepareErrorMode()
                                    Exit Sub
                                Else
                                    If MyBase.SimulationMode Then
                                        ' simulating
                                        'MyBase.DisplaySimulationMessage("stability Testing...")
                                        Me.Cursor = Cursors.WaitCursor
                                        System.Threading.Thread.Sleep(SimulationProcessTime)
                                        MyBase.myServiceMDI.Focus()
                                        Me.Cursor = Cursors.Default
                                        MyBase.CurrentMode = ADJUSTMENT_MODES.TESTED
                                        PrepareArea()
                                    Else
                                        If Not myResultData.HasError AndAlso Ax00ServiceMainMDI.MDIAnalyzerManager.Connected Then
                                            Me.Cursor = Cursors.WaitCursor

                                            ' XBC 21/02/2012
                                            Dim elapsedSpan As New TimeSpan(Date.Now.Ticks)
                                            Dim currentTime As Double = elapsedSpan.TotalSeconds
                                            Dim nextTime As Double = currentTime + StabilityInterval
                                            While currentTime < nextTime
                                                Application.DoEvents()
                                                elapsedSpan = New TimeSpan(Date.Now.Ticks)
                                                currentTime = elapsedSpan.TotalSeconds
                                            End While

                                            ' XBC 21/02/2012 - next measures have well already placed and need to execute BLIGHT fastest
                                            myResultData = myScreenDelegate.SendREAD_COUNTS(ADJUSTMENT_GROUPS.STABILITY, True)
                                        End If
                                        If myResultData.HasError Then
                                            PrepareErrorMode()
                                            Exit Sub
                                        End If
                                    End If
                                End If
                            End If

                        Case ADJUSTMENT_GROUPS.ABSORBANCE_MEASUREMENT
                            '
                            ' ABSORBANCE MEASUREMENT
                            '
                            If myScreenDelegate.TestABSDone Then
                                Me.RunningTest = False
                                Me.FirstIteration = False
                                Me.BsStep2RepeatabilityRadioButton.Enabled = True
                                Me.BsStep2StabilityRadioButton.Enabled = True
                                Me.BsStep2AbsorbanceRadioButton.Enabled = True
                                Me.BsStep2WLCombo.Enabled = True
                                Me.BsStep2WellUpDown.Enabled = True
                                Me.BsTestButton.Enabled = True
                                Me.BsExitButton.Enabled = True
                                Me.BsStep2AutoFillRadioButton.Enabled = True
                                Me.BsStep2ManualFillRadioButton.Enabled = True
                                Me.BsStep2LightRadioButton.Enabled = True
                                Me.Cursor = Cursors.Default
                                ' XBC 20/02/2012
                                Me.BsStep1UpDownWSButton.Enabled = True
                                Me.BsStep2UpDownWSButton.Enabled = True

                                MyBase.DisplayMessage(Messages.SRV_COMPLETED.ToString)
                                ' Populate results of the readed Absorbances to screen
                                PopulateABSStep2(Me.BsStep2WLCombo.SelectedIndex)
                                ' Clear Chart
                                PopulateStep2Chart(myScreenDelegate.GetAbsorbanceABSResult, True)
                                Step2Chart.Legend.Visibility = DevExpress.Utils.DefaultBoolean.False
                            End If

                    End Select

                    'Case PHOTOMETRY_PAGES.CHECKROTOR
                    ' CANCELLED
            End Select

            If myScreenDelegate.CurrentTest <> ADJUSTMENT_GROUPS.IT_EDITION Then
                If myScreenDelegate.TestBaseLineDone And Not Me.RunningTest Then
                    Me.ManageTabPages = True
                    MyBase.ActivateMDIMenusButtons(True) 'SGM 27/09/2011
                Else
                    Me.ManageTabPages = False
                End If
            End If

            If myResultData.HasError Then
                PrepareErrorMode()
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".PrepareTestedMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PrepareTestedMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Prepare GUI for Test Exiting Mode
    ''' </summary>
    ''' <remarks>Created by XBC 15/03/2011</remarks>
    Private Sub PrepareTestExitingMode()
        Try
            DisableAll()
            Me.ManageTabPages = False

        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".PrepareTestExitingMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PrepareTestExitingMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Prepare GUI for Test Exited Mode
    ''' </summary>
    ''' <remarks>Created by XBC 15/03/2011</remarks>
    Private Sub PrepareTestExitedMode()
        Try
            If FlagExitingScreen Then
                ActivateMDIMenusButtons(True) 'SGM 27/09/2011
                Close()
            Else
                PrepareLoadedMode()
                MyBase.DisplayMessage(Messages.SRV_TEST_EXIT_COMPLETED.ToString)
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".PrepareTestExitedMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PrepareTestExitedMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Prepare GUI for Saving Mode
    ''' </summary>
    ''' <remarks>Created by XBC 15/03/2011</remarks>
    Private Sub PrepareSavingMode()
        Try
            DisableAll()
            Me.ManageTabPages = False

        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".PrepareSavingMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PrepareSavingMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Prepare GUI for Saved Mode
    ''' </summary>
    ''' <remarks>Created by XBC 15/03/2011</remarks>
    Private Sub PrepareSavedMode()
        Dim myGlobal As New GlobalDataTO
        Try
            myGlobal = MyBase.UpdateAdjustments(MyClass.SelectedAdjustmentsDS)

            If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                RestorePostItsEdition()
            Else
                PrepareErrorMode()
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".PrepareSavedMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PrepareSavedMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        Finally
            Me.Cursor = Cursors.Default
        End Try
    End Sub

    ''' <summary>
    ''' Prepare GUI for Parking Mode
    ''' </summary>
    ''' <remarks>Created by XBC 20/02/2012</remarks>
    Private Sub PrepareParkingMode()
        Try
            Me.Cursor = Cursors.WaitCursor

            DisableAll()
            Me.ManageTabPages = False

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareParkingMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareParkingMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Prepare GUI for Parked Mode
    ''' </summary>
    ''' <remarks>Created by XBC 20/02/2012</remarks>
    Private Sub PrepareParkedMode()
        Try
            Me.Cursor = Cursors.Default

            If myScreenDelegate.IsWashingStationUp Then
                Me.BsStep1UpDownWSButton.Enabled = True
                Me.BsStep2UpDownWSButton.Enabled = True
            Else
                Me.PrepareLoadedMode()
                MyBase.ActivateMDIMenusButtons(True)
            End If

            MyBase.DisplayMessage("")

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareParkedMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareParkedMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Prepare GUI for Error Mode
    ''' </summary>
    ''' <remarks>Created by XBC 23/02/2011</remarks>
    Public Overrides Sub PrepareErrorMode(Optional ByVal pAlarmType As ManagementAlarmTypes = ManagementAlarmTypes.NONE)
        Try
            MyBase.ErrorMode()
            Me.ProgressBar1.Visible = False
            DisableAll()
            Me.BsExitButton.Enabled = True ' Just Exit button is enabled in error case
            MyBase.ActivateMDIMenusButtons(True)
        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".PrepareErrorMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PrepareErrorMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        Finally
            Me.Cursor = Cursors.Default
        End Try
    End Sub

    ''' <summary>
    ''' Initializations for Baseline - Darnkess Counts tests TAB
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>Created by XBC 25/02/2011</remarks>
    Private Function PrepareScreen() As GlobalDataTO
        Dim myResultData As New GlobalDataTO
        Try
            'Dim myPreloadedMasterDataDS As New PreloadedMasterDataDS
            'Dim myAnalyLedPosDS As New AnalyzerLedPositionsDS
            Dim qLeds As List(Of AnalyzerLedPositionsDS.tcfgAnalyzerLedPositionsRow)

            'Me.ChangedValue = False
            Me.UnableHandlers = True

            ' Get WaveLengths
            myResultData = myScreenDelegate.ReadWaveLengths(Me.ActiveAnalyzer)
            If (Not myResultData.HasError And Not myResultData.SetDatos Is Nothing) Then
                'myAnalyLedPosDS = DirectCast(myResultData.SetDatos, AnalyzerLedPositionsDS)
                qLeds = DirectCast(myResultData.SetDatos, List(Of AnalyzerLedPositionsDS.tcfgAnalyzerLedPositionsRow))

                If qLeds.Count > 0 Then
                    'If myAnalyLedPosDS.tcfgAnalyzerLedPositions.Rows.Count > 0 Then
                    'order the result
                    'qLeds = (From a In myAnalyLedPosDS.tcfgAnalyzerLedPositions _
                    '         Order By a.LedPosition _
                    '         Select a Where a.Status = True).ToList()

                    ' Populate WLs into screen controls
                    '
                    ' STEP 1 DataGrid & WLList
                    '
                    ' Create DataGrid for current LEDs 
                    Me.BsDataGridViewLeds.Columns.Clear()
                    '' '' provisional
                    ' ''Me.BsDataGridViewLeds.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None
                    ' ''Me.BsDataGridViewLeds.CellBorderStyle = DataGridViewCellBorderStyle.Single
                    '' '' provisional

                    Me.BsWLsListView.Items.Clear()
                    Me.BsWLsListView.Columns.Clear()
                    Me.BsWLsListView.Columns.Add("WLs")

                    ' Create DataGrid for References LEDs
                    Me.BsDataGridViewLeds2.Columns.Clear()

                    Me.BsDataGridViewLedsChecks.Columns.Clear()

                    'ReDim Me.EditedValues(myAnalyLedPosDS.tcfgAnalyzerLedPositions.Rows.Count - 1)

                    'For Each row As AnalyzerLedPositionsDS.tcfgAnalyzerLedPositionsRow In myAnalyLedPosDS.tcfgAnalyzerLedPositions.Rows
                    For i As Integer = 0 To qLeds.Count - 1
                        'add columns DataGrid for current LEDs 
                        'Me.BsDataGridViewLeds.Columns.Add(row.WaveLength.ToString, row.WaveLength.ToString)
                        Me.BsDataGridViewLeds.Columns.Add(qLeds(i).WaveLength.ToString, qLeds(i).WaveLength.ToString)

                        '' '' provisional
                        '' ''add columns DataGrid for icons image warnings LEDs 
                        ' ''Dim myImageButtonCol As New DataGridViewImageColumn
                        ' ''myImageButtonCol.Name = row.WaveLength.ToString & "_img"
                        ' ''myImageButtonCol.HeaderText = ""
                        ' ''Me.BsDataGridViewLeds.Columns.Add(myImageButtonCol)
                        '' ''Me.BsDataGridViewLeds.Columns(myImageButtonCol.Name).HeaderCell.Style.BackColor = Me.BackColor
                        '' ''Me.BsDataGridViewLeds.Columns(myImageButtonCol.Name).DefaultCellStyle.BackColor = Me.BackColor
                        '' ''Me.BsDataGridViewLeds.Columns(myImageButtonCol.Name).Width = 40
                        '' '' provisional


                        'add columns DataGrid for References LEDs 
                        'Me.BsDataGridViewLeds2.Columns.Add(row.WaveLength.ToString, row.WaveLength.ToString)
                        Me.BsDataGridViewLeds2.Columns.Add(qLeds(i).WaveLength.ToString, qLeds(i).WaveLength.ToString)
                        'add columns DataGrid for References checks LEDs 
                        Dim myCheckButtonCol As New DataGridViewCheckBoxColumn
                        'myCheckButtonCol.Name = row.WaveLength.ToString
                        myCheckButtonCol.Name = qLeds(i).WaveLength.ToString
                        Me.BsDataGridViewLedsChecks.Columns.Add(myCheckButtonCol)

                        'Me.BsDataGridViewLeds2.Columns.Add(row.WaveLength.ToString, row.WaveLength.ToString)
                        'add rows for List of WaveLengths
                        'Me.BsWLsListView.Items.Add(row.WaveLength.ToString)
                        Me.BsWLsListView.Items.Add(qLeds(i).WaveLength.ToString)

                        ' Initialize EditedValues for Current Leds of Reference
                        'Me.InitializeEditedValues(row.WaveLength.ToString, row.LedPosition.ToString)
                        Me.InitializeEditedValues(qLeds(i).WaveLength.ToString, qLeds(i).LedPosition.ToString)
                    Next
                    '
                    ' STEP 2 ComboWL
                    '
                    Me.BsStep2WLCombo.DataSource = qLeds
                    Me.BsStep2WLCombo.DisplayMember = "WaveLength"
                    Me.BsStep2WLCombo.ValueMember = "LedPosition"
                    ''
                    '' STEP 3 ComboWL
                    ''
                    'Me.BsStep3WLCombo.DataSource = qLeds
                    'Me.BsStep3WLCombo.DisplayMember = "WaveLength"
                    'Me.BsStep3WLCombo.ValueMember = "LedPosition"
                End If
            Else
                'Unexpected error in the import from preparing Tab controls process, show the message
                PrepareErrorMode()
                ShowMessage(Me.Name & ".PrepareScreen", myResultData.ErrorCode, myResultData.ErrorMessage, Me)
            End If

            Me.UnableHandlers = False

        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".PrepareScreen ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PrepareScreen ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
        Return myResultData
    End Function

    ''' <summary>
    ''' Initialize specific selected adjustment related structure
    ''' </summary>
    ''' <remarks>SGM 16/05/2011</remarks>
    Private Sub InitializeEditedValues(ByVal pWaveLength As String, ByVal pLedPosition As String)
        Try
            Dim myLed As New EditedValueStruct

            With myLed
                .WaveLength = pWaveLength
                .LedPos = "ILED" + pLedPosition
                .CurrentValue = Nothing
                .NewValue = Nothing
            End With

            If Me.EditedValues Is Nothing Then
                ReDim Me.EditedValues(0)
            Else
                ReDim Preserve Me.EditedValues(Me.EditedValues.Length)
            End If

            Me.EditedValues(Me.EditedValues.Length - 1) = myLed

        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".InitializeEditedValues ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".InitializeEditedValues ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Get Limits values from BD for every different arm
    ''' </summary>
    ''' <remarks>Created by XBC 30/03/2011</remarks>
    Private Function GetLimitValues() As GlobalDataTO
        Dim myGlobalDataTO As New GlobalDataTO
        '  Dim myFieldLimitsDS As New FieldLimitsDS
        Try
            myGlobalDataTO = myScreenDelegate.GetLimitValues()

            'myGlobalDataTO = GetControlsLimits(FieldLimitsEnum.SRV_LEDS_LIMIT)
            'If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
            '    myFieldLimitsDS = CType(myGlobalDataTO.SetDatos, FieldLimitsDS)
            '    If myFieldLimitsDS.tfmwFieldLimits.Rows.Count > 0 Then
            '        Me.LimitMinLEDs = CType(myFieldLimitsDS.tfmwFieldLimits(0).MinValue, Long)
            '        Me.LimitMaxLEDs = CType(myFieldLimitsDS.tfmwFieldLimits(0).MaxValue, Long)
            '    End If
            'Else
            '    ShowMessage(Me.Name & ".GetLimitValues", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage, Me)
            'End If

            'myGlobalDataTO = GetControlsLimits(FieldLimitsEnum.SRV_BL_PhMAIN_LIMIT)
            'If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
            '    myFieldLimitsDS = CType(myGlobalDataTO.SetDatos, FieldLimitsDS)
            '    If myFieldLimitsDS.tfmwFieldLimits.Rows.Count > 0 Then
            '        Me.LimitMinPhMain = CType(myFieldLimitsDS.tfmwFieldLimits(0).MinValue, Long)
            '        Me.LimitMaxPhMain = CType(myFieldLimitsDS.tfmwFieldLimits(0).MaxValue, Long)
            '    End If
            'Else
            '    ShowMessage(Me.Name & ".GetLimitValues", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage, Me)
            'End If

            'myGlobalDataTO = GetControlsLimits(FieldLimitsEnum.SRV_BL_PhREF_LIMIT)
            'If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
            '    myFieldLimitsDS = CType(myGlobalDataTO.SetDatos, FieldLimitsDS)
            '    If myFieldLimitsDS.tfmwFieldLimits.Rows.Count > 0 Then
            '        Me.LimitMinPhRef = CType(myFieldLimitsDS.tfmwFieldLimits(0).MinValue, Long)
            '        Me.LimitMaxPhRef = CType(myFieldLimitsDS.tfmwFieldLimits(0).MaxValue, Long)
            '    End If
            'Else
            '    ShowMessage(Me.Name & ".GetLimitValues", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage, Me)
            'End If


        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".GetLimitValues ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".GetLimitValues ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
        Return myGlobalDataTO
    End Function

    '''' <summary>
    '''' Method incharge to get the controls limits value. 
    '''' </summary>
    '''' <param name="pLimitsID">Limit to get</param>
    '''' <returns></returns>
    '''' <remarks>Created by XBC 30/03/2011</remarks>
    'Private Function GetControlsLimits(ByVal pLimitsID As FieldLimitsEnum) As GlobalDataTO
    '    Dim myGlobalDataTO As New GlobalDataTO
    '    Dim myFieldLimitsDS As New FieldLimitsDS
    '    Try
    '        Dim myFieldLimitsDelegate As New FieldLimitsDelegate()
    '        'Load the specified limits
    '        myGlobalDataTO = myFieldLimitsDelegate.GetList(Nothing, pLimitsID)
    '    Catch ex As Exception
    '        'Write error SYSTEM_ERROR in the Application Log
    '        CreateLogActivity(ex.Message, Me.Name & " GetControlsLimits ", EventLogEntryType.Error, _
    '                                                        GetApplicationInfoSession().ActivateSystemLog)
    '        'Show error message
    '        ShowMessage(Me.Name & " GetControlsLimits ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
    '    End Try
    '    Return myGlobalDataTO
    'End Function

    ''' <summary>
    ''' Get Limits values from BD for Photometry Tests
    ''' </summary>
    ''' <remarks>Created by XBC 24/02/2011</remarks>
    Private Function GetParameters(ByVal pAnalyzerModel As String) As GlobalDataTO
        Dim myGlobalDataTO As New GlobalDataTO
        Try
            myGlobalDataTO = myScreenDelegate.GetParameters(pAnalyzerModel)

            ' Populate here parameters values which are needed for presentation layer ...

        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".GetParameters ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".GetParameters ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
        Return myGlobalDataTO
    End Function

    ''' <summary>
    ''' Fill datagrid with the values of the leds intensities
    ''' </summary>
    ''' <param name="pLEDsIntensities"></param>
    ''' <remarks>Created by XBC 28/02/2011</remarks>
    Private Sub PopulateLEDSIntensity(ByVal pLEDsIntensities As List(Of Single))
        Dim validationOK As Boolean = True
        Try
            If pLEDsIntensities Is Nothing Then
                validationOK = False
            ElseIf Me.BsDataGridViewLeds Is Nothing Then
                validationOK = False
            ElseIf pLEDsIntensities.Count <> Me.BsDataGridViewLeds.ColumnCount Then
                validationOK = False
            End If

            If validationOK Then
                Me.BsDataGridViewLeds.Rows.Clear()
                Me.BsDataGridViewLeds.Rows.Add()
                'Me.BsDataGridViewLeds.Rows(0).HeaderCell.Value = "LED Current"
                ' OLD
                'For i As Integer = 0 To pLEDsIntensities.Count - 1
                '    Me.BsDataGridViewLeds.Rows(0).Cells(i).Value = pLEDsIntensities(i)
                'Next


                ' provisional
                Dim numWarnings As Integer = 0
                For i As Integer = 0 To pLEDsIntensities.Count - 1
                    Me.BsDataGridViewLeds.Rows(0).Cells(i).Value = pLEDsIntensities(i)
                    If pLEDsIntensities(i) >= myScreenDelegate.LimitMinLEDs And pLEDsIntensities(i) <= myScreenDelegate.LimitMaxLEDs Then
                        ' is into correct limits
                        'Me.BsDataGridViewLeds.Rows(0).Cells((i * 2) + 1).Value = VisibleIcon(False)
                    Else
                        ' isn't correct limits
                        'Me.BsDataGridViewLeds.Rows(0).Cells((i * 2) + 1).Value = VisibleIcon(True)
                        Me.BsDataGridViewLeds.Rows(0).Cells(i).ErrorText = "warning"
                        numWarnings += 1
                    End If
                Next

                If numWarnings > myScreenDelegate.MaxLEDsWarnings Then
                    MyBase.DisplayMessage(Messages.SRV_MAX_LEDS_WARNINGS.ToString)
                End If
                ' provisional

                Me.BsDataGridViewLeds.Refresh()
                Me.BsDataGridViewLeds.CurrentCell.Selected = False
            Else
                'Unexpected error preparing Tab controls process, show the message
                PrepareErrorMode()
                ShowMessage(Me.Name & ".PopulateLEDSIntensity", "ID_MSG PDT !!!", "TXT_MSG PDT !!!", Me)
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".PopulateLEDSIntensity ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PopulateLEDSIntensity ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Public Function VisibleIcon(ByVal visible As Boolean) As Bitmap
        Dim bm_dest As Bitmap = Nothing
        Try
            If visible Then
                bm_dest = Me.myWarningImage
            Else
                bm_dest = Me.myClearImage
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".VisibleIcon ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".VisibleIcon ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
        Return bm_dest
    End Function

    ''' <summary>
    ''' Fill controls with the values of the counts Main Photodiode light BL test
    ''' </summary>
    ''' <param name="pCounts"></param>
    ''' <remarks>Created by XBC 02/03/2011</remarks>
    Private Sub PopulatephMainBL(ByVal pCounts As List(Of Single))
        Try
            Me.BsphMainBLListView.Items.Clear()
            Me.BsphMainBLListView.Columns.Clear()
            Me.BsphMainBLListView.Columns.Add("phMain")
            For i As Integer = 0 To pCounts.Count - 1
                Me.BsphMainBLListView.Items.Add(pCounts(i).ToString("#,##0"))
            Next
            BsphMainBLListView.Refresh()

            Me.BsphMainWarningsListView.Items.Clear()
            Me.BsphMainWarningsListView.Columns.Clear()
            Me.BsphMainWarningsListView.Columns.Add("phMain")
            Dim numWarnings As Integer = 0
            For i As Integer = 0 To pCounts.Count - 1
                If pCounts(i) >= myScreenDelegate.LimitMinPhMain And pCounts(i) <= myScreenDelegate.LimitMaxPhMain Then
                    ' is into correct limits
                    Me.BsphMainWarningsListView.Items.Add("")
                    Me.BsphMainWarningsListView.Items(i).StateImageIndex = -1
                Else
                    ' isn't correct limits
                    Me.BsphMainWarningsListView.Items.Add("")
                    Me.BsphMainWarningsListView.Items(i).StateImageIndex = 0
                    numWarnings += 1
                End If
            Next
            BsphMainBLListView.Refresh()

            If numWarnings > myScreenDelegate.MaxLEDsWarnings Then
                MyBase.DisplayMessage(Messages.SRV_MAX_LEDS_WARNINGS.ToString)
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".PopulatephMainBL ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PopulatephMainBL ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Fill controls with the values of the counts Reference Photodiode light BL test
    ''' </summary>
    ''' <param name="pCounts"></param>
    ''' <remarks>Created by XBC 02/03/2011</remarks>
    Private Sub PopulatephRefBL(ByVal pCounts As List(Of Single))
        Try
            Me.BsphRefBLListView.Items.Clear()
            Me.BsphRefBLListView.Columns.Clear()
            Me.BsphRefBLListView.Columns.Add("phRef")
            For i As Integer = 0 To pCounts.Count - 1
                Me.BsphRefBLListView.Items.Add(pCounts(i).ToString("#,##0"))
            Next
            BsphRefBLListView.Refresh()

            Me.BsphRefWarningsListView.Items.Clear()
            Me.BsphRefWarningsListView.Columns.Clear()
            Me.BsphRefWarningsListView.Columns.Add("phMain")
            Dim numWarnings As Integer = 0
            For i As Integer = 0 To pCounts.Count - 1
                If pCounts(i) >= myScreenDelegate.LimitMinPhRef And pCounts(i) <= myScreenDelegate.LimitMaxPhRef Then
                    ' is into correct limits
                    Me.BsphRefWarningsListView.Items.Add("")
                    Me.BsphRefWarningsListView.Items(i).StateImageIndex = -1
                Else
                    ' isn't correct limits
                    Me.BsphRefWarningsListView.Items.Add("")
                    Me.BsphRefWarningsListView.Items(i).StateImageIndex = 0
                    numWarnings += 1
                End If
            Next
            BsphMainBLListView.Refresh()

            If numWarnings > myScreenDelegate.MaxLEDsWarnings Then
                MyBase.DisplayMessage(Messages.SRV_MAX_LEDS_WARNINGS.ToString)
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".PopulatephRefBL ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PopulatephRefBL ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Paint the chart with the values of the counts Main Photodiode light BL test
    ''' </summary>
    ''' <param name="pCountsphMain"></param>
    ''' <param name="pCountsphRef"></param>
    ''' <remarks>Created by XBC 03/03/2011</remarks>
    Private Sub PopulateBLChart(ByVal pCountsphMain As List(Of Single), ByVal pCountsphRef As List(Of Single))
        Try
            ' Clear series
            If BSStep1BLChart.Series(0).Points.Count > 0 Then
                BSStep1BLChart.Series(0).Points.RemoveRange(0, BSStep1BLChart.Series(0).Points.Count)
            End If
            If BSStep1BLChart.Series(1).Points.Count > 0 Then
                BSStep1BLChart.Series(1).Points.RemoveRange(0, BSStep1BLChart.Series(1).Points.Count)
            End If

            BSStep1BLChart.CrosshairEnabled = DevExpress.Utils.DefaultBoolean.False
            BSStep1BLChart.RuntimeHitTesting = True

            If Not pCountsphMain Is Nothing AndAlso Not pCountsphRef Is Nothing AndAlso pCountsphMain.Count > 0 AndAlso pCountsphRef.Count > 0 Then
                ' Configure Ranges

                ' XBC 01/03/2012
                'Dim myMax As Single = Math.Max(pCountsphMain.Max, pCountsphRef.Max)
                'Dim myMin As Single = Math.Min(pCountsphMain.Min, pCountsphRef.Min)
                Dim myMax As Single = Math.Max(pCountsphMain.Max, pCountsphRef.Max) * CSng(1.1) ' + 10%
                Dim myMin As Single = 0
                ' XBC 01/03/2012

                If CLng(CType(BSStep1BLChart.Diagram, XYDiagram).AxisY.VisualRange.MinValue) >= myMax Then
                    CType(BSStep1BLChart.Diagram, XYDiagram).AxisY.WholeRange.MinValue = myMin
                    CType(BSStep1BLChart.Diagram, XYDiagram).AxisY.WholeRange.MaxValue = myMax
                    CType(BSStep1BLChart.Diagram, XYDiagram).AxisY.VisualRange.MinValue = myMin
                    CType(BSStep1BLChart.Diagram, XYDiagram).AxisY.VisualRange.MaxValue = myMax
                Else
                    CType(BSStep1BLChart.Diagram, XYDiagram).AxisY.WholeRange.MaxValue = myMax
                    CType(BSStep1BLChart.Diagram, XYDiagram).AxisY.VisualRange.MaxValue = myMax
                    If myMax = myMin Then
                        CType(BSStep1BLChart.Diagram, XYDiagram).AxisY.WholeRange.MinValue = myMin - 1
                        CType(BSStep1BLChart.Diagram, XYDiagram).AxisY.VisualRange.MinValue = myMin - 1
                    Else
                        CType(BSStep1BLChart.Diagram, XYDiagram).AxisY.WholeRange.MinValue = myMin
                        CType(BSStep1BLChart.Diagram, XYDiagram).AxisY.VisualRange.MinValue = myMin
                    End If
                End If

                CType(BSStep1BLChart.Diagram, XYDiagram).AxisY.NumericScaleOptions.AutoGrid = True

                'CType(BSStep1BLChart.Diagram, XYDiagram).AxisX.Range.MaxValue = pCountsphMain.Count + 1
                'CType(BSStep1BLChart.Diagram, XYDiagram).AxisX.Range.MinValue = 0
                CType(BSStep1BLChart.Diagram, XYDiagram).AxisX.NumericScaleOptions.AutoGrid = True

                'IT'S MANDATORY TO DEFINE SIDEMARGINSVALUE = 0 FOR PREVENTING LEAVING SPACES IN THE AXES.
                CType(BSStep1BLChart.Diagram, XYDiagram).AxisY.VisualRange.SideMarginsValue = 0

                ' Populate new values
                For i As Integer = 0 To pCountsphMain.Count - 1
                    BSStep1BLChart.Series(0).Points.Add(New SeriesPoint(i + 1, pCountsphMain(i)))
                    BSStep1BLChart.Series(1).Points.Add(New SeriesPoint(i + 1, pCountsphRef(i)))
                Next

                Me.BSStep1BLChart.Refresh()
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".PopulateBLChart ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PopulateBLChart ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Fill controls with the values of the counts Main Photodiode light Repeatability/Stability test
    ''' </summary>
    ''' <remarks>Created by XBC 08/03/2011</remarks>
    Private Sub PopulateABSStep2(ByVal pLedPosition As Integer)
        Try
            If Me.BsStep2RepeatabilityRadioButton.Checked Then
                Me.BsStep2AbsMeanResult.Text = myScreenDelegate.GetAbsorbanceRepeatabilityResult(pLedPosition).ToString("#,##0.0000")
                Me.BsStep2AbsSDResult.Text = myScreenDelegate.GetAbsorbanceDeviationRepeatabilityResult(pLedPosition).ToString("#,##0.0000")
                Me.BsStep2AbsCVResult.Text = myScreenDelegate.GetCVAbsorbanceRepeatabilityResults(pLedPosition)
                Me.BsStep2AbsMaxResult.Text = myScreenDelegate.GetMAXAbsorbanceRepeatabilityResult(pLedPosition).ToString("#,##0.0000")
                Me.BsStep2AbsMinResult.Text = myScreenDelegate.GetMINAbsorbanceRepeatabilityResult(pLedPosition).ToString("#,##0.0000")
                Me.BsStep2AbsRangeResult.Text = myScreenDelegate.GetRangeAbsorbanceRepeatabilityResult(pLedPosition).ToString("#,##0.0000")
            ElseIf Me.BsStep2StabilityRadioButton.Checked Then
                Me.BsStep2AbsMeanResult.Text = myScreenDelegate.GetAbsorbanceStabilityResult(pLedPosition).ToString("#,##0.0000")
                Me.BsStep2AbsSDResult.Text = myScreenDelegate.GetAbsorbanceDeviationStabilityResult(pLedPosition).ToString("#,##0.0000")
                Me.BsStep2AbsCVResult.Text = myScreenDelegate.GetCVAbsorbanceStabilityResults(pLedPosition)
                Me.BsStep2AbsMaxResult.Text = myScreenDelegate.GetMAXAbsorbanceStabilityResult(pLedPosition).ToString("#,##0.0000")
                Me.BsStep2AbsMinResult.Text = myScreenDelegate.GetMINAbsorbanceStabilityResult(pLedPosition).ToString("#,##0.0000")
                Me.BsStep2AbsRangeResult.Text = myScreenDelegate.GetRangeAbsorbanceStabilityResult(pLedPosition).ToString("#,##0.0000")
            ElseIf Me.BsStep2AbsorbanceRadioButton.Checked Then
                ' XBC 01/03/2012
                If myScreenDelegate.GetAbsorbanceABSResult(pLedPosition) = 99999 Then
                    Me.BsStep2AbsMeanResult.Text = " > " & myScreenDelegate.MaxAbsToDisplay.ToString("#,##0.0000")
                Else
                    Me.BsStep2AbsMeanResult.Text = myScreenDelegate.GetAbsorbanceABSResult(pLedPosition).ToString("#,##0.0000")
                End If
                ' XBC 01/03/2012
                Me.BsStep2AbsSDResult.Text = ""
                Me.BsStep2AbsCVResult.Text = ""
                Me.BsStep2AbsMaxResult.Text = ""
                Me.BsStep2AbsMinResult.Text = ""
                Me.BsStep2AbsRangeResult.Text = ""
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".PopulateABSStep2 ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PopulateABSStep2 ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Fill controls with the values of the counts Main Photodiode dark Repeatability/Stability test
    ''' </summary>
    ''' <remarks>Created by XBC 08/03/2011</remarks>
    Private Sub PopulatephMainStep2(ByVal pLedPosition As Integer)
        Try
            If Me.BsStep2RepeatabilityRadioButton.Checked Then
                Me.BsStep2phMainMeanResult.Text = myScreenDelegate.GetPhMainCountsRepeatabilityResultDK(pLedPosition).ToString("#,##0")
                Me.BsStep2phMainSDResult.Text = myScreenDelegate.GetSTDDeviationPhMainRepeatabilityResultDK(pLedPosition).ToString("#,##0")
                Me.BsStep2phMainMaxResult.Text = myScreenDelegate.GetMAXPhMainRepeatabilityResultDK(pLedPosition).ToString("#,##0")
                Me.BsStep2phMainMinResult.Text = myScreenDelegate.GetMINPhMainRepeatabilityResultDK(pLedPosition).ToString("#,##0")
                Me.BsStep2phMainRangeResult.Text = myScreenDelegate.GetRangePhMainRepeatabilityResultDK(pLedPosition).ToString("#,##0")
            ElseIf Me.BsStep2StabilityRadioButton.Checked Then
                Me.BsStep2phMainMeanResult.Text = myScreenDelegate.GetPhMainCountsStabilityResultDK(pLedPosition).ToString("#,##0")
                Me.BsStep2phMainSDResult.Text = myScreenDelegate.GetSTDDeviationPhMainStabilityResultDK(pLedPosition).ToString("#,##0")
                Me.BsStep2phMainMaxResult.Text = myScreenDelegate.GetMAXPhMainStabilityResultDK(pLedPosition).ToString("#,##0")
                Me.BsStep2phMainMinResult.Text = myScreenDelegate.GetMINPhMainStabilityResultDK(pLedPosition).ToString("#,##0")
                Me.BsStep2phMainRangeResult.Text = myScreenDelegate.GetRangePhMainStabilityResultDK(pLedPosition).ToString("#,##0")
            ElseIf Me.BsStep2AbsorbanceRadioButton.Checked Then
                ' Nothing
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".PopulatephMainStep2 ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PopulatephMainStep2 ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Fill controls with the values of the counts Reference Photodiode dark Repeatability/Stability test
    ''' </summary>
    ''' <remarks>Created by XBC 02/08/2011</remarks>
    Private Sub PopulatephRefStep2(ByVal pLedPosition As Integer)
        Try
            If Me.BsStep2RepeatabilityRadioButton.Checked Then
                Me.BsStep2phRefMeanResult.Text = myScreenDelegate.GetPhRefCountsRepeatabilityResultDK(pLedPosition).ToString("#,##0")
                Me.BsStep2phRefSDResult.Text = myScreenDelegate.GetSTDDeviationPhRefRepeatabilityResultDK(pLedPosition).ToString("#,##0")
                Me.BsStep2phRefMaxResult.Text = myScreenDelegate.GetMAXPhRefRepeatabilityResultDK(pLedPosition).ToString("#,##0")
                Me.BsStep2phRefMinResult.Text = myScreenDelegate.GetMINPhRefRepeatabilityResultDK(pLedPosition).ToString("#,##0")
                Me.BsStep2phRefRangeResult.Text = myScreenDelegate.GetRangePhRefRepeatabilityResultDK(pLedPosition).ToString("#,##0")
            ElseIf Me.BsStep2StabilityRadioButton.Checked Then
                Me.BsStep2phRefMeanResult.Text = myScreenDelegate.GetPhRefCountsStabilityResultDK(pLedPosition).ToString("#,##0")
                Me.BsStep2phRefSDResult.Text = myScreenDelegate.GetSTDDeviationPhRefStabilityResultDK(pLedPosition).ToString("#,##0")
                Me.BsStep2phRefMaxResult.Text = myScreenDelegate.GetMAXPhRefStabilityResultDK(pLedPosition).ToString("#,##0")
                Me.BsStep2phRefMinResult.Text = myScreenDelegate.GetMINPhRefStabilityResultDK(pLedPosition).ToString("#,##0")
                Me.BsStep2phRefRangeResult.Text = myScreenDelegate.GetRangePhRefStabilityResultDK(pLedPosition).ToString("#,##0")
            ElseIf Me.BsStep2AbsorbanceRadioButton.Checked Then
                ' Nothing
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".PopulatephRefStep2 ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PopulatephRefStep2 ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Paint the chart with the values of the counts Main Photodiode light Repeatability/Stability test
    ''' </summary>
    ''' <param name="pCounts"></param>
    ''' <remarks>Created by XBC 08/03/2011</remarks>
    Private Sub PopulateStep2Chart(ByVal pCounts As List(Of Single), Optional ByVal pABS As Boolean = False)
        Try

            If pABS Then
                ' XBC 24/02/2012
                Me.Step2ChartAbs.Visible = True
                Me.Step2Chart.Visible = False

                ' Clear chart
                If Step2ChartAbs.Series(0).Points.Count > 0 Then
                    Step2ChartAbs.Series(0).Points.RemoveRange(0, Step2ChartAbs.Series(0).Points.Count)
                End If

                'ADDITIONAL CONFIGURATION BECAUSE OF BEHAVIOUR CHANGES IN NEW LIBRARY VERSION
                Step2ChartAbs.CrosshairEnabled = DevExpress.Utils.DefaultBoolean.False
                Step2ChartAbs.RuntimeHitTesting = True

                If Not pCounts Is Nothing AndAlso pCounts.Count > 0 Then
                    ' Configure Ranges
                    If pCounts.Max = 0 And pCounts.Min = 0 Then
                        Exit Sub
                    End If

                    ' Initializating...
                    CType(Step2ChartAbs.Diagram, XYDiagram).AxisY.WholeRange.MinValue = -1000
                    CType(Step2ChartAbs.Diagram, XYDiagram).AxisY.WholeRange.MaxValue = 1000
                    CType(Step2ChartAbs.Diagram, XYDiagram).AxisY.VisualRange.MinValue = -1000
                    CType(Step2ChartAbs.Diagram, XYDiagram).AxisY.VisualRange.MaxValue = 1000

                    ' Setting values...
                    If pCounts.Max = pCounts.Min Then
                        CType(Step2ChartAbs.Diagram, XYDiagram).AxisY.WholeRange.MinValue = pCounts.Min - 1
                        CType(Step2ChartAbs.Diagram, XYDiagram).AxisY.VisualRange.MinValue = pCounts.Min - 1
                        If pCounts.Max = 99999 Then
                            CType(Step2ChartAbs.Diagram, XYDiagram).AxisY.WholeRange.MaxValue = myScreenDelegate.MaxAbsToDisplay
                            CType(Step2ChartAbs.Diagram, XYDiagram).AxisY.VisualRange.MaxValue = myScreenDelegate.MaxAbsToDisplay
                        Else
                        End If
                        CType(Step2ChartAbs.Diagram, XYDiagram).AxisY.WholeRange.MaxValue = pCounts.Max
                        CType(Step2ChartAbs.Diagram, XYDiagram).AxisY.VisualRange.MaxValue = pCounts.Max
                    Else
                        CType(Step2ChartAbs.Diagram, XYDiagram).AxisY.WholeRange.MinValue = pCounts.Min
                        CType(Step2ChartAbs.Diagram, XYDiagram).AxisY.VisualRange.MinValue = pCounts.Min
                        If pCounts.Max = 99999 Then
                            CType(Step2ChartAbs.Diagram, XYDiagram).AxisY.WholeRange.MaxValue = myScreenDelegate.MaxAbsToDisplay
                            CType(Step2ChartAbs.Diagram, XYDiagram).AxisY.VisualRange.MaxValue = myScreenDelegate.MaxAbsToDisplay
                        Else
                            CType(Step2ChartAbs.Diagram, XYDiagram).AxisY.WholeRange.MaxValue = pCounts.Max
                            CType(Step2ChartAbs.Diagram, XYDiagram).AxisY.VisualRange.MaxValue = pCounts.Max
                        End If
                    End If

                    CType(Step2ChartAbs.Diagram, XYDiagram).AxisY.NumericScaleOptions.AutoGrid = True

                    ' Populate new values
                    For i As Integer = 0 To pCounts.Count - 1
                        If pCounts(i) = 99999 Then
                            ' Error case
                            Step2ChartAbs.Series(0).Points.Add(New SeriesPoint(i + 1))
                        Else
                            Step2ChartAbs.Series(0).Points.Add(New SeriesPoint(i + 1, pCounts(i)))
                        End If
                    Next
                End If
                ' XBC 24/02/2012

            Else
                Me.Step2ChartAbs.Visible = False
                Me.Step2Chart.Visible = True

                ' Clear chart
                If Step2Chart.Series(0).Points.Count > 0 Then
                    Step2Chart.Series(0).Points.RemoveRange(0, Step2Chart.Series(0).Points.Count)
                End If
                If Step2Chart.Series(1).Points.Count > 0 Then
                    Step2Chart.Series(1).Points.RemoveRange(0, Step2Chart.Series(1).Points.Count)
                End If

                'ADDITIONAL CONFIGURATION BECAUSE OF BEHAVIOUR CHANGES IN NEW LIBRARY VERSION
                Step2Chart.CrosshairEnabled = DevExpress.Utils.DefaultBoolean.False
                Step2Chart.RuntimeHitTesting = True

                If Not pCounts Is Nothing AndAlso pCounts.Count > 0 Then
                    ' Configure Ranges
                    If pCounts.Max = 0 And pCounts.Min = 0 Then
                        Exit Sub
                    End If

                    ' Initializating...
                    CType(Step2Chart.Diagram, SwiftPlotDiagram).AxisY.WholeRange.MinValue = -1000
                    CType(Step2Chart.Diagram, SwiftPlotDiagram).AxisY.WholeRange.MaxValue = 1000
                    CType(Step2Chart.Diagram, SwiftPlotDiagram).AxisY.VisualRange.MinValue = -1000
                    CType(Step2Chart.Diagram, SwiftPlotDiagram).AxisY.VisualRange.MaxValue = 1000

                    ' Setting values...
                    If pCounts.Max = pCounts.Min Then
                        CType(Step2Chart.Diagram, SwiftPlotDiagram).AxisY.WholeRange.MinValue = pCounts.Min - 1
                        CType(Step2Chart.Diagram, SwiftPlotDiagram).AxisY.WholeRange.MaxValue = pCounts.Max
                        CType(Step2Chart.Diagram, SwiftPlotDiagram).AxisY.VisualRange.MinValue = pCounts.Min - 1
                        CType(Step2Chart.Diagram, SwiftPlotDiagram).AxisY.VisualRange.MaxValue = pCounts.Max
                    Else
                        CType(Step2Chart.Diagram, SwiftPlotDiagram).AxisY.WholeRange.MinValue = pCounts.Min
                        CType(Step2Chart.Diagram, SwiftPlotDiagram).AxisY.WholeRange.MaxValue = pCounts.Max
                        CType(Step2Chart.Diagram, SwiftPlotDiagram).AxisY.VisualRange.MinValue = pCounts.Min
                        CType(Step2Chart.Diagram, SwiftPlotDiagram).AxisY.VisualRange.MaxValue = pCounts.Max
                    End If


                    'If CLng(CType(Step2Chart.Diagram, SwiftPlotDiagram).AxisY.Range.MinValue) >= pCounts.Max Then
                    '    CType(Step2Chart.Diagram, SwiftPlotDiagram).AxisY.Range.MinValue = pCounts.Min
                    '    CType(Step2Chart.Diagram, SwiftPlotDiagram).AxisY.Range.MaxValue = pCounts.Max
                    'Else
                    '    CType(Step2Chart.Diagram, SwiftPlotDiagram).AxisY.Range.MaxValue = pCounts.Max
                    '    If pCounts.Max = pCounts.Min Then
                    '        CType(Step2Chart.Diagram, SwiftPlotDiagram).AxisY.Range.MinValue = pCounts.Min - 1
                    '    Else
                    '        CType(Step2Chart.Diagram, SwiftPlotDiagram).AxisY.Range.MinValue = pCounts.Min
                    '    End If
                    'End If

                    CType(Step2Chart.Diagram, SwiftPlotDiagram).AxisY.NumericScaleOptions.AutoGrid = True

                    ' XBC 21/02/2012
                    'CType(Step2Chart.Diagram, SwiftPlotDiagram).AxisX.Range.MaxValue = pCounts.Count + 1
                    'CType(Step2Chart.Diagram, SwiftPlotDiagram).AxisX.Range.MinValue = 0
                    'CType(Step2Chart.Diagram, SwiftPlotDiagram).AxisX.GridSpacingAuto = True
                    ' XBC 21/02/2012

                    ' Populate new values
                    For i As Integer = 0 To pCounts.Count - 1
                        Step2Chart.Series(0).Points.Add(New SeriesPoint(i + 1, pCounts(i)))
                    Next
                End If


            End If

            'AJG IT'S MANDATORY TO DEFINE SIDEMARGINSVALUE = 0 FOR PREVENTING LEAVING SPACES IN THE AXES.
            CType(Step2Chart.Diagram, SwiftPlotDiagram).AxisX.VisualRange.SideMarginsValue = 0
            CType(Step2Chart.Diagram, SwiftPlotDiagram).AxisY.VisualRange.SideMarginsValue = 0

        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".PopulateStep2Chart ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PopulateStep2Chart ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Paint the chart with the values of the counts Main Photodiode light Repeatability/Stability test
    ''' </summary>
    ''' <param name="pCountsphMain"></param>
    ''' <param name="pCountsphRef"></param>
    ''' <remarks>Created by XBC 08/03/2011</remarks>
    Private Sub PopulateStep2Chart(ByVal pCountsphMain As List(Of Single), _
                                   ByVal pCountsphRef As List(Of Single))
        Try
            ' Clear chart
            If Step2Chart.Series(0).Points.Count > 0 Then
                Step2Chart.Series(0).Points.RemoveRange(0, Step2Chart.Series(0).Points.Count)
            End If
            If Step2Chart.Series(1).Points.Count > 0 Then
                Step2Chart.Series(1).Points.RemoveRange(0, Step2Chart.Series(1).Points.Count)
            End If

            'AJG ADDITIONAL CONFIGURATION BECAUSE OF BEHAVIOUR CHANGES IN NEW LIBRARY VERSION
            Step2Chart.CrosshairEnabled = DevExpress.Utils.DefaultBoolean.False
            Step2Chart.RuntimeHitTesting = True

            If Not pCountsphMain Is Nothing AndAlso Not pCountsphRef Is Nothing AndAlso pCountsphMain.Count > 0 AndAlso pCountsphRef.Count > 0 Then
                ' Configure Ranges
                Dim myMax As Single = Math.Max(pCountsphMain.Max, pCountsphRef.Max)
                Dim myMin As Single = Math.Min(pCountsphMain.Min, pCountsphRef.Min)
                If myMax = 0 And myMin = 0 Then
                    Exit Sub
                End If
                If CLng(CType(Step2Chart.Diagram, SwiftPlotDiagram).AxisY.VisualRange.MinValue) >= myMax Then
                    CType(Step2Chart.Diagram, SwiftPlotDiagram).AxisY.WholeRange.MinValue = myMin
                    CType(Step2Chart.Diagram, SwiftPlotDiagram).AxisY.WholeRange.MaxValue = myMax
                    CType(Step2Chart.Diagram, SwiftPlotDiagram).AxisY.VisualRange.MinValue = myMin
                    CType(Step2Chart.Diagram, SwiftPlotDiagram).AxisY.VisualRange.MaxValue = myMax
                Else
                    CType(Step2Chart.Diagram, SwiftPlotDiagram).AxisY.WholeRange.MaxValue = myMax
                    CType(Step2Chart.Diagram, SwiftPlotDiagram).AxisY.WholeRange.MinValue = myMin
                    CType(Step2Chart.Diagram, SwiftPlotDiagram).AxisY.VisualRange.MaxValue = myMax
                    CType(Step2Chart.Diagram, SwiftPlotDiagram).AxisY.VisualRange.MinValue = myMin
                End If
                CType(Step2Chart.Diagram, SwiftPlotDiagram).AxisY.NumericScaleOptions.AutoGrid = True

                ' XBC 21/02/2012
                'CType(Step2Chart.Diagram, SwiftPlotDiagram).AxisX.Range.MaxValue = pCountsphMain.Count + 1
                'CType(Step2Chart.Diagram, SwiftPlotDiagram).AxisX.Range.MinValue = 0
                'CType(Step2Chart.Diagram, SwiftPlotDiagram).AxisX.GridSpacingAuto = True
                ' XBC 21/02/2012

                ' Populate new values
                For i As Integer = 0 To pCountsphMain.Count - 1
                    Step2Chart.Series(0).Points.Add(New SeriesPoint(i + 1, pCountsphMain(i)))
                    Step2Chart.Series(1).Points.Add(New SeriesPoint(i + 1, pCountsphRef(i)))
                Next
            End If

            'AJG IT'S MANDATORY TO DEFINE SIDEMARGINSVALUE = 0 FOR PREVENTING LEAVING SPACES IN THE AXES.
            CType(Step2Chart.Diagram, SwiftPlotDiagram).AxisX.VisualRange.SideMarginsValue = 0
            CType(Step2Chart.Diagram, SwiftPlotDiagram).AxisY.VisualRange.SideMarginsValue = 0

        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".PopulateStep2Chart ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PopulateStep2Chart ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub RefreshResultLists(ByVal pLedpPosition As Integer)
        Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate
        Try
            If Not Me.UnableHandlers Then

                If pLedpPosition < 0 Then
                    Exit Sub
                End If

                MyBase.DisplayMessage("")

                Select Case Me.SelectedPage
                    Case PHOTOMETRY_PAGES.STEP1
                        ' Nothing
                    Case PHOTOMETRY_PAGES.STEP2

                        Me.BsStep2AutoFillRadioButton.Visible = True
                        Me.BsStep2ManualFillRadioButton.Visible = True

                        'If Me.BsStep2AutoFillRadioButton.Checked Then
                        '    Me.BsStep2WellUpDown.Value = 1
                        '    Me.BsStep2WellUpDown.Enabled = False
                        'Else
                        '    Me.BsStep2WellUpDown.Enabled = True
                        'End If

                        Me.BsStep2DarkRadioButton.Enabled = True
                        If Me.BsStep2DarkRadioButton.Checked Then
                            Me.BsStep2WLCombo.SelectedIndex = 0
                            Me.BsStep2WLCombo.Enabled = False
                        Else
                            Me.BsStep2WLCombo.Enabled = True
                        End If

                        If Me.BsStep2RepeatabilityRadioButton.Checked Then
                            '
                            ' REPEATABILITY
                            '

                            ' XBC 21/02/2012
                            CType(Step2Chart.Diagram, SwiftPlotDiagram).AxisX.WholeRange.MaxValue = myScreenDelegate.MaxRepeatability + 1
                            CType(Step2Chart.Diagram, SwiftPlotDiagram).AxisX.WholeRange.MinValue = 0
                            CType(Step2Chart.Diagram, SwiftPlotDiagram).AxisX.VisualRange.MaxValue = myScreenDelegate.MaxRepeatability + 1
                            CType(Step2Chart.Diagram, SwiftPlotDiagram).AxisX.VisualRange.MinValue = 0
                            CType(Step2Chart.Diagram, SwiftPlotDiagram).AxisX.NumericScaleOptions.GridSpacing = 50
                            CType(Step2Chart.Diagram, SwiftPlotDiagram).AxisX.NumericScaleOptions.AutoGrid = True
                            ' XBC 21/02/2012

                            If Me.BsStep2DarkRadioButton.Checked Then
                                ' DARK
                                Me.BsStep2phMainGroupBox.Visible = True
                                Me.BsStep2phRefGroupBox.Visible = True
                                Me.BsStep2AbsorbanceGroupBox.Visible = False
                                Me.BsStep2AbsCVResult.Visible = False

                                Me.BsStep2SDevLabel.Visible = True
                                Me.BsStep2CVevLabel.Visible = False
                                Me.BsStep2MaxLabel.Visible = True
                                Me.BsStep2MinLabel.Visible = True
                                Me.BsStep2RangeLabel.Visible = True

                                PopulatephMainStep2(pLedpPosition)
                                PopulatephRefStep2(pLedpPosition)
                                PopulateStep2Chart(myScreenDelegate.MeasuresRepeatabilityphMainCountsDKByLed(pLedpPosition), _
                                                   myScreenDelegate.MeasuresRepeatabilityphRefCountsDKByLed(pLedpPosition))
                                Step2Chart.Legend.Visibility = DevExpress.Utils.DefaultBoolean.True

                                CType(Step2Chart.Diagram, SwiftPlotDiagram).AxisX.Title.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_NumReadings", currentLanguage)
                                CType(Step2Chart.Diagram, SwiftPlotDiagram).AxisY.Title.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_NumberCounts", currentLanguage)
                            ElseIf Me.BsStep2LightRadioButton.Checked Then
                                ' LIGHT
                                Me.BsStep2phMainGroupBox.Visible = False
                                Me.BsStep2phRefGroupBox.Visible = False

                                Me.BsStep2AbsorbanceGroupBox.Visible = True
                                Me.BsStep2AbsorbanceGroupBox.Size = New Size(100, 241)

                                Me.BsStep2AbsCVResult.Visible = True

                                Me.BsStep2SDevLabel.Visible = True
                                Me.BsStep2CVevLabel.Visible = True
                                Me.BsStep2MaxLabel.Visible = True
                                Me.BsStep2MinLabel.Visible = True
                                Me.BsStep2RangeLabel.Visible = True

                                PopulateABSStep2(pLedpPosition)
                                PopulateStep2Chart(myScreenDelegate.MeasuresRepeatabilityAbsorbances(Me.BsStep2WLCombo.SelectedIndex))
                                Step2Chart.Legend.Visibility = DevExpress.Utils.DefaultBoolean.False

                                CType(Step2Chart.Diagram, SwiftPlotDiagram).AxisX.Title.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_NumReadings", currentLanguage)
                                CType(Step2Chart.Diagram, SwiftPlotDiagram).AxisY.Title.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Absorbance_Full", currentLanguage) ' JB 01/10/2012 - Resource String unification
                            End If

                        ElseIf Me.BsStep2StabilityRadioButton.Checked Then
                            '
                            ' STABILITY
                            '

                            ' XBC 21/02/2012
                            CType(Step2Chart.Diagram, SwiftPlotDiagram).AxisX.WholeRange.MaxValue = myScreenDelegate.MaxStability + 1
                            CType(Step2Chart.Diagram, SwiftPlotDiagram).AxisX.WholeRange.MinValue = 0
                            CType(Step2Chart.Diagram, SwiftPlotDiagram).AxisX.VisualRange.MaxValue = myScreenDelegate.MaxStability + 1
                            CType(Step2Chart.Diagram, SwiftPlotDiagram).AxisX.VisualRange.MinValue = 0
                            CType(Step2Chart.Diagram, SwiftPlotDiagram).AxisX.NumericScaleOptions.GridSpacing = 50
                            CType(Step2Chart.Diagram, SwiftPlotDiagram).AxisX.NumericScaleOptions.AutoGrid = True
                            ' XBC 21/02/2012

                            If Me.BsStep2DarkRadioButton.Checked Then
                                ' DARK
                                Me.BsStep2phMainGroupBox.Visible = True
                                Me.BsStep2phRefGroupBox.Visible = True
                                Me.BsStep2AbsorbanceGroupBox.Visible = False
                                Me.BsStep2AbsCVResult.Visible = False

                                Me.BsStep2SDevLabel.Visible = True
                                Me.BsStep2CVevLabel.Visible = False
                                Me.BsStep2MaxLabel.Visible = True
                                Me.BsStep2MinLabel.Visible = True
                                Me.BsStep2RangeLabel.Visible = True

                                PopulatephMainStep2(pLedpPosition)
                                PopulatephRefStep2(pLedpPosition)
                                PopulateStep2Chart(myScreenDelegate.MeasuresStabilityphMainCountsDKByLed(pLedpPosition), _
                                                   myScreenDelegate.MeasuresStabilityphRefCountsDKByLed(pLedpPosition))
                                Step2Chart.Legend.Visibility = DevExpress.Utils.DefaultBoolean.True

                                CType(Step2Chart.Diagram, SwiftPlotDiagram).AxisX.Title.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_NumReadings", currentLanguage)
                                CType(Step2Chart.Diagram, SwiftPlotDiagram).AxisY.Title.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_NumberCounts", currentLanguage)
                            ElseIf Me.BsStep2LightRadioButton.Checked Then
                                ' LIGHT
                                Me.BsStep2phMainGroupBox.Visible = False
                                Me.BsStep2phRefGroupBox.Visible = False

                                Me.BsStep2AbsorbanceGroupBox.Visible = True
                                Me.BsStep2AbsorbanceGroupBox.Size = New Size(100, 241)

                                Me.BsStep2AbsCVResult.Visible = True

                                Me.BsStep2SDevLabel.Visible = True
                                Me.BsStep2CVevLabel.Visible = True
                                Me.BsStep2MaxLabel.Visible = True
                                Me.BsStep2MinLabel.Visible = True
                                Me.BsStep2RangeLabel.Visible = True

                                PopulateABSStep2(pLedpPosition)
                                PopulateStep2Chart(myScreenDelegate.MeasuresStabilityAbsorbances(Me.BsStep2WLCombo.SelectedIndex))
                                Step2Chart.Legend.Visibility = DevExpress.Utils.DefaultBoolean.False

                                CType(Step2Chart.Diagram, SwiftPlotDiagram).AxisX.Title.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_NumReadings", currentLanguage)
                                CType(Step2Chart.Diagram, SwiftPlotDiagram).AxisY.Title.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Absorbance_Full", currentLanguage) ' JB 01/10/2012 - Resource String unification
                            End If

                        ElseIf Me.BsStep2AbsorbanceRadioButton.Checked Then
                            '
                            ' ABSORBANCE MEASUREMENT
                            '

                            ' XBC 24/02/2012
                            CType(Step2ChartAbs.Diagram, XYDiagram).AxisX.WholeRange.MaxValue = myScreenDelegate.MaxWaveLengths
                            CType(Step2ChartAbs.Diagram, XYDiagram).AxisX.WholeRange.MinValue = 1
                            CType(Step2ChartAbs.Diagram, XYDiagram).AxisX.VisualRange.MaxValue = myScreenDelegate.MaxWaveLengths
                            CType(Step2ChartAbs.Diagram, XYDiagram).AxisX.VisualRange.MinValue = 1
                            CType(Step2ChartAbs.Diagram, XYDiagram).AxisX.NumericScaleOptions.GridSpacing = 1
                            CType(Step2ChartAbs.Diagram, XYDiagram).AxisX.NumericScaleOptions.AutoGrid = False
                            ' XBC 24/02/2012

                            Me.BsStep2phMainGroupBox.Visible = False
                            Me.BsStep2phRefGroupBox.Visible = False

                            Me.BsStep2AbsorbanceGroupBox.Visible = True
                            Me.BsStep2AbsorbanceGroupBox.Size = New Size(100, 50)

                            'Me.BsStep2WellUpDown.Enabled = True
                            'Me.BsStep2AutoFillRadioButton.Visible = False
                            'Me.BsStep2ManualFillRadioButton.Visible = False
                            Me.BsStep2DarkRadioButton.Enabled = False
                            Me.BsStep2LightRadioButton.Checked = True

                            Me.BsStep2SDevLabel.Visible = False
                            Me.BsStep2CVevLabel.Visible = False
                            Me.BsStep2MaxLabel.Visible = False
                            Me.BsStep2MinLabel.Visible = False
                            Me.BsStep2RangeLabel.Visible = False


                            ' Populate results of the readed Absorbances to screen
                            PopulateABSStep2(pLedpPosition)
                            PopulateStep2Chart(myScreenDelegate.GetAbsorbanceABSResult, True)
                            Step2Chart.Legend.Visibility = DevExpress.Utils.DefaultBoolean.False

                            ' XBC 24/02/2012
                            'CType(Step2Chart.Diagram, SwiftPlotDiagram).AxisX.Title.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_Wavelength", currentLanguage)
                            'CType(Step2Chart.Diagram, SwiftPlotDiagram).AxisY.Title.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Absorbance_Full", currentLanguage) 'JB 01/10/2012 - Resource String unification
                            CType(Step2ChartAbs.Diagram, XYDiagram).AxisX.Title.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_Wavelength", currentLanguage)
                            CType(Step2ChartAbs.Diagram, XYDiagram).AxisY.Title.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Absorbance_Full", currentLanguage) 'JB 01/10/2012 - Resource String unification
                            ' XBC 24/02/2012
                        End If

                        'Case PHOTOMETRY_PAGES.CHECKROTOR
                        '    Me.RunningTest = False
                        '    Me.BsFillModeCombo.Enabled = True
                        '    Me.BsStep3WLCombo.Enabled = True
                        '    Me.BsStep3TestButton.Enabled = True

                        '    ' Populate results of the readed Absorbances to screen
                        '    PopulateABSStep3()
                        '    PopulateStep3Chart(myScreenDelegate.MeasuresCheckRotorAbsorbances)
                End Select

            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".RefreshResultLists ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".RefreshResultLists ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Fill New IT's readed checkbox List with the values of the leds intensities
    ''' </summary>
    ''' <remarks>Created by XBC 30/03/2011</remarks>
    Private Sub PopulateCurrentLEDsIntensity()
        Try
            Me.BsDataGridViewLeds2.Rows.Clear()
            Me.BsDataGridViewLeds2.Rows.Add()
            'Me.BsDataGridViewLeds2.Rows(0).HeaderCell.Value = "λ"
            For i As Integer = 0 To myScreenDelegate.CurrentLEDsIntensities.Count - 1
                Me.BsDataGridViewLeds2.Rows(0).Cells(i).Value = myScreenDelegate.CurrentLEDsIntensities(i)
            Next

            Me.BsDataGridViewLeds2.Rows.Add()
            'Me.BsDataGridViewLeds.Rows(0).HeaderCell.Value = "LED Current"
            For i As Integer = 0 To Me.EditedValues.Length - 1
                Me.EditedValues(i).CurrentValue = CSng(ReadSpecificAdjustmentData(Me.EditedValues(i).LedPos).Value)
                Me.EditedValues(i).NewValue = Me.EditedValues(i).CurrentValue
                Me.BsDataGridViewLeds2.Rows(1).Cells(i).Value = Me.EditedValues(i).CurrentValue
            Next
            Me.BsDataGridViewLeds2.Rows(1).DefaultCellStyle.BackColor = Color.BlanchedAlmond

            Me.BsDataGridViewLeds2.Refresh()
            Me.BsDataGridViewLeds2.CurrentCell.Selected = False
        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".PopulateCurrentLEDSIntensity ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PopulateCurrentLEDSIntensity ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Fill References IT's List with the values of the leds intensities
    ''' </summary>
    ''' <remarks>Created by XBC 15/03/2011</remarks>
    Private Sub PopulateReferenceLEDSIntensity()
        Try
            Me.BsDataGridViewLedsChecks.Rows.Clear()
            Me.BsDataGridViewLedsChecks.Rows.Add()
            'Me.BsDataGridViewLeds.Rows(0).HeaderCell.Value = "Ref. LED Current"
            For i As Integer = 0 To Me.EditedValues.Length - 1
                Me.BsDataGridViewLedsChecks.Rows(0).Cells(i).Value = False
            Next
            Me.BsDataGridViewLedsChecks.Refresh()
            Me.BsDataGridViewLedsChecks.CurrentCell.Selected = False
        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".PopulateReferenceLEDSIntensity ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PopulateReferenceLEDSIntensity ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub DisableForItsEdition()
        Try
            '' Change text labels
            'Me.BsBaselineLabel.Text = "IT References Edition"   
            'Me.BsBLphMainGroupBox.Text = "New ITs"
            'Me.BsBLphRefGroupBox.Text = "Current ITs"
            '' Hide BL test controls
            Me.BSStep1BLChart.Visible = False
            Me.BsStep1BaselineLabel.Visible = False
            Me.BsStep1WLsGroupBox.Visible = False
            Me.BsStep1BLphMainGroupBox.Visible = False
            Me.BsStep1BLphRefGroupBox.Visible = False
            Me.BsStep1DCLabel.Visible = False
            Me.BsStep1DCphMainGroupBox.Visible = False
            Me.BsStep1DCphRefGroupBox.Visible = False
            Me.BsStep1DacsReferenceButton.Visible = False
            Me.BsDataGridViewLeds.Visible = False
            Me.BsStep1ResultsLabel.Visible = False
            'Me.BsStep1WellUpDown.Visible = False
            'Me.BsWellLabel.Visible = False
            'Me.BsBLTestButton.Visible = False
            ' Show IT's test controls
            Me.bsSelectAllITsCheckbox.Visible = True
            Me.bsSelectAllITsCheckbox.Checked = False
            Me.bsSelectAllITsCheckbox.Enabled = True
            Me.BsStep1ITSaveButton.Visible = True
            Me.BsStep1ITExitButton.Visible = True
            Me.BsStep1ITSaveButton.Enabled = True
            Me.BsStep1ITExitButton.Enabled = True
            Me.BsDataGridViewLeds2.Visible = True
            Me.BsDataGridViewLedsChecks.Visible = True
            Me.bsStep1RefDACSIntensityLabel.Visible = True

            'Me.BsDataGridViewLeds.AutoResizeRow(1)

            ' Disable Controls
            Me.BsStep1DCLabel.Enabled = False
            Me.BsStep1DCphMainGroupBox.Enabled = False
            Me.BsStep1DCphRefGroupBox.Enabled = False
            'Me.bsDACSIntensityLabel.Enabled = False
            Me.BsStep1Label.Enabled = False
            Me.BSStep1BLChart.Enabled = False
            Me.BsExitButton.Enabled = False
            Me.ManageTabPages = False
            Me.BsTestButton.Enabled = False
            Me.BsStep1UpDownWSButton.Enabled = False

            ' XBC 24/02/2012
            Me.BsStep1LEDsGroupBox1.Size = New System.Drawing.Size(727, 158)

        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".DisableForItsEdition ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".DisableForItsEdition ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub RestorePostItsEdition()
        Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate
        'Dim myPhotometryDataTO As PhotometryDataTO
        Dim myResultData As New GlobalDataTO
        Try
            'Me.BsBaselineLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_Baseline", currentLanguage)
            'BsBLphMainGroupBox.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_Photodiode1", currentLanguage)
            'BsBLphRefGroupBox.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_Photodiode2", currentLanguage)

            Me.BSStep1BLChart.Visible = True
            Me.BsStep1BaselineLabel.Visible = True
            Me.BsStep1WLsGroupBox.Visible = True
            Me.BsStep1BLphMainGroupBox.Visible = True
            Me.BsStep1BLphRefGroupBox.Visible = True
            Me.BsStep1DCLabel.Visible = True
            Me.BsStep1DCphMainGroupBox.Visible = True
            Me.BsStep1DCphRefGroupBox.Visible = True
            Me.BsDataGridViewLeds.Visible = True
            Me.BsStep1ResultsLabel.Visible = True
            'Me.BsStep1WellUpDown.Visible = True
            'Me.BsWellLabel.Visible = True
            Me.BsStep1DacsReferenceButton.Visible = True
            'Me.BsBLTestButton.Visible = True

            Me.bsSelectAllITsCheckbox.Visible = False
            Me.BsStep1ITSaveButton.Visible = False
            Me.BsStep1ITExitButton.Visible = False
            Me.BsDataGridViewLeds2.Visible = False
            Me.BsDataGridViewLedsChecks.Visible = False
            Me.bsStep1RefDACSIntensityLabel.Visible = False

            'Me.BsphMainBLListView.CheckBoxes = False
            'Me.BsphMainBLListView.View = View.Tile
            'Me.BsphRefBLListView.View = View.Tile
            'Me.BsWLsListView.View = View.Tile
            'myResultData = myAnalyzerManager.ReadPhotometryData()
            'If (Not myResultData.HasError And Not myResultData.SetDatos Is Nothing) Then
            '    myPhotometryDataTO = DirectCast(myResultData.SetDatos, PhotometryDataTO)

            '    PopulatephMainBL(myPhotometryDataTO.CountsMainBaseline)
            '    PopulatephRefBL(myPhotometryDataTO.CountsRefBaseline)
            'End If

            ' Disable Controls
            Me.BsStep1DCLabel.Enabled = True
            Me.BsStep1DCphMainGroupBox.Enabled = True
            Me.BsStep1DCphRefGroupBox.Enabled = True
            'Me.bsDACSIntensityLabel.Enabled = True
            Me.BsStep1Label.Enabled = True
            Me.BSStep1BLChart.Enabled = True
            Me.BsExitButton.Enabled = True
            Me.BsTestButton.Enabled = True
            Me.BsStep1UpDownWSButton.Enabled = True

            'If Me.BsphMainBLListView.Items.Count > 0 Then
            '    For i As Integer = 0 To Me.BsphMainBLListView.Items.Count - 1
            '        BsphMainBLListView.Items(i).BackColor = Drawing.Color.Gainsboro
            '        BsphRefBLListView.Items(i).BackColor = Drawing.Color.Gainsboro
            '        BsWLsListView.Items(i).BackColor = Drawing.Color.Gainsboro
            '    Next
            'End If

            ' XBC 24/02/2012
            Me.BsStep1LEDsGroupBox1.Size = New System.Drawing.Size(727, 70)

            PrepareLoadedMode()

        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".RestorePostItsEdition ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".RestorePostItsEdition ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub ExitPhotometryTest()
        Dim myGlobal As New GlobalDataTO
        Try
            Me.Close()
            ' Pending on future requirements

            'If myScreenDelegate.CurrentTest = ADJUSTMENT_GROUPS.IT_EDITION Then
            '    If Me.ExitingScreen Then
            '        Me.Close()
            '    End If
            '    Exit Sub
            'End If

            'myGlobal = MyBase.ExitTest
            'If myGlobal.HasError Then
            '    PrepareErrorMode()
            'Else
            '    PrepareArea()

            '    MyBase.DisplayMessage(Messages.SRV_TEST_EXIT_REQUESTED.ToString)

            '    If Not myGlobal.HasError Then
            '        If MyBase.SimulationMode Then
            '            ' simulating
            '            MyBase.DisplaySimulationMessage("Test Exiting...")
            '            Me.Cursor = Cursors.WaitCursor
            '            System.Threading.Thread.Sleep(SimulationProcessTime)
            '            MyBase.myServiceMDI.Focus()
            '            Me.Cursor = Cursors.Default
            '            MyBase.CurrentMode = ADJUSTMENT_MODES.TEST_EXITED
            '            PrepareArea()
            '        Else
            '            ' Manage FwScripts must to be sent to testing
            '            SendFwScript(Me.CurrentMode, myScreenDelegate.CurrentTest)
            '        End If
            '    End If

            'End If

        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".ExitPhotometryTest ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ExitPhotometryTest ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Assigns the specific screen's elements to the common screen layout structure
    ''' </summary>
    ''' <remarks>XBC 18/03/2011</remarks>
    Private Sub DefineScreenLayout(ByVal pPage As PHOTOMETRY_PAGES)
        Try
            With MyBase.myScreenLayout
                .MessagesPanel.Container = Me.BsMessagesPanel
                .MessagesPanel.Icon = Me.BsMessageImage
                .MessagesPanel.Label = Me.BsMessageLabel

                Select Case pPage
                    Case PHOTOMETRY_PAGES.STEP1
                        .AdjustmentPanel.Container = Me.BsStep1AdjustPanel
                        .InfoPanel.Container = Me.BsStep1InfoPanel
                        .InfoPanel.InfoXPS = BsInfoStep1XPSViewer


                    Case PHOTOMETRY_PAGES.STEP2
                        .AdjustmentPanel.Container = Me.BsStep2AdjustPanel
                        .InfoPanel.Container = Me.BsStep2InfoPanel
                        .InfoPanel.InfoXPS = BsInfoStep2XPSViewer


                    Case PHOTOMETRY_PAGES.STEP3
                        .AdjustmentPanel.Container = Me.BsStep3AdjustPanel
                        .InfoPanel.Container = Me.BsStep3InfoPanel
                        .InfoPanel.InfoXPS = BsInfoStep3XPSViewer


                    Case Else
                        .AdjustmentPanel.Container = Nothing
                        .InfoPanel.Container = Nothing
                        .InfoPanel.InfoXPS = Nothing


                End Select
            End With
        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".DefineScreenLayout ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".DefineScreenLayout ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub CreateBLResultsFileOutput(ByVal pPath As String)
        Try
            If File.Exists(pPath) Then File.Delete(pPath)
            Dim myStreamWriter As StreamWriter = File.CreateText(pPath)

            myStreamWriter.WriteLine("(Ph Main - Ph Ref - Current Led -  Integration Time)")
            For i As Integer = 0 To myScreenDelegate.BaseLineMainCounts.Count - 1
                myStreamWriter.WriteLine(myScreenDelegate.BaseLineMainCounts(i).ToString & " " & _
                                         myScreenDelegate.BaseLineRefCounts(i).ToString & " " & _
                                         myScreenDelegate.CurrentLEDsIntensities(i).ToString & " " & _
                                         myScreenDelegate.IntegrationTimes(i).ToString)
            Next

            ' XBC 02/03/2012
            myStreamWriter.WriteLine("")
            myStreamWriter.WriteLine("(Ph Main dark - Ph Ref dark)")
            myStreamWriter.WriteLine(myScreenDelegate.DarkphMainCount.ToString & " " & _
                                     myScreenDelegate.DarkphRefCount.ToString)
            ' XBC 02/03/2012

            myStreamWriter.Close()

        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".CreateBLResultsFileOutput ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".CreateBLResultsFileOutput ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub CreateRepeatabilityResultsFileOutput(ByVal pPath As String)
        Try
            If File.Exists(pPath) Then File.Delete(pPath)
            Dim myStreamWriter As StreamWriter = File.CreateText(pPath)

            myStreamWriter.WriteLine("")
            myStreamWriter.WriteLine("LIGHT")

            myStreamWriter.WriteLine("-----")
            For i As Integer = 0 To Me.BsStep2WLCombo.Items.Count - 1
                myStreamWriter.WriteLine("")
                myStreamWriter.WriteLine("LED Position : " & myScreenDelegate.PositionLEDs(i).ToString)

                ' XBC 29/02/2012
                myStreamWriter.WriteLine("")
                myStreamWriter.WriteLine("Mean : " & myScreenDelegate.GetAbsorbanceRepeatabilityResult(i).ToString("#,##0.0000"))
                myStreamWriter.WriteLine("Std. deviation : " & myScreenDelegate.GetAbsorbanceDeviationRepeatabilityResult(i).ToString("#,##0.0000"))
                myStreamWriter.WriteLine("Coef. variation : " & myScreenDelegate.GetCVAbsorbanceRepeatabilityResults(i))
                myStreamWriter.WriteLine("Max value : " & myScreenDelegate.GetMAXAbsorbanceRepeatabilityResult(i).ToString("#,##0.0000"))
                myStreamWriter.WriteLine("Min value : " & myScreenDelegate.GetMINAbsorbanceRepeatabilityResult(i).ToString("#,##0.0000"))
                myStreamWriter.WriteLine("Range : " & myScreenDelegate.GetRangeAbsorbanceRepeatabilityResult(i).ToString("#,##0.0000"))
                myStreamWriter.WriteLine("")
                ' XBC 29/02/2012

                Dim myResults As List(Of Single)
                Dim myResultsRef As List(Of Single)
                Dim myResultsAbs As List(Of Single)
                myResults = myScreenDelegate.MeasuresRepeatabilityphMainCountsByLed(i)
                myResultsRef = myScreenDelegate.MeasuresRepeatabilityphRefCountsByLed(i)
                myResultsAbs = myScreenDelegate.MeasuresRepeatabilityAbsorbances(i)
                myStreamWriter.WriteLine("Counts : (Ph Main - Ph Ref - Abs)")
                For j As Integer = 0 To myResults.Count - 1

                    ' XBC 02/03/2012
                    'myStreamWriter.WriteLine(myResults(j).ToString & " " & myResultsRef(j).ToString)
                    myStreamWriter.WriteLine(myResults(j).ToString & " " & _
                                             myResultsRef(j).ToString & " " & _
                                             myResultsAbs(j).ToString)
                    ' XBC 02/03/2012
                Next
            Next

            myStreamWriter.WriteLine("")
            myStreamWriter.WriteLine("DARK (Ph Main - Ph Ref)")
            myStreamWriter.WriteLine("-----------------------")

            ' XBC 29/02/2012
            myStreamWriter.WriteLine("")
            myStreamWriter.WriteLine("Mean : " & myScreenDelegate.GetPhMainCountsRepeatabilityResultDK(0).ToString("#,##0") & " " & _
                                     myScreenDelegate.GetPhRefCountsRepeatabilityResultDK(0).ToString("#,##0"))
            myStreamWriter.WriteLine("Std. deviation : " & myScreenDelegate.GetSTDDeviationPhMainRepeatabilityResultDK(0).ToString("#,##0") & " " & _
                                    myScreenDelegate.GetSTDDeviationPhRefRepeatabilityResultDK(0).ToString("#,##0"))
            myStreamWriter.WriteLine("Max value : " & myScreenDelegate.GetMAXPhMainRepeatabilityResultDK(0).ToString("#,##0") & " " & _
                                     myScreenDelegate.GetMAXPhRefRepeatabilityResultDK(0).ToString("#,##0"))
            myStreamWriter.WriteLine("Min value : " & myScreenDelegate.GetMINPhMainRepeatabilityResultDK(0).ToString("#,##0") & " " & _
                                     myScreenDelegate.GetMINPhRefRepeatabilityResultDK(0).ToString("#,##0"))
            myStreamWriter.WriteLine("Range : " & myScreenDelegate.GetRangePhMainRepeatabilityResultDK(0).ToString("#,##0") & " " & _
                                     myScreenDelegate.GetRangePhRefRepeatabilityResultDK(0).ToString("#,##0"))
            myStreamWriter.WriteLine("")
            ' XBC 29/02/2012

            For i As Integer = 0 To Me.BsStep2WLCombo.Items.Count - 1
                myStreamWriter.WriteLine("")
                myStreamWriter.WriteLine("LED Position : " & myScreenDelegate.PositionLEDs(i).ToString & " (Ph Main - Ph Ref)")

                Dim myResults As List(Of Single)
                Dim myResultsRef As List(Of Single)
                myResults = myScreenDelegate.MeasuresRepeatabilityphMainCountsDKByLed(i)
                myResultsRef = myScreenDelegate.MeasuresRepeatabilityphRefCountsDKByLed(i)
                For j As Integer = 0 To myResults.Count - 1
                    myStreamWriter.WriteLine(myResults(j).ToString & " " & myResultsRef(j).ToString)
                Next
            Next

            myStreamWriter.Close()

        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".CreateRepeatabilityResultsFileOutput ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".CreateRepeatabilityResultsFileOutput ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub CreateStabilityResultsFileOutput(ByVal pPath As String)
        Try
            If File.Exists(pPath) Then File.Delete(pPath)
            Dim myStreamWriter As StreamWriter = File.CreateText(pPath)

            myStreamWriter.WriteLine("")
            myStreamWriter.WriteLine("LIGHT")
            myStreamWriter.WriteLine("-----")
            For i As Integer = 0 To Me.BsStep2WLCombo.Items.Count - 1
                myStreamWriter.WriteLine("")
                myStreamWriter.WriteLine("LED Position : " & myScreenDelegate.PositionLEDs(i).ToString & " (Ph Main - Ph Ref)")

                ' XBC 29/02/2012
                myStreamWriter.WriteLine("")
                myStreamWriter.WriteLine("Mean : " & myScreenDelegate.GetAbsorbanceStabilityResult(i).ToString("#,##0.0000"))
                myStreamWriter.WriteLine("Std. deviation : " & myScreenDelegate.GetAbsorbanceDeviationStabilityResult(i).ToString("#,##0.0000"))
                myStreamWriter.WriteLine("Coef. variation : " & myScreenDelegate.GetCVAbsorbanceStabilityResults(i))
                myStreamWriter.WriteLine("Max value : " & myScreenDelegate.GetMAXAbsorbanceStabilityResult(i).ToString("#,##0.0000"))
                myStreamWriter.WriteLine("Min value : " & myScreenDelegate.GetMINAbsorbanceStabilityResult(i).ToString("#,##0.0000"))
                myStreamWriter.WriteLine("Range : " & myScreenDelegate.GetRangeAbsorbanceStabilityResult(i).ToString("#,##0.0000"))
                myStreamWriter.WriteLine("")
                ' XBC 29/02/2012

                Dim myResults As List(Of Single)
                Dim myResultsRef As List(Of Single)
                Dim myResultsAbs As List(Of Single)
                myResults = myScreenDelegate.MeasuresStabilityphMainCountsByLed(i)
                myResultsRef = myScreenDelegate.MeasuresStabilityphRefCountsByLed(i)
                myResultsAbs = myScreenDelegate.MeasuresStabilityAbsorbances(i)
                myStreamWriter.WriteLine("Counts : (Ph Main - Ph Ref - Abs)")
                For j As Integer = 0 To myResults.Count - 1

                    ' XBC 02/03/2012
                    'myStreamWriter.WriteLine(myResults(j).ToString & " " & myResultsRef(j).ToString)
                    myStreamWriter.WriteLine(myResults(j).ToString & " " & _
                                             myResultsRef(j).ToString & " " & _
                                             myResultsAbs(j).ToString)
                    ' XBC 02/03/2012    
                Next
            Next

            myStreamWriter.WriteLine("")
            myStreamWriter.WriteLine("DARK (Ph Main - Ph Ref)")
            myStreamWriter.WriteLine("-----------------------")

            ' XBC 29/02/2012
            myStreamWriter.WriteLine("")
            myStreamWriter.WriteLine("Mean : " & myScreenDelegate.GetPhMainCountsStabilityResultDK(0).ToString("#,##0") & " " & _
                                     myScreenDelegate.GetPhRefCountsStabilityResultDK(0).ToString("#,##0"))
            myStreamWriter.WriteLine("Std. deviation : " & myScreenDelegate.GetSTDDeviationPhMainStabilityResultDK(0).ToString("#,##0") & " " & _
                                    myScreenDelegate.GetSTDDeviationPhRefStabilityResultDK(0).ToString("#,##0"))
            myStreamWriter.WriteLine("Max value : " & myScreenDelegate.GetMAXPhMainStabilityResultDK(0).ToString("#,##0") & " " & _
                                     myScreenDelegate.GetMAXPhRefStabilityResultDK(0).ToString("#,##0"))
            myStreamWriter.WriteLine("Min value : " & myScreenDelegate.GetMINPhMainStabilityResultDK(0).ToString("#,##0") & " " & _
                                     myScreenDelegate.GetMINPhRefStabilityResultDK(0).ToString("#,##0"))
            myStreamWriter.WriteLine("Range : " & myScreenDelegate.GetRangePhMainStabilityResultDK(0).ToString("#,##0") & " " & _
                                     myScreenDelegate.GetRangePhRefStabilityResultDK(0).ToString("#,##0"))
            myStreamWriter.WriteLine("")
            ' XBC 29/02/2012

            For i As Integer = 0 To Me.BsStep2WLCombo.Items.Count - 1
                myStreamWriter.WriteLine("")
                myStreamWriter.WriteLine("LED Position : " & myScreenDelegate.PositionLEDs(i).ToString & " (Ph Main - Ph Ref)")

                Dim myResults As List(Of Single)
                Dim myResultsRef As List(Of Single)
                myResults = myScreenDelegate.MeasuresStabilityphMainCountsDKByLed(i)
                myResultsRef = myScreenDelegate.MeasuresStabilityphRefCountsDKByLed(i)
                For j As Integer = 0 To myResults.Count - 1
                    myStreamWriter.WriteLine(myResults(j).ToString & " " & myResultsRef(j).ToString)
                Next
            Next

            myStreamWriter.Close()

        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".CreateStabilityResultsFileOutput ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".CreateStabilityResultsFileOutput ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub ExitScreen()
        Try
            Me.Close()
            ' Pending on future requirements

            'If MyBase.CurrentMode = ADJUSTMENT_MODES.ERROR_MODE Then
            '    Me.Close()
            '    Exit Sub
            'End If

            'If Not Me.FirstIteration Then
            '    Me.ExitingScreen = True
            '    ExitPhotometryTest()
            'Else
            '    Me.Close()
            'End If

        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".ExitScreen ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ExitScreen ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Load Adjustments High Level Instruction to move Washing Station
    ''' </summary>
    ''' <remarks>
    ''' Created by XBC 20/02/2012
    ''' Modified by XB 13/10/2014 - Use NROTOR instead WSCTRL when Wash Station is down - BA-2004
    ''' </remarks>
    Private Sub SendWASH_STATION_CTRL()
        Dim myGlobal As New GlobalDataTO
        Try
            myGlobal = MyBase.Park
            If myGlobal.HasError Then
                PrepareErrorMode()
            Else
                PrepareArea()

                If Not myGlobal.HasError Then
                    If MyBase.SimulationMode Then
                        ' simulating
                        Me.Cursor = Cursors.WaitCursor
                        System.Threading.Thread.Sleep(SimulationProcessTime)
                        MyBase.myServiceMDI.Focus()
                        Me.Cursor = Cursors.Default
                        MyBase.CurrentMode = ADJUSTMENT_MODES.PARKED
                        myScreenDelegate.IsWashingStationUp = Not myScreenDelegate.IsWashingStationUp
                        PrepareArea()
                    Else
                        ' Manage instruction for Washing Station UP/DOWN
                        If myScreenDelegate.IsWashingStationUp Then
                            'myScreenDelegate.SendWASH_STATION_CTRL(Ax00WashStationControlModes.DOWN)
                            myScreenDelegate.SendNEW_ROTOR()
                        Else
                            myScreenDelegate.SendWASH_STATION_CTRL(Ax00WashStationControlModes.UP)
                        End If
                    End If
                End If

            End If

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".SendWASH_STATION_CTRL", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".SendWASH_STATION_CTRL", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

#Region "Generic Adjusments DS Methods"

    ''' <summary>
    ''' Update some value in the specific adjustments ds
    ''' </summary>
    ''' <param name="pCodew"></param>
    ''' <param name="pValue"></param>
    ''' <returns></returns>
    ''' <remarks>XBC 16/05/2011</remarks>
    Private Function UpdateSpecificAdjustmentsDS(ByVal pCodew As String, ByVal pValue As String) As GlobalDataTO
        Dim myGlobal As New GlobalDataTO
        Try
            For Each SR As SRVAdjustmentsDS.srv_tfmwAdjustmentsRow In MyClass.SelectedAdjustmentsDS.srv_tfmwAdjustments.Rows
                If SR.CodeFw.Trim = pCodew.Trim Then
                    SR.Value = pValue
                    Exit For
                End If
            Next
        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myGlobal.ErrorMessage = ex.Message
            CreateLogActivity(ex.Message, Me.Name & ".UpdateSpecificAdjustmentsDS ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".UpdateSpecificAdjustmentsDS", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
        MyClass.SelectedAdjustmentsDS.AcceptChanges()
        Return myGlobal
    End Function

    ''' <summary>
    ''' Update some value in the specific adjustments ds
    ''' </summary>
    ''' <param name="pCodew"></param>
    ''' <param name="pValue"></param>
    ''' <returns></returns>
    ''' <remarks>XBC 16/05/2011</remarks>
    Private Function UpdateTemporalSpecificAdjustmentsDS(ByVal pCodew As String, ByVal pValue As String) As GlobalDataTO
        Dim myGlobal As New GlobalDataTO
        Try
            For Each SR As SRVAdjustmentsDS.srv_tfmwAdjustmentsRow In Me.TemporalAdjustmentsDS.srv_tfmwAdjustments.Rows
                If SR.CodeFw.Trim = pCodew.Trim Then
                    SR.Value = pValue
                    Exit For
                End If
            Next
        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myGlobal.ErrorMessage = ex.Message
            CreateLogActivity(ex.Message, Me.Name & ".UpdateTemporalSpecificAdjustmentsDS ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".UpdateTemporalSpecificAdjustmentsDS", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
        Me.TemporalAdjustmentsDS.AcceptChanges()
        Return myGlobal
    End Function

    ''' <summary>
    ''' updates all a temporal dataset with changed current adjustments
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>XBC 16/05/2011</remarks>
    Private Function UpdateTemporalAdjustmentsDS() As GlobalDataTO
        Dim myGlobal As New GlobalDataTO
        Try
            For i As Integer = 0 To Me.EditedValues.Length - 1
                UpdateTemporalSpecificAdjustmentsDS(ReadSpecificAdjustmentData(Me.EditedValues(i).LedPos).CodeFw, Me.EditedValues(i).NewValue.ToString)
            Next

        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".UpdateTemporalAdjustmentsDS ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".UpdateTemporalAdjustmentsDS ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
        Return myGlobal
    End Function

    ''' <summary>
    ''' Gets the value corresponding to informed Group and Axis from global adjustments dataset
    ''' </summary>
    ''' <param name="pAxis"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' Created by XBC 16/05/2011
    ''' Modified by XB 04/02/2013 - Upper conversions redundants because the value is already in UpperCase must delete to avoid Regional Settings problems (Bugs tracking #1112)
    ''' </remarks>
    Private Function ReadGlobalAdjustmentData(ByVal pGroupID As String, ByVal pAxis As GlobalEnumerates.AXIS, Optional ByVal pNotForDisplaying As Boolean = False) As AdjustmentRowData
        Dim myGlobal As New GlobalDataTO
        Dim myAdjustmentRowData As New AdjustmentRowData("")
        Try
            Dim myAxis As String = pAxis.ToString
            If myAxis = "_NONE" Then myAxis = ""

            Dim myGroup As String = pGroupID
            If myGroup = "_NONE" Then myGroup = ""


            Dim myAdjustmentRows As New List(Of SRVAdjustmentsDS.srv_tfmwAdjustmentsRow)
            myAdjustmentRows = (From a As SRVAdjustmentsDS.srv_tfmwAdjustmentsRow _
                                In MyBase.myAllAdjustmentsDS.srv_tfmwAdjustments _
                                Where a.GroupID.Trim = myGroup.Trim _
                                And a.AxisID.Trim = myAxis.Trim _
                                Select a).ToList
            'Where a.GroupID.Trim.ToUpper = myGroup.Trim.ToUpper _
            'And a.AxisID.Trim.ToUpper = myAxis.Trim.ToUpper _

            If myAdjustmentRows.Count > 0 Then
                With myAdjustmentRowData
                    .AnalyzerID = myAdjustmentRows(0).AnalyzerID
                    .CodeFw = myAdjustmentRows(0).CodeFw
                    .Value = myAdjustmentRows(0).Value
                    .AxisID = myAdjustmentRows(0).AxisID
                    .GroupID = myAdjustmentRows(0).GroupID
                    .CanSave = myAdjustmentRows(0).CanSave
                    .CanMove = myAdjustmentRows(0).CanMove
                    .InFile = myAdjustmentRows(0).InFile
                End With
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".ReadGlobalAdjustmentValue ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ReadGlobalAdjustmentValue ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try

        'PDT if is not defined set to 0?
        If pNotForDisplaying Then
            If myAdjustmentRowData.Value = "" Then myAdjustmentRowData.Value = "0"
        End If

        Return myAdjustmentRowData
    End Function

    ''' <summary>
    ''' Gets the value corresponding to informed Adjustment identificator from the selected adjustments dataset
    ''' </summary>
    ''' <param name="pAdjustmentID"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' Created by XBC 16/05/2011
    ''' Modified by XB 04/02/2013 - Upper conversions redundants because the value is already in UpperCase must delete to avoid Regional Settings problems (Bugs tracking #1112)
    ''' </remarks>
    Private Function ReadSpecificAdjustmentData(ByVal pAdjustmentID As String) As AdjustmentRowData
        'Dim myGlobal As New GlobalDataTO
        Dim myAdjustmentRowData As New AdjustmentRowData("")
        Try
            Dim myAdjustmentRows As New List(Of SRVAdjustmentsDS.srv_tfmwAdjustmentsRow)
            myAdjustmentRows = (From a As SRVAdjustmentsDS.srv_tfmwAdjustmentsRow _
                                In MyClass.SelectedAdjustmentsDS.srv_tfmwAdjustments _
                                Where a.CodeFw.Trim = pAdjustmentID.Trim _
                                Select a).ToList
            'Where a.CodeFw.Trim.ToUpper = pAdjustmentID.Trim.ToUpper _

            If myAdjustmentRows.Count > 0 Then
                With myAdjustmentRowData
                    .AnalyzerID = myAdjustmentRows(0).AnalyzerID
                    .CodeFw = myAdjustmentRows(0).CodeFw
                    .Value = myAdjustmentRows(0).Value
                    .AxisID = myAdjustmentRows(0).AxisID
                    .GroupID = myAdjustmentRows(0).GroupID
                    .CanSave = myAdjustmentRows(0).CanSave
                    .CanMove = myAdjustmentRows(0).CanMove
                    .InFile = myAdjustmentRows(0).InFile
                End With
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".ReadSpecificAdjustmentData ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ReadSpecificAdjustmentData ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try

        If myAdjustmentRowData.Value = "" Then myAdjustmentRowData.Value = "0"

        Return myAdjustmentRowData
    End Function

    ''' <summary>
    ''' Gets the value corresponding to informed Axis from the selected adjustments dataset (overwritten)
    ''' </summary>
    ''' <param name="pAxis"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' Created by XB 13/10/2014 - new photometry adjustment maneuver (use REAGENTS_ABS_ROTOR (with parameter = current value of GFWR1) instead of REACTIONS_ROTOR_HOME_WELL1) - BA-1953
    ''' </remarks>
    Private Function ReadSpecificAdjustmentData(ByVal pAxis As GlobalEnumerates.AXIS) As AdjustmentRowData
        'Dim myGlobal As New GlobalDataTO
        Dim myAdjustmentRowData As New AdjustmentRowData("")
        Try
            Dim myAxis As String = pAxis.ToString
            If myAxis = "NONE" Then myAxis = ""

            Dim myAdjustmentRows As New List(Of SRVAdjustmentsDS.srv_tfmwAdjustmentsRow)
            myAdjustmentRows = (From a As SRVAdjustmentsDS.srv_tfmwAdjustmentsRow _
                                In Me.SelectedAdjustmentsDS.srv_tfmwAdjustments _
                                Where a.AxisID.Trim = myAxis.Trim _
                                Select a).ToList
            'Where a.AxisID.Trim.ToUpper = myAxis.Trim.ToUpper _

            If myAdjustmentRows.Count > 0 Then
                With myAdjustmentRowData
                    .AnalyzerID = myAdjustmentRows(0).AnalyzerID
                    .CodeFw = myAdjustmentRows(0).CodeFw
                    .Value = myAdjustmentRows(0).Value
                    .AxisID = myAdjustmentRows(0).AxisID
                    .GroupID = myAdjustmentRows(0).GroupID
                    .CanSave = myAdjustmentRows(0).CanSave
                    .CanMove = myAdjustmentRows(0).CanMove
                    .InFile = myAdjustmentRows(0).InFile
                End With
            End If

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".ReadSpecificAdjustmentData ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".ReadSpecificAdjustmentData ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try

        If myAdjustmentRowData.Value = "" Then myAdjustmentRowData.Value = "0"

        Return myAdjustmentRowData
    End Function

    ''' <summary>
    ''' Gets the Dataset that corresponds to the editing adjustments
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>
    ''' Created by XBC 16/05/2011
    ''' Modified by XB 13/10/2014 - new photometry adjustment maneuver (use REAGENTS_ABS_ROTOR (with parameter = current value of GFWR1) instead of REACTIONS_ROTOR_HOME_WELL1) - BA-1953
    ''' </remarks>
    Private Function LoadAdjustmentGroupData() As GlobalDataTO
        Dim resultData As New GlobalDataTO
        Dim CopyOfSelectedAdjustmentsDS As SRVAdjustmentsDS = MyClass.SelectedAdjustmentsDS
        Dim myAdjustmentsGroups As New List(Of String)
        Try
            If MyClass.SelectedAdjustmentsDS IsNot Nothing Then
                MyClass.SelectedAdjustmentsDS.Clear()
            End If
            myAdjustmentsGroups.Add(ADJUSTMENT_GROUPS.LEDS_CURRENT_REF.ToString)
            myAdjustmentsGroups.Add(ADJUSTMENT_GROUPS.PHOTOMETRY.ToString)     ' XB 13/10/2014 - BA-1953
            resultData = MyBase.myAdjustmentsDelegate.ReadAdjustmentsByGroupIDs(myAdjustmentsGroups)
            If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                MyClass.SelectedAdjustmentsDS = CType(resultData.SetDatos, SRVAdjustmentsDS)
            End If

        Catch ex As Exception
            MyClass.SelectedAdjustmentsDS = CopyOfSelectedAdjustmentsDS
            CreateLogActivity(ex.Message, Me.Name & ".LoadAdjustmentGroupData ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".LoadAdjustmentGroupData ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
        Return resultData
    End Function

#End Region

#End Region

#Region "Events"

    ''' <summary>
    ''' IDisposable functionality to force the releasing of the objects which wasn't totally closed by default
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>Created by XBC 12/09/2011</remarks>
    Private Sub IPhotometryAdjustments_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        Try

            If e.CloseReason = CloseReason.MdiFormClosing Then
                e.Cancel = True
            Else
                MyClass.myScreenDelegate.Dispose()
                MyClass.myScreenDelegate = Nothing

                'SGM 28/02/2012
                MyBase.ActivateMDIMenusButtons(Not MyBase.CloseRequestedByMDI)
                MyBase.myServiceMDI.IsFinalClosing = MyBase.CloseRequestedByMDI

            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".FormClosing ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".FormClosing ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub PhotometryAdjustments_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim myGlobal As New GlobalDataTO
        'Dim myGlobalbase As New GlobalBase
        Try
            'Get the current user level
            'Dim CurrentUserLevel As String = ""
            'CurrentUserLevel = GlobalBase.GetSessionInfo.UserLevel
            'Dim myUsersLevel As New UsersLevelDelegate
            'If CurrentUserLevel <> "" Then  'When user level exists then find his numerical level
            '    myGlobal = myUsersLevel.GetUserNumericLevel(Nothing, CurrentUserLevel)
            '    If Not myGlobal.HasError Then
            '        MyBase.CurrentUserNumericalLevel = CType(myGlobal.SetDatos, Integer)
            '    End If
            'End If

            MyBase.GetUserNumericalLevel()

            'Get the current Language from the current Application Session
            Me.currentLanguage = GlobalBase.GetSessionInfo.ApplicationLanguage.Trim.ToString

            'Load the multilanguage texts for all Screen Labels and get Icons for graphical Buttons
            GetScreenLabels()

            Me.SelectedPage = PHOTOMETRY_PAGES.STEP1

            MyClass.PrepareButtons()

            'SGM 12/11/2012 Information
            MyClass.SelectedInfoPanel = Me.BsStep1InfoPanel
            MyClass.SelectedAdjPanel = Me.BsStep1AdjustPanel
            'Me.BsInfoStep1XPSViewer.FitToPageHeight()

            MyBase.DisplayInformation(APPLICATION_PAGES.PHOTO_BASELINE, Me.BsInfoStep1XPSViewer)
            MyBase.DisplayInformation(APPLICATION_PAGES.PHOTO_METROLOGY, Me.BsInfoStep2XPSViewer)
            MyBase.DisplayInformation(APPLICATION_PAGES.PHOTO_ROTOR, Me.BsInfoStep3XPSViewer)


            'Screen delegate SGM 20/01/2012
            MyClass.myScreenDelegate = New PhotometryAdjustmentDelegate(MyBase.myServiceMDI.ActiveAnalyzer, myFwScriptDelegate)

            'Initialize homes SGM 20/09/2011
            MyClass.InitializeHomes()

            DisableAll()

            ' Check communications with Instrument
            If Not Ax00ServiceMainMDI.MDIAnalyzerManager.Connected Then
                PrepareErrorMode()
                MyBase.ActivateMDIMenusButtons(True)
            Else
                ' Reading Adjustments
                If Not MyClass.myServiceMDI.AdjustmentsReaded Then
                    ' Parent Reading Adjustments
                    MyBase.ReadAdjustments()
                    PrepareArea()

                    ' Manage FwScripts must to be sent at load screen
                    If MyBase.SimulationMode Then
                        ' simulating...
                        'MyBase.DisplaySimulationMessage("Request Adjustments from Instrument...")
                        Me.Cursor = Cursors.WaitCursor
                        System.Threading.Thread.Sleep(SimulationProcessTime)
                        MyBase.myServiceMDI.Focus()
                        Me.Cursor = Cursors.Default
                        MyClass.CurrentMode = ADJUSTMENT_MODES.ADJUSTMENTS_READED
                        PrepareArea()
                    Else
                        If Not myGlobal.HasError AndAlso Ax00ServiceMainMDI.MDIAnalyzerManager.Connected Then
                            myGlobal = myScreenDelegate.SendREAD_ADJUSTMENTS(GlobalEnumerates.Ax00Adjustsments.ALL)
                        End If
                    End If
                Else
                    MyClass.CurrentMode = ADJUSTMENT_MODES.ADJUSTMENTS_READED
                    PrepareArea()
                End If
            End If

            If myGlobal.HasError Then
                PrepareErrorMode()
                ShowMessage(Me.Name & ".Load", myGlobal.ErrorCode, myGlobal.ErrorMessage, Me)
            End If

            ResetBorderSRV()

            ' XBC 20/02/2012
            myScreenDelegate.IsWashingStationUp = False

        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".Load ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".Load ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>Created by SGM 12/11/2012</remarks>
    Private Sub IPhotometryAdjustments_Shown(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Shown
        Try
            Me.BsInfoStep1XPSViewer.Visible = True
            Me.BsInfoStep2XPSViewer.Visible = True
            Me.BsInfoStep3XPSViewer.Visible = True

            Me.BsInfoStep1XPSViewer.RefreshPage()
            Me.BsInfoStep2XPSViewer.RefreshPage()
            Me.BsInfoStep3XPSViewer.RefreshPage()
        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".Shown ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".Shown ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub BsTabPagesControl_Selected(ByVal sender As Object, ByVal e As System.Windows.Forms.TabControlEventArgs) Handles BsTabPagesControl.Selected
        Try

            If e.TabPage Is TabBaselineDarkness Then
                MyClass.SelectedInfoPanel = Me.BsStep1InfoPanel
                MyClass.SelectedAdjPanel = Me.BsStep1AdjustPanel
                Me.BsInfoStep1XPSViewer.FitToPageWidth()
                Me.BsInfoStep1XPSViewer.Refresh()

            ElseIf e.TabPage Is TabRepeatabilityStability Then
                MyClass.SelectedInfoPanel = Me.BsStep2InfoPanel
                MyClass.SelectedAdjPanel = Me.BsStep2AdjustPanel
                Me.BsInfoStep2XPSViewer.FitToPageWidth()
                Me.BsInfoStep2XPSViewer.Refresh()

            ElseIf e.TabPage Is TabVerificationRotor Then
                MyClass.SelectedInfoPanel = Me.BsStep3InfoPanel
                MyClass.SelectedAdjPanel = Me.BsStep3AdjustPanel
                Me.BsInfoStep3XPSViewer.FitToPageWidth()
                Me.BsInfoStep3XPSViewer.Refresh()

            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".BsTabPagesControl_Selected ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".BsTabPagesControl_Selected ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub BsTabPagesControl_Selecting(ByVal sender As Object, ByVal e As System.Windows.Forms.TabControlCancelEventArgs) Handles BsTabPagesControl.Selecting
        Try
            If Not Me.ManageTabPages Then
                e.Cancel = True
                Exit Sub
            End If

            If BsTabPagesControl.SelectedTab Is TabBaselineDarkness Then
                Me.SelectedPage = PHOTOMETRY_PAGES.STEP1
            ElseIf BsTabPagesControl.SelectedTab Is TabRepeatabilityStability Then
                Me.SelectedPage = PHOTOMETRY_PAGES.STEP2
                Me.RefreshResultLists(Me.BsStep2WLCombo.SelectedIndex)
                'ElseIf BsTabPagesControl.SelectedTab Is TabVerificationRotor Then
                '    Me.SelectedPage = PHOTOMETRY_PAGES.CHECKROTOR
                '    'Me.BsAdjustButton.Visible = True
            End If

            MyBase.DisplayMessage("")

            ' Pending on future requirements
            'If Not Me.FirstIteration Then
            '    ExitPhotometryTest()
            'End If
            'Me.FirstIteration = True

            PrepareLoadedMode()

        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".BsTabControl_Selecting ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".BsTabControl_Selecting ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ' 
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>
    ''' Modified by XBC 27-04-2011 - Place test buttons outside testing area into buttons below area
    '''              XB 13/10/2014 - new photometry adjustment maneuver (use REAGENTS_ABS_ROTOR (with parameter = current value of GFWR1) instead of REACTIONS_ROTOR_HOME_WELL1) - BA-1953 
    ''' </remarks>
    Private Sub BsTestButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsTestButton.Click
        Dim myGlobal As New GlobalDataTO
        Dim myAdjustmentGroup As ADJUSTMENT_GROUPS = Nothing
        Try
            Select Case Me.SelectedPage
                Case PHOTOMETRY_PAGES.STEP1
                    myGlobal = MyBase.Test
                    If myGlobal.HasError Then
                        PrepareErrorMode()
                    Else
                        PrepareArea()

                        ' Initializations
                        myScreenDelegate.HomesDone = False
                        myScreenDelegate.TestBaseLineDone = False
                        myScreenDelegate.WellToUse = CInt(Me.BsStep1WellUpDown.Value)
                        ' XB 13/10/2014 - BA-1953
                        myScreenDelegate.pValueAdjust = ReadSpecificAdjustmentData(GlobalEnumerates.AXIS.ROTOR).Value

                        If Me.BsStep1ManualFillRadioButton.Checked Then
                            myScreenDelegate.FillMode = FILL_MODE.MANUAL
                        ElseIf Me.BsStep1AutoFillRadioButton.Checked Then
                            myScreenDelegate.FillMode = FILL_MODE.AUTOMATIC

                            ' XBC 21/05/2012 - message no necessary
                            '' XBC 20/02/2012
                            'If myScreenDelegate.GetEmptyWell(myScreenDelegate.WellToUse - 1) Then
                            '    ' well not filled yet !

                            '    ' Inform message for filling the rotor
                            '    Dim dialogResultToReturn As DialogResult
                            '    dialogResultToReturn = ShowMessage(GetMessageText(Messages.SRV_PHOTOMETRY_TESTS.ToString), Messages.SRV_FILL_ROTOR.ToString)
                            '    Application.DoEvents()
                            '    If dialogResultToReturn = Windows.Forms.DialogResult.Cancel Then
                            '        myScreenDelegate.TestBaseLineDone = True
                            '        Me.PrepareLoadedMode()
                            '        Exit Sub
                            '    End If

                            'End If
                            ' XBC 21/05/2012
                        End If

                        MyBase.DisplayMessage(Messages.SRV_TEST_IN_PROCESS.ToString)

                        If Not myGlobal.HasError Then
                            If MyBase.SimulationMode Then
                                ' simulating
                                'MyBase.DisplaySimulationMessage("Baseline Testing...")
                                Me.Cursor = Cursors.WaitCursor
                                System.Threading.Thread.Sleep(SimulationProcessTime)
                                MyBase.myServiceMDI.Focus()
                                Me.Cursor = Cursors.Default
                                MyBase.CurrentMode = ADJUSTMENT_MODES.TESTED
                                myScreenDelegate.TestBaseLineDone = True
                                myScreenDelegate.CurrentTest = ADJUSTMENT_GROUPS.PHOTOMETRY
                                myScreenDelegate.SimulateBLTest()
                                PrepareArea()
                            Else
                                Me.Cursor = Cursors.WaitCursor
                                ' Manage FwScripts must to be sent to send previously a Fill rotor Instruction
                                SendFwScript(Me.CurrentMode, ADJUSTMENT_GROUPS.PHOTOMETRY)
                                If myScreenDelegate.HomesDone Then
                                    MyBase.CurrentMode = ADJUSTMENT_MODES.TESTED
                                    PrepareArea()
                                End If
                            End If
                        End If
                    End If

                Case PHOTOMETRY_PAGES.STEP2
                    myGlobal = MyBase.Test
                    If myGlobal.HasError Then
                        PrepareErrorMode()
                    Else
                        PrepareArea()

                        ' Initializations
                        'myScreenDelegate.HomesDone = False
                        'If IsNumeric(Me.BsStep2WLCombo.SelectedValue) Then ' No s'envia cap WL seleccionat ja que el test es farà per tots els WLs 
                        '    myScreenDelegate.WLSToUse = CInt(Me.BsStep2WLCombo.SelectedValue)
                        'Else
                        '    myScreenDelegate.WLSToUse = 0
                        'End If
                        myScreenDelegate.WellToUse = CInt(Me.BsStep2WellUpDown.Value)

                        ' Inform message for filling the rotor
                        If Me.BsStep2ManualFillRadioButton.Checked Then
                            myScreenDelegate.FillMode = FILL_MODE.MANUAL
                        ElseIf Me.BsStep2AutoFillRadioButton.Checked Then
                            myScreenDelegate.FillMode = FILL_MODE.AUTOMATIC

                            ' XBC 21/05/2012 - message no necessary
                            '' XBC 20/02/2012
                            'If myScreenDelegate.GetEmptyWell(myScreenDelegate.WellToUse - 1) Then
                            '    ' well not filled yet !

                            '    ' Inform message for filling the rotor
                            '    Dim dialogResultToReturn As DialogResult
                            '    dialogResultToReturn = ShowMessage(GetMessageText(Messages.SRV_PHOTOMETRY_TESTS.ToString), Messages.SRV_FILL_ROTOR.ToString)
                            '    Application.DoEvents()
                            '    If dialogResultToReturn = Windows.Forms.DialogResult.Cancel Then
                            '        myScreenDelegate.TestBaseLineDone = True
                            '        Me.PrepareLoadedMode()
                            '        Exit Sub
                            '    End If

                            'End If
                            ' XBC 21/05/2012
                        End If

                        If Me.BsStep2RepeatabilityRadioButton.Checked Then
                            '
                            ' REPEATABILITY
                            '
                            myAdjustmentGroup = ADJUSTMENT_GROUPS.REPEATABILITY
                            myScreenDelegate.TestRepeatabilityDone = False
                            myScreenDelegate.RepeatabilityReadings = 0
                            For i As Integer = 0 To myScreenDelegate.TestReadedCountsNum - 1
                                myScreenDelegate.MeasuresRepeatabilityAbsorbances(i) = New List(Of Single)
                                myScreenDelegate.MeasuresRepeatabilityphMainCountsByLed(i) = New List(Of Single)
                                myScreenDelegate.MeasuresRepeatabilityphRefCountsByLed(i) = New List(Of Single)
                                myScreenDelegate.MeasuresRepeatabilityphMainCountsDKByLed(i) = New List(Of Single)
                                myScreenDelegate.MeasuresRepeatabilityphRefCountsDKByLed(i) = New List(Of Single)
                            Next

                            Me.ProgressBar1.Maximum = CInt(myScreenDelegate.MaxRepeatability)
                            Me.ProgressBar1.Value = 0
                            Me.ProgressBar1.Visible = True
                        ElseIf Me.BsStep2StabilityRadioButton.Checked Then
                            '
                            ' STABILITY
                            '
                            myAdjustmentGroup = ADJUSTMENT_GROUPS.STABILITY
                            myScreenDelegate.TestStabilityDone = False
                            myScreenDelegate.StabilityReadings = 0
                            For i As Integer = 0 To myScreenDelegate.TestReadedCountsNum - 1
                                myScreenDelegate.MeasuresStabilityAbsorbances(i) = New List(Of Single)
                                myScreenDelegate.MeasuresStabilityphMainCountsByLed(i) = New List(Of Single)
                                myScreenDelegate.MeasuresStabilityphRefCountsByLed(i) = New List(Of Single)
                                myScreenDelegate.MeasuresStabilityphMainCountsDKByLed(i) = New List(Of Single)
                                myScreenDelegate.MeasuresStabilityphRefCountsDKByLed(i) = New List(Of Single)
                            Next

                            Me.ProgressBar1.Maximum = CInt(myScreenDelegate.MaxStability)
                            Me.ProgressBar1.Value = 0
                            Me.ProgressBar1.Visible = True
                        ElseIf Me.BsStep2AbsorbanceRadioButton.Checked Then
                            '
                            ' ABSORBANCE MEASUREMENT
                            '
                            myAdjustmentGroup = ADJUSTMENT_GROUPS.ABSORBANCE_MEASUREMENT
                            myScreenDelegate.TestABSDone = False
                        End If


                        MyBase.DisplayMessage(Messages.SRV_TEST_IN_PROCESS.ToString)

                        If Not myGlobal.HasError Then
                            If MyBase.SimulationMode Then
                                ' simulating
                                'MyBase.DisplaySimulationMessage("Testing...")
                                Me.Cursor = Cursors.WaitCursor

                                Me.ProgressBar1.Visible = True
                                Select Case myAdjustmentGroup
                                    Case ADJUSTMENT_GROUPS.REPEATABILITY
                                        Me.ProgressBar1.Maximum = CInt(myScreenDelegate.MaxRepeatability)
                                        For i As Integer = 1 To Me.ProgressBar1.Maximum
                                            Me.ProgressBar1.Value = i
                                            System.Threading.Thread.Sleep(10)
                                        Next
                                    Case ADJUSTMENT_GROUPS.STABILITY
                                        Me.ProgressBar1.Maximum = CInt(myScreenDelegate.MaxStability)
                                        For i As Integer = 1 To Me.ProgressBar1.Maximum
                                            Me.ProgressBar1.Value = i
                                            System.Threading.Thread.Sleep(10)
                                        Next
                                End Select

                                Me.ProgressBar1.Visible = False
                                MyBase.myServiceMDI.Focus()
                                Me.Cursor = Cursors.Default
                                'MyBase.DisplaySimulationMessage("")
                                MyBase.CurrentMode = ADJUSTMENT_MODES.TESTED
                                If Me.BsStep2RepeatabilityRadioButton.Checked Then
                                    myScreenDelegate.TestRepeatabilityDone = True
                                    myScreenDelegate.SimulateRepeatabilityTest()
                                ElseIf Me.BsStep2StabilityRadioButton.Checked Then
                                    myScreenDelegate.TestStabilityDone = True
                                    myScreenDelegate.SimulateStabilityTest()
                                ElseIf Me.BsStep2AbsorbanceRadioButton.Checked Then
                                    myScreenDelegate.TestABSDone = True
                                    myScreenDelegate.SimulateABSTest()
                                End If
                                myScreenDelegate.CurrentTest = myAdjustmentGroup
                                PrepareArea()
                            Else
                                If Not myGlobal.HasError AndAlso Ax00ServiceMainMDI.MDIAnalyzerManager.Connected Then
                                    Me.Cursor = Cursors.WaitCursor
                                    myGlobal = myScreenDelegate.SendREAD_COUNTS(myAdjustmentGroup)
                                End If
                            End If
                        End If
                    End If

            End Select

            If myGlobal.HasError Then
                PrepareErrorMode()
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".BsTestButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".BsTestButton_Click ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub BsDacsReferenceButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsStep1DacsReferenceButton.Click
        'Dim myGlobal As New GlobalDataTO
        Try
            LoadAdjustmentGroupData()

            DisableForItsEdition()

            PopulateCurrentLEDsIntensity()
            PopulateReferenceLEDSIntensity()

        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".BsDacsReferenceButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".BsDacsReferenceButton_Click ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub BsITExitButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsStep1ITExitButton.Click
        Try
            MyBase.DisplayMessage(Messages.SRV_ADJUSTMENTS_CANCELLED.ToString)
            RestorePostItsEdition()
        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".BsITExitButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".BsITExitButton_Click ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub BsITSaveButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsStep1ITSaveButton.Click
        Dim myGlobal As New GlobalDataTO
        Dim anyChecked As Boolean
        Try
            If Not Me.BsDataGridViewLedsChecks Is Nothing AndAlso Me.BsDataGridViewLedsChecks.Rows.Count > 0 Then
                For Each myCurrentCell As DataGridViewCell In Me.BsDataGridViewLedsChecks.Rows(0).Cells
                    If CBool(myCurrentCell.Value) Then
                        anyChecked = True
                        Exit For
                    End If
                Next
            End If

            If anyChecked Then
                myGlobal = MyBase.Save()
                If myGlobal.HasError Then
                    RestorePostItsEdition()
                    PrepareErrorMode()
                Else
                    PrepareArea()

                    ' Initializations
                    myScreenDelegate.SaveLedsIntensitiesDone = False

                    For i As Integer = 0 To Me.EditedValues.Length - 1
                        Me.EditedValues(i).CurrentValue = Me.EditedValues(i).NewValue
                        UpdateSpecificAdjustmentsDS(Me.EditedValues(i).LedPos, Me.EditedValues(i).CurrentValue.ToString)
                    Next

                    ' Takes a copy of the changed values of the dataset of Adjustments
                    myGlobal = myAdjustmentsDelegate.Clone(MyClass.SelectedAdjustmentsDS)
                    If Not myGlobal.SetDatos Is Nothing AndAlso Not myGlobal.HasError Then
                        Me.TemporalAdjustmentsDS = CType(myGlobal.SetDatos, SRVAdjustmentsDS)
                        Me.TempToSendAdjustmentsDelegate = New FwAdjustmentsDelegate(Me.TemporalAdjustmentsDS)
                        ' Update dataset of the temporal dataset of Adjustments to sent to Fw
                        myGlobal = UpdateTemporalAdjustmentsDS()
                    Else
                        PrepareErrorMode()
                        Exit Sub
                    End If

                    If myGlobal.HasError Then
                        PrepareErrorMode()
                    Else
                        MyBase.DisplayMessage(Messages.SRV_SAVE_ADJUSTMENTS.ToString)

                        ' Convert dataset to String for sending to Fw
                        myGlobal = Me.TempToSendAdjustmentsDelegate.ConvertDSToString()

                        If Not myGlobal.SetDatos Is Nothing AndAlso Not myGlobal.HasError Then
                            Dim pAdjuststr As String = CType(myGlobal.SetDatos, String)
                            myScreenDelegate.pValueAdjust = pAdjuststr

                            If MyBase.SimulationMode Then
                                ' simulating
                                'MyBase.DisplaySimulationMessage("Saving Values to Instrument...")
                                Me.Cursor = Cursors.WaitCursor
                                System.Threading.Thread.Sleep(SimulationProcessTime)
                                MyBase.myServiceMDI.Focus()
                                Me.Cursor = Cursors.Default
                                myScreenDelegate.SaveLedsIntensitiesDone = True
                                MyBase.CurrentMode = ADJUSTMENT_MODES.SAVED
                                PrepareArea()
                            Else
                                If Not myGlobal.HasError AndAlso Ax00ServiceMainMDI.MDIAnalyzerManager.Connected Then
                                    Me.Cursor = Cursors.WaitCursor
                                    myGlobal = myScreenDelegate.SendLOAD_ADJUSTMENTS(ADJUSTMENT_GROUPS.IT_EDITION)
                                End If
                            End If
                        Else
                            PrepareErrorMode()
                        End If

                    End If
                End If
            Else
                RestorePostItsEdition()
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".BsITSaveButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".BsITSaveButton_Click ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub BsCloseButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsExitButton.Click
        Try
            Me.ExitScreen()

        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".BsExitButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".BsExitButton_Click ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' When the  ESC key is pressed, the screen is closed 
    ''' </summary>
    ''' <remarks>
    ''' Created by XBC 07/06/2011
    ''' </remarks>
    Private Sub ConfigUsers_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyDown
        Try
            If (e.KeyCode = Keys.Escape) Then
                'If (Me.BsExitButton.Enabled) Then
                '    Me.ExitScreen()
                'End If

                'RH 04/07/2011 Escape key should do exactly the same operations as bsExitButton_Click()
                BsExitButton.PerformClick()
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".KeyDown ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".KeyDown", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub BsStep2WLCombo_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsStep2WLCombo.SelectedIndexChanged
        Try
            Me.RefreshResultLists(Me.BsStep2WLCombo.SelectedIndex)
        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".BsStep2WLCombo.SelectedIndexChanged ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".BsStep2WLCombo.SelectedIndexChanged ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    'Private Sub BsStep1ManualFillRadioButton_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsStep1ManualFillRadioButton.CheckedChanged
    '    Try
    '        If Me.BsStep1ManualFillRadioButton.Checked Then
    '            Me.BsStep1WellUpDown.Enabled = True
    '        End If
    '    Catch ex As Exception
    '        CreateLogActivity(ex.Message, Me.Name & ".bsStep1ManualFillRadioButton.CheckedChanged ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
    '        ShowMessage(Me.Name & ".bsStep1ManualFillRadioButton.CheckedChanged ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
    '    End Try
    'End Sub

    'Private Sub BsStep1AutoFillRadioButton_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsStep1AutoFillRadioButton.CheckedChanged
    '    Try
    '        If Me.BsStep1AutoFillRadioButton.Checked Then
    '            Me.BsStep1WellUpDown.Value = 1
    '            Me.BsStep1WellUpDown.Enabled = False
    '        End If
    '    Catch ex As Exception
    '        CreateLogActivity(ex.Message, Me.Name & ".bsStep1AutoFillRadioButton.CheckedChanged ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
    '        ShowMessage(Me.Name & ".bsStep1AutoFillRadioButton.CheckedChanged ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
    '    End Try
    'End Sub

    'Private Sub bsStep2AutoFillRadioButton_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsStep2AutoFillRadioButton.CheckedChanged
    '    Try
    '        If Me.BsStep2AutoFillRadioButton.Checked Then
    '            Me.BsStep2WellUpDown.Value = 1
    '            Me.BsStep2WellUpDown.Enabled = False
    '        End If
    '    Catch ex As Exception
    '        CreateLogActivity(ex.Message, Me.Name & ".bsStep2AutoFillRadioButton.CheckedChanged ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
    '        ShowMessage(Me.Name & ".bsStep2AutoFillRadioButton.CheckedChanged ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
    '    End Try
    'End Sub

    'Private Sub bsStep2ManualFillRadioButton_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsStep2ManualFillRadioButton.CheckedChanged
    '    Try
    '        If Me.BsStep2ManualFillRadioButton.Checked Then
    '            Me.BsStep2WellUpDown.Enabled = True
    '        End If
    '    Catch ex As Exception
    '        CreateLogActivity(ex.Message, Me.Name & ".bsStep2ManualFillRadioButton.CheckedChanged ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
    '        ShowMessage(Me.Name & ".bsStep2ManualFillRadioButton.CheckedChanged ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
    '    End Try
    'End Sub

    ' XBC 27-04-2011 - Place test buttons outside testing area into buttons below area
    'Private Sub BsStep2TestButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsStep2TestButtonTODELETE.Click
    '    Dim myGlobal As New GlobalDataTO
    '    Dim myAdjustmentGroup As ADJUSTMENT_GROUPS = Nothing
    '    Try
    '        myGlobal = MyBase.Test
    '        If myGlobal.HasError Then
    '            PrepareErrorMode()
    '        Else
    '            PrepareArea()

    '            ' Initializations
    '            myScreenDelegate.FillRotorDone = False
    '            'If IsNumeric(Me.BsStep2WLCombo.SelectedValue) Then ' No s'envia cap WL seleccionat ja que el test es farà per tots els WLs 
    '            '    myScreenDelegate.WLSToUse = CInt(Me.BsStep2WLCombo.SelectedValue)
    '            'Else
    '            '    myScreenDelegate.WLSToUse = 0
    '            'End If
    '            myScreenDelegate.WellToUse = CInt(Me.BsStep2WellUpDown.Value)

    '            ' Inform message for filling the rotor
    '            If Me.BsStep2ManualFillRadioButton.Checked Then
    '                myScreenDelegate.FillMode = FILL_MODE.MANUAL
    '            ElseIf Me.BsStep2AutoFillRadioButton.Checked Then
    '                myScreenDelegate.FillMode = FILL_MODE.AUTOMATIC

    '                ' Inform message for filling the rotor
    '                Dim dialogResultToReturn As DialogResult
    '                dialogResultToReturn = ShowMessage(GetMessageText(Messages.SRV_PHOTOMETRY_TESTS.ToString), Messages.SRV_FILL_ROTOR.ToString)
    '                Application.DoEvents()
    '            End If

    '            If Me.BsStep2RepeatabilityRadioButton.Checked Then
    '                '
    '                ' REPEATABILITY
    '                '
    '                myAdjustmentGroup = ADJUSTMENT_GROUPS.REPEATABILITY
    '                myScreenDelegate.TestRepeatabilityDone = False
    '                myScreenDelegate.RepeatabilityReadings = 0
    '                For i As Integer = 0 To myScreenDelegate.TestReadedCountsNum - 1
    '                    myScreenDelegate.MeasuresRepeatabilityAbsorbances(i) = New List(Of Single)
    '                    myScreenDelegate.MeasuresRepeatabilityphMainCountsByLed(i) = New List(Of Single)
    '                    myScreenDelegate.MeasuresRepeatabilityphRefCountsByLed(i) = New List(Of Single)
    '                    myScreenDelegate.MeasuresRepeatabilityphMainCountsDKByLed(i) = New List(Of Single)
    '                    myScreenDelegate.MeasuresRepeatabilityphRefCountsDKByLed(i) = New List(Of Single)
    '                Next

    '                Me.ProgressBar1.Maximum = CInt(myScreenDelegate.MaxRepeatability)
    '                Me.ProgressBar1.Value = 0
    '                Me.ProgressBar1.Visible = True
    '            ElseIf Me.BsStep2StabilityRadioButton.Checked Then
    '                '
    '                ' STABILITY
    '                '
    '                myAdjustmentGroup = ADJUSTMENT_GROUPS.STABILITY
    '                myScreenDelegate.TestStabilityDone = False
    '                myScreenDelegate.StabilityReadings = 0
    '                For i As Integer = 0 To myScreenDelegate.TestReadedCountsNum - 1
    '                    myScreenDelegate.MeasuresStabilityAbsorbances(i) = New List(Of Single)
    '                    myScreenDelegate.MeasuresStabilityphMainCountsByLed(i) = New List(Of Single)
    '                    myScreenDelegate.MeasuresStabilityphRefCountsByLed(i) = New List(Of Single)
    '                    myScreenDelegate.MeasuresStabilityphMainCountsDKByLed(i) = New List(Of Single)
    '                    myScreenDelegate.MeasuresStabilityphRefCountsDKByLed(i) = New List(Of Single)
    '                Next

    '                Me.ProgressBar1.Maximum = CInt(myScreenDelegate.MaxStability)
    '                Me.ProgressBar1.Value = 0
    '                Me.ProgressBar1.Visible = True
    '            ElseIf Me.BsStep2AbsorbanceRadioButton.Checked Then
    '                '
    '                ' ABSORBANCE MEASUREMENT
    '                '
    '                myAdjustmentGroup = ADJUSTMENT_GROUPS.ABSORBANCE_MEASUREMENT
    '                myScreenDelegate.TestABSDone = False
    '            End If


    '            MyBase.DisplayMessage(Messages.SRV_TEST_IN_PROCESS.ToString)

    '            If Not myGlobal.HasError Then
    '                If Mybase.SimulationMode Then
    '                    ' simulating
    '                    'MessageBox.Show(Me.BsTestTypeCombo.SelectedValue.ToString & " Testing ...")
    '                    Ax00ServiceMainMDI.ErrorStatusLabel.Text = " Testing ..."
    '                    MyBase.CurrentMode = ADJUSTMENT_MODES.TESTED
    '                    If Me.BsStep2RepeatabilityRadioButton.Checked Then
    '                        myScreenDelegate.TestRepeatabilityDone = True
    '                        myScreenDelegate.SimulateRepeatabilityTest()
    '                    ElseIf Me.BsStep2StabilityRadioButton.Checked Then
    '                        myScreenDelegate.TestStabilityDone = True
    '                        myScreenDelegate.SimulateStabilityTest()
    '                    ElseIf Me.BsStep2AbsorbanceRadioButton.Checked Then
    '                        myScreenDelegate.TestABSDone = True
    '                        myScreenDelegate.SimulateABSTest()
    '                    End If
    '                    myScreenDelegate.CurrentTest = myAdjustmentGroup
    '                    PrepareArea()
    '                Else
    '                    ' Manage FwScripts must to be sent to testing
    '                    SendFwScript(Me.CurrentMode, myAdjustmentGroup)
    '                End If
    '            End If
    '        End If

    '    Catch ex As Exception
    '        CreateLogActivity(ex.Message, Me.Name & ".BsStep2TestButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
    '        ShowMessage(Me.Name & ".BsStep2TestButton_Click ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
    '        PrepareErrorMode()
    '    End Try
    'End Sub

    Private Sub BsStep2RepeatabilityRadioButton_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsStep2RepeatabilityRadioButton.CheckedChanged
        Try
            Me.RefreshResultLists(Me.BsStep2WLCombo.SelectedIndex)
        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".BsStep2RepeatabilityRadioButton_CheckedChanged ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".BsStep2RepeatabilityRadioButton_CheckedChanged ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
            PrepareErrorMode()
        End Try
    End Sub

    Private Sub BsStep2StabilityRadioButton_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsStep2StabilityRadioButton.CheckedChanged
        Try
            Me.RefreshResultLists(Me.BsStep2WLCombo.SelectedIndex)
        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".BsStep2StabilityRadioButton_CheckedChanged ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".BsStep2StabilityRadioButton_CheckedChanged ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
            PrepareErrorMode()
        End Try
    End Sub

    Private Sub BsStep2AbsorbanceRadioButton_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsStep2AbsorbanceRadioButton.CheckedChanged
        Try
            Me.RefreshResultLists(Me.BsStep2WLCombo.SelectedIndex)
        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".BsStep2AbsorbanceRadioButton_CheckedChanged ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".BsStep2AbsorbanceRadioButton_CheckedChanged ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
            PrepareErrorMode()
        End Try
    End Sub

    Private Sub BsStep2DarkRadioButton_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsStep2DarkRadioButton.CheckedChanged
        Try
            If Me.BsStep2DarkRadioButton.Checked Then
                Me.BsStep2WLCombo.SelectedIndex = 0
                Me.BsStep2WLCombo.Enabled = False
            End If
            Me.RefreshResultLists(Me.BsStep2WLCombo.SelectedIndex)
        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".BsStep2DarkRadioButton.CheckedChanged ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".BsStep2DarkRadioButton.CheckedChanged ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub BsStep2LightRadioButton_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsStep2LightRadioButton.CheckedChanged
        Try
            If Me.BsStep2LightRadioButton.Checked Then
                Me.BsStep2WLCombo.Enabled = True
            End If
            Me.RefreshResultLists(Me.BsStep2WLCombo.SelectedIndex)
        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".BsStep2LightRadioButton.CheckedChanged ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".BsStep2LightRadioButton.CheckedChanged ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub bsSelectAllITsCheckbox_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles bsSelectAllITsCheckbox.CheckedChanged
        Try
            Dim myValue As Boolean
            If Not Me.BsDataGridViewLedsChecks Is Nothing AndAlso Me.BsDataGridViewLedsChecks.Rows.Count > 0 Then
                If Me.bsSelectAllITsCheckbox.Checked Then
                    For Each myCurrentCell As DataGridViewCell In Me.BsDataGridViewLedsChecks.Rows(0).Cells
                        myCurrentCell.Value = True
                        myValue = True
                    Next
                Else
                    For Each myCurrentCell As DataGridViewCell In Me.BsDataGridViewLedsChecks.Rows(0).Cells
                        myCurrentCell.Value = False
                        myValue = False
                    Next
                End If
            End If

            If Not Me.EditedValues Is Nothing And Me.BsDataGridViewLeds2.Rows.Count > 1 Then
                If myValue Then
                    For i As Integer = 0 To Me.EditedValues.Length - 1
                        Me.EditedValues(i).NewValue = CSng(Me.BsDataGridViewLeds2.Rows(0).Cells(i).Value)    ' new value !!!
                    Next
                Else
                    For i As Integer = 0 To Me.EditedValues.Length - 1
                        Me.EditedValues(i).NewValue = CSng(Me.BsDataGridViewLeds2.Rows(1).Cells(i).Value)    ' saved value !!!
                    Next
                End If
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".bsSelectAllITsCheckbox.CheckedChanged ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsSelectAllITsCheckbox.CheckedChanged ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub BSStep1BLChart_CustomDrawAxisLabel(ByVal sender As Object, ByVal e As CustomDrawAxisLabelEventArgs) Handles BSStep1BLChart.CustomDrawAxisLabel
        Try
            Dim axis As AxisBase = e.Item.Axis
            If TypeOf axis Is AxisX Then
                Dim axisValue As Double = CDbl(e.Item.AxisValue)
                If Not Me.BsWLsListView.Items Is Nothing AndAlso Me.BsWLsListView.Items.Count > 0 Then
                    If axisValue >= 1 Then
                        e.Item.Text = Me.BsWLsListView.Items(CInt(axisValue - 1)).Text
                    End If
                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".BSStep1BLChart.CustomDrawAxisLabel ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
        End Try
    End Sub

    Private Sub Step2Chart_CustomDrawAxisLabel(ByVal sender As Object, ByVal e As CustomDrawAxisLabelEventArgs) Handles Step2Chart.CustomDrawAxisLabel
        Try
            Dim axis As AxisBase = e.Item.Axis
            If Me.BsStep2AbsorbanceRadioButton.Checked Then
                If TypeOf axis Is AxisX Or TypeOf axis Is SwiftPlotDiagramAxisX Then
                    Dim axisValue As Double = CDbl(e.Item.AxisValue)
                    If axisValue > 0 Then
                        If Not Me.BsWLsListView.Items Is Nothing _
                           AndAlso Me.BsWLsListView.Items.Count > 0 Then
                            e.Item.Text = Me.BsWLsListView.Items(CInt(axisValue - 1)).Text
                        End If
                    End If
                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".Step2Chart.CustomDrawAxisLabel ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
        End Try
    End Sub

    Private Sub Step2ChartAbs_CustomDrawAxisLabel(ByVal sender As Object, ByVal e As CustomDrawAxisLabelEventArgs) Handles Step2ChartAbs.CustomDrawAxisLabel
        Try
            Dim axis As AxisBase = e.Item.Axis
            If Me.BsStep2AbsorbanceRadioButton.Checked Then
                If TypeOf axis Is AxisX Then
                    Dim axisValue As Double = CDbl(e.Item.AxisValue)
                    If axisValue > 0 Then
                        If Not Me.BsWLsListView.Items Is Nothing _
                           AndAlso Me.BsWLsListView.Items.Count > 0 Then
                            e.Item.Text = Me.BsWLsListView.Items(CInt(axisValue - 1)).Text
                        End If
                    End If
                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".Step2ChartAbs.CustomDrawAxisLabel ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
        End Try
    End Sub

    Private Sub CurrentOperationTimer_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CurrentOperationTimer.Tick
        Try
            If Me.ProgressBar1.Value + 1 = Me.ProgressBar1.Maximum Then
                CurrentOperationTimer.Enabled = False
                Me.ProgressBar1.Value = 0
                Me.ProgressBar1.Visible = False
                Me.ProgressBar1.Refresh()
                Exit Sub
            End If
            Me.ProgressBar1.Value += 1
            Me.ProgressBar1.Refresh()
        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".CurrentOperationTimer.Tick ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".CurrentOperationTimer.Tick ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub BsDataGridViewLedsChecks_CellContentClick(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles BsDataGridViewLedsChecks.CellContentClick
        Try
            Dim myCurrentCell As DataGridViewCheckBoxCell = CType(Me.BsDataGridViewLedsChecks.Rows(e.RowIndex).Cells(e.ColumnIndex), DataGridViewCheckBoxCell)
            For i As Integer = 0 To Me.EditedValues.Length - 1
                If Me.EditedValues(i).WaveLength = Me.BsDataGridViewLeds2.Columns(e.ColumnIndex).Name.ToString Then
                    If CBool(myCurrentCell.EditingCellFormattedValue) Then
                        Me.EditedValues(i).NewValue = CSng(Me.BsDataGridViewLeds2.Rows(0).Cells(e.ColumnIndex).Value)    ' new value !!!
                    Else
                        Me.EditedValues(i).NewValue = CSng(Me.BsDataGridViewLeds2.Rows(1).Cells(e.ColumnIndex).Value)   ' saved value !!!
                    End If
                End If
            Next
        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".BsDataGridViewLedsChecks.CellContentClick ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".BsDataGridViewLedsChecks.CellContentClick ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try

    End Sub

    Private Sub BsWLsListView_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles BsWLsListView.SelectedIndexChanged
        BsWLsListView.SelectedItems.Clear()
    End Sub

    Private Sub BsphMainBLListView_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles BsphMainBLListView.SelectedIndexChanged
        BsphMainBLListView.SelectedItems.Clear()
    End Sub

    Private Sub BsphRefBLListView_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles BsphRefBLListView.SelectedIndexChanged
        BsphRefBLListView.SelectedItems.Clear()
    End Sub

    ' proves per afegir tooltips als avisos predefinits (no es veu massa bé...)
    'Private Sub BsphMainWarningsListView_MouseLeave(ByVal sender As Object, ByVal e As System.EventArgs) Handles BsphMainWarningsListView.MouseLeave
    '    Try
    '        Dim a As New ToolTip
    '        a.SetToolTip(Me.BsphMainWarningsListView, "")
    '    Catch ex As Exception
    '        CreateLogActivity(ex.Message, Me.Name & ".BsphMainWarningsListView.MouseLeave ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
    '        ShowMessage(Me.Name & ".BsphMainWarningsListView.MouseLeave ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
    '    End Try
    'End Sub

    'Private Sub BsphMainWarningsListView_MouseMove(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles BsphMainWarningsListView.MouseMove
    '    Try
    '        Dim thisItem As ListViewItem = Me.BsphMainWarningsListView.GetItemAt(e.X, e.Y)

    '        Dim a As New ToolTip

    '        If Not thisItem Is Nothing Then
    '            If thisItem.StateImageIndex = 0 Then
    '                a.SetToolTip(Me.BsphMainWarningsListView, Me.WarningLeds)
    '            End If
    '        Else
    '            a.SetToolTip(Me.BsphMainWarningsListView, "")
    '        End If
    '    Catch ex As Exception
    '        CreateLogActivity(ex.Message, Me.Name & ".BsphMainWarningsListView.MouseMove ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
    '        ShowMessage(Me.Name & ".BsphMainWarningsListView.MouseMove ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
    '    End Try
    'End Sub

    ' per activar aquest caldria posar el datagrid a Enable
    'Sub dataGridView1_CellFormatting(ByVal sender As Object, ByVal e As DataGridViewCellFormattingEventArgs) Handles BsDataGridViewLeds.CellFormatting
    '    Try
    '        With Me.BsDataGridViewLeds.Rows(e.RowIndex).Cells(e.ColumnIndex)
    '            If .ErrorText = "warning" Then
    '                .ToolTipText = Me.WarningLeds
    '            End If
    '        End With

    '    Catch ex As Exception
    '        CreateLogActivity(ex.Message, Me.Name & ".BsDataGridViewLeds.CellFormatting ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
    '        ShowMessage(Me.Name & ".BsDataGridViewLeds.CellFormatting ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
    '    End Try
    'End Sub

    Private Sub BsUpDownWSButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsStep1UpDownWSButton.Click, BsStep2UpDownWSButton.Click
        Try
            Me.SendWASH_STATION_CTRL()

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".BsUpDownWSButton1_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".BsUpDownWSButton1_Click ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    'Private Sub BsStep1AutoFillRadioButton_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsStep1AutoFillRadioButton.CheckedChanged
    '    ActivateUpDownButtonVisibility()
    'End Sub

    'Private Sub BsStep1ManualFillRadioButton_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsStep1ManualFillRadioButton.CheckedChanged
    '    ActivateUpDownButtonVisibility()
    'End Sub

#End Region

#Region "COMENTED"
    ' Maybe is used in the future !

    'Private Sub BsphMainBLListView_ItemChecked(ByVal sender As Object, ByVal e As System.Windows.Forms.ItemCheckedEventArgs) Handles BsphMainBLListView.ItemChecked
    '    Try
    '        If Me.BsphMainBLListView.Items.Count > 0 Then
    '            For i As Integer = 0 To Me.BsphMainBLListView.Items.Count - 1
    '                If Me.BsphMainBLListView.Items(i).Checked = True Then
    '                    BsphMainBLListView.Items(i).BackColor = Drawing.Color.LemonChiffon
    '                    BsphRefBLListView.Items(i).BackColor = Drawing.Color.LemonChiffon
    '                    BsWLsListView.Items(i).BackColor = Drawing.Color.LemonChiffon
    '                Else
    '                    BsphMainBLListView.Items(i).BackColor = Drawing.Color.Gainsboro
    '                    BsphRefBLListView.Items(i).BackColor = Drawing.Color.Gainsboro
    '                    BsWLsListView.Items(i).BackColor = Drawing.Color.Gainsboro
    '                End If
    '            Next
    '        End If
    '    Catch ex As Exception
    '        CreateLogActivity(ex.Message, Me.Name & ".BsphMainBLListView.ItemChecked ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
    '        ShowMessage(Me.Name & ".BsphMainBLListView.ItemChecked ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
    '    End Try
    'End Sub
    'Private Sub BsStep3TestButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsStep3TestButton.Click
    '    Dim myGlobal As New GlobalDataTO
    '    Try
    '        ' BLACK is not allowed in Verification Rotor Test
    '        If UCase(Me.BsStep3WLCombo.SelectedValue.ToString) = "BLACK" Then
    '            MyBase.DisplayMessage(Messages.SRV_BLACKNOALLOWED.ToString)
    '            Exit Sub
    '        End If

    '        myGlobal = MyBase.Test
    '        If myGlobal.HasError Then
    '            PrepareErrorMode()
    '        Else
    '            PrepareArea()

    '            ' Initializations
    '            myScreenDelegate.TestCheckRotorDone = False
    '            myScreenDelegate.WellToUse = 0
    '            If IsNumeric(Me.BsStep3WLCombo.SelectedValue) Then
    '                myScreenDelegate.WLSToUse = CInt(Me.BsStep3WLCombo.SelectedValue)
    '            Else
    '                myScreenDelegate.WLSToUse = 0
    '            End If
    '            myScreenDelegate.MeasuresCheckRotorCounts = New List(Of Single)

    '            ' Inform message for filling the rotor
    '            Dim dialogResultToReturn As DialogResult
    '            dialogResultToReturn = ShowMessage(GetMessageText(Messages.SRV_PHOTOMETRY_TESTS.ToString), Messages.SRV_FILL_ROTOR.ToString)    

    '            Me.ProgressBar1.Maximum = CInt(myScreenDelegate.MaxWells)
    '            Me.ProgressBar1.Value = 0
    '            Me.ProgressBar1.Visible = True

    '            MyBase.DisplayMessage(Messages.SRV_TEST_IN_PROCESS.ToString)

    '            If Not myGlobal.HasError Then
    '                If Mybase.SimulationMode Then
    '                    ' simulating
    '                    'MessageBox.Show("Verification Rotor Testing ...")
    '                    Ax00ServiceMainMDI.ErrorStatusLabel.Text = "Verification Rotor Testing ..."
    '                    MyBase.CurrentMode = ADJUSTMENT_MODES.TESTED
    '                    myScreenDelegate.TestCheckRotorDone = True
    '                    myScreenDelegate.CurrentTest = ADJUSTMENT_GROUPS.CHECK_ROTOR
    '                    myScreenDelegate.SimulateCheckRotorTest()
    '                    PrepareArea()
    '                Else
    '                    ' Manage FwScripts must to be sent to testing
    '                    SendFwScript(Me.CurrentMode, ADJUSTMENT_GROUPS.CHECK_ROTOR)
    '                End If
    '            End If
    '        End If

    '    Catch ex As Exception
    '        CreateLogActivity(ex.Message, Me.Name & ".BsStep3TestButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
    '        ShowMessage(Me.Name & ".BsStep3TestButton_Click ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
    '        PrepareErrorMode()
    '    End Try
    'End Sub
    'Private Sub TestModeCombo_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)
    '    Try
    '        Me.RefreshResultLists()

    '        If Not Me.FirstIteration Then
    '            ExitPhotometryTest()
    '        End If
    '        Me.FirstIteration = True

    '    Catch ex As Exception
    '        CreateLogActivity(ex.Message, Me.Name & ".BsTestTypeCombo.SelectedIndexChanged ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
    '        ShowMessage(Me.Name & ".BsTestTypeCombo.SelectedIndexChanged ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
    '    End Try
    'End Sub


    'Private Sub BsSaveButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsSaveButton.Click
    '    Dim myGlobal As New GlobalDataTO
    '    Try
    '        ' Save BLDC Test
    '        'Dim myGlobalbase As New GlobalBase
    '        Dim myPathBLFile As String = Application.StartupPath & GlobalBase.PhotometryTestsFile
    '        myGlobal = myScreenDelegate.SaveBLDCFile(myPathBLFile)
    '        If myGlobal.HasError Then
    '            PrepareErrorMode()
    '            Exit Sub
    '        End If
    '        Me.BsSaveButton.Enabled = False
    '        Me.BsCancelButton.Enabled = False

    '    Catch ex As Exception
    '        CreateLogActivity(ex.Message, Me.Name & ".BsSaveButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
    '        ShowMessage(Me.Name & ".BsSaveButton_Click ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
    '    End Try
    'End Sub

    'Private Sub BsCancelButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsCancelButton.Click
    '    Try
    '        PrepareLoadedMode()
    '        Me.BsCancelButton.Enabled = False
    '    Catch ex As Exception
    '        CreateLogActivity(ex.Message, Me.Name & ".BsCancelButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
    '        ShowMessage(Me.Name & ".BsCancelButton_Click ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
    '    End Try
    'End Sub
    '''' <summary>
    '''' Fill controls with the values of the counts Main Photodiode light Verification Rotor test
    '''' </summary>
    '''' <remarks>Created by XBC 10/03/2011</remarks>
    'Private Sub PopulateABSStep3()
    '    Try
    '        Me.BsStep3AbsMeanResult.Text = myScreenDelegate.GetAbsorbanceCheckRotorResult.ToString() ' ("##,####0.00")
    '        Me.BsStep3AbsSDResult.Text = myScreenDelegate.GetAbsorbanceDeviationCheckRotorResult.ToString() ' ("##,####0.00")
    '        Me.BsStep3AbsCVResult.Text = myScreenDelegate.GetCVAbsorbanceCheckRotorResults.ToString()
    '        Me.BsStep3AbsMaxResult.Text = myScreenDelegate.GetMaxValueCheckRotorResult.ToString() ' ("##,####0.00")
    '        Me.BsStep3AbsMinResult.Text = myScreenDelegate.GetMinValueCheckRotorResult.ToString() ' ("##,####0.00")
    '        Me.BsStep3AbsRangeResult.Text = myScreenDelegate.GetRangeValueCheckRotorResult.ToString() ' ("##,####0.00")
    '    Catch ex As Exception
    '        CreateLogActivity(ex.Message, Me.Name & ".PopulateABSStep3 ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
    '        ShowMessage(Me.Name & ".PopulateABSStep3 ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
    '    End Try
    'End Sub

    '''' <summary>
    '''' Paint the chart with the values of the counts Main Photodiode light Verification Rotor test
    '''' </summary>
    '''' <param name="pCounts"></param>
    '''' <remarks>Created by XBC 11/03/2011</remarks>
    'Private Sub PopulateStep3Chart(ByVal pCounts As List(Of Single))
    '    Try
    '        ' Clear chart
    '        If Step3Chart.Series(0).Points.Count > 0 Then
    '            Step3Chart.Series(0).Points.RemoveRange(0, Step3Chart.Series(0).Points.Count)
    '        End If
    '        If Not pCounts Is Nothing Then
    '            ' Configure Ranges
    '            If CLng(CType(Step3Chart.Diagram, SwiftPlotDiagram).AxisY.Range.MinValue) > pCounts.Max Then
    '                CType(Step3Chart.Diagram, SwiftPlotDiagram).AxisY.Range.MinValue = pCounts.Min
    '                CType(Step3Chart.Diagram, SwiftPlotDiagram).AxisY.Range.MaxValue = pCounts.Max
    '            Else
    '                CType(Step3Chart.Diagram, SwiftPlotDiagram).AxisY.Range.MaxValue = pCounts.Max
    '                If pCounts.Max = pCounts.Min Then
    '                    CType(Step3Chart.Diagram, SwiftPlotDiagram).AxisY.Range.MinValue = pCounts.Min - 1 ' 
    '                Else
    '                    CType(Step3Chart.Diagram, SwiftPlotDiagram).AxisY.Range.MinValue = pCounts.Min
    '                End If
    '            End If
    '            CType(Step3Chart.Diagram, SwiftPlotDiagram).AxisY.GridSpacingAuto = True

    '            CType(Step3Chart.Diagram, SwiftPlotDiagram).AxisX.Range.MaxValue = pCounts.Count + 1
    '            CType(Step3Chart.Diagram, SwiftPlotDiagram).AxisX.Range.MinValue = 0
    '            CType(Step3Chart.Diagram, SwiftPlotDiagram).AxisX.GridSpacingAuto = True

    '            ' Populate new values
    '            For i As Integer = 0 To pCounts.Count - 1
    '                Step3Chart.Series(0).Points.Add(New SeriesPoint(i + 1, pCounts(i)))
    '            Next
    '        End If
    '    Catch ex As Exception
    '        CreateLogActivity(ex.Message, Me.Name & ".PopulateStep3Chart ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
    '        ShowMessage(Me.Name & ".PopulateStep3Chart ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
    '    End Try
    'End Sub
#End Region

#Region "TO DELETE"
    'Private Sub PhotometryAdjustments_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
    '    Try
    '        ' Hide Example ChartControl for design time
    '        Me.ChartControl1.Visible = False

    '        ' Create a chart.
    '        Dim Chart As New ChartControl()
    '        Dim Series1 As New Series("phMain", ViewType.Line)
    '        Dim Series2 As New Series("phRef", ViewType.Line)
    '        Dim ChartTitle1 As ChartTitle = New ChartTitle

    '        ' Create an empty Bar series and add it to the chart.
    '        Chart.Series.Add(Series1)
    '        Chart.Series.Add(Series2)

    '        ' Specify data members to bind the series.
    '        '
    '        ' Series1
    '        '
    '        ' Generate a data table and bind the series to it.
    '        Series1.DataSource = CreateChartData(8)
    '        Series1.ArgumentScaleType = ScaleType.Numerical
    '        Series1.ArgumentDataMember = "Wavelength"
    '        Series1.ValueScaleType = ScaleType.Numerical
    '        Series1.ValueDataMembers.AddRange(New String() {"Value"})
    '        Series1.View.Color = Color.DarkBlue
    '        '
    '        ' Series2
    '        '
    '        ' Generate a data table and bind the series to it.
    '        Series2.DataSource = CreateChartData(8)
    '        Series2.ArgumentScaleType = ScaleType.Numerical
    '        Series2.ArgumentDataMember = "Wavelength"
    '        Series2.ValueScaleType = ScaleType.Numerical
    '        Series2.ValueDataMembers.AddRange(New String() {"Value"})
    '        Series2.View.Color = Color.LimeGreen

    '        ' Set some properties to get a nice-looking chart.
    '        CType(Chart.Diagram, XYDiagram).AxisX.Visible = False
    '        CType(Chart.Diagram, XYDiagram).AxisY.Range.MaxValue = 980000
    '        CType(Chart.Diagram, XYDiagram).AxisY.Range.MinValue = 900000
    '        CType(Chart.Diagram, XYDiagram).AxisY.Title.Text = "Number of Counts"
    '        CType(Chart.Diagram, XYDiagram).AxisY.Title.Visible = True
    '        ChartTitle1.Text = "Baseline Test"
    '        ChartTitle1.Alignment = System.Drawing.StringAlignment.Center
    '        ChartTitle1.TextColor = System.Drawing.Color.Gray
    '        Chart.Titles.AddRange(New ChartTitle() {ChartTitle1})
    '        Chart.Legend.Visible = True

    '        ' Dock the chart into its parent and add it to the current form.
    '        'Chart.Dock = DockStyle.Fill
    '        Me.BsOpticAdjustPanel.Controls.Add(Chart)
    '        Chart.Size = New System.Drawing.Size(572, 353)
    '        Chart.Location = New System.Drawing.Point(3, 23)
    '    Catch ex As Exception
    '        Debug.Print("error : " & ex.Message)
    '    End Try
    'End Sub

    'Private Function CreateChartData(ByVal rowCount As Integer) As DataTable
    '    ' Create an empty table.
    '    Dim Table As New DataTable("Table1")
    '    Try
    '        ' Add two columns to the table.
    '        Table.Columns.Add("Wavelength", GetType(Int32))
    '        Table.Columns.Add("Value", GetType(Int32))

    '        ' Add data rows to the table.
    '        Dim Rnd As New Random()
    '        Dim Row As DataRow = Nothing
    '        Dim i As Integer
    '        For i = 0 To rowCount - 1
    '            Row = Table.NewRow()
    '            Row("Wavelength") = i
    '            Row("Value") = Rnd.Next(900000, 980000)
    '            Table.Rows.Add(Row)
    '        Next i
    '    Catch ex As Exception
    '        Debug.Print("error : " & ex.Message)
    '    End Try
    '    Return Table
    'End Function
#End Region

#Region "XPS Info Events"

    Private Sub BsXPSViewer_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles BsInfoStep1XPSViewer.Load, _
                                                                                            BsInfoStep2XPSViewer.Load, _
                                                                                            BsInfoStep3XPSViewer.Load
        Try
            Dim myBsXPSViewer As BsXPSViewer = CType(sender, BsXPSViewer)
            If myBsXPSViewer IsNot Nothing Then
                If myBsXPSViewer.IsScrollable Then
                    myBsXPSViewer.FitToPageWidth()
                Else
                    myBsXPSViewer.FitToPageHeight()
                    myBsXPSViewer.FitToPageWidth()
                End If
            End If
        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".BsXPSViewer_Load ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".BsXPSViewer_Load ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

#End Region


End Class
