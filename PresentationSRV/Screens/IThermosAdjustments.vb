Option Explicit On
Option Strict On

Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Global.GlobalEnumerates
Imports Biosystems.Ax00.FwScriptsManagement
Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.App


Public Class UiThermosAdjustments
    Inherits PesentationLayer.BSAdjustmentBaseForm

#Region "Enumerations"
    Private Enum THERMOS_PAGES
        ROTOR
        NEEDLES
        HEATER
        'FRIDGE canceled by now
    End Enum

    Private Enum LastConditioningDoneEnum
        _None
        ConditioningRotor
        ConditioningReagent1
        ConditioningReagent2
        ConditioningHeater
    End Enum
#End Region

#Region "Declarations"
    'Screen's business delegate
    Private WithEvents myScreenDelegate As ThermosAdjustmentsDelegate

    ' Language
    Private currentLanguage As String

    Private ActiveAnalyzerModel As String

    Private SelectedPage As THERMOS_PAGES

    Private SelectedAdjustmentsDS As New SRVAdjustmentsDS
    Private TemporalAdjustmentsDS As New SRVAdjustmentsDS
    Private TempToSendAdjustmentsDelegate As FwAdjustmentsDelegate
    ' Edited value
    Private EditedValue As EditedValueStruct

    'determines if the preliminary conditioning (needle priming) has been already performed)
    Private LastConditioningDone As LastConditioningDoneEnum

    Private myDecimalSeparator As String

    Private NeedlesConditioningStopRequested As Boolean = False

    'Information
    Private SelectedInfoPanel As Panel
    Private SelectedAdjPanel As Panel
    Private SelectedInfoExpandButton As Panel




#End Region

#Region "Attributes"
    Private IsRotorConditioningRequestedAttr As Boolean = False 'SGM 01/12/2011
    Private IsRotorConditioningCancelRequestedAttr As Boolean = False 'SGM 02/12/2011
#End Region

#Region "Properties"
    Public WriteOnly Property Tab1UndoMeasure() As Boolean
        Set(ByVal value As Boolean)
            Me.Tab1TextBoxTemp1.Enabled = value
            Me.Tab1TextBoxTemp2.Enabled = value
            Me.Tab1TextBoxTemp3.Enabled = value
            Me.Tab1TextBoxTemp4.Enabled = value
        End Set
    End Property

    ''' <summary>
    ''' Informs if the rotor is being conditioned
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks>Created by SGM 01/12/2011</remarks>
    Private Property IsRotorConditioningRequested() As Boolean
        Get
            Return IsRotorConditioningRequestedAttr
        End Get
        Set(ByVal value As Boolean)

            IsRotorConditioningRequestedAttr = value

            Try
                Dim myGlobal As New GlobalDataTO
                Dim auxIconName As String = String.Empty
                Dim iconPath As String = MyBase.IconsPath
                Dim myRotorCondImage As Image
                Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

                If value Then

                    myServiceMDI.SEND_INFO_STOP()

                    auxIconName = GetIconName("STOP")

                    If IO.File.Exists(iconPath & auxIconName) Then
                        Dim myImage As Image
                        myImage = ImageUtilities.ImageFromFile(iconPath & auxIconName)

                        myGlobal = Utilities.ResizeImage(myImage, New Size(26, 26))
                        If Not myGlobal.HasError And myGlobal.SetDatos IsNot Nothing Then
                            myRotorCondImage = CType(myGlobal.SetDatos, Bitmap)
                        Else
                            myRotorCondImage = CType(myImage, Bitmap)
                        End If
                    Else
                        myRotorCondImage = Nothing
                    End If

                    bsScreenToolTipsControl.SetToolTip(Me.Tab1ConditioningButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "SRV_BTN_TestStop", currentLanguage))

                Else
                    myServiceMDI.SEND_INFO_START()

                    auxIconName = GetIconName("ADJUSTMENT")
                    If System.IO.File.Exists(iconPath & auxIconName) Then
                        myRotorCondImage = ImageUtilities.ImageFromFile(iconPath & auxIconName)
                    Else
                        myRotorCondImage = Nothing
                    End If

                    bsScreenToolTipsControl.SetToolTip(Me.Tab1ConditioningButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "SRV_BTN_Conditionate", currentLanguage))

                End If

                If myRotorCondImage IsNot Nothing Then
                    Me.Tab1ConditioningButton.Image = myRotorCondImage
                End If

                Me.Tab1ConditioningButton.Cursor = Cursors.Default

            Catch ex As Exception
                Throw ex
            End Try

        End Set
    End Property

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks>SGM 02/12/2011</remarks>
    Private Property IsRotorConditioningCancelRequested() As Boolean
        Get
            Return IsRotorConditioningCancelRequestedAttr
        End Get
        Set(ByVal value As Boolean)
            If IsRotorConditioningCancelRequestedAttr <> value Then
                IsRotorConditioningCancelRequestedAttr = value
                If IsRotorConditioningRequested Then
                    Select Case myScreenDelegate.CurrentOperation
                        Case ThermosAdjustmentsDelegate.OPERATIONS.CONDITIONING_ROTOR, ThermosAdjustmentsDelegate.OPERATIONS.CONDITIONING_MANUAL_DOWN, ThermosAdjustmentsDelegate.OPERATIONS.CONDITIONING_MANUAL_UP
                            If value Then
                                myScreenDelegate.RotorConditioningCancelRequested = True
                            End If

                        Case ThermosAdjustmentsDelegate.OPERATIONS.ROTATING_ROTOR
                            If value Then
                                myScreenDelegate.RotorRotatingCancelRequested = True
                                myScreenDelegate.IsKeepRotating = False
                            End If

                        Case Else
                            IsRotorConditioningCancelRequestedAttr = False
                            IsRotorConditioningRequested = False
                    End Select

                Else
                    IsRotorConditioningCancelRequestedAttr = False
                End If
            End If
        End Set
    End Property

#End Region

#Region "Variables"
    Private ManageTabPages As Boolean
    Private UnableHandlers As Boolean
    Private TempBoxEditing As Integer
    Private EditingRotorTemperatures As Boolean
    Private EditingRotorCorrection As Boolean
    Private EditingNeedleTemperature As Boolean
    Private EditingNeedleCorrection As Boolean
    Private EditingHeaterTemperature As Boolean
    Private EditingHeaterCorrection As Boolean

    Private PageInitialized As Boolean
#End Region

#Region "Structures"
    Private Structure EditedValueStruct
        ' Identifier
        Public AdjustmentID As GlobalEnumerates.ADJUSTMENT_GROUPS
        ' Maximum allowed SetPoint value 
        Public SetPointMaxValue As Single
        ' Minimum allowed SetPoint value 
        Public SetPointMinValue As Single
        ' Current SetPoint value 
        Public SetPointCurrentValue As Single
        ' New SetPoint value
        Public SetPointNewValue As Single
        ' Target value
        Public TargetValue As Single
        ' Maximum allowed Target value 
        Public TargetMaxValue As Single
        ' Minimum allowed Target value 
        Public TargetMinValue As Single
        ' Correction
        Public Correction As Single
        ' New Correction
        Public NewCorrection As Single
    End Structure

#End Region

#Region "Constructor"
    Sub New()
        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        DefineScreenLayout(THERMOS_PAGES.HEATER)
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
    ''' <remarks>Created by XBC 15/06/11</remarks>
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
                            MyClass.PrepareArea()
                        Else
                            MyClass.PrepareErrorMode()
                            Exit Function
                        End If
                    End If

                Case ADJUSTMENT_MODES.TESTED
                    If pResponse = RESPONSE_TYPES.START Then
                        'MyClass.PrepareArea()
                    ElseIf pResponse = RESPONSE_TYPES.OK Then
                        MyClass.PrepareArea()
                    Else
                        MyClass.PrepareErrorMode()
                        Exit Function
                    End If


                Case ADJUSTMENT_MODES.SAVED
                    If pResponse = RESPONSE_TYPES.START Then
                        ' Nothing by now
                    End If
                    If pResponse = RESPONSE_TYPES.OK Then
                        MyClass.PrepareArea()
                    End If

                Case ADJUSTMENT_MODES.PARKED
                    If pResponse = RESPONSE_TYPES.OK Then
                        PrepareArea()
                    End If


                Case ADJUSTMENT_MODES.ERROR_MODE
                    MyBase.DisplayMessage(Messages.FWSCRIPT_DATA_ERROR.ToString)
                    MyClass.PrepareErrorMode()
            End Select

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".ScreenReceptionLastFwScriptEvent ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".ScreenReceptionLastFwScriptEvent", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, myGlobal.ErrorMessage, Me)
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
    ''' <remarks>Created by XBC 29/06/2011</remarks>
    Public Overrides Sub RefreshScreen(ByVal pRefreshEventType As List(Of GlobalEnumerates.UI_RefreshEvents), ByVal pRefreshDS As Biosystems.Ax00.Types.UIRefreshDS)
        Dim myGlobal As New GlobalDataTO
        Try
            If pRefreshEventType.Contains(GlobalEnumerates.UI_RefreshEvents.SENSORVALUE_CHANGED) Then
                Dim mySensorValuesChangedDT As New UIRefreshDS.SensorValueChangedDataTable

                mySensorValuesChangedDT = pRefreshDS.SensorValueChanged

                For Each S As UIRefreshDS.SensorValueChangedRow In mySensorValuesChangedDT.Rows
                    Select Case S.SensorID
                        Case AnalyzerSensors.TEMPERATURE_REACTIONS.ToString
                            myScreenDelegate.MonitorThermoRotor = S.Value
                        Case AnalyzerSensors.TEMPERATURE_R1.ToString
                            myScreenDelegate.MonitorThermoR1 = S.Value
                        Case AnalyzerSensors.TEMPERATURE_R2.ToString
                            myScreenDelegate.MonitorThermoR2 = S.Value
                        Case AnalyzerSensors.TEMPERATURE_WASHINGSTATION.ToString
                            myScreenDelegate.MonitorThermoHeater = S.Value
                    End Select
                Next
            Else
                myScreenDelegate.RefreshDelegate(pRefreshEventType, pRefreshDS)

                'if needed manage the event in the Base Form
                MyBase.OnReceptionLastFwScriptEvent(RESPONSE_TYPES.OK, Nothing)

                PrepareArea()
            End If


        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".RefreshScreen ", EventLogEntryType.Error, _
                                                                    GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".RefreshScreen", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub


