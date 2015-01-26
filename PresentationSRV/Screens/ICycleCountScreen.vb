Option Explicit On
Option Strict On


Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.FwScriptsManagement
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Global.GlobalEnumerates
Imports Biosystems.Ax00.CommunicationsSwFw


Public Class UiCycleCountScreen
    Inherits PesentationLayer.BSAdjustmentBaseForm

    'PENDING

    'QUESTIONS
    'De donde se obtiene la información de los ciclos de cada elemento? En FW o en HW?
    'Qué elementos son los que llevan asociado un contador de cicl

#Region "Declarations"

    Private myManifoldCaption As String = "Manifold"
    Private myFluidicsCaption As String = "Fluidics"
    Private myPhotometricsCaption As String = "Photometrics"

    Private mySelectAllCaption As String = "Select All"
    Private myDeSelectAllCaption As String = "Deselect All"

    Private LanguageID As String

    Private myAnalyzer As AnalyzerManager

    Private WithEvents mySendFwScriptDelegate As SendFwScriptsDelegate
    Private WithEvents myScreenDelegate As CycleCountDelegate

    Private IsInfoExpanded As Boolean = False

#End Region

#Region "Enumerates"

    Private Enum ScreenModes
        _NONE
        INITIATING
        INITIATED
        LOADING
        LOADED
        MODIFYING
        WRITING
        ON_ERROR
    End Enum

#End Region

#Region "Constructor"
    Sub New()

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        DefineScreenLayout()
    End Sub
#End Region


#Region "Properties"

    ''' <summary>
    ''' Screen state
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks>Created: SGM 26/07/2011</remarks>
    Private Property CurrentScreenMode() As ScreenModes
        Get
            Return CurrentScreenModeAttr
        End Get
        Set(ByVal value As ScreenModes)
            If value <> CurrentScreenModeAttr Then
                CurrentScreenModeAttr = value
                MyClass.PrepareUIScreen()

                'QUITAR
                Dim myMDI As Ax00ServiceMainMDI
                myMDI = CType(Me.MdiParent, Ax00ServiceMainMDI)
                If myMDI IsNot Nothing Then
                    myMDI.ErrorStatusLabel.Text = CurrentScreenModeAttr.ToString
                End If

            End If
        End Set
    End Property

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks>Created: SGM 26/07/2011</remarks>
    Private ReadOnly Property IsSomeItemSelected() As Boolean
        Get
            Dim res As Boolean = False

            For Each dgvr As DataGridViewRow In Me.bsCyclesDataGridView.Rows
                If CBool(dgvr.Cells("Selected").Value) = True Then
                    Return True
                End If
            Next
            Return False

        End Get
    End Property





#End Region

#Region "Attributes"

    Private CurrentScreenModeAttr As ScreenModes = ScreenModes._NONE

#End Region

#Region "Flags"

    Private IsLocallySaved As Boolean = False
    Private ExitWhilePendingRequested As Boolean = False
    Private IsCheckingAll As Boolean = False
#End Region


#Region "Public Methods"
    ''' <summary>
    ''' Refresh this specified screen with the information received from the Instrument
    ''' </summary>
    ''' <param name="pRefreshEventType"></param>
    ''' <param name="pRefreshDS"></param>
    ''' <remarks>Created by SGM 28/07/2011</remarks>
    Public Overrides Sub RefreshScreen(ByVal pRefreshEventType As List(Of GlobalEnumerates.UI_RefreshEvents), ByVal pRefreshDS As Biosystems.Ax00.Types.UIRefreshDS)
        Dim myGlobal As New GlobalDataTO
        Try

            If pRefreshEventType.Count > 0 Then
                If pRefreshEventType.Contains(UI_RefreshEvents.HWCYCLES_CHANGED) Then
                    myGlobal = MyClass.myScreenDelegate.RefreshCyclesData(pRefreshEventType, pRefreshDS)
                    If Not myGlobal.HasError Then
                        If MyClass.CurrentScreenMode = ScreenModes.LOADING Then
                            myGlobal = MyClass.LoadCyclesGrid
                        End If
                    End If
                End If
            End If

            If myGlobal.HasError Then MyClass.CurrentScreenMode = ScreenModes.ON_ERROR

            MyClass.PrepareUIScreen()

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".RefreshScreen ", EventLogEntryType.Error, _
                                                                    GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".RefreshScreen", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
            MyClass.CurrentScreenMode = ScreenModes.ON_ERROR
        End Try
    End Sub
#End Region

#Region "Private Methods"

#Region "Common Methods"

    ''' <summary>
    ''' Selects or deselects all the elements displayed in the grid
    ''' </summary>
    ''' <param name="pCheck"></param>
    ''' <returns></returns>
    ''' <remarks>Created: SGM 26/07/2011</remarks>
    Private Function SelectAll(ByVal pCheck As Boolean) As GlobalDataTO

        Dim myGlobal As New GlobalDataTO

        Try

            MyClass.IsCheckingAll = True


            For Each dgvr As DataGridViewRow In Me.bsCyclesDataGridView.Rows
                dgvr.Cells("Selected").Value = pCheck
                MyClass.PaintCheckedRow(dgvr, pCheck)
            Next

            For Each dgvr As DataGridViewRow In Me.bsCyclesDataGridView.Rows
                dgvr.Selected = False
            Next

            MyClass.IsCheckingAll = False


            If pCheck Then
                Me.BsSelectAllCheckbox.Text = MyClass.myDeSelectAllCaption
            Else
                Me.BsSelectAllCheckbox.Text = MyClass.mySelectAllCaption
            End If

            Me.BsWriteButton.Enabled = MyClass.IsSomeItemSelected


        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myGlobal.ErrorMessage = ex.Message
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".SelectAll ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".SelectAll", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
            MyClass.CurrentScreenMode = ScreenModes.ON_ERROR
        End Try

        Return myGlobal

    End Function


#End Region

