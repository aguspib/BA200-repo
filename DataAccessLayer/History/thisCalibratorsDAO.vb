Option Explicit On
Option Strict On

Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Types
Imports System.Text

Namespace Biosystems.Ax00.DAL.DAO
    Public Class thisCalibratorsDAO
        Inherits DAOBase

#Region "CRUD Methods"
        ''' <summary>
        ''' A Calibrator is closed in Historics Module (field ClosedCalibrator is set to TRUE) in following cases:
        '''   ** When the LotNumber and/or the Number of Calibrators is changed in Calibrators Programming Screen
        '''   ** When the Calibrator is deleted in Calibrators Programming screen
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pHistCalibratorID">Calibrator Identifier in Historics Module</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 14/03/2012
        ''' </remarks>
        Public Function CloseCalibrator(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pHistCalibratorID As Integer) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString()
                Else
                    Dim cmdText As New StringBuilder
                    cmdText.AppendLine(" UPDATE thisCalibrators SET ClosedCalibrator = 1 ")
                    cmdText.AppendFormat(" WHERE HistCalibratorID = {0} ", pHistCalibratorID)

                    Using dbCmd As New SqlClient.SqlCommand(cmdText.ToString, pDBConnection)
                        resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                        resultData.HasError = False
                    End Using
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "thisCalibratorsDAO.CloseCalibrator", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Add a list of Calibrators to the corresponding table in Historics Module
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pHisCalibratorsDS">Typed DataSet HisCalibratorsDS containing all Calibrators to add</param>
        ''' <returns>GlobalDataTO containing a typed DataSet HisCalibratorsDS with all created Calibrators with the generated 
        '''          HistCalibratorID</returns>
        ''' <remarks>
        ''' Created by:  SA 14/03/2012
        ''' </remarks>
        Public Function Create(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pHisCalibratorsDS As HisCalibratorsDS) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString()
                Else
                    Dim cmdText As New StringBuilder
                    Dim histCalibID As Integer = -1

                    Using dbCmd As New SqlClient.SqlCommand
                        dbCmd.Connection = pDBConnection

                        For Each row As HisCalibratorsDS.thisCalibratorsRow In pHisCalibratorsDS.thisCalibrators.Rows
                            cmdText.Append(" INSERT INTO thisCalibrators (CalibratorID, CalibratorName, LotNumber, NumberOfCalibrators) ")
                            cmdText.AppendFormat(" VALUES({0}, N'{1}', N'{2}', {3}) ", row.CalibratorID, row.CalibratorName.Replace("'", "''"), _
                                                   row.LotNumber.Replace("'", "''"), row.NumberOfCalibrators)
                            cmdText.AppendFormat(" {0} SELECT SCOPE_IDENTITY() ", vbCrLf)
                            dbCmd.CommandText = cmdText.ToString()

                            'Execute the SQL script 
                            histCalibID = CType(dbCmd.ExecuteScalar(), Integer)
                            If (histCalibID > 0) Then
                                row.HistCalibratorID = histCalibID
                            End If
                            cmdText.Length = 0 'Instead of using Remove use the Lenght = 0 
                        Next
                    End Using

                    resultData.SetDatos = pHisCalibratorsDS
                    resultData.HasError = False
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "thisCalibratorsDAO.Create", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Delete all not in use closed Calibrators
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 01/07/2013
        ''' </remarks>
        Public Function DeleteClosedNotInUse(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString()
                Else
                    Dim cmdText As String = " DELETE FROM thisCalibrators " & vbCrLf & _
                                            " WHERE  ClosedCalibrator = 1 " & vbCrLf & _
                                            " AND    HistCalibratorID NOT IN (SELECT HistCalibratorID FROM thisTestCalibratorsValues) " & vbCrLf & _
                                            " AND    HistCalibratorID NOT IN (SELECT HistCalibratorID FROM thisTestSamples) " & vbCrLf

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
                GlobalBase.CreateLogActivity(ex.Message, "thisCalibratorsDAO.DeleteClosedNotInUse", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Verify if an open version of the Calibrator already exists in Historics Module and in this case, get the its data
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pCalibratorID">Calibrator Identifier in Parameters Programming</param>
        ''' <returns>GlobalDataTO containing a typed DS HisCalibratorsDS with data of the Calibrator</returns>
        ''' <remarks>
        ''' Created by:  SA 14/03/2012
        ''' </remarks>
        Public Function ReadByCalibratorID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pCalibratorID As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As New StringBuilder
                        cmdText.AppendLine(" SELECT * FROM thisCalibrators ")

                        cmdText.AppendFormat(" WHERE CalibratorID = {0} ", pCalibratorID)
                        cmdText.AppendFormat(" AND   ClosedCalibrator = 0 ")

                        Dim myDS As New HisCalibratorsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText.ToString(), dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myDS.thisCalibrators)
                            End Using
                        End Using

                        resultData.SetDatos = myDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "thisCalibratorsDAO.ReadByCalibratorID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Update fields CalibratorName, LotNumber and MultipointCalibrator for an specific Calibrator
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pHisCalibratorsDS">Typed DataSet HisCalibratorsDS containing all Calibrators to update</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 14/03/2012 
        ''' </remarks>
        Public Function Update(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pHisCalibratorsDS As HisCalibratorsDS) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString()
                Else
                    Dim cmdText As New StringBuilder
                    Dim row As HisCalibratorsDS.thisCalibratorsRow = pHisCalibratorsDS.thisCalibrators(0)

                    Using dbCmd As New SqlClient.SqlCommand()
                        cmdText.AppendLine(" UPDATE thisCalibrators ")
                        cmdText.AppendFormat(" SET CalibratorName = N'{0}', LotNumber = N'{1}', NumberOfCalibrators = {2} " & vbCrLf, _
                                              row.CalibratorName, row.LotNumber, row.NumberOfCalibrators)
                        cmdText.AppendFormat(" WHERE HistCalibratorID = {0} ", row.HistCalibratorID)

                        dbCmd.Connection = pDBConnection
                        dbCmd.CommandText = cmdText.ToString()

                        resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                        resultData.HasError = False
                    End Using
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "thisCalibratorsDAO.Update", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function
#End Region
    End Class
End Namespace
