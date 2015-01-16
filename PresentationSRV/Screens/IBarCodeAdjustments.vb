Option Explicit On
Option Strict On

Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Global.GlobalEnumerates
Imports Biosystems.Ax00.FwScriptsManagement
Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.Controls.UserControls

Public Class IBarCodeAdjustments
    Inherits PesentationLayer.BSAdjustmentBaseForm

#Region "Declarations"
    'Screen's business delegate
    Private WithEvents myScreenDelegate As BarCodeAdjustmentDelegate

    ' Language
    Private currentLanguage As String

    Private ActiveAnalyzerModel As String

    Private SelectedAdjustmentsDS As New SRVAdjustmentsDS
    Private TemporalAdjustmentsDS As New SRVAdjustmentsDS
    Private TempToSendAdjustmentsDelegate As FwAdjustmentsDelegate
    ' Edited value
    Private EditedValue As EditedValueStruct
#End Region

#Region "Variables"
    Private PageInitialized As Boolean
    Private ChangedValue As Boolean
    Private WaitForScriptsExitingScreen As Boolean
#End Region

#Region "Properties"
    Private IsReadyToCloseAttr As Boolean
    Public ReadOnly Property IsReadyToClose() As Boolean
        Get
            Return IsReadyToCloseAttr
        End Get
    End Property

#End Region

#Region "Structures"
    Private Structure EditedValueStruct
        ' Identifier
        Public AdjustmentID As ADJUSTMENT_GROUPS
        ' Maximum allowed value 
        Public LimitMaxValue As Single
        ' Minimum allowed value 
        Public LimitMinValue As Single
        ' Last value
        Public LastValue As Single
        ' Current value 
        Public CurrentValue As Single
        ' New value
        Public NewValue As Single
        'steps
        Public stepValue As Single
    End Structure
#End Region

#Region "Constructor"
    Sub New()
        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        DefineScreenLayout()
    End Sub
#End Region

