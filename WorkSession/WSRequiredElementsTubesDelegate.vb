Option Explicit On
Option Strict On

Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.DAL.DAO

Namespace Biosystems.Ax00.BL

    Public Class WSRequiredElementsTubesDelegate

#Region "Public Methods"

        ''' <summary>
        ''' Add the number of needed bottles of an specified size for a Required Element
        ''' </summary>
        ''' <param name="pDBConnection">Open Database Connection</param>
        ''' <param name="pRequiredPosTubes">DataSet containing information about the Required Bottle Identifier, the Bottle Identifier 
        '''                                 and the number of needed bottles</param>
        ''' <returns>Global object containing the added record and/or error information</returns>
        ''' <remarks>
        ''' Created by:
        ''' Modified by: SA 11/01/2010 - Changed the way of opening a DB Transaction to fulfill the new template.
        '''              SA 14/01/2010 - Error fixed: when calling to Create, the parameter pDBConnection was passed instead of the local variable dbConnection
        '''              SA 08/02/2012 - Changed the function template
        ''' </remarks>
        Public Function AddRequiredElementsTubes(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pRequiredPosTubes As WSRequiredElementsTubesDS) As GlobalDataTO
            Dim returnData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                returnData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not returnData.HasError AndAlso Not returnData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(returnData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim requiredTubes As New twksWSRequiredElementsTubesDAO
                        returnData = requiredTubes.Create(dbConnection, pRequiredPosTubes)

                        If (Not returnData.HasError) Then
                            'When the Connection was opened locally, then the Commit is executed
                            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                        Else
                            'When the Connection was opened locally, then the Rollback is executed
                            If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                        End If
                    End If
                End If
            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                returnData = New GlobalDataTO()
                returnData.HasError = True
                returnData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                returnData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "WSRequiredElementsTubesDelegate.AddRequiredElementsTubes", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return returnData
        End Function

        ''' <summary>
        ''' Verify if there is a record for the specified Required Element and Reagent Bottle size in table twksRequiredElementsTubes
        ''' </summary>
        ''' <param name="pDBConnection">Open Database Connection</param>
        ''' <param name="pElement">Required Element Identifier</param>
        ''' <param name="pTubeCode">Reagent Bottle Identifier</param>
        ''' <returns>DataSet with structure of table twksWSRequiredElementsTubes</returns>
        ''' <remarks>
        ''' Created by:
        ''' Modified by: SA 11/01/2010 - Changed the way of opening a DB Connection to fulfill the new template.
        '''                              Returned type was changed from WSRequiredElementsTubesDS to a GlobalDataTO
        '''              SA 08/02/2012 - Changed the function template  
        ''' </remarks>
        Public Function ExistRequiredElementTube(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pElement As Integer, ByVal pTubeCode As String) As GlobalDataTO
            Dim returnData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                returnData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not returnData.HasError AndAlso Not returnData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(returnData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim requiredTubes As New twksWSRequiredElementsTubesDAO
                        returnData = requiredTubes.ReadByElementIDAndTubeCode(dbConnection, pElement, pTubeCode)
                    End If
                End If
            Catch ex As Exception
                returnData = New GlobalDataTO()
                returnData.HasError = True
                returnData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                returnData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "WSRequiredElementsTubesDelegate.ExistRequiredElementTube", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return returnData
        End Function

        ''' <summary>
        ''' Get the list of bottles required for the specified required Reagent (those with NumTubes > 0)
        ''' </summary>
        ''' <param name="pDBConnection">Open Database Connection</param>
        ''' <param name="pElementId"> Required Element Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet ReagentTubeTypesDS with the list of bottles needed for the  
        '''          specified required Element</returns>
        ''' <remarks>
        ''' Created by:  TR 18/11/2009
        ''' Modified by: SA 11/01/2010 - Changed the way of opening a DB Connection to fulfill the new template
        '''              SA 08/02/2012 - Changed the function template  
        ''' </remarks>
        Public Function GetAllNeededBottles(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pElementID As Integer) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myWSRequiredElementsTubesDAO As New twksWSRequiredElementsTubesDAO
                        myGlobalDataTO = myWSRequiredElementsTubesDAO.GetAllNeededBottles(dbConnection, pElementID)
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "WSRequiredElementsTubesDelegate.GetAllNeededBottles", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Delete all information for the Work Session
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by: GDS 21/04/2010
        ''' Modified by: SA 08/02/2012 - Changed the function template  
        ''' </remarks>
        Public Function ResetWS(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksWSRequiredElementsTubesDAO
                        resultData = myDAO.ResetWS(dbConnection, pWorkSessionID)

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
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "WSRequiredElementsTubesDelegate.ResetWS", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Modify the number of needed Bottles of the specified size for the informed Element
        ''' </summary>
        ''' <param name="pDBConnection">Open Database Connection</param>
        ''' <param name="pRequiredPosTubes">DataSet containing information about the Required Bottle Identifier, the Bottle Identifier 
        '''                                 and the number of needed bottles</param>
        ''' <returns>Global object containing the modified record and/or error information</returns>
        ''' <remarks>
        ''' Created by:
        ''' Modified by: SA 11/01/2010 - Changed the way of opening a DB Transaction to fulfill the new template
        '''              SA 08/02/2012 - Changed the function template
        ''' </remarks>
        Public Function UpdateNumTubes(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pRequiredPosTubes As WSRequiredElementsTubesDS) As GlobalDataTO
            Dim returnData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                returnData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not returnData.HasError AndAlso Not returnData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(returnData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim requiredTubes As New twksWSRequiredElementsTubesDAO
                        returnData = requiredTubes.Update(dbConnection, pRequiredPosTubes)

                        If (Not returnData.HasError) Then
                            'When the Connection was opened locally, then the Commit is executed
                            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                        Else
                            'When the Connection was opened locally, then the Rollback is executed
                            If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                        End If
                    End If
                End If
            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                returnData = New GlobalDataTO()
                returnData.HasError = True
                returnData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                returnData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "WSRequiredElementsTubesDelegate.UpdateNumTubes", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return returnData
        End Function
#End Region

    End Class

End Namespace
