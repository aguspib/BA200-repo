Option Strict On
Option Explicit On

Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global

Namespace Biosystems.Ax00.DAL.DAO

    Public Class tqcHistoryTestControlLotsDAO
        Inherits DAOBase

#Region "CRUD Methods"
        ''' <summary>
        ''' Create the link between the specified QCTestSampleID and QCControlLotID
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pHistoryTestControlLotDS">Typed DataSet HistoryTestControlLotsDS containing the information needed to 
        '''                                        create the link between the specified QCTestSampleID and QCControlLotID</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  TR 13/05/2011
        ''' Modified by: SA 02/06/2011 - Use function ReplaceNumericString insteat of ToSQLString to format decimal values to avoid loss precision
        ''' </remarks>
        Public Function Create(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pHistoryTestControlLotDS As HistoryTestControlLotsDS) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    'There is not an Opened Database Connection...
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    If (pHistoryTestControlLotDS.tqcHistoryTestControlLots.Count > 0) Then
                        Dim cmdText As String = " INSERT INTO tqcHistoryTestControlLots(QCTestSampleID, QCControlLotID, MinConcentration, MaxConcentration) " & vbCrLf & _
                                                " VALUES (" & pHistoryTestControlLotDS.tqcHistoryTestControlLots(0).QCTestSampleID & ", " & vbCrLf & _
                                                              pHistoryTestControlLotDS.tqcHistoryTestControlLots(0).QCControlLotID & ", " & vbCrLf & _
                                                              ReplaceNumericString(pHistoryTestControlLotDS.tqcHistoryTestControlLots(0).MinConcentration) & ", " & vbCrLf & _
                                                              ReplaceNumericString(pHistoryTestControlLotDS.tqcHistoryTestControlLots(0).MaxConcentration) & ") "

                        Dim dbCmd As New SqlClient.SqlCommand
                        dbCmd.Connection = pDBConnection
                        dbCmd.CommandText = cmdText

                        myGlobalDataTO.AffectedRecords = dbCmd.ExecuteNonQuery()
                        myGlobalDataTO.HasError = False
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "tqcHistoryTestControlLotsDAO.Create", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get all values of an existing link between an specific QCTestSampleID and QCControlLotID (Min/Max Concentration) 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pQCTestSampleID">Identifier of Test/SampleType in QC</param>
        ''' <param name="pQCControlLotID">Identifier of Control/Lot in QC</param>
        ''' <returns>GlobalDataTO containing a typed DataSet HistoryTestControlLotsDS with all Test Control values</returns>
        ''' <remarks>
        ''' Created by:  TR 13/05/2011
        ''' </remarks>
        Public Function Read(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCTestSampleID As Integer, ByVal pQCControlLotID As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT * " & vbCrLf & _
                                                " FROM   tqcHistoryTestControlLots " & vbCrLf & _
                                                " WHERE  QCTestSampleID = " & pQCTestSampleID & vbCrLf & _
                                                " AND    QCControlLotID = " & pQCControlLotID

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

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "tqcHistoryTestControlLotsDAO.Read", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Update Min/Max Concentration values for an specific QCTestSampleID and QCControlLotID
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pQCTestSampleID">Identifier of Test/SampleType in QC</param>
        ''' <param name="pQCControlLotID">Identifier of Control/Lot in QC</param>
        ''' <param name="pMinValue">Min concentration value</param>
        ''' <param name="pMaxValue">Max concentration value</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  TR 10/05/2011
        ''' Modified by: SA 02/06/2011 - Use function ReplaceNumericString insteat of ToSQLString to format decimal values to avoid loss precision
        ''' </remarks>
        Public Function Update(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCTestSampleID As Integer, ByVal pQCControlLotID As Integer, _
                               ByVal pMinValue As Single, ByVal pMaxValue As Single) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    'There is not an Opened Database Connection...
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = " UPDATE tqcHistoryTestControlLots " & vbCrLf & _
                                            " SET    MinConcentration = " & ReplaceNumericString(pMinValue) & ", " & vbCrLf & _
                                                   " MaxConcentration = " & ReplaceNumericString(pMaxValue) & vbCrLf & _
                                            " WHERE QCTestSampleID = " & pQCTestSampleID.ToString & vbCrLf & _
                                            " AND   QCControlLotID = " & pQCControlLotID.ToString()

                    Dim dbCmd As New SqlClient.SqlCommand
                    dbCmd.Connection = pDBConnection
                    dbCmd.CommandText = cmdText

                    myGlobalDataTO.AffectedRecords = dbCmd.ExecuteNonQuery()
                    myGlobalDataTO.HasError = False
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "tqcHistoryTestControlLotsDAO.Update", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function
#End Region

#Region "Other Methods"
        ''' <summary>
        ''' Get values of all Control/Lots linked to the specified Test/SampleType
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pQCTestSampleID">Identifier of Test/SampleType in QC</param>
        ''' <returns>GlobalDataTO containing a typed DataSet HistoryTestControlLotsDS with all values defined for 
        '''          all Control/Lots linked to the specified Test/SampleType</returns>
        ''' <remarks>
        ''' Created by:  TR 27/05/2011
        ''' Modified by: SA 25/01/2012 - Get only Controls/Lots having non cumulated QC Results
        '''              SA 05/06/2012 - Added parameter for AnalyzerID; changed the query by adding a filter by this field 
        ''' </remarks>
        Public Function GetAllControlsLinkedToTestSampleTypeNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCTestSampleID As Integer, _
                                                                ByVal pAnalyzerID As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT DISTINCT HCL.QCControlLotID, HCL.ControlName, HTCL.WestgardControlNum " & vbCrLf & _
                                                " FROM   tqcHistoryTestControlLots HTCL INNER JOIN tqcHistoryControlLots HCL ON HTCL.QCControlLotID = HCL.QCControlLotID " & vbCrLf & _
                                                                                      " INNER JOIN tqcResults R ON HTCL.QCTestSampleID = R.QCTestSampleID " & vbCrLf & _
                                                                                                             " AND HTCL.QCControlLotID = R.QCControlLotID " & vbCrLf & _
                                                " WHERE  HTCL.QCTestSampleID = " & pQCTestSampleID & vbCrLf & _
                                                " AND    HCL.ClosedLot       = 0 " & vbCrLf & _
                                                " AND    HCL.DeletedControl  = 0 " & vbCrLf & _
                                                " AND    R.AnalyzerID        = N'" & pAnalyzerID.Replace("'", "''").Trim & "' " & vbCrLf & _
                                                " ORDER BY HCL.ControlName "

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

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, " tqcHistoryTestControlLotsDAO.GetAllControlsLinkedToTestSampleType", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get the identifiers in history tables of QC Module for the specified  TestType/TestID/SampleType and Control/Lot
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestType">Test Type Code</param>
        ''' <param name="pTestID">Test Identifier</param>
        ''' <param name="pSampleType">Sample Type Code</param>
        ''' <param name="pControlID">Control Identifier</param>
        ''' <param name="pLotNumber">Lot Number</param>
        ''' <returns>GlobalDataTO containing a typed DataSet HistoryTestControlLotsDS with the identifiers of the
        '''          Test/SampleType and the Control/Lot in QC Module</returns>
        ''' <remarks>
        ''' Created by:  SA 17/06/2011
        ''' Modified by: SA 05/06/2012 - Added parameter for TestType and filter the query for this field
        ''' </remarks>
        Public Function GetQCIDsForTestAndControlNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestType As String, ByVal pTestID As Integer, _
                                                     ByVal pSampleType As String, ByVal pControlID As Integer, ByVal pLotNumber As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT HTCL.QCTestSampleID, HTCL.QCControlLotID " & vbCrLf & _
                                                " FROM   tqcHistoryTestControlLots HTCL INNER JOIN tqcHistoryTestSamples HTS ON HTCL.QCTestSampleID = HTS.QCTestSampleID " & vbCrLf & _
                                                                                      " INNER JOIN tqcHistoryControlLots HCL ON HTCL.QCControlLotID = HCL.QCControlLotID " & vbCrLf & _
                                                " WHERE  HTS.TestType          = '" & pTestType.Trim & "' " & vbCrLf & _
                                                " AND    HTS.TestID            = " & pTestID.ToString & vbCrLf & _
                                                " AND    HTS.SampleType        = '" & pSampleType.Trim & "' " & vbCrLf & _
                                                " AND    HTS.DeletedSampleType = 0 " & vbCrLf & _
                                                " AND    HTS.DeletedTest       = 0 " & vbCrLf & _
                                                " AND    HCL.ControlID         = " & pControlID & vbCrLf & _
                                                " AND    HCL.LotNumber         = N'" & pLotNumber.Replace("'", "''") & "' " & vbCrLf & _
                                                " AND    HCL.ClosedLot         = 0 " & vbCrLf & _
                                                " AND    HCL.DeletedControl    = 0 "

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

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, " tqcHistoryTestControlLotsDAO.GetQCIDsForTestAndControl", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Update field WestgardControNum for each Control/Lot linked to the specified Test/SampleType
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pQCTestSampleID">Identifier of Test/SampleType in QC</param>
        ''' <param name="pQCControlLotIDForWESG1">QC Identifier for the first Control/Lot selected to apply Multirules</param>
        ''' <param name="pQCControlLotIDForWESG2">QC Identifier for the second Control/Lot selected to apply Multirules
        '''                                       Optional parameter</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  TR 30/05/2011
        ''' Modified by: SA 25/01/2012 - Parameter for second Control/Lot changed to optional; update the WestgardControlNum=2 only 
        '''                              when the parameter is informed. Changed the query to update WestgardControlNum = 0, it will
        '''                              depend on if the parameter for second Control/Lot is informed or not
        ''' </remarks>
        Public Function UpdateWestgardControlNum(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCTestSampleID As Integer, _
                                                 ByVal pQCControlLotIDForWESG1 As String, Optional ByVal pQCControlLotIDForWESG2 As String = "") As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    'There is not an Opened Database Connection...
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    'Update first Westgard Control
                    Dim cmdText As String = " UPDATE tqcHistoryTestControlLots " & vbCrLf & _
                                            " SET    WestgardControlNum = 1 " & vbCrLf & _
                                            " WHERE  QCTestSampleID = " & pQCTestSampleID.ToString() & vbCrLf & _
                                            " AND    QCControlLotID = " & pQCControlLotIDForWESG1 & vbCrLf

                    'When informed, update second Westgard Control
                    If (pQCControlLotIDForWESG2 <> String.Empty) Then
                        cmdText &= " UPDATE tqcHistoryTestControlLots " & vbCrLf & _
                                   " SET    WestgardControlNum = 2 " & vbCrLf & _
                                   " WHERE  QCTestSampleID = " & pQCTestSampleID.ToString() & vbCrLf & _
                                   " AND    QCControlLotID = " & pQCControlLotIDForWESG2 & vbCrLf
                    End If

                    'Finally, update the rest of Controls to zero
                    cmdText &= " UPDATE tqcHistoryTestControlLots SET WestgardControlNum = 0 " & vbCrLf & _
                               " WHERE  QCTestSampleID = " & pQCTestSampleID.ToString() & vbCrLf & _
                               " AND    QCControlLotID <> " & pQCControlLotIDForWESG1 & vbCrLf
                    If (pQCControlLotIDForWESG2 <> String.Empty) Then cmdText &= " AND QCControlLotID <> " & pQCControlLotIDForWESG2 & vbCrLf

                    Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                        myGlobalDataTO.AffectedRecords = dbCmd.ExecuteNonQuery()
                        myGlobalDataTO.HasError = False
                    End Using
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "tqcHistoryTestControlLotsDAO.UpdateWestgardControlNum", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function
#End Region

#Region "TO DELETE - OLD METHODS"
        '''' <summary>
        '''' Get values of all Control/Lots linked to the specified Test/SampleType
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pQCTestSampleID">Identifier of Test/SampleType in QC</param>
        '''' <returns>GlobalDataTO containing a typed DataSet HistoryTestControlLotsDS with all values defined for 
        ''''          all Control/Lots linked to the specified Test/SampleType</returns>
        '''' <remarks>
        '''' Created by:  TR 27/05/2011
        '''' Modified by: SA 25/01/2012 - Get only Controls/Lots having non cumulated QC Results
        '''' </remarks>
        'Public Function GetAllControlsLinkedToTestSampleType(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCTestSampleID As Integer) As GlobalDataTO
        '    Dim resultData As GlobalDataTO = Nothing
        '    Dim dbConnection As SqlClient.SqlConnection = Nothing

        '    Try
        '        resultData = GetOpenDBConnection(pDBConnection)
        '        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
        '            dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim cmdText As String = " SELECT DISTINCT HCL.QCControlLotID, HCL.ControlName, HTCL.WestgardControlNum " & vbCrLf & _
        '                                        " FROM   tqcHistoryTestControlLots HTCL INNER JOIN tqcHistoryControlLots HCL ON HTCL.QCControlLotID = HCL.QCControlLotID " & vbCrLf & _
        '                                                                              " INNER JOIN tqcResults R ON HTCL.QCTestSampleID = R.QCTestSampleID " & vbCrLf & _
        '                                                                                                     " AND HTCL.QCControlLotID = R.QCControlLotID " & vbCrLf & _
        '                                        " WHERE  HTCL.QCTestSampleID = " & pQCTestSampleID & vbCrLf & _
        '                                        " AND    HCL.ClosedLot = 0 " & vbCrLf & _
        '                                        " AND    HCL.DeletedControl = 0 " & vbCrLf & _
        '                                        " ORDER BY HCL.ControlName "

        '                Dim myHistoryTestControlLotsDS As New HistoryTestControlLotsDS
        '                Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
        '                    Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
        '                        dbDataAdapter.Fill(myHistoryTestControlLotsDS.tqcHistoryTestControlLots)
        '                    End Using
        '                End Using

        '                resultData.SetDatos = myHistoryTestControlLotsDS
        '                resultData.HasError = False
        '            End If
        '        End If
        '    Catch ex As Exception
        '        resultData = New GlobalDataTO
        '        resultData.HasError = True
        '        resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        resultData.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, " tqcHistoryTestControlLotsDAO.GetAllControlsLinkedToTestSampleType", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return resultData
        'End Function

        '''' <summary>
        '''' Get the identifiers in history tables of QC Module for the specified Test/SampleType and Control/Lot
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pTestID">Test Identifier</param>
        '''' <param name="pSampleType">Sample Type Code</param>
        '''' <param name="pControlID">Control Identifier</param>
        '''' <param name="pLotNumber">Lot Number</param>
        '''' <returns>GlobalDataTO containing a typed DataSet HistoryTestControlLotsDS with the identifiers of the
        ''''          Test/SampleType and the Control/Lot in QC Module</returns>
        '''' <remarks>
        '''' Created by:  SA 17/06/2011
        '''' </remarks>
        'Public Function GetQCIDsForTestAndControl(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer, ByVal pSampleType As String, _
        '                                          ByVal pControlID As Integer, ByVal pLotNumber As String) As GlobalDataTO
        '    Dim resultData As GlobalDataTO = Nothing
        '    Dim dbConnection As SqlClient.SqlConnection = Nothing

        '    Try
        '        resultData = GetOpenDBConnection(pDBConnection)
        '        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
        '            dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim cmdText As String = " SELECT HTCL.QCTestSampleID, HTCL.QCControlLotID " & vbCrLf & _
        '                                        " FROM   tqcHistoryTestControlLots HTCL INNER JOIN tqcHistoryTestSamples HTS ON HTCL.QCTestSampleID = HTS.QCTestSampleID " & vbCrLf & _
        '                                                                              " INNER JOIN tqcHistoryControlLots HCL ON HTCL.QCControlLotID = HCL.QCControlLotID " & vbCrLf & _
        '                                        " WHERE  HTS.TestID            = " & pTestID & vbCrLf & _
        '                                        " AND    HTS.SampleType        = '" & pSampleType & "' " & vbCrLf & _
        '                                        " AND    HTS.DeletedSampleType = 0 " & vbCrLf & _
        '                                        " AND    HTS.DeletedTest       = 0 " & vbCrLf & _
        '                                        " AND    HCL.ControlID         = " & pControlID & vbCrLf & _
        '                                        " AND    HCL.LotNumber         = N'" & pLotNumber.Replace("'", "''") & "' " & vbCrLf & _
        '                                        " AND    HCL.ClosedLot         = 0 " & vbCrLf & _
        '                                        " AND    HCL.DeletedControl    = 0 "

        '                Dim myHistoryTestControlLotsDS As New HistoryTestControlLotsDS
        '                Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
        '                    Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
        '                        dbDataAdapter.Fill(myHistoryTestControlLotsDS.tqcHistoryTestControlLots)
        '                    End Using
        '                End Using

        '                resultData.SetDatos = myHistoryTestControlLotsDS
        '                resultData.HasError = False
        '            End If
        '        End If
        '    Catch ex As Exception
        '        resultData = New GlobalDataTO
        '        resultData.HasError = True
        '        resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        resultData.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, " tqcHistoryTestControlLotsDAO.GetQCIDsForTestAndControl", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return resultData
        'End Function
#End Region
    End Class
End Namespace

