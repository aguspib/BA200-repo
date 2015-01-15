Option Strict On
Option Explicit On

Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global

Namespace Biosystems.Ax00.DAL.DAO
    Public Class tparVirtualRotorsPositionsDAO
        Inherits DAOBase

#Region "CRUD Methods"

        ''' <summary>
        ''' Create positions in an specific Virtual Rotor
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pVirtualRotorPositionsDS">Typed DataSet containing the data of all positions in the Virtual Rotor</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by:  VR 18/11/2009 - Tested: OK
        ''' Modified by: VR 22/12/2009 - Tested: OK
        '''              SA 22/12/2009 - For fields PredilutionFactor and RequireVolume the Replace has to be of commas by dots because they are REAL values
        '''              VR 05/01/2010 - Tested : OK
        '''              AG 08/01/2010 - The only fields that can not be NULL are the PK fields (VirtualRotorID, Ring, Cell)
        '''              SA 25/10/2010 - Added new field OnlyForISE to the SQL; use N when set value of field TS_User 
        '''              RH 14/02/2011 - Code optimization and Using statement 
        '''              RH 15/09/2011 - Added new field to the INSERT: ScannedPosition
        '''              AG 03/02/2012 - Added new field to the INSERT: Status
        '''              SA 15/12/2014 - BA-1972 ==> Do not insert positions in the entry DataSet that are marked with InvalidPosition = True, which
        '''                                          means the ID of the element is missing (due to an error which cause is unkown and that cannot be
        '''                                          reproduced)
        ''' </remarks>
        Public Function Create(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pVirtualRotorPositionsDS As VirtualRotorPosititionsDS) _
                               As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString()
                Else
                    Dim objGlobal As New GlobalBase
                    Dim cmdText As String = String.Empty

                    For Each row As VirtualRotorPosititionsDS.tparVirtualRotorPosititionsRow In pVirtualRotorPositionsDS.tparVirtualRotorPosititions
                        If (Not row.InvalidPosition) Then
                            cmdText = " INSERT INTO tparVirtualRotorPositions (VirtualRotorID, RingNumber, CellNumber, TubeContent, " & vbCrLf & _
                                                                             " TubeType, MultiTubeNumber, ReagentID, SolutionCode, CalibratorID, " & vbCrLf & _
                                                                             " ControlID, MultiItemNumber, SampleType, OrderID, PatientID, " & vbCrLf & _
                                                                             " PredilutionFactor, OnlyForISE, RealVolume, ScannedPosition, BarcodeInfo, " & vbCrLf & _
                                                                             " BarcodeStatus, Status, TS_User, TS_DateTime) " & vbCrLf & _
                                      " VALUES (" & row.VirtualRotorID & ", " & vbCrLf & _
                                                    row.RingNumber & ", " & vbCrLf & _
                                                    row.CellNumber & ", " & vbCrLf

                            If (row.IsTubeContentNull) Then
                                cmdText &= " NULL, "
                            Else
                                cmdText &= " '" & row.TubeContent.Trim & "', "
                            End If

                            If (row.IsTubeTypeNull) Then
                                cmdText &= " NULL, "
                            Else
                                cmdText &= " '" & row.TubeType & "', "
                            End If

                            If (row.IsMultiTubeNumberNull) Then
                                cmdText &= " NULL, "
                            Else
                                cmdText &= row.MultiTubeNumber & ", "
                            End If

                            If (row.IsReagentIDNull) Then
                                cmdText &= " NULL, "
                            Else
                                cmdText &= row.ReagentID & ", "
                            End If

                            If (row.IsSolutionCodeNull) Then
                                cmdText &= " NULL, "
                            Else
                                cmdText &= " '" & row.SolutionCode.Trim & "', "
                            End If

                            If (row.IsCalibratorIDNull) Then
                                cmdText &= " NULL, "
                            Else
                                cmdText &= row.CalibratorID & ", "
                            End If

                            If (row.IsControlIDNull) Then
                                cmdText &= " NULL, "
                            Else
                                cmdText &= row.ControlID & ", "
                            End If

                            If (row.IsMultiItemNumberNull) Then
                                cmdText &= " NULL, "
                            Else
                                cmdText &= row.MultiItemNumber & ", "
                            End If

                            If (row.IsSampleTypeNull) Then
                                cmdText &= " NULL, "
                            Else
                                cmdText &= " '" & row.SampleType.Trim & "', "
                            End If

                            If (row.IsOrderIDNull) Then
                                cmdText &= " NULL, "
                            Else
                                cmdText &= " N'" & row.OrderID.Trim.Replace("'", "''") & "', "
                            End If

                            If (row.IsPatientIDNull) Then
                                cmdText &= " NULL, "
                            Else
                                cmdText &= " N'" & row.PatientID.Trim.Replace("'", "''") & "', "
                            End If

                            If (row.IsPredilutionFactorNull) Then
                                cmdText &= " NULL, "
                            Else
                                cmdText &= ReplaceNumericString(row.PredilutionFactor) & ", "
                            End If

                            If (row.IsOnlyForISENull) Then
                                cmdText &= " 0, "
                            Else
                                cmdText &= Convert.ToInt32(row.OnlyForISE) & ", "
                            End If

                            If (row.IsRealVolumeNull) Then
                                cmdText &= " NULL, "
                            Else
                                cmdText &= ReplaceNumericString(row.RealVolume) & ", "
                            End If

                            If (row.IsScannedPositionNull) Then
                                cmdText &= " NULL, "
                            Else
                                cmdText &= Convert.ToInt32(row.ScannedPosition) & ", "
                            End If

                            If (row.IsBarcodeInfoNull) Then
                                cmdText &= " NULL, "
                            Else
                                cmdText &= " '" & row.BarcodeInfo.Trim.Replace("'", "''") & "', "
                            End If

                            If (row.IsBarcodeStatusNull) Then
                                cmdText &= " NULL, "
                            Else
                                cmdText &= " '" & row.BarcodeStatus.Trim.Replace("'", "''") & "', "
                            End If

                            If (row.IsStatusNull) Then
                                cmdText &= " NULL, "
                            Else
                                cmdText &= " '" & row.Status.Trim & "', "
                            End If

                            If (row.IsTS_UserNull) Then
                                cmdText &= " N'" & objGlobal.GetSessionInfo.UserName.Trim.Replace("'", "''") & "', "
                            Else
                                cmdText &= " N'" & row.TS_User.Trim.Replace("'", "''") & "', "
                            End If

                            If (row.IsTS_DateTimeNull) Then
                                cmdText &= " '" & Now.ToString("yyyyMMdd HH:mm:ss") & "') "
                            Else
                                cmdText &= " '" & row.TS_DateTime.ToString("yyyyMMdd HH:mm:ss") & "') "
                            End If

                            'Execute the SQL sentence 
                            Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                                resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                            End Using
                        End If
                    Next
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "tparVirtualRotorsPositionsDAO.Create", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Delete all positions of the specified Virtual Rotor
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pVirtualRotorID">Virtual Rotor Identifier</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks> 
        ''' Created by:  VR 18/11/2009 - Tested: OK
        ''' Modified by: AG 08/01/2010 - Corrections (Tested OK)
        '''              SA 28/03/2012 - Changed the function template
        ''' </remarks>
        Public Function DeleteAll(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pVirtualRotorID As Integer) As GlobalDataTO
            Dim dataToReturn As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    dataToReturn.HasError = True
                    dataToReturn.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = " DELETE FROM tparVirtualRotorPositions " & vbCrLf & _
                                            " WHERE  VirtualRotorID = " & pVirtualRotorID

                    Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                        dataToReturn.AffectedRecords = dbCmd.ExecuteNonQuery()
                        dataToReturn.HasError = False
                    End Using
                End If
            Catch ex As Exception
                dataToReturn = New GlobalDataTO()
                dataToReturn.HasError = True
                dataToReturn.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                dataToReturn.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "tparVirtualRotorsPositionsDAO.Delete", EventLogEntryType.Error, False)
            End Try
            Return dataToReturn
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
        ''' Modified by: SA 28/03/2012 - Changed the function template
        ''' </remarks>
        Public Function DeletePosition(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pVirtualRotorID As Integer, _
                                       ByVal pRingNumber As Integer, ByVal pCellNumber As Integer) As GlobalDataTO
            Dim dataToReturn As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    dataToReturn.HasError = True
                    dataToReturn.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = " DELETE FROM tparVirtualRotorPositions " & vbCrLf & _
                                            " WHERE  VirtualRotorID = " & pVirtualRotorID & vbCrLf & _
                                            " AND    RingNumber     = " & pRingNumber & vbCrLf & _
                                            " AND    CellNumber     = " & pCellNumber & vbCrLf

                    Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                        dataToReturn.AffectedRecords = dbCmd.ExecuteNonQuery()
                        dataToReturn.HasError = False
                    End Using
                End If
            Catch ex As Exception
                dataToReturn.HasError = True
                dataToReturn.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                dataToReturn.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "tparVirtualRotorsPositionsDAO.DeletePostion", EventLogEntryType.Error, False)
            End Try
            Return dataToReturn
        End Function


        ''' <summary>
        ''' Delete tparVirtualRotorPositions by control identifier 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pControlID">Control Identifier</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  DL 10/10/2012
        ''' </remarks>
        Public Function DeleteVirtualRotorByControl(ByVal pDBConnection As SqlClient.SqlConnection, _
                                                    ByVal pControlID As Integer) As GlobalDataTO

            Dim dataToReturn As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    dataToReturn.HasError = True
                    dataToReturn.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdtext As String = String.Empty

                    cmdtext &= "DELETE" & vbCrLf
                    cmdtext &= "  FROM tparVirtualRotorPositions" & vbCrLf
                    cmdtext &= " WHERE ControlID  = " & pControlID & vbCrLf
                    cmdtext &= "   AND TubeContent = 'CTRL'"

                    Using dbCmd As New SqlClient.SqlCommand(cmdtext, pDBConnection)
                        dataToReturn.AffectedRecords = dbCmd.ExecuteNonQuery()
                        dataToReturn.HasError = False
                    End Using
                End If

            Catch ex As Exception
                dataToReturn.HasError = True
                dataToReturn.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                dataToReturn.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "tparVirtualRotorsPositionsDAO.DeleteControl", EventLogEntryType.Error, False)
            End Try
            Return dataToReturn
        End Function


        ''' <summary>
        ''' Delete tparVirtualRotorPositions by control identifier 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pCalibratorID">Calibrator Identifier</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  DL 11/10/2012
        ''' </remarks>
        Public Function DeleteVirtualRotorByCalibrator(ByVal pDBConnection As SqlClient.SqlConnection, _
                                                       ByVal pCalibratorID As Integer) As GlobalDataTO

            Dim dataToReturn As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    dataToReturn.HasError = True
                    dataToReturn.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdtext As String = String.Empty

                    cmdtext &= "DELETE" & vbCrLf
                    cmdtext &= "  FROM tparVirtualRotorPositions" & vbCrLf
                    cmdtext &= " WHERE CalibratorID  = " & pCalibratorID & vbCrLf
                    cmdtext &= "   AND TubeContent = 'CALIB'"

                    Using dbCmd As New SqlClient.SqlCommand(cmdtext, pDBConnection)
                        dataToReturn.AffectedRecords = dbCmd.ExecuteNonQuery()
                        dataToReturn.HasError = False
                    End Using
                End If

            Catch ex As Exception
                dataToReturn.HasError = True
                dataToReturn.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                dataToReturn.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "tparVirtualRotorsPositionsDAO.DeleteVirtualRotorByCalibrator", EventLogEntryType.Error, False)
            End Try
            Return dataToReturn
        End Function

        ''' <summary>
        ''' Delete twksWSNotInUseRotorPositions by control identifier 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pControlID">Control Identifier</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  DL 10/10/2012
        ''' </remarks>
        Public Function DeleteNotInUseByRotorControl(ByVal pDBConnection As SqlClient.SqlConnection, _
                                                     ByVal pControlID As Integer, _
                                                     ByVal pAnalyzerID As String, _
                                                     ByVal pWorkSessionID As String) As GlobalDataTO

            Dim dataToReturn As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    dataToReturn.HasError = True
                    dataToReturn.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdtext As String = String.Empty

                    cmdtext &= "DELETE" & vbCrLf
                    cmdtext &= "  FROM twksWSNotInUseRotorPositions" & vbCrLf
                    cmdtext &= " WHERE ControlID  = " & pControlID & vbCrLf
                    cmdtext &= "   AND TubeContent = 'CTRL'" & vbCrLf
                    cmdtext &= "   AND AnalyzerID = '" & pAnalyzerID & "'" & vbCrLf
                    cmdtext &= "   AND WorkSessionID = '" & pWorkSessionID & "'" & vbCrLf
                    cmdtext &= "   AND RotorType = 'SAMPLES'"

                    Using dbCmd As New SqlClient.SqlCommand(cmdtext, pDBConnection)
                        dataToReturn.AffectedRecords = dbCmd.ExecuteNonQuery()
                        dataToReturn.HasError = False
                    End Using
                End If

            Catch ex As Exception
                dataToReturn.HasError = True
                dataToReturn.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                dataToReturn.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSNotInUseRotorPositionsDAO.DeleteControl", EventLogEntryType.Error, False)
            End Try
            Return dataToReturn
        End Function

       

        ''' <summary>
        ''' Delete twksWSNotInUseRotorPositions by control identifier 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pCalibratorID">Calibrator Identifier</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  DL 10/10/2012
        ''' </remarks>
        Public Function DeleteNotInUseByRotorCalibrator(ByVal pDBConnection As SqlClient.SqlConnection, _
                                                        ByVal pCalibratorID As Integer, _
                                                        ByVal pAnalyzerID As String, _
                                                        ByVal pWorkSessionID As String) As GlobalDataTO

            Dim dataToReturn As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    dataToReturn.HasError = True
                    dataToReturn.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdtext As String = String.Empty

                    cmdtext &= "DELETE" & vbCrLf
                    cmdtext &= "  FROM twksWSNotInUseRotorPositions" & vbCrLf
                    cmdtext &= " WHERE CalibratorID  = " & pCalibratorID & vbCrLf
                    cmdtext &= "   AND TubeContent = 'CALIB'" & vbCrLf
                    cmdtext &= "   AND AnalyzerID = '" & pAnalyzerID & "'" & vbCrLf
                    cmdtext &= "   AND WorkSessionID = '" & pWorkSessionID & "'" & vbCrLf
                    cmdtext &= "   AND RotorType = 'SAMPLES'"

                    Using dbCmd As New SqlClient.SqlCommand(cmdtext, pDBConnection)
                        dataToReturn.AffectedRecords = dbCmd.ExecuteNonQuery()
                        dataToReturn.HasError = False
                    End Using
                End If

            Catch ex As Exception
                dataToReturn.HasError = True
                dataToReturn.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                dataToReturn.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSNotInUseRotorPositionsDAO.DeleteNotInUseByRotorCalibrator", EventLogEntryType.Error, False)
            End Try
            Return dataToReturn
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
        ''' Modified by: SA 28/03/2012 - Changed the function template
        ''' </remarks>
        Public Function ReadByReagentID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pReagentID As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT * FROM tparVirtualRotorPositions " & vbCrLf & _
                                                " WHERE  ReagentID = " & pReagentID & vbCrLf

                        Dim myVRotorPosDataDS As New VirtualRotorPosititionsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myVRotorPosDataDS.tparVirtualRotorPosititions)
                            End Using
                        End Using

                        resultData.SetDatos = myVRotorPosDataDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "tparVirtualRotorsPositionsDAO.ReadByReagentID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
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
        Public Function ReadByControlID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pControlID As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = String.Empty
                        cmdText &= "SELECT VirtualRotorID, RingNumber, CellNumber, TubeContent, TubeType, MultiTubeNumber, ReagentID" & vbCrLf
                        cmdText &= "     , SolutionCode, CalibratorID, ControlID, MultiItemNumber, SampleType, OrderID, PatientID" & vbCrLf
                        cmdText &= "     , PredilutionFactor, OnlyForISE, RealVolume, ScannedPosition, BarcodeInfo, BarcodeStatus" & vbCrLf
                        cmdText &= "     , TS_User, TS_DateTime, Status" & vbCrLf
                        cmdText &= "  FROM tparVirtualRotorPositions " & vbCrLf
                        cmdText &= " WHERE ControlID = " & pControlID & vbCrLf
                        cmdText &= "   AND TubeContent = 'CTRL'"

                        Dim myVRotorPosDataDS As New VirtualRotorPosititionsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myVRotorPosDataDS.tparVirtualRotorPosititions)
                            End Using
                        End Using

                        resultData.SetDatos = myVRotorPosDataDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "tparVirtualRotorsPositionsDAO.ReadByControlID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get details of all Virtual Rotors having the specified Control placed in a Rotor Position
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pCalibrationID">CalibrationID Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet VirtualRotorPosititionsDS with the list
        '''          of positions in Virtual Rotors where the Reagent is placed</returns>
        ''' <remarks>
        ''' Created by:  DL 11/10/2012
        ''' </remarks>
        Public Function ReadByCalibrationID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pCalibrationID As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = String.Empty
                        cmdText &= "SELECT VirtualRotorID, RingNumber, CellNumber, TubeContent, TubeType, MultiTubeNumber, ReagentID" & vbCrLf
                        cmdText &= "     , SolutionCode, CalibratorID, ControlID, MultiItemNumber, SampleType, OrderID, PatientID" & vbCrLf
                        cmdText &= "     , PredilutionFactor, OnlyForISE, RealVolume, ScannedPosition, BarcodeInfo, BarcodeStatus" & vbCrLf
                        cmdText &= "     , TS_User, TS_DateTime, Status" & vbCrLf
                        cmdText &= "  FROM tparVirtualRotorPositions " & vbCrLf
                        cmdText &= " WHERE CalibratorID = " & pCalibrationID & vbCrLf
                        cmdText &= "   AND TubeContent = 'CALIB'"

                        Dim myVRotorPosDataDS As New VirtualRotorPosititionsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myVRotorPosDataDS.tparVirtualRotorPosititions)
                            End Using
                        End Using

                        resultData.SetDatos = myVRotorPosDataDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "tparVirtualRotorsPositionsDAO.ReadByCalibrationID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
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
        ''' Created by:  SA 18/11/2009 
        ''' Modified by: SA 19/04/2011 - Changed the query to allow getting the content of the Internal Virtual Rotor used to save 
        '''                              the last configuration of the Reagents Rotor
        '''              DL 04/08/2011 - Added Rotor Type parameter to allow getting also the content of the Internal Virtual Rotor
        '''                              used to save the last configuration of the Samples Rotor
        '''              SA 28/03/2012 - Changed the function template
        ''' </remarks>
        Public Function ReadByVirtualRotorID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pVirtualRotorID As Integer, _
                                             Optional ByVal pRotorType As String = "") As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT * FROM tparVirtualRotorPositions "

                        If (pVirtualRotorID > 0) Then
                            'Get content of the specified Virtual Rotor
                            cmdText &= " WHERE  VirtualRotorID = " & pVirtualRotorID
                        ElseIf (String.Compare(pRotorType, String.Empty, False) <> 0) Then
                            'Get content of the Internal Rotor of the informed type
                            cmdText &= " WHERE VirtualRotorID IN (SELECT VirtualRotorID FROM tparVirtualRotors " & vbCrLf & _
                                                                " WHERE  RotorType     = '" & pRotorType & "' " & vbCrLf & _
                                                                " AND    InternalRotor = 1) "
                        End If

                        Dim myVRotorPosDataDS As New VirtualRotorPosititionsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myVRotorPosDataDS.tparVirtualRotorPosititions)
                            End Using
                        End Using

                        resultData.SetDatos = myVRotorPosDataDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "tparVirtualRotorsPositionsDAO.ReadByVirtualRotorID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

#End Region

#Region "Other Methods"
        ''' <summary>
        ''' Read the content of one position in the specified Virtual Rotor
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pVirtualRotorID">Virtual Rotor Identifier</param>
        ''' <param name="pRotorType" >Rotor Type</param>
        ''' <param name="pRingNumber">Ring Number</param>
        ''' <param name="pCellNumber">Cell Number</param>
        ''' <returns>GlobalDataTO containing a typed DataSet VirtualRotorPositionsDS with information of the specified 
        '''          Virtual Rotor position (ring and cell)</returns>
        ''' <remarks>
        ''' Created by:  AG 21/01/2010  (Tested OK)
        ''' Modified by: SA 10/03/2010 - Changed the way of opening the DB Connection to fulfill the new template
        '''              AG 03/02/2012 - Added parameter pRotorType. When this parameter is informed, then get information in the
        '''                              Internal Virtual Rotor containing saved positions from a previous WorkSession
        '''              SA 28/03/2012 - Changed the function template
        ''' </remarks>
        Public Function ReadPosition(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pVirtualRotorID As Integer, ByVal pRotorType As String, _
                                     ByVal pRingNumber As Integer, ByVal pCellNumber As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = ""

                        If (pVirtualRotorID > 0) Then
                            cmdText = " SELECT * FROM tparVirtualRotorPositions " & vbCrLf & _
                                      " WHERE  VirtualRotorID = " & pVirtualRotorID & vbCrLf & _
                                      " AND    RingNumber     = " & pRingNumber & vbCrLf & _
                                      " AND    CellNumber     = " & pCellNumber & vbCrLf
                        Else
                            cmdText = " SELECT * FROM tparVirtualRotorPositions " & vbCrLf & _
                                      " WHERE  RingNumber = " & pRingNumber & vbCrLf & _
                                      " AND    CellNumber = " & pCellNumber & vbCrLf & _
                                      " AND    VirtualRotorID IN (SELECT VirtualRotorID FROM tparVirtualRotors " & vbCrLf & _
                                                                " WHERE  RotorType     = '" & pRotorType & "' " & vbCrLf & _
                                                                " AND    InternalRotor = 1) "
                        End If

                        Dim queryResults As New VirtualRotorPosititionsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(queryResults.tparVirtualRotorPosititions)
                            End Using
                        End Using

                        resultData.SetDatos = queryResults
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "VirtualRotorPositionsDelegate.ReadPosition", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function
#End Region
    End Class
End Namespace