Option Explicit On
Option Strict On
Option Infer On

Imports DevExpress.XtraGrid.Views.Grid
Imports DevExpress.XtraEditors.Controls
Imports DevExpress.Utils
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.Global.AlarmEnumerates
Imports Biosystems.Ax00.Global.GlobalEnumerates


'Put here your business code for the tab AlarmsTab inside Monitor Form
Partial Public Class UiMonitor

#Region "Declarations"
    Private ERRORImage As Byte() = Nothing
    Private WARNINGImage As Byte() = Nothing

    Private SOLVEDImage As Byte() = Nothing
    Private SOLVEDWarningImage As Byte() = Nothing
    Private SOLVEDErrorImage As Byte() = Nothing

    Private lblSampleClass As String
    Private lblName As String
    Private lblTest As String
    Private lblReplicateNumber As String
    Private lblRotorPosition As String
    Private lblReagent As String
    Private lblSolution As String
    Private lblWashingSolution As String
    Private lblSpecialSolution As String

    Private S_NO_VOLUME_Format As String
    Private S_NO_VOLUME_BLANK_Format As String
    Private PREP_LOCKED_Format As String
    Private PREP_LOCKED_BLANK_Format As String
    Private PREP_WITH_CLOT_Format As String
    Private PREP_WITH_CLOT_BLANK_Format As String
    Private R_NO_VOLUME_Format As String
    Private BOTTLE_LOCKED_Format As String
    Private WASH_SOL_Format As String
    Private SPEC_SOL_Format As String

    Private SampleClassDict As New Dictionary(Of String, String)
    Private BlankModeDict As New Dictionary(Of String, String)
    Private SolutionCodeDict As New Dictionary(Of String, String)

    Private ERRORLabel As String = String.Empty
    Private WARNINGLabel As String = String.Empty
    Private SOLVEDLabel As String = String.Empty

    'Current alarms in grid (Alarms Tab)
    Private myWSAnalyzerAlarmsDS As WSAnalyzerAlarmsDS = Nothing
#End Region

