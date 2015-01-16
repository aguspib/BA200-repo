Option Strict On
Option Explicit On

Imports System.Data.SqlClient
Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global

Partial Public Class tparControlsDAO
    Inherits DAOBase

#Region "CRUD Methods"

    ''' <summary>
    ''' Create a new Control
    ''' </summary>
    ''' <param name="pDBConnection">Open DB Connection</param>
    ''' <param name="pControlDS">Typed DataSet ControlDS with data of the Control to add</param>
    ''' <returns>GlobalDataTO containing a typed DataSet ControlDS with all data of the added control or error information
    ''' </returns>
    ''' <remarks>
    ''' Created by:  DL 31/03/2011
    ''' Modified by: SA 12/05/2011 - Changed the SQL: ExpirationDate and ActivationDate cannot be Nulls; removed InUse field
    '''              XB 01/09/2014 - add ControlLevel field - BA #1868
    ''' </remarks>
    Public Function Create(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pControlDS As ControlsDS) As GlobalDataTO
        Dim resultData As New GlobalDataTO

        Try
            If (pDBConnection Is Nothing) Then
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
            Else
                Dim cmdText As String = " INSERT INTO tparControls (ControlName, SampleType, LotNumber, ActivationDate, ExpirationDate, " & vbCrLf & _
                                                                  " TS_User, TS_DateTime, ControlLevel) " & vbCrLf & _
                                        " VALUES (N'" & pControlDS.tparControls(0).ControlName.ToString.Replace("'", "''") & "', " & vbCrLf & _
                                                  "'" & pControlDS.tparControls(0).SampleType & "', " & vbCrLf & _
                                                 "N'" & pControlDS.tparControls(0).LotNumber.ToString.Replace("'", "''") & "', " & vbCrLf & _
                                                  "'" & pControlDS.tparControls(0).ActivationDate.ToString("yyyyMMdd HH:mm:ss") & "', " & vbCrLf & _
                                                  "'" & pControlDS.tparControls(0).ExpirationDate.ToString("yyyyMMdd HH:mm:ss") & "', " & vbCrLf

                If (String.IsNullOrEmpty(pControlDS.tparControls(0).TS_User.ToString)) Then
                    'Get the connected Username from the current Application Session
                    Dim currentSession As New GlobalBase
                    cmdText &= "N'" & GlobalBase.GetSessionInfo().UserName.Replace("'", "''") & "', " & vbCrLf
                Else
                    cmdText &= "N'" & pControlDS.tparControls(0).TS_User.Trim.Replace("'", "''") & "', " & vbCrLf
                End If

                If (String.IsNullOrEmpty(pControlDS.tparControls(0).TS_DateTime.ToString)) Then
                    'Get the current DateTime
                    cmdText &= "'" & Now.ToString("yyyyMMdd HH:mm:ss") & "', " & vbCrLf
                Else
                    cmdText &= "'" & pControlDS.tparControls(0).TS_DateTime.ToString("yyyyMMdd HH:mm:ss") & "', " & vbCrLf
                End If

                cmdText &= " " & pControlDS.tparControls(0).ControlLevel & ") " & vbCrLf

                'Finally, get the automatically generated ID for the created control
                cmdText &= " SELECT SCOPE_IDENTITY()"

                Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                    Dim newControlID As Integer
                    newControlID = CType(dbCmd.ExecuteScalar(), Integer)

                    If (newControlID > 0) Then
                        pControlDS.tparControls(0).BeginEdit()
                        pControlDS.tparControls(0).SetField("ControlID", newControlID)
                        pControlDS.tparControls(0).EndEdit()

                        resultData.HasError = False
                        resultData.AffectedRecords = 1
                        resultData.SetDatos = pControlDS
                    Else
                        resultData.HasError = True
                        resultData.AffectedRecords = 0
                    End If
                End Using
            End If
        Catch ex As Exception
            resultData.HasError = True
            resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            resultData.ErrorMessage = ex.Message

            Dim myLogAcciones As New ApplicationLogManager()
            GlobalBase.CreateLogActivity(ex.Message, "tparControlsDAO.Create", EventLogEntryType.Error, False)
        End Try
        Return resultData
    End Function

    ''' <summary>
    ''' Delete the specified Control 
    ''' </summary>
    ''' <param name="pDBConnection">Open DB Connection</param>
    ''' <param name="pControlID">Identifier of the Control to delete</param>        
    ''' <returns>GlobalDataTO containing sucess/error information</returns>
    ''' <remarks>
    ''' Created by:  DL 31/03/2011
    ''' Modified by: SA 10/05/2011 - Removed parameter and query filter by SampleType
    ''' </remarks>
    Public Function Delete(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pControlID As Integer) As GlobalDataTO
        Dim resultData As New GlobalDataTO

        Try
            If (pDBConnection Is Nothing) Then
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
            Else
                Dim cmdText As String = " DELETE tparControls " & vbCrLf & _
                                        " WHERE  ControlID = " & pControlID.ToString & vbCrLf

                Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                    resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                    resultData.HasError = False
                End Using
            End If
        Catch ex As Exception
            resultData.HasError = True
            resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            resultData.ErrorMessage = ex.Message

            Dim myLogAcciones As New ApplicationLogManager()
            GlobalBase.CreateLogActivity(ex.Message, "tparControlsDAO.Delete", EventLogEntryType.Error, False)
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
    Public Function Read(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pControlID As Integer) As GlobalDataTO
        Dim resultData As GlobalDataTO = Nothing
        Dim dbConnection As SqlClient.SqlConnection = Nothing

        Try
            resultData = GetOpenDBConnection(pDBConnection)
            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                If (Not dbConnection Is Nothing) Then
                    Dim cmdText As String = " SELECT * FROM tparControls " & vbCrLf & _
                                            " WHERE  ControlID = " & pControlID.ToString & vbCrLf

                    Dim myControlData As New ControlsDS
                    Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                        Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                            dbDataAdapter.Fill(myControlData.tparControls)
                        End Using
                    End Using

                    resultData.SetDatos = myControlData
                    resultData.HasError = False
                End If
            End If
        Catch ex As Exception
            resultData = New GlobalDataTO()
            resultData.HasError = True
            resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            resultData.ErrorMessage = ex.Message

            Dim myLogAcciones As New ApplicationLogManager()
            GlobalBase.CreateLogActivity(ex.Message, "tparControlsDAO.Read", EventLogEntryType.Error, False)
        Finally
            If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
        End Try
        Return resultData
    End Function

    ''' <summary>
    ''' Get the list of all defined Controls
    ''' </summary>
    ''' <param name="pDBConnection">Open DB Connection</param>
    ''' <returns>GlobalDataTO containing a typed DataSet ControlDS with the list of all defined Controls</returns>
    ''' <remarks>
    ''' Created by:  DL 14/04/2011
    ''' Modified by: SA 10/05/2011 - Removed join with table of tparPreviousControlLots 
    ''' </remarks>
    Public Function ReadAll(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
        Dim resultData As GlobalDataTO = Nothing
        Dim dbConnection As SqlClient.SqlConnection = Nothing

        Try
            resultData = GetOpenDBConnection(pDBConnection)
            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                If (Not dbConnection Is Nothing) Then
                    Dim cmdText As String = " SELECT * FROM tparControls ORDER BY ControlName "

                    Dim myControls As New ControlsDS
                    Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                        Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                            dbDataAdapter.Fill(myControls.tparControls)
                        End Using
                    End Using

                    resultData.SetDatos = myControls
                    resultData.HasError = False
                End If
            End If
        Catch ex As Exception
            resultData = New GlobalDataTO()
            resultData.HasError = True
            resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            resultData.ErrorMessage = ex.Message

            Dim myLogAcciones As New ApplicationLogManager()
            GlobalBase.CreateLogActivity(ex.Message, "tparControlsDAO.ReadAll", EventLogEntryType.Error, False)
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
    ''' Created by  DL 14/04/2011
    ''' Modified by XB 01/02/2013 - Upper conversions must be implemented in same environment (f.ex.SQL)  (Bugs tracking #1112)
    ''' </remarks>
    Public Function ReadByControlName(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pControlName As String, _
                                      Optional ByVal pControlID As Integer = -1) As GlobalDataTO
        Dim resultData As GlobalDataTO = Nothing
        Dim dbConnection As SqlClient.SqlConnection = Nothing

        Try
            resultData = GetOpenDBConnection(pDBConnection)
            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                If (Not dbConnection Is Nothing) Then
                    Dim cmdText As String = " SELECT ControlID FROM tparControls " & vbCrLf & _
                                            " WHERE UPPER(ControlName) = UPPER(N'" & pControlName.Trim.Replace("'", "''") & "') " & vbCrLf
                    ' " WHERE UPPER(ControlName) = N'" & pControlName.Trim.ToUpper.Replace("'", "''") & "' " & vbCrLf

                    'If a ControlID has been informed, exclude it from the query
                    If (pControlID > 0) Then cmdText &= " AND ControlID <> " & pControlID.ToString

                    Dim myControlsDS As New ControlsDS
                    Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                        Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                            dbDataAdapter.Fill(myControlsDS.tparControls)
                        End Using
                    End Using

                    resultData.SetDatos = (myControlsDS.tparControls.Rows.Count > 0)
                    resultData.HasError = False
                End If
            End If
        Catch ex As Exception
            resultData = New GlobalDataTO()
            resultData.HasError = True
            resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            resultData.ErrorMessage = ex.Message

            Dim myLogAcciones As New ApplicationLogManager()
            GlobalBase.CreateLogActivity(ex.Message, "tparControlsDAO.ReadByControlName", EventLogEntryType.Error, False)
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
    Public Function ReadByOrderTestID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOrderTestID As Integer) As GlobalDataTO
        Dim resultData As GlobalDataTO = Nothing
        Dim dbConnection As SqlClient.SqlConnection = Nothing

        Try
            resultData = GetOpenDBConnection(pDBConnection)
            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                If (Not dbConnection Is Nothing) Then
                    Dim cmdText As String = " SELECT C.ControlName, C.LotNumber " & vbCrLf & _
                                            " FROM   twksOrderTests OT INNER JOIN tparControls C ON OT.ControlID = C.ControlID " & vbCrLf & _
                                            " WHERE  OT.OrderTestID = " & pOrderTestID & vbCrLf

                    Dim controlDataDS As New ControlsDS
                    Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                        Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                            dbDataAdapter.Fill(controlDataDS.tparControls)
                        End Using
                    End Using

                    resultData.SetDatos = controlDataDS
                    resultData.HasError = False
                End If
            End If
        Catch ex As Exception
            resultData = New GlobalDataTO()
            resultData.HasError = True
            resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            resultData.ErrorMessage = ex.Message

            Dim myLogAcciones As New ApplicationLogManager()
            GlobalBase.CreateLogActivity(ex.Message, "tparControlsDAO.ReadByOrderTestID", EventLogEntryType.Error, False)
        Finally
            If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
        End Try
        Return resultData
    End Function

    ''' <summary>
    ''' Update data of an specific Control
    ''' </summary>
    ''' <param name="pDBConnection">Open DB Connection</param>
    ''' <param name="pControlDS">Typed DataSet controlDS with data of the Control</param>
    ''' <returns>GlobalDataTO containing a typed DataSet ControlsDS with all data of the modified controls or error information</returns>
    ''' <remarks>
    ''' Created by:  DL 31/03/2011
    ''' Modified by: SA 12/05/2011 - Changed the SQL: SampleType cannot be Null; removed InUse field.
    '''              XB 01/09/2014 - add ControlLevel field - BA #1868
    ''' </remarks>
    Public Function Update(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pControlDS As ControlsDS) As GlobalDataTO
        Dim resultData As New GlobalDataTO

        Try
            If (pDBConnection Is Nothing) Then
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
            Else
                Dim cmdText As String = " UPDATE tparControls " & vbCrLf & _
                                        " SET    ControlName = N'" & pControlDS.tparControls(0).ControlName.ToString.Replace("'", "''") & "', " & vbCrLf & _
                                               " SampleType  =  '" & pControlDS.tparControls(0).SampleType & "', " & vbCrLf & _
                                               " LotNumber   = N'" & pControlDS.tparControls(0).LotNumber.ToString.Replace("'", "''") & "', " & vbCrLf & _
                                               " ExpirationDate = '" & pControlDS.tparControls(0).ExpirationDate.ToString("yyyyMMdd HH:mm:ss") & "', " & vbCrLf & _
                                               " ActivationDate = '" & pControlDS.tparControls(0).ActivationDate.ToString("yyyyMMdd HH:mm:ss") & "', " & vbCrLf

                If (pControlDS.tparControls(0).IsTS_UserNull) Then
                    'Get the connected Username from the current Application Session
                    Dim currentSession As New GlobalBase
                    cmdText &= " TS_User = N'" & GlobalBase.GetSessionInfo().UserName.Trim.Replace("'", "''") & "', " & vbCrLf
                Else
                    cmdText &= " TS_User = N'" & pControlDS.tparControls(0).TS_User.Replace("'", "''") & "', " & vbCrLf
                End If

                If (pControlDS.tparControls(0).IsTS_DateTimeNull) Then
                    cmdText &= " TS_DateTime = '" & Now.ToString("yyyyMMdd HH:mm:ss") & "', " & vbCrLf
                Else
                    cmdText &= " TS_DateTime = '" & pControlDS.tparControls(0).TS_DateTime.ToString("yyyyMMdd HH:mm:ss") & "', " & vbCrLf
                End If

                cmdText &= " ControlLevel = " & pControlDS.tparControls(0).ControlLevel.ToString & " " & vbCrLf

                cmdText &= " WHERE ControlID = " & pControlDS.tparControls(0).ControlID & vbCrLf

                Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                    resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                    resultData.HasError = False
                End Using
            End If
        Catch ex As Exception
            resultData.HasError = True
            resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            resultData.ErrorMessage = ex.Message

            Dim myLogAcciones As New ApplicationLogManager()
            GlobalBase.CreateLogActivity(ex.Message, "tparControlsDAO.Update", EventLogEntryType.Error, False)
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
    ''' Modified by: SA  09/06/2010 - Change the Query. To set InUse=TRUE, the current query works only for positioned Controls, 
    '''                               and it should set both, positioned and not positioned Controls. Added new optional parameter
    '''                               to reuse this method to set InUse=False for Controls that have been excluded from
    '''                               the active WorkSession. Added parameter for the AnalyzerID  
    '''              SA  14/06/2012 - Changed the SQL: the INNER JOIN between tparTestControls and twksOrderTests has to be also 
    '''                               by TestType
    ''' </remarks>
    Public Function UpdateInUseFlag(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, _
                                    ByVal pAnalyzerID As String, ByVal pFlag As Boolean, _
                                    Optional ByVal pUpdateForExcluded As Boolean = False) As GlobalDataTO
        Dim myGlobalDataTO As New GlobalDataTO

        Try
            If (pDBConnection Is Nothing) Then
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
            Else
                Dim cmdText As String
                If (Not pUpdateForExcluded) Then
                    cmdText = " UPDATE tparControls " & vbCrLf & _
                              " SET    InUse = " & Convert.ToInt32(IIf(pFlag, 1, 0)) & vbCrLf & _
                              " WHERE  ControlID IN (SELECT TC.ControlID " & vbCrLf & _
                                                   " FROM   tparTestControls TC INNER JOIN twksOrderTests OT ON TC.TestType = OT.TestType AND TC.TestID = OT.TestID AND TC.SampleType = OT.SampleType " & vbCrLf & _
                                                                              " INNER JOIN twksWSOrderTests WSOT ON OT.OrderTestID = WSOT.OrderTestID " & vbCrLf & _
                                                                              " INNER JOIN twksOrders O ON OT.OrderID = O.OrderID " & vbCrLf & _
                                                   " WHERE  WSOT.WorkSessionID = '" & pWorkSessionID.Trim & "' " & vbCrLf & _
                                                   " AND    OT.AnalyzerID = '" & pAnalyzerID.Trim & "' " & vbCrLf & _
                                                   " AND    O.SampleClass = 'CTRL') " & vbCrLf
                Else
                    cmdText = " UPDATE tparControls " & vbCrLf & _
                              " SET    InUse = 0 " & vbCrLf & _
                              " WHERE  ControlID NOT IN (SELECT TC.ControlID " & vbCrLf & _
                                                       " FROM   tparTestControls TC INNER JOIN twksOrderTests OT ON TC.TestType = OT.TestType AND TC.TestID = OT.TestID AND TC.SampleType = OT.SampleType " & vbCrLf & _
                                                                                  " INNER JOIN twksWSOrderTests WSOT ON OT.OrderTestID = WSOT.OrderTestID " & vbCrLf & _
                                                                                  " INNER JOIN twksOrders O ON OT.OrderID = O.OrderID " & vbCrLf & _
                                                       " WHERE  WSOT.WorkSessionID = '" & pWorkSessionID.Trim & "' " & vbCrLf & _
                                                       " AND    OT.AnalyzerID = '" & pAnalyzerID.Trim & "' " & vbCrLf & _
                                                       " AND    O.SampleClass = 'CTRL') " & vbCrLf & _
                              " AND    InUse = 1 " & vbCrLf
                End If

                Using dbCmd As New SqlCommand(cmdText, pDBConnection)
                    myGlobalDataTO.AffectedRecords = dbCmd.ExecuteNonQuery()
                    myGlobalDataTO.HasError = False
                End Using
            End If
        Catch ex As Exception
            myGlobalDataTO.HasError = True
            myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myGlobalDataTO.ErrorMessage = ex.Message

            Dim myLogAcciones As New ApplicationLogManager()
            GlobalBase.CreateLogActivity(ex.Message, "tparControlsDAO.UpdateInUseFlag", EventLogEntryType.Error, False)
        End Try
        Return myGlobalDataTO
    End Function
#End Region
End Class
