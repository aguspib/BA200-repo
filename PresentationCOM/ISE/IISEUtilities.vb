Option Explicit On
Option Strict On

Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Global.TO
Imports Biosystems.Ax00.Global.GlobalEnumerates
Imports Biosystems.Ax00.FwScriptsManagement
Imports Biosystems.Ax00.BL
Imports System.Windows.Forms
Imports System.Drawing
Imports Biosystems.Ax00.CommunicationsSwFw
Imports System.IO
Imports System.Runtime.InteropServices 'WIN32

Public Class IISEUtilities

#Region "Declarations"
    Private WithEvents mdiAnalyzerCopy As AnalyzerManager
    Private myISEManager As ISEManager

    ' Language
    Private currentLanguage As String

    Private myISEListCommands As New List(Of ISECommandTO)

    Private NumRepetitionsSelectedByUser As Integer

    Private CurrentISECommand As New ISECommandTO()

    Private SimulationMode As Boolean

    Private myISEAction As ISECommands



    Private SelectedAdjustmentsDS As New SRVAdjustmentsDS
    Private TemporalAdjustmentsDS As New SRVAdjustmentsDS
    Private myFwAdjustmentsDelegate As FwAdjustmentsDelegate
    Private TempToSendAdjustmentsDelegate As FwAdjustmentsDelegate
    Private WithEvents myFwScriptDelegate As SendFwScriptsDelegate
    ' Edited value
    Private EditedValue As Boolean

    'SGM 19/02/2012
    Private MaintenanceExitNeeded As Boolean = False

    'Private IsReadyForTesting As Boolean = False
    Private MSG_StartInstrument As String 'AG 04/04/2012
    Private MSG_EndProcess As String 'DL 01/06/2012

    Public myParentMDI As BSBaseForm 'SGM 10/05/2010
    Public myScreenPendingToOpenWhileISEUtilClosing As BSBaseForm 'SGM 10/05/2010
    Public IsMDICloseRequested As Boolean = False 'SGM 10/05/2010
    Private IsScreenCloseRequested As Boolean = False 'SGM 13/06/2012
    Public IsCompletelyClosed As Boolean = False 'SGM 19/09/2012

    Private IsInitialPollSent As Boolean = False 'SGM 12/06/2012

    Private IsSwitchOffInformed As Boolean = False 'SGM 02/07/2012

    Private IsInstallGroupNodeActive As Boolean = False ' XBC 27/07/2012
#End Region

#Region "Win32"
    <DllImport("user32.dll")> _
    Private Shared Function HideCaret(ByVal hWnd As IntPtr) As Boolean
    End Function

    <DllImport("user32.dll")> _
    Private Shared Function ShowCaret(ByVal hWnd As IntPtr) As Boolean
    End Function

#End Region

#Region "Events definitions"
    Public Shared Event ActivateScreenEvent(ByVal pEnable As Boolean, ByVal pMessageID As GlobalEnumerates.Messages)
    Public Shared Event ActivateVerticalButtonsEvent(ByVal pEnable As Boolean) 'SGM 14/05/2012

    Public Event StopCurrentOperationFinished(ByVal pAlarmType As ManagementAlarmTypes) 'SGM 19/10/2012
#End Region

#Region "Constructor"

    ''' <summary>
    ''' New
    ''' </summary>
    ''' <param name="pMDI"></param>
    ''' <remarks>Created by XBC 03/02/2012</remarks>
    Public Sub New(ByVal pMDI As BSBaseForm, Optional ByVal pSimulationMode As Boolean = False)
        'Public Sub New(ByRef myMDI As System.Windows.Forms.Form, Optional ByVal pSimulationMode As Boolean = False)
        'RH 12/04/2012 myMDI not needed.

        MyClass.myParentMDI = pMDI
        Me.SimulationMode = pSimulationMode
        InitializeComponent()

    End Sub
#End Region

#Region "Attributes"
    ' XBC 21/03/2012 - Not used  
    'Private CalibrationBubbleMinOKAttr As Single
    'Private CalibrationPumpsMaxOkAttr As Single
    'Private CalibrationPumpsMinOkAttr As Single
    'Private SafetyVolumeCalAAttr As Single
    'Private SafetyVolumeCalBAttr As Single
    'Private OutOfRangeNAAttr As Single
    'Private OutOfRangeKAttr As Single
    'Private OutOfRangeCLAttr As Single
    'Private CleanRequiredAttr As Single
    'Private ExpirationTimeLIAttr As Single
    'Private ExpirationTimeNAAttr As Single
    'Private ExpirationTimeKAttr As Single
    'Private ExpirationTimeCLAttr As Single
    'Private ExpirationTimeREFAttr As Single
    'Private ExpitationTimePumpTubingAttr As Single
    'Private ExpirationTimeFluidTubingAttr As Single
    'Private ConsumeMaxLIAttr As Single
    'Private ConsumeMaxNAAttr As Single
    'Private ConsumeMaxKAttr As Single
    'Private ConsumeMaxCLAttr As Single
    'Private ConsumeMaxREFAttr As Single
    'Private AcceptableMinLIAttr As Single
    'Private AcceptableMinNAAttr As Single
    'Private AcceptableMinKAttr As Single
    'Private AcceptableMinCLAttr As Single
    'Private AcceptableMaxLIAttr As Single
    'Private AcceptableMaxNAAttr As Single
    'Private AcceptableMaxKAttr As Single
    'Private AcceptableMaxCLAttr As Single
    Private DispVolumeMaxAttr As Single
    Private DispVolumeMinAttr As Single
    Private DispVolumeDefAttr As Single

    Private WorkSessionIDAttribute As String = ""
    Private WSStatusAttribute As String = ""

    'User defined data with auxiliary screen
    Private UserLiSelectedAttr As Boolean
    Private UserDateTimeElement1Attr As DateTime
    Private UserDateTimeElement2Attr As DateTime
    Private UserDateTimeElement3Attr As DateTime
    Private UserDateTimeElement4Attr As DateTime
    Private UserDateTimeElement5Attr As DateTime

    Private CurrentBlockNodeAttr As TreeNode 'SGM 08/06/2012
    Private CurrentActionNodeAttr As TreeNode 'SGM 08/06/2012

    Private IsR2ArmAwayFromParkingAttr As Boolean 'SGM 03/07/2012

#End Region

#Region "Properties"
    Private CurrentOperationAttr As OPERATIONS = OPERATIONS.NONE ' controls the operation which is currently executed in each time
    Public Property CurrentOperation() As OPERATIONS
        Get
            Return CurrentOperationAttr
        End Get
        Set(ByVal value As OPERATIONS)
            If CurrentOperationAttr <> value Then
                CurrentOperationAttr = value
            End If
        End Set
    End Property

    ''' <summary>
    ''' returns if the analyzer is available
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private ReadOnly Property IsAnalyzerAvailable() As Boolean
        Get
            Dim isAvailable As Boolean = False

            If Me.SimulationMode Then
                isAvailable = True
            Else
                If MyClass.mdiAnalyzerCopy.Connected Then
                    If MyClass.mdiAnalyzerCopy.AnalyzerStatus = AnalyzerManagerStatus.STANDBY Then

                        If MyClass.mdiAnalyzerCopy.ISE_Manager.IsAnalyzerWarmUp Then
                            isAvailable = False
                        Else
                            'SGM 20/09/2012 - not to check flags in Service sw
                            If ThisIsService Then
                                isAvailable = True
                            Else
                                isAvailable = Not (MyClass.mdiAnalyzerCopy.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.SDOWNprocess) = "INPROCESS" _
                                                         OrElse String.Compare(Me.mdiAnalyzerCopy.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.SDOWNprocess), "PAUSED", False) = 0)
                            End If
                            'end SGM 20/09/2012
                        End If

                    Else
                            If MyClass.mdiAnalyzerCopy.AnalyzerStatus = AnalyzerManagerStatus.RUNNING Then
                                bsScreenErrorProvider.ShowError(MSG_EndProcess)
                            Else
                                bsScreenErrorProvider.ShowError(MSG_StartInstrument)
                            End If
                            isAvailable = False
                    End If

                ElseIf MyClass.myISEManager.IsAnalyzerDisconnected Then
                    isAvailable = False
                Else
                    Me.DisplayMessage(Messages.ISE_NOT_READY.ToString)
                    isAvailable = False
                End If
            End If


            Return isAvailable

        End Get
    End Property

    ''' <summary>
    ''' Returns if the ISE utilities can be performed
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks>Created by SGM 12/06/2012</remarks>
    Private ReadOnly Property IsISEAvailable() As Boolean
        Get

            Dim isAvailable As Boolean = False

            If Me.SimulationMode Then
                isAvailable = True
            Else
                If MyClass.CurrentActionNode IsNot Nothing AndAlso (CStr(MyClass.CurrentActionNode.Tag) = "ACT_ISE" And Not MyClass.myISEManager.IsISEModuleInstalled) Then
                    isAvailable = True
                Else
                    If Not MyClass.myISEManager.IsAnalyzerWarmUp Then
                        If Not MyClass.myISEManager.IsISEInitiating And MyClass.myISEManager.IsISEInitializationDone Then
                            'when initialization finished
                            isAvailable = MyClass.myISEManager.IsISESwitchON

                        ElseIf MyClass.myISEManager.IsISESwitchON Then
                            'when switch on but initialization is pending
                            isAvailable = True
                        Else
                            isAvailable = False
                        End If
                    Else
                        isAvailable = False
                    End If
                End If
            End If

            Return isAvailable

        End Get
    End Property

    Private ReadOnly Property IsLongTermDeactivated() As Boolean
        Get
            Return MyClass.myISEManager.IsLongTermDeactivation
        End Get
    End Property

    Public Property ActiveWorkSession() As String
        Get
            Return WorkSessionIDAttribute
        End Get
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

    Private ReadOnly Property ThisIsService() As Boolean
        Get
            Return GlobalBase.IsServiceAssembly 'SGM 01/02/2012 - Check if it is Service Assembly - Bug #1112
        End Get
    End Property

    'SGM 08/06/2012
    Private Property CurrentBlockNode() As TreeNode
        Get
            Return CurrentBlockNodeAttr
        End Get
        Set(ByVal value As TreeNode)
            If CurrentBlockNodeAttr IsNot value Then
                If value IsNot Nothing Then
                    If CurrentBlockNodeAttr IsNot Nothing Then
                        CurrentBlockNodeAttr.BackColor = Color.White
                    End If

                    CurrentBlockNodeAttr = value

                    If CurrentBlockNodeAttr IsNot Nothing Then
                        CurrentBlockNodeAttr.BackColor = Color.LightGray
                        CurrentBlockNodeAttr.ForeColor = System.Drawing.Color.FromArgb(33, 86, 135)
                    End If

                End If

            End If
        End Set
    End Property

    'SGM 08/06/2012
    Private Property CurrentActionNode() As TreeNode
        Get
            Return CurrentActionNodeAttr
        End Get
        Set(ByVal value As TreeNode)
            If CurrentActionNodeAttr IsNot value Then
                If value IsNot Nothing Then
                    If value.Parent IsNot Nothing Then
                        CurrentActionNodeAttr = value
                        CurrentBlockNode = value.Parent
                    End If
                End If
            End If
        End Set
    End Property

    'SGM 03/07/2012
    Private Property IsR2ArmAwayFromParking() As Boolean
        Get
            Return IsR2ArmAwayFromParkingAttr
        End Get
        Set(ByVal value As Boolean)
            IsR2ArmAwayFromParkingAttr = value
        End Set
    End Property

    'SGM 23/10/2012
    'SERVICE Analyzer Alarm has been detected
    Private CurrentAlarmTypeAttr As ManagementAlarmTypes = ManagementAlarmTypes.NONE
    Private Property CurrentAlarmType() As ManagementAlarmTypes
        Get
            Return CurrentAlarmTypeAttr
        End Get
        Set(ByVal value As ManagementAlarmTypes)
            CurrentAlarmTypeAttr = value
        End Set
    End Property

#End Region

#Region "Enumerations"
    Public Enum OPERATIONS
        NONE
        ISECMD
        INITIALIZE_ISE_MODULE
        INSTALL_ISE_MODULE
        INSTALL_ELECTRODES
        INSTALL_REAGENT_PACK
        INSTALL_PUMP_TUBING
        INSTALL_FLUID_TUBING
        SAVE_ISE_INSTALLATION
        SET_LONG_TERM_DEACTIVATION
        CHECK_CLEAN_PACK
        ENABLE_ISE_PREPARATIONS
        QUIT_UTILITIES
        CLEANING
        PRIME_CALIBRATION
        PRIME_X2_CALIBRATION
        R2_TO_WASHING 'move away from park position
        R2_TO_PARKING 'move back to park position
        CHECK_ALARMS
    End Enum
#End Region


#Region "Event Handlers"

    ''' <summary>
    ''' Executes ManageReceptionEvent() method in the main thread
    ''' </summary>
    ''' <param name="pResponse"></param>
    ''' <param name="pData"></param>
    ''' <remarks>
    ''' Created by XBC 27/03/2012
    ''' </remarks>
    Public Sub ScreenReceptionLastFwScriptEvent(ByVal pResponse As RESPONSE_TYPES, ByVal pData As Object) Handles myFwScriptDelegate.DataReceivedEvent

        Me.UIThread(Function() ManageReceptionEvent(pResponse, pData))

    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="pResponse"></param>
    ''' <param name="pData"></param>
    ''' <remarks>
    ''' Created by XBC 27/03/2012
    ''' </remarks>
    Private Function ManageReceptionEvent(ByVal pResponse As RESPONSE_TYPES, ByVal pData As Object) As Boolean
        Dim myGlobal As New GlobalDataTO
        Try
            'SGM 01/02/2012 - Check if it is User Assembly - Bug #1112
            'If Not My.Application.Info.AssemblyName.ToUpper.Contains("SERVICE") Then
            If Not GlobalBase.IsServiceAssembly Then
                ' User Sw
                Exit Function
            End If
            'manage special operations according to the screen characteristics

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


            Select Case CurrentOperation
                Case OPERATIONS.SAVE_ISE_INSTALLATION

                    Select Case pResponse
                        Case RESPONSE_TYPES.START

                        Case RESPONSE_TYPES.OK
                            Me.PrepareTestedMode()
                    End Select

            End Select

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ManageReceptionEvent ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".ManageReceptionEvent", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, myGlobal.ErrorMessage, Me)
        End Try
    End Function
#End Region

#Region "Public Methods"

    'Public Sub OnManageSentEvent(ByVal pInstructionSent As String) Handles mdiAnalyzerCopy.SendEvent
    '    Try
    '        If pInstructionSent = AnalyzerManagerSwActionList.WAITING_TIME_EXPIRED.ToString Then

    '            'SGM 11/05/2012
    '            If mdiAnalyzerCopy.ISE_Manager.CurrentProcedure <> ISEManager.ISEProcedures.None Then
    '                Dim myISEResultWithComErrors As ISEResultTO = New ISEResultTO
    '                myISEResultWithComErrors.ISEResultType = ISEResultTO.ISEResultTypes.ComError
    '                mdiAnalyzerCopy.ISE_Manager.LastISEResult = myISEResultWithComErrors
    '            End If

    '            MyClass.PrepareErrorMode()

    '        End If

    '    Catch ex As Exception
    '        Dim myLogAcciones As New ApplicationLogManager()
    '        myLogAcciones.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "OnManageReceptionEvent", EventLogEntryType.Error, False)
    '    End Try
    'End Sub

    ''' <summary>
    ''' Refresh this specified screen with the information received from the Instrument
    ''' </summary>
    ''' <param name="pRefreshEventType"></param>
    ''' <param name="pRefreshDS"></param>
    ''' <remarks>Created by XBC 24/01/2012
    ''' AG 10/02/2014 - #1496</remarks>
    Public Overrides Sub RefreshScreen(ByVal pRefreshEventType As List(Of GlobalEnumerates.UI_RefreshEvents), ByVal pRefreshDS As Biosystems.Ax00.Types.UIRefreshDS)
        'Dim myGlobal As New GlobalDataTO

        Try
            If isClosingFlag Then Return 'AG 10/02/2014 - #1496 do not refresh screen when closing it

            ' XBC 04/09/2012 - Correction : App can't close because this flag. 
            ' This flag could be recover when ReleaseElement() will be implemented in this screen (f.ex: IWSRotorPositions)
            ' And then will hardly required to check 'Close App' + 'Close Screen' test docs !
            'If isClosingFlag Then Return 'AG 03/08/2012
            ' XBC 04/09/2012 

            ' XBC 13/07/2012
            Dim disableScreen As Boolean = False
            If Not ThisIsService Then 'SGM 20/09/2012 - not treat if Service Sw
                If Not mdiAnalyzerCopy Is Nothing Then
                    If mdiAnalyzerCopy.AnalyzerStatus <> GlobalEnumerates.AnalyzerManagerStatus.STANDBY OrElse _
                       String.Compare(mdiAnalyzerCopy.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.WUPprocess), "INPROCESS", False) = 0 OrElse _
                       String.Compare(mdiAnalyzerCopy.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.ABORTprocess), "INPROCESS", False) = 0 OrElse _
                       String.Compare(mdiAnalyzerCopy.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.SDOWNprocess), "INPROCESS", False) = 0 Then
                        ' OrElse _ mdiAnalyzerCopy.AnalyzerIsFreeze Then    ' XB 01/12/2013 - Freeze case is managed above - Task #1410

                        disableScreen = True

                        bsScreenErrorProvider.ShowError(MSG_StartInstrument)
                    End If
                Else
                    disableScreen = True
                End If
            End If

            If disableScreen Then
                Exit Sub
            End If
            ' XBC 13/07/2012

            MyClass.myISEManager = mdiAnalyzerCopy.ISE_Manager

            RefreshDoneField = False 'RH 28/03/2012

            If pRefreshEventType.Contains(GlobalEnumerates.UI_RefreshEvents.SENSORVALUE_CHANGED) Then
                Dim sensorValue As Single = 0

                'ISE switch on changed
                sensorValue = mdiAnalyzerCopy.GetSensorValue(GlobalEnumerates.AnalyzerSensors.ISE_SWITCHON_CHANGED)
                If sensorValue = 1 Then
                    ScreenWorkingProcess = False

                    mdiAnalyzerCopy.SetSensorValue(GlobalEnumerates.AnalyzerSensors.ISE_SWITCHON_CHANGED) = 0 'Once updated UI clear sensor

                    'SGM 29/06/2012 - Inform that ISE is Switch OFF
                    Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate
                    If Not myISEManager.IsISESwitchON AndAlso Not MyClass.IsSwitchOffInformed Then
                        Dim myISEOffText As String = myMultiLangResourcesDelegate.GetResourceText(Nothing, "ISE_OFF_ERR", currentLanguage) + vbCrLf
                        Me.AppendAnalyzerResponseText(Me.BsRichTextBox1, myISEOffText)
                        MyClass.IsSwitchOffInformed = True
                    ElseIf myISEManager.IsISESwitchON And MyClass.IsSwitchOffInformed Then
                        If myISEManager.CurrentProcedure <> ISEManager.ISEProcedures.GeneralCheckings Then

                            'Dim myISEOnText As String = vbCrLf + myMultiLangResourcesDelegate.GetResourceText(Nothing, "ISE_ON_WRN", currentLanguage) + " - " + myMultiLangResourcesDelegate.GetResourceText(Nothing, "ISE_CONNECT_PDT", currentLanguage) + vbCrLf + vbCrLf SGM 26/10/2012

                            'SGM 26/10/2012
                            Dim myISEOnText As String = vbCrLf + myMultiLangResourcesDelegate.GetResourceText(Nothing, "ISE_ON_WRN", currentLanguage) + vbCrLf + vbCrLf
                            Me.AppendAnalyzerResponseText(Me.BsRichTextBox1, myISEOnText, ISEManager.ISEProcedureResult.NOK)
                            'Show current Alarms
                            MyClass.CurrentOperation = OPERATIONS.CHECK_ALARMS
                            MyClass.PrepareTestedMode()
                            MyClass.CurrentOperation = OPERATIONS.NONE
                            'end SGM 26/10/2012

                        End If
                        MyClass.IsSwitchOffInformed = False
                    End If

                End If

                'ISE initiated
                sensorValue = mdiAnalyzerCopy.GetSensorValue(GlobalEnumerates.AnalyzerSensors.ISE_CONNECTION_FINISHED)
                If sensorValue > 0 Then
                    ScreenWorkingProcess = False

                    mdiAnalyzerCopy.SetSensorValue(GlobalEnumerates.AnalyzerSensors.ISE_CONNECTION_FINISHED) = 0 'Once updated UI clear sensor

                End If

                'ISE ready changed
                sensorValue = mdiAnalyzerCopy.GetSensorValue(GlobalEnumerates.AnalyzerSensors.ISE_READY_CHANGED)
                If sensorValue = 1 Then
                    ScreenWorkingProcess = False

                    mdiAnalyzerCopy.SetSensorValue(GlobalEnumerates.AnalyzerSensors.ISE_READY_CHANGED) = 0 'Once updated UI clear sensor

                End If


                'ISE procedure finished
                sensorValue = mdiAnalyzerCopy.GetSensorValue(GlobalEnumerates.AnalyzerSensors.ISE_PROCEDURE_FINISHED)
                If sensorValue = 1 Then
                    ScreenWorkingProcess = False

                    mdiAnalyzerCopy.SetSensorValue(GlobalEnumerates.AnalyzerSensors.ISE_PROCEDURE_FINISHED) = 0 'Once updated UI clear sensor
                    mdiAnalyzerCopy.SetSensorValue(GlobalEnumerates.AnalyzerSensors.ISECMD_ANSWER_RECEIVED) = 0

                    If Not MyClass.myISEManager.IsLongTermDeactivation Then
                        If Me.CurrentOperation = OPERATIONS.INITIALIZE_ISE_MODULE Then
                            MyClass.IsInitialPollSent = True
                        End If
                        If MyClass.IsInitialPollSent Then
                            PrepareTestedMode()
                        End If
                    Else
                        PrepareTestedMode()
                    End If


                Else

                    'ANSISE received
                    sensorValue = mdiAnalyzerCopy.GetSensorValue(GlobalEnumerates.AnalyzerSensors.ISECMD_ANSWER_RECEIVED)
                    If sensorValue = 1 Then
                        ScreenWorkingProcess = False

                        mdiAnalyzerCopy.SetSensorValue(GlobalEnumerates.AnalyzerSensors.ISECMD_ANSWER_RECEIVED) = 0 'Once updated UI clear sensor

                        If Not MyClass.myISEManager.IsLongTermDeactivation Then
                            If MyClass.IsInitialPollSent Then
                                If myISEManager.CurrentProcedure <> ISEManager.ISEProcedures.GeneralCheckings AndAlso myISEManager.CurrentProcedure <> ISEManager.ISEProcedures.ActivateReagentsPack Then
                                    MyClass.DisplayISEInfo()
                                End If
                            ElseIf Not MyClass.IsScreenCloseRequested Then
                                If myISEManager.CurrentProcedure = ISEManager.ISEProcedures.GeneralCheckings Then
                                    If Me.CurrentOperation <> OPERATIONS.INITIALIZE_ISE_MODULE Then
                                        MyClass.DisableAll()
                                        Dim myText As String = ""
                                        Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate
                                        myText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "MSG_INITIALIZING_ISE", currentLanguage)
                                        Me.AppendPcSendText(Me.BsRichTextBox1, myText)
                                        Me.CurrentOperation = OPERATIONS.INITIALIZE_ISE_MODULE
                                    End If
                                End If
                            End If
                        ElseIf Not MyClass.myISEManager.CurrentProcedure = ISEManager.ISEProcedures.GeneralCheckings Then
                            MyClass.DisplayISEInfo()
                        End If

                    End If
                End If

                RefreshDoneField = True 'RH 28/03/2012

            End If

            'AG 29/03/2012 
            If Not pRefreshEventType Is Nothing AndAlso pRefreshEventType.Contains(GlobalEnumerates.UI_RefreshEvents.ALARMS_RECEIVED) Then
                'If BsAdjustButton.Enabled Then
                'BsAdjustButton.Enabled = ActivateButtonWithAlarms(GlobalEnumerates.ActionButton.ISE_COMMAND)
                'End If
                'Debug.Print("Quieto parao !!!")
            End If
            'AG 29/03/2012


            'AG 15/03/2012 - when FREEZE appears while UI is disabled because screen is working Sw must reactivate UI
            If mdiAnalyzerCopy.GetSensorValue(GlobalEnumerates.AnalyzerSensors.FREEZE) = 1 Then
                ScreenWorkingProcess = False 'Process finished
                Me.PrepareErrorMode()
                RefreshDoneField = True 'RH 28/03/2012
            End If
            'AG 15/03/2012

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".RefreshScreen ", EventLogEntryType.Error, _
                                                                    GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".RefreshScreen", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

#End Region