#Region "Public methods"
    ''' <summary>
    ''' Update content of the grid in Alarms Tab 
    ''' </summary>
    ''' <param name="pRefreshDS"></param>
    ''' <remarks>
    ''' Created by:  AG 29/09/2011
    ''' Modified by: RH 30/01/2012 - Decode field Additional Info
    '''              DL 25/07/2012 - Show a different Icon for solved alarms
    '''              SG 30/07/2012 - Decode field Additional Info for ISE Alarms
    '''              SA 23/10/2013 - BA-1355 ==> Special process for Warning Alarm WS_PAUSE_MODE_WARN: if the Alarm has been solved (Pause Mode has 
    '''                                          finished), the total time the Analyzer was in this state is shown following the Alarm description 
    '''                                          and formatted as [Hours:Minutes:Seconds]
    '''              IT 03/06/2014 - BA-1644 ==> No refresh if the screen is disposed
    '''              SA 31/03/2015 - BA-2384 ==> Based on implementation of BA-2236 for BA-200. When field AdditionalInfo is informed and contains 
    '''                                          a numeric value, it means that value is the FW Error Code for the Alarm and it is added to the Alarm
    '''                                          Description between brackets. Changes in the code to call new functions GetAlarmTypeImage, 
    '''                                          GetDescriptionForAlarmWS_PAUSE_MODE_WARN, GetDescriptionForISEAlarms and GetDescriptionForOtherAlarms.
    ''' </remarks>
    Public Sub UpdateAlarmsTab(ByVal pRefreshDS As UIRefreshDS)
        Try
            'BA-1644 No refresh if the screen is disposed
            If (IsDisposed) Then Return

            If (Not AlarmsXtraGrid Is Nothing) Then
                Dim myGlobalData As GlobalDataTO = Nothing
                Dim myWSAlarmsDelegate As New WSAnalyzerAlarmsDelegate

                myGlobalData = myWSAlarmsDelegate.GetAlarmsMonitor(Nothing, AnalyzerIDField)
                If (Not myGlobalData.HasError AndAlso Not myGlobalData.SetDatos Is Nothing) Then
                    myWSAnalyzerAlarmsDS = DirectCast(myGlobalData.SetDatos, WSAnalyzerAlarmsDS)

                    For Each row As WSAnalyzerAlarmsDS.vwksAlarmsMonitorRow In myWSAnalyzerAlarmsDS.vwksAlarmsMonitor
                        'Get the Alarm Image to show (Warning/Error/Solved Warning/Solved Error)
                        GetAlarmTypeImage(row)

                        'BA-1355: Special process for Warning Alarm WS_PAUSE_MODE_WARN
                        If (row.AlarmID = Alarms.WS_PAUSE_MODE_WARN.ToString()) Then
                            GetDescriptionForAlarmWS_PAUSE_MODE_WARN(row)
                        Else
                            'For the rest of Alarms, decode field Additional Info when it is informed
                            If (Not String.IsNullOrEmpty(row.AdditionalInfo)) Then
                                'Decode field Additional Info for ISE Alarms
                                If (row.AlarmID = Alarms.ISE_CALIB_ERROR.ToString OrElse row.AlarmID = Alarms.ISE_ERROR_A.ToString OrElse _
                                    row.AlarmID = Alarms.ISE_ERROR_B.ToString OrElse row.AlarmID = Alarms.ISE_ERROR_C.ToString OrElse _
                                    row.AlarmID = Alarms.ISE_ERROR_D.ToString OrElse row.AlarmID = Alarms.ISE_ERROR_S.ToString OrElse _
                                    row.AlarmID = Alarms.ISE_ERROR_F.ToString OrElse row.AlarmID = Alarms.ISE_ERROR_M.ToString OrElse _
                                    row.AlarmID = Alarms.ISE_ERROR_N.ToString OrElse row.AlarmID = Alarms.ISE_ERROR_R.ToString OrElse _
                                    row.AlarmID = Alarms.ISE_ERROR_W.ToString OrElse row.AlarmID = Alarms.ISE_ERROR_P.ToString OrElse _
                                    row.AlarmID = Alarms.ISE_ERROR_T.ToString) Then
                                    myGlobalData = GetDescriptionForISEAlarms(row)
                                Else
                                    'BA-2384: When field AdditionalInfo is NUMERIC, it contains the FW Error Code for the Alarm (field can contain
                                    '         also a list of FW Error Codes divided by commas, when the Alarm is linked to several Error Codes and
                                    '         the Analyzer has sent all of them). The value is shown between brackets following the Alarm Description
                                    If (IsNumeric(row.AdditionalInfo.Replace(",", ""))) Then
                                        row.Description &= " - [" & row.AdditionalInfo.ToString & "]"
                                    Else
                                        'Decode field Additional Info for other types of Alarms
                                        myGlobalData = GetDescriptionForOtherAlarms(row)
                                    End If
                                End If
                            End If
                        End If
                    Next
                    AlarmsXtraGrid.DataSource = myWSAnalyzerAlarmsDS.vwksAlarmsMonitor
                End If

                If (myGlobalData.HasError) Then
                    'If an error has happened, show it
                    ShowMessage(Me.Name & ".UpdateAlarmsTab", myGlobalData.ErrorCode, myGlobalData.ErrorMessage, MsgParent)
                End If
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex)
            ShowMessage(Me.Name & ".UpdateAlarmsTab", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", MsgParent)
        End Try
    End Sub
#End Region

#Region "Private methods"
    ''' <summary>
    ''' Get all labels needed to fill the global Format variables used for the different types of Alarms
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 01/04/2015 - Code extracted from function GetAlarmsTabLabels
    ''' </remarks>
    Private Sub FillFormatVariables()
        Try
            If (LanguageID Is Nothing) Then
                LanguageID = GlobalBase.GetSessionInfo().ApplicationLanguage
                myMultiLangResourcesDelegate = New MultilanguageResourcesDelegate
            End If

            lblSampleClass = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_WSPrep_SampleClass", LanguageID)
            lblName = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Name", LanguageID)
            lblTest = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Test_Singular", LanguageID)
            lblReplicateNumber = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_CurveReplicate_Rep", LanguageID)
            lblRotorPosition = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_POSITION", LanguageID)
            lblReagent = myMultiLangResourcesDelegate.GetResourceText(Nothing, "PMD_TUBE_CONTENTS_REAGENT", LanguageID)
            lblSolution = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Solution", LanguageID)
            lblWashingSolution = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_RotorPos_WashingSol", LanguageID)
            lblSpecialSolution = myMultiLangResourcesDelegate.GetResourceText(Nothing, "PMD_TUBE_CONTENTS_SPEC_SOL", LanguageID)

            S_NO_VOLUME_Format = String.Format("- {0}: {1}, {2}: {3}, {4}: {5}", lblSampleClass, "{0}", lblName, "{1}", lblRotorPosition, "{2}")
            S_NO_VOLUME_BLANK_Format = String.Format("- {0}: {1}, {2}: {3}, {4}: {5}", lblSampleClass, "{0}", lblSolution, "{1}", lblRotorPosition, "{2}")

            PREP_LOCKED_Format = String.Format("- {0}, {1}, {2}: {3}", "{0}", "{1}", lblTest, "{2}")
            PREP_LOCKED_BLANK_Format = String.Format("- {0}, {1}: {2}, {3}: {4}", "{0}", lblSolution, "{1}", lblTest, "{2}")

            PREP_WITH_CLOT_Format = String.Format("- {0}, {1}, {2}: {3}, {4}: {5}", "{0}", "{1}", lblTest, "{2}", lblReplicateNumber, "{3}")
            PREP_WITH_CLOT_BLANK_Format = String.Format("- {0}, {1}: {2}, {3}: {4}, {5}: {6}", "{0}", lblSolution, "{1}", lblTest, "{2}", lblReplicateNumber, "{3}")

            WASH_SOL_Format = String.Format("- {0}: {1}, {2}: {3}", lblWashingSolution, "{0}", lblRotorPosition, "{1}")
            SPEC_SOL_Format = String.Format("- {0}: {1}, {2}: {3}", lblSpecialSolution, "{0}", lblRotorPosition, "{1}")
            R_NO_VOLUME_Format = String.Format("- {0}: {1}, {2}: {3}", lblReagent, "{0}", lblRotorPosition, "{1}")

            BOTTLE_LOCKED_Format = (String.Format("- {0}: {1}, {2}: {3}", lblReagent, "{0}", lblRotorPosition, "{1}"))
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex)
            ShowMessage(Me.Name & ".FillFormatVariables", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", MsgParent)
        End Try
    End Sub

    ''' <summary>
    ''' Fill the Dictionary of Additional Solution Codes with the type of Additional Solution informed; Dilution, Special or Washing Solutions
    ''' </summary>
    ''' <param name="pAdditionalSolutionType">Unique identifier of the Sub Table of the Preloaded Master Data from which the data will be obtained</param>
    ''' <remarks>
    ''' Created by:  SA 01/04/2015 - Code extracted from function GetAlarmsTabLabels
    ''' </remarks>
    Private Sub FillSolutionCodesDictionary(ByVal pAdditionalSolutionType As PreloadedMasterDataEnum)
        Try
            Dim myGlobalData As New GlobalDataTO
            Dim myPreloadedMasterDataDelegate As New PreloadedMasterDataDelegate

            myGlobalData = myPreloadedMasterDataDelegate.GetList(Nothing, pAdditionalSolutionType)
            If (Not myGlobalData.HasError AndAlso Not myGlobalData.SetDatos Is Nothing) Then
                Dim myMasterDataDS As PreloadedMasterDataDS = DirectCast(myGlobalData.SetDatos, PreloadedMasterDataDS)

                For Each solutionCode As PreloadedMasterDataDS.tfmwPreloadedMasterDataRow In myMasterDataDS.tfmwPreloadedMasterData
                    If (Not SolutionCodeDict.ContainsKey(solutionCode.ItemID)) Then
                        SolutionCodeDict(solutionCode.ItemID) = solutionCode.FixedItemDesc
                    End If
                Next
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex)
            ShowMessage(Me.Name & ".FillSolutionCodesDictionary", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", MsgParent)
        End Try
    End Sub

    ''' <summary>
    ''' Get the formatted additional information to show for alarm PREP_LOCKED_WARN 
    ''' </summary>
    ''' <param name="pAddInfoPreparationRow">Row of typed DataSet WSAnalyzerAlarmsDS (sub-table AdditionalInfoPrepLocked) containing all data needed 
    '''                                      to build the Additional Information field for this type of Alarm</param>
    ''' <param name="pElementName">Name of the affected Element</param>
    ''' <returns>String formatted with all additional information that has to be shown</returns>
    ''' <remarks>
    ''' Created by: SA 01/04/2015 - BA-2384 ==> Code extracted from function UpdateAlarmsTab
    ''' </remarks>
    Private Function GetAdditionalInfoForAlarmPREP_LOCKED_WARN(ByVal pAddInfoPreparationRow As WSAnalyzerAlarmsDS.AdditionalInfoPrepLockedRow, ByVal pElementName As String) As String
        Dim additionalInfo As String = String.Empty

        Try
            If (pAddInfoPreparationRow.SampleClass = "BLANK") Then
                additionalInfo = String.Format(PREP_LOCKED_BLANK_Format, SampleClassDict(pAddInfoPreparationRow.SampleClass), BlankModeDict(pElementName), _
                                               pAddInfoPreparationRow.TestName)
            Else
                additionalInfo = String.Format(PREP_LOCKED_Format, SampleClassDict(pAddInfoPreparationRow.SampleClass), pElementName, pAddInfoPreparationRow.TestName)
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex)
            ShowMessage(Me.Name & ".GetAdditionalInfoForAlarmPREP_LOCKED_WARN", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", MsgParent)
        End Try
        Return additionalInfo
    End Function

    ''' <summary>
    ''' Get the formatted additional information to show for alarm S_NO_VOLUME_WARN (depleted Sample Tube)
    ''' </summary>
    ''' <param name="pAddInfoPreparationRow">Row of typed DataSet WSAnalyzerAlarmsDS (sub-table AdditionalInfoPrepLocked) containing all data needed 
    '''                                      to build the Additional Information field for this type of Alarm</param>
    ''' <param name="pElementName">Name of the affected Element</param>
    ''' <returns>String formatted with all additional information that has to be shown</returns>
    ''' <remarks>
    ''' Created by: SA 01/04/2015 - BA-2384 ==> Code extracted from function UpdateAlarmsTab
    ''' </remarks>
    Private Function GetAdditionalInfoForAlarmS_NO_VOLUME_WARN(ByVal pAddInfoPreparationRow As WSAnalyzerAlarmsDS.AdditionalInfoPrepLockedRow, ByVal pElementName As String) As String
        Dim additionalInfo As String = String.Empty

        Try
            If (pAddInfoPreparationRow.SampleClass = "BLANK") Then
                additionalInfo = String.Format(S_NO_VOLUME_BLANK_Format, SampleClassDict(pAddInfoPreparationRow.SampleClass), BlankModeDict(pElementName), _
                                               pAddInfoPreparationRow.RotorPosition)
            Else
                additionalInfo = String.Format(S_NO_VOLUME_Format, SampleClassDict(pAddInfoPreparationRow.SampleClass), pElementName, pAddInfoPreparationRow.RotorPosition)
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex)
            ShowMessage(Me.Name & ".GetAdditionalInfoForAlarmS_NO_VOLUME_WARN", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", MsgParent)
        End Try
        Return additionalInfo
    End Function

    ''' <summary>
    ''' Get the formatted additional information to show for both Clot Detection Alarms: CLOT_DETECTION_ERR and CLOT_DETECTION_WARN
    ''' </summary>
    ''' <param name="pAddInfoPreparationRow">Row of typed DataSet WSAnalyzerAlarmsDS (sub-table AdditionalInfoPrepLocked) containing all data needed 
    '''                                      to build the Additional Information field for this type of Alarm</param>
    ''' <param name="pElementName">Name of the affected Element</param>
    ''' <returns>String formatted with all additional information that has to be shown</returns>
    ''' <remarks>
    ''' Created by: SA 01/04/2015 - BA-2384 ==> Code extracted from function UpdateAlarmsTab
    ''' </remarks>
    Private Function GetAdditionalInfoForAlarmsCLOT_DETECTION(ByVal pAddInfoPreparationRow As WSAnalyzerAlarmsDS.AdditionalInfoPrepLockedRow, ByVal pElementName As String) As String
        Dim additionalInfo As String = String.Empty

        Try
            If (pAddInfoPreparationRow.SampleClass = "BLANK") Then
                additionalInfo = String.Format(PREP_WITH_CLOT_BLANK_Format, SampleClassDict(pAddInfoPreparationRow.SampleClass), BlankModeDict(pElementName), _
                                               pAddInfoPreparationRow.TestName, pAddInfoPreparationRow.ReplicateNumber)
            Else
                additionalInfo = String.Format(PREP_WITH_CLOT_Format, SampleClassDict(pAddInfoPreparationRow.SampleClass), pElementName, pAddInfoPreparationRow.TestName, _
                                               pAddInfoPreparationRow.ReplicateNumber)
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex)
            ShowMessage(Me.Name & ".GetAdditionalInfoForAlarmsCLOT_DETECTION", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", MsgParent)
        End Try
        Return additionalInfo
    End Function

    ''' <summary>
    ''' Get the formatted additional information to show for both Depleted Reagent Alarms: R1_NO_VOLUME_WARN and R2_NO_VOLUME_WARN
    ''' </summary>
    ''' <param name="pAddInfoPreparationRow">Row of typed DataSet WSAnalyzerAlarmsDS (sub-table AdditionalInfoPrepLocked) containing all data needed 
    '''                                      to build the Additional Information field for this type of Alarm</param>
    ''' <param name="pElementName">Name of the affected Element</param>
    ''' <returns>String formatted with all additional information that has to be shown</returns>
    ''' <remarks>
    ''' Created by: SA 01/04/2015 - BA-2384 ==> Code extracted from function UpdateAlarmsTab
    ''' </remarks>
    Private Function GetAdditionalInfoForAlarmsREAGENT_NO_VOLUME(ByVal pAddInfoPreparationRow As WSAnalyzerAlarmsDS.AdditionalInfoPrepLockedRow, ByVal pElementName As String) As String
        Dim additionalInfo As String = String.Empty

        Try
            Select Case (pAddInfoPreparationRow.TubeContent)
                Case "SPEC_SOL"
                    additionalInfo = String.Format(SPEC_SOL_Format, SolutionCodeDict(pAddInfoPreparationRow.SolutionCode), pAddInfoPreparationRow.RotorPosition)

                Case "WASH_SOL"
                    additionalInfo = String.Format(WASH_SOL_Format, SolutionCodeDict(pAddInfoPreparationRow.SolutionCode), pAddInfoPreparationRow.RotorPosition)

                Case Else
                    additionalInfo = String.Format(R_NO_VOLUME_Format, pAddInfoPreparationRow.TestName, pAddInfoPreparationRow.RotorPosition)
            End Select
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex)
            ShowMessage(Me.Name & ".GetAdditionalInfoForAlarmsREAGENT_NO_VOLUME", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", MsgParent)
        End Try
        Return additionalInfo
    End Function

    ''' <summary>
    '''  Gets all label texts for Alarm Tab
    ''' </summary>
    ''' <remarks>
    ''' Created by:  RH 26/01/2012
    ''' Modified by: XB 04/02/2013 - BA-1112 ==> Upper conversions redundants because the value is already in UpperCase must delete to avoid 
    '''                                          Regional Settings problems
    '''                              BA-2384 ==> Code to fill the global Format variables used for the different types of Alarms moved to a 
    '''                                          new function FillFormatVariables
    '''                                          Code to fill the global Dictionary of Additional Solution Codes moved to a new function 
    '''                                          FillSolutionCodesDictionary
    ''' </remarks>
    Private Sub GetAlarmsTabLabels()
        Try
            If (LanguageID Is Nothing) Then
                LanguageID = GlobalBase.GetSessionInfo().ApplicationLanguage
                myMultiLangResourcesDelegate = New MultilanguageResourcesDelegate
            End If

            bsTitleLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_ALARMS_LIST", LanguageID)
            ERRORLabel = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_ERROR", LanguageID)
            WARNINGLabel = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_WARNING", LanguageID)
            SOLVEDLabel = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SOLVED", LanguageID)

            'BA-2384: Fill the global Format variables used for the different types of Alarms
            FillFormatVariables()

            'Fill the Dictionary of Sample Classes Codes
            SampleClassDict("BLANK") = myMultiLangResourcesDelegate.GetResourceText(Nothing, "PMD_SAMPLE_CLASSES_BLANK", LanguageID)
            SampleClassDict("CALIB") = myMultiLangResourcesDelegate.GetResourceText(Nothing, "PMD_SAMPLE_CLASSES_CALIB", LanguageID)
            SampleClassDict("CTRL") = myMultiLangResourcesDelegate.GetResourceText(Nothing, "PMD_SAMPLE_CLASSES_CTRL", LanguageID)
            SampleClassDict("PATIENT") = myMultiLangResourcesDelegate.GetResourceText(Nothing, "PMD_SAMPLE_CLASSES_PATIENT", LanguageID)

            'Fill the Dictionary of Special Solutions used for Blanks
            BlankModeDict("SALINESOL") = myMultiLangResourcesDelegate.GetResourceText(Nothing, "PMD_SPECIAL_SOLUTIONS_SALINESOL", LanguageID)
            BlankModeDict("DISTW") = myMultiLangResourcesDelegate.GetResourceText(Nothing, "PMD_SPECIAL_SOLUTIONS_DISTW", LanguageID)
            BlankModeDict("REAGENT") = myMultiLangResourcesDelegate.GetResourceText(Nothing, "PMD_BLANK_MODES_REAGENT", LanguageID) 'Blank only with reagent

            'BA-2384: Fill the Dictionary of Additional Solution Codes with all Dilution, Special and Washing Solutions
            FillSolutionCodesDictionary(GlobalEnumerates.PreloadedMasterDataEnum.DIL_SOLUTIONS)
            FillSolutionCodesDictionary(GlobalEnumerates.PreloadedMasterDataEnum.SPECIAL_SOLUTIONS)
            FillSolutionCodesDictionary(GlobalEnumerates.PreloadedMasterDataEnum.WASHING_SOLUTIONS)
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex)
            ShowMessage(Me.Name & ".GetAlarmsTabLabels", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", MsgParent)
        End Try
    End Sub

    ''' <summary>
    ''' According the type of Alarm received, show the Error Icon or the Warning Icon. Additionally, if the Alarm Status is True, it means
    ''' the Alarm has been solved, and in this case the Icon has a different BackColor 
    ''' </summary>
    ''' <param name="pAlarmMonitorRow">Row of a typed DataSet WSAnalyzerAlarmsDS containing data of the Alarm that has to be shown in Alarms
    '''                                Tab in Monitor Screen</param>
    ''' <remarks>
    ''' Created by: SA 31/03/2015 - BA-2384 ==> Code extracted from function UpdateAlarmsTab
    ''' </remarks>
    Private Sub GetAlarmTypeImage(ByVal pAlarmMonitorRow As WSAnalyzerAlarmsDS.vwksAlarmsMonitorRow)
        Try
            Select Case (pAlarmMonitorRow.AlarmType)
                Case "ERROR"
                    If (pAlarmMonitorRow.AlarmStatus) Then
                        pAlarmMonitorRow.AlarmTypeImage = ERRORImage
                    Else
                        pAlarmMonitorRow.AlarmTypeImage = SOLVEDErrorImage
                    End If
                Case "WARNING"
                    If (pAlarmMonitorRow.AlarmStatus) Then
                        pAlarmMonitorRow.AlarmTypeImage = WARNINGImage
                    Else
                        pAlarmMonitorRow.AlarmTypeImage = SOLVEDWarningImage
                    End If
                Case Else
                    pAlarmMonitorRow.AlarmTypeImage = NoImage
            End Select
            pAlarmMonitorRow.AcceptChanges()

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex)
            ShowMessage(Me.Name & ".GetAlarmTypeImage", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", MsgParent)
        End Try
    End Sub

    ''' <summary>
    ''' For Warning Alarm WS_PAUSE_MODE_WARN, get the number of seconds the Analyzer was in Pause Mode, format it as [Hours:Minutes:Seconds]
    ''' and inform it in Description field
    ''' </summary>
    ''' <param name="pAlarmMonitorRow">Row of a typed DataSet WSAnalyzerAlarmsDS containing data of the Alarm that has to be shown in Alarms
    '''                                Tab in Monitor Screen</param>
    ''' <remarks>
    ''' Created by: SA 31/03/2015 - BA-2384 ==> Code extracted from function UpdateAlarmsTab
    ''' </remarks>
    Private Sub GetDescriptionForAlarmWS_PAUSE_MODE_WARN(ByVal pAlarmMonitorRow As WSAnalyzerAlarmsDS.vwksAlarmsMonitorRow)
        Try
            If (Not pAlarmMonitorRow.IsAlarmPeriodSECNull AndAlso pAlarmMonitorRow.AlarmPeriodSEC > 0) Then
                Dim mySeconds = pAlarmMonitorRow.AlarmPeriodSEC
                Dim myMinutes = 0
                Dim myHours = 0

                'Verify if Pause Lapse can be expressed in HOURS
                If (mySeconds >= 3600) Then
                    'Get the number of Hours and also the remaining seconds... 
                    myHours = Math.DivRem(pAlarmMonitorRow.AlarmPeriodSEC, 3600, mySeconds)
                End If

                'Verify if Pause Lapse can be expressed in MINUTES
                If (mySeconds >= 60) Then
                    'Get the number of minutes and also the remaining seconds...
                    Dim mySecondsToCalc As Integer = mySeconds
                    myMinutes = Math.DivRem(mySecondsToCalc, 60, mySeconds)
                End If

                'Finally, format the Pause lapse as [Hours:Minutes:Seconds] and concatenate it to the Alarm Description field
                pAlarmMonitorRow.Description &= " " & String.Format("[{0}:{1}:{2}]", myHours.ToString("#00"), myMinutes.ToString("00"), mySeconds.ToString("00"))
                pAlarmMonitorRow.AcceptChanges()
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex)
            ShowMessage(Me.Name & ".GetDescriptionForAlarmWS_PAUSE_MODE_WARN", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", MsgParent)
        End Try
    End Sub

    ''' <summary>
    ''' For ISE Alarms, decode field AdditionalInfo and inform it in Description field
    ''' </summary>
    ''' <param name="pAlarmMonitorRow">Row of a typed DataSet WSAnalyzerAlarmsDS containing data of the Alarm that has to be shown in Alarms
    '''                                Tab in Monitor Screen</param>
    ''' <returns>GlobalDataTO containing success/error information</returns>
    ''' <remarks>
    ''' Created by: SA 31/03/2015 - BA-2384 ==> Code extracted from function UpdateAlarmsTab
    ''' </remarks>
    Private Function GetDescriptionForISEAlarms(ByVal pAlarmMonitorRow As WSAnalyzerAlarmsDS.vwksAlarmsMonitorRow) As GlobalDataTO
        Dim myGloblaData As New GlobalDataTO

        Try
            Dim myAnalyzerAlarmsDelegate As New WSAnalyzerAlarmsDelegate
            myGloblaData = myAnalyzerAlarmsDelegate.DecodeISEAdditionalInfo(pAlarmMonitorRow.AlarmID, pAlarmMonitorRow.AdditionalInfo, pAlarmMonitorRow.AnalyzerID)

            If (Not myGloblaData.HasError AndAlso Not myGloblaData.SetDatos Is Nothing) Then
                Dim myAdditionalInfoDS = DirectCast(myGloblaData.SetDatos, WSAnalyzerAlarmsDS)

                For Each row As WSAnalyzerAlarmsDS.twksWSAnalyzerAlarmsRow In myAdditionalInfoDS.twksWSAnalyzerAlarms.Rows
                    If (row.AdditionalInfo.Trim.Length > 0) Then
                        pAlarmMonitorRow.Description &= Environment.NewLine & row.AlarmID & " " & row.AdditionalInfo
                    Else
                        pAlarmMonitorRow.Description = row.AlarmID
                    End If
                Next
                pAlarmMonitorRow.AcceptChanges()
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex)
            ShowMessage(Me.Name & ".GetDescriptionForISEAlarms", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", MsgParent)
        End Try
        Return myGloblaData
    End Function

    ''' <summary>
    ''' Decode content of field Additional Info for all types of Alarms that have this field informed
    ''' </summary>
    ''' <param name="pAlarmMonitorRow">Row of a typed DataSet WSAnalyzerAlarmsDS containing data of the Alarm that has to be shown in Alarms
    '''                                Tab in Monitor Screen</param>
    ''' <returns>GlobalDataTO containing success/error information</returns>
    ''' <remarks>
    ''' Created by: SA 31/03/2015 - BA-2384 ==> Code extracted from function UpdateAlarmsTab
    ''' </remarks>
    Private Function GetDescriptionForOtherAlarms(ByVal pAlarmMonitorRow As WSAnalyzerAlarmsDS.vwksAlarmsMonitorRow) As GlobalDataTO
        Dim myGlobalData As New GlobalDataTO
        Try
            Dim myWSAlarmsDelegate As New WSAnalyzerAlarmsDelegate
            myGlobalData = myWSAlarmsDelegate.DecodeAdditionalInfo(pAlarmMonitorRow.AlarmID, pAlarmMonitorRow.AdditionalInfo)

            If (Not myGlobalData.HasError AndAlso Not myGlobalData.SetDatos Is Nothing) Then
                Dim additionalInfoDS As WSAnalyzerAlarmsDS = DirectCast(myGlobalData.SetDatos, WSAnalyzerAlarmsDS)

                If (additionalInfoDS.AdditionalInfoPrepLocked.Rows.Count > 0) Then
                    Dim adRow As WSAnalyzerAlarmsDS.AdditionalInfoPrepLockedRow = additionalInfoDS.AdditionalInfoPrepLocked(0)

                    Dim elementName As String = adRow.Name
                    If (adRow.SampleClass = "CALIB" AndAlso adRow.NumberOfCalibrators <> "1") Then
                        elementName = String.Format("{0}-{1}", adRow.Name, adRow.MultiItemNumber)
                    End If

                    'If at least one of the needed formats have not a value, call function FillFormatVariables to load them
                    If (S_NO_VOLUME_BLANK_Format Is Nothing OrElse S_NO_VOLUME_Format Is Nothing OrElse _
                        PREP_LOCKED_BLANK_Format Is Nothing OrElse PREP_LOCKED_Format Is Nothing OrElse _
                        PREP_WITH_CLOT_Format Is Nothing OrElse PREP_WITH_CLOT_BLANK_Format Is Nothing OrElse _
                        SPEC_SOL_Format Is Nothing OrElse WASH_SOL_Format Is Nothing OrElse _
                        R_NO_VOLUME_Format Is Nothing OrElse BOTTLE_LOCKED_Format Is Nothing) Then
                        FillFormatVariables()
                    End If

                    'Get the formatted Additional Information to show according the type of Alarm
                    Dim additionalInfo As String = String.Empty
                    Select Case (pAlarmMonitorRow.AlarmID)
                        Case Alarms.S_NO_VOLUME_WARN.ToString()
                            additionalInfo = GetAdditionalInfoForAlarmS_NO_VOLUME_WARN(adRow, elementName)

                        Case Alarms.PREP_LOCKED_WARN.ToString()
                            additionalInfo = GetAdditionalInfoForAlarmPREP_LOCKED_WARN(adRow, elementName)

                        Case Alarms.CLOT_DETECTION_ERR.ToString(), Alarms.CLOT_DETECTION_WARN.ToString()
                            additionalInfo = GetAdditionalInfoForAlarmsCLOT_DETECTION(adRow, elementName)

                        Case Alarms.R1_NO_VOLUME_WARN.ToString(), Alarms.R2_NO_VOLUME_WARN.ToString()
                            additionalInfo = GetAdditionalInfoForAlarmsREAGENT_NO_VOLUME(adRow, elementName)

                        Case Alarms.BOTTLE_LOCKED_WARN.ToString()
                            additionalInfo = String.Format(BOTTLE_LOCKED_Format, adRow.TestName, adRow.RotorPosition)
                    End Select

                    'Finally, add the formatted Additional Information in a new line of Description field
                    pAlarmMonitorRow.Description &= Environment.NewLine & additionalInfo
                    pAlarmMonitorRow.AcceptChanges()
                End If
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex)
            ShowMessage(Me.Name & ".GetDescriptionForOtherAlarms", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", MsgParent)
        End Try
        Return myGlobalData
    End Function

    ''' <summary>
    ''' Execute all methods to initialize and load the Alarms Tab in Monitor Screen
    ''' </summary>
    ''' <remarks>
    ''' Created by:  RH
    ''' </remarks>
    Private Sub InitializeAlarmsTab()
        Try
            'Put your initialization code here. It will be executed in the Monitor OnLoad event
            LoadAlarmsTabImages()
            InitializeAlarmsXtraGridView()
            GetAlarmsTabLabels()
            UpdateAlarmsTab(Nothing)
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex)
            ShowMessage(Me.Name & ".InitializeAlarmsTab", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", MsgParent)
        End Try
    End Sub

    ''' <summary>
    ''' Creates and initializes the AlarmsXtraGridView
    ''' </summary>
    ''' <remarks>
    ''' Created by: RH 24/10/2011
    ''' </remarks>
    Private Sub InitializeAlarmsXtraGridView()
        Try
            Dim typeColumn As New DevExpress.XtraGrid.Columns.GridColumn()
            Dim dateColumn As New DevExpress.XtraGrid.Columns.GridColumn()
            Dim timeColumn As New DevExpress.XtraGrid.Columns.GridColumn()
            Dim nameColumn As New DevExpress.XtraGrid.Columns.GridColumn()
            Dim descriptionColumn As New DevExpress.XtraGrid.Columns.GridColumn()
            Dim solutionColumn As New DevExpress.XtraGrid.Columns.GridColumn()
            Dim statusColumn As New DevExpress.XtraGrid.Columns.GridColumn()

            AlarmsXtraGridView.Columns.Clear()
            AlarmsXtraGridView.Columns.AddRange(New DevExpress.XtraGrid.Columns.GridColumn() _
                                               {typeColumn, dateColumn, timeColumn, nameColumn, descriptionColumn, solutionColumn, statusColumn})

            AlarmsXtraGridView.OptionsView.AllowCellMerge = False
            AlarmsXtraGridView.OptionsView.GroupDrawMode = GroupDrawMode.Default
            AlarmsXtraGridView.OptionsView.ShowGroupedColumns = False
            AlarmsXtraGridView.OptionsView.ColumnAutoWidth = False
            AlarmsXtraGridView.OptionsView.RowAutoHeight = True
            AlarmsXtraGridView.OptionsView.ShowIndicator = False
            AlarmsXtraGridView.Appearance.Row.TextOptions.VAlignment = VertAlignment.Center
            AlarmsXtraGridView.OptionsHint.ShowColumnHeaderHints = False
            AlarmsXtraGridView.OptionsBehavior.Editable = False
            AlarmsXtraGridView.OptionsBehavior.ReadOnly = True
            AlarmsXtraGridView.OptionsCustomization.AllowFilter = False
            AlarmsXtraGridView.OptionsCustomization.AllowSort = True
            AlarmsXtraGridView.ColumnPanelRowHeight = 30
            AlarmsXtraGridView.GroupCount = 0
            AlarmsXtraGridView.OptionsMenu.EnableColumnMenu = False

            myMultiLangResourcesDelegate = New MultilanguageResourcesDelegate

            'Alarm Type Column
            typeColumn.Caption = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Type", LanguageID)
            typeColumn.FieldName = "AlarmTypeImage"
            typeColumn.Name = "Type"
            typeColumn.Visible = True
            typeColumn.Width = 50
            typeColumn.OptionsColumn.AllowSize = True
            typeColumn.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False
            typeColumn.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False

            'Alarm Date Column
            dateColumn.Caption = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Date", LanguageID)
            dateColumn.FieldName = "AlarmDateTime"
            dateColumn.DisplayFormat.FormatType = FormatType.DateTime
            dateColumn.DisplayFormat.FormatString = SystemInfoManager.OSDateFormat
            dateColumn.Name = "Date"
            dateColumn.Visible = True
            dateColumn.Width = 75
            dateColumn.OptionsColumn.AllowSize = True
            dateColumn.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False
            dateColumn.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.True
            dateColumn.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center

            'Alarm Time Column
            timeColumn.Caption = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Time", LanguageID)
            timeColumn.FieldName = "AlarmDateTime"
            timeColumn.DisplayFormat.FormatType = FormatType.DateTime
            timeColumn.DisplayFormat.FormatString = SystemInfoManager.OSLongTimeFormat
            timeColumn.Name = "Time"
            timeColumn.Visible = True
            timeColumn.Width = 75
            timeColumn.OptionsColumn.AllowSize = True
            timeColumn.OptionsColumn.ShowCaption = True
            timeColumn.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False
            timeColumn.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.True
            timeColumn.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center

            'Alarm Name Column
            nameColumn.Caption = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Name", LanguageID)
            nameColumn.FieldName = "Name"
            nameColumn.Name = "Name"
            nameColumn.Visible = True
            nameColumn.Width = 140
            nameColumn.OptionsColumn.AllowSize = True
            nameColumn.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False
            nameColumn.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.True

            'Alarm Description Column
            descriptionColumn.Caption = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Description", LanguageID)
            descriptionColumn.FieldName = "Description"
            descriptionColumn.Name = "Description"
            descriptionColumn.Visible = True
            descriptionColumn.Width = 370
            descriptionColumn.OptionsColumn.AllowSize = True
            descriptionColumn.OptionsColumn.ShowCaption = True
            descriptionColumn.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False
            descriptionColumn.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.True

            'Alarm Solution Column
            solutionColumn.Caption = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Solution", LanguageID)
            solutionColumn.FieldName = "Solution"
            solutionColumn.Name = "Solution"
            solutionColumn.Visible = True
            solutionColumn.Width = 370
            solutionColumn.OptionsColumn.AllowSize = True
            solutionColumn.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False
            solutionColumn.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.True

            'Alarm Status Column
            statusColumn.Caption = "AlarmStatus"
            statusColumn.FieldName = "AlarmStatus"
            statusColumn.Name = "AlarmStatus"
            statusColumn.Visible = False
            statusColumn.Width = 0
            statusColumn.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False
            statusColumn.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False

            'For showing an image on cells
            Dim pictureEdit As DevExpress.XtraEditors.Repository.RepositoryItemPictureEdit = TryCast(AlarmsXtraGrid.RepositoryItems.Add("PictureEdit"),  _
                                                                                                     DevExpress.XtraEditors.Repository.RepositoryItemPictureEdit)
            pictureEdit.SizeMode = PictureSizeMode.Clip
            pictureEdit.NullText = " "

            typeColumn.ColumnEdit = pictureEdit

            'For wrapping text
            Dim largeTextEdit As DevExpress.XtraEditors.Repository.RepositoryItemMemoEdit = TryCast(AlarmsXtraGrid.RepositoryItems.Add("MemoEdit"),  _
                                                                                                    DevExpress.XtraEditors.Repository.RepositoryItemMemoEdit)

            nameColumn.ColumnEdit = largeTextEdit
            descriptionColumn.ColumnEdit = largeTextEdit
            solutionColumn.ColumnEdit = largeTextEdit
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex)
            ShowMessage(Me.Name & ".InitializeAlarmsXtraGridView ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", MsgParent)
        End Try
    End Sub

    ''' <summary>
    ''' Loads the button's images
    ''' </summary>
    ''' <remarks>
    ''' Created by:  RH 11/04/2011
    ''' </remarks>
    Private Sub LoadAlarmsTabImages()
        Try
            Dim auxIconName As String = String.Empty
            Dim preloadedDataConfig As New PreloadedMasterDataDelegate

            'ERROR Image
            auxIconName = GetIconName("WARNINGSMALL")
            If (Not String.IsNullOrEmpty(auxIconName)) Then ERRORImage = preloadedDataConfig.GetIconImage("WARNINGSMALL")

            'WARNING Image
            auxIconName = GetIconName("STUS_WITHERRS")
            If (Not String.IsNullOrEmpty(auxIconName)) Then WARNINGImage = preloadedDataConfig.GetIconImage("STUS_WITHERRS")

            'SOLVED ALARM Image
            auxIconName = GetIconName("SOLVEDSMALL")
            If (Not String.IsNullOrEmpty(auxIconName)) Then SOLVEDImage = preloadedDataConfig.GetIconImage("SOLVEDSMALL")

            'WARNING Disable Image 
            auxIconName = GetIconName("WRNGSMLDIS")
            If (Not String.IsNullOrEmpty(auxIconName)) Then SOLVEDErrorImage = preloadedDataConfig.GetIconImage("WRNGSMLDIS")

            'ALERT Disable Image 
            auxIconName = GetIconName("ALRTSMLDIS")
            If (Not String.IsNullOrEmpty(auxIconName)) Then SOLVEDWarningImage = preloadedDataConfig.GetIconImage("ALRTSMLDIS")
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex)
            ShowMessage(Me.Name & ".LoadAlarmsTabImages ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", MsgParent)
        End Try
    End Sub
