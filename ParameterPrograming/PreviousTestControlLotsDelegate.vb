Option Explicit On
Option Strict On

Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.Global

Namespace Biosystems.Ax00.BL

    Public Class PreviousTestControlLotsDelegate

#Region "Public Methods"
        ''' <summary>
        ''' Get all Tests/Sample Types linked to the previous saved Lot of the informed Control
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pControlID">Control Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet TestControlsDS with the list of Tests/SampleTypes 
        '''          linked to the informed Control for its previous saved Lot
        ''' </returns>
        ''' <remarks>
        ''' Created by:  DL
        ''' Modified by: SA 10/05/2011 - Parameter pControlID cannot be optional 
        ''' </remarks>
        Public Function GetPreviousLotTests(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pControlID As Integer) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Get list of defined controls
                        Dim myPreviousTestControlLots As New tparPreviousTestControlLotsDAO
                        resultData = myPreviousTestControlLots.ReadByControlID(dbConnection, pControlID)
                    End If
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "PreviousTestControlLotsDelegate.GetPreviousLotTests", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function


        '''' <summary>
        '''' Get all Controls linked to the specified TestID/SampleType
        '''' </summary>
        '''' <param name="pDBConnection">Open Database Connection</param>
        '''' <param name="pTestID">Test Identifier</param>
        '''' <param name="pSampleType">Sample Type Code</param>
        '''' <returns>GlobalDataTO containing a TestControlsDS</returns>
        '''' <remarks></remarks>
        'Public Function GetControls(ByVal pDBConnection As SqlClient.SqlConnection, _
        '                            ByVal pTestID As Integer, _
        '                            ByVal pSampleType As String, _
        '                            Optional ByVal pOnlyActiveControls As Boolean = False) As GlobalDataTO
        '    Dim resultData As New GlobalDataTO
        '    Dim dbConnection As New SqlClient.SqlConnection

        '    Try
        '        resultData = DAOBase.GetOpenDBConnection(pDBConnection)
        '        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
        '            dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then

        '                Dim myTestControls As New tparTestControlsDAO
        '                resultData = myTestControls.ReadByTestIDAndSampleType(dbConnection, pTestID, pSampleType, 0, pOnlyActiveControls)
        '            End If
        '        End If
        '    Catch ex As Exception
        '        resultData.HasError = True
        '        resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        resultData.ErrorMessage = ex.Message

        '        'Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "PreviousTestControlLotsDelegate.GetControls", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return resultData
        'End Function

        ''' <summary>
        ''' Delete the specified Test/SampleType linked to the previous saved Lot of the informed Control
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pControlID">Identifier of the Control to delete</param>        
        ''' <param name="pTestID">Test Identifier</param>
        ''' <param name="pSampleType">SampleType Code</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 10/05/2011
        ''' </remarks>
        Public Function Delete(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pControlID As Integer, ByVal pTestID As Integer, _
                               ByVal pSampleType As String) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError) AndAlso (Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myPreviousControlLotsDAO As New tparPreviousTestControlLotsDAO
                        myGlobalDataTO = myPreviousControlLotsDAO.Delete(dbConnection, pControlID, pTestID, pSampleType)

                        If (Not myGlobalDataTO.HasError) Then
                            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                        Else
                            If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                        End If
                    End If
                End If

            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "PreviousTestControlLotsDelegate.Delete", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Delete all Tests/SampleTypes linked to the previous saved Lot of the informed Control
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pControlID">Identifier of the Control to delete</param>        
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 10/05/2011
        ''' </remarks>
        Public Function DeleteByControlID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pControlID As Integer) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError) AndAlso (Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myPreviousControlLotsDAO As New tparPreviousTestControlLotsDAO
                        myGlobalDataTO = myPreviousControlLotsDAO.DeleteByControlID(dbConnection, pControlID)

                        If (Not myGlobalDataTO.HasError) Then
                            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                        Else
                            If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                        End If
                    End If
                End If

            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "PreviousTestControlLotsDelegate.DeleteByControlID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Delete all information for a previous Control Lots for an specific Test, and optionally, a 
        ''' Sample Type and/or a list of Controls (the list will contain those Controls that have to remain linked
        ''' to the Test/SampleType)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestID">Test Identifier</param>
        ''' <param name="pSampleType">Sample Type; optional parameter</param>
        ''' <param name="pControlIDList">List of ControlIDs separated by (,). Optional parameter</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by:  TR 07/04/2011
        ''' Modified by: SA 11/05/2011 - Added optional parameter pControlIDList
        ''' </remarks>
        Public Function DeleteByTestID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer, Optional ByVal pSampleType As String = "", _
                                       Optional ByVal pControlIDList As String = "") As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError) AndAlso (Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myPreviousControlLotsDAO As New tparPreviousTestControlLotsDAO
                        myGlobalDataTO = myPreviousControlLotsDAO.DeleteByTestID(dbConnection, pTestID, pSampleType, pControlIDList)

                        If (Not myGlobalDataTO.HasError) Then
                            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                        Else
                            If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                        End If
                    End If
                End If

            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "PreviousTestControlLotsDelegate.DeleteByTestID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        '''' <summary>
        '''' Delete all Controls linked to the specified Standard Test and, optionally SampleType
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pControlID">Test Identifier</param>
        '''' <param name="pSampleType">Sample Type Code. Optional parameters</param>
        '''' <returns>GlobalDataTO containing success/error information</returns>
        '''' <remarks>
        '''' Created by: TR 17/05/2010
        '''' </remarks>
        'Public Function DeleteTestControlsByControlID(ByVal pDBConnection As SqlClient.SqlConnection, _
        '                                              ByVal pControlID As Integer, _
        '                                              Optional ByVal pSampleType As String = "") As GlobalDataTO
        '    Dim myGlobalDataTO As New GlobalDataTO
        '    Dim dbConnection As New SqlClient.SqlConnection
        '    Try
        '        myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
        '        If (Not myGlobalDataTO.HasError) AndAlso (Not myGlobalDataTO.SetDatos Is Nothing) Then
        '            dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim PreviousTestControlLots As New tparPreviousTestControlLotsDAO

        '                myGlobalDataTO = PreviousTestControlLots.Delete(dbConnection, pControlID, pSampleType)

        '                If (Not myGlobalDataTO.HasError) Then
        '                    If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
        '                Else
        '                    If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
        '                End If
        '            End If
        '        End If

        '    Catch ex As Exception
        '        'When the Database Connection was opened locally, then the Rollback is executed
        '        If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

        '        myGlobalDataTO.HasError = True
        '        myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myGlobalDataTO.ErrorMessage = ex.Message

        '        'Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "PreviousTestControlLotsDelegate.DeleteTestControlsByControlID", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return myGlobalDataTO
        'End Function


        ''' <summary>
        ''' Delete all Controls linked to the specified Standard Test and, optionally SampleType
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  DL 14/04/2011
        ''' Modified by: SA 10/05/2011 - New implementation, the previous one was wrong
        ''' </remarks>
        Public Function SavePreviousLotTests(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pControlID As Integer) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection
            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError) AndAlso (Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myPreviousTestControlLots As New tparPreviousTestControlLotsDAO
                        myGlobalDataTO = myPreviousTestControlLots.SavePreviousLotTests(dbConnection, pControlID)

                        If (Not myGlobalDataTO.HasError) Then
                            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                        Else
                            If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                        End If
                    End If
                End If
            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "PreviousTestControlLotsDelegate.SavePreviousLotTests", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Receive a Control Identifier and copy all data of the Tests/SampleTypes/TestType 
        ''' linked to thecurrent Lot, to the equivalent Previous Saved Lot structure
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pControlID">Control Identifier</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by: RH 15/06/2012 
        ''' </remarks>
        Public Function SavePreviousLotTestsNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pControlID As Integer) As GlobalDataTO

            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)

                If (Not myGlobalDataTO.HasError) AndAlso (Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then
                        Dim myPreviousTestControlLots As New tparPreviousTestControlLotsDAO
                        myGlobalDataTO = myPreviousTestControlLots.SavePreviousLotTestsNEW(dbConnection, pControlID)

                        If (Not myGlobalDataTO.HasError) Then
                            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                        Else
                            If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                        End If
                    End If
                End If

            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "PreviousTestControlLotsDelegate.SavePreviousLotTests", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()

            End Try

            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Delete the specified Test/SampleType linked to the previous saved Lot of the informed Control
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pControlID">Identifier of the Control to delete</param>        
        ''' <param name="pTestID">Test Identifier</param>
        ''' <param name="pSampleType">SampleType Code</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 10/05/2011
        ''' Modified by: RH 11/06/2012 - New parameter TestType.
        ''' </remarks>
        Public Function DeleteNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pControlID As Integer, _
                                  ByVal pTestID As Integer, ByVal pSampleType As String, _
                                  ByVal pTestType As String) As GlobalDataTO

            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)

                If (Not myGlobalDataTO.HasError) AndAlso (Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then
                        Dim myPreviousControlLotsDAO As New tparPreviousTestControlLotsDAO
                        myGlobalDataTO = myPreviousControlLotsDAO.DeleteNEW(dbConnection, pControlID, pTestID, pSampleType, pTestType)

                        If (Not myGlobalDataTO.HasError) Then
                            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                        Else
                            If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                        End If
                    End If
                End If

            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "PreviousTestControlLotsDelegate.Delete", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()

            End Try

            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Delete all information for a previous Control Lots for an specific Test, and optionally, a 
        ''' Sample Type and/or a list of Controls (the list will contain those Controls that have to remain linked
        ''' to the Test/SampleType)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestID">Test Identifier</param>
        ''' <param name="pSampleType">Sample Type; optional parameter</param>
        ''' <param name="pControlIDList">List of ControlIDs separated by (,). Optional parameter</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by:  TR 07/04/2011
        ''' Modified by: SA 11/05/2011 - Added optional parameter pControlIDList
        ''' Modified by: RH 11/06/2012 - New parameter TestType.
        ''' </remarks>
        Public Function DeleteByTestIDNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer, _
                                          ByVal pTestType As String, _
                                          Optional ByVal pSampleType As String = "", _
                                          Optional ByVal pControlIDList As String = "") As GlobalDataTO

            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)

                If (Not myGlobalDataTO.HasError) AndAlso (Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then
                        Dim myPreviousControlLotsDAO As New tparPreviousTestControlLotsDAO
                        myGlobalDataTO = myPreviousControlLotsDAO.DeleteByTestIDNEW(dbConnection, _
                                                pTestID, pTestType, pSampleType, pControlIDList)

                        If (Not myGlobalDataTO.HasError) Then
                            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                        Else
                            If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                        End If
                    End If
                End If

            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "PreviousTestControlLotsDelegate.DeleteByTestID", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()

            End Try

            Return myGlobalDataTO
        End Function

#End Region

    End Class

End Namespace