#Region "Must Inherited"
    ''' <summary>
    ''' It manages the Stop of the operation(s) that are currently being performed. 
    ''' </summary>
    ''' <param name="pAlarmType"></param>
    ''' <remarks>
    ''' Created by SGM 19/10/2012
    ''' Updated by XBC 23/10/2012 - Add business logic with every current operation
    ''' </remarks>
    Public Overrides Sub StopCurrentOperation(Optional ByVal pAlarmType As ManagementAlarmTypes = ManagementAlarmTypes.NONE)
        Try
            ' Stop FwScripts
            myFwScriptDelegate.StopFwScriptQueue()

            PrepareErrorMode()

            'when stop action is finished, perform final operations after alarm received
            MyBase.myServiceMDI.ManageAlarmStep2(pAlarmType)

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".StopCurrentOperation ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
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
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".InitializeHomes ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".InitializeHomes", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
        Return myGlobal
    End Function

    ''' <summary>
    ''' Assigns the specific screen's elements to the common screen layout structure
    ''' </summary>
    ''' <remarks>XBC 15/06/2011</remarks>
    Private Sub DefineScreenLayout(ByVal pPage As THERMOS_PAGES)
        Try
            With MyBase.myScreenLayout
                .MessagesPanel.Container = Me.BsMessagesPanel
                .MessagesPanel.Icon = Me.BsMessageImage
                .MessagesPanel.Label = Me.BsMessageLabel

                Select Case pPage
                    Case THERMOS_PAGES.HEATER
                        .AdjustmentPanel.Container = Me.BsWSHeaterAdjustPanel
                        .InfoPanel.Container = Me.BsWSHeaterInfoPanel
                        .InfoPanel.InfoXPS = BsInfoWsHeaterXPSViewer

                    Case THERMOS_PAGES.ROTOR
                        .AdjustmentPanel.Container = Me.BsReactionsRotorAdjustPanel
                        .InfoPanel.Container = Me.BsReactionsRotorInfoPanel
                        .InfoPanel.InfoXPS = BsInfoRotorXPSViewer

                    Case THERMOS_PAGES.NEEDLES
                        .AdjustmentPanel.Container = Me.BsReagentsNeedlesAdjustPanel
                        .InfoPanel.Container = Me.BsReagentsNeedlesInfoPanel
                        .InfoPanel.InfoXPS = BsInfoNeedlesXPSViewer


                    Case Else
                        .AdjustmentPanel.Container = Nothing
                        .InfoPanel.Container = Nothing
                        .InfoPanel.InfoXPS = Nothing

                End Select
            End With

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".DefineScreenLayout ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".DefineScreenLayout ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub Initializations()
        Dim myGlobal As New GlobalDataTO
        Try
            Me.SelectedPage = THERMOS_PAGES.HEATER
            ' Hide Fridge TAB because will not use for the moment
            Me.BsTabPagesControl.TabPages.Remove(TabFridge)

            Me.InitializeRotorReationsTemps()

            ' Instantiate a new EditionValue structure
            Me.EditedValue = New EditedValueStruct
            With Me.EditedValue
                Select Case Me.SelectedPage
                    Case THERMOS_PAGES.ROTOR
                        .AdjustmentID = ADJUSTMENT_GROUPS.THERMOS_PHOTOMETRY
                    Case THERMOS_PAGES.NEEDLES
                        .AdjustmentID = ADJUSTMENT_GROUPS.THERMOS_REAGENT1
                    Case THERMOS_PAGES.HEATER
                        .AdjustmentID = ADJUSTMENT_GROUPS.THERMOS_WS_HEATER
                End Select
            End With

            myGlobal = GetParameters()
            If myGlobal.HasError Then
                PrepareErrorMode()
            End If

            myScreenDelegate.ReactionsRotorPrepareWellDone = New List(Of Boolean)
            myScreenDelegate.ProbeWellsTemps = New List(Of Single)
            For i As Integer = 0 To myScreenDelegate.ReactionsRotorMeasuredWellsCount - 1
                myScreenDelegate.ReactionsRotorPrepareWellDone.Add(False)
                myScreenDelegate.ProbeWellsTemps.Add(0)
            Next

            myScreenDelegate.ReactionsRotorWellFilled = New List(Of Boolean)
            For i As Integer = 0 To myScreenDelegate.ReactionsRotorWellsFilledCount - 1
                myScreenDelegate.ReactionsRotorWellFilled.Add(False)
            Next

            myScreenDelegate.ReactionsRotorRotationsDone = New List(Of Boolean)
            For i As Integer = 0 To myScreenDelegate.ReactionsRotorRotatingTimes - 1
                myScreenDelegate.ReactionsRotorRotationsDone.Add(False)
            Next

            'Reagents Needles priming not done
            myScreenDelegate.ReagentNeedleConditioningDone = False

            Me.Tab1UndoMeasure = True

            Me.EditingRotorTemperatures = False

            Me.LastConditioningDone = LastConditioningDoneEnum._None

            If MyBase.CurrentUserNumericalLevel = USER_LEVEL.lOPERATOR Then
                Me.BsSaveButton.Visible = False
            End If

            MyClass.myDecimalSeparator = SystemInfoManager.OSDecimalSeparator

            Me.ProgressBar1.Visible = False

            Me.PageInitialized = True

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".Initializations ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".Initializations ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="model"></param>
    ''' <remarks>
    ''' Created by:  IT 21/07/2015 - BA-2649
    ''' Modified by: IT 21/07/2015 - BA-2650
    ''' </remarks>
    Private Sub ApplyElementsVisibility(model As String)
        Select Case model
            Case AnalyzerModelEnum.A200.ToString
                'Reaction Rotor Tab2
                Label4.Visible = False
                Label7.Visible = False
                Tab1TextBoxTemp3.Visible = False
                Tab1TextBoxTemp4.Visible = False

                'Reagents Tips Tab3
                Tab2RadioButtonR2.Visible = False

        End Select
    End Sub

    Private Sub InitializeRotorReationsTemps()
        Try
            Me.Tab1TextBoxTemp1.Text = ""
            Me.Tab1TextBoxTemp2.Text = ""
            Me.Tab1TextBoxTemp3.Text = ""
            Me.Tab1TextBoxTemp4.Text = ""

            Me.TabMeanTempLabel.Text = ""
            Me.Tab1CorrectionTextBox.Text = ""

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".InitializeRotorReationsTemps ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".InitializeRotorReationsTemps ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Generic function to send FwScripts to the Instrument through own delegates
    ''' </summary>
    ''' <param name="pMode"></param>
    ''' <remarks>Created by: XBC 15/06/2011</remarks>
    Private Sub SendFwScript(ByVal pMode As ADJUSTMENT_MODES)
        Dim myGlobal As New GlobalDataTO
        Try
            If Not myGlobal.HasError AndAlso AnalyzerController.Instance.Analyzer.Connected Then '#REFACTORING
                myScreenDelegate.NoneInstructionToSend = True
                myGlobal = myScreenDelegate.SendFwScriptsQueueList(pMode)
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
                GlobalBase.CreateLogActivity(myGlobal.ErrorCode, Me.Name & ".SendFwScript ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
                MyBase.ShowMessage(Me.Name & ".SendFwScript ", myGlobal.ErrorCode, myGlobal.ErrorMessage, Me)
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".SendFwScript ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".SendFwScript ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Get texts in the current application language for all screen controls
    ''' </summary>
    ''' <remarks>
    ''' Created by:  XBC 15/06/2011
    ''' Modified by: IT  21/07/2015 - BA-2650
    ''' </remarks>
    Private Sub GetScreenLabels()
        Try
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            'For Labels, CheckBox, RadioButtons.....
            Me.TabWSHeater.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_WSHEATER", currentLanguage)
            Me.TabReactionsRotor.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_REACT_ROTOR", currentLanguage)
            Me.TabReagentsNeedles.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_REAG_NEEDLES", currentLanguage)
            Me.TabFridge.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_FRIDGE", currentLanguage)
            ' TAB ROTOR
            Me.ReactionsRotorAdjustTitle.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_ROTORTEMP_TITLE", currentLanguage)
            Me.Tab1AutoRadioButton.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_AUTOMATIC_FILL", currentLanguage)
            Me.Tab1ManualRadioButton.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_MANUALLY_FILL", currentLanguage)
            Me.Tab1ConditioningGroupBox.Text = "1. " & myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_CONDITIONING", currentLanguage)
            Me.Tab1MeasurementGroupBox.Text = "2. " & myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_TEMP_MES", currentLanguage)
            Me.Tab1AdjustGroupBox.Text = "3. " & myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_ADJUST", currentLanguage)
            Me.Tab1MeanLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Mean", currentLanguage) 'JB 01/10/2012 - Resource String unification
            Me.Tab1AdjustProposedLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_ROT_TEMP_PROS", currentLanguage)
            Me.Tab1RotorReactLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_PLACE_REACTIONS_ROTOR", currentLanguage)
            ' TAB NEEDLES
            Me.BsReagentsNeedlesAdjustTitle.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_NEEDLESTEMP_TITLE", currentLanguage)
            Me.Tab2SelectArmGroupBox.Text = "1. " & myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_SELECT_NEEDLE", currentLanguage)
            Me.Tab2ConditioningGroupBox.Text = "2. " & myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_CONDITIONING", currentLanguage)
            Me.Tab2MeasurementGroupBox.Text = "3. " & myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_TEMP_MES", currentLanguage)
            Me.Tab2AdjustGroupBox.Text = "4. " & myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_ADJUST", currentLanguage)

            'IT 21/07/2015 - BA-2650
            If AnalyzerController.Instance.IsBA200 Then
                Me.Tab2RadioButtonR1.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_SamplesAndReagentsArm", currentLanguage)
            Else
                Me.Tab2RadioButtonR1.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_Reagent1Arm", currentLanguage)
                Me.Tab2RadioButtonR2.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_Reagent2Arm", currentLanguage)
            End If

            Me.Tab2ConditioningDescLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_CONDITIONING_DESC1", currentLanguage)
            Me.Tab2TempMeasuredLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_THERMO_NEEDLE_PROC", currentLanguage)
            Me.Tab2AdjustProposedLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_REAG_TEMP_PROS", currentLanguage)
            ' TAB HEATER
            Me.BsWSHeaterAdjustTitle.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_HEATERTEMP_TITLE", currentLanguage)
            Me.Tab3ConditioningGroupBox.Text = "1. " & myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_CONDITIONING", currentLanguage)
            Me.Tab3MeasurementGroupBox.Text = "2. " & myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_TEMP_MES", currentLanguage)
            Me.Tab3AdjustGroupBox.Text = "3. " & myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_ADJUST", currentLanguage)
            Me.Tab3ConditioningDescLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_CONDITIONING_DESC2", currentLanguage)
            Me.Tab3TempMeasuredLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_MEASURED", currentLanguage)
            Me.Tab3AdjustProposedLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_HEATER_TEMP_PROS", currentLanguage)
            Me.Tab3HeaterWSLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_PLACE_REACTIONS_ROTOR", currentLanguage)

            'Information
            With BsInfoWsHeaterXPSViewer
                .PrintButtonCaption = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_XPS_PRINT", currentLanguage)
                .CopyButtonCaption = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_XPS_COPY", currentLanguage)
                .IncreaseZoomButtonCaption = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_XPS_ZOOM_IN", currentLanguage)
                .DecreaseZoomButtonCaption = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_XPS_ZOOM_OUT", currentLanguage)
                .FitToWidthButtonCaption = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_XPS_TO_WIDTH", currentLanguage)
                .FitToHeightButtonCaption = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_XPS_TO_HEIGHT", currentLanguage)
                .WholePageButtonCaption = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_XPS_WHOLE", currentLanguage)
                .TwoPagesButtonCaption = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_XPS_TWO_PAGES", currentLanguage)
            End With
            With BsInfoRotorXPSViewer
                .PrintButtonCaption = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_XPS_PRINT", currentLanguage)
                .CopyButtonCaption = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_XPS_COPY", currentLanguage)
                .IncreaseZoomButtonCaption = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_XPS_ZOOM_IN", currentLanguage)
                .DecreaseZoomButtonCaption = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_XPS_ZOOM_OUT", currentLanguage)
                .FitToWidthButtonCaption = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_XPS_TO_WIDTH", currentLanguage)
                .FitToHeightButtonCaption = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_XPS_TO_HEIGHT", currentLanguage)
                .WholePageButtonCaption = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_XPS_WHOLE", currentLanguage)
                .TwoPagesButtonCaption = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_XPS_TWO_PAGES", currentLanguage)
            End With
            With BsInfoNeedlesXPSViewer
                .PrintButtonCaption = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_XPS_PRINT", currentLanguage)
                .CopyButtonCaption = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_XPS_COPY", currentLanguage)
                .IncreaseZoomButtonCaption = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_XPS_ZOOM_IN", currentLanguage)
                .DecreaseZoomButtonCaption = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_XPS_ZOOM_OUT", currentLanguage)
                .FitToWidthButtonCaption = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_XPS_TO_WIDTH", currentLanguage)
                .FitToHeightButtonCaption = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_XPS_TO_HEIGHT", currentLanguage)
                .WholePageButtonCaption = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_XPS_WHOLE", currentLanguage)
                .TwoPagesButtonCaption = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_XPS_TWO_PAGES", currentLanguage)
            End With

            ' Tooltips
            GetScreenTooltip()

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Name & ".GetScreenLabels", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Name & ".GetScreenLabels", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
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
            MyBase.bsScreenToolTipsControl.SetToolTip(BsSaveButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Save", currentLanguage))
            MyBase.bsScreenToolTipsControl.SetToolTip(BsExitButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_CloseScreen", currentLanguage))

            MyBase.bsScreenToolTipsControl.SetToolTip(Tab1ConditioningButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "SRV_BTN_Conditionate", currentLanguage))
            MyBase.bsScreenToolTipsControl.SetToolTip(Tab1AdjustButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_ADJUST", currentLanguage)) 'JB 01/10/2012 - Resource String uification

            MyBase.bsScreenToolTipsControl.SetToolTip(Tab2ConditioningButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "SRV_BTN_Conditionate", currentLanguage))
            MyBase.bsScreenToolTipsControl.SetToolTip(Tab2MeasureButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "SRV_BTN_MeasureTemp", currentLanguage))
            MyBase.bsScreenToolTipsControl.SetToolTip(Tab2AdjustButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_ADJUST", currentLanguage)) 'JB 01/10/2012 - Resource String unification

            MyBase.bsScreenToolTipsControl.SetToolTip(Tab3ConditioningButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "SRV_BTN_Conditionate", currentLanguage))
            MyBase.bsScreenToolTipsControl.SetToolTip(Tab3AdjustButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_ADJUST", currentLanguage)) 'JB 01/10/2012 - Resource String unification

            ' XBC 20/04/2012
            MyBase.bsScreenToolTipsControl.SetToolTip(BsUpDownWSButton1, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_UPDOWN_WS", currentLanguage))
            MyBase.bsScreenToolTipsControl.SetToolTip(BsUpDownWSButton2, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_UPDOWN_WS", currentLanguage))

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Name & ".GetScreenTooltip ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Name & ".GetScreenTooltip ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Set all the screen elements to disabled
    ''' </summary>
    ''' <remarks>Created by: XBC 15/06/2011</remarks>
    Private Sub DisableAll()
        Try
            ' TAB ROTOR
            Me.Tab1AutoRadioButton.Enabled = False
            Me.Tab1ManualRadioButton.Enabled = False
            Me.Tab1ConditioningButton.Enabled = True 'it remains enabled to allow to stop conditioning

            Me.Tab1UndoMeasure = False
            Me.Tab1CorrectionTextBox.Enabled = False
            Me.Tab1AdjustButton.Enabled = False

            ' XBC 20/04/2012
            Me.BsUpDownWSButton1.Enabled = False
            Me.BsUpDownWSButton2.Enabled = False

            ' TAB NEEDLES
            Me.Tab2RadioButtonR1.Enabled = False
            Me.Tab2RadioButtonR2.Enabled = False
            Me.Tab2ConditioningButton.Enabled = False
            Me.Tab2MeasureButton.Enabled = False
            Me.Tab2TextBoxTemp.Enabled = False
            Me.Tab2MeasureButton.Enabled = False
            Me.Tab2CorrectionTextBox.Enabled = False
            Me.Tab2AdjustButton.Enabled = False
            Me.Tab2AuxButton.Enabled = False

            ' TAB HEATER
            Me.Tab3ConditioningButton.Enabled = False
            Me.Tab3TextBoxTemp.Enabled = False
            Me.Tab3CorrectionTextBox.Enabled = False

            Me.Tab3AdjustButton.Enabled = False

            ' Disable Area Buttons
            Me.BsSaveButton.Enabled = False
            Me.BsExitButton.Enabled = False

            ' Disable Tabs 
            Me.ManageTabPages = False

            MyBase.ActivateMDIMenusButtons(False)


        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".DisableAll ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".DisableAll ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Get Parameters values from BD for Thermos Tests
    ''' </summary>
    ''' <remarks>Created by XBC 16/06/2011</remarks>
    Private Function GetParameters() As GlobalDataTO
        Dim myGlobal As New GlobalDataTO
        Try
            Me.ActiveAnalyzerModel = Ax00ServiceMainMDI.ActiveAnalyzerModel
            myGlobal = myScreenDelegate.GetParameters(Me.ActiveAnalyzerModel)
            If myGlobal.HasError Then
                PrepareErrorMode()
                Exit Try
            End If

            myGlobal = GetLimitValues()
            If myGlobal.HasError Then
                PrepareErrorMode()
                Exit Try
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".GetParameters ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".GetParameters ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
        Return myGlobal
    End Function

    ''' <summary>
    ''' Get Limits values from BD for every different thermo
    ''' </summary>
    ''' <remarks>Created by XBC 17/06/2011</remarks>
    Private Function GetLimitValues() As GlobalDataTO
        Dim myGlobal As New GlobalDataTO
        Try
            ' Get Value limit ranges
            Dim myFieldLimitsDelegate As New FieldLimitsDelegate()
            Dim myFieldLimitsDS As New FieldLimitsDS

            With Me.EditedValue
                'Load the specified limits for SETPOINTS values
                Select Case .AdjustmentID
                    Case ADJUSTMENT_GROUPS.THERMOS_PHOTOMETRY
                        myGlobal = myFieldLimitsDelegate.GetList(Nothing, FieldLimitsEnum.SRV_THERMO_PHOTOMETRY_SETPOINT)
                    Case ADJUSTMENT_GROUPS.THERMOS_REAGENT1
                        myGlobal = myFieldLimitsDelegate.GetList(Nothing, FieldLimitsEnum.SRV_THERMO_PROBE_R1_SETPOINT)
                    Case ADJUSTMENT_GROUPS.THERMOS_REAGENT2
                        'AG 10/11/2014 BA-2082 filter by model those parameters with changes
                        myGlobal = myFieldLimitsDelegate.GetList(Nothing, FieldLimitsEnum.SRV_THERMO_PROBE_R2_SETPOINT, ActiveAnalyzerModel)
                    Case ADJUSTMENT_GROUPS.THERMOS_WS_HEATER
                        myGlobal = myFieldLimitsDelegate.GetList(Nothing, FieldLimitsEnum.SRV_THERMO_HEATER_SETPOINT)
                End Select

                If (Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing) Then
                    myFieldLimitsDS = CType(myGlobal.SetDatos, FieldLimitsDS)
                    If myFieldLimitsDS.tfmwFieldLimits.Rows.Count > 0 Then
                        .SetPointMinValue = CType(myFieldLimitsDS.tfmwFieldLimits(0).MinValue, Decimal)
                        .SetPointMaxValue = CType(myFieldLimitsDS.tfmwFieldLimits(0).MaxValue, Decimal)
                    End If
                End If

                'Load the specified limits for TARGET values
                Select Case .AdjustmentID
                    Case ADJUSTMENT_GROUPS.THERMOS_PHOTOMETRY
                        myGlobal = myFieldLimitsDelegate.GetList(Nothing, FieldLimitsEnum.THERMO_REACTIONS_LIMIT) 'SRV_REACTIONS_ROTOR_TEMP_LIMIT)
                    Case ADJUSTMENT_GROUPS.THERMOS_REAGENT1
                        myGlobal = myFieldLimitsDelegate.GetList(Nothing, FieldLimitsEnum.THERMO_ARMS_LIMIT) 'SRV_R1_PROBE_TEMP_LIMIT)
                    Case ADJUSTMENT_GROUPS.THERMOS_REAGENT2
                        myGlobal = myFieldLimitsDelegate.GetList(Nothing, FieldLimitsEnum.THERMO_ARMS_LIMIT) 'SRV_R2_PROBE_TEMP_LIMIT)
                    Case ADJUSTMENT_GROUPS.THERMOS_WS_HEATER
                        myGlobal = myFieldLimitsDelegate.GetList(Nothing, FieldLimitsEnum.THERMO_WASHSTATION_LIMIT) 'SRV_WS_HEATER_TEMP_LIMIT)
                End Select

                If (Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing) Then
                    myFieldLimitsDS = CType(myGlobal.SetDatos, FieldLimitsDS)
                    If myFieldLimitsDS.tfmwFieldLimits.Rows.Count > 0 Then
                        .TargetMinValue = CType(myFieldLimitsDS.tfmwFieldLimits(0).MinValue, Decimal)
                        .TargetMaxValue = CType(myFieldLimitsDS.tfmwFieldLimits(0).MaxValue, Decimal)
                    End If
                End If

            End With

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".GetLimitValues ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".GetLimitValues ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
        Return myGlobal
    End Function

    ''' <summary>
    ''' Loads the icons and tooltips used for painting the buttons
    ''' </summary>
    ''' <remarks>Created by: XBC 15/06/2011</remarks>
    Private Sub PrepareButtons()
        Dim auxIconName As String = ""
        Dim iconPath As String = MyBase.IconsPath
        Try

            ' Hidden buttons for these tests
            Me.BsAdjustButtonNOUSED.Visible = False
            Me.BsSaveButtonNOUSED.Visible = False

            'dl 20/04/2012
            auxIconName = GetIconName("ADJUSTMENT")
            If (auxIconName <> "") Then
                Tab1ConditioningButton.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
                Tab1AdjustButton.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
                Tab2ConditioningButton.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
                Tab2MeasureButton.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
                Tab2AdjustButton.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
                Tab3ConditioningButton.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
                Tab3AdjustButton.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
                Tab3AdjustButton.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
            End If

            auxIconName = GetIconName("SAVE")
            If (auxIconName <> "") Then
                BsSaveButton.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
            End If

            auxIconName = GetIconName("UPDOWN")
            If (auxIconName <> "") Then
                BsUpDownWSButton1.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
                BsUpDownWSButton2.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
            End If

            auxIconName = GetIconName("CANCEL")
            If (auxIconName <> "") Then
                BsExitButton.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
            End If            

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareButtons", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareButtons", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Prepare Tab Area according with current operations
    ''' </summary>
    ''' <remarks>Created by: XBC 15/06/2011</remarks>
    Private Sub PrepareArea()
        Try
            Application.DoEvents()

            ' Enabling/desabling form components to this child screen
            Select Case MyBase.CurrentMode

                Case ADJUSTMENT_MODES.ADJUSTMENTS_READING
                    MyBase.DisplayMessage(Messages.SRV_READ_ADJUSTMENTS.ToString)

                Case ADJUSTMENT_MODES.ADJUSTMENTS_READED
                    MyBase.DisplayMessage(Messages.SRV_ADJUSTMENTS_READED.ToString)
                    MyClass.PrepareLoadedMode()

                    MyClass.LoadAdjustmentsData()
                    MyClass.LoadAdjustmentGroupData()
                    MyClass.PopulateEditionValues()

                Case ADJUSTMENT_MODES.LOADED

                    MyClass.PrepareLoadedMode()

                Case ADJUSTMENT_MODES.TESTING

                    MyClass.PrepareTestingMode()


                Case ADJUSTMENT_MODES.TESTED

                    MyClass.PrepareTestedMode()

                Case ADJUSTMENT_MODES.SAVING

                    MyClass.PrepareSavingMode()

                Case ADJUSTMENT_MODES.SAVED
                    MyBase.DisplayMessage(Messages.SRV_ADJUSTMENTS_SAVED.ToString)
                    MyClass.PrepareSavedMode()

                    ' XBC 20/04/2012
                Case ADJUSTMENT_MODES.PARKING
                    Me.PrepareParkingMode()

                    ' XBC 20/04/2012
                Case ADJUSTMENT_MODES.PARKED
                    Me.PrepareParkedMode()

                Case ADJUSTMENT_MODES.ERROR_MODE
                    MyClass.PrepareErrorMode()

            End Select

            If Not MyBase.SimulationMode And AnalyzerController.Instance.Analyzer.AnalyzerStatus = AnalyzerManagerStatus.SLEEPING Then '#REFACTORING
                MyClass.PrepareErrorMode()
                MyBase.DisplayMessage("")
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareArea ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareArea ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Prepare GUI for Adjustments Reading Mode
    ''' </summary>
    ''' <remarks>Created by XBC 15/06/2011</remarks>
    Private Sub PrepareAdjustReadingMode()
        Dim myGlobal As New GlobalDataTO
        Try
            DisableAll()
            Me.Cursor = Cursors.WaitCursor

            ' Reading Adjustments
            If Not MyClass.myServiceMDI.AdjustmentsReaded Then
                ' Parent Reading Adjustments
                MyBase.ReadAdjustments()
                PrepareArea()

                ' Manage FwScripts must to be sent at load screen
                If MyBase.SimulationMode Then
                    ' simulating...
                    System.Threading.Thread.Sleep(SimulationProcessTime)
                    MyBase.myServiceMDI.Focus()
                    MyClass.CurrentMode = ADJUSTMENT_MODES.ADJUSTMENTS_READED
                    PrepareArea()
                Else
                    If Not myGlobal.HasError AndAlso AnalyzerController.Instance.Analyzer.Connected Then '#REFACTORING
                        myGlobal = myScreenDelegate.SendREAD_ADJUSTMENTS(GlobalEnumerates.Ax00Adjustsments.ALL)
                    End If
                End If
            Else
                MyClass.CurrentMode = ADJUSTMENT_MODES.ADJUSTMENTS_READED
                PrepareArea()
            End If

            If myGlobal.HasError Then
                PrepareErrorMode()
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareLoadingMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareLoadingMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Prepare GUI for Loaded Mode
    ''' </summary>
    ''' <remarks>Created by XBC 15/06/2011</remarks>
    Private Sub PrepareLoadedMode()
        Dim myResultData As New GlobalDataTO
        Try
            Me.BsSaveButton.Enabled = False
            Me.BsExitButton.Enabled = True

            ' TAB ROTOR
            Me.Tab1AutoRadioButton.Enabled = True
            Me.Tab1ManualRadioButton.Enabled = True
            Me.Tab1ConditioningButton.Enabled = True
            'Me.Tab1MeasureButton.Enabled = False
            Me.Tab1AdjustButton.Enabled = False

            ' XBC 18/10/2011 - Canceled
            'Me.Tab1CorrectionTextBox.Enabled = False
            Me.Tab1UndoMeasure = True

            If myScreenDelegate.ReactionsRotorConditioningDone Then
                'Me.Tab1MeasureButton.Enabled = True

                ' XBC 18/10/2011 - Canceled
                'Me.Tab1UndoMeasure = False
            End If

            ' XBC 18/10/2011 - Changed
            Me.Tab1CorrectionTextBox.Enabled = True
            If Me.TabMeanTempLabel.Text.Length > 0 Then
                ' XBC 18/10/2011 - Canceled
                'Me.Tab1CorrectionTextBox.Enabled = True

                Me.Tab1AdjustButton.Enabled = True
            End If


            ' TAB NEEDLES
            Me.Tab2RadioButtonR1.Enabled = True
            Me.Tab2RadioButtonR2.Enabled = True
            Me.Tab2ConditioningButton.Enabled = True
            Me.Tab2AdjustButton.Enabled = False
            Me.Tab2TextBoxTemp.Enabled = False
            Me.Tab2CorrectionTextBox.Enabled = False
            'Me.Tab2AuxButton.Enabled = True 'SGM 01/12/2011

            Dim isPrimed As Boolean = myScreenDelegate.ReagentNeedleConditioningDone
            Me.Tab2MeasureButton.Enabled = isPrimed
            'Me.Tab2AdjustButton.Enabled = isPrimed
            Me.Tab2TextBoxTemp.Enabled = isPrimed
            Me.Tab2CorrectionTextBox.Enabled = isPrimed
            ' XBC 27/10/2011


            ' TAB HEATER
            Me.Tab3ConditioningButton.Enabled = True
            Me.Tab3AdjustButton.Enabled = False

            ' XBC 18/10/2011 - Canceled
            'Me.Tab3TextBoxTemp.Enabled = False
            'Me.Tab3CorrectionTextBox.Enabled = False
            'If myScreenDelegate.HeaterConditioningDone Then
            Me.Tab3TextBoxTemp.Enabled = True
            'End If
            'If Me.Tab3TextBoxTemp.Text.Length > 0 Then
            Me.Tab3CorrectionTextBox.Enabled = True
            'End If

            Me.ManageTabPages = True

            MyBase.ActivateMDIMenusButtons(True)

            'Select Case Me.SelectedPage
            '    Case THERMOS_PAGES.ROTOR
            '        Me.Tab1ConditioningButton.Focus()
            '    Case THERMOS_PAGES.NEEDLES
            '        Me.Tab2ConditioningButton.Focus()
            '    Case THERMOS_PAGES.HEATER
            '        Me.Tab3ConditioningButton.Focus()
            'End Select

            Me.ProgressBar1.Visible = False

            ' XBC 20/04/2012
            Me.BsUpDownWSButton1.Enabled = True
            Me.BsUpDownWSButton2.Enabled = True


        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareLoadedMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareLoadedMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        Finally
            Me.Cursor = Cursors.Default
        End Try
    End Sub

    ''' <summary>
    ''' Prepare GUI for Testing Mode
    ''' </summary>
    ''' <remarks>Created by XBC 20/06/2011</remarks>
    Private Sub PrepareTestingMode()
        Dim myResultData As New GlobalDataTO
        Try
            MyClass.DisableAll()

            Me.Cursor = Cursors.WaitCursor

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareTestingMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareTestingMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Manages the tempering of the washing station alowing the user to dispense 
    ''' </summary>
    ''' <remarks>Created by SGM 19/10/2011</remarks>
    Private Sub NeedlesArmsTempering()
        Try

            Me.Tab2DispensationsCountTitleLabel.Visible = True

            ' XBC 22/11/2011 - Counter placed to Delegate to register it into Historics
            'Me.Tab2DispensationsCountLabel.Text = MyClass.NeedlesDispensationsCount.ToString
            Me.Tab2DispensationsCountLabel.Text = myScreenDelegate.NeedlesDispensationsCount.ToString
            ' XBC 22/11/2011 - Counter placed to Delegate to register it into Historics

            Me.Tab2DispensationsCountLabel.Visible = True

            'If Not MyClass.NeedlesConditioningStopRequested Then
            Dim dialogResultToReturn As DialogResult
            dialogResultToReturn = MyBase.ShowMessage(GetMessageText(Messages.SRV_ADJUSTMENTS_TESTS.ToString), Messages.SRV_THERMO_NEEDLE_DISP.ToString)
            Application.DoEvents()

            If dialogResultToReturn = Windows.Forms.DialogResult.Yes Then

                'MyBase.DisplayMessage(Messages.SRV_ADJUSTMENT_IN_PROCESS.ToString)
                MyBase.DisplayMessage(Messages.SRV_CONDITIONING.ToString)
                MyClass.DisableAll()

                Application.DoEvents()

                MyBase.CurrentMode = ADJUSTMENT_MODES.TESTING
                Me.PrepareArea()

                If MyBase.SimulationMode Then

                    myScreenDelegate.ReagentNeedleDispensingDone = False

                    Me.ProgressBar1.Value = 0
                    Me.ProgressBar1.Maximum = 5
                    Me.ProgressBar1.Visible = True

                    For i As Integer = 1 To Me.ProgressBar1.Maximum
                        System.Threading.Thread.Sleep(i * MyBase.SimulationProcessTime)
                        Me.ProgressBar1.Value = i
                        Me.ProgressBar1.Refresh()
                    Next

                    myScreenDelegate.ReagentNeedleDispensingDone = True

                    ' XBC 22/11/2011 - Counter placed to Delegate to register it into Historics
                    'MyClass.NeedlesDispensationsCount += 1
                    'Me.Tab2DispensationsCountLabel.Text = MyClass.NeedlesDispensationsCount.ToString
                    myScreenDelegate.NeedlesDispensationsCount += 1
                    Me.Tab2DispensationsCountLabel.Text = myScreenDelegate.NeedlesDispensationsCount.ToString
                    ' XBC 22/11/2011 - Counter placed to Delegate to register it into Historics

                    MyBase.CurrentMode = ADJUSTMENT_MODES.TESTED
                    MyClass.PrepareArea()

                    Me.ProgressBar1.Visible = False

                Else
                    MyClass.SendFwScript(Me.CurrentMode)
                End If

            Else
                'MyClass.NeedlesConditioningStopRequested = True

                'update presentation elements
                Me.Tab2RadioButtonR1.Enabled = True
                Me.Tab2RadioButtonR2.Enabled = True
                Me.Tab2ConditioningButton.Enabled = True
                Me.Tab2MeasureButton.Enabled = True
                Me.Tab2TextBoxTemp.Enabled = True
                Me.Tab2CorrectionTextBox.Enabled = True
                Me.BsExitButton.Enabled = True
                'Me.Tab2AuxButton.Enabled = True 'SGM 01/12/2011

                Me.Tab2DispensationsCountTitleLabel.Visible = False
                Me.Tab2DispensationsCountLabel.Visible = False

                MyBase.DisplayMessage(Messages.SRV_ADJUSTMENTS_READY.ToString)

                Me.ManageTabPages = True
                MyBase.ActivateMDIMenusButtons(True)
                Me.Cursor = Cursors.Default

            End If
            'End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".NeedlesArmsTempering ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".NeedlesArmsTempering ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Prepare GUI for Tested Mode
    ''' </summary>
    ''' <remarks>
    ''' Created by XBC 17/06/2011
    ''' Modified by XB 14/10/2014 - Use NROTOR instead WSCTRL when Wash Station is down - BA-2004
    ''' </remarks>
    Private Sub PrepareTestedMode()
        Dim myGlobal As New GlobalDataTO
        'Dim Utilities As New Utilities
        Try
            Me.BsSaveButton.Enabled = False

            Select Case Me.SelectedPage

                Case THERMOS_PAGES.ROTOR
                    '
                    ' ROTOR
                    '

                    Select Case myScreenDelegate.CurrentOperation


                        ' XBC 18/10/2011 
                        Case ThermosAdjustmentsDelegate.OPERATIONS.ROTATING_ROTOR

                            If myScreenDelegate.IsKeepRotatingStopped Then

                                myScreenDelegate.ReactionsRotorRotatingDone = False

                                If Not MyClass.myScreenDelegate.RotorRotatingCancelRequested Then 'SGM 02/12/2011

                                    ' Activate screen
                                    Me.Tab1AutoRadioButton.Enabled = True
                                    Me.Tab1ManualRadioButton.Enabled = True
                                    Me.Tab1ConditioningButton.Enabled = True
                                    'Me.Tab1AdjustButton.Enabled = True
                                    Me.BsExitButton.Enabled = True
                                    Me.Tab1UndoMeasure = True
                                    Me.Tab1CorrectionTextBox.Enabled = True

                                    MyBase.DisplayMessage(Messages.SRV_ADJUSTMENTS_READY.ToString)

                                Else

                                    MyBase.CurrentMode = ADJUSTMENT_MODES.ADJUSTMENTS_READED
                                    MyClass.PrepareArea()

                                    MyBase.DisplayMessage(Messages.SRV_ROTOR_COND_CANCELLED.ToString)
                                End If

                                MyClass.myScreenDelegate.ReactionsRotorRotatingDone = False
                                MyClass.myScreenDelegate.RotorRotatingCancelRequested = False
                                MyClass.IsRotorConditioningRequested = False
                                MyClass.IsRotorConditioningCancelRequested = False

                                Me.ProgressBar1.Visible = False

                                Me.ManageTabPages = True
                                MyBase.ActivateMDIMenusButtons(True)
                                Me.Cursor = Cursors.Default


                                ' XBC 20/04/2012
                                Me.BsUpDownWSButton1.Enabled = True
                                Me.BsUpDownWSButton2.Enabled = True



                            ElseIf myScreenDelegate.ReactionsRotorRotatingDone Then

                                If Not myScreenDelegate.IsKeepRotating Then
                                    ' Activate Sound !
                                    If Not MyBase.SimulationMode Then
                                        If Not myGlobal.HasError AndAlso AnalyzerController.Instance.Analyzer.Connected Then '#REFACTORING
                                            myGlobal = myScreenDelegate.SendSOUND()
                                        End If
                                        If myGlobal.HasError Then
                                            PrepareErrorMode()
                                            Exit Try
                                        End If
                                    End If

                                    ' the rotor keeps rotating
                                    myGlobal = MyBase.Test
                                    If myGlobal.HasError Then
                                        PrepareErrorMode()
                                    Else
                                        PrepareArea()

                                        ' Initializations
                                        myScreenDelegate.IsKeepRotating = True
                                        myScreenDelegate.IsKeepRotatingStopped = False

                                        myScreenDelegate.CurrentTest = ADJUSTMENT_GROUPS.THERMOS_PHOTOMETRY
                                        myScreenDelegate.CurrentOperation = ThermosAdjustmentsDelegate.OPERATIONS.ROTATING_ROTOR

                                        MyBase.DisplayMessage(Messages.SRV_CONDITIONING_SYSTEM.ToString)

                                        ' Manage FwScripts must to be sent to send Rotating Rotor instructions to Instrument
                                        If Not MyBase.SimulationMode Then
                                            SendFwScript(Me.CurrentMode)
                                        End If
                                    End If

                                    Me.ProgressBar1.Value = Me.ProgressBar1.Maximum

                                    ' Display message to User for finish rotor movement and sound
                                    Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate
                                    MessageBox.Show(myMultiLangResourcesDelegate.GetResourceText(Nothing, "MSG_SRV_CONDITIONING_DONE", currentLanguage), GetMessageText(Messages.SRV_PHOTOMETRY_TESTS.ToString))

                                    myScreenDelegate.IsKeepRotating = False

                                    'myScreenDelegate.ReactionsRotorRotatingDone = False

                                    If Not MyBase.SimulationMode Then
                                        ' Deactivate Sound !
                                        If Not myGlobal.HasError AndAlso AnalyzerController.Instance.Analyzer.Connected Then '#REFACTORING
                                            myGlobal = myScreenDelegate.SendENDSOUND()
                                        End If
                                        If myGlobal.HasError Then
                                            PrepareErrorMode()
                                            Exit Try
                                        End If
                                    End If

                                    MyClass.myScreenDelegate.IsKeepRotatingStopped = True
                                    Me.CurrentMode = ADJUSTMENT_MODES.TESTED
                                    MyClass.PrepareArea()

                                End If

                            Else
                                Me.ProgressBar1.Visible = True
                                Me.ProgressBar1.Value = myScreenDelegate.ReactionsRotorCurrentRotation
                                Me.ProgressBar1.Refresh()

                                myGlobal = MyBase.Test
                                If myGlobal.HasError Then
                                    PrepareErrorMode()
                                Else
                                    PrepareArea()

                                    ' Initializations
                                    myScreenDelegate.CurrentTest = ADJUSTMENT_GROUPS.THERMOS_PHOTOMETRY
                                    myScreenDelegate.CurrentOperation = ThermosAdjustmentsDelegate.OPERATIONS.ROTATING_ROTOR
                                    myScreenDelegate.ReactionsRotorRotatingDone = False

                                    MyBase.DisplayMessage(Messages.SRV_CONDITIONING_SYSTEM.ToString)

                                    myScreenDelegate.ReactionsRotorCurrentRotation += 1

                                    ' Manage FwScripts must to be sent to send Conditioning instructions to Instrument
                                    SendFwScript(Me.CurrentMode)
                                End If

                            End If



                        Case ThermosAdjustmentsDelegate.OPERATIONS.CONDITIONING_HEATER, _
                             ThermosAdjustmentsDelegate.OPERATIONS.CONDITIONING_ROTOR, _
                             ThermosAdjustmentsDelegate.OPERATIONS.CONDITIONING_NEEDLE, _
                             ThermosAdjustmentsDelegate.OPERATIONS.CONDITIONING_MANUAL_UP, _
                             ThermosAdjustmentsDelegate.OPERATIONS.CONDITIONING_MANUAL_DOWN
                            'ThermosAdjustmentsDelegate.OPERATIONS.HOMES

                            Select Case myScreenDelegate.FillMode
                                Case FILL_MODE.AUTOMATIC

                                    If myScreenDelegate.ReactionsRotorConditioningDone And Not myScreenDelegate.RotorConditioningCancelRequested Then

                                        myGlobal = Me.HomesDone(myScreenDelegate.CurrentTest)
                                        If myGlobal.HasError Then
                                            PrepareErrorMode()
                                            Exit Try
                                        End If

                                        myGlobal = MyBase.Test
                                        If myGlobal.HasError Then
                                            PrepareErrorMode()
                                        Else
                                            PrepareArea()

                                            ' Initializations
                                            'Me.ProgressBar1.Maximum = myScreenDelegate.ReactionsRotorRotatingTimes
                                            myScreenDelegate.CurrentTest = ADJUSTMENT_GROUPS.THERMOS_PHOTOMETRY
                                            myScreenDelegate.CurrentOperation = ThermosAdjustmentsDelegate.OPERATIONS.ROTATING_ROTOR
                                            myScreenDelegate.ReactionsRotorCurrentRotation = 1

                                            MyBase.DisplayMessage(Messages.SRV_CONDITIONING_SYSTEM.ToString)

                                            If Not MyBase.SimulationMode Then
                                                ' Manage FwScripts must to be sent to send Rotating Rotor instructions to Instrument
                                                SendFwScript(Me.CurrentMode)
                                            End If
                                        End If

                                    End If

                                    'SGM 02/12/2011
                                    MyClass.myScreenDelegate.RotorConditioningCancelRequested = False
                                    MyClass.myScreenDelegate.RotorRotatingCancelRequested = False
                                    If MyClass.IsRotorConditioningCancelRequested Then
                                        MyClass.IsRotorConditioningRequested = False
                                        MyClass.IsRotorConditioningCancelRequested = False
                                        MyBase.CurrentMode = ADJUSTMENT_MODES.ADJUSTMENTS_READED
                                        MyClass.PrepareArea()

                                        MyBase.DisplayMessage(Messages.SRV_ROTOR_COND_CANCELLED.ToString)

                                    End If
                                    'end SGM 02/12/2011

                                Case FILL_MODE.MANUAL

                                    If myScreenDelegate.RotorConditioningCancelRequested Then

                                        MyClass.myScreenDelegate.RotorConditioningCancelRequested = False
                                        If MyClass.IsRotorConditioningCancelRequested Then
                                            MyClass.IsRotorConditioningRequested = False
                                            MyClass.IsRotorConditioningCancelRequested = False
                                        End If
                                        myScreenDelegate.ReactionsRotorConditioningManualFirstStepDone = False
                                        myScreenDelegate.ReactionsRotorConditioningManualSecondStepDone = False

                                        MyClass.IsRotorConditioningRequested = False

                                        MyBase.CurrentMode = ADJUSTMENT_MODES.ADJUSTMENTS_READED
                                        MyClass.PrepareArea()

                                        MyBase.DisplayMessage(Messages.SRV_ROTOR_COND_CANCELLED.ToString)

                                    ElseIf myScreenDelegate.ReactionsRotorConditioningManualSecondStepDone Then

                                        ' Washing station is already DOWN
                                        myScreenDelegate.ReactionsRotorConditioningManualSecondStepDone = False


                                        ' Send for Rotor cotinues its movement

                                        myGlobal = MyBase.Test
                                        If myGlobal.HasError Then
                                            PrepareErrorMode()
                                        Else
                                            PrepareArea()

                                            ' Initializations
                                            'Me.ProgressBar1.Maximum = myScreenDelegate.ReactionsRotorRotatingTimes
                                            myScreenDelegate.CurrentTest = ADJUSTMENT_GROUPS.THERMOS_PHOTOMETRY
                                            myScreenDelegate.CurrentOperation = ThermosAdjustmentsDelegate.OPERATIONS.ROTATING_ROTOR
                                            myScreenDelegate.ReactionsRotorCurrentRotation = 1

                                            MyBase.DisplayMessage(Messages.SRV_CONDITIONING_SYSTEM.ToString)

                                            ' Manage FwScripts must to be sent to send Rotating Rotor instructions to Instrument
                                            If Not MyBase.SimulationMode Then
                                                SendFwScript(Me.CurrentMode)
                                            End If
                                        End If

                                    ElseIf myScreenDelegate.ReactionsRotorConditioningManualFirstStepDone Then

                                        ' Washing station is already UP
                                        myScreenDelegate.ReactionsRotorConditioningManualFirstStepDone = False

                                        ' Inform message for instruction Washing Station DOWN
                                        Dim dialogResultToReturn As DialogResult
                                        dialogResultToReturn = MyBase.ShowMessage(GetMessageText(Messages.SRV_ADJUSTMENTS_TESTS.ToString), Messages.SRV_ROTOR_FILLED.ToString)

                                        If dialogResultToReturn = Windows.Forms.DialogResult.Cancel Then
                                            MyClass.IsRotorConditioningCancelRequested = True
                                            MyClass.PrepareArea()
                                        Else
                                            Application.DoEvents()

                                            myGlobal = MyBase.Test
                                            If myGlobal.HasError Then
                                                PrepareErrorMode()
                                            Else
                                                PrepareArea()

                                                ' Initializations
                                                myScreenDelegate.CurrentOperation = ThermosAdjustmentsDelegate.OPERATIONS.CONDITIONING_MANUAL_DOWN
                                                myScreenDelegate.CurrentTest = ADJUSTMENT_GROUPS.THERMOS_PHOTOMETRY

                                                ' instruction for Washing Station DOWN
                                                If Not MyBase.SimulationMode Then
                                                    If Not myGlobal.HasError AndAlso AnalyzerController.Instance.Analyzer.Connected Then '#REFACTORING
                                                        'myScreenDelegate.SendWASH_STATION_CTRL(Ax00WashStationControlModes.DOWN)
                                                        myScreenDelegate.SendNEW_ROTOR(ThermosAdjustmentsDelegate.OPERATIONS.CONDITIONING_MANUAL_DOWN)
                                                    End If
                                                End If

                                            End If
                                        End If


                                    ElseIf myScreenDelegate.ReactionsRotorHomesDone Then

                                        ' homes are done for current adjust
                                        myScreenDelegate.ReactionsRotorHomesDone = False

                                        myGlobal = Me.HomesDone(myScreenDelegate.CurrentTest)
                                        If myGlobal.HasError Then
                                            PrepareErrorMode()
                                            Exit Try
                                        End If

                                        myGlobal = MyBase.Test
                                        If myGlobal.HasError Then
                                            PrepareErrorMode()
                                        Else
                                            PrepareArea()

                                            ' Initializations
                                            myScreenDelegate.CurrentOperation = ThermosAdjustmentsDelegate.OPERATIONS.CONDITIONING_MANUAL_UP
                                            myScreenDelegate.CurrentTest = ADJUSTMENT_GROUPS.THERMOS_PHOTOMETRY

                                            ' instruction for Washing Station UP
                                            If Not MyBase.SimulationMode Then
                                                If Not myGlobal.HasError AndAlso AnalyzerController.Instance.Analyzer.Connected Then '#REFACTORING
                                                    myScreenDelegate.SendWASH_STATION_CTRL(Ax00WashStationControlModes.UP)
                                                End If
                                            End If
                                        End If

                                    End If

                            End Select


                        Case ThermosAdjustmentsDelegate.OPERATIONS.TEST_ADJUSTMENT

                            If myScreenDelegate.ReactionsRotorTestAdjustmentDone Then
                                Me.Tab1AutoRadioButton.Enabled = True
                                Me.Tab1ManualRadioButton.Enabled = True
                                Me.Tab1ConditioningButton.Enabled = True
                                'Me.Tab1MeasureButton.Enabled = True
                                Me.Tab1UndoMeasure = True
                                Me.Tab1CorrectionTextBox.Enabled = True
                                If Tab1CorrectionTextBox.Text.Length > 0 Then
                                    Me.Tab1AdjustButton.Enabled = True
                                End If

                                'Area Buttons
                                Me.BsExitButton.Enabled = True

                                'If IsNumeric(Me.TabMeanTempLabel.Text) Then ' XBC 30/05/2012

                                ' XBC 18/11/2011 - correction singles
                                'If CSng(Me.TabMeanTempLabel.Text.Replace(".", MyClass.myDecimalSeparator.ToString)) > Me.EditedValue.TargetMaxValue Or CSng(Me.TabMeanTempLabel.Text.Replace(".", MyClass.myDecimalSeparator.ToString)) < Me.EditedValue.TargetMinValue Then

                                ' XBC 30/05/2012 - Range allowed is calculated with FwValue (T. Sensor) and AdjSetpoint instead of measured Temp with auxiliar tool
                                'If myUtilities.FormatToSingle(Me.TabMeanTempLabel.Text) > Me.EditedValue.TargetMaxValue Or myUtilities.FormatToSingle(Me.TabMeanTempLabel.Text) < Me.EditedValue.TargetMinValue Then
                                If Me.EditedValue.SetPointNewValue > Me.EditedValue.SetPointMaxValue Or Me.EditedValue.SetPointNewValue < Me.EditedValue.SetPointMinValue Then
                                    ' XBC 30/05/2012 

                                    MyBase.DisplayMessage(Messages.SRV_NO_SUCCESS_TEST.ToString)
                                    myScreenDelegate.FinalResult(0) = ThermosAdjustmentsDelegate.ResultsOperations.NOK
                                    Me.BsSaveButton.Enabled = False
                                Else
                                    MyBase.DisplayMessage(Messages.SRV_ADJUSTMENTS_READY.ToString)
                                    myScreenDelegate.FinalResult(0) = ThermosAdjustmentsDelegate.ResultsOperations.OK
                                    Me.BsSaveButton.Enabled = True
                                End If


                                ' XBC 30/05/2012
                                'Else
                                '    MyBase.DisplayMessage(Messages.SRV_ADJUSTMENTS_READY.ToString)
                                'End If

                                Me.ManageTabPages = True
                                MyBase.ActivateMDIMenusButtons(True)
                                Me.Cursor = Cursors.Default

                                ' XBC 20/04/2012
                                Me.BsUpDownWSButton1.Enabled = True
                                Me.BsUpDownWSButton2.Enabled = True

                            End If

                    End Select


                Case THERMOS_PAGES.NEEDLES
                    ' 
                    ' NEEDLES
                    '

                    Select Case myScreenDelegate.CurrentOperation

                        Case ThermosAdjustmentsDelegate.OPERATIONS.CONDITIONING_NEEDLE
                            If myScreenDelegate.ReagentNeedleConditioningDone Then 'the needles are already conditioned (primed)

                                'update screen elements
                                Me.Tab2RadioButtonR1.Enabled = True
                                Me.Tab2RadioButtonR2.Enabled = True
                                Me.Tab2ConditioningButton.Enabled = True
                                Me.Tab2MeasureButton.Enabled = True
                                Me.Tab2TextBoxTemp.Enabled = True
                                Me.Tab2CorrectionTextBox.Enabled = True
                                Me.BsExitButton.Enabled = True
                                'Me.Tab2AuxButton.Enabled = True 'SGM 01/12/2011

                                MyBase.DisplayMessage(Messages.SRV_CONDITIONED.ToString)

                                Me.ManageTabPages = True
                                MyBase.ActivateMDIMenusButtons(True)
                                Me.Cursor = Cursors.Default

                                ' XBC 20/04/2012
                                Me.BsUpDownWSButton1.Enabled = True
                                Me.BsUpDownWSButton2.Enabled = True

                                ' homes are done for current adjust just with the first well filled
                                myGlobal = Me.HomesDone(myScreenDelegate.CurrentTest)
                                If myGlobal.HasError Then
                                    PrepareErrorMode()
                                    Exit Try
                                End If

                            End If

                        Case ThermosAdjustmentsDelegate.OPERATIONS.DISPENSING_NEEDLE
                            If myScreenDelegate.ReagentNeedleDispensingDone Then

                                MyBase.DisplayMessage(Messages.SRV_CONDITIONING.ToString)

                                ' XBC 22/11/2011 - Counter placed to Delegate to register it into Historics
                                'MyClass.NeedlesDispensationsCount += 1
                                'Me.Tab2DispensationsCountLabel.Text = MyClass.NeedlesDispensationsCount.ToString
                                myScreenDelegate.NeedlesDispensationsCount += 1
                                Me.Tab2DispensationsCountLabel.Text = myScreenDelegate.NeedlesDispensationsCount.ToString
                                ' XBC 22/11/2011 - Counter placed to Delegate to register it into Historics

                                'Manages the tempering of the washing station alowing the user to dispense 
                                MyClass.NeedlesArmsTempering()

                            End If



                        Case ThermosAdjustmentsDelegate.OPERATIONS.TEST_ADJUSTMENT

                            If myScreenDelegate.ReagentNeedleTestAdjustmentDone Then

                                'update presentation elements
                                Me.Tab2RadioButtonR1.Enabled = True
                                Me.Tab2RadioButtonR2.Enabled = True
                                Me.Tab2ConditioningButton.Enabled = True
                                Me.Tab2CorrectionTextBox.Enabled = True
                                If Tab2CorrectionTextBox.Text.Length > 0 Then
                                    Me.Tab2AdjustButton.Enabled = True
                                End If
                                Me.Tab2TextBoxTemp.Enabled = True
                                Me.Tab2MeasureButton.Enabled = True

                                'Area Buttons
                                Me.BsExitButton.Enabled = True

                                'If IsNumeric(Me.Tab2TextBoxTemp.Text) Then  ' XBC 30/05/2012

                                ' XBC 18/11/2011 - correction singles
                                ' If CSng(Me.Tab2TextBoxTemp.Text.Replace(".", MyClass.myDecimalSeparator.ToString)) > Me.EditedValue.TargetMaxValue Or CSng(Me.Tab2TextBoxTemp.Text.Replace(".", MyClass.myDecimalSeparator.ToString)) < Me.EditedValue.TargetMinValue Then

                                ' XBC 30/05/2012 - Range allowed is calculated with FwValue (T. Sensor) and AdjSetpoint instead of measured Temp with auxiliar tool
                                'If myUtilities.FormatToSingle(Me.Tab2TextBoxTemp.Text) > Me.EditedValue.TargetMaxValue Or myUtilities.FormatToSingle(Me.Tab2TextBoxTemp.Text) < Me.EditedValue.TargetMinValue Then
                                If Me.EditedValue.SetPointNewValue > Me.EditedValue.SetPointMaxValue Or Me.EditedValue.SetPointNewValue < Me.EditedValue.SetPointMinValue Then
                                    ' XBC 30/05/2012 

                                    MyBase.DisplayMessage(Messages.SRV_NO_SUCCESS_TEST.ToString)
                                    myScreenDelegate.FinalResult(1) = ThermosAdjustmentsDelegate.ResultsOperations.NOK
                                    Me.BsSaveButton.Enabled = False
                                Else
                                    MyBase.DisplayMessage(Messages.SRV_ADJUSTMENTS_READY.ToString)
                                    myScreenDelegate.FinalResult(1) = ThermosAdjustmentsDelegate.ResultsOperations.OK
                                    Me.BsSaveButton.Enabled = True
                                End If

                                ' XBC 30/05/2012
                                'Else
                                '    MyBase.DisplayMessage(Messages.SRV_ADJUSTMENTS_READY.ToString)
                                'End If

                                Me.ManageTabPages = True
                                MyBase.ActivateMDIMenusButtons(True)
                                Me.Cursor = Cursors.Default

                                ' XBC 20/04/2012
                                Me.BsUpDownWSButton1.Enabled = True
                                Me.BsUpDownWSButton2.Enabled = True

                            End If

                            Me.ProgressBar1.Visible = False

                    End Select

                Case THERMOS_PAGES.HEATER
                    ' 
                    ' HEATER
                    ' 

                    Select Case myScreenDelegate.CurrentOperation

                        Case ThermosAdjustmentsDelegate.OPERATIONS.CONDITIONING_HEATER

                            If myScreenDelegate.HeaterConditioningDone Then 'ws heater already conditioned

                                'hide the progressbar
                                Me.ProgressBar1.Visible = False
                                Me.ProgressBar1.Value = 0
                                Me.ProgressBar1.Refresh()

                                'update presentation elements
                                Me.Tab3TextBoxTemp.Enabled = True
                                Me.Tab3CorrectionTextBox.Enabled = True
                                Me.Tab3ConditioningButton.Enabled = True
                                If Tab3CorrectionTextBox.Text.Length > 0 Then
                                    Me.Tab3AdjustButton.Enabled = True
                                End If

                                'Area Buttons
                                Me.BsSaveButton.Enabled = False
                                Me.BsExitButton.Enabled = True

                                Me.Tab3TextBoxTemp.Focus()

                                MyBase.DisplayMessage(Messages.SRV_ADJUSTMENTS_READY.ToString)

                                Me.ManageTabPages = True
                                MyBase.ActivateMDIMenusButtons(True)
                                Me.Cursor = Cursors.Default

                                ' XBC 20/04/2012
                                Me.BsUpDownWSButton1.Enabled = True
                                Me.BsUpDownWSButton2.Enabled = True


                            ElseIf myScreenDelegate.HeaterConditioningDoing(myScreenDelegate.HeaterConditioningNumDisp - 1) Then

                                If myScreenDelegate.HeaterConditioningNumDisp = 1 Then
                                    ' homes are done for current adjust just with the first well filled
                                    myGlobal = Me.HomesDone(myScreenDelegate.CurrentTest)
                                    If myGlobal.HasError Then
                                        PrepareErrorMode()
                                        Exit Try
                                    End If
                                End If

                                myGlobal = MyBase.Test
                                If myGlobal.HasError Then
                                    PrepareErrorMode()
                                Else
                                    PrepareArea()

                                    Me.ProgressBar1.Visible = True
                                    Me.ProgressBar1.Value = myScreenDelegate.HeaterConditioningNumDisp
                                    Me.ProgressBar1.Refresh()

                                    myScreenDelegate.HeaterConditioningNumDisp += 1

                                    MyBase.DisplayMessage(Messages.SRV_CONDITIONING_SYSTEM.ToString)

                                    ' Manage FwScripts must to be sent to send Conditioning instructions to Instrument
                                    SendFwScript(Me.CurrentMode)
                                End If
                            End If



                        Case ThermosAdjustmentsDelegate.OPERATIONS.TEST_ADJUSTMENT

                            If myScreenDelegate.HeaterTestAdjustmentDone Then

                                'update presentation elements
                                Me.Tab3ConditioningButton.Enabled = True
                                Me.Tab3TextBoxTemp.Enabled = True
                                Me.Tab3CorrectionTextBox.Enabled = True
                                If Tab3CorrectionTextBox.Text.Length > 0 Then
                                    Me.Tab3AdjustButton.Enabled = True
                                End If

                                'Area Buttons
                                Me.BsExitButton.Enabled = True

                                'If IsNumeric(Me.Tab3TextBoxTemp.Text) Then ' XBC 30/05/2012

                                ' XBC 18/11/2011 - correction singles
                                'If CSng(Me.Tab3TextBoxTemp.Text.Replace(".", MyClass.myDecimalSeparator.ToString)) > Me.EditedValue.TargetMaxValue Or CSng(Me.Tab3TextBoxTemp.Text.Replace(".", MyClass.myDecimalSeparator.ToString)) < Me.EditedValue.TargetMinValue Then

                                ' XBC 30/05/2012 - Range allowed is calculated with FwValue (T. Sensor) and AdjSetpoint instead of measured Temp with auxiliar tool
                                'If myUtilities.FormatToSingle(Me.Tab3TextBoxTemp.Text) > Me.EditedValue.TargetMaxValue Or myUtilities.FormatToSingle(Me.Tab3TextBoxTemp.Text) < Me.EditedValue.TargetMinValue Then
                                If Me.EditedValue.SetPointNewValue > Me.EditedValue.SetPointMaxValue Or Me.EditedValue.SetPointNewValue < Me.EditedValue.SetPointMinValue Then
                                    ' XBC 30/05/2012 

                                    MyBase.DisplayMessage(Messages.SRV_NO_SUCCESS_TEST.ToString)
                                    myScreenDelegate.FinalResult(2) = ThermosAdjustmentsDelegate.ResultsOperations.NOK
                                    Me.BsSaveButton.Enabled = False
                                Else
                                    MyBase.DisplayMessage(Messages.SRV_ADJUSTMENTS_READY.ToString)
                                    myScreenDelegate.FinalResult(2) = ThermosAdjustmentsDelegate.ResultsOperations.OK
                                    Me.BsSaveButton.Enabled = True
                                End If


                                ' XBC 30/05/2012
                                'Else
                                '    MyBase.DisplayMessage(Messages.SRV_ADJUSTMENTS_READY.ToString)
                                'End If

                                Me.ManageTabPages = True
                                MyBase.ActivateMDIMenusButtons(True)
                                Me.Cursor = Cursors.Default

                                ' XBC 20/04/2012
                                Me.BsUpDownWSButton1.Enabled = True
                                Me.BsUpDownWSButton2.Enabled = True

                            End If

                    End Select



            End Select



        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareTestedMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareTestedMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
            'Finally
            '    Me.Cursor = Cursors.Default
        End Try
    End Sub

    ''' <summary>
    ''' Prepare GUI for Saving Mode
    ''' </summary>
    ''' <remarks>Created by XBC 27/06/2011</remarks>
    Private Sub PrepareSavingMode()
        Dim myResultData As New GlobalDataTO
        Try
            DisableAll()
            Me.Cursor = Cursors.WaitCursor

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareSavingMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareSavingMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Prepare GUI for Saved Mode
    ''' </summary>
    ''' <remarks>Created by XBC 05/07/2011</remarks>
    Private Sub PrepareSavedMode()
        Dim myGlobal As New GlobalDataTO
        Try
            myGlobal = MyBase.UpdateAdjustments(MyClass.SelectedAdjustmentsDS)

            If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                LoadAdjustmentGroupData()
                PopulateEditionValues()
                PrepareLoadedMode()
            Else
                PrepareErrorMode()
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareSavedMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareSavedMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        Finally
            Me.Cursor = Cursors.Default
        End Try
    End Sub

    ''' <summary>
    ''' Prepare GUI for Error Mode
    ''' </summary>
    ''' <remarks>Created by XBC 15/06/2011</remarks>
    Public Overrides Sub PrepareErrorMode(Optional ByVal pAlarmType As ManagementAlarmTypes = ManagementAlarmTypes.NONE)
        Try
            MyBase.ErrorMode()
            Me.EditingRotorTemperatures = False
            Me.ProgressBar1.Visible = False
            DisableAll()
            Me.Tab1ConditioningButton.Enabled = False
            Me.BsExitButton.Enabled = True ' Just Exit button is enabled in error case

            ' XBC 23/10/2012
            'MyBase.myServiceMDI.AdjustmentsReaded = False
            MyBase.ActivateMDIMenusButtons(True)
            ' XBC 23/10/2012

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareErrorMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareErrorMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        Finally
            Me.Cursor = Cursors.Default
        End Try
    End Sub

    ''' <summary>
    ''' Prepare GUI for Parking Mode
    ''' </summary>
    ''' <remarks>Created by XBC 20/04/2012</remarks>
    Private Sub PrepareParkingMode()
        Try
            Me.Cursor = Cursors.WaitCursor

            DisableAll()
            Me.Tab1ConditioningButton.Enabled = False

            ManageTabPages = False

            MyBase.ActivateMDIMenusButtons(False)

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareParkingMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareParkingMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Prepare GUI for Parked Mode
    ''' </summary>
    ''' <remarks>Created by XBC 20/04/2012</remarks>
    Private Sub PrepareParkedMode()
        Try
            Me.Cursor = Cursors.Default

            If myScreenDelegate.IsWashingStationUp Then
                Me.BsUpDownWSButton1.Enabled = True
                Me.BsUpDownWSButton2.Enabled = True
            Else
                Me.PrepareLoadedMode()
            End If

            MyBase.DisplayMessage("")

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareParkedMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareParkedMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub EnableTemperatureInputTextBox(ByVal pWell As Integer)
        Try
            Select Case pWell
                Case 1
                    Me.Tab1TextBoxTemp1.Enabled = True
                    'Me.Tab1TextBoxTemp1.Text = ""
                    Me.Tab1TextBoxTemp1.Focus()
                Case 2
                    Me.Tab1TextBoxTemp2.Enabled = True
                    'Me.Tab1TextBoxTemp2.Text = ""
                    Me.Tab1TextBoxTemp2.Focus()
                Case 3
                    Me.Tab1TextBoxTemp3.Enabled = True
                    'Me.Tab1TextBoxTemp3.Text = ""
                    Me.Tab1TextBoxTemp3.Focus()
                Case 4
                    Me.Tab1TextBoxTemp4.Enabled = True
                    'Me.Tab1TextBoxTemp4.Text = ""
                    Me.Tab1TextBoxTemp4.Focus()

                    ' XBC 18/10/2011 - Canceled
                    'Case 5
                    '    Me.Tab1TextBoxTemp5.Enabled = True
                    '    Me.Tab1TextBoxTemp5.Text = ""
                    '    Me.Tab1TextBoxTemp5.Focus()
                    'Case 6
                    '    Me.Tab1TextBoxTemp6.Enabled = True
                    '    Me.Tab1TextBoxTemp6.Text = ""
                    '    Me.Tab1TextBoxTemp6.Focus()
                    'Case 7
                    '    Me.Tab1TextBoxTemp7.Enabled = True
                    '    Me.Tab1TextBoxTemp7.Text = ""
                    '    Me.Tab1TextBoxTemp7.Focus()
                    'Case 8
                    '    Me.Tab1TextBoxTemp8.Enabled = True
                    '    Me.Tab1TextBoxTemp8.Text = ""
                    '    Me.Tab1TextBoxTemp8.Focus()
            End Select

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".EnableTemperatureInputTextBox ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".EnableTemperatureInputTextBox ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ' XBC 18/10/2011 - Canceled
    'Private Sub NextEditionTemp()
    '    Dim myGlobal As New GlobalDataTO
    '    Dim mywell As Integer
    '    Try
    '        If Me.EditionMode Then
    '            Dim dialogResultToReturn As DialogResult = Windows.Forms.DialogResult.Yes
    '            dialogResultToReturn = mybase.ShowMessage(GetMessageText(GlobalEnumerates.Messages.SRV_ADJUSTMENTS_TESTS.ToString), Messages.SRV_WARNING_THERMO_MOVS.ToString)
    '            If dialogResultToReturn <> Windows.Forms.DialogResult.Yes Then
    '                CancelWizardEditionTemps()
    '            Else
    '                mywell = myScreenDelegate.NextReactionsRotorWellToEdit

    '                If mywell > 0 Then

    '                    myGlobal = MyBase.Test
    '                    If myGlobal.HasError Then
    '                        PrepareErrorMode()
    '                    Else
    '                        PrepareArea()

    '                        ' Initializations
    '                        myScreenDelegate.CurrentOperation = ThermosAdjustmentsDelegate.OPERATIONS.MEASURE_TEMPERATURE
    '                        myScreenDelegate.ReactionsRotorMeasuredTempDone = False
    '                        myScreenDelegate.ReactionsRotorCurrentWell = mywell
    '                        myScreenDelegate.CurrentTest = ADJUSTMENT_GROUPS.THERMOS_PHOTOMETRY

    '                        MyBase.DisplayMessage(Messages.SRV_ADJUSTMENT_IN_PROCESS.ToString)

    '                        If Not myGlobal.HasError Then
    '                            If MyBase.SimulationMode Then
    '                                ' simulating
    '                                'MyBase.DisplaySimulationMessage("Measuring Temperature into Reactions Rotor...")
    '                                'Me.Cursor = Cursors.WaitCursor
    '                                System.Threading.Thread.Sleep(SimulationProcessTime)
    '                                MyBase.myServiceMDI.Focus()
    '                                'Me.Cursor = Cursors.Default
    '                                MyBase.CurrentMode = ADJUSTMENT_MODES.TESTED
    '                                myScreenDelegate.ReactionsRotorPrepareWellDone(mywell - 1) = True
    '                                If myScreenDelegate.AreReactionsRotorAllPrepared Then
    '                                    myScreenDelegate.ReactionsRotorMeasuredTempDone = True
    '                                End If
    '                                PrepareArea()
    '                            Else
    '                                ' Manage FwScripts must to be sent to send Conditioning instructions to Instrument
    '                                SendFwScript(Me.CurrentMode)
    '                            End If
    '                        End If
    '                    End If

    '                End If

    '            End If

    '        End If

    '    Catch ex As Exception
    '        GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".NextEditionTemp ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
    '        mybase.ShowMessage(Me.Name & ".NextEditionTemp ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
    '    End Try
    'End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>
    ''' Modified by: XBC 28/01/2013 - change CSng function by Utilities.FormatToSingle method (Bugs tracking #1122)
    '''               IT 21/07/2015 - BA-2649
    ''' </remarks>
    Private Sub ValidateToCalulatePHOTOMETRYMeanTemps()
        Dim myResultData As New GlobalDataTO
        Try
            'Dim Utilities As New Utilities
            Dim AllTempsFilled As Boolean = True

            ' 1st validation

            ' XBC 29/11/2011
            'If Me.Tab1TextBoxTemp1.Text.Length = 0 Then
            '    AllTempsFilled = False
            'ElseIf Me.Tab1TextBoxTemp2.Text.Length = 0 Then
            '    AllTempsFilled = False
            'ElseIf Me.Tab1TextBoxTemp3.Text.Length = 0 Then
            '    AllTempsFilled = False
            'ElseIf Me.Tab1TextBoxTemp4.Text.Length = 0 Then
            '    AllTempsFilled = False

            ' XBC 18/10/2011 - Canceled
            'ElseIf Me.Tab1TextBoxTemp5.Text.Length = 0 Then
            '    AllTempsFilled = False
            'ElseIf Me.Tab1TextBoxTemp6.Text.Length = 0 Then
            '    AllTempsFilled = False
            'ElseIf Me.Tab1TextBoxTemp7.Text.Length = 0 Then
            '    AllTempsFilled = False
            'ElseIf Me.Tab1TextBoxTemp8.Text.Length = 0 Then
            '    AllTempsFilled = False

            'End If

            'If Me.Tab1TextBoxTemp1.Text.Length = 0 Then
            '    AllTempsFilled = False
            'Else
            '    If IsNumeric(Me.Tab1TextBoxTemp1.Text) Then
            '        If CDbl(Me.Tab1TextBoxTemp1.Text) = 0 Then
            '            AllTempsFilled = False
            '        End If
            '    End If
            'End If

            'If Me.Tab1TextBoxTemp2.Text.Length = 0 Then
            '    AllTempsFilled = False
            'Else
            '    If IsNumeric(Me.Tab1TextBoxTemp2.Text) Then
            '        If CDbl(Me.Tab1TextBoxTemp2.Text) = 0 Then
            '            AllTempsFilled = False
            '        End If
            '    End If
            'End If

            'If Me.Tab1TextBoxTemp3.Text.Length = 0 Then
            '    AllTempsFilled = False
            'Else
            '    If IsNumeric(Me.Tab1TextBoxTemp3.Text) Then
            '        If CDbl(Me.Tab1TextBoxTemp3.Text) = 0 Then
            '            AllTempsFilled = False
            '        End If
            '    End If
            'End If

            'If Me.Tab1TextBoxTemp4.Text.Length = 0 Then
            '    AllTempsFilled = False
            'Else
            '    If IsNumeric(Me.Tab1TextBoxTemp4.Text) Then
            '        If CDbl(Me.Tab1TextBoxTemp4.Text) = 0 Then
            '            AllTempsFilled = False
            '        End If
            '    End If
            'End If
            If (Me.Tab1TextBoxTemp1.Visible) AndAlso (Me.Tab1TextBoxTemp1.Text.Length = 0) Then 'BA-2649
                AllTempsFilled = False
            Else
                If IsNumeric(Me.Tab1TextBoxTemp1.Text) Then
                    If Utilities.FormatToSingle(Me.Tab1TextBoxTemp1.Text) = 0 Then
                        AllTempsFilled = False
                    End If
                End If
            End If

            If (Me.Tab1TextBoxTemp2.Visible) AndAlso (Me.Tab1TextBoxTemp2.Text.Length = 0) Then 'BA-2649
                AllTempsFilled = False
            Else
                If IsNumeric(Me.Tab1TextBoxTemp2.Text) Then
                    If Utilities.FormatToSingle(Me.Tab1TextBoxTemp2.Text) = 0 Then
                        AllTempsFilled = False
                    End If
                End If
            End If

            If (Me.Tab1TextBoxTemp3.Visible) AndAlso (Me.Tab1TextBoxTemp3.Text.Length = 0) Then 'BA-2649
                AllTempsFilled = False
            Else
                If IsNumeric(Me.Tab1TextBoxTemp3.Text) Then
                    If Utilities.FormatToSingle(Me.Tab1TextBoxTemp3.Text) = 0 Then
                        AllTempsFilled = False
                    End If
                End If
            End If

            If (Me.Tab1TextBoxTemp4.Visible) AndAlso (Me.Tab1TextBoxTemp4.Text.Length = 0) Then 'BA-2649
                AllTempsFilled = False
            Else
                If IsNumeric(Me.Tab1TextBoxTemp4.Text) Then
                    If Utilities.FormatToSingle(Me.Tab1TextBoxTemp4.Text) = 0 Then
                        AllTempsFilled = False
                    End If
                End If
            End If
            ' XBC 29/11/2011

            If AllTempsFilled Then
                ' 2nd validation
                'If CSng(Me.Tab1TextBoxTemp1.Text) = 0 Then
                '    AllTempsFilled = False
                'ElseIf CSng(Me.Tab1TextBoxTemp2.Text) = 0 Then
                '    AllTempsFilled = False
                'ElseIf CSng(Me.Tab1TextBoxTemp3.Text) = 0 Then
                '    AllTempsFilled = False
                'ElseIf CSng(Me.Tab1TextBoxTemp4.Text) = 0 Then
                '    AllTempsFilled = False
                If (Me.Tab1TextBoxTemp1.Visible) AndAlso (Utilities.FormatToSingle(Me.Tab1TextBoxTemp1.Text) = 0) Then 'BA-2649
                    AllTempsFilled = False
                ElseIf (Me.Tab1TextBoxTemp2.Visible) AndAlso (Utilities.FormatToSingle(Me.Tab1TextBoxTemp2.Text) = 0) Then 'BA-2649
                    AllTempsFilled = False
                ElseIf (Me.Tab1TextBoxTemp3.Visible) AndAlso (Utilities.FormatToSingle(Me.Tab1TextBoxTemp3.Text) = 0) Then 'BA-2649
                    AllTempsFilled = False
                ElseIf (Me.Tab1TextBoxTemp4.Visible) AndAlso (Utilities.FormatToSingle(Me.Tab1TextBoxTemp4.Text) = 0) Then 'BA-2649
                    AllTempsFilled = False


                    ' XBC 18/10/2011 - Canceled
                    'ElseIf CSng(Me.Tab1TextBoxTemp5.Text) = 0 Then
                    '    AllTempsFilled = False
                    'ElseIf CSng(Me.Tab1TextBoxTemp6.Text) = 0 Then
                    '    AllTempsFilled = False
                    'ElseIf CSng(Me.Tab1TextBoxTemp7.Text) = 0 Then
                    '    AllTempsFilled = False
                    'ElseIf CSng(Me.Tab1TextBoxTemp8.Text) = 0 Then
                    '    AllTempsFilled = False
                End If

            End If

            If AllTempsFilled Then
                ' calculate Mean Temperatures
                Dim Mean As Single
                Dim NewCorrection As Single
                Dim ProposalCorrection As Single
                Mean = myScreenDelegate.ProbeWellsTemps.Average()
                ' XBC 18/11/2011 - correction singles
                'Me.TabMeanTempLabel.Text = Mean.ToString("0.0").Replace(",", MyClass.myDecimalSeparator.ToString)
                Me.TabMeanTempLabel.Text = Mean.ToString("0.0")

                ' calculate new Setpoint and proposal correction
                With Me.EditedValue
                    myResultData = myScreenDelegate.CalculateThermoSetPoint(.SetPointCurrentValue, .TargetValue, Mean)
                    .SetPointNewValue = CType(myResultData.SetDatos, Single)

                    If .SetPointNewValue >= .SetPointMinValue And .SetPointNewValue <= .SetPointMaxValue Then
                        ' Result temperature inside allowed limits
                        ' Is already assigned !
                    Else
                        ' Otherwise
                        If .SetPointNewValue < .SetPointMinValue Then
                            ' take the minimum allowed value
                            .SetPointNewValue = .SetPointMinValue
                        ElseIf .SetPointNewValue > .SetPointMaxValue Then
                            ' take the maximum allowed value
                            .SetPointNewValue = .SetPointMaxValue
                        End If
                    End If

                    myResultData = myScreenDelegate.CalculateThermoCorrection(.SetPointNewValue, .TargetValue)
                    NewCorrection = CType(myResultData.SetDatos, Single)
                    ProposalCorrection = -(NewCorrection - .Correction)
                    ' XBC 18/11/2011 - correction singles
                    'Me.Tab1CorrectionTextBox.Text = ProposalCorrection.ToString("0.0").Replace(",", MyClass.myDecimalSeparator.ToString)
                    Me.Tab1CorrectionTextBox.Text = ProposalCorrection.ToString("0.0")
                    Me.Tab1CorrectionTextBox.Enabled = True
                    Me.Tab1AdjustButton.Enabled = True
                End With

            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".ValidateToCalulatePHOTOMETRYMeanTemps ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".ValidateToCalulatePHOTOMETRYMeanTemps ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub ValidateToCalulateNEEDLESTemp()
        Dim myResultData As New GlobalDataTO
        'Dim Utilities As New Utilities
        Try
            Dim NewCorrection As Single
            Dim ProposalCorrection As Single
            Dim MeasuredTemp As Single
            If IsNumeric(Me.Tab2TextBoxTemp.Text) Then
                ' XBC 18/11/2011 - correction singles
                'MeasuredTemp = CSng(Me.Tab2TextBoxTemp.Text)
                MeasuredTemp = Utilities.FormatToSingle(Me.Tab2TextBoxTemp.Text)

                myScreenDelegate.ReagentMeasuredTemp = MeasuredTemp

                ' calculate new Setpoint and proposal correction
                With Me.EditedValue
                    myResultData = myScreenDelegate.CalculateThermoSetPoint(.SetPointCurrentValue, .TargetValue, MeasuredTemp)
                    .SetPointNewValue = CType(myResultData.SetDatos, Single)

                    If .SetPointNewValue >= .SetPointMinValue And .SetPointNewValue <= .SetPointMaxValue Then
                        ' Result temperature inside allowed limits
                        ' Is already assigned !
                    Else
                        ' Otherwise
                        If .SetPointNewValue < .SetPointMinValue Then
                            ' take the minimum allowed value
                            .SetPointNewValue = .SetPointMinValue
                        ElseIf .SetPointNewValue > .SetPointMaxValue Then
                            ' take the maximum allowed value
                            .SetPointNewValue = .SetPointMaxValue
                        End If
                    End If

                    myResultData = myScreenDelegate.CalculateThermoCorrection(.SetPointNewValue, .TargetValue)
                    NewCorrection = CType(myResultData.SetDatos, Single)
                    ProposalCorrection = -(NewCorrection - .Correction)
                    ' XBC 18/11/2011 - correction singles
                    'Me.Tab2CorrectionTextBox.Text = ProposalCorrection.ToString("0.0").Replace(",", MyClass.myDecimalSeparator.ToString)
                    Me.Tab2CorrectionTextBox.Text = ProposalCorrection.ToString("0.0")

                    Me.Tab2CorrectionTextBox.Enabled = True
                    Me.Tab2AdjustButton.Enabled = True

                    myScreenDelegate.CurrentOperation = ThermosAdjustmentsDelegate.OPERATIONS.MEASURE_TEMPERATURE

                End With
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".ValidateToCalulateNEEDLESTemp ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".ValidateToCalulateNEEDLESTemp ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub ValidateToCalulateHEATERTemp()
        Dim myResultData As New GlobalDataTO
        'Dim Utilities As New Utilities
        Try
            Dim NewCorrection As Single
            Dim ProposalCorrection As Single
            Dim MeasuredTemp As Single
            If IsNumeric(Me.Tab3TextBoxTemp.Text) Then
                ' XBC 18/11/2011 - correction singles
                'MeasuredTemp = CSng(Me.Tab3TextBoxTemp.Text)
                MeasuredTemp = Utilities.FormatToSingle(Me.Tab3TextBoxTemp.Text)

                myScreenDelegate.HeaterMeasuredTemp = MeasuredTemp

                ' calculate new Setpoint and proposal correction
                With Me.EditedValue
                    myResultData = myScreenDelegate.CalculateThermoSetPoint(.SetPointCurrentValue, .TargetValue, MeasuredTemp)
                    .SetPointNewValue = CType(myResultData.SetDatos, Single)

                    If .SetPointNewValue >= .SetPointMinValue And .SetPointNewValue <= .SetPointMaxValue Then
                        ' Result temperature inside allowed limits
                        ' Is already assigned !
                    Else
                        ' Otherwise
                        If .SetPointNewValue < .SetPointMinValue Then
                            ' take the minimum allowed value
                            .SetPointNewValue = .SetPointMinValue
                        ElseIf .SetPointNewValue > .SetPointMaxValue Then
                            ' take the maximum allowed value
                            .SetPointNewValue = .SetPointMaxValue
                        End If
                    End If

                    myResultData = myScreenDelegate.CalculateThermoCorrection(.SetPointNewValue, .TargetValue)
                    NewCorrection = CType(myResultData.SetDatos, Single)
                    ProposalCorrection = -(NewCorrection - .Correction)
                    ' XBC 18/11/2011 - correction singles
                    'Me.Tab3CorrectionTextBox.Text = ProposalCorrection.ToString("0.0").Replace(",", MyClass.myDecimalSeparator.ToString)
                    Me.Tab3CorrectionTextBox.Text = ProposalCorrection.ToString("0.0")

                    Me.Tab3CorrectionTextBox.Enabled = True
                    Me.Tab3AdjustButton.Enabled = True
                End With
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".ValidateToCalulateHEATERTemp ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".ValidateToCalulateHEATERTemp ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ' XBC 18/10/2011 - Canceled
    'Private Sub CancelWizardEditionTemps()
    '    Try
    '        Me.EditionMode = False
    '        Me.EditingRotorTemperatures = False
    '        Me.InitializeRotorReationsTemps()
    '        MyBase.CurrentMode = ADJUSTMENT_MODES.LOADED
    '        PrepareArea()
    '        MyBase.DisplayMessage("")

    '    Catch ex As Exception
    '        GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".CancelWizardEditionTemps ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
    '        mybase.ShowMessage(Me.Name & ".CancelWizardEditionTemps ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
    '    End Try
    'End Sub

    Private Sub ExitScreen()
        Try
            Me.GenerateReportsOutput()
            Me.Close()
            ' Pending on future requirements

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".ExitScreen ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".ExitScreen ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub PopulateEditionValues()
        'Dim Utilities As New Utilities
        Try
            Dim value As String
            With Me.EditedValue
                value = ReadSpecificAdjustmentData(GlobalEnumerates.AXIS.SETPOINT).Value
                If value.Length > 0 And MyClass.myDecimalSeparator IsNot Nothing Then
                    ' XBC 18/11/2011 - correction singles
                    'value = value.Replace(",", MyClass.myDecimalSeparator.ToString)
                    '.SetPointCurrentValue = CSng(value)
                    .SetPointCurrentValue = Utilities.FormatToSingle(value)
                Else
                    .SetPointCurrentValue = 0
                End If
                .SetPointNewValue = .SetPointCurrentValue
                value = ReadSpecificAdjustmentData(GlobalEnumerates.AXIS.TARGET).Value
                If value.Length > 0 And MyClass.myDecimalSeparator IsNot Nothing Then
                    ' XBC 18/11/2011 - correction singles
                    'value = value.Replace(",", MyClass.myDecimalSeparator.ToString)
                    '.TargetValue = CSng(value)
                    .TargetValue = Utilities.FormatToSingle(value)
                Else
                    .TargetValue = 0
                End If
                .Correction = .TargetValue - .SetPointCurrentValue
            End With

            GetLimitValues()

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".PopulateEditionValues ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PopulateEditionValues ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub LoadAdjustmentsData()
        Try
            With myScreenDelegate
                'IT 22/07/2015 - BA-2650 (INI)
                If AnalyzerController.Instance.IsBA200 Then
                    .ArmReagent1DispPolar = ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.SAMPLES_ARM_DISP1.ToString, GlobalEnumerates.AXIS.POLAR).Value
                    .ArmReagent1DispZ = ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.SAMPLES_ARM_DISP1.ToString, GlobalEnumerates.AXIS.Z).Value
                    .ArmReagent1WSPolar = ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.SAMPLES_ARM_WASH.ToString, GlobalEnumerates.AXIS.POLAR).Value
                    .ArmReagent1Ring1Polar = ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.SAMPLES_ARM_RING1.ToString, GlobalEnumerates.AXIS.POLAR).Value
                    .ArmReagent1Ring1Z = ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.SAMPLES_ARM_RING1.ToString, GlobalEnumerates.AXIS.Z).Value
                Else
                    .ArmReagent1DispPolar = ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.REAGENT1_ARM_DISP1.ToString, GlobalEnumerates.AXIS.POLAR).Value
                    .ArmReagent1DispZ = ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.REAGENT1_ARM_DISP1.ToString, GlobalEnumerates.AXIS.Z).Value
                    .ArmReagent1WSPolar = ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.REAGENT1_ARM_WASH.ToString, GlobalEnumerates.AXIS.POLAR).Value
                    .ArmReagent1Ring1Polar = ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.REAGENT1_ARM_RING1.ToString, GlobalEnumerates.AXIS.POLAR).Value
                    .ArmReagent1Ring1Z = ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.REAGENT1_ARM_RING1.ToString, GlobalEnumerates.AXIS.Z).Value

                    .ArmReagent2DispPolar = ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.REAGENT2_ARM_DISP1.ToString, GlobalEnumerates.AXIS.POLAR).Value
                    .ArmReagent2DispZ = ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.REAGENT2_ARM_DISP1.ToString, GlobalEnumerates.AXIS.Z).Value
                    .ArmReagent2WSPolar = ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.REAGENT2_ARM_WASH.ToString, GlobalEnumerates.AXIS.POLAR).Value
                    .ArmReagent2Ring1Polar = ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.REAGENT2_ARM_RING1.ToString, GlobalEnumerates.AXIS.POLAR).Value
                    .ArmReagent2Ring1Z = ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.REAGENT2_ARM_RING1.ToString, GlobalEnumerates.AXIS.Z).Value
                End If
                'IT 22/07/2015 - BA-2650 (END)
            End With

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".LoadAdjustmentsData ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".LoadAdjustmentsData ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Function HomesDone(ByVal pAdjustmentGroup As ADJUSTMENT_GROUPS) As GlobalDataTO
        Dim myGlobal As New GlobalDataTO
        Try

            myGlobal = myScreenDelegate.SetPreliminaryHomesAsDone(pAdjustmentGroup)

        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myGlobal.ErrorMessage = ex.Message
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".HomesDone ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
        End Try
        Return myGlobal
    End Function

    ''' <summary>
    ''' Function in charge on register any activity on database
    ''' </summary>
    ''' <remarks>Created by XBC 30/08/2011</remarks>
    Private Function GenerateReportsOutput() As GlobalDataTO
        Dim myGlobal As New GlobalDataTO
        Try

            With Me.EditedValue
                Select Case .AdjustmentID
                    Case ADJUSTMENT_GROUPS.THERMOS_PHOTOMETRY
                        If myScreenDelegate.ConditioningExecuted(0) = ThermosAdjustmentsDelegate.ResultsOperations.OK Or _
                           Me.TabMeanTempLabel.Text.Length > 0 Then
                            ' Or myScreenDelegate.MeasuringTempExecuted(0) = ThermosAdjustmentsDelegate.ResultsOperations.OK Then
                            ' Insert the new activity into Historic reports
                            If myScreenDelegate.AssignedNewValues(0) = ThermosAdjustmentsDelegate.ResultsOperations.OK Then
                                myGlobal = myScreenDelegate.InsertReport("ADJUST", "GLF_TER")
                            Else
                                myGlobal = myScreenDelegate.InsertReport("TEST", "GLF_TER")
                            End If
                            myScreenDelegate.Initializations()
                        End If

                    Case ADJUSTMENT_GROUPS.THERMOS_REAGENT1
                        If myScreenDelegate.ConditioningExecuted(1) = ThermosAdjustmentsDelegate.ResultsOperations.OK Or _
                           myScreenDelegate.MeasuringTempExecuted(1) = ThermosAdjustmentsDelegate.ResultsOperations.OK Then
                            ' Insert the new activity into Historic reports
                            If myScreenDelegate.AssignedNewValues(1) = ThermosAdjustmentsDelegate.ResultsOperations.OK Then
                                myGlobal = myScreenDelegate.InsertReport("ADJUST", "REG1_TER")
                            Else
                                myGlobal = myScreenDelegate.InsertReport("TEST", "REG1_TER")
                            End If
                            myScreenDelegate.Initializations()
                        End If

                    Case ADJUSTMENT_GROUPS.THERMOS_REAGENT2
                        If myScreenDelegate.ConditioningExecuted(1) = ThermosAdjustmentsDelegate.ResultsOperations.OK Or _
                           myScreenDelegate.MeasuringTempExecuted(1) = ThermosAdjustmentsDelegate.ResultsOperations.OK Then
                            ' Insert the new activity into Historic reports
                            If myScreenDelegate.AssignedNewValues(1) = ThermosAdjustmentsDelegate.ResultsOperations.OK Then
                                myGlobal = myScreenDelegate.InsertReport("ADJUST", "REG2_TER")
                            Else
                                myGlobal = myScreenDelegate.InsertReport("TEST", "REG2_TER")
                            End If
                            myScreenDelegate.Initializations()
                        End If

                    Case ADJUSTMENT_GROUPS.THERMOS_WS_HEATER
                        If myScreenDelegate.ConditioningExecuted(2) = ThermosAdjustmentsDelegate.ResultsOperations.OK Or _
                           Me.Tab3TextBoxTemp.Text.Length > 0 Then
                            ' Insert the new activity into Historic reports
                            If myScreenDelegate.AssignedNewValues(2) = ThermosAdjustmentsDelegate.ResultsOperations.OK Then
                                myGlobal = myScreenDelegate.InsertReport("ADJUST", "HEAT_TER")
                            Else
                                myGlobal = myScreenDelegate.InsertReport("TEST", "HEAT_TER")
                            End If
                            myScreenDelegate.Initializations()
                        End If

                End Select
            End With

        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myGlobal.ErrorMessage = ex.Message
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".GenerateReportsOutput ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
        End Try
        Return myGlobal
    End Function

    ''' <summary>
    ''' Load Adjustments High Level Instruction to move Washing Station
    ''' </summary>
    ''' <remarks>
    ''' Created by XBC 20/04/2012
    ''' Modified by XB 14/10/2014 - Use NROTOR instead WSCTRL when Wash Station is down - BA-2004
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
                        'MyBase.DisplaySimulationMessage("Doing Specified Test...")
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
                            'myScreenDelegate.SendWASH_STATION_CTRL2(Ax00WashStationControlModes.DOWN)
                            myScreenDelegate.SendNEW_ROTOR(ThermosAdjustmentsDelegate.OPERATIONS.WASHING_STATION_DOWN)
                        Else
                            myScreenDelegate.SendWASH_STATION_CTRL2(Ax00WashStationControlModes.UP)
                        End If
                    End If
                End If

            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".SendWASH_STATION_CTRL", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".SendWASH_STATION_CTRL", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

