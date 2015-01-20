

Option Explicit On
Option Strict On


Imports System.Data.SqlClient
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global

Namespace Biosystems.Ax00.DAL.DAO


    Partial Public Class tparTestSamplesDAO
        Inherits DAOBase

#Region "AUTOMATICALLY GENERATED CODE"

#Region "CRUD"

#Region "READ"

        Public Function Read(ByVal TestID As Integer, ByVal SampleType As String) As TestSamplesDS

            Dim conn As New SqlClient.SqlConnection(GetConnectionString())
            Dim result As New TestSamplesDS()
            Try
                conn.Open()
                Dim cmdText As String = _
                "SELECT * " & _
                "FROM tparTestSamples " & " WHERE " & "TestID = " & TestID.ToString() & " AND " & "SampleType = '" & SampleType.ToString() & "'"

                Dim cmd As SqlClient.SqlCommand = conn.CreateCommand()
                cmd.CommandText = cmdText
                cmd.Connection = conn
                Dim da As New SqlClient.SqlDataAdapter(cmd)
                da.Fill(result.tparTestSamples)

            Catch ex As ArgumentException
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "TparTestSamplesDAO", EventLogEntryType.Error, False)

            Catch ex As InvalidOperationException
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "TparTestSamplesDAO", EventLogEntryType.Error, False)

            Catch ex As Exception
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "TparTestSamplesDAO", EventLogEntryType.Error, False)
            Finally
                conn.Close()
            End Try
            Return result
        End Function


        ''' <summary>
        ''' Update value of field DefaultSampleType for the specified Test/SampleType 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestID">Test Identifier</param>
        ''' <param name="pSampleType">Sample Type Code</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  DL 25/06/2012
        ''' </remarks>
        Public Function UpdateDefaultSampleType(ByVal pDBConnection As SqlClient.SqlConnection, _
                                                ByVal pTestID As Integer, _
                                                ByVal pSampleType As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = ""

                    cmdText &= "UPDATE tparTestSamples " & vbCrLf
                    cmdText &= "   SET DefaultSampleType = 1" & vbCrLf
                    cmdText &= " WHERE TestID = " & pTestID & vbCrLf
                    cmdText &= "   AND SampleType = '" & pSampleType & "' "

                    Dim cmd As New SqlCommand
                    cmd.Connection = pDBConnection
                    cmd.CommandText = cmdText

                    resultData.AffectedRecords = cmd.ExecuteNonQuery()
                    resultData.HasError = (resultData.AffectedRecords = 0)
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparTestSamplesDAO.UpdateDefaultSampleType", EventLogEntryType.Error, False)
            End Try

            Return resultData

        End Function

#End Region 'READ

#End Region 'CRUD
#End Region 'AUTOMATICALLY GENERATED CODE"

    End Class
End Namespace

  