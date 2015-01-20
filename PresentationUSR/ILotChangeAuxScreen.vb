Option Strict On
Option Explicit On
Option Infer On

Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.Types

Public Class ILotChangeAuxScreen
    Inherits Biosystems.Ax00.PresentationCOM.BSBaseForm

#Region "Declarations"
    Private currentLanguage As String
#End Region

#Region "Attributes"
    '' Number of the Lot currently used for the Control (the one to be changed)
    'Private CurrentLotAttribute As String
    'Private PreviousLotAttribute As String
    ''
    ''When True, it indicates all data of the Current Lot (Lot definition + Concentration Values for each linked Test/Sample Typle) 
    ''has to be saved before execute the lot change.
    'Private SavedCurrentLotAttribute As Boolean
    ''
    ''When True, it indicates a new Lot Number was informed.
    ''When False, it indicates a saved Lot Number was selected
    'Private NewLotFlagAttribute As Boolean
    ''
    ''New Number of Lot to be used for the Control. It can be a new number or a previous saved one
    'Private NewLotNumberAttribute As String

    ''When the new Lot corresponds to a previous saved one, this property will return its Expiration Date. 
    'Private AcceptAttribute As Boolean
    'Private NewLotExpDateAttribute As String
    'Private auxActDateAttribute As String
    'Private originalTestControlsAttribute As New TestControlsDS
    'Private currentTestControlsAttribute As New TestControlsDS

    Private CurrentLotAttribute As String
    Private OriginalLotAttribute As String
    Private PreviousLotDataDSAttribute As ControlsDS

    Private PreviousLotRecoveredAttribute As Boolean
    Private NewLotNumberAttribute As String
    Private NewLotExpirationDateAttribute As String
    Private SaveCurrentLotAsPreviousAttribute As Boolean
#End Region

#Region "Properties"
    Public WriteOnly Property CurrentLot() As String
        Set(ByVal value As String)
            CurrentLotAttribute = value
        End Set
    End Property

    Public WriteOnly Property OriginalLot() As String
        Set(ByVal value As String)
            OriginalLotAttribute = value
        End Set
    End Property

    Public WriteOnly Property PreviusLotDataDS() As ControlsDS
        Set(ByVal value As ControlsDS)
            PreviousLotDataDSAttribute = value
        End Set
    End Property

    Public ReadOnly Property NewLotNumber() As String
        Get
            Return NewLotNumberAttribute
        End Get
    End Property

    Public ReadOnly Property NewLotExpirationDate() As String
        Get
            Return NewLotExpirationDateAttribute
        End Get
    End Property

    Public ReadOnly Property PreviousLotRecovered() As Boolean
        Get
            Return PreviousLotRecoveredAttribute
        End Get
    End Property

    Public ReadOnly Property SaveCurrentLotAsPrevious() As Boolean
        Get
            Return SaveCurrentLotAsPreviousAttribute
        End Get
    End Property

    'Public Property currentTestControls() As TestControlsDS
    '    Get
    '        Return currentTestControlsAttribute
    '    End Get
    '    Set(ByVal value As TestControlsDS)
    '        currentTestControlsAttribute = value
    '    End Set
    'End Property

    'Public Property originaltestcontrols() As TestControlsDS
    '    Get
    '        Return originaltestcontrolsAttribute
    '    End Get
    '    Set(ByVal value As TestControlsDS)
    '        originaltestcontrolsAttribute = value
    '    End Set
    'End Property

    'Public Property Accept() As Boolean
    '    Get
    '        Return AcceptAttribute
    '    End Get
    '    Set(ByVal value As Boolean)
    '        AcceptAttribute = value
    '    End Set
    'End Property

    'Public Property previousLot() As String
    '    Get
    '        Return PreviousLotAttribute
    '    End Get
    '    Set(ByVal value As String)
    '        PreviousLotAttribute = value
    '    End Set
    'End Property

    'Public Property currentLot() As String
    '    Get
    '        Return CurrentLotAttribute
    '    End Get
    '    Set(ByVal value As String)
    '        CurrentLotAttribute = value
    '    End Set
    'End Property

    'Public Property SavedCurrentLot() As Boolean
    '    Get
    '        Return SavedCurrentLotAttribute
    '    End Get
    '    Set(ByVal value As Boolean)
    '        SavedCurrentLotAttribute = value
    '    End Set
    'End Property

    'Public Property NewLotNumber() As String
    '    Get
    '        Return NewLotNumberAttribute
    '    End Get
    '    Set(ByVal value As String)
    '        NewLotNumberAttribute = value
    '    End Set
    'End Property

    'Public Property NewLotExpDate() As String
    '    Get
    '        Return NewLotExpDateAttribute
    '    End Get
    '    Set(ByVal value As String)
    '        NewLotExpDateAttribute = value
    '    End Set
    'End Property

    'Public Property auxActDate() As String
    '    Get
    '        Return auxActDateAttribute
    '    End Get
    '    Set(ByVal value As String)
    '        auxActDateAttribute = value
    '    End Set
    'End Property

    'Public Property NewLotFlag() As Boolean
    '    Get
    '        Return NewLotFlagAttribute
    '    End Get
    '    Set(ByVal value As Boolean)
    '        NewLotFlagAttribute = value
    '    End Set
    'End Property