#Region "Generic Adjusments DS Methods"
    ' XBC 22/06/2011 - PENDING TO IMPROVE : 
    ' These methods should be placed on Global/FwAdjustments to be accessible from any part of the application and not duplicate code

    ''' <summary>
    ''' Update some value in the specific adjustments ds
    ''' </summary>
    ''' <param name="pCodew"></param>
    ''' <param name="pValue"></param>
    ''' <returns></returns>
    ''' <remarks>XBC 15/06/2011</remarks>
    Private Function UpdateSpecificAdjustmentsDS(ByVal pCodew As String, ByVal pValue As String) As GlobalDataTO
        Dim myGlobal As New GlobalDataTO
        Try
            For Each SR As SRVAdjustmentsDS.srv_tfmwAdjustmentsRow In MyClass.SelectedAdjustmentsDS.srv_tfmwAdjustments.Rows
                If SR.CodeFw.Trim = pCodew.Trim Then
                    ' XBC 18/11/2011 - correction singles
                    'SR.Value = pValue
                    SR.Value = pValue.Replace(SystemInfoManager.OSDecimalSeparator, ".")
                    Exit For
                End If
            Next
        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myGlobal.ErrorMessage = ex.Message
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".UpdateSpecificAdjustmentsDS ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".UpdateSpecificAdjustmentsDS", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
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
    ''' <remarks>XBC 15/06/2011</remarks>
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
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".UpdateTemporalSpecificAdjustmentsDS ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".UpdateTemporalSpecificAdjustmentsDS", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
        Me.TemporalAdjustmentsDS.AcceptChanges()
        Return myGlobal
    End Function

    ''' <summary>
    ''' updates all a temporal dataset with changed current adjustments
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>XBC 15/06/2011</remarks>
    Private Function UpdateTemporalAdjustmentsDS() As GlobalDataTO
        Dim myGlobal As New GlobalDataTO
        Try
            With Me.EditedValue
                Select Case Me.SelectedPage
                    Case THERMOS_PAGES.ROTOR
                        UpdateTemporalSpecificAdjustmentsDS(ReadSpecificAdjustmentData(GlobalEnumerates.AXIS.SETPOINT).CodeFw, .SetPointNewValue.ToString("0.0"))

                    Case THERMOS_PAGES.NEEDLES
                        If Me.Tab2RadioButtonR1.Checked Then
                            UpdateTemporalSpecificAdjustmentsDS(ReadSpecificAdjustmentData(GlobalEnumerates.AXIS.SETPOINT).CodeFw, .SetPointNewValue.ToString("0.0"))
                        ElseIf Me.Tab2RadioButtonR2.Checked Then
                            UpdateTemporalSpecificAdjustmentsDS(ReadSpecificAdjustmentData(GlobalEnumerates.AXIS.SETPOINT).CodeFw, .SetPointNewValue.ToString("0.0"))
                        End If

                    Case THERMOS_PAGES.HEATER
                        UpdateTemporalSpecificAdjustmentsDS(ReadSpecificAdjustmentData(GlobalEnumerates.AXIS.SETPOINT).CodeFw, .SetPointNewValue.ToString("0.0"))
                End Select
            End With

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".UpdateTemporalAdjustmentsDS ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".UpdateTemporalAdjustmentsDS ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
        Return myGlobal
    End Function

    ''' <summary>
    ''' Gets the value corresponding to informed Group and Axis from global adjustments dataset
    ''' </summary>
    ''' <param name="pAxis"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' Created by XBC 15/06/2011
    ''' Modified by XB 04/02/2013 - Upper conversions redundants because the value is already in UpperCase must delete to avoid Regional Settings problems (Bugs tracking #1112)
    ''' </remarks>
    Private Function ReadGlobalAdjustmentData(ByVal pGroupID As String, ByVal pAxis As GlobalEnumerates.AXIS, Optional ByVal pNotForDisplaying As Boolean = False) As FwAdjustmentsDataTO
        Dim myGlobal As New GlobalDataTO
        Dim myAdjustmentRowData As New Global.FwAdjustmentsDataTO("")
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
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".ReadGlobalAdjustmentValue ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".ReadGlobalAdjustmentValue ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try

        If pNotForDisplaying Then
            If myAdjustmentRowData.Value = "" Then myAdjustmentRowData.Value = "0"
        End If

        Return myAdjustmentRowData
    End Function

    ''' <summary>
    ''' Gets the value corresponding to informed Axis from the selected adjustments dataset
    ''' </summary>
    ''' <param name="pAxis"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' Created by XBC 15/06/2011
    ''' Modified by XB 04/02/2013 - Upper conversions redundants because the value is already in UpperCase must delete to avoid Regional Settings problems (Bugs tracking #1112)
    ''' </remarks>
    Private Function ReadSpecificAdjustmentData(ByVal pAxis As GlobalEnumerates.AXIS) As FwAdjustmentsDataTO
        Dim myGlobal As New GlobalDataTO
        Dim myAdjustmentRowData As New Global.FwAdjustmentsDataTO("")
        Try
            Dim myAxis As String = pAxis.ToString
            If myAxis = "NONE" Then myAxis = ""

            Dim myAdjustmentRows As New List(Of SRVAdjustmentsDS.srv_tfmwAdjustmentsRow)
            myAdjustmentRows = (From a As SRVAdjustmentsDS.srv_tfmwAdjustmentsRow _
                                In MyClass.SelectedAdjustmentsDS.srv_tfmwAdjustments _
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
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".ReadSpecificAdjustmentData ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".ReadSpecificAdjustmentData ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try

        If myAdjustmentRowData.Value = "" Then myAdjustmentRowData.Value = "0"

        Return myAdjustmentRowData
    End Function

    ''' <summary>
    ''' Gets the Dataset that corresponds to the editing adjustments
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>XBC 15/06/2011</remarks>
    Private Function LoadAdjustmentGroupData() As GlobalDataTO
        Dim resultData As New GlobalDataTO
        Dim CopyOfSelectedAdjustmentsDS As SRVAdjustmentsDS = MyClass.SelectedAdjustmentsDS
        Dim myAdjustmentsGroups As New List(Of String)
        Try
            If MyClass.SelectedAdjustmentsDS IsNot Nothing Then
                MyClass.SelectedAdjustmentsDS.Clear()
            End If
            myAdjustmentsGroups.Add(Me.EditedValue.AdjustmentID.ToString)
            resultData = MyBase.myAdjustmentsDelegate.ReadAdjustmentsByGroupIDs(myAdjustmentsGroups)
            If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                MyClass.SelectedAdjustmentsDS = CType(resultData.SetDatos, SRVAdjustmentsDS)
            End If

        Catch ex As Exception
            MyClass.SelectedAdjustmentsDS = CopyOfSelectedAdjustmentsDS
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".LoadAdjustmentGroupData ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".LoadAdjustmentGroupData ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
        Return resultData
    End Function

#End Region

#End Region

#Region "Events"

#Region "Generic"

    ''' <summary>
    ''' IDisposable functionality to force the releasing of the objects which wasn't totally closed by default
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>Created by XBC 12/09/2011</remarks>
    Private Sub IThermosAdjustments_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
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
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".FormClosing ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".FormClosing ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub ThermosAdjustments2_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
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
            '        Me.CurrentUserNumericalLevel = CType(myGlobal.SetDatos, Integer)
            '    End If
            'End If
            MyBase.MyBase_Load(sender, e)

            MyBase.GetUserNumericalLevel()

            'Get the current Language from the current Application Session
            currentLanguage = GlobalBase.GetSessionInfo().ApplicationLanguage.Trim.ToString

            'Load the multilanguage texts for all Screen Labels and get Icons for graphical Buttons
            MyClass.GetScreenLabels()

            MyClass.PrepareButtons()

            'Screen delegate SGM 20/01/2012
            MyClass.myScreenDelegate = New ThermosAdjustmentsDelegate(MyBase.myServiceMDI.ActiveAnalyzer, myFwScriptDelegate)

            'Initialize homes SGM 20/09/2011
            MyClass.InitializeHomes()

            MyClass.Initializations()

            ApplyElementsVisibility(AnalyzerController.Instance.Analyzer.Model)

            MyClass.DisableAll()

            ' Check communications with Instrument
            If Not AnalyzerController.Instance.Analyzer.Connected Then '#REFACTORING
                PrepareErrorMode()
                MyBase.ActivateMDIMenusButtons(True)
            Else
                PrepareAdjustReadingMode()
            End If

            'Information
            MyClass.SelectedAdjPanel = Me.BsWSHeaterAdjustPanel
            MyBase.DisplayInformation(APPLICATION_PAGES.THERMOS_WS, Me.BsInfoWsHeaterXPSViewer)
            MyBase.DisplayInformation(APPLICATION_PAGES.THERMOS_ROTOR, Me.BsInfoRotorXPSViewer)
            MyBase.DisplayInformation(APPLICATION_PAGES.THERMOS_NEEDLES, Me.BsInfoNeedlesXPSViewer)

            If myGlobal.HasError Then
                PrepareErrorMode()
                MyBase.ShowMessage(Me.Name & ".Load", myGlobal.ErrorCode, myGlobal.ErrorMessage, Me)
            End If


            ' XBC 20/04/2012
            myScreenDelegate.IsWashingStationUp = False


            ResetBorderSRV()

            'Me.Tab1ConditioningButton.Focus()

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".Load ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".Load ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub BsTabPagesControl_Selected(ByVal sender As Object, ByVal e As System.Windows.Forms.TabControlEventArgs) Handles BsTabPagesControl.Selected
        Try

            If e.TabPage Is TabWSHeater Then
                MyClass.SelectedInfoPanel = Me.BsWSHeaterInfoPanel
                MyClass.SelectedAdjPanel = Me.BsWSHeaterAdjustPanel
                MyClass.BsInfoWsHeaterXPSViewer.RefreshPage()

            ElseIf e.TabPage Is TabReactionsRotor Then
                MyClass.SelectedInfoPanel = Me.BsReactionsRotorInfoPanel
                MyClass.SelectedAdjPanel = Me.BsReactionsRotorAdjustPanel
                MyClass.BsInfoRotorXPSViewer.RefreshPage()

            ElseIf e.TabPage Is TabReagentsNeedles Then
                MyClass.SelectedInfoPanel = Me.BsReagentsNeedlesInfoPanel
                MyClass.SelectedAdjPanel = Me.BsReagentsNeedlesAdjustPanel
                MyClass.BsInfoNeedlesXPSViewer.RefreshPage()

            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".BsTabPagesControl_Selected", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".BsTabPagesControl_Selected ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub BsTabPagesControl_Selecting(ByVal sender As Object, ByVal e As System.Windows.Forms.TabControlCancelEventArgs) Handles BsTabPagesControl.Selecting
        Try
            If Not Me.ManageTabPages Then
                e.Cancel = True
                Exit Sub
            End If

            Me.GenerateReportsOutput()

            If BsTabPagesControl.SelectedTab Is TabReactionsRotor Then
                Me.SelectedPage = THERMOS_PAGES.ROTOR
                With Me.EditedValue
                    .AdjustmentID = ADJUSTMENT_GROUPS.THERMOS_PHOTOMETRY
                End With

                'clear temperatute textboxes
                Me.PageInitialized = False
                Me.TabMeanTempLabel.Text = ""
                Me.Tab1TextBoxTemp1.Text = ""
                Me.Tab1TextBoxTemp2.Text = ""
                Me.Tab1TextBoxTemp3.Text = ""
                Me.Tab1TextBoxTemp4.Text = ""
                Me.Tab1CorrectionTextBox.Text = ""
                Me.PageInitialized = True

            ElseIf BsTabPagesControl.SelectedTab Is TabReagentsNeedles Then
                Me.SelectedPage = THERMOS_PAGES.NEEDLES
                With Me.EditedValue
                    If Me.Tab2RadioButtonR1.Checked Then
                        .AdjustmentID = ADJUSTMENT_GROUPS.THERMOS_REAGENT1
                    ElseIf Me.Tab2RadioButtonR2.Checked Then
                        .AdjustmentID = ADJUSTMENT_GROUPS.THERMOS_REAGENT2
                    End If
                End With

                'clear temperatute textboxes
                Me.PageInitialized = False
                Me.Tab2TextBoxTemp.Text = ""
                Me.Tab2CorrectionTextBox.Text = ""
                Me.PageInitialized = True

            ElseIf BsTabPagesControl.SelectedTab Is TabWSHeater Then
                Me.SelectedPage = THERMOS_PAGES.HEATER
                With Me.EditedValue
                    .AdjustmentID = ADJUSTMENT_GROUPS.THERMOS_WS_HEATER
                End With

                'clear temperatute textboxes
                Me.PageInitialized = False
                Me.Tab3TextBoxTemp.Text = ""
                Me.Tab3CorrectionTextBox.Text = ""
                Me.PageInitialized = True

            End If

            MyClass.DefineScreenLayout(MyClass.SelectedPage)

            LoadAdjustmentGroupData()
            PopulateEditionValues()
            MyBase.DisplayMessage("")

            PrepareLoadedMode()

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".BsTabControl_Selecting ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".BsTabControl_Selecting ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub BsExitButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsExitButton.Click
        Try
            If Me.EditingRotorTemperatures Then
                Exit Sub
            End If

            Me.ExitScreen()

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".BsExitButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".BsExitButton_Click ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub BsSaveButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsSaveButton.Click
        Dim myGlobal As New GlobalDataTO
        'Dim Utilities As New Utilities
        Try
            If Me.EditingRotorTemperatures Then
                Exit Sub
            End If

            myGlobal = MyBase.Save
            If myGlobal.HasError Then
                PrepareErrorMode()
            Else
                PrepareArea()
                ' Initializations

                ' XBC 18/10/2011 - Cancelled
                'Me.EditionMode = False

                With Me.EditedValue

                    Select Case .AdjustmentID
                        Case ADJUSTMENT_GROUPS.THERMOS_PHOTOMETRY
                            myScreenDelegate.CurrentTest = ADJUSTMENT_GROUPS.THERMOS_PHOTOMETRY
                            If IsNumeric(Me.Tab1CorrectionTextBox.Text) Then
                                ' XBC 18/11/2011 - correction singles
                                'myScreenDelegate.ProposalCorrection = CSng(Me.Tab1CorrectionTextBox.Text.Replace(".", MyClass.myDecimalSeparator.ToString))
                                myScreenDelegate.ProposalCorrection = Utilities.FormatToSingle(Me.Tab1CorrectionTextBox.Text)
                            End If
                        Case ADJUSTMENT_GROUPS.THERMOS_REAGENT1
                            myScreenDelegate.CurrentTest = ADJUSTMENT_GROUPS.THERMOS_REAGENT1
                            If IsNumeric(Me.Tab2CorrectionTextBox.Text) Then
                                ' XBC 18/11/2011 - correction singles
                                'myScreenDelegate.ProposalCorrection = CSng(Me.Tab2CorrectionTextBox.Text.Replace(".", MyClass.myDecimalSeparator.ToString))
                                myScreenDelegate.ProposalCorrection = Utilities.FormatToSingle(Me.Tab2CorrectionTextBox.Text)
                            End If
                        Case ADJUSTMENT_GROUPS.THERMOS_REAGENT2
                            myScreenDelegate.CurrentTest = ADJUSTMENT_GROUPS.THERMOS_REAGENT2
                            If IsNumeric(Me.Tab2CorrectionTextBox.Text) Then
                                ' XBC 18/11/2011 - correction singles
                                'myScreenDelegate.ProposalCorrection = CSng(Me.Tab2CorrectionTextBox.Text.Replace(".", MyClass.myDecimalSeparator.ToString))
                                myScreenDelegate.ProposalCorrection = Utilities.FormatToSingle(Me.Tab2CorrectionTextBox.Text)
                            End If
                        Case ADJUSTMENT_GROUPS.THERMOS_WS_HEATER
                            myScreenDelegate.CurrentTest = ADJUSTMENT_GROUPS.THERMOS_WS_HEATER
                            If IsNumeric(Me.Tab3CorrectionTextBox.Text) Then
                                ' XBC 18/11/2011 - correction singles
                                'myScreenDelegate.ProposalCorrection = CSng(Me.Tab3CorrectionTextBox.Text.Replace(".", MyClass.myDecimalSeparator.ToString))
                                myScreenDelegate.ProposalCorrection = Utilities.FormatToSingle(Me.Tab3CorrectionTextBox.Text)
                            End If
                    End Select

                    ' XBC 17/11/2011
                    .SetPointNewValue = .SetPointCurrentValue + myScreenDelegate.ProposalCorrection
                    If .SetPointNewValue >= .SetPointMinValue And .SetPointNewValue <= .SetPointMaxValue Then
                        ' Result temperature inside allowed limits
                        ' Is already assigned !
                    Else
                        ' Otherwise
                        MyBase.DisplayMessage(Messages.SRV_OUTOFRANGE.ToString)

                        If .SetPointNewValue < .SetPointMinValue Then
                            ' take the minimum allowed value
                            .SetPointNewValue = .SetPointMinValue
                        ElseIf .SetPointNewValue > .SetPointMaxValue Then
                            ' take the maximum allowed value
                            .SetPointNewValue = .SetPointMaxValue
                        End If

                        PrepareLoadedMode()
                        Exit Sub
                    End If
                    ' XBC 17/11/2011

                End With

                myScreenDelegate.CurrentOperation = ThermosAdjustmentsDelegate.OPERATIONS.SAVE_ADJUSTMENT
                myScreenDelegate.LoadAdjDone = False

                'Dim dialogResultToReturn As DialogResult = Windows.Forms.DialogResult.Yes
                'dialogResultToReturn = mybase.ShowMessage(GetMessageText(GlobalEnumerates.Messages.SRV_ADJUSTMENTS_TESTS.ToString), Messages.SRV_OPTIC_ADJUSTMENT_SAVE.ToString)

                'If dialogResultToReturn <> Windows.Forms.DialogResult.Yes Then
                '    MyBase.CurrentMode = ADJUSTMENT_MODES.TESTED
                '    PrepareArea()
                '    MyBase.DisplayMessage("")
                'Else
                UpdateSpecificAdjustmentsDS(ReadSpecificAdjustmentData(GlobalEnumerates.AXIS.SETPOINT).CodeFw, Me.EditedValue.SetPointNewValue.ToString("0.0"))

                '      Me.EditedValue.AdjustmentID.ToString, Me.EditedValue.SetPointNewValue.ToString)
                ' UpdateSpecificAdjustmentsDS(ReadSpecificAdjustmentData(GlobalEnumerates.AXIS.ROTOR).CodeFw, .NewRotorValue.ToString)

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

                        ' XBC 18/11/2011 - correction singles
                        pAdjuststr = pAdjuststr.Replace(".", ",")

                        myScreenDelegate.pValueAdjust = pAdjuststr

                        If MyBase.SimulationMode Then
                            ' simulating
                            'MyBase.DisplaySimulationMessage("Saving Values to Instrument...")
                            'Me.Cursor = Cursors.WaitCursor
                            System.Threading.Thread.Sleep(SimulationProcessTime)
                            MyBase.myServiceMDI.Focus()
                            'Me.Cursor = Cursors.Default
                            myScreenDelegate.LoadAdjDone = True
                            MyBase.CurrentMode = ADJUSTMENT_MODES.SAVED

                            With Me.EditedValue
                                Select Case .AdjustmentID
                                    Case ADJUSTMENT_GROUPS.THERMOS_PHOTOMETRY
                                        myScreenDelegate.AssignedNewValues(0) = ThermosAdjustmentsDelegate.ResultsOperations.OK
                                    Case ADJUSTMENT_GROUPS.THERMOS_REAGENT1
                                        myScreenDelegate.AssignedNewValues(1) = ThermosAdjustmentsDelegate.ResultsOperations.OK
                                    Case ADJUSTMENT_GROUPS.THERMOS_REAGENT2
                                        myScreenDelegate.AssignedNewValues(1) = ThermosAdjustmentsDelegate.ResultsOperations.OK
                                    Case ADJUSTMENT_GROUPS.THERMOS_WS_HEATER
                                        myScreenDelegate.AssignedNewValues(2) = ThermosAdjustmentsDelegate.ResultsOperations.OK
                                End Select
                            End With
                            PrepareArea()
                        Else
                            If Not myGlobal.HasError AndAlso AnalyzerController.Instance.Analyzer.Connected Then '#REFACTORING
                                myGlobal = myScreenDelegate.SendLOAD_ADJUSTMENTS()
                            End If
                        End If
                    Else
                        PrepareErrorMode()
                    End If

                End If
                'End If
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".BsSaveButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".BsSaveButton_Click ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
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
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".CurrentOperationTimer.Tick ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".CurrentOperationTimer.Tick ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Protected Overrides Function ProcessDialogKey(ByVal keyData As System.Windows.Forms.Keys) As Boolean
        Try
            Select Case keyData
                Case Keys.Escape
                    ' XBC 18/10/2011 - Cancelled
                    'If Me.EditionMode Then
                    'Me.CancelWizardEditionTemps()
                    'Else
                    If (Me.BsExitButton.Enabled) Then
                        ' When the  ESC key is pressed, the screen is closed 
                        Me.ExitScreen()
                    End If
                    'End If

                Case Keys.Tab

                    If BsTabPagesControl.SelectedTab Is TabReactionsRotor Then

                        If Me.TempBoxEditing > 0 And Me.TempBoxEditing < 4 Then
                            Me.EnableTemperatureInputTextBox(Me.TempBoxEditing + 1)
                        Else
                            Me.Tab1CorrectionTextBox.Focus()
                        End If

                    End If

                Case Keys.Enter

                    If BsTabPagesControl.SelectedTab Is TabReactionsRotor Then

                        ' XBC 18/10/2011 - Canceled
                        'If Me.EditionMode Then
                        '    ' Last temp to edit in Wizard mode Edition
                        '    If myScreenDelegate.ReactionsRotorMeasuredTempDone Then
                        '        Me.RunningTest = False
                        '        Me.EditingRotorTemperatures = False
                        '        ' Update status of the controls involved
                        '        Me.Tab1AutoRadioButton.Enabled = True
                        '        Me.Tab1ManualRadioButton.Enabled = True
                        '        Me.Tab1ConditioningButton.Enabled = True
                        '        Me.Tab1AdjustButton.Enabled = True
                        '        Me.BsSaveButton.Enabled = True
                        '        Me.Tab1CorrectionTextBox.Enabled = True
                        '        'Me.Tab1ConditioningButton.Focus()
                        '    Else
                        '        Me.NextEditionTemp()
                        '    End If
                        'End If

                        'If Me.EditingRotorCorrection Then
                        '    Me.Tab1AdjustButton.Focus()
                        'End If


                        If Me.TempBoxEditing > 0 And Me.TempBoxEditing < 4 Then
                            Me.EnableTemperatureInputTextBox(Me.TempBoxEditing + 1)
                        Else
                            If Me.Tab1AdjustButton.Enabled Then
                                Me.Tab1AdjustButton.Focus()
                            Else
                                Me.Tab1CorrectionTextBox.Focus()
                            End If
                        End If

                    ElseIf BsTabPagesControl.SelectedTab Is TabReagentsNeedles Then
                        If Me.EditingNeedleTemperature Then
                            Me.Tab2MeasureButton.Focus()
                        ElseIf Me.EditingNeedleCorrection Then
                            Me.Tab2AdjustButton.Focus()
                        End If

                    ElseIf BsTabPagesControl.SelectedTab Is TabWSHeater Then

                        Me.Tab3CorrectionTextBox.Focus()

                    End If

            End Select
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".ProcessDialogKey ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".ProcessDialogKey ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Function

#End Region

#Region "TAB ROTOR"

    Private Sub Tab1ConditioningButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Tab1ConditioningButton.Click
        Dim myGlobal As New GlobalDataTO
        Try
            If Me.EditingRotorTemperatures Then
                Exit Sub
            End If

            If MyClass.IsRotorConditioningRequested Then 'SGM 02/12/2011

                If MyClass.IsRotorConditioningCancelRequested Then
                    MyBase.DisplayMessage(Messages.SRV_WAIT_ACTION.ToString)
                Else
                    MyClass.IsRotorConditioningCancelRequested = True
                    MyBase.DisplayMessage(Messages.SRV_CANCELLING.ToString)
                End If


            Else

                myGlobal = MyBase.Test
                If myGlobal.HasError Then
                    PrepareErrorMode()
                Else
                    'PrepareArea()

                    ' Initializations

                    MyClass.IsRotorConditioningRequested = True
                    MyBase.DisplayMessage("")

                    myScreenDelegate.CurrentTest = ADJUSTMENT_GROUPS.THERMOS_PHOTOMETRY
                    Me.LastConditioningDone = LastConditioningDoneEnum.ConditioningRotor

                    For i As Integer = 0 To myScreenDelegate.ReactionsRotorRotatingTimes - 1
                        myScreenDelegate.ReactionsRotorRotationsDone(i) = False
                    Next
                    myScreenDelegate.ReactionsRotorCurrentRotation = 1

                    myScreenDelegate.IsKeepRotating = False
                    myScreenDelegate.IsKeepRotatingStopped = False

                    If Me.Tab1ManualRadioButton.Checked Then

                        Me.ProgressBar1.Maximum = myScreenDelegate.ReactionsRotorRotatingTimes
                        Me.ProgressBar1.Visible = True
                        Me.ProgressBar1.Value = 1

                        myScreenDelegate.FillMode = FILL_MODE.MANUAL
                        myScreenDelegate.CurrentOperation = ThermosAdjustmentsDelegate.OPERATIONS.CONDITIONING_ROTOR
                        myScreenDelegate.ReactionsRotorHomesDone = False
                        myScreenDelegate.ReactionsRotorConditioningManualFirstStepDone = False
                        myScreenDelegate.ReactionsRotorConditioningManualSecondStepDone = False
                        MyClass.PrepareArea()

                    ElseIf Me.Tab1AutoRadioButton.Checked Then
                        'Me.ProgressBar1.Maximum = myScreenDelegate.ReactionsRotorWellsFilledCount

                        myScreenDelegate.FillMode = FILL_MODE.AUTOMATIC
                        myScreenDelegate.CurrentOperation = ThermosAdjustmentsDelegate.OPERATIONS.CONDITIONING_ROTOR
                        myScreenDelegate.ReactionsRotorConditioningDone = False
                        MyClass.PrepareArea()

                        For i As Integer = 0 To myScreenDelegate.ReactionsRotorWellFilled.Count - 1
                            myScreenDelegate.ReactionsRotorWellFilled(i) = False
                        Next
                        myScreenDelegate.ReactionsRotorCurrentWell = 1

                        ' Inform message for filling the rotor
                        Dim dialogResultToReturn As DialogResult
                        dialogResultToReturn = MyBase.ShowMessage(Me.Name, Messages.SRV_FILL_ROTOR.ToString)
                        Application.DoEvents()

                        If dialogResultToReturn = Windows.Forms.DialogResult.OK Then
                            Me.ProgressBar1.Maximum = myScreenDelegate.ReactionsRotorRotatingTimes
                            Me.ProgressBar1.Visible = True
                            Me.ProgressBar1.Value = 1
                        Else
                            myScreenDelegate.CurrentOperation = ThermosAdjustmentsDelegate.OPERATIONS._NONE
                            myScreenDelegate.IsKeepRotating = False
                            myScreenDelegate.IsKeepRotatingStopped = False
                            MyClass.IsRotorConditioningRequested = False
                            Me.LastConditioningDone = LastConditioningDoneEnum._None

                            MyClass.IsRotorConditioningRequested = False

                            MyBase.CurrentMode = ADJUSTMENT_MODES.ADJUSTMENTS_READED
                            MyClass.PrepareArea()

                            MyBase.DisplayMessage(Messages.SRV_ROTOR_COND_CANCELLED.ToString)
                            Exit Sub
                        End If


                    End If

                    MyBase.DisplayMessage(Messages.SRV_CONDITIONING_SYSTEM.ToString)

                    If Not myGlobal.HasError Then
                        If MyBase.SimulationMode Then

                            Dim isCancelled As Boolean = False


                            'MANUAL
                            If Me.Tab1ManualRadioButton.Checked Then

                                'down
                                Me.ProgressBar1.Maximum = 3
                                Me.ProgressBar1.Value = 0
                                Me.ProgressBar1.Visible = True

                                For D As Integer = 1 To 3
                                    System.Threading.Thread.Sleep(SimulationProcessTime)
                                    Me.ProgressBar1.Value = D
                                    Application.DoEvents()
                                    If myScreenDelegate.RotorConditioningCancelRequested Then
                                        isCancelled = True
                                        Exit For
                                    End If
                                Next

                                If Not isCancelled Then
                                    myScreenDelegate.ReactionsRotorConditioningManualFirstStepDone = True
                                    MyBase.CurrentMode = ADJUSTMENT_MODES.TESTED
                                    MyClass.PrepareArea()
                                Else
                                    MyClass.myScreenDelegate.RotorConditioningCancelRequested = False
                                    MyBase.CurrentMode = ADJUSTMENT_MODES.ADJUSTMENTS_READED
                                    MyClass.PrepareArea()
                                    MyBase.DisplayMessage(Messages.SRV_ROTOR_COND_CANCELLED.ToString)
                                    MyClass.IsRotorConditioningRequested = False
                                    MyClass.IsRotorConditioningCancelRequested = False
                                End If

                                myScreenDelegate.ReactionsRotorHomesDone = True
                                Me.ProgressBar1.Visible = False

                                'Dim dialogResultToReturn As DialogResult
                                'dialogResultToReturn = MyBase.ShowMessage(GetMessageText(Messages.SRV_ADJUSTMENTS_TESTS.ToString), Messages.SRV_ROTOR_FILLED.ToString)

                                If Not isCancelled Then
                                    Me.ProgressBar1.Maximum = 3
                                    Me.ProgressBar1.Value = 0
                                    Me.ProgressBar1.Visible = True

                                    For D As Integer = 1 To 3
                                        System.Threading.Thread.Sleep(SimulationProcessTime)
                                        Me.ProgressBar1.Value = D
                                        Application.DoEvents()
                                        If myScreenDelegate.RotorConditioningCancelRequested Then
                                            isCancelled = True
                                            Exit For
                                        End If
                                    Next

                                    If Not isCancelled Then
                                        myScreenDelegate.ReactionsRotorConditioningManualSecondStepDone = True
                                        myScreenDelegate.ReactionsRotorConditioningDone = True
                                        MyBase.CurrentMode = ADJUSTMENT_MODES.TESTED
                                        MyClass.PrepareArea()

                                        Me.ProgressBar1.Maximum = myScreenDelegate.ReactionsRotorRotatingTimes
                                        Me.ProgressBar1.Value = 0
                                        Me.ProgressBar1.Visible = True

                                        If myScreenDelegate.CurrentOperation = ThermosAdjustmentsDelegate.OPERATIONS.ROTATING_ROTOR Then
                                            'rotating
                                            For R As Integer = 1 To myScreenDelegate.ReactionsRotorRotatingTimes
                                                System.Threading.Thread.Sleep(SimulationProcessTime)
                                                Me.ProgressBar1.Value = R
                                                Application.DoEvents()
                                                If myScreenDelegate.RotorRotatingCancelRequested Then
                                                    isCancelled = True
                                                    Exit For
                                                End If
                                            Next

                                            If Not isCancelled Then
                                                myScreenDelegate.ReactionsRotorRotatingDone = True
                                                myScreenDelegate.ConditioningExecuted(0) = ThermosAdjustmentsDelegate.ResultsOperations.OK
                                                MyBase.CurrentMode = ADJUSTMENT_MODES.TESTED
                                                MyClass.PrepareArea()
                                            Else
                                                MyClass.myScreenDelegate.RotorConditioningCancelRequested = False
                                                MyBase.CurrentMode = ADJUSTMENT_MODES.ADJUSTMENTS_READED
                                                MyClass.PrepareArea()
                                                MyBase.DisplayMessage(Messages.SRV_ROTOR_COND_CANCELLED.ToString)
                                                MyClass.IsRotorConditioningRequested = False
                                                MyClass.IsRotorConditioningCancelRequested = False
                                            End If
                                        End If

                                    Else
                                        MyClass.myScreenDelegate.RotorConditioningCancelRequested = False
                                        MyBase.CurrentMode = ADJUSTMENT_MODES.ADJUSTMENTS_READED
                                        MyClass.PrepareArea()
                                        MyBase.DisplayMessage(Messages.SRV_ROTOR_COND_CANCELLED.ToString)
                                        MyClass.IsRotorConditioningRequested = False
                                        MyClass.IsRotorConditioningCancelRequested = False
                                    End If

                                End If

                                Me.ProgressBar1.Visible = False




                                'AUTOMATIC
                            ElseIf Me.Tab1AutoRadioButton.Checked Then

                                Me.ProgressBar1.Maximum = 1 + myScreenDelegate.ReactionsRotorWellFilled.Count
                                Me.ProgressBar1.Visible = True

                                'priming
                                Me.ProgressBar1.Value = 1
                                Me.ProgressBar1.Refresh()

                                'wells filling
                                Me.ProgressBar1.Value = 2
                                Me.ProgressBar1.Refresh()
                                For W As Integer = 1 To myScreenDelegate.ReactionsRotorWellFilled.Count
                                    System.Threading.Thread.Sleep(2 * SimulationProcessTime)
                                    Me.ProgressBar1.Value = W
                                    Application.DoEvents()
                                    If myScreenDelegate.RotorConditioningCancelRequested Then
                                        isCancelled = True
                                        Exit For
                                    End If
                                Next

                                If Not isCancelled Then
                                    MyClass.myScreenDelegate.ReactionsRotorHomesDone = True
                                    MyClass.myScreenDelegate.ReactionsRotorConditioningDone = True
                                    MyBase.CurrentMode = ADJUSTMENT_MODES.TESTED
                                    MyClass.PrepareArea()
                                Else
                                    MyClass.myScreenDelegate.ReactionsRotorConditioningDone = False
                                    MyClass.IsRotorConditioningCancelRequested = False
                                    MyClass.myScreenDelegate.RotorConditioningCancelRequested = False
                                    MyClass.myScreenDelegate.RotorRotatingCancelRequested = False
                                    MyBase.CurrentMode = ADJUSTMENT_MODES.ADJUSTMENTS_READED
                                    MyClass.PrepareArea()
                                    MyBase.DisplayMessage(Messages.SRV_ROTOR_COND_CANCELLED.ToString)
                                    MyClass.IsRotorConditioningRequested = False
                                    MyClass.IsRotorConditioningCancelRequested = False
                                End If

                                Me.ProgressBar1.Visible = False

                                If Not isCancelled Then

                                    Me.ProgressBar1.Maximum = myScreenDelegate.ReactionsRotorRotatingTimes
                                    Me.ProgressBar1.Value = 0
                                    Me.ProgressBar1.Visible = True

                                    myScreenDelegate.CurrentOperation = ThermosAdjustmentsDelegate.OPERATIONS.ROTATING_ROTOR

                                    'rotating
                                    For R As Integer = 1 To myScreenDelegate.ReactionsRotorRotatingTimes
                                        System.Threading.Thread.Sleep(SimulationProcessTime)
                                        Me.ProgressBar1.Value = R
                                        Application.DoEvents()
                                        If myScreenDelegate.RotorRotatingCancelRequested Then
                                            isCancelled = True
                                            Exit For
                                        End If
                                    Next

                                    If Not isCancelled Then
                                        ' Display message to User for finish rotor movement and sound
                                        Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate
                                        MessageBox.Show(GetMessageText(Messages.SRV_PHOTOMETRY_TESTS.ToString), myMultiLangResourcesDelegate.GetResourceText(Nothing, "MSG_SRV_CONDITIONING_DONE", currentLanguage))
                                    End If

                                    Me.ProgressBar1.Visible = False

                                    If Not isCancelled Then
                                        myScreenDelegate.ConditioningExecuted(0) = ThermosAdjustmentsDelegate.ResultsOperations.OK
                                        MyBase.CurrentMode = ADJUSTMENT_MODES.TESTED
                                        MyClass.PrepareArea()
                                    Else
                                        myScreenDelegate.IsKeepRotatingStopped = True
                                        MyBase.CurrentMode = ADJUSTMENT_MODES.ADJUSTMENTS_READED
                                        MyClass.PrepareArea()
                                        MyBase.DisplayMessage(Messages.SRV_ROTOR_COND_CANCELLED.ToString)
                                        MyClass.IsRotorConditioningRequested = False
                                        MyClass.IsRotorConditioningCancelRequested = False
                                    End If

                                End If

                            End If

                        Else

                            'get script parameters
                            myScreenDelegate.WSFinalPos = CInt(ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.WASHING_STATION.ToString, GlobalEnumerates.AXIS.Z, True).Value)
                            myScreenDelegate.WSReferencePos = CInt(ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.WASHING_STATION_PARK.ToString, GlobalEnumerates.AXIS.Z, True).Value)

                            ' Manage FwScripts must to be sent to send Conditioning instructions to Instrument
                            SendFwScript(Me.CurrentMode)
                            If myScreenDelegate.ReactionsRotorHomesDone Then
                                MyBase.CurrentMode = ADJUSTMENT_MODES.TESTED
                                PrepareArea()
                            End If
                        End If
                    End If
                End If
            End If 'SGM 02/12/2011

        Catch ex As Exception
            MyClass.myScreenDelegate.CurrentOperation = ThermosAdjustmentsDelegate.OPERATIONS._NONE
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".Tab1ConditioningButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".Tab1ConditioningButton_Click ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ' XBC 18/10/2011 - Canceled
    'Private Sub Tab1MeasureButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Tab1MeasureButton.Click
    '    Dim myGlobal As New GlobalDataTO
    '    Try
    '        Dim dialogResultToReturn As DialogResult = Windows.Forms.DialogResult.Yes

    '        ' Check if Monitored Temperature is inside of limits
    '        If myScreenDelegate.MonitorThermoRotor > Me.EditedValue.TargetMaxValue Or myScreenDelegate.MonitorThermoRotor < Me.EditedValue.TargetMinValue Then
    '            dialogResultToReturn = mybase.ShowMessage(GetMessageText(GlobalEnumerates.Messages.SRV_ADJUSTMENTS_TESTS.ToString), Messages.SRV_STABILIZE_THERMO_WARN.ToString)
    '            If dialogResultToReturn = Windows.Forms.DialogResult.Yes Then
    '                Exit Try
    '            End If
    '        End If

    '        ' Check if the corresponding conditioning has been executed
    '        If Me.LastConditioningDone <> LastConditioningDoneEnum.ConditioningRotor Then
    '            dialogResultToReturn = mybase.ShowMessage(GetMessageText(GlobalEnumerates.Messages.SRV_ADJUSTMENTS_TESTS.ToString), Messages.SRV_CONDITIONING_THERMO_WARN.ToString)
    '            If dialogResultToReturn <> Windows.Forms.DialogResult.Yes Then
    '                Exit Try
    '            End If
    '        End If

    '        dialogResultToReturn = mybase.ShowMessage(GetMessageText(GlobalEnumerates.Messages.SRV_ADJUSTMENTS_TESTS.ToString), Messages.SRV_WARNING_THERMO_MOVS.ToString)
    '        If dialogResultToReturn <> Windows.Forms.DialogResult.Yes Then
    '            CancelWizardEditionTemps()
    '        Else

    '            Me.EditingRotorTemperatures = True

    '            myGlobal = MyBase.Test
    '            If myGlobal.HasError Then
    '                PrepareErrorMode()
    '            Else
    '                PrepareArea()

    '                ' Initializations
    '                Me.InitializeRotorReationsTemps()
    '                Me.EditionMode = True
    '                myScreenDelegate.CurrentOperation = ThermosAdjustmentsDelegate.OPERATIONS.MEASURE_TEMPERATURE
    '                myScreenDelegate.ReactionsRotorMeasuredTempDone = False
    '                For i As Integer = 0 To myScreenDelegate.ReactionsRotorPrepareWellDone.Count - 1
    '                    myScreenDelegate.ReactionsRotorPrepareWellDone(i) = False
    '                    myScreenDelegate.ProbeWellsTemps(i) = 0
    '                Next
    '                myScreenDelegate.ReactionsRotorCurrentWell = 1
    '                myScreenDelegate.CurrentTest = ADJUSTMENT_GROUPS.THERMOS_PHOTOMETRY

    '                MyBase.DisplayMessage(Messages.SRV_ADJUSTMENT_IN_PROCESS.ToString)

    '                If Not myGlobal.HasError Then
    '                    If MyBase.SimulationMode Then
    '                        ' simulating
    '                        'MyBase.DisplaySimulationMessage("Measuring Temperature into Reactions Rotor...")
    '                        'Me.Cursor = Cursors.WaitCursor
    '                        System.Threading.Thread.Sleep(SimulationProcessTime)
    '                        MyBase.myServiceMDI.Focus()
    '                        'Me.Cursor = Cursors.Default
    '                        MyBase.CurrentMode = ADJUSTMENT_MODES.TESTED
    '                        myScreenDelegate.FinalResult(0) = ThermosAdjustmentsDelegate.ResultsOperations.NOK
    '                        myScreenDelegate.MeasuringTempExecuted(0) = ThermosAdjustmentsDelegate.ResultsOperations.OK
    '                        myScreenDelegate.ReactionsRotorPrepareWellDone(myScreenDelegate.ReactionsRotorCurrentWell - 1) = True
    '                        If myScreenDelegate.AreReactionsRotorAllPrepared Then
    '                            myScreenDelegate.ReactionsRotorMeasuredTempDone = True
    '                        End If
    '                        PrepareArea()
    '                    Else
    '                        ' Manage FwScripts must to be sent to send Measuring Temperature instructions to Instrument
    '                        SendFwScript(Me.CurrentMode)
    '                    End If
    '                End If
    '            End If

    '        End If

    '    Catch ex As Exception
    '        GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".Tab1MeasureButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
    '        mybase.ShowMessage(Me.Name & ".Tab1MeasureButton_Click ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
    '    End Try
    'End Sub

    'Private Sub Tab1UndoMeasureButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Tab1UndoMeasureButton.Click
    '    Try
    '        Me.CancelWizardEditionTemps()
    '    Catch ex As Exception
    '        GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".Tab1UndoMeasureButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
    '        mybase.ShowMessage(Me.Name & ".Tab1UndoMeasureButton_Click ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
    '    End Try
    'End Sub

    Private Sub Tab1AdjustButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Tab1AdjustButton.Click
        Dim myGlobal As New GlobalDataTO
        Try
            If Me.EditingRotorTemperatures Then
                Exit Sub
            End If

            myGlobal = MyBase.Test
            If myGlobal.HasError Then
                PrepareErrorMode()
            Else
                PrepareArea()

                ' Initializations
                myScreenDelegate.CurrentOperation = ThermosAdjustmentsDelegate.OPERATIONS.TEST_ADJUSTMENT
                myScreenDelegate.ReactionsRotorTestAdjustmentDone = False
                myScreenDelegate.CurrentTest = ADJUSTMENT_GROUPS.THERMOS_PHOTOMETRY

                Me.EditedValue.SetPointNewValue = Me.EditedValue.SetPointCurrentValue + Me.EditedValue.NewCorrection
                myScreenDelegate.ReactionsRotorTCO = Me.EditedValue.SetPointNewValue.ToString("0.0").Replace(MyClass.myDecimalSeparator.ToString, ",")

                MyBase.DisplayMessage(Messages.SRV_TEST_IN_PROCESS.ToString)

                If Not myGlobal.HasError Then
                    If MyBase.SimulationMode Then
                        ' simulating
                        'MyBase.DisplaySimulationMessage("Doing specified Test...")
                        'Me.Cursor = Cursors.WaitCursor
                        System.Threading.Thread.Sleep(SimulationProcessTime)
                        MyBase.myServiceMDI.Focus()
                        'Me.Cursor = Cursors.Default
                        MyBase.CurrentMode = ADJUSTMENT_MODES.TESTED
                        myScreenDelegate.ReactionsRotorTestAdjustmentDone = True
                        PrepareArea()
                    Else
                        ' Manage FwScripts must to be sent to testing
                        SendFwScript(Me.CurrentMode)
                    End If
                End If

            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".Tab1AdjustButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".Tab1AdjustButton_Click ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ' XBC 29/11/2011
    Private Sub Tab1TextBoxTemp1_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles Tab1TextBoxTemp1.GotFocus
        Try
            'Me.Tab1TextBoxTemp1.Text = "0"
            'Me.Tab1TextBoxTemp1.Clear()
            Me.TempBoxEditing = 1

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".Tab1TextBoxTemp1_GotFocus ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".Tab1TextBoxTemp1_GotFocus ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub Tab1TextBoxTemp2_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles Tab1TextBoxTemp2.GotFocus
        Try
            'Me.Tab1TextBoxTemp2.Text = "0"
            'Me.Tab1TextBoxTemp2.Clear()
            Me.TempBoxEditing = 2

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".Tab1TextBoxTemp2_GotFocus ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".Tab1TextBoxTemp2_GotFocus ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub Tab1TextBoxTemp3_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles Tab1TextBoxTemp3.GotFocus
        Try
            'Me.Tab1TextBoxTemp3.Text = "0"
            'Me.Tab1TextBoxTemp3.Clear()
            Me.TempBoxEditing = 3

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".Tab1TextBoxTemp3_GotFocus ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".Tab1TextBoxTemp3_GotFocus ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub Tab1TextBoxTemp4_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles Tab1TextBoxTemp4.GotFocus
        Try
            'Me.Tab1TextBoxTemp4.Text = "0"
            'Me.Tab1TextBoxTemp4.Clear()
            Me.TempBoxEditing = 4

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".Tab1TextBoxTemp4_GotFocus ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".Tab1TextBoxTemp4_GotFocus ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub Tab1TextBoxTemp4_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles Tab1TextBoxTemp4.KeyPress
        Try
            If Asc(e.KeyChar) = 13 Then
                ValidateToCalulatePHOTOMETRYMeanTemps()
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".Tab1TextBoxTemp4_KeyPress ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".Tab1TextBoxTemp4_KeyPress ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub Tab1TextBoxTempX_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles Tab1TextBoxTemp1.LostFocus, _
                                                                                                        Tab1TextBoxTemp2.LostFocus, _
                                                                                                        Tab1TextBoxTemp3.LostFocus, _
                                                                                                        Tab1TextBoxTemp4.LostFocus
        Try
            ValidateToCalulatePHOTOMETRYMeanTemps()

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".Tab1TextBoxTempX_LostFocus ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".Tab1TextBoxTempX_LostFocus ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub
    ' XBC 29/11/2011

    Private Sub Tab1TextBoxTemp1_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Tab1TextBoxTemp1.TextChanged
        'Dim Utilities As New Utilities
        Try
            If Me.PageInitialized Then

                ' XBC 18/10/2011 - Cancelled
                'If Me.EditionMode Then
                If Me.Tab1TextBoxTemp1.Text.Length > 0 Then
                    If IsNumeric(Me.Tab1TextBoxTemp1.Text) Then
                        ' XBC 18/11/2011 - correction singles
                        'myScreenDelegate.ProbeWellsTemps(0) = CSng(Me.Tab1TextBoxTemp1.Text.Replace(".", MyClass.myDecimalSeparator.ToString))
                        myScreenDelegate.ProbeWellsTemps(0) = Utilities.FormatToSingle(Me.Tab1TextBoxTemp1.Text)

                        'ValidateToCalulatePHOTOMETRYMeanTemps()    ' XBC 29/11/2011
                    End If
                    Me.TempBoxEditing = 1
                End If
                'End If
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".Tab1TextBoxTemp1_TextChanged ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".Tab1TextBoxTemp1_TextChanged ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub Tab1TextBoxTemp2_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Tab1TextBoxTemp2.TextChanged
        'Dim Utilities As New Utilities
        Try
            If Me.PageInitialized Then

                ' XBC 18/10/2011 - Cancelled
                'If Me.EditionMode Then
                If Me.Tab1TextBoxTemp2.Text.Length > 0 Then
                    If IsNumeric(Me.Tab1TextBoxTemp2.Text) Then
                        ' XBC 18/11/2011 - correction singles
                        'myScreenDelegate.ProbeWellsTemps(1) = CSng(Me.Tab1TextBoxTemp2.Text.Replace(".", MyClass.myDecimalSeparator.ToString))
                        myScreenDelegate.ProbeWellsTemps(1) = Utilities.FormatToSingle(Me.Tab1TextBoxTemp2.Text)

                        'ValidateToCalulatePHOTOMETRYMeanTemps()    ' XBC 29/11/2011
                    End If
                    Me.TempBoxEditing = 2
                End If
                'End If
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".Tab1TextBoxTemp2_TextChanged ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".Tab1TextBoxTemp2_TextChanged ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub Tab1TextBoxTemp3_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Tab1TextBoxTemp3.TextChanged
        'Dim Utilities As New Utilities
        Try
            If Me.PageInitialized Then

                ' XBC 18/10/2011 - Cancelled
                'If Me.EditionMode Then
                If Me.Tab1TextBoxTemp3.Text.Length > 0 Then
                    If IsNumeric(Me.Tab1TextBoxTemp3.Text) Then
                        ' XBC 18/11/2011 - correction singles
                        'myScreenDelegate.ProbeWellsTemps(2) = CSng(Me.Tab1TextBoxTemp3.Text.Replace(".", MyClass.myDecimalSeparator.ToString))
                        myScreenDelegate.ProbeWellsTemps(2) = Utilities.FormatToSingle(Me.Tab1TextBoxTemp3.Text)

                        'ValidateToCalulatePHOTOMETRYMeanTemps()    ' XBC 29/11/2011
                    End If
                    Me.TempBoxEditing = 3
                End If
                'End If
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".Tab1TextBoxTemp3_TextChanged ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".Tab1TextBoxTemp3_TextChanged ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub Tab1TextBoxTemp4_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Tab1TextBoxTemp4.TextChanged
        'Dim Utilities As New Utilities
        Try
            If Me.PageInitialized Then

                ' XBC 18/10/2011 - Cancelled
                'If Me.EditionMode Then
                If Me.Tab1TextBoxTemp4.Text.Length > 0 Then
                    If IsNumeric(Me.Tab1TextBoxTemp4.Text) Then
                        ' XBC 18/11/2011 - correction singles
                        'myScreenDelegate.ProbeWellsTemps(3) = CSng(Me.Tab1TextBoxTemp4.Text.Replace(".", MyClass.myDecimalSeparator.ToString))
                        myScreenDelegate.ProbeWellsTemps(3) = Utilities.FormatToSingle(Me.Tab1TextBoxTemp4.Text)

                        'ValidateToCalulatePHOTOMETRYMeanTemps()    ' XBC 29/11/2011
                    End If
                    Me.TempBoxEditing = 4
                End If
                'End If
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".Tab1TextBoxTemp4_TextChanged ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".Tab1TextBoxTemp4_TextChanged ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ' XBC 18/10/2011 - Cancelled
    'Private Sub Tab1TextBoxTemp5_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Tab1TextBoxTemp5.TextChanged
    '    Try
    '        If Me.EditionMode Then
    '            If Me.Tab1TextBoxTemp5.Text.Length > 0 Then
    '                If IsNumeric(Me.Tab1TextBoxTemp5.Text) Then
    '                    myScreenDelegate.ProbeWellsTemps(4) = CSng(Me.Tab1TextBoxTemp5.Text.Replace(".", MyClass.myDecimalSeparator.ToString))

    '                    ValidateToCalulatePHOTOMETRYMeanTemps()
    '                End If
    '            End If
    '        End If

    '    Catch ex As Exception
    '        GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".Tab1TextBoxTemp5_TextChanged ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
    '        mybase.ShowMessage(Me.Name & ".Tab1TextBoxTemp5_TextChanged ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
    '    End Try
    'End Sub

    'Private Sub Tab1TextBoxTemp6_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Tab1TextBoxTemp6.TextChanged
    '    Try
    '        If Me.EditionMode Then
    '            If Me.Tab1TextBoxTemp6.Text.Length > 0 Then
    '                If IsNumeric(Me.Tab1TextBoxTemp6.Text) Then
    '                    myScreenDelegate.ProbeWellsTemps(5) = CSng(Me.Tab1TextBoxTemp6.Text.Replace(".", MyClass.myDecimalSeparator.ToString))

    '                    ValidateToCalulatePHOTOMETRYMeanTemps()
    '                End If
    '            End If
    '        End If

    '    Catch ex As Exception
    '        GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".Tab1TextBoxTemp6_TextChanged ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
    '        mybase.ShowMessage(Me.Name & ".Tab1TextBoxTemp6_TextChanged ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
    '    End Try
    'End Sub

    'Private Sub Tab1TextBoxTemp7_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Tab1TextBoxTemp7.TextChanged
    '    Try
    '        If Me.EditionMode Then
    '            If Me.Tab1TextBoxTemp7.Text.Length > 0 Then
    '                If IsNumeric(Me.Tab1TextBoxTemp7.Text) Then
    '                    myScreenDelegate.ProbeWellsTemps(6) = CSng(Me.Tab1TextBoxTemp7.Text.Replace(".", MyClass.myDecimalSeparator.ToString))

    '                    ValidateToCalulatePHOTOMETRYMeanTemps()
    '                End If
    '            End If
    '        End If

    '    Catch ex As Exception
    '        GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".Tab1TextBoxTemp7_TextChanged ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
    '        mybase.ShowMessage(Me.Name & ".Tab1TextBoxTemp7_TextChanged ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
    '    End Try
    'End Sub

    'Private Sub Tab1TextBoxTemp8_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Tab1TextBoxTemp8.TextChanged
    '    Try
    '        If Me.EditionMode Then
    '            If Me.Tab1TextBoxTemp8.Text.Length > 0 Then
    '                If IsNumeric(Me.Tab1TextBoxTemp8.Text) Then
    '                    myScreenDelegate.ProbeWellsTemps(7) = CSng(Me.Tab1TextBoxTemp8.Text.Replace(".", MyClass.myDecimalSeparator.ToString))

    '                    ValidateToCalulatePHOTOMETRYMeanTemps()
    '                End If
    '            End If
    '        End If

    '    Catch ex As Exception
    '        GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".Tab1TextBoxTemp8_TextChanged ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
    '        mybase.ShowMessage(Me.Name & ".Tab1TextBoxTemp8_TextChanged ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
    '    End Try
    'End Sub

    'Private Sub Tab1CorrectionTextBox_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles Tab1CorrectionTextBox.GotFocus
    '    Try
    '        If Me.PageInitialized Then
    '            Tab1CorrectionTextBox_StatusChanged()
    '        End If

    '    Catch ex As Exception
    '        GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".Tab1CorrectionTextBox_GotFocus ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
    '        mybase.ShowMessage(Me.Name & ".Tab1CorrectionTextBox_GotFocus ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
    '    End Try
    'End Sub

    Private Sub Tab1CorrectionTextBox_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Tab1CorrectionTextBox.TextChanged
        Try
            If Me.PageInitialized Then
                Tab1CorrectionTextBox_StatusChanged()
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".Tab1CorrectionTextBox_TextChanged ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".Tab1CorrectionTextBox_TextChanged ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub Tab1CorrectionTextBox_StatusChanged()
        'Dim Utilities As New Utilities
        Try

            If Me.Tab1CorrectionTextBox.Text.Length = 0 Then
                Me.Tab1AdjustButton.Enabled = False
                Me.BsSaveButton.Enabled = False
            Else
                If IsNumeric(Me.Tab1CorrectionTextBox.Text) Then
                    ' XBC 18/11/2011 - correction singles
                    'Me.EditedValue.NewCorrection = CSng(Me.Tab1CorrectionTextBox.Text.Replace(".", MyClass.myDecimalSeparator.ToString))
                    Me.EditedValue.NewCorrection = Utilities.FormatToSingle(Me.Tab1CorrectionTextBox.Text)
                End If

                ' XBC 18/10/2011 - Canceled
                'If myScreenDelegate.ReactionsRotorMeasuredTempDone Then
                Me.Tab1AdjustButton.Enabled = True
                'Me.BsSaveButton.Enabled = True
                'End If
            End If

            Me.EditingRotorTemperatures = False
            Me.EditingRotorCorrection = True
            Me.EditingNeedleTemperature = False
            Me.EditingNeedleCorrection = False
            Me.EditingHeaterTemperature = False
            Me.EditingHeaterCorrection = False

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".Tab1CorrectionTextBox_StatusChanged ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".Tab1CorrectionTextBox_StatusChanged ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub BsUpDownWSButton2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsUpDownWSButton2.Click
        Try
            myScreenDelegate.CurrentTest = ADJUSTMENT_GROUPS.THERMOS_PHOTOMETRY
            Me.SendWASH_STATION_CTRL()

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".BsUpDownWSButton2_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".BsUpDownWSButton2_Click ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    'Private Sub TextBox_KeyPress(ByVal sender As System.Object, _
    '                             ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles Tab1TextBoxTemp1.KeyPress, _
    '                                                                                        Tab1TextBoxTemp2.KeyPress, _
    '                                                                                        Tab1TextBoxTemp3.KeyPress, _
    '                                                                                        Tab1TextBoxTemp4.KeyPress, _
    '                                                                                        Tab1CorrectionTextBox.KeyPress, _
    '                                                                                        Tab2TextBoxTemp.KeyPress, _
    '                                                                                        Tab2CorrectionTextBox.KeyPress, _
    '                                                                                        Tab3TextBoxTemp.KeyPress, _
    '                                                                                        Tab3CorrectionTextBox.KeyPress
    '    Try
    '        If Char.IsControl(e.KeyChar) Then
    '            e.Handled = False
    '        Else
    '            If e.KeyChar = "." Or e.KeyChar = "," Then
    '                If CType(sender, BSTextBox).DecimalsValues Then
    '                    e.KeyChar = CChar(SystemInfoManager.OSDecimalSeparator)
    '                    If CType(sender, BSTextBox).Text.Contains(".") Or CType(sender, BSTextBox).Text.Contains(",") Then
    '                        e.Handled = True
    '                    End If
    '                Else
    '                    e.Handled = True
    '                End If
    '            End If
    '        End If
    '    Catch ex As Exception
    '        GlobalBase.CreateLogActivity(ex.Message, "TextBox_KeyPress " & Name, EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
    '        ShowMessage(Name & ".TextBox_KeyPress", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
    '    End Try
    'End Sub
