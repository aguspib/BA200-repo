Option Strict On
Option Explicit On

Imports System.Data.SqlClient
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global

Namespace Biosystems.Ax00.DAL.DAO

    Public Class twksWSNotInUseRotorPositionsDAO
          

#Region "CRUD"
        ''' <summary>
        ''' Add the additional information of each Element placed in a Rotor Position with Status Not In Use in the specified Work Session
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pRotorType">Rotor Type</param>
        ''' <param name="pVirtualRotorPositionsDS">Typed DataSet containing the information to add for a Rotor Position with Status = NOT IN USE</param>        
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by:  AG 19/01/2010 (Tested OK)
        ''' Modified by: SA 26/10/2010 - Added field OnlyForISE to the SQL sentence; added the N preffix for multilanguage of field PatientID 
        '''              AG 30/08/2011 - Added Barcode fields: ScannedPosition, BarcodeInfo, BarcodeStatus
        '''              AG 03/02/2012 - Added Status field ... NULL or DEPLETED or FEW
        '''              SA 15/12/2014 - BA-1972 ==> Do not insert positions in the entry DataSet that are marked with InvalidPosition = True, which
        '''                                          means the ID of the element is missing (due to an error which cause is unkown and that cannot be
        '''                                          reproduced)
        ''' </remarks>
        Public Function Create(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, _
                               ByVal pRotorType As String, ByVal pVirtualRotorPositionsDS As VirtualRotorPosititionsDS) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    For Each tparVirtualRotorPositionsRow As VirtualRotorPosititionsDS.tparVirtualRotorPosititionsRow In pVirtualRotorPositionsDS.tparVirtualRotorPosititions
                        If (Not tparVirtualRotorPositionsRow.InvalidPosition) Then
                            Dim cmdText As String = " INSERT INTO twksWSNotInUseRotorPositions (AnalyzerID, RotorType, RingNumber, CellNumber, WorkSessionID, " & vbCrLf & _
                                                                                              " TubeContent, ReagentID, SolutionCode, CalibratorID, ControlID, " & vbCrLf & _
                                                                                              " MultiItemNumber, SampleType, PatientID, OrderID, PredilutionFactor, " & vbCrLf & _
                                                                                              " Status, OnlyForISE, ScannedPosition, BarcodeInfo, BarcodeStatus) " & vbCrLf & _
                                                    " VALUES(N'" & pAnalyzerID.Trim.Replace("'", "''") & "', " & vbCrLf & _
                                                             "'" & pRotorType.Trim & "', " & vbCrLf & _
                                                                   tparVirtualRotorPositionsRow.RingNumber & ", " & vbCrLf & _
                                                                   tparVirtualRotorPositionsRow.CellNumber & ", " & vbCrLf & _
                                                             "'" & pWorkSessionID.Trim & "', " & vbCrLf & _
                                                             "'" & tparVirtualRotorPositionsRow.TubeContent.Trim & "', " & vbCrLf

                            If (tparVirtualRotorPositionsRow.IsReagentIDNull) Then
                                cmdText &= " NULL, " & vbCrLf
                            Else
                                cmdText &= tparVirtualRotorPositionsRow.ReagentID & ", " & vbCrLf
                            End If

                            If (tparVirtualRotorPositionsRow.IsSolutionCodeNull) Then
                                cmdText &= " NULL, " & vbCrLf
                            Else
                                cmdText &= " '" & tparVirtualRotorPositionsRow.SolutionCode.Trim & "', " & vbCrLf
                            End If

                            If (tparVirtualRotorPositionsRow.IsCalibratorIDNull) Then
                                cmdText &= " NULL, " & vbCrLf
                            Else
                                cmdText &= tparVirtualRotorPositionsRow.CalibratorID & ", " & vbCrLf
                            End If

                            If (tparVirtualRotorPositionsRow.IsControlIDNull) Then
                                cmdText &= " NULL, " & vbCrLf
                            Else
                                cmdText &= tparVirtualRotorPositionsRow.ControlID & ", " & vbCrLf
                            End If

                            If (tparVirtualRotorPositionsRow.IsMultiItemNumberNull) Then
                                cmdText &= " NULL, " & vbCrLf
                            Else
                                cmdText &= tparVirtualRotorPositionsRow.MultiItemNumber & ", " & vbCrLf
                            End If

                            If (tparVirtualRotorPositionsRow.IsSampleTypeNull) Then
                                cmdText &= " NULL, " & vbCrLf
                            Else
                                cmdText &= " '" & tparVirtualRotorPositionsRow.SampleType & "', " & vbCrLf
                            End If

                            If (tparVirtualRotorPositionsRow.IsPatientIDNull) Then
                                cmdText &= " NULL, " & vbCrLf
                            Else
                                cmdText &= " N'" & tparVirtualRotorPositionsRow.PatientID.Replace("'", "''") & "', " & vbCrLf
                            End If

                            If (tparVirtualRotorPositionsRow.IsOrderIDNull) Then
                                cmdText &= " NULL, " & vbCrLf
                            Else
                                cmdText &= " '" & tparVirtualRotorPositionsRow.OrderID & "', " & vbCrLf
                            End If

                            If (tparVirtualRotorPositionsRow.IsPredilutionFactorNull) Then
                                cmdText &= " NULL, " & vbCrLf
                            Else
                                cmdText &= ReplaceNumericString(tparVirtualRotorPositionsRow.PredilutionFactor) & ", " & vbCrLf
                            End If

                            If (tparVirtualRotorPositionsRow.IsStatusNull) Then
                                cmdText &= " NULL, " & vbCrLf
                            Else
                                cmdText &= " '" & tparVirtualRotorPositionsRow.Status.Trim & "', " & vbCrLf
                            End If

                            If (tparVirtualRotorPositionsRow.IsOnlyForISENull) Then
                                cmdText &= " 0, " & vbCrLf
                            Else
                                cmdText &= Convert.ToInt32(tparVirtualRotorPositionsRow.OnlyForISE) & ", " & vbCrLf
                            End If

                            'Barcode fields
                            If (tparVirtualRotorPositionsRow.IsScannedPositionNull) Then
                                cmdText &= " NULL, " & vbCrLf
                            Else
                                cmdText &= Convert.ToInt32(tparVirtualRotorPositionsRow.ScannedPosition) & ", " & vbCrLf
                            End If

                            If (tparVirtualRotorPositionsRow.IsBarcodeInfoNull) Then
                                cmdText &= " NULL, " & vbCrLf
                            Else
                                cmdText &= " N'" & tparVirtualRotorPositionsRow.BarcodeInfo.Replace("'", "''") & "', " & vbCrLf
                            End If

                            If (tparVirtualRotorPositionsRow.IsBarcodeStatusNull) Then
                                cmdText &= " NULL) " & vbCrLf
                            Else
                                cmdText &= " N'" & tparVirtualRotorPositionsRow.BarcodeStatus.Replace("'", "''") & "') " & vbCrLf
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
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksWSNotInUseRotorPositionsDAO.Create", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Delete the additional information of the Element placed in a Rotor Position with Status Not In Use 
        ''' (due to the Element was physically removed from the Rotor Position of was moved to other Position)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pRotorType">Rotor Type</param>
        ''' <param name="pRingNumber">Ring Number</param>
        ''' <param name="pCellNumber">Cell Number</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by: AG 21/01/2010 (Tested OK)
        ''' </remarks>
        Public Function Delete(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, _
                               ByVal pRotorType As String, ByVal pRingNumber As Integer, ByVal pCellNumber As Integer) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = ""
                    cmdText = " DELETE twksWSNotInUseRotorPositions " & vbCrLf & _
                              " WHERE  AnalyzerID = '" & pAnalyzerID.ToString().Replace("'", "''") & "' " & vbCrLf & _
                              " AND    WorkSessionID = '" & pWorkSessionID.ToString().Replace("'", "''") & "' " & vbCrLf & _
                              " AND    RotorType  = '" & pRotorType & "' " & vbCrLf & _
                              " AND    RingNumber = " & pRingNumber & vbCrLf & _
                              " AND    CellNumber = " & pCellNumber & vbCrLf

                    Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                        resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                        resultData.HasError = False
                    End Using
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksWSNotInUseRotorPositionsDAO.Delete", EventLogEntryType.Error, False)
            End Try
            Return resultData
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
        Public Function DeleteControl(ByVal pDBConnection As SqlClient.SqlConnection, _
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

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksWSNotInUseRotorPositionsDAO.DeleteControl", EventLogEntryType.Error, False)
            End Try
            Return dataToReturn
        End Function

        ''' <summary>
        ''' Reads a position containing a Not In Use Element in a Rotor used in a Work Session
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pRotorType">Rotor Type</param>
        ''' <param name="pRingNumber">Ring Number</param>
        ''' <param name="pCellNumber">Cell Number</param>
        ''' <returns>GlobalDataTO containing a typed DataSet VirtualRotorPositionsDS with the information of the content of the specified 
        '''          Not In Use Rotor Position</returns>
        ''' <remarks>
        ''' Created by: AG 19/01/2010 
        ''' </remarks>
        Public Function Read(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, _
                             ByVal pRotorType As String, ByVal pRingNumber As Integer, ByVal pCellNumber As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT * FROM twksWSNotInUseRotorPositions " & vbCrLf & _
                                                " WHERE  AnalyzerID    = '" & pAnalyzerID & "' " & vbCrLf & _
                                                " AND    WorkSessionID = '" & pWorkSessionID & "' " & vbCrLf & _
                                                " AND    RotorType     = '" & pRotorType & "' " & vbCrLf & _
                                                " AND    RingNumber = " & pRingNumber & vbCrLf & _
                                                " AND    CellNumber = " & pCellNumber

                        Dim resultCommand As New VirtualRotorPosititionsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(resultCommand.tparVirtualRotorPosititions)
                            End Using
                        End Using

                        resultData.SetDatos = resultCommand
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksWSNotInUseRotorPositionsDAO.Read", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Change the Analyzer identifier of the informed Analyzer WorkSession.
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <returns>GlobalDataTO containing Success/Error information</returns>
        ''' <remarks>
        ''' Created by:  XBC 12/06/2012
        ''' </remarks>
        Public Function UpdateWSAnalyzerID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    'There is not an Opened Database Connection...
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString

                Else
                    Dim cmdText As String = " UPDATE twksWSNotInUseRotorPositions " & vbCrLf & _
                                            " SET    AnalyzerID = '" & pAnalyzerID.Trim & "' " & vbCrLf & _
                                            " WHERE  WorkSessionID = '" & pWorkSessionID.Trim & "' " & vbCrLf


                    Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                        resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                        resultData.HasError = False
                    End Using
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksWSAnalyzersDAO.UpdateWSAnalyzerID", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function
#End Region

#Region "Other Methods"
        ''' <summary>
        ''' Get the number of bottles not in use in current worksession
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pRotorType">Rotor Type</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pReagentID" ></param>
        ''' <param name="pSolutionCode" ></param>
        ''' <returns>GlobalDataTO containing an Integer value with the total number of bottles in the Rotor</returns>
        ''' <remarks>
        ''' Created by: AG 30/08/2011
        ''' </remarks>
        Public Function GetPlacedTubesByPosition(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pRotorType As String, _
                                                 ByVal pWorkSessionID As String, ByVal pReagentID As Integer, ByVal pSolutionCode As String) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'SQL Sentence to get data
                        Dim cmdText As String = ""
                        If (pReagentID <> -1) Then
                            cmdText = " SELECT COUNT(MultiItemNumber) AS NumOfPlacedTubes " & vbCrLf & _
                                      " FROM   twksWSNotInUseRotorPositions " & vbCrLf & _
                                      " WHERE  AnalyzerID    = '" & pAnalyzerID & "' " & vbCrLf & _
                                      " AND    WorkSessionID = '" & pWorkSessionID & "' " & vbCrLf & _
                                      " AND    RotorType     = '" & pRotorType & "' " & vbCrLf & _
                                      " AND    ReagentID     = " & pReagentID & vbCrLf

                        ElseIf (pSolutionCode <> "") Then
                            cmdText = " SELECT COUNT(MultiItemNumber) AS NumOfPlacedTubes " & vbCrLf & _
                                      " FROM   twksWSNotInUseRotorPositions " & vbCrLf & _
                                      " WHERE  AnalyzerID    = '" & pAnalyzerID & "' " & vbCrLf & _
                                      " AND    WorkSessionID = '" & pWorkSessionID & "' " & vbCrLf & _
                                      " AND    RotorType     = '" & pRotorType & "' " & vbCrLf & _
                                      " AND    SolutionCode  = '" & pSolutionCode & "' " & vbCrLf
                        End If

                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            myGlobalDataTO.SetDatos = CType(dbCmd.ExecuteScalar(), Integer)
                            myGlobalDataTO.HasError = False
                        End Using
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksWSRotorContentByPositionDAO.GetPlacedTubesByPosition", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get all Not In Use Positions for the specified Work Session and Analyzer Rotor
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pRotorType">Rotor Type</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pFindPatients">When FALSE, not in use rotor positions containing Patient Samples are excluded from the search</param>
        ''' <returns>GlobalDataTO containing a typed DataSet VirtualRotorPositionsDS with the information 
        '''          of the content of all Not In Use Positions for the specified Analyzer Rotor and Work Session </returns>
        ''' <remarks>
        ''' Created by:  VR 25/01/2010 
        ''' Modified by: SA 15/02/2012 - Changed the query to get also fields RealVolume and Status from the Internal Virtual Rotor 
        '''                              containing the NOT IN USE Positions for the specified Rotor
        '''              SA 02/03/2012 - Changed the query to get also field TubeType from table of Virtual Rotor Positions
        '''              SA 28/03/2012 - Changed the query to remove INNER JOIN with table of Virtual Rotor Positions. This function is
        '''                              used also when scanning rotors and in that case not all NOT IN USE positions are in the Internal
        '''                              Virtual Rotor (they do not come from a previous saved Rotor).  Searching of data for positions
        '''                              included in the Internal Virtual Rotor is moved to the corresponding function in Delegate Class
        '''              SA 10/04/2013 - Changed the SQL Query to exclude from the searching all NOT IN USE Rotor Positions containing
        '''                              Patient Samples
        '''              SA 16/04/2013 - Added parameter to indicate if the function has to search also Patient Samples elements; changed 
        '''                              the SQL Query to exclude positions containing Patient Samples when value of the new parameter is FALSE
        ''' </remarks>
        Public Function GetRotorPositionsByWorkSession(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pRotorType As String, _
                                                       ByVal pWorkSessionID As String, ByVal pFindPatients As Boolean) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT NIU.*, RCP.TubeType, RCP.RealVolume, RCP.Status " & vbCrLf & _
                                                " FROM   twksWSNotInUseRotorPositions NIU INNER JOIN twksWSRotorContentByPosition RCP " & vbCrLf & _
                                                                                                " ON NIU.RotorType  = RCP.RotorType " & vbCrLf & _
                                                                                               " AND NIU.RingNumber = RCP.RingNumber " & vbCrLf & _
                                                                                               " AND NIU.CellNumber = RCP.CellNumber " & vbCrLf & _
                                                " WHERE  NIU.AnalyzerID    = '" & pAnalyzerID & "' " & vbCrLf & _
                                                " AND    NIU.RotorType     = '" & pRotorType & "' " & vbCrLf & _
                                                " AND    NIU.WorkSessionID = '" & pWorkSessionID & "' " & vbCrLf

                        'Exclude positions containing NOT IN USE Patient Samples when pFindPatients = FALSE; sort data by Ring and Cell Number 
                        If (Not pFindPatients) Then cmdText &= " AND NIU.TubeContent <> 'PATIENT' " & vbCrLf
                        cmdText &= " ORDER BY NIU.RingNumber, NIU.CellNumber "

                        Dim resultCommand As New VirtualRotorPosititionsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(resultCommand.tparVirtualRotorPosititions)
                            End Using
                        End Using

                        resultData.SetDatos = resultCommand
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksWSNotInUseRotorPositionsDAO.GetRotorPositionsByWorkSession", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Read all positions containing the same Identifier (NumericID or TextID, depending on the TubeContent)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pRotorType">Rotor Type</param>
        ''' <param name="pTubeContent">Type of element placed in the position</param>
        ''' <param name="pNumericID">Identifier of the element placed in the position. Depending on the TubeContent, it
        '''                          can be a ReagentID, CalibratorID or ControlID</param>
        ''' <param name="pTextID">Identifier of the element placed in the position. Depending on the TubeContent, it
        '''                       can be a Special Solution Code, a Washing Solution Code, a PatientID or an OrderID</param>
        ''' <param name="pSampleType">Sample Type Code</param>
        ''' <param name="pPatientOrOrder">When TubeContent is PATIENT, this flag indicates if the TextID containts a 
        '''                               PatientID (when True) or an OrderID (when False)</param>
        ''' <returns>GlobalDataTO containing a typed DataSet VirtualRotorPositionsDS with the list of positions found</returns>
        ''' <remarks>
        ''' Created by:  AG 22/01/2010 (Tested Pending)
        ''' Modified by: SA 26/10/2010 - Added N preffix for multilanguage when comparing by PatientID
        '''              RH 17/06/2011 - Added TUBE_SPEC_SOL and TUBE_WASH_SOL. Code optimization.
        ''' </remarks>
        Public Function ReadPositionsByIdentificator(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
                                                     ByVal pWorkSessionID As String, ByVal pRotorType As String, ByVal pTubeContent As String, _
                                                     ByVal pNumericID As Integer, ByVal pTextID As String, ByVal pSampleType As String, _
                                                     Optional ByVal pPatientOrOrder As Boolean = False) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT * FROM twksWSNotInUseRotorPositions " & vbCrLf & _
                                                " WHERE  AnalyzerID = '" & pAnalyzerID & "' " & vbCrLf & _
                                                " AND    WorkSessionID = '" & pWorkSessionID & "' " & vbCrLf & _
                                                " AND    RotorType = '" & pRotorType & "' " & vbCrLf & _
                                                " AND    TubeContent = '" & pTubeContent & "' " & vbCrLf

                        Select Case (pTubeContent)
                            Case "REAGENT"
                                cmdText &= " AND ReagentID = " & pNumericID & vbCrLf

                            Case "CALIB"
                                cmdText &= " AND CalibratorID = " & pNumericID & vbCrLf

                            Case "CONTROL"
                                cmdText &= " AND ControlID = " & pNumericID & vbCrLf

                            Case "SPEC_SOL", "WASH_SOL", "TUBE_SPEC_SOL", "TUBE_WASH_SOL"
                                cmdText &= " AND SolutionCode = '" & pTextID & "' " & vbCrLf

                            Case "PATIENT"
                                cmdText &= " AND SampleType = '" & pSampleType & "' " & vbCrLf

                                If (pPatientOrOrder) Then
                                    cmdText &= " AND PatientID = N'" & pTextID & "' " & vbCrLf
                                Else
                                    cmdText &= " AND OrderID = '" & pTextID & "' " & vbCrLf
                                End If
                        End Select

                        Dim resultCommand As New VirtualRotorPosititionsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(resultCommand.tparVirtualRotorPosititions)
                            End Using
                        End Using

                        resultData.SetDatos = resultCommand
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksWSNotInUseRotorPositionsDAO.ReadPositionsByIdentificator", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Delete information of all Positions with Status Not In Use in an specific Analyzer Rotor in a Work Session
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pRotorType">Rotor Type</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created By: AG 21/01/2010 (Tested OK)
        ''' </remarks>
        Public Function Reset(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, _
                              ByVal pRotorType As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = " DELETE twksWSNotInUseRotorPositions " & vbCrLf & _
                                            " WHERE  AnalyzerID = '" & pAnalyzerID.ToString().Replace("'", "''") & "' " & vbCrLf & _
                                            " AND    RotorType = '" & pRotorType.Trim & "' " & vbCrLf & _
                                            " AND    WorkSessionID = '" & pWorkSessionID.ToString().Replace("'", "''") & "' " & vbCrLf

                    Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                        resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                        resultData.HasError = False
                    End Using
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksWSNotInUseRotorPositionsDAO.Reset", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Delete all not in use Rotor Positions for the specified Work Session
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by: GDS 21/04/2010
        ''' </remarks>
        Public Function ResetWS(ByVal pDBConnection As SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = " DELETE twksWSNotInUseRotorPositions " & vbCrLf & _
                                            " WHERE  AnalyzerID = '" & pAnalyzerID.Trim.Replace("'", "''") & "' " & vbCrLf & _
                                            " AND    WorkSessionID = '" & pWorkSessionID.Trim.Replace("'", "''") & "' " & vbCrLf

                    Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                        resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                        resultData.HasError = False
                    End Using
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksWSNotInUseRotorPositionsDAO.ResetWS", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Update field Status = NULL for the specified NOT IN USE Rotor Positions (by CellNumber or by CalibratorID)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">WorkSession Identifier</param>
        ''' <param name="pRotorType">Rotor Type (Samples/Reagents)</param>
        ''' <param name="pCellNumber">Optional parameter. When informed, it indicates the number of NOT IN USE Rotor Cell to update</param>
        ''' <param name="pCalibratorID">Optional parameter. When informed, it indicates that all the NOT IN USE Rotor Cells containing a tube
        '''                              of the specified Calibrator will be updated</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 10/02/2012
        ''' </remarks>
        Public Function SetStatusToNull(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, _
                                        ByVal pRotorType As String, Optional ByVal pCellNumber As Integer = -1, Optional ByVal pCalibratorID As Integer = -1) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = " UPDATE twksWSNotInUseRotorPositions SET Status = NULL " & vbCrLf & _
                                            " WHERE  AnalyzerID    = '" & pAnalyzerID.Trim & "' " & vbCrLf & _
                                            " AND    WorkSessionID = '" & pWorkSessionID.Trim & "' " & vbCrLf & _
                                            " AND    RotorType     = '" & pRotorType.Trim & "' " & vbCrLf

                    If (pCalibratorID = -1) Then
                        'Update the specified NOT IN USE position
                        cmdText &= " AND    CellNumber    = " & pCellNumber.ToString & vbCrLf
                    Else
                        'Update all NOT IN USE positions containing the specified Calibrator
                        cmdText &= " AND CalibratorID = " & pCalibratorID & vbCrLf
                    End If

                    Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                        resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                        resultData.HasError = False
                    End Using
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksWSNotInUseRotorPositionsDAO.SetStatusToNull", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Update the Sample Type of the specified Rotor Position 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">WorkSession Identifier</param>
        ''' <param name="pRotorType">Rotor Type (Samples/Reagents)</param>
        ''' <param name="pCellNumber">Cell Number</param>
        ''' <param name="pSampleType">Sample Type Code</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  TR 21/09/2011
        ''' Modified by: SA 28/05/2013 - If SampleType is not informed, set its value to NULL
        ''' </remarks>
        Public Function UpdateSampleType(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, _
                                         ByVal pRotorType As String, ByVal pCellNumber As Integer, ByVal pSampleType As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = " UPDATE twksWSNotInUseRotorPositions " & Environment.NewLine

                    If (pSampleType <> String.Empty) Then
                        cmdText &= " SET SampleType = '" & pSampleType.Trim & "'" & Environment.NewLine
                    Else
                        cmdText &= " SET SampleType = NULL " & Environment.NewLine
                    End If

                    cmdText &= " WHERE  AnalyzerID    = '" & pAnalyzerID.ToString().Replace("'", "''") & "' " & Environment.NewLine
                    cmdText &= " AND    RotorType     = '" & pRotorType.Trim & "' " & Environment.NewLine
                    cmdText &= " AND    WorkSessionID = '" & pWorkSessionID.Trim & "' " & Environment.NewLine
                    cmdText &= " AND    CellNumber    = " & pCellNumber.ToString & Environment.NewLine

                    Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                        resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                        resultData.HasError = False
                    End Using
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksWSNotInUseRotorPositionsDAO.UpdateSampleType", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Delete elements not in use when the barcode configuration change
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pAnalyzerID">Analyzer ID</param>
        ''' <param name="pWorkSessionID">WorkSession ID</param>
        ''' <param name="pRotorType">Rotor Type</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by: TR 26/04/2013
        ''' </remarks>
        Public Function DeleteNotInUsePatients(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
                                                          ByVal pWorkSessionID As String, ByVal pRotorType As GlobalEnumerates.Rotors) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = " DELETE twksWSNotInUseRotorPositions " & Environment.NewLine
                    cmdText &= "              WHERE  AnalyzerID    = '" & pAnalyzerID.ToString().Replace("'", "''") & "' " & Environment.NewLine
                    cmdText &= "              AND    RotorType     = '" & pRotorType.ToString() & "' " & Environment.NewLine
                    cmdText &= "              AND    WorkSessionID = '" & pWorkSessionID.ToString().Replace("'", "''") & "' " & Environment.NewLine
                    cmdText &= "              AND    BarcodeInfo IS NOT NULL " & Environment.NewLine
                    cmdText &= "              AND    TubeContent = 'PATIENT' " & Environment.NewLine

                    Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                        resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                        resultData.HasError = False
                    End Using
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksWSNotInUseRotorPositionsDAO.DeleteNotInUseBarcodeConfigChange", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function
#End Region
    End Class
End Namespace
