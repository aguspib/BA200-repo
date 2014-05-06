Option Explicit On
Option Strict On

Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.DAL.DAO
Imports System.Configuration

Namespace Biosystems.Ax00.BL
    Public Class TestReagentsDelegate

#Region "Public Methods"
        ''' <summary>
        ''' Create relation between Tests and Reagents
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestReagentsDS">Typed Dataset TestReagentsDS containing the list of pairs Test-Reagent to create</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by:  TR 03/10/2010
        ''' Modified by: SA 07/10/2011 - Changed the function template
        ''' </remarks>
        Public Function Create(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestReagentsDS As TestReagentsDS) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError) AndAlso (Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myTestReagentsDAO As New tparTestReagentsDAO()
                        myGlobalDataTO = myTestReagentsDAO.Create(dbConnection, pTestReagentsDS)

                        If (Not myGlobalDataTO.HasError) Then
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

                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "TestReagentsDelegate.Create", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Delete a relation between a Test and a Reagent
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestID">Test Identifier</param>
        ''' <param name="pReagentID">Reagent Identifier</param>
        ''' <param name="pReagentNumber">Reagent Number</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  TR 15/03/2010
        ''' Modified by: SA 07/10/2011 - Changed the function template
        ''' </remarks>
        Public Function Delete(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer, ByVal pReagentID As Integer, _
                               ByVal pReagentNumber As Integer) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError) AndAlso (Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myTestReagentsDAO As New tparTestReagentsDAO()
                        myGlobalDataTO = myTestReagentsDAO.Delete(dbConnection, pTestID, pReagentID, pReagentNumber)

                        If (Not myGlobalDataTO.HasError) Then
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

                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "TestReagentsDelegate.Delete", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get the list of Reagents required for an specific Test
        ''' </summary>
        ''' <param name="pDBConnection">Open Database Connection</param>
        ''' <param name="pTestID">Test Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet TestReagentsDS with the list of Reagents
        '''          required for the informed Test</returns>
        ''' <remarks>
        ''' Created by:  SA 09/02/2010
        ''' Modified by: SA 07/10/2011 - Changed the function template
        ''' </remarks>
        Public Function GetTestReagents(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myTestReagentsDAO As New tparTestReagentsDAO
                        myGlobalDataTO = myTestReagentsDAO.GetTestReagents(dbConnection, pTestID)

                        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                            If (DirectCast(myGlobalDataTO.SetDatos, TestReagentsDS).tparTestReagents.Rows.Count = 0) Then
                                myGlobalDataTO.HasError = True
                                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.MASTER_DATA_MISSING.ToString
                            End If
                        End If
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "TestReagentsDelegate.GetTestReagents", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get all Tests using the specified Reagent
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pReagentID">Reagent Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet TestReagentsDS</returns>
        ''' <remarks>
        ''' Created by:  TR 18/05/2010
        ''' Modified by: SA 07/10/2011 - Changed the function template
        ''' </remarks>
        Public Function GetTestReagentsByReagentID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pReagentID As Integer) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myTestReagentsDAO As New tparTestReagentsDAO
                        myGlobalDataTO = myTestReagentsDAO.GetTestReagentsByReagentID(dbConnection, pReagentID)

                        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                            If (DirectCast(myGlobalDataTO.SetDatos, TestReagentsDS).tparTestReagents.Rows.Count = 0) Then
                                myGlobalDataTO.HasError = True
                                myGlobalDataTO.ErrorCode = "MASTER_DATA_MISSING"
                            End If
                        End If
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "TestReagentsDelegate.GetTestReagentsByReagentID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get the list of Standard Tests with their correspondent Reagent (R1 or R2, depending on the informed
        ''' Reagent Number)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pReagentNum">Indicates the obtained Reagents will be the ones used as this Reagent number</param>
        ''' <returns>GlobalDataTO containing a typed DataSet ?? with the list of Tests and Reagents</returns>
        ''' <remarks>
        ''' Created by:  SA 30/11/2010
        ''' Modified by: SA 07/10/2011 - Changed the function template
        ''' </remarks>
        Public Function GetByReagentNumber(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pReagentNum As Integer, Optional ByVal pExcludedTestID As Integer = -1) _
                                           As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myTestReagentsDAO As New tparTestReagentsDAO
                        resultData = myTestReagentsDAO.ReadByReagentNumber(dbConnection, pReagentNum, pExcludedTestID)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "TestReagentsDelegate.GetByReagentNumber", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function
#End Region

#Region "TO REVIEW-DELETE"
        ''' <summary>
        ''' USED ONLY FOR THE OLD CONTAMINATIONS FORM
        ''' Get the list of Reagents by Reagent Number; or the list of Tests when the informed Contamination Type
        ''' is CUVETTES
        ''' </summary>
        ''' <param name="pDBConnection">Open Database Connection</param>
        ''' <param name="pContaminationType">Reagent Number (R1 for R1/R3; R2 for R2/R4; CUVETTES)</param>
        ''' <returns>GlobalDataTO containing a typed DataSet TestReagentsDS with the list of
        '''          obtained Reagents or Tests plus the path of the Icon defined for Contaminations</returns>
        ''' <remarks>
        ''' Created by:  DL 12/02/2010
        ''' Modified by: SA 22/02/2010 - Name changed from GetListByPosition to GetListForContaminations. Parameter pContaminationType 
        '''                              changed from integer to string. Return changed from TestReagentsDS to GlobalDataTO
        ''' </remarks>
        Public Function GetListForContaminations(ByVal pDBConnection As SqlClient.SqlConnection, _
                                                 ByVal pContaminationType As String) As GlobalDataTO

            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim testReagentsData As New TestReagentsDS
                        Dim testReagentsDAO As New tparTestReagentsDAO

                        Dim reagentType As Integer = CType(IIf(pContaminationType = "R1", 1, IIf(pContaminationType = "R2", 2, 3)), Integer)
                        resultData = testReagentsDAO.GetListReagentsByType(dbConnection, reagentType)
                        testReagentsData = CType(resultData.SetDatos, TestReagentsDS)

                        'Get the Icon Path in Application Configuration file
                        'Dim iconPath As String = ConfigurationManager.AppSettings("IconsPath").ToString
                        'TR 25/01/2011 -Replace by corresponding value on global base.
                        Dim iconPath As String = GlobalBase.ImagesPath

                        If (iconPath.Trim <> "") Then
                            'Get the Icon Name stored in table of Preloaded Master Data
                            Dim getPreloadedMasterData As New tfmwPreloadedMasterDataDAO
                            Dim auxIconPath As String = getPreloadedMasterData.ReadByItemID("ICON_PATHS", "REAGENTS").tfmwPreloadedMasterData.Rows(0)("FixedItemDesc").ToString()

                            'Build the Icon full path and add it to each Contamination
                            If (Not auxIconPath Is Nothing) And (auxIconPath.Trim <> "") Then
                                iconPath += auxIconPath
                                For Each testReagentsRow As TestReagentsDS.tparTestReagentsRow In testReagentsData.tparTestReagents.Rows
                                    testReagentsRow.BeginEdit()
                                    testReagentsRow.IconPath = iconPath
                                    testReagentsRow.EndEdit()
                                Next testReagentsRow
                            End If
                        End If

                        'Return the list of Reagents or Tests with the Icon Path informed
                        resultData.SetDatos = testReagentsData
                    End If
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "TestReagentsDelegate.GetListForContaminations", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        '''' <summary>
        '''' Update Test Reagents. - NOT USED
        '''' </summary>
        '''' <param name="pDBConnection"></param>
        '''' <param name="pTestReagentsDS"></param>
        '''' <returns></returns>
        '''' <remarks>
        '''' Created by:  TR 03/10/2010
        '''' </remarks>
        'Public Function Update(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestReagentsDS As TestReagentsDS) As GlobalDataTO
        '    Dim myGlobalDataTO As New GlobalDataTO
        '    Dim dbConnection As New SqlClient.SqlConnection
        '    Try
        '        myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
        '        If (Not myGlobalDataTO.HasError) And (Not myGlobalDataTO.SetDatos Is Nothing) Then
        '            dbConnection = CType(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim myTestReagentsDAO As New tparTestReagentsDAO()
        '                myGlobalDataTO = myTestReagentsDAO.Update(dbConnection, pTestReagentsDS)

        '                If (Not myGlobalDataTO.HasError) Then
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
        '        If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

        '        myGlobalDataTO.HasError = True
        '        myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myGlobalDataTO.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, "TestReagentsDelegate.Update", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return myGlobalDataTO
        'End Function
#End Region
    End Class
End Namespace
