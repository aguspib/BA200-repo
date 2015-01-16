Option Explicit On
Option Strict On

Imports System.Data.SqlClient
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Global.GlobalEnumerates

Namespace Biosystems.Ax00.DAL.DAO

    Partial Public Class tparTestControlsDAO
        Inherits DAOBase

#Region "CRUD Methods"
        ''' <summary>
        ''' Links a Test/SampleType/TestType to a specific Control
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestControlDS">Typed DataSet TestControlsDS with data of the Test/SampleType/TestType to link</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  DL 31/03/2011
        ''' Modified by: RH 11/06/2012 - New field TestType.
        ''' </remarks>
        Public Function CreateNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestControlDS As TestControlsDS) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString()
                Else
                    Dim row As TestControlsDS.tparTestControlsRow = pTestControlDS.tparTestControls(0)
                    Dim cmdText As String = " INSERT INTO tparTestControls (TestType, TestID, SampleType, ControlID, MinConcentration, " & vbCrLf & _
                                                                          " MaxConcentration, TargetMean,  TargetSD, ActiveControl, " & vbCrLf & _
                                                                          " TS_User, TS_DateTime) " & vbCrLf & _
                                            " VALUES('" & row.TestType.Trim & "', " & row.TestID.ToString & ", '" & row.SampleType.Trim & "', " & vbCrLf & _
                                                          row.ControlID & ", " & ReplaceNumericString(row.MinConcentration) & ", " & vbCrLf & _
                                                          ReplaceNumericString(row.MaxConcentration) & ", " & ReplaceNumericString(row.TargetMean) & ", " & vbCrLf & _
                                                          ReplaceNumericString(row.TargetSD) & ", " & IIf(row.ActiveControl, 1, 0).ToString & ", " & vbCrLf

                    If (String.IsNullOrEmpty(row.TS_User)) Then
                        'Dim currentSession As New GlobalBase
                        cmdText &= " N'" & GlobalBase.GetSessionInfo().UserName.Replace("'", "''") & "', " & vbCrLf
                    Else
                        cmdText &= " N'" & row.TS_User.Replace("'", "''") & "', " & vbCrLf
                    End If

                    If (row.IsTS_DateTimeNull) Then
                        cmdText &= " '" & Now.ToSQLString() & "') " & vbCrLf
                    Else
                        cmdText &= " '" & row.TS_DateTime.ToString("yyyyMMdd HH:mm:ss") & "') " & vbCrLf
                    End If

                    Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                        resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                        resultData.HasError = (resultData.AffectedRecords = 0)
                    End Using
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparTestControlsDAO.CreateNEW", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Delete the relation between the specified Control and Standard Test/SampleType
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pControlID">Control Identifier</param>
        ''' <param name="pTestID">Test Identifier</param>
        ''' <param name="pSampleType">Sample Type Code</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 13/05/2011
        ''' Modified by: RH 11/06/2012 - New parameter TestType.
        ''' </remarks>
        Public Function DeleteNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pControlID As Integer, _
                                  ByVal pTestID As Integer, ByVal pSampleType As String, ByVal pTestType As String) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = " DELETE tparTestControls " & vbCrLf & _
                                            " WHERE  ControlID = " & pControlID.ToString & vbCrLf & _
                                            " AND    TestID    = " & pTestID.ToString & vbCrLf & _
                                            " AND    SampleType = '" & pSampleType.Trim & "' " & vbCrLf & _
                                            " AND    TestType = '" & pTestType.Trim & "' "

                    Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                        myGlobalDataTO.AffectedRecords = dbCmd.ExecuteNonQuery()
                        myGlobalDataTO.HasError = False
                    End Using
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparTestControlsDAO.DeleteNEW", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' For the specified Control, delete all relations with Tests/SampleTypes
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pControlID">Control Identifier</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by:  DL 05/04/2011
        ''' Modified by: SA 10/05/2011 - Removed parameter and filter for Sample Type
        ''' </remarks>
        Public Function DeleteByControlID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pControlID As Integer) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = " DELETE tparTestControls " & vbCrLf & _
                                            " WHERE ControlID = " & pControlID

                    Dim cmd As New SqlCommand
                    cmd.Connection = pDBConnection
                    cmd.CommandText = cmdText

                    myGlobalDataTO.AffectedRecords = cmd.ExecuteNonQuery()
                    myGlobalDataTO.HasError = False
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparTestControlsDAO.DeleteByControlID", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Update values for a TestType/TestID/SampleType linked to an specific Control
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestControlDS">Typed DataSet TestControlsDS with data of the linked TestType/TestID/SampleType to updated</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  DL 31/03/2011
        ''' Modified by: TR 07/04/2011 - Removed updation of PK fields
        '''              TR 12/04/2011 - Added the PK fields on the WHERE specification  
        '''              SA 03/01/2012 - Update field ActiveControl only when the field is informed in the DS 
        '''              SA 06/06/2012 - Changed the query by adding a filter by field TestType
        ''' </remarks>
        Public Function UpdateNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestControlDS As TestControlsDS) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = " UPDATE tparTestControls " & vbCrLf & _
                                               " SET MinConcentration = " & ReplaceNumericString(pTestControlDS.tparTestControls(0).MinConcentration) & ", " & vbCrLf & _
                                                   " MaxConcentration = " & ReplaceNumericString(pTestControlDS.tparTestControls(0).MaxConcentration) & ", " & vbCrLf & _
                                                   " TargetMean       = " & ReplaceNumericString(pTestControlDS.tparTestControls(0).TargetMean) & ", " & vbCrLf & _
                                                   " TargetSD         = " & ReplaceNumericString(pTestControlDS.tparTestControls(0).TargetSD) & ", " & vbCrLf

                    If (Not pTestControlDS.tparTestControls(0).IsActiveControlNull) Then
                        cmdText &= " ActiveControl    = " & IIf(pTestControlDS.tparTestControls(0).ActiveControl, 1, 0).ToString & ", " & vbCrLf
                    End If

                    If (String.IsNullOrEmpty(pTestControlDS.tparTestControls(0).TS_User.ToString)) Then
                        'Dim currentSession As New GlobalBase
                        cmdText &= " TS_User = N'" & GlobalBase.GetSessionInfo().UserName.Replace("'", "''") & "', " & vbCrLf
                    Else
                        cmdText &= " TS_User = N'" & pTestControlDS.tparTestControls(0).TS_User.Trim.Replace("'", "''") & "', " & vbCrLf
                    End If

                    If (pTestControlDS.tparTestControls(0).IsTS_DateTimeNull) Then
                        cmdText &= " TS_DateTime = '" & Now.ToString("yyyyMMdd HH:mm:ss") & "' " & vbCrLf
                    Else
                        cmdText &= " TS_DateTime = '" & pTestControlDS.tparTestControls(0).TS_DateTime.ToString("yyyyMMdd HH:mm:ss") & "' " & vbCrLf
                    End If

                    cmdText &= " WHERE ControlID  = " & pTestControlDS.tparTestControls(0).ControlID.ToString() & vbCrLf & _
                               " AND   TestType   = '" & pTestControlDS.tparTestControls(0).TestType.Trim & "' " & vbCrLf & _
                               " AND   TestID     = " & pTestControlDS.tparTestControls(0).TestID.ToString() & vbCrLf & _
                               " AND   SampleType = '" & pTestControlDS.tparTestControls(0).SampleType.Trim & "' " & vbCrLf

                    Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                        resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                        resultData.HasError = False
                    End Using
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparTestControlsDAO.UpdateNEW", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

#End Region

#Region "Other Methods"
        ''' <summary>
        ''' Count the number of Controls linked to the informed Test/SampleType that are currently marked as active Controls
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestType">Test Type Code</param>
        ''' <param name="pTestID">Test Identifier</param>
        ''' <param name="pSampleType">Sample Type Code</param>
        ''' <returns>GlobalDataTO containing an integer value with the number of Controls linked to the informed TestType/Test/SampleType 
        '''          that are currently marked as active Controls</returns>
        ''' <remarks>
        ''' Created by:  SA 06/10/2011
        ''' Modified by: RH 11/06/2012 - New parameter TestType
        ''' </remarks>
        Public Function CountActiveByTestIDAndSampleTypeNEW(ByVal pDBConnection As SqlConnection, ByVal pTestType As String, ByVal pTestID As Integer, _
                                                            ByVal pSampleType As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT COUNT(*) AS NumActiveControls FROM tparTestControls " & vbCrLf & _
                                                " WHERE  TestType = '" & pTestType.Trim & "' " & vbCrLf & _
                                                " AND    TestID   = " & pTestID.ToString & vbCrLf & _
                                                " AND    SampleType = '" & pSampleType & "' " & vbCrLf & _
                                                " AND    ActiveControl = 1 " & vbCrLf

                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Dim dbDataReader As SqlClient.SqlDataReader = dbCmd.ExecuteReader()
                            If (dbDataReader.HasRows) Then
                                dbDataReader.Read()

                                If (dbDataReader.IsDBNull(0)) Then
                                    resultData.SetDatos = 0
                                Else
                                    resultData.SetDatos = Convert.ToInt32(dbDataReader.Item("NumActiveControls"))
                                End If
                            End If
                            dbDataReader.Close()
                        End Using
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparTestControlsDAO.CountActiveByTestIDAndSampleType", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Delete the relation between an Standard Test and all the Controls defined for it; optionally,
        ''' the deletion can be executed only for Controls the Test uses for the specified SampleType
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestID">Test Identifier</param>
        ''' <param name="pSampleType">Sample Type Code. Optional parameter</param>
        ''' <param name="pControlIDList">List of ControlIDs separated by (,). Optional parameter</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by:  TR 17/04/2011
        ''' Modified by: SA 11/05/2011 - Changed optional parameter pControlID for pControlIDList, a string list
        '''                              containing the list of ControlIDs that have to be deleted for the informed Test/SampleType
        ''' Modified by: RH 11/06/2012 - New parameter TestType.
        ''' </remarks>
        Public Function DeleteTestControlByTestIDNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer, _
                                                     ByVal pTestType As String, Optional ByVal pSampleType As String = "", _
                                                     Optional ByVal pControlIDList As String = "") As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = ""
                    cmdText = " DELETE FROM  tparTestControls " & _
                              " WHERE TestID = " & pTestID & vbCrLf & _
                              " AND    TestType = '" & pTestType & "' " & vbCrLf

                    'Filter data for the specified optional parameters when they are informed
                    If Not String.IsNullOrEmpty(pSampleType) Then cmdText &= " AND SampleType ='" & pSampleType & "'"

                    If Not String.IsNullOrEmpty(pControlIDList) Then cmdText &= " AND ControlID NOT IN (" & pControlIDList & ") "

                    Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                        myGlobalDataTO.AffectedRecords = dbCmd.ExecuteNonQuery()
                        myGlobalDataTO.HasError = False
                    End Using
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparTestControlsDAO.DeleteTestControlByTestID", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get the basic information of the Controls defined for the specified ISE Test and optionally, Sample Type 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestID">ISE Test Identifier</param>
        ''' <param name="pSampleType">Sample Type Code. Optional parameter</param>
        ''' <param name="pControlID">Control Identifier. Optional parameter</param>
        ''' <param name="pOnlyActiveControls">When True, it indicates only Controls linked as Active have to be returned, but only if
        '''                                   the specified ISE Test/Sample Type has the Quality Control feature enabled</param>
        ''' <returns>GlobalDataTO containing a typed DataSet TestControlsDS with data of defined Controls 
        '''          for the informed ISE Test and Sample Type</returns>
        ''' <remarks>
        ''' Created by:  SA 06/06/2012 
        ''' Modified by: XB 01/09/2014 - Get the new field ControlLevel from tparControls table - BA #1868
        ''' </remarks>
        Public Function ReadByISETestIDAndSampleType(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer, _
                                                     Optional ByVal pSampleType As String = "", Optional ByVal pControlID As Integer = 0, _
                                                     Optional ByVal pOnlyActiveControls As Boolean = False) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT TC.TestType, TC.TestID, TC.SampleType, TC.ControlID, TC.MinConcentration, TC.MaxConcentration, " & vbCrLf & _
                                                       " TC.TargetMean, TC.TargetSD, TC.ACtiveControl, C.ControlName, C.LotNumber, C.ExpirationDate, " & vbCrLf & _
                                                       " IT.Name AS TestName, ITS.ControlReplicates, ITS.Decimals AS DecimalsAllowed, C.ControlLevel " & vbCrLf & _
                                                " FROM   tparTestControls AS TC INNER JOIN tparControls AS C ON TC.ControlID = C.ControlID " & vbCrLf & _
                                                                              " INNER JOIN tparISETests AS IT ON TC.TestID = IT.ISETestID " & vbCrLf & _
                                                                              " INNER JOIN tparISETestSamples AS ITS ON TC.TestID = ITS.ISETestID AND TC.SampleType = ITS.SampleType " & vbCrLf & _
                                                " WHERE  TC.TestType = 'ISE' " & vbCrLf & _
                                                " AND    TC.TestID = " & pTestID.ToString & vbCrLf

                        If (pSampleType <> "") Then
                            cmdText &= " AND UPPER(TC.SampleType) = UPPER('" & pSampleType & "') " & vbCrLf
                        End If

                        If (pOnlyActiveControls) Then
                            cmdText &= " AND TC.ActiveControl = 1 " & vbCrLf & _
                                       " AND ITS.QCActive = 1 " & vbCrLf
                        End If

                        If (pControlID <> 0) Then
                            cmdText &= " AND TC.ControlID = " & pControlID & vbCrLf
                        End If
                        cmdText &= " ORDER BY TC.ACtiveControl DESC " & vbCrLf


                        Dim myTestControlsData As New TestControlsDS()
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myTestControlsData.tparTestControls)
                            End Using
                        End Using

                        resultData.SetDatos = myTestControlsData
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparTestControlsDAO.ReadByISETestIDAndSampleType", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get the basic information of the Controls defined for the specified Standard Test and optionally, Sample Type 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestID">Test Identifier</param>
        ''' <param name="pSampleType">Sample Type Code. Optional parameter</param>
        ''' <param name="pControlID">Control Identifier. Optional parameter</param>
        ''' <param name="pOnlyActiveControls">When True, it indicates only Controls linked as Active have to be returned, but only if
        '''                                   the specified Test/Sample Type has the Quality Control feature enabled</param>
        ''' <returns>GlobalDataTO containing a typed DataSet TestControlsDS with data of defined Controls 
        '''          for the informed Standard Test and Sample Type</returns>
        ''' <remarks>
        ''' Modified by: SA 04/03/2010 - Query was changed; it was bad done
        '''              TR 04/04/2011 - Added new optional parameter  pOnlyActiveControls
        '''                            - Get fields TargetMean, TargetSD, and ActiveControl
        '''              TR 07/04/2011 - Added optional parammeter pControlID
        '''              SA 15/04/2011 - Removed field ControlNum from the SQL Query
        '''              TR 11/05/2011 - Changed parameter pSampleType to optional
        '''              SA 21/06/2011 - When parameter pOnlyActiveControls has been set to True, filter data also by QCActive = True
        '''                              for the Test/SampleType
        '''              TR 11/10/2011 - Added the ORDER BY field ActiveControl
        '''              SA 06/06/2012 - Changed the query by adding a filter by field TestType = STD
        '''              XB 01/09/2014 - Get the new field ControlLevel from tparControls table - BA #1868
        ''' </remarks>
        Public Function ReadBySTDTestIDAndSampleType(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer, _
                                                     Optional ByVal pSampleType As String = "", Optional ByVal pControlID As Integer = 0, _
                                                     Optional ByVal pOnlyActiveControls As Boolean = False) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT TC.TestID, TC.SampleType, TC.ControlID, TC.MinConcentration, TC.MaxConcentration, " & vbCrLf & _
                                                       " TC.TargetMean, TC.TargetSD, TC.ACtiveControl, C.ControlName, C.LotNumber, C.ExpirationDate, " & vbCrLf & _
                                                       " T.TestName, TS.ControlReplicates, C.ControlLevel " & vbCrLf & _
                                                " FROM   tparTestControls AS TC INNER JOIN tparControls AS C ON TC.ControlID = C.ControlID " & vbCrLf & _
                                                                              " INNER JOIN tparTests AS T ON TC.TestID = T.TestID " & vbCrLf & _
                                                                              " INNER JOIN tparTestSamples AS TS ON TC.TestID = TS.TestID AND TC.SampleType = TS.SampleType " & vbCrLf & _
                                                " WHERE  TC.TestType = 'STD' " & vbCrLf & _
                                                " AND    TC.TestID = " & pTestID.ToString() & vbCrLf

                        If (pSampleType <> "") Then
                            cmdText &= " AND UPPER(TC.SampleType) = UPPER('" & pSampleType & "') " & vbCrLf
                        End If

                        If (pOnlyActiveControls) Then
                            cmdText &= " AND TC.ActiveControl = 1 " & vbCrLf & _
                                       " AND TS.QCActive = 1 " & vbCrLf
                        End If

                        If (pControlID <> 0) Then
                            cmdText &= " AND TC.ControlID = " & pControlID & vbCrLf
                        End If
                        cmdText &= " ORDER BY TC.ACtiveControl DESC " & vbCrLf


                        Dim myTestControlsData As New TestControlsDS()
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myTestControlsData.tparTestControls)
                            End Using
                        End Using

                        resultData.SetDatos = myTestControlsData
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparTestControlsDAO.ReadBySTDTestIDAndSampleType", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get all Tests/SampleTypes linked to the specified Control
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pControlID">Control Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet TestControlsDS with the list of all Tests/SampleTypes
        '''          linked to the specified Control</returns>
        ''' <remarks>
        ''' Created by: RH 14/06/2012
        ''' </remarks>
        Public Function ReadTestsByControlIDNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pControlID As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String
                        cmdText = " SELECT TC.*, T.TestName, T.DecimalsAllowed, T.PreloadedTest, T.InUse, T.TestPosition, " & vbCrLf & _
                                  " TS.RejectionCriteria, MD.FixedItemDesc As MeasureUnit " & vbCrLf & _
                                  " FROM tparTestControls TC INNER JOIN tparTestSamples TS ON TC.TestID = TS.TestID AND TC.SampleType = TS.SampleType " & vbCrLf & _
                                  " INNER JOIN tparTests T ON TC.TestID = T.TestID " & vbCrLf & _
                                  " INNER JOIN tcfgMasterData MD ON T.MeasureUnit = MD.ItemID " & vbCrLf & _
                                  " WHERE TC.ControlID = " & pControlID & vbCrLf & _
                                  " AND TC.TestType = 'STD'" & vbCrLf & _
                                  " AND   MD.SubtableID = '" & MasterDataEnum.TEST_UNITS.ToString() & "' " & vbCrLf & _
                                  " UNION " & _
                                  " SELECT TC.*, T.Name, TS.Decimals AS DecimalsAllowed, 1 AS PreloadedTest, T.InUse, 0 AS TestPosition, " & vbCrLf & _
                                  " TS.RejectionCriteria, MD.FixedItemDesc As MeasureUnit " & vbCrLf & _
                                  " FROM tparTestControls TC INNER JOIN tparISETestSamples TS ON TC.TestID = TS.ISETestID AND TC.SampleType = TS.SampleType " & vbCrLf & _
                                  " INNER JOIN tparISETests T ON TC.TestID = T.ISETestID " & vbCrLf & _
                                  " INNER JOIN tcfgMasterData MD ON T.Units = MD.ItemID " & vbCrLf & _
                                  " WHERE TC.ControlID = " & pControlID & vbCrLf & _
                                  " AND TC.TestType = 'ISE'" & vbCrLf & _
                                  " AND   MD.SubtableID = '" & MasterDataEnum.TEST_UNITS.ToString() & "' "

                        Dim myTestControlsData As New TestControlsDS()
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myTestControlsData.tparTestControls)
                            End Using
                        End Using

                        resultData.SetDatos = myTestControlsData
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparTestControlsDAO.ReadTestsByControlID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' From identifiers of Test/SampleType and Control/Lot in QC Module, verifies if the link between the Test/SampleType
        ''' and the Control still exists in table tparTestControls
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pQCTestSampleID">Identifier of Test/SampleType in QC Module</param>
        ''' <param name="pQCControlLotID">Identifier of Control/Lot in QC Module</param>
        ''' <returns>GlobalDataTO containing a boolean value indicating if the link between the Test/SampleType and the Control
        '''          still exists in table tparTestControls</returns>
        ''' <remarks>
        ''' Created by:  SA 04/01/2012 
        ''' Modified by: SA 06/06/2012 - Changed the query to include field TestType in the INNER JOIN between tables tparTestControls 
        '''                              and tqcHistoryTestSamples
        ''' </remarks>
        Public Function VerifyLinkByQCModuleIDsNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCTestSampleID As Integer, _
                                                  ByVal pQCControlLotID As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT COUNT(*) AS NumLinks " & vbCrLf & _
                                                " FROM tparTestControls TC INNER JOIN tqcHistoryControlLots HCL ON TC.ControlID = HCL.ControlID " & vbCrLf & _
                                                                         " INNER JOIN tqcHistoryTestSamples HTS ON TC.TestType   = HTS.TestType " & vbCrLf & _
                                                                                                             " AND TC.TestID     = HTS.TestID " & vbCrLf & _
                                                                                                             " AND TC.SampleType = HTS.SampleType " & vbCrLf & _
                                                " WHERE HCL.QCControlLotID = " & pQCControlLotID.ToString & vbCrLf & _
                                                " AND   HTS.QCTestSampleID = " & pQCTestSampleID.ToString & vbCrLf

                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Dim dbDataReader As SqlClient.SqlDataReader = dbCmd.ExecuteReader()
                            If (dbDataReader.HasRows) Then
                                dbDataReader.Read()
                                If (dbDataReader.IsDBNull(0)) Then
                                    resultData.SetDatos = False
                                Else
                                    resultData.SetDatos = (Convert.ToInt32(dbDataReader.Item("NumLinks")) = 1)
                                End If
                            End If
                            dbDataReader.Close()
                        End Using
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparTestControlsDAO.VerifyLinkByQCModuleIDs", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function
#End Region

