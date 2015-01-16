Option Explicit On
Option Strict Off

Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Global.GlobalEnumerates
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.CommunicationsSwFw
Imports LIS.Biosystems.Ax00.LISCommunications
Imports System.Text

Public Class IConfigLIS
    Inherits Biosystems.Ax00.PresentationCOM.BSBaseForm

#Region "Attributes"
    Private AnalyzerIDAttribute As String = ""
#End Region

#Region "Properties"
    Public WriteOnly Property ActiveAnalyzer() As String
        Set(ByVal value As String)
            AnalyzerIDAttribute = value
        End Set
    End Property
#End Region

#Region "Declarations"
    Private ChangesMade As Boolean
    Private ChangesMadeLIS As Boolean
    Private EditionMode As Boolean
    Private LanguageID As String
    Private mdiAnalyzerCopy As AnalyzerManager
    Private mdiESWrapperCopy As ESWrapper
    Private ValidationError As Boolean = False
    Protected wfPreload As Biosystems.Ax00.PresentationCOM.WaitScreen
    Private MaxReceptionInitialValue As Integer
    Private MaxTransmissionInitialValue As Integer

    ' No works well if there are a thread created from this screen because it is angry with the threads created on MDI
    'Private createChannelLISManagerThread As Thread

    ' XB 15/05/2013
    Private myInitialValueField_Sep_HL7 As String
    Private myInitialValueComp_Sep_HL7 As String
    Private myInitialValueRep_Sep_HL7 As String
    Private myInitialValueSpec_Sep_HL7 As String
    Private myInitialValueSubComp_Sep_HL7 As String
    Private myInitialValueField_Sep_ASTM As String
    Private myInitialValueComp_Sep_ASTM As String
    Private myInitialValueRep_Sep_ASTM As String
    Private myInitialValueSpec_Sep_ASTM As String

    ' XB 27/05/2013
    Private myPortLabel As String
    Private myPortClientLabel As String
    Private myPortServerLabel As String

    ' XB 30/05/2013
    Private minCommonTimerbyHL7 As Integer
    Private maxCommonTimerbyHL7 As Integer
    Private minCommonTimerbyASTM As Integer
    Private maxCommonTimerbyASTM As Integer
#End Region

