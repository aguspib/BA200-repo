Option Strict On
Option Explicit On

Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.DAL


Namespace Biosystems.Ax00.BL
    Partial Public Class AutoReportsDelegate

#Region "Declarations"
        Private orderList As List(Of String) = New List(Of String)
#End Region

#Region "Public Methods"
        
        Public Function ManageAutoReportCreationExecutions(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, _
                                                     ByVal pOrderTestID As Integer, ByVal pCalledForOFFSystem As Boolean) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
            Dim StartTime As DateTime = Now
            'Dim myLogAcciones1 As New ApplicationLogManager()
            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***

            Try
                'Special code for this function: the template can not be used....DO NOT CHANGE!!
                Dim dbConnection As SqlClient.SqlConnection = Nothing
                If (pCalledForOFFSystem AndAlso Not pDBConnection Is Nothing) Then dbConnection = pDBConnection

                Dim IsAutomaticReport As Boolean = False
                Dim myReportFrecuency As String = String.Empty
                Dim myUserSettingDelegate As New UserSettingsDelegate()

                'Get the autoreport Type: Manual/Automatic
                myGlobalDataTO = myUserSettingDelegate.GetCurrentValueBySettingID(Nothing, GlobalEnumerates.UserSettingsEnum.AUT_RESULTS_PRINT.ToString)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    If (CType(myGlobalDataTO.SetDatos, Integer) = 1) Then
                        IsAutomaticReport = True
                        'Auto-Report Type is Automatic, then get the programmed Export Frequency
                        myGlobalDataTO = myUserSettingDelegate.GetCurrentValueBySettingID(Nothing, GlobalEnumerates.UserSettingsEnum.AUT_RESULTS_FREQ.ToString)
                        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                            myReportFrecuency = myGlobalDataTO.SetDatos.ToString()
                        End If
                    End If
                End If

                Dim myStatus As String = ""
                Dim myOrdersDelegate As New OrdersDelegate
                orderList.Clear()
                If (Not myGlobalDataTO.HasError AndAlso IsAutomaticReport AndAlso myReportFrecuency <> String.Empty) Then
                    Select Case (myReportFrecuency)
                        '(1) REPORT WHEN THE WORKSESSION HAS FINISHED
                        Case "END_WS"
                            'Get the Status of the WS
                            Dim myWSAnalyzersDelegate As New WSAnalyzersDelegate()
                            myGlobalDataTO = myWSAnalyzersDelegate.ReadWSAnalyzers(dbConnection, pAnalyzerID, pWorkSessionID)

                            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                Dim myWSAnalyzersDS As WSAnalyzersDS = DirectCast(myGlobalDataTO.SetDatos, WSAnalyzersDS)

                                If (myWSAnalyzersDS.twksWSAnalyzers.Rows.Count > 0) Then
                                    myStatus = myWSAnalyzersDS.twksWSAnalyzers(0).WSStatus
                                    If (String.Equals(myStatus, "CLOSED")) Then
                                        'Execute the Export process
                                        myGlobalDataTO = myOrdersDelegate.ReadWSOrdersAllTypes(dbConnection, myWSAnalyzersDS.twksWSAnalyzers(0).AnalyzerID, myWSAnalyzersDS.twksWSAnalyzers(0).WorkSessionID, "PATIENT", "CLOSED")
                                        'Passem les dades obtingudes al orderList
                                        If Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing Then
                                            Dim myOrderDS As OrdersDS = DirectCast(myGlobalDataTO.SetDatos, OrdersDS)
                                            If myOrderDS.twksOrders.Rows.Count > 0 Then
                                                orderList = Enumerable.Cast(Of String)(From l In myOrderDS.twksOrders.AsEnumerable() Select l("OrderID")).Distinct().ToList()
                                                myGlobalDataTO.SetDatos = orderList
                                            Else
                                                myGlobalDataTO.SetDatos = Nothing
                                            End If
                                        Else
                                            myGlobalDataTO.SetDatos = Nothing
                                        End If
                                    Else
                                        myGlobalDataTO.SetDatos = Nothing
                                    End If
                                End If
                            End If
                            '(2) REPORT WHEN ALL TESTS REQUESTED FOR THE PATIENT HAVE FINISHED
                        Case "ORDER"
                            'Get the Status of the Patient ORDER
                            myGlobalDataTO = myOrdersDelegate.ReadByOrderTestID(dbConnection, pOrderTestID)
                            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                Dim myOrdersDS As OrdersDS = DirectCast(myGlobalDataTO.SetDatos, OrdersDS)

                                If (myOrdersDS.twksOrders.Rows.Count > 0) Then
                                    myStatus = myOrdersDS.twksOrders(0).OrderStatus
                                    If (String.Equals(myStatus, "CLOSED") AndAlso myOrdersDS.twksOrders(0).SampleClass = "PATIENT") Then
                                        'Execute the Export process
                                        orderList.Add(myOrdersDS.twksOrders(0).OrderID)
                                        myGlobalDataTO.SetDatos = orderList
                                    Else
                                        myGlobalDataTO.SetDatos = Nothing
                                    End If
                                End If
                            End If
                        Case Else
                            myGlobalDataTO.SetDatos = Nothing
                    End Select
                Else
                    myGlobalDataTO.SetDatos = Nothing
                End If

                '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
                GlobalBase.CreateLogActivity("ManageAutoReportCreationExecutions (Complete): " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), _
                                                 "AutoReportsDelegate.ManageAutoReportCreationExecutions", EventLogEntryType.Information, False)
                '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "AutoReportsDelegate.ManageAutoReportCreationExecutions", EventLogEntryType.Error, False)
            End Try

            Return myGlobalDataTO

        End Function

        Public Function ManageAutoReportCreationOFFSys(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, _
                                                     ByRef pReportType As String) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
            Dim StartTime As DateTime = Now
            'Dim myLogAcciones1 As New ApplicationLogManager()
            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***

            Try
                'Special code for this function: the template can not be used....DO NOT CHANGE!!
                Dim dbConnection As SqlClient.SqlConnection = Nothing

                Dim IsAutoReport As Boolean = False
                Dim sFrequency As String = String.Empty, sType As String = String.Empty, myStatus As String = String.Empty

                GetAutoReportValues(dbConnection, IsAutoReport, sFrequency, sType)
                pReportType = sType
                If IsAutoReport Then
                    'En función de la frecuencia y tipo de report, lanzaremos las funciones adecuadas
                    Dim myOrdersDelegate As New OrdersDelegate
                    Select Case (sFrequency)
                        Case "END_WS"
                            Dim myWSAnalyzersDelegate As New WSAnalyzersDelegate()
                            myGlobalDataTO = myWSAnalyzersDelegate.ReadWSAnalyzers(Nothing, pAnalyzerID, pWorkSessionID)

                            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                Dim myWSAnalyzersDS As WSAnalyzersDS = DirectCast(myGlobalDataTO.SetDatos, WSAnalyzersDS)

                                If (myWSAnalyzersDS.twksWSAnalyzers.Rows.Count > 0) Then
                                    myStatus = myWSAnalyzersDS.twksWSAnalyzers(0).WSStatus
                                    If (String.Equals(myStatus, "CLOSED")) Then
                                        'Execute the Export process
                                        myGlobalDataTO = myOrdersDelegate.ReadWSOrdersAllTypes(Nothing, myWSAnalyzersDS.twksWSAnalyzers(0).AnalyzerID, myWSAnalyzersDS.twksWSAnalyzers(0).WorkSessionID, "PATIENT", "CLOSED")
                                        'Passem les dades obtingudes al orderList
                                        If Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing Then
                                            Dim myOrderDS As OrdersDS = DirectCast(myGlobalDataTO.SetDatos, OrdersDS)
                                            If myOrderDS.twksOrders.Rows.Count > 0 Then
                                                myGlobalDataTO.SetDatos = Enumerable.Cast(Of String)(From l In myOrderDS.twksOrders.AsEnumerable() Select l("OrderID")).Distinct().ToList()
                                            Else
                                                myGlobalDataTO.SetDatos = Nothing
                                            End If
                                        Else
                                            myGlobalDataTO.SetDatos = Nothing
                                        End If
                                    Else
                                        myGlobalDataTO.SetDatos = Nothing
                                    End If
                                End If
                            End If

                        Case "ORDER"
                            'Get the Status of the Patient ORDER
                            myGlobalDataTO = myOrdersDelegate.GetOrdersByTestType(dbConnection, "OFFS", "CLOSED")
                            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                Dim myOrdersDS As OrdersDS = DirectCast(myGlobalDataTO.SetDatos, OrdersDS)
                                If (myOrdersDS.twksOrders.Rows.Count > 0) Then
                                    myGlobalDataTO.SetDatos = Enumerable.Cast(Of String)(From l In myOrdersDS.twksOrders.AsEnumerable() Select l("OrderID")).Distinct().ToList()
                                Else
                                    myGlobalDataTO.SetDatos = Nothing
                                End If
                            End If
                        Case Else
                            'Opción Reset: el informe se lanzará al pulsar el botón de Reset. No hacemos nada aquí.
                    End Select
                End If

                '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
                GlobalBase.CreateLogActivity("ManageAutoRepoManageAutoReportCreationOFFSys (Complete): " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), _
                                                 "AutoReportsDelegate.ManageAutoReportCreationOFFSys", EventLogEntryType.Information, False)
                '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "AutoReportsDelegate.ManageAutoReportCreationOFFSys", EventLogEntryType.Error, False)
            End Try

            Return myGlobalDataTO

        End Function


        Public Function GetAutoReportValues(ByVal pDBConnection As SqlClient.SqlConnection, ByRef pbAutoReport As Boolean, ByRef psFrequency As String, ByRef psType As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)

                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then

                        Dim myUserSettingDelegate As New UserSettingsDelegate()

                        resultData = myUserSettingDelegate.GetCurrentValueBySettingID(Nothing, GlobalEnumerates.UserSettingsEnum.AUT_RESULTS_PRINT.ToString)
                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            If (CType(resultData.SetDatos, Integer) = 1) Then
                                pbAutoReport = True
                                resultData = myUserSettingDelegate.GetCurrentValueBySettingID(Nothing, GlobalEnumerates.UserSettingsEnum.AUT_RESULTS_FREQ.ToString)
                                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                    psFrequency = resultData.SetDatos.ToString()
                                End If
                                resultData = myUserSettingDelegate.GetCurrentValueBySettingID(Nothing, GlobalEnumerates.UserSettingsEnum.AUT_RESULTS_TYPE.ToString)
                                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                    psType = resultData.SetDatos.ToString()
                                End If
                            End If
                        End If


                    End If
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "AutoReportsDelegate.GetAutoReportValues", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()

            End Try

            Return resultData
        End Function


#End Region

#Region "Private Methods"
       
#End Region


        Protected Overrides Sub Finalize()
            MyBase.Finalize()
        End Sub

    End Class
End Namespace
