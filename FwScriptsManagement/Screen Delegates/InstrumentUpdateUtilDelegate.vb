Option Strict On
Option Explicit On

Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Global.GlobalEnumerates
Imports Biosystems.Ax00.DAL.DAO
Imports Biosystems.Ax00.CommunicationsSwFw
Imports Biosystems.Ax00.BL

Imports System.IO

Namespace Biosystems.Ax00.FwScriptsManagement

    Public Class InstrumentUpdateUtilDelegate
        Inherits BaseFwScriptDelegate

#Region "Common"

#Region "Enumerates"
        Public Enum HISTORY_AREAS
            NONE = 0
            BACKUP_ADJ = 1
            RESTORE_ADJ = 2
            RESTORE_FACTORY_ADJ = 3
            UPDATE_FW_OPEN = 11
            UPDATE_FW_START = 12
            UPDATE_FW_SEND = 13
            UPDATE_FW_QUERY = 14
            UPDATE_FW_CPU = 15
            UPDATE_FW_PER = 16
            UPDATE_FW_MAN = 17
            UPDATE_FW_ADJ = 18

        End Enum

        Public Enum HISTORY_RESULTS
            _ERROR = -1
            NOT_DONE = 0
            OK = 1
            NOK = 2
            CANCEL = 3
        End Enum

        Public Enum HISTORY_NOK_REASONS
            NONE = 0
            BACKUP_FILE_ERROR = 1
            RESTORE_ADJFILE_ERROR = 2
            UPDATE_FWFILE_ERROR = 3
            UPDATE_FWFILE_WRONG_VERSION = 4
        End Enum
#End Region

#Region "Declarations"
        'history
        Private RecommendationsList As New List(Of HISTORY_RECOMMENDATIONS)
        Private ReportCountTimeout As Integer
#End Region

#Region "Constructor"
        Public Sub New(ByVal pAnalyzerID As String, ByVal pFwScriptsDelegate As SendFwScriptsDelegate)
            MyBase.New(pAnalyzerID) 'SGM 20/01/2012
            myFwScriptDelegate = pFwScriptsDelegate
        End Sub

        Public Sub New()
            MyBase.New()
        End Sub
#End Region

#Region "Attributes"
        Private AnalyzerIdAttr As String = ""

        'HISTORY
        '***************************
        Private HistoryTaskAttr As PreloadedMasterDataEnum = Nothing
        Private HistoryAreaAttr As HISTORY_AREAS = HISTORY_AREAS.NONE
        Private HistoryResultAttr As HISTORY_RESULTS = HISTORY_RESULTS.NOT_DONE
        Private HistoryNOKReasonAttr As HISTORY_NOK_REASONS = HISTORY_NOK_REASONS.NONE

#End Region

#Region "Properties"

        Public Property AnalyzerId() As String
            Get
                Return AnalyzerIdAttr
            End Get
            Set(ByVal value As String)
                AnalyzerIdAttr = value
            End Set
        End Property

        '**************HISTORY********************ç

        Public Property HistoryTask() As PreloadedMasterDataEnum
            Get
                Return HistoryTaskAttr
            End Get
            Set(ByVal value As PreloadedMasterDataEnum)
                HistoryTaskAttr = value
            End Set
        End Property

        Public Property HistoryArea() As HISTORY_AREAS
            Get
                Return HistoryAreaAttr
            End Get
            Set(ByVal value As HISTORY_AREAS)
                HistoryAreaAttr = value
            End Set
        End Property

        Public Property HistoryResult() As HISTORY_RESULTS
            Get
                Return HistoryResultAttr
            End Get
            Set(ByVal value As HISTORY_RESULTS)
                HistoryResultAttr = value
            End Set
        End Property

        Public Property HistoryNOKReason() As HISTORY_NOK_REASONS
            Get
                Return HistoryNOKReasonAttr
            End Get
            Set(ByVal value As HISTORY_NOK_REASONS)
                HistoryNOKReasonAttr = value
            End Set
        End Property



#End Region

#Region "Public Methods"

        ''' <summary>
        ''' Reset Analyzer Adjustments High Level Instruction
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 30/06/2011</remarks>
        Public Function SendRESET_ANALYZER() As GlobalDataTO

            Dim myGlobal As New GlobalDataTO

            Try

                myGlobal = myFwScriptDelegate.AnalyzerManager.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.RESET_ANALYZER, True, Nothing, Nothing)


            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "InstrumentUpdateUtilDelegate.SendRESET_ANALYZER", EventLogEntryType.Error, False)
            End Try

            Return myGlobal

        End Function

#End Region


#Region "History Data Report"

