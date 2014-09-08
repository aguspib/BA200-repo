Option Strict On
Option Explicit On

Imports System.Data.SqlClient
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global
Imports System.Text

Namespace Biosystems.Ax00.DAL.DAO

    Partial Public Class twksResultsDAO
        Inherits DAOBase

#Region "CRUD"
        ''' <summary>
        ''' Add a new result
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pResultsDS">Typed DataSet twksResults containing all data of the Result to add</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by: GDS 02/03/2010
        ''' Modified by: RH 05/20/2010 - Added parameters ExportStatus and Printed 
        '''              AG 12/07/2010 - Added ABS_WorkReagent (tested pending)
        '''              AG 10/11/2010 - Added ManualResult and ManualResultText fields
        '''              SA 21/01/2011 - Fields ResultDateTime and TS_User should not be NULLS: in that case,
        '''                              set ResultDateTime = Now and TS_User = logged User
        '''              AG 01/07/2011 - Added columns CurveSlope, CurveOffset, CurveCorrelation
        '''              SA 25/06/2012 - Added columns AnalyzerID and WorkSessionID; changed the function template. 
        '''                              Name changed from InsertNewResult to Create.
        '''              TR 19/07/2012 - Added columns CtrlsSendingGroup and SampleClass used on QC.
        ''' </remarks>
        Public Function Create(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pResultsDS As ResultsDS) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = ""
                    With pResultsDS.twksResults(0)
                        cmdText = " INSERT INTO twksResults (OrderTestID, RerunNumber, MultiPointNumber, ValidationStatus, TestVersion, " & vbCrLf & _
                                                           " AcceptedResultFlag, ManualResultFlag, ExportStatus, Printed, ABSValue, ABS_Error, " & vbCrLf & _
                                                           " SubstrateDepletion, ABS_Initial, ABS_MainFilter, ABS_WorkReagent, CalibratorFactor, " & vbCrLf & _
                                                           " CalibrationError, CalibratorBlankAbsUsed, CurveResultsID, CurveGrowthType, " & vbCrLf & _
                                                           " CurveType, CurveAxisXType, CurveAxisYType, RelativeErrorCurve, CONC_Value, CONC_Error, " & vbCrLf & _
                                                           " ResultDateTime, ManualResult, ManualResultText, AnalyzerID, WorkSessionID, CurveSlope, " & vbCrLf & _
                                                           " CurveOffset, CurveCorrelation, CtrlsSendingGroup, SampleClass, TS_User, TS_DateTime) " & vbCrLf & _
                                  " VALUES (" & .OrderTestID.ToString & ", " & .RerunNumber.ToString & ", " & .MultiPointNumber.ToString & ", " & vbCrLf

                        If (.IsValidationStatusNull) Then
                            cmdText &= "NULL, "
                        Else
                            cmdText &= "'" & .ValidationStatus.Trim & "', "
                        End If

                        If (.IsTestVersionNull) Then
                            cmdText &= "NULL, "
                        Else
                            cmdText &= .TestVersion.ToString & ", "
                        End If

                        If (.IsAcceptedResultFlagNull) Then
                            cmdText &= "0, "
                        Else
                            cmdText &= "1, "
                        End If

                        If (.IsManualResultFlagNull) Then
                            cmdText &= "0, "
                        Else
                            cmdText &= IIf(.ManualResultFlag, 1, 0).ToString & ", "
                        End If

                        If (.IsExportStatusNull) Then
                            cmdText &= "NULL, "
                        Else
                            cmdText &= "'" & .ExportStatus.Trim & "', "
                        End If

                        If (.IsPrintedNull) Then
                            cmdText &= "0, "
                        Else
                            cmdText &= IIf(.Printed, 1, 0).ToString & ", "
                        End If

                        If (.IsABSValueNull) Then
                            cmdText &= "NULL, "
                        Else
                            cmdText &= ReplaceNumericString(.ABSValue) & ", "
                        End If

                        If (.IsABS_ErrorNull) Then
                            cmdText &= "NULL, "
                        Else
                            cmdText &= "'" & .ABS_Error.Trim & "', "
                        End If

                        If (.IsSubstrateDepletionNull) Then
                            cmdText &= "NULL, "
                        Else
                            cmdText &= .SubstrateDepletion.ToString & ", "
                        End If

                        If (.IsABS_InitialNull) Then
                            cmdText &= "NULL, "
                        Else
                            cmdText &= ReplaceNumericString(.ABS_Initial) & ", "
                        End If

                        If (.IsABS_MainFilterNull) Then
                            cmdText &= "NULL, "
                        Else
                            cmdText &= ReplaceNumericString(.ABS_MainFilter) & ", "
                        End If

                        If (.IsAbs_WorkReagentNull) Then
                            cmdText &= "NULL, "
                        Else
                            cmdText &= ReplaceNumericString(.Abs_WorkReagent) & ", "
                        End If

                        If (.IsCalibratorFactorNull) Then
                            cmdText &= "NULL, "
                        Else
                            cmdText &= ReplaceNumericString(.CalibratorFactor) & ", "
                        End If

                        If (.IsCalibrationErrorNull) Then
                            cmdText &= "NULL, "
                        Else
                            cmdText &= "'" & .CalibrationError.Trim & "', "
                        End If

                        If (.IsCalibratorBlankAbsUsedNull) Then
                            cmdText &= "NULL, "
                        Else
                            cmdText &= ReplaceNumericString(.CalibratorBlankAbsUsed) & ", "
                        End If

                        If (.IsCurveResultsIDNull) Then
                            cmdText &= "NULL, "
                        Else
                            cmdText &= .CurveResultsID.ToString & ", "
                        End If

                        If (.IsCurveGrowthTypeNull) Then
                            cmdText &= "NULL, "
                        Else
                            cmdText &= "'" & .CurveGrowthType.Trim & "', "
                        End If

                        If (.IsCurveTypeNull) Then
                            cmdText &= "NULL, "
                        Else
                            cmdText &= "'" & .CurveType.Trim & "', "
                        End If

                        If (.IsCurveAxisXTypeNull) Then
                            cmdText &= "NULL, "
                        Else
                            cmdText &= "'" & .CurveAxisXType.Trim & "', "
                        End If

                        If (.IsCurveAxisYTypeNull) Then
                            cmdText &= " NULL, "
                        Else
                            cmdText &= "'" & .CurveAxisYType.Trim & "', "
                        End If

                        If (.IsRelativeErrorCurveNull) Then
                            cmdText &= "NULL, "
                        Else
                            cmdText &= ReplaceNumericString(.RelativeErrorCurve) & ", "
                        End If

                        If (.IsCONC_ValueNull) Then
                            cmdText &= "NULL, "
                        Else
                            cmdText &= ReplaceNumericString(.CONC_Value) & ", "
                        End If

                        If (.IsCONC_ErrorNull) Then
                            cmdText &= "NULL, "
                        Else
                            cmdText &= "'" & .CONC_Error.Trim & "', "
                        End If

                        If (.IsResultDateTimeNull) Then
                            cmdText &= "'" & Now.ToString("yyyyMMdd HH:mm:ss") & "', "
                        Else
                            cmdText &= "'" & .ResultDateTime.ToString("yyyyMMdd HH:mm:ss") & "', "
                        End If

                        If (.IsManualResultNull) Then
                            cmdText &= "NULL, "
                        Else
                            cmdText &= ReplaceNumericString(.ManualResult) & ", "
                        End If

                        If (.IsManualResultTextNull) Then
                            cmdText &= "NULL, "
                        Else
                            cmdText &= "N'" & .ManualResultText.ToString.Replace("'", "''") & "', "
                        End If

                        If (.IsAnalyzerIDNull) Then
                            cmdText &= "NULL, "
                        Else
                            cmdText &= "N'" & .AnalyzerID.ToString.Replace("'", "''") & "', "
                        End If

                        If (.IsWorkSessionIDNull) Then
                            cmdText &= "NULL, "
                        Else
                            cmdText &= "N'" & .WorkSessionID.ToString.Replace("'", "''") & "', "
                        End If

                        If (.IsCurveSlopeNull) Then
                            cmdText &= "NULL, "
                        Else
                            cmdText &= ReplaceNumericString(.CurveSlope) & ", "
                        End If

                        If (.IsCurveOffsetNull) Then
                            cmdText &= "NULL, "
                        Else
                            cmdText &= ReplaceNumericString(.CurveOffset) & ", "
                        End If

                        If (.IsCurveCorrelationNull) Then
                            cmdText &= "NULL, "
                        Else
                            cmdText &= ReplaceNumericString(.CurveCorrelation) & ", "
                        End If
                        'TR 19/07/2012
                        If (.IsCtrlsSendingGroupNull) Then
                            cmdText &= " NULL, "
                        Else
                            cmdText &= .CtrlsSendingGroup.ToString() & ", "
                        End If

                        If (.IsSampleClassNull) Then
                            cmdText &= " NULL, "
                        Else
                            cmdText &= "'" & .SampleClass & "', "
                        End If
                        'TR 19/07/2012 -END.

                        If (.IsTS_UserNull) Then
                            'Get the connected Username from the current Application Session
                            Dim currentSession As New GlobalBase
                            cmdText &= "N'" & currentSession.GetSessionInfo().UserName.Replace("'", "''") & "', "
                        Else
                            cmdText &= "N'" & .TS_User.Trim & "', "
                        End If

                        If (.IsTS_DateTimeNull) OrElse (Not IsDate(.TS_DateTime)) Then
                            cmdText &= "'" & Now.ToString("yyyyMMdd HH:mm:ss") & "'"
                        Else
                            cmdText &= "'" & .TS_DateTime.ToString("yyyyMMdd HH:mm:ss") & "'"
                        End If
                        cmdText &= ")"
                    End With

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
                myLogAcciones.CreateLogActivity(ex.Message, "twksResultsDAO.Create", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Insert a specific row of twksResults
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pResultsDS">twksResults DataSet</param>
        ''' <returns>GlobalDataTO containing a typed DataSet ResultsDS with all affected records</returns>
        ''' <remarks>
        ''' Created by: GDS 02/03/2010
        ''' Modified by: RH 05/20/2010 - Added parameters ExportStatus and Printed 
        '''              AG 12/07/2010 - Added ABS_WorkReagent (tested pending)
        '''              AG 10/11/2010 - Added ManualResult and ManualResultText fields
        '''              SA 21/01/2011 - Fields ResultDateTime and TS_User should not be NULLS: in that case,
        '''                              set ResultDateTime = Now and TS_User = logged User
        '''              AG 01/07/2011 - Add columns CurveSlope, CurveOffset, CurveCorrelation
        ''' </remarks>
        Public Function InsertResult(ByVal pDBConnection As SqlClient.SqlConnection, _
                                     ByVal pResultsDS As ResultsDS) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String
                    With pResultsDS.twksResults(0)
                        cmdText = "INSERT INTO twksResults " & _
                                  "( OrderTestID" & _
                                  ", RerunNumber" & _
                                  ", MultiPointNumber" & _
                                  ", ValidationStatus" & _
                                  ", TestVersion" & _
                                  ", AcceptedResultFlag" & _
                                  ", ManualResultFlag" & _
                                  ", ExportStatus" & _
                                  ", Printed" & _
                                  ", ABSValue" & _
                                  ", ABS_Error" & _
                                  ", SubstrateDepletion" & _
                                  ", ABS_Initial" & _
                                  ", ABS_MainFilter" & _
                                  ", ABS_WorkReagent" & _
                                  ", CalibratorFactor" & _
                                  ", CalibrationError" & _
                                  ", CalibratorBlankAbsUsed" & _
                                  ", CurveResultsID" & _
                                  ", CurveGrowthType" & _
                                  ", CurveType" & _
                                  ", CurveAxisXType" & _
                                  ", CurveAxisYType" & _
                                  ", RelativeErrorCurve" & _
                                  ", CONC_Value" & _
                                  ", CONC_Error" & _
                                  ", ResultDateTime" & _
                                  ", ManualResult" & _
                                  ", ManualResultText" & _
                                  ", CurveSlope" & _
                                  ", CurveOffset" & _
                                  ", CurveCorrelation" & _
                                  ", TS_User" & _
                                  ", TS_DateTime)" & _
                                  " VALUES " & _
                                  "( " & .OrderTestID.ToString & _
                                  ", " & .RerunNumber.ToString & _
                                  ", " & .MultiPointNumber.ToString

                        If .IsValidationStatusNull Then
                            cmdText += ", NULL"
                        Else
                            cmdText += ", '" & .ValidationStatus & "'"
                        End If

                        If .IsTestVersionNull Then
                            cmdText += ", NULL"
                        Else
                            cmdText += ", " & .TestVersion.ToString
                        End If

                        If .IsAcceptedResultFlagNull Then
                            cmdText += ", 'False'"
                        Else
                            cmdText += ", '" & .AcceptedResultFlag.ToString & "'"
                        End If

                        If .IsManualResultFlagNull Then
                            cmdText += ", 'False'"
                        Else
                            cmdText += ", '" & .ManualResultFlag.ToString & "'"
                        End If

                        If .IsExportStatusNull Then
                            cmdText += ", NULL"
                        Else
                            cmdText += ", '" & .ExportStatus & "'"
                        End If

                        If .IsPrintedNull Then
                            cmdText += ", 'False'"
                        Else
                            cmdText += ", '" & .Printed.ToString & "'"
                        End If

                        If .IsABSValueNull Then
                            cmdText += ", NULL"
                        Else
                            'AG 10/06/2010
                            '' modified by dl 12/03/2010
                            ''cmdText += ", " & pResultsDS.twksResults(0).ABSValue.ToString.Replace(",", ".") & vbCrLf
                            'cmdText += ", " & .ABSValue.ToSQLString()
                            cmdText += ", " & ReplaceNumericString(.ABSValue)
                        End If

                        If .IsABS_ErrorNull Then
                            cmdText += ", NULL" & vbCrLf
                        Else
                            cmdText += ", '" & .ABS_Error & "'"
                        End If

                        If .IsSubstrateDepletionNull Then
                            cmdText += ", NULL"
                        Else
                            cmdText += ", " & .SubstrateDepletion.ToString
                        End If

                        If .IsABS_InitialNull Then
                            cmdText += ", NULL"
                        Else
                            'AG 10/06/2010
                            '' modified by dl 12/03/2010
                            ''cmdText += ", " & pResultsDS.twksResults(0).ABS_Initial.ToString.Replace(",", ".") & vbCrLf
                            'cmdText += ", " & .ABS_Initial.ToSQLString()
                            cmdText += ", " & ReplaceNumericString(.ABS_Initial)
                        End If

                        If .IsABS_MainFilterNull Then
                            cmdText += ", NULL"
                        Else
                            'AG 10/06/2010
                            '' modified by dl 12/03/2010
                            ''cmdText += ", " & pResultsDS.twksResults(0).ABS_MainFilter.ToString.Replace(",", ".") & vbCrLf
                            'cmdText += ", " & .ABS_MainFilter.ToSQLString()
                            cmdText += ", " & ReplaceNumericString(.ABS_MainFilter)
                        End If

                        'AG 12/07/2010
                        If .IsAbs_WorkReagentNull Then
                            cmdText += ", NULL"
                        Else
                            cmdText += ", " & ReplaceNumericString(.Abs_WorkReagent)
                        End If
                        'END AG 12/07/2010

                        If .IsCalibratorFactorNull Then
                            cmdText += ", NULL"
                        Else

                            '' modified by dl 12/03/2010
                            ''cmdText += ", " & pResultsDS.twksResults(0).CalibratorFactor.ToString.Replace(",", ".") & vbCrLf
                            'cmdText += ", " & .CalibratorFactor.ToSQLString()
                            cmdText += ", " & ReplaceNumericString(.CalibratorFactor)
                        End If

                        If .IsCalibrationErrorNull Then
                            cmdText += ", NULL"
                        Else
                            cmdText += ", '" & .CalibrationError & "'"
                        End If

                        If .IsCalibratorBlankAbsUsedNull Then
                            cmdText += ", NULL"
                        Else
                            'AG 10/06/2010
                            ' modified by dl 12/03/2010
                            'cmdText += ", " & pResultsDS.twksResults(0).CalibratorBlankAbsUsed.ToString.Replace(",", ".") & vbCrLf
                            'cmdText += ", " & .CalibratorBlankAbsUsed.ToSQLString()
                            cmdText += ", " & ReplaceNumericString(.CalibratorBlankAbsUsed)
                        End If

                        If .IsCurveResultsIDNull Then
                            cmdText += ", NULL"
                        Else
                            cmdText += ", " & .CurveResultsID.ToString
                        End If

                        If .IsCurveGrowthTypeNull Then
                            cmdText += ", NULL"
                        Else
                            cmdText += ", '" & .CurveGrowthType & "'"
                        End If

                        If .IsCurveTypeNull Then
                            cmdText += ", NULL"
                        Else
                            cmdText += ", '" & .CurveType & "'"
                        End If

                        If .IsCurveAxisXTypeNull Then
                            cmdText += ", NULL"
                        Else
                            cmdText += ", '" & .CurveAxisXType & "'"
                        End If

                        If .IsCurveAxisYTypeNull Then
                            cmdText += ", NULL"
                        Else
                            'AG 12/03/2010
                            'cmdText += ", '" & pResultsDS.twksResults(0).IsCurveAxisYTypeNull.ToString & "'" & vbCrLf
                            cmdText += ", '" & .CurveAxisYType & "'"
                            'AG 12/03/2010

                        End If

                        If .IsRelativeErrorCurveNull Then
                            cmdText += ", NULL"
                        Else
                            'AG 10/06/2010
                            ' modified by dl 12/03/2010
                            'cmdText += ", " & pResultsDS.twksResults(0).RelativeErrorCurve.ToString.Replace(",", ".") & vbCrLf
                            'cmdText += ", " & .RelativeErrorCurve.ToSQLString()
                            cmdText += ", " & ReplaceNumericString(.RelativeErrorCurve)
                        End If

                        If .IsCONC_ValueNull Then
                            cmdText += ", NULL"
                        Else
                            'AG 10/06/2010
                            ' modified by dl 12/03/2010
                            'cmdText += ", " & pResultsDS.twksResults(0).CONC_Value.ToString.Replace(",", ".") & vbCrLf
                            'cmdText += ", " & .CONC_Value.ToSQLString()
                            cmdText += ", " & ReplaceNumericString(.CONC_Value)
                        End If

                        If .IsCONC_ErrorNull Then
                            cmdText += ", NULL"
                        Else
                            cmdText += ", '" & .CONC_Error & "'"
                        End If

                        If .IsResultDateTimeNull Then
                            cmdText += ", '" & Now.ToString("yyyyMMdd HH:mm:ss") & "'"
                        Else
                            cmdText += ", '" & .ResultDateTime.ToString("yyyyMMdd HH:mm:ss") & "'"
                        End If

                        'AG 10/11/2010 - Add new fields ManualResult and ManualResultText
                        If .IsManualResultNull Then
                            cmdText += ", NULL"
                        Else
                            cmdText += ", " & ReplaceNumericString(.ManualResult)
                        End If

                        If .IsManualResultTextNull Then
                            cmdText += ", NULL"
                        Else
                            cmdText += ", N'" & .ManualResultText.ToString.Replace("'", "''") & "'"
                        End If
                        'END AG 10/11/2010

                        'AG 01/07/2011
                        If .IsCurveSlopeNull Then
                            cmdText += ", NULL"
                        Else
                            cmdText += ", " & ReplaceNumericString(.CurveSlope)
                        End If
                        If .IsCurveOffsetNull Then
                            cmdText += ", NULL"
                        Else
                            cmdText += ", " & ReplaceNumericString(.CurveOffset)
                        End If
                        If .IsCurveCorrelationNull Then
                            cmdText += ", NULL"
                        Else
                            cmdText += ", " & ReplaceNumericString(.CurveCorrelation)
                        End If
                        'AG 01/07/2011

                        If .IsTS_UserNull Then
                            'Get the connected Username from the current Application Session
                            Dim currentSession As New GlobalBase
                            cmdText += ", N'" & currentSession.GetSessionInfo().UserName.Replace("'", "''") & "'"
                        Else
                            cmdText += ", '" & .TS_User & "'"
                        End If

                        If .IsResultDateTimeNull Then
                            cmdText += ", '" & Now.ToString("yyyyMMdd HH:mm:ss") & "'"

                        ElseIf Not IsDate(.TS_DateTime) Then
                            cmdText += ", '" & Now.ToString("yyyyMMdd HH:mm:ss") & "'"

                        Else
                            cmdText += ", '" & .TS_DateTime.ToString("yyyyMMdd HH:mm:ss") & "'"
                        End If

                        cmdText += ")"
                    End With

                    'AG 25/07/2014 RQ00086 - improve memory usage
                    'Dim dbCmd As New SqlCommand
                    'dbCmd.Connection = pDBConnection
                    'dbCmd.CommandText = cmdText

                    'resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                    'If (resultData.AffectedRecords = 1) Then
                    '    resultData.HasError = False
                    'Else
                    '    resultData.HasError = True
                    '    resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                    'End If

                    Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                        resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                        If (resultData.AffectedRecords = 1) Then
                            resultData.HasError = False
                        Else
                            resultData.HasError = True
                        End If
                    End Using
                    'AG 25/07/2014 RQ00086

                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksResultsDAO.InsertResult", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get the accepted Results for the specified OrderTestID and rerunnumber 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pOrderTestID">Order Test Identifier</param>
        ''' <param name="prerunnumber" >rerunnumber</param>
        ''' <returns>GlobalDataTO containing a typed DataSet ResultsDS with all accepted Results found for the informed OrderTestID and rerunnumber</returns>
        ''' <remarks>
        ''' Created by:  DL 06/05/2010 
        ''' Modified by: AG 12/07/2010 - Add field Abs_WorkReagent (tested OK)
        '''              AG 30/08/2010 - Add fields ResultDateTime and sort query by MultiPointNumber
        '''              AG 09/11/2010 - Select also ManualResultFlag, ManualResult and ManualResultText
        '''              AG 01/07/2011 - Return columns: CurveOffset. CurveSlope and CurveCorrelation
        ''' </remarks>
        Public Function ReadByOrderTestIDandRerunNumber(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOrderTestID As Integer, _
                                                        ByVal pRerunNumber As Integer) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = ""
                        cmdText = cmdText & " SELECT OrderTestID, RerunNumber, MultiPointNumber, ValidationStatus, AcceptedResultFlag, TestVersion, ManualResultFlag, " & vbCrLf
                        cmdText = cmdText & "        UserComment, ExportStatus, ExportDateTime, Printed, ABSValue, ABS_Error, SubstrateDepletion, ABS_Initial, " & vbCrLf
                        cmdText = cmdText & "        ABS_MainFilter, CalibratorFactor, CalibrationError, CalibratorBlankAbsUsed, CurveResultsID, CurveGrowthType, " & vbCrLf
                        cmdText = cmdText & "        CurveType, CurveAxisXType, CurveAxisYType, RelativeErrorCurve, CONC_Value, CONC_Error, ABS_WorkReagent, ResultDateTime, " & vbCrLf
                        cmdText = cmdText & "        ManualResultFlag, ManualResult, ManualResultText, CurveSlope, CurveOffset, CurveCorrelation " & vbCrLf
                        cmdText = cmdText & " FROM   twksResults" & vbCrLf
                        cmdText = cmdText & " WHERE  OrderTestID = " & pOrderTestID & vbCrLf
                        cmdText = cmdText & " AND    RerunNumber = " & pRerunNumber & vbCrLf
                        cmdText = cmdText & " ORDER BY MultiPointNumber"

                        Dim dbCmd As New SqlClient.SqlCommand
                        dbCmd.Connection = dbConnection
                        dbCmd.CommandText = cmdText

                        'Fill the DataSet to return 
                        Dim lastResultsDS As New ResultsDS
                        Dim dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                        dbDataAdapter.Fill(lastResultsDS.twksResults)

                        resultData.SetDatos = lastResultsDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksResultsDAO.ReadByOrderTestIDandRerunNumber", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Update fields ExportStatus and ExportDataTime for the group of results sent to and external LIMS system
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pResultsDS">Typed DataSet ResultsDS containing the exported results</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by:  TR 14/05/2010
        ''' Modified by: TR 12/07/2012 - Changed implementation to allow the update of several Results
        '''              SA 01/08/2012 - If ExportStatus is not informed, set SENT as value; if TS_User is not informed, set the connected 
        '''                              User; changed implementation to send to SQL Server groups of updates instead of one by one
        ''' Modified by: SG 10/04/2013 - new parameter "pAlternativeStatus". If this parameter != “” then the new ExportStatus = pAlternativeStatus
        ''' Modified by: SG 25-07-2013 - In case of the informed ResultsDS contains the data in the twksResults object, they must be copied to the 
        '''                              vwksResults object
        ''' </remarks>
        Public Function UpdateExportStatus(ByVal pDBConnection As SqlClient.SqlConnection, _
                                           ByVal pResultsDS As ResultsDS, _
                                           ByVal pAlternativeStatus As String) As GlobalDataTO

            Dim myGlobalDataTO As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString

                Else
                    'SGM 25-07-2013 - In case of the informed ResultsDS contains the data in the twksResults object, they must be copied to the vwksResults object
                    If pResultsDS.vwksResults.Rows.Count = 0 AndAlso pResultsDS.twksResults.Rows.Count > 0 Then
                        For Each R As ResultsDS.twksResultsRow In pResultsDS.twksResults.Rows
                            
                            Dim myRow As ResultsDS.vwksResultsRow = pResultsDS.vwksResults.NewvwksResultsRow
                            With myRow
                                .BeginEdit()
                                .OrderTestID = R.OrderTestID
                                .RerunNumber = R.RerunNumber
                                .EndEdit()
                            End With
                            pResultsDS.vwksResults.AddvwksResultsRow(myRow)
                            pResultsDS.AcceptChanges()
                        Next
                    End If
                    'end SGM 25-07-2013

                    Dim cmdText As New StringBuilder
                    For Each myResultsRow As ResultsDS.vwksResultsRow In pResultsDS.vwksResults.Rows
                        cmdText.Append("UPDATE twksResults SET" & vbCrLf)

                        'SG 10/04/2013
                        Dim myNewStatus As String = ""
                        If pAlternativeStatus.Length > 0 Then
                            myNewStatus = pAlternativeStatus.Trim
                        ElseIf (myResultsRow.IsExportStatusNull) Then
                            myNewStatus = "SENT"
                        Else
                            myNewStatus = myResultsRow.ExportStatus.Trim
                        End If

                        cmdText.Append("ExportStatus = '" & myNewStatus & "'" & vbCrLf)
                        'SG 10/04/2013

                        If (myResultsRow.IsExportDateTimeNull) Then
                            cmdText.Append(", ExportDateTime = '" & Now.ToString("yyyyMMdd HH:mm:ss") & "'" & vbCrLf)
                        Else
                            cmdText.Append(", ExportDateTime = '" & myResultsRow.ExportDateTime.ToString("yyyyMMdd HH:mm:ss") & "'" & vbCrLf)
                        End If
                        If (myResultsRow.IsTS_UserNull) Then
                            'Get the connected Username from the current Application Session
                            Dim currentSession As New GlobalBase
                            cmdText.Append(", TS_User = N'" & currentSession.GetSessionInfo().UserName.Replace("'", "''") & "'" & vbCrLf)
                        Else
                            cmdText.Append(", TS_User = N'" & myResultsRow.TS_User.Trim.Replace("'", "''") & "'" & vbCrLf)
                        End If
                        If (myResultsRow.IsTS_DateTimeNull) Then
                            cmdText.Append(", TS_DateTime = '" & Now.ToString("yyyyMMdd HH:mm:ss") & "'" & vbCrLf)
                        Else
                            cmdText.Append(", TS_DateTime = '" & myResultsRow.TS_DateTime.ToString("yyyyMMdd HH:mm:ss") & "'" & vbCrLf)
                        End If

                        cmdText.Append("WHERE OrderTestID      = " & myResultsRow.OrderTestID & vbCrLf)
                        cmdText.Append("  AND RerunNumber      = " & myResultsRow.RerunNumber & vbCrLf)

                        If Not myResultsRow.IsMultiPointNumberNull Then
                            cmdText.Append("  AND MultiPointNumber = " & myResultsRow.MultiPointNumber)
                        End If
                        cmdText.Append(Environment.NewLine)
                    Next

                    If cmdText.Length > 0 Then
                        Using dbCmd As New SqlCommand(cmdText.ToString(), pDBConnection)
                            myGlobalDataTO.AffectedRecords = dbCmd.ExecuteNonQuery()
                            myGlobalDataTO.HasError = False
                        End Using
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksResultsDAO.UpdateExportStatus", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function


        ''' <summary>
        ''' Update fields ExportStatus and ExportDateTime for a group of Patient Results that have been selected to be exported 
        ''' manually to LIS. Update also timestamp fields (TS_User and TS_DateTime). This function does not fulfill the template 
        ''' for DAO functions due to the quantity of data to update can be huge
        ''' </summary>
        ''' <param name="pResultsDS">Typed DataSet ResultsDS containing all Patient Results that have been exported</param>
        ''' <param name="pAlternativeStatus">When informed, the status of all Results will be updated with this value</param>
        ''' <returns>GlobalDataTO containing success/errorinformation</returns>
        ''' <remarks>
        ''' Created by:  SA 12/02/2014 - BT #1497
        ''' </remarks>
        Public Function UpdateExportStatusMASIVE(ByVal pResultsDS As ResultsDS, ByVal pAlternativeStatus As String) As GlobalDataTO
            Dim openDBConn As Boolean = False
            Dim openDBTran As Boolean = False
            Dim myGlobalDataTO As New GlobalDataTO
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                Dim i As Integer = 0
                Dim maxInserts As Integer = 100
                Dim cmdText As New StringBuilder()
                Dim myNewStatus As String = String.Empty

                'Get the connected Username from the current Application Session
                Dim currentSession As New GlobalBase
                Dim connectedUser As String = currentSession.GetSessionInfo().UserName

                'Open a DB Connection
                myGlobalDataTO = GetOpenDBConnection(dbConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        openDBConn = True

                        For Each myResultsRow As ResultsDS.vwksResultsRow In pResultsDS.vwksResults.Rows
                            cmdText.Append(" UPDATE twksResults SET " & vbCrLf)

                            If (pAlternativeStatus.Length > 0) Then
                                myNewStatus = pAlternativeStatus.Trim
                            ElseIf (myResultsRow.IsExportStatusNull) Then
                                myNewStatus = "SENT"
                            Else
                                myNewStatus = myResultsRow.ExportStatus.Trim
                            End If
                            cmdText.Append("ExportStatus = '" & myNewStatus & "'" & vbCrLf)

                            If (myResultsRow.IsExportDateTimeNull) Then
                                cmdText.Append(", ExportDateTime = '" & Now.ToString("yyyyMMdd HH:mm:ss") & "'" & vbCrLf)
                            Else
                                cmdText.Append(", ExportDateTime = '" & myResultsRow.ExportDateTime.ToString("yyyyMMdd HH:mm:ss") & "'" & vbCrLf)
                            End If
                            If (myResultsRow.IsTS_UserNull) Then
                                cmdText.Append(", TS_User = N'" & connectedUser.Replace("'", "''") & "'" & vbCrLf)
                            Else
                                cmdText.Append(", TS_User = N'" & myResultsRow.TS_User.Trim.Replace("'", "''") & "'" & vbCrLf)
                            End If

                            If (myResultsRow.IsTS_DateTimeNull) Then
                                cmdText.Append(", TS_DateTime = '" & Now.ToString("yyyyMMdd HH:mm:ss") & "'" & vbCrLf)
                            Else
                                cmdText.Append(", TS_DateTime = '" & myResultsRow.TS_DateTime.ToString("yyyyMMdd HH:mm:ss") & "'" & vbCrLf)
                            End If

                            cmdText.Append("WHERE OrderTestID      = " & myResultsRow.OrderTestID & vbCrLf)
                            cmdText.Append("  AND RerunNumber      = " & myResultsRow.RerunNumber & vbCrLf)

                            If (Not myResultsRow.IsMultiPointNumberNull) Then cmdText.Append("  AND MultiPointNumber = " & myResultsRow.MultiPointNumber)
                            cmdText.Append(Environment.NewLine)

                            'Increment the sentences counter and verify if the max has been reached
                            i += 1
                            If (i = maxInserts) Then
                                DAOBase.BeginTransaction(dbConnection)
                                openDBTran = True

                                'Execute the SQL scripts
                                Using dbCmd As New SqlClient.SqlCommand(cmdText.ToString, dbConnection)
                                    myGlobalDataTO.AffectedRecords += dbCmd.ExecuteNonQuery()
                                End Using

                                If (Not myGlobalDataTO.HasError) Then
                                    DAOBase.CommitTransaction(dbConnection)
                                Else
                                    DAOBase.RollbackTransaction(dbConnection)
                                End If
                                openDBTran = False

                                'Initialize the counter and the StringBuilder
                                i = 0
                                cmdText.Remove(0, cmdText.Length)
                            End If
                        Next

                        If (Not myGlobalDataTO.HasError) Then
                            If (cmdText.Length > 0) Then
                                DAOBase.BeginTransaction(dbConnection)
                                openDBTran = True

                                'Execute the remaining scripts
                                Using dbCmd As New SqlClient.SqlCommand(cmdText.ToString, dbConnection)
                                    myGlobalDataTO.AffectedRecords += dbCmd.ExecuteNonQuery()
                                End Using

                                If (Not myGlobalDataTO.HasError) Then
                                    DAOBase.CommitTransaction(dbConnection)
                                Else
                                    DAOBase.RollbackTransaction(dbConnection)
                                End If
                                openDBTran = False
                            End If
                        End If
                    End If
                End If
            Catch ex As Exception
                If (Not dbConnection Is Nothing AndAlso openDBTran) Then DAOBase.RollbackTransaction(dbConnection)

                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksResultsDAO.UpdateExportStatusMASIVE", EventLogEntryType.Error, False)
            Finally
                If (Not dbConnection Is Nothing AndAlso openDBConn) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Update a specific row of twksResults
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pResultsDS">twksResults DataSet</param>
        ''' <returns>GlobalDataTO containing a typed DataSet ResultsDS with all affected records</returns> 
        ''' <remarks>
        ''' Created by: GDS 02/03/2010 
        ''' Modified by: RH 05/20/2010 - Added parameters ExportStatus and Printed
        '''              AG 12/07/2010 - Added ABS_WorkReagent (tested ok)
        '''              AG 21/07/2010 - If AcceptedResultFlag is not informed, do not update it
        '''              AG 10/11/2010 - Add ManualResult and ManualResultText fields
        '''              AG 01/07/2011 - Add columns CurveSlope, CurveOffset, CurveCorrelation
        ''' </remarks>
        Public Function UpdateResult(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pResultsDS As ResultsDS) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String
                    With pResultsDS.twksResults(0)
                        cmdText = "UPDATE twksResults SET"

                        If .IsValidationStatusNull Then
                            cmdText += " ValidationStatus = NULL"
                        Else
                            cmdText += " ValidationStatus = '" & .ValidationStatus & "'"
                        End If

                        If .IsTestVersionNull Then
                            cmdText += ", TestVersion = NULL"
                        Else
                            cmdText += ", TestVersion = " & .TestVersion.ToString
                        End If

                        If .IsAcceptedResultFlagNull Then
                            'AG 21/07/2010
                            'cmdText += ", AcceptedResultFlag = 'False'"
                        Else
                            cmdText += ", AcceptedResultFlag = '" & .AcceptedResultFlag.ToString & "'"
                        End If

                        If .IsManualResultFlagNull Then
                            cmdText += ", ManualResultFlag = 'False'"
                        Else
                            cmdText += ", ManualResultFlag = '" & .ManualResultFlag.ToString & "'"
                        End If

                        If .IsExportStatusNull Then
                            cmdText += ", ExportStatus = NULL"
                        Else
                            cmdText += ", ExportStatus = '" & .ExportStatus & "'"
                        End If

                        If .IsPrintedNull Then
                            cmdText += ", Printed = 'False'"
                        Else
                            cmdText += ", Printed = '" & .Printed.ToString & "'"
                        End If

                        If .IsABSValueNull Then
                            cmdText += ", ABSValue = NULL"
                        Else
                            'AG 10/06/2010
                            '' modified by dl 12/03/2010
                            ''cmdText += ", ABSValue = " & pResultsDS.twksResults(0).ABSValue.ToString.Replace(",", ".") & vbCrLf
                            'cmdText += ", ABSValue = " & .ABSValue.ToSQLString()
                            cmdText += ", ABSValue = " & ReplaceNumericString(.ABSValue)
                        End If

                        If .IsABS_ErrorNull Then
                            cmdText += ", ABS_Error = NULL"
                        Else
                            cmdText += ", ABS_Error = '" & .ABS_Error & "'"
                        End If

                        If .IsSubstrateDepletionNull Then
                            cmdText += ", SubstrateDepletion = NULL"
                        Else
                            cmdText += ", SubstrateDepletion = " & .SubstrateDepletion.ToString
                        End If

                        If .IsABS_InitialNull Then
                            cmdText += ", ABS_Initial = NULL"
                        Else
                            'AG 10/06/2010
                            '' modified by dl 12/03/2010
                            ''cmdText += ", ABS_Initial = " & pResultsDS.twksResults(0).ABS_Initial.ToString.Replace(",", ".") & vbCrLf
                            'cmdText += ", ABS_Initial = " & .ABS_Initial.ToSQLString()
                            cmdText += ", ABS_Initial = " & ReplaceNumericString(.ABS_Initial)
                        End If

                        If .IsABS_MainFilterNull Then
                            cmdText += ", ABS_MainFilter = NULL"
                        Else
                            'AG 10/06/2010
                            '' modified by dl 12/03/2010
                            ''cmdText += ", ABS_MainFilter = " & pResultsDS.twksResults(0).ABS_MainFilter.ToString.Replace(",", ".") & vbCrLf
                            'cmdText += ", ABS_MainFilter = " & .ABS_MainFilter.ToSQLString()
                            cmdText += ", ABS_MainFilter = " & ReplaceNumericString(.ABS_MainFilter)
                        End If

                        'AG 12/07/2010
                        If .IsAbs_WorkReagentNull Then
                            cmdText += ", ABS_WorkReagent = NULL"
                        Else
                            cmdText += ", ABS_WorkReagent = " & ReplaceNumericString(.Abs_WorkReagent)
                        End If
                        'END AG 12/07/2010


                        If .IsCalibratorFactorNull Then
                            cmdText += ", CalibratorFactor = NULL"
                        Else
                            'AG 10/06/2010
                            '' modified by dl 12/03/2010
                            ''cmdText += ", CalibratorFactor = " & pResultsDS.twksResults(0).CalibratorFactor.ToString.Replace(",", ".") & vbCrLf
                            'cmdText += ", CalibratorFactor = " & .CalibratorFactor.ToSQLString()
                            cmdText += ", CalibratorFactor = " & ReplaceNumericString(.CalibratorFactor)
                        End If

                        If .IsCalibrationErrorNull Then
                            cmdText += ", CalibrationError = NULL"
                        Else
                            cmdText += ", CalibrationError = '" & .CalibrationError & "'"
                        End If

                        If .IsCalibratorBlankAbsUsedNull Then
                            cmdText += ", CalibratorBlankAbsUsed = NULL"
                        Else
                            'AG 10/06/2010
                            '' modified by dl 12/03/2010
                            ''cmdText += ", CalibratorBlankAbsUsed = " & pResultsDS.twksResults(0).CalibratorBlankAbsUsed.ToString.Replace(",", ".") & vbCrLf
                            'cmdText += ", CalibratorBlankAbsUsed = " & .CalibratorBlankAbsUsed.ToSQLString()
                            cmdText += ", CalibratorBlankAbsUsed = " & ReplaceNumericString(.CalibratorBlankAbsUsed)
                        End If

                        If .IsCurveResultsIDNull Then
                            cmdText += ", CurveResultsID = NULL"
                        Else
                            cmdText += ", CurveResultsID = " & .CurveResultsID.ToString
                        End If

                        If .IsCurveGrowthTypeNull Then
                            cmdText += ", CurveGrowthType = NULL"
                        Else
                            cmdText += ", CurveGrowthType = '" & .CurveGrowthType & "'"
                        End If

                        If .IsCurveTypeNull Then
                            cmdText += ", CurveType = NULL"
                        Else
                            cmdText += ", CurveType = '" & .CurveType & "'"
                        End If

                        If .IsCurveAxisXTypeNull Then
                            cmdText += ", CurveAxisXType = NULL"
                        Else
                            cmdText += ", CurveAxisXType = '" & .CurveAxisXType & "'"
                        End If

                        If .IsCurveAxisYTypeNull Then
                            cmdText += ", CurveAxisYType = NULL"
                        Else
                            'AG 12/03/2010
                            'cmdText += ", CurveAxisYType = '" & pResultsDS.twksResults(0).IsCurveAxisYTypeNull.ToString & "'" & vbCrLf
                            cmdText += ", CurveAxisYType = '" & .CurveAxisYType & "'"
                            'AG 12/03/2010
                        End If

                        If .IsRelativeErrorCurveNull Then
                            cmdText += ", RelativeErrorCurve = NULL"
                        Else
                            'AG 10/06/2010
                            '' modified by dl 12/03/2010
                            ''cmdText += ", RelativeErrorCurve = " & pResultsDS.twksResults(0).RelativeErrorCurve.ToString.Replace(",", ".") & vbCrLf
                            'cmdText += ", RelativeErrorCurve = " & .RelativeErrorCurve.ToSQLString()
                            cmdText += ", RelativeErrorCurve = " & ReplaceNumericString(.RelativeErrorCurve)
                        End If

                        'AG 01/07/2011
                        If .IsCurveSlopeNull Then
                            cmdText += ", CurveSlope = NULL"
                        Else
                            cmdText += ", CurveSlope = " & ReplaceNumericString(.CurveSlope)
                        End If

                        If .IsCurveOffsetNull Then
                            cmdText += ", CurveOffset = NULL"
                        Else
                            cmdText += ", CurveOffset = " & ReplaceNumericString(.CurveOffset)
                        End If

                        If .IsCurveCorrelationNull Then
                            cmdText += ", CurveCorrelation = NULL"
                        Else
                            cmdText += ", CurveCorrelation = " & ReplaceNumericString(.CurveCorrelation)
                        End If
                        'AG 01/07/2011

                        If .IsCONC_ValueNull Then
                            cmdText += ", CONC_Value = NULL"
                        Else
                            'AG 10/06/2010
                            '' modified by dl 12/03/2010
                            ''cmdText += ", CONC_Value = " & pResultsDS.twksResults(0).CONC_Value.ToString.Replace(",", ".") & vbCrLf
                            'cmdText += ", CONC_Value = " & .CONC_Value.ToSQLString()
                            cmdText += ", CONC_Value = " & ReplaceNumericString(.CONC_Value)
                        End If

                        If .IsCONC_ErrorNull Then
                            cmdText += ", CONC_Error = NULL"
                        Else
                            cmdText += ", CONC_Error = '" & .CONC_Error & "'"
                        End If

                        If .IsResultDateTimeNull Then
                            cmdText += ", ResultDateTime = NULL"
                        Else
                            cmdText += ", ResultDateTime = '" & .ResultDateTime.ToString("yyyyMMdd HH:mm:ss") & "'"
                        End If

                        'AG 10/11/2010 - Add new fields ManualResult and ManualResultText
                        If .IsManualResultNull Then
                            cmdText += ", ManualResult = NULL"
                        Else
                            cmdText += ", ManualResult = " & ReplaceNumericString(.ManualResult)
                        End If

                        If .IsManualResultTextNull Then
                            cmdText += ", ManualResultText = NULL"
                        Else
                            cmdText += ", ManualResultText = N'" & .ManualResultText.ToString.Replace("'", "''") & "'"
                        End If

                        'END AG 10/11/2010

                        If .IsTS_UserNull Then
                            cmdText += ", TS_User = ''"
                        Else
                            cmdText += ", TS_User = '" & .TS_User & "'"
                        End If

                        If .IsTS_DateTimeNull Then 'If .IsResultDateTimeNull Then 
                            cmdText += ", TS_DateTime = '" & Now.ToString("yyyyMMdd HH:mm:ss") & "'"
                        Else
                            cmdText += ", TS_DateTime = '" & .TS_DateTime.ToString("yyyyMMdd HH:mm:ss") & "'"
                        End If

                        cmdText += " WHERE OrderTestID      = " & .OrderTestID & _
                                   "  AND RerunNumber      = " & .RerunNumber & _
                                   "  AND MultiPointNumber = " & .MultiPointNumber
                    End With

                    'AG 25/07/2014 RQ00086 - improve memory usage
                    'Dim dbCmd As New SqlCommand
                    'dbCmd.Connection = pDBConnection
                    'dbCmd.CommandText = cmdText
                    'resultData.AffectedRecords = dbCmd.ExecuteNonQuery()

                    Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                        resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                        resultData.HasError = False
                    End Using
                    'AG 25/07/2014 RQ00086

                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksResultsDAO.UpdateResult", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function
#End Region

#Region "Other Methods"
        ''' <summary>
        ''' Clears (set to NULL) the CurveResultsID field by OrderTestID and RerunNumber
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pOrderTestID">Order Test Identifier</param>
        ''' <param name="pRerunNumber">Rerun Number</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  AG 09/06/2010 (Tested pending)
        ''' </remarks>
        Public Function ClearCurveResultsID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOrderTestID As Integer, _
                                            ByVal pRerunNumber As Integer) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString

                Else
                    Dim cmdText As String = " UPDATE twksResults SET CurveResultsID = NULL " & vbCrLf & _
                                            " WHERE  OrderTestID = " & pOrderTestID.ToString & vbCrLf & _
                                            " AND    RerunNumber = " & pRerunNumber.ToString & vbCrLf

                    Using dbCmd As New SqlCommand(cmdText, pDBConnection)
                        myGlobalDataTO.AffectedRecords = dbCmd.ExecuteNonQuery()
                        myGlobalDataTO.HasError = False
                    End Using
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksResultsDAO.ClearCurveResultsID", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Delete all Calibrator Results that exist for the informed TestID/SampleType for the specified TestVersion
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestID">Identifier of an Standard Test</param>
        ''' <param name="pTestVersion">Active TestVersion Number</param>
        ''' <param name="pSampleType">Sample Type Code</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  AG 18/03/2010 (Tested pending)
        ''' Modified by: SA 20/07/2010 - Query changed. Table twksResults is not needed in the subquery used to get the Order Tests
        '''              SA 19/04/2012 - Changed the query by adding a filter by Standard Tests
        ''' </remarks>
        Public Function DeleteCalibrationResultsByTestIDSampleType(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer, _
                                                                   ByVal pTestVersion As Integer, ByVal pSampleType As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = " DELETE FROM twksResults " & vbCrLf & _
                                            " WHERE  TestVersion = " & pTestVersion.ToString & vbCrLf & _
                                            " AND    OrderTestID IN (SELECT OT.OrderTestID " & vbCrLf & _
                                                                   " FROM   twksOrderTests OT INNER JOIN twksOrders O ON OT.OrderID = O.OrderID " & vbCrLf & _
                                                                   " WHERE  O.SampleClass = 'CALIB' " & vbCrLf & _
                                                                   " AND    OT.TestType   = 'STD' " & vbCrLf & _
                                                                   " AND    OT.TestID     = " & pTestID.ToString & vbCrLf & _
                                                                   " AND    OT.SampleType = '" & pSampleType.Trim & "') " & vbCrLf

                    Using dbCmd As New SqlCommand(cmdText, pDBConnection)
                        resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                        resultData.HasError = False
                    End Using
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksResultsDAO.DeleteCalibrationResultsByTestIdSampleType", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Delete all Results that exist for all Order Tests belonging to the specified Order
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pOrderID">Order Identifier</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 25/01/2011
        ''' </remarks>
        Public Function DeleteResultsByOrderID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOrderID As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = " DELETE FROM twksResults " & vbCrLf & _
                                            " WHERE  OrderTestID IN (SELECT OrderTestID FROM twksOrderTests " & vbCrLf & _
                                                                   " WHERE OrderID = '" & pOrderID & "') " & vbCrLf

                    Using dbCmd As New SqlCommand(cmdText, pDBConnection)
                        resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                        resultData.HasError = False
                    End Using
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksResultsDAO.DeleteResultsByOrderID", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Delete all Results that exist for the specified Order Test
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pOrderTestId">Order Test Identifier</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  AG 09/03/2010 (Tested pending)
        ''' Modified by: SA 16/04/2012 - Changed the function template
        ''' </remarks>
        Public Function DeleteResultsByOrderTestID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOrderTestID As Integer) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = " DELETE FROM twksResults " & vbCrLf & _
                                            " WHERE  OrderTestID = " & pOrderTestID.ToString & vbCrLf

                    Using dbCmd As New SqlCommand(cmdText, pDBConnection)
                        resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                        resultData.HasError = False
                    End Using
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksResultsDAO.DeleteResultsByOrderTestID", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Delete all Blank and Calibrator Results that exist for the informed TestID (whatever SampleType) for the specified TestVersion
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestID">Identifier of an Standard Test</param>
        ''' <param name="pTestVersion">Active Test Version Number</param>        
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  AG 18/03/2010 (Tested pending)
        ''' Modified by: SA 20/07/2010 - Query changed. Table twksResults is not needed in the subquery used to get the Order Tests
        '''              SA 19/04/2012 - Changed the query by adding a filter by Standard Tests and also a filter by SampleClass BLANK/CALIB
        ''' </remarks>
        Public Function DeleteResultsByTestID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer, ByVal pTestVersion As Integer) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = " DELETE FROM twksResults " & vbCrLf & _
                                            " WHERE  TestVersion = " & pTestVersion.ToString & vbCrLf & _
                                            " AND    OrderTestID IN (SELECT OT.OrderTestID " & vbCrLf & _
                                                                   " FROM   twksOrderTests OT INNER JOIN twksOrders O ON OT.OrderID = O.OrderID " & vbCrLf & _
                                                                   " WHERE  O.SampleClass IN ('BLANK', 'CALIB') " & vbCrLf & _
                                                                   " AND    OT.TestType = 'STD' " & vbCrLf & _
                                                                   " AND    OT.TestID   = " & pTestID.ToString & ") " & vbCrLf

                    Using dbCmd As New SqlCommand(cmdText, pDBConnection)
                        resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                        resultData.HasError = False
                    End Using
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksResultsDAO.DeleteResultsByTestId", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Determine if an especific row exists in twksResults
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pResultDS">ResultDS with the PK fields</param>
        ''' <returns>True is exists, else false</returns>
        ''' <remarks>
        ''' Created by: GDS 2/03/2010 
        ''' Modified by: SA 21/01/2011 - Get SELECT * instead of SELECT 1
        ''' </remarks>
        Public Function ExistsResult(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pResultDS As ResultsDS) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT * FROM twksResults " & vbCrLf & _
                                                " WHERE  OrderTestID      = " & pResultDS.twksResults(0).OrderTestID.ToString & vbCrLf & _
                                                " AND    RerunNumber      = " & pResultDS.twksResults(0).RerunNumber.ToString & vbCrLf & _
                                                " AND    MultiPointNumber = " & pResultDS.twksResults(0).MultiPointNumber.ToString & vbCrLf

                        Dim existsResultsDS As New ResultsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(existsResultsDS.twksResults)
                            End Using
                        End Using

                        resultData.SetDatos = existsResultsDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksResultsDAO.ExistsResult", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get the accepted Results for the specified OrderTestID (Blank or Calibrator Order Test)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pOrderTestID">Order Test Identifier</param>
        ''' <param name="pIgnoreValidationStatus">Optional parameter; this parameter is set to TRUE to get the last calculated results 
        '''                                       without taking care of validation status </param>
        ''' <param name="pApplyCONVERT">Optional parameter; when it is set to false, return Real values without apply the CONVERT</param>
        ''' <returns>GlobalDataTO containing a typed DataSet ResultsDS with all accepted Results found 
        '''          for the informed OrderTestID</returns>
        ''' <remarks>
        ''' Created by:  SA 23/02/2010 
        ''' Modified by: AG 04/03/2010 - Return more fields in SELECT (ABS_Initial, ABS_MainFilter, CalibrationError, CalibratorBlankAbsUsed, 
        '''                              CurveResultsID, CurveGrowthType, CurveType, CurveAxisXType, CurveAxisYType, RelativeErrorCurve, 
        '''                              CONC_Value, and CONC_Error)
        '''              AG 12/03/2010 - Added optional parameter pIgnoreValidationStatus. This parameter is set to TRUE to get the last 
        '''                              calculated results without taking care of validation status.
        '''              TR 13/05/2010 - Return field OrderTestID in SELECT
        '''              TR 14/05/2010 - Return fields RerunNumber and ExportStatus in SELECT
        '''              AG 12/07/2010 - Return field  ABS_WorkReagent in SELECT
        '''              AG 10/11/2010 - Return fields ManualResultFlag, ManualResult and ManualResultText in SELECT
        '''              SA 30/11/2010 - Return fields ABSValue, CalibrationFactor and ManualResult formatted with maximum 10 digits and 
        '''                              4 decimal places 
        '''              AG 01/07/2011 - Return columns: CurveOffset. CurveSlope and CurveCorrelation
        '''              AG 18/07/2011 - For CalibratorFactor and ManualResult change DECIMAL(10,4) for DECIMAL(20,4) to avoid overflow errors
        '''              AG 25/07/2011 - Added parameter pApplyCONVERT; when it is set to false, return Real values without apply the CONVERT
        '''              DL 22/05/2012 - Added fields to query and modify sentence
        ''' </remarks>
        Public Function GetAcceptedResults(ByVal pDBConnection As SqlClient.SqlConnection, _
                                           ByVal pOrderTestID As Integer, _
                                           Optional ByVal pIgnoreValidationStatus As Boolean = False, _
                                           Optional ByVal pApplyCONVERT As Boolean = True, _
                                           Optional ByVal pTestType As String = "STD") _
                                           As GlobalDataTO

            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = ""
                        If (pApplyCONVERT) Then

                            cmdText &= "  SELECT R.OrderTestID, R.RerunNumber, R.MultiPointNumber, CONVERT(DECIMAL(10,4), R.ABSValue) AS ABSValue, R.ExportStatus, R.ABS_Initial," & vbCrLf
                            cmdText &= "  	     R.ABS_MainFilter, CONVERT(DECIMAL(20,4), R.CalibratorFactor) AS CalibratorFactor, R.CalibrationError, R.CalibratorBlankAbsUsed," & vbCrLf
                            cmdText &= "  	     R.CurveResultsID, R.CurveGrowthType, R.CurveType, R.CurveAxisXType, R.CurveAxisYType, R.RelativeErrorCurve, R.CONC_Value," & vbCrLf
                            cmdText &= "  	     R.CONC_Error, R.ResultDateTime, R.ABS_WorkReagent, R.ManualResultFlag, CONVERT(DECIMAL(20,4), R.ManualResult) AS ManualResult," & vbCrLf
                            cmdText &= "  	     R.ManualResultText, R.CurveSlope, R.CurveOffset, R.CurveCorrelation, " & vbCrLf

                        Else
                            cmdText &= "  SELECT R.OrderTestID, R.RerunNumber, R.MultiPointNumber, R.ABSValue, R.ExportStatus, R.ABS_Initial," & vbCrLf
                            cmdText &= "  	     R.ABS_MainFilter, R.CalibratorFactor, R.CalibrationError, R.CalibratorBlankAbsUsed," & vbCrLf
                            cmdText &= "  	     R.CurveResultsID, R.CurveGrowthType, R.CurveType, R.CurveAxisXType, R.CurveAxisYType, R.RelativeErrorCurve, R.CONC_Value," & vbCrLf
                            cmdText &= "  	     R.CONC_Error, R.ResultDateTime, R.ABS_WorkReagent, R.ManualResultFlag, R.ManualResult," & vbCrLf
                            cmdText &= "  	     R.ManualResultText, R.CurveSlope, R.CurveOffset, R.CurveCorrelation, "

                        End If

                        Select Case pTestType
                            Case "STD"
                                cmdText &= "  	     OT.TestType, OT.TestID, T.TestName, MD.FixedItemDesc AS MeasureUnit, O.StatFlag, O.SampleClass, OT.SampleType, OT.TubeType, O.SampleID, O.PatientID" & vbCrLf
                                cmdText &= "    FROM twksResults R INNER JOIN twksOrderTests OT ON OT.OrderTestID = R.OrderTestID " & vbCrLf
                                cmdText &= "  	                 INNER JOIN tparTests T ON OT.TestID = T.TestID " & vbCrLf
                                cmdText &= "  	                 INNER JOIN tcfgMasterData MD on MD.ItemID = T.MeasureUnit " & vbCrLf
                                cmdText &= "  	                 INNER JOIN twksOrders O ON OT.OrderID = O.OrderID" & vbCrLf
                                cmdText &= "   WHERE R.OrderTestID = " & pOrderTestID & vbCrLf
                                cmdText &= "     AND OT.TestType = 'STD'" & vbCrLf

                            Case "ISE"
                                cmdText &= "  	     OT.TestType, OT.TestID, T.Name as TestName, MD.FixedItemDesc AS MeasureUnit, O.StatFlag, O.SampleClass, OT.SampleType, OT.TubeType, O.SampleID, O.PatientID" & vbCrLf
                                cmdText &= "    FROM twksResults R INNER JOIN twksOrderTests OT ON OT.OrderTestID = R.OrderTestID " & vbCrLf
                                cmdText &= "  	                 INNER JOIN tparISETests T ON OT.TestID = T.ISETestID " & vbCrLf
                                cmdText &= "  	                 INNER JOIN tcfgMasterData MD on MD.ItemID = T.ISE_Units " & vbCrLf
                                cmdText &= "  	                 INNER JOIN twksOrders O ON OT.OrderID = O.OrderID" & vbCrLf
                                cmdText &= "   WHERE R.OrderTestID = " & pOrderTestID & vbCrLf
                                cmdText &= "     AND OT.TestType = 'ISE'" & vbCrLf

                            Case "OFFS"
                                cmdText &= "  	     OT.TestType, OT.TestID, T.Name as TestName, MD.FixedItemDesc AS MeasureUnit, O.StatFlag, O.SampleClass, OT.SampleType, OT.TubeType, O.SampleID, O.PatientID" & vbCrLf
                                cmdText &= "    FROM twksResults R INNER JOIN twksOrderTests OT ON OT.OrderTestID = R.OrderTestID " & vbCrLf
                                cmdText &= "  	                 INNER JOIN tparOffSystemTests T ON OT.TestID = T.OffSystemTestID " & vbCrLf
                                cmdText &= "  	                 INNER JOIN twksOrders O ON OT.OrderID = O.OrderID " & vbCrLf
                                cmdText &= "  	                 LEFT JOIN tcfgMasterData MD on MD.ItemID = T.Units" & vbCrLf
                                cmdText &= "   WHERE R.OrderTestID = " & pOrderTestID & vbCrLf
                                cmdText &= "     AND OT.TestType = 'OFFS'" & vbCrLf

                            Case "CALC"
                                cmdText &= "  	     OT.TestType, OT.TestID, CT.CalcTestLongName as TestName, MD.FixedItemDesc AS MeasureUnit, O.StatFlag, O.SampleClass, OT.SampleType, OT.TubeType, O.SampleID, O.PatientID" & vbCrLf
                                cmdText &= "    FROM twksResults R INNER JOIN twksOrderCalculatedTests OCT ON R.OrderTestID = OCT.CalcOrderTestID " & vbCrLf
                                cmdText &= "                       INNER JOIN twksOrderTests OT ON OCT.CalcOrderTestID = OT.OrderTestID " & vbCrLf
                                cmdText &= "                       INNER JOIN tparCalculatedTests CT ON OT.TestID = CT.CalcTestID " & vbCrLf
                                cmdText &= "                       INNER JOIN tcfgMasterData MD ON MD.SubTableID = 'TEST_UNITS' AND MD.ItemID = CT.MeasureUnit " & vbCrLf
                                cmdText &= "                       INNER JOIN twksWSOrderTests WSOT ON OT.OrderTestID = WSOT.OrderTestID " & vbCrLf
                                cmdText &= "                       twksOrders O ON OT.OrderID = O.OrderID"

                            Case "CTRL"
                                cmdText &= "  	     OT.TestType, OT.TestID, IT.Name as TestName, MD.FixedItemDesc AS MeasureUnit, O.StatFlag, O.SampleClass, OT.SampleType, OT.TubeType, O.SampleID, O.PatientID" & vbCrLf
                                cmdText &= "    FROM twksResults R INNER JOIN twksOrderTests OT ON R.OrderTestID = OT.OrderTestID " & vbCrLf
                                cmdText &= "                       INNER JOIN tparISETests IT ON OT.TestID = CT.CalcTestID " & vbCrLf
                                cmdText &= "                       INNER JOIN tcfgMasterData MD ON MD.SubTableID = 'TEST_UNITS' AND MD.ItemID = CT.MeasureUnit " & vbCrLf
                                cmdText &= "                       INNER JOIN twksWSOrderTests WSOT ON OT.OrderTestID = WSOT.OrderTestID " & vbCrLf
                                cmdText &= "                       twksOrders O ON OT.OrderID = O.OrderID"

                                'Case Else
                                '    cmdText &= "  	     OT.TestType, OT.TestID, T.TestName, MD.FixedItemDesc AS MeasureUnit, O.StatFlag, O.SampleClass, OT.SampleType, OT.TubeType, O.SampleID, O.PatientID" & vbCrLf
                                '    cmdText &= "    FROM twksResults R INNER JOIN twksOrderTests OT ON OT.OrderTestID = R.OrderTestID " & vbCrLf
                                '    cmdText &= "  	                 INNER JOIN tparTests T ON OT.TestID = T.TestID " & vbCrLf
                                '    cmdText &= "  	                 INNER JOIN tcfgMasterData MD on MD.ItemID = T.MeasureUnit " & vbCrLf
                                '    cmdText &= "  	                 INNER JOIN twksOrders O ON OT.OrderID = O.OrderID" & vbCrLf
                                '    cmdText &= "   WHERE R.OrderTestID = " & pOrderTestID & vbCrLf
                                '    cmdText &= "     AND OT.TestType = 'STD'" & vbCrLf
                        End Select

                        cmdText &= "     AND R.AcceptedResultFlag = 1" & vbCrLf

                        If (pIgnoreValidationStatus) Then
                            cmdText &= "ORDER BY R.MultiPointNumber "
                        Else
                            'Add where condition + order by
                            cmdText &= "     AND R.ValidationStatus = 'OK'" & vbCrLf
                            cmdText &= "ORDER BY R.MultiPointNumber"
                        End If

                        Dim lastResultsDS As New ResultsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(lastResultsDS.twksResults)
                            End Using
                        End Using

                        resultData.SetDatos = lastResultsDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksResultsDAO.GetAcceptedResults", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get the list of Order Tests with Calculated Test Results
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pOrderForReportFlag"></param>
        ''' <returns>GlobalDataTO indicating if an error has occurred or not. If succeed, returns an ResultsDS 
        '''          dataset with the results (view vwksCalcResults)</returns>
        ''' <remarks>
        ''' Created by:  RH 25/08/2010
        ''' Modified by: AG 01/12/2010 - Filter query by WorkSessionID instead of AnalyzerID
        ''' Modified by AG 01/08/2014 #1897 fix issue Test order for patient report (final and compact) from current WS results
        ''' </remarks>
        Public Function GetCalculatedTestResults(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
                                                 ByVal pWorkSessionID As String, ByVal pOrderForReportFlag As Boolean) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then

                        Dim cmdText As String = String.Empty
                        If Not pOrderForReportFlag Then
                            cmdText = String.Format(" SELECT * FROM vwksCalcResults " & _
                                                    " WHERE  WorkSessionID = '{0}' ", pWorkSessionID.Trim)
                        Else
                            'AG 01/08/2014 - #1897 new query appliying test order for reports
                            cmdText = String.Format(" SELECT CR.*, TS.TestPosition FROM vwksCalcResults CR INNER JOIN " & _
                                                    " tcfgReportsTestsSorting TS ON CR.TestType = TS.TestType AND CR.TestID = TS.TestID  " & _
                                                    " WHERE  WorkSessionID = N'{0}' ", pWorkSessionID.Trim) & _
                                                    " ORDER BY TS.TestPosition "
                        End If

                        Dim resultsDataDS As New ResultsDS
                        Using myCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(myCmd)
                                dbDataAdapter.Fill(resultsDataDS.vwksResults)
                            End Using
                        End Using

                        resultData.SetDatos = resultsDataDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksResultsDAO.GetCalculatedTestResults", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function


        ''' <summary>
        ''' Get all the Results for the specified Worksession and analyzer
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID" ></param>
        ''' <param name="pWorkSessionID" ></param>
        ''' <param name="pOnlineExportClause">additional clause to improve memory usage in automatic export to LIS</param>
        ''' <returns>GlobalDataTO containing a typed DataSet ResultsDS with all the Results (vwksResults) found 
        '''          for the informed AnalyzerID and WorkSessionID</returns>
        ''' <remarks>
        ''' Created by: AG 25/06/2012
        ''' AG 17/02/2014 - #1505 new parameter pOnlineExportClause
        ''' </remarks>
        Public Function GetCompleteResults(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, _
                                   Optional pOnlineExportClause As String = "") As GlobalDataTO

            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT * FROM vwksCompleteResults WHERE AnalyzerID = '" & pAnalyzerID.Trim & "' " & vbCrLf & _
                                                " AND WorkSessionID = '" & pWorkSessionID.Trim & "' " & vbCrLf

                        'AG 17/02/2014 - #1505
                        If pOnlineExportClause <> "" Then
                            cmdText &= " AND " & pOnlineExportClause
                        End If
                        'AG 17/02/2014 - #1505

                        cmdText &= " ORDER BY ResultDateTime DESC "

                        Dim lastResultsDS As New ResultsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(lastResultsDS.vwksResults)
                            End Using
                        End Using

                        resultData.SetDatos = lastResultsDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksResultsDAO.GetCompleteResults", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get all the old Blanks and Calibrators 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTO containing a typed DataSet ResultsDS with data of Blanks and Calibrators in twksResults</returns>
        ''' <remarks>
        ''' Created by:  JB 09/10/2012
        ''' </remarks>
        Public Function GetOldBlankCalibToDelete(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing
            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String
                        cmdText = "SELECT O.SampleClass, OT.AnalyzerID, OT.TestID, OT.SampleType, R.TestVersion, COUNT(*) " & _
                                  "  FROM twksResults R " & _
                                  "      INNER JOIN twksOrderTests OT ON R.AnalyzerID = OT.AnalyzerID AND R.OrderTestID = OT.OrderTestID " & _
                                  "      INNER JOIN twksOrders O ON OT.OrderID = O.OrderID " & _
                                  "  WHERE O.SampleClass IN ('BLANK', 'CALIB') " & _
                                  "  GROUP BY OT.AnalyzerID, O.SampleClass, OT.TestID, OT.SampleType, R.TestVersion " & _
                                  "  HAVING COUNT(*)>1" & _
                                  "  ORDER BY O.SampleClass"

                        Dim lastResultDS As New ResultsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(lastResultDS.twksResults)
                            End Using
                        End Using

                        resultData.SetDatos = lastResultDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksResultsDAO.GetOldBlankCalibToDelete", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function


        ''' <summary>
        ''' If exists an accepted valid result for a Blank executed in the informed Analyzer with the same version that the current 
        ''' Test Version and optionally, inside the allowed period of time, this function returns the ABS Value of the Blank
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestID">Test Identifier</param>
        ''' <param name="pCurrentTestVersion">Current Version of the informed Test</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier. Optional parameter</param>
        ''' <param name="pMaxDaysLastBlank">Maximum number of days that can have passed from the last Blank execution for the informed Test. 
        '''                                 It is an optional parameter</param>
        ''' <param name="pApplyCONVERT">Optional parameter; when it is set to false, return Real values without apply the CONVERT</param>
        ''' <param name="pReturnAllBlanks" ></param>
        ''' <param name="pIgnoreOrderTestStatus"></param>
        ''' <returns>GlobalDataTO containing a typed DataSet WSAdditionalElementsDS with the result of the last executed 
        '''          Blank for the informed Test</returns>
        ''' <remarks>
        ''' Created by:  SA
        ''' Modified botherwisey: SA 08/01/2010    - Changes to return a GlobalDataTO instead a typed DataSet WSAdditionalElementsDS. Changes
        '''                                 to implement the open of a DB Connection according the new template. 
        '''              AG 09/03/2010    - AnalyzerID is an optional parameter
        '''              AG/SA 12/05/2010 - Change ACCEPTED by CLOSED
        '''              SA 30/11/2010    - Return field ABSValue formatted with maximum 10 digits and 4 decimal places 
        '''              AG 25/07/2011    - Added parameter pApplyCONVERT; when it is set to false, return Real values without apply the CONVERT
        '''              JB 09/10/2012    - Added parameter pReturnAllBlanks: when it is set to true, returns all; otherwise returns only the last one
        '''              AG 19/11/2012 - add optional parameter pIgnoreOrderTestStatus (it will be informed as TRUE when called from calculations class)
        ''' </remarks>
        Public Function GetLastExecutedBlank(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer, ByVal pCurrentTestVersion As Integer, _
                                             Optional ByVal pAnalyzerID As String = "", Optional ByVal pMaxDaysLastBlank As String = "", _
                                             Optional ByVal pApplyCONVERT As Boolean = True, _
                                             Optional ByVal pReturnAllBlanks As Boolean = False, _
                                             Optional ByVal pIgnoreOrderTestStatus As Boolean = False) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing
            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = "SELECT "

                        If Not pReturnAllBlanks Then cmdText &= " TOP 1 "

                        If (pApplyCONVERT) Then
                            cmdText &= " R.OrderTestID AS PreviousOrderTestID, CONVERT(DECIMAL(10,4), R.ABSValue) AS ABSValue, "
                        Else
                            cmdText &= " R.OrderTestID AS PreviousOrderTestID, R.ABSValue, "
                        End If
                        cmdText &= "      R.ResultDateTime, OT.TestID, O.SampleClass " & vbCrLf & _
                                   " FROM twksResults R INNER JOIN twksOrderTests OT ON R.OrderTestID = OT.OrderTestID " & vbCrLf & _
                                                      " INNER JOIN twksOrders O ON OT.OrderID = O.OrderID " & vbCrLf & _
                                   " WHERE  R.ValidationStatus   = 'OK' " & vbCrLf & _
                                   " AND    R.AcceptedResultFlag = 1 " & vbCrLf & _
                                   " AND    R.TestVersion        = " & pCurrentTestVersion & vbCrLf & _
                                   " AND    O.SampleClass        = 'BLANK' " & vbCrLf & _
                                   " AND    OT.TestType          = 'STD' " & vbCrLf & _
                                   " AND    OT.TestID            = " & pTestID & vbCrLf

                        'AG 19/11/2012 - this AND condition was evaluated always. When called from calculations the orderteststatus must be ignored
                        '                because the blank ordertest can be INPROCESS
                        If Not pIgnoreOrderTestStatus Then
                            cmdText &= " AND    OT.OrderTestStatus   = 'CLOSED' " & vbCrLf
                        End If

                        If (String.Compare(pAnalyzerID, "", False) <> 0) Then cmdText &= " AND OT.AnalyzerID = '" & pAnalyzerID & "' " & vbCrLf
                        If (String.Compare(pMaxDaysLastBlank, "", False) <> 0) Then cmdText &= " AND Datediff(d, R.ResultDateTime, getDate()) <= " & pMaxDaysLastBlank & vbCrLf
                        cmdText &= " ORDER BY R.ResultDateTime DESC "

                        Dim lastResultDS As New WSAdditionalElementsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(lastResultDS.WSAdditionalElementsTable)
                            End Using
                        End Using

                        resultData.SetDatos = lastResultDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksResultsDAO.GetLastExecutedBlank", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' If exists an accepted valid result for an Experimental Calibrator executed in the informed Analyzer with the same version 
        ''' that the current Test Version and optionally, inside the allowed period of time, this function returns the ABS Value and 
        ''' Calibrator Factor of each Calibrator point 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestID">Test Identifier</param>
        ''' <param name="pSampleType">Sample Type</param>
        ''' <param name="pCurrentTestVersion">Current Version of the informed Test</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier. Optional parameter</param>
        ''' <param name="pMaxDaysLastCalib">Maximum number of days that can have passed from the last Calibrator execution for the 
        '''                                 informed TestID/SampleType. It is an optional parameter</param>
        ''' <param name="pApplyCONVERT">Optional parameter; when it is set to false, return Real values without apply the CONVERT</param>
        ''' <param name="pIgnoreOrderTestStatus"></param>
        ''' <returns>GlobalDataTO containing a typed DataSet WSAdditionalElementsDS with the results of the last executed 
        '''          Calibrator for the informed TestID/SampleType</returns>
        ''' <remarks>
        ''' Created by:  SA
        ''' Modified by: SA 08/01/2010    - Changes to return a GlobalDataTO instead a typed DataSet WSAdditionalElementsDS. Changes
        '''                                 to implement the open of a DB Connection according the new template. 
        '''              AG 09/03/2010    - AnalyzerID is an optional parameter
        '''              AG/SA 12/05/2010 - Change ACCEPTED by CLOSED
        '''              AG 15/08/2010    - Order by ResultDate DESC and MultiPointNumber
        '''              AG 09/11/2010    - Select also R.ManualResultFlag, R.ManualResult and R.ManualResultText
        '''              SA 30/11/2010    - Return fields ABSValue, CalibrationFactor and ManualResult formatted with maximum 10 digits 
        '''                                 and 4 decimal places 
        '''              AG 18/07/2011    - For CalibratorFactor and ManualResult change DECIMAL(10,4) for DECIMAL(20,4) to avoid overflow errors
        '''              AG 25/07/2011    - Added parameter pApplyCONVERT; when it is set to false, return Real values (calculations) without apply the CONVERT (presentation)
        '''              AG 15/05/2012    - When pApplyCONVERT is FALSE do not filter by ValidationStatus (get the last result, it does not matter if OK or NOTCALC
        '''              AG 19/11/2012 - add optional parameter pIgnoreOrderTestStatus (it will be informed as TRUE when called from calculations class)
        ''' </remarks>
        Public Function GetLastExecutedCalibrator(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer, ByVal pSampleType As String, _
                                                  ByVal pCurrentTestVersion As Integer, Optional ByVal pAnalyzerID As String = "", _
                                                  Optional ByVal pMaxDaysLastCalib As String = "", Optional ByVal pApplyCONVERT As Boolean = True, _
                                                  Optional ByVal pIgnoreOrderTestStatus As Boolean = False) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing
            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String
                        If (pApplyCONVERT) Then
                            cmdText = " SELECT DISTINCT R.OrderTestID AS PreviousOrderTestID, R.MultiPointNumber, CONVERT(DECIMAL(10,4), R.ABSValue) AS ABSValue, " & vbCrLf & _
                                                      " CONVERT(DECIMAL(20,4), R.CalibratorFactor) AS CalibratorFactor, R.ResultDateTime, OT.TestID, OT.SampleType, " & vbCrLf & _
                                                      " O.SampleClass, R.ManualResultFlag, CONVERT(DECIMAL(20,4), R.ManualResult) AS ManualResult, R.ManualResultText " & vbCrLf & _
                                      " FROM twksResults R INNER JOIN twksOrderTests OT ON R.OrderTestID = OT.OrderTestID " & vbCrLf & _
                                                         " INNER JOIN twksOrders O ON OT.OrderID = O.OrderID " & vbCrLf & _
                                      " WHERE  R.ValidationStatus   = 'OK' " & vbCrLf & _
                                      " AND    R.AcceptedResultFlag = 1 " & vbCrLf & _
                                      " AND    R.TestVersion        = " & pCurrentTestVersion & vbCrLf & _
                                      " AND    O.SampleClass        = 'CALIB' " & vbCrLf & _
                                      " AND    OT.TestType          = 'STD' " & vbCrLf & _
                                      " AND    OT.TestID            = " & pTestID & vbCrLf & _
                                      " AND    OT.SampleType        = '" & pSampleType.Trim & "' " & vbCrLf
                        Else
                            cmdText = " SELECT DISTINCT R.OrderTestID AS PreviousOrderTestID, R.MultiPointNumber, R.ABSValue, R.CalibratorFactor, R.ResultDateTime, " & vbCrLf & _
                                                      " OT.TestID, OT.SampleType, O.SampleClass, R.ManualResultFlag, R.ManualResult, R.ManualResultText " & vbCrLf & _
                                      " FROM twksResults R INNER JOIN twksOrderTests OT ON R.OrderTestID = OT.OrderTestID " & vbCrLf & _
                                                         " INNER JOIN twksOrders O ON OT.OrderID = O.OrderID " & vbCrLf

                            'AG 15/05/2012 - If called for calculation do not filter by ValidationStatus, get the last calibrator accepted executed
                            'cmdText &= " WHERE  R.ValidationStatus   = 'OK' " & vbCrLf & _
                            '          " AND    R.AcceptedResultFlag = 1 " & vbCrLf & _
                            cmdText &= " WHERE  R.AcceptedResultFlag = 1 " & vbCrLf & _
                                        " AND    R.TestVersion        = " & pCurrentTestVersion & vbCrLf & _
                                        " AND    O.SampleClass        = 'CALIB' " & vbCrLf & _
                                        " AND    OT.TestType          = 'STD' " & vbCrLf & _
                                        " AND    OT.TestID            = " & pTestID & vbCrLf & _
                                        " AND    OT.SampleType        = '" & pSampleType.Trim & "' " & vbCrLf
                        End If

                        'AG 19/11/2012 - this AND condition was evaluated always. When called from calculations the orderteststatus must be ignored
                        '                because the blank ordertest can be INPROCESS
                        If Not pIgnoreOrderTestStatus Then
                            cmdText &= " AND    OT.OrderTestStatus   = 'CLOSED' " & vbCrLf
                        End If

                        If (String.Compare(pAnalyzerID, "", False) <> 0) Then cmdText &= " AND OT.AnalyzerID = '" & pAnalyzerID & "' " & vbCrLf
                        If (String.Compare(pMaxDaysLastCalib, "", False) <> 0) Then cmdText &= " AND Datediff(d, R.ResultDateTime, getDate()) <= " & pMaxDaysLastCalib & vbCrLf
                        cmdText &= " ORDER BY R.ResultDateTime DESC, R.MultiPointNumber "

                        Dim lastResultDS As New WSAdditionalElementsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(lastResultDS.WSAdditionalElementsTable)
                            End Using
                        End Using

                        resultData.SetDatos = lastResultDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksResultsDAO.GetLastExecutedCalibrator", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get the list of Order Tests with ISE and OffSystem Test Results
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pOrderForReportFlag"></param>
        ''' <returns>GlobalDataTO containing a typed DataSet ResultsDS with all ISE and OFF-SYSTEM Tests for the WorkSession</returns>
        ''' <remarks>
        ''' Created by: AG 01/12/2010 - (copied and adapted from GetCalculatedTestResults)
        '''                             Filter query by WorkSessionId instead of AnalyzerID
        ''' Modified by AG 01/08/2014 #1897 fix issue Test order for patient report (final and compact) from current WS results
        ''' </remarks>
        Public Function GetISEOFFSTestResults(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
                                              ByVal pWorkSessionID As String, ByVal pOrderForReportFlag As Boolean) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = String.Empty
                        If Not pOrderForReportFlag Then
                            cmdText = String.Format(" SELECT * FROM vwksWSISEOffSystemResults " & _
                                                    " WHERE  WorkSessionID = '{0}' ", pWorkSessionID.Trim)

                        Else
                            'AG 01/08/2014 - #1897 new query appliying test order for reports
                            cmdText = String.Format(" SELECT IOR.*, TS.TestPosition FROM vwksWSISEOffSystemResults IOR INNER JOIN " & _
                                                    " tcfgReportsTestsSorting TS ON IOR.TestType = TS.TestType AND IOR.TestID = TS.TestID  " & _
                                                    " WHERE  WorkSessionID = N'{0}' ", pWorkSessionID.Trim) & _
                                                    " ORDER BY TS.TestPosition "
                        End If

                        Dim resultsDataDS As New ResultsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(resultsDataDS.vwksResults)
                            End Using
                        End Using

                        resultData.SetDatos = resultsDataDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksResultsDAO.GetISEOFFSTestResults", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get all the Results for the specified OrderTestID (Blank or Calibrator Order Test)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pOrderTestID">Order Test Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet ResultsDS with all the Results (vwksResults) found 
        '''          for the informed OrderTestID</returns>
        ''' <remarks>
        ''' Created by: RH 19/07/2010 
        ''' </remarks>
        Public Function GetResults(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOrderTestID As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT * FROM vwksResults WHERE OrderTestID = " & pOrderTestID

                        Dim lastResultsDS As New ResultsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(lastResultsDS.vwksResults)
                            End Using
                        End Using

                        resultData.SetDatos = lastResultsDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksResultsDAO.GetResults", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get all the Result Alarms (Blank or Calibrator Order Test)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pOnlineExportClause"></param>
        ''' <returns>GlobalDataTO containing a typed DataSet ResultsDS with all the Result Alarms (vwksResultsAlarms)</returns>
        ''' <remarks>
        ''' Created by: RH 07/19/2010 
        ''' modified by: TR 06/06/2012 -Add the AlarmID Column
        ''' AG 17/02/2014 - #1505 new parameter pOnlineExportClause
        ''' </remarks>
        Public Function GetResultAlarms(ByVal pDBConnection As SqlClient.SqlConnection, Optional pOnlineExportClause As String = "") As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT OrderTestID, RerunNumber, MultiPointNumber, AcceptedResultFlag, AlarmID, Description " & vbCrLf & _
                                                " FROM   vwksResultsAlarms"

                        'AG 17/02/2014 - #1505
                        If pOnlineExportClause <> "" Then
                            cmdText &= " WHERE " & pOnlineExportClause
                        End If
                        'AG 17/02/2014 - #1505

                        Dim lastResultsDS As New ResultsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(lastResultsDS.vwksResultsAlarms)
                            End Using
                        End Using

                        resultData.SetDatos = lastResultsDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksResultsDAO.GetResultAlarms", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get Get all the Results for Several OrderTestID 
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pOrderTestID">List of OrderTestIS separated by comma</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' CREATED BY: TR 09/07/2012
        ''' Modified by XB+JC+TR 02/10/2103 - Preserve the sorting configured on Reports Tests Sorting screen #1309 Bugs tracking
        ''' Modified by AG 01/08/2014 #1897 fix issue Test order for patient report (final and compact) from current WS results
        ''' </remarks>
        Public Function GetResultsForReport(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOrderTestID As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'AG 01/08/2014 #1897 also return TS.TestPosition
                        Dim cmdText As String = " SELECT *, TS.TestPosition FROM vwksResults R INNER JOIN tcfgReportsTestsSorting TS ON TS.TestType = R.TestType AND TS.TestID = R.TestID "
                        cmdText &= " WHERE OrderTestID IN(" & pOrderTestID & ")"
                        cmdText &= " ORDER BY TS.TestPosition "
                        '  XB+TR 02/10/2103
                        Dim lastResultsDS As New ResultsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(lastResultsDS.vwksResults)
                            End Using
                        End Using

                        resultData.SetDatos = lastResultsDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksResultsDAO.GetResults", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get Patient and Control Results to export (according the configured Export Frequency)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">WorkSession Identifier</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pSampleClass">SampleClass code. Optional parameter; when informed, value is CTRL or PATIENT</param>
        ''' <param name="pOrderID">Order Identifier. Optional parameter; when informed, only results of the OrderTests included in the Order
        '''                        are returned</param>
        ''' <param name="pOrderTestID">Order Test Identifier. Optional parameter; when informed, only results of the specified OrderTest
        '''                            are returned</param>
        ''' <returns>GlobalDataTO containing a typed DataSet ResultsDS (subtable vwksResults) with the group of Results to export</returns>
        '''  <remarks>
        ''' Created by:  TR 13/07/212
        ''' Modified by: SA 01/08/2012 - Get also fields for manual results: ManualResultFlag, ManualResult, ManualResultText
        '''              TR 28/08/2012 - Get also fields ControlName and LotNumber 
        '''              SA 29/10/2012 - Changed the SQL Query by adding a filter to get validated AND ACCEPTED Results
        '''              SG 10/04/2012 - add parameter "pIncludeSentResults". When this parameter value is TRUE do not take into account 
        '''              the condition "R.ExportStatus != 'SENT'"
        ''' </remarks>
        Public Function GetResultsToExport(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, ByVal pAnalyzerID As String, _
                                           Optional ByVal pSampleClass As String = "", Optional ByVal pOrderID As String = "", _
                                           Optional ByVal pOrderTestID As Integer = -1, _
                                           Optional pIncludeSentResults As Boolean = False) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As New StringBuilder
                        cmdText.Append("SELECT R.OrderTestID, R.RerunNumber, R.MultiPointNumber, ")
                        cmdText.Append("(CASE WHEN OT.SampleClass = 'CTRL' THEN 'Q' ")
                        cmdText.Append("      WHEN OT.SampleClass = 'PATIENT' AND OT.StatFlag = 0 THEN 'N' ")
                        cmdText.Append("      ELSE 'U' END) AS SampleClass, ")
                        cmdText.Append("(CASE WHEN OT.SampleClass = 'PATIENT' AND OT.SampleID IS NOT NULL THEN OT.SampleID ")
                        cmdText.Append("      WHEN OT.SampleClass = 'PATIENT' AND OT.PatientID IS NOT NULL THEN OT.PatientID ")
                        cmdText.Append("      ELSE NULL END) AS PatientID,")
                        cmdText.Append(" OT.TestType, OT.TestName, OT.SampleType, OT.TubeType, MD.FixedItemDesc AS MeasureUnit, ")
                        cmdText.Append(" R.CONC_Value, R.ResultDateTime, R.ManualResultFlag, R.ManualResult, R.ManualResultText, ")
                        cmdText.Append(" OT.ControlName, OT.LotNumber ")
                        cmdText.Append("FROM twksResults R INNER JOIN vwksWSOrderTests OT ON R.OrderTestID = OT.OrderTestID AND R.AnalyzerID = OT.AnalyzerID ")
                        cmdText.Append("                   LEFT OUTER JOIN tcfgMasterData MD ON OT.MeasureUnit = MD.ItemID AND MD.SubTableID = 'TEST_UNITS' ")
                        cmdText.Append("WHERE OT.AnalyzerID = '" & pAnalyzerID & "' ")
                        cmdText.Append("AND   OT.WorkSessionID = '" & pWorkSessionID & "' ")
                        cmdText.Append("AND   R.ValidationStatus = 'OK' ")
                        cmdText.Append("AND   R.AcceptedResultFlag = 1 ")
                        cmdText.Append("AND   OT.OrderToExport = 1 ")
                        cmdText.Append("AND   OT.OrderTestStatus = 'CLOSED' ")

                        'SGM 10/04/2013
                        If Not pIncludeSentResults Then
                            cmdText.Append("AND   R.ExportStatus <> 'SENT' ")
                        End If

                        If (String.Compare(pSampleClass, String.Empty, False) = 0) Then
                            cmdText.Append("AND OT.SampleClass  IN ('PATIENT', 'CTRL') ")
                        Else
                            cmdText.Append("AND OT.SampleClass = '" & pSampleClass.Trim & "' ")
                        End If

                        'Validate if the order id is informed to filter.
                        If (String.Compare(pOrderID, String.Empty, False) <> 0) Then
                            cmdText.Append("AND OT.OrderID ='" & pOrderID & "' ")

                        ElseIf (pOrderTestID > 0) Then
                            cmdText.Append("AND OT.OrderTestID =" & pOrderTestID & " ")
                        End If

                        cmdText.Append("ORDER BY OT.SampleClass, OT.PatientID, OT.SampleID")

                        Dim myResultsDS As New ResultsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText.ToString(), dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myResultsDS.vwksResults)
                            End Using
                        End Using

                        resultData.SetDatos = myResultsDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksResultsDAO.GetResultsToExport", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get Patient and Control Results to export (those related with a new Blank or Calibrator result)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">WorkSession Identifier</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pTestType">Test Type code</param>
        ''' <param name="pTestID">Test Identifier</param>
        ''' <param name="pSampleType">Sample Type code. Optional parameter, not informed when the method is searching Patient 
        '''                           or Control Results related with a new Blank Result</param>
        ''' <returns>GlobalDataTO containing a typed DataSet ResultsDS (subtable vwksResults) with the group of Results to export</returns>
        ''' <remarks>
        ''' Created by:  TR 13/07/212
        ''' Modified by: SA 01/08/2012 - Function name changed to remove overload of function GetResultsToExport; get also fields for manual
        '''                              results: ManualResultFlag, ManualResult, ManualResultText
        '''              SA 29/10/2012 - Changed the SQL Query by adding a filter to get validated AND ACCEPTED Results
        ''' </remarks>
        Public Function GetResultsToExportForBLANKAndCALIB(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, ByVal pAnalyzerID As String, _
                                                           ByVal pTestType As String, ByVal pTestID As Integer, Optional ByVal pSampleType As String = "") As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As New StringBuilder
                        cmdText.Append("SELECT OrderTestID, RerunNumber, MultiPointNumber, ")
                        cmdText.Append(" (CASE WHEN SampleClass = 'CTRL' THEN 'Q' ")
                        cmdText.Append("       WHEN SampleClass = 'PATIENT' AND StatFlag = 0 THEN 'N' ")
                        cmdText.Append("       ELSE 'U' END) AS SampleClass, SampleID, PatientID,")
                        cmdText.Append(" (CASE WHEN ManualResultFlag = 0 THEN CONC_VALUE  ")
                        cmdText.Append("       WHEN ManualResultFlag = 1 AND ManualResult IS NOT NULL THEN ManualResult ")
                        cmdText.Append("       ELSE NULL END) AS CONC_VALUE, ManualResultText, ")
                        cmdText.Append("SampleType, TubeType, TestName, TestType, MeasureUnit, ResultDateTime, ")
                        cmdText.Append("ManualResultFlag, ManualResult, ManualResultText ")
                        cmdText.Append("FROM vwksResults ")
                        cmdText.Append("WHERE AnalyzerID = N'" & pAnalyzerID.Trim.Replace("'", "''") & "' ")
                        cmdText.Append("AND   WorkSessionID = '" & pWorkSessionID.Trim & "' ")
                        cmdText.Append("AND   ValidationStatus = 'OK' ")
                        cmdText.Append("AND   AcceptedResultFlag = 1 ")
                        cmdText.Append("AND   OrderTestStatus = 'CLOSED' ")
                        cmdText.Append("AND   ExportStatus <> 'SENT' ")
                        cmdText.Append("AND   SampleClass  IN ('PATIENT', 'CTRL') ")
                        cmdText.Append("AND   TestType = '" & pTestType.Trim & "' ")
                        cmdText.Append("AND   TestID = " & pTestID.ToString & " ")

                        If (String.Compare(pSampleType, String.Empty, False) <> 0) Then
                            'SampleType is always informed excepting when the method is searching Patient or Control Results related with a new Blank Result
                            cmdText.Append("AND SampleType = '" & pSampleType & "' ")
                        End If

                        cmdText.Append("ORDER BY SampleClass, PatientID, SampleID")

                        Dim myResultsDS As New ResultsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText.ToString(), dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myResultsDS.vwksResults)
                            End Using
                        End Using

                        resultData.SetDatos = myResultsDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksResultsDAO.GetResultsToExportForBLANKAndCALIB", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get all accepted and validated Results for Blanks, Calibrators and Patient Samples requested in the specified WS Analyzer
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet ResultsDS with accepted and validated Results for Blanks, Calibrators 
        '''          and Patient Samples</returns>
        ''' <remarks>
        ''' Created by:  SA 21/02/2012
        ''' Modified by: SA 19/06/2012 - Query changed
        '''              TR 19/06/2012 - Changed R.ABS_Value by R.ABSValue
        '''              JB 05/10/2012 - Added field ValidationStatus in the select
        '''              JB 15/10/2012 - Added field CurveResultsID in the select
        '''              SA 17/10/2012 - Added field ExportStatus in the select
        '''              SA 22/10/2012 - Added fields ABS_Initial, ABS_WorkReagent and ABS_MainFilter in the select 
        '''              SA 25/10/2012 - Added fields CurveType, CurveGrowthType, CurveAxisXType and CurveAxisYType in the select
        '''              AG 17/04/2013 - Added fields LISMessageID, LISRequest, ExternalQC
        ''' </remarks>
        Public Function ReadWSAcceptedResults(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, ByVal pAnalyzerID As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing
            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT R.OrderTestID, R.RerunNumber, R.MultiPointNumber, R.ResultDateTime, R.ABSValue, R.CONC_Value, " & vbCrLf & _
                                                       " R.ManualResultFlag, R.ManualResult, R.ManualResultText, R.UserComment, R.CalibratorFactor, " & vbCrLf & _
                                                       " R.CalibratorBlankAbsUsed, R.CurveSlope, R.CurveOffset, R.CurveCorrelation, R.RelativeErrorCurve, " & vbCrLf & _
                                                       " R.ValidationStatus, R.CurveResultsID, R.ExportStatus, R.ABS_Initial, R.ABS_WorkReagent, R.ABS_MainFilter, " & vbCrLf & _
                                                       " R.CurveType, R.CurveGrowthType, R.CurveAxisXType, R.CurveAxisYType, " & vbCrLf & _
                                                       " O.SampleClass, O.OrderID, O.StatFlag, O.OrderDateTime, O.PatientID, O.SampleID, WSOT.WorkSessionID, " & vbCrLf & _
                                                       " OT.AnalyzerID, OT.OrderTestID, OT.TestType, OT.TestID, OT.SampleType, OT.ReplicatesNumber, " & vbCrLf & _
                                                       "(CASE WHEN OT.TestType = 'STD'  THEN (SELECT T.ActiveRangeType FROM tparTestSamples T " & vbCrLf & _
                                                                                            " WHERE  T.TestID = OT.TestID AND T.SampleType = OT.SampleType) " & vbCrLf & _
                                                            " WHEN OT.TestType = 'ISE'  THEN (SELECT T.ActiveRangeType FROM tparISETestSamples T " & vbCrLf & _
                                                                                            " WHERE  T.ISETestID = OT.TestID AND T.SampleType = OT.SampleType) " & vbCrLf & _
                                                            " WHEN OT.TestType = 'CALC' THEN (SELECT T.ActiveRangeType FROM tparCalculatedTests T " & vbCrLf & _
                                                                                            " WHERE T.CalcTestID = OT.TestID) " & vbCrLf & _
                                                            " WHEN OT.TestType = 'OFFS' THEN (SELECT T.ActiveRangeType FROM tparOffSystemTestSamples T " & vbCrLf & _
                                                                                            " WHERE T.OffSystemTestID = OT.TestID AND T.SampleType = OT.SampleType) " & vbCrLf & _
                                                       "  END) AS ActiveRangeType " & vbCrLf & _
                                                       " , R.LISMessageID, OT.LISRequest, OT.ExternalQC " & vbCrLf & _
                                                " FROM   twksResults R INNER JOIN twksOrderTests OT     ON R.OrderTestID  = OT.OrderTestID " & vbCrLf & _
                                                                     " INNER JOIN twksOrders O          ON OT.OrderID     = O.OrderID " & vbCrLf & _
                                                                     " INNER JOIN twksWSOrderTests WSOT ON OT.OrderTestID = WSOT.OrderTestID " & vbCrLf & _
                                                " WHERE O.SampleClass       <> 'CTRL' " & vbCrLf & _
                                                " AND   WSOT.WorkSessionID   = '" & pWorkSessionID.Trim & "' " & vbCrLf & _
                                                " AND   OT.AnalyzerID        = '" & pAnalyzerID.Trim & "' " & vbCrLf & _
                                                " AND   R.ValidationStatus   = 'OK' " & vbCrLf & _
                                                " AND   R.AcceptedResultFlag = 1 " & vbCrLf & _
                                                " AND   OT.OrderTestStatus   = 'CLOSED' " & vbCrLf & _
                                                " UNION " & vbCrLf & _
                                                " SELECT R.OrderTestID, R.RerunNumber, R.MultiPointNumber, R.ResultDateTime, R.ABSValue, R.CONC_Value, " & vbCrLf & _
                                                       " R.ManualResultFlag, R.ManualResult, R.ManualResultText, R.UserComment, R.CalibratorFactor, " & vbCrLf & _
                                                       " R.CalibratorBlankAbsUsed, R.CurveSlope, R.CurveOffset, R.CurveCorrelation, R.RelativeErrorCurve, " & vbCrLf & _
                                                       " R.ValidationStatus, R.CurveResultsID, R.ExportStatus, R.ABS_Initial, R.ABS_WorkReagent, R.ABS_MainFilter, " & vbCrLf & _
                                                       " R.CurveType, R.CurveGrowthType, R.CurveAxisXType, R.CurveAxisYType, " & vbCrLf & _
                                                       " O.SampleClass, O.OrderID, O.StatFlag, O.OrderDateTime, O.PatientID, O.SampleID, WSOT.WorkSessionID, " & vbCrLf & _
                                                       " OT.AnalyzerID, OT.OrderTestID, OT.TestType, OT.TestID, OT.SampleType, OT.ReplicatesNumber, T.ActiveRangeType " & vbCrLf & _
                                                       " , R.LISMessageID, OT.LISRequest, OT.ExternalQC " & vbCrLf & _
                                                " FROM   twksResults R INNER JOIN twksOrderTests OT      ON R.OrderTestID  = OT.OrderTestID " & vbCrLf & _
                                                                     " INNER JOIN twksOrders O           ON OT.OrderID     = O.OrderID " & vbCrLf & _
                                                                     " INNER JOIN twksWSOrderTests WSOT  ON OT.OrderTestID = WSOT.OrderTestID " & vbCrLf & _
                                                                     " INNER JOIN tparTestSamples T      ON T.TestID = OT.TestID AND T.SampleType = OT.SampleType " & vbCrLf & _
                                                                     " INNER JOIN tparTestCalibrators TC ON TC.TestID = T.TestID AND TC.SampleType = T.SampleType " & vbCrLf & _
                                                                     " INNER JOIN tparCalibrators C      ON C.CalibratorID = TC.CalibratorID " & vbCrLf & _
                                                " WHERE O.SampleClass        = 'CALIB' " & vbCrLf & _
                                                " AND   WSOT.WorkSessionID   = '" & pWorkSessionID.Trim & "' " & vbCrLf & _
                                                " AND   OT.AnalyzerID        = '" & pAnalyzerID.Trim & "' " & vbCrLf & _
                                                " AND   R.ValidationStatus   = 'NOTCALC' " & vbCrLf & _
                                                " AND   R.ManualResultFlag   = 1 " & vbCrLf & _
                                                " AND   R.ManualResult IS NOT NULL " & vbCrLf & _
                                                " AND   R.AcceptedResultFlag = 1 " & vbCrLf & _
                                                " AND   OT.OrderTestStatus   = 'CLOSED' " & vbCrLf & _
                                                " AND   OT.TestType          = 'STD' " & vbCrLf & _
                                                " AND   C.NumberOfCalibrators = 1 " & vbCrLf

                        Dim myResultsDS As New ResultsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myResultsDS.vwksResults)
                            End Using
                        End Using

                        resultData.SetDatos = myResultsDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksResultsDAO.ReadWSAcceptedResults", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get all accepted and  validated QC Results in the specified WS Analyzer
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet ResultsDS with accepted and validated QC Results</returns>
        ''' <remarks>
        ''' Created by:  SA 20/07/2011
        ''' </remarks>
        Public Function ReadWSControlResults(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, ByVal pAnalyzerID As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing
            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT R.* FROM twksResults R INNER JOIN twksOrderTests OT ON R.OrderTestID = OT.OrderTestID " & vbCrLf & _
                                                                              " INNER JOIN twksWSOrderTests WSOT ON OT.OrderTestID = WSOT.OrderTestID " & vbCrLf & _
                                                                              " INNER JOIN twksOrders O ON OT.OrderID = O.OrderID " & vbCrLf & _
                                                " WHERE R.ValidationStatus   = 'OK' " & vbCrLf & _
                                                " AND   R.AcceptedResultFlag = 1 " & vbCrLf & _
                                                " AND   OT.OrderTestStatus   = 'CLOSED' " & vbCrLf & _
                                                " AND   WSOT.WorkSessionID   = '" & pWorkSessionID & "' " & vbCrLf & _
                                                " AND   OT.AnalyzerID        = '" & pAnalyzerID & "' " & vbCrLf & _
                                                " AND   O.SampleClass        = 'CTRL' " & vbCrLf & _
                                                " ORDER BY R.OrderTestID, R.ResultDateTime "

                        Dim myResultsDS As New ResultsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myResultsDS.twksResults)
                            End Using
                        End Using

                        resultData.SetDatos = myResultsDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksResultsDAO.ReadWSCResults", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Reset (set to False) the accepted result flag for the result with the informed OrderTestID and RerunNumber different of the informed one
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pOrderTestID">Order Test Identifier</param>
        ''' <param name="pRerunNumber">Rerun Number</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  AG 03/03/2010 
        ''' Modified by: SA 16/07/2012 - Added optional parameters for the Analyzer and WorkSession
        ''' </remarks>
        Public Function ResetAcceptedResultFlag(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOrderTestId As Integer, ByVal pRerunNumber As Integer, _
                                                Optional ByVal pAnalyzerID As String = "", Optional ByVal pWorkSessionID As String = "") As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = " UPDATE twksResults SET AcceptedResultFlag = 0 " & vbCrLf & _
                                            " WHERE  OrderTestID = " & pOrderTestId.ToString & vbCrLf & _
                                            " AND    RerunNumber <> " & pRerunNumber.ToString & vbCrLf

                    If (String.Compare(pAnalyzerID.Trim, String.Empty, False) <> 0) Then cmdText &= " AND AnalyzerID = N'" & pAnalyzerID.Trim.Replace("'", "''") & "' " & vbCrLf
                    If (String.Compare(pWorkSessionID.Trim, String.Empty, False) <> 0) Then cmdText &= " AND WorkSessionID = '" & pWorkSessionID.Trim & "' " & vbCrLf

                    Using dbCmd As New SqlCommand(cmdText, pDBConnection)
                        resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                        resultData.HasError = False
                    End Using
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksResultsDAO.ResetAcceptedResultFlag", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Delete all Blank and Calibrator Results for the Work Session, excepting those that have been 
        ''' validated and accepted
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 20/07/2010 - Code moved here from old ResetWS function
        ''' </remarks>
        Public Function ResetWSBlankCalibResults(ByVal pDBConnection As SqlConnection) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = " DELETE twksResults " & vbCrLf & _
                                            " WHERE  OrderTestID IN (SELECT OrderTestID " & vbCrLf & _
                                                                   " FROM   vwksOrderTests " & vbCrLf & _
                                                                   " WHERE  SampleClass = 'BLANK' " & vbCrLf & _
                                                                   " OR     SampleClass = 'CALIB') " & vbCrLf & _
                                            " AND   (ValidationStatus != 'OK' " & vbCrLf & _
                                            " OR     AcceptedResultFlag = 0) " & vbCrLf

                    Using dbCmd As New SqlCommand(cmdText, pDBConnection)
                        resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                        resultData.HasError = False
                    End Using
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksResultsDAO.ResetWSBlankCalibResults", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get all accepted and validated Quality Control Results in the informed Analyzer WorkSession
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">WorkSession Identifier</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <returns>GlobalDataTO containing a typed Dataset QCResultsDS with all QC Results in the informed Analyzer WorkSession</returns>
        ''' <remarks>
        ''' Created by:  SA 21/05/2012
        ''' Modified By: TR 19/07/2012 -Add the CtrlsSendingGroup row.
        ''' </remarks>
        Public Function ReadWSControlResultsNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, _
                                                ByVal pAnalyzerID As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing
            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT R.RerunNumber, R.ManualResultFlag, R.CONC_Value AS ResultValue, R.ResultDateTime, " & vbCrLf & _
                                                       " R.ManualResult AS ManualResultValue, R.UserComment AS ResultComment, R.TS_User, R.CtrlsSendingGroup, " & vbCrLf & _
                                                       " OT.AnalyzerID, OT.TestType, OT.TestID, OT.SampleType, OT.ControlID, C.LotNumber " & vbCrLf & _
                                                " FROM   twksResults R INNER JOIN twksOrderTests OT ON R.OrderTestID = OT.OrderTestID " & vbCrLf & _
                                                                     " INNER JOIN tparControls C ON OT.ControlID = C.ControlID " & vbCrLf & _
                                                                     " INNER JOIN twksWSOrderTests WSOT ON OT.OrderTestID = WSOT.OrderTestID " & vbCrLf & _
                                                                     " INNER JOIN twksOrders O ON OT.OrderID = O.OrderID " & vbCrLf & _
                                                " WHERE R.ValidationStatus   = 'OK' " & vbCrLf & _
                                                " AND   R.AcceptedResultFlag = 1 " & vbCrLf & _
                                                " AND   OT.OrderTestStatus   = 'CLOSED' " & vbCrLf & _
                                                " AND   O.SampleClass        = 'CTRL' " & vbCrLf & _
                                                " AND   OT.AnalyzerID        = '" & pAnalyzerID.Trim & "' " & vbCrLf & _
                                                " AND   WSOT.WorkSessionID   = '" & pWorkSessionID.Trim & "' " & vbCrLf & _
                                                " ORDER BY OT.TestType, OT.TestID, OT.SampleType, R.CtrlsSendingGroup, R.ResultDateTime " & vbCrLf

                        Dim myResultsDS As New QCResultsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myResultsDS.tqcResults)
                            End Using
                        End Using

                        resultData.SetDatos = myResultsDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksResultsDAO.ReadWSControlResults", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Delete all Patient and Control Results for the Work Session
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by:  DL 04/06/2010
        ''' Modified by: SG 19/07/2010 - Delete only results for Patient and Control Order Tests
        '''              SA 20/07/2010 - Function is divided in two: this one to delete results belonging
        '''                              to Patient and Control Order Tests and a different one to delete
        '''                              non valid and accepted results belonging to Blank and Calibrator
        '''                              Order Tests; name changed from ResetWS to ResetWSPatientCtrlResults
        ''' </remarks>
        Public Function ResetWSPatientCtrlResults(ByVal pDBConnection As SqlConnection) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = " DELETE twksResults " & vbCrLf & _
                                            " WHERE  OrderTestID IN (SELECT OrderTestID " & vbCrLf & _
                                                                   " FROM   vwksOrderTests " & vbCrLf & _
                                                                   " WHERE  SampleClass = 'PATIENT' " & vbCrLf & _
                                                                   " OR     SampleClass = 'CTRL') " & vbCrLf

                    Using dbCmd As New SqlCommand(cmdText, pDBConnection)
                        resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                        resultData.HasError = False
                    End Using
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksResultsDAO.ResetWSPatientCtrlResults", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Delete all Results of the active Work Session
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 05/09/2012
        ''' </remarks>
        Public Function ResetWSResults(ByVal pDBConnection As SqlConnection) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Const cmdText As String = " DELETE FROM twksResults "
                    Using dbCmd As New SqlCommand(cmdText, pDBConnection)
                        resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                        resultData.HasError = False
                    End Using
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksResultsDAO.ResetWSResults", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Set to the informed value the AcceptedResultFlag of the Result with the informed OrderTestID and RerunNumber 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pOrderTestID">Order Test Identifier</param>
        ''' <param name="pRerunNumber">Rerun Number</param>
        ''' <param name="pNewAcceptedValue">Value TRUE/FALSE to assign to the result</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  AG 03/03/2010 
        ''' Modified by: SA 16/07/2012 - Added optional parameters for the Analyzer and WorkSession
        ''' </remarks>
        Public Function UpdateAcceptedResult(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOrderTestID As Integer, ByVal pRerunNumber As Integer, _
                                             ByVal pNewAcceptedValue As Boolean, Optional ByVal pAnalyzerID As String = "", _
                                             Optional ByVal pWorkSessionID As String = "") As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = " UPDATE twksResults SET AcceptedResultFlag = " & IIf(pNewAcceptedValue, 1, 0).ToString & vbCrLf & _
                                            " WHERE  OrderTestID = " & pOrderTestID.ToString & vbCrLf & _
                                            " AND    RerunNumber = " & pRerunNumber.ToString

                    If (String.Compare(pAnalyzerID.Trim, String.Empty, False) <> 0) Then cmdText &= " AND AnalyzerID = N'" & pAnalyzerID.Trim.Replace("'", "''") & "' " & vbCrLf
                    If (String.Compare(pWorkSessionID.Trim, String.Empty, False) <> 0) Then cmdText &= " AND WorkSessionID = '" & pWorkSessionID.Trim & "' " & vbCrLf

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
                myLogAcciones.CreateLogActivity(ex.Message, "twksResultsDAO.UpdateAcceptedResult", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Updates the Collapse field into the twksResults table
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pNewValue"></param>
        ''' <param name="pOrderTestID">Order Test Identifier</param>
        ''' <param name="pRerunNumber"></param>
        ''' <param name="pMultiItemNumber"></param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  AG 02/08/2010
        ''' </remarks>
        Public Function UpdateCollapse(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pNewValue As Boolean, _
                                       ByVal pOrderTestID As Integer, Optional ByVal pRerunNumber As Integer = Nothing, _
                                       Optional ByVal pMultiItemNumber As Integer = Nothing) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = ""

                    cmdText = "UPDATE twksResults SET"
                    cmdText += " Collapsed = " & CStr(IIf(pNewValue, 1, 0))
                    cmdText += " WHERE OrderTestID = " & pOrderTestID

                    If pRerunNumber <> Nothing Then
                        cmdText += " AND RerunNumber = " & pRerunNumber
                    End If

                    If pMultiItemNumber <> Nothing Then
                        cmdText += " AND MultiPointNumber = " & pMultiItemNumber
                    End If

                    'AG 25/07/2014 RQ00086 - improve memory usage
                    'Dim dbCmd As New SqlCommand
                    'dbCmd.Connection = pDBConnection
                    'dbCmd.CommandText = cmdText

                    'resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                    Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                        resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                        resultData.HasError = False
                    End Using
                    'AG 25/07/2014 RQ00086

                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksResultsDAO.UpdateCollapse", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Updates fields ManualResultFlag, ManualResult and ManualResultText for the specified OrderTestID/MultiItemNumber/RerunNumber
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pManualResultFlag">When TRUE, parameter pManualResult or pManualResultText has to be informed (according value
        '''                                 of parameter pResultType)</param>
        ''' <param name="pResultType">QUANTITATIVE or QUALITATIVE</param>
        ''' <param name="pManualResult">Informed when ManualResultFlag is TRUE, and ResultType is QUANTITATIVE</param>
        ''' <param name="pManualResultText">Informed when ManualResultFlag is TRUE, and ResultType is QUALITATIVE</param>
        ''' <param name="pOrderTestID">Order Test Identifier</param>
        ''' <param name="pMultiItemNumber">MultiItem Number (only for multipoint Calibrators; otherwise, its value is always one</param>
        ''' <param name="pRerunNumber">Rerun Number</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by: 
        ''' Modified by: SA 26/11/2010 - Changed parameter pRerunNumber to optional; when it is not informed, then the result
        '''                              to update will be the one accepted and validated
        '''              SA 21/01/2011 - Update also fields ResultDateTime, TS_User and TS_DateTime
        ''' </remarks>
        Public Function UpdateManualResult(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pManualResultFlag As Boolean, ByVal pResultType As String, _
                                           ByVal pManualResult As Single, ByVal pManualResultText As String, ByVal pOrderTestID As Integer, _
                                           ByVal pMultiItemNumber As Integer, Optional ByVal pRerunNumber As Integer = -1) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = " UPDATE twksResults " & vbCrLf
                    cmdText &= " SET ManualResultFlag = " & IIf(pManualResultFlag, 1, 0).ToString & vbCrLf

                    If (Not pManualResultFlag) Then
                        cmdText += ", ManualResult = NULL" & vbCrLf
                        cmdText += ", ManualResultText = NULL" & vbCrLf

                    ElseIf (String.Compare(pResultType, "QUANTIVE", False) = 0) Then 'Quantitative Manual Result (NUMERICAL)
                        cmdText += ", ManualResult = " & ReplaceNumericString(pManualResult) & vbCrLf
                        cmdText += ", ManualResultText = NULL " & vbCrLf

                    ElseIf (String.Compare(pResultType, "QUALTIVE", False) = 0) Then 'Qualitative Manual Result (TEXT)
                        cmdText += ", ManualResult = NULL " & vbCrLf
                        cmdText += ", ManualResultText = N'" & pManualResultText.ToString.Replace("'", "''") & "' " & vbCrLf
                    End If

                    'Get the connected Username from the current Application Session
                    Dim currentSession As New GlobalBase
                    cmdText += ", TS_User = N'" & currentSession.GetSessionInfo().UserName.Replace("'", "''") & "' " & vbCrLf

                    'Update ResultDateTime and TS_DateTime with the current datetime
                    cmdText += ", ResultDateTime = '" & Now.ToString("yyyyMMdd HH:mm:ss") & "' " & vbCrLf
                    cmdText += ", TS_DateTime = '" & Now.ToString("yyyyMMdd HH:mm:ss") & "' " & vbCrLf

                    cmdText += " WHERE OrderTestID      = " & pOrderTestID & vbCrLf & _
                               " AND   MultiPointNumber = " & pMultiItemNumber & vbCrLf

                    If (pRerunNumber <> -1) Then
                        cmdText += " AND RerunNumber = " & pRerunNumber & vbCrLf
                    Else
                        cmdText += " AND AcceptedResultFlag = 1 " & vbCrLf & _
                                   " AND ValidationStatus   = 'OK' "
                    End If

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
                myLogAcciones.CreateLogActivity(ex.Message, "twksResultsDAO.UpdateManualResult", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Updates the Printed field into the twksResults table
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pNewValue"></param>
        ''' <param name="pOrderTestID">Order Test Identifier</param>
        ''' <returns>GlobalDataTo with error or not</returns>
        ''' <remarks>
        ''' Created by:  RH 22/09/2010
        ''' </remarks>
        Public Function UpdatePrinted(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pNewValue As Boolean, _
                                       ByVal pOrderTestID As Integer) As GlobalDataTO

            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String

                    cmdText = "UPDATE twksResults SET"
                    cmdText += " Printed = " & CStr(IIf(pNewValue, 1, 0))
                    cmdText += " WHERE OrderTestID = " & pOrderTestID

                    'AG 25/07/2014 RQ00086 - improve memory usage
                    'Dim dbCmd As New SqlCommand
                    'dbCmd.Connection = pDBConnection
                    'dbCmd.CommandText = cmdText
                    'resultData.AffectedRecords = dbCmd.ExecuteNonQuery()

                    Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                        resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                        resultData.HasError = False
                    End Using
                    'AG 25/07/2014 RQ00086

                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksResultsDAO.UpdatePrinted", EventLogEntryType.Error, False)
            End Try

            Return resultData

        End Function

        ''' <summary>
        ''' Update the history data of a specific row of twksResults
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTO containing a typed DataSet ResultsDS with all affected records</returns> 
        ''' <remarks>
        ''' Created by:  JB 08/10/2012 
        ''' </remarks>
        Public Function UpdateHistoryData(ByVal pDBConnection As SqlClient.SqlConnection, _
                                          ByVal pRow As ResultsDS.vwksResultsRow) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String
                    cmdText = "UPDATE twksResults SET"
                    With pRow
                        If pRow.IsHistWorkSessionIDNull Then
                            cmdText += " HistWSID = NULL"
                        Else
                            cmdText += " HistWSID = '" & pRow.HistWorkSessionID & "'"
                        End If

                        If pRow.IsHistOrderTestIDNull Then
                            cmdText += ", HistOTID = NULL"
                        Else
                            cmdText += ", HistOTID = " & pRow.HistOrderTestID.ToString
                        End If

                        cmdText += " WHERE OrderTestID      = " & pRow.OrderTestID & _
                                   "   AND RerunNumber      = " & pRow.RerunNumber & _
                                   "   AND MultiPointNumber = " & pRow.MultiPointNumber
                    End With

                    'AG 25/07/2014 RQ00086 - improve memory usage
                    'Dim dbCmd As New SqlCommand
                    'dbCmd.Connection = pDBConnection
                    'dbCmd.CommandText = cmdText
                    'resultData.AffectedRecords = dbCmd.ExecuteNonQuery()

                    Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                        resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                        resultData.HasError = False
                    End Using
                    'AG 25/07/2014 RQ00086


                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksResultsDAO.UpdateHistoryData", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get all the Results for the specified OrderTestID
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pOrderTestID">Order Test Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet ResultsDS with all the Results (vwksResults) found 
        '''          for the informed OrderTestID</returns>
        ''' <remarks>
        ''' Created by: XB 21/03/2013
        ''' </remarks>
        Public Function GetResultsByOrderTest(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOrderTestID As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT * FROM twksResults WHERE OrderTestID = " & pOrderTestID & vbCrLf & _
                                                " ORDER BY RerunNumber DESC"

                        Dim lastResultsDS As New ResultsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(lastResultsDS.vwksResults)
                            End Using
                        End Using

                        resultData.SetDatos = lastResultsDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksResultsDAO.GetResultsByOrderTest", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Updates the LISMessageID
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pResultsDS"></param>
        ''' <returns>GlobalDataTo with error or not</returns>
        ''' <remarks>
        ''' Created by:  SG 10/04/2012
        ''' AG 14/02/2014 - #1505 (Remove clause add SENDING, because we have commented the code that set as SENDING in ExportDelegate methods) - ACTIVATED 24/03/2014 (PAUSED 17/02/2014)
        ''' </remarks>
        Public Function UpdateLISMessageID(ByVal pDBConnection As SqlClient.SqlConnection, _
                                           ByVal pResultsDS As ResultsDS) As GlobalDataTO

            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = ""

                    For Each dr As ResultsDS.twksResultsRow In pResultsDS.twksResults

                        'Update exportStatus + LISMessageID by ordertest and rerunnumber and ExportStatus = SENDING
                        cmdText &= " UPDATE twksResults SET ExportStatus = '" & dr.ExportStatus & "'" & ", LISMessageID = '" & dr.LISMessageID & "' "
                        cmdText &= " WHERE OrderTestID = " & dr.OrderTestID.ToString
                        cmdText &= " AND RerunNumber = " & dr.RerunNumber.ToString
                        'cmdText &= " AND ExportStatus = 'SENDING' " 'AG 24/03/2014 - AG 17/02/2014 this line must be COMMENTED when implement #1505 point 7 ('AG 14/02/2014 - #1505 comment this line)

                    Next

                    'AG 25/07/2014 RQ00086 - improve memory usage
                    'Dim dbCmd As New SqlCommand
                    'dbCmd.Connection = pDBConnection
                    'dbCmd.CommandText = cmdText
                    'resultData.AffectedRecords = dbCmd.ExecuteNonQuery()

                    Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                        resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                        resultData.HasError = False
                    End Using
                    'AG 25/07/2014 RQ00086

                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksResultsDAO.UpdateLISMessageID", EventLogEntryType.Error, False)
            End Try

            Return resultData

        End Function

        ''' <summary>
        ''' Updates the LISMessageID depnding on ExportStatus
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pLISMessageID"></param>
        ''' <param name="pExportStatus"></param>
        ''' <param name="pSetDateTime"></param>
        ''' <returns>GlobalDataTo with error or not</returns>
        ''' <remarks>
        ''' Created by:  SG 10/04/2012
        ''' AG 17/04/2013 add condition and ExportStatus = SENDING in WHERE. Solve the cases with RECALCULATIONS while status is SENDING
        ''' AG 02/04/2014 - #1564 add parameter pSetDateTime (when TRUE the DAO has to inform ExportDateTime, else leave as NULL)
        ''' </remarks>
        Public Function UpdateExportStatusByMessageID(ByVal pDBConnection As SqlClient.SqlConnection, _
                                           ByVal pLISMessageID As String, _
                                           ByVal pExportStatus As String, ByVal pSetDateTime As Boolean) As GlobalDataTO

            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String

                    cmdText = "UPDATE twksResults SET ExportStatus = '" & pExportStatus & "' "

                    'AG 02/04/2014 - #1564
                    If pSetDateTime Then
                        cmdText &= " , ExportDateTime = '" & Now.ToString("yyyyMMdd HH:mm:ss") & "'"
                    Else
                        cmdText &= " , ExportDateTime = NULL "
                    End If
                    'AG 02/04/2014

                    cmdText &= " WHERE LISMessageID = '" & pLISMessageID & "' "
                    cmdText &= " AND ExportStatus = 'SENDING' "

                    'AG 25/07/2014 RQ00086 - improve memory usage
                    'Dim dbCmd As New SqlCommand
                    'dbCmd.Connection = pDBConnection
                    'dbCmd.CommandText = cmdText
                    'resultData.AffectedRecords = dbCmd.ExecuteNonQuery()

                    Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                        resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                        resultData.HasError = False
                    End Using
                    'AG 25/07/2014 RQ00086
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksResultsDAO.UpdateExportStatusByMessageID", EventLogEntryType.Error, False)
            End Try

            Return resultData

        End Function


        ''' <summary>
        ''' Get all OrderTests - Rerun - optionally Execution#1 which results was uploaded using the messageID in parameter
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pLISMessageID"></param>
        ''' <returns></returns>
        ''' <remarks>AG 14/03/2014 - #1533 creation</remarks>
        Public Function GetResultsByMessageID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pLISMessageID As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)

                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT r.OrderTestID, r.RerunNumber, ex.ExecutionID FROM twksResults r LEFT OUTER JOIN "
                        cmdText &= " twksWSExecutions ex ON r.OrderTestID = ex.OrderTestID AND r.RerunNumber = ex.RerunNumber AND ex.ReplicateNumber = 1 "
                        cmdText &= " WHERE r.LISMessageID = '" & pLISMessageID.Replace("'", "''") & "'"

                        Dim myDataSet As New ExecutionsDS

                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dBConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myDataSet.twksWSExecutions)
                            End Using
                        End Using

                        resultData.SetDatos = myDataSet
                        resultData.HasError = False
                    End If
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksResultsDAO.GetResultsByMessageID", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()

            End Try

            Return resultData
        End Function

#End Region
    End Class
End Namespace