#Region "Initialization Methods"

    ''' <summary>
    ''' Assigns the specific screen's elements to the common screen layout structure
    ''' </summary>
    ''' <remarks>Created: SGM 26/07/2011</remarks>
    Private Sub DefineScreenLayout()
        Try
            With MyBase.myScreenLayout
                .MessagesPanel.Container = Me.BsMessagesPanel
                .MessagesPanel.Icon = Me.BsMessageImage
                .MessagesPanel.Label = Me.BsMessageLabel

                .AdjustmentPanel.Container = Me.BsCycleAdjustPanel
                .InfoPanel.Container = Me.BsCycleCountInfoPanel
                .InfoPanel.InfoXPS = BsInfoXPSViewer
                .InfoPanel.InfoExpandButton = Me.BsInfoExpandButton
            End With
        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".DefineScreenLayout ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".DefineScreenLayout ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
            MyClass.CurrentScreenMode = ScreenModes.ON_ERROR
        End Try
    End Sub

    ''' <summary>
    ''' Get texts in the current application language for all screen controls
    ''' </summary>
    ''' <remarks>
    ''' Created: SGM 26/07/2011
    ''' </remarks>
    Private Sub GetScreenLabels()
        Try
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            myManifoldCaption = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_MANIFOLD", LanguageID)
            myFluidicsCaption = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_FLUIDICS", LanguageID)
            myPhotometricsCaption = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_PHOTOMETRICS", LanguageID)

            mySelectAllCaption = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_SELECT_ALL", LanguageID)
            myDeSelectAllCaption = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_DESELECT_ALL", LanguageID)

            Me.BsTitleLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_TITLE_CYCLE_COUNTS", LanguageID)
            'Me.BsCycleCountInfoTitle.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_INFO_CYCLE_COUNTS", LanguageID)
            Me.BsCycleCountTitleLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_TITLE_CYCLE_COUNTS", LanguageID)


        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Name & ".GetScreenLabels", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Name & ".GetScreenLabels", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
            MyClass.CurrentScreenMode = ScreenModes.ON_ERROR
        End Try
    End Sub



    ''' <summary>
    ''' Loads the icons and tooltips used for painting the buttons
    ''' </summary>
    ''' <remarks>Created: SGM 26/07/2011</remarks>
    Private Sub PrepareButtons()
        Dim auxIconName As String = ""
        Dim iconPath As String = MyBase.IconsPath
        Dim myGlobal As New GlobalDataTO
        ''Dim myUtil As New Utilities.
        Try
            MyClass.SetButtonImage(BsExitButton, "CANCEL")
            MyClass.SetButtonImage(BsWriteButton, "SAVEALL", 24, 24)

            'Info Button
            auxIconName = GetIconName("RIGHT")
            If System.IO.File.Exists(iconPath & auxIconName) Then
                Me.BsInfoExpandButton.BackgroundImage = ImageUtilities.ImageFromFile(iconPath & auxIconName)
                Me.BsInfoExpandButton.BackgroundImageLayout = ImageLayout.Stretch
            End If

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareButtons", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareButtons", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
            MyClass.CurrentScreenMode = ScreenModes.ON_ERROR
        End Try
    End Sub

    ''' <summary>
    ''' Initializes DataGrid
    ''' </summary>
    ''' <remarks>Created: SGM 26/07/2011</remarks>
    Private Sub InitializeCyclesGrid()
        Try
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate '//15/11/2010 PG
            Dim gridWidth As Integer = Me.bsCyclesDataGridView.Width - 23

            Dim colItemId As New DataGridViewTextBoxColumn
            colItemId.HeaderText = "ItemID"
            colItemId.Name = "ItemId"
            Me.bsCyclesDataGridView.Columns.Add(colItemId)

            Dim colSubSystem As New DataGridViewTextBoxColumn
            colSubSystem.HeaderText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_CYCLES_SubSystem", LanguageID) '10/11/2010 PG
            colSubSystem.Name = "SubSystem"
            Me.bsCyclesDataGridView.Columns.Add(colSubSystem)

            Dim colItemName As New DataGridViewTextBoxColumn
            colItemName.HeaderText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_CYCLES_ElementId", LanguageID)
            colItemName.Name = "ItemName"
            Me.bsCyclesDataGridView.Columns.Add(colItemName)

            Dim colValue As New DataGridViewTextBoxColumn
            colValue.HeaderText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_CYCLES_Value", LanguageID)
            colValue.Name = "Value"
            Me.bsCyclesDataGridView.Columns.Add(colValue)

            Dim colUnits As New DataGridViewTextBoxColumn
            colUnits.HeaderText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_CYCLES_Units", LanguageID)
            colUnits.Name = "Units"
            Me.bsCyclesDataGridView.Columns.Add(colUnits)

            Dim colSelected As New DataGridViewCheckBoxColumn
            'colPars.HeaderText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_CYCLES_SubSystem", LanguageID)
            colSelected.Name = "Selected"
            Me.bsCyclesDataGridView.Columns.Add(colSelected)

            Dim colNewValue As New DataGridViewTextBoxColumn
            colNewValue.HeaderText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_CYCLES_NewValue", LanguageID)
            colNewValue.Name = "NewValue"
            Me.bsCyclesDataGridView.Columns.Add(colNewValue)

            Me.bsCyclesDataGridView.Columns("ItemID").Visible = False
            Me.bsCyclesDataGridView.Columns("SubSystem").Width = CInt(12 * gridWidth / 100)
            Me.bsCyclesDataGridView.Columns("ItemName").Width = CInt(35 * gridWidth / 100)
            Me.bsCyclesDataGridView.Columns("Value").Width = CInt(12 * gridWidth / 100)
            Me.bsCyclesDataGridView.Columns("Units").Width = CInt(18 * gridWidth / 100)
            Me.bsCyclesDataGridView.Columns("Selected").Width = CInt(11 * gridWidth / 100)
            Me.bsCyclesDataGridView.Columns("NewValue").Width = CInt(12 * gridWidth / 100)

            Me.bsCyclesDataGridView.Columns("ItemID").SortMode = DataGridViewColumnSortMode.Automatic
            Me.bsCyclesDataGridView.Columns("SubSystem").SortMode = DataGridViewColumnSortMode.Automatic
            Me.bsCyclesDataGridView.Columns("ItemName").SortMode = DataGridViewColumnSortMode.Automatic
            Me.bsCyclesDataGridView.Columns("Value").SortMode = DataGridViewColumnSortMode.NotSortable
            Me.bsCyclesDataGridView.Columns("Units").SortMode = DataGridViewColumnSortMode.Automatic
            Me.bsCyclesDataGridView.Columns("Selected").SortMode = DataGridViewColumnSortMode.Automatic
            Me.bsCyclesDataGridView.Columns("NewValue").SortMode = DataGridViewColumnSortMode.NotSortable

            Me.bsCyclesDataGridView.Columns("ItemID").ReadOnly = True
            Me.bsCyclesDataGridView.Columns("SubSystem").ReadOnly = True
            Me.bsCyclesDataGridView.Columns("ItemName").ReadOnly = True
            Me.bsCyclesDataGridView.Columns("Value").ReadOnly = True
            Me.bsCyclesDataGridView.Columns("Units").ReadOnly = True
            Me.bsCyclesDataGridView.Columns("Selected").ReadOnly = False
            Me.bsCyclesDataGridView.Columns("NewValue").ReadOnly = False

            Me.bsCyclesDataGridView.Columns("ItemID").HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter
            Me.bsCyclesDataGridView.Columns("SubSystem").HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter
            Me.bsCyclesDataGridView.Columns("ItemName").HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter
            Me.bsCyclesDataGridView.Columns("Value").HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter
            Me.bsCyclesDataGridView.Columns("Units").HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter
            Me.bsCyclesDataGridView.Columns("Selected").HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter
            Me.bsCyclesDataGridView.Columns("NewValue").HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter

            Me.bsCyclesDataGridView.Columns("ItemID").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            Me.bsCyclesDataGridView.Columns("SubSystem").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
            Me.bsCyclesDataGridView.Columns("ItemName").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
            Me.bsCyclesDataGridView.Columns("Value").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            Me.bsCyclesDataGridView.Columns("Units").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
            Me.bsCyclesDataGridView.Columns("Selected").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            Me.bsCyclesDataGridView.Columns("NewValue").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight


            Me.bsCyclesDataGridView.Columns("Value").DefaultCellStyle.Font = New Font(Me.BsFontLabel.Font, FontStyle.Regular)
            Me.bsCyclesDataGridView.Columns("NewValue").DefaultCellStyle.Font = New Font(Me.BsFontLabel.Font, FontStyle.Bold)

            Me.bsCyclesDataGridView.Columns("NewValue").DefaultCellStyle.ForeColor = Color.SteelBlue

            Me.bsCyclesDataGridView.EditMode = DataGridViewEditMode.EditOnEnter

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & " InitializeExperimentalsGrid ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message)
            MyClass.CurrentScreenMode = ScreenModes.ON_ERROR
        End Try
    End Sub

    ''' <summary>
    ''' Gets the corresponding text related to the informed cycle item's type
    ''' </summary>
    ''' <param name="pItemID"></param>
    ''' <param name="pLanguageID"></param>
    ''' <returns></returns>
    ''' <remarks>Created: SGM 26/07/2011</remarks>
    Private Function GetCountTypeName(ByVal pItemID As String, ByVal pLanguageID As String) As String

        Dim res As String = ""
        Dim myMLRD As New MultilanguageResourcesDelegate

        Try

            res = myMLRD.GetResourceText(Nothing, "LBL_SRV_" & pItemID, pLanguageID)

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".GetCountTypeName", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".GetCountTypeName", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
            MyClass.CurrentScreenMode = ScreenModes.ON_ERROR
        End Try

        Return res

    End Function

    ''' <summary>
    ''' Manages screen loading operation
    ''' </summary>
    ''' <remarks>Created: SGM 26/07/2011</remarks>
    Private Sub LoadScreen()

        Dim myGlobal As New GlobalDataTO

        Try
            MyClass.IsLocallySaved = False

            MyClass.CurrentScreenMode = ScreenModes.INITIATING

            MyClass.PrepareButtons()
            MyClass.GetScreenLabels()

            Me.BsSelectAllCheckbox.Text = MyClass.mySelectAllCaption

            MyClass.InitializeCyclesGrid()

            MyClass.myAnalyzer = CType(AppDomain.CurrentDomain.GetData("GlobalAnalyzerManager"), AnalyzerManager)
            MyClass.mySendFwScriptDelegate = New SendFwScriptsDelegate(myAnalyzer)

            'MyClass.CurrentCyclesData = CyclesDS

            MyClass.CurrentScreenMode = ScreenModes.INITIATED



        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".LoadScreen", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".LoadScreen", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
            MyClass.CurrentScreenMode = ScreenModes.ON_ERROR
        End Try
    End Sub

    ''' <summary>
    ''' Manages screen exiting operation
    ''' </summary>
    ''' <remarks>Created: SGM 26/07/2011</remarks>
    Private Sub ExitScreen()
        Dim myGlobal As New GlobalDataTO
        Dim dialogResultToReturn As DialogResult = Windows.Forms.DialogResult.No
        Dim waitForScripts As Boolean = False
        Try
            If MyClass.CurrentScreenMode = ScreenModes.ON_ERROR Then
                Me.Close()
                Exit Sub
            End If


            dialogResultToReturn = MyBase.ShowMessage(GetMessageText(Messages.SAVE_PENDING.ToString), Messages.SAVE_PENDING.ToString)

            If dialogResultToReturn = Windows.Forms.DialogResult.Yes Then
                MyClass.ExitWhilePendingRequested = True
                myGlobal = MyClass.WriteCyclesToAnalyzer
            End If

            If Not myGlobal.HasError Then
                If Not MyClass.ExitWhilePendingRequested Then
                    myGlobal = MyBase.CloseForm()
                    If Not myGlobal.HasError Then
                        Me.Close()
                    End If
                End If
            End If

            If myGlobal.HasError Then MyClass.CurrentScreenMode = ScreenModes.ON_ERROR

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".ExitScreen ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".ExitScreen ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
            MyClass.CurrentScreenMode = ScreenModes.ON_ERROR
        End Try
    End Sub

#End Region


#Region "Grid Methods"

    ''' <summary>
    ''' Load the data with which the datagrid is filled
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>Created: SGM 26/07/2011</remarks>
    Private Function LoadCyclesGrid() As GlobalDataTO

        Dim myGlobal As New GlobalDataTO

        Try

            Me.bsCyclesDataGridView.Rows.Clear()

            'If MyBase.SimulationMode Then

            MyClass.SimulateLoadCycles()

            'Else

            'End If

            For Each DR As UIRefreshDS.CyclesValuesChangedRow In MyClass.myScreenDelegate.CurrentCyclesCountItemsDT.Rows

                Me.bsCyclesDataGridView.Rows.Add()

                If Me.bsCyclesDataGridView.Rows.Count > 0 Then
                    Dim dgvr As DataGridViewRow = Me.bsCyclesDataGridView.Rows(Me.bsCyclesDataGridView.Rows.Count - 1)

                    With dgvr
                        .Cells("SubSystem").Value = DR.SubSystemID
                        .Cells("ItemID").Value = DR.ItemID
                        .Cells("ItemName").Value = MyBase.GetHWElementsName(DR.ItemID, LanguageID)
                        .Cells("Units").Value = DR.CycleUnits
                        .Cells("Selected").Value = False

                        .Cells("Value").Value = DR.CyclesCount
                        .Cells("Value").Style.Font = New Font(Me.BsFontLabel.Font, FontStyle.Regular)
                        .Cells("Value").Style.ForeColor = Color.DimGray

                        .Cells("NewValue").Value = "0"
                        .Cells("NewValue").Style.Font = New Font(Me.BsFontLabel.Font, FontStyle.Bold)
                        .Cells("NewValue").Style.ForeColor = Color.SteelBlue

                        MyClass.PaintCheckedRow(dgvr, False)

                        .Selected = False

                    End With

                End If

            Next DR

            If Me.bsCyclesDataGridView.SelectedRows.Count > 0 Then
                Me.bsCyclesDataGridView.SelectedRows(0).Selected = False
            End If

            MyClass.CurrentScreenMode = ScreenModes.LOADED

        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myGlobal.ErrorMessage = ex.Message
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".LoadCyclesGrid", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".LoadCyclesGrid", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
            MyClass.CurrentScreenMode = ScreenModes.ON_ERROR
        End Try

        Return myGlobal

    End Function

    ''' <summary>
    ''' Paints each cell of the indicated row according of the item's status
    ''' </summary>
    ''' <param name="pRow"></param>
    ''' <param name="pChecked"></param>
    ''' <remarks>Created: SGM 26/07/2011</remarks>
    Private Sub PaintCheckedRow(ByVal pRow As DataGridViewRow, ByVal pChecked As Boolean)
        Try
            If pRow IsNot Nothing Then

                If pChecked Then

                    pRow.Cells("SubSystem").Style.BackColor = Color.LightGray
                    pRow.Cells("ItemName").Style.BackColor = Color.LightGray
                    pRow.Cells("Value").Style.BackColor = Color.LightGray
                    pRow.Cells("Units").Style.BackColor = Color.LightGray
                    pRow.Cells("Selected").Style.BackColor = Color.LightGray
                    pRow.Cells("NewValue").Style.BackColor = Color.LightGray

                Else
                    pRow.Cells("SubSystem").Style.BackColor = Color.WhiteSmoke
                    pRow.Cells("ItemName").Style.BackColor = Color.White
                    pRow.Cells("Value").Style.BackColor = Color.WhiteSmoke
                    pRow.Cells("Units").Style.BackColor = Color.White
                    pRow.Cells("Selected").Style.BackColor = Color.WhiteSmoke
                    pRow.Cells("NewValue").Style.BackColor = Color.White
                    pRow.Cells("NewValue").ReadOnly = True

                End If

                If MyClass.CurrentScreenMode = ScreenModes.MODIFYING Then
                    pRow.Cells("NewValue").Style.BackColor = Color.LightYellow
                End If

            End If

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".PaintCheckedRow", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PaintCheckedRow", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

#End Region

#Region "UI Prepare Methods"



    ''' <summary>
    ''' Prepares User Interface according to the screen's status
    ''' </summary>
    ''' <remarks>Created: SGM 26/07/2011</remarks>
    Private Sub PrepareUIScreen()
        Try

            Select Case MyClass.CurrentScreenMode

                Case ScreenModes.INITIATING
                    MyClass.PrepareUIInitiating()

                Case ScreenModes.INITIATED
                    MyClass.PrepareUIInitiated()

                Case ScreenModes.LOADING
                    MyClass.PrepareUILoading()

                Case ScreenModes.LOADED
                    MyClass.PrepareUILoaded()

                Case ScreenModes.MODIFYING
                    MyClass.PrepareUIModifying()

                Case ScreenModes.WRITING
                    MyClass.PrepareUIWriting()


                Case ScreenModes.ON_ERROR
                    MyClass.PrepareUIOnError()

            End Select

            If Not MyBase.SimulationMode And Ax00ServiceMainMDI.MDIAnalyzerManager.AnalyzerStatus = AnalyzerManagerStatus.SLEEPING Then
                MyClass.PrepareUIOnError()
                MyBase.DisplayMessage("")
            End If

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareUIPrepareScreen", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareUIPrepareScreen", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Prepares User Interface when Initiating
    ''' </summary>
    ''' <remarks>Created: SGM 26/07/2011</remarks>
    Private Sub PrepareUIInitiating()
        Try
            Me.Cursor = Cursors.WaitCursor

            Me.bsCyclesDataGridView.Enabled = False
            If Me.bsCyclesDataGridView.Columns.Count > 0 Then
                Me.bsCyclesDataGridView.Columns("ItemID").ReadOnly = True
                Me.bsCyclesDataGridView.Columns("SubSystem").ReadOnly = True
                Me.bsCyclesDataGridView.Columns("ItemName").ReadOnly = True
                Me.bsCyclesDataGridView.Columns("Value").ReadOnly = True
                Me.bsCyclesDataGridView.Columns("Units").ReadOnly = True
                Me.bsCyclesDataGridView.Columns("Selected").ReadOnly = True
                Me.bsCyclesDataGridView.Columns("NewValue").ReadOnly = True
            End If

            Me.BsWriteButton.Enabled = False
            Me.BsSelectAllCheckbox.Enabled = False

            Me.BsExitButton.Enabled = False

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareUIInitiating", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareUIInitiating", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
            MyClass.CurrentScreenMode = ScreenModes.ON_ERROR
        End Try
    End Sub

    ''' <summary>
    ''' Prepares User Interface once Initiated
    ''' </summary>
    ''' <remarks>Created: SGM 26/07/2011</remarks>
    Private Sub PrepareUIInitiated()
        Try
            Me.Cursor = Cursors.Default

            Me.bsCyclesDataGridView.Enabled = False
            If Me.bsCyclesDataGridView.Columns.Count > 0 Then
                Me.bsCyclesDataGridView.Columns("ItemID").ReadOnly = True
                Me.bsCyclesDataGridView.Columns("SubSystem").ReadOnly = True
                Me.bsCyclesDataGridView.Columns("ItemName").ReadOnly = True
                Me.bsCyclesDataGridView.Columns("Value").ReadOnly = True
                Me.bsCyclesDataGridView.Columns("Units").ReadOnly = True
                Me.bsCyclesDataGridView.Columns("Selected").ReadOnly = True
                Me.bsCyclesDataGridView.Columns("NewValue").ReadOnly = True
            End If

            Me.BsWriteButton.Enabled = False
            Me.BsSelectAllCheckbox.Enabled = False

            Me.BsExitButton.Enabled = True

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareUIInitiated", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareUIInitiated", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
            MyClass.CurrentScreenMode = ScreenModes.ON_ERROR
        End Try
    End Sub

    ''' <summary>
    ''' Prepares User Interface when Loading
    ''' </summary>
    ''' <remarks>Created: SGM 26/07/2011</remarks>
    Private Sub PrepareUILoading()
        Try
            Me.Cursor = Cursors.WaitCursor

            Me.bsCyclesDataGridView.Enabled = False
            Me.bsCyclesDataGridView.Columns("ItemID").ReadOnly = True
            Me.bsCyclesDataGridView.Columns("SubSystem").ReadOnly = True
            Me.bsCyclesDataGridView.Columns("ItemName").ReadOnly = True
            Me.bsCyclesDataGridView.Columns("Value").ReadOnly = True
            Me.bsCyclesDataGridView.Columns("Units").ReadOnly = True
            Me.bsCyclesDataGridView.Columns("Selected").ReadOnly = True
            Me.bsCyclesDataGridView.Columns("NewValue").ReadOnly = True

            Me.BsWriteButton.Enabled = False
            Me.BsSelectAllCheckbox.Enabled = False

            Me.BsExitButton.Enabled = False

            Me.BsWaitProgressBar.Visible = True
            'Me.WaitProgressBar.TimeForWait = 5000 ' MyClass.TimeForWait Pending to Spec
            'Me.WaitProgressBar.StartWaiting()


        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareUILoading", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareUILoading", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
            MyClass.CurrentScreenMode = ScreenModes.ON_ERROR
        End Try
    End Sub

    ''' <summary>
    ''' Prepares User Interface once Loaded
    ''' </summary>
    ''' <remarks>Created: SGM 26/07/2011</remarks>
    Private Sub PrepareUILoaded()
        Try
            Me.BsWaitProgressBar.Visible = False
            'Me.WaitProgressBar.StopWaiting()

            Me.Cursor = Cursors.Default

            Me.bsCyclesDataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect
            Me.bsCyclesDataGridView.Enabled = True
            Me.bsCyclesDataGridView.Columns("ItemID").ReadOnly = True
            Me.bsCyclesDataGridView.Columns("SubSystem").ReadOnly = True
            Me.bsCyclesDataGridView.Columns("ItemName").ReadOnly = True
            Me.bsCyclesDataGridView.Columns("Value").ReadOnly = True
            Me.bsCyclesDataGridView.Columns("Units").ReadOnly = True
            Me.bsCyclesDataGridView.Columns("Selected").ReadOnly = False
            Me.bsCyclesDataGridView.Columns("NewValue").ReadOnly = True


            Me.BsWriteButton.Enabled = IsSomeItemSelected
            Me.BsSelectAllCheckbox.Enabled = (Me.bsCyclesDataGridView.RowCount > 0)

            Me.BsExitButton.Enabled = True


        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareUILoaded", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareUILoaded", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
            MyClass.CurrentScreenMode = ScreenModes.ON_ERROR

        End Try
    End Sub

    ''' <summary>
    ''' Prepares User Interface while cell is modifying
    ''' </summary>
    ''' <remarks>Created: SGM 26/07/2011</remarks>
    Private Sub PrepareUIModifying()
        Try
            Me.Cursor = Cursors.Default


            Me.bsCyclesDataGridView.Enabled = True
            Me.bsCyclesDataGridView.Columns("ItemID").ReadOnly = True
            Me.bsCyclesDataGridView.Columns("SubSystem").ReadOnly = True
            Me.bsCyclesDataGridView.Columns("ItemName").ReadOnly = True
            Me.bsCyclesDataGridView.Columns("Value").ReadOnly = True
            Me.bsCyclesDataGridView.Columns("Units").ReadOnly = True
            Me.bsCyclesDataGridView.Columns("Selected").ReadOnly = False
            Me.bsCyclesDataGridView.Columns("NewValue").ReadOnly = False

            Me.BsWriteButton.Enabled = IsSomeItemSelected
            Me.BsSelectAllCheckbox.Enabled = (Me.bsCyclesDataGridView.RowCount > 0)

            Me.BsExitButton.Enabled = True

            Me.bsCyclesDataGridView.SelectionMode = DataGridViewSelectionMode.CellSelect

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareUIModifying", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareUIModifying", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
            MyClass.CurrentScreenMode = ScreenModes.ON_ERROR
        End Try
    End Sub

    ''' <summary>
    ''' Prepares User Interface while Cycles are being written to the Analyzer
    ''' </summary>
    ''' <remarks>Created: SGM 26/07/2011</remarks>
    Private Sub PrepareUIWriting()
        Try
            Me.Cursor = Cursors.WaitCursor

            Me.bsCyclesDataGridView.SelectionMode = DataGridViewSelectionMode.CellSelect
            Me.bsCyclesDataGridView.Enabled = False
            Me.bsCyclesDataGridView.Columns("ItemID").ReadOnly = True
            Me.bsCyclesDataGridView.Columns("SubSystem").ReadOnly = True
            Me.bsCyclesDataGridView.Columns("ItemName").ReadOnly = True
            Me.bsCyclesDataGridView.Columns("Value").ReadOnly = True
            Me.bsCyclesDataGridView.Columns("Units").ReadOnly = True
            Me.bsCyclesDataGridView.Columns("Selected").ReadOnly = True
            Me.bsCyclesDataGridView.Columns("NewValue").ReadOnly = True

            Me.BsWriteButton.Enabled = False
            Me.BsSelectAllCheckbox.Enabled = False

            Me.BsExitButton.Enabled = False

            Me.BsWaitProgressBar.Visible = True
            'Me.WaitProgressBar.TimeForWait = 5000 ' MyClass.TimeForWait  Pending To Spec
            'Me.WaitProgressBar.StartWaiting()

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareUIWriting", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareUIWriting", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
            MyClass.CurrentScreenMode = ScreenModes.ON_ERROR
        End Try
    End Sub



    ''' <summary>
    ''' Prepares User Interface when Error
    ''' </summary>
    ''' <remarks>Created: SGM 26/07/2011</remarks>
    Private Sub PrepareUIOnError()
        Try

            Me.BsWaitProgressBar.Visible = False
            'Me.WaitProgressBar.StopWaiting()

            Me.bsCyclesDataGridView.Enabled = False

            Me.BsWriteButton.Enabled = False
            Me.BsSelectAllCheckbox.Enabled = False

            Me.BsExitButton.Enabled = True


            Me.Cursor = Cursors.Default

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareUIOnError", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareUIOnError", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
            MyClass.CurrentScreenMode = ScreenModes.ON_ERROR
        End Try
    End Sub

#End Region


#Region "Read/Write methods"
    ''' <summary>
    ''' Requests cycles from analyzer
    ''' </summary>
    ''' <remarks>Created by SGM 27/07/2011</remarks>
    Private Function ReadCyclesFromAnalyzer() As GlobalDataTO
        Dim myGlobal As New GlobalDataTO
        Try

            MyBase.ReadCycles()

            MyClass.CurrentScreenMode = ScreenModes.LOADING

            MyBase.DisplayMessage(Messages.SRV_READ_CYCLES.ToString)

            ' Manage FwScripts must to be sent at load screen

            'PENDING
            'If MyBase.SimulationMode Then
            'MyBase.DisplaySimulationMessage("Request Cycles from Instrument...")
            MyClass.SimulateLoadCycles()
            MyClass.LoadCyclesGrid()
            MyClass.SimulateWaiting(5000)
            MyBase.myServiceMDI.Focus()
            MyClass.CurrentScreenMode = ScreenModes.LOADED
            'MyBase.DisplaySimulationMessage("Cycles successfully readed.")
            MyBase.DisplayMessage(Messages.SRV_CYCLES_READED.ToString)
            'Else
            'myGlobal = myScreenDelegate.SendREAD_CYCLES(GlobalEnumerates.Ax00Adjustsments.ALL)
            'End If

            If myGlobal.HasError Then
                MyClass.CurrentScreenMode = ScreenModes.ON_ERROR
            End If


        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myGlobal.ErrorMessage = ex.Message
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".ReadCyclesFromAnalyzer ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".ReadCyclesFromAnalyzer", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
            MyClass.CurrentScreenMode = ScreenModes.ON_ERROR
        End Try

        Return myGlobal

    End Function


    ''' <summary>
    ''' Writes cycles to analyzer
    ''' </summary>
    ''' <remarks>Created by SGM 27/07/2011</remarks>
    Private Function WriteCyclesToAnalyzer() As GlobalDataTO
        Dim myGlobal As New GlobalDataTO
        Try

            MyBase.WriteCycles()

            MyClass.CurrentScreenMode = ScreenModes.WRITING

            MyBase.DisplayMessage(Messages.SRV_WRITE_CYCLES.ToString)

            'PENDING
            'If MyBase.SimulationMode Then
            'MyBase.DisplaySimulationMessage("Write Cycles to Instrument...")
            MyClass.SimulateWaiting(5000)
            MyBase.myServiceMDI.Focus()
            MyClass.CurrentScreenMode = ScreenModes.LOADED
            'MyBase.DisplaySimulationMessage("Cycles successfully written.")
            MyBase.DisplayMessage(Messages.SRV_CYCLES_WRITTEN.ToString)
            'Else

            'myGlobal = MyClass.CollectCycleCountToUpdate
            'If Not myGlobal.HasError And myGlobal.SetDatos IsNot Nothing Then
            '    Dim myNewCyclesDT As New UIRefreshDS.CyclesValuesChangedDataTable
            '    myNewCyclesDT = CType(myGlobal.SetDatos, UIRefreshDS.CyclesValuesChangedDataTable)

            '    'PENDING TO SPEC How to send the data to Analyzer
            '    'myGlobal = myScreenDelegate.SendWRITE_CYCLES(GlobalEnumerates.)

            'End If

            'End If

            If Not myGlobal.HasError Then
                myGlobal = MyClass.ReadCyclesFromAnalyzer
            End If

            If myGlobal.HasError Then MyClass.CurrentScreenMode = ScreenModes.ON_ERROR

        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myGlobal.ErrorMessage = ex.Message
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".WriteCyclesToAnalyzer ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".WriteCyclesToAnalyzer", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
            MyClass.CurrentScreenMode = ScreenModes.ON_ERROR
        End Try

        Return myGlobal

    End Function

    ''' <summary>
    ''' Collects all the items selected to be written to the Analyzer
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>Created: SGM 26/07/2011</remarks>
    Private Function CollectCycleCountToUpdate() As GlobalDataTO

        Dim myGlobal As New GlobalDataTO

        Try
            Dim myNewCyclesCountItemsDT As New UIRefreshDS.CyclesValuesChangedDataTable

            For Each dgvr As DataGridViewRow In Me.bsCyclesDataGridView.Rows
                If CBool(dgvr.Cells("Selected").Value) Then
                    Dim myRow As UIRefreshDS.CyclesValuesChangedRow
                    myRow = myNewCyclesCountItemsDT.NewCyclesValuesChangedRow

                    With myRow
                        .BeginEdit()
                        .ItemID = CStr(dgvr.Cells("ItemID").Value)
                        .SubSystemID = CStr(dgvr.Cells("SubSystem").Value)
                        .CycleUnits = CStr(dgvr.Cells("Units").Value)
                        .CyclesCount = CLng(dgvr.Cells("NewValue").Value)
                        .EndEdit()
                    End With

                    myNewCyclesCountItemsDT.AddCyclesValuesChangedRow(myRow)

                End If
            Next

            myNewCyclesCountItemsDT.AcceptChanges()

            myGlobal.SetDatos = myNewCyclesCountItemsDT

        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myGlobal.ErrorMessage = ex.Message
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".CollectCycleCountToUpdate ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".CollectCycleCountToUpdate", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
            MyClass.CurrentScreenMode = ScreenModes.ON_ERROR
        End Try

        Return myGlobal

    End Function

#End Region

#Region "Simulation Methods"

    ''' <summary>
    ''' Simulates loading cycles data
    ''' </summary>
    ''' <remarks>Created: SGM 26/07/2011</remarks>
    Private Sub SimulateLoadCycles()
        Try

            MyClass.myScreenDelegate.CurrentCyclesCountItemsDT = New UIRefreshDS.CyclesValuesChangedDataTable

            Dim myRow As UIRefreshDS.CyclesValuesChangedRow

            'MANIFOLD
            '************************************************************************** 

            'JE1_B1
            myRow = MyClass.myScreenDelegate.CurrentCyclesCountItemsDT.NewCyclesValuesChangedRow
            With myRow
                .BeginEdit()
                .SubSystemID = MyClass.myManifoldCaption
                .ItemID = CYCLE_ELEMENTS.JE1_B1_CYC.ToString
                .CycleUnits = MyClass.GetCountTypeName(CYCLE_UNITS.SWITCHES.ToString, LanguageID)
                .CyclesCount = CInt(10000 * Rnd(100000))
                .EndEdit()
            End With
            MyClass.myScreenDelegate.CurrentCyclesCountItemsDT.Rows.Add(myRow)

            'JE1_B2
            myRow = MyClass.myScreenDelegate.CurrentCyclesCountItemsDT.NewCyclesValuesChangedRow
            With myRow
                .BeginEdit()
                .SubSystemID = MyClass.myManifoldCaption
                .ItemID = CYCLE_ELEMENTS.JE1_B2_CYC.ToString
                .CycleUnits = MyClass.GetCountTypeName(CYCLE_UNITS.SWITCHES.ToString, LanguageID)
                .CyclesCount = CInt(10000 * Rnd(100000))
                .EndEdit()
            End With
            MyClass.myScreenDelegate.CurrentCyclesCountItemsDT.Rows.Add(myRow)

            'JE1_B3
            myRow = MyClass.myScreenDelegate.CurrentCyclesCountItemsDT.NewCyclesValuesChangedRow
            With myRow
                .BeginEdit()
                .SubSystemID = MyClass.myManifoldCaption
                .ItemID = CYCLE_ELEMENTS.JE1_B3_CYC.ToString
                .CycleUnits = MyClass.GetCountTypeName(CYCLE_UNITS.SWITCHES.ToString, LanguageID)
                .CyclesCount = CInt(10000 * Rnd(100000))
                .EndEdit()
            End With
            MyClass.myScreenDelegate.CurrentCyclesCountItemsDT.Rows.Add(myRow)

            'JE1_EV1
            myRow = MyClass.myScreenDelegate.CurrentCyclesCountItemsDT.NewCyclesValuesChangedRow
            With myRow
                .BeginEdit()
                .SubSystemID = MyClass.myManifoldCaption
                .ItemID = CYCLE_ELEMENTS.JE1_EV1_CYC.ToString
                .CycleUnits = MyClass.GetCountTypeName(CYCLE_UNITS.SWITCHES.ToString, LanguageID)
                .CyclesCount = CInt(10000 * Rnd(100000))
                .EndEdit()
            End With
            MyClass.myScreenDelegate.CurrentCyclesCountItemsDT.Rows.Add(myRow)

            'JE1_EV2
            myRow = MyClass.myScreenDelegate.CurrentCyclesCountItemsDT.NewCyclesValuesChangedRow
            With myRow
                .BeginEdit()
                .SubSystemID = MyClass.myManifoldCaption
                .ItemID = CYCLE_ELEMENTS.JE1_EV2_CYC.ToString
                .CycleUnits = MyClass.GetCountTypeName(CYCLE_UNITS.SWITCHES.ToString, LanguageID)
                .CyclesCount = CInt(10000 * Rnd(100000))
                .EndEdit()
            End With
            MyClass.myScreenDelegate.CurrentCyclesCountItemsDT.Rows.Add(myRow)

            'JE1_EV3
            myRow = MyClass.myScreenDelegate.CurrentCyclesCountItemsDT.NewCyclesValuesChangedRow
            With myRow
                .BeginEdit()
                .SubSystemID = MyClass.myManifoldCaption
                .ItemID = CYCLE_ELEMENTS.JE1_EV3_CYC.ToString
                .CycleUnits = MyClass.GetCountTypeName(CYCLE_UNITS.SWITCHES.ToString, LanguageID)
                .CyclesCount = CInt(10000 * Rnd(100000))
                .EndEdit()
            End With
            MyClass.myScreenDelegate.CurrentCyclesCountItemsDT.Rows.Add(myRow)

            'JE1_EV4
            myRow = MyClass.myScreenDelegate.CurrentCyclesCountItemsDT.NewCyclesValuesChangedRow
            With myRow
                .BeginEdit()
                .SubSystemID = MyClass.myManifoldCaption
                .ItemID = CYCLE_ELEMENTS.JE1_EV4_CYC.ToString
                .CycleUnits = MyClass.GetCountTypeName(CYCLE_UNITS.SWITCHES.ToString, LanguageID)
                .CyclesCount = CInt(10000 * Rnd(100000))
                .EndEdit()
            End With
            MyClass.myScreenDelegate.CurrentCyclesCountItemsDT.Rows.Add(myRow)

            'JE1_EV5
            myRow = MyClass.myScreenDelegate.CurrentCyclesCountItemsDT.NewCyclesValuesChangedRow
            With myRow
                .BeginEdit()
                .SubSystemID = MyClass.myManifoldCaption
                .ItemID = CYCLE_ELEMENTS.JE1_EV5_CYC.ToString
                .CycleUnits = MyClass.GetCountTypeName(CYCLE_UNITS.SWITCHES.ToString, LanguageID)
                .CyclesCount = CInt(10000 * Rnd(100000))
                .EndEdit()
            End With
            MyClass.myScreenDelegate.CurrentCyclesCountItemsDT.Rows.Add(myRow)

            'JE1_MS
            myRow = MyClass.myScreenDelegate.CurrentCyclesCountItemsDT.NewCyclesValuesChangedRow
            With myRow
                .BeginEdit()
                .SubSystemID = MyClass.myManifoldCaption
                .ItemID = CYCLE_ELEMENTS.JE1_MS_CYC.ToString
                .CycleUnits = MyClass.GetCountTypeName(CYCLE_UNITS.x100STEPS.ToString, LanguageID)
                .CyclesCount = CInt(10000 * Rnd(100000))
                .EndEdit()
            End With
            MyClass.myScreenDelegate.CurrentCyclesCountItemsDT.Rows.Add(myRow)

            'JE1_MR1
            myRow = MyClass.myScreenDelegate.CurrentCyclesCountItemsDT.NewCyclesValuesChangedRow
            With myRow
                .BeginEdit()
                .SubSystemID = MyClass.myManifoldCaption
                .ItemID = CYCLE_ELEMENTS.JE1_MR1_CYC.ToString
                .CycleUnits = MyClass.GetCountTypeName(CYCLE_UNITS.x100STEPS.ToString, LanguageID)
                .CyclesCount = CInt(10000 * Rnd(100000))
                .EndEdit()
            End With
            MyClass.myScreenDelegate.CurrentCyclesCountItemsDT.Rows.Add(myRow)

            'JE1_MR2
            myRow = MyClass.myScreenDelegate.CurrentCyclesCountItemsDT.NewCyclesValuesChangedRow
            With myRow
                .BeginEdit()
                .SubSystemID = MyClass.myManifoldCaption
                .ItemID = CYCLE_ELEMENTS.JE1_MR2_CYC.ToString
                .CycleUnits = MyClass.GetCountTypeName(CYCLE_UNITS.x100STEPS.ToString, LanguageID)
                .CyclesCount = CInt(10000 * Rnd(100000))
                .EndEdit()
            End With
            MyClass.myScreenDelegate.CurrentCyclesCountItemsDT.Rows.Add(myRow)


            'SF1_MS
            myRow = MyClass.myScreenDelegate.CurrentCyclesCountItemsDT.NewCyclesValuesChangedRow
            With myRow
                .BeginEdit()
                .SubSystemID = MyClass.myFluidicsCaption
                .ItemID = CYCLE_ELEMENTS.SF1_MS_CYC.ToString
                .CycleUnits = MyClass.GetCountTypeName(CYCLE_UNITS.x1000STEPS.ToString, LanguageID)
                .CyclesCount = CInt(10000 * Rnd(100000))
                .EndEdit()
            End With
            MyClass.myScreenDelegate.CurrentCyclesCountItemsDT.Rows.Add(myRow)

            'SF1_B1
            myRow = MyClass.myScreenDelegate.CurrentCyclesCountItemsDT.NewCyclesValuesChangedRow
            With myRow
                .BeginEdit()
                .SubSystemID = MyClass.myFluidicsCaption
                .ItemID = CYCLE_ELEMENTS.SF1_B1_CYC.ToString
                .CycleUnits = MyClass.GetCountTypeName(CYCLE_UNITS.SWITCHES.ToString, LanguageID)
                .CyclesCount = CInt(10000 * Rnd(100000))
                .EndEdit()
            End With
            MyClass.myScreenDelegate.CurrentCyclesCountItemsDT.Rows.Add(myRow)

            'SF1_B2
            myRow = MyClass.myScreenDelegate.CurrentCyclesCountItemsDT.NewCyclesValuesChangedRow
            With myRow
                .BeginEdit()
                .SubSystemID = MyClass.myFluidicsCaption
                .ItemID = CYCLE_ELEMENTS.SF1_B2_CYC.ToString
                .CycleUnits = MyClass.GetCountTypeName(CYCLE_UNITS.SWITCHES.ToString, LanguageID)
                .CyclesCount = CInt(10000 * Rnd(100000))
                .EndEdit()
            End With
            MyClass.myScreenDelegate.CurrentCyclesCountItemsDT.Rows.Add(myRow)

            'SF1_B3
            myRow = MyClass.myScreenDelegate.CurrentCyclesCountItemsDT.NewCyclesValuesChangedRow
            With myRow
                .BeginEdit()
                .SubSystemID = MyClass.myFluidicsCaption
                .ItemID = CYCLE_ELEMENTS.SF1_B3_CYC.ToString
                .CycleUnits = MyClass.GetCountTypeName(CYCLE_UNITS.SWITCHES.ToString, LanguageID)
                .CyclesCount = CInt(10000 * Rnd(100000))
                .EndEdit()
            End With
            MyClass.myScreenDelegate.CurrentCyclesCountItemsDT.Rows.Add(myRow)

            'SF1_B4
            myRow = MyClass.myScreenDelegate.CurrentCyclesCountItemsDT.NewCyclesValuesChangedRow
            With myRow
                .BeginEdit()
                .SubSystemID = MyClass.myFluidicsCaption
                .ItemID = CYCLE_ELEMENTS.SF1_B4_CYC.ToString
                .CycleUnits = MyClass.GetCountTypeName(CYCLE_UNITS.SWITCHES.ToString, LanguageID)
                .CyclesCount = CInt(10000 * Rnd(100000))
                .EndEdit()
            End With
            MyClass.myScreenDelegate.CurrentCyclesCountItemsDT.Rows.Add(myRow)

            'SF1_B5
            myRow = MyClass.myScreenDelegate.CurrentCyclesCountItemsDT.NewCyclesValuesChangedRow
            With myRow
                .BeginEdit()
                .SubSystemID = MyClass.myFluidicsCaption
                .ItemID = CYCLE_ELEMENTS.SF1_B5_CYC.ToString
                .CycleUnits = MyClass.GetCountTypeName(CYCLE_UNITS.SWITCHES.ToString, LanguageID)
                .CyclesCount = CInt(10000 * Rnd(100000))
                .EndEdit()
            End With
            MyClass.myScreenDelegate.CurrentCyclesCountItemsDT.Rows.Add(myRow)

            'SF1_B6
            myRow = MyClass.myScreenDelegate.CurrentCyclesCountItemsDT.NewCyclesValuesChangedRow
            With myRow
                .BeginEdit()
                .SubSystemID = MyClass.myFluidicsCaption
                .ItemID = CYCLE_ELEMENTS.SF1_B6_CYC.ToString
                .CycleUnits = MyClass.GetCountTypeName(CYCLE_UNITS.SWITCHES.ToString, LanguageID)
                .CyclesCount = CInt(10000 * Rnd(100000))
                .EndEdit()
            End With
            MyClass.myScreenDelegate.CurrentCyclesCountItemsDT.Rows.Add(myRow)

            'SF1_B7
            myRow = MyClass.myScreenDelegate.CurrentCyclesCountItemsDT.NewCyclesValuesChangedRow
            With myRow
                .BeginEdit()
                .SubSystemID = MyClass.myFluidicsCaption
                .ItemID = CYCLE_ELEMENTS.SF1_B7_CYC.ToString
                .CycleUnits = MyClass.GetCountTypeName(CYCLE_UNITS.SWITCHES.ToString, LanguageID)
                .CyclesCount = CInt(10000 * Rnd(100000))
                .EndEdit()
            End With
            MyClass.myScreenDelegate.CurrentCyclesCountItemsDT.Rows.Add(myRow)

            'SF1_B8
            myRow = MyClass.myScreenDelegate.CurrentCyclesCountItemsDT.NewCyclesValuesChangedRow
            With myRow
                .BeginEdit()
                .SubSystemID = MyClass.myFluidicsCaption
                .ItemID = CYCLE_ELEMENTS.SF1_B8_CYC.ToString
                .CycleUnits = MyClass.GetCountTypeName(CYCLE_UNITS.SWITCHES.ToString, LanguageID)
                .CyclesCount = CInt(10000 * Rnd(100000))
                .EndEdit()
            End With
            MyClass.myScreenDelegate.CurrentCyclesCountItemsDT.Rows.Add(myRow)

            'SF1_B9
            myRow = MyClass.myScreenDelegate.CurrentCyclesCountItemsDT.NewCyclesValuesChangedRow
            With myRow
                .BeginEdit()
                .SubSystemID = MyClass.myFluidicsCaption
                .ItemID = CYCLE_ELEMENTS.SF1_B9_CYC.ToString
                .CycleUnits = MyClass.GetCountTypeName(CYCLE_UNITS.SWITCHES.ToString, LanguageID)
                .CyclesCount = CInt(10000 * Rnd(100000))
                .EndEdit()
            End With
            MyClass.myScreenDelegate.CurrentCyclesCountItemsDT.Rows.Add(myRow)

            'SF1_B10
            myRow = MyClass.myScreenDelegate.CurrentCyclesCountItemsDT.NewCyclesValuesChangedRow
            With myRow
                .BeginEdit()
                .SubSystemID = MyClass.myFluidicsCaption
                .ItemID = CYCLE_ELEMENTS.SF1_B10_CYC.ToString
                .CycleUnits = MyClass.GetCountTypeName(CYCLE_UNITS.SWITCHES.ToString, LanguageID)
                .CyclesCount = CInt(10000 * Rnd(100000))
                .EndEdit()
            End With
            MyClass.myScreenDelegate.CurrentCyclesCountItemsDT.Rows.Add(myRow)

            'SF1_EV1
            myRow = MyClass.myScreenDelegate.CurrentCyclesCountItemsDT.NewCyclesValuesChangedRow
            With myRow
                .BeginEdit()
                .SubSystemID = MyClass.myFluidicsCaption
                .ItemID = CYCLE_ELEMENTS.SF1_EV1_CYC.ToString
                .CycleUnits = MyClass.GetCountTypeName(CYCLE_UNITS.SWITCHES.ToString, LanguageID)
                .CyclesCount = CInt(10000 * Rnd(100000))
                .EndEdit()
            End With
            MyClass.myScreenDelegate.CurrentCyclesCountItemsDT.Rows.Add(myRow)

            'SF1_EV2
            myRow = MyClass.myScreenDelegate.CurrentCyclesCountItemsDT.NewCyclesValuesChangedRow
            With myRow
                .BeginEdit()
                .SubSystemID = MyClass.myFluidicsCaption
                .ItemID = CYCLE_ELEMENTS.SF1_EV2_CYC.ToString
                .CycleUnits = MyClass.GetCountTypeName(CYCLE_UNITS.SWITCHES.ToString, LanguageID)
                .CyclesCount = CInt(10000 * Rnd(100000))
                .EndEdit()
            End With
            MyClass.myScreenDelegate.CurrentCyclesCountItemsDT.Rows.Add(myRow)

            'SF1_GE1
            myRow = MyClass.myScreenDelegate.CurrentCyclesCountItemsDT.NewCyclesValuesChangedRow
            With myRow
                .BeginEdit()
                .SubSystemID = MyClass.myFluidicsCaption
                .ItemID = CYCLE_ELEMENTS.SF1_GE1_CYC.ToString
                .CycleUnits = MyClass.GetCountTypeName(CYCLE_UNITS.SWITCHES.ToString, LanguageID)
                .CyclesCount = CInt(10000 * Rnd(100000))
                .EndEdit()
            End With
            MyClass.myScreenDelegate.CurrentCyclesCountItemsDT.Rows.Add(myRow)

            'GLF_MR
            myRow = MyClass.myScreenDelegate.CurrentCyclesCountItemsDT.NewCyclesValuesChangedRow
            With myRow
                .BeginEdit()
                .SubSystemID = MyClass.myPhotometricsCaption
                .ItemID = CYCLE_ELEMENTS.GLF_MR_CYC.ToString
                .CycleUnits = MyClass.GetCountTypeName(CYCLE_UNITS.x10000STEPS.ToString, LanguageID)
                .CyclesCount = CInt(10000 * Rnd(100000))
                .EndEdit()
            End With
            MyClass.myScreenDelegate.CurrentCyclesCountItemsDT.Rows.Add(myRow)

            'GLF_MW
            myRow = MyClass.myScreenDelegate.CurrentCyclesCountItemsDT.NewCyclesValuesChangedRow
            With myRow
                .BeginEdit()
                .SubSystemID = MyClass.myPhotometricsCaption
                .ItemID = CYCLE_ELEMENTS.GLF_MW_CYC.ToString
                .CycleUnits = MyClass.GetCountTypeName(CYCLE_UNITS.x100STEPS.ToString, LanguageID)
                .CyclesCount = CInt(10000 * Rnd(100000))
                .EndEdit()
            End With
            MyClass.myScreenDelegate.CurrentCyclesCountItemsDT.Rows.Add(myRow)


            MyClass.myScreenDelegate.CurrentCyclesCountItemsDT.AcceptChanges()

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".SimulateLoadCyclesGrid ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".SimulateLoadCyclesGrid ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
            MyClass.CurrentScreenMode = ScreenModes.ON_ERROR
        End Try
    End Sub

    ''' <summary>
    ''' Manages the behaviour of the waiting progress bar while simulation mode
    ''' </summary>
    ''' <param name="pTime"></param>
    ''' <remarks>SGM 28/07/2011</remarks>
    Private Sub SimulateWaiting(ByVal pTime As Integer)
        Try
            Me.BsWaitProgressBar.Visible = True
            'Me.WaitProgressBar.TimeForWait = pTime
            'Me.WaitProgressBar.StartWaiting()

            For i As Integer = 1 To pTime Step 100
                Application.DoEvents()
                System.Threading.Thread.Sleep(100)
            Next

            Me.BsWaitProgressBar.Visible = False
            'Me.WaitProgressBar.StopWaiting()

            'Me.WaitingProgressBar.Maximum = pTime
            'Me.WaitingProgressBar.Visible = True
            'For m As Integer = 0 To pTime Step 10
            '    System.Threading.Thread.Sleep(1)
            '    If m <= Me.WaitingProgressBar.Maximum Then
            '        Me.WaitingProgressBar.Value = m
            '    Else
            '        Exit Sub
            '    End If
            '    Me.WaitingProgressBar.Refresh()
            'Next
            'Me.WaitingProgressBar.Visible = False
        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".SimulateWaiting ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".SimulateWaiting ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
            Me.BsWaitProgressBar.Visible = False
            'Me.WaitProgressBar.StopWaiting()
            MyClass.CurrentScreenMode = ScreenModes.ON_ERROR
            'Me.WaitingProgressBar.Visible = False
        End Try
    End Sub

