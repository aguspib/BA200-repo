Option Explicit On
Option Strict On

Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.DAL.DAO
Imports Biosystems.Ax00.BL

Namespace Biosystems.Ax00.BL

    Public Class SavedWSDelegate

#Region "Public Methods"
        ''' <summary>
        ''' Delete all the specified Work Sessions
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pSavedWSDS">Typed DataSet SavedWSDS containing the list of saved WS to delete</param>
        ''' <param name="pFromLIS">Optional parameter. When TRUE, call function in twksWSBarcodePositionsWithNoRequestsDAO to update 
        '''                        the LISStatus and MessageID fields of all incomplete Patient Samples with Barcode equal to one of 
        '''                        the SpecimenIDs in the Saved WS in process</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  GDS 26/05/2010
        ''' Modified by: SA  21/06/2010 - Changed the entry parameter for a DataSet to allow deleting several 
        '''                               saved Work Sessions
        '''              SA  29/05/2013 - Added optional parameter pFromLIS to indicate when the function has been called to delete
        '''                               LIS Saved WorkSessions. In that case, before executing the deletes, the LISStatus and MessageID
        '''                               fields of all incomplete Patient Samples with Barcode equal to one of the SpecimenIDs in the 
        '''                               Saved WS in process have to be updated to PENDING and PROCESSED respectively 
        '''              SA  17/07/2013 - Added call to new function GetSpecimensWithSeveralSampleTypes to obtain all SpecimenIDs in the SavedWS
        '''                               for which LIS has sent Test for more than one Sample Type. Create a string list with all returned SpecimenIDs
        '''                               and then call function UpdateMessageIDToProcessed
        ''' </remarks>
        Public Function Delete(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pSavedWSDS As SavedWSDS, Optional ByVal pFromLIS As Boolean = False) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim mySavedWSDAO As New tparSavedWSDAO
                        Dim mySpecimenIDList As String = String.Empty
                        Dim mySavedWSOrderTestsDS As New SavedWSOrderTestsDS
                        Dim mySavedWSOrderTestsDelegate As New SavedWSOrderTestsDelegate
                        Dim myBCPosWithNoRequestDAO As New twksWSBarcodePositionsWithNoRequestsDAO

                        For Each savedWS As SavedWSDS.tparSavedWSRow In pSavedWSDS.tparSavedWS.Rows
                            If (pFromLIS) Then
                                'From the Saved WS to delete, get all Specimens with Tests for more than one Sample Type
                                resultData = mySavedWSOrderTestsDelegate.GetSpecimensWithSeveralSampleTypes(dbConnection, savedWS.SavedWSID)
                                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                    mySavedWSOrderTestsDS = DirectCast(resultData.SetDatos, SavedWSOrderTestsDS)

                                    For Each specimenID As SavedWSOrderTestsDS.tparSavedWSOrderTestsRow In mySavedWSOrderTestsDS.tparSavedWSOrderTests.Rows
                                        If (mySpecimenIDList.Trim <> String.Empty) Then mySpecimenIDList &= ","
                                        mySpecimenIDList &= "'" & specimenID.SpecimenID & "'"
                                    Next

                                    'Update LISStatus for all incomplete Patient Samples with Barcode equal to one of the SpecimenIDs in the Saved WS.
                                    'For SpecimenIDs with several Sample Types in the Saved WS, update also field MessageID - NOTE: It is not possible 
                                    'to call the DELEGATE due to circular references between ParametersProgramming and WorkSessions projects
                                    resultData = myBCPosWithNoRequestDAO.UpdateMessageIDToProcessed(dbConnection, savedWS.SavedWSID, mySpecimenIDList)
                                End If
                            End If

                            If (Not resultData.HasError) Then
                                'Delete the content of the saved Work Session
                                resultData = mySavedWSOrderTestsDelegate.DeleteAll(dbConnection, savedWS.SavedWSID)
                            End If

                            If (Not resultData.HasError) Then
                                'Then delete the Saved Work Session
                                resultData = mySavedWSDAO.Delete(dbConnection, savedWS.SavedWSID)
                            End If

                            'If an error happens deleting the content of the saved WS or the saved WS, then the process finishes
                            If (resultData.HasError) Then Exit For
                        Next

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
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "SavedWSDelegate.Delete", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Delete the specified Saved WS if it is empty (it does not have any Order Test)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pSavedWSID">Saved Work Session ID</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 23/04/2013 
        ''' </remarks>
        Public Function DeleteEmptySavedWS(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pSavedWSID As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim mySavedWSDAO As New tparSavedWSDAO
                        resultData = mySavedWSDAO.DeleteEmptySavedWS(dbConnection, pSavedWSID)
                    End If
                End If
            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "SavedWSDelegate.DeleteEmptySavedWS", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get the list of Saved Work Sessions
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pFromLIS">Optional parameter to filter the returned Saved WorkSession to avoid shown the
        '''                        ones created for the Import from LIMS process (Files) or the LIS Orders Download process (ES)</param>
        ''' <returns>GlobalDataTO containing a typed DataSet SavedWSDS with the list of all Saved WorkSessions</returns>
        ''' <remarks>
        ''' Created by:  GDS 06/04/2010
        ''' Modified by: SA  13/09/2010 - Added new optional parameter pFromLIMS
        '''              DL  08/05/2013 - Added new optional parameter pAll
        '''              SA  14/05/2013 - Removed optional parameter pAll
        ''' </remarks>
        Public Function GetAll(ByVal pDBConnection As SqlClient.SqlConnection, Optional ByVal pFromLIS As Boolean = False) As GlobalDataTO

            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim mySavedWSDAO As New tparSavedWSDAO
                        resultData = mySavedWSDAO.ReadAll(dbConnection, pFromLIS)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "SavedWSDelegate.GetAll", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Verify if there is already a Saved Work Session from LIS
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTO containing a typed DataSet SavedWSDS with information of a WS for the specified Name</returns>
        ''' <remarks>
        ''' Created by:  DL 26/04/2013
        ''' Modified by: AG 13/05/201 - Renamed method from "ReadBySavedWSID" to "ReadLISSavedWS", also change DAO query
        ''' </remarks> 
        Public Function ReadLISSavedWS(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim mySavedWSDAO As New tparSavedWSDAO
                        resultData = mySavedWSDAO.ReadLISSavedWS(dbConnection)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "SavedWSDelegate.ReadLISSavedWS", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function


        ''' <summary>
        ''' Verify if there is already a Saved Work Session with the specified name 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pSavedWSName">Name of the Saved WS Order Tests</param>
        ''' <param name="pFromLIMS">Optional parameter to filter the returned Saved WorkSession to avoid include in 
        '''                         the verification the ones created for the Import from LIMS process</param>
        ''' <returns>GlobalDataTO containing a typed DataSet SavedWSDS with information of a WS for the specified Name</returns>
        ''' <remarks>
        ''' Created by:  GDS 06/04/2010
        ''' Modified by: SA  13/09/2010 - Added new optional parameter pFromLIMS
        '''              SA  19/10/2010 - Changed the return type to a GlobalDataTO
        ''' </remarks> 
        Public Function ExistsSavedWS(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pSavedWSName As String, _
                                      Optional ByVal pFromLIMS As Boolean = False) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim mySavedWSDAO As New tparSavedWSDAO
                        resultData = mySavedWSDAO.ReadBySavedWSName(dbConnection, pSavedWSName, pFromLIMS)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "SavedWSDelegate.ExistsSavedWS", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Create a new Saved WS or update the specified one
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pSavedWSName">Name given to the WS</param>
        ''' <param name="pWorkSessionResultDS">Typed DataSet WorkSessionResultDS containing the list of all Order Tests
        '''                                    (all Sample Classes) to include in the Saved WS to create/update. When the
        '''                                    Save is executed in an Import From LIMS process, value of this DS is Nothing</param>
        ''' <param name="pSavedWSID">If a Saved WS has to be updated, this parameter contains the Identifier;
        '''                          if not (value = -1), it means a new WS has to be created</param>
        ''' <param name="pFromLIMS">Optional parameter. When True, it indicates the Save is executed in an Import From LIMS process</param>
        ''' <param name="pSavedWSOrderTestsDS">Typed DataSet SavedWSOrderTestsDS containing the list of Order Tests to include
        '''                                    in the Saved WS; optional parameter informed only when the Saved WS is created in
        '''                                    an Import From LIMS process </param>
        ''' <returns>GlobalDataTO containing sucess/error information plus an integer value with the ID of the
        '''          Saved WS created or updated</returns>
        ''' <remarks>
        ''' Created by:  GDS 06/04/2010
        ''' Modified by: SA  26/05/2010 - For Patients, if the Test is Calculated, the TubeType is not informed; additionally 
        '''                               fields for the Test Profile should not be saved (same in A25) due to the Profile definition
        '''                               could has changed. For Calculated Test, the Formula text is saved.        
        '''              SA  13/09/2010 - Added new optional parameters pFromLIMS and pSavedWSOrderTestsDS. When pFromLIMS=True and
        '''                               there are records in pSavedWSOrderTestsDS, then the list of Order Tests for the Saved WS is
        '''                               obtained from this DS instead of from pWorkSessionResultDS. Additionally, the ID of the Saved
        '''                               WS created or updated is returned inside the SetDatos of the GlobalDataTO
        '''              SA  22/10/2010 - Inform fields TubeType and NumReplicates also for ISE Tests; besides, for Calculated Tests, 
        '''                               get the Test Formula from new field CalcTestFormula instead of from TestProfileName.   
        '''              SA  18/05/2012 - When the WS to Save has been imported from LIMS: 
        '''                                 ** When SampleClass is CTRL, field StatFlag is set to False, and SampleID is not informed. 
        '''                                 ** Removed assignation of NumReplicates for CALC and OFFS Tests due to import of these Test Types is not allowed  
        '''              SA  12/06/2012 - For Blanks, if TubeType is informed, save it.     
        '''              XB  25/04/2013 - Read required LIS data info to match it with every Control/Patient OrderTest requested from LIS
        '''              XB  28/08/2014 - Add new field Selected - BT #1868
        ''' </remarks>
        Public Function Save(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pSavedWSName As String, ByVal pWorkSessionResultDS As WorkSessionResultDS, _
                             ByVal pSavedWSID As Integer, Optional ByVal pFromLIMS As Boolean = False, _
                             Optional ByVal pSavedWSOrderTestsDS As SavedWSOrderTestsDS = Nothing) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError) AndAlso (Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'If the ID was not informed
                        If (pSavedWSID = -1) Then
                            Dim mySavedWSDS As New SavedWSDS
                            Dim mySavedWSRow As SavedWSDS.tparSavedWSRow

                            mySavedWSRow = mySavedWSDS.tparSavedWS.NewtparSavedWSRow
                            mySavedWSRow.SavedWSName = pSavedWSName
                            mySavedWSRow.FromLIMS = pFromLIMS

                            mySavedWSDS.tparSavedWS.Rows.Add(mySavedWSRow)
                            mySavedWSDS.AcceptChanges()

                            Dim mySavedWSDAO As New tparSavedWSDAO
                            resultData = mySavedWSDAO.Create(dbConnection, mySavedWSDS)
                            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                pSavedWSID = DirectCast(resultData.SetDatos, SavedWSDS).tparSavedWS(0).SavedWSID
                            End If
                        Else
                            'The content of the Saved WS will be Update, delete the current information contained in it
                            Dim mySavedWSOrderTestsDelegate As New SavedWSOrderTestsDelegate
                            resultData = mySavedWSOrderTestsDelegate.DeleteAll(dbConnection, pSavedWSID)
                        End If

                        If (Not resultData.HasError) Then
                            Dim mySavedWSOrderTestsDS As New SavedWSOrderTestsDS
                            Dim mySavedWSOrderTestRow As SavedWSOrderTestsDS.tparSavedWSOrderTestsRow

                            If (Not pFromLIMS) Then

                                ' XB 25/04/2013 - Read required LIS data info
                                Dim lisInfoDS As New OrderTestsLISInfoDS
                                Dim myOrderTestsLISInfoDAO As New twksOrderTestsLISInfoDAO
                                resultData = myOrderTestsLISInfoDAO.ReadAll(dbConnection)
                                If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                                    lisInfoDS = DirectCast(resultData.SetDatos, OrderTestsLISInfoDS)
                                End If
                                ' XB 25/04/2013

                                'Get the list of Order Tests to include in the Saved WS from the pWorkSessionResultDS
                                Dim mySavedWSOrderTestRow2 As SavedWSOrderTestsDS.tparSavedWSOrderTestsRow
                                For Each myBlankCalibratorsRow As WorkSessionResultDS.BlankCalibratorsRow In pWorkSessionResultDS.BlankCalibrators.Rows
                                    mySavedWSOrderTestRow = mySavedWSOrderTestsDS.tparSavedWSOrderTests.NewtparSavedWSOrderTestsRow

                                    mySavedWSOrderTestRow.SavedWSID = pSavedWSID
                                    mySavedWSOrderTestRow.SampleClass = myBlankCalibratorsRow.SampleClass
                                    mySavedWSOrderTestRow.StatFlag = False
                                    mySavedWSOrderTestRow.TestType = myBlankCalibratorsRow.TestType
                                    mySavedWSOrderTestRow.TestID = myBlankCalibratorsRow.TestID
                                    mySavedWSOrderTestRow.TestName = myBlankCalibratorsRow.TestName
                                    mySavedWSOrderTestRow.SampleType = myBlankCalibratorsRow.SampleType
                                    mySavedWSOrderTestRow.ReplicatesNumber = myBlankCalibratorsRow.NumReplicates
                                    mySavedWSOrderTestRow.CreationOrder = myBlankCalibratorsRow.CreationOrder

                                    ' XB 28/08/2014 -- BT #1868
                                    mySavedWSOrderTestRow.Selected = myBlankCalibratorsRow.Selected

                                    If (Not myBlankCalibratorsRow.IsTubeTypeNull) Then
                                        mySavedWSOrderTestRow.TubeType = myBlankCalibratorsRow.TubeType
                                    End If
                                    mySavedWSOrderTestsDS.tparSavedWSOrderTests.Rows.Add(mySavedWSOrderTestRow)

                                    If (myBlankCalibratorsRow.SampleClass = "CALIB") Then
                                        Dim arrSampleType() As String = Split(myBlankCalibratorsRow.RequestedSampleTypes.Trim)

                                        For Each sSampleType As String In arrSampleType
                                            If (sSampleType <> myBlankCalibratorsRow.SampleType) Then
                                                mySavedWSOrderTestRow2 = mySavedWSOrderTestsDS.tparSavedWSOrderTests.NewtparSavedWSOrderTestsRow

                                                mySavedWSOrderTestRow2.ItemArray = mySavedWSOrderTestRow.ItemArray
                                                mySavedWSOrderTestRow2.SampleType = sSampleType

                                                mySavedWSOrderTestsDS.tparSavedWSOrderTests.Rows.Add(mySavedWSOrderTestRow2)
                                            End If
                                        Next
                                    End If
                                Next

                                For Each myControlsRow As WorkSessionResultDS.ControlsRow In pWorkSessionResultDS.Controls.Rows
                                    mySavedWSOrderTestRow = mySavedWSOrderTestsDS.tparSavedWSOrderTests.NewtparSavedWSOrderTestsRow

                                    mySavedWSOrderTestRow.SavedWSID = pSavedWSID
                                    mySavedWSOrderTestRow.SampleClass = myControlsRow.SampleClass
                                    mySavedWSOrderTestRow.StatFlag = False
                                    mySavedWSOrderTestRow.TestType = myControlsRow.TestType
                                    mySavedWSOrderTestRow.TestID = myControlsRow.TestID
                                    mySavedWSOrderTestRow.TestName = myControlsRow.TestName
                                    mySavedWSOrderTestRow.SampleType = myControlsRow.SampleType
                                    mySavedWSOrderTestRow.TubeType = myControlsRow.TubeType
                                    mySavedWSOrderTestRow.ReplicatesNumber = myControlsRow.NumReplicates
                                    'mySavedWSOrderTestRow.ControlNumber = myControlsRow.ControlNumber
                                    mySavedWSOrderTestRow.ControlID = myControlsRow.ControlID
                                    mySavedWSOrderTestRow.CreationOrder = myControlsRow.CreationOrder

                                    ' XB 28/08/2014 -- BT #1868
                                    mySavedWSOrderTestRow.Selected = myControlsRow.Selected

                                    ' XB 25/04/2013 - Match with the LIS data information for the corresponding OrderTestID
                                    If myControlsRow.LISRequest Then
                                        Dim lisInfoLinqRes As List(Of OrderTestsLISInfoDS.twksOrderTestsLISInfoRow)
                                        lisInfoLinqRes = (From a As OrderTestsLISInfoDS.twksOrderTestsLISInfoRow In lisInfoDS.twksOrderTestsLISInfo _
                                                    Where a.OrderTestID = myControlsRow.OrderTestID _
                                                    Select a).ToList
                                        If lisInfoLinqRes.Count > 0 Then
                                            If Not lisInfoLinqRes(0).IsAwosIDNull Then mySavedWSOrderTestRow.AwosID = lisInfoLinqRes(0).AwosID
                                            If Not lisInfoLinqRes(0).IsSpecimenIDNull Then mySavedWSOrderTestRow.SpecimenID = lisInfoLinqRes(0).SpecimenID
                                            If Not lisInfoLinqRes(0).IsESOrderIDNull Then mySavedWSOrderTestRow.ESOrderID = lisInfoLinqRes(0).ESOrderID
                                            If Not lisInfoLinqRes(0).IsLISOrderIDNull Then mySavedWSOrderTestRow.LISOrderID = lisInfoLinqRes(0).LISOrderID
                                            If Not lisInfoLinqRes(0).IsESPatientIDNull Then mySavedWSOrderTestRow.ESPatientID = lisInfoLinqRes(0).ESPatientID
                                            If Not lisInfoLinqRes(0).IsLISPatientIDNull Then mySavedWSOrderTestRow.LISPatientID = lisInfoLinqRes(0).LISPatientID
                                        End If
                                    End If
                                    ' XB 25/04/2013

                                    mySavedWSOrderTestsDS.tparSavedWSOrderTests.Rows.Add(mySavedWSOrderTestRow)
                                Next

                                For Each myPatientsRow As WorkSessionResultDS.PatientsRow In pWorkSessionResultDS.Patients.Rows
                                    mySavedWSOrderTestRow = mySavedWSOrderTestsDS.tparSavedWSOrderTests.NewtparSavedWSOrderTestsRow

                                    mySavedWSOrderTestRow.SavedWSID = pSavedWSID
                                    mySavedWSOrderTestRow.SampleClass = myPatientsRow.SampleClass
                                    mySavedWSOrderTestRow.StatFlag = myPatientsRow.StatFlag
                                    mySavedWSOrderTestRow.SampleID = myPatientsRow.SampleID
                                    mySavedWSOrderTestRow.TestType = myPatientsRow.TestType
                                    mySavedWSOrderTestRow.TestID = myPatientsRow.TestID
                                    mySavedWSOrderTestRow.TestName = myPatientsRow.TestName
                                    mySavedWSOrderTestRow.SampleType = myPatientsRow.SampleType
                                    mySavedWSOrderTestRow.CreationOrder = myPatientsRow.CreationOrder

                                    ' XB 28/08/2014 -- BT #1868
                                    mySavedWSOrderTestRow.Selected = myPatientsRow.Selected

                                    If (myPatientsRow.TestType = "STD" Or myPatientsRow.TestType = "ISE") Then
                                        mySavedWSOrderTestRow.TubeType = myPatientsRow.TubeType
                                        mySavedWSOrderTestRow.ReplicatesNumber = myPatientsRow.NumReplicates
                                    ElseIf (myPatientsRow.TestType = "CALC") Then
                                        mySavedWSOrderTestRow.ReplicatesNumber = 1
                                        mySavedWSOrderTestRow.FormulaText = myPatientsRow.CalcTestFormula.Substring(1)
                                    ElseIf (myPatientsRow.TestType = "OFFS") Then
                                        mySavedWSOrderTestRow.ReplicatesNumber = 1
                                    End If

                                    ' XB 25/04/2013 - Match with the LIS data information for the corresponding OrderTestID
                                    If myPatientsRow.LISRequest Then
                                        If Not myPatientsRow.IsExternalQCNull Then mySavedWSOrderTestRow.ExternalQC = myPatientsRow.ExternalQC
                                        Dim lisInfoLinqRes As List(Of OrderTestsLISInfoDS.twksOrderTestsLISInfoRow)
                                        lisInfoLinqRes = (From a As OrderTestsLISInfoDS.twksOrderTestsLISInfoRow In lisInfoDS.twksOrderTestsLISInfo _
                                                    Where a.OrderTestID = myPatientsRow.OrderTestID _
                                                    Select a).ToList
                                        If lisInfoLinqRes.Count > 0 Then
                                            If Not lisInfoLinqRes(0).IsAwosIDNull Then mySavedWSOrderTestRow.AwosID = lisInfoLinqRes(0).AwosID
                                            If Not lisInfoLinqRes(0).IsSpecimenIDNull Then mySavedWSOrderTestRow.SpecimenID = lisInfoLinqRes(0).SpecimenID
                                            If Not lisInfoLinqRes(0).IsESOrderIDNull Then mySavedWSOrderTestRow.ESOrderID = lisInfoLinqRes(0).ESOrderID
                                            If Not lisInfoLinqRes(0).IsLISOrderIDNull Then mySavedWSOrderTestRow.LISOrderID = lisInfoLinqRes(0).LISOrderID
                                            If Not lisInfoLinqRes(0).IsESPatientIDNull Then mySavedWSOrderTestRow.ESPatientID = lisInfoLinqRes(0).ESPatientID
                                            If Not lisInfoLinqRes(0).IsLISPatientIDNull Then mySavedWSOrderTestRow.LISPatientID = lisInfoLinqRes(0).LISPatientID
                                        End If
                                    End If
                                    ' XB 25/04/2013

                                    mySavedWSOrderTestsDS.tparSavedWSOrderTests.Rows.Add(mySavedWSOrderTestRow)
                                Next
                                mySavedWSOrderTestsDS.AcceptChanges()
                            Else
                                'Get the list of Order Tests to include in the Saved WS from the optional DS pSavedWSOrderTestsDS
                                'This means the Saved WS is created during an Import from LIMS process
                                For Each myOrderTestRow As SavedWSOrderTestsDS.tparSavedWSOrderTestsRow In pSavedWSOrderTestsDS.tparSavedWSOrderTests.Rows
                                    mySavedWSOrderTestRow = mySavedWSOrderTestsDS.tparSavedWSOrderTests.NewtparSavedWSOrderTestsRow

                                    mySavedWSOrderTestRow.SavedWSID = pSavedWSID
                                    mySavedWSOrderTestRow.SampleClass = myOrderTestRow.SampleClass

                                    'StatFlag and SampleID are informed only for Patient Samples
                                    mySavedWSOrderTestRow.StatFlag = False
                                    If (Not myOrderTestRow.IsStatFlagNull) Then mySavedWSOrderTestRow.StatFlag = myOrderTestRow.StatFlag
                                    If (Not myOrderTestRow.IsSampleIDNull) Then mySavedWSOrderTestRow.SampleID = myOrderTestRow.SampleID

                                    mySavedWSOrderTestRow.TestType = myOrderTestRow.TestType
                                    mySavedWSOrderTestRow.TestID = myOrderTestRow.TestID
                                    mySavedWSOrderTestRow.TestName = myOrderTestRow.TestName
                                    mySavedWSOrderTestRow.SampleType = myOrderTestRow.SampleType
                                    mySavedWSOrderTestRow.CreationOrder = myOrderTestRow.CreationOrder

                                    If (myOrderTestRow.TestType = "STD" Or myOrderTestRow.TestType = "ISE") Then
                                        mySavedWSOrderTestRow.TubeType = myOrderTestRow.TubeType
                                        mySavedWSOrderTestRow.ReplicatesNumber = myOrderTestRow.ReplicatesNumber
                                        'ElseIf (myOrderTestRow.TestType = "CALC") Then
                                        '    mySavedWSOrderTestRow.ReplicatesNumber = 1
                                        '    mySavedWSOrderTestRow.FormulaText = myOrderTestRow.FormulaText
                                        'ElseIf (myOrderTestRow.TestType = "OFFS") Then
                                        '    mySavedWSOrderTestRow.ReplicatesNumber = 1
                                    End If

                                    mySavedWSOrderTestsDS.tparSavedWSOrderTests.Rows.Add(mySavedWSOrderTestRow)
                                Next
                                mySavedWSOrderTestsDS.AcceptChanges()
                            End If

                            Dim mySavedWSOrderTestsDelegate As New SavedWSOrderTestsDelegate
                            resultData = mySavedWSOrderTestsDelegate.AddOrderTestsToSavedWS(dbConnection, mySavedWSOrderTestsDS)
                        End If

                        If (Not resultData.HasError) Then
                            'When the Database Connection was opened locally, then the Commit is executed
                            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)

                            'Return the ID of the SavedWS inside the SetDatos of the GlobalDataTO
                            resultData.SetDatos = pSavedWSID
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
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "SavedWSDelegate.Save", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Create a new SavedWS from a processed XML Message sent by LIS, and insert all accepted AwosID in the XML as Order Tests.
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pSavedWSName">Message Identifier, which will be used as name of the SavedWS to create</param>
        ''' <param name="pSavedWSOrderTestsDS">Typed DS SavedWSOrderTestsDS containing all information of the Order Tests to add to the Saved WS</param>
        ''' <param name="pMsgDateTime">Date and Time LIS sent the Message from which the Orders Test to save were extracted</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  TR 14/03/2013
        ''' Modified by: SA 01/04/2014 - BT #1564 ==> Added new parameter for the Message Date and Time, and inform it in field TS_DateTime when fill the SavedWSDS
        ''' </remarks>
        Public Function SaveFromLIS(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pSavedWSName As String, ByVal pSavedWSOrderTestsDS As SavedWSOrderTestsDS, _
                                    ByVal pMsgDateTime As Date) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError) AndAlso (Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim mySavedWSID As Integer
                        Dim mySavedWSDS As New SavedWSDS
                        Dim mySavedWSRow As SavedWSDS.tparSavedWSRow
                        'Create a new row for savedWSDS
                        mySavedWSRow = mySavedWSDS.tparSavedWS.NewtparSavedWSRow
                        mySavedWSRow.SavedWSName = pSavedWSName
                        mySavedWSRow.FromLIMS = True
                        mySavedWSRow.TS_DateTime = pMsgDateTime 'BT #1564 - Date Time in which LIS sent the Message
                        mySavedWSDS.tparSavedWS.AddtparSavedWSRow(mySavedWSRow)
                        mySavedWSDS.AcceptChanges()

                        'Create the new Saved WS
                        Dim mySavedWSDAO As New tparSavedWSDAO
                        myGlobalDataTO = mySavedWSDAO.Create(dbConnection, mySavedWSDS)
                        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                            mySavedWSID = DirectCast(myGlobalDataTO.SetDatos, SavedWSDS).tparSavedWS(0).SavedWSID

                            'Add OrderTests to the created Saved WS
                            Dim mySavedWSOrderTestsDelegate As New SavedWSOrderTestsDelegate
                            myGlobalDataTO = mySavedWSOrderTestsDelegate.AddOrderTestsToSavedWS(dbConnection, pSavedWSOrderTestsDS, mySavedWSID)
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
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "SavedWSDelegate.SaveFromLIS", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO

        End Function

#End Region
    End Class
End Namespace
