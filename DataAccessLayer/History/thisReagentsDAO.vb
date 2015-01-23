Option Explicit On
Option Strict On

Imports System.Text
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global


Namespace Biosystems.Ax00.DAL.DAO

    Public Class thisReagentsDAO
        Inherits DAOBase

#Region "CRUD Methods"

        ''' <summary>
        ''' A Reagent is closed in Historics Module (field ClosedReagent is set to TRUE) in following cases:
        '''  ** When a Test is deleted in Tests Programming Screen, if the linked Reagents exist in the corresponding table in Historics 
        '''     Module and they are not used for another Tests also existing in Historics Module
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pHistReagentID">Reagent Identifier in Historics Module</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 27/03/2012 
        ''' </remarks>
        Public Function CloseReagent(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pHistReagentID As Integer) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString()
                Else
                    Dim cmdText As New StringBuilder
                    cmdText.AppendLine(" UPDATE thisReagents SET ClosedReagent = 1 ")
                    cmdText.AppendFormat(" WHERE HistReagentID = {0} ", pHistReagentID)

                    Using dbCmd As New SqlClient.SqlCommand(cmdText.ToString, pDBConnection)
                        resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                        resultData.HasError = False
                    End Using
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "thisReagentsDAO.CloseReagent", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Add a list of Reagents to the corresponding table in Historics Module
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pHisReagentsDS">Typed DataSet HisReagentsDS containing all Reagents to add</param>
        ''' <returns>GlobalDataTO containing a typed DataSet HisReagentsDS with all created Reagents with the generated HistReagentID</returns>
        ''' <remarks>
        ''' Created by: TR 27/02/2012
        ''' </remarks>
        Public Function Create(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pHisReagentsDS As HisReagentsDS) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString()
                Else
                    Dim cmdText As New StringBuilder
                    Dim newHisReagentID As Integer = -1

                    For Each myHisReagentRow As HisReagentsDS.thisReagentsRow In pHisReagentsDS.thisReagents.Rows
                        cmdText.Append(" INSERT INTO thisReagents (ReagentID, ReagentName, PreloadedReagent, ClosedReagent) ")
                        cmdText.AppendFormat(" VALUES ({0}, N'{1}', '{2}', '{3}') ", myHisReagentRow.ReagentID, myHisReagentRow.ReagentName, _
                                               myHisReagentRow.PreloadedReagent, myHisReagentRow.ClosedReagent)
                        cmdText.Append(" SELECT SCOPE_IDENTITY() ")

                        Using dbCmd As New SqlClient.SqlCommand(cmdText.ToString, pDBConnection)
                            newHisReagentID = CInt(dbCmd.ExecuteScalar())
                            If (newHisReagentID > 0) Then
                                myHisReagentRow.HistReagentID = newHisReagentID
                            End If
                            cmdText.Length = 0 'Instead of using Remove use the Lenght = 0 
                        End Using
                    Next

                    myGlobalDataTO.SetDatos = pHisReagentsDS
                    myGlobalDataTO.HasError = False
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "thisReagentsDAO.Create", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Delete all not in use closed Reagents
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 01/07/2013
        ''' </remarks>
        Public Function DeleteClosedNotInUse(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString()
                Else
                    Dim cmdText As String = " DELETE FROM thisReagents " & vbCrLf & _
                                            " WHERE  ClosedReagent = 1 " & vbCrLf & _
                                            " AND    HistReagentID NOT IN (SELECT HistReagent1ID FROM thisTestSamples) " & vbCrLf & _
                                            " AND    HistReagentID NOT IN (SELECT HistReagent2ID FROM thisTestSamples) " & vbCrLf

                    Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                        resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                        resultData.HasError = False
                    End Using
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "thisReagentsDAO.DeleteClosedNotInUse", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Verify if an open version of the Reagent already exists in Historics Module and in this case, get the its data
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pReagentID">Reagent Identifier in Parameters Programming</param>
        ''' <returns>GlobalDataTO containing a typed DS HisReagentsDS with data of the Reagent</returns>
        ''' <remarks>
        ''' Created by: TR 27/02/2012
        ''' </remarks>
        Public Function ReadByReagentID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pReagentID As Integer) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing
            Try
                myGlobalDataTO = GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As New StringBuilder
                        cmdText.AppendLine(" SELECT * FROM thisReagents ")

                        cmdText.AppendFormat(" WHERE ReagentID = {0} ", pReagentID)
                        cmdText.AppendFormat(" AND   ClosedReagent = 0 ")

                        Dim myHisReagentsDS As New HisReagentsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText.ToString(), dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myHisReagentsDS.thisReagents)
                            End Using
                        End Using

                        myGlobalDataTO.SetDatos = myHisReagentsDS
                        myGlobalDataTO.HasError = False
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "thisReagentsDAO.ReadByReagentID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function
#End Region
    End Class

End Namespace
