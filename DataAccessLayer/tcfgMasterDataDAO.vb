Option Strict On
Option Explicit On

Imports System.Data.SqlClient
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global

Namespace Biosystems.Ax00.DAL.DAO

    Partial Public Class tcfgMasterDataDAO
        Inherits DAOBase

#Region "CRUD"
        ''' <summary>
        ''' Get all values of the specified Sub Table in MasterData table
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pSubTableID">Unique identifier of the Sub Table of the Master Data from which the data will be obtained</param>
        ''' <returns>GlobalDataTO containing a typed DataSet MasterDataDS with all items in the specified SubTable</returns>
        ''' <remarks>
        ''' Created by:
        ''' Modified by: GDS 13/05/2010 - Added Order By Position field
        '''              DL  08/10/2010 - Changed the query to implement multilanguage
        '''              SA  26/01/2012 - Changed the function template
        '''              AG 21/02/2014 - #1516 add line: " AND    MultiLanguageFlag = 1 "  vbCrLf  _
        ''' </remarks>
        Public Function ReadBySubTableID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pSubTableID As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim var As New GlobalBase
                        Dim cmdText As String = " SELECT MD.SubTableID, MD.ItemID, MR.ResourceText AS FixedItemDesc, MD.Position, MD.Status " & vbCrLf & _
                                                " FROM   tcfgMasterData MD INNER JOIN tfmwMultiLanguageResources MR ON MD.ResourceID = MR.ResourceID " & vbCrLf & _
                                                                                " AND MR.LanguageID = '" & var.GetSessionInfo.ApplicationLanguage & "' " & vbCrLf & _
                                                " WHERE  MD.SubTableID = '" & pSubTableID & "' " & vbCrLf & _
                                                " AND    MultiLanguageFlag = 1 " & vbCrLf & _
                                                " AND    MD.Status = 1 " & vbCrLf & _
                                                " UNION " & vbCrLf & _
                                                " SELECT SubTableID, ItemID, FixedItemDesc, Position, Status " & vbCrLf & _
                                                " FROM   tcfgMasterData " & vbCrLf & _
                                                " WHERE  SubTableID = '" & pSubTableID & "'" & vbCrLf & _
                                                " AND    MultiLanguageFlag = 0 " & vbCrLf & _
                                                " AND    Status = 1 " & vbCrLf & _
                                                " ORDER BY Position " & vbCrLf

                        Dim masterData As New MasterDataDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(masterData.tcfgMasterData)
                            End Using
                        End Using

                        resultData.SetDatos = masterData
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "tcfgMasterDataDAO.ReadBySubTableID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Update the LIS Value by the Subtable ID and the Item ID.
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pSubtableID">Sub table id</param>
        ''' <param name="pItemID">Item ID.</param>
        ''' <param name="pLISValue">LIS Value</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' CREATED BY: TR 04/03/2013
        ''' </remarks>
        Public Function UpdateLISValueBySubTableIDAndItemID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pSubtableID As String, _
                                                    pItemID As String, pLISValue As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = ""
                    Dim currentSession As New GlobalBase

                    cmdText &= "UPDATE tcfgMasterData " & vbCrLf
                    cmdText &= "   SET LISValue = N'" & pLISValue & "'" & vbCrLf
                    cmdText &= " , TS_User = N'" & currentSession.GetSessionInfo().UserName.Replace("'", "''") & "'" & vbCrLf
                    cmdText &= " , TS_DateTime = '" & Now.ToString("yyyyMMdd HH:mm:ss") & "' " & vbCrLf

                    cmdText &= " WHERE SubTableID = '" & pSubtableID & "'" & vbCrLf
                    cmdText &= " AND ItemID = '" & pItemID & "'" & vbCrLf

                    'Execute the SQL Sentence
                    Dim dbCmd As New SqlCommand() With {.Connection = pDBConnection, .CommandText = cmdText}

                    resultData.AffectedRecords = dbCmd.ExecuteNonQuery
                    If (resultData.AffectedRecords = 1) Then
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "tparOffSystemTestsDAO.UpdatepdateLISValueByTestID", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

#End Region

    End Class
End Namespace

