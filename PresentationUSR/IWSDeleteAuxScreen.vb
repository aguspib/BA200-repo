Option Strict On
Option Explicit On
Option Infer On
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.Types

Public Class UiWSDeleteAuxScreen
    Inherits Biosystems.Ax00.PresentationCOM.BSBaseForm

#Region "Attributes"
    Private ScreenUseAttribute As String = "VROTORS"
    'Private AnalyzerModelAttribute1 As String = "A400"
#End Region

#Region "Properties"
    Public Property ScreenUse() As String
        Get
            Return ScreenUseAttribute
        End Get
        Set(ByVal value As String)
            ScreenUseAttribute = value
        End Set
    End Property

    'Public WriteOnly Property AnalyzerModel() As String
    '    Set(ByVal value As String)
    '        AnalyzerModelAttribute = value
    '    End Set
    'End Property
#End Region

#Region "Methods"
    ''' <summary>
    ''' Not allow moving form and mantain the center location regarding the parent form
    ''' </summary>
    ''' <remarks>
    ''' Created by: DL 27/07/2011
    ''' </remarks>
    Protected Overrides Sub WndProc(ByRef m As Message)
        If (m.Msg = WM_WINDOWPOSCHANGING) Then
            Dim mySize As Size = Me.Parent.Size
            Dim myLocation As Point = Me.Parent.Location
            Dim pos As WINDOWPOS = DirectCast(Runtime.InteropServices.Marshal.PtrToStructure(m.LParam, GetType(WINDOWPOS)), WINDOWPOS)

            pos.x = myLocation.X + CInt((mySize.Width - Me.Width) / 2)
            pos.y = myLocation.Y + CInt((mySize.Height - Me.Height) / 2) - 60
            Runtime.InteropServices.Marshal.StructureToPtr(pos, m.LParam, True)
        End If
        MyBase.WndProc(m)
    End Sub

    ''' <summary>
    ''' Delete all Saved WorkSessions selected in the ListView
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 21/06/2010
    ''' </remarks>
    Private Sub DeleteSavedWS()
        Try
            'Load the needed DataSet with the list of selected Saved Work Sessions
            Dim selectedSavedWSDS As New SavedWSDS
            Dim selSavedWSRow As SavedWSDS.tparSavedWSRow

            For Each mySelectedItem As ListViewItem In bsElementsListView.SelectedItems
                selSavedWSRow = selectedSavedWSDS.tparSavedWS.NewtparSavedWSRow
                selSavedWSRow.SavedWSID = Convert.ToInt32(mySelectedItem.SubItems(1).Text)
                selectedSavedWSDS.tparSavedWS.Rows.Add(selSavedWSRow)
            Next

            'Delete the selected Saved Work Sessions
            Dim resultData As New GlobalDataTO
            Dim mySavedWSDelegate As New SavedWSDelegate

            resultData = mySavedWSDelegate.Delete(Nothing, selectedSavedWSDS)
            If (resultData.HasError) Then
                'An error has happened deleting the Saved WorkSessions
                ShowMessage(Me.Name & ".DeleteSavedWS", resultData.ErrorCode, resultData.ErrorMessage, Me)
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".DeleteSavedWS", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".DeleteSavedWS", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Delete all elements selected in the ListView, according the Screen Use
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 21/06/2010
    ''' </remarks>
    Private Sub DeleteSelectedElements()
        Try
            If (ShowMessage("", GlobalEnumerates.Messages.DELETE_CONFIRMATION.ToString) = Windows.Forms.DialogResult.Yes) Then
                If (String.Compare(ScreenUseAttribute, "VROTORS", False) = 0) Then
                    'Delete selected Virtual Rotors and reload the ListView
                    DeleteVirtualRotors()
                    LoadVirtualRotors(True)

                ElseIf (String.Compare(ScreenUseAttribute, "SAVEDWS", False) = 0) Then
                    'Delete selected Saved Work Sessions and reload the ListView
                    DeleteSavedWS()
                    LoadSavedWorkSessions(True)
                End If
            End If

            'Delete button is enabled only when there is at least one item selected in the ListView
            bsDeleteButton.Enabled = (bsElementsListView.SelectedItems.Count > 0)
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".DeleteSelectedElements", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".DeleteSelectedElements", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Delete all Virtual Rotors selected in the ListView
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 21/06/2010
    ''' </remarks>
    Private Sub DeleteVirtualRotors()
        Try
            'Load the needed DataSet with the list of selected Virtual Rotors
            Dim selectedVRotorsDS As New VirtualRotorsDS
            Dim selVRotorRow As VirtualRotorsDS.tparVirtualRotorsRow

            For Each mySelectedItem As ListViewItem In bsElementsListView.SelectedItems
                selVRotorRow = selectedVRotorsDS.tparVirtualRotors.NewtparVirtualRotorsRow
                selVRotorRow.VirtualRotorID = Convert.ToInt32(mySelectedItem.SubItems(0).Name)
                selectedVRotorsDS.tparVirtualRotors.Rows.Add(selVRotorRow)
            Next

            'Delete the selected Virtual Rotors
            Dim resultData As New GlobalDataTO
            Dim myVRotorsDelegate As New VirtualRotorsDelegate

            resultData = myVRotorsDelegate.Delete(Nothing, selectedVRotorsDS)
            If (resultData.HasError) Then
                'An error has happened deleting the selected Virtual Rotors
                ShowMessage(Me.Name & ".DeleteVirtualRotors", resultData.ErrorCode, resultData.ErrorMessage, Me)
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".DeleteVirtualRotors", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".DeleteVirtualRotors", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Get multilanguage labels needed for the ListView according the Screen Use
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 07/07/2010
    ''' Modified by: PG 13/10/2010 - Modified GetListViewTitles_ForTESTING. We have to differentiate between Rotor and Work Session. 
    '''                              We use the same screen by Save and Delete
    ''' </remarks>
    Private Sub GetListViewTitles(ByVal pLanguageID As String)
        Try
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            If (String.Compare(ScreenUseAttribute, "SAVEDWS", False) = 0) Then
                bsListViewTitleLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_DelAux_SavedWS", pLanguageID)
                bsElementsListView.Columns(0).Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_DelAux_SavedWSNames", pLanguageID)
            Else
                bsListViewTitleLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_DelAux_VRotors", pLanguageID)
                bsElementsListView.Columns(0).Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_DelAux_VRotorsNames", pLanguageID)
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".GetListViewTitles", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".GetListViewTitles", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
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

            ' For Tooltips...
            bsScreenToolTips.SetToolTip(bsExitButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_CloseScreen", pLanguageID))
            bsScreenToolTips.SetToolTip(bsDeleteButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_DelAux_DelSelected", pLanguageID))
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".GetScreenLabels", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".GetScreenLabels", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Configure and initialize the ListView according the Screen Use
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 18/06/2010 
    ''' </remarks>
    Private Sub InitializeElementsList()
        Try
            'Set properties that are common to all Screen Uses
            bsElementsListView.Alignment = ListViewAlignment.Left
            bsElementsListView.FullRowSelect = True
            bsElementsListView.MultiSelect = True
            bsElementsListView.Scrollable = True
            bsElementsListView.View = View.Details
            bsElementsListView.HideSelection = False

            If (String.Compare(ScreenUseAttribute, "VROTORS", False) = 0) Then
                LoadVirtualRotors(False)
            ElseIf (String.Compare(ScreenUseAttribute, "SAVEDWS", False) = 0) Then
                LoadSavedWorkSessions(False)
            End If

            'Delete button is enabled only when there is at least one item selected in the ListView
            bsDeleteButton.Enabled = (bsElementsListView.SelectedItems.Count > 0)
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".InitializeElementsList", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".InitializeElementsList", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Configure and initialize the ListView when the screen is used for Saved Work Sessions
    ''' </summary>
    ''' <param name="pRefresh">When False, it means the function was called during the Screen Loading and, in case
    '''                        there are not SavedWS, the correspondent information message is shown and the screen
    '''                        loading is cancelled; when True, it means the function was called after delete process</param>
    ''' <remarks>
    ''' Created by:  SA 18/06/2010 
    ''' Modified by: SA 23/09/2010 - Added the Dispose after every Close
    ''' Modified by: RH 18/10/2010 - Removed the Dispose after every Close
    '''              RH 30/06/2011 - Remove Not Found message. Code optimization.
    ''' </remarks>
    Private Sub LoadSavedWorkSessions(ByVal pRefresh As Boolean)
        Try
            bsElementsListView.Items.Clear()

            'List columns definition  --> Column for SavedWS Identifier is hidden
            bsElementsListView.Columns.Add("SavedWSName", -2, HorizontalAlignment.Left)
            bsElementsListView.Columns.Add("SavedWSID", 0, HorizontalAlignment.Left)

            'Load the list of Saved WS sorted by Name
            Dim resultData As GlobalDataTO
            Dim mySavedWSDelegate As New SavedWSDelegate

            resultData = mySavedWSDelegate.GetAll(Nothing)
            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                Dim mySavedWSDS As SavedWSDS = DirectCast(resultData.SetDatos, SavedWSDS)

                If (mySavedWSDS.tparSavedWS.Rows.Count > 0) Then
                    'Load elements in the ListView
                    Dim i As Integer = 0
                    For Each savedWS As SavedWSDS.tparSavedWSRow In mySavedWSDS.tparSavedWS.Rows
                        bsElementsListView.Items.Add(savedWS.SavedWSName)
                        bsElementsListView.Items(i).SubItems.Add(savedWS.SavedWSID.ToString())
                        i += 1
                    Next
                End If
            Else
                'Error getting the list of Saved WS 
                ShowMessage(Me.Name & ".LoadSavedWorkSessions", resultData.ErrorCode, resultData.ErrorMessage, Me)
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".LoadSavedWorkSessions", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".LoadSavedWorkSessions", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Configure and initialize the ListView when the screen is used for Virtual Rotors
    ''' </summary>
    ''' <param name="pRefresh">When False, it means the function was called during the Screen Loading and, in case
    '''                        there are not Virtual Rotors, the correspondent information message is shown and the screen
    '''                        loading is cancelled; when True, it means the function was called after delete process</param>
    ''' <remarks>
    ''' Created by:  SA 18/06/2010
    ''' Modified by: RH 30/06/2011 - Remove Not Found message. Code optimization.
    ''' Modified by: RH 21/02/2012 - Bug correction (different icon for different Rotor Type). Code simplification.
    ''' </remarks>
    Private Sub LoadVirtualRotors(ByVal pRefresh As Boolean)
        Try
            bsElementsListView.Items.Clear()

            'List columns definition  --> Column for VRotor Identifier is hidden
            bsElementsListView.Columns.Add("VRotorName", -2, HorizontalAlignment.Left)
            bsElementsListView.Columns.Add("VRotorID", 0, HorizontalAlignment.Left)

            'Load the list of existing Virtual Rotors sorted by Rotor Type and Rotor Name...
            Dim resultData As GlobalDataTO
            If (String.Compare(AnalyzerModel(), "A400", False) = 0) Then
                Dim myVRotorsDelegate As New VirtualRotorsDelegate

                'Get all Virtual Rotors 
                resultData = myVRotorsDelegate.GetVRotorsByRotorType(Nothing, "")
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    Dim myVRotorDS As VirtualRotorsDS = DirectCast(resultData.SetDatos, VirtualRotorsDS)

                    If (myVRotorDS.tparVirtualRotors.Rows.Count > 0) Then
                        'Get Icons for Reagents and Samples Rotors
                        Dim myIcons As New ImageList
                        Dim auxIconName As String = ""
                        Dim iconPath As String = MyBase.IconsPath

                        'Icon for RotorType=Reagents
                        auxIconName = GetIconName("REAGENTPOS")
                        If (String.Compare(auxIconName, String.Empty, False) <> 0) Then myIcons.Images.Add("REAGENTS", ImageUtilities.ImageFromFile(iconPath & auxIconName))

                        'Icon for RotorType=Samples
                        auxIconName = GetIconName("SAMPLEPOS")
                        If (String.Compare(auxIconName, String.Empty, False) <> 0) Then myIcons.Images.Add("SAMPLES", ImageUtilities.ImageFromFile(iconPath & auxIconName))

                        'Link the Icons to the ListView
                        bsElementsListView.SmallImageList = myIcons

                        'Load elements in the ListView
                        Dim i As Integer = 0
                        For Each vRotors As VirtualRotorsDS.tparVirtualRotorsRow In myVRotorDS.tparVirtualRotors.Rows
                            bsElementsListView.Items.Add(vRotors.VirtualRotorID.ToString(), vRotors.VirtualRotorName, vRotors.RotorType)
                            bsElementsListView.Items(i).SubItems.Add(vRotors.VirtualRotorID.ToString())
                            i += 1
                        Next
                    End If
                Else
                    'Error getting the list of VirtualRotors 
                    ShowMessage(Me.Name & ".LoadVirtualRotors", resultData.ErrorCode, resultData.ErrorMessage, Me)
                End If
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".LoadVirtualRotors", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".LoadVirtualRotors", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Search Icons for screen buttons
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 18/06/2010 
    ''' </remarks>
    Private Sub PrepareButtons()
        Try
            Dim auxIconName As String = ""
            Dim iconPath As String = MyBase.IconsPath

            'DELETE Button
            auxIconName = GetIconName("REMOVE")
            If (String.Compare(auxIconName, "", False) <> 0) Then bsDeleteButton.BackgroundImage = ImageUtilities.ImageFromFile(iconPath & auxIconName)

            'CANCEL Button
            auxIconName = GetIconName("CANCEL")
            If (String.Compare(auxIconName, "", False) <> 0) Then bsExitButton.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".PrepareButtons ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PrepareButtons ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Initialize the screen when loading
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 18/06/2010 
    ''' Modified by: PG 13/10/2010 - Get the current language
    ''' </remarks>
    Private Sub ScreenLoad()
        Try
            Dim mySize As Size = Me.Parent.Size
            Dim myLocation As Point = Me.Parent.Location
            Me.Location = New Point(myLocation.X + CInt((mySize.Width - Me.Width) / 2), myLocation.Y + CInt((mySize.Height - Me.Height) / 2) - 60)

            'Get the current Language from the current Application Session
            'Dim currentLanguageGlobal As New GlobalBase
            Dim currentLanguage As String = GlobalBase.GetSessionInfo().ApplicationLanguage

            PrepareButtons()
            GetScreenLabels(currentLanguage)
            InitializeElementsList()
            GetListViewTitles(currentLanguage)
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".InitializeScreen ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".InitializeScreen ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
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
    Private Sub WSDeleteAuxScreen_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyDown
        Try
            If (e.KeyCode = Keys.Escape) Then
                'RH 30/06/2011 Escape key should do exactly the same operations as bsExitButton_Click()
                bsExitButton.PerformClick()
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".WSDeleteAuxScreen_KeyDown", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".WSDeleteAuxScreen_KeyDown", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub WSDeleteAuxScreen_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ScreenLoad()
        BsTimer1.Enabled = True
    End Sub

    Private Sub BsTimer1_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsTimer1.Tick
        BsTimer1.Enabled = False
        'If no elements were loaded in the ListView, the screen is not shown
        If (bsElementsListView.Items.Count = 0) Then
            If (String.Compare(ScreenUseAttribute, "VROTORS", False) = 0) Then
                'Show the Error Message - There are not Virtual Rotors to Load
                ShowMessage(Me.Name, GlobalEnumerates.Messages.NO_VIRTUAL_ROTORS.ToString, "", Me)

            ElseIf (String.Compare(ScreenUseAttribute, "SAVEDWS", False) = 0) Then
                'Show the Error Message - There are not WS to Load
                ShowMessage(Me.Name, GlobalEnumerates.Messages.WS_NOT_FOUND.ToString, "", Me)
            End If

            'RH 17/12/2010
            'Open the WS Monitor form and close this one
            UiAx00MainMDI.OpenMonitorForm(Me)
        End If
    End Sub

    Private Sub bsDeleteButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles bsDeleteButton.Click
        DeleteSelectedElements()
    End Sub

    Private Sub bsExitButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsExitButton.Click
        Try
            'TR 11/04/2012 -Disable form on close to avoid any button press.
            Me.Enabled = False

            'RH 17/12/2010
            If Not Me.Tag Is Nothing Then
                'A PerformClick() method was executed
                Me.Close()
            Else
                'Normal button click
                'Open the WS Monitor form and close this one
                UiAx00MainMDI.OpenMonitorForm(Me)
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsExitButton_Click", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsExitButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try

    End Sub

    Private Sub bsElementsListView_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles bsElementsListView.Click
        'Delete button is enabled only when there is at least one item selected in the ListView
        bsDeleteButton.Enabled = (bsElementsListView.SelectedItems.Count > 0)
    End Sub

    Private Sub bsElementsListView_ColumnWidthChanging(ByVal sender As Object, ByVal e As System.Windows.Forms.ColumnWidthChangingEventArgs) Handles bsElementsListView.ColumnWidthChanging
        e.Cancel = True
        e.NewWidth = bsElementsListView.Width - 5
    End Sub

    Private Sub bsElementsListView_ItemSelectionChanged(ByVal sender As Object, ByVal e As System.Windows.Forms.ListViewItemSelectionChangedEventArgs) Handles bsElementsListView.ItemSelectionChanged
        'Delete button is enabled only when there is at least one item selected in the ListView
        bsDeleteButton.Enabled = (bsElementsListView.SelectedItems.Count > 0)
    End Sub

    Private Sub bsElementsListView_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles bsElementsListView.KeyDown
        If (e.KeyCode = Keys.Delete) Then
            DeleteSelectedElements()
        End If
    End Sub
#End Region

End Class
