Option Explicit On
Option Strict On

Imports System.Data.SqlClient
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.DAL.DAO
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Calculations


Namespace Biosystems.Ax00.BL

    ''' <summary>
    ''' A testing class for the OperateCalculatedTestDelegate class
    ''' </summary>
    ''' <remarks>
    ''' Created by: RH 05/17/2010
    ''' </remarks>
    Public Class TestOperateCalculatedTestDelegate

        ''' <summary>
        ''' The testing method. Run it in debug mode (trace it)
        ''' </summary>
        ''' <remarks>
        ''' Created by: RH 05/17/2010
        ''' </remarks>
        Public Shared Sub RunTest()
            'WARNING: Prepare Work Session and twksOrderCalculatedTests needed data previously in order to run the test

            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Dim operateCalculatedTest As New OperateCalculatedTestDelegate()

            Try
                resultData = DAOBase.GetOpenDBConnection(Nothing)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then

                        'Write down here a valid OrderTestID
                        Dim OrderTestID As Integer = 38859
                        Dim analyzerModel As String = "A400"
                        operateCalculatedTest.AnalyzerID = ""
                        operateCalculatedTest.WorkSessionID = ""

                        'Test method ExecuteCalculatedTest()
                        resultData = operateCalculatedTest.ExecuteCalculatedTest(dbConnection, OrderTestID, False)
                    End If
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = "SYSTEM_ERROR"
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "TestOperateCalculatedTestDelegate.RunTest", EventLogEntryType.Error, False)
            Finally
                If (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try

        End Sub

    End Class

End Namespace