Option Explicit On
Option Strict On

Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.DAL.DAO
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Types

Namespace Biosystems.Ax00.BL
    Public Class HisReagentsDelegate

#Region "Public Methods"

        ''' <summary>
        ''' Receive a list of Reagent Identifiers and for each one of them, verify if it already exists in Historics Module and when it 
        ''' does not exist, create the new Reagent
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pHisReagentsDS">Typed DataSet HisReagentsDS containing the list of Reagents to verify if they already exist 
        '''                              in Historics Module and create them when not</param>
        ''' <returns>GlobalDataTO containing a typed Dataset HisReagentsDS with the identifier in Historics Module for each Reagent in it</returns>
        ''' <remarks>
        ''' Created by:  TR 28/02/2012 
        ''' </remarks>
        Public Function CheckReagentsInHistorics(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pHisReagentsDS As HisReagentsDS) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim auxHisReagentsDS As New HisReagentsDS
                        Dim reagentsToAddDS As New HisReagentsDS
                        Dim myHisReagentsDAO As New thisReagentsDAO

                        For Each myHisReagentRow As HisReagentsDS.thisReagentsRow In pHisReagentsDS.thisReagents.Rows
                            'Search if the Reagent already exists in Historics Module
                            myGlobalDataTO = myHisReagentsDAO.ReadByReagentID(dbConnection, myHisReagentRow.ReagentID)
                            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                auxHisReagentsDS = DirectCast(myGlobalDataTO.SetDatos, HisReagentsDS)

                                If (auxHisReagentsDS.thisReagents.Count = 0) Then
                                    'New Reagent; copy it to the auxiliary DS of elements to add
                                    reagentsToAddDS.thisReagents.ImportRow(myHisReagentRow)
                                Else
                                    'The Reagent already exists in Historics Module; inform field HistReagentID
                                    myHisReagentRow.BeginEdit()
                                    myHisReagentRow.HistReagentID = auxHisReagentsDS.thisReagents.First().HistReagentID
                                    myHisReagentRow.EndEdit()
                                End If
                            Else
                                'Error verifying if the Reagent exists in Historics Module...
                                Exit For
                            End If
                        Next

                        If (Not myGlobalDataTO.HasError AndAlso reagentsToAddDS.thisReagents.Count > 0) Then
                            'Add to Historics Module all new  Reagents
                            myGlobalDataTO = myHisReagentsDAO.Create(dbConnection, reagentsToAddDS)

                            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                reagentsToAddDS = DirectCast(myGlobalDataTO.SetDatos, HisReagentsDS)

                                'Search the added Reagents in the entry DS and inform the HistReagentID in Historics Module
                                Dim lstReagentsToUpdate As List(Of HisReagentsDS.thisReagentsRow)
                                For Each myHisReagentRow As HisReagentsDS.thisReagentsRow In reagentsToAddDS.thisReagents.Rows
                                    lstReagentsToUpdate = (From a In pHisReagentsDS.thisReagents _
                                                          Where a.ReagentID = myHisReagentRow.ReagentID _
                                                         Select a).ToList()

                                    If (lstReagentsToUpdate.Count = 1) Then
                                        lstReagentsToUpdate.First().BeginEdit()
                                        lstReagentsToUpdate.First().HistReagentID = myHisReagentRow.HistReagentID
                                        lstReagentsToUpdate.First().EndEdit()
                                    End If
                                Next
                                lstReagentsToUpdate = Nothing
                            End If
                        End If

                        If (Not myGlobalDataTO.HasError) Then
                            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                            myGlobalDataTO.SetDatos = pHisReagentsDS
                        Else
                            If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                        End If
                    End If
                End If
            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "HisReagentsDelegate.CheckReagentsInHistorics", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Delete all not in use closed Reagents
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by:  SG 01/07/2013
        ''' </remarks>
        Public Function DeleteClosedNotInUse(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New thisReagentsDAO
                        resultData = myDAO.DeleteClosedNotInUse(dbConnection)

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
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "HisReagentsDelegate.DeleteClosedNotInUse", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

#End Region
    End Class
End Namespace

