Option Explicit On
Option Strict On

Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Global.GlobalConstants
Imports Biosystems.Ax00.Global.GlobalEnumerates

Namespace Biosystems.Ax00.BL

    Partial Public Class BarcodeWSDelegate

#Region "Class variables definition"
        'List of Element IDs for Reagents with BarCode that belong to the WorkSession
        Dim reagentsElemIdList As New List(Of Integer)

        'BT #1552 - List of Element IDs for Special Solutions with BarCode that belong to the WorkSession (SA 26/03/2014)
        Dim specSolsElemIdList As New List(Of Integer)

        'Class variables for managing the case of manual Barcode entry 
        Dim manualEntryTubeType As String = ""
        Dim manualEntryFlag As Boolean = False
#End Region

#Region "Manage barcode instruction methods LEVEL #1 (Public)"
        ''' <summary>
        ''' Implements the business related with the reception of a barcode read instruction in Sample / Reagents Rotor
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pBarCodeRotorContentDS">Typed DS WSRotorContentByPositionDS</param>
        ''' <param name="pRotorType">Rotor Type</param>
        ''' <returns>RotorContentByPositionDS with new rotor type contents (inside a GlobalDataTo)</returns>
        ''' <remarks>
        ''' Created by:  AG 02/08/2011
        ''' Modified by: SA 02/09/2011  - When there is not an active WS, a new one with EMPTY status has to be created before executing
        '''                               the Barcode scanning
        '''              SA 15/09/2011  - When insert the read cell as Not InUse position get as PatientID the ExternalPID or the PatientID
        '''                               depending if the patient is already in the application DB or not
        '''              SA 27/09/2011   - Changed load of VirtualRotorPositionsDS needed to create the Not InUse Rotor Positions:
        '''                                ** For both Rotors, Samples and Reagents, Not InUse positions with ScannedFlag=False should remain with
        '''                                   the same content than before
        '''              SA 20/02/2012    - Changed the way of calculating the Status of Reagents: call function CalculateNeededBottlesAndReagentStatus instead
        '''                                 of CalculateReagentStatus
        '''              SA 30/03/2012    - For both, NOT IN USE and IN USE Reagents, get fields TubeType and RealVolume from the DecodedDataDS and update
        '''                                 them in tables of WS Not In Use Rotor Positions and WS Rotor Positions respectively
        '''              SG 12/03/2013    - Informed field PatientID in VirtualRotorPositionsDS with value of field ExternalPID 
        '''              SA 09/09/2013    - Write a warning in the application LOG if the process of creation a new Empty WS was stopped due to a previous one still exists
        '''              XB/JC 09/10/2013 - BT #1274 ==> Correction on Load Virtual Rotors 
        '''              SA/JV 12/12/2013 - BT #1384 ==> When NOT IN USE Positions in Reagents Rotor are processed, if the position Status is DEPLETED or FEW, the current 
        '''                                              value of fields Status and RealVolume have to be keep
        '''              SA 21/03/2014    - BT #1545 ==> Changes to divide AddWorkSession process in several DB Transactions. When value of global flag 
        '''                                              NEWAddWorkSession is TRUE, call new version of function AddWorkSession 
        '''              SA 26/03/2014    - BT #1552 ==> For all Special Solutions in the WS, updates the Element Status to POS (there is at least a Bottle
        '''                                              of the Special Solution positioned in the Reagents Rotor)
        '''              AG 07/10/2014 - BA-1979 add traces into log when virtual rotor is saved with invalid values in order to find the origin
        ''' </remarks>
        Public Function ManageBarcodeInstruction(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pBarCodeRotorContentDS As WSRotorContentByPositionDS, _
                                                 ByVal pRotorType As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        If (Not manualEntryFlag) Then manualEntryTubeType = String.Empty

                        Dim myAnalyzerID As String = String.Empty
                        Dim myWorkSessionID As String = String.Empty
                        Dim myWSStatus As String = String.Empty

                        If (pBarCodeRotorContentDS.twksWSRotorContentByPosition.Rows.Count > 0) Then
                            If (Not pBarCodeRotorContentDS.twksWSRotorContentByPosition(0).IsAnalyzerIDNull) Then myAnalyzerID = pBarCodeRotorContentDS.twksWSRotorContentByPosition(0).AnalyzerID
                            If (Not pBarCodeRotorContentDS.twksWSRotorContentByPosition(0).IsWorkSessionIDNull) Then myWorkSessionID = pBarCodeRotorContentDS.twksWSRotorContentByPosition(0).WorkSessionID

                            If (myWorkSessionID = String.Empty) Then
                                'Create an empty WS
                                Dim myWSOrderTestsDS As New WSOrderTestsDS
                                Dim myWSDelegate As New WorkSessionsDelegate

                                If (NEWAddWorkSession) Then
                                    'BT #1545
                                    resultData = myWSDelegate.AddWorkSession_NEW(Nothing, myWSOrderTestsDS, True, myAnalyzerID)
                                Else
                                    resultData = myWSDelegate.AddWorkSession(dbConnection, myWSOrderTestsDS, True, myAnalyzerID)
                                End If

                                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                    'Get the ID of both: Work Session and Analyzer
                                    Dim myWorkSessionsDS As WorkSessionsDS = DirectCast(resultData.SetDatos, WorkSessionsDS)

                                    If (myWorkSessionsDS.twksWorkSessions.Rows.Count = 1) Then
                                        myWorkSessionID = myWorkSessionsDS.twksWorkSessions(0).WorkSessionID
                                        myAnalyzerID = myWorkSessionsDS.twksWorkSessions(0).AnalyzerID

                                        'SA/JV 12/12/2013 - BT #1384
                                        myWSStatus = myWorkSessionsDS.twksWorkSessions(0).WorkSessionStatus

                                        'If there was an existing WS and the adding of a new Empty one was stopped, write the Warning in the Application LOG
                                        If (myWorkSessionsDS.twksWorkSessions(0).CreateEmptyWSStopped) Then
                                            Dim myLogAcciones As New ApplicationLogManager()
                                            myLogAcciones.CreateLogActivity("WARNING: Source of call to add EMPTY WS when the previous one still exists", "BarcodeWSDelegate.ManageBarcodeInstruction", EventLogEntryType.Error, False)
                                        End If
                                    End If
                                End If
                            Else
                                'SA/JV 12/12/2013 - BT #1384
                                Dim myWSDelegate As New WSAnalyzersDelegate
                                resultData = myWSDelegate.ReadWSAnalyzers(dbConnection, myAnalyzerID, myWorkSessionID)

                                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                    'Get the ID of both: Work Session and Analyzer
                                    Dim myWSAnalyzersDS As WSAnalyzersDS = DirectCast(resultData.SetDatos, WSAnalyzersDS)

                                    If (myWSAnalyzersDS.twksWSAnalyzers.Rows.Count > 0) Then
                                        myWSStatus = myWSAnalyzersDS.twksWSAnalyzers(0).WSStatus
                                    End If
                                End If
                            End If

                            If (myAnalyzerID <> String.Empty) Then
                                'Read information of all elements in the current rotor (RotorType) BEFORE processing the Barcode results
                                Dim rcpDel As New WSRotorContentByPositionDelegate
                                resultData = rcpDel.GetRotorCurrentContentForBarcodeManagement(dbConnection, myAnalyzerID, myWorkSessionID, pRotorType)

                                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                    Dim currentContentDS As WSRotorContentByPositionDS = DirectCast(resultData.SetDatos, WSRotorContentByPositionDS)

                                    'Clear the list of Element IDs for Reagents used in reception of Barcode instructions
                                    reagentsElemIdList.Clear()

                                    'BT #1552 - Clear the list of Element IDs for Special Solutions used in reception of Barcode instruction
                                    specSolsElemIdList.Clear()

                                    Dim positionRead As Integer = 0
                                    Dim linqCurrentPosData As List(Of WSRotorContentByPositionDS.twksWSRotorContentByPositionRow)

                                    For Each readBarcodeRow As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow In pBarCodeRotorContentDS.twksWSRotorContentByPosition
                                        If (Not readBarcodeRow.IsCellNumberNull) Then
                                            positionRead = readBarcodeRow.CellNumber

                                            linqCurrentPosData = (From a As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow In currentContentDS.twksWSRotorContentByPosition _
                                                                 Where a.RotorType = pRotorType AndAlso a.CellNumber = positionRead _
                                                                Select a).ToList

                                            If (linqCurrentPosData.Count > 0) Then
                                                'Process information read with Barcode for the tube/bottle placed in the Rotor Cell
                                                resultData = ManageSinglePosition(dbConnection, myAnalyzerID, myWorkSessionID, pRotorType, readBarcodeRow, linqCurrentPosData(0))
                                                If (resultData.HasError) Then Exit For
                                            End If
                                        End If
                                    Next
                                    linqCurrentPosData = Nothing

                                    If (Not resultData.HasError) Then
                                        'For all Reagents in the WS, calculates the Element Status (POS, INCOMPLETE or NOPOS), depending on the total Required Volume 
                                        'and the available (positioned) ones
                                        If (pRotorType = "REAGENTS") Then
                                            Dim dlgReqElem As New WSRequiredElementsDelegate
                                            Dim myWSReqElementsDS As New WSRequiredElementsDS
                                            Dim myElementRow As WSRequiredElementsDS.twksWSRequiredElementsRow = myWSReqElementsDS.twksWSRequiredElements.NewtwksWSRequiredElementsRow

                                            For Each item As Integer In reagentsElemIdList
                                                myElementRow.BeginEdit()
                                                myElementRow.WorkSessionID = myWorkSessionID
                                                myElementRow.ElementID = item
                                                myElementRow.RequiredVolume = 0 'Total required volume will be recalculated by function CalculateNeededBottlesAndReagentStatus
                                                myElementRow.EndEdit()

                                                resultData = dlgReqElem.CalculateNeededBottlesAndReagentStatus(dbConnection, myAnalyzerID, myElementRow, 0)
                                                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                                    myElementRow = DirectCast(resultData.SetDatos, WSRequiredElementsDS.twksWSRequiredElementsRow)

                                                    'Update the Reagent Status...
                                                    resultData = dlgReqElem.UpdateStatus(dbConnection, item, myElementRow.ElementStatus)
                                                    If (resultData.HasError) Then Exit For
                                                Else
                                                    'Error calculating the Reagent Status...
                                                    Exit For
                                                End If
                                            Next
                                            reagentsElemIdList.Clear()

                                            'BT #1552 - For all Special Solutions in the WS, updates the Element Status to POS (there is at least a Bottle
                                            '           of the Special Solution positioned in the Reagents Rotor)
                                            For Each item As Integer In specSolsElemIdList
                                                'Update the Status of the Required Element for the Special Solution...
                                                resultData = dlgReqElem.UpdateStatus(dbConnection, item, "POS")
                                                If (resultData.HasError) Then Exit For
                                            Next
                                            specSolsElemIdList.Clear()
                                        End If
                                    End If

                                    If (Not resultData.HasError) Then
                                        'Read the FINAL rotor (RotorType) contents information AFTER treat the barcode results
                                        'This object must be returned to the AnalyzerManager method who has called me (ProcessCodeBarInstructionReceived)

                                        'SA/JV 12/12/2013 
                                        'BT #1384 - Get information of all cells in the Rotor, filter those with Status NOT IN USE and add them to the corresponding table 
                                        resultData = rcpDel.ReadByCellNumber(dbConnection, myAnalyzerID, myWorkSessionID, pRotorType, -1, True)

                                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                            currentContentDS = DirectCast(resultData.SetDatos, WSRotorContentByPositionDS)

                                            'SA/JV 12/12/2013
                                            'BT #1384 - Implement LINQ to get Rotor Positions containing not in use Elements
                                            Dim myNoInUsePosition As List(Of WSRotorContentByPositionDS.twksWSRotorContentByPositionRow)
                                            myNoInUsePosition = (From e As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow In currentContentDS.twksWSRotorContentByPosition _
                                                                Where e.RotorType = pRotorType _
                                                              AndAlso (e.Status = "NO_INUSE" _
                                                               OrElse (e.Status <> "FREE" AndAlso e.IsElementIDNull)) _
                                                               Select e).ToList

                                            If (myNoInUsePosition.Count > 0) Then
                                                'Add all NO_INUSE Elements into twksWSNotInUseRotorPositions table
                                                Dim noInUseDelegate As New WSNotInUseRotorPositionsDelegate

                                                'Delete NOT IN USE Rotor Positions previously saved for the Rotor Type and create the new ones
                                                resultData = noInUseDelegate.Reset(dbConnection, myAnalyzerID, myWorkSessionID, pRotorType)
                                                If (Not resultData.HasError) Then
                                                    Dim newNoInUseDS As New VirtualRotorPosititionsDS
                                                    Dim newRow As VirtualRotorPosititionsDS.tparVirtualRotorPosititionsRow

                                                    If (pRotorType = "SAMPLES") Then
                                                        For Each rcpNoInUseRow As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow In myNoInUsePosition
                                                            newRow = newNoInUseDS.tparVirtualRotorPosititions.NewtparVirtualRotorPosititionsRow
                                                            newRow.RingNumber = rcpNoInUseRow.RingNumber
                                                            newRow.CellNumber = rcpNoInUseRow.CellNumber

                                                            If (Not rcpNoInUseRow.IsTubeContentNull) Then newRow.TubeContent = rcpNoInUseRow.TubeContent Else newRow.SetTubeContentNull()
                                                            If (Not rcpNoInUseRow.IsMultiItemNumberNull) Then newRow.MultiItemNumber = rcpNoInUseRow.MultiItemNumber Else newRow.MultiItemNumber = 1

                                                            If (Not rcpNoInUseRow.IsScannedPositionNull) Then
                                                                newRow.ScannedPosition = rcpNoInUseRow.ScannedPosition
                                                                If (Not newRow.ScannedPosition) Then
                                                                    'In Samples Rotor, not InUse positions marked as not scanned should remains with the same information as before
                                                                    If (Not rcpNoInUseRow.IsSolutionCodeNull) Then newRow.SolutionCode = rcpNoInUseRow.SolutionCode
                                                                    If (Not rcpNoInUseRow.IsCalibratorIDNull) Then newRow.CalibratorID = rcpNoInUseRow.CalibratorID
                                                                    If (Not rcpNoInUseRow.IsControlIDNull) Then newRow.ControlID = rcpNoInUseRow.ControlID
                                                                End If

                                                                If (Not rcpNoInUseRow.IsBarCodeInfoNull) Then
                                                                    newRow.BarcodeInfo = rcpNoInUseRow.BarCodeInfo
                                                                    newRow.BarcodeStatus = rcpNoInUseRow.BarcodeStatus
                                                                    If (newRow.IsTubeContentNull) Then newRow.TubeContent = "PATIENT"

                                                                    'Decode fields PatientID and SampleType
                                                                    resultData = DecodeSamplesBarCode(dbConnection, rcpNoInUseRow.BarCodeInfo)
                                                                    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                                                        Dim decodedDataDS As BarCodesDS = DirectCast(resultData.SetDatos, BarCodesDS)

                                                                        If (decodedDataDS.DecodedSamplesFields.Rows.Count > 0) Then
                                                                            If (Not decodedDataDS.DecodedSamplesFields(0).IsExternalPIDNull) Then newRow.PatientID = decodedDataDS.DecodedSamplesFields(0).ExternalPID
                                                                            If (Not decodedDataDS.DecodedSamplesFields(0).IsSampleTypeNull) Then newRow.SampleType = decodedDataDS.DecodedSamplesFields(0).SampleType
                                                                        End If

                                                                    ElseIf (resultData.HasError AndAlso resultData.ErrorMessage = "Barcode Sample Type Error") Then
                                                                        'XB/JC 09/10/2013
                                                                        'BT #1274 - If there was an error decodifing field SampleType, set BarcodeStatus = ERROR for the Rotor Position 
                                                                        '           (in which case the position will be represented with a red cross) and continue with the next position
                                                                        '           (do not stopt the process)
                                                                        newRow.BarcodeStatus = "ERROR"
                                                                    End If
                                                                End If
                                                            End If
                                                            newNoInUseDS.tparVirtualRotorPosititions.AddtparVirtualRotorPosititionsRow(newRow)
                                                        Next rcpNoInUseRow

                                                    ElseIf (pRotorType = "REAGENTS") Then
                                                        For Each rcpNoInUseRow As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow In myNoInUsePosition
                                                            newRow = newNoInUseDS.tparVirtualRotorPosititions.NewtparVirtualRotorPosititionsRow
                                                            newRow.RingNumber = rcpNoInUseRow.RingNumber
                                                            newRow.CellNumber = rcpNoInUseRow.CellNumber

                                                            If (Not rcpNoInUseRow.IsTubeContentNull) Then newRow.TubeContent = rcpNoInUseRow.TubeContent Else newRow.SetTubeContentNull()
                                                            If (Not rcpNoInUseRow.IsMultiItemNumberNull) Then newRow.MultiItemNumber = rcpNoInUseRow.MultiItemNumber Else newRow.MultiItemNumber = 1

                                                            If (Not rcpNoInUseRow.IsScannedPositionNull) Then
                                                                newRow.ScannedPosition = rcpNoInUseRow.ScannedPosition

                                                                If (Not newRow.ScannedPosition) Then
                                                                    'In Reagents Rotor, Not InUse positions marked as not scanned should remains with the same information as before
                                                                    If (Not rcpNoInUseRow.IsSolutionCodeNull) Then newRow.SolutionCode = rcpNoInUseRow.SolutionCode
                                                                    If (Not rcpNoInUseRow.IsReagentIDNull) Then newRow.ReagentID = rcpNoInUseRow.ReagentID

                                                                    If (Not rcpNoInUseRow.IsRealVolumeNull) Then newRow.RealVolume = rcpNoInUseRow.RealVolume
                                                                    If (Not rcpNoInUseRow.IsStatusNull AndAlso (rcpNoInUseRow.Status = "DEPLETED" OrElse rcpNoInUseRow.Status = "FEW")) Then
                                                                        newRow.Status = rcpNoInUseRow.Status
                                                                    End If
                                                                End If

                                                                If (Not rcpNoInUseRow.IsBarCodeInfoNull) Then
                                                                    newRow.BarcodeInfo = rcpNoInUseRow.BarCodeInfo
                                                                    newRow.BarcodeStatus = rcpNoInUseRow.BarcodeStatus

                                                                    'Decode ReagentID or SolutionCode for Special and Washing Solutions
                                                                    resultData = DecodeReagentsBarCode(dbConnection, rcpNoInUseRow.BarCodeInfo, myAnalyzerID, newRow.CellNumber)
                                                                    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                                                        Dim decodedDataDS As BarCodesDS = DirectCast(resultData.SetDatos, BarCodesDS)

                                                                        If (decodedDataDS.DecodedReagentsFields.Rows.Count > 0) Then
                                                                            If (Not decodedDataDS.DecodedReagentsFields(0).IsReagentIDNull) Then newRow.ReagentID = decodedDataDS.DecodedReagentsFields(0).ReagentID
                                                                            If (Not decodedDataDS.DecodedReagentsFields(0).IsReagentNumberNull) Then newRow.MultiItemNumber = decodedDataDS.DecodedReagentsFields(0).ReagentNumber
                                                                            If (Not decodedDataDS.DecodedReagentsFields(0).IsSolutionCodeNull) Then newRow.SolutionCode = decodedDataDS.DecodedReagentsFields(0).SolutionCode

                                                                            If (newRow.IsTubeContentNull) Then
                                                                                If (decodedDataDS.DecodedReagentsFields(0).IsReagentFlag) Then
                                                                                    newRow.TubeContent = "REAGENT"
                                                                                Else
                                                                                    newRow.TubeContent = "SPEC_SOL"
                                                                                    If (InStr(decodedDataDS.DecodedReagentsFields(0).SolutionCode, "WASH", CompareMethod.Text) <> 0) Then newRow.TubeContent = "WASH_SOL"
                                                                                End If
                                                                            End If

                                                                            'Decode also Type and Volume for the Bottle
                                                                            If (Not decodedDataDS.DecodedReagentsFields(0).IsBottleTypeNull) Then newRow.TubeType = decodedDataDS.DecodedReagentsFields(0).BottleType
                                                                            If (Not decodedDataDS.DecodedReagentsFields(0).IsBottleVolumeNull) Then newRow.RealVolume = decodedDataDS.DecodedReagentsFields(0).BottleVolume

                                                                            'SA/JV 12/12/2013 - BT #1384
                                                                            If (Not rcpNoInUseRow.IsStatusNull AndAlso (rcpNoInUseRow.Status = "DEPLETED" OrElse rcpNoInUseRow.Status = "FEW")) Then
                                                                                newRow.RealVolume = rcpNoInUseRow.RealVolume
                                                                                newRow.Status = rcpNoInUseRow.Status
                                                                            End If
                                                                        End If
                                                                    End If
                                                                End If
                                                            End If
                                                            newNoInUseDS.tparVirtualRotorPosititions.AddtparVirtualRotorPosititionsRow(newRow)
                                                        Next
                                                    End If
                                                    newNoInUseDS.AcceptChanges()

                                                    'AG 07/10/2014 BA-1979 add classCalledFrom parameter
                                                    resultData = noInUseDelegate.Add(dbConnection, myAnalyzerID, pRotorType, myWorkSessionID, newNoInUseDS, WSNotInUseRotorPositionsDelegate.ClassCalledFrom.BarcodeResultsManagement)
                                                End If
                                            End If
                                            myNoInUsePosition = Nothing

                                            'Finally, get data in all Rotor cells and return a WSRotorContentByPositionDS
                                            If (Not resultData.HasError) Then
                                                'SA/JV 12/12/2013 - BT #1384
                                                resultData = rcpDel.ReadByCellNumber(dbConnection, myAnalyzerID, myWorkSessionID, pRotorType, -1, (myWSStatus = "EMPTY" OrElse myWSStatus = "OPEN"))
                                                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                                    currentContentDS = DirectCast(resultData.SetDatos, WSRotorContentByPositionDS)
                                                    resultData.SetDatos = currentContentDS 'Method returns the current rotor contents
                                                End If
                                            End If
                                        End If
                                    End If
                                End If
                            End If
                        End If

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
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "BarcodeWSDelegate.ManageBarcodeInstruction", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Barcode management for manual Barcode entry (from Rotor Positions screen)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pRotorType">Rotor Type: SAMPLES or REAGENTS</param>
        ''' <param name="pCellNumber">Number of Rotor Position</param>
        ''' <param name="pTubeType">Tube/Bottle Type</param>
        ''' <param name="pBarcodeManualValue">Barcode manually entered</param>
        ''' <param name="pWSStatus">Current Status of the active Work Session</param>
        ''' <returns>Rotor content by position wiht new position information</returns>
        ''' <remarks>
        ''' Created by:  AG 07/09/2011
        ''' Modified by: SA 11/06/2013 - Added new parameter pWSStatus for the status of the active WorkSession. Field WSStatus in the WSRotorContentByPositionDS 
        '''                              that is passed as parameter when calling function ManageBarcodeInstruction is informed with value of the new parameter
        '''                              (needed to update the WS Status from OPEN to PENDING when the WS Required Elements are created after scanning the Samples 
        '''                              Rotor -> new functionality added for LIS with ES)
        '''              SA 26/03/2014 - Linq variable set to Nothing once it is not needed
        ''' </remarks>
        Public Function EntryManualBarcode(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, _
                                           ByVal pRotorType As String, ByVal pCellNumber As Integer, ByVal pTubeType As String, ByVal pBarcodeManualValue As String, _
                                           ByVal pWSStatus As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim returnDS As New WSRotorContentByPositionDS
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Inform flag & tube type for manual entry (pediatric has different business in decodification process)
                        manualEntryFlag = True
                        manualEntryTubeType = pTubeType

                        'Prepare data
                        Dim myRCPdataSet As New WSRotorContentByPositionDS
                        Dim myRow As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow

                        myRow = myRCPdataSet.twksWSRotorContentByPosition.NewtwksWSRotorContentByPositionRow
                        myRow.AnalyzerID = pAnalyzerID
                        myRow.WorkSessionID = pWorkSessionID
                        myRow.WSStatus = pWSStatus
                        myRow.RotorType = pRotorType
                        myRow.CellNumber = pCellNumber
                        myRow.BarCodeInfo = pBarcodeManualValue
                        myRow.BarcodeStatus = "OK"
                        myRow.ScannedPosition = False
                        myRCPdataSet.twksWSRotorContentByPosition.AddtwksWSRotorContentByPositionRow(myRow)
                        myRCPdataSet.AcceptChanges()

                        'Call the main barcode results management
                        resultData = ManageBarcodeInstruction(dbConnection, myRCPdataSet, pRotorType)

                        'Prepare the returnDS with new information in position in entry parameters
                        returnDS.Clear()
                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            myRCPdataSet = DirectCast(resultData.SetDatos, WSRotorContentByPositionDS)

                            Dim linqRes As List(Of WSRotorContentByPositionDS.twksWSRotorContentByPositionRow)
                            linqRes = (From a As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow In myRCPdataSet.twksWSRotorContentByPosition _
                                      Where a.AnalyzerID = pAnalyzerID _
                                    AndAlso a.WorkSessionID = pWorkSessionID _
                                    AndAlso a.RotorType = pRotorType _
                                    AndAlso a.CellNumber = pCellNumber _
                                     Select a).ToList

                            If (linqRes.Count > 0) Then
                                For Each row As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow In linqRes
                                    returnDS.twksWSRotorContentByPosition.ImportRow(row)
                                Next
                                returnDS.AcceptChanges()
                            End If
                            linqRes = Nothing
                        End If

                        If (Not resultData.HasError) Then
                            'When the Database Connection was opened locally, then the Commit is executed
                            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                            resultData.SetDatos = returnDS
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
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "BarcodeWSDelegate.EntryManualBarcode", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Search if exists Barcode information read with critical data to be solved before go running (for instance: samples with no requests)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pRotorType">Rotor Type: SAMPLES or REAGENTS</param>
        ''' <returns>GlobalDataTO containing a Boolean value</returns>
        ''' <remarks>
        ''' Created by: 
        ''' </remarks>
        Public Function ExistBarcodeCriticalWarnings(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, _
                                                     ByVal pRotorType As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing
            Dim criticalWarnings As Boolean = False

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim dlgPositionWithNoRequests As New BarcodePositionsWithNoRequestsDelegate
                        resultData = dlgPositionWithNoRequests.ReadByAnalyzerAndWorkSession(dbConnection, pAnalyzerID, pWorkSessionID, pRotorType)

                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            Dim localDS As BarcodePositionsWithNoRequestsDS = DirectCast(resultData.SetDatos, BarcodePositionsWithNoRequestsDS)
                            If (localDS.twksWSBarcodePositionsWithNoRequests.Rows.Count > 0) Then criticalWarnings = True
                        End If
                    End If
                End If

                resultData.SetDatos = criticalWarnings
            Catch ex As Exception
                resultData = New GlobalDataTO()

                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "BarcodeWSDelegate.ExistBarcodeCriticalWarnings", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' When configuration of Samples BarCode is changed:
        ''' ** All scanned Patient Samples are removed from table twksBarcodePositionsWithNoRequests.
        ''' ** All NOT IN USE Patient Samples are removed from table twksWSNotInUseRotorPositions
        ''' ** All positions in Samples Rotor in which a NOT IN USE Patient Sample was placed as marked as FREE (in table twksWSRotorPositions)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by: TR 26/04/2013
        ''' </remarks>
        Public Function BarcodeConfigChangeActions(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, _
                                                   ByVal pRotorType As String) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myBarcodePositionWithNoRequests As New BarcodePositionsWithNoRequestsDelegate
                        Dim myWSNotInUseRotorPositionsDelegate As New WSNotInUseRotorPositionsDelegate
                        Dim myWSRotorContentByPositionDelegate As New WSRotorContentByPositionDelegate

                        'Remove all scanned Patient Samples from table twksBarcodePositionsWithNoRequests
                        myGlobalDataTO = myBarcodePositionWithNoRequests.ResetWS(dbConnection, pAnalyzerID, pWorkSessionID)
                        If (Not myGlobalDataTO.HasError) Then
                            'Delete all NOT IN USE Patient Sample tubes from Samples Rotor (twksWSNotInUseRotorPositions)
                            myGlobalDataTO = myWSNotInUseRotorPositionsDelegate.DeleteNotInUsePatients(dbConnection, pAnalyzerID, pWorkSessionID, GlobalEnumerates.Rotors.SAMPLES)
                        End If

                        'Mark as FREE all positions in Samples Rotor in which a NOT IN USE Patient Sample tube was placed
                        If (Not myGlobalDataTO.HasError) Then
                            myGlobalDataTO = myWSRotorContentByPositionDelegate.SetScannedPositionToFREE(dbConnection, pAnalyzerID, pWorkSessionID)
                        End If

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
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "BarcodeWSDelegate.BarcodeConfigChangeActions", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function
#End Region

#Region "Manage barcode instruction methods LEVEL #2 (Private)"
        ''' <summary>
        ''' Manage each individual Rotor Position read by Barcode (OK, NOREAD or ERROR)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pRotorType">Rotor Type: SAMPLES or REAGENTS</param>
        ''' <param name="pBarCodeResPosition">Row of Typed Dataset WSRotorContentByPositionDS containing the data read when scanning a Rotor Cell in the specified Rotor Type</param>
        ''' <param name="pCurrentContentRow">Row of Typed Dataset WSRotorContentByPositionDS containing data of the Rotor Cell before scanning it</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by: AG
        ''' </remarks>
        Private Function ManageSinglePosition(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, _
                                              ByVal pRotorType As String, ByVal pBarCodeResPosition As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow, _
                                              ByVal pCurrentContentRow As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Depending on the Status of the read position, different business is required
                        Select Case (pBarCodeResPosition.BarcodeStatus)
                            Case "OK" 'READ OK
                                'Different business for SAMPLES & REAGENTS
                                If (pRotorType = "SAMPLES") Then
                                    resultData = SaveOKReadSamplesRotorPosition(dbConnection, pAnalyzerID, pWorkSessionID, pRotorType, pBarCodeResPosition, pCurrentContentRow)
                                ElseIf (pRotorType = "REAGENTS") Then
                                    resultData = SaveOKReadReagentsRotorPosition(dbConnection, pAnalyzerID, pWorkSessionID, pRotorType, pBarCodeResPosition, pCurrentContentRow)
                                End If

                            Case "EMPTY" 'NOREAD
                                resultData = SaveNOREADPosition(dbConnection, pAnalyzerID, pWorkSessionID, pRotorType, pBarCodeResPosition, pCurrentContentRow)

                            Case "ERROR" 'ERROR READ
                                resultData = SaveERRORPosition(dbConnection, pAnalyzerID, pWorkSessionID, pRotorType, pBarCodeResPosition, pCurrentContentRow)

                        End Select

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
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "BarcodeWSDelegate.ManageSinglePosition", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Update a Rotor Position when a Barcode NOREAD is received
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pRotorType">Rotor Type: SAMPLES or REAGENTS</param>
        ''' <param name="pBarCodeResPosition">Row of Typed Dataset WSRotorContentByPositionDS containing the data read when scanning a Rotor Cell in the specified Rotor Type</param>
        ''' <param name="pCurrentContentRow">Row of Typed Dataset WSRotorContentByPositionDS containing data of the Rotor Cell before scanning it</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by: AG 03/08/2011
        ''' </remarks>
        Private Function SaveNOREADPosition(ByVal pDBConnection As SqlClient.SqlConnection, _
                                            ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, _
                                            ByVal pRotorType As String, _
                                            ByVal pBarCodeResPosition As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow, _
                                            ByVal pCurrentContentRow As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then

                        'If some previous user action (ScannedPosition = FALSE): -> Maintains information
                        'Else: -> Delete position
                        Dim maintainFlag As Boolean = True
                        If pCurrentContentRow.IsScannedPositionNull Then
                            maintainFlag = False
                        ElseIf pCurrentContentRow.ScannedPosition Then
                            maintainFlag = False
                        End If

                        If Not maintainFlag Then
                            'DELETE POSITION
                            'The method DeletePosition uses DS no row ... so convert it
                            Dim rcpDS As New WSRotorContentByPositionDS
                            rcpDS.twksWSRotorContentByPosition.ImportRow(pCurrentContentRow)
                            rcpDS.AcceptChanges()

                            Dim rcpDelegate As New WSRotorContentByPositionDelegate

                            'AG 11/11/2011 - If a multicalibrator point is deleted then delete the whole calibration kit
                            If String.Compare(pRotorType, "SAMPLES", False) = 0 Then
                                Dim myMessage As String = ""
                                resultData = rcpDelegate.CompleteDeletePositionSeleted(dbConnection, rcpDS, myMessage)
                                If Not resultData.HasError And Not resultData.SetDatos Is Nothing Then
                                    rcpDS = CType(resultData.SetDatos, WSRotorContentByPositionDS)
                                End If
                            End If
                            'AG 11/11/2011

                            resultData = rcpDelegate.DeletePositions(dbConnection, rcpDS, False)

                            'UPDATE BARCODE FIELDS
                            If Not resultData.HasError Then
                                rcpDS.Clear()
                                rcpDS.twksWSRotorContentByPosition.ImportRow(pBarCodeResPosition)
                                rcpDS.AcceptChanges()
                                resultData = rcpDelegate.UpdateBarCodeFields(dbConnection, rcpDS, False)
                            End If

                            'FOR SAMPLES DELETE POSITION FROM ExternalSampleID with no requrest
                            If Not resultData.HasError Then
                                If String.Compare(pRotorType, "SAMPLES", False) = 0 Then
                                    'If an ExternalSampleID with no request is informed in this posisiton. If affirmative: Remove from this table
                                    Dim noInformedSampleID As New BarcodePositionsWithNoRequestsDelegate
                                    resultData = noInformedSampleID.DeletePosition(dbConnection, pAnalyzerID, pWorkSessionID, pRotorType, pCurrentContentRow.CellNumber)
                                End If
                            End If

                        End If

                        If (Not resultData.HasError) Then
                            'When the Database Connection was opened locally, then the Commit is executed
                            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                            'resultData.SetDatos = '<value to return; if any>
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
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "BarcodeWSDelegate.SaveNOREADPosition", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData

        End Function

        ''' <summary>
        ''' Update a Rotor Position when a Barcode ERROR is received
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pRotorType">Rotor Type: SAMPLES or REAGENTS</param>
        ''' <param name="pBarCodeResPosition">Row of Typed Dataset WSRotorContentByPositionDS containing the data read when scanning a Rotor Cell in the specified Rotor Type</param>
        ''' <param name="pCurrentContentRow">Row of Typed Dataset WSRotorContentByPositionDS containing data of the Rotor Cell before scanning it</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by: AG 03/08/2011
        ''' </remarks>
        Private Function SaveERRORPosition(ByVal pDBConnection As SqlClient.SqlConnection, _
                                           ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, _
                                           ByVal pRotorType As String, _
                                           ByVal pBarCodeResPosition As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow, _
                                           ByVal pCurrentContentRow As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow, _
                                           Optional ByVal pDefaultErrorTubeType As String = "") As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then

                        'If some previous user action (ScannedPosition = FALSE): -> Maintains information
                        'Else: -> Delete position
                        Dim maintainFlag As Boolean = True
                        If pCurrentContentRow.IsBarcodeStatusNull Then
                            maintainFlag = False
                        ElseIf pCurrentContentRow.IsBarCodeInfoNull Then
                            maintainFlag = False
                        ElseIf String.Compare(pCurrentContentRow.BarCodeInfo, pBarCodeResPosition.BarCodeInfo, False) <> 0 Then
                            maintainFlag = False
                        End If
                        Dim updateAdditionalFields As Boolean = False

                        If Not maintainFlag Then
                            'AG 05/09/2011 - This method is called when:
                            'Sw receive an ERROR read from Analyzer -> No changes required
                            'Sw receive an OK read from Analyzer but decoding it Sw generates ERROR -> Change BarCodeStatus from OK to ERROR
                            If Not pBarCodeResPosition.IsBarcodeStatusNull AndAlso String.Compare(pBarCodeResPosition.BarcodeStatus, "OK", False) = 0 Then
                                pBarCodeResPosition.BeginEdit()
                                pBarCodeResPosition.BarcodeStatus = "ERROR"
                                'pBarCodeResPosition.BarCodeInfo = "" 'Clear the wrong barcode contents (use "" due in DAO this is the condition to set to NULL)

                                If String.Compare(pDefaultErrorTubeType, "", False) <> 0 Then
                                    pBarCodeResPosition.TubeType = pDefaultErrorTubeType
                                    updateAdditionalFields = True
                                End If
                                pBarCodeResPosition.EndEdit()
                            End If


                            'AG 10/10/2011 - DELETE POSITION
                            'The method DeletePosition uses DS no row ... so convert it
                            Dim rcpDS As New WSRotorContentByPositionDS
                            rcpDS.twksWSRotorContentByPosition.ImportRow(pCurrentContentRow)
                            rcpDS.AcceptChanges()

                            Dim rcpDelegate As New WSRotorContentByPositionDelegate

                            'AG 11/11/2011 - If a multicalibrator point is deleted then delete the whole calibration kit
                            If String.Compare(pRotorType, "SAMPLES", False) = 0 Then
                                Dim myMessage As String = ""
                                resultData = rcpDelegate.CompleteDeletePositionSeleted(dbConnection, rcpDS, myMessage)
                                If Not resultData.HasError And Not resultData.SetDatos Is Nothing Then
                                    rcpDS = CType(resultData.SetDatos, WSRotorContentByPositionDS)
                                End If
                            End If
                            'AG 11/11/2011

                            resultData = rcpDelegate.DeletePositions(dbConnection, rcpDS, False)
                            'AG 10/10/2011

                            'UPDATE BARCODE FIELDS
                            'The method UpdateBarCodeFields uses DS no row ... so convert it
                            If Not resultData.HasError Then
                                rcpDS.Clear()
                                rcpDS.twksWSRotorContentByPosition.ImportRow(pBarCodeResPosition)
                                rcpDS.AcceptChanges()
                                resultData = rcpDelegate.UpdateBarCodeFields(dbConnection, rcpDS, updateAdditionalFields)
                            End If

                        End If

                        If (Not resultData.HasError) Then
                            'When the Database Connection was opened locally, then the Commit is executed
                            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                            'resultData.SetDatos = '<value to return; if any>
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
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "BarcodeWSDelegate.SaveERRORPosition", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData

        End Function

        ''' <summary>
        ''' Saves / update position with OK read barcode result received for REAGENTS ROTOR
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pRotorType">Rotor Type: REAGENTS</param>
        ''' <param name="pBarCodeResPosition">Row of Typed Dataset WSRotorContentByPositionDS containing the data read when scanning a Rotor Cell in Reagents Rotor</param>
        ''' <param name="pCurrentContentRow">Row of Typed Dataset WSRotorContentByPositionDS containing data of the Rotor Cell before scanning it</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  AG 03/08/2011
        ''' Modified by: SA 05/09/2011 - Inform AnalyzerID and WorkSessionID before update the content of the Rotor Cell
        '''              SA 30/03/2012 - When the scanned position contains a Reagent or Special Solution that is not included in the active
        '''                              Work Session, assign as real volume the full bottle volume according is type 
        '''              SA 29/05/2012 - When the Reagent/Special Solution exists in the WorkSession as a required Element, assign the TubeContent
        '''                              of the required Element when it is loading in DS UpdatedRcpDS. Besides, add the Element to the Reagents list 
        '''                              needed to calculate the needed Reagents volume and bottles only when TubeContent = REAGENT, ignore Special
        '''                              and Washing Solutions 
        '''              JV 09/01/2014 - BT #1443 ==> Added changes to update the Position Status with the Status saved in the table of Historic Reagent Bottles 
        '''              SA 26/03/2014 - BT #1552 ==> When the scanned position contains an Special Solution that exists as Required Element in the active WS,
        '''                                           save the ElementID in the list of Element IDs which Element Status has to be updated to POS 
        ''' </remarks>
        Private Function SaveOKReadReagentsRotorPosition(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, _
                                                         ByVal pRotorType As String, ByVal pBarCodeResPosition As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow, _
                                                         ByVal pCurrentContentRow As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim rcpDelegate As New WSRotorContentByPositionDelegate
                        Dim UpdatedRcpDS As New WSRotorContentByPositionDS
                        Dim updateAdditonalRcpFlag As Boolean = False

                        'Only performe business if Barcode information is different from current
                        If pCurrentContentRow.IsBarCodeInfoNull OrElse _
                           (Not pCurrentContentRow.IsBarCodeInfoNull AndAlso pBarCodeResPosition.BarCodeInfo <> pCurrentContentRow.BarCodeInfo) Then

                            'Decode bar code info
                            resultData = DecodeReagentsBarCode(dbConnection, pBarCodeResPosition.BarCodeInfo, pAnalyzerID, pCurrentContentRow.CellNumber)
                            If resultData.HasError Then
                                'Mark position as BarCodeStatus ERROR due the barcode read is not compatible with required codification 
                                '(by default consider bottle 20ml due to it can be used in both rings)
                                resultData = SaveERRORPosition(dbConnection, pAnalyzerID, pWorkSessionID, pRotorType, pBarCodeResPosition, pCurrentContentRow, "BOTTLE2")
                            Else
                                Dim decodedDataDS As BarCodesDS = DirectCast(resultData.SetDatos, BarCodesDS)
                                If (decodedDataDS.DecodedReagentsFields.Rows.Count > 0) Then
                                    'AG 16/09/2011 - When new Barcode information is read ... means the current information must be deleted before treat the new one
                                    UpdatedRcpDS.Clear()
                                    UpdatedRcpDS.twksWSRotorContentByPosition.ImportRow(pCurrentContentRow)

                                    UpdatedRcpDS.twksWSRotorContentByPosition(0).BeginEdit()
                                    UpdatedRcpDS.twksWSRotorContentByPosition(0).AnalyzerID = pAnalyzerID
                                    UpdatedRcpDS.twksWSRotorContentByPosition(0).WorkSessionID = pWorkSessionID
                                    UpdatedRcpDS.twksWSRotorContentByPosition(0).EndEdit()
                                    UpdatedRcpDS.AcceptChanges()

                                    resultData = rcpDelegate.DeletePositions(dbConnection, UpdatedRcpDS, False)
                                    UpdatedRcpDS.Clear()

                                    If (decodedDataDS.DecodedReagentsFields(0).IsExternalReagentFlag) Then
                                        'Update barcode fields (note that BarcodeStatus changes from OK to UNKNOWN)
                                        'Inform in RotorContentByPosition --> ElementID=NULL, MultiTubeNumber=1, TubeType (BottleType), RealVolume (BottleVolume) 
                                        '                                     Status=NO_INUSE, BarcodeStatus=UNKNOWN, BarcodeInfo, ScannedPosition=True
                                        If (Not resultData.HasError) Then
                                            UpdatedRcpDS.twksWSRotorContentByPosition.ImportRow(pBarCodeResPosition)
                                            UpdatedRcpDS.twksWSRotorContentByPosition(0).BeginEdit()
                                            UpdatedRcpDS.twksWSRotorContentByPosition(0).AnalyzerID = pAnalyzerID
                                            UpdatedRcpDS.twksWSRotorContentByPosition(0).WorkSessionID = pWorkSessionID
                                            UpdatedRcpDS.twksWSRotorContentByPosition(0).BarcodeStatus = "UNKNOWN"
                                            UpdatedRcpDS.twksWSRotorContentByPosition(0).TubeContent = "REAGENT"

                                            UpdatedRcpDS.twksWSRotorContentByPosition(0).SetElementIDNull()
                                            UpdatedRcpDS.twksWSRotorContentByPosition(0).SetElementStatusNull()
                                            UpdatedRcpDS.twksWSRotorContentByPosition(0).MultiTubeNumber = 1
                                            UpdatedRcpDS.twksWSRotorContentByPosition(0).TubeType = decodedDataDS.DecodedReagentsFields(0).BottleType
                                            UpdatedRcpDS.twksWSRotorContentByPosition(0).RealVolume = decodedDataDS.DecodedReagentsFields(0).BottleVolume
                                            UpdatedRcpDS.twksWSRotorContentByPosition(0).Status = "NO_INUSE"
                                            UpdatedRcpDS.twksWSRotorContentByPosition(0).EndEdit()
                                            UpdatedRcpDS.AcceptChanges()
                                            updateAdditonalRcpFlag = True
                                        End If
                                    Else
                                        'BIOSYSTEMS Reagent or BIOSYSTEMS Special Solution
                                        'Check if the Reagent/Special Solution exists in the WorkSession as a required Element
                                        Dim reqElementDlg As New WSRequiredElementsDelegate

                                        resultData = reqElementDlg.ReadReagentRotorElementByBarcodeID(dbConnection, decodedDataDS)
                                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                            Dim myReqElement As WSRequiredElementsDS = DirectCast(resultData.SetDatos, WSRequiredElementsDS)

                                            If (myReqElement.twksWSRequiredElements.Rows.Count = 0) Then
                                                'Reagent or Special Solution does not belong to the WorkSession
                                                'Inform in RotorContentByPosition --> ElementID=NULL, MultiTubeNumber, TubeType (BottleType), RealVolume (BottleVolume), 
                                                '                                     Status=NO_INUSE, BarcodeStatus=OK, BarcodeInfo, ScannedPosition=True

                                                'Import row to get values of fields ScannedPosition, BarcodeInfo, BarcodeStatus, and update rest of fields
                                                UpdatedRcpDS.twksWSRotorContentByPosition.ImportRow(pBarCodeResPosition)

                                                UpdatedRcpDS.twksWSRotorContentByPosition(0).BeginEdit()
                                                UpdatedRcpDS.twksWSRotorContentByPosition(0).AnalyzerID = pAnalyzerID
                                                UpdatedRcpDS.twksWSRotorContentByPosition(0).WorkSessionID = pWorkSessionID
                                                UpdatedRcpDS.twksWSRotorContentByPosition(0).SetElementIDNull()
                                                UpdatedRcpDS.twksWSRotorContentByPosition(0).SetElementStatusNull()
                                                UpdatedRcpDS.twksWSRotorContentByPosition(0).MultiTubeNumber = 1 'Default value

                                                Dim rcpNoInUseDelegate As New WSNotInUseRotorPositionsDelegate

                                                Dim reagentID As Integer = -1
                                                If (Not decodedDataDS.DecodedReagentsFields(0).IsReagentIDNull) Then
                                                    reagentID = decodedDataDS.DecodedReagentsFields(0).ReagentID
                                                    UpdatedRcpDS.twksWSRotorContentByPosition(0).TubeContent = "REAGENT"
                                                End If

                                                Dim solutionCode As String = String.Empty
                                                If (Not decodedDataDS.DecodedReagentsFields(0).IsSolutionCodeNull) Then
                                                    solutionCode = decodedDataDS.DecodedReagentsFields(0).SolutionCode
                                                End If

                                                'Verify if there are bottles of the Reagent or Special Solution already positioned in the Rotor (to assign 
                                                'value of MultitubeNumber)
                                                resultData = rcpNoInUseDelegate.GetPlacedTubesByPosition(dbConnection, pAnalyzerID, pRotorType, pWorkSessionID, reagentID, solutionCode)
                                                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                                    UpdatedRcpDS.twksWSRotorContentByPosition(0).MultiTubeNumber = CType(resultData.SetDatos, Integer) + 1
                                                End If

                                                UpdatedRcpDS.twksWSRotorContentByPosition(0).TubeType = decodedDataDS.DecodedReagentsFields(0).BottleType
                                                UpdatedRcpDS.twksWSRotorContentByPosition(0).RealVolume = decodedDataDS.DecodedReagentsFields(0).BottleVolume
                                                UpdatedRcpDS.twksWSRotorContentByPosition(0).Status = "NO_INUSE"
                                                UpdatedRcpDS.twksWSRotorContentByPosition(0).EndEdit()

                                                UpdatedRcpDS.AcceptChanges()
                                                updateAdditonalRcpFlag = True
                                            Else
                                                'Reagent or Special Solution belongs to the WorkSession
                                                'Inform in RotorContentByPosition --> ElementID, MultiTubeNumber, TubeType (BottleType), RealVolume (BottleVolume),
                                                '                                     Status (INUSE), BarcodeStatus=OK, BarcodeInfo, ScannedPosition=True

                                                'Import row to get values of fields ScannedPosition, BarcodeInfo, BarcodeStatus, and update rest of fields
                                                UpdatedRcpDS.twksWSRotorContentByPosition.ImportRow(pBarCodeResPosition) 'BarCodeStatus, BarcodeInfo, ScannedPosition

                                                UpdatedRcpDS.twksWSRotorContentByPosition(0).BeginEdit()
                                                UpdatedRcpDS.twksWSRotorContentByPosition(0).AnalyzerID = pAnalyzerID
                                                UpdatedRcpDS.twksWSRotorContentByPosition(0).WorkSessionID = pWorkSessionID
                                                UpdatedRcpDS.twksWSRotorContentByPosition(0).ElementID = myReqElement.twksWSRequiredElements(0).ElementID
                                                UpdatedRcpDS.twksWSRotorContentByPosition(0).MultiTubeNumber = 1 'Default value
                                                UpdatedRcpDS.twksWSRotorContentByPosition(0).TubeContent = myReqElement.twksWSRequiredElements(0).TubeContent

                                                'Verify if there are bottles of the Reagent or Special Solution already positioned in the Rotor (to assign 
                                                'value of MultitubeNumber)
                                                resultData = rcpDelegate.GetPlacedTubesByElement(dbConnection, pAnalyzerID, pRotorType, pWorkSessionID, myReqElement.twksWSRequiredElements(0).ElementID)
                                                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                                                    UpdatedRcpDS.twksWSRotorContentByPosition(0).MultiTubeNumber = CType(resultData.SetDatos, Integer) + 1
                                                End If

                                                UpdatedRcpDS.twksWSRotorContentByPosition(0).TubeType = decodedDataDS.DecodedReagentsFields(0).BottleType
                                                UpdatedRcpDS.twksWSRotorContentByPosition(0).RealVolume = decodedDataDS.DecodedReagentsFields(0).BottleVolume
                                                UpdatedRcpDS.twksWSRotorContentByPosition(0).Status = "INUSE"
                                                UpdatedRcpDS.twksWSRotorContentByPosition(0).EndEdit()

                                                UpdatedRcpDS.AcceptChanges()
                                                updateAdditonalRcpFlag = True

                                                'When it is a BioSystems REAGENT belonging to the WorkSession: add it into Reagents list
                                                If (myReqElement.twksWSRequiredElements(0).TubeContent = "REAGENT") Then
                                                    If (Not reagentsElemIdList.Contains(myReqElement.twksWSRequiredElements(0).ElementID)) Then reagentsElemIdList.Add(myReqElement.twksWSRequiredElements(0).ElementID)

                                                    'TR 12/06/2012 - Search if the Reagent Bottle exists on table thisReagentsBottles. If it exists, then read
                                                    '                the last volume detected and determine if the Bottle has to be Locked due to refill
                                                    Dim myhisReagentsBottlesDelegate As New HisReagentBottlesDelegate
                                                    resultData = myhisReagentsBottlesDelegate.ReadByBarCode(dbConnection, pBarCodeResPosition.BarCodeInfo)
                                                    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                                        Dim myReagentsBottlesDS As HisReagentsBottlesDS = DirectCast(resultData.SetDatos, HisReagentsBottlesDS)

                                                        If (myReagentsBottlesDS.thisReagentsBottles.Count > 0) Then
                                                            'TR 01/10/2012 - Validate if the Bottle is Locked due to invalid refill
                                                            'JV 09/01/2014 - BT #1443: Update the Position Status with the Status saved in the table of Historic Reagent Bottles 
                                                            UpdatedRcpDS.twksWSRotorContentByPosition(0).Status = myReagentsBottlesDS.thisReagentsBottles(0).Status
                                                            If (myReagentsBottlesDS.thisReagentsBottles(0).BottleStatus = "LOCKED") Then
                                                                'Update the Rotor Position Status with the Bottle Status found on Historic table
                                                                UpdatedRcpDS.twksWSRotorContentByPosition(0).Status = myReagentsBottlesDS.thisReagentsBottles(0).BottleStatus
                                                            End If

                                                            'Update the Rotor Position Volume with the Bottle Volume saved on Historic table
                                                            UpdatedRcpDS.twksWSRotorContentByPosition(0).RealVolume = myReagentsBottlesDS.thisReagentsBottles(0).BottleVolume
                                                        End If

                                                        'Calculate the Number of Tests that can be executed with the Real Bottle Volume 
                                                        resultData = reqElementDlg.CalculateRemainingTests(dbConnection, pWorkSessionID, _
                                                                                                           myReqElement.twksWSRequiredElements(0).ElementID, _
                                                                                                           UpdatedRcpDS.twksWSRotorContentByPosition(0).RealVolume, _
                                                                                                           UpdatedRcpDS.twksWSRotorContentByPosition(0).TubeType)

                                                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                                            UpdatedRcpDS.twksWSRotorContentByPosition(0).RemainingTestsNumber = CType(resultData.SetDatos, Integer)
                                                        End If
                                                    End If
                                                Else
                                                    'BT #1552 - When it is a BioSystems SPECIAL SOLUTION belonging to the WorkSession: add it into Special Solutions list
                                                    If (Not specSolsElemIdList.Contains(myReqElement.twksWSRequiredElements(0).ElementID)) Then specSolsElemIdList.Add(myReqElement.twksWSRequiredElements(0).ElementID)
                                                End If
                                            End If
                                        End If
                                    End If
                                End If
                            End If

                            'If current Barcode info read is the same Sw has but Sw has BarcodeStatus = UNKNOWN ... do nothing
                            'Fw sends BarcodeStatus OK but previous Sw management has changed it to UNKNOWN so not change it
                        ElseIf (Not pCurrentContentRow.IsBarCodeInfoNull AndAlso pBarCodeResPosition.BarCodeInfo = pCurrentContentRow.BarCodeInfo AndAlso _
                                pCurrentContentRow.BarcodeStatus = "UNKNOWN") Then
                            'Do nothing

                        Else
                            'Update barcode fields (note that BarcodeInfo, BarcodeStatus, ScannedPosition)
                            UpdatedRcpDS.twksWSRotorContentByPosition.ImportRow(pBarCodeResPosition)
                            UpdatedRcpDS.twksWSRotorContentByPosition(0).BeginEdit()
                            UpdatedRcpDS.twksWSRotorContentByPosition(0).AnalyzerID = pAnalyzerID
                            UpdatedRcpDS.twksWSRotorContentByPosition(0).WorkSessionID = pWorkSessionID
                            UpdatedRcpDS.twksWSRotorContentByPosition(0).SetBarcodeStatusNull() 'AG 26/09/2011 - Fw sends OK but Sw can transform it into ERROR, so not change it
                            UpdatedRcpDS.twksWSRotorContentByPosition(0).EndEdit()
                            UpdatedRcpDS.AcceptChanges()
                            updateAdditonalRcpFlag = False
                        End If

                        'Update Barcode fields in table RotorContentByPosition
                        If (Not resultData.HasError) Then resultData = rcpDelegate.UpdateBarCodeFields(dbConnection, UpdatedRcpDS, updateAdditonalRcpFlag)

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
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "BarcodeWSDelegate.SaveOKReadReagentsRotorPosition", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Saves / update position with OK read barcode result received for SAMPLES ROTOR
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID" >Analyzer Identifier</param>
        ''' <param name="pWorkSessionID" >WorkSession Identifier</param>
        ''' <param name="pRotorType">Rotor Type: SAMPLES</param>
        ''' <param name="pBarCodeResPosition">Row of Typed Dataset WSRotorContentByPositionDS containing the data read when scanning a Rotor Cell in Samples Rotor</param>
        ''' <param name="pCurrentContentRow">Row of Typed Dataset WSRotorContentByPositionDS containing data of the Rotor Cell before scanning it</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  AG 03/08/2011
        ''' Modified by: SA 01/09/2011 - Replaced calling to function ReadElementIdByExternalOID in OrdersDelegate by function 
        '''                              VerifyPatientExistsInActiveWS in BarcodeWSDelegate. Inform AnalyzerID and WorkSessionID in the DS used
        '''                              to update the content of a Rotor cell
        '''              SA 10/04/2013 - Replaced calling to function VerifyPatientExistsInActiveWS in BarcodeWSDelegate for calling to function CheckIncompletedPatientSample
        '''                              in BarcodePositionsWithNoRequestsDelegate (added for implementation of LIS with ES)
        '''              SA 11/06/2013 - When load the BarcodePositionsWithNoRequestsDS that has to be passed as parameter when calling function CheckIncompletedPatientSample
        '''                              in BarcodePositionsWithNoRequestsDelegate, inform field WSStatus with value of same field in the parameter pBarCodeResPosition
        '''                              (needed to update the WS Status from OPEN to PENDING when the WS Required Elements are created after scanning the Samples Rotor -> new 
        '''                              functionality added for LIS with ES)
        '''             TR 22/07/2013    When user set the barcode manualy by default the tube type is Tube, now user can introduce barcode for pediatrics tubes, then if the
        '''                             element with barcode is(required or not) and the tube type is Pediatric then do not change the tube type. BugTracking #1195.
        ''' </remarks>
        Private Function SaveOKReadSamplesRotorPosition(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, _
                                                        ByVal pRotorType As String, ByVal pBarCodeResPosition As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow, _
                                                        ByVal pCurrentContentRow As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim updateAdditonalRcpFlag As Boolean = False
                        Dim rcpDelegate As New WSRotorContentByPositionDelegate
                        Dim updatedRcpDS As New WSRotorContentByPositionDS

                        'TR 22/07/2013 Bug #1195 Variable used to indicate the bottle type 
                        Dim myPrevTubeType As String = String.Empty

                        'Execute the business logic only when the Barcode read by scanning is different from the current one
                        If (pCurrentContentRow.IsBarCodeInfoNull) OrElse _
                           (Not pCurrentContentRow.IsBarCodeInfoNull AndAlso pBarCodeResPosition.BarCodeInfo <> pCurrentContentRow.BarCodeInfo) Then
                            'Decode BarCode information
                            resultData = DecodeSamplesBarCode(dbConnection, pBarCodeResPosition.BarCodeInfo)
                            If (resultData.HasError) Then
                                'Mark position as BarCodeStatus ERROR due the barcode read is not compatible with required codification
                                resultData = SaveERRORPosition(dbConnection, pAnalyzerID, pWorkSessionID, pRotorType, pBarCodeResPosition, pCurrentContentRow)
                            Else
                                Dim decodedDataDS As BarCodesDS = DirectCast(resultData.SetDatos, BarCodesDS)
                                If (decodedDataDS.DecodedSamplesFields.Rows.Count > 0) Then
                                    'AG 16/09/2011 - When new Barcode information is read ... means the current information must be deleted before treat the new one
                                    updatedRcpDS.Clear()
                                    updatedRcpDS.twksWSRotorContentByPosition.ImportRow(pCurrentContentRow)
                                    updatedRcpDS.twksWSRotorContentByPosition(0).BeginEdit()
                                    updatedRcpDS.twksWSRotorContentByPosition(0).AnalyzerID = pAnalyzerID
                                    updatedRcpDS.twksWSRotorContentByPosition(0).WorkSessionID = pWorkSessionID
                                    updatedRcpDS.twksWSRotorContentByPosition(0).EndEdit()
                                    updatedRcpDS.AcceptChanges()

                                    'AG 11/11/2011 - If a multicalibrator point is deleted then delete the whole calibration kit
                                    Dim myMessage As String = ""
                                    resultData = rcpDelegate.CompleteDeletePositionSeleted(dbConnection, updatedRcpDS, myMessage)
                                    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                        updatedRcpDS = DirectCast(resultData.SetDatos, WSRotorContentByPositionDS)
                                    End If

                                    If Not pCurrentContentRow.IsTubeTypeNull Then
                                        'TR 22/07/2013 Bug #1195 -Before clear set the bottle type to my local variable
                                        myPrevTubeType = pCurrentContentRow.TubeType
                                    End If
                                    

                                    resultData = rcpDelegate.DeletePositions(dbConnection, updatedRcpDS, False)
                                    updatedRcpDS.Clear()
                                    'AG 16/09/2011

                                    '**BIOSYSTEMS SAMPLE BARCODE BUSINESS
                                    Dim elementID As Integer = -1
                                    Dim myReqElementsDS As New WSRequiredElementsDS

                                    Dim defaultPatientTubeType As String = "T13"
                                    Dim dlgUserSettings As New UserSettingsDelegate
                                    resultData = dlgUserSettings.GetCurrentValueBySettingID(dbConnection, GlobalEnumerates.UserSettingsEnum.DEFAULT_TUBE_PATIENT.ToString)
                                    If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                                        defaultPatientTubeType = DirectCast(resultData.SetDatos, String)
                                        If (defaultPatientTubeType = "PED") Then defaultPatientTubeType = "T13" 'Barcode can not be read for pediatrics
                                    End If

                                    'Prepare data to check if the read Barcode can be link to a required Element of the active WS
                                    Dim myBCPosWithNoRequestDS As New BarcodePositionsWithNoRequestsDS
                                    Dim myBCPosWithNoRequestRow As BarcodePositionsWithNoRequestsDS.twksWSBarcodePositionsWithNoRequestsRow

                                    myBCPosWithNoRequestRow = myBCPosWithNoRequestDS.twksWSBarcodePositionsWithNoRequests.NewtwksWSBarcodePositionsWithNoRequestsRow()
                                    myBCPosWithNoRequestRow.AnalyzerID = pAnalyzerID
                                    myBCPosWithNoRequestRow.WorkSessionID = pWorkSessionID
                                    myBCPosWithNoRequestRow.WSStatus = pBarCodeResPosition.WSStatus 'SA 11/06/2013: Inform status of the active WorkSession
                                    myBCPosWithNoRequestRow.RotorType = pRotorType
                                    myBCPosWithNoRequestRow.CellNumber = pCurrentContentRow.CellNumber
                                    myBCPosWithNoRequestRow.BarCodeInfo = decodedDataDS.DecodedSamplesFields.First.BarcodeInfo
                                    myBCPosWithNoRequestRow.ExternalPID = decodedDataDS.DecodedSamplesFields.First.ExternalPID
                                    If (Not decodedDataDS.DecodedSamplesFields.First.IsPatientIDNull) Then myBCPosWithNoRequestRow.PatientID = decodedDataDS.DecodedSamplesFields.First.PatientID
                                    If (Not decodedDataDS.DecodedSamplesFields.First.IsSampleTypeNull) Then myBCPosWithNoRequestRow.SampleType = decodedDataDS.DecodedSamplesFields.First.SampleType
                                    myBCPosWithNoRequestDS.twksWSBarcodePositionsWithNoRequests.AddtwksWSBarcodePositionsWithNoRequestsRow(myBCPosWithNoRequestRow)

                                    Dim myListOfBCRows As New List(Of BarcodePositionsWithNoRequestsDS.twksWSBarcodePositionsWithNoRequestsRow)
                                    myListOfBCRows.Add(myBCPosWithNoRequestRow)

                                    'Verify if there is a required Element created in the WS for the scanned Barcode
                                    Dim myBCPosWithNoRequestDelegate As New BarcodePositionsWithNoRequestsDelegate
                                    resultData = myBCPosWithNoRequestDelegate.CheckIncompletedPatientSample(dbConnection, myListOfBCRows, False)
                                    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                        myReqElementsDS = DirectCast(resultData.SetDatos, WSRequiredElementsDS)
                                        If (myReqElementsDS.twksWSRequiredElements.Rows.Count > 0) Then elementID = myReqElementsDS.twksWSRequiredElements.First.ElementID

                                        'resultData = VerifyPatientExistsInActiveWS(dbConnection, pWorkSessionID, decodedDataDS)
                                        'If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                        '    'elementID = DirectCast(resultData.SetDatos, Integer)
                                        '    myReqElementsDS = DirectCast(resultData.SetDatos, WSRequiredElementsDS)
                                        '    If Not myReqElementsDS.twksWSRequiredElements(0).IsElementIDNull Then elementID = myReqElementsDS.twksWSRequiredElements(0).ElementID

                                        If (elementID = -1) Then
                                            'The content of the scanned cell is unknown....
                                            '**Include the rotor position in the table of incomplete Patients/Samples (twksBarCodePositionWithNoRequest)

                                            Dim noTestRequestDlg As New BarcodePositionsWithNoRequestsDelegate
                                            resultData = noTestRequestDlg.AddPosition(dbConnection, pAnalyzerID, pWorkSessionID, pRotorType, pCurrentContentRow.CellNumber, decodedDataDS)

                                            '**Update the Rotor Position to indicate the cell content is unknown
                                            updatedRcpDS.twksWSRotorContentByPosition.ImportRow(pBarCodeResPosition) 'BarCodeStatus, BarcodeInfo, ScannedPosition

                                            updatedRcpDS.twksWSRotorContentByPosition(0).BeginEdit()
                                            updatedRcpDS.twksWSRotorContentByPosition(0).AnalyzerID = pAnalyzerID
                                            updatedRcpDS.twksWSRotorContentByPosition(0).WorkSessionID = pWorkSessionID
                                            updatedRcpDS.twksWSRotorContentByPosition(0).SetElementIDNull()
                                            updatedRcpDS.twksWSRotorContentByPosition(0).SetElementStatusNull()
                                            updatedRcpDS.twksWSRotorContentByPosition(0).MultiTubeNumber = 1
                                            updatedRcpDS.twksWSRotorContentByPosition(0).TubeContent = "PATIENT"
                                            'TR 22/07/2013 -BugTracking 1195 validate the tube type before asigning the default tube type..
                                            If myPrevTubeType = String.Empty Then
                                                updatedRcpDS.twksWSRotorContentByPosition(0).TubeType = defaultPatientTubeType
                                            Else
                                                'Set the tube type that has before.
                                                updatedRcpDS.twksWSRotorContentByPosition(0).TubeType = myPrevTubeType
                                            End If
                                            updatedRcpDS.twksWSRotorContentByPosition(0).Status = "NO_INUSE"
                                            updatedRcpDS.twksWSRotorContentByPosition(0).EndEdit()

                                            updatedRcpDS.AcceptChanges()
                                            updateAdditonalRcpFlag = True
                                        Else
                                            'The content of the scanned cell is known: there are Tests requested previously for the Patient/Sample
                                            '**Update the rotor position to inform data of the required Element placed in it
                                            updatedRcpDS.twksWSRotorContentByPosition.ImportRow(pBarCodeResPosition) 'BarCodeStatus, BarcodeInfo, ScannedPosition

                                            updatedRcpDS.twksWSRotorContentByPosition(0).BeginEdit()
                                            updatedRcpDS.twksWSRotorContentByPosition(0).AnalyzerID = pAnalyzerID
                                            updatedRcpDS.twksWSRotorContentByPosition(0).WorkSessionID = pWorkSessionID
                                            updatedRcpDS.twksWSRotorContentByPosition(0).ElementID = elementID
                                            updatedRcpDS.twksWSRotorContentByPosition(0).MultiTubeNumber = 1
                                            updatedRcpDS.twksWSRotorContentByPosition(0).TubeContent = "PATIENT"
                                            'TR 22/07/2013 -BugTracking 1195 validate the tube type before asigning the default tube type..
                                            If myPrevTubeType = String.Empty Then
                                                updatedRcpDS.twksWSRotorContentByPosition(0).TubeType = defaultPatientTubeType
                                            Else
                                                'Set the tube type that has before.
                                                updatedRcpDS.twksWSRotorContentByPosition(0).TubeType = myPrevTubeType
                                            End If
                                            updatedRcpDS.twksWSRotorContentByPosition(0).Status = "PENDING"

                                            'AG 08/09/2011 - Status can get the following values (depending the Executions by ElementID): Pending (default), InProcess, Finished, Depleted
                                            '(Maybe the same Status calculation business has to be used in method RotorContentByPositionDelegate.PatientSamplePositioning)
                                            Dim newStatus As String = ""
                                            resultData = rcpDelegate.UpdateSamplePositionStatus(dbConnection, -1, pWorkSessionID, pAnalyzerID, elementID, myReqElementsDS.twksWSRequiredElements.First.TubeContent, _
                                                                                                updatedRcpDS.twksWSRotorContentByPosition(0).MultiTubeNumber, newStatus, -1)
                                            If (Not resultData.HasError AndAlso newStatus <> String.Empty) Then updatedRcpDS.twksWSRotorContentByPosition(0).Status = newStatus
                                            'AG 08/09/2011

                                            updatedRcpDS.twksWSRotorContentByPosition(0).EndEdit()
                                            updatedRcpDS.AcceptChanges()

                                            updateAdditonalRcpFlag = True

                                            If (updatedRcpDS.twksWSRotorContentByPosition(0).Status <> "DEPLETED") Then
                                                Dim myWSRequiredElementsDelegate As New WSRequiredElementsDelegate
                                                resultData = myWSRequiredElementsDelegate.UpdateStatus(dbConnection, elementID, "POS")
                                                updatedRcpDS.twksWSRotorContentByPosition(0).ElementStatus = "POS"
                                            Else
                                                updatedRcpDS.twksWSRotorContentByPosition(0).ElementStatus = "NOPOS"
                                            End If
                                        End If
                                    End If

                                End If 'If decodedDataDS.DecodedSamplesFields.Rows.Count > 0 Then
                            End If

                        Else 'Case ELSE (If Not pCurrentContentRow.IsBarCodeInfoNull AndAlso pBarCodeResPosition.BarCodeInfo <> pCurrentContentRow.BarCodeInfo Then)
                            'Update BarCode fields (note that BarcodeInfo, BarcodeStatus (OK), ScannedPosition (True))
                            updatedRcpDS.twksWSRotorContentByPosition.ImportRow(pBarCodeResPosition)

                            updatedRcpDS.twksWSRotorContentByPosition(0).BeginEdit()
                            updatedRcpDS.twksWSRotorContentByPosition(0).AnalyzerID = pAnalyzerID
                            updatedRcpDS.twksWSRotorContentByPosition(0).WorkSessionID = pWorkSessionID
                            updatedRcpDS.twksWSRotorContentByPosition(0).SetBarcodeStatusNull() 'AG 26/09/2011 - Fw sends OK but Sw can transform it into ERROR, so not change it
                            updatedRcpDS.twksWSRotorContentByPosition(0).EndEdit()
                            updatedRcpDS.AcceptChanges()
                            updateAdditonalRcpFlag = False
                        End If 'If Not pCurrentContentRow.IsBarCodeInfoNull AndAlso pBarCodeResPosition.BarCodeInfo <> pCurrentContentRow.BarCodeInfo Then

                        'Update table RotorContentByPosition
                        If (Not resultData.HasError) Then
                            resultData = rcpDelegate.UpdateBarCodeFields(dbConnection, updatedRcpDS, updateAdditonalRcpFlag)
                        End If

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
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "BarcodeWSDelegate.SaveOKReadSamplesRotorPosition", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function
#End Region

#Region "Decoding barcode methods"

        ''' <summary>
        ''' Decode the Sample Barcode read (Sw must use the configuration of the Sample BarCode defined by user)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pBarCodeInfo">Group of characters in the read BarCode</param>
        ''' <returns>GlobalDataTO containing a typed DataSet BarCodesDS with a fields in the Samples Barcode decoded: ExternalPID, 
        '''          PatientID (only if the patient has been sent previously from the LIS and exists in the DB, and optionally, the 
        '''          internal code used for the informed SampleType</returns>
        ''' <remarks>
        ''' Created by:  DL 02/08/2011
        ''' Modified by: AG 30/08/2011 - Mapping BarCode SampleType (ExternalSampleType) with the internal SampleType code
        '''              AG 07/09/2011 - When BarCode manual entry is performed over a pediatric tube then no decode the sample, 
        '''                              considers pBarCodeInfo is directly ExternalSampleID (and SampleType NULL)
        '''              SA 16/09/2011 - Search if the decoded ExternalPID corresponds to a Patient that exists in DB and in this
        '''                              case, inform field PatientID in the BarCodesDS to return
        '''              SG 12/03/2013 - New treatment of the incoming Barcode (truncate if length > than configured)
        '''              TR 09/04/2013 - Verify the Barcode length is lower than or equal to the length programmed in 
        '''                              UserSettingsEnum.BARCODE_FULL_TOTAL to allow flexible Barcode
        '''              SA 09/04/2013 - Removed getting of MaxValue of limit SAMPLE_BARCODE_SIZE_LIMIT: it is not needed due to now value  
        '''                              of setting BARCODE_FULL_TOTAL is fixed and equal to that MaxValue. When the Barcode contains a 
        '''                              SampleID, verify if the obtained value corresponds to a PatientID of table tparPatients
        '''              AG 29/08/2013 - change in v2.1.1 MyExternalPID will be always the pBarCodeInfo 
        '''              XB+JC 09/10/2013 - Correction on Load Virtual Rotors #1274 Bugs tracking
        '''                                 Fix Bug #1274 and Mark as Error Samples with Incomplete Barcode (length(barcode) is less than SampleType.Position)
        ''' </remarks>
        Public Function DecodeSamplesBarCode(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pBarCodeInfo As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myHasError As Boolean = False
                        Dim myUserSettingDelegate As New UserSettingsDelegate
                        Dim myExternalPID As String = ""
                        Dim myExternalSampleType As String = ""
                        Dim myBarcodeInfo As String = ""
                        Dim myIni As Integer = -1
                        Dim myEnd As Integer = -1
                        Dim myErrMsg As String = ""
                        Dim myErrorException As New Exception

                        'DL 03/10/2011
                        Dim BarCodeFullTotal As Integer = 1
                        resultData = myUserSettingDelegate.GetCurrentValueBySettingID(dbConnection, _
                                                           GlobalEnumerates.UserSettingsEnum.BARCODE_FULL_TOTAL.ToString())
                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            BarCodeFullTotal = CType(resultData.SetDatos, Integer)
                        Else
                            myHasError = True
                            myErrMsg = resultData.ErrorMessage
                        End If

                        'SG 12/03/2013
                        'Get BARCODE_SAMPLEID_FLAG - This flag indicates if the BC is configured with a SampleID part and optionally a SampleType 
                        '                            part (when TRUE), or if it is used as read (when FALSE)
                        Dim BarCodeSampleIdFlag As Boolean = False
                        resultData = myUserSettingDelegate.GetCurrentValueBySettingID(dbConnection, GlobalEnumerates.UserSettingsEnum.BARCODE_SAMPLEID_FLAG.ToString())
                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            BarCodeSampleIdFlag = (CType(resultData.SetDatos, Integer) > 0)
                        Else
                            myHasError = True
                            myErrMsg = resultData.ErrorMessage
                        End If

                        If (Not myHasError) Then
                            myBarcodeInfo = pBarCodeInfo

                            'TR 09/04/2013 - Verify the Barcode length is lower than or equal to the length programmed in 
                            '                UserSettingsEnum.BARCODE_FULL_TOTAL to allow flexible Barcode
                            If (pBarCodeInfo.Length <= BarCodeFullTotal) Then
                                If (BarCodeSampleIdFlag) Then
                                    'Get lower limit for SampleID part in the Barcode (BARCODE_EXTERNAL_INI)
                                    resultData = myUserSettingDelegate.GetCurrentValueBySettingID(dbConnection, GlobalEnumerates.UserSettingsEnum.BARCODE_EXTERNAL_INI.ToString())
                                    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                        myIni = CType(resultData.SetDatos, Integer)
                                    Else
                                        myHasError = True
                                        myErrorException = New Exception(resultData.ErrorCode)
                                        myErrMsg = resultData.ErrorMessage
                                    End If

                                    If (Not myHasError) Then
                                        'Get upper limit for SampleID part in the Barcode (BARCODE_EXTERNAL_END)
                                        resultData = myUserSettingDelegate.GetCurrentValueBySettingID(dbConnection, GlobalEnumerates.UserSettingsEnum.BARCODE_EXTERNAL_END.ToString())
                                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                            myEnd = CType(resultData.SetDatos, Integer)
                                        Else
                                            myHasError = True
                                            myErrorException = New Exception(resultData.ErrorCode)
                                            myErrMsg = resultData.ErrorMessage
                                        End If
                                    End If

                                    If (Not myHasError) Then
                                        'AG 29/08/2013 - ExternalPID = barcode always (change in v2.1.1)
                                        'If (pBarCodeInfo.Length > 0 AndAlso pBarCodeInfo.Length >= myEnd AndAlso _
                                        '    pBarCodeInfo.Length > (myEnd - myIni) AndAlso myIni > -1 AndAlso myEnd > -1) Then
                                        '    'If (pBarCodeInfo.Length > 0 AndAlso pBarCodeInfo.Length > (myEnd - myIni) AndAlso myIni > -1 AndAlso myEnd > -1) Then
                                        '    myExternalPID = pBarCodeInfo.Substring(myIni - 1, (myEnd - myIni) + 1)
                                        'Else
                                        '    myHasError = True
                                        '    myErrorException = New Exception(resultData.ErrorCode)
                                        '    myErrMsg = "Barcode Length Error"
                                        'End If

                                        If (pBarCodeInfo.Length > 0) Then
                                            myExternalPID = pBarCodeInfo
                                        Else
                                            myHasError = True
                                            myErrorException = New Exception(resultData.ErrorCode)
                                            myErrMsg = "Barcode Length Error"
                                        End If
                                        'AG 29/08/2013
                                    End If


                                    If (Not myHasError) Then
                                        'Verify if the Barcode includes the SampleType
                                        resultData = myUserSettingDelegate.GetCurrentValueBySettingID(dbConnection, GlobalEnumerates.UserSettingsEnum.BARCODE_SAMPLETYPE_FLAG.ToString())
                                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                            Dim myFlag As Boolean = CType(resultData.SetDatos, Boolean)
                                            If (myFlag) Then
                                                'Get lower limit for Sample Type part in the Barcode (BARCODE_SAMPLETYPE_INI)
                                                If (Not myHasError) Then
                                                    myIni = -1
                                                    myEnd = -1

                                                    resultData = myUserSettingDelegate.GetCurrentValueBySettingID(dbConnection, GlobalEnumerates.UserSettingsEnum.BARCODE_SAMPLETYPE_INI.ToString())
                                                    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                                        myIni = CType(resultData.SetDatos, Integer)
                                                    Else
                                                        myHasError = True
                                                        myErrorException = New Exception(resultData.ErrorCode)
                                                        myErrMsg = resultData.ErrorMessage
                                                    End If
                                                End If

                                                'Get upper limit for Sample Type part in the Barcode (BARCODE_SAMPLETYPE_END)
                                                If (Not myHasError) Then
                                                    resultData = myUserSettingDelegate.GetCurrentValueBySettingID(dbConnection, GlobalEnumerates.UserSettingsEnum.BARCODE_SAMPLETYPE_END.ToString())
                                                    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                                        myEnd = CType(resultData.SetDatos, Integer)
                                                    Else
                                                        myHasError = True
                                                        myErrorException = New Exception(resultData.ErrorCode)
                                                        myErrMsg = resultData.ErrorMessage
                                                    End If
                                                End If


                                                ' XB+JC 09/10/2013 
                                                ' JC 20/09/2013 - Fix Bug #1274
                                                ' Check if sample type positions is in the barcode
                                                'If (Not myHasError) Then
                                                '    If (pBarCodeInfo.Length > 0 AndAlso pBarCodeInfo.Length > (myEnd - myIni) AndAlso myIni > -1 AndAlso myEnd > -1) Then
                                                '        myExternalSampleType = pBarCodeInfo.Substring(myIni - 1, (myEnd - myIni) + 1)
                                                '    Else
                                                '        myHasError = True
                                                '        myErrorException = New Exception(resultData.ErrorCode)
                                                '        myErrMsg = "Barcode Length Error"
                                                '    End If
                                                'End If
                                                If (pBarCodeInfo.Length < myIni OrElse pBarCodeInfo.Length < myEnd) Then

                                                    myHasError = True
                                                    myErrorException = New Exception(resultData.ErrorCode)
                                                    myErrMsg = "Barcode Sample Type Error"

                                                    ' Check Sample Type Length
                                                ElseIf (pBarCodeInfo.Length <= 0 _
                                                        OrElse pBarCodeInfo.Length <= (myEnd - myIni) _
                                                        OrElse myIni < 0 _
                                                        OrElse myEnd < 0) Then

                                                    myHasError = True
                                                    myErrorException = New Exception(resultData.ErrorCode)
                                                    myErrMsg = "Barcode Length Error"
                                                End If

                                                If (Not myHasError) Then
                                                    myExternalSampleType = pBarCodeInfo.Substring(myIni - 1, (myEnd - myIni) + 1)
                                                End If
                                                ' XB+JC 09/10/2013 
                                            End If
                                        End If
                                    End If
                                Else
                                    'There is not a SampleID nor SampleType part in the Barcode; the full read Barcode is used as SampleID
                                    If (pBarCodeInfo.Length <= BarCodeFullTotal) Then
                                        myBarcodeInfo = pBarCodeInfo
                                        myExternalPID = pBarCodeInfo
                                    End If
                                End If
                            Else
                                myHasError = True
                                myErrorException = New Exception(resultData.ErrorCode)
                                myErrMsg = "Barcode Length Error"
                            End If

                        End If

                        If (Not myHasError) Then
                            Dim patientID As String = String.Empty
                            Dim mySampleType As String = String.Empty

                            If (BarCodeSampleIdFlag) Then
                                'Verify if the External PID read corresponds to a Patient stored in DB 
                                Dim myPatientsDelegate As New PatientDelegate

                                resultData = myPatientsDelegate.GetPatientData(dbConnection, myExternalPID)
                                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                    Dim myPatientsDS As PatientsDS = DirectCast(resultData.SetDatos, PatientsDS)
                                    If (myPatientsDS.tparPatients.Rows.Count = 1) Then patientID = myPatientsDS.tparPatients(0).PatientID
                                End If

                                'If the BC contains a SampleType, verify if the value is mapped to a valid BAx00 Sample Type code
                                If (myExternalSampleType <> String.Empty) Then
                                    Dim dlgMapping As New BarCodeSampleTypesMappingDelegate

                                    resultData = dlgMapping.ReadByExternalSampleType(dbConnection, myExternalSampleType)
                                    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                        Dim mappingDS As BarCodeSampleTypesMappingDS = DirectCast(resultData.SetDatos, BarCodeSampleTypesMappingDS)

                                        If (mappingDS.tcfgBarCodeSampleTypesMapping.Rows.Count > 0 AndAlso Not mappingDS.tcfgBarCodeSampleTypesMapping(0).IsSampleTypeNull) Then
                                            mySampleType = mappingDS.tcfgBarCodeSampleTypesMapping(0).SampleType
                                        End If
                                    End If
                                End If
                            End If

                            'Finally, add a row to a BarCodesDS with all information obtained from the scanned Sample Barcode
                            Dim myDataSet As New BarCodesDS
                            Dim myDataRow As BarCodesDS.DecodedSamplesFieldsRow

                            myDataRow = myDataSet.DecodedSamplesFields.NewDecodedSamplesFieldsRow
                            myDataRow.BarcodeInfo = myBarcodeInfo

                            myDataRow.ExternalPID = myExternalPID
                            If (patientID = String.Empty) Then myDataRow.SetPatientIDNull() Else myDataRow.PatientID = patientID
                            If (myExternalSampleType = String.Empty) Then myDataRow.SetExternalSampleTypeNull() Else myDataRow.ExternalSampleType = myExternalSampleType
                            If (mySampleType = String.Empty) Then myDataRow.SetSampleTypeNull() Else myDataRow.SampleType = mySampleType

                            myDataSet.DecodedSamplesFields.AddDecodedSamplesFieldsRow(myDataRow)
                            myDataSet.AcceptChanges()

                            'Return the BarCodesDS
                            resultData.SetDatos = myDataSet
                            resultData.HasError = False
                        Else
                            'Throw New Exception(myErrMsg, myErrorException) 'SGM 13/03/2013 - trace error
                            resultData.HasError = True
                            resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                            resultData.ErrorMessage = myErrMsg
                            resultData.SetDatos = Nothing
                        End If

                    End If
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "BarcodeWSDelegate.DecodeSamplesBarCode", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function


        ''' <summary>
        ''' Decode the Barcode Reagent read (Sw must use the Reagents Barcode configuration fixed by factory)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pBarCodeInfo">Scanned Reagent Barcode</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pCellNumber">Cell Number</param>
        ''' <returns>GlobalDataTO containing a typed Dataset BarCodesDS with the information obtained after decoding the scanned Barcode. 
        '''          Data is contained in subtable DecodedReagentsFields</returns>
        ''' <remarks>
        ''' Created by:  DL 02/08/2011
        ''' Modified by: AG 30/08/2011 - Added Business for internal fields (IsReagentFlag, IsExternalReagentFlag, ReagentID, SolutionCode)
        '''              TR 27/09/2011 - Before start the Barcode decoding, get the Check Digit and verify it is correct
        '''              AG 10/10/2011 - Big bottles (BottleType=2 or TubeType=BOTTLE3) are not allowed in the external ring in Reagents Rotor  
        '''              TR 26/01/2012 - Changed field CodeTest from INTEGER to STRING
        '''              TR 27/01/2012 - Validate values are not NULL to avoid exceptions
        '''              SA 30/03/2012 - Once the Bottle Type field has been decoded, get the full bottle volume and inform field Bottle Volume 
        '''                              in the DS to return
        ''' </remarks>
        Public Function DecodeReagentsBarCode(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pBarCodeInfo As String, ByVal pAnalyzerID As String, _
                                              ByVal pCellNumber As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myErrCode As String = String.Empty
                        Dim myErrMsg As String = String.Empty
                        Dim myHasError As Boolean = False

                        'Get volume of all types of bottles available for Reagents and Additional Solutions
                        Dim myBottleTypesDS As New ReagentTubeTypesDS
                        Dim myBottleTypesDelegate As New ReagentTubeTypesDelegate

                        resultData = myBottleTypesDelegate.GetReagentBottles(dbConnection)
                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            myBottleTypesDS = DirectCast(resultData.SetDatos, ReagentTubeTypesDS)
                        End If

                        'Get all defined Field Limits
                        Dim fieldDS As New FieldLimitsDS
                        If (Not resultData.HasError) Then
                            Dim fieldLimitsConfig As New FieldLimitsDelegate

                            resultData = fieldLimitsConfig.GetAllList(dbConnection)
                            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                fieldDS = DirectCast(resultData.SetDatos, FieldLimitsDS)
                            End If
                        End If

                        If (Not resultData.HasError) Then
                            'Const myBottleFullVolume As Single = 0
                            Dim myCodeTest As String = "-1"
                            Dim myBottleType As Integer = -1
                            Dim myReagentType As Integer = -1
                            Dim myMonthExpedition As String = ""
                            Dim myYearExpedition As String = ""
                            Dim myBottleLotNumber As String = ""
                            Dim myLotNumber As String = ""
                            Dim myCheckDigit As Integer = -1
                            Dim myIni As Integer = -1
                            Dim myEnd As Integer = -1

                            'Validate the Barcode read has the proper length 
                            Dim myList As List(Of FieldLimitsDS.tfmwFieldLimitsRow) = (From a As FieldLimitsDS.tfmwFieldLimitsRow In fieldDS.tfmwFieldLimits _
                                                                                      Where String.Compare(a.LimitID, GlobalEnumerates.FieldLimitsEnum.BARCODE_CHKDIGIT_LIMIT.ToString, False) = 0 _
                                                                                     Select a).ToList
                            If (myList.Count > 0) Then
                                If (pBarCodeInfo.Length = 0 OrElse pBarCodeInfo.Length < myList(0).MaxValue) Then
                                    myHasError = True
                                    myErrCode = GlobalEnumerates.Messages.MASTER_DATA_MISSING.ToString()
                                Else
                                    'Validate the Check Digit is correct
                                    resultData = IsCheckDigitCorrect(pBarCodeInfo, CInt(myList(0).MaxValue))
                                    If (Not resultData.HasError) Then
                                        'Get CODE TEST field
                                        myList = (From a As FieldLimitsDS.tfmwFieldLimitsRow In fieldDS.tfmwFieldLimits _
                                                 Where String.Compare(a.LimitID, GlobalEnumerates.FieldLimitsEnum.BARCODE_CODETEST_LIMIT.ToString, False) = 0 _
                                                Select a).ToList

                                        If (myList.Count > 0) Then
                                            myIni = CType(myList(0).MinValue, Integer)
                                            myEnd = CType(myList(0).MaxValue, Integer)

                                            If (myIni > -1 And myEnd > -1) Then
                                                myCodeTest = pBarCodeInfo.Substring(myIni - 1, (myEnd - myIni) + 1)
                                            Else
                                                myHasError = True
                                                myErrMsg = "Barcode Length Error"
                                            End If
                                        Else
                                            myHasError = True
                                            myErrCode = GlobalEnumerates.Messages.MASTER_DATA_MISSING.ToString()
                                        End If

                                        'Get BOTTLE TYPE field
                                        If (Not myHasError) Then
                                            myList = (From a As FieldLimitsDS.tfmwFieldLimitsRow In fieldDS.tfmwFieldLimits _
                                                     Where String.Compare(a.LimitID, GlobalEnumerates.FieldLimitsEnum.BARCODE_BOTTLETYPE_LIMIT.ToString, False) = 0 _
                                                    Select a).ToList

                                            If (myList.Count > 0) Then
                                                myIni = CType(myList(0).MinValue, Integer)
                                                myEnd = CType(myList(0).MaxValue, Integer)

                                                If (myIni > -1 And myEnd > -1) Then
                                                    myBottleType = CType(pBarCodeInfo.Substring(myIni - 1, (myEnd - myIni) + 1), Integer)

                                                    'TR 08/05/2012 -Change Values: Fixed: 1 = 60ml, 2 = 20ml.
                                                    If (myBottleType < 1 OrElse myBottleType > 2) Then
                                                        myHasError = True
                                                        myErrMsg = "Barcode Data Error"
                                                    End If
                                                Else
                                                    myHasError = True
                                                    myErrMsg = "Barcode Length Error"
                                                End If
                                            Else
                                                myHasError = True
                                                myErrCode = GlobalEnumerates.Messages.MASTER_DATA_MISSING.ToString()
                                            End If
                                        End If
                                    Else
                                        myHasError = resultData.HasError
                                        myErrMsg = resultData.ErrorMessage
                                    End If

                                    'Get REAGENT NUMBER field
                                    If (Not myHasError) Then
                                        myList = (From a As FieldLimitsDS.tfmwFieldLimitsRow In fieldDS.tfmwFieldLimits _
                                                 Where String.Compare(a.LimitID, GlobalEnumerates.FieldLimitsEnum.BARCODE_REAGTYPE_LIMIT.ToString, False) = 0 _
                                                Select a).ToList

                                        If (myList.Count > 0) Then
                                            myIni = CType(myList(0).MinValue, Integer)
                                            myEnd = CType(myList(0).MaxValue, Integer)

                                            If (myIni > -1 And myEnd > -1) Then
                                                myReagentType = CType(pBarCodeInfo.Substring(myIni - 1, (myEnd - myIni) + 1), Integer)
                                            Else
                                                myHasError = True
                                                myErrMsg = "Barcode Length Error"
                                            End If
                                        Else
                                            myHasError = True
                                            myErrCode = GlobalEnumerates.Messages.MASTER_DATA_MISSING.ToString()
                                        End If
                                    End If

                                    'Get MONTH of EXP DATE field
                                    If (Not myHasError) Then
                                        myList = (From a As FieldLimitsDS.tfmwFieldLimitsRow In fieldDS.tfmwFieldLimits _
                                                 Where String.Compare(a.LimitID, GlobalEnumerates.FieldLimitsEnum.BARCODE_MONTHEXP_LIMIT.ToString, False) = 0 _
                                                Select a).ToList

                                        If (myList.Count > 0) Then
                                            myIni = CType(myList(0).MinValue, Integer)
                                            myEnd = CType(myList(0).MaxValue, Integer)

                                            If (myIni > -1 And myEnd > -1) Then
                                                myMonthExpedition = pBarCodeInfo.Substring(myIni - 1, (myEnd - myIni) + 1).ToString
                                            Else
                                                myHasError = True
                                                myErrMsg = "Barcode Length Error"
                                            End If
                                        Else
                                            myHasError = True
                                            myErrCode = GlobalEnumerates.Messages.MASTER_DATA_MISSING.ToString()
                                        End If
                                    End If

                                    'Get YEAR of EXP DATE field
                                    If (Not myHasError) Then
                                        myList = (From a As FieldLimitsDS.tfmwFieldLimitsRow In fieldDS.tfmwFieldLimits _
                                                 Where String.Compare(a.LimitID, GlobalEnumerates.FieldLimitsEnum.BARCODE_YEAREXP_LIMIT.ToString, False) = 0 _
                                                Select a).ToList

                                        If (myList.Count > 0) Then
                                            myIni = CType(myList(0).MinValue, Integer)
                                            myEnd = CType(myList(0).MaxValue, Integer)

                                            If (myIni > -1 And myEnd > -1) Then
                                                myYearExpedition = pBarCodeInfo.Substring(myIni - 1, (myEnd - myIni) + 1).ToString
                                            Else
                                                myHasError = True
                                                myErrMsg = "Barcode Length Error"
                                            End If
                                        Else
                                            myHasError = True
                                            myErrCode = GlobalEnumerates.Messages.MASTER_DATA_MISSING.ToString()
                                        End If
                                    End If

                                    'TR 10/04/2014 BT#1583 -'Validate Expiration Date
                                    ' If (Not myHasError) Then
                                    'Dim myExpirationDate As Date
                                    ''Add to year expiration the 2000 to avoid error of 1900
                                    'myYearExpedition = "20" & myYearExpedition
                                    'Date.TryParse("01" & "-" & myMonthExpedition & "-" & myYearExpedition, myExpirationDate)
                                    'If myExpirationDate = Date.MinValue Then
                                    '                                    myHasError = True
                                    'myErrMsg = "Invalid Expiration Date"
                                    'End If
                                    'End If
                                    'TR 10/04/2014 BT#1583 -END

                                    'Get BOTTLE NUMBER field
                                    If (Not myHasError) Then
                                        myList = (From a As FieldLimitsDS.tfmwFieldLimitsRow In fieldDS.tfmwFieldLimits _
                                                 Where String.Compare(a.LimitID, GlobalEnumerates.FieldLimitsEnum.BARCODE_BOTTLENUMBER_LIMIT.ToString, False) = 0 _
                                                Select a).ToList

                                        If (myList.Count > 0) Then
                                            myIni = CType(myList(0).MinValue, Integer)
                                            myEnd = CType(myList(0).MaxValue, Integer)

                                            If (myIni > -1 And myEnd > -1) Then
                                                myBottleLotNumber = pBarCodeInfo.Substring(myIni - 1, (myEnd - myIni) + 1).ToString
                                            Else
                                                myHasError = True
                                                myErrMsg = "Barcode Length Error"
                                            End If
                                        Else
                                            myHasError = True
                                            myErrCode = GlobalEnumerates.Messages.MASTER_DATA_MISSING.ToString()
                                        End If
                                    End If

                                    'Get LOT NUMBER field
                                    If (Not myHasError) Then
                                        myList = (From a As FieldLimitsDS.tfmwFieldLimitsRow In fieldDS.tfmwFieldLimits _
                                                 Where String.Compare(a.LimitID, GlobalEnumerates.FieldLimitsEnum.BARCODE_LOTNUMBER_LIMIT.ToString, False) = 0 _
                                                Select a).ToList

                                        If (myList.Count > 0) Then
                                            myIni = CType(myList(0).MinValue, Integer)
                                            myEnd = CType(myList(0).MaxValue, Integer)

                                            If (myIni > -1 And myEnd > -1) Then
                                                myLotNumber = pBarCodeInfo.Substring(myIni - 1, (myEnd - myIni) + 1).ToString
                                            Else
                                                myHasError = True
                                                myErrMsg = "Barcode Length Error"
                                            End If
                                        Else
                                            myHasError = True
                                            myErrCode = GlobalEnumerates.Messages.MASTER_DATA_MISSING.ToString()
                                        End If
                                    End If

                                    'Get CHECK DIGIT field
                                    If (Not myHasError) Then
                                        myList = (From a As FieldLimitsDS.tfmwFieldLimitsRow In fieldDS.tfmwFieldLimits _
                                                 Where String.Compare(a.LimitID, GlobalEnumerates.FieldLimitsEnum.BARCODE_CHKDIGIT_LIMIT.ToString, False) = 0 _
                                                Select a).ToList

                                        If (myList.Count > 0) Then
                                            myIni = CType(myList(0).MinValue, Integer)
                                            myEnd = CType(myList(0).MaxValue, Integer)

                                            If (myIni > -1 And myEnd > -1) Then
                                                myCheckDigit = CType(pBarCodeInfo.Substring(myIni - 1, (myEnd - myIni) + 1), Integer)
                                            Else
                                                myHasError = True
                                                myErrMsg = "Barcode Length Error"
                                            End If
                                        Else
                                            myHasError = True
                                            myErrCode = GlobalEnumerates.Messages.MASTER_DATA_MISSING.ToString()
                                        End If
                                    End If
                                End If
                            Else
                                myHasError = True
                                myErrMsg = "Don't find Check digit field"
                            End If

                            'AG 10/10/2011 - Big bottles (BOTTLE3) can be placed only in the internal ring 
                            If (Not myHasError AndAlso myBottleType = 1) Then 'TR 08/05/2012 -Change bottle value from 2 to one (change specifications)
                                Dim externalReagentsRingMaxBottle As Integer = 44 'Configuration for BA400

                                Dim analyzerModel As New AnalyzerModelRotorsConfigDelegate

                                resultData = analyzerModel.GetAnalyzerRotorsConfiguration(dbConnection, pAnalyzerID)
                                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                    Dim analyzerModelConfDS As AnalyzerModelRotorsConfigDS = DirectCast(resultData.SetDatos, AnalyzerModelRotorsConfigDS)

                                    If (analyzerModelConfDS.tfmwAnalyzerModelRotorsConfig.Rows.Count > 0) Then
                                        Dim linqRes As List(Of AnalyzerModelRotorsConfigDS.tfmwAnalyzerModelRotorsConfigRow)
                                        linqRes = (From a As AnalyzerModelRotorsConfigDS.tfmwAnalyzerModelRotorsConfigRow In analyzerModelConfDS.tfmwAnalyzerModelRotorsConfig _
                                                  Where String.Compare(a.RotorType, "REAGENTS", False) = 0 AndAlso a.RingNumber = 1 AndAlso a.BarcodeReaderFlag = True Select a).ToList

                                        If (linqRes.Count > 0) Then
                                            externalReagentsRingMaxBottle = linqRes.First.LastCellNumber
                                        End If
                                        linqRes = Nothing
                                    End If
                                End If

                                If (pCellNumber <= externalReagentsRingMaxBottle) Then
                                    myHasError = True
                                    myErrMsg = "Invalid bottle type"
                                End If
                            End If
                            'AG 10/10/2011

                            If (Not myHasError) Then
                                Dim myDataSet As New BarCodesDS
                                Dim myDataRow As BarCodesDS.DecodedReagentsFieldsRow

                                myDataRow = myDataSet.DecodedReagentsFields.NewDecodedReagentsFieldsRow
                                myDataRow.CodeTest = myCodeTest
                                'TR 08/05/2012 -The values for the bottle type has change Now 1=60 ml. and 2=20ml  
                                If (myBottleType = 1) Then
                                    myDataRow.BottleType = "BOTTLE3" '60ml

                                ElseIf (myBottleType = 2) Then
                                    myDataRow.BottleType = "BOTTLE2" '20ml
                                End If

                                'Inform field Bottle Volume according the Bottle Type
                                If (myBottleTypesDS.ReagentTubeTypes.Where(Function(a) String.Compare(a.TubeCode, myDataRow.BottleType, False) = 0).Count() > 0) Then
                                    myDataRow.BottleVolume = myBottleTypesDS.ReagentTubeTypes.Where(Function(a) String.Compare(a.TubeCode, myDataRow.BottleType, False) = 0).First.TubeVolume
                                End If

                                myDataRow.ReagentType = myReagentType
                                myDataRow.MonthExpedition = myMonthExpedition
                                myDataRow.YearExpedition = myYearExpedition
                                myDataRow.BottleLotNumber = myBottleLotNumber
                                myDataRow.LotNumber = myLotNumber
                                myDataRow.CheckDigit = myCheckDigit

                                'AG 30/08/2011 - Business for internal fields (IsReagentFlag, IsExternalReagentFlag, ReagentID, SolutionCode)
                                Dim swParam As New SwParametersDelegate
                                Dim myParamList As New List(Of ParametersDS.tfmwSwParametersRow)

                                resultData = swParam.GetAllList(dbConnection)
                                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                    Dim paramDS As ParametersDS = DirectCast(resultData.SetDatos, ParametersDS)

                                    'Verify if the CodeTest correspondes to a Reagent or an Special Solution? 
                                    'Default values: it is Reagent and it is also a BioSystems Reagent)
                                    myDataRow.IsReagentFlag = True
                                    myDataRow.IsExternalReagentFlag = False

                                    myList = (From a As FieldLimitsDS.tfmwFieldLimitsRow In fieldDS.tfmwFieldLimits _
                                             Where String.Compare(a.LimitID, GlobalEnumerates.FieldLimitsEnum.CODETEST_ADDSOL_LIMITS.ToString, False) = 0 _
                                            Select a).ToList

                                    If (myList.Count > 0) Then
                                        If (myList(0).MaxValue >= CType(myCodeTest, Integer) And myList(0).MinValue <= CType(myCodeTest, Integer)) Then
                                            'It is an Special Solution
                                            myDataRow.IsReagentFlag = False

                                            'Search the Solution Code in the list of SW Parameters (excluding those with NULL ValueNumeric to avoid exceptions)
                                            myParamList = (From a As ParametersDS.tfmwSwParametersRow In paramDS.tfmwSwParameters _
                                                      Where Not a.IsValueNumericNull AndAlso a.ValueNumeric = CType(myCodeTest, Integer) _
                                                    AndAlso Not a.IsValueTextNull Select a).ToList
                                            If (myParamList.Count > 0) Then myDataRow.SolutionCode = myParamList(0).ValueText
                                        End If
                                    End If

                                    'It is a Reagent; check if it is a BioSystems Reagent or if it is an External one. For BioSystems Reagents, get the ReagentID
                                    If (myDataRow.IsReagentFlag) Then
                                        myParamList = (From a As ParametersDS.tfmwSwParametersRow In paramDS.tfmwSwParameters _
                                                      Where String.Compare(a.ParameterName, GlobalEnumerates.SwParameters.EXTERNAL_REAGENT_BARCODE.ToString, False) = 0 _
                                                     Select a).ToList

                                        If (myParamList.Count > 0) Then
                                            If (CType(myCodeTest, Integer) = CInt(myParamList(0).ValueNumeric)) Then
                                                'It is an External Reagent
                                                myDataRow.IsExternalReagentFlag = True
                                            Else
                                                'It is a BioSystems Reagent: search the ReagentID
                                                Dim dlgReagents As New ReagentsDelegate

                                                resultData = dlgReagents.GetByCodeTest(dbConnection, myCodeTest, myReagentType)
                                                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                                    If (CType(resultData.SetDatos, ReagentsDS).tparReagents.Count > 0) AndAlso _
                                                       (Not CType(resultData.SetDatos, ReagentsDS).tparReagents(0).IsReagentIDNull) Then
                                                        myDataRow.ReagentID = CType(resultData.SetDatos, ReagentsDS).tparReagents(0).ReagentID
                                                        myDataRow.ReagentNumber = CType(resultData.SetDatos, ReagentsDS).tparReagents(0).ReagentNumber
                                                    Else
                                                        'TODO: Here validate if the test is defined as Monoreagent or Bireagent to find 
                                                        'why the the reagent is not found, because is programmed as mono.
                                                        resultData.HasError = True
                                                        resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                                                        resultData.ErrorMessage = ""
                                                        resultData.SetDatos = Nothing
                                                    End If
                                                End If
                                            End If
                                        End If
                                    End If
                                End If
                                'END: AG 30/08/2011 - Business for internal fields

                                myDataSet.DecodedReagentsFields.AddDecodedReagentsFieldsRow(myDataRow)
                                myDataSet.AcceptChanges()

                                resultData.SetDatos = myDataSet
                            Else
                                resultData.HasError = True
                                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                                resultData.ErrorMessage = myErrMsg
                                resultData.SetDatos = Nothing
                            End If
                        End If
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "BarcodeWSDelegate.DecodeReagentsBarCode", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

#End Region

#Region "Private Methods"
        ''' <summary>
        ''' Validate if the barcode is correct by the check digit validation.
        ''' </summary>
        ''' <param name="pBarcodeRead"></param>
        ''' <returns>True on the haserror property if the Barcode is not correct 
        '''          and false if is correct.</returns>
        ''' <remarks>
        ''' CREATED BY: TR 27/09/2011
        ''' MODIFIED BY: TR 26/01/2012 - change check digit new implementation 
        ''' </remarks>
        Private Function IsCheckDigitCorrect(ByVal pBarcodeRead As String, ByVal pCheckDigitPos As Integer) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO()
            Try
                Const myErrorMessage As String = "Check Digit Error"
                Dim myResult As Boolean = False

                If pBarcodeRead.Length >= 20 Then
                    Dim mySumBC As Integer = 0
                    Dim myCheckDigit As Integer = 0
                    Dim CharsToSum As String = pBarcodeRead.Substring(0, pCheckDigitPos - 1)

                    myCheckDigit = CInt(pBarcodeRead.Substring(pCheckDigitPos - 1, 1).ToString())

                    For Each Bc_Char As String In CharsToSum
                        If IsNumeric(Bc_Char) Then
                            mySumBC = CInt(Bc_Char) + mySumBC
                        Else
                            myResult = True
                            myGlobalDataTO.ErrorMessage = myErrorMessage
                            Exit For
                        End If
                    Next
                    'Dim ModCalculation As Double = 0
                    'ModCalculation = mySumBC Mod 9

                    '26/01/2012 -NEW Calculation 
                    Dim CalculatedDigit As Integer = CInt(mySumBC.ToString().Substring(mySumBC.ToString.ToString().Length - 1, 1))

                    If Not myResult AndAlso CalculatedDigit <> myCheckDigit Then
                        myResult = True
                        myGlobalDataTO.ErrorMessage = myErrorMessage
                    End If

                Else
                    'Error no tiene el tamaño correcto.
                    myGlobalDataTO.ErrorMessage = myErrorMessage
                End If

                myGlobalDataTO.HasError = myResult

            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "BarcodeWSDelegate.ValidateCheckDigit", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function
#End Region

#Region "OLD METHODS USED FOR LIS IMPLEMENTED WITH FILES - NOT UPDATED!!"
        ''' <summary>
        ''' Decode the Sample Barcode read (Sw must use the configuration of the Sample BarCode defined by user)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pBarCodeInfo">Group of characters in the read BarCode</param>
        ''' <returns>GlobalDataTO containing a typed DataSet BarCodesDS with a fields in the Samples Barcode decoded: ExternalPID, 
        '''          PatientID (only if the patient has been sent previously from the LIS and exists in the DB, and optionally, the 
        '''          internal code used for the informed SampleType</returns>
        ''' <remarks>
        ''' Created by:  DL 02/08/2011
        ''' Modified by: AG 30/08/2011 - Mapping BarCode SampleType (ExternalSampleType) with the internal SampleType code
        '''              AG 07/09/2011 - When BarCode manual entry is performed over a pediatric tube then no decode the sample, 
        '''                              considers pBarCodeInfo is directly ExternalSampleID (and SampleType NULL)
        '''              SA 16/09/2011 - Search if the decoded ExternalPID corresponds to a Patient that exists in DB and in this
        '''                              case, inform field PatientID in the BarCodesDS to return
        ''' </remarks>
        Public Function DecodeSamplesBarCodeOLD(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pBarCodeInfo As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myHasError As Boolean = False
                        Dim myUserSettingDelegate As New UserSettingsDelegate
                        Dim myExternalPID As String = ""
                        Dim myExternalSampleType As String = ""
                        Dim myIni As Integer = -1
                        Dim myEnd As Integer = -1
                        Dim myErrMsg As String = ""

                        'DL 03/10/2011
                        Dim BarCodeFullTotal As Integer = 1
                        resultData = myUserSettingDelegate.GetCurrentValueBySettingID(dbConnection, GlobalEnumerates.UserSettingsEnum.BARCODE_FULL_TOTAL.ToString())
                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            BarCodeFullTotal = CType(resultData.SetDatos, Integer)
                        Else
                            myHasError = True
                            myErrMsg = resultData.ErrorMessage
                        End If

                        'Hay que comprobar que el codigo de barras tenga una long <=  a la programada UserSettingsEnum.BARCODE_FULL_TOTAL
                        If pBarCodeInfo.Length <= BarCodeFullTotal Then
                            'DL 03/10/2011

                            'AG 03/10/2011 - Spec change: Tube and pediatric manual barcode format is the same
                            'If (manualEntryTubeType <> "PED") Then
                            'BARCODE_EXTERNAL_INI
                            resultData = myUserSettingDelegate.GetCurrentValueBySettingID(dbConnection, GlobalEnumerates.UserSettingsEnum.BARCODE_EXTERNAL_INI.ToString())
                            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                myIni = CType(resultData.SetDatos, Integer)
                            Else
                                myHasError = True
                                myErrMsg = resultData.ErrorMessage
                            End If

                            If Not myHasError Then
                                resultData = myUserSettingDelegate.GetCurrentValueBySettingID(dbConnection, GlobalEnumerates.UserSettingsEnum.BARCODE_EXTERNAL_END.ToString())
                                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                    myEnd = CType(resultData.SetDatos, Integer)
                                Else
                                    myHasError = True
                                    myErrMsg = resultData.ErrorMessage
                                End If
                            End If

                            If Not myHasError Then
                                If pBarCodeInfo.Length > 0 AndAlso pBarCodeInfo.Length > (myEnd - myIni) AndAlso myIni > -1 AndAlso myEnd > -1 Then
                                    myExternalPID = pBarCodeInfo.Substring(myIni - 1, (myEnd - myIni) + 1)
                                Else
                                    myHasError = True
                                    myErrMsg = "Barcode Length Error"
                                End If
                            End If

                            If Not myHasError Then '(N)
                                resultData = myUserSettingDelegate.GetCurrentValueBySettingID(dbConnection, GlobalEnumerates.UserSettingsEnum.BARCODE_SAMPLETYPE_FLAG.ToString())
                                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                    Dim myFlag As Boolean = False
                                    myFlag = CType(resultData.SetDatos, Boolean)

                                    If myFlag Then

                                        'BARCODE_SAMPLETYPE_INI
                                        If Not myHasError Then
                                            myIni = -1
                                            myEnd = -1

                                            ' Get Ini and End Sample type
                                            resultData = myUserSettingDelegate.GetCurrentValueBySettingID(dbConnection, GlobalEnumerates.UserSettingsEnum.BARCODE_SAMPLETYPE_INI.ToString())
                                            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                                myIni = CType(resultData.SetDatos, Integer)
                                            Else
                                                myHasError = True
                                                myErrMsg = resultData.ErrorMessage
                                            End If
                                        End If

                                        'BARCODE_SAMPLETYPE_END
                                        If Not myHasError Then
                                            resultData = myUserSettingDelegate.GetCurrentValueBySettingID(dbConnection, GlobalEnumerates.UserSettingsEnum.BARCODE_SAMPLETYPE_END.ToString())
                                            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                                myEnd = CType(resultData.SetDatos, Integer)
                                            Else
                                                myHasError = True
                                                myErrMsg = resultData.ErrorMessage
                                            End If
                                        End If

                                        If Not myHasError Then
                                            If pBarCodeInfo.Length > 0 AndAlso pBarCodeInfo.Length > (myEnd - myIni) AndAlso myIni > -1 AndAlso myEnd > -1 Then
                                                myExternalSampleType = pBarCodeInfo.Substring(myIni - 1, (myEnd - myIni) + 1)
                                            Else
                                                myHasError = True
                                                myErrMsg = "Barcode Length Error"
                                            End If
                                        End If
                                    End If
                                End If

                            End If 'If Not myHasError Then '(N)

                        Else
                            myHasError = True
                            myErrMsg = "Barcode Length Error"
                        End If

                        'AG 03/10/2011 - Spec change: Tube and pediatric manual barcode format is the same
                        'Else
                        '    'AG 07/09/2011 - when barcode manual entry is performed over an pediatric tube then no decode sample, considers pBarCodeInfo is directly ExternalSampleID (and SampleType NULL)
                        '    myExternalPID = pBarCodeInfo
                        '    myExternalSampleType = ""

                        'End If 'If manualEntryFlag AndAlso manualEntryTubeType = "PED" Then
                        'AG 03/10/2011

                        If Not myHasError Then
                            'Verify if the External PID read corresponds to a Patient stored in DB
                            Dim patientID As String = String.Empty
                            Dim myPatientsDelegate As New PatientDelegate

                            resultData = myPatientsDelegate.ReadByExternalPID(dbConnection, myExternalPID)
                            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                Dim myPatientsDS As PatientsDS = DirectCast(resultData.SetDatos, PatientsDS)

                                If (myPatientsDS.tparPatients.Rows.Count = 1) Then
                                    patientID = myPatientsDS.tparPatients(0).PatientID
                                End If
                            End If

                            'Fill the DS
                            Dim myDataSet As New BarCodesDS
                            Dim myDataRow As BarCodesDS.DecodedSamplesFieldsRow

                            myDataRow = myDataSet.DecodedSamplesFields.NewDecodedSamplesFieldsRow

                            myDataRow.ExternalPID = myExternalPID
                            If (String.Compare(patientID, String.Empty, False) = 0) Then myDataRow.SetPatientIDNull() Else myDataRow.PatientID = patientID

                            myDataRow.SetSampleTypeNull()
                            myDataRow.SetExternalSampleTypeNull()
                            If (String.Compare(myExternalSampleType, "", False) <> 0) Then
                                myDataRow.ExternalSampleType = myExternalSampleType

                                'AG 30/08/2011 - Get the internal Sampletype
                                Dim dlgMapping As New BarCodeSampleTypesMappingDelegate
                                resultData = dlgMapping.ReadByExternalSampleType(dbConnection, myExternalSampleType)
                                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                    Dim mappingDS As New BarCodeSampleTypesMappingDS
                                    mappingDS = CType(resultData.SetDatos, BarCodeSampleTypesMappingDS)
                                    If mappingDS.tcfgBarCodeSampleTypesMapping.Rows.Count > 0 AndAlso Not mappingDS.tcfgBarCodeSampleTypesMapping(0).IsSampleTypeNull Then
                                        myDataRow.SampleType = mappingDS.tcfgBarCodeSampleTypesMapping(0).SampleType
                                        'Else
                                        'When sample type is UNKNOWN the patient is added to the list of incompleted 
                                        'patiens samples and the position is marked as not in use. This case is not consider an error.
                                        'myHasError = True
                                        'myErrMsg = "Sample Type not found"
                                    End If
                                End If
                                'AG 30/08/2011 - Get the internal Sampletype
                            End If

                            myDataSet.DecodedSamplesFields.AddDecodedSamplesFieldsRow(myDataRow)
                            myDataSet.AcceptChanges()
                            resultData.SetDatos = myDataSet
                        End If

                        If myHasError Then
                            resultData.HasError = True
                            resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                            resultData.ErrorMessage = myErrMsg
                            resultData.SetDatos = Nothing
                        End If

                    End If
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "BarcodeWSDelegate.DecodeSamplesBarCode", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()

            End Try

            Return resultData
        End Function

        ''' <summary>
        ''' Saves / update position with OK read barcode result received for SAMPLES ROTOR
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID" >Analyzer Identifier</param>
        ''' <param name="pWorkSessionID" >WorkSession Identifier</param>
        ''' <param name="pRotorType">Rotor Type: SAMPLES</param>
        ''' <param name="pBarCodeResPosition">Typed Dataset BarCodesDS containing the data read when scanning a Rotor Cell in Samples Rotor</param>
        ''' <param name="pCurrentContentRow">Content of the Rotor Cell before scanning it</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by:  AG 03/08/2011
        ''' Modified by: SA 01/09/2011 - Replaced calling to function ReadElementIdByExternalOID in OrdersDelegate by function 
        '''                              VerifyPatientExistsInActiveWS in BarcodeWSDelegate. Inform AnalyzerID and WorkSessionID in the DS used
        '''                              to update the content of a Rotor cell
        ''' </remarks>
        Private Function SaveOKReadSamplesRotorPositionOLD(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, _
                                                           ByVal pRotorType As String, ByVal pBarCodeResPosition As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow, _
                                                           ByVal pCurrentContentRow As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim updateAdditonalRcpFlag As Boolean = False
                        Dim rcpDelegate As New WSRotorContentByPositionDelegate
                        Dim updatedRcpDS As New WSRotorContentByPositionDS

                        'Execute the business logic only when the Barcode read by scanning is different from the current one
                        If (pCurrentContentRow.IsBarCodeInfoNull) OrElse _
                           (Not pCurrentContentRow.IsBarCodeInfoNull AndAlso pBarCodeResPosition.BarCodeInfo <> pCurrentContentRow.BarCodeInfo) Then
                            'Decode BarCode information
                            resultData = DecodeSamplesBarCodeOLD(dbConnection, pBarCodeResPosition.BarCodeInfo)
                            If (resultData.HasError) Then
                                'Mark position as BarCodeStatus ERROR due the barcode read is not compatible with required codification
                                resultData = SaveERRORPosition(dbConnection, pAnalyzerID, pWorkSessionID, pRotorType, pBarCodeResPosition, pCurrentContentRow)
                            Else
                                Dim decodedDataDS As BarCodesDS = DirectCast(resultData.SetDatos, BarCodesDS)
                                If (decodedDataDS.DecodedSamplesFields.Rows.Count > 0) Then
                                    'AG 16/09/2011 - When new Barcode information is read ... means the current information must be deleted before treat the new one
                                    updatedRcpDS.Clear()
                                    updatedRcpDS.twksWSRotorContentByPosition.ImportRow(pCurrentContentRow)
                                    updatedRcpDS.twksWSRotorContentByPosition(0).BeginEdit()
                                    updatedRcpDS.twksWSRotorContentByPosition(0).AnalyzerID = pAnalyzerID
                                    updatedRcpDS.twksWSRotorContentByPosition(0).WorkSessionID = pWorkSessionID
                                    updatedRcpDS.twksWSRotorContentByPosition(0).EndEdit()
                                    updatedRcpDS.AcceptChanges()

                                    'AG 11/11/2011 - If a multicalibrator point is deleted then delete the whole calibration kit
                                    Dim myMessage As String = ""
                                    resultData = rcpDelegate.CompleteDeletePositionSeleted(dbConnection, updatedRcpDS, myMessage)
                                    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                        updatedRcpDS = DirectCast(resultData.SetDatos, WSRotorContentByPositionDS)
                                    End If

                                    resultData = rcpDelegate.DeletePositions(dbConnection, updatedRcpDS, False)
                                    updatedRcpDS.Clear()
                                    'AG 16/09/2011

                                    '**BIOSYSTEMS SAMPLE BARCODE BUSINESS
                                    Dim elementID As Integer = -1
                                    Dim myReqElementsDS As New WSRequiredElementsDS

                                    Dim defaultPatientTubeType As String = "T13"
                                    Dim dlgUserSettings As New UserSettingsDelegate
                                    resultData = dlgUserSettings.GetCurrentValueBySettingID(dbConnection, GlobalEnumerates.UserSettingsEnum.DEFAULT_TUBE_PATIENT.ToString)
                                    If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                                        defaultPatientTubeType = DirectCast(resultData.SetDatos, String)
                                        If (defaultPatientTubeType = "PED") Then defaultPatientTubeType = "T13" 'Barcode can not be read for pediatrics
                                    End If

                                    'Prepare data to check if the read Barcode can be link to a required Element of the active WS
                                    Dim myBCPosWithNoRequestDS As New BarcodePositionsWithNoRequestsDS
                                    Dim myBCPosWithNoRequestRow As BarcodePositionsWithNoRequestsDS.twksWSBarcodePositionsWithNoRequestsRow


                                    myBCPosWithNoRequestRow = myBCPosWithNoRequestDS.twksWSBarcodePositionsWithNoRequests.NewtwksWSBarcodePositionsWithNoRequestsRow()
                                    myBCPosWithNoRequestRow.AnalyzerID = pAnalyzerID
                                    myBCPosWithNoRequestRow.WorkSessionID = pWorkSessionID
                                    myBCPosWithNoRequestRow.RotorType = pRotorType
                                    myBCPosWithNoRequestRow.CellNumber = pCurrentContentRow.CellNumber
                                    myBCPosWithNoRequestRow.ExternalPID = decodedDataDS.DecodedSamplesFields.First.ExternalPID
                                    If (Not decodedDataDS.DecodedSamplesFields.First.IsPatientIDNull) Then myBCPosWithNoRequestRow.PatientID = decodedDataDS.DecodedSamplesFields.First.PatientID
                                    If (Not decodedDataDS.DecodedSamplesFields.First.IsSampleTypeNull) Then myBCPosWithNoRequestRow.SampleType = decodedDataDS.DecodedSamplesFields.First.SampleType
                                    myBCPosWithNoRequestDS.twksWSBarcodePositionsWithNoRequests.AddtwksWSBarcodePositionsWithNoRequestsRow(myBCPosWithNoRequestRow)

                                    Dim myListOfBCRows As New List(Of BarcodePositionsWithNoRequestsDS.twksWSBarcodePositionsWithNoRequestsRow)
                                    myListOfBCRows.Add(myBCPosWithNoRequestRow)

                                    'Verify if there is a required Element created in the WS for the scanned Barcode
                                    Dim myBCPosWithNoRequestDelegate As New BarcodePositionsWithNoRequestsDelegate
                                    resultData = VerifyPatientExistsInActiveWS(dbConnection, pWorkSessionID, decodedDataDS)
                                    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                        myReqElementsDS = DirectCast(resultData.SetDatos, WSRequiredElementsDS)
                                        If (Not myReqElementsDS.twksWSRequiredElements(0).IsElementIDNull) Then elementID = myReqElementsDS.twksWSRequiredElements(0).ElementID

                                        If (elementID = -1) Then
                                            'The content of the scanned cell is unknown....
                                            '**Include the rotor position in the table of incomplete Patients/Samples (twksBarCodePositionWithNoRequest)
                                            Dim noTestRequestDlg As New BarcodePositionsWithNoRequestsDelegate
                                            resultData = noTestRequestDlg.AddPosition(dbConnection, pAnalyzerID, pWorkSessionID, pRotorType, pCurrentContentRow.CellNumber, decodedDataDS)

                                            '**Update the Rotor Position to indicate the cell content is unknown
                                            updatedRcpDS.twksWSRotorContentByPosition.ImportRow(pBarCodeResPosition) 'BarCodeStatus, BarcodeInfo, ScannedPosition

                                            updatedRcpDS.twksWSRotorContentByPosition(0).BeginEdit()
                                            updatedRcpDS.twksWSRotorContentByPosition(0).AnalyzerID = pAnalyzerID
                                            updatedRcpDS.twksWSRotorContentByPosition(0).WorkSessionID = pWorkSessionID
                                            updatedRcpDS.twksWSRotorContentByPosition(0).SetElementIDNull()
                                            updatedRcpDS.twksWSRotorContentByPosition(0).SetElementStatusNull()
                                            updatedRcpDS.twksWSRotorContentByPosition(0).MultiTubeNumber = 1
                                            updatedRcpDS.twksWSRotorContentByPosition(0).TubeContent = "PATIENT"
                                            updatedRcpDS.twksWSRotorContentByPosition(0).TubeType = defaultPatientTubeType
                                            updatedRcpDS.twksWSRotorContentByPosition(0).Status = "NO_INUSE"
                                            updatedRcpDS.twksWSRotorContentByPosition(0).EndEdit()

                                            updatedRcpDS.AcceptChanges()
                                            updateAdditonalRcpFlag = True
                                        Else
                                            'The content of the scanned cell is known: there are Tests requested previously for the Patient/Sample
                                            '**Update the rotor position to inform data of the required Element placed in it
                                            updatedRcpDS.twksWSRotorContentByPosition.ImportRow(pBarCodeResPosition) 'BarCodeStatus, BarcodeInfo, ScannedPosition

                                            updatedRcpDS.twksWSRotorContentByPosition(0).BeginEdit()
                                            updatedRcpDS.twksWSRotorContentByPosition(0).AnalyzerID = pAnalyzerID
                                            updatedRcpDS.twksWSRotorContentByPosition(0).WorkSessionID = pWorkSessionID
                                            updatedRcpDS.twksWSRotorContentByPosition(0).ElementID = elementID
                                            updatedRcpDS.twksWSRotorContentByPosition(0).MultiTubeNumber = 1
                                            updatedRcpDS.twksWSRotorContentByPosition(0).TubeContent = "PATIENT"
                                            updatedRcpDS.twksWSRotorContentByPosition(0).TubeType = defaultPatientTubeType
                                            updatedRcpDS.twksWSRotorContentByPosition(0).Status = "PENDING"

                                            'AG 08/09/2011 - Status can get the following values (depending the Executions by ElementID): Pending (default), InProcess, Finished, Depleted
                                            '(Maybe the same Status calculation business has to be used in method RotorContentByPositionDelegate.PatientSamplePositioning)
                                            Dim newStatus As String = ""
                                            resultData = rcpDelegate.UpdateSamplePositionStatus(dbConnection, -1, pWorkSessionID, pAnalyzerID, elementID, myReqElementsDS.twksWSRequiredElements.First.TubeContent, _
                                                                                                updatedRcpDS.twksWSRotorContentByPosition(0).MultiTubeNumber, newStatus, -1)
                                            If (Not resultData.HasError AndAlso newStatus <> String.Empty) Then updatedRcpDS.twksWSRotorContentByPosition(0).Status = newStatus
                                            'AG 08/09/2011

                                            updatedRcpDS.twksWSRotorContentByPosition(0).EndEdit()
                                            updatedRcpDS.AcceptChanges()

                                            updateAdditonalRcpFlag = True

                                            If (updatedRcpDS.twksWSRotorContentByPosition(0).Status <> "DEPLETED") Then
                                                Dim myWSRequiredElementsDelegate As New WSRequiredElementsDelegate
                                                resultData = myWSRequiredElementsDelegate.UpdateStatus(dbConnection, elementID, "POS")
                                                updatedRcpDS.twksWSRotorContentByPosition(0).ElementStatus = "POS"
                                            Else
                                                updatedRcpDS.twksWSRotorContentByPosition(0).ElementStatus = "NOPOS"
                                            End If
                                        End If
                                    End If

                                End If 'If decodedDataDS.DecodedSamplesFields.Rows.Count > 0 Then
                            End If

                        Else 'Case ELSE (If Not pCurrentContentRow.IsBarCodeInfoNull AndAlso pBarCodeResPosition.BarCodeInfo <> pCurrentContentRow.BarCodeInfo Then)
                            'Update BarCode fields (note that BarcodeInfo, BarcodeStatus (OK), ScannedPosition (True))
                            updatedRcpDS.twksWSRotorContentByPosition.ImportRow(pBarCodeResPosition)

                            updatedRcpDS.twksWSRotorContentByPosition(0).BeginEdit()
                            updatedRcpDS.twksWSRotorContentByPosition(0).AnalyzerID = pAnalyzerID
                            updatedRcpDS.twksWSRotorContentByPosition(0).WorkSessionID = pWorkSessionID
                            updatedRcpDS.twksWSRotorContentByPosition(0).SetBarcodeStatusNull() 'AG 26/09/2011 - Fw sends OK but Sw can transform it into ERROR, so not change it
                            updatedRcpDS.twksWSRotorContentByPosition(0).EndEdit()
                            updatedRcpDS.AcceptChanges()
                            updateAdditonalRcpFlag = False
                        End If 'If Not pCurrentContentRow.IsBarCodeInfoNull AndAlso pBarCodeResPosition.BarCodeInfo <> pCurrentContentRow.BarCodeInfo Then

                        'Update table RotorContentByPosition
                        If (Not resultData.HasError) Then
                            resultData = rcpDelegate.UpdateBarCodeFields(dbConnection, updatedRcpDS, updateAdditonalRcpFlag)
                        End If

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
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "BarcodeWSDelegate.SaveOKReadSamplesRotorPosition", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Verify if a scanned Patient Sample already exists in the active WorkSession and in this case, it returns the identifier of the
        ''' correspondent required Element; otherwise, the value returned is minus one
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pDecodedDataDS">Typed Dataset BarCodesDS containing the data read when scanning a Rotor Cell in Samples Rotor</param>
        ''' <returns>GlobalDataTO containing a typed DataSet WSRequiredElementsDS with information of the scanned Patient Sample when it is already 
        '''          included in the specified WorkSession</returns>
        ''' <remarks>
        ''' Created by:  SA 
        ''' Modified by: AG 08/09/2011 - Return value in the GlobalDataTO changed from an Integer to a typed DataSet WSRequiredElementsDS due to the 
        '''                              method who treat the results need values of ElementID, TubeContent and MultiItemNumber and not only the 
        '''                              ElementID
        '''              SA 16/09/2011 - Verification of ExternalPID corresponding to a Patient that already exist in DB was moved to function 
        '''                              DecodeSamplesBarCode. That information is now included in the received DataSet pDecodedDataDS
        ''' </remarks>
        Private Function VerifyPatientExistsInActiveWS(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, _
                                                       ByVal pDecodedDataDS As BarCodesDS) As GlobalDataTO
            Dim elementID As Integer = -1
            Dim myReqElementsDS As New WSRequiredElementsDS
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then

                        Dim patientExist As Boolean = (Not pDecodedDataDS.DecodedSamplesFields(0).IsPatientIDNull)
                        Dim patientSampleID As String = pDecodedDataDS.DecodedSamplesFields(0).ExternalPID
                        If (patientExist) Then patientSampleID = pDecodedDataDS.DecodedSamplesFields(0).PatientID

                        Dim sampleType As String = String.Empty
                        If (Not resultData.HasError) Then
                            'Verify if the Sample Type is informed (it was included in the BarCode and it could be mapped)
                            If (Not pDecodedDataDS.DecodedSamplesFields(0).IsSampleTypeNull) OrElse (String.Compare(pDecodedDataDS.DecodedSamplesFields(0).SampleType, String.Empty, False) <> 0) Then
                                sampleType = pDecodedDataDS.DecodedSamplesFields(0).SampleType
                            End If
                            'SA-TR 23/09/2011 
                            'If SampleType is not informed in the BarCode, search if there are requested Order Tests for the Patient in the active WS 
                            'If (sampleType = String.Empty) Then
                            '    Dim myOrderTestsDelegate As New OrderTestsDelegate
                            '    resultData = myOrderTestsDelegate.GetSampleTypesByPatient(dbConnection, pWorkSessionID, patientExist, patientSampleID)

                            '    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            '        Dim myOrderTestsDS As OrderTestsDS = DirectCast(resultData.SetDatos, OrderTestsDS)

                            '        If (myOrderTestsDS.twksOrderTests.Rows.Count = 1) Then
                            '            sampleType = myOrderTestsDS.twksOrderTests(0).SampleType
                            '        End If
                            '    End If
                            'End If
                            'SA-TR 23/09/2011 -END
                        End If

                        If (Not resultData.HasError) Then
                            'Verify if the Element already exists in the table of required elements for the active WS
                            If (String.Compare(sampleType, String.Empty, False) <> 0) Then
                                'Dim myReqElementsDS As New WSRequiredElementsDS 'AG 08/09/2011
                                Dim myReqElementsDR As WSRequiredElementsDS.twksWSRequiredElementsRow = myReqElementsDS.twksWSRequiredElements.NewtwksWSRequiredElementsRow

                                myReqElementsDR.WorkSessionID = pWorkSessionID
                                myReqElementsDR.TubeContent = "PATIENT"
                                myReqElementsDR.SampleType = sampleType
                                myReqElementsDR.OnlyForISE = False
                                myReqElementsDR.SetPredilutionFactorNull()

                                If (patientExist) Then
                                    myReqElementsDR.PatientID = pDecodedDataDS.DecodedSamplesFields(0).PatientID
                                Else
                                    myReqElementsDR.SampleID = pDecodedDataDS.DecodedSamplesFields(0).ExternalPID
                                End If

                                myReqElementsDR.SetPatientIDNull()
                                myReqElementsDR.SampleID = patientSampleID
                                myReqElementsDS.twksWSRequiredElements.AddtwksWSRequiredElementsRow(myReqElementsDR)

                                Dim myWSReqElementDelegate As New WSRequiredElementsDelegate
                                resultData = myWSReqElementDelegate.ExistRequiredElement(dbConnection, myReqElementsDS, True)

                                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                    myReqElementsDS = DirectCast(resultData.SetDatos, WSRequiredElementsDS)

                                    If (myReqElementsDS.twksWSRequiredElements.Rows.Count = 1) Then
                                        'If there is a tube for the Patient Sample, get the ElementID
                                        elementID = myReqElementsDS.twksWSRequiredElements(0).ElementID
                                    Else
                                        'AG 08/09/2011 - add a row with elementID = -1
                                        myReqElementsDS.Clear()
                                    End If
                                End If
                            End If
                        End If
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "BarcodeWSDelegate.VerifyPatientExistsInActiveWS", EventLogEntryType.Error, False)
            Finally
                'resultData.SetDatos = elementID
                If myReqElementsDS.twksWSRequiredElements.Rows.Count = 0 Then
                    Dim newRow As WSRequiredElementsDS.twksWSRequiredElementsRow
                    newRow = myReqElementsDS.twksWSRequiredElements.NewtwksWSRequiredElementsRow
                    newRow.ElementID = elementID
                    myReqElementsDS.twksWSRequiredElements.AddtwksWSRequiredElementsRow(newRow)
                    myReqElementsDS.AcceptChanges()
                End If
                resultData.SetDatos = myReqElementsDS
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Implements the business related with the reception of a barcode read instruction in Sample / Reagents Rotor
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pBarCodeRotorContentDS">Typed DS WSRotorContentByPositionDS</param>
        ''' <param name="pRotorType">Rotor Type</param>
        ''' <returns>RotorContentByPositionDS with new rotor type contents (inside a GlobalDataTo)</returns>
        ''' <remarks>
        ''' Created by:  AG 02/08/2011
        ''' Modified by: SA 02/09/2011 - When there is not an active WS, a new one with EMPTY status has to be created before executing
        '''                              the Barcode scanning
        '''              SA 15/09/2011 - When insert the read cell as Not InUse position get as PatientID the ExternalPID or the PatientID
        '''                              depending if the patient is already in the application DB or not
        '''              SA 27/09/2011 - Changed load of VirtualRotorPositionsDS needed to create the Not InUse Rotor Positions:
        '''                                ** For both Rotors, Samples and Reagents, Not InUse positions with ScannedFlag=False should remain with
        '''                                   the same content than before
        '''              SA 20/02/2012 - Changed the way of calculating the Status of Reagents: call function CalculateNeededBottlesAndReagentStatus instead
        '''                              of CalculateReagentStatus
        '''              SA 30/03/2012 - For both, NOT IN USE and IN USE Reagents, get fields TubeType and RealVolume from the DecodedDataDS and update
        '''                              them in tables of WS Not In Use Rotor Positions and WS Rotor Positions respectively
        '''              SA 09/09/2013 - Write a warning in the application LOG if the process of creation a new Empty WS was stopped due to a previous one still exists
        '''              AG 07/10/2014 - BA-1979 add traces into log when virtual rotor is saved with invalid values in order to find the origin
        ''' </remarks>
        Public Function ManageBarcodeInstructionOLD(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pBarCodeRotorContentDS As WSRotorContentByPositionDS, _
                                                    ByVal pRotorType As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        If (Not manualEntryFlag) Then manualEntryTubeType = String.Empty

                        Dim myAnalyzerID As String = String.Empty
                        Dim myWorkSessionID As String = String.Empty

                        If (pBarCodeRotorContentDS.twksWSRotorContentByPosition.Rows.Count > 0) Then
                            If (Not pBarCodeRotorContentDS.twksWSRotorContentByPosition(0).IsAnalyzerIDNull) Then myAnalyzerID = pBarCodeRotorContentDS.twksWSRotorContentByPosition(0).AnalyzerID
                            If (Not pBarCodeRotorContentDS.twksWSRotorContentByPosition(0).IsWorkSessionIDNull) Then myWorkSessionID = pBarCodeRotorContentDS.twksWSRotorContentByPosition(0).WorkSessionID

                            If (String.Compare(myWorkSessionID, String.Empty, False) = 0) Then
                                'Create an empty WS
                                Dim myWSOrderTestsDS As New WSOrderTestsDS
                                Dim myWSDelegate As New WorkSessionsDelegate

                                resultData = myWSDelegate.AddWorkSession(dbConnection, myWSOrderTestsDS, True, myAnalyzerID)
                                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                    'Get the ID of both: Work Session and Analyzer
                                    Dim myWorkSessionsDS As WorkSessionsDS = DirectCast(resultData.SetDatos, WorkSessionsDS)

                                    If (myWorkSessionsDS.twksWorkSessions.Rows.Count = 1) Then
                                        myWorkSessionID = myWorkSessionsDS.twksWorkSessions(0).WorkSessionID
                                        myAnalyzerID = myWorkSessionsDS.twksWorkSessions(0).AnalyzerID

                                        'If there was an existing WS and the adding of a new Empty one was stopped, write the Warning in the Application LOG
                                        If (myWorkSessionsDS.twksWorkSessions(0).CreateEmptyWSStopped) Then
                                            Dim myLogAcciones As New ApplicationLogManager()
                                            myLogAcciones.CreateLogActivity("WARNING: Source of call to add EMPTY WS when the previous one still exists", "BarcodeWSDelegate.ManageBarcodeInstructionOLD", EventLogEntryType.Error, False)
                                        End If
                                    End If
                                End If
                            End If

                            If (String.Compare(myAnalyzerID, String.Empty, False) <> 0) Then
                                'Read information of all elements in the current rotor (RotorType) BEFORE processing the Barcode results
                                Dim rcpDel As New WSRotorContentByPositionDelegate
                                resultData = rcpDel.GetRotorCurrentContentForBarcodeManagement(dbConnection, myAnalyzerID, myWorkSessionID, pRotorType)

                                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                    Dim currentContentDS As WSRotorContentByPositionDS = DirectCast(resultData.SetDatos, WSRotorContentByPositionDS)

                                    'Clear the Reagents list used in reception of Barcode instructions
                                    reagentsElemIdList.Clear()

                                    Dim positionRead As Integer = 0
                                    Dim linqCurrentPosData As List(Of WSRotorContentByPositionDS.twksWSRotorContentByPositionRow)


                                    For Each readBarcodeRow As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow In pBarCodeRotorContentDS.twksWSRotorContentByPosition
                                        If (Not readBarcodeRow.IsCellNumberNull) Then
                                            positionRead = readBarcodeRow.CellNumber

                                            linqCurrentPosData = (From a As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow In currentContentDS.twksWSRotorContentByPosition _
                                                                 Where String.Compare(a.RotorType, pRotorType, False) = 0 AndAlso a.CellNumber = positionRead _
                                                                Select a).ToList

                                            If (linqCurrentPosData.Count > 0) Then
                                                'Process information read with Barcode for the tube/bottle placed in the Rotor Cell
                                                resultData = ManageSinglePositionOLD(dbConnection, myAnalyzerID, myWorkSessionID, pRotorType, readBarcodeRow, linqCurrentPosData(0))
                                                If (resultData.HasError) Then Exit For
                                            End If
                                        End If
                                    Next
                                    linqCurrentPosData = Nothing


                                    If (Not resultData.HasError) Then
                                        'For ALL Reagents in the WS, calculates the Element Status (POS, INCOMPLETE or NOPOS), depending on the total required volume 
                                        'and the available (positioned) one
                                        If (String.Compare(pRotorType, "REAGENTS", False) = 0) Then
                                            Dim dlgReqElem As New WSRequiredElementsDelegate
                                            Dim myWSReqElementsDS As New WSRequiredElementsDS
                                            Dim myElementRow As WSRequiredElementsDS.twksWSRequiredElementsRow = myWSReqElementsDS.twksWSRequiredElements.NewtwksWSRequiredElementsRow

                                            For Each item As Integer In reagentsElemIdList
                                                myElementRow.BeginEdit()
                                                myElementRow.WorkSessionID = myWorkSessionID
                                                myElementRow.ElementID = item
                                                myElementRow.RequiredVolume = 0 'Total required volume will be recalculated by function CalculateNeededBottlesAndReagentStatus
                                                myElementRow.EndEdit()

                                                resultData = dlgReqElem.CalculateNeededBottlesAndReagentStatus(dbConnection, myAnalyzerID, myElementRow, 0)
                                                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                                    myElementRow = DirectCast(resultData.SetDatos, WSRequiredElementsDS.twksWSRequiredElementsRow)

                                                    'Update the Reagent Status...
                                                    resultData = dlgReqElem.UpdateStatus(dbConnection, item, myElementRow.ElementStatus)
                                                    If (resultData.HasError) Then Exit For
                                                Else
                                                    'Error calculating the Reagent Status...
                                                    Exit For
                                                End If
                                            Next
                                            reagentsElemIdList.Clear()
                                        End If
                                    End If

                                    If (Not resultData.HasError) Then
                                        'Read the FINAL rotor (RotorType) contents information AFTER treat the barcode results
                                        'This object must be returned to the AnalyzerManager method who has called me (ProcessCodeBarInstructionReceived)


                                        'Get information of all cells in the Rotor, filter those with Status NOT IN USE and add them to the corresponding table 
                                        resultData = rcpDel.ReadByCellNumber(dbConnection, myAnalyzerID, myWorkSessionID, pRotorType, -1)
                                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                            currentContentDS = DirectCast(resultData.SetDatos, WSRotorContentByPositionDS)

                                            'Implement LINQ to get Rotor Positions containing not in use Elements
                                            Dim myNoInUsePosition As List(Of WSRotorContentByPositionDS.twksWSRotorContentByPositionRow)
                                            myNoInUsePosition = (From e As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow In currentContentDS.twksWSRotorContentByPosition _
                                                                Where String.Compare(e.Status, "NO_INUSE", False) = 0 _
                                                              AndAlso String.Compare(e.RotorType, pRotorType, False) = 0 _
                                                               Select e).ToList

                                            If (myNoInUsePosition.Count > 0) Then
                                                'Add all NO_INUSE Elements into twksWSNotInUseRotorPositions table
                                                Dim noInUseDelegate As New WSNotInUseRotorPositionsDelegate

                                                'Delete NOT IN USE Rotor Positions previously saved for the Rotor Type and create the new ones
                                                resultData = noInUseDelegate.Reset(dbConnection, myAnalyzerID, myWorkSessionID, pRotorType)
                                                If (Not resultData.HasError) Then
                                                    Dim newNoInUseDS As New VirtualRotorPosititionsDS
                                                    Dim newRow As VirtualRotorPosititionsDS.tparVirtualRotorPosititionsRow

                                                    If (String.Compare(pRotorType, "SAMPLES", False) = 0) Then
                                                        For Each rcpNoInUseRow As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow In myNoInUsePosition
                                                            newRow = newNoInUseDS.tparVirtualRotorPosititions.NewtparVirtualRotorPosititionsRow
                                                            newRow.RingNumber = rcpNoInUseRow.RingNumber
                                                            newRow.CellNumber = rcpNoInUseRow.CellNumber

                                                            If (Not rcpNoInUseRow.IsTubeContentNull) Then newRow.TubeContent = rcpNoInUseRow.TubeContent Else newRow.SetTubeContentNull()
                                                            If (Not rcpNoInUseRow.IsMultiItemNumberNull) Then newRow.MultiItemNumber = rcpNoInUseRow.MultiItemNumber Else newRow.MultiItemNumber = 1

                                                            If (Not rcpNoInUseRow.IsScannedPositionNull) Then
                                                                newRow.ScannedPosition = rcpNoInUseRow.ScannedPosition

                                                                If (Not newRow.ScannedPosition) Then
                                                                    'In Samples Rotor, not InUse positions marked as not scanned should remains with the same information as before
                                                                    If (Not rcpNoInUseRow.IsSolutionCodeNull) Then newRow.SolutionCode = rcpNoInUseRow.SolutionCode
                                                                    If (Not rcpNoInUseRow.IsCalibratorIDNull) Then newRow.CalibratorID = rcpNoInUseRow.CalibratorID
                                                                    If (Not rcpNoInUseRow.IsControlIDNull) Then newRow.ControlID = rcpNoInUseRow.ControlID
                                                                End If

                                                                If (Not rcpNoInUseRow.IsBarCodeInfoNull) Then
                                                                    newRow.BarcodeInfo = rcpNoInUseRow.BarCodeInfo
                                                                    newRow.BarcodeStatus = rcpNoInUseRow.BarcodeStatus
                                                                    If (newRow.IsTubeContentNull) Then newRow.TubeContent = "PATIENT"

                                                                    'Decode fields PatientID and SampleType
                                                                    resultData = DecodeSamplesBarCodeOLD(dbConnection, rcpNoInUseRow.BarCodeInfo)
                                                                    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                                                        Dim decodedDataDS As BarCodesDS = DirectCast(resultData.SetDatos, BarCodesDS)

                                                                        If (decodedDataDS.DecodedSamplesFields.Rows.Count > 0) Then
                                                                            If (Not decodedDataDS.DecodedSamplesFields(0).IsPatientIDNull) Then
                                                                                newRow.PatientID = decodedDataDS.DecodedSamplesFields(0).PatientID
                                                                            ElseIf (Not decodedDataDS.DecodedSamplesFields(0).IsExternalPIDNull) Then
                                                                                newRow.PatientID = decodedDataDS.DecodedSamplesFields(0).ExternalPID
                                                                            End If
                                                                            If (Not decodedDataDS.DecodedSamplesFields(0).IsSampleTypeNull) Then newRow.SampleType = decodedDataDS.DecodedSamplesFields(0).SampleType
                                                                        End If
                                                                    End If
                                                                End If
                                                            End If
                                                            newNoInUseDS.tparVirtualRotorPosititions.AddtparVirtualRotorPosititionsRow(newRow)
                                                        Next rcpNoInUseRow

                                                    ElseIf (String.Compare(pRotorType, "REAGENTS", False) = 0) Then
                                                        For Each rcpNoInUseRow As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow In myNoInUsePosition
                                                            newRow = newNoInUseDS.tparVirtualRotorPosititions.NewtparVirtualRotorPosititionsRow
                                                            newRow.RingNumber = rcpNoInUseRow.RingNumber
                                                            newRow.CellNumber = rcpNoInUseRow.CellNumber

                                                            If (Not rcpNoInUseRow.IsTubeContentNull) Then newRow.TubeContent = rcpNoInUseRow.TubeContent Else newRow.SetTubeContentNull()
                                                            If (Not rcpNoInUseRow.IsMultiItemNumberNull) Then newRow.MultiItemNumber = rcpNoInUseRow.MultiItemNumber Else newRow.MultiItemNumber = 1

                                                            If (Not rcpNoInUseRow.IsScannedPositionNull) Then
                                                                newRow.ScannedPosition = rcpNoInUseRow.ScannedPosition

                                                                If (Not newRow.ScannedPosition) Then
                                                                    'In Reagents Rotor, not InUse positions marked as not scanned should remains with the same information as before
                                                                    If (Not rcpNoInUseRow.IsSolutionCodeNull) Then newRow.SolutionCode = rcpNoInUseRow.SolutionCode
                                                                    If (Not rcpNoInUseRow.IsReagentIDNull) Then newRow.ReagentID = rcpNoInUseRow.ReagentID

                                                                    If (Not rcpNoInUseRow.IsRealVolumeNull) Then newRow.RealVolume = rcpNoInUseRow.RealVolume
                                                                    If (Not rcpNoInUseRow.IsStatusNull AndAlso _
                                                                       (String.Compare(rcpNoInUseRow.Status, "DEPLETED", False) = 0 OrElse String.Compare(rcpNoInUseRow.Status, "FEW", False) = 0)) Then
                                                                        newRow.Status = rcpNoInUseRow.Status
                                                                    End If
                                                                End If

                                                                If (Not rcpNoInUseRow.IsBarCodeInfoNull) Then
                                                                    newRow.BarcodeInfo = rcpNoInUseRow.BarCodeInfo
                                                                    newRow.BarcodeStatus = rcpNoInUseRow.BarcodeStatus

                                                                    'Decode ReagentID or SolutionCode for Special and Washing Solutions
                                                                    resultData = DecodeReagentsBarCode(dbConnection, rcpNoInUseRow.BarCodeInfo, myAnalyzerID, newRow.CellNumber)
                                                                    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                                                        Dim decodedDataDS As BarCodesDS = DirectCast(resultData.SetDatos, BarCodesDS)


                                                                        If (decodedDataDS.DecodedReagentsFields.Rows.Count > 0) Then
                                                                            If (Not decodedDataDS.DecodedReagentsFields(0).IsReagentIDNull) Then newRow.ReagentID = decodedDataDS.DecodedReagentsFields(0).ReagentID
                                                                            If (Not decodedDataDS.DecodedReagentsFields(0).IsReagentNumberNull) Then newRow.MultiItemNumber = decodedDataDS.DecodedReagentsFields(0).ReagentNumber
                                                                            If (Not decodedDataDS.DecodedReagentsFields(0).IsSolutionCodeNull) Then newRow.SolutionCode = decodedDataDS.DecodedReagentsFields(0).SolutionCode

                                                                            If (newRow.IsTubeContentNull) Then
                                                                                If (decodedDataDS.DecodedReagentsFields(0).IsReagentFlag) Then
                                                                                    newRow.TubeContent = "REAGENT"
                                                                                Else
                                                                                    newRow.TubeContent = "SPEC_SOL"
                                                                                    If (InStr(decodedDataDS.DecodedReagentsFields(0).SolutionCode, "WASH", CompareMethod.Text) <> 0) Then newRow.TubeContent = "WASH_SOL"
                                                                                End If
                                                                            End If

                                                                            'Decode also Type and Volume for the Bottle
                                                                            If (Not decodedDataDS.DecodedReagentsFields(0).IsBottleTypeNull) Then newRow.TubeType = decodedDataDS.DecodedReagentsFields(0).BottleType
                                                                            If (Not decodedDataDS.DecodedReagentsFields(0).IsBottleVolumeNull) Then newRow.RealVolume = decodedDataDS.DecodedReagentsFields(0).BottleVolume
                                                                        End If
                                                                    End If
                                                                End If
                                                            End If
                                                            newNoInUseDS.tparVirtualRotorPosititions.AddtparVirtualRotorPosititionsRow(newRow)
                                                        Next
                                                    End If
                                                    newNoInUseDS.AcceptChanges()

                                                    'AG 07/10/2014 BA-1979 add classCalledFrom parameter
                                                    resultData = noInUseDelegate.Add(dbConnection, myAnalyzerID, pRotorType, myWorkSessionID, newNoInUseDS, WSNotInUseRotorPositionsDelegate.ClassCalledFrom.BarcodeResultsManagement)
                                                End If
                                            End If 'If myNoInUsePosition.Count > 0 Then
                                            myNoInUsePosition = Nothing

                                            'Finally, get data in all Rotor cells and return a WSRotorContentByPositionDS
                                            If (Not resultData.HasError) Then
                                                resultData = rcpDel.ReadByCellNumber(dbConnection, myAnalyzerID, myWorkSessionID, pRotorType, -1)
                                                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                                    currentContentDS = DirectCast(resultData.SetDatos, WSRotorContentByPositionDS)
                                                    resultData.SetDatos = currentContentDS 'Method returns the current rotor contents
                                                End If
                                            End If
                                        End If
                                    End If
                                End If 'If Not resultData.HasError Then'(1)
                            End If 'If myAnalyzerID <> "" Then
                        End If 'If pBarCodeRotorContentDS.twksWSRotorContentByPosition.Rows.Count > 0 Then

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
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "BarcodeWSDelegate.ManageBarcodeInstruction", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Treat each position read by barcode (OK, NOREAD or ERROR
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pAnalyzerID" ></param>
        ''' <param name="pWorkSessionID" ></param>
        ''' <param name="pRotorType"></param>
        ''' <param name="pBarCodeResPosition"></param>
        ''' <param name="pCurrentContentRow"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function ManageSinglePositionOLD(ByVal pDBConnection As SqlClient.SqlConnection, _
                                              ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, _
                                              ByVal pRotorType As String, _
                                              ByVal pBarCodeResPosition As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow, _
                                              ByVal pCurrentContentRow As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then

                        'Depending the position read status different business is required
                        Select Case pBarCodeResPosition.BarcodeStatus
                            Case "OK" 'READ OK
                                'Different business for SAMPLES & REAGENTS
                                If String.Compare(pRotorType, "SAMPLES", False) = 0 Then
                                    resultData = SaveOKReadSamplesRotorPositionOLD(dbConnection, pAnalyzerID, pWorkSessionID, pRotorType, pBarCodeResPosition, pCurrentContentRow)
                                ElseIf String.Compare(pRotorType, "REAGENTS", False) = 0 Then
                                    resultData = SaveOKReadReagentsRotorPosition(dbConnection, pAnalyzerID, pWorkSessionID, pRotorType, pBarCodeResPosition, pCurrentContentRow)
                                End If

                            Case "EMPTY" 'NOREAD
                                resultData = SaveNOREADPosition(dbConnection, pAnalyzerID, pWorkSessionID, pRotorType, pBarCodeResPosition, pCurrentContentRow)

                            Case "ERROR" 'ERROR READ
                                resultData = SaveERRORPosition(dbConnection, pAnalyzerID, pWorkSessionID, pRotorType, pBarCodeResPosition, pCurrentContentRow)

                        End Select

                        If (Not resultData.HasError) Then
                            'When the Database Connection was opened locally, then the Commit is executed
                            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                            'resultData.SetDatos = '<value to return; if any>
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
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "BarcodeWSDelegate.ManageSinglePosition", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData

        End Function
#End Region
    End Class
End Namespace