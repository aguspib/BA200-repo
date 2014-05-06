Option Strict On
Option Explicit On

Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.DAL.DAO
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.DAL



Namespace Biosystems.Ax00.BL
    Public Class VirtualRotorsPositionsDelegate

#Region " Public Methods"

        ''' <summary>
        ''' Create all positions of a Virtual Rotor
        ''' </summary>
        ''' <param name="pDbConnection">Open DB Connection</param>
        ''' <param name="pVirtualRotorPosDS">Typed DataSet VirtualRotorPositions containing data of all positions in the
        '''                                  Virtual Rotor to create</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by:  VR 20/11/2009 - Tested: OK / re-tested after datatype changed in DS - OK 
        ''' Modified by: SA 10/03/2010 - Changed the way of opening the DB Transaction to fulfill the new template; removed the 
        '''                              For/Next, it has not sense
        ''' </remarks>
        Public Function CreateRotor(ByVal pDbConnection As SqlClient.SqlConnection, ByVal pVirtualRotorPosDS As VirtualRotorPosititionsDS) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDbConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myVirtualRotorsPos As New tparVirtualRotorsPositionsDAO
                        resultData = myVirtualRotorsPos.Create(dbConnection, pVirtualRotorPosDS)

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
                If (pDbConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "VirtualRotorsPositionsDelegate.CreateRotor", EventLogEntryType.Error, False)
            Finally
                If (pDbConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Delete position (Ring and Cell Number) on the informed Virtual Rotor
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pVirtualRotorID">Virtual Rotor Identifier</param>
        ''' <param name="pRingNumber">Ring Number</param>
        ''' <param name="pCellNumber">Cell Number</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  TR 23/03/2010
        ''' </remarks>
        Public Function DeletePosition(ByVal pDbConnection As SqlClient.SqlConnection, ByVal pVirtualRotorID As Integer, ByVal pRingNumber As Integer, _
                                       ByVal pCellNumber As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDbConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myVirtualRotorsPos As New tparVirtualRotorsPositionsDAO
                        resultData = myVirtualRotorsPos.DeletePosition(dbConnection, pVirtualRotorID, pRingNumber, pCellNumber)

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
                If (pDbConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "VirtualRotorsPositionsDelegate.DeletePosition", EventLogEntryType.Error, False)
            Finally
                If (pDbConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Delete all positions of the specified Virtual Rotor
        ''' </summary>
        ''' <param name="pDbConnection">Open DB Connection</param>
        ''' <param name="pVirtualRotorID">Virtual Rotor Identifier</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by:  VR 20/11/2009 - Tested: OK / re-tested after datatype changed in DS - OK 
        ''' Modified by: SA 10/03/2010 - Changed the way of opening the DB Transaction to fulfill the new template
        ''' </remarks>
        Public Function DeleteRotor(ByVal pDbConnection As SqlClient.SqlConnection, ByVal pVirtualRotorID As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDbConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myVirtualRotorsPos As New tparVirtualRotorsPositionsDAO
                        resultData = myVirtualRotorsPos.DeleteAll(dbConnection, pVirtualRotorID)

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
                If (pDbConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "VirtualRotorsPositionsDelegate.DeleteRotor", EventLogEntryType.Error, False)
            Finally
                If (pDbConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get details of all positions in the specified Virtual Rotor
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pVirtualRotorID">Virtual Rotor Identifier. When parameter for the Rotor Type is informed, this parameter
        '''                               should be set to zero or to a negative value to ignore it</param>
        ''' <param name="pRotorType">Rotor Type. Optional parameter; when informed, data returned will be the content of
        '''                          all positions in the Internal Virtual Rotor of this type (the Virtual Rotor containing 
        '''                          positioned elements from a previous WorkSession)</param>
        ''' <returns>GlobalDataTO containing a typed DataSet VirtualRotorPositionsDS with information of all positions of the 
        '''          specified Virtual Rotor</returns>
        ''' <remarks>
        ''' Created by:  VR 20/11/2009 - Tested: OK / re-tested after datatype changed in DS - OK  
        ''' Modified by: SA 10/03/2010 - Changed the way of open the DB Connection to fulfill the new template 
        '''              DL 04/08/2011 - Added Rotor Type parameter to allow getting also the content of the Internal Virtual Rotor
        '''                              used to save the last configuration of the Samples Rotor
        '''              SA 28/03/2012 - Changed the function template
        ''' </remarks>
        Public Function GetRotor(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pVirtualRotorID As Integer, Optional ByVal pRotorType As String = "") As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myVirtualRotorsPos As New tparVirtualRotorsPositionsDAO
                        resultData = myVirtualRotorsPos.ReadByVirtualRotorID(dbConnection, pVirtualRotorID, pRotorType)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "VirtualRotorsPositionsDelegate.GetRotor", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Read the content of one position in the specified Virtual Rotor
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pVirtualRotorID">Virtual Rotor Identifier</param>
        ''' <param name="pRotorType">Rotor Type</param>
        ''' <param name="pRingNumber">Ring Number</param>
        ''' <param name="pCellNumber">Cell Number</param>
        ''' <returns>GlobalDataTO containing a typed DataSet VirtualRotorPositionsDS with information of 
        '''          the specified Virtual Rotor position (ring and cell)</returns>
        ''' <remarks>
        ''' Created by:  AG 21/01/2010 (Tested OK)
        ''' Modified by: SA 10/03/2010 - Changed the way of open the DB Connection to fulfill the new template
        '''              AG 03/02/2012 - Added parameter for the Rotor Type
        ''' </remarks>
        Public Function GetPosition(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pVirtualRotorID As Integer, ByVal pRotorType As String, _
                                    ByVal pRingNumber As Integer, ByVal pCellNumber As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError) AndAlso (Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New tparVirtualRotorsPositionsDAO
                        resultData = myDAO.ReadPosition(dbConnection, pVirtualRotorID, pRotorType, pRingNumber, pCellNumber)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "VirtualRotorsPositionsDelegate.GetPosition", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get details of all Virtual Rotors having the specified Reagent placed in a Rotor Position
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pReagentID">Reagent Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet VirtualRotorPosititionsDS with the list
        '''          of positions in Virtual Rotors where the Reagent is placed</returns>
        ''' <remarks>
        ''' Created by:  TR 23/03/2010
        ''' </remarks>
        Public Function GetPositionsByReagentID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pReagentID As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError) AndAlso (Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New tparVirtualRotorsPositionsDAO
                        resultData = myDAO.ReadByReagentID(dbConnection, pReagentID)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "VirtualRotorsPositionsDelegate.GetPositionsByReagentID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function


        ''' <summary>
        ''' Get details of all Virtual Rotors having the specified Control placed in a Rotor Position
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pControlID">Control Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet VirtualRotorPosititionsDS with the list
        '''          of positions in Virtual Rotors where the Reagent is placed</returns>
        ''' <remarks>
        ''' Created by:  DL 10/10/2012
        ''' </remarks>
        Public Function GetPositionsByControlID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pControlID As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError) AndAlso (Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New tparVirtualRotorsPositionsDAO
                        resultData = myDAO.ReadByControlID(dbConnection, pControlID)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "VirtualRotorsPositionsDelegate.GetPositionsByControlID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get details of all Virtual Rotors having the specified Control placed in a Rotor Position
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pCalibrationID">Calibration Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet VirtualRotorPosititionsDS with the list
        '''          of positions in Virtual Rotors where the Reagent is placed</returns>
        ''' <remarks>
        ''' Created by:  DL 10/10/2012
        ''' </remarks>
        Public Function GetPositionsByCalibrationID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pCalibrationID As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError) AndAlso (Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New tparVirtualRotorsPositionsDAO
                        resultData = myDAO.ReadByCalibrationID(dbConnection, pCalibrationID)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "VirtualRotorsPositionsDelegate.GetPositionsByCalibrationID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

#End Region
    End Class
End Namespace