#Region "TEMPORARY - TO SIMULATE QC RESULTS"
        ''' <summary>
        ''' Get all Tests/SampleTypes linked to the specified list of Controls
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pControlIDList">List of Control Identifiers</param>
        ''' <returns>GlobalDataTO containing a typed DataSet TestControlsDS with the list of all Tests/SampleTypes
        '''          linked to the specified list of Controls</returns>
        ''' <remarks>
        ''' Created by:  SA 24/05/2011
        ''' Modified by: SA 15/06/2012 - Changed the query by adding a subquery to get also ISE Tests/SampleTypes linked to the 
        '''                              specified Controls
        ''' Modified by: RH 20/06/2012 - Invalid column name 'TestID'. Changed to IT.ISETestID
        ''' </remarks>
        Public Function ReadTestsByControlIDListNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pControlIDList As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)

                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT TC.ControlID, TC.TestType, TC.TestID, TC.SampleType, T.TestName, T.PreloadedTest, C.ControlName " & vbCrLf & _
                                                " FROM tparTestControls TC INNER JOIN tparTests T ON TC.TestID = T.TestID " & vbCrLf & _
                                                                         " INNER JOIN tparControls C ON TC.ControlID = C.ControlID " & vbCrLf & _
                                                " WHERE TC.ControlID IN (" & pControlIDList & ") " & vbCrLf & _
                                                " UNION " & vbCrLf & _
                                                " SELECT TC.ControlID, TC.TestType, TC.TestID, TC.SampleType, IT.Name AS TestName, 0 AS PreloadedTest, C.ControlName " & vbCrLf & _
                                                " FROM tparTestControls TC INNER JOIN tparISETests IT ON TC.TestID = IT.ISETestID " & vbCrLf & _
                                                                         " INNER JOIN tparControls C ON TC.ControlID = C.ControlID " & vbCrLf & _
                                                " WHERE TC.ControlID IN (" & pControlIDList & ") " & vbCrLf & _
                                                " ORDER BY TC.TestType DESC, TestName, TC.SampleType, C.ControlName "

                        Dim myTestControlsData As New TestControlsDS()

                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myTestControlsData.tparTestControls)
                            End Using
                        End Using

                        resultData.SetDatos = myTestControlsData
                        resultData.HasError = False
                    End If
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparTestControlsDAO.ReadTestsByControlIDList", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try

            Return resultData
        End Function


        ''' <summary>
        ''' Get all Tests/SampleTypes linked to the specified list of Controls
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestID">List of Control Identifiers</param>
        ''' <returns>GlobalDataTO containing a typed DataSet TestControlsDS with the list of all Tests/SampleTypes
        '''          linked to the specified list of Controls</returns>
        ''' <remarks>
        ''' Created by:  DL 11/10/2012
        ''' </remarks>
        Public Function ReadControlByTests(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer, ByVal pTestType As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)

                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then
                        Dim cmdtext As String = String.Empty

                        cmdtext &= "SELECT TC.ControlID, TC.TestType, TC.TestID, TC.SampleType, T.TestName, T.PreloadedTest, C.ControlName" & vbCrLf
                        cmdtext &= "  FROM tparTestControls  TC  INNER JOIN tparTests T ON TC.TestID = T.TestID INNER JOIN tparControls C  ON TC.ControlID = C.ControlID " & vbCrLf
                        cmdtext &= " WHERE TC.TestID = " & pTestID & vbCrLf
                        cmdtext &= "   AND TC.TestType = '" & pTestType & "'"

                        Dim myTestControlsData As New TestControlsDS()

                        Using dbCmd As New SqlClient.SqlCommand(cmdtext, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myTestControlsData.tparTestControls)
                            End Using
                        End Using

                        resultData.SetDatos = myTestControlsData
                        resultData.HasError = False
                    End If
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparTestControlsDAO.ReadControlByTests", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try

            Return resultData
        End Function

        ''' <summary>
        ''' Get all Tests/SampleTypes having QC active and at least a linked Control/Lot
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTO containing a typed DataSet TestControlsDS with the list of Tests/SampleTypes having
        '''          QC active and at least a linked Control/Lot</returns>
        ''' <remarks>
        ''' Created by:  TR 23/05/2012
        ''' Modified by: SA 29/05/2012 - Get field TestType; removed table tparControls from the query; added DISTINCT   
        ''' </remarks>
        Public Function ReadAllNEW(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT DISTINCT 'STD' AS TestType, T.TestID, T.TestName, TC.SampleType," & vbCrLf & _
                                                                " MD.FixedItemDesc AS MeasureUnit, T.DecimalsAllowed, T.PreloadedTest " & vbCrLf & _
                                                " FROM   tparTestControls TC INNER JOIN tparTests T ON TC.TestID = T.TestID " & vbCrLf & _
                                                                           " INNER JOIN tparTestSamples TS ON TC.TestID = TS.TestID AND TC.SampleType = TS.SampleType " & vbCrLf & _
                                                                           " INNER JOIN tcfgMasterData MD ON T.MeasureUnit = MD.ItemID " & vbCrLf & _
                                                " WHERE  MD.SubTableID = 'TEST_UNITS' " & vbCrLf & _
                                                " AND    TC.TestType   = 'STD' " & vbCrLf & _
                                                " AND    TS.QCActive   = 1 " & vbCrLf & _
                                                " AND    TS.NumberOfControls > 0  " & vbCrLf & _
                                                " UNION " & vbCrLf & _
                                                " SELECT DISTINCT 'ISE' AS TestType, IT.ISETestID AS TestID, IT.Name AS TestName, ITS.SampleType, " & vbCrLf & _
                                                                " MD.FixedItemDesc AS MeasureUnit, ITS.Decimals AS DecimalsAllowed, 1 AS PreloadedTest " & vbCrLf & _
                                                " FROM   tparTestControls TC INNER JOIN tparISETests IT ON TC.TestID = IT.ISETestID " & vbCrLf & _
                                                                           " INNER JOIN tparISETestSamples ITS ON TC.TestID = ITS.ISETestID AND TC.SampleType = ITS.SampleType " & vbCrLf & _
                                                                           " INNER JOIN tcfgMasterData MD ON IT.Units = MD.ItemID " & vbCrLf & _
                                                " WHERE  MD.SubTableID = 'TEST_UNITS' " & vbCrLf & _
                                                " AND    TC.TestType   = 'ISE' " & vbCrLf & _
                                                " AND    ITS.QCActive  = 1 " & vbCrLf & _
                                                " AND    ITS.NumberOfControls > 0  " & vbCrLf & _
                                                " ORDER BY TestType DESC, PreloadedTest DESC, TestName, SampleType "

                        Dim myTestControlsData As New TestControlsDS()
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myTestControlsData.tparTestControls)
                            End Using
                        End Using

                        resultData.SetDatos = myTestControlsData
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparTestControlsDAO.ReadAll", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get the list of active Controls/Lots linked to the specified Standard TestID/SampleType
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestID">Test Identifier</param>
        ''' <param name="pSampleType">Sample Type Code</param>
        ''' <returns>GlobalDataTO containing a typed DataSet TestControlsDS with the list of active Controls/Lots linked to the
        '''          informed Standard TestID/SampleType</returns>
        ''' <remarks>
        ''' Created by:  TR 23/05/2012
        ''' </remarks>
        Public Function ReadAdditionalInfoForSTDTests(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer, _
                                                      ByVal pSampleType As String) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing
            Try
                myGlobalDataTo = GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTo.HasError AndAlso Not myGlobalDataTo.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTo.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT TC.ControlID, C.ControlName, C.LotNumber, TC.MinConcentration, TC.MaxConcentration " & vbCrLf & _
                                                " FROM   tparTestControls TC INNER JOIN tparControls C ON TC.ControlID  = C.ControlID " & vbCrLf & _
                                                                           " INNER JOIN tparTests T    ON TC.TestID     = T.TestID " & vbCrLf & _
                                                                           " INNER JOIN tparTestSamples TS ON TC.TestID = TS.TestID AND TC.SampleType = TS.SampleType " & vbCrLf & _
                                  " WHERE  TC.TestType      = 'STD' " & vbCrLf & _
                                  " AND    TC.TestID        = " & pTestID.ToString & vbCrLf & _
                                  " AND    TC.SampleType    = '" & pSampleType.ToString & "' " & vbCrLf & _
                                  " AND    TC.ActiveControl = 1 " & vbCrLf

                        Dim myTestControlsDS As New TestControlsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myTestControlsDS.tparTestControls)
                            End Using
                        End Using

                        myGlobalDataTO.SetDatos = myTestControlsDS
                        myGlobalDataTO.HasError = False
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparTestControlsDAO.ReadAdditionalInfoForSTDTests", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get the list of active Controls/Lots linked to the specified ISE TestID/SampleType
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestID">Test Identifier</param>
        ''' <param name="pSampleType">Sample Type Code</param>
        ''' <returns>GlobalDataTO containing a typed DataSet TestControlsDS with the list of active Controls/Lots linked to the
        '''          informed ISE TestID/SampleType</returns>
        ''' <remarks>
        ''' Created by:  SA 06/06/2012
        ''' </remarks>
        Public Function ReadAdditionalInfoForISETests(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer, _
                                                      ByVal pSampleType As String) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing
            Try
                myGlobalDataTO = GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT TC.ControlID, C.ControlName, C.LotNumber, TC.MinConcentration, TC.MaxConcentration " & vbCrLf & _
                                                " FROM   tparTestControls TC INNER JOIN tparControls C  ON TC.ControlID  = C.ControlID " & vbCrLf & _
                                                                           " INNER JOIN tparISETests IT ON TC.TestID     = IT.ISETestID " & vbCrLf & _
                                                                           " INNER JOIN tparISETestSamples ITS ON TC.TestID = ITS.ISETestID AND TC.SampleType = ITS.SampleType " & vbCrLf & _
                                  " WHERE  TC.TestType      = 'ISE' " & vbCrLf & _
                                  " AND    TC.TestID        = " & pTestID.ToString & vbCrLf & _
                                  " AND    TC.SampleType    = '" & pSampleType.ToString & "' " & vbCrLf & _
                                  " AND    TC.ActiveControl = 1 " & vbCrLf

                        Dim myTestControlsDS As New TestControlsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myTestControlsDS.tparTestControls)
                            End Using
                        End Using

                        myGlobalDataTO.SetDatos = myTestControlsDS
                        myGlobalDataTO.HasError = False
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparTestControlsDAO.ReadAdditionalInfoForISETests", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function
#End Region

