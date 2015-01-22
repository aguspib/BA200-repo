Option Explicit On
Option Strict On
Option Infer On

Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.BL

Public Class IResultsOffSystemsTest

#Region "Declarations"
    'Global variable needed to control edition of ResultValue in the grid
    Private myResultValueTextBox As TextBox

    'Global variable to control if there are changes pending to save
    Private ChangesMade As Boolean = False

    'Global variable used to avoid the screen movement
    Private myNewLocation As Point
#End Region

#Region "Attributes"
    Private OffSystemTestsListAttribute As New OffSystemTestsResultsDS
#End Region

#Region "Properties"
    'To receive and return the list of Off System Tests with their informed result
    Public Property OffSystemTestsList() As OffSystemTestsResultsDS
        Get
            Return OffSystemTestsListAttribute
        End Get
        Set(ByVal value As OffSystemTestsResultsDS)
            OffSystemTestsListAttribute = value
        End Set
    End Property
#End Region

#Region "Methods"
    ''' <summary>
    ''' Search Icons for screen buttons
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 18/01/2011 
    ''' </remarks>
    Private Sub PrepareButtons()
        Try
            Dim auxIconName As String = ""
            Dim iconPath As String = IconsPath

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
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".PrepareButtons", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".PrepareButtons", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
            Close()
        End Try
    End Sub

    ''' <summary>
    ''' Get texts in the current application language for all screen controls
    ''' </summary>
    ''' <param name="pLanguageID"> The current Language of Application </param>
    ''' <remarks>
    ''' Created by:  SA 18/01/2011
    ''' </remarks>
    Private Sub GetScreenLabels(ByVal pLanguageID As String)
        Try
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            'For Labels, CheckBox, RadioButtons...
            bsOffSystemTestsResultsLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_OffSystem_Tests_Results", pLanguageID)

            'For Tooltips...
            bsScreenToolTips.SetToolTip(bsAcceptButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Save&Close", pLanguageID))
            bsScreenToolTips.SetToolTip(bsCancelButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Cancel&Close", pLanguageID))
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".GetScreenLabels", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".GetScreenLabels", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Initialization of the OffSystem Tests Results grid
    ''' </summary>
    ''' ''' <remarks>
    ''' Created by:  SA 18/01/2011
    ''' Modified by: JC 13/11/2012 - Modified column widths
    '''              SA 10/12/2012 - Undo change in property MaxInputLength of textBoxColumnResultValue: it was changed from 15 to 130, 
    '''                              but it is not correct, it is the column Width property which has to be changed from 180 to 130. MaxInputLength
    '''                              has to be 15, the maximum length for the field in DB, otherwise a DB error is raised
    ''' </remarks>
    Private Sub InitializeGrid(ByVal pLanguageID As String)
        Try
            Dim columnName As String
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            bsOffSystemTestsResultsDataGridView.AutoSize = False
            bsOffSystemTestsResultsDataGridView.AllowUserToAddRows = False
            bsOffSystemTestsResultsDataGridView.AllowUserToDeleteRows = False
            bsOffSystemTestsResultsDataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect
            bsOffSystemTestsResultsDataGridView.EditMode = DataGridViewEditMode.EditOnKeystrokeOrF2

            bsOffSystemTestsResultsDataGridView.Rows.Clear()
            bsOffSystemTestsResultsDataGridView.Columns.Clear()

            'OrderTestID column
            columnName = "OrderTestID"
            bsOffSystemTestsResultsDataGridView.Columns.Add(columnName, "")
            bsOffSystemTestsResultsDataGridView.Columns(columnName).Width = 0
            bsOffSystemTestsResultsDataGridView.Columns(columnName).Visible = False
            bsOffSystemTestsResultsDataGridView.Columns(columnName).DataPropertyName = columnName
            bsOffSystemTestsResultsDataGridView.Columns(columnName).ReadOnly = True

            'PatientID/SampleID column
            columnName = "SampleID"
            bsOffSystemTestsResultsDataGridView.Columns.Add(columnName, myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_PatientSample", pLanguageID))
            bsOffSystemTestsResultsDataGridView.Columns(columnName).Width = 180
            bsOffSystemTestsResultsDataGridView.Columns(columnName).DataPropertyName = columnName
            bsOffSystemTestsResultsDataGridView.Columns(columnName).ReadOnly = True
            bsOffSystemTestsResultsDataGridView.Columns(columnName).SortMode = DataGridViewColumnSortMode.Programmatic

            'StatFlag column
            Dim checkBoxColumnStat As New DataGridViewCheckBoxColumn
            columnName = "StatFlag"
            checkBoxColumnStat.Name = columnName
            checkBoxColumnStat.HeaderText = "StatFlag"

            bsOffSystemTestsResultsDataGridView.Columns.Add(checkBoxColumnStat)
            bsOffSystemTestsResultsDataGridView.Columns(columnName).Width = 0
            bsOffSystemTestsResultsDataGridView.Columns(columnName).Visible = False
            bsOffSystemTestsResultsDataGridView.Columns(columnName).DataPropertyName = columnName
            bsOffSystemTestsResultsDataGridView.Columns(columnName).ReadOnly = True

            'SampleClassIcon column - StatFlag Icon
            'JC 13/11/2012
            Dim iconSampleClassColumn As New DataGridViewImageColumn
            columnName = "StatFlagIcon"
            iconSampleClassColumn.Name = columnName
            iconSampleClassColumn.HeaderText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Stat", pLanguageID)

            bsOffSystemTestsResultsDataGridView.Columns.Add(iconSampleClassColumn)
            bsOffSystemTestsResultsDataGridView.Columns(columnName).Width = 50
            bsOffSystemTestsResultsDataGridView.Columns(columnName).DataPropertyName = columnName
            bsOffSystemTestsResultsDataGridView.Columns(columnName).ReadOnly = True
            bsOffSystemTestsResultsDataGridView.Columns(columnName).SortMode = DataGridViewColumnSortMode.Programmatic
            bsOffSystemTestsResultsDataGridView.Columns(columnName).HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter

            'TestID column
            columnName = "TestID"
            bsOffSystemTestsResultsDataGridView.Columns.Add(columnName, "")
            bsOffSystemTestsResultsDataGridView.Columns(columnName).Width = 0
            bsOffSystemTestsResultsDataGridView.Columns(columnName).Visible = False
            bsOffSystemTestsResultsDataGridView.Columns(columnName).DataPropertyName = columnName
            bsOffSystemTestsResultsDataGridView.Columns(columnName).ReadOnly = True

            'TestName column
            columnName = "TestName"
            bsOffSystemTestsResultsDataGridView.Columns.Add(columnName, myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Test", pLanguageID))
            bsOffSystemTestsResultsDataGridView.Columns(columnName).Width = 180
            bsOffSystemTestsResultsDataGridView.Columns(columnName).DataPropertyName = columnName
            bsOffSystemTestsResultsDataGridView.Columns(columnName).ReadOnly = True
            bsOffSystemTestsResultsDataGridView.Columns(columnName).SortMode = DataGridViewColumnSortMode.Programmatic

            'SampleType column
            columnName = "SampleType"
            bsOffSystemTestsResultsDataGridView.Columns.Add(columnName, myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Type", pLanguageID))
            bsOffSystemTestsResultsDataGridView.Columns(columnName).Width = 80
            bsOffSystemTestsResultsDataGridView.Columns(columnName).DataPropertyName = columnName
            bsOffSystemTestsResultsDataGridView.Columns(columnName).ReadOnly = True
            bsOffSystemTestsResultsDataGridView.Columns(columnName).SortMode = DataGridViewColumnSortMode.Programmatic

            'ResultType column
            columnName = "ResultType"
            bsOffSystemTestsResultsDataGridView.Columns.Add(columnName, "")
            bsOffSystemTestsResultsDataGridView.Columns(columnName).Width = 0
            bsOffSystemTestsResultsDataGridView.Columns(columnName).Visible = False
            bsOffSystemTestsResultsDataGridView.Columns(columnName).DataPropertyName = columnName
            bsOffSystemTestsResultsDataGridView.Columns(columnName).ReadOnly = True

            'Qualitative Result Flag column
            'JC 13/11/2012
            Dim checkBoxColumnQuantitative As New DataGridViewCheckBoxColumn
            columnName = "QuantitativeFlag"
            checkBoxColumnQuantitative.Name = columnName
            checkBoxColumnQuantitative.HeaderText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "PMD_RESULT_TYPE_QUANTIVE", pLanguageID)

            bsOffSystemTestsResultsDataGridView.Columns.Add(checkBoxColumnQuantitative)
            bsOffSystemTestsResultsDataGridView.Columns(columnName).Width = 100
            bsOffSystemTestsResultsDataGridView.Columns(columnName).Visible = True
            bsOffSystemTestsResultsDataGridView.Columns(columnName).DataPropertyName = columnName
            bsOffSystemTestsResultsDataGridView.Columns(columnName).ReadOnly = True
            bsOffSystemTestsResultsDataGridView.Columns(columnName).HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter

            'ResultValue column
            'JC 13/11/2012
            Dim textBoxColumnResultValue As New DataGridViewTextBoxColumn
            columnName = "ResultValue"
            textBoxColumnResultValue.Name = columnName
            textBoxColumnResultValue.HeaderText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_ResultValue", pLanguageID)
            textBoxColumnResultValue.MaxInputLength = 15
            textBoxColumnResultValue.ReadOnly = False

            bsOffSystemTestsResultsDataGridView.Columns.Add(textBoxColumnResultValue)
            bsOffSystemTestsResultsDataGridView.Columns(columnName).Width = 130
            bsOffSystemTestsResultsDataGridView.Columns(columnName).DataPropertyName = columnName
            bsOffSystemTestsResultsDataGridView.Columns(columnName).ReadOnly = False
            bsOffSystemTestsResultsDataGridView.Columns(columnName).SortMode = DataGridViewColumnSortMode.Programmatic

            'MeasureUnit column
            'JC 13/11/2012
            columnName = "Unit"
            bsOffSystemTestsResultsDataGridView.Columns.Add(columnName, myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Unit", pLanguageID))
            bsOffSystemTestsResultsDataGridView.Columns(columnName).Width = 50
            bsOffSystemTestsResultsDataGridView.Columns(columnName).DataPropertyName = columnName
            bsOffSystemTestsResultsDataGridView.Columns(columnName).ReadOnly = True
            bsOffSystemTestsResultsDataGridView.Columns(columnName).SortMode = DataGridViewColumnSortMode.Programmatic

            'ResultDateTime column
            columnName = "ResultDateTime"
            bsOffSystemTestsResultsDataGridView.Columns.Add(columnName, "")
            bsOffSystemTestsResultsDataGridView.Columns(columnName).Width = 0
            bsOffSystemTestsResultsDataGridView.Columns(columnName).Visible = False
            bsOffSystemTestsResultsDataGridView.Columns(columnName).DataPropertyName = columnName
            bsOffSystemTestsResultsDataGridView.Columns(columnName).ReadOnly = True

            'AllowedDecimals column
            columnName = "AllowedDecimals"
            bsOffSystemTestsResultsDataGridView.Columns.Add(columnName, "")
            bsOffSystemTestsResultsDataGridView.Columns(columnName).Width = 0
            bsOffSystemTestsResultsDataGridView.Columns(columnName).Visible = False
            bsOffSystemTestsResultsDataGridView.Columns(columnName).DataPropertyName = columnName
            bsOffSystemTestsResultsDataGridView.Columns(columnName).ReadOnly = True

            'ActiveRangeType column
            columnName = "ActiveRangeType"
            bsOffSystemTestsResultsDataGridView.Columns.Add(columnName, "")
            bsOffSystemTestsResultsDataGridView.Columns(columnName).Width = 0
            bsOffSystemTestsResultsDataGridView.Columns(columnName).Visible = False
            bsOffSystemTestsResultsDataGridView.Columns(columnName).DataPropertyName = columnName
            bsOffSystemTestsResultsDataGridView.Columns(columnName).ReadOnly = True

            bsOffSystemTestsResultsDataGridView.ScrollBars = ScrollBars.Vertical
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".InitializeGrid ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".InitializeGrid", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    '''  Screen initialization
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 18/01/2011
    ''' </remarks>
    Private Sub ScreenLoad()
        Try
            'Get the current Language from the current Application Session
            'Dim currentLanguageGlobal As New GlobalBase
            Dim currentLanguage As String = GlobalBase.GetSessionInfo().ApplicationLanguage.Trim.ToString

            'Load images for graphical buttons
            PrepareButtons()

            'Get multilanguage labels for all screen controls
            GetScreenLabels(currentLanguage)

            'Initialize the grid
            InitializeGrid(currentLanguage)

            'Load the list of Errors
            Dim viewOffSystemTests As DataView = New DataView
            viewOffSystemTests = OffSystemTestsListAttribute.OffSystemTestsResults.DefaultView
            viewOffSystemTests.Sort = "StatFlag DESC, SampleID ASC, SampleType ASC "

            Dim bsOffSystemTestsBindingSource As BindingSource = New BindingSource() With {.DataSource = viewOffSystemTests}
            bsOffSystemTestsResultsDataGridView.DataSource = bsOffSystemTestsBindingSource
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".ScreenLoad ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ScreenLoad", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Validate if the value entered is numeric or one of the characters allowed as decimal separators. 
    ''' String values are not allowed.
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 18/01/2011
    ''' Modified by: SA 08/11/2012 - Implementation copied from the one in Controls Programmig Screen to:
    '''                              ** Avoid more than one decimal separator in a cell
    '''                              ** If the pressed key is BackSpace, allow it (e.Handle=false) and stop
    ''' </remarks>
    Private Sub CheckNumericCell(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyPressEventArgs)
        Try
            If (e.KeyChar = CChar("") OrElse e.KeyChar = ChrW(Keys.Back)) Then
                e.Handled = False
            Else
                Dim myDecimalSeparator As String = SystemInfoManager.OSDecimalSeparator
                If (e.KeyChar = CChar(".") OrElse e.KeyChar = CChar(",")) Then
                    If (e.KeyChar = CChar(".") OrElse e.KeyChar = CChar(",")) Then
                        e.KeyChar = CChar(myDecimalSeparator)
                    End If

                    If (CType(sender, TextBox).Text.Contains(".") Or CType(sender, TextBox).Text.Contains(",")) Then
                        e.Handled = True
                    Else
                        e.Handled = False
                    End If
                Else
                    If (Not IsNumeric(e.KeyChar)) Then
                        e.Handled = True
                    End If
                End If
            End If

            'If (e.KeyChar = CChar(".") OrElse e.KeyChar = CChar(",") OrElse e.KeyChar = CChar("'")) Then
            '    e.KeyChar = CChar(SystemInfoManager.OSDecimalSeparator)
            'End If

            'If (Not ValidateSpecialCharacters(e.KeyChar, String.Format("[\d\{0}\]", SystemInfoManager.OSDecimalSeparator))) Then e.Handled = True
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".CheckNumericCell ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".CheckNumericCell ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' If there are changes pending to save, shown the warning message before closing the screen
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 19/01/2011
    ''' </remarks>
    Private Sub CancelAndClose()
        Try
            Dim screenClose As Boolean = True
            If (ChangesMade) Then
                If (ShowMessage(Name, GlobalEnumerates.Messages.DISCARD_PENDING_CHANGES.ToString) = Windows.Forms.DialogResult.No) Then
                    screenClose = False
                End If
            End If

            If (screenClose) Then Close()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".CancelAndClose ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".CancelAndClose", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub
#End Region

#Region "Events"
    '*****************'
    '* SCREEN EVENTS *'
    '*****************'
    Private Sub IResultsOffSystemTest_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            Dim mySize As Size = UiAx00MainMDI.Size
            Dim myLocation As Point = UiAx00MainMDI.Location

            If (Not Me.MdiParent Is Nothing) Then
                mySize = Me.Parent.Size
                myLocation = Me.Parent.Location
            End If

            myNewLocation = New Point(myLocation.X + CInt((mySize.Width - Me.Width) / 2), myLocation.Y + CInt((mySize.Height - Me.Height) / 2))
            Me.Location = myNewLocation

            ScreenLoad()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".IResultsOffSystemTest_Load ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".IResultsOffSystemTest_Load", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub IResultsOffSystemTest_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyDown
        Try
            If (e.KeyCode = Keys.Escape) Then
                'RH 04/07/2011 Escape key should do exactly the same operations as bsCancelButton_Click()
                bsCancelButton.PerformClick()
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".IResultsOffSystemTest_KeyDown ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".IResultsOffSystemTest_KeyDown", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Not allow move form and mantain the center location in center parent
    ''' </summary>
    Protected Overrides Sub WndProc(ByRef m As Message)
        Try
            If (m.Msg = WM_WINDOWPOSCHANGING) Then
                Dim pos As WINDOWPOS = DirectCast(Runtime.InteropServices.Marshal.PtrToStructure(m.LParam, GetType(WINDOWPOS)), WINDOWPOS)

                Dim mySize As Size = UiAx00MainMDI.Size
                Dim myLocation As Point = UiAx00MainMDI.Location
                If (Not Me.MdiParent Is Nothing) Then
                    mySize = Me.Parent.Size
                    myLocation = Me.Parent.Location
                End If

                pos.x = myLocation.X + CInt((mySize.Width - Me.Width) / 2)
                pos.y = myLocation.Y + CInt((mySize.Height - Me.Height) / 2)
                Runtime.InteropServices.Marshal.StructureToPtr(pos, m.LParam, True)
            End If
            MyBase.WndProc(m)
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", ".WndProc " & Me.Name, EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".WndProc", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    '***************'
    '* GRID EVENTS *'
    '***************'
    Private Sub bsOffSystemTestsResultsDataGridView_CellFormatting(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellFormattingEventArgs) Handles bsOffSystemTestsResultsDataGridView.CellFormatting
        Try
            If (bsOffSystemTestsResultsDataGridView.Columns(e.ColumnIndex).Name = "ResultValue") AndAlso _
               (Not String.IsNullOrEmpty(bsOffSystemTestsResultsDataGridView.Rows(e.RowIndex).Cells("ResultValue").Value.ToString)) Then
                If (Convert.ToBoolean(bsOffSystemTestsResultsDataGridView.Rows(e.RowIndex).Cells("QuantitativeFlag").Value)) Then
                    Dim allowedDecimals As Integer = Convert.ToInt32(bsOffSystemTestsResultsDataGridView.Rows(e.RowIndex).Cells("AllowedDecimals").Value)
                    e.Value = CType(e.Value, Double).ToString("F" & allowedDecimals.ToString)

                    bsOffSystemTestsResultsDataGridView.Rows(e.RowIndex).Cells("ResultValue").Style.Alignment = DataGridViewContentAlignment.MiddleRight
                Else
                    bsOffSystemTestsResultsDataGridView.Rows(e.RowIndex).Cells("ResultValue").Style.Alignment = DataGridViewContentAlignment.MiddleLeft
                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".bsOffSystemTestsResultsDataGridView_CellFormatting ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsOffSystemTestsResultsDataGridView_CellFormatting", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub bsOffSystemTestsResultsDataGridView_EditingControlShowing(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewEditingControlShowingEventArgs) Handles bsOffSystemTestsResultsDataGridView.EditingControlShowing
        Try
            Dim currentCol As Integer = bsOffSystemTestsResultsDataGridView.CurrentCell.ColumnIndex
            Dim currentRow As Integer = bsOffSystemTestsResultsDataGridView.CurrentCell.RowIndex

            If (bsOffSystemTestsResultsDataGridView.Columns(currentCol).Name = "ResultValue") AndAlso _
               (bsOffSystemTestsResultsDataGridView.IsCurrentCellInEditMode) Then
                If (Not myResultValueTextBox Is Nothing) Then
                    RemoveHandler myResultValueTextBox.KeyPress, AddressOf CheckNumericCell
                    myResultValueTextBox = Nothing
                End If

                If (Convert.ToBoolean(bsOffSystemTestsResultsDataGridView.Rows(currentRow).Cells("QuantitativeFlag").Value)) Then
                    myResultValueTextBox = CType(e.Control, TextBox)
                    myResultValueTextBox.ShortcutsEnabled = False

                    AddHandler myResultValueTextBox.KeyPress, AddressOf CheckNumericCell
                    ChangesMade = True
                Else
                    myResultValueTextBox = CType(e.Control, TextBox)
                    myResultValueTextBox.ShortcutsEnabled = False

                    ChangesMade = True
                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".bsOffSystemTestsResultsDataGridView_EditingControlShowing ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsOffSystemTestsResultsDataGridView_EditingControlShowing", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub bsOffSystemTestsResultsDataGridView_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles bsOffSystemTestsResultsDataGridView.KeyDown
        Try
            If (e.KeyCode = Keys.Delete) Then
                bsOffSystemTestsResultsDataGridView.Item("ResultValue", bsOffSystemTestsResultsDataGridView.CurrentCell.RowIndex).Value = DBNull.Value
                ChangesMade = True
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".bsOffSystemTestsResultsDataGridView_KeyDown ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsOffSystemTestsResultsDataGridView_KeyDown", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    '*****************'
    '* BUTTON EVENTS *'
    '*****************'
    Private Sub bsAcceptButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsAcceptButton.Click
        Try
            Close()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".bsAcceptButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsAcceptButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub bsCancelButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsCancelButton.Click
        Try
            CancelAndClose()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".bsCancelButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsCancelButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub
#End Region



End Class
