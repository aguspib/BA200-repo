Option Explicit On
Option Strict On
Option Infer On

Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Controls.UserControls

Public Class IQCResultsEditionAux

#Region "Declarations"
    Private LocalChange As Boolean = False
    Private LocalValueChange As Boolean = False
    Private LocalDecimalsAllowed As Integer = 0

    'To avoid the screen movement
    Dim myNewLocation As Point
#End Region

#Region "Attributes"
    Private LanguageIDAttribute As String
    Private ResultInformationDSAttribute As New ResultInformationDS
#End Region

#Region "Properties"
    Public WriteOnly Property LanguageID() As String
        Set(ByVal value As String)
            LanguageIDAttribute = value
        End Set
    End Property

    Public WriteOnly Property ResultInformationDS() As ResultInformationDS
        Set(ByVal value As ResultInformationDS)
            If value Is Nothing Then
                ResultInformationDSAttribute = New ResultInformationDS
            Else
                ResultInformationDSAttribute = value
            End If
        End Set
    End Property
#End Region

#Region "Methods"
    ''' <summary>
    ''' Bind each screen control with its correspondent data 
    ''' </summary>
    ''' <remarks>
    ''' Created by:  TR 07/06/2011
    ''' Modified by: SA 25/04/2012 - Set value of LocalDecimalsAllowed before use it to format the received Result
    ''' </remarks>
    Private Sub BindControls()
        Try
            If (ResultInformationDSAttribute.tResultInformation.Count > 0) Then
                LocalDecimalsAllowed = ResultInformationDSAttribute.tResultInformation(0).DecimalsAllowed

                bsTestSampleTypeTextBox.Text = ResultInformationDSAttribute.tResultInformation(0).TestName & " [" & _
                                               ResultInformationDSAttribute.tResultInformation(0).SampleTypeCode & "] "
                bsControlNameTextBox.Text = ResultInformationDSAttribute.tResultInformation(0).ControlName
                bsLotNumberTextBox.Text = ResultInformationDSAttribute.tResultInformation(0).LotNumber
                bsRunNumberTextBox.Text = ResultInformationDSAttribute.tResultInformation(0).CalNumberSeries.ToString()
                bsResultValueTextBox.Text = ResultInformationDSAttribute.tResultInformation(0).ResultValue.ToString("F" & LocalDecimalsAllowed)
                bsMeasureUnitLabel.Text = ResultInformationDSAttribute.tResultInformation(0).TestMeasureUnit

                If (Not ResultInformationDSAttribute.tResultInformation(0).IsResultCommentNull) Then
                    bsRemarkTextBox.Text = ResultInformationDSAttribute.tResultInformation(0).ResultComment
                End If

                If (Not ResultInformationDSAttribute.tResultInformation(0).IsExcludedNull) Then
                    bsExcludeResultCheckBox.Checked = ResultInformationDSAttribute.tResultInformation(0).Excluded
                End If


            End If

            'After loading data set local value change to false.
            LocalChange = False
            LocalValueChange = False
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".BindControls ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".BindControls ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Get all the screen labels in the selected application language
    ''' </summary>
    ''' <remarks>
    ''' Created by: TR 07/06/2001
    ''' </remarks>
    Private Sub GetScreenLabels()
        Try
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            bsResultEditionLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Result_Edition", LanguageIDAttribute)
            bsTestSampleTypeLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_TestName", LanguageIDAttribute) & ":"
            bsControlLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_WSPrep_Control", LanguageIDAttribute) & ":"
            bsLotNumberLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_LotNumber", LanguageIDAttribute) & ":"
            bsResultLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Result", LanguageIDAttribute) & ":"
            bsRemarksLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Remarks", LanguageIDAttribute) & ":"
            bsExcludeResultCheckBox.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Exlude_Result", LanguageIDAttribute)
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".GetScreenLabels", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".GetScreenLabels", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Screen initialization
    ''' </summary>
    ''' <remarks>
    ''' Created by:  TR 07/06/2011
    ''' </remarks>
    Private Sub InitializeScreen()
        Try
            PrepareButtons()
            GetScreenLabels()
            SetControlsBackGround()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".InitializeScreen ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".InitializeScreen ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Get Icon and ToolTipText for all screen buttons
    ''' </summary>
    ''' <remarks>
    ''' Created by: TR 07/06/2011
    ''' </remarks>
    Private Sub PrepareButtons()
        Try
            Dim auxIconName As String = ""
            Dim iconPath As String = MyBase.IconsPath
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            'SAVE Button
            auxIconName = GetIconName("ACCEPT1")
            If (auxIconName <> "") Then
                bsAcceptButton.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
                myToolTipsControl.SetToolTip(bsAcceptButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Save&Close", LanguageIDAttribute))
            End If

            'EXIT Button
            auxIconName = GetIconName("CANCEL")
            If (auxIconName <> "") Then
                bsExitButton.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
                myToolTipsControl.SetToolTip(bsExitButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Cancel", LanguageIDAttribute))
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".PrepareButtons ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PrepareButtons ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Save all changes made over the QC Result
    ''' </summary>
    ''' <returns>True if the saving was sucessful; otherwise, False</returns>
    ''' <remarks>
    ''' Created by:  TR 07/06/2011
    ''' Modified by: SA 05/06/2012 - Informed field AnalyzerID in the DS used to update the QC Result  
    ''' </remarks>
    Private Function SaveChanges() As Boolean
        Dim myResult As Boolean = True
        Try
            'Add the result to update to a QCResultsDS
            Dim myQCResultsDS As New QCResultsDS
            Dim myQCResultsRow As QCResultsDS.tqcResultsRow

            myQCResultsRow = myQCResultsDS.tqcResults.NewtqcResultsRow
            myQCResultsRow.QCTestSampleID = ResultInformationDSAttribute.tResultInformation(0).QCTestSampleID
            myQCResultsRow.QCControlLotID = ResultInformationDSAttribute.tResultInformation(0).QCControlID
            myQCResultsRow.AnalyzerID = ResultInformationDSAttribute.tResultInformation(0).AnalyzerID
            myQCResultsRow.RunsGroupNumber = ResultInformationDSAttribute.tResultInformation(0).RunsGroupNumber
            myQCResultsRow.RunNumber = ResultInformationDSAttribute.tResultInformation(0).NumberOfSeries

            If (LocalValueChange) Then
                myQCResultsRow.ManualResultFlag = True
                myQCResultsRow.ManualResultValue = CSng(bsResultValueTextBox.Text)
            End If

            myQCResultsRow.ResultComment = bsRemarkTextBox.Text.TrimEnd()
            myQCResultsRow.Excluded = bsExcludeResultCheckBox.Checked
            myQCResultsDS.tqcResults.AddtqcResultsRow(myQCResultsRow)

            'Save changes
            Dim myGlobalDataTO As New GlobalDataTO
            Dim myQCResultsDelegate As New QCResultsDelegate

            'myGlobalDataTO = myQCResultsDelegate.UpdateManualResult(Nothing, myQCResultsDS)
            myGlobalDataTO = myQCResultsDelegate.UpdateManualResultNEW(Nothing, myQCResultsDS)
            If (myGlobalDataTO.HasError) Then
                'Error saving the changes; shown it
                myResult = False
                ShowMessage(Me.Name & ".SaveChanges ", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage)
            End If

        Catch ex As Exception
            myResult = False
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".SaveChanges ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".SaveChanges ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
        Return myResult
    End Function

    ''' <summary>
    ''' Set the background color for read-only screen controls
    ''' </summary>
    ''' <remarks>
    ''' Created by: TR 07/06/2011
    ''' </remarks>
    Private Sub SetControlsBackGround()
        Try
            bsTestSampleTypeTextBox.BackColor = Color.Gainsboro
            bsControlNameTextBox.BackColor = Color.Gainsboro
            bsLotNumberTextBox.BackColor = Color.Gainsboro
            bsRunNumberTextBox.BackColor = Color.Gainsboro
            bsMeasureUnitLabel.BackColor = Color.Gainsboro
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".SetControlsBackGround ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".SetControlsBackGround ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    Private Sub ReleaseElements()

        Try
            '--- Detach variable defined using WithEvents ---
            bsResultEditionGroupBox = Nothing
            bsExitButton = Nothing
            bsAcceptButton = Nothing
            bsLotNumberLabel = Nothing
            bsLotNumberTextBox = Nothing
            bsTestSampleTypeTextBox = Nothing
            bsTestSampleTypeLabel = Nothing
            bsResultEditionLabel = Nothing
            bsMeasureUnitLabel = Nothing
            bsResultValueTextBox = Nothing
            bsResultLabel = Nothing
            bsRunNumberTextBox = Nothing
            bsControlNameTextBox = Nothing
            bsControlLabel = Nothing
            bsRemarkTextBox = Nothing
            bsExcludeResultCheckBox = Nothing
            bsRemarksLabel = Nothing
            myToolTipsControl = Nothing
            myErrorProvider = Nothing
            '-----------------------------------------------
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ReleaseElements ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ReleaseElements ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try

    End Sub

#End Region

#Region "Events"
    '*************************
    '* EVENTS FOR THE SCREEN *
    '*************************
    Private Sub IQCResultsEditionAux_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles MyBase.KeyDown
        Try
            If (e.KeyCode = Keys.Escape) Then bsExitButton.PerformClick()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".IQCResultsEditionAux_KeyDown ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".IQCResultsEditionAux_KeyDown ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub IQCResultsEditionAux_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try
            InitializeScreen()
            BindControls()

            Dim mySize As Size = IAx00MainMDI.Size
            Dim myLocation As Point = IAx00MainMDI.Location

            If (Not Me.MdiParent Is Nothing) Then
                mySize = Me.Parent.Size
                myLocation = Me.Parent.Location
            End If

            myNewLocation = New Point(myLocation.X + CInt((mySize.Width - Me.Width) / 2), myLocation.Y + CInt((mySize.Height - Me.Height) / 2))
            Me.Location = myNewLocation
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".IAddManualQCResultsAux_Load ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".IAddManualQCResultsAux_Load ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Not allow move form and mantain the center location in center parent
    ''' </summary>
    Protected Overrides Sub WndProc(ByRef m As Message)
        Try
            If (m.Msg = WM_WINDOWPOSCHANGING) Then
                Dim pos As WINDOWPOS = DirectCast(Runtime.InteropServices.Marshal.PtrToStructure(m.LParam, GetType(WINDOWPOS)), WINDOWPOS)

                Dim mySize As Size = IAx00MainMDI.Size
                Dim myLocation As Point = IAx00MainMDI.Location
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
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WndProc " & Me.Name, EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & "WndProc", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub IQCResultsEditionAux_Shown(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Shown
        Try
            bsResultValueTextBox.Focus()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".QCResultsEditionAux_Shown ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".QCResultsEditionAux_Shown ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    '**********************************
    '* EVENTS FOR THE SCREEN CONTROLS *
    '**********************************
    Private Sub ResultValueTextBox_KeyPress(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles bsResultValueTextBox.KeyPress
        Try
            If (e.KeyChar = "." Or e.KeyChar = ",") Then
                e.KeyChar = CChar(SystemInfoManager.OSDecimalSeparator)
                If (CType(sender, BSTextBox).Text.Contains(".") Or CType(sender, BSTextBox).Text.Contains(",")) Then
                    e.Handled = True
                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ResultValueTextBox_KeyPress ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ResultValueTextBox_KeyPress ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub ResultValueTextBox_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles bsResultValueTextBox.Validating
        Try
            myErrorProvider.Clear()
            If (bsResultValueTextBox.Text.TrimEnd() = String.Empty) Then
                bsResultValueTextBox.Focus()
                myErrorProvider.SetError(bsResultValueTextBox, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString))
                bsAcceptButton.Enabled = False
            Else
                bsAcceptButton.Enabled = True
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ResultValueTextBox_Validating ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ResultValueTextBox_Validating ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub ValueChange_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsResultValueTextBox.TextChanged, _
                                                                                                            bsRemarkTextBox.TextChanged, _
                                                                                                            bsExcludeResultCheckBox.CheckStateChanged
        Try
            LocalChange = True
            If (sender.GetType.Name = "BSTextBox" AndAlso DirectCast(sender, BSTextBox).Name = "bsResultValueTextBox") Then
                LocalValueChange = True
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ValueChange_TextChanged ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ValueChange_TextChanged ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    '*****************************
    '* EVENTS FOR SCREEN BUTTONS *
    '*****************************
    Private Sub AcceptButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsAcceptButton.Click
        Try
            If (LocalChange) Then
                If (SaveChanges()) Then Me.Close()
            Else
                Me.Close()
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".AcceptButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".AcceptButton_Click ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    Private Sub ExitButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsExitButton.Click
        Try
            If (LocalChange) Then
                If (ShowMessage(Me.Name & ".ExitButton_Click ", GlobalEnumerates.Messages.DISCARD_PENDING_CHANGES.ToString, , Me) = Windows.Forms.DialogResult.Yes) Then
                    Me.Close()
                End If
            Else
                Me.Close()
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ExitButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ExitButton_Click ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub
#End Region
End Class
