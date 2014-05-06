Option Strict On
Option Explicit On

Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.DAL.DAO
Imports System.Data.SqlClient
Imports Biosystems.Ax00.Global.TO
Imports Biosystems.Ax00.Global.GlobalEnumerates

Namespace Biosystems.Ax00.BL

    Public Class WSPreparationsDelegate

#Region "Public Methods"

        ''' <summary>
        ''' Adds a new Work Session Preparation
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pPreparation">DataSet containing data of the Work Session Preparation to add</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by: RH 14/04/2010 - Tested: OK
        ''' </remarks>
        Public Function AddWSPreparation(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pPreparation As WSPreparationsDS) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim wsPreparationData As New twksWSPreparationsDAO()
                        resultData = wsPreparationData.Create(dbConnection, pPreparation)

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
                myLogAcciones.CreateLogActivity(ex.Message, "WSPreparationsDelegate.AddWSPreparation", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Gets detailed information of the specified Work Session Preparation
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pPreparationID">Work Session Preparation Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet WSPreparationsDS with all data of the specified Work Session Preparation</returns>
        ''' <remarks>
        ''' Created by: RH 14/04/2010 
        ''' AG 20/09/2012 - Rename as Read (old name was 'GetWSPreparationData'))
        ''' </remarks>
        Public Function Read(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pPreparationID As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim wsPreparationData As New twksWSPreparationsDAO()
                        resultData = wsPreparationData.Read(dbConnection, pPreparationID)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "WSPreparationsDelegate.Read", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Gets detailed information of all Work Session Preparations
        ''' </summary>
        ''' <param name="pDbConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTO containing a typed DataSet WSPreparationsDS with all data of all Work Session Preparations
        ''' </returns>
        ''' <remarks>
        ''' Created by: RH 14/04/2010
        ''' </remarks>
        Public Function GetAllWSPreparationData(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim wsPreparationData As New twksWSPreparationsDAO()
                        resultData = wsPreparationData.ReadAll(dbConnection)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "WSPreparationsDelegate.GetAllWSPreparationData", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Updates the Status of the specified Work Session Preparation
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pPreparationID">Work Session Preparation Identifier</param>
        ''' <param name="pNewPreparationStatus">New Work Session Preparation Status</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by: RH 15/04/2010
        ''' </remarks>
        Public Function UpdateStatus(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pPreparationID As Integer, _
                                     ByVal pNewPreparationStatus As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim wsPreparationData As New twksWSPreparationsDAO()
                        resultData = wsPreparationData.UpdateStatus(dbConnection, pPreparationID, pNewPreparationStatus)

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
                myLogAcciones.CreateLogActivity(ex.Message, "WSPreparationsDelegate.UpdateStatus", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Deletes the specified Work Session Preparation
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by: RH 15/04/2010
        ''' </remarks>
        Public Function Delete(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pPreparationID As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim wsPreparationData As New twksWSPreparationsDAO()
                        resultData = wsPreparationData.Delete(dbConnection, pPreparationID)

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
                myLogAcciones.CreateLogActivity(ex.Message, "WSPreparationsDelegate.Delete", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Deletes all Work Session Preparations data
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by: RH 15/04/2010
        ''' </remarks>
        Public Function DeleteAll(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim wsPreparationData As New twksWSPreparationsDAO()
                        resultData = wsPreparationData.DeleteAll(dbConnection)

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
                myLogAcciones.CreateLogActivity(ex.Message, "WSPreparationsDelegate.DeleteAll", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Generates the next PreparationID for a specific Work Session Preparation
        ''' </summary>
        ''' <param name="pDBConnection">Opened Database Connection</param>
        ''' <returns>GlobalDataTO containing the generated PreparationID</returns>
        ''' <remarks>
        ''' Created by: RH 15/04/2010 
        ''' </remarks>
        Public Function GeneratePreparationID(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim wsPreparationData As New twksWSPreparationsDAO()
                        resultData = wsPreparationData.GeneratePreparationID(dbConnection)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "WSPreparationsDelegate.GeneratePreparationID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Delete all information for the Work Session
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created By: GDS 21/04/2010
        ''' </remarks>
        Public Function ResetWS(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
                                ByVal pWorkSessionID As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksWSPreparationsDAO
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
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "WSPreparationsDelegate.ResetWS", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function


        ''' <summary>
        ''' Looks for the position DEPLETED informed in ANSTIN instruction
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pPrepID"></param>
        ''' <param name="pKOdescription"></param>
        ''' <returns>GlobalDataTo with integer as data</returns>
        ''' <remarks>AG 20/09/2012</remarks>
        Public Function GetDepletedPositionDuringRecoveryResults(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pPrepID As Integer, ByVal pKOdescription As String) As GlobalDataTO
            Dim myGlobal As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobal = DAOBase.GetOpenDBConnection(pDBConnection)

                If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobal.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim rotorPosition As Integer = 0

                        'Get the TEST or PTEST or ISETEST preparation string
                        Dim myDAO As New twksWSPreparationsDAO
                        myGlobal = myDAO.Read(dbConnection, pPrepID)

                        If Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing Then
                            Dim prepDS As WSPreparationsDS
                            prepDS = CType(myGlobal.SetDatos, WSPreparationsDS)

                            If prepDS.twksWSPreparations.Rows.Count > 0 AndAlso Not prepDS.twksWSPreparations(0).IsLAX00DataNull Then
                                Dim lax00Command As String = prepDS.twksWSPreparations(0).LAX00Data

                                'To test SAMPLE, R1, R2
                                'Dim lax00Command As String = "A400;TEST;TI:1;ID:7;M1:2;TM1:2;RM1:0;R1:1;TR1:5;RR1:0;R2:0;TR2:0;RR2:0;VM1:170;VR1:2387;VR2:0;OW:3;EW:6;RN:36;"

                                'To test SAMPLE
                                'Dim lax00Command As String = "A400;ISETEST;TI:1;ID:46;M1:4;TM1:2;RM1:0;VM1:4527;"

                                'To test SAMPLETODILUTE, R1, R2, DILUENT
                                'Dim lax00Command As String = "A400;PTEST;TI:1;ID:48;R1:2;TR1:5;RR1:0;R2:0;TR2:0;RR2:0;VM1:170;VR1:2387;VR2:0;OW:7;EW:7;RN:50;PM1:4;PTM:2;PRM:0;PR1:88;PTR:6;PR1R:0;PVM:2829;PVR:796;"

                                Dim startStringPosition As Integer = 0
                                Dim endStringPosition As Integer = 0
                                Select Case pKOdescription
                                    Case "SAMPLE" 'Search for ';M1:'
                                        startStringPosition = InStr(lax00Command, ";M1:")
                                        If startStringPosition > 0 Then
                                            startStringPosition += 4
                                            endStringPosition = InStr(lax00Command, ";TM1:")
                                            If endStringPosition > 0 Then
                                                rotorPosition = CInt(Mid$(lax00Command, startStringPosition, endStringPosition - startStringPosition))
                                            End If
                                        End If

                                    Case "R1" 'Search for ';R1:'
                                        startStringPosition = InStr(lax00Command, ";R1:")
                                        If startStringPosition > 0 Then
                                            startStringPosition += 4
                                            endStringPosition = InStr(lax00Command, ";TR1:")
                                            If endStringPosition > 0 Then
                                                rotorPosition = CInt(Mid$(lax00Command, startStringPosition, endStringPosition - startStringPosition))
                                            End If
                                        End If

                                    Case "R2" 'Search for ';R2:'
                                        startStringPosition = InStr(lax00Command, ";R2:")
                                        If startStringPosition > 0 Then
                                            startStringPosition += 4
                                            endStringPosition = InStr(lax00Command, ";TR2:")
                                            If endStringPosition > 0 Then
                                                rotorPosition = CInt(Mid$(lax00Command, startStringPosition, endStringPosition - startStringPosition))
                                            End If
                                        End If

                                    Case "DILUENT" 'Search for ';PR1:'
                                        startStringPosition = InStr(lax00Command, ";PR1:")
                                        If startStringPosition > 0 Then
                                            startStringPosition += 5
                                            endStringPosition = InStr(lax00Command, ";PTR:")
                                            If endStringPosition > 0 Then
                                                rotorPosition = CInt(Mid$(lax00Command, startStringPosition, endStringPosition - startStringPosition))
                                            End If
                                        End If

                                    Case "SAMPLETODILUTE" 'Search for ';PM1:'
                                        startStringPosition = InStr(lax00Command, ";PM1:")
                                        If startStringPosition > 0 Then
                                            startStringPosition += 5
                                            endStringPosition = InStr(lax00Command, ";PTM:")
                                            If endStringPosition > 0 Then
                                                rotorPosition = CInt(Mid$(lax00Command, startStringPosition, endStringPosition - startStringPosition))
                                            End If
                                        End If
                                End Select

                            End If
                        End If

                        myGlobal.SetDatos = rotorPosition

                    End If
                End If

            Catch ex As Exception
                myGlobal = New GlobalDataTO()
                myGlobal.HasError = True
                myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "WSPreparationsDelegate.GetDepletedPositionDuringRecoveryResults", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()

            End Try

            Return myGlobal
        End Function


        ''' <summary>
        ''' Get the cuvettes contaminated during recovery results (communications failed during Running)
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pMinPrepID"></param>
        ''' <returns></returns>
        ''' <remarks>AG 25/09/2012 created</remarks>
        Public Function ReadCuvettesContaminatedAfterRecoveryResults(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pMinPrepID As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)

                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksWSPreparationsDAO
                        resultData = myDAO.ReadCuvettesContaminatedAfterRecoveryResults(dbConnection, pMinPrepID)
                    End If
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "WSPreparationsDelegate.ReadCuvettesContaminatedAfterRecoveryResults", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()

            End Try

            Return resultData
        End Function


#End Region
    End Class
End Namespace
