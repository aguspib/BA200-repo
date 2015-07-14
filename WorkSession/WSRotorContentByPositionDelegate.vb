Option Strict On
Option Explicit On

Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Global.TO
Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.DAL.DAO
Imports Biosystems.Ax00.Global.GlobalEnumerates
'Imports System.Drawing


Namespace Biosystems.Ax00.BL

    Partial Public Class WSRotorContentByPositionDelegate

#Region "Public Enumerates"
        ''' <summary>
        ''' This enumerate informs the process who is saving the information about the rotors positions
        ''' By now it is only use for UPDATE items, not for delete
        ''' </summary>
        ''' <remarks>
        ''' Created by: AG 07/10/2014 BA-1979
        ''' </remarks>
        Public Enum ClassCalledFrom
            NotInitiated
            ChangeElementPosition1
            ChangeElementPosition2
            ChangeElementPosition3
            ChangeElementPosition4
            ChangeElementPosition5
            ChangeElementPosition6
            ChangeVolumeByPosition1
            ChangeVolumeByPosition2
            ChangeVolumeByPosition3
            ChangeVolumeByPosition4
            ChangeVolumeByPosition5
            DeletePositions
            AdditionalTubeSolutionPositioning
            CalibratorPositioning
            ControlPositioning
            PatientSamplePositioning
            ReagentAUTOPositioning
            ReagentPositioning
            AdditionalSolutionAUTOPositioning
            AdditionalSolutionPositioning
            LoadRotor
            UpdateMultiTubeNumber
            UpdateBarcodeFields
        End Enum
#End Region

#Region "Public methods"
        ''' <summary>
        ''' Manages the automatic positioning of Additional Solutions. The default bottle used and the process will depend 
        ''' on the Analyzer Model and the Rotor Type:
        '''   For positioning in a REAGENTS Rotor (A400 Model):
        '''       * All types of Additional Solutions can be placed in this Rotor
        '''       * Several bottles of the same Additional Solution are allowed
        '''       * Process of automatic positioning, place all Additional Solutions in cells of the internal Ring, using the
        '''         biggest Bottle available
        '''   For positioning in a SAMPLES And REAGENTS Rotor (A200 Model): All the process is pending of definition
        ''' </summary>
        ''' <param name="pDBConnection">Open Database Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pRotorType">Analyzer Rotor Type</param>
        ''' <returns>GlobalDataTO containing a typed DataSet WSRotorContentByPositionDS with information of all positioned 
        '''          Additional Solutions</returns>
        ''' <remarks>
        ''' Created by:  TR 24/11/2009 - Tested: OK
        ''' Modified by: SA 04/01/2010 - Changes needed in call to GetMaxRingNumber due to a new parameter was added
        '''              SA 13/01/2010 - Changed the way of opening the DB Connection to fulfill the new template
        '''              RH 14/09/2011 - Code optimization. short-circuit evaluation. Remove unneeded and memory wasting "New" instructions
        '''              SA 12/01/2012 - This function has to open a DB Transaction instead of a DB Connection. Code improved
        '''              SA 10/02/2012 - Get also Additional Solutions marked as INCOMPLETE an positioning a bottle for each one of them in
        '''                              the internal Reagents Rotor Ring
        '''              XB 07/10/2014 - BA-1978 ==> Add log traces to catch NULL wrong assignment on RealVolume field
        '''              AG 07/10/2014 - BA-1979 ==> Replaced call to function twksWSRotorContentByPositionDAO.Update by a call to function Update 
        '''                                          in this Delegate, informing the process who is updating the content of the Rotor Position (to 
        '''                                          search which process is inserting invalid values: positions with TubeContent but not element ID)
        ''' </remarks>
        Public Function AdditionalSolutionsAutoPositioning(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, _
                                                           ByVal pAnalyzerID As String, ByVal pRotorType As String) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        If (String.Compare(pRotorType, "REAGENTS", False) = 0) Then
                            'Search all Additional Solutions in the Work Session that are not positioned in the Rotor 
                            Dim myResult As New WSRotorContentByPositionDS
                            Dim myRequiredElementsDelegate As New WSRequiredElementsDelegate

                            myGlobalDataTO = myRequiredElementsDelegate.GetRequiredSolutionsDetails(dbConnection, pWorkSessionID, Nothing, "NOPOS")
                            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                Dim myAdditionalSolutionDS As WSRequiredElementsTreeDS = DirectCast(myGlobalDataTO.SetDatos, WSRequiredElementsTreeDS)

                                'Search all Additional Solutions in the WorkSession that are positioned in the Rotor but incompleted
                                myGlobalDataTO = myRequiredElementsDelegate.GetRequiredSolutionsDetails(dbConnection, pWorkSessionID, Nothing, "INCOMPLETE")
                                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                    'Add all incomplete Additional Solutions to the DS containing the not positioned ones
                                    For Each incompleteSolution As WSRequiredElementsTreeDS.AdditionalSolutionsRow In DirectCast(myGlobalDataTO.SetDatos, WSRequiredElementsTreeDS).AdditionalSolutions
                                        myAdditionalSolutionDS.AdditionalSolutions.ImportRow(incompleteSolution)
                                    Next

                                    If (myAdditionalSolutionDS.AdditionalSolutions.Rows.Count = 0) Then
                                        'If there are not AdditionalSolutions pending to positioning, return an empty WSRotorContentByPositionDS
                                        myGlobalDataTO.SetDatos = myResult
                                    Else
                                        'Search the maximum Ring Number in the informed Analyzer and Rotor
                                        Dim maxRing As Integer = 0
                                        Dim myAnalyzerModelRotorsConfigDelegate As New AnalyzerModelRotorsConfigDelegate()

                                        myGlobalDataTO = myAnalyzerModelRotorsConfigDelegate.GetMaxRingNumber(dbConnection, pAnalyzerID, pRotorType)
                                        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                            If (Not String.IsNullOrEmpty(myGlobalDataTO.SetDatos.ToString)) Then maxRing = CType(myGlobalDataTO.SetDatos, Integer)
                                        End If

                                        'Search maximum volume of "big" bottles and the code of the bottle for the volume found
                                        Dim maxBottleSize As Integer = 0
                                        Dim bottleCode As String = String.Empty

                                        If (Not myGlobalDataTO.HasError) Then
                                            Dim myReagentTubeTypesDelegate As New ReagentTubeTypesDelegate()

                                            myGlobalDataTO = myReagentTubeTypesDelegate.GetMaximumBottleSize(dbConnection)
                                            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                                If (myGlobalDataTO.SetDatos.ToString() <> "") Then
                                                    maxBottleSize = CType(myGlobalDataTO.SetDatos, Integer)

                                                    'Get the Bottle code...
                                                    myGlobalDataTO = myReagentTubeTypesDelegate.GetBottleByVolume(dbConnection, maxBottleSize)
                                                    If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                                        bottleCode = CType(myGlobalDataTO.SetDatos, String)
                                                    End If
                                                End If
                                            End If
                                        End If

                                        Dim myRotorContentByPositionDS As WSRotorContentByPositionDS
                                        Dim myWSRotorContentByPositionDAO As New twksWSRotorContentByPositionDAO

                                        If (Not myGlobalDataTO.HasError) Then
                                            'Load all Additional Solutions pending to positioning in a WSRotorContentByPosition DataSet
                                            myRotorContentByPositionDS = ProcessAdditionalSolutions(myAdditionalSolutionDS, pWorkSessionID, pAnalyzerID, _
                                                                                                    pRotorType, maxRing, bottleCode, maxBottleSize)
                                            'Get the logged User
                                            'Dim myGlobalbase As New GlobalBase
                                            Dim myLoggedUser As String = GlobalBase.GetSessionInfo().UserName

                                            Dim myRotorContentPosTMP As New WSRotorContentByPositionDS
                                            For Each row As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow _
                                                         In myRotorContentByPositionDS.twksWSRotorContentByPosition
                                                'Get the Free cell having the maximum number in the specified Ring
                                                myGlobalDataTO = myWSRotorContentByPositionDAO.GetMaxFreeCellByRing(dbConnection, row.AnalyzerID, row.RotorType, _
                                                                                                                    row.WorkSessionID, row.RingNumber)
                                                If (myGlobalDataTO.HasError) Then Exit For

                                                If (String.IsNullOrEmpty(myGlobalDataTO.SetDatos.ToString)) Then
                                                    'There are not FREE positions in the internal ring
                                                    myGlobalDataTO.ErrorCode = "RING2_FULL"
                                                    Exit For
                                                Else
                                                    'Prepare the DataSet to update the Rotor position
                                                    row.BeginEdit()
                                                    row.CellNumber = CType(myGlobalDataTO.SetDatos, Integer)
                                                    row.TS_DateTime = DateTime.Now
                                                    row.TS_User = myLoggedUser
                                                    row.EndEdit()
                                                    myRotorContentPosTMP.twksWSRotorContentByPosition.ImportRow(row)

                                                    'Update the Rotor Cell/Position informing the Element positioned in it
                                                    'myGlobalDataTO = myWSRotorContentByPositionDAO.Update(dbConnection, myRotorContentPosTMP)
                                                    myGlobalDataTO = Update(dbConnection, pRotorType, myRotorContentPosTMP, ClassCalledFrom.AdditionalSolutionAUTOPositioning) 'AG 07/10/2014 BA-1979 call the Update method  in delegate instead of in DAO
                                                    If (myGlobalDataTO.HasError) Then Exit For

                                                    'Update Status of the Required Element to Positioned (POS)
                                                    myGlobalDataTO = myRequiredElementsDelegate.UpdateStatus(dbConnection, row.ElementID, "POS")
                                                    If (myGlobalDataTO.HasError) Then Exit For

                                                    'Finally, move the update Rotor Cell/Position to the final DS
                                                    row.BeginEdit()
                                                    row.ElementStatus = "POS"
                                                    row.EndEdit()
                                                    myResult.twksWSRotorContentByPosition.ImportRow(row)
                                                End If
                                            Next
                                        End If

                                        If (Not myGlobalDataTO.HasError) Then
                                            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                                            myGlobalDataTO.SetDatos = myResult
                                        Else
                                            If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                                        End If
                                    End If
                                End If
                            End If
                            'Else
                            'TODO: PENDING** Process for A200 Rotor for Samples and Reagents
                        End If
                    End If
                End If

            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                myGlobalDataTO.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSRotorContentByPositionDelegate.AdditionalSolutionsAutoPositioning", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Manages the positioning of Sample Additional Solutions (manual and automatic). In both cases, the default tube 
        ''' defined for the Sample Additional Solution is used. When this function is used for manual positioning, the ring 
        ''' and cell to which the User drag and drop the Element is used. When it is used for automatic positioning, a ring/cell 
        ''' is searched according the rules of automatic positioning of Sample Additional Solutions
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>        
        ''' <param name="pWSRotorContentByPositionDS">Typed DataSet containing the basic data needed to place a required 
        '''                                           Sample Additional Solutions Element in a Rotor Cell</param>
        ''' <param name="pMaxRotorRingNumber">Maximum Ring Number</param>
        ''' <param name="pAutoPositioning">Flag indicating if the function is used for manual (when false)  or automatic 
        '''                                (when true) positioning</param>
        ''' <returns>GlobalDataTO containing a typed DataSet WSRotorContentByPositionDS with the information about the Rotor 
        '''          Cell in which the Sample Additional Solutions was placed plus the recalculated Element Status
        ''' </returns>
        ''' <remarks>
        ''' Created by:  RH 16/06/2011
        ''' Modified by: AG 08/09/2011 - Calculate Sample Position Status (PENDING, INPROCESS,....)
        '''              SA 11/01/2012 - Implementation improved; use value of field ElementFinished to set Cell Status = Finished. 
        '''                              Call function GetSamplePositionStatus only when ElementFinished is False
        '''              XB 07/10/2014 - BA-1978 ==> Add log traces to catch NULL wrong assignment on RealVolume field 
        '''              AG 07/10/2014 - BA-1979 ==> Replaced call to function twksWSRotorContentByPositionDAO.Update by a call to function Update 
        '''                                          in this Delegate, informing the process who is updating the content of the Rotor Position (to 
        '''                                          search which process is inserting invalid values: positions with TubeContent but not element ID)
        ''' </remarks>
        Public Function AdditionalTubeSolutionPositioning(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWSRotorContentByPositionDS As WSRotorContentByPositionDS, _
                                                          ByVal pMaxRotorRingNumber As Integer, ByVal pAutoPositioning As Boolean) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim rotorContentRow As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow
                        rotorContentRow = pWSRotorContentByPositionDS.twksWSRotorContentByPosition(0)

                        'Search details of the Required Element needed to be positioned (SolutionCode and TubeType)
                        Dim myRequiredElementsDelegate As New WSRequiredElementsDelegate
                        myGlobalDataTO = myRequiredElementsDelegate.GetRequiredElementData(dbConnection, rotorContentRow.ElementID)

                        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                            Dim myRequiredElementsDS As WSRequiredElementsDS = DirectCast(myGlobalDataTO.SetDatos, WSRequiredElementsDS)

                            Dim solutionCode As String = String.Empty
                            Dim solutionFinished As Boolean = False
                            If (myRequiredElementsDS.twksWSRequiredElements.Rows.Count > 0) Then
                                'Save SolutionCode and ElementFinished in local variables
                                solutionCode = myRequiredElementsDS.twksWSRequiredElements(0).SolutionCode
                                solutionFinished = myRequiredElementsDS.twksWSRequiredElements(0).ElementFinished

                                'If it is not informed, set the TubeType in the entry DataSet
                                If (rotorContentRow.IsTubeTypeNull) Then rotorContentRow.TubeType = myRequiredElementsDS.twksWSRequiredElements(0).TubeType

                                If (pAutoPositioning) Then
                                    'If the function has been called for automatic positioning...search the next free position
                                    'NOTE: It is the same algorithm as for CTRL, so we pass CTRL as TubeContent indeed
                                    myGlobalDataTO = GetRotorPositionForSample(dbConnection, rotorContentRow.WorkSessionID, rotorContentRow.AnalyzerID, _
                                                                               rotorContentRow.RotorType, "CTRL", pMaxRotorRingNumber)

                                    'Validate if a warning message of Rotor Full has been sent in field ErrorCode
                                    If (Not myGlobalDataTO.HasError AndAlso String.IsNullOrEmpty(myGlobalDataTO.ErrorCode)) Then
                                        Dim myRingCellNumberDS As RingCellNumbersDS = DirectCast(myGlobalDataTO.SetDatos, RingCellNumbersDS)

                                        'Inform the found RingNumber and CellNumber in the DataSet 
                                        rotorContentRow.RingNumber = myRingCellNumberDS.RingCellTable(0).RingNumber
                                        rotorContentRow.CellNumber = myRingCellNumberDS.RingCellTable(0).CellNumber
                                    End If
                                    'Else
                                    'If the Additional Solution Tube was manually dropped in a cell, fields RingNumber and CellNumber are informed 
                                    'in the entry DS, it is not needed search the next position here
                                End If

                                If (Not myGlobalDataTO.HasError AndAlso String.IsNullOrEmpty(myGlobalDataTO.ErrorCode)) Then
                                    If (solutionFinished) Then
                                        rotorContentRow.Status = "FINISHED"
                                    Else
                                        'Get the position status for the Additional Solution...
                                        rotorContentRow.Status = "PENDING"
                                        myGlobalDataTO = GetSamplePositionStatus(dbConnection, rotorContentRow.WorkSessionID, rotorContentRow.AnalyzerID, _
                                                                                 rotorContentRow.ElementID, "BLANK", 1)
                                        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                            Dim newStatus As String = DirectCast(myGlobalDataTO.SetDatos, String)
                                            If (String.Compare(newStatus, String.Empty, False) <> 0 AndAlso String.Compare(newStatus, "FINISHED", False) <> 0) Then rotorContentRow.Status = newStatus
                                        End If
                                    End If
                                End If

                                If (Not myGlobalDataTO.HasError AndAlso String.IsNullOrEmpty(myGlobalDataTO.ErrorCode)) Then
                                    'Update the Rotor Cell / Position informing the Element positioned in it
                                    'Dim myRotorContentByPositionDAO As New twksWSRotorContentByPositionDAO()
                                    'myGlobalDataTO = myRotorContentByPositionDAO.Update(dbConnection, pWSRotorContentByPositionDS)
                                    myGlobalDataTO = Update(dbConnection, "SAMPLES", pWSRotorContentByPositionDS, ClassCalledFrom.AdditionalTubeSolutionPositioning) 'AG 07/10/2014 BA-1979 call the Update method  in delegate instead of in DAO
                                End If

                                If (Not myGlobalDataTO.HasError AndAlso myGlobalDataTO.ErrorCode = "") Then
                                    'Update the Element Status informing it is positioned
                                    myGlobalDataTO = myRequiredElementsDelegate.UpdateStatus(dbConnection, rotorContentRow.ElementID, "POS")
                                    If (Not myGlobalDataTO.HasError) Then rotorContentRow.ElementStatus = "POS"
                                End If
                            End If
                        End If

                        If (Not myGlobalDataTO.HasError) Then
                            'Commit the Transaction when it was locally opened and return the updated Rotor Positions
                            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                            myGlobalDataTO.SetDatos = pWSRotorContentByPositionDS
                        Else
                            'Rollback the Transaction when it was locally opened
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
                myGlobalDataTO.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSRotorContentByPositionDelegate.AdditionalTubeSolutionPositioning", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Manages all logic needed to drag and drop manually a required Element of a Work Session from a position in an Analyzer Rotor to another position 
        ''' in the same Rotor.       
        ''' * For positioning in a REAGENTS Rotor (A400 Model):
        '''      If the Element is moved from a Cell in the internal Ring to a Cell in the external Ring and the bottle currently used is bigger than the 
        '''      maximum bottle size allowed in the external Ring, then the bottle size is changed to the maximum allowed in the target Ring (a warning 
        '''      mensaje is not needed).
        ''' * For positioning in SAMPLES-REAGENTS Rotor (A200 Model):
        '''      All process is pending of definition
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWSRotorContentByPositionDS">Typed DataSet containing the data of the Rotor Cell where is currently 
        '''                                           the Element that has to be moved to a new Rotor Cell</param>
        ''' <param name="pToRingNumber">Number of the target Ring (the Ring where is the Cell to which the Element has to be moved)</param>
        ''' <param name="pToCellNumber">Number of the target Cell (the Cell to which the Element has to be moved)</param>
        ''' <param name="pToBarCodeStatus">Current Barcode Status for the target Cell (the Cell to which the Element has to be moved)</param>
        ''' <param name="pToBarCodeInfo">Current Barcode Information for the target Cell (the Cell to which the Element has to be moved)</param>
        ''' <returns>GlobalDataTO containing a typed DataSet WSRotorContentByPositionDS with the information about the Rotor Cell in 
        '''          which the Additional Solution was placed plus the recalculated Element Status</returns>
        ''' <remarks>
        ''' Created by:  BK 08/12/2009
        ''' Modified by: BK 21/12/2009
        '''              AG 21/12/2009 - PENDING VR  CORRECTIONS OF BK CODE - Tested: PENDING
        '''              VR 23/12/2009   (Testing : Partial OK) - REAGENT FLOW IS PENDING
        '''              TR 04/01/2010 - Added the update dataset to the result GlobalDataTO with the new
        '''                              position assigned to the element.
        '''              SA 13/01/2010 - Changed the way of opening the DB Transaction to fulfill the new template
        '''              SA 15/01/2010 - Changes needed to allow the dragged of a "death" volume Bottle from the internal Ring
        '''                              to the external Ring in a Reagents Rotor
        '''              AG 21/01/2010 - Changes needed for the no_inuse positions (Tested ok)
        '''              AG 06/10/2011 - Added new parameters pToBarCodeStatus + pToBarCodeInfo
        '''              SA 15/02/2012 - Management of "death volume" bottle is removed; changes when a bottle in Reagents Rotor is moved from the internal
        '''                              ring to the external one, bottle is refilled only when the bottle size changes
        '''              SA 02/03/2012 - Changed the calling to function GetReagentBottles due it was modified by removing the parameter for the residual volume
        '''              XB 07/10/2014 - BA-1978 ==> Add log traces to catch NULL wrong assignment on RealVolume field
        '''              AG 07/10/2014 - BA-1979 ==> Replaced call to function twksWSRotorContentByPositionDAO.Update by a call to function Update 
        '''                                          in this Delegate, informing the process who is updating the content of the Rotor Position (to 
        '''                                          search which process is inserting invalid values: positions with TubeContent but not element ID)
        '''              SA 22/12/2014 - BA-1999 ==> Before update field in the DS of Not In Use Rotor Positions, verify the DS has a row to avoid errors when
        '''                                          a position with BarcodeStatus = UNKNOWN is moved to another position in Reagents Rotor (in this case, the 
        '''                                          position Status is NO_INUSE but there is not a row in table twksWSNotInUseRotorPositions)
        '''              SA 08/01/2015 - BA-1999 ==> When it is verified is the Element to move is NOT IN USE in the active Work Session, if Position Status is 
        '''                                          different of NO_INUSE, but field ElementID is not informed, it means the Position is NOT IN USE, but it contains
        '''                                          a bottle that is DEPLETED, FEW or LOCKED; then, both conditions have to be checked
        ''' </remarks>
        Public Function ChangeElementPosition(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWSRotorContentByPositionDS As WSRotorContentByPositionDS, _
                                              ByVal pToRingNumber As Integer, ByVal pToCellNumber As Integer, ByVal pToBarCodeStatus As String, _
                                              ByVal pToBarCodeInfo As String) As GlobalDataTO
            Dim dataToReturn As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing
            Try
                dataToReturn = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not dataToReturn.HasError AndAlso Not dataToReturn.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(dataToReturn.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myNotInUsePositionDS As New VirtualRotorPosititionsDS
                        Dim myNotInUsePositionsDelegate As New WSNotInUseRotorPositionsDelegate

                        'Save the Ring in which the Element was placed in a local variable
                        Dim previousRing As Integer = pWSRotorContentByPositionDS.twksWSRotorContentByPosition(0).RingNumber

                        'Prepare the DataSet containing the information required to liberate the current Rotor Position
                        Dim tmpWSRotorContentByPositionDS As New WSRotorContentByPositionDS
                        Dim tmpWSRotorContentByPositionRow As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow

                        tmpWSRotorContentByPositionRow = tmpWSRotorContentByPositionDS.twksWSRotorContentByPosition.NewtwksWSRotorContentByPositionRow()
                        tmpWSRotorContentByPositionRow.AnalyzerID = pWSRotorContentByPositionDS.twksWSRotorContentByPosition(0).AnalyzerID
                        tmpWSRotorContentByPositionRow.WorkSessionID = pWSRotorContentByPositionDS.twksWSRotorContentByPosition(0).WorkSessionID
                        tmpWSRotorContentByPositionRow.RotorType = pWSRotorContentByPositionDS.twksWSRotorContentByPosition(0).RotorType
                        tmpWSRotorContentByPositionRow.RingNumber = pWSRotorContentByPositionDS.twksWSRotorContentByPosition(0).RingNumber
                        tmpWSRotorContentByPositionRow.CellNumber = pWSRotorContentByPositionDS.twksWSRotorContentByPosition(0).CellNumber
                        tmpWSRotorContentByPositionRow.Status = "FREE"
                        tmpWSRotorContentByPositionRow.TS_User = pWSRotorContentByPositionDS.twksWSRotorContentByPosition(0).TS_User
                        tmpWSRotorContentByPositionRow.SetScannedPositionNull()
                        tmpWSRotorContentByPositionRow.SetBarCodeInfoNull()
                        tmpWSRotorContentByPositionRow.SetBarcodeStatusNull()
                        tmpWSRotorContentByPositionDS.twksWSRotorContentByPosition.Rows.Add(tmpWSRotorContentByPositionRow)

                        'Update the Rotor Position
                        'Dim rotorContentPosition As New twksWSRotorContentByPositionDAO
                        'dataToReturn = rotorContentPosition.Update(dbConnection, tmpWSRotorContentByPositionDS)
                        dataToReturn = Update(dbConnection, tmpWSRotorContentByPositionRow.RotorType, tmpWSRotorContentByPositionDS, ClassCalledFrom.ChangeElementPosition1) 'AG 07/10/2014 BA-1979 call the Update method  in delegate instead of in DAO

                        If (Not dataToReturn.HasError) Then
                            'If pWSRotorContentByPositionDS has not informed the BarCode but the destination yes .. do not delete destination BarCode
                            If (pWSRotorContentByPositionDS.twksWSRotorContentByPosition(0).IsBarCodeInfoNull AndAlso pToBarCodeInfo <> "") Then
                                pWSRotorContentByPositionDS.twksWSRotorContentByPosition(0).BeginEdit()
                                pWSRotorContentByPositionDS.twksWSRotorContentByPosition(0).BarCodeInfo = pToBarCodeInfo
                                pWSRotorContentByPositionDS.twksWSRotorContentByPosition(0).EndEdit()
                                pWSRotorContentByPositionDS.twksWSRotorContentByPosition.AcceptChanges()
                            End If

                            'The moved TUBE/BOTTLE is NOT IN USE in the current WorkSession
                            'BA-1999: Positions with Status DEPLETED, FEW or LOCKED but without ElementID are also NOT IN USE
                            If (pWSRotorContentByPositionDS.twksWSRotorContentByPosition(0).Status = "NO_INUSE" OrElse _
                                pWSRotorContentByPositionDS.twksWSRotorContentByPosition(0).IsElementIDNull()) Then
                                'Get the information of the NOT IN USE Position to move
                                dataToReturn = myNotInUsePositionsDelegate.GetPositionContent(dbConnection, pWSRotorContentByPositionDS.twksWSRotorContentByPosition(0).AnalyzerID, _
                                                                                              pWSRotorContentByPositionDS.twksWSRotorContentByPosition(0).RotorType, _
                                                                                              pWSRotorContentByPositionDS.twksWSRotorContentByPosition(0).RingNumber, _
                                                                                              pWSRotorContentByPositionDS.twksWSRotorContentByPosition(0).CellNumber, _
                                                                                              pWSRotorContentByPositionDS.twksWSRotorContentByPosition(0).WorkSessionID)

                                If (Not dataToReturn.HasError AndAlso Not dataToReturn.SetDatos Is Nothing) Then
                                    myNotInUsePositionDS = DirectCast(dataToReturn.SetDatos, VirtualRotorPosititionsDS)

                                    'BA-1999: Before change the Ring and Cell Number, verify if the DS has a row (due to positions with BarcodeStatus = ERROR 
                                    '         have Status NOT IN USE but do not exist in table twksWSNotInUseRotorPositions and then an error is raised when try
                                    '         to move them to another position) 
                                    If (myNotInUsePositionDS.tparVirtualRotorPosititions.Rows.Count > 0) Then
                                        'Update the Ring/Cell fields with values of the target position
                                        myNotInUsePositionDS.tparVirtualRotorPosititions.First.RingNumber = pToRingNumber
                                        myNotInUsePositionDS.tparVirtualRotorPosititions.First.CellNumber = pToCellNumber

                                        'Delete the position from table of NOT IN USE Rotor Positions for the active WorkSession
                                        dataToReturn = myNotInUsePositionsDelegate.Delete(dbConnection, pWSRotorContentByPositionDS)
                                    End If
                                End If
                            End If
                        End If

                        If (Not dataToReturn.HasError) Then
                            'Change the Rotor Position to the new one in the entry DataSet
                            pWSRotorContentByPositionDS.twksWSRotorContentByPosition(0).RingNumber = pToRingNumber
                            pWSRotorContentByPositionDS.twksWSRotorContentByPosition(0).CellNumber = pToCellNumber

                            Select Case pWSRotorContentByPositionDS.twksWSRotorContentByPosition(0).RotorType
                                Case "SAMPLES"
                                    'Positioning is in a Samples Rotor, update the Rotor Position informing the Element positioned in it
                                    'dataToReturn = rotorContentPosition.Update(dbConnection, pWSRotorContentByPositionDS)
                                    dataToReturn = Update(dbConnection, pWSRotorContentByPositionDS.twksWSRotorContentByPosition(0).RotorType, pWSRotorContentByPositionDS, ClassCalledFrom.ChangeElementPosition2) 'AG 07/10/2014 BA-1979 call the Update method  in delegate instead of in DAO
                                    Exit Select

                                Case "REAGENTS"
                                    If (pToRingNumber >= previousRing) Then
                                        'Update the Rotor Cell / Position informing the Element positioned in it
                                        'dataToReturn = rotorContentPosition.Update(dbConnection, pWSRotorContentByPositionDS)
                                        dataToReturn = Update(dbConnection, pWSRotorContentByPositionDS.twksWSRotorContentByPosition(0).RotorType, pWSRotorContentByPositionDS, ClassCalledFrom.ChangeElementPosition3) 'AG 07/10/2014 BA-1979 call the Update method  in delegate instead of in DAO
                                    Else
                                        'If the bottle has been moved from the internal ring to the external one, it is possible a size change is required (external ring
                                        'allow only small bottles) 
                                        Dim myReagentTubeTypesDelegate As New ReagentTubeTypesDelegate

                                        'Search all available Bottles sorted ascending according the Bottle volume
                                        dataToReturn = myReagentTubeTypesDelegate.GetReagentBottles(dbConnection, True)
                                        If (Not dataToReturn.HasError And Not dataToReturn.SetDatos Is Nothing) Then
                                            Dim myReagentTubeTypesDS As ReagentTubeTypesDS = DirectCast(dataToReturn.SetDatos, ReagentTubeTypesDS)

                                            If (pWSRotorContentByPositionDS.twksWSRotorContentByPosition(0).TubeType <> myReagentTubeTypesDS.ReagentTubeTypes(0).TubeCode) Then
                                                'The moved bottle is a big one, update Bottle Code and Real Volume for the target position 
                                                pWSRotorContentByPositionDS.twksWSRotorContentByPosition(0).TubeType = myReagentTubeTypesDS.ReagentTubeTypes(0).TubeCode
                                                pWSRotorContentByPositionDS.twksWSRotorContentByPosition(0).RealVolume = myReagentTubeTypesDS.ReagentTubeTypes(0).TubeVolume

                                                If (pWSRotorContentByPositionDS.twksWSRotorContentByPosition(0).Status <> "NO_INUSE") Then
                                                    'Update also the position Status to IN_USE, due to the new bottle is full
                                                    pWSRotorContentByPositionDS.twksWSRotorContentByPosition(0).Status = "INUSE"

                                                    If (pWSRotorContentByPositionDS.twksWSRotorContentByPosition(0).TubeContent = "REAGENT") Then
                                                        'Calculate the number of remaining Tests that can be executed with the volume of the new positioned Bottle, and also if the 
                                                        'total volume of all positioned Reagent Bottles is enough for the Work Session. Update the Rotor Position and also the Element Status
                                                        Dim myWSRotorContentByPosDelegate As New WSRotorContentByPositionDelegate
                                                        dataToReturn = myWSRotorContentByPosDelegate.UpdateReagentPosition(dbConnection, pWSRotorContentByPositionDS)
                                                    Else
                                                        'The Additional Solution is positioned; change the Element Status 
                                                        pWSRotorContentByPositionDS.twksWSRotorContentByPosition(0).ElementStatus = "POS"

                                                        Dim myWSReqElementDelegate As New WSRequiredElementsDelegate
                                                        dataToReturn = myWSReqElementDelegate.UpdateStatusAndTubeType(dbConnection, pWSRotorContentByPositionDS.twksWSRotorContentByPosition(0).WorkSessionID, _
                                                                                                                      pWSRotorContentByPositionDS.twksWSRotorContentByPosition(0).ElementID, _
                                                                                                                      pWSRotorContentByPositionDS.twksWSRotorContentByPosition(0).ElementStatus, _
                                                                                                                      pWSRotorContentByPositionDS.twksWSRotorContentByPosition(0).TubeType, _
                                                                                                                      pWSRotorContentByPositionDS.twksWSRotorContentByPosition(0).TubeContent, False)

                                                        If (Not dataToReturn.HasError) Then
                                                            'Update the Rotor Cell / Position 
                                                            'dataToReturn = rotorContentPosition.Update(dbConnection, pWSRotorContentByPositionDS)
                                                            dataToReturn = Update(dbConnection, pWSRotorContentByPositionDS.twksWSRotorContentByPosition(0).RotorType, pWSRotorContentByPositionDS, ClassCalledFrom.ChangeElementPosition4) 'AG 07/10/2014 BA-1979 call the Update method  in delegate instead of in DAO
                                                        End If
                                                    End If
                                                Else
                                                    'BA-1999: Before change the Status and TubeType, verify if the DS has a row (due to positions with BarcodeStatus = ERROR 
                                                    '         have Status NOT IN USE but do not exist in table twksWSNotInUseRotorPositions and then an error is raised when try
                                                    '         to move them to another position) 
                                                    If (myNotInUsePositionDS.tparVirtualRotorPosititions.Rows.Count > 0) Then
                                                        'Set to NULL the position Status, and update also the TubeType for the new one
                                                        myNotInUsePositionDS.tparVirtualRotorPosititions.First.SetStatusNull()
                                                        myNotInUsePositionDS.tparVirtualRotorPosititions.First.TubeType = pWSRotorContentByPositionDS.twksWSRotorContentByPosition(0).TubeType

                                                        'Update the Rotor Cell / Position 
                                                        'dataToReturn = rotorContentPosition.Update(dbConnection, pWSRotorContentByPositionDS)
                                                        dataToReturn = Update(dbConnection, pWSRotorContentByPositionDS.twksWSRotorContentByPosition(0).RotorType, pWSRotorContentByPositionDS, ClassCalledFrom.ChangeElementPosition5) 'AG 07/10/2014 BA-1979 call the Update method  in delegate instead of in DAO
                                                    End If
                                                End If
                                            Else
                                                'The bottle size is the same, update the Rotor Cell / Position informing the Element positioned in it
                                                'dataToReturn = rotorContentPosition.Update(dbConnection, pWSRotorContentByPositionDS)
                                                dataToReturn = Update(dbConnection, pWSRotorContentByPositionDS.twksWSRotorContentByPosition(0).RotorType, pWSRotorContentByPositionDS, ClassCalledFrom.ChangeElementPosition6) 'AG 07/10/2014 BA-1979 call the Update method  in delegate instead of in DAO
                                            End If
                                        End If
                                    End If
                                    Exit Select
                            End Select
                        End If

                        If (Not dataToReturn.HasError) Then
                            'BA-1999: Positions with Status DEPLETED, FEW or LOCKED but without ElementID are also NOT IN USE
                            If (pWSRotorContentByPositionDS.twksWSRotorContentByPosition(0).Status = "NO_INUSE" OrElse _
                                pWSRotorContentByPositionDS.twksWSRotorContentByPosition(0).IsElementIDNull()) Then
                                If (pToBarCodeStatus = "UNKNOWN") Then
                                    'When the destination position has BarcodeStatus = UNKNOWN, the target position already exists in the table of Not In Use Elements
                                    'and it should be updated; however, due to update function does not exist, the target position has to be deleted before continuing
                                    dataToReturn = myNotInUsePositionsDelegate.Delete(dbConnection, pWSRotorContentByPositionDS)
                                End If

                                If (Not dataToReturn.HasError) Then
                                    'BA-1999: Before call the Add function, verify if the DS has a row (due to positions with BarcodeStatus = ERROR 
                                    '         have Status NOT IN USE but do not exist in table twksWSNotInUseRotorPositions) 
                                    If (myNotInUsePositionDS.tparVirtualRotorPosititions.Rows.Count > 0) Then
                                        'BA-1979: Added classCalledFrom parameter
                                        'Add the new NOT IN USE Position in with the same information of the previous one
                                        dataToReturn = myNotInUsePositionsDelegate.Add(dbConnection, pWSRotorContentByPositionDS.twksWSRotorContentByPosition(0).AnalyzerID, _
                                                                                       pWSRotorContentByPositionDS.twksWSRotorContentByPosition(0).RotorType, _
                                                                                       pWSRotorContentByPositionDS.twksWSRotorContentByPosition(0).WorkSessionID, _
                                                                                       myNotInUsePositionDS, WSNotInUseRotorPositionsDelegate.ClassCalledFrom.ChangeElementPosition)
                                    End If
                                End If
                            End If
                        End If

                        If (Not dataToReturn.HasError) Then
                            'When the Database Connection was opened locally, then the Commit is executed
                            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)

                            'Import the cleaned to position (the source one) to the DS to return
                            pWSRotorContentByPositionDS.twksWSRotorContentByPosition.ImportRow(tmpWSRotorContentByPositionDS.twksWSRotorContentByPosition(0))

                            'Finally, return the two updated Rotor Positions, the source one and the target one
                            dataToReturn.SetDatos = pWSRotorContentByPositionDS
                        Else
                            'When the Database Connection was opened locally, then the Rollback is executed
                            If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                        End If
                    End If
                End If
            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                dataToReturn = New GlobalDataTO()
                dataToReturn.HasError = True
                dataToReturn.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                dataToReturn.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSRotorContentByPositionDelegate.ChangeElementPosition", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return dataToReturn
        End Function

        ''' <summary>
        ''' Re-fill a group of Rotor Positions or change the size of the tube/bottle placed in an specific Rotor Position
        ''' </summary>
        ''' <param name="pDbConnection">Open DB connection</param>
        ''' <param name="pSelectedPositionsDS">Group of Rotor Positions selected to be re-filled or the Rotor Position selected to change 
        '''                                    the tube/bottle size</param>
        ''' <returns>GlobalDataTO containing a typed DataSet WSRotorContentByPositionDS with the same entry information 
        '''          but with some fields updated</returns>
        ''' <remarks>
        ''' Created by:  VR 01/12/2009 - Tested: OK 08/12/2009
        ''' Modified by: VR 24/12/2009 - Tested: OK
        '''              AG 05/01/2010 - Inside the FOR loop for each row we can not use pSelectedPositionsDS as parameter 
        '''                              calling other functions. Before this changes the function fails when pSelectedPositionsDS has 
        '''                              more than 1 row. Not all selected positions can change volume (for instance the FREE positions)
        '''              SA 07/01/2010 - Name of called function GetVolumeByTubeSize was changed to GetVolumeByTubeType
        '''              SA 13/01/2010 - Changed the way of opening the DB Transaction to fulfill the new template
        '''              AG 22/01/2010 - Changes for NO_INUSE positions (Tested PENDING)
        '''              AG 28/01/2010 - Return value not null for real volume in special solutions and reagents no inuse
        '''              SA 19/03/2010 - For Calibrators, Controls and Patient Samples, besides the Element Status, it is needed to update 
        '''                              the TubeType in twksWSRequiredElements and in twksOrderTests for those Order Tests using the Element
        '''                              (change was implemented by calling function UpdateStatusAndTubeType instead of UpdateStatus)
        '''              RH 17/06/2011 - Add TUBE_SPEC_SOL and TUBE_WASH_SOL. Code optimization.
        '''              SA 20/06/2011 - Added parameter WorkSessionID when call function UpdateStatusAndTubeType in WSRequiredElementDelegate
        '''              SA 13/02/2012 - Added optional parameter pOnlyRefill to indicate the selected action was Refill (TRUE) instead of Tube/Bottle Type
        '''                              change (FALSE); the difference is that for multipoint Calibrators, a tube change means the change applies to all tubes
        '''                              in the kit, which are also refilled and then the status of all of them changes to PENDING. However, when the selected
        '''                              action is only Refill, then only the status of the specific Calibrator tube changes to PENDING, the rest remains with
        '''                              the previous state. Implementation changed.
        '''              SA 06/03/2012 - Change "IN_USE" for "INUSE", which is the correct code in PreloadedMasterData
        '''              JV 04/12/2013 - #1384 new optional parameter to assure we have not active session and the user wants to fill the cell.
        '''              XB 07/10/2014 - BA-1978 ==> Add log traces to catch NULL wrong assignment on RealVolume field
        '''              AG 07/10/2014 - BA-1979 ==> Replaced call to function twksWSRotorContentByPositionDAO.Update by a call to function Update 
        '''                                          in this Delegate, informing the process who is updating the content of the Rotor Position (to 
        '''                                          search which process is inserting invalid values: positions with TubeContent but not element ID)
        ''' </remarks>
        Public Function ChangeVolumeByPosition(ByVal pDbConnection As SqlClient.SqlConnection, ByVal pSelectedPositionsDS As WSRotorContentByPositionDS, _
                                               Optional ByVal pOnlyRefill As Boolean = False, Optional ByVal pEmptyWS As Boolean = False) As GlobalDataTO
            Dim returnedData As GlobalDataTO
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                returnedData = DAOBase.GetOpenDBTransaction(pDbConnection)
                If (Not returnedData.HasError AndAlso Not returnedData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(returnedData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim calibratorID As Integer
                        Dim myWSRequiredElemDS As New WSRequiredElementsDS
                        Dim myWSRequiredElemDelegate As New WSRequiredElementsDelegate
                        Dim myWSNotInUseElemDS As New VirtualRotorPosititionsDS
                        Dim myWSNotInUseElemDelegate As New WSNotInUseRotorPositionsDelegate

                        Dim myReagentTubeTypesDS As New ReagentTubeTypesDS
                        Dim myReagentTubeTypesDelegate As New ReagentTubeTypesDelegate

                        Dim myAddPositionsToReturnDS As New WSRotorContentByPositionDS
                        Dim myNewSelectedPositionsDS As New WSRotorContentByPositionDS
                        Dim myRotorContentByPosDAO As New twksWSRotorContentByPositionDAO

                        For Each rotorPosition As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow In pSelectedPositionsDS.twksWSRotorContentByPosition
                            If (rotorPosition.Status <> "FREE") Then
                                'Only for Bottles positioned in Reagents Rotor (Reagents and/or Additional Solutions), get max volume of the positioned bottle,
                                'and set it as RealVolume of the bottle
                                If (String.Compare(rotorPosition.RotorType, "REAGENTS", False) = 0) Then
                                    returnedData = myReagentTubeTypesDelegate.GetVolumeByTubeType(dbConnection, rotorPosition.TubeType)
                                    If (Not returnedData.HasError AndAlso Not returnedData.SetDatos Is Nothing) Then
                                        myReagentTubeTypesDS = DirectCast(returnedData.SetDatos, ReagentTubeTypesDS)
                                        If (myReagentTubeTypesDS.ReagentTubeTypes.Rows.Count > 0) Then rotorPosition.RealVolume = myReagentTubeTypesDS.ReagentTubeTypes(0).TubeVolume
                                    Else
                                        Exit For
                                    End If
                                End If

                                'PROCESS FOR IN USE ROTOR POSITIONS
                                If (rotorPosition.Status <> "NO_INUSE" And Not pEmptyWS) Then 'If (rotorPosition.Status <> "NO_INUSE") Then
                                    Select Case (rotorPosition.TubeContent)
                                        Case "CTRL", "PATIENT", "TUBE_SPEC_SOL", "TUBE_WASH_SOL"
                                            'Update field Status to PENDING and move data to an auxiliary DataSet
                                            rotorPosition.Status = "PENDING"
                                            myNewSelectedPositionsDS.Clear()
                                            myNewSelectedPositionsDS.twksWSRotorContentByPosition.ImportRow(rotorPosition)

                                            'Update TubeType and Status = PENDING for the selected Rotor Position
                                            'returnedData = myRotorContentByPosDAO.Update(dbConnection, myNewSelectedPositionsDS)
                                            returnedData = Update(dbConnection, rotorPosition.RotorType, myNewSelectedPositionsDS, ClassCalledFrom.ChangeVolumeByPosition1) 'AG 07/10/2014 BA-1979 call the Update method  in delegate instead of in DAO
                                            If (returnedData.HasError) Then Exit For

                                            'Update TubeType and ElementStatus = POS for the correspondent required Element and OrderTest
                                            rotorPosition.ElementStatus = "POS"
                                            returnedData = myWSRequiredElemDelegate.UpdateStatusAndTubeType(dbConnection, rotorPosition.WorkSessionID, rotorPosition.ElementID, _
                                                                                                            rotorPosition.ElementStatus, rotorPosition.TubeType, _
                                                                                                            rotorPosition.TubeContent, False)
                                            If (returnedData.HasError) Then Exit For

                                        Case "CALIB"
                                            If (pOnlyRefill) Then
                                                'Move data to an auxiliary DataSet
                                                rotorPosition.Status = "PENDING"
                                                myNewSelectedPositionsDS.Clear()
                                                myNewSelectedPositionsDS.twksWSRotorContentByPosition.ImportRow(rotorPosition)

                                                'Update Status = PENDING for the selected Rotor Position
                                                'returnedData = myRotorContentByPosDAO.Update(dbConnection, myNewSelectedPositionsDS)
                                                returnedData = Update(dbConnection, rotorPosition.RotorType, myNewSelectedPositionsDS, ClassCalledFrom.ChangeVolumeByPosition2) 'AG 07/10/2014 BA-1979 call the Update method  in delegate instead of in DAO
                                                If (returnedData.HasError) Then Exit For
                                            Else
                                                'Search the Calibrator Identifier in table of WS Required Elements (by ElementID)
                                                calibratorID = 0
                                                returnedData = myWSRequiredElemDelegate.GetRequiredElementData(dbConnection, rotorPosition.ElementID)
                                                If (Not returnedData.HasError AndAlso Not returnedData.SetDatos Is Nothing) Then
                                                    myWSRequiredElemDS = DirectCast(returnedData.SetDatos, WSRequiredElementsDS)
                                                    If (myWSRequiredElemDS.twksWSRequiredElements.Rows.Count > 0) Then
                                                        calibratorID = myWSRequiredElemDS.twksWSRequiredElements(0).CalibratorID
                                                    End If
                                                Else
                                                    'Error getting data of the specified REQUIRED Element 
                                                    Exit For
                                                End If

                                                'Update Status=PENDING for all positions of tubes of the Calibrator kit
                                                rotorPosition.Status = "PENDING"
                                                returnedData = myRotorContentByPosDAO.UpdateCalibStatusAndTubeType(dbConnection, rotorPosition.AnalyzerID, rotorPosition.WorkSessionID, _
                                                                                                                   calibratorID, rotorPosition.TubeType, "PENDING")
                                                If (returnedData.HasError) Then Exit For
                                            End If

                                            'Update TubeType and ElementStatus = POS. If pOnlyRefill=FALSE, the update is executed for all Elements in the Calibrator kit
                                            rotorPosition.ElementStatus = "POS"
                                            returnedData = myWSRequiredElemDelegate.UpdateStatusAndTubeType(dbConnection, rotorPosition.WorkSessionID, rotorPosition.ElementID, _
                                                                                                            rotorPosition.ElementStatus, rotorPosition.TubeType, _
                                                                                                            rotorPosition.TubeContent, Not pOnlyRefill)
                                            If (returnedData.HasError) Then Exit For

                                            If (Not pOnlyRefill) Then
                                                'Get the rest of Rotor Positions for the other tubes in the Calibrator kit...
                                                returnedData = myRotorContentByPosDAO.GetMultiPointCalibratorPositions(dbConnection, rotorPosition.WorkSessionID, calibratorID, _
                                                                                                                       rotorPosition.ElementID)
                                                If (Not returnedData.HasError AndAlso Not returnedData.SetDatos Is Nothing) Then
                                                    myNewSelectedPositionsDS = DirectCast(returnedData.SetDatos, WSRotorContentByPositionDS)

                                                    'Move them to a temporary DataSet that will be added to the final DS to return
                                                    For Each myAddTube As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow In myNewSelectedPositionsDS.twksWSRotorContentByPosition
                                                        myAddTube.ElementStatus = "POS"
                                                        myAddPositionsToReturnDS.twksWSRotorContentByPosition.ImportRow(myAddTube)
                                                    Next
                                                Else
                                                    'Error getting data of the rest of Rotor Positions for the other tubes in the Calibrator kit
                                                    Exit For
                                                End If
                                            End If

                                        Case "REAGENT"
                                            'Update field Status to IN USE and move data to an auxiliary DataSet
                                            rotorPosition.Status = "INUSE"
                                            myNewSelectedPositionsDS.Clear()
                                            myNewSelectedPositionsDS.twksWSRotorContentByPosition.ImportRow(rotorPosition)

                                            'Update Status=INUSE, and calculate the number of Remaining Tests and the Element Status for the selected Rotor Position
                                            returnedData = UpdateReagentPosition(dbConnection, myNewSelectedPositionsDS)
                                            If (Not returnedData.HasError AndAlso Not returnedData.SetDatos Is Nothing) Then
                                                myNewSelectedPositionsDS = DirectCast(returnedData.SetDatos, WSRotorContentByPositionDS)

                                                If (myNewSelectedPositionsDS.twksWSRotorContentByPosition.Rows.Count > 0) Then
                                                    rotorPosition.BeginEdit()
                                                    rotorPosition.Status = myNewSelectedPositionsDS.twksWSRotorContentByPosition(0).Status
                                                    rotorPosition.ElementStatus = myNewSelectedPositionsDS.twksWSRotorContentByPosition(0).ElementStatus
                                                    rotorPosition.RealVolume = myNewSelectedPositionsDS.twksWSRotorContentByPosition(0).RealVolume
                                                    rotorPosition.RemainingTestsNumber = myNewSelectedPositionsDS.twksWSRotorContentByPosition(0).RemainingTestsNumber
                                                    rotorPosition.EndEdit()
                                                End If
                                            Else
                                                Exit For
                                            End If

                                        Case "SPEC_SOL", "WASH_SOL"
                                            'Update field Status to IN USE and move data to an auxiliary DataSet
                                            rotorPosition.Status = "INUSE"
                                            myNewSelectedPositionsDS.Clear()
                                            myNewSelectedPositionsDS.twksWSRotorContentByPosition.ImportRow(rotorPosition)

                                            'Update BottleType and Status = INUSE for the selected Rotor Position
                                            'returnedData = myRotorContentByPosDAO.Update(dbConnection, myNewSelectedPositionsDS)
                                            returnedData = Update(dbConnection, rotorPosition.RotorType, myNewSelectedPositionsDS, ClassCalledFrom.ChangeVolumeByPosition3) 'AG 07/10/2014 BA-1979 call the Update method  in delegate instead of in DAO
                                            If (returnedData.HasError) Then Exit For

                                            'Update BottleType and ElementStatus = POS for the correspondent required Element and OrderTest
                                            rotorPosition.ElementStatus = "POS"
                                            returnedData = myWSRequiredElemDelegate.UpdateStatusAndTubeType(dbConnection, rotorPosition.WorkSessionID, rotorPosition.ElementID, _
                                                                                                            rotorPosition.ElementStatus, rotorPosition.TubeType, _
                                                                                                            rotorPosition.TubeContent, False)
                                            If (returnedData.HasError) Then Exit For
                                    End Select
                                ElseIf Not pEmptyWS Then 'Else
                                    'PROCESS FOR NOT IN USE ROTOR POSITIONS
                                    Select Case (rotorPosition.TubeContent)
                                        Case "CTRL", "PATIENT", "TUBE_SPEC_SOL", "TUBE_WASH_SOL", "REAGENT", "SPEC_SOL", "WASH_SOL"
                                            If (Not pOnlyRefill) Then
                                                'Move data to an auxiliary DataSet; position Status does not change
                                                myNewSelectedPositionsDS.Clear()
                                                myNewSelectedPositionsDS.twksWSRotorContentByPosition.ImportRow(rotorPosition)

                                                'Update TubeType/BottleType for the selected Rotor Position
                                                'returnedData = myRotorContentByPosDAO.Update(dbConnection, myNewSelectedPositionsDS)
                                                returnedData = Update(dbConnection, rotorPosition.RotorType, myNewSelectedPositionsDS, ClassCalledFrom.ChangeVolumeByPosition4) 'AG 07/10/2014 BA-1979 call the Update method  in delegate instead of in DAO
                                                If (returnedData.HasError) Then Exit For
                                            End If

                                            'Set to NULL field Status in table of Not InUse Rotor Positions
                                            returnedData = myWSNotInUseElemDelegate.SetStatusToNull(dbConnection, rotorPosition.AnalyzerID, rotorPosition.WorkSessionID, _
                                                                                                    rotorPosition.RotorType, rotorPosition.CellNumber)
                                            If (returnedData.HasError) Then Exit For

                                        Case "CALIB"
                                            If (pOnlyRefill) Then
                                                'Set to NULL field Status in table of Not InUse Rotor Positions
                                                returnedData = myWSNotInUseElemDelegate.SetStatusToNull(dbConnection, rotorPosition.AnalyzerID, rotorPosition.WorkSessionID, _
                                                                                                        rotorPosition.RotorType, rotorPosition.CellNumber)
                                                If (returnedData.HasError) Then Exit For
                                            Else
                                                'Search the Calibrator Identifier by getting content of the selected Not InUse Rotor Position
                                                calibratorID = 0
                                                returnedData = myWSNotInUseElemDelegate.GetPositionContent(dbConnection, rotorPosition.AnalyzerID, rotorPosition.RotorType, _
                                                                                                           rotorPosition.RingNumber, rotorPosition.CellNumber, rotorPosition.WorkSessionID)
                                                If (Not returnedData.HasError AndAlso Not returnedData.SetDatos Is Nothing) Then
                                                    myWSNotInUseElemDS = DirectCast(returnedData.SetDatos, VirtualRotorPosititionsDS)
                                                    If (myWSNotInUseElemDS.tparVirtualRotorPosititions.Rows.Count > 0) Then
                                                        calibratorID = myWSNotInUseElemDS.tparVirtualRotorPosititions(0).CalibratorID
                                                    End If
                                                Else
                                                    'Error getting data of the Not InUse Rotor Position
                                                    Exit For
                                                End If

                                                'Update TubeType for all positions in the Calibrator kit
                                                returnedData = myRotorContentByPosDAO.UpdateCalibStatusAndTubeType(dbConnection, rotorPosition.AnalyzerID, rotorPosition.WorkSessionID, _
                                                                                                                   calibratorID, rotorPosition.TubeType)
                                                If (returnedData.HasError) Then Exit For

                                                'Set to NULL field Status in table of Not InUse Rotor Positions for all cells containing the Calibrator
                                                returnedData = myWSNotInUseElemDelegate.SetStatusToNull(dbConnection, rotorPosition.AnalyzerID, rotorPosition.WorkSessionID, _
                                                                                                        rotorPosition.RotorType, -1, calibratorID)
                                                If (returnedData.HasError) Then Exit For
                                            End If
                                    End Select
                                Else 'JV 04/12/2013 #1384 new else to treat the fact of having session "EMPTY", and refill REAGENTS rotor positions
                                    Select Case (rotorPosition.TubeContent)
                                        Case "REAGENT", "SPEC_SOL", "WASH_SOL"
                                            If (pOnlyRefill) Then
                                                rotorPosition.Status = "NO_INUSE"
                                                'Set to NULL field Status in table of Not InUse Rotor Positions
                                                returnedData = myWSNotInUseElemDelegate.SetStatusToNull(dbConnection, rotorPosition.AnalyzerID, rotorPosition.WorkSessionID, _
                                                                                                        rotorPosition.RotorType, rotorPosition.CellNumber)
                                                If (returnedData.HasError) Then Exit For
                                                'Move data to an auxiliary DataSet; position Status does not change
                                                myNewSelectedPositionsDS.Clear()
                                                myNewSelectedPositionsDS.twksWSRotorContentByPosition.ImportRow(rotorPosition)

                                                'Update TubeType/BottleType for the selected Rotor Position
                                                'returnedData = myRotorContentByPosDAO.Update(dbConnection, myNewSelectedPositionsDS)
                                                returnedData = Update(dbConnection, rotorPosition.RotorType, myNewSelectedPositionsDS, ClassCalledFrom.ChangeVolumeByPosition5) 'AG 07/10/2014 BA-1979 call the Update method  in delegate instead of in DAO
                                                If (returnedData.HasError) Then Exit For
                                            End If
                                    End Select
                                End If
                            End If
                        Next

                        If (Not returnedData.HasError) Then
                            'If Multipoint Calibrators were processed, add all updated positions that were not in the entry DS of selected cells
                            Dim searchPos As List(Of WSRotorContentByPositionDS.twksWSRotorContentByPositionRow)
                            For Each myAddTube As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow In myAddPositionsToReturnDS.twksWSRotorContentByPosition
                                'Verify the cell is not included in the list of additional positions to add
                                searchPos = (From a As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow In pSelectedPositionsDS.twksWSRotorContentByPosition _
                                            Where a.AnalyzerID = myAddTube.AnalyzerID _
                                          AndAlso String.Compare(a.WorkSessionID, myAddTube.WorkSessionID, False) = 0 _
                                          AndAlso a.RotorType = myAddTube.RotorType _
                                          AndAlso a.RingNumber = myAddTube.RingNumber _
                                          AndAlso a.CellNumber = myAddTube.CellNumber _
                                           Select a).ToList

                                If (searchPos.Count = 0) Then pSelectedPositionsDS.twksWSRotorContentByPosition.ImportRow(myAddTube)
                            Next
                        End If

                        If (Not returnedData.HasError) Then
                            If (pDbConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                            returnedData.SetDatos = pSelectedPositionsDS
                        Else
                            If (pDbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                            returnedData.HasError = True
                        End If
                    End If
                End If

            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDbConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                returnedData = New GlobalDataTO()
                returnedData.HasError = True
                returnedData.ErrorMessage = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                returnedData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSRotorContentByPositionDelegate.ChangeVolumeByPosition", EventLogEntryType.Error, False)
            Finally
                If (pDbConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return returnedData
        End Function

        ''' <summary>
        ''' Return the complete group of positions to clear (or delete) in an Analyzer Rotor. When a position corresponding 
        ''' to a MultiPoint Calibrator is selected to be cleared, all tubes in the Calibrator kit have to be downloaded
        ''' </summary>
        ''' <param name="pDbConnection">Open DB connection </param>
        ''' <param name="pRotorPositionsDS">Typed DataSet containing information of Rotor Positions that have to be emptied in a Rotor</param>
        ''' <param name="pWarningCodeFlag">By Reference parameter to set the code of the warning message that will be shown to the application User 
        '''                                when more positions than the selected have to be emptied in the Rotor (case of multipoint Calibrators)</param>
        ''' <returns>GlobalDataTO containing a typed DataSet WSRotorContentByPositionDS containing the same entry information plus positions found 
        '''          for Multipoint Calibrators</returns>
        ''' <remarks>
        ''' Created by:  VR 
        ''' Modified by: VR 31/12/2009 
        '''              VR 06/01/2010 
        '''              TR 11/01/2010 - Removed selected rows that correspond to positions without an ElementID informed
        '''              SA 13/01/2010 - Changed the way of opening the DB Connection to fulfill the new template
        '''              AG 01/06/2011 - Removed special management for Patient Samples with automatic dilutions. The dilution tubes are deleted indiviadually 
        '''                              due to the automatic predilutions are performed into the Reactions Rotor
        '''              SA 15/10/2011 - Code changed due to when there are Calibrators selected to be deleted, the rest of elements is lost (they are not 
        '''                              added to the DS that is finally returned)
        '''              TR 07/11/2013 - BT#1358 ==> Exclude from DS all elements with AllowedActionInPause = False. These elements cannot be deleted in Pause mode.
        ''' </remarks>
        Public Function CompleteDeletePositionSeleted(ByVal pDbConnection As SqlClient.SqlConnection, ByVal pRotorPositionsDS As WSRotorContentByPositionDS, _
                                                      ByRef pWarningCodeFlag As String, Optional pIsAnalyzerPause As Boolean = False) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDbConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Exclude from the DS all selected empty positions
                        Dim lstPositions As List(Of WSRotorContentByPositionDS.twksWSRotorContentByPositionRow)
                        lstPositions = (From a As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow In pRotorPositionsDS.twksWSRotorContentByPosition _
                                       Where (a.Status = "FREE" AndAlso a.IsBarcodeStatusNull) _
                                  OrElse Not a.AllowedActionInPause
                                      Select a).ToList

                        pRotorPositionsDS.twksWSRotorContentByPosition.BeginInit()
                        For Each emptyPos As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow In lstPositions
                            emptyPos.Delete()
                        Next
                        pRotorPositionsDS.twksWSRotorContentByPosition.AcceptChanges()

                        'Get all Calibrator tubes INCLUDED in the active WorkSession and placed in the Rotor Positions selected to be deleted
                        lstPositions = (From a As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow _
                                        In pRotorPositionsDS.twksWSRotorContentByPosition _
                                        Where a.TubeContent = "CALIB" _
                                        AndAlso Not a.IsElementIDNull _
                                        Order By a.ElementID _
                                        Select a).ToList

                        Dim myMultiPointPosDS As WSRotorContentByPositionDS = Nothing
                        Dim myRotorContentDAO As New twksWSRotorContentByPositionDAO
                        Dim cellSelected As List(Of WSRotorContentByPositionDS.twksWSRotorContentByPositionRow)

                        Dim myCalibID As Integer = 0
                        Dim myRequiredElementsDS As WSRequiredElementsDS = Nothing
                        Dim myRequiredElementsDelegate As New WSRequiredElementsDelegate

                        For Each calibPos As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow In lstPositions
                            resultData = myRequiredElementsDelegate.GetRequiredElementData(dbConnection, calibPos.ElementID)

                            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                myRequiredElementsDS = DirectCast(resultData.SetDatos, WSRequiredElementsDS)

                                If (myRequiredElementsDS.twksWSRequiredElements.Rows.Count > 0) Then
                                    If (myRequiredElementsDS.twksWSRequiredElements.First.CalibratorID <> myCalibID) Then
                                        myCalibID = myRequiredElementsDS.twksWSRequiredElements.First.CalibratorID

                                        'Verify if the Calibrator is a multipoint one, and in this case, return the Rotor Positions in which the rest of the points are placed
                                        resultData = myRotorContentDAO.GetMultiPointCalibratorPositions(dbConnection, _
                                                                       calibPos.WorkSessionID, myCalibID, calibPos.ElementID)

                                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                            myMultiPointPosDS = DirectCast(resultData.SetDatos, WSRotorContentByPositionDS)

                                            For Each rotorPos As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow In _
                                                                                                    myMultiPointPosDS.twksWSRotorContentByPosition
                                                'Verify if the cell in which the Calibrator Point is placed was selected to be deleted
                                                cellSelected = (From a As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow In pRotorPositionsDS.twksWSRotorContentByPosition _
                                                               Where a.CellNumber = rotorPos.CellNumber _
                                                              Select a).ToList
                                                'If the position had not been selected for deletion, it is added to the final DS of positions to delete
                                                'due to all the Calibrator Kit has to be downloaded
                                                If (cellSelected.Count = 0) Then pRotorPositionsDS.twksWSRotorContentByPosition.ImportRow(rotorPos)
                                                If (String.Compare(pWarningCodeFlag, String.Empty, False) = 0) Then pWarningCodeFlag = "FULL_KIT_DELETION"
                                            Next rotorPos
                                        Else
                                            'Error verifying if it is a multipoint Calibrator; the deleting of selected positions is aborted
                                            Exit For
                                        End If
                                    End If
                                End If
                            Else
                                'Error getting data of the Required WS Element; the deleting of selected positions is aborted
                                Exit For
                            End If
                        Next calibPos

                        'Get all positions containing Calibrators NOT INCLUDED in the active WorkSession and placed in the Rotor Positions selected to be deleted
                        If (Not resultData.HasError) Then
                            lstPositions = (From a As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow In pRotorPositionsDS.twksWSRotorContentByPosition _
                                           Where String.Compare(a.TubeContent, "CALIB", False) = 0 _
                                         AndAlso a.IsElementIDNull _
                                         AndAlso String.Compare(a.Status, "NO_INUSE", False) = 0 _
                                          Select a).ToList

                            Dim notInUseWarningFlag As String = String.Empty
                            Dim notInUseDelegate As New WSNotInUseRotorPositionsDelegate

                            For Each calibPos As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow In lstPositions
                                'Verify if the NotInUse Calibrator is a multipoint one, and in this case, return the Rotor Positions in which the rest of the points are placed
                                resultData = notInUseDelegate.CompleteSelection(dbConnection, calibPos, notInUseWarningFlag)

                                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                    myMultiPointPosDS = DirectCast(resultData.SetDatos, WSRotorContentByPositionDS)

                                    For Each rotorPos As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow In myMultiPointPosDS.twksWSRotorContentByPosition
                                        'Verify if the cell in which the Calibrator Point is placed was already selected to be deleted
                                        cellSelected = (From a As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow In pRotorPositionsDS.twksWSRotorContentByPosition _
                                                       Where a.CellNumber = rotorPos.CellNumber _
                                                      Select a).ToList

                                        'If the position had not been selected for deletion, it is added to the final DS of positions to delete
                                        'due to all the Calibrator Kit has to be downloaded
                                        If (cellSelected.Count = 0) Then pRotorPositionsDS.twksWSRotorContentByPosition.ImportRow(rotorPos)
                                    Next rotorPos

                                    If (pWarningCodeFlag = String.Empty) Then pWarningCodeFlag = notInUseWarningFlag
                                Else
                                    'Error verifying if the NotInUse Calibrator is a multipoint one; the deleting of selected positions is aborted
                                    Exit For
                                End If
                            Next calibPos
                        End If
                        lstPositions = Nothing
                        cellSelected = Nothing

                        If (Not resultData.HasError) Then
                            'Return the same DS received but updated with the missing possitions of selected multipoint Calibrators
                            resultData.SetDatos = pRotorPositionsDS
                        End If
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSRotorContentByPositionDelegate.CompleteDeletePositionSeleted", EventLogEntryType.Error, False)
            Finally
                If (pDbConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' When a new Work Session is created, add all Cells for each one of the Rotors the selected Analyzer has according its Model.  
        ''' All Cells are created with Status FREE. The Analyzer Model determines the number and types of Rotors and, for each one of them, 
        ''' the number of Rings and the total number of Cells by Ring, and also the number of each Cell. 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <returns>GlobalDataTO containing typed DataSet AnalyzerModelRotorsConfigDS with the basic information of the Analyzer 
        '''          in which all Rotors Positions were created</returns>
        ''' <remarks>
        ''' Created by:  AG 24/11/2009 - Tested: OK 27/11/099
        ''' Modified by: SA 04/01/2010 - Changed the way of open the DB Connection to the new template 
        '''              SA 19/04/2011 - After create all positions for all Analyzer Rotors, verify if there is a saved Internal Virtual 
        '''                              Rotor for Reagents and load it
        '''              AG 14/07/2011 - Set field ScannedPosition to NULL instead of to False
        '''              SA 12/01/2012 - Changed the function template; this function has to be a DB Transaction instead of a DB Connection
        ''' </remarks>
        Public Function CreateWSRotorPositions(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, _
                                               Optional ByVal pAnalyzerID As String = "") As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'If the AnalyzerID is not informed, then get the ID of the current Analyzer
                        If (String.Compare(pAnalyzerID.Trim, "", False) = 0) Then
                            Dim currentAnalyzer As New AnalyzersDelegate
                            resultData = currentAnalyzer.GetAnalyzer(dbConnection)
                            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                Dim analyzerReturned As AnalyzersDS = DirectCast(resultData.SetDatos, AnalyzersDS)
                                If (analyzerReturned.tcfgAnalyzers.Rows.Count > 0) Then
                                    'Update the parameter with the found Analyzer ID
                                    pAnalyzerID = analyzerReturned.tcfgAnalyzers(0).AnalyzerID
                                End If
                            End If
                        End If

                        'If an Analyzer was not found, it is not possible to continue
                        If (String.Compare(pAnalyzerID.Trim, "", False) <> 0) Then
                            'Search characteristics of each one of the Analyzer's Rotors according his model
                            Dim analyzerModelConf As AnalyzerModelRotorsConfigDS
                            Dim analyzerModel As New AnalyzerModelRotorsConfigDelegate

                            resultData = analyzerModel.GetAnalyzerRotorsConfiguration(dbConnection, pAnalyzerID)
                            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                analyzerModelConf = DirectCast(resultData.SetDatos, AnalyzerModelRotorsConfigDS)
                                If (analyzerModelConf.tfmwAnalyzerModelRotorsConfig.Rows.Count > 0) Then
                                    'Create all Rotors Rings & Cells
                                    Dim rotorContent As New WSRotorContentByPositionDS
                                    Dim configRow As AnalyzerModelRotorsConfigDS.tfmwAnalyzerModelRotorsConfigRow
                                    Dim newRotorRow As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow

                                    For Each configRow In analyzerModelConf.tfmwAnalyzerModelRotorsConfig
                                        For cellNumber = configRow.FirstCellNumber To configRow.LastCellNumber
                                            newRotorRow = rotorContent.twksWSRotorContentByPosition.NewtwksWSRotorContentByPositionRow()
                                            With newRotorRow
                                                .WorkSessionID = pWorkSessionID
                                                .AnalyzerID = pAnalyzerID
                                                .RotorType = configRow.RotorType
                                                .RingNumber = configRow.RingNumber
                                                .CellNumber = cellNumber
                                                .Status = "FREE"
                                                .SetScannedPositionNull()
                                            End With
                                            rotorContent.twksWSRotorContentByPosition.AddtwksWSRotorContentByPositionRow(newRotorRow)
                                        Next
                                    Next

                                    'Insert Rotor Ring Cell for the informed Work Session and return the Analyzer Model Configuration data
                                    resultData = AddRotorRingCell(dbConnection, rotorContent)

                                    'Verify if there is a saved Internal Virtual Rotor for Reagents and load it
                                    If (Not resultData.HasError) Then
                                        resultData = LoadRotor(dbConnection, pAnalyzerID, pWorkSessionID, "REAGENTS", -1)
                                    End If

                                    'Verify if there is a saved Internal Virtual Rotor for Samples and load it
                                    If (Not resultData.HasError) Then
                                        resultData = LoadRotor(dbConnection, pAnalyzerID, pWorkSessionID, "SAMPLES", -1)
                                    End If

                                    If (Not resultData.HasError) Then
                                        If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                                        resultData.SetDatos = analyzerModelConf
                                    Else
                                        If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                                    End If
                                End If
                            End If
                        End If
                    End If
                End If
            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSRotorContentByPositionDelegate.CreateWSRotorPositions", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Clear (or delete) a list of Rotor Positions
        ''' </summary>
        ''' <param name="pDbConnection">Open DB Connection</param>
        ''' <param name="pRotorPositionsDS">Typed DataSet containing information of all Rotor Positions that have to be emptied</param>
        ''' <param name="pOnlyCalculateElementStatus" >FALSE: Delete position and calculate new element status 
        '''                                            TRUE: Do not delete position but calculate new element status</param>
        ''' <returns> GlobalDataTO containing a typed DataSet WSRotorContentByPositionDS with the same entry information but 
        '''           with some fields updated: Status, MultiTubeNumber, ElementID and ElementStatus</returns>
        ''' <remarks>
        ''' Created by:  VR - Tested: Pending
        ''' Modified by: VR 17/12/2009 - Tested: OK (REAGENTS flow is pending - NO data)
        '''              VR 30/12/2009 - Tested: OK (Partial)
        '''              VR 31/12/2009 - Tested: OK (Partial - Reagent Flow is Pending)
        '''              VR 06/01/2010 - Tested: OK
        '''              TR 08/01/2010 
        '''              SA 13/01/2010 - Changed the way of opening the DB Transaction to fulfill the new template
        '''              AG 21/01/2010 - When NO_INUSE positions are deleted, they are deleted also from table twksWSNotInUseRotorPositions (Tested OK)
        '''              AG 12/04/2011 - Adapt code for DELETE (original case) and NO volume (new case using parameter pOnlyCalculateElementStatus)
        '''              TR 22/09/2011 - New business rule: Unload tubes from Samples Rotor and validate if exist another tube on the same rotor for  
        '''                              the same patient to indicate the Element Status as POS or NOPOS
        '''              SA 12/01/2012 - Changed the function template; code improved
        '''              SA 10/02/2012 - When a deleted position in REAGENTS Rotor contains an Additional Solution, the Element Status is calculated in the
        '''                              following way: ** If there is at least a not depleted bottle in the rotor: ElementStatus = POS
        '''                                             ** Else If there is at least a depleted bottle in the rotor: ElementStatus = INCOMPLETE
        '''                                             ** Else: ElementStatus = NOPOS  
        '''              SA 15/02/2012 - When a deleted position in REAGENTS Rotor contains a Reagent, the Element Status is calculated by getting the total volume
        '''                              of Reagent currently placed in the Rotor and verifying if it is enough to execute all pending tests for it (new function
        '''                              CalculateReagentStatus is called to do that)
        '''              AG 07/10/2014 - BA-1979 ==> Replaced call to function twksWSRotorContentByPositionDAO.Update by a call to function Update 
        '''                                          in this Delegate, informing the process who is updating the content of the Rotor Position (to 
        '''                                          search which process is inserting invalid values: positions with TubeContent but not element ID)
        ''' </remarks>
        Public Function DeletePositions(ByVal pDbConnection As SqlClient.SqlConnection, ByVal pRotorPositionsDS As WSRotorContentByPositionDS, _
                                        ByVal pOnlyCalculateElementStatus As Boolean) _
                                        As GlobalDataTO
            Dim returnedData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                returnedData = DAOBase.GetOpenDBTransaction(pDbConnection)
                If (Not returnedData.HasError AndAlso Not returnedData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(returnedData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Copy table into temporal DS
                        Dim myRotorContentByPosDS As New WSRotorContentByPositionDS
                        pRotorPositionsDS.twksWSRotorContentByPosition.CopyToDataTable(myRotorContentByPosDS.twksWSRotorContentByPosition, LoadOption.OverwriteChanges)

                        If (Not pOnlyCalculateElementStatus) Then
                            'Clear values of all positions selected to be deleted 
                            For Each myRotorContPosResetElem As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow In myRotorContentByPosDS.twksWSRotorContentByPosition.Rows
                                myRotorContPosResetElem.BeginEdit()
                                myRotorContPosResetElem.SetElementIDNull()
                                myRotorContPosResetElem.SetBarCodeInfoNull()
                                myRotorContPosResetElem.SetBarcodeStatusNull()
                                myRotorContPosResetElem.SetScannedPositionNull()
                                myRotorContPosResetElem.MultiTubeNumber = 0
                                myRotorContPosResetElem.SetTubeTypeNull()
                                myRotorContPosResetElem.SetTubeContentNull()
                                myRotorContPosResetElem.RealVolume = 0
                                myRotorContPosResetElem.RemainingTestsNumber = 0
                                myRotorContPosResetElem.Status = "FREE"
                                myRotorContPosResetElem.ElementStatus = "NOPOS"
                                myRotorContPosResetElem.Selected = False
                                myRotorContPosResetElem.EndEdit()
                            Next

                            'Clear content of selected rotor positions 
                            'Dim myWSRotorContentByPosDAO As New twksWSRotorContentByPositionDAO
                            'returnedData = myWSRotorContentByPosDAO.Update(dbConnection, myRotorContentByPosDS)
                            If myRotorContentByPosDS.twksWSRotorContentByPosition.Rows.Count > 0 Then
                                returnedData = Update(dbConnection, myRotorContentByPosDS.twksWSRotorContentByPosition(0).RotorType, myRotorContentByPosDS, ClassCalledFrom.DeletePositions) 'AG 07/10/2014 BA-1979 call the Update method  in delegate instead of in DAO
                            End If

                            If Not (returnedData.HasError) Then
                                'If there are NOT IN USE Positions selected to be deleted, get all of them
                                Dim myNoInUsePositions As List(Of WSRotorContentByPositionDS.twksWSRotorContentByPositionRow)
                                myNoInUsePositions = (From e As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow In pRotorPositionsDS.twksWSRotorContentByPosition _
                                                      Where e.Status = "NO_INUSE" _
                                                      Select e).ToList

                                'Delete NOT IN USE Positions from all possible tables
                                If (myNoInUsePositions.Count > 0) Then
                                    'Delete Not In Use positions from table of incomplete Barcode Samples 
                                    Dim BarcodePositionsWithNoRequests As New BarcodePositionsWithNoRequestsDelegate
                                    For Each row As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow In myNoInUsePositions
                                        returnedData = BarcodePositionsWithNoRequests.DeletePosition(dbConnection, row.AnalyzerID, row.WorkSessionID, row.RotorType, row.CellNumber)
                                        If (returnedData.HasError) Then Exit For
                                    Next

                                    If (Not returnedData.HasError) Then
                                        'Move not in use positions to a temporary DataSet
                                        Dim myRotorCbPos As New WSRotorContentByPositionDS
                                        For i = 0 To myNoInUsePositions.Count - 1
                                            myRotorCbPos.twksWSRotorContentByPosition.ImportRow(myNoInUsePositions(i))
                                        Next i

                                        'Delete Not In Use positions from the correspondent table
                                        Dim myNoInUsePositionDelegate As New WSNotInUseRotorPositionsDelegate
                                        returnedData = myNoInUsePositionDelegate.Delete(dbConnection, myRotorCbPos)
                                    End If
                                End If
                                myNoInUsePositions = Nothing
                            End If
                        End If

                        If Not (returnedData.HasError) Then
                            'If there are IN USE Positions selected to be deleted, get all of them
                            Dim myInUsePositions As List(Of WSRotorContentByPositionDS.twksWSRotorContentByPositionRow)
                            myInUsePositions = (From e As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow In pRotorPositionsDS.twksWSRotorContentByPosition _
                                           Where Not e.IsElementIDNull _
                                            Order By e.ElementID _
                                              Select e).ToList

                            Dim previousElementID As Integer = 0
                            Dim myUpdatedMultitubeNumberDS As WSRotorContentByPositionDS
                            Dim lstBottlesByStatus As List(Of WSRotorContentByPositionDS.twksWSRotorContentByPositionRow)

                            Dim myWSReqElementsDS As New WSRequiredElementsDS
                            Dim myRequiredElementsDelegate As New WSRequiredElementsDelegate
                            Dim myElementRow As WSRequiredElementsDS.twksWSRequiredElementsRow = myWSReqElementsDS.twksWSRequiredElements.NewtwksWSRequiredElementsRow

                            For Each myCellContentROW As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow In myInUsePositions
                                If (myCellContentROW.ElementID <> previousElementID) Then
                                    Select Case (myCellContentROW.TubeContent)
                                        Case "SPEC_SOL", "WASH_SOL"
                                            'Update the MultiTubeNumber of Element bottles still positioned in the Rotor
                                            returnedData = UpdateMultitubeNumber(dbConnection, myCellContentROW.AnalyzerID, myCellContentROW.RotorType, myCellContentROW.ElementID)
                                            If (Not returnedData.HasError AndAlso Not returnedData.SetDatos Is Nothing) Then
                                                myUpdatedMultitubeNumberDS = DirectCast(returnedData.SetDatos, WSRotorContentByPositionDS)

                                                If (myUpdatedMultitubeNumberDS.twksWSRotorContentByPosition.Rows.Count > 0) Then
                                                    'Verify if there is at least one not depleted bottle
                                                    lstBottlesByStatus = (From a As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow In myUpdatedMultitubeNumberDS.twksWSRotorContentByPosition _
                                                                         Where a.Status <> "DEPLETED" _
                                                                        Select a).ToList

                                                    myCellContentROW.ElementStatus = "POS"
                                                    If (lstBottlesByStatus.Count = 0) Then
                                                        'Verify if there is at least one depleted bottle
                                                        lstBottlesByStatus = (From a As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow In myUpdatedMultitubeNumberDS.twksWSRotorContentByPosition _
                                                                             Where a.Status = "DEPLETED" _
                                                                            Select a).ToList

                                                        myCellContentROW.ElementStatus = "NOPOS"
                                                        If (lstBottlesByStatus.Count > 0) Then myCellContentROW.ElementStatus = "INCOMPLETE"
                                                    End If

                                                    'Update TubeContent and Element Status for all bottles still positioned in the Rotor
                                                    For Each updatedMultiTubeElem As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow In myUpdatedMultitubeNumberDS.twksWSRotorContentByPosition
                                                        updatedMultiTubeElem.BeginEdit()
                                                        updatedMultiTubeElem.TubeContent = myCellContentROW.TubeContent
                                                        updatedMultiTubeElem.ElementStatus = myCellContentROW.ElementStatus
                                                        updatedMultiTubeElem.EndEdit()
                                                        myRotorContentByPosDS.twksWSRotorContentByPosition.ImportRow(updatedMultiTubeElem)
                                                    Next
                                                Else
                                                    'There are not more positioned bottles, the Additional Solution is not positioned
                                                    myCellContentROW.ElementStatus = "NOPOS"
                                                End If
                                            Else
                                                'Error updating the Multitube Number...
                                                Exit For
                                            End If

                                        Case "REAGENT"
                                            'Fill needed data in a row of WSRequiredElementsDS 
                                            myElementRow.BeginEdit()
                                            myElementRow.WorkSessionID = myCellContentROW.WorkSessionID
                                            myElementRow.ElementID = myCellContentROW.ElementID
                                            myElementRow.RequiredVolume = 0 'Current required volume is recalculated by function CalculateNeededBottlesAndReagentStatus
                                            myElementRow.EndEdit()

                                            returnedData = myRequiredElementsDelegate.CalculateNeededBottlesAndReagentStatus(dbConnection, myCellContentROW.AnalyzerID, myElementRow, 0)
                                            If (Not returnedData.HasError AndAlso Not returnedData.SetDatos Is Nothing) Then
                                                myElementRow = DirectCast(returnedData.SetDatos, WSRequiredElementsDS.twksWSRequiredElementsRow)

                                                'Update the ElementStatus for the Rotor Position
                                                myCellContentROW.ElementStatus = myElementRow.ElementStatus

                                                'Update the MultiTubeNumber of Element bottles still positioned in the Rotor
                                                returnedData = UpdateMultitubeNumber(dbConnection, myCellContentROW.AnalyzerID, myCellContentROW.RotorType, myCellContentROW.ElementID)
                                                If (Not returnedData.HasError AndAlso Not returnedData.SetDatos Is Nothing) Then
                                                    myUpdatedMultitubeNumberDS = DirectCast(returnedData.SetDatos, WSRotorContentByPositionDS)

                                                    'Update TubeContent and Element Status for all bottles still positioned in the Rotor
                                                    For Each updatedMultiTubeElem As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow In myUpdatedMultitubeNumberDS.twksWSRotorContentByPosition
                                                        updatedMultiTubeElem.BeginEdit()
                                                        updatedMultiTubeElem.TubeContent = myCellContentROW.TubeContent
                                                        updatedMultiTubeElem.ElementStatus = myCellContentROW.ElementStatus
                                                        updatedMultiTubeElem.EndEdit()
                                                        myRotorContentByPosDS.twksWSRotorContentByPosition.ImportRow(updatedMultiTubeElem)
                                                    Next
                                                Else
                                                    'Error updating the Multitube Number...
                                                    Exit For
                                                End If
                                            Else
                                                'Error calculating the Reagent Status...
                                                Exit For
                                            End If

                                        Case "PATIENT"
                                            'Update the MultiTubeNumber of Patient Sample Tubes still positioned in the Rotor
                                            returnedData = UpdateMultitubeNumber(dbConnection, myCellContentROW.AnalyzerID, myCellContentROW.RotorType, myCellContentROW.ElementID)
                                            If (Not returnedData.HasError AndAlso Not returnedData.SetDatos Is Nothing) Then
                                                myUpdatedMultitubeNumberDS = DirectCast(returnedData.SetDatos, WSRotorContentByPositionDS)

                                                If (myUpdatedMultitubeNumberDS.twksWSRotorContentByPosition.Where(Function(a) a.Status <> "DEPLETED").ToList.Count > 0) Then
                                                    'If there is at least a not depleted tube of the Patient Sample, the ElementStatus is set to POS
                                                    myCellContentROW.ElementStatus = "POS"
                                                Else
                                                    'There are not other tubes of the Patient Sample in the Rotor or there are but all of them are depleted, the ElementStatus is set to NOPOS
                                                    myCellContentROW.ElementStatus = "NOPOS"
                                                End If
                                            Else
                                                'Error updating the Multitube Number...
                                                Exit For
                                            End If

                                        Case Else
                                            myCellContentROW.ElementStatus = "NOPOS"
                                    End Select

                                    If (Not returnedData.HasError) Then
                                        'Update the Element Status
                                        returnedData = myRequiredElementsDelegate.UpdateStatus(dbConnection, myCellContentROW.ElementID, myCellContentROW.ElementStatus)
                                        If (returnedData.HasError) Then Exit For

                                        'Set the value of previous ElementID 
                                        previousElementID = myCellContentROW.ElementID
                                    End If
                                End If
                            Next
                            lstBottlesByStatus = Nothing
                            myInUsePositions = Nothing
                        End If


                        If (Not returnedData.HasError) Then
                            If (pDbConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                            If (Not pOnlyCalculateElementStatus) Then
                                returnedData.SetDatos = myRotorContentByPosDS
                            Else
                                returnedData.SetDatos = pRotorPositionsDS
                            End If
                        Else
                            If (pDbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                        End If
                    End If
                End If
            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDbConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                returnedData = New GlobalDataTO()
                returnedData.HasError = True
                returnedData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                returnedData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSRotorContentByPositionDelegate.DeletePositions", EventLogEntryType.Error, False)
            Finally
                If (pDbConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return returnedData
        End Function

        ''' <summary>
        ''' Count how many tubes there are in Samples Rotor for the active Work Session labelled with the specified 
        ''' Specimen Identifier
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pSpecimenID">Specimen Identifier to search</param>
        ''' <returns>GlobalDataTO containing a boolean value: TRUE when a tube with the specified SpecimenID exists as Barcode label
        '''          in Samples Rotor; otherwise FALSE</returns>
        ''' <remarks>
        ''' Created by:  SA 29/01/2014 - BT #1474
        ''' </remarks>
        Public Function ExistBarcodeInfo(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, ByVal pSpecimenID As String) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myWSRotorContentByPosition As New twksWSRotorContentByPositionDAO
                        myGlobalDataTO = myWSRotorContentByPosition.ExistBarcodeInfo(dbConnection, pWorkSessionID, pSpecimenID)
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                myGlobalDataTO.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "twksWSRotorContentByPositionDAO.ExistBarcodeInfo", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Search all Rotor positions containing a not depleted tube of the informed Element
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pElementID">Element Identifier</param>
        ''' <param name="pWorkSessionID">WorkSession Identifier</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet WSRotorContentByPositionsDS with all Rotor positions
        '''          with not depleted tubes of the informed Element</returns>
        ''' <remarks>
        ''' Created by:  DL 05/01/2011
        ''' </remarks>
        Public Function ExistOtherPosition(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pElementID As Integer, _
                                           ByVal pWorkSessionID As String, ByVal pAnalyzerID As String) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myWSRotorContentByPosition As New twksWSRotorContentByPositionDAO
                        myGlobalDataTO = myWSRotorContentByPosition.ExistOtherPosition(dbConnection, pElementID, pWorkSessionID, pAnalyzerID)
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSRotorContentByPositionDelegate.ExistOtherPosition", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get from Samples Rotor (for the specified Analyzer and WorkSession) all scanned tubes containing Patient Samples, regardless their Status (complete
        ''' and incomplete Patient Samples are returned)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pExcludeDuplicates">When TRUE, it indicates that cells containing duplicated Barcodes will not be returned</param>
        ''' <returns>GlobalDataTO containing a typed DataSet BarcodePositionsWithNoRequestsDS with all scanned Patient Samples in Samples Rotor (complete
        '''          and incomplete Patient Samples)</returns>
        ''' <remarks>
        ''' Createb by:  SA 10/07/2013
        ''' Modified by: AG 11/07/2013 - Implemented same treatment for the StatFlag than in method myBarcodePositionsWithNoRequestsDelegate.ReadByAnalyzerAndWorkSession
        '''              SA 24/07/2013 - Added new parameter to indicate when duplicated tubes have to be excluded
        ''' </remarks>
        Public Function GetAllPatientTubesForHQ(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, _
                                                ByVal pExcludeDuplicates As Boolean) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim elementPosition As New twksWSRotorContentByPositionDAO

                        myGlobalDataTO = elementPosition.GetAllPatientTubesForHQ(dbConnection, pAnalyzerID, pWorkSessionID, pExcludeDuplicates)
                        If (Not myGlobalDataTO.HasError) Then
                            Dim myBarcodePositionsWithNoRequestsDS As BarcodePositionsWithNoRequestsDS = DirectCast(myGlobalDataTO.SetDatos, BarcodePositionsWithNoRequestsDS)

                            'Get icons used for STAT and ROUTINE Patient Samples  
                            Dim preloadedDataConfig As New PreloadedMasterDataDelegate
                            Dim STATIcon As Byte() = preloadedDataConfig.GetIconImage("STATS")
                            Dim NORMALIcon As Byte() = preloadedDataConfig.GetIconImage("ROUTINES")

                            'Load the corresponding ICON in the DS to return according value of field StatFlag
                            For Each bcpRow As BarcodePositionsWithNoRequestsDS.twksWSBarcodePositionsWithNoRequestsRow In myBarcodePositionsWithNoRequestsDS.twksWSBarcodePositionsWithNoRequests.Rows
                                If (bcpRow.StatFlag) Then
                                    bcpRow.SampleClassIcon = STATIcon
                                Else
                                    bcpRow.SampleClassIcon = NORMALIcon
                                End If
                            Next

                            'NOTE (SA 10/12/2013): DO NOT DELETE!! 
                            'Dim myAuxiliaryBCPWithNRDS As New BarcodePositionsWithNoRequestsDS
                            'If (myBarcodePositionsWithNoRequestsDS.twksWSBarcodePositionsWithNoRequests.Rows.Count > 0) Then
                            '    'Get icons used for STAT and ROUTINE Patient Samples  
                            '    Dim preloadedDataConfig As New PreloadedMasterDataDelegate
                            '    Dim STATIcon As Byte() = preloadedDataConfig.GetIconImage("STATS")
                            '    Dim NORMALIcon As Byte() = preloadedDataConfig.GetIconImage("ROUTINES")

                            '    'Load the corresponding ICON in the DS to return according value of field StatFlag
                            '    Dim lstDuplicatedPatientSamples As List(Of BarcodePositionsWithNoRequestsDS.twksWSBarcodePositionsWithNoRequestsRow)
                            '    For Each bcpRow As BarcodePositionsWithNoRequestsDS.twksWSBarcodePositionsWithNoRequestsRow In myBarcodePositionsWithNoRequestsDS.twksWSBarcodePositionsWithNoRequests.Rows
                            '        If (bcpRow.StatFlag) Then
                            '            bcpRow.SampleClassIcon = STATIcon
                            '            myAuxiliaryBCPWithNRDS.twksWSBarcodePositionsWithNoRequests.ImportRow(bcpRow)
                            '        Else
                            '            'Search if the Samples Rotor Position exists in the returned DS also for the opposite StatFlag
                            '            lstDuplicatedPatientSamples = (From a As BarcodePositionsWithNoRequestsDS.twksWSBarcodePositionsWithNoRequestsRow In myBarcodePositionsWithNoRequestsDS.twksWSBarcodePositionsWithNoRequests _
                            '                                          Where a.CellNumber = bcpRow.CellNumber _
                            '                                        AndAlso a.StatFlag = True _
                            '                                         Select a).ToList()

                            '            If (lstDuplicatedPatientSamples.Count = 0) Then
                            '                bcpRow.SampleClassIcon = NORMALIcon
                            '                myAuxiliaryBCPWithNRDS.twksWSBarcodePositionsWithNoRequests.ImportRow(bcpRow)
                            '            End If
                            '        End If
                            '    Next
                            '    lstDuplicatedPatientSamples = Nothing
                            'End If

                            'myGlobalDataTO.SetDatos = myAuxiliaryBCPWithNRDS                            
                        End If
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSRotorContentByPositionDelegate.GetAllPatientTubesForHQ", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get all available Bottles for Reagent and Additional Solutions and all available Tubes for Patient Samples, Calibrators 
        ''' and Controls, for each Rotor Ring of an specified Analyzer Model
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerModel">Analyzer Model</param>
        ''' <returns> GlobalDataTO containing a list of objects TubeSizeTO with data of all available Bottles and Tubes for 
        '''           the specified Analyzer Model</returns>
        ''' <remarks>
        ''' Created by:  AG 24/11/2009 - Tested: OK
        ''' Modified by: SA 12/01/2010 - Added parameter for the Analyzer Model. Changes due to modifications in function GetAllBottles.
        '''                              Changes to call new function GetAllTubes instead of PreloadedMasterDataDelegate.GetList. 
        '''                              Changed the way of open the DB Connection to the new template
        ''' </remarks>
        Public Function GetAllTubeSizes(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerModel As String) As GlobalDataTO
            Dim dataToReturn As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                dataToReturn = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not dataToReturn.HasError AndAlso Not dataToReturn.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(dataToReturn.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Get all available bottles for Reagents and Additional Solutions by Rotor and 
                        'Ring in the specified Analyzer Model
                        Dim tubeSizeList As New List(Of TubeSizeTO)
                        Dim tubesByRotorRing As New AnalyzerModelTubesByRingDelegate

                        dataToReturn = tubesByRotorRing.GetAllBottles(dbConnection, pAnalyzerModel)
                        If (Not dataToReturn.HasError AndAlso Not dataToReturn.SetDatos Is Nothing) Then
                            Dim reagentsBottlesDS As AnalyzerModelTubesByRingDS = DirectCast(dataToReturn.SetDatos, AnalyzerModelTubesByRingDS)

                            'Insert all obtained bottles in the list of tubes and bottles to return
                            Dim count As Integer = 0
                            Dim bottleRow As AnalyzerModelTubesByRingDS.tfmwAnalyzerModelTubesByRingRow
                            For Each bottleRow In reagentsBottlesDS.tfmwAnalyzerModelTubesByRing
                                count = count + 1

                                Dim newElementList As New TubeSizeTO
                                With newElementList
                                    .RotorType = bottleRow.RotorType
                                    .RingNumber = bottleRow.RingNumber
                                    .TubeCode = bottleRow.TubeCode
                                    .FixedTubeName = bottleRow.TubeName
                                    .Volume = bottleRow.TubeVolume
                                    .Position = count
                                    .ManualUseFlag = bottleRow.ManualUseFlag
                                End With
                                tubeSizeList.Add(newElementList)
                            Next

                            'Get all available tubes for Samples by Rotor and Ring in the specified Analyzer Model
                            dataToReturn = tubesByRotorRing.GetAllTubes(dbConnection, pAnalyzerModel)
                            If (Not dataToReturn.HasError AndAlso Not dataToReturn.SetDatos Is Nothing) Then
                                Dim sampleTubesDS As AnalyzerModelTubesByRingDS = DirectCast(dataToReturn.SetDatos, AnalyzerModelTubesByRingDS)

                                'Insert all obtained Tubes in the list of tubes and bottles to return
                                Dim tubeRow As AnalyzerModelTubesByRingDS.tfmwAnalyzerModelTubesByRingRow
                                For Each tubeRow In sampleTubesDS.tfmwAnalyzerModelTubesByRing
                                    Dim newElementList As New TubeSizeTO
                                    With newElementList
                                        .RotorType = tubeRow.RotorType
                                        .RingNumber = tubeRow.RingNumber
                                        .TubeCode = tubeRow.TubeCode
                                        .FixedTubeName = tubeRow.TubeName
                                        .Volume = 0
                                        .Position = tubeRow.Position
                                    End With
                                    tubeSizeList.Add(newElementList)
                                Next

                                dataToReturn.SetDatos = tubeSizeList
                                dataToReturn.HasError = False
                            End If
                        End If
                        tubeSizeList = Nothing
                    End If
                End If
            Catch ex As Exception
                dataToReturn = New GlobalDataTO()
                dataToReturn.HasError = True
                dataToReturn.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                dataToReturn.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSRotorContentByPositionDelegate.GetAllTubeSizes", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return dataToReturn
        End Function

        ''' <summary>
        ''' Get the Analyzer and Rotor in which the informed required Element is positioned
        ''' for the specified Work Session
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pElementID">Element Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet WSRotorContentByPosition with the Analyzer
        '''          and Rotor in which the informed required Work Session Element is positioned</returns>
        ''' <remarks>
        ''' Created by:  SA 05/01/2010
        ''' </remarks>
        Public Function GetElementPositionInRotor(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, _
                                                  ByVal pElementID As Integer) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim elementPosition As New twksWSRotorContentByPositionDAO
                        myGlobalDataTO = elementPosition.ReadByElementIDAndWorkSessionID(dbConnection, pWorkSessionID, pElementID)
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSRotorContentByPositionDelegate.GetElementPositionInRotor", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get the number of bottles of a required Work Session Element (Reagent, Special Solution or Washing Solution)
        ''' currently placed in the specified Analyzer Rotor
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pRotorType">Rotor Type</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pElementID">Identifier of the required Element</param>
        ''' <returns>GlobalDataTO containing an Integer value with the total number of bottles in the Rotor</returns>
        ''' <remarks>
        ''' Created by:
        ''' </remarks>
        Public Function GetPlacedTubesByElement(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
                                                 ByVal pRotorType As String, ByVal pWorkSessionID As String, _
                                                 ByVal pElementID As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksWSRotorContentByPositionDAO
                        resultData = myDAO.GetPlacedTubesByElement(dbConnection, pAnalyzerID, pRotorType, pWorkSessionID, pElementID)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSRotorContentByPositionDelegate.GetPlacedTubesByElement", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Receive as entry a group of Elements required in a Work Session, and returns which of those Elements are 
        ''' positioned in the specified Analyzer Rotor
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalizerID">Analizer Identifier</param>
        ''' <param name="pRotorType">Rotor Type</param>
        ''' <param name="pElementList">List of Required Element Identifiers separated by commas</param>
        ''' <returns>GlobalDataTO containing a typed DataSet WSRotorContentByPositionDS having only the positioned 
        '''          Elements with the Ring and Cell Number in which each one is placed
        ''' </returns>
        ''' <remarks>
        ''' Created by:  TR 18/11/2009
        ''' Modified by: SA 13/01/2010 - Changed the way of opening the DB Connection to fulfill the new template
        '''              SA 12/01/2012 - Changed the function template
        ''' </remarks>
        Public Function GetPositionedElements(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalizerID As String, _
                                              ByVal pRotorType As String, ByVal pElementList As String) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myWSRotorContentByPosition As New twksWSRotorContentByPositionDAO
                        myGlobalDataTO = myWSRotorContentByPosition.ReadByElementIDList(dbConnection, pAnalizerID, pRotorType, pElementList)
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSRotorContentByPositionDelegate.GetPositionedElements", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get the total positioned volume by bottle of a Reagent required in the WorkSession
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pRotorType">Rotor Type</param>
        ''' <param name="pElementID">Identifier of the required Element</param>
        ''' <returns>GlobalDataTO containing a single value with the total positioned volume for the specified required Reagent Element</returns>
        ''' <remarks>
        ''' Created by:  TR 18/11/2009
        ''' Modified by: SA 13/01/2010 - Changed the way of opening the DB Connection to fulfill the new template
        '''              SA 12/01/2012 - Changed the function template
        '''              SA 27/02/2012 - Get the residual volume defined for Reagent Bottles in SW Parameters. Calculate the total positioned
        '''                              volume excluding the residual volume of each bottle 
        '''              SA 02/03/2012 - Called new function CalculateDeathVolByBottleType to get the death volume of each bottle according its size
        ''' </remarks>
        Public Function GetPositionedReagentVolume(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pRotorType As String, _
                                                   ByVal pElementID As Integer) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Get the number of positioned bottles and the total positioned volume by bottle of a Reagent required in the WorkSession
                        Dim myTotalVolume As Single = 0
                        Dim myWSRotorContentByPosition As New twksWSRotorContentByPositionDAO

                        myGlobalDataTO = myWSRotorContentByPosition.GetPositionedReagentVolume(dbConnection, pAnalyzerID, pRotorType, pElementID)
                        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                            Dim myReagentBottlesDS As ReagentTubeTypesDS = DirectCast(myGlobalDataTO.SetDatos, ReagentTubeTypesDS)

                            Dim deathVolume As Single = 0
                            Dim reagentBottles As New ReagentTubeTypesDelegate
                            For Each bottleType As ReagentTubeTypesDS.ReagentTubeTypesRow In myReagentBottlesDS.ReagentTubeTypes
                                'Calculate the death volume for the bottle according its size
                                myGlobalDataTO = reagentBottles.CalculateDeathVolByBottleType(dbConnection, bottleType.TubeCode, bottleType.Section)
                                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                    deathVolume = CType(myGlobalDataTO.SetDatos, Single)

                                    'Calculate the total positioned volume as: SUM(RealVolume of all bottles of this size) - (COUNT(All bottles of this size) * DeathVolume)
                                    'AG 10/03/2014 - #1539
                                    'If (bottleType.RealVolume > 0) Then
                                    If (Not bottleType.IsRealVolumeNull AndAlso bottleType.RealVolume > 0) Then
                                        'AG 10/03/2014 - #1539
                                        myTotalVolume += bottleType.RealVolume - (bottleType.NumOfBottles * deathVolume)
                                    End If
                                Else
                                    Exit For
                                End If
                            Next
                        End If

                        myGlobalDataTO.SetDatos = myTotalVolume
                        myGlobalDataTO.HasError = False
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSRotorContentByPositionDelegate.GetPositionedReagentVolume", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        '''<summary>
        ''' Get the information of the content in an specific Rotor Position. If the Rotor Position
        ''' contains a Required Element, then the specific information of it is obtained and returned
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pRotorPositionDS">Typed DataSet containing information of the Rotor Position for 
        '''                                which detailed information is consulted</param>
        ''' <returns>GlobalDataTO containing a typed DataSet CellPositionInformationDS with the detailed 
        '''          information of the content of a Rotor Position</returns>
        ''' <remarks>
        ''' Created by:  BK - 08/12/2009 - Tested: OK
        ''' Modified by: TR 10/12/2009 - Change the PredilutionFactor validation to show the PredilutionFactor value 
        '''                              if it is greater than 0 in the corresponding format. Until the indicated date 
        '''                              this function is INCOMPLETE.
        '''              BK 18/12/2009 - Change the PredilutionFactor, Diluted Status as True/False.
        '''                              Change field type definition according the fields
        '''              BK 22/12/2009 - Changes in typed Datasets - AX00Types
        '''              SA 23/12/2009 - Changes needed to get all the information in each case. Changes in the code structure
        '''              SA 13/01/2010 - Changed the way of opening the DB Connection to fulfill the new template
        '''              TR 25/01/2010 - Set the tubeType for Elements Status = NO_INUSE.
        '''              AG 28/01/2010 - Fill and return the RealVolume, RemainingTestsLeft values (Tested OK)
        '''              SA 10/03/2010 - Changes due to new field SampleID for Patient Order Tests
        '''              SA 26/10/2010 - When the position contains a Not In Use Element and it corresponds to a Patient Sample of 
        '''                              diluted URI for ISE Tests, the suffix (ISE) has to be added to the Sample Type. For Washing
        '''                              and Special Solutions, changed the way of getting the Solution Name. 
        '''              RH 16/06/2011 - Add TUBE_WASH_SOL and TUBE_SPEC_SOL. Code optimization.
        '''              SA 29/09/2011 - Search of multilanguage description of a not in use position containing Tube Special Solutions was bad done;
        '''                              it has to be searched by SolutionCode in subtable SPECIAL_SOLUTIONS in table of Preloaded Master Data
        '''              SA 07/10/2011 - For each NOT INUSE Reagent, get the name of all Tests that can use it and fill field TestList in the DS
        '''              TR 14/11/213  - BA-1383 ==> Calculate the remaining test number for reagents with status NOT INUSE
        '''              SA 07/01/2015 - BA-1999 ==> Code for IN USE Positions moved to a new function GetINUSEPositionInfo; code for NOT IN USE Positions
        '''                                          moved to a new function GetNOT_INUSEPositionInfo; calculation of number or remaining tests for NOT IN 
        '''                                          USE Positions moved to the new function GetNOT_INUSEPositionInfo 
        ''' </remarks>
        Public Function GetPositionInfo(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pRotorPositionDS As WSRotorContentByPositionDS) As GlobalDataTO
            Dim resultData As GlobalDataTO
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        If (Not pRotorPositionDS Is Nothing) Then
                            If (pRotorPositionDS.twksWSRotorContentByPosition.Rows.Count > 0) Then
                                Dim myCellPosInfoDS As New CellPositionInformationDS

                                'Get all data of the specified Rotor Position
                                Dim myRotorContentPosition As New twksWSRotorContentByPositionDAO
                                resultData = myRotorContentPosition.Read(dbConnection, pRotorPositionDS.twksWSRotorContentByPosition(0).AnalyzerID, _
                                                                         pRotorPositionDS.twksWSRotorContentByPosition(0).WorkSessionID, _
                                                                         pRotorPositionDS.twksWSRotorContentByPosition(0).RotorType, _
                                                                         pRotorPositionDS.twksWSRotorContentByPosition(0).CellNumber, _
                                                                         pRotorPositionDS.twksWSRotorContentByPosition(0).RingNumber)

                                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                    Dim myRotorContentByPositionDS As WSRotorContentByPositionDS = DirectCast(resultData.SetDatos, WSRotorContentByPositionDS)

                                    If (myRotorContentByPositionDS.twksWSRotorContentByPosition.Rows.Count > 0) Then
                                        Dim myRow As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow = myRotorContentByPositionDS.twksWSRotorContentByPosition.First

                                        ''TR 14/11/2013 -BT #1383 Get all the Reagents not in use and complete the remaining test.
                                        'If Not myGlobalDataTo.HasError Then
                                        '    Dim myWSRotorContentByPositionDS As New WSRotorContentByPositionDS
                                        '    Dim myListNotInUse As New List(Of WSRotorContentByPositionDS.twksWSRotorContentByPositionRow)
                                        '    myWSRotorContentByPositionDS = DirectCast(myGlobalDataTo.SetDatos, WSRotorContentByPositionDS)
                                        '    'Get all the elements with status  NOT_INUSE
                                        '    myListNotInUse = (From a In myWSRotorContentByPositionDS.twksWSRotorContentByPosition
                                        '                      Where a.Status = "NO_INUSE" Select a).ToList()
                                        '    For Each NotInUseElement As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow In myListNotInUse
                                        '        'Validate the Reagent id and the real volume are not null values to avoid exception error.
                                        '        If Not pRotorPositionDS.twksWSRotorContentByPosition(0).IsReagentIDNull AndAlso _
                                        '            Not pRotorPositionDS.twksWSRotorContentByPosition(0).IsRealVolumeNull Then

                                        '            'calculate the remaining test number.
                                        '            myGlobalDataTo = CalculateRemainingTestNotInUseReagent(dbConnection, NotInUseElement.WorkSessionID, _
                                        '                                                  pRotorPositionDS.twksWSRotorContentByPosition(0).ReagentID, _
                                        '                                                  NotInUseElement.MultiTubeNumber, pRotorPositionDS.twksWSRotorContentByPosition(0).RealVolume, _
                                        '                                                  NotInUseElement.TubeType)

                                        '            If Not myGlobalDataTo.HasError Then
                                        '                NotInUseElement.RemainingTestsNumber = CInt(myGlobalDataTo.SetDatos)
                                        '            End If
                                        '        End If
                                        '    Next
                                        'End If
                                        ''TR 14/11/2013 -END.

                                        'Fill fields in table PositionInformation 
                                        Dim myCellPosInfoRow As CellPositionInformationDS.PositionInformationRow
                                        myCellPosInfoRow = myCellPosInfoDS.PositionInformation.NewPositionInformationRow()

                                        myCellPosInfoRow.RingNumber = myRow.RingNumber
                                        myCellPosInfoRow.CellNumber = myRow.CellNumber
                                        myCellPosInfoRow.Status = myRow.Status.ToString
                                        myCellPosInfoRow.BarcodeInfo = myRow.BarCodeInfo.ToString
                                        myCellPosInfoRow.BarcodeStatus = myRow.BarcodeStatus.ToString
                                        If (Not myRow.IsElementIDNull) Then
                                            myCellPosInfoRow.ElementID = myRow.ElementID
                                        End If
                                        myCellPosInfoDS.PositionInformation.Rows.Add(myCellPosInfoRow)

                                        If (Not myRow.IsElementIDNull) Then
                                            'Get information of an INUSE Rotor Position
                                            resultData = GetINUSEPositionInfo(dbConnection, myRow, myCellPosInfoDS)
                                        ElseIf (myRow.Status <> "FREE") Then
                                            'Get information of a NOT INUSE Rotor Position
                                            resultData = GetNOT_INUSEPositionInfo(dbConnection, myRow, myCellPosInfoDS)
                                        End If
                                    End If

                                    'If (myRotorContentByPositionDS.twksWSRotorContentByPosition.Rows.Count > 0) AndAlso (Not myRotorContentByPositionDS.twksWSRotorContentByPosition(0).IsElementIDNull) Then
                                    ''Get data of the specified Required Work Session Element 
                                    'Dim myGlobalDataTo As GlobalDataTO
                                    'Dim myRequiredElements As New WSRequiredElementsDelegate
                                    'myGlobalDataTo = myRequiredElements.GetRequiredElementInfo(dbConnection, myRotorContentByPositionDS.twksWSRotorContentByPosition(0).ElementID)

                                    'If (Not myGlobalDataTo.HasError AndAlso Not myGlobalDataTo.SetDatos Is Nothing) Then
                                    '    Dim resultDataDS As WSRequiredElementsTreeDS
                                    '    resultDataDS = DirectCast(myGlobalDataTo.SetDatos, WSRequiredElementsTreeDS)

                                    '    Dim myCellSamplesPosInfoRow As CellPositionInformationDS.SamplesRow
                                    '    Dim myCellReagentPosInfoRow As CellPositionInformationDS.ReagentsRow

                                    '    'By checking which of the tables in the DataSet has rows, the data is obtained from the proper
                                    '    'table in the returned DataSet and inserted in the proper table in the final DataSet to return
                                    '    If (resultDataDS.Controls.Rows.Count > 0) Then
                                    '        'The TubeContent is informed in table PositionInfo in the DataSet to return
                                    '        myCellPosInfoDS.PositionInformation(0).BeginEdit()
                                    '        myCellPosInfoDS.PositionInformation(0).Content = "CTRL"
                                    '        myCellPosInfoDS.PositionInformation(0).SetSolutionCodeNull() 'AG 11/01/2012
                                    '        myCellPosInfoDS.PositionInformation(0).EndEdit()

                                    '        'Information is get from Controls table in WSRequiredElementsTreeDS and 
                                    '        'inserted in Samples Table in CellPositionInformationDS.  Value of field 
                                    '        'TubeType is in the DataSet with position information (myRotorContentByPositionDS)
                                    '        myCellSamplesPosInfoRow = myCellPosInfoDS.Samples.NewSamplesRow()
                                    '        myCellSamplesPosInfoRow.ElementID = resultDataDS.Controls(0).ElementID
                                    '        myCellSamplesPosInfoRow.SampleID = resultDataDS.Controls(0).ControlName
                                    '        myCellSamplesPosInfoRow.LotNumber = resultDataDS.Controls(0).LotNumber
                                    '        myCellSamplesPosInfoRow.ExpirationDate = Convert.ToDateTime(resultDataDS.Controls(0).ExpirationDate)
                                    '        myCellSamplesPosInfoRow.TubeType = myRotorContentByPositionDS.twksWSRotorContentByPosition(0).TubeType
                                    '        myCellPosInfoDS.Samples.Rows.Add(myCellSamplesPosInfoRow)

                                    '    ElseIf (resultDataDS.Calibrators.Rows.Count > 0) Then
                                    '        'The TubeContent is informed in table PositionInfo in the DataSet to return
                                    '        myCellPosInfoDS.PositionInformation(0).BeginEdit()
                                    '        myCellPosInfoDS.PositionInformation(0).Content = "CALIB"
                                    '        myCellPosInfoDS.PositionInformation(0).SetSolutionCodeNull() 'AG 11/01/2012
                                    '        myCellPosInfoDS.PositionInformation(0).EndEdit()

                                    '        'Information is get from Calibrators table in WSRequiredElementsTreeDS and 
                                    '        'inserted in Samples Table in CellPositionInformationDS.  Value of field 
                                    '        'TubeType is in the DataSet with position information (myRotorContentByPositionDS)
                                    '        myCellSamplesPosInfoRow = myCellPosInfoDS.Samples.NewSamplesRow()
                                    '        myCellSamplesPosInfoRow.ElementID = resultDataDS.Calibrators(0).ElementID
                                    '        myCellSamplesPosInfoRow.SampleID = resultDataDS.Calibrators(0).CalibratorName
                                    '        myCellSamplesPosInfoRow.MultiItemNumber = resultDataDS.Calibrators(0).CalibratorNumber
                                    '        myCellSamplesPosInfoRow.LotNumber = resultDataDS.Calibrators(0).LotNumber
                                    '        myCellSamplesPosInfoRow.ExpirationDate = Convert.ToDateTime(resultDataDS.Calibrators(0).ExpirationDate)
                                    '        myCellSamplesPosInfoRow.TubeType = myRotorContentByPositionDS.twksWSRotorContentByPosition(0).TubeType
                                    '        myCellPosInfoDS.Samples.Rows.Add(myCellSamplesPosInfoRow)

                                    '    ElseIf (resultDataDS.PatientSamples.Rows.Count > 0) Then
                                    '        'The TubeContent is informed in table PositionInfo in the DataSet to return
                                    '        myCellPosInfoDS.PositionInformation(0).BeginEdit()
                                    '        myCellPosInfoDS.PositionInformation(0).Content = "PATIENT"
                                    '        myCellPosInfoDS.PositionInformation(0).SetSolutionCodeNull() 'AG 11/01/2012
                                    '        myCellPosInfoDS.PositionInformation(0).EndEdit()

                                    '        'Information is get from Patients table in WSRequiredElementsTreeDS and 
                                    '        'inserted in Samples Table in CellPositionInformationDS.  Value of field 
                                    '        'TubeType is in the DataSet with position information (myRotorContentByPositionDS)
                                    '        myCellSamplesPosInfoRow = myCellPosInfoDS.Samples.NewSamplesRow()
                                    '        myCellSamplesPosInfoRow.ElementID = resultDataDS.PatientSamples(0).ElementID

                                    '        If (Not resultDataDS.PatientSamples(0).IsPatientIDNull) Then
                                    '            myCellSamplesPosInfoRow.SampleID = resultDataDS.PatientSamples(0).PatientID
                                    '        ElseIf Not resultDataDS.PatientSamples(0).IsSampleIDNull Then
                                    '            myCellSamplesPosInfoRow.SampleID = resultDataDS.PatientSamples(0).SampleID
                                    '        ElseIf Not resultDataDS.PatientSamples(0).IsOrderIDNull Then
                                    '            myCellSamplesPosInfoRow.SampleID = resultDataDS.PatientSamples(0).OrderID
                                    '        End If

                                    '        myCellSamplesPosInfoRow.SampleType = resultDataDS.PatientSamples(0).SampleType

                                    '        If (Not resultDataDS.PatientSamples(0).IsPredilutionFactorNull) Then
                                    '            myCellSamplesPosInfoRow.PredilutionFactor = resultDataDS.PatientSamples(0).PredilutionFactor
                                    '            myCellSamplesPosInfoRow.Diluted = (resultDataDS.PatientSamples(0).PredilutionFactor > 0)
                                    '        End If

                                    '        myCellSamplesPosInfoRow.Stat = resultDataDS.PatientSamples(0).StatFlag
                                    '        myCellSamplesPosInfoRow.TubeType = myRotorContentByPositionDS.twksWSRotorContentByPosition(0).TubeType
                                    '        myCellPosInfoDS.Samples.Rows.Add(myCellSamplesPosInfoRow)

                                    '    ElseIf (resultDataDS.Reagents.Rows.Count > 0) Then
                                    '        'The TubeContent is informed in table PositionInfo in the DataSet to return
                                    '        myCellPosInfoDS.PositionInformation(0).BeginEdit()
                                    '        myCellPosInfoDS.PositionInformation(0).Content = "REAGENT"
                                    '        myCellPosInfoDS.PositionInformation(0).SetSolutionCodeNull() 'AG 11/01/2012
                                    '        myCellPosInfoDS.PositionInformation(0).EndEdit()

                                    '        'Information is get from Reagents table in WSRequiredElementsTreeDS and 
                                    '        'inserted in Reagents Table in CellPositionInformationDS.  Value of field 
                                    '        'BottleCode is in the DataSet with position information (myRotorContentByPositionDS)
                                    '        myCellReagentPosInfoRow = myCellPosInfoDS.Reagents.NewReagentsRow()
                                    '        myCellReagentPosInfoRow.ElementID = resultDataDS.Reagents(0).ElementID
                                    '        myCellReagentPosInfoRow.ReagentName = resultDataDS.Reagents(0).ReagentName
                                    '        myCellReagentPosInfoRow.MultiItemNumber = resultDataDS.Reagents(0).ReagentNumber
                                    '        myCellReagentPosInfoRow.TestList = resultDataDS.Reagents(0).TestName
                                    '        myCellReagentPosInfoRow.BottleCode = myRotorContentByPositionDS.twksWSRotorContentByPosition(0).TubeType

                                    '        If (Not resultDataDS.Reagents(0).IsLotNumberNull) Then
                                    '            myCellReagentPosInfoRow.LotNumber = resultDataDS.Reagents(0).LotNumber.ToString
                                    '        End If

                                    '        If (Not resultDataDS.Reagents(0).IsExpirationDateNull) Then
                                    '            myCellReagentPosInfoRow.ExpirationDate = Convert.ToDateTime(resultDataDS.Reagents(0).ExpirationDate)
                                    '        End If

                                    '        If (Not myRotorContentByPositionDS.twksWSRotorContentByPosition(0).IsRealVolumeNull) Then
                                    '            myCellReagentPosInfoRow.RealVolume = myRotorContentByPositionDS.twksWSRotorContentByPosition(0).RealVolume
                                    '        End If
                                    '        'TR 14/11/2013 -BT #1383
                                    '        If (Not myRotorContentByPositionDS.twksWSRotorContentByPosition(0).IsRemainingTestsNumberNull) Then
                                    '            myCellReagentPosInfoRow.RemainingTests = myRotorContentByPositionDS.twksWSRotorContentByPosition(0).RemainingTestsNumber
                                    '        End If
                                    '        'TR 14/11/2013 -BT #1383 -END.
                                    '        If (Not myRotorContentByPositionDS.twksWSRotorContentByPosition(0).IsRemainingTestsNumberNull) Then
                                    '            myCellReagentPosInfoRow.RemainingTests = myRotorContentByPositionDS.twksWSRotorContentByPosition(0).RemainingTestsNumber
                                    '        End If
                                    '        myCellPosInfoDS.Reagents.Rows.Add(myCellReagentPosInfoRow)

                                    '    ElseIf (resultDataDS.AdditionalSolutions.Rows.Count > 0) Then
                                    '        'The TubeContent is informed in table PositionInfo in the DataSet to return
                                    '        myCellPosInfoDS.PositionInformation(0).BeginEdit()
                                    '        myCellPosInfoDS.PositionInformation(0).Content = resultDataDS.AdditionalSolutions(0).TubeContent
                                    '        myCellPosInfoDS.PositionInformation(0).SetSolutionCodeNull()
                                    '        If (Not resultDataDS.AdditionalSolutions(0).IsSolutionCodeNull) Then
                                    '            myCellPosInfoDS.PositionInformation(0).SolutionCode = resultDataDS.AdditionalSolutions(0).SolutionCode
                                    '        End If
                                    '        myCellPosInfoDS.PositionInformation(0).EndEdit()

                                    '        'Information is get from AdditionalSolutions table in WSRequiredElementsTreeDS and 
                                    '        'inserted in Reagents Table in CellPositionInformationDS.  Value of field 
                                    '        'BottleCode is in the DataSet with position information (myRotorContentByPositionDS)
                                    '        myCellReagentPosInfoRow = myCellPosInfoDS.Reagents.NewReagentsRow()
                                    '        myCellReagentPosInfoRow.ElementID = resultDataDS.AdditionalSolutions(0).ElementID
                                    '        myCellReagentPosInfoRow.ReagentName = resultDataDS.AdditionalSolutions(0).SolutionName
                                    '        myCellReagentPosInfoRow.BottleCode = myRotorContentByPositionDS.twksWSRotorContentByPosition(0).TubeType

                                    '        If (Not myRotorContentByPositionDS.twksWSRotorContentByPosition(0).IsRealVolumeNull) Then
                                    '            myCellReagentPosInfoRow.RealVolume = myRotorContentByPositionDS.twksWSRotorContentByPosition(0).RealVolume
                                    '        End If
                                    '        If (Not myRotorContentByPositionDS.twksWSRotorContentByPosition(0).IsRemainingTestsNumberNull) Then
                                    '            myCellReagentPosInfoRow.RemainingTests = myRotorContentByPositionDS.twksWSRotorContentByPosition(0).RemainingTestsNumber
                                    '        End If
                                    '        myCellPosInfoDS.Reagents.Rows.Add(myCellReagentPosInfoRow)

                                    '    ElseIf (resultDataDS.TubeAdditionalSolutions.Rows.Count > 0) Then
                                    '        'The TubeContent is informed in table PositionInfo in the DataSet to return
                                    '        myCellPosInfoDS.PositionInformation(0).BeginEdit()
                                    '        myCellPosInfoDS.PositionInformation(0).Content = resultDataDS.TubeAdditionalSolutions(0).TubeContent
                                    '        myCellPosInfoDS.PositionInformation(0).SetSolutionCodeNull()
                                    '        If (Not resultDataDS.TubeAdditionalSolutions(0).IsSolutionCodeNull) Then
                                    '            myCellPosInfoDS.PositionInformation(0).SolutionCode = resultDataDS.TubeAdditionalSolutions(0).SolutionCode
                                    '        End If
                                    '        myCellPosInfoDS.PositionInformation(0).EndEdit()

                                    '        'Information is get from AdditionalSolutions table in WSRequiredElementsTreeDS and 
                                    '        'inserted in Samples Table in CellPositionInformationDS.  Value of field 
                                    '        'TubeType is in the DataSet with position information (myRotorContentByPositionDS)
                                    '        myCellSamplesPosInfoRow = myCellPosInfoDS.Samples.NewSamplesRow()
                                    '        myCellSamplesPosInfoRow.ElementID = resultDataDS.TubeAdditionalSolutions(0).ElementID
                                    '        myCellSamplesPosInfoRow.SampleID = resultDataDS.TubeAdditionalSolutions(0).SolutionName
                                    '        myCellSamplesPosInfoRow.TubeType = myRotorContentByPositionDS.twksWSRotorContentByPosition(0).TubeType
                                    '        myCellPosInfoDS.Samples.Rows.Add(myCellSamplesPosInfoRow)
                                    '        'Else
                                    '        'TODO: pending process for other Solutions: ISE, etc
                                    '    End If
                                    'End If

                                    'ElseIf (myRotorContentByPositionDS.twksWSRotorContentByPosition.Rows.Count > 0) AndAlso _
                                    '       (myRotorContentByPositionDS.twksWSRotorContentByPosition(0).IsElementIDNull) AndAlso _
                                    '       (String.Compare(myRotorContentByPositionDS.twksWSRotorContentByPosition(0).Status, "NO_INUSE", False) = 0) Then
                                    ''Process for NOT IN USE positions
                                    'Dim myNoInUsePositions As New WSNotInUseRotorPositionsDelegate
                                    'myGlobalDataTo = myNoInUsePositions.GetPositionContent(dbConnection, pRotorPositionDS.twksWSRotorContentByPosition(0).AnalyzerID, _
                                    '                                                       pRotorPositionDS.twksWSRotorContentByPosition(0).RotorType, _
                                    '                                                       pRotorPositionDS.twksWSRotorContentByPosition(0).RingNumber, _
                                    '                                                       pRotorPositionDS.twksWSRotorContentByPosition(0).CellNumber, _
                                    '                                                       pRotorPositionDS.twksWSRotorContentByPosition(0).WorkSessionID)

                                    'Dim resultDataDS As New VirtualRotorPosititionsDS
                                    'Dim myCellSamplesPosInfoRow As CellPositionInformationDS.SamplesRow
                                    'Dim myCellReagentPosInfoRow As CellPositionInformationDS.ReagentsRow

                                    'If (Not myGlobalDataTo.HasError AndAlso Not myGlobalDataTo.SetDatos Is Nothing) Then
                                    '    resultDataDS = DirectCast(myGlobalDataTo.SetDatos, VirtualRotorPosititionsDS)

                                    '    If (resultDataDS.tparVirtualRotorPosititions.Rows.Count > 0) Then
                                    '        Select Case (resultDataDS.tparVirtualRotorPosititions(0).TubeContent())
                                    '            Case "WASH_SOL"
                                    '                myCellPosInfoDS.PositionInformation(0).BeginEdit()
                                    '                myCellPosInfoDS.PositionInformation(0).Content = "WASH_SOL"
                                    '                myCellPosInfoDS.PositionInformation(0).SetSolutionCodeNull()
                                    '                If (Not resultDataDS.tparVirtualRotorPosititions(0).IsSolutionCodeNull) Then
                                    '                    myCellPosInfoDS.PositionInformation(0).SolutionCode = resultDataDS.tparVirtualRotorPosititions(0).SolutionCode
                                    '                End If
                                    '                myCellPosInfoDS.PositionInformation(0).EndEdit()

                                    '                Dim myPreloadedMasterDataDelegate As New PreloadedMasterDataDelegate
                                    '                myGlobalDataTo = myPreloadedMasterDataDelegate.GetSubTableItem(dbConnection, GlobalEnumerates.PreloadedMasterDataEnum.WASHING_SOLUTIONS, _
                                    '                                                                               resultDataDS.tparVirtualRotorPosititions(0).SolutionCode)
                                    '                If (Not myGlobalDataTo.HasError AndAlso Not myGlobalDataTo.SetDatos Is Nothing) Then
                                    '                    Dim myPreloadedMasterDataDS As PreloadedMasterDataDS = DirectCast(myGlobalDataTo.SetDatos, PreloadedMasterDataDS)

                                    '                    myCellReagentPosInfoRow = myCellPosInfoDS.Reagents.NewReagentsRow()
                                    '                    myCellReagentPosInfoRow.ReagentName = myPreloadedMasterDataDS.tfmwPreloadedMasterData(0).FixedItemDesc
                                    '                    If (Not myRotorContentByPositionDS.twksWSRotorContentByPosition(0).IsRealVolumeNull) Then
                                    '                        myCellReagentPosInfoRow.RealVolume = myRotorContentByPositionDS.twksWSRotorContentByPosition(0).RealVolume
                                    '                    End If
                                    '                    If (Not myRotorContentByPositionDS.twksWSRotorContentByPosition(0).IsRemainingTestsNumberNull) Then
                                    '                        myCellReagentPosInfoRow.RemainingTests = myRotorContentByPositionDS.twksWSRotorContentByPosition(0).RemainingTestsNumber
                                    '                    End If
                                    '                    myCellReagentPosInfoRow.BottleCode = myRotorContentByPositionDS.twksWSRotorContentByPosition(0).TubeType
                                    '                    myCellPosInfoDS.Reagents.Rows.Add(myCellReagentPosInfoRow)
                                    '                End If

                                    '            Case "SPEC_SOL"
                                    '                myCellPosInfoDS.PositionInformation(0).BeginEdit()
                                    '                myCellPosInfoDS.PositionInformation(0).Content = "SPEC_SOL"
                                    '                myCellPosInfoDS.PositionInformation(0).SetSolutionCodeNull()
                                    '                If (Not resultDataDS.tparVirtualRotorPosititions(0).IsSolutionCodeNull) Then
                                    '                    myCellPosInfoDS.PositionInformation(0).SolutionCode = resultDataDS.tparVirtualRotorPosititions(0).SolutionCode
                                    '                End If
                                    '                myCellPosInfoDS.PositionInformation(0).EndEdit()

                                    '                Dim myPreloadedMasterDataDelegate As New PreloadedMasterDataDelegate
                                    '                myGlobalDataTo = myPreloadedMasterDataDelegate.GetSubTableItem(dbConnection, GlobalEnumerates.PreloadedMasterDataEnum.SPECIAL_SOLUTIONS, _
                                    '                                                                               resultDataDS.tparVirtualRotorPosititions(0).SolutionCode)

                                    '                If (Not myGlobalDataTo.HasError AndAlso Not myGlobalDataTo.SetDatos Is Nothing) Then
                                    '                    Dim myPreloadedMasterDataDS As PreloadedMasterDataDS = DirectCast(myGlobalDataTo.SetDatos, PreloadedMasterDataDS)

                                    '                    myCellReagentPosInfoRow = myCellPosInfoDS.Reagents.NewReagentsRow()
                                    '                    myCellReagentPosInfoRow.ReagentName = myPreloadedMasterDataDS.tfmwPreloadedMasterData(0).FixedItemDesc
                                    '                    If (Not myRotorContentByPositionDS.twksWSRotorContentByPosition(0).IsRealVolumeNull) Then
                                    '                        myCellReagentPosInfoRow.RealVolume = myRotorContentByPositionDS.twksWSRotorContentByPosition(0).RealVolume
                                    '                    End If
                                    '                    If (Not myRotorContentByPositionDS.twksWSRotorContentByPosition(0).IsRemainingTestsNumberNull) Then
                                    '                        myCellReagentPosInfoRow.RemainingTests = myRotorContentByPositionDS.twksWSRotorContentByPosition(0).RemainingTestsNumber
                                    '                    End If

                                    '                    myCellReagentPosInfoRow.BottleCode = myRotorContentByPositionDS.twksWSRotorContentByPosition(0).TubeType
                                    '                    myCellPosInfoDS.Reagents.Rows.Add(myCellReagentPosInfoRow)
                                    '                End If

                                    '            Case "TUBE_WASH_SOL"
                                    '                myCellPosInfoDS.PositionInformation(0).BeginEdit()
                                    '                myCellPosInfoDS.PositionInformation(0).Content = "TUBE_WASH_SOL"
                                    '                myCellPosInfoDS.PositionInformation(0).SetSolutionCodeNull()
                                    '                If (Not resultDataDS.tparVirtualRotorPosititions(0).IsSolutionCodeNull) Then
                                    '                    myCellPosInfoDS.PositionInformation(0).SolutionCode = resultDataDS.tparVirtualRotorPosititions(0).SolutionCode
                                    '                End If
                                    '                myCellPosInfoDS.PositionInformation(0).EndEdit()

                                    '                Dim myPreloadedMasterDataDelegate As New PreloadedMasterDataDelegate
                                    '                myGlobalDataTo = myPreloadedMasterDataDelegate.GetSubTableItem(dbConnection, GlobalEnumerates.PreloadedMasterDataEnum.WASHING_SOLUTIONS, _
                                    '                                                                               resultDataDS.tparVirtualRotorPosititions(0).SolutionCode)
                                    '                If (Not myGlobalDataTo.HasError AndAlso Not myGlobalDataTo.SetDatos Is Nothing) Then
                                    '                    Dim myPreloadedMasterDataDS As PreloadedMasterDataDS
                                    '                    myPreloadedMasterDataDS = DirectCast(myGlobalDataTo.SetDatos, PreloadedMasterDataDS)

                                    '                    myCellSamplesPosInfoRow = myCellPosInfoDS.Samples.NewSamplesRow()
                                    '                    myCellSamplesPosInfoRow.SampleID = myPreloadedMasterDataDS.tfmwPreloadedMasterData(0).FixedItemDesc
                                    '                    myCellSamplesPosInfoRow.TubeType = myRotorContentByPositionDS.twksWSRotorContentByPosition(0).TubeType
                                    '                    myCellPosInfoDS.Samples.Rows.Add(myCellSamplesPosInfoRow)
                                    '                End If

                                    '            Case "TUBE_SPEC_SOL"
                                    '                myCellPosInfoDS.PositionInformation(0).BeginEdit()
                                    '                myCellPosInfoDS.PositionInformation(0).Content = "TUBE_SPEC_SOL"
                                    '                myCellPosInfoDS.PositionInformation(0).SetSolutionCodeNull()
                                    '                If (Not resultDataDS.tparVirtualRotorPosititions(0).IsSolutionCodeNull) Then
                                    '                    myCellPosInfoDS.PositionInformation(0).SolutionCode = resultDataDS.tparVirtualRotorPosititions(0).SolutionCode
                                    '                End If
                                    '                myCellPosInfoDS.PositionInformation(0).EndEdit()

                                    '                Dim myPreloadedMasterDataDelegate As New PreloadedMasterDataDelegate
                                    '                myGlobalDataTo = myPreloadedMasterDataDelegate.GetSubTableItem(dbConnection, GlobalEnumerates.PreloadedMasterDataEnum.SPECIAL_SOLUTIONS, _
                                    '                                                                               resultDataDS.tparVirtualRotorPosititions(0).SolutionCode)
                                    '                If (Not myGlobalDataTo.HasError AndAlso Not myGlobalDataTo.SetDatos Is Nothing) Then
                                    '                    Dim myPreloadedMasterDataDS As PreloadedMasterDataDS = DirectCast(myGlobalDataTo.SetDatos, PreloadedMasterDataDS)

                                    '                    myCellSamplesPosInfoRow = myCellPosInfoDS.Samples.NewSamplesRow()
                                    '                    myCellSamplesPosInfoRow.SampleID = myPreloadedMasterDataDS.tfmwPreloadedMasterData(0).FixedItemDesc
                                    '                    myCellSamplesPosInfoRow.TubeType = myRotorContentByPositionDS.twksWSRotorContentByPosition(0).TubeType
                                    '                    myCellPosInfoDS.Samples.Rows.Add(myCellSamplesPosInfoRow)
                                    '                End If

                                    '            Case "CTRL"
                                    '                myCellPosInfoDS.PositionInformation(0).BeginEdit()
                                    '                myCellPosInfoDS.PositionInformation(0).Content = "CTRL"
                                    '                myCellPosInfoDS.PositionInformation(0).SetSolutionCodeNull()
                                    '                myCellPosInfoDS.PositionInformation(0).EndEdit()

                                    '                Dim myControlsDelegate As New ControlsDelegate
                                    '                resultData = myControlsDelegate.GetControlData(pDBConnection, resultDataDS.tparVirtualRotorPosititions(0).ControlID)

                                    '                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                    '                    Dim myControlDataTo As ControlsDS = DirectCast(resultData.SetDatos, ControlsDS)

                                    '                    myCellSamplesPosInfoRow = myCellPosInfoDS.Samples.NewSamplesRow()
                                    '                    myCellSamplesPosInfoRow.LotNumber = myControlDataTo.tparControls(0).LotNumber
                                    '                    myCellSamplesPosInfoRow.ExpirationDate = Convert.ToDateTime(myControlDataTo.tparControls(0).ExpirationDate)
                                    '                    myCellSamplesPosInfoRow.SampleID = myControlDataTo.tparControls(0).ControlName
                                    '                    myCellSamplesPosInfoRow.TubeType = myRotorContentByPositionDS.twksWSRotorContentByPosition(0).TubeType
                                    '                    myCellPosInfoDS.Samples.Rows.Add(myCellSamplesPosInfoRow)
                                    '                End If

                                    '            Case "CALIB"
                                    '                myCellPosInfoDS.PositionInformation(0).BeginEdit()
                                    '                myCellPosInfoDS.PositionInformation(0).Content = "CALIB"
                                    '                myCellPosInfoDS.PositionInformation(0).SetSolutionCodeNull()
                                    '                myCellPosInfoDS.PositionInformation(0).EndEdit()

                                    '                Dim myCalibratorDelegate As New CalibratorsDelegate
                                    '                resultData = myCalibratorDelegate.GetCalibratorData(pDBConnection, resultDataDS.tparVirtualRotorPosititions(0).CalibratorID)

                                    '                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                    '                    Dim myCalibratorDataTo As CalibratorsDS = DirectCast(resultData.SetDatos, CalibratorsDS)

                                    '                    myCellSamplesPosInfoRow = myCellPosInfoDS.Samples.NewSamplesRow()
                                    '                    myCellSamplesPosInfoRow.MultiItemNumber = resultDataDS.tparVirtualRotorPosititions(0).MultiItemNumber
                                    '                    myCellSamplesPosInfoRow.LotNumber = myCalibratorDataTo.tparCalibrators(0).LotNumber
                                    '                    myCellSamplesPosInfoRow.ExpirationDate = Convert.ToDateTime(myCalibratorDataTo.tparCalibrators(0).ExpirationDate)
                                    '                    myCellSamplesPosInfoRow.SampleID = myCalibratorDataTo.tparCalibrators(0).CalibratorName
                                    '                    myCellSamplesPosInfoRow.TubeType = myRotorContentByPositionDS.twksWSRotorContentByPosition(0).TubeType
                                    '                    myCellPosInfoDS.Samples.Rows.Add(myCellSamplesPosInfoRow)
                                    '                End If

                                    '            Case "REAGENT"
                                    '                myCellPosInfoDS.PositionInformation(0).BeginEdit()
                                    '                myCellPosInfoDS.PositionInformation(0).Content = "REAGENT"
                                    '                myCellPosInfoDS.PositionInformation(0).SetSolutionCodeNull()
                                    '                myCellPosInfoDS.PositionInformation(0).EndEdit()

                                    '                myCellReagentPosInfoRow = myCellPosInfoDS.Reagents.NewReagentsRow()
                                    '                myCellReagentPosInfoRow.MultiItemNumber = resultDataDS.tparVirtualRotorPosititions(0).MultiItemNumber
                                    '                myCellReagentPosInfoRow.BottleCode = myRotorContentByPositionDS.twksWSRotorContentByPosition(0).TubeType

                                    '                Dim myReagentsDelegate As New ReagentsDelegate
                                    '                If (Not resultDataDS.tparVirtualRotorPosititions(0).IsReagentIDNull) Then
                                    '                    resultData = myReagentsDelegate.GetReagentsData(pDBConnection, resultDataDS.tparVirtualRotorPosititions(0).ReagentID)
                                    '                    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                    '                        Dim myReagentDataTo As ReagentsDS = DirectCast(resultData.SetDatos, ReagentsDS)

                                    '                        If myReagentDataTo.tparReagents.Count > 0 Then 'DL 07/02/2012

                                    '                            myCellReagentPosInfoRow.ReagentName = myReagentDataTo.tparReagents(0).ReagentName

                                    '                            'Get the list of Tests that use the Reagent
                                    '                            Dim myTestReagentDelegate As New TestReagentsDelegate
                                    '                            resultData = myTestReagentDelegate.GetTestReagentsByReagentID(dbConnection, resultDataDS.tparVirtualRotorPosititions(0).ReagentID)

                                    '                            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                    '                                Dim myTestReagentsDS As TestReagentsDS = DirectCast(resultData.SetDatos, TestReagentsDS)

                                    '                                For i As Integer = 0 To myTestReagentsDS.tparTestReagents.Rows.Count - 1
                                    '                                    If (i > 0) Then myCellReagentPosInfoRow.TestList &= ","
                                    '                                    myCellReagentPosInfoRow.TestList &= myTestReagentsDS.tparTestReagents(0).TestName
                                    '                                Next
                                    '                            End If
                                    '                        End If 'DL 02/07/2012
                                    '                    End If
                                    '                End If

                                    '                If (Not myRotorContentByPositionDS.twksWSRotorContentByPosition(0).IsRealVolumeNull) Then
                                    '                    myCellReagentPosInfoRow.RealVolume = myRotorContentByPositionDS.twksWSRotorContentByPosition(0).RealVolume
                                    '                End If
                                    '                If (Not myRotorContentByPositionDS.twksWSRotorContentByPosition(0).IsRemainingTestsNumberNull) Then
                                    '                    myCellReagentPosInfoRow.RemainingTests = myRotorContentByPositionDS.twksWSRotorContentByPosition(0).RemainingTestsNumber
                                    '                End If
                                    '                myCellPosInfoDS.Reagents.Rows.Add(myCellReagentPosInfoRow)

                                    '            Case "PATIENT"
                                    '                myCellPosInfoDS.PositionInformation(0).BeginEdit()
                                    '                myCellPosInfoDS.PositionInformation(0).Content = "PATIENT"
                                    '                myCellPosInfoDS.PositionInformation(0).SetSolutionCodeNull()
                                    '                myCellPosInfoDS.PositionInformation(0).EndEdit()

                                    '                myCellSamplesPosInfoRow = myCellPosInfoDS.Samples.NewSamplesRow()
                                    '                myCellSamplesPosInfoRow.SampleType = resultDataDS.tparVirtualRotorPosititions(0).SampleType
                                    '                myCellSamplesPosInfoRow.TubeType = myRotorContentByPositionDS.twksWSRotorContentByPosition(0).TubeType

                                    '                If (Not resultDataDS.tparVirtualRotorPosititions(0).IsPatientIDNull) Then
                                    '                    myCellSamplesPosInfoRow.SampleID = resultDataDS.tparVirtualRotorPosititions(0).PatientID
                                    '                ElseIf Not resultDataDS.tparVirtualRotorPosititions(0).IsOrderIDNull Then
                                    '                    myCellSamplesPosInfoRow.SampleID = resultDataDS.tparVirtualRotorPosititions(0).OrderID
                                    '                End If

                                    '                If (Not resultDataDS.tparVirtualRotorPosititions(0).IsPredilutionFactorNull) Then
                                    '                    myCellSamplesPosInfoRow.PredilutionFactor = resultDataDS.tparVirtualRotorPosititions(0).PredilutionFactor
                                    '                    myCellSamplesPosInfoRow.Diluted = (resultDataDS.tparVirtualRotorPosititions(0).PredilutionFactor > 0)
                                    '                End If

                                    '                If (Not resultDataDS.tparVirtualRotorPosititions(0).IsOnlyForISENull) Then
                                    '                    If (resultDataDS.tparVirtualRotorPosititions(0).OnlyForISE) Then
                                    '                        myCellSamplesPosInfoRow.SampleType &= " (ISE)"
                                    '                    End If
                                    '                End If
                                    '                myCellPosInfoDS.Samples.Rows.Add(myCellSamplesPosInfoRow)
                                    '        End Select
                                    '    End If
                                    'End If
                                    'End If

                                    'The information is returned
                                    resultData.HasError = False
                                    resultData.SetDatos = myCellPosInfoDS
                                End If
                            End If
                        End If
                    End If
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSRotorContentByPositionDelegate.GetPositionInfo", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get all details of an IN USE Rotor Position (all information to show in Position Info area in Rotor Positions Screen and/or in Monitor Screen) 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pPositionRow">Row of typed DataSet WSRotorContentByPositionDS containing data saved in DB for an specific Rotor Position</param>
        ''' <param name="pCellPosInfoDS">Typed DataSet CellPositionInformationDS to return detailed information about the content of the specified 
        '''                              IN USE Rotor Position</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by: SA 07/01/2015 - BA-1999 (Code extracted from function GetPositionInfo)
        ''' </remarks>
        Private Function GetINUSEPositionInfo(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pPositionRow As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow, _
                                              ByRef pCellPosInfoDS As CellPositionInformationDS) As GlobalDataTO
            Dim resultData As GlobalDataTO
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Get data of the specified Required Work Session Element 
                        Dim myRequiredElements As New WSRequiredElementsDelegate

                        resultData = myRequiredElements.GetRequiredElementInfo(dbConnection, pPositionRow.ElementID)
                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            Dim resultDataDS As WSRequiredElementsTreeDS = DirectCast(resultData.SetDatos, WSRequiredElementsTreeDS)

                            Dim myCellSamplesPosInfoRow As CellPositionInformationDS.SamplesRow
                            Dim myCellReagentPosInfoRow As CellPositionInformationDS.ReagentsRow

                            'By checking which of the tables in the DataSet has rows, the data is obtained from the proper table in the returned 
                            'DataSet and inserted in the proper table in the final DataSet to return
                            If (resultDataDS.Controls.Rows.Count > 0) Then
                                'The TubeContent is informed in table PositionInfo in the DataSet to return
                                pCellPosInfoDS.PositionInformation(0).BeginEdit()
                                pCellPosInfoDS.PositionInformation(0).Content = "CTRL"
                                pCellPosInfoDS.PositionInformation(0).SetSolutionCodeNull()
                                pCellPosInfoDS.PositionInformation(0).EndEdit()

                                'Information is get from Controls table in WSRequiredElementsTreeDS and inserted in Samples Table in CellPositionInformationDS  
                                myCellSamplesPosInfoRow = pCellPosInfoDS.Samples.NewSamplesRow()
                                myCellSamplesPosInfoRow.ElementID = resultDataDS.Controls(0).ElementID
                                myCellSamplesPosInfoRow.SampleID = resultDataDS.Controls(0).ControlName
                                myCellSamplesPosInfoRow.LotNumber = resultDataDS.Controls(0).LotNumber
                                myCellSamplesPosInfoRow.ExpirationDate = Convert.ToDateTime(resultDataDS.Controls(0).ExpirationDate)
                                myCellSamplesPosInfoRow.TubeType = pPositionRow.TubeType
                                pCellPosInfoDS.Samples.Rows.Add(myCellSamplesPosInfoRow)

                            ElseIf (resultDataDS.Calibrators.Rows.Count > 0) Then
                                'The TubeContent is informed in table PositionInfo in the DataSet to return
                                pCellPosInfoDS.PositionInformation(0).BeginEdit()
                                pCellPosInfoDS.PositionInformation(0).Content = "CALIB"
                                pCellPosInfoDS.PositionInformation(0).SetSolutionCodeNull()
                                pCellPosInfoDS.PositionInformation(0).EndEdit()

                                'Information is get from Calibrators table in WSRequiredElementsTreeDS and inserted in Samples Table in CellPositionInformationDS
                                myCellSamplesPosInfoRow = pCellPosInfoDS.Samples.NewSamplesRow()
                                myCellSamplesPosInfoRow.ElementID = resultDataDS.Calibrators(0).ElementID
                                myCellSamplesPosInfoRow.SampleID = resultDataDS.Calibrators(0).CalibratorName
                                myCellSamplesPosInfoRow.MultiItemNumber = resultDataDS.Calibrators(0).CalibratorNumber
                                myCellSamplesPosInfoRow.LotNumber = resultDataDS.Calibrators(0).LotNumber
                                myCellSamplesPosInfoRow.ExpirationDate = Convert.ToDateTime(resultDataDS.Calibrators(0).ExpirationDate)
                                myCellSamplesPosInfoRow.TubeType = pPositionRow.TubeType
                                pCellPosInfoDS.Samples.Rows.Add(myCellSamplesPosInfoRow)

                            ElseIf (resultDataDS.PatientSamples.Rows.Count > 0) Then
                                'The TubeContent is informed in table PositionInfo in the DataSet to return
                                pCellPosInfoDS.PositionInformation(0).BeginEdit()
                                pCellPosInfoDS.PositionInformation(0).Content = "PATIENT"
                                pCellPosInfoDS.PositionInformation(0).SetSolutionCodeNull()
                                pCellPosInfoDS.PositionInformation(0).EndEdit()

                                'Information is get from Patients table in WSRequiredElementsTreeDS and inserted in Samples Table in CellPositionInformationDS.  
                                myCellSamplesPosInfoRow = pCellPosInfoDS.Samples.NewSamplesRow()
                                myCellSamplesPosInfoRow.ElementID = resultDataDS.PatientSamples(0).ElementID

                                If (Not resultDataDS.PatientSamples(0).IsPatientIDNull) Then
                                    myCellSamplesPosInfoRow.SampleID = resultDataDS.PatientSamples(0).PatientID
                                ElseIf Not resultDataDS.PatientSamples(0).IsSampleIDNull Then
                                    myCellSamplesPosInfoRow.SampleID = resultDataDS.PatientSamples(0).SampleID
                                ElseIf Not resultDataDS.PatientSamples(0).IsOrderIDNull Then
                                    myCellSamplesPosInfoRow.SampleID = resultDataDS.PatientSamples(0).OrderID
                                End If

                                myCellSamplesPosInfoRow.SampleType = resultDataDS.PatientSamples(0).SampleType

                                If (Not resultDataDS.PatientSamples(0).IsPredilutionFactorNull) Then
                                    myCellSamplesPosInfoRow.PredilutionFactor = resultDataDS.PatientSamples(0).PredilutionFactor
                                    myCellSamplesPosInfoRow.Diluted = (resultDataDS.PatientSamples(0).PredilutionFactor > 0)
                                End If

                                myCellSamplesPosInfoRow.Stat = resultDataDS.PatientSamples(0).StatFlag
                                myCellSamplesPosInfoRow.TubeType = pPositionRow.TubeType
                                pCellPosInfoDS.Samples.Rows.Add(myCellSamplesPosInfoRow)

                            ElseIf (resultDataDS.Reagents.Rows.Count > 0) Then
                                'The TubeContent is informed in table PositionInfo in the DataSet to return
                                pCellPosInfoDS.PositionInformation(0).BeginEdit()
                                pCellPosInfoDS.PositionInformation(0).Content = "REAGENT"
                                pCellPosInfoDS.PositionInformation(0).SetSolutionCodeNull()
                                pCellPosInfoDS.PositionInformation(0).EndEdit()

                                'Information is get from Reagents table in WSRequiredElementsTreeDS and inserted in Reagents Table in CellPositionInformationDS
                                myCellReagentPosInfoRow = pCellPosInfoDS.Reagents.NewReagentsRow()
                                myCellReagentPosInfoRow.ElementID = resultDataDS.Reagents(0).ElementID
                                myCellReagentPosInfoRow.ReagentName = resultDataDS.Reagents(0).ReagentName
                                myCellReagentPosInfoRow.MultiItemNumber = resultDataDS.Reagents(0).ReagentNumber
                                myCellReagentPosInfoRow.TestList = resultDataDS.Reagents(0).TestName
                                myCellReagentPosInfoRow.BottleCode = pPositionRow.TubeType

                                If (Not resultDataDS.Reagents(0).IsLotNumberNull) Then
                                    myCellReagentPosInfoRow.LotNumber = resultDataDS.Reagents(0).LotNumber.ToString
                                End If

                                If (Not resultDataDS.Reagents(0).IsExpirationDateNull) Then
                                    myCellReagentPosInfoRow.ExpirationDate = Convert.ToDateTime(resultDataDS.Reagents(0).ExpirationDate)
                                End If

                                If (Not pPositionRow.IsRealVolumeNull) Then
                                    myCellReagentPosInfoRow.RealVolume = pPositionRow.RealVolume
                                End If

                                If (Not pPositionRow.IsRemainingTestsNumberNull) Then
                                    myCellReagentPosInfoRow.RemainingTests = pPositionRow.RemainingTestsNumber
                                End If
                                pCellPosInfoDS.Reagents.Rows.Add(myCellReagentPosInfoRow)

                            ElseIf (resultDataDS.AdditionalSolutions.Rows.Count > 0) Then
                                'The TubeContent is informed in table PositionInfo in the DataSet to return
                                pCellPosInfoDS.PositionInformation(0).BeginEdit()
                                pCellPosInfoDS.PositionInformation(0).Content = resultDataDS.AdditionalSolutions(0).TubeContent
                                pCellPosInfoDS.PositionInformation(0).SetSolutionCodeNull()
                                If (Not resultDataDS.AdditionalSolutions(0).IsSolutionCodeNull) Then
                                    pCellPosInfoDS.PositionInformation(0).SolutionCode = resultDataDS.AdditionalSolutions(0).SolutionCode
                                End If
                                pCellPosInfoDS.PositionInformation(0).EndEdit()

                                'Information is get from AdditionalSolutions table in WSRequiredElementsTreeDS and inserted in Reagents Table in CellPositionInformationDS
                                myCellReagentPosInfoRow = pCellPosInfoDS.Reagents.NewReagentsRow()
                                myCellReagentPosInfoRow.ElementID = resultDataDS.AdditionalSolutions(0).ElementID
                                myCellReagentPosInfoRow.ReagentName = resultDataDS.AdditionalSolutions(0).SolutionName
                                myCellReagentPosInfoRow.BottleCode = pPositionRow.TubeType

                                If (Not pPositionRow.IsRealVolumeNull) Then
                                    myCellReagentPosInfoRow.RealVolume = pPositionRow.RealVolume
                                End If
                                myCellReagentPosInfoRow.RemainingTests = 0
                                pCellPosInfoDS.Reagents.Rows.Add(myCellReagentPosInfoRow)

                            ElseIf (resultDataDS.TubeAdditionalSolutions.Rows.Count > 0) Then
                                'The TubeContent is informed in table PositionInfo in the DataSet to return
                                pCellPosInfoDS.PositionInformation(0).BeginEdit()
                                pCellPosInfoDS.PositionInformation(0).Content = resultDataDS.TubeAdditionalSolutions(0).TubeContent
                                pCellPosInfoDS.PositionInformation(0).SetSolutionCodeNull()
                                If (Not resultDataDS.TubeAdditionalSolutions(0).IsSolutionCodeNull) Then
                                    pCellPosInfoDS.PositionInformation(0).SolutionCode = resultDataDS.TubeAdditionalSolutions(0).SolutionCode
                                End If
                                pCellPosInfoDS.PositionInformation(0).EndEdit()

                                'Information is get from AdditionalSolutions table in WSRequiredElementsTreeDS and inserted in Samples Table in CellPositionInformationDS
                                myCellSamplesPosInfoRow = pCellPosInfoDS.Samples.NewSamplesRow()
                                myCellSamplesPosInfoRow.ElementID = resultDataDS.TubeAdditionalSolutions(0).ElementID
                                myCellSamplesPosInfoRow.SampleID = resultDataDS.TubeAdditionalSolutions(0).SolutionName
                                myCellSamplesPosInfoRow.TubeType = pPositionRow.TubeType
                                pCellPosInfoDS.Samples.Rows.Add(myCellSamplesPosInfoRow)
                            End If
                            pCellPosInfoDS.AcceptChanges()
                        End If
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSRotorContentByPositionDelegate.GetINUSEPositionInfo", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get all details of a NOT IN USE Rotor Position (all information to show in Position Info area in Rotor Positions Screen and/or in Monitor Screen) 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pPositionRow">Row of typed DataSet WSRotorContentByPositionDS containing data saved in DB for an specific Rotor Position</param>
        ''' <param name="pCellPosInfoDS">Typed DataSet CellPositionInformationDS to return detailed information about the content of the specified 
        '''                              NOT IN USE Rotor Position</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by: SA 07/01/2015 - BA-1999 (Code extracted from function GetPositionInfo)
        ''' </remarks>
        Private Function GetNOT_INUSEPositionInfo(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pPositionRow As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow, _
                                                  ByRef pCellPosInfoDS As CellPositionInformationDS) As GlobalDataTO
            Dim resultData As GlobalDataTO
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myNoInUsePositions As New WSNotInUseRotorPositionsDelegate
                        resultData = myNoInUsePositions.GetPositionContent(dbConnection, pPositionRow.AnalyzerID, pPositionRow.RotorType, pPositionRow.RingNumber, _
                                                                           pPositionRow.CellNumber, pPositionRow.WorkSessionID)

                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            Dim resultDataDS As VirtualRotorPosititionsDS = DirectCast(resultData.SetDatos, VirtualRotorPosititionsDS)
                            If (resultDataDS.tparVirtualRotorPosititions.Rows.Count > 0) Then
                                Dim myCellSamplesPosInfoRow As CellPositionInformationDS.SamplesRow = Nothing
                                Dim myCellReagentPosInfoRow As CellPositionInformationDS.ReagentsRow = Nothing

                                Select Case (resultDataDS.tparVirtualRotorPosititions.First.TubeContent())
                                    Case "WASH_SOL"
                                        pCellPosInfoDS.PositionInformation(0).BeginEdit()
                                        pCellPosInfoDS.PositionInformation(0).Content = "WASH_SOL"
                                        pCellPosInfoDS.PositionInformation(0).SetSolutionCodeNull()
                                        If (Not resultDataDS.tparVirtualRotorPosititions(0).IsSolutionCodeNull) Then
                                            pCellPosInfoDS.PositionInformation(0).SolutionCode = resultDataDS.tparVirtualRotorPosititions(0).SolutionCode
                                        End If
                                        pCellPosInfoDS.PositionInformation(0).EndEdit()

                                        Dim myPreloadedMasterDataDelegate As New PreloadedMasterDataDelegate
                                        resultData = myPreloadedMasterDataDelegate.GetSubTableItem(dbConnection, GlobalEnumerates.PreloadedMasterDataEnum.WASHING_SOLUTIONS, _
                                                                                                   resultDataDS.tparVirtualRotorPosititions(0).SolutionCode)
                                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                            Dim myPreloadedMasterDataDS As PreloadedMasterDataDS = DirectCast(resultData.SetDatos, PreloadedMasterDataDS)

                                            myCellReagentPosInfoRow = pCellPosInfoDS.Reagents.NewReagentsRow()
                                            myCellReagentPosInfoRow.ReagentName = myPreloadedMasterDataDS.tfmwPreloadedMasterData(0).FixedItemDesc
                                            If (Not pPositionRow.IsRealVolumeNull) Then
                                                myCellReagentPosInfoRow.RealVolume = pPositionRow.RealVolume
                                            End If
                                            If (Not pPositionRow.IsRemainingTestsNumberNull) Then
                                                myCellReagentPosInfoRow.RemainingTests = pPositionRow.RemainingTestsNumber
                                            End If
                                            myCellReagentPosInfoRow.BottleCode = pPositionRow.TubeType
                                            pCellPosInfoDS.Reagents.Rows.Add(myCellReagentPosInfoRow)
                                        End If

                                    Case "SPEC_SOL"
                                        pCellPosInfoDS.PositionInformation(0).BeginEdit()
                                        pCellPosInfoDS.PositionInformation(0).Content = "SPEC_SOL"
                                        pCellPosInfoDS.PositionInformation(0).SetSolutionCodeNull()
                                        If (Not resultDataDS.tparVirtualRotorPosititions(0).IsSolutionCodeNull) Then
                                            pCellPosInfoDS.PositionInformation(0).SolutionCode = resultDataDS.tparVirtualRotorPosititions(0).SolutionCode
                                        End If
                                        pCellPosInfoDS.PositionInformation(0).EndEdit()

                                        Dim myPreloadedMasterDataDelegate As New PreloadedMasterDataDelegate
                                        resultData = myPreloadedMasterDataDelegate.GetSubTableItem(dbConnection, GlobalEnumerates.PreloadedMasterDataEnum.SPECIAL_SOLUTIONS, _
                                                                                                   resultDataDS.tparVirtualRotorPosititions(0).SolutionCode)

                                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                            Dim myPreloadedMasterDataDS As PreloadedMasterDataDS = DirectCast(resultData.SetDatos, PreloadedMasterDataDS)

                                            myCellReagentPosInfoRow = pCellPosInfoDS.Reagents.NewReagentsRow()
                                            myCellReagentPosInfoRow.ReagentName = myPreloadedMasterDataDS.tfmwPreloadedMasterData(0).FixedItemDesc
                                            If (Not pPositionRow.IsRealVolumeNull) Then
                                                myCellReagentPosInfoRow.RealVolume = pPositionRow.RealVolume
                                            End If
                                            If (Not pPositionRow.IsRemainingTestsNumberNull) Then
                                                myCellReagentPosInfoRow.RemainingTests = pPositionRow.RemainingTestsNumber
                                            End If

                                            myCellReagentPosInfoRow.BottleCode = pPositionRow.TubeType
                                            pCellPosInfoDS.Reagents.Rows.Add(myCellReagentPosInfoRow)
                                        End If

                                    Case "TUBE_WASH_SOL"
                                        pCellPosInfoDS.PositionInformation(0).BeginEdit()
                                        pCellPosInfoDS.PositionInformation(0).Content = "TUBE_WASH_SOL"
                                        pCellPosInfoDS.PositionInformation(0).SetSolutionCodeNull()
                                        If (Not resultDataDS.tparVirtualRotorPosititions(0).IsSolutionCodeNull) Then
                                            pCellPosInfoDS.PositionInformation(0).SolutionCode = resultDataDS.tparVirtualRotorPosititions(0).SolutionCode
                                        End If
                                        pCellPosInfoDS.PositionInformation(0).EndEdit()

                                        Dim myPreloadedMasterDataDelegate As New PreloadedMasterDataDelegate
                                        resultData = myPreloadedMasterDataDelegate.GetSubTableItem(dbConnection, GlobalEnumerates.PreloadedMasterDataEnum.WASHING_SOLUTIONS, _
                                                                                                   resultDataDS.tparVirtualRotorPosititions(0).SolutionCode)
                                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                            Dim myPreloadedMasterDataDS As PreloadedMasterDataDS
                                            myPreloadedMasterDataDS = DirectCast(resultData.SetDatos, PreloadedMasterDataDS)

                                            myCellSamplesPosInfoRow = pCellPosInfoDS.Samples.NewSamplesRow()
                                            myCellSamplesPosInfoRow.SampleID = myPreloadedMasterDataDS.tfmwPreloadedMasterData(0).FixedItemDesc
                                            myCellSamplesPosInfoRow.TubeType = pPositionRow.TubeType
                                            pCellPosInfoDS.Samples.Rows.Add(myCellSamplesPosInfoRow)
                                        End If

                                    Case "TUBE_SPEC_SOL"
                                        pCellPosInfoDS.PositionInformation(0).BeginEdit()
                                        pCellPosInfoDS.PositionInformation(0).Content = "TUBE_SPEC_SOL"
                                        pCellPosInfoDS.PositionInformation(0).SetSolutionCodeNull()
                                        If (Not resultDataDS.tparVirtualRotorPosititions(0).IsSolutionCodeNull) Then
                                            pCellPosInfoDS.PositionInformation(0).SolutionCode = resultDataDS.tparVirtualRotorPosititions(0).SolutionCode
                                        End If
                                        pCellPosInfoDS.PositionInformation(0).EndEdit()

                                        Dim myPreloadedMasterDataDelegate As New PreloadedMasterDataDelegate
                                        resultData = myPreloadedMasterDataDelegate.GetSubTableItem(dbConnection, GlobalEnumerates.PreloadedMasterDataEnum.SPECIAL_SOLUTIONS, _
                                                                                                   resultDataDS.tparVirtualRotorPosititions(0).SolutionCode)
                                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                            Dim myPreloadedMasterDataDS As PreloadedMasterDataDS = DirectCast(resultData.SetDatos, PreloadedMasterDataDS)

                                            myCellSamplesPosInfoRow = pCellPosInfoDS.Samples.NewSamplesRow()
                                            myCellSamplesPosInfoRow.SampleID = myPreloadedMasterDataDS.tfmwPreloadedMasterData(0).FixedItemDesc
                                            myCellSamplesPosInfoRow.TubeType = pPositionRow.TubeType
                                            pCellPosInfoDS.Samples.Rows.Add(myCellSamplesPosInfoRow)
                                        End If

                                    Case "CTRL"
                                        pCellPosInfoDS.PositionInformation(0).BeginEdit()
                                        pCellPosInfoDS.PositionInformation(0).Content = "CTRL"
                                        pCellPosInfoDS.PositionInformation(0).SetSolutionCodeNull()
                                        pCellPosInfoDS.PositionInformation(0).EndEdit()

                                        Dim myControlsDelegate As New ControlsDelegate
                                        resultData = myControlsDelegate.GetControlData(pDBConnection, resultDataDS.tparVirtualRotorPosititions(0).ControlID)

                                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                            Dim myControlDataTo As ControlsDS = DirectCast(resultData.SetDatos, ControlsDS)

                                            myCellSamplesPosInfoRow = pCellPosInfoDS.Samples.NewSamplesRow()
                                            myCellSamplesPosInfoRow.LotNumber = myControlDataTo.tparControls(0).LotNumber
                                            myCellSamplesPosInfoRow.ExpirationDate = Convert.ToDateTime(myControlDataTo.tparControls(0).ExpirationDate)
                                            myCellSamplesPosInfoRow.SampleID = myControlDataTo.tparControls(0).ControlName
                                            myCellSamplesPosInfoRow.TubeType = pPositionRow.TubeType
                                            pCellPosInfoDS.Samples.Rows.Add(myCellSamplesPosInfoRow)
                                        End If

                                    Case "CALIB"
                                        pCellPosInfoDS.PositionInformation(0).BeginEdit()
                                        pCellPosInfoDS.PositionInformation(0).Content = "CALIB"
                                        pCellPosInfoDS.PositionInformation(0).SetSolutionCodeNull()
                                        pCellPosInfoDS.PositionInformation(0).EndEdit()

                                        Dim myCalibratorDelegate As New CalibratorsDelegate
                                        resultData = myCalibratorDelegate.GetCalibratorData(pDBConnection, resultDataDS.tparVirtualRotorPosititions(0).CalibratorID)

                                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                            Dim myCalibratorDataTo As CalibratorsDS = DirectCast(resultData.SetDatos, CalibratorsDS)

                                            myCellSamplesPosInfoRow = pCellPosInfoDS.Samples.NewSamplesRow()
                                            myCellSamplesPosInfoRow.MultiItemNumber = resultDataDS.tparVirtualRotorPosititions(0).MultiItemNumber
                                            myCellSamplesPosInfoRow.LotNumber = myCalibratorDataTo.tparCalibrators(0).LotNumber
                                            myCellSamplesPosInfoRow.ExpirationDate = Convert.ToDateTime(myCalibratorDataTo.tparCalibrators(0).ExpirationDate)
                                            myCellSamplesPosInfoRow.SampleID = myCalibratorDataTo.tparCalibrators(0).CalibratorName
                                            myCellSamplesPosInfoRow.TubeType = pPositionRow.TubeType
                                            pCellPosInfoDS.Samples.Rows.Add(myCellSamplesPosInfoRow)
                                        End If

                                    Case "REAGENT"
                                        pCellPosInfoDS.PositionInformation(0).BeginEdit()
                                        pCellPosInfoDS.PositionInformation(0).Content = "REAGENT"
                                        pCellPosInfoDS.PositionInformation(0).SetSolutionCodeNull()
                                        pCellPosInfoDS.PositionInformation(0).EndEdit()

                                        myCellReagentPosInfoRow = pCellPosInfoDS.Reagents.NewReagentsRow()
                                        myCellReagentPosInfoRow.MultiItemNumber = resultDataDS.tparVirtualRotorPosititions(0).MultiItemNumber
                                        myCellReagentPosInfoRow.BottleCode = pPositionRow.TubeType

                                        Dim myReagentsDelegate As New ReagentsDelegate
                                        If (Not resultDataDS.tparVirtualRotorPosititions(0).IsReagentIDNull) Then
                                            resultData = myReagentsDelegate.GetReagentsData(pDBConnection, resultDataDS.tparVirtualRotorPosititions(0).ReagentID)
                                            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                                Dim myReagentDataTo As ReagentsDS = DirectCast(resultData.SetDatos, ReagentsDS)

                                                If (myReagentDataTo.tparReagents.Rows.Count > 0) Then
                                                    myCellReagentPosInfoRow.ReagentName = myReagentDataTo.tparReagents(0).ReagentName

                                                    'Get the list of Tests that use the Reagent
                                                    Dim myTestReagentDelegate As New TestReagentsDelegate
                                                    resultData = myTestReagentDelegate.GetTestReagentsByReagentID(dbConnection, resultDataDS.tparVirtualRotorPosititions(0).ReagentID)

                                                    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                                        Dim myTestReagentsDS As TestReagentsDS = DirectCast(resultData.SetDatos, TestReagentsDS)

                                                        For i As Integer = 0 To myTestReagentsDS.tparTestReagents.Rows.Count - 1
                                                            If (i > 0) Then myCellReagentPosInfoRow.TestList &= ","
                                                            myCellReagentPosInfoRow.TestList &= myTestReagentsDS.tparTestReagents(0).TestName
                                                        Next
                                                    End If
                                                End If
                                            End If
                                        End If

                                        If (Not pPositionRow.IsRealVolumeNull) Then
                                            myCellReagentPosInfoRow.RealVolume = pPositionRow.RealVolume

                                            If (Not pPositionRow.IsRemainingTestsNumberNull) Then
                                                myCellReagentPosInfoRow.RemainingTests = pPositionRow.RemainingTestsNumber
                                            Else
                                                'Calculate the Number of Tests that can be executed with the available volume 
                                                resultData = CalculateRemainingTestNotInUseReagent(dbConnection, pPositionRow.WorkSessionID, resultDataDS.tparVirtualRotorPosititions(0).ReagentID, _
                                                                                                   pPositionRow.MultiTubeNumber, pPositionRow.RealVolume, pPositionRow.TubeType)

                                                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                                    myCellReagentPosInfoRow.RemainingTests = CInt(resultData.SetDatos)
                                                End If
                                            End If
                                        End If
                                        pCellPosInfoDS.Reagents.Rows.Add(myCellReagentPosInfoRow)

                                    Case "PATIENT"
                                        pCellPosInfoDS.PositionInformation(0).BeginEdit()
                                        pCellPosInfoDS.PositionInformation(0).Content = "PATIENT"
                                        pCellPosInfoDS.PositionInformation(0).SetSolutionCodeNull()
                                        pCellPosInfoDS.PositionInformation(0).EndEdit()

                                        myCellSamplesPosInfoRow = pCellPosInfoDS.Samples.NewSamplesRow()
                                        myCellSamplesPosInfoRow.SampleType = resultDataDS.tparVirtualRotorPosititions(0).SampleType
                                        myCellSamplesPosInfoRow.TubeType = pPositionRow.TubeType

                                        If (Not resultDataDS.tparVirtualRotorPosititions(0).IsPatientIDNull) Then
                                            myCellSamplesPosInfoRow.SampleID = resultDataDS.tparVirtualRotorPosititions(0).PatientID
                                        ElseIf Not resultDataDS.tparVirtualRotorPosititions(0).IsOrderIDNull Then
                                            myCellSamplesPosInfoRow.SampleID = resultDataDS.tparVirtualRotorPosititions(0).OrderID
                                        End If

                                        If (Not resultDataDS.tparVirtualRotorPosititions(0).IsPredilutionFactorNull) Then
                                            myCellSamplesPosInfoRow.PredilutionFactor = resultDataDS.tparVirtualRotorPosititions(0).PredilutionFactor
                                            myCellSamplesPosInfoRow.Diluted = (resultDataDS.tparVirtualRotorPosititions(0).PredilutionFactor > 0)
                                        End If

                                        If (Not resultDataDS.tparVirtualRotorPosititions(0).IsOnlyForISENull) Then
                                            If (resultDataDS.tparVirtualRotorPosititions(0).OnlyForISE) Then
                                                myCellSamplesPosInfoRow.SampleType &= " (ISE)"
                                            End If
                                        End If
                                        pCellPosInfoDS.Samples.Rows.Add(myCellSamplesPosInfoRow)
                                End Select
                                pCellPosInfoDS.AcceptChanges()
                            End If
                        End If
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSRotorContentByPositionDelegate.GetNOT_INUSEPositionInfo", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Calculate the number of remaining Tests that can be executed with the volume currently placed in the Analyzer Rotor 
        ''' for the specified Reagent (when the Reagent Status is NOT_INUSE)
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by:  TR 15/11/2013 - BA-1383
        ''' Modified by: SA 22/12/2014 - BA-1999 ==> Function changed from Private to Public to allow call it when the Reagents Rotor is scanned. 
        ''' </remarks>
        Public Function CalculateRemainingTestNotInUseReagent(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSession As String, _
                                                               ByVal pReagentID As Integer, pReagentNumber As Integer, ByVal pRealBottleVolume As Single, _
                                                               ByVal pBottleType As String) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Dim dbConnection As SqlClient.SqlConnection = Nothing
            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myTestID As Integer = 0
                        Dim numRemainingTests As Integer = 0
                        Dim myTestReagentVolume As Single = 0

                        Dim myTestDelegate As New TestsDelegate
                        Dim myTestReagentDelegate As New TestReagentsDelegate
                        Dim myTestReagentVolumeDelegate As New TestReagentsVolumeDelegate

                        Dim myTestReagentsDS As New TestReagentsDS
                        Dim mytestReagentsVolumeDS As New TestReagentsVolumesDS

                        'Get the Test that use the Reagent and if it is used as first or second Reagent to set the Volume
                        myGlobalDataTO = myTestReagentDelegate.GetTestReagentsByReagentID(dbConnection, pReagentID)
                        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                            myTestReagentsDS = DirectCast(myGlobalDataTO.SetDatos, TestReagentsDS)

                            If (myTestReagentsDS.tparTestReagents.Count > 0) Then
                                'Get TestID and ReagentNumber 
                                myTestID = myTestReagentsDS.tparTestReagents(0).TestID
                                pReagentNumber = myTestReagentsDS.tparTestReagents(0).ReagentNumber
                            End If
                        End If

                        ''Get the required volume for this Test
                        If (myTestID > 0) Then
                            myGlobalDataTO = myTestReagentVolumeDelegate.GetReagentsVolumesByTestID(dbConnection, myTestID)
                            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                'Get the required volume depending on if the Test uses the Reagent as first or second
                                mytestReagentsVolumeDS = DirectCast(myGlobalDataTO.SetDatos, TestReagentsVolumesDS)

                                If (mytestReagentsVolumeDS.tparTestReagentsVolumes.Where(Function(a) a.ReagentNumber = pReagentNumber).Count > 0) Then
                                    myTestReagentVolume = mytestReagentsVolumeDS.tparTestReagentsVolumes.Where(Function(a) a.ReagentNumber = pReagentNumber).First().ReagentVolume
                                End If
                            End If
                        End If

                        'Get the Section defined for the Bottle according its size 
                        Dim reagentBottles As New ReagentTubeTypesDelegate
                        myGlobalDataTO = reagentBottles.GetVolumeByTubeType(dbConnection, pBottleType)

                        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                            Dim reagentBottlesDS As ReagentTubeTypesDS = DirectCast(myGlobalDataTO.SetDatos, ReagentTubeTypesDS)

                            If (reagentBottlesDS.ReagentTubeTypes.Rows.Count = 1) Then
                                'Calculate the death volume for the bottle according its size
                                myGlobalDataTO = reagentBottles.CalculateDeathVolByBottleType(dbConnection, pBottleType, reagentBottlesDS.ReagentTubeTypes.First.Section)
                                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                    Dim deathBottleVol As Single = CType(myGlobalDataTO.SetDatos, Single)

                                    'Finally, calculate the number of Tests that can be executed with the remaining volume (excluding the death volume)
                                    If ((pRealBottleVolume - deathBottleVol) < 0) Then
                                        numRemainingTests = 0
                                    Else
                                        Dim preparationVolume As Single = CType((myTestReagentVolume) / 1000, Single)
                                        If (preparationVolume > 0) Then
                                            numRemainingTests = CType(Math.Truncate((pRealBottleVolume - deathBottleVol) / preparationVolume), Integer)
                                        End If
                                    End If
                                End If
                            End If
                        End If

                        'Set the result value to the GlobalDataTO 
                        myGlobalDataTO.SetDatos = numRemainingTests
                        myGlobalDataTO.HasError = False
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSRequiredElementsDelegate.CalculateRemainingTestNotInUseReagent", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        '''<summary>
        ''' Get the info for the report as the 'GetPositionInfo' method way
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pRotorPositionDS">Typed DataSet containing information of the Rotor Positions for 
        '''                                which detailed information is consulted</param>
        ''' <returns>GlobalDataTO containing a typed DataSet CellPositionInformationDS with the detailed 
        '''          information of the content of a Rotor Position</returns>
        ''' <remarks>
        ''' Created by:  JV 14/11/2013 - BT #1382 
        ''' Modified by: JV 28/11/2013 - BT #1412 ==> Update with the changes done in 'GetPositionInfo'. Also establish a relation (not existent) between Reagents 
        '''                                           table and PositionInformation (CellNumber) in CellPositionInformationDS
        ''' </remarks>
        Public Function GetPositionInfoForReport(ByVal pDBConnection As SqlClient.SqlConnection, _
                                        ByVal pRotorPositionDS As WSRotorContentByPositionDS) As GlobalDataTO
            Dim resultData As GlobalDataTO
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        If (Not pRotorPositionDS Is Nothing) Then
                            If (pRotorPositionDS.Tables(0).Rows.Count > 0) Then
                                Dim myGlobalDataTo As GlobalDataTO
                                Dim myCellPosInfoDS As New CellPositionInformationDS

                                'Get all data of the specified Rotor Position
                                Dim myRotorContentPosition As New twksWSRotorContentByPositionDAO
                                For Each pos As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow In pRotorPositionDS.Tables(pRotorPositionDS.twksWSRotorContentByPosition.TableName).Rows
                                    myGlobalDataTo = myRotorContentPosition.Read(dbConnection, _
                                                                             pos.AnalyzerID, _
                                                                             pos.WorkSessionID, _
                                                                             pos.RotorType, _
                                                                             pos.CellNumber, _
                                                                             pos.RingNumber)

                                    If (Not myGlobalDataTo.HasError AndAlso Not myGlobalDataTo.SetDatos Is Nothing) Then
                                        Dim myRotorContentByPositionDS As WSRotorContentByPositionDS
                                        myRotorContentByPositionDS = DirectCast(myGlobalDataTo.SetDatos, WSRotorContentByPositionDS)

                                        If (myRotorContentByPositionDS.twksWSRotorContentByPosition.Rows.Count > 0) Then

                                            'TR 14/11/2013 -BT #1383 Get all the Reagents not in use and complete the remaining test.
                                            If Not myGlobalDataTo.HasError Then
                                                Dim myWSRotorContentByPositionDS As New WSRotorContentByPositionDS
                                                Dim myListNotInUse As New List(Of WSRotorContentByPositionDS.twksWSRotorContentByPositionRow)
                                                myWSRotorContentByPositionDS = DirectCast(myGlobalDataTo.SetDatos, WSRotorContentByPositionDS)
                                                'Get all the elements with status  NOT_INUSE
                                                myListNotInUse = (From a In myWSRotorContentByPositionDS.twksWSRotorContentByPosition
                                                                  Where a.Status = "NO_INUSE" Select a).ToList()
                                                For Each NotInUseElement As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow In myListNotInUse
                                                    'Validate the Reagent id and the real volume are not null values to avoid exception error.
                                                    If Not pos.IsReagentIDNull AndAlso _
                                                        Not pos.IsRealVolumeNull Then

                                                        'calculate the remaining test number.
                                                        myGlobalDataTo = CalculateRemainingTestNotInUseReagent(dbConnection, NotInUseElement.WorkSessionID, _
                                                                                              pos.ReagentID, _
                                                                                              NotInUseElement.MultiTubeNumber, pos.RealVolume, _
                                                                                              NotInUseElement.TubeType)

                                                        If Not myGlobalDataTo.HasError Then
                                                            NotInUseElement.RemainingTestsNumber = CInt(myGlobalDataTo.SetDatos)
                                                        End If
                                                    End If
                                                Next
                                            End If
                                            'TR 14/11/2013 -END.

                                            'Fill fields in table PositionInformation 
                                            Dim myCellPosInfoRow As CellPositionInformationDS.PositionInformationRow
                                            myCellPosInfoRow = myCellPosInfoDS.PositionInformation.NewPositionInformationRow()

                                            myCellPosInfoRow.RingNumber = myRotorContentByPositionDS.twksWSRotorContentByPosition(0).RingNumber
                                            myCellPosInfoRow.CellNumber = myRotorContentByPositionDS.twksWSRotorContentByPosition(0).CellNumber
                                            myCellPosInfoRow.Status = myRotorContentByPositionDS.twksWSRotorContentByPosition(0).Status.ToString
                                            myCellPosInfoRow.BarcodeInfo = myRotorContentByPositionDS.twksWSRotorContentByPosition(0).BarCodeInfo.ToString
                                            myCellPosInfoRow.BarcodeStatus = myRotorContentByPositionDS.twksWSRotorContentByPosition(0).BarcodeStatus.ToString
                                            If (Not myRotorContentByPositionDS.twksWSRotorContentByPosition(0).IsElementIDNull) Then
                                                myCellPosInfoRow.ElementID = myRotorContentByPositionDS.twksWSRotorContentByPosition(0).ElementID
                                            End If

                                            myCellPosInfoDS.PositionInformation.Rows.Add(myCellPosInfoRow)
                                        End If

                                        If (myRotorContentByPositionDS.twksWSRotorContentByPosition.Rows.Count > 0) AndAlso (Not myRotorContentByPositionDS.twksWSRotorContentByPosition(0).IsElementIDNull) Then
                                            'Get data of the specified Required Work Session Element 
                                            'myGlobalDataTo = New GlobalDataTO
                                            Dim myRequiredElements As New WSRequiredElementsDelegate
                                            myGlobalDataTo = myRequiredElements.GetRequiredElementInfo(dbConnection, _
                                                                                                       myRotorContentByPositionDS.twksWSRotorContentByPosition(0).ElementID)

                                            If (Not myGlobalDataTo.HasError AndAlso Not myGlobalDataTo.SetDatos Is Nothing) Then
                                                Dim resultDataDS As WSRequiredElementsTreeDS
                                                resultDataDS = DirectCast(myGlobalDataTo.SetDatos, WSRequiredElementsTreeDS)

                                                Dim myCellSamplesPosInfoRow As CellPositionInformationDS.SamplesRow
                                                Dim myCellReagentPosInfoRow As CellPositionInformationDS.ReagentsRow

                                                'By checking which of the tables in the DataSet has rows, the data is obtained from the proper
                                                'table in the returned DataSet and inserted in the proper table in the final DataSet to return
                                                If (resultDataDS.Controls.Rows.Count > 0) Then
                                                    'The TubeContent is informed in table PositionInfo in the DataSet to return
                                                    myCellPosInfoDS.PositionInformation(0).BeginEdit()
                                                    myCellPosInfoDS.PositionInformation(0).Content = "CTRL"
                                                    myCellPosInfoDS.PositionInformation(0).SetSolutionCodeNull() 'AG 11/01/2012
                                                    myCellPosInfoDS.PositionInformation(0).EndEdit()

                                                    'Information is get from Controls table in WSRequiredElementsTreeDS and 
                                                    'inserted in Samples Table in CellPositionInformationDS.  Value of field 
                                                    'TubeType is in the DataSet with position information (myRotorContentByPositionDS)
                                                    myCellSamplesPosInfoRow = myCellPosInfoDS.Samples.NewSamplesRow()
                                                    myCellSamplesPosInfoRow.ElementID = resultDataDS.Controls(0).ElementID
                                                    myCellSamplesPosInfoRow.SampleID = resultDataDS.Controls(0).ControlName
                                                    myCellSamplesPosInfoRow.LotNumber = resultDataDS.Controls(0).LotNumber
                                                    myCellSamplesPosInfoRow.ExpirationDate = Convert.ToDateTime(resultDataDS.Controls(0).ExpirationDate)
                                                    myCellSamplesPosInfoRow.TubeType = myRotorContentByPositionDS.twksWSRotorContentByPosition(0).TubeType
                                                    myCellPosInfoDS.Samples.Rows.Add(myCellSamplesPosInfoRow)

                                                ElseIf (resultDataDS.Calibrators.Rows.Count > 0) Then
                                                    'The TubeContent is informed in table PositionInfo in the DataSet to return
                                                    myCellPosInfoDS.PositionInformation(0).BeginEdit()
                                                    myCellPosInfoDS.PositionInformation(0).Content = "CALIB"
                                                    myCellPosInfoDS.PositionInformation(0).SetSolutionCodeNull() 'AG 11/01/2012
                                                    myCellPosInfoDS.PositionInformation(0).EndEdit()

                                                    'Information is get from Calibrators table in WSRequiredElementsTreeDS and 
                                                    'inserted in Samples Table in CellPositionInformationDS.  Value of field 
                                                    'TubeType is in the DataSet with position information (myRotorContentByPositionDS)
                                                    myCellSamplesPosInfoRow = myCellPosInfoDS.Samples.NewSamplesRow()
                                                    myCellSamplesPosInfoRow.ElementID = resultDataDS.Calibrators(0).ElementID
                                                    myCellSamplesPosInfoRow.SampleID = resultDataDS.Calibrators(0).CalibratorName
                                                    myCellSamplesPosInfoRow.MultiItemNumber = resultDataDS.Calibrators(0).CalibratorNumber
                                                    myCellSamplesPosInfoRow.LotNumber = resultDataDS.Calibrators(0).LotNumber
                                                    myCellSamplesPosInfoRow.ExpirationDate = Convert.ToDateTime(resultDataDS.Calibrators(0).ExpirationDate)
                                                    myCellSamplesPosInfoRow.TubeType = myRotorContentByPositionDS.twksWSRotorContentByPosition(0).TubeType
                                                    myCellPosInfoDS.Samples.Rows.Add(myCellSamplesPosInfoRow)

                                                ElseIf (resultDataDS.PatientSamples.Rows.Count > 0) Then
                                                    'The TubeContent is informed in table PositionInfo in the DataSet to return
                                                    myCellPosInfoDS.PositionInformation(0).BeginEdit()
                                                    myCellPosInfoDS.PositionInformation(0).Content = "PATIENT"
                                                    myCellPosInfoDS.PositionInformation(0).SetSolutionCodeNull() 'AG 11/01/2012
                                                    myCellPosInfoDS.PositionInformation(0).EndEdit()

                                                    'Information is get from Patients table in WSRequiredElementsTreeDS and 
                                                    'inserted in Samples Table in CellPositionInformationDS.  Value of field 
                                                    'TubeType is in the DataSet with position information (myRotorContentByPositionDS)
                                                    myCellSamplesPosInfoRow = myCellPosInfoDS.Samples.NewSamplesRow()
                                                    myCellSamplesPosInfoRow.ElementID = resultDataDS.PatientSamples(0).ElementID

                                                    If (Not resultDataDS.PatientSamples(0).IsPatientIDNull) Then
                                                        myCellSamplesPosInfoRow.SampleID = resultDataDS.PatientSamples(0).PatientID
                                                    ElseIf Not resultDataDS.PatientSamples(0).IsSampleIDNull Then
                                                        myCellSamplesPosInfoRow.SampleID = resultDataDS.PatientSamples(0).SampleID
                                                    ElseIf Not resultDataDS.PatientSamples(0).IsOrderIDNull Then
                                                        myCellSamplesPosInfoRow.SampleID = resultDataDS.PatientSamples(0).OrderID
                                                    End If

                                                    myCellSamplesPosInfoRow.SampleType = resultDataDS.PatientSamples(0).SampleType

                                                    If (Not resultDataDS.PatientSamples(0).IsPredilutionFactorNull) Then
                                                        myCellSamplesPosInfoRow.PredilutionFactor = resultDataDS.PatientSamples(0).PredilutionFactor
                                                        myCellSamplesPosInfoRow.Diluted = (resultDataDS.PatientSamples(0).PredilutionFactor > 0)
                                                    End If

                                                    myCellSamplesPosInfoRow.Stat = resultDataDS.PatientSamples(0).StatFlag
                                                    myCellSamplesPosInfoRow.TubeType = myRotorContentByPositionDS.twksWSRotorContentByPosition(0).TubeType
                                                    myCellPosInfoDS.Samples.Rows.Add(myCellSamplesPosInfoRow)

                                                ElseIf (resultDataDS.Reagents.Rows.Count > 0) Then
                                                    'The TubeContent is informed in table PositionInfo in the DataSet to return
                                                    myCellPosInfoDS.PositionInformation(0).BeginEdit()
                                                    myCellPosInfoDS.PositionInformation(0).Content = "REAGENT"
                                                    myCellPosInfoDS.PositionInformation(0).SetSolutionCodeNull() 'AG 11/01/2012
                                                    myCellPosInfoDS.PositionInformation(0).EndEdit()

                                                    'Information is get from Reagents table in WSRequiredElementsTreeDS and 
                                                    'inserted in Reagents Table in CellPositionInformationDS.  Value of field 
                                                    'BottleCode is in the DataSet with position information (myRotorContentByPositionDS)
                                                    myCellReagentPosInfoRow = myCellPosInfoDS.Reagents.NewReagentsRow()
                                                    myCellReagentPosInfoRow.CellNumber = myRotorContentByPositionDS.twksWSRotorContentByPosition(0).CellNumber
                                                    myCellReagentPosInfoRow.ElementID = resultDataDS.Reagents(0).ElementID
                                                    myCellReagentPosInfoRow.ReagentName = resultDataDS.Reagents(0).ReagentName
                                                    myCellReagentPosInfoRow.MultiItemNumber = resultDataDS.Reagents(0).ReagentNumber
                                                    myCellReagentPosInfoRow.TestList = resultDataDS.Reagents(0).TestName
                                                    myCellReagentPosInfoRow.BottleCode = myRotorContentByPositionDS.twksWSRotorContentByPosition(0).TubeType

                                                    If (Not resultDataDS.Reagents(0).IsLotNumberNull) Then
                                                        myCellReagentPosInfoRow.LotNumber = resultDataDS.Reagents(0).LotNumber.ToString
                                                    End If

                                                    If (Not resultDataDS.Reagents(0).IsExpirationDateNull) Then
                                                        myCellReagentPosInfoRow.ExpirationDate = Convert.ToDateTime(resultDataDS.Reagents(0).ExpirationDate)
                                                    End If

                                                    If (Not myRotorContentByPositionDS.twksWSRotorContentByPosition(0).IsRealVolumeNull) Then
                                                        myCellReagentPosInfoRow.RealVolume = myRotorContentByPositionDS.twksWSRotorContentByPosition(0).RealVolume
                                                    End If
                                                    'TR 14/11/2013 -BT #1383
                                                    If (Not myRotorContentByPositionDS.twksWSRotorContentByPosition(0).IsRemainingTestsNumberNull) Then
                                                        myCellReagentPosInfoRow.RemainingTests = myRotorContentByPositionDS.twksWSRotorContentByPosition(0).RemainingTestsNumber
                                                    End If
                                                    'TR 14/11/2013 -BT #1383 -END.
                                                    If (Not myRotorContentByPositionDS.twksWSRotorContentByPosition(0).IsRemainingTestsNumberNull) Then
                                                        myCellReagentPosInfoRow.RemainingTests = myRotorContentByPositionDS.twksWSRotorContentByPosition(0).RemainingTestsNumber
                                                    End If
                                                    myCellPosInfoDS.Reagents.Rows.Add(myCellReagentPosInfoRow)

                                                ElseIf (resultDataDS.AdditionalSolutions.Rows.Count > 0) Then
                                                    'The TubeContent is informed in table PositionInfo in the DataSet to return
                                                    myCellPosInfoDS.PositionInformation(0).BeginEdit()
                                                    myCellPosInfoDS.PositionInformation(0).Content = resultDataDS.AdditionalSolutions(0).TubeContent
                                                    myCellPosInfoDS.PositionInformation(0).SetSolutionCodeNull()
                                                    If (Not resultDataDS.AdditionalSolutions(0).IsSolutionCodeNull) Then
                                                        myCellPosInfoDS.PositionInformation(0).SolutionCode = resultDataDS.AdditionalSolutions(0).SolutionCode
                                                    End If
                                                    myCellPosInfoDS.PositionInformation(0).EndEdit()

                                                    'Information is get from AdditionalSolutions table in WSRequiredElementsTreeDS and 
                                                    'inserted in Reagents Table in CellPositionInformationDS.  Value of field 
                                                    'BottleCode is in the DataSet with position information (myRotorContentByPositionDS)
                                                    myCellReagentPosInfoRow = myCellPosInfoDS.Reagents.NewReagentsRow()
                                                    myCellReagentPosInfoRow.CellNumber = myRotorContentByPositionDS.twksWSRotorContentByPosition(0).CellNumber
                                                    myCellReagentPosInfoRow.ElementID = resultDataDS.AdditionalSolutions(0).ElementID
                                                    myCellReagentPosInfoRow.ReagentName = resultDataDS.AdditionalSolutions(0).SolutionName
                                                    myCellReagentPosInfoRow.BottleCode = myRotorContentByPositionDS.twksWSRotorContentByPosition(0).TubeType

                                                    If (Not myRotorContentByPositionDS.twksWSRotorContentByPosition(0).IsRealVolumeNull) Then
                                                        myCellReagentPosInfoRow.RealVolume = myRotorContentByPositionDS.twksWSRotorContentByPosition(0).RealVolume
                                                    End If
                                                    If (Not myRotorContentByPositionDS.twksWSRotorContentByPosition(0).IsRemainingTestsNumberNull) Then
                                                        myCellReagentPosInfoRow.RemainingTests = myRotorContentByPositionDS.twksWSRotorContentByPosition(0).RemainingTestsNumber
                                                    End If
                                                    myCellPosInfoDS.Reagents.Rows.Add(myCellReagentPosInfoRow)

                                                ElseIf (resultDataDS.TubeAdditionalSolutions.Rows.Count > 0) Then
                                                    'The TubeContent is informed in table PositionInfo in the DataSet to return
                                                    myCellPosInfoDS.PositionInformation(0).BeginEdit()
                                                    myCellPosInfoDS.PositionInformation(0).Content = resultDataDS.TubeAdditionalSolutions(0).TubeContent
                                                    myCellPosInfoDS.PositionInformation(0).SetSolutionCodeNull()
                                                    If (Not resultDataDS.TubeAdditionalSolutions(0).IsSolutionCodeNull) Then
                                                        myCellPosInfoDS.PositionInformation(0).SolutionCode = resultDataDS.TubeAdditionalSolutions(0).SolutionCode
                                                    End If
                                                    myCellPosInfoDS.PositionInformation(0).EndEdit()

                                                    'Information is get from AdditionalSolutions table in WSRequiredElementsTreeDS and 
                                                    'inserted in Samples Table in CellPositionInformationDS.  Value of field 
                                                    'TubeType is in the DataSet with position information (myRotorContentByPositionDS)
                                                    myCellSamplesPosInfoRow = myCellPosInfoDS.Samples.NewSamplesRow()
                                                    myCellSamplesPosInfoRow.ElementID = resultDataDS.TubeAdditionalSolutions(0).ElementID
                                                    myCellSamplesPosInfoRow.SampleID = resultDataDS.TubeAdditionalSolutions(0).SolutionName
                                                    myCellSamplesPosInfoRow.TubeType = myRotorContentByPositionDS.twksWSRotorContentByPosition(0).TubeType
                                                    myCellPosInfoDS.Samples.Rows.Add(myCellSamplesPosInfoRow)
                                                    'Else
                                                    'TODO: pending process for other Solutions: ISE, etc
                                                End If
                                            End If

                                        ElseIf (myRotorContentByPositionDS.twksWSRotorContentByPosition.Rows.Count > 0) AndAlso _
                                               (myRotorContentByPositionDS.twksWSRotorContentByPosition(0).IsElementIDNull) AndAlso _
                                               (String.Compare(myRotorContentByPositionDS.twksWSRotorContentByPosition(0).Status, "NO_INUSE", False) = 0) Then
                                            'Process for NOT IN USE positions
                                            Dim myNoInUsePositions As New WSNotInUseRotorPositionsDelegate
                                            myGlobalDataTo = myNoInUsePositions.GetPositionContent(dbConnection, pos.AnalyzerID, _
                                                                                                   pos.RotorType, _
                                                                                                   pos.RingNumber, _
                                                                                                   pos.CellNumber, _
                                                                                                   pos.WorkSessionID)

                                            Dim resultDataDS As New VirtualRotorPosititionsDS
                                            Dim myCellSamplesPosInfoRow As CellPositionInformationDS.SamplesRow
                                            Dim myCellReagentPosInfoRow As CellPositionInformationDS.ReagentsRow

                                            If (Not myGlobalDataTo.HasError AndAlso Not myGlobalDataTo.SetDatos Is Nothing) Then
                                                resultDataDS = DirectCast(myGlobalDataTo.SetDatos, VirtualRotorPosititionsDS)

                                                If (resultDataDS.tparVirtualRotorPosititions.Rows.Count > 0) Then
                                                    Select Case (resultDataDS.tparVirtualRotorPosititions(0).TubeContent())
                                                        Case "WASH_SOL"
                                                            myCellPosInfoDS.PositionInformation(0).BeginEdit()
                                                            myCellPosInfoDS.PositionInformation(0).Content = "WASH_SOL"
                                                            myCellPosInfoDS.PositionInformation(0).SetSolutionCodeNull()
                                                            If (Not resultDataDS.tparVirtualRotorPosititions(0).IsSolutionCodeNull) Then
                                                                myCellPosInfoDS.PositionInformation(0).SolutionCode = resultDataDS.tparVirtualRotorPosititions(0).SolutionCode
                                                            End If
                                                            myCellPosInfoDS.PositionInformation(0).EndEdit()

                                                            Dim myPreloadedMasterDataDelegate As New PreloadedMasterDataDelegate
                                                            myGlobalDataTo = myPreloadedMasterDataDelegate.GetSubTableItem(dbConnection, GlobalEnumerates.PreloadedMasterDataEnum.WASHING_SOLUTIONS, _
                                                                                                                           resultDataDS.tparVirtualRotorPosititions(0).SolutionCode)
                                                            If (Not myGlobalDataTo.HasError AndAlso Not myGlobalDataTo.SetDatos Is Nothing) Then
                                                                Dim myPreloadedMasterDataDS As PreloadedMasterDataDS = DirectCast(myGlobalDataTo.SetDatos, PreloadedMasterDataDS)

                                                                myCellReagentPosInfoRow = myCellPosInfoDS.Reagents.NewReagentsRow()
                                                                myCellReagentPosInfoRow.CellNumber = pos.CellNumber
                                                                myCellReagentPosInfoRow.ReagentName = myPreloadedMasterDataDS.tfmwPreloadedMasterData(0).FixedItemDesc
                                                                If (Not myRotorContentByPositionDS.twksWSRotorContentByPosition(0).IsRealVolumeNull) Then
                                                                    myCellReagentPosInfoRow.RealVolume = myRotorContentByPositionDS.twksWSRotorContentByPosition(0).RealVolume
                                                                End If
                                                                If (Not myRotorContentByPositionDS.twksWSRotorContentByPosition(0).IsRemainingTestsNumberNull) Then
                                                                    myCellReagentPosInfoRow.RemainingTests = myRotorContentByPositionDS.twksWSRotorContentByPosition(0).RemainingTestsNumber
                                                                End If
                                                                myCellReagentPosInfoRow.BottleCode = myRotorContentByPositionDS.twksWSRotorContentByPosition(0).TubeType
                                                                myCellPosInfoDS.Reagents.Rows.Add(myCellReagentPosInfoRow)
                                                            End If

                                                        Case "SPEC_SOL"
                                                            myCellPosInfoDS.PositionInformation(0).BeginEdit()
                                                            myCellPosInfoDS.PositionInformation(0).Content = "SPEC_SOL"
                                                            myCellPosInfoDS.PositionInformation(0).SetSolutionCodeNull()
                                                            If (Not resultDataDS.tparVirtualRotorPosititions(0).IsSolutionCodeNull) Then
                                                                myCellPosInfoDS.PositionInformation(0).SolutionCode = resultDataDS.tparVirtualRotorPosititions(0).SolutionCode
                                                            End If
                                                            myCellPosInfoDS.PositionInformation(0).EndEdit()

                                                            Dim myPreloadedMasterDataDelegate As New PreloadedMasterDataDelegate
                                                            myGlobalDataTo = myPreloadedMasterDataDelegate.GetSubTableItem(dbConnection, GlobalEnumerates.PreloadedMasterDataEnum.SPECIAL_SOLUTIONS, _
                                                                                                                           resultDataDS.tparVirtualRotorPosititions(0).SolutionCode)

                                                            If (Not myGlobalDataTo.HasError AndAlso Not myGlobalDataTo.SetDatos Is Nothing) Then
                                                                Dim myPreloadedMasterDataDS As PreloadedMasterDataDS = DirectCast(myGlobalDataTo.SetDatos, PreloadedMasterDataDS)

                                                                myCellReagentPosInfoRow = myCellPosInfoDS.Reagents.NewReagentsRow()
                                                                myCellReagentPosInfoRow.CellNumber = pos.CellNumber
                                                                myCellReagentPosInfoRow.ReagentName = myPreloadedMasterDataDS.tfmwPreloadedMasterData(0).FixedItemDesc
                                                                If (Not myRotorContentByPositionDS.twksWSRotorContentByPosition(0).IsRealVolumeNull) Then
                                                                    myCellReagentPosInfoRow.RealVolume = myRotorContentByPositionDS.twksWSRotorContentByPosition(0).RealVolume
                                                                End If
                                                                If (Not myRotorContentByPositionDS.twksWSRotorContentByPosition(0).IsRemainingTestsNumberNull) Then
                                                                    myCellReagentPosInfoRow.RemainingTests = myRotorContentByPositionDS.twksWSRotorContentByPosition(0).RemainingTestsNumber
                                                                End If

                                                                myCellReagentPosInfoRow.BottleCode = myRotorContentByPositionDS.twksWSRotorContentByPosition(0).TubeType
                                                                myCellPosInfoDS.Reagents.Rows.Add(myCellReagentPosInfoRow)
                                                            End If

                                                        Case "TUBE_WASH_SOL"
                                                            myCellPosInfoDS.PositionInformation(0).BeginEdit()
                                                            myCellPosInfoDS.PositionInformation(0).Content = "TUBE_WASH_SOL"
                                                            myCellPosInfoDS.PositionInformation(0).SetSolutionCodeNull()
                                                            If (Not resultDataDS.tparVirtualRotorPosititions(0).IsSolutionCodeNull) Then
                                                                myCellPosInfoDS.PositionInformation(0).SolutionCode = resultDataDS.tparVirtualRotorPosititions(0).SolutionCode
                                                            End If
                                                            myCellPosInfoDS.PositionInformation(0).EndEdit()

                                                            Dim myPreloadedMasterDataDelegate As New PreloadedMasterDataDelegate
                                                            myGlobalDataTo = myPreloadedMasterDataDelegate.GetSubTableItem(dbConnection, GlobalEnumerates.PreloadedMasterDataEnum.WASHING_SOLUTIONS, _
                                                                                                                           resultDataDS.tparVirtualRotorPosititions(0).SolutionCode)
                                                            If (Not myGlobalDataTo.HasError AndAlso Not myGlobalDataTo.SetDatos Is Nothing) Then
                                                                Dim myPreloadedMasterDataDS As PreloadedMasterDataDS
                                                                myPreloadedMasterDataDS = DirectCast(myGlobalDataTo.SetDatos, PreloadedMasterDataDS)

                                                                myCellSamplesPosInfoRow = myCellPosInfoDS.Samples.NewSamplesRow()
                                                                myCellSamplesPosInfoRow.SampleID = myPreloadedMasterDataDS.tfmwPreloadedMasterData(0).FixedItemDesc
                                                                myCellSamplesPosInfoRow.TubeType = myRotorContentByPositionDS.twksWSRotorContentByPosition(0).TubeType
                                                                myCellPosInfoDS.Samples.Rows.Add(myCellSamplesPosInfoRow)
                                                            End If

                                                        Case "TUBE_SPEC_SOL"
                                                            myCellPosInfoDS.PositionInformation(0).BeginEdit()
                                                            myCellPosInfoDS.PositionInformation(0).Content = "TUBE_SPEC_SOL"
                                                            myCellPosInfoDS.PositionInformation(0).SetSolutionCodeNull()
                                                            If (Not resultDataDS.tparVirtualRotorPosititions(0).IsSolutionCodeNull) Then
                                                                myCellPosInfoDS.PositionInformation(0).SolutionCode = resultDataDS.tparVirtualRotorPosititions(0).SolutionCode
                                                            End If
                                                            myCellPosInfoDS.PositionInformation(0).EndEdit()

                                                            Dim myPreloadedMasterDataDelegate As New PreloadedMasterDataDelegate
                                                            myGlobalDataTo = myPreloadedMasterDataDelegate.GetSubTableItem(dbConnection, GlobalEnumerates.PreloadedMasterDataEnum.SPECIAL_SOLUTIONS, _
                                                                                                                           resultDataDS.tparVirtualRotorPosititions(0).SolutionCode)
                                                            If (Not myGlobalDataTo.HasError AndAlso Not myGlobalDataTo.SetDatos Is Nothing) Then
                                                                Dim myPreloadedMasterDataDS As PreloadedMasterDataDS = DirectCast(myGlobalDataTo.SetDatos, PreloadedMasterDataDS)

                                                                myCellSamplesPosInfoRow = myCellPosInfoDS.Samples.NewSamplesRow()
                                                                myCellSamplesPosInfoRow.SampleID = myPreloadedMasterDataDS.tfmwPreloadedMasterData(0).FixedItemDesc
                                                                myCellSamplesPosInfoRow.TubeType = myRotorContentByPositionDS.twksWSRotorContentByPosition(0).TubeType
                                                                myCellPosInfoDS.Samples.Rows.Add(myCellSamplesPosInfoRow)
                                                            End If

                                                        Case "CTRL"
                                                            myCellPosInfoDS.PositionInformation(0).BeginEdit()
                                                            myCellPosInfoDS.PositionInformation(0).Content = "CTRL"
                                                            myCellPosInfoDS.PositionInformation(0).SetSolutionCodeNull()
                                                            myCellPosInfoDS.PositionInformation(0).EndEdit()

                                                            Dim myControlsDelegate As New ControlsDelegate
                                                            resultData = myControlsDelegate.GetControlData(pDBConnection, resultDataDS.tparVirtualRotorPosititions(0).ControlID)

                                                            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                                                Dim myControlDataTo As ControlsDS = DirectCast(resultData.SetDatos, ControlsDS)

                                                                myCellSamplesPosInfoRow = myCellPosInfoDS.Samples.NewSamplesRow()
                                                                myCellSamplesPosInfoRow.LotNumber = myControlDataTo.tparControls(0).LotNumber
                                                                myCellSamplesPosInfoRow.ExpirationDate = Convert.ToDateTime(myControlDataTo.tparControls(0).ExpirationDate)
                                                                myCellSamplesPosInfoRow.SampleID = myControlDataTo.tparControls(0).ControlName
                                                                myCellSamplesPosInfoRow.TubeType = myRotorContentByPositionDS.twksWSRotorContentByPosition(0).TubeType
                                                                myCellPosInfoDS.Samples.Rows.Add(myCellSamplesPosInfoRow)
                                                            End If

                                                        Case "CALIB"
                                                            myCellPosInfoDS.PositionInformation(0).BeginEdit()
                                                            myCellPosInfoDS.PositionInformation(0).Content = "CALIB"
                                                            myCellPosInfoDS.PositionInformation(0).SetSolutionCodeNull()
                                                            myCellPosInfoDS.PositionInformation(0).EndEdit()

                                                            Dim myCalibratorDelegate As New CalibratorsDelegate
                                                            resultData = myCalibratorDelegate.GetCalibratorData(pDBConnection, resultDataDS.tparVirtualRotorPosititions(0).CalibratorID)

                                                            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                                                Dim myCalibratorDataTo As CalibratorsDS = DirectCast(resultData.SetDatos, CalibratorsDS)

                                                                myCellSamplesPosInfoRow = myCellPosInfoDS.Samples.NewSamplesRow()
                                                                myCellSamplesPosInfoRow.MultiItemNumber = resultDataDS.tparVirtualRotorPosititions(0).MultiItemNumber
                                                                myCellSamplesPosInfoRow.LotNumber = myCalibratorDataTo.tparCalibrators(0).LotNumber
                                                                myCellSamplesPosInfoRow.ExpirationDate = Convert.ToDateTime(myCalibratorDataTo.tparCalibrators(0).ExpirationDate)
                                                                myCellSamplesPosInfoRow.SampleID = myCalibratorDataTo.tparCalibrators(0).CalibratorName
                                                                myCellSamplesPosInfoRow.TubeType = myRotorContentByPositionDS.twksWSRotorContentByPosition(0).TubeType
                                                                myCellPosInfoDS.Samples.Rows.Add(myCellSamplesPosInfoRow)
                                                            End If

                                                        Case "REAGENT"
                                                            myCellPosInfoDS.PositionInformation(0).BeginEdit()
                                                            myCellPosInfoDS.PositionInformation(0).Content = "REAGENT"
                                                            myCellPosInfoDS.PositionInformation(0).SetSolutionCodeNull()
                                                            myCellPosInfoDS.PositionInformation(0).EndEdit()

                                                            myCellReagentPosInfoRow = myCellPosInfoDS.Reagents.NewReagentsRow()
                                                            myCellReagentPosInfoRow.CellNumber = pos.CellNumber
                                                            myCellReagentPosInfoRow.MultiItemNumber = resultDataDS.tparVirtualRotorPosititions(0).MultiItemNumber
                                                            myCellReagentPosInfoRow.BottleCode = myRotorContentByPositionDS.twksWSRotorContentByPosition(0).TubeType

                                                            Dim myReagentsDelegate As New ReagentsDelegate
                                                            If (Not resultDataDS.tparVirtualRotorPosititions(0).IsReagentIDNull) Then
                                                                resultData = myReagentsDelegate.GetReagentsData(pDBConnection, resultDataDS.tparVirtualRotorPosititions(0).ReagentID)
                                                                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                                                    Dim myReagentDataTo As ReagentsDS = DirectCast(resultData.SetDatos, ReagentsDS)

                                                                    If myReagentDataTo.tparReagents.Count > 0 Then 'DL 07/02/2012

                                                                        myCellReagentPosInfoRow.ReagentName = myReagentDataTo.tparReagents(0).ReagentName

                                                                        'Get the list of Tests that use the Reagent
                                                                        Dim myTestReagentDelegate As New TestReagentsDelegate
                                                                        resultData = myTestReagentDelegate.GetTestReagentsByReagentID(dbConnection, resultDataDS.tparVirtualRotorPosititions(0).ReagentID)

                                                                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                                                            Dim myTestReagentsDS As TestReagentsDS = DirectCast(resultData.SetDatos, TestReagentsDS)

                                                                            For i As Integer = 0 To myTestReagentsDS.tparTestReagents.Rows.Count - 1
                                                                                If (i > 0) Then myCellReagentPosInfoRow.TestList &= ","
                                                                                myCellReagentPosInfoRow.TestList &= myTestReagentsDS.tparTestReagents(0).TestName
                                                                            Next
                                                                        End If
                                                                    End If 'DL 02/07/2012
                                                                End If
                                                            End If

                                                            If (Not myRotorContentByPositionDS.twksWSRotorContentByPosition(0).IsRealVolumeNull) Then
                                                                myCellReagentPosInfoRow.RealVolume = myRotorContentByPositionDS.twksWSRotorContentByPosition(0).RealVolume
                                                            End If
                                                            If (Not myRotorContentByPositionDS.twksWSRotorContentByPosition(0).IsRemainingTestsNumberNull) Then
                                                                myCellReagentPosInfoRow.RemainingTests = myRotorContentByPositionDS.twksWSRotorContentByPosition(0).RemainingTestsNumber
                                                            End If
                                                            myCellPosInfoDS.Reagents.Rows.Add(myCellReagentPosInfoRow)

                                                        Case "PATIENT"
                                                            myCellPosInfoDS.PositionInformation(0).BeginEdit()
                                                            myCellPosInfoDS.PositionInformation(0).Content = "PATIENT"
                                                            myCellPosInfoDS.PositionInformation(0).SetSolutionCodeNull()
                                                            myCellPosInfoDS.PositionInformation(0).EndEdit()

                                                            myCellSamplesPosInfoRow = myCellPosInfoDS.Samples.NewSamplesRow()
                                                            myCellSamplesPosInfoRow.SampleType = resultDataDS.tparVirtualRotorPosititions(0).SampleType
                                                            myCellSamplesPosInfoRow.TubeType = myRotorContentByPositionDS.twksWSRotorContentByPosition(0).TubeType

                                                            If (Not resultDataDS.tparVirtualRotorPosititions(0).IsPatientIDNull) Then
                                                                myCellSamplesPosInfoRow.SampleID = resultDataDS.tparVirtualRotorPosititions(0).PatientID
                                                            ElseIf Not resultDataDS.tparVirtualRotorPosititions(0).IsOrderIDNull Then
                                                                myCellSamplesPosInfoRow.SampleID = resultDataDS.tparVirtualRotorPosititions(0).OrderID
                                                            End If

                                                            If (Not resultDataDS.tparVirtualRotorPosititions(0).IsPredilutionFactorNull) Then
                                                                myCellSamplesPosInfoRow.PredilutionFactor = resultDataDS.tparVirtualRotorPosititions(0).PredilutionFactor
                                                                myCellSamplesPosInfoRow.Diluted = (resultDataDS.tparVirtualRotorPosititions(0).PredilutionFactor > 0)
                                                            End If

                                                            If (Not resultDataDS.tparVirtualRotorPosititions(0).IsOnlyForISENull) Then
                                                                If (resultDataDS.tparVirtualRotorPosititions(0).OnlyForISE) Then
                                                                    myCellSamplesPosInfoRow.SampleType &= " (ISE)"
                                                                End If
                                                            End If
                                                            myCellPosInfoDS.Samples.Rows.Add(myCellSamplesPosInfoRow)
                                                    End Select
                                                End If
                                            End If
                                        End If
                                    End If
                                Next
                                'The information is returned
                                resultData.HasError = False
                                resultData.SetDatos = myCellPosInfoDS
                            End If
                        End If
                    End If
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSRotorContentByPositionDelegate.GetPositionInfoForReport", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get information of all non free positions in the Reagents Rotor for the active WorkSession, including those that contain
        ''' Not In Use elements. Used to save the rotor as an internal Virtual Rotor before reset the WS (due to the Reagents Rotor
        ''' is not phisically discharged after finishing each WS) 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pVirtualRotorID"></param>
        ''' <returns>GlobalDataTO containing a typed DataSet VirtualRotorPositionsDS with the content of all non-free
        '''          positions in the Reagents Rotor for the active WorkSession</returns>
        ''' <remarks>
        ''' Created by:  SA 19/04/2011
        ''' </remarks>
        Public Function GetReagentsRotorPositions(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
                                                  ByVal pWorkSessionID As String, ByVal pVirtualRotorID As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myWSRotorPosDAO As New twksWSRotorContentByPositionDAO
                        resultData = myWSRotorPosDAO.GetReagentsRotorPositions(dbConnection, pAnalyzerID, pWorkSessionID, pVirtualRotorID)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSRotorContentByPositionDelegate.GetReagentsRotorPositions", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get information of all Ring Cells (status and content) in all Rotors of an Analyzer included in a Work Session
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifer</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet WSRotorContentByPositionDS with information of all Cells
        '''          in all the Analyzer Rotors</returns>
        ''' <remarks>
        ''' Created by:  VR 26/11/2009 - Tested: OK
        ''' Modified by: SA 13/01/2010 - Changed the way of opening the DB Connection to fulfill the new template
        ''' </remarks>
        Public Function GetRotorContentPositions(ByVal pDBConnection As SqlClient.SqlConnection, _
                                                 ByVal pWorkSessionID As String, ByVal pAnalyzerID As String) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing
            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myRotorContentByPosDAO As New twksWSRotorContentByPositionDAO
                        myGlobalDataTO = myRotorContentByPosDAO.GetRotorContentPositions(pDBConnection, pAnalyzerID, pWorkSessionID)
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSRotorContentByPositionDelegate.GetRotorContentPositions", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get information of all Ring Cells (status and content) in all Rotors of an Analyzer included in a Work Session
        ''' For NOT IN USE Positions, get the Status saved in table of Not In Use Rotor Positions
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifer</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet WSRotorContentByPositionDS with information of all Cells
        '''          in all the Analyzer Rotors</returns>
        ''' <remarks>
        ''' Created by:  JV 03/12/2013 - BT #1384
        ''' </remarks>
        Public Function GetRotorContentPositionsResetDone(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, _
                                                          ByVal pAnalyzerID As String) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing
            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myRotorContentByPosDAO As New twksWSRotorContentByPositionDAO
                        myGlobalDataTO = myRotorContentByPosDAO.GetRotorContentPositionsResetDone(pDBConnection, pAnalyzerID, pWorkSessionID)
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSRotorContentByPositionDelegate.GetRotorContentPositionsResetDone", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Read the current contents for the Samples or Reagents Rotor (AnalyzerID and RotorType must be informed, 
        ''' WorkSessionID can be informed or can be an empty string)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pRotorType">Rotor Type</param>
        ''' <returns>GlobalDataTO containing a typed DataSet WSRotorContentByPositionDS</returns>
        ''' <remarks>
        ''' Created by:  AG 03/08/2011
        ''' </remarks>
        Public Function GetRotorCurrentContentForBarcodeManagement(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
                                                                   ByVal pWorkSessionID As String, ByVal pRotorType As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myRotorContentByPosDAO As New twksWSRotorContentByPositionDAO
                        If String.Compare(pAnalyzerID, "", False) <> 0 And String.Compare(pWorkSessionID, "", False) <> 0 Then
                            resultData = myRotorContentByPosDAO.GetRotorTypeContentPositions(pDBConnection, pAnalyzerID, pWorkSessionID, pRotorType)
                            'Else
                            'TODO
                            'Read current Sample Rotor when a WorkSession does not exist (we have to use the VirtualRotor generated in reset)
                        End If
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSRotorContentByPositionDelegate.GetRotorCurrentContentForBarcodeManagement", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' For the specified OrderTestID, search Rotor Positions in which its required Elements are placed (the searching is done according value of 
        ''' entry parameters pOnlySamplesRotor and pOnlyExecutionSamplePosition
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pOrderTestID">Order Test Identifier</param>
        ''' <param name="pOnlySamplesRotor">When TRUE, it indicates the searching is done only in SAMPLES Rotor; otherwise, the searching is done in both
        '''                                 Rotors, Samples and Reagents</param>
        ''' <param name="pOnlyExecutionSamplePosition">When TRUE it indicates the searching is only for Rotor positions in which the Sample tube is placed;
        '''                                            otherwise, positions for all related required Elements are searched</param>
        ''' <returns>GlobalDataTO containing a typed DataSet WSRotorContentByPositionDS with the list of Rotor Positions related with the informed Order Test</returns>
        ''' <remarks>
        ''' Created by:  AG 04/07/2011
        ''' </remarks>
        Public Function GetRotorPositionsByOrderTestID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOrderTestID As Integer, ByVal pOnlySamplesRotor As Boolean, _
                                                       ByVal pOnlyExecutionSamplePosition As Boolean) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksWSRotorContentByPositionDAO
                        resultData = myDAO.GetRotorPositionsByOrderTestID(dbConnection, pOrderTestID, pOnlySamplesRotor, pOnlyExecutionSamplePosition)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSRotorContentByPositionDelegate.GetRotorPositionsByOrderTestID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get information of all non free positions in the Samples Rotor for the active WorkSession, including those that contain
        ''' Not In Use elements. Used to save the rotor as an internal Virtual Rotor before reset the WS 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pVirtualRotorID"></param>
        ''' <returns>GlobalDataTO containing a typed DataSet VirtualRotorPositionsDS with the content of all non-free
        '''          positions in the Samples Rotor for the active WorkSession</returns>
        ''' <remarks>
        ''' Created by:  DL 04/08/2011
        ''' Modified by: RH 01/09/2011 - Short-circuit evaluation. Remove unneeded and memory wasting "New" instructions.
        ''' </remarks>
        Public Function GetSamplesRotorPositions(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
                                                 ByVal pWorkSessionID As String, ByVal pVirtualRotorID As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myWSRotorPosDAO As New twksWSRotorContentByPositionDAO
                        resultData = myWSRotorPosDAO.GetSamplesRotorPositions(dbConnection, pAnalyzerID, pWorkSessionID, pVirtualRotorID)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSRotorContentByPositionDelegate.GetSamplesRotorPositions", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get the Rotor Cells containing bottles of the specified Washing Solution
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pSolutionCode">Code for the Washing Solution</param>
        ''' <returns>GlobalDataTO containing a typed DataSet WSRotorContentByPositionDS with the list of Rotor Cells</returns>
        ''' <remarks>
        ''' Created by:  TR 28/01/2011
        ''' </remarks>
        Public Function GetWashingSolutionPosInfoBySolutionCode(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pSolutionCode As String) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing
            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myRotorContentByPosDAO As New twksWSRotorContentByPositionDAO
                        myGlobalDataTO = myRotorContentByPosDAO.GetWashingSolutionPosInfoBySolutionCode(dbConnection, pSolutionCode)
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSRotorContentByPositionDelegate.GetWashingSolutionPosInfoBySolutionCode", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Fill Rotor Positions with the content of the specified Virtual Rotor
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pRotorType"> Rotor Type</param>
        ''' <param name="pVirtualRotorID">Virtual Rotor Identifier. When its value is -1, it means the Reagents Rotor will be loaded with 
        '''                               the content of the Internal Virtual Rotor containing the last configuration of the Reagents Rotor</param>
        ''' <returns>GlobalDataTO containing a typed DataSet WSRotorContentByPositionDS with the information 
        '''          of all the positions in the Rotor (loaded with the content of positions in the Virtual Rotor)</returns>
        ''' <remarks>
        ''' Created by:  VR
        ''' Modified by: AG 03/12/2009 - Call Update in RotorContentByPositionDelegate  
        '''              VR 16/12/2009 - Tested: OK 
        '''              VR 05/01/2010 - Tested: OK (Partial) REAGENT flow is Pending for Test
        '''              VR 06/01/2010 - Tested: OK
        '''              SA 13/01/2010 - Added the transaction control (following the new template)
        '''              AG 14/01/2010 - Modify LINQ clause where (dont select when ElementID is null) (Tested OK)
        '''              AG 21/01/2010 - Positions NO_INUSE has to update twksWSNotInUseRotorPositions table (Tested OK)
        '''              AG 25/01/2010 - Virtual rotors has only NO FREE positions but LoadRotor has to return all the rotor positions
        '''              TR 05/02/2010 - Eliminate the Reset Rotor from this function
        '''              SA 19/04/2011 - When parameter pVirtualRotorID is not informed (value is -1), inform it with the value returned
        '''                              after calling function GetRotor
        '''              RH 01/09/2011 - Code optimization and bug correction
        '''              AG 03/02/2012 - Changes to save status of cells marked as DEPLETED or FEW in table of Not In Use Rotor Positions
        '''              SA 09/02/2012 - Code optimization: when preparing data to save Not In Use positions in the correspondent table, it is not 
        '''                              needed to search in DB the Position Status, all information about the cell is contained in DS myVirtualRotorPosDS
        '''              DL 30/04/2013 - 
        '''              XB+JC 09/10/2013 - Correction on Load Virtual Rotors #1274 Bugs tracking
        '''              XB 07/10/2014 - BA-1978 ==> Add log traces to catch NULL wrong assignment on RealVolume field
        '''              AG 07/10/2014 - BA-1979 ==> Replaced call to function twksWSRotorContentByPositionDAO.Update by a call to function Update 
        '''                                          in this Delegate, informing the process who is updating the content of the Rotor Position (to 
        '''                                          search which process is inserting invalid values: positions with TubeContent but not element ID)
        '''              SA 12/01/2015 - BA-1999 ==> When calling function CalculateReagentStatus, update the ElementStatus field also when the returned Status 
        '''                                          is NOPOS (currently, it is updated only when the returned Status is incomplete, and if all positioned Reagent
        '''                                          bottles are DEPLETED, the element is shown positioned in the TreeView of required Elements in Rotor Positioning Screen)
        ''' </remarks>
        Public Function LoadRotor(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, _
                                  ByVal pRotorType As String, ByVal pVirtualRotorID As Integer) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Get content of the selected Virtual Rotor
                        Dim resultDS As WSRotorContentByPositionDS = Nothing
                        Dim myVirtualRotorsDelegate As New VirtualRotorsPositionsDelegate

                        myGlobalDataTO = myVirtualRotorsDelegate.GetRotor(dbConnection, pVirtualRotorID, pRotorType)
                        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                            Dim myVirtualRotorPosDS As VirtualRotorPosititionsDS = DirectCast(myGlobalDataTO.SetDatos, VirtualRotorPosititionsDS)

                            If (myVirtualRotorPosDS.tparVirtualRotorPosititions.Rows.Count > 0) Then
                                Dim myVirtualRotorID As Integer = myVirtualRotorPosDS.tparVirtualRotorPosititions(0).VirtualRotorID

                                'Verify if positions in the Virtual Rotor containing a valid required Work Session Elements
                                Dim myRequiredElementsDelegate As New WSRequiredElementsDelegate
                                myGlobalDataTO = myRequiredElementsDelegate.FindElementIDRelatedWithRotorPosition(dbConnection, pAnalyzerID, pWorkSessionID, pRotorType, _
                                                                                                                  myVirtualRotorPosDS)
                                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                    resultDS = DirectCast(myGlobalDataTO.SetDatos, WSRotorContentByPositionDS)

                                    'Update positions in the physical Rotor
                                    myGlobalDataTO = Update(dbConnection, pRotorType, resultDS, ClassCalledFrom.LoadRotor) 'AG 07/10/2014 BA-1979 call the Update method  in delegate instead of in DAO

                                    If (Not myGlobalDataTO.HasError) Then
                                        If (pRotorType = "REAGENTS") Then
                                            'Get Rotor Positions containing required Reagents to calculate its status (POS or INCOMPLETE)
                                            'Use TubeContent = REAGENT and ElementID NOT NULL, and sort data by ElementID; for each Reagent, calculate the Reagent Status just once
                                            '(due to it is possible to have several bottles of the same Reagent placed in the Rotor)
                                            Dim myReagentTubes As List(Of WSRotorContentByPositionDS.twksWSRotorContentByPositionRow)
                                            myReagentTubes = (From e As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow In resultDS.twksWSRotorContentByPosition _
                                                             Where e.RotorType = "REAGENTS" _
                                                           AndAlso e.TubeContent = "REAGENT" _
                                                       AndAlso Not e.IsElementIDNull _
                                                            Select e Order By e.ElementID).Distinct.ToList

                                            Dim immPreviousRow As Integer = 0
                                            Dim myReagentStatus As String = String.Empty
                                            Dim previousEleStatus As String = String.Empty
                                            Dim wsElementsRow As WSRequiredElementsDS.twksWSRequiredElementsRow = Nothing

                                            For Each myReagentContentROW As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow In myReagentTubes
                                                If (myReagentContentROW.ElementID <> immPreviousRow) Then
                                                    myGlobalDataTO = myRequiredElementsDelegate.CalculateReagentStatus(dbConnection, pAnalyzerID, pRotorType, myReagentContentROW.ElementID, True)
                                                    If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                                        myReagentStatus = DirectCast(myGlobalDataTO.SetDatos, String)

                                                        'BA-1999: If the returned Reagent Status is NOPOS (all positioned bottles are DEPLETED), ElementStatus field is also updated
                                                        If (myReagentStatus = "INCOMPLETE" OrElse myReagentStatus = "NOPOS") Then
                                                            myReagentContentROW.ElementStatus = myReagentStatus
                                                            previousEleStatus = myReagentContentROW.ElementStatus
                                                        End If

                                                        myGlobalDataTO = myRequiredElementsDelegate.UpdateStatus(dbConnection, myReagentContentROW.ElementID, myReagentContentROW.ElementStatus)
                                                        If (myGlobalDataTO.HasError) Then Exit For
                                                    Else
                                                        'Error calculating the Reagent Status
                                                        Exit For
                                                    End If
                                                Else
                                                    'Same Reagent but placed in another position; the Element Status has to be the same
                                                    'in the DataSet to return
                                                    myReagentContentROW.ElementStatus = previousEleStatus
                                                End If

                                                immPreviousRow = myReagentContentROW.ElementID
                                                previousEleStatus = myReagentContentROW.ElementStatus
                                            Next
                                            myReagentTubes = Nothing
                                        End If

                                        If (Not myGlobalDataTO.HasError) Then
                                            'For Reagents Rotor: get positions containing Additional Solutions
                                            'For Samples Rotor : get positions containing Patient Samples, Calibrators, Controls and Additional Solutions  
                                            Dim myTubes As List(Of WSRotorContentByPositionDS.twksWSRotorContentByPositionRow)
                                            myTubes = (From e As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow In resultDS.twksWSRotorContentByPosition _
                                                      Where e.RotorType = pRotorType _
                                                    AndAlso e.TubeContent <> "REAGENT" _
                                                AndAlso Not e.IsElementIDNull _
                                                     Select e Order By e.ElementID).Distinct.ToList

                                            For Each myContentROW As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow In myTubes
                                                'Update Status and also TubeType for the Rotor Position
                                                myGlobalDataTO = myRequiredElementsDelegate.UpdateStatusAndTubeType(dbConnection, pWorkSessionID, myContentROW.ElementID, _
                                                                                                                    myContentROW.ElementStatus, myContentROW.TubeType, _
                                                                                                                    myContentROW.TubeContent)
                                                If (myGlobalDataTO.HasError) Then Exit For
                                            Next
                                            myTubes = Nothing
                                        End If
                                    End If

                                    If (Not myGlobalDataTO.HasError) Then
                                        'Implement LINQ to get Rotor Positions containing not in use Elements
                                        Dim myNotInUseCells As List(Of WSRotorContentByPositionDS.twksWSRotorContentByPositionRow)
                                        myNotInUseCells = (From e As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow In resultDS.twksWSRotorContentByPosition _
                                                          Where e.Status = "NO_INUSE" _
                                                         Select e).ToList

                                        'Add all NOT_INUSE Elements into twksWSNotInUseRotorPositions table
                                        Dim myRingNumber As Integer
                                        Dim myCellNumber As Integer

                                        Dim noInUseDelegate As New WSNotInUseRotorPositionsDelegate
                                        Dim myNoInUseVirtualPosition As New VirtualRotorPosititionsDS
                                        Dim virtualRotorPosition As List(Of VirtualRotorPosititionsDS.tparVirtualRotorPosititionsRow)

                                        For Each noInUseRow As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow In myNotInUseCells
                                            myRingNumber = noInUseRow.RingNumber
                                            myCellNumber = noInUseRow.CellNumber

                                            'Implements LINQ to get information of the Cell from the Internal Virtual Rotor
                                            virtualRotorPosition = (From a As VirtualRotorPosititionsDS.tparVirtualRotorPosititionsRow In myVirtualRotorPosDS.tparVirtualRotorPosititions _
                                                                   Where a.VirtualRotorID = myVirtualRotorID _
                                                                 AndAlso a.RingNumber = myRingNumber _
                                                                 AndAlso a.CellNumber = myCellNumber _
                                                                  Select a).ToList

                                            If (virtualRotorPosition.Count > 0) Then
                                                'Copy information to the final DS containing NOT IN USE cells
                                                myNoInUseVirtualPosition.tparVirtualRotorPosititions.ImportRow(virtualRotorPosition.First())
                                            End If
                                        Next
                                        virtualRotorPosition = Nothing
                                        myNotInUseCells = Nothing

                                        'AG 07/10/2014 BA-1979 add classCalledFrom parameter
                                        'Insert the positions in the table of positions not in use for the WorkSession
                                        myGlobalDataTO = noInUseDelegate.Add(dbConnection, pAnalyzerID, pRotorType, pWorkSessionID, myNoInUseVirtualPosition, WSNotInUseRotorPositionsDelegate.ClassCalledFrom.LoadVirtualRotor)

                                        'DL 30/04/2013. Changes v2.0.0. Load also incomplete sample list from virtual rotor
                                        Dim myRotorContentByPosition As List(Of WSRotorContentByPositionDS.twksWSRotorContentByPositionRow)

                                        ' XB+JC 09/10/2013 
                                        myRotorContentByPosition = (From row As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow In resultDS.twksWSRotorContentByPosition _
                                                                   Where row.Status = "NO_INUSE" _
                                                                 AndAlso row.RotorType = "SAMPLES" _
                                                                 AndAlso row.TubeContent = "PATIENT" _
                                                                 AndAlso Not row.IsBarCodeInfoNull _
                                                                  Select row).ToList
                                        'AndAlso row.BarcodeStatus = "OK" _
                                        ' XB+JC 09/10/2013 

                                        Dim myBarCodeWS As New BarcodeWSDelegate
                                        For Each RowRotorContentByPos As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow In myRotorContentByPosition
                                            myGlobalDataTO = myBarCodeWS.DecodeSamplesBarCode(dbConnection, RowRotorContentByPos.BarCodeInfo)

                                            ' XB+JC 09/10/2013 
                                            ' JC 20/09/2013 - Fix Bug #1274
                                            ' Check if there was any error and the 
                                            ' LoadRotor is necessary continue
                                            If (myGlobalDataTO.HasError) Then
                                                ' if error was decodifing sample type
                                                ' set the sample with Error and paint the sample at Rotor Position with a cross 
                                                ' continue with next sample from virtual rotor 
                                                If (myGlobalDataTO.ErrorMessage = "Barcode Sample Type Error") Then
                                                    RowRotorContentByPos.BarcodeStatus = "ERROR"
                                                    myGlobalDataTO.HasError = False
                                                    Continue For
                                                End If
                                            End If ' END JC 20/09/2013 - Fix Bug #1274

                                            If (Not myGlobalDataTO.HasError) Then
                                                ' JC 20-09-2013 
                                                'if there wasnot any error on decode, and BarcodeStatus has Error value, set as Ok.
                                                ' special case: Rotor was saved with sample barcode status = error and restored when
                                                ' barcode configuration has changed and now, the sample may be is correct
                                                If (RowRotorContentByPos.BarcodeStatus = "ERROR") Then
                                                    RowRotorContentByPos.BarcodeStatus = "OK"
                                                End If ' END JC 20/09/2013 - Fix Bug #1274
                                                ' XB+JC 09/10/2013 

                                                Dim decodedDataDS As BarCodesDS = DirectCast(myGlobalDataTO.SetDatos, BarCodesDS)

                                                'new incorpore sample type
                                                For Each row As BarCodesDS.DecodedSamplesFieldsRow In decodedDataDS.DecodedSamplesFields
                                                    virtualRotorPosition = (From a As VirtualRotorPosititionsDS.tparVirtualRotorPosititionsRow In myVirtualRotorPosDS.tparVirtualRotorPosititions _
                                                                           Where a.VirtualRotorID = myVirtualRotorID _
                                                                         AndAlso a.RingNumber = RowRotorContentByPos.RingNumber _
                                                                         AndAlso a.CellNumber = RowRotorContentByPos.CellNumber _
                                                                          Select a).ToList

                                                    If (virtualRotorPosition.Count > 0) Then
                                                        row.BeginEdit()
                                                        row.SampleType = virtualRotorPosition.First.SampleType
                                                        row.EndEdit()
                                                        row.AcceptChanges()
                                                    End If
                                                Next
                                                'end new

                                                myRotorContentByPosition = (From row As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow In resultDS.twksWSRotorContentByPosition _
                                                                           Where row.Status = "NO_INUSE" _
                                                                         AndAlso row.RotorType = "SAMPLES" _
                                                                         AndAlso row.TubeContent = "PATIENT" _
                                                                         AndAlso row.BarcodeStatus = "OK" _
                                                                         AndAlso Not row.IsBarCodeInfoNull _
                                                                          Select row).ToList

                                                Dim noTestRequestDlg As New BarcodePositionsWithNoRequestsDelegate
                                                myGlobalDataTO = noTestRequestDlg.AddPosition(dbConnection, pAnalyzerID, pWorkSessionID, pRotorType, RowRotorContentByPos.CellNumber, decodedDataDS)
                                            End If
                                        Next
                                    End If
                                End If
                            End If
                        End If

                        If (Not myGlobalDataTO.HasError) Then
                            'When the Database Connection was opened locally, then the Commit is executed
                            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                            myGlobalDataTO.SetDatos = resultDS
                        Else
                            'When the Database Connection was opened locally, then the Rollback is executed
                            If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                            myGlobalDataTO.SetDatos = Nothing
                        End If
                    End If
                End If
            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                myGlobalDataTO.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSRotorContentByPositionDelegate.LoadRotor", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Manages all logic needed to drag and drop manually a required Element of a Work Session to a position in an Analyzer Rotor.
        ''' For Controls, Calibrators and Patient Samples, only one tube can be positioned in an Analyzer Rotor.  For Reagents and 
        ''' Additional Solutions more than one bottle in the Rotor is allowed
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pElementPos">Typed DataSet containing the basic data needed to place a required Element in a Rotor Cell</param>
        ''' <returns>GlobalDataTO containing a typed DataSet WSRotorContentByPositionDS the information about the Rotor Cell in 
        '''          which the Element was placed plus the recalculated Element Status </returns>
        ''' <remarks>
        ''' Created by:  AG 30/11/2009 - Tested: OK 01/12/2009 (Pending Calibrator positioning)
        ''' Modified by: SA 13/01/2010 - Changed the way of opening the DB Transaction to fulfill the new template
        '''              SA 26/01/2010 - Changes to get the max Ring Number for the Analyzer Rotor and pass it as parameter to each one 
        '''                              of the specific positioning functions for Samples (to improve the performance of positioning)
        '''              RH 16/06/2011 - Added TUBE_SPEC_SOL and TUBE_WASH_SOL. Code optimization
        '''              RH 13/09/2011 - Informed Barcode fields for the Rotor Position (ScannedPosition=False, BarcodeStatus=Null when it is Error/Unknown)
        '''              AG 10/10/2011 - Value of field BarcodeInfo for the Rotor Position has to remain without changes when BarcodeStatus is Error
        '''              SA 10/01/2012 - ErrorCode "ELEMENT_ALREADY_POSITIONED" is not returned; control of that case is made in the screen
        '''              SA 07/02/2012 - For Additional Solutions in tube, allow positioning of several tubes in the Analyzer Rotor
        ''' </remarks>
        Public Function ManualElementPositioning(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pElementPos As WSRotorContentByPositionDS) As GlobalDataTO
            Dim resultData As GlobalDataTO
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Get the maximum Ring Number for the Analyzer Rotor
                        Dim maxRingNumber As Integer = -1
                        Dim analyzerModelRotorsConfig As New AnalyzerModelRotorsConfigDelegate
                        resultData = analyzerModelRotorsConfig.GetMaxRingNumber(dbConnection, pElementPos.twksWSRotorContentByPosition(0).AnalyzerID, _
                                                                                pElementPos.twksWSRotorContentByPosition(0).RotorType)

                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            maxRingNumber = CType(resultData.SetDatos, Integer)

                            'Search if there are tubes/bottles of the Required Element positioned in the informed Analyzer/Rotor 
                            Dim numOfPlacedTubes As Integer = 0
                            Dim commandsDAO As New twksWSRotorContentByPositionDAO
                            resultData = commandsDAO.GetPlacedTubesByElement(dbConnection, pElementPos.twksWSRotorContentByPosition(0).AnalyzerID, _
                                                                             pElementPos.twksWSRotorContentByPosition(0).RotorType, _
                                                                             pElementPos.twksWSRotorContentByPosition(0).WorkSessionID, _
                                                                             pElementPos.twksWSRotorContentByPosition(0).ElementID)

                            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                'Set value of next MultiTubeNumber for the required element                        
                                numOfPlacedTubes = CType(resultData.SetDatos, Integer)

                                'Get the type of Element to place...
                                Dim tubeContent As String = pElementPos.twksWSRotorContentByPosition(0).TubeContent

                                pElementPos.twksWSRotorContentByPosition(0).BeginEdit()
                                pElementPos.twksWSRotorContentByPosition(0).MultiTubeNumber = numOfPlacedTubes + 1
                                pElementPos.twksWSRotorContentByPosition(0).ScannedPosition = False
                                If (pElementPos.twksWSRotorContentByPosition(0).BarcodeStatus = "ERROR") Then
                                    pElementPos.twksWSRotorContentByPosition(0).SetBarcodeStatusNull()
                                ElseIf (pElementPos.twksWSRotorContentByPosition(0).BarcodeStatus = "UNKNOWN") Then
                                    pElementPos.twksWSRotorContentByPosition(0).SetBarcodeStatusNull()
                                End If
                                pElementPos.twksWSRotorContentByPosition(0).EndEdit()

                                'SAMPLES ROTOR: Calibrators, Controls, Patient Samples and Additional Solutions in tube
                                If (tubeContent = "CALIB" OrElse tubeContent = "CTRL" OrElse String.Compare(tubeContent, "PATIENT", False) = 0 OrElse _
                                    tubeContent = "TUBE_SPEC_SOL" OrElse tubeContent = "TUBE_WASH_SOL") Then
                                    If (numOfPlacedTubes = 0) Then
                                        'The Element can be positioned in SamplesRotor
                                        Select Case (tubeContent)
                                            Case "CTRL"
                                                resultData = Me.ControlPositioning(dbConnection, pElementPos, maxRingNumber, False)
                                            Case "CALIB"
                                                resultData = Me.CalibratorPositioning(dbConnection, pElementPos, maxRingNumber, False)
                                            Case "PATIENT"
                                                resultData = Me.PatientSamplePositioning(dbConnection, pElementPos, maxRingNumber, False)
                                            Case "TUBE_SPEC_SOL", "TUBE_WASH_SOL"
                                                resultData = Me.AdditionalTubeSolutionPositioning(dbConnection, pElementPos, maxRingNumber, False)
                                        End Select
                                    Else
                                        'In Samples Rotor, more than one tube by Element is allowed only for Additional Solutions in tube...
                                        If (tubeContent = "TUBE_SPEC_SOL" OrElse tubeContent = "TUBE_WASH_SOL") Then
                                            resultData = Me.AdditionalTubeSolutionPositioning(dbConnection, pElementPos, maxRingNumber, False)
                                        Else
                                            'In this case, return an empty DataSet to avoid errorsin Rotor Positioning Screen
                                            resultData.SetDatos = New WSRotorContentByPositionDS
                                        End If

                                        'SA 10/01/2012 - This code is commented due to in Rotor Positioning Screen it is not allowed to drag&drop 
                                        'an Element already placed in Samples Rotor
                                        'resultData.HasError = True
                                        'resultData.ErrorCode = "ELEMENT_ALREADY_POSITIONED"
                                    End If
                                Else
                                    'REAGENTS ROTOR: Reagents or Additional Solutions
                                    If (String.Compare(tubeContent, "REAGENT", False) = 0) Then
                                        resultData = Me.ReagentPositioning(dbConnection, pElementPos)
                                    Else
                                        resultData = Me.AdditionalSolutionPositioning(dbConnection, pElementPos)
                                    End If
                                End If
                            End If
                        End If

                        If (Not resultData.HasError) Then
                            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                        Else
                            If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                        End If
                    End If
                End If
            Catch ex As Exception
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSRotorContentByPositionDelegate.ManualElementPositioning", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get data of the specified Rotor Cell or if a Cell is not informed, get data of all Cells in the Rotor
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pRotorType">Rotor Type</param>
        ''' <param name="pCellNumber">If -1 read by AnalyzerID, WorkSessionID and RotorType</param>
        ''' <returns>GlobalDataTO containing a typed DataSet WSRotorContentByPositionDS with the information obtained</returns>
        ''' <remarks>
        ''' Created by:  AG 09/06/2011
        ''' Modified by: SA 05/09/2011 - Removed parameter pNotInUsedCreatedFlag
        ''' </remarks>
        Public Function ReadByCellNumber(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, _
                                         ByVal pRotorType As String, ByVal pCellNumber As Integer, Optional ByVal pGetNotInUseStatus As Boolean = False) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksWSRotorContentByPositionDAO
                        resultData = myDAO.ReadByCellNumber(dbConnection, pAnalyzerID, pWorkSessionID, pRotorType, pCellNumber, pGetNotInUseStatus)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSRotorContentByPositionDelegate.ReadByCellNumber", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary></summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pRotorType">Rotor Type</param>
        ''' <param name="pCellNumber">Cell number</param>
        ''' <param name="pWorkSessionID">WorkSession Identifier</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet WSRotorContentByPosition</returns>
        ''' <remarks>
        ''' Created by:  DL 04/01/2011
        ''' </remarks>
        Public Function ReadByRotorTypeAndCellNumber(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pRotorType As String, ByVal pCellNumber As Integer, _
                                                     ByVal pWorkSessionID As String, ByVal pAnalyzerID As String) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myWSRotorContentByPosition As New twksWSRotorContentByPositionDAO
                        myGlobalDataTO = myWSRotorContentByPosition.ReadByRotorTypeAndCellNumber(dbConnection, pRotorType, pCellNumber, pWorkSessionID, pAnalyzerID)
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSRotorContentByPositionDelegate.ReadByRotorTypeAndCellNumber", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Manages the automatic positioning of Reagents and Additional Solutions. The default bottle used and the process 
        ''' will depend on the Analyzer Model and the Rotor Type:
        '''   For positioning in a REAGENTS Rotor (A400 Model):
        '''       * Reagents can be positioned only in the REAGENTS Rotor (positioning in a Non Cooled Rack is pending of definition)
        '''       * Reagents are placed by default in the external Ring in the min free position using the smallest bottle 
        '''         (but not the “death volume” bottle, which is only for User selection)
        '''       * If the external ring is full, the min free position is searched in the internal ring (the inmediate next one) 
        '''         and in this case the smallest bottle does not allowed in the external ring is used
        '''   For positioning in a SAMPLES and REAGENTS Rotor (A200 Model): All the process is pending of definition
        ''' 
        ''' After positioning a Reagent bottle, the number of remaining Tests that can be executed with the volume contained 
        ''' in the placed bottle is calculated
        ''' When Reagent Auto Positioning finishes, this function executes the Additional Solutions Auto Positioning by calling 
        ''' a specific function for this process
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pRotorType">Analyzer Rotor Type</param>
        ''' <returns>GlobalDataTO containing typed DataSet WSRotorContentByPositionDS with information of all positioned Reagents
        '''          and Additional Solutions
        ''' </returns>
        ''' <remarks>
        ''' Created by:  TR 14/12/2009 - Test: PENDING 
        ''' Modified by: SA 04/01/2010 - Changes needed in call to GetMaxRingNumber due to a new parameter was added
        '''              SA 13/01/2010 - Changed the way of opening the DB Transaction to fulfill the new template
        '''              TR 27/01/2010 - Add ring number validation for incrementing the ring or breaking the while.
        '''              TR 04/02/2010 - Add functionality to validate Reagents elments with status INCOMPLETE, and position the 
        '''                              missing bottles to be positioned.
        '''              RH 30/08/2011 - Bug corrections. Code optimization (unneeded News get out, short-circuit evaluation)
        '''              SA 17/02/2012 - Code improved 
        '''              SA 02/03/2012 - Informed parameter for the BottleType with calling function CalculateRemainingTests
        '''              SA 06/03/2012 - Call to function for automatic positioning of Additional Solutions have to be done although
        '''                              there are not Reagents pending to positioning
        '''              XB 07/10/2014 - BA-1978 ==> Add log traces to catch NULL wrong assignment on RealVolume field
        '''              AG 07/10/2014 - BA-1979 ==> Replaced call to function twksWSRotorContentByPositionDAO.Update by a call to function Update 
        '''                                          in this Delegate, informing the process who is updating the content of the Rotor Position (to 
        '''                                          search which process is inserting invalid values: positions with TubeContent but not element ID)
        ''' </remarks>
        Public Function ReagentsAutoPositioning(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, _
                                                ByVal pAnalyzerID As String, ByVal pRotorType As String) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myWSRotorContentByPosDS As New WSRotorContentByPositionDS
                        Dim myWSRequiredElementsDelegate As New WSRequiredElementsDelegate

                        'Validate if there are Reagents or Additional Solutions pending to positioning
                        myGlobalDataTO = myWSRequiredElementsDelegate.CountNotPositionedElements(dbConnection, pWorkSessionID, pRotorType)
                        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                            If (CType(myGlobalDataTO.SetDatos, Integer) > 0) Then
                                'Validate if there are free Rotor Positions
                                myGlobalDataTO = CheckAvailablePositionsByRotor(dbConnection, pAnalyzerID, pRotorType, pWorkSessionID)
                                If (Not myGlobalDataTO.HasError) Then
                                    'Continue only if there is not a warning of Rotor Full or Rotor Full but with Not In Use positions
                                    If (String.IsNullOrEmpty(myGlobalDataTO.ErrorCode)) Then
                                        Dim maxRingNumber As Integer = -1
                                        Dim analyzerModelRotorsConfig As New AnalyzerModelRotorsConfigDelegate

                                        'Get the maximum Ring Number for the Analyzer Rotor
                                        myGlobalDataTO = analyzerModelRotorsConfig.GetMaxRingNumber(dbConnection, pAnalyzerID, pRotorType)
                                        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                            maxRingNumber = CType(myGlobalDataTO.SetDatos, Integer)

                                            'Search all Reagents required in the WorkSession that has not been still positioned
                                            Dim myRequiredReagentsDS As New WSRequiredElementsTreeDS
                                            myGlobalDataTO = myWSRequiredElementsDelegate.GetRequiredReagentsDetails(dbConnection, pWorkSessionID, Nothing, "NOPOS")

                                            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                                myRequiredReagentsDS = DirectCast(myGlobalDataTO.SetDatos, WSRequiredElementsTreeDS)

                                                'Search all Reagents required in the WorkSession that has not been positioned but the total volume is not enough for the WS
                                                myGlobalDataTO = myWSRequiredElementsDelegate.GetRequiredReagentsDetails(dbConnection, pWorkSessionID, Nothing, "INCOMPLETE")
                                                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                                    'Add INCOMPLETE Reagents to the DataSet containing the NOPOS ones
                                                    For Each reqElementRow As WSRequiredElementsTreeDS.ReagentsRow In DirectCast(myGlobalDataTO.SetDatos, WSRequiredElementsTreeDS).Reagents
                                                        myRequiredReagentsDS.Reagents.ImportRow(reqElementRow)
                                                    Next
                                                End If
                                            End If

                                            If (Not myGlobalDataTO.HasError AndAlso myRequiredReagentsDS.Reagents.Rows.Count > 0) Then
                                                'Search minimum volume of "small" bottles
                                                Dim myReagentTubeTypesDelegate As New ReagentTubeTypesDelegate
                                                myGlobalDataTO = myReagentTubeTypesDelegate.GetMinimumBottleSize(dbConnection)

                                                If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                                                    Dim minBottleSize As Integer = CType(myGlobalDataTO.SetDatos, Integer)

                                                    'Process all Reagents pending to positioning or positioned but incomplete
                                                    Dim ringNumber As Integer
                                                    Dim refCellNumber As Integer
                                                    Dim bottleNumber As Integer

                                                    Dim noPosSmallBottles As Integer = 0
                                                    Dim noPosBigBottles As Integer = 0

                                                    Dim myReagentTubeTypesDS As ReagentTubeTypesDS
                                                    Dim myWSRequiredElementTubesDelegate As New WSRequiredElementsTubesDelegate

                                                    Dim myTempRCPDS As New WSRotorContentByPositionDS
                                                    Dim myWSRotorContentByPositionDAO As New twksWSRotorContentByPositionDAO
                                                    Dim myRCPRow As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow

                                                    For Each myReagentRow As WSRequiredElementsTreeDS.ReagentsRow In myRequiredReagentsDS.Reagents.Rows
                                                        'Get bottles needed to positioning the needed volume
                                                        myGlobalDataTO = myWSRequiredElementTubesDelegate.GetAllNeededBottles(dbConnection, myReagentRow.ElementID)

                                                        If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                                                            myReagentTubeTypesDS = DirectCast(myGlobalDataTO.SetDatos, ReagentTubeTypesDS)

                                                            For Each myReagTubeTypeRow As ReagentTubeTypesDS.ReagentTubeTypesRow In myReagentTubeTypesDS.ReagentTubeTypes.Rows
                                                                'Select the Rotor Ring according the Bottle Size (small bottles in external ring, big bottles in the internal one)
                                                                ringNumber = 1
                                                                If (myReagTubeTypeRow.TubeVolume > minBottleSize) Then ringNumber = maxRingNumber

                                                                'If at least a Small Bottle could not be positioned: ROTOR is FULL
                                                                If (noPosSmallBottles > 0) Then Exit For

                                                                'If the current bottle is big and at least a Big Bottle could not be positioned: INTERNAL ROTOR is FULL
                                                                If (ringNumber = maxRingNumber AndAlso noPosBigBottles > 0) Then Exit For

                                                                bottleNumber = 1
                                                                refCellNumber = 0
                                                                Do While (bottleNumber <= myReagTubeTypeRow.NumOfBottles)
                                                                    'Search minimum free cell in the informed Ring nearest to the informed cell
                                                                    myGlobalDataTO = myWSRotorContentByPositionDAO.GetMinFreeCellByRing(dbConnection, pAnalyzerID, pRotorType, pWorkSessionID, _
                                                                                                                                        ringNumber, refCellNumber)
                                                                    If (myGlobalDataTO.HasError) Then Exit Do

                                                                    If (Not String.IsNullOrEmpty(myGlobalDataTO.SetDatos.ToString)) Then
                                                                        'Fill the Rotor Cell with data of the positioned Reagent Bottle
                                                                        myRCPRow = myTempRCPDS.twksWSRotorContentByPosition.NewtwksWSRotorContentByPositionRow
                                                                        myRCPRow.AnalyzerID = pAnalyzerID
                                                                        myRCPRow.RotorType = pRotorType
                                                                        myRCPRow.RingNumber = ringNumber
                                                                        myRCPRow.CellNumber = CType(myGlobalDataTO.SetDatos, Integer)
                                                                        myRCPRow.WorkSessionID = pWorkSessionID
                                                                        myRCPRow.ElementID = myReagentRow.ElementID
                                                                        myRCPRow.MultiTubeNumber = bottleNumber
                                                                        myRCPRow.TubeContent = myReagentRow.TubeContent
                                                                        myRCPRow.TubeType = myReagTubeTypeRow.TubeCode
                                                                        myRCPRow.RealVolume = myReagTubeTypeRow.TubeVolume
                                                                        myRCPRow.Status = "INUSE"
                                                                        myRCPRow.ScannedPosition = False
                                                                        myRCPRow.SetBarCodeInfoNull()
                                                                        myRCPRow.SetBarcodeStatusNull()
                                                                        myTempRCPDS.twksWSRotorContentByPosition.AddtwksWSRotorContentByPositionRow(myRCPRow)

                                                                        'Calculate the number of remaining Tests that can be executed with the real volume contained in the positioned bottle
                                                                        'and update the information for the position 
                                                                        myGlobalDataTO = myWSRequiredElementsDelegate.CalculateRemainingTests(dbConnection, pWorkSessionID, myReagentRow.ElementID, _
                                                                                                                                              myReagTubeTypeRow.TubeVolume, myReagTubeTypeRow.TubeCode)
                                                                        If (myGlobalDataTO.HasError) Then Exit Do
                                                                        If (Not myGlobalDataTO.SetDatos Is Nothing) Then myRCPRow.RemainingTestsNumber = CType(myGlobalDataTO.SetDatos, Integer)

                                                                        'Update the Rotor Cell...
                                                                        'myGlobalDataTO = myWSRotorContentByPositionDAO.Update(dbConnection, myTempRCPDS)
                                                                        myGlobalDataTO = Update(dbConnection, pRotorType, myTempRCPDS, ClassCalledFrom.ReagentAUTOPositioning) 'AG 07/10/2014 BA-1979 call the Update method  in delegate instead of in DAO
                                                                        If (myGlobalDataTO.HasError) Then Exit Do

                                                                        refCellNumber = myRCPRow.CellNumber
                                                                        bottleNumber += 1

                                                                        'Import the updated row to a Dataset and clear the temporary one used to update the Rotor Cell
                                                                        myWSRotorContentByPosDS.twksWSRotorContentByPosition.ImportRow(myRCPRow)
                                                                        myTempRCPDS.twksWSRotorContentByPosition.Clear()
                                                                    Else
                                                                        If (ringNumber = 1) Then
                                                                            'There is not a free position in the Ring; if it is the external one, move to the internal one
                                                                            ringNumber += 1
                                                                            refCellNumber = 0
                                                                        Else
                                                                            'No free position to place the Reagent bottle 
                                                                            If (myReagTubeTypeRow.TubeVolume = minBottleSize) Then
                                                                                noPosSmallBottles += 1
                                                                            Else
                                                                                noPosBigBottles += 1
                                                                            End If
                                                                            'TR 23/03/2012 -Exit loop to continue with procees and show message not available positions.
                                                                            Exit Do
                                                                        End If
                                                                    End If
                                                                Loop
                                                                If (myGlobalDataTO.HasError) Then Exit For
                                                            Next
                                                            If (myGlobalDataTO.HasError) Then Exit For

                                                            'If at least a Small Bottle could not be positioned: ROTOR is FULL
                                                            If (noPosSmallBottles > 0) Then Exit For
                                                        Else
                                                            'Error getting the list of bottles needed to positioning the needed volume
                                                            Exit For
                                                        End If
                                                    Next

                                                    If (Not myGlobalDataTO.HasError) Then
                                                        'Get the ElementID of all Reagents that could be positioned (those saved in DS myWSRotorContentByPosDS)
                                                        Dim reagentElementIDs As List(Of Integer) = (From a In myWSRotorContentByPosDS.twksWSRotorContentByPosition _
                                                                                                   Select a.ElementID Distinct).ToList()

                                                        Dim myReagentStatus As String
                                                        Dim reagentCells As List(Of WSRotorContentByPositionDS.twksWSRotorContentByPositionRow)
                                                        For Each posReagentID As Integer In reagentElementIDs
                                                            'Calculate the Reagent Status according the real volume of all positioned Reagent bottles
                                                            myGlobalDataTO = myWSRequiredElementsDelegate.CalculateReagentStatus(dbConnection, pAnalyzerID, pRotorType, posReagentID, True)
                                                            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                                                myReagentStatus = CType(myGlobalDataTO.SetDatos, String)

                                                                'Update the Element Status for the Element with the calculated value
                                                                myGlobalDataTO = myWSRequiredElementsDelegate.UpdateStatus(dbConnection, posReagentID, myReagentStatus)
                                                                If (myGlobalDataTO.HasError) Then Exit For

                                                                'Get all Reagent Cells and update field ElementStatus with the calculated value
                                                                reagentCells = (From b As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow In myWSRotorContentByPosDS.twksWSRotorContentByPosition _
                                                                               Where b.ElementID = posReagentID _
                                                                              Select b).ToList

                                                                For Each rotorCell As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow In reagentCells
                                                                    rotorCell.BeginEdit()
                                                                    rotorCell.ElementStatus = myReagentStatus
                                                                    rotorCell.EndEdit()
                                                                Next
                                                            Else
                                                                'Error calculating the Reagent Status...
                                                                Exit For
                                                            End If
                                                        Next
                                                    End If

                                                    If (noPosSmallBottles > 0) Then
                                                        'If the Rotor if FULL, set the proper ErrorCode according if there are NOT IN USE cells in it or not
                                                        If (CType(CountPositionsByStatus(dbConnection, pAnalyzerID, pWorkSessionID, pRotorType, "NO_INUSE").SetDatos, Integer) > 0) Then
                                                            myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.ROTOR_FULL_NOINUSE_CELLS.ToString
                                                        Else
                                                            myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.ROTOR_FULL.ToString
                                                        End If

                                                    ElseIf (noPosBigBottles > 0) Then
                                                        'If the Internal Rotor Ring if FULL, set the proper ErrorCode according if there are NOT IN USE cells in it or not
                                                        If (CType(CountPositionsByStatus(dbConnection, pAnalyzerID, pWorkSessionID, pRotorType, "NO_INUSE", 2).SetDatos, Integer) > 0) Then
                                                            myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.RING2_FULL_NOINUSE_CELLS.ToString
                                                        Else
                                                            myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.RING2_FULL.ToString
                                                        End If
                                                    End If
                                                End If
                                            End If

                                            'Continue only if there is not error nor a warning of Rotor Full or Rotor Full but with Not In Use positions
                                            If (Not myGlobalDataTO.HasError AndAlso String.IsNullOrEmpty(myGlobalDataTO.ErrorCode)) Then
                                                'Execute the auto positioning of Additional Solutions
                                                myGlobalDataTO = AdditionalSolutionsAutoPositioning(dbConnection, pWorkSessionID, pAnalyzerID, pRotorType)
                                                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                                    'Add all returned rows (if any) to DataSet to return (myWSRotorContentByPosDS)
                                                    For Each addSolutionPos As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow In DirectCast(myGlobalDataTO.SetDatos, WSRotorContentByPositionDS).twksWSRotorContentByPosition
                                                        myWSRotorContentByPosDS.twksWSRotorContentByPosition.ImportRow(addSolutionPos)
                                                    Next
                                                End If
                                            End If
                                        End If
                                    End If
                                End If

                                If (Not myGlobalDataTO.HasError) Then
                                    If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                                    myGlobalDataTO.SetDatos = myWSRotorContentByPosDS
                                Else
                                    If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                                    myGlobalDataTO.SetDatos = Nothing
                                End If
                            Else
                                'DL 23/02/2012. Avoid that returns 0 in myGlobalDataTO
                                myGlobalDataTO.SetDatos = Nothing
                            End If
                        End If
                    End If
                End If

                '**** OLD ***'
                '    Dim myTempRCPDS As New WSRotorContentByPositionDS
                '    Dim myWSRotorContentByPosDS As New WSRotorContentByPositionDS

                '    If (pRotorType = "REAGENTS") Then
                '        'TR 02/02/2010 -Validate if there are any required elements not positioned, to start the process
                '        Dim myReqElementsDelegate As New WSRequiredElementsDelegate()
                '        myGlobalDataTO = myReqElementsDelegate.CountNotPositionedElements(dbConnection, pWorkSessionID, pRotorType)

                '        'RH 30/08/2011 Go ahead only if there are not positioned elements
                '        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                '            Dim NotPositionedElements As Integer = CType(myGlobalDataTO.SetDatos, Integer)
                '            If (NotPositionedElements > 0) Then
                '                'Validate if there are any available Position on the rotor.
                '                myGlobalDataTO = CheckAvailablePositionsByRotor(dbConnection, pAnalyzerID, pRotorType, pWorkSessionID)
                '                If (Not myGlobalDataTO.HasError) Then
                '                    'validate if there are any warnings.
                '                    If myGlobalDataTO.ErrorCode = "" Then
                '                        'Search all Reagents required in the WorkSession that has not been still positioned
                '                        Dim myWSRequiredElementsDelegate As New WSRequiredElementsDelegate
                '                        myGlobalDataTO = myWSRequiredElementsDelegate.GetRequiredReagentsDetails(dbConnection, pWorkSessionID, Nothing, "NOPOS")

                '                        If Not myGlobalDataTO.HasError Then
                '                            Dim myRequiredReagentsDS As WSRequiredElementsTreeDS = DirectCast(myGlobalDataTO.SetDatos, WSRequiredElementsTreeDS)

                '                            'Search all Reagents required in the WorkSession that has not been positioned but the total volume is not enough for the WS
                '                            myGlobalDataTO = myWSRequiredElementsDelegate.GetRequiredReagentsDetails(dbConnection, pWorkSessionID, Nothing, "INCOMPLETE")
                '                            If Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing Then
                '                                'Add INCOMPLETE Reagents to the DataSet containing the NOPOS ones
                '                                For Each ReqElementRow As WSRequiredElementsTreeDS.ReagentsRow In CType(myGlobalDataTO.SetDatos, WSRequiredElementsTreeDS).Reagents
                '                                    myRequiredReagentsDS.Reagents.ImportRow(ReqElementRow)
                '                                Next
                '                            End If

                '                            If (myRequiredReagentsDS.Reagents.Rows.Count > 0) Then
                '                                Dim myReagentTubeTypesDelegate As New ReagentTubeTypesDelegate

                '                                'Search minimum volume of "small" bottles
                '                                myGlobalDataTO = myReagentTubeTypesDelegate.GetMinimumBottleSize(dbConnection)

                '                                If Not myGlobalDataTO.HasError Then
                '                                    'Was Minimum Bottle Size found 
                '                                    If Not myGlobalDataTO.SetDatos Is Nothing AndAlso myGlobalDataTO.SetDatos.ToString() <> "" Then
                '                                        'Set the min bottle size value to a variable.
                '                                        Dim minBottleSize As Integer = CType(myGlobalDataTO.SetDatos, Integer)

                '                                        'myGlobalDataTO = New GlobalDataTO
                '                                        Dim myAnalyzerModelRotorsConfigDelegate As New AnalyzerModelRotorsConfigDelegate
                '                                        'Search the maximum Ring Number in the informed Analyzer and Rotor
                '                                        myGlobalDataTO = myAnalyzerModelRotorsConfigDelegate.GetMaxRingNumber(Nothing, pAnalyzerID, pRotorType)

                '                                        If Not myGlobalDataTO.HasError Then
                '                                            'Was the maximum Rotor Ring found ?
                '                                            If Not myGlobalDataTO.SetDatos Is Nothing AndAlso myGlobalDataTO.SetDatos.ToString() <> "" Then
                '                                                Dim myMaxRingNumber As Integer = CType(myGlobalDataTO.SetDatos, Integer)

                '                                                Dim mytwksWSRotorContentByPositionDAO As New twksWSRotorContentByPositionDAO
                '                                                Dim myWSRequiredElementTubesDelegate As New WSRequiredElementsTubesDelegate
                '                                                Dim myRCPRow As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow

                '                                                Dim myReagentTubeTypesDS As ReagentTubeTypesDS

                '                                                Dim referenceCellNumber As Integer
                '                                                Dim bottleNumber As Integer
                '                                                Dim ringNumber As Integer
                '                                                Dim CellNumber As Integer
                '                                                Dim noPositioned As Integer = 0
                '                                                Dim noPositionedInRing2 As Integer = 0

                '                                                'process All not positioned Reagents
                '                                                For Each myReagentRow As WSRequiredElementsTreeDS.ReagentsRow In myRequiredReagentsDS.Reagents.Rows
                '                                                    'Get all the required Bottles for the Reagent
                '                                                    myGlobalDataTO = myWSRequiredElementTubesDelegate.GetAllNeededBottles(dbConnection, myReagentRow.ElementID)

                '                                                    If Not myGlobalDataTO.HasError Then
                '                                                        myReagentTubeTypesDS = CType(myGlobalDataTO.SetDatos, ReagentTubeTypesDS)

                '                                                        'Where Bottles needed for the Reagent found?
                '                                                        If myReagentTubeTypesDS.ReagentTubeTypes.Rows.Count > 0 Then
                '                                                            ''process  all needed Reagent Bottles
                '                                                            'If myReagentRow.ElementStatus = "INCOMPLETE" Then
                '                                                            '    myGlobalDataTO = ProcessReagentPositionedBottle(dbConnection, pAnalyzerID, pWorkSessionID, myReagentRow.ElementID, myReagentTubeTypesDS)
                '                                                            '    If myGlobalDataTO.HasError Then Exit For
                '                                                            'End If

                '                                                            For Each myReagTubeTypeRow As ReagentTubeTypesDS.ReagentTubeTypesRow In myReagentTubeTypesDS.ReagentTubeTypes.Rows
                '                                                                'TR 02/02/2010 -Are there no more Free Positions.
                '                                                                If (noPositioned <= 0) Then
                '                                                                    referenceCellNumber = 0
                '                                                                    bottleNumber = 1
                '                                                                    If (myReagTubeTypeRow.TubeVolume = minBottleSize) Then
                '                                                                        ringNumber = 1
                '                                                                    Else
                '                                                                        ringNumber = myMaxRingNumber
                '                                                                    End If

                '                                                                    'TR 04/02/2010 -Validate if there are bottles of this type to position.
                '                                                                    If myReagTubeTypeRow.NumOfBottles > 0 Then
                '                                                                        'Process all needed Bottles of the current size 
                '                                                                        While (bottleNumber <= myReagTubeTypeRow.NumOfBottles)
                '                                                                            'Search minimum free cell in the informed Ring nearest to the informed cell
                '                                                                            myGlobalDataTO = mytwksWSRotorContentByPositionDAO.GetMinFreeCellByRing(dbConnection, pAnalyzerID, pRotorType, _
                '                                                                                                                                                    pWorkSessionID, ringNumber, referenceCellNumber)
                '                                                                            If (Not myGlobalDataTO.HasError) Then
                '                                                                                'A free Cell was found?
                '                                                                                If (Not myGlobalDataTO.SetDatos Is Nothing AndAlso myGlobalDataTO.SetDatos.ToString() <> "") Then
                '                                                                                    'Set the value to CellNumber Variable.
                '                                                                                    CellNumber = CType(myGlobalDataTO.SetDatos, Integer)
                '                                                                                    myRCPRow = myTempRCPDS.twksWSRotorContentByPosition.NewtwksWSRotorContentByPositionRow

                '                                                                                    myRCPRow.AnalyzerID = pAnalyzerID
                '                                                                                    myRCPRow.RotorType = pRotorType
                '                                                                                    myRCPRow.RingNumber = ringNumber
                '                                                                                    myRCPRow.CellNumber = CellNumber
                '                                                                                    myRCPRow.WorkSessionID = pWorkSessionID
                '                                                                                    myRCPRow.ElementID = myReagentRow.ElementID
                '                                                                                    myRCPRow.MultiTubeNumber = bottleNumber
                '                                                                                    myRCPRow.TubeContent = myReagentRow.TubeContent
                '                                                                                    myRCPRow.TubeType = myReagTubeTypeRow.TubeCode
                '                                                                                    myRCPRow.RealVolume = myReagTubeTypeRow.TubeVolume
                '                                                                                    myRCPRow.Status = "INUSE"

                '                                                                                    'RH 14/09/2011
                '                                                                                    myRCPRow.ScannedPosition = False
                '                                                                                    myRCPRow.SetBarCodeInfoNull()
                '                                                                                    myRCPRow.SetBarcodeStatusNull()

                '                                                                                    'Add the found position to DataSet
                '                                                                                    myTempRCPDS.twksWSRotorContentByPosition.AddtwksWSRotorContentByPositionRow(myRCPRow)

                '                                                                                    referenceCellNumber = CellNumber
                '                                                                                    bottleNumber += 1

                '                                                                                    'Calculate the number of remaining Tests that can be executed with the real volume contained in the positioned bottle
                '                                                                                    myGlobalDataTO = myWSRequiredElementsDelegate.CalculateRemainingTests(dbConnection, pWorkSessionID, _
                '                                                                                                                                                          myReagentRow.ElementID, myReagTubeTypeRow.TubeVolume)
                '                                                                                    If (Not myGlobalDataTO.HasError) Then
                '                                                                                        If (Not myGlobalDataTO.SetDatos Is Nothing AndAlso myGlobalDataTO.SetDatos.ToString() <> "") Then
                '                                                                                            'Set value of field RemainingTests in DataSet
                '                                                                                            myRCPRow.BeginEdit()
                '                                                                                            myRCPRow.RemainingTestsNumber = CType(myGlobalDataTO.SetDatos, Integer)
                '                                                                                            myRCPRow.EndEdit()

                '                                                                                            'Update the Rotor Cell / Position informing the Element positioned in it.  
                '                                                                                            myGlobalDataTO = mytwksWSRotorContentByPositionDAO.Update(dbConnection, myTempRCPDS)
                '                                                                                            If (myGlobalDataTO.HasError) Then
                '                                                                                                If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                '                                                                                                Exit For
                '                                                                                            Else
                '                                                                                                'Import the update row to a Dataset
                '                                                                                                myWSRotorContentByPosDS.twksWSRotorContentByPosition.ImportRow(myRCPRow)

                '                                                                                                'Clear the temporal dataset use to update
                '                                                                                                myTempRCPDS.twksWSRotorContentByPosition.Clear()
                '                                                                                            End If
                '                                                                                        End If
                '                                                                                    End If
                '                                                                                    'TR 27/01/2010 -Add ring number validation for incrementing or breaking the while.
                '                                                                                Else ' a free cell was not found
                '                                                                                    If (ringNumber = 1) Then
                '                                                                                        ringNumber += 1
                '                                                                                    Else
                '                                                                                        'The Reagent bottle cannot be positioned, but no error is Returned because the 
                '                                                                                        'TR 02/01/2010 -Is the no positioned Bottle a small one 
                '                                                                                        If (myReagTubeTypeRow.TubeVolume = minBottleSize) Then
                '                                                                                            'Increment the number of bottles no positioned in Ring 2
                '                                                                                            noPositionedInRing2 += 1
                '                                                                                        Else
                '                                                                                            'Increment the number of bottles no positioned in whatever Ring.
                '                                                                                            noPositioned += 1
                '                                                                                        End If
                '                                                                                        'TR 02/01/2010 END 
                '                                                                                        Exit For
                '                                                                                    End If
                '                                                                                    'TR 27/01/2010 END
                '                                                                                End If
                '                                                                            Else
                '                                                                                'Error searching minimun free cell
                '                                                                                Exit For
                '                                                                            End If
                '                                                                        End While
                '                                                                    End If
                '                                                                End If
                '                                                            Next 'process  all needed Reagent Bottles
                '                                                        Else
                '                                                            'no tube found 
                '                                                            myGlobalDataTO.HasError = True
                '                                                            myGlobalDataTO.ErrorCode = "MASTER_DATA_MISSING"
                '                                                            Exit For
                '                                                        End If
                '                                                    Else
                '                                                        'error searching All needed Bottles
                '                                                        Exit For
                '                                                    End If

                '                                                    'if there was an error then exit the for loop.
                '                                                    If (myGlobalDataTO.HasError) Then
                '                                                        Exit For
                '                                                    End If
                '                                                Next 'process All not positioned Reagents

                '                                                'All positioned Reagents have been processed
                '                                                If (Not myGlobalDataTO.HasError) Then
                '                                                    'Get all distinct Elements id from the dataset
                '                                                    'Use LINQ to get all different Reagents (ElementID) in DataSet toReturn_WSRotorContentBy PositionDS
                '                                                    Dim DistintElementID = (From a In myWSRotorContentByPosDS.twksWSRotorContentByPosition _
                '                                                                          Select a.ElementID).Distinct()

                '                                                    Dim myReagentStatus As String = ""
                '                                                    Dim ElementIDDist As Integer = 0

                '                                                    For Each ElementIDDist In DistintElementID
                '                                                        'Calculate the Reagent Status according the real volume of all positioned Reagent bottles
                '                                                        myGlobalDataTO = myWSRequiredElementsDelegate.CalculateReagentStatus(dbConnection, pAnalyzerID, pRotorType, ElementIDDist, True)

                '                                                        If (Not myGlobalDataTO.HasError) Then
                '                                                            If (Not myGlobalDataTO.SetDatos Is Nothing AndAlso myGlobalDataTO.SetDatos.ToString() <> "") Then
                '                                                                myReagentStatus = CType(myGlobalDataTO.SetDatos, String)
                '                                                                'Update Status of the Required Element
                '                                                                myGlobalDataTO = myWSRequiredElementsDelegate.UpdateStatus(dbConnection, ElementIDDist, myReagentStatus)
                '                                                                If (myGlobalDataTO.HasError) Then
                '                                                                    Exit For
                '                                                                Else
                '                                                                    'from local dataset update the status
                '                                                                    Dim updateStatus = From a In myWSRotorContentByPosDS.twksWSRotorContentByPosition _
                '                                                                                       Where a.ElementID = ElementIDDist _
                '                                                                                       Select a

                '                                                                    'Set field ElementStatus = POS
                '                                                                    For Each UpdateRow As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow In updateStatus
                '                                                                        UpdateRow.BeginEdit()
                '                                                                        UpdateRow.ElementStatus = "POS"
                '                                                                        UpdateRow.EndEdit()
                '                                                                    Next
                '                                                                End If
                '                                                            End If
                '                                                        End If

                '                                                    Next

                '                                                    'TR 02/02/2010 -At least one Reagent in small bottle could not be positioned. 
                '                                                    If (noPositioned > 0) Then
                '                                                        If (CType(CountPositionsByStatus(dbConnection, pAnalyzerID, pWorkSessionID, pRotorType, "NO_INUSE").SetDatos, Integer) > 0) Then
                '                                                            myGlobalDataTO.ErrorCode = "ROTOR_FULL_NOINUSE_CELLS"
                '                                                        Else
                '                                                            myGlobalDataTO.ErrorCode = "ROTOR_FULL"
                '                                                        End If
                '                                                    Else
                '                                                        'TR 02/02/2010 -At least one Reagent in big bottle could not be positioned in Ring 2
                '                                                        If (noPositionedInRing2 > 0) Then
                '                                                            'Verify if there are not in use positions in Ring2
                '                                                            If (CType(CountPositionsByStatus(dbConnection, pAnalyzerID, pWorkSessionID, pRotorType, "NO_INUSE", 2).SetDatos, Integer) > 0) Then
                '                                                                myGlobalDataTO.ErrorCode = "ROTOR_FULL_NOINUSE_CELLS"
                '                                                            Else
                '                                                                myGlobalDataTO.ErrorCode = "RING2_FULL"
                '                                                            End If
                '                                                        End If
                '                                                    End If
                '                                                Else
                '                                                    If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                '                                                End If
                '                                            Else
                '                                                myGlobalDataTO.HasError = True
                '                                                myGlobalDataTO.ErrorCode = "MASTER_DATA_MISSING"

                '                                                'RH 30/08/2011 This line is a bug (Exit Try). If you exit Try here,
                '                                                'the Commit or Rollback Transaction code block is never reached.
                '                                                'Exit Try
                '                                            End If
                '                                        Else
                '                                            'error searching MaxRingNumber

                '                                            'RH 30/08/2011 This line is a bug (Exit Try). If you exit Try here,
                '                                            'the Commit or Rollback Transaction code block is never reached.
                '                                            'Instead, say an error has taken placed (myGlobalDataTO.HasError = True).
                '                                            'BTW, here myGlobalDataTO.HasError = True
                '                                            'Exit Try
                '                                        End If
                '                                    End If
                '                                End If

                '                            End If

                '                            'TR 02/02/2010 -validate if there was any error to continue
                '                            If (Not myGlobalDataTO.HasError AndAlso myGlobalDataTO.ErrorCode = "") Then
                '                                Dim AdditionalSolGlobalDataTO As GlobalDataTO

                '                                'Execute the auto positioning of Additional Solutions
                '                                AdditionalSolGlobalDataTO = AdditionalSolutionsAutoPositioning(dbConnection, pWorkSessionID, pAnalyzerID, pRotorType)

                '                                If (Not AdditionalSolGlobalDataTO.HasError) Then
                '                                    For Each AddSolPosRow As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow _
                '                                                          In CType(AdditionalSolGlobalDataTO.SetDatos, WSRotorContentByPositionDS).twksWSRotorContentByPosition
                '                                        'Add all returned rows (if any) to DataSet toReturn_WSRotorContentByPositionDS 
                '                                        myWSRotorContentByPosDS.twksWSRotorContentByPosition.ImportRow(AddSolPosRow)
                '                                    Next

                '                                    'If every thing was OK then COMMIT Transacction.
                '                                    If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                '                                    myGlobalDataTO.SetDatos = myWSRotorContentByPosDS
                '                                    If AdditionalSolGlobalDataTO.ErrorCode <> "" Then
                '                                        myGlobalDataTO.ErrorCode = AdditionalSolGlobalDataTO.ErrorCode
                '                                    End If
                '                                Else
                '                                    'If every thing was OK then rollback Transacction.
                '                                    If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                '                                End If
                '                            Else
                '                                If Not myGlobalDataTO.HasError Then
                '                                    If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                '                                    myGlobalDataTO.SetDatos = myWSRotorContentByPosDS
                '                                Else
                '                                    If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                '                                End If
                '                            End If
                '                            'TR 02/02/2010 END.
                '                        End If
                '                    Else
                '                        'SA 22/09/2011 There is not Element to be positioned
                '                        myGlobalDataTO.SetDatos = Nothing
                '                    End If
                '                End If
                '            Else
                '                'RH 30/08/2011 There is no element to be positioned.
                '                myGlobalDataTO.SetDatos = Nothing
                '            End If
                '        End If
                '    Else
                '        'TODO: PENDING: Process for A200 Rotor for Samples and Reagents
                '    End If
                '    End If
                'End If
            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                myGlobalDataTO.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSRotorContentByPositionDelegate.ReagentsAutoPositioning", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Clear (or delete) all positions of the specified Analyzer Rotor
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pRotorType">Rotor Type</param>
        ''' <returns>GlobalDataTO containing a typed DataSet WSRotorContentByPositionDS with all Elements that were 
        '''          positioned in the reseted Rotor with the updated ElementStatus to NOPOS</returns>
        ''' <remarks>
        ''' Created by:  BK 08/12/2009 - Tested: OK
        ''' Modified by: SA 13/01/2010 - Changed the way of opening the DB Transaction to fulfill the new template
        '''              AG 21/01/2010 - Reset the not in use rotor positions table (Tested OK)
        ''' </remarks>
        Public Function ResetRotor(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
                                   ByVal pRotorType As String, ByVal pWorkSessionID As String) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = CType(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myRequiredElements As New WSRequiredElementsDelegate
                        Dim myRotorContentsByPosition As New twksWSRotorContentByPositionDAO
                        myGlobalDataTO = myRequiredElements.UpdateStatusByResetRotor(dbConnection, pAnalyzerID, pRotorType, pWorkSessionID)

                        If (Not myGlobalDataTO.HasError) Then
                            'Obtain the Rotor Position status in a Dataset
                            myResultData = myRotorContentsByPosition.GetAllPositionedElementsToReset(dbConnection, pAnalyzerID, pRotorType)

                            'Reset the Rotor Position
                            myRotorContentsByPosition.ResetRotor(dbConnection, pAnalyzerID, pRotorType, pWorkSessionID)

                            'AG 21/01/2010
                            Dim myNoInUseDeleg As New WSNotInUseRotorPositionsDelegate
                            myNoInUseDeleg.Reset(dbConnection, pAnalyzerID, pWorkSessionID, pRotorType)
                            'END AG 21/01/2010


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

                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSRotorContentByPositionDelegate.ResetRotor", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myResultData
        End Function

        ''' <summary>
        ''' Delete all Rotor Positions for the specified Analyzer and Work Session
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by: GDS 21/04/2010
        ''' </remarks>
        Public Function ResetWS(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
                                ByVal pWorkSessionID As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError) And (Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksWSRotorContentByPositionDAO
                        resultData = myDAO.ResetWS(dbConnection, pAnalyzerID, pWorkSessionID)

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
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSRotorContentByPositionDelegate.ResetWS", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Manages the automatic positioning of Controls, Calibrators and Patient Samples. 
        ''' For each different type of Samples, the non positioned Elements of that type are 
        ''' positioned according the specific rules of each type.  
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pWorkSessionID">Work Session Identification</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pRotorType">Rotor Type</param>
        ''' <returns>GlobalDataTO.SetData = typed DataSet WSRotorContentByPositionDS containing information 
        '''          of all positioned Samples</returns>
        ''' <remarks>
        ''' Created by: TR 03/12/2009
        ''' Modified by: SA 13/01/2010 - Changed the way of opening the DB Transaction to fulfill the new template
        '''              TR 21/01/2010 - When calling the function to get not positioned Calibrators and Patient Samples, inform the opened Connection 
        '''              SA 26/01/2010 - Changes to get the max Ring Number for the Analyzer Rotor and pass it as parameter to each one 
        '''                              of the specific positioning functions (to improve the performance of positioning)
        '''              TR 29/01/2010 - Validate if there are required Elements pending to positioning before start the process; verify if there is 
        '''                              at least a Multipoint Calibrator that could not be positioned when there are enough NOT IN USE CELLS in the Rotor
        '''              RH 16/06/2011 - Get not positioned Tube Additional Solutions. Code optimization.
        '''              AG 11/07/2011 - Get not positioned elements in the following order: Additional Solutions, Calibrators, Controls, 
        '''                              and Patient Samples
        '''              SA 11/01/2012 - Code improved
        '''              SA 07/02/2012 - Changed called to function GetRequiredPatientSamplesElements due to parameter pExcludedDepleted was removed 
        ''' </remarks>
        Public Function SamplesAutoPositioning(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, ByVal pAnalyzerID As String, ByVal pRotorType As String) _
                                               As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myResult As New WSRotorContentByPositionDS
                        Dim notPositionedCalibrators As Boolean = False
                        Dim myReqElementsDelegate As New WSRequiredElementsDelegate()

                        myGlobalDataTO = myReqElementsDelegate.CountNotPositionedElements(dbConnection, pWorkSessionID, pRotorType)
                        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                            If (CType(myGlobalDataTO.SetDatos, Integer) > 0) Then
                                'Validate if there are free Rotor Positions
                                myGlobalDataTO = CheckAvailablePositionsByRotor(dbConnection, pAnalyzerID, pRotorType, pWorkSessionID)
                                If (Not myGlobalDataTO.HasError) Then
                                    'Continue only if there is not a warning of Rotor Full or Rotor Full but with Not In Use positions
                                    If (String.IsNullOrEmpty(myGlobalDataTO.ErrorCode)) Then
                                        Dim maxRingNumber As Integer = -1
                                        Dim analyzerModelRotorsConfig As New AnalyzerModelRotorsConfigDelegate

                                        'Get the maximum Ring Number for the Analyzer Rotor
                                        myGlobalDataTO = analyzerModelRotorsConfig.GetMaxRingNumber(dbConnection, pAnalyzerID, pRotorType)
                                        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                            maxRingNumber = CType(myGlobalDataTO.SetDatos, Integer)

                                            Dim myRequiredSamples As WSRequiredElementsTreeDS
                                            Dim myRequiredElementDelegate As New WSRequiredElementsDelegate

                                            'Search all not finished SAMPLE Additional Solutions in the Work Session that are not positioned in the Rotor 
                                            myGlobalDataTO = myRequiredElementDelegate.GetRequiredTubeSolutionsDetails(dbConnection, pWorkSessionID, Nothing, "NOPOS")
                                            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                                myRequiredSamples = DirectCast(myGlobalDataTO.SetDatos, WSRequiredElementsTreeDS)

                                                'Are there some SAMPLE Additional Solutions to process?
                                                If (myRequiredSamples.TubeAdditionalSolutions.Rows.Count > 0) Then

                                                    myGlobalDataTO = ProcessTubeAdditionalSolutionsForAutoPositioning(dbConnection, myRequiredSamples, pAnalyzerID, pRotorType, _
                                                                                                                      pWorkSessionID, maxRingNumber)
                                                    If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                                                        For Each row As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow _
                                                                     In DirectCast(myGlobalDataTO.SetDatos, WSRotorContentByPositionDS).twksWSRotorContentByPosition
                                                            'All positioned SAMPLE Additional Solutions are moved to the final DataSet
                                                            myResult.twksWSRotorContentByPosition.ImportRow(row)
                                                        Next
                                                    End If
                                                End If
                                            End If

                                            'Continue only if there is not error nor a warning of Rotor Full or Rotor Full but with Not In Use positions
                                            If (Not myGlobalDataTO.HasError AndAlso String.IsNullOrEmpty(myGlobalDataTO.ErrorCode)) Then
                                                'Search all not finished Calibrators in the Work Session that are not positioned in the Rotor 
                                                myGlobalDataTO = myRequiredElementDelegate.GetRequiredCalibratorsDetails(dbConnection, pWorkSessionID, 0, "NOPOS")
                                                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                                    myRequiredSamples = DirectCast(myGlobalDataTO.SetDatos, WSRequiredElementsTreeDS)

                                                    'Are there Calibrators to process?
                                                    If (myRequiredSamples.Calibrators.Rows.Count > 0) Then
                                                        myGlobalDataTO = ProcessCalibratorsForAutoPositioning(dbConnection, myRequiredSamples, pAnalyzerID, pRotorType, _
                                                                                                              pWorkSessionID, maxRingNumber, notPositionedCalibrators)
                                                        If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                                                            For Each row As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow _
                                                                         In DirectCast(myGlobalDataTO.SetDatos, WSRotorContentByPositionDS).twksWSRotorContentByPosition
                                                                'All positioned Calibrators are moved to the final DataSet
                                                                myResult.twksWSRotorContentByPosition.ImportRow(row)
                                                            Next
                                                        End If
                                                    End If
                                                End If
                                            End If

                                            'Continue only if there is not error nor a warning of Rotor Full or Rotor Full but with Not In Use positions
                                            If (Not myGlobalDataTO.HasError AndAlso String.IsNullOrEmpty(myGlobalDataTO.ErrorCode)) Then
                                                'Search all not finished Controls in the Work Session that are not positioned in the Rotor 
                                                myGlobalDataTO = myRequiredElementDelegate.GetRequiredControlsDetails(dbConnection, pWorkSessionID, 0, "NOPOS")
                                                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                                    myRequiredSamples = DirectCast(myGlobalDataTO.SetDatos, WSRequiredElementsTreeDS)

                                                    'Are there Controls to process?
                                                    If (myRequiredSamples.Controls.Rows.Count > 0) Then
                                                        myGlobalDataTO = ProcessControlsForAutoPositioning(dbConnection, myRequiredSamples, pAnalyzerID, pRotorType, _
                                                                                                           pWorkSessionID, maxRingNumber)
                                                        If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                                                            For Each row As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow _
                                                                         In DirectCast(myGlobalDataTO.SetDatos, WSRotorContentByPositionDS).twksWSRotorContentByPosition
                                                                'All positioned Controls are moved to the final DataSet
                                                                myResult.twksWSRotorContentByPosition.ImportRow(row)
                                                            Next
                                                        End If
                                                    End If
                                                End If
                                            End If

                                            'Continue only if there is not error nor a warning of Rotor Full or Rotor Full but with Not In Use positions
                                            If (Not myGlobalDataTO.HasError AndAlso String.IsNullOrEmpty(myGlobalDataTO.ErrorCode)) Then
                                                'Search all not finished and not depleted Patient Samples in the WorkSession that are not positioned in the Rotor
                                                myGlobalDataTO = myRequiredElementDelegate.GetRequiredPatientSamplesDetails(dbConnection, pWorkSessionID, 0, "NOPOS")
                                                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                                    myRequiredSamples = DirectCast(myGlobalDataTO.SetDatos, WSRequiredElementsTreeDS)

                                                    'Are there no positioned Patient Samples to process?
                                                    If (myRequiredSamples.PatientSamples.Rows.Count > 0) Then
                                                        myGlobalDataTO = ProcessPatientsForAutoPositioning(dbConnection, myRequiredSamples, pAnalyzerID, pRotorType, _
                                                                                                           pWorkSessionID, maxRingNumber)
                                                        If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                                                            For Each row As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow _
                                                                         In DirectCast(myGlobalDataTO.SetDatos, WSRotorContentByPositionDS).twksWSRotorContentByPosition
                                                                'All positioned Patient Samples are moved to the final DataSet
                                                                myResult.twksWSRotorContentByPosition.ImportRow(row)
                                                            Next
                                                        End If
                                                    End If
                                                End If
                                            End If
                                        End If
                                    End If

                                    If (Not myGlobalDataTO.HasError) Then
                                        'Verify if there is at least a Multipoint Calibrator that could not be positioned when there are enough 
                                        'NOT IN USE CELLS in the Rotor....
                                        If (notPositionedCalibrators) Then
                                            If (CType(CountPositionsByStatus(dbConnection, pAnalyzerID, pWorkSessionID, pRotorType, "NO_INUSE").SetDatos, Integer) > 0) Then
                                                myGlobalDataTO.ErrorCode = "ROTOR_FULL_NOINUSE_CELLS"
                                            Else
                                                myGlobalDataTO.ErrorCode = "ROTOR_FULL"
                                            End If
                                        End If
                                        myGlobalDataTO.SetDatos = myResult

                                        If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                                    Else
                                        If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                                    End If
                                End If
                            Else
                                myGlobalDataTO.SetDatos = Nothing
                            End If
                        End If
                    End If
                End If
            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                myGlobalDataTO.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSRotorContentByPositionDelegate.SamplesAutoPositioning", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Receive as entry a rotor position and rotor type for an Work Session and seach for other positions (INUSE)
        ''' for the same ElementID as the position in parameter
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">WorkSession Identifier</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pRotorType">Type of Rotor</param>
        ''' <param name="pCellNumber">Rotor Position</param>
        ''' <returns>GlobalDataTO containing a typed DataSet WSRotorContentByPositionDS</returns>
        ''' <remarks>
        ''' Created by:  AG 30/03/2011 - Based on ExistOtherPosition
        ''' Modified by: AG 13/06/2014 - BT #1662 ==> Protection against change positions in pause when S / R2 arm still have not been finished with this position
        ''' </remarks>
        Public Function SearchOtherPosition(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, ByVal pAnalyzerID As String, _
                                            ByVal pRotorType As String, ByVal pCellNumber As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Get the ElementID placed in the informed Rotor Position
                        resultData = ReadByRotorTypeAndCellNumber(dbConnection, pRotorType, pCellNumber, pWorkSessionID, pAnalyzerID)
                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            Dim myWSRotorContentByPositionDS As WSRotorContentByPositionDS = DirectCast(resultData.SetDatos, WSRotorContentByPositionDS)

                            'Search if the Element is placed in another position...
                            Dim myElementID As Integer = 0
                            'AG 13/06/2014 - #1662
                            'If (Not myWSRotorContentByPositionDS.twksWSRotorContentByPosition.First.IsElementIDNull) Then
                            If (myWSRotorContentByPositionDS.twksWSRotorContentByPosition.Rows.Count > 0 AndAlso Not myWSRotorContentByPositionDS.twksWSRotorContentByPosition.First.IsElementIDNull) Then
                                myElementID = myWSRotorContentByPositionDS.twksWSRotorContentByPosition.First.ElementID
                            End If
                            resultData = ExistOtherPosition(dbConnection, myElementID, pWorkSessionID, pAnalyzerID)
                        End If
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSRotorContentByPositionDelegate.SearchOtherPosition", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Set scanned position to free status.
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  26/04/2013
        '''</remarks>
        Public Function SetScannedPositionToFREE(ByVal pDBConnection As SqlClient.SqlConnection, _
                                                 ByVal pAnalyzerID As String, ByVal pWorkSessionID As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing
            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError) AndAlso (Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksWSRotorContentByPositionDAO
                        resultData = myDAO.SetScannedPositionToFREE(dbConnection, pAnalyzerID, pWorkSessionID)

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
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSRotorContentByPositionDelegate.SetScannedPositionToFREE", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Update ScannedPosition and the rest of Barcode fields of a Rotor position with the data read from Analyzer (BarCodeInfo and BarCodeStatus)
        ''' When parameter pAdditionalFieldsFlag is TRUE: 
        '''  ** Fields ElementID, MultiTubeNumber and TubeType are also informed
        '''  ** Fields RealVolume and Status are also informed but only when they are informed in the entry DS (that is to say, they are not set to NULL)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pBarCodeDS">Typed DataSet WSRotorContentByPositionDS with information of the Rotor position to update</param>
        ''' <param name="pAdditionalFieldsFlag">When FALSE, only Barcode fields are updated (BarcodeStatus, BarcodeInfo, ScannedPosition)
        '''                                     When TRUE, besides Barcode fields, update also fields ElementID, MultiTubeNumber, TubeType, 
        '''                                     RealVolume and Status)</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  AG 26/06/2011 
        ''' Modified by: AG 07/10/2014 - BA-1979 ==>  Added a call to new auxiliary function CheckForInvalidPosition to check if there are FREE or NOT IN USE Rotor 
        '''                                           Positions with incomplete data (TubeContent without ID of the corresponding element)
        ''' </remarks>
        Public Function UpdateBarCodeFields(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pRotorType As String, ByVal pBarcodeDS As WSRotorContentByPositionDS, _
                                            ByVal pAdditionalFieldsFlag As Boolean) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing
            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError) AndAlso (Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'BA-1979: Check if there are FREE or NOT IN USE Rotor Positions with incomplete data (TubeContent without ID of the element) 
                        resultData = CheckForInvalidPosition(dbConnection, pRotorType, pBarcodeDS, ClassCalledFrom.UpdateBarcodeFields)

                        If (Not resultData.HasError) Then
                            Dim myDAO As New twksWSRotorContentByPositionDAO
                            resultData = myDAO.UpdateBarCodeFields(dbConnection, pBarcodeDS, pAdditionalFieldsFlag)
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
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSRotorContentByPositionDelegate.UpdateBarCodeFields", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Update the Status of the specified Rotor Position and, if parameter pUpdateOnlyStatusFlag=FALSE, update also fields
        ''' RealVolume and RemainingTestsNumber
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID" >Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pRotorType">Rotor Type: SAMPLES or REAGENTS</param>
        ''' <param name="pCellNumber">Cell Number</param>
        ''' <param name="pStatus">Status to set to the Rotor Cell</param>
        ''' <param name="pRealVolume">Real Volume of the Reagent bottle placed in the Rotor Position</param>
        ''' <param name="pTestLeft">Number of Tests that can be executed with the current real Volume of the Reagent bottle</param>
        ''' <param name="pOnlyUpdateFlag">When FALSE, indicates that besides updating the Rotor Position, this function will search the 
        '''                               ElementID of the positioned Reagent and calculate the Element Status</param>
        ''' <param name="pUpdateOnlyStatusFlag">When TRUE, indicates that only the Status field has to be updated for the Rotor Position
        '''                                     (fields RealVolume and RemainingTestsNumber are not changed)</param>
        ''' <returns>GlobalDataTO containing a typed DataSet WSRotorContentByPositionDS with values of the update
        '''          Rotor Position</returns>
        ''' <remarks>
        ''' Created by:  DL 04/01/2011 
        ''' Modified by: AG 30/03/2011 - Added parameters for Real Volume and Number of Remaining Tests 
        '''              AG 06/04/2011 - Changed the function template, the used one was wrong
        '''              AG 12/04/2011 - Added calculation and update of ElementStatus field for the positioned ElementID
        '''              AG 05/07/2011 - Added parameter pOnlyUpdateFlag
        '''              AG 26/07/2011 - Added parameter pOnlyUpdateStatusFlag
        '''              SA 06/03/2012 - Changed the way of calculate and update the ElementStatus: instead of using function DeletePositions,
        '''                              call function CalculateReagentStaus and then UpdateStatus, both from class WSRequiredElementsDelegate
        '''              SA 07/03/2012 - Return a typed DS WSRotorContentByPositionDS containing data of the updated Rotor Position
        ''' </remarks>
        Public Function UpdateByRotorTypeAndCellNumber(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
                                                       ByVal pWorkSessionID As String, ByVal pRotorType As String, ByVal pCellNumber As Integer, _
                                                       ByVal pStatus As String, ByVal pRealVolume As Single, ByVal pTestLeft As Integer, _
                                                       ByVal pOnlyUpdateFlag As Boolean, ByVal pUpdateOnlyStatusFlag As Boolean) As GlobalDataTO

            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing
            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim rotorContentsDS As New WSRotorContentByPositionDS

                        Dim myDAO As New twksWSRotorContentByPositionDAO
                        resultData = myDAO.UpdateByRotorTypeAndCellNumber(dbConnection, pAnalyzerID, pWorkSessionID, pRotorType, pCellNumber, _
                                                                          pStatus, pRealVolume, pTestLeft, pUpdateOnlyStatusFlag)
                        If (Not resultData.HasError) Then
                            'Calculate and update the new ElementStatus for the required Reagent
                            'Used when: Barcode ERROR is read (pStatus = LOCKED), volume missing, ...
                            If (Not pOnlyUpdateFlag) Then
                                'Search the ElementID for the bottle placed in the Rotor Cell
                                resultData = myDAO.ReadByRotorTypeAndCellNumber(dbConnection, pRotorType, pCellNumber, pWorkSessionID, pAnalyzerID)
                                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                    rotorContentsDS = DirectCast(resultData.SetDatos, WSRotorContentByPositionDS)

                                    If rotorContentsDS.twksWSRotorContentByPosition.Rows.Count > 0 Then 'AG 08/01/2014 - add this line to avoid exception (esporadic)
                                        'Calculate the Reagent Status 
                                        Dim myReqElementsDelegate As New WSRequiredElementsDelegate
                                        resultData = myReqElementsDelegate.CalculateReagentStatus(dbConnection, pAnalyzerID, pRotorType, rotorContentsDS.twksWSRotorContentByPosition.First.ElementID, True)

                                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                            Dim reagentStatus As String = CType(resultData.SetDatos, String)

                                            'Update Reagent Status in the DS to return 
                                            rotorContentsDS.twksWSRotorContentByPosition.First.ElementStatus = reagentStatus

                                            'Update the Reagent Status with the calculated value
                                            resultData = myReqElementsDelegate.UpdateStatus(dbConnection, rotorContentsDS.twksWSRotorContentByPosition.First.ElementID, reagentStatus)
                                        End If
                                        'resultData = DeletePositions(dbConnection, rotorContentsDS, True)
                                    End If
                                End If
                            End If
                        End If

                        If (Not resultData.HasError) Then
                            'When the Database Connection was opened locally, then the Commit is executed
                            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                            resultData.SetDatos = rotorContentsDS
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
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSRotorContentByPositionDelegate.UpdateByRotorTypeAndCellNumber", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' For Reagents and Additional Solutions, when there are more than one bottle of the Element positioned 
        ''' in an Analyzer Rotor, recalculate the tube numbers of all bottles after removing at least one of them 
        ''' from the Rotor
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalizerID">Analyzer Identifier</param>
        ''' <param name="pRotorType">Rotor Type</param>
        ''' <param name="pElementID">Element Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet WSRotorContentByPositionDS with positions of the bottles 
        '''          of the Element placed in the Analyzer Rotor with the updated MutiTubeNumber
        ''' </returns>
        ''' <remarks>
        ''' Created by:  TR 20/11/2009 - Tested: OK
        ''' Modified by: SA 13/01/2010 - Changed the way of opening the DB Transaction to fulfill the new template
        '''              XB 07/10/2014 - BA-1978 ==> Add log traces to catch NULL wrong assignment on RealVolume field
        '''              AG 07/10/2014 - BA-1979 ==> Replaced call to function twksWSRotorContentByPositionDAO.Update by a call to function Update 
        '''                                          in this Delegate, informing the process who is updating the content of the Rotor Position (to 
        '''                                          search which process is inserting invalid values: positions with TubeContent but not element ID)
        ''' </remarks>
        Public Function UpdateMultitubeNumber(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalizerID As String, _
                                              ByVal pRotorType As String, ByVal pElementID As Integer) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myRotorContentByPosition As New twksWSRotorContentByPositionDAO

                        myGlobalDataTO = myRotorContentByPosition.ReadByElementIDList(dbConnection, pAnalizerID, pRotorType, pElementID.ToString())
                        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                            Dim myWSRotorContentByPositionDS As WSRotorContentByPositionDS = DirectCast(myGlobalDataTO.SetDatos, WSRotorContentByPositionDS)

                            'Update the multitube number
                            Dim newMultitubeNumber As Integer = 1
                            For Each wsRotorRow As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow _
                                                In myWSRotorContentByPositionDS.twksWSRotorContentByPosition.Rows
                                wsRotorRow.BeginEdit()
                                wsRotorRow.MultiTubeNumber = CType(newMultitubeNumber, Integer)
                                wsRotorRow.EndEdit()

                                newMultitubeNumber += 1
                            Next

                            'myGlobalDataTO = myRotorContentByPosition.Update(dbConnection, myWSRotorContentByPositionDS)
                            myGlobalDataTO = Update(dbConnection, pRotorType, myWSRotorContentByPositionDS, ClassCalledFrom.UpdateMultiTubeNumber) 'AG 07/10/2014 BA-1979 call the Update method  in delegate instead of in DAO

                            If (Not myGlobalDataTO.HasError) Then
                                'When the Database Connection was opened locally, then the Commit is executed
                                If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                                myGlobalDataTO.SetDatos = myWSRotorContentByPositionDS
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
                myGlobalDataTO.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSRotorContentByPositionDelegate.UpdateMultitubeNumber", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Update values of  group of Not In Use Positions when a required WorkSession Element is found for them.  
        ''' Fields ElementID and Status are updated, and the Rotor Position is removed from the table of Not In Use positions
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pNotInUsePositionDS">DataSet containing information of all Not In Use Rotor Positions</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>        
        ''' Created by:  VR 25/01/2010 - (Tested : OK) Tested for REAGENTS flow
        ''' Modified by: SA 26/01/2010 - Error fixed. The Delete should be executed from the table of Not In Use Rotor Positions
        '''                              (calling function in WSNotInUseRotorPositionDelegate, no in twksWSRotorContentByPositionDAO)
        ''' </remarks>
        Public Function UpdateNotInUseRotorPosition(ByVal pDBConnection As SqlClient.SqlConnection, _
                                                    ByVal pNotInUsePositionDS As WSRotorContentByPositionDS) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection
            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        If (Not pNotInUsePositionDS Is Nothing) Then
                            'Update values of ElementID and Status in the Rotor Position. 
                            Dim myRotorContentByPositionDAO As New twksWSRotorContentByPositionDAO
                            myGlobalDataTO = myRotorContentByPositionDAO.UpdateNotInUseRotorPosition(dbConnection, pNotInUsePositionDS)

                            If (Not myGlobalDataTO.HasError) Then
                                'Delete the Rotor Positions from the table that store information of cells with Status Not In Use
                                Dim myWSNotInUseRotorPosition As New WSNotInUseRotorPositionsDelegate
                                myGlobalDataTO = myWSNotInUseRotorPosition.Delete(dbConnection, pNotInUsePositionDS)
                            End If

                            If Not (myGlobalDataTO.HasError) Then
                                If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                            Else
                                myGlobalDataTO.HasError = True
                                If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                            End If
                        End If
                    End If
                End If

            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSRotorContentByPositionDelegate.UpdateNotInUseRotorPosition", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' When a Reagent is positioned in an Analyzer Rotor, calculate the number of remaining Tests  according the 
        ''' real volume of the Reagent positioned in an Analyzer Rotor. Update the Rotor Position information.
        ''' Calculate if the total volume placed in the Rotor is enough to execute all the Test in the Work Session and 
        ''' according the result updates the status of the required Reagent Element
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWSRotorContentByPositionDS">Typed DataSet containing information of the Ring and Cell in which 
        '''                                           a Reagent is positioned in an Analyzer Rotor.</param>
        ''' <returns>GlobalDataTO containing a typed DataSet WSRotorContentByPositionDS with the entry information with 
        '''          fields RemainingTests, Status and ElementStatus updated with the calculated values</returns>
        ''' <remarks>
        ''' Created by:  TR 26/11/2009 - Tested: OK 18/12/2009
        ''' Modified by: AG 05/01/2010 - Return a not empty DataSet (Tested: OK)
        '''              SA 13/01/2010 - Changed the way of opening the DB Transaction to fulfill the new template
        '''                              Error fixed: Commit/Rollback was executed only for the first record: FOR was removed
        '''                              (the entry DS contains only a row)
        '''              RH 31/08/2011 - Code optimization. short-circuit evaluation. Remove unneeded and memory wasting "New" instructions.
        '''              SA 15/02/2012 - Function updated due to changes in function CalculateReagentStatus
        '''              SA 02/03/2012 - Informed parameter for the BottleType with calling function CalculateRemainingTests
        '''              XB 07/10/2014 - BA-1978 ==> Add log traces to catch NULL wrong assignment on RealVolume field
        '''              AG 07/10/2014 - BA-1979 ==> Replaced call to function twksWSRotorContentByPositionDAO.Update by a call to function Update 
        '''                                          in this Delegate, informing the process who is updating the content of the Rotor Position (to 
        '''                                          search which process is inserting invalid values: positions with TubeContent but not element ID)
        ''' </remarks>
        Public Function UpdateReagentPosition(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWSRotorContentByPositionDS As WSRotorContentByPositionDS) _
                                              As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim rotorContenPositionRow As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow = pWSRotorContentByPositionDS.twksWSRotorContentByPosition(0)

                        If (Not rotorContenPositionRow.IsElementIDNull) Then
                            'Calculate the number of remaining Tests that can be executed with the real volume contained in the positioned bottle
                            Dim myRequiredElementsDelegate As New WSRequiredElementsDelegate
                            myGlobalDataTO = myRequiredElementsDelegate.CalculateRemainingTests(dbConnection, rotorContenPositionRow.WorkSessionID, rotorContenPositionRow.ElementID, _
                                                                                                rotorContenPositionRow.RealVolume, rotorContenPositionRow.TubeType)
                            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                If (String.Compare(myGlobalDataTO.SetDatos.ToString(), "", False) <> 0) Then
                                    'Dim myGlobalbase As New GlobalBase

                                    'Set value of field RemainingTests in DataSet 
                                    rotorContenPositionRow.BeginEdit()
                                    rotorContenPositionRow.RemainingTestsNumber = CType(myGlobalDataTO.SetDatos, Integer)
                                    rotorContenPositionRow.TS_User = GlobalBase.GetSessionInfo.UserName
                                    rotorContenPositionRow.TS_DateTime = DateTime.Now
                                    rotorContenPositionRow.EndEdit()
                                End If

                                'Update the Rotor Cell / Position informing the Element positioned in it
                                'Dim myRotorContentByPositionDAO As New twksWSRotorContentByPositionDAO
                                'myGlobalDataTO = myRotorContentByPositionDAO.Update(dbConnection, pWSRotorContentByPositionDS)
                                myGlobalDataTO = Update(dbConnection, "REAGENTS", pWSRotorContentByPositionDS, ClassCalledFrom.ReagentPositioning) 'AG 07/10/2014 BA-1979 call the Update method  in delegate instead of in DAO

                                If (Not myGlobalDataTO.HasError) Then
                                    'Fill needed data in a row of WSRequiredElementsDS 
                                    Dim myWSRequiredElementDS As New WSRequiredElementsDS
                                    Dim myElementRow As WSRequiredElementsDS.twksWSRequiredElementsRow = myWSRequiredElementDS.twksWSRequiredElements.NewtwksWSRequiredElementsRow

                                    myElementRow.BeginEdit()
                                    myElementRow.WorkSessionID = rotorContenPositionRow.WorkSessionID
                                    myElementRow.ElementID = rotorContenPositionRow.ElementID
                                    myElementRow.RequiredVolume = 0
                                    myElementRow.EndEdit()

                                    'Calculate the Reagent Status according the real volume of all positioned Reagent Bottles
                                    myGlobalDataTO = myRequiredElementsDelegate.CalculateNeededBottlesAndReagentStatus(dbConnection, rotorContenPositionRow.AnalyzerID, myElementRow, 0)
                                    If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                        myElementRow = DirectCast(myGlobalDataTO.SetDatos, WSRequiredElementsDS.twksWSRequiredElementsRow)

                                        'Set the Element Status for the Rotor Position and update the Reagent Status in table of required Element Status
                                        rotorContenPositionRow.ElementStatus = myElementRow.ElementStatus
                                        myGlobalDataTO = myRequiredElementsDelegate.UpdateStatus(dbConnection, myElementRow.ElementID, myElementRow.ElementStatus)
                                    End If
                                End If

                                If (Not myGlobalDataTO.HasError) Then
                                    If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                                Else
                                    If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                                End If
                            End If
                        End If

                        'Return the updated Rotor Position
                        myGlobalDataTO.SetDatos = pWSRotorContentByPositionDS
                    End If
                End If
            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                myGlobalDataTO.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSRotorContentByPositionDelegate.UpdateReagentPosition", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Update the status of Sample Tubes positioned in SamplesRotor (PENDING -> INPROCESS -> FINISHED). The searching of affected
        ''' tubes can be done in two ways:
        ''' ** By ExecutionID: when parameter pExecutionID is informed, business has to find values of ElementID, SampleClass, MultiItemNumber 
        '''    for all tubes placed in Samples Rotor and related with the informed ExecutionID
        ''' ** By ElementID: when optional parameters pElementID, pTubeContent and pMultiItemNumber are informed, business has to find all 
        '''    tubes placed in Samples Rotor for these values
        ''' 
        ''' If sample tube position status is DEPLETED do not update his status using this method
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pExecutionID">Execution Identifier. Informed when the status searching is by Execution</param>
        ''' <param name="pWorkSessionID">WorkSession Identifier</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pElementID">Element Identifier. Informed when the status searching is by Element</param>
        ''' <param name="pTubeContent">Element Type. Informed when the status searching is by Element</param>
        ''' <param name="pMultiItemNumber">For Multipoint Calibrators: the number of item; otherwise, always one. Informed when the status 
        '''                                searching is by Element</param>
        ''' <param name="pNewPositionStatus"></param>
        ''' <returns>GlobalDataTO containing a typed DataSet WSRotorContentByPositionDS with the list of updated Sample Rotor Positions
        '''          Additionally, the ByRef parameter pNewPositionStatus returns the new status assigned to the affected Positions</returns>
        ''' <remarks>
        ''' Created by:
        ''' Modified by: AG 07/09/2011 - Some executions using this tube are INPROCESS => Position Status is INPROCESS  
        '''                              Some executions using this tube are PENDING but none is INPROCESS => Position Status is PENDING
        '''                              All executions using this tube are CLOSED or CLOSEDNOK => Position Status is FINISHED
        '''              SA 11/01/2012 - The part for calculation of the position status has been moved to a new function
        '''                              (GetSamplePositionStatus) 
        '''              SA 12/01/2012 - When the position status is updated, then flag ElementFinished for the ElementID placed in the 
        '''                              rotor cell is updated in the following way: if position status is set to FINISHED, then 
        '''                              ElementFinished = TRUE; otherwise, ElementFinished = FALSE
        '''              SA 18/01/2012 - When the searching is done by ExecutionID, verify if there is a positioned tube for the correspondent
        '''                              ElementID (field CellNumber is informed) and only in this case, update the position status.  Field 
        '''                              ElementFinished has to be updated for the ElementID always, not only when there is a positioned tube
        '''              AG 24/04/2012 - Added special processing for Special Test HbTotal (it is linked to a 5 points Calibrators but it uses only
        '''                              one of them)
        '''              AG 10/05/2012 - If the position in which the Sample Tube is placed has status DEPLETED, it is not calculated nor updated 
        '''                              by this function
        '''              SA 10/05/2012 - Code for verification of Special Test settings and management of Multipoint Calibrators are executed now only
        '''                              when the informed Execution corresponds to a Calibrator
        ''' </remarks>
        Public Function UpdateSamplePositionStatus(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pExecutionID As Integer, ByVal pWorkSessionID As String, _
                                                   ByVal pAnalyzerID As String, Optional ByVal pElementID As Integer = -1, Optional ByVal pTubeContent As String = "", _
                                                   Optional ByVal pMultiItemNumber As Integer = -1, Optional ByRef pNewPositionStatus As String = "", _
                                                   Optional ByVal pCellNumber As Integer = -1) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                '' XBC 03/07/2012 - time estimation
                'Dim StartTime As DateTime = Now

                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myRingNumber As Integer = -1
                        Dim myCellNumber As Integer = -1
                        Dim myElementID As Integer = -1
                        Dim newPositionStatus As String = String.Empty
                        Dim myReturnDS As New WSRotorContentByPositionDS

                        'Calculate the status to assign to the Rotor Position for the tube needed for the informed ElementID or ExecutionID 
                        resultData = GetSamplePositionStatus(dbConnection, pWorkSessionID, pAnalyzerID, pElementID, pTubeContent, pMultiItemNumber, _
                                                             pExecutionID)
                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            If (pExecutionID = -1) Then
                                'Get the calculated position status
                                newPositionStatus = DirectCast(resultData.SetDatos, String)

                                myElementID = pElementID
                                myCellNumber = pCellNumber
                            Else
                                'Get the calculated position status, the ElementID and the cell number from the returned DS
                                myReturnDS = DirectCast(resultData.SetDatos, WSRotorContentByPositionDS)
                                If (myReturnDS.twksWSRotorContentByPosition.Rows.Count > 0) Then
                                    newPositionStatus = myReturnDS.twksWSRotorContentByPosition(0).Status
                                    myElementID = myReturnDS.twksWSRotorContentByPosition(0).ElementID

                                    myCellNumber = -1
                                    If (Not myReturnDS.twksWSRotorContentByPosition(0).IsCellNumberNull) Then
                                        myRingNumber = myReturnDS.twksWSRotorContentByPosition(0).RingNumber
                                        myCellNumber = myReturnDS.twksWSRotorContentByPosition(0).CellNumber
                                    End If
                                End If
                            End If

                            Dim changeStatusFlag As Boolean = False
                            Dim myDAO As New twksWSRotorContentByPositionDAO

                            'AG 10/05/2012 - If sample tube position status is DEPLETED do not update the status using this method
                            'If (newPositionStatus <> pNewPositionStatus) Then
                            '   pNewPositionStatus = newPositionStatus
                            '   changeStatusFlag = True
                            'End If
                            If (newPositionStatus <> pNewPositionStatus AndAlso myCellNumber <> -1) Then
                                'Get the current Status of the Rotor Position
                                resultData = myDAO.Read(dbConnection, pAnalyzerID, pWorkSessionID, "SAMPLES", myCellNumber, myRingNumber)
                                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                    'The position will be updated only if the Sample Tube is not DEPLETED
                                    If (DirectCast(resultData.SetDatos, WSRotorContentByPositionDS).twksWSRotorContentByPosition(0).Status <> "DEPLETED") Then
                                        pNewPositionStatus = newPositionStatus
                                        changeStatusFlag = True
                                    End If
                                End If
                            End If

                            If (Not resultData.HasError AndAlso String.Compare(newPositionStatus, String.Empty, False) <> 0 AndAlso changeStatusFlag) Then
                                'Update value of flag ElementFinished for the Required Element
                                Dim myWSReqElementsDelegate As New WSRequiredElementsDelegate
                                resultData = myWSReqElementsDelegate.UpdateElementFinished(dbConnection, myElementID, (String.Compare(newPositionStatus, "FINISHED", False) = 0))

                                If (Not resultData.HasError) Then
                                    If (myCellNumber <> -1) Then
                                        'Update Samples Rotor Position Status
                                        resultData = myDAO.UpdateByRotorTypeAndCellNumber(dbConnection, pAnalyzerID, pWorkSessionID, "SAMPLES", myCellNumber, _
                                                                                          newPositionStatus, 0, 0, False)
                                    End If
                                End If

                                'Verify if the Execution belongs to a Calibrator
                                Dim myRealMultiItemNumber As Integer = pMultiItemNumber
                                If (Not resultData.HasError AndAlso pExecutionID <> -1 AndAlso newPositionStatus = "FINISHED") Then
                                    Dim myExecutionDelegate As New ExecutionsDelegate
                                    resultData = myExecutionDelegate.GetExecution(dbConnection, pExecutionID)

                                    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                        Dim wsExecDS As ExecutionsDS = DirectCast(resultData.SetDatos, ExecutionsDS)

                                        If (wsExecDS.twksWSExecutions.Rows.Count > 0) Then
                                            If (wsExecDS.twksWSExecutions(0).SampleClass.Trim = "CALIB") Then
                                                If (myRealMultiItemNumber = 1) Then
                                                    'Verify if the Calibrator is used for a Test/SampleType marked as Special (case of HbTotal)
                                                    Dim mySettingsDelegate As New SpecialTestsSettingsDelegate
                                                    resultData = mySettingsDelegate.Read(dbConnection, wsExecDS.twksWSExecutions(0).TestID, wsExecDS.twksWSExecutions(0).SampleType, _
                                                                                         GlobalEnumerates.SpecialTestsSettings.CAL_POINT_USED.ToString)

                                                    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                                        Dim mySettingsDS As SpecialTestsSettingsDS = DirectCast(resultData.SetDatos, SpecialTestsSettingsDS)

                                                        If (mySettingsDS.tfmwSpecialTestsSettings.Rows.Count > 0) Then
                                                            myRealMultiItemNumber = CInt(mySettingsDS.tfmwSpecialTestsSettings(0).SettingValue)
                                                        End If
                                                    End If
                                                End If

                                                If (myRealMultiItemNumber > 1) Then
                                                    'It is a MULTIPOINT CALIBRATOR - Get the ExecutionID for the rest of Calibrator Tubes in the kit
                                                    resultData = myExecutionDelegate.GetExecutionMultititem(dbConnection, pExecutionID)

                                                    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                                        Dim myExecutionDS As ExecutionsDS = DirectCast(resultData.SetDatos, ExecutionsDS)

                                                        Dim myElementInfoDS As WSRotorContentByPositionDS
                                                        For Each myExecutionRow As ExecutionsDS.twksWSExecutionsRow In myExecutionDS.twksWSExecutions.Rows
                                                            'Get the ElementID for the Execution
                                                            resultData = myExecutionDelegate.GetElementInfoByExecutionID(dbConnection, pAnalyzerID, pWorkSessionID, _
                                                                                                                         myExecutionRow.ExecutionID, False)

                                                            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                                                myElementInfoDS = DirectCast(resultData.SetDatos, WSRotorContentByPositionDS)

                                                                If (myElementInfoDS.twksWSRotorContentByPosition.Count > 0) Then
                                                                    'Mark the Calibrator Point as FINISHED
                                                                    resultData = myWSReqElementsDelegate.UpdateElementFinished(dbConnection, _
                                                                                                                               myElementInfoDS.twksWSRotorContentByPosition(0).ElementID, True)
                                                                End If

                                                                If (Not resultData.HasError) Then
                                                                    'Get the Rotor position in which the Tube for the Calibrator Point is placed
                                                                    resultData = GetElementPositionInRotor(dbConnection, pWorkSessionID, _
                                                                                                           myElementInfoDS.twksWSRotorContentByPosition(0).ElementID)

                                                                    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                                                        myElementInfoDS = DirectCast(resultData.SetDatos, WSRotorContentByPositionDS)

                                                                        If (myElementInfoDS.twksWSRotorContentByPosition.Count > 0) Then
                                                                            'Update Status of the Rotor Position in which the Tube for the Calibrator Point is placed
                                                                            resultData = myDAO.UpdateByRotorTypeAndCellNumber(dbConnection, pAnalyzerID, pWorkSessionID, "SAMPLES", _
                                                                                                                              myElementInfoDS.twksWSRotorContentByPosition(0).CellNumber, _
                                                                                                                              "FINISHED", 0, 0, False)

                                                                            If (Not resultData.HasError) Then
                                                                                'Set the new position Status in theDS
                                                                                myElementInfoDS.twksWSRotorContentByPosition(0).BeginEdit()
                                                                                myElementInfoDS.twksWSRotorContentByPosition(0).Status = "FINISHED"
                                                                                myElementInfoDS.twksWSRotorContentByPosition(0).EndEdit()
                                                                                myReturnDS.twksWSRotorContentByPosition.ImportRow(myElementInfoDS.twksWSRotorContentByPosition(0))
                                                                            Else
                                                                                'Error updating Status of the Rotor Position in which the Calibrator Tube is placed...
                                                                                Exit For
                                                                            End If
                                                                        End If
                                                                    Else
                                                                        'Error reading the Rotor position in which the Calibrator Tube is placed...
                                                                        Exit For
                                                                    End If
                                                                Else
                                                                    'Error marking the Calibrator Tube as FINISHED... 
                                                                    Exit For
                                                                End If
                                                            Else
                                                                'Error getting the ElementID for the Execution...
                                                                Exit For
                                                            End If
                                                        Next
                                                    End If
                                                End If
                                            End If
                                        End If
                                    End If
                                End If

                                ''Added special processing for Special Test HbTotal
                                'Dim myRealMultiItemNumber As Integer = pMultiItemNumber
                                'If (Not resultData.HasError AndAlso pExecutionID <> -1 AndAlso newPositionStatus = "FINISHED" AndAlso myRealMultiItemNumber = 1) Then
                                '    'Verify if the execution belongs to a Calibration of Special Test HbTotal 
                                '    Dim myExecutionDelegate As New ExecutionsDelegate
                                '    resultData = myExecutionDelegate.GetExecution(dbConnection, pExecutionID)

                                '    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                '        Dim wsExecDS As ExecutionsDS = DirectCast(resultData.SetDatos, ExecutionsDS)

                                '        If (wsExecDS.twksWSExecutions.Rows.Count > 0) Then
                                '            If (wsExecDS.twksWSExecutions(0).SampleClass.Trim = "CALIB") Then
                                '                Dim mySettingsDelegate As New SpecialTestsSettingsDelegate
                                '                resultData = mySettingsDelegate.Read(dbConnection, wsExecDS.twksWSExecutions(0).TestID, wsExecDS.twksWSExecutions(0).SampleType, _
                                '                                                     GlobalEnumerates.SpecialTestsSettings.CAL_POINT_USED.ToString)

                                '                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                '                    Dim mySettingsDS As SpecialTestsSettingsDS = DirectCast(resultData.SetDatos, SpecialTestsSettingsDS)

                                '                    If (mySettingsDS.tfmwSpecialTestsSettings.Rows.Count > 0) Then
                                '                        myRealMultiItemNumber = CInt(mySettingsDS.tfmwSpecialTestsSettings(0).SettingValue)
                                '                    End If
                                '                End If
                                '            End If
                                '        End If
                                '    End If
                                'End If

                                ''AG 01/03/2012 - additional business for case INPROCESS -> FINISHED and calibrator multiitem
                                ''AG 24/04/2012 - change pMultiItemNumber for myRealMultiItemNumber in the if
                                'If (Not resultData.HasError AndAlso pExecutionID <> -1 AndAlso newPositionStatus = "FINISHED" AndAlso myRealMultiItemNumber > 1) Then
                                '    'Get the ExecutionID for the rest of Calibrator Tubes in the kit
                                '    Dim myExecutionDelegate As New ExecutionsDelegate
                                '    resultData = myExecutionDelegate.GetExecutionMultititem(dbConnection, pExecutionID)

                                '    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                '        Dim myExecutionDS As ExecutionsDS = DirectCast(resultData.SetDatos, ExecutionsDS)

                                '        Dim myElementInfoDS As WSRotorContentByPositionDS
                                '        For Each myExecutionRow As ExecutionsDS.twksWSExecutionsRow In myExecutionDS.twksWSExecutions.Rows
                                '            'Get the ElementID for the Execution
                                '            resultData = myExecutionDelegate.GetElementInfoByExecutionID(dbConnection, pAnalyzerID, pWorkSessionID, _
                                '                                                                         myExecutionRow.ExecutionID, False)

                                '            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                '                myElementInfoDS = DirectCast(resultData.SetDatos, WSRotorContentByPositionDS)
                                '                If (myElementInfoDS.twksWSRotorContentByPosition.Count > 0) Then
                                '                    'Mark the Calibrator Tube as FINISHED
                                '                    resultData = myWSReqElementsDelegate.UpdateElementFinished(dbConnection, _
                                '                                                                               myElementInfoDS.twksWSRotorContentByPosition(0).ElementID, True)
                                '                End If

                                '                If (Not resultData.HasError) Then
                                '                    'Get the Rotor position in which the Calibrator Tube is placed
                                '                    resultData = GetElementPositionInRotor(dbConnection, pWorkSessionID, _
                                '                                                           myElementInfoDS.twksWSRotorContentByPosition(0).ElementID)

                                '                    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                '                        myElementInfoDS = DirectCast(resultData.SetDatos, WSRotorContentByPositionDS)

                                '                        If (myElementInfoDS.twksWSRotorContentByPosition.Count > 0) Then
                                '                            'Update Status of the Rotor Position in which the Calibrator Tube is placed
                                '                            resultData = myDAO.UpdateByRotorTypeAndCellNumber(dbConnection, pAnalyzerID, pWorkSessionID, "SAMPLES", _
                                '                                                                              myElementInfoDS.twksWSRotorContentByPosition(0).CellNumber, _
                                '                                                                              "FINISHED", 0, 0, False)

                                '                            If (Not resultData.HasError) Then
                                '                                'Set the new position Status in theDS
                                '                                myElementInfoDS.twksWSRotorContentByPosition(0).BeginEdit()
                                '                                myElementInfoDS.twksWSRotorContentByPosition(0).Status = "FINISHED"
                                '                                myElementInfoDS.twksWSRotorContentByPosition(0).EndEdit()
                                '                                myReturnDS.twksWSRotorContentByPosition.ImportRow(myElementInfoDS.twksWSRotorContentByPosition(0))
                                '                            Else
                                '                                'Error updating Status of the Rotor Position in which the Calibrator Tube is placed...
                                '                                Exit For
                                '                            End If
                                '                        End If
                                '                    Else
                                '                        'Error reading the Rotor position in which the Calibrator Tube is placed...
                                '                        Exit For
                                '                    End If
                                '                Else
                                '                    'Error marking the Calibrator Tube as FINISHED... 
                                '                    Exit For
                                '                End If
                                '            Else
                                '                'Error getting the ElementID for the Execution...
                                '                Exit For
                                '            End If
                                '        Next
                                '    End If
                                'End If
                            End If
                        End If

                        If (Not resultData.HasError) Then
                            'When the Database Connection was opened locally, then the Commit is executed
                            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                            resultData.SetDatos = myReturnDS
                        Else
                            'When the Database Connection was opened locally, then the Rollback is executed
                            If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                        End If
                    End If
                End If


                '' XBC 03/07/2012 - time estimation
                ''Dim myLogAcciones As New ApplicationLogManager()
                'GlobalBase.CreateLogActivity("Update Sample Position : Id [" & pExecutionID.ToString & "] " & _
                '                                "New Position Status [" & pNewPositionStatus.ToString & "] " & _
                '                                Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), "WSRotorContentByPositionDelegate.UpdateSamplePositionStatus", EventLogEntryType.Information, False)

            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSRotorContentByPositionDelegate.UpdateSamplePositionStatus", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' For the specified ElementID, which corresponds to an Element required in the Analyzer WorkSession that is marked as not positioned,
        ''' verify if there are positioned tubes (only those with status different of DEPLETED if parameter pExcludeDepleted is TRUE)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pRotorType">Type of Rotor</param>
        ''' <param name="pElementID">Element Identifier</param>
        ''' <param name="pExcludeDepleted">When True, all Rotor positions containing not depleted bottles/tubes of the specified Element are returned
        '''                                When False, all Rotor positions containing bottles/tubes of the specified Element are returned (depleted or not)</param>
        ''' <returns>GlobalDataTO containing a typed DataSet WSRotorContentByPositionDS with the Rotor Position in which the tubes of the informed 
        '''          Element are positioned</returns>
        ''' <remarks>
        ''' Created by:  SA 10/02/2012
        ''' </remarks>
        Public Function VerifyTubesByElement(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, _
                                             ByVal pRotorType As String, ByVal pElementID As Integer, ByVal pExcludeDepleted As Boolean) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksWSRotorContentByPositionDAO
                        myGlobalDataTO = myDAO.VerifyTubesByElement(dbConnection, pAnalyzerID, pWorkSessionID, pRotorType, pElementID, pExcludeDepleted)
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSRotorContentByPositionDelegate.VerifyTubesByElement", EventLogEntryType.Error, False)
            Finally
                'When Database Connection was opened locally, it has to be closed 
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Change the Analyzer identifier of the informed Analyzer WorkSession.
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <returns>GlobalDataTO containing Success/Error information</returns>
        ''' <remarks>
        ''' Created by:  XBC 11/06/2012
        ''' </remarks>
        Public Function UpdateWSAnalyzerID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection
            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError) And (Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksWSRotorContentByPositionDAO
                        resultData = myDAO.UpdateWSAnalyzerID(dbConnection, pAnalyzerID, pWorkSessionID)

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
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSRotorContentByPositionDelegate.UpdateWSAnalyzerID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' All calls to Update function in the DAO Class have been replaced for a call to this funcion, which add traces to the Application Log when
        ''' there are positions with invalid values (TubeContent informed but not information about the corresponding Element ID). 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pRotorType">Rotor Type: SAMPLES or REAGENTS</param>
        ''' <param name="pRotorContentsDS">Typed DataSet WSRotorContentByPositionDS containing data of the Rotor Positions to update</param>
        ''' <param name="pProcessWhoCalls">Enumerate that informs the process who calls the delegate instance</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  AG 07/10/2014 - BA-1979
        ''' </remarks>
        Public Function Update(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pRotorType As String, ByVal pRotorContentsDS As WSRotorContentByPositionDS, Optional ByVal pProcessWhoCalls As ClassCalledFrom = ClassCalledFrom.NotInitiated) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'BA-1979: Check if there are FREE and NOT IN USE Rotor Positions with incomplete data (TubeContent without ID of the element) 
                        If (pProcessWhoCalls <> ClassCalledFrom.NotInitiated) Then
                            resultData = CheckForInvalidPosition(dbConnection, pRotorType, pRotorContentsDS, pProcessWhoCalls)
                        End If

                        If (Not resultData.HasError) Then
                            Dim myDAO As New twksWSRotorContentByPositionDAO
                            resultData = myDAO.Update(dbConnection, pRotorContentsDS)
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

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSRotorContentByPositionDelegate.Update", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

#End Region

#Region "Private methods"
        ''' <summary>
        ''' Manages the manual positioning of Additional Solutions. The default bottle used and the process will depend on 
        ''' the Analyzer Model and the Rotor Type
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWSRotorContentByPositionDS">Typed DataSet containing the basic data needed to place 
        '''                                           a required Additional Solution Element in a Rotor Cell</param>
        ''' <returns>GlobalDataTO containing a typed DataSet WSRotorContentByPositionDS with all information about the 
        '''          Rotor Cell in which the Additional Solution was placed plus the recalculated Element Status</returns>
        ''' <remarks>
        ''' Created by:  TR 30/11/2009 - Tested: OK
        ''' Modified by: SA 13/01/2010 - Changed the way of opening the DB Transaction to fulfill the new template
        '''              SA 11/01/2012 - Code improved
        '''              XB 07/10/2014 - BA-1978 ==> Add log traces to catch NULL wrong assignment on RealVolume field
        '''              AG 07/10/2014 - BA-1979 ==> Replaced call to function twksWSRotorContentByPositionDAO.Update by a call to function Update 
        '''                                          in this Delegate, informing the process who is updating the content of the Rotor Position (to 
        '''                                          search which process is inserting invalid values: positions with TubeContent but not element ID)
        ''' </remarks>
        Private Function AdditionalSolutionPositioning(ByVal pDBConnection As SqlClient.SqlConnection, _
                                                       ByVal pWSRotorContentByPositionDS As WSRotorContentByPositionDS) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myRotorContentPosROW As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow
                        If (pWSRotorContentByPositionDS.twksWSRotorContentByPosition.Rows.Count > 0) Then
                            'Assign the value to a row object
                            myRotorContentPosROW = pWSRotorContentByPositionDS.twksWSRotorContentByPosition(0)

                            If (String.Compare(myRotorContentPosROW.RotorType, "REAGENTS", False) = 0) Then
                                'Search the size of the bottle to place according the Ring Number
                                Dim myReagentTubeTypesDelegate As New ReagentTubeTypesDelegate
                                If (myRotorContentPosROW.RingNumber = 1) Then
                                    'In Ring 1, the proposed bottle if the smallest one
                                    'Search minimum volume of "small" bottles
                                    myGlobalDataTO = myReagentTubeTypesDelegate.GetMinimumBottleSize(dbConnection)
                                Else
                                    'In Ring 2, the proposed bottle will be the biggest one
                                    'Search maximum volume of "big" bottles
                                    myGlobalDataTO = myReagentTubeTypesDelegate.GetMaximumBottleSize(dbConnection)
                                End If

                                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                    Dim bottleSize As Integer = CType(myGlobalDataTO.SetDatos, Integer)

                                    'Get code of the Bottle for the volume found
                                    myGlobalDataTO = myReagentTubeTypesDelegate.GetBottleByVolume(dbConnection, bottleSize)
                                    If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                        Dim bottleCode As String = CType(myGlobalDataTO.SetDatos, String)

                                        'Set value of fields TubeType, RealVolume and Status in DataSet pWSRotorContentByPositionDS
                                        myRotorContentPosROW.BeginEdit()
                                        myRotorContentPosROW.TubeType = bottleCode
                                        myRotorContentPosROW.RealVolume = bottleSize
                                        myRotorContentPosROW.Status = "INUSE"
                                        myRotorContentPosROW.EndEdit()

                                        'Update the Rotor Cell / Position informing the Element positioned in it.
                                        'Dim myWSRotorContentByPositionDAO As New twksWSRotorContentByPositionDAO
                                        'myGlobalDataTO = myWSRotorContentByPositionDAO.Update(dbConnection, pWSRotorContentByPositionDS)
                                        myGlobalDataTO = Update(dbConnection, "REAGENTS", pWSRotorContentByPositionDS, ClassCalledFrom.AdditionalSolutionPositioning) 'AG 07/10/2014 BA-1979 call the Update method  in delegate instead of in DAO

                                        If (Not myGlobalDataTO.HasError) Then
                                            'Update Status of the Required Element to Positioned (POS)
                                            Dim myWSRequiredElementsDelegate As New WSRequiredElementsDelegate
                                            myGlobalDataTO = myWSRequiredElementsDelegate.UpdateStatus(dbConnection, myRotorContentPosROW.ElementID, "POS")

                                            If (Not myGlobalDataTO.HasError) Then
                                                'Set field ElementStatus in DataSet to POS  
                                                myRotorContentPosROW.BeginEdit()
                                                myRotorContentPosROW.ElementStatus = "POS"
                                                myRotorContentPosROW.EndEdit()

                                                'Accept changes made in the entry DataSet
                                                pWSRotorContentByPositionDS.AcceptChanges()
                                            End If
                                        End If
                                    End If
                                End If
                                'Else
                                'PENDING: for Rotor used for A200 Model
                            End If
                        End If

                        If (Not myGlobalDataTO.HasError) Then
                            'Commit DB Transaction
                            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)

                            myGlobalDataTO.SetDatos = pWSRotorContentByPositionDS
                            myGlobalDataTO.HasError = False
                        Else
                            'RollBack DB Transaction
                            If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                        End If
                    End If
                End If
            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSRotorContentByPositionDelegate.AdditionalSolutionPositioning", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Add a new Ring Cell for an Analyzer Rotor (create a new RotorContentByPosition Row on twksWSRotorContentByPosition table)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWSRotorContentByPositionDS">Typed DataSet containing the basic data needed to add a cell for an Analyzer Rotor</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by: TR 18/11/2009 - Tested: OK
        '''             SA 13/01/2010 - Added the transaction control (following the new template)
        '''             SA 12/01/2012 - Changed the function template
        ''' </remarks>
        Private Function AddRotorRingCell(ByVal pDBConnection As SqlClient.SqlConnection, _
                                          ByVal pWSRotorContentByPositionDS As WSRotorContentByPositionDS) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing
            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myWSRotorContentByPositionDAO As New twksWSRotorContentByPositionDAO
                        myGlobalDataTO = myWSRotorContentByPositionDAO.Create(dbConnection, pWSRotorContentByPositionDS)

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
                myGlobalDataTO.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSRotorContentByPositionDelegate.AddRotorRingCell", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Manages the positioning of Calibrators (manual and automatic). In both cases, the default tube defined for Calibrators is used, and, if the Calibrator 
        ''' is a Multipoint one, the full kit is positioned, but if at least one of the tubes of the kit cannot be positioned, then the full kit is not positioned 
        ''' and the ErrorCode ROTOR_FULL_FOR_CALIBRATOR_KIT is returned
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWSRotorContentByPositionDS">Typed DataSet WSRotorContentByPositionDS containing the basic data needed to place a required Calibrator Element
        '''                                           in a Rotor Cell</param>
        ''' <param name="pMaxRotorRingNumber">Maximum Ring Number</param>
        ''' <param name="pAutoPositioning">Flag indicating if the function is used for manual (when false)  or automatic (when true) positioning</param>
        ''' <returns>GlobalDataTO containing a typed DataSet WSRotorContentByPositionDS with the information of the Rotor Cell in which the Calibrator was placed 
        '''          plus the recalculated Element Status</returns>
        ''' <remarks>
        ''' Created by:  TR 
        ''' Modified by: BK 23/12/2009 - Changed datatype Byte to Integer
        '''              SA 13/01/2010 - Changed the way of opening the DB Transaction to fulfill the new template
        '''              SA 26/01/2010 - Added new parameter for the maximum Ring Number in the Analyzer Rotor and pass it to functions GetRotorPositionForSample and 
        '''                              GetNextRotorPositionForSample
        '''              TR 29/01/2010 - Before search a position for the additional Calibrator points, verify first if there are FREE positions in the Rotor for the 
        '''                              Work Session
        '''              SA 18/03/2010 - Remove getting the default Tube Type for Calibrators from the corresponding User Setting; the TubeType is now informed for 
        '''                              the Required Element
        '''              SA 15/10/2010 - After verifying if there are more required elements for the Calibrator (case of multipoint Calibrators), validate if number of 
        '''                              returned records is "> 0", instead of "> 1"; due to this error 2P Calibrators were not correctly positioned
        '''              RH 30/08/2011 - Bug correction. Code optimization. Every unneeded New is out. Short-circuit evaluation
        '''              AG 08/09/2011 - When the Calibrator is positioned, calculate the Position Status by calling function UpdateSamplePositionStatus
        '''              SA 27/09/2011 - Set values of Barcode fields for all positions of a Multipoint Calibrator (currently they are updated only for the first point)
        '''              SA 10/01/2012 - Use value of field ElementFinished to set Cell Status = Finished. Call function GetSamplePositionStatus only when 
        '''                              ElementFinished is False. Implementation improved 
        '''              XB 07/10/2014 - BA-1978 ==> Add log traces to catch NULL wrong assignment on RealVolume field
        '''              AG 07/10/2014 - BA-1979 ==> Replaced call to function twksWSRotorContentByPositionDAO.Update by a call to function Update 
        '''                                          in this Delegate, informing the process who is updating the content of the Rotor Position (to 
        '''                                          search which process is inserting invalid values: positions with TubeContent but not element ID)
        '''              MR 04/06/2015 - BA-2558 ==> Add a control before to positioned a  multipoint calibrator, to check if we have enough free cells where the user selected when the user is triying to positioned manually. 
        '''                                          If there are cells arround we move all calibrator kit if not we suggest to the user select another starting point.
        ''' </remarks>
        Private Function CalibratorPositioning(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWSRotorContentByPositionDS As WSRotorContentByPositionDS, _
                                               ByVal pMaxRotorRingNumber As Integer, ByVal pAutoPositioning As Boolean) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim rotorContentRow As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow
                        rotorContentRow = pWSRotorContentByPositionDS.twksWSRotorContentByPosition(0)

                        'Get Username of the logged User                                       
                        'Dim myGlobalbase As New GlobalBase
                        Dim loggedUser As String = GlobalBase.GetSessionInfo().UserName

                        'Search details of the Required Element needed to be positioned (Calibrator Identifier, TubeType and indicator of Element Finished)
                        Dim myRequiredElementsDelegate As New WSRequiredElementsDelegate
                        myGlobalDataTO = myRequiredElementsDelegate.GetRequiredElementData(dbConnection, rotorContentRow.ElementID)

                        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                            Dim myRequiredElementsDS As WSRequiredElementsDS = DirectCast(myGlobalDataTO.SetDatos, WSRequiredElementsDS)

                            Dim calibratorID As Integer = 0
                            Dim calibratorFinished As Boolean = False
                            If (myRequiredElementsDS.twksWSRequiredElements.Rows.Count > 0) Then
                                'Save CalibratorID and ElementFinished in local variables
                                calibratorID = myRequiredElementsDS.twksWSRequiredElements(0).CalibratorID
                                calibratorFinished = myRequiredElementsDS.twksWSRequiredElements(0).ElementFinished

                                'If it is not informed, set the TubeType in the entry DataSet
                                If (rotorContentRow.IsTubeTypeNull) Then rotorContentRow.TubeType = myRequiredElementsDS.twksWSRequiredElements(0).TubeType

                                'If the function has been called for automatic positioning...
                                If (pAutoPositioning) Then
                                    myGlobalDataTO = GetRotorPositionForSample(dbConnection, rotorContentRow.WorkSessionID, rotorContentRow.AnalyzerID, _
                                                                               rotorContentRow.RotorType, "CALIB", pMaxRotorRingNumber)
                                    'Validate if a warning message of Rotor Full has been sent in field ErrorCode
                                    If (Not myGlobalDataTO.HasError AndAlso String.IsNullOrEmpty(myGlobalDataTO.ErrorCode)) Then
                                        Dim myRingCellNumberDS As RingCellNumbersDS = DirectCast(myGlobalDataTO.SetDatos, RingCellNumbersDS)

                                        'Inform the found RingNumber and CellNumber in the DataSet 
                                        rotorContentRow.RingNumber = myRingCellNumberDS.RingCellTable(0).RingNumber
                                        rotorContentRow.CellNumber = myRingCellNumberDS.RingCellTable(0).CellNumber
                                    End If
                                    'Else
                                    'If the Calibrator was manually dropped in a cell, fields RingNumber and CellNumber are informed in the 
                                    'entry DS, it is not needed search the next position here
                                End If

                                If (Not myGlobalDataTO.HasError AndAlso String.IsNullOrEmpty(myGlobalDataTO.ErrorCode)) Then
                                    If (calibratorFinished) Then
                                        rotorContentRow.Status = "FINISHED"
                                    Else
                                        'Get the position status for the first Calibrator point ...
                                        rotorContentRow.Status = "PENDING"
                                        myGlobalDataTO = GetSamplePositionStatus(dbConnection, rotorContentRow.WorkSessionID, rotorContentRow.AnalyzerID, _
                                                                                 rotorContentRow.ElementID, "CALIB", 1)
                                        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                            Dim newStatus As String = DirectCast(myGlobalDataTO.SetDatos, String)
                                            If (newStatus <> String.Empty AndAlso String.Compare(newStatus, "FINISHED", False) <> 0) Then rotorContentRow.Status = newStatus
                                        End If
                                    End If

                                    rotorContentRow.TS_User = loggedUser
                                    rotorContentRow.TS_DateTime = Now
                                End If

                                If (Not myGlobalDataTO.HasError AndAlso String.IsNullOrEmpty(myGlobalDataTO.ErrorCode)) Then
                                    'Search if there are more Required Elements created for the Calibrator (case of MultiPoint Calibrators)
                                    myGlobalDataTO = myRequiredElementsDelegate.GetMultiPointCalibratorElements(dbConnection, rotorContentRow.WorkSessionID, rotorContentRow.ElementID, _
                                                                                                                calibratorID)
                                    If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                        myRequiredElementsDS = DirectCast(myGlobalDataTO.SetDatos, WSRequiredElementsDS)

                                        If (myRequiredElementsDS.twksWSRequiredElements.Rows.Count > 0) Then
                                            'The Calibrator is MultiPoint - Verify if there are FREE positions for all the Calibrator points
                                            myGlobalDataTO = CountPositionsByStatus(pDBConnection, rotorContentRow.AnalyzerID, rotorContentRow.WorkSessionID, _
                                                                                    rotorContentRow.RotorType, "FREE")
                                            Dim numFreeCells As Integer = 0
                                            If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                                                numFreeCells = CType(myGlobalDataTO.SetDatos, Integer)

                                                'If there are enough free positions to place all Calibrator Points...
                                                'RH 30/08/2011 Take the first (passed) element into account. That is, add 1 to Count
                                                If (numFreeCells >= myRequiredElementsDS.twksWSRequiredElements.Rows.Count + 1) Then
                                                    'Set values of RingNumber and CellNumber to local variables to be used as reference to search the next free position
                                                    Dim cellNumber As Integer = rotorContentRow.CellNumber
                                                    Dim ringNumber As Integer = rotorContentRow.RingNumber

                                                    'Check if we have enough free cells before to start positioning the first point of calibrator, if not we move our starting point.
                                                    myGlobalDataTO = CheckEnoughFreeCellsXMultipointCalib(dbConnection, rotorContentRow.AnalyzerID, rotorContentRow.WorkSessionID, rotorContentRow.RotorType, ringNumber, _
                                                                                                                   cellNumber, myRequiredElementsDS.twksWSRequiredElements.Rows.Count + 1)
                                                    If (Not myGlobalDataTO.HasError AndAlso String.IsNullOrEmpty(myGlobalDataTO.ErrorCode)) Then
                                                        cellNumber = CType(myGlobalDataTO.SetDatos, Integer)
                                                        'If we need move our starting point, we reasigned the first element to be positioned.
                                                        If cellNumber <> rotorContentRow.CellNumber Then
                                                            rotorContentRow.CellNumber = cellNumber
                                                        End If

                                                        Dim newStatus As String = String.Empty
                                                        Dim rotorContentByPosTMP As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow
                                                        For Each reqElementRow As WSRequiredElementsDS.twksWSRequiredElementsRow In myRequiredElementsDS.twksWSRequiredElements
                                                            'Search next free cell to place the Tube for the Calibrator point
                                                            myGlobalDataTO = GetNextRotorPositionForSample(dbConnection, rotorContentRow.WorkSessionID, rotorContentRow.AnalyzerID, _
                                                                                                           rotorContentRow.RotorType, ringNumber, cellNumber, pMaxRotorRingNumber)
                                                            'A Free Position was found?
                                                            If (Not myGlobalDataTO.HasError AndAlso String.IsNullOrEmpty(myGlobalDataTO.ErrorCode)) Then
                                                                Dim myRingCellNumberDS As RingCellNumbersDS = DirectCast(myGlobalDataTO.SetDatos, RingCellNumbersDS)

                                                                rotorContentByPosTMP = pWSRotorContentByPositionDS.twksWSRotorContentByPosition.NewtwksWSRotorContentByPositionRow()
                                                                rotorContentByPosTMP.AnalyzerID = rotorContentRow.AnalyzerID
                                                                rotorContentByPosTMP.RotorType = rotorContentRow.RotorType
                                                                rotorContentByPosTMP.RingNumber = myRingCellNumberDS.RingCellTable(0).RingNumber
                                                                rotorContentByPosTMP.CellNumber = myRingCellNumberDS.RingCellTable(0).CellNumber
                                                                rotorContentByPosTMP.WorkSessionID = rotorContentRow.WorkSessionID
                                                                rotorContentByPosTMP.ElementID = reqElementRow.ElementID
                                                                rotorContentByPosTMP.MultiItemNumber = reqElementRow.MultiItemNumber
                                                                rotorContentByPosTMP.MultiTubeNumber = 1

                                                                rotorContentByPosTMP.TubeContent = rotorContentRow.TubeContent
                                                                rotorContentByPosTMP.TubeType = rotorContentRow.TubeType

                                                                'Update Barcode fields with the same values of the first Calibrator point
                                                                rotorContentByPosTMP.ScannedPosition = rotorContentRow.ScannedPosition
                                                                If (rotorContentRow.ScannedPosition) Then
                                                                    rotorContentByPosTMP.BarCodeInfo = rotorContentRow.BarCodeInfo
                                                                    rotorContentByPosTMP.BarcodeStatus = rotorContentRow.BarcodeStatus
                                                                Else
                                                                    rotorContentByPosTMP.SetBarCodeInfoNull()
                                                                    rotorContentByPosTMP.SetBarcodeStatusNull()
                                                                End If

                                                                rotorContentByPosTMP.TS_User = loggedUser
                                                                rotorContentByPosTMP.TS_DateTime = DateTime.Now

                                                                If (reqElementRow.ElementFinished) Then
                                                                    rotorContentByPosTMP.Status = "FINISHED"
                                                                Else
                                                                    'Get position status for the Calibrator point...
                                                                    rotorContentByPosTMP.Status = "PENDING"
                                                                    myGlobalDataTO = GetSamplePositionStatus(dbConnection, rotorContentByPosTMP.WorkSessionID, rotorContentByPosTMP.AnalyzerID, _
                                                                                                             rotorContentByPosTMP.ElementID, "CALIB", reqElementRow.MultiItemNumber)

                                                                    If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                                                        newStatus = DirectCast(myGlobalDataTO.SetDatos, String)
                                                                        If (newStatus <> String.Empty AndAlso String.Compare(newStatus, "FINISHED", False) <> 0) Then rotorContentByPosTMP.Status = newStatus
                                                                    Else
                                                                        'Error getting the position status for the Calibrator point; stop processing of the rest of Calibrator points
                                                                        Exit For
                                                                    End If
                                                                End If

                                                                'Add the row to the entry DataSet
                                                                pWSRotorContentByPositionDS.twksWSRotorContentByPosition.AddtwksWSRotorContentByPositionRow(rotorContentByPosTMP)

                                                                'The found position is used as reference to search a free position for the next Calibrator Point
                                                                ringNumber = myRingCellNumberDS.RingCellTable(0).RingNumber
                                                                cellNumber = myRingCellNumberDS.RingCellTable(0).CellNumber

                                                            ElseIf (myGlobalDataTO.ErrorCode <> "") Then
                                                                'There are not enough free positions for a Calibrator kit; inform the ErrorCode and stop the processing of the rest 
                                                                'of Calibrator points
                                                                myGlobalDataTO.ErrorCode = "ROTOR_FULL_FOR_CALIBRATOR_KIT"
                                                                myGlobalDataTO.HasError = False
                                                                Exit For
                                                            Else
                                                                'Error getting a position for the Calibrator Point; stop the processing of the rest of Calibrator points
                                                                Exit For
                                                            End If
                                                        Next

                                                    Else
                                                        'MR 13/07/2015
                                                        'NEW ERROR
                                                        'Choose another position in order to have the whole Calibrator kit together" 
                                                        ' "Seleccione otra posición para tener todo el kit de calibración junto"

                                                        'Set the error value when there are not enough free positions for a Calibrator kit
                                                        'OLD  myGlobalDataTO.ErrorCode = "ROTOR_FULL_FOR_CALIBRATOR_KIT"
                                                        myGlobalDataTO.ErrorCode = "ALL_CALIBRATOR_KIT_TOGETHER"
                                                        myGlobalDataTO.HasError = False

                                                    End If
                                                Else
                                                    'Set the error value when there are not enough free positions for a Calibrator kit
                                                    myGlobalDataTO.ErrorCode = "ROTOR_FULL_FOR_CALIBRATOR_KIT"
                                                    myGlobalDataTO.HasError = False
                                                End If
                                            End If
                                        End If

                                        If (Not myGlobalDataTO.HasError AndAlso String.IsNullOrEmpty(myGlobalDataTO.ErrorCode)) Then
                                            'Update all positions found in Rotor to place the Calibrator points and update the correspondent Elements as positioned
                                            Dim myRotorContentPosTMP As New WSRotorContentByPositionDS
                                            'Dim myRotorContentbyPositionDAO As New twksWSRotorContentByPositionDAO

                                            For Each rcpRow As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow In pWSRotorContentByPositionDS.twksWSRotorContentByPosition
                                                'Add the row to a temporal DataSet to be processed
                                                myRotorContentPosTMP.twksWSRotorContentByPosition.ImportRow(rcpRow)

                                                'Update the Rotor Cell/Position informing the Element positioned in it
                                                'myGlobalDataTO = myRotorContentbyPositionDAO.Update(dbConnection, myRotorContentPosTMP)
                                                myGlobalDataTO = Update(dbConnection, "SAMPLES", myRotorContentPosTMP, ClassCalledFrom.CalibratorPositioning) 'AG 07/10/2014 BA-1979 call the Update method  in delegate instead of in DAO
                                                If (Not myGlobalDataTO.HasError) Then
                                                    'Update Status of the Required Element to Positioned (POS)
                                                    myGlobalDataTO = myRequiredElementsDelegate.UpdateStatus(dbConnection, rcpRow.ElementID, "POS")
                                                    If (Not myGlobalDataTO.HasError) Then
                                                        rcpRow.ElementStatus = "POS"
                                                        myRotorContentPosTMP.twksWSRotorContentByPosition.Clear()
                                                    Else
                                                        'Error updating the Status of the Required Element
                                                        Exit For
                                                    End If
                                                Else
                                                    'Error updating the Rotor Position
                                                    Exit For
                                                End If
                                            Next
                                        End If
                                    End If
                                End If
                            End If
                        End If

                        If (Not myGlobalDataTO.HasError) Then
                            'Return the updated Rotor Positions
                            myGlobalDataTO.SetDatos = pWSRotorContentByPositionDS

                            'Commit the transaction if it was locally opened
                            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                        Else
                            'Rollback the transacction if it was locally opened
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
                myGlobalDataTO.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSRotorContentByPositionDelegate.CalibratorPositioning", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

#Region "FUNCTIONS ADDED TO CONTROL THE MANUAL POSITIONING OF MULTIPOINT CALIBRATORS"

        ''' <summary>
        ''' Get the number of positions free between two points of the rotor.
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pAnalyzerID"></param>
        ''' <param name="pWorkSessionID"></param>
        ''' <param name="pRotorType"></param>
        ''' <param name="pRingNumber"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function CheckEnoughFreeCellsXMultipointCalib(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, _
                                                              ByVal pRotorType As String, ByVal pRingNumber As Integer, ByVal pRefCellNumber As Integer, ByVal pNumElemRequired As Integer) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing
            Dim numFreePos As Integer = -1
            Dim resCellNumber As Integer = 0
            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then

                        Dim MaxNumCellbyRotor As Integer = 44

                        'MaxNumCellbyRotor = CountMaxCellByRotor(dbConnection, pAnalyzerID, pRotorType, pRingNumber)

                        Dim myWSRotorContentByPositionDAO As New twksWSRotorContentByPositionDAO
                        myGlobalDataTO = myWSRotorContentByPositionDAO.CountPositionsByStatus(dbConnection, pAnalyzerID, pWorkSessionID, pRotorType, "FREE", pRingNumber, pRefCellNumber, MaxNumCellbyRotor)

                        If (Not myGlobalDataTO.HasError) Then
                            If (Not myGlobalDataTO.SetDatos Is Nothing AndAlso String.Compare(myGlobalDataTO.SetDatos.ToString(), "", False) <> 0) Then
                                numFreePos = CType(myGlobalDataTO.SetDatos, Integer)

                                If numFreePos >= pNumElemRequired Then
                                    'We have enough position after selected Pos
                                    'resCellNumber = pRefCellNumber
                                    myGlobalDataTO.SetDatos = CType(pRefCellNumber, Integer)

                                Else
                                    'We need to move the start selectedPosition.
                                    Dim difPos As Integer = pNumElemRequired - numFreePos
                                    myGlobalDataTO = myWSRotorContentByPositionDAO.CountPositionsByStatus(dbConnection, pAnalyzerID, pWorkSessionID, pRotorType, "FREE", pRingNumber, pRefCellNumber - difPos, pRefCellNumber - 1)

                                    If (Not myGlobalDataTO.HasError) Then
                                        If (Not myGlobalDataTO.SetDatos Is Nothing AndAlso String.Compare(myGlobalDataTO.SetDatos.ToString(), "", False) <> 0) Then
                                            Dim numFreePosAnt = CType(myGlobalDataTO.SetDatos, Integer)
                                            If numFreePosAnt > 0 AndAlso numFreePosAnt >= difPos Then
                                                resCellNumber = pRefCellNumber - difPos
                                                myGlobalDataTO.SetDatos = CType(resCellNumber, Integer)
                                            Else
                                                'We haven't enough positions before the selected point to move the multipoint calibrator, then we send a warning message, saying select other point into the rotor.
                                                myGlobalDataTO.SetDatos = 0
                                                myGlobalDataTO.HasError = False
                                                myGlobalDataTO.ErrorCode = "ROTOR_FULL"
                                            End If
                                        Else
                                            'We haven't enough positions before the selected point to move the multipoint calibrator, then we send a warning message, saying select other point into the rotor.
                                            myGlobalDataTO.SetDatos = 0
                                            myGlobalDataTO.HasError = False
                                            myGlobalDataTO.ErrorCode = "ROTOR_FULL_FOR_CALIBRATOR_KIT"
                                        End If
                                    End If
                                End If
                            Else
                                'In case the returned value is null or empty then the result value is 0
                                myGlobalDataTO.SetDatos = 0
                            End If
                        End If

                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSRotorContentByPositionDelegate.CheckEnoughFreeCellsXMultipointCalib", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get the maximum position of rotor, depends the rotor type wwe send to check.</summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pRotorType">Type of Analyzer Rotor</param>
        ''' <param name="pRingNumber">Ring Number. Optional parameter</param>
        ''' <returns>GlobalDataTO containing an integer value with the number of cells having the informed Status</returns>
        ''' <remarks>
        ''' Created by:  TR 28/01/2010 - TESTED: OK
        ''' Modified by: TR 01/02/2010 - Added new optional parameter pRingNumber
        ''' </remarks>
        Private Function CountMaxCellByRotor(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
                                               ByVal pRotorType As String, ByVal pRingNumber As Integer) As Integer
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing
            Dim MaxCell As Integer = -1
            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myRotorContentByPositionDAO As New twksWSRotorContentByPositionDAO()

                        Dim rotorConfig As New tfmwAnalyzerModelRotorsConfigDAO
                        myGlobalDataTO = rotorConfig.GetAnalyzerRotorsRingConfiguration(dbConnection, pAnalyzerID, pRotorType, pRingNumber)

                        If Not myGlobalDataTO.HasError Then
                            'Get the last cell number
                            MaxCell = CType(myGlobalDataTO.SetDatos, AnalyzerModelRotorsConfigDS).tfmwAnalyzerModelRotorsConfig(0).LastCellNumber
                            myGlobalDataTO.SetDatos = MaxCell
                        End If

                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSRotorContentByPositionDelegate.CountMaxCellByRotor", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return MaxCell
        End Function

#End Region

        ''' <summary>
        ''' Validate if there are available positions in the specified Rotor. In case the Rotor is full, then search if there
        ''' are NOT IN USE positions
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pRotorType">Type of Analyzer Rotor</param>
        ''' <param name="pWorkSessionID">WorkSession Identifier</param>
        ''' <returns>GlobalDataTO containing an integer value with the number of FREE positions in the informed Analyzer Rotor
        '''          If there are not FREE positions, field ErrorCode will contain ROTOR_FULL, but in case there is at least a
        '''          position with status NOT IN USE, then it will contain ROTOR_FULL_NOINUSE_CELLS 
        ''' </returns>
        ''' <remarks>
        ''' Created by   TR 29/01/2010
        ''' Modified by: RH 31/08/2011 - Remove unneeded and memory wasting "New" instructions
        '''              SA 11/01/2012 - Code improved
        ''' </remarks>
        Private Function CheckAvailablePositionsByRotor(ByRef pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
                                                        ByVal pRotorType As String, ByVal pWorkSessionID As String) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing

            Try
                'Get the number of NOT IN USE positions in the Rotor for the Work Session
                myGlobalDataTO = CountPositionsByStatus(pDBConnection, pAnalyzerID, pWorkSessionID, pRotorType, "NO_INUSE")
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    Dim numNotInUseCells As Integer = CType(myGlobalDataTO.SetDatos, Integer)

                    'Get the number of FREE positions in the Rotor for the Work Session
                    myGlobalDataTO = CountPositionsByStatus(pDBConnection, pAnalyzerID, pWorkSessionID, pRotorType, "FREE")
                    If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                        Dim numFreeCells As Integer = CType(myGlobalDataTO.SetDatos, Integer)

                        If (numFreeCells > 0) Then
                            'There are free cells in the Rotor
                            myGlobalDataTO.ErrorCode = ""
                        Else
                            If (numNotInUseCells > 0) Then
                                'The Rotor is full but there are some cells with NOT IN USE elements
                                myGlobalDataTO.ErrorCode = "ROTOR_FULL_NOINUSE_CELLS"
                            Else
                                'The Rotor is full
                                myGlobalDataTO.ErrorCode = "ROTOR_FULL"
                            End If
                        End If
                        myGlobalDataTO.HasError = False
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSRotorContentByPositionDelegate.CheckAvailablePositionsByRotor", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Manages the positioning of Controls (manual and automatic).  In both cases, the default tube defined for Controls is used. When this function is 
        ''' used for manual positioning, the ring and cell to which the User drag and drop the Element is used. When it is used for automatic positioning, a 
        ''' ring/cell is searched according the rules of automatic positioning of Controls
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>        
        ''' <param name="pWSRotorContentByPositionDS">Typed DataSet containing the basic data needed to place a required Control Element in a Rotor Cell</param>
        ''' <param name="pMaxRotorRingNumber">Maximum Ring Number</param>
        ''' <param name="pAutoPositioning">Flag indicating if the function is used for manual (when false) or automatic (when true) positioning</param>
        ''' <returns>GlobalDataTO containing a typed DataSet WSRotorContentByPositionDS with the information about the Rotor Cell in which the Control was placed 
        '''          plus the recalculated Element Status
        ''' </returns>
        ''' <remarks>
        ''' Created by:  TR 25/11/2009 - Tested: OK
        ''' Modified by: SA 13/01/2010 - Changed the way of opening the DB Transaction to fulfill the new template
        '''              SA 26/01/2010 - Added new parameter for the maximum Ring Number in the Analyzer Rotor and pass it 
        '''                              to function GetRotorPositionForSample
        '''              SA 18/03/2010 - Remove getting the default Tube Type for Controls from the corresponding
        '''                              User Setting; the TubeType is now informed for the Required Element; DB Transaction was 
        '''                              bad implemented (commit/rollback was inside a For/Next)
        '''              SA 26/04/2011 - Once the Status of the correspondent Required WS Element has been updated to POS in DB,
        '''                              update also the field ElementStatus in the DS to return
        '''              AG 08/09/2011 - Calculate sample position status (PENDING, INPROCESS, ...)
        '''              TR 17/10/2011 - Removed Exit Try in code and changed the implementation
        '''              SA 10/01/2012 - Implementation improved
        '''              XB 07/10/2014 - BA-1978 ==> Add log traces to catch NULL wrong assignment on RealVolume field
        '''              AG 07/10/2014 - BA-1979 ==> Replaced call to function twksWSRotorContentByPositionDAO.Update by a call to function Update 
        '''                                          in this Delegate, informing the process who is updating the content of the Rotor Position (to 
        '''                                          search which process is inserting invalid values: positions with TubeContent but not element ID)
        ''' </remarks>
        Private Function ControlPositioning(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWSRotorContentByPositionDS As WSRotorContentByPositionDS, _
                                            ByVal pMaxRotorRingNumber As Integer, ByVal pAutoPositioning As Boolean) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim rotorContentRow As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow
                        rotorContentRow = pWSRotorContentByPositionDS.twksWSRotorContentByPosition(0)

                        'Search details of the Required Element needed to be positioned (Control Identifier, TubeType and indicator of Element Finished)
                        Dim myRequiredElementsDelegate As New WSRequiredElementsDelegate
                        myGlobalDataTO = myRequiredElementsDelegate.GetRequiredElementData(dbConnection, rotorContentRow.ElementID)

                        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                            Dim myRequiredElementsDS As WSRequiredElementsDS = DirectCast(myGlobalDataTO.SetDatos, WSRequiredElementsDS)

                            Dim controlID As Integer = 0
                            Dim controlFinished As Boolean = False
                            If (myRequiredElementsDS.twksWSRequiredElements.Rows.Count > 0) Then
                                'Save ControlID and ElementFinished in local variables
                                controlID = myRequiredElementsDS.twksWSRequiredElements(0).ControlID
                                controlFinished = myRequiredElementsDS.twksWSRequiredElements(0).ElementFinished

                                'If it is not informed, set the TubeType in the entry DataSet
                                If (rotorContentRow.IsTubeTypeNull) Then rotorContentRow.TubeType = myRequiredElementsDS.twksWSRequiredElements(0).TubeType

                                If (pAutoPositioning) Then
                                    'If the function has been called for automatic positioning...search the next free position
                                    myGlobalDataTO = GetRotorPositionForSample(dbConnection, rotorContentRow.WorkSessionID, rotorContentRow.AnalyzerID, _
                                                                               rotorContentRow.RotorType, "CTRL", pMaxRotorRingNumber)

                                    'TR 29/01/2010 - Validate if a warning message of Rotor Full has been sent in field ErrorCode
                                    If (Not myGlobalDataTO.HasError AndAlso String.IsNullOrEmpty(myGlobalDataTO.ErrorCode)) Then
                                        Dim myRingCellNumberDS As RingCellNumbersDS = DirectCast(myGlobalDataTO.SetDatos, RingCellNumbersDS)

                                        'Inform the found RingNumber and CellNumber in the DataSet 
                                        rotorContentRow.RingNumber = myRingCellNumberDS.RingCellTable(0).RingNumber
                                        rotorContentRow.CellNumber = myRingCellNumberDS.RingCellTable(0).CellNumber
                                    End If
                                    'Else
                                    'If the Control was manually dropped in a cell, fields RingNumber and CellNumber are informed in the 
                                    'entry DS, it is not needed search the next position here
                                End If

                                If (Not myGlobalDataTO.HasError AndAlso String.IsNullOrEmpty(myGlobalDataTO.ErrorCode)) Then
                                    If (controlFinished) Then
                                        rotorContentRow.Status = "FINISHED"
                                    Else
                                        'Calculate position status for the Control...
                                        rotorContentRow.Status = "PENDING"
                                        myGlobalDataTO = GetSamplePositionStatus(dbConnection, rotorContentRow.WorkSessionID, rotorContentRow.AnalyzerID, _
                                                                                 rotorContentRow.ElementID, "CTRL", 1)
                                        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                            Dim newStatus As String = DirectCast(myGlobalDataTO.SetDatos, String)
                                            If (newStatus <> String.Empty AndAlso String.Compare(newStatus, "FINISHED", False) <> 0) Then rotorContentRow.Status = newStatus
                                        End If
                                    End If
                                End If

                                If (Not myGlobalDataTO.HasError AndAlso String.IsNullOrEmpty(myGlobalDataTO.ErrorCode)) Then
                                    'Update the Rotor Cell / Position informing the Element positioned in it
                                    'Dim myRotorContentByPositionDAO As New twksWSRotorContentByPositionDAO()
                                    'myGlobalDataTO = myRotorContentByPositionDAO.Update(dbConnection, pWSRotorContentByPositionDS)
                                    myGlobalDataTO = Update(dbConnection, "SAMPLES", pWSRotorContentByPositionDS, ClassCalledFrom.ControlPositioning) 'AG 07/10/2014 BA-1979 call the Update method  in delegate instead of in DAO
                                End If

                                If (Not myGlobalDataTO.HasError AndAlso String.IsNullOrEmpty(myGlobalDataTO.ErrorCode)) Then
                                    'Update the Element Status informing it is positioned
                                    myGlobalDataTO = myRequiredElementsDelegate.UpdateStatus(dbConnection, rotorContentRow.ElementID, "POS")
                                    If (Not myGlobalDataTO.HasError) Then rotorContentRow.ElementStatus = "POS"
                                End If
                            End If
                        End If

                        If (Not myGlobalDataTO.HasError) Then
                            'Commit the Transaction when it was locally opened and return the updated Rotor Positions
                            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                            myGlobalDataTO.SetDatos = pWSRotorContentByPositionDS
                        Else
                            'Rollback the Transaction when it was locally opened
                            If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                        End If
                    End If
                End If
            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSRotorContentByPositionDelegate.ControlPositioning", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Count the number of positioned bottles on the Reagents Rotor for an specific Reagent or Additional Solution (ElementID)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzeID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pElementID">Element Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet ReagentTubeTypesDS with the number of different Bottles
        '''          placed for the specified Reagent in the Reagents Rotor</returns>
        ''' <remarks>
        ''' Created by:  TR 04/02/2010 - TESTED: OK
        ''' Modified by: SA 29/07/2010 - Implement the template for DB access
        ''' </remarks>
        Private Function CountPositionedReagentsBottlesByElementID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzeID As String, _
                                                                   ByVal pWorkSessionID As String, ByVal pElementID As Integer) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myRotorContentPosDAO As New twksWSRotorContentByPositionDAO()
                        myGlobalDataTO = myRotorContentPosDAO.CountPositionedReagentsBottlesByElementID(dbConnection, pAnalyzeID, pWorkSessionID, pElementID)
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = ""
                myGlobalDataTO.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSRotorContentByPositionDelegate.CountPositionedReagentsBottlesByElementID", EventLogEntryType.Error, False)
            Finally
                'When Database Connection was opened locally, it has to be closed 
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Count how many cells in the specified Analyzer Rotor have the informed Status
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">WorkSession Identifier</param>
        ''' <param name="pRotorType">Type of Analyzer Rotor</param>
        ''' <param name="pStatus">Cell Status to search</param>
        ''' <param name="pRingNumber">Ring Number. Optional parameter</param>
        ''' <returns>GlobalDataTO containing an integer value with the number of cells having the informed Status</returns>
        ''' <remarks>
        ''' Created by:  TR 28/01/2010 - TESTED: OK
        ''' Modified by: TR 01/02/2010 - Added new optional parameter pRingNumber
        ''' </remarks>
        Private Function CountPositionsByStatus(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, _
                                               ByVal pRotorType As String, ByVal pStatus As String, Optional ByVal pRingNumber As Integer = 0) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing
            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myWSRotorContentByPositionDAO As New twksWSRotorContentByPositionDAO
                        myGlobalDataTO = myWSRotorContentByPositionDAO.CountPositionsByStatus(dbConnection, pAnalyzerID, pWorkSessionID, pRotorType, pStatus, pRingNumber)

                        If (Not myGlobalDataTO.HasError) Then
                            If (Not myGlobalDataTO.SetDatos Is Nothing AndAlso String.Compare(myGlobalDataTO.SetDatos.ToString(), "", False) <> 0) Then
                                myGlobalDataTO.SetDatos = CType(myGlobalDataTO.SetDatos, Integer)
                            Else
                                'In case the returned value is null or empty then the result value is 0
                                myGlobalDataTO.SetDatos = 0
                            End If
                        End If
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSRotorContentByPositionDelegate.CountPositionsByStatus", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Search the next free position to place a Control, a Calibrator or a Patient Sample (full or diluted) in the informed
        ''' Analyzer Rotor, but the free position should be the nearest free position to an informed Reference Position (Ring and
        ''' Cell). It is used to allow placing all tubes for a Multipoint Calibrator, or all Patient Sample tubes (the full Sample 
        ''' and the required dilutions) in near cells
        '''  * For A400 Model the free position is searched in the SAMPLES Rotor 
        '''  * For A200 Model all the searching process is pending of definition
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pRotorType">Analyzer Rotor Type</param>
        ''' <param name="pRefRingNumber">Reference Ring Number</param>
        ''' <param name="pRefCellNumber">Reference Cell Number</param>
        ''' <param name="pMaxRotorRingNumber">Maximum Ring Number</param>
        ''' <returns>GlobalDataTO containing a typed DataSet RingCellNumberDS with the free position found (Ring and Cell). If a 
        '''          free position is not found, then the GlobalDataTO contains the ErrorCode ROTOR_FULL (although HasError flag 
        '''          is False)
        ''' </returns>
        ''' <remarks>
        ''' Created by:  TR 25/11/2009 - Tested: OK
        ''' Modified by: BK 23/12/2009 - Changed parameters and declarations Byte to Integer
        '''              SA 04/01/2010 - Changes needed in call to GetMaxRingNumber due to a new parameter was added
        '''              SA 13/01/2010 - Changed the way of opening the DB Connection to fulfill the new template
        '''              SA 26/01/2010 - Added new parameter for the maximum Ring Number in the Analyzer Rotor. Removed the call
        '''                              to function GetMaxRingNumber in code. Removed all sentences to convert to datatype Byte 
        ''' </remarks>
        Private Function GetNextRotorPositionForSample(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, _
                                                       ByVal pAnalyzerID As String, ByVal pRotorType As String, ByVal pRefRingNumber As Integer, _
                                                       ByVal pRefCellNumber As Integer, ByVal pMaxRotorRingNumber As Integer) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        If (String.Compare(pRotorType, "SAMPLES", False) = 0) Then
                            If (pMaxRotorRingNumber > 0) Then
                                Dim cellNumber As Integer = 0
                                Dim ringNumber As Integer = pRefRingNumber
                                Dim myRotorContentByPositionDAO As New twksWSRotorContentByPositionDAO()

                                'Dim MaxCellRotor As Integer = CType(CountMaxCellByRotor(dbConnection, pAnalyzerID, pRotorType, pMaxRotorRingNumber), Integer)

                                Dim freeCell As Boolean = False
                                Dim notFreePosition As Boolean = False
                                While (Not freeCell AndAlso Not notFreePosition)

                                    'If pRefCellNumber <> MaxCellRotor Then

                                    'Search minimum free cell in the informed Ring nearest to the informed cell
                                    myGlobalDataTO = myRotorContentByPositionDAO.GetMinFreeCellByRing(dbConnection, pAnalyzerID, pRotorType, _
                                                                                                      pWorkSessionID, ringNumber, pRefCellNumber)
                                    'Else
                                    ''Search minimum free cell in the informed Ring nearest to the finish.
                                    'myGlobalDataTO = myRotorContentByPositionDAO.GetMinFreeCellByRing(dbConnection, pAnalyzerID, pRotorType, _
                                    '                                                                  pWorkSessionID, ringNumber)
                                    'End If

                                    If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                        cellNumber = 0
                                        If (Not String.IsNullOrEmpty(myGlobalDataTO.SetDatos.ToString)) Then cellNumber = CType(myGlobalDataTO.SetDatos, Integer)

                                        If (cellNumber > 0) Then
                                            'A free cell was found in the current ring...
                                            freeCell = True
                                        Else
                                            'Search the free cell in the next ring when it is possible (if current ring is the internal one, 
                                            'then the next ring will be the external one)
                                            ringNumber = CType((ringNumber + 1), Byte)
                                            If (ringNumber > pMaxRotorRingNumber) Then ringNumber = 1

                                            If (ringNumber <> pRefRingNumber) Then
                                                'For the new ring there is not a reference cell number
                                                pRefCellNumber = 0
                                            Else
                                                'Search minimum free cell in the informed Ring nearest to the finish.
                                                myGlobalDataTO = myRotorContentByPositionDAO.GetMinFreeCellByRing(dbConnection, pAnalyzerID, pRotorType, _
                                                                                                                  pWorkSessionID, ringNumber)

                                                If (Not String.IsNullOrEmpty(myGlobalDataTO.SetDatos.ToString)) Then cellNumber = CType(myGlobalDataTO.SetDatos, Integer)
                                                If (cellNumber > 0) Then
                                                    'A free cell was found in the current ring...
                                                    freeCell = True
                                                Else
                                                    'All Rings have been reviewed... there are not free cells in it
                                                    notFreePosition = True
                                                End If
                                            End If
                                        End If
                                    Else
                                        'Error getting the next free cell...
                                        Exit While
                                    End If
                                End While

                                If (Not myGlobalDataTO.HasError) Then
                                    If (freeCell) Then
                                        'Inform the free Ring/Cell in the DS to return
                                        Dim myRingCellNumberDS As New RingCellNumbersDS
                                        Dim myRingCellRow As RingCellNumbersDS.RingCellTableRow

                                        myRingCellRow = myRingCellNumberDS.RingCellTable.NewRingCellTableRow()
                                        myRingCellRow.CellNumber = cellNumber
                                        myRingCellRow.RingNumber = ringNumber
                                        myRingCellNumberDS.RingCellTable.AddRingCellTableRow(myRingCellRow)

                                        myGlobalDataTO.SetDatos = myRingCellNumberDS
                                        myGlobalDataTO.HasError = False
                                    Else
                                        'A free cell was not found, return ErrorCode for ROTOR FULL with HasError = FALSE
                                        myGlobalDataTO.HasError = False
                                        myGlobalDataTO.ErrorCode = "ROTOR_FULL"
                                    End If
                                End If
                            End If
                            'Else
                            'PENDING: Process for A200 Rotor for Samples and Reagents
                        End If
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSRotorContentByPositionDelegate.GetNextRotorPositionForSample", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Search the first free position to place a Control, a Calibrator, a Patient Sample or an Sample Additional Solution
        ''' in the informed Analyzer Rotor.  
        '''    For A400 Model the free position is searched in the SAMPLES Rotor
        '''    For A200 Model all the searching process is pending of definition
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identificator</param>
        ''' <param name="pAnalyzerID">Analyzer Identificator</param>
        ''' <param name="pRotorType">Analyzer Rotor Type</param>
        ''' <param name="pTubeContent">Tube Content (type of Element for which a free Rotor Cell is searched)</param>
        ''' <param name="pMaxRotorRingNumber">Maximum Ring Number</param>
        ''' <returns>GlobalDataTO containing a typed DataSet RingCellNumbersDS with the free position found (Ring and Cell)</returns>
        ''' <remarks>
        ''' Created by:  TR 24/11/2009 - Tested: OK
        ''' Modified by: SA 04/01/2010 - Changes needed in call to GetMaxRingNumber due to a new parameter was added
        '''              SA 04/01/2010 - Error fixed: Function Summary was wrong (copy/paste of summary of a different function)
        '''              SA 13/01/2010 - Changed the way of opening the DB Connection to fulfill the new template
        '''              SA 26/01/2010 - Added new parameter for the maximum Ring Number in the Analyzer Rotor. Removed the call
        '''                              to function GetMaxRingNumber in code. Removed all sentences to convert to datatype Byte 
        ''' </remarks>
        Private Function GetRotorPositionForSample(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, _
                                                   ByVal pAnalyzerID As String, ByVal pRotorType As String, ByVal pTubeContent As String, _
                                                   ByVal pMaxRotorRingNumber As Integer) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim ringNumber As Integer = -1
                        Dim myRotorContentbyPositionDAO As New twksWSRotorContentByPositionDAO

                        If (pRotorType = "SAMPLES") Then
                            If (pMaxRotorRingNumber > 0) Then
                                Dim freeCell As Boolean = False
                                Dim myRingCellNumberDS As New RingCellNumbersDS
                                Dim myRingCellRow As RingCellNumbersDS.RingCellTableRow
                                myRingCellRow = myRingCellNumberDS.RingCellTable.NewRingCellTableRow

                                'Sample contained in the tube is Calibrator or Control
                                If (String.Compare(pTubeContent, "CALIB", False) = 0 OrElse pTubeContent = "CTRL") Then
                                    'Search begins in the internal Ring
                                    ringNumber = pMaxRotorRingNumber

                                    Do While ((Not freeCell) AndAlso (ringNumber >= 1))
                                        'Search minimum free cell in the informed Ring
                                        myGlobalDataTO = myRotorContentbyPositionDAO.GetMinFreeCellByRing(dbConnection, pAnalyzerID, pRotorType, pWorkSessionID, ringNumber)
                                        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                            If (Not String.IsNullOrEmpty(myGlobalDataTO.SetDatos.ToString)) Then
                                                freeCell = True

                                                'Inform the Ring and Cell found in the DS to return
                                                myRingCellRow.RingNumber = ringNumber
                                                myRingCellRow.CellNumber = CType(myGlobalDataTO.SetDatos, Integer)
                                                myRingCellNumberDS.RingCellTable.AddRingCellTableRow(myRingCellRow)
                                            Else
                                                ringNumber -= 1
                                            End If
                                        Else
                                            'Error getting the MinFreeCell in the informed Ring; stop the searching by forcing the EXIT WHILE
                                            ringNumber = 0
                                        End If
                                    Loop
                                Else
                                    'Sample contained in the tube is a Patient Sample
                                    'Search begins in the external Ring
                                    ringNumber = 1
                                    Do While ((Not freeCell) AndAlso (ringNumber <= pMaxRotorRingNumber))
                                        myGlobalDataTO = myRotorContentbyPositionDAO.GetMinFreeCellByRing(dbConnection, pAnalyzerID, pRotorType, pWorkSessionID, ringNumber)

                                        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                            If (Not String.IsNullOrEmpty(myGlobalDataTO.SetDatos.ToString)) Then
                                                freeCell = True

                                                'Inform the Ring and Cell found in the DS to return
                                                myRingCellRow.RingNumber = ringNumber
                                                myRingCellRow.CellNumber = CType(myGlobalDataTO.SetDatos, Integer)
                                                myRingCellNumberDS.RingCellTable.AddRingCellTableRow(myRingCellRow)
                                            Else
                                                ringNumber += 1
                                            End If
                                        Else
                                            'Error getting the MinFreeCell in the informed Ring; stop the searching by forcing the EXIT WHILE
                                            ringNumber = (pMaxRotorRingNumber + 1)
                                        End If
                                    Loop
                                End If

                                If (Not myGlobalDataTO.HasError) Then
                                    If (freeCell) Then
                                        'Return the found Ring/Cell in the GlobalDataTO
                                        myGlobalDataTO.SetDatos = myRingCellNumberDS
                                    Else
                                        'A Free Cell was not found, the Rotor is full
                                        myGlobalDataTO.ErrorCode = "ROTOR_FULL"
                                    End If
                                End If
                            End If
                            'Else
                            'TODO: PENDING: Process for A200 Rotor for Samples and Reagents
                        End If
                    End If
                End If

            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSRotorContentByPositionDelegate.GetRotorPositionForSample", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' For the informed ElementID or ExecutionID, calculate the status to assign to the Rotor Position in which it is placed. The status 
        ''' is calculated based in the current status of all its Executions in the following way:
        '''    ** If at least an Execution is INPROCESS --> NEW Sample Rotor Position Status: INPROCESS
        '''    ** If there are not INPROCESS Executions and at least one Execution is PENDING --> NEW Sample Rotor Position Status: PENDING
        '''    ** All Executions are CLOSED or CLOSEDNOK --> NEW Sample Rotor Position Status: FINISHED
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">WorkSession Identifier</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pElementID">Element Identifier. Informed when the status searching is by Element</param>
        ''' <param name="pTubeContent">Element Type. Informed when the status searching is by Element</param>
        ''' <param name="pMultiItemNumber">For Multipoint Calibrators: the number of item; otherwise, always one. Informed when the status 
        '''                                searching is by Element</param>
        ''' <param name="pExecutionID">Execution Identifier. Informed when the status searching is by Execution</param>
        ''' <returns>If the status searching is by Element: GlobalDataTO containing an string value with the new status to assign to 
        '''                                                 the Element once it is placed in a Rotor Cell
        '''          If the status searching is by Execution: GlobalDataTO containing a typed DataSet WSRotorContentByPositionDS
        '''                                                   with all information of the Rotor Position with the new status updated</returns>
        ''' <remarks>
        ''' Created by:  SA 10/01/2012 - Copy from UpdateSamplePositionStatus to separate the Status calculation from the update  
        ''' Modified by: SA 18/01/2012 - When the searching is done by ExecutionID, call function GetElementInfoByExecutionID to get 
        '''                              data of the related Element and function GetPositionedElements to verify if the Element is 
        '''                              already positioned and get the CellNumber (instead of calling function GetSampleRotorPositionByExecutionID, 
        '''                              the Element can be positioned or not) 
        '''              SA 01/02/2012 - Do not search Executions when an ElementID has not been informed/found. In that case,
        '''                              return an empty WSRotorContentByPositionDS 
        ''' </remarks>
        Private Function GetSamplePositionStatus(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, ByVal pAnalyzerID As String, _
                                                 ByVal pElementID As Integer, ByVal pTubeContent As String, ByVal pMultiItemNumber As Integer, _
                                                 Optional ByVal pExecutionID As Integer = -1) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myElementID As Integer = -1
                        Dim myItemNumber As Integer = 0
                        Dim mySampleClass As String = String.Empty

                        Dim exec_delg As New ExecutionsDelegate
                        Dim myReturnDS As New WSRotorContentByPositionDS

                        If (pExecutionID = -1) Then
                            'Get SampleClass, ElementID and MultiItemNumber from the entry parameters
                            mySampleClass = pTubeContent
                            If (mySampleClass = "TUBE_SPEC_SOL") Then mySampleClass = "BLANK"

                            myElementID = pElementID
                            myItemNumber = pMultiItemNumber
                        Else
                            'For the informed ExecutionID, search the correspondent SampleClass, ElementID and MultiItemNumber 
                            resultData = exec_delg.GetElementInfoByExecutionID(dbConnection, pAnalyzerID, pWorkSessionID, pExecutionID, False)
                            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                myReturnDS = DirectCast(resultData.SetDatos, WSRotorContentByPositionDS)

                                If (myReturnDS.twksWSRotorContentByPosition.Rows.Count > 0) Then
                                    'Get SampleClass and MultiItemNumber from the returned DataSet 
                                    'NOTE: field MultiTubeNumber in the DS contains field MultiItemNumber due to in function GetSampleRotorPositionByExecutionID
                                    '      in class twksWSExecutions obtains "EX.MultiItemNumber As MultiTubeNumber"
                                    mySampleClass = myReturnDS.twksWSRotorContentByPosition(0).SampleClass
                                    myItemNumber = myReturnDS.twksWSRotorContentByPosition(0).MultiTubeNumber
                                    myElementID = myReturnDS.twksWSRotorContentByPosition(0).ElementID

                                    'Verify if the Element is positioned in the Samples Rotor
                                    Dim myWSRotorContentByPosDelegate As New WSRotorContentByPositionDelegate

                                    resultData = myWSRotorContentByPosDelegate.GetPositionedElements(dbConnection, pAnalyzerID, "SAMPLES", myElementID.ToString)
                                    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                        Dim myRotorPosDS As WSRotorContentByPositionDS = DirectCast(resultData.SetDatos, WSRotorContentByPositionDS)

                                        If (myRotorPosDS.twksWSRotorContentByPosition.Rows.Count > 0) Then
                                            'Update fields RingNumber and CellNumber in the DS to return
                                            myReturnDS.BeginInit()
                                            myReturnDS.twksWSRotorContentByPosition(0).RingNumber = myRotorPosDS.twksWSRotorContentByPosition(0).RingNumber
                                            myReturnDS.twksWSRotorContentByPosition(0).CellNumber = myRotorPosDS.twksWSRotorContentByPosition(0).CellNumber
                                            myReturnDS.EndInit()
                                        End If
                                    End If
                                End If
                            End If


                            ''For the informed ExecutionID, search the SampleClass, ElementID and MultiItemNumber for the Sample Tube used by 
                            ''the informed ExecutionID, and get also the Sample Rotor Position used by it
                            'resultData = exec_delg.GetSampleRotorPositionByExecutionID(dbConnection, pWorkSessionID, pAnalyzerID, pExecutionID)
                            'If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            '    myReturnDS = DirectCast(resultData.SetDatos, WSRotorContentByPositionDS)

                            '    If (myReturnDS.twksWSRotorContentByPosition.Rows.Count > 0) Then
                            '        'Get SampleClass and MultiItemNumber from the returned DataSet 
                            '        'NOTE: field MultiTubeNumber in the DS contains field MultiItemNumber due to in function GetSampleRotorPositionByExecutionID
                            '        '      in class twksWSExecutions obtains "EX.MultiItemNumber As MultiTubeNumber"
                            '        mySampleClass = myReturnDS.twksWSRotorContentByPosition(0).SampleClass
                            '        myItemNumber = myReturnDS.twksWSRotorContentByPosition(0).MultiTubeNumber
                            '        myElementID = myReturnDS.twksWSRotorContentByPosition(0).ElementID
                            '    End If
                            'End If
                        End If

                        If (myElementID <> -1) Then
                            'Get all Executions for the informed Element and calculate the new status for the Sample Rotor Position:
                            Dim newPositionStatus As String = ""
                            resultData = exec_delg.GetByElementID(dbConnection, pWorkSessionID, pAnalyzerID, myElementID)
                            If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                                Dim exec_DS As ExecutionsDS = DirectCast(resultData.SetDatos, ExecutionsDS)

                                If (exec_DS.twksWSExecutions.Rows.Count > 0) Then
                                    'Verify if there are INPROCESS Executions for the informed Element
                                    Dim myLinqRes As List(Of ExecutionsDS.twksWSExecutionsRow) = (From a As ExecutionsDS.twksWSExecutionsRow In exec_DS.twksWSExecutions _
                                                                                                 Where String.Compare(a.ExecutionStatus, "INPROCESS", False) = 0 _
                                                                                               AndAlso a.SampleClass = mySampleClass _
                                                                                               AndAlso a.MultiItemNumber = myItemNumber _
                                                                                                Select a).ToList

                                    If (myLinqRes.Count > 0) Then
                                        'There is at least an Execution InProcess, the Cell Status will be also INPROCESS
                                        newPositionStatus = "INPROCESS"
                                    Else
                                        'There are not INPROCESS Executions; verify if there are PENDING Executions for the Element 
                                        myLinqRes = (From a As ExecutionsDS.twksWSExecutionsRow In exec_DS.twksWSExecutions _
                                                    Where a.ExecutionStatus = "PENDING" _
                                                  AndAlso String.Compare(a.SampleClass, mySampleClass, False) = 0 _
                                                  AndAlso a.MultiItemNumber = myItemNumber _
                                                   Select a).ToList

                                        If (myLinqRes.Count > 0) Then
                                            'There is at least an Execution Pending, the Cell Status will be also PENDING
                                            newPositionStatus = "PENDING"
                                        Else
                                            'There are not INPROCESS nor PENDING Executions; verify if there are CLOSED or CLOSEDNOK Executions for the Element 
                                            myLinqRes = (From a As ExecutionsDS.twksWSExecutionsRow In exec_DS.twksWSExecutions _
                                                        Where (String.Compare(a.ExecutionStatus, "CLOSED", False) = 0 OrElse a.ExecutionStatus = "CLOSEDNOK") _
                                                      AndAlso a.SampleClass = mySampleClass _
                                                      AndAlso a.MultiItemNumber = myItemNumber _
                                                       Select a).ToList

                                            If (myLinqRes.Count > 0) Then
                                                'There is at least an Execution Close (OK or NOK), the Cell Status will be FINISHED
                                                newPositionStatus = "FINISHED"
                                            Else
                                                'All Executions are LOCKED for the informed Element... the Cell Status does not change
                                                'RH 22/05/2012 Set the status to PENDING, because status LOCKED is not defined for sample tubes
                                                newPositionStatus = "PENDING"
                                            End If
                                        End If
                                    End If
                                End If

                                If (pExecutionID = -1) Then
                                    'Return the calculated status
                                    resultData.SetDatos = newPositionStatus
                                    resultData.HasError = False
                                Else
                                    'Update the calculated status in DataSet WSRotorContentByPositionDS to return
                                    myReturnDS.twksWSRotorContentByPosition(0).BeginEdit()
                                    myReturnDS.twksWSRotorContentByPosition(0).Status = newPositionStatus
                                    myReturnDS.twksWSRotorContentByPosition(0).EndEdit()
                                    myReturnDS.AcceptChanges()

                                    resultData.SetDatos = myReturnDS
                                    resultData.HasError = False
                                End If
                            End If
                        Else
                            'Return an empty DataSet...
                            resultData.SetDatos = myReturnDS
                            resultData.HasError = False
                        End If
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSRotorContentByPositionDelegate.GetSamplePositionStatus", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Manages the positioning of Patient Samples (manual and automatic). In both cases, the default tube defined for Patient Samples 
        ''' is used.When this function is used for manual positioning, the ring and cell to which the User drag and drop the Element is used. 
        ''' When it is used for automatic positioning, a ring/cell is searched according the rules of automatic positioning of Patient Samples
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWSRotorContentByPositionDS">Typed DataSet containing the basic data needed to place a required Patient Sample Element 
        '''                                           in a Rotor Cell</param>
        ''' <param name="pMaxRotorRingNumber">Maximum Ring Number</param>
        ''' <param name="pAutoPositioning">Flag indicating if the function is used for manual positioning (when False) or automatic positioning
        '''                                (when True)</param>
        ''' <returns>GlobalDataTO containing a typed DataSet WSRotorContentByPositionDS with the information about the Rotor Cell in which the 
        '''          Patient Sample was placed plus the recalculated Element Status. If dilutions are required in the Work Session for the Patient 
        '''          Sample, the DataSet will contain one row for each one of the dilution tubes (when they were positioned)</returns>
        ''' <remarks>
        ''' Created by:  TR 26/11/2009 - Tested: OK
        ''' Modified by: SA 13/01/2010 - Changed the way of opening the DB Transaction to fulfill the new template
        '''              SA 26/01/2010 - Added new parameter for the maximum Ring Number in the Analyzer Rotor and pass it 
        '''                              to functions GetRotorPositionForSample and PrepareSampleForDilution
        '''              SA 18/03/2010 - Remove getting the default Tube Type for Patient Samples from the corresponding
        '''                              User Setting; the TubeType is now informed for the Required Element.  Changes due 
        '''                              function PreparePatientSamples now return a GlobalDataTO
        '''              AG 08/09/2011 - Calculate sample position status (PENDING, INPROCESS,...)
        '''              SA 10/01/2012 - Use value of field ElementFinished to set Cell Status = Finished. Call function GetSamplePositionStatus 
        '''                              only when ElementFinished is False. Implementation improved 
        '''              SA 29/08/2013 - For the Patient Sample that has to be positioned, if the tube does not contain a manual dilution, field BarcodeInfo 
        '''                              is informed in the following way:
        '''                              ** If field SpecimenIDList is informed for the required Patient Sample, fill BarcodeInfo with the first SpecimenID in the list
        '''                                 and set BarcodeStatus = OK
        '''                              ** Otherwise, both Barcode fields remain with NULL value
        '''              XB 07/10/2014 - BA-1978 ==> Add log traces to catch NULL wrong assignment on RealVolume field
        '''              AG 07/10/2014 - BA-1979 ==> Replaced call to function twksWSRotorContentByPositionDAO.Update by a call to function Update 
        '''                                          in this Delegate, informing the process who is updating the content of the Rotor Position (to 
        '''                                          search which process is inserting invalid values: positions with TubeContent but not element ID)
        ''' </remarks>
        Private Function PatientSamplePositioning(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWSRotorContentByPositionDS As WSRotorContentByPositionDS, _
                                                 ByVal pMaxRotorRingNumber As Integer, ByVal pAutoPositioning As Boolean) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim wngMsgReturned As String = String.Empty

                        'Assign the value of the row in the DataSet to a local row...
                        Dim rotorContentPosRow As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow
                        rotorContentPosRow = pWSRotorContentByPositionDS.twksWSRotorContentByPosition(0)

                        'Search details of the Required Element needed to be positioned
                        Dim myWSRequiredElementsDelegate As New WSRequiredElementsDelegate()
                        myGlobalDataTO = myWSRequiredElementsDelegate.GetRequiredElementData(dbConnection, rotorContentPosRow.ElementID)

                        Dim dilutionTube As Boolean = False
                        Dim patientSampleFinished As Boolean = False
                        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                            Dim myRequiredElemDS As WSRequiredElementsDS = DirectCast(myGlobalDataTO.SetDatos, WSRequiredElementsDS)

                            If (myRequiredElemDS.twksWSRequiredElements.Rows.Count > 0) Then
                                'Set value of field ElementFinished to a local variable
                                patientSampleFinished = myRequiredElemDS.twksWSRequiredElements(0).ElementFinished

                                'If it is not informed, set the TubeType in the entry DataSet
                                If (rotorContentPosRow.IsTubeTypeNull) Then rotorContentPosRow.TubeType = myRequiredElemDS.twksWSRequiredElements(0).TubeType

                                'Verify if the Element corresponds to a Predilution Factor
                                dilutionTube = CType(IIf(myRequiredElemDS.twksWSRequiredElements(0).IsPredilutionFactorNull, False, True), Boolean)

                                'SA 29/08/2013 - See comments about this code in the function header
                                If (Not dilutionTube) Then
                                    If (Not myRequiredElemDS.twksWSRequiredElements(0).IsSpecimenIDListNull AndAlso myRequiredElemDS.twksWSRequiredElements(0).SpecimenIDList <> String.Empty) Then
                                        rotorContentPosRow.BarCodeInfo = myRequiredElemDS.twksWSRequiredElements(0).SpecimenIDList.Split(CChar(vbCrLf))(0)
                                        rotorContentPosRow.BarcodeStatus = "OK"
                                    End If
                                End If
                            End If

                            'If function has been called for automatic positioning...
                            If (pAutoPositioning) Then
                                'Search Rotor Cell / Position for the Patient Sample
                                myGlobalDataTO = GetRotorPositionForSample(dbConnection, rotorContentPosRow.WorkSessionID, rotorContentPosRow.AnalyzerID, _
                                                                           rotorContentPosRow.RotorType, rotorContentPosRow.TubeContent, pMaxRotorRingNumber)
                                If (Not myGlobalDataTO.HasError AndAlso String.IsNullOrEmpty(myGlobalDataTO.ErrorCode)) Then
                                    Dim myRingCellPosDS As RingCellNumbersDS = DirectCast(myGlobalDataTO.SetDatos, RingCellNumbersDS)

                                    'A Position was found, inform the Ring/Cell Number found
                                    rotorContentPosRow.RingNumber = myRingCellPosDS.RingCellTable(0).RingNumber
                                    rotorContentPosRow.CellNumber = myRingCellPosDS.RingCellTable(0).CellNumber
                                End If
                            End If
                        End If

                        If (Not myGlobalDataTO.HasError AndAlso String.IsNullOrEmpty(myGlobalDataTO.ErrorCode)) Then
                            If (patientSampleFinished) Then
                                rotorContentPosRow.Status = "FINISHED"
                            Else
                                'Get position status for the Patient Sample...
                                rotorContentPosRow.Status = "PENDING"
                                myGlobalDataTO = GetSamplePositionStatus(dbConnection, rotorContentPosRow.WorkSessionID, rotorContentPosRow.AnalyzerID, _
                                                                         rotorContentPosRow.ElementID, "PATIENT", 1)
                                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                    Dim newStatus As String = DirectCast(myGlobalDataTO.SetDatos, String)
                                    If (String.Compare(newStatus, String.Empty, False) <> 0 AndAlso newStatus <> "FINISHED") Then rotorContentPosRow.Status = newStatus
                                End If
                            End If

                            If (Not myGlobalDataTO.HasError) Then
                                'If the Element is not a PatientSample with manual dilution, search if there are manual dilution for it
                                If (Not dilutionTube) Then
                                    'Prepare dilutions for the Patient Sample (if any)
                                    myGlobalDataTO = PrepareSampleForDilution(dbConnection, rotorContentPosRow, pMaxRotorRingNumber, pAutoPositioning)
                                    If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                                        Dim dilutionPositionsDS As WSRotorContentByPositionDS = DirectCast(myGlobalDataTO.SetDatos, WSRotorContentByPositionDS)

                                        If (dilutionPositionsDS.twksWSRotorContentByPosition.Rows.Count > 0) Then
                                            'Import the received rows (positioned Predilutions) to the DataSet to return
                                            For Each tempRow As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow In dilutionPositionsDS.twksWSRotorContentByPosition
                                                pWSRotorContentByPositionDS.twksWSRotorContentByPosition.ImportRow(tempRow)
                                            Next
                                        End If
                                    End If
                                End If
                            End If

                            If (Not myGlobalDataTO.HasError) Then
                                'If a Rotor Full Warning was returned, save it
                                wngMsgReturned = myGlobalDataTO.ErrorCode

                                Dim tempDS As New WSRotorContentByPositionDS()
                                'Dim myRotorContentByPosDAO As New twksWSRotorContentByPositionDAO()
                                For Each rcpRow As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow In pWSRotorContentByPositionDS.twksWSRotorContentByPosition
                                    'Import the Rotor Position to a temporary DataSet
                                    tempDS.twksWSRotorContentByPosition.ImportRow(rcpRow)

                                    'Update the Rotor Cell/Position informing the Element positioned in it
                                    'myGlobalDataTO = myRotorContentByPosDAO.Update(dbConnection, tempDS)
                                    myGlobalDataTO = Update(dbConnection, "SAMPLES", tempDS, ClassCalledFrom.PatientSamplePositioning) 'AG 07/10/2014 BA-1979 call the Update method  in delegate instead of in DAO
                                    If (myGlobalDataTO.HasError) Then Exit For

                                    'Update Status of the Required Element to Positioned (POS)
                                    myGlobalDataTO = myWSRequiredElementsDelegate.UpdateStatus(dbConnection, rcpRow.ElementID, "POS")
                                    If (myGlobalDataTO.HasError) Then Exit For

                                    'Set field ElementStatus = POS
                                    rcpRow.ElementStatus = "POS"

                                    'Clear the temporary DataSet to reuse it
                                    tempDS.twksWSRotorContentByPosition.Clear()
                                Next
                            End If
                        End If

                        If (Not myGlobalDataTO.HasError) Then
                            'Commit DB Transaction if it was locally opened - return all the assigned Rotor Positions
                            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                            myGlobalDataTO.SetDatos = pWSRotorContentByPositionDS
                            If (String.Compare(wngMsgReturned, String.Empty, False) <> 0) Then myGlobalDataTO.ErrorCode = wngMsgReturned
                        Else
                            If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                        End If
                    End If
                End If
            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                myGlobalDataTO = New GlobalDataTO
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSRotorContentByPositionDelegate.PatientSamplePositioning", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Verify if there are dilutions for an specific Patient Sample and in this case search free Rotor positions for
        ''' each one of them
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pRotorContentPosRow">Rotor Position in which has been placed the non diluted Patient Sample</param>
        ''' <param name="pMaxRotorRingNumber">Maximum Ring Number</param>
        ''' <returns>GlobalDataTO containing a typed DataSet WSRotorContentByPositionDS with all the positioned 
        '''          Patient Sample Dilutions</returns>
        ''' <remarks>
        ''' Created by:  TR 27/06/2009 - Tested: OK.
        ''' Modified by: BK 23/12/2009 - Change datatype Byte to Integer
        '''              SA 26/01/2010 - Added new parameter for the maximum Ring Number in the Analyzer Rotor and pass it 
        '''                              to function GetNextRotorPositionForSample
        '''              AG 28/01/2010 - Exit for if globaldatato has error
        '''              SA 18/03/2010 - Changed the return datatype to a GlobalDataTO; pass the open connection when calling
        '''                              function GetNextRotorPositionForSample
        '''              SA 10/01/2012 - Calculate the dilution Status using field ElementFinished or by calling function GetSamplePositionStatus
        ''' </remarks>
        Private Function PrepareSampleForDilution(ByRef pDBConnection As SqlClient.SqlConnection, ByVal pRotorContentPosRow As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow, _
                                                  ByVal pMaxRotorRingNumber As Integer, ByVal pForAutopositioning As Boolean) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                Dim resultDS As New WSRotorContentByPositionDS

                'Search if there are more Required Elements created for the Patient Sample (case of Sample dilutions)
                Dim myWSRequiredElementsDelegate As New WSRequiredElementsDelegate
                myGlobalDataTO = myWSRequiredElementsDelegate.GetSampleDilutionElements(pDBConnection, pRotorContentPosRow.WorkSessionID, _
                                                                                        pRotorContentPosRow.AnalyzerID, pRotorContentPosRow.RotorType, _
                                                                                        pRotorContentPosRow.ElementID, False, pForAutopositioning)

                If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                    Dim myRequiredElementsDS As WSRequiredElementsDS = DirectCast(myGlobalDataTO.SetDatos, WSRequiredElementsDS)

                    'Set values of RingNumber and CellNumber of the full Patient Sample to local variables; they will be used as reference
                    'to search a free position for the first dilution
                    Dim ringNumber As Integer = pRotorContentPosRow.RingNumber
                    Dim cellNumber As Integer = pRotorContentPosRow.CellNumber

                    'Get the logged User
                    'Dim myGlobalbase As New GlobalBase
                    Dim loggedUser As String = GlobalBase.GetSessionInfo().UserName

                    'Declare a RotorContentByPosition row to add to it the positions for the Patient Sample Dilutions
                    Dim rcpRow As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow
                    For Each reqElementRow As WSRequiredElementsDS.twksWSRequiredElementsRow In myRequiredElementsDS.twksWSRequiredElements
                        'Search next free cell to place the dilution tube (Ring and Cell Number)
                        myGlobalDataTO = GetNextRotorPositionForSample(pDBConnection, pRotorContentPosRow.WorkSessionID, pRotorContentPosRow.AnalyzerID, _
                                                                       pRotorContentPosRow.RotorType, ringNumber, cellNumber, pMaxRotorRingNumber)

                        If (Not myGlobalDataTO.HasError AndAlso String.IsNullOrEmpty(myGlobalDataTO.ErrorCode)) Then
                            Dim rotorPositionDS As RingCellNumbersDS = DirectCast(myGlobalDataTO.SetDatos, RingCellNumbersDS)

                            If (rotorPositionDS.RingCellTable.Rows.Count > 0) Then
                                rcpRow = resultDS.twksWSRotorContentByPosition.NewtwksWSRotorContentByPositionRow()
                                rcpRow.WorkSessionID = pRotorContentPosRow.WorkSessionID
                                rcpRow.AnalyzerID = pRotorContentPosRow.AnalyzerID
                                rcpRow.RotorType = pRotorContentPosRow.RotorType
                                rcpRow.TubeContent = pRotorContentPosRow.TubeContent
                                rcpRow.TubeType = pRotorContentPosRow.TubeType
                                rcpRow.ElementID = reqElementRow.ElementID
                                rcpRow.MultiTubeNumber = 1
                                rcpRow.RingNumber = rotorPositionDS.RingCellTable(0).RingNumber
                                rcpRow.CellNumber = rotorPositionDS.RingCellTable(0).CellNumber

                                If (reqElementRow.ElementFinished) Then
                                    rcpRow.Status = "FINISHED"
                                Else
                                    rcpRow.Status = "PENDING"
                                    myGlobalDataTO = GetSamplePositionStatus(pDBConnection, pRotorContentPosRow.WorkSessionID, pRotorContentPosRow.AnalyzerID, _
                                                                             reqElementRow.ElementID, "PATIENT", 1)
                                    If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                        Dim newStatus As String = DirectCast(myGlobalDataTO.SetDatos, String)
                                        If (String.Compare(newStatus, String.Empty, False) <> 0 AndAlso String.Compare(newStatus, "FINISHED", False) <> 0) Then rcpRow.Status = newStatus
                                    End If
                                End If

                                rcpRow.TS_User = loggedUser
                                rcpRow.TS_DateTime = DateTime.Now

                                'Add the row to the DataSet to return
                                resultDS.twksWSRotorContentByPosition.AddtwksWSRotorContentByPositionRow(rcpRow)

                                'Set values of the found RingNumber and CellNumber to local variables to be used as reference
                                'to get a free position for the next Patient Sample Dilution
                                ringNumber = rcpRow.RingNumber
                                cellNumber = rcpRow.CellNumber
                            End If
                        Else
                            'Error getting the next free position in Samples Rotor (or ROTOR_FULL ErrorCode was returned)
                            Exit For
                        End If
                    Next

                    'Return all positioned Dilutions
                    If (Not myGlobalDataTO.HasError) Then myGlobalDataTO.SetDatos = resultDS
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSRotorContentByPositionDelegate.PrepareSampleForDilution", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Prepare and fill the WSRotorContentByPositionDS with all the positioned Additional Solutions
        ''' </summary>
        ''' <param name="myWSRequiredElementsTreeDS">Required Elements Tree DataSet</param>
        ''' <param name="pWorkSessionID">Work Session Identification</param>
        ''' <param name="pAnalizerID">Analizer Identification</param>
        ''' <param name="pRotorType">Rotor Type</param>
        ''' <param name="pRingNumber">Ring Number</param>
        ''' <param name="pBottleCode">Bottle Code</param>
        ''' <param name="pBottleSize">Bottle Size</param>
        ''' <returns>WSRotorContentByPositionDS fill with all the additional solutions </returns>
        ''' <remarks>
        ''' Created by: TR 24/11/2009
        ''' </remarks>
        Private Function ProcessAdditionalSolutions(ByVal myWSRequiredElementsTreeDS As WSRequiredElementsTreeDS, ByVal pWorkSessionID As String, _
                                                    ByVal pAnalizerID As String, ByVal pRotorType As String, ByVal pRingNumber As Integer, _
                                                    ByVal pBottleCode As String, ByVal pBottleSize As Integer) As WSRotorContentByPositionDS

            Dim myWSRotorContentByPositionDS As New WSRotorContentByPositionDS()

            Try
                Dim myRotorContentByPositionRow As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow
                For Each RETRow As WSRequiredElementsTreeDS.AdditionalSolutionsRow In myWSRequiredElementsTreeDS.AdditionalSolutions.Rows
                    myRotorContentByPositionRow = myWSRotorContentByPositionDS.twksWSRotorContentByPosition.NewtwksWSRotorContentByPositionRow()

                    myRotorContentByPositionRow.AnalyzerID = pAnalizerID
                    myRotorContentByPositionRow.RotorType = pRotorType
                    myRotorContentByPositionRow.RingNumber = pRingNumber
                    myRotorContentByPositionRow.WorkSessionID = pWorkSessionID
                    myRotorContentByPositionRow.ElementID = RETRow.ElementID
                    myRotorContentByPositionRow.MultiTubeNumber = 1
                    myRotorContentByPositionRow.TubeContent = RETRow.TubeContent
                    myRotorContentByPositionRow.TubeType = pBottleCode
                    myRotorContentByPositionRow.RealVolume = pBottleSize
                    myRotorContentByPositionRow.Status = "INUSE"
                    myRotorContentByPositionRow.ScannedPosition = False
                    myRotorContentByPositionRow.SetBarCodeInfoNull()
                    myRotorContentByPositionRow.SetBarcodeStatusNull()

                    myWSRotorContentByPositionDS.twksWSRotorContentByPosition.AddtwksWSRotorContentByPositionRow(myRotorContentByPositionRow)
                Next
            Catch ex As Exception
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSRotorContentByPositionDelegate.ProcesseAdditionalSolutions", EventLogEntryType.Error, False)
            End Try
            Return myWSRotorContentByPositionDS
        End Function

        ''' <summary>
        '''  Prepare the DataSet with all non positioned Calibrators before execute the automatic positioning
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>        
        ''' <param name="pRequiredSamples">Typed DataSet WSRequiredElementsTreeDS containing all Calibrators
        '''                                still non positioned (in table Calibrators)</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pRotorType">Analyzer Rotor Type</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pMaxRotorRingNumber">Maximum Ring Number</param>
        ''' <param name="pNotPositionedCalibrators">When True, it indicates that not all Calibrators could be positioned
        '''                                         It is a parameter by reference</param>
        ''' <returns>GlobalDataTO containing a typed DataSet WSRotorContentByPositionDS with all positions in which the
        '''          Calibrators were placed in the Analyzer Rotor</returns>
        ''' <remarks>
        ''' Created by:  TR 03/12/2009
        ''' Modified by: SA 26/01/2010 - Added new parameter for the maximum Ring Number in the Analyzer Rotor and pass it 
        '''                              to functiona GetRotorPositionForSample and CalibratorPositioning
        '''              SA 18/03/2010 - The Tube Type to place by default in the Rotor Position for the Calibrator will be the assigned to it
        '''              RH 31/08/2011 - Bug corrections. Remove unneeded and memory wasting "New" instructions
        '''              RH 14/09/2011 - Initialize Barcode fields for each position in the DS to return
        '''              RH 31/08/2011 - Actually return all positioned Elements, even an empty DS
        '''              SA 11/01/2012 - Code improved
        ''' </remarks>
        Private Function ProcessCalibratorsForAutoPositioning(ByRef pDBConnection As SqlClient.SqlConnection, ByVal pRequiredSamples As WSRequiredElementsTreeDS, _
                                                              ByVal pAnalyzerID As String, ByVal pRotorType As String, ByVal pWorkSessionID As String, _
                                                              ByVal pMaxRotorRingNumber As Integer, ByRef pNotPositionedCalibrators As Boolean) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                Dim elementID As Integer = 0
                Dim myRotorContentByPosDS As New WSRotorContentByPositionDS
                Dim myTempRotorContentByPosDS As New WSRotorContentByPositionDS
                Dim myRCPRow As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow
                Dim searchElement As List(Of WSRotorContentByPositionDS.twksWSRotorContentByPositionRow)

                'Process all not positioned Calibrators
                For Each myCalibratorRow As WSRequiredElementsTreeDS.CalibratorsRow In pRequiredSamples.Calibrators
                    elementID = myCalibratorRow.ElementID

                    'Before process the following element, validate if it is already in the temporary DataSet
                    searchElement = (From a As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow In myTempRotorContentByPosDS.twksWSRotorContentByPosition _
                                    Where a.ElementID = elementID _
                                   Select a).ToList

                    If (searchElement.Count = 0) Then
                        'Fill fields required to search a position for the Calibrator in the local DataSet 
                        myRCPRow = myRotorContentByPosDS.twksWSRotorContentByPosition.NewtwksWSRotorContentByPositionRow()
                        myRCPRow.AnalyzerID = pAnalyzerID
                        myRCPRow.RotorType = pRotorType
                        myRCPRow.WorkSessionID = pWorkSessionID
                        myRCPRow.ElementID = myCalibratorRow.ElementID
                        myRCPRow.TubeContent = myCalibratorRow.TubeContent
                        myRCPRow.TubeType = myCalibratorRow.TubeType
                        myRCPRow.ScannedPosition = False
                        myRCPRow.SetBarCodeInfoNull()
                        myRCPRow.SetBarcodeStatusNull()
                        myRotorContentByPosDS.twksWSRotorContentByPosition.AddtwksWSRotorContentByPositionRow(myRCPRow)

                        myGlobalDataTO = CalibratorPositioning(pDBConnection, myRotorContentByPosDS, pMaxRotorRingNumber, True)
                        If (myGlobalDataTO.HasError) Then Exit For

                        If (String.IsNullOrEmpty(myGlobalDataTO.ErrorCode)) Then
                            'FREE positions were found for all tubes in the Calibrator Kit... Import all rows to the temporary DataSet 
                            'that will be returned 
                            myRotorContentByPosDS = DirectCast(myGlobalDataTO.SetDatos, WSRotorContentByPositionDS)
                            For Each myRow As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow _
                                           In myRotorContentByPosDS.twksWSRotorContentByPosition
                                myTempRotorContentByPosDS.twksWSRotorContentByPosition.ImportRow(myRow)
                            Next

                        ElseIf (String.Compare(myGlobalDataTO.ErrorCode, "ROTOR_FULL_FOR_CALIBRATOR_KIT", False) = 0) Then
                            'There are not enough FREE cells for a MultipointCalibrator...set the ByRef parameter to True and clean the ErrorCode
                            pNotPositionedCalibrators = True
                            myGlobalDataTO.ErrorCode = String.Empty

                            'Verify if there are FREE positions to continue trying to find a cell for the rest of required Calibrators
                            myGlobalDataTO = CheckAvailablePositionsByRotor(pDBConnection, pAnalyzerID, pRotorType, pWorkSessionID)
                            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                'If there are not more FREE positions, stop the autopositioning
                                If (CType(myGlobalDataTO.SetDatos, Integer) = 0) Then Exit For
                            Else
                                'Error verifying if there are FREE positions; stop the process
                                Exit For
                            End If
                        End If

                        'Clear the local DataSet to reuse it for the next Calibrator to process
                        myRotorContentByPosDS.twksWSRotorContentByPosition.Clear()
                    End If
                Next
                searchElement = Nothing

                'Return all positioned Elements, even an empty DS
                myGlobalDataTO.SetDatos = myTempRotorContentByPosDS

            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSRotorContentByPositionDelegate.ProcessCalibratorsForAutoPositioning", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Prepare the DataSet with all non positioned Controls before execute the automatic positioning
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>        
        ''' <param name="pRequiredSamples">Typed DataSet WSRequiredElementsTreeDS containing all Controls
        '''                                still non positioned (in table Controls)</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pRotorType">Analyzer Rotor Type</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pMaxRotorRingNumber">Maximum Ring Number</param>
        ''' <returns>GlobalDataTO containing a typed DataSet WSRotorContentByPositionDS with all positions in which the
        '''          Controls were placed in the Analyzer Rotor</returns>
        ''' <remarks>
        ''' Created by:  TR 
        ''' Modified by: SA 26/01/2010 - Added new parameter for the maximum Ring Number in the Analyzer Rotor and pass it 
        '''                              to functions GetRotorPositionForSample and ControlPositioning
        '''              TR 29/01/2010 - 
        '''              SA 18/03/2010 - The Tube Type to place by default in the Rotor Position for the Control will be the assigned to it
        '''              RH 14/09/2011 - Initialize Barcode fields for each position in the DS to return
        '''              SA 11/01/2012 - Code improved
        ''' </remarks>
        Private Function ProcessControlsForAutoPositioning(ByRef pDBConnection As SqlClient.SqlConnection, ByVal pRequiredSamples As WSRequiredElementsTreeDS, _
                                                           ByVal pAnalyzerID As String, ByVal pRotorType As String, ByVal pWorkSessionID As String, _
                                                           ByVal pMaxRotorRingNumber As Integer) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                Dim elementID As Integer = 0
                Dim myRotorContentByPosDS As New WSRotorContentByPositionDS
                Dim myTempRotorContentByPosDS As New WSRotorContentByPositionDS
                Dim myRCPRow As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow
                Dim searchElement As List(Of WSRotorContentByPositionDS.twksWSRotorContentByPositionRow)

                'Process all no positioned Controls
                For Each myControlRow As WSRequiredElementsTreeDS.ControlsRow In pRequiredSamples.Controls
                    elementID = myControlRow.ElementID

                    'Before process the following element, validate if it is already in the temporary DataSet
                    searchElement = (From a As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow In myTempRotorContentByPosDS.twksWSRotorContentByPosition _
                                    Where a.ElementID = elementID _
                                   Select a).ToList

                    If (searchElement.Count = 0) Then
                        'Fill fields required to search a position for the Control in the local DataSet 
                        myRCPRow = myRotorContentByPosDS.twksWSRotorContentByPosition.NewtwksWSRotorContentByPositionRow()
                        myRCPRow.AnalyzerID = pAnalyzerID
                        myRCPRow.RotorType = pRotorType
                        myRCPRow.WorkSessionID = pWorkSessionID
                        myRCPRow.ElementID = myControlRow.ElementID
                        myRCPRow.TubeContent = myControlRow.TubeContent
                        myRCPRow.TubeType = myControlRow.TubeType
                        myRCPRow.ScannedPosition = False
                        myRCPRow.SetBarCodeInfoNull()
                        myRCPRow.SetBarcodeStatusNull()
                        myRotorContentByPosDS.twksWSRotorContentByPosition.AddtwksWSRotorContentByPositionRow(myRCPRow)

                        myGlobalDataTO = ControlPositioning(pDBConnection, myRotorContentByPosDS, pMaxRotorRingNumber, True)
                        If (myGlobalDataTO.HasError) Then Exit For

                        If (String.IsNullOrEmpty(myGlobalDataTO.ErrorCode)) Then
                            'A FREE position was found... Import the row to the temporary DataSet that will be returned 
                            myRotorContentByPosDS = DirectCast(myGlobalDataTO.SetDatos, WSRotorContentByPositionDS)
                            myTempRotorContentByPosDS.twksWSRotorContentByPosition.ImportRow(myRotorContentByPosDS.twksWSRotorContentByPosition(0))
                        Else
                            'A FREE position was not found... 
                            'Verify if there are NOT IN USE positions to change the Error Code
                            myGlobalDataTO = CountPositionsByStatus(pDBConnection, pAnalyzerID, pWorkSessionID, pRotorType, "NO_INUSE")
                            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                If (CType(myGlobalDataTO.SetDatos, Integer) > 0) Then
                                    'Rotor is full but there are some NOT IN USE Elements; the ErrorCode to return is changed 
                                    myGlobalDataTO.ErrorCode = "ROTOR_FULL_NOINUSE_CELLS"
                                End If
                            End If
                            Exit For
                        End If

                        'Clear the local DataSet to reuse it for the next Control to process
                        myRotorContentByPosDS.twksWSRotorContentByPosition.Clear()
                    End If
                Next
                searchElement = Nothing

                'Return all positioned Elements, even an empty DS
                myGlobalDataTO.SetDatos = myTempRotorContentByPosDS
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = ""
                myGlobalDataTO.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSRotorContentByPositionDelegate.ProcessControlsForAutoPositioning", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        '''  Prepare the DataSet with all non positioned Patients Samples before execute the automatic positioning
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>        
        ''' <param name="pRequiredSamples">Typed DataSet WSRequiredElementsTreeDS containing all Patient Samples
        '''                                still non positioned (in table Patients)</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pRotorType">Analyzer Rotor Type</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pMaxRotorRingNumber">Maximum Ring Number</param>
        ''' <returns>GlobalDataTO containing a typed DataSet WSRotorContentByPositionDS with all positions in which the
        '''          Patient Samples were placed in the Analyzer Rotor</returns>
        ''' <remarks>
        ''' Created by:  TR 03/12/2009
        ''' Modified by: SA 26/01/2010 - Added new parameter for the maximum Ring Number in the Analyzer Rotor and pass it 
        '''                              to function PatientSamplePositioning
        '''              AG 28/01/2010 - After autopositioning if there are some stat NOPOS and some CALIB NO_INUSE show a error message
        '''              SA 18/03/2010 - The Tube Type to place by default in the Rotor Position for the Patient Sample will be the assigned to it
        '''              SA 11/01/2012 - Code improved
        ''' </remarks>
        Private Function ProcessPatientsForAutoPositioning(ByRef pDBConnection As SqlClient.SqlConnection, ByVal pRequiredSamples As WSRequiredElementsTreeDS, _
                                                           ByVal pAnalyzerID As String, ByVal pRotorType As String, ByVal pWorkSessionID As String, _
                                                           ByVal pMaxRotorRingNumber As Integer) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                Dim elementID As Integer = 0
                Dim myRotorContentByPosDS As New WSRotorContentByPositionDS
                Dim myTempRotorContentByPosDS As New WSRotorContentByPositionDS
                Dim myRCPRow As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow
                Dim searchElement As List(Of WSRotorContentByPositionDS.twksWSRotorContentByPositionRow)

                'Process all non positioned Patient Samples
                For Each patientRow As WSRequiredElementsTreeDS.PatientSamplesRow In pRequiredSamples.PatientSamples
                    elementID = patientRow.ElementID

                    'Before process the following element, validate if it is already in the temporary DataSet
                    searchElement = (From a As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow In myTempRotorContentByPosDS.twksWSRotorContentByPosition _
                                    Where a.ElementID = elementID _
                                   Select a).ToList

                    If (searchElement.Count = 0) Then
                        'Fill fields required to search a position for the Patient Sample in local DataSet 
                        myRCPRow = myRotorContentByPosDS.twksWSRotorContentByPosition.NewtwksWSRotorContentByPositionRow()
                        myRCPRow.AnalyzerID = pAnalyzerID
                        myRCPRow.RotorType = pRotorType
                        myRCPRow.WorkSessionID = pWorkSessionID
                        myRCPRow.ElementID = patientRow.ElementID
                        myRCPRow.TubeContent = patientRow.TubeContent
                        myRCPRow.TubeType = patientRow.TubeType
                        myRCPRow.ScannedPosition = False
                        myRCPRow.SetBarCodeInfoNull()
                        myRCPRow.SetBarcodeStatusNull()
                        myRotorContentByPosDS.twksWSRotorContentByPosition.AddtwksWSRotorContentByPositionRow(myRCPRow)

                        myGlobalDataTO = PatientSamplePositioning(pDBConnection, myRotorContentByPosDS, pMaxRotorRingNumber, True)
                        If (myGlobalDataTO.HasError) Then Exit For

                        'FREE positions were found for all patient tubes (full Sample and manual dilutions)... 
                        'Import all rows to the temporary DataSet that will be returned 
                        myRotorContentByPosDS = DirectCast(myGlobalDataTO.SetDatos, WSRotorContentByPositionDS)
                        For Each myRow As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow _
                                       In myRotorContentByPosDS.twksWSRotorContentByPosition
                            'Verify a Cell has been found. In Patients case is not enough to verify the ErrorCode, due to the special 
                            'management of dilution tubes
                            If (Not myRow.IsCellNumberNull) Then myTempRotorContentByPosDS.twksWSRotorContentByPosition.ImportRow(myRow)
                        Next

                        If (Not String.IsNullOrEmpty(myGlobalDataTO.ErrorCode)) Then
                            'A FREE position was not found for at least one of the patient tubes... 
                            'Verify if there are NOT IN USE positions to change the Error Code
                            myGlobalDataTO = CountPositionsByStatus(pDBConnection, pAnalyzerID, pWorkSessionID, pRotorType, "NO_INUSE")
                            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                If (CType(myGlobalDataTO.SetDatos, Integer) > 0) Then
                                    'Rotor is full but there are some NOT IN USE Elements; the ErrorCode to return is changed 
                                    myGlobalDataTO.ErrorCode = "ROTOR_FULL_NOINUSE_CELLS"
                                Else
                                    'Rotor is full and there are not NOT IN USE Elements
                                    myGlobalDataTO.ErrorCode = "ROTOR_FULL"
                                End If
                            End If
                            Exit For
                        End If

                        'Clear the local DataSet to reuse it for the next Patient Sample to process
                        myRotorContentByPosDS.twksWSRotorContentByPosition.Clear()
                    End If
                Next
                searchElement = Nothing

                'Return all positioned Elements, even an empty DS
                myGlobalDataTO.SetDatos = myTempRotorContentByPosDS
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = ""
                myGlobalDataTO.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSRotorContentByPositionDelegate.ProcessPatientsForAutoPositioning", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Validate the required number of bottles to be positioned and get the positioned ones for
        ''' the reagent, then subtract the positioned to the required
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pAnalyzeID"></param>
        ''' <param name="pWorkSessionID">WorkSession ID</param>
        ''' <param name="pElementID">ElementID to evaluate</param>
        ''' <param name="pReagentTubeTypesDS">DS with Required number of bottles by Tube code</param>
        ''' <returns>A dataset containing the real number of bottles to be positioned by tube code</returns>
        ''' <remarks>
        ''' Created by:  TR 04/02/2010 - TESTED: OK
        ''' Modified by: SA 29/07/2010 - Implement the template for DB access
        '''              SA 14/02/2012 - Changed the way of calculate the number of needed bottles: it has to be done based in the
        '''                              positioned volume against the required volume
        ''' </remarks>
        Private Function ProcessReagentPositionedBottle(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzeID As String, _
                                                        ByVal pWorkSessionID As String, ByVal pElementID As Integer, _
                                                        ByVal pReagentTubeTypesDS As ReagentTubeTypesDS) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        If (pReagentTubeTypesDS.ReagentTubeTypes.Rows.Count > 0) Then
                            'Get the number of positioned bottles for the informed ElementID
                            myGlobalDataTO = CountPositionedReagentsBottlesByElementID(dbConnection, pAnalyzeID, pWorkSessionID, pElementID)
                            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                Dim tempReagentTubeTypesDS As ReagentTubeTypesDS = DirectCast(myGlobalDataTO.SetDatos, ReagentTubeTypesDS)

                                Dim myTubeCode As String
                                Dim myNeededVolume As Single

                                Dim myPendingVol As Single
                                Dim myPendingBottles As Integer
                                For Each reagTubeTypeRow As ReagentTubeTypesDS.ReagentTubeTypesRow In pReagentTubeTypesDS.ReagentTubeTypes.Rows
                                    myTubeCode = reagTubeTypeRow.TubeCode
                                    myNeededVolume = reagTubeTypeRow.NumOfBottles * reagTubeTypeRow.TubeVolume

                                    'Use LINQ TO filter the tempReagentTubeTypesDS and sub the result value to the recived data set
                                    Dim qTempDS = (From a In tempReagentTubeTypesDS.ReagentTubeTypes _
                                                  Where String.Compare(a.TubeCode, myTubeCode, False) = 0 _
                                                 Select a).ToList()

                                    If (qTempDS.Count > 0) Then
                                        If (qTempDS.First.RealVolume < myNeededVolume) Then
                                            myPendingVol = myNeededVolume - qTempDS.First.RealVolume

                                            myPendingBottles = CType(myPendingVol / reagTubeTypeRow.TubeVolume, Integer)
                                            If (myPendingVol Mod reagTubeTypeRow.TubeVolume) > 0 Then myPendingBottles += 1

                                            reagTubeTypeRow.NumOfBottles = myPendingBottles
                                        Else
                                            'All needed Bottles of this size are already positioned
                                            reagTubeTypeRow.NumOfBottles = 0
                                        End If

                                        'If (reagTubeTypeRow.NumOfBottles > qTempDS.First.NumOfBottles) Then
                                        '    reagTubeTypeRow.NumOfBottles = reagTubeTypeRow.NumOfBottles - qTempDS.First().NumOfBottles
                                        'Else
                                        '    'All needed Bottles of this size are already positioned
                                        '    reagTubeTypeRow.NumOfBottles = 0
                                        'End If
                                    End If
                                Next
                                myGlobalDataTO.SetDatos = pReagentTubeTypesDS
                            End If
                        End If
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = ""
                myGlobalDataTO.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSRotorContentByPositionDelegate.ProcessReagentPositionedBottle", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Prepare the DataSet with all non positioned Tube Additional Solutions before execute the automatic positioning
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>        
        ''' <param name="pRequiredSamples">Typed DataSet WSRequiredElementsTreeDS containing all Tube Additional Solutions
        '''                                still non positioned (in table TubeAdditionalSolutions)</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pRotorType">Analyzer Rotor Type</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pMaxRotorRingNumber">Maximum Ring Number</param>
        ''' <returns>GlobalDataTO containing a typed DataSet WSRotorContentByPositionDS with all positions in which the
        '''          Tube Additional Solutions were placed in the Analyzer Rotor</returns>
        ''' <remarks>
        ''' Created by:  RH 16/06/2011
        ''' Modified by: RH 14/09/2011 - Initialize Barcode fields for each position in the DS to return
        '''              SA 11/01/2012 - Code improved
        '''              TR 13/03/2012 - Excluded the ISE Washing solution from the autopositioning on sample rotor
        ''' </remarks>
        Private Function ProcessTubeAdditionalSolutionsForAutoPositioning(ByRef pDBConnection As SqlClient.SqlConnection, ByVal pRequiredSamples As WSRequiredElementsTreeDS, _
                                                                          ByVal pAnalyzerID As String, ByVal pRotorType As String, ByVal pWorkSessionID As String, _
                                                                          ByVal pMaxRotorRingNumber As Integer) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                Dim elementID As Integer = 0
                Dim myRotorContentByPosDS As New WSRotorContentByPositionDS
                Dim myTempRotorContentByPosDS As New WSRotorContentByPositionDS
                Dim myRCPRow As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow
                Dim searchElement As List(Of WSRotorContentByPositionDS.twksWSRotorContentByPositionRow)

                'Process all non positioned Tube Additional Solutions
                For Each row As WSRequiredElementsTreeDS.TubeAdditionalSolutionsRow In pRequiredSamples.TubeAdditionalSolutions
                    If Not String.Compare(row.SolutionCode, "WASHSOL3", False) = 0 Then
                        elementID = row.ElementID

                        'Before process the element, validate if it is already in the temporary DataSet
                        searchElement = (From a As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow In myTempRotorContentByPosDS.twksWSRotorContentByPosition _
                                        Where a.ElementID = elementID _
                                       Select a).ToList

                        If (searchElement.Count = 0) Then
                            'Fill fields required to search a position for the Tube Additional Solution in local DataSet 
                            myRCPRow = myRotorContentByPosDS.twksWSRotorContentByPosition.NewtwksWSRotorContentByPositionRow()
                            myRCPRow.AnalyzerID = pAnalyzerID
                            myRCPRow.RotorType = pRotorType
                            myRCPRow.WorkSessionID = pWorkSessionID
                            myRCPRow.ElementID = row.ElementID
                            myRCPRow.TubeContent = row.TubeContent
                            myRCPRow.TubeType = row.TubeType

                            myRCPRow.ScannedPosition = False
                            myRCPRow.SetBarCodeInfoNull()
                            myRCPRow.SetBarcodeStatusNull()
                            myRotorContentByPosDS.twksWSRotorContentByPosition.AddtwksWSRotorContentByPositionRow(myRCPRow)

                            'Search a FREE Position in Samples Rotor
                            myGlobalDataTO = AdditionalTubeSolutionPositioning(pDBConnection, myRotorContentByPosDS, pMaxRotorRingNumber, True)
                            If (myGlobalDataTO.HasError) Then Exit For

                            If (String.IsNullOrEmpty(myGlobalDataTO.ErrorCode)) Then
                                'A FREE position was found... Import the row to the temporary DataSet that will be returned 
                                myRotorContentByPosDS = DirectCast(myGlobalDataTO.SetDatos, WSRotorContentByPositionDS)
                                myTempRotorContentByPosDS.twksWSRotorContentByPosition.ImportRow(myRotorContentByPosDS.twksWSRotorContentByPosition(0))
                            Else
                                'A FREE position was not found... 
                                'Verify if there are NOT IN USE positions to change the Error Code
                                myGlobalDataTO = CountPositionsByStatus(pDBConnection, pAnalyzerID, pWorkSessionID, pRotorType, "NO_INUSE")
                                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                    If (CType(myGlobalDataTO.SetDatos, Integer) > 0) Then
                                        'Rotor is full but there are some NOT IN USE Elements; the ErrorCode to return is changed 
                                        myGlobalDataTO.ErrorCode = "ROTOR_FULL_NOINUSE_CELLS"
                                    End If
                                End If
                                Exit For
                            End If

                            'Clear the local DataSet to reuse it for the next Tube Additional Solution to process
                            myRotorContentByPosDS.twksWSRotorContentByPosition.Clear()
                        End If
                    End If
                Next
                searchElement = Nothing

                'Return all positioned Elements, even an empty DS
                myGlobalDataTO.SetDatos = myTempRotorContentByPosDS
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = String.Empty
                myGlobalDataTO.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSRotorContentByPositionDelegate.ProcessTubeAdditionalSolutionsForAutoPositioning", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Manages the manual positioning of Reagents.  The default bottle used will depend on the Analyzer Model. 
        '''    For A400 model:	
        '''       * If the Reagent is dropped in a cell of the external Ring, then the smallest bottle is used 
        '''         (but not the “death volume” bottle, which is only for User selection).
        '''       * If the Reagent is dropped in a cell of the internal Ring, then the smallest bottle not allowed 
        '''         in the external Ring is used.
        '''    For A200 model or Non-Cooled Rack: process is pending of definition
        ''' After positioning a Reagent bottle, the number of remaining Tests that can be executed with the volume 
        ''' contained in the placed bottle is calculated.
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pReagents">Typed DataSet WSRotorContentByPositionDS containing the basic data needed to place 
        '''                        a required Reagent Element in a Rotor Cell</param>
        ''' <returns>GlobalDataTO containing a typed DataSet WSRotorContentByPositionDS with the information about the Rotor Cell 
        '''          in which the Reagent was placed plus the recalculated Element Status</returns>
        ''' <remarks>
        ''' Created by:  AG 30/11/2009 - Tested: OK 18/12/2009 
        ''' Modified by: SA 23/12/2009 - Function was returning an empty DataSet inside the GlobalDataTO; this is wrong,
        '''                              function must return the same DataSet received as entry with the obtained fields
        '''                              TubeType, RealVolume and ElementStatus updated
        '''              VR 29/12/2009 - Change the Constant value to Enum Value
        '''              SA 13/01/2010 - Changed the way of opening the DB Connection to fulfill the new template
        '''              RH 31/08/2011 - Code optimization. short-circuit evaluation. Remove unneeded and memory wasting "New" instructions
        '''              SA 12/01/2012 - This function has to be a DB Transaction instead of a DB Connection; implementation improved
        ''' </remarks>
        Private Function ReagentPositioning(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pReagents As WSRotorContentByPositionDS) As GlobalDataTO
            Dim dataToReturn As GlobalDataTO
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                dataToReturn = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not dataToReturn.HasError AndAlso Not dataToReturn.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(dataToReturn.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Check if Rotor Type equals to Reagents Rotor Type
                        If (String.Compare(pReagents.twksWSRotorContentByPosition(0).RotorType, "REAGENTS", False) <> 0) Then
                            'TODO: PENDING (Process for not cooled rack || process for A200 rotor for samples & reagents)
                        Else
                            Dim reagentsTubes As New ReagentTubeTypesDelegate

                            'Check if we are in the first Ring and then search for the minimum volume of small bottles
                            If (pReagents.twksWSRotorContentByPosition(0).RingNumber = 1) Then
                                dataToReturn = reagentsTubes.GetMinimumBottleSize(dbConnection)
                            Else
                                'Search for minimum volume of big bottles
                                dataToReturn = GeneralSettingsDelegate.GetGeneralSettingValue(dbConnection, GeneralSettingsEnum.BIG_BOTTLE_MIN_VOLUME)
                            End If

                            If (Not dataToReturn.HasError AndAlso Not dataToReturn.SetDatos Is Nothing) Then
                                Dim minVolume As Integer = CType(dataToReturn.SetDatos, Integer)

                                'Get the bottle code for the minimum volume found
                                dataToReturn = reagentsTubes.GetBottleByVolume(dbConnection, minVolume)
                                If (Not dataToReturn.HasError AndAlso Not dataToReturn.SetDatos Is Nothing) Then
                                    Dim bottleCode As String = dataToReturn.SetDatos.ToString

                                    pReagents.twksWSRotorContentByPosition.Rows(0).BeginEdit()
                                    pReagents.twksWSRotorContentByPosition(0).TubeType = bottleCode
                                    pReagents.twksWSRotorContentByPosition(0).RealVolume = minVolume
                                    pReagents.twksWSRotorContentByPosition(0).Status = "INUSE"
                                    pReagents.twksWSRotorContentByPosition.Rows(0).EndEdit()

                                    'Calculate the number of remaining Tests according the positioned volume and update the Rotor Position 
                                    dataToReturn = Me.UpdateReagentPosition(dbConnection, pReagents)
                                End If
                            End If
                        End If

                        If (Not dataToReturn.HasError) Then
                            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                            dataToReturn.SetDatos = pReagents
                        Else
                            If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                        End If
                    End If
                End If
            Catch ex As Exception
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                dataToReturn = New GlobalDataTO()
                dataToReturn.HasError = True
                dataToReturn.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                dataToReturn.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSRotorContentByPositionDelegate.ReagentPositioning", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return dataToReturn
        End Function

        ''' <summary>
        ''' Evaluate if data in parameters contains invalid values. In this case do nothing but inform into internal LOG
        ''' -	Status not “FREE” And Status not “NO_INUSE” And ElementID = vbNULL
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pRotorType">Rotor Type: SAMPLES or REAGENTS</param>
        ''' <param name="pRotorPositionsDS">Typed DataSet WSRotorContentByPositionDS containing data of the Rotor Positions to update</param>
        ''' <param name="pProcessWhoCalls">Enumerate that informs the process who calls the delegate instance</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  AG 07/10/2014 - BA-1979 ==> Add log traces to catch NULL values in field ElementID for Rotor Positions with Status 
        '''                                          different of FREE and/or NO_INUSE
        ''' Modified by: XB 08/10/2014 - BA-1978 ==> Add log traces to catch NULL wrong assignment on RealVolume field
        ''' </remarks>
        Private Function CheckForInvalidPosition(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pRotorType As String, ByVal pRotorPositionsDS As WSRotorContentByPositionDS, _
                                                 ByVal pProcessWhoCalls As ClassCalledFrom) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim textDetails As String = "(" & pProcessWhoCalls.ToString & ")"
                        textDetails &= " - (" & pRotorType & " rotor)"

                        'Dim myLogAcciones As New ApplicationLogManager()
                        Dim linqRes As List(Of WSRotorContentByPositionDS.twksWSRotorContentByPositionRow)

                        linqRes = (From a As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow In pRotorPositionsDS.twksWSRotorContentByPosition _
                                  Where Not a.IsStatusNull AndAlso a.Status <> "FREE" AndAlso a.Status <> "NO_INUSE" AndAlso a.IsElementIDNull _
                                 Select a).ToList

                        If (linqRes.Count > 0) Then
                            GlobalBase.CreateLogActivity("Invalid values: Position with status <> FREE and <> NO_INUSE with ElementID = vbNULL " & textDetails, "WSRotorContentByPositionDelegate.CheckForInvalidPosition", EventLogEntryType.Error, False)
                        End If

                        If (pRotorType = "REAGENTS") Then
                            linqRes = (From a As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow In pRotorPositionsDS.twksWSRotorContentByPosition _
                                      Where Not a.IsStatusNull AndAlso a.Status <> "FREE" AndAlso a.IsRealVolumeNull _
                                     Select a).ToList

                            If (linqRes.Count > 0) Then
                                GlobalBase.CreateLogActivity("Invalid values: REAGENTS rotor Position with status <> FREE with RealVolume = vbNULL " & textDetails, "WSRotorContentByPositionDelegate.CheckForInvalidPosition", EventLogEntryType.Error, False)
                            End If
                        End If
                        linqRes = Nothing
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSRotorContentByPositionDelegate.CheckForInvalidPosition", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            resultData.HasError = False 'Not inform error flag in this method!!
            Return resultData
        End Function
#End Region
    End Class
End Namespace