#Region "Public"



        ''' <summary>
        ''' Saves the History data to DB
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 01/08/2011</remarks>
        Public Function ManageHistoryResults() As GlobalDataTO

            Dim myGlobal As New GlobalDataTO
            Dim myHistoryDelegate As New HistoricalReportsDelegate

            Try

                If MyClass.HistoryArea <> Nothing Then

                    Dim myHistoricReportDS As New SRVResultsServiceDS
                    Dim myHistoricReportRow As SRVResultsServiceDS.srv_thrsResultsServiceRow

                    Dim myTask As String = ""
                    Dim myAction As String = ""

                    Select Case MyClass.HistoryTask
                        Case PreloadedMasterDataEnum.SRV_ACT_ADJ_TYPES : myTask = "ADJUST"
                        Case PreloadedMasterDataEnum.SRV_ACT_TEST_TYPES : myTask = "TEST"
                        Case PreloadedMasterDataEnum.SRV_ACT_UTIL_TYPES : myTask = "UTIL"
                    End Select

                    Select Case MyClass.HistoryArea
                        Case HISTORY_AREAS.BACKUP_ADJ : myAction = "ADJ_BK"
                        Case HISTORY_AREAS.RESTORE_ADJ : myAction = "ADJ_RES"
                        Case HISTORY_AREAS.RESTORE_FACTORY_ADJ : myAction = "ADJ_FAC"
                        Case HISTORY_AREAS.UPDATE_FW_OPEN : myAction = "FW_OPEN"
                        Case HISTORY_AREAS.UPDATE_FW_START : myAction = "FW_START"
                        Case HISTORY_AREAS.UPDATE_FW_SEND : myAction = "FW_SEND"
                        Case HISTORY_AREAS.UPDATE_FW_QUERY : myAction = "FW_QUERY"
                        Case HISTORY_AREAS.UPDATE_FW_CPU : myAction = "FW_CPU"
                        Case HISTORY_AREAS.UPDATE_FW_PER : myAction = "FW_PER"
                        Case HISTORY_AREAS.UPDATE_FW_MAN : myAction = "FW_MAN"
                        Case HISTORY_AREAS.UPDATE_FW_ADJ : myAction = "FW_ADJ"

                    End Select

                    'Fill the data
                    myHistoricReportRow = myHistoricReportDS.srv_thrsResultsService.Newsrv_thrsResultsServiceRow
                    With myHistoricReportRow
                        .BeginEdit()
                        .TaskID = myTask
                        .ActionID = myAction
                        .Data = MyClass.GenerateResultData()
                        .AnalyzerID = MyClass.AnalyzerId
                        .EndEdit()
                    End With

                    'save to history
                    myGlobal = myHistoryDelegate.Add(Nothing, myHistoricReportRow)


                    If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
                        'Get the generated ID from the dataset returned 
                        Dim generatedID As Integer = -1
                        generatedID = DirectCast(myGlobal.SetDatos, SRVResultsServiceDS.srv_thrsResultsServiceRow).ResultServiceID

                        'PDT!!
                        ' pending implementation for add possibles Recommendations usings in case of INCIDENCE!!!
                        If generatedID >= 0 Then
                            ' Insert recommendations if existing
                            If MyClass.RecommendationsList IsNot Nothing Then
                                Dim myRecommendationsList As New SRVRecommendationsServiceDS
                                Dim myRecommendationsRow As SRVRecommendationsServiceDS.srv_thrsRecommendationsServiceRow

                                For Each R As HISTORY_RECOMMENDATIONS In MyClass.RecommendationsList
                                    myRecommendationsRow = myRecommendationsList.srv_thrsRecommendationsService.Newsrv_thrsRecommendationsServiceRow
                                    myRecommendationsRow.ResultServiceID = generatedID
                                    myRecommendationsRow.RecommendationID = CInt(R)
                                    myRecommendationsList.srv_thrsRecommendationsService.Rows.Add(myRecommendationsRow)
                                Next

                                myGlobal = myHistoryDelegate.AddRecommendations(Nothing, myRecommendationsList)
                                If myGlobal.HasError Then
                                    myGlobal.HasError = True
                                End If
                            End If
                        End If

                    End If

                Else
                    myGlobal.HasError = True
                    myGlobal.ErrorCode = GlobalEnumerates.Messages.MASTER_DATA_MISSING.ToString
                End If



            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobal.ErrorMessage = ex.Message
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "InstrumentUpdateUtilDelegate.ManageHistoryResults", EventLogEntryType.Error, False)
            End Try

            Return myGlobal

        End Function


        ''' <summary>
        ''' Decodes the data from DB in order to display in History Screen
        ''' </summary>
        ''' <param name="pTask"></param>
        ''' <param name="pAction"></param>
        ''' <param name="pData"></param>
        ''' <param name="pLanguageId"></param>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 01/08/2011</remarks>
        Public Function DecodeDataReport(ByVal pTask As String, ByVal pAction As String, ByVal pData As String, ByVal pLanguageId As String) As GlobalDataTO

            Dim myGlobal As New GlobalDataTO
            Dim MLRD As New MultilanguageResourcesDelegate
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try

                Dim myUtility As New Utilities()
                Dim text1 As String = ""
                Dim text2 As String = ""
                Dim text3 As String = ""

                Dim myLines As List(Of List(Of String))
                Dim FinalText As String = ""

                Dim myTask As PreloadedMasterDataEnum
                Dim myArea As HISTORY_AREAS


                'set the task type
                Select Case pTask
                    Case "ADJUST" : myTask = PreloadedMasterDataEnum.SRV_ACT_ADJ_TYPES
                    Case "TEST" : myTask = PreloadedMasterDataEnum.SRV_ACT_TEST_TYPES
                    Case "UTIL" : myTask = PreloadedMasterDataEnum.SRV_ACT_UTIL_TYPES
                End Select

                'set the area
                Select Case pAction
                    Case "ADJ_BK" : myArea = HISTORY_AREAS.BACKUP_ADJ
                    Case "ADJ_RES" : myArea = HISTORY_AREAS.RESTORE_ADJ
                    Case "ADJ_FAC" : myArea = HISTORY_AREAS.RESTORE_FACTORY_ADJ
                    Case "FW_OPEN" : myArea = HISTORY_AREAS.UPDATE_FW_OPEN
                    Case "FW_START" : myArea = HISTORY_AREAS.UPDATE_FW_START
                    Case "FW_SEND" : myArea = HISTORY_AREAS.UPDATE_FW_SEND
                    Case "FW_QUERY" : myArea = HISTORY_AREAS.UPDATE_FW_QUERY
                    Case "FW_CPU" : myArea = HISTORY_AREAS.UPDATE_FW_CPU
                    Case "FW_PER" : myArea = HISTORY_AREAS.UPDATE_FW_PER
                    Case "FW_MAN" : myArea = HISTORY_AREAS.UPDATE_FW_MAN
                    Case "FW_ADJ" : myArea = HISTORY_AREAS.UPDATE_FW_ADJ

                End Select


                'obtain the connection
                myGlobal = DAOBase.GetOpenDBConnection(Nothing)
                If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobal.SetDatos, SqlClient.SqlConnection)
                End If

                Select Case myTask
                    Case PreloadedMasterDataEnum.SRV_ACT_UTIL_TYPES

                        myLines = New List(Of List(Of String))

                        Dim myLine As List(Of String)
                        Dim myColWidth As Integer = 35
                        Dim myColSep As Integer = 3

                        Dim myResult As HISTORY_RESULTS = MyClass.DecodeHistoryDataResult(pData)

                        myLine = New List(Of String)

                        Select Case myArea
                            Case HISTORY_AREAS.BACKUP_ADJ

                                '1st LINE
                                '*****************************************************************

                                text1 = (MLRD.GetResourceText(dbConnection, "LBL_SRV_ADJ_BK", pLanguageId) + ":")
                                myLine.Add(text1)

                                text2 = MyClass.GetResultLanguageResource(dbConnection, myResult, pLanguageId)
                                myLine.Add(text2)



                            Case HISTORY_AREAS.RESTORE_ADJ

                                '1st LINE
                                '*****************************************************************

                                text1 = (MLRD.GetResourceText(dbConnection, "LBL_SRV_ ADJ_RES", pLanguageId) + ":")
                                myLine.Add(text1)

                                text2 = MyClass.GetResultLanguageResource(dbConnection, myResult, pLanguageId)
                                myLine.Add(text2)


                            Case HISTORY_AREAS.RESTORE_FACTORY_ADJ

                                '1st LINE
                                '*****************************************************************

                                text1 = (MLRD.GetResourceText(dbConnection, "LBL_SRV_ ADJ_FAC", pLanguageId) + ":")
                                myLine.Add(text1)

                                text2 = MyClass.GetResultLanguageResource(dbConnection, myResult, pLanguageId)
                                myLine.Add(text2)


                            Case HISTORY_AREAS.UPDATE_FW_OPEN

                                '1st LINE
                                '*****************************************************************

                                text1 = (MLRD.GetResourceText(dbConnection, "LBL_SRV_FW_UPDATE_OPEN", pLanguageId) + ":")
                                myLine.Add(text1)

                                text2 = MyClass.GetResultLanguageResource(dbConnection, myResult, pLanguageId)
                                myLine.Add(text2)

                            Case HISTORY_AREAS.UPDATE_FW_START

                                '1st LINE
                                '*****************************************************************

                                text1 = (MLRD.GetResourceText(dbConnection, "LBL_SRV_FW_UPDATE_START", pLanguageId) + ":")
                                myLine.Add(text1)

                                text2 = MyClass.GetResultLanguageResource(dbConnection, myResult, pLanguageId)
                                myLine.Add(text2)

                            Case HISTORY_AREAS.UPDATE_FW_SEND

                                '1st LINE
                                '*****************************************************************

                                text1 = (MLRD.GetResourceText(dbConnection, "LBL_SRV_FW_UPDATE_SEND", pLanguageId) + ":")
                                myLine.Add(text1)

                                text2 = MyClass.GetResultLanguageResource(dbConnection, myResult, pLanguageId)
                                myLine.Add(text2)

                            Case HISTORY_AREAS.UPDATE_FW_QUERY

                                '1st LINE
                                '*****************************************************************

                                text1 = (MLRD.GetResourceText(dbConnection, "LBL_SRV_FW_UPDATE_QUERY", pLanguageId) + ":")
                                myLine.Add(text1)

                                text2 = MyClass.GetResultLanguageResource(dbConnection, myResult, pLanguageId)
                                myLine.Add(text2)

                            Case HISTORY_AREAS.UPDATE_FW_CPU

                                '1st LINE
                                '*****************************************************************

                                text1 = (MLRD.GetResourceText(dbConnection, "LBL_SRV_FW_UPDATE_CPU", pLanguageId) + ":")
                                myLine.Add(text1)

                                text2 = MyClass.GetResultLanguageResource(dbConnection, myResult, pLanguageId)
                                myLine.Add(text2)

                            Case HISTORY_AREAS.UPDATE_FW_PER

                                '1st LINE
                                '*****************************************************************

                                text1 = (MLRD.GetResourceText(dbConnection, "LBL_SRV_FW_UPDATE_PER", pLanguageId) + ":")
                                myLine.Add(text1)

                                text2 = MyClass.GetResultLanguageResource(dbConnection, myResult, pLanguageId)
                                myLine.Add(text2)

                            Case HISTORY_AREAS.UPDATE_FW_MAN

                                '1st LINE
                                '*****************************************************************

                                text1 = (MLRD.GetResourceText(dbConnection, "LBL_SRV_FW_UPDATE_MAN", pLanguageId) + ":")
                                myLine.Add(text1)

                                text2 = MyClass.GetResultLanguageResource(dbConnection, myResult, pLanguageId)
                                myLine.Add(text2)

                            Case HISTORY_AREAS.UPDATE_FW_ADJ

                                '1st LINE
                                '*****************************************************************

                                text1 = (MLRD.GetResourceText(dbConnection, "LBL_SRV_FW_UPDATE_ADJ", pLanguageId) + ":")
                                myLine.Add(text1)

                                text2 = MyClass.GetResultLanguageResource(dbConnection, myResult, pLanguageId)
                                myLine.Add(text2)



                        End Select

                        If myResult = HISTORY_RESULTS.NOK Then

                            text3 = MyClass.DecodeHistoryNOKReasons(pData, dbConnection, pLanguageId)
                            myLine.Add(text3)



                        End If

                        myLines.Add(myLine)

                        For Each Line As List(Of String) In myLines
                            FinalText &= myUtility.FormatLineHistorics(Line, myColWidth, False)
                        Next

                        myGlobal.SetDatos = FinalText


                End Select


            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "InstrumentUpdateUtilDelegate.DecodeDataReport", EventLogEntryType.Error, False)

            Finally
                If Not dbConnection IsNot Nothing Then dbConnection.Close()
            End Try

            Return myGlobal

        End Function

#End Region

#Region "Private"

        ''' <summary>
        ''' Reports the History data to DB. Encodes the result data to history
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 01/08/2011</remarks>
        Private Function GenerateResultData() As String

            Dim myData As String = ""

            Try
                myData = MyClass.EncodeHistoryResult(MyClass.HistoryResult)
                myData &= CInt(MyClass.HistoryNOKReason).ToString



            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "InstrumentUpdateUtilDelegate.GenerateResultData", EventLogEntryType.Error, False)
            End Try

            Return myData

        End Function

        ''' <summary>
        ''' Encodes History Result
        ''' </summary>
        ''' <param name="pValue"></param>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 01/08/2011</remarks>
        Private Function EncodeHistoryResult(ByVal pValue As Integer) As String

            Dim res As String = ""

            Try
                If pValue >= 0 Then
                    res = pValue.ToString
                Else
                    res = "x"
                End If

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "InstrumentUpdateUtilDelegate.EncodeHistoryResult", EventLogEntryType.Error, False)
            End Try

            Return res

        End Function


        ''' <summary>
        ''' Decodes History Result
        ''' </summary>
        ''' <param name="pData"></param>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 01/08/2011</remarks>
        Private Function DecodeHistoryDataResult(ByVal pData As String) As HISTORY_RESULTS

            Dim myResult As HISTORY_RESULTS

            Try
                Dim myIndex As Integer = 0

                Dim myResultNumber As String = ""


                myResultNumber = pData.Substring(myIndex, 1)

                If IsNumeric(myResultNumber) Then
                    myResult = CType(CInt(myResultNumber), HISTORY_RESULTS)
                ElseIf myResultNumber = "x" Then
                    myResult = CType(-1, HISTORY_RESULTS)
                End If

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "InstrumentUpdateUtilDelegate.DecodeHistoryDataResult", EventLogEntryType.Error, False)
            End Try

            Return myResult

        End Function


        ''' <summary>
        ''' Decodes History NOK reason
        ''' </summary>
        ''' <param name="pData"></param>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 08/09/2011</remarks>
        Private Function DecodeHistoryNOKReasons(ByVal pData As String, _
                                                 ByVal pdbConnection As SqlClient.SqlConnection, _
                                                 ByVal pLanguageId As String) As String

            Dim myReasonText As String = ""
            Dim myReason As HISTORY_NOK_REASONS = HISTORY_NOK_REASONS.NONE
            Dim MLRD As New MultilanguageResourcesDelegate

            Try
                Dim myIndex As Integer = 1
                Dim myResultNumber As String = ""

                myResultNumber = pData.Substring(myIndex, 1)

                If IsNumeric(myResultNumber) Then
                    myReason = CType(CInt(myResultNumber), HISTORY_NOK_REASONS)

                    Select Case myReason
                        Case HISTORY_NOK_REASONS.BACKUP_FILE_ERROR
                            myReasonText = MLRD.GetResourceText(pdbConnection, "LBL_SRV_BKP_FILE_ERROR", pLanguageId)

                        Case HISTORY_NOK_REASONS.RESTORE_ADJFILE_ERROR
                            myReasonText = MLRD.GetResourceText(pdbConnection, "LBL_SRV_RES_FILE_ERROR", pLanguageId)

                        Case HISTORY_NOK_REASONS.UPDATE_FWFILE_ERROR
                            myReasonText = MLRD.GetResourceText(pdbConnection, "LBL_SRV_FWFILE_ERROR", pLanguageId)

                    End Select
                End If

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "InstrumentUpdateUtilDelegate.DecodeHistoryNOKReasons", EventLogEntryType.Error, False)
            End Try

            Return myReasonText

        End Function


        ''' <summary>
        ''' Gets Result Text
        ''' </summary>
        ''' <param name="pdbConnection"></param>
        ''' <param name="pResult"></param>
        ''' <param name="pLanguageId"></param>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 01/08/2011</remarks>
        Private Function GetResultLanguageResource(ByRef pdbConnection As SqlClient.SqlConnection, ByVal pResult As HISTORY_RESULTS, ByVal pLanguageId As String) As String

            Dim res As String = ""
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            Try
                Select Case pResult
                    Case HISTORY_RESULTS.OK
                        res = "Ok"

                    Case HISTORY_RESULTS.NOK, HISTORY_RESULTS.NOT_DONE
                        res = "No OK"

                    Case HISTORY_RESULTS.CANCEL
                        res = myMultiLangResourcesDelegate.GetResourceText(pdbConnection, "LBL_SRV_CANCELLED", pLanguageId)

                    Case HISTORY_RESULTS._ERROR
                        res = myMultiLangResourcesDelegate.GetResourceText(pdbConnection, "LBL_SRV_FAILED", pLanguageId)

                End Select
            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "InstrumentUpdateUtilDelegate.GetResultLanguageResource", EventLogEntryType.Error, False)
            End Try

            Return res

        End Function

        ''' <summary>
        ''' registering the incidence in historical reports activity
        ''' </summary>
        ''' <remarks>Created by SGM 04/08/2011</remarks>
        Private Sub UpdateRecommendationsList(ByVal pRecommendationID As HISTORY_RECOMMENDATIONS)
            Try
                ' registering the incidence in historical reports activity

                'If MyClass.RecommendationsReport Is Nothing Then
                '    ReDim MyClass.RecommendationsReport(0)
                'Else
                '    ReDim Preserve MyClass.RecommendationsReport(UBound(MyClass.RecommendationsReport) + 1)
                'End If
                'MyClass.RecommendationsReport(UBound(MyClass.RecommendationsReport)) = pRecommendationID

                If MyClass.RecommendationsList Is Nothing Then
                    MyClass.RecommendationsList = New List(Of HISTORY_RECOMMENDATIONS)
                End If
                MyClass.RecommendationsList.Add(pRecommendationID)

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "InstrumentUpdateUtilDelegate.UpdateRecommendations", EventLogEntryType.Error, False)
            End Try
        End Sub

#End Region


#End Region


#Region "Event Handlers"
        ''' <summary>
        ''' manages the responses of the Analyzer
        ''' The response can be OK, NG, Timeout or Exception
        ''' </summary>
        ''' <param name="pResponse">response type</param>
        ''' <param name="pData">data received</param>
        ''' <remarks>Created by XBC 18/05/11</remarks>
        Private Sub ScreenReceptionLastFwScriptEvent(ByVal pResponse As RESPONSE_TYPES, ByVal pData As Object) Handles Me.ReceivedLastFwScriptEvent
            Dim myGlobal As New GlobalDataTO
            Try
                'manage special operations according to the screen characteristics

                If pResponse = RESPONSE_TYPES.TIMEOUT Or _
                   pResponse = RESPONSE_TYPES.EXCEPTION Then

                    If MyClass.ReportCountTimeout = 0 Then
                        MyClass.ReportCountTimeout += 1
                        ' registering the incidence in historical reports activity
                        MyClass.UpdateRecommendationsList(HISTORY_RECOMMENDATIONS.ERR_COMM)
                    End If

                    Exit Sub

                End If

                'If FWUpdateCurrentAction <> FwUpdateActions.None Then
                '    Select Case pResponse
                '        Case RESPONSE_TYPES.START

                '        Case RESPONSE_TYPES.OK

                '    End Select
                'End If


            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "InstrumentUpdateUtilDelegate.ScreenReceptionLastFwScriptEvent", EventLogEntryType.Error, False)
            End Try
        End Sub
