Option Strict On
Option Explicit On
Option Infer On

Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.BL

Public Class UiWSImportLIMSErrors
    Inherits Biosystems.Ax00.PresentationCOM.BSBaseForm

#Region "Declarations"
    'Global variable used to avoid the screen movement
    Private myNewLocation As Point
#End Region

#Region "Attributes"
    Private ListOfImportErrorsAttribute As New ImportErrorsLogDS
#End Region

#Region "Properties"
    Public WriteOnly Property ListOfImportErrors() As ImportErrorsLogDS
        Set(ByVal value As ImportErrorsLogDS)
            ListOfImportErrorsAttribute = value
        End Set
    End Property
#End Region

#Region "Methods"

    ''' <summary>
    '''  Screen initialization
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 15/09/2010
    ''' </remarks>
    Private Sub ScreenLoad()
        Try
            'Get the current Language from the current Application Session
            'Dim currentLanguageGlobal As New GlobalBase
            Dim currentLanguage As String = GlobalBase.GetSessionInfo().ApplicationLanguage.Trim

            'Load images for graphical buttons
            PrepareButtons()

            'Get multilanguage labels for all screen controls
            GetScreenLabels(currentLanguage) 'PG 13/10/2010

            'Initialize the grid
            InitializeErrorsGrid(currentLanguage) 'PG 13/10/2010

            'Get the Import Date Time from the first row in the DS
            bsImportDateTimeTextBox.Text = ListOfImportErrorsAttribute.twksImportErrorsLog(0).ImportDate.ToString(SystemInfoManager.OSDateFormat & " " & _
                                                                                                                  SystemInfoManager.OSLongTimeFormat)
            bsImportDateTimeTextBox.BackColor = Color.LightGray

            'Load the list of Errors
            Dim viewErrors As DataView = New DataView
            viewErrors = ListOfImportErrorsAttribute.twksImportErrorsLog.DefaultView
            viewErrors.Sort = "LineNumber ASC "

            Dim bsImportErrorsBindingSource As BindingSource = New BindingSource
            bsImportErrorsBindingSource.DataSource = viewErrors
            bsLIMSErrorsDataGridView.DataSource = bsImportErrorsBindingSource

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ScreenLoad ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ScreenLoad", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Initialization of the Errors grid
    ''' </summary>
    ''' ''' <remarks>
    ''' Created by:  SA 15/09/2010
    ''' Modified by: PG 13/10/2010 - Add the LanguageID parameter and get Multilanguage text for the header of all 
    '''                              visible columns
    ''' </remarks>
    Private Sub InitializeErrorsGrid(ByVal pLanguageID As String)
        Try

            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            Dim columnName As String

            bsLIMSErrorsDataGridView.AllowUserToAddRows = False
            bsLIMSErrorsDataGridView.AllowUserToDeleteRows = False
            bsLIMSErrorsDataGridView.EditMode = DataGridViewEditMode.EditProgrammatically

            bsLIMSErrorsDataGridView.Rows.Clear()
            bsLIMSErrorsDataGridView.Columns.Clear()

            'Line Number column
            columnName = "LineNumber"
            bsLIMSErrorsDataGridView.Columns.Add(columnName, myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_LIMSErrors_LineNum", pLanguageID))
            bsLIMSErrorsDataGridView.Columns(columnName).Width = 50
            bsLIMSErrorsDataGridView.Columns(columnName).DataPropertyName = columnName
            bsLIMSErrorsDataGridView.Columns(columnName).ReadOnly = True
            bsLIMSErrorsDataGridView.Columns(columnName).SortMode = DataGridViewColumnSortMode.Programmatic
            bsLIMSErrorsDataGridView.Columns(columnName).HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight
            bsLIMSErrorsDataGridView.Columns(columnName).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight

            'Line Text column
            columnName = "LineText"
            bsLIMSErrorsDataGridView.Columns.Add(columnName, myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_LIMSErrors_LineText", pLanguageID))
            bsLIMSErrorsDataGridView.Columns(columnName).Width = 350
            bsLIMSErrorsDataGridView.Columns(columnName).DataPropertyName = columnName
            bsLIMSErrorsDataGridView.Columns(columnName).ReadOnly = True
            bsLIMSErrorsDataGridView.Columns(columnName).SortMode = DataGridViewColumnSortMode.Programmatic

            'Error description column
            columnName = "ErrorMessage"
            bsLIMSErrorsDataGridView.Columns.Add(columnName, myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_ERROR", pLanguageID)) 'JB 01/10/2012 - Resource String unification
            bsLIMSErrorsDataGridView.Columns(columnName).Width = 400
            bsLIMSErrorsDataGridView.Columns(columnName).DataPropertyName = columnName
            bsLIMSErrorsDataGridView.Columns(columnName).ReadOnly = True
            bsLIMSErrorsDataGridView.Columns(columnName).SortMode = DataGridViewColumnSortMode.Programmatic

            'Create the hide columns...
            columnName = "ImportID"
            bsLIMSErrorsDataGridView.Columns.Add(columnName, "ImportID")
            bsLIMSErrorsDataGridView.Columns(columnName).Width = 0
            bsLIMSErrorsDataGridView.Columns(columnName).DataPropertyName = columnName
            bsLIMSErrorsDataGridView.Columns(columnName).ReadOnly = True
            bsLIMSErrorsDataGridView.Columns(columnName).Visible = False

            columnName = "ImportDate"
            bsLIMSErrorsDataGridView.Columns.Add(columnName, "ImportDate")
            bsLIMSErrorsDataGridView.Columns(columnName).Width = 0
            bsLIMSErrorsDataGridView.Columns(columnName).DataPropertyName = columnName
            bsLIMSErrorsDataGridView.Columns(columnName).ReadOnly = True
            bsLIMSErrorsDataGridView.Columns(columnName).Visible = False

            columnName = "ErrorCode"
            bsLIMSErrorsDataGridView.Columns.Add(columnName, "ErrorCode")
            bsLIMSErrorsDataGridView.Columns(columnName).Width = 0
            bsLIMSErrorsDataGridView.Columns(columnName).DataPropertyName = columnName
            bsLIMSErrorsDataGridView.Columns(columnName).ReadOnly = True
            bsLIMSErrorsDataGridView.Columns(columnName).Visible = False

            bsLIMSErrorsDataGridView.ScrollBars = ScrollBars.Both
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".InitializeErrorsGrid ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".InitializeErrorsGrid", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Method incharge to load the image for each graphical button  
    ''' </summary>
    ''' ''' <remarks>
    ''' Created by:  SA 15/09/2010
    ''' </remarks>
    Private Sub PrepareButtons()
        Try
            Dim auxIconName As String = ""
            Dim iconPath As String = MyBase.IconsPath

            'EXIT Button
            auxIconName = GetIconName("CANCEL")
            If (auxIconName <> "") Then bsExitButton.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".PrepareButtons ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PrepareButtons ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Get texts in the current application language for all screen controls
    ''' </summary>
    ''' <param name="pLanguageID"> The current Language of Application </param>
    ''' <remarks>
    ''' Created by: PG 13/10/10
    ''' </remarks>
    Private Sub GetScreenLabels(ByVal pLanguageID As String)
        Try
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            'For CheckBox, RadioButtons...
            bsImportDTLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_LIMSErrors_DateTime", pLanguageID) + ":"
            bsLIMSErrorsLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "TITLE_LIMSImportErrors", pLanguageID)

            'For Tooltips...
            bsScreenToolTips.SetToolTip(bsExitButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_CloseScreen", pLanguageID))
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".GetScreenLabels", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".GetScreenLabels", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

#End Region

#Region "Events"

    ''' <summary>
    ''' When the screen ESC key is pressed, the screen is closed
    ''' </summary>
    Private Sub IWSImportLIMSErrors_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyDown
        Try
            If (e.KeyCode = Keys.Escape) Then
                'RH 04/07/2011 Escape key should do exactly the same operations as bsExitButton_Click()
                bsExitButton.PerformClick()
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".IWSImportLIMSErrors_KeyDown ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".IWSImportLIMSErrors_KeyDown", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub IWSImportLIMSErrors_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
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
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".IWSImportLIMSErrors_Load ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".IWSImportLIMSErrors_Load", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
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
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", ".WndProc " & Me.Name, EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".WndProc", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub bsLIMSErrorsDataGridView_SelectionChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles bsLIMSErrorsDataGridView.SelectionChanged
        bsLIMSErrorsDataGridView.ClearSelection()
    End Sub

    Private Sub bsExitButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles bsExitButton.Click
        Me.Close()
    End Sub
#End Region

End Class
