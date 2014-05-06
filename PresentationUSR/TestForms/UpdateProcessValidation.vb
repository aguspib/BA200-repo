Option Strict On
Option Explicit On

Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.BL.UpdateVersion

Public Class UpdateProcessValidation

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>
    ''' CREATED BY: TR 23/01/2013
    ''' </remarks>
    Private Function ExecuteValidationScript() As GlobalDataTO
        Dim myGlobalDataTO As New GlobalDataTO
        Try
            Dim myTestParametersUpdateData As New TestParametersUpdateData
            myGlobalDataTO = myTestParametersUpdateData.UpdateFromFactoryUpdates(Nothing)

            If Not myGlobalDataTO.HasError Then
                myGlobalDataTO = myTestParametersUpdateData.UpdateFromFactoryRemoves(Nothing)
            End If

            'Dim myContaminationsUpdateData As New ContaminationsUpdateData
            'myGlobalDataTO = myContaminationsUpdateData.UpdateFromFactoryUpdates(Nothing)
            'If Not myGlobalDataTO.HasError Then

            'End If

        Catch ex As Exception
            myGlobalDataTO.HasError = True
            myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
            myGlobalDataTO.ErrorMessage = ex.Message
        End Try
        Return myGlobalDataTO
    End Function

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        ExecuteValidationScript()
    End Sub
End Class
