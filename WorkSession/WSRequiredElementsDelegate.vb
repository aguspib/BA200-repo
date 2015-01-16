Option Strict On
Option Explicit On

Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.DAL.DAO
Imports Biosystems.Ax00.Global.TO
Imports Biosystems.Ax00.Global.GlobalEnumerates

Namespace Biosystems.Ax00.BL

    Public Class WSRequiredElementsDelegate

#Region "Public Methods"

        ''' <summary>
        ''' Add a new Required Element for a Work Session
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="twksWSRequiredElements">DataSet containing data of the Required Element to add</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:
        ''' Modified by: SA 09/03/2010 - Changes to open the DB Transaction according the new template
        ''' Modified by: SG 29/04/2013 - Insert SpecimenList value when informed
        ''' </remarks>
        Public Function AddRequiredElement(ByVal pDBConnection As SqlClient.SqlConnection, ByVal twksWSRequiredElements As WSRequiredElementsDS, Optional pSpecimenIDList As String = "") _
                                           As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myRequiredPosDAO As New twksWSRequiredElementsDAO()
                        resultData = myRequiredPosDAO.Create(dbConnection, twksWSRequiredElements, pSpecimenIDList)

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
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSRequiredElementsDelegate.AddRequiredElement", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get the total volume required for the Reagents needed in a WorkSession
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection </param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pRotorType">Analyzer Rotor Type</param>
        ''' <param name="pElementID">Element Identifier</param>
        ''' <param name="pNotForClear">When True, indicates this function is not used to calculate Reagent Status after remove 
        '''                            Reagent bottles from the Analyzer Rotor</param>
        ''' <returns>GlobalDataTO containing an string value with the calculated Reagent Status</returns>
        ''' <remarks>
        ''' Created by:  VR 21/11/2009    - Tested: OK
        ''' Modified by: SA 08/01/2010    - Errors fixed to fulfill the function design
        '''              SA,TR 03/02/2010 - New validation to fix the wrong reagent status value 
        '''              SA 06/02/2012    - Changed the function template. When the total positioned volume of the informed Element is zero, then return
        '''                                 "INCOMPLETE" as ElementStatus
        '''              SA 02/03/2012    - Undo the previous change; when the total positioned volume of the informed Element is zero, then return "NOPOS"
        '''                                 as ElementStatus
        '''              SA 29/05/2014    - BT #1627 ==> The final validation comparing the required volume against the total positioned volume must be done
        '''                                              only for Reagents; for Dilution and Washing Solutions the required volume is unknown
        ''' </remarks>
        Public Function CalculateReagentStatus(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pRotorType As String, _
                                               ByVal pElementID As Integer, ByVal pNotForClear As Boolean) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myWSRequiredEleDAO As New twksWSRequiredElementsDAO
                        resultData = myWSRequiredEleDAO.Read(dbConnection, pElementID)

                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            Dim myRequiredVolume As WSRequiredElementsDS = DirectCast(resultData.SetDatos, WSRequiredElementsDS)

                            If (myRequiredVolume.twksWSRequiredElements.Rows.Count > 0) Then
                                'Get the positioned volume and the number of positioned bottles of each different bottle type for the required Reagent
                                Dim myRotorContentByPosition As New WSRotorContentByPositionDelegate
                                resultData = myRotorContentByPosition.GetPositionedReagentVolume(dbConnection, pAnalyzerID, pRotorType, pElementID)

                                If (Not resultData.HasError) Then
                                    Dim myTotalVolume As Integer = 0
                                    If (Not resultData.SetDatos Is DBNull.Value) Then myTotalVolume = CType(resultData.SetDatos, Integer)

                                    Dim reagentStatus As String = "POS"
                                    If (Not pNotForClear) Then
                                        reagentStatus = "NOPOS"
                                    ElseIf (myTotalVolume = 0) Then
                                        reagentStatus = "NOPOS"
                                    End If

                                    'BT #1627 - The validation comparing the required volume against the total positioned volume must be done
                                    '           only for Reagents; for Dilution and Washing Solutions the required volume is unknown
                                    If (myRequiredVolume.twksWSRequiredElements.First.TubeContent = "REAGENT") Then
                                        'Validate the required volume and the total volume to know if the Element is incomplete
                                        If (myTotalVolume > 0 AndAlso myRequiredVolume.twksWSRequiredElements(0).RequiredVolume > myTotalVolume) Then
                                            reagentStatus = "INCOMPLETE"
                                        ElseIf (myTotalVolume > 0 AndAlso myRequiredVolume.twksWSRequiredElements(0).RequiredVolume <= myTotalVolume) Then
                                            reagentStatus = "POS"
                                        End If
                                    End If

                                    resultData.SetDatos = reagentStatus
                                    resultData.HasError = False
                                End If
                            End If
                        End If
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSRequiredElementsDelegate.CalculateReagentStatus", EventLogEntryType.Error, False)
            Finally
                'When a Database Connection was locally opened, it is closed
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Calculate the Status (POS/NOPOS/INCOMPLETE) of a required Reagent identified for the informed ElementID, and update the quantity of Bottles
        ''' of all available sizes that are needed to positioning all the required volume currently not positioned
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pElementRow">Row of typed Dataset WSRequiredElementsDS containing data of the required Reagent</param>
        ''' <param name="pReagentVolume">Programmed Reagent Volume; if it is not informed, it is obtained in this function</param>
        ''' <param name="pCheckPendingExecutions">Optional parameter. When TRUE, it indicates the volume needed to execute all pending Executions has to be
        '''                                       added to the volume needed for new requested Order Tests using the Reagent.  Default value is FALSE, and 
        '''                                       it is set to TRUE only when the current WS is modified and the volume of an existing Reagent is
        '''                                       recalculated (from WorkSessionsDelegate.AddWSElementsForReagents</param>
        ''' <returns>GlobalDataTO containing a row of typed Dataset WSRequiredElementsDS with the data of the required Reagent after 
        '''          updating the Required Volume and the Element Status</returns>
        ''' <remarks>
        ''' Created by:  SA 15/02/2012
        ''' Modified by: SA 27/02/2012 - Before getting the total volume needed for all Executions pending to execute, verify if there are
        '''                              Order Tests recently added to the WorkSession but which Executions have not been generated; in this 
        '''                              case, bottles and status are calculated using the total needed Required volume for the Reagent
        '''              SA 02/03/2012 - Changed the calling to function GetReagentTubesByElement due it was modified by removing the parameter for 
        '''                              the ReagentVolume needed for a preparation
        '''                            - When the total volume positioned is zero, the Reagent has to be marked as NOPOS although there are empty Bottles
        '''                              positioned in the Rotor         
        '''              SA 06/03/2012 - Passed the ElementID as parameter when calling function CountOTsWithoutExecutions. 
        ''' </remarks>
        Public Function CalculateNeededBottlesAndReagentStatus(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
                                                               ByVal pElementRow As WSRequiredElementsDS.twksWSRequiredElementsRow, ByVal pReagentVolume As Single, _
                                                               Optional ByVal pCheckPendingExecutions As Boolean = False) As GlobalDataTO
            Dim dataToReturn As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                dataToReturn = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not dataToReturn.HasError AndAlso Not dataToReturn.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(dataToReturn.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myReqVolume As Single = 0
                        If (pReagentVolume = 0) Then
                            'Get the Programmed Reagent Volume and the RequiredVolume currently saved for the Test/SampleType 
                            dataToReturn = GetProgrammedReagentVol(dbConnection, pElementRow.WorkSessionID, pElementRow.ElementID)
                            If (Not dataToReturn.HasError AndAlso Not dataToReturn.SetDatos Is Nothing) Then
                                Dim myWSRequiredElementDS As WSRequiredElementsDS = DirectCast(dataToReturn.SetDatos, WSRequiredElementsDS)
                                If myWSRequiredElementDS.twksWSRequiredElements.Rows.Count > 0 Then 'AG 29/05/2012
                                    pReagentVolume = myWSRequiredElementDS.twksWSRequiredElements.First.ReagentVolume / 1000
                                    myReqVolume = myWSRequiredElementDS.twksWSRequiredElements.First.RequiredVolume
                                End If
                            End If
                        End If

                        If (Not dataToReturn.HasError) Then
                            'Count the number of OrderTests related with the Reagent but without Executions (those recently added to the WS)
                            Dim myWSOTsDelegate As New WSOrderTestsDelegate
                            dataToReturn = myWSOTsDelegate.CountOTsWithoutExecutions(dbConnection, pElementRow.WorkSessionID, pElementRow.ElementID)
                            If (Not dataToReturn.HasError AndAlso Not dataToReturn.SetDatos Is Nothing) Then
                                Dim checkExecutions As Boolean = True

                                If (Not dataToReturn.SetDatos Is DBNull.Value) Then
                                    If (CType(dataToReturn.SetDatos, Integer) > 0) Then
                                        If (Not pCheckPendingExecutions) Then
                                            'The total needed Reagent Volume for the required Reagent is used
                                            pElementRow.RequiredVolume = myReqVolume
                                            checkExecutions = False
                                        End If
                                    Else
                                        'The Reagent is not new, nothing to add to the total required volume
                                        myReqVolume = 0
                                    End If
                                End If

                                If (checkExecutions) Then
                                    'Calculate the Reagent Volume needed for all Tests pending to execute...
                                    Dim wsExecutionsDelegate As New ExecutionsDelegate
                                    dataToReturn = wsExecutionsDelegate.CountNotClosedSTDExecutions(dbConnection, pAnalyzerID, pElementRow.WorkSessionID, pElementRow.ElementID)

                                    If (Not dataToReturn.HasError AndAlso Not dataToReturn.SetDatos Is Nothing) Then
                                        If (Not dataToReturn.SetDatos Is DBNull.Value) Then
                                            'There are Executions for the Element, calculate the volume needed for all pending work
                                            pElementRow.RequiredVolume += (CType(dataToReturn.SetDatos, Integer) * pReagentVolume) + myReqVolume
                                        End If
                                    End If
                                End If
                            End If
                        End If

                        If (Not dataToReturn.HasError) Then
                            'Calculate the total volume positioned for the Reagent
                            Dim reagentPosition As New WSRotorContentByPositionDelegate
                            dataToReturn = reagentPosition.GetPositionedReagentVolume(dbConnection, pAnalyzerID, "REAGENTS", pElementRow.ElementID)

                            If (Not dataToReturn.HasError AndAlso Not dataToReturn.SetDatos Is Nothing) Then
                                Dim totalRealVolume As Single = 0
                                If (Not dataToReturn.SetDatos Is DBNull.Value) Then totalRealVolume = CType(dataToReturn.SetDatos, Single)

                                If (totalRealVolume = 0) Then
                                    pElementRow.ElementStatus = "NOPOS"

                                    ''If positioned volume is zero, verify if there are depleted bottles positioned
                                    'dataToReturn = reagentPosition.GetPlacedTubesByElement(dbConnection, pAnalyzerID, "REAGENTS", pElementRow.WorkSessionID, pElementRow.ElementID)
                                    'If (Not dataToReturn.HasError AndAlso Not dataToReturn.SetDatos Is Nothing) Then
                                    '    If (Not dataToReturn.SetDatos Is DBNull.Value AndAlso CType(dataToReturn.SetDatos, Integer) > 0) Then
                                    '        'Positioned Volume is not enough, Element Status is set to Incomplete
                                    '        pElementRow.ElementStatus = "INCOMPLETE"
                                    '    End If

                                    'Calculate the number of Bottles of different sizes required for the Reagent according the total required Reagent Volume 
                                    'pending to positioning
                                    Dim myWSDelegate As New WorkSessionsDelegate
                                    dataToReturn = myWSDelegate.GetReagentTubesByElement(dbConnection, pElementRow.ElementID, pElementRow.RequiredVolume)
                                    'End If

                                ElseIf (totalRealVolume < pElementRow.RequiredVolume) Then
                                    'Positioned Volume is not enough, Element Status is set to Incomplete
                                    pElementRow.ElementStatus = "INCOMPLETE"

                                    'Calculate the number of Bottles of different sizes required for the Reagent according the required Reagent Volume pending to 
                                    'positioning, which is (RequiredVolume - PositionedVolume)
                                    Dim myWSDelegate As New WorkSessionsDelegate
                                    dataToReturn = myWSDelegate.GetReagentTubesByElement(dbConnection, pElementRow.ElementID, (pElementRow.RequiredVolume - totalRealVolume))
                                Else
                                    'Positioned Volume is enough, Element Status is POS
                                    pElementRow.ElementStatus = "POS"

                                    'The Element is fully positioned, update to zero the number of Required Bottles of all available sizes
                                    Dim myWSDelegate As New WorkSessionsDelegate
                                    dataToReturn = myWSDelegate.GetReagentTubesByElement(dbConnection, pElementRow.ElementID, 0)
                                End If
                            End If
                        End If
                    End If

                    If (Not dataToReturn.HasError) Then
                        'When the Database Connection was opened locally, then the Commit is executed
                        If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                        dataToReturn.SetDatos = pElementRow
                    Else
                        'When the Database Connection was opened locally, then the Rollback is executed
                        If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                    End If
                End If
            Catch ex As Exception
                dataToReturn = New GlobalDataTO()
                dataToReturn.HasError = True
                dataToReturn.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                dataToReturn.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSRequiredElementsDelegate.CalculateNeededBottlesAndReagentStatus", EventLogEntryType.Error, False)
            Finally
                'When a Database Connection was locally opened, it is closed
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return dataToReturn
        End Function

        ''' <summary>
        ''' Calculate the number of remaining Tests that can be executed with the volume currently placed in the Analyzer Rotor 
        ''' for the specified Reagent
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSession">Work Session Identifier</param>
        ''' <param name="pElementID"> Element Identifier</param>
        ''' <param name="pRealBottleVolume">Real Bottle Volume</param>
        ''' <returns>GlobalDataTO containing an integer value with the number of remaining Tests that can be executed
        '''          with the volume currently placed in the Analyzer Rotor for the specified Reagent</returns>
        ''' <remarks>
        ''' Created by:  VR 21/11/2009
        ''' Modified by: TR 11/01/2010 - Set the value of remaining test to the GlobalDataTO to return
        '''              SA 17/12/2012 - Read MinValue of LimitID REAGENT_BOTTLE_VOLUME_LIMIT, which is the % of bottle volume that cannot be reach (residual volume)
        '''              SA 28/02/2012 - If the Real Bottle Volume is less than the residual volume, number of Remaining Tests is set to zero
        '''              SA 02/03/2012 - Added parameter for the type of bottle to allow search the section defined for it and calls functions GetVolumeByTubeType 
        '''                              to get the Bottle section and CalculateDeathVolByBottleType to calculate the death volume for the Bottle according its size
        ''' </remarks>
        Public Function CalculateRemainingTests(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSession As String, ByVal pElementID As Integer, _
                                                ByVal pRealBottleVolume As Single, ByVal pBottleType As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Get the Reagent volume needed for one preparation
                        resultData = GetProgrammedReagentVol(dbConnection, pWorkSession, pElementID)
                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            Dim myRequiredElementsDS As WSRequiredElementsDS = DirectCast(resultData.SetDatos, WSRequiredElementsDS)

                            Dim numRemainingTests As Integer = 0
                            If (myRequiredElementsDS.twksWSRequiredElements.Rows.Count > 0) Then
                                'Get the Section defined for the Bottle according its size 
                                Dim reagentBottles As New ReagentTubeTypesDelegate
                                resultData = reagentBottles.GetVolumeByTubeType(dbConnection, pBottleType)

                                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                    Dim reagentBottlesDS As ReagentTubeTypesDS = DirectCast(resultData.SetDatos, ReagentTubeTypesDS)

                                    If (reagentBottlesDS.ReagentTubeTypes.Rows.Count = 1) Then
                                        'Calculate the death volume for the bottle according its size
                                        resultData = reagentBottles.CalculateDeathVolByBottleType(dbConnection, pBottleType, reagentBottlesDS.ReagentTubeTypes.First.Section)
                                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                            Dim deathBottleVol As Single = CType(resultData.SetDatos, Single)

                                            'Get the max programmed Reagent Volume between all the requested Test/SampleType using the Reagent
                                            Dim maxVolume As Integer = 0
                                            For Each myReqElement As WSRequiredElementsDS.twksWSRequiredElementsRow In myRequiredElementsDS.twksWSRequiredElements.Rows
                                                If (myReqElement.ReagentVolume > maxVolume) Then maxVolume = CInt(myReqElement.ReagentVolume)
                                            Next

                                            'Finally, calculate the number of Tests that can be executed with the remaining volume (excluding the death volume)
                                            If ((pRealBottleVolume - deathBottleVol) < 0) Then
                                                numRemainingTests = 0
                                            Else
                                                Dim preparationVolume As Single = CType((maxVolume) / 1000, Single)
                                                numRemainingTests = CType(Math.Truncate((pRealBottleVolume - deathBottleVol) / preparationVolume), Integer)
                                            End If
                                        End If
                                    End If
                                End If
                            End If

                            'Set the result value to the GlobalDataTO 
                            resultData.SetDatos = numRemainingTests
                            resultData.HasError = False
                        End If
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSRequiredElementsDelegate.CalculateRemainingTests", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' When LIS sends demographics for a Patient previously added as SampleID to the active WS, the field PatientID is updated in the Order,
        ''' but if the SampleID already exists as a required Element, the PatientID has to be updated also in table twksWSRequiredElements  
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pPatientID">Patient Identifier</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by: SA 03/05/2013 
        ''' </remarks>
        Public Function ChangeSampleIDToPatientID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pPatientID As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myRequiredElementsDAO As New twksWSRequiredElementsDAO
                        resultData.SetDatos = myRequiredElementsDAO.ChangeSampleIDToPatientID(dbConnection, pPatientID)

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
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSRequiredElementsDelegate.ChangeSampleIDToPatientID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get the number of Elements belonging to the specified Work Session that have been still non positioned in an Analyzer Rotor
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pRotorType">Analyzer Rotor Type. Optional parameter; when not informed, then all non positioned
        '''                          Elements (excepting Washing Solution Tubes) will be count, without filtering them by TubeContent</param>
        ''' <param name="pStatusDiffOfPOS">Optional parameter to indicate that all Elements with Status different of POS have 
        '''                                to be count</param>
        ''' <param name="pExcludePatients">TRUE only when called from automatic WS creation with LIS process</param>
        ''' <returns>GlobalDataTO containing an integer value with the number of non positioned Work Session Elements</returns>
        ''' <remarks>
        ''' Created by:  TR 29/01/2010
        ''' Modified by: SA 27/07/2010 - Parameter Rotor Type is now optional. Added parameters to indicate the filter by ElementStatus
        '''                              should be different of POS.
        '''              AG 16/07/2013 - Added new optional parameter pExcludePatients. When this parameter is TRUE (only when this function is called
        '''                              during process of Automatic WS Creation with LIS), not positioned Patient Samples Tubes are not included in 
        '''                              the total number of not positioned required Elements
        '''              SA 29/01/2014 - BT #1474 ==> When the function was called with optional parameter pExcludePatients = TRUE, if all required Additional 
        '''                                           Solutions, Calibrators, Controls and Reagents are positioned (function CountNotPositionedElements returns
        '''                                           zero), then a new function GetNotPositionedPredilutionSamples is called to check if there are tubes of
        '''                                           prediluted Patient Samples needed for Tests requested by LIS. For each one of these tubes, check if there
        '''                                           is a tube in Samples Rotor having as Barcode the LIS SpecimenID and only in that case count the tube as
        '''                                           not positioned 
        ''' </remarks>
        Public Function CountNotPositionedElements(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, Optional ByVal pRotorType As String = "", _
                                                   Optional ByVal pStatusDiffOfPOS As Boolean = False, Optional ByVal pExcludePatients As Boolean = False) _
                                                   As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myRequiredElementsDAO As New twksWSRequiredElementsDAO
                        myGlobalDataTO = myRequiredElementsDAO.CountNotPositionedElements(dbConnection, pWorkSessionID, pRotorType, pStatusDiffOfPOS, pExcludePatients)

                        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                            If (pExcludePatients AndAlso DirectCast(myGlobalDataTO.SetDatos, Integer) = 0) Then
                                myGlobalDataTO = myRequiredElementsDAO.GetNotPositionedPredilutionSamples(dbConnection, pWorkSessionID)

                                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                    Dim myWSReqElementsDS As WSRequiredElementsDS = DirectCast(myGlobalDataTO.SetDatos, WSRequiredElementsDS)

                                    Dim mySpecimenIDList() As String
                                    Dim noPosPredilutions As Integer = 0
                                    Dim myWSRotorPosDelegate As New WSRotorContentByPositionDelegate

                                    For Each reqElem As WSRequiredElementsDS.twksWSRequiredElementsRow In myWSReqElementsDS.twksWSRequiredElements
                                        mySpecimenIDList = Split(reqElem.SpecimenIDList, ",")
                                        For i As Integer = 0 To mySpecimenIDList.Length - 1
                                            'Check if a tube with Barcode = mySpecimenIDList(i) is positioned in Samples Rotor
                                            myGlobalDataTO = myWSRotorPosDelegate.ExistBarcodeInfo(dbConnection, pWorkSessionID, mySpecimenIDList(i))
                                            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                                'If a tube with the specified Barcode is positioned in Samples Rotor, increment the counter by one
                                                If (DirectCast(myGlobalDataTO.SetDatos, Boolean)) Then noPosPredilutions += 1
                                            Else
                                                Exit For
                                            End If
                                        Next
                                    Next

                                    myGlobalDataTO.SetDatos = noPosPredilutions
                                    myGlobalDataTO.HasError = False
                                End If
                            End If
                        End If
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                myGlobalDataTO.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSRequiredElementsDelegate.CountNotPositionedElements", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Create Node structures for the TreeView of required Elements of a WorkSession
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWSRequiredElements">Typed DataSet WSRequiredElements containing the list of
        '''                                   required Elements that have to be shown in the TreeView</param>
        ''' <returns>GlobalDataTO containing the List(Of WSRequiredElementsTO), needed to drawn the TreeView</returns>
        ''' <remarks>
        ''' Created by:  AG 19/11/2009 - Tested: OK - 26/11/2009
        ''' Modified by: SA 09/03/2010 - Changed the way the Database Connection is opened: call function GetOpenDBConnection in DAOBase
        '''              RH 10/06/2011 - Add Tube Additional Solutions. Code optimization.
        '''              TR 11/10/2011 - Correct the Exit Try, instead validate if there was an error.
        ''' </remarks>
        Public Function CreateNodeList(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWSRequiredElements As WSRequiredElementsTreeDS) As GlobalDataTO
            Dim resultData As GlobalDataTO
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim reqElementsTree As New List(Of WSRequiredElementsTO)

                        'Are there Reagents in the WorkSession? - List reqElementsTree is updated by adding all required Reagents
                        If (pWSRequiredElements.Reagents.Rows.Count > 0) Then resultData = Me.InsertReagents(dbConnection, reqElementsTree, pWSRequiredElements.Reagents)

                        If (Not resultData.HasError) Then
                            'Are there Additional Solutions in the WorkSession? - List reqElementsTree is updated by adding all required Additional Solutions
                            If (pWSRequiredElements.AdditionalSolutions.Rows.Count > 0) Then resultData = Me.InsertAddditionalsSolutions(dbConnection, reqElementsTree, pWSRequiredElements.AdditionalSolutions)
                        End If

                        If (Not resultData.HasError) Then
                            'Are there Tube Additional Solutions in the WorkSession? - List reqElementsTree is updated by adding all required Tube Additional Solutions
                            If (pWSRequiredElements.TubeAdditionalSolutions.Rows.Count > 0) Then resultData = Me.InsertTubeAdditionalSolutions(dbConnection, reqElementsTree, pWSRequiredElements.TubeAdditionalSolutions)
                        End If

                        If (Not resultData.HasError) Then
                            'Are there Calibrators in the WorkSession? - List reqElementsTree is updated by adding all required Calibrators
                            If (pWSRequiredElements.Calibrators.Rows.Count > 0) Then resultData = Me.InsertCalibrators(dbConnection, reqElementsTree, pWSRequiredElements.Calibrators)
                        End If

                        If (Not resultData.HasError) Then
                            'Are there Controls in the WorkSession? - List reqElementsTree is updated by adding all required Controls
                            If (pWSRequiredElements.Controls.Rows.Count > 0) Then resultData = Me.InsertControls(dbConnection, reqElementsTree, pWSRequiredElements.Controls)
                        End If

                        If (Not resultData.HasError) Then
                            'Are there Patient Samples in the WorkSession? - List reqElementsTree is updated by adding all required Patient Samples
                            If (pWSRequiredElements.PatientSamples.Rows.Count > 0) Then resultData = Me.InsertPatients(dbConnection, reqElementsTree, pWSRequiredElements.PatientSamples)
                        End If

                        If (Not resultData.HasError) Then
                            'Return all the TreView Nodes...
                            resultData.SetDatos = reqElementsTree
                        End If
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSRequiredElementsDelegate.CreateNodeList", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Verify if an Element (of whatever Content Type) already exists as a Required Element in a Work Session
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWSRequiredElementsDS">DataSet containing all the information needed to verify if there is a Required Element for 
        '''                                     the informed element in the WorkSession according its TubeContent</param>
        ''' <param name="pIgnorePredilutionFactor">When informed, filters that depend on value of field PredilutionFactor are not applied
        '''                                        Used only for Patient Samples (TubeContent=PATIENT)</param>
        ''' <returns>GlobalDataTO with SetDatos containing a typed DataSet WSRequiredElementsDS  with all the information of the found 
        '''          Required Element</returns>
        ''' <remarks>
        ''' Created by:  VR 09/12/2009 - Tested: OK
        ''' Modified by: SA 04/01/2010 - Changed the way of open the DB Connection to the new template 
        '''                            - Error fixed: when calling GetReagentElementID, the fourth parameter should be field 
        '''                              ReagentNumber in the DataSet, not MultiItemNumber
        '''                            - Case Else section removed due to it is not possible have a value different of the specified
        '''                              ones in the TubeContent field
        '''              SA 09/03/2010 - Changes due to new field SampleID for Required Elements for Patient Samples
        '''              SA 26/10/2010 - When calling the DAO function GetPatientElements, new optional parameter was informed with 
        '''                              value of field OnlyForISE (to allow having two manual dilutions with the same factor for the 
        '''                              same Patient Sample when one of them is used exclusively for ISE Tests)
        '''              RH 16/06/2011 - Added verification of Elements when TubeContent is TUBE_SPEC_SOL and TUBE_WASH_SOL
        '''              SA 01/09/2011 - Added optional parameter pIgnorePredilutionFactor
        '''              SA 13/01/2012 - For Calibrators, do not inform the MultiItemNumber when calling function GetCalibratorElementID
        '''                              to allow return all points of the specified Calibrator when it already exists as required Element
        ''' </remarks>
        Public Function ExistRequiredElement(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWSRequiredElementsDS As WSRequiredElementsDS, _
                                             Optional ByVal pIgnorePredilutionFactor As Boolean = False) As GlobalDataTO
            Dim resultData As GlobalDataTO
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myRequiredElementsDAO As New twksWSRequiredElementsDAO
                        Dim reqElementRow As WSRequiredElementsDS.twksWSRequiredElementsRow

                        If (pWSRequiredElementsDS.twksWSRequiredElements.Rows.Count > 0) Then
                            reqElementRow = pWSRequiredElementsDS.twksWSRequiredElements(0)

                            Select Case (reqElementRow.TubeContent)
                                Case "REAGENT"
                                    'Search if there is a Required Element defined for the informed Reagent in the specified Work Session
                                    resultData = myRequiredElementsDAO.GetReagentElementID(dbConnection, reqElementRow.WorkSessionID, reqElementRow.ReagentID, reqElementRow.ReagentNumber)

                                Case "SPEC_SOL", "WASH_SOL", "TUBE_SPEC_SOL", "TUBE_WASH_SOL"
                                    'Search if there is a Required Element defined for the informed Special or Washing Solution in the specified Work Session
                                    resultData = myRequiredElementsDAO.GetAddittionalSolutionElementID(dbConnection, reqElementRow.WorkSessionID, reqElementRow.TubeContent, reqElementRow.SolutionCode)

                                Case "CTRL"
                                    'Search if there is a Required Element defined for the informed Control in the specified Work Session
                                    resultData = myRequiredElementsDAO.GetControlElementID(dbConnection, reqElementRow.WorkSessionID, reqElementRow.ControlID)

                                Case "CALIB"
                                    'Search if there is a Required Element defined for the informed Calibrator in the specified Work Session
                                    resultData = myRequiredElementsDAO.GetCalibratorElementID(dbConnection, reqElementRow.WorkSessionID, reqElementRow.CalibratorID)

                                Case "PATIENT"
                                    'Create a row of DataSet OrderTestsDS informing PatientID, OrderID, SampleType and PredilutionFactor with values of the entry DataSet
                                    Dim myOrderTestDetailDS As New OrderTestsDetailsDS
                                    Dim myOrderTestDetailDR As OrderTestsDetailsDS.OrderTestsDetailsRow
                                    myOrderTestDetailDR = myOrderTestDetailDS.OrderTestsDetails.NewOrderTestsDetailsRow()

                                    If (Not reqElementRow.IsPatientIDNull) Then
                                        myOrderTestDetailDR.PatientID = reqElementRow.PatientID
                                        myOrderTestDetailDR.SetSampleIDNull()
                                        myOrderTestDetailDR.SetOrderIDNull()
                                    ElseIf (Not reqElementRow.IsSampleIDNull) Then
                                        myOrderTestDetailDR.SampleID = reqElementRow.SampleID
                                        myOrderTestDetailDR.SetPatientIDNull()
                                        myOrderTestDetailDR.SetOrderIDNull()
                                    ElseIf (Not reqElementRow.IsOrderIDNull) Then
                                        myOrderTestDetailDR.OrderID = reqElementRow.OrderID
                                        myOrderTestDetailDR.SetSampleIDNull()
                                        myOrderTestDetailDR.SetPatientIDNull()
                                    End If

                                    If reqElementRow.IsPredilutionFactorNull Then
                                        myOrderTestDetailDR.SetPredilutionFactorNull()
                                    Else
                                        myOrderTestDetailDR.PredilutionFactor = reqElementRow.PredilutionFactor
                                    End If

                                    myOrderTestDetailDR.SampleType = reqElementRow.SampleType

                                    'Search if there is a Required Element defined for the informed Patient Sample in the specified Work Session
                                    resultData = myRequiredElementsDAO.GetPatientElements(dbConnection, reqElementRow.WorkSessionID, myOrderTestDetailDR, _
                                                                                          Convert.ToInt32(reqElementRow.OnlyForISE), pIgnorePredilutionFactor)
                                Case Else
                                    resultData.SetDatos = New WSRequiredElementsDS 'Returns an empty DataSet
                            End Select
                        End If
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSRequiredElementsDelegate.ExistRequiredElement", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' On loading a Virtual Rotor we need related all busy ring-cell with one (or none) required elements in current WorkSession
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pRotorType">Rotor Type</param>
        ''' <param name="pRotorLoadedDS">Typed DataSet VirtualRotorPosititionsDS containing all positions in the Virtual Rotor to load</param>
        ''' <param name="pOnlyRotorPosition">Optional parameter. If this parameter is true, return only rows where the ElementID is not Null; 
        '''                                  when it is False return all rows</param> 
        ''' <returns>GlobalDataTO containing a typed DataSet WSRotorContentsByPositionDS with the list of rotor positions with the information 
        '''          updated according if the content corresponds or not to a required Work Session Element</returns>
        ''' <remarks>
        ''' Created by:  AG 01/12/2009
        ''' Modified by: VR 09/12/2009 - To fix the error after changed the return data to GlobalDataTo in the function GetPatientElements
        '''                              Tested OK: 15/12/2009 AG
        '''              VR 09/12/2009 - TESTING : PENDING)
        '''              VR 21/12/2009 - TESTING : OK)
        '''              BK 23/12/2009 - Remove Ctype() Conversions
        '''              VR 06/10/2010 - Tested: OK .... Be carefull with date marks (07/01/2010 AG)
        '''              AG 08/01/2010 - Corrections (Tested Pending)
        '''              AG 14/01/2010 - Corrections on load virtual different from the virtual rotor worksession creation (Tested OK)
        '''              AG 21/01/2010 - Not use local variable VirtualRotorIDLoaded (Delete it)
        '''              DL 26/01/2010 - Add new parameter pOnlyRotorPosition (True: dataset with elementid not null, False: dataset with all elements)
        '''              SA 10/03/2010 - Changes to open the DB Connection according the new template; changes due to new field SampleID for 
        '''                              Patient Order Tests
        '''              RH 16/06/2011 - Add TUBE_SPEC_SOL and TUBE_WASH_SOL
        '''              TR 09/09/2011 - Implement the new functionality of barcode information.
        '''              RH 15/09/2011 - Field ScannedPosition
        '''              AG 03/02/2012 - When a Virtual Rotor is loaded, remember the Position Status when the tube/bottle placed is Depleted or Few
        '''              SA 08/02/2012 - When the Status of the tube/bottle placed in a Position is Depleted, then the Status of the correspondent
        '''                              Element is set to NOPOS (for Samples). For Reagents is set to POS due to the real Status has to be calculated later 
        '''                              based in the total volume needed and in the quantity of positioned volume 
        '''              XB 07/10/2014 - BA-1978 ==> Added log traces to catch NULL wrong assignment on RealVolume field 
        '''              SA 09/01/2015 - BA-1999 ==> If the TubeContent is not informed for the Position but it has a Barcode with Status UNKNOWN or ERROR,  
        '''                                          the Position Status is set to FREE
        '''              SA 12/01/2015 - BA-1999 ==> For IN USE and NOT IN USE Reagents, calculate the Number of Tests that can be executed with the available Bottle Volume
        ''' </remarks>
        Public Function FindElementIDRelatedWithRotorPosition(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, _
                                                              ByVal pRotorType As String, ByVal pRotorLoadedDS As VirtualRotorPosititionsDS, _
                                                              Optional ByVal pOnlyRotorPosition As Boolean = False) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim returnDS As New WSRotorContentByPositionDS
                        Dim commandDAO As New twksWSRequiredElementsDAO
                        Dim rcpDelegate As New WSRotorContentByPositionDelegate

                        Dim elementID As Integer = -1
                        Dim contentIsBottle As Boolean = False

                        Dim myWSRequiredElemDS As WSRequiredElementsDS
                        Dim rowDS As VirtualRotorPosititionsDS.tparVirtualRotorPosititionsRow
                        Dim newReturnRow As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow

                        Dim patientDataDS As PatientsDS
                        Dim myPatientDelegate As New PatientDelegate

                        Dim patient As New OrderTestsDetailsDS
                        Dim patientRow As OrderTestsDetailsDS.OrderTestsDetailsRow

                        For Each rowDS In pRotorLoadedDS.tparVirtualRotorPosititions
                            elementID = -1
                            contentIsBottle = False

                            'Find ElementID depending on TubeContent field
                            newReturnRow = returnDS.twksWSRotorContentByPosition.NewtwksWSRotorContentByPositionRow()

                            'BA-1999: Extracted code executed when TubeContent is informed. Process for positions with TubeContent not informed is 
                            '         executed later
                            If (Not rowDS.IsTubeContentNull) Then
                                If (rowDS.TubeContent = "REAGENT") Then
                                    contentIsBottle = True

                                    If (Not rowDS.IsReagentIDNull) Then
                                        resultData = commandDAO.GetReagentElementID(dbConnection, pWorkSessionID, rowDS.ReagentID, rowDS.MultiItemNumber)
                                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                            myWSRequiredElemDS = DirectCast(resultData.SetDatos, WSRequiredElementsDS)
                                            If (myWSRequiredElemDS.twksWSRequiredElements.Rows.Count > 0) Then elementID = myWSRequiredElemDS.twksWSRequiredElements(0).ElementID
                                        Else
                                            Exit For
                                        End If
                                    End If

                                ElseIf (rowDS.TubeContent = "SPEC_SOL") Then
                                    contentIsBottle = True

                                    resultData = commandDAO.GetAddittionalSolutionElementID(dbConnection, pWorkSessionID, "SPEC_SOL", rowDS.SolutionCode)
                                    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                        myWSRequiredElemDS = DirectCast(resultData.SetDatos, WSRequiredElementsDS)
                                        If (myWSRequiredElemDS.twksWSRequiredElements.Rows.Count > 0) Then elementID = myWSRequiredElemDS.twksWSRequiredElements(0).ElementID
                                    Else
                                        Exit For
                                    End If

                                ElseIf (rowDS.TubeContent = "WASH_SOL") Then
                                    contentIsBottle = True

                                    resultData = commandDAO.GetAddittionalSolutionElementID(dbConnection, pWorkSessionID, "WASH_SOL", rowDS.SolutionCode)
                                    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                        myWSRequiredElemDS = DirectCast(resultData.SetDatos, WSRequiredElementsDS)
                                        If (myWSRequiredElemDS.twksWSRequiredElements.Rows.Count > 0) Then elementID = myWSRequiredElemDS.twksWSRequiredElements(0).ElementID
                                    Else
                                        Exit For
                                    End If

                                ElseIf (rowDS.TubeContent = "TUBE_SPEC_SOL") Then
                                    resultData = commandDAO.GetAddittionalSolutionElementID(dbConnection, pWorkSessionID, "TUBE_SPEC_SOL", rowDS.SolutionCode)
                                    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                        myWSRequiredElemDS = DirectCast(resultData.SetDatos, WSRequiredElementsDS)
                                        If (myWSRequiredElemDS.twksWSRequiredElements.Rows.Count > 0) Then elementID = myWSRequiredElemDS.twksWSRequiredElements(0).ElementID
                                    Else
                                        Exit For
                                    End If

                                ElseIf (rowDS.TubeContent = "TUBE_WASH_SOL") Then
                                    resultData = commandDAO.GetAddittionalSolutionElementID(dbConnection, pWorkSessionID, "TUBE_WASH_SOL", rowDS.SolutionCode)
                                    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                        myWSRequiredElemDS = DirectCast(resultData.SetDatos, WSRequiredElementsDS)
                                        If (myWSRequiredElemDS.twksWSRequiredElements.Rows.Count > 0) Then elementID = myWSRequiredElemDS.twksWSRequiredElements(0).ElementID
                                    Else
                                        Exit For
                                    End If

                                ElseIf (rowDS.TubeContent = "CALIB") Then
                                    resultData = commandDAO.GetCalibratorElementID(dbConnection, pWorkSessionID, rowDS.CalibratorID, rowDS.MultiItemNumber)
                                    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                        myWSRequiredElemDS = DirectCast(resultData.SetDatos, WSRequiredElementsDS)
                                        If (myWSRequiredElemDS.twksWSRequiredElements.Rows.Count > 0) Then elementID = myWSRequiredElemDS.twksWSRequiredElements(0).ElementID
                                    Else
                                        Exit For
                                    End If

                                ElseIf (rowDS.TubeContent = "CTRL") Then
                                    resultData = commandDAO.GetControlElementID(dbConnection, pWorkSessionID, rowDS.ControlID)
                                    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                        myWSRequiredElemDS = DirectCast(resultData.SetDatos, WSRequiredElementsDS)
                                        If (myWSRequiredElemDS.twksWSRequiredElements.Rows.Count > 0) Then elementID = myWSRequiredElemDS.twksWSRequiredElements(0).ElementID
                                    Else
                                        Exit For
                                    End If

                                ElseIf (rowDS.TubeContent = "PATIENT") Then
                                    patientRow = patient.OrderTestsDetails.NewOrderTestsDetailsRow
                                    patientRow.SampleType = rowDS.SampleType

                                    If (Not rowDS.IsOrderIDNull) Then
                                        patientRow.OrderID = rowDS.OrderID
                                    ElseIf (Not rowDS.IsPatientIDNull) Then
                                        'Verify if the PatientID in the Virtual Rotor position corresponds to an existing Patient 
                                        resultData = myPatientDelegate.GetPatientData(dbConnection, rowDS.PatientID)
                                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                            patientDataDS = DirectCast(resultData.SetDatos, PatientsDS)
                                            If (patientDataDS.tparPatients.Rows.Count = 1) Then
                                                'It is an exiting PatientID, field PatientID is informed
                                                patientRow.PatientID = rowDS.PatientID
                                            Else
                                                'It is not an existing PatientID, field SampleID is informed
                                                patientRow.SampleID = rowDS.PatientID
                                            End If
                                        Else
                                            'Error verifying if the Patient exists in Patients table in DB 
                                            Exit For
                                        End If
                                    End If

                                    If (Not rowDS.IsPredilutionFactorNull) Then patientRow.PredilutionFactor = rowDS.PredilutionFactor
                                  
                                    resultData = commandDAO.GetPatientElements(dbConnection, pWorkSessionID, patientRow)
                                    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                        myWSRequiredElemDS = DirectCast(resultData.SetDatos, WSRequiredElementsDS)
                                        If (myWSRequiredElemDS.twksWSRequiredElements.Rows.Count > 0) Then
                                            If (Not myWSRequiredElemDS.twksWSRequiredElements(0).IsElementIDNull) Then elementID = myWSRequiredElemDS.twksWSRequiredElements(0).ElementID
                                        End If
                                    Else
                                        Exit For
                                    End If
                                End If
                            End If

                            'Transform VirtualRotorPositionDS into WSRotorContentPositionDS
                            newReturnRow.AnalyzerID = pAnalyzerID
                            newReturnRow.WorkSessionID = pWorkSessionID
                            newReturnRow.RingNumber = rowDS.RingNumber
                            newReturnRow.CellNumber = rowDS.CellNumber
                            newReturnRow.RotorType = pRotorType

                            If (Not rowDS.IsTubeContentNull) Then newReturnRow.TubeContent = rowDS.TubeContent Else newReturnRow.SetTubeContentNull()
                            If (Not rowDS.IsMultiTubeNumberNull) Then newReturnRow.MultiTubeNumber = rowDS.MultiTubeNumber Else newReturnRow.SetMultiTubeNumberNull()
                            If (Not rowDS.IsTubeTypeNull) Then newReturnRow.TubeType = rowDS.TubeType Else newReturnRow.SetTubeTypeNull()
                            If (Not rowDS.IsRealVolumeNull) Then newReturnRow.RealVolume = rowDS.RealVolume Else newReturnRow.SetRealVolumeNull()

                            If (Not rowDS.IsBarcodeStatusNull) Then newReturnRow.BarcodeStatus = rowDS.BarcodeStatus Else newReturnRow.SetBarcodeStatusNull()
                            If (Not rowDS.IsBarcodeInfoNull) Then newReturnRow.BarCodeInfo = rowDS.BarcodeInfo Else newReturnRow.SetBarCodeInfoNull()
                            If (Not rowDS.IsScannedPositionNull) Then newReturnRow.ScannedPosition = rowDS.ScannedPosition Else newReturnRow.SetScannedPositionNull()

                            'Finally if ElementID found: ElementStatus = POS, Status = PENDING (Samples) or INUSE (Reagents or Additional Solutions)
                            If (elementID <> -1) Then
                                newReturnRow.ElementID = elementID

                                If (contentIsBottle) Then
                                    'Reagents and Additional Solutions
                                    newReturnRow.Status = "INUSE"
                                    newReturnRow.ElementStatus = "POS"

                                    If (Not rowDS.IsRealVolumeNull) Then
                                        'Assign the Bottle Volume
                                        newReturnRow.RealVolume = rowDS.RealVolume

                                        'BA-1999: For IN USE Reagents, calculate the number of tests that can be executed with the available Bottle Volume
                                        If (rowDS.TubeContent = "REAGENT") Then
                                            resultData = CalculateRemainingTests(dbConnection, pWorkSessionID, elementID, rowDS.RealVolume, rowDS.TubeType)
                                            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then newReturnRow.RemainingTestsNumber = CType(resultData.SetDatos, Integer)
                                        End If
                                    End If
                                Else
                                    'Patient Samples, Controls, Calibrators and Additional Solutions in Tube
                                    newReturnRow.Status = "PENDING"
                                    newReturnRow.ElementStatus = "POS"
                                End If

                                'When a Virtual Rotor is loaded, remember the previous status of the placed tube/bottle (only if DEPLETED or FEW)
                                If (Not rowDS.IsStatusNull AndAlso (rowDS.Status = "DEPLETED" OrElse rowDS.Status = "FEW")) Then
                                    'This code is executed when the WorkSession exists and the user loads a Virtual Rotor. Parameter pRotorLoadedDS contains 
                                    'the information contained in VirtualRotorPosition (which contains Status field)
                                    newReturnRow.Status = rowDS.Status
                                    If (Not contentIsBottle AndAlso rowDS.Status = "DEPLETED") Then newReturnRow.ElementStatus = "NOPOS"
                                End If

                            ElseIf (Not rowDS.IsTubeContentNull) Then
                                'If ElementID not found but TubeContent is not null:  Status = NO_INUSE
                                newReturnRow.Status = "NO_INUSE"

                                'BA-1999: For NOT IN USE Reagents, calculate the number of tests that can be executed with the available Bottle Volume
                                If (rowDS.TubeContent = "REAGENT") Then
                                    If (Not rowDS.IsReagentIDNull AndAlso Not rowDS.IsMultiItemNumberNull AndAlso Not rowDS.IsRealVolumeNull AndAlso Not rowDS.IsTubeTypeNull) Then
                                        resultData = rcpDelegate.CalculateRemainingTestNotInUseReagent(dbConnection, pWorkSessionID, rowDS.ReagentID, rowDS.MultiItemNumber, _
                                                                                                       rowDS.RealVolume, rowDS.TubeType)

                                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then newReturnRow.RemainingTestsNumber = CType(resultData.SetDatos, Integer)
                                    End If
                                End If
                            Else
                                newReturnRow.Status = "FREE"
                            End If

                            'Add new row
                            returnDS.twksWSRotorContentByPosition.AddtwksWSRotorContentByPositionRow(newReturnRow)
                        Next

                        If (Not resultData.HasError) Then
                            'If this parameter is true, return only rows where the ElementID is not Null; when it is False return all rows
                            If (pOnlyRotorPosition) Then
                                Dim requiredElementsDS As New WSRotorContentByPositionDS

                                For Each ElementsDR As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow In returnDS.twksWSRotorContentByPosition.Rows()
                                    If (Not ElementsDR.IsElementIDNull) Then requiredElementsDS.twksWSRotorContentByPosition.ImportRow(ElementsDR)
                                Next
                                resultData.SetDatos = requiredElementsDS
                            Else
                                resultData.SetDatos = returnDS
                            End If
                        End If
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSRequiredElementsDelegate.FindElementIDRelatesWithRotorPosition", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Generate the next ElementID for an specific Work Session
        ''' </summary>
        ''' <param name="pDBConnection">Open Database Connection</param>
        ''' <returns>GlobalDataTO containing the generated ElementID</returns>
        ''' <remarks>
        ''' Created by:
        ''' Modified by: SA 09/03/2010 - Changed the way of open the DB Connection to fulfill the new template
        '''                              Change the returned type to a GlobalDataTO 
        ''' </remarks>
        Public Function GenerateElementID(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myRequiredPosDAO As New twksWSRequiredElementsDAO()
                        resultData = myRequiredPosDAO.GenerateElementID(dbConnection)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSRequiredElementsDelegate.GenerateElementID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Verify if there is a required Element in the WorkSession for the specified PatientID/SampleID and optionally, an specific Sample Type
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pPatientID">Patient or Sample Identifier</param>
        ''' <param name="pSampleType">Sample Type code. Optional parameter</param>
        ''' <returns>GlobalDataTO containing a typed DataSet WSRequiredElementsDS </returns>
        ''' <remarks>
        ''' Created by:  SA 10/04/2013
        ''' Modified by: SA 17/04/2013 - Deleted parameter pPatientExists
        ''' </remarks>
        Public Function GetByPatientAndSampleType(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, _
                                                  ByVal pPatientID As String, Optional ByVal pSampleType As String = "") As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myWSRequiredElements As New twksWSRequiredElementsDAO
                        myGlobalDataTO = myWSRequiredElements.GetByPatientAndSampleType(dbConnection, pWorkSessionID, pPatientID, pSampleType)
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                myGlobalDataTO.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSRequiredElementsDelegate.GetByPatientAndSampleType", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get the Control Identifier of a required Element by Execution
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pExecutionID">Execution Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet WSRequiredElementsDS containing the Control Identifier</returns>
        ''' <remarks>
        ''' Created by:  DL 23/02/2010
        ''' </remarks>
        Public Function GetControlID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pExecutionID As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim mywsRequiredElements As New twksWSRequiredElementsDAO
                        resultData = mywsRequiredElements.GetControlID(pDBConnection, pExecutionID)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSRequiredElementsDelegate.GetControlID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get the Element Identifiers for the Experimental Calibrator used 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pAlternativeOrderTestID">Order Test Identifier of the requested Calibrator</param>
        ''' <returns>GlobalDataTO containing a typed DataSet WSRequiredElementsDS with the list of ElementIDs for the Experimental
        '''          Calibrator for the TestID/SampleType of the specified Order Test</returns>
        ''' <remarks>
        ''' Created by:  SA 17/02/2011
        ''' </remarks>
        Public Function GetElemIDForAlternativeCalibrator(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, _
                                                          ByVal pAlternativeOrderTestID As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myRequiredElementsDAO As New twksWSRequiredElementsDAO
                        resultData = myRequiredElementsDAO.GetElemIDForAlternativeCalibrator(dbConnection, pWorkSessionID, pAlternativeOrderTestID)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSRequiredElementsDelegate.GetElemIDForAlternativeCalibrator", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get required elements for a patient where specimen identifier has value
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet WSRequiredElementsDS</returns>
        ''' <remarks>
        ''' Created by:  DL 14/06/2013
        ''' </remarks>
        Public Function GetLISPatientElements(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myRequiredElementsDAO As New twksWSRequiredElementsDAO
                        resultData = myRequiredElementsDAO.GetLISPatientElements(dbConnection, pWorkSessionID)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSRequiredElementsDelegate.GetLISPatientElements", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' For a required Element corresponding to a Calibrator, search if there are other related required Elements 
        ''' (case of MultiPoint Calibrators)
        ''' </summary>
        ''' <param name="pDBConnection">Database Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pElementID">Identifier of the required Calibrator Element</param>
        ''' <param name="pCalibratorID">Identifier of the Calibrator</param>
        ''' <returns>GlobalDataTO containing a typed DataSet WSRequiredElementsDS with the Element Identifier of all points 
        '''          of the informed Calibrator</returns>
        ''' <remarks>
        ''' Created by:  VR 21/11/2009
        ''' Modified by: VR 08/12/2009
        '''              SA 11/03/2010 - Changed the way of open the DB Connection to fulfill the new template 
        ''' </remarks>
        Public Function GetMultiPointCalibratorElements(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, _
                                                        ByVal pElementID As Integer, ByVal pCalibratorID As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myWSRequiredEleDAO As New twksWSRequiredElementsDAO
                        resultData = myWSRequiredEleDAO.GetMultipointCalibratorElements(dbConnection, pWorkSessionID, pElementID, pCalibratorID)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSRequiredElementsDelegate.GetMultiPointCalibratorElements", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get the list of all required Elements of the specified Work Session that have not been still positioned in the correspondent
        ''' Analyzer Rotor
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet WSNoPosRequiredElementsDS with the list of required Elements of the specified
        '''          Work Session that have not been still positioned in the correspondent Analyzer Rotor</returns>
        ''' <remarks>
        ''' Created by:  SA 26/07/2010
        ''' Modified by: SA 25/10/2010 - Inform ISE suffix for not positioned predilutions used only for ISE Tests
        '''              SA 14/02/2011 - Set False as value of new parameter of function ExistStatPatientSampleInWS to indicate that only
        '''                              positioned STAT Patient Samples have to be counted  
        '''              RH 16/06/2011 - Add TUBE_SPEC_SOL and TUBE_WASH_SOL
        '''              SA 28/11/2011 - For Patient Samples, link a dash plus the SampleType code at the end of field PatientID/SampleID
        '''              SA 09/01/2012 - For Patient Samples, inform the parameter to get only not finished Patient Samples
        '''              SA 07/02/2012 - Changed called to function GetRequiredPatientSamplesElements due to parameter pExcludedDepleted was removed
        '''                              Changed the function template
        '''              SA 29/08/2013 - When filling the WSNoPosRequiredElementsDS  to return with data of the not positioned Patient Samples, if the 
        '''                              required element is not a manual predilution and field SpecimenIDList is informed, set value of field SampleName 
        '''                              as SpecimenIDList plus the PatientID/SampleID between brackets; otherwise, set field SampleName = PatientID/SampleID
        '''              TR 13/11/2013 - BT #1380 ==> When call function GetRequiredPatientSamplesElements, parameter pOnlyNotFinished is set to TRUE, to get 
        '''                                           only Not Positioned and NOT FINISHED Patient Samples.
        '''              TR 21/11/2013 - BT #1380 ==> After calling function GetNotPositionedElements, unload Not Positioned Reagents that are not needed  
        '''                                           anymore in the active WorkSession
        '''              SA 27/05/2014 - BT #1519 ==> Undo the last change. Now not positioned Reagents that are not needed anymore in the active WorkSession 
        '''                                           are not returned by function GetNotPositionedElements
        ''' </remarks>
        Public Function GetNotPositionedElements(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing
            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myWSRequiredElementsDAO As New twksWSRequiredElementsDAO

                        'Get not positioned Elements that are not Patient Samples...
                        resultData = myWSRequiredElementsDAO.GetNotPositionedElements(dbConnection, pWorkSessionID)
                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            Dim myWSNoPosElementsDS As WSNoPosRequiredElementsDS = DirectCast(resultData.SetDatos, WSNoPosRequiredElementsDS)

                            ''TR 21/11/2013 -BT #1380
                            'Dim myOrderTestsDS As New OrderTestsDS
                            'Dim NoPosElementsToRemove As New List(Of WSNoPosRequiredElementsDS.twksWSRequiredElementsRow)
                            'For Each noposElementRow As WSNoPosRequiredElementsDS.twksWSRequiredElementsRow In _
                            '                                        myWSNoPosElementsDS.twksWSRequiredElements.Rows
                            '    If noposElementRow.SampleClass = "REAGENT" AndAlso _
                            '                    Not noposElementRow.IsElementIDNull Then

                            '        'TR 21/11/2013 -Validate reagent rotor elemens if not requiered any more.
                            '        resultData = ValidateReagentRotorElementIsRequired(dbConnection, noposElementRow.ElementID)
                            '        If Not resultData.HasError Then
                            '            myOrderTestsDS = DirectCast(resultData.SetDatos, OrderTestsDS)
                            '            If myOrderTestsDS.twksOrderTests.Count = 0 Then
                            '                'MarkElement To be remove for dataset
                            '                NoPosElementsToRemove.Add(noposElementRow)
                            '            End If
                            '        Else
                            '            Exit Try
                            '        End If
                            '    End If
                            'Next

                            'For Each ElemetToRemove In NoPosElementsToRemove
                            '    ElemetToRemove.Delete()
                            'Next
                            'myWSNoPosElementsDS.AcceptChanges()
                            ''TR 21/11/2013 -BT #1380 END    

                            If (myWSNoPosElementsDS.twksWSRequiredElements.Rows.Count > 0) Then
                                Dim preloadedDataConfig As New PreloadedMasterDataDelegate

                                'Verify if there are Reagents pending to positioning to get the correspondent Icon
                                Dim lstNoPosElements As List(Of WSNoPosRequiredElementsDS.twksWSRequiredElementsRow)
                                lstNoPosElements = (From a In myWSNoPosElementsDS.twksWSRequiredElements _
                                                   Where a.SampleClass = "REAGENT" _
                                                  Select a).ToList
                                If (lstNoPosElements.Count > 0) Then
                                    'Get the Icon defined for Reagents as a Byte array and assign it to each Reagent
                                    Dim imageBytes As Byte() = preloadedDataConfig.GetIconImage("REAGENTS")
                                    For Each noPosElement As WSNoPosRequiredElementsDS.twksWSRequiredElementsRow In lstNoPosElements
                                        noPosElement.SampleClassIcon = imageBytes
                                    Next
                                End If

                                'Verify if there are Special and/or Washing Solutions pending to positioning to get the correspondent Icon
                                lstNoPosElements = (From a In myWSNoPosElementsDS.twksWSRequiredElements _
                                                   Where a.SampleClass = "SPEC_SOL" OrElse a.SampleClass = "WASH_SOL" _
                                                  Select a).ToList
                                If (lstNoPosElements.Count > 0) Then
                                    'Get the Icon defined for Additional Solutions as a Byte array and assign it to each Additional Solution
                                    Dim imageBytes As Byte() = preloadedDataConfig.GetIconImage("ADD_SOL")
                                    For Each noPosElement As WSNoPosRequiredElementsDS.twksWSRequiredElementsRow In lstNoPosElements
                                        noPosElement.SampleClassIcon = imageBytes
                                    Next
                                End If

                                'RH 16/06/2011
                                'Verify if there are SAMPLE Special and/or Washing Solutions pending to positioning to get the correspondent Icon
                                lstNoPosElements = (From a In myWSNoPosElementsDS.twksWSRequiredElements _
                                                   Where a.SampleClass = "TUBE_SPEC_SOL" OrElse a.SampleClass = "TUBE_WASH_SOL" _
                                                  Select a).ToList
                                If (lstNoPosElements.Count > 0) Then
                                    'Get the Icon defined for Additional Solutions as a Byte array and assign it to each Additional Solution
                                    Dim imageBytes As Byte() = preloadedDataConfig.GetIconImage("ADD_SAMPLE_SOL")
                                    For Each noPosElement As WSNoPosRequiredElementsDS.twksWSRequiredElementsRow In lstNoPosElements
                                        noPosElement.SampleClassIcon = imageBytes
                                    Next
                                End If

                                'Verify if there are Calibrators pending to positioning to get the correspondent Icon
                                lstNoPosElements = (From a In myWSNoPosElementsDS.twksWSRequiredElements _
                                                   Where a.SampleClass = "CALIB" _
                                                  Select a).ToList
                                If (lstNoPosElements.Count > 0) Then
                                    'Get the Icon defined for Calibrators as a Byte array and assign it to each Calibrator
                                    Dim imageBytes As Byte() = preloadedDataConfig.GetIconImage("CALIB")
                                    For Each noPosElement As WSNoPosRequiredElementsDS.twksWSRequiredElementsRow In lstNoPosElements
                                        noPosElement.SampleClassIcon = imageBytes
                                    Next
                                End If

                                'Verify if there are Controls pending to positioning to get the correspondent Icon
                                lstNoPosElements = (From a In myWSNoPosElementsDS.twksWSRequiredElements _
                                                   Where a.SampleClass = "CTRL" _
                                                  Select a).ToList
                                If (lstNoPosElements.Count > 0) Then
                                    'Get the Icon defined for Controls as a Byte array and assign it to each Control
                                    Dim imageBytes As Byte() = preloadedDataConfig.GetIconImage("CTRL")
                                    For Each noPosElement As WSNoPosRequiredElementsDS.twksWSRequiredElementsRow In lstNoPosElements
                                        noPosElement.SampleClassIcon = imageBytes
                                    Next
                                End If
                            End If

                            'TR 13/11/2013 -BT #1380
                            'Get not positioned Patient Samples
                            resultData = myWSRequiredElementsDAO.GetRequiredPatientSamplesElements(dbConnection, pWorkSessionID, 0, "NOPOS", True, True)
                            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                Dim myRequiredPatientsDS As WSRequiredElementsTreeDS = DirectCast(resultData.SetDatos, WSRequiredElementsTreeDS)

                                If (myRequiredPatientsDS.PatientSamples.Rows.Count > 0) Then
                                    'Get the Icons defined for Stat, Routine and Dilution as a Byte array
                                    Dim preloadedDataConfig As New PreloadedMasterDataDelegate
                                    Dim statImageBytes As Byte() = preloadedDataConfig.GetIconImage("STATS")
                                    Dim routineImageBytes As Byte() = preloadedDataConfig.GetIconImage("ROUTINES")
                                    Dim dilutionImageBytes As Byte() = preloadedDataConfig.GetIconImage("DILUTIONS")

                                    'For each different Patient Sample, verify if it is Stat, Routine or Dilution to assign the proper Icon;
                                    'add each Patient Sample to the DataSet to return
                                    Dim statFlag As Boolean
                                    Dim position As Integer
                                    For Each noPosPatientSample As WSRequiredElementsTreeDS.PatientSamplesRow In myRequiredPatientsDS.PatientSamples.Rows
                                        Dim myOrderTestDelegate As New WSOrderTestsDelegate
                                        resultData = myOrderTestDelegate.ExistStatPatientSampleInWS(dbConnection, pWorkSessionID, noPosPatientSample.PatientID, _
                                                                                                    noPosPatientSample.SampleID, noPosPatientSample.OrderID, _
                                                                                                    noPosPatientSample.SampleType, False)

                                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                            statFlag = DirectCast(resultData.SetDatos, Boolean)
                                            position = Convert.ToInt32(IIf(statFlag, 6, 7))

                                            Dim noPosReqElemRow As WSNoPosRequiredElementsDS.twksWSRequiredElementsRow
                                            noPosReqElemRow = myWSNoPosElementsDS.twksWSRequiredElements.NewtwksWSRequiredElementsRow

                                            noPosReqElemRow.SampleClass = "PATIENT"
                                            noPosReqElemRow.SampleType = noPosPatientSample.SampleType

                                            'If (Not noPosPatientSample.IsPatientIDNull) Then
                                            '    noPosReqElemRow.SampleName = noPosPatientSample.PatientID
                                            'ElseIf (Not noPosPatientSample.IsSampleIDNull) Then
                                            '    noPosReqElemRow.SampleName = noPosPatientSample.SampleID
                                            'Else
                                            '    noPosReqElemRow.SampleName = noPosPatientSample.OrderID
                                            'End If

                                            'SA 29/08/2013 - See comment about this code in the function header
                                            Dim patientID As String = String.Empty
                                            If (Not noPosPatientSample.IsPatientIDNull) Then
                                                patientID = noPosPatientSample.PatientID
                                            ElseIf (Not noPosPatientSample.IsSampleIDNull) Then
                                                patientID = noPosPatientSample.SampleID
                                            Else
                                                patientID = noPosPatientSample.OrderID
                                            End If

                                            If (noPosPatientSample.IsPredilutionFactorNull) Then
                                                If (Not noPosPatientSample.IsSpecimenIDListNull AndAlso noPosPatientSample.SpecimenIDList <> String.Empty) Then
                                                    noPosReqElemRow.SampleName = noPosPatientSample.SpecimenIDList.Trim.Replace(vbCrLf, ",") & " (" & patientID & ")"
                                                Else
                                                    noPosReqElemRow.SampleName = patientID
                                                End If
                                            Else
                                                noPosReqElemRow.SampleName = patientID
                                            End If



                                            If (noPosPatientSample.IsPredilutionFactorNull) Then
                                                If (statFlag) Then
                                                    noPosReqElemRow.SampleClassIcon = statImageBytes
                                                Else
                                                    noPosReqElemRow.SampleClassIcon = routineImageBytes
                                                End If
                                            Else
                                                noPosReqElemRow.SampleClassIcon = dilutionImageBytes
                                                noPosReqElemRow.SampleName &= " 1/" & noPosPatientSample.PredilutionFactor.ToString

                                                If (noPosPatientSample.OnlyForISE) Then
                                                    noPosReqElemRow.SampleType &= " (ISE)"
                                                End If
                                            End If

                                            'Link the Sample Type Code at the end of the SampleName
                                            noPosReqElemRow.SampleName &= "-" & noPosReqElemRow.SampleType

                                            noPosReqElemRow.Position = position
                                            myWSNoPosElementsDS.twksWSRequiredElements.AddtwksWSRequiredElementsRow(noPosReqElemRow)
                                        Else
                                            'Error verifying if the Patient Sample is STAT or ROUTINE
                                            Exit For
                                        End If
                                    Next
                                End If
                            End If

                            If (Not resultData.HasError) Then
                                resultData.SetDatos = myWSNoPosElementsDS
                            End If
                        End If
                    End If
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSRequiredElementsDelegate.GetNotPositionedElements", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' For a Required Element of type Reagent, get the volume programmed for each Test/SampleType using it, and also the 
        ''' total volume of the Reagent currently required in the WorkSession
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pElementID">Identifier of the required Reagent Element</param>
        ''' <returns>GlobalDataTO containing a typed DataSet WSRequiredElementsDS with the Volume of the informed Reagent
        '''          required for every different TestID/SampleType in the Work Session</returns>
        ''' <remarks>
        ''' Created by:  SA 15/02/2012
        ''' </remarks>
        Public Function GetProgrammedReagentVol(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, ByVal pElementID As Integer) As GlobalDataTO
            Dim dataToReturn As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                dataToReturn = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not dataToReturn.HasError AndAlso Not dataToReturn.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(dataToReturn.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myRequiredElementsDAO As New twksWSRequiredElementsDAO
                        dataToReturn = myRequiredElementsDAO.GetProgrammedReagentVol(dbConnection, pWorkSessionID, pElementID)
                    End If
                End If
            Catch ex As Exception
                dataToReturn = New GlobalDataTO()
                dataToReturn.HasError = True
                dataToReturn.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                dataToReturn.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSRequiredElementsDelegate.GetProgrammedReagentVol", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return dataToReturn
        End Function

        ''' <summary>
        ''' Get details of all Calibrators needed for a Work Session.  Optionally, this function can get details of just one Calibrator, 
        ''' or get details of all Calibrators that have an specified Status (positioned or non positioned). Multipoint Calibrators will 
        ''' be returned as an unique row  in the DataSet indicating the total number of points
        ''' </summary>
        ''' <param name="pDBConnection">Database Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pElementID">Optional parameter. When informed, it is the Element Identifier to filter by</param>
        ''' <param name="pElementStatus">Optional parameter. When informed, it is the Element Status to filter by</param>
        ''' <returns>GlobalDataTO with a typed DataSet WSRequiredElementsTreeDS containing the detailed information of the Calibrators filtered 
        '''          according the informed entry parameters</returns>
        ''' <remarks>
        ''' Created by:  VR - 20/11/2009  
        ''' Modified by: BK 23/12/2009 - Removed CShort() conversion for variables
        '''              SA 24/12/2009 - Function fails when it is executed for an specific ElementID. Code flow changed.
        '''                              Non used variables removed. Added writing in Log when an exception happens.
        '''              SA 24/12/2009 - Changed the way the Database Connection is opened: call function GetOpenDBConnection in DAOBase
        '''              SA 09/01/2012 - When the optional parameter pElementID has not been informed, get only Calibrators not marked as finished
        ''' </remarks>
        Public Function GetRequiredCalibratorsDetails(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, Optional ByVal pElementID As Integer = 0, _
                                                      Optional ByVal pElementStatus As String = "") As GlobalDataTO
            Dim returnedData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                returnedData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not returnedData.HasError AndAlso Not returnedData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(returnedData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Get Calibrators Details
                        Dim myWSRequiredEleDAO As New twksWSRequiredElementsDAO
                        returnedData = myWSRequiredEleDAO.GetRequiredCalibratorsDetails(dbConnection, pWorkSessionID, pElementID, pElementStatus, (pElementID = 0))

                        If (Not returnedData.HasError AndAlso Not returnedData.SetDatos Is Nothing) Then
                            Dim myRequiredElementTree As WSRequiredElementsTreeDS = DirectCast(returnedData.SetDatos, WSRequiredElementsTreeDS)

                            Dim numPoints As Integer = 1
                            Dim initialElem As Integer = -1
                            Dim previousCalib As Integer = -1
                            Dim resultDS As New WSRequiredElementsTreeDS

                            For i As Integer = 0 To myRequiredElementTree.Calibrators.Rows.Count - 1
                                If (i = 0) Then
                                    'First Calibrator: data is saved in the variables used to control when a 
                                    'new Calibrator has to be processed and the ElementID of the initial point of
                                    'a multipoint Calibrator kit
                                    initialElem = myRequiredElementTree.Calibrators(i).ElementID
                                    previousCalib = myRequiredElementTree.Calibrators(i).CalibratorID
                                Else
                                    If (myRequiredElementTree.Calibrators(i).CalibratorID <> previousCalib) Then
                                        'Insert the previous processed Calibrator row
                                        If (numPoints >= 1) Then
                                            'Inform the number of points and save the initial Element of the Calibrator kit
                                            myRequiredElementTree.Calibrators(i - 1).BeginEdit()
                                            myRequiredElementTree.Calibrators(i - 1).NumOfCalibrators = numPoints
                                            myRequiredElementTree.Calibrators(i - 1).ElementID = initialElem
                                            myRequiredElementTree.Calibrators(i - 1).EndEdit()
                                        End If

                                        'Add the Calibrator data to the DataSet to return
                                        resultDS.Calibrators.ImportRow(myRequiredElementTree.Calibrators(i - 1))

                                        'Initialize control variables to process next Calibrator
                                        numPoints = 1
                                        initialElem = myRequiredElementTree.Calibrators(i).ElementID
                                        previousCalib = myRequiredElementTree.Calibrators(i).CalibratorID
                                    Else
                                        'Same Calibrator, increment the number of Calibrator points
                                        numPoints += 1
                                    End If
                                End If

                                'Insert the last processed Calibrator row
                                If (i = myRequiredElementTree.Calibrators.Rows.Count - 1) Then
                                    If (numPoints >= 1) Then
                                        'Inform the number of points and save the initial Element of the Calibrator kit
                                        myRequiredElementTree.Calibrators(i).BeginEdit()
                                        myRequiredElementTree.Calibrators(i).NumOfCalibrators = numPoints
                                        myRequiredElementTree.Calibrators(i).ElementID = initialElem
                                        myRequiredElementTree.Calibrators(i).EndEdit()
                                    End If

                                    'Add the Calibrator data to the DataSet to return
                                    resultDS.Calibrators.ImportRow(myRequiredElementTree.Calibrators(i))
                                End If
                            Next

                            returnedData.HasError = False
                            returnedData.SetDatos = resultDS
                        End If
                    End If
                End If
            Catch ex As Exception
                returnedData = New GlobalDataTO()
                returnedData.HasError = True
                returnedData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                returnedData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSRequiredElementsDelegate.GetRequiredCalibratorsDetails", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return returnedData
        End Function

        ''' <summary>
        ''' Get details of all the Controls included in the specified Work Session or, optionally, get details
        ''' of an specific Control or of all Controls that have the specified status
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection </param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pElementID">Element Identifier. Optional parameter; when informed, the function gets details of the specific Control</param>
        ''' <param name="pElementStatus">Element Status. Optional parameter; when informed, the function gets details of all Controls in the 
        '''                              Work Session that have this status</param>
        ''' <returns>GlobalDataTO containing a typed DataSet WSRequiredElementsTreeDS with details of the Controls included in the specified Work Session</returns>
        ''' <remarks>
        ''' Created by:  VR 20/11/2009 
        ''' Modified by: SA 11/03/2010 - Changed the way of opening the DB Connection to fulfill the new template
        '''              SA 09/01/2012 - When the optional parameter pElementID has not been informed, get only Controls not marked as finished
        ''' </remarks>      
        Public Function GetRequiredControlsDetails(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, Optional ByVal pElementID As Integer = 0, _
                                                   Optional ByVal pElementStatus As String = "") As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myWSRequiredEleDAO As New twksWSRequiredElementsDAO
                        resultData = myWSRequiredEleDAO.GetRequiredControlsDetails(dbConnection, pWorkSessionID, pElementID, pElementStatus, (pElementID = 0))
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSRequiredElementsDelegate.GetRequiredControlsDetails", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get detailed information of the specified required Element
        ''' </summary>
        ''' <param name="pDbConnection">Open DB Connection</param>
        ''' <param name="pElementID">Element Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet WSRequiredElementsDS with all information of the specified Required Element</returns>
        ''' <remarks>
        ''' Created by:  VR 21/11/2009 - Tested: OK
        ''' Modified by: SA 10/03/2010 - Changes to open the DB Connection according the new template
        ''' </remarks>
        Public Function GetRequiredElementData(ByVal pDbConnection As SqlClient.SqlConnection, ByVal pElementID As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDbConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myWSRequiredEleDAO As New twksWSRequiredElementsDAO
                        resultData = myWSRequiredEleDAO.Read(dbConnection, pElementID)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSRequiredElementsDelegate.GetRequiredElementData", EventLogEntryType.Error, False)
            Finally
                If (pDbConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get detailed information of a specific required Element according its type 
        ''' (Reagent, Special Solution, Washing Solution, Calibrator, Control or Patient Sample).
        ''' </summary>
        ''' <param name="pDBConnection">Database Connection</param>
        ''' <param name="pElementID">Element Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet WSRequiredElementsTreeDS containing the 
        '''          detailed information of the specified required Element</returns>
        ''' <remarks>
        ''' Created by:  AG 25/11/2009 - Tested: PENDING
        ''' Modified by: SA 23/12/2009 - Return error when Read fails
        '''              SA 09/03/2010 - Changes to open the DB Connection according the new template
        '''              RH 16/06/2011 - Add TUBE_SPEC_SOL and TUBE_WASH_SOL. Code optimization.
        ''' </remarks>
        Public Function GetRequiredElementInfo(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pElementID As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim requiredElementsDAO As New twksWSRequiredElementsDAO
                        resultData = requiredElementsDAO.Read(dbConnection, pElementID)

                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            Dim elementDS As WSRequiredElementsDS = DirectCast(resultData.SetDatos, WSRequiredElementsDS)

                            Dim elementRow As WSRequiredElementsDS.twksWSRequiredElementsRow
                            elementRow = elementDS.twksWSRequiredElements(0)

                            Select Case (elementRow.TubeContent)
                                Case "REAGENT"
                                    resultData = Me.GetRequiredReagentsDetails(dbConnection, elementRow.WorkSessionID, pElementID)

                                Case "SPEC_SOL", "WASH_SOL"
                                    resultData = Me.GetRequiredSolutionsDetails(dbConnection, elementRow.WorkSessionID, pElementID)

                                Case "TUBE_SPEC_SOL", "TUBE_WASH_SOL"
                                    resultData = Me.GetRequiredTubeSolutionsDetails(dbConnection, elementRow.WorkSessionID, pElementID)

                                Case "CALIB"
                                    resultData = Me.GetRequiredCalibratorsDetails(dbConnection, elementRow.WorkSessionID, pElementID)

                                Case "CTRL"
                                    resultData = Me.GetRequiredControlsDetails(dbConnection, elementRow.WorkSessionID, pElementID)

                                Case "PATIENT"
                                    resultData = Me.GetRequiredPatientSamplesDetails(dbConnection, elementRow.WorkSessionID, pElementID)

                                Case Else
                                    resultData.HasError = True
                                    resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                            End Select
                        End If
                    End If
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSRequiredElementsDelegate.GetRequiredElementInfo", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get details of all required Elements in the specified Work Session. Used to load the TreeView 
        ''' control in the screen of Rotor Positioning
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identification.</param>
        ''' <returns>GlobalDataTO containing a typed DataSet WSRequiredElementsTreeDS with details of all required
        '''          Elements for the specified Work Session</returns>
        ''' <remarks>
        ''' Created by: 
        ''' Modified by: BK 23/12/2009 - Removed CShort() conversion for variables
        '''              SA 15/01/2010 - Added the parameter and management of DB Connection.  Function was moved from "FOR TESTING" Region.
        '''                              Change to fill PatientSamples table in the following way: first Stat Samples and then Routine Samples
        '''              SA 09/03/2010 - Changes due to adding of field SampleID for Required Elements for Patients
        '''              SA 14/02/2011 - Set False as value of new parameter of function ExistStatPatientSampleInWS to indicate that only
        '''                              positioned STAT Patient Samples have to be counted  
        '''              RH 10/06/2011 - Get Tube Additional Solutions
        '''              AG 10/10/2011 - For Calibrators, if at least one of the Calibrator points is not positioned, then the tree node is not positioned
        '''              SA 28/02/2012 - Added parameters for AnalyzerID and Work Session Status. For Reagents, the required volume and the element status
        '''                              have to be recalculated and updated
        ''' </remarks> 
        Public Function GetRequiredElementsDetails(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, _
                                                   ByVal pWSStatus As String) As GlobalDataTO
            Dim myGlobalData As GlobalDataTO
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalData.HasError AndAlso Not myGlobalData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Get Reagents
                        Dim resultData As New WSRequiredElementsTreeDS
                        Dim resultDataTmp As New WSRequiredElementsTreeDS
                        Dim myWSRequiredElementDAO As New twksWSRequiredElementsDAO

                        myGlobalData = myWSRequiredElementDAO.GetRequiredReagentsDetails(dbConnection, pWorkSessionID)
                        If (Not myGlobalData.HasError AndAlso Not myGlobalData.SetDatos Is Nothing) Then
                            resultDataTmp = DirectCast(myGlobalData.SetDatos, WSRequiredElementsTreeDS)

                            Dim j As Integer = 0
                            Dim lastImportRow As Integer = -1
                            Dim testIDList As String = ""
                            Dim testNameList As String = ""
                            Dim previousReagent As Integer = -1

                            Do While (j < resultDataTmp.Reagents.Rows.Count)
                                If (previousReagent <> resultDataTmp.Reagents(j).ReagentID) Then
                                    If (testIDList.Trim <> "") Then
                                        resultData.Reagents(lastImportRow).TestID = testIDList
                                        resultData.Reagents(lastImportRow).TestName = testNameList
                                    End If

                                    resultData.Reagents.ImportRow(resultDataTmp.Reagents(j))
                                    lastImportRow += 1

                                    previousReagent = resultDataTmp.Reagents(j).ReagentID
                                    testIDList = resultDataTmp.Reagents(j).TestID
                                    testNameList = resultDataTmp.Reagents(j).TestName
                                Else
                                    testIDList += ", " & resultDataTmp.Reagents(j).TestID
                                    testNameList += ", " & resultDataTmp.Reagents(j).TestName

                                    If (j = resultDataTmp.Reagents.Rows.Count - 1) Then
                                        resultData.Reagents(lastImportRow).TestID = testIDList
                                        resultData.Reagents(lastImportRow).TestName = testNameList
                                    End If
                                End If
                                j += 1
                            Loop
                        End If

                        If (Not myGlobalData.HasError) Then
                            'Calculate the required volume and the status of every Reagent
                            Dim myWSReqElementsDS As New WSRequiredElementsDS
                            Dim myElementRow As WSRequiredElementsDS.twksWSRequiredElementsRow
                            For Each myReagent As WSRequiredElementsTreeDS.ReagentsRow In resultData.Reagents
                                'Fill needed data in a row of WSRequiredElementsDS 
                                myElementRow = myWSReqElementsDS.twksWSRequiredElements.NewtwksWSRequiredElementsRow
                                myElementRow.WorkSessionID = pWorkSessionID
                                myElementRow.ElementID = myReagent.ElementID
                                myElementRow.TubeContent = myReagent.TubeContent
                                myElementRow.ReagentID = myReagent.ReagentID
                                myElementRow.ReagentNumber = myReagent.ReagentNumber
                                myElementRow.RequiredVolume = 0 'Current required volume is recalculated by function CalculateNeededBottlesAndReagentStatus

                                myGlobalData = CalculateNeededBottlesAndReagentStatus(dbConnection, pAnalyzerID, myElementRow, 0, (pWSStatus <> "PENDING"))
                                If (Not myGlobalData.HasError AndAlso Not myGlobalData.SetDatos Is Nothing) Then
                                    myElementRow = DirectCast(myGlobalData.SetDatos, WSRequiredElementsDS.twksWSRequiredElementsRow)
                                    myWSReqElementsDS.twksWSRequiredElements.AddtwksWSRequiredElementsRow(myElementRow)

                                    'Update values for the required Reagent
                                    myGlobalData = myWSRequiredElementDAO.Update(dbConnection, myWSReqElementsDS)
                                    If (myGlobalData.HasError) Then Exit For

                                    'Update fields in the DS to return
                                    myReagent.ElementStatus = myElementRow.ElementStatus
                                    myReagent.RequiredVolume = myElementRow.RequiredVolume

                                    myWSReqElementsDS.Clear()
                                Else
                                    Exit For
                                End If
                            Next
                        End If

                        If (Not myGlobalData.HasError) Then
                            'Get Additional Solutions
                            myGlobalData = myWSRequiredElementDAO.GetRequiredSolutionsDetails(dbConnection, pWorkSessionID)
                            If (Not myGlobalData.HasError AndAlso Not myGlobalData.SetDatos Is Nothing) Then
                                resultDataTmp = DirectCast(myGlobalData.SetDatos, WSRequiredElementsTreeDS)

                                For Each solutionElementsDR As WSRequiredElementsTreeDS.AdditionalSolutionsRow In resultDataTmp.AdditionalSolutions.Rows
                                    resultData.AdditionalSolutions.ImportRow(solutionElementsDR)
                                Next
                            End If
                        End If

                        If (Not myGlobalData.HasError) Then
                            'Get Tube Additional Solutions
                            myGlobalData = myWSRequiredElementDAO.GetRequiredTubeSolutionsDetails(dbConnection, pWorkSessionID)
                            If (Not myGlobalData.HasError AndAlso Not myGlobalData.SetDatos Is Nothing) Then
                                resultDataTmp = DirectCast(myGlobalData.SetDatos, WSRequiredElementsTreeDS)

                                For Each solutionElementsDR As WSRequiredElementsTreeDS.TubeAdditionalSolutionsRow In resultDataTmp.TubeAdditionalSolutions.Rows
                                    resultData.TubeAdditionalSolutions.ImportRow(solutionElementsDR)
                                Next
                            End If
                        End If

                        If (Not myGlobalData.HasError) Then
                            'Get Calibrators
                            myGlobalData = myWSRequiredElementDAO.GetRequiredCalibratorsDetails(dbConnection, pWorkSessionID)
                            If (Not myGlobalData.HasError AndAlso Not myGlobalData.SetDatos Is Nothing) Then
                                resultDataTmp = DirectCast(myGlobalData.SetDatos, WSRequiredElementsTreeDS)

                                Dim i As Integer = 0
                                Dim numPoints As Integer = 1
                                Dim initialPos As Integer = -1
                                Dim previousCalib As Integer = -1
                                Dim nodeStatus As String = ""

                                Do While (i < resultDataTmp.Calibrators.Rows.Count)
                                    If (i = 0) Then
                                        initialPos = resultDataTmp.Calibrators(i).ElementID
                                        previousCalib = resultDataTmp.Calibrators(i).CalibratorID
                                        nodeStatus = resultDataTmp.Calibrators(i).ElementStatus 'AG 10/10/2011
                                    Else
                                        'Insert the previous processed Calibrator Row
                                        If (previousCalib <> resultDataTmp.Calibrators(i).CalibratorID) Then
                                            If (numPoints > 1) Then
                                                'Inform the Number of Points and save the initial Element of the Calibrator kit
                                                resultDataTmp.Calibrators(i - 1).NumOfCalibrators = numPoints
                                                resultDataTmp.Calibrators(i - 1).ElementID = initialPos
                                            End If

                                            'Add the Calibrator data to final DataSet
                                            resultData.Calibrators.ImportRow(resultDataTmp.Calibrators(i - 1))

                                            'Initialize control variables to process next Calibrator
                                            numPoints = 1
                                            initialPos = resultDataTmp.Calibrators(i).ElementID
                                            previousCalib = resultDataTmp.Calibrators(i).CalibratorID
                                            nodeStatus = resultDataTmp.Calibrators(i).ElementStatus 'AG 10/10/2011
                                        Else
                                            'Increment the number of Calibrator points and verify the status of the Calibrator point
                                            numPoints += 1
                                            If (resultDataTmp.Calibrators(i).ElementStatus = "NOPOS") Then nodeStatus = "NOPOS"
                                        End If
                                    End If
                                    i += 1

                                    'Insert the last procesed Calibrator Row
                                    If (i = resultDataTmp.Calibrators.Rows.Count) Then
                                        If (numPoints > 1) Then
                                            'Inform the Number of Points and save the initial Element of the Calibrator kit
                                            resultDataTmp.Calibrators(i - 1).NumOfCalibrators = numPoints
                                            resultDataTmp.Calibrators(i - 1).ElementID = initialPos
                                            resultDataTmp.Calibrators(i - 1).ElementStatus = nodeStatus 'AG 10/10/2011
                                        End If

                                        'Add the Calibrator data to final DataSet
                                        resultData.Calibrators.ImportRow(resultDataTmp.Calibrators(i - 1))
                                    End If
                                Loop
                            End If
                        End If

                        If (Not myGlobalData.HasError) Then
                            'Get Controls
                            myGlobalData = myWSRequiredElementDAO.GetRequiredControlsDetails(dbConnection, pWorkSessionID)
                            If (Not myGlobalData.HasError AndAlso Not myGlobalData.SetDatos Is Nothing) Then
                                resultDataTmp = CType(myGlobalData.SetDatos, WSRequiredElementsTreeDS)

                                For Each ControlElementsDR As WSRequiredElementsTreeDS.ControlsRow In resultDataTmp.Controls.Rows
                                    resultData.Controls.ImportRow(ControlElementsDR)
                                Next
                            End If
                        End If

                        'Get Patient Samples
                        If (Not myGlobalData.HasError) Then
                            myGlobalData = myWSRequiredElementDAO.GetRequiredPatientSamplesElements(dbConnection, pWorkSessionID)

                            If (Not myGlobalData.HasError AndAlso Not myGlobalData.SetDatos Is Nothing) Then
                                resultDataTmp = CType(myGlobalData.SetDatos, WSRequiredElementsTreeDS)

                                Dim wsOrderTestsData As New WSOrderTestsDelegate
                                Dim routineSamplesDS As New WSRequiredElementsTreeDS

                                For Each patientSampleElementsDR As WSRequiredElementsTreeDS.PatientSamplesRow In resultDataTmp.PatientSamples.Rows
                                    If (patientSampleElementsDR.IsPatientIDNull) Then patientSampleElementsDR.PatientID = ""
                                    If (patientSampleElementsDR.IsSampleIDNull) Then patientSampleElementsDR.SampleID = ""
                                    If (patientSampleElementsDR.IsOrderIDNull) Then patientSampleElementsDR.OrderID = ""

                                    'Mark Patient Samples required for Stat
                                    myGlobalData = wsOrderTestsData.ExistStatPatientSampleInWS(dbConnection, pWorkSessionID, patientSampleElementsDR.PatientID, _
                                                                                               patientSampleElementsDR.SampleID, patientSampleElementsDR.OrderID, _
                                                                                               patientSampleElementsDR.SampleType, False)

                                    If (Not myGlobalData.HasError AndAlso Not myGlobalData.SetDatos Is Nothing) Then
                                        patientSampleElementsDR.StatFlag = CType(myGlobalData.SetDatos, Boolean)

                                        If (patientSampleElementsDR.StatFlag) Then
                                            resultData.PatientSamples.ImportRow(patientSampleElementsDR)
                                        Else
                                            'Import the Routine Patient Sample in a temporary DataSet
                                            routineSamplesDS.PatientSamples.ImportRow(patientSampleElementsDR)
                                        End If
                                    Else
                                        Exit For
                                    End If
                                Next

                                'Add the Routine Patient Samples to the final DataSet
                                For Each patientSampleElementsDR As WSRequiredElementsTreeDS.PatientSamplesRow In routineSamplesDS.PatientSamples.Rows
                                    resultData.PatientSamples.ImportRow(patientSampleElementsDR)
                                Next
                            End If
                        End If

                        If (Not myGlobalData.HasError) Then
                            myGlobalData.SetDatos = resultData
                        End If
                    End If
                End If
            Catch ex As Exception
                myGlobalData = New GlobalDataTO()
                myGlobalData.HasError = True
                myGlobalData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                myGlobalData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSRequiredElementsDelegate.GetRequiredElementsDetails", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalData
        End Function

        ''' <summary>
        ''' Get details of all Patient Samples needed for a Work Session.  Optionally, this function can get details of just one Patient Sample, 
        ''' or get detaills of all Patient Samples that have an specified Status (positioned or non positioned). This function also verify if 
        ''' there is at least one Stat Order for each different PatientID/OrderID and SampleType; if there is at least one, then the Patient Sample 
        ''' will be marked as required for Stat in the WorkSession
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">WorkSession Identifier</param>
        ''' <param name="pElementID">Identifier of the required Patient Sample Element. Optional parameter</param>
        ''' <param name="pElementStatus">Code of  Element Status. Optional parameter</param>
        ''' <param name="pGetAllNoPos">Optional parameter. When value of this parameter is TRUE, Patient Samples marked as Not Positioned and having DEPLETED or FEW
        '''                            tubes positioned in the Rotor are also returned; otherwise, they are not returned (this is needed in positioning function)</param>
        ''' <returns>GlobalDataTO containing a typed DataSet WSRequiredElementsTreeDS with the detailed information of the Patient Samples filtered 
        '''          according the informed entry parameters</returns>
        ''' <remarks>
        ''' Created by:  VR 20/11/2009 - Tested: OK
        ''' Modified by: SA 14/01/2010 - Changed the way of opening the DB Connection to fulfill the new template
        '''                              Error fixed, it seems that LINQ sort Boolean fields as strings (and in this case the Routine 
        '''                              Patient Samples was returned first). Some changes in code to improve it.
        '''              SA 09/03/2010 - Changes due to new field SampleID in table of Required Elements
        '''              SA 25/10/2010 - When the function is called for an specific Element, and it is a dilution marked as Only For ISE,
        '''                              then the suffix ISE is linked to field SampleType 
        '''              TR 29/04/2011 - Optimization of function and add new variable named myPosStatus
        '''              TR 22/09/2011 - Added optional parameter pExcludedDepleted
        '''              SA 09/11/2011 - Returned records have to be sorted by StatFlag and ElementID
        '''              SA 10/01/2012 - When the optional parameter pElementID has not been informed, get only Patient Samples not marked as finished
        '''              SA 07/02/2012 - Removed parameter pExcludedDepleted; changed the function template
        '''              SA 09/02/2012 - Added optional parameter pGetAllNoPos to allow get also Patient Samples positioned in Rotor but marked as DEPLETED or FEW
        '''                              This parameter is used only when pElementStatus is informed as NOPOS
        ''' </remarks>
        Public Function GetRequiredPatientSamplesDetails(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, Optional ByVal pElementID As Integer = 0, _
                                                         Optional ByVal pElementStatus As String = "", Optional ByVal pGetAllNoPos As Boolean = False) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Get details of Patient Samples included in the WorkSession
                        Dim myWSRequiredEleDAO As New twksWSRequiredElementsDAO
                        resultData = myWSRequiredEleDAO.GetRequiredPatientSamplesElements(dbConnection, pWorkSessionID, pElementID, pElementStatus, (pElementID = 0), pGetAllNoPos)

                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            Dim myRequiredElementTree As WSRequiredElementsTreeDS = DirectCast(resultData.SetDatos, WSRequiredElementsTreeDS)

                            Dim statFlag As Boolean = False
                            Dim previousRecord As String = ""
                            Dim currentRecord As String = ""
                            Dim myOrderTestDelegate As New WSOrderTestsDelegate
                            Dim myResultDS As New WSRequiredElementsTreeDS
                            Dim myPosStatus As Boolean

                            For Each myReqPatientSample As WSRequiredElementsTreeDS.PatientSamplesRow In myRequiredElementTree.PatientSamples.Rows
                                currentRecord = myReqPatientSample.PatientID + myReqPatientSample.SampleID + myReqPatientSample.OrderID + myReqPatientSample.SampleType
                                If (currentRecord <> previousRecord) Then
                                    'Verify if for the PatientID/SampleType (or SampleID/SampleType or OrderID/SampleType) there is at least an 
                                    myPosStatus = DirectCast(IIf(pElementStatus = "POS", False, True), Boolean)

                                    'Order Test that corresponds to a Stat Order
                                    resultData = myOrderTestDelegate.ExistStatPatientSampleInWS(dbConnection, pWorkSessionID, myReqPatientSample.PatientID, _
                                                                                                myReqPatientSample.SampleID, myReqPatientSample.OrderID, _
                                                                                                myReqPatientSample.SampleType, myPosStatus)
                                    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                        statFlag = CType(resultData.SetDatos, Boolean)
                                    End If
                                    previousRecord = currentRecord
                                End If

                                'Update the StatFlag for the Patient Sample in process, and additionally, if field PredilutionFactor 
                                'is Null, set its value to 0
                                myReqPatientSample.BeginEdit()
                                myReqPatientSample.StatFlag = statFlag
                                If (myReqPatientSample.IsPredilutionFactorNull) Then
                                    myReqPatientSample.PredilutionFactor = 0
                                Else
                                    If (pElementID <> 0) Then
                                        'If the function was called for an specific Element, and it is a dilution marked as Only For ISE,
                                        'then the suffix ISE is linked to field SampleType
                                        If (myReqPatientSample.OnlyForISE) Then myReqPatientSample.SampleType &= " (ISE)"
                                    End If
                                End If
                                myReqPatientSample.EndEdit()
                            Next

                            'Get Stat Patient Samples and sort them by ElementID (elements were created sorted by PatientID, SampleID, OrderID, SampleType and PredilutionFactor)
                            'Add them to the DataSet to return
                            Dim myStatPatientSamples = From e In myRequiredElementTree.PatientSamples _
                                                     Select e Where e.StatFlag = True _
                                                   Order By e.ElementID
                            myStatPatientSamples.CopyToDataTable(myResultDS.PatientSamples, LoadOption.OverwriteChanges)

                            'Get Routine Patient Samples and sort them by ElementID (elements were created sorted by PatientID, SampleID, OrderID, SampleType and PredilutionFactor) 
                            'Add them to the DataSet to return
                            Dim myRoutinePatientSamples = From e In myRequiredElementTree.PatientSamples _
                                                        Select e Where e.StatFlag = False _
                                                      Order By e.ElementID
                            myRoutinePatientSamples.CopyToDataTable(myResultDS.PatientSamples, LoadOption.OverwriteChanges)

                            'Return the Patient Samples sorted by StatFlag (first the Urgent ones), PatientID, SampleID, OrderID, SampleType and PredilutionFactor
                            resultData.SetDatos = myResultDS
                            resultData.HasError = False
                        End If
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSRequiredElementsDelegate.GetRequiredPatientSamplesDetails", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get details of all the Reagents included in the specified Work Session or, optionally, get details
        ''' of an specific Reagent or of all Reagents that have the specified status
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection </param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pElementID">Element Identifier. Optional parameter; when informed, the function gets details
        '''                          of the specific Reagent</param>
        ''' <param name="pElementStatus">Element Status. Optional parameter; when informed, the function gets details
        '''                              of all Reagents in the Work Session that have this status</param>
        ''' <returns>GlobalDataTO containing a typed DataSet WSRequiredElementsTreeDS with details of the Reagents
        '''           included in the specified Work Session</returns>
        ''' <remarks>
        ''' Created by:  VR 20/11/2009 - Tested: OK
        ''' Modified by: TR 14/12/2009 - Pass parameters pElementID and pElementStatus to DAO function - Tested: OK
        '''              SA 11/03/2010 - Changed the way of opening the DB Connection to fulfill the new template
        ''' </remarks>
        Public Function GetRequiredReagentsDetails(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, _
                                                   Optional ByVal pElementID As Integer = 0, Optional ByVal pElementStatus As String = "") _
                                                   As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim result As New WSRequiredElementsTreeDS
                        Dim myWSRequiredEleDAO As New twksWSRequiredElementsDAO

                        resultData = myWSRequiredEleDAO.GetRequiredReagentsDetails(dbConnection, pWorkSessionID, pElementID, pElementStatus)
                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            Dim myRequiredElementTree As WSRequiredElementsTreeDS = DirectCast(resultData.SetDatos, WSRequiredElementsTreeDS)

                            Dim lastImportRow As Integer = -1
                            Dim currentRowCount As Integer = 0
                            Dim testIDList As String = ""
                            Dim testNameList As String = ""
                            Dim previousReagent As Integer = -1
                            Dim i As Integer = myRequiredElementTree.Reagents.Rows.Count - 1

                            For Each myReqReagent As WSRequiredElementsTreeDS.ReagentsRow In myRequiredElementTree.Reagents.Rows
                                If (myReqReagent.ReagentID <> previousReagent) Then
                                    If (testIDList <> "") Then
                                        result.Reagents(lastImportRow).TestID = myReqReagent.TestID
                                        result.Reagents(lastImportRow).TestName = myReqReagent.TestName

                                        result.Reagents.ImportRow(myReqReagent) 'Import row
                                        lastImportRow = lastImportRow + 1

                                        previousReagent = myReqReagent.ReagentID
                                        testIDList = myReqReagent.TestID
                                        testNameList = myReqReagent.TestName
                                    Else
                                        result.Reagents.ImportRow(myReqReagent) 'Import row
                                        lastImportRow = lastImportRow + 1

                                        previousReagent = myReqReagent.ReagentID
                                        testIDList = myReqReagent.TestID
                                        testNameList = myReqReagent.TestName
                                    End If
                                Else
                                    testIDList += ", " & myReqReagent.TestID
                                    testNameList += ", " & myReqReagent.TestName

                                    If (i = currentRowCount) Then
                                        result.Reagents(lastImportRow).TestID = testIDList
                                        result.Reagents(lastImportRow).TestName = testNameList

                                        previousReagent = myReqReagent.ReagentID
                                        testIDList = myReqReagent.TestID
                                        testNameList = myReqReagent.TestName

                                        lastImportRow = lastImportRow + 1
                                    End If
                                End If

                                currentRowCount = currentRowCount + 1
                            Next

                            'Return the DataSet 
                            resultData.SetDatos = result
                            resultData.HasError = False
                        End If
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSRequiredElementsDelegate.GetRequiredReagentsDetails", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get details of all the Additional Solutions included in the specified Work Session or, optionally, get details
        ''' of an specific Additional Solution or of all Additional Solutions that have the specified status
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection </param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pElementID">Element Identifier. Optional parameter; when informed, the function gets details
        '''                          of the specific Additional Solution</param>
        ''' <param name="pElementStatus">Element Status. Optional parameter; when informed, the function gets details
        '''                              of all Additional Solutions in the Work Session that have this status</param>
        ''' <returns>GlobalDataTO containing a typed DataSet WSRequiredElementsTreeDS with details of the Additional Solutions
        '''          included in the specified Work Session</returns>
        ''' <remarks>
        ''' Created by:  VR 20/11/2009 
        ''' Modified by: TR 14/12/2009 - Pass parameters pElementID and pElementStatus to DAO function
        '''              SA 11/03/2010 - Changed the way of opening the DB Connection to fulfill the new template
        ''' </remarks>        
        Public Function GetRequiredSolutionsDetails(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, _
                                                    Optional ByVal pElementID As Integer = 0, Optional ByVal pElementStatus As String = "") _
                                                    As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myWSRequiredEleDAO As New twksWSRequiredElementsDAO
                        resultData = myWSRequiredEleDAO.GetRequiredSolutionsDetails(dbConnection, pWorkSessionID, pElementID, pElementStatus)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSRequiredElementsDelegate.GetRequiredSolutionsDetails", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get details of all the Tube Additional Solutions included in the specified Work Session or, optionally, get details
        ''' of an specific Tube Additional Solution or of all Tube Additional Solutions that have the specified status
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection </param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pElementID">Element Identifier. Optional parameter; when informed, the function gets details
        '''                          of the specific Additional Solution</param>
        ''' <param name="pElementStatus">Element Status. Optional parameter; when informed, the function gets details
        '''                              of all Additional Solutions in the Work Session that have this status</param>
        ''' <returns>GlobalDataTO containing a typed DataSet WSRequiredElementsTreeDS with details of the Additional Solutions
        '''          included in the specified Work Session</returns>
        ''' <remarks>
        ''' Created by:  RH 16/06/2011 - Based on GetRequiredSolutionsDetails
        ''' Modified by: SA 11/01/2012 - When the optional parameter pElementID has not been informed, get only Sample Tube Solutions
        '''                              not marked as finished
        ''' </remarks>        
        Public Function GetRequiredTubeSolutionsDetails(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, _
                                                        Optional ByVal pElementID As Integer = 0, Optional ByVal pElementStatus As String = "") _
                                                        As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myWSRequiredEleDAO As New twksWSRequiredElementsDAO
                        resultData = myWSRequiredEleDAO.GetRequiredTubeSolutionsDetails(dbConnection, pWorkSessionID, pElementID, pElementStatus, (pElementID = 0))
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSRequiredElementsDelegate.GetRequiredTubeSolutionsDetails", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get dilutions for the specified Patient Sample Element
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifer</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pRotorType">Analyzer Rotor Type</param>
        ''' <param name="pElementID">Element Identifier</param>
        ''' <param name="pPositionedElements">Flag indicating what type of data should return the function: all non positioned Patient Sample Dilutions
        '''                                   (when False) or rotor positions in which the Patient Sample Dilutions are placed (in both cases, related
        '''                                   with the specified ElementID)</param>
        ''' <param name="pOnlyNotFinished">When True, it indicates that only not positioned dilutions not marked as finished will be returned.
        '''                                Applied only when pPositionedElements=False</param>
        ''' <returns>When pPositionedElements is False, GlobalDataTO containing a typed DataSet WSRequiredElementsDS with the Element Identifiers 
        '''          of all non positioned Patient Sample Dilutions; when pPositionedElements is True, then a GlobalDataTO containing a typed DataSet
        '''          WSRotorContentByPositionDS with the physical Rotor positions of all positioned Patient Sample Dilutions for the informed Element 
        ''' </returns>
        ''' <remarks>
        ''' Created by:  VR 21/11/2009 - Tested: OK
        ''' Modified by: SA 09/03/2010 - Changes to open the DB Connection according the new template
        '''              SA 10/01/2012 - Added new optional parameter to allow getting only not finished and not positioned dilutions (only when 
        '''                              parameter pPositionedElements is False)
        '''              SA 29/01/2014 - When there are several different dilutions for the specified Patient Sample Element and all of them are positioned in 
        '''                              Samples Rotor, an error was raised in the FOR/NEXT loop over WSRequiredElementsDS  when all rows have been removed from 
        '''                              it. Added code to Exit For when the described situation happens.
        ''' </remarks>
        Public Function GetSampleDilutionElements(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, ByVal pAnalyzerID As String, _
                                                  ByVal pRotorType As String, ByVal pElementID As Integer, ByVal pPositionedElements As Boolean, _
                                                  Optional ByVal pOnlyNotFinished As Boolean = False) As GlobalDataTO
            Dim resultData As GlobalDataTO
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Get all Elements that correspond to Dilutions of the specified Patient Sample
                        Dim myWSRequiredElementsDAO As New twksWSRequiredElementsDAO
                        resultData = myWSRequiredElementsDAO.GetSampleDilutionElements(dbConnection, pWorkSessionID, pElementID)

                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            Dim myRequiredElementsDS As WSRequiredElementsDS = DirectCast(resultData.SetDatos, WSRequiredElementsDS)

                            'If there are dilutions for the Patient Sample, build a string list containing their Element Identifiers
                            If (myRequiredElementsDS.twksWSRequiredElements.Rows.Count > 0) Then
                                Dim elementIDList As String = ""
                                For i As Integer = 0 To myRequiredElementsDS.twksWSRequiredElements.Rows.Count - 1
                                    elementIDList += myRequiredElementsDS.twksWSRequiredElements(i).ElementID.ToString + ","
                                Next
                                elementIDList = Left(elementIDList, elementIDList.Length - 1)

                                'Get the positions in which the dilutions are placed in the Analyzer Rotor
                                Dim myRotorContentByPosition As New WSRotorContentByPositionDelegate
                                resultData = myRotorContentByPosition.GetPositionedElements(dbConnection, pAnalyzerID, pRotorType, elementIDList)

                                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                    Dim myRotorContentByPositionDS As WSRotorContentByPositionDS = DirectCast(resultData.SetDatos, WSRotorContentByPositionDS)

                                    If (Not pPositionedElements) Then
                                        'When pPositionedElements is False, then the positioned dilutions are removed from the DataSet WSRequiredElementsDS
                                        'to return only the Element Identifiers of non positioned dilutions. Additionally, when pOnlyNotFinished is True,
                                        'not positioned dilutions marked as finished are also removed from the DataSet WSRequiredElementsDS
                                        Dim deletedElement As Boolean
                                        For i As Integer = 0 To myRequiredElementsDS.twksWSRequiredElements.Rows.Count - 1
                                            deletedElement = False
                                            For j As Integer = 0 To myRotorContentByPositionDS.twksWSRotorContentByPosition.Rows.Count - 1
                                                If (myRequiredElementsDS.twksWSRequiredElements(i).ElementID = myRotorContentByPositionDS.twksWSRotorContentByPosition(j).ElementID) Then
                                                    'If the required Element is positioned in the Rotor, then it is removed from the DataSet
                                                    myRequiredElementsDS.twksWSRequiredElements(i).Delete()
                                                    myRequiredElementsDS.twksWSRequiredElements.AcceptChanges()

                                                    deletedElement = True
                                                End If
                                            Next

                                            If (Not deletedElement AndAlso pOnlyNotFinished AndAlso myRequiredElementsDS.twksWSRequiredElements(i).ElementFinished) Then
                                                'If the required Element is not positioned in the Rotor but it is marked as finished, then it 
                                                'is also removed from the DataSet
                                                myRequiredElementsDS.twksWSRequiredElements(i).Delete()
                                                myRequiredElementsDS.twksWSRequiredElements.AcceptChanges()
                                            End If

                                            'If the DS of Required Elements is empty, then leave the FOR/NEXT loop
                                            If (myRequiredElementsDS.twksWSRequiredElements.Rows.Count = 0) Then Exit For
                                        Next

                                        'Return the WSRequiredElementsDS, even if it is empty
                                        resultData.SetDatos = myRequiredElementsDS
                                    Else
                                        'When pPositionedElements is True, the DataSet WSRotorContentByPositionDS is returned...
                                    End If
                                End If
                            End If
                        End If
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSRequiredElementsDelegate.GetSampleDilutionElements", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get all Patient Samples for manual predilutions having field SpecimenIDList informed
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTO containing a typed DataSet WSRequiredElementsDS with all data obtained</returns>
        ''' <remarks>
        ''' Created by:  SA 04/04/2014 - BT #1524
        ''' </remarks>
        Public Function ReadAllPatientDilutions(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksWSRequiredElementsDAO
                        resultData = myDAO.ReadAllPatientDilutions(dbConnection)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSRequiredElementsDelegate.ReadAllPatientDilutions", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Read all elements required in the informed WS
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <returns>GlobalDataTO containing a typed Dataset WSRequiredElementsDS with data of all elements required in the informed WS</returns>
        ''' <remarks></remarks>
        Public Function ReadAll(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksWSRequiredElementsDAO
                        resultData = myDAO.ReadAll(dbConnection, pWorkSessionID)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSRequiredElementsDelegate.ReadAll", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get the ElementID related to the Reagents or Additional Solutions read from Barcode
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pBarCodeDS">Typed DataSet BarCodesDS</param>
        ''' <returns>GlobalDataTO containing a typed DataSet WSRequiredElementsDS with data of the required Element</returns>
        ''' <remarks>
        ''' Created by:  AG 29/08/2011
        ''' Modified by: TR 06/09/2011 - When searching by Solution Code, set Reagent parameter to zero 
        ''' </remarks>
        Public Function ReadReagentRotorElementByBarcodeID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pBarCodeDS As BarCodesDS) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        If (pBarCodeDS.DecodedReagentsFields.Rows.Count > 0) Then
                            Dim myDAO As New twksWSRequiredElementsDAO

                            Dim solutionCode As String = ""
                            If (Not pBarCodeDS.DecodedReagentsFields(0).IsSolutionCodeNull) Then
                                solutionCode = pBarCodeDS.DecodedReagentsFields(0).SolutionCode

                                'Search a Solution Code, set Reagent parameter as zero 
                                resultData = myDAO.ReadElemIdByCodeTest(dbConnection, pBarCodeDS.DecodedReagentsFields(0).CodeTest, 0, solutionCode)
                            Else
                                'Search a Reagent, set SolutionCode parameter to an empty string
                                resultData = myDAO.ReadElemIdByCodeTest(dbConnection, pBarCodeDS.DecodedReagentsFields(0).CodeTest, pBarCodeDS.DecodedReagentsFields(0).ReagentNumber, "")
                            End If
                        End If
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSRequiredElementsDelegate.ReadReagentRotorElementByBarcodeID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Delete all information for the Work Session
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by: GDS 21/04/2010
        ''' </remarks>
        Public Function ResetWS(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError) AndAlso (Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksWSRequiredElementsDAO
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
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSRequiredElementsDelegate.ResetWS", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Update data of the specified ElementID
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWSRequiredElementsDS">Typed DataSet WSRequiredElementsDS containing data of the Element to update</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  VR 17/12/2009 - Tested: OK
        ''' Modified by: SA 09/03/2010 - Changes open the DB Transaction according the new template
        ''' </remarks>
        Public Function Update(ByVal pDbConnection As SqlClient.SqlConnection, ByVal pWSRequiredElementsDS As WSRequiredElementsDS) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDbConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myWSRequiredEleDAO As New twksWSRequiredElementsDAO
                        resultData = myWSRequiredEleDAO.Update(dbConnection, pWSRequiredElementsDS)

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
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSRequiredElementsDelegate.Update", EventLogEntryType.Error, False)
            Finally
                If (pDbConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Update field ElementFinished for the specified Required Element
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pElementID">Required Element Identifier</param>
        ''' <param name="pElementFinished">When True, it indicates the Element is not needed anymore in the WS</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 12/01/2012
        ''' Modified by: SG 29/04/2013 - Updated SpecimenIDList for TreeView Tooltip
        ''' </remarks>
        Public Function UpdateElementFinished(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pElementID As Integer, _
                                              ByVal pElementFinished As Boolean, Optional ByVal pSpecimenIDList As String = "") As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myWSRequiredEleDAO As New twksWSRequiredElementsDAO
                        resultData = myWSRequiredEleDAO.UpdateElementFinished(dbConnection, pElementID, pElementFinished, pSpecimenIDList)

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
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSRequiredElementsDelegate.UpdateElementFinished", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Update fields PatientID and SampleID for the required Patient Sample Element of the informed Order, depending on value of parameter pSampleIDType:
        ''' ** If MAN, update SampleID = pSampleID and PatientID = NULL
        ''' ** If DB,  update PatientID = pSampleID and SampleID = NULL
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pOrderID">Order Identifier</param>
        ''' <param name="pSampleID">Sample or Patient Identifier to update</param>
        ''' <param name="pSampleIDType">Type of Sample ID: DB or MAN</param>
        ''' <param name="pSampleType">Sample Type Code</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 01/08/2013 
        ''' </remarks>
        Public Function UpdatePatientSampleFields(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOrderID As String, ByVal pSampleID As String, ByVal pSampleIDType As String, _
                                                  ByVal pSampleType As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myWSRequiredEleDAO As New twksWSRequiredElementsDAO
                        resultData = myWSRequiredEleDAO.UpdatePatientSampleFields(dbConnection, pOrderID, pSampleID, pSampleIDType, pSampleType)

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
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSRequiredElementsDelegate.UpdatePatientSampleFields", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Update the Status of the specified Required Element
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pElementID">Required Element Identifier</param>
        ''' <param name="pNewElementStatus">New Element Status</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  VR 21/11/2009 - Tested: OK
        ''' Modified by: SA 11/03/2010 - Changed the way of opening the DB Transaction to fulfill the new template
        ''' </remarks>
        Public Function UpdateStatus(ByVal pDbConnection As SqlClient.SqlConnection, ByVal pElementID As Integer, ByVal pNewElementStatus As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDbConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myWSRequiredEleDAO As New twksWSRequiredElementsDAO
                        resultData = myWSRequiredEleDAO.UpdateStatus(dbConnection, pElementID, pNewElementStatus)

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
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSRequiredElementsDelegate.UpdateStatus", EventLogEntryType.Error, False)
            Finally
                If (pDbConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Update the Status and Tube Type of the specified Required Element
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pElementID">Required Element Identifier</param>
        ''' <param name="pNewElementStatus">New Element Status</param>
        ''' <param name="pNewTubeType">New Tube Type for the specified Element</param>
        ''' <param name="pTubeContent">Tube Content</param>
        ''' <param name="pApplyToAllTubes">Optional parameter. When FALSE and TubeContent=CALIB, it means only the Element for the selected
        '''                                Calibrator point is updated (refill case), while, when TRUE and TubeContent=CALIB, it means the 
        '''                                Elements for all Calibrator points in the kit (tube type change case) are updated</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 19/03/2010
        ''' Modified by: SA 20/06/2011 - Added new parameter for the WorkSession Identifier
        '''              SA 13/02/2012 - Added optional parameter pApplyToAllTubes to indicate if the Status has to be updated for all points in
        '''                              the Calibrator kit (only whe TubeContent = CALIB)
        ''' </remarks>
        Public Function UpdateStatusAndTubeType(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, ByVal pElementID As Integer, _
                                                ByVal pNewElementStatus As String, ByVal pNewTubeType As String, ByVal pTubeContent As String, _
                                                 Optional ByVal pApplyToAllTubes As Boolean = False) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Update TubeType and Status of the specified ElementID
                        Dim myWSRequiredEleDAO As New twksWSRequiredElementsDAO
                        resultData = myWSRequiredEleDAO.UpdateStatusAndTubeType(dbConnection, pWorkSessionID, pElementID, pNewElementStatus, pNewTubeType, pTubeContent, _
                                                                                pApplyToAllTubes)

                        If (Not resultData.HasError) Then
                            'Update the Tube Type for the related Order Tests
                            Dim myOrderTestsDelegate As New OrderTestsDelegate
                            resultData = myOrderTestsDelegate.UpdateTubeTypeByRequiredElement(dbConnection, pWorkSessionID, pElementID, pNewTubeType)
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
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSRequiredElementsDelegate.UpdateStatusAndTubeType", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Update ElementStatus = NOPOS for all Required Elements placed in an Analyzer Rotor after Reset the Rotor
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pRotorType">Analyzer Rotor Type</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  AG 25/11/2009 - Tested: OK
        ''' Modified by: AG 30/11/2009 - Add parameter WorkSessionID - Tested: OK
        '''              SA 11/03/2010 - Changed the way of opening the DB Transaction to fulfill the new template
        ''' </remarks>
        Public Function UpdateStatusByResetRotor(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
                                                 ByVal pRotorType As String, ByVal pWorkSessionID As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myRequiredElementsDAO As New twksWSRequiredElementsDAO
                        resultData.SetDatos = myRequiredElementsDAO.UpdateStatusByResetRotor(dbConnection, pAnalyzerID, pRotorType, pWorkSessionID)

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
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSRequiredElementsDelegate.UpdateStatusByResetRotor", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function
#End Region

#Region "Private Methods"

        ''' <summary>
        ''' Add a new Master Node in the TreeView of Required Elements: Reagents, Additional Solutions, Samples, Calibrators, 
        ''' Controls, Stat Patient Samples or Routine Patient Samples
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pElementslist">List of Elements in the TreeView of Required Elements</param>
        ''' <param name="pMasterElementTitle">Name of the Master Node to add</param>
        ''' <param name="pRequireLinQ">Flag indicating if LINQ has to be used to verify if the Master Node exists before add it</param>
        ''' <param name="pTestName">Optional parameter. Only for Reagents when the name of the Master Node to create is TESTNAME</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  AG 18/11/2009 - Tested: OK - 26/11/2009
        ''' Modified by: VR 29/12/2009 - Changed the Constant value to Enum Value
        '''              SA 09/03/2010 - Changes to open the DB Connection according the new template; changes to receive a GlobalDataTO
        '''                              when calling function GetSubTableItem in PreloadedMasterDataDelegate
        '''              RH 09/06/2011 - Add Tube Additional Solutions logic
        '''              XB 04/02/2013 - Upper conversions redundants because the value is already in UpperCase must delete to avoid Regional Settings problems (Bugs tracking #1112)
        ''' </remarks>
        Private Function CreateMasterNode(ByVal pDBConnection As SqlClient.SqlConnection, ByRef pElementslist As List(Of WSRequiredElementsTO), _
                                          ByVal pMasterElementTitle As String, ByVal pRequireLinQ As Boolean, Optional ByVal pTestName As String = "") As GlobalDataTO
            Dim resultData As GlobalDataTO
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim createNode As Boolean = True
                        Dim queryElementTitle As String = String.Empty
                        'Dim fixedItemDesc As String = "FixedItemDesc"
                        Dim myPreMasterDataDS As PreloadedMasterDataDS
                        Dim myPreMasterDataDelegate As New PreloadedMasterDataDelegate

                        If (pRequireLinQ) Then
                            Select Case UCase(pMasterElementTitle)
                                Case "REAGENTS"
                                    'Get title used for Reagents (Level 1)
                                    resultData = myPreMasterDataDelegate.GetSubTableItem(dbConnection, PreloadedMasterDataEnum.TREE_LEVEL1_TITLES, "REAGENTS")

                                Case "TESTNAME"
                                    'Nothing to do in this case...

                                Case "ADD_SOL"
                                    'Get title used for Additional Solutions (Level 1)
                                    resultData = myPreMasterDataDelegate.GetSubTableItem(dbConnection, PreloadedMasterDataEnum.TREE_LEVEL1_TITLES, "ADD_SOL")

                                Case "ADD_TUBESOL"
                                    'Get title used for Tube Additional Solutions (Level 2)
                                    resultData = myPreMasterDataDelegate.GetSubTableItem(dbConnection, PreloadedMasterDataEnum.TREE_LEVEL2_TITLES, "ADD_SOL")

                                Case "SAMPLES"
                                    'Get title used for Samples (Level 1)
                                    resultData = myPreMasterDataDelegate.GetSubTableItem(dbConnection, PreloadedMasterDataEnum.TREE_LEVEL1_TITLES, "SAMPLES")

                                Case "CALIBS"
                                    'Get title used for Calibrators (Level 2 - Samples child)
                                    resultData = myPreMasterDataDelegate.GetSubTableItem(dbConnection, PreloadedMasterDataEnum.TREE_LEVEL2_TITLES, "CALIBS")

                                Case "CTRLS"
                                    'Get title used for Controls (Level 2 - Samples child)
                                    resultData = myPreMasterDataDelegate.GetSubTableItem(dbConnection, PreloadedMasterDataEnum.TREE_LEVEL2_TITLES, "CTRLS")

                                Case "STATS"
                                    'Get title used for Stat Patient Samples (Level 2 - Samples child)
                                    resultData = myPreMasterDataDelegate.GetSubTableItem(dbConnection, PreloadedMasterDataEnum.TREE_LEVEL2_TITLES, "STATS")

                                Case "ROUTINES"
                                    'Get title used for Routine Patient Samples (Level 2 - Samples child)
                                    resultData = myPreMasterDataDelegate.GetSubTableItem(dbConnection, PreloadedMasterDataEnum.TREE_LEVEL2_TITLES, "ROUTINES")

                                Case "SAMPLESNODE" ' dl 10/12/2010
                                    'Get title used for Samples (Level 1)
                                    resultData = myPreMasterDataDelegate.GetSubTableItem(dbConnection, PreloadedMasterDataEnum.TREE_LEVEL1_TITLES, "SAMPLESNODE")
                            End Select

                            If (UCase(pMasterElementTitle) <> "TESTNAME") Then
                                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                    myPreMasterDataDS = DirectCast(resultData.SetDatos, PreloadedMasterDataDS)
                                    If (myPreMasterDataDS.tfmwPreloadedMasterData.Rows.Count > 0) Then queryElementTitle = myPreMasterDataDS.tfmwPreloadedMasterData(0).FixedItemDesc
                                End If
                            Else
                                queryElementTitle = pTestName
                            End If

                            Dim query = (From res In pElementslist _
                                        Where res.ElementTitle = queryElementTitle _
                                       Select res)
                            createNode = (query.Count = 0)
                        End If

                        If (createNode) Then
                            Dim fatherVar As String = String.Empty
                            Dim iconNameVar As String = String.Empty
                            Dim elementTitleVar As String = String.Empty

                            Select Case UCase(pMasterElementTitle)
                                Case "REAGENTS"
                                    'Get the name of the Icon used for Reagents
                                    resultData = myPreMasterDataDelegate.GetSubTableItem(dbConnection, PreloadedMasterDataEnum.ICON_PATHS, "REAGENTS")
                                    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                        myPreMasterDataDS = DirectCast(resultData.SetDatos, PreloadedMasterDataDS)
                                        If (myPreMasterDataDS.tfmwPreloadedMasterData.Rows.Count > 0) Then iconNameVar = myPreMasterDataDS.tfmwPreloadedMasterData(0).FixedItemDesc

                                        'Get title used for Reagents (Level 1)
                                        resultData = myPreMasterDataDelegate.GetSubTableItem(dbConnection, PreloadedMasterDataEnum.TREE_LEVEL1_TITLES, "REAGENTS")
                                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                            myPreMasterDataDS = DirectCast(resultData.SetDatos, PreloadedMasterDataDS)
                                            If (myPreMasterDataDS.tfmwPreloadedMasterData.Rows.Count > 0) Then elementTitleVar = myPreMasterDataDS.tfmwPreloadedMasterData(0).FixedItemDesc
                                        End If
                                    End If

                                Case "TESTNAME"
                                    'Get the name of the Icon used for Reagents
                                    resultData = myPreMasterDataDelegate.GetSubTableItem(dbConnection, PreloadedMasterDataEnum.ICON_PATHS, "REAGENTS")
                                    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                        myPreMasterDataDS = DirectCast(resultData.SetDatos, PreloadedMasterDataDS)
                                        If (myPreMasterDataDS.tfmwPreloadedMasterData.Rows.Count > 0) Then iconNameVar = myPreMasterDataDS.tfmwPreloadedMasterData(0).FixedItemDesc

                                        'Get title used for Reagents (Level 1)
                                        resultData = myPreMasterDataDelegate.GetSubTableItem(dbConnection, PreloadedMasterDataEnum.TREE_LEVEL1_TITLES, "REAGENTS")
                                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                            myPreMasterDataDS = DirectCast(resultData.SetDatos, PreloadedMasterDataDS)
                                            If (myPreMasterDataDS.tfmwPreloadedMasterData.Rows.Count > 0) Then fatherVar = myPreMasterDataDS.tfmwPreloadedMasterData(0).FixedItemDesc
                                        End If
                                    End If

                                Case "ADD_SOL"
                                    'Get the name of the Icon used for Additional Solutions
                                    resultData = myPreMasterDataDelegate.GetSubTableItem(dbConnection, PreloadedMasterDataEnum.ICON_PATHS, "ADD_SOL")
                                    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                        myPreMasterDataDS = DirectCast(resultData.SetDatos, PreloadedMasterDataDS)
                                        If (myPreMasterDataDS.tfmwPreloadedMasterData.Rows.Count > 0) Then iconNameVar = myPreMasterDataDS.tfmwPreloadedMasterData(0).FixedItemDesc

                                        'Get title used for Additional Solutions (Level 1)
                                        resultData = myPreMasterDataDelegate.GetSubTableItem(dbConnection, PreloadedMasterDataEnum.TREE_LEVEL1_TITLES, "ADD_SOL")
                                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                            myPreMasterDataDS = DirectCast(resultData.SetDatos, PreloadedMasterDataDS)
                                            If (myPreMasterDataDS.tfmwPreloadedMasterData.Rows.Count > 0) Then elementTitleVar = myPreMasterDataDS.tfmwPreloadedMasterData(0).FixedItemDesc
                                        End If
                                    End If

                                Case "ADD_TUBESOL"
                                    'Get the name of the Icon used for Tube Additional Solutions
                                    resultData = myPreMasterDataDelegate.GetSubTableItem(dbConnection, PreloadedMasterDataEnum.ICON_PATHS, "ADD_SAMPLE_SOL")
                                    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                        myPreMasterDataDS = DirectCast(resultData.SetDatos, PreloadedMasterDataDS)
                                        If (myPreMasterDataDS.tfmwPreloadedMasterData.Rows.Count > 0) Then iconNameVar = myPreMasterDataDS.tfmwPreloadedMasterData(0).FixedItemDesc

                                        'Get title used for Tube Additional Solutions (Level 2)
                                        resultData = myPreMasterDataDelegate.GetSubTableItem(dbConnection, PreloadedMasterDataEnum.TREE_LEVEL2_TITLES, "ADD_SOL")
                                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                            myPreMasterDataDS = DirectCast(resultData.SetDatos, PreloadedMasterDataDS)
                                            If (myPreMasterDataDS.tfmwPreloadedMasterData.Rows.Count > 0) Then elementTitleVar = myPreMasterDataDS.tfmwPreloadedMasterData(0).FixedItemDesc

                                            'Get title used for Samples (Level 1)
                                            resultData = myPreMasterDataDelegate.GetSubTableItem(dbConnection, PreloadedMasterDataEnum.TREE_LEVEL1_TITLES, "SAMPLES")
                                            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                                myPreMasterDataDS = DirectCast(resultData.SetDatos, PreloadedMasterDataDS)
                                                If (myPreMasterDataDS.tfmwPreloadedMasterData.Rows.Count > 0) Then fatherVar = myPreMasterDataDS.tfmwPreloadedMasterData(0).FixedItemDesc
                                            End If
                                        End If
                                    End If

                                Case "SAMPLES"
                                    'Get the name of the Icon used for Samples
                                    resultData = myPreMasterDataDelegate.GetSubTableItem(dbConnection, PreloadedMasterDataEnum.ICON_PATHS, "SAMPLESNODE")
                                    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                        myPreMasterDataDS = DirectCast(resultData.SetDatos, PreloadedMasterDataDS)
                                        If (myPreMasterDataDS.tfmwPreloadedMasterData.Rows.Count > 0) Then iconNameVar = myPreMasterDataDS.tfmwPreloadedMasterData(0).FixedItemDesc

                                        'Get title used for Samples (Level 1)
                                        resultData = myPreMasterDataDelegate.GetSubTableItem(dbConnection, PreloadedMasterDataEnum.TREE_LEVEL1_TITLES, "SAMPLES")
                                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                            myPreMasterDataDS = DirectCast(resultData.SetDatos, PreloadedMasterDataDS)
                                            If (myPreMasterDataDS.tfmwPreloadedMasterData.Rows.Count > 0) Then elementTitleVar = myPreMasterDataDS.tfmwPreloadedMasterData(0).FixedItemDesc
                                        End If
                                    End If

                                Case "CALIBS"
                                    'Get the name of the Icon used for Calibrators
                                    resultData = myPreMasterDataDelegate.GetSubTableItem(dbConnection, PreloadedMasterDataEnum.ICON_PATHS, "CALIB")
                                    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                        myPreMasterDataDS = DirectCast(resultData.SetDatos, PreloadedMasterDataDS)
                                        If (myPreMasterDataDS.tfmwPreloadedMasterData.Rows.Count > 0) Then iconNameVar = myPreMasterDataDS.tfmwPreloadedMasterData(0).FixedItemDesc

                                        'Get title used for Calibrators (Level 2)
                                        resultData = myPreMasterDataDelegate.GetSubTableItem(dbConnection, PreloadedMasterDataEnum.TREE_LEVEL2_TITLES, "CALIBS")
                                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                            myPreMasterDataDS = DirectCast(resultData.SetDatos, PreloadedMasterDataDS)
                                            If (myPreMasterDataDS.tfmwPreloadedMasterData.Rows.Count > 0) Then elementTitleVar = myPreMasterDataDS.tfmwPreloadedMasterData(0).FixedItemDesc

                                            'Get title used for Samples (Level 1)
                                            resultData = myPreMasterDataDelegate.GetSubTableItem(dbConnection, PreloadedMasterDataEnum.TREE_LEVEL1_TITLES, "SAMPLES")
                                            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                                myPreMasterDataDS = DirectCast(resultData.SetDatos, PreloadedMasterDataDS)
                                                If (myPreMasterDataDS.tfmwPreloadedMasterData.Rows.Count > 0) Then fatherVar = myPreMasterDataDS.tfmwPreloadedMasterData(0).FixedItemDesc
                                            End If
                                        End If
                                    End If

                                Case "CTRLS"
                                    'Get the name of the Icon used for Controls
                                    resultData = myPreMasterDataDelegate.GetSubTableItem(dbConnection, PreloadedMasterDataEnum.ICON_PATHS, "CTRL")
                                    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                        myPreMasterDataDS = DirectCast(resultData.SetDatos, PreloadedMasterDataDS)
                                        If (myPreMasterDataDS.tfmwPreloadedMasterData.Rows.Count > 0) Then iconNameVar = myPreMasterDataDS.tfmwPreloadedMasterData(0).FixedItemDesc

                                        'Get title used for Controls (Level 2)
                                        resultData = myPreMasterDataDelegate.GetSubTableItem(dbConnection, PreloadedMasterDataEnum.TREE_LEVEL2_TITLES, "CTRLS")
                                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                            myPreMasterDataDS = DirectCast(resultData.SetDatos, PreloadedMasterDataDS)
                                            If (myPreMasterDataDS.tfmwPreloadedMasterData.Rows.Count > 0) Then elementTitleVar = myPreMasterDataDS.tfmwPreloadedMasterData(0).FixedItemDesc

                                            'Get title used for Samples (Level 1)
                                            resultData = myPreMasterDataDelegate.GetSubTableItem(dbConnection, PreloadedMasterDataEnum.TREE_LEVEL1_TITLES, "SAMPLES")
                                            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                                myPreMasterDataDS = DirectCast(resultData.SetDatos, PreloadedMasterDataDS)
                                                If (myPreMasterDataDS.tfmwPreloadedMasterData.Rows.Count > 0) Then fatherVar = myPreMasterDataDS.tfmwPreloadedMasterData(0).FixedItemDesc
                                            End If
                                        End If
                                    End If

                                Case "STATS"
                                    'Get the name of the Icon used for Stat Patient Samples
                                    resultData = myPreMasterDataDelegate.GetSubTableItem(dbConnection, PreloadedMasterDataEnum.ICON_PATHS, "STATS")
                                    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                        myPreMasterDataDS = DirectCast(resultData.SetDatos, PreloadedMasterDataDS)
                                        If (myPreMasterDataDS.tfmwPreloadedMasterData.Rows.Count > 0) Then iconNameVar = myPreMasterDataDS.tfmwPreloadedMasterData(0).FixedItemDesc

                                        'Get title used for Stat Patient Samples (Level 2)
                                        resultData = myPreMasterDataDelegate.GetSubTableItem(dbConnection, PreloadedMasterDataEnum.TREE_LEVEL2_TITLES, "STATS")
                                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                            myPreMasterDataDS = DirectCast(resultData.SetDatos, PreloadedMasterDataDS)
                                            If (myPreMasterDataDS.tfmwPreloadedMasterData.Rows.Count > 0) Then elementTitleVar = myPreMasterDataDS.tfmwPreloadedMasterData(0).FixedItemDesc

                                            'Get title used for Samples (Level 1)
                                            resultData = myPreMasterDataDelegate.GetSubTableItem(dbConnection, PreloadedMasterDataEnum.TREE_LEVEL1_TITLES, "SAMPLES")
                                            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                                myPreMasterDataDS = DirectCast(resultData.SetDatos, PreloadedMasterDataDS)
                                                If (myPreMasterDataDS.tfmwPreloadedMasterData.Rows.Count > 0) Then fatherVar = myPreMasterDataDS.tfmwPreloadedMasterData(0).FixedItemDesc
                                            End If
                                        End If
                                    End If

                                Case "ROUTINES"
                                    'Get the name of the Icon used for Routine Patient Samples
                                    resultData = myPreMasterDataDelegate.GetSubTableItem(dbConnection, PreloadedMasterDataEnum.ICON_PATHS, "ROUTINES")
                                    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                        myPreMasterDataDS = DirectCast(resultData.SetDatos, PreloadedMasterDataDS)
                                        If (myPreMasterDataDS.tfmwPreloadedMasterData.Rows.Count > 0) Then iconNameVar = myPreMasterDataDS.tfmwPreloadedMasterData(0).FixedItemDesc

                                        'Get title used for Routine Patient Samples (Level 2)
                                        resultData = myPreMasterDataDelegate.GetSubTableItem(dbConnection, PreloadedMasterDataEnum.TREE_LEVEL2_TITLES, "ROUTINES")
                                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                            myPreMasterDataDS = DirectCast(resultData.SetDatos, PreloadedMasterDataDS)
                                            If (myPreMasterDataDS.tfmwPreloadedMasterData.Rows.Count > 0) Then elementTitleVar = myPreMasterDataDS.tfmwPreloadedMasterData(0).FixedItemDesc

                                            'Get title used for Samples (Level 1)
                                            resultData = myPreMasterDataDelegate.GetSubTableItem(dbConnection, PreloadedMasterDataEnum.TREE_LEVEL1_TITLES, "SAMPLES")
                                            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                                myPreMasterDataDS = DirectCast(resultData.SetDatos, PreloadedMasterDataDS)
                                                If (myPreMasterDataDS.tfmwPreloadedMasterData.Rows.Count > 0) Then fatherVar = myPreMasterDataDS.tfmwPreloadedMasterData(0).FixedItemDesc
                                            End If
                                        End If
                                    End If
                            End Select

                            Dim newElementList As New WSRequiredElementsTO
                            With newElementList
                                .Father = fatherVar

                                'If (pMasterElementTitle.ToUpper() <> "TESTNAME") Then
                                If (pMasterElementTitle <> "TESTNAME") Then
                                    .ElementTitle = elementTitleVar
                                Else
                                    .ElementTitle = pTestName
                                End If

                                .ElementStatus = "NOPOS"
                                .ElementIcon = iconNameVar
                                .Allow = False
                            End With
                            pElementslist.Add(newElementList)
                        End If
                    End If
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSRequiredElementsDelegate.CreateMasterNode", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Fill the required fields in order to add new element to the RequiredElementsList
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWSRequiredElementRow">Row of a typed DataSet WSRequiredElementsDS containing the information of the 
        '''                                     Element to add</param>
        ''' <returns>GlobalDataTO with setData = WSRequiredElementsTO</returns>
        ''' <remarks>
        ''' Created by:  AG 18/11/2009 - Tested: OK - 26/11/2009
        ''' Modified by: VR 29/12/2009 - Change the Constant value to Enum Value
        '''              SA 09/03/2010 - Changes to open the DB Connection according the new template; changes to shown new field
        '''                              SampleID when it is informed (instead of fields PatientID or OrderID); changes to receive 
        '''                              a GlobalDataTO when calling function GetSubTableItem in PreloadedMasterDataDelegate
        '''              RH 14/06/2011 - Add Additional Tube Solutions logic
        '''              AG 02/01/2012 - Volume for Reagents and Additional Solutions have to be formatted with 2 decimals 
        '''              SA 21/02/2012 - For additional solutions and also for the ISE WashingSolution, do not show the required volume 
        '''                              following the name (it is always zero)
        '''              RH 17/04/2012 - Code optimization: remove string members ToString() convertions all around. Do not convert apples into apples!
        '''              SG 30/04/2013 - When load the WSRequiredElementsTO for PATIENTS, if field SpecimenIDList in the received WSRequiredElementsDS 
        '''                              row is not NULL nor EMPTY, assign its value to field ElementToolTip in the TO
        '''              SA 28/08/2013 - Added changes to shown the list of Specimen IDs + (PatientID/SampleID) + SampleType as ElementTitle (and the 
        '''                              same as a ToolTip) when field SpecimenIDList is informed 
        ''' </remarks>
        Private Function FillWSRequiredElementTO(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWSRequiredElementRow As DataRow) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim toReturnData As New WSRequiredElementsTO
                        Dim myPreloadedMasterDataDS As New PreloadedMasterDataDS
                        Dim myPreloadedMasterDataDelegate As New PreloadedMasterDataDelegate

                        Select Case UCase(pWSRequiredElementRow("TubeContent").ToString)
                            Case "REAGENT"
                                'RequiredElement is a Reagent 
                                Dim reagentRow As WSRequiredElementsTreeDS.ReagentsRow
                                reagentRow = CType(pWSRequiredElementRow, WSRequiredElementsTreeDS.ReagentsRow)

                                'Set values to WSRequiredElementsTO for Reagents
                                With reagentRow
                                    toReturnData.Father = .TestName
                                    toReturnData.ElementCode = .ReagentID.ToString
                                    toReturnData.ElementStatus = .ElementStatus

                                    'AG 02/01/2012 - Format with 2 decimals as we do with Real volume in position information area
                                    toReturnData.ElementTitle = .ReagentName & " (" & .RequiredVolume.ToStringWithDecimals(2, True) & ")"

                                    toReturnData.ElementID = .ElementID 'CType(.ElementID.ToString, Integer) 'WTF is this?
                                    toReturnData.TubeContent = .TubeContent
                                    toReturnData.Allow = True
                                End With

                                'Get the name of the Icon used for Reagents
                                resultData = myPreloadedMasterDataDelegate.GetSubTableItem(dbConnection, PreloadedMasterDataEnum.ICON_PATHS, "REAGENTS")
                                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                    myPreloadedMasterDataDS = DirectCast(resultData.SetDatos, PreloadedMasterDataDS)
                                    If (myPreloadedMasterDataDS.tfmwPreloadedMasterData.Rows.Count > 0) Then
                                        toReturnData.ElementIcon = myPreloadedMasterDataDS.tfmwPreloadedMasterData(0).FixedItemDesc
                                    End If
                                End If

                            Case "SPEC_SOL", "WASH_SOL"
                                'RequiredElement is an Special or Washing Solution
                                Dim specSolutionRow As WSRequiredElementsTreeDS.AdditionalSolutionsRow
                                specSolutionRow = CType(pWSRequiredElementRow, WSRequiredElementsTreeDS.AdditionalSolutionsRow)

                                'Set values to WSRequiredElementsTO for Additional Solutions
                                With specSolutionRow
                                    toReturnData.ElementCode = .SolutionCode

                                    'AG 02/01/2012 - Format with 2 decimals as we do with Real volume in position information area
                                    toReturnData.ElementTitle = .SolutionName '& " (" & .RequiredVolume.ToStringWithDecimals(2, True) & ")"

                                    toReturnData.ElementStatus = .ElementStatus
                                    toReturnData.ElementID = .ElementID 'CType(.ElementID.ToString, Integer) 'WTF is this?
                                    toReturnData.TubeContent = .TubeContent
                                    toReturnData.Allow = True
                                End With

                                'Get the title used for the group of Additional Solutions
                                resultData = myPreloadedMasterDataDelegate.GetSubTableItem(dbConnection, PreloadedMasterDataEnum.TREE_LEVEL1_TITLES, "ADD_SOL")
                                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                    myPreloadedMasterDataDS = DirectCast(resultData.SetDatos, PreloadedMasterDataDS)
                                    If (myPreloadedMasterDataDS.tfmwPreloadedMasterData.Rows.Count > 0) Then
                                        toReturnData.Father = myPreloadedMasterDataDS.tfmwPreloadedMasterData(0).FixedItemDesc
                                    End If

                                    'Get the name of the Icon used for Additional Solutions
                                    resultData = myPreloadedMasterDataDelegate.GetSubTableItem(dbConnection, PreloadedMasterDataEnum.ICON_PATHS, "ADD_SOL")
                                    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                        myPreloadedMasterDataDS = DirectCast(resultData.SetDatos, PreloadedMasterDataDS)
                                        If (myPreloadedMasterDataDS.tfmwPreloadedMasterData.Rows.Count > 0) Then
                                            toReturnData.ElementIcon = myPreloadedMasterDataDS.tfmwPreloadedMasterData(0).FixedItemDesc
                                        End If
                                    End If
                                End If

                            Case "TUBE_SPEC_SOL", "TUBE_WASH_SOL" 'RH 14/06/2011
                                'RequiredElement is a Special or Washing Solution used in Samples Rotor
                                Dim specSolutionRow As WSRequiredElementsTreeDS.TubeAdditionalSolutionsRow
                                specSolutionRow = CType(pWSRequiredElementRow, WSRequiredElementsTreeDS.TubeAdditionalSolutionsRow)

                                'Set values to WSRequiredElementsTO for Additional Solutions
                                With specSolutionRow
                                    toReturnData.ElementCode = .SolutionCode

                                    'For ISE Washing Solution (WASHSOL3) do not show the required volume; it is always zero
                                    toReturnData.ElementTitle = .SolutionName
                                    'DL 01/03/2012. BUG: 383. Begin
                                    'If (.SolutionCode <> "WASHSOL3") Then toReturnData.ElementTitle &= " (" & .RequiredVolume.ToString & ")"
                                    'DL 01/03/2012. BUG: 383. Begin

                                    'toReturnData.ElementTitle = String.Format("{0} ({1})", .SolutionName, .RequiredVolume)
                                    toReturnData.ElementStatus = .ElementStatus
                                    toReturnData.ElementID = .ElementID
                                    toReturnData.TubeContent = .TubeContent
                                    toReturnData.Allow = True
                                End With

                                'Get the title used for the group of Additional Solutions
                                resultData = myPreloadedMasterDataDelegate.GetSubTableItem(dbConnection, PreloadedMasterDataEnum.TREE_LEVEL2_TITLES, "ADD_SOL")
                                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                    myPreloadedMasterDataDS = DirectCast(resultData.SetDatos, PreloadedMasterDataDS)
                                    If (myPreloadedMasterDataDS.tfmwPreloadedMasterData.Rows.Count > 0) Then
                                        toReturnData.Father = myPreloadedMasterDataDS.tfmwPreloadedMasterData(0).FixedItemDesc
                                    End If

                                    'Get the name of the Icon used for Additional Solutions
                                    'resultData = myPreloadedMasterDataDelegate.GetSubTableItem(dbConnection, PreloadedMasterDataEnum.ICON_PATHS, "ADD_SOL")
                                    resultData = myPreloadedMasterDataDelegate.GetSubTableItem(dbConnection, PreloadedMasterDataEnum.ICON_PATHS, "ADD_SAMPLE_SOL") 'RH 17/04/2012
                                    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                        myPreloadedMasterDataDS = DirectCast(resultData.SetDatos, PreloadedMasterDataDS)
                                        If (myPreloadedMasterDataDS.tfmwPreloadedMasterData.Rows.Count > 0) Then
                                            toReturnData.ElementIcon = myPreloadedMasterDataDS.tfmwPreloadedMasterData(0).FixedItemDesc
                                        End If
                                    End If
                                End If

                            Case "CALIB"
                                'RequiredElement is a Calibrator
                                Dim calibratorRow As WSRequiredElementsTreeDS.CalibratorsRow
                                calibratorRow = CType(pWSRequiredElementRow, WSRequiredElementsTreeDS.CalibratorsRow)

                                'Set values to WSRequiredElementsTO for Calibrators
                                With calibratorRow
                                    toReturnData.ElementCode = .CalibratorID.ToString()

                                    If (Not .IsNumOfCalibratorsNull) Then
                                        'For Multipoint Calibrators, the number of points is shown following the Calibrator Name
                                        toReturnData.ElementTitle = .CalibratorName & " (" & .NumOfCalibrators.ToString & ")"
                                    Else
                                        toReturnData.ElementTitle = .CalibratorName
                                    End If

                                    toReturnData.ElementStatus = .ElementStatus
                                    toReturnData.ElementID = .ElementID 'CType(.ElementID.ToString, Integer) 'WTF is this?
                                    toReturnData.TubeContent = .TubeContent
                                    toReturnData.Allow = True
                                End With

                                'Get the title used for the group of Calibrators
                                resultData = myPreloadedMasterDataDelegate.GetSubTableItem(dbConnection, PreloadedMasterDataEnum.TREE_LEVEL2_TITLES, "CALIBS")
                                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                    myPreloadedMasterDataDS = DirectCast(resultData.SetDatos, PreloadedMasterDataDS)
                                    If (myPreloadedMasterDataDS.tfmwPreloadedMasterData.Rows.Count > 0) Then
                                        toReturnData.Father = myPreloadedMasterDataDS.tfmwPreloadedMasterData(0).FixedItemDesc
                                    End If

                                    'Get the name of the Icon used for Calibrators
                                    resultData = myPreloadedMasterDataDelegate.GetSubTableItem(dbConnection, PreloadedMasterDataEnum.ICON_PATHS, "CALIB")
                                    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                        myPreloadedMasterDataDS = DirectCast(resultData.SetDatos, PreloadedMasterDataDS)
                                        If (myPreloadedMasterDataDS.tfmwPreloadedMasterData.Rows.Count > 0) Then
                                            toReturnData.ElementIcon = myPreloadedMasterDataDS.tfmwPreloadedMasterData(0).FixedItemDesc
                                        End If
                                    End If
                                End If

                            Case "CTRL"
                                'RequiredElement is a Control
                                Dim ctrlRow As WSRequiredElementsTreeDS.ControlsRow
                                ctrlRow = CType(pWSRequiredElementRow, WSRequiredElementsTreeDS.ControlsRow)

                                'Set values to WSRequiredElementsTO for Controls
                                With ctrlRow
                                    toReturnData.ElementCode = .ControlID.ToString
                                    toReturnData.ElementStatus = .ElementStatus
                                    toReturnData.ElementTitle = .ControlName '& " (" & .ControlNumber.ToString & ")"
                                    toReturnData.ElementID = .ElementID 'CType(.ElementID.ToString, Integer) 'WTF is this?
                                    toReturnData.TubeContent = .TubeContent
                                    toReturnData.Allow = True
                                End With

                                'Get the title used for the group of Controls
                                resultData = myPreloadedMasterDataDelegate.GetSubTableItem(dbConnection, PreloadedMasterDataEnum.TREE_LEVEL2_TITLES, "CTRLS")
                                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                    myPreloadedMasterDataDS = DirectCast(resultData.SetDatos, PreloadedMasterDataDS)
                                    If (myPreloadedMasterDataDS.tfmwPreloadedMasterData.Rows.Count > 0) Then
                                        toReturnData.Father = myPreloadedMasterDataDS.tfmwPreloadedMasterData(0).FixedItemDesc
                                    End If

                                    'Get the name of the Icon used for Controls
                                    resultData = myPreloadedMasterDataDelegate.GetSubTableItem(dbConnection, PreloadedMasterDataEnum.ICON_PATHS, "CTRL")
                                    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                        myPreloadedMasterDataDS = DirectCast(resultData.SetDatos, PreloadedMasterDataDS)
                                        If (myPreloadedMasterDataDS.tfmwPreloadedMasterData.Rows.Count > 0) Then
                                            toReturnData.ElementIcon = myPreloadedMasterDataDS.tfmwPreloadedMasterData(0).FixedItemDesc
                                        End If
                                    End If
                                End If

                            Case "PATIENT"
                                'RequiredElement is a Patient Sample
                                Dim patientRow As WSRequiredElementsTreeDS.PatientSamplesRow
                                patientRow = CType(pWSRequiredElementRow, WSRequiredElementsTreeDS.PatientSamplesRow)

                                ''Set values to WSRequiredElementsTO for Patient Samples
                                'With patientRow
                                '    toReturnData.ElementID = .ElementID 'CType(.ElementID.ToString, Integer) 'WTF is this?

                                '    The informed value between PatientID, SampleID or OrderID will be shown 
                                '    If (Not .IsOrderIDNull) AndAlso (.OrderID <> "") Then
                                '        toReturnData.ElementCode = .OrderID
                                '    ElseIf (Not .IsPatientIDNull) AndAlso (.PatientID <> "") Then
                                '        toReturnData.ElementCode = .PatientID
                                '    ElseIf (Not .IsSampleIDNull) AndAlso (.SampleID <> "") Then
                                '        toReturnData.ElementCode = .SampleID
                                '    End If


                                '    toReturnData.ElementStatus = .ElementStatus
                                '    toReturnData.ElementTitle = toReturnData.ElementCode & "-" & .SampleType
                                '    toReturnData.TubeContent = .TubeContent
                                '    toReturnData.Allow = True

                                '    SGM 30 / 4 / 2013
                                '    If (patientRow.IsPredilutionFactorNull) Then toReturnData.ElementToolTip = .SpecimenIDList
                                'End With

                                'SA 28/08/2013 - See comments about this code in the function header
                                With patientRow
                                    toReturnData.ElementID = .ElementID
                                    toReturnData.ElementStatus = .ElementStatus
                                    toReturnData.TubeContent = .TubeContent
                                    toReturnData.Allow = True

                                    'Get the informed value between PatientID, SampleID and OrderID
                                    If (Not .IsPatientIDNull) AndAlso (.PatientID <> String.Empty) Then
                                        toReturnData.ElementCode = .PatientID
                                    ElseIf (Not .IsSampleIDNull) AndAlso (.SampleID <> String.Empty) Then
                                        toReturnData.ElementCode = .SampleID
                                    ElseIf (Not .IsOrderIDNull) AndAlso (.OrderID <> String.Empty) Then
                                        toReturnData.ElementCode = .OrderID
                                    End If

                                    If (Not .IsSpecimenIDListNull AndAlso .SpecimenIDList.Trim <> String.Empty) Then
                                        toReturnData.ElementTitle = .SpecimenIDList.Trim.Replace(vbCrLf, ", ") & " (" & toReturnData.ElementCode & ")" & "-" & .SampleType
                                    Else
                                        toReturnData.ElementTitle = toReturnData.ElementCode & "-" & .SampleType
                                    End If
                                    toReturnData.ElementToolTip = toReturnData.ElementTitle
                                End With

                                Dim subTableItemID As String = "ROUTINES"
                                If (patientRow.StatFlag) Then
                                    subTableItemID = "STATS"
                                End If

                                'Get the title used for the group of Patient Samples (Stat or Routine)
                                resultData = myPreloadedMasterDataDelegate.GetSubTableItem(dbConnection, PreloadedMasterDataEnum.TREE_LEVEL2_TITLES, subTableItemID)
                                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                    myPreloadedMasterDataDS = DirectCast(resultData.SetDatos, PreloadedMasterDataDS)
                                    If (myPreloadedMasterDataDS.tfmwPreloadedMasterData.Rows.Count > 0) Then
                                        toReturnData.Father = myPreloadedMasterDataDS.tfmwPreloadedMasterData(0).FixedItemDesc
                                    End If

                                    'Get the name of the Icon used for Patient Samples (Stat or Routine)
                                    resultData = myPreloadedMasterDataDelegate.GetSubTableItem(dbConnection, PreloadedMasterDataEnum.ICON_PATHS, subTableItemID)
                                    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                        myPreloadedMasterDataDS = DirectCast(resultData.SetDatos, PreloadedMasterDataDS)
                                        If (myPreloadedMasterDataDS.tfmwPreloadedMasterData.Rows.Count > 0) Then
                                            toReturnData.ElementIcon = myPreloadedMasterDataDS.tfmwPreloadedMasterData(0).FixedItemDesc
                                        End If
                                    End If
                                End If
                        End Select

                        'Return the updated WSRequiredElementsTO
                        resultData.SetDatos = toReturnData
                        resultData.HasError = False
                    End If
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSRequiredElementsDelegate.FillWSRequiredElementTO", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()

            End Try

            Return resultData
        End Function

        ''' <summary>
        ''' Build the TreeView structure for Required Elements for Additional Solutions in a Work Session
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pElementsList">List of Elements in the TreeView of Required Elements</param>
        ''' <param name="pWSAdditionalSolutions">List of Additional Solutions in the WS</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by:  AG 18/11/2009 - Tested: OK - 26/11/2009
        ''' Modified by: SA 09/03/2010 - Changes to open the DB Connection according the new template
        ''' </remarks>
        Private Function InsertAddditionalsSolutions(ByVal pDBConnection As SqlClient.SqlConnection, ByRef pElementsList As List(Of WSRequiredElementsTO), _
                                                     ByVal pWSAdditionalSolutions As WSRequiredElementsTreeDS.AdditionalSolutionsDataTable) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Create main Node for Additional Solutions if it does not exist
                        Me.CreateMasterNode(dbConnection, pElementsList, "ADD_SOL", False)

                        Dim newElementList As New WSRequiredElementsTO
                        Dim additionalSolutionRow As WSRequiredElementsTreeDS.AdditionalSolutionsRow

                        For Each additionalSolutionRow In pWSAdditionalSolutions
                            'Add child Node 
                            resultData = Me.FillWSRequiredElementTO(dbConnection, additionalSolutionRow)
                            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                newElementList = CType(resultData.SetDatos, WSRequiredElementsTO)
                                pElementsList.Add(newElementList)
                            Else
                                'Error getting data for the Child Node
                                Exit For
                            End If
                        Next
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSRequiredElementsDelegate.InsertAddditionalsSolutions", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Build the TreeView structure for Required Elements for Calibrators in a Work Session
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pElementsList">List of Elements in the TreeView of Required Elements</param>
        ''' <param name="pWSCalibrators">List of Calibrators in the WS</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by:  AG 18/11/2009 - Tested: OK - 26/11/2009
        ''' Modified by: SA 09/03/2010 - Changes to open the DB Connection according the new template
        ''' </remarks>
        Private Function InsertCalibrators(ByVal pDBConnection As SqlClient.SqlConnection, ByRef pElementsList As List(Of WSRequiredElementsTO), _
                                           ByVal pWSCalibrators As WSRequiredElementsTreeDS.CalibratorsDataTable) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Create the main Node for Samples if it does not exist
                        Me.CreateMasterNode(dbConnection, pElementsList, "SAMPLES", True)

                        'Create the main Node for Calibrators if it does not exist
                        Me.CreateMasterNode(dbConnection, pElementsList, "CALIBS", False)

                        Dim newElementList As New WSRequiredElementsTO
                        Dim calibratorRow As WSRequiredElementsTreeDS.CalibratorsRow

                        For Each calibratorRow In pWSCalibrators
                            'Add child Node 
                            resultData = Me.FillWSRequiredElementTO(dbConnection, calibratorRow)
                            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                newElementList = CType(resultData.SetDatos, WSRequiredElementsTO)
                                pElementsList.Add(newElementList)
                            Else
                                'Error getting data for the Child Node
                                Exit For
                            End If
                        Next
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSRequiredElementsDelegate.InsertCalibrators", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Build the TreeView structure for Required Elements for Controls in a Work Session
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pElementsList">List of Elements in the TreeView of Required Elements</param>
        ''' <param name="pWSControls">List of Controls in the WS</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by:  AG 18/11/2009 - Tested: OK - 26/11/2009
        ''' Modified by: SA 09/03/2010 - Changes to open the DB Connection according the new template
        ''' </remarks>
        Private Function InsertControls(ByVal pDBConnection As SqlClient.SqlConnection, ByRef pElementsList As List(Of WSRequiredElementsTO), _
                                        ByVal pWSControls As WSRequiredElementsTreeDS.ControlsDataTable) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Create the main Node for Samples if it does not exist
                        Me.CreateMasterNode(dbConnection, pElementsList, "SAMPLES", True)

                        'Create the main Node for Controls if it does not exist
                        Me.CreateMasterNode(dbConnection, pElementsList, "CTRLS", False)

                        Dim newElementList As New WSRequiredElementsTO
                        Dim controlRow As WSRequiredElementsTreeDS.ControlsRow

                        For Each controlRow In pWSControls
                            'Add child Node 
                            resultData = Me.FillWSRequiredElementTO(dbConnection, controlRow)
                            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                newElementList = CType(resultData.SetDatos, WSRequiredElementsTO)
                                pElementsList.Add(newElementList)
                            Else
                                'Error getting data for the Child Node
                                Exit For
                            End If
                        Next
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSRequiredElementsDelegate.InsertControls", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Build the TreeView structure for Required Elements for Patient Samples in a Work Session
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pElementsList">List of Elements in the TreeView of Required Elements</param>
        ''' <param name="pWSPatients">List of Patient Samples in the WS</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by:  AG 18/11/2009 - Tested: OK - 26/11/2009
        ''' Modified by: SA 09/03/2010 - Changes to open the DB Connection according the new template; changes to shown new field
        '''                              SampleID when it is informed (instead of fields PatientID or OrderID)
        '''              RH 04/08/2011 - Modified logic at inserting a new node. Code optimization.
        '''              SA 28/08/2013 - When verifying if the Patient Sample already exists as a TreeView branch, the searching should be done 
        '''                              depending on value of field SpecimenIDList:
        '''                              ** If field is informed, search by ElementTitle = SpecimenIDList (PatientID/SampleID)-SampleType
        '''                              ** Otherwise, search by ElementTitle = PatientID/SampleID-SampleType
        ''' </remarks>
        Private Function InsertPatients(ByVal pDBConnection As SqlClient.SqlConnection, ByRef pElementsList As List(Of WSRequiredElementsTO), _
                                        ByVal pWSPatients As WSRequiredElementsTreeDS.PatientSamplesDataTable) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Create main Patient Samples Node if it does not exist
                        Me.CreateMasterNode(dbConnection, pElementsList, "SAMPLES", True)

                        Dim patientRow As WSRequiredElementsTreeDS.PatientSamplesRow
                        For Each patientRow In pWSPatients
                            'Create main Nodes for Stats and Routines if they do not exist
                            If (patientRow.StatFlag) Then
                                Me.CreateMasterNode(dbConnection, pElementsList, "STATS", True)
                            Else
                                Me.CreateMasterNode(dbConnection, pElementsList, "ROUTINES", True)
                            End If

                            ''RH 04/06/2011
                            ''Show PatientID, SampleID or OrderID (only one is informed at the same time)
                            'Dim searchQuery As String = String.Empty
                            'If (Not String.IsNullOrEmpty(patientRow.OrderID)) Then
                            '    searchQuery = patientRow.OrderID
                            'ElseIf (Not String.IsNullOrEmpty(patientRow.PatientID)) Then
                            '    searchQuery = patientRow.PatientID
                            'ElseIf (Not String.IsNullOrEmpty(patientRow.SampleID)) Then
                            '    searchQuery = patientRow.SampleID.ToString
                            'End If
                            'searchQuery = searchQuery & "-" & patientRow.SampleType

                            'SA 28/08/2013 - See comments about this code in the function header
                            Dim patientID As String = String.Empty
                            If (Not String.IsNullOrEmpty(patientRow.OrderID)) Then
                                patientID = patientRow.OrderID
                            ElseIf (Not String.IsNullOrEmpty(patientRow.PatientID)) Then
                                patientID = patientRow.PatientID
                            ElseIf (Not String.IsNullOrEmpty(patientRow.SampleID)) Then
                                patientID = patientRow.SampleID
                            End If

                            Dim searchQuery As String = String.Empty
                            If (Not patientRow.IsSpecimenIDListNull AndAlso patientRow.SpecimenIDList.Trim <> String.Empty) Then
                                searchQuery = patientRow.SpecimenIDList.Trim.Replace(vbCrLf, ", ") & " (" & patientID & ")"
                            Else
                                searchQuery = patientID
                            End If
                            searchQuery = searchQuery & "-" & patientRow.SampleType

                            Dim query As List(Of WSRequiredElementsTO) = (From res In pElementsList _
                                                                         Where res.ElementTitle = searchQuery _
                                                                        Select res).ToList()
                            If (patientRow.IsPredilutionFactorNull) Then
                                'It is not a Predilution
                                If (query.Count = 0) Then
                                    'Add child Node (PatientID, SampleID or OrderID)
                                    resultData = Me.FillWSRequiredElementTO(dbConnection, patientRow)

                                    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                        Dim newElementList As WSRequiredElementsTO
                                        newElementList = CType(resultData.SetDatos, WSRequiredElementsTO)
                                        pElementsList.Add(newElementList)
                                    Else
                                        'Error getting data for the Child Node
                                        Exit For
                                    End If
                                Else
                                    'Update the ElementID
                                    query.First().ElementID = patientRow.ElementID
                                    query.First().ElementStatus = patientRow.ElementStatus
                                    query.First().Allow = True 'Allowed to be positioned
                                End If
                            Else
                                'It is a Predilution. Create the parent node
                                If (query.Count = 0) Then
                                    'Add child Node (PatientID, SampleID or OrderID)
                                    resultData = Me.FillWSRequiredElementTO(dbConnection, patientRow)

                                    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                        Dim newElementList As WSRequiredElementsTO
                                        newElementList = CType(resultData.SetDatos, WSRequiredElementsTO)
                                        newElementList.ElementID = 0 'It is a parent node
                                        newElementList.ElementStatus = "NOPOS"
                                        newElementList.Allow = False 'Not allowed to be positioned
                                        pElementsList.Add(newElementList)
                                    Else
                                        'Error getting data for the Child Node
                                        Exit For
                                    End If
                                End If

                                'Insert the predilution
                                Me.InsertPredilutions(dbConnection, pElementsList, patientRow)
                            End If
                            'RH 04/06/2011 END
                        Next
                    End If
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSRequiredElementsDelegate.InsertPatients", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Build the TreeView structure for Required Elements for prediluted Patient Samples in a Work Session
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pElementsList">List of Elements in the TreeView of Required Elements</param>
        ''' <param name="pPatientRow">Patient Sample with predilution in the WS</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by:  AG 18/11/2009 - Tested: OK - 26/11/2009
        ''' Modified by: VR 29/12/2009 - Changed the Constant value to Enum Value
        '''              SA 09/03/2010 - Changes to open the DB Connection according the new template; changes to shown new field
        '''                              SampleID when it is informed (instead of fields PatientID or OrderID); remove hardcode for
        '''                              dilutions Icon, it should be get from the DB; changes to receive a GlobalDataTO
        '''                              when calling function GetSubTableItem in PreloadedMasterDataDelegate
        '''              SA 22/10/2010 - When the predilution element has OnlyForISE = TRUE, then concact to the ElementTitle the suffix (ISE)  
        '''              SA 28/08/2013 - When seeking the “father” node for the Predilution branch to add, the searching should be done depending 
        '''                              on value of field SpecimenIDList:
        '''                              ** If field is informed, search by ElementTitle = SpecimenIDList (PatientID/SampleID)-SampleType
        '''                              ** Otherwise, search by ElementTitle = PatientID/SampleID-SampleType
        ''' </remarks>
        Private Function InsertPredilutions(ByVal pDBConnection As SqlClient.SqlConnection, ByRef pElementsList As List(Of WSRequiredElementsTO), _
                                            ByVal pPatientRow As WSRequiredElementsTreeDS.PatientSamplesRow) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Dim fatherVar As String = ""
                        'If (pPatientRow.PatientID.ToString <> "") Then
                        '    fatherVar = pPatientRow.PatientID.ToString & "-" & pPatientRow.SampleType.ToString
                        'ElseIf (pPatientRow.SampleID.ToString <> "") Then
                        '    fatherVar = pPatientRow.SampleID.ToString & "-" & pPatientRow.SampleType.ToString
                        'Else
                        '    fatherVar = pPatientRow.OrderID.ToString & "-" & pPatientRow.SampleType.ToString
                        'End If

                        'SA 28/08/2013 - See comment about this code in the function header
                        Dim patientID As String = String.Empty
                        If (Not String.IsNullOrEmpty(pPatientRow.OrderID)) Then
                            patientID = pPatientRow.OrderID
                        ElseIf (Not String.IsNullOrEmpty(pPatientRow.PatientID)) Then
                            patientID = pPatientRow.PatientID
                        ElseIf (Not String.IsNullOrEmpty(pPatientRow.SampleID)) Then
                            patientID = pPatientRow.SampleID
                        End If

                        Dim fatherVar As String = String.Empty
                        If (Not pPatientRow.IsSpecimenIDListNull AndAlso pPatientRow.SpecimenIDList.Trim <> String.Empty) Then
                            fatherVar = pPatientRow.SpecimenIDList.Trim.Replace(vbCrLf, ", ") & " (" & patientID & ")" & "-" & pPatientRow.SampleType
                        Else
                            fatherVar = patientID & "-" & pPatientRow.SampleType
                        End If

                        Dim elementTitleVar As String = ""
                        Dim dilutionIconName As String = ""
                        Dim FixedItemDesc As String = "FixedItemDesc"
                        Dim myPreloadedMasterDataDS As New PreloadedMasterDataDS
                        Dim myPreloadedMasterDataDelegate As New PreloadedMasterDataDelegate

                        'Get the title used for predilutions
                        resultData = myPreloadedMasterDataDelegate.GetSubTableItem(dbConnection, PreloadedMasterDataEnum.TREE_LEVEL4_TITLES, "DILUTIONS")
                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            myPreloadedMasterDataDS = DirectCast(resultData.SetDatos, PreloadedMasterDataDS)
                            If (myPreloadedMasterDataDS.tfmwPreloadedMasterData.Rows.Count > 0) Then
                                elementTitleVar = myPreloadedMasterDataDS.tfmwPreloadedMasterData(0).FixedItemDesc
                            End If

                            'Get the name of the Icon used for predilutions
                            resultData = myPreloadedMasterDataDelegate.GetSubTableItem(dbConnection, PreloadedMasterDataEnum.ICON_PATHS, "DILUTIONS")
                            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                myPreloadedMasterDataDS = DirectCast(resultData.SetDatos, PreloadedMasterDataDS)
                                If (myPreloadedMasterDataDS.tfmwPreloadedMasterData.Rows.Count > 0) Then
                                    dilutionIconName = myPreloadedMasterDataDS.tfmwPreloadedMasterData(0).FixedItemDesc
                                End If
                            End If
                        End If

                        'Add the predilution to the list of Required Elements
                        Dim newElementList As New WSRequiredElementsTO
                        With newElementList
                            .Father = fatherVar
                            .ElementTitle = elementTitleVar & pPatientRow.PredilutionFactor.ToString
                            .ElementStatus = pPatientRow.ElementStatus.ToString
                            .ElementID = CType(pPatientRow.ElementID.ToString, Integer)
                            .TubeContent = pPatientRow.TubeContent.ToString
                            .ElementIcon = dilutionIconName
                            .Allow = True

                            If (pPatientRow.OnlyForISE) Then .ElementTitle &= " (ISE)"
                        End With
                        pElementsList.Add(newElementList)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSRequiredElementsDelegate.InsertPredilutions", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Build the TreeView structure for Required Elements for Reagents in a Work Session
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pElementsList">List of Elements in the TreeView of Required Elements</param>
        ''' <param name="pWSReagents">List of Reagents in the WS</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by:  AG 18/11/2009 - Tested: OK - 26/11/2009
        ''' Modified by: SA 09/03/2010 - Changes to open the DB Connection according the new template
        ''' </remarks>
        Private Function InsertReagents(ByVal pDBConnection As SqlClient.SqlConnection, ByRef pElementsList As List(Of WSRequiredElementsTO), _
                                        ByVal pWSReagents As WSRequiredElementsTreeDS.ReagentsDataTable) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Create main Node for Reagents if it does not exist
                        Me.CreateMasterNode(dbConnection, pElementsList, "REAGENTS", False)

                        Dim newElementList As New WSRequiredElementsTO
                        Dim reagentRow As WSRequiredElementsTreeDS.ReagentsRow

                        For Each reagentRow In pWSReagents
                            Me.CreateMasterNode(dbConnection, pElementsList, "TESTNAME", True, reagentRow.TestName)

                            'Add child Node 
                            resultData = Me.FillWSRequiredElementTO(dbConnection, reagentRow)
                            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                newElementList = CType(resultData.SetDatos, WSRequiredElementsTO)
                                pElementsList.Add(newElementList)
                            Else
                                'Error getting data for the Child Node
                                Exit For
                            End If
                        Next
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSRequiredElementsDelegate.InsertReagents", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Build the TreeView structure for Required Elements for Tube Additional Solutions in a Work Session
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pElementsList">List of Elements in the TreeView of Required Elements</param>
        ''' <param name="pWSTubeAdditionalSolutions">List of Tube Additional Solutions in the WS</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by:  RH 10/06/2011 based on InsertAddditionalsSolutions()
        ''' </remarks>
        Private Function InsertTubeAdditionalSolutions(ByVal pDBConnection As SqlClient.SqlConnection, ByRef pElementsList As List(Of WSRequiredElementsTO), _
                                                       ByVal pWSTubeAdditionalSolutions As WSRequiredElementsTreeDS.TubeAdditionalSolutionsDataTable) As GlobalDataTO
            Dim resultData As GlobalDataTO
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Create the main Node for Samples if it does not exist
                        Me.CreateMasterNode(dbConnection, pElementsList, "SAMPLES", True)

                        'Create main Node for TubeAdditional Solutions if it does not exist
                        Me.CreateMasterNode(dbConnection, pElementsList, "ADD_TUBESOL", False)

                        Dim newElementList As New WSRequiredElementsTO
                        Dim tubeAdditionalSolutionRow As WSRequiredElementsTreeDS.TubeAdditionalSolutionsRow

                        For Each tubeAdditionalSolutionRow In pWSTubeAdditionalSolutions
                            'Add child Node 
                            resultData = Me.FillWSRequiredElementTO(dbConnection, tubeAdditionalSolutionRow)
                            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                newElementList = CType(resultData.SetDatos, WSRequiredElementsTO)
                                pElementsList.Add(newElementList)
                            Else
                                'Error getting data for the Child Node
                                Exit For
                            End If
                        Next
                    End If
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSRequiredElementsDelegate.InsertTubeAddditionalsSolutions", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()

            End Try

            Return resultData
        End Function

#End Region

#Region "TO REVIEW - DELETE?"
        ' ''' <summary>
        ' ''' Validate if there are open Order Tests for the specified Element Identifier
        ' ''' </summary>
        ' ''' <param name="pDBConnection">Open DB Connection</param>
        ' ''' <param name="pElementID">Element Identifier</param>
        ' ''' <returns>GlobalDataTO containing a typed DataSet OrderTestsDS with data of all OPEN Order Tests related with the specified Element</returns>
        ' ''' <remarks>
        ' ''' Created by:  TR 21/11/2013 - BT #1380
        ' ''' </remarks>
        'Public Function ValidateReagentRotorElementIsRequired(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pElementID As Integer) As GlobalDataTO
        '    Dim myGlobalDataTO As GlobalDataTO = Nothing
        '    Dim dbConnection As SqlClient.SqlConnection = Nothing
        '    Try
        '        myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
        '        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '            dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim myOrderTestDelegate As New OrderTestsDelegate
        '                myGlobalDataTO = myOrderTestDelegate.GetNotClosedOrderTestByElementID(dbConnection, pElementID)
        '            End If
        '        End If
        '    Catch ex As Exception
        '        myGlobalDataTO = New GlobalDataTO()
        '        myGlobalDataTO.HasError = True
        '        myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myGlobalDataTO.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSRequiredElementsDelegate.ValidateReagentRotorElementIsRequired", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return myGlobalDataTO
        'End Function

        '''' <summary>
        '''' Get the list of Required Elements for an Order Test belonging to a Control Order according its TestID/SampleType
        '''' </summary>
        '''' <param name="pDBConnection">Open Database Connection</param>
        '''' <param name="pWorkSessionID">Work Session Identifier</param>
        '''' <param name="pOrderTestDetailsRow">Row with structure of DataSet OrderTestsDetailsDS containing
        ''''                                    TestID and SampleType</param>
        '''' <returns>DataSet containing the list of Required Control Elements for the specified Order Test</returns>
        '''' <remarks>
        '''' Created by:  SA
        '''' Modified by: SA 05/01/2010 - Changed the way of open the DB Connection to the new template; changes to return a 
        ''''                              GlobalDataTO instead a typed DataSet WSElementsByOrderTestDS   
        '''' </remarks>
        'Public Function GetControlElements(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, _
        '                                   ByVal pOrderTestDetailsRow As OrderTestsDetailsDS.OrderTestsDetailsRow) As GlobalDataTO
        '    Dim myGlobalDataTO As New GlobalDataTO
        '    Dim dbConnection As New SqlClient.SqlConnection

        '    Try
        '        myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
        '        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '            dbConnection = CType(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                'Get Required Control Elements
        '                Dim requiredElementsDS As New WSElementsByOrderTestDS
        '                Dim requiredElementsData As New twksWSRequiredElementsDAO

        '                myGlobalDataTO = requiredElementsData.GetControlElements(dbConnection, pWorkSessionID, pOrderTestDetailsRow)
        '                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '                    requiredElementsDS = DirectCast(myGlobalDataTO.SetDatos, WSElementsByOrderTestDS)

        '                    'The identifier of the Order Test is included for all records of the returned DataSet
        '                    For i As Integer = 0 To requiredElementsDS.twksWSRequiredElemByOrderTest.Rows.Count - 1
        '                        requiredElementsDS.twksWSRequiredElemByOrderTest(i).BeginEdit()
        '                        requiredElementsDS.twksWSRequiredElemByOrderTest(i).OrderTestID = pOrderTestDetailsRow.OrderTestID
        '                        requiredElementsDS.twksWSRequiredElemByOrderTest(i).EndEdit()
        '                    Next

        '                    'Get Required Calibrators Elements
        '                    myGlobalDataTO = GetCalibratorElements(dbConnection, pWorkSessionID, pOrderTestDetailsRow)

        '                    If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '                        Dim calibratorElementsDS As WSElementsByOrderTestDS = DirectCast(myGlobalDataTO.SetDatos, WSElementsByOrderTestDS)

        '                        'Add the obtained records to the DataSet to return 
        '                        For Each calibratorElementsDR As WSElementsByOrderTestDS.twksWSRequiredElemByOrderTestRow In calibratorElementsDS.twksWSRequiredElemByOrderTest.Rows
        '                            requiredElementsDS.twksWSRequiredElemByOrderTest.ImportRow(calibratorElementsDR)
        '                        Next

        '                        myGlobalDataTO.HasError = False
        '                        myGlobalDataTO.SetDatos = requiredElementsDS
        '                    End If
        '                End If
        '            End If
        '        End If
        '    Catch ex As Exception
        '        myGlobalDataTO.HasError = True
        '        myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myGlobalDataTO.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSRequiredElementsDelegate.GetControlElements", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return myGlobalDataTO
        'End Function

        '''' <summary>
        '''' Get the list of Required Elements for an Order Test belonging to a Patient's Order according its SampleType and   
        '''' PatientID (or OrderID, for Patient's Orders in which the Patient is not informed)
        '''' </summary>
        '''' <param name="pDBConnection">Open Database Connection</param>
        '''' <param name="pWorkSessionID">Work Session Identifier</param>
        '''' <param name="pOrderTestDetailsRow">Row with structure of DataSet OrderTestsDetailsDS containing
        ''''                                    SampleType and PatientID/SampleID/OrderID</param>
        ''''<returns>DataSet containing the list of Required Patient Sample Elements for the 
        ''''         specified Order Test</returns>
        '''' <remarks>
        '''' Created by:  SA
        '''' Modified by: VR 09/12/2009 - To fix the Error after changed the returned Data Type as GlobalDataTo
        ''''              SA 05/01/2010 - Error fixed: GetPatientElements returns WSRequiredElementsDS in the GlobalDataTO,
        ''''                              not WSElementsByOrderTestDS
        ''''              SA 05/01/2010 - Changed the way of open the DB Connection to the new template; changes to return a 
        ''''                              GlobalDataTO instead a typed DataSet WSElementsByOrderTestDS 
        ''''              RH 13/04/2010 - Deleted Get Required Control Elements. Added Get Required Calibrators Elements 
        ''''              SA 25/10/2010 - Before get the required Calibrators and Blanks, verify the Order Test in process 
        ''''                              corresponds to an Standard Test
        '''' </remarks>
        'Public Function GetPatientElements(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, _
        '                                   ByVal pOrderTestDetailsRow As OrderTestsDetailsDS.OrderTestsDetailsRow) As GlobalDataTO
        '    Dim myGlobalDataTO As New GlobalDataTO
        '    Dim dbConnection As New SqlClient.SqlConnection

        '    Try
        '        myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
        '        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '            dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                'Get Required Patient's Sample Elements
        '                Dim noError As Boolean = True
        '                Dim orderTestElementsDS As New WSElementsByOrderTestDS
        '                Dim requiredElementsData As New twksWSRequiredElementsDAO

        '                myGlobalDataTO = requiredElementsData.GetPatientElements(dbConnection, pWorkSessionID, pOrderTestDetailsRow)
        '                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '                    Dim requiredElementsDS As WSRequiredElementsDS = DirectCast(myGlobalDataTO.SetDatos, WSRequiredElementsDS)

        '                    'Add all Elements to the DataSet to return
        '                    Dim wsOrderTestElementsRow As WSElementsByOrderTestDS.twksWSRequiredElemByOrderTestRow
        '                    For i As Integer = 0 To requiredElementsDS.twksWSRequiredElements.Rows.Count - 1
        '                        wsOrderTestElementsRow = orderTestElementsDS.twksWSRequiredElemByOrderTest.NewtwksWSRequiredElemByOrderTestRow

        '                        wsOrderTestElementsRow.OrderTestID = pOrderTestDetailsRow.OrderTestID
        '                        wsOrderTestElementsRow.ElementID = requiredElementsDS.twksWSRequiredElements(i).ElementID
        '                        wsOrderTestElementsRow.StatFlag = pOrderTestDetailsRow.StatFlag

        '                        orderTestElementsDS.twksWSRequiredElemByOrderTest.Rows.Add(wsOrderTestElementsRow)
        '                    Next

        '                    'Only for Standard Tests, get the required Calibrators and Blanks
        '                    If (pOrderTestDetailsRow.TestType = "STD") Then
        '                        'Get Required Calibrators Elements
        '                        myGlobalDataTO = GetCalibratorElements(dbConnection, pWorkSessionID, pOrderTestDetailsRow)

        '                        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '                            Dim calibratorElementsDS As WSElementsByOrderTestDS = DirectCast(myGlobalDataTO.SetDatos, WSElementsByOrderTestDS)

        '                            'Add the obtained records to the DataSet to return 
        '                            For Each calibratorElementsDR As WSElementsByOrderTestDS.twksWSRequiredElemByOrderTestRow In calibratorElementsDS.twksWSRequiredElemByOrderTest.Rows
        '                                orderTestElementsDS.twksWSRequiredElemByOrderTest.ImportRow(calibratorElementsDR)
        '                            Next

        '                            myGlobalDataTO.HasError = False
        '                            myGlobalDataTO.SetDatos = orderTestElementsDS
        '                        End If
        '                    Else
        '                        myGlobalDataTO.HasError = False
        '                        myGlobalDataTO.SetDatos = orderTestElementsDS
        '                    End If
        '                End If
        '            End If
        '        End If
        '    Catch ex As Exception
        '        myGlobalDataTO.HasError = True
        '        myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myGlobalDataTO.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSRequiredElementsDelegate.GetPatientElements", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return myGlobalDataTO
        'End Function

        '''' <summary>
        '''' Get the list of Required Elements for an Order Test belonging to a Calibrator Order according its TestID/SampleType
        '''' </summary>
        '''' <param name="pDBConnection">Open Database Connection</param>
        '''' <param name="pWorkSessionID">Work Session Identifier</param>
        '''' <param name="pOrderTestDetailsRow">Row with structure of DataSet OrderTestsDetailsDS containing
        ''''                                    TestID and SampleType</param>
        '''' <returns>DataSet containing the list of Required Calibrator Elements for the specified Order Test</returns>
        '''' <remarks>
        '''' Created by:  SA
        '''' Modified by: SA 05/01/2010 - Changed the way of open the DB Connection to the new template; changes to return a 
        ''''                              GlobalDataTO instead a typed DataSet WSElementsByOrderTestDS   
        ''''              SA 30/08/2010 - If an Order Test corresponds to a Test marked as Special, verify if it has a special
        ''''                              Setting for the number of Calibrator points to use; in this case, only the required
        ''''                              Calibrator Element corresponding to that point will be link to the Order Test
        '''' </remarks>
        'Public Function GetCalibratorElements(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, _
        '                                      ByVal pOrderTestDetailsRow As OrderTestsDetailsDS.OrderTestsDetailsRow) As GlobalDataTO
        '    Dim myGlobalDataTO As New GlobalDataTO
        '    Dim dbConnection As New SqlClient.SqlConnection

        '    Try
        '        myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
        '        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '            dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                'Get Required Calibrator Elements
        '                Dim specifPointNum As Integer = -1
        '                If (pOrderTestDetailsRow.SpecialTest) Then
        '                    'The Test is marked as Special... search if it uses an specific Calibrator Point instead of all the points
        '                    'defined for it (for instance, case of Special Test HbTotal with WBL)
        '                    Dim mySpTestSettingsDelegate As New SpecialTestsSettingsDelegate
        '                    myGlobalDataTO = mySpTestSettingsDelegate.Read(dbConnection, pOrderTestDetailsRow.TestID, pOrderTestDetailsRow.SampleType, _
        '                                                                   GlobalEnumerates.SpecialTestsSettings.CAL_POINT_USED.ToString)

        '                    If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '                        Dim mySpTestsSettingsDS As SpecialTestsSettingsDS = DirectCast(myGlobalDataTO.SetDatos, SpecialTestsSettingsDS)

        '                        If (mySpTestsSettingsDS.tfmwSpecialTestsSettings.Rows.Count > 0) Then
        '                            'The Test has a special Setting for the number of Calibrator points to use.  Only the required
        '                            'Calibrator Element corresponding to that point will be link to the Order Test
        '                            specifPointNum = Convert.ToInt32(mySpTestsSettingsDS.tfmwSpecialTestsSettings(0).SettingValue)
        '                        End If
        '                    End If
        '                End If

        '                Dim requiredElementsData As New twksWSRequiredElementsDAO
        '                If (Not myGlobalDataTO.HasError) Then
        '                    myGlobalDataTO = requiredElementsData.GetCalibratorElements(dbConnection, pWorkSessionID, pOrderTestDetailsRow, specifPointNum)
        '                End If

        '                Dim requiredElementsDS As New WSElementsByOrderTestDS
        '                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '                    requiredElementsDS = DirectCast(myGlobalDataTO.SetDatos, WSElementsByOrderTestDS)

        '                    'If there are not Required Calibrator Elements for the Test/SampleType, verify if it is due to 
        '                    'the Calibrator is an Alternative one
        '                    If (requiredElementsDS.twksWSRequiredElemByOrderTest.Rows.Count = 0) Then
        '                        Dim testSampleData As New TestSamplesDelegate

        '                        myGlobalDataTO = testSampleData.GetDefinition(dbConnection, pOrderTestDetailsRow.TestID, pOrderTestDetailsRow.SampleType)
        '                        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '                            Dim testSampleDS As TestSamplesDS = DirectCast(myGlobalDataTO.SetDatos, TestSamplesDS)
        '                            If (testSampleDS.tparTestSamples.Rows.Count > 0) Then
        '                                If (testSampleDS.tparTestSamples(0).CalibratorType = "ALTERNATIV") Then
        '                                    specifPointNum = -1
        '                                    If (pOrderTestDetailsRow.SpecialTest) Then
        '                                        'The Test is marked as Special... search if for the Alternative SampleType it uses an specific 
        '                                        'Calibrator Point instead of all the points defined for it 
        '                                        Dim mySpTestSettingsDelegate As New SpecialTestsSettingsDelegate
        '                                        myGlobalDataTO = mySpTestSettingsDelegate.Read(dbConnection, pOrderTestDetailsRow.TestID, _
        '                                                                                       testSampleDS.tparTestSamples(0).SampleTypeAlternative, _
        '                                                                                       GlobalEnumerates.SpecialTestsSettings.CAL_POINT_USED.ToString)

        '                                        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '                                            Dim mySpTestsSettingsDS As SpecialTestsSettingsDS = DirectCast(myGlobalDataTO.SetDatos, SpecialTestsSettingsDS)

        '                                            If (mySpTestsSettingsDS.tfmwSpecialTestsSettings.Rows.Count > 0) Then
        '                                                'The Test with the Alternative SampleType has a special Setting for the number of Calibrator points to use.  
        '                                                'Only the required Calibrator Element corresponding to that point will be link to the Order Test
        '                                                specifPointNum = Convert.ToInt32(mySpTestsSettingsDS.tfmwSpecialTestsSettings(0).SettingValue)
        '                                            End If
        '                                        End If
        '                                    End If

        '                                    If (Not myGlobalDataTO.HasError) Then
        '                                        'Get Required Calibrator Elements for the Test and the Alternative Sample Type
        '                                        pOrderTestDetailsRow.SampleType = testSampleDS.tparTestSamples(0).SampleTypeAlternative

        '                                        myGlobalDataTO = requiredElementsData.GetCalibratorElements(dbConnection, pWorkSessionID, pOrderTestDetailsRow, specifPointNum)
        '                                        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '                                            requiredElementsDS = DirectCast(myGlobalDataTO.SetDatos, WSElementsByOrderTestDS)
        '                                        End If
        '                                    End If
        '                                End If
        '                            End If
        '                        End If
        '                    End If
        '                End If

        '                If (Not myGlobalDataTO.HasError) Then
        '                    'The identifier of the Order Test is included for all records of the returned DataSet
        '                    For i As Integer = 0 To requiredElementsDS.twksWSRequiredElemByOrderTest.Rows.Count - 1
        '                        requiredElementsDS.twksWSRequiredElemByOrderTest(i).BeginEdit()
        '                        requiredElementsDS.twksWSRequiredElemByOrderTest(i).OrderTestID = pOrderTestDetailsRow.OrderTestID
        '                        requiredElementsDS.twksWSRequiredElemByOrderTest(i).StatFlag = pOrderTestDetailsRow.StatFlag
        '                        requiredElementsDS.twksWSRequiredElemByOrderTest(i).EndEdit()
        '                    Next

        '                    'Get Required Blank Elements
        '                    myGlobalDataTO = GetBlankElements(dbConnection, pWorkSessionID, pOrderTestDetailsRow)
        '                    If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '                        Dim reagentElementsDS As WSElementsByOrderTestDS = DirectCast(myGlobalDataTO.SetDatos, WSElementsByOrderTestDS)

        '                        'Add the obtained records to the DataSet to return 
        '                        For Each reagentElementsDR As WSElementsByOrderTestDS.twksWSRequiredElemByOrderTestRow In reagentElementsDS.twksWSRequiredElemByOrderTest.Rows()
        '                            requiredElementsDS.twksWSRequiredElemByOrderTest.ImportRow(reagentElementsDR)
        '                        Next

        '                        myGlobalDataTO.HasError = False
        '                        myGlobalDataTO.SetDatos = requiredElementsDS
        '                    End If
        '                End If
        '            End If
        '        End If
        '    Catch ex As Exception
        '        myGlobalDataTO.HasError = True
        '        myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myGlobalDataTO.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSRequiredElementsDelegate.GetControlElements", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return myGlobalDataTO
        'End Function

        '''' <summary>
        '''' Get the list of Required Elements for an Order Test belonging to a Blank Order according its TestID
        '''' </summary>
        '''' <param name="pDBConnection">Open Database Connection</param>
        '''' <param name="pWorkSessionID">Work Session Identifier</param>
        '''' <param name="pOrderTestDetailsRow">Row with structure of DataSet OrderTestsDetailsDS containing
        ''''                                    TestID</param>
        '''' <returns>DataSet containing the list of Required Reagent Elements for the specified Order Test</returns>
        '''' <remarks>
        '''' Created by:  SA
        '''' Modified by: SA 05/01/2010 - Changed the way of open the DB Connection to the new template; changes to return a 
        ''''                              GlobalDataTO instead a typed DataSet WSElementsByOrderTestDS 
        ''''              RH 13/04/2010 - Added Get Required Special Solution Elements  
        '''' </remarks>
        'Public Function GetBlankElements(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, _
        '                                 ByVal pOrderTestDetailsRow As OrderTestsDetailsDS.OrderTestsDetailsRow) As GlobalDataTO
        '    Dim myGlobalDataTO As GlobalDataTO = Nothing
        '    Dim dbConnection As SqlClient.SqlConnection = Nothing

        '    Try
        '        myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
        '        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '            dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim requiredElementsDS As New WSElementsByOrderTestDS

        '                'Get Required Reagent Elements
        '                Dim requiredElementsData As New twksWSRequiredElementsDAO
        '                myGlobalDataTO = requiredElementsData.GetReagentElements(dbConnection, pWorkSessionID, pOrderTestDetailsRow)

        '                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '                    requiredElementsDS = DirectCast(myGlobalDataTO.SetDatos, WSElementsByOrderTestDS)

        '                    'The identifier of the Order Test is included for all records of the returned DataSet
        '                    For i As Integer = 0 To requiredElementsDS.twksWSRequiredElemByOrderTest.Rows.Count - 1
        '                        requiredElementsDS.twksWSRequiredElemByOrderTest(i).BeginEdit()
        '                        requiredElementsDS.twksWSRequiredElemByOrderTest(i).OrderTestID = pOrderTestDetailsRow.OrderTestID
        '                        requiredElementsDS.twksWSRequiredElemByOrderTest(i).StatFlag = pOrderTestDetailsRow.StatFlag
        '                        requiredElementsDS.twksWSRequiredElemByOrderTest(i).EndEdit()
        '                    Next
        '                End If

        '                If (Not myGlobalDataTO.HasError) Then
        '                    'Get Required Special Solution Elements
        '                    myGlobalDataTO = requiredElementsData.GetSpecialSolutionElements(dbConnection, pWorkSessionID, pOrderTestDetailsRow)
        '                    If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '                        Dim specsolElementsDS As WSElementsByOrderTestDS = DirectCast(myGlobalDataTO.SetDatos, WSElementsByOrderTestDS)

        '                        'The identifier of the Order Test is included for all records of the returned DataSet
        '                        For i As Integer = 0 To specsolElementsDS.twksWSRequiredElemByOrderTest.Rows.Count - 1
        '                            specsolElementsDS.twksWSRequiredElemByOrderTest(i).BeginEdit()
        '                            specsolElementsDS.twksWSRequiredElemByOrderTest(i).OrderTestID = pOrderTestDetailsRow.OrderTestID
        '                            specsolElementsDS.twksWSRequiredElemByOrderTest(i).StatFlag = pOrderTestDetailsRow.StatFlag
        '                            specsolElementsDS.twksWSRequiredElemByOrderTest(i).EndEdit()
        '                            requiredElementsDS.twksWSRequiredElemByOrderTest.ImportRow(specsolElementsDS.twksWSRequiredElemByOrderTest(i))
        '                        Next
        '                    End If

        '                    myGlobalDataTO.HasError = False
        '                    myGlobalDataTO.SetDatos = requiredElementsDS
        '                End If
        '            End If
        '        End If
        '    Catch ex As Exception
        '        myGlobalDataTO = New GlobalDataTO()
        '        myGlobalDataTO.HasError = True
        '        myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
        '        myGlobalDataTO.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSRequiredElementsDelegate.GetBlankElements", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return myGlobalDataTO
        'End Function
#End Region
    End Class
End Namespace