#End Region


#End Region

#Region "Events"

#Region "Common Event Handlers"

    ''' <summary>
    ''' IDisposable functionality to force the releasing of the objects which wasn't totally closed by default
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>Created by XBC 12/09/2011</remarks>
    Private Sub ICycleCountScreen_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
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

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>Created: SGM 26/07/2011</remarks>
    Private Sub ICycleCountScreen_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Dim myglobal As New GlobalDataTO
        'Dim myGlobalbase As New GlobalBase

        Try
            'screen delegate SGM 20/01/2012
            MyClass.myScreenDelegate = New CycleCountDelegate(MyBase.myServiceMDI.ActiveAnalyzer, mySendFwScriptDelegate)

            'Get the current user level
            MyBase.GetUserNumericalLevel()
            If MyBase.CurrentUserNumericalLevel = USER_LEVEL.lOPERATOR Then
                Me.BsWriteButton.Visible = False
            End If

            'Get the current Language from the current Application Session
            'Dim currentLanguageGlobal As New GlobalBase
            LanguageID = GlobalBase.GetSessionInfo().ApplicationLanguage.Trim.ToString()

            'AnalyzerId
            MyClass.myScreenDelegate.AnalyzerId = MyBase.myServiceMDI.ActiveAnalyzer

            Me.Location = New Point(0, 0)

            MyClass.LoadScreen()

            ResetBorderSRV()
            '

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".ICycleCountScreen_Load", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".ICycleCountScreen_Load", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
            MyClass.CurrentScreenMode = ScreenModes.ON_ERROR
            '
        End Try
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>Created: SGM 26/07/2011</remarks>
    Private Sub ICycleCountScreen_Shown(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Shown

        Dim myGlobal As New GlobalDataTO

        Try

            myGlobal = MyClass.ReadCyclesFromAnalyzer()

            If Not myGlobal.HasError Then
                myGlobal = MyClass.LoadCyclesGrid()
                If Not myGlobal.HasError Then
                    MyClass.CurrentScreenMode = ScreenModes.LOADED
                End If
            End If


            If myGlobal.HasError Then MyClass.CurrentScreenMode = ScreenModes.ON_ERROR

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".ICycleCountScreen_Shown", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".ICycleCountScreen_Shown", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
            MyClass.CurrentScreenMode = ScreenModes.ON_ERROR
        End Try
    End Sub

    ''' <summary>
    ''' When the  ESC key is pressed, the screen is closed 
    ''' </summary>
    ''' <remarks>
    ''' Created by SGM 27/07/2011
    ''' </remarks>
    Private Sub ICycleCountScreen_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyDown
        Try
            If (e.KeyCode = Keys.Escape) Then
                'If (Me.BsExitButton.Enabled) Then
                '    Me.Close()
                'End If

                'RH 04/07/2011 Escape key should do exactly the same operations as bsExitButton_Click()
                BsExitButton.PerformClick()
            End If
        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".KeyDown ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".KeyDown", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>Created: SGM 26/07/2011</remarks>
    Private Sub BsWriteButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsWriteButton.Click

        Dim myGlobal As New GlobalDataTO

        Try
            myGlobal = MyClass.WriteCyclesToAnalyzer
        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & " BsWriteButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".BsWriteButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
            MyClass.CurrentScreenMode = ScreenModes.ON_ERROR
        End Try
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>Created: SGM 26/07/2011</remarks>
    Private Sub BsExitButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsExitButton.Click
        Try

            MyClass.ExitScreen()

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & " BsExitButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".BsExitButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
            MyClass.CurrentScreenMode = ScreenModes.ON_ERROR
        End Try
    End Sub

    Private Sub BsInfoExpandButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsInfoExpandButton.Click

        Try

            Dim myInfoPanel As Panel = Me.BsCycleCountInfoPanel
            Dim myAdjPanel As Panel = Me.BsCycleAdjustPanel
            Dim myExpandButton As Panel = Me.BsInfoExpandButton
            Dim myGlobal As GlobalDataTO = MyBase.ExpandInformation(Not MyClass.IsInfoExpanded, myInfoPanel, myAdjPanel, myExpandButton)
            If Not myGlobal.HasError And myGlobal.SetDatos IsNot Nothing Then
                IsInfoExpanded = CBool(myGlobal.SetDatos)
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".BsInfoExpandButton_Click", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".BsInfoExpandButton_Click ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub
#End Region

#Region "Grid Event Handlers"


    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>Created: SGM 26/07/2011</remarks>
    Private Sub BsSelectAllCheckbox_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsSelectAllCheckbox.CheckedChanged
        Dim myGlobal As New GlobalDataTO
        Try

            Dim checkvalue As Boolean = False
            Dim myCheckBox As CheckBox = CType(sender, CheckBox)
            If myCheckBox IsNot Nothing Then

                If MyClass.CurrentScreenMode = ScreenModes.LOADED Then
                    checkvalue = myCheckBox.Checked
                    myGlobal = MyClass.SelectAll(checkvalue)
                End If

            End If


        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".BsSelectAllCheckbox_CheckedChanged", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".BsSelectAllCheckbox_CheckedChanged", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
            MyClass.CurrentScreenMode = ScreenModes.ON_ERROR
            MyClass.IsCheckingAll = False
        End Try
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>Created: SGM 26/07/2011</remarks>
    Private Sub bsCyclesDataGridView_CellEnter(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles bsCyclesDataGridView.CellEnter
        Try

            If MyClass.IsCheckingAll Then Exit Sub

            Dim myGrid As DataGridView = CType(sender, DataGridView)
            Dim myRow As DataGridViewRow = myGrid.Rows(e.RowIndex)
            Dim myCell As DataGridViewCell = myRow.Cells(e.ColumnIndex)
            If myCell IsNot Nothing Then
                If MyClass.CurrentScreenMode = ScreenModes.LOADED Or MyClass.CurrentScreenMode = ScreenModes.MODIFYING Then
                    If myCell.ColumnIndex = 6 Then 'new value

                        MyClass.CurrentScreenMode = ScreenModes.MODIFYING

                        Dim myCheckValue As Boolean
                        myCheckValue = CBool(myRow.Cells(5).Value)
                        MyClass.PaintCheckedRow(myRow, myCheckValue)
                        myCell.ReadOnly = False

                    Else
                        MyClass.CurrentScreenMode = ScreenModes.LOADED
                        myRow.Selected = True
                    End If
                End If
            End If

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".bsCyclesDataGridView_CellEnter", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".bsCyclesDataGridView_CellEnter", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
            MyClass.CurrentScreenMode = ScreenModes.ON_ERROR
        End Try
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>Created: SGM 26/07/2011</remarks>
    Private Sub bsCyclesDataGridView_CellLeave(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles bsCyclesDataGridView.CellLeave
        Try

            If MyClass.IsCheckingAll Then Exit Sub

            Dim myGrid As DataGridView = CType(sender, DataGridView)
            Dim myRow As DataGridViewRow = myGrid.Rows(e.RowIndex)
            Dim myCell As DataGridViewCell = myRow.Cells(e.ColumnIndex)
            If myCell IsNot Nothing Then
                If MyClass.CurrentScreenMode = ScreenModes.LOADED Or MyClass.CurrentScreenMode = ScreenModes.MODIFYING Then
                    If myCell.ColumnIndex = 6 Then 'new value
                        If MyClass.CurrentScreenMode = ScreenModes.MODIFYING Then
                            MyClass.CurrentScreenMode = ScreenModes.LOADED
                        End If

                        If MyClass.CurrentScreenMode = ScreenModes.LOADED Then
                            Dim myCheckValue As Boolean
                            myCheckValue = CBool(myRow.Cells(5).Value)
                            MyClass.PaintCheckedRow(myRow, myCheckValue)
                            myCell.ReadOnly = False
                        End If
                    End If
                End If
            End If

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".bsCyclesDataGridView_CellLeave", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".bsCyclesDataGridView_CellLeave", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
            MyClass.CurrentScreenMode = ScreenModes.ON_ERROR
        End Try
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>Created: SGM 26/07/2011</remarks>
    Private Sub bsCyclesDataGridView_CellValidating(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellValidatingEventArgs) Handles bsCyclesDataGridView.CellValidating
        Try

            If MyClass.IsCheckingAll Then Exit Sub

            Dim myGrid As DataGridView = CType(sender, DataGridView)
            Dim myRow As DataGridViewRow = myGrid.Rows(e.RowIndex)
            Dim myCell As DataGridViewCell = myRow.Cells(e.ColumnIndex)
            If myCell IsNot Nothing Then
                If MyClass.CurrentScreenMode = ScreenModes.LOADED Or MyClass.CurrentScreenMode = ScreenModes.MODIFYING Then
                    If myCell.ColumnIndex = 5 Then 'selected
                        Dim myCheckValue As Boolean
                        myCheckValue = CBool(myCell.Value)

                        Dim myEditCell As DataGridViewCell
                        myEditCell = myGrid.Rows(e.RowIndex).Cells(6)

                        MyClass.PaintCheckedRow(myGrid.Rows(e.RowIndex), myCheckValue)
                        myEditCell.Selected = myCheckValue

                        Me.BsWriteButton.Enabled = myCheckValue Or MyClass.IsSomeItemSelected
                        Me.BsWriteButton.Refresh()

                    End If
                    If myCell.ColumnIndex = 6 Then 'new value
                        If IsNumeric(myCell.Value) Then
                            Dim myIntValue As Integer = CInt(myCell.Value)
                            myCell.Value = myIntValue
                        ElseIf CStr(myCell.Value) = "" Then
                            myCell.Value = 0
                        Else

                        End If
                        myCell.ReadOnly = False
                    End If
                End If
            End If
        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".bsCyclesDataGridView_CellValidating", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".bsCyclesDataGridView_CellValidating", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
            MyClass.CurrentScreenMode = ScreenModes.ON_ERROR
        End Try
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>Created: SGM 26/07/2011</remarks>
    Private Sub bsCyclesDataGridView_CellValueChanged(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles bsCyclesDataGridView.CellValueChanged
        Try

            If MyClass.IsCheckingAll Then Exit Sub

            Dim myGrid As DataGridView = CType(sender, DataGridView)
            Dim myRow As DataGridViewRow = myGrid.Rows(e.RowIndex)
            Dim myCell As DataGridViewCell = myRow.Cells(e.ColumnIndex)
            If myCell IsNot Nothing Then
                If MyClass.CurrentScreenMode = ScreenModes.LOADED Or MyClass.CurrentScreenMode = ScreenModes.MODIFYING Then
                    If myCell.ColumnIndex = 5 Then
                        If Not MyClass.IsCheckingAll Then
                            If MyClass.CurrentScreenMode = ScreenModes.LOADED Then
                                Dim myCheckValue As Boolean
                                myCheckValue = CBool(myCell.Value)
                                MyClass.PaintCheckedRow(myRow, myCheckValue)

                                Me.BsWriteButton.Enabled = myCheckValue Or MyClass.IsSomeItemSelected
                                Me.BsWriteButton.Refresh()
                            End If
                            myRow.Selected = True
                        End If

                    ElseIf myCell.ColumnIndex < 5 Then
                        MyClass.CurrentScreenMode = ScreenModes.LOADED
                        myRow.Selected = True
                    End If
                End If
            End If

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".bsCyclesDataGridView_CellValueChanged", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".bsCyclesDataGridView_CellValueChanged", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
            MyClass.CurrentScreenMode = ScreenModes.ON_ERROR
        End Try
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>Created: SGM 26/07/2011</remarks>
    Private Sub bsCyclesDataGridView_EditingControlShowing(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewEditingControlShowingEventArgs) Handles bsCyclesDataGridView.EditingControlShowing
        Try

            If MyClass.CurrentScreenMode = ScreenModes.MODIFYING Then

                If Not TypeOf e.Control Is TextBox Then Return

                Dim tb As TextBox = CType(e.Control, TextBox)

                If Not tb Is Nothing Then
                    tb.Multiline = True
                    AddHandler tb.KeyDown, AddressOf dgvTextBox_KeyDown
                End If

            End If

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & " bsCyclesDataGridView_EditingControlShowing ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage("Error", "SYSTEM_ERROR", ex.Message)
            MyClass.CurrentScreenMode = ScreenModes.ON_ERROR
        End Try
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>Created: SGM 26/07/2011</remarks>
    Private Sub dgvTextBox_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs)
        Try

            Select Case e.KeyCode
                Case Keys.Delete, Keys.Back

                Case Keys.Right, Keys.Left, Keys.Up, Keys.Down, Keys.Tab, Keys.Escape, Keys.Control, Keys.ControlKey, Keys.Shift, Keys.Alt

                Case Keys.Escape
                    MyClass.CurrentScreenMode = ScreenModes.LOADED
                    e.Handled = True

                Case Keys.NumPad0, Keys.NumPad1, Keys.NumPad2, Keys.NumPad3, Keys.NumPad4, Keys.NumPad5, Keys.NumPad6, Keys.NumPad7, Keys.NumPad8, Keys.NumPad9
                    'ONLY NUMERIC
                    'OK

                Case Else
                    e.SuppressKeyPress = True
                    e.Handled = True

            End Select


        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & " dgvTextBox_KeyDown ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage("Error", "SYSTEM_ERROR", ex.Message)
            MyClass.CurrentScreenMode = ScreenModes.ON_ERROR
        End Try
    End Sub

#End Region

#Region "Communication"
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
    ''' <remarks>Created by SGM 27/07/2011</remarks>
    Private Function ManageReceptionEvent(ByVal pResponse As RESPONSE_TYPES, ByVal pData As Object) As Boolean
        Dim myGlobal As New GlobalDataTO
        Try
            'manage special operations according to the screen characteristics
            Application.DoEvents()

            'If MyBase.SimulationMode Then Exit Sub

            'if needed manage the event in the Base Form
            MyBase.OnReceptionLastFwScriptEvent(pResponse, pData)
            'MyClass.myFwScriptDelegate.IsWaitingForResponse = False
            Select Case MyBase.CurrentMode
                Case ADJUSTMENT_MODES.CYCLES_READED
                    If pResponse = RESPONSE_TYPES.OK Then
                        myGlobal = MyBase.DisplayMessage(Messages.SRV_CYCLES_READED.ToString)
                        MyClass.CurrentScreenMode = ScreenModes.LOADED
                    End If

                Case ADJUSTMENT_MODES.CYCLES_WRITTEN
                    If pResponse = RESPONSE_TYPES.OK Then
                        myGlobal = MyBase.DisplayMessage(Messages.SRV_CYCLES_WRITTEN.ToString)
                        MyClass.CurrentScreenMode = ScreenModes.LOADED
                    End If



                Case ADJUSTMENT_MODES.ERROR_MODE
                    myGlobal = MyBase.DisplayMessage(Messages.FWSCRIPT_DATA_ERROR.ToString)
                    MyClass.CurrentScreenMode = ScreenModes.ON_ERROR
            End Select

            If myGlobal.HasError Then MyClass.CurrentScreenMode = ScreenModes.ON_ERROR

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".ScreenReceptionLastFwScriptEvent ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".ScreenReceptionLastFwScriptEvent", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, myGlobal.ErrorMessage, Me)
            MyClass.CurrentScreenMode = ScreenModes.ON_ERROR
        End Try

        Return True
    End Function



#End Region


#End Region


    Public Overrides Sub PrepareErrorMode(Optional ByVal pAlarmType As ManagementAlarmTypes = ManagementAlarmTypes.NONE)
        Try



        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareErrorMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareErrorMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
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
            'TODO

            'when stop action is finished, perform final operations after alarm received
            MyBase.myServiceMDI.ManageAlarmStep2(pAlarmType)

        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".StopCurrentOperation ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".StopCurrentOperation ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

#End Region
End Class