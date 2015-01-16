Option Explicit On
Option Strict On

Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.DAL.DAO
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Types

Namespace Biosystems.Ax00.BL
    Public Class HisCalibratorsDelegate

#Region "Public Methods"
        ''' <summary>
        ''' Receive a list of CalibratorIDs/LotNumbers and for each one of them, verify if it already exists in Historics Module and when 
        ''' it does not exist, get the needed data and create the new Calibrator
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pHisCalibratorsDS">Typed DataSet HisCalibratorsDS containing the list of Calibrators to verify if they already exist 
        '''                                 in Historics Module and create them when not</param>
        ''' <returns>GlobalDataTO containing a typed Dataset HisCalibratorsDS with the identifier in Historics Module for each Calibrator in it</returns>
        ''' <remarks>
        ''' Created by:  SA 14/03/2012
        ''' </remarks>
        Public Function CheckCalibratorsInHistorics(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pHisCalibratorsDS As HisCalibratorsDS) _
                                                    As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New thisCalibratorsDAO
                        Dim auxiliaryDS As New HisCalibratorsDS
                        Dim calibratorsToAddDS As New HisCalibratorsDS

                        For Each calibRow As HisCalibratorsDS.thisCalibratorsRow In pHisCalibratorsDS.thisCalibrators
                            'Verify if the Calibrator already exists in Historics Module
                            resultData = myDAO.ReadByCalibratorID(dbConnection, calibRow.CalibratorID)

                            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                auxiliaryDS = DirectCast(resultData.SetDatos, HisCalibratorsDS)

                                If (auxiliaryDS.thisCalibrators.Rows.Count = 0) Then
                                    'New Calibrator; copy it to the auxiliary DS of elements to add
                                    calibratorsToAddDS.thisCalibrators.ImportRow(calibRow)
                                Else
                                    'The Calibrator already exists in Historics Module; inform all fields in the DS
                                    calibRow.BeginEdit()
                                    calibRow.HistCalibratorID = auxiliaryDS.thisCalibrators.First.HistCalibratorID
                                    calibRow.NumberOfCalibrators = auxiliaryDS.thisCalibrators.First.NumberOfCalibrators
                                    calibRow.EndEdit()
                                End If
                            Else
                                'Error verifying if the Calibrator exists in Historics Module
                                Exit For
                            End If
                        Next

                        'Add to Historics Module all new Calibrators
                        If (Not resultData.HasError AndAlso calibratorsToAddDS.thisCalibrators.Rows.Count > 0) Then
                            resultData = myDAO.Create(dbConnection, calibratorsToAddDS)

                            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                calibratorsToAddDS = DirectCast(resultData.SetDatos, HisCalibratorsDS)

                                'Search the added Calibrators in the entry DS and inform the CalibratorID in Historics Module
                                Dim lstCalibratorToUpdate As List(Of HisCalibratorsDS.thisCalibratorsRow)
                                For Each calibratorRow As HisCalibratorsDS.thisCalibratorsRow In calibratorsToAddDS.thisCalibrators
                                    lstCalibratorToUpdate = (From a As HisCalibratorsDS.thisCalibratorsRow In pHisCalibratorsDS.thisCalibrators _
                                                            Where a.CalibratorID = calibratorRow.CalibratorID _
                                                          AndAlso a.IsHistCalibratorIDNull).ToList

                                    If (lstCalibratorToUpdate.Count = 1) Then
                                        lstCalibratorToUpdate.First.BeginEdit()
                                        lstCalibratorToUpdate.First.HistCalibratorID = calibratorRow.HistCalibratorID
                                        lstCalibratorToUpdate.First.EndEdit()
                                    End If
                                Next
                                lstCalibratorToUpdate = Nothing
                            End If
                        End If

                        resultData.SetDatos = pHisCalibratorsDS

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
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "HisCalibratorsDelegate.CheckCalibratorsInHistorics", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Delete all not in use closed Calibrators
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by:  SG 01/07/2013
        ''' </remarks>
        Public Function DeleteClosedNotInUse(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New thisCalibratorsDAO
                        resultData = myDAO.DeleteClosedNotInUse(dbConnection)

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
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "HisCalibratorsDelegate.DeleteClosedNotInUse", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

#End Region
    End Class
End Namespace