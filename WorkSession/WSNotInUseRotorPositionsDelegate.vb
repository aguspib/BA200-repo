Option Strict On
Option Explicit On

Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global
'Imports Biosystems.Ax00.Global.TO
Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.DAL.DAO
Imports Biosystems.Ax00.Global.GlobalEnumerates

Namespace Biosystems.Ax00.BL

    Public Class WSNotInUseRotorPositionsDelegate

#Region "Public Enumerates"
        ''' <summary>
        ''' This enumerate informs the process who is saving the information about the not in use positions
        ''' By now it is only use for ADD items, not for delete
        ''' </summary>
        ''' <remarks>AG 07/10/2014 BA-1979</remarks>
        Public Enum ClassCalledFrom
            BarcodeResultsManagement
            ChangeElementPosition
            LoadVirtualRotor
        End Enum

#End Region


#Region "Public Methods"
        ''' <summary>
        ''' Add the additional information of each Element placed in a Rotor Position with Status Not In Use in the specified Work Session
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pRotorType">Rotor Type</param>
        ''' <param name="pVirtualRotorPos">Typed DataSet containing the information to add for a Rotor Position with Status = NOT IN USE</param>        
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by:  AG 21/01/2010 (Tested OK)
        ''' AG 07/10/2014 - BA-1979 add traces into log when NO inuse rotor is saved with invalid values in order to find the origin
        ''' </remarks>
        Public Function Add(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pRotorType As String, _
                            ByVal pWorkSessionID As String, ByVal pVirtualRotorPos As VirtualRotorPosititionsDS, ByVal pProcessWhoCalls As ClassCalledFrom) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'AG 07/10/2014 BA-1979
                        resultData = CheckForInvalidPosition(dbConnection, pRotorType, pVirtualRotorPos, pProcessWhoCalls)
                        If Not resultData.HasError Then
                            'AG 07/10/2014 BA-1979

                            Dim myDAO As New twksWSNotInUseRotorPositionsDAO
                            resultData = myDAO.Create(dbConnection, pAnalyzerID, pWorkSessionID, pRotorType, pVirtualRotorPos)

                        End If 'AG 07/10/2014 BA-1979


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
                myLogAcciones.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSNotInUseRotorPositionsDelegate.Add", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Return the complete group of Not In Use positions to clear (or delete) in an Analyzer Rotor when a position corresponding 
        ''' to a MultiPoint Calibrator (all tubes in the Calibrator kit have to be downloaded) or a Patient Sample with dilutions 
        ''' (all tubes of the Patient Sample have to be downloaded) is selected to be cleared.
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pRotorContents">Row of a typed DataSet WSRotorContentByPositionsDS containing the Not In Use Rotor 
        '''                              position that has to be deleted</param>
        ''' <param name="pWarningCodeFlag">By Reference parameter to set the code of the warning message that will be shown to the application 
        '''                                User when more positions than the selected have to be emptied in the Rotor (case of multipoint 
        '''                                Calibrators and Patient Samples with dilutions)</param>
        ''' <returns>GlobalDataTO containing a typed DataSet VirtualRotorPositionsDS with the same entry information plus additional 
        '''          positions found for Multipoint Calibrators and/or Patient Sample Dilutions</returns>
        ''' <remarks>
        ''' Created by:  AG 22/01/2010
        ''' </remarks>
        Public Function CompleteSelection(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pRotorContents As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow, _
                                          ByRef pWarningCodeFlag As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'If there are not related positions or if an error happens, then return the same entry parameter...
                        Dim rotorDS As New WSRotorContentByPositionDS
                        rotorDS.twksWSRotorContentByPosition.ImportRow(pRotorContents)
                        resultData.SetDatos = rotorDS

                        'Complete selection is possible only for Calibrators (when they have multiple points) and Patients (when they have
                        'manual predilutions) - In these cases, read the cell information in table of Not In Use positions
                        If (pRotorContents.TubeContent = "CALIB" OrElse pRotorContents.TubeContent = "PATIENT") Then
                            Dim tempGlobalTo As GlobalDataTO = Nothing
                            Dim myDAO As New twksWSNotInUseRotorPositionsDAO

                            tempGlobalTo = myDAO.Read(dbConnection, pRotorContents.AnalyzerID, pRotorContents.WorkSessionID, pRotorContents.RotorType, _
                                                      pRotorContents.RingNumber, pRotorContents.CellNumber)

                            If (Not tempGlobalTo.HasError AndAlso Not tempGlobalTo.SetDatos Is Nothing) Then
                                Dim virtualPos As VirtualRotorPosititionsDS = DirectCast(tempGlobalTo.SetDatos, VirtualRotorPosititionsDS)

                                If (virtualPos.tparVirtualRotorPosititions.Rows.Count > 0) Then
                                    Select Case (pRotorContents.TubeContent)
                                        Case "CALIB"
                                            'Search cells where the rest of Calibrator points are placed 
                                            tempGlobalTo = myDAO.ReadPositionsByIdentificator(dbConnection, pRotorContents.AnalyzerID, pRotorContents.WorkSessionID, _
                                                                                              pRotorContents.RotorType, pRotorContents.TubeContent, _
                                                                                              virtualPos.tparVirtualRotorPosititions(0).CalibratorID, "", _
                                                                                              virtualPos.tparVirtualRotorPosititions(0).SampleType, False)
                                        Case "PATIENT"
                                            'Only if the selected cell is a full Patient Sample, search cells where Sample Dilutions are placed 
                                            If (virtualPos.tparVirtualRotorPosititions(0).IsPredilutionFactorNull) Then
                                                If (Not virtualPos.tparVirtualRotorPosititions(0).IsPatientIDNull) Then
                                                    'Search by PatientID
                                                    tempGlobalTo = myDAO.ReadPositionsByIdentificator(dbConnection, pRotorContents.AnalyzerID, pRotorContents.WorkSessionID, _
                                                                                                      pRotorContents.RotorType, pRotorContents.TubeContent, _
                                                                                                      0, virtualPos.tparVirtualRotorPosititions(0).PatientID, _
                                                                                                      virtualPos.tparVirtualRotorPosititions(0).SampleType, True)
                                                Else
                                                    'Search by OrderID
                                                    tempGlobalTo = myDAO.ReadPositionsByIdentificator(dbConnection, pRotorContents.AnalyzerID, pRotorContents.WorkSessionID, _
                                                                                                      pRotorContents.RotorType, pRotorContents.TubeContent, _
                                                                                                      0, virtualPos.tparVirtualRotorPosititions(0).OrderID, _
                                                                                                      virtualPos.tparVirtualRotorPosititions(0).SampleType, False)
                                                End If
                                            End If
                                    End Select

                                    If (Not tempGlobalTo.HasError AndAlso Not tempGlobalTo.SetDatos Is Nothing) Then
                                        virtualPos = DirectCast(tempGlobalTo.SetDatos, VirtualRotorPosititionsDS)

                                        'If number of rows is > 1, then there are related positions that have to be also selected
                                        If (virtualPos.tparVirtualRotorPosititions.Rows.Count > 1) Then
                                            pWarningCodeFlag = "FULL_KIT_DELETION"

                                            Dim rotorDlg As New WSRotorContentByPositionDelegate
                                            Dim newRotorContents As New WSRotorContentByPositionDS

                                            'Get information of all positions in Samples Rotor
                                            tempGlobalTo = rotorDlg.GetRotorContentPositions(dbConnection, pRotorContents.WorkSessionID, pRotorContents.AnalyzerID)
                                            If (Not tempGlobalTo.HasError AndAlso Not tempGlobalTo.SetDatos Is Nothing) Then
                                                Dim completeRotorContents As WSRotorContentByPositionDS = DirectCast(tempGlobalTo.SetDatos, WSRotorContentByPositionDS)

                                                Dim myRingNumber As Integer
                                                Dim myCellNumber As Integer
                                                Dim myNoInUsePosition As List(Of WSRotorContentByPositionDS.twksWSRotorContentByPositionRow)

                                                'Search each one of the NOT IN USE positions selected in Samples Rotor and copy the position information 
                                                'to the final DataSet to return
                                                For Each row As VirtualRotorPosititionsDS.tparVirtualRotorPosititionsRow In virtualPos.tparVirtualRotorPosititions
                                                    myRingNumber = row.RingNumber
                                                    myCellNumber = row.CellNumber

                                                    myNoInUsePosition = (From e As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow In completeRotorContents.twksWSRotorContentByPosition _
                                                                        Where e.Status = "NO_INUSE" _
                                                                      AndAlso e.RingNumber = myRingNumber _
                                                                      AndAlso e.CellNumber = myCellNumber _
                                                                      AndAlso e.RotorType = pRotorContents.RotorType _
                                                                       Select e).ToList()

                                                    For Each RotorRow As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow In myNoInUsePosition
                                                        newRotorContents.twksWSRotorContentByPosition.ImportRow(RotorRow)
                                                    Next
                                                Next
                                                myNoInUsePosition = Nothing

                                                resultData.SetDatos = newRotorContents
                                                resultData.HasError = False
                                            End If
                                        End If
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
                myLogAcciones.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSNotInUseRotorPositionsDelegate.CompleteSelection", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Delete the additional information of the Element placed in a Rotor Position with Status Not In Use (due to the Element was physically 
        ''' removed from the Rotor Position of was moved to other Position)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pRotorPositions">Typed DataSet WSRotorContentByPositionDS containing the the list of Not In Use positions to be deleted</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by: AG 21/01/2010 (Tested OK)
        ''' </remarks>
        Public Function Delete(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pRotorPositions As WSRotorContentByPositionDS) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksWSNotInUseRotorPositionsDAO
                        For Each RotorRow In pRotorPositions.twksWSRotorContentByPosition
                            resultData = myDAO.Delete(dbConnection, RotorRow.AnalyzerID, RotorRow.WorkSessionID, RotorRow.RotorType, RotorRow.RingNumber, RotorRow.CellNumber)
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
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSNotInUseRotorPositionsDelegate.Delete", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Delete the specified control identifier
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pControlID">Typed DataSet VirtualRotorsDS containing the list of Virtual Rotors to delete</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by:  DL 10/10/2012
        ''' </remarks>
        Public Function DeleteByControlID(ByVal pDbConnection As SqlClient.SqlConnection, ByVal pControlID As Integer) As GlobalDataTO

            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDbConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myNotInUseRotorPositionsDAO As New twksWSNotInUseRotorPositionsDAO
                        resultData = myNotInUseRotorPositionsDAO.DeleteControl(dbConnection, pControlID, "", "")

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
                If (pDbConnection Is Nothing) And (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSNotInUseRotorPositionsDelegate.DeleteByControlID", EventLogEntryType.Error, False)
            Finally
                If (pDbConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get the Number of Bottles Not In Use in Reagents Rotor in the current WorkSession
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pRotorType">Rotor Type</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pReagentID">Reagent Identifier</param>
        ''' <param name="pSolutionCode">Additional Solution Code</param>
        ''' <returns>GlobalDataTO containing an Integer value with the total number of bottles in the Rotor</returns>
        ''' <remarks>
        ''' Created by:  AG 30/08/2011
        ''' </remarks>
        Public Function GetPlacedTubesByPosition(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pRotorType As String, _
                                                 ByVal pWorkSessionID As String, ByVal pReagentID As Integer, ByVal pSolutionCode As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksWSNotInUseRotorPositionsDAO
                        resultData = myDAO.GetPlacedTubesByPosition(dbConnection, pAnalyzerID, pRotorType, pWorkSessionID, pReagentID, pSolutionCode)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSNotInUseRotorPositionsDelegate.GetPlacedTubesByPosition", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get a position containing a Not In Use Element in a Rotor used in a Work Session
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pRotorType">Rotor Type</param>
        ''' <param name="pRingNumber">Ring Number</param>
        ''' <param name="pCellNumber">Cell Number</param>
        ''' <returns>GlobalDataTO containing a type DataSet VirtualRotorPositionsDS with all information of the specified
        '''          NOT IN USE Rotor Position</returns>
        ''' <remarks>
        ''' Created by: AG 21/01/2010 (Tested OK)
        ''' </remarks>
        Public Function GetPositionContent(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pRotorType As String, _
                                           ByVal pRingNumber As Integer, ByVal pCellNumber As Integer, ByVal pWorkSessionID As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksWSNotInUseRotorPositionsDAO
                        resultData = myDAO.Read(dbConnection, pAnalyzerID, pWorkSessionID, pRotorType, pRingNumber, pCellNumber)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSNotInUseRotorPositionsDelegate.GetPositionContent", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get all Not In Use Positions for the specified Work Session and Analyzer Rotor
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pRotorType">Rotor Type</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pFindPatients">When FALSE, not in use rotor positions containing Patient Samples are excluded from the search</param>
        ''' <returns>GlobalDataTO containing a typed DataSet VirtualRotorPositionsDS with the information of the content of all Not In Use 
        '''          Positions for the specified Analyzer Rotor and Work Session </returns>
        ''' <remarks>
        ''' Created by:  VR 25/01/2010
        ''' Modified by: SA 28/03/2012 - Added code to get all Rotor Positions saved from the previous WS in the Internal Virtual Rotor, 
        '''                              search if it is a NOT IN USE position in the Analyzer Rotor and, in this case update fields TubeType, 
        '''                              RealVolume and Status with the saved values
        '''              DL 15/06/2012 - When the RealVolume of a Not InUse Reagent is not informed, then search the maximum volume of the 
        '''                              Reagent bottle according its size (TubeType)               
        '''              SA 20/06/2012 - If field RealVolume is not informed in the Virtual Rotor, then set it to NULL when updating the 
        '''                              information of the Rotor position
        '''              SA 16/04/2013 - Added parameter to indicate if the function has to search also Patient Samples elements
        '''              XB 18/12/2013 - Fix exceptional malfunction cases when NotInUseRotor positions are being matched with VirtualRotorPositions saved 
        '''                              (barcodeInfo was not compared betwwen both tables) - Task #1443
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
                        Dim WSNotInUseRotorPosDAO As New twksWSNotInUseRotorPositionsDAO
                        resultData = WSNotInUseRotorPosDAO.GetRotorPositionsByWorkSession(dbConnection, pAnalyzerID, pRotorType, pWorkSessionID, pFindPatients)

                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            Dim allNotInUseRotorPosDS As VirtualRotorPosititionsDS = DirectCast(resultData.SetDatos, VirtualRotorPosititionsDS)

                            If (allNotInUseRotorPosDS.tparVirtualRotorPosititions.Count > 0) Then
                                'Get all NOT IN USE positions saved in the Internal Virtual Rotor
                                Dim myVRotorPosDelegate As New VirtualRotorsPositionsDelegate
                                resultData = myVRotorPosDelegate.GetRotor(dbConnection, 0, pRotorType)

                                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                    Dim savedNotInUseRotorPosDS As VirtualRotorPosititionsDS = DirectCast(resultData.SetDatos, VirtualRotorPosititionsDS)

                                    Dim reagentBottlesDS As New ReagentTubeTypesDS
                                    Dim reagentBottles As New ReagentTubeTypesDelegate
                                    Dim lstRotorPos As List(Of VirtualRotorPosititionsDS.tparVirtualRotorPosititionsRow)

                                    For Each savedPosRow As VirtualRotorPosititionsDS.tparVirtualRotorPosititionsRow In savedNotInUseRotorPosDS.tparVirtualRotorPosititions
                                        'Search the corresponding Rotor Position to update fields TubeType, RealVolume and Status with the saved ones
                                        lstRotorPos = (From a As VirtualRotorPosititionsDS.tparVirtualRotorPosititionsRow In allNotInUseRotorPosDS.tparVirtualRotorPosititions _
                                                      Where a.RingNumber = savedPosRow.RingNumber _
                                                    AndAlso a.CellNumber = savedPosRow.CellNumber _
                                                     Select a).ToList

                                        If (lstRotorPos.Count = 1) Then
                                            lstRotorPos.First.BeginEdit()
                                            If (lstRotorPos.First.IsTubeTypeNull AndAlso Not savedPosRow.IsTubeTypeNull) Then lstRotorPos.First.TubeType = savedPosRow.TubeType

                                            ' XB 18/12/2013
                                            ' old code
                                            'If (String.Compare(pRotorType, "REAGENTS", False) = 0) Then
                                            '    If (savedPosRow.IsRealVolumeNull) Then
                                            '        'Search the maximum volume of the Reagent bottle according its size (TubeType)
                                            '        resultData = reagentBottles.GetVolumeByTubeType(dbConnection, lstRotorPos.First.TubeType)
                                            '        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                            '            reagentBottlesDS = DirectCast(resultData.SetDatos, ReagentTubeTypesDS)

                                            '            If (reagentBottlesDS.ReagentTubeTypes.Rows.Count > 0) Then
                                            '                lstRotorPos.First.RealVolume = reagentBottlesDS.ReagentTubeTypes.First.TubeVolume
                                            '            End If
                                            '        End If
                                            '    Else
                                            '        If (Not savedPosRow.IsRealVolumeNull) Then lstRotorPos.First.RealVolume = savedPosRow.RealVolume
                                            '    End If

                                            'ElseIf (String.Compare(pRotorType, "SAMPLES", False) = 0) Then
                                            '    If Not savedPosRow.IsRealVolumeNull Then lstRotorPos.First.RealVolume = savedPosRow.RealVolume

                                            'Else
                                            '    If (Not savedPosRow.IsRealVolumeNull) Then lstRotorPos.First.RealVolume = savedPosRow.RealVolume
                                            'End If

                                            'If (Not savedPosRow.IsStatusNull) Then lstRotorPos.First.Status = savedPosRow.Status

                                            ' new code
                                            If (String.Compare(pRotorType, "REAGENTS", False) = 0) Then
                                                Dim GetSavedValues As Boolean = True

                                                If (Not savedPosRow.IsBarcodeInfoNull) And (lstRotorPos.First.IsBarcodeInfoNull) Then
                                                    ' Not match
                                                    GetSavedValues = False
                                                End If
                                                If (savedPosRow.IsBarcodeInfoNull) And (Not lstRotorPos.First.IsBarcodeInfoNull) Then
                                                    ' Not match
                                                    GetSavedValues = False
                                                End If
                                                If (Not savedPosRow.IsBarcodeInfoNull) And (Not lstRotorPos.First.IsBarcodeInfoNull) Then
                                                    If savedPosRow.BarcodeInfo <> lstRotorPos.First.BarcodeInfo Then
                                                        ' Not match
                                                        GetSavedValues = False
                                                    End If
                                                End If

                                                If Not GetSavedValues Then
                                                    'Search the maximum volume of the Reagent bottle according its size (TubeType)
                                                    resultData = reagentBottles.GetVolumeByTubeType(dbConnection, lstRotorPos.First.TubeType)
                                                    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                                        reagentBottlesDS = DirectCast(resultData.SetDatos, ReagentTubeTypesDS)

                                                        If (reagentBottlesDS.ReagentTubeTypes.Rows.Count > 0) Then
                                                            lstRotorPos.First.RealVolume = reagentBottlesDS.ReagentTubeTypes.First.TubeVolume
                                                        End If
                                                    End If
                                                Else
                                                    If (Not savedPosRow.IsRealVolumeNull) Then lstRotorPos.First.RealVolume = savedPosRow.RealVolume
                                                    If (Not savedPosRow.IsStatusNull) Then lstRotorPos.First.Status = savedPosRow.Status
                                                End If


                                            ElseIf (String.Compare(pRotorType, "SAMPLES", False) = 0) Then
                                                If Not savedPosRow.IsRealVolumeNull Then lstRotorPos.First.RealVolume = savedPosRow.RealVolume

                                                If (Not savedPosRow.IsStatusNull) Then lstRotorPos.First.Status = savedPosRow.Status
                                            Else
                                                If (Not savedPosRow.IsRealVolumeNull) Then lstRotorPos.First.RealVolume = savedPosRow.RealVolume
                                            End If
                                            ' XB 18/12/2013

                                            lstRotorPos.First.EndEdit()
                                        End If
                                    Next savedPosRow
                                    lstRotorPos = Nothing
                                    allNotInUseRotorPosDS.AcceptChanges()
                                End If
                            End If

                            resultData.SetDatos = allNotInUseRotorPosDS
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
                myLogAcciones.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSNotInUseRotorPositionsDelegate.GetRotorPositionsByWorkSession", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
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
        ''' Created by: AG 21/01/2010 (Tested OK)
        ''' </remarks>
        Public Function Reset(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, _
                              ByVal pRotorType As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksWSNotInUseRotorPositionsDAO
                        resultData = myDAO.Reset(dbConnection, pAnalyzerID, pWorkSessionID, pRotorType)

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
                myLogAcciones.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSNotInUseRotorPositionsDelegate.Reset", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
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
        ''' Created By: GDS 21/04/2010
        ''' </remarks>
        Public Function ResetWS(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError) AndAlso (Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksWSNotInUseRotorPositionsDAO
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
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSNotInUseRotorPositionsDelegate.ResetWS", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Update field Status = NULL for the specified NOT IN USE Rotor Position 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">WorkSession Identifier</param>
        ''' <param name="pRotorType">Rotor Type (Samples/Reagents)</param>
        ''' <param name="pCellNumber">Optional parameter. When informed, it indicates the number of NOT IN USE Rotor Cell to update</param>
        ''' <param name="pCalibratorID">Optional parameter. When informed, it indicates that all the NOT IN USE Rotor Cells containing a tube
        '''                             of the specified Calibrator will be updated</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 10/02/2012
        ''' </remarks>
        Public Function SetStatusToNull(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, _
                                        ByVal pRotorType As String, Optional ByVal pCellNumber As Integer = -1, Optional ByVal pCalibratorID As Integer = -1) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksWSNotInUseRotorPositionsDAO
                        resultData = myDAO.SetStatusToNull(dbConnection, pAnalyzerID, pWorkSessionID, pRotorType, pCellNumber, pCalibratorID)

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
                myLogAcciones.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSNotInUseRotorPositionsDelegate.SetStatusToNull", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Update the Sample Type of the specified NOT IN USE Rotor Position 
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
        ''' </remarks>
        Public Function UpdateSampleType(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, _
                                         ByVal pRotorType As String, ByVal pCellNumber As Integer, ByVal pSampleType As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksWSNotInUseRotorPositionsDAO
                        resultData = myDAO.UpdateSampleType(dbConnection, pAnalyzerID, pWorkSessionID, pRotorType, pCellNumber, pSampleType)

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
                myLogAcciones.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSNotInUseRotorPositionsDelegate.UpdateSampleType", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
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
            Dim dbConnection As New SqlClient.SqlConnection
            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError) And (Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myWSNotInUseRotorPositionsDAO As New twksWSNotInUseRotorPositionsDAO
                        resultData = myWSNotInUseRotorPositionsDAO.UpdateWSAnalyzerID(dbConnection, pAnalyzerID, pWorkSessionID)

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

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSNotInUseRotorPositionsDelegate.UpdateWSAnalyzerID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Delete elements not in use when the barcode configuration change.
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pAnalyzerID">Analyzer ID</param>
        ''' <param name="pWorkSessionID">WorkSession ID</param>
        ''' <param name="pRotorType">Rotor Type</param>
        ''' <returns></returns>
        ''' <remarks>CREATED BY: TR 26/04/2013</remarks>
        Public Function DeleteNotInUsePatients(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
                                                          ByVal pWorkSessionID As String, ByVal pRotorType As GlobalEnumerates.Rotors) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError) AndAlso (Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksWSNotInUseRotorPositionsDAO
                        resultData = myDAO.DeleteNotInUsePatients(dbConnection, pAnalyzerID, pWorkSessionID, pRotorType)

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
                myLogAcciones.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSNotInUseRotorPositionsDelegate.ResetWS", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

#End Region

#Region "Private methods"
        ''' <summary>
        ''' Evaluate if data in paramaters contains invalid values. In this case do nothing but inform into internal LOG
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
        ''' <param name="pDBConnection"></param>
        ''' <param name="pRotorType"></param>
        ''' <param name="pNotInUseRotorPositionsDS"></param>
        ''' <param name="pProcessWhoCalls">Enumerate that informs the process who calls the delegate instance</param>
        ''' <returns></returns>
        ''' <remarks>AG 07/10/2014 - BA-1979 add traces into log when virtual rotor is saved with invalid values in order to find the origin</remarks>
        Private Function CheckForInvalidPosition(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pRotorType As String, ByVal pNotInUseRotorPositionsDS As VirtualRotorPosititionsDS, ByVal pProcessWhoCalls As ClassCalledFrom) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)

                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim textDetails As String = String.Empty
                        Select Case pProcessWhoCalls
                            Case ClassCalledFrom.BarcodeResultsManagement
                                textDetails = "(called during barcode results management)"
                            Case ClassCalledFrom.ChangeElementPosition
                                textDetails = "(called during change NO INUSE element position)"
                            Case ClassCalledFrom.LoadVirtualRotor
                                textDetails = "(called during load virtual rotor (NO INUSE elements))"
                        End Select


                        Dim myLogAcciones As New ApplicationLogManager()
                        Dim linqRes As List(Of VirtualRotorPosititionsDS.tparVirtualRotorPosititionsRow)
                        If pRotorType = "REAGENTS" Then
                            'REAGENTS without reagentID
                            linqRes = (From a As VirtualRotorPosititionsDS.tparVirtualRotorPosititionsRow In pNotInUseRotorPositionsDS.tparVirtualRotorPosititions _
                                      Where a.TubeContent = "REAGENT" AndAlso a.IsReagentIDNull Select a).ToList
                            If linqRes.Count > 0 Then
                                myLogAcciones.CreateLogActivity("Invalid values: REAGENT with ReagentID = vbNULL " & textDetails, "WSNotInUseRotorPositionsDelegate.CheckForInvalidPosition", EventLogEntryType.Error, False)
                            End If

                            'SPECIAL or WASH or SALINE solution without solutioncode
                            linqRes = (From a As VirtualRotorPosititionsDS.tparVirtualRotorPosititionsRow In pNotInUseRotorPositionsDS.tparVirtualRotorPosititions _
                                       Where (a.TubeContent = "SPEC_SOL" OrElse a.TubeContent = "WASH_SOL" OrElse a.TubeContent = "SALINESOL") AndAlso a.IsSolutionCodeNull Select a).ToList

                            If linqRes.Count > 0 Then
                                myLogAcciones.CreateLogActivity("Invalid values: Bottle solution with SolutionCode = vbNULL " & textDetails, "WSNotInUseRotorPositionsDelegate.CheckForInvalidPosition", EventLogEntryType.Error, False)
                            End If


                        Else
                            'CALIBRATOR without CalibratorID
                            linqRes = (From a As VirtualRotorPosititionsDS.tparVirtualRotorPosititionsRow In pNotInUseRotorPositionsDS.tparVirtualRotorPosititions _
                                      Where a.TubeContent = "CALIB" AndAlso a.IsCalibratorIDNull Select a).ToList
                            If linqRes.Count > 0 Then
                                myLogAcciones.CreateLogActivity("Invalid values: CALIBRATOR with CalibratorID = vbNULL " & textDetails, "WSNotInUseRotorPositionsDelegate.CheckForInvalidPosition", EventLogEntryType.Error, False)
                            End If

                            'CONTROL without ControlID
                            linqRes = (From a As VirtualRotorPosititionsDS.tparVirtualRotorPosititionsRow In pNotInUseRotorPositionsDS.tparVirtualRotorPosititions _
                                      Where a.TubeContent = "CTRL" AndAlso a.IsControlIDNull Select a).ToList
                            If linqRes.Count > 0 Then
                                myLogAcciones.CreateLogActivity("Invalid values: CONTROL with ControlID = vbNULL " & textDetails, "WSNotInUseRotorPositionsDelegate.CheckForInvalidPosition", EventLogEntryType.Error, False)
                            End If

                            'PATIENT without PatientID
                            linqRes = (From a As VirtualRotorPosititionsDS.tparVirtualRotorPosititionsRow In pNotInUseRotorPositionsDS.tparVirtualRotorPosititions _
                                      Where a.TubeContent = "PATIENT" AndAlso a.IsPatientIDNull Select a).ToList
                            If linqRes.Count > 0 Then
                                myLogAcciones.CreateLogActivity("Invalid values: PATIENT with PatientID = vbNULL " & textDetails, "WSNotInUseRotorPositionsDelegate.CheckForInvalidPosition", EventLogEntryType.Error, False)
                            End If

                            'Tube SPECIAL or WASH solution without solutioncode
                            linqRes = (From a As VirtualRotorPosititionsDS.tparVirtualRotorPosititionsRow In pNotInUseRotorPositionsDS.tparVirtualRotorPosititions _
                                       Where (a.TubeContent = "TUBE_SPEC_SOL" OrElse a.TubeContent = "TUBE_WASH_SOL") AndAlso a.IsSolutionCodeNull Select a).ToList

                            If linqRes.Count > 0 Then
                                myLogAcciones.CreateLogActivity("Invalid values: Tube solution with SolutionCode = vbNULL " & textDetails, "WSNotInUseRotorPositionsDelegate.CheckForInvalidPosition", EventLogEntryType.Error, False)
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

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSNotInUseRotorPositionsDelegate.CheckForInvalidPosition", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()

            End Try
            resultData.HasError = False 'Not inform error flag in this method!!
            Return resultData
        End Function

#End Region

    End Class
End Namespace
