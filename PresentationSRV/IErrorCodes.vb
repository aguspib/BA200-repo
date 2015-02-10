Option Explicit On
Option Strict On

Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Global.GlobalEnumerates
Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.App

Public Class UiErrorCodes
    Inherits PesentationLayer.BSAdjustmentBaseForm

#Region "Declarations"
    ' Language
    Private currentLanguage As String
#End Region

#Region "Private Methods"
    ''' <summary>
    ''' Loads the icons and tooltips used for painting the buttons
    ''' </summary>
    ''' <remarks>XBC 07/11/2012</remarks>
    Private Sub PrepareButtons()
        Try
            MyClass.SetButtonImage(bsClearButton, "RESETFIELD")
            MyClass.SetButtonImage(bsExitButton, "CANCEL")

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareButtons", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareButtons", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Get texts in the current application language for all screen controls
    ''' </summary>
    ''' <remarks>
    ''' Created by: XBC 07/11/2012
    ''' </remarks>
    Private Sub GetScreenLabels()
        Try
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            Me.BsTitleLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_ALARMS_LIST", currentLanguage)

            ' Tooltips
            GetScreenTooltip()

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Name & ".GetScreenLabels", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".GetScreenLabels", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Get texts in the current application language for all screen controls
    ''' </summary>
    ''' <remarks>
    ''' Created by: XBC 07/11/2012
    ''' </remarks>
    Private Sub GetScreenTooltip()
        Try
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            ' For Tooltips...
            MyBase.bsScreenToolTipsControl.SetToolTip(bsClearButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_SRV_ClearResults", currentLanguage))
            MyBase.bsScreenToolTipsControl.SetToolTip(BsExitButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_CloseScreen", currentLanguage))

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Name & ".GetScreenTooltip ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".GetScreenTooltip ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Get Error codes detected on Analyzer Manager layer and show them on screen
    ''' </summary>
    ''' <remarks>
    ''' Created by: XBC 07/11/2012
    ''' Modified by: IT 23/10/2014 - REFACTORING (BA-2016)
    ''' </remarks>
    Private Sub DisplayErrorCodes()
        Try
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate
            If (AnalyzerController.IsAnalyzerInstantiated) Then
                If AnalyzerController.Instance.Analyzer.ErrorCodesDisplay.Count > 0 Then
                    For i As Integer = 0 To AnalyzerController.Instance.Analyzer.ErrorCodesDisplay.Count - 1
                        Dim newText As String = ""

                        newText += "[" & AnalyzerController.Instance.Analyzer.ErrorCodesDisplay(i).ErrorDateTime.ToString & "]"
                        If AnalyzerController.Instance.Analyzer.ErrorCodesDisplay(i).ErrorCode <> "-1" Then
                            newText += " " & myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_ERROR", currentLanguage)
                            newText += " : " & AnalyzerController.Instance.Analyzer.ErrorCodesDisplay(i).ErrorCode.ToString
                        End If
                        newText += " (" & myMultiLangResourcesDelegate.GetResourceText(Nothing, AnalyzerController.Instance.Analyzer.ErrorCodesDisplay(i).ResourceID, currentLanguage) & ")"
                        If AnalyzerController.Instance.Analyzer.ErrorCodesDisplay(i).Solved Then
                            newText += " - " & UCase(myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SOLVED", currentLanguage))
                        End If
                        newText += vbCrLf

                        BsRichTextBox1.AppendText(newText)
                    Next
                End If
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Name & ".DisplayErrorCodes", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".DisplayErrorCodes", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

#End Region

#Region "Events"
    Private Sub IErrorCodes_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            'Get the current Language from the current Application Session
            'Dim currentLanguageGlobal As New GlobalBase
            currentLanguage = GlobalBase.GetSessionInfo().ApplicationLanguage.Trim.ToString

            'Load the multilanguage texts for all Screen Labels and get Icons for graphical Buttons
            GetScreenLabels()

            PrepareButtons()

            DisplayErrorCodes()

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".Load ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".Load ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' When the  ESC key is pressed, the screen is closed 
    ''' </summary>
    ''' <remarks>
    ''' Created by XBC 07/11/2012
    ''' </remarks>
    Private Sub ConfigUsers_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyDown
        Try
            If (e.KeyCode = Keys.Escape) Then
                'If (Me.BsExitButton.Enabled) Then
                '    Me.Close()
                'End If

                'RH 04/07/2011 Escape key should do exactly the same operations as bsExitButton_Click()
                bsExitButton.PerformClick()
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".KeyDown ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".KeyDown", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub


    Private Sub bsClearButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsClearButton.Click
        Try
            Dim myGlobal As New GlobalDataTO
            If (AnalyzerController.IsAnalyzerInstantiated) Then '#REFACTORING
                myGlobal = AnalyzerController.Instance.Analyzer.RemoveErrorCodesToDisplay()

                If Not myGlobal.HasError Then
                    Me.BsRichTextBox1.Clear()
                Else
                    GlobalBase.CreateLogActivity("Error when remove Error Codes List", Me.Name & ".bsClearButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
                    ShowMessage(Me.Name & ".bsClearButton_Click ", Messages.SYSTEM_ERROR.ToString, "Error when remove Error Codes List", Me)
                End If
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".bsClearButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsClearButton_Click ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub bsExitButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsExitButton.Click
        Try
            Me.Close()
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".bsExitButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsExitButton_Click ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub
#End Region

End Class
