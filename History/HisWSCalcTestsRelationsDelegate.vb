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

        ''' <summary>
        ''' For all selected Calculated Tests with PrintExpTests = False, remove from the list of selected Historic Order Tests to print all Tests included in 
        ''' their Formulas. If a Test is shared for two or more selected Calculated Tests and at least one of them has PrintExpTests = True, the Test will be 
        ''' printed.
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pHisResultsDataTable">List of rows from typed DataSet HisWSResultsDS (subtable vhisWSResults) containing all rows selected for 
        '''                                    printing in the grid of Historic Patient Results</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <returns>GlobalDataTO containing a Boolean value informed in the following way: 
        '''          ** TRUE: if at least one of the selected Historic Order Tests has to be removed from the list of results to print (due to it is included
        '''                   in the Formula of a Calculated Test with flag PrintExpTests = False). Historic Order Tests to remove are marked in the entry list
        '''                   with field ExpTestToPrint = False
        '''          ** FALSE: if there are not changes in the entry list</returns>
        ''' <remarks>
        ''' Created by:  SA 02/09/2014 - BA-1868
        ''' Modified by: SA 09/09/2014 - BA-1868 ==> Changes to solve errors found in the unit test
        ''' </remarks>
        Public Function MarkExcludedExpTests(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pHisResultsDataTable As List(Of HisWSResultsDS.vhisWSResultsRow), _
                                             ByVal pAnalyzerID As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim testsWereExcluded As Boolean = False
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Get the list of all selected Calculated Tests
                        Dim lstCalculatedOT As List(Of Integer) = (From a In pHisResultsDataTable _
                                                                 Where a.TestType = "CALC" _
                                                                Select a.HistOrderTestID).ToList

                        If (lstCalculatedOT.Count > 0) Then
                            'Get the list of Experimental Tests included in the list of Calculated Tests and value of flag PrintExpTests for each one of them
                            Dim myResultData As New GlobalDataTO()
                            Dim myWSCalcTestsRelationsDAO As New thisWSCalcTestsRelationsDAO

                            myResultData = myWSCalcTestsRelationsDAO.GetExpTestsToExclude(dbConnection, lstCalculatedOT, pAnalyzerID)
                            If (Not myResultData.HasError AndAlso Not myResultData.SetDatos Is Nothing) Then
                                Dim myHisWSCalcTestRelationsDS As HisWSCalcTestRelations = DirectCast(myResultData.SetDatos, HisWSCalcTestRelations)

                                'Get the list of HistOrderTestIDs of Experimental Tests that have to be INCLUDED in reports
                                Dim lstExpToPrint As List(Of HisWSCalcTestRelations.thisWSCalcTestsRelationsRow) = (From a As HisWSCalcTestRelations.thisWSCalcTestsRelationsRow In myHisWSCalcTestRelationsDS.thisWSCalcTestsRelations _
                                                                                                                    Where a.PrintExpTests = True _
                                                                                                                   Select a).ToList()

                                Dim lstSearch As List(Of HisWSCalcTestRelations.thisWSCalcTestsRelationsRow)
                                For Each row As HisWSCalcTestRelations.thisWSCalcTestsRelationsRow In lstExpToPrint
                                    'Verify if the Experimental Test to Print is in the formula of a Calculated Tests that is part of another Calculated Test with PrintExpTests = False
                                    'Note to understand this case:
                                    ' ** ALBUMIN/GLOBULIN has PrintExpTests = False, which means that component tests ALBUMIN and GLOBULIN will not be printed
                                    ' ** GLOBULIN (=PROTEIN TOTAL - ALBUMIN) has PrintExpTests = True, which means that component tests ALBUMIN and PROTEIN TOTAL should be printed;
                                    '    but, due to the GLOBULIN will be not printed, then ALBUMIN and PROTEIN TOTAL will not be printed either
                                    lstSearch = (From a As HisWSCalcTestRelations.thisWSCalcTestsRelationsRow In myHisWSCalcTestRelationsDS.thisWSCalcTestsRelations _
                                                Where a.AnalyzerID = row.AnalyzerID _
                                              AndAlso a.WorkSessionID = row.WorkSessionID _
                                              AndAlso a.HistOrderTestID = row.HistOrderTestIDCALC _
                                              AndAlso a.PrintExpTests = False
                                               Select a).ToList()

                                    'If the Calculated Test has not to be printed, then the component Test has not to be printed
                                    If (lstSearch.Count > 0) Then row.PrintExpTests = False
                                Next

                                'For each Order Test selected to print, calculate the correct value for flag ExpTestToPrint
                                For Each row As HisWSResultsDS.vhisWSResultsRow In pHisResultsDataTable
                                    'Verify if the Test is included in a selected Calculated Test
                                    lstSearch = (From b As HisWSCalcTestRelations.thisWSCalcTestsRelationsRow In myHisWSCalcTestRelationsDS.thisWSCalcTestsRelations _
                                                Where b.AnalyzerID = row.AnalyzerID _
                                              AndAlso b.WorkSessionID = row.WorkSessionID _
                                              AndAlso b.HistOrderTestID = row.HistOrderTestID _
                                               Select b).ToList()

                                    If (lstSearch.Count = 0) Then
                                        'The selected Test is not included in the formula of a selected Calculated Test ==> it will be printed
                                        row.ExpTestToPrint = True

                                    ElseIf (lstSearch.Count = 1) Then
                                        'The selected Test is included in the formula of a selected Calculated Test (only one) ==> it will be printed or not 
                                        'according value of the PrintExpTests flag for the Calculated Test
                                        row.ExpTestToPrint = lstSearch.First.PrintExpTests

                                    Else
                                        'The selected Test is included in the formula of several selected Calculated Tests ==> it will be printed if flag PrintExpTests is True
                                        'for at least one of the Calculated Tests
                                        If ((From c As HisWSCalcTestRelations.thisWSCalcTestsRelationsRow In lstSearch Where c.PrintExpTests = True).ToList.Count = 0) Then
                                            'All Calculated Tests have PrintExpTests = False; the component Test will not be printed
                                            row.ExpTestToPrint = False
                                        Else
                                            'At least one of the Calculated Tests has PrintExpTests = True: the component Test will be printed
                                            row.ExpTestToPrint = True
                                        End If
                                    End If

                                    'If at least one of the selected Order Tests was marked with ExpTestsToPrint = False, the function will return TRUE
                                    If (Not testsWereExcluded And Not row.ExpTestToPrint) Then testsWereExcluded = True
                                Next
                                lstSearch = Nothing

                            End If
                        End If
                        lstCalculatedOT = Nothing

                        'Finally, return the Boolean value testsWereExcluded inside the GlobalDataTO
                        resultData.SetDatos = testsWereExcluded
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "HisWSCalcTestsRelationsDelegate.MarkExcludedExpTests", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
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