#Region "Private Methods"

    ''' <summary>
    ''' Displays a mesage in the message area (service Sw)
    ''' </summary>
    ''' <param name="pMessageID"></param>
    ''' <returns></returns>
    ''' <remarks>Created by XBC 07/02/2012</remarks>
    Private Function DisplayMessage(ByVal pMessageID As String, Optional ByVal p2ndMessageID As String = "") As GlobalDataTO
        Dim resultData As New GlobalDataTO
        Try
            Dim Messages As New MessageDelegate
            Dim myMessagesDS As New MessagesDS
            Dim myGlobalDataTO As New GlobalDataTO

            Dim auxIconName As String = ""
            Dim iconPath As String = MyBase.IconsPath

            'Get type and multilanguage text for the informed Message
            Dim msgText As String = ""
            If pMessageID.Length > 0 Then
                myGlobalDataTO = Messages.GetMessageDescription(Nothing, pMessageID)
                If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                    myMessagesDS = DirectCast(myGlobalDataTO.SetDatos, MessagesDS)

                    Dim Exists As Boolean = False
                    If (myMessagesDS.tfmwMessages.Rows.Count > 0) Then
                        msgText = myMessagesDS.tfmwMessages(0).MessageText
                        Me.BsMessageImage.BackgroundImage = Nothing
                        'Show message with the proper icon according the Message Type
                        If (myMessagesDS.tfmwMessages(0).MessageType = "Error") Then
                            'Error Message 
                            Me.BsMessageLabel.Text = msgText
                            auxIconName = GetIconName("CANCELF")
                            Exists = System.IO.File.Exists(iconPath & auxIconName)

                        ElseIf (myMessagesDS.tfmwMessages(0).MessageType = "Information") Then
                            'Information Message 
                            Me.BsMessageLabel.Text = msgText
                            auxIconName = GetIconName("INFO")
                            Exists = System.IO.File.Exists(iconPath & auxIconName)

                        ElseIf (String.Compare(myMessagesDS.tfmwMessages(0).MessageType, "Warning", False) = 0) Then
                            'Warning
                            Me.BsMessageLabel.Text = msgText
                            auxIconName = GetIconName("WARNING")
                            Exists = System.IO.File.Exists(iconPath & auxIconName)

                        ElseIf (myMessagesDS.tfmwMessages(0).MessageType = "OK") Then
                            'Warning
                            Me.BsMessageLabel.Text = msgText
                            auxIconName = GetIconName("ACCEPTF")
                            Exists = System.IO.File.Exists(iconPath & auxIconName)

                        ElseIf (myMessagesDS.tfmwMessages(0).MessageType = "Working") Then
                            'Warning
                            Me.BsMessageLabel.Text = msgText
                            auxIconName = GetIconName("GEAR")
                            Exists = System.IO.File.Exists(iconPath & auxIconName)
                        End If

                    End If

                    'second line
                    If p2ndMessageID.Length > 0 Then
                        myGlobalDataTO = Messages.GetMessageDescription(Nothing, p2ndMessageID)
                        If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                            myMessagesDS = DirectCast(myGlobalDataTO.SetDatos, MessagesDS)
                            msgText = msgText & " | " & myMessagesDS.tfmwMessages(0).MessageText
                            Me.BsMessageLabel.Text = msgText
                        End If
                    End If

                    If Exists Then
                        If System.IO.File.Exists(iconPath & auxIconName) Then
                            Dim myUtil As New Utilities
                            Dim myImage As Image = Image.FromFile(iconPath & auxIconName)
                            myGlobalDataTO = myUtil.ResizeImage(myImage, New Size(20, 20))
                            If Not myGlobalDataTO.HasError And myGlobalDataTO.SetDatos IsNot Nothing Then
                                Me.BsMessageImage.BackgroundImage = CType(myGlobalDataTO.SetDatos, Image) 'Image.FromFile(iconPath & auxIconName)
                            Else
                                Me.BsMessageImage.BackgroundImage = myImage
                            End If
                            Me.BsMessageImage.BackgroundImageLayout = ImageLayout.Center
                            'myScreenLayout.MessagesPanel.Icon.Size = New Size(20, 20)
                        End If
                    End If
                Else
                    MessageBox.Show(Me, myGlobalDataTO.ErrorCode, "BSAdjustmentBaseForm.mybase.displaymessage", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End If
            Else
                'clear
                Me.BsMessageLabel.Text = ""
                Me.BsMessageImage.BackgroundImage = Nothing
            End If

            Application.DoEvents()

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".DisplayMessage", EventLogEntryType.Error, False)
        End Try
        Return resultData
    End Function

    ''' <summary>
    ''' Send ISE command High Level Instruction
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>Created by XBC 23/01/2012</remarks>
    Private Function SendISEInstruction() As GlobalDataTO
        Dim myResultData As New GlobalDataTO
        Dim myScreenIseCmdTo As New ISECommandTO
        Try
            'If mdiAnalyzerCopy.AnalyzerStatus <> GlobalEnumerates.AnalyzerManagerStatus.SLEEPING Then
            '    myResultData = mdiAnalyzerCopy.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.INFO, True, Nothing, GlobalEnumerates.Ax00InfoInstructionModes.STP) 'Stop ANSINF
            'End If

            MyClass.IsSwitchOffInformed = False

            If Not myResultData.HasError Then
                Select Case Me.myISEAction
                    Case ISECommands.POLL
                        myResultData = myISEManager.PrepareDataToSend_POLL()

                    Case ISECommands.VERSION_CHECKSUM
                        myResultData = myISEManager.PrepareDataToSend_VERSION_CHECKSUM()

                    Case ISECommands.SHOW_BUBBLE_CAL
                        myResultData = myISEManager.PrepareDataToSend_SHOW_BUBBLE_CAL()

                    Case ISECommands.SHOW_PUMP_CAL
                        myResultData = myISEManager.PrepareDataToSend_SHOW_PUMP_CAL()

                    Case ISECommands.LAST_SLOPES
                        myResultData = myISEManager.PrepareDataToSend_LAST_SLOPES()

                    Case ISECommands.READ_mV
                        myResultData = myISEManager.PrepareDataToSend_READ_mV()

                    Case ISECommands.READ_PAGE_0_DALLAS
                        ' by the moment read the page 0 by default
                        myResultData = myISEManager.PrepareDataToSend_READ_PAGE_DALLAS(0)

                    Case ISECommands.READ_PAGE_1_DALLAS
                        ' by the moment read the page 0 by default
                        myResultData = myISEManager.PrepareDataToSend_READ_PAGE_DALLAS(1)

                    Case ISECommands.MAINTENANCE
                        myResultData = myISEManager.PrepareDataToSend_MAINTENANCE()

                    Case ISECommands.DSPA
                        myResultData = myISEManager.PrepareDataToSend_DSPA(Me.BsVolumeUpDown.Value.ToString)

                    Case ISECommands.DSPB
                        myResultData = myISEManager.PrepareDataToSend_DSPB(Me.BsVolumeUpDown.Value.ToString)

                    Case ISECommands.PURGEA
                        myResultData = myISEManager.PrepareDataToSend_PURGEA()

                    Case ISECommands.PURGEB
                        myResultData = myISEManager.PrepareDataToSend_PURGEB()

                    Case ISECommands.PRIME_CALA
                        myResultData = myISEManager.PrepareDataToSend_PRIME_CALA()

                    Case ISECommands.PRIME_CALB
                        myResultData = myISEManager.PrepareDataToSend_PRIME_CALB()

                    Case ISECommands.CLEAN 'it is sent as a procedure SGM 09/05/2012
                        '' Nota : Fw no envia primer ISE! 
                        'myResultData = myISEManager.PrepareDataToSend_CLEAN(CInt(Me.BsPositionUpDown.Value), mdiAnalyzerCopy.ActiveAnalyzer)

                    Case ISECommands.CALB
                        myResultData = myISEManager.PrepareDataToSend_CALB()

                    Case ISECommands.BUBBLE_CAL
                        myResultData = myISEManager.PrepareDataToSend_BUBBLE_CAL()

                    Case ISECommands.PUMP_CAL
                        ' Nota : Fw no envia primer ISE! 
                        myResultData = myISEManager.PrepareDataToSend_PUMP_CAL(CInt(Me.BsPositionUpDown.Value), mdiAnalyzerCopy.ActiveAnalyzer)

                    Case ISECommands.DEBUG_mV_ONE
                        myResultData = myISEManager.PrepareDataToSend_DEBUG_mV(ISECommands.DEBUG_mV_ONE)

                    Case ISECommands.DEBUG_mV_TWO
                        myResultData = myISEManager.PrepareDataToSend_DEBUG_mV(ISECommands.DEBUG_mV_TWO)

                    Case ISECommands.DEBUG_mV_OFF
                        myResultData = myISEManager.PrepareDataToSend_DEBUG_mV(ISECommands.DEBUG_mV_OFF)

                End Select
            End If

            If Not myResultData.HasError AndAlso Not myResultData.SetDatos Is Nothing Then
                myScreenIseCmdTo = CType(myResultData.SetDatos, ISECommandTO)

                'SGM 08/03/2012
                Select Case myScreenIseCmdTo.ISECommandID
                    Case ISECommands.CALB, ISECommands.LAST_SLOPES, ISECommands.PUMP_CAL, ISECommands.SHOW_PUMP_CAL, _
                    ISECommands.BUBBLE_CAL, ISECommands.SHOW_BUBBLE_CAL, ISECommands.READ_mV, ISECommands.READ_PAGE_0_DALLAS, _
                    ISECommands.READ_PAGE_1_DALLAS, ISECommands.VERSION_CHECKSUM

                        myISEManager.CurrentProcedure = ISEManager.ISEProcedures.SingleReadCommand
                        myISEManager.CurrentCommandTO = myScreenIseCmdTo

                    Case Else

                        myISEManager.CurrentProcedure = ISEManager.ISEProcedures.SingleCommand
                        myISEManager.CurrentCommandTO = myScreenIseCmdTo

                End Select

                'update the flag for maintenance exit SGM 19/03/2012
                MyClass.UpdateMaintenanceExitIsNeeded()

                myResultData = myISEManager.SendISECommand

                'SGM 17/10/2012 - Log ISE Operation
                If myScreenIseCmdTo.ISECommandID <> ISECommands.NONE Then
                    Dim myLogAcciones As New ApplicationLogManager()
                    myLogAcciones.CreateLogActivity("ISE UTILITIES: " & myScreenIseCmdTo.ISECommandID.ToString, Me.Name & ".SendISEInstruction ", EventLogEntryType.Information, False)
                End If
                'end SGM 17/10/2012

            End If

            If (myResultData.HasError) OrElse Not mdiAnalyzerCopy.Connected Then
                ShowMessage("Error", myResultData.ErrorCode, myResultData.ErrorMessage)
                Me.PrepareErrorMode()
            End If

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".SendISEInstruction", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            'DL 15/05/2013
            'MyBase.ShowMessage(Me.Name & ".SendISEInstruction", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
            Me.UIThread(Function() ShowMessage(Name & ".SendISEInstruction", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))"))
            'DL 15/05/2013
        End Try
        Return myResultData
    End Function

    ''' <summary>
    ''' Send Internal ISE Manager Instruction
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>
    ''' Created by XBC 07/03/2012
    ''' Modified by XBC 21/01/2013 - RaiseEvent ActivateScreenEvent cause problems (‘Operación no válida a través de subprocesos’) 
    '''                              if is called from here. This operation is placed on Sub ExecuteISEAction, like other ones (Bugs tracking #1108)
    ''' </remarks>
    Private Function SendISEAction() As GlobalDataTO
        Dim myResultData As New GlobalDataTO
        Dim myScreenIseCmdTo As New ISECommandTO
        Try
            'If mdiAnalyzerCopy.AnalyzerStatus <> GlobalEnumerates.AnalyzerManagerStatus.SLEEPING Then
            '    myResultData = mdiAnalyzerCopy.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.INFO, True, Nothing, GlobalEnumerates.Ax00InfoInstructionModes.STP) 'Stop ANSINF
            'End If

            MyClass.IsSwitchOffInformed = False

            If Not myResultData.HasError Then
                Select Case Me.CurrentOperation

                    Case OPERATIONS.R2_TO_WASHING 'move away SGM 04/07/2012
                        MyClass.CurrentActionNode = Nothing
                        myResultData = myISEManager.PrepareDataToSend_R2_TO_WASH

                    Case OPERATIONS.R2_TO_PARKING 'move back SGM 04/07/2012
                        MyClass.CurrentActionNode = Nothing
                        myResultData = myISEManager.PrepareDataToSend_R2_TO_PARK

                    Case OPERATIONS.INITIALIZE_ISE_MODULE
                        ' XBC 22/01/2013
                        'SGM 04/04/2012
                        'If myISEManager.IsISECommsOk Then
                        '    RaiseEvent ActivateScreenEvent(False, Messages.INITIALIZING_ISE)
                        'End If
                        ' XBC 22/01/2013

                        myResultData = myISEManager.DoGeneralCheckings
                        'end SGM 04/04/2012

                    Case OPERATIONS.INSTALL_ISE_MODULE
                        'myResultData = myISEManager.InstallISEModule()

                        'SGM 14/06/2012
                        Application.DoEvents()
                        MyClass.PrepareTestedMode()

                    Case OPERATIONS.INSTALL_REAGENT_PACK
                        'SGM 16/01/2013 Bug #1108
                        myResultData = myISEManager.ActivateReagentsPack()

                        'Commented SGM 16/01/2013
                        '' XBC 04/05/2012
                        'If myISEManager.ReagentsPackInstallationDate <> Nothing Then
                        '    myISEManager.IsCleanPackInstalled = False
                        '    myResultData = myISEManager.ActivateReagentsPack()
                        'Else
                        '    ' XBC 02/04/2012 - Refresh Changes
                        '    myResultData = myISEManager.SetReagentsPackInstallDate(DateTime.Now, True)
                        '    If myResultData.HasError Then
                        '        Me.PrepareErrorMode()
                        '    Else
                        '        'SGM 12/03/2012
                        '        myResultData = myISEManager.ActivateReagentsPack()
                        '        'end SGM 12/03/2012
                        '    End If
                        '    ' XBC 02/04/2012 - Refresh Changes
                        'End If

                    Case OPERATIONS.INSTALL_ELECTRODES

                        ' XBC 30/03/2012 - Refresh Changes
                        myISEManager.IsLiEnabledByUser = Me.UserLiSelectedAttr
                        myResultData = myISEManager.SetElectrodesInstallDates(Me.UserDateTimeElement1Attr, Me.UserDateTimeElement2Attr, Me.UserDateTimeElement3Attr, Me.UserDateTimeElement4Attr, Me.UserDateTimeElement5Attr)
                        If myResultData.HasError Then
                            Me.PrepareErrorMode()
                        Else
                            'SGM 12/03/2012
                            myResultData = myISEManager.ActivateElectrodes()
                            'Update ISE Tests table
                            myResultData = MyClass.UpdateLithiumTestEnabled(myISEManager.IsLiEnabledByUser)
                            'end SGM 12/03/2012
                        End If
                        ' XBC 30/03/2012 - Refresh Changes


                    Case OPERATIONS.INSTALL_FLUID_TUBING
                        'MyClass.PrepareTestedMode()

                    Case OPERATIONS.INSTALL_PUMP_TUBING
                        'MyClass.PrepareTestedMode()

                    Case OPERATIONS.ENABLE_ISE_PREPARATIONS
                        myResultData = myISEManager.DoGeneralCheckings

                    Case OPERATIONS.CHECK_CLEAN_PACK
                        myResultData = myISEManager.CheckCleanPackInstalled

                    Case OPERATIONS.CLEANING
                        myResultData = myISEManager.DoCleaning(CInt(Me.BsPositionUpDown.Value))

                    Case OPERATIONS.PRIME_CALIBRATION  'SGM 11/06/2012
                        myResultData = myISEManager.DoPrimeAndCalibration

                    Case OPERATIONS.PRIME_X2_CALIBRATION  'SGM 11/06/2012
                        myResultData = myISEManager.DoPrimeX2AndCalibration

                End Select
            End If

            'SGM 05/09/2012
            If Me.CurrentOperation <> OPERATIONS.NONE Then
                Me.BsStopButton.Enabled = True

                'SGM 17/10/2012 - Log ISE Operation
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity("ISE UTILITIES: " & Me.CurrentOperation.ToString, Me.Name & ".SendISEAction ", EventLogEntryType.Information, False)

            End If

            If (myResultData.HasError) OrElse Not mdiAnalyzerCopy.Connected Then
                ShowMessage("Error", myResultData.ErrorCode, myResultData.ErrorMessage)
                Me.PrepareErrorMode()
            Else
                MyClass.UpdateMaintenanceExitIsNeeded()
            End If

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".SendISEAction", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            'DL 15/05/2013
            'MyBase.ShowMessage(Me.Name & ".SendISEAction", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
            Me.UIThread(Function() ShowMessage(Name & ".SendISEAction", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))"))
            'DL 15/05/2013
        End Try
        Return myResultData
    End Function

    ''' <summary>
    ''' Send ISE command High Level Instruction
    ''' </summary>
    ''' <remarks>Created by XBC 23/01/2012</remarks>
    Private Sub SendISECMD(ByVal pAction As ISECommands)
        'Dim myGlobal As New GlobalDataTO
        Dim myText As String
        Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate
        Try
            Me.DisplayMessage("")
            Me.PrepareTestingMode()
            Me.Cursor = Cursors.WaitCursor

            If pAction = ISECommands.DSPA Or pAction = ISECommands.DSPB Then
                myText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_ISE_PC_INSTRUCTION", currentLanguage) + _
                         " [" + pAction.ToString + " " + Me.BsVolumeUpDown.Value.ToString + "]"
                Me.AppendPcSendText(Me.BsRichTextBox1, myText)
            Else
                myText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_ISE_PC_INSTRUCTION", currentLanguage) + _
                         " [" + pAction.ToString + "]"
                Me.AppendPcSendText(Me.BsRichTextBox1, myText)
            End If

            Me.CurrentOperation = OPERATIONS.ISECMD

            Me.myISEAction = pAction

            RaiseEvent ActivateScreenEvent(False, Messages.PERFORMING_ISE)

            Application.DoEvents()

            If Me.SimulationMode Then
                ' simulating...
                Me.Cursor = Cursors.WaitCursor
                System.Threading.Thread.Sleep(2000)
                Me.Cursor = Cursors.Default
                'myScreenDelegate.CurrentOperation = IseAdjustmentDelegate.OPERATIONS.DEBUG_ON1  ' in simulation mode test with this particular instruction
                'myScreenDelegate.DebugOn1Readed = True
                'myText = "A400 << <ISE!>" + vbCrLf + vbCrLf
                'Me.AppendAnalyzerResponseText(Me.BsRichTextBox1, myText)
                PrepareTestedMode()
                Me.Cursor = Cursors.Default
            Else

                Dim workingThread As New Threading.Thread(AddressOf SendISEInstruction) 'AddressOf do not allow call method with parameters so use a screen variable
                workingThread.Start()
                While ScreenWorkingProcess
                    Application.DoEvents()
                End While
                workingThread = Nothing
            End If


        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".SendISECMD", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".SendISECMD", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Updates the flag for mintenance exit
    ''' </summary>
    ''' <remarks>created by SGM 19/03/2012</remarks>
    Private Sub UpdateMaintenanceExitIsNeeded()
        Try
            Dim isNeeded As Boolean = False

            If myISEManager.CurrentCommandTO IsNot Nothing Then
                Select Case myISEManager.CurrentCommandTO.ISECommandID  ' MyClass.CurrentISECommand.ISECommandID
                    Case ISECommands.BUBBLE_CAL, ISECommands.CALB, ISECommands.CLEAN, ISECommands.DSPA, ISECommands.DSPB, _
                    ISECommands.MAINTENANCE, ISECommands.PRIME_CALA, ISECommands.PRIME_CALB, ISECommands.PUMP_CAL, ISECommands.PURGEA, _
                    ISECommands.PURGEB
                        isNeeded = True
                    Case Else
                        isNeeded = False
                End Select

                MyClass.MaintenanceExitNeeded = MyClass.MaintenanceExitNeeded Or isNeeded

            End If

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".UpdateMaintenanceExitIsNeeded", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".UpdateMaintenanceExitIsNeeded", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Execute ISE managemenet internal action Instruction
    ''' </summary>
    ''' <remarks>
    ''' Created by XBC 06/03/2012
    ''' Modified by XBC 21/01/2013 - RaiseEvent ActivateScreenEvent cause problems (‘Operación no válida a través de subprocesos’) 
    '''                              if is called from SendISEAction Function. This operation is placed here, like other ones (Bugs tracking #1108)
    ''' </remarks>
    Private Sub ExecuteISEAction(ByVal pNode As TreeNode)
        Dim myGlobal As New GlobalDataTO
        Try
            Dim myText As String = ""
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate
            Me.DisplayMessage("")
            Me.PrepareTestingMode()
            Me.Cursor = Cursors.WaitCursor

            Dim myAction As String = CStr(pNode.Tag)

            Select Case myAction
                Case "INIT_ISE"
                    myText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "MSG_INITIALIZING_ISE", currentLanguage)
                    Me.AppendPcSendText(Me.BsRichTextBox1, myText)
                    Me.CurrentOperation = OPERATIONS.INITIALIZE_ISE_MODULE

                    RaiseEvent ActivateScreenEvent(False, Messages.INITIALIZING_ISE)    ' XBC 21/01/2013

                Case "ACT_ISE"
                    myText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_ISE_ActivateModule", currentLanguage)
                    Me.AppendPcSendText(Me.BsRichTextBox1, myText)
                    Me.CurrentOperation = OPERATIONS.INSTALL_ISE_MODULE
                    Application.DoEvents()
                    MyClass.PrepareTestedMode()
                    If Me.CurrentOperation <> OPERATIONS.SAVE_ISE_INSTALLATION Then
                        Me.CurrentOperation = OPERATIONS.NONE
                    End If


                Case "DEACT_ISE"
                    myText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_ISE_DeactivateModule", currentLanguage)
                    Me.AppendPcSendText(Me.BsRichTextBox1, myText)
                    myGlobal = myISEManager.DeactivateISEModule(True)
                    Me.CurrentOperation = OPERATIONS.SET_LONG_TERM_DEACTIVATION
                    Application.DoEvents()
                    MyClass.PrepareTestedMode()
                    Me.CurrentOperation = OPERATIONS.NONE

                Case "ACT_ELE"

                    'SGM 17/05/2012
                    'verify if there is any Li+ Test in use
                    Dim InUse As Boolean = MyClass.LithiumTestInUse()

                    Using myForm As New IISEDateElementSelection(myISEManager.IsLiEnabledByUser, InUse)
                        myForm.TitleLabel = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_InstallElectrodes", currentLanguage)

                        myForm.Element1Name = "Ref:"    ' myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_RefElectrode", currentLanguage)
                        myForm.Element1Selected = True
                        If myISEManager.HasRefInstallDate Then   ' XBC 29/06/2012
                            myForm.Element1DateTime = myISEManager.RefInstallDate
                        Else
                            myForm.Element1DateTime = Now.Date
                        End If

                        myForm.Element2Name = "Na+:"    ' myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_NaElectrode", currentLanguage)
                        myForm.Element2Selected = True
                        If myISEManager.HasNaInstallDate Then   ' XBC 29/06/2012
                            myForm.Element2DateTime = myISEManager.NaInstallDate
                        Else
                            myForm.Element2DateTime = Now.Date
                        End If

                        myForm.Element3Name = "K+:"     ' myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_KElectrode", currentLanguage)
                        myForm.Element3Selected = True
                        If myISEManager.HasKInstallDate Then   ' XBC 29/06/2012
                            myForm.Element3DateTime = myISEManager.KInstallDate
                        Else
                            myForm.Element3DateTime = Now.Date
                        End If

                        myForm.Element4Name = "Cl-:"    ' myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_ClElectrode", currentLanguage)
                        myForm.Element4Selected = True
                        If myISEManager.HasClInstallDate Then   ' XBC 29/06/2012
                            myForm.Element4DateTime = myISEManager.ClInstallDate
                        Else
                            myForm.Element4DateTime = Now.Date
                        End If

                        myForm.Element5Name = "Li+:"    ' myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_LiElectrode", currentLanguage)
                        myForm.Element5Selected = True
                        If myISEManager.HasLiInstallDate Then   ' XBC 29/06/2012
                            myForm.Element5DateTime = myISEManager.LiInstallDate
                        Else
                            myForm.Element5DateTime = Now.Date
                        End If

                        myForm.ShortMode = False

                        myForm.ShowDialog()

                        If (myForm.DialogResult = Windows.Forms.DialogResult.OK) Then
                            Me.UserDateTimeElement1Attr = myForm.Element1DateTime
                            Me.UserDateTimeElement2Attr = myForm.Element2DateTime
                            Me.UserDateTimeElement3Attr = myForm.Element3DateTime
                            Me.UserDateTimeElement4Attr = myForm.Element4DateTime
                            Me.UserDateTimeElement5Attr = myForm.Element5DateTime
                            Me.UserLiSelectedAttr = myForm.Element5Selected

                            If Me.SimulationMode Then   ' XBC 29/06/2012 
                                myISEManager.IsLiEnabledByUser = Me.UserLiSelectedAttr
                            End If

                            myText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_ISE_ActivateElectrodes", currentLanguage)
                            Me.AppendPcSendText(Me.BsRichTextBox1, myText)
                            Me.CurrentOperation = OPERATIONS.INSTALL_ELECTRODES

                            RaiseEvent ActivateScreenEvent(False, Messages.PERFORMING_ISE)
                        Else
                            Me.PrepareLoadedMode()
                        End If
                    End Using

                Case "ACT_REA"
                    Me.CurrentOperation = OPERATIONS.INSTALL_REAGENT_PACK
                    myText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_ISE_ActivateReagentPack", currentLanguage)
                    Me.AppendPcSendText(Me.BsRichTextBox1, myText)

                    RaiseEvent ActivateScreenEvent(False, Messages.PERFORMING_ISE)

                Case "CLEN"
                    Me.CurrentOperation = OPERATIONS.CLEANING
                    myText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_ISE_PC_INSTRUCTION", currentLanguage) + " >> [" + myAction.ToString + "]"
                    ' myText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_ISE_ActivateReagentPack", currentLanguage) 
                    Me.AppendPcSendText(Me.BsRichTextBox1, myText)

                    RaiseEvent ActivateScreenEvent(False, Messages.PERFORMING_ISE)

                Case "WASH_PACK_INST"
                    Me.CurrentOperation = OPERATIONS.CHECK_CLEAN_PACK
                    myText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_ISE_CHECK_CLEAN_PACK", currentLanguage)
                    Me.AppendPcSendText(Me.BsRichTextBox1, myText)

                Case "ACT_PREP"
                    Me.CurrentOperation = OPERATIONS.ENABLE_ISE_PREPARATIONS
                    myText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_ISE_ENABLE_PREP", currentLanguage)
                    Me.AppendPcSendText(Me.BsRichTextBox1, myText)

                Case "ACT_PUMP_TUB"

                    Using myForm As New IISEDateElementSelection()
                        myForm.TitleLabel = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_InstallTubing", currentLanguage)

                        myForm.Element1Name = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_InstallPumpTubing", currentLanguage)
                        myForm.Element1Selected = True

                        If myISEManager.HasPumpTubingInstallDate Then   ' XBC 29/06/2012
                            myForm.Element1DateTime = myISEManager.PumpTubingInstallDate
                        Else
                            myForm.Element1DateTime = Now.Date
                        End If

                        myForm.ShortMode = True

                        myForm.ShowDialog()

                        If (myForm.DialogResult = Windows.Forms.DialogResult.OK) Then
                            Me.UserDateTimeElement1Attr = myForm.Element1DateTime

                            myText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_InstallPumpTubing", currentLanguage)
                            Me.AppendPcSendText(Me.BsRichTextBox1, myText)
                            Me.BsRichTextBox1.Refresh()

                            Me.CurrentOperation = OPERATIONS.INSTALL_PUMP_TUBING
                            MyClass.PrepareTestedMode()
                            Me.CurrentOperation = OPERATIONS.NONE
                        Else
                            Me.PrepareLoadedMode()
                        End If
                    End Using


                Case "ACT_FLUID_TUB"

                    Using myForm As New IISEDateElementSelection()
                        myForm.TitleLabel = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_InstallTubing", currentLanguage)

                        myForm.Element1Name = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_InstallFluidTubing", currentLanguage)
                        myForm.Element1Selected = True

                        If myISEManager.HasFluidicTubingInstallDate Then   ' XBC 29/06/2012
                            myForm.Element1DateTime = myISEManager.FluidicTubingInstallDate
                        Else
                            myForm.Element1DateTime = Now.Date
                        End If

                        myForm.ShortMode = True

                        myForm.ShowDialog()

                        If (myForm.DialogResult = Windows.Forms.DialogResult.OK) Then
                            Me.UserDateTimeElement1Attr = myForm.Element1DateTime

                            myText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_InstallFluidTubing", currentLanguage)
                            Me.AppendPcSendText(Me.BsRichTextBox1, myText)
                            Me.BsRichTextBox1.Refresh()

                            Me.CurrentOperation = OPERATIONS.INSTALL_FLUID_TUBING
                            MyClass.PrepareTestedMode()
                            Me.CurrentOperation = OPERATIONS.NONE
                        Else
                            Me.PrepareLoadedMode()
                        End If
                    End Using

                Case "PRM_CALB" 'SGM 11/06/2012
                    Me.CurrentOperation = OPERATIONS.PRIME_CALIBRATION
                    myText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_ISE_PRM_CALB", currentLanguage)
                    Me.AppendPcSendText(Me.BsRichTextBox1, myText)
                    RaiseEvent ActivateScreenEvent(False, Messages.PERFORMING_ISE)

                Case "PRM2_CALB" 'SGM 11/06/2012
                    Me.CurrentOperation = OPERATIONS.PRIME_X2_CALIBRATION
                    myText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_ISE_PRM2_CALB", currentLanguage)
                    Me.AppendPcSendText(Me.BsRichTextBox1, myText)
                    RaiseEvent ActivateScreenEvent(False, Messages.PERFORMING_ISE)

                Case "ISE_ALMS" 'SGM 26/10/2012
                    Me.CurrentOperation = OPERATIONS.CHECK_ALARMS
                    MyClass.PrepareTestedMode()
                    Me.CurrentOperation = OPERATIONS.NONE

            End Select

            If Me.CurrentOperation <> OPERATIONS.NONE Then

                'Me.AppendPcSendText(Me.BsRichTextBox1, myText)

                If Me.SimulationMode Then
                    ' simulating...
                    Me.Cursor = Cursors.WaitCursor
                    System.Threading.Thread.Sleep(500)
                    Me.Cursor = Cursors.Default
                    PrepareTestedMode()
                Else
                    Application.DoEvents()
                    Dim workingThread As New Threading.Thread(AddressOf SendISEAction) 'AddressOf do not allow call method with parameters so use a screen variable
                    workingThread.Start()
                    While ScreenWorkingProcess
                        Application.DoEvents()
                    End While
                    workingThread = Nothing
                End If
            End If


        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ExecuteISEAction", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".ExecuteISEAction", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Loads the icons and tooltips used for painting the buttons
    ''' </summary>
    ''' <remarks>XBC 10/01/2012</remarks>
    Private Sub PrepareButtons()
        'Const auxIconName As String = ""
        Dim iconPath As String = MyBase.IconsPath
        'Dim myGlobal As New GlobalDataTO
        Try
            'ACCEPT1
            MyClass.SetButtonImage(BsAdjustButton, "ADJUSTMENT")
            MyClass.SetButtonImage(BsExitButton, "CANCEL")
            MyClass.SetButtonImage(BsSaveAsButton, "READADJ")
            MyClass.SetButtonImage(BsClearButton, "RESETFIELD")
            MyClass.SetButtonImage(BsStopButton, "STOP", 24, 24)

            ''ADJUST Button
            'auxIconName = GetIconName("ADJUSTMENT")
            'If (auxIconName <> "") Then
            '    BsAdjustButton.Image = Image.FromFile(iconPath & auxIconName)
            'End If

            ''Exit button
            'auxIconName = GetIconName("CANCEL")
            'If (auxIconName <> "") Then
            '    BsExitButton.Image = Image.FromFile(iconPath & auxIconName)
            'End If

            ''ADJUST Button
            'auxIconName = GetIconName("READADJ")
            'If (auxIconName <> "") Then
            '    BsSaveAsButton.Image = Image.FromFile(iconPath & auxIconName)
            'End If

            ''ADJUST Button
            'auxIconName = GetIconName("RESETFIELD")
            'If (auxIconName <> "") Then
            '    BsClearButton.Image = Image.FromFile(iconPath & auxIconName)
            'End If

            ''STOP Button
            'auxIconName = GetIconName("STOP")
            'If (auxIconName <> "") Then
            '    BsStopButton.Image = Image.FromFile(iconPath & auxIconName)
            'End If


            'auxIconName = GetIconName("ADJUSTMENT")
            'If System.IO.File.Exists(iconPath & auxIconName) Then
            '            BsAdjustButton.Image = Image.FromFile(iconPath & auxIconName)
            '           BsAdjustButton.ImageAlign = ContentAlignment.MiddleCenter
            '          End If

            'EXIT Button
            'auxIconName = GetIconName("CANCEL")
            'If System.IO.File.Exists(iconPath & auxIconName) Then
            '    BsExitButton.Image = Image.FromFile(iconPath & auxIconName)
            '    BsExitButton.ImageAlign = ContentAlignment.MiddleCenter
            'End If

            ''ADJUST Button
            'auxIconName = GetIconName("READADJ")
            'If System.IO.File.Exists(iconPath & auxIconName) Then
            '    BsSaveAsButton.Image = Image.FromFile(iconPath & auxIconName)
            '    BsSaveAsButton.ImageAlign = ContentAlignment.MiddleCenter
            'End If

            ''ADJUST Button
            'auxIconName = GetIconName("RESETFIELD")
            'If System.IO.File.Exists(iconPath & auxIconName) Then
            '    BsClearButton.Image = Image.FromFile(iconPath & auxIconName)
            '    BsClearButton.ImageAlign = ContentAlignment.MiddleCenter
            'End If

            ''STOP Button
            'auxIconName = GetIconName("STOP")
            'If System.IO.File.Exists(iconPath & auxIconName) Then
            '    BsStopButton.Image = Image.FromFile(iconPath & auxIconName)
            '    BsStopButton.ImageAlign = ContentAlignment.MiddleCenter
            'End If

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".PrepareButtons", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareButtons", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub SetButtonImage(ByVal pButton As Button, ByVal pImageName As String, _
                             Optional ByVal pWidth As Integer = 28, _
                             Optional ByVal pHeight As Integer = 28, _
                             Optional ByVal pAlignment As ContentAlignment = ContentAlignment.MiddleCenter)

        Dim auxIconName As String = ""
        Dim iconPath As String = MyBase.IconsPath
        Dim myGlobal As New GlobalDataTO
        Dim myUtil As New Utilities

        Try

            Dim myButtonImage As Image

            auxIconName = GetIconName(pImageName)
            If System.IO.File.Exists(iconPath & auxIconName) Then
                Dim myImage As Image
                myImage = Image.FromFile(iconPath & auxIconName)

                myGlobal = myUtil.ResizeImage(myImage, New Size(pWidth, pHeight))
                If Not myGlobal.HasError And myGlobal.SetDatos IsNot Nothing Then
                    myButtonImage = CType(myGlobal.SetDatos, Bitmap)
                Else
                    myButtonImage = CType(myImage, Bitmap)
                End If

                pButton.Image = myButtonImage
                pButton.ImageAlign = pAlignment

            End If

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".SetButtonImage", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".SetButtonImage", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Get texts in the current application language for all screen controls
    ''' </summary>
    ''' <remarks>
    ''' Created by: XBC 27/01/2012
    ''' </remarks>
    Private Sub GetScreenLabels()
        Try
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            ' Me.BsTitleLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_ISE", currentLanguage)
            Me.BsISEInfoTitle.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_INFO_TITLE", currentLanguage)

            'SGM 01/02/2012 - Check if it is Service Assembly - Bug #1112
            'If My.Application.Info.AssemblyName.ToUpper.Contains("SERVICE") Then
            If GlobalBase.IsServiceAssembly Then
                ' Service Sw
                Me.BsSubtitleLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_Utilities", currentLanguage)
            Else
                ' User Sw
                Me.BsSubtitleLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_Utilities", currentLanguage)
            End If

            ' Me.GroupBoxActions.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_Actions", currentLanguage)
            Me.FeaturesGroupBox.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Parameters", currentLanguage) 'JB 01/10/2012 - REsource String unification
            ' Me.GroupBoxResults.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_RESULTS_TEST", currentLanguage)
            Me.BsLegendCommandLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Command", currentLanguage)
            'Me.bsFilterCMDLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Command", currentLanguage)
            Me.BsLegendResponseLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Response", currentLanguage)
            'Me.bsFilterANSLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Response", currentLanguage)
            Me.BsLegendWarningLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_WARNING", currentLanguage)
            'Me.bsFilterERRLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_Warning", currentLanguage)

            MSG_StartInstrument = myMultiLangResourcesDelegate.GetResourceText(Nothing, "MSG_StartInstrument", currentLanguage) 'AG 04/04/2012
            MSG_EndProcess = myMultiLangResourcesDelegate.GetResourceText(Nothing, "MSG_EndProcess", currentLanguage) 'DL 01/06/2012

            Me.BsTimesLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Repetition_Times", currentLanguage)
            Me.BsPositionLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRotor_Position", currentLanguage)
            Me.BsVolumeLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Volumen", currentLanguage)
            Me.BsLabelResults.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Results", currentLanguage)

            ' Tooltips
            GetScreenTooltip()

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".GetScreenLabels", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".GetScreenLabels", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Get texts in the current application language for all screen controls
    ''' </summary>
    ''' <remarks>
    ''' Created by: XBC 02/02/2012
    ''' </remarks>
    Private Sub GetScreenTooltip()
        Try
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            ' For Tooltips...
            Me.bsScreenToolTips.SetToolTip(BsAdjustButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_SRV_Action", currentLanguage))
            Me.bsScreenToolTips.SetToolTip(BsClearButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_SRV_ClearResults", currentLanguage))
            Me.bsScreenToolTips.SetToolTip(BsSaveAsButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_SRV_ExportResults", currentLanguage))
            Me.bsScreenToolTips.SetToolTip(BsExitButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_CloseScreen", currentLanguage))
            Me.bsScreenToolTips.SetToolTip(BsStopButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "SRV_BTN_TestStop", currentLanguage))

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".GetScreenTooltip ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".GetScreenTooltip ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Loads the icons used for painting the treeview
    ''' </summary>
    ''' <remarks>XBC 10/01/2012</remarks>
    Private Sub PrepareTreeViewImages()
        Try
            Dim auxIconName As String = ""
            Dim iconPath As String = MyBase.IconsPath

            'NONE Image
            auxIconName = GetIconName("FREECELL") ' NONE
            If System.IO.File.Exists(iconPath & auxIconName) Then
                BSActionsTreeImageList.Images.Add("NONE", Image.FromFile(iconPath & auxIconName))
            End If

            'ACTION Image
            auxIconName = GetIconName("ADJUSTMENT") ' ACTION
            If System.IO.File.Exists(iconPath & auxIconName) Then
                BSActionsTreeImageList.Images.Add("ACTION", Image.FromFile(iconPath & auxIconName))
            End If

            'DONE Image SGM 08/06/2012
            auxIconName = GetIconName("ACCEPT1") ' ACTION DONE
            If System.IO.File.Exists(iconPath & auxIconName) Then
                BSActionsTreeImageList.Images.Add("DONE", Image.FromFile(iconPath & auxIconName))
            End If


            Me.BsActionsTreeView.ImageList = BSActionsTreeImageList

            Me.BsActionsTreeView.SelectedNode = Nothing



        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".PrepareTreeViewImages", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareTreeViewImages", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Set all the screen buttons to true
    ''' </summary>
    ''' <remarks>XBC 23/01/2012</remarks>
    Private Sub EnableButtons()
        Try
            'Me.BsAdjustButton.Enabled = True
            Me.BsClearButton.Enabled = True
            Me.BsSaveAsButton.Enabled = True
            Me.BsExitButton.Enabled = True

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".EnableButtons ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".EnableButtons ", Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Set all the screen buttons to false
    ''' </summary>
    ''' <remarks>XBC 23/01/2012</remarks>
    Private Sub DisableButtons()
        Try
            Me.BsAdjustButton.Enabled = False
            Me.BsClearButton.Enabled = False
            Me.BsSaveAsButton.Enabled = False
            Me.BsExitButton.Enabled = False

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".DisableButtons ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".DisableButtons ", Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub


    ''' <summary>
    ''' Set all the screen elements to disabled
    ''' </summary>
    ''' <remarks>XBC 23/01/2012</remarks>
    Private Sub DisableAll()
        Try

            Me.DisableButtons()
            Me.BsActionsTreeView.Enabled = False
            Me.FeaturesGroupBox.Enabled = False

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".DisableAll ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".DisableAll ", Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Set all the screen elements to enabled
    ''' </summary>
    ''' <remarks>XBC 23/01/2012</remarks>
    Private Sub EnableAll()
        Try

            Me.EnableButtons()
            Me.BsActionsTreeView.Enabled = True
            Me.FeaturesGroupBox.Enabled = True

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".EnableAll ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".EnableAll ", Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Get Limits values from BD for Ise tests
    ''' </summary>
    ''' <remarks>Created by XBC 24/01/2012</remarks>
    Public Function GetLimitValues() As GlobalDataTO
        Dim myResultData As New GlobalDataTO
        Try
            ' Get Value limit ranges
            Dim myFieldLimitsDS As New FieldLimitsDS
            Dim myFieldLimitsDelegate As New FieldLimitsDelegate()



            'Load limits for Acceptable dispensations volume
            myResultData = myFieldLimitsDelegate.GetList(Nothing, FieldLimitsEnum.ISE_DISPAB_LIMITS)
            If (Not myResultData.HasError And Not myResultData.SetDatos Is Nothing) Then
                myFieldLimitsDS = CType(myResultData.SetDatos, FieldLimitsDS)
                If myFieldLimitsDS.tfmwFieldLimits.Rows.Count > 0 Then
                    MyClass.DispVolumeMaxAttr = CType(myFieldLimitsDS.tfmwFieldLimits(0).MaxValue, Decimal)
                    MyClass.DispVolumeMinAttr = CType(myFieldLimitsDS.tfmwFieldLimits(0).MinValue, Decimal)
                    MyClass.DispVolumeDefAttr = CType(myFieldLimitsDS.tfmwFieldLimits(0).DefaultValue, Decimal)
                End If
            End If

        Catch ex As Exception
            myResultData.HasError = True
            myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myResultData.ErrorMessage = ex.Message

            Dim myLogAcciones As New ApplicationLogManager()
            myLogAcciones.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "IseAdjustmentDelegate.GetLimitValues", EventLogEntryType.Error, False)
        End Try
        Return myResultData
    End Function

    ''' <summary>
    ''' Prepare GUI for Loading Mode
    ''' </summary>
    ''' <remarks>
    ''' Created by XBC 24/01/2012
    ''' Modified by XBC - 11/07/2012 - add common protection for tools screens in front of Wup process
    ''' Modified by XBC - 11/07/2012 - set function to Public due make available to be invoked from MDI
    ''' REMARK
    ''' All Tools screens (CHANGE ROTOR, CONDITIONING, ISE and futures) 
    ''' must have the same logic about Open and Close functionalities
    '''            XB 06/11/2013 - Add protection against more performing operations (Shutting down, aborting WS) - BT #1115
    ''' </remarks>
    Public Sub PrepareLoadingMode()
        Dim myResultData As New GlobalDataTO
        'Dim myGlobalbase As New GlobalBase
        Try
            Me.DisableAll()

            RaiseEvent ActivateVerticalButtonsEvent(False) 'Disable Vertical Buttons bar while ISE Utilities is open SGM 14/05/2012

            'SGM 01/02/2012 - Check if it is Service Assembly - Bug #1112
            'If My.Application.Info.AssemblyName.ToUpper.Contains("SERVICE") Then
            If GlobalBase.IsServiceAssembly Then
                Me.LoadAdjustmentGroupData()
                MyClass.myFwScriptDelegate = New SendFwScriptsDelegate(mdiAnalyzerCopy)
            End If

            ' Get common Parameters
            'myResultData = GetParams(mdiAnalyzerCopy.GetModelValue)

            ' XB 27/09/2013 - Set the value by default
            'Me.BsPositionUpDown.Value = 1
            Dim myParams As New SwParametersDelegate
            Dim myParametersDS As New ParametersDS
            myResultData = myParams.ReadByParameterName(Nothing, GlobalEnumerates.SwParameters.ISE_UTIL_WASHSOL_POSITION.ToString, MyClass.AnalyzerModel)
            If Not myResultData.HasError And Not myResultData.SetDatos Is Nothing Then
                myParametersDS = CType(myResultData.SetDatos, ParametersDS)
                Me.BsPositionUpDown.Value = CInt(myParametersDS.tfmwSwParameters.Item(0).ValueNumeric)
            Else
                myResultData.HasError = True
                Exit Try
            End If
            ' XB 27/09/2013

            myResultData = Me.GetLimitValues()

            If myResultData.HasError Then

                MyClass.PrepareErrorMode()

            Else

                Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

                If Me.SimulationMode Then

                    MyClass.IsInitialPollSent = True
                    MyClass.SendISECMD(ISECommands.POLL)

                Else

                    ' XBC 11/07/2012 - First of all - check availabilty of the Intrument
                    Dim disableScreen As Boolean = False
                    If Not mdiAnalyzerCopy Is Nothing Then
                        If mdiAnalyzerCopy.AnalyzerStatus <> GlobalEnumerates.AnalyzerManagerStatus.STANDBY OrElse _
                           String.Compare(mdiAnalyzerCopy.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.WUPprocess), "INPROCESS", False) = 0 OrElse _
                           String.Compare(mdiAnalyzerCopy.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.ABORTprocess), "INPROCESS", False) = 0 OrElse _
                           String.Compare(mdiAnalyzerCopy.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.SDOWNprocess), "INPROCESS", False) = 0 OrElse _
                           mdiAnalyzerCopy.AnalyzerIsFreeze OrElse _
                           Not mdiAnalyzerCopy.AnalyzerIsReady Then
                            ' XB 06/11/2013 - ABORTprocess and SDOWNprocess added 

                            disableScreen = True

                            Me.DisplayMessage(Messages.ISE_NOT_READY.ToString)
                            'JV 05/11/2013 issue #1156: only the MSG_StartInstrument if the Analyzer is not ShuttingDown
                            If mdiAnalyzerCopy.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.WUPprocess) = "INPROCESS" Then
                                bsScreenErrorProvider.ShowError(MSG_StartInstrument)
                            End If
                            'bsScreenErrorProvider.ShowError(MSG_StartInstrument)
                            'JV 05/11/2013 issue #1156
                        End If
                    Else
                        disableScreen = True
                    End If


                    If disableScreen Then
                        EnableAll()
                    Else

                        'SGM 26/10/2012 - Show current Alarms
                        MyClass.CurrentOperation = OPERATIONS.CHECK_ALARMS
                        MyClass.PrepareTestedMode()
                        MyClass.CurrentOperation = OPERATIONS.NONE
                        'end SGM 26/10/2012


                        ' XBC 31/08/2102 - Correction : Initialization ISE module is required
                        'If MyClass.ValidateISEAvailability Then
                        If MyClass.ValidateISEAvailability(True) Then
                            ' XBC 31/08/2102

                            If Not myISEManager.IsLongTermDeactivation Then

                                'SGM 04/04/2012 If the ISE Module is pending to connect, start connection process
                                If Not myISEManager.IsISEOnceInitiatedOK OrElse Not myISEManager.IsISEInitiatedOK Then

                                    Dim myText As String = ""
                                    myText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "MSG_INITIALIZING_ISE", currentLanguage) + vbCrLf
                                    Me.AppendPcSendText(Me.BsRichTextBox1, myText)
                                    Me.BsExitButton.Enabled = False
                                    Me.CurrentOperation = OPERATIONS.INITIALIZE_ISE_MODULE
                                    If Not myISEManager.CurrentProcedure = ISEManager.ISEProcedures.GeneralCheckings Then
                                        MyClass.SendISEAction()
                                    End If

                                ElseIf Not myISEManager.CurrentProcedure = ISEManager.ISEProcedures.GeneralCheckings And Not myISEManager.IsISEInitiatedOK Then
                                    'else, send poll
                                    MyClass.IsInitialPollSent = True
                                    MyClass.SendISECMD(ISECommands.POLL)

                                ElseIf myISEManager.IsISEInitiatedOK Then
                                    'Start moving arm to washing if already initiated
                                    MyClass.IsInitialPollSent = True
                                    MyClass.PrepareTestingMode()
                                    Dim myText As String = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_ISE_R2_OUT", currentLanguage) + vbCrLf
                                    Me.AppendPcSendText(Me.BsRichTextBox1, myText)
                                    MyClass.CurrentOperation = OPERATIONS.R2_TO_WASHING
                                    MyClass.myISEAction = ISECommands.R2_TO_WASHING
                                    MyClass.myParentMDI.Refresh()
                                    RaiseEvent ActivateScreenEvent(False, Messages.PERFORMING_ISE)
                                    Application.DoEvents()
                                    myResultData = myISEManager.MoveR2ArmAway
                                    If myResultData.HasError Then
                                        Me.PrepareErrorMode()
                                    End If
                                End If
                            Else
                                Me.DisplayMessage(Messages.ISE_NOT_READY.ToString)
                                MyClass.EnableAll()
                            End If

                        ElseIf myISEManager.CurrentProcedure = ISEManager.ISEProcedures.GeneralCheckings Then
                            'if the Initializing process is being performed
                            Dim myText As String = ""
                            myText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "MSG_INITIALIZING_ISE", currentLanguage) + vbCrLf
                            Me.AppendPcSendText(Me.BsRichTextBox1, myText)
                            Me.CurrentOperation = OPERATIONS.INITIALIZE_ISE_MODULE

                        Else
                            If Not myISEManager.IsLongTermDeactivation Then
                                If myISEManager.IsISEInitializationDone Then
                                    MyClass.EnableAll()
                                Else
                                    MyClass.EnableAll()
                                    Me.BsExitButton.Enabled = True
                                    If MyClass.mdiAnalyzerCopy.AnalyzerStatus = AnalyzerManagerStatus.SLEEPING Then
                                        RaiseEvent ActivateVerticalButtonsEvent(True)
                                    End If
                                End If
                            Else
                                MyClass.EnableAll()
                            End If

                            If Not myISEManager.IsISESwitchON Then
                                MyClass.IsSwitchOffInformed = True
                            End If

                        End If

                    End If

                End If


            End If


        Catch ex As Exception
            myResultData.HasError = True
            myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myResultData.ErrorMessage = ex.Message
            MyBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".PrepareLoadingMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareLoadingMode ", Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub


    ''' <summary>
    ''' refresh the ability of the Analyzer for testing
    ''' </summary>
    ''' <remarks>SGM 23/03/2012</remarks>
    Private Function ValidateAnalyzerReadiness() As Boolean
        Dim myGlobal As New GlobalDataTO
        Try
            Dim isReady As Boolean = False

            isReady = MyClass.IsAnalyzerAvailable

            If isReady Then
                If String.Compare(mdiAnalyzerCopy.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.WUPprocess), "INPROCESS", False) = 0 OrElse _
                   String.Compare(mdiAnalyzerCopy.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.ABORTprocess), "INPROCESS", False) = 0 OrElse _
                   String.Compare(mdiAnalyzerCopy.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.SDOWNprocess), "INPROCESS", False) = 0 Then
                    isReady = False
                End If
            End If

            If Not isReady Then
                'Disable buttons
                Me.DisplayMessage(Messages.ISE_NOT_READY.ToString)

                If String.Compare(mdiAnalyzerCopy.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.WUPprocess), "INPROCESS", False) = 0 OrElse _
                   String.Compare(mdiAnalyzerCopy.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.ABORTprocess), "INPROCESS", False) = 0 OrElse _
                   String.Compare(mdiAnalyzerCopy.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.SDOWNprocess), "INPROCESS", False) = 0 Then
                    ' Nothing to do ' XB 06/11/2013 - Don't change the label warning if there are processing operations
                Else
                    RaiseEvent ActivateScreenEvent(True, Messages.STANDBY)
                End If

            Else
                Me.DisplayMessage("")
            End If

            Me.BsExitButton.Enabled = True
            Me.BsClearButton.Enabled = (Me.BsRichTextBox1.Text.Length > 0)
            Me.BsSaveAsButton.Enabled = (Me.BsRichTextBox1.Text.Length > 0)

            'RaiseEvent ActivateScreenEvent(True)

            ' Me.BsAdjustButton.Visible = True
            'Me.BsStopButton.Visible = False


            Return isReady


        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ValidateAnalyzerReadiness ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".ValidateAnalyzerReadiness ", Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Function

    ''' <summary>
    ''' Prepare GUI for Loaded Mode
    ''' </summary>
    ''' <remarks>Created by XBC 24/01/2012</remarks>
    Private Sub PrepareLoadedMode()
        Try

            Me.DisplayMessage("")

            Me.BsExitButton.Enabled = True
            Me.BsActionsTreeView.Enabled = True
            Me.BsClearButton.Enabled = True
            Me.BsSaveAsButton.Enabled = True

            Me.FeaturesGroupBox.Enabled = True
            RaiseEvent ActivateScreenEvent(True, Messages.STANDBY)

            Me.BsAdjustButton.Visible = True
            Me.BsStopButton.Visible = False


        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".PrepareLoadedMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareLoadedMode ", Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        Finally
            Me.Cursor = Cursors.Default
        End Try
    End Sub

    ''' <summary>
    ''' Prepare GUI for Testing Mode
    ''' </summary>
    ''' <remarks>Created by XBC 23/01/2012</remarks>
    Private Sub PrepareTestingMode()
        Dim myResultData As New GlobalDataTO
        Try
            MyClass.DisableAll()
            ScreenWorkingProcess = True

            Me.Cursor = Cursors.WaitCursor

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".PrepareTestingMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareTestingMode ", Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub


    ''' <summary>
    ''' Prepare GUI for Tested Mode
    ''' </summary>
    ''' <remarks>
    ''' Created by:  XB 24/01/2012
    ''' Modified by: XB 27/09/2013 - BT #1294 ==> Set the value by default to ISE Washing solution position tube
    '''              XB 28/04/2014 - Disable screen when saving consumptions
    ''' </remarks>
    Private Sub PrepareTestedMode(Optional ByVal pResponse As RESPONSE_TYPES = RESPONSE_TYPES.OK)
        Try
            Dim myGlobal As New GlobalDataTO
            Dim myText As String = ""
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            Dim IsActionCompleted As Boolean = False 'SGM 30/03/2012


            If Me.SimulationMode Then

                Me.DisplayISEInfo()

                If CurrentOperation = OPERATIONS.ISECMD Then
                    Me.NumRepetitionsSelectedByUser -= 1
                    If Me.NumRepetitionsSelectedByUser > 0 Then
                        If Me.CurrentISECommand.ISECommandID = ISECommands.READ_PAGE_1_DALLAS Then
                            Me.CurrentISECommand.ISECommandID = ISECommands.READ_PAGE_0_DALLAS
                        End If
                        Me.SendISECMD(Me.CurrentISECommand.ISECommandID)
                        Exit Try
                    End If
                End If

                Select Case CurrentOperation
                    Case OPERATIONS.INSTALL_PUMP_TUBING
                        myGlobal = myISEManager.SetPumpTubingInstallDate(Me.UserDateTimeElement1Attr)
                    Case OPERATIONS.INSTALL_FLUID_TUBING
                        myGlobal = myISEManager.SetFluidicTubingInstallDate(Me.UserDateTimeElement1Attr)
                    Case OPERATIONS.INSTALL_ELECTRODES
                        myGlobal = myISEManager.SetElectrodesInstallDates(Me.UserDateTimeElement1Attr, _
                                                                          Me.UserDateTimeElement2Attr, _
                                                                          Me.UserDateTimeElement3Attr, _
                                                                          Me.UserDateTimeElement4Attr, _
                                                                          Me.UserDateTimeElement5Attr, True)
                End Select


                IsActionCompleted = True ' Not myGlobal.HasError 'SGM 30/03/2012

                Me.DisplayMessage(Messages.SRV_TEST_READY.ToString)

                'Me.BsTimesUpDown.Value = 1

                ' XB 27/09/2013 - Set the value by default
                'Me.BsPositionUpDown.Value = 1
                Dim myParams As New SwParametersDelegate
                Dim myParametersDS As New ParametersDS
                myGlobal = myParams.ReadByParameterName(Nothing, GlobalEnumerates.SwParameters.ISE_UTIL_WASHSOL_POSITION.ToString, MyClass.AnalyzerModel)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myParametersDS = CType(myGlobal.SetDatos, ParametersDS)
                    Me.BsPositionUpDown.Value = CInt(myParametersDS.tfmwSwParameters.Item(0).ValueNumeric)
                Else
                    myGlobal.HasError = True
                    Exit Try
                End If
                ' XB 27/09/2013

                Me.BsActionsTreeView.Enabled = True
                Me.BsClearButton.Enabled = True
                Me.BsSaveAsButton.Enabled = True

                Me.FeaturesGroupBox.Enabled = True

                Me.BsAdjustButton.Visible = True
                Me.BsStopButton.Visible = False

                Me.Cursor = Cursors.Default
                Me.BsExitButton.Enabled = True


            Else

                If myISEManager.LastProcedureResult = ISEManager.ISEProcedureResult.Exception Then

                    'SERVICE: In case of alarm not to treat
                    If ThisIsService Then
                        If MyClass.CurrentAlarmType <> ManagementAlarmTypes.NONE Then
                            Exit Sub
                        End If
                    End If

                    'System.Threading.Thread.Sleep(1000)

                    MyClass.MaintenanceExitNeeded = False
                    Me.BsExitButton.Enabled = True
                    MyClass.EnableAll()
                    RaiseEvent ActivateScreenEvent(True, Messages.STANDBY)

                    Me.BsAdjustButton.Enabled = ValidateNodeByBlock(MyClass.CurrentActionNode)

                    MyClass.mdiAnalyzerCopy.RefreshISEAlarms()

                    Me.NumRepetitionsSelectedByUser = 0
                    Me.BsAdjustButton.Visible = True
                    Me.BsStopButton.Visible = False

                    'SGM 26/10/2012 - ISE Alarms
                    If MyClass.CurrentOperation = OPERATIONS.CHECK_ALARMS Then
                        Dim myPendingCal As New List(Of ISEManager.MaintenanceOperations)
                        myGlobal = myISEManager.GetISEAlarmsForUtilities(myPendingCal)
                        If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                            Dim myAlarms As List(Of Alarms) = CType(myGlobal.SetDatos, List(Of Alarms))
                            Me.AppendISEAlarmsText(Me.BsRichTextBox1, myAlarms, myPendingCal)
                        End If
                        Exit Sub
                    End If
                    'end SGM 26/10/2012

                Else

                    Select Case MyClass.CurrentOperation
                        Case OPERATIONS.ISECMD

                            If myISEManager.LastProcedureResult <> ISEManager.ISEProcedureResult.OK Then
                                myGlobal.HasError = True

                                MyClass.mdiAnalyzerCopy.RefreshISEAlarms()
                                Me.DisplayISEInfo() 'JB 19/09/2012 : Show ISE Info when errors too
                            Else
                                If Me.CurrentISECommand.ISECommandID <> ISECommands.NONE Then
                                    Me.DisplayISEInfo()
                                    'ElseIf (myISEManager.CurrentProcedure <> Nothing AndAlso myISEManager.CurrentProcedure = ISEManager.ISEProcedures.GeneralCheckings) And myISEManager.LastProcedureResult = ISEManager.ISEProcedureResult.OK Then
                                    '    MyClass.IsInitialPollSent = True
                                End If

                                If MyClass.IsR2ArmAwayFromParking Then
                                    Me.DisplayMessage(Messages.SRV_TEST_READY.ToString)
                                End If


                                If Me.CurrentISECommand.ISECommandID = ISECommands.READ_PAGE_0_DALLAS Then
                                    Application.DoEvents()
                                    Me.CurrentISECommand.ISECommandID = ISECommands.READ_PAGE_1_DALLAS
                                    Me.SendISECMD(Me.CurrentISECommand.ISECommandID)
                                    Exit Try

                                    'SGM 15/10/2012 - in case of LongTermDeactivation, if not Initiated yet, launch initialization
                                ElseIf Me.CurrentISECommand.ISECommandID = ISECommands.READ_PAGE_1_DALLAS Then
                                    If myISEManager.LastProcedureResult = ISEManager.ISEProcedureResult.OK Then
                                        If MyClass.IsInstallGroupNodeActive And myISEManager.IsLongTermDeactivation And Not myISEManager.IsISEInitiatedOK Then
                                            Application.DoEvents()
                                            Me.CurrentISECommand.ISECommandID = ISECommands.NONE
                                            MyClass.CurrentOperation = OPERATIONS.INITIALIZE_ISE_MODULE
                                            myISEManager.DoGeneralCheckings(True)
                                            RaiseEvent ActivateScreenEvent(False, Messages.INITIALIZING_ISE)
                                            Exit Try
                                        End If
                                    End If
                                End If

                                Me.NumRepetitionsSelectedByUser -= 1
                                If Me.NumRepetitionsSelectedByUser > 0 Then

                                    'Not Apply - SGM 16/10/2012
                                    'If Me.CurrentISECommand.ISECommandID = ISECommands.READ_PAGE_1_DALLAS Then
                                    '    Me.CurrentISECommand.ISECommandID = ISECommands.READ_PAGE_0_DALLAS
                                    'End If

                                    Me.SendISECMD(Me.CurrentISECommand.ISECommandID)
                                    Exit Try
                                End If

                                If Not MyClass.IsR2ArmAwayFromParking Or Me.myISEAction = ISECommands.CLEAN_START Or Me.myISEAction = ISECommands.PUMP_CAL_START Then
                                    Me.Cursor = Cursors.WaitCursor
                                Else
                                    Me.BsActionsTreeView.Enabled = True
                                    'Me.bsFilterCMDCheckbox.Enabled = True
                                    'Me.bsFilterANSCheckbox.Enabled = True
                                    'Me.bsFilterERRCheckbox.Enabled = True
                                    Me.BsClearButton.Enabled = True
                                    Me.BsSaveAsButton.Enabled = True

                                    Me.FeaturesGroupBox.Enabled = True
                                    'RaiseEvent ActivateScreenEvent(True)

                                    Me.BsAdjustButton.Visible = True
                                    Me.BsStopButton.Visible = False
                                End If

                            End If

                            If MyClass.IsR2ArmAwayFromParking Then
                                IsActionCompleted = True ' Not myGlobal.HasError 'SGM 30/03/2012
                                Me.Cursor = Cursors.Default
                                Me.BsExitButton.Enabled = True

                                'Me.BsTimesUpDown.Value = 1

                                ' XB 27/09/2013 - Set the value by default
                                'Me.BsPositionUpDown.Value = 1
                                Dim myParams As New SwParametersDelegate
                                Dim myParametersDS As New ParametersDS
                                myGlobal = myParams.ReadByParameterName(Nothing, GlobalEnumerates.SwParameters.ISE_UTIL_WASHSOL_POSITION.ToString, MyClass.AnalyzerModel)
                                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                                    myParametersDS = CType(myGlobal.SetDatos, ParametersDS)
                                    Me.BsPositionUpDown.Value = CInt(myParametersDS.tfmwSwParameters.Item(0).ValueNumeric)
                                Else
                                    myGlobal.HasError = True
                                    Exit Try
                                End If
                                ' XB 27/09/2013
                            End If

                        Case OPERATIONS.R2_TO_WASHING ' moved away SGM 04/07/2012
                            If myISEManager.LastProcedureResult = ISEManager.ISEProcedureResult.OK Then
                                IsActionCompleted = True
                                MyClass.IsR2ArmAwayFromParking = True
                                Me.Cursor = Cursors.Default
                                Me.BsExitButton.Enabled = True
                            Else
                                myGlobal.HasError = True
                            End If

                        Case OPERATIONS.R2_TO_PARKING ' moved back SGM 04/07/2012
                            If myISEManager.LastProcedureResult = ISEManager.ISEProcedureResult.OK Then
                                IsActionCompleted = True
                                MyClass.IsR2ArmAwayFromParking = False

                                If MyClass.IsScreenCloseRequested Or MyClass.IsMDICloseRequested Then
                                    MyClass.CurrentOperation = OPERATIONS.NONE
                                    RaiseEvent ActivateScreenEvent(True, Messages.STANDBY)
                                    Me.Close()
                                    Exit Sub
                                End If

                                Me.Cursor = Cursors.Default
                                Me.BsExitButton.Enabled = True
                            Else
                                myGlobal.HasError = True
                            End If

                        Case OPERATIONS.INITIALIZE_ISE_MODULE 'SGM 04/04/2012 initialize (Connect) after ISE is previously switched on
                            If myISEManager.LastProcedureResult = ISEManager.ISEProcedureResult.OK Then
                                System.Threading.Thread.Sleep(1000)
                                MyClass.IsInitialPollSent = True
                                IsActionCompleted = True
                                MyClass.PrepareReadyForTestingMode()
                            Else
                                'Me.CurrentOperation = OPERATIONS.NONE
                                myGlobal.HasError = True
                                Me.PrepareErrorMode()
                            End If


                        Case OPERATIONS.INSTALL_ISE_MODULE
                            If pResponse = RESPONSE_TYPES.OK Then
                                ' Ise is succesfully installed so save it into Adjustments of the Analyzer
                                Me.CurrentOperation = OPERATIONS.SAVE_ISE_INSTALLATION
                                If ThisIsService And Not myISEManager.IsISEModuleInstalled Then
                                    myGlobal = Me.SaveAdjustments(True)
                                    If Not myGlobal.HasError Then
                                        Exit Sub
                                    End If
                                Else
                                    myGlobal = myISEManager.DeactivateISEModule(False)
                                End If

                                myGlobal = mdiAnalyzerCopy.RefreshISEAlarms()

                                IsActionCompleted = True
                                RaiseEvent ActivateScreenEvent(True, Messages.STANDBY)
                                MyClass.PrepareReadyForTestingMode()

                            Else
                                Me.PrepareErrorMode()
                            End If


                        Case OPERATIONS.SAVE_ISE_INSTALLATION
                            myGlobal = MyClass.UpdateAdjustments()
                            If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                                MyClass.myISEManager.IsISEModuleInstalled = True
                                myISEManager.DeactivateISEModule(False)

                                IsActionCompleted = True 'SGM 30/03/2012

                                myGlobal = mdiAnalyzerCopy.RefreshISEAlarms()

                                RaiseEvent ActivateScreenEvent(True, Messages.STANDBY)
                                MyClass.PrepareReadyForTestingMode()

                            Else
                                Me.PrepareErrorMode()
                            End If



                        Case OPERATIONS.INSTALL_ELECTRODES

                            'SGM 12/03/2012
                            If myISEManager.LastProcedureResult = ISEManager.ISEProcedureResult.OK Then
                                ''SGM 17/05/2012
                                'myISEManager.IsLiEnabledByUser = Me.UserLiSelectedAttr
                                ''Update Installation dates
                                'myGlobal = myISEManager.SetElectrodesInstallDates(Me.UserDateTimeElement1Attr, Me.UserDateTimeElement2Attr, Me.UserDateTimeElement3Attr, Me.UserDateTimeElement4Attr, Me.UserDateTimeElement5Attr)

                            Else
                                Me.DisplayISEInfo() 'SGM 21/09/2012 - show received string
                                myGlobal.HasError = True
                            End If
                            IsActionCompleted = Not myGlobal.HasError 'SGM 30/03/2012
                            MyClass.PrepareReadyForTestingMode()



                        Case OPERATIONS.INSTALL_REAGENT_PACK

                            'SGM 12/03/2012
                            If myISEManager.LastProcedureResult = ISEManager.ISEProcedureResult.OK Then
                                ' XBC 02/04/2012 - Refresh Changes
                                'myGlobal = myISEManager.SetReagentsPackInstallDate(DateTime.Now, True)
                                ' XBC 02/04/2012 - Refresh Changes
                            Else
                                myGlobal.HasError = True
                            End If

                            IsActionCompleted = Not myGlobal.HasError 'SGM 30/03/2012

                            'SGM 17/01/2012 Bug #1108 initialize after install rp
                            If IsActionCompleted Then
                                If Not MyClass.myISEManager.IsISEInitiatedOK Then
                                    Me.PrepareTestingMode()
                                    MyClass.DisplayMessage("")
                                    myText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "MSG_INITIALIZING_ISE", currentLanguage) + vbCrLf
                                    Me.AppendPcSendText(Me.BsRichTextBox1, myText)
                                    MyClass.CurrentOperation = OPERATIONS.INITIALIZE_ISE_MODULE
                                    MyClass.myISEAction = ISECommands.POLL
                                    myGlobal = myISEManager.DoGeneralCheckings
                                    If myGlobal.HasError Then
                                        Me.PrepareErrorMode()
                                    End If
                                    Exit Try
                                End If
                            Else
                                MyClass.PrepareReadyForTestingMode()
                            End If
                            'End SGM 17/01/2012

                            'MyClass.PrepareReadyForTestingMode()


                        Case OPERATIONS.INSTALL_FLUID_TUBING
                            myGlobal = myISEManager.SetFluidicTubingInstallDate(Me.UserDateTimeElement1Attr)
                            IsActionCompleted = Not myGlobal.HasError 'SGM 30/03/2012
                            MyClass.PrepareReadyForTestingMode()


                        Case OPERATIONS.INSTALL_PUMP_TUBING
                            myGlobal = myISEManager.SetPumpTubingInstallDate(Me.UserDateTimeElement1Attr)
                            IsActionCompleted = Not myGlobal.HasError 'SGM 30/03/2012
                            MyClass.PrepareReadyForTestingMode()


                        Case OPERATIONS.SET_LONG_TERM_DEACTIVATION

                            ' XBC 07/05/2012 - disable ISE preparations
                            'SGM 01/02/2012 - Check if it is User Assembly - Bug #1112
                            'If My.Application.Info.AssemblyName.ToUpper.Contains("SERVICE") Then
                            If Not GlobalBase.IsServiceAssembly Then
                                ' Sw User
                                If Not myISEManager.IsISEModuleReady AndAlso Not MyClass.WorkSessionIDAttribute.Trim = "" Then
                                    Dim myExecutionDelegate As New ExecutionsDelegate
                                    myGlobal = myExecutionDelegate.UpdateStatusByExecutionTypeAndStatus(Nothing, WorkSessionIDAttribute, MyClass.mdiAnalyzerCopy.ActiveAnalyzer, "PREP_ISE", "PENDING", "LOCKED")
                                End If
                            End If
                            ' XBC 07/05/2012

                            myGlobal = mdiAnalyzerCopy.RefreshISEAlarms() 'SGM 07/06/2012

                            IsActionCompleted = True
                            RaiseEvent ActivateScreenEvent(True, Messages.STANDBY)
                            MyClass.PrepareReadyForTestingMode()


                        Case OPERATIONS.CHECK_CLEAN_PACK
                            If myISEManager.LastProcedureResult <> ISEManager.ISEProcedureResult.OK Then
                                myGlobal.HasError = True
                            Else
                                ' XBC 07/05/2012 - disable ISE preparations
                                'SGM 01/02/2012 - Check if it is User Assembly - Bug #1112
                                'If Not My.Application.Info.AssemblyName.ToUpper.Contains("SERVICE") Then
                                If Not GlobalBase.IsServiceAssembly Then
                                    ' Sw User
                                    If Not myISEManager.IsISEModuleReady AndAlso Not MyClass.WorkSessionIDAttribute.Trim = "" Then
                                        Dim myExecutionDelegate As New ExecutionsDelegate
                                        myGlobal = myExecutionDelegate.UpdateStatusByExecutionTypeAndStatus(Nothing, WorkSessionIDAttribute, MyClass.mdiAnalyzerCopy.ActiveAnalyzer, "PREP_ISE", "PENDING", "LOCKED")
                                    End If
                                End If
                                ' XBC 07/05/2012
                            End If
                            IsActionCompleted = Not myGlobal.HasError 'SGM 30/03/2012
                            MyClass.PrepareReadyForTestingMode()

                        Case OPERATIONS.CLEANING
                            'SGM 12/03/2012
                            If myISEManager.LastProcedureResult = ISEManager.ISEProcedureResult.OK Then

                            Else
                                Me.DisplayISEInfo() 'SGM 21/09/2012 - show received string
                                myGlobal.HasError = True
                            End If
                            IsActionCompleted = Not myGlobal.HasError 'SGM 30/03/2012
                            MyClass.PrepareReadyForTestingMode()


                        Case OPERATIONS.ENABLE_ISE_PREPARATIONS
                            If myISEManager.LastProcedureResult = ISEManager.ISEProcedureResult.OK Then

                                ' XBC 20/03/2012 
                                'myISEManager.ActivateISEPreparations()
                                Dim myExecutionDelegate As New ExecutionsDelegate

                                ' Verify if exist any work session
                                If MyClass.myISEManager.IsISEModuleReady AndAlso _
                                    Not String.Compare(WorkSessionIDAttribute.Trim, "", False) = 0 AndAlso _
                                   String.Compare(Me.WSStatusAttribute, "EMPTY", False) <> 0 And _
                                   String.Compare(Me.WSStatusAttribute, "OPEN", False) <> 0 Then

                                    'Verify is the current Analyzer Status is RUNNING
                                    Dim createWSInRunning As Boolean = False
                                    If (Not mdiAnalyzerCopy Is Nothing) Then createWSInRunning = (mdiAnalyzerCopy.AnalyzerStatus = GlobalEnumerates.AnalyzerManagerStatus.RUNNING)

                                    'SGM 25/09/2012 - check if any calibration is needed
                                    Dim isReady As Boolean = False
                                    Dim myAffectedElectrodes As List(Of String)
                                    myGlobal = MyClass.myISEManager.CheckAnyCalibrationIsNeeded(myAffectedElectrodes)
                                    If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                                        isReady = Not (CBool(myGlobal.SetDatos) And myAffectedElectrodes Is Nothing)
                                    End If
                                    CreateLogActivity("Launch CreateWSExecutions !", Me.Name & ".PrepareTestedMode", EventLogEntryType.Information, False) 'AG 31/03/2014 - #1565
                                    myGlobal = myExecutionDelegate.CreateWSExecutions(Nothing, MyClass.mdiAnalyzerCopy.ActiveAnalyzer, MyClass.WorkSessionIDAttribute, _
                                                                                  createWSInRunning, -1, String.Empty, isReady, myAffectedElectrodes)
                                    If Not isReady Then Me.DisplayMessage(Messages.ISE_NOT_READY.ToString)
                                    'end SGM 25/09/2012
                                    myAffectedElectrodes = Nothing 'AG 19/02/2014 - #1514
                                Else
                                    IsActionCompleted = False

                                End If
                                ' XBC 20/03/2012
                                IsActionCompleted = True

                            Else
                                IsActionCompleted = False

                            End If
                            'Me.AppendAnalyzerResponseText(Me.BsRichTextBox1, myText)
                            MyClass.PrepareReadyForTestingMode()


                        Case OPERATIONS.PRIME_CALIBRATION 'SGM 11/06/2012
                            If myISEManager.LastProcedureResult = ISEManager.ISEProcedureResult.OK Then

                            Else
                                myGlobal.HasError = True
                            End If
                            MyClass.DisplayISEInfo()
                            IsActionCompleted = Not myGlobal.HasError
                            MyClass.PrepareReadyForTestingMode()

                        Case OPERATIONS.PRIME_X2_CALIBRATION 'SGM 11/06/2012
                            If myISEManager.LastProcedureResult = ISEManager.ISEProcedureResult.OK Then

                            Else
                                myGlobal.HasError = True
                            End If
                            MyClass.DisplayISEInfo()
                            IsActionCompleted = Not myGlobal.HasError
                            MyClass.PrepareReadyForTestingMode()

                        Case OPERATIONS.QUIT_UTILITIES
                            If myISEManager.LastProcedureResult = ISEManager.ISEProcedureResult.OK Then
                                'If My.Application.Info.AssemblyName.ToUpper.Contains("SERVICE") Then
                                '    SendISECMD(ISECommands.DEBUG_mV_OFF)
                                'End If
                                'RaiseEvent ActivateScreenEvent(True, Messages.STANDBY)
                                'Me.Close()
                            Else
                                'in case of error during final purging
                                'RaiseEvent ActivateScreenEvent(True, Messages.STANDBY)
                                'Me.Close()
                                'Me.PrepareErrorMode()
                            End If

                            RaiseEvent ActivateScreenEvent(True, Messages.PERFORMING_ISE)
                            MyClass.DisplayISEInfo()

                            'SGM 04/07/2012
                            If MyClass.IsR2ArmAwayFromParking Then
                                Me.PrepareTestingMode()
                                MyClass.DisplayMessage("")
                                myText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_ISE_R2_BACK", currentLanguage) + vbCrLf
                                Me.AppendPcSendText(Me.BsRichTextBox1, myText)
                                MyClass.CurrentOperation = OPERATIONS.R2_TO_PARKING
                                MyClass.myISEAction = ISECommands.R2_TO_PARKING
                                myGlobal = myISEManager.MoveR2ArmBack
                                If myGlobal.HasError Then
                                    Me.PrepareErrorMode()
                                End If
                                Exit Try
                            Else
                                RaiseEvent ActivateScreenEvent(True, Messages.STANDBY)
                                Me.Close()
                            End If
                            'SGM 04/07/2012


                        Case OPERATIONS.CHECK_ALARMS 'SGM 26/10/2012
                            Dim myPendingCal As New List(Of ISEManager.MaintenanceOperations)
                            myGlobal = myISEManager.GetISEAlarmsForUtilities(myPendingCal)
                            If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                                Dim myAlarms As List(Of Alarms) = CType(myGlobal.SetDatos, List(Of Alarms))
                                Me.AppendISEAlarmsText(Me.BsRichTextBox1, myAlarms, myPendingCal)
                            End If
                            IsActionCompleted = True
                            RaiseEvent ActivateScreenEvent(True, Messages.STANDBY)
                            MyClass.PrepareReadyForTestingMode()
                            Exit Sub
                    End Select

                    'SGM 29/06/2012
                    If myISEManager.LastProcedureResult <> ISEManager.ISEProcedureResult.Exception Then
                        MyClass.PrepareReadyForTestingMode()
                    End If

                End If

            End If


            If Me.CurrentOperation <> OPERATIONS.NONE Then
                If Not Me.SimulationMode Then

                    ' XBC 30/08/2012 - Correction : No save consumptions when Reagents Pack is installed
                    If Me.CurrentOperation <> OPERATIONS.INSTALL_REAGENT_PACK Then

                        ' XBC 27/08/2012 - Correction : Save consumptions after Current Procedure is finished
                        If myISEManager.CurrentProcedureIsFinished And _
                           myISEManager.IsISEInitiatedOK And _
                           myISEManager.IsReagentsPackReady Then

                            ' XBC 04/04/2012
                            If myISEManager.IsCalAUpdateRequired Or myISEManager.IsCalBUpdateRequired Then
                                Me.DisableAll() ' XB 28/04/2014 - Disable screen when saving consumptions
                                myText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SavingData", currentLanguage) + vbCrLf
                                Me.AppendPcSendText(Me.BsRichTextBox1, myText)

                                myGlobal = myISEManager.SaveConsumptions()
                                If myGlobal.HasError Then
                                    Me.PrepareErrorMode()
                                End If
                                Exit Try
                            End If
                            ' XBC 04/04/2012

                        End If

                    End If

                    'SGM 04/07/2012
                    If MyClass.CurrentOperation = OPERATIONS.INITIALIZE_ISE_MODULE Then
                        'If MyClass.CurrentOperation <> OPERATIONS.SAVE_ISE_INSTALLATION Then
                        If Not MyClass.IsR2ArmAwayFromParking Then
                            If myISEManager.LastProcedureResult <> ISEManager.ISEProcedureResult.Exception Then
                                If myISEManager.LastProcedureResult <> ISEManager.ISEProcedureResult.CancelError Then
                                    Me.PrepareTestingMode()
                                    MyClass.DisplayMessage("")
                                    myText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_ISE_R2_OUT", currentLanguage) + vbCrLf
                                    Me.AppendPcSendText(Me.BsRichTextBox1, myText)
                                    MyClass.CurrentOperation = OPERATIONS.R2_TO_WASHING
                                    MyClass.myISEAction = ISECommands.R2_TO_WASHING
                                    MyClass.myParentMDI.Refresh()
                                    RaiseEvent ActivateScreenEvent(False, Messages.PERFORMING_ISE)
                                    Application.DoEvents()
                                    myGlobal = myISEManager.MoveR2ArmAway
                                    If myGlobal.HasError Then
                                        Me.PrepareErrorMode()
                                    End If
                                    Exit Try
                                End If
                            End If
                        End If
                    End If


                    'SGM 04/07/2012
                End If
            End If

            If Me.CurrentOperation <> OPERATIONS.QUIT_UTILITIES And Me.CurrentOperation <> OPERATIONS.R2_TO_PARKING Then
                RaiseEvent ActivateScreenEvent(True, Messages.STANDBY)
            Else
                If MyClass.myISEManager.LastProcedureResult = ISEManager.ISEProcedureResult.Exception Then
                    myText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "MSG_SRV_NOT_COMPLETED", currentLanguage) + vbCrLf + vbCrLf
                    If Not myISEManager.IsISECommsOk Then
                        Dim myISEnoAnswerText As String = myMultiLangResourcesDelegate.GetResourceText(Nothing, "ISE_NO_ANSWER", currentLanguage)
                        myText = myText.Trim + " - " + myISEnoAnswerText + vbCrLf
                    End If
                    If myText.Length > 0 Then
                        Me.AppendAnalyzerResponseText(Me.BsRichTextBox1, myText, ISEManager.ISEProcedureResult.Exception, True)
                    End If

                    RaiseEvent ActivateScreenEvent(True, Messages.STANDBY)

                    Application.DoEvents()
                    Me.EnableAll()
                    Me.BsExitButton.Enabled = True
                    Me.BsAdjustButton.Enabled = False

                    Exit Try

                End If
            End If

            If Me.SimulationMode Then
                If IsActionCompleted Then
                    myText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "MSG_SRV_COMPLETED", currentLanguage) '+ vbCrLf + vbCrLf
                    Me.DisplayMessage(Messages.SRV_COMPLETED.ToString)
                Else
                    myText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "MSG_SRV_NOT_COMPLETED", currentLanguage) '+ vbCrLf + vbCrLf
                    Me.DisplayMessage(Messages.SRV_NOT_COMPLETED.ToString)
                End If

            Else

                'SGM 30/03/2012
                If Me.CurrentOperation <> OPERATIONS.NONE And Me.CurrentOperation <> OPERATIONS.QUIT_UTILITIES Then
                    If IsActionCompleted Then
                        If MyClass.myISEManager.LastProcedureResult <> ISEManager.ISEProcedureResult.None Then
                            Dim isCalibration As Boolean = (MyClass.myISEManager.LastISEResult.ISEResultType = ISEResultTO.ISEResultTypes.CAL)
                            Select Case MyClass.myISEManager.LastProcedureResult
                                Case ISEManager.ISEProcedureResult.OK : myText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "MSG_SRV_COMPLETED", currentLanguage) '+ vbCrLf '+ vbCrLf
                                Case ISEManager.ISEProcedureResult.NOK
                                    myText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "MSG_SRV_NOT_COMPLETED", currentLanguage)
                                    If MyClass.myISEManager.LastISEResult.Errors.Count > 0 Then
                                        myText = myText & ":"

                                        For Each E As ISEErrorTO In MyClass.myISEManager.LastISEResult.Errors
                                            If E.IsCancelError Then
                                                'If E.CancelErrorDesc.Length > 0 Then 'QUITAR cuando se implemente tratamiento errores ERC
                                                Dim myDesc = MyClass.myISEManager.GetISEErrorDescription(E, False, isCalibration)
                                                myText = myText & vbCrLf & myDesc
                                                Exit For
                                                'End If
                                            Else
                                                Dim myDesc = MyClass.myISEManager.GetISEErrorDescription(E, False, isCalibration)
                                                myText = myText & vbCrLf & myDesc & " (" & ISEErrorTO.FormatAffected(E.Affected) & ")"
                                            End If
                                        Next
                                    End If

                                Case ISEManager.ISEProcedureResult.CancelError
                                    myText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "MSG_SRV_NOT_COMPLETED", currentLanguage)
                                    If MyClass.myISEManager.LastISEResult.IsCancelError AndAlso MyClass.myISEManager.LastISEResult.Errors.Count > 0 Then
                                        For Each E As ISEErrorTO In MyClass.myISEManager.LastISEResult.Errors
                                            If E.IsCancelError Then
                                                Dim myDesc = MyClass.myISEManager.GetISEErrorDescription(E, False, isCalibration)
                                                myText = myText & ":" & vbCrLf & myDesc
                                                Exit For
                                            End If
                                        Next
                                    End If

                                Case ISEManager.ISEProcedureResult.Exception : myText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "MSG_SRV_NOT_COMPLETED", currentLanguage) '+ vbCrLf '+ vbCrLf
                            End Select
                        Else
                            If pResponse = RESPONSE_TYPES.OK Then
                                myText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "MSG_SRV_COMPLETED", currentLanguage) '+ vbCrLf '+ vbCrLf
                            Else
                                myText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "MSG_SRV_NOT_COMPLETED", currentLanguage) '+ vbCrLf '+ vbCrLf
                            End If
                        End If
                        'myText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "MSG_SRV_COMPLETED", currentLanguage) + vbCrLf + vbCrLf
                        Me.DisplayMessage(Messages.SRV_COMPLETED.ToString)
                    Else
                        myText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "MSG_SRV_NOT_COMPLETED", currentLanguage) '+ vbCrLf + vbCrLf
                        Me.DisplayMessage(Messages.SRV_NOT_COMPLETED.ToString)
                    End If

                End If
                'end SGM 30/03/2012

            End If

            If myText.Length > 0 Then
                If Not myISEManager.IsISESwitchON Then
                    If Not MyClass.IsSwitchOffInformed Then
                        Dim myISEOffText As String = myMultiLangResourcesDelegate.GetResourceText(Nothing, "ISE_OFF_ERR", currentLanguage)
                        myText = myText + " - " + myISEOffText + vbCrLf
                        MyClass.IsSwitchOffInformed = True
                    End If
                ElseIf Not myISEManager.IsISECommsOk Then
                    If Me.CurrentOperation <> OPERATIONS.SAVE_ISE_INSTALLATION Then 'SGM 21/09/2012
                        Dim myISEnoAnswerText As String = myMultiLangResourcesDelegate.GetResourceText(Nothing, "ISE_NO_ANSWER", currentLanguage)
                        myText = myText + " - " + myISEnoAnswerText + vbCrLf
                    End If
                Else
                    myText = myText + vbCrLf
                End If
                Me.AppendAnalyzerResponseText(Me.BsRichTextBox1, myText + vbCrLf)
            End If

            If Me.CurrentOperation <> OPERATIONS.SAVE_ISE_INSTALLATION Then 'SGM 21/09/2012
                'SGM 05/09/2012 - not to stop the sequence when cancel error occured
                If MyClass.myISEManager.LastISEResult IsNot Nothing AndAlso MyClass.myISEManager.LastISEResult.IsCancelError Then
                    If Me.NumRepetitionsSelectedByUser >= 1 Then
                        Me.NumRepetitionsSelectedByUser -= 1
                        If Me.NumRepetitionsSelectedByUser > 0 Then
                            Me.BsStopButton.Visible = True
                            Me.SendISECMD(Me.CurrentISECommand.ISECommandID)
                            Exit Try
                        End If
                    End If
                End If
                'end SGM 05/09/2012
            End If

            'SGM 15/10/2012
            ''special case in which is LongTermDeactivation and the user has performed an action from Installation Group
            Dim IsInInstallActivateBlock As Boolean = False
            If (MyClass.CurrentBlockNode IsNot Nothing AndAlso MyClass.CurrentBlockNode.Tag IsNot Nothing) Then
                IsInInstallActivateBlock = (CStr(MyClass.CurrentBlockNode.Tag) = "ISE_ACT_USR_ACT" Or CStr(MyClass.CurrentBlockNode.Tag) = "ISE_ACT_SRV_ACT")
            End If
            If (MyClass.CurrentOperation = OPERATIONS.ENABLE_ISE_PREPARATIONS Or IsInInstallActivateBlock) And Not myISEManager.IsISEInitiatedOK Then
                'If MyClass.CurrentOperation = OPERATIONS.ENABLE_ISE_PREPARATIONS And Not myISEManager.IsISEInitiatedOK Then
                Dim myISEPdtText As String = myMultiLangResourcesDelegate.GetResourceText(Nothing, "ISE_CONNECT_PDT", currentLanguage) + vbCrLf + vbCrLf
                Me.AppendAnalyzerResponseText(Me.BsRichTextBox1, myISEPdtText, ISEManager.ISEProcedureResult.NOK)
            End If
            'end SGM 15/10/2012

            'Reevaluate if the same action can be performed before enabling again the action button SGM 09/05/2012
            If Not MyClass.CurrentActionNode Is Nothing AndAlso Me.BSActionsTreeImageList IsNot Nothing AndAlso Me.BsActionsTreeView IsNot Nothing Then

                'SGM 11/06/2012
                If myISEManager.LastProcedureResult = ISEManager.ISEProcedureResult.OK Then
                    MyClass.CurrentActionNode.ImageKey = "DONE"
                    MyClass.CurrentActionNode.SelectedImageKey = "DONE"
                    MyClass.CurrentActionNode.ForeColor = MyClass.CurrentActionNode.Parent.ForeColor
                ElseIf myISEManager.LastProcedureResult <> ISEManager.ISEProcedureResult.None Then
                    MyClass.CurrentActionNode.ImageKey = "ACTION"
                    MyClass.CurrentActionNode.SelectedImageKey = "ACTION"
                    MyClass.CurrentActionNode.ForeColor = Color.Black
                Else
                    If pResponse = RESPONSE_TYPES.OK Then
                        MyClass.CurrentActionNode.ImageKey = "DONE"
                        MyClass.CurrentActionNode.SelectedImageKey = "DONE"
                        MyClass.CurrentActionNode.ForeColor = MyClass.CurrentActionNode.Parent.ForeColor
                    Else
                        MyClass.CurrentActionNode.ImageKey = "ACTION"
                        MyClass.CurrentActionNode.SelectedImageKey = "ACTION"
                        MyClass.CurrentActionNode.ForeColor = Color.Black
                    End If
                End If

                'check if all items in the group are already done
                Dim AreAllDone As Boolean = True
                For Each N As TreeNode In MyClass.CurrentBlockNode.Nodes
                    If N.ImageKey <> "DONE" Then AreAllDone = False : Exit For
                Next

                AreAllDone = AreAllDone And (Me.NumRepetitionsSelectedByUser = 0)

                If AreAllDone Then
                    MyClass.CurrentBlockNode.ImageKey = "DONE"
                    MyClass.CurrentBlockNode.SelectedImageKey = "DONE"
                End If
                'end SGM 11/06/2012

                ''SGM 21/01/2012 - Show alarms after every action from Installation Group -PENDING TO IMPLEMENT
                'If MyClass.IsInstallGroupNodeActive Then
                '    'Show current Alarms
                '    Dim myOperation As OPERATIONS = MyClass.CurrentOperation
                '    MyClass.CurrentOperation = OPERATIONS.CHECK_ALARMS
                '    MyClass.PrepareTestedMode()
                '    MyClass.CurrentOperation = myOperation
                'End If
                ''end SGM 21/01/2012

                ' XBC 27/07/2012 - Only for no re-activate the action node that is just completed
                If Me.BsActionsTreeView.AccessibleDescription Is Nothing Then

                    Exit Try
                End If

                Dim myISECommand As New ISECommandTO()
                Me.BsAdjustButton.Enabled = False

                If MyClass.CurrentActionNode.Tag.ToString = "ISE_ALMS" Then
                    Me.BsAdjustButton.Enabled = True
                Else
                    If MyClass.ValidateNodeByBlock(MyClass.CurrentActionNode) Then
                        myGlobal = MyClass.ValidateActionNode(MyClass.CurrentActionNode.Tag.ToString)
                        If (Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing) Then
                            myISECommand = CType(myGlobal.SetDatos, ISECommandTO)
                            If myISECommand.ISECommandID <> ISECommands.NONE Then
                                'SIMPLE ACTIOn NODE
                                'MyClass.PrepareTimesUpDown(myISECommand.ISECommandID.ToString)
                                Me.BsAdjustButton.Enabled = MyClass.ValidateISEAvailability
                            Else
                                'COMPLEX ACTION NODE
                                If MyClass.ValidateISEAction(MyClass.CurrentActionNode) Then
                                    Me.BsAdjustButton.Enabled = MyClass.ValidateISEAvailability
                                End If
                                Me.BsTimesUpDown.Value = 1
                            End If
                        End If
                    End If
                End If

                Me.BsActionsTreeView.SelectedNode = MyClass.CurrentActionNode
                'Me.BsActionsTreeView.Refresh()
                Me.BsActionsTreeView.Focus()
            End If
            'END SGM 09/05/2012

            'If MyClass.IsInstallGroupNodeActive Then
            '    'Show current Alarms
            '    MyClass.CurrentOperation = OPERATIONS.CHECK_ALARMS
            '    MyClass.PrepareTestedMode()
            '    MyClass.CurrentOperation = OPERATIONS.NONE
            'End If

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".PrepareTestedMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareTestedMode ", Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
            'Me.ReportHistoryError()    PDT !!!
        Finally
            Me.Cursor = Cursors.Default 'SGM 07/06/2012
        End Try
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>Created by SGM 15/03/2012</remarks>
    Private Sub PrepareReadyForTestingMode()
        Try
            Me.Cursor = Cursors.Default
            Me.DisplayMessage(Messages.SRV_TEST_READY.ToString)

            Me.BsExitButton.Enabled = True
            Me.BsActionsTreeView.Enabled = True
            Me.BsClearButton.Enabled = True
            Me.BsSaveAsButton.Enabled = True

            Me.FeaturesGroupBox.Enabled = True
            'RaiseEvent ActivateScreenEvent(True)

            Me.BsAdjustButton.Visible = True
            Me.BsStopButton.Visible = False

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".PrepareReadyForTestingMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareReadyForTestingMode ", Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub


    ''' <summary>
    ''' Prepare GUI for Error Mode
    ''' </summary>
    ''' <remarks>
    ''' Created by XBC
    ''' Modified by XBC - Change to Public to can be called outside from MDI
    ''' </remarks>
    Public Sub PrepareErrorMode(Optional ByVal pAlarmType As ManagementAlarmTypes = ManagementAlarmTypes.NONE)
        Try
            Me.Cursor = Cursors.Default
            ScreenWorkingProcess = False

            Me.ProgressBar1.Visible = False
            DisableAll()
            Me.BsExitButton.Enabled = True ' Just Exit button is enabled in error case

            Me.DisplayMessage(Messages.ISE_NOT_READY.ToString)

            MyClass.MaintenanceExitNeeded = False

            'Me.ReportHistoryError()         ' PDT !!!

            'SGM 23/10/2012
            If pAlarmType <> ManagementAlarmTypes.NONE Then
                Me.IsR2ArmAwayFromParking = False
                Me.BsActionsTreeView.Enabled = False
                Me.BsAdjustButton.Enabled = False
                Me.BsExitButton.Enabled = True
                Me.DisplayMessage("")
            End If
            'end SGM 23/10/2012

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".PrepareErrorMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareErrorMode ", Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        Finally
            Me.Cursor = Cursors.Default
        End Try
    End Sub

    ''' <summary>
    ''' Stops the operation currently being performed due to an incoming alarm
    ''' </summary>
    ''' <param name="pAlarmType"></param>
    ''' <remarks>Created by SGM 19/10/2012</remarks>
    Public Sub StopCurrentOperation(Optional ByVal pAlarmType As GlobalEnumerates.ManagementAlarmTypes = ManagementAlarmTypes.NONE)
        Try
            MyClass.PrepareErrorMode(pAlarmType)

            MyClass.CurrentAlarmType = pAlarmType

            If MyClass.CurrentOperation <> OPERATIONS.NONE Then
                If MyClass.myISEManager.CurrentProcedure <> ISEManager.ISEProcedures.None Then
                    MyClass.myISEManager.AbortCurrentProcedureDueToException()
                End If
                MyClass.CurrentOperation = OPERATIONS.NONE
            End If

            RaiseEvent StopCurrentOperationFinished(pAlarmType)

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".StopCurrentOperation ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".StopCurrentOperation ", Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        Finally
            Me.Cursor = Cursors.Default
        End Try
    End Sub


    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>
    ''' Modified by XB 04/02/2013 - Upper conversions redundants because the value is already in UpperCase must delete to avoid Regional Settings problems (Bugs tracking #1112)
    ''' </remarks>
    Private Sub DisplayISEInfo()
        Dim myText As String
        Try

            Dim myISEResult As ISEResultTO

            If Me.SimulationMode Then

                myISEResult = New ISEResultTO()

                Select Case myISEAction
                    Case ISECommands.POLL, ISECommands.DSPA, ISECommands.DSPB, _
                        ISECommands.PRIME_CALA, ISECommands.PRIME_CALB, _
                        ISECommands.PURGEA, ISECommands.PURGEB, _
                        ISECommands.MAINTENANCE, _
                        ISECommands.R2_TO_PARKING, ISECommands.R2_TO_WASHING

                        myISEResult.ReceivedResults = "<ISE!>"

                    Case ISECommands.CALB, ISECommands.LAST_SLOPES
                        myISEResult.ReceivedResults = "<CAL Li 26.25 Na 369.2 K 88.96 Cl 123.05>"

                    Case ISECommands.READ_mV
                        myISEResult.ReceivedResults = "<AMV Li 242.9 Na 216.5 K 184.1 Cl 201.2>"

                    Case ISECommands.PUMP_CAL
                        myISEResult.ReceivedResults = "<PMC A 3000 B 2000 W 1000>"

                    Case ISECommands.READ_PAGE_0_DALLAS
                        myISEResult.ReceivedResults = "<DSN 0990BFCE060000A0~><DDT 00 B25000640029057800009C1DA202BC019003208204E2019A344054010E1F13DBã>"

                    Case ISECommands.READ_PAGE_1_DALLAS
                        myISEResult.ReceivedResults = "<DSN 0990BFCE060000A0~><DDT 01 FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFý>"

                    Case ISECommands.BUBBLE_CAL, ISECommands.SHOW_BUBBLE_CAL
                        myISEResult.ReceivedResults = "<BBC A 111 M 222 L 333>"

                    Case ISECommands.PUMP_CAL, ISECommands.SHOW_PUMP_CAL
                        myISEResult.ReceivedResults = "<PMC A 3000 B 2000 W 1000>"

                End Select

            Else

                myISEResult = mdiAnalyzerCopy.ISE_Manager.LastISEResult

            End If


            If myISEResult IsNot Nothing Then
                With myISEResult

                    Dim myResultsToDisplay As String = .ReceivedResults

                    'Not to display if not biosystems user SGM 09/05/2012
                    'If String.Compare(MyBase.CurrentUserLevel.ToUpper, USER_LEVEL.lBIOSYSTEMS.ToString.ToUpper, False) <> 0 Then
                    If String.Compare(MyBase.CurrentUserLevel, USER_LEVEL.lBIOSYSTEMS.ToString, False) <> 0 Then
                        If myISEAction = ISECommands.READ_PAGE_0_DALLAS Then
                            myResultsToDisplay = "************************"
                        ElseIf myISEAction = ISECommands.READ_PAGE_1_DALLAS Then
                            myResultsToDisplay = "************************"
                        ElseIf MyClass.myISEManager.CurrentCommandTO IsNot Nothing Then
                            If MyClass.myISEManager.CurrentCommandTO.ISECommandID = ISECommands.READ_PAGE_0_DALLAS Then
                                myResultsToDisplay = "************************"
                            ElseIf MyClass.myISEManager.CurrentCommandTO.ISECommandID = ISECommands.READ_PAGE_1_DALLAS Then
                                myResultsToDisplay = "************************"
                            End If
                        End If
                    End If
                    'end SGM 09/05/2012

                    'Me.BsRichTextBox1.Text += "A400 << " + .ReceivedResults + vbCrLf + vbCrLf
                    myText = MyBase.AnalyzerModel & " << " + myResultsToDisplay + vbCrLf '+ vbCrLf
                    Me.AppendAnalyzerResponseText(Me.BsRichTextBox1, myText)

                End With
            End If

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".DisplayISEInfo ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".DisplayISEInfo ", Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub AppendPcSendText(ByVal RTC As RichTextBox, ByVal text As String)
        Try
            Dim isMoreThanOne As Boolean = (NumRepetitionsSelectedByUser > 0 And Me.BsTimesUpDown.Value > 1)
            Dim countText As String = ""
            If isMoreThanOne Then
                countText = " (" & (Me.BsTimesUpDown.Value - NumRepetitionsSelectedByUser + 1).ToString & "/" & Me.BsTimesUpDown.Value.ToString & ")"
            End If


            With RTC
                .Select(.TextLength, 0)
                .SelectionFont = New Font(.SelectionFont, FontStyle.Regular)
                .SelectionColor = Color.SteelBlue
                If .Text.EndsWith(vbCrLf & vbCrLf) Then
                    .AppendText(text & countText & vbCrLf)
                Else
                    .AppendText(vbCrLf & text & countText & vbCrLf)
                End If
            End With

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".AppendPcSendText ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".AppendPcSendText ", Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub AppendAnalyzerResponseText(ByVal RTC As RichTextBox, ByVal text As String, Optional ByVal pForcedResult As ISEManager.ISEProcedureResult = ISEManager.ISEProcedureResult.None, Optional ByVal pForceWrite As Boolean = False)
        Try

            If Not pForceWrite And Me.IsScreenCloseRequested Then Exit Sub

            'Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate
            Dim myColor As System.Drawing.Color

            If Me.SimulationMode Then
                myColor = Color.DarkOliveGreen
            Else

                Dim myResult As ISEManager.ISEProcedureResult
                If pForcedResult <> ISEManager.ISEProcedureResult.None Then
                    myResult = pForcedResult
                Else
                    myResult = MyClass.myISEManager.LastProcedureResult
                End If

                myColor = Color.DarkOliveGreen
                Select Case myResult
                    Case ISEManager.ISEProcedureResult.OK : myColor = Color.DarkOliveGreen
                    Case ISEManager.ISEProcedureResult.NOK : myColor = Color.OrangeRed : Me.DisplayMessage(Messages.RESULT_ERROR.ToString) ': text += myMultiLangResourcesDelegate.GetResourceText(Nothing, "MSG_RESULT_ERROR", currentLanguage)
                    Case ISEManager.ISEProcedureResult.CancelError : myColor = Color.OrangeRed : Me.DisplayMessage(Messages.SRV_NOT_COMPLETED.ToString)
                    Case ISEManager.ISEProcedureResult.Exception : myColor = Color.OrangeRed : Me.DisplayMessage(Messages.SRV_NOT_COMPLETED.ToString)
                End Select


            End If


            With RTC
                .Select(.TextLength, 0)
                .SelectionFont = New Font(.SelectionFont, FontStyle.Bold)
                .SelectionColor = myColor
                .AppendText(text)
            End With

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".AppendAnalyzerResponseText ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".AppendAnalyzerResponseText ", Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Displays the current ISE Alarms in the results text
    ''' </summary>
    ''' <param name="RTC"></param>
    ''' <param name="pAlarmsList"></param>
    ''' <remarks></remarks>
    Private Sub AppendISEAlarmsText(ByVal RTC As RichTextBox, ByVal pAlarmsList As List(Of GlobalEnumerates.Alarms), _
                                    ByVal pPendingCalibrations As List(Of ISEManager.MaintenanceOperations))
        Try

            Dim myTextList As New List(Of String)

            Dim myGlobal As New GlobalDataTO

            If pAlarmsList IsNot Nothing AndAlso pAlarmsList.Count > 0 Then

                'get alarms definitions
                Dim myAlarmsDelegate As New AlarmsDelegate
                myGlobal = myAlarmsDelegate.ReadAll(Nothing)
                If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                    Dim myAlarmsDS As AlarmsDS = CType(myGlobal.SetDatos, AlarmsDS)
                    If myAlarmsDS IsNot Nothing Then
                        For Each A As Alarms In pAlarmsList

                            Dim myAlarmsDesc As List(Of AlarmsDS.tfmwAlarmsRow)
                            myAlarmsDesc = (From d As AlarmsDS.tfmwAlarmsRow In myAlarmsDS.tfmwAlarms _
                                            Where d.AlarmID = A.ToString Select d).ToList()


                            If myAlarmsDesc.Count > 0 Then
                                myTextList.Add(myAlarmsDesc(0).Name)
                            End If
                        Next
                    End If
                End If

            End If

            'get pending calibrations
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate
            If pPendingCalibrations IsNot Nothing AndAlso pPendingCalibrations.Count > 0 Then
                For Each C As ISEManager.MaintenanceOperations In pPendingCalibrations
                    Dim myCalText As String = ""
                    Select Case C
                        Case ISEManager.MaintenanceOperations.ElectrodesCalibration : myCalText = "CALB: "
                        Case ISEManager.MaintenanceOperations.PumpsCalibration : myCalText = "PMCL: "
                        Case ISEManager.MaintenanceOperations.BubbleCalibration : myCalText = "BBCL: "
                    End Select

                    If myCalText.Length > 0 Then
                        myCalText &= myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_ISE_CALIB_RECOMMENDED", currentLanguage)
                        myTextList.Add(myCalText)
                    End If

                Next
            End If

            'build text
            Dim myText As String = ""
            If myTextList.Count > 0 Then
                'myText &= vbCrLf
                For Each T As String In myTextList
                    myText &= T & vbCrLf
                Next
            End If

            'display text
            If myText.Length > 0 Then
                With RTC
                    .Select(.TextLength, 0)
                    .SelectionFont = New Font(.SelectionFont, FontStyle.Bold)
                    .SelectionColor = Color.OrangeRed
                    .AppendText(myText)
                End With
            End If



        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".AppendISEAlarmsText ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".AppendISEAlarmsText ", Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="pISEActionBlock"></param>
    ''' <returns></returns>
    ''' <remarks>Created by SGM 15/03/2012</remarks>
    Private Function LoadActionTreeViewNode(ByVal pISEActionBlock As PreloadedMasterDataEnum) As GlobalDataTO
        Dim myGlobal As New GlobalDataTO
        Try


            Dim myPreMasterDataDS As New PreloadedMasterDataDS
            Dim myPreloadedMasterDataDelegate As New PreloadedMasterDataDelegate
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            'Load captions
            Dim myActionCaption As String = ""
            Select Case pISEActionBlock
                'Case PreloadedMasterDataEnum.ISE_ACT_USR_INITIAL : myActionCaption = "LBL_ISE_GROUP_Initialize"
                Case PreloadedMasterDataEnum.ISE_ACT_USR_ACT : myActionCaption = "LBL_ISE_GROUP_Activate" 'SGM 11/06/2012
                Case PreloadedMasterDataEnum.ISE_ACT_USR_GENERAL : myActionCaption = "LBL_Tests_General"
                Case PreloadedMasterDataEnum.ISE_ACT_USR_CALIBR : myActionCaption = "LBL_ISE_GROUP_Calibrate"
                Case PreloadedMasterDataEnum.ISE_ACT_USR_INS_REAG : myActionCaption = "LBL_ISE_GROUP_InstallReagentPack"
                Case PreloadedMasterDataEnum.ISE_ACT_USR_INS_ELEC : myActionCaption = "LBL_ISE_GROUP_InstallElectrodes"
                Case PreloadedMasterDataEnum.ISE_ACT_USR_DEACT : myActionCaption = "LBL_ISE_GROUP_LongSD"
                Case PreloadedMasterDataEnum.ISE_ACT_USR_PUMP_TUB : myActionCaption = "LBL_ISE_GROUP_PumpTubing"
                Case PreloadedMasterDataEnum.ISE_ACT_USR_FLUI_TUB : myActionCaption = "LBL_ISE_GROUP_FluidTubing"
                Case PreloadedMasterDataEnum.ISE_ACT_USR_PREPOK : myActionCaption = "MENU_WorkSession" ' "LBL_ISE_GROUP_IsePreparations"    ' XBC 29/05/2012
                Case PreloadedMasterDataEnum.ISE_ACT_USR_ALM : myActionCaption = "LBL_ISE_ALARMS" 'SGM 26/10/2012

                    'Case PreloadedMasterDataEnum.ISE_ACT_USR_INITIAL : myActionCaption = "LBL_ISE_GROUP_Initialize"
                Case PreloadedMasterDataEnum.ISE_ACT_SRV_ACT : myActionCaption = "LBL_ISE_GROUP_Activate" 'SGM 11/06/2012
                Case PreloadedMasterDataEnum.ISE_ACT_SRV_INITIAL : myActionCaption = "LBL_ISE_Initialization"
                Case PreloadedMasterDataEnum.ISE_ACT_SRV_REQUEST : myActionCaption = "LBL_ISE_GROUP_Request"
                Case PreloadedMasterDataEnum.ISE_ACT_SRV_GENERAL : myActionCaption = "LBL_Tests_General"
                Case PreloadedMasterDataEnum.ISE_ACT_SRV_CALIBR : myActionCaption = "LBL_ISE_GROUP_Calibrate"
                    'Case PreloadedMasterDataEnum.ISE_ACT_SRV_INS_MODL : myActionCaption = "LBL_ISE_GROUP_InstallISE"
                Case PreloadedMasterDataEnum.ISE_ACT_SRV_INS_REAG : myActionCaption = "LBL_ISE_GROUP_InstallReagentPack"
                Case PreloadedMasterDataEnum.ISE_ACT_SRV_INS_ELEC : myActionCaption = "LBL_ISE_GROUP_InstallElectrodes"
                Case PreloadedMasterDataEnum.ISE_ACT_SRV_DEACT : myActionCaption = "LBL_ISE_GROUP_LongSD"
                Case PreloadedMasterDataEnum.ISE_ACT_SRV_PUMP_TUB : myActionCaption = "LBL_ISE_GROUP_PumpTubing"
                Case PreloadedMasterDataEnum.ISE_ACT_SRV_FLUI_TUB : myActionCaption = "LBL_ISE_GROUP_FluidTubing"
                Case PreloadedMasterDataEnum.ISE_ACT_SRV_PREPOK : myActionCaption = "MENU_WorkSession" ' "LBL_ISE_GROUP_IsePreparations"    ' XBC 29/05/2012
                Case PreloadedMasterDataEnum.ISE_ACT_SRV_ALM : myActionCaption = "LBL_ISE_ALARMS" 'SGM 26/10/2012

                Case Else : Return myGlobal
            End Select

            If myActionCaption.Trim.Length > 0 Then

                Dim myGroupNode As TreeNode
                myGlobal = myPreloadedMasterDataDelegate.GetList(Nothing, pISEActionBlock)
                If (Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing) Then
                    myPreMasterDataDS = CType(myGlobal.SetDatos, PreloadedMasterDataDS)

                    Dim qActionTypes As New List(Of PreloadedMasterDataDS.tfmwPreloadedMasterDataRow)

                    qActionTypes = (From a In myPreMasterDataDS.tfmwPreloadedMasterData _
                                    Order By a.Position _
                                    Select a).ToList()

                    If qActionTypes.Count > 0 Then
                        ' Add Item GROUP to Nodes Tree
                        myGroupNode = New TreeNode(myMultiLangResourcesDelegate.GetResourceText(Nothing, myActionCaption, currentLanguage))
                        BsActionsTreeView.Nodes.Add(myGroupNode)
                        myGroupNode.Tag = pISEActionBlock.ToString
                        myGroupNode.ImageKey = "NONE"


                        For i As Integer = 0 To qActionTypes.Count - 1

                            'PENDING until validate conditioning after activation SGM 11/06/2012
                            If String.Compare(qActionTypes.Item(i).ItemID, "ACT_ISE_CALB", False) <> 0 Then
                                'Me.AddISECommand(qActionTypes(i).ItemID)
                                ' Add Item ACTIONS to Nodes Tree
                                Dim ActionNode As New TreeNode()
                                ActionNode = myGroupNode.Nodes.Add(qActionTypes.Item(i).FixedItemDesc)

                                ' XBC 25/05/2012
                                ActionNode.Tag = qActionTypes.Item(i).ItemID.ToString
                                ActionNode.Tag = DeleteActionRepetitions(qActionTypes.Item(i).ItemID.ToString)
                                ' XBC 25/05/2012

                                ActionNode.ImageKey = "ACTION"
                                ActionNode.SelectedImageKey = "ACTION"

                            End If
                        Next
                    End If
                Else
                    ShowMessage("Error", myGlobal.ErrorCode, myGlobal.ErrorMessage)
                    Me.PrepareErrorMode()
                End If
            End If

        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myGlobal.ErrorMessage = ex.Message

            MyBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".LoadActionTreeViewNode", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".LoadActionTreeViewNode", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
        Return myGlobal
    End Function


    ''' <summary>
    ''' Delete _Rep characters from the ISE Action
    ''' </summary>
    ''' <param name="pActionDesc"></param>
    ''' <returns></returns>
    ''' <remarks>Created by XBC 25/05/2012 - Some Actions ara repeated but tfmwPreloadedMasterData not accept repeated keys</remarks>
    Private Function DeleteActionRepetitions(ByVal pActionDesc As String) As String
        Dim returnValue As String = ""
        Try
            returnValue = pActionDesc
            Dim i As Integer
            i = InStr(pActionDesc, "_Rep")
            If i > 0 Then
                returnValue = pActionDesc.Substring(0, i - 1)
            End If

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".DeleteActionRepetitions", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".DeleteActionRepetitions", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
        Return returnValue
    End Function


    ''' <summary>
    ''' Loads all ISE Actions by Service Sw Data related to the Analyzer
    ''' </summary>
    ''' <remarks>
    ''' Created by XBC 27/01/2012
    ''' </remarks>
    Private Function LoadActionsServiceTreeView() As GlobalDataTO
        Dim myGlobal As New GlobalDataTO
        Try
            BsActionsTreeView.Nodes.Clear()
            BsActionsTreeView.ShowNodeToolTips = True

            If Not myGlobal.HasError Then myGlobal = MyClass.LoadActionTreeViewNode(PreloadedMasterDataEnum.ISE_ACT_SRV_ACT) 'SGM 11/06/2012
            'If Not myGlobal.HasError Then myGlobal = MyClass.LoadActionTreeViewNode(PreloadedMasterDataEnum.ISE_ACT_SRV_INITIAL) 'SGM 13/06/2012
            If Not myGlobal.HasError Then myGlobal = MyClass.LoadActionTreeViewNode(PreloadedMasterDataEnum.ISE_ACT_SRV_REQUEST)
            If Not myGlobal.HasError Then myGlobal = MyClass.LoadActionTreeViewNode(PreloadedMasterDataEnum.ISE_ACT_SRV_GENERAL)
            If Not myGlobal.HasError Then myGlobal = MyClass.LoadActionTreeViewNode(PreloadedMasterDataEnum.ISE_ACT_SRV_CALIBR)
            If Not myGlobal.HasError Then myGlobal = MyClass.LoadActionTreeViewNode(PreloadedMasterDataEnum.ISE_ACT_SRV_PUMP_TUB)
            If Not myGlobal.HasError Then myGlobal = MyClass.LoadActionTreeViewNode(PreloadedMasterDataEnum.ISE_ACT_SRV_FLUI_TUB)
            If Not myGlobal.HasError Then myGlobal = MyClass.LoadActionTreeViewNode(PreloadedMasterDataEnum.ISE_ACT_SRV_INS_MODL)
            If Not myGlobal.HasError Then myGlobal = MyClass.LoadActionTreeViewNode(PreloadedMasterDataEnum.ISE_ACT_SRV_INS_REAG)
            If Not myGlobal.HasError Then myGlobal = MyClass.LoadActionTreeViewNode(PreloadedMasterDataEnum.ISE_ACT_SRV_INS_ELEC)
            If Not myGlobal.HasError Then myGlobal = MyClass.LoadActionTreeViewNode(PreloadedMasterDataEnum.ISE_ACT_SRV_DEACT)
            'If Not myGlobal.HasError Then myGlobal = MyClass.LoadActionTreeViewNode(PreloadedMasterDataEnum.ISE_ACT_SRV_PREPOK)
            If Not myGlobal.HasError Then myGlobal = MyClass.LoadActionTreeViewNode(PreloadedMasterDataEnum.ISE_ACT_SRV_ALM) 'SGM 26/10/2012 - Current ISE Alarms

        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myGlobal.ErrorMessage = ex.Message

            MyBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".LoadActionsServiceTreeView", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".LoadActionsServiceTreeView", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
        Return myGlobal
    End Function

    ''' <summary>
    ''' Loads all ISE Actions by User Sw Data related to the Analyzer
    ''' </summary>
    ''' <remarks>
    ''' Created by XBC 16/02/2012
    ''' </remarks>
    Private Function LoadActionsUserTreeView() As GlobalDataTO
        Dim myGlobal As New GlobalDataTO
        Try
            BsActionsTreeView.Nodes.Clear()
            BsActionsTreeView.ShowNodeToolTips = True

            If Not myGlobal.HasError Then myGlobal = MyClass.LoadActionTreeViewNode(PreloadedMasterDataEnum.ISE_ACT_USR_ACT) 'SGM 11/06/2012
            'If Not myGlobal.HasError Then myGlobal = MyClass.LoadActionTreeViewNode(PreloadedMasterDataEnum.ISE_ACT_USR_INITIAL) 'SGM 13/06/2012
            If Not myGlobal.HasError Then myGlobal = MyClass.LoadActionTreeViewNode(PreloadedMasterDataEnum.ISE_ACT_USR_GENERAL)
            If Not myGlobal.HasError Then myGlobal = MyClass.LoadActionTreeViewNode(PreloadedMasterDataEnum.ISE_ACT_USR_CALIBR)
            If Not myGlobal.HasError Then myGlobal = MyClass.LoadActionTreeViewNode(PreloadedMasterDataEnum.ISE_ACT_USR_INS_REAG)
            If Not myGlobal.HasError Then myGlobal = MyClass.LoadActionTreeViewNode(PreloadedMasterDataEnum.ISE_ACT_USR_INS_ELEC)
            If Not myGlobal.HasError Then myGlobal = MyClass.LoadActionTreeViewNode(PreloadedMasterDataEnum.ISE_ACT_USR_PUMP_TUB)
            If Not myGlobal.HasError Then myGlobal = MyClass.LoadActionTreeViewNode(PreloadedMasterDataEnum.ISE_ACT_USR_DEACT)
            'If Not myGlobal.HasError Then myGlobal = MyClass.LoadActionTreeViewNode(PreloadedMasterDataEnum.ISE_ACT_USR_FLUI_TUB)  ' XBC 29/05/2012 - Just for SAT
            If Not myGlobal.HasError Then myGlobal = MyClass.LoadActionTreeViewNode(PreloadedMasterDataEnum.ISE_ACT_USR_PREPOK)
            If Not myGlobal.HasError Then myGlobal = MyClass.LoadActionTreeViewNode(PreloadedMasterDataEnum.ISE_ACT_USR_ALM) 'SGM 26/10/2012 - Current ISE Alarms

        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myGlobal.ErrorMessage = ex.Message

            MyBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".LoadActionsServiceTreeView", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".LoadActionsServiceTreeView", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
        Return myGlobal
    End Function

    Private Function ValidateActionNode(ByVal pValue As String, Optional ByVal pSend As Boolean = False) As GlobalDataTO
        Dim myGlobal As New GlobalDataTO
        Try
            Dim myISECommand As New ISECommandTO()

            ' initializations
            Me.BsTimesUpDown.Enabled = False
            Me.BsPositionUpDown.Enabled = False
            Me.BsVolumeUpDown.Enabled = False

            Me.BsVolumeUpDown.Maximum = CInt(Me.DispVolumeMaxAttr)
            Me.BsVolumeUpDown.Minimum = CInt(Me.DispVolumeMinAttr)
            If Not pSend Then
                Me.BsVolumeUpDown.Value = CInt(Me.DispVolumeDefAttr)
            End If

            With myISECommand
                Select Case pValue
                    Case "ISE?"
                        .ISECommandID = ISECommands.POLL
                    Case "CKSM"
                        .ISECommandID = ISECommands.VERSION_CHECKSUM
                    Case "SWBC"
                        .ISECommandID = ISECommands.SHOW_BUBBLE_CAL
                    Case "SWPC"
                        .ISECommandID = ISECommands.SHOW_PUMP_CAL
                    Case "SWCS"
                        .ISECommandID = ISECommands.LAST_SLOPES
                    Case "RDMV"
                        .ISECommandID = ISECommands.READ_mV
                    Case "DLRD"
                        ' By now, read page 0
                        .ISECommandID = ISECommands.READ_PAGE_0_DALLAS
                    Case "MANT"
                        .ISECommandID = ISECommands.MAINTENANCE
                    Case "DSPA"
                        .ISECommandID = ISECommands.DSPA
                        Me.BsTimesUpDown.Enabled = True
                        Me.BsVolumeUpDown.Enabled = True
                    Case "DISB"
                        .ISECommandID = ISECommands.DSPB
                        Me.BsTimesUpDown.Enabled = True
                        Me.BsVolumeUpDown.Enabled = True
                    Case "PUGA"
                        .ISECommandID = ISECommands.PURGEA
                        Me.BsTimesUpDown.Enabled = True
                    Case "PUGB"
                        .ISECommandID = ISECommands.PURGEB
                        Me.BsTimesUpDown.Enabled = True
                    Case "PRMA"
                        .ISECommandID = ISECommands.PRIME_CALA
                        Me.BsTimesUpDown.Enabled = True
                    Case "PRMB"
                        .ISECommandID = ISECommands.PRIME_CALB
                        Me.BsTimesUpDown.Enabled = True
                        'Case "CLEN"
                        '    .ISECommandID = ISECommands.CLEAN
                        '    Me.BsPositionUpDown.Enabled = True
                    Case "CALB"
                        .ISECommandID = ISECommands.CALB
                    Case "PMCL"
                        .ISECommandID = ISECommands.PUMP_CAL
                        'Me.BsPositionUpDown.Enabled = True
                    Case "BBCL"
                        .ISECommandID = ISECommands.BUBBLE_CAL
                    Case "DVON"
                        .ISECommandID = ISECommands.DEBUG_mV_ONE
                    Case "MVON"
                        .ISECommandID = ISECommands.DEBUG_mV_TWO
                    Case "DVFF"
                        .ISECommandID = ISECommands.DEBUG_mV_OFF

                    Case Else
                        .ISECommandID = ISECommands.NONE
                End Select


                ' general validations
                If Not Me.SimulationMode Then
                    If Not myISEManager.IsISESwitchON Then
                        .ISECommandID = ISECommands.NONE
                    End If
                End If

            End With


            myGlobal.SetDatos = myISECommand

        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myGlobal.ErrorMessage = ex.Message

            MyBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ValidateActionNode", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".ValidateActionNode", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
        Return myGlobal
    End Function

    ''' <summary>
    ''' Prepares the Times Up Down according to the command and group
    ''' </summary>
    ''' <param name="pCommandID"></param>
    ''' <remarks>
    ''' Created by SGM 11/06/2012
    ''' Modified by XB 04/02/2013 - Upper conversions redundants because the value is already in UpperCase must delete to avoid Regional Settings problems (Bugs tracking #1112)
    ''' </remarks>
    Private Sub PrepareTimesUpDown(ByVal pCommandID As String)
        Try
            Select Case CurrentBlockNode.Tag.ToString   '.ToUpper

                Case PreloadedMasterDataEnum.ISE_ACT_USR_ACT.ToString, PreloadedMasterDataEnum.ISE_ACT_SRV_ACT.ToString
                    Select Case pCommandID  '.ToUpper
                        Case "PRIME_CALA", "PRIME_CALB"
                            Me.BsTimesUpDown.Value = 9

                        Case Else
                            Me.BsTimesUpDown.Value = 1

                    End Select

                Case PreloadedMasterDataEnum.ISE_ACT_USR_PUMP_TUB.ToString, PreloadedMasterDataEnum.ISE_ACT_SRV_PUMP_TUB.ToString, _
                    PreloadedMasterDataEnum.ISE_ACT_USR_FLUI_TUB.ToString, PreloadedMasterDataEnum.ISE_ACT_SRV_FLUI_TUB.ToString
                    Select Case pCommandID  '.ToUpper
                        Case "PRIME_CALA", "PRIME_CALB"
                            Me.BsTimesUpDown.Value = 2

                        Case Else
                            Me.BsTimesUpDown.Value = 1

                    End Select

                Case PreloadedMasterDataEnum.ISE_ACT_SRV_DEACT.ToString, PreloadedMasterDataEnum.ISE_ACT_USR_DEACT.ToString
                    Select Case pCommandID  '.ToUpper
                        Case "DSPA", "DSPB"
                            Me.BsTimesUpDown.Value = 3

                        Case "PURGEA", "PURGB"
                            Me.BsTimesUpDown.Value = 3

                        Case "PRIME_CALA", "PRIME_CALB"
                            Me.BsTimesUpDown.Value = 20

                        Case Else
                            Me.BsTimesUpDown.Value = 1

                    End Select

                Case PreloadedMasterDataEnum.ISE_ACT_USR_INS_ELEC.ToString, PreloadedMasterDataEnum.ISE_ACT_SRV_INS_ELEC.ToString
                    Select Case pCommandID  '.ToUpper
                        Case "PURGEA", "PURGB"
                            Me.BsTimesUpDown.Value = 3

                        Case Else
                            Me.BsTimesUpDown.Value = 1

                    End Select

                Case PreloadedMasterDataEnum.ISE_ACT_USR_INS_REAG.ToString, PreloadedMasterDataEnum.ISE_ACT_SRV_INS_REAG.ToString
                    Select Case pCommandID  '.ToUpper
                        Case "PURGEA", "PURGB"
                            Me.BsTimesUpDown.Value = 3

                        Case "PRIME_CALA", "PRIME_CALB"
                            Me.BsTimesUpDown.Value = 9

                        Case Else
                            Me.BsTimesUpDown.Value = 1

                    End Select


                Case Else
                    Me.BsTimesUpDown.Value = 1

            End Select

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".PrepareTimesUpDown", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareTimesUpDown", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' Modified by SA 14/05/2012 - Call to function IsThereAnyISETest changed by call to new function IsThereAnyTestByType 
    '''                             informing TestType parameter as ISE
    Private Function ValidateISEAction(ByVal pNode As TreeNode) As Boolean

        Dim resultValue As Boolean = True
        Dim myGlobal As New GlobalDataTO

        Try

            Dim myAction As String = CStr(pNode.Tag)

            If Me.SimulationMode Then

                resultValue = (myAction.Length > 0)

            Else
                Dim isInstalled As Boolean = myISEManager.IsISEModuleInstalled

                Dim myBlockNode As TreeNode = pNode.Parent
                If myBlockNode IsNot Nothing Then
                    If String.Compare(myBlockNode.Tag.ToString, PreloadedMasterDataEnum.ISE_ACT_USR_ACT.ToString, False) = 0 Or myBlockNode.Tag.ToString = PreloadedMasterDataEnum.ISE_ACT_SRV_ACT.ToString Then
                        isInstalled = True
                    End If
                End If

                Select Case myAction

                    Case "INIT_ISE"
                        resultValue = resultValue And isInstalled And Not myISEManager.IsISEInitiatedOK And (MyClass.IsInstallGroupNodeActive Or Not myISEManager.IsLongTermDeactivation) 'SGM 15/10/2012
                        'resultValue = resultValue And isInstalled And Not myISEManager.IsISEInitiatedOK And Not myISEManager.IsLongTermDeactivation 'SGM 13/06/2012
                    Case "ACT_ISE"
                        resultValue = resultValue And (myISEManager.IsLongTermDeactivation Or Not myISEManager.IsISEModuleInstalled) ' SGM 20/04/2012
                    Case "DEACT_ISE"
                        resultValue = resultValue And isInstalled ' XBC 19/04/2012
                    Case "NOT_DEACT_ISE"
                        resultValue = resultValue And isInstalled ' XBC 19/04/2012
                    Case "ACT_ELE"
                        resultValue = resultValue And isInstalled ' XBC 19/04/2012
                    Case "ACT_REA"
                        'resultValue = (myISEManager.ReagentsPackInstallationDate = Nothing)    ' XBC 04/05/2012
                        resultValue = resultValue And isInstalled ' XBC 19/04/2012
                    Case "WASH_PACK_INST"
                        resultValue = resultValue And isInstalled ' XBC 19/04/2012
                    Case "ACT_PREP"
                        'SGM 10/05/2012
                        If MyClass.myISEManager.IsISEModuleReady Then
                            If Not WorkSessionIDAttribute.Trim = "" Then
                                If MyClass.WSStatusAttribute <> "EMPTY" And MyClass.WSStatusAttribute <> "OPEN" Then
                                    Dim myOrderTestsDelegate As New OrderTestsDelegate
                                    myGlobal = myOrderTestsDelegate.IsThereAnyTestByType(Nothing, WorkSessionIDAttribute, "ISE")
                                    If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
                                        If CBool(myGlobal.SetDatos) Then
                                            'SGM 25/09/2012 - check if calibrations are ok
                                            Dim isReady As Boolean = False
                                            Dim myAffectedElectrodes As List(Of String)
                                            myGlobal = MyClass.myISEManager.CheckAnyCalibrationIsNeeded(myAffectedElectrodes)
                                            isReady = Not (CBool(myGlobal.SetDatos) And myAffectedElectrodes Is Nothing)

                                            resultValue = isReady

                                            If Not isReady Then Me.DisplayMessage(Messages.ISE_NOT_READY.ToString)
                                            'end SGM 25/09/2012
                                        End If
                                    Else
                                        resultValue = False
                                    End If

                                ElseIf MyClass.WSStatusAttribute = "EMPTY" Or MyClass.WSStatusAttribute = "OPEN" Then
                                    resultValue = False
                                    'Me.DisplayMessage(Messages.ISE_NOT_READY.ToString)
                                End If
                            Else
                                resultValue = False
                            End If
                            resultValue = resultValue And isInstalled ' XBC 19/04/2012
                        Else
                            resultValue = False
                            Me.DisplayMessage(Messages.ISE_NOT_READY.ToString)
                        End If

                    Case "ACT_FLUID_TUB"
                        resultValue = resultValue And isInstalled ' XBC 19/04/2012
                    Case "ACT_PUMP_TUB"
                        resultValue = resultValue And isInstalled ' XBC 19/04/2012

                    Case "CLEN"
                        resultValue = resultValue And isInstalled ' SGM 09/05/2012
                        Me.BsPositionUpDown.Enabled = True

                    Case "PRM_CALB" 'SGM 11/06/2012
                        resultValue = resultValue And isInstalled ' XBC 19/04/2012

                    Case "PRM2_CALB" 'SGM 11/06/2012
                        resultValue = resultValue And isInstalled ' XBC 19/04/2012

                    Case Else
                        resultValue = False
                End Select

                ' general validations
                If myISEManager.IsISEModuleInstalled Then ' XBC 19/04/2012 

                    'resultValue = resultValue And myISEManager.IsISECommsOk
                    resultValue = resultValue And myISEManager.IsISESwitchON
                End If

            End If


            If (Not resultValue And myAction <> "ACT_ISE" And myAction <> "INIT_ISE" And myAction <> "ACT_PREP") Then
                Me.DisplayMessage(Messages.ISE_NOT_READY.ToString)
            End If



        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ValidateISEAction", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".ValidateISEAction", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
        Return resultValue
    End Function

    Private Function ValidateNodeByBlock(ByVal pNode As TreeNode) As Boolean
        Dim resultValue As Boolean = False
        Try
            If pNode IsNot Nothing Then
                If Me.SimulationMode Then
                    resultValue = True
                Else

                    If MyClass.ValidateAnalyzerReadiness Then
                        Dim myBlockNode As TreeNode = pNode.Parent
                        If myBlockNode IsNot Nothing Then

                            ' XBC + SG 30/10/2012
                            If ThisIsService And Not MyClass.myISEManager.IsISEModuleInstalled Then
                                If myBlockNode.Tag.ToString = PreloadedMasterDataEnum.ISE_ACT_SRV_ACT.ToString Then
                                    If pNode.Tag.ToString = "ACT_ISE" Then
                                        resultValue = True
                                    End If
                                    Exit Try
                                End If
                            End If
                            ' XBC + SG 30/10/2012

                            If MyClass.myISEManager.IsLongTermDeactivation Then
                                If myBlockNode.Tag.ToString = PreloadedMasterDataEnum.ISE_ACT_USR_ACT.ToString Or myBlockNode.Tag.ToString = PreloadedMasterDataEnum.ISE_ACT_SRV_ACT.ToString Then
                                    If Not ThisIsService And Not MyClass.myISEManager.IsISEModuleInstalled Then
                                        resultValue = False
                                    Else
                                        resultValue = True
                                    End If
                                    MyClass.IsInstallGroupNodeActive = resultValue
                                End If
                            Else
                                'SGM 01/02/2012 - Check if it is Service Assembly - Bug #1112
                                'If My.Application.Info.AssemblyName.ToUpper.Contains("SERVICE") Then
                                If GlobalBase.IsServiceAssembly Then
                                    If Not MyClass.myISEManager.IsISEInitiatedOK Then
                                        If pNode.Tag.ToString = "INIT_ISE" Then
                                            resultValue = True
                                        Else
                                            resultValue = False
                                        End If
                                    Else
                                        resultValue = True
                                    End If
                                Else
                                    If mdiAnalyzerCopy.Alarms.Contains(Alarms.ISE_CONNECT_PDT_ERR) Then
                                        If pNode.Tag.ToString = "INIT_ISE" Then
                                            resultValue = True
                                        Else
                                            resultValue = False
                                        End If
                                    Else
                                        resultValue = True
                                    End If
                                End If
                            End If
                        End If
                    End If



                    If Not MyClass.myISEManager.IsISESwitchON Then
                        resultValue = False
                    End If

                    'SGM 16/01/2013
                    If Not resultValue Then
                        'Me.BsAdjustButton.Enabled = False
                        Me.DisplayMessage(Messages.ISE_NOT_READY.ToString)
                    End If


                    'SGM 16/01/2013 Allow in case of already having install date
                    ''SGM 21/09/2012 - special case for Ragents Pack is already activated
                    'If resultValue Then
                    '    If pNode.Tag.ToString = "ACT_REA" Then
                    '        If myISEManager.ReagentsPackInstallationDate = Nothing Then
                    '            resultValue = True
                    '        Else
                    '            resultValue = False
                    '        End If
                    '    End If
                    'Else
                    '    'Me.BsAdjustButton.Enabled = False
                    '    Me.DisplayMessage(Messages.ISE_NOT_READY.ToString)
                    'End If
                    ''end SGM 21/09/2012

                End If

            End If


        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ValidateNodeByBlock", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".ValidateNodeByBlock", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
        Return resultValue
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>
    ''' Modified by XBC 31/08/2012 - Add special case from PrepareLoadingMode 
    '''                              managed with the Optional parameter (pLoadingScreen). 
    '''                              For the case when Initialization ISE module is required
    ''' </remarks>
    Private Function ValidateISEAvailability(Optional ByVal pLoadingScreen As Boolean = False) As Boolean
        Dim resultValue As Boolean = False
        Try

            If MyClass.ValidateAnalyzerReadiness Then

                If Me.SimulationMode Then
                    resultValue = True
                Else
                    If pLoadingScreen Then
                        ' Comming from loading screen
                        If MyClass.myISEManager.IsISEInitiating Then
                            resultValue = False
                        Else
                            resultValue = MyClass.myISEManager.IsISESwitchON

                            ' XBC 27/07/2012 - While User is intalling the module all functions under its Node must be Enable
                            If MyClass.IsInstallGroupNodeActive Then
                                resultValue = True
                            End If
                        End If
                    Else
                        ' Comming form Enable/Disable Action validation
                        If Not MyClass.myISEManager.IsISEInitiating And MyClass.myISEManager.IsISEInitializationDone Then
                            'when initialization finished
                            resultValue = MyClass.myISEManager.IsISESwitchON
                        Else
                            resultValue = False

                            ' XBC 27/07/2012 - While User is intalling the module all functions under its Node must be Enable
                            If MyClass.IsInstallGroupNodeActive Then
                                resultValue = True
                            End If
                        End If
                    End If


                    'resultValue = MyClass.IsISEAvailable
                    If Not resultValue Then
                        Me.DisplayMessage(Messages.ISE_NOT_READY.ToString)
                    Else
                        Me.DisplayMessage("")
                    End If
                End If
            End If

            ' XBC 28/06/2012
            'Me.BsVolumeUpDown.Enabled = resultValue
            'Me.BsPositionUpDown.Enabled = resultValue
            'Me.BsTimesUpDown.Enabled = resultValue
            'Me.FeaturesGroupBox.Enabled = resultValue

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ValidateISEAvailability", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".ValidateISEAvailability", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
        Return resultValue
    End Function

    Private Sub CreateFileOutput(ByVal pPath As String)
        Try
            If File.Exists(pPath) Then File.Delete(pPath)
            Dim myStreamWriter As StreamWriter = File.CreateText(pPath)

            For Each line As String In BsRichTextBox1.Lines
                myStreamWriter.WriteLine(line)
            Next line

            myStreamWriter.Close()

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".CreateFileOutput ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".CreateFileOutput ", Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Copied and adapted from IAx00MainMDI (PresentationUSR) because we can not call to IAx00MainMDI.ActivateButtonWithAlarms
    ''' </summary>
    ''' <param name="pButton"></param>
    ''' <returns></returns>
    ''' <remarks>AG 29/03/2012</remarks>
    Public Function ActivateButtonWithAlarms(ByVal pButton As GlobalEnumerates.ActionButton) As Boolean
        Dim myStatus As Boolean = True
        Try

            If Me.SimulationMode Then Return True

            If Not mdiAnalyzerCopy Is Nothing Then

                'Dim resultData As New GlobalDataTO
                Dim myAlarms As New List(Of GlobalEnumerates.Alarms)

                ' AG+XBC 24/05/2012
                'Dim myAx00Status As GlobalEnumerates.AnalyzerManagerStatus = GlobalEnumerates.AnalyzerManagerStatus.SLEEPING
                Dim myAx00Status As GlobalEnumerates.AnalyzerManagerStatus = GlobalEnumerates.AnalyzerManagerStatus.NONE
                ' AG+XBC 24/05/2012

                myAlarms = mdiAnalyzerCopy.Alarms
                myAx00Status = mdiAnalyzerCopy.AnalyzerStatus

                ''AG 25/10/2011 - Before treat the cover alarms read if they are deactivated (0 disabled, 1 enabled)
                'Dim mainCoverAlarm As Boolean = CType(IIf(myAlarms.Contains(GlobalEnumerates.Alarms.MAIN_COVER_WARN), 1, 0), Boolean)
                'Dim reactionsCoverAlarm As Boolean = CType(IIf(myAlarms.Contains(GlobalEnumerates.Alarms.REACT_COVER_WARN), 1, 0), Boolean)
                'Dim fridgeCoverAlarm As Boolean = CType(IIf(myAlarms.Contains(GlobalEnumerates.Alarms.FRIDGE_COVER_WARN), 1, 0), Boolean)
                'Dim samplesCoverAlarm As Boolean = CType(IIf(myAlarms.Contains(GlobalEnumerates.Alarms.S_COVER_WARN), 1, 0), Boolean)

                'resultData = mdiAnalyzerCopy.ReadFwAdjustmentsDS
                'If Not resultData.HasError And Not resultData.SetDatos Is Nothing Then
                '    'Dim myAdjDS As SRVAdjustmentsDS = CType(resultData.SetDatos, SRVAdjustmentsDS) 'Causes system error in develop mode
                '    Dim myAdjDS As New SRVAdjustmentsDS
                '    myAdjDS = CType(resultData.SetDatos, SRVAdjustmentsDS)

                '    Dim linqRes As List(Of SRVAdjustmentsDS.srv_tfmwAdjustmentsRow)
                '    'Main cover disabled (0 disabled, 1 enabled)
                '    linqRes = (From a As SRVAdjustmentsDS.srv_tfmwAdjustmentsRow In myAdjDS.srv_tfmwAdjustments _
                '               Where a.CodeFw = GlobalEnumerates.Ax00Adjustsments.MCOV.ToString Select a).ToList
                '    If linqRes.Count > 0 AndAlso linqRes(0).Value <> "" Then
                '        If Not CType(linqRes(0).Value, Boolean) Then mainCoverAlarm = False
                '    End If
                '    'Reactions cover disabled (0 disabled, 1 enabled)
                '    linqRes = (From a As SRVAdjustmentsDS.srv_tfmwAdjustmentsRow In myAdjDS.srv_tfmwAdjustments _
                '               Where a.CodeFw = GlobalEnumerates.Ax00Adjustsments.PHCOV.ToString Select a).ToList
                '    If linqRes.Count > 0 AndAlso linqRes(0).Value <> "" Then
                '        If Not CType(linqRes(0).Value, Boolean) Then reactionsCoverAlarm = False
                '    End If
                '    'Reagents cover disabled (0 disabled, 1 enabled)
                '    linqRes = (From a As SRVAdjustmentsDS.srv_tfmwAdjustmentsRow In myAdjDS.srv_tfmwAdjustments _
                '               Where a.CodeFw = GlobalEnumerates.Ax00Adjustsments.RCOV.ToString Select a).ToList
                '    If linqRes.Count > 0 AndAlso linqRes(0).Value <> "" Then
                '        If Not CType(linqRes(0).Value, Boolean) Then fridgeCoverAlarm = False
                '    End If
                '    'Samples cover disabled (0 disabled, 1 enabled)
                '    linqRes = (From a As SRVAdjustmentsDS.srv_tfmwAdjustmentsRow In myAdjDS.srv_tfmwAdjustments _
                '               Where a.CodeFw = GlobalEnumerates.Ax00Adjustsments.SCOV.ToString Select a).ToList
                '    If linqRes.Count > 0 AndAlso linqRes(0).Value <> "" Then
                '        If Not CType(linqRes(0).Value, Boolean) Then samplesCoverAlarm = False
                '    End If
                'End If
                ''AG 25/10/2011

                'AG 02/04/2012
                '(ISE instaled and (initiated or not SwitchedON)) Or not instaled
                Dim iseInitiatedFinishedFlag As Boolean = True
                If mdiAnalyzerCopy.ISE_Manager IsNot Nothing AndAlso _
                   (mdiAnalyzerCopy.ISE_Manager.IsISEInitiating OrElse Not mdiAnalyzerCopy.ISE_Manager.ConnectionTasksCanContinue) Then
                    iseInitiatedFinishedFlag = False
                End If
                'AG 02/04/2012

                Select Case pButton
                    'ISE COMMAND FROM ISE UTILITIES SCREEN
                    Case ActionButton.ISE_COMMAND
                        If myAx00Status = GlobalEnumerates.AnalyzerManagerStatus.STANDBY Then
                            'If mainCoverAlarm OrElse _
                            '   reactionsCoverAlarm OrElse _
                            '   fridgeCoverAlarm OrElse _
                            '   samplesCoverAlarm OrElse Not iseInitiatedFinishedFlag Then

                            '    myStatus = False
                            'End If
                        Else
                            myStatus = False
                        End If
                        'AG 28/03/2012
                End Select

                'XBC 03/04/2012
                If ThisIsService Then
                    If myISEManager.IsISEModuleInstalled And Not myISEManager.IsISESwitchON Then
                        myStatus = False
                    End If
                Else
                    If myAlarms.Contains(GlobalEnumerates.Alarms.ISE_OFF_ERR) Then
                        myStatus = False
                    End If
                End If
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".ActivateButtonWithAlarms ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & " ActivateButtonWithAlarms, ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
        Return myStatus
    End Function

#Region "Service Adjustments management"
    ''' <summary>
    ''' Gets the Dataset that corresponds to the editing adjustments
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>Created by XBC 07/03/2012</remarks>
    Private Function LoadAdjustmentGroupData() As GlobalDataTO
        Dim myGlobal As New GlobalDataTO
        Dim myAllAdjustmentsDS As New SRVAdjustmentsDS
        Dim CopyOfSelectedAdjustmentsDS As SRVAdjustmentsDS = MyClass.SelectedAdjustmentsDS
        Dim myAdjustmentsGroups As New List(Of String)
        Try
            myGlobal = MyClass.mdiAnalyzerCopy.ReadFwAdjustmentsDS
            If myGlobal.SetDatos IsNot Nothing AndAlso Not myGlobal.HasError Then
                Dim resultDS As SRVAdjustmentsDS = CType(myGlobal.SetDatos, SRVAdjustmentsDS)
                'if the global dataset is empty the force to load again from the db
                If resultDS Is Nothing OrElse resultDS.srv_tfmwAdjustments.Count = 0 Then
                    myGlobal = MyClass.mdiAnalyzerCopy.LoadFwAdjustmentsMasterData(MyClass.SimulationMode)
                    If myGlobal.SetDatos IsNot Nothing AndAlso Not myGlobal.HasError Then
                        resultDS = CType(myGlobal.SetDatos, SRVAdjustmentsDS)
                    End If
                End If

                myAllAdjustmentsDS = resultDS
                MyClass.myFwAdjustmentsDelegate = New FwAdjustmentsDelegate(myAllAdjustmentsDS)

                If Not myGlobal.HasError And MyClass.myFwAdjustmentsDelegate IsNot Nothing Then
                    If MyClass.SelectedAdjustmentsDS IsNot Nothing Then
                        MyClass.SelectedAdjustmentsDS.Clear()
                    End If
                    myAdjustmentsGroups.Add(ADJUSTMENT_GROUPS.ISE_MODULE.ToString)
                    myGlobal = MyClass.myFwAdjustmentsDelegate.ReadAdjustmentsByGroupIDs(myAdjustmentsGroups)
                    If (Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing) Then
                        MyClass.SelectedAdjustmentsDS = CType(myGlobal.SetDatos, SRVAdjustmentsDS)
                    End If
                End If
            End If

        Catch ex As Exception
            MyClass.SelectedAdjustmentsDS = CopyOfSelectedAdjustmentsDS
            MyBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".LoadAdjustmentGroupData ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".LoadAdjustmentGroupData ", Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
        Return myGlobal
    End Function

    Private Function UpdateAdjustments() As GlobalDataTO
        Dim myGlobal As New GlobalDataTO
        Try
            If MyClass.SelectedAdjustmentsDS IsNot Nothing Then
                'update the dataset
                myGlobal = MyClass.myFwAdjustmentsDelegate.UpdateAdjustments(MyClass.SelectedAdjustmentsDS, MyClass.mdiAnalyzerCopy.ActiveAnalyzer)

                If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                    'update DDBB
                    Dim myAdjustmentsDelegate As New DBAdjustmentsDelegate
                    myGlobal = myAdjustmentsDelegate.UpdateAdjustmentsDB(Nothing, MyClass.SelectedAdjustmentsDS)

                    If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                        'update Adjustments File
                        myGlobal = MyClass.myFwAdjustmentsDelegate.ExportDSToFile(MyClass.mdiAnalyzerCopy.ActiveAnalyzer)
                    End If
                End If
            End If

        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myGlobal.ErrorMessage = ex.Message
            MyBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " UpdateAdjustments ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
        Return myGlobal
    End Function

    ''' <summary>
    ''' Store adjustments for ISE installed
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>Created by XBC 07/03/2012</remarks>
    Private Function SaveAdjustments(ByVal pIseInstalled As Boolean) As GlobalDataTO
        Dim myGlobal As New GlobalDataTO
        Dim exMessage As String = ""
        Try
            Me.EditedValue = pIseInstalled

            ' Convert dataset to String for sending to Fw

            ' Update dataset of the temporal dataset of Adjustments to sent to Fw
            myGlobal = UpdateTemporalAdjustmentsDS()

            ' Takes a copy of the changed values of the dataset of Adjustments
            myGlobal = myFwAdjustmentsDelegate.Clone(MyClass.SelectedAdjustmentsDS)
            If Not myGlobal.SetDatos Is Nothing AndAlso Not myGlobal.HasError Then
                Me.TemporalAdjustmentsDS = CType(myGlobal.SetDatos, SRVAdjustmentsDS)
                Me.TempToSendAdjustmentsDelegate = New FwAdjustmentsDelegate(Me.TemporalAdjustmentsDS)
                ' Update dataset of the temporal dataset of Adjustments to sent to Fw
                myGlobal = UpdateTemporalAdjustmentsDS()
            End If

            If myGlobal.HasError Then
                PrepareErrorMode()
            Else
                Me.DisplayMessage(Messages.SRV_SAVE_ADJUSTMENTS.ToString)

                ' Convert dataset to String for sending to Fw
                myGlobal = Me.TempToSendAdjustmentsDelegate.ConvertDSToString()

                If Not myGlobal.SetDatos Is Nothing AndAlso Not myGlobal.HasError Then
                    Dim pAdjuststr As String = CType(myGlobal.SetDatos, String)

                    Me.DisableAll()

                    If Me.SimulationMode Then
                        ' simulating
                        Dim myText As String = ""
                        Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate
                        System.Threading.Thread.Sleep(500)
                        myText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "MSG_SRV_COMPLETED", currentLanguage) + vbCrLf + vbCrLf
                        Me.AppendAnalyzerResponseText(Me.BsRichTextBox1, myText)
                        Me.Cursor = Cursors.Default
                        Me.DisplayMessage(Messages.SRV_TEST_READY.ToString)
                        MyClass.PrepareReadyForTestingMode()
                    Else
                        If Not myGlobal.HasError AndAlso MyClass.mdiAnalyzerCopy.Connected Then
                            myGlobal = MyClass.SendLOAD_ADJUSTMENTS(pAdjuststr)
                        End If

                        If myGlobal.HasError Then
                            Me.PrepareErrorMode()
                        End If
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
            MyBase.CreateLogActivity(exMessage, Me.Name & " SaveAdjustments ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, exMessage)
        End If
        Return myGlobal
    End Function

    ''' <summary>
    ''' Load Adjustments High Level Instruction to save values into the instrument
    ''' </summary>
    ''' <param name="pValueAdjustAttr"></param>
    ''' <returns></returns>
    ''' <remarks>Created by XBC 07/03/2012</remarks>
    Public Function SendLOAD_ADJUSTMENTS(ByVal pValueAdjustAttr As String) As GlobalDataTO
        Dim myResultData As New GlobalDataTO
        Try
            myResultData = myFwScriptDelegate.AnalyzerManager.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.LOADADJ, True, Nothing, pValueAdjustAttr)

        Catch ex As Exception
            myResultData.HasError = True
            myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myResultData.ErrorMessage = ex.Message

            MyBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " SendLOAD_ADJUSTMENTS ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
        Return myResultData
    End Function

    ''' <summary>
    ''' updates the temporal dataset with changed current adjustment
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>Created by XBC 07/03/2012</remarks>
    Private Function UpdateTemporalAdjustmentsDS() As GlobalDataTO
        Dim myGlobal As New GlobalDataTO
        Try
            Dim myValueToSave As String
            If Me.EditedValue Then
                myValueToSave = "1"
            Else
                myValueToSave = "0"
            End If
            Me.UpdateTemporalSpecificAdjustmentsDS(GlobalEnumerates.Ax00Adjustsments.ISEINS.ToString, myValueToSave)

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".UpdateTemporalAdjustmentsDS ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
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
    ''' <remarks>Created by XBC 07/03/2012</remarks>
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
            MyBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".UpdateTemporalSpecificAdjustmentsDS ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".UpdateTemporalSpecificAdjustmentsDS", Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
        Me.SelectedAdjustmentsDS.AcceptChanges()
        Return myGlobal
    End Function