#Region "Event Handlers"
    ''' <summary>
    ''' Executes ManageReceptionEvent() method in the main thread
    ''' </summary>
    ''' <param name="pResponse"></param>
    ''' <param name="pData"></param>
    ''' <remarks>Created by XBC 15/12/2011</remarks>
    Public Sub ScreenReceptionLastFwScriptEvent(ByVal pResponse As RESPONSE_TYPES, ByVal pData As Object) Handles myScreenDelegate.ReceivedLastFwScriptEvent

        Me.UIThread(Function() ManageReceptionEvent(pResponse, pData))

    End Sub

    ''' <summary>
    ''' manages the response of the Analyzer after sending a FwScript List
    ''' The response can be OK, NG, Timeout or Exception
    ''' </summary>
    ''' <param name="pResponse">response type</param>
    ''' <param name="pData">data received</param>
    ''' <remarks>Created by XBC 15/12/2011</remarks>
    Private Function ManageReceptionEvent(ByVal pResponse As RESPONSE_TYPES, ByVal pData As Object) As Boolean
        Dim myGlobal As New GlobalDataTO
        Try
            'manage special operations according to the screen characteristics

            'timeout limit repetitions
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
            ' timeout limit repetitions

            'if needed manage the event in the Base Form
            MyBase.OnReceptionLastFwScriptEvent(pResponse, pData)


            Select Case MyBase.CurrentMode
                Case ADJUSTMENT_MODES.ADJUSTMENTS_READED
                    If pResponse = RESPONSE_TYPES.OK Then

                        MyBase.DisplayMessage(Messages.SRV_ADJUSTMENTS_READED.ToString)

                        Application.DoEvents()

                        myGlobal = InitializeHomes()
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


                Case ADJUSTMENT_MODES.ADJUST_PREPARED
                    If pResponse = RESPONSE_TYPES.OK Then
                        If MyBase.SimulationMode Then

                            MyBase.myServiceMDI.Focus()

                            Me.Cursor = Cursors.WaitCursor
                            System.Threading.Thread.Sleep(SimulationProcessTime)
                            MyBase.myServiceMDI.Focus()
                            Me.Cursor = Cursors.Default

                            MyBase.myServiceMDI.Focus()

                        Else
                            ' homes are done for current adjust
                            myGlobal = myScreenDelegate.SetPreliminaryHomesAsDone(Me.EditedValue.AdjustmentID)
                            If myGlobal.HasError Then
                                PrepareErrorMode()
                                Exit Function
                            End If

                        End If

                        MyBase.DisplayMessage(Messages.SRV_HOMES_FINISHED.ToString, Messages.SRV_ADJUSTMENTS_READY.ToString)

                        PrepareArea()
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
                        MyBase.DisplayMessage(Messages.SRV_ADJUSTMENTS_SAVED.ToString)
                        PrepareArea()
                    End If

                Case ADJUSTMENT_MODES.TESTED
                    If pResponse = RESPONSE_TYPES.START Then

                        If myScreenDelegate.CurrentOperation = BarCodeAdjustmentDelegate.OPERATIONS.TEST Then
                            If myScreenDelegate.HomesDone Then
                                ' Configuring Progress bar & timer associated
                                Me.ProgressBar1.Maximum = myScreenDelegate.CurrentTimeOperation
                                Me.ProgressBar1.Value = 0
                                Me.ProgressBar1.Visible = True
                                MyClass.TestProcessTimer.Interval = 1000 ' 1 second
                                MyClass.TestProcessTimer.Enabled = True
                                Me.Cursor = Cursors.WaitCursor
                            End If
                        End If

                    End If
                    If pResponse = RESPONSE_TYPES.OK Then
                        myGlobal = myScreenDelegate.SetPreliminaryHomesAsDone(Me.EditedValue.AdjustmentID)
                        If Not myGlobal.HasError Then
                            PrepareArea()
                        Else
                            PrepareErrorMode()
                            Exit Function
                        End If
                    End If

                    ' XBC 17-04-2012
                Case ADJUSTMENT_MODES.TEST_EXITED
                    If pResponse = RESPONSE_TYPES.OK Then
                        PrepareArea()
                    End If


                Case ADJUSTMENT_MODES.ERROR_MODE
                    MyBase.DisplayMessage(Messages.FWSCRIPT_DATA_ERROR.ToString)
                    PrepareErrorMode()
            End Select


        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".ScreenReceptionLastFwScriptEvent ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
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
    ''' <remarks>Created by XBC 16/12/2011</remarks>
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
    ''' <remarks>
    ''' Created by SGM 19/10/2012
    ''' Updated by XBC 23/10/2012 - Add business logic with every current operation
    ''' </remarks>
    Public Overrides Sub StopCurrentOperation(Optional ByVal pAlarmType As ManagementAlarmTypes = ManagementAlarmTypes.NONE)
        Try
            ' Stop FwScripts
            myFwScriptDelegate.StopFwScriptQueue()

            Select Case MyBase.CurrentMode

                Case ADJUSTMENT_MODES.ADJUSTMENTS_READING
                    ' No additional treatment to do

                Case ADJUSTMENT_MODES.ADJUSTING
                    ' No additional treatment to do

                Case ADJUSTMENT_MODES.SAVING
                    ' No additional treatment to do

                Case ADJUSTMENT_MODES.TESTING

                    Select Case myScreenDelegate.CurrentOperation

                        Case BarCodeAdjustmentDelegate.OPERATIONS.HOMES
                            ' No additional treatment to do

                        Case BarCodeAdjustmentDelegate.OPERATIONS.TEST
                            ' No additional treatment to do

                        Case BarCodeAdjustmentDelegate.OPERATIONS.TEST_MODE

                            StopReading()

                        Case BarCodeAdjustmentDelegate.OPERATIONS.TEST_MODE_END
                            ' No additional treatment to do

                    End Select

            End Select

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
    ''' Generic function to send FwScripts to the Instrument through own delegates
    ''' </summary>
    ''' <param name="pMode"></param>
    ''' <param name="pAdjustmentGroup"></param>
    ''' <remarks>XBC 15/12/2011</remarks>
    Private Sub SendFwScript(ByVal pMode As ADJUSTMENT_MODES, _
                             Optional ByVal pAdjustmentGroup As ADJUSTMENT_GROUPS = Nothing)
        Dim myGlobal As New GlobalDataTO
        Try
            If Not myGlobal.HasError AndAlso MyBase.myServiceMDI.MDIAnalyzerManager.Connected Then
                myScreenDelegate.NoneInstructionToSend = True
                myGlobal = myScreenDelegate.SendFwScriptsQueueList(pMode, pAdjustmentGroup)
                If Not myGlobal.HasError Then
                    If myScreenDelegate.NoneInstructionToSend Then
                        ' Send FwScripts
                        myGlobal = myFwScriptDelegate.StartFwScriptQueue
                        Me.Cursor = Cursors.WaitCursor
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
                MyBase.CreateLogActivity(myGlobal.ErrorCode, Me.Name & ".SendFwScript ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
                MyBase.ShowMessage(Me.Name & ".SendFwScript ", myGlobal.ErrorCode, myGlobal.ErrorMessage, Me)
            End If

        Catch ex As Exception
            Me.Cursor = Cursors.Default
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".SendFwScript ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".SendFwScript ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Assigns the specific screen's elements to the common screen layout structure
    ''' </summary>
    ''' <remarks>XBC 14/12/2011</remarks>
    Private Sub DefineScreenLayout()
        Try
            With MyBase.myScreenLayout

                .ButtonsPanel.SaveButton = Me.SaveButton
                .ButtonsPanel.CancelButton = Me.ButtonCancel
                .ButtonsPanel.ExitButton = Me.BsExitButton

                .MessagesPanel.Container = Me.BsMessagesPanel
                .MessagesPanel.Icon = Me.BsMessageImage
                .MessagesPanel.Label = Me.BsMessageLabel

                .ButtonsPanel.AdjustButton = Me.AdjustButton
                .ButtonsPanel.TestButton = Nothing
                .AdjustmentPanel.AdjustPanel.Container = Me.BsAdjustPanel
                .AdjustmentPanel.AdjustPanel.AdjustAreas = MyBase.GetAdjustAreas(Me.BsAdjustPanel)

            End With

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".DefineScreenLayout ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".DefineScreenLayout ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub Initializations()
        Dim myGlobal As New GlobalDataTO
        Try
            Me.PageInitialized = True
            Me.ChangedValue = False

            ' Instantiate a new EditionValue structure
            Me.EditedValue = New EditedValueStruct
            With Me.EditedValue
                .AdjustmentID = ADJUSTMENT_GROUPS.SAMPLES_ROTOR_BC
                myScreenDelegate.AdjustmentID = ADJUSTMENT_GROUPS.SAMPLES_ROTOR_BC
            End With

            myGlobal = GetParameters()
            If myGlobal.HasError Then
                PrepareErrorMode()
                Exit Try
            End If

            myGlobal = PrepareScreen()
            If myGlobal.HasError Then
                PrepareErrorMode()
            End If

            If MyBase.CurrentUserNumericalLevel = USER_LEVEL.lOPERATOR Then
                Me.AdjustButton.Visible = False
                Me.SaveButton.Visible = False
                Me.ButtonCancel.Visible = False
            End If

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".Initializations ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".Initializations ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Get texts in the current application language for all screen controls
    ''' </summary>
    ''' <remarks>
    ''' Created by: XBC 14/12/2011
    ''' </remarks>
    Private Sub GetScreenLabels()
        Try
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            Me.BsTitleLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "SRV_MENU_BarCode", currentLanguage) 'dl 30/11/2012 SRV_MENU_BarCode
            Me.BsInfoTitle.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_INFO_TITLE", currentLanguage)
            Me.BsSubtitleLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_TITLE_BARCODE", currentLanguage)
            Me.SelectRotorGroupBox.Text = "1. " + myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_SELECT_ROTOR", currentLanguage)
            Me.SampleRotorRadioButton.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Samples", currentLanguage)
            Me.ReagentRotorRadioButton.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Reagents", currentLanguage)
            Me.CenterRotorGroupBox.Text = "2. " + myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_RotorCentering", currentLanguage)
            Me.ReadingBCGroupBox.Text = "3. " + myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_ReadingTest", currentLanguage)
            Me.TestGroupBox.Text = "4. " + myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_Test", currentLanguage)
            Me.BsAdjust.RangeTitle = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_RANGE", currentLanguage)

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
    ''' Created by: XBC 14/12/2011
    ''' </remarks>
    Private Sub GetScreenTooltip()
        Try
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            ' For Tooltips...

            MyBase.bsScreenToolTipsControl.SetToolTip(AdjustButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_ADJUST", currentLanguage)) 'JB 01/10/2012 - Resource String unification
            MyBase.bsScreenToolTipsControl.SetToolTip(SaveButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Save", currentLanguage))
            MyBase.bsScreenToolTipsControl.SetToolTip(ButtonCancel, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Cancel", currentLanguage))

            MyBase.bsScreenToolTipsControl.SetToolTip(StartReadingButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "SRV_BTN_TestStart", currentLanguage))
            MyBase.bsScreenToolTipsControl.SetToolTip(StopButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "SRV_BTN_TestStop", currentLanguage))

            MyBase.bsScreenToolTipsControl.SetToolTip(TestButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "SRV_BTN_Test", currentLanguage))

            MyBase.bsScreenToolTipsControl.SetToolTip(BsExitButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_CloseScreen", currentLanguage))

        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".GetScreenTooltip ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".GetScreenTooltip ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Loads the icons and tooltips used for painting the buttons
    ''' </summary>
    ''' <remarks>Created by: XBC 14/12/2011</remarks>
    Private Sub PrepareButtons()
        'Dim auxIconName As String = ""
        'Dim iconPath As String = MyBase.IconsPath
        'Dim myUtil As New Utilities
        Try

            MyBase.SetButtonImage(AdjustButton, "ADJUSTMENT")
            MyBase.SetButtonImage(SaveButton, "SAVE")
            MyBase.SetButtonImage(ButtonCancel, "UNDO")
            MyBase.SetButtonImage(StartReadingButton, "ADJUSTMENT")
            MyBase.SetButtonImage(StopButton, "STOP", 24, 24)
            MyBase.SetButtonImage(TestButton, "ADJUSTMENT")
            MyBase.SetButtonImage(BsExitButton, "CANCEL")


            ''ADJUST Button
            'auxIconName = GetIconName("ADJUSTMENT")
            'If System.IO.File.Exists(iconPath & auxIconName) Then
            '    Dim myImage As Image = Image.FromFile(iconPath & auxIconName)
            '    myImage = CType(myUtil.ResizeImage(myImage, New Size(28, 28)).SetDatos, Image)
            '    AdjustButton.Image = myImage
            '    AdjustButton.ImageAlign = ContentAlignment.MiddleCenter
            'End If

            ''SAVE Button
            'auxIconName = GetIconName("SAVE")
            'If System.IO.File.Exists(iconPath & auxIconName) Then
            '    Dim myImage As Image = Image.FromFile(iconPath & auxIconName)
            '    myImage = CType(myUtil.ResizeImage(myImage, New Size(28, 28)).SetDatos, Image)
            '    SaveButton.Image = myImage
            '    SaveButton.ImageAlign = ContentAlignment.MiddleCenter
            'End If

            ''CANCEL Button
            'auxIconName = GetIconName("UNDO") 'CANCEL
            'If System.IO.File.Exists(iconPath & auxIconName) Then
            '    Dim myImage As Image = Image.FromFile(iconPath & auxIconName)
            '    myImage = CType(myUtil.ResizeImage(myImage, New Size(28, 28)).SetDatos, Image)
            '    CancelButton.Image = myImage
            '    CancelButton.ImageAlign = ContentAlignment.MiddleCenter
            'End If

            ''START READING Button
            'auxIconName = GetIconName("ADJUSTMENT")
            'If System.IO.File.Exists(iconPath & auxIconName) Then
            '    Dim myImage As Image = Image.FromFile(iconPath & auxIconName)
            '    myImage = CType(myUtil.ResizeImage(myImage, New Size(28, 28)).SetDatos, Image)
            '    StartReadingButton.Image = myImage
            '    StartReadingButton.ImageAlign = ContentAlignment.MiddleCenter
            'End If

            ''STOP READING Button
            'auxIconName = GetIconName("STOP")
            'If System.IO.File.Exists(iconPath & auxIconName) Then
            '    Dim myImage As Image = Image.FromFile(iconPath & auxIconName)
            '    myImage = CType(myUtil.ResizeImage(myImage, New Size(24, 24)).SetDatos, Image)
            '    StopButton.Image = myImage
            '    StopButton.ImageAlign = ContentAlignment.MiddleCenter
            'End If

            ''TEST Button
            'auxIconName = GetIconName("ADJUSTMENT")
            'If System.IO.File.Exists(iconPath & auxIconName) Then
            '    Dim myImage As Image = Image.FromFile(iconPath & auxIconName)
            '    myImage = CType(myUtil.ResizeImage(myImage, New Size(28, 28)).SetDatos, Image)
            '    TestButton.Image = myImage
            '    TestButton.ImageAlign = ContentAlignment.MiddleCenter
            'End If

            ''EXIT Button
            'auxIconName = GetIconName("CANCEL")
            'If System.IO.File.Exists(iconPath & auxIconName) Then
            '    Dim myImage As Image = Image.FromFile(iconPath & auxIconName)
            '    myImage = CType(myUtil.ResizeImage(myImage, New Size(28, 28)).SetDatos, Image)
            '    BsExitButton.Image = myImage
            '    BsExitButton.ImageAlign = ContentAlignment.MiddleCenter
            'End If

        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".PrepareButtons", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PrepareButtons", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub ExitScreen()
        'Dim myGlobal As New GlobalDataTO
        Dim dialogResultToReturn As DialogResult = Windows.Forms.DialogResult.No
        Try
            If MyBase.CurrentMode = ADJUSTMENT_MODES.ERROR_MODE Then
                MyClass.IsReadyToCloseAttr = True
                Me.Close()
                Exit Sub
            End If

            Me.WaitForScriptsExitingScreen = False
            If ChangedValue Then
                dialogResultToReturn = MyBase.ShowMessage(GetMessageText(Messages.SAVE_PENDING.ToString), Messages.SAVE_PENDING.ToString)

                If dialogResultToReturn = Windows.Forms.DialogResult.Yes Then
                    'dialogResultToReturn = Windows.Forms.DialogResult.No
                    Me.SaveAdjustment()
                    Me.WaitForScriptsExitingScreen = True
                Else
                    Me.CancelAdjustment()
                End If
            End If

            If Not Me.WaitForScriptsExitingScreen Then
                ' XBC 17-04-2012
                'Me.FinishExitScreen()
                Me.PreviousFinishExitScreen()
                ' XBC 17-04-2012
            End If

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".ExitScreen", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".ExitScreen", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' previous operation to close screen
    ''' </summary>
    ''' <remarks>Created by XBC 17/04/2012</remarks>
    Private Sub PreviousFinishExitScreen()
        'Dim myGlobal As New GlobalDataTO
        Try
            MyBase.ExitTest()

            Me.DisableAll()

            If MyBase.SimulationMode Then
                ' simulating
                Me.Cursor = Cursors.WaitCursor
                System.Threading.Thread.Sleep(SimulationProcessTime)
                MyBase.myServiceMDI.Focus()
                Me.Cursor = Cursors.Default
                MyBase.CurrentMode = ADJUSTMENT_MODES.TEST_EXITED
                PrepareArea()
            Else
                ' Manage FwScripts must to be sent to adjusting
                Me.SendFwScript(Me.CurrentMode, EditedValue.AdjustmentID)
            End If

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".PreviousFinishExitScreen", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PreviousFinishExitScreen", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Final operation to close screen
    ''' </summary>
    ''' <remarks>Created by XBC 15/12/2011</remarks>
    Private Sub FinishExitScreen()
        Dim myGlobal As New GlobalDataTO
        Try
            myGlobal = MyBase.CloseForm()
            If myGlobal.HasError Then
                Me.PrepareErrorMode()
            Else
                Me.PrepareArea()
                MyClass.IsReadyToCloseAttr = True
                Me.Close()

                'SGM 22/05/2012
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

            End If

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".FinishExitScreen", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".FinishExitScreen", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Set all the screen elements to disabled
    ''' </summary>
    ''' <remarks>Created by: XBC 14/12/2011</remarks>
    Private Sub DisableAll()
        Try
            Me.SelectRotorGroupBox.Enabled = False
            Me.CenterRotorGroupBox.Enabled = False

            'Me.ReadingBCGroupBox.Enabled = False
            Me.StartReadingButton.Enabled = False

            Me.TestGroupBox.Enabled = False

            ' Disable Area Buttons
            Me.BsExitButton.Enabled = False

            MyBase.ActivateMDIMenusButtons(False)

        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".DisableAll ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".DisableAll ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' reset all homes
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>XBC 14/12/2011</remarks>
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

    ''' <summary>
    ''' Get Limits values from BD for every different thermo
    ''' </summary>
    ''' <remarks>Created by XBC 14/12/2011</remarks>
    Private Function GetLimitValues() As GlobalDataTO
        Dim myGlobal As New GlobalDataTO
        Try
            ' Get Value limit ranges
            Dim myFieldLimitsDelegate As New FieldLimitsDelegate()
            Dim myFieldLimitsDS As New FieldLimitsDS

            With Me.EditedValue
                'Load the specified limits values
                Select Case .AdjustmentID
                    Case ADJUSTMENT_GROUPS.SAMPLES_ROTOR_BC
                        myGlobal = myFieldLimitsDelegate.GetList(Nothing, FieldLimitsEnum.SRV_SAMPLE_ROTOR_LIMIT)
                    Case ADJUSTMENT_GROUPS.REAGENTS_ROTOR_BC
                        myGlobal = myFieldLimitsDelegate.GetList(Nothing, FieldLimitsEnum.SRV_REAGENT_ROTOR_LIMIT)
                End Select

                If (Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing) Then
                    myFieldLimitsDS = CType(myGlobal.SetDatos, FieldLimitsDS)
                    If myFieldLimitsDS.tfmwFieldLimits.Rows.Count > 0 Then
                        .LimitMinValue = CType(myFieldLimitsDS.tfmwFieldLimits(0).MinValue, Decimal)
                        .LimitMaxValue = CType(myFieldLimitsDS.tfmwFieldLimits(0).MaxValue, Decimal)

                        Me.BsAdjust.MinimumLimit = .LimitMinValue
                        Me.BsAdjust.MaximumLimit = .LimitMaxValue
                        Me.BsAdjust.MaxNumDecimals = 0
                        Me.BsAdjust.CurrentStepValue = 1
                    End If
                End If

            End With

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".GetLimitValues ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".GetLimitValues ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
        Return myGlobal
    End Function

    ''' <summary>
    ''' Get Parameters values from BD for Thermos Tests
    ''' </summary>
    ''' <remarks>Created by XBC 14/12/2011</remarks>
    Private Function GetParameters() As GlobalDataTO
        Dim myGlobal As New GlobalDataTO
        Try
            Me.ActiveAnalyzerModel = MyBase.myServiceMDI.ActiveAnalyzerModel
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
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".GetParameters ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".GetParameters ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
        Return myGlobal
    End Function

    ''' <summary>
    ''' Initializes all the screen adjustment controls
    ''' </summary>
    ''' <remarks>XBC 15/12/2011</remarks>
    Private Sub InitializeAdjustControls()
        Try
            With Me.BsAdjust
                .HomingEnabled = True
                .EditingEnabled = True
                .UnitsCaption = "steps"
            End With

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".InitializeAdjustControls ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".InitializeAdjustControls ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Prepare Tab Area according with current operations
    ''' </summary>
    ''' <remarks>Created by: XBC 14/12/2011</remarks>
    Private Sub PrepareArea()
        Try
            Application.DoEvents()

            ' Enabling/desabling form components to this child screen
            Select Case MyBase.CurrentMode

                Case ADJUSTMENT_MODES.ADJUSTMENTS_READING
                    MyBase.DisplayMessage(Messages.SRV_READ_ADJUSTMENTS.ToString)

                Case ADJUSTMENT_MODES.ADJUSTMENTS_READED
                    MyBase.DisplayMessage(Messages.SRV_ADJUSTMENTS_READED.ToString)

                    MyClass.LoadAdjustmentsData()
                    MyClass.LoadAdjustmentGroupData()
                    MyClass.PopulateEditionValues()

                    MyClass.PrepareLoadingMode()

                Case ADJUSTMENT_MODES.ADJUST_PREPARING
                    Me.PrepareAdjustPreparingMode()

                Case ADJUSTMENT_MODES.ADJUST_PREPARED
                    Me.PrepareAdjustPreparedMode()

                Case ADJUSTMENT_MODES.ADJUSTING
                    Me.PrepareAdjustingMode()

                Case ADJUSTMENT_MODES.ADJUSTED
                    Me.PrepareAdjustedMode()

                Case ADJUSTMENT_MODES.LOADED
                    Me.PrepareLoadedMode()

                Case ADJUSTMENT_MODES.SAVING
                    Me.PrepareSavingMode()

                Case ADJUSTMENT_MODES.SAVED
                    Me.PrepareSavedMode()

                Case ADJUSTMENT_MODES.TESTING
                    Me.PrepareTestingMode()

                Case ADJUSTMENT_MODES.TESTED
                    Me.PrepareTestedMode()

                    ' XBC 17-04-2012
                Case ADJUSTMENT_MODES.TEST_EXITED
                    Me.FinishExitScreen()

                Case ADJUSTMENT_MODES.ERROR_MODE
                    PrepareErrorMode()
            End Select

            If MyBase.myServiceMDI IsNot Nothing Then
                If Not MyBase.SimulationMode And MyBase.myServiceMDI.MDIAnalyzerManager.AnalyzerStatus = AnalyzerManagerStatus.SLEEPING Then
                    MyClass.PrepareErrorMode()
                    MyBase.DisplayMessage("")
                End If
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".PrepareArea ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PrepareArea ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Prepare GUI for Loading Mode
    ''' </summary>
    ''' <remarks>Created by XBC 30/03/2012</remarks>
    Private Sub PrepareLoadingMode()
        Try
            MyBase.Initialize()

            Me.DisableAll()

            ' Initializations

            If MyBase.SimulationMode Then
                ' simulating
                Me.Cursor = Cursors.WaitCursor
                System.Threading.Thread.Sleep(SimulationProcessTime)
                MyBase.myServiceMDI.Focus()
                Me.Cursor = Cursors.Default
                MyBase.CurrentMode = ADJUSTMENT_MODES.LOADED
                PrepareArea()
            Else
                ' Manage FwScripts must to be sent to adjusting
                Me.SendFwScript(Me.CurrentMode, EditedValue.AdjustmentID)
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".PrepareLoadingMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PrepareLoadingMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Prepare GUI for Adjustments Reading Mode
    ''' </summary>
    ''' <remarks>Created by XBC 14/12/2011</remarks>
    Private Sub PrepareAdjustReadingMode()
        'Dim myResultData As New GlobalDataTO
        Dim myGlobal As New GlobalDataTO
        Try
            DisableAll()

            ' Reading Adjustments
            If Not MyClass.myServiceMDI.AdjustmentsReaded Then
                ' Parent Reading Adjustments
                MyBase.ReadAdjustments()
                PrepareArea()

                ' Manage FwScripts must to be sent at load screen
                If MyBase.SimulationMode Then
                    ' simulating...
                    Me.Cursor = Cursors.WaitCursor
                    System.Threading.Thread.Sleep(SimulationProcessTime)
                    MyBase.myServiceMDI.Focus()
                    Me.Cursor = Cursors.Default
                    MyClass.CurrentMode = ADJUSTMENT_MODES.ADJUSTMENTS_READED
                    PrepareArea()
                Else
                    If Not myGlobal.HasError AndAlso MyBase.myServiceMDI.MDIAnalyzerManager.Connected Then
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
            CreateLogActivity(ex.Message, Me.Name & ".PrepareAdjustReadingMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PrepareAdjustReadingMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Prepare GUI for Loaded Mode
    ''' </summary>
    ''' <remarks>Created by XBC 14/12/2011</remarks>
    Private Sub PrepareLoadedMode()
        'Dim myResultData As New GlobalDataTO
        Try
            Me.InitializeAdjustControls()

            ' ROTOR SELECTION AREA
            Me.SelectRotorGroupBox.Enabled = True

            ' ROTOR CENTERING AREA
            Me.CenterRotorGroupBox.Enabled = True
            Me.BsAdjust.Enabled = False
            Me.AdjustButton.Enabled = True
            Me.SaveButton.Enabled = False
            Me.ButtonCancel.Enabled = False

            ' READING AREA
            'Me.ReadingBCGroupBox.Enabled = True
            Me.StartReadingButton.Enabled = True

            ' TEST AREA
            Me.TestGroupBox.Enabled = True

            ' BUTTONS AREA
            Me.BsExitButton.Enabled = True

            ' OTHERS
            MyBase.ActivateMDIMenusButtons(True)
            Me.ProgressBar1.Visible = False

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareLoadedMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareLoadedMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        Finally
            Me.Cursor = Cursors.Default
        End Try
    End Sub

    ''' <summary>
    ''' Prepare GUI for Adjust Preparing Mode
    ''' </summary>
    ''' <remarks>Created by XBC 15/12/2011</remarks>
    Private Sub PrepareAdjustPreparingMode()
        Try
            DisableAll()

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareAdjustPreparingMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareAdjustPreparingMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Prepare GUI for Adjust Prepared Mode
    ''' </summary>
    ''' <remarks>Created by XBC 15/12/2011</remarks>
    Private Sub PrepareAdjustPreparedMode()
        'Dim myGlobal As New GlobalDataTO
        Try
            LoadAdjustmentGroupData()
            Me.CenterRotorGroupBox.Enabled = True

            With Me.EditedValue
                .CurrentValue = .LastValue
                .NewValue = .LastValue
                Me.BsAdjust.CurrentValue = .CurrentValue
                Me.BsAdjust.Enabled = True
            End With
            MyBase.SetAdjustmentItems(Me.BsAdjust)

            Me.BsAdjust.Focus()

            MyBase.ActivateMDIMenusButtons(True)
            Me.AdjustButton.Enabled = False
            Me.ButtonCancel.Enabled = True
            MyBase.DisplayMessage(Messages.SRV_ADJUSTMENTS_READY.ToString)

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareAdjustPreparedMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareAdjustPreparedMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        Finally
            Me.Cursor = Cursors.Default
        End Try
    End Sub

    ''' <summary>
    ''' Prepare GUI for Adjusting Mode
    ''' </summary>
    ''' <remarks>Created by XBC 15/12/2011</remarks>
    Private Sub PrepareAdjustingMode()
        Try
            MyBase.ActivateMDIMenusButtons(False)

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareAdjustingMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareAdjustingMode", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Prepare GUI for Adjusted Mode
    ''' </summary>
    ''' <remarks>Created by XBC 15/12/2011</remarks>
    Private Sub PrepareAdjustedMode()
        Try
            Me.CenterRotorGroupBox.Enabled = True

            ' Updating EditionValue Structure as well as each position Arm selected
            With Me.EditedValue
                .CurrentValue = .NewValue
                Me.BsAdjust.CurrentValue = .NewValue
            End With

            Me.BsAdjust.Enabled = True
            Me.BsAdjust.EscapeRequest()

            Me.ChangedValue = True

            Me.SaveButton.Enabled = True
            Me.ButtonCancel.Enabled = True
            Me.BsExitButton.Enabled = True

            Me.Enabled = True
            MyBase.ActivateMDIMenusButtons(True)

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareAdjustedMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareAdjustedMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        Finally
            Me.Cursor = Cursors.Default
        End Try
    End Sub

    ''' <summary>
    ''' Prepare GUI for Saving Mode
    ''' </summary>
    ''' <remarks>Created by XBC 15/12/2011</remarks>
    Private Sub PrepareSavingMode()
        Try
            Me.Cursor = Cursors.WaitCursor
            Me.DisableAll()

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareSavingMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareSavingMode", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Prepare GUI for Saved Mode
    ''' </summary>
    ''' <remarks>Created by XBC 15/12/2011</remarks>
    Private Sub PrepareSavedMode()
        Dim myGlobal As New GlobalDataTO
        'Dim myResultData As New GlobalDataTO
        Try
            If myScreenDelegate.LoadAdjDone Then
                ' Adjustments already saved into the Instrument !

                With Me.EditedValue
                    .LastValue = .NewValue
                    Me.BsAdjust.Text = .NewValue.ToString
                    Me.UpdateSpecificAdjustmentsDS(ReadSpecificAdjustmentData(GlobalEnumerates.AXIS.ROTOR).CodeFw, .NewValue.ToString)
                    .NewValue = 0
                End With

                myGlobal = MyBase.UpdateAdjustments(Me.SelectedAdjustmentsDS)

                If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then

                    Me.ChangedValue = False

                    Me.SelectedAdjustmentsDS.Clear()

                    If Me.WaitForScriptsExitingScreen Then
                        ' XBC 17-04-2012
                        'Me.FinishExitScreen()
                        Me.PreviousFinishExitScreen()
                        ' XBC 17-04-2012
                    Else
                        Me.PrepareLoadedMode()
                    End If

                Else
                    PrepareErrorMode()
                End If

            End If

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareSavedMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareSavedMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        Finally
            Me.Cursor = Cursors.Default
        End Try
    End Sub

    ''' <summary>
    ''' Prepare GUI for Testing Mode
    ''' </summary>
    ''' <remarks>Created by XBC 15/12/2011</remarks>
    Private Sub PrepareTestingMode()
        Try
            Me.Cursor = Cursors.WaitCursor

            Me.DisableAll()

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareTestingMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareTestingMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Prepare GUI for Tested Mode
    ''' </summary>
    ''' <remarks>Created by XBC 15/12/2011</remarks>
    Private Sub PrepareTestedMode()
        'Dim myGlobal As New GlobalDataTO
        Try
            Select Case myScreenDelegate.CurrentOperation
                Case BarCodeAdjustmentDelegate.OPERATIONS.HOMES

                    If myScreenDelegate.HomesDone Then
                        Me.ScanningBarCode()
                    End If

                Case BarCodeAdjustmentDelegate.OPERATIONS.TEST

                    If myScreenDelegate.TestDone Then

                        ' Stop Progress bar & Timer
                        Me.ProgressBar1.Value = 0
                        Me.ProgressBar1.Visible = False
                        Me.ProgressBar1.Refresh()
                        MyClass.TestProcessTimer.Enabled = False

                        ' refresh info into grid
                        If myScreenDelegate.BcResultsCount > 0 Then
                            Me.BarCodeDataGridView.Rows.Clear()

                            For i As Integer = 0 To myScreenDelegate.BcResultsCount - 1
                                Me.BarCodeDataGridView.Rows.Add()
                                Me.BarCodeDataGridView.Rows(i).Cells(0).Value = myScreenDelegate.BcResults(i).Position
                                Me.BarCodeDataGridView.Rows(i).Cells(1).Value = myScreenDelegate.BcResults(i).Value
                            Next

                        End If

                        MyBase.DisplayMessage(Messages.SRV_TEST_COMPLETED.ToString)

                        Me.PrepareLoadedMode()

                        'ElseIf myScreenDelegate.HomesDone Then

                        '    Me.ScanningBarCode()

                    End If

                Case BarCodeAdjustmentDelegate.OPERATIONS.TEST_MODE

                    If myScreenDelegate.TestModeDone Then
                        If myScreenDelegate.BcResultsCount > 0 Then
                            ' Remark : 'position' is the field used by Test Mode
                            Me.ReadedValueLabel.Text = myScreenDelegate.BcResults(0).Position.ToString + " %"
                        End If
                    End If

                    Me.DisableAll()
                    Me.StartReadingButton.Visible = False
                    Me.StopButton.Visible = True

                    MyBase.DisplayMessage(Messages.SRV_TEST_IN_PROCESS.ToString)

                Case BarCodeAdjustmentDelegate.OPERATIONS.TEST_MODE_END

                    If myScreenDelegate.TestModeDone Then
                        MyBase.DisplayMessage(Messages.SRV_TEST_COMPLETED.ToString)

                        Me.PrepareLoadedMode()

                        Me.StartReadingButton.Visible = True
                        Me.StopButton.Visible = False

                        MyBase.myServiceMDI.SEND_INFO_START()
                    End If

            End Select


        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareTestedMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareTestedMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        Finally
            Me.Cursor = Cursors.Default
        End Try
    End Sub

    ''' <summary>
    ''' Prepare GUI for Error Mode
    ''' </summary>
    ''' <remarks>Created by XBC 14/12/2011</remarks>
    Public Overrides Sub PrepareErrorMode(Optional ByVal pAlarmType As ManagementAlarmTypes = ManagementAlarmTypes.NONE)
        Try
            MyBase.ErrorMode()
            Me.ProgressBar1.Visible = False
            DisableAll()
            Me.ReadingBCGroupBox.Enabled = False
            Me.BsExitButton.Enabled = True ' Just Exit button is enabled in error case
        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".PrepareErrorMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PrepareErrorMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        Finally
            Me.Cursor = Cursors.Default
        End Try
    End Sub

    ''' <summary>
    ''' Initializations for Barcode screen
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>Created by XBC 19/12/2011</remarks>
    Private Function PrepareScreen() As GlobalDataTO
        Dim myResultData As New GlobalDataTO
        Try
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate
            Dim columnName As String

            Me.ReadedValueLabel.Text = ""

            Me.BarCodeDataGridView.Columns.Clear()
            Me.BarCodeDataGridView.Rows.Clear()

            ' Add Position column
            columnName = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_TUBE_POSITION", currentLanguage)
            Me.BarCodeDataGridView.Columns.Add(columnName, columnName)
            ' Add CodeBar column
            columnName = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_BARCODE", currentLanguage)
            Me.BarCodeDataGridView.Columns.Add(columnName, columnName)

        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".PrepareScreen ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PrepareScreen ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
        Return myResultData
    End Function

    Private Sub LoadAdjustmentsData()
        Try
            With myScreenDelegate
                .SamplesRotorBCPosition = ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.SAMPLES_ROTOR_BC.ToString, GlobalEnumerates.AXIS.ROTOR).Value
                .ReagentsRotorBCPosition = ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.REAGENTS_ROTOR_BC.ToString, GlobalEnumerates.AXIS.ROTOR).Value
            End With

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".LoadAdjustmentsData ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".LoadAdjustmentsData ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub PopulateEditionValues()
        Dim myUtilities As New Utilities
        Try
            Dim value As String
            With Me.EditedValue
                value = ReadSpecificAdjustmentData(GlobalEnumerates.AXIS.ROTOR).Value
                .LastValue = myUtilities.FormatToSingle(value)
                .CurrentValue = .LastValue
                .NewValue = .LastValue
            End With

            GetLimitValues()

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".PopulateEditionValues ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PopulateEditionValues ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Defines the needed parameters for the screen delegate in order to make an adjustment
    ''' </summary>
    ''' <param name="pMovement"></param>
    ''' <remarks>XBC 15/15/2011</remarks>
    Private Sub MakeAdjustment(ByVal pMovement As MOVEMENT)
        Dim myGlobal As New GlobalDataTO
        Try
            Dim AdjustToDo As ADJUSTMENT_GROUPS
            Dim MovToDo As MOVEMENT
            Dim valueToDo As Single

            myGlobal = MyBase.Adjust()
            If myGlobal.HasError Then
                PrepareErrorMode()
            Else
                PrepareArea()

                With EditedValue
                    AdjustToDo = .AdjustmentID
                    Select Case pMovement
                        Case MOVEMENT.ABSOLUTE
                            MovToDo = MOVEMENT.ABSOLUTE
                            valueToDo = .NewValue

                        Case MOVEMENT.RELATIVE
                            MovToDo = MOVEMENT.RELATIVE
                            valueToDo = .stepValue

                        Case MOVEMENT.HOME
                            MovToDo = MOVEMENT.HOME

                    End Select

                End With

                ' Populating parameters into own Delegate
                myScreenDelegate.pMovAdjust = MovToDo
                myScreenDelegate.pValueAdjust = valueToDo.ToString

                ' Manage FwScripts must to be sent to adjusting
                Me.SendFwScript(Me.CurrentMode, AdjustToDo)
            End If

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".MakeAdjustment ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".MakeAdjustment ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Saves changed values for the selected adjustment
    ''' </summary>
    ''' <remarks>XBC 15/12/2011</remarks>
    Private Sub SaveAdjustment()
        Dim myGlobal As New GlobalDataTO
        Try
            myGlobal = MyBase.Save()
            If myGlobal.HasError Then
                Me.PrepareErrorMode()
            Else
                ' Initializations
                Me.myScreenDelegate.LoadAdjDone = False

                Me.PrepareArea()

                If Me.ChangedValue Then

                    ' Takes a copy of the changed values of the dataset of Adjustments
                    myGlobal = myAdjustmentsDelegate.Clone(Me.SelectedAdjustmentsDS)
                    If Not myGlobal.SetDatos Is Nothing AndAlso Not myGlobal.HasError Then
                        Me.TemporalAdjustmentsDS = CType(myGlobal.SetDatos, SRVAdjustmentsDS)
                        Me.TempToSendAdjustmentsDelegate = New FwAdjustmentsDelegate(Me.TemporalAdjustmentsDS)
                        ' Update dataset of the temporal dataset of Adjustments to sent to Fw
                        myGlobal = Me.UpdateTemporalAdjustmentsDS()
                    Else
                        Me.PrepareErrorMode()
                        Exit Sub
                    End If

                    If myGlobal.HasError Then
                        Me.PrepareErrorMode()
                    Else
                        If MyBase.SimulationMode Then
                            ' Insert the new activity into Historic reports
                            myScreenDelegate.AdjustmentBCPoint = EditedValue.NewValue
                            myGlobal = myScreenDelegate.InsertReport("ADJUST", "BARCODE")
                            If Not myGlobal.HasError Then
                                myScreenDelegate.LoadAdjDone = True
                                MyBase.CurrentMode = ADJUSTMENT_MODES.SAVED
                                Me.PrepareArea()
                            Else
                                Me.PrepareErrorMode()
                            End If
                        Else
                            MyBase.DisplayMessage(Messages.SRV_SAVE_ADJUSTMENTS.ToString)

                            ' Convert dataset to String for sending to Fw
                            myGlobal = Me.TempToSendAdjustmentsDelegate.ConvertDSToString()

                            If Not myGlobal.SetDatos Is Nothing AndAlso Not myGlobal.HasError Then
                                Dim pAdjuststr As String = CType(myGlobal.SetDatos, String)
                                myScreenDelegate.pValueAdjust = pAdjuststr

                                myScreenDelegate.AdjustmentBCPoint = EditedValue.NewValue

                                If Not myGlobal.HasError AndAlso MyBase.myServiceMDI.MDIAnalyzerManager.Connected Then
                                    myGlobal = Me.myScreenDelegate.SendLoad_Adjustments()
                                End If

                            Else
                                Me.PrepareErrorMode()
                            End If
                        End If

                    End If

                Else
                    Me.myScreenDelegate.LoadAdjDone = True
                    MyBase.CurrentMode = ADJUSTMENT_MODES.SAVED
                    Me.PrepareArea()
                    MyBase.DisplayMessage(Messages.SRV_ADJUSTMENTS_SAVED.ToString)
                End If
            End If

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".SaveAdjustment ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".SaveAdjustment ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Cancels changed values for the selected adjustment
    ''' </summary>
    ''' <remarks>XBC 15/12/2011</remarks>
    Private Sub CancelAdjustment()
        'Dim myGlobal As New GlobalDataTO
        Try
            If Me.ChangedValue Then
                MyBase.DisplayMessage(Messages.SRV_ADJUSTMENTS_CANCELLED.ToString)
            Else
                MyBase.DisplayMessage("")
            End If

            Me.ChangedValue = False
            MyBase.CurrentMode = ADJUSTMENT_MODES.LOADED
            PrepareArea()

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".CancelAdjustment ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".CancelAdjustment ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub ScanningBarCode()
        Dim myGlobal As New GlobalDataTO
        Try
            myGlobal = MyBase.Test
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
                        MyBase.CurrentMode = ADJUSTMENT_MODES.TESTED
                        PrepareArea()
                    Else
                        'Get the available Rotor Types for the Analyzer according its model
                        'Dim AnalyzerConfig As New AnalyzerModelRotorsConfigDelegate
                        Dim BarCodeDS As New AnalyzerManagerDS
                        Dim rowBarCode As AnalyzerManagerDS.barCodeRequestsRow

                        'All positions
                        rowBarCode = BarCodeDS.barCodeRequests.NewbarCodeRequestsRow
                        With rowBarCode
                            Select Case EditedValue.AdjustmentID
                                Case ADJUSTMENT_GROUPS.SAMPLES_ROTOR_BC
                                    .RotorType = "SAMPLES"
                                Case ADJUSTMENT_GROUPS.REAGENTS_ROTOR_BC
                                    .RotorType = "REAGENTS"
                            End Select
                            .Action = GlobalEnumerates.Ax00CodeBarAction.FULL_ROTOR
                            .Position = 0
                            ' XBC 10/02/2012
                            '.SetCommangConfigNull()
                            ' XBC 10/02/2012
                        End With
                        BarCodeDS.barCodeRequests.AddbarCodeRequestsRow(rowBarCode)

                        BarCodeDS.AcceptChanges()

                        Me.Cursor = Cursors.WaitCursor
                        myScreenDelegate.SendBARCODE_REQUEST(BarCodeDS, BarCodeAdjustmentDelegate.OPERATIONS.TEST)
                    End If
                End If

            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".ScanningBarCode", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ScanningBarCode", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Start reading intensity mode barcode
    ''' </summary>
    ''' <remarks>Created by XBC 23/10/2012</remarks>
    Private Sub StartReading()
        Try
            MyBase.myServiceMDI.SEND_INFO_STOP()
            Me.DisableAll()
            System.Threading.Thread.Sleep(500)

            'Get the available Rotor Types for the Analyzer according its model
            'Dim AnalyzerConfig As New AnalyzerModelRotorsConfigDelegate
            Dim BarCodeDS As New AnalyzerManagerDS
            Dim rowBarCode As AnalyzerManagerDS.barCodeRequestsRow

            'All positions
            rowBarCode = BarCodeDS.barCodeRequests.NewbarCodeRequestsRow
            With rowBarCode
                Select Case EditedValue.AdjustmentID
                    Case ADJUSTMENT_GROUPS.SAMPLES_ROTOR_BC
                        .RotorType = "SAMPLES"
                    Case ADJUSTMENT_GROUPS.REAGENTS_ROTOR_BC
                        .RotorType = "REAGENTS"
                End Select
                .Action = GlobalEnumerates.Ax00CodeBarAction.START_TEST_MODE
                .Position = 0
            End With
            BarCodeDS.barCodeRequests.AddbarCodeRequestsRow(rowBarCode)

            BarCodeDS.AcceptChanges()

            Me.Cursor = Cursors.WaitCursor
            myScreenDelegate.SendBARCODE_REQUEST(BarCodeDS, BarCodeAdjustmentDelegate.OPERATIONS.TEST_MODE)

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".StartReading ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".StartReading ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Stop reading intensity mode barcode
    ''' </summary>
    ''' <remarks>Created by XBC 23/10/2012</remarks>
    Private Sub StopReading()
        Try
            'Get the available Rotor Types for the Analyzer according its model
            'Dim AnalyzerConfig As New AnalyzerModelRotorsConfigDelegate
            Dim BarCodeDS As New AnalyzerManagerDS
            Dim rowBarCode As AnalyzerManagerDS.barCodeRequestsRow

            'All positions
            rowBarCode = BarCodeDS.barCodeRequests.NewbarCodeRequestsRow
            With rowBarCode
                Select Case EditedValue.AdjustmentID
                    Case ADJUSTMENT_GROUPS.SAMPLES_ROTOR_BC
                        .RotorType = "SAMPLES"
                    Case ADJUSTMENT_GROUPS.REAGENTS_ROTOR_BC
                        .RotorType = "REAGENTS"
                End Select
                .Action = GlobalEnumerates.Ax00CodeBarAction.END_TEST_MODE
                .Position = 0
            End With
            BarCodeDS.barCodeRequests.AddbarCodeRequestsRow(rowBarCode)

            BarCodeDS.AcceptChanges()

            Me.Cursor = Cursors.WaitCursor
            myScreenDelegate.SendBARCODE_REQUEST(BarCodeDS, BarCodeAdjustmentDelegate.OPERATIONS.TEST_MODE_END)

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".StopReading ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".StopReading ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

