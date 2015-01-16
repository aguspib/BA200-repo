Option Explicit On
Option Strict On

Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.DAL.DAO



Namespace Biosystems.Ax00.BL
    Public Class ControlsDelegate

#Region "TO DELETE - OLD FUNCTIONS"
        '''' <summary>
        '''' Create a new Control 
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pControlDS">Typed DataSet ControlsDS with data of the Control to add</param>
        '''' <param name="pTestControlsDS">Typed DataSet TestControlsDS with the list of Tests/SampleTypes to link to the new Control</param>
        '''' <param name="pControlScreen">Flag indicating if the function has been called from the screen of Controls Programming
        ''''                              Optional parameter</param>
        '''' <returns>GlobalDataTO containing a typed DataSet ControlsDS with all data of the added control
        ''''          or error information</returns>
        '''' <remarks>
        '''' Created by:  DL 31/03/2011
        '''' Modified by: SA 10/05/2011 - New implementation; the previous one was wrong
        '''' </remarks>
        'Public Function AddControlOLD(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pControlDS As ControlsDS, ByVal pTestControlsDS As TestControlsDS, _
        '                              Optional ByVal pControlScreen As Boolean = False) As GlobalDataTO
        '    Dim resultData As GlobalDataTO = Nothing
        '    Dim dbConnection As SqlClient.SqlConnection = Nothing

        '    Try
        '        resultData = DAOBase.GetOpenDBConnection(pDBConnection)
        '        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
        '            dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                'Insert the new control in tparControls
        '                Dim controlToAdd As New tparControlsDAO
        '                resultData = controlToAdd.CreateOLD(dbConnection, pControlDS)

        '                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
        '                    'Get the generated ControlID from the DS returned 
        '                    Dim generatedID As Integer = DirectCast(resultData.SetDatos, ControlsDS).tparControls(0).ControlID

        '                    'If there are Tests/SampleTypes to link to the Control, inform the generated ControlID in the DS and save them
        '                    If (Not pTestControlsDS Is Nothing AndAlso pTestControlsDS.tparTestControls.Rows.Count > 0) Then
        '                        For Each row As TestControlsDS.tparTestControlsRow In pTestControlsDS.tparTestControls.Rows
        '                            row.ControlID = generatedID
        '                        Next row
        '                        pTestControlsDS.AcceptChanges()

        '                        Dim myTestControls As New TestControlsDelegate
        '                        resultData = myTestControls.SaveTestControlsOLD(dbConnection, pTestControlsDS, Nothing, pControlScreen)
        '                    End If

        '                    If (Not resultData.HasError) Then
        '                        'When the Database Connection was opened locally, then the Commit is executed
        '                        If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)

        '                        'The added Control is returned 
        '                        resultData.SetDatos = pControlDS
        '                    Else
        '                        'When the Database Connection was opened locally, then the Rollback is executed
        '                        If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
        '                    End If
        '                End If
        '            End If
        '        End If
        '    Catch ex As Exception
        '        'When the Database Connection was opened locally, then the Rollback is executed
        '        If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

        '        resultData = New GlobalDataTO()
        '        resultData.HasError = True
        '        resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        resultData.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "ControlsDelegate.AddControl", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return resultData
        'End Function

        '''' <summary>
        '''' Modify an exiting control
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pControlDS">Typed DataSet ControlsDS with basic data of the Control to update</param>
        '''' <param name="pTestControlsDS">Typed DataSet TestControlsDS with the list of Tests/SampleTypes to link to the Control</param>
        '''' <param name="pDeletedTestControlsDS">Typed DataSet SelectedTestsDS with the list of Tests/SampleTypes to unlink from the Control</param>
        '''' <param name="pSaveCurrentLotAsPrevious">Flag indicating if the Lot has been changed and the current one has to be
        ''''                                         saved as previous before the update</param>
        '''' <param name="pChangedLot">When the Lot has been changed for a new one, this optional parameter contains the number of the Lot changed,
        ''''                           When informed, open QC Results for the changed Lot has to be cumulated and it has to be marked as Closed in 
        ''''                           the history table of QC Module</param>
        '''' <returns>GlobalDataTO containing success/error information</returns>
        '''' <remarks>
        '''' Created by:  DL 31/03/2011
        '''' Modified by: SA 12/05/2011 - New implementation; the previous one was wrong
        ''''              SA 20/05/2011 - When the Lot used for the Control is changed, all open QC Results that exist for it
        ''''                              has to be cumulated and the Lot has to be marked as Closed in the history table of 
        ''''                              Controls/Lots in QC Module
        '''' </remarks>
        'Public Function ModifyControlOLD(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pControlDS As ControlsDS, ByVal pTestControlsDS As TestControlsDS, _
        '                                 ByVal pDeletedTestControlsDS As SelectedTestsDS, ByVal pSaveCurrentLotAsPrevious As Boolean, _
        '                                 Optional ByVal pChangedLot As String = "") As GlobalDataTO
        '    Dim resultData As GlobalDataTO = Nothing
        '    Dim dbConnection As SqlClient.SqlConnection = Nothing

        '    Try
        '        resultData = DAOBase.GetOpenDBConnection(pDBConnection)
        '        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
        '            dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                If (pSaveCurrentLotAsPrevious) Then
        '                    'If there is a Previous Control Lot saved, delete it
        '                    Dim myPreviousControlLotDelegate As New PreviousControlLotsDelegate
        '                    resultData = myPreviousControlLotDelegate.DeleteOLD(dbConnection, pControlDS.tparControls(0).ControlID)

        '                    If (Not resultData.HasError) Then
        '                        'Save data of the current Control Lot as Previous
        '                        resultData = myPreviousControlLotDelegate.SavePreviousLotOLD(dbConnection, pControlDS.tparControls(0).ControlID)
        '                    End If
        '                End If

        '                If (Not resultData.HasError) Then
        '                    'Update basic Control data and also the list of Tests/SampleTypes linked to it
        '                    Dim controlToUpdate As New tparControlsDAO
        '                    resultData = controlToUpdate.Update(dbConnection, pControlDS)

        '                    If (Not resultData.HasError) Then
        '                        Dim myTestControls As New TestControlsDelegate
        '                        resultData = myTestControls.SaveTestControlsOLD(dbConnection, pTestControlsDS, pDeletedTestControlsDS, True)
        '                    End If
        '                End If

        '                Dim lotChanged As Boolean = (pChangedLot <> String.Empty)
        '                Dim myQCControlLotsDelegate As New HistoryControlLotsDelegate
        '                If (Not resultData.HasError) Then
        '                    'If there is information for the modified Control/Lot in the history tables of QC Module, update information also there
        '                    resultData = myQCControlLotsDelegate.UpdateControl(dbConnection, pControlDS, pTestControlsDS, pDeletedTestControlsDS, lotChanged)
        '                End If

        '                If (Not resultData.HasError) Then
        '                    If (lotChanged) Then
        '                        'If the Lot used for the Control has been changed, cumulate all open QC Results and mark the Lot as Closed
        '                        resultData = CumulateQCResultsByControl(dbConnection, pControlDS.tparControls(0).ControlID)
        '                        If (Not resultData.HasError) Then
        '                            'Update flag ClosedLot in QC history table for the Control/Lot
        '                            resultData = myQCControlLotsDelegate.CloseLotDeleteControl(dbConnection, pControlDS.tparControls(0).ControlID, pChangedLot)
        '                        End If
        '                    End If
        '                End If
        '            End If
        '        End If

        '        If (Not resultData.HasError) Then
        '            'When the Database Connection was opened locally, then the Commit is executed
        '            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
        '        Else
        '            'When the Database Connection was opened locally, then the Rollback is executed
        '            If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
        '        End If

        '    Catch ex As Exception
        '        'When the Database Connection was opened locally, then the Rollback is executed
        '        If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

        '        resultData = New GlobalDataTO()
        '        resultData.HasError = True
        '        resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        resultData.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "ControlsDelegate.ModifyControl", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return resultData
        'End Function
