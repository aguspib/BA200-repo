Option Strict On
Option Explicit On

Imports System.Text
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global

Namespace Biosystems.Ax00.DAL.DAO

#Region "Partial twksWSExecutionsDAO Class"
    Partial Public Class TemporalDAO
          

        Public Function Create(ByVal pDBConnection As SqlClient.SqlConnection, _
                              ByVal pTemporalTableDS As TemporalTableDS) As GlobalDataTO

            Dim myGlobalTO As New GlobalDataTO()
            Dim dataToReturn As New GlobalDataTO()
            'Dim cmdText As New StringBuilder()
            Dim dbConnection As New SqlClient.SqlConnection

            Try

                myGlobalTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalTO.HasError And Not myGlobalTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalTO.SetDatos, SqlClient.SqlConnection)

                    Dim i As Integer = 0
                    Dim maxInserts As Integer = 300
                    'Dim dbCmd As New SqlClient.SqlCommand
                    Dim cmdText As New StringBuilder()
                    For Each row As TemporalTableDS.TemporalTableRow In pTemporalTableDS.TemporalTable.Rows
                        cmdText.Append("INSERT INTO TemporalTable")
                        cmdText.Append("(Nombre, Apellido, Fecha) VALUES(")

                        cmdText.AppendFormat("'{0}', '{1}', '{2}'", row.Nombre, row.Apellido, row.Fecha)

                        'Add the final parenthesis and increment the value for the next ExecutionID
                        cmdText.Append(") " & vbCrLf)

                        'Increment the sentences counter and verify if the max has been reached
                        i += 1
                        If (i = maxInserts) Then

                            'TR 04/08/2011 -Implement using 
                            Using dbCmd As New SqlClient.SqlCommand
                                dbCmd.Connection = dbConnection
                                dbCmd.CommandText = cmdText.ToString()
                                'Execute the SQL script 
                                dataToReturn.AffectedRecords = dbCmd.ExecuteNonQuery()
                                If (Not dataToReturn.AffectedRecords > 0) Then
                                    dataToReturn.HasError = True
                                    Exit For
                                End If

                                'Initialize the counter and the StringBuilder
                                i = 0
                                cmdText.Length = 0 'TR 04/08/2011 -Instead of using Remove use the Lenght = 0 
                            End Using

                        End If
                    Next

                    If (Not dataToReturn.HasError) Then
                        If (cmdText.Length > 0) Then
                            'TR 04/08/2011 -Implement using 
                            Using dbCmd As New SqlClient.SqlCommand
                                'Execute the remaining Inserts...
                                dbCmd.Connection = dbConnection
                                dbCmd.CommandText = cmdText.ToString()

                                dataToReturn.AffectedRecords = dbCmd.ExecuteNonQuery()
                                cmdText.Length = 0 'TR 04/08/2011 -Instead of using Remove use the Lenght = 0 
                            End Using
                            'TR 04/08/2011 -END
                            If (Not dataToReturn.AffectedRecords > 0) Then dataToReturn.HasError = True

                        End If
                    End If

                    If Not dataToReturn.HasError Then
                        DAOBase.CommitTransaction(dbConnection)
                    Else

                        DAOBase.RollbackTransaction(dbConnection)
                    End If

                End If
            Catch ex As Exception
                dataToReturn.HasError = True
                dataToReturn.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                dataToReturn.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksWSExecutionsDAO.Create", EventLogEntryType.Error, False)
            End Try
            Return dataToReturn
        End Function


    End Class
#End Region

End Namespace
