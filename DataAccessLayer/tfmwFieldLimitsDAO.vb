Option Strict On
Option Explicit On

Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Global.GlobalEnumerates
Imports Biosystems.Ax00.Types.FieldLimitsDS

Namespace Biosystems.Ax00.DAL.DAO

    Public Class tfmwFieldLimitsDAO
        Inherits DAOBase

#Region "CRUD Methods"

        ''' <summary>
        ''' Get minimum and maximum allowed values for the specified Limit and optionally, by an specific Analyzer Model
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pLimitID">Unique identifier of the limit id of FieldLimitsDelegate</param>
        ''' <param name="pAnalyzerModel">Optional parameter; Analyzer Model. It is used with values of the specified Limit are different for each 
        '''                              available Analyzer Model</param>
        ''' <returns>GlobalDataTO containing a typed DataSet FieldLimitsDS with the minimum and maximum allowed values for the specified Limit</returns>
        ''' <remarks>
        ''' Created by:
        ''' Modified by: SA 25/02/2010 - Changed datatype of parameter pLimitID 
        '''              SA 16/02/2012 - Changed the function template
        ''' </remarks>
        Public Function Read(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pLimitID As FieldLimitsEnum, Optional ByVal pAnalyzerModel As String = "") As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = ""
                        cmdText &= "SELECT LimitID, LimitDescription, MinValue, MaxValue, StepValue, DefaultValue, DecimalsAllowed, AnalyzerModel " & vbCrLf
                        cmdText &= "  FROM tfmwFieldLimits " & vbCrLf
                        cmdText &= " WHERE  LimitID = '" & pLimitID.ToString & "' " & vbCrLf

                        If (pAnalyzerModel.Length > 0) Then cmdText &= " AND AnalyzerModel = '" & pAnalyzerModel.ToString & "' " & vbCrLf

                        Dim fieldLimitDataDS As New FieldLimitsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(fieldLimitDataDS.tfmwFieldLimits)
                            End Using
                        End Using

                        resultData.SetDatos = fieldLimitDataDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tfmwFieldLimitsDAO.Read", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get minimum and maximum allowed values for all Limits, or optionally filtered by an specific Analyzer Model
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerModel">Optional parameter; Analyzer Model</param>
        ''' <returns>GlobalDataTO containing a typed DataSet FieldLimitsDS with data of all Limits (or all Limits defined for the specified 
        '''          Analyzer Model)</returns>
        ''' <remarks>
        ''' Created by:
        ''' Modified by: SG 01/09/2010
        ''' Modified by: DL 07/07/2011 - Added optional parameter for the AnalyzerModel and filter data for it when informed
        '''              SA 16/02/2012 - Changed the function template
        ''' </remarks>
        Public Function ReadAll(ByVal pDBConnection As SqlClient.SqlConnection, Optional ByVal pAnalyzerModel As String = "") As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT * FROM tfmwFieldLimits " & vbCrLf
                        If (pAnalyzerModel <> "") Then cmdText &= " WHERE AnalyzerModel = '" & pAnalyzerModel.Trim & "' " & vbCrLf

                        Dim fieldLimitDataDS As New FieldLimitsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(fieldLimitDataDS.tfmwFieldLimits)
                            End Using
                        End Using

                        resultData.SetDatos = fieldLimitDataDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tfmwFieldLimitsDAO.ReadAll", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function


        ''' <summary>
        ''' Update values for an specific Limit
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pLimitRow">Row of a Typed DataSet FieldLimitsDS containing all data of the Limit to update</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  DL 11/07/2011
        ''' Modified by: SA 16/02/2012 - Changed the function template
        ''' </remarks>
        Public Function Update(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pLimitRow As tfmwFieldLimitsRow) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = " UPDATE tfmwFieldLimits " & vbCrLf & _
                                            " SET    MinValue = " & ReplaceNumericString(pLimitRow.MinValue) & vbCrLf & _
                                                  ", MaxValue = " & ReplaceNumericString(pLimitRow.MaxValue) & vbCrLf & _
                                                  ", DecimalsAllowed = " & ReplaceNumericString(pLimitRow.DecimalsAllowed) & vbCrLf

                    If (Not pLimitRow.IsStepValueNull) Then cmdText &= ", StepValue = " & ReplaceNumericString(pLimitRow.StepValue) & vbCrLf
                    If (Not pLimitRow.IsDefaultValueNull) Then cmdText &= ", DefaultValue = " & ReplaceNumericString(pLimitRow.DefaultValue) & vbCrLf

                    cmdText &= " WHERE LimitID = '" & pLimitRow.LimitID.Trim & "' " & vbCrLf
                    If (Not pLimitRow.AnalyzerModel Is String.Empty) Then cmdText &= " AND AnalyzerModel = '" & pLimitRow.AnalyzerModel.Trim & "' " & vbCrLf

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
                GlobalBase.CreateLogActivity(ex.Message, "tfmwFieldLimitsDAO.Update", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

#End Region
    End Class
End Namespace

