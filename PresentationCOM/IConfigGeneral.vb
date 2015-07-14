Option Strict On
Option Explicit On

Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Global.GlobalEnumerates
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.BL
Imports System.Drawing 'SG 03/12/10
Imports System.Windows.Forms 'SG 03/12/10

Imports Biosystems.Ax00.CommunicationsSwFw  ' XBC 06/09/2011

Imports Biosystems.Ax00.FwScriptsManagement 'SGM 28/10/2011
Imports Biosystems.Ax00.App

Public Class UiConfigGeneral

#Region "Attributes"
    Private AnalyzerIDAttribute As String = ""
    'Private ParentLocationAttribute As Point
    'Private ParentSizeAttribute As Size
    Private SimulationModeAttr As Boolean = False   ' XBC 06/09/2011
#End Region

#Region "Properties"
    Public WriteOnly Property ActiveAnalyzer() As String
        Set(ByVal value As String)
            AnalyzerIDAttribute = value
        End Set
    End Property

    ' XBC 06/09/2011
    Protected Friend ReadOnly Property SimulationMode() As Boolean
        Get
            If GlobalConstants.REAL_DEVELOPMENT_MODE = 1 Then
                SimulationModeAttr = True ' Simulation mode
            ElseIf GlobalConstants.REAL_DEVELOPMENT_MODE = 2 Then
                SimulationModeAttr = False ' Developer mode
            Else
                SimulationModeAttr = False ' Real mode
            End If

            Return SimulationModeAttr
        End Get
    End Property

    ' '' </summary>
    '''' <remarks></remarks>
    'Public WriteOnly Property ParentLocation() As Point
    '    Set(ByVal value As Point)
    '        ParentLocationAttribute = value
    '    End Set
    'End Property

    ' '' </summary>
    '''' <remarks></remarks>
    'Public WriteOnly Property ParentSize() As Size
    '    Set(ByVal value As Size)
    '        ParentSizeAttribute = value
    '    End Set
    'End Property
#End Region

#Region "Declarations"
    Private newAnalyzer As Boolean = False    'DL 21/07/2011

    Private ChangesMade As Boolean
    Private EditionMode As Boolean
    Private LanguageID As String

    Private myNewLocation As Point

    ' XBC 07/09/2011
    Private testResult As Boolean
    Private testExecuted As Boolean
    'Private mdiAnalyzerCopy As AnalyzerManager 'DL 09/09/2011

    'TR 28/10/2011 
    Private StopRinging As Boolean = False

    'SGM 28/10/2011
    Private IsAdjustmentsSaving As Boolean = False
    Private SelectedAdjustmentsDS As New SRVAdjustmentsDS
    Private myFwAdjustmentsDelegate As FwAdjustmentsDelegate
    Private WithEvents myFwScriptDelegate As SendFwScriptsDelegate
    Private AdjustChangesMade As Boolean
    Private RecommendationsReport() As HISTORY_RECOMMENDATIONS
    Private ReportCountTimeout As Integer

    'XBC 03/11/2011
    ''event that manages the response of the Analyzer after sending a Script List
    'Public Event ReceivedLastFwScriptEvent(ByVal pResponse As RESPONSE_TYPES, ByVal pData As Object)
    Private TempToSendAdjustmentsDelegate As FwAdjustmentsDelegate
    'XBC 03/11/2011

    Private entryValueForExternalTankWaterCheck As Boolean 'AG 23/11/2011 - remember the entry value for check Water Supply (External Tank)

    Private IsService As Boolean = GlobalBase.IsServiceAssembly ' My.Application.Info.AssemblyName.ToUpper.Contains("SERVICE") 'SGM 23/02/2012

    Private CurrentManualComPort As String = "COM1" 'SGM 24/02/2012

    Private AlarmsDetected As Boolean = False   ' XBC 22/10/2012

    Private IsServiceAdjustmentsLoaded As Boolean = False 'SGM 21/11/2012

#End Region

#Region "Constructor" 'SG 03/12/10
    'Public Sub New(ByRef myMDI As Form)
    Public Sub New()
        'RH 12/04/2012 myMDI not needed.
        'MyBase.New()
        'SetParentMDI(myMDI)
        InitializeComponent()
    End Sub

    'SGM 21/11/2012 - special constructor for service
    Public Sub New(ByVal pIsServiceAdjustmentsLoaded As Boolean)

        ' This call is required by the Windows Form Designer.
        InitializeComponent()
        ' Add any initialization after the InitializeComponent() call.
        MyClass.IsServiceAdjustmentsLoaded = pIsServiceAdjustmentsLoaded

        If AnalyzerController.Instance.IsBA200 Then
            bsSamplesRotorCvrCheckbox.Visible = False
        End If
    End Sub

#End Region

#Region "Public Events"
    Public Event StopCurrentOperationFinished(ByVal pAlarmType As ManagementAlarmTypes) 'SGM 19/10/2012
    Public Event FormIsClosed(ByVal sender As Object) 'SGM 08/11/2012
#End Region

#Region "Public Methods"

    ''' <summary>
    ''' This method is called when the USB cable is unplugged while the configuration screen is active
    ''' </summary>
    ''' <param name="pRefreshEventType"></param>
    ''' <param name="pRefreshDS"></param>
    ''' <remarks></remarks>
    Public Overrides Sub RefreshScreen(ByVal pRefreshEventType As List(Of GlobalEnumerates.UI_RefreshEvents), ByVal pRefreshDS As Biosystems.Ax00.Types.UIRefreshDS)
        Try
            If isClosingFlag Then Return 'AG 03/08/2012

            'TR 24/10/2011 -On refresh screen then reload port combo with available ports. ---> System Errors appears
            'http://stackoverflow.com/questions/1272466/disconnectedcontext-error-while-running-unit-test-project-in-vs-2008
            '•One of your components directly or indirectly uses a native COM object
            '•The COM object is an STA object (IME most are)
            '•STA COM objects essentially live on a thread, this means that the COM Object can be access from the main Thread.
            'And fillportscombo implement COM. Istead of refreshing we close the windows.
            'FillPortsCombo()

            'Second option: Instead of reload the ports combo, close the screen (without save changes)
            Me.Enabled = False 'RH 13/04/2012 Disable the form to avoid pressing buttons on closing
            Me.Close()

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".RefreshScreen", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".RefreshScreen", Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub


#End Region