#End Region

#Region "Constructor"
    Public Sub New()
        InitializeComponent()
    End Sub
#End Region

#Region "Methods"

    ''' <summary>
    ''' Search Icons for screen buttons
    ''' </summary>
    Private Sub PrepareButtons()
        Try
            Dim auxIconName As String = ""
            Dim iconPath As String = MyBase.IconsPath

            'ACCEPT Button
            auxIconName = GetIconName("ACCEPT1")
            If (auxIconName <> "") Then
                bsAcceptButton.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
            End If

            'CANCEL Button
            auxIconName = GetIconName("CANCEL")
            If (auxIconName <> "") Then
                bsCancelButton.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".PrepareButtons ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PrepareButtons ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Get texts in the current application language for all screen controls
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 31/03/2011
    ''' Modified by: SA 12/05/2011 - Removed parameter pLanguageID; use the global variable instead
    '''                              Get multilanguage resources for grid header texts, moved to a new function for grid preparation
    ''' </remarks>
    Private Sub GetScreenLabels()
        Try
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            bsTitleLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "TITLE_LotChange", currentLanguage)
            bsNewLotRadioButton.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_NewLot", currentLanguage)
            bsSavedLotRadioButton.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SavedLot", currentLanguage)
            bsSaveDataCheckBox.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SaveCurrentLot", currentLanguage) & ": " & OriginalLotAttribute

            'For Tooltips...
            bsScreenToolTips.SetToolTip(bsAcceptButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Save&Close", currentLanguage))
            bsScreenToolTips.SetToolTip(bsCancelButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Cancel&Close", currentLanguage))
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".GetScreenLabels", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".GetScreenLabels", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Initialize the grid used to shown data of a Previous Saved Lot for the Control
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 12/05/2011
    ''' </remarks>
    Private Sub PreparePreviousLotGrid()
        Try
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            bsPreviousLotDataGridView.Rows.Clear()
            bsPreviousLotDataGridView.Columns.Clear()

            bsPreviousLotDataGridView.AutoGenerateColumns = False
            bsPreviousLotDataGridView.AllowUserToAddRows = False
            bsPreviousLotDataGridView.AllowUserToDeleteRows = False
            bsPreviousLotDataGridView.EditMode = DataGridViewEditMode.EditProgrammatically

            Dim columnLotNum As New DataGridViewTextBoxColumn
            columnLotNum.Name = "LotNumber"
            columnLotNum.DataPropertyName = "LotNumber"
            columnLotNum.HeaderText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_LotNumber", currentLanguage)
            columnLotNum.ReadOnly = True
            bsPreviousLotDataGridView.Columns.Add(columnLotNum)
            bsPreviousLotDataGridView.Columns("LotNumber").Width = 100
            bsPreviousLotDataGridView.Columns("LotNumber").HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft
            bsPreviousLotDataGridView.Columns("LotNumber").DefaultCellStyle.BackColor = SystemColors.MenuBar

            Dim columnActDate As New DataGridViewTextBoxColumn
            columnActDate.Name = "ActivationDate"
            columnActDate.DataPropertyName = "ActivationDate"
            columnActDate.HeaderText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_ActivationDate", currentLanguage)
            columnActDate.ReadOnly = True
            bsPreviousLotDataGridView.Columns.Add(columnActDate)
            bsPreviousLotDataGridView.Columns("ActivationDate").Width = 134 '135 JB - 12/11/2012
            bsPreviousLotDataGridView.Columns("ActivationDate").HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft
            bsPreviousLotDataGridView.Columns("ActivationDate").DefaultCellStyle.BackColor = SystemColors.MenuBar

            Dim columnExpDate As New DataGridViewTextBoxColumn
            columnExpDate.Name = "ExpirationDate"
            columnExpDate.DataPropertyName = "ExpirationDate"
            columnExpDate.HeaderText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Expdate_Full", currentLanguage)
            columnExpDate.ReadOnly = True
            bsPreviousLotDataGridView.Columns.Add(columnExpDate)
            bsPreviousLotDataGridView.Columns("ExpirationDate").Width = 140 '135 JB - 12/11/2012
            bsPreviousLotDataGridView.Columns("ExpirationDate").HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft
            bsPreviousLotDataGridView.Columns("ExpirationDate").DefaultCellStyle.BackColor = SystemColors.MenuBar
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".PreparePreviousLotGrid", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PreparePreviousLotGrid", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Screen initialization according the information received
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 31/03/2011
    ''' Modified by: SA 12/05/2011 - Implementation changed
    ''' </remarks>
    Private Sub InitializeScreen()
        Try

            'DL 28/07/2011
            Dim myLocation As Point = IAx00MainMDI.Location
            Dim mySize As Size = IAx00MainMDI.Size

            Me.Location = New Point(myLocation.X + CInt((mySize.Width - Me.Width) / 2), myLocation.Y + CInt((mySize.Height - Me.Height) / 2))
            'END DL 28/07/2011

            'Get the current Language from the current Application Session
            'Dim currentLanguageGlobal As New GlobalBase
            currentLanguage = GlobalBase.GetSessionInfo().ApplicationLanguage.Trim.ToString

            'Get Icons for Screen Buttons
            PrepareButtons()

            'Set multilanguage texts for all screen labels and tooltips...
            GetScreenLabels()

            'If there is a previous saved Lot for the Control, show it and enable the SavedLot RadioButton
            bsSavedLotRadioButton.Enabled = False

            PreparePreviousLotGrid()
            If (Not PreviousLotDataDSAttribute Is Nothing AndAlso PreviousLotDataDSAttribute.tparControls.Count = 1) Then
                bsSavedLotRadioButton.Enabled = True

                'Load previous saved Lot data in the grid (in this way to allow format the date/time fields properly)
                bsPreviousLotDataGridView.Rows.Add()
                bsPreviousLotDataGridView.Rows(0).Cells("LotNumber").Value = PreviousLotDataDSAttribute.tparControls.First.LotNumber.ToString
                bsPreviousLotDataGridView.Rows(0).Cells("ActivationDate").Value = PreviousLotDataDSAttribute.tparControls.First.ActivationDate.ToString(SystemInfoManager.OSDateFormat) & " " & _
                                                                                  PreviousLotDataDSAttribute.tparControls.First.ActivationDate.ToString(SystemInfoManager.OSShortTimeFormat)
                bsPreviousLotDataGridView.Rows(0).Cells("ExpirationDate").Value = PreviousLotDataDSAttribute.tparControls.First.ExpirationDate.ToString(SystemInfoManager.OSDateFormat)
            End If
            bsPreviousLotDataGridView.ClearSelection()

            'If originalTestControlsAttribute.tparTestControls.Count > 0 Then
            '    Dim selRow As Integer = -1

            '    selRow = bsPreviousLotDataGridView.Rows.Add()
            '    If originalTestControlsAttribute.tparTestControls.Rows.Count > 0 Then
            '        bsPreviousLotDataGridView.Rows(selRow).Cells("Lotnumber").Value = originalTestControlsAttribute.tparTestControls.First.LotNumber    'currentTestControlsAttribute.tparTestControls.First.LotNumber  ' PreviousLotAttribute
            '        bsPreviousLotDataGridView.Rows(selRow).Cells("ExpirationDate").Value = NewLotExpDate
            '        bsPreviousLotDataGridView.Rows(selRow).Cells("ActivationDate").Value = auxActDateAttribute
            '        bsSavedLotRadioButton.Enabled = True
            '    Else
            '        bsPreviousLotDataGridView.Rows(selRow).Cells("Lotnumber").Value = ""
            '        bsPreviousLotDataGridView.Rows(selRow).Cells("ExpirationDate").Value = ""
            '        bsPreviousLotDataGridView.Rows(selRow).Cells("ActivationDate").Value = ""
            '        bsSavedLotRadioButton.Enabled = False
            '    End If
            'Else
            '    bsSavedLotRadioButton.Enabled = False
            'End If
            Me.Opacity = 100
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", ".InitializeScreen " & Me.Name, EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".InitializeScreen", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Not allow move form and mantain the center location in center parent
    ''' </summary>
    ''' <remarks>
    ''' Created by: DL 27/07/2011
    ''' </remarks>
    Protected Overrides Sub WndProc(ByRef m As Message)
        If m.Msg = WM_WINDOWPOSCHANGING Then
            Dim pos As WINDOWPOS = DirectCast(Runtime.InteropServices.Marshal.PtrToStructure(m.LParam, _
                                                                                             GetType(WINDOWPOS)),  _
                                                                                             WINDOWPOS)
            Dim myLocation As Point = IAx00MainMDI.Location
            Dim mySize As Size = IAx00MainMDI.Size

            pos.x = myLocation.X + CInt((mySize.Width - Me.Width) / 2)
            pos.y = myLocation.Y + CInt((mySize.Height - Me.Height) / 2)
            Runtime.InteropServices.Marshal.StructureToPtr(pos, m.LParam, True)
        End If

        MyBase.WndProc(m)

    End Sub


    ''' <summary>
    ''' Accept the Lot Change
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 01/04/2011
    ''' Modified by: SA 12/05/2011 - Implementation changed
    ''' </remarks>
    Private Sub AcceptLotChange()
        Try
            Dim fieldsOk As Boolean = True
            bsScreenErrorProvider.Clear()

            'If there is a Previous Lot loaded in the grid, get the LotNumber
            Dim previousLotNumber As String = ""
            Dim previousLotExpDate As String = ""
            If (bsPreviousLotDataGridView.Rows.Count > 0) Then
                previousLotNumber = bsPreviousLotDataGridView.Rows(0).Cells("LotNumber").Value.ToString.Trim
                previousLotExpDate = bsPreviousLotDataGridView.Rows(0).Cells("ExpirationDate").Value.ToString.Trim
            End If

            If (bsNewLotRadioButton.Checked) Then
                If (bsLotNumberTextBox.Text.Length = 0) Then
                    fieldsOk = False
                    bsScreenErrorProvider.SetError(bsLotNumberTextBox, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString, currentLanguage))
                ElseIf (bsLotNumberTextBox.Text.ToUpper.Trim = CurrentLotAttribute.ToUpper.Trim) OrElse _
                       (bsLotNumberTextBox.Text.ToUpper.Trim = previousLotNumber) Then
                    fieldsOk = False
                    bsScreenErrorProvider.SetError(bsLotNumberTextBox, GetMessageText(GlobalEnumerates.Messages.REPEATED_LOT_NUMBER.ToString, currentLanguage))
                End If
            End If

            If (fieldsOk) Then
                'Inform properties and close the screen
                If (bsNewLotRadioButton.Checked) Then
                    NewLotNumberAttribute = bsLotNumberTextBox.Text
                    NewLotExpirationDateAttribute = Now.AddMonths(3).ToString
                    PreviousLotRecoveredAttribute = False
                Else
                    NewLotNumberAttribute = previousLotNumber
                    NewLotExpirationDateAttribute = previousLotExpDate
                    PreviousLotRecoveredAttribute = True
                End If
                SaveCurrentLotAsPreviousAttribute = bsSaveDataCheckBox.Checked

                Me.DialogResult = Windows.Forms.DialogResult.OK
                Me.Close()
            End If

            'If (bsLotNumberTextBox.Text.Length > 0) Then
            '    If (UCase(bsLotNumberTextBox.Text) = UCase(CurrentLotAttribute.ToString)) Or _
            '    (Not PreviousLotAttribute Is Nothing AndAlso UCase(bsLotNumberTextBox.Text) = UCase(PreviousLotAttribute.ToString)) Then
            '        Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate
            '        fieldsOk = False
            '        BsErrorProvider1.SetError(bsLotNumberTextBox, myMultiLangResourcesDelegate.GetResourceText(Nothing, "MSG_REPEATED_LOT", currentLanguage))
            '    End If
            'Else
            '    fieldsOk = False
            '    BsErrorProvider1.SetError(bsLotNumberTextBox, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString))
            'End If

            '    If fieldsOk Then
            '        CurrentLotAttribute = bsLotNumberTextBox.Text
            '        NewLotFlagAttribute = True

            '        If bsSaveDataCheckBox.Checked Then
            '            SavedCurrentLotAttribute = True
            '        End If

            '    End If
            'Else
            '    NewLotFlagAttribute = False

            '    If bsPreviousLotDataGridView.Rows.Count > 0 Then
            '        SavedCurrentLotAttribute = True
            '        PreviousLotAttribute = CurrentLotAttribute
            '        CurrentLotAttribute = bsPreviousLotDataGridView.Rows(0).Cells("LotNumber").Value.ToString
            '        NewLotExpDateAttribute = bsPreviousLotDataGridView.Rows(0).Cells("ExpirationDate").Value.ToString
            '    End If
            'End If

            'AcceptAttribute = True
            'If fieldsOk Then Me.Close()

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".AcceptLotChange ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".AcceptLotChange", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub
#End Region

#Region "Events"

    ''' <summary>
    ''' When the screen ESC key is pressed, the screen is closed
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 09/11/2010
    ''' Modified by: SA 12/05/2011 - New implementation, the previous one was wrong because was copied from a screen opened 
    '''                              from the Main Menu
    ''' </remarks>
    Private Sub LotChangeAuxScreen_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyDown
        Try
            If (e.KeyCode = Keys.Escape) Then
                'Me.Close()

                'RH 04/07/2011 Escape key should do exactly the same operations as bsCancelButton_Click()
                bsCancelButton.PerformClick()
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".LotChangeAuxScreen_KeyDown ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".LotChangeAuxScreen_KeyDown", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Screen loading
    ''' </summary>
    Private Sub LotChangeAuxScreen_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            InitializeScreen()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".LotChangeAuxScreen_Load ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".LotChangeAuxScreen_Load", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Control screen status when the OptionButton of New Lot is checked
    ''' </summary>
    Private Sub bsNewLotRadioButton_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsNewLotRadioButton.CheckedChanged
        Try
            bsLotNumberTextBox.Enabled = True
            bsLotNumberTextBox.BackColor = Color.Khaki
            bsSaveDataCheckBox.Enabled = True
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsNewLotRadioButton_CheckedChanged ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsNewLotRadioButton_CheckedChanged", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Control screen status when the OptionButton of Saved Lot is checked
    ''' </summary>
    Private Sub bsSavedLotRadioButton_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsSavedLotRadioButton.CheckedChanged
        Try
            bsLotNumberTextBox.Enabled = False
            bsLotNumberTextBox.BackColor = SystemColors.MenuBar
            If (bsScreenErrorProvider.GetError(bsLotNumberTextBox) <> "") Then bsScreenErrorProvider.SetError(bsLotNumberTextBox, String.Empty)

            bsSaveDataCheckBox.Checked = True
            bsSaveDataCheckBox.Enabled = False
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsSavedLotRadioButton_CheckedChanged ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsSavedLotRadioButton_CheckedChanged", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Accept the Lot change and close the screen if there is not error 
    ''' </summary>
    Private Sub bsAcceptButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsAcceptButton.Click
        Try
            AcceptLotChange()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsAcceptButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsAcceptButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub bsCancelButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsCancelButton.Click
        Me.Close()
    End Sub











    '''' <summary>
    '''' Close the screen when button Cancel is clicked
    '''' </summary>
    '''' <remarks>
    '''' Created by:  DL 21/01/2010
    '''' Modified by: SA 23/09/2010 - Added the Dispose after the Close
    '''' Modified by: RH 18/10/2010 - Removed the Dispose after the Close
    '''' </remarks>
    'Private Sub bsExitButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsCancelButton.Click
    '    Try
    '        'Me.Close()
    '        AcceptAttribute = False

    '        'RH 17/12/2010
    '        If Not Me.MdiParent Is Nothing Then
    '            If Not Me.Tag Is Nothing Then
    '                'A PerformClick() method was executed
    '                Me.Close()
    '            Else
    '                'Normal button click
    '                'Open the WS Monitor form and close this one
    '                Ax00MainMDI.OpenMonitorForm(Me)
    '            End If
    '        Else
    '            Me.Close()
    '        End If

    '        'RH 18/10/2010 Don't call Dispose() on Managed Resources.
    '        'Let the Garbage Collector do that work.
    '        'When you close a form, its Dispose method is called automatically.
    '        'http://visualbasic.about.com/od/usingvbnet/a/disposeobj.htm
    '        'Me.Dispose()
    '    Catch ex As Exception
    '        CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "bsExitButton_Click " & Me.Name, EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
    '        ShowMessage(Me.Name & ".bsExitButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
    '    End Try
    'End Sub


    'Private Sub bsElementTextBox_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsLotNumberTextBox.TextChanged
    'bsAcceptButton.Enabled = (Not String.IsNullOrEmpty(bsLotNumberTextBox.Text.Trim))
    'End Sub


#End Region


End Class