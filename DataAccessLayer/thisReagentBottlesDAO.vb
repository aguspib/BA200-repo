Option Strict On
Option Explicit On

Imports System.Data.SqlClient
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global

Namespace Biosystems.Ax00.DAL.DAO


    Partial Public Class thisReagentBottlesDAO
          



#Region "C R U D"

        ''' <summary>
        ''' Create a new record.
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pHistoryReagentBottlesDS"></param>
        ''' <returns>On the global dataset the affected record result.</returns>
        ''' <remarks>
        ''' CREATED BY: DL 03/08/2011
        ''' Modified BY: TR 08/06/2012 implement Using.
        ''' Modified BY: JV 09/01/2014 #1443 new field Status.
        ''' </remarks>
        Public Function Create(ByVal pDBConnection As SqlConnection, ByVal pHistoryReagentBottlesDS As HisReagentsBottlesDS) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    'There is not an Opened Database Connection...
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString

                ElseIf (Not pHistoryReagentBottlesDS Is Nothing) Then
                    Dim cmdText As String = ""

                    For Each HistoryReagentBottlesRow As HisReagentsBottlesDS.thisReagentsBottlesRow In pHistoryReagentBottlesDS.thisReagentsBottles
                        cmdText = "INSERT INTO thisReagentsBottles"
                        cmdText &= " (CodeTest, BottleType, ReagentNumber, LotNumber, BottleNumber, BarCode, ExpirationDate, BottleVolume, BottleStatus, Status"
                        cmdText &= ") VALUES ("

                        'cmdText &= HistoryReagentBottlesRow.CodeTest                  'Required
                        'cmdText &= ", '" & HistoryReagentBottlesRow.BottleType & "'"  'Required
                        'cmdText &= ", " & HistoryReagentBottlesRow.ReagentNumber      'Required
                        'cmdText &= ", '" & HistoryReagentBottlesRow.LotNumber & "'"   'Required
                        'cmdText &= ", " & HistoryReagentBottlesRow.BottleNumber       'Required
                        'TR 13/06/2012 -Set null to previous values they are not in use now.
                        cmdText &= "NULL"
                        cmdText &= ", NULL"
                        cmdText &= ", NULL"
                        cmdText &= ", NULL"
                        cmdText &= ", NULL"
                        'TR 13/06/2012 -END.
                        cmdText &= ", '" & HistoryReagentBottlesRow.Barcode & "'"     'Required

                        If Not HistoryReagentBottlesRow.IsExpirationDateNull Then
                            cmdText &= ", '" & HistoryReagentBottlesRow.ExpirationDate.ToString("yyyyMMdd HH:mm:ss") & "'"
                        Else
                            cmdText &= ", NULL"
                        End If

                        If Not HistoryReagentBottlesRow.IsBottleVolumeNull Then
                            cmdText &= ", " & HistoryReagentBottlesRow.BottleVolume.ToSQLString()
                        Else
                            cmdText &= ", NULL"
                        End If

                        If Not HistoryReagentBottlesRow.IsBottleStatusNull Then
                            cmdText &= ", '" & HistoryReagentBottlesRow.BottleStatus & "'"
                        Else
                            cmdText &= ", NULL"
                        End If

                        If Not HistoryReagentBottlesRow.IsStatusNull Then
                            cmdText &= ", '" & HistoryReagentBottlesRow.Status & "'"
                        Else
                            cmdText &= ", NULL"
                        End If

                        cmdText &= ")" & vbCrLf 'insert line break

                    Next HistoryReagentBottlesRow

                    Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                        myGlobalDataTO.AffectedRecords = dbCmd.ExecuteNonQuery()
                    End Using

                End If

            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparHistoryReagentBottlesDAO.Create", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Read history reagent bottles by primary key
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pHistoryReagentBottlesDS"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' CREATED BY: DL 03/08/2011
        ''' MODIFIED BY: TR 08/06/2012 -Implement the using statement.
        ''' </remarks>
        Public Function Read(ByVal pDBConnection As SqlConnection, ByVal pHistoryReagentBottlesDS As HisReagentsBottlesDS) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = ""
                        cmdText &= "SELECT *" & vbCrLf
                        cmdText &= "  FROM thisReagentsBottles" & vbCrLf
                        cmdText &= "  WHERE CodeTest = " & pHistoryReagentBottlesDS.thisReagentsBottles.First.CodeTest & vbCrLf
                        cmdText &= "  AND BottleType = '" & pHistoryReagentBottlesDS.thisReagentsBottles.First.BottleType & "'" & vbCrLf
                        cmdText &= "  AND ReagentNumber = " & pHistoryReagentBottlesDS.thisReagentsBottles.First.ReagentNumber & vbCrLf
                        cmdText &= "  AND LotNumber = '" & pHistoryReagentBottlesDS.thisReagentsBottles.First.LotNumber & "'" & vbCrLf
                        cmdText &= "  AND BottleNumber = " & pHistoryReagentBottlesDS.thisReagentsBottles.First.BottleNumber


                        Dim myDataDS As New HisReagentsBottlesDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                            dbCmd.Connection = pDBConnection
                            dbCmd.CommandText = cmdText
                            Using dbDataAdapter As New SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myDataDS.thisReagentsBottles)
                            End Using
                        End Using

                        myGlobalDataTO.SetDatos = myDataDS
                        myGlobalDataTO.HasError = False
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparHistoryReagentBottlesDAO.Read", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Update the records by the table primary key.
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pHistoryReagentBottlesDS"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' CREATED BY: DL 03/08/2011
        ''' Modified BY: JV 09/01/2014 #1443 new field Status.
        ''' </remarks>
        Public Function Update(ByVal pDBConnection As SqlConnection, ByVal pHistoryReagentBottlesDS As HisReagentsBottlesDS) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    'There is not an Opened Database Connection...
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString

                ElseIf (Not pHistoryReagentBottlesDS Is Nothing) Then
                    Dim cmdText As String = ""
                    Dim dbCmd As New SqlClient.SqlCommand
                    dbCmd.Connection = pDBConnection

                    For Each HistoryReagentBottlesRow As HisReagentsBottlesDS.thisReagentsBottlesRow In _
                                                                    pHistoryReagentBottlesDS.thisReagentsBottles.Rows
                        cmdText &= " UPDATE thisReagentsBottles "
                        cmdText &= " SET "

                        If Not HistoryReagentBottlesRow.IsExpirationDateNull Then
                            cmdText &= " ExpirationDate = '" & HistoryReagentBottlesRow.ExpirationDate.ToString("yyyyMMdd HH:mm:ss") & "'"
                        Else
                            cmdText &= " ExpirationDate = ''"
                        End If

                        If Not HistoryReagentBottlesRow.IsBottleVolumeNull Then
                            cmdText &= ", BottleVolume = " & ReplaceNumericString(HistoryReagentBottlesRow.BottleVolume)
                        Else
                            cmdText &= ", BottleVolume = NULL"
                        End If

                        If Not HistoryReagentBottlesRow.IsBottleStatusNull Then
                            cmdText &= ", BottleStatus = '" & HistoryReagentBottlesRow.BottleStatus & "'"
                        Else
                            cmdText &= ", BottleStatus = NULL"
                        End If

                        If Not HistoryReagentBottlesRow.IsStatusNull Then
                            cmdText &= ", Status = '" & HistoryReagentBottlesRow.Status & "'"
                        Else
                            cmdText &= ", Status = NULL"
                        End If

                        cmdText &= " WHERE Barcode = '" & HistoryReagentBottlesRow.Barcode & "'"

                        cmdText &= String.Format("{0}", vbNewLine) 'insert line break

                    Next HistoryReagentBottlesRow

                    dbCmd.CommandText = cmdText
                    myGlobalDataTO.AffectedRecords = dbCmd.ExecuteNonQuery()
                End If

            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparHistoryReagentBottlesDAO.Update", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Read history reagent bottles by the Barcode value
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pBarcode"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' CREATED BY: TR 11/06/2012
        ''' </remarks>
        Public Function ReadByBarCode(ByVal pDBConnection As SqlConnection, ByVal pBarcode As String) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = ""
                        cmdText &= "SELECT * " & vbCrLf
                        cmdText &= "  FROM thisReagentsBottles" & vbCrLf
                        cmdText &= "  WHERE BarCode = '" & pBarcode & "'"

                        Dim myDataDS As New HisReagentsBottlesDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                            dbCmd.Connection = pDBConnection
                            dbCmd.CommandText = cmdText
                            Using dbDataAdapter As New SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myDataDS.thisReagentsBottles)
                            End Using
                        End Using

                        myGlobalDataTO.SetDatos = myDataDS
                        myGlobalDataTO.HasError = False
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparHistoryReagentBottlesDAO.ReadByBarCode", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

#End Region



    End Class

End Namespace
