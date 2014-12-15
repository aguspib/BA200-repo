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

        ''' <summary>
        ''' Evaluate if data in parameters contains invalid values. In this case do nothing but inform into internal LOG:
        ''' -	TubeContent = “CALIB” + CalibratorID = vbNULL
        ''' -	TubeContent = “CTRL” + CalibratorID = vbNULL
        ''' -	TubeContent = “TUBE_SPEC_SOL” + SolutionCode = vbNULL
        ''' -	TubeContent = “TUBE_WASH_SOL” + SolutionCode = vbNULL
        ''' -	TubeContent = “PATIENT” + PatientID = vbNULL
        ''' -	TubeContent = “REAGENT” + ReagentID = vbNULL
        ''' -	TubeContent = “SPEC_SOL” + SolutionCode = vbNULL
        ''' -	TubeContent = “WASH_SOL” + SolutionCode = vbNULL
        ''' -	TubeContent = “SALINESOL” + SolutionCode = vbNULL
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pRotorType">Rotor Type: SAMPLES or REAGENT</param>
        ''' <param name="pVirtualRotorPositionsDS">Typed DataSet VirtualRotorPosititionsDS containing all positions to save for the 
        '''                                        specified Virtual Rotor</param>
        ''' <param name="pInternalRotor">TRUE when called during internal virtual rotors generation in ResetWS</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  AG 07/10/2014 - BA-1979 ==> Add traces into log for positions with invalid values in order to find the origin
        ''' Modified by: SA 15/12/2014 - BA-1972 ==> Mark as invalid all positions with field TubeContent informed but without ID of the corresponding Element
        '''                                          to avoid save it with incomplete data (due to the error cannot be reproduced, the wrong positions are not
        '''                                          saved). If an error is raised during function execution, return it.
        ''' </remarks> 
        Public Function CheckForInvalidPosition(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pRotorType As String, ByVal pVirtualRotorPositionsDS As VirtualRotorPosititionsDS, ByVal pInternalRotor As Boolean) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim textDetails As String = "(called during ResetWS)"
                        If (Not pInternalRotor) Then textDetails = "(called during save virtual rotor from Screen)"

                        Dim myLogAcciones As New ApplicationLogManager()
                        Dim linqRes As List(Of VirtualRotorPosititionsDS.tparVirtualRotorPosititionsRow)

                        If (pRotorType = "REAGENTS") Then
                            'REAGENTS without ReagentID
                            linqRes = (From a As VirtualRotorPosititionsDS.tparVirtualRotorPosititionsRow In pVirtualRotorPositionsDS.tparVirtualRotorPosititions _
                                      Where a.TubeContent = "REAGENT" AndAlso a.IsReagentIDNull _
                                     Select a).ToList

                            If (linqRes.Count > 0) Then
                                'Mark all positions with TubeContent = REAGENT but not ReagentID as invalid 
                                For Each row As VirtualRotorPosititionsDS.tparVirtualRotorPosititionsRow In linqRes
                                    row.BeginEdit()
                                    row.InvalidPosition = True
                                    row.EndEdit()
                                Next

                                'Write the error in the LOG
                                myLogAcciones.CreateLogActivity("Invalid values: REAGENT with ReagentID = vbNULL " & textDetails, "WSNotInUseRotorPositionsDelegate.CheckForInvalidPosition", EventLogEntryType.Error, False)
                            End If

                            'SPECIAL SOLUTION, WASH SOLUTION or SALINE SOLUTION without SolutionCode
                            linqRes = (From a As VirtualRotorPosititionsDS.tparVirtualRotorPosititionsRow In pVirtualRotorPositionsDS.tparVirtualRotorPosititions _
                                      Where (a.TubeContent = "SPEC_SOL" OrElse a.TubeContent = "WASH_SOL" OrElse a.TubeContent = "SALINESOL") AndAlso a.IsSolutionCodeNull _
                                     Select a).ToList

                            If (linqRes.Count > 0) Then
                                'Mark all positions with TubeContent = SPEC_SOL/WASH_SOL/SALINESOL but not SolutionCode as invalid 
                                For Each row As VirtualRotorPosititionsDS.tparVirtualRotorPosititionsRow In linqRes
                                    row.BeginEdit()
                                    row.InvalidPosition = True
                                    row.EndEdit()
                                Next

                                'Write the error in the LOG
                                myLogAcciones.CreateLogActivity("Invalid values: Bottle solution with SolutionCode = vbNULL " & textDetails, "WSNotInUseRotorPositionsDelegate.CheckForInvalidPosition", EventLogEntryType.Error, False)
                            End If

                        Else
                            'CALIBRATOR without CalibratorID
                            linqRes = (From a As VirtualRotorPosititionsDS.tparVirtualRotorPosititionsRow In pVirtualRotorPositionsDS.tparVirtualRotorPosititions _
                                      Where a.TubeContent = "CALIB" AndAlso a.IsCalibratorIDNull _
                                     Select a).ToList

                            If (linqRes.Count > 0) Then
                                'Mark all positions with TubeContent = CALIB but not CalibratorID as invalid 
                                For Each row As VirtualRotorPosititionsDS.tparVirtualRotorPosititionsRow In linqRes
                                    row.BeginEdit()
                                    row.InvalidPosition = True
                                    row.EndEdit()
                                Next

                                'Write the error in the LOG
                                myLogAcciones.CreateLogActivity("Invalid values: CALIBRATOR with CalibratorID = vbNULL " & textDetails, "WSNotInUseRotorPositionsDelegate.CheckForInvalidPosition", EventLogEntryType.Error, False)
                            End If

                            'CONTROL without ControlID
                            linqRes = (From a As VirtualRotorPosititionsDS.tparVirtualRotorPosititionsRow In pVirtualRotorPositionsDS.tparVirtualRotorPosititions _
                                      Where a.TubeContent = "CTRL" AndAlso a.IsControlIDNull _
                                     Select a).ToList

                            If (linqRes.Count > 0) Then
                                'Mark all positions with TubeContent = CTRL but not ControlID as invalid 
                                For Each row As VirtualRotorPosititionsDS.tparVirtualRotorPosititionsRow In linqRes
                                    row.BeginEdit()
                                    row.InvalidPosition = True
                                    row.EndEdit()
                                Next

                                'Write the error in the LOG
                                myLogAcciones.CreateLogActivity("Invalid values: CONTROL with ControlID = vbNULL " & textDetails, "WSNotInUseRotorPositionsDelegate.CheckForInvalidPosition", EventLogEntryType.Error, False)
                            End If

                            'PATIENT without PatientID
                            linqRes = (From a As VirtualRotorPosititionsDS.tparVirtualRotorPosititionsRow In pVirtualRotorPositionsDS.tparVirtualRotorPosititions _
                                      Where a.TubeContent = "PATIENT" AndAlso a.IsPatientIDNull _
                                     Select a).ToList

                            If (linqRes.Count > 0) Then
                                'Mark all positions with TubeContent = PATIENT but not PatientID as invalid 
                                For Each row As VirtualRotorPosititionsDS.tparVirtualRotorPosititionsRow In linqRes
                                    row.BeginEdit()
                                    row.InvalidPosition = True
                                    row.EndEdit()
                                Next

                                'Write the error in the LOG
                                myLogAcciones.CreateLogActivity("Invalid values: PATIENT with PatientID = vbNULL " & textDetails, "WSNotInUseRotorPositionsDelegate.CheckForInvalidPosition", EventLogEntryType.Error, False)
                            End If

                            'SPECIAL SOLUTION or WASH SOLUTION in Tube without SolutionCode
                            linqRes = (From a As VirtualRotorPosititionsDS.tparVirtualRotorPosititionsRow In pVirtualRotorPositionsDS.tparVirtualRotorPosititions _
                                      Where (a.TubeContent = "TUBE_SPEC_SOL" OrElse a.TubeContent = "TUBE_WASH_SOL") AndAlso a.IsSolutionCodeNull _
                                     Select a).ToList

                            If (linqRes.Count > 0) Then
                                'Mark all positions with TubeContent = TUBE_SPEC_SOL/TUBE_WASH_SOL but not SolutionCode as invalid 
                                For Each row As VirtualRotorPosititionsDS.tparVirtualRotorPosititionsRow In linqRes
                                    row.BeginEdit()
                                    row.InvalidPosition = True
                                    row.EndEdit()
                                Next

                                'Write the error in the LOG
                                myLogAcciones.CreateLogActivity("Invalid values: Tube solution with SolutionCode = vbNULL " & textDetails, "WSNotInUseRotorPositionsDelegate.CheckForInvalidPosition", EventLogEntryType.Error, False)
                            End If
                        End If
                        pVirtualRotorPositionsDS.AcceptChanges()
                        linqRes = Nothing
                    End If
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "VirtualRotorsPositionsDelegate.CheckForInvalidPosition", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function
#End Region
    End Class
End Namespace