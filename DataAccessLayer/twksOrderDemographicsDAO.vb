
Option Explicit On
Option Strict On

Imports System.Data.SqlClient
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global

Namespace Biosystems.Ax00.DAL.DAO

    Partial Public Class twksOrderDemographicsDAO
          

#Region "CRUD Methods"

        ''' <summary>
        ''' Add values for one or more customized Order Demographics to an Order
        ''' </summary>
        ''' <param name="pDBConnection">Open Database Connection</param>
        ''' <param name="pCustomDemographics">DataSet containing values of the customized 
        '''                                   Order Demographics to add</param>
        ''' <returns>Global Object containing error information or the added data</returns>
        ''' <remarks></remarks>
        Public Function Create(ByVal pDBConnection As SqlClient.SqlConnection, _
                               ByVal pCustomDemographics As OrderDemographicsDS) As GlobalDataTO
            Dim dataToReturn As New GlobalDataTO

            Try
                If (IsNothing(pDBConnection)) Then
                    'There is not an Opened Database Connection...
                    dataToReturn.HasError = True
                    dataToReturn.ErrorCode = "DB_CONNECTION_ERROR"

                ElseIf (Not IsNothing(pCustomDemographics)) Then
                    Dim dbCmd As New SqlCommand
                    dbCmd.Connection = pDBConnection

                    Dim i As Integer = 0
                    Dim cmdText As String = ""
                    Dim noError As Boolean = True

                    Do While (i < pCustomDemographics.twksOrderDemographics.Rows.Count) And (noError)
                        'SQL Sentence to insert the new Order Test
                        cmdText = " INSERT INTO twksOrderDemographics(OrderID, DemographicID, DemographicValue, TS_User, TS_DateTime) " & _
                                  " VALUES('" & pCustomDemographics.twksOrderDemographics(i).OrderID & "', " & _
                                         " '" & pCustomDemographics.twksOrderDemographics(i).DemographicID & "', " & _
                                         " N'" & pCustomDemographics.twksOrderDemographics(i).DemographicValue.Trim & "', " & _
                                         " N'" & pCustomDemographics.twksOrderDemographics(i).TS_User.Trim.Replace("'", "''") & "', " & _
                                         " '" & CType(pCustomDemographics.twksOrderDemographics(i).TS_DateTime.ToString(), DateTime).ToString("yyyyMMdd HH:mm:ss") & "') "

                        'Execute the SQL Sentence
                        dbCmd.CommandText = cmdText

                        'If number of affected records is different from 1, then it was an uncontrolled error
                        noError = (dbCmd.ExecuteNonQuery() = 1)
                        i += 1
                    Loop

                    If (noError) Then
                        dataToReturn.HasError = False
                        dataToReturn.AffectedRecords = i
                        dataToReturn.SetDatos = pCustomDemographics.Clone
                    Else
                        dataToReturn.HasError = True
                        dataToReturn.AffectedRecords = 0
                    End If
                End If
            Catch ex As Exception
                dataToReturn.HasError = True
                dataToReturn.ErrorCode = "SYSTEM_ERROR"
                dataToReturn.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksOrderDemographicsDAO.Create", EventLogEntryType.Error, False)
            End Try
            Return dataToReturn
        End Function
#End Region

#Region "Other Methods"

        ''' <summary>
        ''' Delete all information for the Work Session
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created By: GDS 21/04/2010
        ''' </remarks>
        Public Function ResetWS(ByVal pDBConnection As SqlConnection) As GlobalDataTO

            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = "DB_CONNECTION_ERROR"

                Else
                    Dim cmdText As String
                    cmdText = "DELETE twksOrderDemographics" & vbCrLf & _
                              "  WHERE OrderID NOT IN (SELECT OrderID" & vbCrLf & _
                              "                          FROM twksOrderTests)"

                    Dim cmd As SqlCommand

                    cmd = pDBConnection.CreateCommand
                    cmd.CommandText = cmdText

                    cmd.ExecuteNonQuery()
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = "SYSTEM_ERROR"
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksOrderDemographicsDAO.ResetWS", EventLogEntryType.Error, False)
            End Try

            Return resultData

        End Function

#End Region

    End Class
End Namespace
