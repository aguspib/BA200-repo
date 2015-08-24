Option Explicit On
Option Strict On

Imports System.IO
Imports System.Threading
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Global.TO
Imports Biosystems.Ax00.Global.GlobalEnumerates
Imports Biosystems.Ax00.FwScriptsManagement
Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.Controls.UserControls
Imports DevExpress.XtraCharts
Imports Biosystems.Ax00.App
Imports DevExpress.Utils

Public Class UiPositionsAdjustments
    Inherits BSAdjustmentBaseForm


#Region "Declarations"

    'Screen's business delegate
    Private WithEvents myScreenDelegate As PositionsAdjustmentDelegate

    Private SelectedPage As ADJUSTMENT_PAGES = ADJUSTMENT_PAGES.OPTIC_CENTERING
    Private SelectedArmTab As ADJUSTMENT_ARMS = ADJUSTMENT_ARMS.SAMPLE
    Private SelectedAdjustmentGroup As ADJUSTMENT_GROUPS = ADJUSTMENT_GROUPS.PHOTOMETRY
    Private SelectedAdjustmentsDS As New SRVAdjustmentsDS 'SGM 28/01/11
    Private LocalSavedAdjustmentsDS As New SRVAdjustmentsDS 'XBC 01/02/11
    Private TempToSendAdjustmentsDelegate As FwAdjustmentsDelegate 'XBC 01/02/11
    Private SelectedRow As Integer

    ' Value ranges
    Private LedCurrentMin As Integer
    Private LedCurrentMax As Integer

    Private LimitMinPolar As Single
    Private LimitMaxPolar As Single
    Private LimitMinZ As Single
    Private LimitMaxZ As Single
    Private LimitMinRotor As Single
    Private LimitMaxRotor As Single

    ' Z Fine limit - below of which all brusque movements are disabled (is just allowed the step-by-step movements)
    Private SampleLimitZFine As Single
    Private Reagent1LimitZFine As Single
    Private Reagent2LimitZFine As Single
    Private Mixer1LimitZFine As Single
    Private Mixer2LimitZFine As Single

    'Button states before Stirrertest
    Private OkButtonEnabledBeforeStirrerTest As Boolean = False
    Private CancelButtonEnabledBeforeStirrerTest As Boolean = False

    ' Edited value
    Private EditedValue As EditedValueStruct

    Private FocusedFromGrid As Boolean = False 'SG 31/01/11

    'optic centering
    'Private ScannedAbsorbanceSteps As Integer = 0 'SG 01/02/11
    Private AbsorbanceData As List(Of Single) 'SG 01/02/11
    Private EncoderData As List(Of Single)  ' XBC 21/12/2011

    ' XBC 02/01/2012 - Add Encoder functionality
    Private WellCenters As List(Of Single)
    Private EncoderTransitions As List(Of Single)
    Private CenterDistances As List(Of Single)
    ' XBC 02/01/2012 - Add Encoder functionality
    Private LEDCurrent As Integer 'SGM 28/04/2011 

    Private CurrentArmPositionsAdjustmentsDS As New SRVAdjustmentsDS

    Private Structure EditedValueStruct
        ' Adjustment
        Public AdjustmentID As ADJUSTMENT_GROUPS
        Public AxisID As GlobalEnumerates.AXIS
        ' Saved values
        Public LastPolarValue As Single
        Public LastZValue As Single
        Public LastRotorValue As Single
        ' XBC 03/01/2012 - Add Encoder functionality
        Public LastEncoderValue As Single

        ' Current values
        Public CurrentPolarValue As Single
        Public CurrentZValue As Single
        Public CurrentRotorValue As Single
        ' XBC 03/01/2012 - Add Encoder functionality

        ' New values
        Public NewPolarValue As Single
        Public NewZValue As Single
        Public NewRotorValue As Single
        ' XBC 03/01/2012 - Add Encoder functionality
        Public NewEncoderValue As Single

        'can move values
        Public canMovePolarValue As Boolean
        Public canMoveZValue As Boolean
        Public canMoveRotorValue As Boolean

        'can save values
        Public canSavePolarValue As Boolean
        Public canSaveZValue As Boolean
        Public canSaveRotorValue As Boolean

        'steps
        Public stepValue As Single

    End Structure

    'SGM 28/01/11
    Private Structure AdjustmentRowData

        Public CodeFw As String
        Public Value As String
        Public CanSave As Boolean
        Public CanMove As Boolean
        Public GroupID As String


        Public Sub New(ByVal pCodeFw As String)
            CodeFw = pCodeFw
            Value = ""
            CanSave = False
            CanMove = False
            GroupID = ""
        End Sub

    End Structure

    ' Homes
    Private HomePolar As Single
    Private HomeZ As Single
    Private HomeRotor As Single

    ' Language
    Private currentLanguage As String

    ' XBC 16/11/2011
    Private IsWaitingExitTest As Boolean

    ' XBC 30/11/2011
    Private IsReadingCountsAbortedByUser As Boolean

    ' XBC 03/01/2012
    Private LimitMinEncoder As Single
    Private LimitMaxEncoder As Single

    ' XB 04/12/2013
    Private EncoderOutOfRange As Boolean

#End Region

#Region "Constants"
    ' Constant Value Columns
    Const POLAR_COLUMN As Integer = 0
    Const Z_COLUMN As Integer = 1
    Const ROTOR_COLUMN As Integer = 2

    ' Constant Value Specified Rows
    Const ZREF_SAMPLE_ROW As Integer = 2
    Const ZTUBE_SAMPLE_ROW As Integer = 7
    Const ZREF_REAGENT_ROW As Integer = 1
    Const ZREF_MIXER_ROW As Integer = 1
    ' XBC 28/03/2012 -> Change to variable (AC)
    Private ISE_SAMPLE_ROW As Integer = 8
#End Region

#Region "Variables"
    Private WizardMode As Boolean
    Private ManageTabArms As Boolean
    Private ManageTabPages As Boolean
    ' XBC 07/12/2011
    Private WaitForScriptsExitingScreen As Boolean
    ' XBC 21/05/2012
    Public IsCenteringOptic As Boolean
#End Region

#Region "Properties"

    Public ReadOnly Property IsReadyToClose() As Boolean
        Get
            Return IsReadyToCloseAttr
        End Get
    End Property

    'SGM 24/01/11
    Private Property ChangedValue() As Boolean
        Get
            Return ChangedValueAttr
        End Get
        Set(ByVal value As Boolean)

            If ChangedValueAttr <> value Then
                ChangedValueAttr = value
            End If
        End Set
    End Property

    'SGM 28/02/2012
    Private Property IsLocallySaved() As Boolean
        Get
            Return IsLocallySavedAttr
        End Get
        Set(ByVal value As Boolean)
            IsLocallySavedAttr = value
        End Set
    End Property

    'modifications relationships
    Private Property OpticCenteringModified() As Boolean
        Get
            Return OpticCenteringModifiedAttr
        End Get
        Set(ByVal value As Boolean)
            If value Then
                WashingStationValidated = False
                BsGridSample.SetAllRowDataInvalidated()
                BsGridReagent1.SetAllRowDataInvalidated()
                BsGridReagent2.SetAllRowDataInvalidated()
                BsGridMixer1.SetAllRowDataInvalidated()
                BsGridMixer2.SetAllRowDataInvalidated()
                AllArmsPositionsValidated = False
            End If
            OpticCenteringModifiedAttr = value
        End Set
    End Property
    Private Property WashingStationValidated() As Boolean
        Get
            Return WashingStationValidatedAttr
        End Get
        Set(ByVal value As Boolean)
            WashingStationValidatedAttr = value
        End Set
    End Property
    Private Property AllArmsPositionsValidated() As Boolean
        Get
            If OpticCenteringModified Then
                Dim all As Boolean = True
                all = all And Me.BsGridSample.AreAllRowDataValidated
                all = all And Me.BsGridReagent1.AreAllRowDataValidated
                all = all And Me.BsGridReagent2.AreAllRowDataValidated
                all = all And Me.BsGridMixer1.AreAllRowDataValidated
                all = all And Me.BsGridMixer2.AreAllRowDataValidated

                AllArmsPositionsValidatedAttr = all
            End If

            Return AllArmsPositionsValidatedAttr

        End Get
        Set(ByVal value As Boolean)
            If Not value Then
                BsGridSample.SetAllRowDataInvalidated()
                BsGridReagent1.SetAllRowDataInvalidated()
                BsGridReagent2.SetAllRowDataInvalidated()
                BsGridMixer1.SetAllRowDataInvalidated()
                BsGridMixer2.SetAllRowDataInvalidated()
            End If
            AllArmsPositionsValidatedAttr = value
        End Set
    End Property

    Private ReadOnly Property SomeArmPositionModified() As Boolean
        Get
            Dim some As Boolean = False
            some = some Or Me.BsGridSample.IsAnyRowDataValidated
            some = some Or Me.BsGridReagent1.IsAnyRowDataValidated
            some = some Or Me.BsGridReagent2.IsAnyRowDataValidated
            some = some Or Me.BsGridMixer1.IsAnyRowDataValidated
            some = some Or Me.BsGridMixer2.IsAnyRowDataValidated

            SomeArmPositionModifiedAttr = some

            Return SomeArmPositionModifiedAttr
        End Get
    End Property

    Private Property AllHomesAreDone() As Boolean
        Get
            If myScreenDelegate IsNot Nothing Then
                Dim myGlobal = myScreenDelegate.GetPendingPreliminaryHomes(SelectedAdjustmentGroup)
                If Not myGlobal.HasError And myGlobal IsNot Nothing Then
                    Dim myPendingList = CType(myGlobal.SetDatos, List(Of SRVPreliminaryHomesDS.srv_tadjPreliminaryHomesRow))
                    AllHomesAreDoneAttr = (myPendingList.Count = 0)
                Else
                    AllHomesAreDoneAttr = False
                End If
            Else
                AllHomesAreDoneAttr = False
            End If

            Return AllHomesAreDoneAttr

        End Get
        Set(ByVal value As Boolean)
            AllHomesAreDoneAttr = value
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

    Private Property IsStirrerTesting() As Boolean
        Get
            Return IsStirrerTestingAttr
        End Get
        Set(ByVal value As Boolean)
            If IsStirrerTestingAttr <> value Then
                IsStirrerTestingAttr = value
                MyBase.ActivateMDIMenusButtons(Not value) 'SGM 28/09/2011
            End If
        End Set
    End Property
#End Region

#Region "Attributes"
    Private ChangedValueAttr As Boolean = False

    Private OpticCenteringModifiedAttr As Boolean = False 'SGM 04/02/11
    Private WashingStationValidatedAttr As Boolean = True 'SGM 04/02/11
    Private AllArmsPositionsValidatedAttr As Boolean = True 'SGM 04/02/11
    Private SomeArmPositionModifiedAttr As Boolean = True 'SGM 04/02/11

    Private AllHomesAreDoneAttr As Boolean = False 'SGM 04/02/11

    Private ActiveAnalyzerModelAttr As String

    Private IsStirrerTestingAttr As Boolean = False

    Private IsLocallySavedAttr As Boolean = False
    Private IsReadyToCloseAttr As Boolean = False
#End Region

#Region "Enumerations"
    Private Enum ADJUSTMENT_ARMS
        SAMPLE
        REAGENT1
        REAGENT2
        MIXER1
        MIXER2
    End Enum

    Private Enum ADJUSTMENT_PAGES
        NONE
        OPTIC_CENTERING
        WASHING_STATION
        ARM_POSITIONS
    End Enum

    Private Enum HISTORY_TASKS
        ADJUSTMENT = 1
        TEST = 2
    End Enum

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
    ''' Created by: SG 17/11/10
    ''' Modified by XB 15/10/2014 - Perform Fine Optical Centering - BA-2004
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
                        Application.DoEvents()
                        myGlobal = InitializeHomes() 'SGM 04/02/11
                        If Not myGlobal.HasError Then
                            PrepareArea()
                        Else
                            PrepareErrorMode()
                            Exit Function
                        End If
                    End If

                Case ADJUSTMENT_MODES.LOADED
                    If pResponse = RESPONSE_TYPES.OK Then
                        PrepareArea()
                    End If


                Case ADJUSTMENT_MODES.HOME_FINISHED
                    If pResponse = RESPONSE_TYPES.OK Then
                        myGlobal = myScreenDelegate.SetPreliminaryHomesAsDone(ADJUSTMENT_GROUPS.INTERNAL_TANKS)
                        If Not myGlobal.HasError Then
                            Me.AllHomesAreDone = True
                            MyBase.DisplayMessage(Messages.SRV_HOMES_FINISHED.ToString, Messages.SRV_HOMES_FINISHED.ToString)

                            PrepareArea()
                        Else
                            PrepareErrorMode()
                            Exit Function
                        End If
                    End If


                Case ADJUSTMENT_MODES.ADJUST_PREPARED
                    If pResponse = RESPONSE_TYPES.OK Then
                        If Not Me.AllHomesAreDone Then
                            If MyBase.SimulationMode Then
                                MyBase.myServiceMDI.Focus()
                                Me.Cursor = Cursors.WaitCursor
                                Thread.Sleep(SimulationProcessTime)
                                MyBase.myServiceMDI.Focus()
                                Me.Cursor = Cursors.Default
                                MyBase.myServiceMDI.Focus()
                                Me.AllHomesAreDone = True
                            Else
                                ' homes are done for current adjust
                                myGlobal = myScreenDelegate.SetPreliminaryHomesAsDone(Me.SelectedAdjustmentGroup)
                                If Not myGlobal.HasError Then
                                    Me.AllHomesAreDone = True
                                Else
                                    PrepareErrorMode()
                                    Exit Function
                                End If
                            End If
                            MyBase.DisplayMessage(Messages.SRV_HOMES_FINISHED.ToString, Messages.SRV_ADJUSTMENTS_READY.ToString)
                        Else
                            MyBase.DisplayMessage(Messages.SRV_ADJUSTMENTS_READY.ToString)
                        End If
                        PrepareArea()
                    End If

                Case ADJUSTMENT_MODES.ABSORBANCE_SCANNED
                    If pResponse = RESPONSE_TYPES.OK Then
                        PrepareArea()
                    Else
                        PrepareErrorMode()
                    End If

                Case ADJUSTMENT_MODES.ABSORBANCE_PREPARED
                    If pResponse = RESPONSE_TYPES.OK Then
                        PrepareArea()
                    Else
                        PrepareErrorMode()
                    End If

                Case ADJUSTMENT_MODES.ADJUSTED
                    If pResponse = RESPONSE_TYPES.OK Then
                        MyBase.DisplayMessage(Messages.SRV_COMPLETED.ToString)
                        PrepareArea()
                    End If

                Case ADJUSTMENT_MODES.SAVED
                    If pResponse = RESPONSE_TYPES.START Then
                        ' Nothing by now
                    End If
                    If pResponse = RESPONSE_TYPES.OK Then
                        If MyClass.IsLocallySaved Then
                            MyBase.DisplayMessage(Messages.SRV_ADJUSTMENTS_LOCAL_SAVED.ToString)
                            myScreenDelegate.ParkDone = True
                        Else
                            If Not myScreenDelegate.ParkDone Then
                                If Me.SelectedPage = ADJUSTMENT_PAGES.ARM_POSITIONS Then
                                    MyBase.DisplayMessage(Messages.SRV_ARMS_TO_PARKING.ToString)
                                End If
                            Else
                                MyBase.DisplayMessage(Messages.SRV_ADJUSTMENTS_SAVED.ToString)
                            End If

                        End If
                        PrepareArea()
                    End If

                Case ADJUSTMENT_MODES.TESTED
                    If pResponse = RESPONSE_TYPES.OK Then
                        If Not Me.AllHomesAreDone Then
                            If MyBase.SimulationMode Then
                                MyBase.myServiceMDI.Focus()
                                Me.Cursor = Cursors.WaitCursor
                                Thread.Sleep(SimulationProcessTime)
                                MyBase.myServiceMDI.Focus()
                                Me.Cursor = Cursors.Default
                                MyBase.myServiceMDI.Focus()
                                Me.AllHomesAreDone = True
                            Else
                                ' homes are done for current adjust
                                myGlobal = myScreenDelegate.SetPreliminaryHomesAsDone(Me.SelectedAdjustmentGroup)
                                If Not myGlobal.HasError Then
                                    Me.AllHomesAreDone = True
                                Else
                                    PrepareErrorMode()
                                    Exit Function
                                End If
                            End If
                            MyBase.DisplayMessage(Messages.SRV_HOMES_FINISHED.ToString, Messages.SRV_TEST_COMPLETED.ToString)
                        Else
                            MyBase.DisplayMessage(Messages.SRV_TEST_COMPLETED.ToString)
                        End If
                        PrepareArea()
                    End If

                Case ADJUSTMENT_MODES.TEST_EXITED
                    If pResponse = RESPONSE_TYPES.OK Then
                        MyBase.DisplayMessage(Messages.SRV_TEST_EXIT_COMPLETED.ToString)
                        PrepareArea()
                    End If

                Case ADJUSTMENT_MODES.STIRRER_TESTING
                    If pResponse = RESPONSE_TYPES.OK Then
                        MyBase.DisplayMessage(Messages.SRV_MIXER_TESTING.ToString)
                        PrepareArea()
                    End If

                Case ADJUSTMENT_MODES.STIRRER_TESTED
                    If pResponse = RESPONSE_TYPES.OK Then
                        PrepareArea()
                    End If

                    ' XBC 30/11/2011
                Case ADJUSTMENT_MODES.PARKED
                    If pResponse = RESPONSE_TYPES.OK Then
                        PrepareArea()
                    End If

                    ' XB 15/10/2014 - BA-2004
                Case ADJUSTMENT_MODES.FINE_OPTICAL_CENTERING_DONE
                    If pResponse = RESPONSE_TYPES.OK Then
                        PrepareArea()
                    End If

                Case ADJUSTMENT_MODES.ERROR_MODE
                    MyBase.DisplayMessage(Messages.FWSCRIPT_DATA_ERROR.ToString)
                    PrepareErrorMode()
            End Select

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".ScreenReceptionLastFwScriptEvent ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".ScreenReceptionLastFwScriptEvent", Messages.SYSTEM_ERROR.ToString, myGlobal.ErrorMessage, Me)
        End Try

        Return True
    End Function



#End Region

#Region "Public Methods"
    ''' <summary>
    ''' Refresh this specified screen with the information received from the Instrument
    ''' </summary>
    ''' <param name="pRefreshEventType"></param>
    ''' <param name="pRefreshDs"></param>
    ''' <remarks>Created by XBC 05/12/2011</remarks>
    Public Overrides Sub RefreshScreen(ByVal pRefreshEventType As List(Of UI_RefreshEvents), ByVal pRefreshDs As UIRefreshDS)
        'Dim myGlobal As New GlobalDataTO
        Try
            myScreenDelegate.RefreshDelegate(pRefreshEventType, pRefreshDS)
            'if needed manage the event in the Base Form
            MyBase.OnReceptionLastFwScriptEvent(RESPONSE_TYPES.OK, Nothing)
            PrepareArea()

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".RefreshScreen ", EventLogEntryType.Error, _
                                                                    GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".RefreshScreen", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

#Region "Must Inherited"

    ''' <summary>
    ''' Prepare GUI for Error Mode
    ''' </summary>
    ''' <remarks>Created by XBC 14/01/2011</remarks>
    Public Overrides Sub PrepareErrorMode(Optional ByVal pAlarmType As ManagementAlarmTypes = ManagementAlarmTypes.NONE)
        Try
            Me.Cursor = Cursors.Default

            MyBase.ErrorMode()
            Me.ProgressBar1.Visible = False
            DisableAll()
            Me.BsExitButton.Enabled = True ' Just Exit button is enabled in error case
            Me.BsOpticStopButton.Enabled = False
            MyBase.ActivateMDIMenusButtons(True) 'SGM 28/09/2011

            'report results to history 
            Me.ReportHistoryError()
            Me.IsCenteringOptic = False

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareErrorMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareErrorMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
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
    Public Overrides Sub StopCurrentOperation(Optional ByVal pAlarmType As ManagementAlarmTypes = ManagementAlarmTypes.NONE)
        Try
            ' Stop FwScripts
            myFwScriptDelegate.StopFwScriptQueue()

            Select Case MyBase.CurrentMode
                Case ADJUSTMENT_MODES.ADJUSTMENTS_READING
                    ' No additional treatment to do

                Case ADJUSTMENT_MODES.ADJUST_PREPARING

                    Select Case Me.SelectedPage
                        Case ADJUSTMENT_PAGES.OPTIC_CENTERING
                            Me.BsOpticAdjustButton.Visible = True
                            Me.BsOpticStopButton.Visible = False
                            Me.IsReadingCountsAbortedByUser = True

                        Case ADJUSTMENT_PAGES.WASHING_STATION, _
                            ADJUSTMENT_PAGES.ARM_POSITIONS
                            ' No additional treatment to do

                    End Select

                Case ADJUSTMENT_MODES.ABSORBANCE_SCANNING, ADJUSTMENT_MODES.ABSORBANCE_PREPARING

                    Me.BsOpticAdjustButton.Visible = True
                    Me.BsOpticStopButton.Visible = False
                    Me.IsReadingCountsAbortedByUser = True

                Case ADJUSTMENT_MODES.ADJUSTING

                    Select Case SelectedPage
                        Case ADJUSTMENT_PAGES.OPTIC_CENTERING, _
                            ADJUSTMENT_PAGES.WASHING_STATION,
                            ADJUSTMENT_PAGES.ARM_POSITIONS
                            Me.BsSaveButton.Enabled = False

                    End Select

                Case ADJUSTMENT_MODES.ADJUST_EXITING, _
                    ADJUSTMENT_MODES.TESTING
                    ' No additional treatment to do

                Case ADJUSTMENT_MODES.TEST_EXITING

                    Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate
                    ActivateCheckButton(Me.SelectedRow, False)
                    Select Case SelectedArmTab
                        Case ADJUSTMENT_ARMS.SAMPLE
                            Me.BsGridSample.NameColumnButton2ByRow(Me.SelectedRow) = myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_SRV_TEST", currentLanguage)
                        Case ADJUSTMENT_ARMS.REAGENT1
                            Me.BsGridReagent1.NameColumnButton2ByRow(Me.SelectedRow) = myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_SRV_TEST", currentLanguage)
                        Case ADJUSTMENT_ARMS.REAGENT2
                            Me.BsGridReagent2.NameColumnButton2ByRow(Me.SelectedRow) = myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_SRV_TEST", currentLanguage)
                        Case ADJUSTMENT_ARMS.MIXER1
                            Me.BsGridMixer1.NameColumnButton2ByRow(Me.SelectedRow) = myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_SRV_TEST", currentLanguage)
                        Case ADJUSTMENT_ARMS.MIXER2
                            Me.BsGridMixer2.NameColumnButton2ByRow(Me.SelectedRow) = myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_SRV_TEST", currentLanguage)
                    End Select

                Case ADJUSTMENT_MODES.SAVING, _
                    ADJUSTMENT_MODES.STIRRER_TEST, _
                    ADJUSTMENT_MODES.STIRRER_TESTING, _
                    ADJUSTMENT_MODES.PARKING
                    ' No additional treatment to do

            End Select

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

#Region "Arms Position Private Methods"
    ''' <summary>
    ''' Determines the layout of the Arms Positions Grids
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>Creatred by SG 28/02/2012</remarks>
    Private Function PrepareTabsArms() As GlobalDataTO
        Dim myResultData As New GlobalDataTO
        Try
            Me.ChangedValue = False

            If Not myResultData.HasError Then
                myResultData = MyClass.PrepareArmTab(Me.BsGridSample)
            End If
            If Not myResultData.HasError AndAlso Not AnalyzerController.Instance.IsBA200() Then
                myResultData = MyClass.PrepareArmTab(Me.BsGridReagent1)
                If Not myResultData.HasError Then
                    myResultData = MyClass.PrepareArmTab(Me.BsGridReagent2)
                End If
            End If
            If Not myResultData.HasError Then
                myResultData = MyClass.PrepareArmTab(Me.BsGridMixer1)
            End If
            If Not myResultData.HasError AndAlso Not AnalyzerController.Instance.IsBA200() Then
                myResultData = MyClass.PrepareArmTab(Me.BsGridMixer2)
            End If

        Catch ex As Exception
            myResultData.HasError = True
            myResultData.ErrorCode = Messages.SYSTEM_ERROR.ToString
            myResultData.ErrorMessage = ex.Message
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareTabsArms ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareTabsArms ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
        Return myResultData
    End Function

    ''' <summary>
    ''' Determines the layout of the Arms Positions Grid depending on the selected tab
    ''' </summary>
    ''' <param name="pBsGrid"></param>
    ''' <returns></returns>
    ''' <remarks>Creatred by SG 28/02/2012</remarks>
    Private Function PrepareArmTab(ByVal pBsGrid As BSGridControl) As GlobalDataTO
        Dim myResultData As New GlobalDataTO
        Try
            Dim myPreloadedMasterDataDS As New PreloadedMasterDataDS
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            myResultData = myScreenDelegate.ReadMovementsValues(PreloadedMasterDataEnum.SRV_ARM_MOVEMENTS)
            If (Not myResultData.HasError And Not myResultData.SetDatos Is Nothing) Then
                myPreloadedMasterDataDS = DirectCast(myResultData.SetDatos, PreloadedMasterDataDS)

                ' Cofiguring number of rows/columns of DataGrid
                Dim ds As DataSet
                Dim row As DataRow
                pBsGrid.numParams = myPreloadedMasterDataDS.tfmwPreloadedMasterData.Rows.Count
                ' configuring column names (every Form/Tag Form must configure this)
                pBsGrid.nameIdentificator = "ID"
                pBsGrid.nameColButton1 = "ACTION"
                pBsGrid.nameColImage = "VALIDATED"
                For i As Integer = 0 To myPreloadedMasterDataDS.tfmwPreloadedMasterData.Rows.Count - 1
                    pBsGrid.nameColsParams(i) = myPreloadedMasterDataDS.tfmwPreloadedMasterData(i).ItemID
                Next
                pBsGrid.nameColButton2 = "CHECK"

                ' initalizating datagrid with size values by default
                ds = pBsGrid.PrepareControls()

                ' Configure size columns
                pBsGrid.ColumnWidth(Me.BsGridSample.nameColButton1) = 110 '90
                For i As Integer = 0 To myPreloadedMasterDataDS.tfmwPreloadedMasterData.Rows.Count - 1
                    pBsGrid.ColumnWidth(Me.BsGridSample.nameColsParams(i)) = 160 '155
                Next
                pBsGrid.ColumnWidth(Me.BsGridSample.nameColButton2) = 65 '100

                If pBsGrid Is BsGridSample Then
                    myResultData = myScreenDelegate.ReadPositionsValues(PreloadedMasterDataEnum.SRV_SAMPLE_POSITIONS)
                ElseIf pBsGrid Is BsGridReagent1 Then
                    myResultData = myScreenDelegate.ReadPositionsValues(PreloadedMasterDataEnum.SRV_REAG_POSITIONS)
                ElseIf pBsGrid Is BsGridReagent2 Then
                    myResultData = myScreenDelegate.ReadPositionsValues(PreloadedMasterDataEnum.SRV_REAG_POSITIONS)
                ElseIf pBsGrid Is BsGridMixer1 Then
                    myResultData = myScreenDelegate.ReadPositionsValues(PreloadedMasterDataEnum.SRV_MIXER_POSITIONS)
                ElseIf pBsGrid Is BsGridMixer2 Then
                    myResultData = myScreenDelegate.ReadPositionsValues(PreloadedMasterDataEnum.SRV_MIXER_POSITIONS)
                End If

                If (Not myResultData.HasError And Not myResultData.SetDatos Is Nothing) Then
                    myPreloadedMasterDataDS = DirectCast(myResultData.SetDatos, PreloadedMasterDataDS)
                    For i As Integer = 0 To myPreloadedMasterDataDS.tfmwPreloadedMasterData.Rows.Count - 1
                        '// create new row
                        row = ds.Tables(0).NewRow()
                        row(pBsGrid.nameColButton1) = myPreloadedMasterDataDS.tfmwPreloadedMasterData(i).FixedItemDesc
                        row(pBsGrid.nameColButton2) = myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_SRV_TEST", currentLanguage)
                        row(pBsGrid.nameIdentificator) = myPreloadedMasterDataDS.tfmwPreloadedMasterData(i).ItemID
                        ds.Tables(0).Rows.Add(row)

                        pBsGrid.RowHeight(i) = 25

                    Next

                    ' populate content data to Grid Control
                    pBsGrid.PopulateGrid(ds)

                End If

            End If

        Catch ex As Exception
            myResultData.HasError = True
            myResultData.ErrorCode = Messages.SYSTEM_ERROR.ToString
            myResultData.ErrorMessage = ex.Message
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareArmTab ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareArmTab ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
        Return myResultData
    End Function
    ''' <summary>
    ''' Activates the Screen's check or test buttons
    ''' </summary>
    ''' <param name="pValue"></param>
    ''' <remarks>XBC 04/01/11</remarks>
    Private Sub ActivateCheckButtons(ByVal pValue As Boolean)
        Try

            Select Case SelectedArmTab
                Case ADJUSTMENT_ARMS.SAMPLE
                    If Not Me.BsGridSample Is Nothing AndAlso Me.BsGridSample.RowsCount > 0 Then
                        For i As Integer = 0 To Me.BsGridSample.RowsCount - 1
                            If Me.BsGridSample.EnableButton2(i) <> pValue Then
                                ActivateCheckButton(i, pValue)
                            End If
                        Next
                    End If
                Case ADJUSTMENT_ARMS.REAGENT1
                    If Not Me.BsGridReagent1 Is Nothing AndAlso Me.BsGridReagent1.RowsCount > 0 Then
                        For i As Integer = 0 To Me.BsGridReagent1.RowsCount - 1
                            If Me.BsGridReagent1.EnableButton2(i) <> pValue Then
                                ActivateCheckButton(i, pValue)
                            End If
                        Next
                    End If
                Case ADJUSTMENT_ARMS.REAGENT2
                    If Not Me.BsGridReagent2 Is Nothing AndAlso Me.BsGridReagent2.RowsCount > 0 Then
                        For i As Integer = 0 To Me.BsGridReagent2.RowsCount - 1
                            If Me.BsGridReagent2.EnableButton2(i) <> pValue Then
                                ActivateCheckButton(i, pValue)
                            End If
                        Next
                    End If
                Case ADJUSTMENT_ARMS.MIXER1
                    If Not Me.BsGridMixer1 Is Nothing AndAlso Me.BsGridMixer1.RowsCount > 0 Then
                        For i As Integer = 0 To Me.BsGridMixer1.RowsCount - 1
                            If Me.BsGridMixer1.EnableButton2(i) <> pValue Then
                                ActivateCheckButton(i, pValue)
                            End If
                        Next
                    End If
                Case ADJUSTMENT_ARMS.MIXER2
                    If Not Me.BsGridMixer2 Is Nothing AndAlso Me.BsGridMixer2.RowsCount > 0 Then
                        For i As Integer = 0 To Me.BsGridMixer2.RowsCount - 1
                            If Me.BsGridMixer2.EnableButton2(i) <> pValue Then
                                ActivateCheckButton(i, pValue)
                            End If
                        Next
                    End If
            End Select
            'End Select

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".ActivateCheckButtons ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".ActivateCheckButtons ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Activates the specific selected adjustment Arm Position grid row
    ''' </summary>
    ''' <param name="pRowIndex"></param>
    ''' <param name="pValue"></param>
    ''' <remarks>XBC 04/01/11</remarks>
    Private Sub ActivateRowCells(ByVal pRowIndex As Integer, ByVal pValue As Boolean)
        Try
            Select Case SelectedArmTab
                Case ADJUSTMENT_ARMS.SAMPLE
                    If Not Me.BsGridSample Is Nothing Then
                        For i As Integer = 0 To Me.BsGridSample.numParams - 1
                            Me.BsGridSample.EnableCell(pRowIndex, Me.BsGridSample.NAMECOLUMNSPARAMS(i)) = pValue
                        Next

                        ' XBC 06/10/2011
                        Me.BsGridSample.HeaderHighlight(Me.BsGridSample.nameColsParams(0)) = False
                        Me.BsGridSample.HeaderHighlight(Me.BsGridSample.nameColsParams(1)) = False
                        Me.BsGridSample.HeaderHighlight(Me.BsGridSample.nameColsParams(2)) = False
                    End If
                Case ADJUSTMENT_ARMS.REAGENT1
                    If Not Me.BsGridReagent1 Is Nothing Then
                        For i As Integer = 0 To Me.BsGridReagent1.numParams - 1
                            Me.BsGridReagent1.EnableCell(pRowIndex, Me.BsGridReagent1.NAMECOLUMNSPARAMS(i)) = pValue
                        Next

                        ' XBC 06/10/2011
                        Me.BsGridReagent1.HeaderHighlight(Me.BsGridSample.nameColsParams(0)) = False
                        Me.BsGridReagent1.HeaderHighlight(Me.BsGridSample.nameColsParams(1)) = False
                        Me.BsGridReagent1.HeaderHighlight(Me.BsGridSample.nameColsParams(2)) = False
                    End If
                Case ADJUSTMENT_ARMS.REAGENT2
                    If Not Me.BsGridReagent2 Is Nothing Then
                        For i As Integer = 0 To Me.BsGridReagent2.numParams - 1
                            Me.BsGridReagent2.EnableCell(pRowIndex, Me.BsGridReagent2.NAMECOLUMNSPARAMS(i)) = pValue
                        Next

                        ' XBC 06/10/2011
                        Me.BsGridReagent2.HeaderHighlight(Me.BsGridSample.nameColsParams(0)) = False
                        Me.BsGridReagent2.HeaderHighlight(Me.BsGridSample.nameColsParams(1)) = False
                        Me.BsGridReagent2.HeaderHighlight(Me.BsGridSample.nameColsParams(2)) = False
                    End If
                Case ADJUSTMENT_ARMS.MIXER1
                    If Not Me.BsGridMixer1 Is Nothing Then
                        For i As Integer = 0 To Me.BsGridMixer1.numParams - 1
                            Me.BsGridMixer1.EnableCell(pRowIndex, Me.BsGridMixer1.NAMECOLUMNSPARAMS(i)) = pValue
                        Next

                        ' XBC 06/10/2011
                        Me.BsGridMixer1.HeaderHighlight(Me.BsGridSample.nameColsParams(0)) = False
                        Me.BsGridMixer1.HeaderHighlight(Me.BsGridSample.nameColsParams(1)) = False
                        Me.BsGridMixer1.HeaderHighlight(Me.BsGridSample.nameColsParams(2)) = False
                    End If
                Case ADJUSTMENT_ARMS.MIXER2
                    If Not Me.BsGridMixer2 Is Nothing Then
                        For i As Integer = 0 To Me.BsGridMixer2.numParams - 1
                            Me.BsGridMixer2.EnableCell(pRowIndex, Me.BsGridMixer2.NAMECOLUMNSPARAMS(i)) = pValue
                        Next

                        ' XBC 06/10/2011
                        Me.BsGridMixer2.HeaderHighlight(Me.BsGridSample.nameColsParams(0)) = False
                        Me.BsGridMixer2.HeaderHighlight(Me.BsGridSample.nameColsParams(1)) = False
                        Me.BsGridMixer2.HeaderHighlight(Me.BsGridSample.nameColsParams(2)) = False
                    End If
            End Select
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".ActivateRowCells ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".ActivateRowCells ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Deactivates the specific selected adjustment Arm Position grid row if not used
    ''' </summary>
    ''' <param name="pRowIndex"></param>
    ''' <remarks>XBC 04/01/11</remarks>
    Private Sub DeactivateUnusedRowCells(ByVal pRowIndex As Integer)
        Try
            Dim BsGridTemp As New BSGridControl

            Select Case Me.SelectedArmTab
                Case ADJUSTMENT_ARMS.SAMPLE
                    BsGridTemp = Me.BsGridSample
                Case ADJUSTMENT_ARMS.REAGENT1
                    BsGridTemp = Me.BsGridReagent1
                Case ADJUSTMENT_ARMS.REAGENT2
                    BsGridTemp = Me.BsGridReagent2
                Case ADJUSTMENT_ARMS.MIXER1
                    BsGridTemp = Me.BsGridMixer1
                Case ADJUSTMENT_ARMS.MIXER2
                    BsGridTemp = Me.BsGridMixer2
            End Select

            If Not BsGridTemp Is Nothing Then
                With EditedValue
                    If Not .canMovePolarValue Then
                        BsGridTemp.EnableCell(pRowIndex, Me.BsGridSample.NAMECOLUMNSPARAMS(POLAR_COLUMN)) = False
                    End If
                    If Not .canMoveZValue Then
                        BsGridTemp.EnableCell(pRowIndex, Me.BsGridSample.NAMECOLUMNSPARAMS(Z_COLUMN)) = False
                    End If
                    If Not .canMoveRotorValue Then
                        BsGridTemp.EnableCell(pRowIndex, Me.BsGridSample.NAMECOLUMNSPARAMS(ROTOR_COLUMN)) = False
                    End If
                End With
            End If


        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".DeactivateUnusedRowCells ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".DeactivateUnusedRowCells ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Deactivates the Screen's adjust controls if not used
    ''' </summary>
    ''' <remarks>XBC 04/01/11</remarks>
    Private Sub DeactivateUnusedAdjustControls()
        Dim LimitZFine As Single
        Try
            Select Case Me.SelectedArmTab
                Case ADJUSTMENT_ARMS.SAMPLE
                    LimitZFine = Me.SampleLimitZFine
                Case ADJUSTMENT_ARMS.REAGENT1
                    LimitZFine = Me.Reagent1LimitZFine
                Case ADJUSTMENT_ARMS.REAGENT2
                    LimitZFine = Me.Reagent2LimitZFine
                Case ADJUSTMENT_ARMS.MIXER1
                    LimitZFine = Me.Mixer1LimitZFine
                Case ADJUSTMENT_ARMS.MIXER2
                    LimitZFine = Me.Mixer2LimitZFine
            End Select

            With EditedValue
                If .canMovePolarValue Then
                    Me.BsAdjustPolar.Visible = True

                    If .canMoveZValue Then
                        If IsNumeric(.CurrentZValue) And IsNumeric(LimitZFine) Then
                            If .CurrentZValue > LimitZFine Then
                                ' 'Homes' and 'Manual Positioning' are disabled below Z Fine Limit
                                Me.BsAdjustPolar.HomingEnabled = False
                                Me.BsAdjustPolar.EditingEnabled = False
                            Else
                                Me.BsAdjustPolar.HomingEnabled = True
                                Me.BsAdjustPolar.EditingEnabled = True
                            End If
                        End If
                    End If
                Else
                    Me.BsAdjustPolar.Visible = False
                End If
                If .canMoveZValue Then
                    Me.BsAdjustZ.Visible = True
                Else
                    Me.BsAdjustZ.Visible = False
                End If
                If .canMoveRotorValue Then
                    Me.BsAdjustRotor.Visible = True

                    If .canMoveZValue Then
                        If IsNumeric(.CurrentZValue) And IsNumeric(LimitZFine) Then
                            If .CurrentZValue > LimitZFine Then
                                ' 'Homes' and 'Manual Positioning' are disabled below Z Fine Limit
                                Me.BsAdjustRotor.HomingEnabled = False
                                Me.BsAdjustRotor.EditingEnabled = False
                            Else
                                Me.BsAdjustRotor.HomingEnabled = True
                                Me.BsAdjustRotor.EditingEnabled = True
                            End If
                        End If
                    End If
                Else
                    Me.BsAdjustRotor.Visible = False
                End If
            End With
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".DeactivateUnusedAdjustControls ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".DeactivateUnusedAdjustControls ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Activates the specific selected adjustment Arm Position grid test button
    ''' All active but ZRef
    ''' </summary>
    ''' <remarks>XBC 04/01/11</remarks>
    Private Sub ActivateRowCheckButton()
        Select Case SelectedArmTab
            Case ADJUSTMENT_ARMS.SAMPLE
                If Not Me.BsGridSample Is Nothing Then
                    For i As Integer = 0 To Me.BsGridSample.RowsCount - 1
                        If i = ZREF_SAMPLE_ROW Then
                            ' Specified for Z REFERENCE
                            ActivateCheckButton(i, False)

                        ElseIf i = ISE_SAMPLE_ROW Then
                            ' XBC 28/03/2012
                            If Not AnalyzerController.Instance.Analyzer.ISEAnalyzer.IsISEModuleInstalled Then '#REFACTORING
                                ' Specified for ISE
                                ActivateCheckButton(i, False)
                            Else
                                ActivateCheckButton(i, True)
                            End If
                        Else
                            ActivateCheckButton(i, True)
                        End If
                    Next
                End If
            Case ADJUSTMENT_ARMS.REAGENT1
                If Not Me.BsGridReagent1 Is Nothing Then
                    For i As Integer = 0 To Me.BsGridReagent1.RowsCount - 1
                        If i = ZREF_REAGENT_ROW Then
                            ' Specified for Z REFERENCE
                            ActivateCheckButton(i, False)
                        Else
                            ActivateCheckButton(i, True)
                        End If
                    Next
                End If
            Case ADJUSTMENT_ARMS.REAGENT2
                If Not Me.BsGridReagent2 Is Nothing Then
                    For i As Integer = 0 To Me.BsGridReagent2.RowsCount - 1
                        If i = ZREF_REAGENT_ROW Then
                            ' Specified for Z REFERENCE
                            ActivateCheckButton(i, False)
                        Else
                            ActivateCheckButton(i, True)
                        End If
                    Next
                End If
            Case ADJUSTMENT_ARMS.MIXER1
                If Not Me.BsGridMixer1 Is Nothing Then
                    For i As Integer = 0 To Me.BsGridMixer1.RowsCount - 1
                        If i = ZREF_MIXER_ROW Then
                            ' Specified for Z REFERENCE
                            ActivateCheckButton(i, False)
                        Else
                            ActivateCheckButton(i, True)
                        End If
                    Next
                End If
            Case ADJUSTMENT_ARMS.MIXER2
                If Not Me.BsGridMixer2 Is Nothing Then
                    For i As Integer = 0 To Me.BsGridMixer2.RowsCount - 1
                        If i = ZREF_MIXER_ROW Then
                            ' Specified for Z REFERENCE
                            ActivateCheckButton(i, False)
                        Else
                            ActivateCheckButton(i, True)
                        End If
                    Next
                End If
        End Select
    End Sub

    ''' <summary>
    ''' Activates the specific selected adjustment Arm Position test button
    ''' </summary>
    ''' <param name="pRowIndex"></param>
    ''' <param name="pValue"></param>
    ''' <remarks>XBC 04/01/11</remarks>
    Private Sub ActivateCheckButton(ByVal pRowIndex As Integer, ByVal pValue As Boolean)
        Try
            Select Case SelectedArmTab
                Case ADJUSTMENT_ARMS.SAMPLE
                    If Not Me.BsGridSample Is Nothing Then
                        Me.BsGridSample.EnableButton2(pRowIndex) = pValue
                    End If
                Case ADJUSTMENT_ARMS.REAGENT1
                    If Not Me.BsGridReagent1 Is Nothing Then
                        Me.BsGridReagent1.EnableButton2(pRowIndex) = pValue
                    End If
                Case ADJUSTMENT_ARMS.REAGENT2
                    If Not Me.BsGridReagent2 Is Nothing Then
                        Me.BsGridReagent2.EnableButton2(pRowIndex) = pValue
                    End If
                Case ADJUSTMENT_ARMS.MIXER1
                    If Not Me.BsGridMixer1 Is Nothing Then
                        Me.BsGridMixer1.EnableButton2(pRowIndex) = pValue
                    End If
                Case ADJUSTMENT_ARMS.MIXER2
                    If Not Me.BsGridMixer2 Is Nothing Then
                        Me.BsGridMixer2.EnableButton2(pRowIndex) = pValue
                    End If
            End Select
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".ActivateCheckButton ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".ActivateCheckButton ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Show the icon that indicates that that the adjustment has been successfully saved
    ''' </summary>
    ''' <param name="pRowIndex"></param>
    ''' <param name="pValue"></param>
    ''' <remarks>XBC 04/01/11</remarks>
    Private Sub ShowIconValidate(ByVal pRowIndex As Integer, ByVal pValue As Boolean)
        Try
            Select Case SelectedArmTab
                Case ADJUSTMENT_ARMS.SAMPLE
                    If Not Me.BsGridSample Is Nothing Then
                        If pValue Then
                            Me.BsGridSample.ShowIconOk(pRowIndex)
                        Else
                            Me.BsGridSample.HideIconOk(pRowIndex)
                        End If
                    End If
                Case ADJUSTMENT_ARMS.REAGENT1
                    If Not Me.BsGridReagent1 Is Nothing Then
                        If pValue Then
                            Me.BsGridReagent1.ShowIconOk(pRowIndex)
                        Else
                            Me.BsGridReagent1.HideIconOk(pRowIndex)
                        End If
                    End If
                Case ADJUSTMENT_ARMS.REAGENT2
                    If Not Me.BsGridReagent2 Is Nothing Then
                        If pValue Then
                            Me.BsGridReagent2.ShowIconOk(pRowIndex)
                        Else
                            Me.BsGridReagent2.HideIconOk(pRowIndex)
                        End If
                    End If
                Case ADJUSTMENT_ARMS.MIXER1
                    If Not Me.BsGridMixer1 Is Nothing Then
                        If pValue Then
                            Me.BsGridMixer1.ShowIconOk(pRowIndex)
                        Else
                            Me.BsGridMixer1.HideIconOk(pRowIndex)
                        End If
                    End If
                Case ADJUSTMENT_ARMS.MIXER2
                    If Not Me.BsGridMixer2 Is Nothing Then
                        If pValue Then
                            Me.BsGridMixer2.ShowIconOk(pRowIndex)
                        Else
                            Me.BsGridMixer2.HideIconOk(pRowIndex)
                        End If
                    End If
            End Select
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".ShowIconValidate ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".ShowIconValidate ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Leads the user through a guided mode
    ''' </summary>
    ''' <remarks>Created by XBC 16/03/2011</remarks>
    Private Sub GoToNextWizardMode()
        Dim RowToGo As Integer
        Try
            RowToGo = Me.SelectedRow + 1
            Select Case Me.SelectedArmTab
                Case ADJUSTMENT_ARMS.SAMPLE
                    ' XBC 28/09/2011 by now Z-Tube is not able to configure by user
                    If Me.SelectedRow + 1 = ZTUBE_SAMPLE_ROW Then
                        RowToGo += 1
                    End If
                    If Me.SelectedRow = Me.BsGridSample.RowsCount - 1 Then
                        Exit Sub
                    End If
                Case ADJUSTMENT_ARMS.REAGENT1
                    If Me.SelectedRow = Me.BsGridReagent1.RowsCount - 1 Then
                        Exit Sub
                    End If
                Case ADJUSTMENT_ARMS.REAGENT2
                    If Me.SelectedRow = Me.BsGridReagent2.RowsCount - 1 Then
                        Exit Sub
                    End If
                Case ADJUSTMENT_ARMS.MIXER1
                    If Me.SelectedRow = Me.BsGridMixer1.RowsCount - 1 Then
                        Exit Sub
                    End If
                Case ADJUSTMENT_ARMS.MIXER2
                    If Me.SelectedRow = Me.BsGridMixer2.RowsCount - 1 Then
                        Exit Sub
                    End If
            End Select

            BsAdjustButtons_Click(Nothing, RowToGo)


        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".GoToNextWizardMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".GoToNextWizardMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub PrepareStirrerButton(ByRef pButton As BSButton)

        Dim myGlobal As New GlobalDataTO
        'Dim Utilities As New Utilities

        Dim auxIconName As String = String.Empty
        Dim iconPath As String = MyBase.IconsPath

        Try

            ' XBC 05/10/2011
            'If MyBase.CurrentMode <> ADJUSTMENT_MODES.STIRRER_TESTING Then
            If Not Me.IsStirrerTesting Then
                auxIconName = GetIconName("ESPIRAL")
            Else
                auxIconName = GetIconName("STOP")
            End If

            Dim myNewImage As Image
            If File.Exists(iconPath & auxIconName) Then

                Dim myImage As Image
                myImage = ImageUtilities.ImageFromFile(iconPath & auxIconName)

                myGlobal = ResizeImage(myImage, New Size(24, 24))
                If Not myGlobal.HasError And myGlobal.SetDatos IsNot Nothing Then
                    myNewImage = CType(myGlobal.SetDatos, Bitmap)
                Else
                    myNewImage = CType(myImage, Bitmap)
                End If

                pButton.Image = myNewImage
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareStirrerButton ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareStirrerButton ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try

    End Sub

    ''' <summary>
    ''' Prepare GUI for Mixer Test Start Mode
    ''' </summary>
    ''' <remarks>Created by SGM 12/07/2011</remarks>
    Private Sub PrepareStirrerTestMode()
        Try
            Me.Cursor = Cursors.Default

            ActivateAdjustButtons(False)
            ActivateCheckButtons(False)
            ManageTabArms = False
            ManageTabPages = False

            MyClass.OkButtonEnabledBeforeStirrerTest = Me.BsArmsOkButton.Enabled
            MyClass.CancelButtonEnabledBeforeStirrerTest = Me.BsArmsCancelButton.Enabled

            MyBase.myScreenLayout.ButtonsPanel.AdjustButton.Enabled = False
            MyBase.myScreenLayout.ButtonsPanel.SaveButton.Enabled = False
            MyBase.myScreenLayout.ButtonsPanel.ExitButton.Enabled = True
            MyBase.myScreenLayout.ButtonsPanel.CancelButton.Enabled = False

            Me.IsStirrerTesting = True

            ' XBC 30/11/2011
            Me.BsUpDownWSButton3.Enabled = False

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareStirrerTestMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareStirrerTestMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Prepare GUI for Mixer Testing Mode
    ''' </summary>
    ''' <remarks>Created by SGM 12/07/2011</remarks>
    Private Sub PrepareStirrerTestingMode()
        Try
            Me.Cursor = Cursors.Default

            MyBase.DisplayMessage(Messages.SRV_MIXER_TESTING.ToString)

            Select Case Me.SelectedArmTab
                Case ADJUSTMENT_ARMS.MIXER1
                    Me.PrepareStirrerButton(Me.BsStirrer1Button)
                Case ADJUSTMENT_ARMS.MIXER2
                    Me.PrepareStirrerButton(Me.BsStirrer2Button)
            End Select

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareStirrerTestingMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareStirrerTestingMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
            Me.ReportHistoryError()
        End Try
    End Sub

    ''' <summary>
    ''' Prepare GUI for Mixer Tested Mode
    ''' </summary>
    ''' <remarks>Created by SGM 12/07/2011</remarks>
    Private Sub PrepareStirrerTestedMode()
        Try
            Me.Cursor = Cursors.Default

            MyBase.DisplayMessage(Messages.SRV_MIXER_TESTED.ToString)

            Me.ReportHistory(HISTORY_TASKS.TEST, PositionsAdjustmentDelegate.HISTORY_RESULTS.OK)

            Me.IsStirrerTesting = False

            Select Case Me.SelectedArmTab
                Case ADJUSTMENT_ARMS.MIXER1
                    Me.PrepareStirrerButton(Me.BsStirrer1Button)
                Case ADJUSTMENT_ARMS.MIXER2
                    Me.PrepareStirrerButton(Me.BsStirrer2Button)
            End Select

            MyBase.ActivateMDIMenusButtons(True) 'SGM 28/09/2011

            ' XBC 16/11/2011
            If Not Me.BsAdjustPolar.Enabled And Not Me.BsAdjustZ.Enabled And Not Me.IsWaitingExitTest Then
                Me.PrepareLoadedMode()
            ElseIf Me.IsWaitingExitTest Then
                MyBase.CurrentMode = ADJUSTMENT_MODES.TESTED
                Me.ActivateCheckButton(Me.SelectedRow, True)
            Else
                MyBase.myScreenLayout.ButtonsPanel.SaveButton.Enabled = True
                MyBase.myScreenLayout.ButtonsPanel.ExitButton.Enabled = True
                MyBase.myScreenLayout.ButtonsPanel.CancelButton.Enabled = True
            End If
            ' XBC 16/11/2011

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareStirrerTestedMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareStirrerTestedMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

#End Region

#Region "optic Centering Private Methods"
    ''' <summary>
    ''' draws the chart with the obtained absorbance data
    ''' </summary>
    ''' <remarks>SG 01/02/2011</remarks>
    Private Sub DrawAbsorbanceChart()
        Try
            If Me.AbsorbanceData IsNot Nothing AndAlso Me.AbsorbanceData.Count > 0 Then
                ' Initializations
                EncoderTransitions = New List(Of Single)
                CenterDistances = New List(Of Single)

                If MyBase.SimulationMode Then
                    CType(Me.AbsorbanceChart.Diagram, XYDiagram).AxisY.WholeRange.MinValue = 0
                    CType(Me.AbsorbanceChart.Diagram, XYDiagram).AxisY.WholeRange.MaxValue = (AbsorbanceData.Max * 100000) + 200000
                    CType(Me.AbsorbanceChart.Diagram, XYDiagram).AxisY.VisualRange.MinValue = 0
                    CType(Me.AbsorbanceChart.Diagram, XYDiagram).AxisY.VisualRange.MaxValue = (AbsorbanceData.Max * 100000) + 200000
                Else
                    CType(Me.AbsorbanceChart.Diagram, XYDiagram).AxisY.WholeRange.MinValue = 0
                    CType(Me.AbsorbanceChart.Diagram, XYDiagram).AxisY.WholeRange.MaxValue = AbsorbanceData.Max + 200000
                    CType(Me.AbsorbanceChart.Diagram, XYDiagram).AxisY.VisualRange.MinValue = 0
                    CType(Me.AbsorbanceChart.Diagram, XYDiagram).AxisY.VisualRange.MaxValue = AbsorbanceData.Max + 200000
                End If

                'ADDED THOSE INSTRUCTIONS TO AVOID AUTOGRID
                CType(AbsorbanceChart.Diagram, XYDiagram).AxisX.NumericScaleOptions.AutoGrid = False
                CType(AbsorbanceChart.Diagram, XYDiagram).AxisX.DateTimeScaleOptions.AutoGrid = False

                ' Generate a data table and bind the COUNTS serie to it.
                AbsorbanceChart.Series(0).DataSource = MyClass.CreateChartDataCounts()

                ' Specify data members to bind the series.
                AbsorbanceChart.Series(0).ArgumentScaleType = ScaleType.Numerical
                AbsorbanceChart.Series(0).ArgumentDataMember = "Argument"
                AbsorbanceChart.Series(0).ValueScaleType = ScaleType.Numerical
                AbsorbanceChart.Series(0).ValueDataMembers.AddRange(New String() {"Value"})
                CType(AbsorbanceChart.Series(0).View, LineSeriesView).MarkerVisibility = DefaultBoolean.True


                ' Generate a data table and bind the ENCODER serie to it.
                AbsorbanceChart.Series(1).DataSource = MyClass.CreateChartDataEncoder()

                ' Specify data members to bind the series.
                AbsorbanceChart.Series(1).ArgumentScaleType = ScaleType.Numerical
                AbsorbanceChart.Series(1).ArgumentDataMember = "Argument"
                AbsorbanceChart.Series(1).ValueScaleType = ScaleType.Numerical
                AbsorbanceChart.Series(1).ValueDataMembers.AddRange(New String() {"Value"})
                CType(AbsorbanceChart.Series(1).View, LineSeriesView).MarkerVisibility = DefaultBoolean.True

                MyClass.CalculateDistances()
                ' XBC 02/01/2012 - Add Encoder functionality

            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Name & ".GetScreenLabels", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Name & ".GetScreenLabels", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Procedure to calculate transition distances to well centers
    ''' </summary>
    ''' <remarks>Created by XBC 02/01/2012 - Add Encoder functionality</remarks>
    Private Sub CalculateDistances()
        Try
            Dim value As Single
            Dim meanValue As Single

            ' XBC 09/05/2012
            Dim checkFirstZero As Boolean = True
            Dim checkFirstOne As Boolean = False

            For j As Integer = EncoderData.Count - 1 To 0 Step -1
                If checkFirstZero Then
                    If EncoderData(j) = 0 Then
                        checkFirstZero = False
                        checkFirstOne = True
                    End If
                End If
                If checkFirstOne Then
                    If EncoderData(j) = 1 Then
                        EncoderTransitions.Add(j)
                        checkFirstZero = True
                        checkFirstOne = False
                    End If
                End If
            Next
            ' XBC 09/05/2012

            If EncoderTransitions IsNot Nothing AndAlso _
               EncoderTransitions.Count > 0 Then

                CenterDistances = New List(Of Single)
                Dim j As Integer = 0
                For i As Integer = WellCenters.Count - 1 To 0 Step -1
                    If j < EncoderTransitions.Count Then
                        ' looking for the NEXT well
                        If EncoderTransitions(j) > WellCenters(i) Then
                            value = EncoderTransitions(j) - WellCenters(i)
                            CenterDistances.Add(value)
                            j += 1
                        End If
                    Else
                        Exit For
                    End If
                Next

                meanValue = CInt(CenterDistances.Average) ' round final average
                Me.BsEncoderAdjustmentLabel.Text = meanValue.ToString

                EncoderOutOfRange = False     ' XB 04/12/2013
                If meanValue < LimitMinEncoder Or meanValue > LimitMaxEncoder Then
                    Me.BsEncoderAdjustmentLabel.ForeColor = Color.Red
                    MyBase.DisplayMessage(Messages.SRV_OUTOFRANGE.ToString)
                    MyBase.myScreenLayout.ButtonsPanel.SaveButton.Enabled = False
                    EncoderOutOfRange = True     ' XB 04/12/2013
                Else
                    Me.BsEncoderAdjustmentLabel.ForeColor = Color.Black
                    EditedValue.NewEncoderValue = meanValue
                    MyBase.DisplayMessage(Messages.SRV_ADJUSTMENTS_READY.ToString)
                    MyBase.myScreenLayout.ButtonsPanel.SaveButton.Enabled = True
                End If

            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Name & ".CalculateDistances", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Name & ".CalculateDistances", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Generate Chart Counts values
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>Created by XBC 02/01/2012 - Add Encoder functionality</remarks>
    Private Function CreateChartDataCounts() As DataTable
        Try
            ' Create an empty table.
            Dim Table As New DataTable("Table1")

            ' Add two columns to the table.
            Table.Columns.Add("Argument", GetType(Int32))
            Table.Columns.Add("Value", GetType(Int32))

            ' Add data rows to the table.
            'Dim Rnd As New Random()
            Dim Row As DataRow = Nothing
            Dim i As Integer

            i = 1
            For Each A As Single In AbsorbanceData
                Row = Table.NewRow()
                Row("Argument") = i
                If MyBase.SimulationMode Then
                    Row("Value") = A * 100000
                Else
                    Row("Value") = A
                End If
                Table.Rows.Add(Row)
                i = i + 1
            Next

            Return Table

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Name & ".CreateChartDataCounts", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Name & ".CreateChartDataCounts", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' Generate Chart Encoder values
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>Created by XBC 02/01/2012 - Add Encoder functionality</remarks>
    Private Function CreateChartDataEncoder() As DataTable
        Try
            ' Create an empty table.
            Dim Table As New DataTable("Table1")

            ' Add two columns to the table.
            Table.Columns.Add("Argument", GetType(Int32))
            Table.Columns.Add("Value", GetType(Int32))

            ' Add data rows to the table.
            Dim Row As DataRow = Nothing
            Dim i As Integer
            Dim value As Single

            i = 1
            For Each A As Single In EncoderData
                Row = Table.NewRow()
                Row("Argument") = i

                If A = 0 Then
                    value = A
                Else
                    value = AbsorbanceData.Max
                End If

                If MyBase.SimulationMode Then
                    value = value * 100000
                End If

                value += 100000
                Row("Value") = value
                Table.Rows.Add(Row)
                i = i + 1
            Next

            Return Table

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Name & ".CreateChartDataEncoder", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Name & ".CreateChartDataEncoder", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
            Return Nothing
        End Try
    End Function

    Private Sub InitializeChart()
        Try
            ' Configure Ranges
            Dim myMin As Integer
            Dim myMax As Integer

            myMin = 0
            myMax = CInt(myScreenDelegate.NumWells * myScreenDelegate.StepsbyWell)

            ' XBC 02/01/2012 - Add Encoder functionality
            ' Y Axis
            CType(AbsorbanceChart.Diagram, XYDiagram).AxisX.WholeRange.MinValue = myMin
            CType(AbsorbanceChart.Diagram, XYDiagram).AxisX.WholeRange.MaxValue = myMax
            CType(AbsorbanceChart.Diagram, XYDiagram).AxisX.VisualRange.MinValue = myMin
            CType(AbsorbanceChart.Diagram, XYDiagram).AxisX.VisualRange.MaxValue = myMax

            'ADDITIONAL CONFIGURATION BECAUSE OF BEHAVIOUR CHANGES IN NEW LIBRARY VERSION
            AbsorbanceChart.CrosshairEnabled = DefaultBoolean.False
            AbsorbanceChart.RuntimeHitTesting = True
            CType(AbsorbanceChart.Diagram, XYDiagram).AxisY.VisualRange.SideMarginsValue = 0
            CType(AbsorbanceChart.Diagram, XYDiagram).AxisY.VisualRange.SideMarginsValue = 0
            CType(AbsorbanceChart.Diagram, XYDiagram).AxisX.VisualRange.SideMarginsValue = 0

            ' Constant lines x wall well
            Dim myDiagram As XYDiagram = CType(Me.AbsorbanceChart.Diagram, XYDiagram)
            ' XBC 02/01/2012 - Add Encoder functionality

            Dim myConstantLine As ConstantLine
            For i As Integer = 1 To myScreenDelegate.NumWells + 1
                myConstantLine = CType(myDiagram.AxisX.ConstantLines.GetElementByName("OldWall" + i.ToString), ConstantLine)
                If myConstantLine IsNot Nothing Then
                    myConstantLine.AxisValue = (i - 1) * myScreenDelegate.StepsbyWell
                End If
                myConstantLine = CType(myDiagram.AxisX.ConstantLines.GetElementByName("NewWall" + i.ToString), ConstantLine)
                If myConstantLine IsNot Nothing Then
                    myConstantLine.AxisValue = (i - 1) * myScreenDelegate.StepsbyWell
                End If

            Next

            ' XBC 02/01/2012 - Add Encoder functionality
            ' Initializations
            WellCenters = New List(Of Single)
            ' Initial Centers by default
            WellCenters.Add(40)
            WellCenters.Add(120)
            WellCenters.Add(200)
            WellCenters.Add(280)
            WellCenters.Add(360)

            For i As Integer = 1 To WellCenters.Count
                myConstantLine = CType(myDiagram.AxisX.ConstantLines.GetElementByName("CenterWell" + i.ToString), ConstantLine)
                If myConstantLine IsNot Nothing Then
                    myConstantLine.AxisValue = WellCenters(i - 1)
                End If
            Next
            ' XBC 02/01/2012 - Add Encoder functionality

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Name & ".InitializeChart", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Name & ".InitializeChart", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

#End Region

#Region "Washing Station Private Methods"

#End Region

#Region "Generic Private Methods"
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="pMode"></param>
    ''' <param name="pAdjustmentGroup"></param>
    ''' <remarks>XBC 10/01/2011</remarks>
    Private Sub SendFwScript(ByVal pMode As ADJUSTMENT_MODES, _
                             Optional ByVal pAdjustmentGroup As ADJUSTMENT_GROUPS = Nothing)
        Dim myGlobal As New GlobalDataTO
        Try
            If Not myGlobal.HasError AndAlso AnalyzerController.Instance.Analyzer.Connected Then '#REFACTORING
                myScreenDelegate.NoneInstructionToSend = True
                myGlobal = myScreenDelegate.SendFwScriptsQueueList(pMode, pAdjustmentGroup)
                If Not myGlobal.HasError Then
                    If myScreenDelegate.NoneInstructionToSend Then
                        ' Send FwScripts
                        myGlobal = myFwScriptDelegate.StartFwScriptQueue
                        Dim x As String = myFwScriptDelegate.CurrentFwScriptsQueue.Aggregate("", Function(current, o) current + (o.FwScriptID & ";"))
                        If Me.SelectedPage <> ADJUSTMENT_PAGES.OPTIC_CENTERING Then Me.Cursor = Cursors.WaitCursor
                    Else
                        PrepareArea()
                    End If
                End If
            Else
                myGlobal.HasError = True
            End If

            If myGlobal.HasError Then
                PrepareErrorMode()
                If MyBase.myFwScriptDelegate.CurrentFwScriptsQueue IsNot Nothing Then
                    MyBase.myFwScriptDelegate.CurrentFwScriptsQueue.Clear()
                End If
                GlobalBase.CreateLogActivity(myGlobal.ErrorCode, Me.Name & ".SendFwScript ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
                MyBase.ShowMessage(Me.Name & ".SendFwScript ", myGlobal.ErrorCode, myGlobal.ErrorMessage, Me)
            End If

        Catch ex As Exception
            Me.Cursor = Cursors.Default
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".SendFwScript ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".SendFwScript ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>
    ''' Created by XBC 10/01/2011
    ''' Modified by XB 15/10/2014 - Use FCK command after save Optical Centering adjustment - BA-2004
    ''' </remarks>
    Private Sub PrepareArea()

        Dim myGlobal As New GlobalDataTO

        Try
            Application.DoEvents()

            If MyBase.myServiceMDI Is Nothing Then Exit Sub 'SG 22/05/2012

            ' Enabling/desabling form components to this child screen


            Select Case MyBase.CurrentMode

                Case ADJUSTMENT_MODES.ADJUSTMENTS_READING
                    MyBase.DisplayMessage(Messages.SRV_READ_ADJUSTMENTS.ToString)

                Case ADJUSTMENT_MODES.ADJUSTMENTS_READED

                    'SGM 04/02/11
                    'warning question about if change in Optic Centering you must re-adjust the Washing and Arms
                    If Me.SelectedPage = ADJUSTMENT_PAGES.NONE Then

                        Application.DoEvents()
                        MyBase.myServiceMDI.Focus()

                        BsTabPagesControl.SelectedTab = Me.TabOpticCentering
                        Me.SelectedPage = ADJUSTMENT_PAGES.OPTIC_CENTERING
                        Me.SelectedAdjustmentGroup = ADJUSTMENT_GROUPS.PHOTOMETRY


                        myGlobal = MyClass.LoadAdjustmentGroupData()


                        Dim dialogResultToReturn As DialogResult
                        dialogResultToReturn = MyBase.ShowMessage(GetMessageText(Messages.SRV_ADJUSTMENTS_TESTS.ToString), Messages.SRV_OPTIC_ADJUSTMENT_ENTER.ToString)

                    End If

                    If Me.SelectedPage = ADJUSTMENT_PAGES.OPTIC_CENTERING Then
                        Me.PrepareLoadedMode()

                    ElseIf Me.SelectedPage = ADJUSTMENT_PAGES.WASHING_STATION Then
                        Me.PrepareLoadedMode()
                    End If

                    Me.PrepareLoadingMode()

                Case ADJUSTMENT_MODES.LOADING
                    Me.PrepareLoadedMode()

                Case ADJUSTMENT_MODES.LOADED
                    Me.PrepareLoadedMode()

                Case ADJUSTMENT_MODES.ADJUST_PREPARING
                    Me.PrepareAdjustPreparingMode()

                Case ADJUSTMENT_MODES.ADJUST_PREPARED
                    Me.PrepareAdjustPreparedMode()

                Case ADJUSTMENT_MODES.ABSORBANCE_SCANNING
                    Me.PrepareAbsorbanceScanningMode()

                Case ADJUSTMENT_MODES.ABSORBANCE_SCANNED
                    Me.PrepareAbsorbanceScannedMode()

                Case ADJUSTMENT_MODES.ABSORBANCE_PREPARED
                    Me.PrepareAbsorbancePreparedMode()

                Case ADJUSTMENT_MODES.ADJUSTING
                    Me.PrepareAdjustingMode()

                Case ADJUSTMENT_MODES.ADJUSTED
                    Me.PrepareAdjustedMode()

                Case ADJUSTMENT_MODES.ADJUST_EXITING
                    Me.PrepareAdjustExitingMode()

                Case ADJUSTMENT_MODES.SAVING
                    Me.PrepareSavingMode()

                Case ADJUSTMENT_MODES.SAVED
                    ' XB 15/10/2014 - BA-2004
                    Select Case Me.SelectedPage
                        Case ADJUSTMENT_PAGES.OPTIC_CENTERING
                            PrepareFineOpticalCenteringPerformingMode()
                        Case Else
                            Me.PrepareSavedMode()
                    End Select

                Case ADJUSTMENT_MODES.TESTING
                    Me.PrepareTestingMode()

                Case ADJUSTMENT_MODES.TESTED
                    Me.PrepareTestedMode()

                Case ADJUSTMENT_MODES.TEST_EXITING
                    Me.PrepareTestExitingMode()

                Case ADJUSTMENT_MODES.TEST_EXITED
                    Me.PrepareTestExitedMode()
                    Me.PrepareLoadedMode()

                Case ADJUSTMENT_MODES.STIRRER_TEST
                    Me.PrepareStirrerTestMode()

                Case ADJUSTMENT_MODES.STIRRER_TESTING
                    Me.PrepareStirrerTestingMode()

                Case ADJUSTMENT_MODES.STIRRER_TESTED
                    Me.PrepareStirrerTestedMode()

                    ' XBC 30/11/2011
                Case ADJUSTMENT_MODES.PARKING
                    Me.PrepareParkingMode()

                    ' XBC 30/11/2011
                Case ADJUSTMENT_MODES.PARKED
                    Me.PrepareParkedMode()

                    ' XB 15/10/2014 - BA-2004
                Case ADJUSTMENT_MODES.FINE_OPTICAL_CENTERING_DONE
                    Me.PrepareSavedMode()

                Case ADJUSTMENT_MODES.ERROR_MODE
                    Me.PrepareErrorMode()
            End Select

            'SG 24/01/11
            If Me.SelectedPage = ADJUSTMENT_PAGES.ARM_POSITIONS Then
                Me.BsGridSample.Refresh()
                Me.BsGridReagent1.Refresh()
                Me.BsGridReagent2.Refresh()
                Me.BsGridMixer1.Refresh()
                Me.BsGridMixer2.Refresh()
            End If

            'SGM 12/07/2011 visibility of mixer test buttons
            If Me.SelectedPage = ADJUSTMENT_PAGES.ARM_POSITIONS Then
                Me.PrepareStirrerButtonAvailability()
            Else
                Me.BsStirrer1Button.Visible = False
                Me.BsStirrer2Button.Visible = False
            End If

            If Not MyBase.SimulationMode And AnalyzerController.Instance.Analyzer.AnalyzerStatus = AnalyzerManagerStatus.SLEEPING Then '#REFACTORING
                Me.PrepareErrorMode()
                MyBase.DisplayMessage("")
            End If

            If Me.WaitForScriptsExitingScreen Then
                Me.Cursor = Cursors.WaitCursor
                Me.BsOpticAdjustButton.Enabled = False
                Me.BsWSAdjustButton.Enabled = False
                Me.BsSaveButton.Enabled = False
                Me.BsExitButton.Enabled = False
            Else
                'SGM 28/02/2012
                If Me.SelectedPage = ADJUSTMENT_PAGES.ARM_POSITIONS Then
                    If MyBase.CurrentMode = ADJUSTMENT_MODES.STIRRER_TESTED Then
                        Me.BsArmsOkButton.Enabled = MyClass.OkButtonEnabledBeforeStirrerTest
                        Me.BsArmsCancelButton.Enabled = MyClass.CancelButtonEnabledBeforeStirrerTest
                    Else
                        Me.BsArmsOkButton.Enabled = (MyBase.CurrentMode = ADJUSTMENT_MODES.ADJUSTED)
                        Me.BsArmsCancelButton.Enabled = (MyBase.CurrentMode = ADJUSTMENT_MODES.ADJUST_PREPARED Or MyBase.CurrentMode = ADJUSTMENT_MODES.ADJUSTED)
                    End If
                    Me.BsSaveButton.Enabled = MyClass.IsLocallySaved And (MyBase.CurrentMode = ADJUSTMENT_MODES.LOADED Or MyBase.CurrentMode = ADJUSTMENT_MODES.SAVED)
                    If MyBase.CurrentMode = ADJUSTMENT_MODES.ADJUST_PREPARED Or MyBase.CurrentMode = ADJUSTMENT_MODES.ADJUSTING Or MyBase.CurrentMode = ADJUSTMENT_MODES.ADJUSTED Then
                        Me.BsExitButton.Enabled = False
                    End If

                Else
                    If Me.SelectedPage <> ADJUSTMENT_PAGES.OPTIC_CENTERING Then ' XB 04/12/2013 - SaveButton rules activation change for OPTIC CENTERING - Task #173 (SERVICE)
                        Me.BsSaveButton.Enabled = (MyBase.CurrentMode = ADJUSTMENT_MODES.ADJUSTED)
                    End If
                End If

            End If


        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareArea ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareArea ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Shows or hides the stirrer button
    ''' </summary>
    ''' <remarks>SGM 13/07/2011</remarks>
    Private Sub PrepareStirrerButtonAvailability()
        Try
            If MyBase.CurrentMode = ADJUSTMENT_MODES.LOADED Or _
               MyBase.CurrentMode = ADJUSTMENT_MODES.ADJUST_PREPARED Or _
               MyBase.CurrentMode = ADJUSTMENT_MODES.ADJUSTED Or _
               MyBase.CurrentMode = ADJUSTMENT_MODES.SAVED Or _
               MyBase.CurrentMode = ADJUSTMENT_MODES.TESTED Or _
               MyBase.CurrentMode = ADJUSTMENT_MODES.STIRRER_TEST Or _
               MyBase.CurrentMode = ADJUSTMENT_MODES.STIRRER_TESTING Or _
               MyBase.CurrentMode = ADJUSTMENT_MODES.STIRRER_TESTED Then

                If Me.SelectedArmTab = ADJUSTMENT_ARMS.MIXER1 Then
                    Me.BsStirrer1Button.Visible = True
                    Me.BsStirrer1Button.Enabled = True
                ElseIf Me.SelectedArmTab = ADJUSTMENT_ARMS.MIXER2 Then
                    Me.BsStirrer2Button.Visible = True
                    Me.BsStirrer2Button.Enabled = True
                End If

            Else
                If Me.SelectedArmTab = ADJUSTMENT_ARMS.MIXER1 Then
                    Me.BsStirrer1Button.Enabled = False
                    Me.BsStirrer1Button.Visible = True
                ElseIf Me.SelectedArmTab = ADJUSTMENT_ARMS.MIXER2 Then
                    Me.BsStirrer2Button.Enabled = False
                    Me.BsStirrer2Button.Visible = True
                Else
                    Me.BsStirrer1Button.Visible = False
                    Me.BsStirrer2Button.Visible = False
                End If

            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareStirrerButtonAvailability ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareStirrerButtonAvailability ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Prepare GUI for Loading Mode
    ''' </summary>
    ''' <remarks>Created by XBC 14/01/2011</remarks>
    Private Sub PrepareLoadingMode()
        Dim myResultData As New GlobalDataTO
        Try
            ' Initializations
            InitializeAdjustControls()

            ' Get common Parameters
            If Not myResultData.HasError Then
                Me.ActiveAnalyzerModel = MyBase.myServiceMDI.ActiveAnalyzerModel
                myResultData = GetParameters(Me.ActiveAnalyzerModel)
            End If

            Me.InitializeChart()

            DefineScreenLayout(MyClass.SelectedPage)

            If Me.SelectedPage = ADJUSTMENT_PAGES.ARM_POSITIONS Then

                MyBase.myScreenLayout.ButtonsPanel.SaveButton.Enabled = False
                MyBase.myScreenLayout.ButtonsPanel.CancelButton.Enabled = False
                MyBase.myScreenLayout.ButtonsPanel.AdjustButton.Enabled = False
                MyBase.myScreenLayout.ButtonsPanel.ExitButton.Enabled = False

            End If

            Me.ChangedValue = False
            myResultData = PrepareTabsArms()

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareLoadingMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareLoadingMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Prepare GUI for Loaded Mode
    ''' </summary>
    ''' <remarks>Created by XBC 14/01/2011</remarks>
    Private Sub PrepareLoadedMode()
        Dim myResultData As New GlobalDataTO
        Try
            InitializeAdjustControls()
            DefineScreenLayout(MyClass.SelectedPage)

            If Me.CurrentMode = ADJUSTMENT_MODES.TEST_EXITING Then
                MyBase.DisplayMessage(Messages.SRV_TEST_EXIT_COMPLETED.ToString)
            End If

            Select Case Me.SelectedPage

                Case ADJUSTMENT_PAGES.OPTIC_CENTERING
                    'display the chart
                    Me.BsAdjustOptic.Visible = True
                    Me.BsLEDCurrentTrackBar.Enabled = True
                    Me.BsMinusLabel.Cursor = Cursors.Hand
                    Me.BsPlusLabel.Cursor = Cursors.Hand
                    ' XBC 30/11/2011
                    Me.BsUpDownWSButton1.Enabled = True
                    Me.BsOpticAdjustButton.Visible = True
                    Me.BsOpticStopButton.Visible = False

                Case ADJUSTMENT_PAGES.WASHING_STATION
                    Me.BsAdjustWashing.Visible = True
                    Me.BsUpDownWSButton2.Enabled = True

                Case ADJUSTMENT_PAGES.ARM_POSITIONS
                    ManageTabArms = True
                    ' XBC 30/11/2011
                    Me.BsUpDownWSButton3.Enabled = True
                    Me.BsGridSample.Enabled = True
                    Me.BsGridReagent1.Enabled = True
                    Me.BsGridReagent2.Enabled = True
                    Me.BsGridMixer1.Enabled = True
                    Me.BsGridMixer2.Enabled = True
                    MyBase.DisplayMessage("")

            End Select

            MyBase.myScreenLayout.ButtonsPanel.SaveButton.Enabled = False
            MyBase.myScreenLayout.ButtonsPanel.CancelButton.Enabled = False

            If MyBase.CurrentUserNumericalLevel = USER_LEVEL.lOPERATOR Then
                MyBase.myScreenLayout.ButtonsPanel.AdjustButton.Enabled = False
            Else
                MyBase.myScreenLayout.ButtonsPanel.AdjustButton.Enabled = True
            End If

            MyBase.myScreenLayout.ButtonsPanel.ExitButton.Enabled = True

            If Not myResultData.HasError Then
                MyClass.PreparePage()
                If Not myResultData.HasError Then
                    MyClass.ActivateAdjustButtons(True)
                    MyClass.ActivateRowCheckButton()
                    MyBase.myScreenLayout.ButtonsPanel.ExitButton.Enabled = True
                    MyClass.ManageTabPages = True
                    MyBase.ActivateMDIMenusButtons(True) 'SGM 28/09/2011
                End If
            End If

            If myResultData.HasError Then
                MyClass.PrepareErrorMode()
            End If

            'SGM 29/02/2012
            If Me.WaitForScriptsExitingScreen Then
                If Not MyBase.SimulationMode Then Thread.Sleep(2000)
                Me.FinishExitScreen()
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareLoadedMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareLoadedMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        Finally
            Me.Cursor = Cursors.Default
        End Try
    End Sub

    ''' <summary>
    ''' Prepare GUI for Adjust Preparing Mode
    ''' </summary>
    ''' <remarks>Created by XBC 14/01/2011</remarks>
    Private Sub PrepareAdjustPreparingMode()
        Try
            ActivateAdjustButtons(False)

            ' XBC 30/11/2011
            DisableAll()

            Select Case Me.SelectedPage
                Case ADJUSTMENT_PAGES.OPTIC_CENTERING
                    Me.AbsorbanceData = New List(Of Single)

                    If myScreenDelegate.HomesDone Then
                        MyBase.CurrentMode = ADJUSTMENT_MODES.ADJUST_PREPARED
                        PrepareArea()
                    End If

                Case ADJUSTMENT_PAGES.WASHING_STATION

                    ' XBC 18/04/2012
                    If myScreenDelegate.WSAdjustPrepared Then
                        Me.CurrentMode = ADJUSTMENT_MODES.ADJUST_PREPARED
                        PrepareArea()
                        Exit Try
                    End If

                    Me.Cursor = Cursors.WaitCursor

                Case ADJUSTMENT_PAGES.ARM_POSITIONS
                    Me.Cursor = Cursors.WaitCursor
                    ActivateCheckButtons(False)

            End Select

            DisableButtons() 'SGM 24/01/11

            MyBase.myScreenLayout.ButtonsPanel.ExitButton.Focus()

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareAdjustPreparingMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareAdjustPreparingMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub PrepareAbsorbanceScanningMode()
        Try
            ' XBC 21/05/2012
            Me.IsCenteringOptic = True
            MyBase.DisplayMessage(Messages.SRV_ABSORBANCE_REQUEST.ToString)
            MyBase.ActivateMDIMenusButtons(False) 'SGM 28/09/2011

            If MyBase.SimulationMode Then

                MyBase.myServiceMDI.Focus()

                Me.ProgressBar1.Maximum = 5
                Me.ProgressBar1.Value = 0
                Me.ProgressBar1.Visible = True
                Thread.Sleep(SimulationProcessTime)
                Me.ProgressBar1.Value = 1
                Thread.Sleep(SimulationProcessTime)
                Me.ProgressBar1.Value = 2
                Thread.Sleep(SimulationProcessTime)
                Me.ProgressBar1.Value = 3
                Thread.Sleep(SimulationProcessTime)
                Me.ProgressBar1.Value = 4
                Thread.Sleep(SimulationProcessTime)
                Me.ProgressBar1.Value = 5
                Me.ProgressBar1.Visible = False

                MyBase.myServiceMDI.Focus()
                Me.Cursor = Cursors.Default

                MyBase.CurrentMode = ADJUSTMENT_MODES.ABSORBANCE_SCANNED
                PrepareArea()
            Else
                MyBase.ReadAbsorbance()

                Me.SendFwScript(Me.CurrentMode)
                DisableAll()

                DisableButtons()
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareAbsorbanceScanningMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareAbsorbanceScanningMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
            Me.ReportHistoryError()
        End Try
    End Sub


    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>
    ''' Creation ?
    ''' AG 01/10/2014 - BA-1953 new photometry adjustment maneuver (use REAGENTS_HOME_ROTOR + REAGENTS_ABS_ROTOR (with parameter = current value of GFWR1) instead of REACTIONS_ROTOR_HOME_WELL1)
    ''' </remarks>
    Private Sub PrepareAbsorbanceScannedMode()
        Try
            If Me.myScreenDelegate.AbsorbanceScanDone Then
                ' Absorbance readings has finished, proceed to place rotor in well 1 again
                MyBase.PrepareAbsorbance()
                MyBase.DisplayMessage(Messages.SRV_ABS_REQUESTED.ToString)
                If MyBase.SimulationMode Then
                    ' simulating
                    MyBase.CurrentMode = ADJUSTMENT_MODES.ABSORBANCE_PREPARED
                    Me.PrepareArea()
                Else
                    myScreenDelegate.pValueAdjust = BsOpticAdjustmentLabel.Text 'AG 01/10/2014 - BA-1953 inform the current value of adjustment GFWR1 (Posición referencia lectura - Pocillo 1)
                    Me.SendFwScript(Me.CurrentMode)
                    Me.DisableAll()
                End If

            Else
                ' Continue with the Readings until finish it
                ' Manage FwScripts must to be sent to testing
                MyBase.ReadAbsorbance()
                MyBase.DisplayMessage(Messages.SRV_READ_COUNTS.ToString)

                Me.ProgressBar1.Value += 1
                Me.ProgressBar1.Refresh()

                If MyBase.SimulationMode Then
                    ' simulating
                    MyBase.CurrentMode = ADJUSTMENT_MODES.ABSORBANCE_SCANNED
                    MyBase.ActivateMDIMenusButtons(True) 'SGM 28/09/2011
                    Me.PrepareArea()
                Else
                    Me.SendFwScript(Me.CurrentMode)
                    Me.DisableAll()
                End If
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareAbsorbanceScanningMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareAbsorbanceScanningMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
            Me.ReportHistoryError()
        End Try
    End Sub

    ''' <summary>
    ''' Prepare GUI for Optical centering final absorbance reading Prepared Mode
    ''' </summary>
    ''' <remarks>
    ''' Created by XBC 23/09/2011
    ''' Modified by XB 31/10/2014 - Correction - BA-2058
    ''' </remarks>
    Private Sub PrepareAbsorbancePreparedMode()
        Try
            Dim myGlobal As New GlobalDataTO
            ' XBC 21/05/2012
            Me.IsCenteringOptic = False
            ' XBC 30/11/2011
            If Me.IsReadingCountsAbortedByUser Then
                Me.IsReadingCountsAbortedByUser = False
                Me.ProgressBar1.Value = 0
                Me.ProgressBar1.Visible = False
                Me.ProgressBar1.Refresh()
                MyBase.DisplayMessage(Messages.SRV_TEST_STOP_BY_USER.ToString)
                Me.PrepareLoadedMode()
                MyBase.myServiceMDI.SEND_INFO_START()
                myFwScriptDelegate.INFOManagementEnabled = True

                Exit Try
            End If

            If MyBase.SimulationMode Then
                MyBase.DisplayMessage(Messages.SRV_ABSORBANCE_RECEIVED.ToString)
                MyBase.myServiceMDI.Focus()
                'simulate Absorbance reception
                myGlobal = Me.SimulateAbsorbanceData()

                If Not myGlobal.HasError And myGlobal.SetDatos IsNot Nothing Then
                    Me.AbsorbanceData = CType(myGlobal.SetDatos, List(Of Single))

                    myGlobal = Me.SimulateEncoderData()
                    If Not myGlobal.HasError And myGlobal.SetDatos IsNot Nothing Then
                        Me.EncoderData = CType(myGlobal.SetDatos, List(Of Single))
                    End If

                    Me.DrawAbsorbanceChart()
                Else
                    PrepareErrorMode()
                End If

            Else

                Me.ProgressBar1.Value = 0
                Me.ProgressBar1.Visible = False
                Me.ProgressBar1.Refresh()

                ' Populate results of the readed Counts into screen

                'obtain data from the App Layer
                Me.AbsorbanceData = myScreenDelegate.ReadedCountsResult ' CType(myGlobal.SetDatos, List(Of Double))
                ' XBC 21/12/2011 - Add Encoder functionality
                Me.EncoderData = myScreenDelegate.ReadedEncoderResult

                ' XBC 05/12/2011
                If Me.AbsorbanceData.Count < myScreenDelegate.NumWells * myScreenDelegate.StepsbyWell Then
                    Me.FillAbsorbanceDataWithEmptyValues()
                End If
                Me.InitializeChart() 'SGM 05/12/2012
                Me.DrawAbsorbanceChart()

                AbsorbanceChart.Refresh()

                MyBase.DisplayMessage(Messages.SRV_ADJUSTMENTS_READY.ToString)

            End If

            MyBase.ActivateMDIMenusButtons(True) 'SGM 28/09/2011
            ' Filling EditionValue Structure as well as each position Arm selected
            With Me.EditedValue
                .canMoveRotorValue = True
                .canSaveRotorValue = True
                .LastRotorValue = CSng(ReadSpecificAdjustmentData(GlobalEnumerates.AXIS.ROTOR).Value)
                .CurrentRotorValue = .LastRotorValue
                .NewRotorValue = .LastRotorValue

                ' XBC 03/01/2012 - Add Encoder functionality
                .LastEncoderValue = CSng(ReadSpecificAdjustmentData(GlobalEnumerates.AXIS.ENCODER).Value)

                Me.BsAdjustOptic.CurrentValue = .CurrentRotorValue
                Me.BsAdjustOptic.Enabled = True
            End With
            MyBase.SetAdjustmentItems(Me.BsOpticAdjustPanel)

            Me.BsAdjustOptic.Focus()

            Me.BsLEDCurrentTrackBar.Enabled = False

            MyBase.myScreenLayout.ButtonsPanel.CancelButton.Enabled = True
            MyBase.myScreenLayout.ButtonsPanel.AdjustButton.Enabled = False
            MyBase.myScreenLayout.ButtonsPanel.ExitButton.Enabled = True
            ' XBC 30/11/2011
            Me.BsOpticAdjustButton.Visible = True
            Me.BsOpticStopButton.Visible = False


        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareAbsorbancePreparedMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareAbsorbancePreparedMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
            Me.ReportHistoryError()
        Finally
            Me.Cursor = Cursors.Default
        End Try
    End Sub

    ''' <summary>
    ''' When expected values not match with received values must be filled the rest with an empty value
    ''' </summary>
    ''' <remarks>Created by XBC 05/12/2011</remarks>
    Private Sub FillAbsorbanceDataWithEmptyValues()
        Try
            Dim initialValue As Integer
            Dim maxValue As Integer

            maxValue = myScreenDelegate.NumWells * myScreenDelegate.StepsbyWell
            initialValue = Me.AbsorbanceData.Count

            For i As Integer = initialValue To maxValue - 1
                Me.AbsorbanceData.Add(0)
            Next

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".FillAbsorbanceDataWithEmptyValues ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".FillAbsorbanceDataWithEmptyValues ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Prepare GUI for Adjust Prepared Mode
    ''' </summary>
    ''' <remarks>Created by XBC 14/01/2011</remarks>
    Private Sub PrepareAdjustPreparedMode()
        Dim firstAvailableParam As Integer
        Try
            firstAvailableParam = -1

            LoadAdjustmentGroupData()

            Select Case Me.SelectedPage
                Case ADJUSTMENT_PAGES.OPTIC_CENTERING
                    PrepareAdjustPreparedModeForOpticCentering()

                Case ADJUSTMENT_PAGES.WASHING_STATION
                    PrepareAdjustPreparedModeForWashingStation()
                    Me.BsWSAdjustButton.Enabled = False
                    Me.BsWSCancelButton.Enabled = True
                    Me.BsSaveButton.Enabled = True
                    Me.BsExitButton.Enabled = True
                    Me.ManageTabPages = False
                    MyBase.DisplayMessage(Messages.SRV_ADJUSTMENTS_READY.ToString)
                    Me.ChangedValue = False

                Case ADJUSTMENT_PAGES.ARM_POSITIONS
                    PrepareAdjustPreparedModeForArmPositions(firstAvailableParam)
                    Me.BsArmsOkButton.Enabled = False
                    Me.BsArmsCancelButton.Enabled = True
                    Me.BsSaveButton.Enabled = MyClass.IsLocallySaved
                    Me.BsExitButton.Enabled = True
                    Me.ManageTabPages = False
                    MyBase.DisplayMessage(Messages.SRV_ADJUSTMENTS_READY.ToString)

            End Select

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareAdjustPreparedMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareAdjustPreparedMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        Finally
            Me.Cursor = Cursors.Default
        End Try
    End Sub

    Private Sub PrepareAdjustPreparedModeForArmPositions(ByRef firstAvailableParam As Integer)

        ' Filling EditionValue Structure as well as each position Arm selected

        Select Case Me.SelectedArmTab
            Case ADJUSTMENT_ARMS.SAMPLE
                Me.BsGridSample.SelectedRow = SelectedRow
                ' Set row text cells as a available to Edit
                If Me.BsGridSample.RowsCount > 0 Then
                    Me.BsGridSample.EnableCell(SelectedRow, Me.BsGridSample.nameColsParams(POLAR_COLUMN)) = True
                    Me.BsGridSample.EnableCell(SelectedRow, Me.BsGridSample.nameColsParams(Z_COLUMN)) = True
                    Me.BsGridSample.EnableCell(SelectedRow, Me.BsGridSample.nameColsParams(ROTOR_COLUMN)) = True
                    ' Set another rows text cells as a not available to Edit
                    For i As Integer = 0 To Me.BsGridSample.RowsCount - 1
                        If i <> SelectedRow Then
                            Me.BsGridSample.EnableCell(i, Me.BsGridSample.nameColsParams(POLAR_COLUMN)) = False
                            Me.BsGridSample.EnableCell(i, Me.BsGridSample.nameColsParams(Z_COLUMN)) = False
                            Me.BsGridSample.EnableCell(i, Me.BsGridSample.nameColsParams(ROTOR_COLUMN)) = False
                        End If
                    Next
                End If

                firstAvailableParam = PrepareEditedValueForAdjusting()

                If firstAvailableParam > -1 Then
                    ' First available element is activated By default
                    Dim send As Object = Nothing
                    Dim myCellEventArgs As DataGridViewCellEventArgs = New DataGridViewCellEventArgs(firstAvailableParam + 3, SelectedRow)
                    Me.BsGridSample.CellClick(send, myCellEventArgs)
                End If

            Case ADJUSTMENT_ARMS.REAGENT1

                Me.BsGridReagent1.SelectedRow = SelectedRow
                ' Set row text cells as a available to Edit
                If Me.BsGridReagent1.RowsCount > 0 Then
                    Me.BsGridReagent1.EnableCell(SelectedRow, Me.BsGridReagent1.nameColsParams(POLAR_COLUMN)) = True
                    Me.BsGridReagent1.EnableCell(SelectedRow, Me.BsGridReagent1.nameColsParams(Z_COLUMN)) = True
                    Me.BsGridReagent1.EnableCell(SelectedRow, Me.BsGridReagent1.nameColsParams(ROTOR_COLUMN)) = True
                    ' Set another rows text cells as a not available to Edit
                    For i As Integer = 0 To Me.BsGridReagent1.RowsCount - 1
                        If i <> SelectedRow Then
                            Me.BsGridReagent1.EnableCell(i, Me.BsGridReagent1.nameColsParams(POLAR_COLUMN)) = False
                            Me.BsGridReagent1.EnableCell(i, Me.BsGridReagent1.nameColsParams(Z_COLUMN)) = False
                            Me.BsGridReagent1.EnableCell(i, Me.BsGridReagent1.nameColsParams(ROTOR_COLUMN)) = False
                        End If
                    Next
                End If

                firstAvailableParam = PrepareEditedValueForAdjusting()

                If firstAvailableParam > -1 Then
                    ' First available element is activated By default
                    Dim send As Object = Nothing
                    Dim myCellEventArgs As DataGridViewCellEventArgs = New DataGridViewCellEventArgs(firstAvailableParam + 3, SelectedRow)
                    Me.BsGridReagent1.CellClick(send, myCellEventArgs)
                End If


            Case ADJUSTMENT_ARMS.REAGENT2

                Me.BsGridReagent2.SelectedRow = SelectedRow
                ' Set row text cells as a available to Edit
                If Me.BsGridReagent2.RowsCount > 0 Then
                    Me.BsGridReagent2.EnableCell(SelectedRow, Me.BsGridReagent2.nameColsParams(POLAR_COLUMN)) = True
                    Me.BsGridReagent2.EnableCell(SelectedRow, Me.BsGridReagent2.nameColsParams(Z_COLUMN)) = True
                    Me.BsGridReagent2.EnableCell(SelectedRow, Me.BsGridReagent2.nameColsParams(ROTOR_COLUMN)) = True
                    ' Set another rows text cells as a not available to Edit
                    For i As Integer = 0 To Me.BsGridReagent2.RowsCount - 1
                        If i <> SelectedRow Then
                            Me.BsGridReagent2.EnableCell(i, Me.BsGridReagent2.nameColsParams(POLAR_COLUMN)) = False
                            Me.BsGridReagent2.EnableCell(i, Me.BsGridReagent2.nameColsParams(Z_COLUMN)) = False
                            Me.BsGridReagent2.EnableCell(i, Me.BsGridReagent2.nameColsParams(ROTOR_COLUMN)) = False
                        End If
                    Next
                End If

                firstAvailableParam = PrepareEditedValueForAdjusting()

                If firstAvailableParam > -1 Then
                    ' First available element is activated By default
                    Dim send As Object = Nothing
                    Dim myCellEventArgs As DataGridViewCellEventArgs = New DataGridViewCellEventArgs(firstAvailableParam + 3, SelectedRow)
                    Me.BsGridReagent2.CellClick(send, myCellEventArgs)
                End If

            Case ADJUSTMENT_ARMS.MIXER1

                Me.BsGridMixer1.SelectedRow = SelectedRow
                ' Set row text cells as a available to Edit
                If Me.BsGridMixer1.RowsCount > 0 Then
                    Me.BsGridMixer1.EnableCell(SelectedRow, Me.BsGridMixer1.nameColsParams(POLAR_COLUMN)) = True
                    Me.BsGridMixer1.EnableCell(SelectedRow, Me.BsGridMixer1.nameColsParams(Z_COLUMN)) = True
                    Me.BsGridMixer1.EnableCell(SelectedRow, Me.BsGridMixer1.nameColsParams(ROTOR_COLUMN)) = True
                    ' Set another rows text cells as a not available to Edit
                    For i As Integer = 0 To Me.BsGridMixer1.RowsCount - 1
                        If i <> SelectedRow Then
                            Me.BsGridMixer1.EnableCell(i, Me.BsGridMixer1.nameColsParams(POLAR_COLUMN)) = False
                            Me.BsGridMixer1.EnableCell(i, Me.BsGridMixer1.nameColsParams(Z_COLUMN)) = False
                            Me.BsGridMixer1.EnableCell(i, Me.BsGridMixer1.nameColsParams(ROTOR_COLUMN)) = False
                        End If
                    Next
                End If

                firstAvailableParam = PrepareEditedValueForAdjusting()

                If firstAvailableParam > -1 Then
                    ' First available element is activated By default
                    Dim send As Object = Nothing
                    Dim myCellEventArgs As DataGridViewCellEventArgs = New DataGridViewCellEventArgs(firstAvailableParam + 3, SelectedRow)
                    Me.BsGridMixer1.CellClick(send, myCellEventArgs)
                End If

            Case ADJUSTMENT_ARMS.MIXER2

                Me.BsGridMixer2.SelectedRow = SelectedRow
                ' Set row text cells as a available to Edit
                If Me.BsGridMixer2.RowsCount > 0 Then
                    Me.BsGridMixer2.EnableCell(SelectedRow, Me.BsGridMixer2.nameColsParams(POLAR_COLUMN)) = True
                    Me.BsGridMixer2.EnableCell(SelectedRow, Me.BsGridMixer2.nameColsParams(Z_COLUMN)) = True
                    Me.BsGridMixer2.EnableCell(SelectedRow, Me.BsGridMixer2.nameColsParams(ROTOR_COLUMN)) = True
                    ' Set another rows text cells as a not available to Edit
                    For i As Integer = 0 To Me.BsGridMixer2.RowsCount - 1
                        If i <> SelectedRow Then
                            Me.BsGridMixer2.EnableCell(i, Me.BsGridMixer2.nameColsParams(POLAR_COLUMN)) = False
                            Me.BsGridMixer2.EnableCell(i, Me.BsGridMixer2.nameColsParams(Z_COLUMN)) = False
                            Me.BsGridMixer2.EnableCell(i, Me.BsGridMixer2.nameColsParams(ROTOR_COLUMN)) = False
                        End If
                    Next
                End If

                firstAvailableParam = PrepareEditedValueForAdjusting()

                If firstAvailableParam > -1 Then
                    ' First available element is activated By default
                    Dim send As Object = Nothing
                    Dim myCellEventArgs As DataGridViewCellEventArgs = New DataGridViewCellEventArgs(firstAvailableParam + 3, SelectedRow)
                    Me.BsGridMixer2.CellClick(send, myCellEventArgs)
                End If

        End Select

        DeactivateUnusedRowCells(SelectedRow)
        DeactivateUnusedAdjustControls()

        MyBase.SetAdjustmentItems(Me.BsArmsAdjustPanel)

        Me.ManageTabArms = False
    End Sub

    Private Sub PrepareAdjustPreparedModeForWashingStation()

        ' Filling EditionValue Structure as well as each position Arm selected
        With Me.EditedValue
            .canMoveZValue = True
            .canSaveZValue = True

            ' XBC 18/04/2012 - Z aproximation is anuled
            .LastZValue = -100

            .CurrentZValue = .LastZValue
            .NewZValue = .LastZValue
            Me.BsAdjustWashing.CurrentValue = .CurrentZValue
            Me.BsAdjustWashing.Enabled = True
        End With
        MyBase.SetAdjustmentItems(Me.BsWashingAdjustPanel)

        Me.BsAdjustWashing.Focus()

        MyBase.ActivateMDIMenusButtons(True) 'SGM 28/09/2011
    End Sub

    Private Sub PrepareAdjustPreparedModeForOpticCentering()

        ' XBC 09/05/2012
        MyBase.myServiceMDI.SEND_INFO_STOP()

        If MyBase.SimulationMode Then
            MyBase.myServiceMDI.Focus()

            Application.DoEvents()

            Me.CurrentMode = ADJUSTMENT_MODES.ABSORBANCE_SCANNING
            SimulateAbsorbanceData()
            Me.PrepareArea()
        Else
            MyBase.DisplayMessage(Messages.SRV_READ_COUNTS.ToString)

            MyBase.ReadAbsorbance()

            Me.SendFwScript(Me.CurrentMode)
            Me.DisableAll()
        End If
    End Sub

    ''' <summary>
    ''' prepare Edited Value variable after Adjusting prepared
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>
    ''' Created by SG 31/01/11
    ''' Modified by XBC 06/06/2011 - Add Z offset aproximation
    ''' </remarks>
    Private Function PrepareEditedValueForAdjusting() As Integer
        Dim firstAvailableParam As Integer = -1
        Try
            Dim BsGridTemp As New BSGridControl

            ' XBC 18/04/2012 - Z aproximation is anuled
            'Dim myOffsetZ As Single
            Dim myCurrentZ As String = "0"

            With Me.EditedValue
                Select Case Me.SelectedArmTab
                    Case ADJUSTMENT_ARMS.SAMPLE
                        BsGridTemp = Me.BsGridSample

                        ' XBC 18/04/2012 - Z aproximation is anuled
                        myCurrentZ = myScreenDelegate.SampleSecurityFly

                    Case ADJUSTMENT_ARMS.REAGENT1
                        BsGridTemp = Me.BsGridReagent1

                        ' XBC 18/04/2012 - Z aproximation is anuled
                        myCurrentZ = myScreenDelegate.Reagent1SecurityFly

                    Case ADJUSTMENT_ARMS.REAGENT2
                        BsGridTemp = Me.BsGridReagent2

                        ' XBC 18/04/2012 - Z aproximation is anuled
                        myCurrentZ = myScreenDelegate.Reagent2SecurityFly

                    Case ADJUSTMENT_ARMS.MIXER1
                        BsGridTemp = Me.BsGridMixer1

                        ' XBC 18/04/2012 - Z aproximation is anuled
                        myCurrentZ = myScreenDelegate.Mixer1SecurityFly

                    Case ADJUSTMENT_ARMS.MIXER2
                        BsGridTemp = Me.BsGridMixer2

                        ' XBC 18/04/2012 - Z aproximation is anuled
                        myCurrentZ = myScreenDelegate.Mixer2SecurityFly

                End Select

                If BsGridTemp.RowsCount > 0 Then
                    'POLAR
                    If ReadSpecificAdjustmentData(GlobalEnumerates.AXIS.POLAR).CanMove Then
                        .canMovePolarValue = True
                        If BsGridTemp.ParameterCellValue(SelectedRow, POLAR_COLUMN).Length > 0 Then
                            .LastPolarValue = CSng(BsGridTemp.ParameterCellValue(SelectedRow, POLAR_COLUMN))
                        End If
                        .CurrentPolarValue = .LastPolarValue
                        .NewPolarValue = .LastPolarValue
                        If firstAvailableParam < 0 Then firstAvailableParam = POLAR_COLUMN
                    End If
                    If ReadSpecificAdjustmentData(GlobalEnumerates.AXIS.POLAR).CanSave Then
                        .canSavePolarValue = True
                    End If

                    'Z
                    If ReadSpecificAdjustmentData(GlobalEnumerates.AXIS.Z).CanMove Then
                        .canMoveZValue = True

                        If BsGridTemp.ParameterCellValue(SelectedRow, Z_COLUMN).Length > 0 Then
                            ' XBC 18/04/2012 - Z aproximation is anuled
                            .LastZValue = CSng(myCurrentZ)
                        End If
                        .CurrentZValue = .LastZValue
                        .NewZValue = .LastZValue
                        If firstAvailableParam < 0 Then firstAvailableParam = Z_COLUMN
                    End If
                    If ReadSpecificAdjustmentData(GlobalEnumerates.AXIS.Z).CanSave Then
                        .canSaveZValue = True
                    End If

                    'ROTOR
                    If ReadSpecificAdjustmentData(GlobalEnumerates.AXIS.ROTOR).CanMove Then
                        .canMoveRotorValue = True
                        If BsGridTemp.ParameterCellValue(SelectedRow, ROTOR_COLUMN).Length > 0 Then
                            .LastRotorValue = CSng(BsGridTemp.ParameterCellValue(SelectedRow, ROTOR_COLUMN))
                        End If
                        .CurrentRotorValue = .LastRotorValue
                        .NewRotorValue = .LastRotorValue
                        If firstAvailableParam < 0 Then firstAvailableParam = ROTOR_COLUMN
                    End If
                    If ReadSpecificAdjustmentData(GlobalEnumerates.AXIS.ROTOR).CanSave Then
                        .canSaveRotorValue = True
                    End If


                    ' XBC 06/10/2011
                    Select Case Me.SelectedArmTab
                        Case ADJUSTMENT_ARMS.SAMPLE
                            Me.BsGridSample.HeaderHighlight(Me.BsGridSample.nameColsParams(0)) = .canSavePolarValue
                            Me.BsGridSample.HeaderHighlight(Me.BsGridSample.nameColsParams(1)) = .canSaveZValue
                            Me.BsGridSample.HeaderHighlight(Me.BsGridSample.nameColsParams(2)) = .canSaveRotorValue
                        Case ADJUSTMENT_ARMS.REAGENT1
                            Me.BsGridReagent1.HeaderHighlight(Me.BsGridSample.nameColsParams(0)) = .canSavePolarValue
                            Me.BsGridReagent1.HeaderHighlight(Me.BsGridSample.nameColsParams(1)) = .canSaveZValue
                            Me.BsGridReagent1.HeaderHighlight(Me.BsGridSample.nameColsParams(2)) = .canSaveRotorValue
                        Case ADJUSTMENT_ARMS.REAGENT2
                            Me.BsGridReagent2.HeaderHighlight(Me.BsGridSample.nameColsParams(0)) = .canSavePolarValue
                            Me.BsGridReagent2.HeaderHighlight(Me.BsGridSample.nameColsParams(1)) = .canSaveZValue
                            Me.BsGridReagent2.HeaderHighlight(Me.BsGridSample.nameColsParams(2)) = .canSaveRotorValue
                        Case ADJUSTMENT_ARMS.MIXER1
                            Me.BsGridMixer1.HeaderHighlight(Me.BsGridSample.nameColsParams(0)) = .canSavePolarValue
                            Me.BsGridMixer1.HeaderHighlight(Me.BsGridSample.nameColsParams(1)) = .canSaveZValue
                            Me.BsGridMixer1.HeaderHighlight(Me.BsGridSample.nameColsParams(2)) = .canSaveRotorValue
                        Case ADJUSTMENT_ARMS.MIXER2
                            Me.BsGridMixer2.HeaderHighlight(Me.BsGridSample.nameColsParams(0)) = .canSavePolarValue
                            Me.BsGridMixer2.HeaderHighlight(Me.BsGridSample.nameColsParams(1)) = .canSaveZValue
                            Me.BsGridMixer2.HeaderHighlight(Me.BsGridSample.nameColsParams(2)) = .canSaveRotorValue
                    End Select
                Else
                    PrepareErrorMode()
                End If
            End With

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareEditedValueForAdjusting ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareEditedValueForAdjusting ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
        Return firstAvailableParam
    End Function

    ''' <summary>
    ''' Prepare GUI for Adjusting Mode
    ''' </summary>
    ''' <remarks>Created by XBC 14/01/2011</remarks>
    Private Sub PrepareAdjustingMode()
        Try
            ActivateAdjustButtons(False)
            Select Case Me.SelectedPage
                Case ADJUSTMENT_PAGES.OPTIC_CENTERING
                    'Me.BsOpticTestButton.Enabled = False
                Case ADJUSTMENT_PAGES.WASHING_STATION
                    Me.Cursor = Cursors.WaitCursor
                    'Me.BsWashingTestButton.Enabled = False
                Case ADJUSTMENT_PAGES.ARM_POSITIONS
                    Me.Cursor = Cursors.WaitCursor
                    ActivateCheckButtons(False)
            End Select

            DisableButtons() 'SGM 24/01/11

            MyBase.ActivateMDIMenusButtons(False) 'SGM 28/09/2011

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareAdjustingMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareAdjustingMode", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Prepare GUI for Adjusted Mode
    ''' </summary>
    ''' <remarks>Created by XBC 14/01/2011</remarks>
    Private Sub PrepareAdjustedMode()
        Dim selectedColumn As Integer
        Try

            ' Updating EditionValue Structure as well as each position Arm selected
            If Me.SelectedPage = ADJUSTMENT_PAGES.ARM_POSITIONS Then
                With Me.EditedValue
                    Select Case .AxisID
                        Case GlobalEnumerates.AXIS.POLAR
                            .CurrentPolarValue = .NewPolarValue
                            selectedColumn = POLAR_COLUMN
                        Case GlobalEnumerates.AXIS.Z
                            .CurrentZValue = .NewZValue
                            selectedColumn = Z_COLUMN
                        Case GlobalEnumerates.AXIS.ROTOR
                            .CurrentRotorValue = .NewRotorValue
                            selectedColumn = ROTOR_COLUMN
                    End Select
                End With
            End If

            Select Case Me.SelectedPage
                Case ADJUSTMENT_PAGES.OPTIC_CENTERING
                    Me.EditedValue.CurrentRotorValue = Me.EditedValue.NewRotorValue

                    Me.BsAdjustOptic.CurrentValue = Me.EditedValue.CurrentRotorValue

                    Me.BsAdjustOptic.Enabled = True
                    Me.BsAdjustOptic.EscapeRequest()

                Case ADJUSTMENT_PAGES.WASHING_STATION
                    Me.EditedValue.CurrentZValue = Me.EditedValue.NewZValue
                    Me.BsAdjustWashing.CurrentValue = Me.EditedValue.CurrentZValue
                    Me.BsAdjustWashing.Enabled = True
                    Me.BsAdjustWashing.EscapeRequest()


                Case ADJUSTMENT_PAGES.ARM_POSITIONS
                    Select Case Me.SelectedArmTab
                        Case ADJUSTMENT_ARMS.SAMPLE
                            Dim send As Object = Nothing
                            Dim myCellEventArgs As DataGridViewCellEventArgs = New DataGridViewCellEventArgs(selectedColumn + 3, SelectedRow)
                            Me.BsGridSample.CellClick(send, myCellEventArgs)
                        Case ADJUSTMENT_ARMS.REAGENT1
                            Dim send As Object = Nothing
                            Dim myCellEventArgs As DataGridViewCellEventArgs = New DataGridViewCellEventArgs(selectedColumn + 3, SelectedRow)
                            Me.BsGridReagent1.CellClick(send, myCellEventArgs)
                        Case ADJUSTMENT_ARMS.REAGENT2
                            Dim send As Object = Nothing
                            Dim myCellEventArgs As DataGridViewCellEventArgs = New DataGridViewCellEventArgs(selectedColumn + 3, SelectedRow)
                            Me.BsGridReagent2.CellClick(send, myCellEventArgs)
                        Case ADJUSTMENT_ARMS.MIXER1
                            Dim send As Object = Nothing
                            Dim myCellEventArgs As DataGridViewCellEventArgs = New DataGridViewCellEventArgs(selectedColumn + 3, SelectedRow)
                            Me.BsGridMixer1.CellClick(send, myCellEventArgs)
                        Case ADJUSTMENT_ARMS.MIXER2
                            Dim send As Object = Nothing
                            Dim myCellEventArgs As DataGridViewCellEventArgs = New DataGridViewCellEventArgs(selectedColumn + 3, SelectedRow)
                            Me.BsGridMixer2.CellClick(send, myCellEventArgs)
                    End Select

                    DeactivateUnusedRowCells(SelectedRow)
                    DeactivateUnusedAdjustControls()

                    ManageTabArms = False

            End Select

            Me.ChangedValue = True
            If Me.SelectedPage <> ADJUSTMENT_PAGES.OPTIC_CENTERING Then
                MyBase.myScreenLayout.ButtonsPanel.AdjustButton.Enabled = False
            End If

            If Me.SelectedPage = ADJUSTMENT_PAGES.OPTIC_CENTERING Then
                MyBase.myScreenLayout.ButtonsPanel.SaveButton.Enabled = Not EncoderOutOfRange
            ElseIf Me.SelectedPage <> ADJUSTMENT_PAGES.ARM_POSITIONS Then
                MyBase.myScreenLayout.ButtonsPanel.SaveButton.Enabled = True
            End If

            MyBase.myScreenLayout.ButtonsPanel.ExitButton.Enabled = True
            MyBase.myScreenLayout.ButtonsPanel.CancelButton.Enabled = True

            ManageTabPages = False

            Me.Enabled = True

            MyBase.ActivateMDIMenusButtons(True) 'SGM 28/09/2011

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareAdjustedMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareAdjustedMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        Finally
            Me.Cursor = Cursors.Default
        End Try
    End Sub

    ''' <summary>
    ''' Prepare GUI for AdjustExiting Mode
    ''' </summary>
    ''' <remarks>Created by XBC 14/01/2011</remarks>
    Private Sub PrepareAdjustExitingMode()
        Try
            Me.Cursor = Cursors.WaitCursor

            With Me.EditedValue
                If .canMovePolarValue Then
                    .CurrentPolarValue = .LastPolarValue
                    .NewPolarValue = 0
                End If
                If .canMoveZValue Then
                    .CurrentZValue = .LastZValue
                    .NewZValue = 0
                End If
                If .canMoveRotorValue Then
                    .CurrentRotorValue = .LastRotorValue
                    .NewRotorValue = 0

                    .NewEncoderValue = 0
                End If
            End With

            'SGM 24/01/11
            Select Case Me.SelectedPage
                Case ADJUSTMENT_PAGES.OPTIC_CENTERING
                    If Not MyBase.SimulationMode Then DisableAll()
                Case ADJUSTMENT_PAGES.WASHING_STATION
                    If Not MyBase.SimulationMode Then DisableAll()
                Case ADJUSTMENT_PAGES.ARM_POSITIONS
                    DisableAll()    ' XBC 09/01/2012
                    Me.ActivateRowCells(Me.SelectedRow, False)
            End Select

            Me.ChangedValue = False

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareAdjustExitingMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareAdjustExitingMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Prepare GUI for Saving Mode
    ''' </summary>
    ''' <remarks>Created by XBC 14/01/2011</remarks>
    Private Sub PrepareSavingMode()
        Try
            Me.Cursor = Cursors.WaitCursor

            If Me.SelectedPage = ADJUSTMENT_PAGES.OPTIC_CENTERING Or Me.SelectedPage = ADJUSTMENT_PAGES.WASHING_STATION Then
                If MyBase.SimulationMode Then

                    ' simulating
                    MyBase.DisplayMessage(Messages.SRV_SAVE_ADJUSTMENTS.ToString)

                    Thread.Sleep(SimulationProcessTime)
                    MyBase.myServiceMDI.Focus()
                    Me.Cursor = Cursors.Default


                    MyBase.DisplayMessage(Messages.SRV_ADJUSTMENTS_SAVED.ToString)

                    myScreenDelegate.LoadAdjDone = True
                    myScreenDelegate.ParkDone = True

                    MyBase.CurrentMode = ADJUSTMENT_MODES.SAVED
                    Me.PrepareArea()
                End If
            End If

            ' XBC 21/05/2012 - grid buttons must be Disabled while saving
            Select Case Me.SelectedPage
                Case ADJUSTMENT_PAGES.ARM_POSITIONS
                    ActivateAdjustButtons(False)
                    ActivateCheckButtons(False)
            End Select
            ' XBC 21/05/2012 

            DisableAll()

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareSavingMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareSavingMode", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Prepare GUI and Sends a Command to perform a Fine Optical Centering Performing Mode
    ''' </summary>
    ''' <remarks>
    ''' Created by XB 15/10/2014 - Sends a Command to perform a fine optical centering - BA-2004
    ''' </remarks>
    Private Sub PrepareFineOpticalCenteringPerformingMode()
        Dim myResultData As New GlobalDataTO
        Try
            myResultData = MyBase.FineOpticalCentering
            If myResultData.HasError Then
                PrepareErrorMode()
                Exit Sub
            Else
                ' Sending fck command
                If MyBase.SimulationMode Then
                    ' simulating
                    Me.Cursor = Cursors.WaitCursor
                    Thread.Sleep(SimulationProcessTime)
                    MyBase.CurrentMode = ADJUSTMENT_MODES.FINE_OPTICAL_CENTERING_DONE
                    MyBase.myServiceMDI.Focus()
                    Me.Cursor = Cursors.Default
                    Me.PrepareSavedMode()
                Else
                    ' Manage FwScripts must to be sent to parking
                    Me.SendFwScript(Me.CurrentMode, EditedValue.AdjustmentID)
                    Me.Cursor = Cursors.WaitCursor
                End If
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareFineOpticalCenteringPerformingMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareFineOpticalCenteringPerformingMode", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Prepare GUI for Saved Mode
    ''' </summary>
    ''' <remarks>Created by XBC 14/01/2011</remarks>
    Private Sub PrepareSavedMode()
        Dim myGlobal As New GlobalDataTO
        Dim myResultData As New GlobalDataTO
        Try


            If myScreenDelegate.LoadAdjDone And Not myScreenDelegate.ParkDone Then
                ' Adjustments already saved into the Instrument !

                ' Continue with the Test until finish it
                myResultData = MyBase.Save
                If myResultData.HasError Then
                    PrepareErrorMode()
                    Exit Sub
                Else
                    ' Sending park operation 
                    If MyBase.SimulationMode Then
                        ' simulating
                        Me.Cursor = Cursors.WaitCursor
                        Thread.Sleep(SimulationProcessTime)
                        MyBase.myServiceMDI.Focus()
                        Me.Cursor = Cursors.Default
                        myScreenDelegate.LoadAdjDone = True
                        myScreenDelegate.ParkDone = True
                    Else
                        ' Manage FwScripts must to be sent to parking
                        Me.SendFwScript(Me.CurrentMode, EditedValue.AdjustmentID)
                    End If
                End If
            ElseIf myScreenDelegate.ParkDone Then
                ' Last operation after adjustments load (park operation) is done !
                With Me.EditedValue
                    Select Case Me.SelectedPage
                        Case ADJUSTMENT_PAGES.OPTIC_CENTERING
                            If .canSaveRotorValue Then
                                .LastRotorValue = .NewRotorValue
                                Me.BsOpticAdjustmentLabel.Text = .NewRotorValue.ToString
                                Me.UpdateSpecificAdjustmentsDS(ReadSpecificAdjustmentData(GlobalEnumerates.AXIS.ROTOR).CodeFw, .NewRotorValue.ToString)
                                .NewRotorValue = 0

                                ' XBC 03/01/2012 - Add Encoder functionality
                                .LastEncoderValue = .NewEncoderValue
                                Me.BsEncoderAdjustmentLabel.Text = .NewEncoderValue.ToString
                                Me.UpdateSpecificAdjustmentsDS(ReadSpecificAdjustmentData(GlobalEnumerates.AXIS.ENCODER).CodeFw, .NewEncoderValue.ToString)
                                .NewEncoderValue = 0

                                Me.BsLEDCurrentTrackBar.Enabled = True
                                Me.ReportHistory(HISTORY_TASKS.ADJUSTMENT, PositionsAdjustmentDelegate.HISTORY_RESULTS.OK) 'History SGM 02/08/2011

                            End If
                            ' XBC 30/11/2011
                            Me.BsUpDownWSButton1.Enabled = True

                        Case ADJUSTMENT_PAGES.WASHING_STATION
                            'SGM 24/01/11
                            If .canSaveZValue Then
                                .LastZValue = .NewZValue
                                Me.BsWashingAdjustmentLabel.Text = (.NewZValue).ToString
                                UpdateSpecificAdjustmentsDS(ReadSpecificAdjustmentData(GlobalEnumerates.AXIS.Z).CodeFw, (.NewZValue).ToString)
                                .NewZValue = 0

                                WashingStationValidated = True
                                Me.ReportHistory(HISTORY_TASKS.ADJUSTMENT, PositionsAdjustmentDelegate.HISTORY_RESULTS.OK) 'History SGM 02/08/2011

                            End If
                            ' XBC 30/11/2011
                            Me.BsUpDownWSButton2.Enabled = True

                        Case ADJUSTMENT_PAGES.ARM_POSITIONS

                            myScreenDelegate.ManageArmsParkingStatus(.AdjustmentID, True)

                            MyBase.DisplayMessage("")
                            If MyClass.IsLocallySaved Then

                                MyBase.DisplayMessage(Messages.SRV_ADJUSTMENTS_LOCAL_SAVED.ToString)

                                If .canSavePolarValue Then
                                    .LastPolarValue = .NewPolarValue
                                    Select Case Me.SelectedArmTab
                                        Case ADJUSTMENT_ARMS.SAMPLE
                                            Me.BsGridSample.ParameterCellValue(SelectedRow, POLAR_COLUMN) = .NewPolarValue.ToString
                                        Case ADJUSTMENT_ARMS.REAGENT1
                                            Me.BsGridReagent1.ParameterCellValue(SelectedRow, POLAR_COLUMN) = .NewPolarValue.ToString
                                        Case ADJUSTMENT_ARMS.REAGENT2
                                            Me.BsGridReagent2.ParameterCellValue(SelectedRow, POLAR_COLUMN) = .NewPolarValue.ToString
                                        Case ADJUSTMENT_ARMS.MIXER1
                                            Me.BsGridMixer1.ParameterCellValue(SelectedRow, POLAR_COLUMN) = .NewPolarValue.ToString
                                        Case ADJUSTMENT_ARMS.MIXER2
                                            Me.BsGridMixer2.ParameterCellValue(SelectedRow, POLAR_COLUMN) = .NewPolarValue.ToString
                                    End Select
                                    UpdateSpecificAdjustmentsDS(ReadSpecificAdjustmentData(GlobalEnumerates.AXIS.POLAR).CodeFw, .NewPolarValue.ToString)
                                    .NewPolarValue = 0
                                End If
                                If .canSaveZValue Then
                                    .LastZValue = .NewZValue
                                    Select Case Me.SelectedArmTab
                                        Case ADJUSTMENT_ARMS.SAMPLE
                                            Me.BsGridSample.ParameterCellValue(SelectedRow, Z_COLUMN) = .NewZValue.ToString
                                        Case ADJUSTMENT_ARMS.REAGENT1
                                            Me.BsGridReagent1.ParameterCellValue(SelectedRow, Z_COLUMN) = .NewZValue.ToString
                                        Case ADJUSTMENT_ARMS.REAGENT2
                                            Me.BsGridReagent2.ParameterCellValue(SelectedRow, Z_COLUMN) = .NewZValue.ToString
                                        Case ADJUSTMENT_ARMS.MIXER1
                                            Me.BsGridMixer1.ParameterCellValue(SelectedRow, Z_COLUMN) = .NewZValue.ToString
                                        Case ADJUSTMENT_ARMS.MIXER2
                                            Me.BsGridMixer2.ParameterCellValue(SelectedRow, Z_COLUMN) = .NewZValue.ToString
                                    End Select
                                    UpdateSpecificAdjustmentsDS(ReadSpecificAdjustmentData(GlobalEnumerates.AXIS.Z).CodeFw, .NewZValue.ToString)
                                    .NewZValue = 0
                                End If
                                If .canSaveRotorValue Then
                                    .LastRotorValue = .NewRotorValue
                                    Select Case Me.SelectedArmTab
                                        Case ADJUSTMENT_ARMS.SAMPLE
                                            Me.BsGridSample.ParameterCellValue(SelectedRow, ROTOR_COLUMN) = .NewRotorValue.ToString
                                        Case ADJUSTMENT_ARMS.REAGENT1
                                            Me.BsGridReagent1.ParameterCellValue(SelectedRow, ROTOR_COLUMN) = .NewRotorValue.ToString
                                        Case ADJUSTMENT_ARMS.REAGENT2
                                            Me.BsGridReagent2.ParameterCellValue(SelectedRow, ROTOR_COLUMN) = .NewRotorValue.ToString
                                        Case ADJUSTMENT_ARMS.MIXER1
                                            Me.BsGridMixer1.ParameterCellValue(SelectedRow, ROTOR_COLUMN) = .NewRotorValue.ToString
                                        Case ADJUSTMENT_ARMS.MIXER2
                                            Me.BsGridMixer2.ParameterCellValue(SelectedRow, ROTOR_COLUMN) = .NewRotorValue.ToString
                                    End Select
                                    UpdateSpecificAdjustmentsDS(ReadSpecificAdjustmentData(GlobalEnumerates.AXIS.ROTOR).CodeFw, .NewRotorValue.ToString)
                                    .NewRotorValue = 0
                                End If

                                Me.ActivateRowCheckButton()
                                Me.ActivateRowCells(Me.SelectedRow, False)
                                Me.ShowIconValidate(Me.SelectedRow, True)

                                ' XBC 17-03-2011 - Wizard Mode 
                                If Me.WizardMode Then
                                    GoToNextWizardMode()
                                Else
                                    Me.ActivateAdjustButtons(True)
                                    Me.BsUpDownWSButton3.Enabled = True
                                End If

                            End If




                    End Select

                    'SGM 24/01/11


                End With

                If Not MyClass.IsLocallySaved Then
                    myGlobal = MyBase.UpdateAdjustments(Me.LocalSavedAdjustmentsDS) 'SGM 28/02/2012
                    If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then

                        MyBase.DisplayMessage(Messages.SRV_ADJUSTMENTS_SAVED.ToString)

                        'report to history
                        Me.ReportHistory(HISTORY_TASKS.ADJUSTMENT, PositionsAdjustmentDelegate.HISTORY_RESULTS.OK) 'History SGM 02/08/2011

                        'clear local saved dataset
                        Me.LocalSavedAdjustmentsDS.Clear()

                        'set changes made to false
                        Me.ChangedValue = False

                        If Me.SelectedPage = ADJUSTMENT_PAGES.ARM_POSITIONS Then
                            Me.BsGridSample.HideAllIconsOk()
                            Me.BsGridReagent1.HideAllIconsOk()
                            Me.BsGridReagent2.HideAllIconsOk()
                            Me.BsGridMixer1.HideAllIconsOk()
                            Me.BsGridMixer2.HideAllIconsOk()

                            Me.ActivateRowCheckButton()
                            Me.ActivateRowCells(Me.SelectedRow, False)

                            Me.ActivateAdjustButtons(True)

                            Me.BsUpDownWSButton3.Enabled = True
                        End If

                        'allow change tab
                        Me.ManageTabPages = True
                        MyBase.ActivateMDIMenusButtons(True) 'SGM 28/09/2011

                    Else
                        PrepareErrorMode()
                        Exit Sub
                    End If
                End If

                Me.FillAdjustmentValuesintoDelegate()

                Me.InitializeAdjustControls()

                'set changes made to false
                Me.ChangedValue = False


                MyBase.myScreenLayout.ButtonsPanel.CancelButton.Enabled = False
                If Me.SelectedPage = ADJUSTMENT_PAGES.ARM_POSITIONS Then
                    Me.BsArmsOkButton.Enabled = False
                    Me.BsSaveButton.Enabled = MyClass.IsLocallySaved

                    Me.BsGridSample.Enabled = True
                    Me.BsGridReagent1.Enabled = True
                    Me.BsGridReagent2.Enabled = True
                    Me.BsGridMixer1.Enabled = True
                    Me.BsGridMixer2.Enabled = True

                    Me.ManageTabArms = Not MyClass.IsLocallySaved

                Else
                    MyBase.myScreenLayout.ButtonsPanel.AdjustButton.Enabled = True
                End If

                Me.BsExitButton.Enabled = True

                ' XBC 07/12/2011
                If Me.WaitForScriptsExitingScreen Then
                    Me.FinishExitScreen()
                End If

            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareSavedMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareSavedMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        Finally
            Me.Cursor = Cursors.Default
        End Try
    End Sub

    ''' <summary>
    ''' Prepare GUI for Testing Mode
    ''' </summary>
    ''' <remarks>Created by XBC 14/01/2011</remarks>
    Private Sub PrepareTestingMode()
        Try
            Me.Cursor = Cursors.WaitCursor

            ActivateAdjustButtons(False)
            ActivateCheckButtons(False)
            ManageTabArms = False
            ManageTabPages = False

            MyBase.myScreenLayout.ButtonsPanel.AdjustButton.Enabled = False
            MyBase.myScreenLayout.ButtonsPanel.SaveButton.Enabled = False
            MyBase.myScreenLayout.ButtonsPanel.ExitButton.Enabled = True
            MyBase.myScreenLayout.ButtonsPanel.CancelButton.Enabled = False

            MyBase.ActivateMDIMenusButtons(False) 'SGM 28/09/2011

            ' XBC 30/11/2011
            Me.BsUpDownWSButton3.Enabled = False

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareTestingMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareTestingMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
            Me.ReportHistoryError()
        End Try
    End Sub

    ''' <summary>
    ''' Prepare GUI for Tested Mode
    ''' </summary>
    ''' <remarks>Created by XBC 14/01/2011</remarks>
    Private Sub PrepareTestedMode()
        Try
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate
            Select Case SelectedArmTab
                Case ADJUSTMENT_ARMS.SAMPLE
                    Me.BsGridSample.NameColumnButton2ByRow(Me.SelectedRow) = myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_SRV_EXIT_TEST", currentLanguage)
                Case ADJUSTMENT_ARMS.REAGENT1
                    Me.BsGridReagent1.NameColumnButton2ByRow(Me.SelectedRow) = myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_SRV_EXIT_TEST", currentLanguage)
                Case ADJUSTMENT_ARMS.REAGENT2
                    Me.BsGridReagent2.NameColumnButton2ByRow(Me.SelectedRow) = myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_SRV_EXIT_TEST", currentLanguage)
                Case ADJUSTMENT_ARMS.MIXER1
                    Me.BsGridMixer1.NameColumnButton2ByRow(Me.SelectedRow) = myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_SRV_EXIT_TEST", currentLanguage)
                Case ADJUSTMENT_ARMS.MIXER2
                    Me.BsGridMixer2.NameColumnButton2ByRow(Me.SelectedRow) = myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_SRV_EXIT_TEST", currentLanguage)
            End Select
            Me.ActivateCheckButton(Me.SelectedRow, True)

            Me.ReportHistory(HISTORY_TASKS.TEST, PositionsAdjustmentDelegate.HISTORY_RESULTS.OK)

            MyBase.DisplayMessage(Messages.SRV_TEST_COMPLETED.ToString)

            MyBase.myScreenLayout.ButtonsPanel.AdjustButton.Enabled = False
            MyBase.myScreenLayout.ButtonsPanel.SaveButton.Enabled = False
            MyBase.myScreenLayout.ButtonsPanel.ExitButton.Enabled = False
            MyBase.myScreenLayout.ButtonsPanel.CancelButton.Enabled = False

            'mixer buttons
            Select Case Me.SelectedArmTab
                Case ADJUSTMENT_ARMS.MIXER1
                    Me.PrepareStirrerButton(Me.BsStirrer1Button)
                Case ADJUSTMENT_ARMS.MIXER2
                    Me.PrepareStirrerButton(Me.BsStirrer2Button)
            End Select

            Me.IsWaitingExitTest = True ' XBC 16/11/2011

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareTestedMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareTestedMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
            Me.ReportHistoryError()
        Finally
            Me.Cursor = Cursors.Default
        End Try
    End Sub

    ''' <summary>
    ''' Prepare GUI for Exiting Test Mode
    ''' </summary>
    ''' <remarks>Created by XBC 14/01/2011</remarks>
    Private Sub PrepareTestExitingMode()
        Try
            Me.Cursor = Cursors.WaitCursor

            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate
            ActivateCheckButton(Me.SelectedRow, False)
            Select Case SelectedArmTab
                Case ADJUSTMENT_ARMS.SAMPLE
                    Me.BsGridSample.NameColumnButton2ByRow(Me.SelectedRow) = myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_SRV_EXIT_TEST", currentLanguage)
                Case ADJUSTMENT_ARMS.REAGENT1
                    Me.BsGridReagent1.NameColumnButton2ByRow(Me.SelectedRow) = myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_SRV_EXIT_TEST", currentLanguage)
                Case ADJUSTMENT_ARMS.REAGENT2
                    Me.BsGridReagent2.NameColumnButton2ByRow(Me.SelectedRow) = myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_SRV_EXIT_TEST", currentLanguage)
                Case ADJUSTMENT_ARMS.MIXER1
                    Me.BsGridMixer1.NameColumnButton2ByRow(Me.SelectedRow) = myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_SRV_EXIT_TEST", currentLanguage)
                Case ADJUSTMENT_ARMS.MIXER2
                    Me.BsGridMixer2.NameColumnButton2ByRow(Me.SelectedRow) = myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_SRV_EXIT_TEST", currentLanguage)
            End Select

            MyBase.myScreenLayout.ButtonsPanel.AdjustButton.Enabled = False
            MyBase.myScreenLayout.ButtonsPanel.SaveButton.Enabled = False
            MyBase.myScreenLayout.ButtonsPanel.ExitButton.Enabled = False
            MyBase.myScreenLayout.ButtonsPanel.CancelButton.Enabled = False

            ManageTabArms = False
            ManageTabPages = False

            MyBase.ActivateMDIMenusButtons(False) 'SGM 28/09/2011

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareTestExitingMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareTestExitingMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Prepare GUI for Exited Test Mode
    ''' </summary>
    ''' <remarks>Created by XBC 14/01/2011</remarks>
    Private Sub PrepareTestExitedMode()
        Try
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate
            ActivateCheckButton(Me.SelectedRow, False)
            Select Case SelectedArmTab
                Case ADJUSTMENT_ARMS.SAMPLE
                    Me.BsGridSample.NameColumnButton2ByRow(Me.SelectedRow) = myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_SRV_TEST", currentLanguage)
                Case ADJUSTMENT_ARMS.REAGENT1
                    Me.BsGridReagent1.NameColumnButton2ByRow(Me.SelectedRow) = myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_SRV_TEST", currentLanguage)
                Case ADJUSTMENT_ARMS.REAGENT2
                    Me.BsGridReagent2.NameColumnButton2ByRow(Me.SelectedRow) = myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_SRV_TEST", currentLanguage)
                Case ADJUSTMENT_ARMS.MIXER1
                    Me.BsGridMixer1.NameColumnButton2ByRow(Me.SelectedRow) = myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_SRV_TEST", currentLanguage)
                Case ADJUSTMENT_ARMS.MIXER2
                    Me.BsGridMixer2.NameColumnButton2ByRow(Me.SelectedRow) = myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_SRV_TEST", currentLanguage)
            End Select

            MyBase.DisplayMessage(Messages.SRV_TEST_EXIT_COMPLETED.ToString)


            If MyBase.CurrentUserNumericalLevel = USER_LEVEL.lOPERATOR Then
                MyBase.myScreenLayout.ButtonsPanel.AdjustButton.Enabled = False
            Else
                MyBase.myScreenLayout.ButtonsPanel.AdjustButton.Enabled = True
            End If

            MyBase.myScreenLayout.ButtonsPanel.SaveButton.Enabled = False
            MyBase.myScreenLayout.ButtonsPanel.ExitButton.Enabled = True
            MyBase.myScreenLayout.ButtonsPanel.CancelButton.Enabled = False

            ManageTabArms = True
            ManageTabPages = True

            MyBase.ActivateMDIMenusButtons(True) 'SGM 28/09/2011

            Me.IsWaitingExitTest = False ' XBC 16/11/2011

            ' XBC 30/11/2011
            Me.BsUpDownWSButton3.Enabled = True

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareTestExitedMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareTestExitedMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        Finally
            Me.Cursor = Cursors.Default
        End Try
    End Sub

    ''' <summary>
    ''' Prepare GUI for Parking Mode
    ''' </summary>
    ''' <remarks>Created by XBC 30/11/2011</remarks>
    Private Sub PrepareParkingMode()
        Try
            Me.Cursor = Cursors.WaitCursor

            ActivateAdjustButtons(False)
            Select Case Me.SelectedPage
                Case ADJUSTMENT_PAGES.OPTIC_CENTERING
                    'Me.BsOpticTestButton.Enabled = False
                Case ADJUSTMENT_PAGES.WASHING_STATION
                    'Me.BsWashingTestButton.Enabled = False
                Case ADJUSTMENT_PAGES.ARM_POSITIONS
                    ActivateCheckButtons(False)
            End Select

            DisableAll()

            ManageTabArms = False
            ManageTabPages = False

            MyBase.ActivateMDIMenusButtons(False)

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareParkingMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareParkingMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
            Me.ReportHistoryError()
        End Try
    End Sub

    ''' <summary>
    ''' Prepare GUI for Parked Mode
    ''' </summary>
    ''' <remarks>Created by XBC 30/11/2011</remarks>
    Private Sub PrepareParkedMode()
        Try
            Me.Cursor = Cursors.Default

            If myScreenDelegate.IsWashingStationUp Then
                Select Case Me.SelectedPage
                    Case ADJUSTMENT_PAGES.OPTIC_CENTERING
                        Me.BsUpDownWSButton1.Enabled = True

                    Case ADJUSTMENT_PAGES.WASHING_STATION
                        Me.BsUpDownWSButton2.Enabled = True

                    Case ADJUSTMENT_PAGES.ARM_POSITIONS
                        Me.BsUpDownWSButton3.Enabled = True

                End Select
            Else
                Me.PrepareLoadedMode()
            End If

            MyBase.DisplayMessage("")

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareParkedMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareParkedMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
            Me.ReportHistoryError()
        End Try
    End Sub



    ''' <summary>
    ''' Loads the icons and tooltips used for painting the buttons
    ''' </summary>
    ''' <remarks>XBC 10/01/2011</remarks>
    Private Sub PrepareButtons()
        Dim auxIconName As String = ""
        Dim iconPath As String = MyBase.IconsPath
        Try
            ' XBC 02/01/2012 - Add Encoder functionality
            Me.BsTabPagesControl.TabPages.Remove(TabPageTODELETE)

            'dl 20/04/2012
            auxIconName = GetIconName("ADJUSTMENT")
            If (auxIconName <> "") Then
                BsOpticAdjustButton.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
                BsWSAdjustButton.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
            End If

            auxIconName = GetIconName("UPDOWN")
            If (auxIconName <> "") Then
                BsUpDownWSButton1.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
                BsUpDownWSButton2.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
                BsUpDownWSButton3.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
            End If

            MyBase.SetButtonImage(BsOpticStopButton, "STOP", 24, 24) 'SGM 09/05/2012

            auxIconName = GetIconName("ESPIRAL")
            If (auxIconName <> "") Then
                BsStirrer1Button.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
                BsStirrer2Button.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
            End If

            auxIconName = GetIconName("UNDO")
            If (auxIconName <> "") Then
                BsOpticCancelButton.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
                BsWSCancelButton.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
                BsArmsCancelButton.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
            End If

            auxIconName = GetIconName("SAVE")
            If (auxIconName <> "") Then
                BsSaveButton.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
            End If

            auxIconName = GetIconName("CANCEL")
            If (auxIconName <> "") Then
                BsExitButton.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
            End If

            auxIconName = GetIconName("ACCEPT1")
            If (auxIconName <> "") Then
                BsArmsOkButton.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
            End If

            auxIconName = GetIconName("ACCEPTF")
            If File.Exists(iconPath & auxIconName) Then
                Dim myImage As Image = Image.FromFile(iconPath & auxIconName)
                myImage = CType(ResizeImage(myImage, New Size(16, 16)).SetDatos, Image)
                Me.BsGridSample.OkImage = myImage
                Me.BsGridReagent1.OkImage = myImage
                Me.BsGridReagent2.OkImage = myImage
                Me.BsGridMixer1.OkImage = myImage
                Me.BsGridMixer2.OkImage = myImage
            End If

            ' Define Form icons
            auxIconName = GetIconName("ACCEPTF")
            If auxIconName <> "" Then
                Me.BsGridSample.ValidationImage = iconPath & auxIconName
                Me.BsGridReagent1.ValidationImage = iconPath & auxIconName
                Me.BsGridReagent2.ValidationImage = iconPath & auxIconName
                Me.BsGridMixer1.ValidationImage = iconPath & auxIconName
                Me.BsGridMixer2.ValidationImage = iconPath & auxIconName
            End If
            auxIconName = GetIconName("FREECELL")
            If auxIconName <> "" Then
                Me.BsGridSample.NoValidationImage = iconPath & auxIconName
                Me.BsGridReagent1.NoValidationImage = iconPath & auxIconName
                Me.BsGridReagent2.NoValidationImage = iconPath & auxIconName
                Me.BsGridMixer1.NoValidationImage = iconPath & auxIconName
                Me.BsGridMixer2.NoValidationImage = iconPath & auxIconName
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareButtons", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareButtons", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Method incharge to get the controls limits value. 
    ''' </summary>
    ''' <param name="pLimitsID">Limit to get</param>
    ''' <returns></returns>
    ''' <remarks>Created by XBC 05/01/2011</remarks>
    Private Function GetControlsLimits(ByVal pLimitsID As FieldLimitsEnum) As GlobalDataTO
        Dim myGlobalDataTO As New GlobalDataTO

        Try
            Dim myFieldLimitsDelegate As New FieldLimitsDelegate()
            'Load the specified limits
            myGlobalDataTO = myFieldLimitsDelegate.GetList(Nothing, pLimitsID, AnalyzerModel)

        Catch ex As Exception
            'Write error SYSTEM_ERROR in the Application Log
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & " GetControlsLimits ", EventLogEntryType.Error, _
                                                            GetApplicationInfoSession().ActivateSystemLog)
            'Show error message
            MyBase.ShowMessage(Me.Name & " GetControlsLimits ", Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
        Return myGlobalDataTO
    End Function

    ''' <summary>
    ''' Get Limits values from BD for every different arm
    ''' </summary>
    ''' <remarks>Created by XBC 05/01/2011</remarks>
    Private Function GetLimitValues() As GlobalDataTO
        Dim myGlobalDataTO As New GlobalDataTO
        Try
            ' Get Value limit ranges
            Dim myFieldLimitsDS As New FieldLimitsDS

            'SGX pages functionality
            Select Case Me.SelectedPage
                Case ADJUSTMENT_PAGES.OPTIC_CENTERING
                    myGlobalDataTO = GetControlsLimits(FieldLimitsEnum.SRV_REACTIONS_ROTOR_LIMIT)
                    If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                        myFieldLimitsDS = CType(myGlobalDataTO.SetDatos, FieldLimitsDS)
                        If myFieldLimitsDS.tfmwFieldLimits.Rows.Count > 0 Then
                            Me.LimitMinRotor = CType(myFieldLimitsDS.tfmwFieldLimits(0).MinValue, Decimal)
                            Me.LimitMaxRotor = CType(myFieldLimitsDS.tfmwFieldLimits(0).MaxValue, Decimal)
                        End If

                        ' Configuring Optic Adjust Control 
                        Me.BsAdjustOptic.MinimumLimit = Me.LimitMinRotor
                        Me.BsAdjustOptic.MaximumLimit = Me.LimitMaxRotor
                        Me.BsAdjustOptic.MaxNumDecimals = 0
                        Me.BsAdjustOptic.CurrentStepValue = 1
                    Else
                        MyBase.ShowMessage(Me.Name & ".GetLimitValues", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage, Me)
                    End If

                    'Led Current limits
                    myGlobalDataTO = GetControlsLimits(FieldLimitsEnum.SRV_LEDS_LIMIT)
                    If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                        myFieldLimitsDS = CType(myGlobalDataTO.SetDatos, FieldLimitsDS)
                        If myFieldLimitsDS.tfmwFieldLimits.Rows.Count > 0 Then
                            Me.LedCurrentMin = CType(myFieldLimitsDS.tfmwFieldLimits(0).MinValue, Integer)
                            Me.LedCurrentMax = CType(myFieldLimitsDS.tfmwFieldLimits(0).MaxValue, Integer)
                        End If

                        ' Configuring Led Current Control 
                        Me.BsLEDCurrentTrackBar.Minimum = Me.LedCurrentMin
                        Me.BsLEDCurrentTrackBar.Maximum = Me.LedCurrentMax
                        Me.BsLEDCurrentTrackBar.SmallChange = 1
                        Me.BsLEDCurrentTrackBar.LargeChange = OpticalCenteringStepLed

                        Me.LEDCurrent = OpticalCenteringCurrentLed
                        Me.BsLEDCurrentTrackBar.Value = Me.LEDCurrent

                    Else
                        MyBase.ShowMessage(Me.Name & ".GetLimitValues", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage, Me)
                    End If

                    ' XBC 03/01/2012 - Add Encoder functionality
                    'Encoder Adjustment limits
                    myGlobalDataTO = GetControlsLimits(FieldLimitsEnum.SRV_ENCODER_LIMIT)
                    If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                        myFieldLimitsDS = CType(myGlobalDataTO.SetDatos, FieldLimitsDS)
                        If myFieldLimitsDS.tfmwFieldLimits.Rows.Count > 0 Then
                            Me.LimitMinEncoder = CType(myFieldLimitsDS.tfmwFieldLimits(0).MinValue, Integer)
                            Me.LimitMaxEncoder = CType(myFieldLimitsDS.tfmwFieldLimits(0).MaxValue, Integer)
                        End If
                    Else
                        MyBase.ShowMessage(Me.Name & ".GetLimitValues", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage, Me)
                    End If
                    ' XBC 03/01/2012 - Add Encoder functionality

                Case ADJUSTMENT_PAGES.WASHING_STATION
                    'SGM 24/01/11
                    myGlobalDataTO = GetControlsLimits(FieldLimitsEnum.SRV_WASHING_Z_LIMIT)
                    If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                        myFieldLimitsDS = CType(myGlobalDataTO.SetDatos, FieldLimitsDS)
                        If myFieldLimitsDS.tfmwFieldLimits.Rows.Count > 0 Then
                            Me.LimitMinZ = CType(myFieldLimitsDS.tfmwFieldLimits(0).MinValue, Decimal)
                            Me.LimitMaxZ = CType(myFieldLimitsDS.tfmwFieldLimits(0).MaxValue, Decimal)
                        End If

                        ' Configuring Washing Adjust Control 
                        Me.BsAdjustWashing.MinimumLimit = Me.LimitMinZ
                        Me.BsAdjustWashing.MaximumLimit = Me.LimitMaxZ
                        Me.BsAdjustWashing.MaxNumDecimals = 0
                        Me.BsAdjustWashing.CurrentStepValue = 1
                    Else
                        MyBase.ShowMessage(Me.Name & ".GetLimitValues", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage, Me)
                    End If


                Case ADJUSTMENT_PAGES.ARM_POSITIONS
                    Select Case Me.SelectedArmTab
                        Case ADJUSTMENT_ARMS.SAMPLE
                            ' Polar 
                            myGlobalDataTO = GetControlsLimits(FieldLimitsEnum.SRV_SAMPLE_POLAR_LIMIT)
                            If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                                myFieldLimitsDS = CType(myGlobalDataTO.SetDatos, FieldLimitsDS)
                                If myFieldLimitsDS.tfmwFieldLimits.Rows.Count > 0 Then
                                    Me.LimitMinPolar = CType(myFieldLimitsDS.tfmwFieldLimits(0).MinValue, Decimal)
                                    Me.LimitMaxPolar = CType(myFieldLimitsDS.tfmwFieldLimits(0).MaxValue, Decimal)
                                End If
                            Else
                                MyBase.ShowMessage(Me.Name & ".GetLimitValues", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage, Me)
                            End If
                            ' Z 
                            myGlobalDataTO = GetControlsLimits(FieldLimitsEnum.SRV_SAMPLE_Z_LIMIT)
                            If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                                myFieldLimitsDS = CType(myGlobalDataTO.SetDatos, FieldLimitsDS)
                                If myFieldLimitsDS.tfmwFieldLimits.Rows.Count > 0 Then
                                    Me.LimitMinZ = CType(myFieldLimitsDS.tfmwFieldLimits(0).MinValue, Decimal)
                                    Me.LimitMaxZ = CType(myFieldLimitsDS.tfmwFieldLimits(0).MaxValue, Decimal)
                                End If
                            Else
                                MyBase.ShowMessage(Me.Name & ".GetLimitValues", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage, Me)
                            End If
                            ' Rotor 
                            myGlobalDataTO = GetControlsLimits(FieldLimitsEnum.SRV_SAMPLE_ROTOR_LIMIT)
                            If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                                myFieldLimitsDS = CType(myGlobalDataTO.SetDatos, FieldLimitsDS)
                                If myFieldLimitsDS.tfmwFieldLimits.Rows.Count > 0 Then
                                    Me.LimitMinRotor = CType(myFieldLimitsDS.tfmwFieldLimits(0).MinValue, Decimal)
                                    Me.LimitMaxRotor = CType(myFieldLimitsDS.tfmwFieldLimits(0).MaxValue, Decimal)
                                End If
                            Else
                                MyBase.ShowMessage(Me.Name & ".GetLimitValues", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage, Me)
                            End If
                        Case ADJUSTMENT_ARMS.REAGENT1
                            ' Polar 
                            myGlobalDataTO = GetControlsLimits(FieldLimitsEnum.SRV_REAGENT1_POLAR_LIMIT)
                            If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                                myFieldLimitsDS = CType(myGlobalDataTO.SetDatos, FieldLimitsDS)
                                If myFieldLimitsDS.tfmwFieldLimits.Rows.Count > 0 Then
                                    Me.LimitMinPolar = CType(myFieldLimitsDS.tfmwFieldLimits(0).MinValue, Decimal)
                                    Me.LimitMaxPolar = CType(myFieldLimitsDS.tfmwFieldLimits(0).MaxValue, Decimal)
                                End If
                            Else
                                MyBase.ShowMessage(Me.Name & ".GetLimitValues", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage, Me)
                            End If
                            ' Z 
                            myGlobalDataTO = GetControlsLimits(FieldLimitsEnum.SRV_REAGENT1_Z_LIMIT)
                            If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                                myFieldLimitsDS = CType(myGlobalDataTO.SetDatos, FieldLimitsDS)
                                If myFieldLimitsDS.tfmwFieldLimits.Rows.Count > 0 Then
                                    Me.LimitMinZ = CType(myFieldLimitsDS.tfmwFieldLimits(0).MinValue, Decimal)
                                    Me.LimitMaxZ = CType(myFieldLimitsDS.tfmwFieldLimits(0).MaxValue, Decimal)
                                End If
                            Else
                                MyBase.ShowMessage(Me.Name & ".GetLimitValues", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage, Me)
                            End If
                            ' Rotor 
                            myGlobalDataTO = GetControlsLimits(FieldLimitsEnum.SRV_REAGENT_ROTOR_LIMIT)
                            If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                                myFieldLimitsDS = CType(myGlobalDataTO.SetDatos, FieldLimitsDS)
                                If myFieldLimitsDS.tfmwFieldLimits.Rows.Count > 0 Then
                                    Me.LimitMinRotor = CType(myFieldLimitsDS.tfmwFieldLimits(0).MinValue, Decimal)
                                    Me.LimitMaxRotor = CType(myFieldLimitsDS.tfmwFieldLimits(0).MaxValue, Decimal)
                                End If
                            Else
                                MyBase.ShowMessage(Me.Name & ".GetLimitValues", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage, Me)
                            End If
                        Case ADJUSTMENT_ARMS.REAGENT2
                            ' Polar 
                            myGlobalDataTO = GetControlsLimits(FieldLimitsEnum.SRV_REAGENT2_POLAR_LIMIT)
                            If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                                myFieldLimitsDS = CType(myGlobalDataTO.SetDatos, FieldLimitsDS)
                                If myFieldLimitsDS.tfmwFieldLimits.Rows.Count > 0 Then
                                    Me.LimitMinPolar = CType(myFieldLimitsDS.tfmwFieldLimits(0).MinValue, Decimal)
                                    Me.LimitMaxPolar = CType(myFieldLimitsDS.tfmwFieldLimits(0).MaxValue, Decimal)
                                End If
                            Else
                                MyBase.ShowMessage(Me.Name & ".GetLimitValues", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage, Me)
                            End If
                            ' Z 
                            myGlobalDataTO = GetControlsLimits(FieldLimitsEnum.SRV_REAGENT2_Z_LIMIT)
                            If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                                myFieldLimitsDS = CType(myGlobalDataTO.SetDatos, FieldLimitsDS)
                                If myFieldLimitsDS.tfmwFieldLimits.Rows.Count > 0 Then
                                    Me.LimitMinZ = CType(myFieldLimitsDS.tfmwFieldLimits(0).MinValue, Decimal)
                                    Me.LimitMaxZ = CType(myFieldLimitsDS.tfmwFieldLimits(0).MaxValue, Decimal)
                                End If
                            Else
                                MyBase.ShowMessage(Me.Name & ".GetLimitValues", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage, Me)
                            End If
                            ' Rotor 
                            myGlobalDataTO = GetControlsLimits(FieldLimitsEnum.SRV_REAGENT_ROTOR_LIMIT)
                            If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                                myFieldLimitsDS = CType(myGlobalDataTO.SetDatos, FieldLimitsDS)
                                If myFieldLimitsDS.tfmwFieldLimits.Rows.Count > 0 Then
                                    Me.LimitMinRotor = CType(myFieldLimitsDS.tfmwFieldLimits(0).MinValue, Decimal)
                                    Me.LimitMaxRotor = CType(myFieldLimitsDS.tfmwFieldLimits(0).MaxValue, Decimal)
                                End If
                            Else
                                MyBase.ShowMessage(Me.Name & ".GetLimitValues", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage, Me)
                            End If
                        Case ADJUSTMENT_ARMS.MIXER1
                            ' Polar 
                            myGlobalDataTO = GetControlsLimits(FieldLimitsEnum.SRV_MIXER1_POLAR_LIMIT)
                            If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                                myFieldLimitsDS = CType(myGlobalDataTO.SetDatos, FieldLimitsDS)
                                If myFieldLimitsDS.tfmwFieldLimits.Rows.Count > 0 Then
                                    Me.LimitMinPolar = CType(myFieldLimitsDS.tfmwFieldLimits(0).MinValue, Decimal)
                                    Me.LimitMaxPolar = CType(myFieldLimitsDS.tfmwFieldLimits(0).MaxValue, Decimal)
                                End If
                            Else
                                MyBase.ShowMessage(Me.Name & ".GetLimitValues", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage, Me)
                            End If
                            ' Z 
                            myGlobalDataTO = GetControlsLimits(FieldLimitsEnum.SRV_MIXER1_Z_LIMIT)
                            If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                                myFieldLimitsDS = CType(myGlobalDataTO.SetDatos, FieldLimitsDS)
                                If myFieldLimitsDS.tfmwFieldLimits.Rows.Count > 0 Then
                                    Me.LimitMinZ = CType(myFieldLimitsDS.tfmwFieldLimits(0).MinValue, Decimal)
                                    Me.LimitMaxZ = CType(myFieldLimitsDS.tfmwFieldLimits(0).MaxValue, Decimal)
                                End If
                            Else
                                MyBase.ShowMessage(Me.Name & ".GetLimitValues", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage, Me)
                            End If
                            ' Rotor 
                            myGlobalDataTO = GetControlsLimits(FieldLimitsEnum.SRV_MIXER1_ROTOR_LIMIT)
                            If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                                myFieldLimitsDS = CType(myGlobalDataTO.SetDatos, FieldLimitsDS)
                                If myFieldLimitsDS.tfmwFieldLimits.Rows.Count > 0 Then
                                    Me.LimitMinRotor = CType(myFieldLimitsDS.tfmwFieldLimits(0).MinValue, Decimal)
                                    Me.LimitMaxRotor = CType(myFieldLimitsDS.tfmwFieldLimits(0).MaxValue, Decimal)
                                End If
                            Else
                                MyBase.ShowMessage(Me.Name & ".GetLimitValues", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage, Me)
                            End If
                        Case ADJUSTMENT_ARMS.MIXER2
                            ' Polar 
                            myGlobalDataTO = GetControlsLimits(FieldLimitsEnum.SRV_MIXER2_POLAR_LIMIT)
                            If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                                myFieldLimitsDS = CType(myGlobalDataTO.SetDatos, FieldLimitsDS)
                                If myFieldLimitsDS.tfmwFieldLimits.Rows.Count > 0 Then
                                    Me.LimitMinPolar = CType(myFieldLimitsDS.tfmwFieldLimits(0).MinValue, Decimal)
                                    Me.LimitMaxPolar = CType(myFieldLimitsDS.tfmwFieldLimits(0).MaxValue, Decimal)
                                End If
                            Else
                                MyBase.ShowMessage(Me.Name & ".GetLimitValues", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage, Me)
                            End If
                            ' Z 
                            myGlobalDataTO = GetControlsLimits(FieldLimitsEnum.SRV_MIXER2_Z_LIMIT)
                            If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                                myFieldLimitsDS = CType(myGlobalDataTO.SetDatos, FieldLimitsDS)
                                If myFieldLimitsDS.tfmwFieldLimits.Rows.Count > 0 Then
                                    Me.LimitMinZ = CType(myFieldLimitsDS.tfmwFieldLimits(0).MinValue, Decimal)
                                    Me.LimitMaxZ = CType(myFieldLimitsDS.tfmwFieldLimits(0).MaxValue, Decimal)
                                End If
                            Else
                                MyBase.ShowMessage(Me.Name & ".GetLimitValues", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage, Me)
                            End If
                            ' Rotor 
                            myGlobalDataTO = GetControlsLimits(FieldLimitsEnum.SRV_MIXER2_ROTOR_LIMIT)
                            If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                                myFieldLimitsDS = CType(myGlobalDataTO.SetDatos, FieldLimitsDS)
                                If myFieldLimitsDS.tfmwFieldLimits.Rows.Count > 0 Then
                                    Me.LimitMinRotor = CType(myFieldLimitsDS.tfmwFieldLimits(0).MinValue, Decimal)
                                    Me.LimitMaxRotor = CType(myFieldLimitsDS.tfmwFieldLimits(0).MaxValue, Decimal)
                                End If
                            Else
                                MyBase.ShowMessage(Me.Name & ".GetLimitValues", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage, Me)
                            End If
                    End Select

                    ' Sample Z Fine limit - below of which all brusque movements are disabled (is just allowed the step-by-step movements)
                    myGlobalDataTO = GetControlsLimits(FieldLimitsEnum.SRV_SAMPLE_Z_FINE_LIMIT)
                    If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                        myFieldLimitsDS = CType(myGlobalDataTO.SetDatos, FieldLimitsDS)
                        If myFieldLimitsDS.tfmwFieldLimits.Rows.Count > 0 Then
                            Me.SampleLimitZFine = CType(myFieldLimitsDS.tfmwFieldLimits(0).MaxValue, Decimal)
                        End If
                    Else
                        MyBase.ShowMessage(Me.Name & ".GetLimitValues", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage, Me)
                    End If
                    ' Reagent1 Z Fine limit - below of which all brusque movements are disabled (is just allowed the step-by-step movements)
                    myGlobalDataTO = GetControlsLimits(FieldLimitsEnum.SRV_REAGENT1_Z_FINE_LIMIT)
                    If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                        myFieldLimitsDS = CType(myGlobalDataTO.SetDatos, FieldLimitsDS)
                        If myFieldLimitsDS.tfmwFieldLimits.Rows.Count > 0 Then
                            Me.Reagent1LimitZFine = CType(myFieldLimitsDS.tfmwFieldLimits(0).MaxValue, Decimal)
                        End If
                    Else
                        MyBase.ShowMessage(Me.Name & ".GetLimitValues", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage, Me)
                    End If
                    ' Reagent2 Z Fine limit - below of which all brusque movements are disabled (is just allowed the step-by-step movements)
                    myGlobalDataTO = GetControlsLimits(FieldLimitsEnum.SRV_REAGENT2_Z_FINE_LIMIT)
                    If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                        myFieldLimitsDS = CType(myGlobalDataTO.SetDatos, FieldLimitsDS)
                        If myFieldLimitsDS.tfmwFieldLimits.Rows.Count > 0 Then
                            Me.Reagent2LimitZFine = CType(myFieldLimitsDS.tfmwFieldLimits(0).MaxValue, Decimal)
                        End If
                    Else
                        MyBase.ShowMessage(Me.Name & ".GetLimitValues", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage, Me)
                    End If
                    ' Mixer1 Z Fine limit - below of which all brusque movements are disabled (is just allowed the step-by-step movements)
                    myGlobalDataTO = GetControlsLimits(FieldLimitsEnum.SRV_MIXER1_Z_FINE_LIMIT)
                    If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                        myFieldLimitsDS = CType(myGlobalDataTO.SetDatos, FieldLimitsDS)
                        If myFieldLimitsDS.tfmwFieldLimits.Rows.Count > 0 Then
                            Me.Mixer1LimitZFine = CType(myFieldLimitsDS.tfmwFieldLimits(0).MaxValue, Decimal)
                        End If
                    Else
                        MyBase.ShowMessage(Me.Name & ".GetLimitValues", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage, Me)
                    End If
                    ' Mixer2 Z Fine limit - below of which all brusque movements are disabled (is just allowed the step-by-step movements)
                    myGlobalDataTO = GetControlsLimits(FieldLimitsEnum.SRV_MIXER2_Z_FINE_LIMIT)
                    If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                        myFieldLimitsDS = CType(myGlobalDataTO.SetDatos, FieldLimitsDS)
                        If myFieldLimitsDS.tfmwFieldLimits.Rows.Count > 0 Then
                            Me.Mixer2LimitZFine = CType(myFieldLimitsDS.tfmwFieldLimits(0).MaxValue, Decimal)
                        End If
                    Else
                        MyBase.ShowMessage(Me.Name & ".GetLimitValues", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage, Me)
                    End If


                    ' Configuring POLAR Adjust Control 
                    Me.BsAdjustPolar.MinimumLimit = Me.LimitMinPolar
                    Me.BsAdjustPolar.MaximumLimit = Me.LimitMaxPolar
                    'Me.BsAdjustPolar.UnitsCaption = ""
                    Me.BsAdjustPolar.MaxNumDecimals = 0
                    Me.BsAdjustPolar.CurrentStepValue = 1
                    ' Configuring Z Adjust Control 
                    Me.BsAdjustZ.MinimumLimit = Me.LimitMinZ
                    Me.BsAdjustZ.MaximumLimit = Me.LimitMaxZ
                    'Me.BsAdjustZ.UnitsCaption = ""
                    Me.BsAdjustZ.MaxNumDecimals = 0
                    Me.BsAdjustZ.CurrentStepValue = 1
                    ' Configuring ROTOR Adjust Control 
                    Me.BsAdjustRotor.MinimumLimit = Me.LimitMinRotor
                    Me.BsAdjustRotor.MaximumLimit = Me.LimitMaxRotor
                    'Me.BsAdjustRotor.UnitsCaption = ""
                    Me.BsAdjustRotor.MaxNumDecimals = 0
                    Me.BsAdjustRotor.CurrentStepValue = 1

            End Select

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".GetLimitValues ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".GetLimitValues ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
        Return myGlobalDataTO
    End Function

    ''' <summary>
    ''' Get Limits values from BD for Positioning Tests
    ''' </summary>
    ''' <remarks>Created by XBC 06/05/2011</remarks>
    Private Function GetParameters(ByVal pAnalyzerModel As String) As GlobalDataTO
        Dim myGlobalDataTO As New GlobalDataTO
        Try
            myGlobalDataTO = myScreenDelegate.GetParameters(pAnalyzerModel)

            ' Populate here parameters values which are needed for presentation layer ...

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".GetParameters ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".GetParameters ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
        Return myGlobalDataTO
    End Function

    ''' <summary>
    ''' Assigns the specific screen's elements to the common screen layout structure
    ''' </summary>
    ''' <remarks>SG 10/01/2011</remarks>
    Private Sub DefineScreenLayout(ByVal pPage As ADJUSTMENT_PAGES)

        Try
            With MyBase.myScreenLayout

                .ButtonsPanel.SaveButton = Me.BsSaveButton
                .ButtonsPanel.ExitButton = Me.BsExitButton

                .MessagesPanel.Container = Me.BsMessagesPanel
                .MessagesPanel.Icon = Me.BsMessageImage
                .MessagesPanel.Label = Me.BsMessageLabel

                Select Case Me.SelectedPage
                    Case ADJUSTMENT_PAGES.OPTIC_CENTERING
                        .ButtonsPanel.AdjustButton = Me.BsOpticAdjustButton
                        .ButtonsPanel.CancelButton = Me.BsOpticCancelButton
                        .ButtonsPanel.TestButton = Nothing
                        .AdjustmentPanel.AdjustPanel.Container = Me.BsOpticAdjustPanel
                        .AdjustmentPanel.AdjustPanel.AdjustAreas = MyBase.GetAdjustAreas(Me.BsOpticAdjustPanel)

                    Case ADJUSTMENT_PAGES.WASHING_STATION
                        .ButtonsPanel.AdjustButton = Me.BsWSAdjustButton
                        .ButtonsPanel.CancelButton = Me.BsWSCancelButton
                        .ButtonsPanel.TestButton = Nothing
                        .AdjustmentPanel.AdjustPanel.Container = Me.BsWashingAdjustPanel
                        .AdjustmentPanel.AdjustPanel.AdjustAreas = MyBase.GetAdjustAreas(Me.BsWashingAdjustPanel)

                    Case ADJUSTMENT_PAGES.ARM_POSITIONS
                        .ButtonsPanel.CancelButton = Me.BsArmsCancelButton
                        Select Case Me.SelectedArmTab
                            Case ADJUSTMENT_ARMS.SAMPLE, _
                                 ADJUSTMENT_ARMS.REAGENT1, _
                                 ADJUSTMENT_ARMS.REAGENT2, _
                                 ADJUSTMENT_ARMS.MIXER1, _
                                 ADJUSTMENT_ARMS.MIXER2
                                .AdjustmentPanel.AdjustPanel.Container = Me.BsArmsAdjustPanel
                                .AdjustmentPanel.AdjustPanel.AdjustAreas = MyBase.GetAdjustAreas(Me.BsArmsAdjustPanel)

                        End Select
                End Select

                'information
                Select Case pPage
                    Case ADJUSTMENT_PAGES.OPTIC_CENTERING
                        .AdjustmentPanel.Container = Me.BsOpticAdjustPanel
                        .InfoPanel.Container = Me.BsOpticInfoPanel

                    Case ADJUSTMENT_PAGES.WASHING_STATION
                        .AdjustmentPanel.Container = Me.BsWashingAdjustPanel
                        .InfoPanel.Container = Me.BsWashingInfoPanel

                    Case ADJUSTMENT_PAGES.ARM_POSITIONS
                        .AdjustmentPanel.Container = Me.BsArmsAdjustPanel
                        .InfoPanel.Container = Me.BsArmsInfoPanel

                    Case Else
                        .AdjustmentPanel.Container = Nothing
                        .InfoPanel.Container = Nothing
                        .InfoPanel.InfoExpandButton = Nothing

                End Select

            End With
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".DefineScreenLayout ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".DefineScreenLayout ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Gets limits and adjustments values for the specific screen
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>Created by XBC 05/01/2011</remarks>
    Private Function PreparePage() As GlobalDataTO
        Dim myResultData As New GlobalDataTO
        Try

            myResultData = GetLimitValues()
            If Not myResultData.HasError Then
                MyBase.CurrentMode = ADJUSTMENT_MODES.LOADED
                FillAdjustmentValues()
            End If

        Catch ex As Exception
            myResultData.HasError = True
            myResultData.ErrorCode = Messages.SYSTEM_ERROR.ToString
            myResultData.ErrorMessage = ex.Message
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".PreparePage ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PreparePage ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
        Return myResultData
    End Function

    ''' <summary>
    ''' Saves changed values for the selected adjustment
    ''' </summary>
    ''' <remarks>SG 24/01/11</remarks>
    Private Sub SaveAdjustment(Optional ByVal pForceToSend As Boolean = False)
        Dim myGlobal As New GlobalDataTO
        Try
            myGlobal = MyBase.Save()
            If myGlobal.HasError Then
                Me.PrepareErrorMode()
            Else
                ' Initializations
                Me.myScreenDelegate.LoadAdjDone = False
                Me.myScreenDelegate.ParkDone = False

                Me.PrepareArea()

                Select Case EditedValue.AdjustmentID
                    Case ADJUSTMENT_GROUPS.PHOTOMETRY

                        Me.myScreenDelegate.ParkDone = True

                    Case ADJUSTMENT_GROUPS.SAMPLES_ARM_PARK, _
                         ADJUSTMENT_GROUPS.REAGENT1_ARM_PARK, _
                         ADJUSTMENT_GROUPS.REAGENT2_ARM_PARK, _
                         ADJUSTMENT_GROUPS.MIXER1_ARM_PARK, _
                         ADJUSTMENT_GROUPS.MIXER2_ARM_PARK

                        Me.myScreenDelegate.ParkDone = True
                End Select

                'in case of changes made
                If Me.ChangedValue Or pForceToSend Then

                    'show warning in case of the change referes to Optic centering adjustment
                    Dim dialogResultToReturn As DialogResult = DialogResult.Yes
                    If Me.SelectedPage = ADJUSTMENT_PAGES.OPTIC_CENTERING Then
                        dialogResultToReturn = MyBase.ShowMessage(GetMessageText(Messages.SRV_ADJUSTMENTS_TESTS.ToString), Messages.SRV_OPTIC_ADJUSTMENT_SAVE.ToString)
                    End If

                    If dialogResultToReturn <> DialogResult.Yes Then

                        Me.ChangedValue = False
                        Me.PrepareLoadedMode()
                        MyBase.DisplayMessage(Messages.SRV_ADJUSTMENTS_CANCELLED.ToString)

                    Else
                        ' activate the flag that indicates that the Optic Centering adjustment has been modified
                        If Me.SelectedPage = ADJUSTMENT_PAGES.OPTIC_CENTERING Then
                            Me.OpticCenteringModified = True
                        End If

                        ' Takes a temporal copy of the changed values of the dataset of Adjustments
                        myGlobal = myAdjustmentsDelegate.Clone(Me.SelectedAdjustmentsDS)
                        If Not myGlobal.SetDatos Is Nothing AndAlso Not myGlobal.HasError Then

                            'Add the current Adjustments set to the LocalSaved dataset
                            Dim myLocalDS As SRVAdjustmentsDS = CType(myGlobal.SetDatos, SRVAdjustmentsDS)
                            MyClass.AddToLocalSavedAdjustmentsDS(myLocalDS)

                            ' Update LocalSaved dataset with the adjusted values
                            If Me.SelectedPage <> ADJUSTMENT_PAGES.ARM_POSITIONS OrElse MyClass.IsLocallySaved Then
                                myGlobal = Me.UpdateLocalSavedAdjustmentsDS()
                            End If

                        End If

                        If myGlobal.HasError Then
                            Me.PrepareErrorMode()
                            Exit Sub
                        Else
                            'if it is requested to send to the Analyzer
                            If Not MyClass.IsLocallySaved Then

                                MyBase.DisplayMessage(Messages.SRV_SAVE_ADJUSTMENTS.ToString)

                                'build the scripts delegate
                                Me.TempToSendAdjustmentsDelegate = New FwAdjustmentsDelegate(Me.LocalSavedAdjustmentsDS)

                                'XBC 27/02/2012 !!!!
                                myGlobal = Me.SetAdditionalAdjustments(Me.LocalSavedAdjustmentsDS)
                                If myGlobal.HasError Then
                                    Me.PrepareErrorMode()
                                    Exit Sub
                                End If

                            End If

                            'update SelectedDS
                            myGlobal = myAdjustmentsDelegate.Clone(Me.LocalSavedAdjustmentsDS)
                            If Not myGlobal.SetDatos Is Nothing AndAlso Not myGlobal.HasError Then
                                ' Selected adjustments is updated with the new values
                                Me.SelectedAdjustmentsDS = CType(myGlobal.SetDatos, SRVAdjustmentsDS)
                            Else
                                Me.PrepareErrorMode()
                                Exit Sub
                            End If

                            If MyBase.SimulationMode Then

                                myScreenDelegate.LoadAdjDone = True
                                myScreenDelegate.ParkDone = True
                                MyBase.CurrentMode = ADJUSTMENT_MODES.SAVED
                                MyClass.PrepareArea()

                            Else
                                'SGM 28/02/2012 
                                If Not MyClass.IsLocallySaved Then


                                    myGlobal = Me.TempToSendAdjustmentsDelegate.ConvertDSToString()

                                    If Not myGlobal.SetDatos Is Nothing AndAlso Not myGlobal.HasError Then


                                        Dim pAdjuststr As String = CType(myGlobal.SetDatos, String)
                                        myScreenDelegate.pValueAdjust = pAdjuststr

                                        Me.Cursor = Cursors.WaitCursor
                                        If Not myGlobal.HasError AndAlso AnalyzerController.Instance.Analyzer.Connected Then '#REFACTORING
                                            myGlobal = Me.myScreenDelegate.SendLoad_Adjustments()
                                            If Me.SelectedPage = ADJUSTMENT_PAGES.ARM_POSITIONS Then
                                                Me.myScreenDelegate.ParkDone = True
                                            End If
                                        End If

                                    Else
                                        Me.PrepareErrorMode()
                                    End If

                                ElseIf Me.SelectedPage = ADJUSTMENT_PAGES.ARM_POSITIONS Then

                                    MyBase.DisplayMessage(Messages.SRV_ARMS_TO_PARKING.ToString)

                                    ' Sending park operation 
                                    If MyBase.SimulationMode Then
                                        ' simulating
                                        Me.Cursor = Cursors.WaitCursor
                                        Thread.Sleep(SimulationProcessTime)
                                        MyBase.myServiceMDI.Focus()
                                        Me.Cursor = Cursors.Default
                                        myScreenDelegate.ParkDone = True

                                        MyBase.DisplayMessage(Messages.SRV_ADJUSTMENTS_LOCAL_SAVED.ToString)
                                    Else
                                        ' Manage FwScripts must to be sent to parking
                                        Me.SendFwScript(Me.CurrentMode, EditedValue.AdjustmentID)
                                    End If
                                End If
                            End If
                        End If
                    End If
                Else
                    MyClass.CancelAdjustment()
                End If
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".SaveAdjustment ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".SaveAdjustment ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
            Me.ReportHistoryError()
        End Try
    End Sub


    ''' <summary>
    ''' Cancels changed values for the selected adjustment
    ''' </summary>
    ''' <remarks>SG 24/01/11</remarks>
    Private Sub CancelAdjustment()
        Dim myGlobal As New GlobalDataTO
        Try
            If EditedValue.AdjustmentID = ADJUSTMENT_GROUPS._NONE Then
                Exit Try
            End If

            myGlobal = MyBase.ExitAdjust()
            If myGlobal.HasError Then
                PrepareErrorMode()
            Else

                MyBase.DisplayMessage("")
                If Me.SelectedPage = ADJUSTMENT_PAGES.ARM_POSITIONS AndAlso Not myScreenDelegate.ParkDone Then
                    MyBase.DisplayMessage(Messages.SRV_ARMS_TO_PARKING.ToString)
                End If

                If ChangedValue Then
                    Me.ReportHistory(HISTORY_TASKS.ADJUSTMENT, PositionsAdjustmentDelegate.HISTORY_RESULTS.CANCEL)
                End If

                PrepareArea()

                ' Initializations
                myScreenDelegate.ParkDone = False

                If MyBase.SimulationMode Then
                    ' simulating
                    MyBase.CurrentMode = ADJUSTMENT_MODES.LOADED
                    PrepareArea()
                Else
                    ' Manage FwScripts must to be sent to Adjust Exiting
                    Me.SendFwScript(Me.CurrentMode, EditedValue.AdjustmentID)
                    If myScreenDelegate.ParkDone Then
                        MyBase.CurrentMode = ADJUSTMENT_MODES.LOADED
                        PrepareArea()
                    End If
                End If

                Me.SelectedRow = Nothing

                If Me.ChangedValue Then

                    If Not myGlobal.HasError Then
                        If MyBase.SimulationMode Then
                            MyBase.DisplayMessage("")
                        Else
                            ' Manage FwScripts must to be sent to Adjust Exiting
                            Me.SendFwScript(Me.CurrentMode, EditedValue.AdjustmentID)
                            If myScreenDelegate.ParkDone Then
                                Me.WaitForScriptsExitingScreen = False
                                MyBase.CurrentMode = ADJUSTMENT_MODES.LOADED
                                PrepareArea()
                            End If
                        End If
                    End If


                Else

                    If Me.SelectedPage = ADJUSTMENT_PAGES.ARM_POSITIONS Then
                        If Me.WaitForScriptsExitingScreen Then
                            If Not MyBase.SimulationMode Then Thread.Sleep(2000)
                            Me.FinishExitScreen()
                        End If
                    End If

                End If

                ManageTabPages = True



            End If



        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".CancelAdjustment ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".CancelAdjustment ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
            Me.ReportHistoryError()
        End Try
    End Sub

    ''' <summary>
    ''' Fills the specific adjustment fields
    ''' </summary>
    ''' <remarks>SGM 28/01/11</remarks>
    Private Sub FillAdjustmentValues()
        Try
            ' Initialize values from Instrument

            Me.BsOpticAdjustmentLabel.Text = ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.PHOTOMETRY.ToString, GlobalEnumerates.AXIS.ROTOR).Value
            ' XBC 03/01/2012 - Add Encoder functionality
            Me.BsEncoderAdjustmentLabel.Text = ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.PHOTOMETRY.ToString, GlobalEnumerates.AXIS.ENCODER).Value

            ' XBC 18/04/2012 - Z aproximation is anuled
            Me.BsWashingAdjustmentLabel.Text = ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.WASHING_STATION.ToString, GlobalEnumerates.AXIS.Z).Value

            LoadAllArmsPositionsAdjustmentData() 'SGM 02/09/2011 History

            Select Case SelectedArmTab
                Case ADJUSTMENT_ARMS.SAMPLE
                    LoadAdjustmentsForSampleArmTab()
                Case ADJUSTMENT_ARMS.REAGENT1
                    LoadAdjustmentsForReagent1ArmTab()
                Case ADJUSTMENT_ARMS.REAGENT2
                    LoadAdjustmentsForReagent2ArmTab()
                Case ADJUSTMENT_ARMS.MIXER1
                    LoadAdjustmentsForMixer1ArmTab()
                Case ADJUSTMENT_ARMS.MIXER2
                    LoadAdjustmentsForMixer2ArmTab()
            End Select

            Me.FillAdjustmentValuesintoDelegate()

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".FillAdjustmentValues ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".FillAdjustmentValues ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub LoadAdjustmentsForMixer2ArmTab()

        Me.HomePolar = 0
        Me.HomeZ = 0
        Me.HomeRotor = 0
        If Not Me.BsGridMixer2 Is Nothing AndAlso Not AnalyzerController.Instance.IsBA200() Then
            If Me.BsGridMixer2.RowsCount > 0 Then
                '' DISP1
                Me.BsGridMixer2.ParameterCellValue(0, POLAR_COLUMN) = ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.MIXER2_ARM_DISP1.ToString, GlobalEnumerates.AXIS.POLAR).Value
                Me.BsGridMixer2.ParameterCellValue(0, Z_COLUMN) = ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.MIXER2_ARM_DISP1.ToString, GlobalEnumerates.AXIS.Z).Value
                Me.BsGridMixer2.ParameterCellValue(0, ROTOR_COLUMN) = ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.MIXER2_ARM_DISP1.ToString, GlobalEnumerates.AXIS.ROTOR).Value
                '' Z REFERENCE
                Me.BsGridMixer2.ParameterCellValue(1, POLAR_COLUMN) = myScreenDelegate.Mixer2ArmPolarZREF.ToString ' ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.MIXER2_ARM_ZREF.ToString, GlobalEnumerates.AXIS.POLAR).Value
                Me.BsGridMixer2.ParameterCellValue(1, Z_COLUMN) = ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.MIXER2_ARM_ZREF.ToString, GlobalEnumerates.AXIS.Z).Value
                Me.BsGridMixer2.ParameterCellValue(1, ROTOR_COLUMN) = ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.MIXER2_ARM_ZREF.ToString, GlobalEnumerates.AXIS.ROTOR).Value
                '' WASHING
                Me.BsGridMixer2.ParameterCellValue(2, POLAR_COLUMN) = ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.MIXER2_ARM_WASH.ToString, GlobalEnumerates.AXIS.POLAR).Value
                Me.BsGridMixer2.ParameterCellValue(2, Z_COLUMN) = ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.MIXER2_ARM_WASH.ToString, GlobalEnumerates.AXIS.Z).Value
                Me.BsGridMixer2.ParameterCellValue(2, ROTOR_COLUMN) = ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.MIXER2_ARM_WASH.ToString, GlobalEnumerates.AXIS.ROTOR).Value
                '' PARKING
                Me.BsGridMixer2.ParameterCellValue(3, POLAR_COLUMN) = ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.MIXER2_ARM_PARK.ToString, GlobalEnumerates.AXIS.POLAR).Value
                Me.BsGridMixer2.ParameterCellValue(3, Z_COLUMN) = ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.MIXER2_ARM_PARK.ToString, GlobalEnumerates.AXIS.Z).Value
                Me.BsGridMixer2.ParameterCellValue(3, ROTOR_COLUMN) = ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.MIXER2_ARM_PARK.ToString, GlobalEnumerates.AXIS.ROTOR).Value
            End If
        End If
    End Sub

    Private Sub LoadAdjustmentsForMixer1ArmTab()

        Me.HomePolar = 0
        Me.HomeZ = 0
        Me.HomeRotor = 0
        If Not Me.BsGridMixer1 Is Nothing Then
            If Me.BsGridMixer1.RowsCount > 0 Then
                '' DISP1
                Me.BsGridMixer1.ParameterCellValue(0, POLAR_COLUMN) = ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.MIXER1_ARM_DISP1.ToString, GlobalEnumerates.AXIS.POLAR).Value
                Me.BsGridMixer1.ParameterCellValue(0, Z_COLUMN) = ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.MIXER1_ARM_DISP1.ToString, GlobalEnumerates.AXIS.Z).Value
                Me.BsGridMixer1.ParameterCellValue(0, ROTOR_COLUMN) = ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.MIXER1_ARM_DISP1.ToString, GlobalEnumerates.AXIS.ROTOR).Value
                '' Z REFERENCE
                Me.BsGridMixer1.ParameterCellValue(1, POLAR_COLUMN) = myScreenDelegate.Mixer1ArmPolarZREF.ToString ' ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.MIXER1_ARM_ZREF.ToString, GlobalEnumerates.AXIS.POLAR).Value
                Me.BsGridMixer1.ParameterCellValue(1, Z_COLUMN) = ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.MIXER1_ARM_ZREF.ToString, GlobalEnumerates.AXIS.Z).Value
                Me.BsGridMixer1.ParameterCellValue(1, ROTOR_COLUMN) = ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.MIXER1_ARM_ZREF.ToString, GlobalEnumerates.AXIS.ROTOR).Value
                '' WASHING
                Me.BsGridMixer1.ParameterCellValue(2, POLAR_COLUMN) = ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.MIXER1_ARM_WASH.ToString, GlobalEnumerates.AXIS.POLAR).Value
                Me.BsGridMixer1.ParameterCellValue(2, Z_COLUMN) = ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.MIXER1_ARM_WASH.ToString, GlobalEnumerates.AXIS.Z).Value
                Me.BsGridMixer1.ParameterCellValue(2, ROTOR_COLUMN) = ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.MIXER1_ARM_WASH.ToString, GlobalEnumerates.AXIS.ROTOR).Value
                '' PARKING
                Me.BsGridMixer1.ParameterCellValue(3, POLAR_COLUMN) = ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.MIXER1_ARM_PARK.ToString, GlobalEnumerates.AXIS.POLAR).Value
                Me.BsGridMixer1.ParameterCellValue(3, Z_COLUMN) = ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.MIXER1_ARM_PARK.ToString, GlobalEnumerates.AXIS.Z).Value
                Me.BsGridMixer1.ParameterCellValue(3, ROTOR_COLUMN) = ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.MIXER1_ARM_PARK.ToString, GlobalEnumerates.AXIS.ROTOR).Value
            End If
        End If
    End Sub

    Private Sub LoadAdjustmentsForReagent2ArmTab()

        Me.HomePolar = 0
        Me.HomeZ = 0
        Me.HomeRotor = 0
        If Not Me.BsGridReagent2 Is Nothing Then
            If Me.BsGridReagent2.RowsCount > 0 Then
                '' DISP1
                BsGridReagent2.ParameterCellValue(0, POLAR_COLUMN) = ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.REAGENT2_ARM_DISP1.ToString, GlobalEnumerates.AXIS.POLAR).Value
                BsGridReagent2.ParameterCellValue(0, Z_COLUMN) = ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.REAGENT2_ARM_DISP1.ToString, GlobalEnumerates.AXIS.Z).Value
                BsGridReagent2.ParameterCellValue(0, ROTOR_COLUMN) = ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.REAGENT2_ARM_DISP1.ToString, GlobalEnumerates.AXIS.ROTOR).Value
                '' Z REFERENCE
                BsGridReagent2.ParameterCellValue(1, POLAR_COLUMN) = myScreenDelegate.Reagent2ArmPolarZREF.ToString ' ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.REAGENT2_ARM_ZREF.ToString, GlobalEnumerates.AXIS.POLAR).Value
                BsGridReagent2.ParameterCellValue(1, Z_COLUMN) = ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.REAGENT2_ARM_ZREF.ToString, GlobalEnumerates.AXIS.Z).Value
                BsGridReagent2.ParameterCellValue(1, ROTOR_COLUMN) = ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.REAGENT2_ARM_ZREF.ToString, GlobalEnumerates.AXIS.ROTOR).Value
                '' WASHING
                BsGridReagent2.ParameterCellValue(2, POLAR_COLUMN) = ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.REAGENT2_ARM_WASH.ToString, GlobalEnumerates.AXIS.POLAR).Value
                BsGridReagent2.ParameterCellValue(2, Z_COLUMN) = ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.REAGENT2_ARM_WASH.ToString, GlobalEnumerates.AXIS.Z).Value
                BsGridReagent2.ParameterCellValue(2, ROTOR_COLUMN) = ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.REAGENT2_ARM_WASH.ToString, GlobalEnumerates.AXIS.ROTOR).Value
                '' REAGENT1
                BsGridReagent2.ParameterCellValue(3, POLAR_COLUMN) = ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.REAGENT2_ARM_RING1.ToString, GlobalEnumerates.AXIS.POLAR).Value
                BsGridReagent2.ParameterCellValue(3, Z_COLUMN) = ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.REAGENT2_ARM_RING1.ToString, GlobalEnumerates.AXIS.Z).Value
                Me.BsGridReagent2.ParameterCellValue(3, ROTOR_COLUMN) = ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.REAGENT2_ARM_RING1.ToString, GlobalEnumerates.AXIS.ROTOR).Value
                '' REAGENT2
                Me.BsGridReagent2.ParameterCellValue(4, POLAR_COLUMN) = ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.REAGENT2_ARM_RING2.ToString, GlobalEnumerates.AXIS.POLAR).Value
                Me.BsGridReagent2.ParameterCellValue(4, Z_COLUMN) = ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.REAGENT2_ARM_RING2.ToString, GlobalEnumerates.AXIS.Z).Value
                Me.BsGridReagent2.ParameterCellValue(4, ROTOR_COLUMN) = ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.REAGENT2_ARM_RING2.ToString, GlobalEnumerates.AXIS.ROTOR).Value
                '' PARKING
                Me.BsGridReagent2.ParameterCellValue(5, POLAR_COLUMN) = ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.REAGENT2_ARM_PARK.ToString, GlobalEnumerates.AXIS.POLAR).Value
                Me.BsGridReagent2.ParameterCellValue(5, Z_COLUMN) = ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.REAGENT2_ARM_PARK.ToString, GlobalEnumerates.AXIS.Z).Value
                Me.BsGridReagent2.ParameterCellValue(5, ROTOR_COLUMN) = ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.REAGENT2_ARM_PARK.ToString, GlobalEnumerates.AXIS.ROTOR).Value
            End If
        End If
    End Sub

    Private Sub LoadAdjustmentsForReagent1ArmTab()

        Me.HomePolar = 0
        Me.HomeZ = 0
        Me.HomeRotor = 0
        If Not Me.BsGridReagent1 Is Nothing Then
            If Me.BsGridReagent1.RowsCount > 0 Then
                '' DISP1
                Me.BsGridReagent1.ParameterCellValue(0, POLAR_COLUMN) = ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.REAGENT1_ARM_DISP1.ToString, GlobalEnumerates.AXIS.POLAR).Value
                Me.BsGridReagent1.ParameterCellValue(0, Z_COLUMN) = ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.REAGENT1_ARM_DISP1.ToString, GlobalEnumerates.AXIS.Z).Value
                Me.BsGridReagent1.ParameterCellValue(0, ROTOR_COLUMN) = ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.REAGENT1_ARM_DISP1.ToString, GlobalEnumerates.AXIS.ROTOR).Value
                '' Z REFERENCE
                Me.BsGridReagent1.ParameterCellValue(1, POLAR_COLUMN) = myScreenDelegate.Reagent1ArmPolarZREF.ToString ' ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.REAGENT1_ARM_ZREF.ToString, GlobalEnumerates.AXIS.POLAR).Value
                Me.BsGridReagent1.ParameterCellValue(1, Z_COLUMN) = ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.REAGENT1_ARM_ZREF.ToString, GlobalEnumerates.AXIS.Z).Value
                Me.BsGridReagent1.ParameterCellValue(1, ROTOR_COLUMN) = ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.REAGENT1_ARM_ZREF.ToString, GlobalEnumerates.AXIS.ROTOR).Value
                '' WASHING
                Me.BsGridReagent1.ParameterCellValue(2, POLAR_COLUMN) = ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.REAGENT1_ARM_WASH.ToString, GlobalEnumerates.AXIS.POLAR).Value
                Me.BsGridReagent1.ParameterCellValue(2, Z_COLUMN) = ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.REAGENT1_ARM_WASH.ToString, GlobalEnumerates.AXIS.Z).Value
                Me.BsGridReagent1.ParameterCellValue(2, ROTOR_COLUMN) = ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.REAGENT1_ARM_WASH.ToString, GlobalEnumerates.AXIS.ROTOR).Value
                '' REAGENT1
                Me.BsGridReagent1.ParameterCellValue(3, POLAR_COLUMN) = ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.REAGENT1_ARM_RING1.ToString, GlobalEnumerates.AXIS.POLAR).Value
                Me.BsGridReagent1.ParameterCellValue(3, Z_COLUMN) = ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.REAGENT1_ARM_RING1.ToString, GlobalEnumerates.AXIS.Z).Value
                Me.BsGridReagent1.ParameterCellValue(3, ROTOR_COLUMN) = ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.REAGENT1_ARM_RING1.ToString, GlobalEnumerates.AXIS.ROTOR).Value
                '' REAGENT2
                Me.BsGridReagent1.ParameterCellValue(4, POLAR_COLUMN) = ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.REAGENT1_ARM_RING2.ToString, GlobalEnumerates.AXIS.POLAR).Value
                Me.BsGridReagent1.ParameterCellValue(4, Z_COLUMN) = ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.REAGENT1_ARM_RING2.ToString, GlobalEnumerates.AXIS.Z).Value
                Me.BsGridReagent1.ParameterCellValue(4, ROTOR_COLUMN) = ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.REAGENT1_ARM_RING2.ToString, GlobalEnumerates.AXIS.ROTOR).Value
                '' PARKING
                Me.BsGridReagent1.ParameterCellValue(5, POLAR_COLUMN) = ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.REAGENT1_ARM_PARK.ToString, GlobalEnumerates.AXIS.POLAR).Value
                Me.BsGridReagent1.ParameterCellValue(5, Z_COLUMN) = ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.REAGENT1_ARM_PARK.ToString, GlobalEnumerates.AXIS.Z).Value
                Me.BsGridReagent1.ParameterCellValue(5, ROTOR_COLUMN) = ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.REAGENT1_ARM_PARK.ToString, GlobalEnumerates.AXIS.ROTOR).Value
            End If
        End If
    End Sub

    Private Sub LoadAdjustmentsForSampleArmTab()

        Me.HomePolar = 0
        Me.HomeZ = 0
        Me.HomeRotor = 0

        If Not Me.BsGridSample Is Nothing Then
            If Me.BsGridSample.RowsCount > 0 Then
                Dim position As Integer = 0
                '' DISP1
                Me.BsGridSample.ParameterCellValue(position, POLAR_COLUMN) = ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.SAMPLES_ARM_DISP1.ToString, GlobalEnumerates.AXIS.POLAR).Value
                Me.BsGridSample.ParameterCellValue(position, Z_COLUMN) = ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.SAMPLES_ARM_DISP1.ToString, GlobalEnumerates.AXIS.Z).Value
                Me.BsGridSample.ParameterCellValue(position, ROTOR_COLUMN) = ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.SAMPLES_ARM_DISP1.ToString, GlobalEnumerates.AXIS.ROTOR).Value
                position += 1
                '' DISP2
                Me.BsGridSample.ParameterCellValue(position, POLAR_COLUMN) = ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.SAMPLES_ARM_DISP2.ToString, GlobalEnumerates.AXIS.POLAR).Value
                Me.BsGridSample.ParameterCellValue(position, Z_COLUMN) = ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.SAMPLES_ARM_DISP2.ToString, GlobalEnumerates.AXIS.Z).Value
                Me.BsGridSample.ParameterCellValue(position, ROTOR_COLUMN) = ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.SAMPLES_ARM_DISP2.ToString, GlobalEnumerates.AXIS.ROTOR).Value
                position += 1
                '' Z REFERENCE
                Me.BsGridSample.ParameterCellValue(position, POLAR_COLUMN) = myScreenDelegate.SampleArmPolarZREF.ToString
                Me.BsGridSample.ParameterCellValue(position, Z_COLUMN) = ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.SAMPLES_ARM_ZREF.ToString, GlobalEnumerates.AXIS.Z).Value
                Me.BsGridSample.ParameterCellValue(position, ROTOR_COLUMN) = ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.SAMPLES_ARM_ZREF.ToString, GlobalEnumerates.AXIS.ROTOR).Value
                position += 1
                '' WASHING
                Me.BsGridSample.ParameterCellValue(position, POLAR_COLUMN) = ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.SAMPLES_ARM_WASH.ToString, GlobalEnumerates.AXIS.POLAR).Value
                Me.BsGridSample.ParameterCellValue(position, Z_COLUMN) = ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.SAMPLES_ARM_WASH.ToString, GlobalEnumerates.AXIS.Z).Value
                Me.BsGridSample.ParameterCellValue(position, ROTOR_COLUMN) = ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.SAMPLES_ARM_WASH.ToString, GlobalEnumerates.AXIS.ROTOR).Value
                position += 1
                '' SAMPLE RING1 PEDIATRIC
                Me.BsGridSample.ParameterCellValue(position, POLAR_COLUMN) = ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.SAMPLES_ARM_RING1.ToString, GlobalEnumerates.AXIS.POLAR).Value
                Me.BsGridSample.ParameterCellValue(position, Z_COLUMN) = ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.SAMPLES_ARM_RING1.ToString, GlobalEnumerates.AXIS.Z).Value
                Me.BsGridSample.ParameterCellValue(position, ROTOR_COLUMN) = ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.SAMPLES_ARM_RING1.ToString, GlobalEnumerates.AXIS.ROTOR).Value
                position += 1
                '' SAMPLE RING2 PEDIATRIC
                Me.BsGridSample.ParameterCellValue(position, POLAR_COLUMN) = ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.SAMPLES_ARM_RING2.ToString, GlobalEnumerates.AXIS.POLAR).Value
                Me.BsGridSample.ParameterCellValue(position, Z_COLUMN) = ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.SAMPLES_ARM_RING2.ToString, GlobalEnumerates.AXIS.Z).Value
                Me.BsGridSample.ParameterCellValue(position, ROTOR_COLUMN) = ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.SAMPLES_ARM_RING2.ToString, GlobalEnumerates.AXIS.ROTOR).Value
                position += 1
                If Not AnalyzerController.Instance.IsBA200() Then
                    '' SAMPLE RING3 PEDIATRIC
                    Me.BsGridSample.ParameterCellValue(position, POLAR_COLUMN) = ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.SAMPLES_ARM_RING3.ToString, GlobalEnumerates.AXIS.POLAR).Value
                    Me.BsGridSample.ParameterCellValue(position, Z_COLUMN) = ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.SAMPLES_ARM_RING3.ToString, GlobalEnumerates.AXIS.Z).Value
                    Me.BsGridSample.ParameterCellValue(position, ROTOR_COLUMN) = ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.SAMPLES_ARM_RING3.ToString, GlobalEnumerates.AXIS.ROTOR).Value
                    position += 1
                End If
                '' Z TUBE Polar and Rotor values taken from Ring1 when saving write to Z of Tube1, Tube2 and Tube3
                Me.BsGridSample.ParameterCellValue(position, POLAR_COLUMN) = ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.SAMPLES_ARM_RING1.ToString, GlobalEnumerates.AXIS.POLAR).Value
                Me.BsGridSample.ParameterCellValue(position, Z_COLUMN) = ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.SAMPLES_ARM_ZTUBE1.ToString, GlobalEnumerates.AXIS.Z).Value
                Me.BsGridSample.ParameterCellValue(position, ROTOR_COLUMN) = ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.SAMPLES_ARM_RING1.ToString, GlobalEnumerates.AXIS.ROTOR).Value
                position += 1
                '' ISE
                Me.BsGridSample.ParameterCellValue(position, POLAR_COLUMN) = ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.SAMPLES_ARM_ISE.ToString, GlobalEnumerates.AXIS.POLAR).Value
                Me.BsGridSample.ParameterCellValue(position, Z_COLUMN) = ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.SAMPLES_ARM_ISE.ToString, GlobalEnumerates.AXIS.Z).Value
                Me.BsGridSample.ParameterCellValue(position, ROTOR_COLUMN) = ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.SAMPLES_ARM_ISE.ToString, GlobalEnumerates.AXIS.ROTOR).Value
                ISE_SAMPLE_ROW = position
                position += 1
                '' PARKING
                Me.BsGridSample.ParameterCellValue(position, POLAR_COLUMN) = ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.SAMPLES_ARM_PARK.ToString, GlobalEnumerates.AXIS.POLAR).Value
                Me.BsGridSample.ParameterCellValue(position, Z_COLUMN) = ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.SAMPLES_ARM_PARK.ToString, GlobalEnumerates.AXIS.Z).Value
                Me.BsGridSample.ParameterCellValue(position, ROTOR_COLUMN) = ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.SAMPLES_ARM_PARK.ToString, GlobalEnumerates.AXIS.ROTOR).Value
                position += 1
                If AnalyzerController.Instance.IsBA200() Then
                    '' Z TUBE REAGENT Polar and Rotor values taken from Ring1 when saving write to Z of Tube1 and Tube2
                    Me.BsGridSample.ParameterCellValue(position, POLAR_COLUMN) = ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.SAMPLES_ARM_RING1.ToString, GlobalEnumerates.AXIS.POLAR).Value
                    Me.BsGridSample.ParameterCellValue(position, Z_COLUMN) = ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.REAGENT1_ARM_RING1.ToString, GlobalEnumerates.AXIS.Z).Value
                    Me.BsGridSample.ParameterCellValue(position, ROTOR_COLUMN) = ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.SAMPLES_ARM_RING1.ToString, GlobalEnumerates.AXIS.ROTOR).Value
                    position += 1
                End If
            End If
        End If
    End Sub

    ''' <summary>
    ''' Fills the specific delegate adjustment fields
    ''' </summary>
    ''' <remarks>XBC 10/05/11</remarks>
    Private Sub FillAdjustmentValuesintoDelegate()
        Try
            Me.myScreenDelegate.WashingStationPark = ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.WASHING_STATION_PARK.ToString, GlobalEnumerates.AXIS.Z).Value

            If Not Me.BsGridSample Is Nothing Then
                If Me.BsGridSample.RowsCount > 0 Then
                    ' Fill parking values into Delegate parameters
                    Me.myScreenDelegate.SampleArmParkH = ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.SAMPLES_ARM_PARK.ToString, GlobalEnumerates.AXIS.POLAR).Value
                    Me.myScreenDelegate.SampleArmParkV = ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.SAMPLES_ARM_PARK.ToString, GlobalEnumerates.AXIS.Z).Value
                    Me.myScreenDelegate.SampleSecurityFly = ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.SAMPLES_ARM_VSEC.ToString, GlobalEnumerates.AXIS.Z).Value
                    Me.SampleLimitZFine = CSng(Me.myScreenDelegate.SampleSecurityFly)

                    If AnalyzerController.Instance.IsBA200 Then
                        Me.myScreenDelegate.Reagent1ArmParkH = ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.SAMPLES_ARM_PARK.ToString, GlobalEnumerates.AXIS.POLAR).Value
                        Me.myScreenDelegate.Reagent1ArmParkV = ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.SAMPLES_ARM_PARK.ToString, GlobalEnumerates.AXIS.Z).Value
                        Me.myScreenDelegate.Reagent1SecurityFly = ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.SAMPLES_ARM_VSEC.ToString, GlobalEnumerates.AXIS.Z).Value
                        Me.Reagent1LimitZFine = CSng(Me.myScreenDelegate.Reagent1SecurityFly)
                    End If
                End If
            End If
            If Not Me.BsGridReagent1 Is Nothing AndAlso Not (AnalyzerController.Instance.IsBA200) Then 'AndAlso Not (AnalyzerController.Instance.Analyzer.Model = AnalyzerModelEnum.A200.ToString()) Then
                If Me.BsGridReagent1.RowsCount > 0 Then
                    ' Fill parking values into Delegate parameters
                    Me.myScreenDelegate.Reagent1ArmParkH = ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.REAGENT1_ARM_PARK.ToString, GlobalEnumerates.AXIS.POLAR).Value
                    Me.myScreenDelegate.Reagent1ArmParkV = ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.REAGENT1_ARM_PARK.ToString, GlobalEnumerates.AXIS.Z).Value
                    Me.myScreenDelegate.Reagent1SecurityFly = ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.REAGENT1_ARM_VSEC.ToString, GlobalEnumerates.AXIS.Z).Value
                    Me.Reagent1LimitZFine = CSng(Me.myScreenDelegate.Reagent1SecurityFly)
                End If
            End If
            If Not Me.BsGridReagent2 Is Nothing AndAlso Not (AnalyzerController.Instance.IsBA200) Then
                If Me.BsGridReagent2.RowsCount > 0 Then
                    ' Fill parking values into Delegate parameters
                    Me.myScreenDelegate.Reagent2ArmParkH = ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.REAGENT2_ARM_PARK.ToString, GlobalEnumerates.AXIS.POLAR).Value
                    Me.myScreenDelegate.Reagent2ArmParkV = ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.REAGENT2_ARM_PARK.ToString, GlobalEnumerates.AXIS.Z).Value
                    Me.myScreenDelegate.Reagent2SecurityFly = ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.REAGENT2_ARM_VSEC.ToString, GlobalEnumerates.AXIS.Z).Value
                    Me.Reagent2LimitZFine = CSng(Me.myScreenDelegate.Reagent2SecurityFly)
                End If
            End If
            If Not Me.BsGridMixer1 Is Nothing Then
                If Me.BsGridMixer1.RowsCount > 0 Then
                    ' Fill parking values into Delegate parameters
                    Me.myScreenDelegate.Mixer1ArmParkH = ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.MIXER1_ARM_PARK.ToString, GlobalEnumerates.AXIS.POLAR).Value
                    Me.myScreenDelegate.Mixer1ArmParkV = ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.MIXER1_ARM_PARK.ToString, GlobalEnumerates.AXIS.Z).Value
                    Me.myScreenDelegate.Mixer1SecurityFly = ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.MIXER1_ARM_VSEC.ToString, GlobalEnumerates.AXIS.Z).Value
                    Me.Mixer1LimitZFine = CSng(Me.myScreenDelegate.Mixer1SecurityFly)
                End If
            End If
            If Not Me.BsGridMixer2 Is Nothing AndAlso Not (AnalyzerController.Instance.IsBA200) Then
                If Me.BsGridMixer2.RowsCount > 0 Then
                    ' Fill parking values into Delegate parameters
                    Me.myScreenDelegate.Mixer2ArmParkH = ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.MIXER2_ARM_PARK.ToString, GlobalEnumerates.AXIS.POLAR).Value
                    Me.myScreenDelegate.Mixer2ArmParkV = ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.MIXER2_ARM_PARK.ToString, GlobalEnumerates.AXIS.Z).Value
                    Me.myScreenDelegate.Mixer2SecurityFly = ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.MIXER2_ARM_VSEC.ToString, GlobalEnumerates.AXIS.Z).Value
                    Me.Mixer2LimitZFine = CSng(Me.myScreenDelegate.Mixer2SecurityFly)
                End If
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".FillAdjustmentValuesintoDelegate ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".FillAdjustmentValuesintoDelegate ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Set all the screen buttons to false
    ''' </summary>
    ''' <remarks>SGM 24/01/11</remarks>
    Private Sub DisableButtons()
        Try
            'tab buttons
            Me.BsOpticAdjustButton.Enabled = False
            Me.BsWSAdjustButton.Enabled = False
            Me.BsArmsOkButton.Enabled = False

            Me.BsOpticCancelButton.Enabled = False
            Me.BsWSCancelButton.Enabled = False
            Me.BsArmsCancelButton.Enabled = False

            'common buttons
            Me.BsSaveButton.Enabled = False
            Me.BsExitButton.Enabled = False

            'SGM 01/10/2012
            Me.BsUpDownWSButton1.Enabled = False
            Me.BsUpDownWSButton2.Enabled = False
            Me.BsUpDownWSButton3.Enabled = False

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".DisableButtons ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".DisableButtons ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Set all the screen elements to disabled
    ''' </summary>
    ''' <remarks>SGM 24/01/11</remarks>
    Private Sub DisableAll()
        Try
            Select Case SelectedPage
                Case ADJUSTMENT_PAGES.OPTIC_CENTERING
                    Me.BsAdjustOptic.Enabled = False
                    ' XBC 28/03/2012
                    If Me.BsLEDCurrentTrackBar.Enabled Then
                        Me.BsLEDCurrentTrackBar.Enabled = False
                        MyBase.ActivateMDIMenusButtons(False)
                    End If
                    ' XBC 28/03/2012
                    Me.BsMinusLabel.Cursor = Cursors.WaitCursor
                    Me.BsPlusLabel.Cursor = Cursors.WaitCursor
                    ' XBC 30/11/2011
                    Me.BsUpDownWSButton1.Enabled = False


                Case ADJUSTMENT_PAGES.WASHING_STATION
                    'SGM 24/01/11
                    Me.BsAdjustWashing.Enabled = False
                    ' XBC 30/11/2011
                    Me.BsUpDownWSButton2.Enabled = False

                    ' XBC 28/03/2012
                    MyBase.ActivateMDIMenusButtons(False)

                Case ADJUSTMENT_PAGES.ARM_POSITIONS
                    Me.BsGridSample.Enabled = False
                    Me.BsGridReagent1.Enabled = False
                    Me.BsGridReagent2.Enabled = False
                    Me.BsGridMixer1.Enabled = False
                    Me.BsGridMixer2.Enabled = False
                    Me.BsAdjustPolar.Enabled = False
                    Me.BsAdjustZ.Enabled = False
                    Me.BsAdjustRotor.Enabled = False
                    ' XBC 30/11/2011
                    Me.BsUpDownWSButton3.Enabled = False

                    ' XBC 28/03/2012
                    MyBase.ActivateMDIMenusButtons(False)

            End Select

            DisableButtons() 'SGM 24/01/11

            ManageTabArms = False
            ManageTabPages = False

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".DeactivateAll ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".DeactivateAll ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' reset all homes
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>SGM 04/02/11</remarks>
    Private Function InitializeHomes() As GlobalDataTO
        Dim myGlobal As New GlobalDataTO
        Try
            myGlobal = myScreenDelegate.ResetAllPreliminaryHomes(MyBase.myServiceMDI.ActiveAnalyzer)

        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = Messages.SYSTEM_ERROR.ToString
            myGlobal.ErrorMessage = ex.Message
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".InitializeHomes ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".InitializeHomes", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
        Return myGlobal
    End Function

    ''' <summary>
    ''' Initializes all the screen adjustment controls
    ''' </summary>
    ''' <remarks>XBC 04/01/11</remarks>
    Private Sub InitializeAdjustControls()
        Try
            Select Case SelectedPage
                Case ADJUSTMENT_PAGES.OPTIC_CENTERING
                    With Me.BsAdjustOptic
                        .HomingEnabled = False ' True   ' XBC 04/01/2012 
                        .EditingEnabled = False ' True  ' XBC 04/01/2012 
                        .Enabled = False
                        .UnitsCaption = "steps"
                    End With

                Case ADJUSTMENT_PAGES.WASHING_STATION
                    With Me.BsAdjustWashing
                        .HomingEnabled = True
                        .EditingEnabled = True
                        .Enabled = False
                        .UnitsCaption = "steps"
                    End With

                Case ADJUSTMENT_PAGES.ARM_POSITIONS

                    Me.BsAdjustPolar.HomingEnabled = True
                    Me.BsAdjustPolar.EditingEnabled = True
                    Me.BsAdjustPolar.UnitsCaption = "steps"
                    Me.BsAdjustZ.HomingEnabled = True
                    Me.BsAdjustZ.EditingEnabled = True
                    Me.BsAdjustZ.UnitsCaption = "steps"
                    Me.BsAdjustRotor.HomingEnabled = True
                    Me.BsAdjustRotor.EditingEnabled = True
                    Me.BsAdjustRotor.UnitsCaption = "steps"

                    Select Case SelectedArmTab
                        Case ADJUSTMENT_ARMS.SAMPLE
                            ' Adjust Controls by each Aam
                            Me.BsAdjustPolar.Visible = True
                            Me.BsAdjustZ.Visible = True
                            Me.BsAdjustRotor.Visible = True
                            Me.BsAdjustPolar.Enabled = False
                            Me.BsAdjustZ.Enabled = False
                            Me.BsAdjustRotor.Enabled = False

                        Case ADJUSTMENT_ARMS.REAGENT1
                            ' Adjust Controls by each Aam
                            Me.BsAdjustPolar.Visible = True
                            Me.BsAdjustZ.Visible = True
                            Me.BsAdjustRotor.Visible = True
                            Me.BsAdjustPolar.Enabled = False
                            Me.BsAdjustZ.Enabled = False
                            Me.BsAdjustRotor.Enabled = False

                        Case ADJUSTMENT_ARMS.REAGENT2
                            ' Adjust Controls by each Aam
                            Me.BsAdjustPolar.Visible = True
                            Me.BsAdjustZ.Visible = True
                            Me.BsAdjustRotor.Visible = True
                            Me.BsAdjustPolar.Enabled = False
                            Me.BsAdjustZ.Enabled = False
                            Me.BsAdjustRotor.Enabled = False

                        Case ADJUSTMENT_ARMS.MIXER1
                            ' Adjust Controls by each Aam
                            Me.BsAdjustPolar.Visible = True
                            Me.BsAdjustZ.Visible = True
                            Me.BsAdjustRotor.Visible = True
                            Me.BsAdjustPolar.Enabled = False
                            Me.BsAdjustZ.Enabled = False
                            Me.BsAdjustRotor.Enabled = False


                        Case ADJUSTMENT_ARMS.MIXER2
                            ' Adjust Controls by each Aam
                            Me.BsAdjustPolar.Visible = True
                            Me.BsAdjustZ.Visible = True
                            Me.BsAdjustRotor.Visible = True
                            Me.BsAdjustPolar.Enabled = False
                            Me.BsAdjustZ.Enabled = False
                            Me.BsAdjustRotor.Enabled = False

                    End Select

                    Me.BsAdjustPolar.Refresh()
                    Me.BsAdjustZ.Refresh()
                    Me.BsAdjustRotor.Refresh()

            End Select

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".InitializeAdjustControls ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".InitializeAdjustControls ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Activates the Screen's adjust controls
    ''' </summary>
    ''' <param name="pValue"></param>
    ''' <remarks>XBC 04/01/11</remarks>
    Private Sub ActivateAdjustButtons(ByVal pValue As Boolean)
        Try
            Select Case Me.SelectedPage
                Case ADJUSTMENT_PAGES.OPTIC_CENTERING

                    If MyBase.CurrentUserNumericalLevel = USER_LEVEL.lOPERATOR Then
                        MyBase.myScreenLayout.ButtonsPanel.AdjustButton.Enabled = False
                    Else
                        MyBase.myScreenLayout.ButtonsPanel.AdjustButton.Enabled = pValue
                    End If

                Case ADJUSTMENT_PAGES.WASHING_STATION
                    If MyBase.CurrentUserNumericalLevel = USER_LEVEL.lOPERATOR Then
                        MyBase.myScreenLayout.ButtonsPanel.AdjustButton.Enabled = False
                    Else
                        MyBase.myScreenLayout.ButtonsPanel.AdjustButton.Enabled = pValue
                    End If

                Case ADJUSTMENT_PAGES.ARM_POSITIONS
                    Select Case SelectedArmTab
                        Case ADJUSTMENT_ARMS.SAMPLE
                            If Not Me.BsGridSample Is Nothing Then
                                If Me.BsGridSample.RowsCount > 0 Then
                                    For i As Integer = 0 To Me.BsGridSample.RowsCount - 1
                                        If Me.BsGridSample.EnableButton1(i) <> pValue Then
                                            If MyBase.CurrentUserNumericalLevel = USER_LEVEL.lOPERATOR Then
                                                Me.BsGridSample.EnableButton1(i) = False
                                            Else
                                                '' XBC 28/09/2011 by now Z-Tube is not able to configure by user
                                                If i = ISE_SAMPLE_ROW Then
                                                    ' XBC 28/03/2012
                                                    If Not AnalyzerController.Instance.Analyzer.ISEAnalyzer.IsISEModuleInstalled Then '#REFACTORING
                                                        ' Specified for ISE
                                                        Me.BsGridSample.EnableButton1(i) = False
                                                    Else
                                                        Me.BsGridSample.EnableButton1(i) = pValue
                                                    End If
                                                    ' XBC 28/03/2012

                                                Else
                                                    Me.BsGridSample.EnableButton1(i) = pValue
                                                End If
                                            End If
                                        End If
                                    Next
                                End If
                            End If
                        Case ADJUSTMENT_ARMS.REAGENT1
                            If Not Me.BsGridReagent1 Is Nothing Then
                                If Me.BsGridReagent1.RowsCount > 0 Then
                                    For i As Integer = 0 To Me.BsGridReagent1.RowsCount - 1
                                        If Me.BsGridReagent1.EnableButton1(i) <> pValue Then
                                            If MyBase.CurrentUserNumericalLevel = USER_LEVEL.lOPERATOR Then
                                                Me.BsGridReagent1.EnableButton1(i) = False
                                            Else
                                                Me.BsGridReagent1.EnableButton1(i) = pValue
                                            End If
                                        End If
                                    Next
                                End If
                            End If
                        Case ADJUSTMENT_ARMS.REAGENT2
                            If Not Me.BsGridReagent2 Is Nothing Then
                                If Me.BsGridReagent2.RowsCount > 0 Then
                                    For i As Integer = 0 To Me.BsGridReagent2.RowsCount - 1
                                        If Me.BsGridReagent2.EnableButton1(i) <> pValue Then
                                            If MyBase.CurrentUserNumericalLevel = USER_LEVEL.lOPERATOR Then
                                                Me.BsGridReagent2.EnableButton1(i) = False
                                            Else
                                                Me.BsGridReagent2.EnableButton1(i) = pValue
                                            End If
                                        End If
                                    Next
                                End If
                            End If
                        Case ADJUSTMENT_ARMS.MIXER1
                            If Not Me.BsGridMixer1 Is Nothing Then
                                If Me.BsGridMixer1.RowsCount > 0 Then
                                    For i As Integer = 0 To Me.BsGridMixer1.RowsCount - 1
                                        If Me.BsGridMixer1.EnableButton1(i) <> pValue Then
                                            If MyBase.CurrentUserNumericalLevel = USER_LEVEL.lOPERATOR Then
                                                Me.BsGridMixer1.EnableButton1(i) = False
                                            Else
                                                Me.BsGridMixer1.EnableButton1(i) = pValue
                                            End If
                                        End If
                                    Next
                                End If
                            End If
                        Case ADJUSTMENT_ARMS.MIXER2
                            If Not Me.BsGridMixer2 Is Nothing Then
                                If Me.BsGridMixer2.RowsCount > 0 Then
                                    For i As Integer = 0 To Me.BsGridMixer2.RowsCount - 1
                                        If Me.BsGridMixer2.EnableButton1(i) <> pValue Then
                                            If MyBase.CurrentUserNumericalLevel = USER_LEVEL.lOPERATOR Then
                                                Me.BsGridMixer2.EnableButton1(i) = False
                                            Else
                                                Me.BsGridMixer2.EnableButton1(i) = pValue
                                            End If
                                        End If
                                    Next
                                End If
                            End If
                    End Select

            End Select

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".ActivateAdjustButtons ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".ActivateAdjustButtons ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Initialize specific selected adjustment related structure
    ''' </summary>
    ''' <remarks>XBC 04/01/11</remarks>
    Private Sub InitializeEditedValue()
        Try
            ' Instantiate a new EditionValue structure
            Me.EditedValue = New EditedValueStruct

            With Me.EditedValue
                .CurrentPolarValue = Nothing
                .CurrentZValue = Nothing
                .CurrentRotorValue = Nothing

                .LastPolarValue = Nothing
                .LastZValue = Nothing
                .LastRotorValue = Nothing

                .NewPolarValue = Nothing
                .NewZValue = Nothing
                .NewRotorValue = Nothing

                ' XBC 03/01/2012 - Add Encoder functionality
                .LastEncoderValue = Nothing
                .NewEncoderValue = Nothing

                .canMovePolarValue = False
                .canMoveZValue = False
                .canMoveRotorValue = False

                .canSavePolarValue = False
                .canSaveZValue = False
                .canSaveRotorValue = False

            End With
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".InitializeEditedValue ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".InitializeEditedValue ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Moves the Absorbance Chart wells
    ''' </summary>
    ''' <param name="pSteps"></param>
    ''' <remarks>SGM 14/03/11</remarks>
    Private Sub MoveAbsorbanceChartWells(ByVal pSteps As Integer)
        Try

            Dim myDiagram As XYDiagram = CType(Me.AbsorbanceChart.Diagram, XYDiagram)
            For Each L As ConstantLine In myDiagram.AxisX.ConstantLines
                If L.Name.Contains("New") Then
                    L.AxisValue = CInt(L.AxisValue) + pSteps
                End If
                If L.Name.Contains("Center") Then
                    L.AxisValue = CInt(L.AxisValue) + pSteps
                End If
            Next

            For i As Integer = 0 To WellCenters.Count - 1
                WellCenters(i) += pSteps
            Next

            Me.CalculateDistances()

            'SGM 02/10/2012
            Me.BsAdjustOptic.Focus()

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".MoveAbsorbanceChartWells", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".MoveAbsorbanceChartWells ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Defines the needed parameters for the screen delegate in order to make an adjustment
    ''' </summary>
    ''' <param name="pMovement"></param>
    ''' <remarks>XBC 04/01/11
    ''' AG 01/10/2014 - BA-1953 new photometry adjustment maneuver (use REAGENTS_HOME_ROTOR + REAGENTS_ABS_ROTOR (with parameter = current value of GFWR1) instead of REACTIONS_ROTOR_HOME_WELL1)
    ''' </remarks>
    Private Sub MakeAdjustment(ByVal pMovement As MOVEMENT)
        Dim myGlobal As New GlobalDataTO
        Try
            Dim AdjustToDo As ADJUSTMENT_GROUPS
            Dim AxisToDo As GlobalEnumerates.AXIS
            Dim MovToDo As MOVEMENT
            Dim valueToDo As Single

            myGlobal = MyBase.Adjust()
            If myGlobal.HasError Then
                PrepareErrorMode()
            Else
                PrepareArea()

                With EditedValue
                    AdjustToDo = .AdjustmentID
                    AxisToDo = .AxisID
                    Select Case pMovement
                        Case MOVEMENT.ABSOLUTE
                            MovToDo = MOVEMENT.ABSOLUTE
                            Select Case AxisToDo
                                Case GlobalEnumerates.AXIS.POLAR
                                    valueToDo = .NewPolarValue
                                Case GlobalEnumerates.AXIS.Z
                                    valueToDo = .NewZValue
                                Case GlobalEnumerates.AXIS.ROTOR
                                    valueToDo = .NewRotorValue
                            End Select
                        Case MOVEMENT.RELATIVE
                            MovToDo = MOVEMENT.RELATIVE
                            valueToDo = .stepValue

                        Case MOVEMENT.HOME
                            MovToDo = MOVEMENT.HOME

                            'AG 01/10/2014 BA-1953 - in case photometric reactions rotor inform the current value of adjustment GFWR1 (Posición referencia lectura - Pocillo 1)
                            If Me.CurrentMode = ADJUSTMENT_MODES.ADJUSTING AndAlso AxisToDo = GlobalEnumerates.AXIS.ROTOR Then
                                myScreenDelegate.pValueAdjust = BsOpticAdjustmentLabel.Text
                            End If
                            'AG 01/10/2014 BA-1953
                    End Select

                End With

                ' Populating parameters into own Delegate
                myScreenDelegate.pAxisAdjust = AxisToDo
                myScreenDelegate.pMovAdjust = MovToDo
                myScreenDelegate.pValueAdjust = valueToDo.ToString


                ' Manage FwScripts must to be sent to adjusting
                Me.SendFwScript(Me.CurrentMode, AdjustToDo)

            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".MakeAdjustment ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".MakeAdjustment ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Get texts in the current application language for all screen controls
    ''' </summary>
    ''' <param name="pLanguageID">Current Application Language</param>
    ''' <remarks>
    ''' Created by: XBC 31/01/2011
    ''' </remarks>
    Private Sub GetScreenLabels(ByVal pLanguageID As String)
        Try
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            'For Labels, CheckBox, RadioButtons.....
            TabOpticCentering.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_OpticCenter", pLanguageID)
            TabWashingStation.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_WashStation", pLanguageID)
            TabArmsPositions.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_ArmsPositions", pLanguageID)

            BsOpticInfoTitle.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_INFO_TITLE", pLanguageID)
            BsWashingInfoTitle.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_INFO_TITLE", pLanguageID)
            BsArmsInfoTitle.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_INFO_TITLE", pLanguageID)

            BsOpticAdjustTitle.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_OPTIC_TITLE", pLanguageID)
            BsWashingAdjustTitle.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_POS_WS_TITLE", pLanguageID)
            BsArmsAdjustTitle.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_POS_ARMS_TITLE", pLanguageID)

            BsOpticAdjustmentTitle.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_OpticAdjustment", pLanguageID) & ":"
            BsEncoderAdjustmentTitle.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_Encoder", pLanguageID) & ":"
            BsLEDCurrentLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_Dacs", pLanguageID) & ":"

            BsWashingAdjustmentTitle.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_WSSaveValue", pLanguageID) & ":"

            TabSample.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, If(AnalyzerController.Instance.IsBA200, "LBL_SRV_SAMPLEREAGENT_ARM", "LBL_SRV_SampleArm"), pLanguageID)
            TabReagent1.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_Reagent1Arm", pLanguageID)
            TabReagent2.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_Reagent2Arm", pLanguageID)
            TabMixer1.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, If(AnalyzerController.Instance.IsBA200, "LBL_SRV_STIRRER", "LBL_SRV_Mixer1Arm"), pLanguageID)
            TabMixer2.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_Mixer2Arm", pLanguageID)

            CType(AbsorbanceChart.Diagram, XYDiagram).AxisY.Title.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_Counts", pLanguageID)
            CType(AbsorbanceChart.Diagram, XYDiagram).AxisX.Title.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_STEPS", pLanguageID)
            AbsorbanceChart.Series(0).LegendText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_Counts", pLanguageID)
            AbsorbanceChart.Series(1).LegendText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_Encoder", pLanguageID)

            BsOpticWSLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_PLACE_REACTIONS_ROTOR", pLanguageID)
            BsWSWSLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_PLACE_REACTIONS_ROTOR", pLanguageID)
            BsArmsWSLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_PLACE_REACTIONS_ROTOR", pLanguageID)

            BsAdjustOptic.RangeTitle = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_Range", pLanguageID) & ":"
            BsAdjustWashing.RangeTitle = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_Range", pLanguageID) & ":"
            BsAdjustPolar.RangeTitle = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_Range", pLanguageID) & ":"
            BsAdjustZ.RangeTitle = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_Range", pLanguageID) & ":"
            BsAdjustRotor.RangeTitle = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_Range", pLanguageID) & ":"


            'Information
            With BsInfoOptXPSViewer
                .PrintButtonCaption = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_XPS_PRINT", pLanguageID)
                .CopyButtonCaption = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_XPS_COPY", pLanguageID)
                .IncreaseZoomButtonCaption = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_XPS_ZOOM_IN", pLanguageID)
                .DecreaseZoomButtonCaption = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_XPS_ZOOM_OUT", pLanguageID)
                .FitToWidthButtonCaption = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_XPS_TO_WIDTH", pLanguageID)
                .FitToHeightButtonCaption = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_XPS_TO_HEIGHT", pLanguageID)
                .WholePageButtonCaption = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_XPS_WHOLE", pLanguageID)
                .TwoPagesButtonCaption = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_XPS_TWO_PAGES", pLanguageID)
            End With
            With BsInfoWsXPSViewer
                .PrintButtonCaption = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_XPS_PRINT", pLanguageID)
                .CopyButtonCaption = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_XPS_COPY", pLanguageID)
                .IncreaseZoomButtonCaption = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_XPS_ZOOM_IN", pLanguageID)
                .DecreaseZoomButtonCaption = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_XPS_ZOOM_OUT", pLanguageID)
                .FitToWidthButtonCaption = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_XPS_TO_WIDTH", pLanguageID)
                .FitToHeightButtonCaption = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_XPS_TO_HEIGHT", pLanguageID)
                .WholePageButtonCaption = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_XPS_WHOLE", pLanguageID)
                .TwoPagesButtonCaption = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_XPS_TWO_PAGES", pLanguageID)
            End With
            With BsInfoArmsXPSViewer
                .PrintButtonCaption = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_XPS_PRINT", pLanguageID)
                .CopyButtonCaption = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_XPS_COPY", pLanguageID)
                .IncreaseZoomButtonCaption = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_XPS_ZOOM_IN", pLanguageID)
                .DecreaseZoomButtonCaption = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_XPS_ZOOM_OUT", pLanguageID)
                .FitToWidthButtonCaption = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_XPS_TO_WIDTH", pLanguageID)
                .FitToHeightButtonCaption = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_XPS_TO_HEIGHT", pLanguageID)
                .WholePageButtonCaption = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_XPS_WHOLE", pLanguageID)
                .TwoPagesButtonCaption = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_XPS_TWO_PAGES", pLanguageID)
            End With

            ' Tooltips
            GetScreenTooltip()

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Name & ".GetScreenLabels", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".GetScreenLabels", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Get texts in the current application language for all screen controls
    ''' </summary>
    ''' <remarks>
    ''' Created by XBC 26/07/2011
    ''' Modified by XBC 30/11/2011 - Add Up/Down Washing Station functionality
    ''' Modified by XBC 30/11/2011 - Add functionality to Stop Optic Centering Adjustment
    ''' </remarks>
    Private Sub GetScreenTooltip()
        'Private Sub GetScreenTooltip(ByVal pLanguageID As String)
        Try
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            ' For Tooltips...

            MyBase.bsScreenToolTipsControl.SetToolTip(BsOpticAdjustButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_ADJUST", currentLanguage)) 'JB 01/10/2012 - Resource String unification
            MyBase.bsScreenToolTipsControl.SetToolTip(BsOpticCancelButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Cancel", currentLanguage))
            MyBase.bsScreenToolTipsControl.SetToolTip(BsOpticStopButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "SRV_BTN_TestStop", currentLanguage))

            MyBase.bsScreenToolTipsControl.SetToolTip(BsWSAdjustButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_ADJUST", currentLanguage)) 'JB 01/10/2012 - Resource String unification
            MyBase.bsScreenToolTipsControl.SetToolTip(BsWSCancelButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Cancel", currentLanguage))

            MyBase.bsScreenToolTipsControl.SetToolTip(BsArmsCancelButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Cancel", currentLanguage))
            MyBase.bsScreenToolTipsControl.SetToolTip(BsArmsOkButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_SAVE_LOCAL", currentLanguage))

            MyBase.bsScreenToolTipsControl.SetToolTip(BsSaveButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Save", currentLanguage))
            MyBase.bsScreenToolTipsControl.SetToolTip(BsExitButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_CloseScreen", currentLanguage))

            ' XBC 30/11/2011
            MyBase.bsScreenToolTipsControl.SetToolTip(BsUpDownWSButton1, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_UPDOWN_WS", currentLanguage))
            MyBase.bsScreenToolTipsControl.SetToolTip(BsUpDownWSButton2, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_UPDOWN_WS", currentLanguage))
            MyBase.bsScreenToolTipsControl.SetToolTip(BsUpDownWSButton3, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_UPDOWN_WS", currentLanguage))

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Name & ".GetScreenTooltip ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Name & ".GetScreenTooltip ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub ExitScreen()
        'Dim myGlobal As New GlobalDataTO
        Dim dialogResultToReturn As DialogResult = DialogResult.No
        Try
            If MyBase.CurrentMode = ADJUSTMENT_MODES.ERROR_MODE Then
                Me.Close()
                Exit Sub
            End If

            Me.WaitForScriptsExitingScreen = False

            Dim ChangesToSave As Boolean = False
            If Me.SelectedPage = ADJUSTMENT_PAGES.ARM_POSITIONS Then
                ChangesToSave = MyClass.ChangedValue Or MyClass.IsLocallySaved
            Else
                ChangesToSave = MyClass.ChangedValue
            End If

            If ChangesToSave Then
                dialogResultToReturn = MyBase.ShowMessage("", Messages.SRV_DISCARD_CHANGES.ToString)

                If dialogResultToReturn = DialogResult.Yes Then

                    Me.BsGridSample.HideAllIconsOk()
                    Me.BsGridReagent1.HideAllIconsOk()
                    Me.BsGridReagent2.HideAllIconsOk()
                    Me.BsGridMixer1.HideAllIconsOk()
                    Me.BsGridMixer2.HideAllIconsOk()

                    MyClass.IsLocallySaved = False
                    Me.ChangedValue = False
                    MyBase.DisplayMessage(Messages.SRV_ADJUSTMENTS_CANCELLED.ToString)

                    Me.WaitForScriptsExitingScreen = True

                    Me.CancelAdjustment() 'SGM 24/01/11
                End If

            Else

                Me.WaitForScriptsExitingScreen = True
                Me.CancelAdjustment()
                If MyBase.CurrentMode = ADJUSTMENT_MODES.LOADED Then
                    Me.FinishExitScreen()
                End If

            End If


        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".ExitScreen", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".ExitScreen", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Final operation to close screen
    ''' </summary>
    ''' <remarks>Created by XBC 07/12/2011</remarks>
    Private Sub FinishExitScreen()
        Dim myGlobal As New GlobalDataTO
        Dim dialogResultToReturn As DialogResult = DialogResult.No
        Try
            Dim ExitScreen As Boolean = False

            If Not WashingStationValidated Or Not AllArmsPositionsValidated Then
                dialogResultToReturn = MyBase.ShowMessage(GetMessageText(Messages.SRV_ADJUSTMENTS_TESTS.ToString), Messages.SRV_CHANGE_MANDATORY.ToString)
                If dialogResultToReturn = DialogResult.Yes Then
                    GlobalBase.CreateLogActivity("Leaving screen without complete Adjustment decided by user...", Me.Name & ".ExitScreen", EventLogEntryType.Information, False)
                    ExitScreen = True
                    myGlobal = myScreenDelegate.ReportExitWithOutFinishAdjustment()
                End If
            Else
                ExitScreen = True
            End If

            If ExitScreen Then
                OpticCenteringModified = False
                myGlobal = MyBase.CloseForm()
                If myGlobal.HasError Then
                    Me.PrepareErrorMode()
                Else
                    MyClass.IsReadyToCloseAttr = True
                    Me.Close()

                    Application.DoEvents()

                    If MyBase.CloseRequestedByMDI Then
                        MyBase.myServiceMDI.isWaitingForCloseApp = True
                        If MyBase.CloseWithShutDownRequestedByMDI Then
                            MyBase.myServiceMDI.WithShutToolStripMenuItem.PerformClick()
                        ElseIf MyBase.CloseWithoutShutDownRequestedByMDI Then
                            MyBase.myServiceMDI.WithOutShutDownToolStripMenuItem.PerformClick()
                        Else
                            Me.Close()
                        End If

                    End If

                    Exit Sub

                End If
            Else
                Me.PrepareLoadedMode()
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".FinishExitScreen", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".FinishExitScreen", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Load Adjustments High Level Instruction to move Washing Station
    ''' </summary>
    ''' <remarks>
    ''' Created by XBC 30/11/2011
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
                        Me.Cursor = Cursors.WaitCursor
                        Thread.Sleep(SimulationProcessTime)
                        MyBase.myServiceMDI.Focus()
                        Me.Cursor = Cursors.Default
                        MyBase.CurrentMode = ADJUSTMENT_MODES.PARKED
                        myScreenDelegate.IsWashingStationUp = Not myScreenDelegate.IsWashingStationUp
                        PrepareArea()
                    Else
                        ' Manage instruction for Washing Station UP/DOWN
                        If myScreenDelegate.IsWashingStationUp Then
                            myScreenDelegate.SendNEW_ROTOR()
                        Else
                            myScreenDelegate.SendWASH_STATION_CTRL(Ax00WashStationControlModes.UP)
                        End If
                    End If
                End If

            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".SendWASH_STATION_CTRL", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".SendWASH_STATION_CTRL", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

#Region "Generic Adjusments DS Methods"

    ''' <summary>
    ''' Update some value in the specific adjustments ds
    ''' </summary>
    ''' <param name="pCodew"></param>
    ''' <param name="pValue"></param>
    ''' <returns></returns>
    ''' <remarks>SG 31/01/11</remarks>
    Private Function UpdateSpecificAdjustmentsDS(ByVal pCodew As String, ByVal pValue As String) As GlobalDataTO
        Dim myGlobal As New GlobalDataTO
        Try
            For Each SR As SRVAdjustmentsDS.srv_tfmwAdjustmentsRow In Me.SelectedAdjustmentsDS.srv_tfmwAdjustments.Rows
                If SR.CodeFw.Trim = pCodew.Trim Then
                    SR.Value = pValue
                    Exit For
                End If
            Next
        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = Messages.SYSTEM_ERROR.ToString
            myGlobal.ErrorMessage = ex.Message
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".UpdateSpecificAdjustmentsDS ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".UpdateSpecificAdjustmentsDS", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
        Me.SelectedAdjustmentsDS.AcceptChanges()
        Return myGlobal
    End Function

    ''' <summary>
    ''' Update some value in the specific adjustments ds
    ''' </summary>
    ''' <param name="pCodew"></param>
    ''' <param name="pValue"></param>
    ''' <returns></returns>
    ''' <remarks>XBC 01/02/11</remarks>
    Private Function UpdateLocalSavedSpecificAdjustmentsDS(ByVal pCodew As String, ByVal pValue As String) As GlobalDataTO
        Dim myGlobal As New GlobalDataTO
        Try
            For Each SR As SRVAdjustmentsDS.srv_tfmwAdjustmentsRow In Me.LocalSavedAdjustmentsDS.srv_tfmwAdjustments.Rows
                If SR.CodeFw.Trim = pCodew.Trim Then
                    SR.BeginEdit()
                    SR.Value = pValue
                    SR.EndEdit()
                    Exit For
                End If
            Next
        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = Messages.SYSTEM_ERROR.ToString
            myGlobal.ErrorMessage = ex.Message
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".UpdateLocalSavedSpecificAdjustmentsDS ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".UpdateLocalSavedSpecificAdjustmentsDS", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
        Me.LocalSavedAdjustmentsDS.AcceptChanges()
        Return myGlobal
    End Function





    ''' <summary>
    ''' updates all a temporal dataset with changed current adjustments
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>XBC 01/02/11</remarks>
    Private Function UpdateLocalSavedAdjustmentsDS() As GlobalDataTO
        Dim myGlobal As New GlobalDataTO
        Try
            With Me.EditedValue
                Select Case Me.SelectedPage
                    Case ADJUSTMENT_PAGES.OPTIC_CENTERING
                        If .canSaveRotorValue Then
                            Me.UpdateLocalSavedSpecificAdjustmentsDS(ReadSpecificAdjustmentData(GlobalEnumerates.AXIS.ROTOR).CodeFw, .NewRotorValue.ToString)

                            ' XBC 03/01/2012 - Add Encoder functionality
                            Me.UpdateLocalSavedSpecificAdjustmentsDS(ReadSpecificAdjustmentData(GlobalEnumerates.AXIS.ENCODER).CodeFw, .NewEncoderValue.ToString)
                        End If

                    Case ADJUSTMENT_PAGES.WASHING_STATION
                        If .canSaveZValue Then
                            Me.UpdateLocalSavedSpecificAdjustmentsDS(ReadSpecificAdjustmentData(GlobalEnumerates.AXIS.Z).CodeFw, (.NewZValue).ToString)
                        End If

                    Case ADJUSTMENT_PAGES.ARM_POSITIONS
                        If .canSavePolarValue Then
                            Me.UpdateLocalSavedSpecificAdjustmentsDS(ReadSpecificAdjustmentData(GlobalEnumerates.AXIS.POLAR).CodeFw, .NewPolarValue.ToString)
                        End If
                        If .canSaveZValue Then
                            Me.UpdateLocalSavedSpecificAdjustmentsDS(ReadSpecificAdjustmentData(GlobalEnumerates.AXIS.Z).CodeFw, .NewZValue.ToString)

                            If ReadSpecificAdjustmentData(GlobalEnumerates.AXIS.Z).GroupID = ADJUSTMENT_GROUPS.REAGENT1_ARM_RING1.ToString AndAlso AnalyzerController.Instance.IsBA200 Then
                                Me.UpdateLocalSavedSpecificAdjustmentsDS(Ax00Adjustsments.R1PV2.ToString, .NewZValue.ToString)
                            End If

                            ' XBC 12/09/2011 - By now ZTube 2 & 3 takes the value of ZTube1
                            If ReadSpecificAdjustmentData(GlobalEnumerates.AXIS.Z).GroupID = ADJUSTMENT_GROUPS.SAMPLES_ARM_ZTUBE1.ToString Then
                                Me.UpdateLocalSavedSpecificAdjustmentsDS(ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.SAMPLES_ARM_ZTUBE2.ToString, GlobalEnumerates.AXIS.Z).CodeFw, .NewZValue.ToString)
                                Me.UpdateLocalSavedSpecificAdjustmentsDS(ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.SAMPLES_ARM_ZTUBE3.ToString, GlobalEnumerates.AXIS.Z).CodeFw, .NewZValue.ToString)
                            End If

                        End If
                        If .canSaveRotorValue Then
                            Me.UpdateLocalSavedSpecificAdjustmentsDS(ReadSpecificAdjustmentData(GlobalEnumerates.AXIS.ROTOR).CodeFw, .NewRotorValue.ToString)
                        End If
                End Select

            End With

        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = Messages.SYSTEM_ERROR.ToString
            myGlobal.ErrorMessage = ex.Message
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".UpdateLocalSavedAdjustmentsDS ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".UpdateLocalSavedAdjustmentsDS ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
        Return myGlobal
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="pNewAdjustmentsDS"></param>
    ''' <remarks>
    ''' Modified by XB 04/02/2013 - Upper conversions redundants because the value is already in UpperCase must delete to avoid Regional Settings problems (Bugs tracking #1112)
    ''' </remarks>
    Private Sub AddToLocalSavedAdjustmentsDS(ByVal pNewAdjustmentsDS As SRVAdjustmentsDS)
        'Dim myGlobal As New GlobalDataTO
        Try
            If MyClass.LocalSavedAdjustmentsDS IsNot Nothing AndAlso pNewAdjustmentsDS IsNot Nothing Then
                For Each dr2 As SRVAdjustmentsDS.srv_tfmwAdjustmentsRow In pNewAdjustmentsDS.srv_tfmwAdjustments.Rows
                    Dim Skip As Boolean = False
                    For Each dr1 As SRVAdjustmentsDS.srv_tfmwAdjustmentsRow In MyClass.LocalSavedAdjustmentsDS.srv_tfmwAdjustments.Rows
                        If dr1.CodeFw.Trim = dr2.CodeFw.Trim Then
                            Skip = True
                            Exit For
                        End If
                    Next dr1

                    If Not Skip Then

                        Dim myNewRow As SRVAdjustmentsDS.srv_tfmwAdjustmentsRow = MyClass.LocalSavedAdjustmentsDS.srv_tfmwAdjustments.Newsrv_tfmwAdjustmentsRow
                        With myNewRow
                            .BeginEdit()
                            .AnalyzerID = dr2.AnalyzerID
                            .AreaFw = dr2.AreaFw
                            .AxisID = dr2.AxisID
                            .CanMove = dr2.CanMove
                            .CanSave = dr2.CanSave
                            .CodeFw = dr2.CodeFw
                            .DescriptionFw = dr2.DescriptionFw
                            .FwVersion = dr2.FwVersion
                            .GroupID = dr2.GroupID
                            .InFile = dr2.InFile
                            .Value = dr2.Value
                            .EndEdit()
                        End With

                        MyClass.LocalSavedAdjustmentsDS.srv_tfmwAdjustments.Addsrv_tfmwAdjustmentsRow(myNewRow)
                        MyClass.LocalSavedAdjustmentsDS.AcceptChanges()

                    End If
                Next dr2
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".AddToLocalSavedAdjustmentsDS ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".AddToLocalSavedAdjustmentsDS ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Add offset ZRef values to save into Fw
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>
    ''' Created by XBC 27/02/2012
    ''' Modified by XB 12/11/2013 - Spec change on calculations formula for Arms adjustment by Zref offsets - BT #170 (SERVICE)
    ''' </remarks>
    Public Function SetAdditionalAdjustments(ByVal pAdjustmentsDS As SRVAdjustmentsDS) As GlobalDataTO
        Dim myGlobal As New GlobalDataTO

        Try
            Dim myTemporalAdjustmentsDS As New SRVAdjustmentsDS

            Dim myR1RVValue As String = "0"
            Dim myR2RVValue As String = "0"
            Dim myM1RVValue As String = "0"
            Dim myA1RVValue As String = "0"
            Dim myA2RVValue As String = "0"
            Dim myWSEVValue As String = "0"

            ' Takes a copy of the dataset of Adjustments
            myGlobal = myAdjustmentsDelegate.Clone(pAdjustmentsDS)
            If Not myGlobal.SetDatos Is Nothing AndAlso Not myGlobal.HasError Then
                myTemporalAdjustmentsDS = CType(myGlobal.SetDatos, SRVAdjustmentsDS)
            End If

            If Not myTemporalAdjustmentsDS Is Nothing Then
                For Each R As SRVAdjustmentsDS.srv_tfmwAdjustmentsRow In myTemporalAdjustmentsDS.srv_tfmwAdjustments.Rows

                    If Not AnalyzerController.Instance.IsBA200() Then
                        ' REAGENT 1
                        If UCase(R.CodeFw.Trim) = Ax00Adjustsments.R1RV.ToString Then
                            SetAdditionalAdjustmentsForReagent1(myTemporalAdjustmentsDS, R, myR1RVValue)
                        End If

                        ' REAGENT 2
                        If UCase(R.CodeFw.Trim) = Ax00Adjustsments.R2RV.ToString Then
                            SetAdditionalAdjustmentsForReagent2(myTemporalAdjustmentsDS, R, myR2RVValue)
                        End If

                        ' SAMPLES 
                        If UCase(R.CodeFw.Trim) = Ax00Adjustsments.M1RV.ToString Then
                            SetAdditionalAdjustmentsForSample(myTemporalAdjustmentsDS, R, myM1RVValue)
                        End If

                        ' MIXER 2
                        If UCase(R.CodeFw.Trim) = Ax00Adjustsments.A2RV.ToString Then
                            SetAdditionalAdjustmentsForMixer2(myTemporalAdjustmentsDS, R, myA2RVValue)
                        End If
                    Else
                        If UCase(R.CodeFw.Trim) = Ax00Adjustsments.R1RV.ToString Then
                            SetAdditionalAdjustmentsForBa200Arm(myTemporalAdjustmentsDS, R, myR1RVValue)
                        End If
                    End If
                    ' MIXER 1
                    If UCase(R.CodeFw.Trim) = Ax00Adjustsments.A1RV.ToString Then
                        SetAdditionalAdjustmentsForMixer1(myTemporalAdjustmentsDS, R, myA1RVValue)
                    End If

                    ' WASHING STATION
                    If UCase(R.CodeFw.Trim) = Ax00Adjustsments.WSEV.ToString Then
                        SetAdditionalAdjustmentsForWaqshingStation(myTemporalAdjustmentsDS, R, myWSEVValue)
                    End If
                Next
            End If
        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = Messages.SYSTEM_ERROR.ToString
            myGlobal.ErrorMessage = ex.Message
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".SetAdditionalAdjustments ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".SetAdditionalAdjustments ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
        Return myGlobal
    End Function

    Private Sub SetAdditionalAdjustmentsForWaqshingStation(ByVal myTemporalAdjustmentsDS As SRVAdjustmentsDS, ByVal R As SRVAdjustmentsDS.srv_tfmwAdjustmentsRow, ByVal myWSEVValue As String)
        Dim myNewRow As SRVAdjustmentsDS.srv_tfmwAdjustmentsRow

        If IsNumeric(R.Value) Then
            myWSEVValue = R.Value
        End If
        ' add WSRR offset
        myNewRow = myTemporalAdjustmentsDS.srv_tfmwAdjustments.Newsrv_tfmwAdjustmentsRow
        With myNewRow
            .AnalyzerID = R.AnalyzerID
            .FwVersion = R.FwVersion
            .GroupID = ADJUSTMENT_GROUPS.WASHING_STATION.ToString
            .CodeFw = Ax00Adjustsments.WSRR.ToString
            .Value = (CSng(myWSEVValue) + myScreenDelegate.WashingStationRR_ZOffset).ToString
            .AxisID = GlobalEnumerates.AXIS.REL_Z.ToString
            .CanSave = True
            .CanMove = False
            .InFile = True
        End With
        Me.TempToSendAdjustmentsDelegate.AddNewRowToDS(myNewRow)
    End Sub

    Private Sub SetAdditionalAdjustmentsForMixer2(ByVal myTemporalAdjustmentsDS As SRVAdjustmentsDS, ByVal R As SRVAdjustmentsDS.srv_tfmwAdjustmentsRow, ByVal myA2RVValue As String)
        Dim myNewRow As SRVAdjustmentsDS.srv_tfmwAdjustmentsRow
        Dim myA2SVValue As String

        If IsNumeric(R.Value) Then
            myA2RVValue = R.Value
        End If
        ' add A2SV offset
        myNewRow = myTemporalAdjustmentsDS.srv_tfmwAdjustments.Newsrv_tfmwAdjustmentsRow
        With myNewRow
            .AnalyzerID = R.AnalyzerID
            .FwVersion = R.FwVersion
            .GroupID = ADJUSTMENT_GROUPS.MIXER2_ARM_VSEC.ToString
            .CodeFw = Ax00Adjustsments.A2SV.ToString
            .Value = (CSng(myA2RVValue) + myScreenDelegate.Mixer2SV_ZOffset).ToString
            myA2SVValue = .Value
            .AxisID = GlobalEnumerates.AXIS.Z.ToString
            .CanSave = True
            .CanMove = False
            .InFile = True
        End With
        Me.TempToSendAdjustmentsDelegate.AddNewRowToDS(myNewRow)

        ' add A2WVR offset
        myNewRow = myTemporalAdjustmentsDS.srv_tfmwAdjustments.Newsrv_tfmwAdjustmentsRow
        With myNewRow
            .AnalyzerID = R.AnalyzerID
            .FwVersion = R.FwVersion
            .GroupID = ADJUSTMENT_GROUPS.MIXER2_ARM_WASH.ToString
            .CodeFw = Ax00Adjustsments.A2WVR.ToString
            .Value = (CSng(myA2RVValue) + myScreenDelegate.Mixer2WVR_ZOffset - CSng(myA2SVValue)).ToString
            .AxisID = GlobalEnumerates.AXIS.Z.ToString
            .CanSave = True
            .CanMove = False
            .InFile = True
        End With
        Me.TempToSendAdjustmentsDelegate.AddNewRowToDS(myNewRow)

        ' add A2DV offset
        myNewRow = myTemporalAdjustmentsDS.srv_tfmwAdjustments.Newsrv_tfmwAdjustmentsRow
        With myNewRow
            .AnalyzerID = R.AnalyzerID
            .FwVersion = R.FwVersion
            .GroupID = ADJUSTMENT_GROUPS.MIXER2_ARM_DISP1.ToString
            .CodeFw = Ax00Adjustsments.A2DV.ToString
            .Value = (CSng(myA2RVValue) + myScreenDelegate.Mixer2DV_ZOffset).ToString
            .AxisID = GlobalEnumerates.AXIS.Z.ToString
            .CanSave = True
            .CanMove = False
            .InFile = True
        End With
        Me.TempToSendAdjustmentsDelegate.AddNewRowToDS(myNewRow)
    End Sub

    Private Sub SetAdditionalAdjustmentsForMixer1(ByVal myTemporalAdjustmentsDS As SRVAdjustmentsDS, ByVal R As SRVAdjustmentsDS.srv_tfmwAdjustmentsRow, ByVal myA1RVValue As String)
        Dim myNewRow As SRVAdjustmentsDS.srv_tfmwAdjustmentsRow
        Dim myA1SVValue As String

        If IsNumeric(R.Value) Then
            myA1RVValue = R.Value
        End If
        ' add A1SV offset
        myNewRow = myTemporalAdjustmentsDS.srv_tfmwAdjustments.Newsrv_tfmwAdjustmentsRow
        With myNewRow
            .AnalyzerID = R.AnalyzerID
            .FwVersion = R.FwVersion
            .GroupID = ADJUSTMENT_GROUPS.MIXER1_ARM_VSEC.ToString
            .CodeFw = Ax00Adjustsments.A1SV.ToString
            .Value = (CSng(myA1RVValue) + myScreenDelegate.Mixer1SV_ZOffset).ToString
            myA1SVValue = .Value
            .AxisID = GlobalEnumerates.AXIS.Z.ToString
            .CanSave = True
            .CanMove = False
            .InFile = True
        End With
        Me.TempToSendAdjustmentsDelegate.AddNewRowToDS(myNewRow)

        ' add A1WVR offset
        myNewRow = myTemporalAdjustmentsDS.srv_tfmwAdjustments.Newsrv_tfmwAdjustmentsRow
        With myNewRow
            .AnalyzerID = R.AnalyzerID
            .FwVersion = R.FwVersion
            .GroupID = ADJUSTMENT_GROUPS.MIXER1_ARM_WASH.ToString
            .CodeFw = Ax00Adjustsments.A1WVR.ToString
            .Value = (CSng(myA1RVValue) + myScreenDelegate.Mixer1WVR_ZOffset - CSng(myA1SVValue)).ToString
            .AxisID = GlobalEnumerates.AXIS.Z.ToString
            .CanSave = True
            .CanMove = False
            .InFile = True
        End With
        Me.TempToSendAdjustmentsDelegate.AddNewRowToDS(myNewRow)

        ' add A1DV offset
        myNewRow = myTemporalAdjustmentsDS.srv_tfmwAdjustments.Newsrv_tfmwAdjustmentsRow
        With myNewRow
            .AnalyzerID = R.AnalyzerID
            .FwVersion = R.FwVersion
            .GroupID = ADJUSTMENT_GROUPS.MIXER1_ARM_DISP1.ToString
            .CodeFw = Ax00Adjustsments.A1DV.ToString
            .Value = (CSng(myA1RVValue) + myScreenDelegate.Mixer1DV_ZOffset).ToString
            .AxisID = GlobalEnumerates.AXIS.Z.ToString
            .CanSave = True
            .CanMove = False
            .InFile = True
        End With
        Me.TempToSendAdjustmentsDelegate.AddNewRowToDS(myNewRow)
    End Sub

    Private Sub SetAdditionalAdjustmentsForSample(ByVal myTemporalAdjustmentsDS As SRVAdjustmentsDS, ByVal R As SRVAdjustmentsDS.srv_tfmwAdjustmentsRow, ByVal myM1RVValue As String)
        Dim myNewRow As SRVAdjustmentsDS.srv_tfmwAdjustmentsRow
        Dim myM1SVValue As String

        If IsNumeric(R.Value) Then
            myM1RVValue = R.Value
        End If
        ' add M1RV offset
        myNewRow = myTemporalAdjustmentsDS.srv_tfmwAdjustments.Newsrv_tfmwAdjustmentsRow
        With myNewRow
            .AnalyzerID = R.AnalyzerID
            .FwVersion = R.FwVersion
            .GroupID = ADJUSTMENT_GROUPS.SAMPLES_ARM_VSECWS.ToString
            .CodeFw = Ax00Adjustsments.M1SV.ToString

            ' XB 12/11/2013
            .Value = myScreenDelegate.SampleSV_ZOffset.ToString

            myM1SVValue = .Value
            .AxisID = GlobalEnumerates.AXIS.Z.ToString
            .CanSave = True
            .CanMove = False
            .InFile = True
        End With
        Me.TempToSendAdjustmentsDelegate.AddNewRowToDS(myNewRow)

        ' add M1WVR offset
        myNewRow = myTemporalAdjustmentsDS.srv_tfmwAdjustments.Newsrv_tfmwAdjustmentsRow
        With myNewRow
            .AnalyzerID = R.AnalyzerID
            .FwVersion = R.FwVersion
            .GroupID = ADJUSTMENT_GROUPS.SAMPLES_ARM_WASH.ToString
            .CodeFw = Ax00Adjustsments.M1WVR.ToString
            .Value = (CSng(myM1RVValue) + myScreenDelegate.SampleWVR_ZOffset - CSng(myM1SVValue)).ToString
            .AxisID = GlobalEnumerates.AXIS.Z.ToString
            .CanSave = True
            .CanMove = False
            .InFile = True
        End With
        Me.TempToSendAdjustmentsDelegate.AddNewRowToDS(myNewRow)

        ' add M1DV1 offset
        myNewRow = myTemporalAdjustmentsDS.srv_tfmwAdjustments.Newsrv_tfmwAdjustmentsRow
        With myNewRow
            .AnalyzerID = R.AnalyzerID
            .FwVersion = R.FwVersion
            .GroupID = ADJUSTMENT_GROUPS.SAMPLES_ARM_DISP1.ToString
            .CodeFw = Ax00Adjustsments.M1DV1.ToString
            .Value = (CSng(myM1RVValue) + myScreenDelegate.SampleDV1_ZOffset).ToString
            .AxisID = GlobalEnumerates.AXIS.Z.ToString
            .CanSave = True
            .CanMove = False
            .InFile = True
        End With
        Me.TempToSendAdjustmentsDelegate.AddNewRowToDS(myNewRow)

        ' add M1PI offset
        myNewRow = myTemporalAdjustmentsDS.srv_tfmwAdjustments.Newsrv_tfmwAdjustmentsRow
        With myNewRow
            .AnalyzerID = R.AnalyzerID
            .FwVersion = R.FwVersion
            .GroupID = ADJUSTMENT_GROUPS.SAMPLES_ARM_LEVEL_DET.ToString
            .CodeFw = Ax00Adjustsments.M1PI.ToString
            .Value = (CSng(myM1RVValue) + myScreenDelegate.SamplePI_ZOffset).ToString
            .AxisID = GlobalEnumerates.AXIS.Z.ToString
            .CanSave = True
            .CanMove = False
            .InFile = True
        End With
        Me.TempToSendAdjustmentsDelegate.AddNewRowToDS(myNewRow)

        ' add M1RPI offset
        myNewRow = myTemporalAdjustmentsDS.srv_tfmwAdjustments.Newsrv_tfmwAdjustmentsRow
        With myNewRow
            .AnalyzerID = R.AnalyzerID
            .FwVersion = R.FwVersion
            .GroupID = ADJUSTMENT_GROUPS.SAMPLES_ARM_LEVEL_DET.ToString
            .CodeFw = Ax00Adjustsments.M1RPI.ToString
            .Value = (CSng(myM1RVValue) + myScreenDelegate.SampleRPI_ZOffset).ToString
            .AxisID = GlobalEnumerates.AXIS.ROTOR.ToString
            .CanSave = True
            .CanMove = False
            .InFile = True
        End With
        Me.TempToSendAdjustmentsDelegate.AddNewRowToDS(myNewRow)

        ' add M1DV2 offset
        myNewRow = myTemporalAdjustmentsDS.srv_tfmwAdjustments.Newsrv_tfmwAdjustmentsRow
        With myNewRow
            .AnalyzerID = R.AnalyzerID
            .FwVersion = R.FwVersion
            .GroupID = ADJUSTMENT_GROUPS.SAMPLES_ARM_DISP2.ToString
            .CodeFw = Ax00Adjustsments.M1DV2.ToString
            .Value = (CSng(myM1RVValue) + myScreenDelegate.SampleDV2_ZOffset).ToString
            .AxisID = GlobalEnumerates.AXIS.Z.ToString
            .CanSave = True
            .CanMove = False
            .InFile = True
        End With
        Me.TempToSendAdjustmentsDelegate.AddNewRowToDS(myNewRow)
    End Sub

    Private Sub SetAdditionalAdjustmentsForReagent2(ByVal myTemporalAdjustmentsDS As SRVAdjustmentsDS, ByVal R As SRVAdjustmentsDS.srv_tfmwAdjustmentsRow, ByVal myR2RVValue As String)
        Dim myNewRow As SRVAdjustmentsDS.srv_tfmwAdjustmentsRow
        Dim myR2SVValue As String

        If IsNumeric(R.Value) Then
            myR2RVValue = R.Value
        End If
        ' add R2SV offset
        myNewRow = myTemporalAdjustmentsDS.srv_tfmwAdjustments.Newsrv_tfmwAdjustmentsRow
        With myNewRow
            .AnalyzerID = R.AnalyzerID
            .FwVersion = R.FwVersion
            .GroupID = ADJUSTMENT_GROUPS.REAGENT2_ARM_VSEC.ToString
            .CodeFw = Ax00Adjustsments.R2SV.ToString

            ' XB 12/11/2013
            .Value = myScreenDelegate.Reagent2SV_ZOffset.ToString

            myR2SVValue = .Value
            .AxisID = GlobalEnumerates.AXIS.Z.ToString
            .CanSave = True
            .CanMove = False
            .InFile = True
        End With
        Me.TempToSendAdjustmentsDelegate.AddNewRowToDS(myNewRow)

        ' add R2WVR offset
        myNewRow = myTemporalAdjustmentsDS.srv_tfmwAdjustments.Newsrv_tfmwAdjustmentsRow
        With myNewRow
            .AnalyzerID = R.AnalyzerID
            .FwVersion = R.FwVersion
            .GroupID = ADJUSTMENT_GROUPS.REAGENT2_ARM_WASH.ToString
            .CodeFw = Ax00Adjustsments.R2WVR.ToString
            .Value = (CSng(myR2RVValue) + myScreenDelegate.Reagent2WVR_ZOffset - CSng(myR2SVValue)).ToString
            .AxisID = GlobalEnumerates.AXIS.Z.ToString
            .CanSave = True
            .CanMove = False
            .InFile = True
        End With
        Me.TempToSendAdjustmentsDelegate.AddNewRowToDS(myNewRow)

        ' add R2DV offset
        myNewRow = myTemporalAdjustmentsDS.srv_tfmwAdjustments.Newsrv_tfmwAdjustmentsRow
        With myNewRow
            .AnalyzerID = R.AnalyzerID
            .FwVersion = R.FwVersion
            .GroupID = ADJUSTMENT_GROUPS.REAGENT2_ARM_DISP1.ToString
            .CodeFw = Ax00Adjustsments.R2DV.ToString
            .Value = (CSng(myR2RVValue) + myScreenDelegate.Reagent2DV_ZOffset).ToString
            .AxisID = GlobalEnumerates.AXIS.Z.ToString
            .CanSave = True
            .CanMove = False
            .InFile = True
        End With
        Me.TempToSendAdjustmentsDelegate.AddNewRowToDS(myNewRow)

        ' add R2PI offset
        myNewRow = myTemporalAdjustmentsDS.srv_tfmwAdjustments.Newsrv_tfmwAdjustmentsRow
        With myNewRow
            .AnalyzerID = R.AnalyzerID
            .FwVersion = R.FwVersion
            .GroupID = ADJUSTMENT_GROUPS.REAGENT2_ARM_LEVEL.ToString
            .CodeFw = Ax00Adjustsments.R2PI.ToString
            .Value = (CSng(myR2RVValue) + myScreenDelegate.Reagent2PI_ZOffset).ToString
            .AxisID = GlobalEnumerates.AXIS.Z.ToString
            .CanSave = True
            .CanMove = False
            .InFile = True
        End With
        Me.TempToSendAdjustmentsDelegate.AddNewRowToDS(myNewRow)
    End Sub

    Private Sub SetAdditionalAdjustmentsForReagent1(ByVal myTemporalAdjustmentsDS As SRVAdjustmentsDS, ByVal R As SRVAdjustmentsDS.srv_tfmwAdjustmentsRow, ByVal myR1RVValue As String)
        Dim myNewRow As SRVAdjustmentsDS.srv_tfmwAdjustmentsRow
        Dim myR1SVValue As String

        If IsNumeric(R.Value) Then
            myR1RVValue = R.Value
        End If
        ' add R1SV offset
        myNewRow = myTemporalAdjustmentsDS.srv_tfmwAdjustments.Newsrv_tfmwAdjustmentsRow
        With myNewRow
            .AnalyzerID = R.AnalyzerID
            .FwVersion = R.FwVersion
            .GroupID = ADJUSTMENT_GROUPS.REAGENT1_ARM_VSEC.ToString
            .CodeFw = Ax00Adjustsments.R1SV.ToString

            ' XB 12/11/2013
            .Value = myScreenDelegate.Reagent1SV_ZOffset.ToString

            myR1SVValue = .Value
            .AxisID = GlobalEnumerates.AXIS.Z.ToString
            .CanSave = True
            .CanMove = False
            .InFile = True
        End With
        Me.TempToSendAdjustmentsDelegate.AddNewRowToDS(myNewRow)

        ' add R1WVR offset
        myNewRow = myTemporalAdjustmentsDS.srv_tfmwAdjustments.Newsrv_tfmwAdjustmentsRow
        With myNewRow
            .AnalyzerID = R.AnalyzerID
            .FwVersion = R.FwVersion
            .GroupID = ADJUSTMENT_GROUPS.REAGENT1_ARM_WASH.ToString
            .CodeFw = Ax00Adjustsments.R1WVR.ToString
            .Value = (CSng(myR1RVValue) + myScreenDelegate.Reagent1WVR_ZOffset - CSng(myR1SVValue)).ToString
            .AxisID = GlobalEnumerates.AXIS.Z.ToString
            .CanSave = True
            .CanMove = False
            .InFile = True
        End With
        Me.TempToSendAdjustmentsDelegate.AddNewRowToDS(myNewRow)

        ' add R1DV offset
        myNewRow = myTemporalAdjustmentsDS.srv_tfmwAdjustments.Newsrv_tfmwAdjustmentsRow
        With myNewRow
            .AnalyzerID = R.AnalyzerID
            .FwVersion = R.FwVersion
            .GroupID = ADJUSTMENT_GROUPS.REAGENT1_ARM_DISP1.ToString
            .CodeFw = Ax00Adjustsments.R1DV.ToString
            .Value = (CSng(myR1RVValue) + myScreenDelegate.Reagent1DV_ZOffset).ToString
            .AxisID = GlobalEnumerates.AXIS.Z.ToString
            .CanSave = True
            .CanMove = False
            .InFile = True
        End With
        Me.TempToSendAdjustmentsDelegate.AddNewRowToDS(myNewRow)

        ' add R1PI offset
        myNewRow = myTemporalAdjustmentsDS.srv_tfmwAdjustments.Newsrv_tfmwAdjustmentsRow
        With myNewRow
            .AnalyzerID = R.AnalyzerID
            .FwVersion = R.FwVersion
            .GroupID = ADJUSTMENT_GROUPS.REAGENT1_ARM_LEVEL.ToString
            .CodeFw = Ax00Adjustsments.R1PI.ToString
            .Value = (CSng(myR1RVValue) + myScreenDelegate.Reagent1PI_ZOffset).ToString
            .AxisID = GlobalEnumerates.AXIS.Z.ToString
            .CanSave = True
            .CanMove = False
            .InFile = True
        End With
        Me.TempToSendAdjustmentsDelegate.AddNewRowToDS(myNewRow)

    End Sub

    'New functionallity for Ba200
    Private Sub SetAdditionalAdjustmentsForBa200Arm(ByVal myTemporalAdjustmentsDS As SRVAdjustmentsDS, ByVal R As SRVAdjustmentsDS.srv_tfmwAdjustmentsRow, ByVal myR1RVValue As String)

        SetAdditionalAdjustmentsForReagent1(myTemporalAdjustmentsDS, R, myR1RVValue)

        Dim myNewRow As SRVAdjustmentsDS.srv_tfmwAdjustmentsRow

        If IsNumeric(R.Value) Then
            myR1RVValue = R.Value
        End If

        ' add R2DV offset
        myNewRow = myTemporalAdjustmentsDS.srv_tfmwAdjustments.Newsrv_tfmwAdjustmentsRow
        With myNewRow
            .AnalyzerID = R.AnalyzerID
            .FwVersion = R.FwVersion
            .GroupID = ADJUSTMENT_GROUPS.REAGENT2_ARM_DISP1.ToString
            .CodeFw = Ax00Adjustsments.R2DV.ToString
            .Value = (CSng(myR1RVValue) + myScreenDelegate.Reagent2DV_ZOffset).ToString
            .AxisID = GlobalEnumerates.AXIS.Z.ToString
            .CanSave = True
            .CanMove = False
            .InFile = True
        End With
        Me.TempToSendAdjustmentsDelegate.AddNewRowToDS(myNewRow)

        ' add M1PI offset
        myNewRow = myTemporalAdjustmentsDS.srv_tfmwAdjustments.Newsrv_tfmwAdjustmentsRow
        With myNewRow
            .AnalyzerID = R.AnalyzerID
            .FwVersion = R.FwVersion
            .GroupID = ADJUSTMENT_GROUPS.SAMPLES_ARM_LEVEL_DET.ToString
            .CodeFw = Ax00Adjustsments.M1PI.ToString
            .Value = (CSng(myR1RVValue) + myScreenDelegate.SamplePI_ZOffset).ToString
            .AxisID = GlobalEnumerates.AXIS.Z.ToString
            .CanSave = True
            .CanMove = False
            .InFile = True
        End With
        Me.TempToSendAdjustmentsDelegate.AddNewRowToDS(myNewRow)

        ' add M1RPI offset
        myNewRow = myTemporalAdjustmentsDS.srv_tfmwAdjustments.Newsrv_tfmwAdjustmentsRow
        With myNewRow
            .AnalyzerID = R.AnalyzerID
            .FwVersion = R.FwVersion
            .GroupID = ADJUSTMENT_GROUPS.SAMPLES_ARM_LEVEL_DET.ToString
            .CodeFw = Ax00Adjustsments.M1RPI.ToString
            .Value = (CSng(myR1RVValue) + myScreenDelegate.SampleRPI_ZOffset).ToString
            .AxisID = GlobalEnumerates.AXIS.ROTOR.ToString
            .CanSave = True
            .CanMove = False
            .InFile = True
        End With
        Me.TempToSendAdjustmentsDelegate.AddNewRowToDS(myNewRow)

        ' add M1DV2 offset
        myNewRow = myTemporalAdjustmentsDS.srv_tfmwAdjustments.Newsrv_tfmwAdjustmentsRow
        With myNewRow
            .AnalyzerID = R.AnalyzerID
            .FwVersion = R.FwVersion
            .GroupID = ADJUSTMENT_GROUPS.SAMPLES_ARM_DISP2.ToString
            .CodeFw = Ax00Adjustsments.M1DV2.ToString
            .Value = (CSng(myR1RVValue) + myScreenDelegate.SampleDV2_ZOffset).ToString
            .AxisID = GlobalEnumerates.AXIS.Z.ToString
            .CanSave = True
            .CanMove = False
            .InFile = True
        End With
        Me.TempToSendAdjustmentsDelegate.AddNewRowToDS(myNewRow)

    End Sub

    ''' <summary>
    ''' Gets the value corresponding to informed Group and Axis from global adjustments dataset
    ''' </summary>
    ''' <param name="pAxis"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' Created by SGM 28/01/11
    ''' Modified by XB 04/02/2013 - Upper conversions redundants because the value is already in UpperCase must delete to avoid Regional Settings problems (Bugs tracking #1112)
    ''' </remarks>
    Private Function ReadGlobalAdjustmentData(ByVal pGroupID As String, ByVal pAxis As GlobalEnumerates.AXIS, Optional ByVal pNotForDisplaying As Boolean = False) As AdjustmentRowData
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

            If myAdjustmentRows.Count > 0 Then
                With myAdjustmentRowData
                    .CodeFw = myAdjustmentRows(0).CodeFw
                    .Value = myAdjustmentRows(0).Value
                    .GroupID = myAdjustmentRows(0).GroupID
                    .CanSave = myAdjustmentRows(0).CanSave
                    .CanMove = myAdjustmentRows(0).CanMove
                End With
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".ReadGlobalAdjustmentValue ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".ReadGlobalAdjustmentValue ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try

        ' If is not defined set to 0
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
    ''' Created by SGM 28/01/11
    ''' Modified by XB 04/02/2013 - Upper conversions redundants because the value is already in UpperCase must delete to avoid Regional Settings problems (Bugs tracking #1112)
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

            If myAdjustmentRows.Count > 0 Then
                With myAdjustmentRowData
                    .CodeFw = myAdjustmentRows(0).CodeFw
                    .Value = myAdjustmentRows(0).Value
                    .GroupID = myAdjustmentRows(0).GroupID
                    .CanSave = myAdjustmentRows(0).CanSave
                    .CanMove = myAdjustmentRows(0).CanMove
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
    ''' <remarks>SGM 28/01/11</remarks>
    Private Function LoadAdjustmentGroupData() As GlobalDataTO

        Dim resultData As New GlobalDataTO
        Dim CopyOfSelectedAdjustmentsDS As SRVAdjustmentsDS = Me.SelectedAdjustmentsDS
        Dim myAdjustmentsGroups As New List(Of String)
        Try
            Dim myAdjustmentID As ADJUSTMENT_GROUPS = EditedValue.AdjustmentID

            'SGM 29/02/2012 add only if not exists
            If Not myAdjustmentsGroups.Contains(Me.SelectedAdjustmentGroup.ToString) Then
                myAdjustmentsGroups.Add(Me.SelectedAdjustmentGroup.ToString)
            End If

            ' XBC 12/09/2011 - By now ZTube 2 & 3 takes the value of ZTube1
            If myAdjustmentID = ADJUSTMENT_GROUPS.SAMPLES_ARM_ZTUBE1 Then
                myAdjustmentsGroups.Add(ADJUSTMENT_GROUPS.SAMPLES_ARM_ZTUBE2.ToString)
                myAdjustmentsGroups.Add(ADJUSTMENT_GROUPS.SAMPLES_ARM_ZTUBE3.ToString)
            End If

            resultData = MyBase.myAdjustmentsDelegate.ReadAdjustmentsByGroupIDs(myAdjustmentsGroups)
            If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                Me.SelectedAdjustmentsDS = CType(resultData.SetDatos, SRVAdjustmentsDS)
            End If

            If AnalyzerController.Instance.IsBA200 AndAlso myAdjustmentID = ADJUSTMENT_GROUPS.REAGENT1_ARM_RING1 Then
                SelectedAdjustmentsDS.srv_tfmwAdjustments.ImportRow(myAdjustmentsDelegate.ReadFirstAdjustmentValueByCode(Ax00Adjustsments.R1PH1.ToString))
            End If

        Catch ex As Exception
            Me.SelectedAdjustmentsDS = CopyOfSelectedAdjustmentsDS
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".LoadAdjustmentGroupData ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".LoadAdjustmentGroupData ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
        Return resultData
    End Function

    ''' <summary>
    ''' Gets the Dataset that corresponds to all Arm Positions Data
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>SGM 28/01/11</remarks>
    Private Function LoadAllArmsPositionsAdjustmentData() As GlobalDataTO

        Dim resultData As New GlobalDataTO
        Dim CopyOfSelectedAdjustmentsDS As SRVAdjustmentsDS = Me.SelectedAdjustmentsDS
        Dim myAdjustmentsGroups As New List(Of String)
        Try

            Select Case Me.SelectedArmTab

                Case ADJUSTMENT_ARMS.SAMPLE
                    myAdjustmentsGroups.Add(ADJUSTMENT_GROUPS.SAMPLES_ARM_DISP1.ToString)
                    myAdjustmentsGroups.Add(ADJUSTMENT_GROUPS.SAMPLES_ARM_DISP2.ToString)
                    myAdjustmentsGroups.Add(ADJUSTMENT_GROUPS.SAMPLES_ARM_ISE.ToString)
                    myAdjustmentsGroups.Add(ADJUSTMENT_GROUPS.SAMPLES_ARM_PARK.ToString)
                    myAdjustmentsGroups.Add(ADJUSTMENT_GROUPS.SAMPLES_ARM_RING1.ToString)
                    myAdjustmentsGroups.Add(ADJUSTMENT_GROUPS.SAMPLES_ARM_RING2.ToString)
                    myAdjustmentsGroups.Add(ADJUSTMENT_GROUPS.SAMPLES_ARM_RING3.ToString)
                    myAdjustmentsGroups.Add(ADJUSTMENT_GROUPS.SAMPLES_ARM_WASH.ToString)
                    myAdjustmentsGroups.Add(ADJUSTMENT_GROUPS.SAMPLES_ARM_ZREF.ToString)
                    myAdjustmentsGroups.Add(ADJUSTMENT_GROUPS.SAMPLES_ARM_ZTUBE1.ToString)
                    ' XBC 12/09/2011 - By now ZTube 2 & 3 takes the value of ZTube1
                    myAdjustmentsGroups.Add(ADJUSTMENT_GROUPS.SAMPLES_ARM_ZTUBE2.ToString)
                    myAdjustmentsGroups.Add(ADJUSTMENT_GROUPS.SAMPLES_ARM_ZTUBE3.ToString)
                    'If AnalyzerController.Instance.IsBA200 Then myAdjustmentsGroups.Add(ADJUSTMENT_GROUPS.REAG.ToString)

                Case ADJUSTMENT_ARMS.REAGENT1
                    myAdjustmentsGroups.Add(ADJUSTMENT_GROUPS.REAGENT1_ARM_DISP1.ToString)
                    myAdjustmentsGroups.Add(ADJUSTMENT_GROUPS.REAGENT1_ARM_ZREF.ToString)
                    myAdjustmentsGroups.Add(ADJUSTMENT_GROUPS.REAGENT1_ARM_WASH.ToString)
                    myAdjustmentsGroups.Add(ADJUSTMENT_GROUPS.REAGENT1_ARM_RING1.ToString)
                    myAdjustmentsGroups.Add(ADJUSTMENT_GROUPS.REAGENT1_ARM_RING2.ToString)
                    myAdjustmentsGroups.Add(ADJUSTMENT_GROUPS.REAGENT1_ARM_PARK.ToString)

                Case ADJUSTMENT_ARMS.REAGENT2
                    myAdjustmentsGroups.Add(ADJUSTMENT_GROUPS.REAGENT2_ARM_DISP1.ToString)
                    myAdjustmentsGroups.Add(ADJUSTMENT_GROUPS.REAGENT2_ARM_ZREF.ToString)
                    myAdjustmentsGroups.Add(ADJUSTMENT_GROUPS.REAGENT2_ARM_WASH.ToString)
                    myAdjustmentsGroups.Add(ADJUSTMENT_GROUPS.REAGENT2_ARM_RING1.ToString)
                    myAdjustmentsGroups.Add(ADJUSTMENT_GROUPS.REAGENT2_ARM_RING2.ToString)
                    myAdjustmentsGroups.Add(ADJUSTMENT_GROUPS.REAGENT2_ARM_PARK.ToString)

                Case ADJUSTMENT_ARMS.MIXER1
                    myAdjustmentsGroups.Add(ADJUSTMENT_GROUPS.MIXER1_ARM_DISP1.ToString)
                    myAdjustmentsGroups.Add(ADJUSTMENT_GROUPS.MIXER1_ARM_PARK.ToString)
                    myAdjustmentsGroups.Add(ADJUSTMENT_GROUPS.MIXER1_ARM_WASH.ToString)
                    myAdjustmentsGroups.Add(ADJUSTMENT_GROUPS.MIXER1_ARM_ZREF.ToString)

                Case ADJUSTMENT_ARMS.MIXER2
                    myAdjustmentsGroups.Add(ADJUSTMENT_GROUPS.MIXER2_ARM_DISP1.ToString)
                    myAdjustmentsGroups.Add(ADJUSTMENT_GROUPS.MIXER2_ARM_PARK.ToString)
                    myAdjustmentsGroups.Add(ADJUSTMENT_GROUPS.MIXER2_ARM_WASH.ToString)
                    myAdjustmentsGroups.Add(ADJUSTMENT_GROUPS.MIXER2_ARM_ZREF.ToString)


            End Select

            Me.CurrentArmPositionsAdjustmentsDS.Clear()

            ' XBC 22/11/2011
            If Not myAdjustmentsGroups.Contains(Me.SelectedAdjustmentGroup.ToString) Then
                myAdjustmentsGroups.Add(Me.SelectedAdjustmentGroup.ToString)
            End If

            resultData = MyBase.myAdjustmentsDelegate.ReadAdjustmentsByGroupIDs(myAdjustmentsGroups)
            If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                Me.CurrentArmPositionsAdjustmentsDS = CType(resultData.SetDatos, SRVAdjustmentsDS)
            End If

        Catch ex As Exception
            Me.SelectedAdjustmentsDS = CopyOfSelectedAdjustmentsDS
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".LoadAllArmsAdjustmentData ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".LoadAllArmsAdjustmentData ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
        Return resultData
    End Function

#End Region

#End Region

#Region "History Methods"

    ''' <summary>
    ''' Updates the screen delegate's properties used for History management
    ''' </summary>
    ''' <param name="pTask"></param>
    ''' <param name="pResult"></param>
    ''' <remarks>Created by SGM 02/08/2011</remarks>
    Private Sub ReportHistory(Optional ByVal pTask As HISTORY_TASKS = HISTORY_TASKS.ADJUSTMENT, _
                              Optional ByVal pResult As PositionsAdjustmentDelegate.HISTORY_RESULTS = PositionsAdjustmentDelegate.HISTORY_RESULTS.NOT_DONE, _
                              Optional ByVal pIsLastArmAction As Boolean = False)
        Try

            ' XBC 22/11/2011
            Dim HistoryResultsToManage As Boolean = False

            With Me.myScreenDelegate

                Select Case Me.SelectedPage

                    Case ADJUSTMENT_PAGES.OPTIC_CENTERING

                        Me.ClearHistoryData(PositionsAdjustmentDelegate.HISTORY_AREAS.OPT)

                        If pTask = HISTORY_TASKS.ADJUSTMENT Then

                            .HistoryTask = PreloadedMasterDataEnum.SRV_ACT_ADJ_TYPES
                            .HistoryArea = PositionsAdjustmentDelegate.HISTORY_AREAS.OPT

                            Select Case MyBase.CurrentMode

                                Case ADJUSTMENT_MODES.SAVED
                                    .HisOpticCenteringAdjResult = pResult
                                    .HisOpticCenteringValue = CInt(Me.EditedValue.LastRotorValue)
                                    ' XBC 05/01/2012 - Add Encoder functionality
                                    .HisEncoderValue = CInt(Me.EditedValue.LastEncoderValue)
                                    .HisLEDIntensity = CInt(.LEDIntensity)
                                    .HisWaveLength = Me.myScreenDelegate.WaveLength
                                    .HisNumWells = Me.myScreenDelegate.NumWells
                                    .HisStepsbyWell = Me.myScreenDelegate.StepsbyWell

                                    ' XBC 22/11/2011
                                    HistoryResultsToManage = True
                            End Select

                        ElseIf pTask = HISTORY_TASKS.TEST Then

                            .HistoryTask = PreloadedMasterDataEnum.SRV_ACT_TEST_TYPES
                            .HistoryArea = PositionsAdjustmentDelegate.HISTORY_AREAS.OPT

                            Select Case MyBase.CurrentMode

                                Case ADJUSTMENT_MODES.ABSORBANCE_SCANNED
                                    .HisOpticCenteringTestResult = pResult
                                    .HisOpticCenteringValue = CInt(Me.EditedValue.CurrentRotorValue)
                                    ' XBC 05/01/2012 - Add Encoder functionality
                                    .HisEncoderValue = CInt(Me.EditedValue.LastEncoderValue)
                                    .HisLEDIntensity = CInt(.LEDIntensity)
                                    .HisWaveLength = Me.myScreenDelegate.WaveLength
                                    .HisNumWells = Me.myScreenDelegate.NumWells
                                    .HisStepsbyWell = Me.myScreenDelegate.StepsbyWell

                                    ' XBC 22/11/2011
                                    HistoryResultsToManage = True
                            End Select

                        End If

                        ' XBC 22/11/2011
                        If HistoryResultsToManage Then
                            Me.myScreenDelegate.ManageHistoryResults()
                        End If



                    Case ADJUSTMENT_PAGES.WASHING_STATION

                        Me.ClearHistoryData(PositionsAdjustmentDelegate.HISTORY_AREAS.WS_POS)

                        If pTask = HISTORY_TASKS.ADJUSTMENT Then

                            .HistoryTask = PreloadedMasterDataEnum.SRV_ACT_ADJ_TYPES
                            .HistoryArea = PositionsAdjustmentDelegate.HISTORY_AREAS.WS_POS

                            Select Case MyBase.CurrentMode

                                Case ADJUSTMENT_MODES.SAVED
                                    .HisWashingStationAdjResult = pResult
                                    .HisWashingStationValue = CInt(Me.EditedValue.LastZValue)

                                    ' XBC 22/11/2011
                                    HistoryResultsToManage = True
                            End Select



                        ElseIf pTask = HISTORY_TASKS.TEST Then

                            .HistoryTask = PreloadedMasterDataEnum.SRV_ACT_TEST_TYPES
                            .HistoryArea = PositionsAdjustmentDelegate.HISTORY_AREAS.WS_POS

                            Select Case MyBase.CurrentMode

                                Case ADJUSTMENT_MODES.TESTED
                                    .HisWashingStationTestResult = pResult
                                    .HisWashingStationValue = CInt(Me.EditedValue.CurrentZValue)

                                    ' XBC 22/11/2011
                                    HistoryResultsToManage = True
                            End Select

                        End If

                        ' XBC 22/11/2011
                        If HistoryResultsToManage Then
                            Me.myScreenDelegate.ManageHistoryResults()
                        End If




                    Case ADJUSTMENT_PAGES.ARM_POSITIONS

                        'arm position name
                        Select Case SelectedAdjustmentGroup
                            Case ADJUSTMENT_GROUPS.SAMPLES_ARM_DISP1, _
                                 ADJUSTMENT_GROUPS.REAGENT1_ARM_DISP1, _
                                 ADJUSTMENT_GROUPS.REAGENT2_ARM_DISP1, _
                                 ADJUSTMENT_GROUPS.MIXER1_ARM_DISP1, _
                                 ADJUSTMENT_GROUPS.MIXER2_ARM_DISP1

                                .HisArmPosName = PositionsAdjustmentDelegate.HISTORY_ARM_POSITIONS.Dispensation

                                ' XBC 22/11/2011
                                HistoryResultsToManage = True

                            Case ADJUSTMENT_GROUPS.SAMPLES_ARM_DISP2
                                .HisArmPosName = PositionsAdjustmentDelegate.HISTORY_ARM_POSITIONS.Predilution

                                ' XBC 22/11/2011
                                HistoryResultsToManage = True

                            Case ADJUSTMENT_GROUPS.SAMPLES_ARM_ZREF, ADJUSTMENT_GROUPS.REAGENT1_ARM_ZREF, _
                                ADJUSTMENT_GROUPS.REAGENT2_ARM_ZREF, _
                                ADJUSTMENT_GROUPS.MIXER1_ARM_ZREF, _
                                ADJUSTMENT_GROUPS.MIXER2_ARM_ZREF
                                .HisArmPosName = PositionsAdjustmentDelegate.HISTORY_ARM_POSITIONS.Z_Ref

                                ' XBC 22/11/2011
                                HistoryResultsToManage = True

                            Case ADJUSTMENT_GROUPS.SAMPLES_ARM_WASH, _
                                ADJUSTMENT_GROUPS.REAGENT1_ARM_WASH, _
                                ADJUSTMENT_GROUPS.REAGENT2_ARM_WASH, _
                                ADJUSTMENT_GROUPS.MIXER1_ARM_WASH, _
                                ADJUSTMENT_GROUPS.MIXER2_ARM_WASH
                                .HisArmPosName = PositionsAdjustmentDelegate.HISTORY_ARM_POSITIONS.Washing

                                ' XBC 22/11/2011
                                HistoryResultsToManage = True

                            Case ADJUSTMENT_GROUPS.SAMPLES_ARM_RING1, ADJUSTMENT_GROUPS.REAGENT1_ARM_RING1, ADJUSTMENT_GROUPS.REAGENT2_ARM_RING1
                                .HisArmPosName = PositionsAdjustmentDelegate.HISTORY_ARM_POSITIONS.Ring1

                                ' XBC 22/11/2011
                                HistoryResultsToManage = True

                            Case ADJUSTMENT_GROUPS.SAMPLES_ARM_RING2, ADJUSTMENT_GROUPS.REAGENT1_ARM_RING2, ADJUSTMENT_GROUPS.REAGENT2_ARM_RING2
                                .HisArmPosName = PositionsAdjustmentDelegate.HISTORY_ARM_POSITIONS.Ring2

                                ' XBC 22/11/2011
                                HistoryResultsToManage = True

                            Case ADJUSTMENT_GROUPS.SAMPLES_ARM_RING3
                                .HisArmPosName = PositionsAdjustmentDelegate.HISTORY_ARM_POSITIONS.Ring3

                                ' XBC 22/11/2011
                                HistoryResultsToManage = True

                            Case ADJUSTMENT_GROUPS.SAMPLES_ARM_ZTUBE1
                                .HisArmPosName = PositionsAdjustmentDelegate.HISTORY_ARM_POSITIONS.Z_Tube

                                ' XBC 22/11/2011
                                HistoryResultsToManage = True

                            Case ADJUSTMENT_GROUPS.SAMPLES_ARM_ISE
                                .HisArmPosName = PositionsAdjustmentDelegate.HISTORY_ARM_POSITIONS.ISE_Pos

                                ' XBC 22/11/2011
                                HistoryResultsToManage = True

                            Case ADJUSTMENT_GROUPS.SAMPLES_ARM_PARK, _
                                ADJUSTMENT_GROUPS.REAGENT1_ARM_PARK, _
                                ADJUSTMENT_GROUPS.REAGENT2_ARM_PARK, _
                                ADJUSTMENT_GROUPS.MIXER1_ARM_PARK, _
                                ADJUSTMENT_GROUPS.MIXER2_ARM_PARK
                                .HisArmPosName = PositionsAdjustmentDelegate.HISTORY_ARM_POSITIONS.Parking

                                ' XBC 22/11/2011
                                HistoryResultsToManage = True

                        End Select


                        ' XBC 24/11/2011  
                        If Me.IsStirrerTesting Then
                            pIsLastArmAction = True
                        End If

                        If Not pIsLastArmAction Then
                            'arm name
                            Select Case Me.SelectedArmTab
                                Case ADJUSTMENT_ARMS.SAMPLE
                                    .HistoryArea = PositionsAdjustmentDelegate.HISTORY_AREAS.SAM_POS
                                Case ADJUSTMENT_ARMS.REAGENT1
                                    .HistoryArea = PositionsAdjustmentDelegate.HISTORY_AREAS.REG1_POS
                                Case ADJUSTMENT_ARMS.REAGENT2
                                    .HistoryArea = PositionsAdjustmentDelegate.HISTORY_AREAS.REG2_POS
                                Case ADJUSTMENT_ARMS.MIXER1
                                    .HistoryArea = PositionsAdjustmentDelegate.HISTORY_AREAS.MIX1_POS
                                Case ADJUSTMENT_ARMS.MIXER2
                                    .HistoryArea = PositionsAdjustmentDelegate.HISTORY_AREAS.MIX2_POS

                            End Select

                            If pTask = HISTORY_TASKS.ADJUSTMENT Then
                                .HistoryTask = PreloadedMasterDataEnum.SRV_ACT_ADJ_TYPES
                                If .HisArmAdjResults.ContainsKey(.HisArmPosName) Then
                                    .HisArmAdjResults(.HisArmPosName) = pResult
                                Else
                                    .HisArmAdjResults.Add(.HisArmPosName, pResult)
                                End If


                            ElseIf pTask = HISTORY_TASKS.TEST Then
                                .HistoryTask = PreloadedMasterDataEnum.SRV_ACT_TEST_TYPES
                                If .HisArmTestResults.ContainsKey(.HisArmPosName) Then
                                    .HisArmTestResults(.HisArmPosName) = pResult
                                Else
                                    .HisArmTestResults.Add(.HisArmPosName, pResult)
                                End If
                            End If


                            Me.ReportHistoryArmPositions(pTask, .HisArmPosName, Me.CurrentArmPositionsAdjustmentsDS, Me.SelectedAdjustmentsDS, pResult)

                        Else

                            ' XBC 24/11/2011
                            If .HistoryArea = PositionsAdjustmentDelegate.HISTORY_AREAS.NONE Or _
                               .HistoryArea = PositionsAdjustmentDelegate.HISTORY_AREAS.MIX1_TEST Or _
                               .HistoryArea = PositionsAdjustmentDelegate.HISTORY_AREAS.MIX2_TEST Then

                                ' Nothing to do
                            Else
                                ' XBC 24/11/2011

                                If .HisArmAdjResults.Count > 0 Then
                                    'complete all positions adjustments results
                                    If Not .HisArmAdjResults.ContainsKey(PositionsAdjustmentDelegate.HISTORY_ARM_POSITIONS.Dispensation) Then .HisArmAdjResults.Add(PositionsAdjustmentDelegate.HISTORY_ARM_POSITIONS.Dispensation, PositionsAdjustmentDelegate.HISTORY_RESULTS.NOT_DONE)
                                    If Not .HisArmAdjResults.ContainsKey(PositionsAdjustmentDelegate.HISTORY_ARM_POSITIONS.Predilution) Then .HisArmAdjResults.Add(PositionsAdjustmentDelegate.HISTORY_ARM_POSITIONS.Predilution, PositionsAdjustmentDelegate.HISTORY_RESULTS.NOT_DONE)
                                    If Not .HisArmAdjResults.ContainsKey(PositionsAdjustmentDelegate.HISTORY_ARM_POSITIONS.Z_Ref) Then .HisArmAdjResults.Add(PositionsAdjustmentDelegate.HISTORY_ARM_POSITIONS.Z_Ref, PositionsAdjustmentDelegate.HISTORY_RESULTS.NOT_DONE)
                                    If Not .HisArmAdjResults.ContainsKey(PositionsAdjustmentDelegate.HISTORY_ARM_POSITIONS.Washing) Then .HisArmAdjResults.Add(PositionsAdjustmentDelegate.HISTORY_ARM_POSITIONS.Washing, PositionsAdjustmentDelegate.HISTORY_RESULTS.NOT_DONE)
                                    If Not .HisArmAdjResults.ContainsKey(PositionsAdjustmentDelegate.HISTORY_ARM_POSITIONS.Ring1) Then .HisArmAdjResults.Add(PositionsAdjustmentDelegate.HISTORY_ARM_POSITIONS.Ring1, PositionsAdjustmentDelegate.HISTORY_RESULTS.NOT_DONE)
                                    If Not .HisArmAdjResults.ContainsKey(PositionsAdjustmentDelegate.HISTORY_ARM_POSITIONS.Ring2) Then .HisArmAdjResults.Add(PositionsAdjustmentDelegate.HISTORY_ARM_POSITIONS.Ring2, PositionsAdjustmentDelegate.HISTORY_RESULTS.NOT_DONE)
                                    If Not .HisArmAdjResults.ContainsKey(PositionsAdjustmentDelegate.HISTORY_ARM_POSITIONS.Ring3) Then .HisArmAdjResults.Add(PositionsAdjustmentDelegate.HISTORY_ARM_POSITIONS.Ring3, PositionsAdjustmentDelegate.HISTORY_RESULTS.NOT_DONE)
                                    If Not .HisArmAdjResults.ContainsKey(PositionsAdjustmentDelegate.HISTORY_ARM_POSITIONS.Z_Tube) Then .HisArmAdjResults.Add(PositionsAdjustmentDelegate.HISTORY_ARM_POSITIONS.Z_Tube, PositionsAdjustmentDelegate.HISTORY_RESULTS.NOT_DONE)
                                    If Not .HisArmAdjResults.ContainsKey(PositionsAdjustmentDelegate.HISTORY_ARM_POSITIONS.ISE_Pos) Then .HisArmAdjResults.Add(PositionsAdjustmentDelegate.HISTORY_ARM_POSITIONS.ISE_Pos, PositionsAdjustmentDelegate.HISTORY_RESULTS.NOT_DONE)
                                    If Not .HisArmAdjResults.ContainsKey(PositionsAdjustmentDelegate.HISTORY_ARM_POSITIONS.Parking) Then .HisArmAdjResults.Add(PositionsAdjustmentDelegate.HISTORY_ARM_POSITIONS.Parking, PositionsAdjustmentDelegate.HISTORY_RESULTS.NOT_DONE)
                                End If

                                If .HisArmTestResults.Count > 0 Then
                                    'complete all positions adjustments tests
                                    If Not .HisArmTestResults.ContainsKey(PositionsAdjustmentDelegate.HISTORY_ARM_POSITIONS.Dispensation) Then .HisArmTestResults.Add(PositionsAdjustmentDelegate.HISTORY_ARM_POSITIONS.Dispensation, PositionsAdjustmentDelegate.HISTORY_RESULTS.NOT_DONE)
                                    If Not .HisArmTestResults.ContainsKey(PositionsAdjustmentDelegate.HISTORY_ARM_POSITIONS.Predilution) Then .HisArmTestResults.Add(PositionsAdjustmentDelegate.HISTORY_ARM_POSITIONS.Predilution, PositionsAdjustmentDelegate.HISTORY_RESULTS.NOT_DONE)
                                    If Not .HisArmTestResults.ContainsKey(PositionsAdjustmentDelegate.HISTORY_ARM_POSITIONS.Z_Ref) Then .HisArmTestResults.Add(PositionsAdjustmentDelegate.HISTORY_ARM_POSITIONS.Z_Ref, PositionsAdjustmentDelegate.HISTORY_RESULTS.NOT_DONE)
                                    If Not .HisArmTestResults.ContainsKey(PositionsAdjustmentDelegate.HISTORY_ARM_POSITIONS.Washing) Then .HisArmTestResults.Add(PositionsAdjustmentDelegate.HISTORY_ARM_POSITIONS.Washing, PositionsAdjustmentDelegate.HISTORY_RESULTS.NOT_DONE)
                                    If Not .HisArmTestResults.ContainsKey(PositionsAdjustmentDelegate.HISTORY_ARM_POSITIONS.Ring1) Then .HisArmTestResults.Add(PositionsAdjustmentDelegate.HISTORY_ARM_POSITIONS.Ring1, PositionsAdjustmentDelegate.HISTORY_RESULTS.NOT_DONE)
                                    If Not .HisArmTestResults.ContainsKey(PositionsAdjustmentDelegate.HISTORY_ARM_POSITIONS.Ring2) Then .HisArmTestResults.Add(PositionsAdjustmentDelegate.HISTORY_ARM_POSITIONS.Ring2, PositionsAdjustmentDelegate.HISTORY_RESULTS.NOT_DONE)
                                    If Not .HisArmTestResults.ContainsKey(PositionsAdjustmentDelegate.HISTORY_ARM_POSITIONS.Ring3) Then .HisArmTestResults.Add(PositionsAdjustmentDelegate.HISTORY_ARM_POSITIONS.Ring3, PositionsAdjustmentDelegate.HISTORY_RESULTS.NOT_DONE)
                                    If Not .HisArmTestResults.ContainsKey(PositionsAdjustmentDelegate.HISTORY_ARM_POSITIONS.Z_Tube) Then .HisArmTestResults.Add(PositionsAdjustmentDelegate.HISTORY_ARM_POSITIONS.Z_Tube, PositionsAdjustmentDelegate.HISTORY_RESULTS.NOT_DONE)
                                    If Not .HisArmTestResults.ContainsKey(PositionsAdjustmentDelegate.HISTORY_ARM_POSITIONS.ISE_Pos) Then .HisArmTestResults.Add(PositionsAdjustmentDelegate.HISTORY_ARM_POSITIONS.ISE_Pos, PositionsAdjustmentDelegate.HISTORY_RESULTS.NOT_DONE)
                                    If Not .HisArmTestResults.ContainsKey(PositionsAdjustmentDelegate.HISTORY_ARM_POSITIONS.Parking) Then .HisArmTestResults.Add(PositionsAdjustmentDelegate.HISTORY_ARM_POSITIONS.Parking, PositionsAdjustmentDelegate.HISTORY_RESULTS.NOT_DONE)
                                End If

                                ' XBC 22/11/2011
                                If HistoryResultsToManage Then
                                    Me.myScreenDelegate.ManageHistoryResults()
                                End If

                                Me.ClearHistoryData(.HistoryArea, pIsLastArmAction)
                            End If


                        End If


                        ' XBC 24/11/2011
                        If Me.IsStirrerTesting Then

                            'stirrer test

                            Me.ClearHistoryData(.HistoryArea)

                            .HistoryTask = PreloadedMasterDataEnum.SRV_ACT_TEST_TYPES

                            Select Case Me.SelectedArmTab
                                Case ADJUSTMENT_ARMS.MIXER1
                                    .HistoryArea = PositionsAdjustmentDelegate.HISTORY_AREAS.MIX1_TEST

                                    ' XBC 22/11/2011
                                    HistoryResultsToManage = True

                                Case ADJUSTMENT_ARMS.MIXER2
                                    .HistoryArea = PositionsAdjustmentDelegate.HISTORY_AREAS.MIX2_TEST

                                    ' XBC 22/11/2011
                                    HistoryResultsToManage = True
                            End Select

                            .HisStirrerTestResult = pResult

                            ' XBC 22/11/2011
                            If HistoryResultsToManage Then
                                Me.myScreenDelegate.ManageHistoryResults()
                            End If

                        End If

                End Select

            End With



        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".ReportHistory ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".ReportHistory ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try

    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="pTask"></param>
    ''' <param name="pPosition"></param>
    ''' <param name="pAllAxesValues"></param>
    ''' <param name="pNewAxesValues"></param>
    ''' <param name="pResult"></param>
    ''' <remarks>
    ''' Modified by XB 04/02/2013 - Upper conversions redundants because the value is already in UpperCase must delete to avoid Regional Settings problems (Bugs tracking #1112)
    ''' </remarks>
    Private Sub ReportHistoryArmPositions(ByVal pTask As HISTORY_TASKS, _
                                          ByVal pPosition As PositionsAdjustmentDelegate.HISTORY_ARM_POSITIONS, _
                                          ByVal pAllAxesValues As SRVAdjustmentsDS, _
                                          ByVal pNewAxesValues As SRVAdjustmentsDS, _
                                          ByVal pResult As PositionsAdjustmentDelegate.HISTORY_RESULTS)
        Try
            With Me.myScreenDelegate


                'All Arm's Positions Adjustment Values
                Dim myAllAdjustments As New List(Of SRVAdjustmentsDS.srv_tfmwAdjustmentsRow)

                ' XBC 22/11/2011
                myAllAdjustments = (From a As SRVAdjustmentsDS.srv_tfmwAdjustmentsRow _
                                    In pAllAxesValues.srv_tfmwAdjustments _
                                    Select a).ToList


                'Adjustment Values corresponded to Arm's Position that is being adjusted/tested
                Dim myGroup As String = "_"
                Select Case pPosition
                    Case PositionsAdjustmentDelegate.HISTORY_ARM_POSITIONS.Dispensation : myGroup = "_DISP1"
                    Case PositionsAdjustmentDelegate.HISTORY_ARM_POSITIONS.Predilution : myGroup = "_DISP2"
                    Case PositionsAdjustmentDelegate.HISTORY_ARM_POSITIONS.Z_Ref : myGroup = "_ZREF"
                    Case PositionsAdjustmentDelegate.HISTORY_ARM_POSITIONS.Washing : myGroup = "_WASH"
                    Case PositionsAdjustmentDelegate.HISTORY_ARM_POSITIONS.Ring1 : myGroup = "_RING1"
                    Case PositionsAdjustmentDelegate.HISTORY_ARM_POSITIONS.Ring2 : myGroup = "_RING2"
                    Case PositionsAdjustmentDelegate.HISTORY_ARM_POSITIONS.Ring3 : myGroup = "_RING3"
                    Case PositionsAdjustmentDelegate.HISTORY_ARM_POSITIONS.Z_Tube : myGroup = "_ZTUBE1"
                    Case PositionsAdjustmentDelegate.HISTORY_ARM_POSITIONS.ISE_Pos : myGroup = "_ISE"
                    Case PositionsAdjustmentDelegate.HISTORY_ARM_POSITIONS.Parking : myGroup = "_PARK"

                End Select

                Dim myNewAdjustments As New List(Of SRVAdjustmentsDS.srv_tfmwAdjustmentsRow)

                ' XBC 22/11/2011
                myNewAdjustments = (From a As SRVAdjustmentsDS.srv_tfmwAdjustmentsRow _
                    In pNewAxesValues.srv_tfmwAdjustments _
                    Where a.GroupID.Trim.EndsWith(myGroup) _
                    Select a).ToList


                Dim myAxesValues As New List(Of Integer)

                Dim myGroupID As String = ""

                For Each R As SRVAdjustmentsDS.srv_tfmwAdjustmentsRow In myAllAdjustments

                    If myGroupID <> R.GroupID.Trim Then
                        myAxesValues = New List(Of Integer)
                        myAxesValues.Add(0) ' (-1)  ' XBC 23/11/2011
                        myAxesValues.Add(0) ' (-1)  ' XBC 23/11/2011
                        myAxesValues.Add(0) ' (-1)  ' XBC 23/11/2011
                    End If

                    myGroupID = R.GroupID.Trim

                    Select Case R.AxisID    '.ToUpper
                        Case "POLAR" : If R.InFile Then myAxesValues(0) = CInt(R.Value) Else myAxesValues(0) = 0 ' -1  ' XBC 23/11/2011
                        Case "Z" : If R.InFile Then myAxesValues(1) = CInt(R.Value) Else myAxesValues(1) = 0 ' -1  ' XBC 23/11/2011
                        Case "ROTOR" : If R.InFile Then myAxesValues(2) = CInt(R.Value) Else myAxesValues(2) = 0 ' -1  ' XBC 23/11/2011

                    End Select

                    'Arm's Position that is being adjusted/tested
                    Dim isNewAdjustment As Boolean = False

                    For Each N As SRVAdjustmentsDS.srv_tfmwAdjustmentsRow In myNewAdjustments

                        If R.GroupID.Trim = N.GroupID.Trim Then
                            isNewAdjustment = True
                            Exit For
                        End If

                    Next

                    If isNewAdjustment Then

                        ' XBC 23/11/2011
                        If Not .HisArmAxesValues.ContainsKey(pPosition) Then
                            .HisArmAxesValues(pPosition) = myAxesValues
                        End If

                        Select Case R.AxisID    '.ToUpper
                            Case "POLAR" : .HisArmAxesValues(pPosition).Item(0) = myAxesValues(0)
                            Case "Z" : .HisArmAxesValues(pPosition).Item(1) = myAxesValues(1)
                            Case "ROTOR" : .HisArmAxesValues(pPosition).Item(2) = myAxesValues(2)

                        End Select

                        If pTask = HISTORY_TASKS.ADJUSTMENT Then
                            .HisArmAdjResults(pPosition) = pResult
                        ElseIf pTask = HISTORY_TASKS.TEST Then
                            .HisArmTestResults(pPosition) = pResult
                        End If

                    Else

                        Dim myPosition As PositionsAdjustmentDelegate.HISTORY_ARM_POSITIONS = PositionsAdjustmentDelegate.HISTORY_ARM_POSITIONS.None

                        If R.GroupID.Trim.Contains("_DISP1") Then myPosition = PositionsAdjustmentDelegate.HISTORY_ARM_POSITIONS.Dispensation
                        If R.GroupID.Trim.Contains("_DISP2") Then myPosition = PositionsAdjustmentDelegate.HISTORY_ARM_POSITIONS.Predilution
                        If R.GroupID.Trim.Contains("_ZREF") Then myPosition = PositionsAdjustmentDelegate.HISTORY_ARM_POSITIONS.Z_Ref
                        If R.GroupID.Trim.Contains("_WASH") Then myPosition = PositionsAdjustmentDelegate.HISTORY_ARM_POSITIONS.Washing
                        If R.GroupID.Trim.Contains("_RING1") Then myPosition = PositionsAdjustmentDelegate.HISTORY_ARM_POSITIONS.Ring1
                        If R.GroupID.Trim.Contains("_RING2") Then myPosition = PositionsAdjustmentDelegate.HISTORY_ARM_POSITIONS.Ring2
                        If R.GroupID.Trim.Contains("_RING3") Then myPosition = PositionsAdjustmentDelegate.HISTORY_ARM_POSITIONS.Ring3
                        If R.GroupID.Trim.Contains("_ZTUBE1") Then myPosition = PositionsAdjustmentDelegate.HISTORY_ARM_POSITIONS.Z_Tube
                        If R.GroupID.Trim.Contains("_ISE") Then myPosition = PositionsAdjustmentDelegate.HISTORY_ARM_POSITIONS.ISE_Pos
                        If R.GroupID.Trim.Contains("_PARK") Then myPosition = PositionsAdjustmentDelegate.HISTORY_ARM_POSITIONS.Parking

                        If myPosition <> PositionsAdjustmentDelegate.HISTORY_ARM_POSITIONS.None Then
                            ' XBC 23/11/2011

                            If Not .HisArmAxesValues.ContainsKey(myPosition) Then
                                .HisArmAxesValues(myPosition) = myAxesValues
                            End If

                            Select Case R.AxisID    '.ToUpper
                                Case "POLAR" : .HisArmAxesValues(myPosition).Item(0) = myAxesValues(0)
                                Case "Z" : .HisArmAxesValues(myPosition).Item(1) = myAxesValues(1)
                                Case "ROTOR" : .HisArmAxesValues(myPosition).Item(2) = myAxesValues(2)

                            End Select
                            ' XBC 23/11/2011
                        End If

                    End If

                Next




            End With


        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".ReportHistoryArmPositions ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".ReportHistoryArmPositions ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Report Task not performed successfully to History
    ''' </summary>
    ''' <remarks>Created by SGM 02/08/2011</remarks>
    Private Sub ReportHistoryError()
        Try
            Select Case Me.CurrentMode

                Case ADJUSTMENT_MODES.ABSORBANCE_SCANNING, _
                ADJUSTMENT_MODES.ABSORBANCE_SCANNED, _
                ADJUSTMENT_MODES.ADJUSTING, _
                ADJUSTMENT_MODES.ADJUSTED

                    Me.ReportHistory(HISTORY_TASKS.ADJUSTMENT, PositionsAdjustmentDelegate.HISTORY_RESULTS._ERROR)

            End Select

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".ReportHistoryError ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".ReportHistoryError ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Clears all the History Data
    ''' </summary>
    ''' <remarks>Created by SGM 02/08/2011</remarks>
    Private Sub ClearHistoryData(ByVal pArea As PositionsAdjustmentDelegate.HISTORY_AREAS, Optional ByVal pIsLastArmAction As Boolean = False)
        Try
            With Me.myScreenDelegate

                Select Case pArea
                    Case PositionsAdjustmentDelegate.HISTORY_AREAS.OPT
                        .HisOpticCenteringAdjResult = PositionsAdjustmentDelegate.HISTORY_RESULTS.NOT_DONE
                        .HisOpticCenteringTestResult = PositionsAdjustmentDelegate.HISTORY_RESULTS.NOT_DONE
                        .HisOpticCenteringValue = -1
                        ' XBC 05/01/2012 - Add Encoder functionality
                        .HisEncoderValue = -1
                        ' XBC 05/01/2012 - Add Encoder functionality
                        .HisLEDIntensity = -1
                        .HisWaveLength = -1
                        .HisNumWells = -1
                        .HisStepsbyWell = -1

                    Case PositionsAdjustmentDelegate.HISTORY_AREAS.WS_POS
                        .HisWashingStationAdjResult = PositionsAdjustmentDelegate.HISTORY_RESULTS.NOT_DONE
                        .HisWashingStationTestResult = PositionsAdjustmentDelegate.HISTORY_RESULTS.NOT_DONE
                        .HisWashingStationValue = -1

                    Case PositionsAdjustmentDelegate.HISTORY_AREAS.MIX1_TEST, PositionsAdjustmentDelegate.HISTORY_AREAS.MIX2_TEST
                        .HisStirrerTestResult = PositionsAdjustmentDelegate.HISTORY_RESULTS.NOT_DONE

                    Case Else
                        .HisArmPosName = PositionsAdjustmentDelegate.HISTORY_ARM_POSITIONS.None

                        If pIsLastArmAction Then
                            'initialize all the positions adjustments results
                            .HisArmAdjResults = New Dictionary(Of PositionsAdjustmentDelegate.HISTORY_ARM_POSITIONS, PositionsAdjustmentDelegate.HISTORY_RESULTS)


                            'initialize all the positions tests results
                            .HisArmTestResults = New Dictionary(Of PositionsAdjustmentDelegate.HISTORY_ARM_POSITIONS, PositionsAdjustmentDelegate.HISTORY_RESULTS)

                            'initialize all the positions coordinates
                            Dim myAxesValues As New List(Of Integer)
                            myAxesValues.Add(0) ' (-1)  ' XBC 23/11/2011 'Polar
                            myAxesValues.Add(0) ' (-1)  ' XBC 23/11/2011 'Z
                            myAxesValues.Add(0) ' (-1)  ' XBC 23/11/2011 'Rotor

                            .HisArmAxesValues = New Dictionary(Of PositionsAdjustmentDelegate.HISTORY_ARM_POSITIONS, List(Of Integer))

                        End If
                End Select

                .HistoryArea = PositionsAdjustmentDelegate.HISTORY_AREAS.NONE
            End With

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".ReportHistory ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".ReportHistory ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

#End Region

#Region "Simulation Methods"

    ''' <summary>
    ''' Simulation of Absorbance Data
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>SGM 06/04/11</remarks>
    Private Function SimulateAbsorbanceData() As GlobalDataTO
        Dim myGlobal As New GlobalDataTO
        Try
            Dim K As Single = 1
            'Dim myAbs As String = ""
            Dim AbsorbanceData As New List(Of Single)

            Me.Cursor = Cursors.WaitCursor

            For a As Integer = 1 To 400 Step 1

                If a > 1 And a <= 14 Then
                    Select Case a
                        Case 1, 2 : AbsorbanceData.Add(CSng(0.8 * K))
                        Case 3, 4 : AbsorbanceData.Add(CSng(7.9 * K))
                        Case 5, 6 : AbsorbanceData.Add(CSng(8.6 * K))
                        Case 7, 8 : AbsorbanceData.Add(CSng(9.0 * K))
                        Case 9, 10 : AbsorbanceData.Add(CSng(8.6 * K))
                        Case 11, 12 : AbsorbanceData.Add(CSng(7.9 * K))
                        Case 13, 14 : AbsorbanceData.Add(CSng(6.8 * K))
                    End Select
                ElseIf a > 14 And a <= 78 Then
                    AbsorbanceData.Add(6 * K)
                ElseIf a > 78 And a <= 92 Then
                    Select Case a
                        Case 78, 79 : AbsorbanceData.Add(CSng(6.8 * K))
                        Case 80, 81 : AbsorbanceData.Add(CSng(7.9 * K))
                        Case 82, 83 : AbsorbanceData.Add(CSng(8.6 * K))
                        Case 84, 85 : AbsorbanceData.Add(CSng(9 * K))
                        Case 86, 87 : AbsorbanceData.Add(CSng(8.8 * K))
                        Case 88, 89 : AbsorbanceData.Add(CSng(8.5 * K))
                        Case 90, 91 : AbsorbanceData.Add(CSng(7.5 * K))
                    End Select

                ElseIf a > 92 And a <= 158 Then
                    AbsorbanceData.Add(7 * K)

                ElseIf a > 158 And a <= 172 Then
                    Select Case a
                        Case 159, 160 : AbsorbanceData.Add(CSng(7.5 * K))
                        Case 161, 162 : AbsorbanceData.Add(CSng(8.5 * K))
                        Case 163, 164 : AbsorbanceData.Add(CSng(8.8 * K))
                        Case 165, 166 : AbsorbanceData.Add(CSng(9 * K))
                        Case 167, 168 : AbsorbanceData.Add(CSng(8.8 * K))
                        Case 169, 170 : AbsorbanceData.Add(CSng(8.5 * K))
                        Case 171, 172 : AbsorbanceData.Add(CSng(7.5 * K))
                    End Select

                ElseIf a > 172 And a <= 238 Then
                    AbsorbanceData.Add(7 * K)

                ElseIf a > 238 And a <= 252 Then
                    Select Case a
                        Case 239, 240 : AbsorbanceData.Add(CSng(7.5 * K))
                        Case 241, 242 : AbsorbanceData.Add(CSng(8.5 * K))
                        Case 243, 244 : AbsorbanceData.Add(CSng(8.8 * K))
                        Case 245, 246 : AbsorbanceData.Add(CSng(9 * K))
                        Case 247, 248 : AbsorbanceData.Add(CSng(8.8 * K))
                        Case 249, 250 : AbsorbanceData.Add(CSng(8.5 * K))
                        Case 251, 252 : AbsorbanceData.Add(CSng(7.5 * K))
                    End Select

                ElseIf a > 252 And a <= 318 Then
                    AbsorbanceData.Add(7 * K)

                ElseIf a > 318 And a <= 332 Then
                    Select Case a
                        Case 319, 320 : AbsorbanceData.Add(CSng(7.5 * K))
                        Case 321, 322 : AbsorbanceData.Add(CSng(8.5 * K))
                        Case 323, 324 : AbsorbanceData.Add(CSng(8.8 * K))
                        Case 325, 326 : AbsorbanceData.Add(CSng(9 * K))
                        Case 327, 328 : AbsorbanceData.Add(CSng(8.6 * K))
                        Case 329, 330 : AbsorbanceData.Add(CSng(7.9 * K))
                        Case 331, 332 : AbsorbanceData.Add(CSng(6.8 * K))
                    End Select

                ElseIf a > 332 And a <= 398 Then
                    AbsorbanceData.Add(6 * K)

                ElseIf a > 398 And a <= 400 Then
                    Select Case a
                        Case 400 : AbsorbanceData.Add(CSng(6.8 * K))
                    End Select
                End If

            Next

            Me.Cursor = Cursors.Default

            If AbsorbanceData.Count > 0 Then
                myGlobal.SetDatos = AbsorbanceData
            End If

        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = Messages.SYSTEM_ERROR.ToString
            myGlobal.ErrorMessage = ex.Message
            GlobalBase.CreateLogActivity(ex.Message, Name & ".SimulateAbsorbanceData", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Name & ".SimulateAbsorbanceData", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try

        Return myGlobal

    End Function

    Private Sub SimulateRELPositioning()
        Try

            ' simulating
            MyBase.DisplayMessage(Messages.SRV_STEP_POSITIONING.ToString)

            MyBase.CurrentMode = ADJUSTMENT_MODES.ADJUSTING
            PrepareArea()

            Me.Cursor = Cursors.WaitCursor
            Thread.Sleep(SimulationProcessTime)
            MyBase.myServiceMDI.Focus()
            Me.Cursor = Cursors.Default

            MyBase.CurrentMode = ADJUSTMENT_MODES.ADJUSTED
            MyBase.DisplayMessage(Messages.SRV_COMPLETED.ToString)
            PrepareArea()


        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Name & ".SimulateStepPositioning", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Name & ".SimulateStepPositioning", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub SimulateABSPositioning()
        Try

            ' simulating
            MyBase.DisplayMessage(Messages.SRV_ABS_REQUESTED.ToString)

            MyBase.CurrentMode = ADJUSTMENT_MODES.ADJUSTING
            PrepareArea()

            Me.Cursor = Cursors.WaitCursor
            Thread.Sleep(SimulationProcessTime)
            MyBase.myServiceMDI.Focus()
            Me.Cursor = Cursors.Default

            MyBase.CurrentMode = ADJUSTMENT_MODES.ADJUSTED
            MyBase.DisplayMessage(Messages.SRV_COMPLETED.ToString)
            PrepareArea()


        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Name & ".SimulateAbsPositioning", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Name & ".SimulateAbsPositioning", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub SimulateHOMEPositioning()
        Try

            ' simulating
            MyBase.DisplayMessage(Messages.SRV_HOMES_IN_PROGRESS.ToString)

            MyBase.CurrentMode = ADJUSTMENT_MODES.ADJUSTING
            PrepareArea()

            Me.Cursor = Cursors.WaitCursor
            Thread.Sleep(SimulationProcessTime)
            MyBase.myServiceMDI.Focus()
            Me.Cursor = Cursors.Default

            MyBase.CurrentMode = ADJUSTMENT_MODES.ADJUSTED
            MyBase.DisplayMessage(Messages.SRV_COMPLETED.ToString)
            PrepareArea()


        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Name & ".SimulateHOMEPositioning", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Name & ".SimulateHOMEPositioning", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Function SimulateEncoderData() As GlobalDataTO
        Dim myGlobal As New GlobalDataTO
        Try
            'Dim K As Single = 1
            'Dim myAbs As String = ""
            Dim EncoderData As New List(Of Single)

            For a As Integer = 1 To 400 Step 1

                If a > 0 And a <= 78 Then
                    EncoderData.Add(0)
                ElseIf a > 78 And a <= 92 Then
                    EncoderData.Add(1)
                ElseIf a > 92 And a <= 158 Then
                    EncoderData.Add(0)
                ElseIf a > 158 And a <= 172 Then
                    EncoderData.Add(1)
                ElseIf a > 172 And a <= 238 Then
                    EncoderData.Add(0)
                ElseIf a > 238 And a <= 252 Then
                    EncoderData.Add(1)
                ElseIf a > 252 And a <= 318 Then
                    EncoderData.Add(0)
                ElseIf a > 318 And a <= 332 Then
                    EncoderData.Add(1)
                ElseIf a > 332 And a <= 392 Then
                    EncoderData.Add(0)
                ElseIf a > 392 And a <= 400 Then
                    EncoderData.Add(1)
                End If

            Next


            If EncoderData.Count > 0 Then
                myGlobal.SetDatos = EncoderData
            End If

        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = Messages.SYSTEM_ERROR.ToString
            myGlobal.ErrorMessage = ex.Message
            GlobalBase.CreateLogActivity(ex.Message, Name & ".SimulateEncoderData", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Name & ".SimulateEncoderData", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
        Return myGlobal
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
    Private Sub IPositionsAdjustments_FormClosing(ByVal sender As Object, ByVal e As FormClosingEventArgs) Handles Me.FormClosing

        Try

            If e.CloseReason = CloseReason.MdiFormClosing Then
                e.Cancel = True
            Else

                ' XBC 26/09/2011
                MyBase.myServiceMDI.SEND_INFO_START()

                Me.ReportHistory(Nothing, Nothing, True)
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

    Private Sub PositionsAdjustments_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        Dim myGlobal As New GlobalDataTO
        Try

            GetUserNumericalLevel()

            'Get the current Language from the current Application Session
            currentLanguage = GetSessionInfo().ApplicationLanguage.Trim.ToString

            'Load the multilanguage texts for all Screen Labels and get Icons for graphical Buttons
            GetScreenLabels(currentLanguage)

            HideTabsForBa200Model()

            'Screen delegate SGM 20/01/2012
            myScreenDelegate = New PositionsAdjustmentDelegate(myServiceMDI.ActiveAnalyzer, myFwScriptDelegate)

            WizardMode = False 'True

            SelectedPage = ADJUSTMENT_PAGES.NONE
            DefineScreenLayout(ADJUSTMENT_PAGES.OPTIC_CENTERING)

            'Initialize homes SGM 20/09/2011
            InitializeHomes()

            ' Configure Grids style
            BsGridSample.BorderStyle = BorderStyle.None
            BsGridReagent1.BorderStyle = BorderStyle.None
            BsGridReagent2.BorderStyle = BorderStyle.None
            BsGridMixer1.BorderStyle = BorderStyle.None
            BsGridMixer2.BorderStyle = BorderStyle.None

            PrepareButtons()

            DisableAll()

            DisplayInformation(APPLICATION_PAGES.POS_PHOTOMETRY, BsInfoOptXPSViewer)
            DisplayInformation(APPLICATION_PAGES.POS_WASHING_STATION, BsInfoWsXPSViewer)
            DisplayInformation(APPLICATION_PAGES.POS_ARMS, BsInfoArmsXPSViewer)

            If Not myGlobal.HasError Then


            Else
                PrepareErrorMode()
                ShowMessage(Name & ".Load", myGlobal.ErrorCode, myGlobal.ErrorMessage, Me)
            End If

            ResetBorderSRV()

            ' XBC 30/11/2011
            myScreenDelegate.IsWashingStationUp = False

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Name & ".Load ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".Load ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    'SGM 08/03/11
    Private Sub PositionsAdjustments_Shown(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Shown
        Dim myGlobal As New GlobalDataTO
        Try
            ' Check communications with Instrument
            If Not AnalyzerController.Instance.Analyzer.Connected Then '#REFACTORING
                myGlobal.ErrorCode = "ERROR_COMM"
                myGlobal.HasError = True
                ' Prepare Error Mode Controls in GUI
                Me.CurrentMode = ADJUSTMENT_MODES.ERROR_MODE
                Me.PrepareArea()
            Else
                ' Reading Adjustments
                If Not Me.myServiceMDI.AdjustmentsReaded Then
                    ' Parent Reading Adjustments
                    MyBase.ReadAdjustments()
                    PrepareArea()

                    ' Manage FwScripts must to be sent at load screen
                    If MyBase.SimulationMode Then

                        MyBase.myServiceMDI.Focus()

                        Me.CurrentMode = ADJUSTMENT_MODES.ADJUSTMENTS_READING
                        PrepareArea()
                        MyBase.DisplayMessage(Messages.SRV_READ_ADJUSTMENTS.ToString)

                        Me.Cursor = Cursors.WaitCursor
                        Thread.Sleep(SimulationProcessTime)
                        MyBase.myServiceMDI.Focus()
                        Me.Cursor = Cursors.Default

                        Me.CurrentMode = ADJUSTMENT_MODES.ADJUSTMENTS_READED

                        MyBase.DisplayMessage(Messages.SRV_ADJUSTMENTS_READED.ToString)

                        PrepareArea()

                    Else
                        If Not myGlobal.HasError AndAlso AnalyzerController.Instance.Analyzer.Connected Then '#REFACTORING
                            myGlobal = myScreenDelegate.SendREAD_ADJUSTMENTS(Ax00Adjustsments.ALL)
                        End If
                    End If

                Else
                    Me.CurrentMode = ADJUSTMENT_MODES.ADJUSTMENTS_READED
                    Me.PrepareArea()
                End If
            End If

            Me.BsInfoOptXPSViewer.Visible = True
            Me.BsInfoWsXPSViewer.Visible = True
            Me.BsInfoArmsXPSViewer.Visible = True

            Me.BsInfoOptXPSViewer.RefreshPage()
            Me.BsInfoWsXPSViewer.RefreshPage()
            Me.BsInfoArmsXPSViewer.RefreshPage()

            If myGlobal.HasError Then
                PrepareErrorMode()
                ' XBC 25/10/2011 - message is shown in method ManageReceptionEvent
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".Shown ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".Shown ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub BsTabArmsControl_Selecting(ByVal sender As Object, ByVal e As TabControlCancelEventArgs) Handles BsTabArmsControl.Selecting
        Try

            'check if changes before changing arm tab
            If MyClass.IsLocallySaved Then
                Dim dialogResultToReturn As DialogResult = MyBase.ShowMessage("", Messages.SRV_DISCARD_CHANGES.ToString)

                If dialogResultToReturn = DialogResult.No Then

                    Me.ManageTabArms = False
                Else

                    Me.BsGridSample.HideAllIconsOk()
                    Me.BsGridReagent1.HideAllIconsOk()
                    Me.BsGridReagent2.HideAllIconsOk()
                    Me.BsGridMixer1.HideAllIconsOk()
                    Me.BsGridMixer2.HideAllIconsOk()

                    MyClass.IsLocallySaved = False
                    Me.ChangedValue = False
                    MyBase.DisplayMessage(Messages.SRV_ADJUSTMENTS_CANCELLED.ToString)
                    Me.ManageTabArms = True
                End If

            End If

            If Not Me.ManageTabArms Then
                e.Cancel = True
                Exit Sub
            End If



            If BsTabArmsControl.SelectedTab Is TabSample Then
                Me.SelectedArmTab = ADJUSTMENT_ARMS.SAMPLE
            ElseIf BsTabArmsControl.SelectedTab Is TabReagent1 Then
                Me.SelectedArmTab = ADJUSTMENT_ARMS.REAGENT1
            ElseIf BsTabArmsControl.SelectedTab Is TabReagent2 Then
                Me.SelectedArmTab = ADJUSTMENT_ARMS.REAGENT2
            ElseIf BsTabArmsControl.SelectedTab Is TabMixer1 Then
                Me.SelectedArmTab = ADJUSTMENT_ARMS.MIXER1
            ElseIf BsTabArmsControl.SelectedTab Is TabMixer2 Then
                Me.SelectedArmTab = ADJUSTMENT_ARMS.MIXER2
            End If

            Me.myScreenLayout.AdjustmentPanel.AdjustPanel.AdjustAreas = MyBase.GetAdjustAreas(Me.BsArmsAdjustPanel)
            MyBase.SetAdjustmentItems(Me.BsArmsAdjustPanel)

            Me.ReportHistory(Nothing, Nothing, True)


            Me.SelectedAdjustmentGroup = Me.SetPositionAdjustmentType(Me.SelectedArmTab, Me.SelectedRow)
            MyBase.CurrentMode = ADJUSTMENT_MODES.LOADED
            PrepareArea()

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".BsTabArmsControl_Selecting ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".BsTabArmsControl_Selecting ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>
    ''' Creation ?
    ''' AG 01/10/2014 - BA-1953 - also reset REACTIONS_HOME_ROTOR because it is the script used during this adjustment
    ''' </remarks>
    Private Sub BsTabPagesControl_Deselecting(ByVal sender As Object, ByVal e As TabControlCancelEventArgs) Handles BsTabPagesControl.Deselecting

        Dim myGlobal As New GlobalDataTO

        Try
            ' SGM 28/11/2012 - Always reset preliminary GLF home before quit Optic Centering
            If e.TabPage Is TabOpticCentering Then

                'AG 01/10/2014 - BA-1953 - also reset REACTIONS_HOME_ROTOR because it is the script used during this adjustment
                Dim preliminaryHomesToResetList As New List(Of String)
                preliminaryHomesToResetList.Add(FwSCRIPTS_IDS.REACTIONS_ROTOR_HOME_WELL1.ToString)
                preliminaryHomesToResetList.Add(FwSCRIPTS_IDS.REACTIONS_HOME_ROTOR.ToString)
                myGlobal = myScreenDelegate.ResetSpecifiedPreliminaryHomes(MyBase.myServiceMDI.ActiveAnalyzer, preliminaryHomesToResetList)

                If myGlobal.HasError Then
                    PrepareErrorMode()
                    Exit Try
                End If
            End If
            ' SGM 28/11/2012
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".BsTabPagesControl_Deselecting ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".BsTabPagesControl_Deselecting ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub BsTabPagesControl_Selected(ByVal sender As Object, ByVal e As TabControlEventArgs) Handles BsTabPagesControl.Selected
        Try

            Application.DoEvents()


            If e.TabPage Is TabOpticCentering Then
                Me.BsInfoOptXPSViewer.RefreshPage()

            ElseIf e.TabPage Is TabWashingStation Then
                Me.BsInfoWsXPSViewer.RefreshPage()

            ElseIf e.TabPage Is TabArmsPositions Then
                Me.BsInfoArmsXPSViewer.RefreshPage()

            End If

            'SGM 27/02/2012 clear temporal DS
            MyClass.LocalSavedAdjustmentsDS.Clear()

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".BsTabArmsControl_Selected ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".BsTabArmsControl_Selected ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub BsTabPagesControl_Selecting(ByVal sender As Object, ByVal e As TabControlCancelEventArgs) Handles BsTabPagesControl.Selecting
        Dim dialogResultToReturn As DialogResult
        Try

            If BsTabPagesControl.SelectedTab IsNot TabArmsPositions Then
                'check if changes before changing  tab
                If MyClass.IsLocallySaved Then
                    dialogResultToReturn = MyBase.ShowMessage("", Messages.SRV_DISCARD_CHANGES.ToString)

                    If dialogResultToReturn = DialogResult.No Then
                        Me.ManageTabPages = False
                    Else

                        Me.BsGridSample.HideAllIconsOk()
                        Me.BsGridReagent1.HideAllIconsOk()
                        Me.BsGridReagent2.HideAllIconsOk()
                        Me.BsGridMixer1.HideAllIconsOk()
                        Me.BsGridMixer2.HideAllIconsOk()

                        MyClass.IsLocallySaved = False
                        Me.ChangedValue = False
                        MyBase.DisplayMessage(Messages.SRV_ADJUSTMENTS_CANCELLED.ToString)
                        Me.ManageTabPages = True
                    End If

                End If
            End If


            If Not Me.ManageTabPages Then
                e.Cancel = True
                Exit Sub
            End If

            If BsTabPagesControl.SelectedTab Is TabOpticCentering Then

                dialogResultToReturn = MyBase.ShowMessage(GetMessageText(Messages.SRV_ADJUSTMENTS_TESTS.ToString), Messages.SRV_OPTIC_ADJUSTMENT_ENTER.ToString)

                'history of arms pending history adjustments and tests
                Me.ReportHistory(Nothing, Nothing, True)

                Me.SelectedPage = ADJUSTMENT_PAGES.OPTIC_CENTERING
                Me.SelectedAdjustmentGroup = ADJUSTMENT_GROUPS.PHOTOMETRY
                Me.BsOpticAdjustButton.Visible = True

            ElseIf BsTabPagesControl.SelectedTab Is TabWashingStation Then

                'history of arms pending history adjustments and tests
                Me.ReportHistory(Nothing, Nothing, True)

                Me.SelectedPage = ADJUSTMENT_PAGES.WASHING_STATION
                Me.SelectedAdjustmentGroup = ADJUSTMENT_GROUPS.WASHING_STATION
                Me.BsWSAdjustButton.Visible = True

                Me.ReportHistory(Nothing, Nothing, True)


            ElseIf BsTabPagesControl.SelectedTab Is TabArmsPositions Then
                Me.SelectedPage = ADJUSTMENT_PAGES.ARM_POSITIONS
                ' Adjustments in this section are done through each tab list of buttons so the generical button is no need
                Me.SelectedAdjustmentGroup = Me.SetPositionAdjustmentType(Me.SelectedArmTab, Me.SelectedRow)

            End If

            MyBase.DisplayMessage("")


            PrepareLoadedMode()


        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".BsTabControl1_Selecting ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".BsTabControl1_Selecting ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub BsCloseButton_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BsExitButton.Click
        Try
            Me.ExitScreen()

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".BsExitButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".BsExitButton_Click ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' When the  ESC key is pressed, the screen is closed 
    ''' </summary>
    ''' <remarks>
    ''' Created by XBC 07/06/2011
    ''' </remarks>
    Private Sub ConfigUsers_KeyDown(ByVal sender As Object, ByVal e As KeyEventArgs) Handles Me.KeyDown
        Try
            If (e.KeyCode = Keys.Escape) Then

                'RH 04/07/2011 Escape key should do exactly the same operations as bsExitButton_Click()
                BsExitButton.PerformClick()
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".KeyDown ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".KeyDown", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    'SG 24/01/11
    Private Sub BsSaveButton_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BsSaveButton.Click
        Try
            MyClass.IsLocallySavedAttr = False
            Me.SaveAdjustment(True)

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".BsSaveButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".BsSaveButton_Click ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub



    Private Sub SelectedValueEvent(ByVal pRowIndex As Integer, ByVal pColIndex As Integer) Handles BsGridSample.SelectedValueEvent, _
                                                                                                   BsGridReagent1.SelectedValueEvent, _
                                                                                                   BsGridReagent2.SelectedValueEvent, _
                                                                                                   BsGridMixer1.SelectedValueEvent, _
                                                                                                   BsGridMixer2.SelectedValueEvent
        Try


            ' EditedValue Structure managing
            With EditedValue
                If .canMovePolarValue Then
                    Me.BsAdjustPolar.CurrentValue = EditedValue.CurrentPolarValue
                    If Me.BsAdjustPolar.Visible Then
                        Me.BsAdjustPolar.Enabled = True
                    End If
                End If
                If .canMoveZValue Then
                    Me.BsAdjustZ.CurrentValue = EditedValue.CurrentZValue
                    If Me.BsAdjustZ.Visible Then
                        Me.BsAdjustZ.Enabled = True
                    End If
                End If
                If .canMoveRotorValue Then
                    Me.BsAdjustRotor.CurrentValue = EditedValue.CurrentRotorValue
                    If Me.BsAdjustRotor.Visible Then
                        Me.BsAdjustRotor.Enabled = True
                    End If
                End If

                Select Case pColIndex - 2
                    Case 1
                        .AxisID = GlobalEnumerates.AXIS.POLAR
                        If Not Me.BsAdjustPolar.IsFocused Then
                            FocusedFromGrid = True
                            Me.BsAdjustPolar.Focus()
                        End If
                        Me.BsAdjustPolar.EditionMode = False

                    Case 2
                        .AxisID = GlobalEnumerates.AXIS.Z
                        If Not Me.BsAdjustZ.IsFocused Then
                            FocusedFromGrid = True
                            Me.BsAdjustZ.Focus()
                        End If
                        Me.BsAdjustZ.EditionMode = False

                    Case 3
                        .AxisID = GlobalEnumerates.AXIS.ROTOR
                        If Not Me.BsAdjustRotor.IsFocused Then
                            FocusedFromGrid = True
                            Me.BsAdjustRotor.Focus()
                        End If
                        Me.BsAdjustRotor.EditionMode = False

                End Select

            End With

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".SelectedValueEvent ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".SelectedValueEvent ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Function SetPositionAdjustmentType(ByVal pArm As ADJUSTMENT_ARMS, ByVal pRowIndex As Integer) As ADJUSTMENT_GROUPS

        Dim myAdjustment As ADJUSTMENT_GROUPS

        Try

            Select Case pArm
                Case ADJUSTMENT_ARMS.SAMPLE
                    Select Case Me.BsGridSample.IdentValue(pRowIndex).ToString
                        Case "DISP1"
                            myAdjustment = ADJUSTMENT_GROUPS.SAMPLES_ARM_DISP1

                        Case "DISP2"
                            myAdjustment = ADJUSTMENT_GROUPS.SAMPLES_ARM_DISP2

                        Case "Z_REF"
                            myAdjustment = ADJUSTMENT_GROUPS.SAMPLES_ARM_ZREF

                        Case "WASH"
                            myAdjustment = ADJUSTMENT_GROUPS.SAMPLES_ARM_WASH

                        Case "RING1"
                            myAdjustment = ADJUSTMENT_GROUPS.SAMPLES_ARM_RING1

                        Case "RING2"
                            myAdjustment = ADJUSTMENT_GROUPS.SAMPLES_ARM_RING2

                        Case "REAGENTZ"
                            myAdjustment = ADJUSTMENT_GROUPS.REAGENT1_ARM_RING1

                        Case "RING3"
                            myAdjustment = ADJUSTMENT_GROUPS.SAMPLES_ARM_RING3

                        Case "Z_TUBE"
                            myAdjustment = ADJUSTMENT_GROUPS.SAMPLES_ARM_ZTUBE1

                        Case "ISE"
                            myAdjustment = ADJUSTMENT_GROUPS.SAMPLES_ARM_ISE

                        Case "PKG"
                            myAdjustment = ADJUSTMENT_GROUPS.SAMPLES_ARM_PARK

                    End Select
                Case ADJUSTMENT_ARMS.REAGENT1
                    Select Case Me.BsGridReagent1.IdentValue(pRowIndex).ToString
                        Case "DISP1"
                            myAdjustment = ADJUSTMENT_GROUPS.REAGENT1_ARM_DISP1

                        Case "Z_REF"
                            myAdjustment = ADJUSTMENT_GROUPS.REAGENT1_ARM_ZREF

                        Case "WASH"
                            myAdjustment = ADJUSTMENT_GROUPS.REAGENT1_ARM_WASH

                        Case "RING1"
                            myAdjustment = ADJUSTMENT_GROUPS.REAGENT1_ARM_RING1

                        Case "RING2"
                            myAdjustment = ADJUSTMENT_GROUPS.REAGENT1_ARM_RING2

                        Case "PKG"
                            myAdjustment = ADJUSTMENT_GROUPS.REAGENT1_ARM_PARK

                    End Select
                Case ADJUSTMENT_ARMS.REAGENT2
                    Select Case Me.BsGridReagent2.IdentValue(pRowIndex).ToString
                        Case "DISP1"
                            myAdjustment = ADJUSTMENT_GROUPS.REAGENT2_ARM_DISP1

                        Case "Z_REF"
                            myAdjustment = ADJUSTMENT_GROUPS.REAGENT2_ARM_ZREF

                        Case "WASH"
                            myAdjustment = ADJUSTMENT_GROUPS.REAGENT2_ARM_WASH

                        Case "RING1"
                            myAdjustment = ADJUSTMENT_GROUPS.REAGENT2_ARM_RING1

                        Case "RING2"
                            myAdjustment = ADJUSTMENT_GROUPS.REAGENT2_ARM_RING2

                        Case "PKG"
                            myAdjustment = ADJUSTMENT_GROUPS.REAGENT2_ARM_PARK

                    End Select
                Case ADJUSTMENT_ARMS.MIXER1
                    Select Case Me.BsGridMixer1.IdentValue(pRowIndex).ToString
                        Case "DISP1"
                            myAdjustment = ADJUSTMENT_GROUPS.MIXER1_ARM_DISP1

                        Case "Z_REF"
                            myAdjustment = ADJUSTMENT_GROUPS.MIXER1_ARM_ZREF

                        Case "WASH"
                            myAdjustment = ADJUSTMENT_GROUPS.MIXER1_ARM_WASH

                        Case "PKG"
                            myAdjustment = ADJUSTMENT_GROUPS.MIXER1_ARM_PARK

                    End Select
                Case ADJUSTMENT_ARMS.MIXER2
                    Select Case Me.BsGridMixer2.IdentValue(pRowIndex).ToString
                        Case "DISP1"
                            myAdjustment = ADJUSTMENT_GROUPS.MIXER2_ARM_DISP1

                        Case "Z_REF"
                            myAdjustment = ADJUSTMENT_GROUPS.MIXER2_ARM_ZREF

                        Case "WASH"
                            myAdjustment = ADJUSTMENT_GROUPS.MIXER2_ARM_WASH

                        Case "PKG"
                            myAdjustment = ADJUSTMENT_GROUPS.MIXER2_ARM_PARK

                    End Select
            End Select
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".SetPositionAdjustmentType ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".SetPositionAdjustmentType ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try

        Return myAdjustment

    End Function

    Private Sub BsAdjustButtons_Click(ByVal adjButton As DataGridViewDisableButtonCell, ByVal pRowIndex As Integer) Handles BsGridSample.Button1Click, _
                                                                          BsGridReagent1.Button1Click, _
                                                                          BsGridReagent2.Button1Click, _
                                                                          BsGridMixer1.Button1Click, _
                                                                          BsGridMixer2.Button1Click
        Dim myGlobal As New GlobalDataTO
        Try
            myGlobal = MyBase.PrepareAdjust()
            If myGlobal.HasError Then
                PrepareErrorMode()
            Else
                Me.SelectedRow = pRowIndex
                PrepareArea()
                ' Instantiate EditedValue Structure
                InitializeEditedValue()
                With EditedValue

                    Dim myPolar As New AdjustmentRowData
                    Dim myZ As New AdjustmentRowData
                    Dim myRotor As New AdjustmentRowData

                    Select Case SelectedArmTab
                        Case ADJUSTMENT_ARMS.SAMPLE
                            .AdjustmentID = SetPositionAdjustmentType(ADJUSTMENT_ARMS.SAMPLE, pRowIndex)
                            If AnalyzerController.Instance.IsBA200 AndAlso .AdjustmentID = ADJUSTMENT_GROUPS.REAGENT1_ARM_RING1 Then
                                myPolar = ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.SAMPLES_ARM_RING1.ToString, GlobalEnumerates.AXIS.POLAR, True)
                                myRotor = ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.SAMPLES_ARM_RING1.ToString, GlobalEnumerates.AXIS.ROTOR, True)
                            Else
                                myPolar = ReadGlobalAdjustmentData(.AdjustmentID.ToString, GlobalEnumerates.AXIS.POLAR, True)
                                myRotor = ReadGlobalAdjustmentData(.AdjustmentID.ToString, GlobalEnumerates.AXIS.ROTOR, True)
                            End If

                            myZ = ReadGlobalAdjustmentData(.AdjustmentID.ToString, GlobalEnumerates.AXIS.Z, True)


                            'SGM 02/03/2012 take the Polar and Rotor from Ring1
                            If .AdjustmentID = ADJUSTMENT_GROUPS.SAMPLES_ARM_ZTUBE1 Then
                                myPolar = ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.SAMPLES_ARM_RING1.ToString, GlobalEnumerates.AXIS.POLAR, True)
                                myRotor = ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.SAMPLES_ARM_RING1.ToString, GlobalEnumerates.AXIS.ROTOR, True)
                                'PENDING TO DEFINE ROTOR POS NUMBER
                            End If

                            Select Case Me.BsGridSample.IdentValue(pRowIndex).ToString
                                Case "DISP1"
                                    If (myPolar.CanSave And myPolar.Value.Length = 0) Or (myZ.CanSave And myZ.Value.Length = 0) Then
                                        myGlobal.HasError = True
                                    Else
                                        myScreenDelegate.pArmABSMovPolar = myPolar.Value
                                        myScreenDelegate.pArmABSMovZ = myZ.Value
                                    End If

                                Case "DISP2"
                                    If (myPolar.CanSave And myPolar.Value.Length = 0) Or (myZ.CanSave And myZ.Value.Length = 0) Then
                                        myGlobal.HasError = True
                                    Else
                                        myScreenDelegate.pArmABSMovPolar = myPolar.Value
                                        myScreenDelegate.pArmABSMovZ = myZ.Value
                                    End If

                                Case "Z_TUBE", "REAGENTZ"
                                    If (myPolar.CanSave And myPolar.Value.Length = 0) Or (myZ.CanSave And myZ.Value.Length = 0) Then
                                        myGlobal.HasError = True
                                    Else
                                        myScreenDelegate.pArmABSMovPolar = myPolar.Value
                                        myScreenDelegate.pArmABSMovZ = myZ.Value
                                        myScreenDelegate.pValueRotorABSMov = myRotor.Value
                                    End If

                                Case "WASH", "ISE", "PKG"
                                    If (myPolar.CanSave And myPolar.Value.Length = 0) Or (myZ.CanSave And myZ.Value.Length = 0) Then
                                        myGlobal.HasError = True
                                    Else
                                        myScreenDelegate.pArmABSMovPolar = myPolar.Value
                                        myScreenDelegate.pArmABSMovZ = myZ.Value
                                    End If

                                Case "Z_REF"
                                    If (myPolar.CanSave And myPolar.Value.Length = 0) Or (myZ.CanSave And myZ.Value.Length = 0) Then
                                        myGlobal.HasError = True
                                    Else
                                        myScreenDelegate.pArmABSMovPolar = myScreenDelegate.SampleArmPolarZREF.ToString
                                        myScreenDelegate.pArmABSMovZ = myZ.Value
                                    End If

                                Case "RING1", "RING2", "RING3"
                                    If (myPolar.CanSave And myPolar.Value.Length = 0) Or (myZ.CanSave And myZ.Value.Length = 0) Or (myRotor.CanSave And myRotor.Value.Length = 0) Then
                                        myGlobal.HasError = True
                                    Else
                                        myScreenDelegate.pArmABSMovPolar = myPolar.Value
                                        myScreenDelegate.pArmABSMovZ = myZ.Value
                                        myScreenDelegate.pValueRotorABSMov = myRotor.Value
                                    End If

                            End Select

                        Case ADJUSTMENT_ARMS.REAGENT1
                            .AdjustmentID = SetPositionAdjustmentType(ADJUSTMENT_ARMS.REAGENT1, pRowIndex)
                            myPolar = ReadGlobalAdjustmentData(.AdjustmentID.ToString, GlobalEnumerates.AXIS.POLAR, True)
                            myZ = ReadGlobalAdjustmentData(.AdjustmentID.ToString, GlobalEnumerates.AXIS.Z, True)
                            myRotor = ReadGlobalAdjustmentData(.AdjustmentID.ToString, GlobalEnumerates.AXIS.ROTOR, True)

                            Select Case Me.BsGridReagent1.IdentValue(pRowIndex).ToString
                                Case "DISP1"
                                    If (myPolar.CanSave And myPolar.Value.Length = 0) Or (myZ.CanSave And myZ.Value.Length = 0) Then
                                        myGlobal.HasError = True
                                    Else
                                        myScreenDelegate.pArmABSMovPolar = myPolar.Value
                                        myScreenDelegate.pArmABSMovZ = myZ.Value
                                    End If

                                Case "WASH", "PKG"
                                    If (myPolar.CanSave And myPolar.Value.Length = 0) Or (myZ.CanSave And myZ.Value.Length = 0) Then
                                        myGlobal.HasError = True
                                    Else
                                        myScreenDelegate.pArmABSMovPolar = myPolar.Value
                                        myScreenDelegate.pArmABSMovZ = myZ.Value
                                    End If

                                Case "Z_REF"
                                    If (myPolar.CanSave And myPolar.Value.Length = 0) Or (myZ.CanSave And myZ.Value.Length = 0) Then
                                        myGlobal.HasError = True
                                    Else
                                        myScreenDelegate.pArmABSMovPolar = myScreenDelegate.Reagent1ArmPolarZREF.ToString
                                        myScreenDelegate.pArmABSMovZ = myZ.Value
                                    End If

                                Case "RING1", "RING2"
                                    If (myPolar.CanSave And myPolar.Value.Length = 0) Or (myZ.CanSave And myZ.Value.Length = 0) Or (myRotor.CanSave And myRotor.Value.Length = 0) Then
                                        myGlobal.HasError = True
                                    Else
                                        myScreenDelegate.pArmABSMovPolar = myPolar.Value
                                        myScreenDelegate.pArmABSMovZ = myZ.Value
                                        myScreenDelegate.pValueRotorABSMov = myRotor.Value
                                    End If

                            End Select
                        Case ADJUSTMENT_ARMS.REAGENT2
                            .AdjustmentID = SetPositionAdjustmentType(ADJUSTMENT_ARMS.REAGENT2, pRowIndex)
                            myPolar = ReadGlobalAdjustmentData(.AdjustmentID.ToString, GlobalEnumerates.AXIS.POLAR, True)
                            myZ = ReadGlobalAdjustmentData(.AdjustmentID.ToString, GlobalEnumerates.AXIS.Z, True)
                            myRotor = ReadGlobalAdjustmentData(.AdjustmentID.ToString, GlobalEnumerates.AXIS.ROTOR, True)

                            Select Case Me.BsGridReagent2.IdentValue(pRowIndex).ToString
                                Case "DISP1"
                                    If (myPolar.CanSave And myPolar.Value.Length = 0) Or (myZ.CanSave And myZ.Value.Length = 0) Then
                                        myGlobal.HasError = True
                                    Else
                                        myScreenDelegate.pArmABSMovPolar = myPolar.Value
                                        myScreenDelegate.pArmABSMovZ = myZ.Value
                                    End If

                                Case "WASH", "PKG"
                                    If (myPolar.CanSave And myPolar.Value.Length = 0) Or (myZ.CanSave And myZ.Value.Length = 0) Then
                                        myGlobal.HasError = True
                                    Else
                                        myScreenDelegate.pArmABSMovPolar = myPolar.Value
                                        myScreenDelegate.pArmABSMovZ = myZ.Value
                                    End If

                                Case "Z_REF"
                                    If (myPolar.CanSave And myPolar.Value.Length = 0) Or (myZ.CanSave And myZ.Value.Length = 0) Then
                                        myGlobal.HasError = True
                                    Else
                                        myScreenDelegate.pArmABSMovPolar = myScreenDelegate.Reagent2ArmPolarZREF.ToString
                                        myScreenDelegate.pArmABSMovZ = myZ.Value
                                    End If

                                Case "RING1", "RING2"
                                    If (myPolar.CanSave And myPolar.Value.Length = 0) Or (myZ.CanSave And myZ.Value.Length = 0) Or (myRotor.CanSave And myRotor.Value.Length = 0) Then
                                        myGlobal.HasError = True
                                    Else
                                        myScreenDelegate.pArmABSMovPolar = myPolar.Value
                                        myScreenDelegate.pArmABSMovZ = myZ.Value
                                        myScreenDelegate.pValueRotorABSMov = myRotor.Value
                                    End If

                            End Select

                        Case ADJUSTMENT_ARMS.MIXER1
                            .AdjustmentID = SetPositionAdjustmentType(ADJUSTMENT_ARMS.MIXER1, pRowIndex)
                            myPolar = ReadGlobalAdjustmentData(.AdjustmentID.ToString, GlobalEnumerates.AXIS.POLAR, True)
                            myZ = ReadGlobalAdjustmentData(.AdjustmentID.ToString, GlobalEnumerates.AXIS.Z, True)
                            myRotor = ReadGlobalAdjustmentData(.AdjustmentID.ToString, GlobalEnumerates.AXIS.ROTOR, True)

                            Select Case Me.BsGridMixer1.IdentValue(pRowIndex).ToString
                                Case "DISP1"
                                    If (myPolar.CanSave And myPolar.Value.Length = 0) Or (myZ.CanSave And myZ.Value.Length = 0) Then
                                        myGlobal.HasError = True
                                    Else
                                        myScreenDelegate.pArmABSMovPolar = myPolar.Value
                                        myScreenDelegate.pArmABSMovZ = myZ.Value
                                    End If

                                Case "WASH", "PKG"
                                    If (myPolar.CanSave And myPolar.Value.Length = 0) Or (myZ.CanSave And myZ.Value.Length = 0) Then
                                        myGlobal.HasError = True
                                    Else
                                        myScreenDelegate.pArmABSMovPolar = myPolar.Value
                                        myScreenDelegate.pArmABSMovZ = myZ.Value
                                    End If

                                Case "Z_REF"
                                    If (myPolar.CanSave And myPolar.Value.Length = 0) Or (myZ.CanSave And myZ.Value.Length = 0) Then
                                        myGlobal.HasError = True
                                    Else
                                        myScreenDelegate.pArmABSMovPolar = myScreenDelegate.Mixer1ArmPolarZREF.ToString
                                        myScreenDelegate.pArmABSMovZ = myZ.Value
                                    End If

                            End Select
                        Case ADJUSTMENT_ARMS.MIXER2
                            .AdjustmentID = SetPositionAdjustmentType(ADJUSTMENT_ARMS.MIXER2, pRowIndex)
                            myPolar = ReadGlobalAdjustmentData(.AdjustmentID.ToString, GlobalEnumerates.AXIS.POLAR, True)
                            myZ = ReadGlobalAdjustmentData(.AdjustmentID.ToString, GlobalEnumerates.AXIS.Z, True)
                            myRotor = ReadGlobalAdjustmentData(.AdjustmentID.ToString, GlobalEnumerates.AXIS.ROTOR, True)

                            Select Case Me.BsGridMixer1.IdentValue(pRowIndex).ToString
                                Case "DISP1"
                                    If (myPolar.CanSave And myPolar.Value.Length = 0) Or (myZ.CanSave And myZ.Value.Length = 0) Then
                                        myGlobal.HasError = True
                                    Else
                                        myScreenDelegate.pArmABSMovPolar = myPolar.Value
                                        myScreenDelegate.pArmABSMovZ = myZ.Value
                                    End If

                                Case "WASH", "PKG"
                                    If (myPolar.CanSave And myPolar.Value.Length = 0) Or (myZ.CanSave And myZ.Value.Length = 0) Then
                                        myGlobal.HasError = True
                                    Else
                                        myScreenDelegate.pArmABSMovPolar = myPolar.Value
                                        myScreenDelegate.pArmABSMovZ = myZ.Value
                                    End If

                                Case "Z_REF"
                                    If (myPolar.CanSave And myPolar.Value.Length = 0) Or (myZ.CanSave And myZ.Value.Length = 0) Then
                                        myGlobal.HasError = True
                                    Else
                                        myScreenDelegate.pArmABSMovPolar = myScreenDelegate.Mixer2ArmPolarZREF.ToString
                                        myScreenDelegate.pArmABSMovZ = myZ.Value
                                    End If

                            End Select
                    End Select

                    If myGlobal.HasError Then
                        PrepareErrorMode()
                        Exit Try
                    Else
                        myGlobal = myScreenDelegate.GetPendingPreliminaryHomes(.AdjustmentID)
                        If myGlobal IsNot Nothing AndAlso Not myGlobal.HasError Then
                            If myGlobal.AffectedRecords > 0 Then
                                Me.AllHomesAreDone = False
                                MyBase.DisplayMessage(Messages.SRV_HOMES_IN_PROGRESS.ToString)
                            Else
                                Me.AllHomesAreDone = True
                                MyBase.DisplayMessage(Messages.SRV_PREPARE_ADJUSTMENTS.ToString)
                            End If
                        End If
                    End If

                End With

                If myGlobal.HasError Then
                    PrepareErrorMode()
                Else
                    Me.SelectedAdjustmentGroup = EditedValue.AdjustmentID 'SGM 28/01/11

                    If MyBase.SimulationMode Then
                        ' simulating
                        Me.Cursor = Cursors.WaitCursor
                        Thread.Sleep(SimulationProcessTime)
                        MyBase.myServiceMDI.Focus()
                        Me.Cursor = Cursors.Default
                        MyBase.CurrentMode = ADJUSTMENT_MODES.ADJUST_PREPARED
                        PrepareArea()
                    Else
                        ' Manage FwScripts must to be sent to adjusting
                        Me.SendFwScript(Me.CurrentMode, EditedValue.AdjustmentID)
                    End If
                    Me.ManageTabPages = False
                    Me.ManageTabArms = False
                    MyBase.ActivateMDIMenusButtons(False) 'SGM 28/09/2011
                End If

            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".BsAdjustButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".BsAdjustButton_Click ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub BsTestButton_Button2Click(ByVal testButton As DataGridViewDisableButtonCell, ByVal pRowIndex As Integer) Handles BsGridSample.Button2Click, _
                                                                              BsGridReagent1.Button2Click, _
                                                                              BsGridReagent2.Button2Click, _
                                                                              BsGridMixer1.Button2Click, _
                                                                              BsGridMixer2.Button2Click
        Dim myGlobal As New GlobalDataTO
        Try
            Me.SelectedRow = pRowIndex

            If MyBase.CurrentMode = ADJUSTMENT_MODES.TESTED Then
                myGlobal = MyBase.BeginTestExit
                If myGlobal.HasError Then
                    PrepareErrorMode()
                Else
                    PrepareArea()

                    If Not myGlobal.HasError Then
                        If MyBase.SimulationMode Then
                            ' simulating
                            Me.Cursor = Cursors.WaitCursor
                            Thread.Sleep(SimulationProcessTime)
                            MyBase.myServiceMDI.Focus()
                            Me.Cursor = Cursors.Default
                            MyBase.CurrentMode = ADJUSTMENT_MODES.TEST_EXITED
                            PrepareArea()
                        Else
                            ' Manage FwScripts must to be sent to Adjust Exiting
                            Me.SendFwScript(Me.CurrentMode, EditedValue.AdjustmentID)
                        End If
                    End If

                End If

            Else

                'select testing group
                Select Case Me.SelectedArmTab
                    Case ADJUSTMENT_ARMS.SAMPLE
                        Select Case Me.BsGridSample.IdentValue(pRowIndex).ToString
                            Case "DISP1" : Me.SelectedAdjustmentGroup = ADJUSTMENT_GROUPS.SAMPLES_ARM_DISP1
                            Case "DISP2" : Me.SelectedAdjustmentGroup = ADJUSTMENT_GROUPS.SAMPLES_ARM_DISP2
                            Case "WASH" : Me.SelectedAdjustmentGroup = ADJUSTMENT_GROUPS.SAMPLES_ARM_WASH
                            Case "RING1" : Me.SelectedAdjustmentGroup = ADJUSTMENT_GROUPS.SAMPLES_ARM_RING1
                            Case "RING2" : Me.SelectedAdjustmentGroup = ADJUSTMENT_GROUPS.SAMPLES_ARM_RING2
                            Case "RING3" : Me.SelectedAdjustmentGroup = ADJUSTMENT_GROUPS.SAMPLES_ARM_RING3
                            Case "Z_TUBE" : Me.SelectedAdjustmentGroup = ADJUSTMENT_GROUPS.SAMPLES_ARM_ZTUBE1
                            Case "REAGENTZ" : Me.SelectedAdjustmentGroup = ADJUSTMENT_GROUPS.REAGENT1_ARM_RING1
                            Case "ISE" : Me.SelectedAdjustmentGroup = ADJUSTMENT_GROUPS.SAMPLES_ARM_ISE
                            Case "PKG" : Me.SelectedAdjustmentGroup = ADJUSTMENT_GROUPS.SAMPLES_ARM_PARK
                        End Select

                    Case ADJUSTMENT_ARMS.REAGENT1
                        Select Case Me.BsGridReagent1.IdentValue(pRowIndex).ToString
                            Case "DISP1" : Me.SelectedAdjustmentGroup = ADJUSTMENT_GROUPS.REAGENT1_ARM_DISP1
                            Case "WASH" : Me.SelectedAdjustmentGroup = ADJUSTMENT_GROUPS.REAGENT1_ARM_WASH
                            Case "RING1" : Me.SelectedAdjustmentGroup = ADJUSTMENT_GROUPS.REAGENT1_ARM_RING1
                            Case "RING2" : Me.SelectedAdjustmentGroup = ADJUSTMENT_GROUPS.REAGENT1_ARM_RING2
                            Case "PKG" : Me.SelectedAdjustmentGroup = ADJUSTMENT_GROUPS.REAGENT1_ARM_PARK
                        End Select

                    Case ADJUSTMENT_ARMS.REAGENT2
                        Select Case Me.BsGridReagent2.IdentValue(pRowIndex).ToString
                            Case "DISP1" : Me.SelectedAdjustmentGroup = ADJUSTMENT_GROUPS.REAGENT2_ARM_DISP1
                            Case "WASH" : Me.SelectedAdjustmentGroup = ADJUSTMENT_GROUPS.REAGENT2_ARM_WASH
                            Case "RING1" : Me.SelectedAdjustmentGroup = ADJUSTMENT_GROUPS.REAGENT2_ARM_RING1
                            Case "RING2" : Me.SelectedAdjustmentGroup = ADJUSTMENT_GROUPS.REAGENT2_ARM_RING2
                            Case "PKG" : Me.SelectedAdjustmentGroup = ADJUSTMENT_GROUPS.REAGENT2_ARM_PARK
                        End Select

                    Case ADJUSTMENT_ARMS.MIXER1
                        Select Case Me.BsGridMixer1.IdentValue(pRowIndex).ToString
                            Case "DISP1" : Me.SelectedAdjustmentGroup = ADJUSTMENT_GROUPS.MIXER1_ARM_DISP1
                            Case "WASH" : Me.SelectedAdjustmentGroup = ADJUSTMENT_GROUPS.MIXER1_ARM_WASH
                            Case "PKG" : Me.SelectedAdjustmentGroup = ADJUSTMENT_GROUPS.MIXER1_ARM_PARK
                        End Select

                    Case ADJUSTMENT_ARMS.MIXER2
                        Select Case Me.BsGridMixer2.IdentValue(pRowIndex).ToString
                            Case "DISP1" : Me.SelectedAdjustmentGroup = ADJUSTMENT_GROUPS.MIXER2_ARM_DISP1
                            Case "WASH" : Me.SelectedAdjustmentGroup = ADJUSTMENT_GROUPS.MIXER2_ARM_WASH
                            Case "PKG" : Me.SelectedAdjustmentGroup = ADJUSTMENT_GROUPS.MIXER2_ARM_PARK
                        End Select

                End Select


                myGlobal = MyBase.BeginTest
                If myGlobal.HasError Then
                    PrepareErrorMode()
                Else
                    PrepareArea()
                    InitializeEditedValue()

                    With EditedValue

                        Dim myPolarValue As String = ""
                        Dim myZValue As String = ""
                        Dim myRotorValue As String = ""
                        Dim myAdditionalZValue As String = ""   ' XBC 13/11/2012
                        Dim myIntZValue As Integer  ' XBC 13/11/2012

                        Select Case SelectedArmTab
                            Case ADJUSTMENT_ARMS.SAMPLE
                                .AdjustmentID = SetPositionAdjustmentType(ADJUSTMENT_ARMS.SAMPLE, pRowIndex)

                                myPolarValue = ReadGlobalAdjustmentData(.AdjustmentID.ToString, GlobalEnumerates.AXIS.POLAR).Value
                                myZValue = ReadGlobalAdjustmentData(.AdjustmentID.ToString, GlobalEnumerates.AXIS.Z).Value
                                myRotorValue = ReadGlobalAdjustmentData(.AdjustmentID.ToString, GlobalEnumerates.AXIS.ROTOR).Value

                                'SGM 02/03/2012 take the Polar and Rotor from Ring1
                                If .AdjustmentID = ADJUSTMENT_GROUPS.SAMPLES_ARM_ZTUBE1 Then
                                    myPolarValue = ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.SAMPLES_ARM_RING1.ToString, GlobalEnumerates.AXIS.POLAR).Value
                                    myRotorValue = ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.SAMPLES_ARM_RING1.ToString, GlobalEnumerates.AXIS.ROTOR).Value
                                    'PENDING TO DEFINE ROTOR POS NUMBER
                                End If

                                If AnalyzerController.Instance.IsBA200 AndAlso .AdjustmentID = ADJUSTMENT_GROUPS.REAGENT1_ARM_RING1 Then
                                    myPolarValue = ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.SAMPLES_ARM_RING1.ToString, GlobalEnumerates.AXIS.POLAR).Value
                                    myRotorValue = ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.SAMPLES_ARM_RING1.ToString, GlobalEnumerates.AXIS.ROTOR).Value
                                End If


                                ' XBC 13/11/2012 - Wash station Z test must add security fly position
                                If Me.BsGridSample.IdentValue(pRowIndex).ToString = "WASH" Then
                                    myAdditionalZValue = ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.SAMPLES_ARM_VSEC.ToString, GlobalEnumerates.AXIS.Z).Value
                                    myIntZValue = CInt(myZValue) + CInt(myAdditionalZValue)
                                    myZValue = myIntZValue.ToString
                                End If

                                Select Case Me.BsGridSample.IdentValue(pRowIndex).ToString
                                    Case "DISP1", "DISP2", "Z_REF", "WASH", "ISE", "PKG"
                                        If myPolarValue.Length = 0 Or myZValue.Length = 0 Then
                                            myGlobal.HasError = True
                                        Else
                                            myScreenDelegate.pArmABSMovPolar = myPolarValue
                                            myScreenDelegate.pArmABSMovZ = myZValue
                                        End If
                                    Case "RING1", "RING2", "RING3"
                                        If myPolarValue.Length = 0 Or myZValue.Length = 0 Or myRotorValue.Length = 0 Then
                                            myGlobal.HasError = True
                                        Else
                                            myScreenDelegate.pArmABSMovPolar = myPolarValue
                                            myScreenDelegate.pArmABSMovZ = myZValue
                                            myScreenDelegate.pValueRotorABSMov = myRotorValue
                                        End If
                                    Case "Z_TUBE", "REAGENTZ"
                                        If myPolarValue.Length = 0 Or myZValue.Length = 0 Or myRotorValue.Length = 0 Then
                                            myGlobal.HasError = True
                                        Else
                                            myScreenDelegate.pArmABSMovPolar = myPolarValue
                                            myScreenDelegate.pArmABSMovZ = myZValue
                                            myScreenDelegate.pValueRotorABSMov = myRotorValue
                                        End If


                                End Select

                            Case ADJUSTMENT_ARMS.REAGENT1
                                .AdjustmentID = SetPositionAdjustmentType(ADJUSTMENT_ARMS.REAGENT1, pRowIndex)

                                myPolarValue = ReadGlobalAdjustmentData(.AdjustmentID.ToString, GlobalEnumerates.AXIS.POLAR).Value
                                myZValue = ReadGlobalAdjustmentData(.AdjustmentID.ToString, GlobalEnumerates.AXIS.Z).Value
                                myRotorValue = ReadGlobalAdjustmentData(.AdjustmentID.ToString, GlobalEnumerates.AXIS.ROTOR).Value

                                ' XBC 13/11/2012 - Wash station Z test must add security fly position
                                If Me.BsGridReagent1.IdentValue(pRowIndex).ToString = "WASH" Then
                                    myAdditionalZValue = ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.REAGENT1_ARM_VSEC.ToString, GlobalEnumerates.AXIS.Z).Value
                                    myIntZValue = CInt(myZValue) + CInt(myAdditionalZValue)
                                    myZValue = myIntZValue.ToString
                                End If

                                Select Case Me.BsGridReagent1.IdentValue(pRowIndex).ToString
                                    Case "DISP1", "Z_REF", "WASH", "PKG"
                                        If myPolarValue.Length = 0 Or myZValue.Length = 0 Then
                                            myGlobal.HasError = True
                                        Else
                                            myScreenDelegate.pArmABSMovPolar = myPolarValue
                                            myScreenDelegate.pArmABSMovZ = myZValue
                                        End If
                                    Case "RING1", "RING2"
                                        If myPolarValue.Length = 0 Or myZValue.Length = 0 Or myRotorValue.Length = 0 Then
                                            myGlobal.HasError = True
                                        Else
                                            myScreenDelegate.pArmABSMovPolar = myPolarValue
                                            myScreenDelegate.pArmABSMovZ = myZValue
                                            myScreenDelegate.pValueRotorABSMov = myRotorValue
                                        End If
                                End Select

                            Case ADJUSTMENT_ARMS.REAGENT2
                                .AdjustmentID = SetPositionAdjustmentType(ADJUSTMENT_ARMS.REAGENT2, pRowIndex)

                                myPolarValue = ReadGlobalAdjustmentData(.AdjustmentID.ToString, GlobalEnumerates.AXIS.POLAR).Value
                                myZValue = ReadGlobalAdjustmentData(.AdjustmentID.ToString, GlobalEnumerates.AXIS.Z).Value
                                myRotorValue = ReadGlobalAdjustmentData(.AdjustmentID.ToString, GlobalEnumerates.AXIS.ROTOR).Value

                                ' XBC 13/11/2012 - Wash station Z test must add security fly position
                                If Me.BsGridReagent2.IdentValue(pRowIndex).ToString = "WASH" Then
                                    myAdditionalZValue = ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.REAGENT2_ARM_VSEC.ToString, GlobalEnumerates.AXIS.Z).Value
                                    myIntZValue = CInt(myZValue) + CInt(myAdditionalZValue)
                                    myZValue = myIntZValue.ToString
                                End If

                                Select Case Me.BsGridReagent2.IdentValue(pRowIndex).ToString
                                    Case "DISP1", "Z_REF", "WASH", "PKG"
                                        If myPolarValue.Length = 0 Or myZValue.Length = 0 Then
                                            myGlobal.HasError = True
                                        Else
                                            myScreenDelegate.pArmABSMovPolar = myPolarValue
                                            myScreenDelegate.pArmABSMovZ = myZValue
                                        End If
                                    Case "RING1", "RING2"
                                        If myPolarValue.Length = 0 Or myZValue.Length = 0 Or myRotorValue.Length = 0 Then
                                            myGlobal.HasError = True
                                        Else
                                            myScreenDelegate.pArmABSMovPolar = myPolarValue
                                            myScreenDelegate.pArmABSMovZ = myZValue
                                            myScreenDelegate.pValueRotorABSMov = myRotorValue
                                        End If
                                End Select

                            Case ADJUSTMENT_ARMS.MIXER1
                                .AdjustmentID = SetPositionAdjustmentType(ADJUSTMENT_ARMS.MIXER1, pRowIndex)

                                myPolarValue = ReadGlobalAdjustmentData(.AdjustmentID.ToString, GlobalEnumerates.AXIS.POLAR).Value
                                myZValue = ReadGlobalAdjustmentData(.AdjustmentID.ToString, GlobalEnumerates.AXIS.Z).Value

                                ' XBC 13/11/2012 - Wash station Z test must add security fly position
                                If Me.BsGridMixer1.IdentValue(pRowIndex).ToString = "WASH" Then
                                    myAdditionalZValue = ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.MIXER1_ARM_VSEC.ToString, GlobalEnumerates.AXIS.Z).Value
                                    myIntZValue = CInt(myZValue) + CInt(myAdditionalZValue)
                                    myZValue = myIntZValue.ToString
                                End If

                                Select Case Me.BsGridMixer1.IdentValue(pRowIndex).ToString
                                    Case "DISP1", "Z_REF", "WASH", "PKG"
                                        If myPolarValue.Length = 0 Or myZValue.Length = 0 Then
                                            myGlobal.HasError = True
                                        Else
                                            myScreenDelegate.pArmABSMovPolar = myPolarValue
                                            myScreenDelegate.pArmABSMovZ = myZValue
                                        End If
                                End Select

                            Case ADJUSTMENT_ARMS.MIXER2
                                .AdjustmentID = SetPositionAdjustmentType(ADJUSTMENT_ARMS.MIXER2, pRowIndex)

                                myPolarValue = ReadGlobalAdjustmentData(.AdjustmentID.ToString, GlobalEnumerates.AXIS.POLAR).Value
                                myZValue = ReadGlobalAdjustmentData(.AdjustmentID.ToString, GlobalEnumerates.AXIS.Z).Value

                                ' XBC 13/11/2012 - Wash station Z test must add security fly position
                                If Me.BsGridMixer2.IdentValue(pRowIndex).ToString = "WASH" Then
                                    myAdditionalZValue = ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.MIXER2_ARM_VSEC.ToString, GlobalEnumerates.AXIS.Z).Value
                                    myIntZValue = CInt(myZValue) + CInt(myAdditionalZValue)
                                    myZValue = myIntZValue.ToString
                                End If

                                Select Case Me.BsGridMixer2.IdentValue(pRowIndex).ToString
                                    Case "DISP1", "Z_REF", "WASH", "PKG"
                                        If myPolarValue.Length = 0 Or myZValue.Length = 0 Then
                                            myGlobal.HasError = True
                                        Else
                                            myScreenDelegate.pArmABSMovPolar = myPolarValue
                                            myScreenDelegate.pArmABSMovZ = myZValue
                                        End If
                                End Select
                        End Select


                        If myGlobal.HasError Then
                            PrepareErrorMode()
                            Exit Try
                        Else
                            myGlobal = myScreenDelegate.GetPendingPreliminaryHomes(.AdjustmentID)
                            If myGlobal IsNot Nothing AndAlso Not myGlobal.HasError Then
                                If myGlobal.AffectedRecords > 0 Then
                                    Me.AllHomesAreDone = False
                                    MyBase.DisplayMessage(Messages.SRV_HOMES_IN_PROGRESS.ToString)
                                Else
                                    Me.AllHomesAreDone = True
                                    MyBase.DisplayMessage(Messages.SRV_PREPARE_TEST.ToString)
                                End If
                            End If
                        End If

                    End With


                    If Not myGlobal.HasError Then
                        If MyBase.SimulationMode Then
                            ' simulating
                            Me.Cursor = Cursors.WaitCursor
                            Thread.Sleep(SimulationProcessTime)
                            MyBase.myServiceMDI.Focus()
                            Me.Cursor = Cursors.Default
                            MyBase.CurrentMode = ADJUSTMENT_MODES.TESTED
                            PrepareArea()
                        Else
                            ' Manage FwScripts must to be sent to testing
                            Me.SendFwScript(Me.CurrentMode, EditedValue.AdjustmentID)
                        End If
                    End If

                End If

            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".BsTestButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".BsTestButton_Click ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub


    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>
    ''' Created by SGM
    ''' AG 01/10/2014 - BA-1953 new photometry adjustment maneuver (use REAGENTS_HOME_ROTOR + REAGENTS_ABS_ROTOR (with parameter = current value of GFWR1) instead of REACTIONS_ROTOR_HOME_WELL1)
    ''' </remarks>
    Private Sub BsAdjustButton_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BsOpticAdjustButton.Click, _
                                                                                                        BsWSAdjustButton.Click
        Dim myGlobal As New GlobalDataTO
        Try
            myGlobal = PrepareAdjust()
            If myGlobal.HasError Then
                PrepareErrorMode()
            Else

                PrepareArea()
                ' Instantiate EditedValue Structure
                InitializeEditedValue()
                With EditedValue
                    Select Case Me.SelectedPage
                        Case ADJUSTMENT_PAGES.OPTIC_CENTERING

                            'AG 01/10/2014 - BA-1953 - also reset REACTIONS_HOME_ROTOR because it is the script used during this adjustment
                            Dim preliminaryHomesToResetList As New List(Of String)
                            preliminaryHomesToResetList.Add(FwSCRIPTS_IDS.REACTIONS_ROTOR_HOME_WELL1.ToString)
                            preliminaryHomesToResetList.Add(FwSCRIPTS_IDS.REACTIONS_HOME_ROTOR.ToString)
                            myGlobal = myScreenDelegate.ResetSpecifiedPreliminaryHomes(myServiceMDI.ActiveAnalyzer, preliminaryHomesToResetList)

                            If myGlobal.HasError Then
                                PrepareErrorMode()
                                Exit Try
                            End If

                            Me.IsCenteringOptic = True

                            .AdjustmentID = ADJUSTMENT_GROUPS.PHOTOMETRY
                            .AxisID = GlobalEnumerates.AXIS.ROTOR
                            myScreenDelegate.LEDIntensity = Me.LEDCurrent
                            myScreenDelegate.AbsorbanceScanDone = False
                            myScreenDelegate.AbsorbanceScanReadings = 0
                            myScreenDelegate.ReadedCounts = New List(Of OpticCenterDataTO)
                            myScreenDelegate.pValueAdjust = BsOpticAdjustmentLabel.Text 'AG 01/10/2014 - BA-1953 inform the current value of adjustment GFWR1 (Posición referencia lectura - Pocillo 1)

                            If Not SimulationMode Then
                                Me.ProgressBar1.Maximum = CInt(myScreenDelegate.NumWells * myScreenDelegate.StepsbyWell)
                                Me.ProgressBar1.Value = 0
                                Me.ProgressBar1.Visible = True
                            End If

                            ' XBC 30/11/2011
                            Me.BsOpticAdjustButton.Visible = False
                            Me.BsOpticStopButton.Visible = True

                        Case ADJUSTMENT_PAGES.WASHING_STATION
                            .AdjustmentID = ADJUSTMENT_GROUPS.WASHING_STATION
                            .AxisID = GlobalEnumerates.AXIS.Z

                            Dim myZ = ReadGlobalAdjustmentData(.AdjustmentID.ToString, GlobalEnumerates.AXIS.Z, True)

                            If myZ.CanSave And myZ.Value.Length = 0 Then
                                myGlobal.HasError = True
                            Else
                                myScreenDelegate.pArmABSMovZ = myZ.Value
                            End If

                        Case ADJUSTMENT_PAGES.ARM_POSITIONS
                            'not applied
                    End Select

                    If myGlobal.HasError Then
                        Select Case Me.SelectedPage
                            Case ADJUSTMENT_PAGES.OPTIC_CENTERING
                                Me.BsOpticAdjustButton.Visible = True
                                Me.BsOpticStopButton.Visible = False

                            Case ADJUSTMENT_PAGES.WASHING_STATION
                                Me.BsWSAdjustButton.Visible = True

                            Case ADJUSTMENT_PAGES.ARM_POSITIONS
                                'not applied

                        End Select

                        PrepareErrorMode()
                        Exit Try
                    Else
                        If MyBase.SimulationMode Then

                            If Not Me.AllHomesAreDone Then
                                DisplayMessage(Messages.SRV_HOMES_IN_PROGRESS.ToString)

                                Me.Cursor = Cursors.WaitCursor
                                Thread.Sleep(SimulationProcessTime)
                                myServiceMDI.Focus()
                                Me.Cursor = Cursors.Default

                                Me.AllHomesAreDone = True
                                DisplayMessage(Messages.SRV_HOMES_FINISHED.ToString)
                                myBaseScreenDelegate.SetPreliminaryHomesAsDone(SelectedAdjustmentGroup)

                                myServiceMDI.Focus()
                                DisplayMessage(Messages.SRV_PREPARE_ADJUSTMENTS.ToString)
                            End If

                        Else
                            myGlobal = myScreenDelegate.GetPendingPreliminaryHomes(.AdjustmentID)
                            If myGlobal IsNot Nothing AndAlso Not myGlobal.HasError Then
                                If myGlobal.AffectedRecords > 0 Then
                                    Me.AllHomesAreDone = False
                                    DisplayMessage(Messages.SRV_HOMES_IN_PROGRESS.ToString)
                                Else
                                    Me.AllHomesAreDone = True
                                    DisplayMessage(Messages.SRV_PREPARE_ADJUSTMENTS.ToString)
                                End If
                            End If
                        End If
                    End If

                End With

                If Me.SelectedPage = ADJUSTMENT_PAGES.OPTIC_CENTERING Then
                    'Initialize Chart
                    For Each s As Series In Me.AbsorbanceChart.Series
                        s.DataSource = Nothing
                    Next
                    For Each s As Series In Me.AbsorbanceChart.Series
                        s.Points.Clear()
                    Next

                    Dim wellCounts As Integer = 0

                    Dim myDiagram As XYDiagram = CType(Me.AbsorbanceChart.Diagram, XYDiagram)
                    For Each L As ConstantLine In myDiagram.AxisX.ConstantLines
                        If L.Name.Contains("New") Then
                            L.AxisValue = wellCounts
                            wellCounts += myScreenDelegate.StepsbyWell  ' 80
                        End If
                    Next
                End If

                If myGlobal.HasError Then
                    Select Case Me.SelectedPage
                        Case ADJUSTMENT_PAGES.OPTIC_CENTERING
                            Me.BsOpticAdjustButton.Visible = True
                            Me.BsOpticStopButton.Visible = False

                        Case ADJUSTMENT_PAGES.WASHING_STATION
                            Me.BsWSAdjustButton.Visible = True

                        Case ADJUSTMENT_PAGES.ARM_POSITIONS

                    End Select

                    PrepareErrorMode()
                Else

                    If MyBase.SimulationMode Then
                        Select Case Me.SelectedPage
                            Case ADJUSTMENT_PAGES.OPTIC_CENTERING
                                Me.CurrentMode = ADJUSTMENT_MODES.ADJUST_PREPARED  ' ABSORBANCE_SCANNING

                            Case ADJUSTMENT_PAGES.WASHING_STATION
                                MyBase.CurrentMode = ADJUSTMENT_MODES.ADJUST_PREPARED

                            Case ADJUSTMENT_PAGES.ARM_POSITIONS

                        End Select

                        Application.DoEvents()

                        myScreenDelegate.AbsorbanceScanDone = True

                        PrepareArea()

                    Else

                        If Me.SelectedPage = ADJUSTMENT_PAGES.WASHING_STATION And Me.CurrentMode <> ADJUSTMENT_MODES.ADJUST_PREPARING Then Exit Sub 'SGM 22/05/2012
                        ' Manage FwScripts must to be sent to adjusting
                        Me.SendFwScript(Me.CurrentMode, EditedValue.AdjustmentID)
                    End If
                End If

            End If


        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".BsAdjustButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".BsAdjustButton_Click ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub BsCancelButton_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BsOpticCancelButton.Click, _
                                                                                                            BsWSCancelButton.Click, _
                                                                                                            BsArmsCancelButton.Click
        Try
            ' XBC 21/05/2012
            Me.IsCenteringOptic = False

            Me.WizardMode = False

            Dim AreChangesToSave As Boolean = False

            If Me.SelectedPage = ADJUSTMENT_PAGES.ARM_POSITIONS Then
                AreChangesToSave = MyClass.IsLocallySaved
            ElseIf Me.SelectedPage = ADJUSTMENT_PAGES.OPTIC_CENTERING Then
                Me.InitializeChart() 'SGM 05/12/2012
            Else
                AreChangesToSave = Me.ChangedValue
            End If

            If AreChangesToSave Then
                Dim dialogResultToReturn As DialogResult = DialogResult.No
                dialogResultToReturn = MyBase.ShowMessage("", Messages.SRV_DISCARD_CHANGES.ToString)

                If dialogResultToReturn = DialogResult.Yes Then

                    Me.BsGridSample.HideAllIconsOk()
                    Me.BsGridReagent1.HideAllIconsOk()
                    Me.BsGridReagent2.HideAllIconsOk()
                    Me.BsGridMixer1.HideAllIconsOk()
                    Me.BsGridMixer2.HideAllIconsOk()
                    MyClass.IsLocallySaved = False
                    Me.ChangedValue = True
                    Me.CancelAdjustment()

                End If
            Else
                If Me.SelectedPage = ADJUSTMENT_PAGES.ARM_POSITIONS Then
                    Me.ChangedValue = False
                End If
                Me.CancelAdjustment()
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".BsCancelButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".BsCancelButton_Click ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub BsArmsOkButton_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BsArmsOkButton.Click
        Try
            MyClass.IsLocallySavedAttr = True
            MyClass.SaveAdjustment()
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".BsArmsOkButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".BsArmsOkButton_Click ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub BsMixButtons_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BsStirrer1Button.Click, BsStirrer2Button.Click
        Try
            If Not Me.IsStirrerTesting Then
                If MyBase.SimulationMode Then
                    MyBase.CurrentMode = ADJUSTMENT_MODES.STIRRER_TEST
                    Me.PrepareArea()
                    Thread.Sleep(MyBase.SimulationProcessTime)
                    MyBase.CurrentMode = ADJUSTMENT_MODES.STIRRER_TESTING

                    Select Case Me.SelectedArmTab
                        Case ADJUSTMENT_ARMS.MIXER1
                            Me.PrepareStirrerButton(Me.BsStirrer1Button)
                        Case ADJUSTMENT_ARMS.MIXER2
                            Me.PrepareStirrerButton(Me.BsStirrer2Button)
                    End Select
                    Me.PrepareArea()
                Else
                    MyBase.CurrentMode = ADJUSTMENT_MODES.STIRRER_TEST
                    Me.PrepareArea()

                    Select Case Me.SelectedArmTab
                        Case ADJUSTMENT_ARMS.MIXER1
                            Me.SendFwScript(MyBase.CurrentMode, ADJUSTMENT_GROUPS.MIXER1_ARM_DISP1)
                        Case ADJUSTMENT_ARMS.MIXER2
                            Me.SendFwScript(MyBase.CurrentMode, ADJUSTMENT_GROUPS.MIXER2_ARM_DISP1)
                    End Select

                End If

            Else
                MyBase.CurrentMode = ADJUSTMENT_MODES.STIRRER_TESTING

                If MyBase.SimulationMode Then
                    Thread.Sleep(MyBase.SimulationProcessTime)
                    ' XBC 13/10/2011
                    Me.PrepareStirrerTestedMode()

                Else
                    Select Case Me.SelectedArmTab
                        Case ADJUSTMENT_ARMS.MIXER1
                            Me.SendFwScript(MyBase.CurrentMode, ADJUSTMENT_GROUPS.MIXER1_ARM_DISP1)
                        Case ADJUSTMENT_ARMS.MIXER2
                            Me.SendFwScript(MyBase.CurrentMode, ADJUSTMENT_GROUPS.MIXER2_ARM_DISP1)
                    End Select

                End If
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".BsMixButtons_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".BsMixButtons_Click ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub BsUpDownWSButton1_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BsUpDownWSButton1.Click
        Try
            Me.SendWASH_STATION_CTRL()

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".BsUpDownWSButton1_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".BsUpDownWSButton1_Click ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub BsUpDownWSButton2_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BsUpDownWSButton2.Click
        Try
            Me.SendWASH_STATION_CTRL()

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".BsUpDownWSButton2_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".BsUpDownWSButton2_Click ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub BsUpDownWSButton3_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BsUpDownWSButton3.Click
        Try
            Me.SendWASH_STATION_CTRL()

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".BsUpDownWSButton3_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".BsUpDownWSButton3_Click ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Stop Reading Counts operations
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>Created by XBC 30/11/2011</remarks>
    Private Sub BsStopButton_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BsOpticStopButton.Click
        Try
            Select Case Me.SelectedPage
                Case ADJUSTMENT_PAGES.OPTIC_CENTERING
                    Me.BsOpticAdjustButton.Visible = True
                    Me.BsOpticStopButton.Visible = False
                    Me.IsReadingCountsAbortedByUser = True
                    Me.CurrentMode = ADJUSTMENT_MODES.ABSORBANCE_PREPARED
                    PrepareArea()
            End Select

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".BsStopButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".BsStopButton_Click ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub BsStopButton_MouseHover(ByVal sender As Object, ByVal e As EventArgs) Handles BsOpticStopButton.MouseHover
        Me.Cursor = Cursors.Default
    End Sub


    ''' <summary>
    ''' Change cursor when Mouse is over Stop button
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>Created by XBC 30/11/2011</remarks>
    Private Sub BsStopButton_MouseMove(ByVal sender As Object, ByVal e As MouseEventArgs) Handles BsOpticStopButton.MouseMove
        Me.Cursor = Cursors.Default
    End Sub

    Private Sub AbsorbanceChart_Paint(ByVal sender As Object, ByVal e As PaintEventArgs) Handles AbsorbanceChart.Paint
        Try

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".AbsorbanceChart_Paint ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".AbsorbanceChart_Paint ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub AbsorbanceChart_CustomPaint(ByVal sender As Object, ByVal e As CustomPaintEventArgs) Handles AbsorbanceChart.CustomPaint
        Try

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".AbsorbanceChart_Paint ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".AbsorbanceChart_Paint ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub AbsorbanceChart_Zoom(ByVal sender As Object, ByVal e As ChartZoomEventArgs) Handles AbsorbanceChart.Zoom
        Try

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".AbsorbanceChart_Paint ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".AbsorbanceChart_Paint ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Tooltip Information to display
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>Created by XBC 02/01/2012 - Add Encoder functionality</remarks>
    Private Sub chartControl1_MouseMove(ByVal sender As Object, ByVal e As MouseEventArgs) Handles AbsorbanceChart.MouseMove
        Try
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            ' Obtain hit information under the test point.
            Dim hi As ChartHitInfo = AbsorbanceChart.CalcHitInfo(e.X, e.Y)

            If hi.Series IsNot Nothing AndAlso hi.Series.LegendText = Me.AbsorbanceChart.Series(0).LegendText Then

                ' Obtain the series point under the test point.
                Dim point As SeriesPoint = hi.SeriesPoint

                ' Check whether the series point was clicked or not.
                If point IsNot Nothing Then
                    ' Obtain the series point argument.
                    Dim StepsToHome As Double

                    If IsNumeric(point.Argument) And IsNumeric(EditedValue.LastRotorValue) And IsNumeric(myScreenDelegate.StepsbyWell) Then
                        StepsToHome = CDbl(point.Argument) + CDbl(EditedValue.LastRotorValue) ' - (myScreenDelegate.StepsbyWell / 2)

                        Dim argument As String = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_STEPS_TO_HOME", currentLanguage) & ": " & StepsToHome.ToString()

                        ' Obtain series point values.
                        Dim values As String = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_NumberCounts", currentLanguage) & ": " & point.Values(0).ToString()

                        ' Show the tooltip.
                        ToolTipController1.ShowHint(argument & vbLf & values, myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_SeriesPointData", currentLanguage))
                    End If
                Else
                    ' Hide the tooltip.
                    ToolTipController1.HideHint()
                End If

            Else
                ' Hide the tooltip.
                ToolTipController1.HideHint()
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".chartControl1_MouseMove ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".chartControl1_MouseMove ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Individually Customize Axis Labels
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>Created by XBC 02/01/2012 - Add Encoder functionality</remarks>
    Private Sub AbsorbanceChart_CustomDrawAxisLabel(ByVal sender As Object, ByVal e As CustomDrawAxisLabelEventArgs) Handles AbsorbanceChart.CustomDrawAxisLabel
        Try
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate
            Dim axis As AxisBase = e.Item.Axis
            If TypeOf axis Is AxisX Then
                Select Case e.Item.Text
                    Case "40"
                        e.Item.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_CurveReplicate_Well", currentLanguage) & " 1"
                    Case "120"
                        e.Item.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_CurveReplicate_Well", currentLanguage) & " 2"
                    Case "200"
                        e.Item.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_CurveReplicate_Well", currentLanguage) & " 3"
                    Case "280"
                        e.Item.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_CurveReplicate_Well", currentLanguage) & " 4"
                    Case "360"
                        e.Item.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_CurveReplicate_Well", currentLanguage) & " 5"
                    Case Else
                        e.Item.Text = ""
                End Select
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".AbsorbanceChart.CustomDrawAxisLabel ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
        End Try
    End Sub

    ''' <summary>
    ''' Display a label with the steps between center and transition 1 by each well
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>Created by XBC 04/01/2012 - Add Encoder functionality</remarks>
    Private Sub chartControl1_CustomDrawSeriesPoint(ByVal sender As Object, ByVal e As CustomDrawSeriesPointEventArgs) Handles AbsorbanceChart.CustomDrawSeriesPoint
        Try
            e.LabelText = ""
            If EncoderTransitions IsNot Nothing And CenterDistances IsNot Nothing Then
                If EncoderTransitions.Count > 0 And CenterDistances.Count > 0 Then
                    For i As Integer = 0 To EncoderTransitions.Count - 1
                        If IsNumeric(e.SeriesPoint.Argument) Then
                            If CSng(e.SeriesPoint.Argument) = EncoderTransitions(i) Then
                                If i < CenterDistances.Count Then
                                    e.LabelText = CenterDistances(i).ToString
                                End If
                                Exit For
                            End If
                        End If
                    Next
                End If
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".AbsorbanceChart.CustomDrawSeriesPoint ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
        End Try
    End Sub

    Private Sub BsAdjustControl_VisibleChanged(ByVal sender As Object, ByVal e As EventArgs) Handles BsAdjustPolar.VisibleChanged, _
                                                                                                               BsAdjustZ.VisibleChanged, _
                                                                                                               BsAdjustRotor.VisibleChanged
        Try

            If Me.BsAdjustRotor.Visible Then
                Me.BsArmsCancelButton.Left = Me.BsAdjustRotor.Left + 160

            ElseIf Me.BsAdjustZ.Visible Then
                Me.BsArmsCancelButton.Left = Me.BsAdjustZ.Left + 160

            ElseIf Me.BsAdjustPolar.Visible Then
                Me.BsArmsCancelButton.Left = Me.BsAdjustPolar.Left + 160
            End If

            Me.BsArmsOkButton.Left = Me.BsArmsCancelButton.Left

            Me.BsArmsCancelButton.Refresh()
            Me.BsArmsOkButton.Refresh()

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".BsAdjustControl_VisibleChanged ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".BsAdjustControl_VisibleChanged ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

#End Region

#Region "XPS Info Events"

    Private Sub BsXPSViewer_Load(ByVal sender As Object, ByVal e As EventArgs) Handles BsInfoOptXPSViewer.Load, _
                                                                                            BsInfoWsXPSViewer.Load, _
                                                                                            BsInfoArmsXPSViewer.Load
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

#Region "event handler overloads"
    ' SET ABSOLUTE POINT
    Private Sub BsAdjustOptic_SetABSPointReleased(ByVal sender As Object, ByVal Value As Single) Handles BsAdjustOptic.AbsoluteSetPointReleased
        Try
            ' XBC 04/01/2012 
            'EditedValue.AxisID = GlobalEnumerates.AXIS.ROTOR
            'EditedValue.NewRotorValue = Value

            'If MyBase.SimulationMode Then
            '    Me.SimulateABSPositioning()
            '    ' XBC 03/01/2012
            '    Me.BsSaveButton.Focus()
            '    Me.BsAdjustOptic.Focus()
            '    ' XBC 03/01/2012
            'Else
            '    MakeAdjustment(MOVEMENT.ABSOLUTE)
            '    MyBase.DisplayMessage(Messages.SRV_ABS_REQUESTED.ToString)
            'End If
            ' XBC 04/01/2012 
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".BsAdjustOptic_SetABSPointReleased ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".BsAdjustOptic_SetABSPointReleased ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub
    Private Sub BsAdjustWashing_SetABSPointReleased(ByVal sender As Object, ByVal Value As Single) Handles BsAdjustWashing.AbsoluteSetPointReleased
        Try
            EditedValue.AxisID = GlobalEnumerates.AXIS.Z
            EditedValue.NewZValue = Value

            If MyBase.SimulationMode Then
                Me.SimulateABSPositioning()
                ' XBC 03/01/2012
                Me.BsSaveButton.Focus()
                Me.BsAdjustWashing.Focus()
                ' XBC 03/01/2012
            Else
                MakeAdjustment(MOVEMENT.ABSOLUTE)
                MyBase.DisplayMessage(Messages.SRV_ABS_REQUESTED.ToString)
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".BsAdjustWashing_SetABSPointReleased ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".BsAdjustWashing_SetABSPointReleased ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub
    Private Sub BsAdjustPolar_SetABSPointReleased(ByVal sender As Object, ByVal Value As Single) Handles BsAdjustPolar.AbsoluteSetPointReleased
        Try
            EditedValue.AxisID = GlobalEnumerates.AXIS.POLAR
            EditedValue.NewPolarValue = Value

            If MyBase.SimulationMode Then
                Me.SimulateABSPositioning()
                ' XBC 03/01/2012
                Me.BsArmsOkButton.Focus()
                Me.BsSaveButton.Focus()
                Me.BsAdjustPolar.Focus()
                ' XBC 03/01/2012
            Else
                MakeAdjustment(MOVEMENT.ABSOLUTE)
                MyBase.DisplayMessage(Messages.SRV_ABS_REQUESTED.ToString)
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".BsAdjustPolar_SetABSPointReleased ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".BsAdjustPolar_SetABSPointReleased ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub
    Private Sub BsAdjustZ_SetABSPointReleased(ByVal sender As Object, ByVal Value As Single) Handles BsAdjustZ.AbsoluteSetPointReleased
        Try
            EditedValue.AxisID = GlobalEnumerates.AXIS.Z
            EditedValue.NewZValue = Value

            If MyBase.SimulationMode Then
                Me.SimulateABSPositioning()
                ' XBC 03/01/2012
                Me.BsArmsOkButton.Focus()
                Me.BsSaveButton.Focus()
                Me.BsAdjustZ.Focus()
                ' XBC 03/01/2012
            Else
                MakeAdjustment(MOVEMENT.ABSOLUTE)
                MyBase.DisplayMessage(Messages.SRV_ABS_REQUESTED.ToString)
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".BsAdjustZ_SetABSPointReleased ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".BsAdjustZ_SetABSPointReleased ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub
    Private Sub BsAdjustRotor_SetABSPointReleased(ByVal sender As Object, ByVal Value As Single) Handles BsAdjustRotor.AbsoluteSetPointReleased
        Try
            EditedValue.AxisID = GlobalEnumerates.AXIS.ROTOR
            EditedValue.NewRotorValue = Value

            If MyBase.SimulationMode Then
                Me.SimulateABSPositioning()
                ' XBC 03/01/2012
                Me.BsArmsOkButton.Focus()
                Me.BsSaveButton.Focus()
                Me.BsAdjustRotor.Focus()
                ' XBC 03/01/2012
            Else
                MakeAdjustment(MOVEMENT.ABSOLUTE)
                MyBase.DisplayMessage(Messages.SRV_ABS_REQUESTED.ToString)
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".BsAdjustRotor_SetABSPointReleased ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".BsAdjustRotor_SetABSPointReleased ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ' SET RELATIVE POINT
    Private Sub BsAdjustOptic_SetRELPointReleased(ByVal sender As Object, ByVal Value As Single) Handles BsAdjustOptic.RelativeSetPointReleased
        Try
            ' XBC 29/10/2012
            If MyBase.CurrentMode = ADJUSTMENT_MODES.ADJUSTING Or MyBase.CurrentMode = ADJUSTMENT_MODES.ERROR_MODE Then Exit Sub

            EditedValue.AxisID = GlobalEnumerates.AXIS.ROTOR
            EditedValue.stepValue = Value
            EditedValue.NewRotorValue = EditedValue.CurrentRotorValue + Value

            ' XBC 13/11/2012 - fix enabling control
            MoveAbsorbanceChartWells(CInt(Value))

            If MyBase.SimulationMode Then
                Me.SimulateRELPositioning()
                ' XBC 03/01/2012
                Me.BsArmsOkButton.Focus()
                Me.BsSaveButton.Focus()
                Me.BsAdjustOptic.Focus()
                ' XBC 03/01/2012
            Else
                MakeAdjustment(MOVEMENT.RELATIVE)
                MyBase.DisplayMessage(Messages.SRV_STEP_POSITIONING.ToString)
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".BsAdjustOptic_SetRELPointReleased ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".BsAdjustOptic_SetRELPointReleased ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub
    Private Sub BsAdjustWashing_SetRELPointReleased(ByVal sender As Object, ByVal Value As Single) Handles BsAdjustWashing.RelativeSetPointReleased
        Try
            EditedValue.AxisID = GlobalEnumerates.AXIS.Z
            EditedValue.stepValue = Value
            EditedValue.NewZValue = EditedValue.CurrentZValue + Value

            If MyBase.SimulationMode Then
                Me.SimulateRELPositioning()
                ' XBC 03/01/2012
                Me.BsArmsOkButton.Focus()
                Me.BsSaveButton.Focus()
                Me.BsAdjustWashing.Focus()
                ' XBC 03/01/2012
            Else
                MakeAdjustment(MOVEMENT.RELATIVE)
                MyBase.DisplayMessage(Messages.SRV_STEP_POSITIONING.ToString)
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".BsAdjustWashing_SetRELPointReleased ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".BsAdjustWashing_SetRELPointReleased ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub
    Private Sub BsAdjustPolar_SetRELPointReleased(ByVal sender As Object, ByVal Value As Single) Handles BsAdjustPolar.RelativeSetPointReleased
        Try
            EditedValue.AxisID = GlobalEnumerates.AXIS.POLAR
            EditedValue.stepValue = Value
            EditedValue.NewPolarValue = EditedValue.CurrentPolarValue + Value

            If MyBase.SimulationMode Then
                Me.SimulateRELPositioning()
                ' XBC 03/01/2012
                Me.BsArmsOkButton.Focus()
                Me.BsSaveButton.Focus()
                Me.BsAdjustPolar.Focus()
                ' XBC 03/01/2012
            Else
                MakeAdjustment(MOVEMENT.RELATIVE)
                MyBase.DisplayMessage(Messages.SRV_STEP_POSITIONING.ToString)
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".BsAdjustPolar_SetRELPointReleased ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".BsAdjustPolar_SetRELPointReleased ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub
    Private Sub BsAdjustZ_SetRELPointReleased(ByVal sender As Object, ByVal Value As Single) Handles BsAdjustZ.RelativeSetPointReleased
        Try
            EditedValue.AxisID = GlobalEnumerates.AXIS.Z
            EditedValue.stepValue = Value
            EditedValue.NewZValue = EditedValue.CurrentZValue + Value

            If MyBase.SimulationMode Then
                Me.SimulateRELPositioning()
                ' XBC 03/01/2012
                Me.BsArmsOkButton.Focus()
                Me.BsSaveButton.Focus()
                Me.BsAdjustZ.Focus()
                ' XBC 03/01/2012
            Else
                MakeAdjustment(MOVEMENT.RELATIVE)
                MyBase.DisplayMessage(Messages.SRV_STEP_POSITIONING.ToString)
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".BsAdjustZ_SetRELPointReleased ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".BsAdjustZ_SetRELPointReleased ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub
    Private Sub BsAdjustRotor_SetRELPointReleased(ByVal sender As Object, ByVal Value As Single) Handles BsAdjustRotor.RelativeSetPointReleased
        Try
            EditedValue.AxisID = GlobalEnumerates.AXIS.ROTOR
            EditedValue.stepValue = Value
            EditedValue.NewRotorValue = EditedValue.CurrentRotorValue + Value

            If MyBase.SimulationMode Then
                Me.SimulateRELPositioning()
                ' XBC 03/01/2012
                Me.BsArmsOkButton.Focus()
                Me.BsSaveButton.Focus()
                Me.BsAdjustRotor.Focus()
                ' XBC 03/01/2012
            Else
                MakeAdjustment(MOVEMENT.RELATIVE)
                MyBase.DisplayMessage(Messages.SRV_STEP_POSITIONING.ToString)
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".BsAdjustRotor_SetRELPointReleased ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".BsAdjustRotor_SetRELPointReleased ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ' HOMES
    Private Sub BsAdjustOptic_HomeRequestReleased(ByVal sender As Object) Handles BsAdjustOptic.HomeRequestReleased
        Try
            'EditedValue.AxisID = GlobalEnumerates.AXIS.ROTOR
            'EditedValue.NewRotorValue = 0

            'If MyBase.SimulationMode Then
            '    Me.SimulateHOMEPositioning()
            '    BsAdjustOptic.CurrentValue = 0
            '    ' XBC 03/01/2012
            'Me.BsArmsOkButton.Focus()
            '    Me.BsSaveButton.Focus()
            '    Me.BsAdjustOptic.Focus()
            '    ' XBC 03/01/2012
            'Else
            '    MakeAdjustment(MOVEMENT.HOME)
            '    MyBase.DisplayMessage(Messages.SRV_HOMES_IN_PROGRESS.ToString)
            'End If
            ' XBC 04/01/2012 
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".BsAdjustOptic_HomeRequestReleased ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".BsAdjustOptic_HomeRequestReleased ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub
    Private Sub BsAdjustWashing_HomeRequestReleased(ByVal sender As Object) Handles BsAdjustWashing.HomeRequestReleased
        Try
            EditedValue.AxisID = GlobalEnumerates.AXIS.Z
            EditedValue.NewZValue = Me.HomeZ

            If MyBase.SimulationMode Then
                Me.SimulateHOMEPositioning()
                BsAdjustWashing.CurrentValue = 0
                ' XBC 03/01/2012
                Me.BsArmsOkButton.Focus()
                Me.BsSaveButton.Focus()
                Me.BsAdjustWashing.Focus()
                ' XBC 03/01/2012
            Else
                MakeAdjustment(MOVEMENT.HOME)
                MyBase.DisplayMessage(Messages.SRV_HOMES_IN_PROGRESS.ToString)
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".BsAdjustWashing_HomeRequestReleased ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".BsAdjustWashing_HomeRequestReleased ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub
    Private Sub BsAdjustPolar_HomeRequestReleased(ByVal sender As Object) Handles BsAdjustPolar.HomeRequestReleased
        Try
            EditedValue.AxisID = GlobalEnumerates.AXIS.POLAR
            EditedValue.NewPolarValue = Me.HomePolar

            If MyBase.SimulationMode Then
                Me.SimulateHOMEPositioning()
                BsAdjustPolar.CurrentValue = 0
                EditedValue.CurrentPolarValue = 0
                ' XBC 03/01/2012
                Me.BsArmsOkButton.Focus()
                Me.BsSaveButton.Focus()
                Me.BsAdjustPolar.Focus()
                ' XBC 03/01/2012
            Else
                MakeAdjustment(MOVEMENT.HOME)
                MyBase.DisplayMessage(Messages.SRV_HOMES_IN_PROGRESS.ToString)
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".BsAdjustPolar_HomeRequestReleased ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".BsAdjustPolar_HomeRequestReleased ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub
    Private Sub BsAdjustZ_HomeRequestReleased(ByVal sender As Object) Handles BsAdjustZ.HomeRequestReleased
        Try
            EditedValue.AxisID = GlobalEnumerates.AXIS.Z
            EditedValue.NewZValue = Me.HomeZ

            If MyBase.SimulationMode Then
                Me.SimulateHOMEPositioning()
                BsAdjustZ.CurrentValue = 0
                EditedValue.CurrentZValue = 0
                ' XBC 03/01/2012
                Me.BsArmsOkButton.Focus()
                Me.BsSaveButton.Focus()
                Me.BsAdjustZ.Focus()
                ' XBC 03/01/2012
            Else
                MakeAdjustment(MOVEMENT.HOME)
                MyBase.DisplayMessage(Messages.SRV_HOMES_IN_PROGRESS.ToString)
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".BsAdjustZ_HomeRequestReleased ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".BsAdjustZ_HomeRequestReleased ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub
    Private Sub BsAdjustRotor_HomeRequestReleased(ByVal sender As Object) Handles BsAdjustRotor.HomeRequestReleased
        Try
            EditedValue.AxisID = GlobalEnumerates.AXIS.ROTOR
            EditedValue.NewRotorValue = Me.HomeRotor

            If MyBase.SimulationMode Then
                Me.SimulateHOMEPositioning()
                BsAdjustRotor.CurrentValue = 0
                EditedValue.CurrentRotorValue = 0
                ' XBC 03/01/2012
                Me.BsArmsOkButton.Focus()
                Me.BsSaveButton.Focus()
                Me.BsAdjustRotor.Focus()
                ' XBC 03/01/2012
            Else
                MakeAdjustment(MOVEMENT.HOME)
                MyBase.DisplayMessage(Messages.SRV_HOMES_IN_PROGRESS.ToString)
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".BsAdjustRotor_HomeRequestReleased ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".BsAdjustRotor_HomeRequestReleased ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ' SET POINT OUT OF RANGE
    Private Sub BsAdjustOptic_SetPointOutOfRange(ByVal sender As Object) Handles BsAdjustOptic.SetPointOutOfRange
        Try
            MyBase.DisplayMessage(Messages.SRV_OUTOFRANGE.ToString)
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".BsAdjustOptic_SetPointOutOfRange ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".BsAdjustOptic_SetPointOutOfRange ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub
    Private Sub BsAdjustWashing_SetPointOutOfRange(ByVal sender As Object) Handles BsAdjustWashing.SetPointOutOfRange
        Try
            MyBase.DisplayMessage(Messages.SRV_OUTOFRANGE.ToString)
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".BsAdjustWashing_SetPointOutOfRange ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".BsAdjustWashing_SetPointOutOfRange ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub
    Private Sub BsAdjustPolar_SetPointOutOfRange(ByVal sender As Object) Handles BsAdjustPolar.SetPointOutOfRange
        Try
            MyBase.DisplayMessage(Messages.SRV_OUTOFRANGE.ToString)
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".BsAdjustPolar_SetPointOutOfRange ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".BsAdjustPolar_SetPointOutOfRange ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub
    Private Sub BsAdjustZ_SetPointOutOfRange(ByVal sender As Object) Handles BsAdjustZ.SetPointOutOfRange
        Try
            MyBase.DisplayMessage(Messages.SRV_OUTOFRANGE.ToString)
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".BsAdjustZ_SetPointOutOfRange ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".BsAdjustZ_SetPointOutOfRange ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub
    Private Sub BsAdjustRotor_SetPointOutOfRange(ByVal sender As Object) Handles BsAdjustRotor.SetPointOutOfRange
        Try
            MyBase.DisplayMessage(Messages.SRV_OUTOFRANGE.ToString)
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".BsAdjustRotor_SetPointOutOfRange ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".BsAdjustRotor_SetPointOutOfRange ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ' VALIDATION ERROR
    Private Sub BsAdjustOptic_ValidationError(ByVal sender As Object, ByVal Value As String) Handles BsAdjustOptic.ValidationError
        Try
            MyBase.DisplayMessage(Messages.FWSCRIPT_VALIDATION_ERROR.ToString)
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".BsAdjustOptic_ValidationError ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".BsAdjustOptic_ValidationError ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub
    Private Sub BsAdjustWashing_ValidationError(ByVal sender As Object, ByVal Value As String) Handles BsAdjustWashing.ValidationError
        Try
            MyBase.DisplayMessage(Messages.FWSCRIPT_VALIDATION_ERROR.ToString)
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".BsAdjustWashing_ValidationError ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".BsAdjustWashing_ValidationError ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub
    Private Sub BsAdjustPolar_ValidationError(ByVal sender As Object, ByVal Value As String) Handles BsAdjustPolar.ValidationError
        Try
            MyBase.DisplayMessage(Messages.FWSCRIPT_VALIDATION_ERROR.ToString)
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".BsAdjustPolar_ValidationError ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".BsAdjustPolar_ValidationError ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub
    Private Sub BsAdjustZ_ValidationError(ByVal sender As Object, ByVal Value As String) Handles BsAdjustZ.ValidationError
        Try
            MyBase.DisplayMessage(Messages.FWSCRIPT_VALIDATION_ERROR.ToString)
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".BsAdjustZ_ValidationError ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".BsAdjustZ_ValidationError ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub
    Private Sub BsAdjustRotor_ValidationError(ByVal sender As Object, ByVal Value As String) Handles BsAdjustRotor.ValidationError
        Try
            MyBase.DisplayMessage(Messages.FWSCRIPT_VALIDATION_ERROR.ToString)
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".BsAdjustRotor_ValidationError ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".BsAdjustRotor_ValidationError ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ' FOCUS RECEIVED
    Private Sub BsAdjustOptic_FocusReceived(ByVal sender As Object) Handles BsAdjustOptic.FocusReceived
        Try

            MyBase.myFocusedAdjustControl = CType(sender, BSAdjustControl)

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".BsAdjustOptic_FocusReceived ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".BsAdjustOptic_FocusReceived ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub
    Private Sub BsAdjustWashing_FocusReceived(ByVal sender As Object) Handles BsAdjustWashing.FocusReceived
        Try

            MyBase.myFocusedAdjustControl = CType(sender, BSAdjustControl)

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".BsAdjustOptic_FocusReceived ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".BsAdjustOptic_FocusReceived ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub
    Private Sub BsAdjustPolar_FocusReceived(ByVal sender As Object) Handles BsAdjustPolar.FocusReceived
        Try


            Dim send As Object = Nothing
            Dim myCellEventArgs As DataGridViewCellEventArgs = New DataGridViewCellEventArgs(POLAR_COLUMN + 3, SelectedRow)
            If Not FocusedFromGrid Then
                Select Case SelectedArmTab
                    Case ADJUSTMENT_ARMS.SAMPLE
                        Me.BsGridSample.CellClick(send, myCellEventArgs)
                    Case ADJUSTMENT_ARMS.REAGENT1
                        Me.BsGridReagent1.CellClick(send, myCellEventArgs)
                    Case ADJUSTMENT_ARMS.REAGENT2
                        Me.BsGridReagent2.CellClick(send, myCellEventArgs)
                    Case ADJUSTMENT_ARMS.MIXER1
                        Me.BsGridMixer1.CellClick(send, myCellEventArgs)
                    Case ADJUSTMENT_ARMS.MIXER2
                        Me.BsGridMixer2.CellClick(send, myCellEventArgs)
                End Select

            End If

            FocusedFromGrid = False


            MyBase.myFocusedAdjustControl = CType(sender, BSAdjustControl)

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".BsAdjustPolar_FocusReceived ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".BsAdjustPolar_FocusReceived ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' disables buttons while editing a control
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="pEditionMode"></param>
    ''' <remarks>SGM 03/02/11</remarks>
    Private Sub BsAdjustControl_OnEditionMode(ByVal sender As Object, ByVal pEditionMode As Boolean) Handles BsAdjustOptic.EditionModeChanged, _
                                                                                                             BsAdjustWashing.EditionModeChanged, _
                                                                                                             BsAdjustPolar.EditionModeChanged, _
                                                                                                             BsAdjustZ.EditionModeChanged, _
                                                                                                             BsAdjustRotor.EditionModeChanged
        Try
            Dim myControl As BSAdjustControl = CType(sender, BSAdjustControl)
            If myControl.Enabled Then
                With MyBase.myScreenLayout.ButtonsPanel
                    If pEditionMode Then
                        .AdjustButton.Enabled = False
                        .SaveButton.Enabled = False
                    End If
                End With
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".BsAdjustControl_OnEditionMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".BsAdjustControl_OnEditionMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub
    Private Sub BsAdjustZ_FocusReceived(ByVal sender As Object) Handles BsAdjustZ.FocusReceived
        Try

            Dim send As Object = Nothing
            Dim myCellEventArgs As DataGridViewCellEventArgs = New DataGridViewCellEventArgs(Z_COLUMN + 3, SelectedRow)
            If Not FocusedFromGrid Then
                Select Case SelectedArmTab
                    Case ADJUSTMENT_ARMS.SAMPLE
                        Me.BsGridSample.CellClick(send, myCellEventArgs)
                    Case ADJUSTMENT_ARMS.REAGENT1
                        Me.BsGridReagent1.CellClick(send, myCellEventArgs)
                    Case ADJUSTMENT_ARMS.REAGENT2
                        Me.BsGridReagent2.CellClick(send, myCellEventArgs)
                    Case ADJUSTMENT_ARMS.MIXER1
                        Me.BsGridMixer1.CellClick(send, myCellEventArgs)
                    Case ADJUSTMENT_ARMS.MIXER2
                        Me.BsGridMixer2.CellClick(send, myCellEventArgs)
                End Select
            End If


            FocusedFromGrid = False

            MyBase.myFocusedAdjustControl = CType(sender, BSAdjustControl)

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".BsAdjustZ_FocusReceived ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".BsAdjustZ_FocusReceived ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub
    Private Sub BsAdjustRotor_FocusReceived(ByVal sender As Object) Handles BsAdjustRotor.FocusReceived
        Try

            Dim send As Object = Nothing
            Dim myCellEventArgs As DataGridViewCellEventArgs = New DataGridViewCellEventArgs(ROTOR_COLUMN + 3, SelectedRow)
            If Not FocusedFromGrid Then
                Select Case SelectedArmTab
                    Case ADJUSTMENT_ARMS.SAMPLE
                        Me.BsGridSample.CellClick(send, myCellEventArgs)
                    Case ADJUSTMENT_ARMS.REAGENT1
                        Me.BsGridReagent1.CellClick(send, myCellEventArgs)
                    Case ADJUSTMENT_ARMS.REAGENT2
                        Me.BsGridReagent2.CellClick(send, myCellEventArgs)
                    Case ADJUSTMENT_ARMS.MIXER1
                        Me.BsGridMixer1.CellClick(send, myCellEventArgs)
                    Case ADJUSTMENT_ARMS.MIXER2
                        Me.BsGridMixer2.CellClick(send, myCellEventArgs)
                End Select
            End If


            FocusedFromGrid = False

            MyBase.myFocusedAdjustControl = CType(sender, BSAdjustControl)

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".BsAdjustRotor_FocusReceived ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".BsAdjustRotor_FocusReceived ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub


#Region "LED Current"
    Private Sub LEDCurrentTrackBar_ValueChanged(ByVal sender As Object, ByVal e As EventArgs) Handles BsLEDCurrentTrackBar.ValueChanged
        Try
            Me.LEDCurrent = Me.BsLEDCurrentTrackBar.Value
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".LEDCurrentTrackBar_ValueChanged ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".LEDCurrentTrackBar_ValueChanged ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub BsPlusMinusLabel_EnabledChanged(ByVal sender As Object, ByVal e As EventArgs) Handles BsPlusLabel.EnabledChanged, BsMinusLabel.EnabledChanged
        Try

            If Not BsLEDCurrentTrackBar.Enabled Then
                Dim myLabel As Label = CType(sender, Label)
                If myLabel IsNot Nothing Then
                    If Not myLabel.Enabled Then
                        myLabel.ForeColor = Color.DimGray
                        myLabel.BackColor = Color.Transparent
                    End If
                End If
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".BsPlusMinusLabels_MouseEnter ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".BsPlusMinusLabels_MouseEnter ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub BsPlusMinusLabels_MouseEnter(ByVal sender As Object, ByVal e As EventArgs) Handles BsMinusLabel.MouseEnter, BsPlusLabel.MouseEnter
        Try
            If BsLEDCurrentTrackBar.Enabled Then
                Dim myLabel As Label = CType(sender, Label)
                If myLabel IsNot Nothing Then
                    Select Case myLabel.Text
                        Case "+"
                            BsPlusLabel.ForeColor = Color.White
                            BsPlusLabel.BackColor = Color.Silver
                        Case "-"
                            BsMinusLabel.ForeColor = Color.White
                            BsMinusLabel.BackColor = Color.Silver
                    End Select
                End If
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".BsPlusMinusLabels_MouseEnter ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".BsPlusMinusLabels_MouseEnter ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub BsPlusMinusLabels_MouseLeave(ByVal sender As Object, ByVal e As EventArgs) Handles BsMinusLabel.MouseLeave, BsPlusLabel.MouseLeave
        Try
            If BsLEDCurrentTrackBar.Enabled Then
                Dim myLabel As Label = CType(sender, Label)
                If myLabel IsNot Nothing Then
                    Select Case myLabel.Text
                        Case "+"
                            BsPlusLabel.ForeColor = Color.DimGray
                            BsPlusLabel.BackColor = Color.Transparent
                        Case "-"
                            BsMinusLabel.ForeColor = Color.DimGray
                            BsMinusLabel.BackColor = Color.Transparent
                    End Select
                End If
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".BsPlusMinusLabels_MouseLeave ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".BsPlusMinusLabels_MouseLeave ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub BsMinusLabel_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BsMinusLabel.Click
        Try
            If BsLEDCurrentTrackBar.Enabled And BsLEDCurrentTrackBar.Value > 0 Then
                If BsLEDCurrentTrackBar.Value - BsLEDCurrentTrackBar.LargeChange >= BsLEDCurrentTrackBar.Minimum Then
                    BsLEDCurrentTrackBar.Value -= BsLEDCurrentTrackBar.LargeChange
                Else
                    BsLEDCurrentTrackBar.Value = BsLEDCurrentTrackBar.Minimum
                End If
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".BsMinusLabel_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".BsMinusLabel_Click ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub BsPlusLabel_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BsPlusLabel.Click
        Try
            If BsLEDCurrentTrackBar.Enabled And BsLEDCurrentTrackBar.Value > 0 Then
                If BsLEDCurrentTrackBar.Value + BsLEDCurrentTrackBar.LargeChange <= BsLEDCurrentTrackBar.Maximum Then
                    BsLEDCurrentTrackBar.Value += BsLEDCurrentTrackBar.LargeChange
                Else
                    BsLEDCurrentTrackBar.Value = BsLEDCurrentTrackBar.Maximum
                End If
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".BsPlusLabel_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".BsPlusLabel_Click ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub
#End Region

#End Region

#Region "New functionallity for Ba200"
    Private Sub HideTabsForBa200Model()

        If (AnalyzerController.Instance.IsBA200()) Then
            BsTabArmsControl.TabPages.Remove(TabReagent1)
            BsTabArmsControl.TabPages.Remove(TabReagent2)
            BsTabArmsControl.TabPages.Remove(TabMixer2)
        End If
    End Sub


#End Region


    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

    End Sub
End Class