#End Region

#Region "Events"
    ''' <summary>
    ''' Processes the mouse over event over XtraGridView
    ''' </summary>
    ''' <remarks>
    ''' Created by: RH 26/04/2012
    ''' </remarks>
    Private Sub ToolTipController1_GetActiveObjectInfo(ByVal sender As System.Object, ByVal e As ToolTipControllerGetActiveObjectInfoEventArgs) Handles ToolTipController1.GetActiveObjectInfo
        Try
            If (Not e.SelectedControl Is AlarmsXtraGrid) Then Return

            'Get the view at the current mouse position
            Dim view = TryCast(AlarmsXtraGrid.GetViewAt(e.ControlMousePosition), GridView)
            If (view Is Nothing) Then Return

            'Get the view's element information that resides at the current position
            Dim hi = view.CalcHitInfo(e.ControlMousePosition)
            If (hi.InRowCell) Then
                'This is how to get the DataRow behind the GridViewRow
                Dim currentRow = CType(view.GetDataRow(hi.RowHandle), WSAnalyzerAlarmsDS.vwksAlarmsMonitorRow)

                Select Case hi.Column.Name
                    Case "Type"
                        If (Not currentRow.AlarmStatus) Then
                            e.Info = New ToolTipControlInfo(SOLVEDLabel, SOLVEDLabel)
                        Else
                            Select Case currentRow.AlarmType
                                Case "ERROR"
                                    e.Info = New ToolTipControlInfo(ERRORLabel, ERRORLabel)

                                Case "WARNING"
                                    e.Info = New ToolTipControlInfo(WARNINGLabel, WARNINGLabel)
                            End Select
                        End If
                End Select
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex)
            ShowMessage(Me.Name & ".ToolTipController1_GetActiveObjectInfo ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub
#End Region
End Class