#Region "Private Methods"

    ''' <summary>
    ''' Method incharge to get and load the Icons for graphical buttons
    ''' </summary>
    ''' <remarks>
    ''' Modified by: DL 03/11/2010 - Load Icon in the Image Property instead of in BackgroundImage property
    ''' </remarks>
    Private Sub PrepareButtons()
        Try
            Dim auxIconName As String = ""
            Dim iconPath As String = IconsPath

            'SAVE Button
            auxIconName = GetIconName("ACCEPT1")
            If (auxIconName <> "") Then
                bsAcceptButton.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
            End If

            'CANCEL Button
            auxIconName = GetIconName("CANCEL")
            If (auxIconName <> "") Then
                bsCancelButton.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
            End If

            If MyClass.IsService Then
                'TEST Communications Button
                auxIconName = GetIconName("ADJUSTMENT")
                If System.IO.File.Exists(iconPath & auxIconName) Then
                    BsTestCommunicationsButton.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
                    BsTestCommunicationsButton.Visible = True
                End If
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".PrepareButtons ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".PrepareButtons ", Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Prepare screen for loading: get Icons for Graphical Buttons, get Multilanguage texts, fill ComboBoxes and assign
    ''' to each control the current value of each Setting
    ''' </summary>
    ''' <remarks>
    ''' Created by:  
    ''' Modified by: TR 27/04/2010 - Load ComboBoxes of Ports and Speeds in Communications Tab
    '''              TR 03/05/2010 - Get the settings for the active Analyzer and load them in Communications Tab
    '''              DL 20/05/2010 - Hide/Show WorkSession and LIMS Tabs depending if the screen is used by Service/User Application
    '''              DL 21/05/2010 - Load ComboBox of frequencies for export to LIMS in LIMS Tab
    '''              SA 07/06/2010 - Code move from the screen loading event
    '''              PG 06/10/2010 - Get current Language
    '''              RH 12/07/2011 - Load ComboBox of Tube Types in WorkSession Tab
    '''              DL 28/07/2011 - Added code to center the screen regarding the main MDI form
    '''              DL 09/09/2011 - Added code to get the Analyzer Manager and disable all fields when the Analyzer status is RUNNING
    '''              DL 19/10/2011 - Get the WorkSession settings and load them in WorkSession Tab
    '''              SG 28/10/2011 - Get the Analyzer Adjustments and load them in Analyzer Tab
    '''              XB 03/11/2011 - If the screen is used for Service Application, remove tabs WorkSession and LIMS, and load FW values
    '''              AG 06/02/2012 - Added code to disable all screen fields and the Accept Button also when the Analyzer is not connected
    '''              TR 22/02/2012 - If the Analyzer is connected, select the Port in the Ports ComboBox in Communications Tab
    '''              TR 30/07/2012 - In LIMS Tab, validate if the option for OnLine Export is checked to enable/disable the GroupBox
    '''              SA 06/09/2012 - Added code to disable all screen fields and the Accept Button also when the Analyzer connection is
    '''                              in process
    '''              XB 04/03/2013 - Hide LIMS Tabs for User Application
    '''              XB 06/11/2013 - Add protection against more performing operations (Starting Instrument, Shutting down, aborting WS) - BT #1150 + #1151
    '''              IT 23/10/2014 - REFACTORING (BA-2016)
    ''' </remarks>
    Private Sub ScreenLoad()
        Try
            'Center the screen
            Dim myLocation As Point = Me.Parent.Location
            Dim mySize As Size = Me.Parent.Size
            Me.Location = New Point(myLocation.X + CInt((mySize.Width - Me.Width) / 2), myLocation.Y + CInt((mySize.Height - Me.Height) / 2) - 60)

            'Get the current Language from the current Application Session
            'Dim currentLanguageGlobal As New GlobalBase
            LanguageID = GlobalBase.GetSessionInfo().ApplicationLanguage

            'Get Icons for Form Buttons
            PrepareButtons()

            'Set multilanguage texts for all form labels and tooltips...
            GetScreenLabels()

            'Get current values of Analyzer Settings and load them in Settings Tab
            LoadAnalyzerDetails()

            'Load ComboBoxes of Ports and Speeds in Settings Tab
            FillPortsCombo()
            FillSpeedsCombo()

            'Load ComboBox of export frequencies in LIMS Tab and disable the control
            'Set Manual Exportation as default value in LIMS Tab
            FillFreqCombo()
            bsFreqComboBox.Enabled = False
            bsManualRadioButtonS.Checked = True

            FillAutoReportFreq()
            FillAutoReportType()

            'Get the Analyzer Adjustments and load them in Analyzer Tab
            LoadAdjustmentGroupData()

            'Validate if the option for OnLine Export is checked to enable/disable the GroupBox in LIMS Tab
            'bsOnlineExportGroupBox.Enabled = bsOnLineExportCheckBox.Checked  DL 09/10/2012

            'Communications and Analyzer Tabs are always available (for both Service and User Applications)
            CommunicationSettingsTab.Visible = True
            AnalyzerTab.Visible = True

            If (MyClass.IsService) Then
                'Hide WorkSession and LIMS Tabs
                bsAnalyzersConfigTabControl.TabPages.Remove(SessionSettingsTab)
                bsAnalyzersConfigTabControl.TabPages.Remove(LIMSTab)
                SessionSettingsTab.Visible = False
                LIMSTab.Visible = False

                'Creathe the FW Scripts Class
                MyClass.myFwScriptDelegate = New SendFwScriptsDelegate()

                'Show GroupBox of Disabled Elements in Analyzer Tab
                bsDisabledElementsGroupBox.Visible = True

                ' XBC 25/10/2012
                AnalyzerController.Instance.Analyzer.IsConfigGeneralProcess = True '#REFACTORING
            Else
                'Show WorkSession
                SessionSettingsTab.Visible = True

                ' Hide LIMS Tab
                bsAnalyzersConfigTabControl.TabPages.Remove(LIMSTab)
                LIMSTab.Visible = False

                'Load ComboBox of Tube Types in WorkSession Tab
                FillSampleTubeTypeCombo()

                'Get the WorkSession settings and load them in WorkSession Tab
                LoadSessionDetails()

                'Hide GroupBox of Disabled Elements in Analyzer Tab
                bsDisabledElementsGroupBox.Visible = False
            End If

            'Verify if the screen has to be opened in READ-ONLY mode
            '#REFACTORING
            If (AnalyzerController.IsAnalyzerInstantiated) Then
                'If the connection process is in process, disable all screen fields (changes are not allowed) ORELSE
                'If the Analyzer is connected and its status is RUNNING, disable all screen fields (changes are not allowed)
                If (AnalyzerController.Instance.Analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.CONNECTprocess) = "INPROCESS") OrElse _
                   (AnalyzerController.Instance.Analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.ABORTprocess) = "INPROCESS") OrElse _
                   (AnalyzerController.Instance.Analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.SDOWNprocess) = "INPROCESS") OrElse _
                   Not AnalyzerController.Instance.Analyzer.AnalyzerIsReady OrElse _
                   (AnalyzerController.Instance.Analyzer.Connected AndAlso AnalyzerController.Instance.Analyzer.AnalyzerStatus = GlobalEnumerates.AnalyzerManagerStatus.RUNNING) Then
                    ' XB 06/11/2013 - WUPprocess, ABORTprocess and SDOWNprocess added 

                    For Each myControl As Control In CommunicationSettingsTab.Controls
                        myControl.Enabled = False
                    Next myControl

                    For Each myControl As Control In SessionSettingsTab.Controls
                        myControl.Enabled = False
                    Next myControl

                    For Each myControl As Control In LIMSTab.Controls
                        myControl.Enabled = False
                    Next

                    For Each myControl As Control In AnalyzerTab.Controls
                        myControl.Enabled = False
                    Next

                    bsAcceptButton.Enabled = False
                End If

                'If the Analyzer is Connected, select the PORT in the Ports ComboBox in Communications Tab
                '#REFACTORING
                If (AnalyzerController.Instance.Analyzer.Connected) Then
                    bsPortComboBox.SelectedItem = AnalyzerController.Instance.Analyzer.PortName
                End If
            End If
            EditionMode = True

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".ScreenLoad", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ScreenLoad", Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
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
                Dim pos As WINDOWPOS = DirectCast(Runtime.InteropServices.Marshal.PtrToStructure(m.LParam, _
                                                                                                 GetType(WINDOWPOS)),  _
                                                                                                 WINDOWPOS)
                Dim myLocation As Point = Me.Parent.Location
                Dim mySize As Size = Me.Parent.Size

                pos.x = myLocation.X + CInt((mySize.Width - Me.Width) / 2)
                pos.y = myLocation.Y + CInt((mySize.Height - Me.Height) / 2) - 60
                Runtime.InteropServices.Marshal.StructureToPtr(pos, m.LParam, True)
            End If

            MyBase.WndProc(m)

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".WndProc", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".WndProc", Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try

    End Sub

    ''' <summary>
    ''' Load current values of Session Settings
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 20/05/2010
    ''' Modified by: SA 05/07/2010 - Use the enum defined for each User Setting; verify there is a value before assign it to the control
    '''              RH 12/07/2011 - Added code to get value of setting for the Type of Tube used by default for Patient Samples
    '''              DL 21/07/2011 - Added code to get value of setting for automatic Barcode Scanning before Start a WorkSession
    '''              DL 04/08/2011 - Added code to get value of setting to download only Patient Sample tubes from Samples Rotor when
    '''                              the active WS is reset (Calibrator, Control and Additional Solution tubes remain in the rotor to be
    '''                              used in the next WS)
    '''              DL 19/10/2011 - Added code to get value of setting to activate the OnLine export to LIMS functionality
    ''' </remarks>
    Private Sub LoadSessionDetails()
        Try
            Dim returnData As GlobalDataTO
            Dim myUserSettingDelegate As New UserSettingsDelegate

            returnData = myUserSettingDelegate.GetCurrentValueBySettingID(Nothing, UserSettingsEnum.AUTOMATIC_RERUN.ToString())
            If (Not returnData.HasError AndAlso Not returnData.SetDatos Is Nothing) Then
                bsAutomaticRerunCheckBox.Checked = CType(returnData.SetDatos, Boolean)
            Else
                'Error getting the Session Setting value, show it 
                ShowMessage(Name & ".LoadSessionDetails ", returnData.ErrorCode, returnData.ErrorMessage)
            End If

            returnData = myUserSettingDelegate.GetCurrentValueBySettingID(Nothing, UserSettingsEnum.AUT_RESULTS_PRINT.ToString())
            If (Not returnData.HasError AndAlso Not returnData.SetDatos Is Nothing) Then
                bsAutResultPrintCheckBox.Checked = CType(returnData.SetDatos, Boolean)
            Else
                'Error getting the Session Setting value, show it 
                ShowMessage(Name & ".LoadSessionDetails ", returnData.ErrorCode, returnData.ErrorMessage)
            End If

            returnData = myUserSettingDelegate.GetCurrentValueBySettingID(Nothing, UserSettingsEnum.AUT_RESULTS_TYPE.ToString())
            If (Not returnData.HasError AndAlso Not returnData.SetDatos Is Nothing) Then
                bsAutomaticReportType.SelectedValue = CType(returnData.SetDatos, String)
            Else
                'Error getting the Session Setting value, show it 
                ShowMessage(Name & ".LoadSessionDetails ", returnData.ErrorCode, returnData.ErrorMessage)
            End If
            returnData = myUserSettingDelegate.GetCurrentValueBySettingID(Nothing, UserSettingsEnum.AUT_RESULTS_FREQ.ToString())
            If (Not returnData.HasError AndAlso Not returnData.SetDatos Is Nothing) Then
                bsAutomaticReportFrequency.SelectedValue = CType(returnData.SetDatos, String)
            Else
                'Error getting the Session Setting value, show it 
                ShowMessage(Name & ".LoadSessionDetails ", returnData.ErrorCode, returnData.ErrorMessage)
            End If

            'Indicate to if lims file is create on worksession reset.
            returnData = myUserSettingDelegate.GetCurrentValueBySettingID(Nothing, UserSettingsEnum.AUTOMATIC_RESET.ToString())
            If (Not returnData.HasError AndAlso Not returnData.SetDatos Is Nothing) Then
                bsAutomaticSessionCheckBox.Checked = CType(returnData.SetDatos, Boolean)
            Else
                'Error getting the Session Setting value, show it 
                ShowMessage(Name & ".LoadSessionDetails ", returnData.ErrorCode, returnData.ErrorMessage)
            End If

            returnData = myUserSettingDelegate.GetCurrentValueBySettingID(Nothing, UserSettingsEnum.AUTOMATIC_EXPORT.ToString())
            If (Not returnData.HasError AndAlso Not returnData.SetDatos Is Nothing) Then
                bsAutomaticRadioButtonS.Checked = CType(returnData.SetDatos, Boolean)
            Else
                'Error getting the Session Setting value, show it 
                ShowMessage(Name & ".LoadSessionDetails ", returnData.ErrorCode, returnData.ErrorMessage)
            End If

            returnData = myUserSettingDelegate.GetCurrentValueBySettingID(Nothing, UserSettingsEnum.EXPORT_FREQUENCY.ToString())
            If (Not returnData.HasError AndAlso Not returnData.SetDatos Is Nothing) Then
                bsFreqComboBox.SelectedValue = CType(returnData.SetDatos, String)
            Else
                'Error getting the Session Setting value, show it 
                ShowMessage(Name & ".LoadSessionDetails ", returnData.ErrorCode, returnData.ErrorMessage)
            End If

            returnData = myUserSettingDelegate.GetCurrentValueBySettingID(Nothing, UserSettingsEnum.DEFAULT_TUBE_PATIENT.ToString())
            If (Not returnData.HasError AndAlso Not returnData.SetDatos Is Nothing) Then
                SampleTubeTypeComboBox.SelectedValue = CType(returnData.SetDatos, String)
            Else
                'Error getting the Session Setting value, show it 
                ShowMessage(Name & ".LoadSessionDetails ", returnData.ErrorCode, returnData.ErrorMessage)
            End If

            returnData = myUserSettingDelegate.GetCurrentValueBySettingID(Nothing, UserSettingsEnum.BARCODE_BEFORE_START_WS.ToString())
            If (Not returnData.HasError AndAlso Not returnData.SetDatos Is Nothing) Then
                bsBarCodeStartCheckBox.Checked = CType(returnData.SetDatos, Boolean)
            Else
                'Error getting the Session Setting value, show it 
                ShowMessage(Name & ".LoadSessionDetails ", returnData.ErrorCode, returnData.ErrorMessage)
            End If

            returnData = myUserSettingDelegate.GetCurrentValueBySettingID(Nothing, UserSettingsEnum.RESETWS_DOWNLOAD_ONLY_PATIENTS.ToString())
            If (Not returnData.HasError AndAlso Not returnData.SetDatos Is Nothing) Then
                bsResetSamplesRotorCheckBox.Checked = CType(returnData.SetDatos, Boolean)
            Else
                'Error getting the Session Setting value, show it 
                ShowMessage(Name & ".LoadSessionDetails ", returnData.ErrorCode, returnData.ErrorMessage)
            End If

            'DL 09/10/2012
            'returnData = myUserSettingDelegate.GetCurrentValueBySettingID(Nothing, UserSettingsEnum.ONLINE_EXPORT.ToString())
            'If (Not returnData.HasError AndAlso Not returnData.SetDatos Is Nothing) Then
            '   bsOnLineExportCheckBox.Checked = CType(returnData.SetDatos, Boolean)
            'Else
            '    'Error getting the Session Setting value, show it 
            '    ShowMessage(Name & ".LoadSessionDetails ", returnData.ErrorCode, returnData.ErrorMessage)
            'End If
            'DL 09/10/2012
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".LoadSessionDetails ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".LoadSessionDetails ", Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Load current values of Analyzer Settings
    ''' </summary>
    ''' <remarks>
    ''' Modified by: DL 21/07/2011 - Used new Delegate / DAO
    '''              SA 01/08/2011 - Settings should exist in the DB; if not then it is an error; it cannot be 
    '''                              created by code. Removed calls to GetAnalyzerSettingsByAnalyzerID and CreateDefaultSettings
    '''              DL 19/10/2011 - Get also value of settings WATER_TANK and ALARM_DISABLED
    ''' </remarks>
    Private Sub LoadAnalyzerDetails()
        Try
            Dim resultData As New GlobalDataTO
            Dim myAnalyzerSettingsDelegate As New AnalyzerSettingsDelegate
            Dim myAnalyzerSettingsDS As New AnalyzerSettingsDS

            resultData = myAnalyzerSettingsDelegate.GetAnalyzerSetting(AnalyzerIDAttribute, AnalyzerSettingsEnum.COMM_PORT).GetCompatibleGlobalDataTO()
            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                myAnalyzerSettingsDS = DirectCast(resultData.SetDatos, AnalyzerSettingsDS)

                If (myAnalyzerSettingsDS.tcfgAnalyzerSettings.Rows.Count = 1) Then
                    bsPortComboBox.SelectedItem = CType(myAnalyzerSettingsDS.tcfgAnalyzerSettings.First.CurrentValue, String)
                Else
                    'Analyzer Setting is missing...
                    bsPortComboBox.SelectedIndex = -1
                    ShowMessage(Name & ".LoadAnalyzerDetails ", Messages.MASTER_DATA_MISSING.ToString)
                End If
            Else
                'Error getting the Analyzer Setting value, show it 
                ShowMessage(Name & ".LoadAnalyzerDetails ", resultData.ErrorCode, resultData.ErrorMessage)
            End If

            If (Not resultData.HasError) Then
                resultData = myAnalyzerSettingsDelegate.GetAnalyzerSetting(AnalyzerIDAttribute, AnalyzerSettingsEnum.COMM_AUTO).GetCompatibleGlobalDataTO()
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    myAnalyzerSettingsDS = DirectCast(resultData.SetDatos, AnalyzerSettingsDS)

                    If (myAnalyzerSettingsDS.tcfgAnalyzerSettings.Rows.Count = 1) Then
                        bsAutomaticRadioButtonC.Checked = CType(myAnalyzerSettingsDS.tcfgAnalyzerSettings.First.CurrentValue, Boolean)
                    Else
                        'Analyzer Setting is missing...
                        bsAutomaticRadioButtonC.Checked = False
                        ShowMessage(Name & ".LoadAnalyzerDetails ", Messages.MASTER_DATA_MISSING.ToString)
                    End If
                Else
                    'Error getting the Analyzer Setting value, show it 
                    ShowMessage(Name & ".LoadAnalyzerDetails ", resultData.ErrorCode, resultData.ErrorMessage)
                End If
                bsManualRadioButtonC.Checked = Not bsAutomaticRadioButtonC.Checked
            End If

            If (Not resultData.HasError) Then
                resultData = myAnalyzerSettingsDelegate.GetAnalyzerSetting(AnalyzerIDAttribute, AnalyzerSettingsEnum.COMM_SPEED).GetCompatibleGlobalDataTO()
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    myAnalyzerSettingsDS = DirectCast(resultData.SetDatos, AnalyzerSettingsDS)

                    If (myAnalyzerSettingsDS.tcfgAnalyzerSettings.Rows.Count = 1) Then
                        bsSpeedComboBox.SelectedItem = CType(myAnalyzerSettingsDS.tcfgAnalyzerSettings.First.CurrentValue, String)
                    Else
                        'Analyzer Setting is missing...
                        bsSpeedComboBox.SelectedIndex = -1
                        ShowMessage(Name & ".LoadAnalyzerDetails ", Messages.MASTER_DATA_MISSING.ToString)
                    End If
                Else
                    'Error getting the Analyzer Setting value, show it 
                    ShowMessage(Name & ".LoadAnalyzerDetails ", resultData.ErrorCode, resultData.ErrorMessage)
                End If
            End If

            If (Not resultData.HasError) Then
                resultData = myAnalyzerSettingsDelegate.GetAnalyzerSetting(AnalyzerIDAttribute, AnalyzerSettingsEnum.WATER_TANK).GetCompatibleGlobalDataTO()
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    myAnalyzerSettingsDS = DirectCast(resultData.SetDatos, AnalyzerSettingsDS)

                    If (myAnalyzerSettingsDS.tcfgAnalyzerSettings.Rows.Count = 1) Then
                        bsExtWaterTankRadioButton.Checked = CType(myAnalyzerSettingsDS.tcfgAnalyzerSettings.First.CurrentValue, Boolean)
                    Else
                        'Analyzer Setting is missing...
                        bsExtWaterTankRadioButton.Checked = False
                        ShowMessage(Name & ".LoadAnalyzerDetails ", Messages.MASTER_DATA_MISSING.ToString)
                    End If
                Else
                    'Error getting the Analyzer Setting value, show it 
                    ShowMessage(Name & ".LoadAnalyzerDetails ", resultData.ErrorCode, resultData.ErrorMessage)
                End If
            End If
            entryValueForExternalTankWaterCheck = bsExtWaterTankRadioButton.Checked 'AG 23/11/2011 - Remember the initial value

            If (Not resultData.HasError) Then
                resultData = myAnalyzerSettingsDelegate.GetAnalyzerSetting(AnalyzerIDAttribute, AnalyzerSettingsEnum.ALARM_DISABLED).GetCompatibleGlobalDataTO()
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    myAnalyzerSettingsDS = DirectCast(resultData.SetDatos, AnalyzerSettingsDS)

                    If (myAnalyzerSettingsDS.tcfgAnalyzerSettings.Rows.Count = 1) Then
                        bsAlarmSoundDisabledCheckbox.Checked = CType(myAnalyzerSettingsDS.tcfgAnalyzerSettings.First.CurrentValue, Boolean)
                    Else
                        'Analyzer Setting is missing...
                        bsAlarmSoundDisabledCheckbox.Checked = False
                        ShowMessage(Name & ".LoadAnalyzerDetails ", Messages.MASTER_DATA_MISSING.ToString)
                    End If
                Else
                    'Error getting the Analyzer Setting value, show it 
                    ShowMessage(Name & ".LoadAnalyzerDetails ", resultData.ErrorCode, resultData.ErrorMessage)
                End If
            End If

            'SGM 24/02/2012 get the currently stored Port for manual connection
            If (Not resultData.HasError) Then
                resultData = myAnalyzerSettingsDelegate.GetAnalyzerSetting(AnalyzerIDAttribute, AnalyzerSettingsEnum.COMM_PORT).GetCompatibleGlobalDataTO()
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    myAnalyzerSettingsDS = DirectCast(resultData.SetDatos, AnalyzerSettingsDS)

                    If (myAnalyzerSettingsDS.tcfgAnalyzerSettings.Rows.Count = 1) Then
                        Dim myPort As String = CType(myAnalyzerSettingsDS.tcfgAnalyzerSettings.First.CurrentValue, String)
                        If myPort.Trim.Length > 0 Then MyClass.CurrentManualComPort = myPort.Trim
                    Else
                        'Analyzer Setting is missing...
                        bsAlarmSoundDisabledCheckbox.Checked = False
                        ShowMessage(Name & ".LoadAnalyzerDetails ", Messages.MASTER_DATA_MISSING.ToString)
                    End If
                Else
                    'Error getting the Analyzer Setting value, show it 
                    ShowMessage(Name & ".LoadAnalyzerDetails ", resultData.ErrorCode, resultData.ErrorMessage)
                End If
            End If
            'end 24/02/2012

            If MyClass.IsService Then 'AG 24/10/2011 - Change Title for AssemblyName
                'AG 27/10/2011 - Cover deactivation (main, reagents, samples or reactions) or clot deactivation are adjusts, so read from adjustments
                'If (Not resultData.HasError) Then
                '    resultData = myAnalyzerSettingsDelegate.GetAnalyzerSetting(Nothing, AnalyzerIDAttribute, AnalyzerSettingsEnum.GRL_ANALYZER_COVER.ToString())
                '    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                '        myAnalyzerSettingsDS = DirectCast(resultData.SetDatos, AnalyzerSettingsDS)

                '        If (myAnalyzerSettingsDS.tcfgAnalyzerSettings.Rows.Count = 1) Then
                '            bsGralAnalyzerCvrCheckbox.Checked = CType(myAnalyzerSettingsDS.tcfgAnalyzerSettings.First.CurrentValue, Boolean)
                '        Else
                '            'Analyzer Setting is missing...
                '            bsGralAnalyzerCvrCheckbox.Checked = False
                '            ShowMessage(Name & ".LoadAnalyzerDetails ", Messages.MASTER_DATA_MISSING.ToString)
                '        End If
                '    Else
                '        'Error getting the Analyzer Setting value, show it 
                '        ShowMessage(Name & ".LoadAnalyzerDetails ", resultData.ErrorCode, resultData.ErrorMessage)
                '    End If
                'End If

                'If (Not resultData.HasError) Then
                '    resultData = myAnalyzerSettingsDelegate.GetAnalyzerSetting(Nothing, AnalyzerIDAttribute, AnalyzerSettingsEnum.REACTION_ROTOR_COVER.ToString())
                '    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                '        myAnalyzerSettingsDS = DirectCast(resultData.SetDatos, AnalyzerSettingsDS)

                '        If (myAnalyzerSettingsDS.tcfgAnalyzerSettings.Rows.Count = 1) Then
                '            bsReactionRotorCvrCheckbox.Checked = CType(myAnalyzerSettingsDS.tcfgAnalyzerSettings.First.CurrentValue, Boolean)
                '        Else
                '            'Analyzer Setting is missing...
                '            bsReactionRotorCvrCheckbox.Checked = False
                '            ShowMessage(Name & ".LoadAnalyzerDetails ", Messages.MASTER_DATA_MISSING.ToString)
                '        End If
                '    Else
                '        'Error getting the Analyzer Setting value, show it 
                '        ShowMessage(Name & ".LoadAnalyzerDetails ", resultData.ErrorCode, resultData.ErrorMessage)
                '    End If
                'End If

                'If (Not resultData.HasError) Then
                '    resultData = myAnalyzerSettingsDelegate.GetAnalyzerSetting(Nothing, AnalyzerIDAttribute, AnalyzerSettingsEnum.SAMPLES_ROTOR_COVER.ToString())
                '    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                '        myAnalyzerSettingsDS = DirectCast(resultData.SetDatos, AnalyzerSettingsDS)

                '        If (myAnalyzerSettingsDS.tcfgAnalyzerSettings.Rows.Count = 1) Then
                '            bsSamplesRotorCvrCheckbox.Checked = CType(myAnalyzerSettingsDS.tcfgAnalyzerSettings.First.CurrentValue, Boolean)
                '        Else
                '            'Analyzer Setting is missing...
                '            bsSamplesRotorCvrCheckbox.Checked = False
                '            ShowMessage(Name & ".LoadAnalyzerDetails ", Messages.MASTER_DATA_MISSING.ToString)
                '        End If
                '    Else
                '        'Error getting the Analyzer Setting value, show it 
                '        ShowMessage(Name & ".LoadAnalyzerDetails ", resultData.ErrorCode, resultData.ErrorMessage)
                '    End If
                'End If

                'If (Not resultData.HasError) Then
                '    resultData = myAnalyzerSettingsDelegate.GetAnalyzerSetting(Nothing, AnalyzerIDAttribute, AnalyzerSettingsEnum.REAGENTS_ROTOR_COVER.ToString())
                '    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                '        myAnalyzerSettingsDS = DirectCast(resultData.SetDatos, AnalyzerSettingsDS)

                '        If (myAnalyzerSettingsDS.tcfgAnalyzerSettings.Rows.Count = 1) Then
                '            bsReagentsRotorCvrCheckbox.Checked = CType(myAnalyzerSettingsDS.tcfgAnalyzerSettings.First.CurrentValue, Boolean)
                '        Else
                '            'Analyzer Setting is missing...
                '            bsReagentsRotorCvrCheckbox.Checked = False
                '            ShowMessage(Name & ".LoadAnalyzerDetails ", Messages.MASTER_DATA_MISSING.ToString)
                '        End If
                '    Else
                '        'Error getting the Analyzer Setting value, show it 
                '        ShowMessage(Name & ".LoadAnalyzerDetails ", resultData.ErrorCode, resultData.ErrorMessage)
                '    End If
                'End If

                'If (Not resultData.HasError) Then
                '    resultData = myAnalyzerSettingsDelegate.GetAnalyzerSetting(Nothing, AnalyzerIDAttribute, AnalyzerSettingsEnum.CLOT_DETECTION.ToString())
                '    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                '        myAnalyzerSettingsDS = DirectCast(resultData.SetDatos, AnalyzerSettingsDS)

                '        If (myAnalyzerSettingsDS.tcfgAnalyzerSettings.Rows.Count = 1) Then
                '            bsClotDetectionCheckbox.Checked = CType(myAnalyzerSettingsDS.tcfgAnalyzerSettings.First.CurrentValue, Boolean)
                '        Else
                '            'Analyzer Setting is missing...
                '            bsClotDetectionCheckbox.Checked = False
                '            ShowMessage(Name & ".LoadAnalyzerDetails ", Messages.MASTER_DATA_MISSING.ToString)
                '        End If
                '    Else
                '        'Error getting the Analyzer Setting value, show it 
                '        ShowMessage(Name & ".LoadAnalyzerDetails ", resultData.ErrorCode, resultData.ErrorMessage)
                '    End If
                'End If

                '#REFACTORING
                If (AnalyzerController.IsAnalyzerInstantiated) Then
                    'Define default values
                    Dim mainCoverDisabled As Boolean = True
                    Dim reagentsCoverDisabled As Boolean = True
                    Dim samplesCoverDisabled As Boolean = True
                    Dim reactionsCoverDisabled As Boolean = True
                    Dim clotDetectionDisabled As Boolean = True

                    'Read current Analyzer Adjustments
                    resultData = AnalyzerController.Instance.Analyzer.ReadFwAdjustmentsDS
                    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                        Dim myAdjDS As SRVAdjustmentsDS = DirectCast(resultData.SetDatos, SRVAdjustmentsDS)
                        Dim linqRes As List(Of SRVAdjustmentsDS.srv_tfmwAdjustmentsRow)

                        'Main cover disabled
                        linqRes = (From a As SRVAdjustmentsDS.srv_tfmwAdjustmentsRow In myAdjDS.srv_tfmwAdjustments _
                                   Where a.CodeFw = GlobalEnumerates.Ax00Adjustsments.MCOV.ToString Select a).ToList
                        If (linqRes.Count > 0) Then
                            If (IsNumeric(linqRes(0).Value)) Then ' XBC 04/11/2011
                                mainCoverDisabled = Not CType(linqRes(0).Value, Boolean)
                            End If
                        End If

                        'Reactions cover disabled
                        linqRes = (From a As SRVAdjustmentsDS.srv_tfmwAdjustmentsRow In myAdjDS.srv_tfmwAdjustments _
                                   Where a.CodeFw = GlobalEnumerates.Ax00Adjustsments.PHCOV.ToString Select a).ToList
                        If linqRes.Count > 0 Then
                            If IsNumeric(linqRes(0).Value) Then ' XBC 04/11/2011
                                reactionsCoverDisabled = Not CType(linqRes(0).Value, Boolean)
                            End If
                        End If

                        'Reagents cover disabled
                        linqRes = (From a As SRVAdjustmentsDS.srv_tfmwAdjustmentsRow In myAdjDS.srv_tfmwAdjustments _
                                   Where a.CodeFw = GlobalEnumerates.Ax00Adjustsments.RCOV.ToString Select a).ToList
                        If linqRes.Count > 0 Then
                            If IsNumeric(linqRes(0).Value) Then ' XBC 04/11/2011
                                reagentsCoverDisabled = Not CType(linqRes(0).Value, Boolean)
                            End If
                        End If

                        'Samples cover disabled
                        linqRes = (From a As SRVAdjustmentsDS.srv_tfmwAdjustmentsRow In myAdjDS.srv_tfmwAdjustments _
                                   Where a.CodeFw = GlobalEnumerates.Ax00Adjustsments.SCOV.ToString Select a).ToList
                        If linqRes.Count > 0 Then
                            If IsNumeric(linqRes(0).Value) Then ' XBC 04/11/2011
                                samplesCoverDisabled = Not CType(linqRes(0).Value, Boolean)
                            End If
                        End If

                        'Clot detection disabled
                        linqRes = (From a As SRVAdjustmentsDS.srv_tfmwAdjustmentsRow In myAdjDS.srv_tfmwAdjustments _
                                   Where a.CodeFw = GlobalEnumerates.Ax00Adjustsments.CLOT.ToString Select a).ToList
                        If linqRes.Count > 0 Then
                            If IsNumeric(linqRes(0).Value) Then ' XBC 04/11/2011
                                clotDetectionDisabled = Not CType(linqRes(0).Value, Boolean)
                            End If
                        End If
                    End If

                    bsGralAnalyzerCvrCheckbox.Checked = mainCoverDisabled
                    bsReactionRotorCvrCheckbox.Checked = reactionsCoverDisabled
                    bsSamplesRotorCvrCheckbox.Checked = samplesCoverDisabled
                    bsReagentsRotorCvrCheckbox.Checked = reagentsCoverDisabled
                    bsClotDetectionCheckbox.Checked = clotDetectionDisabled
                End If
                'AG 27/10/2011
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".LoadAnalyzerDetails ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".LoadAnalyzerDetails ", Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Method incharge to fill the Ports ComboBox
    ''' </summary>
    ''' <remarks>
    ''' Created by:   TR 27/04/2010
    ''' Modified by: XBC 07/09/2011 - Added simulation mode functionality
    ''' </remarks>
    Public Sub FillPortsCombo()
        Try
            'If Me.SimulationMode Then
            '    bsPortComboBox.Items.Add(MyClass.CurrentManualComPort)
            '    bsPortComboBox.SelectedIndex = 0
            '    Exit Sub
            'End If

            Dim myGlobalDataTO As GlobalDataTO
            'TR 24/10/2011 -Call the read Register ports to update every time the form is open.
            myGlobalDataTO = AnalyzerController.Instance.Analyzer.ReadRegistredPorts() '#REFACTORING
            If Not myGlobalDataTO.HasError Then
                Dim myPortsList As List(Of String) = DirectCast(myGlobalDataTO.SetDatos, List(Of String))
                If myPortsList IsNot Nothing Then
                    ''Set saved port as first
                    bsPortComboBox.Items.Add(MyClass.CurrentManualComPort)
                    For Each myPort As String In myPortsList
                        If Not bsPortComboBox.Items.Contains(myPort) Then
                            bsPortComboBox.Items.Add(myPort)
                        End If
                    Next
                End If
            Else
                'Error getting the list of values for Export Frequency, show it
                ShowMessage(Name & ".FillPortsCombo ", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage)
            End If
            'TR 24/10/2011 -END

            If (bsPortComboBox.Items.Count > 0) Then
                bsPortComboBox.SelectedIndex = 0 'Select the first PORT 
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".FillPortsCombo ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".FillPortsCombo ", Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Method incharge to fill the Speed ComboBox
    ''' </summary>
    ''' <remarks>
    ''' Created by:  TR 27/04/2010
    ''' </remarks>
    Private Sub FillSpeedsCombo()
        Try
            'Add all the available Speeds to the ComboBox
            'For Each mySpeed As String In ConfigurationManager.AppSettings("PortSpeedList").Split(CChar(",")).ToList()
            'TR 25/01/2011 -Replace by corresponding value on global base.
            For Each mySpeed As String In GlobalBase.PortSpeedList.Split(CChar(",")).ToList()
                bsSpeedComboBox.Items.Add(mySpeed)
            Next

            If (bsSpeedComboBox.Items.Count > 0) Then
                bsSpeedComboBox.SelectedIndex = 0 ' Select the first SPEED
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".FillSpeedsCombo ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".FillSpeedsCombo ", Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Method incharge to fill the Export To LIS Frequency ComboBox
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 27/04/2010
    ''' Modified by: SA 05/07/2010 - Use the enum defined for the Preloaded Master Table
    ''' </remarks>
    Private Sub FillFreqCombo()
        Try
            Dim myGlobalDataTO As New GlobalDataTO()
            Dim myPreloadedMasterDataDelegate As New PreloadedMasterDataDelegate

            myGlobalDataTO = myPreloadedMasterDataDelegate.GetList(Nothing, PreloadedMasterDataEnum.EXPORT_FREQUENCY)
            If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                Dim myPreMasterDataDS As New PreloadedMasterDataDS
                myPreMasterDataDS = CType(myGlobalDataTO.SetDatos, PreloadedMasterDataDS)

                'Sort results by Position
                Dim qReactionTypes As List(Of PreloadedMasterDataDS.tfmwPreloadedMasterDataRow)
                qReactionTypes = (From a In myPreMasterDataDS.tfmwPreloadedMasterData _
                                  Order By a.Position _
                                  Select a).ToList()

                bsFreqComboBox.DataSource = qReactionTypes
                bsFreqComboBox.DisplayMember = "FixedItemDesc"
                bsFreqComboBox.ValueMember = "ItemID"
            Else
                'Error getting the list of values for Export Frequency, show it
                ShowMessage(Name & ".FillSpeedsCombo ", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage)
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".FillSpeedsCombo ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".FillSpeedsCombo ", Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Method in charge of filling the Automatic report printing frequency. 
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 27/04/2010
    ''' Modified by: CF 29/09/2013 
    ''' </remarks>
    Private Sub FillAutoReportFreq()
        Try
            Dim myGlobalDataTO As New GlobalDataTO()
            Dim myPreloadedMasterDataDelegate As New PreloadedMasterDataDelegate

            myGlobalDataTO = myPreloadedMasterDataDelegate.GetList(Nothing, PreloadedMasterDataEnum.AUT_RESULTS_FREQ)
            If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                Dim myPreMasterDataDS As New PreloadedMasterDataDS
                myPreMasterDataDS = CType(myGlobalDataTO.SetDatos, PreloadedMasterDataDS)

                'Sort results by Position
                Dim qReactionTypes As List(Of PreloadedMasterDataDS.tfmwPreloadedMasterDataRow)
                qReactionTypes = (From a In myPreMasterDataDS.tfmwPreloadedMasterData _
                                  Order By a.Position _
                                  Select a).ToList()

                bsAutomaticReportFrequency.DataSource = qReactionTypes
                bsAutomaticReportFrequency.DisplayMember = "FixedItemDesc"
                bsAutomaticReportFrequency.ValueMember = "ItemID"
            Else
                'Error getting the list of values for Export Frequency, show it
                ShowMessage(Name & ".FillAutoReportFreq ", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage)
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".FillAutoReportFreq ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".FillAutoReportFreq ", Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub


    ''' <summary>
    ''' Method in charge of filling the Automatic report type. 
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 27/04/2010
    ''' Modified by: CF 29/09/2013 
    ''' </remarks>
    Private Sub FillAutoReportType()
        Try
            Dim myGlobalDataTO As New GlobalDataTO()
            Dim myPreloadedMasterDataDelegate As New PreloadedMasterDataDelegate

            myGlobalDataTO = myPreloadedMasterDataDelegate.GetList(Nothing, PreloadedMasterDataEnum.AUT_RESULTS_TYPE)
            If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                Dim myPreMasterDataDS As New PreloadedMasterDataDS
                myPreMasterDataDS = CType(myGlobalDataTO.SetDatos, PreloadedMasterDataDS)

                'Sort results by Position
                Dim qReactionTypes As List(Of PreloadedMasterDataDS.tfmwPreloadedMasterDataRow)
                qReactionTypes = (From a In myPreMasterDataDS.tfmwPreloadedMasterData _
                                  Order By a.Position _
                                  Select a).ToList()

                bsAutomaticReportType.DataSource = qReactionTypes
                bsAutomaticReportType.DisplayMember = "FixedItemDesc"
                bsAutomaticReportType.ValueMember = "ItemID"
            Else
                'Error getting the list of values for Export Frequency, show it
                ShowMessage(Name & ".FillAutoReportFreq ", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage)
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".FillAutoReportFreq ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".FillAutoReportFreq ", Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub


    ''' <summary>
    ''' Loads SampleTubeTypeComboBox values
    ''' </summary>
    ''' <remarks>
    ''' Created by:  RH 12/07/2011
    ''' </remarks>
    Private Sub FillSampleTubeTypeCombo()
        Try
            Dim myGlobalDataTO As GlobalDataTO
            Dim myPreloadedMasterDataDelegate As New PreloadedMasterDataDelegate

            myGlobalDataTO = myPreloadedMasterDataDelegate.GetList(Nothing, PreloadedMasterDataEnum.TUBE_TYPES_SAMPLES)

            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                Dim myPreMasterDataDS As PreloadedMasterDataDS
                myPreMasterDataDS = CType(myGlobalDataTO.SetDatos, PreloadedMasterDataDS)

                'Sort results by Position
                Dim qTubeTypes As List(Of PreloadedMasterDataDS.tfmwPreloadedMasterDataRow)
                qTubeTypes = (From a In myPreMasterDataDS.tfmwPreloadedMasterData _
                              Order By a.Position _
                              Select a).ToList()

                SampleTubeTypeComboBox.DataSource = qTubeTypes
                SampleTubeTypeComboBox.DisplayMember = "FixedItemDesc"
                SampleTubeTypeComboBox.ValueMember = "ItemID"
            Else
                'Error getting the list of values for Export Frequency, show it
                ShowMessage(Name & ".FillTubeTypeCombo ", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage)
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".FillTubeTypeCombo ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".FillTubeTypeCombo ", Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Save values set for the Communication and User Settings
    ''' </summary>
    ''' <remarks>
    ''' Created by:  VR 28/04/2010 - Tested OK
    ''' Modified by: VR 29/04/2010 - Tested OK
    '''              DL 21/05/2010
    '''              SA 05/07/2010 - Get the AnalyzerID from the screen Attribute; call new Delegate function
    '''                              to save all values in an unique DB Transaction
    '''              DL 21/07/2011 - Get the new data
    '''              XB 06/09/2011 - Don't save Tab2's values for Service Sw
    '''              XB 04/03/2013 - Hide LIMS Tabs for User Application
    ''' </remarks>
    Private Sub SaveChanges()
        Try
            Cursor = Cursors.WaitCursor

            'Load in a DS the current value of the Analyzer Settings
            Dim myAnalyzerSettingsDS As New AnalyzerSettingsDS
            Dim myAnalyzerSettingsRow As AnalyzerSettingsDS.tcfgAnalyzerSettingsRow

            'COMM_PORT
            myAnalyzerSettingsRow = myAnalyzerSettingsDS.tcfgAnalyzerSettings.NewtcfgAnalyzerSettingsRow
            With myAnalyzerSettingsRow
                .AnalyzerID = AnalyzerIDAttribute
                .SettingID = AnalyzerSettingsEnum.COMM_PORT.ToString()
                .CurrentValue = IIf(bsManualRadioButtonC.Checked, bsPortComboBox.Text, "").ToString()
            End With
            myAnalyzerSettingsDS.tcfgAnalyzerSettings.Rows.Add(myAnalyzerSettingsRow)

            'COMM_AUTO
            myAnalyzerSettingsRow = myAnalyzerSettingsDS.tcfgAnalyzerSettings.NewtcfgAnalyzerSettingsRow
            With myAnalyzerSettingsRow
                .AnalyzerID = AnalyzerIDAttribute
                .SettingID = AnalyzerSettingsEnum.COMM_AUTO.ToString()
                .CurrentValue = IIf(bsManualRadioButtonC.Checked, "0", "1").ToString()
            End With
            myAnalyzerSettingsDS.tcfgAnalyzerSettings.Rows.Add(myAnalyzerSettingsRow)

            'COMM_SPEED
            myAnalyzerSettingsRow = myAnalyzerSettingsDS.tcfgAnalyzerSettings.NewtcfgAnalyzerSettingsRow
            With myAnalyzerSettingsRow
                .AnalyzerID = AnalyzerIDAttribute
                .SettingID = AnalyzerSettingsEnum.COMM_SPEED.ToString()
                .CurrentValue = IIf(bsManualRadioButtonC.Checked, bsSpeedComboBox.Text, "").ToString
            End With
            myAnalyzerSettingsDS.tcfgAnalyzerSettings.Rows.Add(myAnalyzerSettingsRow)

            'DL 19/10/2011
            'WATER_TANK
            myAnalyzerSettingsRow = myAnalyzerSettingsDS.tcfgAnalyzerSettings.NewtcfgAnalyzerSettingsRow
            With myAnalyzerSettingsRow
                .AnalyzerID = AnalyzerIDAttribute
                .SettingID = AnalyzerSettingsEnum.WATER_TANK.ToString()
                .CurrentValue = IIf(bsExtWaterTankRadioButton.Checked, "1", "0").ToString()
            End With
            myAnalyzerSettingsDS.tcfgAnalyzerSettings.Rows.Add(myAnalyzerSettingsRow)

            myAnalyzerSettingsRow = myAnalyzerSettingsDS.tcfgAnalyzerSettings.NewtcfgAnalyzerSettingsRow
            With myAnalyzerSettingsRow
                .AnalyzerID = AnalyzerIDAttribute
                .SettingID = AnalyzerSettingsEnum.ALARM_DISABLED.ToString()
                .CurrentValue = IIf(bsAlarmSoundDisabledCheckbox.Checked, "1", "0").ToString()
            End With
            myAnalyzerSettingsDS.tcfgAnalyzerSettings.Rows.Add(myAnalyzerSettingsRow)
            'DL 19/10/2011

            ''REAGENT_BARCODE_STATUS
            'myAnalyzerSettingsRow = myAnalyzerSettingsDS.tcfgAnalyzerSettings.NewtcfgAnalyzerSettingsRow
            'With myAnalyzerSettingsRow
            '    .AnalyzerID = AnalyzerIDAttribute
            '    .SettingID = AnalyzerSettingsEnum.REAGENT_BARCODE_STATUS.ToString()
            '    '.CurrentValue = IIf(bsManualRadioButtonC.Checked, bsSpeedComboBox.Text, "").ToString
            'End With
            'myAnalyzerSettingsDS.tcfgAnalyzerSettings.Rows.Add(myAnalyzerSettingsRow)

            ''SAMPLE_BARCODE_STATUS
            'myAnalyzerSettingsRow = myAnalyzerSettingsDS.tcfgAnalyzerSettings.NewtcfgAnalyzerSettingsRow
            'With myAnalyzerSettingsRow
            '    .AnalyzerID = AnalyzerIDAttribute
            '    .SettingID = AnalyzerSettingsEnum.SAMPLE_BARCODE_STATUS.ToString()
            '    '    .CurrentValue = IIf(bsManualRadioButtonC.Checked, bsSpeedComboBox.Text, "").ToString
            'End With
            'myAnalyzerSettingsDS.tcfgAnalyzerSettings.Rows.Add(myAnalyzerSettingsRow)

            'Load in a DS the current value of the Session Settings
            Dim sessionSettings As New UserSettingDS()
            Dim sessionSettingRow As UserSettingDS.tcfgUserSettingsRow

            If Not MyClass.IsService Then 'AG 24/10/2011 change Title for AssemblyName

                sessionSettingRow = sessionSettings.tcfgUserSettings.NewtcfgUserSettingsRow
                sessionSettingRow.SettingID = UserSettingsEnum.AUTOMATIC_RERUN.ToString()
                sessionSettingRow.CurrentValue = IIf(bsAutomaticRerunCheckBox.Checked, "1", "0").ToString()
                sessionSettings.tcfgUserSettings.Rows.Add(sessionSettingRow)

                sessionSettingRow = sessionSettings.tcfgUserSettings.NewtcfgUserSettingsRow
                sessionSettingRow.SettingID = UserSettingsEnum.AUT_RESULTS_PRINT.ToString()
                sessionSettingRow.CurrentValue = IIf(bsAutResultPrintCheckBox.Checked, "1", "0").ToString()
                sessionSettings.tcfgUserSettings.Rows.Add(sessionSettingRow)

                'CF - v211 26/09/2013 - save autoreport printing settings.
                sessionSettingRow = sessionSettings.tcfgUserSettings.NewtcfgUserSettingsRow
                sessionSettingRow.SettingID = UserSettingsEnum.AUT_RESULTS_FREQ.ToString()
                sessionSettingRow.CurrentValue = bsAutomaticReportFrequency.SelectedValue.ToString()
                sessionSettings.tcfgUserSettings.Rows.Add(sessionSettingRow)

                sessionSettingRow = sessionSettings.tcfgUserSettings.NewtcfgUserSettingsRow
                sessionSettingRow.SettingID = UserSettingsEnum.AUT_RESULTS_TYPE.ToString()
                sessionSettingRow.CurrentValue = bsAutomaticReportType.SelectedValue.ToString()
                sessionSettings.tcfgUserSettings.Rows.Add(sessionSettingRow)


                ' XBC 04/03/2013 - LIMS Tab hiden
                'sessionSettingRow = sessionSettings.tcfgUserSettings.NewtcfgUserSettingsRow
                'sessionSettingRow.SettingID = UserSettingsEnum.AUTOMATIC_RESET.ToString()
                'sessionSettingRow.CurrentValue = IIf(bsAutomaticSessionCheckBox.Checked, "1", "0").ToString()
                'sessionSettings.tcfgUserSettings.Rows.Add(sessionSettingRow)

                'sessionSettingRow = sessionSettings.tcfgUserSettings.NewtcfgUserSettingsRow
                'sessionSettingRow.SettingID = UserSettingsEnum.AUTOMATIC_EXPORT.ToString()
                'sessionSettingRow.CurrentValue = IIf(bsAutomaticRadioButtonS.Checked, "1", "0").ToString()
                'sessionSettings.tcfgUserSettings.Rows.Add(sessionSettingRow)
                ' XBC 04/03/2013 - LIMS Tab hiden

                'DL 21/07/2011
                sessionSettingRow = sessionSettings.tcfgUserSettings.NewtcfgUserSettingsRow
                sessionSettingRow.SettingID = UserSettingsEnum.BARCODE_BEFORE_START_WS.ToString()
                sessionSettingRow.CurrentValue = IIf(bsBarCodeStartCheckBox.Checked, "1", "0").ToString()
                sessionSettings.tcfgUserSettings.Rows.Add(sessionSettingRow)
                'DL 21/07/2011

                ' XBC 04/03/2013 - LIMS Tab hiden
                'If (bsAutomaticRadioButtonS.Checked) Then
                '    sessionSettingRow = sessionSettings.tcfgUserSettings.NewtcfgUserSettingsRow
                '    sessionSettingRow.SettingID = UserSettingsEnum.EXPORT_FREQUENCY.ToString()
                '    sessionSettingRow.CurrentValue = bsFreqComboBox.SelectedValue.ToString()
                '    sessionSettings.tcfgUserSettings.Rows.Add(sessionSettingRow)
                'End If
                ' XBC 04/03/2013 - LIMS Tab hiden

                'RH 12/07/2011
                sessionSettingRow = sessionSettings.tcfgUserSettings.NewtcfgUserSettingsRow
                sessionSettingRow.SettingID = UserSettingsEnum.DEFAULT_TUBE_PATIENT.ToString()
                sessionSettingRow.CurrentValue = SampleTubeTypeComboBox.SelectedValue.ToString()
                sessionSettings.tcfgUserSettings.Rows.Add(sessionSettingRow)

                'DL 04/08/2011
                sessionSettingRow = sessionSettings.tcfgUserSettings.NewtcfgUserSettingsRow
                sessionSettingRow.SettingID = UserSettingsEnum.RESETWS_DOWNLOAD_ONLY_PATIENTS.ToString()
                sessionSettingRow.CurrentValue = IIf(bsResetSamplesRotorCheckBox.Checked, "1", "0").ToString()
                sessionSettings.tcfgUserSettings.Rows.Add(sessionSettingRow)

                'DL 19/10/2011
                'sessionSettingRow = sessionSettings.tcfgUserSettings.NewtcfgUserSettingsRow
                'sessionSettingRow.SettingID = UserSettingsEnum.ONLINE_EXPORT.ToString()
                'sessionSettingRow.CurrentValue = IIf(bsOnLineExportCheckBox.Checked, "1", "0").ToString() 
                'sessionSettings.tcfgUserSettings.Rows.Add(sessionSettingRow)
                'DL 09/10/2012

            End If

            Dim resultData As New GlobalDataTO

            resultData = AnalyzerSettingsDelegate.Save(Nothing, AnalyzerIDAttribute, myAnalyzerSettingsDS, sessionSettings)
            'resultData = myAnalyzersDelegate.SaveUserSettings(Nothing, commConfigData, sessionSettings)

            'TR 28/10/2011 -Validate if the sound is on then Desactivate. if the value for bsAlarmSoundDisabledCheckbox has change for false to true.
            If Not resultData.HasError Then
                If StopRinging Then
                    resultData = AnalyzerController.Instance.Analyzer.StopAnalyzerRinging() '#REFACTORING
                    If resultData.HasError Then
                        ShowMessage("Error", resultData.ErrorMessage)
                    End If
                End If
            End If
            'TR 28/10/2011 -END.

            If (resultData.HasError) Then
                'Error saving the settings, show it
                Cursor = Cursors.Default
                ShowMessage(Name & ".SaveChanges", resultData.ErrorCode, resultData.ErrorMessage)
            End If

        Catch ex As Exception
            Cursor = Cursors.Default
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".SaveChanges ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".SaveChanges ", Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")

        Finally
            Cursor = Cursors.Default

        End Try
    End Sub

    ''' <summary>
    ''' Get texts in the current application language for all screen controls
    ''' </summary>
    ''' <remarks>
    ''' Created by: PG 06/10/10
    ''' </remarks>
    Private Sub GetScreenLabels()
        Try
            Dim MLRD As New MultilanguageResourcesDelegate

            'For Labels, CheckBox, RadioButtons.....
            bsAnalyzerConfigurationLabel.Text = MLRD.GetResourceText(Nothing, "LBL_ANALYZERCONFIGURATION", LanguageID)
            AnalyzerTab.Text = MLRD.GetResourceText(Nothing, "LBL_SCRIPT_EDIT_Analyzer", LanguageID) 'JB 01/10/2012: String Resource unification

            CommunicationSettingsTab.Text = MLRD.GetResourceText(Nothing, "LBL_CfgAnalyzer_CommSettings", LanguageID)
            bsAutomaticRadioButtonC.Text = MLRD.GetResourceText(Nothing, "LBL_AUTOMATIC", LanguageID)
            bsManualRadioButtonC.Text = MLRD.GetResourceText(Nothing, "LBL_MANUAL", LanguageID)
            bsPortLabel.Text = MLRD.GetResourceText(Nothing, "LBL_CFGANALYZER_PORT", LanguageID) & ":"
            bsSpeedLabel.Text = MLRD.GetResourceText(Nothing, "LBL_CFGANALYZER_PORTSPEED", LanguageID) & ":"

            SessionSettingsTab.Text = MLRD.GetResourceText(Nothing, "LBL_Worksession", LanguageID) 'LBL_CfgAnalyzer_SessionSettings", LanguageID)
            bsAutResultPrintCheckBox.Text = MLRD.GetResourceText(Nothing, "LBL_CFGANALYZER_AUTOPRINT", LanguageID)
            bsAutomaticRerunCheckBox.Text = MLRD.GetResourceText(Nothing, "LBL_CFGANALYZER_AUTORERUN", LanguageID)
            bsAutomaticSessionCheckBox.Text = MLRD.GetResourceText(Nothing, "LBL_CFGANALYZER_EXPORTWHENRESET", LanguageID)
            'bsOnlineExportGroupBox.Text = MLRD.GetResourceText(Nothing, "LBL_CFGANALYZER_EXPORTONLINE", LanguageID)
            'bsOnLineExportCheckBox.Text = MLRD.GetResourceText(Nothing, "LBL_CFGANALYZER_EXPORTONLINE", LanguageID) DL 09/10/2012
            bsResetSamplesRotorCheckBox.Text = MLRD.GetResourceText(Nothing, "LBL_ResetSamplesRotor", LanguageID)
            bsAutomaticRadioButtonS.Text = MLRD.GetResourceText(Nothing, "LBL_AUTOMATIC", LanguageID)
            bsFrequencyLabel.Text = MLRD.GetResourceText(Nothing, "LBL_CFGANALYZER_EXPORTFREQ", LanguageID) & ":"
            bsBarCodeStartCheckBox.Text = MLRD.GetResourceText(Nothing, "LBL_CFGBARCODE_START", LanguageID)  'DL 21/07/2011
            bsManualRadioButtonS.Text = MLRD.GetResourceText(Nothing, "LBL_MANUAL", LanguageID)
            bsWaterEntranceGroupBox.Text = MLRD.GetResourceText(Nothing, "LBL_WaterEntranceSelection", LanguageID)
            'CF - 26/09/2013 Adding the text labels for the auto printing of the reports. 
            bsAutoReportTypeLabel.Text = MLRD.GetResourceText(Nothing, "LBL_Type", LanguageID) & ":"
            bsAutoReportFreqLabel.Text = MLRD.GetResourceText(Nothing, "LBL_CFGANALYZER_EXPORTFREQ", LanguageID) & ":"

            'For Tooltip
            If MyClass.IsService Then 'AG 24/10/2011 change Title for AssemblyName
                bsScreenToolTips.SetToolTip(bsAcceptButton, MLRD.GetResourceText(Nothing, "BTN_Save&Test", LanguageID))
            Else
                bsScreenToolTips.SetToolTip(bsAcceptButton, MLRD.GetResourceText(Nothing, "BTN_Save&Close", LanguageID))
            End If

            bsScreenToolTips.SetToolTip(bsCancelButton, MLRD.GetResourceText(Nothing, "BTN_Cancel&Close", LanguageID))
            bsSampleTubeTypeLabel.Text = MLRD.GetResourceText(Nothing, "LBL_CFGANALYZER_SAMPLETUBE", LanguageID) & ":"
            'bsTubeTypeLabel.Text = MLRD.GetResourceText(Nothing, "LBL_CFGANALYZER_TUBETYPE", LanguageID) & ":"

            bsExtWaterTankRadioButton.Text = MLRD.GetResourceText(Nothing, "LBL_ExternalWaterTank", LanguageID)
            bsLabWaterCircuitRadioButton.Text = MLRD.GetResourceText(Nothing, "LBL_LaboratoryWaterCircuit", LanguageID)
            bsAlarmSoundDisabledCheckbox.Text = MLRD.GetResourceText(Nothing, "LBL_AlarmSoundDisabled", LanguageID)

            'SGM 23/02/2012
            If MyClass.IsService Then
                bsDisabledElementsGroupBox.Text = MLRD.GetResourceText(Nothing, "LBL_DisabledElements", LanguageID)
                bsGralAnalyzerCvrCheckbox.Text = MLRD.GetResourceText(Nothing, "LBL_GeneralAnalyzerCover", LanguageID)
                'Labels in SAMPLES BARCODE area
                If AnalyzerController.Instance.IsBA200 Then
                    'Sample&Regeant Rotor cover
                    bsReagentsRotorCvrCheckbox.Text = MLRD.GetResourceText(Nothing, "LBL_ReagentsRotorCover_BA200", LanguageID)
                Else
                    bsSamplesRotorCvrCheckbox.Text = MLRD.GetResourceText(Nothing, "LBL_SamplesRotorCover", LanguageID)
                    bsReagentsRotorCvrCheckbox.Text = MLRD.GetResourceText(Nothing, "LBL_ReagentsRotorCover", LanguageID)
                End If

                bsReactionRotorCvrCheckbox.Text = MLRD.GetResourceText(Nothing, "LBL_ReactionRotorCover", LanguageID)
                bsClotDetectionCheckbox.Text = MLRD.GetResourceText(Nothing, "LBL_SRV_CLOT_DET", LanguageID)
                bsScreenToolTips.SetToolTip(Me.BsTestCommunicationsButton, MLRD.GetResourceText(Nothing, "SRV_BTN_Test", LanguageID) & " " & MLRD.GetResourceText(Nothing, "LBL_SRV_COMM", LanguageID))
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".GetScreenLabels ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".GetScreenLabels ", Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Test communications functionality
    ''' </summary>
    ''' <remarks>Created by XBC 06/09/2011</remarks>
    Private Sub ExecuteCommunicationsTest()
        Try

            Application.DoEvents()
            Me.Cursor = Cursors.WaitCursor

            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate
            Dim myGlobal As New GlobalDataTO

            Me.testResult = False
            Me.testExecuted = True

            'SGM 24/11/2011
            Me.bsAnalyzersConfigTabControl.Enabled = False
            Me.BsTestCommunicationsButton.Enabled = False
            Me.bsCancelButton.Enabled = False
            Me.bsAcceptButton.Enabled = False
            Me.Cursor = Cursors.WaitCursor
            'SGM 24/11/2011

            If Me.SimulationMode Then
                ' random
                Dim Rnd As New Random()
                Dim result As Integer
                result = Rnd.Next(1, 3)
                Me.testResult = (result = 1)


                'XBC 09/01/2012
                ''SGM 24/11/2011
                'Me.bsAnalyzersConfigTabControl.Enabled = True
                'Me.bsCancelButton.Enabled = True
                'Me.bsAcceptButton.Enabled = True
                'Me.Cursor = Cursors.Default
                ''SGM 24/11/2011
                'XBC 09/01/2012

            Else

                'SGM 23/02/2012
                '#REFACTORING
                If (AnalyzerController.IsAnalyzerInstantiated) Then

                    'SGM 23/02/2012
                    Dim myCommObject As Object = Nothing
                    If Me.bsAutomaticRadioButtonC.Checked Then
                        myCommObject = True
                    Else
                        Dim manualSettings As New List(Of String)
                        manualSettings.Add(CStr(Me.bsPortComboBox.SelectedItem))
                        manualSettings.Add(CStr(Me.bsSpeedComboBox.SelectedItem))
                        myCommObject = manualSettings
                    End If
                    myGlobal = AnalyzerController.Instance.Analyzer.ManageAnalyzer(AnalyzerManagerSwActionList.CONNECT, True, Nothing, myCommObject)

                    If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                        Me.testResult = AnalyzerController.Instance.Analyzer.Connected
                    End If

                End If

            End If

            ' XBC 22/10/2012
            If MyClass.IsService Then
                ' Service Sw
                If AlarmsDetected Then
                    Exit Try
                End If
            End If
            ' XBC 22/10/2012


            Dim msg As String
            If Not myGlobal.HasError Then
                If Me.testResult Then
                    msg = Messages.SRV_COMM_OK.ToString
                Else
                    msg = Messages.SRV_ERR_COMM.ToString
                End If
            Else
                msg = Messages.SYSTEM_ERROR.ToString
            End If

            If msg.Length > 0 Then
                MyBase.ShowMessage(My.Application.Info.ProductName, msg)
            End If
            'end SGM 23/02/2012

            'TR 22/02/2012 -Validate if is connected to selecte 
            'connected port on manual connection combo list.
            '#REFACTORING
            If AnalyzerController.Instance.Analyzer.Connected Then
                bsPortComboBox.SelectedItem = AnalyzerController.Instance.Analyzer.PortName
            End If
            'TR 22/02/2012 -END.

            'XBC 09/01/2012
            Me.bsAnalyzersConfigTabControl.Enabled = True
            Me.BsTestCommunicationsButton.Enabled = True
            Me.bsCancelButton.Enabled = True
            Me.bsAcceptButton.Enabled = True
            Me.Cursor = Cursors.Default
            'XBC 09/01/2012

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".ExecuteCommunicationsTest ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ExecuteCommunicationsTest ", Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        Finally
            Me.Cursor = Cursors.Default
        End Try
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks> Creation ?
    ''' AG 10/02/2014 - #1496 Mark screen closing when ReleaseElement is called
    ''' </remarks>
    Private Sub ReleaseElements()
        Try
            isClosingFlag = True 'AG 10/02/2014 - #1496 Mark screen closing when ReleaseElement is called

            bsAcceptButton.Image = Nothing
            bsCancelButton.Image = Nothing
            SelectedAdjustmentsDS = Nothing
            'mdiAnalyzerCopy = Nothing 'not this variable

            GC.Collect()

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ReleaseElements ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ReleaseElements ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub
#End Region