#End Region

#End Region

#Region "Adjustments"

#Region "Attributes"
        Public RestoringAdjustmentsTextAttr As String = ""

#End Region


#Region "Public Methods"

        ''' <summary>
        ''' Restore Adjustments High Level Instruction to restore adjustments into the instrument 
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 30/06/2011</remarks>
        Public Function SendRESTORE_ADJUSTMENTS() As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Try
                'MyClass.RestoringAdjustmentsTextAttr = "ISEINS:1;" 'QUITAR
                If MyClass.RestoringAdjustmentsTextAttr.Length > 0 Then
                    myGlobal = myFwScriptDelegate.AnalyzerManager.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.LOADADJ, _
                                                                                     True, Nothing, _
                                                                                     MyClass.RestoringAdjustmentsTextAttr)
                End If

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "InstrumentUpdateUtilDelegate.SendRESTORE_ADJUSTMENTS", EventLogEntryType.Error, False)
            End Try
            MyClass.RestoringAdjustmentsTextAttr = ""
            Return myGlobal
        End Function

        ''' <summary>
        ''' Restore Adjustments High Level Instruction to restore default adjustments into the instrument 
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 30/06/2011</remarks>
        Public Function SendRESTORE_FACTORY_ADJUSTMENTS() As GlobalDataTO

            Dim myGlobal As New GlobalDataTO

            Try

                myGlobal = myFwScriptDelegate.AnalyzerManager.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.LOADFACTORYADJ, True, Nothing, Nothing)


            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "InstrumentUpdateUtilDelegate.SendRESTORE_FACTORY_ADJUSTMENTS", EventLogEntryType.Error, False)
            End Try

            Return myGlobal

        End Function