#Region "Private Methods"

    ''' <summary>
    ''' Prepare screen for loading: get Icons for Graphical Buttons, get Multilanguage texts, fill ComboBoxes and assign
    ''' to each control the current value of each Setting
    ''' </summary>
    ''' <remarks>
    ''' Created by:  XB 04/03/2013
    ''' </remarks>
    Private Sub ScreenLoad()
        Try
            'Get the Analyzer Manager
            If (Not AppDomain.CurrentDomain.GetData("GlobalAnalyzerManager") Is Nothing) Then
                mdiAnalyzerCopy = CType(AppDomain.CurrentDomain.GetData("GlobalAnalyzerManager"), AnalyzerManager)
            End If

            ChangesMade = False
            ChangesMadeLIS = False

            'Center the screen
            Dim myLocation As Point = Me.Parent.Location
            Dim mySize As Size = Me.Parent.Size
            Me.Location = New Point(myLocation.X + CInt((mySize.Width - Me.Width) / 2), myLocation.Y + CInt((mySize.Height - Me.Height) / 2) - 60)

            'Get the current Language from the current Application Session
            'Dim currentLanguageGlobal As New GlobalBase
            LanguageID = GlobalBase.GetSessionInfo().ApplicationLanguage

            If Not AppDomain.CurrentDomain.GetData("GlobalLISManager") Is Nothing Then
                mdiESWrapperCopy = CType(AppDomain.CurrentDomain.GetData("GlobalLISManager"), ESWrapper) ' Use the same ESWrapper as the MDI
            End If

            'Get Icons for Form Buttons
            PrepareButtons()

            'Set multilanguage texts for all form labels and tooltips...
            GetScreenLabels()

            'Load all ComboBoxes
            FillFrequencyCombo()
            bsFreqComboBox.Enabled = False
            bsFrequencyLabel.Enabled = False
            FillWSRerunsCombo()
            FillProtocolNameCombo()

            ' Get limits values
            GetLimitValues()

            ' Settings NO visible by now : 
            BsAcceptUnsolicitedOrdersCheckbox.Visible = False
            BsAcceptOnRunningCheckbox.Visible = False
            BsWSReflexTestsLabel.Visible = False
            BsWSReflexTestsComboBox.Visible = False
            'FillWSReflexTestsCombo()

            'Get current values of LIS Settings and load them in Settings Tab
            LoadLISDetails()

            EnableScreen()

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".ScreenLoad", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ScreenLoad", Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Enable or disable controls according the information received
    ''' </summary>
    ''' <remarks>
    ''' Created by: XB 24/04/2013 - This function is called when the form loads and every Refresh
    ''' AG 10/02/2014 - #1496</remarks>
    Private Sub EnableScreen()
        Try
            If isClosingFlag Then Return 'AG 10/02/2014 - #1496 do not refresh screen when closing it

            'Verify if the screen has to be opened in READ-ONLY mode
            If (Not mdiAnalyzerCopy Is Nothing) Then
                'If the connection process is in process, disable all screen fields (changes are not allowed) ORELSE
                'If the Analyzer is connected and its status is RUNNING, disable all screen fields (changes are not allowed)
                If (mdiAnalyzerCopy.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.CONNECTprocess) = "INPROCESS") OrElse _
                   (mdiAnalyzerCopy.Connected AndAlso mdiAnalyzerCopy.AnalyzerStatus = GlobalEnumerates.AnalyzerManagerStatus.RUNNING) Then

                    For Each myControl As Control In SessionSettingsTab.Controls
                        myControl.Enabled = False
                    Next myControl

                    For Each myControl As Control In CommunicationsSettingsTab.Controls
                        myControl.Enabled = False
                    Next myControl

                    For Each myControl As Control In ProtocolSettingsTab.Controls
                        myControl.Enabled = False
                    Next

                    bsAcceptButton.Enabled = False

                    Exit Try
                End If

            End If

            For Each myControl As Control In SessionSettingsTab.Controls
                myControl.Enabled = True
            Next myControl

            For Each myControl As Control In CommunicationsSettingsTab.Controls
                myControl.Enabled = True
            Next myControl

            For Each myControl As Control In ProtocolSettingsTab.Controls
                myControl.Enabled = True
            Next

            bsAcceptButton.Enabled = True

            'Load level permissions
            'Dim myGlobalbase As New GlobalBase
            CurrentUserLevel = GlobalBase.GetSessionInfo.UserLevel
            ScreenStatusByUserLevel()

            ' Prepare Screen fields
            'When Automatic Upload to LIS, enable FREQUENCY ComboBox
            If BsAutomaticCheckbox.Checked Then
                bsFrequencyLabel.Enabled = True
                bsFreqComboBox.Enabled = True
            Else
                bsFrequencyLabel.Enabled = False
                bsFreqComboBox.Enabled = False
            End If

            'When LIS Comms is not checked, disable whole LIS communications Groupbox
            If BsLISCommsEnabledCheckbox.Checked Then
                If CurrentUserLevel <> "OPERATOR" Then
                    bsLISCommsGroupBox.Enabled = True
                    bsInternalGroupBox.Enabled = True
                End If
            Else
                bsLISCommsGroupBox.Enabled = False
                bsInternalGroupBox.Enabled = False
            End If

            Select Case BsDataTransmissionComboBox.SelectedValue
                Case "TCPIP-Client"
                    HostNameTextBox.Mandatory = True
                    TCPPort2Label.Visible = False
                    TCPPort2NumericUpDown.Visible = False
                    TCPPortLabel.Text = myPortLabel & ":"
                Case "TCPIP-Server"
                    HostNameTextBox.Mandatory = False
                    TCPPort2Label.Visible = False
                    TCPPort2NumericUpDown.Visible = False
                    TCPPortLabel.Text = myPortLabel & ":"
                Case "TCPIP-Trans"
                    HostNameTextBox.Mandatory = True
                    TCPPort2Label.Visible = True
                    TCPPort2NumericUpDown.Visible = True
                    TCPPortLabel.Text = myPortClientLabel & ":"
                    TCPPort2Label.Text = myPortServerLabel & ":"
            End Select

            If bsProtocolNameComboBox.SelectedValue = "HL7" Then
                bsIHECompliantCheckbox.Enabled = True
                BsSubcomponentLabel.Visible = True
                BsSubComponentSeparatorTextBox.Visible = True
                BsHostQueryPackageLabel.Enabled = False
                BsHostQueryPackageNumericUpDown.Enabled = False
            Else
                bsIHECompliantCheckbox.Enabled = False
                BsSubcomponentLabel.Visible = False
                BsSubComponentSeparatorTextBox.Visible = False
                BsHostQueryPackageLabel.Enabled = True
                BsHostQueryPackageNumericUpDown.Enabled = True
            End If

            EditionMode = True

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".EnableScreen", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".EnableScreen", Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Get Limits values from BD for form elements
    ''' </summary>
    ''' <remarks>Created by XBC 25/03/2013</remarks>
    Private Function GetLimitValues() As GlobalDataTO
        Dim myGlobal As New GlobalDataTO
        Try
            ' Get Value limit ranges
            Dim myFieldLimitsDelegate As New FieldLimitsDelegate()
            Dim myFieldLimitsDS As New FieldLimitsDS

            myGlobal = myFieldLimitsDelegate.GetList(Nothing, FieldLimitsEnum.LIS_PORTS_LIMIT)
            If Not myGlobal.HasError Then
                myFieldLimitsDS = CType(myGlobal.SetDatos, FieldLimitsDS)
                If myFieldLimitsDS.tfmwFieldLimits.Rows.Count > 0 Then
                    TCPPortNumericUpDown.Minimum = CType(myFieldLimitsDS.tfmwFieldLimits(0).MinValue, Decimal)
                    TCPPortNumericUpDown.Maximum = CType(myFieldLimitsDS.tfmwFieldLimits(0).MaxValue, Decimal)
                    TCPPort2NumericUpDown.Minimum = CType(myFieldLimitsDS.tfmwFieldLimits(0).MinValue, Decimal)
                    TCPPort2NumericUpDown.Maximum = CType(myFieldLimitsDS.tfmwFieldLimits(0).MaxValue, Decimal)
                End If
            Else
                ShowMessage("Error", myGlobal.ErrorCode)
                Exit Try
            End If
            myGlobal = myFieldLimitsDelegate.GetList(Nothing, FieldLimitsEnum.LIS_SIZE_STORAGE_LIMIT)
            If Not myGlobal.HasError Then
                myFieldLimitsDS = CType(myGlobal.SetDatos, FieldLimitsDS)
                If myFieldLimitsDS.tfmwFieldLimits.Rows.Count > 0 Then
                    BsMaxReceptionMsgsNumericUpDown.Minimum = CType(myFieldLimitsDS.tfmwFieldLimits(0).MinValue, Decimal)
                    BsMaxReceptionMsgsNumericUpDown.Maximum = CType(myFieldLimitsDS.tfmwFieldLimits(0).MaxValue, Decimal)
                    BsMaxTransmissionMsgsNumericUpDown.Minimum = CType(myFieldLimitsDS.tfmwFieldLimits(0).MinValue, Decimal)
                    BsMaxTransmissionMsgsNumericUpDown.Maximum = CType(myFieldLimitsDS.tfmwFieldLimits(0).MaxValue, Decimal)
                End If
            Else
                ShowMessage("Error", myGlobal.ErrorCode)
                Exit Try
            End If

            ' XB 30/05/2013
            myGlobal = myFieldLimitsDelegate.GetList(Nothing, FieldLimitsEnum.LIS_TIMER_LIMIT)
            If Not myGlobal.HasError Then
                myFieldLimitsDS = CType(myGlobal.SetDatos, FieldLimitsDS)
                If myFieldLimitsDS.tfmwFieldLimits.Rows.Count > 0 Then
                    BsMaxTimeToRespondNumericUpDown.Minimum = CType(myFieldLimitsDS.tfmwFieldLimits(0).MinValue, Decimal)
                    BsMaxTimeToRespondNumericUpDown.Maximum = CType(myFieldLimitsDS.tfmwFieldLimits(0).MaxValue, Decimal)
                    BsmaxTimeWaitingForResponseNumericUpDown.Minimum = CType(myFieldLimitsDS.tfmwFieldLimits(0).MinValue, Decimal)
                    BsmaxTimeWaitingForResponseNumericUpDown.Maximum = CType(myFieldLimitsDS.tfmwFieldLimits(0).MaxValue, Decimal)
                    minCommonTimerbyHL7 = CType(myFieldLimitsDS.tfmwFieldLimits(0).MinValue, Decimal)
                    maxCommonTimerbyHL7 = CType(myFieldLimitsDS.tfmwFieldLimits(0).MaxValue, Decimal)
                End If
                myGlobal = myFieldLimitsDelegate.GetList(Nothing, FieldLimitsEnum.LIS_TIMER_LIMIT_ASTM)
                If Not myGlobal.HasError Then
                    myFieldLimitsDS = CType(myGlobal.SetDatos, FieldLimitsDS)
                    If myFieldLimitsDS.tfmwFieldLimits.Rows.Count > 0 Then
                        minCommonTimerbyASTM = CType(myFieldLimitsDS.tfmwFieldLimits(0).MinValue, Decimal)
                        maxCommonTimerbyASTM = CType(myFieldLimitsDS.tfmwFieldLimits(0).MaxValue, Decimal)
                    End If
                Else
                    ShowMessage("Error", myGlobal.ErrorCode)
                    Exit Try
                End If

                If bsProtocolNameComboBox.SelectedValue = "HL7" Then
                    BsmaxTimeWaitACKNumericUpDown.Minimum = minCommonTimerbyHL7
                    BsmaxTimeWaitACKNumericUpDown.Maximum = maxCommonTimerbyHL7
                Else
                    BsmaxTimeWaitACKNumericUpDown.Minimum = minCommonTimerbyASTM
                    BsmaxTimeWaitACKNumericUpDown.Maximum = maxCommonTimerbyASTM
                End If
            Else
                ShowMessage("Error", myGlobal.ErrorCode)
                Exit Try
            End If
            ' XB 30/05/2013

            myGlobal = myFieldLimitsDelegate.GetList(Nothing, FieldLimitsEnum.LIS_HOSTQUERY_PACK_LIMIT)
            If Not myGlobal.HasError Then
                myFieldLimitsDS = CType(myGlobal.SetDatos, FieldLimitsDS)
                If myFieldLimitsDS.tfmwFieldLimits.Rows.Count > 0 Then
                    BsHostQueryPackageNumericUpDown.Minimum = CType(myFieldLimitsDS.tfmwFieldLimits(0).MinValue, Decimal)
                    BsHostQueryPackageNumericUpDown.Maximum = CType(myFieldLimitsDS.tfmwFieldLimits(0).MaxValue, Decimal)
                End If
            Else
                ShowMessage("Error", myGlobal.ErrorCode)
                Exit Try
            End If

            'SGM 11/07/2013 - max time to wait LIS orders
            myGlobal = myFieldLimitsDelegate.GetList(Nothing, FieldLimitsEnum.LIS_MAX_WAIT_ORDERS_LIMIT)
            If Not myGlobal.HasError Then
                myFieldLimitsDS = CType(myGlobal.SetDatos, FieldLimitsDS)
                If myFieldLimitsDS.tfmwFieldLimits.Rows.Count > 0 Then
                    BsBsMaxTimeToWaitLISOrdersNumericUpDown.Minimum = CType(myFieldLimitsDS.tfmwFieldLimits(0).MinValue, Decimal)
                    BsBsMaxTimeToWaitLISOrdersNumericUpDown.Maximum = CType(myFieldLimitsDS.tfmwFieldLimits(0).MaxValue, Decimal)
                End If
            Else
                ShowMessage("Error", myGlobal.ErrorCode)
                Exit Try
            End If

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".GetLimitValues ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".GetLimitValues ", Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
        Return myGlobal
    End Function


    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>
    ''' CREATED BY: TR 05/03/2013
    ''' Modified by XB 19/03/2013
    ''' </remarks>
    Private Sub ScreenStatusByUserLevel()
        Try
            Select Case CurrentUserLevel    '.ToUpper()
                Case "ADMINISTRATOR"
                    'Comunication TAB
                    bsInternalGroupBox.Visible = False
                    Exit Select

                Case "SUPERVISOR"
                    'Comunication TAB
                    bsInternalGroupBox.Visible = False
                    Exit Select

                Case "OPERATOR"
                    'WorkSession TAB.
                    BsDownloadGroupBox.Enabled = False
                    BsUploadGroupBox.Enabled = False

                    'Comunication TAB
                    BsLISCommsEnabledCheckbox.Enabled = False
                    bsLISCommsGroupBox.Enabled = False
                    bsInternalGroupBox.Visible = False

                    'Protocol TAB
                    bsLISProtocolGroupBox.Enabled = False

                    'Buttons 
                    bsAcceptButton.Enabled = False

                    Exit Select
            End Select

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".ScreenStatusByUserLevel ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ScreenStatusByUserLevel ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Method incharge to get and load the Icons for graphical buttons
    ''' </summary>
    ''' <remarks>
    ''' Created by XB 04/03/2013 - Load Icon in the Image Property instead of in BackgroundImage property
    ''' </remarks>
    Private Sub PrepareButtons()
        Try
            Dim auxIconName As String = ""
            Dim iconPath As String = IconsPath

            'SAVE Button
            auxIconName = GetIconName("ACCEPT1")
            If (auxIconName <> "") Then
                bsAcceptButton.Image = Image.FromFile(iconPath & auxIconName)
            End If

            'CANCEL Button
            auxIconName = GetIconName("CANCEL")
            If (auxIconName <> "") Then
                bsCancelButton.Image = Image.FromFile(iconPath & auxIconName)
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".PrepareButtons ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".PrepareButtons ", Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Get texts in the current application language for all screen controls
    ''' </summary>
    ''' <remarks>
    ''' Created by: XB 05/03/2013
    ''' </remarks>
    Private Sub GetScreenLabels()
        Try
            Dim MLRD As New MultilanguageResourcesDelegate

            'For Labels, CheckBox, RadioButtons.....
            bsLISConfigurationLabel.Text = MLRD.GetResourceText(Nothing, "LIS_CONFIG_TITLE", LanguageID)
            ' WORK SESSION'S TAB
            SessionSettingsTab.Text = MLRD.GetResourceText(Nothing, "LBL_Worksession", LanguageID)
            ' Download
            BsDownloadGroupBox.Text = MLRD.GetResourceText(Nothing, "LIS_LBL_DOWNLOAD", LanguageID)
            BsHostQueryCheckbox.Text = MLRD.GetResourceText(Nothing, "LIS_LBL_HOSTQUERY", LanguageID)
            BsAcceptUnsolicitedOrdersCheckbox.Text = MLRD.GetResourceText(Nothing, "LIS_LBL_UNSOLICITED_ORDERS", LanguageID)
            BsAcceptOnRunningCheckbox.Text = MLRD.GetResourceText(Nothing, "LIS_LBL_DOWNLOAD_RUNNING", LanguageID)
            BsWSRerunsLabel.Text = MLRD.GetResourceText(Nothing, "LIS_LBL_MODE_RERUNS", LanguageID) & ":"
            BsWSReflexTestsLabel.Text = MLRD.GetResourceText(Nothing, "LIS_LBL_MODE_REFLEXTESTS", LanguageID) & ":"
            ' Upload
            BsUploadGroupBox.Text = MLRD.GetResourceText(Nothing, "LIS_LBL_UPLOAD", LanguageID)
            BsUploadUnsolicitedPatientResultsCheckbox.Text = MLRD.GetResourceText(Nothing, "LIS_LBL_UPLOAD_PAT", LanguageID)
            BsUploadUnsolicitedQCResultsCheckbox.Text = MLRD.GetResourceText(Nothing, "LIS_LBL_UPLOAD_QC", LanguageID)
            BsUploadResultsOnResetCheckbox.Text = MLRD.GetResourceText(Nothing, "LIS_LBL_UPLOAD_RESET", LanguageID)
            BsAutomaticCheckbox.Text = MLRD.GetResourceText(Nothing, "LIS_LBL_AUTOMATIC_UPLOAD", LanguageID)
            bsFrequencyLabel.Text = MLRD.GetResourceText(Nothing, "LBL_CFGANALYZER_EXPORTFREQ", LanguageID) & ":"

            'SG 11/07/2013
            BsAutoQueryStartCheckbox.Text = MLRD.GetResourceText(Nothing, "LIS_LBL_AUTOMATIC_QUERY_ON_START", LanguageID) '"*Automatic Query to LIS (before starting/continuing worksession)"
            BsMaxTimeToWaitLISOrdersLabel.Text = MLRD.GetResourceText(Nothing, "LIS_LBL_MAX_TIME_WAIT_LIS_ORDERS", LanguageID) & ":" ' *Maximum Time waiting for LIS Order[s]:

            ' COMMUNICATIONS TAB
            ' Comms
            CommunicationsSettingsTab.Text = MLRD.GetResourceText(Nothing, "LBL_CfgAnalyzer_CommSettings", LanguageID)
            BsLISCommsEnabledCheckbox.Text = MLRD.GetResourceText(Nothing, "LIS_LBL_COMMS_ENABLED", LanguageID)
            BsDataTransmissionLabel.Text = MLRD.GetResourceText(Nothing, "LIS_DATA_TRANS_TYPE", LanguageID) & ":"
            HostNameLabel.Text = MLRD.GetResourceText(Nothing, "LIS_LBL_HOSTNAME", LanguageID) & ":"

            ' XB 27/05/2013
            'TCPPortLabel.Text = MLRD.GetResourceText(Nothing, "LIS_LBL_TCP_Client", LanguageID) & ":"
            'TCPPort2Label.Text = MLRD.GetResourceText(Nothing, "LIS_LBL_TCP_Server", LanguageID) & ":"
            myPortLabel = MLRD.GetResourceText(Nothing, "LBL_CfgAnalyzer_Port", LanguageID)
            myPortClientLabel = MLRD.GetResourceText(Nothing, "LIS_LBL_TCP_Client", LanguageID)
            myPortServerLabel = MLRD.GetResourceText(Nothing, "LIS_LBL_TCP_Server", LanguageID)
            TCPPortLabel.Text = myPortClientLabel & ":"
            TCPPort2Label.Text = myPortServerLabel & ":"

            bsLISCommsGroupBox.Text = MLRD.GetResourceText(Nothing, "LIS_LBL_COMMS_SETTINGS", LanguageID)
            bsInternalGroupBox.Text = MLRD.GetResourceText(Nothing, "LIS_LBL_Internal_Settings", LanguageID)
            ' Internal
            BsMaxReceptionMsgsLabel.Text = MLRD.GetResourceText(Nothing, "LIS_LBL_MAX_REC_MSGS", LanguageID) & ":"
            BsMaxTransmissionMsgsLabel.Text = MLRD.GetResourceText(Nothing, "LIS_LBL_MAX_TRANS_MSGS", LanguageID) & ":"
            BsTimeoutQueueLabel.Text = MLRD.GetResourceText(Nothing, "LIS_LBL_MaxTimeToRespond", LanguageID) & ":"

            ' PROTOCOL'S TAB
            ProtocolSettingsTab.Text = MLRD.GetResourceText(Nothing, "LIS_LBL_PROTOCOL", LanguageID)
            bsLISProtocolGroupBox.Text = MLRD.GetResourceText(Nothing, "LIS_LBL_PROTOCOL", LanguageID) & " " & MLRD.GetResourceText(Nothing, "LBL_Settings", LanguageID)
            bsProtocolNameLabel.Text = MLRD.GetResourceText(Nothing, "LIS_LBL_PROTOCOL_NAME", LanguageID) & ":"
            bsIHECompliantCheckbox.Text = MLRD.GetResourceText(Nothing, "LIS_LBL_IHE", LanguageID)
            BsTransmissionCodePageLabel.Text = MLRD.GetResourceText(Nothing, "LIS_LBL_CODEPAGE", LanguageID) & ":"
            BsHostQueryPackageLabel.Text = MLRD.GetResourceText(Nothing, "LIS_LBL_HOSTQUERY_PACKAGE", LanguageID) & ":"
            BsmaxTimeWaitingForResponseLabel.Text = MLRD.GetResourceText(Nothing, "LIS_LBL_TimeForResponse", LanguageID) & ":"
            BsmaxTimeWaitACKLabel.Text = MLRD.GetResourceText(Nothing, "LIS_LBL_MaxTimeWaitACK", LanguageID) & ":"
            BsHostIDLabel.Text = MLRD.GetResourceText(Nothing, "LIS_LBL_HOST_ID", LanguageID) & ":"
            BsHostProviderLabel.Text = MLRD.GetResourceText(Nothing, "LIS_LBL_HOST_PROVIDER", LanguageID) & ":"
            BsInstrumentIDLabel.Text = MLRD.GetResourceText(Nothing, "LIS_LBL_INST_ID", LanguageID) & ":"
            BsInstrumentProviderLabel.Text = MLRD.GetResourceText(Nothing, "LIS_LBL_INST_PROVIDER", LanguageID) & ":"
            BsDelimitersGroupBox.Text = MLRD.GetResourceText(Nothing, "LIS_LBL_DELIMITERS", LanguageID)
            BsFieldLabel.Text = MLRD.GetResourceText(Nothing, "LIS_LBL_FIELD_DELIMITER", LanguageID) & ":"
            BsComponentLabel.Text = MLRD.GetResourceText(Nothing, "LIS_LBL_COMP_DELIMITER", LanguageID) & ":"
            BsRepeatLabel.Text = MLRD.GetResourceText(Nothing, "LIS_LBL_REP_DELIMITER", LanguageID) & ":"
            BsSpecialLabel.Text = MLRD.GetResourceText(Nothing, "LIS_LBL_SPEC_DELIMITER", LanguageID) & ":"
            BsSubcomponentLabel.Text = MLRD.GetResourceText(Nothing, "LIS_LBL_SUBC_DELIMITER", LanguageID) & ":"


            'For Tooltip
            bsScreenToolTips.SetToolTip(bsIHECompliantCheckbox, MLRD.GetResourceText(Nothing, "LIS_DESC_IHE", LanguageID))
            bsScreenToolTips.SetToolTip(BsHostQueryPackageNumericUpDown, MLRD.GetResourceText(Nothing, "LIS_DESC_HOSTQUERY_PACKAGE", LanguageID))
            bsScreenToolTips.SetToolTip(BsmaxTimeWaitingForResponseNumericUpDown, MLRD.GetResourceText(Nothing, "LIS_DESC_TimeForResponse", LanguageID))
            bsScreenToolTips.SetToolTip(BsmaxTimeWaitACKNumericUpDown, MLRD.GetResourceText(Nothing, "LIS_DESC_MaxTimeWaitACK", LanguageID))
            bsScreenToolTips.SetToolTip(BsLISCommsEnabledCheckbox, MLRD.GetResourceText(Nothing, "LIS_DESC_COMMS_ENABLED", LanguageID))
            bsScreenToolTips.SetToolTip(BsMaxReceptionMsgsNumericUpDown, MLRD.GetResourceText(Nothing, "LIS_DESC_MAX_REC_MSGS", LanguageID))
            bsScreenToolTips.SetToolTip(BsMaxTransmissionMsgsNumericUpDown, MLRD.GetResourceText(Nothing, "LIS_DESC_MAX_TRANS_MSGS", LanguageID))

            bsScreenToolTips.SetToolTip(bsAcceptButton, MLRD.GetResourceText(Nothing, "BTN_Save&Close", LanguageID))
            bsScreenToolTips.SetToolTip(bsCancelButton, MLRD.GetResourceText(Nothing, "BTN_Cancel&Close", LanguageID))

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".GetScreenLabels ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".GetScreenLabels ", Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Method incharge to fill the Upload To LIS Frequency ComboBox
    ''' </summary>
    ''' <remarks>
    ''' Created by:  XB 05/03/2013
    ''' </remarks>
    Private Sub FillFrequencyCombo()
        Try
            Dim myGlobalDataTO As New GlobalDataTO()
            Dim myPreloadedMasterDataDelegate As New PreloadedMasterDataDelegate

            myGlobalDataTO = myPreloadedMasterDataDelegate.GetList(Nothing, PreloadedMasterDataEnum.EXPORT_FREQUENCY)
            If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                Dim myPreMasterDataDS As New PreloadedMasterDataDS
                myPreMasterDataDS = CType(myGlobalDataTO.SetDatos, PreloadedMasterDataDS)

                'Sort results by Position
                Dim qPreloadedItems As List(Of PreloadedMasterDataDS.tfmwPreloadedMasterDataRow)
                qPreloadedItems = (From a In myPreMasterDataDS.tfmwPreloadedMasterData _
                                  Order By a.Position _
                                  Select a).ToList()

                bsFreqComboBox.DataSource = qPreloadedItems
                bsFreqComboBox.DisplayMember = "FixedItemDesc"
                bsFreqComboBox.ValueMember = "ItemID"
            Else
                'Error getting the list of values for Export Frequency, show it
                ShowMessage(Name & ".FillFrequencyCombo ", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage)
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".FillFrequencyCombo ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".FillFrequencyCombo ", Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Method incharge to fill the WS reruns mode ComboBox
    ''' </summary>
    ''' <remarks>
    ''' Created by:  XB 05/03/2013
    ''' </remarks>
    Private Sub FillWSRerunsCombo()
        Try
            Dim myGlobalDataTO As New GlobalDataTO()
            Dim myPreloadedMasterDataDelegate As New PreloadedMasterDataDelegate

            myGlobalDataTO = myPreloadedMasterDataDelegate.GetList(Nothing, PreloadedMasterDataEnum.WS_MODE_RERUNS)
            If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                Dim myPreMasterDataDS As New PreloadedMasterDataDS
                myPreMasterDataDS = CType(myGlobalDataTO.SetDatos, PreloadedMasterDataDS)

                'Sort results by Position
                Dim qPreloadedItems As List(Of PreloadedMasterDataDS.tfmwPreloadedMasterDataRow)
                qPreloadedItems = (From a In myPreMasterDataDS.tfmwPreloadedMasterData _
                                  Order By a.Position _
                                  Select a).ToList()

                BsWSRerunsComboBox.DataSource = qPreloadedItems
                BsWSRerunsComboBox.DisplayMember = "FixedItemDesc"
                BsWSRerunsComboBox.ValueMember = "ItemID"
            Else
                'Error getting the list of values for WS reruns mode, show it
                ShowMessage(Name & ".FillWSRerunsCombo ", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage)
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".FillWSRerunsCombo ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".FillWSRerunsCombo ", Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Method incharge to fill the Data transmission type ComboBox
    ''' </summary>
    ''' <remarks>
    ''' Created by:  XB 18/03/2013
    ''' </remarks>
    Private Sub FillDataTransmissionCombo()
        Try
            Dim myGlobalDataTO As New GlobalDataTO()
            Dim myPreloadedMasterDataDelegate As New PreloadedMasterDataDelegate


            If bsProtocolNameComboBox.SelectedValue = "HL7" Then
                myGlobalDataTO = myPreloadedMasterDataDelegate.GetList(Nothing, PreloadedMasterDataEnum.LIS_DATA_TRANS_HL7)
            Else
                myGlobalDataTO = myPreloadedMasterDataDelegate.GetList(Nothing, PreloadedMasterDataEnum.LIS_DATA_TRANS_ASTM)
            End If

            If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                Dim myPreMasterDataDS As New PreloadedMasterDataDS
                myPreMasterDataDS = CType(myGlobalDataTO.SetDatos, PreloadedMasterDataDS)

                'Sort results by Position
                Dim qPreloadedItems As List(Of PreloadedMasterDataDS.tfmwPreloadedMasterDataRow)
                qPreloadedItems = (From a In myPreMasterDataDS.tfmwPreloadedMasterData _
                                  Order By a.Position _
                                  Select a).ToList()

                BsDataTransmissionComboBox.DataSource = qPreloadedItems
                BsDataTransmissionComboBox.DisplayMember = "FixedItemDesc"
                BsDataTransmissionComboBox.ValueMember = "ItemID"
            Else
                'Error getting the list of values for WS reruns mode, show it
                ShowMessage(Name & ".FillDataTransmissionCombo ", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage)
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".FillDataTransmissionCombo ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".FillDataTransmissionCombo ", Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Method incharge to fill the Protocol Name ComboBox
    ''' </summary>
    ''' <remarks>
    ''' Created by:  XB 05/03/2013
    ''' </remarks>
    Private Sub FillProtocolNameCombo()
        Try
            Dim myGlobalDataTO As New GlobalDataTO()
            Dim myPreloadedMasterDataDelegate As New PreloadedMasterDataDelegate

            myGlobalDataTO = myPreloadedMasterDataDelegate.GetList(Nothing, PreloadedMasterDataEnum.LIS_PROTOCOL)
            If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                Dim myPreMasterDataDS As New PreloadedMasterDataDS
                myPreMasterDataDS = CType(myGlobalDataTO.SetDatos, PreloadedMasterDataDS)

                'Sort results by Position
                Dim qPreloadedItems As List(Of PreloadedMasterDataDS.tfmwPreloadedMasterDataRow)
                qPreloadedItems = (From a In myPreMasterDataDS.tfmwPreloadedMasterData _
                                  Order By a.Position _
                                  Select a).ToList()

                bsProtocolNameComboBox.DataSource = qPreloadedItems
                bsProtocolNameComboBox.DisplayMember = "FixedItemDesc"
                bsProtocolNameComboBox.ValueMember = "ItemID"
            Else
                'Error getting the list of values for Protocol Name, show it
                ShowMessage(Name & ".FillProtocolNameCombo ", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage)
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".FillProtocolNameCombo ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".FillProtocolNameCombo ", Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Method incharge to fill the Transmission CodePage ComboBox
    ''' </summary>
    ''' <remarks>
    ''' Created by:  XB 05/03/2013
    ''' </remarks>
    Private Sub FillTransmissionCodePageField()
        Try
            Dim CodePagexHL7 As String = "65001"    ' value by default
            Dim CodePagexASTM As String = "28591"   ' value by default

            If bsProtocolNameComboBox.SelectedValue = "HL7" Then
                BsTransmissionCodePageTextBox.Text = CodePagexHL7
            Else
                BsTransmissionCodePageTextBox.Text = CodePagexASTM
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".FillTransmissionCodePageField ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".FillTransmissionCodePageField ", Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Load current values of LIS Settings
    ''' </summary>
    ''' <remarks>
    ''' Created by  XB 05/03/2013
    ''' MOdified by XB 15/05/2013 - duplication of delimiters to distinct HL7 to ASTM
    ''' </remarks>
    Private Sub LoadLISDetails()
        Try
            Dim returnData As GlobalDataTO
            Dim myUserSettingDelegate As New UserSettingsDelegate

            ' WORK SESSION'S TAB
            ' Download
            returnData = myUserSettingDelegate.GetCurrentValueBySettingID(Nothing, UserSettingsEnum.LIS_HOST_QUERY.ToString())
            If (Not returnData.HasError AndAlso Not returnData.SetDatos Is Nothing) Then
                BsHostQueryCheckbox.Checked = CType(returnData.SetDatos, Boolean)
            Else
                'Error getting the Session Setting value, show it 
                ShowMessage(Name & ".LoadLISDetails ", returnData.ErrorCode, returnData.ErrorMessage)
            End If
            returnData = myUserSettingDelegate.GetCurrentValueBySettingID(Nothing, UserSettingsEnum.LIS_ACCEPT_UNSOLICITED.ToString())
            If (Not returnData.HasError AndAlso Not returnData.SetDatos Is Nothing) Then
                BsAcceptUnsolicitedOrdersCheckbox.Checked = CType(returnData.SetDatos, Boolean)
            Else
                'Error getting the Session Setting value, show it 
                ShowMessage(Name & ".LoadLISDetails ", returnData.ErrorCode, returnData.ErrorMessage)
            End If

            'SGM 11/07/2013
            returnData = myUserSettingDelegate.GetCurrentValueBySettingID(Nothing, UserSettingsEnum.AUTO_WS_WITH_LIS_MODE.ToString())
            If (Not returnData.HasError AndAlso Not returnData.SetDatos Is Nothing) Then
                BsAutoQueryStartCheckbox.Checked = CType(returnData.SetDatos, Boolean)
            Else
                'Error getting the Session Setting value, show it 
                ShowMessage(Name & ".LoadLISDetails ", returnData.ErrorCode, returnData.ErrorMessage)
            End If
            returnData = myUserSettingDelegate.GetCurrentValueBySettingID(Nothing, UserSettingsEnum.AUTO_LIS_WAIT_TIME.ToString())
            If (Not returnData.HasError AndAlso Not returnData.SetDatos Is Nothing) Then
                BsBsMaxTimeToWaitLISOrdersNumericUpDown.Value = CType(returnData.SetDatos, Integer)
            Else
                'Error getting the Session Setting value, show it 
                ShowMessage(Name & ".LoadLISDetails ", returnData.ErrorCode, returnData.ErrorMessage)
            End If
            'end SGM 11/07/2013

            returnData = myUserSettingDelegate.GetCurrentValueBySettingID(Nothing, UserSettingsEnum.LIS_DOWNLOAD_ONRUNNING.ToString())
            If (Not returnData.HasError AndAlso Not returnData.SetDatos Is Nothing) Then
                BsAcceptOnRunningCheckbox.Checked = CType(returnData.SetDatos, Boolean)
            Else
                'Error getting the Session Setting value, show it 
                ShowMessage(Name & ".LoadLISDetails ", returnData.ErrorCode, returnData.ErrorMessage)
            End If
            ' Upload
            returnData = myUserSettingDelegate.GetCurrentValueBySettingID(Nothing, UserSettingsEnum.LIS_UPLOAD_UNSOLICITED_PAT_RES.ToString())
            If (Not returnData.HasError AndAlso Not returnData.SetDatos Is Nothing) Then
                BsUploadUnsolicitedPatientResultsCheckbox.Checked = CType(returnData.SetDatos, Boolean)
            Else
                'Error getting the Session Setting value, show it 
                ShowMessage(Name & ".LoadLISDetails ", returnData.ErrorCode, returnData.ErrorMessage)
            End If
            returnData = myUserSettingDelegate.GetCurrentValueBySettingID(Nothing, UserSettingsEnum.LIS_UPLOAD_UNSOLICITED_QC_RES.ToString())
            If (Not returnData.HasError AndAlso Not returnData.SetDatos Is Nothing) Then
                BsUploadUnsolicitedQCResultsCheckbox.Checked = CType(returnData.SetDatos, Boolean)
            Else
                'Error getting the Session Setting value, show it 
                ShowMessage(Name & ".LoadLISDetails ", returnData.ErrorCode, returnData.ErrorMessage)
            End If
            'Indicate to if lis file is create on worksession reset.
            returnData = myUserSettingDelegate.GetCurrentValueBySettingID(Nothing, UserSettingsEnum.AUTOMATIC_RESET.ToString())
            If (Not returnData.HasError AndAlso Not returnData.SetDatos Is Nothing) Then
                BsUploadResultsOnResetCheckbox.Checked = CType(returnData.SetDatos, Boolean)
            Else
                'Error getting the Session Setting value, show it 
                ShowMessage(Name & ".LoadLISDetails ", returnData.ErrorCode, returnData.ErrorMessage)
            End If
            returnData = myUserSettingDelegate.GetCurrentValueBySettingID(Nothing, UserSettingsEnum.AUTOMATIC_EXPORT.ToString())
            If (Not returnData.HasError AndAlso Not returnData.SetDatos Is Nothing) Then
                BsAutomaticCheckbox.Checked = CType(returnData.SetDatos, Boolean)
            Else
                'Error getting the Session Setting value, show it 
                ShowMessage(Name & ".LoadLISDetails ", returnData.ErrorCode, returnData.ErrorMessage)
            End If
            returnData = myUserSettingDelegate.GetCurrentValueBySettingID(Nothing, UserSettingsEnum.EXPORT_FREQUENCY.ToString())
            If (Not returnData.HasError AndAlso Not returnData.SetDatos Is Nothing) Then
                bsFreqComboBox.SelectedValue = CType(returnData.SetDatos, String)
            Else
                'Error getting the Session Setting value, show it 
                ShowMessage(Name & ".LoadLISDetails ", returnData.ErrorCode, returnData.ErrorMessage)
            End If

            ' COMMUNICATIONS TAB

            ' NOTE : Load Protocol value (belowing to Protocol Tab) Is required before to load Data Transmission because both field have dependencies
            returnData = myUserSettingDelegate.GetCurrentValueBySettingID(Nothing, UserSettingsEnum.LIS_PROTOCOL_NAME.ToString())
            If (Not returnData.HasError AndAlso Not returnData.SetDatos Is Nothing) Then
                bsProtocolNameComboBox.SelectedValue = CType(returnData.SetDatos, String)
            Else
                'Error getting the Session Setting value, show it 
                ShowMessage(Name & ".LoadLISDetails ", returnData.ErrorCode, returnData.ErrorMessage)
            End If
            FillDataTransmissionCombo()

            returnData = myUserSettingDelegate.GetCurrentValueBySettingID(Nothing, UserSettingsEnum.LIS_DATA_TRANSMISSION_TYPE.ToString())
            If (Not returnData.HasError AndAlso Not returnData.SetDatos Is Nothing) Then
                BsDataTransmissionComboBox.SelectedValue = CType(returnData.SetDatos, String)
            Else
                'Error getting the Session Setting value, show it 
                ShowMessage(Name & ".LoadLISDetails ", returnData.ErrorCode, returnData.ErrorMessage)
            End If

            returnData = myUserSettingDelegate.GetCurrentValueBySettingID(Nothing, UserSettingsEnum.LIS_HOST_NAME.ToString())
            If (Not returnData.HasError AndAlso Not returnData.SetDatos Is Nothing) Then
                HostNameTextBox.Text = CType(returnData.SetDatos, String)
            Else
                'Error getting the Session Setting value, show it 
                ShowMessage(Name & ".LoadLISDetails ", returnData.ErrorCode, returnData.ErrorMessage)
            End If
            returnData = myUserSettingDelegate.GetCurrentValueBySettingID(Nothing, UserSettingsEnum.LIS_TCP_PORT.ToString())
            If (Not returnData.HasError AndAlso Not returnData.SetDatos Is Nothing) Then
                TCPPortNumericUpDown.Value = CType(returnData.SetDatos, Integer)
            Else
                'Error getting the Session Setting value, show it 
                ShowMessage(Name & ".LoadLISDetails ", returnData.ErrorCode, returnData.ErrorMessage)
            End If
            returnData = myUserSettingDelegate.GetCurrentValueBySettingID(Nothing, UserSettingsEnum.LIS_TCP_PORT2.ToString())
            If (Not returnData.HasError AndAlso Not returnData.SetDatos Is Nothing) Then
                TCPPort2NumericUpDown.Value = CType(returnData.SetDatos, Integer)
            Else
                'Error getting the Session Setting value, show it 
                ShowMessage(Name & ".LoadLISDetails ", returnData.ErrorCode, returnData.ErrorMessage)
            End If
            returnData = myUserSettingDelegate.GetCurrentValueBySettingID(Nothing, UserSettingsEnum.LIS_IHE_COMPLIANT.ToString())
            If (Not returnData.HasError AndAlso Not returnData.SetDatos Is Nothing) Then
                bsIHECompliantCheckbox.Checked = CType(returnData.SetDatos, Boolean)
            Else
                'Error getting the Session Setting value, show it 
                ShowMessage(Name & ".LoadLISDetails ", returnData.ErrorCode, returnData.ErrorMessage)
            End If
            returnData = myUserSettingDelegate.GetCurrentValueBySettingID(Nothing, UserSettingsEnum.LIS_ENABLE_COMMS.ToString())
            If (Not returnData.HasError AndAlso Not returnData.SetDatos Is Nothing) Then
                BsLISCommsEnabledCheckbox.Checked = CType(returnData.SetDatos, Boolean)
            Else
                'Error getting the Session Setting value, show it 
                ShowMessage(Name & ".LoadLISDetails ", returnData.ErrorCode, returnData.ErrorMessage)
            End If
            returnData = myUserSettingDelegate.GetCurrentValueBySettingID(Nothing, UserSettingsEnum.LIS_WORKING_MODE_RERUNS.ToString())
            If (Not returnData.HasError AndAlso Not returnData.SetDatos Is Nothing) Then
                BsWSRerunsComboBox.SelectedValue = CType(returnData.SetDatos, String)
            Else
                'Error getting the Session Setting value, show it 
                ShowMessage(Name & ".LoadLISDetails ", returnData.ErrorCode, returnData.ErrorMessage)
            End If
            ' Meeting EF+XB 05/03/2013 : NO V2 by now
            'returnData = myUserSettingDelegate.GetCurrentValueBySettingID(Nothing, UserSettingsEnum.LIS_WORKING_MODE_REFLEX_TESTS.ToString())
            'If (Not returnData.HasError AndAlso Not returnData.SetDatos Is Nothing) Then
            '    BsWSReflexTestsComboBox.SelectedValue = CType(returnData.SetDatos, String)
            'Else
            '    'Error getting the Session Setting value, show it 
            '    ShowMessage(Name & ".LoadLISDetails ", returnData.ErrorCode, returnData.ErrorMessage)
            'End If
            ' Storage reception
            returnData = myUserSettingDelegate.GetCurrentValueBySettingID(Nothing, UserSettingsEnum.LIS_STORAGE_RECEPTION_MAX_MSG.ToString())
            If (Not returnData.HasError AndAlso Not returnData.SetDatos Is Nothing) Then
                BsMaxReceptionMsgsNumericUpDown.Value = CType(returnData.SetDatos, Integer)
                MaxReceptionInitialValue = BsMaxReceptionMsgsNumericUpDown.Value
            Else
                'Error getting the Session Setting value, show it 
                ShowMessage(Name & ".LoadLISDetails ", returnData.ErrorCode, returnData.ErrorMessage)
            End If
            ' Storage transmission
            returnData = myUserSettingDelegate.GetCurrentValueBySettingID(Nothing, UserSettingsEnum.LIS_STORAGE_TRANS_MAX_MSG.ToString())
            If (Not returnData.HasError AndAlso Not returnData.SetDatos Is Nothing) Then
                BsMaxTransmissionMsgsNumericUpDown.Value = CType(returnData.SetDatos, Integer)
                MaxTransmissionInitialValue = BsMaxTransmissionMsgsNumericUpDown.Value
            Else
                'Error getting the Session Setting value, show it 
                ShowMessage(Name & ".LoadLISDetails ", returnData.ErrorCode, returnData.ErrorMessage)
            End If
            returnData = myUserSettingDelegate.GetCurrentValueBySettingID(Nothing, UserSettingsEnum.LIS_BAx00maxTimeToRespond.ToString())
            If (Not returnData.HasError AndAlso Not returnData.SetDatos Is Nothing) Then
                BsMaxTimeToRespondNumericUpDown.Value = CType(returnData.SetDatos, Integer)
            Else
                'Error getting the Session Setting value, show it 
                ShowMessage(Name & ".LoadLISDetails ", returnData.ErrorCode, returnData.ErrorMessage)
            End If

            'PROTOCOL() 'S TAB
            returnData = myUserSettingDelegate.GetCurrentValueBySettingID(Nothing, UserSettingsEnum.LIS_CODEPAGE.ToString())
            If (Not returnData.HasError AndAlso Not returnData.SetDatos Is Nothing) Then
                BsTransmissionCodePageTextBox.Text = CType(returnData.SetDatos, String)
            Else
                'Error getting the Session Setting value, show it 
                ShowMessage(Name & ".LoadLISDetails ", returnData.ErrorCode, returnData.ErrorMessage)
            End If
            returnData = myUserSettingDelegate.GetCurrentValueBySettingID(Nothing, UserSettingsEnum.LIS_HOST_QUERY_PACKAGE.ToString())
            If (Not returnData.HasError AndAlso Not returnData.SetDatos Is Nothing) Then
                BsHostQueryPackageNumericUpDown.Value = CType(returnData.SetDatos, Integer)
            Else
                'Error getting the Session Setting value, show it 
                ShowMessage(Name & ".LoadLISDetails ", returnData.ErrorCode, returnData.ErrorMessage)
            End If
            returnData = myUserSettingDelegate.GetCurrentValueBySettingID(Nothing, UserSettingsEnum.LIS_maxTimeWaitingForResponse.ToString())
            If (Not returnData.HasError AndAlso Not returnData.SetDatos Is Nothing) Then
                BsmaxTimeWaitingForResponseNumericUpDown.Value = CType(returnData.SetDatos, Integer)
            Else
                'Error getting the Session Setting value, show it 
                ShowMessage(Name & ".LoadLISDetails ", returnData.ErrorCode, returnData.ErrorMessage)
            End If
            returnData = myUserSettingDelegate.GetCurrentValueBySettingID(Nothing, UserSettingsEnum.LIS_maxTimeWaitingForACK.ToString())
            If (Not returnData.HasError AndAlso Not returnData.SetDatos Is Nothing) Then
                BsmaxTimeWaitACKNumericUpDown.Value = CType(returnData.SetDatos, Integer)
            Else
                'Error getting the Session Setting value, show it 
                ShowMessage(Name & ".LoadLISDetails ", returnData.ErrorCode, returnData.ErrorMessage)
            End If

            returnData = myUserSettingDelegate.GetCurrentValueBySettingID(Nothing, UserSettingsEnum.LIS_HOST_ID.ToString())
            If (Not returnData.HasError AndAlso Not returnData.SetDatos Is Nothing) Then
                BsHostIDTextBox.Text = CType(returnData.SetDatos, String)
            Else
                'Error getting the Session Setting value, show it 
                ShowMessage(Name & ".LoadLISDetails ", returnData.ErrorCode, returnData.ErrorMessage)
            End If
            returnData = myUserSettingDelegate.GetCurrentValueBySettingID(Nothing, UserSettingsEnum.LIS_HOST_PROVIDER.ToString())
            If (Not returnData.HasError AndAlso Not returnData.SetDatos Is Nothing) Then
                BsHostProviderTextBox.Text = CType(returnData.SetDatos, String)
            Else
                'Error getting the Session Setting value, show it 
                ShowMessage(Name & ".LoadLISDetails ", returnData.ErrorCode, returnData.ErrorMessage)
            End If
            returnData = myUserSettingDelegate.GetCurrentValueBySettingID(Nothing, UserSettingsEnum.LIS_INSTRUMENT_ID.ToString())
            If (Not returnData.HasError AndAlso Not returnData.SetDatos Is Nothing) Then
                BsInstrumentIDTextBox.Text = CType(returnData.SetDatos, String)
            Else
                'Error getting the Session Setting value, show it 
                ShowMessage(Name & ".LoadLISDetails ", returnData.ErrorCode, returnData.ErrorMessage)
            End If
            returnData = myUserSettingDelegate.GetCurrentValueBySettingID(Nothing, UserSettingsEnum.LIS_INSTRUMENT_PROVIDER.ToString())
            If (Not returnData.HasError AndAlso Not returnData.SetDatos Is Nothing) Then
                BsInstrumentProviderTextBox.Text = CType(returnData.SetDatos, String)
            Else
                'Error getting the Session Setting value, show it 
                ShowMessage(Name & ".LoadLISDetails ", returnData.ErrorCode, returnData.ErrorMessage)
            End If

            ' Delimiters
            returnData = myUserSettingDelegate.GetCurrentValueBySettingID(Nothing, UserSettingsEnum.LIS_FIELD_SEPARATOR_HL7.ToString())
            If (Not returnData.HasError AndAlso Not returnData.SetDatos Is Nothing) Then
                myInitialValueField_Sep_HL7 = CType(returnData.SetDatos, String)
            Else
                'Error getting the Session Setting value, show it 
                ShowMessage(Name & ".LoadLISDetails ", returnData.ErrorCode, returnData.ErrorMessage)
            End If
            returnData = myUserSettingDelegate.GetCurrentValueBySettingID(Nothing, UserSettingsEnum.LIS_COMP_SEPARATOR_HL7.ToString())
            If (Not returnData.HasError AndAlso Not returnData.SetDatos Is Nothing) Then
                myInitialValueComp_Sep_HL7 = CType(returnData.SetDatos, String)
            Else
                'Error getting the Session Setting value, show it 
                ShowMessage(Name & ".LoadLISDetails ", returnData.ErrorCode, returnData.ErrorMessage)
            End If
            returnData = myUserSettingDelegate.GetCurrentValueBySettingID(Nothing, UserSettingsEnum.LIS_REP_SEPARATOR_HL7.ToString())
            If (Not returnData.HasError AndAlso Not returnData.SetDatos Is Nothing) Then
                myInitialValueRep_Sep_HL7 = CType(returnData.SetDatos, String)
            Else
                'Error getting the Session Setting value, show it 
                ShowMessage(Name & ".LoadLISDetails ", returnData.ErrorCode, returnData.ErrorMessage)
            End If
            returnData = myUserSettingDelegate.GetCurrentValueBySettingID(Nothing, UserSettingsEnum.LIS_SPEC_SEPARATOR_HL7.ToString())
            If (Not returnData.HasError AndAlso Not returnData.SetDatos Is Nothing) Then
                myInitialValueSpec_Sep_HL7 = CType(returnData.SetDatos, String)
            Else
                'Error getting the Session Setting value, show it 
                ShowMessage(Name & ".LoadLISDetails ", returnData.ErrorCode, returnData.ErrorMessage)
            End If
            returnData = myUserSettingDelegate.GetCurrentValueBySettingID(Nothing, UserSettingsEnum.LIS_SUBC_SEPARATOR_HL7.ToString())
            If (Not returnData.HasError AndAlso Not returnData.SetDatos Is Nothing) Then
                myInitialValueSubComp_Sep_HL7 = CType(returnData.SetDatos, String)
            Else
                'Error getting the Session Setting value, show it 
                ShowMessage(Name & ".LoadLISDetails ", returnData.ErrorCode, returnData.ErrorMessage)
            End If
            returnData = myUserSettingDelegate.GetCurrentValueBySettingID(Nothing, UserSettingsEnum.LIS_FIELD_SEPARATOR_ASTM.ToString())
            If (Not returnData.HasError AndAlso Not returnData.SetDatos Is Nothing) Then
                myInitialValueField_Sep_ASTM = CType(returnData.SetDatos, String)
            Else
                'Error getting the Session Setting value, show it 
                ShowMessage(Name & ".LoadLISDetails ", returnData.ErrorCode, returnData.ErrorMessage)
            End If
            returnData = myUserSettingDelegate.GetCurrentValueBySettingID(Nothing, UserSettingsEnum.LIS_COMP_SEPARATOR_ASTM.ToString())
            If (Not returnData.HasError AndAlso Not returnData.SetDatos Is Nothing) Then
                myInitialValueComp_Sep_ASTM = CType(returnData.SetDatos, String)
            Else
                'Error getting the Session Setting value, show it 
                ShowMessage(Name & ".LoadLISDetails ", returnData.ErrorCode, returnData.ErrorMessage)
            End If
            returnData = myUserSettingDelegate.GetCurrentValueBySettingID(Nothing, UserSettingsEnum.LIS_REP_SEPARATOR_ASTM.ToString())
            If (Not returnData.HasError AndAlso Not returnData.SetDatos Is Nothing) Then
                myInitialValueRep_Sep_ASTM = CType(returnData.SetDatos, String)
            Else
                'Error getting the Session Setting value, show it 
                ShowMessage(Name & ".LoadLISDetails ", returnData.ErrorCode, returnData.ErrorMessage)
            End If
            returnData = myUserSettingDelegate.GetCurrentValueBySettingID(Nothing, UserSettingsEnum.LIS_SPEC_SEPARATOR_ASTM.ToString())
            If (Not returnData.HasError AndAlso Not returnData.SetDatos Is Nothing) Then
                myInitialValueSpec_Sep_ASTM = CType(returnData.SetDatos, String)
            Else
                'Error getting the Session Setting value, show it 
                ShowMessage(Name & ".LoadLISDetails ", returnData.ErrorCode, returnData.ErrorMessage)
            End If

            If bsProtocolNameComboBox.SelectedValue = "HL7" Then
                ' HL7
                BsFieldSeparatorTextBox.Text = myInitialValueField_Sep_HL7
                BsComponentSeparatorTextBox.Text = myInitialValueComp_Sep_HL7
                BsRepeatSeparatorTextBox.Text = myInitialValueRep_Sep_HL7
                BsSpecialSeparatorTextBox.Text = myInitialValueSpec_Sep_HL7
                BsSubComponentSeparatorTextBox.Text = myInitialValueSubComp_Sep_HL7
            Else
                ' ASTM
                BsFieldSeparatorTextBox.Text = myInitialValueField_Sep_ASTM
                BsComponentSeparatorTextBox.Text = myInitialValueComp_Sep_ASTM
                BsRepeatSeparatorTextBox.Text = myInitialValueRep_Sep_ASTM
                BsSpecialSeparatorTextBox.Text = myInitialValueSpec_Sep_ASTM
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".LoadLISDetails ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".LoadLISDetails ", Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Method that validate all the required value on all the tabs
    ''' </summary>
    ''' <remarks>
    ''' Created by XB 19/03/2013
    ''' </remarks>
    Private Sub ValidateAllTabs(Optional ByVal pControl As String = "")
        Try
            ValidationError = False
            bsErrorProvider1.Clear()

            If EditionMode Then
                If BsLISCommsEnabledCheckbox.Checked Then
                    ' WORKSESSION TAB   *************************************************************************************************************
                    ' Nothing to validate

                    ' COMMUNICATIONS TAB    *********************************************************************************************************
                    If Not TCPPortNumericUpDown.Text.Length > 0 Then
                        bsErrorProvider1.SetError(TCPPortNumericUpDown, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString))
                        ValidationError = True
                    End If

                    If Not BsMaxReceptionMsgsNumericUpDown.Text.Length > 0 Then
                        bsErrorProvider1.SetError(BsMaxReceptionMsgsNumericUpDown, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString))
                        ValidationError = True
                    ElseIf IsNumeric(BsMaxReceptionMsgsNumericUpDown.Text.ToString) Then
                        Try
                            If MaxReceptionInitialValue < CLng(BsMaxReceptionMsgsNumericUpDown.Text) Then
                                ' User tries to reduce storage size
                                If Not mdiESWrapperCopy.Storage Is Nothing Then
                                    Select Case mdiESWrapperCopy.Storage
                                        Case "0"
                                        Case Else
                                            ' But if storage is close to full or overloaded, is not able to reduce the storage size !
                                            bsErrorProvider1.SetError(BsMaxReceptionMsgsNumericUpDown, GetMessageText(GlobalEnumerates.Messages.LIS_STORAGE_LIMIT.ToString))
                                            ValidationError = True
                                    End Select
                                End If
                            End If
                        Catch ex As Exception
                            bsErrorProvider1.SetError(BsMaxReceptionMsgsNumericUpDown, GetMessageText(GlobalEnumerates.Messages.LIS_STORAGE_LIMIT.ToString))
                            ValidationError = True
                        End Try
                    End If

                    If Not BsMaxTransmissionMsgsNumericUpDown.Text.Length > 0 Then
                        bsErrorProvider1.SetError(BsMaxTransmissionMsgsNumericUpDown, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString))
                        ValidationError = True
                    ElseIf IsNumeric(BsMaxTransmissionMsgsNumericUpDown.Text.ToString) Then
                        Try
                            If MaxTransmissionInitialValue < CLng(BsMaxTransmissionMsgsNumericUpDown.Text) Then
                                ' User tries to reduce storage size
                                If Not mdiESWrapperCopy.Storage Is Nothing Then
                                    Select Case mdiESWrapperCopy.Storage
                                        Case "0"
                                        Case Else
                                            ' But if storage is close to full or overloaded, is not able to reduce the storage size !
                                            bsErrorProvider1.SetError(BsMaxTransmissionMsgsNumericUpDown, GetMessageText(GlobalEnumerates.Messages.LIS_STORAGE_LIMIT.ToString))
                                            ValidationError = True
                                    End Select
                                End If
                            End If
                        Catch ex As Exception
                            bsErrorProvider1.SetError(BsMaxReceptionMsgsNumericUpDown, GetMessageText(GlobalEnumerates.Messages.LIS_STORAGE_LIMIT.ToString))
                            ValidationError = True
                        End Try
                    End If

                    If Not BsMaxTimeToRespondNumericUpDown.Text.Length > 0 Then
                        bsErrorProvider1.SetError(BsMaxTimeToRespondNumericUpDown, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString))
                        ValidationError = True
                    End If

                    Select Case BsDataTransmissionComboBox.SelectedValue
                        Case "TCPIP-Client"
                            If Not HostNameTextBox.Text.Length > 0 Then
                                bsErrorProvider1.SetError(HostNameTextBox, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString))
                                ValidationError = True
                            End If
                        Case "TCPIP-Server"
                            ' Nothing additional to validate

                        Case "TCPIP-Trans"
                            If Not HostNameTextBox.Text.Length > 0 Then
                                bsErrorProvider1.SetError(HostNameTextBox, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString))
                                ValidationError = True
                            End If

                            If Not TCPPort2NumericUpDown.Text.Length > 0 Then
                                bsErrorProvider1.SetError(TCPPort2NumericUpDown, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString))
                                ValidationError = True
                            End If
                    End Select

                    ' PROTOCOL TAB  *****************************************************************************************************************

                    If Not BsTransmissionCodePageTextBox.Text.Length > 0 Then
                        bsErrorProvider1.SetError(BsTransmissionCodePageTextBox, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString))
                        ValidationError = True
                    Else
                        ' check a valid CodePage
                        Try
                            Dim codePageString As String = BsTransmissionCodePageTextBox.Text
                            Dim cp As Integer = CInt(codePageString)
                            Dim enc As Encoding = Encoding.GetEncoding(cp)

                            Debug.WriteLine(String.Format("codePageString = '{0}' BodyName = '{1}' CodePage = {2} EncodingName = '{3}' HeaderName = '{4}' WindowsCodePage = {5}", _
                                                          codePageString, enc.BodyName, enc.CodePage, enc.EncodingName, enc.HeaderName, enc.WindowsCodePage))

                        Catch ex As Exception
                            bsErrorProvider1.SetError(BsTransmissionCodePageTextBox, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString))
                            ValidationError = True
                        End Try
                    End If

                    If Not BsHostQueryPackageNumericUpDown.Text.Length > 0 Then
                        bsErrorProvider1.SetError(BsHostQueryPackageNumericUpDown, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString))
                        ValidationError = True
                    End If
                    If Not BsmaxTimeWaitingForResponseNumericUpDown.Text.Length > 0 Then
                        bsErrorProvider1.SetError(BsmaxTimeWaitingForResponseNumericUpDown, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString))
                        ValidationError = True
                    End If
                    If Not BsmaxTimeWaitACKNumericUpDown.Text.Length > 0 Then
                        bsErrorProvider1.SetError(BsmaxTimeWaitACKNumericUpDown, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString))
                        ValidationError = True
                    End If

                    If Not BsHostIDTextBox.Text.Length > 0 Then
                        bsErrorProvider1.SetError(BsHostIDTextBox, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString))
                        ValidationError = True
                    End If
                    If Not BsHostProviderTextBox.Text.Length > 0 Then
                        bsErrorProvider1.SetError(BsHostProviderTextBox, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString))
                        ValidationError = True
                    End If
                    If Not BsInstrumentIDTextBox.Text.Length > 0 Then
                        bsErrorProvider1.SetError(BsInstrumentIDTextBox, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString))
                        ValidationError = True
                    End If
                    If Not BsInstrumentProviderTextBox.Text.Length > 0 Then
                        bsErrorProvider1.SetError(BsInstrumentProviderTextBox, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString))
                        ValidationError = True
                    End If

                    If Not BsFieldSeparatorTextBox.Text.Length > 0 Then
                        bsErrorProvider1.SetError(BsFieldSeparatorTextBox, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString))
                        ValidationError = True
                    End If
                    If Not BsComponentSeparatorTextBox.Text.Length > 0 Then
                        bsErrorProvider1.SetError(BsComponentSeparatorTextBox, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString))
                        ValidationError = True
                    End If
                    If Not BsRepeatSeparatorTextBox.Text.Length > 0 Then
                        bsErrorProvider1.SetError(BsRepeatSeparatorTextBox, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString))
                        ValidationError = True
                    End If
                    If Not BsSpecialSeparatorTextBox.Text.Length > 0 Then
                        bsErrorProvider1.SetError(BsSpecialSeparatorTextBox, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString))
                        ValidationError = True
                    End If
                    If bsProtocolNameComboBox.SelectedValue = "HL7" Then
                        If Not BsSubComponentSeparatorTextBox.Text.Length > 0 Then
                            bsErrorProvider1.SetError(BsSubComponentSeparatorTextBox, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString))
                            ValidationError = True
                        End If
                    End If

                End If
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " ValidateAllTabs ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))") 'AG 07/07/2010  "SYSTEM_ERROR", ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Save all values that have been set for the LIS Settings
    ''' </summary>
    ''' <remarks>
    ''' Created by:  XB 05/03/2013
    ''' Modified by: XB 15/05/2013 - Changes in settings in Protocol Tab: settings for message field delimiters have been duplicated to allow distinct between 
    '''                              the ones used for HL7 and the ones used for ASTM
    '''              SG 11/07/2013 - Added new settings AUTO_WS_WITH_LIS_MODE and AUTO_LIS_WAIT_TIME in WorkSession Tab. Inform MDI property 
    '''                              autoWSCreationWithLISMode with value of setting AUTO_WS_WITH_LIS_MODE
    '''              AG 15/07/2013 - Added code to re-program the timer in MDI (property LISWaitTime) when required 
    '''                              (depending on value of setting AUTO_LIS_WAIT_TIME)
    '''              SA 21/10/2013 - BT #1349 ==> Value to assign to Main MDI Property autoWSCreationWithLISMode will depend on values of two settings: 
    '''                              AUTO_WS_WITH_LIS_MODE and LIS_ENABLE_COMMS. 
    ''' </remarks>
    Private Sub SaveChanges()
        Try
            'Validate data in all the screen tabs
            ValidateAllTabs()

            If (Not ValidationError) Then
                Cursor = Cursors.WaitCursor

                'Load in a DS the current value of the Session Settings
                Dim sessionSettings As New UserSettingDS()
                Dim sessionSettingRow As UserSettingDS.tcfgUserSettingsRow

                'WORK SESSION TAB
                '** Settings for DOWNLOAD
                sessionSettingRow = sessionSettings.tcfgUserSettings.NewtcfgUserSettingsRow
                sessionSettingRow.SettingID = UserSettingsEnum.LIS_HOST_QUERY.ToString()
                sessionSettingRow.CurrentValue = IIf(BsHostQueryCheckbox.Checked, "1", "0").ToString()
                sessionSettings.tcfgUserSettings.Rows.Add(sessionSettingRow)

                sessionSettingRow = sessionSettings.tcfgUserSettings.NewtcfgUserSettingsRow
                sessionSettingRow.SettingID = UserSettingsEnum.LIS_ACCEPT_UNSOLICITED.ToString()
                sessionSettingRow.CurrentValue = IIf(BsAcceptUnsolicitedOrdersCheckbox.Checked, "1", "0").ToString()
                sessionSettings.tcfgUserSettings.Rows.Add(sessionSettingRow)

                sessionSettingRow = sessionSettings.tcfgUserSettings.NewtcfgUserSettingsRow
                sessionSettingRow.SettingID = UserSettingsEnum.LIS_DOWNLOAD_ONRUNNING.ToString()
                sessionSettingRow.CurrentValue = IIf(BsAcceptOnRunningCheckbox.Checked, "1", "0").ToString()
                sessionSettings.tcfgUserSettings.Rows.Add(sessionSettingRow)

                sessionSettingRow = sessionSettings.tcfgUserSettings.NewtcfgUserSettingsRow
                sessionSettingRow.SettingID = UserSettingsEnum.LIS_WORKING_MODE_RERUNS.ToString()
                sessionSettingRow.CurrentValue = BsWSRerunsComboBox.SelectedValue.ToString()
                sessionSettings.tcfgUserSettings.Rows.Add(sessionSettingRow)

                'Meeting EF+XB 05/03/2013 : NO V2 by now
                'sessionSettingRow = sessionSettings.tcfgUserSettings.NewtcfgUserSettingsRow
                'sessionSettingRow.SettingID = UserSettingsEnum.LIS_WORKING_MODE_REFLEX_TESTS.ToString()
                'sessionSettingRow.CurrentValue = BsWSReflexTestsComboBox.SelectedValue.ToString()
                'sessionSettings.tcfgUserSettings.Rows.Add(sessionSettingRow)

                'SG 11/07/2013 - New settings AUTO_WS_WITH_LIS_MODE, AUTO_LIS_WAIT_TIME
                sessionSettingRow = sessionSettings.tcfgUserSettings.NewtcfgUserSettingsRow
                sessionSettingRow.SettingID = UserSettingsEnum.AUTO_WS_WITH_LIS_MODE.ToString()
                sessionSettingRow.CurrentValue = IIf(BsAutoQueryStartCheckbox.Checked, "1", "0").ToString()
                sessionSettings.tcfgUserSettings.Rows.Add(sessionSettingRow)

                sessionSettingRow = sessionSettings.tcfgUserSettings.NewtcfgUserSettingsRow
                sessionSettingRow.SettingID = UserSettingsEnum.AUTO_LIS_WAIT_TIME.ToString()
                sessionSettingRow.CurrentValue = BsBsMaxTimeToWaitLISOrdersNumericUpDown.Value.ToString()
                sessionSettings.tcfgUserSettings.Rows.Add(sessionSettingRow)

                'Reprogram the Main MDI timer when it is needed
                IAx00MainMDI.LISWaitTime = CInt(BsBsMaxTimeToWaitLISOrdersNumericUpDown.Value)

                '** Settings for UPLOAD
                sessionSettingRow = sessionSettings.tcfgUserSettings.NewtcfgUserSettingsRow
                sessionSettingRow.SettingID = UserSettingsEnum.LIS_UPLOAD_UNSOLICITED_PAT_RES.ToString()
                sessionSettingRow.CurrentValue = IIf(BsUploadUnsolicitedPatientResultsCheckbox.Checked, "1", "0").ToString()
                sessionSettings.tcfgUserSettings.Rows.Add(sessionSettingRow)

                sessionSettingRow = sessionSettings.tcfgUserSettings.NewtcfgUserSettingsRow
                sessionSettingRow.SettingID = UserSettingsEnum.LIS_UPLOAD_UNSOLICITED_QC_RES.ToString()
                sessionSettingRow.CurrentValue = IIf(BsUploadUnsolicitedQCResultsCheckbox.Checked, "1", "0").ToString()
                sessionSettings.tcfgUserSettings.Rows.Add(sessionSettingRow)

                sessionSettingRow = sessionSettings.tcfgUserSettings.NewtcfgUserSettingsRow
                sessionSettingRow.SettingID = UserSettingsEnum.AUTOMATIC_RESET.ToString()
                sessionSettingRow.CurrentValue = IIf(BsUploadResultsOnResetCheckbox.Checked, "1", "0").ToString()
                sessionSettings.tcfgUserSettings.Rows.Add(sessionSettingRow)

                sessionSettingRow = sessionSettings.tcfgUserSettings.NewtcfgUserSettingsRow
                sessionSettingRow.SettingID = UserSettingsEnum.AUTOMATIC_EXPORT.ToString()
                sessionSettingRow.CurrentValue = IIf(BsAutomaticCheckbox.Checked, "1", "0").ToString()
                sessionSettings.tcfgUserSettings.Rows.Add(sessionSettingRow)

                If (BsAutomaticCheckbox.Checked) Then
                    sessionSettingRow = sessionSettings.tcfgUserSettings.NewtcfgUserSettingsRow
                    sessionSettingRow.SettingID = UserSettingsEnum.EXPORT_FREQUENCY.ToString()
                    sessionSettingRow.CurrentValue = bsFreqComboBox.SelectedValue.ToString()
                    sessionSettings.tcfgUserSettings.Rows.Add(sessionSettingRow)
                End If

                'COMMUNICATIONS TAB
                sessionSettingRow = sessionSettings.tcfgUserSettings.NewtcfgUserSettingsRow
                sessionSettingRow.SettingID = UserSettingsEnum.LIS_ENABLE_COMMS.ToString()
                sessionSettingRow.CurrentValue = IIf(BsLISCommsEnabledCheckbox.Checked, "1", "0").ToString()
                sessionSettings.tcfgUserSettings.Rows.Add(sessionSettingRow)

                sessionSettingRow = sessionSettings.tcfgUserSettings.NewtcfgUserSettingsRow
                sessionSettingRow.SettingID = UserSettingsEnum.LIS_DATA_TRANSMISSION_TYPE.ToString()
                sessionSettingRow.CurrentValue = BsDataTransmissionComboBox.SelectedValue.ToString()
                sessionSettings.tcfgUserSettings.Rows.Add(sessionSettingRow)

                sessionSettingRow = sessionSettings.tcfgUserSettings.NewtcfgUserSettingsRow
                sessionSettingRow.SettingID = UserSettingsEnum.LIS_HOST_NAME.ToString()
                sessionSettingRow.CurrentValue = HostNameTextBox.Text.ToString()
                sessionSettings.tcfgUserSettings.Rows.Add(sessionSettingRow)

                sessionSettingRow = sessionSettings.tcfgUserSettings.NewtcfgUserSettingsRow
                sessionSettingRow.SettingID = UserSettingsEnum.LIS_TCP_PORT.ToString()
                sessionSettingRow.CurrentValue = TCPPortNumericUpDown.Value.ToString()
                sessionSettings.tcfgUserSettings.Rows.Add(sessionSettingRow)

                sessionSettingRow = sessionSettings.tcfgUserSettings.NewtcfgUserSettingsRow
                sessionSettingRow.SettingID = UserSettingsEnum.LIS_TCP_PORT2.ToString()
                sessionSettingRow.CurrentValue = TCPPort2NumericUpDown.Value.ToString()
                sessionSettings.tcfgUserSettings.Rows.Add(sessionSettingRow)

                sessionSettingRow = sessionSettings.tcfgUserSettings.NewtcfgUserSettingsRow
                sessionSettingRow.SettingID = UserSettingsEnum.LIS_BAx00maxTimeToRespond.ToString()
                sessionSettingRow.CurrentValue = BsMaxTimeToRespondNumericUpDown.Value.ToString()
                sessionSettings.tcfgUserSettings.Rows.Add(sessionSettingRow)

                sessionSettingRow = sessionSettings.tcfgUserSettings.NewtcfgUserSettingsRow
                sessionSettingRow.SettingID = UserSettingsEnum.LIS_STORAGE_RECEPTION_MAX_MSG.ToString()
                sessionSettingRow.CurrentValue = BsMaxReceptionMsgsNumericUpDown.Value.ToString()
                sessionSettings.tcfgUserSettings.Rows.Add(sessionSettingRow)

                sessionSettingRow = sessionSettings.tcfgUserSettings.NewtcfgUserSettingsRow
                sessionSettingRow.SettingID = UserSettingsEnum.LIS_STORAGE_TRANS_MAX_MSG.ToString()
                sessionSettingRow.CurrentValue = BsMaxTransmissionMsgsNumericUpDown.Value.ToString()
                sessionSettings.tcfgUserSettings.Rows.Add(sessionSettingRow)

                'PROTOCOL TAB
                sessionSettingRow = sessionSettings.tcfgUserSettings.NewtcfgUserSettingsRow
                sessionSettingRow.SettingID = UserSettingsEnum.LIS_PROTOCOL_NAME.ToString()
                sessionSettingRow.CurrentValue = bsProtocolNameComboBox.SelectedValue.ToString()
                sessionSettings.tcfgUserSettings.Rows.Add(sessionSettingRow)

                sessionSettingRow = sessionSettings.tcfgUserSettings.NewtcfgUserSettingsRow
                sessionSettingRow.SettingID = UserSettingsEnum.LIS_IHE_COMPLIANT.ToString()
                sessionSettingRow.CurrentValue = IIf(bsIHECompliantCheckbox.Checked, "1", "0").ToString()
                sessionSettings.tcfgUserSettings.Rows.Add(sessionSettingRow)

                sessionSettingRow = sessionSettings.tcfgUserSettings.NewtcfgUserSettingsRow
                sessionSettingRow.SettingID = UserSettingsEnum.LIS_CODEPAGE.ToString()
                sessionSettingRow.CurrentValue = BsTransmissionCodePageTextBox.Text.ToString()
                sessionSettings.tcfgUserSettings.Rows.Add(sessionSettingRow)

                sessionSettingRow = sessionSettings.tcfgUserSettings.NewtcfgUserSettingsRow
                sessionSettingRow.SettingID = UserSettingsEnum.LIS_HOST_QUERY_PACKAGE.ToString()
                sessionSettingRow.CurrentValue = BsHostQueryPackageNumericUpDown.Value.ToString()
                sessionSettings.tcfgUserSettings.Rows.Add(sessionSettingRow)

                sessionSettingRow = sessionSettings.tcfgUserSettings.NewtcfgUserSettingsRow
                sessionSettingRow.SettingID = UserSettingsEnum.LIS_maxTimeWaitingForResponse.ToString()
                sessionSettingRow.CurrentValue = BsmaxTimeWaitingForResponseNumericUpDown.Value.ToString()
                sessionSettings.tcfgUserSettings.Rows.Add(sessionSettingRow)

                sessionSettingRow = sessionSettings.tcfgUserSettings.NewtcfgUserSettingsRow
                sessionSettingRow.SettingID = UserSettingsEnum.LIS_maxTimeWaitingForACK.ToString()
                sessionSettingRow.CurrentValue = BsmaxTimeWaitACKNumericUpDown.Value.ToString()
                sessionSettings.tcfgUserSettings.Rows.Add(sessionSettingRow)

                sessionSettingRow = sessionSettings.tcfgUserSettings.NewtcfgUserSettingsRow
                sessionSettingRow.SettingID = UserSettingsEnum.LIS_HOST_ID.ToString()
                sessionSettingRow.CurrentValue = BsHostIDTextBox.Text.ToString()
                sessionSettings.tcfgUserSettings.Rows.Add(sessionSettingRow)

                sessionSettingRow = sessionSettings.tcfgUserSettings.NewtcfgUserSettingsRow
                sessionSettingRow.SettingID = UserSettingsEnum.LIS_HOST_PROVIDER.ToString()
                sessionSettingRow.CurrentValue = BsHostProviderTextBox.Text.ToString()
                sessionSettings.tcfgUserSettings.Rows.Add(sessionSettingRow)

                sessionSettingRow = sessionSettings.tcfgUserSettings.NewtcfgUserSettingsRow
                sessionSettingRow.SettingID = UserSettingsEnum.LIS_INSTRUMENT_ID.ToString()
                sessionSettingRow.CurrentValue = BsInstrumentIDTextBox.Text.ToString()
                sessionSettings.tcfgUserSettings.Rows.Add(sessionSettingRow)

                sessionSettingRow = sessionSettings.tcfgUserSettings.NewtcfgUserSettingsRow
                sessionSettingRow.SettingID = UserSettingsEnum.LIS_INSTRUMENT_PROVIDER.ToString()
                sessionSettingRow.CurrentValue = BsInstrumentProviderTextBox.Text.ToString()
                sessionSettings.tcfgUserSettings.Rows.Add(sessionSettingRow)

                If (bsProtocolNameComboBox.SelectedValue = "HL7") Then
                    '** Settings for HL7 Protocol
                    sessionSettingRow = sessionSettings.tcfgUserSettings.NewtcfgUserSettingsRow
                    sessionSettingRow.SettingID = UserSettingsEnum.LIS_FIELD_SEPARATOR_HL7.ToString()
                    sessionSettingRow.CurrentValue = BsFieldSeparatorTextBox.Text
                    sessionSettings.tcfgUserSettings.Rows.Add(sessionSettingRow)

                    sessionSettingRow = sessionSettings.tcfgUserSettings.NewtcfgUserSettingsRow
                    sessionSettingRow.SettingID = UserSettingsEnum.LIS_COMP_SEPARATOR_HL7.ToString()
                    sessionSettingRow.CurrentValue = BsComponentSeparatorTextBox.Text
                    sessionSettings.tcfgUserSettings.Rows.Add(sessionSettingRow)

                    sessionSettingRow = sessionSettings.tcfgUserSettings.NewtcfgUserSettingsRow
                    sessionSettingRow.SettingID = UserSettingsEnum.LIS_REP_SEPARATOR_HL7.ToString()
                    sessionSettingRow.CurrentValue = BsRepeatSeparatorTextBox.Text
                    sessionSettings.tcfgUserSettings.Rows.Add(sessionSettingRow)

                    sessionSettingRow = sessionSettings.tcfgUserSettings.NewtcfgUserSettingsRow
                    sessionSettingRow.SettingID = UserSettingsEnum.LIS_SPEC_SEPARATOR_HL7.ToString()
                    sessionSettingRow.CurrentValue = BsSpecialSeparatorTextBox.Text
                    sessionSettings.tcfgUserSettings.Rows.Add(sessionSettingRow)

                    sessionSettingRow = sessionSettings.tcfgUserSettings.NewtcfgUserSettingsRow
                    sessionSettingRow.SettingID = UserSettingsEnum.LIS_SUBC_SEPARATOR_HL7.ToString()
                    sessionSettingRow.CurrentValue = BsSubComponentSeparatorTextBox.Text
                    sessionSettings.tcfgUserSettings.Rows.Add(sessionSettingRow)
                Else
                    '** Settings for ASTM Protocol
                    sessionSettingRow = sessionSettings.tcfgUserSettings.NewtcfgUserSettingsRow
                    sessionSettingRow.SettingID = UserSettingsEnum.LIS_FIELD_SEPARATOR_ASTM.ToString()
                    sessionSettingRow.CurrentValue = BsFieldSeparatorTextBox.Text
                    sessionSettings.tcfgUserSettings.Rows.Add(sessionSettingRow)

                    sessionSettingRow = sessionSettings.tcfgUserSettings.NewtcfgUserSettingsRow
                    sessionSettingRow.SettingID = UserSettingsEnum.LIS_COMP_SEPARATOR_ASTM.ToString()
                    sessionSettingRow.CurrentValue = BsComponentSeparatorTextBox.Text
                    sessionSettings.tcfgUserSettings.Rows.Add(sessionSettingRow)

                    sessionSettingRow = sessionSettings.tcfgUserSettings.NewtcfgUserSettingsRow
                    sessionSettingRow.SettingID = UserSettingsEnum.LIS_REP_SEPARATOR_ASTM.ToString()
                    sessionSettingRow.CurrentValue = BsRepeatSeparatorTextBox.Text
                    sessionSettings.tcfgUserSettings.Rows.Add(sessionSettingRow)

                    sessionSettingRow = sessionSettings.tcfgUserSettings.NewtcfgUserSettingsRow
                    sessionSettingRow.SettingID = UserSettingsEnum.LIS_SPEC_SEPARATOR_ASTM.ToString()
                    sessionSettingRow.CurrentValue = BsSpecialSeparatorTextBox.Text
                    sessionSettings.tcfgUserSettings.Rows.Add(sessionSettingRow)
                End If

                'Save value of all Settings
                Dim resultData As New GlobalDataTO
                Dim myAnalyzerSettings As New AnalyzerSettingsDelegate

                resultData = myAnalyzerSettings.Save(Nothing, AnalyzerIDAttribute, Nothing, sessionSettings)
                If (Not resultData.HasError) Then
                    'BT #1349 ==> Value to assign to Main MDI Property autoWSCreationWithLISMode will depend also on value of setting LIS_ENABLE_COMMS
                    IAx00MainMDI.autoWSCreationWithLISMode = (BsAutoQueryStartCheckbox.Checked AndAlso BsLISCommsEnabledCheckbox.Checked)
                Else
                    'Error saving the settings, show it
                    Cursor = Cursors.Default
                    ShowMessage(Name & ".SaveChanges", resultData.ErrorCode, resultData.ErrorMessage)
                End If
            End If
        Catch ex As Exception
            Cursor = Cursors.Default
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".SaveChanges ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".SaveChanges ", Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
        Finally
            Cursor = Cursors.Default
        End Try
    End Sub

    ''' <summary>
    ''' Close the screen when there are not changes pending to save or there are some but the User decides discard them
    ''' </summary>
    Private Sub ExitScreen()
        Dim screenClose As Boolean = False
        Try
            If EditionMode Then
                If ChangesMade Then
                    If (ShowMessage(Name, GlobalEnumerates.Messages.DISCARD_PENDING_CHANGES.ToString, , Me) = Windows.Forms.DialogResult.Yes) Then
                        screenClose = True
                    End If
                Else
                    screenClose = True
                End If
            Else
                screenClose = True
            End If

            If screenClose Then
                ' XB 26/11/2013 - Inform to MDI that this screen is closing aims to open next screen - Task #1303
                ExitingScreen()

                'Disable form on close to avoid any button press.
                Me.Enabled = False
                If Not Tag Is Nothing Then
                    'A PerformClick() method was executed
                    Close()
                Else
                    'Normal button click - Open the WS Monitor form and close this one
                    IAx00MainMDI.OpenMonitorForm(Me)
                End If
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".ExitScreen", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ExitScreen", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ' No works well if there are a thread created from this screen because it is angry with the threads created on MDI
    'Private Sub WaitingControl(ByVal pText1 As String, ByVal pText2 As String)
    '    Try
    '        Application.DoEvents()
    '        bwPreload.RunWorkerAsync()
    '        Application.DoEvents()

    '        wfPreload = New WaitScreen(Nothing) _
    '            With { _
    '                .Title = pText1, _
    '                .WaitText = pText2 _
    '            }

    '        wfPreload.Show()
    '        wfPreload.Focus()
    '        Application.DoEvents()
    '    Catch ex As Exception
    '        CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".WaitingControl ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
    '    End Try
    'End Sub

    'Private Sub bwPreload_DoWork(ByVal sender As System.Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles bwPreload.DoWork
    '    Try
    '        'Release the LIS manager object
    '        IAx00MainMDI.InvokeReleaseLIS(mdiESWrapperCopy, False)
    '        System.Threading.Thread.Sleep(1000)
    '        'IAx00MainMDI.SynchronousLISManagerRelease(mdiESWrapperCopy, False)

    '        If RunningAx00MDBackGround Or IAx00MainMDI.ProcessingLISManagerObject Then
    '            While RunningAx00MDBackGround Or IAx00MainMDI.ProcessingLISManagerObject
    '                Application.DoEvents()
    '                System.Threading.Thread.Sleep(100)
    '            End While
    '        End If

    '    Catch ex As Exception
    '        CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".bwPreload_DoWork ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
    '        ShowMessage(Name & ".bwPreload_DoWork ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
    '    End Try
    'End Sub

    'Private Sub bwPreload_RunWorkerCompleted(ByVal sender As System.Object, ByVal e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles bwPreload.RunWorkerCompleted
    '    Try
    '        'If mdiESWrapperCopy.Status = "released" Then
    '        'LIS manager object re-create channel
    '        IAx00MainMDI.InvokeCreateLISChannel(mdiESWrapperCopy)

    '        If RunningAx00MDBackGround Or IAx00MainMDI.ProcessingLISManagerObject Then
    '            While RunningAx00MDBackGround Or IAx00MainMDI.ProcessingLISManagerObject
    '                Application.DoEvents()
    '                System.Threading.Thread.Sleep(100)
    '            End While
    '        End If
    '        'createChannelLISManagerThread = New Thread(Sub() IAx00MainMDI.SynchronousLISManagerCreateChannel(mdiESWrapperCopy))
    '        'createChannelLISManagerThread.Start()
    '        'End If

    '        wfPreload.Close()

    '        'Disable form on close to avoid any button press.
    '        Me.Enabled = False
    '        If Not Tag Is Nothing Then
    '            'A PerformClick() method was executed
    '            Close()
    '        Else
    '            'Normal button click - Open the WS Monitor form and close this one
    '            IAx00MainMDI.OpenMonitorForm(Me)
    '        End If

    '    Catch ex As Exception
    '        CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".bwPreload_RunWorkerCompleted ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
    '        ShowMessage(Name & ".bwPreload_RunWorkerCompleted ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
    '    End Try
    'End Sub
    ' No works well if there are a thread created from this screen because it is angry with the threads created on MDI

    Private Sub ReleaseElements()
        Try
            isClosingFlag = True 'AG 10/02/2014 - #1496 Mark screen closing when ReleaseElement is called
            bsAcceptButton.Image = Nothing
            bsCancelButton.Image = Nothing

            ' No works well if there are a thread created from this screen because it is angry with the threads created on MDI
            'bwPreload = Nothing
            'wfPreload = Nothing

            'If Not createChannelLISManagerThread Is Nothing Then
            '    If createChannelLISManagerThread.IsAlive Then
            '        createChannelLISManagerThread.Abort()
            '    End If
            '    createChannelLISManagerThread = Nothing
            'End If
            ' No works well if there are a thread created from this screen because it is angry with the threads created on MDI

            Me.Dispose()

            GC.Collect()

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ReleaseElements ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ReleaseElements ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Disable elements of all screen
    ''' </summary>
    ''' <remarks>Created by XB 19/03/2013</remarks>
    Private Sub DisableScreen()
        Try
            BsDownloadGroupBox.Enabled = False
            BsUploadGroupBox.Enabled = False
            BsLISCommsEnabledCheckbox.Enabled = False
            bsLISCommsGroupBox.Enabled = False
            bsInternalGroupBox.Enabled = False
            bsLISProtocolGroupBox.Enabled = False
            bsAcceptButton.Enabled = False
            bsCancelButton.Enabled = False

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".DisableScreen ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".DisableScreen ", Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Enable/Disable the ComboBoxes for PORTS and SPEED when Communication is changed between Automatic and Manual
    ''' Enable/Disable the ComboBox for FREQUENCY when the Export To LIS Setting is changed between Automatic and Manual
    ''' </summary>
    ''' <remarks>
    ''' Created by  XB 06/03/2013
    ''' Modified by XB + AG 27/03/2013
    '''             XB 15/05/2013 - duplication of delimiters to distinct HL7 to ASTM
    ''' </remarks>
    Private Sub ControlValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Try
            If EditionMode Then

                Dim myControl As String = DirectCast(sender, Control).Name.ToString
                ValidateAllTabs(myControl)

                Select Case myControl

                    Case "BsAutomaticCheckbox"
                        'When Automatic Upload to LIS, enable FREQUENCY ComboBox
                        If BsAutomaticCheckbox.Checked Then
                            bsFrequencyLabel.Enabled = True
                            bsFreqComboBox.Enabled = True
                        Else
                            bsFrequencyLabel.Enabled = False
                            bsFreqComboBox.Enabled = False
                        End If

                    Case "BsLISCommsEnabledCheckbox"
                        'When LIS Comms is not checked, disable whole LIS communications Groupbox
                        If BsLISCommsEnabledCheckbox.Checked Then
                            If CurrentUserLevel <> "OPERATOR" Then
                                bsLISCommsGroupBox.Enabled = True
                                bsInternalGroupBox.Enabled = True
                            End If
                        Else
                            bsLISCommsGroupBox.Enabled = False
                            bsInternalGroupBox.Enabled = False
                        End If

                    Case "BsDataTransmissionComboBox"
                        Select Case BsDataTransmissionComboBox.SelectedValue
                            Case "TCPIP-Client"
                                HostNameTextBox.Mandatory = True
                                TCPPort2Label.Visible = False
                                TCPPort2NumericUpDown.Visible = False
                                TCPPortLabel.Text = myPortLabel & ":"
                            Case "TCPIP-Server"
                                HostNameTextBox.Mandatory = False
                                TCPPort2Label.Visible = False
                                TCPPort2NumericUpDown.Visible = False
                                TCPPortLabel.Text = myPortLabel & ":"
                            Case "TCPIP-Trans"
                                HostNameTextBox.Mandatory = True
                                TCPPort2Label.Visible = True
                                TCPPort2NumericUpDown.Visible = True
                                TCPPortLabel.Text = myPortClientLabel & ":"
                                TCPPort2Label.Text = myPortServerLabel & ":"
                        End Select

                    Case "bsProtocolNameComboBox"
                        If bsProtocolNameComboBox.SelectedValue = "HL7" Then
                            bsIHECompliantCheckbox.Enabled = True
                            BsSubcomponentLabel.Visible = True
                            BsSubComponentSeparatorTextBox.Visible = True
                            BsHostQueryPackageLabel.Enabled = False
                            BsHostQueryPackageNumericUpDown.Enabled = False
                            BsFieldSeparatorTextBox.Text = myInitialValueField_Sep_HL7
                            BsComponentSeparatorTextBox.Text = myInitialValueComp_Sep_HL7
                            BsRepeatSeparatorTextBox.Text = myInitialValueRep_Sep_HL7
                            BsSpecialSeparatorTextBox.Text = myInitialValueSpec_Sep_HL7
                            BsSubComponentSeparatorTextBox.Text = myInitialValueSubComp_Sep_HL7
                            ' XB 30/05/2013
                            BsmaxTimeWaitACKNumericUpDown.Minimum = minCommonTimerbyHL7
                            BsmaxTimeWaitACKNumericUpDown.Maximum = maxCommonTimerbyHL7
                        Else
                            bsIHECompliantCheckbox.Enabled = False
                            BsSubcomponentLabel.Visible = False
                            BsSubComponentSeparatorTextBox.Visible = False
                            BsHostQueryPackageLabel.Enabled = True
                            BsHostQueryPackageNumericUpDown.Enabled = True
                            BsFieldSeparatorTextBox.Text = myInitialValueField_Sep_ASTM
                            BsComponentSeparatorTextBox.Text = myInitialValueComp_Sep_ASTM
                            BsRepeatSeparatorTextBox.Text = myInitialValueRep_Sep_ASTM
                            BsSpecialSeparatorTextBox.Text = myInitialValueSpec_Sep_ASTM
                            ' XB 30/05/2013
                            BsmaxTimeWaitACKNumericUpDown.Minimum = minCommonTimerbyASTM
                            BsmaxTimeWaitACKNumericUpDown.Maximum = maxCommonTimerbyASTM
                        End If

                        FillDataTransmissionCombo()

                        If BsTransmissionCodePageTextBox.Text.Length = 0 Then
                            ' Call own Fill-function to 'help' user...
                            FillTransmissionCodePageField()
                        End If


                        'Case "BsAutoQueryStartCheckbox" 'SGM 12/07/2013 - not to allow Auto LIS when Samples Barcode disabled
                        '    If Not MyClass.AllowToEnableAutoLIS() Then
                        '        Exit Sub
                        '    End If


                End Select

                ChangesMade = True

                If (myControl = "BsLISCommsEnabledCheckbox" Or _
                    myControl = "BsDataTransmissionComboBox" Or _
                    myControl = "HostNameTextBox" Or _
                    myControl = "TCPPortNumericUpDown" Or _
                    myControl = "TCPPort2NumericUpDown" Or _
                    myControl = "BsMaxReceptionMsgsNumericUpDown" Or _
                    myControl = "BsMaxTransmissionMsgsNumericUpDown" Or _
                    myControl = "BsMaxTimeToRespondNumericUpDown" Or _
                    myControl = "bsProtocolNameComboBox" Or _
                    myControl = "bsIHECompliantCheckbox" Or _
                    myControl = "BsTransmissionCodePageTextBox" Or _
                    myControl = "BsHostIDTextBox" Or _
                    myControl = "BsHostProviderTextBox" Or _
                    myControl = "BsInstrumentIDTextBox" Or _
                    myControl = "BsInstrumentProviderTextBox" Or _
                    myControl = "BsHostQueryPackageNumericUpDown" Or _
                    myControl = "BsmaxTimeWaitingForResponseNumericUpDown" Or _
                    myControl = "BsmaxTimeWaitACKNumericUpDown" Or _
                    myControl = "BsFieldSeparatorTextBox" Or _
                    myControl = "BsComponentSeparatorTextBox" Or _
                    myControl = "BsRepeatSeparatorTextBox" Or _
                    myControl = "BsSpecialSeparatorTextBox" Or _
                    myControl = "BsSubComponentSeparatorTextBox" Or _
                    myControl = "BsBsMaxTimeToWaitLISOrdersNumericUpDown") Then

                    ChangesMadeLIS = True
                End If

            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".ControlValueChanged ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ControlValueChanged ", Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub
#End Region


#Region "Public Methods"

    ''' <summary>
    ''' Activate or deactivate controls depending the current process state
    ''' </summary>
    ''' <param name="pRefreshEventType"></param>
    ''' <param name="pRefreshDS"></param>
    ''' <remarks>XB 24/04/2013</remarks>
    Public Overrides Sub RefreshScreen(ByVal pRefreshEventType As List(Of GlobalEnumerates.UI_RefreshEvents), ByVal pRefreshDS As Biosystems.Ax00.Types.UIRefreshDS)
        Try
            EnableScreen()

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", ".RefreshScreen " & Me.Name, EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".RefreshScreen", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub
#End Region


#Region "Events"

    Private Sub IConfigLIS_FormClosed(sender As Object, e As FormClosedEventArgs) Handles Me.FormClosed
        ReleaseElements()
    End Sub

    ''' <summary>
    ''' Load the screen
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub IConfigLIS_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            ScreenLoad()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".IConfigLIS_Load", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".IConfigLIS_Load", Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' When the screen is in ADD or EDIT Mode and the ESC key is pressed, the code for Cancel Button Click is executed;
    ''' in other case, the screen is closed
    ''' </summary>
    ''' <remarks>
    ''' Created by  XB 06/03/2013
    ''' </remarks>
    Private Sub IConfigLIS_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyDown
        Try
            If (e.KeyCode = Keys.Escape) Then
                bsCancelButton.PerformClick()
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".IConfigLIS_KeyDown", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".IConfigLIS_KeyDown", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub CancelButton_Click(sender As Object, e As EventArgs) Handles bsCancelButton.Click
        Try
            ExitScreen()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".bsCancelButton_Click", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsCancelButton_Click", Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Close screen and save all changes made
    ''' </summary>
    ''' <remarks>
    ''' Created by   XB     05/03/2013
    ''' Modified by AG + XB 27/03/2013 - release + create channels when changes made lis
    '''                  XB 24/04/2013 - Invoke Create Channel again is processed into MDI when 'released' notification is received   
    ''' </remarks>
    Private Sub bsAcceptButton_Click(sender As Object, e As EventArgs) Handles bsAcceptButton.Click
        Try
            Dim isToClose As Boolean = False

            If ChangesMade Then
                If (MyBase.ShowMessage(Name, GlobalEnumerates.Messages.SRV_SAVE_GENERAL_CHANGES.ToString) = Windows.Forms.DialogResult.Yes) Then

                    SaveChanges()
                    If Not ValidationError Then

                        'If ChangesMadeLIS AndAlso BsLISCommsEnabledCheckbox.Checked Then
                        If ChangesMadeLIS Then

                            ' XB 24/04/2013 - Invoke Create Channel again is processed into MDI when 'released' notification is received
                            Cursor = Cursors.WaitCursor
                            'If mdiESWrapperCopy.Status.ToUpperInvariant = LISStatus.unknown.ToString.ToUpperInvariant Or _
                            '   mdiESWrapperCopy.Status.ToUpperInvariant = LISStatus.noconnectionEnabled.ToString.ToUpperInvariant Or _
                            '   mdiESWrapperCopy.Status.ToUpperInvariant = LISStatus.noconnectionDisabled.ToString.ToUpperInvariant Or _
                            '   mdiESWrapperCopy.Status.ToUpperInvariant = LISStatus.connectionEnabled.ToString.ToUpperInvariant Or _
                            '   mdiESWrapperCopy.Status.ToUpperInvariant = LISStatus.connectionDisabled.ToString.ToUpperInvariant Or _
                            '   mdiESWrapperCopy.Status.ToUpperInvariant = LISStatus.connectionRejected.ToString.ToUpperInvariant Or _
                            '   mdiESWrapperCopy.Status.ToUpperInvariant = LISStatus.connectionAccepted.ToString.ToUpperInvariant Then
                            If Not (mdiESWrapperCopy.Status.ToUpperInvariant = LISStatus.released.ToString.ToUpperInvariant) Then

                                DisableScreen()

                                ' No works well if there are a thread created from this screen because it is angry with the threads created on MDI''Release the LIS manager object and create again with new setting values
                                'Me.WaitingControl("Waiting ongoing processes completion...", "Please wait...")
                                'wfPreload.Refresh()
                                'Exit Try
                                ' No works well if there are a thread created from this screen because it is angry with the threads created on MDI

                                'Release the LIS manager object
                                IAx00MainMDI.InvokeReleaseLIS(False)

                                IAx00MainMDI.InvokeReleaseFromConfigSettings = True

                                'System.Threading.Thread.Sleep(1000)

                                'If RunningAx00MDBackGround Or IAx00MainMDI.ProcessingLISManagerObject Then
                                '    While RunningAx00MDBackGround Or IAx00MainMDI.ProcessingLISManagerObject
                                '        Application.DoEvents()
                                '        System.Threading.Thread.Sleep(100)
                                '    End While
                                'End If

                                '' Re-create Channel with new change settings
                                'IAx00MainMDI.InvokeCreateLISChannel()
                                'System.Threading.Thread.Sleep(500)

                                'If RunningAx00MDBackGround Or IAx00MainMDI.ProcessingLISManagerObject Then
                                '    While RunningAx00MDBackGround Or IAx00MainMDI.ProcessingLISManagerObject
                                '        Application.DoEvents()
                                '        System.Threading.Thread.Sleep(100)
                                '    End While
                                'End If


                            Else
                                ' Re-create Channel with new change settings
                                IAx00MainMDI.InvokeCreateLISChannel()
                            End If
                            Cursor = Cursors.Default
                            ' XB 24/04/2013

                        End If

                        isToClose = True
                    End If

                Else
                    isToClose = True
                End If
            Else
                isToClose = True
            End If

            If isToClose Then
                Me.Enabled = False ' Disable the form to avoid pressing buttons on closing
                ChangesMade = False
                ChangesMadeLIS = False
                ExitScreen()
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".bsAcceptButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsAcceptButton_Click ", Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Function to preserve user changes
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>Created by XB 15/05/2013</remarks>
    Private Sub BsFieldSeparatorTextBox_LostFocus(sender As Object, e As EventArgs) Handles BsFieldSeparatorTextBox.LostFocus, _
                                                                                            BsComponentSeparatorTextBox.LostFocus, _
                                                                                            BsRepeatSeparatorTextBox.LostFocus, _
                                                                                            BsSpecialSeparatorTextBox.LostFocus, _
                                                                                            BsSubComponentSeparatorTextBox.LostFocus

        Try
            If bsProtocolNameComboBox.SelectedValue = "HL7" Then
                ' HL7
                myInitialValueField_Sep_HL7 = BsFieldSeparatorTextBox.Text
                myInitialValueComp_Sep_HL7 = BsComponentSeparatorTextBox.Text
                myInitialValueRep_Sep_HL7 = BsRepeatSeparatorTextBox.Text
                myInitialValueSpec_Sep_HL7 = BsSpecialSeparatorTextBox.Text
                myInitialValueSubComp_Sep_HL7 = BsSubComponentSeparatorTextBox.Text
            Else
                ' ASTM
                myInitialValueField_Sep_ASTM = BsFieldSeparatorTextBox.Text
                myInitialValueComp_Sep_ASTM = BsComponentSeparatorTextBox.Text
                myInitialValueRep_Sep_ASTM = BsRepeatSeparatorTextBox.Text
                myInitialValueSpec_Sep_ASTM = BsSpecialSeparatorTextBox.Text
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".BsFieldSeparatorTextBox_LostFocus ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".BsFieldSeparatorTextBox_LostFocus ", Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    Private IsAutoLISCanceled As Boolean = False
    ''' <summary>
    ''' special case for auto lis check
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>Created by SG 12/07/2013</remarks>
    Private Sub BsAutoQueryStartCheckbox_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsAutoQueryStartCheckbox.CheckedChanged
        Try
            If EditionMode Then
                If Me.BsAutoQueryStartCheckbox.Checked Then
                    MyClass.AllowToEnableAutoLIS()
                End If
                If Not IsAutoLISCanceled Then
                    ChangesMade = True
                    'ChangesMadeLIS = True
                End If
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".BsAutoQueryStartCheckbox_CheckedChanged ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".BsAutoQueryStartCheckbox_CheckedChanged ", Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Validate characters entered in both TextBoxes used for Names 
    ''' </summary>
    Private Sub BsSeparatorTextBoxes_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles BsFieldSeparatorTextBox.KeyPress, _
                                                                                                                                 BsComponentSeparatorTextBox.KeyPress, _
                                                                                                                                 BsRepeatSeparatorTextBox.KeyPress, _
                                                                                                                                 BsSpecialSeparatorTextBox.KeyPress, _
                                                                                                                                 BsSubComponentSeparatorTextBox.KeyPress
        Try
            'If (ValidateSpecialCharacters(e.KeyChar, "[@#~$%&/()-+><_.:,;!¿?=·ªº'¡|}^{]")) Then e.Handled = True

            Select Case Asc(e.KeyChar)
                Case 7, 9, 11, 12, 32 ' <BELL> <TAB> <TAB Vertical> <Form Feed> <Space>
                Case 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47 ' ! ' # $ % & " ( ) * + , - . /
                Case 48, 49, 50, 51, 52, 53, 54, 55, 56, 57 ' 0 1 2 3 4 5 6 7 8 9 
                Case 58, 59, 60, 61, 62, 63, 64 ' : ; < = > ? @
                Case 65, 66, 67, 68, 69, 70, 71, 73, 74, 75, 78, 84, 85, 86, 87, 88, 89, 90 ' A B C D E F G I J K N T U V W X Y Z
                Case 91, 92, 93, 94, 95, 96 ' [ \ ] ^ _`
                Case 97, 98, 99, 100, 101, 102, 103, 104, 105, 106, 107, 108, 109, 110, 111, 112, 113, 114, 115, 116, 117, 118, 119, 120, 121, 122 ' a b c d e f g h i j k l m n o p q r s t u v w x y z
                Case 123, 124, 125, 126 ' { | } ~

                Case Else
                    ' the rest of values are not accepted
                    e.Handled = True
            End Select

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".BsSeparatorTextBoxes_KeyPress", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".BsSeparatorTextBoxes_KeyPress", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Checks if AutoLIS is enabled but the Samples Barcode is disabled. If so, a warning message is shown 
    ''' </summary>
    ''' <remarks>
    ''' Created by SGM 12/07/2013
    ''' </remarks>
    Public Function AllowToEnableAutoLIS() As Boolean
        Dim ret As Boolean = False
        Try
            If isClosingFlag Then Return ret 'AG 10/02/2014 - #1496 No refresh is screen is closing
            Dim resultData As New GlobalDataTO

            Dim myAnalyzersDelegate As New AnalyzerSettingsDelegate
            resultData = myAnalyzersDelegate.GetAnalyzerSetting(Nothing, IAx00MainMDI.ActiveAnalyzer, AnalyzerSettingsEnum.SAMPLE_BARCODE_DISABLED.ToString)
            If Not resultData.HasError AndAlso resultData.SetDatos IsNot Nothing Then
                Dim myAnalyzerSettingsDS As AnalyzerSettingsDS = CType(resultData.SetDatos, AnalyzerSettingsDS)
                If myAnalyzerSettingsDS IsNot Nothing AndAlso myAnalyzerSettingsDS.tcfgAnalyzerSettings.Rows.Count > 0 Then
                    Dim myRow As AnalyzerSettingsDS.tcfgAnalyzerSettingsRow = myAnalyzerSettingsDS.tcfgAnalyzerSettings.Rows(0)
                    Dim isDisabled As Boolean = IIf(CInt(myRow.CurrentValue) = 1, True, False).ToString()
                    If isDisabled Then
                        Dim res As DialogResult = MyBase.ShowMessage(Me.Name, Messages.AUTOLIS_BARCODE_DISABLED.ToString)
                    End If
                    ret = isDisabled
                    MyClass.IsAutoLISCanceled = isDisabled
                    Me.BsAutoQueryStartCheckbox.Checked = Not isDisabled
                End If
            End If


        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".AllowToEnableAutoLIS", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".AllowToEnableAutoLIS", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
        Return ret
    End Function


    Private Sub BsDataTransmissionComboBox_SelectedIndexChanged(sender As Object, e As EventArgs) Handles BsDataTransmissionComboBox.SelectedIndexChanged, _
                                                                                                    BsHostQueryCheckbox.CheckedChanged, _
                                                                                                    BsAcceptUnsolicitedOrdersCheckbox.CheckedChanged, _
                                                                                                    BsAcceptOnRunningCheckbox.CheckedChanged, _
 _
 _
                                                                                                    BsUploadUnsolicitedPatientResultsCheckbox.CheckedChanged, _
                                                                                                    BsUploadUnsolicitedQCResultsCheckbox.CheckedChanged, _
                                                                                                    BsUploadResultsOnResetCheckbox.CheckedChanged, _
                                                                                                    BsAutomaticCheckbox.CheckedChanged, _
 _
                                                                                                    HostNameTextBox.TextChanged, _
                                                                                                    TCPPortNumericUpDown.ValueChanged, _
                                                                                                    TCPPort2NumericUpDown.ValueChanged, _
                                                                                                    BsLISCommsEnabledCheckbox.CheckedChanged, _
                                                                                                    BsMaxTimeToRespondNumericUpDown.ValueChanged, _
                                                                                                    bsProtocolNameComboBox.SelectedIndexChanged, _
                                                                                                    BsTransmissionCodePageTextBox.TextChanged, _
                                                                                                    bsIHECompliantCheckbox.CheckedChanged, _
                                                                                                    BsMaxReceptionMsgsNumericUpDown.ValueChanged, _
                                                                                                    BsMaxTransmissionMsgsNumericUpDown.ValueChanged, _
                                                                                                    BsHostQueryPackageNumericUpDown.ValueChanged, _
                                                                                                    BsmaxTimeWaitingForResponseNumericUpDown.ValueChanged, _
                                                                                                    BsmaxTimeWaitACKNumericUpDown.ValueChanged, _
                                                                                                    BsHostIDTextBox.TextChanged, _
                                                                                                    BsHostProviderTextBox.TextChanged, _
                                                                                                    BsInstrumentIDTextBox.TextChanged, _
                                                                                                    BsInstrumentProviderTextBox.TextChanged, _
                                                                                                    BsFieldSeparatorTextBox.TextChanged, _
                                                                                                    BsComponentSeparatorTextBox.TextChanged, _
                                                                                                    BsRepeatSeparatorTextBox.TextChanged, _
                                                                                                    BsSpecialSeparatorTextBox.TextChanged, _
                                                                                                    BsSubComponentSeparatorTextBox.TextChanged, _
                                                                                                    TCPPortNumericUpDown.Validating, _
                                                                                                    TCPPort2NumericUpDown.Validating, _
                                                                                                    BsMaxReceptionMsgsNumericUpDown.Validating, _
                                                                                                    BsMaxTransmissionMsgsNumericUpDown.Validating, _
                                                                                                    BsMaxTimeToRespondNumericUpDown.Validating, _
                                                                                                    BsHostQueryPackageNumericUpDown.Validating, _
                                                                                                    BsmaxTimeWaitingForResponseNumericUpDown.Validating, _
                                                                                                    BsmaxTimeWaitACKNumericUpDown.Validating, _
                                                                                                    BsLISCommsEnabledCheckbox.CheckedChanged, _
                                                                                                    BsBsMaxTimeToWaitLISOrdersNumericUpDown.Validating, _
                                                                                                    BsWSRerunsComboBox.SelectedIndexChanged, _
                                                                                                    BsWSReflexTestsComboBox.SelectedIndexChanged, _
                                                                                                    bsFreqComboBox.SelectedIndexChanged
        ControlValueChanged(sender, e)
    End Sub

#End Region

End Class