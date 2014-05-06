Option Strict On
Option Explicit On

Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.DAL.DAO
Imports Biosystems.Ax00.DAL

Namespace Biosystems.Ax00.BL


    Public Class AnalyzerReactionsRotorDelegate

#Region "C R U D"

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pAnalyzerID"></param>
        ''' <returns></returns>
        ''' <remarks>AG 18/05/2011</remarks>
        Public Function Read(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then
                        If Not pAnalyzerID Is Nothing Then
                            If pAnalyzerID <> "" Then
                                Dim myDAO As New tcfgAnalyzerReactionsRotorDAO
                                resultData = myDAO.Read(dbConnection, pAnalyzerID)
                            Else
                                resultData.SetDatos = Nothing
                            End If
                        End If

                        End If
                    End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerReactionsRotorDelegate.Read", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function


        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pEntryDS"></param>
        ''' <returns></returns>
        ''' <remarks>AG 18/05/2011</remarks>
        Public Function Create(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pEntryDS As AnalyzerReactionsRotorDS) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection
            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError) And (Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        If Not pEntryDS Is Nothing Then
                            If pEntryDS.tcfgAnalyzerReactionsRotor.Rows.Count > 0 Then
                                Dim myDAO As New tcfgAnalyzerReactionsRotorDAO
                                resultData = myDAO.Create(dbConnection, pEntryDS)
                            End If
                        End If

                        If (Not resultData.HasError) Then
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

                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerReactionsRotorDelegate.Create", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function


        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pEntryDS"></param>
        ''' <returns></returns>
        ''' <remarks>AG 18/05/2011</remarks>
        Public Function Update(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pEntryDS As AnalyzerReactionsRotorDS) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection
            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError) And (Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        If Not pEntryDS Is Nothing Then
                            If pEntryDS.tcfgAnalyzerReactionsRotor.Rows.Count > 0 Then
                                Dim myDAO As New tcfgAnalyzerReactionsRotorDAO
                                resultData = myDAO.Update(dbConnection, pEntryDS)
                            End If
                        End If

                        If (Not resultData.HasError) Then
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

                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerReactionsRotorDelegate.Update", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function


        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pAnalyzerID "></param>
        ''' <returns></returns>
        ''' <remarks>AG 18/05/2011</remarks>
        Public Function Delete(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection
            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError) And (Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        If Not pAnalyzerID Is Nothing Then
                            If pAnalyzerID <> "" Then
                                Dim myDAO As New tcfgAnalyzerReactionsRotorDAO
                                resultData = myDAO.Delete(dbConnection, pAnalyzerID)
                            End If
                        End If

                        If (Not resultData.HasError) Then
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

                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerReactionsRotorDelegate.Delete", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

#End Region

#Region "Other public methods"

        ''' <summary>
        ''' Read and evaluates the current reactions rotor in analyzer an return a message code if Sw needs to warn the final user
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pAnalyzerID"></param>
        ''' <param name="pAnalyzerModel" ></param>
        ''' <returns>GlobalDataTo (SetDatos as String)</returns>
        ''' <remarks>AG 18/05/2011</remarks>
        Public Function ChangeReactionsRotorRecommended(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
                                                        ByVal pAnalyzerModel As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection
            Dim messageID As String = ""

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then
                        If Not pAnalyzerID Is Nothing Then
                            If pAnalyzerID <> "" Then
                                Dim myDAO As New tcfgAnalyzerReactionsRotorDAO
                                resultData = myDAO.Read(dbConnection, pAnalyzerID)

                                If Not resultData.HasError And Not resultData.SetDatos Is Nothing Then
                                    Dim myDS As New AnalyzerReactionsRotorDS
                                    myDS = CType(resultData.SetDatos, AnalyzerReactionsRotorDS)

                                    If myDS.tcfgAnalyzerReactionsRotor.Rows.Count > 0 Then
                                        Dim showMessageFlag As Boolean = False

                                        If myDS.tcfgAnalyzerReactionsRotor(0).BLParametersRejected Then
                                            showMessageFlag = True
                                        End If

                                        If Not showMessageFlag Then

                                            'Read the Sw parameter limits
                                            Dim SwParametersDelegate As New SwParametersDelegate

                                            resultData = SwParametersDelegate.ReadByAnalyzerModel(dbConnection, pAnalyzerModel)
                                            If Not resultData.HasError And Not resultData.SetDatos Is Nothing Then
                                                Dim SwParamDS As New ParametersDS
                                                SwParamDS = CType(resultData.SetDatos, ParametersDS)

                                                Dim myList As List(Of ParametersDS.tfmwSwParametersRow)
                                                myList = (From a As ParametersDS.tfmwSwParametersRow In SwParamDS.tfmwSwParameters _
                                                          Where a.ParameterName = GlobalEnumerates.SwParameters.MAX_REJECTED_WELLS.ToString Select a).ToList

                                                If myList.Count > 0 Then
                                                    If myDS.tcfgAnalyzerReactionsRotor(0).WellsRejectedNumber > CInt(myList(0).ValueNumeric) Then
                                                        showMessageFlag = True
                                                    End If
                                                End If

                                                If Not showMessageFlag Then
                                                    myList = (From a As ParametersDS.tfmwSwParametersRow In SwParamDS.tfmwSwParameters _
                                                              Where a.ParameterName = GlobalEnumerates.SwParameters.MAX_REACTROTOR_DAYS.ToString Select a).ToList

                                                    If myList.Count > 0 Then
                                                        Dim daysNumber As TimeSpan
                                                        daysNumber = Now.Subtract(myDS.tcfgAnalyzerReactionsRotor(0).InstallDate)

                                                        If daysNumber.Days > CInt(myList(0).ValueNumeric) Then
                                                            showMessageFlag = True
                                                        End If
                                                    End If


                                                End If
                                            End If

                                        End If

                                        If showMessageFlag Then messageID = GlobalEnumerates.Messages.CHANGE_REACTROTOR_RECOMMEND.ToString

                                    End If
                                End If

                            End If

                        End If
                    End If

                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerReactionsRotorDelegate.ChangeReactionsRotorRecommended", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            resultData.SetDatos = messageID
            Return resultData
        End Function

        ''' <summary>
        ''' Implements the proper business when a change rotor has been performed succesfully
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pAnalyzerID"></param>
        ''' <returns></returns>
        ''' <remarks>AG 30/06/2011</remarks>
        Public Function ChangeRotorPerformed(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection
            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError) And (Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        If Not pAnalyzerID Is Nothing Then
                            If pAnalyzerID <> "" Then
                                Dim myDAO As New tcfgAnalyzerReactionsRotorDAO
                                resultData = myDAO.Delete(dbConnection, pAnalyzerID)

                                If Not resultData.HasError Then
                                    Dim createDS As New AnalyzerReactionsRotorDS
                                    Dim row As AnalyzerReactionsRotorDS.tcfgAnalyzerReactionsRotorRow
                                    row = createDS.tcfgAnalyzerReactionsRotor.NewtcfgAnalyzerReactionsRotorRow
                                    row.BeginEdit()
                                    row.AnalyzerID = pAnalyzerID
                                    row.InstallDate = Now
                                    row.BLParametersRejected = False
                                    row.WellsRejectedNumber = 0
                                    row.EndEdit()
                                    createDS.tcfgAnalyzerReactionsRotor.AddtcfgAnalyzerReactionsRotorRow(row)
                                    createDS.AcceptChanges()
                                    resultData = myDAO.Create(dbConnection, createDS)
                                End If
                            End If
                        End If

                        If (Not resultData.HasError) Then
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

                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerReactionsRotorDelegate.ChangeRotorPerformed", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function


#End Region




    End Class

End Namespace