#Region "Generic Adjusments DS Methods"

    ''' <summary>
    ''' Update some value in the specific adjustments ds
    ''' </summary>
    ''' <param name="pCodew"></param>
    ''' <param name="pValue"></param>
    ''' <returns></returns>
    ''' <remarks>XBC 14/12/2011</remarks>
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
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".UpdateSpecificAdjustmentsDS ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
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
    ''' <remarks>XBC 14/12/2011</remarks>
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
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".UpdateTemporalSpecificAdjustmentsDS ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".UpdateTemporalSpecificAdjustmentsDS", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
        Me.TemporalAdjustmentsDS.AcceptChanges()
        Return myGlobal
    End Function

    ''' <summary>
    ''' updates all a temporal dataset with changed current adjustments
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>XBC 14/12/2011</remarks>
    Private Function UpdateTemporalAdjustmentsDS() As GlobalDataTO
        Dim myGlobal As New GlobalDataTO
        Try
            With Me.EditedValue
                UpdateTemporalSpecificAdjustmentsDS(ReadSpecificAdjustmentData(GlobalEnumerates.AXIS.ROTOR).CodeFw, .NewValue.ToString())
            End With

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".UpdateTemporalAdjustmentsDS ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
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
    ''' Created by XBC 14/12/2011
    ''' Modified by XB 04/02/2013 - Upper conversions redundants because the value is already in UpperCase must delete to avoid Regional Settings problems (Bugs tracking #1112)
    ''' </remarks>
    Private Function ReadGlobalAdjustmentData(ByVal pGroupID As String, ByVal pAxis As GlobalEnumerates.AXIS, Optional ByVal pNotForDisplaying As Boolean = False) As FwAdjustmentsDataTO
        'Dim myGlobal As New GlobalDataTO
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
            'Where a.GroupID.Trim.ToUpper = myGroup.Trim.ToUpper
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
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".ReadGlobalAdjustmentValue ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
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
    ''' Created by XBC 14/12/2011
    ''' Modified by XB 04/02/2013 - Upper conversions redundants because the value is already in UpperCase must delete to avoid Regional Settings problems (Bugs tracking #1112)
    ''' </remarks>
    Private Function ReadSpecificAdjustmentData(ByVal pAxis As GlobalEnumerates.AXIS) As FwAdjustmentsDataTO
        'Dim myGlobal As New GlobalDataTO
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
    ''' <remarks>XBC 14/12/2011</remarks>
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
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".LoadAdjustmentGroupData ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".LoadAdjustmentGroupData ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
        Return resultData
    End Function

#End Region

#Region "Simulation Methods"

    Private Sub SimulateRELPositioning()
        Try
            ' simulating
            MyBase.DisplayMessage(Messages.SRV_STEP_POSITIONING.ToString)

            MyBase.CurrentMode = ADJUSTMENT_MODES.ADJUSTING
            PrepareArea()

            Me.Cursor = Cursors.WaitCursor
            System.Threading.Thread.Sleep(SimulationProcessTime)
            MyBase.myServiceMDI.Focus()
            Me.Cursor = Cursors.Default

            MyBase.CurrentMode = ADJUSTMENT_MODES.ADJUSTED
            MyBase.DisplayMessage(Messages.SRV_COMPLETED.ToString)
            PrepareArea()

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Name & ".SimulateStepPositioning", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Name & ".SimulateStepPositioning", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub SimulateABSPositioning()
        Try
            ' simulating
            MyBase.DisplayMessage(Messages.SRV_ABS_REQUESTED.ToString)

            MyBase.CurrentMode = ADJUSTMENT_MODES.ADJUSTING
            PrepareArea()

            Me.Cursor = Cursors.WaitCursor
            System.Threading.Thread.Sleep(SimulationProcessTime)
            MyBase.myServiceMDI.Focus()
            Me.Cursor = Cursors.Default

            MyBase.CurrentMode = ADJUSTMENT_MODES.ADJUSTED
            MyBase.DisplayMessage(Messages.SRV_COMPLETED.ToString)
            PrepareArea()

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Name & ".SimulateAbsPositioning", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Name & ".SimulateAbsPositioning", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub SimulateHOMEPositioning()
        Try
            ' simulating
            MyBase.DisplayMessage(Messages.SRV_HOMES_IN_PROGRESS.ToString)

            MyBase.CurrentMode = ADJUSTMENT_MODES.ADJUSTING
            PrepareArea()

            Me.Cursor = Cursors.WaitCursor
            System.Threading.Thread.Sleep(SimulationProcessTime)
            MyBase.myServiceMDI.Focus()
            Me.Cursor = Cursors.Default

            MyBase.CurrentMode = ADJUSTMENT_MODES.ADJUSTED
            MyBase.DisplayMessage(Messages.SRV_COMPLETED.ToString)
            PrepareArea()

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Name & ".SimulateHOMEPositioning", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Name & ".SimulateHOMEPositioning", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Function SimulateBarcodeReading() As GlobalDataTO
        Dim myGlobal As New GlobalDataTO
        Try
            myScreenDelegate.BcResultsAdd(45, "AAAAAccc")
            myScreenDelegate.BcResultsAdd(67, "BBBBBBBBBBBB123")
            myScreenDelegate.BcResultsAdd(110, "12911298CCCCCCCCCCC")

            ' Insert the new activity into Historic reports
            myGlobal = myScreenDelegate.InsertReport("TEST", "BARCODE")
            If myGlobal.HasError Then
                Me.PrepareErrorMode()
            End If

        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myGlobal.ErrorMessage = ex.Message
            MyBase.CreateLogActivity(ex.Message, Name & ".SimulateBarcodeReading", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Name & ".SimulateBarcodeReading", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
        Return myGlobal
    End Function

#End Region

#End Region

#Region "Events"

#Region "GENERIC"

    Private Sub IBarCodeAdjustments_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
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

    Private Sub IBarCodeAdjustments_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim myGlobal As New GlobalDataTO
        'Dim myGlobalbase As New GlobalBase
        Try
            MyBase.MyBase_Load(sender, e)

            MyBase.GetUserNumericalLevel()

            'Get the current Language from the current Application Session
            currentLanguage = GlobalBase.GetSessionInfo().ApplicationLanguage.Trim.ToString

            'Screen delegate
            MyClass.myScreenDelegate = New BarCodeAdjustmentDelegate(MyBase.myServiceMDI.ActiveAnalyzer, MyBase.myFwScriptDelegate)

            'Load the multilanguage texts for all Screen Labels and get Icons for graphical Buttons
            MyClass.GetScreenLabels()

            MyClass.PrepareButtons()

            'SGM 12/11/2012 - Information
            Me.BsInfoXPSViewer.FitToPageHeight()
            MyBase.DisplayInformation(APPLICATION_PAGES.BARCODE, Me.BsInfoXPSViewer)

            MyClass.DisableAll()

            MyClass.InitializeHomes()

            MyClass.Initializations()

            ' Check communications with Instrument
            If Not MyBase.myServiceMDI.MDIAnalyzerManager.Connected Then
                PrepareErrorMode()
                MyBase.ActivateMDIMenusButtons(True)
            Else
                PrepareAdjustReadingMode()
            End If

            If myGlobal.HasError Then
                PrepareErrorMode()
                ShowMessage(Me.Name & ".Load", myGlobal.ErrorCode, myGlobal.ErrorMessage, Me)
            End If

            ResetBorderSRV()

        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".Load ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".Load ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub IBarCodeAdjustments_Shown(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Shown
        Try
            Me.BsInfoXPSViewer.Visible = True
            Me.BsInfoXPSViewer.RefreshPage()

        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".Shown ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".Shown ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub BsExitButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsExitButton.Click
        Try
            MyBase.DisplayMessage("")
            Me.ExitScreen()

        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".BsExitButton.Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".BsExitButton.Click ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' When the  ESC key is pressed, the screen is closed 
    ''' </summary>
    ''' <remarks>
    ''' Created by XBC 14/12/2011
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
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".KeyDown ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".KeyDown", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub TestProcessTimer_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TestProcessTimer.Tick
        Try
            Me.ProgressBar1.Value += 1
            Me.ProgressBar1.Refresh()

        Catch ex As Exception
            MyClass.TestProcessTimer.Enabled = False
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".TestProcessTimer_Tick ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            'MyBase.ShowMessage(Me.Name & ".TestProcessTimer_Tick", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
            Me.PrepareErrorMode()
        End Try
    End Sub

#End Region

#Region "ROTOR SELECTION"

    Private Sub SampleRotorRadioButton_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SampleRotorRadioButton.CheckedChanged
        Try
            If Me.PageInitialized Then

                Me.BarCodeDataGridView.Rows.Clear()
                Me.ReadedValueLabel.Text = ""

                With Me.EditedValue
                    If Me.SampleRotorRadioButton.Checked Then
                        .AdjustmentID = ADJUSTMENT_GROUPS.SAMPLES_ROTOR_BC
                        myScreenDelegate.AdjustmentID = ADJUSTMENT_GROUPS.SAMPLES_ROTOR_BC

                        LoadAdjustmentGroupData()
                        PopulateEditionValues()

                        MyBase.CurrentMode = ADJUSTMENT_MODES.LOADED
                        PrepareArea()
                        MyBase.DisplayMessage("")

                    End If
                End With

            End If

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".SampleRotorRadioButton_CheckedChanged ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".SampleRotorRadioButton_CheckedChanged ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub ReagentRotorRadioButton_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ReagentRotorRadioButton.CheckedChanged
        Try
            If Me.PageInitialized Then

                Me.BarCodeDataGridView.Rows.Clear()
                Me.ReadedValueLabel.Text = ""

                With Me.EditedValue
                    If Me.ReagentRotorRadioButton.Checked Then
                        .AdjustmentID = ADJUSTMENT_GROUPS.REAGENTS_ROTOR_BC
                        myScreenDelegate.AdjustmentID = ADJUSTMENT_GROUPS.REAGENTS_ROTOR_BC

                        LoadAdjustmentGroupData()
                        PopulateEditionValues()

                        MyBase.CurrentMode = ADJUSTMENT_MODES.LOADED
                        PrepareArea()
                        MyBase.DisplayMessage("")

                    End If
                End With

            End If

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".ReagentRotorRadioButton_CheckedChanged ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".ReagentRotorRadioButton_CheckedChanged ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

#End Region

#Region "ROTOR CENTERING"

    Private Sub AdjustButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AdjustButton.Click
        Dim myGlobal As New GlobalDataTO
        Try
            MyBase.DisplayMessage("")
            myGlobal = MyBase.PrepareAdjust()
            If myGlobal.HasError Then
                PrepareErrorMode()
            Else
                PrepareArea()

                ' Initializations

                If myGlobal.HasError Then
                    PrepareErrorMode()
                Else

                    If MyBase.SimulationMode Then
                        ' simulating
                        Me.Cursor = Cursors.WaitCursor
                        System.Threading.Thread.Sleep(SimulationProcessTime)
                        MyBase.myServiceMDI.Focus()
                        Me.Cursor = Cursors.Default
                        MyBase.CurrentMode = ADJUSTMENT_MODES.ADJUST_PREPARED
                        PrepareArea()
                    Else
                        ' Manage FwScripts must to be sent to adjusting
                        Me.SendFwScript(Me.CurrentMode, EditedValue.AdjustmentID)
                    End If
                End If

            End If

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".AdjustButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".AdjustButton_Click ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub SaveButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SaveButton.Click
        Try
            MyBase.DisplayMessage("")
            Me.SaveAdjustment()

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".SaveButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".SaveButton_Click ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub CancelButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButtonCancel.Click
        'Dim myGlobal As New GlobalDataTO
        Dim dialogResultToReturn As DialogResult = Windows.Forms.DialogResult.No
        Try

            If Me.ChangedValue Then
                dialogResultToReturn = MyBase.ShowMessage("", Messages.SRV_DISCARD_CHANGES.ToString)

                If dialogResultToReturn = Windows.Forms.DialogResult.Yes Then
                    Me.CancelAdjustment()
                End If
            Else
                Me.CancelAdjustment()
            End If

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".CancelButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".CancelButton_Click ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

#Region "event handler overloads"
    ' SET ABSOLUTE POINT
    Private Sub BsAdjustRotor_SetABSPointReleased(ByVal sender As System.Object, ByVal Value As System.Single) Handles BsAdjust.AbsoluteSetPointReleased
        Try
            Me.EditedValue.NewValue = Value

            If MyBase.SimulationMode Then
                Me.SimulateABSPositioning()
            Else
                MakeAdjustment(MOVEMENT.ABSOLUTE)
                MyBase.DisplayMessage(Messages.SRV_ABS_REQUESTED.ToString)
            End If

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".BsAdjustRotor_SetABSPointReleased ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".BsAdjustRotor_SetABSPointReleased ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub
    ' SET RELATIVE POINT
    Private Sub BsAdjustRotor_SetRELPointReleased(ByVal sender As System.Object, ByVal Value As System.Single) Handles BsAdjust.RelativeSetPointReleased
        Try
            Me.EditedValue.stepValue = Value
            Me.EditedValue.NewValue = Me.EditedValue.CurrentValue + Value

            If MyBase.SimulationMode Then
                Me.SimulateRELPositioning()
            Else
                MakeAdjustment(MOVEMENT.RELATIVE)
                MyBase.DisplayMessage(Messages.SRV_STEP_POSITIONING.ToString)
            End If

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".BsAdjustRotor_SetRELPointReleased ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".BsAdjustRotor_SetRELPointReleased ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub
    ' HOMES
    Private Sub BsAdjustRotor_HomeRequestReleased(ByVal sender As System.Object) Handles BsAdjust.HomeRequestReleased
        Try
            Me.EditedValue.NewValue = 0 ' HomeRotor

            If MyBase.SimulationMode Then
                Me.SimulateHOMEPositioning()
                Me.BsAdjust.CurrentValue = 0
                Me.EditedValue.CurrentValue = 0
            Else
                MakeAdjustment(MOVEMENT.HOME)
                MyBase.DisplayMessage(Messages.SRV_HOMES_IN_PROGRESS.ToString)
            End If

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".BsAdjustRotor_HomeRequestReleased ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".BsAdjustRotor_HomeRequestReleased ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub
    ' SET POINT OUT OF RANGE
    Private Sub BsAdjustRotor_SetPointOutOfRange(ByVal sender As Object) Handles BsAdjust.SetPointOutOfRange
        Try
            MyBase.DisplayMessage(Messages.SRV_OUTOFRANGE.ToString)
        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".BsAdjustRotor_SetPointOutOfRange ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".BsAdjustRotor_SetPointOutOfRange ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub
    '' VALIDATION ERROR
    'Private Sub BsAdjustRotor_ValidationError(ByVal sender As Object, ByVal Value As String) Handles BsAdjust.ValidationError
    '    Try
    '        MyBase.DisplayMessage(Messages.FWSCRIPT_VALIDATION_ERROR.ToString)
    '    Catch ex As Exception
    '        MyBase.CreateLogActivity(ex.Message, Me.Name & ".BsAdjustRotor_ValidationError ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
    '        MyBase.ShowMessage(Me.Name & ".BsAdjustRotor_ValidationError ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
    '    End Try
    'End Sub

    Private Sub BsAdjustRotor_FocusReceived(ByVal sender As Object) Handles BsAdjust.FocusReceived
        Try
            MyBase.myFocusedAdjustControl = CType(sender, BSAdjustControl)

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".BsAdjustRotor_FocusReceived ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".BsAdjustRotor_FocusReceived ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' disables buttons while editing a control
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <remarks>XBC 15/12/2011</remarks>
    Private Sub BsAdjustRotor_OnEditionMode(ByVal sender As Object, ByVal pEditionMode As Boolean) Handles BsAdjust.EditionModeChanged

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
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".BsAdjustRotor_OnEditionMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".BsAdjustRotor_OnEditionMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

#End Region

#End Region

#Region "ROTOR READING"

    Private Sub StartReadingButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles StartReadingButton.Click
        Dim myGlobal As New GlobalDataTO
        Try
            MyBase.DisplayMessage("")
            myGlobal = MyBase.Test
            If myGlobal.HasError Then
                PrepareErrorMode()
            Else
                PrepareArea()

                Me.StartReadingButton.Visible = False
                Me.StopButton.Visible = True

                If Not myGlobal.HasError Then
                    If MyBase.SimulationMode Then
                        ' simulating
                        Me.Cursor = Cursors.WaitCursor
                        System.Threading.Thread.Sleep(SimulationProcessTime)
                        MyBase.myServiceMDI.Focus()
                        Me.Cursor = Cursors.Default
                        Me.ReadedValueLabel.Text = "100 %"
                        myScreenDelegate.CurrentOperation = BarCodeAdjustmentDelegate.OPERATIONS.TEST_MODE
                        MyBase.CurrentMode = ADJUSTMENT_MODES.TESTED
                        PrepareArea()
                    Else
                        StartReading()
                    End If
                End If

            End If

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".StartReadingButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".StartReadingButton_Click ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub StopButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles StopButton.Click
        Dim myGlobal As New GlobalDataTO
        Try
            myGlobal = MyBase.Test
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
                        myScreenDelegate.CurrentOperation = BarCodeAdjustmentDelegate.OPERATIONS.TEST_MODE_END
                        MyBase.CurrentMode = ADJUSTMENT_MODES.TESTED
                        PrepareArea()
                    Else
                        StopReading()
                    End If
                End If

            End If

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".StopButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".StopButton_Click ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

#End Region

#Region "BARCODE TEST"

    Private Sub TestButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TestButton.Click
        Dim myGlobal As New GlobalDataTO
        Try
            MyBase.DisplayMessage("")
            myGlobal = MyBase.Test
            If myGlobal.HasError Then
                PrepareErrorMode()
            Else
                PrepareArea()

                ' initializations
                myScreenDelegate.HomesDone = False
                myScreenDelegate.TestDone = False
                Me.BarCodeDataGridView.Rows.Clear()

                If Not myGlobal.HasError Then
                    If MyBase.SimulationMode Then
                        ' simulating
                        MyBase.DisplayMessage(Messages.SRV_HOMES_IN_PROGRESS.ToString)

                        Me.Cursor = Cursors.WaitCursor

                        Me.ProgressBar1.Value = 0
                        Me.ProgressBar1.Maximum = 5
                        Me.ProgressBar1.Visible = True

                        For i As Integer = 1 To Me.ProgressBar1.Maximum
                            System.Threading.Thread.Sleep(i * MyBase.SimulationProcessTime)
                            Me.ProgressBar1.Value = i
                            Me.ProgressBar1.Refresh()
                        Next

                        myScreenDelegate.TestDone = True
                        Me.SimulateBarcodeReading()

                        Me.Cursor = Cursors.Default

                        myScreenDelegate.CurrentOperation = BarCodeAdjustmentDelegate.OPERATIONS.TEST
                        MyBase.CurrentMode = ADJUSTMENT_MODES.TESTED
                        PrepareArea()
                    Else
                        ' Manage FwScripts must to be sent to testing
                        Me.SendFwScript(Me.CurrentMode, EditedValue.AdjustmentID)
                        If myScreenDelegate.HomesDone Then
                            MyBase.CurrentMode = ADJUSTMENT_MODES.TESTED
                            PrepareArea()
                        End If
                    End If
                End If

            End If

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".TestButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".TestButton_Click ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

#End Region


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
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".BsXPSViewer_Load ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".BsXPSViewer_Load ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

#End Region

End Class