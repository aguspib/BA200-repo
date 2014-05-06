Option Strict On
Option Explicit On

Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.DAL.DAO
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.DAL

Namespace Biosystems.Ax00.BL

    Public Class VirtualRotorsDelegate

#Region " Public Methods"
        ''' <summary>
        ''' Delete the all the specified Virtual Rotors
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pVRotorsDS">Typed DataSet VirtualRotorsDS containing the list of Virtual Rotors to delete</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by:  VR 20/11/2009 - Tested: OK
        ''' Modified by: SA 18/06/2010 - Changed the way of opening the DB Transaction to fulfill the new template
        '''              SA 21/06/2010 - Changed the entry parameter for a DataSet to allow deleting several Virtual Rotors
        ''' </remarks>
        Public Function Delete(ByVal pDbConnection As SqlClient.SqlConnection, ByVal pVRotorsDS As VirtualRotorsDS) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDbConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        For Each vRotor As VirtualRotorsDS.tparVirtualRotorsRow In pVRotorsDS.tparVirtualRotors.Rows
                            'First delete all positions of the specified Virtual Rotor
                            Dim myVirtualRotorsPositionsDelegate As New VirtualRotorsPositionsDelegate
                            resultData = myVirtualRotorsPositionsDelegate.DeleteRotor(dbConnection, vRotor.VirtualRotorID)

                            If (Not resultData.HasError) Then
                                'Then delete the specified Virtual Rotor
                                Dim myDAO As New tparVirtualRotorsDAO
                                resultData = myDAO.Delete(dbConnection, vRotor.VirtualRotorID)
                            End If

                            'If an error happens deleting the Rotor Positions or the Rotor, then the process finishes
                            If (resultData.HasError) Then Exit For
                        Next

                        If (Not resultData.HasError) Then
                            'When the Database Connection was opened locally, then the Commit is executed
                            If (pDbConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                        Else
                            'When the Database Connection was opened locally, then the Rollback is executed
                            If (pDbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                        End If
                    End If
                End If
            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDbConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "VirtualRotorsDelegate.Delete", EventLogEntryType.Error, False)
            Finally
                If (pDbConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function


        ''' <summary>
        ''' Delete the specified control identifier
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pControlID">Typed DataSet VirtualRotorsDS containing the list of Virtual Rotors to delete</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by:  DL 10/10/2012
        ''' </remarks>
        Public Function DeleteByControlID(ByVal pDbConnection As SqlClient.SqlConnection, _
                                          ByVal pControlID As Integer, _
                                          ByVal pAnalyzerID As String, _
                                          ByVal pWorkSessionID As String) As GlobalDataTO

            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDbConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myVirtualRotorPositionDAO As New tparVirtualRotorsPositionsDAO
                        resultData = myVirtualRotorPositionDAO.DeleteVirtualRotorByControl(dbConnection, pControlID)

                        If (Not resultData.HasError) Then
                            resultData = myVirtualRotorPositionDAO.DeleteNotInUseByRotorControl(dbConnection, pControlID, pAnalyzerID, pWorkSessionID)
                        End If

                        If (Not resultData.HasError) Then
                            'When the Database Connection was opened locally, then the Commit is executed
                            If (pDbConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                        Else
                            'When the Database Connection was opened locally, then the Rollback is executed
                            If (pDbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                        End If
                    End If
                End If
            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDbConnection Is Nothing) And (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "VirtualRotorsDelegate.DeleteByControlID", EventLogEntryType.Error, False)
            Finally
                If (pDbConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function


        ''' <summary>
        ''' Delete the specified calibrator identifier
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pCalibratorID">Calibrator identifier</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by:  DL 10/10/2012
        ''' </remarks>
        Public Function DeleteByCalibratorID(ByVal pDbConnection As SqlClient.SqlConnection, _
                                             ByVal pCalibratorID As Integer, _
                                             ByVal pAnalyzerID As String, _
                                             ByVal pWorkSessionID As String) As GlobalDataTO

            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDbConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myVirtualRotorPositionDAO As New tparVirtualRotorsPositionsDAO
                        resultData = myVirtualRotorPositionDAO.DeleteVirtualRotorByCalibrator(dbConnection, pCalibratorID)

                        If (Not resultData.HasError) Then
                            resultData = myVirtualRotorPositionDAO.DeleteNotInUseByRotorCalibrator(dbConnection, pCalibratorID, pAnalyzerID, pWorkSessionID)
                        End If

                        If (Not resultData.HasError) Then
                            'When the Database Connection was opened locally, then the Commit is executed
                            If (pDbConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                        Else
                            'When the Database Connection was opened locally, then the Rollback is executed
                            If (pDbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                        End If
                    End If
                End If
            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDbConnection Is Nothing) And (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "VirtualRotorsDelegate.DeleteByCalibratorID", EventLogEntryType.Error, False)
            Finally
                If (pDbConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Verify if a Virtual Rotor exists, searching it by Rotor Type and Name
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pRotorType">Rotor Type</param>
        ''' <param name="pRotorName">Rotor Name</param>
        ''' <returns>GlobalDataTO containing an integer value with the ID of the Virtual Rotor
        '''          when it exists</returns>
        ''' <remarks>
        ''' Created by:  VR 20/11/2009 - Tested: OK
        ''' Modified by: SA 18/06/2010 - Changed the way of open the DB Connection to fulfill the new template
        ''' </remarks>
        Public Function ExistVRotor(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pRotorType As String, ByVal pRotorName As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New tparVirtualRotorsDAO
                        resultData = myDAO.ReadByRotorTypeAndVirtualRotorName(dbConnection, pRotorType, pRotorName)
                    End If
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "VirtualRotorsDelegate.ExistVRotor", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get the list of Virtual Rotors of an specified type
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pRotorType">Type of Virtual Rotors to get</param>
        ''' <param name="pInternalRotor">Optional parameter to indicate when the Virtual Rotor to get
        '''                              is the Internal one to save Reagents Rotor positions</param>  
        ''' <returns>GlobalDataTO containing a typed DataSet VirtualRotorsDS with the list of all
        '''          Virtual Rotors of the specified type</returns>
        ''' <remarks>
        ''' Created by:  VR 20/11/2009 - Tested: OK
        ''' Modified by: SA 18/06/2010 - Changed the way of open the DB Connection to fulfill the new template
        '''              SA 18/04/2011 - Added new optional parameter pInternalRotor and filter the query 
        '''                              according value of this new field
        ''' </remarks>        
        Public Function GetVRotorsByRotorType(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pRotorType As String, _
                                              Optional ByVal pInternalRotor As Boolean = False) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New tparVirtualRotorsDAO
                        resultData = myDAO.ReadByRotorType(dbConnection, pRotorType, pInternalRotor)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "VirtualRotorsDelegate.GetVRotorsByRotorType", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Create or update a Virtual Rotor of the specified type with all filled Rotor positions
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pRotorType">Rotor Type</param>
        ''' <param name="pVirtualRotorPositionsDS">Typed DataSet VirtualRotorPosititionsDS containing all filled 
        '''                                        positions in the Virtual Rotor</param>
        ''' <param name="pVirtualRotorName">Name of the Virtual Rotor</param>
        ''' <returns>GlobalDataTO containing an Integer value with the Identifier of the created Virtual Rotor</returns>
        ''' <remarks>
        ''' Created by:  VR 20/11/2009 - Tested: OK
        ''' Modified by: VR 22/12/2009 - Tested: OK
        '''              AG 08/01/2010 - Function return a GlobalDataTO not a GlobalDataTO.SetDatos - Tested: OK
        '''              AG 25/01/2010 - Before update and existing virtual rotor delete all positions
        '''              SA 10/03/2010 - Changed the way of opening the DB Transaction to fulfill the new template
        '''              SA 19/04/2011 - When field VirtualRotorID in the DS is informed but its value is -1 it means the Virtual
        '''                              Rotor has to be created (same as when the field has a Null value)
        ''' </remarks>
        Public Function Save(ByVal pDbConnection As SqlClient.SqlConnection, ByVal pRotorType As String, ByVal pVirtualRotorPositionsDS As VirtualRotorPosititionsDS, _
                             ByVal pVirtualRotorName As String, Optional ByVal pInternalRotor As Boolean = False) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDbConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        If (pVirtualRotorPositionsDS.tparVirtualRotorPosititions.Rows.Count > 0) Then
                            Dim virtualRotorPosToAdd As New tparVirtualRotorsPositionsDAO

                            If (pVirtualRotorPositionsDS.tparVirtualRotorPosititions(0).IsVirtualRotorIDNull) OrElse _
                               (pVirtualRotorPositionsDS.tparVirtualRotorPosititions(0).VirtualRotorID = -1) Then
                                'Saving a new Virtual Rotor
                                Dim rotorToAdd As New tparVirtualRotorsDAO
                                resultData = rotorToAdd.Create(dbConnection, pRotorType, pVirtualRotorName, pInternalRotor)

                                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                    'Successfully addition; get the automatically generated ID
                                    Dim vRotorID As Integer = DirectCast(resultData.SetDatos, Integer)

                                    'Inform the generated ID for each one of the Rotor Positions before adding them
                                    For i As Integer = 0 To pVirtualRotorPositionsDS.tparVirtualRotorPosititions.Rows.Count - 1
                                        pVirtualRotorPositionsDS.tparVirtualRotorPosititions(i).VirtualRotorID = vRotorID
                                    Next

                                    'Create also the Rotor Positions
                                    resultData = virtualRotorPosToAdd.Create(dbConnection, pVirtualRotorPositionsDS)
                                End If
                            Else
                                'Updating an exiting Virtual Rotor
                                'First delete all currently filled positions...
                                resultData = virtualRotorPosToAdd.DeleteAll(dbConnection, pVirtualRotorPositionsDS.tparVirtualRotorPosititions(0).VirtualRotorID)
                                If (Not resultData.HasError) Then
                                    'Finally add the new informed positions
                                    resultData = virtualRotorPosToAdd.Create(dbConnection, pVirtualRotorPositionsDS)
                                End If
                            End If
                        End If

                        If (Not resultData.HasError) Then
                            'When the Database Connection was opened locally, then the Commit is executed
                            If (pDbConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                        Else
                            'When the Database Connection was opened locally, then the Rollback is executed
                            If (pDbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                        End If
                    End If
                End If
            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDbConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "VirtualRotorsDelegate.Save", EventLogEntryType.Error, False)
            Finally
                If (pDbConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function
#End Region
    End Class
End Namespace