#End Region

#End Region

#Region "Firmware"

#Region "Constants"

        Private Const FirmwareTag As String = " v"
        Private Const ReleaseTag As String = "Release="     ' Tag to identify the Release info in the Firmware file (BA-1871).
        Private Const ChecksumTag As String = "CHECKSUM="
        Private Const SizeTag As String = "SIZE="
        Private Const StartTag As String = "START"

#End Region

#Region "Enumerates"


#End Region

#Region "Attributes"

        Private FWUpdateCurrentActionAttr As FwUpdateActions = FwUpdateActions.None

        Private FWFileHeaderVersionAttr As String
        Private FWFileHeaderSizeAttr As Integer
        Private FWFileHeaderCRC32HexAttr As String = ""

        Private FWFileCRC32HexAttr As String
        Private FWFileBytesDataAttr As Byte()
        Private FWFileBlocksAttr As List(Of Byte())
        Private FWFileStringDataAttr As String = ""

        Private FWFileSendingBlockIndexAttr As Integer 'index of the block currently being sent
        Private FWFileSendingBlockSizeAttr As Integer 'size of the block currently being sent
        Private FwUpdateNeededAreas As List(Of FwUpdateAreas) = Nothing
        Private IsFwFileCompatibleAttr As Boolean = False
#End Region

