Option Strict On
Option Explicit On
Option Infer On

Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.Global.GlobalEnumerates
Imports LIS.Biosystems.Ax00.LISCommunications
Imports Biosystems.Ax00.CommunicationsSwFw

Public Class ILISUtilities
    Inherits Biosystems.Ax00.PresentationCOM.BSBaseForm

#Region "Declaration"
    Private LanguageID As String
    Private MainMDI As IAx00MainMDI
    Private OKImage As String
    Private WrongImage As String
    Private mdiAnalyzerCopy As AnalyzerManager
    Private mdiESWrapperCopy As ESWrapper
#End Region



    ''' <summary>
    ''' Get the images path to indicate when utility execution is OK or Wrong.
    ''' </summary>
    ''' <remarks>
    ''' CREATED BY: TR 22/04/2013
    ''' </remarks>
    Private Sub GetOKAndWrongImages()
        Try
            OKImage = MyBase.IconsPath & GetIconName("ACCEPTF")
            WrongImage = MyBase.IconsPath & GetIconName("WARNINGF")

        Catch ex As Exception
            'Write error SYSTEM_ERROR in the Application Log
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & " GetOKAndWrongImages ", EventLogEntryType.Error, _
                                                            GetApplicationInfoSession().ActivateSystemLog)
            'Show error message
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    Private Sub ExitScreen()
        Try
            ExecuteActionButton.Enabled = False ' Disable button on windows close.
            bsCancelButton.Enabled = False
            If Not Me.Tag Is Nothing Then
                'A PerformClick() method was executed
                Me.Close()
            Else
                'Normal button click
                'Open the WS Monitor form and close this one
                IAx00MainMDI.OpenMonitorForm(Me)
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", ".bsCancelButton_Click " & Me.Name, EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsCancelButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Fill the Log level combo box.
    ''' </summary>
    ''' <remarks>CREATED BY: TR 22/04/2013</remarks>
    Private Sub FillLogLevels()
        Try
            Dim myGlobalDataTO As New GlobalDataTO()
            Dim myPreMasterDataDS As New PreloadedMasterDataDS
            Dim myPreloadedMasterDataDelegate As New PreloadedMasterDataDelegate
            'Get the data corresponding to the Analysis type
            myGlobalDataTO = myPreloadedMasterDataDelegate.GetList(Nothing, _
                                    GlobalEnumerates.PreloadedMasterDataEnum.LIS_TRACE_LEVEL)

            If Not myGlobalDataTO.HasError Then
                myPreMasterDataDS = CType(myGlobalDataTO.SetDatos, PreloadedMasterDataDS)

                Dim qAnalysisMode As New List(Of PreloadedMasterDataDS.tfmwPreloadedMasterDataRow)
                'Order the analysis mode by the position number.
                qAnalysisMode = (From a In myPreMasterDataDS.tfmwPreloadedMasterData _
                                Order By a.Position Select a).ToList()

                TraceLevelCombo.DisplayMember = "FixedItemDesc"
                TraceLevelCombo.ValueMember = "ItemID"
                TraceLevelCombo.DataSource = qAnalysisMode
            Else
                ShowMessage("Error", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage)
            End If
        Catch ex As Exception
            'Write error SYSTEM_ERROR in the Application Log
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & " FillLogLevels ", EventLogEntryType.Error, _
                                                            GetApplicationInfoSession().ActivateSystemLog)
            'Show error message
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))") 'AG 07/07/2010  "SYSTEM_ERROR", ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try

    End Sub

    Private Sub GetScreenLabels()
        Try
            Dim MLRD As New MultilanguageResourcesDelegate
            ' TODO TEXTS !!!!
            LISUtilitiesLabel.Text = MLRD.GetResourceText(Nothing, "MENU_LIS_UTILITIES", LanguageID) ' "         *LIS Utilities"
            DeleteLISOrdersRB.Text = MLRD.GetResourceText(Nothing, "DELETE_LIS_ORDERS", LanguageID) '       "*Delete LIS orders"
            DeleteInternalQueue.Text = MLRD.GetResourceText(Nothing, "DELETE_INTERNAL_QUEUES", LanguageID) '"*Delete Internal queues"
            TracinLevelRB.Text = MLRD.GetResourceText(Nothing, "SEL_TRACE_LEVEL_ES_LOG", LanguageID) '     "*Selection of Tracing level for ES log"

            'Set ToolTips.
            bsScreenToolTips.SetToolTip(bsCancelButton, MLRD.GetResourceText(Nothing, "BTN_Cancel&Close", LanguageID)) 'For Tooltip
            bsScreenToolTips.SetToolTip(ExecuteActionButton, MLRD.GetResourceText(Nothing, "BTN_SRV_Action", LanguageID)) 'For Tooltip

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".GetScreenLabels ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".GetScreenLabels ", Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Load the value of Trace level on combo.
    ''' </summary>
    ''' <remarks>CREATED BY: TR 22/04/2013</remarks>
    Private Sub LoadTraceLevel()
        Try
            Dim myGlobalDataTO As New GlobalDataTO
            Dim myUserSettingsDelegate As New UserSettingsDelegate
            myGlobalDataTO = myUserSettingsDelegate.GetCurrentValueBySettingID(Nothing, UserSettingsEnum.LIS_TRACE_LEVEL.ToString())
            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                TraceLevelCombo.SelectedValue = DirectCast(myGlobalDataTO.SetDatos, String)
            Else
                'Error getting the Session Setting value, show it 
                ShowMessage(Name & ".LoadTraceLevel ", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage)
            End If
        Catch ex As Exception

        End Try
    End Sub

    ''' <summary>
    ''' Hide all the status images.
    ''' </summary>
    ''' <remarks>CREATED BY: TR 22/04/2013</remarks>
    Private Sub HideStatusImages()
        Try
            DelLISOrdersPictureBox.Visible = False
            DelInternalQPictureBox.Visible = False
            TaceLevelPictureBox.Visible = False
        Catch ex As Exception

        End Try
    End Sub

    ''' <summary>
    ''' Initialize all screen controls.
    ''' </summary>
    ''' <remarks>CREATED BY: TR 22/04/2013</remarks>
    Private Sub ScreenLoad()
        Try
            MainMDI = CType(Me.MdiParent, IAx00MainMDI)

            'Dim currentLanguageGlobal As New GlobalBase
            LanguageID = GlobalBase.GetSessionInfo().ApplicationLanguage

            GetScreenLabels()
            FillLogLevels()
            LoadTraceLevel()

            TraceLevelCombo.Enabled = TracinLevelRB.Checked

            GetOKAndWrongImages()

            LoadScreenStatus()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".ScreenLoad ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ScreenLoad ", Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub


    ''' <summary>
    ''' Set the screen status enable or disable.
    ''' </summary>
    ''' <remarks>CREATED BY: TR 23/04/2013
    ''' AG 26/04/2013 - lis actions activation rules moved to ESBusiness class</remarks>
    Private Sub LoadScreenStatus()
        Try
            If Not AppDomain.CurrentDomain.GetData("GlobalLISManager") Is Nothing Then
                mdiESWrapperCopy = CType(AppDomain.CurrentDomain.GetData("GlobalLISManager"), ESWrapper) ' Use the same ESWrapper as the MDI
            End If

            'Get the Analyzer Manager
            If (Not AppDomain.CurrentDomain.GetData("GlobalAnalyzerManager") Is Nothing) Then
                mdiAnalyzerCopy = CType(AppDomain.CurrentDomain.GetData("GlobalAnalyzerManager"), AnalyzerManager)
            End If

            'If (Not mdiAnalyzerCopy Is Nothing) Then
            '    'If the connection process is in process, disable all screen fields (changes are not allowed) ORELSE
            '    'If the Analyzer is connected and its status is RUNNING, disable all screen fields (changes are not allowed)
            '    If (mdiAnalyzerCopy.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.CONNECTprocess) = "INPROCESS") OrElse _
            '       (mdiAnalyzerCopy.Connected AndAlso mdiAnalyzerCopy.AnalyzerStatus = GlobalEnumerates.AnalyzerManagerStatus.RUNNING) _
            '       OrElse (mdiESWrapperCopy.Status <> GlobalEnumerates.LISStatus.connectionAccepted.ToString().ToUpper() AndAlso _
            '               mdiESWrapperCopy.Status <> GlobalEnumerates.LISStatus.connectionEnabled.ToString().ToUpper()) Then
            '        DeleteLISOrdersRB.Enabled = False
            '        DeleteInternalQueue.Enabled = False
            '    End If
            'End If
            'If (Not mdiAnalyzerCopy Is Nothing AndAlso Not mdiESWrapperCopy Is Nothing) Then
            '    Dim myESBusiness As New ESBusiness
            '    Dim runningFlag As Boolean = CBool(IIf(mdiAnalyzerCopy.AnalyzerStatus = AnalyzerManagerStatus.RUNNING, True, False))
            '    Dim connectingFlag As Boolean = CBool(IIf(mdiAnalyzerCopy.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.CONNECTprocess) = "INPROCESS", True, False))
            '    DeleteLISOrdersRB.Enabled = myESBusiness.AllowLISAction(Nothing, LISActions.LISUtilities_DeleteLISSavedWS, runningFlag, connectingFlag, mdiESWrapperCopy.Status, mdiESWrapperCopy.Storage)
            '    DeleteInternalQueue.Enabled = myESBusiness.AllowLISAction(Nothing, LISActions.LISUtilities_ClearQueue, runningFlag, connectingFlag, mdiESWrapperCopy.Status, mdiESWrapperCopy.Storage)
            '    TracinLevelRB.Enabled = myESBusiness.AllowLISAction(Nothing, LISActions.LISUtilities_TracesEnabling, runningFlag, connectingFlag, mdiESWrapperCopy.Status, mdiESWrapperCopy.Storage)
            'End If

            MyClass.RefreshElementsEnabled() 'SG 15/05/2013

            'If user level operator read only.
            'Dim myGlobalbase As New GlobalBase
            CurrentUserLevel = GlobalBase.GetSessionInfo.UserLevel
            If CurrentUserLevel = "OPERATOR" Then
                MainGroupBox.Enabled = False
                ExecuteActionButton.Enabled = False
            End If


        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".LoadScreenStatus ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".LoadScreenStatus ", Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Refreshes the ability of the screen elements according to the current LIS status
    ''' </summary>
    ''' <remarks>Created by SG 15/05/2013</remarks>
    Public Sub RefreshElementsEnabled()
        Try
            If (Not mdiAnalyzerCopy Is Nothing AndAlso Not mdiESWrapperCopy Is Nothing) Then
                Dim myESBusiness As New ESBusiness
                Dim runningFlag As Boolean = CBool(IIf(mdiAnalyzerCopy.AnalyzerStatus = AnalyzerManagerStatus.RUNNING, True, False))
                Dim connectingFlag As Boolean = CBool(IIf(mdiAnalyzerCopy.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.CONNECTprocess) = "INPROCESS", True, False))
                DeleteLISOrdersRB.Enabled = myESBusiness.AllowLISAction(Nothing, LISActions.LISUtilities_DeleteLISSavedWS, runningFlag, connectingFlag, mdiESWrapperCopy.Status, mdiESWrapperCopy.Storage)
                DeleteInternalQueue.Enabled = myESBusiness.AllowLISAction(Nothing, LISActions.LISUtilities_ClearQueue, runningFlag, connectingFlag, mdiESWrapperCopy.Status, mdiESWrapperCopy.Storage)
                TracinLevelRB.Enabled = myESBusiness.AllowLISAction(Nothing, LISActions.LISUtilities_TracesEnabling, runningFlag, connectingFlag, mdiESWrapperCopy.Status, mdiESWrapperCopy.Storage)

                'deselect checked if not enabled
                If Not DeleteLISOrdersRB.Enabled Then DeleteLISOrdersRB.Checked = False
                If Not DeleteInternalQueue.Enabled Then DeleteInternalQueue.Checked = False
                If Not TracinLevelRB.Enabled Then TracinLevelRB.Checked = False

            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".RefreshElementsEnabled ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".RefreshElementsEnabled ", Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Excecute the action depending then selected utility
    ''' </summary>
    ''' <remarks>CREATED BY: TR 22/04/2013</remarks>
    Private Sub ExecuteAction()
        Try
            Me.Cursor = Cursors.WaitCursor

            HideStatusImages()

            Dim myGlobalDataTO As New GlobalDataTO
            If DeleteLISOrdersRB.Checked Then
                myGlobalDataTO = DeleteLISOrders()

                If Not myGlobalDataTO.HasError Then
                    DelLISOrdersPictureBox.ImageLocation = OKImage
                Else
                    DelLISOrdersPictureBox.ImageLocation = WrongImage
                End If

                DelLISOrdersPictureBox.Visible = True

            ElseIf DeleteInternalQueue.Checked Then
                myGlobalDataTO = DeleteInternalQueues()

                If Not myGlobalDataTO.HasError Then
                    DelInternalQPictureBox.ImageLocation = OKImage
                Else
                    DelInternalQPictureBox.ImageLocation = WrongImage
                End If

                DelInternalQPictureBox.Visible = True

            ElseIf TracinLevelRB.Checked Then
                myGlobalDataTO = SetTraceLevel()

                If Not myGlobalDataTO.HasError Then
                    TaceLevelPictureBox.ImageLocation = OKImage
                Else
                    TaceLevelPictureBox.ImageLocation = WrongImage
                End If

                TaceLevelPictureBox.Visible = True

            End If

            If myGlobalDataTO.HasError Then
                ShowMessage("Error", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage)
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".ExecuteAction ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ExecuteAction ", Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        Finally
            Me.Cursor = Cursors.Default
        End Try

    End Sub

    ''' <summary>
    ''' From table twksXMLMessages are removed all messages with Error Status.
    ''' Remove all the worksession saved on table tparSavedWS where FromLIS = TRUE 
    ''' and on table tparSavedWSOrderTests remove the orders that belong to the removed worksession.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>
    ''' CREATED BY: TR
    ''' MODIFIED BY: TR 02/05/2013 -Refresh LIS buttons. after delete.
    '''              TR 19/06/2013 - From table twksxmlMessages remove all the element with status PENDING.
    ''' </remarks>
    Private Function DeleteLISOrders() As GlobalDataTO
        Dim myGlobalDataTO As New GlobalDataTO
        Try
            'From table twksXMLMessages are removed all messages with Error Status
            Dim myXmlMessagesDelegate As New xmlMessagesDelegate
            'Delete all messages with error.
            myGlobalDataTO = myXmlMessagesDelegate.DeleteByStatus(Nothing, "ERROR")

            'TR 19/06/2013 -Delete messages with status PENDING.
            If Not myGlobalDataTO.HasError Then
                'TODO: Need to process the xml befor deleting to notify LIS.
                myGlobalDataTO = myXmlMessagesDelegate.DeleteByStatus(Nothing, "PENDING")
            End If
            'TR 19/06/2013 -END

            Dim rejectedAwosDS As New OrderTestsLISInfoDS
            Dim mySAvedWSOrderTestsDelegate As New SavedWSOrderTestsDelegate
            myGlobalDataTO = mySAvedWSOrderTestsDelegate.GetAllLISOrderTestToReject(Nothing)

            If Not myGlobalDataTO.HasError Then
                rejectedAwosDS = DirectCast(myGlobalDataTO.SetDatos, OrderTestsLISInfoDS)

                If (rejectedAwosDS.twksOrderTestsLISInfo.Rows.Count > 0) Then
                    IAx00MainMDI.InvokeRejectAwosDelayedLIS(rejectedAwosDS)
                End If

            End If

            If Not myGlobalDataTO.HasError Then
                'Remove all the worksession saved on table tparSavedWS where FromLIS = TRUE 
                'and from table tparSavedWSOrderTests.
                Dim mySavedWSDelegate As New SavedWSDelegate
                Dim mySavedWSDS As SavedWSDS
                'Get all WS From LIS
                myGlobalDataTO = mySavedWSDelegate.GetAll(Nothing, True)
                If Not myGlobalDataTO.HasError Then
                    mySavedWSDS = DirectCast(myGlobalDataTO.SetDatos, SavedWSDS)
                    'Delete 
                    myGlobalDataTO = mySavedWSDelegate.Delete(Nothing, mySavedWSDS)
                End If
            End If

            'TR 02/05/2013.
            IAx00MainMDI.ActivateLISActionButton()

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".DeleteLISOrders ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".DeleteLISOrders ", Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
        Return myGlobalDataTO
    End Function

    ''' <summary>
    ''' Delete all messages pending to be sent to LIS (Clean storage)
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function DeleteInternalQueues() As GlobalDataTO
        Dim myGlobalDataTO As New GlobalDataTO
        Try
            MainMDI.InvokeDeleteAllMessage()
        Catch ex As Exception
            myGlobalDataTO.HasError = True
            myGlobalDataTO.ErrorMessage = ex.Message
            myGlobalDataTO.ErrorCode = Messages.SYSTEM_ERROR.ToString()
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".DeleteInternalQueues ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".DeleteInternalQueues ", Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
        Return myGlobalDataTO
    End Function

    ''' <summary>
    ''' Whrite on the machine registy the LIS Trace level and save value
    ''' on table UserSettings.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>
    ''' CREATED BY: TR 22/04/2013
    ''' MODIFIED BY: TR 24/05/2013 -implement the functionality of saving information on the Registry
    '''                             into the Utilities class instead of this form.
    ''' </remarks>
    Private Function SetTraceLevel() As GlobalDataTO
        Dim myGlobalDataTO As New GlobalDataTO
        Try
            'Dim myUtilities As New Utilities

            myGlobalDataTO = Utilities.SetLISTraceLevel(TraceLevelCombo.SelectedValue.ToString())

            If Not myGlobalDataTO.HasError Then
                'Save on tcfgUserSettings the saved value.
                Dim myUserSettingsDelegate As New UserSettingsDelegate
                myGlobalDataTO = myUserSettingsDelegate.Update(Nothing, UserSettingsEnum.LIS_TRACE_LEVEL.ToString(), _
                                                                                TraceLevelCombo.SelectedValue.ToString())
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".SetTraceLevel ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".SetTraceLevel ", Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try

        Return myGlobalDataTO

    End Function

    Private Sub bsCancelButton_Click(sender As Object, e As EventArgs) Handles bsCancelButton.Click
        ExitScreen()
    End Sub

    Private Sub ILISUtilities_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        If (e.KeyCode = Keys.Escape) Then
            ExitScreen()
        End If
    End Sub

    Private Sub ILISUtilities_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ScreenLoad()
    End Sub

    Private Sub TracinLevelRB_CheckedChanged(sender As Object, e As EventArgs) Handles TracinLevelRB.CheckedChanged
        HideStatusImages()
        TraceLevelCombo.Enabled = TracinLevelRB.Checked
    End Sub

    Private Sub ExecuteActionButton_Click(sender As Object, e As EventArgs) Handles ExecuteActionButton.Click
        ExecuteAction()
    End Sub

    Private Sub DeleteLISOrdersRB_CheckedChanged(sender As Object, e As EventArgs) Handles DeleteLISOrdersRB.CheckedChanged
        HideStatusImages()
    End Sub

    Private Sub DeleteInternalQueue_CheckedChanged(sender As Object, e As EventArgs) Handles DeleteInternalQueue.CheckedChanged
        HideStatusImages()
    End Sub



End Class