Option Strict On
Option Explicit On

Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Types.ParametersDS

Namespace Biosystems.Ax00.DAL.DAO

    Public Class tfmwSwParametersDAO
          

#Region "CRUD Methods"

        ''' <summary>
        ''' Get details of the specified Parameter for the specified Analyzer Model
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pParameterName">Parameter Name</param>
        ''' <param name="pAnalyzerModel">Analyzer Model</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by: DL 19/02/2010
        ''' </remarks>
        Public Function ReadByParameterName(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pParameterName As String, _
                                            ByVal pAnalyzerModel As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = ""

                        cmdText += " SELECT ParameterID, ParameterName, DependByModel, AnalyzerModel, ValueNumeric, ValueText " & vbCrLf
                        cmdText += " FROM   tfmwSwParameters " & vbCrLf
                        cmdText += " WHERE  ParameterName = '" & pParameterName & "'" & vbCrLf

                        'SGM 08/03/11
                        If pAnalyzerModel IsNot Nothing Then
                            cmdText += " AND   (AnalyzerModel = '" & pAnalyzerModel & "') "
                        Else
                            cmdText += " AND   (AnalyzerModel IS NULL) "
                        End If

                        Dim myParameterDS As New ParametersDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myParameterDS.tfmwSwParameters)
                            End Using
                        End Using

                        resultData.SetDatos = myParameterDS
                        resultData.HasError = False
                    End If
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tfmwSwParametersDAO.ReadByParameterName", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get details of all Parameters that are not dependend on the Analyzer Model plus:
        ''' ** All Parameters defined for the specified Analyzer Model (pAnalyzerModel is informed) OR
        ''' ** All Parameters defined for the model of the specified Analyzer Identifier (pAnalyzerID is informed)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerModel">Analyzer Model</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier. Optional parameter</param>
        ''' <returns>GlobalDataTO containing a typed DataSet ParametersDS with all Parameters that fulfill the searching criteria</returns>
        ''' <remarks>
        ''' Created by:  TR 25/02/2010
        ''' Modified by: SA 11/07/2012 - Added optional parameter for the AnalyzerID.  When informed, parameters returned will be those
        '''                              defined for the model of the specified Analyzer plus those defined for all models
        ''' </remarks>
        Public Function ReadByAnalyzerModel(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerModel As String, _
                                            Optional ByVal pAnalyzerID As String = "") As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT ParameterID, ParameterName, DependByModel, AnalyzerModel, ValueNumeric, ValueText " & vbCrLf & _
                                                " FROM   tfmwSwParameters " & vbCrLf & _
                                                " WHERE  AnalyzerModel IS NULL " & vbCrLf

                        If (pAnalyzerModel.Trim <> String.Empty) Then
                            cmdText &= " OR AnalyzerModel ='" & pAnalyzerModel.Trim & "' " & vbCrLf
                        ElseIf (pAnalyzerID.Trim <> String.Empty) Then
                            cmdText &= " OR AnalyzerModel IN (SELECT AnalyzerModel FROM tcfgAnalyzers " & vbCrLf & _
                                                            " WHERE AnalyzerID = N'" & pAnalyzerID.Trim.Replace("'", "''") & "') " & vbCrLf
                        End If

                        Dim myParameterDS As New ParametersDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myParameterDS.tfmwSwParameters)
                            End Using
                        End Using

                        resultData.SetDatos = myParameterDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tfmwSwParametersDAO.ReadByAnalyzerModel", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get value of the specified Software Parameter. If value of Parameter depends on an Analyzer Model, then the function
        ''' get the value for the Model of the informed Analyzer
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pParameterName">Parameter Name</param>
        ''' <param name="pDependOnModel">Flag indicating if the Parameter value is common to all Analyzer Models or it is specific
        '''                              for an Analyzer Model</param>
        ''' <returns>GlobalDataTO containing a typed DataSet ParametersDS with the current value of the specified Parameter</returns>
        ''' <remarks>
        ''' Created by:  AG 23/09/2010
        ''' Modified by: SA 21/10/2011 - Changed the function template
        ''' </remarks>
        Public Function GetParameterByAnalyzer(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
                                               ByVal pParameterName As String, ByVal pDependOnModel As Boolean) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT ParameterID, ParameterName, DependByModel, AnalyzerModel, ValueNumeric, ValueText " & vbCrLf & _
                                                " FROM   tfmwSwParameters P " & vbCrLf & _
                                                " WHERE  P.ParameterName = '" & pParameterName & "' " & vbCrLf

                        If (pDependOnModel) Then
                            cmdText &= " AND P.AnalyzerModel IN (SELECT A.AnalyzerModel FROM tcfgAnalyzers A " & vbCrLf & _
                                                               " WHERE  A.AnalyzerID = '" & pAnalyzerID & "' )"
                        End If

                        Dim myParameterDS As New ParametersDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myParameterDS.tfmwSwParameters)
                            End Using
                        End Using

                        resultData.SetDatos = myParameterDS
                        resultData.HasError = False
                    End If
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tfmwSwParametersDAO.GetParameterByAnalyzer", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerModel">Analyzer Model</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by:  
        ''' </remarks>
        Public Function GetAllList(ByVal pDBConnection As SqlClient.SqlConnection, Optional ByVal pAnalyzerModel As String = "") As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError) And (Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = ""
                        cmdText &= "SELECT ParameterID, ParameterName, DependByModel, AnalyzerModel, ValueNumeric, ValueText, Description " & vbCrLf
                        cmdText &= "  FROM tfmwSwParameters " & vbCrLf

                        If pAnalyzerModel <> "" Then cmdText &= " WHERE AnalyzerModel = '" & pAnalyzerModel & "' "

                        Dim dbCmd As New SqlClient.SqlCommand
                        dbCmd.Connection = dbConnection
                        dbCmd.CommandText = cmdText

                        'Fill the DataSet to return 
                        Dim myParameterDS As New ParametersDS
                        Dim dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                        dbDataAdapter.Fill(myParameterDS.tfmwSwParameters)

                        resultData.SetDatos = myParameterDS
                        resultData.HasError = False
                    End If
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tfmwSwParametersDAO.GetAllList", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Update values for a Parameter
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pParameterRow">Typed DataSet limit</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  DL 11/07/2011
        ''' </remarks>
        Public Function Update(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pParameterRow As tfmwSwParametersRow) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = ""
                    cmdText &= "UPDATE tfmwSwParameters " & vbCrLf
                    cmdText &= "   SET"

                    If Not pParameterRow.IsValueNumericNull Then
                        cmdText &= " ValueNumeric = " & ReplaceNumericString(pParameterRow.ValueNumeric) & vbCrLf
                    Else
                        cmdText &= " ValueNumeric = NULL" & vbCrLf
                    End If

                    If Not pParameterRow.IsValueTextNull Then
                        cmdText &= "     , ValueText = N'" & pParameterRow.ValueText.ToString.Replace("'", "''") & "'" & vbCrLf
                    Else
                        cmdText &= "     , ValueText = NULL" & vbCrLf
                    End If

                    cmdText &= " WHERE ParameterID = '" & pParameterRow.ParameterID & "'" & vbCrLf

                    Dim dbCmd As New SqlClient.SqlCommand
                    dbCmd.Connection = pDBConnection
                    dbCmd.CommandText = cmdText

                    resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                    resultData.HasError = (resultData.AffectedRecords = 0)
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tfmwFieldLimitsDAO.Update", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Gets only ISE related parameters
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerModel">Analyzer Identifier</param>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 01/02/2012</remarks>
        Public Function GetAllISEList(ByVal pDBConnection As SqlClient.SqlConnection, Optional ByVal pAnalyzerModel As String = "") As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError) And (Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = ""
                        cmdText &= "SELECT ParameterID, ParameterName, DependByModel, AnalyzerModel, ValueNumeric, ValueText, Description " & vbCrLf
                        cmdText &= "  FROM tfmwSwParameters " & vbCrLf
                        cmdText &= " WHERE ParameterName LIKE 'ISE_%' "

                        If pAnalyzerModel <> "" Then cmdText &= " AND AnalyzerModel = '" & pAnalyzerModel & "' "

                        Dim dbCmd As New SqlClient.SqlCommand
                        dbCmd.Connection = dbConnection
                        dbCmd.CommandText = cmdText

                        'Fill the DataSet to return 
                        Dim myParameterDS As New ParametersDS
                        Dim dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                        dbDataAdapter.Fill(myParameterDS.tfmwSwParameters)

                        resultData.SetDatos = myParameterDS
                        resultData.HasError = False
                    End If
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tfmwSwParametersDAO.GetAllISEList", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function
#End Region

    End Class

End Namespace
