Option Explicit On
Option Strict On

Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.DAL.DAO
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Global.TO

Namespace Biosystems.Ax00.BL
    Public Class ReagentsDelegate

#Region "Public Methods"
        ''' <summary>
        ''' Add new Reagents
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pReagentsDS">Typed DataSet ReagentsDS containing the data of the Reagents to add</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by: TR 10/03/2010
        ''' </remarks>
        Public Function Create(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pReagentsDS As ReagentsDS) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError) AndAlso (Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        If (pReagentsDS.tparReagents.Rows.Count > 0) Then
                            Dim myReagentsDAO As New tparReagentsDAO()
                            myGlobalDataTO = myReagentsDAO.Create(dbConnection, pReagentsDS)

                            If (Not myGlobalDataTO.HasError) Then
                                'When the Database Connection was opened locally, then the Commit is executed
                                If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                            Else
                                'When the Database Connection was opened locally, then the Rollback is executed
                                If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                            End If
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
                GlobalBase.CreateLogActivity(ex.Message, "ReagentsDelegate.Create", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Delete the specified Reagent
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pReagentID">Reagent Identifier</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by: TR 15/03/2010
        ''' </remarks>
        Public Function Delete(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pReagentID As Integer) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError) AndAlso (Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myReagentsDAO As New tparReagentsDAO()
                        myGlobalDataTO = myReagentsDAO.Delete(dbConnection, pReagentID)

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
                GlobalBase.CreateLogActivity(ex.Message, "ReagentsDelegate.Delete", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Remove a Reagent and all the related elements:
        ''' ** Volumes defined for the Reagent when used for the Test
        ''' ** Positions in Reagents Rotor
        ''' ** Positions in Virtual Reagents Rotor
        ''' ** Link between Test and Reagent
        ''' ** All Contaminations defined for the Reagent 
        ''' ** If the Reagent is used for another Test, then it is not deleted
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestReagentsVolumeList">Typed Object DeletedTestReagentsVolumeTO containing all Reagent data that should be saved</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by: TR 15/03/2010
        ''' </remarks>
        Public Function DeleteReagentCascade(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
                                             ByVal pTestReagentsVolumeList As List(Of DeletedTestReagentsVolumeTO)) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError) AndAlso (Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myTestReagentDelegate As New TestReagentsDelegate()
                        Dim myTestReagentsVolumeDelegate As New TestReagentsVolumeDelegate()

                        Dim myVirtualRotorPosDS As New VirtualRotorPosititionsDS
                        Dim myVirtualRotorDelegate As New VirtualRotorsDelegate()
                        Dim myVirtualRotorPosDelegate As New VirtualRotorsPositionsDelegate

                        Dim myReagentsDS As New ReagentsDS
                        Dim myContaminationsDelegate As New ContaminationsDelegate

                        Dim myRotorContentPosDS As New WSRotorContentByPositionDS
                        Dim myRotorContentByPosDAO As New twksWSRotorContentByPositionDAO()

                        For Each DeleteReagentTO As DeletedTestReagentsVolumeTO In pTestReagentsVolumeList
                            'Delete the  Test/Reagent Volume
                            myGlobalDataTO = myTestReagentsVolumeDelegate.DeleteByTestIDReagNumberReagID(dbConnection, DeleteReagentTO.TestID, _
                                                                                                         DeleteReagentTO.ReagentNumber, DeleteReagentTO.ReagentID)
                            If (myGlobalDataTO.HasError) Then Exit For

                            'It there are bottles placed in whatever Virtual Reagents Rotor, get the list of Rotor Positions to download the Reagent
                            myGlobalDataTO = myVirtualRotorPosDelegate.GetPositionsByReagentID(dbConnection, DeleteReagentTO.ReagentID)
                            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                myVirtualRotorPosDS = DirectCast(myGlobalDataTO.SetDatos, VirtualRotorPosititionsDS)

                                For Each virtualRotorPosRow As VirtualRotorPosititionsDS.tparVirtualRotorPosititionsRow In myVirtualRotorPosDS.tparVirtualRotorPosititions.Rows
                                    'Verify if there are bottles placed in the Analyzer Reagents Rotor 
                                    myGlobalDataTO = myRotorContentByPosDAO.ReadByElementIDList(dbConnection, pAnalyzerID, "REAGENT", virtualRotorPosRow.ReagentID.ToString())
                                    If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                        myRotorContentPosDS = DirectCast(myGlobalDataTO.SetDatos, WSRotorContentByPositionDS)

                                        'Remove all Reagent bottles from the Analyzer Reagents Rotor
                                        If (myRotorContentPosDS.twksWSRotorContentByPosition.Rows.Count > 0) Then
                                            myGlobalDataTO = myRotorContentByPosDAO.Delete(dbConnection, myRotorContentPosDS)
                                            If (myGlobalDataTO.HasError) Then Exit For
                                        End If


                                        'Remove all Reagent bottles from the Virtual Reagents Rotor
                                        myGlobalDataTO = myVirtualRotorPosDelegate.DeletePosition(dbConnection, virtualRotorPosRow.VirtualRotorID, _
                                                                                                  virtualRotorPosRow.RingNumber, virtualRotorPosRow.CellNumber)
                                        If (myGlobalDataTO.HasError) Then Exit For
                                    Else
                                        Exit For
                                    End If
                                Next
                            End If

                            'if every thing is ok then delete the test reagent and the reagent.
                            If (Not myGlobalDataTO.HasError) Then
                                'Delete Link between Test and Reagent
                                myGlobalDataTO = myTestReagentDelegate.Delete(dbConnection, DeleteReagentTO.TestID, DeleteReagentTO.ReagentID, DeleteReagentTO.ReagentNumber)
                                If (myGlobalDataTO.HasError) Then Exit For

                                'Delete all Contaminations defined for the Reagent 
                                myGlobalDataTO = myContaminationsDelegate.DeleteCascadeContaminationByReagentID(dbConnection, DeleteReagentTO.ReagentID)
                                If (myGlobalDataTO.HasError) Then Exit For

                                'Verify if the Reagent is used for another Tests
                                myGlobalDataTO = GetReagentsData(dbConnection, DeleteReagentTO.ReagentID)
                                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                    myReagentsDS = DirectCast(myGlobalDataTO.SetDatos, ReagentsDS)

                                    If (myReagentsDS.tparReagents.Rows.Count > 0) Then
                                        If (Not myReagentsDS.tparReagents(0).IsShared) Then
                                            myGlobalDataTO = Delete(dbConnection, DeleteReagentTO.ReagentID)
                                        End If
                                    End If
                                Else
                                    Exit For
                                End If
                            Else
                                Exit For
                            End If
                        Next
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
                GlobalBase.CreateLogActivity(ex.Message, "ReagentsDelegate.DeleteReagentCascade", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get data of the Reagent identified for the informed CodeTest and ReagentNumber
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pCodeTest">Biosystems Code Test</param>
        ''' <param name="pReagentNumber">Number of Reagent</param>
        ''' <returns>GlobalDataTO containing a typed DataSet ReagentsDS with the data of the Reagent</returns>
        ''' <remarks>
        ''' Created by: AG 30/08/2011
        ''' </remarks>
        Public Function GetByCodeTest(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pCodeTest As String, ByVal pReagentNumber As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New tparReagentsDAO
                        resultData = myDAO.GetByCodeTest(dbConnection, pCodeTest, pReagentNumber)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ReagentsDelegate.GetByCodeTest", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get Reagent data searching by Reagent Name
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pReagentName">Reagent Name</param>
        ''' <returns>GlobalDataTO containing a typed DataSet ReagentsDS with data of the informed Reagent</returns>
        ''' <remarks>
        ''' Created by:  TR 30/03/2011
        ''' </remarks>
        Public Function GetReagentByReagentName(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pReagentName As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim reagentsDataDAO As New tparReagentsDAO
                        resultData = reagentsDataDAO.ReadByReagentName(dbConnection, pReagentName)
                    End If
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ReagentsDelegate.GetReagentByReagentName", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get basic data of the specified Reagent
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pReagentID">Reagent Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet ReagentsDS with data of the informed Reagent</returns>
        ''' <remarks>
        ''' Created by:  DL 22/01/2010
        ''' </remarks>
        Public Function GetReagentsData(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pReagentID As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim reagentsDataDAO As New tparReagentsDAO
                        resultData = reagentsDataDAO.Read(dbConnection, pReagentID)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ReagentsDelegate.GetReagentsData", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Update data of an existing Reagent
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pReagentsDS">Typed DataSet ReagentsDS containing the data of the Reagents to update</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  TR 10/03/2010
        ''' </remarks>
        Public Function Update(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pReagentsDS As ReagentsDS) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError) AndAlso (Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        If (pReagentsDS.tparReagents.Rows.Count > 0) Then
                            Dim myReagentsDAO As New tparReagentsDAO()
                            myGlobalDataTO = myReagentsDAO.Update(dbConnection, pReagentsDS)

                            If (Not myGlobalDataTO.HasError) Then
                                'When the Database Connection was opened locally, then the Commit is executed
                                If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                            Else
                                'When the Database Connection was opened locally, then the Rollback is executed
                                If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                            End If
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
                GlobalBase.CreateLogActivity(ex.Message, "ReagentsDelegate.Update", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Set value of flag InUse for all Reagents needed for the Tests added/removed from the Active WorkSession 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pFlag">Value of the InUse Flag to set</param>
        ''' <param name="pUpdateForExcluded">Optional parameter. When True, it means the InUse will be set to False
        '''                                  only for Reagents of Tests that have been excluded from the active WorkSession</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  GDS 10/05/2010 
        ''' Modified by: SA  09/06/2010 - Added new optional parameter to reuse this method to set InUse=False for Reagents of 
        '''                               Tests that have been excluded from the active WorkSession. Added parameter for the AnalyzerID 
        ''' </remarks>
        Public Function UpdateInUseFlag(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, ByVal pAnalyzerID As String, _
                                        ByVal pFlag As Boolean, Optional ByVal pUpdateForExcluded As Boolean = False) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError) AndAlso (Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New tparReagentsDAO
                        myGlobalDataTO = myDAO.UpdateInUseFlag(dbConnection, pWorkSessionID, pAnalyzerID, pFlag, pUpdateForExcluded)

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
                GlobalBase.CreateLogActivity(ex.Message, "ReagentsDelegate.UpdateInUseFlag", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' A Reagent is closed in Historics Module (field ClosedReagent is set to TRUE) in following cases:
        '''  ** When a Test or Test/SampleType is deleted in Tests Programming Screen, if the linked Reagents exist in the corresponding table 
        '''     in Historics Module and they are not used for another Test or Test/SampleType also existing in Historics Module
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pHistReagentID">Reagent Identifier in Historic Module</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 20/09/2012 - Method has been created here due to it is not possible to call functions in History project from 
        '''                              Parameters Programming project (circular references are not allowed). Functions in DAO Class for
        '''                              table thisReagents are called
        ''' </remarks>
        Public Function HIST_CloseReagent(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pHistReagentID As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New thisReagentsDAO
                        resultData = myDAO.CloseReagent(dbConnection, pHistReagentID)

                        If (resultData.HasError) Then
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
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ReagentsDelegate.HIST_CloseReagent", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get the maximum value of field ReagentID in table tparReagents. Used in the Update Version process to assign a suitable temporary ReagentID
        ''' to new Reagents (because function PrepareTestToSave needs it)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTO containing an integer value with the maximum value of field ReagentID </returns>
        ''' <remarks>
        ''' Created by:  SA 09/10/20014 - BA-1944 
        ''' </remarks>
        Public Function GetMaxReagentID(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New tparReagentsDAO
                        resultData = myDAO.GetMaxReagentID(dbConnection)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ReagentsDelegate.GetMaxReagentID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function
#End Region
    End Class
End Namespace