#End Region

#End Region

#Region "Events"

    Private Sub IISEUtilities_FormClosed(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosedEventArgs) Handles Me.FormClosed
        Try

            ' SGM 10/05/2010
            If Not Me.myScreenPendingToOpenWhileISEUtilClosing Is Nothing Then
                'Everything is OK, open the selected screen
                Me.myScreenPendingToOpenWhileISEUtilClosing.AnalyzerModel = Me.AnalyzerModel
                Me.myScreenPendingToOpenWhileISEUtilClosing.MdiParent = MyClass.myParentMDI
                Me.myScreenPendingToOpenWhileISEUtilClosing.Show()
                Me.myScreenPendingToOpenWhileISEUtilClosing = Nothing
            Else
                If MyClass.IsMDICloseRequested Then
                    MyClass.IsMDICloseRequested = False
                    MyClass.IsCompletelyClosed = True
                    MyClass.myParentMDI.Close()
                Else
                    If Not MyClass.myISEManager.IsISESwitchON Then
                        MyClass.mdiAnalyzerCopy.RefreshISEAlarms()
                    End If
                    If MyClass.myParentMDI IsNot Nothing Then MyClass.myParentMDI.Text = My.Application.Info.ProductName
                End If
            End If
            ' END SGM 10/05/2010

            'SGM 30/07/2012
            If myISEManager IsNot Nothing Then
                myISEManager.IsInUtilities = False
            End If

            'RaiseEvent ActivateVerticalButtonsEvent(True) 'Enable Vertical Buttons bar while ISE Utilities is open SGM 14/05/2012

        Catch ex As Exception

            MyBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".FormClosed ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".FormClosed ", Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    '''' <summary>
    '''' 
    '''' </summary>
    '''' <param name="pResult"></param>
    '''' <remarks>Created by SGM 07/03/2012</remarks>
    'Private Sub OnISEOperationFinished(ByVal pResult As ISEManager.ISEOperationResult) Handles mdiAnalyzerCopy.ISEOperationFinished
    '    Try
    '        If mdiAnalyzerCopy.ISE_Manager IsNot Nothing Then
    '            PrepareTestedMode()
    '        End If
    '    Catch ex As Exception
    '        Dim myLogAcciones As New ApplicationLogManager()
    '        myLogAcciones.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & "OnISEOperationFinished", EventLogEntryType.Error, False)
    '    End Try
    'End Sub

    Protected Sub IIseAdjustments_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try

            'Dim myGlobal As New GlobalDataTO
            Dim myGlobalbase As New GlobalBase

            'Get an instance of the ISE manager class
            If Not AppDomain.CurrentDomain.GetData("GlobalAnalyzerManager") Is Nothing Then
                mdiAnalyzerCopy = CType(AppDomain.CurrentDomain.GetData("GlobalAnalyzerManager"), AnalyzerManager) ' Use the same AnalyzerManager as the MDI
                myISEManager = mdiAnalyzerCopy.ISE_Manager
                myISEManager.IsInUtilities = True
            End If

            'Get the current user level SGM 07/06/2012
            MyBase.CurrentUserLevel = myGlobalbase.GetSessionInfo.UserLevel

            'Get the current Language from the current Application Session
            Me.currentLanguage = myGlobalbase.GetSessionInfo.ApplicationLanguage.Trim.ToString

            'Load the multilanguage texts for all Screen Labels and get Icons for graphical Buttons
            Me.GetScreenLabels()
            Me.PrepareButtons()
            Me.PrepareTreeViewImages()

            'hide Caret of the textbox
            MyClass.HideCaret(Me.BsRichTextBox1.Handle)

            If ThisIsService Then

                'SGM 12/11/2012 - Information
                'Me.BsInfoXPSViewer.FitToPageHeight()
                MyClass.DisplayInformation(APPLICATION_PAGES.ISE_UTIL, Me.BsInfoXPSViewer)

                ' Service Sw
                Me.LoadActionsServiceTreeView()
                ' Me.GroupBoxLegendSRV.Visible = True
                'Me.GroupBoxLegendUSR.Visible = False    ' by now is no used
                Me.BsClearButton.Visible = True
                Me.BsSaveAsButton.Visible = True
                Me.BsLegendWarningLabel.ForeColor = Color.OrangeRed
                Me.BsLegendResponseLabel.ForeColor = Color.DarkOliveGreen
                Me.BsLegendCommandLabel.ForeColor = Color.SteelBlue
            Else
                ' User Sw
                Me.LoadActionsUserTreeView()
                'Me.GroupBoxLegendSRV.Visible = True
                'Me.GroupBoxLegendUSR.Visible = False    ' by now is no used
                Me.BsClearButton.Visible = False
                Me.BsSaveAsButton.Visible = False
                Me.BsLegendWarningLabel.ForeColor = Color.Black
                Me.BsLegendResponseLabel.ForeColor = Color.Black
                Me.BsLegendCommandLabel.ForeColor = Color.Black
            End If

            'If myGlobal.HasError Then
            '    ShowMessage("Error", myGlobal.ErrorCode, myGlobal.ErrorMessage)
            '    Me.PrepareErrorMode()
            '    Exit Try
            'End If

            'MyClass.myScreenDelegate = New IseAdjustmentDelegate(MyBase.myServiceMDI.ActiveAnalyzer, myFwScriptDelegate)

            'MyClass.InitializeHomes()

            'DisableAll()

            'RH 26/03/2012
            If ThisIsService Then
                ResetBorderSRV()
            Else
                ResizeUserForm()
            End If

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".Load ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".Load ", Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Resizes the form and hides unused areas
    ''' </summary>
    ''' <remarks>
    ''' Created by:  RH 26/03/2012
    ''' </remarks>
    Private Sub ResizeUserForm()
        Try
            Me.ControlBox = False
            Me.MinimizeBox = False
            Me.MaximizeBox = False
            Me.AutoScaleMode = Windows.Forms.AutoScaleMode.None
            Me.FormBorderStyle = Windows.Forms.FormBorderStyle.None
            Me.WindowState = FormWindowState.Normal

            Me.Controls.Remove(Me.bsISEInformationGroupBox)
            Me.Width = 737
            Me.Height = 592

            Dim myLocation As Point = Me.Parent.Location
            Dim mySize As Size = Me.Parent.Size
            Me.Location = New Point(myLocation.X + CInt((mySize.Width - Me.Width) / 2), myLocation.Y + CInt((mySize.Height - Me.Height) / 2) - 70)

            Application.DoEvents()

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ResizeForm ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ResizeForm ", Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub


    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>
    ''' Commented by XBC 11/07/2012 - 
    ''' USER SW : Prepare Loading functionality is invoked by MDI after Monitor is closed
    ''' SERVICE SW : no changes
    ''' </remarks>
    Private Sub IIseAdjustments_Shown(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Shown
        Dim myGlobal As New GlobalDataTO
        Try
            ' XBC 11/07/2012
            'MyClass.PrepareLoadingMode() 'SGM 12/06/2012
            If ThisIsService Then
                MyClass.PrepareLoadingMode()
            Else
                ' XB 27/11/2013 - Inform to MDI that this screen is shown - Task #1303
                ShownScreen()
            End If
            ' XBC 11/07/2012


            '' Check ISE available
            'If Not MyClass.IsISEAvailable Then
            '    myGlobal.HasError = True
            '    ' Prepare Error Mode Controls in GUI
            '    Me.DisplayMessage(Messages.ISE_NOT_READY.ToString)
            '    PrepareErrorMode()
            'Else
            '    'SGM 12/06/2012
            '    ''AG 04/04/2012
            '    'If mdiAnalyzerCopy.AnalyzerStatus <> GlobalEnumerates.AnalyzerManagerStatus.STANDBY Then

            '    '    If My.Application.Info.AssemblyName.ToUpper.Contains("SERVICE") Then
            '    '        'Nothing
            '    '    Else
            '    '        If mdiAnalyzerCopy.AnalyzerStatus = AnalyzerManagerStatus.RUNNING Then  'DL 01/06/2012
            '    '            bsScreenErrorProvider.ShowError(MSG_EndProcess)                     'DL 01/06/2012
            '    '        Else
            '    '            bsScreenErrorProvider.ShowError(MSG_StartInstrument)
            '    '        End If
            '    '    End If
            '    'End If

            '    'AG 04/04/2012
            '    Me.Refresh()
            '    PrepareLoadingMode()

            'End If

            'SGM 12/11/2012
            If ThisIsService Then
                Me.BsInfoXPSViewer.Visible = True
                Me.BsInfoXPSViewer.RefreshPage()
            End If

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".Shown ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".Shown ", Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub BsExitButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsExitButton.Click

        Dim myGlobal As New GlobalDataTO
        Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

        Try
            'TRAZA DE CIERRE DE PANTALLA
            Dim myApplicationLogMang As New ApplicationLogManager
            myApplicationLogMang.CreateLogActivity(Me.Name & ".BsExitButton.Click ", "IISEUtilities", EventLogEntryType.Information, False)
            'TRAZA DE CIERRE DE PANTALLA

            MyClass.IsScreenCloseRequested = True 'SGM 13/06/2012

            If myISEManager Is Nothing Or _
               Not MyClass.mdiAnalyzerCopy.Connected Or _
               mdiAnalyzerCopy.AnalyzerStatus = GlobalEnumerates.AnalyzerManagerStatus.SLEEPING Then
                MyClass.CurrentOperation = OPERATIONS.NONE ' XBC 11/07/2012
                Me.Close()
            Else
                If Not myISEManager.IsLongTermDeactivation AndAlso myISEManager.IsISECommsOk AndAlso myISEManager.IsISESwitchON Then

                    ' XBC 31/08/2012 - CreateWSExecutions consumes 1 second or more - Screen must be disabled from begining
                    MyClass.PrepareTestingMode()
                    MyClass.DisplayMessage("")

                    'SGM 14/05/2012  Verify if exist any work session
                    If MyClass.myISEManager.IsISEModuleReady AndAlso _
                        Not WorkSessionIDAttribute.Trim = "" AndAlso _
                       MyClass.WSStatusAttribute <> "EMPTY" And _
                       MyClass.WSStatusAttribute <> "OPEN" Then

                        'Verify is the current Analyzer Status is RUNNING
                        Dim createWSInRunning As Boolean = False
                        If (Not mdiAnalyzerCopy Is Nothing) Then createWSInRunning = (mdiAnalyzerCopy.AnalyzerStatus = GlobalEnumerates.AnalyzerManagerStatus.RUNNING)

                        'SGM 25/09/2012 - check if any calibration is needed
                        Dim isReady As Boolean = False
                        Dim myAffectedElectrodes As List(Of String)
                        myGlobal = MyClass.myISEManager.CheckAnyCalibrationIsNeeded(myAffectedElectrodes)
                        If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                            isReady = Not (CBool(myGlobal.SetDatos) And myAffectedElectrodes Is Nothing)
                        End If

                        If isReady Then ' XBC 15/04/2013 - if ISE still Not Ready, is no need re-generate executions because there no changes and it is long time process
                            CreateLogActivity("Launch CreateWSExecutions !", Me.Name & ".BsExitButton.Click ", EventLogEntryType.Information, False) 'AG 31/03/2014 - #1565
                            Dim myExecutionDelegate As New ExecutionsDelegate
                            myGlobal = myExecutionDelegate.CreateWSExecutions(Nothing, MyClass.mdiAnalyzerCopy.ActiveAnalyzer, MyClass.WorkSessionIDAttribute, _
                                                                              createWSInRunning, -1, String.Empty, isReady, myAffectedElectrodes)
                        End If
                        If Not isReady Then Me.DisplayMessage(Messages.ISE_NOT_READY.ToString)
                        'end SGM 25/09/2012
                        myAffectedElectrodes = Nothing 'AG 19/02/2014 - #1514

                    End If
                    'end SGM 14/05/2012


                    If MyClass.MaintenanceExitNeeded And Not MyClass.myISEManager.IsLongTermDeactivation Then
                        RaiseEvent ActivateScreenEvent(False, Messages.ENDING_ISE)

                        Dim myText As String
                        myText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_ISE_UTIL_EXIT", currentLanguage) + vbCrLf
                        'myText = "Closing ..." + vbCrLf
                        Me.AppendPcSendText(Me.BsRichTextBox1, myText)
                        Me.BsRichTextBox1.Refresh()
                        BsExitButton.Enabled = False
                        MyClass.DisplayMessage("")
                        Me.PrepareTestingMode()
                        Me.CurrentOperation = OPERATIONS.QUIT_UTILITIES
                        myGlobal = myISEManager.DoMaintenanceExit

                        Exit Sub
                    Else
                        'Me.Close()SGM 04/07/2012
                    End If
                Else
                    'Me.Close()SGM 04/07/2012
                End If

                'SGM 04/07/2012
                If MyClass.IsR2ArmAwayFromParking And MyClass.CurrentOperation <> OPERATIONS.R2_TO_PARKING And _
                    myISEManager.IsISESwitchON And myISEManager.IsISECommsOk Then

                    RaiseEvent ActivateScreenEvent(False, Messages.ENDING_ISE)
                    MyClass.PrepareTestingMode()
                    MyClass.DisplayMessage("")
                    Dim myText As String
                    myText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_ISE_R2_BACK", currentLanguage) + vbCrLf
                    Me.AppendPcSendText(Me.BsRichTextBox1, myText)
                    MyClass.CurrentOperation = OPERATIONS.R2_TO_PARKING
                    MyClass.myISEAction = ISECommands.R2_TO_PARKING
                    BsExitButton.Enabled = False
                    myGlobal = myISEManager.MoveR2ArmBack
                    If myGlobal.HasError Then
                        Me.PrepareErrorMode()
                    End If
                    'RaiseEvent ActivateScreenEvent(False, Messages.ENDING_ISE)
                    'BsExitButton.Enabled = False
                    Exit Sub
                ElseIf MyClass.CurrentOperation <> OPERATIONS.NONE Then
                    BsExitButton.Enabled = False

                    Me.Close()  ' XBC 31/08/2012 - correction : screen was not close when there are Error (DAL for example; IsISECommsOk=False) and after this, try to exit screen
                Else
                    ' XBC 11/07/2012
                    MyClass.CurrentOperation = OPERATIONS.NONE
                    Me.Close()
                    ' XBC 11/07/2012
                End If
                'SGM 04/07/2012

            End If

            'If My.Application.Info.AssemblyName.ToUpper.Contains("SERVICE") Then
            '    SendISECMD(ISECommands.DEBUG_mV_OFF)
            'End If
            'Me.Close()

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".BsExitButton.Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".BsExitButton.Click ", Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub BsActionsTreeView_AfterExpand1(ByVal sender As Object, ByVal e As System.Windows.Forms.TreeViewEventArgs) Handles BsActionsTreeView.AfterExpand
        Try
            Me.BsActionsTreeView.Focus()
        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".BsActionsTreeView_AfterExpand ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".BsActionsTreeView_AfterExpand ", Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub BsActionsTreeView_BeforeExpand(ByVal sender As Object, ByVal e As System.Windows.Forms.TreeViewCancelEventArgs) Handles BsActionsTreeView.BeforeExpand, BsActionsTreeView.BeforeCollapse
        Try
            Me.BsAdjustButton.Enabled = False
            Me.BsActionsTreeView.SelectedNode = Nothing
        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".BsActionsTreeView_BeforeExpand ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".BsActionsTreeView_BeforeExpand ", Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub



    Private Sub BsScreenActionsTreeView_NodeMouseClick(ByVal sender As Object, ByVal e As System.Windows.Forms.TreeNodeMouseClickEventArgs) Handles BsActionsTreeView.NodeMouseClick
        Try

            If e.Node.Parent Is Nothing Then
                'If e.Node.IsExpanded Then
                '    e.Node.Collapse()
                'Else
                '    e.Node.Expand()
                'End If
                Me.BsAdjustButton.Enabled = False
                Me.BsActionsTreeView.SelectedNode = Nothing
                Exit Sub
            End If

            Dim currNode As TreeNode
            Dim ClickPoint As Point = New Point(e.X, e.Y)
            currNode = BsActionsTreeView.GetNodeAt(ClickPoint)
            If Not currNode Is Nothing Then

                Dim myGlobal As New GlobalDataTO
                Dim myISECommand As New ISECommandTO()
                Me.BsAdjustButton.Enabled = False

                If currNode.Tag.ToString = "ISE_ALMS" Then
                    MyClass.CurrentActionNode = currNode
                    Me.BsAdjustButton.Enabled = (MyClass.IsAnalyzerAvailable And MyClass.myISEManager.IsISEModuleInstalled)
                Else
                    If MyClass.ValidateNodeByBlock(currNode) Then
                        myGlobal = MyClass.ValidateActionNode(currNode.Tag.ToString)
                        If (Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing) Then
                            myISECommand = CType(myGlobal.SetDatos, ISECommandTO)
                            If myISECommand.ISECommandID <> ISECommands.NONE Then
                                'ISE CMD ACTION NODE
                                MyClass.CurrentActionNode = currNode
                                MyClass.PrepareTimesUpDown(myISECommand.ISECommandID.ToString)
                                Me.BsAdjustButton.Enabled = MyClass.ValidateISEAvailability

                            Else
                                'COMPLEX ACTION NODE

                                ' XBC + SG 30/10/2012
                                If ThisIsService And Not MyClass.myISEManager.IsISEModuleInstalled Then
                                    If currNode.Tag.ToString = "ACT_ISE" Then
                                        MyClass.CurrentActionNode = currNode
                                        Me.BsAdjustButton.Enabled = True
                                    End If
                                    Exit Try
                                End If
                                ' XBC + SG 30/10/2012

                                Me.myISEAction = ISECommands.NONE
                                Me.BsTimesUpDown.Value = 1
                                If MyClass.ValidateISEAction(currNode) Then
                                    MyClass.CurrentActionNode = currNode
                                    If Not myISEManager.IsLongTermDeactivation Then
                                        Me.BsAdjustButton.Enabled = MyClass.ValidateISEAvailability
                                    Else
                                        Me.BsAdjustButton.Enabled = True
                                    End If
                                    Me.BsTimesUpDown.Value = 1
                                End If
                            End If
                        End If

                    End If
                End If
            End If
        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".BsScreenActionsTreeView.NodeMouseClick ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".BsScreenActionsTreeView.NodeMouseClick ", Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub


    Private Sub BsAdjustButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsAdjustButton.Click
        Try
            If Not MyClass.CurrentActionNode Is Nothing Then
                Dim myGlobal As New GlobalDataTO
                Dim myISECommand As New ISECommandTO()

                If MyClass.CurrentActionNode.Tag.ToString = "ISE_ALMS" Then
                    CreateLogActivity("ISE: " & MyClass.CurrentActionNode.Tag.ToString, Me.Name & ".BsAdjustButton_Click", EventLogEntryType.Information, False) 'JV #1360 24/10/2013
                    MyClass.ExecuteISEAction(MyClass.CurrentActionNode)
                Else
                    If MyClass.ValidateNodeByBlock(MyClass.CurrentActionNode) Then
                        myGlobal = MyClass.ValidateActionNode(MyClass.CurrentActionNode.Tag.ToString, True)
                        If (Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing) Then
                            myISECommand = CType(myGlobal.SetDatos, ISECommandTO)
                            CreateLogActivity("ISE: " & myISECommand.ISECommandID.ToString, Me.Name & ".BsAdjustButton_Click", EventLogEntryType.Information, False) 'JV #1360 24/10/2013
                            If myISECommand.ISECommandID <> ISECommands.NONE Then
                                'ISE CMD ACTION
                                Me.NumRepetitionsSelectedByUser = 1
                                If IsNumeric(Me.BsTimesUpDown.Value) Then
                                    Me.NumRepetitionsSelectedByUser = CInt(Me.BsTimesUpDown.Value)
                                End If

                                If Me.NumRepetitionsSelectedByUser > 1 Then
                                    Me.BsAdjustButton.Visible = False
                                    Me.BsStopButton.Visible = True
                                    Me.BsStopButton.Enabled = True 'SGM 25/09/2012
                                End If

                                Me.CurrentISECommand.ISECommandID = myISECommand.ISECommandID
                                SendISECMD(myISECommand.ISECommandID)

                            Else
                                'ISE COMPLEX ACTION
                                Me.myISEAction = ISECommands.NONE
                                If MyClass.ValidateISEAction(MyClass.CurrentActionNode) Then
                                    MyClass.ExecuteISEAction(MyClass.CurrentActionNode)
                                End If
                            End If
                        End If
                    End If
                End If
            End If

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".BsAdjustButton.Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".BsAdjustButton.Click ", Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' When the  ESC key is pressed, the screen is closed 
    ''' </summary>
    ''' <remarks>
    ''' Created by XBC 01/02/2012
    ''' </remarks>
    Private Sub ConfigUsers_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyDown
        Try
            If (e.KeyCode = Keys.Escape) Then
                'If (Me.BsExitButton.Enabled) Then
                '    Me.Close()
                'End If

                'RH 04/07/2011 Escape key should do exactly the same operations as bsExitButton_Click()
                BsExitButton.PerformClick()
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".KeyDown ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".KeyDown", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub IIseAdjustments_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyUp
        Try

            If (e.KeyCode = Keys.Up Or e.KeyCode = Keys.Down) Then

                Dim myGlobal As New GlobalDataTO
                Dim myISECommand As New ISECommandTO()
                Me.BsAdjustButton.Enabled = False

                If Me.BsActionsTreeView.SelectedNode IsNot Nothing Then
                    If Me.BsActionsTreeView.SelectedNode.Tag.ToString = "ISE_ALMS" Then
                        MyClass.CurrentActionNode = Me.BsActionsTreeView.SelectedNode
                        If e.KeyCode = Keys.Enter Then
                            MyClass.ExecuteISEAction(MyClass.CurrentActionNode)
                        Else
                            Me.BsAdjustButton.Enabled = True
                        End If
                    Else
                        If MyClass.ValidateNodeByBlock(Me.BsActionsTreeView.SelectedNode) Then
                            myGlobal = MyClass.ValidateActionNode(Me.BsActionsTreeView.SelectedNode.Tag.ToString)
                            If (Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing) Then
                                myISECommand = CType(myGlobal.SetDatos, ISECommandTO)
                                If myISECommand.ISECommandID <> ISECommands.NONE Then
                                    'SIMPLE ACTION NODE
                                    MyClass.CurrentActionNode = Me.BsActionsTreeView.SelectedNode
                                    MyClass.PrepareTimesUpDown(myISECommand.ISECommandID.ToString)
                                    Me.BsAdjustButton.Enabled = ActivateButtonWithAlarms(ActionButton.ISE_COMMAND) 'True

                                Else
                                    'COMPLEX ACTION NODE
                                    Me.myISEAction = ISECommands.NONE
                                    Me.BsTimesUpDown.Value = 1
                                    If MyClass.ValidateISEAction(Me.BsActionsTreeView.SelectedNode) Then
                                        MyClass.CurrentActionNode = Me.BsActionsTreeView.SelectedNode
                                        Me.BsAdjustButton.Enabled = MyClass.ValidateISEAvailability
                                    End If
                                End If
                            End If

                        End If
                    End If
                End If

                'SGM 08/012/2012
            ElseIf e.KeyCode = Keys.Enter Then
                If Me.BsActionsTreeView.SelectedNode IsNot Nothing Then
                    If Me.BsActionsTreeView.SelectedNode.Tag.ToString = "ISE_ALMS" Then
                        MyClass.CurrentActionNode = Me.BsActionsTreeView.SelectedNode
                        MyClass.ExecuteISEAction(MyClass.CurrentActionNode)
                    Else
                        Dim myNode As TreeNode = Me.BsActionsTreeView.SelectedNode
                        If Me.BsActionsTreeView.SelectedNode.Parent Is Nothing Then
                            If Not Me.BsActionsTreeView.SelectedNode.IsExpanded Then
                                Me.BsActionsTreeView.SelectedNode.Expand()
                            Else
                                Me.BsActionsTreeView.SelectedNode.Collapse()
                            End If
                            Me.BsActionsTreeView.SelectedNode = myNode
                            Dim myTag As String = CStr(myNode.Tag)
                            Me.BsActionsTreeView.Focus()
                            Me.BsActionsTreeView.SelectedNode = myNode
                        Else

                            Me.BsAdjustButton.Enabled = MyClass.ValidateISEAvailability
                            If BsAdjustButton.Enabled Then
                                Me.BsAdjustButton_Click(BsAdjustButton, Nothing)
                            End If

                        End If
                    End If
                End If
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".KeyUp ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".KeyUp", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub BsClearButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsClearButton.Click
        Me.BsRichTextBox1.Clear()
    End Sub

    Private Sub BsStopButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsStopButton.Click
        Try
            Me.NumRepetitionsSelectedByUser = 0
            Me.BsStopButton.Enabled = False 'SGM 05/09/2012
            Me.BsAdjustButton.Enabled = MyClass.ValidateISEAvailability 'SGM 25/09/2012
            'Me.PrepareTestedMode()

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".BsStopButton ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".BsStopButton", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub BsStopButton_MouseLeave(ByVal sender As Object, ByVal e As System.EventArgs) Handles BsStopButton.MouseLeave
        Me.Cursor = Cursors.WaitCursor
    End Sub

    Private Sub BsStopButton_MouseMove(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles BsStopButton.MouseMove
        Me.Cursor = Cursors.Hand
    End Sub

    Private Sub BsSaveAsButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsSaveAsButton.Click
        Try
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            With Me.SaveAsDialog
                .Title = myMultiLangResourcesDelegate.GetResourceText(Nothing, "MSG_FWSCRIPT_TEXT_EXPORT", currentLanguage)
                .InitialDirectory = My.Computer.FileSystem.SpecialDirectories.MyDocuments
                .Filter = "Text files|*.txt|All files|*.*"
                .CheckPathExists = True
                .OverwritePrompt = True
            End With

            Dim res As DialogResult = Me.SaveAsDialog.ShowDialog

            If res <> Windows.Forms.DialogResult.Cancel Then
                Me.DisplayMessage("")

                Dim myPath As String = Me.SaveAsDialog.FileName

                Me.CreateFileOutput(myPath)
            End If

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".SaveAsButton ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".SaveAsButtonown ", Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub BsRichTextBox1_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles BsRichTextBox1.GotFocus
        Try
            'hide Caret of the textbox
            MyClass.HideCaret(Me.BsRichTextBox1.Handle)
        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".BsRichTextBox1_GotFocus ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".BsRichTextBox1_GotFocus ", Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub


    Private Sub BsRichTextBox1_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles BsRichTextBox1.TextChanged
        Try
            Me.BsSaveAsButton.Enabled = (Me.BsRichTextBox1.Text.Length > 0)
            Me.BsClearButton.Enabled = (Me.BsRichTextBox1.Text.Length > 0)
            Me.BsRichTextBox1.ScrollToCaret() 'SGM 14/05/2012
        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".BsRichTextBox1_TextChanged ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".BsRichTextBox1_TextChanged ", Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

#End Region

#Region "Lithium Enabling/Disabling"
    ''' <summary>
    ''' Check if a Li+ Test is in Use
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>SGM 17/05/2012</remarks>
    Public Function LithiumTestInUse() As Boolean
        Dim myGlobal As New GlobalDataTO
        Dim InUse As Boolean = False
        Try
            Dim myISETestDelegate As New ISETestsDelegate
            myGlobal = myISETestDelegate.ExistsISETestName(Nothing, "Li+", "NAME")
            If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                Dim myISETestsDS As ISETestsDS = CType(myGlobal.SetDatos, ISETestsDS)
                If myISETestsDS.tparISETests.Count > 0 Then
                    InUse = CBool(myISETestsDS.tparISETests(0).InUse)
                End If
            End If

        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myGlobal.ErrorMessage = ex.Message

            MyBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".LithiumTestInUse", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".LithiumTestInUse", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
        Return InUse
    End Function

    ''' <summary>
    ''' Updates tparISETests table depending on the Li+ is enabled
    ''' </summary>
    ''' <param name="pEnabled"></param>
    ''' <returns></returns>
    ''' <remarks>SGM 17/05/2012</remarks>
    Public Function UpdateLithiumTestEnabled(ByVal pEnabled As Boolean) As GlobalDataTO
        Dim myGlobal As New GlobalDataTO
        Try
            Dim myISETestDelegate As New ISETestsDelegate

            myGlobal = myISETestDelegate.ExistsISETestName(Nothing, "Li+", "NAME")
            If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                Dim myISETestsDS As ISETestsDS = CType(myGlobal.SetDatos, ISETestsDS)
                If myISETestsDS.tparISETests.Count > 0 Then
                    myISETestsDS.tparISETests(0).Enabled = pEnabled
                    myGlobal = myISETestDelegate.Modify(Nothing, myISETestsDS)
                End If
            End If

        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myGlobal.ErrorMessage = ex.Message

            MyBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".UpdateLithiumTestEnabled", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".UpdateLithiumTestEnabled", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
        Return myGlobal
    End Function

#End Region


#Region "Information"

    ''' <summary>
    ''' gets information about a specific adjustment
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>Created by SG 03/01/11</remarks>
    Private Function DisplayInformation(ByVal pPage As APPLICATION_PAGES, ByVal pInfoTextControl As BsXPSViewer) As GlobalDataTO
        Dim myGlobal As New GlobalDataTO
        Try

            'Find document path
            Dim myDocumentPath As String = ""
            Dim myVideoPath As String = ""

            Dim myDelegate As New BaseFwScriptDelegate
            myGlobal = myDelegate.GetInformationDocument(Nothing, MyClass.AnalyzerModel, pPage.ToString)
            If Not myGlobal.HasError And myGlobal.SetDatos IsNot Nothing Then
                Dim myDocs As SRVInfoDocumentsDS = CType(myGlobal.SetDatos, SRVInfoDocumentsDS)
                If myDocs IsNot Nothing Then
                    Dim myDocRow As SRVInfoDocumentsDS.srv_tfmwInfoDocumentsRow = CType(myDocs.srv_tfmwInfoDocuments.Rows(0), SRVInfoDocumentsDS.srv_tfmwInfoDocumentsRow)

                    Dim myGlobalbase As New GlobalBase

                    If myDocRow.DocumentPath.Length > 0 Then
                        myDocumentPath = Application.StartupPath & myGlobalbase.ServiceInfoDocsPath & myDocRow.DocumentPath

                        Dim isScrollable As Boolean = myDocRow.Expandable

                        'Document
                        If myDocumentPath.Length > 0 Then
                            If myDocumentPath.Length > 0 Then
                                'show document
                                If System.IO.File.Exists(myDocumentPath) Then
                                    If pInfoTextControl IsNot Nothing Then
                                        With pInfoTextControl

                                            .IsScrollable = isScrollable

                                            .PopupMenuEnabled = False

                                            .PrintButtonVisible = False
                                            .WholePageButtonVisible = False
                                            .FitToWidthButtonVisible = False
                                            .FitToHeightButtonVisible = False
                                            .DecreaseZoomButtonVisible = False
                                            .IncreaseZoomButtonVisible = False
                                            .MenuBarVisible = False
                                            .SearchBarVisible = False

                                            'open document
                                            .Open(myDocumentPath)

                                            Application.DoEvents()

                                            'without margis
                                            .HorizontalPageMargin = 0
                                            .VerticalPageMargin = 0

                                            'readjust control size depending if the information is long or not
                                            If Not isScrollable Then
                                                .Left = .Left - 2
                                                .Top = .Top - 2
                                                .Width = .Width + 4
                                                .Height = .Height + 4
                                            Else
                                                .Left = .Left - 2
                                                .Top = .Top - 2
                                                .Width = .Width + 4
                                                .Height = .Height + 2
                                            End If
                                            'If pInfoExpandButton IsNot Nothing Then
                                            '    pInfoExpandButton.Visible = isExpandable
                                            'End If

                                            .RefreshPage()

                                        End With

                                    End If

                                End If
                            Else
                                myGlobal.HasError = True
                                myGlobal.ErrorCode = GlobalEnumerates.Messages.MASTER_DATA_MISSING.ToString


                                MyBase.CreateLogActivity(GlobalEnumerates.Messages.MASTER_DATA_MISSING.ToString, Me.Name & " GetInformationText ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
                                'Myclass.ShowMessage("Error", GlobalEnumerates.Messages.MASTER_DATA_MISSING.ToString)
                            End If

                        End If

                    End If

                    'video NOT V1
                    'If myDocRow.VideoPath.Length > 0 Then
                    '    myVideoPath = Application.StartupPath & myGlobalbase.ServiceInfoDocsPath & myDocRow.VideoPath
                    '    If File.Exists(myVideoPath) Then

                    '    Else
                    '        myGlobal.HasError = True
                    '        myGlobal.ErrorCode = GlobalEnumerates.Messages.MASTER_DATA_MISSING.ToString

                    '        MyBase.CreateLogActivity(GlobalEnumerates.Messages.MASTER_DATA_MISSING.ToString, Me.Name & " GetInformationText ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
                    '        MyClass.ShowMessage("Error", GlobalEnumerates.Messages.MASTER_DATA_MISSING.ToString)
                    '    End If
                    'End If
                End If
            End If


        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myGlobal.ErrorMessage = ex.Message
            MyBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " GetInformationText ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyClass.ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try

        Return myGlobal

    End Function


#End Region

#Region "XPS Info Events"

    Private Sub BsXPSViewer_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles BsInfoXPSViewer.Load
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
            MyBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".BsXPSViewer_Load ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".BsXPSViewer_Load ", Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

#End Region

    '' XBC 31/08/2012 - Que es esto ??? - se comenta por que no se entiende su sentido - se puede recuperar
    ' cuando se de explicacion para que era y NO interfiera en el cierre de la pantalla
    'Private Sub IISEUtilities_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
    '    Try
    '        If MyClass.CurrentOperation <> Nothing Then
    '            e.Cancel = True
    '        End If
    '    Catch ex As Exception
    '        MyBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".FormClosing ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
    '        MyBase.ShowMessage(Me.Name & ".FormClosing ", Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
    '    End Try
    'End Sub


End Class