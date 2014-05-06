Option Strict On
Option Explicit On

Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.DAL.DAO
Imports Biosystems.Ax00.Types

Namespace Biosystems.Ax00.BL
    Public Class HisWSCalcTestsRelationsDelegate

#Region "Public Methods"
        ''' <summary>
        ''' Create for all results of Calculated Tests in the Analyzer/WorkSession, the relation with the results of the Standard and/or Calculated 
        ''' Tests used to get it
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pHisWSCalcTestsRelationsDS">Typed Dataset HisPreviousBlkCalibUsedDS containing the relations of all Calculated Tests 
        '''                                          requested for Patients in the Analyzer WorkSession</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 14/09/2012
        ''' </remarks>
        Public Function Create(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pHisWSCalcTestsRelationsDS As HisWSCalcTestRelations) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New thisWSCalcTestsRelationsDAO
                        resultData = myDAO.Create(dbConnection, pHisWSCalcTestsRelationsDS)

                        If (Not resultData.HasError) Then
                            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                        Else
                            If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                        End If
                    End If
                End If
            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "HisWSCalcTestsRelationsDelegate.Create", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function
#End Region

#Region "NOT USED"
        ' ''' <summary>
        ' ''' When a result for a Calculated Test is deleted, delete the link between the informed HistOrderTestID and the HistOrderTestID of all Tests 
        ' ''' included in its formula
        ' ''' </summary>
        ' ''' <param name="pDBConnection">Open DB Connection</param>
        ' ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ' ''' <param name="pWorkSessionID">Work Session Identifier in Historic Module</param>
        ' ''' <param name="pHistOrderTestIDCALC">Identifier of the Order Test of a Calculated Test in Historic Module</param>
        ' ''' <returns>GlobalDataTO containing success/error information</returns>
        ' ''' <remarks>
        ' ''' Created by:  SG 02/07/2013
        ' ''' </remarks>
        'Public Function DeleteByHistOrderTestIDCALC(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
        '                                            ByVal pWorkSessionID As String, ByVal pHistOrderTestIDCALC As Integer) As GlobalDataTO
        '    Dim resultData As GlobalDataTO = Nothing
        '    Dim dbConnection As SqlClient.SqlConnection = Nothing

        '    Try
        '        resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
        '        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
        '            dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim myDao As New thisWSCalcTestsRelationsDAO
        '                resultData = myDao.DeleteByHistOrderTestIDCALC(dbConnection, pAnalyzerID, pWorkSessionID, pHistOrderTestIDCALC)

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
        '        myLogAcciones.CreateLogActivity(ex.Message, "HisWSCalcTestsRelationsDelegate.DeleteByHistOrderTestIDCALC", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return resultData
        'End Function

        ' ''' <summary>
        ' ''' Get the HistOrderTestID of all Calculated Tests in which formula the informed HistOrderTestID is included
        ' ''' </summary>
        ' ''' <param name="pDBConnection">Open DB Connection</param>
        ' ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ' ''' <param name="pWorkSessionID">Work Session Identifier in Historic Module</param>
        ' ''' <param name="pHistOrderTestID">Identifier of the Order Test in Historic Module</param>
        ' ''' <returns>GlobalDataTO containing a typed DataSet HisWSCalcTestRelations with the list of HistOrderTestIDs of Calculated
        ' '''          Tests in which formula the informed HistOrderTestID is included</returns>
        ' ''' <remarks>
        ' ''' Created by:  SG 02/07/2013
        ' ''' </remarks>
        'Public Function ReadByHistOrderTestID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
        '                                      ByVal pWorkSessionID As String, ByVal pHistOrderTestID As Integer) As GlobalDataTO
        '    Dim resultData As GlobalDataTO = Nothing
        '    Dim dbConnection As SqlClient.SqlConnection = Nothing

        '    Try
        '        resultData = DAOBase.GetOpenDBConnection(pDBConnection)

        '        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
        '            dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim myDAO As New thisWSCalcTestsRelationsDAO
        '                resultData = myDAO.ReadByHistOrderTestID(dbConnection, pAnalyzerID, pWorkSessionID, pHistOrderTestID)

        '            End If
        '        End If

        '    Catch ex As Exception
        '        resultData = New GlobalDataTO()
        '        resultData.HasError = True
        '        resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
        '        resultData.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, "HisWSCalcTestsRelationsDelegate.ReadByHistOrderTestID", EventLogEntryType.Error, False)

        '    Finally
        '        If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()

        '    End Try

        '    Return resultData
        'End Function
#End Region
    End Class
End Namespace