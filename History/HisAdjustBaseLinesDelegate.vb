Option Strict On
Option Explicit On

Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.DAL.DAO

Namespace Biosystems.Ax00.BL
    Public Class HisAdjustBaseLinesDelegate

#Region "Public Methods"
        ''' <summary>
        ''' Get the identifier of the last Adjustment BaseLine moved to Historic for the specified Analyzer 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <returns>GlobalDataTO containing an integer value with the identifier of the last Adjustment BaseLine moved to Historic for
        '''          the specified Analyzer</returns>
        ''' <remarks>
        ''' Created by:  SA 26/09/2012
        ''' </remarks>
        Public Function GetLastHistAdjustBaseLine(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New thisAdjustBaseLinesDAO
                        resultData = myDAO.GetLastHistAdjustBaseLine(dbConnection, pAnalyzerID)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "HisAdjustBaseLinesDelegate.GetLastHistAdjustBaseLine", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get the next Adjust Base Line Identifier for the specified Analyzer (by getting the last generated 
        ''' and adding one to it)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <returns>GlobalDataTO containing an integer value with the ID for the next Adjustment BaseLine to create
        '''          in Historic Module for the specified AnalyzerID</returns>
        ''' <remarks>
        ''' Created by:  TR 22/06/2012
        ''' Modified by: SA 30/08/2012 - Added parameter to inform the AnalyzerID; changed the DAO Class used
        ''' </remarks>
        Public Function GetNextAdjustBaseLineID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New thisAdjustBaseLinesDAO
                        resultData = myDAO.GenerateNextAdjustBaseLineID(dbConnection, pAnalyzerID)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "HisAdjustBaseLinesDelegate.GetNextAdjustBaseLineID", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' For the specified AnalyzerID and AdjustBaseLineID, get data of all WaveLengths and move all of them to Historic
        ''' Module, using the previously generated next HistAdjustBaseLineID
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pAdjustBaseLineID">Identifier of the Adjustment BaseLine in WorkSessions Module</param>
        ''' <param name="pHistAdjustBaseLineID">Identifier of the Adjustment BaseLine generated to add it to Historic Module</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  TR 22/06/2012
        ''' Modified by: SA 30/08/2012 - Changed parameter names; changed the DAO Class used
        ''' </remarks>
        Public Function InsertNewBaseLines(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pAdjustBaseLineID As Integer, _
                                           ByVal pHistAdjustBaseLineID As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myHisWSBaseLinesDAO As New thisAdjustBaseLinesDAO
                        resultData = myHisWSBaseLinesDAO.InsertNewBaseLines(dbConnection, pAnalyzerID, pAdjustBaseLineID, pHistAdjustBaseLineID)

                        If (Not resultData.HasError) Then
                            'When the Database Connection was opened locally, then the Commit is executed
                            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                        Else
                            'When the Database Connection was opened locally, then the Rollback is executed
                            If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                        End If
                    End If
                End If
            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "HisAdjustBaseLinesDelegate.InsertNewBaseLines", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function
#End Region

#Region "NOT USED"
        ' ''' <summary>
        ' ''' For the specified Analyzer, delete all Adjust Base Lines not linked to any Execution
        ' ''' </summary>
        ' ''' <param name="pDBConnection">Open DB Connection</param>
        ' ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ' ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ' ''' <remarks>
        ' ''' Created by:  SG 02/07/2013
        ' ''' </remarks>
        'Public Function DeleteNotInUseAdjustBL(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String) As GlobalDataTO
        '    Dim resultData As GlobalDataTO = Nothing
        '    Dim dbConnection As SqlClient.SqlConnection = Nothing

        '    Try
        '        resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
        '        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
        '            dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim myDao As New thisAdjustBaseLinesDAO
        '                resultData = myDao.DeleteNotInUseAdjustBL(dbConnection, pAnalyzerID)

        '                If (Not resultData.HasError) Then
        '                    'When the Database Connection was opened locally, then the Commit is executed
        '                    If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
        '                Else
        '                    'When the Database Connection was opened locally, then the Rollback is executed
        '                    If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
        '                End If
        '            End If
        '        End If

        '    Catch ex As Exception
        '        'When the Database Connection was opened locally, then the Rollback is executed
        '        If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
        '        resultData = New GlobalDataTO()
        '        resultData.HasError = True
        '        resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
        '        resultData.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, "HisAdjustBaseLinesDelegate.InsertNewBaseLines", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return resultData
        'End Function
#End Region

    End Class
End Namespace
