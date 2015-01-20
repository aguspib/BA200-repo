Option Explicit On
Option Strict Off

Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.BL

Imports Biosystems.Ax00.CommunicationsSwFw

Imports System.Drawing
Imports System.Windows.Forms


Public Class ISettings


#Region "Constructor"
    ''' <summary>
    ''' New
    ''' </summary>
    ''' <remarks>Created by XBC 18/01/2012</remarks>
    Public Sub New()
        'Public Sub New(ByRef myMDI As Form)
        'RH 12/04/2012 myMDI not needed.

        'MyBase.New()
        'MyBase.SetParentMDI(myMDI)
        InitializeComponent()

    End Sub

    'Public Sub New()

    '    ' This call is required by the Windows Form Designer.
    '    InitializeComponent()

    '    ' Add any initialization after the InitializeComponent() call.

    'End Sub

#End Region


#Region "Declarations"
    Private currentLanguage As String                      'To store the current application language  
    Private LblParameters As String
    Private LblLimits As String

    Private EditionMode As Boolean = False                 'To validate when the selected Control is in Edition Mode
    Private ChangesMade As Boolean = False                 'To validate if there are changes pending to save when cancelling 

    Private selectedParameterID As Integer = -1              'To store the ID of the selected Control
    Private originalselectedindex As Integer = -1

    Dim myFieldLimitsDS As New FieldLimitsDS
    Dim myFieldParametersDS As New ParametersDS

    Private MinAllowedConcentration As Single              'To store the minimum allowed value for Min/Max Concentration fields
    Private MaxAllowedConcentration As Single              'To store the maximum allowed value for Min/Max Concentration fields

    Private AllAnalyzerModels As String = "Common"

    Private mdiAnalyzerCopy As AnalyzerManager 'DL 09/09/2011

    Private FirmwareVersionChanged As Boolean  ' XBC 14/09/2012
    Private myPackageVersion As String   ' XBC 14/09/2012
#End Region