#Region "TO DELETE - OLD FUNCTIONS"

        '''' <summary>
        '''' Count the number of Controls linked to the informed Test/SampleType that are currently marked as active Controls
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pTestID">Test Identifier</param>
        '''' <param name="pSampleType">Sample Type Code</param>
        '''' <returns>GlobalDataTO containing an integer value with the number of Controls linked to the informed Test/SampleType that are currently 
        ''''          marked as active Controls</returns>
        '''' <remarks>
        '''' Created by:  SA 06/10/2011
        '''' </remarks>
        'Public Function CountActiveByTestIDAndSampleTypeOLD(ByVal pDBConnection As SqlConnection, ByVal pTestID As Integer, ByVal pSampleType As String) As GlobalDataTO
        '    Dim resultData As GlobalDataTO = Nothing
        '    Dim dbConnection As SqlClient.SqlConnection = Nothing

        '    Try
        '        resultData = GetOpenDBConnection(pDBConnection)
        '        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
        '            dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim cmdText As String = " SELECT COUNT(*) AS NumActiveControls FROM tparTestControls " & vbCrLf & _
        '                                        " WHERE  TestID = " & pTestID.ToString & vbCrLf & _
        '                                        " AND    SampleType = '" & pSampleType & "' " & vbCrLf & _
        '                                        " AND    ActiveControl = 1 "

        '                Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
        '                    Dim dbDataReader As SqlClient.SqlDataReader = dbCmd.ExecuteReader()
        '                    If (dbDataReader.HasRows) Then
        '                        dbDataReader.Read()
        '                        If (dbDataReader.IsDBNull(0)) Then
        '                            resultData.SetDatos = 0
        '                        Else
        '                            resultData.SetDatos = Convert.ToInt32(dbDataReader.Item("NumActiveControls"))
        '                        End If
        '                    End If
        '                    dbDataReader.Close()
        '                End Using
        '            End If
        '        End If
        '    Catch ex As Exception
        '        resultData = New GlobalDataTO()
        '        resultData.HasError = True
        '        resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        resultData.ErrorMessage = ex.Message

        '        'Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "tparTestControlsDAO.CountActiveByTestIDAndSampleType", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return resultData
        'End Function

        '''' <summary>
        '''' Link a Test/SampleType to an specific Control
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pTestControlDS">Typed DataSet TestControlsDS with data of the Test/SampleType to link</param>
        '''' <returns>GlobalDataTO containing success/error information</returns>
        '''' <remarks>
        '''' Created by:  DL 31/03/2011
        '''' </remarks>
        'Public Function CreateOLD(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestControlDS As TestControlsDS) As GlobalDataTO
        '    Dim resultData As New GlobalDataTO

        '    Try
        '        If (pDBConnection Is Nothing) Then
        '            resultData.HasError = True
        '            resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
        '        Else
        '            Dim cmdText As String
        '            cmdText = " INSERT INTO tparTestControls (TestID, SampleType, ControlID, MinConcentration, MaxConcentration, TargetMean, " & vbCrLf & _
        '                                                    " TargetSD, ActiveControl, TS_User, TS_DateTime) " & vbCrLf & _
        '                      " VALUES( " & pTestControlDS.tparTestControls(0).TestID & ", " & vbCrLf & _
        '                             " '" & pTestControlDS.tparTestControls(0).SampleType & "', " & vbCrLf & _
        '                                    pTestControlDS.tparTestControls(0).ControlID & ", " & vbCrLf & _
        '                                    ReplaceNumericString(pTestControlDS.tparTestControls(0).MinConcentration) & ", " & vbCrLf & _
        '                                    ReplaceNumericString(pTestControlDS.tparTestControls(0).MaxConcentration) & ", " & vbCrLf & _
        '                                    ReplaceNumericString(pTestControlDS.tparTestControls(0).TargetMean) & ", " & vbCrLf & _
        '                                    ReplaceNumericString(pTestControlDS.tparTestControls(0).TargetSD) & ", " & vbCrLf & _
        '                                    Convert.ToInt32(IIf(pTestControlDS.tparTestControls(0).ActiveControl, 1, 0)) & ", " & vbCrLf

        '            If (String.IsNullOrEmpty(pTestControlDS.tparTestControls(0).TS_User.ToString)) Then
        '                'Dim currentSession As New GlobalBase
        '                cmdText &= " N'" & GlobalBase.GetSessionInfo().UserName.Replace("'", "''") & "', " & vbCrLf
        '            Else
        '                cmdText &= " N'" & pTestControlDS.tparTestControls(0).TS_User.Trim.Replace("'", "''") & "', " & vbCrLf
        '            End If

        '            If (pTestControlDS.tparTestControls(0).IsTS_DateTimeNull) Then
        '                cmdText &= " '" & Now.ToString("yyyyMMdd HH:mm:ss") & "') " & vbCrLf
        '            Else
        '                cmdText &= " '" & pTestControlDS.tparTestControls(0).TS_DateTime.ToString("yyyyMMdd HH:mm:ss") & "') " & vbCrLf
        '            End If

        '            Dim dbCmd As New SqlClient.SqlCommand
        '            dbCmd.Connection = pDBConnection
        '            dbCmd.CommandText = cmdText

        '            resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
        '            resultData.HasError = (resultData.AffectedRecords = 0)
        '        End If
        '    Catch ex As Exception
        '        resultData.HasError = True
        '        resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        resultData.ErrorMessage = ex.Message

        '        'Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "tparTestControlsDAO.Create", EventLogEntryType.Error, False)
        '    End Try
        '    Return resultData
        'End Function

        '''' <summary>
        '''' Delete the relation between the specified Control and Standard Test/SampleType
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pControlID">Control Identifier</param>
        '''' <param name="pTestID">Test Identifier</param>
        '''' <param name="pSampleType">Sample Type Code</param>
        '''' <returns>GlobalDataTO containing sucess/error information</returns>
        '''' <remarks>
        '''' Created by: SA 13/05/2011
        '''' </remarks>
        'Public Function DeleteOLD(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pControlID As Integer, ByVal pTestID As Integer, _
        '                       ByVal pSampleType As String) As GlobalDataTO
        '    Dim myGlobalDataTO As New GlobalDataTO
        '    Try
        '        If (pDBConnection Is Nothing) Then
        '            myGlobalDataTO.HasError = True
        '            myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
        '        Else
        '            Dim cmdText As String = " DELETE tparTestControls " & vbCrLf & _
        '                                    " WHERE  ControlID = " & pControlID & vbCrLf & _
        '                                    " AND    TestID    = " & pTestID & vbCrLf & _
        '                                    " AND    SampleType = '" & pSampleType & "' "

        '            Dim cmd As New SqlCommand
        '            cmd.Connection = pDBConnection
        '            cmd.CommandText = cmdText

        '            myGlobalDataTO.AffectedRecords = cmd.ExecuteNonQuery()
        '            myGlobalDataTO.HasError = False
        '        End If
        '    Catch ex As Exception
        '        myGlobalDataTO.HasError = True
        '        myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myGlobalDataTO.ErrorMessage = ex.Message

        '        'Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "tparTestControlsDAO.Delete", EventLogEntryType.Error, False)
        '    End Try
        '    Return myGlobalDataTO
        'End Function

        '''' <summary>
        '''' Update values for a Test/SampleType linked to an specific Control
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pTestControlDS">Typed DataSet TestControlsDS with data of the linked Test/SampleType to updated</param>
        '''' <returns>GlobalDataTO containing success/error information</returns>
        '''' <remarks>
        '''' Created by:  DL 31/03/2011
        '''' Modified by: TR 07/04/2011 - Removed updation of PK fields
        ''''              TR 12/04/2011 - Added the PK fields on the WHERE specification  
        ''''              SA 03/01/2012 - Update field ActiveControl only when the field is informed in the DS 
        '''' </remarks>
        'Public Function UpdateOLD(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestControlDS As TestControlsDS) As GlobalDataTO
        '    Dim resultData As New GlobalDataTO

        '    Try
        '        If (pDBConnection Is Nothing) Then
        '            resultData.HasError = True
        '            resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
        '        Else
        '            Dim cmdText As String = " UPDATE tparTestControls " & vbCrLf & _
        '                                       " SET MinConcentration = " & ReplaceNumericString(pTestControlDS.tparTestControls(0).MinConcentration) & ", " & vbCrLf & _
        '                                           " MaxConcentration = " & ReplaceNumericString(pTestControlDS.tparTestControls(0).MaxConcentration) & ", " & vbCrLf & _
        '                                           " TargetMean       = " & ReplaceNumericString(pTestControlDS.tparTestControls(0).TargetMean) & ", " & vbCrLf & _
        '                                           " TargetSD         = " & ReplaceNumericString(pTestControlDS.tparTestControls(0).TargetSD) & ", " & vbCrLf

        '            If (Not pTestControlDS.tparTestControls(0).IsActiveControlNull) Then
        '                cmdText &= " ActiveControl    = " & IIf(pTestControlDS.tparTestControls(0).ActiveControl, 1, 0).ToString & ", " & vbCrLf
        '            End If

        '            If (String.IsNullOrEmpty(pTestControlDS.tparTestControls(0).TS_User.ToString)) Then
        '                'Dim currentSession As New GlobalBase
        '                cmdText &= " TS_User = N'" & GlobalBase.GetSessionInfo().UserName.Replace("'", "''") & "', " & vbCrLf
        '            Else
        '                cmdText &= " TS_User = N'" & pTestControlDS.tparTestControls(0).TS_User.Trim.Replace("'", "''") & "', " & vbCrLf
        '            End If

        '            If (pTestControlDS.tparTestControls(0).IsTS_DateTimeNull) Then
        '                cmdText &= " TS_DateTime = '" & Now.ToString("yyyyMMdd HH:mm:ss") & "' " & vbCrLf
        '            Else
        '                cmdText &= " TS_DateTime = '" & pTestControlDS.tparTestControls(0).TS_DateTime.ToString("yyyyMMdd HH:mm:ss") & "' " & vbCrLf
        '            End If

        '            cmdText &= " WHERE ControlID = " & pTestControlDS.tparTestControls(0).ControlID & vbCrLf & _
        '                         " AND TestID = " & pTestControlDS.tparTestControls(0).TestID & vbCrLf & _
        '                         " AND SampleType = '" & pTestControlDS.tparTestControls(0).SampleType & "' " & vbCrLf

        '            Dim dbCmd As New SqlClient.SqlCommand
        '            dbCmd.Connection = pDBConnection
        '            dbCmd.CommandText = cmdText

        '            resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
        '            resultData.HasError = False
        '        End If
        '    Catch ex As Exception
        '        resultData.HasError = True
        '        resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        resultData.ErrorMessage = ex.Message

        '        'Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "tparTestControlsDAO.Update", EventLogEntryType.Error, False)
        '    End Try
        '    Return resultData
        'End Function

        '''' <summary>
        '''' Delete the relation between an Standard Test and all the Controls defined for it; optionally,
        '''' the deletion can be executed only for Controls the Test uses for the specified SampleType
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pTestID">Test Identifier</param>
        '''' <param name="pSampleType">Sample Type Code. Optional parameter</param>
        '''' <param name="pControlIDList">List of ControlIDs separated by (,). Optional parameter</param>
        '''' <returns>GlobalDataTO containing sucess/error information</returns>
        '''' <remarks>
        '''' Created by:  TR 17/04/2011
        '''' Modified by: SA 11/05/2011 - Changed optional parameter pControlID for pControlIDList, a string list
        ''''                              containing the list of ControlIDs that have to be deleted for the informed Test/SampleType
        '''' </remarks>
        'Public Function DeleteTestControlByTestIDOLD(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer, Optional ByVal pSampleType As String = "", _
        '                                          Optional ByVal pControlIDList As String = "") As GlobalDataTO
        '    Dim myGlobalDataTO As New GlobalDataTO
        '    Try
        '        If (pDBConnection Is Nothing) Then
        '            myGlobalDataTO.HasError = True
        '            myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
        '        Else
        '            Dim cmdText As String = ""
        '            cmdText = " DELETE FROM  tparTestControls " & _
        '                      " WHERE TestID = " & pTestID

        '            'Filter data for the specified optional parameters when they are informed
        '            If (pSampleType <> "") Then cmdText &= " AND SampleType ='" & pSampleType & "'"
        '            If (pControlIDList <> "") Then cmdText &= " AND ControlID NOT IN (" & pControlIDList & ") "

        '            Dim cmd As New SqlCommand
        '            cmd.Connection = pDBConnection
        '            cmd.CommandText = cmdText

        '            myGlobalDataTO.AffectedRecords = cmd.ExecuteNonQuery()
        '            myGlobalDataTO.HasError = False
        '        End If
        '    Catch ex As Exception
        '        myGlobalDataTO.HasError = True
        '        myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myGlobalDataTO.ErrorMessage = ex.Message

        '        'Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "tparTestControlsDAO.DeleteTestControlByTestID", EventLogEntryType.Error, False)
        '    End Try
        '    Return myGlobalDataTO
        'End Function

        '''' <summary>
        '''' Get the basic information of the Controls defined for the specified Test and optionally, Sample Type 
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pTestID">Test Identifier</param>
        '''' <param name="pSampleType">Sample Type Code. Optional parameter</param>
        '''' <param name="pControlID">Control Identifier. Optional parameter</param>
        '''' <param name="pOnlyActiveControls">When True, it indicates only Controls linked as Active have to be returned, but only if
        ''''                                   the specified Test/Sample Type has the Quality Control feature enabled</param>
        '''' <returns>GlobalDataTO containing a typed DataSet TestControlsDS with data of defined Controls 
        ''''          for the informed Test and Sample Type</returns>
        '''' <remarks>
        '''' Modified by: SA 04/03/2010 - Query was changed; it was bad done
        ''''              TR 04/04/2011 - Added new optional parameter  pOnlyActiveControls
        ''''                            - Get fields TargetMean, TargetSD, and ActiveControl
        ''''              TR 07/04/2011 - Added optional parammeter pControlID
        ''''              SA 15/04/2011 - Removed field ControlNum from the SQL Query
        ''''              TR 11/05/2011 - Changed parameter pSampleType to optional
        ''''              SA 21/06/2011 - When parameter pOnlyActiveControls has been set to True, filter data also by QCActive = True
        ''''                              for the Test/SampleType
        ''''              TR 11/10/2011 - Added the ORDER BY field ActiveControl
        '''' </remarks>
        'Public Function ReadByTestIDAndSampleTypeOLD(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer, _
        '                                          Optional ByVal pSampleType As String = "", Optional ByVal pControlID As Integer = 0, _
        '                                          Optional ByVal pOnlyActiveControls As Boolean = False) As GlobalDataTO
        '    Dim resultData As GlobalDataTO = Nothing
        '    Dim dbConnection As SqlClient.SqlConnection = Nothing

        '    Try
        '        resultData = GetOpenDBConnection(pDBConnection)
        '        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
        '            dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim cmdText As String = " SELECT TC.TestID, TC.SampleType, TC.ControlID, TC.MinConcentration, TC.MaxConcentration, " & vbCrLf & _
        '                                               " TC.TargetMean, TC.TargetSD, TC.ACtiveControl, C.ControlName, C.LotNumber, C.ExpirationDate, " & vbCrLf & _
        '                                               " T.TestName, TS.ControlReplicates " & vbCrLf & _
        '                                        " FROM   tparTestControls AS TC INNER JOIN tparControls AS C ON TC.ControlID = C.ControlID " & vbCrLf & _
        '                                                                      " INNER JOIN tparTests AS T ON TC.TestID = T.TestID " & vbCrLf & _
        '                                                                      " INNER JOIN tparTestSamples AS TS ON TC.TestID = TS.TestID AND TC.SampleType = TS.SampleType " & vbCrLf & _
        '                                        " WHERE  TC.TestID = " & pTestID & vbCrLf

        '                If (pSampleType <> "") Then
        '                    cmdText &= " AND UPPER(TC.SampleType) = UPPER('" & pSampleType & "') " & vbCrLf
        '                End If

        '                If (pOnlyActiveControls) Then
        '                    cmdText &= " AND TC.ActiveControl = 1 " & vbCrLf & _
        '                               " AND TS.QCActive = 1 " & vbCrLf
        '                End If

        '                If (pControlID <> 0) Then
        '                    cmdText &= " AND TC.ControlID = " & pControlID & vbCrLf
        '                End If
        '                cmdText &= " ORDER BY TC.ACtiveControl DESC " & vbCrLf


        '                Dim myTestControlsData As New TestControlsDS()
        '                Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
        '                    Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
        '                        dbDataAdapter.Fill(myTestControlsData.tparTestControls)
        '                    End Using
        '                End Using

        '                resultData.SetDatos = myTestControlsData
        '                resultData.HasError = False
        '            End If
        '        End If
        '    Catch ex As Exception
        '        resultData = New GlobalDataTO()
        '        resultData.HasError = True
        '        resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        resultData.ErrorMessage = ex.Message

        '        'Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "tparTestControlsDAO.ReadByTestIDAndSampleType", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return resultData
        'End Function

        '''' <summary>
        '''' Get all Tests/SampleTypes linked to the specified Control
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pControlID">Control Identifier</param>
        '''' <returns>GlobalDataTO containing a typed DataSet TestControlsDS with the list of all Tests/SampleTypes
        ''''          linked to the specified Control</returns>
        '''' <remarks>
        '''' Created by:  DL
        '''' Modified by: SA 18/04/2011 - Removed field ControlNum from the SQL
        ''''              SA 10/05/2011 - Removed parameter and filter for the SampleType; changed the query, the Join with
        ''''                              tparTestSamples was wrong; sort records by field TestPosition instead of TestName
        '''' </remarks>
        'Public Function ReadTestsByControlIDOLD(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pControlID As Integer) As GlobalDataTO
        '    Dim resultData As GlobalDataTO = Nothing
        '    Dim dbConnection As SqlClient.SqlConnection = Nothing

        '    Try
        '        resultData = GetOpenDBConnection(pDBConnection)
        '        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
        '            dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim cmdText As String = " SELECT TC.*, T.TestName, T.DecimalsAllowed, T.PreloadedTest, T.InUse, T.TestPosition, " & vbCrLf & _
        '                                               " TS.RejectionCriteria, MD.FixedItemDesc As MeasureUnit " & vbCrLf & _
        '                                        " FROM tparTestControls TC INNER JOIN tparTestSamples TS ON TC.TestID = TS.TestID AND TC.SampleType = TS.SampleType " & vbCrLf & _
        '                                                                 " INNER JOIN tparTests T ON TC.TestID = T.TestID " & vbCrLf & _
        '                                                                 " INNER JOIN tcfgMasterData MD ON T.MeasureUnit = MD.ItemID " & vbCrLf & _
        '                                        " WHERE TC.ControlID = " & pControlID & vbCrLf & _
        '                                        " AND   MD.SubtableID = '" & MasterDataEnum.TEST_UNITS.ToString & "' " & vbCrLf & _
        '                                        " ORDER BY T.TestPosition "

        '                Dim myTestControlsData As New TestControlsDS()
        '                Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
        '                    Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
        '                        dbDataAdapter.Fill(myTestControlsData.tparTestControls)
        '                    End Using
        '                End Using

        '                resultData.SetDatos = myTestControlsData
        '                resultData.HasError = False
        '            End If
        '        End If
        '    Catch ex As Exception
        '        resultData = New GlobalDataTO()
        '        resultData.HasError = True
        '        resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        resultData.ErrorMessage = ex.Message

        '        'Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "tparTestControlsDAO.ReadTestsByControlID", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return resultData
        'End Function

        '''' <summary>
        '''' Get all Tests/SampleTypes linked to the specified list of Controls
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pControlIDList">List of Control Identifiers</param>
        '''' <returns>GlobalDataTO containing a typed DataSet TestControlsDS with the list of all Tests/SampleTypes
        ''''          linked to the specified list of Controls</returns>
        '''' <remarks>
        '''' Created by:  SA 24/05/2011
        '''' </remarks>
        'Public Function ReadTestsByControlIDListOLD(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pControlIDList As String) As GlobalDataTO
        '    Dim resultData As GlobalDataTO = Nothing
        '    Dim dbConnection As SqlClient.SqlConnection = Nothing

        '    Try
        '        resultData = GetOpenDBConnection(pDBConnection)
        '        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
        '            dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim cmdText As String = " SELECT TC.ControlID, TC.TestID, TC.SampleType, T.TestName, T.PreloadedTest, C.ControlName " & vbCrLf & _
        '                                        " FROM tparTestControls TC INNER JOIN tparTests T ON TC.TestID = T.TestID " & vbCrLf & _
        '                                                                 " INNER JOIN tparControls C ON TC.ControlID = C.ControlID " & vbCrLf & _
        '                                        " WHERE TC.ControlID IN (" & pControlIDList & ") " & vbCrLf & _
        '                                        " ORDER BY T.TestName, TC.SampleType, C.ControlName "

        '                Dim myTestControlsData As New TestControlsDS()
        '                Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
        '                    Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
        '                        dbDataAdapter.Fill(myTestControlsData.tparTestControls)
        '                    End Using
        '                End Using

        '                resultData.SetDatos = myTestControlsData
        '                resultData.HasError = False
        '            End If
        '        End If
        '    Catch ex As Exception
        '        resultData = New GlobalDataTO()
        '        resultData.HasError = True
        '        resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        resultData.ErrorMessage = ex.Message

        '        'Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "tparTestControlsDAO.ReadTestsByControlIDList", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return resultData
        'End Function

        '''' <summary>
        '''' From identifiers of Test/SampleType and Control/Lot in QC Module, verifies if the link between the Test/SampleType
        '''' and the Control still exists in table tparTestControls
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pQCTestSampleID">Identifier of Test/SampleType in QC Module</param>
        '''' <param name="pQCControlLotID">Identifier of Control/Lot in QC Module</param>
        '''' <returns>GlobalDataTO containing a boolean value indicating if the link between the Test/SampleType and the Control
        ''''          still exists in table tparTestControls</returns>
        '''' <remarks>
        '''' Created by:  SA 04/01/2012 
        '''' </remarks>
        'Public Function VerifyLinkByQCModuleIDsOLD(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCTestSampleID As Integer, _
        '                                        ByVal pQCControlLotID As Integer) As GlobalDataTO
        '    Dim resultData As GlobalDataTO = Nothing
        '    Dim dbConnection As SqlClient.SqlConnection = Nothing

        '    Try
        '        resultData = GetOpenDBConnection(pDBConnection)
        '        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
        '            dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim cmdText As String = " SELECT COUNT(*) AS NumLinks " & vbCrLf & _
        '                                        " FROM tparTestControls TC INNER JOIN tqcHistoryControlLots HCL ON TC.ControlID = HCL.ControlID " & vbCrLf & _
        '                                                                 " INNER JOIN tqcHistoryTestSamples HTS ON TC.TestID = HTS.TestID AND TC.SampleType = HTS.SampleType " & vbCrLf & _
        '                                        " WHERE HCL.QCControlLotID = " & pQCControlLotID.ToString & vbCrLf & _
        '                                        " AND   HTS.QCTestSampleID = " & pQCTestSampleID.ToString & vbCrLf

        '                Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
        '                    Dim dbDataReader As SqlClient.SqlDataReader = dbCmd.ExecuteReader()
        '                    If (dbDataReader.HasRows) Then
        '                        dbDataReader.Read()
        '                        If (dbDataReader.IsDBNull(0)) Then
        '                            resultData.SetDatos = False
        '                        Else
        '                            resultData.SetDatos = (Convert.ToInt32(dbDataReader.Item("NumLinks")) = 1)
        '                        End If
        '                    End If
        '                    dbDataReader.Close()
        '                End Using
        '            End If
        '        End If
        '    Catch ex As Exception
        '        resultData = New GlobalDataTO()
        '        resultData.HasError = True
        '        resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        resultData.ErrorMessage = ex.Message

        '        'Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "tparTestControlsDAO.VerifyLinkByQCModuleIDs", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return resultData
        'End Function

        'Public Function ReadAllOLD(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
        '    Dim resultData As New GlobalDataTO
        '    Dim dbConnection As New SqlClient.SqlConnection

        '    Try
        '        resultData = GetOpenDBConnection(pDBConnection)
        '        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
        '            dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim cmdText As String = ""
        '                cmdText = " SELECT T.PreloadedTest, T.TestName, TC.SampleType, C.ControlName, C.LotNumber, TC.MinConcentration, " & vbCrLf & _
        '                                 " TC.MaxConcentration, T.TestID, TC.ControlID, TS.RejectionCriteria, " & vbCrLf & _
        '                                 " TC.TargetMean, TC.TargetSD, MD.FixedItemDesc AS MeasureUnit, T.DecimalsAllowed " & vbCrLf & _
        '                          " FROM   tparTestControls TC INNER JOIN tparTests T ON TC.TestID = T.TestID " & vbCrLf & _
        '                                                     " INNER JOIN tparTestSamples TS ON TC.TestID = TS.TestID AND TC.SampleType = TS.SampleType " & vbCrLf & _
        '                                                     " INNER JOIN tparControls C ON TC.ControlID = C.ControlID " & vbCrLf & _
        '                                                     " INNER JOIN tcfgMasterData MD ON T.MeasureUnit = MD.ItemID " & vbCrLf & _
        '                          " WHERE  MD.SubTableID = 'TEST_UNITS' " & vbCrLf & _
        '                          " AND    TC.ActiveControl = 1 " & vbCrLf & _
        '                          " ORDER BY T.TestName, TC.SampleType, C.ControlName "

        '                Dim dbCmd As New SqlClient.SqlCommand
        '                dbCmd.Connection = dbConnection
        '                dbCmd.CommandText = cmdText

        '                Dim dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
        '                Dim TestControlsData As New TestControlsDS()
        '                dbDataAdapter.Fill(TestControlsData.tparTestControls)

        '                resultData.SetDatos = TestControlsData
        '                resultData.HasError = False
        '            End If
        '        End If
        '    Catch ex As Exception
        '        resultData.HasError = True
        '        resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        resultData.ErrorMessage = ex.Message

        '        'Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "tparTestControlsDAO.ReadAll", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return resultData
        'End Function

        'Public Function ReadAdditionalInformationOLD(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer, _
        '                                          ByVal pSampleType As String, ByVal pControlID As Integer) As GlobalDataTO
        '    Dim myGlobalDataTo As New GlobalDataTO
        '    Dim dbConnection As New SqlClient.SqlConnection
        '    Try
        '        myGlobalDataTo = GetOpenDBConnection(pDBConnection)
        '        If (Not myGlobalDataTo.HasError) And (Not myGlobalDataTo.SetDatos Is Nothing) Then
        '            dbConnection = DirectCast(myGlobalDataTo.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim cmdText As String
        '                cmdText = " SELECT TC.TestID, TC.SampleType, TC.ControlID, C.ControlName, C.LotNumber, T.TestName, " & vbCrLf & _
        '                                 " T.ShortName AS TestShortName, T.PreloadedTest, T.MeasureUnit, T.DecimalsAllowed, " & vbCrLf & _
        '                                 " TS.RejectionCriteria, TS.CalculationMode, TS.NumberOfSeries " & vbCrLf & _
        '                          " FROM   tparTestControls TC INNER JOIN tparControls C     ON TC.ControlID = C.ControlID " & vbCrLf & _
        '                                                     " INNER JOIN tparTests T        ON TC.TestID    = T.TestID " & vbCrLf & _
        '                                                     " INNER JOIN tparTestSamples TS ON TC.TestID    = TS.TestID AND TC.SampleType = TS.SampleType " & vbCrLf & _
        '                          " WHERE  TC.TestID = " & pTestID & vbCrLf & _
        '                          " AND    TC.SampleType = '" & pSampleType & "' " & vbCrLf & _
        '                          " AND    TC.ControlID  = " & pControlID & vbCrLf

        '                Dim dbCmd As New SqlClient.SqlCommand
        '                dbCmd.Connection = dbConnection
        '                dbCmd.CommandText = cmdText

        '                Dim mytwksOrderTestDS As New OrderTestsDS
        '                Dim dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
        '                dbDataAdapter.Fill(mytwksOrderTestDS.twksOrderTests)
        '                myGlobalDataTo.SetDatos = mytwksOrderTestDS
        '            End If
        '        End If
        '    Catch ex As Exception
        '        myGlobalDataTo.HasError = True
        '        myGlobalDataTo.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myGlobalDataTo.ErrorMessage = ex.Message

        '        'Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "tparTestControlsDAO.ReadAdditionalInformation", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return myGlobalDataTo
        'End Function
#End Region

    End Class
End Namespace