#Region "Events"
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>
    ''' Created by: XBC 03/11/2011
    ''' </remarks>
    Private Sub IConfigGeneral_FormClosing(ByVal sender As Object, ByVal e As FormClosingEventArgs) Handles Me.FormClosing
        Try
            Me.myFwScriptDelegate = Nothing

            'TR 11/04/2012 -Disable to avoid any action on form closing.
            'bsAnalyzersConfigTabControl.Enabled = False
            'bsAcceptButton.Enabled = False
            'bsCancelButton.Enabled = False
            'TR 11/04/2012 -END.
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".FormScriptsManager_FormClosing", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".FormScriptsManager_FormClosing", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Load the screen
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub AnalyzersConfig_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            ScreenLoad()
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".AnalyzersConfig_Load", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".AnalyzersConfig_Load", Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Close screen and discard all changes made
    ''' </summary>
    ''' <remarks>
    ''' Modified by XBC 06/09/2011 : Add test communications functionality for Service Sw
    ''' </remarks>
    Private Sub bsCancelButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsCancelButton.Click
        Try
            If MyClass.IsService Then
                ' Service Sw

                ' XBC 22/10/2012
                If AlarmsDetected Then
                    Me.Close()
                    Exit Try
                End If
                ' XBC 22/10/2012

                If Me.testExecuted Then
                    Me.InsertReport("UTIL", "COMM")
                End If
            End If

            'TR 13/02/2012 -Validate if changes made.
            If ChangesMade Then
                If (ShowMessage(Name, GlobalEnumerates.Messages.DISCARD_PENDING_CHANGES.ToString) = Windows.Forms.DialogResult.Yes) Then
                    ChangesMade = False
                    EditionMode = False
                    Me.Enabled = False 'RH 13/04/2012 Disable the form to avoid pressing buttons on closing
                    Me.Close()
                End If
            Else
                Me.Enabled = False 'RH 13/04/2012 Disable the form to avoid pressing buttons on closing
                Me.Close()
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".bsCancelButton_Click", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsCancelButton_Click", Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Close screen and save all changes made
    ''' </summary>
    ''' <remarks>
    ''' Modified by XBC 06/09/2011 : Add test communications functionality for Service Sw
    ''' Modified by XBC 13/03/2012 : Add CONFIG instruction for Sw Service too
    ''' </remarks>
    Private Sub bsAcceptButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsAcceptButton.Click
        Try
            GlobalBase.CreateLogActivity("Btn Accept", Me.Name & ".bsAcceptButton_Click", EventLogEntryType.Information, False) 'JV #1360 24/10/2013
            If MyClass.IsService Then
                Dim isToClose As Boolean = False

                ' Service Sw
                If (ChangesMade) Then
                    If (MyBase.ShowMessage(Name, GlobalEnumerates.Messages.SRV_SAVE_GENERAL_CHANGES.ToString) = Windows.Forms.DialogResult.Yes) Then

                        MyClass.SaveChanges()

                        Dim myGlobal As New GlobalDataTO

                        ' XBC 13/03/2012
                        'Instrument is ready, Instrument is in Sleep or StandBy and changes in Water Supply Selection have been made
                        '#REFACTORING
                        If AnalyzerController.Instance.Analyzer.Connected AndAlso (AnalyzerController.Instance.Analyzer.AnalyzerStatus = AnalyzerManagerStatus.SLEEPING OrElse AnalyzerController.Instance.Analyzer.AnalyzerStatus = AnalyzerManagerStatus.STANDBY) _
                           AndAlso AnalyzerController.Instance.Analyzer.AnalyzerIsReady AndAlso entryValueForExternalTankWaterCheck <> bsExtWaterTankRadioButton.Checked Then
                            myGlobal = AnalyzerController.Instance.Analyzer.ManageAnalyzer(AnalyzerManagerSwActionList.CONFIG, True)
                            If myGlobal.HasError Then
                                ShowMessage("Error", myGlobal.ErrorMessage)
                                Exit Try
                            End If
                        End If
                        ' XBC 13/03/2012

                        If MyClass.AdjustChangesMade Then
                            System.Threading.Thread.Sleep(500)
                            myGlobal = MyClass.SaveAdjustments()
                            If myGlobal.HasError Then
                                MyBase.ShowMessage(My.Application.Info.ProductName, Messages.SYSTEM_ERROR.ToString)
                                Exit Try
                            End If
                        Else
                            isToClose = True
                        End If
                    Else
                        isToClose = True
                    End If
                Else
                    isToClose = True
                End If

                If isToClose Then
                    Me.Enabled = False 'RH 13/04/2012 Disable the form to avoid pressing buttons on closing
                    Me.Close()
                End If

            Else
                ' User Sw
                If (ChangesMade) Then SaveChanges()
                'AG 23/11/2011 - Send the CONFIGURATION instruction with new values when:
                'Instrument is ready, Instrument is in Sleep or StandBy and changes in Water Supply Selection have been made
                '#REFACTORING
                If AnalyzerController.Instance.Analyzer.Connected AndAlso (AnalyzerController.Instance.Analyzer.AnalyzerStatus = AnalyzerManagerStatus.SLEEPING OrElse AnalyzerController.Instance.Analyzer.AnalyzerStatus = AnalyzerManagerStatus.STANDBY) _
                   AndAlso AnalyzerController.Instance.Analyzer.AnalyzerIsReady AndAlso entryValueForExternalTankWaterCheck <> bsExtWaterTankRadioButton.Checked Then
                    Dim myGlobal As New GlobalDataTO
                    myGlobal = AnalyzerController.Instance.Analyzer.ManageAnalyzer(AnalyzerManagerSwActionList.CONFIG, True)
                    If myGlobal.HasError Then
                        ShowMessage("Error", myGlobal.ErrorMessage)
                    End If
                End If
                'AG 23/11/2011

                Me.Enabled = False 'RH 13/04/2012 Disable the form to avoid pressing buttons on closing

                Me.Close()

            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".bsAcceptButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsAcceptButton_Click ", Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' performs the Communications test 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>SGM 23/02/2012</remarks>
    Private Sub BsTestCommunicationsButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsTestCommunicationsButton.Click
        Try
            MyClass.ExecuteCommunicationsTest()
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".BsTestCommunicationsButton_Click", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".BsTestCommunicationsButton_Click", Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Enable/Disable the ComboBoxes for PORTS and SPEED when Communication is changed between Automatic and Manual
    ''' Enable/Disable the ComboBox for FREQUENCY when the Export To LIS Setting is changed between Automatic and Manual
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub ControlValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsAutomaticRerunCheckBox.CheckedChanged, _
                                                                                                        bsAutResultPrintCheckBox.CheckedChanged, _
                                                                                                        bsPortComboBox.TextChanged, _
                                                                                                        bsSpeedComboBox.TextChanged, _
                                                                                                        bsAutomaticRadioButtonC.CheckedChanged, _
                                                                                                        bsManualRadioButtonC.CheckedChanged, _
                                                                                                        bsManualRadioButtonS.CheckedChanged, _
                                                                                                        bsAutomaticRadioButtonS.CheckedChanged, _
                                                                                                        bsOnLineExportCheckBox.CheckedChanged, _
                                                                                                        bsBarCodeStartCheckBox.CheckedChanged, _
                                                                                                        bsAlarmSoundDisabledCheckbox.CheckedChanged, _
                                                                                                        bsResetSamplesRotorCheckBox.CheckedChanged, _
                                                                                                        bsLabWaterCircuitRadioButton.CheckedChanged, _
                                                                                                        bsExtWaterTankRadioButton.CheckedChanged, _
                                                                                                        bsAutomaticSessionCheckBox.CheckedChanged

        Try
            Dim myControl As String = DirectCast(sender, Control).Name.ToString
            Select Case myControl
                Case "bsAutomaticRadioButtonC"
                    'When Automatic Communication, disable PORTS and SPEEDS ComboBoxes
                    bsPortComboBox.Enabled = Not bsAutomaticRadioButtonC.Checked
                    bsSpeedComboBox.Enabled = Not bsAutomaticRadioButtonC.Checked

                Case "bsManualRadioButtonC"
                    'When Manual Communication, enable PORTS and SPEEDS ComboBoxes
                    bsPortComboBox.Enabled = bsManualRadioButtonC.Checked
                    bsSpeedComboBox.Enabled = bsManualRadioButtonC.Checked

                Case "bsAutomaticRadioButtonS"
                    'When Automatic Export to LIS, enable FREQUENCY ComboBox
                    bsFrequencyLabel.Enabled = True
                    bsFreqComboBox.Enabled = True

                Case "bsManualRadioButtonS"
                    'When Manual Export to LIS, disable FREQUENCY ComboBox
                    bsFrequencyLabel.Enabled = False
                    bsFreqComboBox.Enabled = False

                    'DL 09/10/2012
                    'Case "bsOnLineExportCheckBox"
                    'If bsOnLineExportCheckBox.Checked Then
                    '    bsOnlineExportGroupBox.Enabled = True
                    'Else
                    '    bsOnlineExportGroupBox.Enabled = False
                    'End If
                    'DL 09/10/2012
                Case "bsAutResultPrintCheckBox"
                    If (bsAutResultPrintCheckBox.Checked) Then
                        bsAutomaticReportFrequency.Enabled = True
                        bsAutomaticReportType.Enabled = True
                    Else
                        bsAutomaticReportFrequency.Enabled = False
                        bsAutomaticReportType.Enabled = False
                    End If


            End Select

            If EditionMode Then
                ChangesMade = True
            End If


        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".ControlValueChanged ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ControlValueChanged ", Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' When the  ESC key is pressed, the screen is closed 
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 08/11/2010 
    ''' </remarks>
    Private Sub AnalyzersConfig_KeyDown(ByVal sender As Object, ByVal e As KeyEventArgs) Handles Me.KeyDown
        Try
            If (e.KeyCode = Keys.Escape) Then
                'Close()
                'RH 01/07/2011 Escape key should do exactly the same operations as bsCancelButton_Click()
                bsCancelButton.PerformClick()
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".AnalyzersConfig_KeyDown", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".AnalyzersConfig_KeyDown", Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Updates ChangesMade to True
    ''' </summary>
    ''' <remarks>
    ''' Created by:  RH 12/07/2011
    ''' </remarks>
    Private Sub SampleTubeTypeComboBox_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SampleTubeTypeComboBox.SelectedIndexChanged
        Try
            If EditionMode Then
                ChangesMade = True
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".IConfigAnalyzer_Move", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".IConfigAnalyzer_Move", Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub IConfigAnalyzer_Move(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Move
        Try
            Me.Location = myNewLocation
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".IConfigAnalyzer_Move", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".IConfigAnalyzer_Move", Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub bsAlarmSoundDisabledCheckbox_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsAlarmSoundDisabledCheckbox.CheckedChanged
        Try
            'TR 28/10/2011 -Validate if Analyzer is ringing. to disable sound when save is ok.
            If EditionMode AndAlso AnalyzerController.Instance.Analyzer.Ringing AndAlso bsAlarmSoundDisabledCheckbox.Checked Then '#REFACTORING
                StopRinging = True
            ElseIf Not bsAlarmSoundDisabledCheckbox.Checked Then
                StopRinging = False
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".bsAlarmSoundDisabledCheckbox_CheckedChanged", EventLogEntryType.Error, _
                              GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsAlarmSoundDisabledCheckbox_CheckedChanged", Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    'SGM 28/10/2011
    Private Sub DetectionCheckbox_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles bsClotDetectionCheckbox.CheckedChanged, _
                                                                                                           bsGralAnalyzerCvrCheckbox.CheckedChanged, _
                                                                                                           bsReactionRotorCvrCheckbox.CheckedChanged, _
                                                                                                           bsReagentsRotorCvrCheckbox.CheckedChanged, _
                                                                                                           bsSamplesRotorCvrCheckbox.CheckedChanged
        Try
            'TR 13/02/2012 -Set the ChangesMade.
            If EditionMode Then
                If MyClass.IsService Then MyClass.AdjustChangesMade = True
                Me.bsAcceptButton.Enabled = True
                ChangesMade = True
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".AnalyzersConfig_Load", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".AnalyzersConfig_Load", Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

#End Region

#Region "Historical reports"
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="pTaskID"></param>
    ''' <param name="pActionID"></param>
    ''' <returns></returns>
    ''' <remarks>Created by XBC 07/09/2011</remarks>
    Private Function InsertReport(ByVal pTaskID As String, ByVal pActionID As String) As GlobalDataTO
        Dim resultData As New GlobalDataTO
        Try
            Dim myHistoricalReportsDelegate As New HistoricalReportsDelegate
            Dim myHistoricReport As New SRVResultsServiceDS
            Dim myHistoricReportRow As SRVResultsServiceDS.srv_thrsResultsServiceRow

            myHistoricReportRow = myHistoricReport.srv_thrsResultsService.Newsrv_thrsResultsServiceRow
            myHistoricReportRow.TaskID = pTaskID
            myHistoricReportRow.ActionID = pActionID
            myHistoricReportRow.Data = MyClass.GenerateDataReport(myHistoricReportRow.TaskID, myHistoricReportRow.ActionID)
            myHistoricReportRow.AnalyzerID = Me.AnalyzerIDAttribute

            resultData = myHistoricalReportsDelegate.Add(Nothing, myHistoricReportRow)
            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                'Get the generated ID from the dataset returned 
                Dim generatedID As Integer = -1
                generatedID = DirectCast(resultData.SetDatos, SRVResultsServiceDS.srv_thrsResultsServiceRow).ResultServiceID

                '' Insert recommendations if existing (don't have recommendations by now !)
                'If MyClass.RecommendationsReport IsNot Nothing Then
                '    Dim myRecommendationsList As New SRVRecommendationsServiceDS
                '    Dim myRecommendationsRow As SRVRecommendationsServiceDS.srv_thrsRecommendationsServiceRow

                '    For i As Integer = 0 To MyClass.RecommendationsReport.Length - 1
                '        myRecommendationsRow = myRecommendationsList.srv_thrsRecommendationsService.Newsrv_thrsRecommendationsServiceRow
                '        myRecommendationsRow.ResultServiceID = generatedID
                '        myRecommendationsRow.RecommendationID = CInt(MyClass.RecommendationsReport(i))
                '        myRecommendationsList.srv_thrsRecommendationsService.Rows.Add(myRecommendationsRow)
                '    Next

                '    resultData = myHistoricalReportsDelegate.AddRecommendations(Nothing, myRecommendationsList)
                '    If resultData.HasError Then
                '        resultData.HasError = True
                '    End If
                'End If
            Else
                resultData.HasError = True
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".InsertReport ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".InsertReport ", Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
        Return resultData
    End Function

    ''' <summary>
    ''' Information of every test in this screen with its corresponding codification to be inserted in the common database of Historics
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>
    ''' Created by XBC 07/09/2011
    ''' 
    ''' Data Format : 
    ''' -----------
    ''' Final Result (1)
    ''' Communications mode (1)
    ''' Port value (5)
    ''' Speed value (7)
    ''' </remarks>
    Private Function GenerateDataReport(ByVal pTask As String, ByVal pAction As String) As String
        Dim returnValue As String = ""
        Try

            returnValue = ""
            ' Final Result
            If Me.testResult Then
                ' all is Ok
                returnValue += "1"
            Else
                ' any dysfunction
                returnValue += "2"
            End If

            ' Communications mode
            If bsAutomaticRadioButtonC.Checked Then
                ' automatic mode
                returnValue += "0"
            Else
                ' manual mode
                returnValue += "1"
            End If

            ' Speed value
            Dim value As Integer = 0
            If IsNumeric(Me.bsSpeedComboBox.Text) Then
                value = CInt(Me.bsSpeedComboBox.Text)
            End If
            returnValue += value.ToString("0000000")

            ' Port value
            returnValue += Me.bsPortComboBox.Text

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".GenerateDataReport ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".GenerateDataReport ", Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
        Return returnValue
    End Function
#End Region

#Region "Event Handlers"
    ''' <summary>
    ''' Executes ManageReceptionEvent() method in the main thread
    ''' </summary>
    ''' <param name="pResponse"></param>
    ''' <param name="pData"></param>
    ''' <remarks>Created by XBC 04/11/2011</remarks>
    Public Sub ScreenReceptionLastFwScriptEvent(ByVal pResponse As RESPONSE_TYPES, ByVal pData As Object) Handles myFwScriptDelegate.DataReceivedEvent

        Me.UIThread(Function() ManageReceptionEvent(pResponse, pData))

    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="pResponse"></param>
    ''' <param name="pData"></param>
    ''' <remarks>
    ''' Created by SGM 28/10/2011
    ''' Modified by XBC 03/11/2011
    ''' </remarks>
    Private Function ManageReceptionEvent(ByVal pResponse As RESPONSE_TYPES, ByVal pData As Object) As Boolean
        Dim myGlobal As New GlobalDataTO
        Try
            'manage special operations according to the screen characteristics

            ' XBC 05/05/2011 - timeout limit repetitions
            If pResponse = RESPONSE_TYPES.TIMEOUT Then
                Select Case pData.ToString
                    Case AnalyzerManagerSwActionList.TRYING_CONNECTION.ToString
                        'MyBase.DisplayMessage(Messages.TRY_CONNECTION.ToString)

                    Case AnalyzerManagerSwActionList.WAITING_TIME_EXPIRED.ToString
                        'MyBase.DisplayMessage(Messages.ERROR_COMM.ToString)
                        PrepareErrorMode()

                End Select
                Exit Function
            ElseIf pResponse = RESPONSE_TYPES.EXCEPTION Then
                PrepareErrorMode()
                Exit Function
            End If


            If MyClass.IsAdjustmentsSaving Then

                Select Case pResponse
                    Case RESPONSE_TYPES.START


                    Case RESPONSE_TYPES.OK
                        MyClass.AdjustChangesMade = True
                        MyClass.IsAdjustmentsSaving = False

                        Me.bsAnalyzersConfigTabControl.Enabled = True
                        Me.bsCancelButton.Enabled = True
                        Me.bsAcceptButton.Enabled = True
                        Me.Cursor = Cursors.Default

                        ' XBC 03/11/2011
                        'Update Adjustmensts
                        myGlobal = MyClass.UpdateAdjustments()

                        If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then

                        Else
                            MyBase.ShowMessage(My.Application.Info.ProductName, Messages.SYSTEM_ERROR.ToString)
                        End If

                        Me.Enabled = False 'RH 13/04/2012 Disable the form to avoid pressing buttons on closing
                        Me.Close()


                    Case Else
                        ' registering the incidence in historical reports activity
                        If MyClass.RecommendationsReport Is Nothing Then
                            ReDim MyClass.RecommendationsReport(0)
                        Else
                            ReDim Preserve MyClass.RecommendationsReport(UBound(MyClass.RecommendationsReport) + 1)
                        End If
                        MyClass.RecommendationsReport(UBound(MyClass.RecommendationsReport)) = HISTORY_RECOMMENDATIONS.ERR_COMM

                End Select

            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ScreenReceptionLastFwScriptEvent ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".ScreenReceptionLastFwScriptEvent", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, myGlobal.ErrorMessage, Me)
        End Try
    End Function
