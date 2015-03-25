Option Strict On
Option Explicit On

Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.DAL


Namespace Biosystems.Ax00.BL
    Public Class ReportsTestsSortingDelegate

#Region "Public Methods"
        ''' <summary>
        ''' Get the default sorting for all Tests selecting them from the correspondent table according its Test Type
        ''' Default sorting is the following one:
        ''' 1- Standard Tests:
        '''    a) Preloaded Test
        '''    b) UserTest
        ''' 2- Calculated Tests
        ''' 3- ISE Test
        ''' 4- OFF Systems Test
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTO containing a typed DataSet ReportsTestsSortingDS with all Tests sorted according
        '''          the default criteria</returns>
        ''' <remarks>
        ''' Created by: TR 25/11/2011
        ''' </remarks>
        Public Function GetDefaultSortedTestList(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myReportsTestsSortingDAO As New tcfgReportsTestsSortingDAO
                        myGlobalDataTO = myReportsTestsSortingDAO.GetDefaultSortedTestList(dbConnection)
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ReportsTestsSortingDelegate.GetDefaultSortedTestList", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get the list of Tests with the current saved sorting order
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTO containing a typed DataSet ReportsTestsSortingDS with all Tests sorted according 
        '''          the current saved criteria</returns>
        ''' <remarks>
        ''' Created by:  TR 24/11/2011
        ''' </remarks>
        Public Function GetSortedTestList(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myReportsTestsSortingDAO As New tcfgReportsTestsSortingDAO
                        myGlobalDataTO = myReportsTestsSortingDAO.GetSortedTestList(dbConnection)
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ReportsTestsSortingDelegate.GetSortedTestList", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Update the position for reports of all Tests 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pReportsTestsSortingDS">Typed DataSet ReportsTestsSortingDS containing all tests to update</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by: TR 24/11/2011
        ''' </remarks>
        Public Function UpdateTestPosition(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pReportsTestsSortingDS As ReportsTestsSortingDS) _
                                           As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myReportsTestSortingDAO As New tcfgReportsTestsSortingDAO
                        myGlobalDataTO = myReportsTestSortingDAO.UpdateTestPosition(dbConnection, pReportsTestsSortingDS)

                        If (Not myGlobalDataTO.HasError) Then
                            'Update GeneralSetting LAST_TEST_REPORT_POS with value of the last TestPosition
                            Dim myLastPosition As Integer = pReportsTestsSortingDS.tcfgReportsTestsSorting(pReportsTestsSortingDS.tcfgReportsTestsSorting.Count - 1).TestPosition

                            myGlobalDataTO = GeneralSettingsDelegate.UpdateCurrValBySettingID(dbConnection, GlobalEnumerates.GeneralSettingsEnum.LAST_TEST_REPORT_POS, myLastPosition.ToString())
                        End If
                    End If

                    If (Not myGlobalDataTO.HasError) Then
                        'When the Database Connection was opened locally, then the Commit is executed
                        If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                    Else
                        'When the Database Connection was opened locally, then the Rollback is executed
                        If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                    End If
                End If

            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                myGlobalDataTO = New GlobalDataTO
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ReportTestsSortingDelegate.UpdateTestPosition", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return (myGlobalDataTO)
        End Function
#End Region
    End Class
End Namespace
