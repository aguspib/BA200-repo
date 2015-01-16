Option Explicit On
Option Strict On

Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.DAL.DAO
Imports Biosystems.Ax00.Global


Namespace Biosystems.Ax00.BL
    Public Class SwParametersDelegate


#Region "Methods"


        ''' <summary>
        ''' Get number of multiitem 
        ''' </summary>
        ''' <param name="pDBConnection">Open Database Connection</param>
        ''' <param name="pParameterName">Execution identifier></param>
        ''' <param name="pAnalyzerModel">Execution identifier></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ReadByParameterName(ByVal pDBConnection As SqlClient.SqlConnection, _
                                            ByVal pParameterName As String, _
                                            ByVal pAnalyzerModel As String) As GlobalDataTO

            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim mytfmwSwParametersDAO As New tfmwSwParametersDAO
                        resultData = mytfmwSwParametersDAO.ReadByParameterName(dbConnection, pParameterName, pAnalyzerModel)
                    End If
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = "SYSTEM_ERROR"
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "SwParametersDelegate.ReadByParameterName", EventLogEntryType.Error, False)
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
        ''' Created by:  TR 22/02/2010
        ''' Modified by: SA 11/07/2012 - Added optional parameter for the AnalyzerID.  When informed, parameters returned will be those
        '''                              defined for the model of the specified Analyzer plus those defined for all models
        ''' </remarks>
        Public Function ReadByAnalyzerModel(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerModel As String, _
                                            Optional ByVal pAnalyzerID As String = "") As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim mytfmwSwParametersDAO As New tfmwSwParametersDAO
                        myGlobalDataTO = mytfmwSwParametersDAO.ReadByAnalyzerModel(dbConnection, pAnalyzerModel, pAnalyzerID)
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "SwParametersDelegate.ReadByAnalyzerModel", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
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
        ''' </remarks>
        Public Function GetParameterByAnalyzer(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
                                               ByVal pParameterName As String, ByVal pDependOnModel As Boolean) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError) AndAlso (Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New tfmwSwParametersDAO
                        resultData = myDAO.GetParameterByAnalyzer(dbConnection, pAnalyzerID, pParameterName, pDependOnModel)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "SwParametersDelegate.GetParameterByAnalyzer", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function


        ''' <summary>
        ''' Get the values of a parameter in tfmwSwParameters by AnalyzerID
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pAnalyzerModel"></param>
        ''' <returns>GlobalDataTo with data as ParametersDS</returns>
        ''' <remarks>Created by AG 23/09/2010</remarks>
        Public Function GetAllList(ByVal pDBConnection As SqlClient.SqlConnection, Optional ByVal pAnalyzerModel As String = "") As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)

                If (Not resultData.HasError) And (Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New tfmwSwParametersDAO
                        resultData = myDAO.GetAllList(dbConnection, pAnalyzerModel)
                    End If
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = "SYSTEM_ERROR"
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "AnalyzersDelegate.GetAllList", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get value of specific parameter
        ''' </summary>
        ''' <param name="pDBConnection">Open Database Connection</param>
        ''' <param name="pParameterName">Execution identifier></param>
        ''' <param name="pAnalyzerModel">Execution identifier></param>
        ''' <returns></returns>
        ''' <remarks>SGM 08/03/11</remarks>
        Public Function ReadTextValueByParameterName(ByVal pDBConnection As SqlClient.SqlConnection, _
                                            ByVal pParameterName As String, _
                                            ByVal pAnalyzerModel As String) As GlobalDataTO

            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim mytfmwSwParametersDAO As New tfmwSwParametersDAO
                        resultData = mytfmwSwParametersDAO.ReadByParameterName(dbConnection, pParameterName, pAnalyzerModel)
                        If Not resultData.HasError And resultData.SetDatos IsNot Nothing Then
                            Dim myParamsDS As ParametersDS
                            myParamsDS = CType(resultData.SetDatos, ParametersDS)
                            resultData.SetDatos = myParamsDS.tfmwSwParameters(0).ValueText
                        End If
                    End If
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = "SYSTEM_ERROR"
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "SwParametersDelegate.ReadTextValueByParameterName", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Read the numeric value of the specified Software Parameter for the specified Analyzer Model
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pParameterName">Name of the Software Parameter to read</param>
        ''' <param name="pAnalyzerModel">Analyzer Model. If the Software Parameter is not dependent of the Analyzer Model, this function parameter has to be informed as Nothing</param>
        ''' <returns>GlobalDataTO containg an integer with the current value of the specified SW Parameter for the informed Analyzer Model</returns>
        ''' <remarks>
        ''' Created by:  SG 08/03/2011
        ''' Modified by: SA 11/11/2014 - BA-1885 ==> Some code improvements
        ''' </remarks>
        Public Function ReadNumValueByParameterName(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pParameterName As String, ByVal pAnalyzerModel As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim mySwParametersDAO As New tfmwSwParametersDAO

                        resultData = mySwParametersDAO.ReadByParameterName(dbConnection, pParameterName, pAnalyzerModel)
                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            Dim myParamsDS As ParametersDS = DirectCast(resultData.SetDatos, ParametersDS)
                            If (myParamsDS.tfmwSwParameters.Rows.Count > 0) Then
                                resultData.SetDatos = myParamsDS.tfmwSwParameters.First.ValueNumeric
                            Else
                                'If the SW Parameter does not exist in the table, it is an error
                                resultData.ErrorCode = GlobalEnumerates.Messages.MASTER_DATA_MISSING.ToString
                                resultData.HasError = True
                            End If
                        End If
                    End If
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "SwParametersDelegate.ReadNumValueByParameterName", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Update Parameters settings
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pParametersDS"></param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  DL 11/07/2011
        ''' </remarks>
        Public Function SaveParameters(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pParametersDS As ParametersDS) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then

                        For Each ParametersRow As ParametersDS.tfmwSwParametersRow In pParametersDS.tfmwSwParameters.Rows
                            'Update data of the existing Test/Control

                            Dim myParameters As New tfmwSwParametersDAO
                            myGlobalDataTO = myParameters.Update(dbConnection, ParametersRow)

                            If (myGlobalDataTO.HasError) Then Exit For

                        Next ParametersRow

                        If (Not myGlobalDataTO.HasError) Then
                            'When the Database Connection was opened locally, then the Commit is executed
                            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                        Else
                            'When the Database Connection was opened locally, then the Rollback is executed
                            If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                        End If
                    End If
                End If
            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "FieldLimitsDelegate.SaveLimits", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get the values of a parameter in tfmwSwParameters related to the ISE Module
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pAnalyzerModel"></param>
        ''' <returns>GlobalDataTo with data as ParametersDS</returns>
        ''' <remarks>Created by SGM 01/02/2012</remarks>
        Public Function GetAllISEList(ByVal pDBConnection As SqlClient.SqlConnection, Optional ByVal pAnalyzerModel As String = "") As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)

                If (Not resultData.HasError) And (Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New tfmwSwParametersDAO
                        resultData = myDAO.GetAllISEList(dbConnection, pAnalyzerModel)
                    End If
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = "SYSTEM_ERROR"
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "AnalyzersDelegate.GetAllList", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

#End Region

    End Class
End Namespace