#End Region

#Region "Service Adjustments management"

    ''' <summary>
    ''' Loads adjustments data for updating the checkboxes
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>Created by SGM 28/10/2011</remarks>
    Private Function LoadAdjustmentGroupData() As GlobalDataTO
        Dim myGlobal As New GlobalDataTO
        Dim myAllAdjustmentsDS As New SRVAdjustmentsDS
        Dim CopyOfSelectedAdjustmentsDS As SRVAdjustmentsDS = MyClass.SelectedAdjustmentsDS
        Dim myAdjustmentsGroups As New List(Of String)
        Try
            myGlobal = AnalyzerController.Instance.Analyzer.ReadFwAdjustmentsDS '#REFACTORING
            If myGlobal.SetDatos IsNot Nothing AndAlso Not myGlobal.HasError Then
                Dim resultDS As SRVAdjustmentsDS = CType(myGlobal.SetDatos, SRVAdjustmentsDS)
                'if the global dataset is empty the force to load again from the db
                If resultDS Is Nothing OrElse resultDS.srv_tfmwAdjustments.Count = 0 Then
                    myGlobal = AnalyzerController.Instance.Analyzer.LoadFwAdjustmentsMasterData(MyClass.SimulationMode) '#REFACTORING
                    If myGlobal.SetDatos IsNot Nothing AndAlso Not myGlobal.HasError Then
                        resultDS = CType(myGlobal.SetDatos, SRVAdjustmentsDS)

                        ''update DB
                        'Dim myAdjustmentsDS As SRVAdjustmentsDS = CType(myGlobal.SetDatos, SRVAdjustmentsDS)

                        'Dim myAdjustmentsDelegate As New AdjustmentsDelegate
                        'myGlobal = myAdjustmentsDelegate.UpdateAdjustmentsDB(Nothing, myAdjustmentsDS)

                        ''update text file
                        'MyClass.myAdjustmentsDelegate = New FwAdjustmentsDelegate(myAllAdjustmentsDS)
                        'myGlobal = MyClass.myAdjustmentsDelegate.ExportDSToFile(AnalyzerController.Instance.Analyzer.ActiveAnalyzer, AnalyzerController.Instance.Analyzer.ActiveFwVersion)

                    End If
                End If

                myAllAdjustmentsDS = resultDS
                MyClass.myFwAdjustmentsDelegate = New FwAdjustmentsDelegate(myAllAdjustmentsDS)

                If Not myGlobal.HasError And MyClass.myFwAdjustmentsDelegate IsNot Nothing Then
                    If MyClass.SelectedAdjustmentsDS IsNot Nothing Then
                        MyClass.SelectedAdjustmentsDS.Clear()
                    End If
                    myAdjustmentsGroups.Add(ADJUSTMENT_GROUPS.ALARMS_CONFIG.ToString)
                    myGlobal = MyClass.myFwAdjustmentsDelegate.ReadAdjustmentsByGroupIDs(myAdjustmentsGroups)
                    If (Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing) Then
                        MyClass.SelectedAdjustmentsDS = CType(myGlobal.SetDatos, SRVAdjustmentsDS)
                    End If

                    For Each A As SRVAdjustmentsDS.srv_tfmwAdjustmentsRow In MyClass.SelectedAdjustmentsDS.srv_tfmwAdjustments.Rows
                        If String.Compare(A.Value, "", False) = 0 Then A.Value = "0" ' XBC 04/11/2011
                        Select Case A.CodeFw.ToString
                            '14/11/2011 AG: Use the enum, not embedded text .... (0 - disabled, control checked , 1 - enabled, control unchecked)
                            Case GlobalEnumerates.Ax00Adjustsments.CLOT.ToString : Me.bsClotDetectionCheckbox.Checked = CBool(IIf((CInt(A.Value) = 0), True, False))
                            Case GlobalEnumerates.Ax00Adjustsments.MCOV.ToString : Me.bsGralAnalyzerCvrCheckbox.Checked = CBool(IIf((CInt(A.Value) = 0), True, False))
                            Case GlobalEnumerates.Ax00Adjustsments.PHCOV.ToString : Me.bsReactionRotorCvrCheckbox.Checked = CBool(IIf((CInt(A.Value) = 0), True, False))
                            Case GlobalEnumerates.Ax00Adjustsments.RCOV.ToString : Me.bsReagentsRotorCvrCheckbox.Checked = CBool(IIf((CInt(A.Value) = 0), True, False))
                            Case GlobalEnumerates.Ax00Adjustsments.SCOV.ToString : Me.bsSamplesRotorCvrCheckbox.Checked = CBool(IIf((CInt(A.Value) = 0), True, False))
                        End Select
                    Next

                End If
            End If

            'SGM 21/11/2012 Set checkboxes disabled in case of Service Adjustments not loaded and sleep
            If MyClass.IsService And AnalyzerController.Instance.Analyzer.AnalyzerStatus = AnalyzerManagerStatus.SLEEPING And Not MyClass.IsServiceAdjustmentsLoaded Then '#REFACTORING
                Me.bsGralAnalyzerCvrCheckbox.Checked = False
                Me.bsReactionRotorCvrCheckbox.Checked = False
                Me.bsReagentsRotorCvrCheckbox.Checked = False
                Me.bsSamplesRotorCvrCheckbox.Checked = False
                Me.bsClotDetectionCheckbox.Checked = False
            End If
            'end SGM 21/11/2012

        Catch ex As Exception
            MyClass.SelectedAdjustmentsDS = CopyOfSelectedAdjustmentsDS
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".LoadAdjustmentGroupData ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".LoadAdjustmentGroupData ", Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
        Return myGlobal
    End Function

    Private Function UpdateAdjustments() As GlobalDataTO
        Dim myGlobal As New GlobalDataTO
        Try
            If MyClass.SelectedAdjustmentsDS IsNot Nothing Then
                'update the dataset
                myGlobal = MyClass.myFwAdjustmentsDelegate.UpdateAdjustments(MyClass.SelectedAdjustmentsDS, AnalyzerController.Instance.Analyzer.ActiveAnalyzer) '#REFACTORING

                If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                    'update DDBB
                    Dim myAdjustmentsDelegate As New DBAdjustmentsDelegate
                    myGlobal = myAdjustmentsDelegate.UpdateAdjustmentsDB(Nothing, MyClass.SelectedAdjustmentsDS)

                    If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                        'update Adjustments File
                        myGlobal = MyClass.myFwAdjustmentsDelegate.ExportDSToFile(AnalyzerController.Instance.Analyzer.ActiveAnalyzer) '#REFACTORING
                    End If
                End If
            End If

        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myGlobal.ErrorMessage = ex.Message
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " UpdateAdjustments ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
        Return myGlobal
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>Created by SGM 28/10/2011</remarks>
    Private Function SaveAdjustments() As GlobalDataTO
        Dim myGlobal As New GlobalDataTO
        Dim exMessage As String = ""
        Try
            ' Convert dataset to String for sending to Fw

            ' XBC 03/11/2011
            ' myGlobal = MyClass.myAdjustmentsDelegate.ConvertDSToString()

            ' Update dataset of the temporal dataset of Adjustments to sent to Fw
            myGlobal = UpdateTemporalAdjustmentsDS()

            If Not myGlobal.HasError Then
                TempToSendAdjustmentsDelegate = New FwAdjustmentsDelegate(SelectedAdjustmentsDS)
                ' Convert dataset to String for sending to Fw
                myGlobal = Me.TempToSendAdjustmentsDelegate.ConvertDSToString()
            End If
            ' XBC 03/11/2011

            If Not myGlobal.SetDatos Is Nothing AndAlso Not myGlobal.HasError Then
                Dim pAdjuststr As String = CType(myGlobal.SetDatos, String)

                Me.bsAnalyzersConfigTabControl.Enabled = False
                Me.bsCancelButton.Enabled = False
                Me.bsAcceptButton.Enabled = False
                Me.Cursor = Cursors.WaitCursor

                If MyClass.SimulationModeAttr Then
                    ' simulating

                    System.Threading.Thread.Sleep(500)

                    MyClass.IsAdjustmentsSaving = False
                    MyClass.AdjustChangesMade = False
                    Me.bsCancelButton.Enabled = True
                    Me.bsAnalyzersConfigTabControl.Enabled = True
                    Me.bsCancelButton.Enabled = True
                    Me.bsAcceptButton.Enabled = True
                    Me.Cursor = Cursors.Default

                Else
                    If Not myGlobal.HasError AndAlso AnalyzerController.Instance.Analyzer.Connected Then '#REFACTORING
                        myGlobal = MyClass.SendLOAD_ADJUSTMENTS(pAdjuststr)
                    End If

                    If Not myGlobal.HasError Then
                        'Me.bsAcceptButton.Enabled = True
                        'Me.bsAnalyzersConfigTabControl.Enabled = True
                    End If
                End If
            End If

        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myGlobal.ErrorMessage = ex.Message
            exMessage = ex.Message + " ((" + ex.HResult.ToString + "))"
        End Try

        If myGlobal.HasError Then
            GlobalBase.CreateLogActivity(exMessage, Me.Name & " SaveAdjustments ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, exMessage)
        End If
        Return myGlobal
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="pValueAdjustAttr"></param>
    ''' <returns></returns>
    ''' <remarks>Created by SGM 28/10/2011</remarks>
    Public Function SendLOAD_ADJUSTMENTS(ByVal pValueAdjustAttr As String) As GlobalDataTO
        Dim myResultData As New GlobalDataTO
        Try
            MyClass.IsAdjustmentsSaving = True
            Me.bsCancelButton.Enabled = False
            Me.bsAcceptButton.Enabled = False
            Me.bsAnalyzersConfigTabControl.Enabled = False
            myResultData = AnalyzerController.Instance.Analyzer.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.LOADADJ, True, Nothing, pValueAdjustAttr) '#REFACTORING

        Catch ex As Exception
            myResultData.HasError = True
            myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myResultData.ErrorMessage = ex.Message

            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " SendLOAD_ADJUSTMENTS ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
        Return myResultData
    End Function

    ''' <summary>
    ''' updates all a temporal dataset with changed current adjustments
    ''' When control Checked -> adjust value 0 (disabled)
    ''' When control unChecked -> adjust value 1 (enabled)
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>
    ''' XBC 03/11/2011
    ''' AG 24/11/2011 - invert the adjusts values
    ''' XBC 22/03/2012 - Not checked items means Disabled components
    ''' </remarks>
    Private Function UpdateTemporalAdjustmentsDS() As GlobalDataTO
        Dim myGlobal As New GlobalDataTO
        Try
            If MyClass.IsService Then
                '14/11/2011 AG .......... Use the enums not the embedded text
                If Me.bsGralAnalyzerCvrCheckbox.Checked Then
                    Me.UpdateTemporalSpecificAdjustmentsDS(GlobalEnumerates.Ax00Adjustsments.MCOV.ToString, "0") 'Disabled
                Else
                    Me.UpdateTemporalSpecificAdjustmentsDS(GlobalEnumerates.Ax00Adjustsments.MCOV.ToString, "1") 'Enabled
                End If

                If Me.bsReactionRotorCvrCheckbox.Checked Then
                    Me.UpdateTemporalSpecificAdjustmentsDS(GlobalEnumerates.Ax00Adjustsments.PHCOV.ToString, "0") 'Disabled
                Else
                    Me.UpdateTemporalSpecificAdjustmentsDS(GlobalEnumerates.Ax00Adjustsments.PHCOV.ToString, "1") 'Enabled
                End If

                If Me.bsReagentsRotorCvrCheckbox.Checked Then
                    Me.UpdateTemporalSpecificAdjustmentsDS(GlobalEnumerates.Ax00Adjustsments.RCOV.ToString, "0") 'Disabled
                Else
                    Me.UpdateTemporalSpecificAdjustmentsDS(GlobalEnumerates.Ax00Adjustsments.RCOV.ToString, "1") 'Enabled
                End If

                If Me.bsSamplesRotorCvrCheckbox.Checked Then
                    Me.UpdateTemporalSpecificAdjustmentsDS(GlobalEnumerates.Ax00Adjustsments.SCOV.ToString, "0") 'Disabled
                Else
                    Me.UpdateTemporalSpecificAdjustmentsDS(GlobalEnumerates.Ax00Adjustsments.SCOV.ToString, "1") 'Enabled
                End If

                If Me.bsClotDetectionCheckbox.Checked Then
                    Me.UpdateTemporalSpecificAdjustmentsDS(GlobalEnumerates.Ax00Adjustsments.CLOT.ToString, "0") 'Disabled
                Else
                    Me.UpdateTemporalSpecificAdjustmentsDS(GlobalEnumerates.Ax00Adjustsments.CLOT.ToString, "1") 'Enabled
                End If

                'Else
                ' TODO
            End If

            Me.SelectedAdjustmentsDS.AcceptChanges()

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".UpdateTemporalAdjustmentsDS ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".UpdateTemporalAdjustmentsDS ", Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
        Return myGlobal
    End Function

    ''' <summary>
    ''' Update some value in the specific adjustments ds
    ''' </summary>
    ''' <param name="pCodew"></param>
    ''' <param name="pValue"></param>
    ''' <returns></returns>
    ''' <remarks>XBC 03/11/2011</remarks>
    Private Function UpdateTemporalSpecificAdjustmentsDS(ByVal pCodew As String, ByVal pValue As String) As GlobalDataTO
        Dim myGlobal As New GlobalDataTO
        Try
            For Each SR As SRVAdjustmentsDS.srv_tfmwAdjustmentsRow In Me.SelectedAdjustmentsDS.srv_tfmwAdjustments.Rows
                If SR.CodeFw.Trim = pCodew.Trim Then
                    SR.BeginEdit()
                    SR.Value = pValue
                    SR.EndEdit()
                    Exit For
                End If
            Next



        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myGlobal.ErrorMessage = ex.Message
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".UpdateTemporalSpecificAdjustmentsDS ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".UpdateTemporalSpecificAdjustmentsDS", Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
        Me.SelectedAdjustmentsDS.AcceptChanges()
        Return myGlobal
    End Function

    ''' <summary>
    ''' Prepare GUI for Error Mode
    ''' </summary>
    ''' <remarks>Created by XBC 04/11/2011</remarks>
    Public Sub PrepareErrorMode(Optional ByVal pAlarmType As ManagementAlarmTypes = ManagementAlarmTypes.NONE)
        Try
            AlarmsDetected = True
            Me.bsAcceptButton.Enabled = False
            Me.BsTestCommunicationsButton.Enabled = False
            Me.bsCancelButton.Enabled = True ' Just Exit button is enabled in error case

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".PrepareErrorMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareErrorMode ", Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        Finally
            Me.Cursor = Cursors.Default
        End Try
    End Sub

    ''' <summary>
    ''' It manages the Stop of the operation(s) that are currently being performed. 
    ''' </summary>
    ''' <param name="pAlarmType"></param>
    ''' <remarks>
    ''' Created by SGM 19/10/2012
    ''' Updated by XBC 22/10/2012 - Add business logic with every current operation
    ''' </remarks>
    Public Sub StopCurrentOperation(Optional ByVal pAlarmType As GlobalEnumerates.ManagementAlarmTypes = ManagementAlarmTypes.NONE)
        Try
            PrepareErrorMode()

            RaiseEvent StopCurrentOperationFinished(pAlarmType)

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".StopCurrentOperation ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".StopCurrentOperation ", Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        Finally
            Me.Cursor = Cursors.Default
        End Try
    End Sub

#End Region


    Private Sub bsFreqComboBox_SelectionChangeCommitted(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsFreqComboBox.SelectionChangeCommitted
        If EditionMode Then
            ChangesMade = True
        End If
    End Sub

    Private Sub IConfigGeneral_FormClosed(ByVal sender As System.Object, ByVal e As System.Windows.Forms.FormClosedEventArgs) Handles MyBase.FormClosed
        If IsService Then
            RaiseEvent FormIsClosed(Me) 'SGM 08/11/2012
        End If
        ReleaseElements()
    End Sub

    Private Sub bsAutomaticReportType_SelectedIndexChanged(sender As Object, e As EventArgs) Handles bsAutomaticReportType.SelectedIndexChanged
        If EditionMode Then
            ChangesMade = True
        End If
    End Sub

    Private Sub bsAutomaticReportFrequency_SelectedIndexChanged(sender As Object, e As EventArgs) Handles bsAutomaticReportFrequency.SelectedIndexChanged
        If EditionMode Then
            ChangesMade = True
        End If
    End Sub
End Class
