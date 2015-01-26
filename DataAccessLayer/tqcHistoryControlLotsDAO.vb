Option Strict On
Option Explicit On

Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global

Namespace Biosystems.Ax00.DAL.DAO

    Public Class tqcHistoryControlLotsDAO
          

#Region "CRUD Methods"
        ''' <summary>
        ''' When a Lot is changed for a Control or a Control is deleted, the Lot is marked as Closed in the history table of QC Module.
        ''' Besides, if the Control was deleted, it is marked also as deleted in the history table of QC Module
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pControlID">Identifier of the Control that has been changed or deleted</param>
        ''' <param name="pLotNumber">Lot Number that has been changed for the Control. Optional parameter, informed only when
        '''                          the screen was called for a Lot change</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 20/05/2011
        ''' </remarks>
        Public Function CloseLotDeleteControl(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pControlID As Integer, _
                                              Optional ByVal pLotNumber As String = "") As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    'There is not an Opened Database Connection...
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String
                    If (pLotNumber <> String.Empty) Then
                        'Lot changed --> Mark Lot as Closed
                        cmdText = " UPDATE tqcHistoryControlLots " & vbCrLf & _
                                  " SET    ClosedLot = 1 " & vbCrLf & _
                                  " WHERE  ControlID = " & pControlID & vbCrLf & _
                                  " AND    LotNumber = N'" & pLotNumber.Replace("'", "''") & "' "
                    Else
                        'Control deleted --> Mark Lot as Closed and Control as Deleted
                        cmdText = " UPDATE tqcHistoryControlLots " & vbCrLf & _
                                  " SET    ClosedLot = 1, DeletedControl = 1 " & vbCrLf & _
                                  " WHERE  ControlID = " & pControlID
                    End If

                    Dim dbCmd As New SqlClient.SqlCommand
                    dbCmd.Connection = pDBConnection
                    dbCmd.CommandText = cmdText

                    myGlobalDataTO.AffectedRecords = dbCmd.ExecuteNonQuery()
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, " tqcHistoryControlLotsDAO.CloseLotDeleteControl", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Create a new Control/Lot in the history table of QC Module
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pHistoryControlLotsDS">Typed DataSet HistoryControlLotsDS containing all data needed to create the 
        '''                                     Control/Lot in the history table of QC Module</param>
        ''' <returns>GlobalDataTO containing a typed DataSet HistoryControlLotsDS containing data of the created Control/Lot
        '''          including the automatically generated QCControlLotID</returns>
        ''' <remarks>
        ''' Created by:  TR 13/05/2011
        ''' Modified by: SA 20/05/2011 - Removed the For/Next loop. Added N prefix to allow not ansi characters in text fields.
        '''                              None of the values can be NULL
        ''' </remarks>
        Public Function Create(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pHistoryControlLotsDS As HistoryControlLotsDS) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    'There is not an Opened Database Connection...
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString

                ElseIf (Not pHistoryControlLotsDS Is Nothing) Then
                    Dim cmdText As String = " INSERT INTO tqcHistoryControlLots (ControlID, ControlName, SampleType, LotNumber, CreationDate, " & vbCrLf & _
                                                                               " ClosedLot, DeletedControl) " & vbCrLf & _
                                                " VALUES (" & pHistoryControlLotsDS.tqcHistoryControlLots(0).ControlID & ", " & vbCrLf & _
                                                      " N'" & pHistoryControlLotsDS.tqcHistoryControlLots(0).ControlName.Replace("'", "''") & "', " & vbCrLf & _
                                                       " '" & pHistoryControlLotsDS.tqcHistoryControlLots(0).SampleType & "', " & vbCrLf & _
                                                      " N'" & pHistoryControlLotsDS.tqcHistoryControlLots(0).LotNumber.Replace("'", "''") & "', " & vbCrLf & _
                                                       " '" & pHistoryControlLotsDS.tqcHistoryControlLots(0).CreationDate.ToString("yyyyMMdd HH:mm:ss") & "', 0, 0) " & vbCrLf
                    cmdText &= " SELECT SCOPE_IDENTITY() "

                    Dim dbCmd As New SqlClient.SqlCommand
                    dbCmd.Connection = pDBConnection
                    dbCmd.CommandText = cmdText

                    'Get the generated ID and assign it to a local variable
                    Dim myNewQCControlLotsID As Integer = CType(dbCmd.ExecuteScalar(), Integer)
                    If (myNewQCControlLotsID > 0) Then
                        'Inform the field in the DS to return
                        pHistoryControlLotsDS.tqcHistoryControlLots(0).SetField("QCControlLotID", myNewQCControlLotsID)
                    End If
                    myGlobalDataTO.SetDatos = pHistoryControlLotsDS
                    myGlobalDataTO.HasError = False
                End If

            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, " tqcHistoryControlLotsDAO.Create ", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get all data of the specified QCControlLotID in the history Control/Lots table in QC Module. 
        ''' Flags of ClosedLot and DeletedControl have to be FALSE, which mean it is the active Control/Lot
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pQCControlLotID">Identifier of the Control/Lot in QC Module</param>
        ''' <returns>GlobalDataTO containing a typed DataSet HistoryControlLotsDS with data of the Control/Lot in
        '''          the history table in QC Module</returns>
        ''' <remarks>
        ''' Created by: SA 02/01/2012
        ''' </remarks>
        Public Function Read(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCControlLotID As Integer) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT * " & vbCrLf & _
                                                " FROM  tqcHistoryControlLots  " & vbCrLf & _
                                                " WHERE QCControlLotID = " & pQCControlLotID & vbCrLf & _
                                                " AND   ClosedLot = 0 " & vbCrLf & _
                                                " AND   DeletedControl = 0 "

                        Dim myHistoryControlLotsDS As New HistoryControlLotsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myHistoryControlLotsDS.tqcHistoryControlLots)
                            End Using
                        End Using

                        myGlobalDataTO.SetDatos = myHistoryControlLotsDS
                        myGlobalDataTO.HasError = False
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tqcHistoryControlLotsDAO.Read", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' When basic data of a Control (Name and/or Sample Type) is changed in Controls Programming Screen,
        ''' values are updated for all not delete records the Control has in the history table of QC Module
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pControlDS">Typed DataSet ControlDS containing data to update the Control basic
        '''                          information in the history table of QC Module</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 20/05/2011
        ''' </remarks>
        Public Function UpdateControlData(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pControlDS As ControlsDS) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    'There is not an Opened Database Connection...
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = " UPDATE tqcHistoryControlLots " & vbCrLf & _
                                            " SET    ControlName = N'" & pControlDS.tparControls(0).ControlName.Replace("'", "''") & "', " & vbCrLf & _
                                                   " SampleType = '" & pControlDS.tparControls(0).SampleType & "' " & vbCrLf & _
                                            " WHERE  ControlID = " & pControlDS.tparControls(0).ControlID & vbCrLf & _
                                            " AND    DeletedControl = 0 "

                    Dim dbCmd As New SqlClient.SqlCommand
                    dbCmd.Connection = pDBConnection
                    dbCmd.CommandText = cmdText

                    myGlobalDataTO.AffectedRecords = dbCmd.ExecuteNonQuery()
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, " tqcHistoryControlLotsDAO.UpdateControlData", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function
#End Region

#Region "Other Methods"
        ''' <summary>
        ''' For the informed Control, search if it exists in QC Module and in this case, get the identifier QCControlLotID.
        ''' Besides, search all Tests/SampleTypes linked to the Control and having information in QC Module, and get the 
        ''' identifier QCTestSample for each one of them 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pControlID">Control Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet HistoryTestControlLotsDS with the identifier of the informed Control in
        '''          the history table in QC Module (QCControlLotID) and the identifier of each one of the linked Tests/SampleTypes in 
        '''          the history table in QC Module (QCTestSampleID)</returns>
        ''' <remarks>
        ''' Created by:  SA 23/05/2011
        ''' Modified by: SA 20/12/2011 - Changed the query to get also field CalculationMode from table tqcHistoryTestSamples
        ''' </remarks>
        Public Function GetLinkedTestsSampleTypesByControl(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pControlID As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing
            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT HCL.QCControlLotID, HTCL.QCTestSampleID, HTS.CalculationMode " & vbCrLf & _
                                                " FROM   tqcHistoryControlLots HCL INNER JOIN tqcHistoryTestControlLots HTCL ON HCL.QCControlLotID = HTCL.QCControlLotID " & vbCrLf & _
                                                                                 " INNER JOIN tqcHistoryTestSamples HTS ON HTCL.QCTestSampleID = HTS.QCTestSampleID " & vbCrLf & _
                                                " WHERE  HCL.ControlID = " & pControlID & vbCrLf & _
                                                " AND    HCL.ClosedLot = 0 " & vbCrLf & _
                                                " AND    HCL.DeletedControl = 0 " & vbCrLf & _
                                                " AND    HTS.DeletedSampleType = 0 " & vbCrLf & _
                                                " AND    HTS.DeletedTest = 0 " & vbCrLf

                        Dim myHistoryTestControlLotsDS As New HistoryTestControlLotsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myHistoryTestControlLotsDS.tqcHistoryTestControlLots)
                            End Using
                        End Using

                        resultData.SetDatos = myHistoryTestControlLotsDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, " tqcHistoryControlLotsDAO.GetLinkedTestsSampleTypesByControl", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get all data of the specified Control/Lot in the history table in QC Module (field QCControlLotID)
        ''' Flags of ClosedLot and DeletedControl have to be FALSE, which mean it is the active Control/Lot
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pControlID">Control Identifier</param>
        ''' <param name="pLotNumber">Lot Number; optional parameter</param>
        ''' <returns>GlobalDataTO containing a typed DataSet HistoryControlLotsDS with data of the Control/Lot in 
        '''          the history table in QC Module</returns>
        ''' <remarks>
        ''' Created by:  TR 10/05/2011
        ''' Modified by: SA 20/05/2011 - Added Replace and N prefix to LotNumber field. Get all fields instead only the QCControlLotID
        ''' </remarks>
        Public Function GetQCControlLotIDByControlIDAndLotNumber(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pControlID As Integer, _
                                                                 ByVal pLotNumber As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing
            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = ""
                        cmdText &= " SELECT * " & vbCrLf
                        cmdText &= " FROM  tqcHistoryControlLots " & vbCrLf
                        cmdText &= " WHERE ControlID = " & pControlID & vbCrLf
                        cmdText &= " AND   LotNumber = N'" & pLotNumber.Replace("'", "''") & "' " & vbCrLf
                        cmdText &= " AND   ClosedLot = 0 " & vbCrLf
                        cmdText &= " AND   DeletedControl = 0 "

                        Dim myHistoryControlLotsDS As New HistoryControlLotsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myHistoryControlLotsDS.tqcHistoryControlLots)
                            End Using
                        End Using

                        resultData.SetDatos = myHistoryControlLotsDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, " tqcHistoryControlLotsDAO.GetQCControlLotIDByControlIDAndLotNumber", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function
#End Region
    End Class
End Namespace
