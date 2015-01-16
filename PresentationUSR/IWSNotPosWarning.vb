Option Explicit On
Option Strict On

Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.BL

Public Class IWSNotPosWarning
    Inherits Biosystems.Ax00.PresentationCOM.BSBaseForm

#Region "Attributes"
    Private WorkSessionIDAttribute As String
    Private OpenModeAttribute As String
#End Region

#Region "Properties"
    Public WriteOnly Property ActiveWorkSession() As String
        Set(ByVal value As String)
            WorkSessionIDAttribute = value
        End Set
    End Property

    Public WriteOnly Property OpenMode() As String
        Set(ByVal value As String)
            OpenModeAttribute = value
        End Set
    End Property
#End Region

#Region "Private Methods"

    ''' <summary>
    ''' Screen initialization
    ''' </summary>
    ''' <remarks>
    ''' Modified by: PG 13/10/2010 - Get the current language
    '''              SA 08/11/2010 - Set visibility of Return to Positioning button according value of 
    '''                              entry property OpenMode 
    ''' </remarks>
    Private Sub ScreenLoad()
        Try
            'DL 28/07/2011
            Dim mySize As Size = IAx00MainMDI.Size
            Dim myLocation As Point = IAx00MainMDI.Location
            Me.Location = New Point(myLocation.X + CInt((mySize.Width - Me.Width) / 2), myLocation.Y + CInt((mySize.Height - Me.Height) / 2))
            'END DL 28/07/2011

            'Get the current Language from the current Application Session
            'Dim currentLanguageGlobal As New GlobalBase
            Dim currentLanguage As String = GlobalBase.GetSessionInfo().ApplicationLanguage.Trim.ToString

            'Load images for graphical buttons and picture boxes
            PrepareButtons()
            PrepareIcons()

            'Get multilanguage labels for all screen controls
            GetScreenLabels(currentLanguage)

            'Get multilanguage text for the warning label
            LoadWarningLabel()

            'Initialize the grid
            InitializeNotPosWarningGrid(currentLanguage)

            'Get the list of not positioned elements and load them in the grid
            Dim resultData As New GlobalDataTO
            Dim myRequiredElementsDelegate As New WSRequiredElementsDelegate

            resultData = myRequiredElementsDelegate.GetNotPositionedElements(Nothing, WorkSessionIDAttribute)
            If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                Dim myNoPosWSRequiredElementsDS As New WSNoPosRequiredElementsDS
                myNoPosWSRequiredElementsDS = DirectCast(resultData.SetDatos, WSNoPosRequiredElementsDS)

                'DL 22/07/2011
                If (OpenModeAttribute = "TO_EXIT") Then
                    If myNoPosWSRequiredElementsDS.twksWSRequiredElements.Rows.Count > 0 Then
                        InitializeScreen(myNoPosWSRequiredElementsDS)
                    Else
                        'If there is nothing to shown then close...
                        Me.Close()
                    End If

                Else
                    InitializeScreen(myNoPosWSRequiredElementsDS)
                End If
            Else
                ShowMessage(Me.Name & ".ScreenLoad", resultData.ErrorCode, resultData.ErrorMessage, Me)
                Me.Close()
            End If

            'Button for returning to Rotor Positioning Screen is visible only when the screen was opened due to 
            'the Create Executions button was clicked in Rotor Positioning Screen
            bsPositioningButton.Visible = (OpenModeAttribute = "TO_EXIT")
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ScreenLoad", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ScreenLoad", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Load the grid of not positioned WS Elements
    ''' </summary>
    ''' <param name="pWSNoPosRequiredElementsDS">Typed DataSet WSNoPosRequiredElementsDS containing the list of
    '''                                          not positioned WS Elements in the order in which have to be shown
    '''                                          in the grid</param>
    ''' <remarks>
    ''' Modified by: SA 28/11/2011 - Changes due column Sample Type has been removed from the grid
    ''' Modified by: JC 13/11/2012 - Modify column width.
    ''' </remarks>
    Private Sub InitializeScreen(ByVal pWSNoPosRequiredElementsDS As WSNoPosRequiredElementsDS)
        Try
            'Load the not positioned elements
            Dim rowIndex As Integer
            For i As Integer = 0 To pWSNoPosRequiredElementsDS.twksWSRequiredElements.Rows.Count - 1
                rowIndex = bsNotPositionedDataGridView.Rows.Add()

                bsNotPositionedDataGridView.Rows(rowIndex).Cells("SampleClassIcon").Value = pWSNoPosRequiredElementsDS.twksWSRequiredElements(i).SampleClassIcon
                bsNotPositionedDataGridView.Rows(rowIndex).Cells("SampleName").Value = pWSNoPosRequiredElementsDS.twksWSRequiredElements(i).SampleName
            Next

            Dim gridWidth As Integer = bsNotPositionedDataGridView.Width
            bsNotPositionedDataGridView.Columns(0).Width = CInt(0.2 * gridWidth)
            bsNotPositionedDataGridView.Columns(1).Width = CInt(0.8 * gridWidth)
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".InitializeScren", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".InitializeScren", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Method incharge to load the image for each graphical button
    ''' </summary>
    ''' <remarks>
    ''' Created by: SG
    ''' Modified by: DL 04/11/2010 - Load Icon in Image Property instead of in BackGroundImage property
    '''              SA 08/11/2010 - Get Icon for new button to return to Rotor Positioning screen
    '''              TR 27/09/2013 - Change the bsExitButton Icon depending of OpenModeAttribute will show 
    '''                              the Accept1 else Cancel. Bug# 1280.
    ''' </remarks>
    Private Sub PrepareButtons()
        Try
            Dim auxIconName As String = ""
            Dim iconPath As String = MyBase.IconsPath

            'POSITIONING Button
            auxIconName = GetIconName("SENDTOPOS")
            If (auxIconName <> "") Then
                bsPositioningButton.Image = Image.FromFile(iconPath & auxIconName)
            End If

            'TR 27/09/2013 - Validate the OpenModeAttribute to set the corresponding ICON
            'EXIT Button
            If OpenModeAttribute = "TO_EXIT" Then
                auxIconName = GetIconName("ACCEPT1")
            Else
                auxIconName = GetIconName("CANCEL")
            End If
            'TR 27/09/2013 -END.
            If auxIconName <> "" Then
                bsExitButton.Image = Image.FromFile(iconPath & auxIconName)
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".PrepareButtons ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PrepareButtons ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Initialize the grid of not positioned WS Elements
    ''' </summary>
    ''' <remarks>
    ''' Modified by: PG 13/10/2010 - Added the LanguageID parameter and get the MultiLanguage text for
    '''                              all visible columns
    '''              SA 28/11/2011 - Hide column SampleType. For Patient Samples, the SampleType is shown now
    '''                              as part of the SampleName(at the end of it, separate by a dash character)
    ''' </remarks>
    Private Sub InitializeNotPosWarningGrid(ByVal pLanguageID As String)
        Try
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate
            Dim columnName As String

            bsNotPositionedDataGridView.AutoGenerateColumns = False
            bsNotPositionedDataGridView.AllowUserToAddRows = False
            bsNotPositionedDataGridView.AllowUserToDeleteRows = False
            bsNotPositionedDataGridView.EditMode = DataGridViewEditMode.EditProgrammatically

            bsNotPositionedDataGridView.Rows.Clear()
            bsNotPositionedDataGridView.Columns.Clear()

            'Sample Class Icon column
            Dim iconSampleClassColumn As New DataGridViewImageColumn
            columnName = "SampleClassIcon"
            iconSampleClassColumn.Name = columnName
            iconSampleClassColumn.HeaderText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SampleClass_Short", pLanguageID)

            bsNotPositionedDataGridView.Columns.Add(iconSampleClassColumn)
            bsNotPositionedDataGridView.Columns(columnName).DataPropertyName = columnName
            bsNotPositionedDataGridView.Columns(columnName).ReadOnly = True
            bsNotPositionedDataGridView.Columns(columnName).SortMode = DataGridViewColumnSortMode.Programmatic

            'SampleName column
            columnName = "SampleName"
            bsNotPositionedDataGridView.Columns.Add(columnName, myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_NotPosWng_SampleName", pLanguageID))
            bsNotPositionedDataGridView.Columns(columnName).DataPropertyName = columnName
            bsNotPositionedDataGridView.Columns(columnName).ReadOnly = True
            bsNotPositionedDataGridView.Columns(columnName).Visible = True
            bsNotPositionedDataGridView.Columns(columnName).SortMode = DataGridViewColumnSortMode.Programmatic

            bsNotPositionedDataGridView.ScrollBars = ScrollBars.Vertical
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".InitializeNotPosWarningGrid", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".InitializeNotPosWarningGrid", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Get the Icon Name for the Warning PictureBox in the advice area
    ''' </summary>
    ''' <remarks>
    ''' Modified by: DL 04/11/2010 - Load the icon using BackgroundImage property instead of Image property
    ''' </remarks>
    Private Sub PrepareIcons()
        Try
            'Get the Icon Name for the Warning PictureBox
            Dim WARNING_IconName As String = ""
            WARNING_IconName = GetIconName("STUS_WITHERRS") 'WARNING") dl 23/03/2012

            bsWarningPictureBox.BackgroundImage = Image.FromFile(MyBase.IconsPath & WARNING_IconName)
            bsWarningPictureBox.BackgroundImageLayout = ImageLayout.Stretch
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".PrepareIconNames ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PrepareIconNames ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Writes the text of the warning label
    ''' </summary>
    Private Sub LoadWarningLabel()
        Try
            Dim Messages As New MessageDelegate
            Dim myGlobalDataTO As New GlobalDataTO

            myGlobalDataTO = Messages.GetMessageDescription(Nothing, GlobalEnumerates.Messages.CHECK_NOPOS_ELEMENTS.ToString)
            If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                Dim myMessagesDS As New MessagesDS
                myMessagesDS = DirectCast(myGlobalDataTO.SetDatos, MessagesDS)

                If (myMessagesDS.tfmwMessages.Rows.Count > 0) Then Me.bsAdviceLabel.Text = myMessagesDS.tfmwMessages(0).MessageText
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".LoadWarningLabel", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".LoadWarningLabel", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Get texts in the current application language for all screen controls
    ''' </summary>
    ''' <param name="pLanguageID"> The current Language of Application </param>
    ''' <remarks>
    ''' Created by:  PG 13/10/2010  
    ''' Modified by: SA 08/11/2010 - Add multilanguage text for ToolTip of new button to return 
    '''                              to Positioning Screen
    '''              TR 27/09/2013 - change the bsExitButton tooltip  depending on the OpenModeAttibute it will show diferent tips.
    '''                              Bug #1280.
    ''' </remarks>
    Private Sub GetScreenLabels(ByVal pLanguageID As String)
        Try
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            'For Labels, CheckBox, RadioButtons...
            bsNotPosWarningLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_NotPosWng_NoPosElements", pLanguageID) + ":"
            bsNotPosWarningTitleLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "TITLE_NotPosWarning", pLanguageID)

            'For Tooltips...
            bsScreenToolTips.SetToolTip(bsPositioningButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_NotPosWng_ReturnToPos", pLanguageID))

            'TR 27/09/2013 -Validate to indicate the tooltip.
            If OpenModeAttribute = "TO_EXIT" Then
                bsScreenToolTips.SetToolTip(bsExitButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Close&Continue", pLanguageID))
            Else
                bsScreenToolTips.SetToolTip(bsExitButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_CloseScreen", pLanguageID))
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".GetScreenLabels", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".GetScreenLabels", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub
#End Region

#Region "Events"
    ''' <summary>
    ''' When the screen ESC key is pressed, the screen is closed
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 09/11/2010
    ''' </remarks>
    Private Sub WSNotPosWarning_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyDown
        Try
            If (e.KeyCode = Keys.Escape) Then
                Me.DialogResult = Windows.Forms.DialogResult.Cancel
                bsExitButton.PerformClick()
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".WSNotPosWarning_KeyDown ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".WSNotPosWarning_KeyDown", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub WSNotPosWarning_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try
            ScreenLoad()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".WSNotPosWarning_Load ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".WSNotPosWarning_Load", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
            Me.Close()
        End Try
    End Sub

    Private Sub bsNotPositionedDataGridView_SelectionChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)
        bsNotPositionedDataGridView.ClearSelection()
    End Sub

    Private Sub bsPositioningButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles bsPositioningButton.Click
        ' XB 27/11/2013 - Inform to MDI that below screen will shown - Task #1303
        ShownScreen()
        Me.DialogResult = Windows.Forms.DialogResult.OK
        Me.Close()
    End Sub

    Private Sub bsExitButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsExitButton.Click
        ' XB 27/11/2013 - Inform to MDI that this screen is closing aims to open next screen - Task #1303
        ExitingScreen()
        IAx00MainMDI.EnableButtonAndMenus(True)
        Application.DoEvents()
        Me.DialogResult = Windows.Forms.DialogResult.Cancel
        Me.Close()
    End Sub

    ''' <summary>
    ''' Not allow move form and mantain the center location in center parent
    ''' </summary>
    ''' <remarks>
    ''' Created by: DL 27/07/2011
    ''' </remarks>
    Protected Overrides Sub WndProc(ByRef m As Message)
        Try
            If m.Msg = WM_WINDOWPOSCHANGING Then
                Dim pos As WINDOWPOS = DirectCast(Runtime.InteropServices.Marshal.PtrToStructure(m.LParam, GetType(WINDOWPOS)), WINDOWPOS)
                Dim myLocation As Point = IAx00MainMDI.Location
                Dim mySize As Size = IAx00MainMDI.Size

                pos.x = myLocation.X + CInt((mySize.Width - Me.Width) / 2)
                pos.y = myLocation.Y + CInt((mySize.Height - Me.Height) / 2)
                Runtime.InteropServices.Marshal.StructureToPtr(pos, m.LParam, True)
            End If

            MyBase.WndProc(m)
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".WndProc ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".WndProc", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub
#End Region
End Class