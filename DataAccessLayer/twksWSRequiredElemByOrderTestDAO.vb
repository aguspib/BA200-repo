Option Explicit On
Option Strict On

Imports System.Data.SqlClient
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global

Namespace Biosystems.Ax00.DAL.DAO

    Partial Public Class twksWSRequiredElemByOrderTestDAO
          

#Region "CRUD Methods"
        ''' <summary>
        ''' Add a new link between an Order Test and a required WS Element
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pOrderTestID">Identifier of an Order Test requested in the WS</param>
        ''' <param name="pElementID">Identifier of a required WS Element</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:
        ''' Modified by: RH 13/04/2010 - Added parameter pStatFlag
        ''' </remarks>
        Public Function Create(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOrderTestID As Integer, ByVal pElementID As Integer, _
                               ByVal pStatFlag As Boolean) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = String.Format(" INSERT INTO twksWSRequiredElemByOrderTest(OrderTestID, ElementID, StatFlag) " & _
                                                          " VALUES ({0}, {1}, {2}) ", pOrderTestID, pElementID, IIf(pStatFlag, 1, 0))

                    'Execute the SQL sentence 
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
                GlobalBase.CreateLogActivity(ex.Message, "twksWSRequiredElemByOrderTestDAO.Create", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Verify if the link between and Order Test and a required WS Element has been already created
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pOrderTestID">Identifier of an Order Test requested in the WS</param>
        ''' <param name="pElementID">Identifier of a WS required Element</param>
        ''' <returns>GlobalDataTO containing a typed DataSet WSRequiredElemByOrderTestDS with data of the link</returns>
        ''' <remarks>
        ''' Created by:  SA 12/04/2012
        ''' </remarks>
        Public Function Read(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOrderTestID As Integer, ByVal pElementID As Integer) As GlobalDataTO
            Dim dataToReturn As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                dataToReturn = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not dataToReturn.HasError AndAlso Not dataToReturn.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(dataToReturn.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT * FROM twksWSRequiredElemByOrderTest " & vbCrLf & _
                                                " WHERE  OrderTestID = " & pOrderTestID.ToString & vbCrLf & _
                                                " AND    ElementID = " & pElementID.ToString & vbCrLf

                        Dim resultData As New WSRequiredElemByOrderTestDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(resultData.twksWSRequiredElemByOrderTest)
                            End Using
                        End Using

                        dataToReturn.SetDatos = resultData
                        dataToReturn.HasError = False
                    End If
                End If
            Catch ex As Exception
                dataToReturn = New GlobalDataTO()
                dataToReturn.HasError = True
                dataToReturn.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                dataToReturn.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksWSRequiredElemByOrderTestDAO.Read", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return dataToReturn
        End Function

        ''' <summary>
        ''' Get all the OrderTestIDs related with the informed required WS Element
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pElementID">Identifier of a WS required Element</param>
        ''' <returns>GlobalDataTO containing a typed DataSet WSRequiredElemByOrderTestDS with all OrderTestIDs related with 
        '''          the specified required WS Element</returns>
        ''' <remarks>
        ''' Created by:  DL 05/01/2011
        ''' </remarks>
        Public Function ReadByElementID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pElementID As Integer) As GlobalDataTO
            Dim dataToReturn As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                dataToReturn = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not dataToReturn.HasError AndAlso Not dataToReturn.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(dataToReturn.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT * FROM twksWSRequiredElemByOrderTest " & vbCrLf & _
                                                " WHERE  ElementID = " & pElementID & vbCrLf

                        Dim resultData As New WSRequiredElemByOrderTestDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(resultData.twksWSRequiredElemByOrderTest)
                            End Using
                        End Using

                        dataToReturn.SetDatos = resultData
                        dataToReturn.HasError = False
                    End If
                End If
            Catch ex As Exception
                dataToReturn = New GlobalDataTO()
                dataToReturn.HasError = True
                dataToReturn.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                dataToReturn.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksWSRequiredElemByOrderTestDAO.ReadByElementID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return dataToReturn
        End Function

        ''' <summary>
        ''' Get all OrderTestIDs of the specified Sample Class linked with the informed required WS Element
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pElementID">Identifier of a WS required Element</param>
        ''' <param name="pSampleClass">Sample Class Code</param>
        ''' <returns>GlobalDataTO containing a typed DataSet WSRequiredElemByOrderTestDS with all OrderTestIDs of the specified
        '''          Sample Class linked to the informed required WS Element</returns>
        ''' <remarks>
        ''' Created by:  AG 04/04/2011 - Created based on ReadByElementID
        ''' </remarks>
        Public Function ReadByElementIDAndSampleClass(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pElementID As Integer, _
                                                      ByVal pSampleClass As String) As GlobalDataTO
            Dim dataToReturn As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                dataToReturn = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not dataToReturn.HasError AndAlso Not dataToReturn.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(dataToReturn.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT RQ.OrderTestID, RQ.ElementID, RQ.StatFlag " & vbCrLf & _
                                                " FROM   twksWSRequiredElemByOrderTest RQ INNER JOIN twksOrderTests OT ON RQ.OrderTestID = OT.OrderTestID " & vbCrLf & _
                                                                                        " INNER JOIN twksOrders O ON OT.OrderID = O.OrderID " & vbCrLf & _
                                                " WHERE  RQ.ElementID = " & pElementID.ToString & vbCrLf & _
                                                " AND    O.SampleClass = '" & pSampleClass.Trim & "' " & vbCrLf

                        Dim resultData As New WSRequiredElemByOrderTestDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(resultData.twksWSRequiredElemByOrderTest)
                            End Using
                        End Using

                        dataToReturn.SetDatos = resultData
                        dataToReturn.HasError = False
                    End If
                End If
            Catch ex As Exception
                dataToReturn = New GlobalDataTO()
                dataToReturn.HasError = True
                dataToReturn.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                dataToReturn.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksWSRequiredElemByOrderTestDAO.ReadByElementIDAndSampleClass", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return dataToReturn
        End Function
#End Region

#Region "Others Methods"
        ''' <summary>
        ''' For every Order Test to be executed in the specified WorkSession Analyzer, get the related Calibrator Elements and create  
        ''' the relation (only for Experimental Calibrators, including those defined for Special Tests)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">WorkSession Identifier</param>
        ''' <param name="pPointNumber">When informed, indicates the query has to be filtered by the specified Multipoint Number instead of
        '''                            get all Calibrator Points. Optional parameter that will be informed only for Special Test HbTotal</param>
        ''' <param name="pTestID">Test Identifier. Optional parameter that will be informed only for Special Tests</param>
        ''' <param name="pSampleType">Sample Type Code. Optional parameter that will be informed only for Special Tests</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 16/02/2011
        ''' </remarks>
        Public Function InsertRelationsForCalibrators(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, Optional ByVal pPointNumber As Integer = -1, _
                                                      Optional ByVal pTestID As Integer = -1, Optional ByVal pSampleType As String = "") As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = " INSERT INTO twksWSRequiredElemByOrderTest (OrderTestID, ElementID, StatFlag) " & vbCrLf & _
                                            " SELECT WSOT.OrderTestID, RE.ElementID,  WSOT.StatFlag " & vbCrLf & _
                                            " FROM   vwksWSOrderTests WSOT INNER JOIN tparTestCalibrators TC ON TC.TestID = WSOT.TestID " & vbCrLf & _
                                                                                                          " AND TC.SampleType = WSOT.SampleType " & vbCrLf & _
                                                                         " INNER JOIN twksWSRequiredElements RE ON TC.CalibratorID = RE.CalibratorID " & vbCrLf & _
                                                                                                             " AND RE.TubeContent = 'CALIB' " & vbCrLf & _
                                            " WHERE  WSOT.WorkSessionID   = '" & pWorkSessionID.Trim & "' " & vbCrLf & _
                                            " AND    WSOT.SampleClass    <> 'BLANK' " & vbCrLf & _
                                            " AND    WSOT.OrderTestStatus = 'OPEN' " & vbCrLf & _
                                            " AND    WSOT.TestType        = 'STD' " & vbCrLf & _
                                            " AND    WSOT.ToSendFlag      = 1 " & vbCrLf & _
                                            " AND    WSOT.OpenOTFlag      = 0 " & vbCrLf

                    'Add additional filter when the function was called for Special Test HbTotal
                    If (pPointNumber <> -1) Then cmdText &= " AND RE.MultiItemNumber = " & pPointNumber & vbCrLf

                    'Filter for the specified Test/SampleType (for Special Tests)
                    If (pTestID <> -1) And (pSampleType.Trim <> "") Then
                        cmdText &= " AND WSOT.TestID     = " & pTestID & vbCrLf & _
                                   " AND WSOT.SampleType = '" & pSampleType & "' "
                    Else
                        'If an specific Test/Sample Type was not informed, then exclude Special Tests (general case)
                        cmdText &= " AND WSOT.SpecialTest = 0 "
                    End If

                    Using dbCmd As New SqlCommand(cmdText, pDBConnection)
                        resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                        resultData.HasError = False
                    End Using
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksWSRequiredElemByOrderTestDAO.InsertRelationsForCalibrators", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' For every Order Test to be executed in the specified WorkSession Analyzer, get the related Control Elements and create the relation
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">WorkSession Identifier</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 16/02/2011
        ''' Modified by: SA 19/06/2012 - Changed the SQL: the INNER JOIN with tparTestControls has to include the TestType field; besides, the filter
        '''                              by TestType has to include also the ISE Tests
        ''' </remarks>
        Public Function InsertRelationsForControls(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = " INSERT INTO twksWSRequiredElemByOrderTest (OrderTestID, ElementID, StatFlag) " & vbCrLf & _
                                            " SELECT WSOT.OrderTestID, RE.ElementID,  WSOT.StatFlag " & vbCrLf & _
                                            " FROM   vwksWSOrderTests WSOT INNER JOIN tparTestControls TC ON WSOT.TestType   = TC.TestType " & vbCrLf & _
                                                                                                       " AND WSOT.TestID     = TC.TestID " & vbCrLf & _
                                                                                                       " AND WSOT.SampleType = TC.SampleType " & vbCrLf & _
                                                                                                       " AND WSOT.ControlID  = TC.ControlID " & vbCrLf & _
                                                                         " INNER JOIN twksWSRequiredElements RE ON TC.ControlID   = RE.ControlID " & vbCrLf & _
                                                                                                             " AND RE.TubeContent = 'CTRL' " & vbCrLf & _
                                            " WHERE  WSOT.WorkSessionID   = '" & pWorkSessionID.Trim & "' " & vbCrLf & _
                                            " AND    WSOT.OrderTestStatus = 'OPEN' " & vbCrLf & _
                                            " AND    WSOT.SampleClass     = 'CTRL' " & vbCrLf & _
                                            " AND    WSOT.TestType       IN ('STD', 'ISE') " & vbCrLf & _
                                            " AND    WSOT.ToSendFlag      = 1 " & vbCrLf & _
                                            " AND    WSOT.OpenOTFlag      = 0 "

                    Using dbCmd As New SqlCommand(cmdText, pDBConnection)
                        resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                        resultData.HasError = False
                    End Using
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksWSRequiredElemByOrderTestDAO.InsertRelationsForControls", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' For every Order Test to be executed in the specified WorkSession Analyzer, get the related Patient Samples Elements 
        ''' and create the relation 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">WorkSession Identifier</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 16/02/2011
        ''' Modified by: SA 20/04/2011 - Changed the SELECT part of the query; there were problems with Predilutions in the old one
        ''' </remarks>
        Public Function InsertRelationsForPatientSamples(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim fixedSelect As String = " SELECT WSOT.OrderTestID, RE.ElementID,  WSOT.StatFlag " & vbCrLf & _
                                                " FROM   vwksWSOrderTests WSOT INNER JOIN twksWSRequiredElements RE " & vbCrLf & _
                                                                                 " ON ((WSOT.SampleType = RE.SampleType AND RE.PatientID IS NULL AND WSOT.SampleID = RE.SampleID) " & vbCrLf & _
                                                                                 " OR  (WSOT.SampleType = RE.SampleType AND RE.SampleID IS NULL AND WSOT.PatientID = RE.PatientID)) " & vbCrLf & _
                                                                                 " AND RE.TubeContent = 'PATIENT' " & vbCrLf & _
                                                " WHERE  WSOT.WorkSessionID   = '" & pWorkSessionID.Trim & "' " & vbCrLf & _
                                                " AND    WSOT.SampleClass     = 'PATIENT' " & vbCrLf & _
                                                " AND    WSOT.OrderTestStatus = 'OPEN' " & vbCrLf & _
                                                " AND    WSOT.ToSendFlag      = 1 " & vbCrLf & _
                                                " AND    WSOT.OpenOTFlag      = 0 " & vbCrLf

                    Dim cmdText As String = " INSERT INTO twksWSRequiredElemByOrderTest (OrderTestID, ElementID, StatFlag) " & vbCrLf & _
                                            fixedSelect & vbCrLf & _
                                            " AND  ((WSOT.PredilutionUseFlag = 0) OR (WSOT.PredilutionUseFlag = 1 AND WSOT.PredilutionMode = 'INST')) " & vbCrLf & _
                                            " AND    RE.PredilutionFactor IS NULL " & vbCrLf & _
                                            " UNION " & vbCrLf & _
                                            fixedSelect & vbCrLf & _
                                            " AND   (WSOT.PredilutionUseFlag = 1 AND WSOT.PredilutionMode = 'USER') " & vbCrLf & _
                                            " AND   (RE.PredilutionFactor IS NOT NULL AND RE.PredilutionFactor = WSOT.PredilutionFactor) "

                    Using dbCmd As New SqlCommand(cmdText, pDBConnection)
                        resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                        resultData.HasError = False
                    End Using
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksWSRequiredElemByOrderTestDAO.InsertRelationsForPatientSamples", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' For every Order Test to be executed in the specified WorkSession Analyzer, get the related Reagent Elements 
        ''' and create the relation
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">WorkSession Identifier</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 16/02/2011
        ''' </remarks>
        Public Function InsertRelationsForReagents(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = " INSERT INTO twksWSRequiredElemByOrderTest (OrderTestID, ElementID, StatFlag) " & vbCrLf & _
                                            " SELECT WSOT.OrderTestID, RE.ElementID,  WSOT.StatFlag " & vbCrLf & _
                                            " FROM   vwksWSOrderTests WSOT INNER JOIN tparTestReagents TR ON WSOT.TestID = TR.TestID " & vbCrLf & _
                                                                         " INNER JOIN twksWSRequiredElements RE ON TR.ReagentID     = RE.ReagentID " & vbCrLf & _
                                                                                                             " AND TR.ReagentNumber = RE.MultiItemNumber " & vbCrLf & _
                                                                                                             " AND RE.TubeContent   = 'REAGENT' " & vbCrLf & _
                                            " WHERE  WSOT.WorkSessionID   = '" & pWorkSessionID.Trim & "' " & vbCrLf & _
                                            " AND    WSOT.OrderTestStatus = 'OPEN' " & vbCrLf & _
                                            " AND    WSOT.TestType        = 'STD' " & vbCrLf & _
                                            " AND    WSOT.ToSendFlag      = 1 " & vbCrLf & _
                                            " AND    WSOT.OpenOTFlag      = 0 "

                    Using dbCmd As New SqlCommand(cmdText, pDBConnection)
                        resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                        resultData.HasError = False
                    End Using
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksWSRequiredElemByOrderTestDAO.InsertRelationsForReagents", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Delete all information for the Work Session
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pDelOnlyOpened">When True indicates that only relations of Order Tests with
        '''                              status OPEN should be deleted; optional parameter</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by:  GDS 21/04/2010
        ''' Modified by: SA  25/08/2010 - Added new optional parameter to indicate if only relations
        '''                               of Order Tests with status OPEN have to be deleted
        ''' </remarks>
        Public Function ResetWS(ByVal pDBConnection As SqlConnection, ByVal pWorkSessionID As String, _
                                Optional ByVal pDelOnlyOpened As Boolean = False) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String
                    'AJG
                    'cmdText = " DELETE twksWSRequiredElemByOrderTest " & vbCrLf & _
                    '          " WHERE  ElementID IN (SELECT ElementID " & vbCrLf & _
                    '                               " FROM   twksWSRequiredElements " & vbCrLf & _
                    '                               " WHERE  WorkSessionID = '" & pWorkSessionID.Trim.Replace("'", "''") & "') " & vbCrLf

                    cmdText = " DELETE twksWSRequiredElemByOrderTest " & vbCrLf & _
                              " WHERE  EXISTS (SELECT ElementID " & vbCrLf & _
                                                   " FROM   twksWSRequiredElements " & vbCrLf & _
                                                   " WHERE  WorkSessionID = '" & pWorkSessionID.Trim.Replace("'", "''") & "' AND twksWSRequiredElemByOrderTest.ElementID = ElementID) " & vbCrLf

                    If (pDelOnlyOpened) Then
                        'AJG
                        'cmdText &= " AND OrderTestID IN (SELECT WSOT.OrderTestID " & vbCrLf & _
                        '                               " FROM   twksWSOrderTests WSOT INNER JOIN twksOrderTests OT ON WSOT.OrderTestID = OT.OrderTestID " & vbCrLf & _
                        '                               " WHERE  WSOT.WorkSessionID = '" & pWorkSessionID.Trim.Replace("'", "''") & "' " & vbCrLf & _
                        '                               " AND    OT.OrderTestStatus = 'OPEN') " & vbCrLf
                        cmdText &= " AND EXISTS (SELECT WSOT.OrderTestID " & vbCrLf & _
                                                " FROM   twksWSOrderTests WSOT INNER JOIN twksOrderTests OT ON WSOT.OrderTestID = OT.OrderTestID " & vbCrLf & _
                                                " WHERE  WSOT.WorkSessionID = '" & pWorkSessionID.Trim.Replace("'", "''") & "' " & vbCrLf & _
                                                " AND    OT.OrderTestStatus = 'OPEN' AND twksWSRequiredElemByOrderTest.OrderTestID = WSOT.OrderTestID) " & vbCrLf
                    End If

                    Using dbCmd As New SqlCommand(cmdText, pDBConnection)
                        resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                        resultData.HasError = False
                    End Using
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksWSRequiredElemByOrderTestDAO.ResetWS", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' For every OPEN Order Test to be executed in the specified WorkSession, get the related Special Solutions Elements 
        ''' and create the relation. Special Solutions will be:
        '''   ** The needed to execute the Blank for the Test (BlankMode in tparTests)
        '''   ** For Patient Samples of Tests/SampleTypes with automatic Predilution, the solution used as diluent (DiluentSolution
        '''      in tparTestSamples when PredilutionUseFlag is True and PredilutionMode is INST)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">WorkSession Identifier</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 16/02/2011
        ''' Modified by: RH 17/06/2011 Added subquery for TubeContent = TUBE_SPEC_SOL
        ''' </remarks>
        Public Function InsertRelationsForSpecialSolutions(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = " INSERT INTO twksWSRequiredElemByOrderTest (OrderTestID, ElementID, StatFlag) " & vbCrLf & _
                                            " SELECT WSOT.OrderTestID, RE.ElementID,  WSOT.StatFlag " & vbCrLf & _
                                            " FROM   vwksWSOrderTests WSOT INNER JOIN tparTests T ON  WSOT.TestID = T.TestID " & vbCrLf & _
                                                                         " INNER JOIN twksWSRequiredElements RE ON T.BlankMode = RE.SolutionCode " & vbCrLf & _
                                                                                                             " AND RE.TubeContent = 'TUBE_SPEC_SOL' " & vbCrLf & _
                                            " WHERE  WSOT.WorkSessionID   = '" & pWorkSessionID.Trim & "' " & vbCrLf & _
                                            " AND    WSOT.OrderTestStatus = 'OPEN' " & vbCrLf & _
                                            " AND    WSOT.TestType        = 'STD' " & vbCrLf & _
                                            " AND    WSOT.ToSendFlag      = 1 " & vbCrLf & _
                                            " AND    WSOT.OpenOTFlag      = 0 " & vbCrLf & _
                                            " UNION " & vbCrLf & _
                                            " SELECT WSOT.OrderTestID, RE.ElementID,  WSOT.StatFlag " & vbCrLf & _
                                            " FROM   vwksWSOrderTests WSOT INNER JOIN tparTestSamples TS ON  WSOT.TestID = TS.TestID AND WSOT.SampleType = TS.SampleType " & vbCrLf & _
                                                                         " INNER JOIN twksWSRequiredElements RE ON TS.DiluentSolution = RE.SolutionCode " & vbCrLf & _
                                                                                                             " AND RE.TubeContent = 'SPEC_SOL' " & vbCrLf & _
                                            " WHERE  WSOT.WorkSessionID      = '" & pWorkSessionID.Trim & "' " & vbCrLf & _
                                            " AND    WSOT.OrderTestStatus    = 'OPEN' " & vbCrLf & _
                                            " AND    WSOT.SampleClass        = 'PATIENT' " & vbCrLf & _
                                            " AND    WSOT.TestType           = 'STD' " & vbCrLf & _
                                            " AND    WSOT.ToSendFlag         = 1 " & vbCrLf & _
                                            " AND    WSOT.OpenOTFlag         = 0 " & vbCrLf & _
                                            " AND    WSOT.PredilutionUseFlag = 1 " & vbCrLf & _
                                            " AND    WSOT.PredilutionMode    = 'INST' "

                    Using dbCmd As New SqlCommand(cmdText, pDBConnection)
                        resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                        resultData.HasError = False
                    End Using
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksWSRequiredElemByOrderTestDAO.InsertRelationsForSpecialSolutions", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' For every OPEN Order Test to be executed in the specified WorkSession, get the related Washing Solutions Elements 
        ''' and create the relation. Washing Solutions will be:
        '''   ** Every ReagentContaminatorID in use in the WorkSession needs his washing solution
        '''   ** Apply only for STD tests
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">WorkSession Identifier</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by:  AG 21/06/2011 - Based on InsertRelationsForSpecialSolutions
        ''' Modified by: SA 22/06/2011 - Removed filter tparTests.InUse=True
        '''              SA 28/05/2014 - BT #1519 ==> Changed the query to create a relation between Order Tests and required Elements for Washing Solutions when 
        '''                                           the Order Test is for the Contaminated Test (the previous query create the relation only for Order Tests that 
        '''                                           are for the Contaminant Test). This change is to allow show the Washing Solution in the Warning of Not Positioned
        '''                                           Elements when whatever of the two Tests (the Contaminant and the Contaminated) are not finished (CLOSED)
        '''              SA 03/06/2014 - Undo last change. THIS FUNCTION IS NOT USED ANY MORE.
        ''' </remarks>
        Public Function InsertRelationsForWashingSolutions(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = " INSERT INTO twksWSRequiredElemByOrderTest (OrderTestID, ElementID, StatFlag) " & vbCrLf & _
                                            " SELECT DISTINCT OT.OrderTestID, RE.ElementID,  O.StatFlag " & vbCrLf & _
                                            " FROM   twksWSRequiredElements RE INNER JOIN tparContaminations CT " & vbCrLf & _
                                                                                     " ON (RE.SolutionCode = CT.WashingSolutionR1 OR RE.SolutionCode = CT.WashingSolutionR2) " & vbCrLf & _
                                                                             " INNER JOIN tparTestReagents TR ON CT.ReagentContaminatorID = TR.ReagentID " & vbCrLf & _
                                                                             " INNER JOIN twksOrderTests OT ON TR.TestID = OT.TestID " & vbCrLf & _
                                                                             " INNER JOIN twksOrders O ON OT.OrderID = O.OrderID " & vbCrLf & _
                                                                             " INNER JOIN twksWSOrderTests WSOT ON OT.OrderTestID = WSOT.OrderTestID " & vbCrLf & _
                                            " WHERE  RE.TubeContent       = 'WASH_SOL' " & vbCrLf & _
                                            " AND    OT.OrderTestStatus   = 'OPEN' " & vbCrLf & _
                                            " AND    OT.TestType          = 'STD' " & vbCrLf & _
                                            " AND    WSOT.WorkSessionID   = '" & pWorkSessionID & "' " & vbCrLf & _
                                            " AND    WSOT.ToSendFlag      = 1 " & vbCrLf & _
                                            " AND    WSOT.OpenOTFlag      = 0 " & vbCrLf

                    Using dbCmd As New SqlCommand(cmdText, pDBConnection)
                        resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                        resultData.HasError = False
                    End Using
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksWSRequiredElemByOrderTestDAO.InsertRelationsForWashingSolutions", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function
#End Region

#Region "TO REVIEW - DELETE"
        '''' <summary>
        '''' NOT USED: the relation is created by function InsertRelationsForCalibrators
        '''' 
        '''' For every Order Test to be executed in the specified WorkSession Analyzer for a Test/SampleType using an Alternative
        '''' Calibrator, create the relations with the Calibrator for the Test with the Alternative SampleType
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pWorkSessionID">WorkSession Identifier</param>
        '''' <param name="pElementIDs">Typed DataSet WSRequiredElementsDS with the list of Identifiers of the Calibrator in the table 
        ''''                           of Required WS Elements (one for each Calibrator point)</param>
        '''' <param name="pTestID">Test Identifier</param>
        '''' <param name="pSampleType">Sample Type</param>
        '''' <returns>GlobalDataTO containing sucess/error information</returns>
        '''' <remarks>
        '''' Created by:  SA 16/02/2011
        '''' </remarks>
        'Public Function InsertRelationsForAlternativeCalibrators(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, ByVal pElementIDs As WSRequiredElementsDS, _
        '                                                         ByVal pTestID As Integer, ByVal pSampleType As String) As GlobalDataTO
        '    Dim resultData As New GlobalDataTO
        '    Try
        '        If (pDBConnection Is Nothing) Then
        '            resultData.HasError = True
        '            resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
        '        Else
        '            Dim cmdText As String
        '            For Each elementID As WSRequiredElementsDS.twksWSRequiredElementsRow In pElementIDs.twksWSRequiredElements
        '                cmdText = " INSERT INTO twksWSRequiredElemByOrderTest (OrderTestID, ElementID, StatFlag) " & vbCrLf & _
        '                          " SELECT WSOT.OrderTestID, " & elementID.ElementID.ToString & " AS ElementID,  WSOT.StatFlag " & vbCrLf & _
        '                          " FROM   vwksWSOrderTests WSOT " & vbCrLf & _
        '                          " WHERE  WSOT.WorkSessionID   = '" & pWorkSessionID & "' " & vbCrLf & _
        '                          " AND    WSOT.OrderTestStatus = 'OPEN' " & vbCrLf & _
        '                          " AND    WSOT.SampleClass IN ('PATIENT', 'CTRL') " & vbCrLf & _
        '                          " AND    WSOT.TestType        = 'STD' " & vbCrLf & _
        '                          " AND    WSOT.ToSendFlag      = 1 " & vbCrLf & _
        '                          " AND    WSOT.OpenOTFlag      = 0 " & vbCrLf & _
        '                          " AND    WSOT.TestID          = " & pTestID & vbCrLf & _
        '                          " AND    WSOT.SampleType      = '" & pSampleType & "' " & vbCrLf

        '                Using dbCmd As New SqlCommand(cmdText, pDBConnection)
        '                    resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
        '                    resultData.HasError = False
        '                End Using
        '            Next
        '        End If
        '    Catch ex As Exception
        '        resultData.HasError = True
        '        resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        resultData.ErrorMessage = ex.Message

        '        'Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "twksWSRequiredElemByOrderTestDAO.InsertRelationsForAlternativeCalibrators", EventLogEntryType.Error, False)
        '    End Try
        '    Return resultData
        'End Function

        '''' <summary>
        '''' NOT USED: the ISE Washing Solution is not required for execution of ISE Tests requested for Patients
        '''' 
        '''' For every OPEN Order Test to be executed in the specified WorkSession, get the related Washing Solutions Elements 
        '''' and create the relation. Washing Solutions will be:
        ''''   ** ISE TESTs requires the ISE wash solution
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pWorkSessionID">WorkSession Identifier</param>
        '''' <returns>GlobalDataTO containing sucess/error information</returns>
        '''' <remarks>
        '''' Created by:  AG 23/06/2011 (based on InsertRelationsForSpecialSolutions)
        '''' </remarks>
        'Public Function InsertRelationsForISEWashingSolutions(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String) As GlobalDataTO
        '    Dim resultData As New GlobalDataTO

        '    Try
        '        If (pDBConnection Is Nothing) Then
        '            resultData.HasError = True
        '            resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
        '        Else
        '            Dim cmdText As String = ""
        '            cmdText = " INSERT INTO twksWSRequiredElemByOrderTest (OrderTestID, ElementID, StatFlag) " & vbCrLf & _
        '                      " SELECT DISTINCT WSOT.OrderTestID, RE.ElementID,  WSOT.StatFlag " & vbCrLf & _
        '                      " FROM   twksWSRequiredElements RE INNER JOIN vwksWSOrderTests WSOT ON WSOT.TestType = 'ISE' " & vbCrLf & _
        '                      " WHERE  RE.TubeContent       = 'TUBE_WASH_SOL' " & vbCrLf & _
        '                      " AND    WSOT.WorkSessionID   = '" & pWorkSessionID & "' " & vbCrLf & _
        '                      " AND    WSOT.OrderTestStatus = 'OPEN' " & vbCrLf & _
        '                      " AND    WSOT.ToSendFlag      = 1 " & vbCrLf & _
        '                      " AND    WSOT.OpenOTFlag      = 0 "

        '            Using dbCmd As New SqlCommand(cmdText, pDBConnection)
        '                resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
        '                resultData.HasError = False
        '            End Using
        '        End If
        '    Catch ex As Exception
        '        resultData.HasError = True
        '        resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        resultData.ErrorMessage = ex.Message

        '        'Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "twksWSRequiredElemByOrderTestDAO.InsertRelationsForISEWashingSolutions", EventLogEntryType.Error, False)
        '    End Try
        '    Return resultData
        'End Function
#End Region
    End Class
End Namespace
