
Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global

Public Class IProgAuxTestContaminations

#Region "Declarations"
    Dim myNewLocation As Point    'Used to avoid the screen movement
#End Region

#Region "Methods"
    Public Sub New()
        'This call is required by the Windows Form Designer
        InitializeComponent()
    End Sub

    ''' <summary>
    ''' Method incharge to load the buttons images
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 21/12/2010
    ''' </remarks>
    Private Sub PrepareButtons()
        Try
            Dim auxIconName As String = ""
            Dim iconPath As String = MyBase.IconsPath

            'EXIT Button
            auxIconName = GetIconName("CANCEL")
            If (auxIconName <> "") Then bsExitButton.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".PrepareButtons ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PrepareButtons ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Screen initialization when loading
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 21/12/2010
    ''' </remarks>
    Private Sub ScreenLoad()
        Try
            Dim myLocation As Point = IAx00MainMDI.PointToScreen(Point.Empty)
            Dim mySize As Size = IAx00MainMDI.Size

            myNewLocation = New Point(myLocation.X + CInt((mySize.Width - Me.Width) / 2), myLocation.Y + CInt((mySize.Height - Me.Height) / 2))
            Me.Location = myNewLocation

            'Get the current Language from the current Application Session
            Dim currentLanguageGlobal As New GlobalBase
            Dim currentLanguage As String = currentLanguageGlobal.GetSessionInfo().ApplicationLanguage.Trim.ToString

            'Load the multilanguage texts for all Screen Labels and get Icons for graphical Buttons
            GetScreenLabels(currentLanguage)
            PrepareButtons()

            'InitializeGridViews()
            InitializeGridViews(currentLanguage)
            LoadGridViews()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ScreenLoad ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ScreenLoad ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Get texts in the current application language for all screen controls
    ''' </summary>
    ''' <param name="pLanguageID">Current Application Language</param>
    ''' <remarks>
    ''' Created by:  DL 21/12/2010
    ''' Modified by: PG 11/01/2011 - Get all screen texts in the current application language
    ''' </remarks>
    Private Sub GetScreenLabels(ByVal pLanguageID As String)
        Try
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            bsContaminationsLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Contaminations", pLanguageID)
            bsCuvettesLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Cuvettes", pLanguageID)
            bsScreenToolTips.SetToolTip(bsExitButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_CloseScreen", pLanguageID))
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".GetScreenLabels", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".GetScreenLabels", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Initialization of DataGridViews for Contaminations and Cuvettes
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 21/12/2010
    ''' Modified by: PG 11/01/2011. Change language text in function current language
    ''' </remarks>
    Private Sub InitializeGridViews(ByVal pLanguageID As String)
        Try
            Dim columnName As String
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            'Configuration of Contaminations DataGridView
            bsContaminationsDataGridView.AllowUserToAddRows = False
            bsContaminationsDataGridView.AllowUserToDeleteRows = False

            bsContaminationsDataGridView.Rows.Clear()
            bsContaminationsDataGridView.Columns.Clear()

            'Contaminator column 
            columnName = "Contaminator"
            bsContaminationsDataGridView.Columns.Add(columnName, myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Contaminator", pLanguageID))
            bsContaminationsDataGridView.Columns(columnName).Width = 139 '150 JB - 13/11/2012
            bsContaminationsDataGridView.Columns(columnName).DataPropertyName = columnName
            bsContaminationsDataGridView.Columns(columnName).ReadOnly = True
            bsContaminationsDataGridView.Columns(columnName).Visible = True

            'Contaminated column
            columnName = "Contaminated"
            bsContaminationsDataGridView.Columns.Add(columnName, myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Contaminated", pLanguageID))
            bsContaminationsDataGridView.Columns(columnName).Width = 139 '150 JB - 13/11/2012
            bsContaminationsDataGridView.Columns(columnName).Visible = True
            bsContaminationsDataGridView.Columns(columnName).DataPropertyName = columnName
            bsContaminationsDataGridView.Columns(columnName).ReadOnly = True

            'Washing Solution column
            columnName = "Wash"
            bsContaminationsDataGridView.Columns.Add(columnName, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_INST_Wash", pLanguageID))
            bsContaminationsDataGridView.Columns(columnName).Width = 100
            bsContaminationsDataGridView.Columns(columnName).Visible = False
            bsContaminationsDataGridView.Columns(columnName).DataPropertyName = columnName
            bsContaminationsDataGridView.Columns(columnName).ReadOnly = True

            'Multilanguage description of Washing Solution
            columnName = "WashDesc"
            bsContaminationsDataGridView.Columns.Add(columnName, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_INST_Wash", pLanguageID))
            bsContaminationsDataGridView.Columns(columnName).Width = 159 '153 JB - 13/11/2012
            bsContaminationsDataGridView.Columns(columnName).Visible = True
            bsContaminationsDataGridView.Columns(columnName).DataPropertyName = columnName
            bsContaminationsDataGridView.Columns(columnName).ReadOnly = True

            'Configuration of Cuvettes DataGridView
            bsCuvettesDataGridView.AllowUserToAddRows = False
            bsCuvettesDataGridView.AllowUserToDeleteRows = False

            bsCuvettesDataGridView.Rows.Clear()
            bsCuvettesDataGridView.Columns.Clear()

            'Contaminators column
            columnName = "Contaminators"
            bsCuvettesDataGridView.Columns.Add(columnName, myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Contaminators", pLanguageID))
            bsCuvettesDataGridView.Columns(columnName).Width = 136 '170 JB - 13/11/2012
            bsCuvettesDataGridView.Columns(columnName).DataPropertyName = columnName
            bsCuvettesDataGridView.Columns(columnName).ReadOnly = True
            bsCuvettesDataGridView.Columns(columnName).Visible = True

            'First Washing Solution
            columnName = "Step1"
            bsCuvettesDataGridView.Columns.Add(columnName, myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Step_1", pLanguageID))
            bsCuvettesDataGridView.Columns(columnName).Width = 100
            bsCuvettesDataGridView.Columns(columnName).DataPropertyName = columnName
            bsCuvettesDataGridView.Columns(columnName).ReadOnly = True
            bsCuvettesDataGridView.Columns(columnName).Visible = False

            'Multilanguage description of the first Washing Solution
            columnName = "Step1Desc"
            bsCuvettesDataGridView.Columns.Add(columnName, myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Step_1", pLanguageID))
            bsCuvettesDataGridView.Columns(columnName).Width = 159 '142 JB - 13/11/2012
            bsCuvettesDataGridView.Columns(columnName).DataPropertyName = columnName
            bsCuvettesDataGridView.Columns(columnName).ReadOnly = True
            bsCuvettesDataGridView.Columns(columnName).Visible = True

            'Second Washing Solution
            columnName = "Step2"
            bsCuvettesDataGridView.Columns.Add(columnName, myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Step_2", pLanguageID))
            bsCuvettesDataGridView.Columns(columnName).Width = 100
            bsCuvettesDataGridView.Columns(columnName).DataPropertyName = columnName
            bsCuvettesDataGridView.Columns(columnName).ReadOnly = True
            bsCuvettesDataGridView.Columns(columnName).Visible = False

            'Multilanguage description of the second Washing Solution
            columnName = "Step2Desc"
            bsCuvettesDataGridView.Columns.Add(columnName, myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Step_2", pLanguageID))
            bsCuvettesDataGridView.Columns(columnName).Width = 159 '142 JB - 13/11/2012
            bsCuvettesDataGridView.Columns(columnName).DataPropertyName = columnName
            bsCuvettesDataGridView.Columns(columnName).ReadOnly = True
            bsCuvettesDataGridView.Columns(columnName).Visible = True
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".InitializeGridViews ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".InitializeGridViews ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Load the DataGridViews of First Reagent Contaminations and Cuvettes Contaminations
    ''' </summary>
    ''' <remarks>
    ''' Created by: DL 21/12/2010
    ''' </remarks>
    Private Sub LoadGridViews()
        Try
            Dim resultData As New GlobalDataTO
            Dim myTestcontaminationsDelegate As New ContaminationsDelegate

            'Read all Contaminations defined for the first Reagents
            resultData = myTestcontaminationsDelegate.GetTestContaminatorsR1(Nothing)
            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                Dim ContaminationsAuxR1 As ContaminationsAuxR1DS = DirectCast(resultData.SetDatos, ContaminationsAuxR1DS)

                Dim viewR1Tests As DataView = New DataView
                viewR1Tests = ContaminationsAuxR1.tparContaminations.DefaultView
                viewR1Tests.Sort = "Contaminator ASC, Contaminated ASC, Wash ASC"

                Dim bsR1TestsBindingSource As BindingSource = New BindingSource
                bsR1TestsBindingSource.DataSource = viewR1Tests
                bsContaminationsDataGridView.DataSource = bsR1TestsBindingSource

                'Get all defined Cuvettes Contaminations 
                resultData = myTestcontaminationsDelegate.GetTestContaminatorsCuv(Nothing)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    Dim ContaminationsAuxCu As ContaminationsAuxCuDS = DirectCast(resultData.SetDatos, ContaminationsAuxCuDS)

                    Dim viewR2Tests As DataView = New DataView
                    viewR2Tests = ContaminationsAuxCu.tparContaminations.DefaultView
                    viewR2Tests.Sort = "Contaminators ASC, Step1 ASC, Step2 ASC"

                    Dim bsR2TestsBindingSource As BindingSource = New BindingSource
                    bsR2TestsBindingSource.DataSource = viewR2Tests
                    bsCuvettesDataGridView.DataSource = bsR2TestsBindingSource
                Else
                    'Error getting the list of defined Cuvettes Contaminations; show it
                    ShowMessage(Me.Name & ".LoadGridViews ", resultData.ErrorCode, resultData.ErrorMessage, Me)
                End If
            Else
                'Error getting the list of defined Contaminations between first Reagents; shown it
                ShowMessage(Me.Name & ".LoadGridViews", resultData.ErrorCode, resultData.ErrorMessage, Me)
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".LoadGridViews ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".LoadGridViews ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Close the screen
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 21/12/2010
    ''' </remarks>
    Private Sub ExitScreen()
        Try
            Me.Close()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ExitScreen ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ExitScreen", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub
#End Region

#Region "Events"
    ''' <summary>
    ''' When the screen ESC key is pressed, the screen is closed
    ''' </summary>
    Private Sub ProgAuxTestContaminations_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyDown
        Try
            If (e.KeyCode = Keys.Escape) Then
                bsExitButton.PerformClick()
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ProgAuxTestContaminations_KeyDown ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ProgAuxTestContaminations_KeyDown", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Screen loading
    ''' </summary>
    Private Sub ProgAuxTestContaminations_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            ScreenLoad()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ProgAuxTestContaminations_Load ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ProgAuxTestContaminations_Load ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Avoid the screen movement
    ''' </summary>
    Private Sub IProgAuxTestContaminations_Move(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Move
        Me.Location = myNewLocation
    End Sub

    ''' <summary>
    ''' Screen closing
    ''' </summary>
    Private Sub bsExitButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles bsExitButton.Click
        Try
            ExitScreen()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsExitButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsExitButton_Click ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub
#End Region


End Class
