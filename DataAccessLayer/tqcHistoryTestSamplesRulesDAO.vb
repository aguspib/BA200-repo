Option Strict On
Option Explicit On

Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global

Namespace Biosystems.Ax00.DAL.DAO

    Public Class tqcHistoryTestSamplesRulesDAO
        Inherits DAOBase

#Region "CRUD Methods"
        ''' <summary>
        ''' Delete all Multirules selected for the informed QCTestSampleID
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pQCTestSampleID">Identifier of Test/SampleType in QC Module</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  TR 10/05/2011
        ''' </remarks>
        Public Function Delete(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCTestSampleID As Integer) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    'There is not an Opened Database Connection...
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = " DELETE FROM tqcHistoryTestSamplesRules " & vbCrLf & _
                                            " WHERE  QCTestSampleID = " & pQCTestSampleID.ToString

                    Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                        myGlobalDataTO.AffectedRecords = dbCmd.ExecuteNonQuery()
                        myGlobalDataTO.HasError = False
                    End Using
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "tqcHistoryTestSamplesRulesDAO.Delete", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' When the selected Multirules are changed for an ISE Test/SampleType in ISE Tests Programming Screen, they are also inserted in 
        ''' the correspondent table in QC Module
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestType">Test Type Code</param>
        ''' <param name="pTestID">Test Identifier</param>
        ''' <param name="pSampleType">Sample Type Code</param>
        ''' <param name="pQCTestSampleID">Identifier of ISE Test/SampleType in QC Module</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  TR 10/05/2011
        ''' Modified by: SA 05/06/2012 - Added parameter for TestType and filter the query for this field
        ''' </remarks>
        Public Function InsertFromTestSampleMultiRulesNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestType As String, ByVal pTestID As Integer, _
                                                          ByVal pSampleType As String, ByVal pQCTestSampleID As Integer) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = " INSERT INTO tqcHistoryTestSamplesRules(QCTestSampleID, RuleID) " & vbCrLf & _
                                            " SELECT " & pQCTestSampleID.ToString() & ", RuleID " & vbCrLf & _
                                            " FROM   tparTestSamplesMultirules " & vbCrLf & _
                                            " WHERE  TestType     = '" & pTestType.Trim & "' " & vbCrLf & _
                                            " AND    TestID       = " & pTestID.ToString() & vbCrLf & _
                                            " AND    SampleType   = '" & pSampleType.Trim & "'" & vbCrLf & _
                                            " AND    SelectedRule = 1 " & vbCrLf

                    Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                        resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                        resultData.HasError = False
                    End Using
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "tqcHistoryTestSamplesRulesDAO.InsertFromISETestSampleMultiRules", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get all Multirules to apply for the specified Test/SampleType
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pQCTestSampleID">Identifier of Test/SampleType in QC Module</param>
        ''' <returns>GlobalDataTO containing a typed DataSet HistoryTestSamplesRulesDS with the list of selected Multirules</returns>
        ''' <remarks>
        '''  Created by:  TR 27/05/2011
        ''' </remarks>
        Public Function ReadByQCTestSampleID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCTestSampleID As Integer) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT QCTestSampleID, RuleID " & vbCrLf & _
                                                " FROM  tqcHistoryTestSamplesRules  " & vbCrLf & _
                                                " WHERE QCTestSampleID = " & pQCTestSampleID & vbCrLf

                        Dim myHistoryTestSamplesRulesDS As New HistoryTestSamplesRulesDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myHistoryTestSamplesRulesDS.tqcHistoryTestSamplesRules)
                            End Using
                        End Using

                        myGlobalDataTO.SetDatos = myHistoryTestSamplesRulesDS
                        myGlobalDataTO.HasError = False
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "tqcHistoryTestSamplesRulesDAO.ReadByQCTestSampleID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function
#End Region

#Region "TO DELETE - OLD FUNCTIONS"
        '''' <summary>
        '''' When the selected Multirules are changed for a Test/SampleType in Tests Programming Screen, they are also inserted in 
        '''' the correspondent table in QC Module
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pTestID">Test Identifier</param>
        '''' <param name="pSampleType">Sample Type Code</param>
        '''' <param name="pQCTestSampleID">Identifier of Test/SampleType in QC Module</param>
        '''' <returns>GlobalDataTO containing success/error information</returns>
        '''' <remarks>
        '''' Created by: TR 10/05/2011
        '''' </remarks>
        'Public Function InsertFromTestSampleMultiRulesOLD(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer, ByVal pSampleType As String, _
        '                                                  ByVal pQCTestSampleID As Integer) As GlobalDataTO
        '    Dim resultData As New GlobalDataTO

        '    Try
        '        If (pDBConnection Is Nothing) Then
        '            resultData.HasError = True
        '            resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
        '        Else
        '            Dim cmdText As String = " INSERT INTO tqcHistoryTestSamplesRules(QCTestSampleID, RuleID) "
        '            cmdText &= " SELECT " & pQCTestSampleID.ToString() & ", RuleID "
        '            cmdText &= " FROM   tparTestSamplesMultirules "
        '            cmdText &= " WHERE  TestID = " & pTestID.ToString()
        '            cmdText &= " AND    SampleType = '" & pSampleType & "'"
        '            cmdText &= " AND    SelectedRule = 1 "

        '            Dim cmd As SqlClient.SqlCommand
        '            cmd = pDBConnection.CreateCommand
        '            cmd.CommandText = cmdText

        '            resultData.AffectedRecords = cmd.ExecuteNonQuery()
        '            resultData.HasError = False
        '        End If
        '    Catch ex As Exception
        '        resultData.HasError = True
        '        resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        resultData.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, "tqcHistoryTestSamplesRulesDAO.InsertFromTestSampleMultiRules", EventLogEntryType.Error, False)
        '    End Try
        '    Return resultData
        'End Function
#End Region

    End Class
End Namespace