#Region "Events"


    Private Sub FilterComboBox_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles FilterComboBox.SelectedIndexChanged
        If (SettingsListView.SelectedItems.Count = 1) Then
            If SettingsListView.SelectedItems.Item(0).Text.ToString = LblLimits Then
                LimitsDataGridView.Visible = True
                ParametersDataGridView.Visible = False

                Dim resultData As New GlobalDataTO
                Dim myFieldLimits As New FieldLimitsDelegate

                If FilterComboBox.Text <> AllAnalyzerModels Then
                    resultData = myFieldLimits.GetAllList(Nothing, FilterComboBox.Text)
                Else
                    resultData = myFieldLimits.GetAllList(Nothing)
                End If

                If (Not resultData.HasError) And (Not resultData.SetDatos Is Nothing) Then

                    myFieldLimitsDS = CType(resultData.SetDatos, FieldLimitsDS)

                    'Sort the Tests/SampleTypes by TestPosition
                    Dim viewTests As DataView = New DataView
                    viewTests = myFieldLimitsDS.tfmwFieldLimits.DefaultView
                    viewTests.Sort = "LimitID ASC "

                    'Set the sorted DS as DataSource of Tests/SampleTypes grid
                    Dim bsTestsBindingSource As BindingSource = New BindingSource
                    bsTestsBindingSource.DataSource = viewTests
                    LimitsDataGridView.DataSource = bsTestsBindingSource
                Else
                    LimitsDataGridView.DataSource = Nothing
                End If

            ElseIf SettingsListView.SelectedItems.Item(0).Text.ToString = LblParameters Then

                LimitsDataGridView.Visible = False
                ParametersDataGridView.Visible = True

                Dim resultData As New GlobalDataTO
                Dim myfieldParameters As New SwParametersDelegate

                If FilterComboBox.Text <> AllAnalyzerModels Then
                    resultData = myfieldParameters.GetAllList(Nothing, FilterComboBox.Text)
                Else
                    resultData = myfieldParameters.GetAllList(Nothing)
                End If

                If (Not resultData.HasError) And (Not resultData.SetDatos Is Nothing) Then

                    myFieldParametersDS = CType(resultData.SetDatos, ParametersDS)

                    'Sort the Tests/SampleTypes by TestPosition
                    Dim viewTests As DataView = New DataView
                    viewTests = myFieldParametersDS.tfmwSwParameters.DefaultView
                    viewTests.Sort = "ParameterID ASC "

                    'Set the sorted DS as DataSource of Tests/SampleTypes grid
                    Dim bsTestsBindingSource As BindingSource = New BindingSource
                    bsTestsBindingSource.DataSource = viewTests
                    ParametersDataGridView.DataSource = bsTestsBindingSource
                Else
                    ParametersDataGridView.DataSource = Nothing
                End If
            End If
        End If
    End Sub

    ''' <summary>
    ''' Validate Min/Max cells have been informed with allowed values
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 11/07/2011 
    ''' </remarks>
    Private Sub LimitsDataGridView_RowValidating(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellCancelEventArgs) Handles LimitsDataGridView.RowValidating
        Try
            ValidateTestGridRow(e.RowIndex)

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".LimitsDataGridView_RowValidating", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".LimitsDataGridView_RowValidating", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    '''' <summary>
    '''' Sort ascending/descending the list of Controls when clicking in the list header
    '''' </summary>
    '''' <remarks>
    '''' Created by:  DL 12/07/2011
    '''' </remarks>
    'Private Sub SettingsListView_ColumnClick(ByVal sender As Object, ByVal e As System.Windows.Forms.ColumnClickEventArgs) Handles SettingsListView.ColumnClick

    '    Try
    '        Select Case SettingsListView.Sorting
    '            Case SortOrder.None
    '                SettingsListView.Sorting = SortOrder.Ascending
    '                Exit Select
    '            Case SortOrder.Ascending
    '                SettingsListView.Sorting = SortOrder.Descending
    '                Exit Select
    '            Case SortOrder.Descending
    '                SettingsListView.Sorting = SortOrder.Ascending
    '                Exit Select
    '            Case Else
    '                Exit Select
    '        End Select
    '        SettingsListView.Sort()

    '    Catch ex As Exception
    '        CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".SettingsListView_ColumnClick", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
    '        ShowMessage(Me.Name & ".SettingsListView_ColumnClick", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
    '    End Try
    'End Sub

    ''' <summary>
    ''' Don't allow Users to resize the visible ListView column
    ''' </summary>
    Private Sub SettingsListView_ColumnWidthChanging(ByVal sender As Object, ByVal e As System.Windows.Forms.ColumnWidthChangingEventArgs) Handles SettingsListView.ColumnWidthChanging
        Try
            e.Cancel = True
            If (e.ColumnIndex = 0) Then
                e.NewWidth = 210
            Else
                e.NewWidth = 0
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".SettingsListView_ColumnWidthChanging", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".SettingsListView_ColumnWidthChanging", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    Private Sub SettingsListView_DoubleClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles SettingsListView.DoubleClick

        Try
            EditModeScreenStatus()

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".SettingsListView_DoubleClick", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".SettingsListView_DoubleClick", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Allow consultation of elements in the Controls list using the keyboard
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 11/07/2011
    ''' </remarks>
    Private Sub SettingsListView_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles SettingsListView.KeyUp

        Try
            If (SettingsListView.SelectedItems.Count = 1) Then
                QueryByListView()
            Else
                InitialModeScreenStatus(False)
                EditButton.Enabled = False
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".SettingsListView_KeyUp", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".SettingsListView_KeyUp", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' allow set the edition Mode for the selected SETTING  when Enter is pressed
    ''' </summary>
    Private Sub SettingsListView_PreviewKeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.PreviewKeyDownEventArgs) Handles SettingsListView.PreviewKeyDown
        Try
            If (e.KeyCode = Keys.Enter And EditButton.Enabled) Then
                EditModeScreenStatus()
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".SettingsListView_PreviewKeyDown", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".SettingsListView_PreviewKeyDown", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub


    ''' <summary>
    ''' Set the screen status to EDIT MODE 
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 11/07/2011 
    ''' </remarks>
    Private Sub EditButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles EditButton.Click
        Try
            EditModeScreenStatus()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".EditButton_Click", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".EditButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Save changes (update) in the database
    ''' </summary>
    ''' <remarks>
    ''' Create by: DL 11/07/2011
    ''' </remarks>   
    Private Sub SaveButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SaveButton.Click
        Try
            SaveChanges()

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".SaveButton_Click", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".SaveButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub


    ''' <summary>
    ''' Execute the cancelling of settings edition
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 11/07/2011
    ''' </remarks>    
    Private Sub CancelButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButtonCancel.Click
        Try
            CancelEdition()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".CancelButton_Click", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".CancelButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    Private Sub ParametersDataGridView_CellFormatting(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellFormattingEventArgs) Handles ParametersDataGridView.CellFormatting
        If e.ColumnIndex = ParametersDataGridView.Columns("ParameterName").Index AndAlso (e.Value IsNot Nothing) Then
            'AG 20/02/2014 - #1505
            'ParametersDataGridView.Rows(e.RowIndex).Cells(e.ColumnIndex).ToolTipText = ParametersDataGridView.Rows(e.RowIndex).Cells("Description").Value
            If Not ParametersDataGridView.Rows(e.RowIndex).Cells("Description") Is Nothing AndAlso Not DBNull.Value.Equals(ParametersDataGridView.Rows(e.RowIndex).Cells("Description").Value) Then
                ParametersDataGridView.Rows(e.RowIndex).Cells(e.ColumnIndex).ToolTipText = ParametersDataGridView.Rows(e.RowIndex).Cells("Description").Value
            End If

        End If
    End Sub

    Private Sub ParametersDataGridView_CellLeave(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles ParametersDataGridView.CellLeave, ParametersDataGridView.CellEndEdit

        Try
            If (ParametersDataGridView.Columns(e.ColumnIndex).Name = "ValueText") Then
                If ParametersDataGridView.Rows(e.RowIndex).Cells("ValueText").Value.ToString <> String.Empty Then
                    ParametersDataGridView.Rows(e.RowIndex).Cells("ValueNumeric").Value = DBNull.Value
                End If

            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ParametersDataGridView_CellLeave ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ParametersDataGridView_CellLeave", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub


    Private Sub ParametersDataGridView_CellPainting(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellPaintingEventArgs) Handles ParametersDataGridView.CellPainting

        If EditButton.Enabled Then

            If e.RowIndex > -1 Then
                e.CellStyle.BackColor = Color.WhiteSmoke

                For i As Integer = 0 To ParametersDataGridView.Columns.Count - 1
                    ParametersDataGridView.Columns(i).ReadOnly = True
                Next i
            End If

        Else

            If e.RowIndex > -1 Then
                ParametersDataGridView.Rows(e.RowIndex).Cells("ValueNumeric").ReadOnly = False
                ParametersDataGridView.Rows(e.RowIndex).Cells("ValueText").ReadOnly = False

                If (e.ColumnIndex = ParametersDataGridView.Columns("ParameterName").Index OrElse e.ColumnIndex = ParametersDataGridView.Columns("AnalyzerModel").Index) Then
                    e.CellStyle.BackColor = Color.WhiteSmoke
                End If
            End If
        End If

        If e.RowIndex > -1 AndAlso e.ColumnIndex = ParametersDataGridView.Columns("ValueNumeric").Index Then
            e.CellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
        End If

    End Sub

    Private Sub ParametersDataGridView_CellValidating(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellValidatingEventArgs) Handles ParametersDataGridView.CellValidating
        Try
            If ParametersDataGridView.Columns(e.ColumnIndex).Name = "ValueNumeric" Then
                If (Not e.FormattedValue Is DBNull.Value AndAlso Not e.FormattedValue Is Nothing AndAlso e.FormattedValue.ToString.Trim <> "" AndAlso _
                    Not IsNumeric(e.FormattedValue)) Then
                    e.Cancel = True
                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ParametersDataGridView_CellValidating ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ParametersDataGridView_CellValidating", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub ParametersDataGridView_EditingControlShowing(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewEditingControlShowingEventArgs) Handles ParametersDataGridView.EditingControlShowing
        Try
            Dim currentCol As Integer = ParametersDataGridView.CurrentCell.ColumnIndex
            Dim currentRow As Integer = ParametersDataGridView.CurrentCell.RowIndex

            If (ParametersDataGridView.CurrentRow.Index >= 0) AndAlso _
               (ParametersDataGridView.Columns(currentCol).Name = "ValueNumeric") Then
                DirectCast(e.Control, DataGridViewTextBoxEditingControl).ShortcutsEnabled = False

                AddHandler e.Control.KeyPress, AddressOf CheckCell
                'ChangesMade = True
            Else
                RemoveHandler e.Control.KeyPress, AddressOf CheckCell
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ParametersDataGridView_EditingControlShowing ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ParametersDataGridView_EditingControlShowing", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub LimitsDataGridView_CellFormatting(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellFormattingEventArgs) Handles LimitsDataGridView.CellFormatting
        If e.ColumnIndex = LimitsDataGridView.Columns("LimitID").Index AndAlso (e.Value IsNot Nothing) Then
            'AG 20/02/2014 - #1505
            'LimitsDataGridView.Rows(e.RowIndex).Cells(e.ColumnIndex).ToolTipText = LimitsDataGridView.Rows(e.RowIndex).Cells("LimitDescription").Value
            If Not LimitsDataGridView.Rows(e.RowIndex).Cells("LimitDescription") Is Nothing AndAlso Not DBNull.Value.Equals(LimitsDataGridView.Rows(e.RowIndex).Cells("LimitDescription").Value) Then
                LimitsDataGridView.Rows(e.RowIndex).Cells(e.ColumnIndex).ToolTipText = LimitsDataGridView.Rows(e.RowIndex).Cells("LimitDescription").Value
            End If
        End If
    End Sub

    '''' <summary>
    '''' Recalculate values of Target Mean and SD when Min and/or Max are changed
    '''' </summary>
    '''' <remarks>
    '''' Modified by: DL 12/07/2011 
    '''' </remarks>
    'Private Sub LimitsDataGridView_CellLeave(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles LimitsDataGridView.CellLeave
    '    Try
    '        If (LimitsDataGridView.Columns(e.ColumnIndex).Name = "MinValue" OrElse LimitsDataGridView.Columns(e.ColumnIndex).Name = "MaxValue") Then
    '            SetLimitValues(e.RowIndex, e.ColumnIndex)
    '        End If
    '    Catch ex As Exception
    '        CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".LimitsDataGridView_CellLeave ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
    '        ShowMessage(Me.Name & ".LimitsDataGridView_CellLeave", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
    '    End Try
    'End Sub

    Private Sub LimitsDataGridView_CellPainting(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellPaintingEventArgs) Handles LimitsDataGridView.CellPainting

        If EditButton.Enabled Then

            If e.RowIndex > -1 Then
                e.CellStyle.BackColor = Color.WhiteSmoke

                For i As Integer = 0 To LimitsDataGridView.Columns.Count - 1
                    LimitsDataGridView.Columns(i).ReadOnly = True
                Next i
            End If

        Else

            If e.RowIndex > -1 Then
                LimitsDataGridView.Rows(e.RowIndex).Cells("MinValue").ReadOnly = False
                LimitsDataGridView.Rows(e.RowIndex).Cells("Maxvalue").ReadOnly = False
                LimitsDataGridView.Rows(e.RowIndex).Cells("StepValue").ReadOnly = False
                LimitsDataGridView.Rows(e.RowIndex).Cells("DefaultValue").ReadOnly = False
                LimitsDataGridView.Rows(e.RowIndex).Cells("DecimalsAllowed").ReadOnly = False

                If (e.ColumnIndex = LimitsDataGridView.Columns("LimitID").Index OrElse e.ColumnIndex = LimitsDataGridView.Columns("AnalyzerModel").Index) Then
                    e.CellStyle.BackColor = Color.WhiteSmoke
                End If
            End If
        End If

        If e.RowIndex > -1 AndAlso (e.ColumnIndex = LimitsDataGridView.Columns("MinValue").Index OrElse _
                                    e.ColumnIndex = LimitsDataGridView.Columns("Maxvalue").Index OrElse _
                                    e.ColumnIndex = LimitsDataGridView.Columns("StepValue").Index OrElse _
                                    e.ColumnIndex = LimitsDataGridView.Columns("DefaultValue").Index OrElse _
                                    e.ColumnIndex = LimitsDataGridView.Columns("DecimalsAllowed").Index) Then

            If e.ErrorText <> "" Then
                e.CellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            Else
                e.CellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            End If

        End If
    End Sub



    Private Sub LimitsDataGridView_CellValidating(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellValidatingEventArgs) Handles LimitsDataGridView.CellValidating
        Try
            If LimitsDataGridView.Columns(e.ColumnIndex).Name = "MinValue" OrElse LimitsDataGridView.Columns(e.ColumnIndex).Name = "MaxValue" OrElse _
               LimitsDataGridView.Columns(e.ColumnIndex).Name = "StepValue" OrElse LimitsDataGridView.Columns(e.ColumnIndex).Name = "DefaultValue" OrElse _
               LimitsDataGridView.Columns(e.ColumnIndex).Name = "DecimalsAllowed" Then

                If (Not e.FormattedValue Is DBNull.Value AndAlso Not e.FormattedValue Is Nothing AndAlso e.FormattedValue.ToString.Trim <> "" AndAlso _
                    Not IsNumeric(e.FormattedValue)) Then
                    e.Cancel = True
                End If

            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".LimitsDataGridView_CellValidating ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".LimitsDataGridView_CellValidating", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub LimitsDataGridView_EditingControlShowing(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewEditingControlShowingEventArgs) Handles LimitsDataGridView.EditingControlShowing
        Try
            Dim currentCol As Integer = LimitsDataGridView.CurrentCell.ColumnIndex
            Dim currentRow As Integer = LimitsDataGridView.CurrentCell.RowIndex

            If (LimitsDataGridView.CurrentRow.Index > -1) AndAlso _
               (LimitsDataGridView.Columns(currentCol).Name = "MinValue" OrElse LimitsDataGridView.Columns(currentCol).Name = "MaxValue" OrElse _
                LimitsDataGridView.Columns(currentCol).Name = "StepValue" OrElse LimitsDataGridView.Columns(currentCol).Name = "DefaultValue" OrElse _
                LimitsDataGridView.Columns(currentCol).Name = "DecimalsAllowed") Then

                DirectCast(e.Control, DataGridViewTextBoxEditingControl).ShortcutsEnabled = False

                AddHandler e.Control.KeyPress, AddressOf CheckCell
                'ChangesMade = True
            Else
                RemoveHandler e.Control.KeyPress, AddressOf CheckCell
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".LimitsDataGridView_EditingControlShowing ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".LimitsDataGridView_EditingControlShowing", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub



    ''' <summary>
    ''' Validate if the value enter is numeric or one of the allowed decimals separators 
    ''' </summary>
    ''' <remarks>
    ''' Created by:  TR 05/05/2011
    ''' Modified by: SA 18/05/2011 - Allow characters "." and "," as decimal separators. Avoid more than one decimal
    '''                              separator in a cell
    ''' Modified by: DL 12/11/2011 - Allow characters "-". Avoid more than one minus signal in a cell
    ''' </remarks>
    Private Sub CheckCell(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyPressEventArgs)
        Try
            Dim myDecimalSeparator As String = SystemInfoManager.OSDecimalSeparator

            If (e.KeyChar = CChar(".") OrElse e.KeyChar = CChar(",") OrElse e.KeyChar = CChar("") OrElse e.KeyChar = ChrW(Keys.Back) OrElse e.KeyChar = CChar("-")) Then
                If (e.KeyChar = CChar(".") OrElse e.KeyChar = CChar(",")) Then
                    e.KeyChar = CChar(myDecimalSeparator)

                    If (CType(sender, TextBox).Text.Contains(".") Or CType(sender, TextBox).Text.Contains(",")) Then
                        e.Handled = True
                    Else
                        e.Handled = False
                    End If

                ElseIf e.KeyChar = CChar("-") Then

                    If CType(sender, TextBox).Text.Contains("-") Then
                        e.Handled = True
                    Else
                        If CType(sender, TextBox).SelectionStart = 0 Then
                            e.Handled = False
                        Else
                            e.Handled = True
                        End If
                    End If
                End If

                'If (CType(sender, TextBox).Text.Contains(".") Or CType(sender, TextBox).Text.Contains(",")) Then
                '    e.Handled = True
                'Else
                '    e.Handled = False
                'End If

                'If CType(sender, TextBox).Text.Contains("-") Then

            Else
                If (Not IsNumeric(e.KeyChar)) Then
                    e.Handled = True
                End If
            End If



        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".CheckCell ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".CheckCell ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub ISettings_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyDown
        Try
            If (e.KeyCode = Keys.Escape) Then
                If (ButtonCancel.Enabled) Then
                    ButtonCancel.PerformClick()
                Else
                    CloseButton.PerformClick()
                End If

            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ISettings_KeyDown", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ISettings_KeyDown", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Load form
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>
    ''' Created by DL 07/07/2011
    '''</remarks>
    Private Sub ISettings_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Try
            ScreenLoad()

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ISettings_Load", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ISettings_Load", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)

        End Try
    End Sub


    ''' <summary>
    ''' Close form
    ''' </summary>
    ''' <remarks>
    ''' Created by: DL 07/07/2011
    ''' </remarks>
    Private Sub CloseButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CloseButton.Click
        Try

            If (PendingChangesVerification()) Then
                If (ShowMessage(Me.Name, GlobalEnumerates.Messages.DISCARD_PENDING_CHANGES.ToString) = DialogResult.Yes) Then
                    Me.Enabled = False 'RH 13/04/2012 Disable the form to avoid pressing buttons on closing
                    Close()
                Else
                    If (originalselectedindex <> -1) Then
                        'Return focus to the control that has been edited
                        SettingsListView.Items(originalselectedindex).Selected = True
                        SettingsListView.Select()
                    End If
                End If
            Else
                Me.Enabled = False 'RH 13/04/2012 Disable the form to avoid pressing buttons on closing
                Close()
            End If

            'If (EditionMode) Then
            '    If (PendingChangesVerification()) Then
            '        If (ShowMessage(Me.Name, GlobalEnumerates.Messages.DISCARD_PENDING_CHANGES.ToString) = Windows.Forms.DialogResult.No) Then
            '            screenClose = False
            '        End If
            '    End If
            'End If

            'If (screenClose) Then
            '    If (Not Me.Tag Is Nothing) Then
            '        'A PerformClick() method was executed
            '        Me.Close()
            '    Else
            '        'Normal button click - Open the WS Monitor form and close this one
            '        IAx00MainMDI.OpenMonitorForm(Me)
            '    End If
            'Else
            '    bsControlNameTextbox.Focus()
            'End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".CloseButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".CloseButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try

    End Sub

    Private Sub SettingsListView_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles SettingsListView.Click
        Try
            If (SettingsListView.SelectedItems.Count = 1) Then
                QueryByListView()
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".SettingsListView_Click", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".SettingsListView_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

#End Region

#Region "Private Methods"

    Private Sub LoadAnalyzerCombo()
        Try
            Dim AnalyzerModel As New AnalyzerModelRotorsConfigDelegate
            Dim resultdata As New GlobalDataTO

            resultdata = AnalyzerModel.GetAnalyzerModel(Nothing)

            If (Not resultdata.HasError) And (Not resultdata.SetDatos Is Nothing) Then
                Dim myAnalyzerModelDS As New AnalyzerModelDS
                myAnalyzerModelDS = DirectCast(resultdata.SetDatos, AnalyzerModelDS)

                FilterComboBox.Items.Clear()

                For Each row As AnalyzerModelDS.tfmwAnalyzerModelsRow In myAnalyzerModelDS.tfmwAnalyzerModels.Rows
                    FilterComboBox.Items.Add(row.AnalyzerModel)
                Next row

                FilterComboBox.Items.Add(AllAnalyzerModels)
            End If


        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".LoadAnalyzerCombo", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".LoadAnalyzerCombo", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Read Firmware Version from database
    ''' </summary>
    ''' <remarks>Created by XBC 14/09/2012</remarks>
    Private Sub LoadFirmwareVersion()
        Try
            Dim myVersionsDelegate As New VersionsDelegate
            Dim myVersionsDS As VersionsDS
            Dim myGlobal As New GlobalDataTO

            myGlobal = myVersionsDelegate.GetVersionsData(Nothing)
            If (Not myGlobal.HasError) And (Not myGlobal.SetDatos Is Nothing) Then
                myVersionsDS = CType(myGlobal.SetDatos, VersionsDS)

                Dim myUtil As New Utilities
                myGlobal = myUtil.GetSoftwareVersion()
                If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
                    Dim SwVersion As String = myGlobal.SetDatos.ToString

                    Dim myVersion As List(Of VersionsDS.tfmwVersionsRow)
                    'SGM 01/02/2012 - Check if it is Service Assembly - Bug #1112
                    'If My.Application.Info.AssemblyName.ToUpper.Contains("SERVICE") Then
                    If GlobalBase.IsServiceAssembly Then
                        myVersion = (From a In myVersionsDS.tfmwVersions Where a.ServiceSoftware = SwVersion Select a).ToList()
                    Else
                        myVersion = (From a In myVersionsDS.tfmwVersions Where a.UserSoftware = SwVersion Select a).ToList()
                    End If

                    If myVersion.Count > 0 Then
                        BsFwVersionTextBox.Text = myVersion.Item(0).Firmware
                        myPackageVersion = myVersion.Item(0).PackageID
                    End If
                End If
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".LoadFirmwareVersion", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".LoadFirmwareVersion", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub


    ''' <summary>
    ''' Load all data needed for the screen
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 11/07/2011 
    ''' </remarks>
    Private Sub ScreenLoad()
        Try
            Cursor = Cursors.WaitCursor
            '
            'Get the current Language from the current Application Session
            Dim currentLanguageGlobal As New GlobalBase
            currentLanguage = currentLanguageGlobal.GetSessionInfo().ApplicationLanguage.Trim.ToString

            'DL 09/09/2011
            If Not AppDomain.CurrentDomain.GetData("GlobalAnalyzerManager") Is Nothing Then
                mdiAnalyzerCopy = CType(AppDomain.CurrentDomain.GetData("GlobalAnalyzerManager"), AnalyzerManager) 'AG 13/07/2011 - Use the same AnalyzerManager as the MDI
            End If
            'DL 09/09/2011

            'Load the multilanguage texts for all Screen Labels
            GetScreenLabels()
            '
            'Get Icons for graphical buttons
            PrepareButtons()
            '
            ' Load Settings list view
            PrepareSettingListView()
            '
            PrepareParametersSettingsGrid()
            PrepareLimitsSettingsGrid()
            '
            LoadAnalyzerCombo()
            InitialModeScreenStatus()
            '
            If SettingsListView.Items.Count > 0 Then
                SettingsListView.Items(0).Selected = True
                SettingsListView_Click(Nothing, Nothing)
            End If

            'DL 09/09/2011
            If Not mdiAnalyzerCopy Is Nothing Then
                If mdiAnalyzerCopy.AnalyzerStatus = GlobalEnumerates.AnalyzerManagerStatus.RUNNING Then
                    bsLoadSaveGroupBox.Enabled = False
                End If
            End If
            'END DL 09/09/2011

            ' XBC 14/09/2012
            Me.BsFwVersionTextBox.Enabled = False
            LoadFirmwareVersion()
            FirmwareVersionChanged = False

            ' XBC 19/01/2012
            'SGM 01/02/2012 - Check if it is Service Assembly - Bug #1112
            'If My.Application.Info.AssemblyName.ToUpper.Contains("SERVICE") Then
            If GlobalBase.IsServiceAssembly Then
                ResetBorderSRV()
            End If
            ' XBC 19/01/2012

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ScreenLoad", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ScreenLoad", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")

        Finally
            Cursor = Cursors.Default
        End Try
    End Sub



    ''' <summary>
    ''' Get texts in the current application language for all screen controls
    ''' </summary>
    ''' <remarks>
    ''' '<param name="pLanguageID"> The current Language of Application </param>
    ''' Created by:  DL 07/07/2011
    ''' </remarks>
    Private Sub GetScreenLabels()
        Try
            'Get the current Language from the current Application Session
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            'For Labels.....
            bsSettingsLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Settings", currentLanguage)
            BsShowLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_AnalyzerModel", currentLanguage)


            ' Fot tooltips
            bsScreenToolTips.SetToolTip(CloseButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_CloseScreen", currentLanguage))
            bsScreenToolTips.SetToolTip(ButtonCancel, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Cancel", currentLanguage))
            bsScreenToolTips.SetToolTip(EditButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Edit", currentLanguage))
            bsScreenToolTips.SetToolTip(SaveButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Save", currentLanguage))

            ' for listview
            LblParameters = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Parameters", currentLanguage)
            LblLimits = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Limits", currentLanguage)

            AllAnalyzerModels = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Common", currentLanguage)

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".GetScreenLabels ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".GetScreenLabels ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Method incharge to load the image for each graphical button 
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 07/07/2011
    ''' </remarks>
    Private Sub PrepareButtons()
        Try
            Dim auxIconName As String = ""
            Dim iconPath As String = IconsPath

            ' Close Button
            auxIconName = GetIconName("CANCEL")
            If (auxIconName <> "") Then
                CloseButton.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
            End If

            ' Edit EditButton
            auxIconName = GetIconName("EDIT")
            If (auxIconName <> "") Then
                EditButton.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
            End If

            ' Save SaveButton
            auxIconName = GetIconName("SAVE")
            If (auxIconName <> "") Then
                SaveButton.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
            End If

            ' Undo Button
            auxIconName = GetIconName("UNDO")
            If (auxIconName <> "") Then
                ButtonCancel.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
            End If

            'PRINT Button
            auxIconName = GetIconName("PRINT")
            If (auxIconName <> "") Then
                PrintButton.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
            End If


        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".PrepareButtons ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".PrepareButtons ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Configure and fill the settinglistView
    ''' </summary>
    ''' <remarks>
    ''' Created by: DL 07/07/2011
    ''' </remarks>
    Private Sub PrepareSettingListView()
        'Private Sub PrepareTestListView()
        Try
            'Initialize 
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate
            '
            'Initialization of setting list
            SettingsListView.Items.Clear()
            SettingsListView.Alignment = ListViewAlignment.Left
            SettingsListView.FullRowSelect = True
            SettingsListView.HeaderStyle = ColumnHeaderStyle.Clickable
            SettingsListView.MultiSelect = False
            SettingsListView.Scrollable = True
            SettingsListView.View = View.Details
            SettingsListView.HideSelection = False
            '
            SettingsListView.Columns.Add(myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SWParameter", currentLanguage), 0, HorizontalAlignment.Left)
            SettingsListView.Columns(0).Width = 180
            '
            SettingsListView.Items.Add(LblParameters)
            SettingsListView.Items.Add(LblLimits)

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " PrepareSettingListView ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub


    ''' <summary>
    ''' Configure the grid for details of settings data grid
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 07/07/2011
    ''' </remarks>
    Private Sub PrepareLimitsSettingsGrid()
        Try
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            LimitsDataGridView.Rows.Clear()
            LimitsDataGridView.Columns.Clear()
            '
            LimitsDataGridView.AutoGenerateColumns = False
            LimitsDataGridView.AllowUserToAddRows = False
            LimitsDataGridView.AllowUserToDeleteRows = False
            LimitsDataGridView.EditMode = DataGridViewEditMode.EditOnEnter
            LimitsDataGridView.MultiSelect = False

            'Name Identifier (Common column)
            Dim SettingIDCol As New DataGridViewTextBoxColumn
            SettingIDCol.Name = "LimitID"
            SettingIDCol.DataPropertyName = "LimitID"
            SettingIDCol.HeaderText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Name", currentLanguage)
            SettingIDCol.ReadOnly = True
            'SettingIDCol.Visible = True
            LimitsDataGridView.Columns.Add(SettingIDCol)
            LimitsDataGridView.Columns("LimitID").Width = 220
            LimitsDataGridView.Columns("LimitID").SortMode = DataGridViewColumnSortMode.Automatic
            LimitsDataGridView.Columns("LimitID").HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft

            'Name Identifier (Common column)
            Dim SettingDescriptionCol As New DataGridViewTextBoxColumn
            SettingDescriptionCol.Name = "LimitDescription"
            SettingDescriptionCol.DataPropertyName = "LimitDescription"
            SettingDescriptionCol.Visible = False
            LimitsDataGridView.Columns.Add(SettingDescriptionCol)

            'Analyzer model
            Dim AnalyzerModelCol As New DataGridViewTextBoxColumn
            AnalyzerModelCol.Name = "AnalyzerModel"
            AnalyzerModelCol.DataPropertyName = "AnalyzerModel"
            AnalyzerModelCol.HeaderText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_AnalyzerModel", currentLanguage)
            AnalyzerModelCol.ReadOnly = True
            LimitsDataGridView.Columns.Add(AnalyzerModelCol)
            LimitsDataGridView.Columns("AnalyzerModel").Width = 80
            LimitsDataGridView.Columns("AnalyzerModel").SortMode = DataGridViewColumnSortMode.NotSortable
            LimitsDataGridView.Columns("AnalyzerModel").HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft

            'Min (Limits)
            Dim MinCol As New DataGridViewTextBoxColumn
            MinCol.Name = "MinValue"
            MinCol.DataPropertyName = "MinValue"
            MinCol.HeaderText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_MinValue", currentLanguage)
            LimitsDataGridView.Columns.Add(MinCol)
            LimitsDataGridView.Columns("MinValue").Width = 75
            LimitsDataGridView.Columns("MinValue").SortMode = DataGridViewColumnSortMode.NotSortable
            LimitsDataGridView.Columns("MinValue").HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight

            'Max (Limits)
            Dim MaxCol As New DataGridViewTextBoxColumn
            MaxCol.Name = "MaxValue"
            MaxCol.DataPropertyName = "MaxValue"
            MaxCol.HeaderText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_MaxValue", currentLanguage)
            LimitsDataGridView.Columns.Add(MaxCol)
            LimitsDataGridView.Columns("MaxValue").Width = 85
            LimitsDataGridView.Columns("MaxValue").SortMode = DataGridViewColumnSortMode.NotSortable
            LimitsDataGridView.Columns("MaxValue").HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight

            'Step (Limits)
            Dim StepCol As New DataGridViewTextBoxColumn
            StepCol.Name = "StepValue"
            StepCol.DataPropertyName = "StepValue"
            StepCol.HeaderText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_StepValue", currentLanguage)
            LimitsDataGridView.Columns.Add(StepCol)
            LimitsDataGridView.Columns("StepValue").Width = 75
            LimitsDataGridView.Columns("StepValue").SortMode = DataGridViewColumnSortMode.NotSortable
            LimitsDataGridView.Columns("StepValue").HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight

            'Default (Limits)
            Dim DefaultCol As New DataGridViewTextBoxColumn
            DefaultCol.Name = "DefaultValue"
            DefaultCol.DataPropertyName = "DefaultValue"
            DefaultCol.HeaderText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_DefaultValue", currentLanguage)
            LimitsDataGridView.Columns.Add(DefaultCol)
            LimitsDataGridView.Columns("DefaultValue").Width = 75
            LimitsDataGridView.Columns("DefaultValue").SortMode = DataGridViewColumnSortMode.NotSortable
            LimitsDataGridView.Columns("DefaultValue").HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight

            'Decimals (Limits)
            Dim DecimalsCol As New DataGridViewTextBoxColumn
            DecimalsCol.Name = "DecimalsAllowed"
            DecimalsCol.DataPropertyName = "DecimalsAllowed"
            DecimalsCol.HeaderText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_DecimalsAllowed", currentLanguage)
            LimitsDataGridView.Columns.Add(DecimalsCol)
            LimitsDataGridView.Columns("DecimalsAllowed").Width = 75
            LimitsDataGridView.Columns("DecimalsAllowed").SortMode = DataGridViewColumnSortMode.NotSortable
            LimitsDataGridView.Columns("DecimalsAllowed").HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".PrepareSettingsGrid " & Name, EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PrepareSettingsGrid ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub


    ''' <summary>
    ''' Configure the grid for details of settings data grid
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 11/07/2011
    ''' </remarks>
    Private Sub PrepareParametersSettingsGrid()
        Try
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            ParametersDataGridView.Rows.Clear()
            ParametersDataGridView.Columns.Clear()
            '
            ParametersDataGridView.AutoGenerateColumns = False
            ParametersDataGridView.AllowUserToAddRows = False
            ParametersDataGridView.AllowUserToDeleteRows = False
            ParametersDataGridView.EditMode = DataGridViewEditMode.EditOnEnter
            ParametersDataGridView.MultiSelect = False

            'Name
            Dim SettingDescriptionCol As New DataGridViewTextBoxColumn
            SettingDescriptionCol.Name = "ParameterID"
            SettingDescriptionCol.DataPropertyName = "ParameterID"
            SettingDescriptionCol.Visible = False
            ParametersDataGridView.Columns.Add(SettingDescriptionCol)

            'Identifier
            Dim SettingIDCol As New DataGridViewTextBoxColumn
            SettingIDCol.Name = "ParameterName"
            SettingIDCol.DataPropertyName = "ParameterName"
            SettingIDCol.HeaderText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Name", currentLanguage)
            SettingIDCol.ReadOnly = True
            ParametersDataGridView.Columns.Add(SettingIDCol)
            ParametersDataGridView.Columns("ParameterName").Width = 260
            ParametersDataGridView.Columns("ParameterName").SortMode = DataGridViewColumnSortMode.Automatic
            ParametersDataGridView.Columns("ParameterName").HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft

            'Name
            Dim DescriptionCol As New DataGridViewTextBoxColumn
            DescriptionCol.Name = "Description"
            DescriptionCol.DataPropertyName = "Description"
            DescriptionCol.Visible = False
            ParametersDataGridView.Columns.Add(DescriptionCol)

            'Depend by model
            Dim SettingModelCol As New DataGridViewTextBoxColumn
            SettingModelCol.Name = "DependByModel"
            SettingModelCol.DataPropertyName = "DependByModel"
            SettingModelCol.Visible = False
            ParametersDataGridView.Columns.Add(SettingModelCol)

            'Analyzer model
            Dim AnalyzerModelCol As New DataGridViewTextBoxColumn
            AnalyzerModelCol.Name = "AnalyzerModel"
            AnalyzerModelCol.DataPropertyName = "AnalyzerModel"
            AnalyzerModelCol.HeaderText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_AnalyzerModel", currentLanguage)
            AnalyzerModelCol.ReadOnly = True
            ParametersDataGridView.Columns.Add(AnalyzerModelCol)
            ParametersDataGridView.Columns("AnalyzerModel").Width = 80
            ParametersDataGridView.Columns("AnalyzerModel").SortMode = DataGridViewColumnSortMode.NotSortable
            ParametersDataGridView.Columns("AnalyzerModel").HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft

            'Value Numeric
            Dim ValueNumCol As New DataGridViewTextBoxColumn
            ValueNumCol.Name = "ValueNumeric"
            ValueNumCol.DataPropertyName = "ValueNumeric"
            ValueNumCol.HeaderText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_ValueNumeric", currentLanguage)
            ParametersDataGridView.Columns.Add(ValueNumCol)
            ParametersDataGridView.Columns("ValueNumeric").Width = 80
            ParametersDataGridView.Columns("ValueNumeric").SortMode = DataGridViewColumnSortMode.NotSortable
            ParametersDataGridView.Columns("ValueNumeric").HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight

            'Description
            Dim ValueTxtCol As New DataGridViewTextBoxColumn
            ValueTxtCol.Name = "ValueText"
            ValueTxtCol.DataPropertyName = "ValueText"
            ValueTxtCol.HeaderText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_ValueText", currentLanguage)
            ValueTxtCol.MaxInputLength = 50
            ParametersDataGridView.Columns.Add(ValueTxtCol)
            ParametersDataGridView.Columns("ValueText").Width = 250
            ParametersDataGridView.Columns("ValueText").SortMode = DataGridViewColumnSortMode.NotSortable
            ParametersDataGridView.Columns("ValueText").HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".PrepareSettingsGrid " & Name, EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PrepareSettingsGrid ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub



    ''' <summary>
    ''' Set the screen controls to INITIAL MODE
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 12/07/2011
    ''' </remarks> 
    Private Sub InitialModeScreenStatus(Optional ByVal pInitializeListView As Boolean = True)
        Try
            'Set variable for control of changes pending to save to FALSE
            ChangesMade = False
            EditionMode = False

            If (pInitializeListView) Then
                EditButton.Enabled = True
                ButtonCancel.Enabled = False
                SaveButton.Enabled = False
                FilterComboBox.Enabled = True
            End If

            FilterComboBox.SelectedIndex = FilterComboBox.Items.Count - 1

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".InitialModeScreenStatus ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".InitialModeScreenStatus", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    Private Sub QueryByElement()

        Try

            If SettingsListView.SelectedItems.Count > 0 AndAlso originalselectedindex <> SettingsListView.SelectedItems.Item(0).Index Then
                EditButton.Enabled = True
                SaveButton.Enabled = False
                ButtonCancel.Enabled = False
                FilterComboBox.Enabled = True

                If SettingsListView.SelectedItems.Item(0).Text.ToString = LblLimits Then
                    LimitsDataGridView.Visible = True
                    ParametersDataGridView.Visible = False

                    Dim resultData As New GlobalDataTO
                    Dim myFieldLimits As New FieldLimitsDelegate

                    If FilterComboBox.Text <> AllAnalyzerModels Then
                        resultData = myFieldLimits.GetAllList(Nothing, FilterComboBox.Text)
                    Else
                        resultData = myFieldLimits.GetAllList(Nothing)
                    End If

                    If (Not resultData.HasError) And (Not resultData.SetDatos Is Nothing) Then

                        myFieldLimitsDS = CType(resultData.SetDatos, FieldLimitsDS)

                        'Sort the Tests/SampleTypes by TestPosition
                        Dim viewTests As DataView = New DataView
                        viewTests = myFieldLimitsDS.tfmwFieldLimits.DefaultView
                        viewTests.Sort = "LimitID ASC "

                        'Set the sorted DS as DataSource of Tests/SampleTypes grid
                        Dim bsTestsBindingSource As BindingSource = New BindingSource
                        bsTestsBindingSource.DataSource = viewTests
                        LimitsDataGridView.DataSource = bsTestsBindingSource
                    Else
                        LimitsDataGridView.DataSource = Nothing
                    End If

                ElseIf SettingsListView.SelectedItems.Item(0).Text.ToString = LblParameters Then

                    LimitsDataGridView.Visible = False
                    ParametersDataGridView.Visible = True

                    Dim resultData As New GlobalDataTO
                    Dim myfieldParameters As New SwParametersDelegate

                    If FilterComboBox.Text <> AllAnalyzerModels Then
                        resultData = myfieldParameters.GetAllList(Nothing, FilterComboBox.Text)
                    Else
                        resultData = myfieldParameters.GetAllList(Nothing)
                    End If

                    If (Not resultData.HasError) And (Not resultData.SetDatos Is Nothing) Then

                        myFieldParametersDS = CType(resultData.SetDatos, ParametersDS)

                        'Sort the Tests/SampleTypes by TestPosition
                        Dim viewTests As DataView = New DataView
                        viewTests = myFieldParametersDS.tfmwSwParameters.DefaultView
                        viewTests.Sort = "ParameterID ASC "

                        'Set the sorted DS as DataSource of Tests/SampleTypes grid
                        Dim bsTestsBindingSource As BindingSource = New BindingSource
                        bsTestsBindingSource.DataSource = viewTests
                        ParametersDataGridView.DataSource = bsTestsBindingSource
                    Else
                        ParametersDataGridView.DataSource = Nothing
                    End If
                End If

            Else

            End If

            If SettingsListView.SelectedItems.Count > 0 Then originalselectedindex = SettingsListView.SelectedItems.Item(0).Index

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".QueryByElement", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".QueryByElement", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Get data of the selected parameter
    ''' </summary>
    ''' <remarks>
    ''' Created by: DL 07/07/2011
    ''' </remarks>
    Private Sub QueryByListView()
        Try

            If (PendingChangesVerification()) Then
                If (ShowMessage(Me.Name, GlobalEnumerates.Messages.DISCARD_PENDING_CHANGES.ToString) = Windows.Forms.DialogResult.Yes) Then
                    QueryByElement()
                Else

                    If (originalselectedindex <> -1) Then
                        'Return focus to the control that has been edited
                        SettingsListView.Items(originalselectedindex).Selected = True
                        SettingsListView.Select()
                    End If
                End If
            Else
                QueryByElement()
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".QueryParameter", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".QueryParameter", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub


    ''' <summary>
    ''' Save data of the updated Parameters
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 11/07/2011
    ''' </remarks>
    Private Sub SaveParametersChanges()
        Try

            Dim myParametersDS As New ParametersDS
            myParametersDS = LoadParameters()

            If myParametersDS.tfmwSwParameters.Count > 0 Then
                Dim myGlobalDataTO As New GlobalDataTO
                Dim mySwParametersDelegate As New SwParametersDelegate

                myGlobalDataTO = mySwParametersDelegate.SaveParameters(Nothing, myParametersDS)
            End If


        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".SaveParametersChanges", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".SaveParametersChanges", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try

    End Sub


    ''' <summary>
    ''' Save Frimware version changes
    ''' </summary>
    ''' <remarks>Created by XBC 14/09/2012</remarks>
    Private Sub SaveFirmwareVersionChanges()
        Try
            Dim myGlobalDataTO As New GlobalDataTO
            Dim myVerionsDelegate As New VersionsDelegate

            myGlobalDataTO = myVerionsDelegate.SaveFirmwareVersion(Nothing, myPackageVersion, Me.BsFwVersionTextBox.Text)

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".SaveFirmwareVersionChanges", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".SaveFirmwareVersionChanges", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try

    End Sub


    Private Function LoadLimits() As FieldLimitsDS
        Dim myLimitsDS As New FieldLimitsDS

        Try
            Dim limitsRow As FieldLimitsDS.tfmwFieldLimitsRow


            For Each dgvRow As DataGridViewRow In LimitsDataGridView.Rows
                limitsRow = myLimitsDS.tfmwFieldLimits.NewtfmwFieldLimitsRow

                limitsRow.LimitID = dgvRow.Cells("LimitID").Value.ToString
                limitsRow.LimitDescription = dgvRow.Cells("LimitDescription").Value.ToString

                If Not dgvRow.Cells("MinValue").Value.ToString Is String.Empty Then
                    limitsRow.MinValue = dgvRow.Cells("MinValue").Value.ToString
                End If

                If Not dgvRow.Cells("MaxValue").Value.ToString Is String.Empty Then
                    limitsRow.MaxValue = dgvRow.Cells("MaxValue").Value.ToString
                End If

                If Not dgvRow.Cells("StepValue").Value.ToString Is String.Empty Then
                    limitsRow.StepValue = dgvRow.Cells("StepValue").Value.ToString
                End If

                If Not dgvRow.Cells("DefaultValue").Value.ToString Is String.Empty Then
                    limitsRow.DefaultValue = dgvRow.Cells("DefaultValue").Value.ToString
                End If

                limitsRow.DecimalsAllowed = dgvRow.Cells("DecimalsAllowed").Value.ToString
                limitsRow.AnalyzerModel = dgvRow.Cells("AnalyzerModel").Value.ToString

                myLimitsDS.tfmwFieldLimits.AddtfmwFieldLimitsRow(limitsRow)
            Next dgvRow

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".LoadLimits", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".LoadLimits", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try

        Return myLimitsDS

    End Function

    Private Function LoadParameters() As ParametersDS

        Dim myParametersDS As New ParametersDS

        Try
            Dim parametersRow As ParametersDS.tfmwSwParametersRow

            For Each dgvRow As DataGridViewRow In ParametersDataGridView.Rows
                parametersRow = myParametersDS.tfmwSwParameters.NewtfmwSwParametersRow

                parametersRow.ParameterID = dgvRow.Cells("ParameterID").Value.ToString
                parametersRow.ParameterName = dgvRow.Cells("ParameterName").Value.ToString
                parametersRow.DependByModel = dgvRow.Cells("DependByModel").Value.ToString
                parametersRow.AnalyzerModel = dgvRow.Cells("AnalyzerModel").Value.ToString

                If Not dgvRow.Cells("ValueNumeric").Value.ToString Is String.Empty Then
                    parametersRow.ValueNumeric = dgvRow.Cells("ValueNumeric").Value.ToString
                End If

                If Not dgvRow.Cells("ValueText").Value.ToString Is String.Empty Then
                    parametersRow.ValueText = dgvRow.Cells("ValueText").Value.ToString
                End If

                myParametersDS.tfmwSwParameters.AddtfmwSwParametersRow(parametersRow)
            Next dgvRow

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".LoadLimits", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".LoadLimits", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try

        Return myParametersDS
    End Function

    ''' <summary>
    ''' Save data of the updated Parameters
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 11/07/2011
    ''' </remarks>
    Private Sub SaveLimitschanges()
        Try
            Dim myLimitsDS As New FieldLimitsDS

            myLimitsDS = LoadLimits()

            If myLimitsDS.tfmwFieldLimits.Count > 0 Then
                Dim myGlobalDataTO As New GlobalDataTO
                Dim myFieldLimitsDelegate As New FieldLimitsDelegate()

                myGlobalDataTO = myFieldLimitsDelegate.SaveLimits(Nothing, myLimitsDS)
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".SaveLimitschanges", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".SaveLimitschanges", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")

        End Try

    End Sub



    ''' <summary>
    ''' Save data of the updated Settings
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 11/07/2011
    ''' </remarks>
    Private Sub SaveChanges()
        Try

            If SettingsListView.SelectedItems.Item(0).Text.ToString = LblLimits Then
                If (ValidateSavingConditions()) Then
                    SaveLimitschanges()

                    myFieldLimitsDS = LoadLimits()

                    For i As Integer = 0 To LimitsDataGridView.Rows.Count - 1
                        LimitsDataGridView.Rows(i).Cells("MinValue").Style.BackColor = Color.WhiteSmoke
                        LimitsDataGridView.Rows(i).Cells("MaxValue").Style.BackColor = Color.WhiteSmoke
                        LimitsDataGridView.Rows(i).Cells("StepValue").Style.BackColor = Color.WhiteSmoke
                        LimitsDataGridView.Rows(i).Cells("DefaultValue").Style.BackColor = Color.WhiteSmoke
                        LimitsDataGridView.Rows(i).Cells("DecimalsAllowed").Style.BackColor = Color.WhiteSmoke
                    Next i

                    LimitsDataGridView.Columns("MinValue").ReadOnly = True
                    LimitsDataGridView.Columns("Maxvalue").ReadOnly = True
                    LimitsDataGridView.Columns("StepValue").ReadOnly = True
                    LimitsDataGridView.Columns("DefaultValue").ReadOnly = True
                    LimitsDataGridView.Columns("DecimalsAllowed").ReadOnly = True

                    EditButton.Enabled = True
                    SaveButton.Enabled = False
                    ButtonCancel.Enabled = False
                    FilterComboBox.Enabled = True
                    EditionMode = False

                End If

            ElseIf SettingsListView.SelectedItems.Item(0).Text.ToString = LblParameters Then
                SaveParametersChanges()

                myFieldParametersDS = LoadParameters()

                For i As Integer = 0 To ParametersDataGridView.Rows.Count - 1
                    ParametersDataGridView.Rows(i).Cells("ValueNumeric").Style.BackColor = Color.WhiteSmoke
                    ParametersDataGridView.Rows(i).Cells("ValueText").Style.BackColor = Color.WhiteSmoke
                Next i

                ParametersDataGridView.Columns("ValueNumeric").ReadOnly = True
                ParametersDataGridView.Columns("ValueText").ReadOnly = True

                EditButton.Enabled = True
                SaveButton.Enabled = False
                ButtonCancel.Enabled = False
                FilterComboBox.Enabled = True
                EditionMode = False
            End If

            ' XBC 14/09/2012
            If FirmwareVersionChanged Then
                SaveFirmwareVersionChanges()
                Me.BsFwVersionTextBox.Enabled = False
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".SaveChanges", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".SaveChanges", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub


    ''' <summary>
    ''' Set the screen controls to EDIT MODE
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 11/07/2011
    ''' </remarks>
    Private Sub EditModeScreenStatus()
        Try
            ChangesMade = False
            EditionMode = True
            EditButton.Enabled = False
            SaveButton.Enabled = True
            ButtonCancel.Enabled = True
            FilterComboBox.Enabled = False
            ' XBC 14/09/2012
            Me.BsFwVersionTextBox.Enabled = True

            If SettingsListView.SelectedItems.Item(0).Text.ToString = LblLimits Then

                For i As Integer = 0 To LimitsDataGridView.Rows.Count - 1
                    LimitsDataGridView.Rows(i).Cells("MinValue").Style.BackColor = Color.White
                    LimitsDataGridView.Rows(i).Cells("MaxValue").Style.BackColor = Color.White
                    LimitsDataGridView.Rows(i).Cells("StepValue").Style.BackColor = Color.White
                    LimitsDataGridView.Rows(i).Cells("DefaultValue").Style.BackColor = Color.White
                    LimitsDataGridView.Rows(i).Cells("DecimalsAllowed").Style.BackColor = Color.White
                Next i

                LimitsDataGridView.Columns("MinValue").ReadOnly = False
                LimitsDataGridView.Columns("Maxvalue").ReadOnly = False
                LimitsDataGridView.Columns("StepValue").ReadOnly = False
                LimitsDataGridView.Columns("DefaultValue").ReadOnly = False
                LimitsDataGridView.Columns("DecimalsAllowed").ReadOnly = False

            ElseIf SettingsListView.SelectedItems.Item(0).Text.ToString = LblParameters Then

                For i As Integer = 0 To ParametersDataGridView.Rows.Count - 1
                    ParametersDataGridView.Rows(i).Cells("ValueNumeric").Style.BackColor = Color.White
                    ParametersDataGridView.Rows(i).Cells("ValueText").Style.BackColor = Color.White
                Next i

                ParametersDataGridView.Columns("ValueNumeric").ReadOnly = False
                ParametersDataGridView.Columns("ValueText").ReadOnly = False
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".EditModeScreenStatus", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".EditModeScreenStatus", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Verify if there is at least one User's change pending to save
    ''' </summary>
    ''' <returns>True if there are changes pending to save; otherwise, False</returns>
    ''' <remarks>
    ''' Created by:  DL 11/07/2011
    ''' </remarks> 
    Public Function PendingChangesVerification() As Boolean
        Try

            ChangesMade = False

            If (EditionMode) Then
                'If SettingsListView.SelectedItems.Item(0).Text.ToString = LblLimits Then
                If SettingsListView.Items(originalselectedindex).Text.ToString = LblLimits Then

                    Dim myLimitsDS As New FieldLimitsDS
                    myLimitsDS = LoadLimits()

                    Dim ds3 As New FieldLimitsDS

                    ds3.Merge(myLimitsDS)
                    ds3.AcceptChanges()
                    ds3.Merge(myFieldLimitsDS)

                    Dim dsDifferences As New FieldLimitsDS
                    dsDifferences = ds3.GetChanges()

                    If Not dsDifferences Is Nothing AndAlso dsDifferences.tfmwFieldLimits.Count > 0 Then ChangesMade = True


                ElseIf SettingsListView.Items(originalselectedindex).Text.ToString = LblParameters Then 'SettingsListView.SelectedItems.Item(0).Text.ToString = LblParameters Then
                    Dim myParameterDS As New ParametersDS
                    myParameterDS = LoadParameters()

                    Dim ds3 As New ParametersDS

                    ds3.Merge(myParameterDS)
                    ds3.AcceptChanges()
                    ds3.Merge(myFieldParametersDS)

                    Dim dsDifferences As New ParametersDS
                    dsDifferences = ds3.GetChanges()

                    If Not dsDifferences Is Nothing AndAlso dsDifferences.tfmwSwParameters.Count > 0 Then ChangesMade = True


                End If
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".PendingChangesVerification", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PendingChangesVerification", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
        Return ChangesMade
    End Function


    ''' <summary>
    ''' Execute the cancelling 
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 11/07/2011
    ''' </remarks>
    Private Sub CancelEdition()
        Dim setScreenToInitial As Boolean = False

        Try
            If (PendingChangesVerification()) Then
                If (ShowMessage(Me.Name, GlobalEnumerates.Messages.DISCARD_PENDING_CHANGES.ToString) = Windows.Forms.DialogResult.Yes) Then
                    EditButton.Enabled = True
                    SaveButton.Enabled = False
                    ButtonCancel.Enabled = False
                    FilterComboBox.Enabled = True

                    If SettingsListView.SelectedItems.Item(0).Text.ToString = LblLimits Then
                        LimitsDataGridView.Visible = True
                        ParametersDataGridView.Visible = False

                        Dim resultData As New GlobalDataTO
                        Dim myFieldLimits As New FieldLimitsDelegate

                        If FilterComboBox.Text <> AllAnalyzerModels Then
                            resultData = myFieldLimits.GetAllList(Nothing, FilterComboBox.Text)
                        Else
                            resultData = myFieldLimits.GetAllList(Nothing)
                        End If

                        If (Not resultData.HasError) And (Not resultData.SetDatos Is Nothing) Then

                            myFieldLimitsDS = CType(resultData.SetDatos, FieldLimitsDS)

                            'Sort the Tests/SampleTypes by TestPosition
                            Dim viewTests As DataView = New DataView
                            viewTests = myFieldLimitsDS.tfmwFieldLimits.DefaultView
                            viewTests.Sort = "LimitID ASC "

                            'Set the sorted DS as DataSource of Tests/SampleTypes grid
                            Dim bsTestsBindingSource As BindingSource = New BindingSource
                            bsTestsBindingSource.DataSource = viewTests
                            LimitsDataGridView.DataSource = bsTestsBindingSource
                        Else
                            LimitsDataGridView.DataSource = Nothing
                        End If

                    ElseIf SettingsListView.SelectedItems.Item(0).Text.ToString = LblParameters Then

                        LimitsDataGridView.Visible = False
                        ParametersDataGridView.Visible = True

                        Dim resultData As New GlobalDataTO
                        Dim myfieldParameters As New SwParametersDelegate

                        If FilterComboBox.Text <> AllAnalyzerModels Then
                            resultData = myfieldParameters.GetAllList(Nothing, FilterComboBox.Text)
                        Else
                            resultData = myfieldParameters.GetAllList(Nothing)
                        End If

                        If (Not resultData.HasError) And (Not resultData.SetDatos Is Nothing) Then

                            myFieldParametersDS = CType(resultData.SetDatos, ParametersDS)

                            'Sort the Tests/SampleTypes by TestPosition
                            Dim viewTests As DataView = New DataView
                            viewTests = myFieldParametersDS.tfmwSwParameters.DefaultView
                            viewTests.Sort = "ParameterID ASC "

                            'Set the sorted DS as DataSource of Tests/SampleTypes grid
                            Dim bsTestsBindingSource As BindingSource = New BindingSource
                            bsTestsBindingSource.DataSource = viewTests
                            ParametersDataGridView.DataSource = bsTestsBindingSource
                        Else
                            ParametersDataGridView.DataSource = Nothing
                        End If
                    End If
                Else
                    If (originalselectedindex <> -1) Then
                        'Return focus to the control that has been edited
                        SettingsListView.Items(originalselectedindex).Selected = True
                        SettingsListView.Select()

                    End If
                End If
            Else
                '                QueryByElement()
                ChangesMade = False
                EditionMode = False
                EditButton.Enabled = True
                SaveButton.Enabled = False
                ButtonCancel.Enabled = False
                FilterComboBox.Enabled = True

                If SettingsListView.SelectedItems.Item(0).Text.ToString = LblLimits Then
                    For i As Integer = 0 To LimitsDataGridView.Rows.Count - 1
                        LimitsDataGridView.Rows(i).Cells("MinValue").Style.BackColor = Color.WhiteSmoke
                        LimitsDataGridView.Rows(i).Cells("MaxValue").Style.BackColor = Color.WhiteSmoke
                        LimitsDataGridView.Rows(i).Cells("StepValue").Style.BackColor = Color.WhiteSmoke
                        LimitsDataGridView.Rows(i).Cells("DefaultValue").Style.BackColor = Color.WhiteSmoke
                        LimitsDataGridView.Rows(i).Cells("DecimalsAllowed").Style.BackColor = Color.WhiteSmoke
                    Next i

                    LimitsDataGridView.Columns("MinValue").ReadOnly = True
                    LimitsDataGridView.Columns("Maxvalue").ReadOnly = True
                    LimitsDataGridView.Columns("StepValue").ReadOnly = True
                    LimitsDataGridView.Columns("DefaultValue").ReadOnly = True
                    LimitsDataGridView.Columns("DecimalsAllowed").ReadOnly = True

                ElseIf SettingsListView.SelectedItems.Item(0).Text.ToString = LblParameters Then

                    For i As Integer = 0 To ParametersDataGridView.Rows.Count - 1
                        ParametersDataGridView.Rows(i).Cells("ValueNumeric").Style.BackColor = Color.WhiteSmoke
                        ParametersDataGridView.Rows(i).Cells("ValueText").Style.BackColor = Color.WhiteSmoke
                    Next i

                    ParametersDataGridView.Columns("ValueNumeric").ReadOnly = True
                    ParametersDataGridView.Columns("ValueText").ReadOnly = True
                End If


            End If



        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".CancelControlEdition", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".CancelControlEdition", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' For the informed grid row, verify all mandatory values have been informed with an allowed 
    ''' value. In case of wrong value, show the Icon Error with the correspondent message in the 
    ''' right side of the cell
    ''' </summary>
    ''' <param name="pCurrentRow">Current row index</param>
    ''' <returns>Boolean value: True if at least one of the cells in the row is wrong; 
    '''          otherwise, it returns False</returns>
    ''' <remarks>
    ''' Created by:  DL 12/07/2011
    ''' </remarks>
    Private Function ValidateTestGridRow(ByVal pCurrentRow As Integer) As Boolean
        Dim rowWithErrors As Boolean = False
        Try
            Dim errorFound As Boolean = False
            'Clear previous errors...
            LimitsDataGridView.Rows(pCurrentRow).Cells("MinValue").ErrorText = String.Empty
            LimitsDataGridView.Rows(pCurrentRow).Cells("MinValue").Style.Alignment = DataGridViewContentAlignment.MiddleRight

            LimitsDataGridView.Rows(pCurrentRow).Cells("MaxValue").ErrorText = String.Empty
            LimitsDataGridView.Rows(pCurrentRow).Cells("MaxValue").Style.Alignment = DataGridViewContentAlignment.MiddleRight

            'Min/Max values have to be informed and Min < Max
            If (LimitsDataGridView.Rows(pCurrentRow).Cells("MinValue").Value Is Nothing) OrElse _
               (LimitsDataGridView.Rows(pCurrentRow).Cells("MinValue").Value Is DBNull.Value) OrElse _
               (LimitsDataGridView.Rows(pCurrentRow).Cells("MinValue").Value.ToString = String.Empty) Then
                errorFound = True
                rowWithErrors = True
                LimitsDataGridView.Rows(pCurrentRow).Cells("MinValue").Style.Alignment = DataGridViewContentAlignment.MiddleLeft
                LimitsDataGridView.Rows(pCurrentRow).Cells("MinValue").ErrorText = GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString)
            End If
            If (LimitsDataGridView.Rows(pCurrentRow).Cells("MaxValue").Value Is Nothing) OrElse _
               (LimitsDataGridView.Rows(pCurrentRow).Cells("MaxValue").Value Is DBNull.Value) OrElse _
               (LimitsDataGridView.Rows(pCurrentRow).Cells("MaxValue").Value.ToString = String.Empty) Then
                errorFound = True
                rowWithErrors = True
                LimitsDataGridView.Rows(pCurrentRow).Cells("MaxValue").Style.Alignment = DataGridViewContentAlignment.MiddleLeft
                LimitsDataGridView.Rows(pCurrentRow).Cells("MaxValue").ErrorText = GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString)
            End If

            If (Not errorFound) Then
                'AG 05/08/2011 - change validation condition >= for >
                'If (CSng(LimitsDataGridView.Rows(pCurrentRow).Cells("MinValue").Value.ToString) >= _
                '    CSng(LimitsDataGridView.Rows(pCurrentRow).Cells("MaxValue").Value.ToString)) Then
                If (CSng(LimitsDataGridView.Rows(pCurrentRow).Cells("MinValue").Value.ToString) > _
                    CSng(LimitsDataGridView.Rows(pCurrentRow).Cells("MaxValue").Value.ToString)) Then
                    errorFound = True
                    rowWithErrors = True
                    LimitsDataGridView.Rows(pCurrentRow).Cells("MinValue").Style.Alignment = DataGridViewContentAlignment.MiddleLeft
                    LimitsDataGridView.Rows(pCurrentRow).Cells("MinValue").ErrorText = GetMessageText(GlobalEnumerates.Messages.MIN_MUST_BE_LOWER_THAN_MAX.ToString)
                End If
            End If

            If (Not rowWithErrors) Then
                'If there are not errors in the grid row, clean all Error symbols
                LimitsDataGridView.Rows(pCurrentRow).Cells("MinValue").ErrorText = String.Empty
                LimitsDataGridView.Rows(pCurrentRow).Cells("MinValue").Style.Alignment = DataGridViewContentAlignment.MiddleRight

                LimitsDataGridView.Rows(pCurrentRow).Cells("MaxValue").ErrorText = String.Empty
                LimitsDataGridView.Rows(pCurrentRow).Cells("MaxValue").Style.Alignment = DataGridViewContentAlignment.MiddleRight

            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ValidateTestGridRow", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ValidateTestGridRow", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
        Return rowWithErrors
    End Function

    ''' <summary>
    ''' Validate if there are errors in values informed in the grid Limit
    ''' </summary>
    ''' <returns>True if there are errors; otherwise, it returns False</returns>
    ''' <remarks>
    ''' Created by:  DL 11/07/2011
    ''' </remarks>
    Private Function ValidateErrorOnTestsValues() As Boolean
        Dim errorFound As Boolean = False
        Try
            For i As Integer = 0 To LimitsDataGridView.Rows.Count - 1
                errorFound = ValidateTestGridRow(i)
                If errorFound Then Exit For
            Next
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ValidateErrorOnTestsValues ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ValidateErrorOnTestsValues ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
        Return errorFound
    End Function



    ''' <summary>
    ''' Validate that all mandatory fields have been informed with a correct value
    ''' </summary>
    ''' <returns>True if all fields have a correct value; otherwise, False</returns>
    ''' <remarks>
    ''' Created by:  DL
    ''' Modified by: DL 11/07/2011
    ''' </remarks>
    Private Function ValidateSavingConditions() As Boolean
        Dim fieldsOK As Boolean = True
        Try
            Dim setFocusTo As Integer = -1

            bsErrorProvider1.Clear()

            If (ValidateErrorOnTestsValues()) Then
                fieldsOK = False
                LimitsDataGridView.Focus()
            End If


        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ValidateSavingConditions", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ValidateSavingConditions", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
        Return fieldsOK
    End Function
#End Region


    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub



    Private Sub PrintButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PrintButton.Click

    End Sub

    ' XBC 14/09/2012
    Private Sub BsFwVersionTextBox_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles BsFwVersionTextBox.TextChanged
        FirmwareVersionChanged = True
    End Sub
End Class