Option Strict On
Option Explicit On

Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global
Imports System.Data.SqlClient

Partial Public Class tparReagentsDAO
      

    ''' <summary>
    ''' Add new Reagents
    ''' </summary>
    ''' <param name="pDBConnection">Open DB Connection</param>
    ''' <param name="pReagentsDS">Typed DataSet ReagentsDS containing the data of the Reagents to add</param>
    ''' <returns></returns>
    ''' <remarks>
    ''' Created by:  TR 10/03/2010
    ''' Modified by: SA 27/10/2010 - Added N preffix for multilanguage of field TS_User
    '''              AG 02/08/2011 -remove LotNumber and ExpirationDate AND add CodeTest and ReagentNumber
    '''              TR 21/02/2013 - Add the columns PreloadedReagent use on the update process.
    ''' 
    ''' </remarks>
    Public Function Create(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pReagentsDS As ReagentsDS) As GlobalDataTO
        Dim myGlobalDataTO As New GlobalDataTO

        Try
            If (pDBConnection Is Nothing) Then
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
            Else
                Dim cmdText As String = ""
                'AG 02/08/2011
                'Dim keys As String = " (ReagentName, LotNumber, ExpirationDate ,TS_User,TS_DateTime) "
                Dim keys As String = " (ReagentName, CodeTest, ReagentNumber, PreloadedReagent, TS_User,TS_DateTime) "

                Dim cmd As New SqlCommand
                cmd.Connection = pDBConnection

                Dim values As String = ""
                For Each reagentRow As ReagentsDS.tparReagentsRow In pReagentsDS.tparReagents.Rows
                    values = ""
                    values &= " N'" & reagentRow.ReagentName.Trim.Replace("'", "''") & "', "

                    'AG 02/08/2011
                    'If (reagentRow.IsLotNumberNull) Then
                    '    values &= " NULL, "
                    'Else
                    '    values &= " N'" & reagentRow.LotNumber.Trim.Replace("'", "''") & "', "
                    'End If

                    'If (reagentRow.IsExpirationDateNull) Then
                    '    values &= " NULL, "
                    'Else
                    '    values &= " '" & reagentRow.ExpirationDate.ToString("yyyyMMdd HH:mm:ss") & "', "
                    'End If
                    If (reagentRow.IsCodeTestNull) Then
                        values &= " NULL, "
                    Else
                        values &= " '" & reagentRow.CodeTest.ToString & "', "
                    End If

                    If (reagentRow.IsReagentNumberNull) Then
                        values &= " NULL, "
                    Else
                        values &= " " & reagentRow.ReagentNumber.ToString & ", "
                    End If
                    'AG 02/08/2011

                    'TR 21/02/2013
                    If (reagentRow.IsPreloadedReagentNull) Then
                        values &= " NULL, "
                    Else
                        values &= " '" & reagentRow.PreloadedReagent.ToString() & "', "
                    End If

                    If (reagentRow.IsTS_UserNull) Then
                        'Dim myGlobalbase As New GlobalBase
                        values &= " N'" & GlobalBase.GetSessionInfo.UserName.Trim.Replace("'", "''") & "', "
                    Else
                        values &= " N'" & reagentRow.TS_User & "', "
                    End If

                    If (reagentRow.IsTS_DateTimeNull) Then
                        values &= "'" & Now.ToString("yyyyMMdd HH:mm:ss") & "' "
                    Else
                        values &= "'" & reagentRow.TS_DateTime.ToString("yyyyMMdd HH:mm:ss") & "' "
                    End If

                    cmdText = "INSERT INTO tparReagents " & keys & " VALUES (" & values & ")"

                    cmd.CommandText = cmdText
                    cmd.CommandText &= " SELECT SCOPE_IDENTITY()"

                    reagentRow.BeginEdit()
                    reagentRow.ReagentID = CType(cmd.ExecuteScalar, Integer)
                    reagentRow.EndEdit()

                    myGlobalDataTO.AffectedRecords += 1
                    If (myGlobalDataTO.AffectedRecords > 0) Then
                        myGlobalDataTO.HasError = False
                    Else
                        myGlobalDataTO.HasError = True
                        Exit For
                    End If
                Next
            End If
        Catch ex As Exception
            myGlobalDataTO.HasError = True
            myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myGlobalDataTO.ErrorMessage = ex.Message

            'Dim myLogAcciones As New ApplicationLogManager()
            GlobalBase.CreateLogActivity(ex.Message, "tparReagentsDAO.Create", EventLogEntryType.Error, False)
        End Try
        Return myGlobalDataTO
    End Function

    ''' <summary>
    ''' Get basic data of the specified Reagent
    ''' </summary>
    ''' <param name="pDBConnection">Open DB Connection</param>
    ''' <param name="pReagentID">Reagent Identifier</param>
    ''' <returns>GlobalDataTO containing a typed DataSet ReagentDS with data of the informed Reagent</returns>
    ''' <remarks>
    ''' Created by:  DL 21/01/2010
    ''' Modified by: DL 20/05/2010 Add field InUse
    ''' AG 02/08/2011 - remove lotnumber + expirationdate and add codetest + reagentnumber
    ''' </remarks>
    Public Function Read(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pReagentID As Integer) As GlobalDataTO
        Dim resultData As New GlobalDataTO
        Dim reagentsDataDS As New ReagentsDS
        Dim dbConnection As New SqlClient.SqlConnection

        Try
            resultData = GetOpenDBConnection(pDBConnection)
            If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                If (Not dbConnection Is Nothing) Then
                    Dim cmdText As String = ""
                    cmdText &= " SELECT ReagentID, ReagentName, CodeTest, ReagentNumber, InUse, IsShared, TS_User, TS_DateTime " & vbCrLf
                    cmdText &= " FROM   tparReagents " & vbCrLf
                    cmdText &= " WHERE  ReagentID = " & pReagentID.ToString

                    Dim dbCmd As New SqlClient.SqlCommand
                    dbCmd.Connection = dbConnection
                    dbCmd.CommandText = cmdText

                    'Fill the DataSet to return 
                    Dim dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                    dbDataAdapter.Fill(reagentsDataDS.tparReagents)

                    resultData.SetDatos = reagentsDataDS
                    resultData.HasError = False
                End If
            End If
        Catch ex As Exception
            resultData.HasError = True
            resultData.ErrorCode = "SYSTEM_ERROR"
            resultData.ErrorMessage = ex.Message

            'Dim myLogAcciones As New ApplicationLogManager()
            GlobalBase.CreateLogActivity(ex.Message, "tparReagentsDAO.Read", EventLogEntryType.Error, False)
        Finally
            If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
        End Try
        Return resultData
    End Function


    ''' <summary>
    ''' Get reagen data by reagent name
    ''' </summary>
    ''' <param name="pDBConnection">Open DB Connection</param>
    ''' <param name="pReagentName">Reagent Namer</param>
    ''' <returns>GlobalDataTO containing a typed DataSet ReagentDS with data of the informed Reagent</returns>
    ''' <remarks>
    ''' Created by:  TR 30/03/2011
    ''' AG 02/08/2011 - remove lotnumber + expirationdate and add codetest + reagentnumber
    ''' Modify AG 13/03/2014 - #1538 fix issue when name contains char ' (use .Replace("'", "''"))
    ''' </remarks>
    Public Function ReadByReagentName(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pReagentName As String) As GlobalDataTO
        Dim myGlobalDataTO As New GlobalDataTO
        Dim reagentsDataDS As New ReagentsDS
        Dim dbConnection As New SqlClient.SqlConnection

        Try
            myGlobalDataTO = GetOpenDBConnection(pDBConnection)
            If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                dbConnection = CType(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                If (Not dbConnection Is Nothing) Then
                    Dim cmdText As String = ""
                    cmdText &= " SELECT ReagentID, ReagentName, CodeTest, ReagentNumber, InUse, IsShared, TS_User, TS_DateTime " & vbCrLf
                    cmdText &= " FROM   tparReagents " & vbCrLf
                    cmdText &= " WHERE  ReagentName = N'" & pReagentName.Replace("'", "''") & "'"

                    Dim dbCmd As New SqlClient.SqlCommand
                    dbCmd.Connection = dbConnection
                    dbCmd.CommandText = cmdText

                    'Fill the DataSet to return 
                    Dim dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                    dbDataAdapter.Fill(reagentsDataDS.tparReagents)

                    myGlobalDataTO.SetDatos = reagentsDataDS
                    myGlobalDataTO.HasError = False
                End If
            End If

        Catch ex As Exception
            myGlobalDataTO.HasError = True
            myGlobalDataTO.ErrorCode = "SYSTEM_ERROR"
            myGlobalDataTO.ErrorMessage = ex.Message

            'Dim myLogAcciones As New ApplicationLogManager()
            GlobalBase.CreateLogActivity(ex.Message, "tparReagentsDAO.ReadByReagentName", EventLogEntryType.Error, False)
        Finally
            If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
        End Try

        Return myGlobalDataTO
    End Function

    ''' <summary>
    ''' Update data of the specified Reagents
    ''' </summary>
    ''' <param name="pDBConnection">Open DB Connection</param>
    ''' <param name="pReagentsDS">>Typed DataSet ReagentsDS containing the data of the Reagents to update</param>
    ''' <returns></returns>
    ''' <remarks>
    ''' Created by:  TR 10/03/2010
    ''' Modified by: SA 27/10/2010 - Added N preffix for multilanguage of field TS_User. Other changes: fields ReagentName and
    '''                              TS_User are mandatory and they do not allow Null values; added Replace for string fields;
    '''                              removed the error raising depending of number of affected records
    ''' </remarks>
    Public Function Update(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pReagentsDS As ReagentsDS) As GlobalDataTO

        Dim myGlobalDataTO As New GlobalDataTO
        Try
            If (pDBConnection Is Nothing) Then
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
            Else
                Dim cmdText As String = ""
                Dim values As String = ""

                Dim cmd As New SqlCommand
                cmd.Connection = pDBConnection

                For Each reagentRow As ReagentsDS.tparReagentsRow In pReagentsDS.tparReagents.Rows
                    values = ""
                    values &= " ReagentName = N'" & reagentRow.ReagentName.Trim.Replace("'", "''") & "', "

                    'AG 02/08/2011
                    'values &= " LotNumber = "
                    'If (reagentRow.IsLotNumberNull) Then
                    '    values &= " NULL, "
                    'Else
                    '    values &= " N'" & reagentRow.LotNumber.Trim.Replace("'", "''") & "', "
                    'End If

                    'values &= " ExpirationDate = "
                    'If (reagentRow.IsExpirationDateNull) Then
                    '    values &= " NULL, "
                    'Else
                    '    values &= " '" & reagentRow.ExpirationDate.ToString("yyyyMMdd HH:mm:ss") & "', "
                    'End If

                    values &= " TS_User = "
                    If (reagentRow.IsTS_UserNull) Then
                        'Dim myGlobalbase As New GlobalBase
                        values &= " N'" & GlobalBase.GetSessionInfo.UserName.Trim.Replace("'", "''") & "', "
                    Else
                        values &= " N'" & reagentRow.TS_User.Trim.Replace("'", "''") & "', "
                    End If

                    values &= " TS_DateTime = "
                    If (reagentRow.IsTS_DateTimeNull) Then
                        values &= " '" & Now.ToString("yyyyMMdd HH:mm:ss") & "' "
                    Else
                        values &= " '" & reagentRow.TS_DateTime.ToString("yyyyMMdd HH:mm:ss") & "' "
                    End If

                    cmdText = " UPDATE tparReagents SET " & values & _
                              " WHERE  ReagentID = " & reagentRow.ReagentID.ToString()

                    cmd.CommandText = cmdText
                    cmd.Connection = pDBConnection

                    myGlobalDataTO.AffectedRecords = cmd.ExecuteNonQuery()
                    'If myGlobalDataTO.AffectedRecords > 0 Then
                    '    myGlobalDataTO.HasError = False
                    'Else
                    '    myGlobalDataTO.HasError = True
                    'End If
                Next
            End If
        Catch ex As Exception
            myGlobalDataTO.HasError = True
            myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myGlobalDataTO.ErrorMessage = ex.Message

            'Dim myLogAcciones As New ApplicationLogManager()
            GlobalBase.CreateLogActivity(ex.Message, "tparReagentsDAO.Update", EventLogEntryType.Error, False)
        End Try
        Return myGlobalDataTO
    End Function

    ''' <summary>
    ''' Delete the specified Reagent
    ''' </summary>
    ''' <param name="pDBConnection">Open DB Connection</param>
    ''' <param name="pReagentID">Reagent Identifier</param>
    ''' <returns>GlobalDataTO conatining success/error information</returns>
    ''' <remarks>
    ''' Created by: TR 15/03/2010
    ''' modified by: TR 12/09/2011 -Do not allow to remove Preloaded Reagents.
    '''              TR 14/02/2013 -The preloaded reagents belong to preloaded test
    '''                              in this case the reagent can not be delete if the
    '''                              test is removed. 
    ''' </remarks>
    Public Function Delete(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pReagentID As Integer) As GlobalDataTO
        Dim myGlobalDataTO As New GlobalDataTO
        Try
            If (pDBConnection Is Nothing) Then
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
            Else
                ''Dim myGlobalbase As New GlobalBase

                Dim cmdText As String = ""
                cmdText = " DELETE FROM tparReagents  "
                cmdText &= " WHERE ReagentID = " & pReagentID.ToString()
                'TR 14/02/2013 Commented
                'cmdText &= " AND PreloadedReagent = 0 " 'TR 12/09/2011 'Not preloaded values



                Dim cmd As New SqlCommand
                cmd.Connection = pDBConnection
                cmd.CommandText = cmdText

                myGlobalDataTO.AffectedRecords += cmd.ExecuteNonQuery()
            End If
        Catch ex As Exception
            myGlobalDataTO.HasError = True
            myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myGlobalDataTO.ErrorMessage = ex.Message

            'Dim myLogAcciones As New ApplicationLogManager()
            GlobalBase.CreateLogActivity(ex.Message, "tparReagentsDAO.Delete", EventLogEntryType.Error, False)
        End Try
        Return myGlobalDataTO
    End Function

    ''' <summary>
    ''' Set value of flag InUse for all Reagents needed for the Tests added/removed from the Active WorkSession 
    ''' </summary>
    ''' <param name="pDBConnection">Open DB Connection</param>
    ''' <param name="pWorkSessionID">Work Session Identifier</param>
    ''' <param name="pAnalyzerID">Analyzer Identifier</param>
    ''' <param name="pFlag">Value of the InUse Flag to set</param>
    ''' <param name="pUpdateForExcluded">Optional parameter. When True, it means the InUse will be set to False
    '''                                  only for Reagents of Tests that have been excluded from the active WorkSession</param>
    ''' <returns>GlobalDataTO containing success/error information</returns>
    ''' <remarks>
    ''' Created by:  GDS 10/05/2010 
    ''' Modified by: SA  09/06/2010 - Change the Query. To set InUse=TRUE, the current query works only for positioned Reagents, 
    '''                               and it should set both, positioned and not positioned Reagents. Added new optional parameter
    '''                               to reuse this method to set InUse=False for Reagents of Tests that have been excluded from
    '''                               the active WorkSession. Added parameter for the AnalyzerID 
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
                    cmdText = " UPDATE tparReagents " & _
                              " SET    InUse = " & Convert.ToInt32(IIf(pFlag, 1, 0)) & _
                              " WHERE  ReagentID IN (SELECT TR.ReagentID " & _
                                                   " FROM   tparTestReagents TR INNER JOIN twksOrderTests OT ON TR.TestID = OT.TestID " & _
                                                                              " INNER JOIN twksWSOrderTests WSOT ON OT.OrderTestID = WSOT.OrderTestID " & _
                                                                              " INNER JOIN twksOrders O ON OT.OrderID = O.OrderID " & _
                                                   " WHERE  WSOT.WorkSessionID = '" & pWorkSessionID & "' " & _
                                                   " AND    OT.AnalyzerID = '" & pAnalyzerID.Trim & "' " & _
                                                   " AND    O.SampleClass = 'BLANK') "
                Else
                    cmdText = " UPDATE tparReagents " & _
                              " SET    InUse = " & Convert.ToInt32(IIf(pFlag, 1, 0)) & _
                              " WHERE  ReagentID NOT IN (SELECT TR.ReagentID " & _
                                                       " FROM   tparTestReagents TR INNER JOIN twksOrderTests OT ON TR.TestID = OT.TestID " & _
                                                                                  " INNER JOIN twksWSOrderTests WSOT ON OT.OrderTestID = WSOT.OrderTestID " & _
                                                                                  " INNER JOIN twksOrders O ON OT.OrderID = O.OrderID " & _
                                                       " WHERE  WSOT.WorkSessionID = '" & pWorkSessionID & "' " & _
                                                       " AND    OT.AnalyzerID = '" & pAnalyzerID.Trim & "' " & _
                                                       " AND    O.SampleClass = 'BLANK') " & _
                              " AND    InUse = 1 "
                End If

                'cmdText = "UPDATE tparReagents" & vbCrLf & _
                '          "  SET InUse = '" & IIf(pFlag, "True", "False").ToString & "'" & vbCrLf & _
                '          "  WHERE ReagentID IN (SELECT DISTINCT ReagentID" & vbCrLf & _
                '          "                        FROM twksWSRequiredElements" & vbCrLf & _
                '          "                        WHERE WorkSessionID = '" & pWorkSession.Trim.Replace("'", "''") & "'" & vbCrLf & _
                '          "                          AND TubeContent   = 'REAGENT')"

                Dim cmd As New SqlCommand
                cmd.Connection = pDBConnection
                cmd.CommandText = cmdText

                myGlobalDataTO.AffectedRecords = cmd.ExecuteNonQuery()
            End If

        Catch ex As Exception
            myGlobalDataTO.HasError = True
            myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myGlobalDataTO.ErrorMessage = ex.Message

            'Dim myLogAcciones As New ApplicationLogManager()
            GlobalBase.CreateLogActivity(ex.Message, "tparReagentsDAO.UpdateInUseFlag", EventLogEntryType.Error, False)
        End Try
        Return myGlobalDataTO
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="pDBConnection"></param>
    ''' <param name="pCodeTest"></param>
    ''' <param name="pReagentNumber"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' Modified By: TR Change the CodeTest parameter to string type.
    ''' </remarks>
    Public Function GetByCodeTest(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pCodeTest As String, _
                                  ByVal pReagentNumber As Integer) As GlobalDataTO
        Dim resultData As GlobalDataTO = Nothing
        Dim dbConnection As SqlClient.SqlConnection = Nothing

        Try
            resultData = DAOBase.GetOpenDBConnection(pDBConnection)

            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)

                If (Not dbConnection Is Nothing) Then
                    Dim cmdText As String = ""
                    cmdText &= " SELECT ReagentID, ReagentName, CodeTest, ReagentNumber, InUse, IsShared, TS_User, TS_DateTime " & vbCrLf
                    cmdText &= " FROM   tparReagents " & vbCrLf
                    cmdText &= " WHERE  CodeTest = '" & pCodeTest & "'" & vbCrLf
                    cmdText &= " AND  ReagentNumber = " & pReagentNumber

                    Dim myDataSet As New ReagentsDS
                    Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                        Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                            dbDataAdapter.Fill(myDataSet.tparReagents)
                        End Using
                    End Using

                    resultData.SetDatos = myDataSet
                    resultData.HasError = False
                End If
            End If

        Catch ex As Exception
            resultData = New GlobalDataTO()
            resultData.HasError = True
            resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
            resultData.ErrorMessage = ex.Message

            'Dim myLogAcciones As New ApplicationLogManager()
            GlobalBase.CreateLogActivity(ex.Message, "tparReagentsDAO.GetByCodeTest", EventLogEntryType.Error, False)

        Finally
            If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()

        End Try

        Return resultData
    End Function

    ''' <summary>
    ''' Get the maximum value of field ReagentID in table tparReagents. Used in the Update Version process to assign a suitable temporary ReagentID
    ''' to new Reagents (because function PrepareTestToSave needs it)
    ''' </summary>
    ''' <param name="pDBConnection">Open DB Connection</param>
    ''' <returns>GlobalDataTO containing an integer value with the maximum value of field ReagentID </returns>
    ''' <remarks>
    ''' Created by:  SA 09/10/20014 - BA-1944 
    ''' </remarks>
    Public Function GetMaxReagentID(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
        Dim resultData As GlobalDataTO = Nothing
        Dim dbConnection As SqlClient.SqlConnection = Nothing

        Try
            resultData = GetOpenDBConnection(pDBConnection)
            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                If (Not dbConnection Is Nothing) Then
                    Dim cmdText As String = " SELECT MAX(ReagentID) AS MaxReagentID " & vbCrLf & _
                                            " FROM   tparReagents  " & vbCrLf 

                    Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                        resultData.SetDatos = dbCmd.ExecuteScalar()
                        resultData.HasError = False
                    End Using
                End If
            End If
        Catch ex As Exception
            resultData = New GlobalDataTO()
            resultData.HasError = True
            resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            resultData.ErrorMessage = ex.Message

            'Dim myLogAcciones As New ApplicationLogManager()
            GlobalBase.CreateLogActivity(ex.Message, "tparReagentsDAO.GetMaxReagentID", EventLogEntryType.Error, False)
        Finally
            If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
        End Try
        Return resultData
    End Function
End Class
