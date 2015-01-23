Imports Biosystems.Ax00.Global

Imports System.Text
Imports Biosystems.Ax00.Types

Public Class IntegrityValidationForm

    Private Const ResultOK As String = "(OK)"
    Private Const ResultNotOK As String = "(FAIL)"

    ''' <summary>
    ''' Start the Integrity validation process.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>
    ''' CREATED BY: TR 24/01/2013
    ''' </remarks>
    Private Function ExecuteIntegrityValProcess() As GlobalDataTO
        Dim myGlobalDataTO As New GlobalDataTO
        Try
            'Clear all control
            ResultTextBox.Clear()
            ProgressBar1.Minimum = 0
            ProgressBar1.Value = 0

            Dim myDBIntegrityDS As New DBIntegrityDS
            myDBIntegrityDS.ReadXml(Application.StartupPath & "\\Scripts\\DataIntegrity.xml")
            Dim myresults As New StringBuilder


            'Before starting validation check if database version are equal
            Dim installedDBVersion As Single = 0
            Dim myIntegrityDelegate As New IntegrityDelegate
            myGlobalDataTO = myIntegrityDelegate.GetInstalledDBVersion()
            If Not myGlobalDataTO.HasError Then
                'Compare version
                installedDBVersion = CSng(myGlobalDataTO.SetDatos.ToString().ToString().Replace(".", SystemInfoManager.OSDecimalSeparator))
            End If

            If myDBIntegrityDS.IntegrityValidation.Count > 0 Then

                ProgressBar1.Maximum = myDBIntegrityDS.IntegrityValidation.Count
                Dim printedVersion As Boolean = False
                Dim IntegrityHasError As Boolean = False

                For Each IntegrityTestRow As DBIntegrityDS.IntegrityValidationRow In myDBIntegrityDS.IntegrityValidation.Rows
                    If installedDBVersion >= IntegrityTestRow.Version Then
                        ProgressBar1.Increment(1)
                        If Not printedVersion Then
                            myresults.AppendLine("Validated Version - " & installedDBVersion & Environment.NewLine)
                            myresults.AppendLine(String.Empty)
                            printedVersion = True
                        End If
                        'Execute the validation script
                        myGlobalDataTO = ExecuteValidationScript(IntegrityTestRow.Script, IntegrityTestRow.Result)
                        If Not myGlobalDataTO.HasError Then
                            'Validate if show only error is enable.
                            If ShowErrorChkBox.Checked Then
                                If myGlobalDataTO.SetDatos.ToString() = ResultNotOK Then
                                    myresults.AppendLine("Version - " & IntegrityTestRow.Version & Environment.NewLine & "  " & _
                                                         IntegrityTestRow.Label & " ---> " & myGlobalDataTO.SetDatos.ToString() & Environment.NewLine)
                                End If
                            Else
                                myresults.AppendLine("Version - " & IntegrityTestRow.Version & Environment.NewLine & "  " & _
                                                     IntegrityTestRow.Label & " ---> " & myGlobalDataTO.SetDatos.ToString() & Environment.NewLine)
                            End If

                            If myGlobalDataTO.SetDatos.ToString = ResultNotOK AndAlso Not IntegrityHasError Then
                                IntegrityHasError = True
                            End If
                        Else

                            myresults.AppendLine("Version Validated - " & IntegrityTestRow.Version & Environment.NewLine & _
                                                 IntegrityTestRow.Label & "---> (NO OK)  " & myGlobalDataTO.ErrorMessage & Environment.NewLine)
                            IntegrityHasError = True
                        End If
                    Else
                        'Database versions are diferent error
                        myresults.AppendLine("VERSION ERROR:Installed version is different than Integrity validation file")
                        myresults.AppendLine("Installed Database Version ---> " & installedDBVersion)
                        myresults.AppendLine("Validated Version File     ---> " & myDBIntegrityDS.IntegrityValidation.Last().Version)
                        ProgressBar1.Increment(myDBIntegrityDS.IntegrityValidation.Count)
                        IntegrityHasError = True
                        Exit For
                    End If

                Next

                'Show the error result 
                myresults.AppendLine("-------------RESULT-------------")
                If IntegrityHasError Then
                    myresults.AppendLine("Error Found.")
                Else
                    myresults.AppendLine("No Error Found.")
                End If

                myresults.AppendLine("---------------END----------------")

                ResultTextBox.Text = myresults.ToString()
            End If

        Catch ex As Exception
            myGlobalDataTO.HasError = True
            myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
            myGlobalDataTO.ErrorMessage = ex.Message
        End Try
        Return myGlobalDataTO
    End Function

    ''' <summary>
    ''' Execute the validation script and validate the recived result with expecte result
    ''' </summary>
    ''' <param name="pValScript"></param>
    ''' <param name="pResult"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' CREATE BY: TR 25/01/2013
    ''' </remarks>
    Private Function ExecuteValidationScript(pValScript As String, pResult As String) As GlobalDataTO
        Dim myGlobalDataTO As New GlobalDataTO
        Try
            Dim myIntegrityDelegate As New IntegrityDelegate

            myGlobalDataTO = myIntegrityDelegate.ExecuteScripts(pValScript)

            If Not myGlobalDataTO.HasError Then
                If myGlobalDataTO.SetDatos.ToString = pResult Then
                    myGlobalDataTO.SetDatos = ResultOK
                Else
                    myGlobalDataTO.SetDatos = ResultNotOK
                End If
            End If

        Catch ex As Exception
            myGlobalDataTO.HasError = True
            myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
            myGlobalDataTO.ErrorMessage = ex.Message
        End Try

        Return myGlobalDataTO

    End Function

    Private Sub ExecuteTestButton_Click_1(sender As Object, e As EventArgs) Handles ExecuteTestButton.Click
        ExecuteIntegrityValProcess()
    End Sub

    Private Sub CancelButton_Click(sender As Object, e As EventArgs) Handles ButtonCancel.Click
        Close()
    End Sub
End Class