#End Region

#Region "Public Methods"
        ''' <summary>
        ''' Create a new Control 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pControlDS">Typed DataSet ControlsDS with data of the Control to add</param>
        ''' <param name="pTestControlsDS">Typed DataSet TestControlsDS with the list of Tests/SampleTypes to link to the new Control</param>
        ''' <param name="pControlScreen">Flag indicating if the function has been called from the screen of Controls Programming
        '''                              Optional parameter</param>
        ''' <returns>GlobalDataTO containing a typed DataSet ControlsDS with all data of the added control
        '''          or error information</returns>
        ''' <remarks>
        ''' Created by:  DL 31/03/2011
        ''' Modified by: SA 10/05/2011 - New implementation; the previous one was wrong
        ''' </remarks>
        Public Function AddControlNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pControlDS As ControlsDS, ByVal pTestControlsDS As TestControlsDS, _
                                      Optional ByVal pControlScreen As Boolean = False) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Insert the new control in tparControls
                        Dim controlToAdd As New tparControlsDAO
                        resultData = controlToAdd.Create(dbConnection, pControlDS)

                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            'Get the generated ControlID from the DS returned 
                            Dim generatedID As Integer = DirectCast(resultData.SetDatos, ControlsDS).tparControls(0).ControlID

                            'If there are Tests/SampleTypes to link to the Control, inform the generated ControlID in the DS and save them
                            If (Not pTestControlsDS Is Nothing AndAlso pTestControlsDS.tparTestControls.Rows.Count > 0) Then
                                For Each row As TestControlsDS.tparTestControlsRow In pTestControlsDS.tparTestControls.Rows
                                    row.ControlID = generatedID
                                Next row
                                pTestControlsDS.AcceptChanges()

                                Dim myTestControls As New TestControlsDelegate
                                resultData = myTestControls.SaveTestControlsNEW(dbConnection, pTestControlsDS, Nothing, pControlScreen)
                            End If

                            If (Not resultData.HasError) Then
                                'When the Database Connection was opened locally, then the Commit is executed
                                If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)

                                'The added Control is returned 
                                resultData.SetDatos = pControlDS
                            Else
                                'When the Database Connection was opened locally, then the Rollback is executed
                                If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                            End If
                        End If
                    End If
                End If
            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                GlobalBase.CreateLogActivity(ex.Message, "ControlsDelegate.AddControl", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Delete all the specified Controls 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pControlsDS">Typed DataSet ControlsDS with the list of Controls to delete</param>        
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by:  DL 31/03/2011
        ''' Modified by: SA 10/05/2011 - New implementation, the previous one was wrong
        '''              SA 20/05/2011 - Mark Controls as deleted in history table in QC Module and, for each Test/Sample Type
        '''                              linked to the Control, cumulate open QC results
        '''              DL 10/10/2012 - Delete tparVirtualRotorPositions by control identifier
        ''' </remarks>
        Public Function DeleteControl(ByVal pDBConnection As SqlClient.SqlConnection, _
                                      ByVal pControlsDS As ControlsDS, _
                                      ByVal pAnalyzerID As String, _
                                      ByVal pWorkSessionID As String) As GlobalDataTO

            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myControlsDAO As New tparControlsDAO
                        Dim myPreviousLot As New PreviousControlLotsDelegate
                        Dim myTestControlsDelegate As New TestControlsDelegate
                        Dim myHistoryControlLotsDelegate As New HistoryControlLotsDelegate
                        Dim myVRotorsDelegate As New VirtualRotorsDelegate

                        'Process each deleted Control
                        For Each myControl As ControlsDS.tparControlsRow In pControlsDS.tparControls.Rows
                            'DL 11/10/2012. Begin
                            'Delete positions in virtual rotor position and update positions in the rotor when this is not in use
                            Dim myVirtualRotorPosDelegate As New VirtualRotorsPositionsDelegate
                            Dim mySelectedElementInfo As New WSRotorContentByPositionDS
                            Dim myVirtualRotorPosDS As New VirtualRotorPosititionsDS

                            resultData = myVirtualRotorPosDelegate.GetPositionsByControlID(Nothing, myControl.ControlID)

                            If Not resultData.HasError Then
                                myVirtualRotorPosDS = DirectCast(resultData.SetDatos, VirtualRotorPosititionsDS)

                                For Each myvirtualRow As VirtualRotorPosititionsDS.tparVirtualRotorPosititionsRow In myVirtualRotorPosDS.tparVirtualRotorPosititions.Rows
                                    Dim myRCPRow As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow

                                    myRCPRow = mySelectedElementInfo.twksWSRotorContentByPosition.NewtwksWSRotorContentByPositionRow
                                    myRCPRow.ControlID = myvirtualRow.ControlID
                                    myRCPRow.AnalyzerID = pAnalyzerID
                                    myRCPRow.WorkSessionID = pWorkSessionID
                                    myRCPRow.CellNumber = myvirtualRow.CellNumber
                                    myRCPRow.RingNumber = myvirtualRow.RingNumber
                                    myRCPRow.RotorType = "SAMPLES"
                                    myRCPRow.TubeContent = "CTRL"
                                    mySelectedElementInfo.twksWSRotorContentByPosition.AddtwksWSRotorContentByPositionRow(myRCPRow)
                                Next myvirtualRow
                            End If

                            For Each myRotorContPosResetElem As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow In mySelectedElementInfo.twksWSRotorContentByPosition.Rows
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
                            Dim myWSRotorContentByPosDAO As New twksWSRotorContentByPositionDAO
                            resultData = myWSRotorContentByPosDAO.Update(dbConnection, mySelectedElementInfo)
                            If (resultData.HasError) Then Exit For

                            'Remove virtual rotor position
                            resultData = myVRotorsDelegate.DeleteByControlID(Nothing, myControl.ControlID, pAnalyzerID, pWorkSessionID)
                            If (resultData.HasError) Then Exit For
                            'DL 11/10/2012. End

                            'If there is information of a previous saved Lot for the Control, delete it
                            resultData = myPreviousLot.Delete(dbConnection, myControl.ControlID)
                            If (resultData.HasError) Then Exit For

                            'If there are Tests/SampleTypes linked to the Control, delete them
                            resultData = myTestControlsDelegate.DeleteByControlID(dbConnection, myControl.ControlID)
                            If (resultData.HasError) Then Exit For

                            'Remove the Control
                            resultData = myControlsDAO.Delete(dbConnection, myControl.ControlID)
                            If (resultData.HasError) Then Exit For

                            'If there are open QC Results, cumulate them
                            resultData = CumulateQCResultsByControl(dbConnection, myControl.ControlID)
                            If (resultData.HasError) Then Exit For

                            'Update flags DeletedControl and ClosedLot in QC history table for Control/Lots
                            resultData = myHistoryControlLotsDelegate.CloseLotDeleteControl(dbConnection, myControl.ControlID)
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
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ControlsDelegate.DeleteControl", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Verify if there is already a Control with the informed Name
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pControlName">Control Name to be validated</param>
        ''' <param name="pControlID">Control Identified. Optional parameter; when informed, it means the
        '''                          ControlID has to be excluded from the validation</param>
        ''' <returns>GlobalDataTO containing a boolean value: True if there is another Control with the same 
        '''          name; otherwise, False</returns>
        ''' <remarks>
        ''' Created by:  DL 14/04/2011
        ''' Modified by: SA 10/05/2011 - Call function ReadByControlName instead of ExistsControl.
        '''                              Parameter for the ControlID has to be optional
        ''' </remarks>
        Public Function ExistsControl(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pControlName As String, _
                                      Optional ByVal pControlId As Integer = -1) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim controlExist As New tparControlsDAO
                        resultData = controlExist.ReadByControlName(dbConnection, pControlName, pControlId)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ControlsDelegate.ExistsControl", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get the list of all defined Controls
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTO containing a typed DataSet ControlsDS with the list of all defined Controls</returns>
        ''' <remarks>
        ''' Created by:  DL 14/04/2011
        ''' </remarks>
        Public Function GetAll(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Get list of defined controls
                        Dim myControls As New tparControlsDAO
                        resultData = myControls.ReadAll(dbConnection)

                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            Dim myControlsDS As ControlsDS = DirectCast(resultData.SetDatos, ControlsDS)

                            'According value of InUse field for each Control, inform field IconPath with the name of the proper Icon 
                            For Each controlRow As ControlsDS.tparControlsRow In myControlsDS.tparControls.Rows
                                controlRow.BeginEdit()
                                controlRow.IconPath = IIf(controlRow.InUse, "INUSECTRL", "CTRL").ToString
                                controlRow.EndEdit()
                            Next
                        End If
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ControlsDelegate.GetAll", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get basic data of the specified Control
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pControlID">Control Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet ControlsDS with data of the informed Control</returns>
        ''' <remarks>
        ''' Created by:  SA 21/01/2010
        ''' </remarks>
        Public Function GetControlData(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pControlID As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim controlDataDAO As New tparControlsDAO
                        resultData = controlDataDAO.Read(dbConnection, pControlID)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ControlsDelegate.GetControlData", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get Name and LotNumber of the Control related with the specified OrderTestID
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pOrderTestID">Order Test Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet ControlsDS with data of the informed Control</returns>
        ''' <remarks>
        ''' Created by:  DL 24/10/2011
        ''' Modified by: SA 14/06/2012 - Removed parameter pTestID; it is not needed due to table twksOrderTests is filtered 
        '''                              by PK field (OrderTestID). Name changed from ReadByTestIDAndOrderTestID to ReadByOrderTestID
        ''' </remarks>
        Public Function GetByOrderTestID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOrderTestID As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim controlDataDAO As New tparControlsDAO
                        resultData = controlDataDAO.ReadByOrderTestID(dbConnection, pOrderTestID)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ControlsDelegate.GetByOrderTestID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Modify an exiting control
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pControlDS">Typed DataSet ControlsDS with basic data of the Control to update</param>
        ''' <param name="pTestControlsDS">Typed DataSet TestControlsDS with the list of Tests/SampleTypes to link to the Control</param>
        ''' <param name="pDeletedTestControlsDS">Typed DataSet SelectedTestsDS with the list of Tests/SampleTypes to unlink from the Control</param>
        ''' <param name="pSaveCurrentLotAsPrevious">Flag indicating if the Lot has been changed and the current one has to be
        '''                                         saved as previous before the update</param>
        ''' <param name="pChangedLot">When the Lot has been changed for a new one, this optional parameter contains the number of the Lot changed,
        '''                           When informed, open QC Results for the changed Lot has to be cumulated and it has to be marked as Closed in 
        '''                           the history table of QC Module</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  DL 31/03/2011
        ''' Modified by: SA 12/05/2011 - New implementation; the previous one was wrong
        '''              SA 20/05/2011 - When the Lot used for the Control is changed, all open QC Results that exist for it
        '''                              has to be cumulated and the Lot has to be marked as Closed in the history table of 
        '''                              Controls/Lots in QC Module
        ''' </remarks>
        Public Function ModifyControlNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pControlDS As ControlsDS, ByVal pTestControlsDS As TestControlsDS, _
                                         ByVal pDeletedTestControlsDS As SelectedTestsDS, ByVal pSaveCurrentLotAsPrevious As Boolean, _
                                         Optional ByVal pChangedLot As String = "") As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        If (pSaveCurrentLotAsPrevious) Then
                            'If there is a Previous Control Lot saved, delete it
                            Dim myPreviousControlLotDelegate As New PreviousControlLotsDelegate
                            resultData = myPreviousControlLotDelegate.Delete(dbConnection, pControlDS.tparControls(0).ControlID)

                            If (Not resultData.HasError) Then
                                'Save data of the current Control Lot as Previous
                                resultData = myPreviousControlLotDelegate.SavePreviousLotNEW(dbConnection, pControlDS.tparControls(0).ControlID)
                            End If
                        End If

                        If (Not resultData.HasError) Then
                            'Update basic Control data and also the list of Tests/SampleTypes linked to it
                            Dim controlToUpdate As New tparControlsDAO
                            resultData = controlToUpdate.Update(dbConnection, pControlDS)

                            If (Not resultData.HasError) Then
                                Dim myTestControls As New TestControlsDelegate
                                resultData = myTestControls.SaveTestControlsNEW(dbConnection, pTestControlsDS, pDeletedTestControlsDS, True)
                            End If
                        End If

                        Dim lotChanged As Boolean = (String.Compare(pChangedLot, String.Empty, False) <> 0)
                        Dim myQCControlLotsDelegate As New HistoryControlLotsDelegate

                        If (Not resultData.HasError) Then
                            'If there is information for the modified Control/Lot in the history tables of QC Module, update information also there
                            resultData = myQCControlLotsDelegate.UpdateControlNEW(dbConnection, pControlDS, pTestControlsDS, pDeletedTestControlsDS, lotChanged)
                        End If

                        If (Not resultData.HasError) Then
                            If (lotChanged) Then
                                'If the Lot used for the Control has been changed, cumulate all open QC Results and mark the Lot as Closed
                                resultData = CumulateQCResultsByControl(dbConnection, pControlDS.tparControls(0).ControlID)

                                If (Not resultData.HasError) Then
                                    'Update flag ClosedLot in QC history table for the Control/Lot
                                    resultData = myQCControlLotsDelegate.CloseLotDeleteControl(dbConnection, pControlDS.tparControls(0).ControlID, pChangedLot)
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

            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ControlsDelegate.ModifyControl", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()

            End Try

            Return resultData
        End Function

        ''' <summary>
        ''' Set value of flag InUse for all Controls added/removed from the Active WorkSession 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pFlag">Value of the InUse Flag to set</param>
        ''' <param name="pUpdateForExcluded">Optional parameter. When True, it means the InUse will be set to False
        '''                                  only for Controls that have been excluded from the active WorkSession</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  GDS 10/05/2010 
        ''' Modified by: SA  09/06/2010 - Added new optional parameter to reuse this method to set InUse=False for Controls 
        '''                               that have been excluded from the active WorkSession. Added parameter for the AnalyzerID  
        ''' </remarks>
        Public Function UpdateInUseFlag(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, _
                                        ByVal pAnalyzerID As String, ByVal pFlag As Boolean, _
                                        Optional ByVal pUpdateForExcluded As Boolean = False) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New tparControlsDAO
                        myGlobalDataTO = myDAO.UpdateInUseFlag(dbConnection, pWorkSessionID, pAnalyzerID, pFlag, pUpdateForExcluded)

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
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                myGlobalDataTO = New GlobalDataTO
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ControlsDelegate.UpdateInUseFlag", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Validate if there are QC results pending to cumulate for a Control that has been updated by changing the Lot used 
        ''' for it or by unlinking Tests/SampleTypes
        ''' </summary>
        ''' <param name="pControlID">Identifier of the updated Control</param>
        ''' <param name="pDeletedTestsDS">Typed DataSet SelectedTestsDS containing the list of Tests/SampleTypes that have 
        '''                               been unlinked for the informed Control. Optional parameter informed only when this
        '''                               function is called for ValidateDependenciesOnUpdate in Controls Programming screen</param>
        ''' <returns>GlobalDataTO containing a typed DataSet DependenciesElementsDS with the list of affected elements</returns>
        ''' <remarks>
        ''' Created by:  SA 24/05/2011
        ''' Modified by: SA 14/06/2012 - Get also the ICON for ISE Tests and load it in the DS of Affected Elements when there are 
        '''                              QC Results pending to cumulate for ISE Tests unlinked from the informed Control
        ''' </remarks>
        Public Function ValidatedDependenciesOnUpdate(ByVal pControlID As Integer, Optional ByVal pDeletedTestsDS As SelectedTestsDS = Nothing) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                Dim myDependeciesElementsDS As New DependenciesElementsDS
                Dim myDependenciesElementsRow As DependenciesElementsDS.DependenciesElementsRow

                'Get Icons for Preloaded and User-defined Standard Tests, and also for ISE Tests
                Dim preloadedDataConfig As New PreloadedMasterDataDelegate
                Dim imageTest As Byte() = preloadedDataConfig.GetIconImage("TESTICON")
                Dim imageUserTest As Byte() = preloadedDataConfig.GetIconImage("USERTEST")
                Dim imageISETest As Byte() = preloadedDataConfig.GetIconImage("TISE_SYS")

                'Get the Warning Message of QC Results Accumulation in the current Language 
                Dim myWarningMSG As String = String.Empty
                Dim currentLanguageGlobal As New GlobalBase
                Dim myMessageDelegate As New MessageDelegate()

                myGlobalDataTO = myMessageDelegate.GetMessageDescription(Nothing, GlobalEnumerates.Messages.CUMULATE_QCRESULTS.ToString, _
                                                                         GlobalBase.GetSessionInfo().ApplicationLanguage.Trim.ToString)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    Dim myMessagesDS As MessagesDS = DirectCast(myGlobalDataTO.SetDatos, MessagesDS)
                    If (myMessagesDS.tfmwMessages.Rows.Count > 0) Then myWarningMSG = myMessagesDS.tfmwMessages(0).MessageText
                End If

                'Search if there are QC Results pending to cumulate for the updated Control
                Dim myQCResultsDelegate As New QCResultsDelegate
                myGlobalDataTO = myQCResultsDelegate.SearchPendingResultsByControlNEW(Nothing, pControlID)

                If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                    Dim myHistoryQCInfoDS As HistoryQCInformationDS = DirectCast(myGlobalDataTO.SetDatos, HistoryQCInformationDS)

                    Dim addAsAffectedElem As Boolean
                    For Each qcResult As HistoryQCInformationDS.HistoryQCInfoTableRow In myHistoryQCInfoDS.HistoryQCInfoTable.Rows
                        addAsAffectedElem = True
                        If (Not pDeletedTestsDS Is Nothing) Then
                            'Search if the TestType/TestID/SampleType for the QCResult is included in the list of unlinked ones
                            Dim lstDeletedTest As List(Of SelectedTestsDS.SelectedTestTableRow)
                            lstDeletedTest = (From a As SelectedTestsDS.SelectedTestTableRow In pDeletedTestsDS.SelectedTestTable _
                                             Where String.Compare(a.TestType, qcResult.TestType, False) = 0 _
                                           AndAlso a.TestID = qcResult.TestID _
                                           AndAlso String.Compare(a.SampleType, qcResult.SampleType, False) = 0 _
                                            Select a).ToList()

                            addAsAffectedElem = (lstDeletedTest.Count > 0)
                        End If
                        If (addAsAffectedElem) Then
                            'Add the Test/SampleType to the DS of affected Elements
                            myDependenciesElementsRow = myDependeciesElementsDS.DependenciesElements.NewDependenciesElementsRow()
                            If (String.Compare(qcResult.TestType, "STD", False) = 0) Then
                                If (qcResult.PreloadedTest) Then
                                    myDependenciesElementsRow.Type = imageTest
                                Else
                                    myDependenciesElementsRow.Type = imageUserTest
                                End If
                            ElseIf (String.Compare(qcResult.TestType, "ISE", False) = 0) Then
                                myDependenciesElementsRow.Type = imageISETest
                            End If
                            myDependenciesElementsRow.Name = qcResult.TestName & " [" & qcResult.SampleType & "]"
                            myDependenciesElementsRow.FormProfileMember = myWarningMSG
                            myDependeciesElementsDS.DependenciesElements.AddDependenciesElementsRow(myDependenciesElementsRow)
                        End If
                    Next
                    myGlobalDataTO.SetDatos = myDependeciesElementsDS
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ControlsDelegate.ValidatedDependenciesOnUpdate", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Validate if there are QC results pending to cumulate for a group of Controls selected to be deleted
        ''' </summary>
        ''' <param name="pControlsDS">Typed DataSet ControlsDS containing the list of Controls to be deleted</param>
        ''' <returns>GlobalDataTO containing a typed DataSet DependenciesElementsDS with the list of affected elements</returns>
        ''' <remarks>
        ''' Created by:  SA 24/05/2011
        ''' Modified by: SA 14/06/2012 - Get also the ICON for ISE Tests and load it in the DS of Affected Elements when there are 
        '''                              QC Results pending to cumulate for ISE Tests linked to Controls selected to be deleted
        ''' </remarks>
        Public Function ValidateDependenciesOnDelete(ByVal pControlsDS As ControlsDS) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                Dim myDependeciesElementsDS As New DependenciesElementsDS

                'Get Icons for Preloaded and User-defined Standard Tests, and also for ISE Tests
                Dim preloadedDataConfig As New PreloadedMasterDataDelegate
                Dim imageTest As Byte() = preloadedDataConfig.GetIconImage("TESTICON")
                Dim imageUserTest As Byte() = preloadedDataConfig.GetIconImage("USERTEST")
                Dim imageISETest As Byte() = preloadedDataConfig.GetIconImage("TISE_SYS")

                'Get the Warning Message of QC Results Accumulation in the current Language 
                Dim myWarningMSG As String = String.Empty
                Dim currentLanguageGlobal As New GlobalBase
                Dim myMessageDelegate As New MessageDelegate()

                myGlobalDataTO = myMessageDelegate.GetMessageDescription(Nothing, GlobalEnumerates.Messages.CUMULATE_QCRESULTS.ToString, _
                                                                         GlobalBase.GetSessionInfo().ApplicationLanguage.Trim.ToString)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    Dim myMessagesDS As MessagesDS = DirectCast(myGlobalDataTO.SetDatos, MessagesDS)
                    If (myMessagesDS.tfmwMessages.Rows.Count > 0) Then myWarningMSG = myMessagesDS.tfmwMessages(0).MessageText
                End If

                'Build a string list with the ID of all Controls selected to be deleted
                Dim myControlIDList As String = ""
                For Each controlRow As ControlsDS.tparControlsRow In pControlsDS.tparControls.Rows
                    myControlIDList &= controlRow.ControlID & ","
                Next
                myControlIDList = Left(myControlIDList, myControlIDList.Length - 1)

                'Get all affected Tests/SampleTypes: all Tests/SampleTypes linked to the Controls selected for deletion
                Dim myTestControlsDelegate As New TestControlsDelegate
                myGlobalDataTO = myTestControlsDelegate.GetTestControlsByControlIDListNEW(Nothing, myControlIDList)

                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    Dim myTestControlsDS As TestControlsDS = DirectCast(myGlobalDataTO.SetDatos, TestControlsDS)

                    If (myTestControlsDS.tparTestControls.Rows.Count > 0) Then
                        Dim numCtrlsToDelete As Integer = pControlsDS.tparControls.Rows.Count
                        Dim myDependenciesElementsRow As DependenciesElementsDS.DependenciesElementsRow

                        'Const pos1 As Integer = 0
                        'Const pos2 As Integer = 0
                        Dim myTestID As Integer = 0
                        Dim myTestType As String = String.Empty
                        Dim mySampleType As String = String.Empty
                        Dim myQCResultsDelegate As New QCResultsDelegate

                        'Get all different TestTypes/Tests/SampleTypes and get all Controls for each one of them
                        Dim lstDiffTestSampleTypes As List(Of String) = (From a As TestControlsDS.tparTestControlsRow In myTestControlsDS.tparTestControls _
                                                                       Select String.Format("{0}|{1}|{2}", a.TestType, a.TestID, a.SampleType) Distinct).ToList
                        For Each testSample As String In lstDiffTestSampleTypes
                            'RH 20/06/2012 These lines raise an exception
                            'pos1 = testSample.IndexOf("|")
                            'pos2 = testSample.LastIndexOf("|")

                            'myTestType = testSample.Substring(0, pos1 - 1)
                            'myTestID = Convert.ToInt32(testSample.Substring(pos1 + 1, pos2 - 1))
                            'mySampleType = testSample.Substring(pos2 + 1)
                            'RH 20/06/2012 These lines raise an exception END

                            'RH 20/06/2012 New implementation
                            Dim myData() As String = testSample.Split(CChar("|"))
                            myTestType = myData(0)
                            myTestID = CInt(myData(1))
                            mySampleType = myData(2)
                            'RH 20/06/2012 New implementation END

                            Dim lstControls As List(Of TestControlsDS.tparTestControlsRow)
                            lstControls = (From b As TestControlsDS.tparTestControlsRow In myTestControlsDS.tparTestControls _
                                          Where String.Compare(b.TestType, myTestType, False) = 0 _
                                        AndAlso b.TestID = myTestID _
                                        AndAlso String.Compare(b.SampleType, mySampleType, False) = 0 _
                                         Select b).ToList

                            Dim additionalInformation As String = ""
                            For Each controlRow As TestControlsDS.tparTestControlsRow In lstControls
                                If (numCtrlsToDelete > 1) Then
                                    If (String.Compare(additionalInformation, "", False) <> 0) Then additionalInformation &= vbCrLf
                                    additionalInformation &= controlRow.ControlName & "   "
                                End If

                                'Search if there is a record for the Control/TestID/SampleType in the QC Results table 
                                myGlobalDataTO = myQCResultsDelegate.SearchPendingResultsByControlNEW(Nothing, controlRow.ControlID, controlRow.TestType, controlRow.TestID, controlRow.SampleType)
                                If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                                    Dim myHistoryQCInfoDS As HistoryQCInformationDS = DirectCast(myGlobalDataTO.SetDatos, HistoryQCInformationDS)
                                    If (myHistoryQCInfoDS.HistoryQCInfoTable.Rows.Count > 0) Then additionalInformation &= myWarningMSG
                                Else
                                    Exit For
                                End If
                            Next
                            If (myGlobalDataTO.HasError) Then Exit For

                            'Add data to DS of affected Elements
                            myDependenciesElementsRow = myDependeciesElementsDS.DependenciesElements.NewDependenciesElementsRow()
                            If (String.Compare(lstControls.First.TestType, "STD", False) = 0) Then
                                If (lstControls.First.PreloadedTest) Then
                                    myDependenciesElementsRow.Type = imageTest
                                Else
                                    myDependenciesElementsRow.Type = imageUserTest
                                End If
                            ElseIf (String.Compare(lstControls.First.TestType, "ISE", False) = 0) Then
                                myDependenciesElementsRow.Type = imageISETest
                            End If

                            myDependenciesElementsRow.Name = lstControls.First.TestName & " [" & lstControls.First.SampleType & "]"
                            myDependenciesElementsRow.FormProfileMember = additionalInformation
                            myDependeciesElementsDS.DependenciesElements.AddDependenciesElementsRow(myDependenciesElementsRow)
                        Next

                        If (Not myGlobalDataTO.HasError) Then myGlobalDataTO.SetDatos = myDependeciesElementsDS
                    Else
                        'Return an empty DS
                        myGlobalDataTO.SetDatos = myDependeciesElementsDS
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ControlsDelegate.ValidatedDependenciesOnDelete", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function
#End Region

#Region "Private Methods"
        ''' <summary>
        ''' For each TestType/Test/Sample Type linked to the specified Control and having open QC Results, cumulate all of them
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pControlID">Control Identifier</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 23/05/2011
        ''' Modified by: SA 20/12/2011 - After executing the Cumulation process, if the CalculationMode defined for the 
        '''                              Test/SampleType is STATISTIC, delete also the group of QC Results used to calculate
        '''                              the statistics values for the specified Control/Lot
        ''' </remarks>
        Private Function CumulateQCResultsByControl(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pControlID As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myHistControlLotsDelegate As New HistoryControlLotsDelegate
                        resultData = myHistControlLotsDelegate.GetLinkedTestsSampleTypesByControl(dbConnection, pControlID)

                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            Dim myHistTestControlLotsDS As HistoryTestControlLotsDS = DirectCast(resultData.SetDatos, HistoryTestControlLotsDS)

                            Dim myQCResultsDelegate As New QCResultsDelegate
                            Dim myCumResultsDelegate As New CumulatedResultsDelegate
                            For Each linkedTC As HistoryTestControlLotsDS.tqcHistoryTestControlLotsRow In myHistTestControlLotsDS.tqcHistoryTestControlLots.Rows
                                resultData = myCumResultsDelegate.SaveCumulateResultNEW(dbConnection, linkedTC.QCTestSampleID, linkedTC.QCControlLotID)
                                If (resultData.HasError) Then Exit For

                                If (String.Compare(linkedTC.CalculationMode, "STATISTIC", False) = 0) Then
                                    resultData = myQCResultsDelegate.DeleteStatisticResultsNEW(dbConnection, linkedTC.QCTestSampleID, linkedTC.QCControlLotID)
                                    If (resultData.HasError) Then Exit For
                                End If
                            Next
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

                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ControlsDelegate.CumulateQCResultsByControl", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function
#End Region
    End Class

End Namespace