#Region "Properties"

        Public Property FWUpdateCurrentAction() As FwUpdateActions
            Get
                Return FWUpdateCurrentActionAttr
            End Get
            Set(ByVal value As FwUpdateActions)
                FWUpdateCurrentActionAttr = value
            End Set
        End Property

        Public ReadOnly Property FWFileHeaderVersion() As String
            Get
                Return FWFileHeaderVersionAttr
            End Get
        End Property

        Public ReadOnly Property FWFileHeaderSize() As Integer
            Get
                Return FWFileHeaderSizeAttr
            End Get
        End Property

        Public ReadOnly Property FWFileHeaderCRC32() As String
            Get
                Return FWFileHeaderCRC32HexAttr
            End Get
        End Property

        Public ReadOnly Property FWFileCRC32Hex() As String
            Get
                Return FWFileCRC32HexAttr
            End Get
        End Property

        Public ReadOnly Property FWFileBytesData() As Byte()
            Get
                Return FWFileBytesDataAttr
            End Get
        End Property

        Public ReadOnly Property FWFileStringData() As String
            Get
                Return FWFileStringDataAttr
            End Get
        End Property


        Public ReadOnly Property FWFileSendingBlockIndex() As Integer
            Get
                Return MyClass.FWFileSendingBlockIndexAttr
            End Get
        End Property

        Public ReadOnly Property FWFileBlocks() As List(Of Byte())
            Get
                Return FWFileBlocksAttr
            End Get
        End Property

        Public Property IsFwFileCompatible() As Boolean
            Get
                Return IsFwFileCompatibleAttr
            End Get
            Set(ByVal value As Boolean)
                IsFwFileCompatibleAttr = value
            End Set
        End Property

