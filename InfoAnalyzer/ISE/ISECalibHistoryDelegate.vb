Option Strict On
Option Explicit On

Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.DAL.DAO
Imports Biosystems.Ax00.DAL

Namespace Biosystems.Ax00.InfoAnalyzer

    Public Class ISECalibHistoryDelegate

#Region "Declarations"

#End Region

#Region "Attributes"
        'Private AnalyzerIDAttr As String = ""
#End Region

#Region "Properties"
        'Public Property AnalyzerID() As String
        '    Get
        '        Return AnalyzerIDAttr
        '    End Get
        '    Set(ByVal value As String)
        '        AnalyzerIDAttr = value
        '    End Set
        'End Property

#End Region

#Region "Constructor"
        Public Sub New()

        End Sub
#End Region

#Region "Public Methods"
        ''' <summary>
        ''' Gets all ISE Calibrations History data
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pAnalyzerID"></param>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 24/01/2012</remarks>
        Public Function ReadAll(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection
            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError) And (Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Get data fields from the specified Analyzer
                        Dim myISECalibHistoryDAO As New thisCalibISEDAO
                        resultData = myISECalibHistoryDAO.ReadAll(dbConnection, pAnalyzerID)
                    End If
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ISECalibHistoryDelegate.ReadAll", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Gets all calibrations performed in the ISE Module installed in the informed Analyzer by its corresponding type
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pAnalyzerID"></param>
        ''' <param name="pTypes"></param>
        ''' <param name="dateFrom"></param>
        ''' <param name="dateTo"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Modified by: JB 25/07/2012 - Changed function name
        '''                            - Added new parameters 
        ''' </remarks>
        Public Function ReadByConditioningTypes(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
                                                ByVal pTypes As GlobalEnumerates.ISEConditioningTypes(), _
                                                Optional ByVal dateFrom As DateTime = Nothing, Optional ByVal dateTo As DateTime = Nothing) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection
            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError) And (Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Get data fields from the specified Analyzer
                        Dim myISECalibHistoryDAO As New thisCalibISEDAO
                        resultData = myISECalibHistoryDAO.ReadByConditioningTypes(dbConnection, pAnalyzerID, pTypes, dateFrom, dateTo)
                    End If
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ISECalibHistoryDelegate.ReadByConditioningTypes", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pAnalyzerID"></param>
        ''' <param name="pISECalibrationID"></param>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 24/01/2012</remarks>
        Public Function ReadByCalibrationID(ByVal pDBConnection As SqlClient.SqlConnection, _
                                            ByVal pAnalyzerID As String, _
                                            ByVal pISECalibrationID As Integer) As GlobalDataTO

            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection
            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError) And (Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Get data fields from the specified Analyzer
                        Dim myISECalibHistoryDAO As New thisCalibISEDAO
                        resultData = myISECalibHistoryDAO.ReadByCalibrationID(dbConnection, pAnalyzerID, pISECalibrationID)
                    End If
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ISECalibHistoryDelegate.ReadByCalibartionID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pISECalibDS"></param>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 24/01/2012</remarks>
        Public Function AddNewCalibration(ByVal pDBConnection As SqlClient.SqlConnection, _
                                          ByVal pISECalibDS As HistoryISECalibrationsDS) As GlobalDataTO

            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError) And (Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myISECalibHistoryDAO As New thisCalibISEDAO
                        resultData = myISECalibHistoryDAO.Create(dbConnection, pISECalibDS)

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
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ISECalibHistoryDelegate.AddNewCalibration", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try

            Return resultData

        End Function

#Region " Add new Conditioning action "
        ''' <summary>
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pAnalyzerId"></param>
        ''' <param name="pConditioningType"></param>
        ''' <param name="pLiEnabled"></param>
        ''' <param name="pExecutionDate"></param>
        ''' <param name="pResults"></param>
        ''' <param name="pErrorsString"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by:  JB 30/07/2012
        ''' Modified by: JB 20/09/2012 - Added pLiEnabled parameter
        ''' </remarks>
        Protected Function AddConditioning(ByVal pDBConnection As SqlClient.SqlConnection, _
                                           ByVal pAnalyzerId As String, _
                                           ByVal pConditioningType As Biosystems.Ax00.Global.GlobalEnumerates.ISEConditioningTypes, _
                                           ByVal pLiEnabled As Boolean, _
                                           ByVal pExecutionDate As Date, _
                                           ByVal pResults As String, _
                                           ByVal pErrorsString As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Try
                Dim myISECalibHisDS As New HistoryISECalibrationsDS
                If pExecutionDate = Nothing Then pExecutionDate = Now
                If pResults Is Nothing Then pResults = ""
                If pErrorsString Is Nothing Then pErrorsString = ""

                myISECalibHisDS.Clear()
                Dim myISECalibHisRow As HistoryISECalibrationsDS.thisCalibISERow
                myISECalibHisRow = myISECalibHisDS.thisCalibISE.NewthisCalibISERow
                With myISECalibHisRow
                    .BeginEdit()
                    .AnalyzerID = pAnalyzerId
                    .ConditioningType = pConditioningType.ToString
                    .CalibrationDate = pExecutionDate
                    .ResultsString = pResults
                    .ErrorsString = pErrorsString
                    .LiEnabled = pLiEnabled
                    .EndEdit()
                End With
                myISECalibHisDS.thisCalibISE.AddthisCalibISERow(myISECalibHisRow)
                myISECalibHisDS.AcceptChanges()

                resultData = AddNewCalibration(pDBConnection, myISECalibHisDS)
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ISECalibHistoryDelegate.AddConditioning", EventLogEntryType.Error, False)
            End Try

            Return resultData
        End Function

        ''' <summary>
        ''' Function to insert a Electrode Calibation to ISE Historical table
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pAnalyzerID"></param>
        ''' <param name="pLiEnabled"></param>
        ''' <param name="pExecutionDate"></param>
        ''' <param name="pResults1"></param>
        ''' <param name="pResults2"></param>
        ''' <param name="pErrorsString"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by:  JB 30/07/2012
        ''' Modified by: JB 20/09/2012 - Added pLiEnabled parameter
        ''' </remarks>
        Public Function AddElectrodeCalibration(ByVal pDBConnection As SqlClient.SqlConnection, _
                                                ByVal pAnalyzerID As String, _
                                                ByVal pLiEnabled As Boolean, _
                                                ByVal pExecutionDate As Date, _
                                                ByVal pResults1 As String, _
                                                ByVal pResults2 As String, _
                                                Optional ByVal pErrorsString As String = "") As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Try
                'JB 21/09/2012 - Correction: Results Always between "<" and ">"
                If Not String.IsNullOrEmpty(pResults1) Then
                    If Not pResults1.StartsWith("<") Then pResults1 = "<" & pResults1
                    If Not pResults1.EndsWith(">") Then pResults1 &= ">"
                End If
                If Not String.IsNullOrEmpty(pResults2) Then
                    If Not pResults2.StartsWith("<") Then pResults2 = "<" & pResults2
                    If Not pResults2.EndsWith(">") Then pResults2 &= ">"
                End If
                resultData = AddConditioning(pDBConnection, pAnalyzerID, GlobalEnumerates.ISEConditioningTypes.CALB, _
                                             pLiEnabled, pExecutionDate, pResults1 & "#" & pResults2, pErrorsString)
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ISECalibHistoryDelegate.AddElectrodeCalibration", EventLogEntryType.Error, False)
            End Try

            Return resultData
        End Function

        ''' <summary>
        ''' Function to insert a Pump Calibation to ISE Historical table
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pAnalyzerID"></param>
        ''' <param name="pLiEnabled"></param>
        ''' <param name="pExecutionDate"></param>
        ''' <param name="pResults"></param>
        ''' <param name="pErrorsString"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by:  JB 30/07/2012
        ''' Modified by: JB 20/09/2012 - Added pLiEnabled parameter
        ''' </remarks>
        Public Function AddPumpCalibration(ByVal pDBConnection As SqlClient.SqlConnection, _
                                           ByVal pAnalyzerID As String, _
                                           ByVal pLiEnabled As Boolean, _
                                           ByVal pExecutionDate As Date, _
                                           ByVal pResults As String, _
                                           Optional ByVal pErrorsString As String = "") As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                resultData = AddConditioning(pDBConnection, pAnalyzerID, GlobalEnumerates.ISEConditioningTypes.PMCL, _
                                             pLiEnabled, pExecutionDate, pResults, pErrorsString)
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ISECalibHistoryDelegate.AddPumpCalibration", EventLogEntryType.Error, False)
            End Try

            Return resultData
        End Function

        ''' <summary>
        ''' Function to insert a Bubble Calibation to ISE Historical table
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pAnalyzerID"></param>
        ''' <param name="pLiEnabled"></param>
        ''' <param name="pExecutionDate"></param>
        ''' <param name="pResults"></param>
        ''' <param name="pErrorsString"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by:  JB 30/07/2012
        ''' Modified by: JB 20/09/2012 - Added pLiEnabled parameter
        ''' </remarks>
        Public Function AddBubbleCalibration(ByVal pDBConnection As SqlClient.SqlConnection, _
                                             ByVal pAnalyzerID As String, _
                                             ByVal pLiEnabled As Boolean, _
                                             ByVal pExecutionDate As Date, _
                                             ByVal pResults As String, _
                                             Optional ByVal pErrorsString As String = "") As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                resultData = AddConditioning(pDBConnection, pAnalyzerID, GlobalEnumerates.ISEConditioningTypes.BBCL, _
                                             pLiEnabled, pExecutionDate, pResults, pErrorsString)
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ISECalibHistoryDelegate.AddBubbleCalibration", EventLogEntryType.Error, False)
            End Try

            Return resultData
        End Function

        ''' <summary>
        ''' Function to insert a Cleaning to ISE Historical table
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pAnalyzerID"></param>
        ''' <param name="pLiEnabled"></param>
        ''' <param name="pExecutionDate"></param>
        ''' <param name="pErrorsString"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by:  JB 30/07/2012
        ''' Modified by: JB 20/09/2012 - Added pLiEnabled parameter
        ''' </remarks>
        Public Function AddCleaning(ByVal pDBConnection As SqlClient.SqlConnection, _
                                    ByVal pAnalyzerID As String, _
                                    ByVal pLiEnabled As Boolean, _
                                    ByVal pExecutionDate As Date, _
                                    Optional ByVal pErrorsString As String = "") As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                resultData = AddConditioning(pDBConnection, pAnalyzerID, GlobalEnumerates.ISEConditioningTypes.CLEN, _
                                             pLiEnabled, pExecutionDate, "", pErrorsString)
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ISECalibHistoryDelegate.AddCleaning", EventLogEntryType.Error, False)
            End Try

            Return resultData
        End Function
#End Region

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pISECalibrationID"></param>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 24/01/2012</remarks>
        Public Function DeleteCalibration(ByVal pDBConnection As SqlClient.SqlConnection, _
                                           ByVal pISECalibrationID As Integer) As GlobalDataTO

            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError) And (Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myISECalibHistoryDAO As New thisCalibISEDAO
                        resultData = myISECalibHistoryDAO.Delete(dbConnection, pISECalibrationID)

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
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ISECalibHistoryDelegate.DeleteCalibration", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try

            Return resultData

        End Function
#End Region
    End Class

End Namespace