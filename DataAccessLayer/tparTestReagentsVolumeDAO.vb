Option Explicit On
Option Strict On

Imports System.Data.SqlClient
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global

Namespace Biosystems.Ax00.DAL.DAO

    Partial Public Class tparTestReagentsVolumeDAO
        Inherits DAOBase

#Region "CRUD Methods"
        ''' <summary>
        ''' Create volumes for Reagents when used for a Test/SampleType
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestReagentsVolumeDS">Typed DataSet TestReagentsVolumesDS containing the list of values to update</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  TR 03/02/2010
        ''' Modified by: SA 28/10/2010 - Changed Replace(",", ".") for callings to global function ReplaceNumericString
        ''' </remarks>
        Public Function Create(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestReagentsVolumeDS As TestReagentsVolumesDS) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = ""
                    Dim values As String = " "

                    Dim cmd As New SqlCommand
                    cmd.Connection = pDBConnection

                    Dim keys As String = " (TestID, ReagentID, ReagentNumber, SampleType, ReagentVolume, " & _
                                          " ReagentVolumeSteps, RedPostReagentVolume, RedPostReagentVolumeSteps, " & _
                                          " IncPostReagentVolume, IncPostReagentVolumeSteps) "

                    For Each tparTestReagentsVolumeDR As TestReagentsVolumesDS.tparTestReagentsVolumesRow In pTestReagentsVolumeDS.tparTestReagentsVolumes.Rows
                        values = ""

                        values &= tparTestReagentsVolumeDR.TestID.ToString() & ", "
                        values &= tparTestReagentsVolumeDR.ReagentID.ToString() & ", "
                        values &= tparTestReagentsVolumeDR.ReagentNumber.ToString() & ", "
                        values &= "'" & tparTestReagentsVolumeDR.SampleType & "', "
                        values &= ReplaceNumericString(tparTestReagentsVolumeDR.ReagentVolume) & ", "

                        If (tparTestReagentsVolumeDR.IsReagentVolumeStepsNull) Then
                            values &= " NULL, "
                        Else
                            values &= ReplaceNumericString(tparTestReagentsVolumeDR.ReagentVolumeSteps) & ", "
                        End If

                        If (tparTestReagentsVolumeDR.IsRedPostReagentVolumeNull) Then
                            values &= " NULL, "
                        Else
                            values &= ReplaceNumericString(tparTestReagentsVolumeDR.RedPostReagentVolume) & ", "
                        End If

                        If (tparTestReagentsVolumeDR.IsRedPostReagentVolumeStepsNull) Then
                            values &= " NULL, "
                        Else
                            values &= ReplaceNumericString(tparTestReagentsVolumeDR.RedPostReagentVolumeSteps) & ", "
                        End If

                        If (tparTestReagentsVolumeDR.IsIncPostReagentVolumeNull) Then
                            values &= " NULL, "
                        Else
                            values &= ReplaceNumericString(tparTestReagentsVolumeDR.IncPostReagentVolume) & ", "
                        End If

                        If (tparTestReagentsVolumeDR.IsIncPostReagentVolumeStepsNull) Then
                            values &= " NULL"
                        Else
                            values &= ReplaceNumericString(tparTestReagentsVolumeDR.IncPostReagentVolumeSteps)
                        End If

                        cmdText = "INSERT INTO tparTestReagentsVolumes " & keys & " VALUES (" & values & ")"

                        cmd.CommandText = cmdText
                        myGlobalDataTO.AffectedRecords += cmd.ExecuteNonQuery()
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
                GlobalBase.CreateLogActivity(ex.Message, "tparTestReagentsVolumeDAO.Create", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get the list of Reagent Volumes required for an specific Test
        ''' </summary>
        ''' <param name="pDBConnection">Open Database Connection</param>
        ''' <param name="pTestID">Test Identifier</param>
        ''' <param name="pSampleType">Sample Type Code. Used as filter only when it has a value different of empty string</param>
        ''' <returns>GlobalDataTO containing a typed DataSet TestReagentsDS with the list of Reagents Volume required for the informed Test</returns>
        ''' <remarks>
        ''' Created by:  DL 22/02/2010
        ''' Modified by: SA 28/10/2010 - Changed the query, the join is not needed, field TestID exists in tparTestReagentsVolumes
        '''              SA 29/02/2012 - Changed the function template; changed the query to get all table fields
        '''              AG 03/07/2012 - Added parameter pSampleType and use it to filter data when it is informed
        ''' </remarks>
        Public Function ReadByTestID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer, ByVal pSampleType As String) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT * FROM tparTestReagentsVolumes " & vbCrLf & _
                                                " WHERE  TestID = " & pTestID.ToString & vbCrLf

                        If (pSampleType <> String.Empty) Then cmdText &= " AND SampleType = '" & pSampleType.ToString & "' "

                        Dim resultData As New TestReagentsVolumesDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(resultData.tparTestReagentsVolumes)
                            End Using
                        End Using

                        myGlobalDataTO.SetDatos = resultData
                        myGlobalDataTO.HasError = False
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparTestReagentsDAO.ReadByTestID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Update volumes of Reagents when used for a Test/SampleType
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestReagentsVolumeDS">Typed DataSet TestReagentsVolumesDS containing the list of values to update</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  TR 02/03/2010
        ''' Modified by: SA 28/10/2010 - Changed Replace(",", ".") for callings to global function ReplaceNumericString;
        '''                              removed error raising depending value of affected records 
        ''' </remarks>
        Public Function Update(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestReagentsVolumeDS As TestReagentsVolumesDS) As GlobalDataTO

            Dim myGlobalDataTO As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = ""
                    Dim values As String = " "
                    Dim cmd As New SqlCommand

                    For Each tparTestReagentsVolumeDR As TestReagentsVolumesDS.tparTestReagentsVolumesRow In pTestReagentsVolumeDS.tparTestReagentsVolumes.Rows
                        values = ""
                        values &= " TestID = " & tparTestReagentsVolumeDR.TestID.ToString() & ", "

                        If (tparTestReagentsVolumeDR.IsReagentVolumeNull) Then
                            values &= " ReagentVolume = NULL, "
                        Else
                            values &= " ReagentVolume = " & ReplaceNumericString(tparTestReagentsVolumeDR.ReagentVolume) & ", "
                        End If

                        If (tparTestReagentsVolumeDR.IsReagentVolumeStepsNull) Then
                            values &= " ReagentVolumeSteps = NULL, "
                        Else
                            values &= " ReagentVolumeSteps = " & ReplaceNumericString(tparTestReagentsVolumeDR.ReagentVolumeSteps) & ", "
                        End If

                        If (tparTestReagentsVolumeDR.IsRedPostReagentVolumeNull) Then
                            values &= " RedPostReagentVolume = NULL, "
                        Else
                            values &= " RedPostReagentVolume = " & ReplaceNumericString(tparTestReagentsVolumeDR.RedPostReagentVolume) & ", "
                        End If

                        If (tparTestReagentsVolumeDR.IsRedPostReagentVolumeStepsNull) Then
                            values &= " RedPostReagentVolumeSteps = NULL, "
                        Else
                            values &= " RedPostReagentVolumeSteps = " & ReplaceNumericString(tparTestReagentsVolumeDR.RedPostReagentVolumeSteps) & ", "
                        End If

                        If (tparTestReagentsVolumeDR.IsIncPostReagentVolumeNull) Then
                            values &= " IncPostReagentVolume = NULL, "
                        Else
                            values &= " IncPostReagentVolume = " & ReplaceNumericString(tparTestReagentsVolumeDR.IncPostReagentVolume) & ", "
                        End If

                        If (tparTestReagentsVolumeDR.IsIncPostReagentVolumeStepsNull) Then
                            values &= " IncPostReagentVolumeSteps = NULL "
                        Else
                            values &= " IncPostReagentVolumeSteps = " & ReplaceNumericString(tparTestReagentsVolumeDR.IncPostReagentVolumeSteps)
                        End If

                        cmdText = " UPDATE tparTestReagentsVolumes SET " & values & _
                                  " WHERE  TestID        =  " & tparTestReagentsVolumeDR.TestID.ToString() & _
                                  " AND    ReagentID     =  " & tparTestReagentsVolumeDR.ReagentID.ToString() & _
                                  " AND    ReagentNumber =  " & tparTestReagentsVolumeDR.ReagentNumber.ToString() & _
                                  " AND    SampleType    = '" & tparTestReagentsVolumeDR.SampleType.Replace("'", "''") & "'"

                        cmd.Connection = pDBConnection
                        cmd.CommandText = cmdText

                        myGlobalDataTO.AffectedRecords = cmd.ExecuteNonQuery()
                        myGlobalDataTO.HasError = False
                    Next
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparTestReagentsVolumeDAO.Update", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function
#End Region

#Region "Other Methods"
        ''' <summary>
        ''' Delete volumes of an specific Reagent when used for a Test/SampleType
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestID">Test Identifier</param>
        ''' <param name="pSampleType">SampleType Code</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  TR 16/03/2010
        ''' Modified by: SA 28/10/2010 - Removed error raising depending value of affected records 
        ''' </remarks>
        Public Function DeleteByTestIDAndSampleType(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer, ByVal pSampleType As String) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = " DELETE FROM tparTestReagentsVolumes " & vbCrLf & _
                                            " WHERE  TestID     = " & pTestID.ToString & vbCrLf & _
                                            " AND    SampleType = '" & pSampleType.ToString & "' " & vbCrLf

                    Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                        myGlobalDataTO.AffectedRecords = dbCmd.ExecuteNonQuery()
                        myGlobalDataTO.HasError = False
                    End Using
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparTestReagentsVolumeDAO.DeleteByTestIDAndSampleType", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Delete volumes of an specific Reagent when used for a Test
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestID">Test Identifier</param>
        ''' <param name="pReagentNumber">Reagent Number</param>
        ''' <param name="pReagentID">Reagent Identifier</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  TR 15/03/2010
        ''' Modified by: SA 28/10/2010 - Removed error raising depending value of affected records 
        ''' </remarks>
        Public Function DeleteByTestIDReagNumberReagID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer, _
                                                       ByVal pReagentNumber As Integer, ByVal pReagentID As Integer) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = " DELETE FROM tparTestReagentsVolumes " & vbCrLf & _
                                            " WHERE  TestID = " & pTestID.ToString & vbCrLf & _
                                            " AND    ReagentID = " & pReagentID.ToString & vbCrLf & _
                                            " AND    ReagentNumber = " & pReagentNumber.ToString & vbCrLf

                    Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                        myGlobalDataTO.AffectedRecords = dbCmd.ExecuteNonQuery()
                        myGlobalDataTO.HasError = False
                    End Using
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparTestReagentsVolumeDAO.DeleteByTestIDReagNumberReagID", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function
#End Region
    End Class
End Namespace