#End Region

#Region "Public Methods"

        '''' <summary>
        '''' Update Firmware High Level Instruction
        '''' </summary>
        '''' <returns></returns>
        '''' <remarks>Created by SGM 30/06/2011</remarks>
        'Public Function StartUPDATE_FIRMWARE() As GlobalDataTO
        '    Dim myGlobal As New GlobalDataTO
        '    Try

        '        MyClass.FWUpdateCurrentActionAttr = FwUpdateActions.StartUpdate

        '        myGlobal = MyClass.SendFWUTIL

        '    Catch ex As Exception
        '        myGlobal.HasError = True
        '        myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myGlobal.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, "InstrumentUpdateUtilDelegate.StartUPDATE_FIRMWARE", EventLogEntryType.Error, False)
        '    End Try
        '    MyClass.RestoringAdjustmentsTextAttr = ""
        '    Return myGlobal
        'End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 20/05/2012</remarks>
        Public Function SendFWUTIL() As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Static BlockIndex As Integer
            Try

                Dim myFWUpdateRequest As FWUpdateRequestTO = Nothing



                If MyClass.FWUpdateCurrentAction <> FwUpdateActions.None Then
                    myFWUpdateRequest = New FWUpdateRequestTO(MyClass.FWUpdateCurrentAction)

                    With myFWUpdateRequest

                        .DataBlockIndex = 0
                        .DataBlockSize = 0

                        If .ActionType <> FwUpdateActions.SendRepository Then
                            BlockIndex = 0
                            .DataBlockBytes = Nothing
                        End If


                        If .ActionType = FwUpdateActions.StartUpdate Then
                            .DataBlockIndex = MyClass.FWFileBlocksAttr.Count
                            '.DataBlockSize = MyClass.FWFileSizeAttr

                        ElseIf .ActionType = FwUpdateActions.SendRepository Then
                            MyClass.FWFileSendingBlockIndexAttr = BlockIndex
                            .DataBlockIndex = MyClass.FWFileSendingBlockIndexAttr + 1
                            .DataBlockSize = MyClass.FWFileBlocksAttr(BlockIndex).Length
                            .DataBlockBytes = MyClass.FWFileBlocksAttr(BlockIndex)
                            BlockIndex += 1

                        Else
                            .DataBlockIndex = 0
                            .DataBlockSize = 0

                        End If


                    End With

                    myGlobal = myFwScriptDelegate.AnalyzerManager.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.FW_UTIL, _
                                                                                     True, Nothing, _
                                                                                     myFWUpdateRequest)
                End If

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "InstrumentUpdateUtilDelegate.SendFWUTIL", EventLogEntryType.Error, False)
            End Try
            Return myGlobal
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pFilePath"></param>
        ''' <param name="pDecrypt"></param>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 20/05/2012</remarks>
        Public Function OpenFwFile(ByVal pFilePath As String, Optional ByVal pDecrypt As Boolean = False) As GlobalDataTO

            Dim myGlobal As New GlobalDataTO
            Dim myTempFilePath As String = ""

            Try
                Dim myUtil As New Utilities
                Dim myFileBytes As Byte()

                '0- Decrypt if needed
                If pDecrypt Then
                    myTempFilePath = Directory.GetDirectoryRoot(pFilePath)
                    myGlobal = myUtil.DecryptFile(pFilePath, myTempFilePath)

                    If Not myGlobal.HasError And myGlobal.SetDatos IsNot Nothing Then
                        myFileBytes = (File.ReadAllBytes(myTempFilePath))
                    End If
                Else
                    myFileBytes = (File.ReadAllBytes(pFilePath))
                End If

                If Not myGlobal.HasError Then

                    Dim myCRC32Hex = ""
                    Dim myBytesData = myFileBytes
                    Dim myStringData = System.Text.ASCIIEncoding.ASCII.GetString(myFileBytes)

                    '1- Get Header Information
                    myGlobal = MyClass.GetFWFileHeaderInfo(myStringData)

                    If Not myGlobal.HasError Then

                        '2- Validate Compatibility with Software
                        myGlobal = MyClass.ValidateFwFileVersion(MyClass.FWFileHeaderVersion)
                        If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                            MyClass.IsFwFileCompatibleAttr = CBool(myGlobal.SetDatos)

                            If Not MyClass.IsFwFileCompatibleAttr Then
                                myGlobal.HasError = True
                            End If

                        End If

                        ''it is allowe any fw file - TO TEST !!!
                        'MyClass.IsFwFileCompatibleAttr = True

                        '3- Calculate CRC32 of all Repository File
                        If Not myGlobal.HasError Then
                            myGlobal = myFwScriptDelegate.AnalyzerManager.CalculateFwFileCRC32(myBytesData)
                            If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                                myCRC32Hex = CStr(myGlobal.SetDatos)
                            End If
                        End If

                        '4- Load obtained data: CRC calculated and bytes
                        If Not myGlobal.HasError Then
                            MyClass.FWFileCRC32HexAttr = myCRC32Hex
                            MyClass.FWFileBytesDataAttr = myFileBytes

                            MyClass.FWFileStringDataAttr = myStringData

                            myGlobal = MyClass.SplitFwFile(2048) 'split in 2048 bytes blocks
                            If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                                MyClass.FWFileBlocksAttr = CType(myGlobal.SetDatos, List(Of Byte()))
                            End If
                        End If
                    End If
                End If

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobal.ErrorMessage = ex.Message
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "InstrumentUpdateUtilDelegate.OpenFwFile", EventLogEntryType.Error, False)
            Finally
                If File.Exists(myTempFilePath) Then
                    File.Delete(myTempFilePath)
                End If
            End Try
            Return myGlobal
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 20/05/2012</remarks>
        Public Function SplitFwFile(ByVal pBytesCount As Integer) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Try
                Dim myBlocks As New List(Of Byte())
                If MyClass.FWFileBytesDataAttr.Length > 0 AndAlso MyClass.FWFileHeaderSizeAttr > 0 Then
                    Dim index = 0

                    While (index <= MyClass.FWFileBytesDataAttr.Length - pBytesCount)
                        Dim myBlock(pBytesCount - 1) As Byte
                        Array.Copy(MyClass.FWFileBytesDataAttr, index, myBlock, 0, pBytesCount)
                        myBlocks.Add(myBlock)
                        index += pBytesCount
                    End While

                    Dim rest As Integer = MyClass.FWFileBytesDataAttr.Length - index '- 1
                    If rest > 0 Then
                        Dim myBlock(rest - 1) As Byte
                        Array.Copy(MyClass.FWFileBytesDataAttr, index, myBlock, 0, rest)
                        myBlocks.Add(myBlock)
                    End If

                End If


                myGlobal.SetDatos = myBlocks

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobal.ErrorMessage = ex.Message
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "InstrumentUpdateUtilDelegate.SplitFwFile", EventLogEntryType.Error, False)
            End Try
            Return myGlobal
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pStringData"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Modified by XB 04/02/2013 - Upper conversions redundants because the value is already in UpperCase must delete to avoid Regional Settings problems (Bugs tracking #1112)
        '''             WE 21/01/2015 - BA-1871: extended with the ability to read the Release number in addition to the existing Version number.
        ''' </remarks>
        Private Function GetFWFileHeaderInfo(ByVal pStringData As String) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Try
                Dim myVersion As String = ""
                Dim myRelease As String = ""
                Dim myCRC32Hex As String = ""
                Dim mySize As Integer = -1

                Dim myLines As String() = pStringData.Split(CChar(vbCrLf))
                For L As Integer = 0 To myLines.Length - 1

                    Dim myLine As String = myLines(L)
                    If myLine.Length > 0 Then
                        If myLine.Contains(InstrumentUpdateUtilDelegate.FirmwareTag) Then
                            ' Extract the Firmware version number (entire string directly after the Firmware tag).
                            myVersion = myLine.Substring(myLine.IndexOf(InstrumentUpdateUtilDelegate.FirmwareTag) + 2).Trim

                        ElseIf myLine.Contains(InstrumentUpdateUtilDelegate.ReleaseTag) Then
                            ' Extract the Firmware release number (entire string directly after the Release tag).
                            myRelease = myLine.Substring(myLine.IndexOf("=") + 1).Replace(";", "").Trim

                        ElseIf myLine.Contains(InstrumentUpdateUtilDelegate.ChecksumTag) Then
                            myCRC32Hex = myLine.Substring(myLine.IndexOf("=") + 1).Replace(";", "").Trim    '.ToUpper

                        ElseIf myLine.Contains(InstrumentUpdateUtilDelegate.SizeTag) Then
                            mySize = CInt(myLine.Substring(myLine.IndexOf("=") + 1).Replace(";", "").Trim)
                            'If mySize Mod 4 > 0 Then
                            '    myGlobal.HasError = True
                            '    myGlobal.ErrorMessage = "File Size must be multiple of 4!!"
                            'End If
                            Exit For

                        End If
                    End If
                Next

                If Not myGlobal.HasError Then
                    'MyClass.FWFileHeaderVersionAttr = myVersion
                    MyClass.FWFileHeaderVersionAttr = myVersion & "." & myRelease
                    MyClass.FWFileHeaderCRC32HexAttr = myCRC32Hex
                    MyClass.FWFileHeaderSizeAttr = mySize
                End If

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobal.ErrorMessage = ex.Message
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "InstrumentUpdateUtilDelegate.GetFWFileHeaderInfo", EventLogEntryType.Error, False)
            End Try
            Return myGlobal
        End Function


        ''' <summary>
        ''' Validates the Fw File according to the versions compatibility
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>SGM 31/05/2012</remarks>
        Public Function ValidateFwFileVersion(ByVal pFWFileVersion As String) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Try
                Dim iscompatible As Boolean = False
                Dim myUtil As New Utilities
                myGlobal = myUtil.GetSoftwareVersion()
                If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
                    Dim SwVersion As String = myGlobal.SetDatos.ToString
                    myGlobal = MyClass.myFwScriptDelegate.AnalyzerManager.ValidateFwSwCompatibility(pFWFileVersion, SwVersion)
                    If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
                        iscompatible = CBool(myGlobal.SetDatos)
                    End If
                End If

                myGlobal.SetDatos = iscompatible

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobal.ErrorMessage = ex.Message
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "InstrumentUpdateUtilDelegate.ValidateFwFileVersion", EventLogEntryType.Error, False)
            End Try
            Return myGlobal
        End Function


#End Region

#End Region

    End Class

End Namespace