#End Region

#Region "TAB NEEDLES"

    Private Sub Tab2RadioButtonR1_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Tab2RadioButtonR1.CheckedChanged
        Try
            If Me.PageInitialized Then

                Me.GenerateReportsOutput()

                With Me.EditedValue
                    If Me.Tab2RadioButtonR1.Checked Then
                        .AdjustmentID = ADJUSTMENT_GROUPS.THERMOS_REAGENT1

                        LoadAdjustmentGroupData()
                        PopulateEditionValues()

                        If myScreenDelegate IsNot Nothing Then
                            myScreenDelegate.CurrentTest = ADJUSTMENT_GROUPS.THERMOS_REAGENT1
                        End If

                        Dim isPrimed As Boolean = myScreenDelegate.ReagentNeedleConditioningDone
                        Me.Tab2MeasureButton.Enabled = isPrimed
                        Me.Tab2AdjustButton.Enabled = isPrimed
                        Me.Tab2TextBoxTemp.Enabled = isPrimed
                        Me.Tab2CorrectionTextBox.Enabled = isPrimed

                        MyBase.CurrentMode = ADJUSTMENT_MODES.LOADED
                        PrepareArea()
                        MyBase.DisplayMessage("")

                        'clear temperatute textboxes
                        Me.PageInitialized = False
                        Me.Tab2TextBoxTemp.Text = ""
                        Me.Tab2CorrectionTextBox.Text = ""
                        Me.PageInitialized = True
                    End If
                End With

            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".Tab2RadioButtonR1_CheckedChanged ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".Tab2RadioButtonR1_CheckedChanged ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub Tab2RadioButtonR2_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Tab2RadioButtonR2.CheckedChanged
        Try
            If Me.PageInitialized Then

                Me.GenerateReportsOutput()

                With Me.EditedValue
                    If Me.Tab2RadioButtonR2.Checked Then
                        .AdjustmentID = ADJUSTMENT_GROUPS.THERMOS_REAGENT2

                        LoadAdjustmentGroupData()
                        PopulateEditionValues()

                        If myScreenDelegate IsNot Nothing Then
                            myScreenDelegate.CurrentTest = ADJUSTMENT_GROUPS.THERMOS_REAGENT2
                        End If

                        Dim isPrimed As Boolean = myScreenDelegate.ReagentNeedleConditioningDone
                        Me.Tab2MeasureButton.Enabled = isPrimed
                        Me.Tab2AdjustButton.Enabled = isPrimed
                        Me.Tab2TextBoxTemp.Enabled = isPrimed
                        Me.Tab2CorrectionTextBox.Enabled = isPrimed


                        MyBase.CurrentMode = ADJUSTMENT_MODES.LOADED
                        PrepareArea()
                        MyBase.DisplayMessage("")

                        'clear temperatute textboxes
                        Me.PageInitialized = False
                        Me.Tab2TextBoxTemp.Text = ""
                        Me.Tab2CorrectionTextBox.Text = ""
                        Me.PageInitialized = True
                    End If
                End With

            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".Tab2RadioButtonR2_CheckedChanged ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".Tab2RadioButtonR2_CheckedChanged ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub Tab2ConditioningButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Tab2ConditioningButton.Click
        Dim myGlobal As New GlobalDataTO
        Try
            myGlobal = MyBase.Test
            If myGlobal.HasError Then
                MyClass.PrepareErrorMode()
            Else

                'SGM MEETING 13/10/2011


                'If Not myScreenDelegate.ReagentNeedleConditioningDone Then

                MyClass.myScreenDelegate.CurrentOperation = ThermosAdjustmentsDelegate.OPERATIONS.CONDITIONING_NEEDLE
                MyClass.PrepareArea()

                Dim dialogResultToReturn As DialogResult
                dialogResultToReturn = MyBase.ShowMessage(GetMessageText(Messages.SRV_ADJUSTMENTS_TESTS.ToString), Messages.SRV_THERMO_PRIME_WARN.ToString)
                Application.DoEvents()

                If dialogResultToReturn = Windows.Forms.DialogResult.No Then

                    MyBase.CurrentMode = ADJUSTMENT_MODES.LOADED
                    MyClass.PrepareArea()

                ElseIf dialogResultToReturn = Windows.Forms.DialogResult.Yes Then

                    ' Initializations
                    If Me.Tab2RadioButtonR1.Checked Then
                        Me.LastConditioningDone = LastConditioningDoneEnum.ConditioningReagent1
                    Else
                        Me.LastConditioningDone = LastConditioningDoneEnum.ConditioningReagent2
                    End If

                    'initialize process variables
                    'myScreenDelegate.ReagentNeedleConditioningDoing = False
                    myScreenDelegate.ReagentNeedleConditioningDone = False
                    myScreenDelegate.CurrentTest = Me.EditedValue.AdjustmentID
                    'Me.ProgressBar1.Maximum = myScreenDelegate.ReagentNeedleHeatMaxDispensations

                    'MyBase.DisplayMessage(Messages.SRV_THERMO_PRIMING.ToString)

                    If Not myGlobal.HasError Then

                        If MyBase.SimulationMode Then

                            MyBase.DisplayMessage(Messages.SRV_CONDITIONING.ToString)

                            Me.ProgressBar1.Value = 0
                            Me.ProgressBar1.Maximum = 5
                            Me.ProgressBar1.Visible = True
                            Me.ProgressBar1.Refresh()

                            For P As Integer = 1 To 3 Step 1
                                System.Threading.Thread.Sleep(P * MyBase.SimulationProcessTime)
                                Me.ProgressBar1.Value = P
                                Me.ProgressBar1.Refresh()
                            Next

                            Me.ProgressBar1.Visible = False
                            Me.ProgressBar1.Refresh()

                            myScreenDelegate.ReagentNeedleConditioningDone = True

                            MyBase.CurrentMode = ADJUSTMENT_MODES.TESTED
                            MyClass.PrepareArea()

                        Else

                            MyBase.CurrentMode = ADJUSTMENT_MODES.TESTING
                            Me.PrepareArea()


                            Select Case myScreenDelegate.CurrentTest
                                Case ADJUSTMENT_GROUPS.THERMOS_REAGENT1
                                    'IT 22/07/2015 - BA-2650 (INI)
                                    If (AnalyzerController.Instance.IsBA200) Then
                                        myScreenDelegate.R1WSHorizontalPos = CInt(ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.SAMPLES_ARM_WASH.ToString, GlobalEnumerates.AXIS.POLAR, True).Value)
                                        myScreenDelegate.R1WSVerticalRefPos = CInt(ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.SAMPLES_ARM_WASH.ToString, GlobalEnumerates.AXIS.Z, True).Value)
                                        myScreenDelegate.R1VerticalSafetyPos = CInt(ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.SAMPLES_ARM_VSEC.ToString, GlobalEnumerates.AXIS.Z, True).Value)
                                    Else
                                        myScreenDelegate.R1WSHorizontalPos = CInt(ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.REAGENT1_ARM_WASH.ToString, GlobalEnumerates.AXIS.POLAR, True).Value)
                                        myScreenDelegate.R1WSVerticalRefPos = CInt(ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.REAGENT1_ARM_WASH.ToString, GlobalEnumerates.AXIS.Z, True).Value)
                                        myScreenDelegate.R1VerticalSafetyPos = CInt(ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.REAGENT1_ARM_VSEC.ToString, GlobalEnumerates.AXIS.Z, True).Value)
                                    End If
                                    'IT 22/07/2015 - BA-2650 (END)
                                Case ADJUSTMENT_GROUPS.THERMOS_REAGENT2
                                    myScreenDelegate.R2WSHorizontalPos = CInt(ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.REAGENT2_ARM_WASH.ToString, GlobalEnumerates.AXIS.POLAR, True).Value)
                                    myScreenDelegate.R2WSVerticalRefPos = CInt(ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.REAGENT2_ARM_WASH.ToString, GlobalEnumerates.AXIS.Z, True).Value)
                                    myScreenDelegate.R2VerticalSafetyPos = CInt(ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.REAGENT2_ARM_VSEC.ToString, GlobalEnumerates.AXIS.Z, True).Value)
                            End Select

                            MyBase.DisplayMessage(Messages.SRV_CONDITIONING_SYSTEM.ToString)

                            Me.SendFwScript(MyBase.CurrentMode)

                        End If

                    End If
                End If

                'End If

            End If




        Catch ex As Exception
            MyClass.myScreenDelegate.CurrentOperation = ThermosAdjustmentsDelegate.OPERATIONS._NONE
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".Tab2ConditioningButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".Tab2ConditioningButton_Click ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub Tab2MeasureButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Tab2MeasureButton.Click
        Dim myGlobal As New GlobalDataTO
        'Dim dialogResultToReturn As DialogResult
        Try

            myGlobal = MyBase.Test
            If myGlobal.HasError Then
                MyClass.PrepareErrorMode()
            Else

                MyClass.myScreenDelegate.CurrentOperation = ThermosAdjustmentsDelegate.OPERATIONS.DISPENSING_NEEDLE
                MyClass.PrepareArea()

                ' XBC 22/11/2011 - Counter placed to Delegate to register it into Historics
                'MyClass.NeedlesDispensationsCount = 0
                myScreenDelegate.NeedlesDispensationsCount = 0
                ' XBC 22/11/2011 - Counter placed to Delegate to register it into Historics

                If Not MyBase.SimulationMode Then
                    Select Case myScreenDelegate.CurrentTest
                        Case ADJUSTMENT_GROUPS.THERMOS_REAGENT1
                            'IT 22/07/2015 - BA-2650 (INI)
                            If AnalyzerController.Instance.IsBA200 Then
                                myScreenDelegate.R1RotorPos = CInt(ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.SAMPLES_ARM_RING1.ToString, GlobalEnumerates.AXIS.ROTOR, True).Value)
                                myScreenDelegate.R1RotorRing1HorPos = CInt(ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.SAMPLES_ARM_RING1.ToString, GlobalEnumerates.AXIS.POLAR, True).Value)
                                myScreenDelegate.R1RotorRing1DetMaxVerPos = CInt(ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.SAMPLES_ARM_RING1.ToString, GlobalEnumerates.AXIS.Z, True).Value)
                                myScreenDelegate.R1VerticalSafetyPos = CInt(ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.SAMPLES_ARM_VSEC.ToString, GlobalEnumerates.AXIS.Z, True).Value)
                                myScreenDelegate.R1WSHorizontalPos = CInt(ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.SAMPLES_ARM_WASH.ToString, GlobalEnumerates.AXIS.POLAR, True).Value)
                                ' XBC 19/04/2012 - Change maneuver
                                myScreenDelegate.R1InitialPointLevelDet = CInt(ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.SAMPLES_ARM_LEVEL_DET.ToString, GlobalEnumerates.AXIS.Z, True).Value)
                            Else
                                myScreenDelegate.R1RotorPos = CInt(ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.REAGENT1_ARM_RING1.ToString, GlobalEnumerates.AXIS.ROTOR, True).Value)
                                myScreenDelegate.R1RotorRing1HorPos = CInt(ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.REAGENT1_ARM_RING1.ToString, GlobalEnumerates.AXIS.POLAR, True).Value)
                                myScreenDelegate.R1RotorRing1DetMaxVerPos = CInt(ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.REAGENT1_ARM_RING1.ToString, GlobalEnumerates.AXIS.Z, True).Value)
                                myScreenDelegate.R1VerticalSafetyPos = CInt(ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.REAGENT1_ARM_VSEC.ToString, GlobalEnumerates.AXIS.Z, True).Value)
                                myScreenDelegate.R1WSHorizontalPos = CInt(ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.REAGENT1_ARM_WASH.ToString, GlobalEnumerates.AXIS.POLAR, True).Value)
                                ' XBC 19/04/2012 - Change maneuver
                                myScreenDelegate.R1InitialPointLevelDet = CInt(ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.REAGENT1_ARM_LEVEL.ToString, GlobalEnumerates.AXIS.Z, True).Value)
                            End If
                            'IT 22/07/2015 - BA-2650 (END)

                        Case ADJUSTMENT_GROUPS.THERMOS_REAGENT2
                            myScreenDelegate.R2RotorPos = CInt(ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.REAGENT2_ARM_RING1.ToString, GlobalEnumerates.AXIS.ROTOR, True).Value)
                            myScreenDelegate.R2RotorRing1HorPos = CInt(ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.REAGENT2_ARM_RING1.ToString, GlobalEnumerates.AXIS.POLAR, True).Value)
                            myScreenDelegate.R2RotorRing1DetMaxVerPos = CInt(ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.REAGENT2_ARM_RING1.ToString, GlobalEnumerates.AXIS.Z, True).Value)
                            myScreenDelegate.R2VerticalSafetyPos = CInt(ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.REAGENT2_ARM_VSEC.ToString, GlobalEnumerates.AXIS.Z, True).Value)
                            myScreenDelegate.R2WSHorizontalPos = CInt(ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.REAGENT2_ARM_WASH.ToString, GlobalEnumerates.AXIS.POLAR, True).Value)

                            ' XBC 19/04/2012 - Change maneuver
                            myScreenDelegate.R2InitialPointLevelDet = CInt(ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.REAGENT2_ARM_LEVEL.ToString, GlobalEnumerates.AXIS.Z, True).Value)
                    End Select
                End If


                MyClass.NeedlesArmsTempering()

            End If



        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".Tab2MeasureButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".Tab2MeasureButton_Click ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub Tab2AdjustButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Tab2AdjustButton.Click
        Dim myGlobal As New GlobalDataTO
        Try
            myGlobal = MyBase.Test
            If myGlobal.HasError Then
                PrepareErrorMode()
            Else
                PrepareArea()

                ' Initializations
                myScreenDelegate.CurrentOperation = ThermosAdjustmentsDelegate.OPERATIONS.TEST_ADJUSTMENT
                myScreenDelegate.ReagentNeedleTestAdjustmentDone = False
                myScreenDelegate.CurrentTest = Me.EditedValue.AdjustmentID

                Me.EditedValue.SetPointNewValue = Me.EditedValue.SetPointCurrentValue + Me.EditedValue.NewCorrection
                myScreenDelegate.ReagentNeedleTCO = Me.EditedValue.SetPointNewValue.ToString("0.0").Replace(MyClass.myDecimalSeparator.ToString, ",")

                MyBase.DisplayMessage(Messages.SRV_TEST_IN_PROCESS.ToString)

                If Not myGlobal.HasError Then
                    If MyBase.SimulationMode Then
                        ' simulating
                        'MyBase.DisplaySimulationMessage("Doing specified Test...")
                        'Me.Cursor = Cursors.WaitCursor
                        System.Threading.Thread.Sleep(SimulationProcessTime)
                        MyBase.myServiceMDI.Focus()
                        'Me.Cursor = Cursors.Default
                        MyBase.CurrentMode = ADJUSTMENT_MODES.TESTED
                        myScreenDelegate.ReagentNeedleTestAdjustmentDone = True
                        PrepareArea()
                    Else
                        ' Manage FwScripts must to be sent to testing
                        SendFwScript(Me.CurrentMode)
                    End If
                End If

            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".Tab2AdjustButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".Tab2AdjustButton_Click ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub Tab2TextBoxTemp_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Tab2TextBoxTemp.TextChanged
        Try
            If Me.PageInitialized Then

                'If Me.EditionMode Then
                If Me.Tab2TextBoxTemp.Text.Length > 0 Then
                    If IsNumeric(Me.Tab2TextBoxTemp.Text) Then
                        ValidateToCalulateNEEDLESTemp()
                    End If
                    '    Me.Tab2TextBoxTemp.BackColor = Color.White
                    'Else
                    '    Me.Tab2TextBoxTemp.BackColor = Color.Khaki
                End If
                'End If

                Me.EditingRotorTemperatures = False
                Me.EditingRotorCorrection = False
                Me.EditingNeedleTemperature = True
                Me.EditingNeedleCorrection = False
                Me.EditingHeaterTemperature = False
                Me.EditingHeaterCorrection = False
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".Tab2TextBoxTemp_TextChanged ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".Tab2TextBoxTemp_TextChanged ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub Tab2CorrectionTextBox_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles Tab2CorrectionTextBox.KeyPress
        Try
            If e.KeyChar = Chr(Keys.Enter) Then
                Me.Tab2AdjustButton.Focus()
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".Tab2CorrectionTextBox_KeyPress ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".Tab2CorrectionTextBox_KeyPress ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub Tab2CorrectionTextBox_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Tab2CorrectionTextBox.TextChanged
        'Dim Utilities As New Utilities
        Try
            If Me.PageInitialized Then

                'Me.Tab2CorrectionTextBox.Text.Replace(",", ".")

                If Me.Tab2CorrectionTextBox.Text.Length = 0 Then
                    Me.Tab2AdjustButton.Enabled = False
                    Me.BsSaveButton.Enabled = False
                Else
                    If IsNumeric(Me.Tab2CorrectionTextBox.Text) Then
                        ' XBC 18/11/2011 - correction singles
                        'Me.EditedValue.NewCorrection = CSng(Me.Tab2CorrectionTextBox.Text.Replace(".", MyClass.myDecimalSeparator.ToString))
                        Me.EditedValue.NewCorrection = Utilities.FormatToSingle(Me.Tab2CorrectionTextBox.Text)
                    End If

                    'If myScreenDelegate.ReagentNeedleMeasuredTempDone Then
                    Me.Tab2AdjustButton.Enabled = True
                    ''Me.BsSaveButton.Enabled = True
                    'End If
                End If

                Me.EditingRotorTemperatures = False
                Me.EditingRotorCorrection = False
                Me.EditingNeedleTemperature = False
                Me.EditingNeedleCorrection = True
                Me.EditingHeaterTemperature = False
                Me.EditingHeaterCorrection = False
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".Tab2CorrectionTextBox_TextChanged ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".Tab2CorrectionTextBox_TextChanged ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub IThermosAdjustments_Shown(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Shown
        Try

            Me.BsInfoWsHeaterXPSViewer.Visible = True
            Me.BsInfoRotorXPSViewer.Visible = True
            Me.BsInfoNeedlesXPSViewer.Visible = True

            Me.BsInfoWsHeaterXPSViewer.RefreshPage()
            Me.BsInfoRotorXPSViewer.RefreshPage()
            Me.BsInfoNeedlesXPSViewer.RefreshPage()

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".IThermosAdjustments_Shown ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".IThermosAdjustments_Shown ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub


    '''' <summary>
    '''' Needle arm to parking/to washing
    '''' </summary>
    '''' <param name="sender"></param>
    '''' <param name="e"></param>
    '''' <remarks>Created by SGM 01/12/2011</remarks>
    'Private Sub Tab2AuxButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Tab2AuxButton.Click
    '    Try

    '        If Not IsNeedleArmOutOfWashing Then
    '            MyClass.CurrentModeBeforeNeedlesArmsOut = MyBase.CurrentMode
    '            MyClass.myScreenDelegate.CurrentOperation = ThermosAdjustmentsDelegate.OPERATIONS.NEEDLE_TO_PARKING
    '            MyBase.CurrentMode = ADJUSTMENT_MODES.THERMO_ARM_TO_PARKING
    '            MyClass.PrepareArea()
    '        Else
    '            MyClass.myScreenDelegate.CurrentOperation = ThermosAdjustmentsDelegate.OPERATIONS.NEEDLE_BACK_TO_WASHING
    '            MyBase.CurrentMode = ADJUSTMENT_MODES.THERMO_ARM_TO_WASHING
    '            MyClass.PrepareArea()
    '        End If

    '        If MyBase.SimulationMode Then

    '            System.Threading.Thread.Sleep(5 * MyBase.SimulationProcessTime)

    '            If Not IsNeedleArmOutOfWashing Then
    '                MyBase.CurrentMode = ADJUSTMENT_MODES.THERMO_ARM_IN_PARKING
    '                MyClass.PrepareArea()
    '            Else
    '                System.Threading.Thread.Sleep(5 * MyBase.SimulationProcessTime)
    '                MyBase.CurrentMode = ADJUSTMENT_MODES.THERMO_ARM_IN_WASHING
    '                MyClass.PrepareArea()
    '            End If

    '        Else

    '            SendFwScript(Me.CurrentMode)

    '        End If
    '    Catch ex As Exception
    '        GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".Tab2AuxButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
    '        MyBase.ShowMessage(Me.Name & ".Tab2AuxButton_Click ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
    '    End Try
    'End Sub

    'Private Sub WaitProgressBar_IsTimeForWaitElapsed(ByVal sender As System.Object) Handles WaitProgressBar.IsTimeForWaitElapsed
    '    Try
    '        If MyBase.SimulationMode Then
    '            Select Case MyBase.CurrentMode

    '                Case ADJUSTMENT_MODES.PRIME_NEEDLE
    '                    MyBase.CurrentMode = ADJUSTMENT_MODES.PRIME_NEEDLE_DONE

    '                Case ADJUSTMENT_MODES.DISPENSE_WS
    '                    MyBase.CurrentMode = ADJUSTMENT_MODES.DISPENSE_WS_DONE
    '                    MyClass.SimulateReagentArmsConditioning()
    '            End Select

    '            MyClass.PrepareArea()

    '        End If
    '    Catch ex As Exception
    '        GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".WaitProgressBar_IsTimeForWaitElapsed ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
    '        mybase.ShowMessage(Me.Name & ".WaitProgressBar_IsTimeForWaitElapsed ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
    '    End Try
    'End Sub
#End Region

#Region "TAB HEATER"

    Private Sub Tab3ConditioningButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Tab3ConditioningButton.Click
        Dim myGlobal As New GlobalDataTO
        Try
            myGlobal = MyBase.Test
            If myGlobal.HasError Then
                PrepareErrorMode()
            Else

                'PrepareArea()

                ' Initializations
                'Me.EditionMode = True
                Me.LastConditioningDone = LastConditioningDoneEnum.ConditioningHeater
                myScreenDelegate.CurrentOperation = ThermosAdjustmentsDelegate.OPERATIONS.CONDITIONING_HEATER
                myScreenDelegate.HeaterConditioningDone = False
                myScreenDelegate.HeaterConditioningNumDisp = 1
                myScreenDelegate.CurrentTest = ADJUSTMENT_GROUPS.THERMOS_WS_HEATER
                Me.ProgressBar1.Maximum = myScreenDelegate.HeaterMaxActionsTemper
                Me.ProgressBar1.Visible = True
                Me.ProgressBar1.Value = 1

                MyClass.PrepareArea()

                'initialize HeaterConditioningDoing
                myScreenDelegate.HeaterConditioningDoing = New List(Of Boolean)
                For i As Integer = 0 To myScreenDelegate.HeaterMaxActionsTemper - 1
                    myScreenDelegate.HeaterConditioningDoing.Add(False)
                Next

                MyBase.DisplayMessage(Messages.SRV_CONDITIONING.ToString)

                If Not myGlobal.HasError Then
                    If MyBase.SimulationMode Then

                        Me.ProgressBar1.Value = 0
                        Me.ProgressBar1.Maximum = myScreenDelegate.HeaterMaxActionsTemper
                        Me.ProgressBar1.Visible = True
                        Me.ProgressBar1.Refresh()

                        For P As Integer = 1 To myScreenDelegate.HeaterMaxActionsTemper Step 1
                            System.Threading.Thread.Sleep(1000)
                            myScreenDelegate.HeaterConditioningDoing(P - 1) = True
                            Me.ProgressBar1.Value = P
                            Me.ProgressBar1.Refresh()
                        Next

                        Me.ProgressBar1.Visible = False
                        Me.ProgressBar1.Refresh()

                        myScreenDelegate.HeaterConditioningDone = True

                        MyBase.CurrentMode = ADJUSTMENT_MODES.TESTED
                        MyClass.PrepareArea()
                    Else

                        myScreenDelegate.WSReadyRefPos = CInt(ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.WASHING_STATION.ToString, GlobalEnumerates.AXIS.REL_Z, True).Value)
                        myScreenDelegate.WSFinalPos = CInt(ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.WASHING_STATION.ToString, GlobalEnumerates.AXIS.Z, True).Value)
                        myScreenDelegate.WSReferencePos = CInt(ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.WASHING_STATION_PARK.ToString, GlobalEnumerates.AXIS.Z, True).Value)

                        ' Manage FwScripts must to be sent to send Conditioning instructions to Instrument
                        SendFwScript(Me.CurrentMode)
                    End If
                End If
            End If

        Catch ex As Exception
            MyClass.myScreenDelegate.CurrentOperation = ThermosAdjustmentsDelegate.OPERATIONS._NONE
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".Tab3ConditioningButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".Tab3ConditioningButton_Click ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub Tab3AdjustButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Tab3AdjustButton.Click
        Dim myGlobal As New GlobalDataTO
        Try
            myGlobal = MyBase.Test
            If myGlobal.HasError Then
                PrepareErrorMode()
            Else
                PrepareArea()

                ' Initializations
                myScreenDelegate.CurrentOperation = ThermosAdjustmentsDelegate.OPERATIONS.TEST_ADJUSTMENT
                myScreenDelegate.HeaterTestAdjustmentDone = False
                myScreenDelegate.CurrentTest = ADJUSTMENT_GROUPS.THERMOS_WS_HEATER

                Me.EditedValue.SetPointNewValue = Me.EditedValue.SetPointCurrentValue + Me.EditedValue.NewCorrection
                myScreenDelegate.HeaterTCO = Me.EditedValue.SetPointNewValue.ToString("0.0").Replace(MyClass.myDecimalSeparator.ToString, ",")

                MyBase.DisplayMessage(Messages.SRV_TEST_IN_PROCESS.ToString)

                If Not myGlobal.HasError Then
                    If MyBase.SimulationMode Then
                        ' simulating
                        'MyBase.DisplaySimulationMessage("Doing specified Test...")
                        'Me.Cursor = Cursors.WaitCursor
                        System.Threading.Thread.Sleep(SimulationProcessTime)
                        MyBase.myServiceMDI.Focus()
                        'Me.Cursor = Cursors.Default
                        MyBase.CurrentMode = ADJUSTMENT_MODES.TESTED
                        myScreenDelegate.HeaterTestAdjustmentDone = True
                        PrepareArea()
                    Else
                        ' Manage FwScripts must to be sent to testing
                        SendFwScript(Me.CurrentMode)
                    End If
                End If

            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".Tab3AdjustButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".Tab3AdjustButton_Click ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try

    End Sub

    Private Sub Tab3TextBoxTemp_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Tab3TextBoxTemp.TextChanged
        Try
            If Me.PageInitialized Then

                'If Me.EditionMode Then
                If Me.Tab3TextBoxTemp.Text.Length > 0 Then
                    If IsNumeric(Me.Tab3TextBoxTemp.Text) Then
                        ValidateToCalulateHEATERTemp()
                    End If
                    '    Me.Tab3TextBoxTemp.BackColor = Color.White
                    'Else
                    '    Me.Tab3TextBoxTemp.BackColor = Color.Khaki
                End If
                'End If

                Me.EditingRotorTemperatures = False
                Me.EditingRotorCorrection = False
                Me.EditingNeedleTemperature = False
                Me.EditingNeedleCorrection = False
                Me.EditingHeaterTemperature = True
                Me.EditingHeaterCorrection = False
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".Tab3TextBoxTemp_TextChanged ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".Tab3TextBoxTemp_TextChanged ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub Tab3CorrectionTextBox_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Tab3CorrectionTextBox.TextChanged
        'Dim Utilities As New Utilities
        Try
            If Me.PageInitialized Then

                If Me.Tab3CorrectionTextBox.Text.Length = 0 Then
                    Me.Tab3AdjustButton.Enabled = False
                    Me.BsSaveButton.Enabled = False
                Else
                    If IsNumeric(Me.Tab3CorrectionTextBox.Text) Then
                        ' XBC 18/11/2011 - correction singles
                        'Me.EditedValue.NewCorrection = CSng(Me.Tab3CorrectionTextBox.Text.Replace(".", MyClass.myDecimalSeparator.ToString))
                        Me.EditedValue.NewCorrection = Utilities.FormatToSingle(Me.Tab3CorrectionTextBox.Text)
                    End If

                    'If myScreenDelegate.HeaterConditioningDone Then
                    Me.Tab3AdjustButton.Enabled = True
                    'Me.BsSaveButton.Enabled = True
                    'End If
                End If

                Me.EditingRotorTemperatures = False
                Me.EditingRotorCorrection = False
                Me.EditingNeedleTemperature = False
                Me.EditingNeedleCorrection = False
                Me.EditingHeaterTemperature = False
                Me.EditingHeaterCorrection = True
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".Tab3CorrectionTextBox_TextChanged ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".Tab3CorrectionTextBox_TextChanged ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub BsUpDownWSButton1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsUpDownWSButton1.Click
        Try
            myScreenDelegate.CurrentTest = ADJUSTMENT_GROUPS.THERMOS_WS_HEATER
            Me.SendWASH_STATION_CTRL()

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".BsUpDownWSButton1_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".BsUpDownWSButton1_Click ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub
#End Region

#End Region

#Region "XPS Info Events"

    Private Sub BsXPSViewer_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles BsInfoWsHeaterXPSViewer.Load, _
                                                                                            BsInfoRotorXPSViewer.Load, _
                                                                                            BsInfoNeedlesXPSViewer.Load
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
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".BsXPSViewer_Load ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".BsXPSViewer_Load ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

#End Region

#Region "Simulation mode"

    Private Sub SimulationTimer_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SimulationTimer.Tick
        Try
            Me.SimulationTimer.Enabled = False

            If MyBase.SimulationMode Then
                If MyBase.CurrentMode = ADJUSTMENT_MODES.TESTING Then
                    MyBase.CurrentMode = ADJUSTMENT_MODES.TESTED
                    MyClass.PrepareArea()
                End If
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".SimulationTimer_Tick ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".SimulationTimer_Tick ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

#End Region

End Class
