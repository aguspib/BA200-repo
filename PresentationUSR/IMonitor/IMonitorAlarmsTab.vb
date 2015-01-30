'Put here your business code for the tab AlarmsTab inside Monitor Form

Option Explicit On
'Option Strict On

Imports DevExpress.XtraGrid
Imports DevExpress.XtraGrid.Columns
Imports DevExpress.XtraGrid.Views.Base
Imports DevExpress.XtraGrid.Views.Grid
Imports DevExpress.XtraGrid.Views.Grid.ViewInfo
Imports DevExpress.XtraGrid.Repository
Imports DevExpress.XtraEditors.Controls
Imports DevExpress.Utils

Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.BL
'Imports Biosystems.Ax00.Controls.UserControls
Imports Biosystems.Ax00.Global.GlobalEnumerates

Partial Public Class IMonitor

#Region "Declarations"

    Private ERRORImage As Byte() = Nothing
    Private WARNINGImage As Byte() = Nothing
    Private SOLVEDimage As Byte() = Nothing 'AG 17/10/2011 - Alarm solved

    Private SOLVEDWarningImage As Byte() = Nothing  'DL 25/07/2012
    Private SOLVEDErrorImage As Byte() = Nothing    'DL 25/07/2012


    Private myWSAnalyzerAlarmsDS As WSAnalyzerAlarmsDS = Nothing 'Current alarms in grid (Alarms Tab)

    Private lblSampleClass As String
    Private lblName As String
    Private lblTest As String
    Private lblReplicateNumber As String
    Private lblRotorPosition As String
    Private lblReagent As String
    Private lblSolution As String
    Private lblWashingSolution As String 'RH 17/05/2012
    Private lblSpecialSolution As String 'RH 17/05/2012

    Private S_NO_VOLUME_Format As String
    Private S_NO_VOLUME_BLANK_Format As String
    Private PREP_LOCKED_Format As String
    Private PREP_LOCKED_BLANK_Format As String
    Private PREP_WITH_CLOT_Format As String       'AG 25/07/2012
    Private PREP_WITH_CLOT_BLANK_Format As String 'AG 25/07/2012
    Private R_NO_VOLUME_Format As String
    Private BOTTLE_LOCKED_Format As String        'TR 01/10/2012 

    Private WASH_SOL_Format As String 'RH 17/05/2012
    Private SPEC_SOL_Format As String 'RH 17/05/2012
    Private SampleClassDict As New Dictionary(Of String, String)
    Private BlankModeDict As New Dictionary(Of String, String)
    Private SolutionCodeDict As New Dictionary(Of String, String) 'RH 18/05/2012

    Private ERRORLabel As String = String.Empty
    Private WARNINGLabel As String = String.Empty
    Private SOLVEDLabel As String = String.Empty

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
    '''              SA 23/10/2013 - BT #1355 ==> Special process for Warning Alarm WS_PAUSE_MODE_WARN: if the Alarm has been solved (Pause Mode has finished),
    '''                              the total time the Analyzer was in this state is shown following the Alarm description and formatted as [Hours:Minutes:Seconds]
    '''              AG 09/12/2014 BA-2236 show the error code when informed 'Alarm Description [error code]' it is stored in AdditionalInfo column
    ''' </remarks>
    Public Sub UpdateAlarmsTab(ByVal pRefreshDS As UIRefreshDS)
        Try
            If (IsDisposed) Then Return 'IT 03/06/2014 - #1644 No refresh if screen is disposed

            Dim myGloblaData As GlobalDataTO = Nothing
            Dim myWSAlarmsDelegate As New WSAnalyzerAlarmsDelegate

            If Not AlarmsXtraGrid Is Nothing Then
                myGloblaData = myWSAlarmsDelegate.GetAlarmsMonitor(Nothing, AnalyzerIDField)
                If (Not myGloblaData.HasError AndAlso Not myGloblaData.SetDatos Is Nothing) Then
                    myWSAnalyzerAlarmsDS = DirectCast(myGloblaData.SetDatos, WSAnalyzerAlarmsDS)

                    Dim name As String
                    Dim additionalInfo As String
                    Dim additionalInfoDS As WSAnalyzerAlarmsDS
                    Dim additionalISEInfoDS As WSAnalyzerAlarmsDS
                    Dim adRow As WSAnalyzerAlarmsDS.AdditionalInfoPrepLockedRow

                    For Each row As WSAnalyzerAlarmsDS.vwksAlarmsMonitorRow In myWSAnalyzerAlarmsDS.vwksAlarmsMonitor
                        'Show a different Icon for solved alarms
                        Select Case row.AlarmType
                            Case "ERROR"
                                If (row.AlarmStatus) Then
                                    row.AlarmTypeImage = ERRORImage
                                Else
                                    row.AlarmTypeImage = SOLVEDErrorImage
                                End If
                            Case "WARNING"
                                If (row.AlarmStatus) Then
                                    row.AlarmTypeImage = WARNINGImage
                                Else
                                    row.AlarmTypeImage = SOLVEDWarningImage
                                End If
                            Case Else
                                row.AlarmTypeImage = NoImage
                        End Select

                        'BT #1355 ==> Special process for Warning Alarm WS_PAUSE_MODE_WARN
                        If (row.AlarmID = GlobalEnumerates.Alarms.WS_PAUSE_MODE_WARN.ToString() AndAlso (Not row.IsAlarmPeriodSECNull)) Then
                            If (row.AlarmPeriodSEC > 0) Then
                                Dim mySeconds As Integer = row.AlarmPeriodSEC
                                Dim myMinutes As Integer = 0
                                Dim myHours As Integer = 0

                                'Verify if Pause Lapse can be expressed in HOURS
                                If (mySeconds >= 3600) Then
                                    'Get the number of Hours and also the remaining seconds... 
                                    myHours = Math.DivRem(row.AlarmPeriodSEC, 3600, mySeconds)
                                End If

                                'Verify if Pause Lapse can be expressed in MINUTES
                                If (mySeconds >= 60) Then
                                    'Get the number of minutes and also the remaining seconds...
                                    Dim mySecondsToCalc As Integer = mySeconds
                                    myMinutes = Math.DivRem(mySecondsToCalc, 60, mySeconds)
                                End If

                                'Finally, format the Pause lapse as [Hours:Minutes:Seconds] and concatenate it to the Alarm Description field
                                additionalInfo = String.Format("[{0}:{1}:{2}]", myHours.ToString("#00"), myMinutes.ToString("00"), mySeconds.ToString("00"))
                                row.Description &= " " & additionalInfo
                            End If
                        End If

                        'Decode field Additional Info
                        If (Not String.IsNullOrEmpty(row.AdditionalInfo)) Then
                            'Decode field Additional Info for ISE Alarms
                            If (row.AlarmID = Alarms.ISE_CALIB_ERROR.ToString OrElse _
                                row.AlarmID = Alarms.ISE_ERROR_A.ToString OrElse _
                                row.AlarmID = Alarms.ISE_ERROR_B.ToString OrElse _
                                row.AlarmID = Alarms.ISE_ERROR_C.ToString OrElse _
                                row.AlarmID = Alarms.ISE_ERROR_D.ToString OrElse _
                                row.AlarmID = Alarms.ISE_ERROR_S.ToString OrElse _
                                row.AlarmID = Alarms.ISE_ERROR_F.ToString OrElse _
                                row.AlarmID = Alarms.ISE_ERROR_M.ToString OrElse _
                                row.AlarmID = Alarms.ISE_ERROR_N.ToString OrElse _
                                row.AlarmID = Alarms.ISE_ERROR_R.ToString OrElse _
                                row.AlarmID = Alarms.ISE_ERROR_W.ToString OrElse _
                                row.AlarmID = Alarms.ISE_ERROR_P.ToString OrElse _
                                row.AlarmID = Alarms.ISE_ERROR_T.ToString) Then

                                myGloblaData = myWSAlarmsDelegate.DecodeISEAdditionalInfo(row.AlarmID, row.AdditionalInfo, row.AnalyzerID)
                                If (Not myGloblaData.HasError AndAlso Not myGloblaData.SetDatos Is Nothing) Then
                                    additionalISEInfoDS = DirectCast(myGloblaData.SetDatos, WSAnalyzerAlarmsDS)

                                    For Each adISERow As WSAnalyzerAlarmsDS.twksWSAnalyzerAlarmsRow In additionalISEInfoDS.twksWSAnalyzerAlarms.Rows
                                        If (adISERow.AdditionalInfo.Trim.Length > 0) Then
                                            additionalInfo = adISERow.AlarmID & " " & adISERow.AdditionalInfo
                                            row.Description &= Environment.NewLine & additionalInfo
                                        Else
                                            row.Description = adISERow.AlarmID
                                        End If
                                    Next
                                End If
                            Else
                                'Decode field Additional Info for other Alarms
                                myGloblaData = myWSAlarmsDelegate.DecodeAdditionalInfo(row.AlarmID, row.AdditionalInfo)
                                If (Not myGloblaData.HasError AndAlso Not myGloblaData.SetDatos Is Nothing) Then
                                    additionalInfoDS = DirectCast(myGloblaData.SetDatos, WSAnalyzerAlarmsDS)

                                    adRow = additionalInfoDS.AdditionalInfoPrepLocked(0)
                                    If (String.Equals(adRow.SampleClass, "CALIB")) Then
                                        If (String.Equals(adRow.NumberOfCalibrators, "1")) Then
                                            name = adRow.Name
                                        Else
                                            name = String.Format("{0}-{1}", adRow.Name, adRow.MultiItemNumber)
                                        End If
                                    Else
                                        name = adRow.Name
                                    End If
                                    additionalInfo = String.Empty

                                    'DL 11/07/2012. In any case arrives here without load values for variable. Ensure that variables always has values
                                    'TR 01/10/2012 -Added the bottle locked validation.
                                    If (S_NO_VOLUME_BLANK_Format Is Nothing OrElse S_NO_VOLUME_Format Is Nothing OrElse _
                                        PREP_LOCKED_BLANK_Format Is Nothing OrElse PREP_LOCKED_Format Is Nothing OrElse _
                                        SPEC_SOL_Format Is Nothing OrElse WASH_SOL_Format Is Nothing OrElse _
                                        PREP_WITH_CLOT_Format Is Nothing OrElse PREP_WITH_CLOT_BLANK_Format Is Nothing OrElse _
                                        R_NO_VOLUME_Format Is Nothing OrElse BOTTLE_LOCKED_Format Is Nothing) Then
                                        GetAlarmsTabLabels()
                                    End If
                                    'DL 11/07/2012 - End.

                                    Select Case row.AlarmID
                                        Case GlobalEnumerates.Alarms.S_NO_VOLUME_WARN.ToString()
                                            If (String.Equals(adRow.SampleClass, "BLANK")) Then
                                                additionalInfo = String.Format(S_NO_VOLUME_BLANK_Format, SampleClassDict(adRow.SampleClass), _
                                                                               BlankModeDict(name), adRow.RotorPosition)
                                            Else
                                                additionalInfo = String.Format(S_NO_VOLUME_Format, SampleClassDict(adRow.SampleClass), _
                                                                               name, adRow.RotorPosition)
                                            End If

                                        Case GlobalEnumerates.Alarms.PREP_LOCKED_WARN.ToString()
                                            If (String.Equals(adRow.SampleClass, "BLANK")) Then
                                                additionalInfo = String.Format(PREP_LOCKED_BLANK_Format, SampleClassDict(adRow.SampleClass), _
                                                                               BlankModeDict(name), adRow.TestName) ', adRow.ReplicateNumber) 'AG 18/06/2012 - do not shown the replicate number
                                            Else
                                                additionalInfo = String.Format(PREP_LOCKED_Format, SampleClassDict(adRow.SampleClass), _
                                                                               name, adRow.TestName) ', adRow.ReplicateNumber)'AG 18/06/2012 - do not shown the replicate number
                                            End If

                                            'AG 25/07/2012
                                        Case GlobalEnumerates.Alarms.CLOT_DETECTION_ERR.ToString(), _
                                             GlobalEnumerates.Alarms.CLOT_DETECTION_WARN.ToString()
                                            If (String.Equals(adRow.SampleClass, "BLANK")) Then
                                                additionalInfo = String.Format(PREP_WITH_CLOT_BLANK_Format, SampleClassDict(adRow.SampleClass), _
                                                                               BlankModeDict(name), adRow.TestName, adRow.ReplicateNumber)
                                            Else
                                                additionalInfo = String.Format(PREP_WITH_CLOT_Format, SampleClassDict(adRow.SampleClass), _
                                                                               name, adRow.TestName, adRow.ReplicateNumber)
                                            End If
                                            'AG 25/07/2012

                                        Case GlobalEnumerates.Alarms.R1_NO_VOLUME_WARN.ToString(), _
                                             GlobalEnumerates.Alarms.R2_NO_VOLUME_WARN.ToString()
                                            'RH 18/05/2012
                                            Select Case adRow.TubeContent
                                                Case "SPEC_SOL"
                                                    additionalInfo = String.Format(SPEC_SOL_Format, SolutionCodeDict(adRow.SolutionCode), adRow.RotorPosition)

                                                Case "WASH_SOL"
                                                    additionalInfo = String.Format(WASH_SOL_Format, SolutionCodeDict(adRow.SolutionCode), adRow.RotorPosition)

                                                Case Else
                                                    additionalInfo = String.Format(R_NO_VOLUME_Format, adRow.TestName, adRow.RotorPosition)
                                            End Select
                                            'RH 18/05/2012 - END

                                        Case GlobalEnumerates.Alarms.BOTTLE_LOCKED_WARN.ToString() 'TR 01/10/2012 -Case Reagent bottle locked. 
                                            additionalInfo = String.Format(BOTTLE_LOCKED_Format, adRow.TestName, adRow.RotorPosition)
                                    End Select

                                    row.Description &= Environment.NewLine & additionalInfo

                                    'AG 09/12/2014 BA-2236 - AdditionalInfo = ErrorCode
                                ElseIf Not myGloblaData.HasError AndAlso IsNumeric(row.AdditionalInfo) Then
                                    row.Description &= " - [" & row.AdditionalInfo.ToString & "]"
                                    'AG 09/12/2014 BA-2236

                                End If
                            End If
                        End If
                        'END RH 30/01/2012

                    Next

                    AlarmsXtraGrid.DataSource = myWSAnalyzerAlarmsDS.vwksAlarmsMonitor
                End If
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".UpdateAlarmsTab ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".UpdateAlarmsTab", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", MsgParent)
        End Try
    End Sub
#End Region

#Region "Private methods"

    Private Sub InitializeAlarmsTab()
        'Put your initialization code here. It will be executed in the Monitor OnLoad event
        LoadAlarmsTabImages()
        InitializeAlarmsXtraGridView()
        GetAlarmsTabLabels()
        UpdateAlarmsTab(Nothing)
    End Sub

    ''' <summary>
    ''' Creates and initializes the AlarmsXtraGridView
    ''' </summary>
    ''' <remarks>
    ''' Created by: RH 24/10/2011
    ''' </remarks>
    Private Sub InitializeAlarmsXtraGridView()
        Try
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            AlarmsXtraGridView.Columns.Clear()

            Dim TypeColumn As New DevExpress.XtraGrid.Columns.GridColumn()
            Dim DateColumn As New DevExpress.XtraGrid.Columns.GridColumn()
            Dim TimeColumn As New DevExpress.XtraGrid.Columns.GridColumn()
            Dim NameColumn As New DevExpress.XtraGrid.Columns.GridColumn()
            Dim DescriptionColumn As New DevExpress.XtraGrid.Columns.GridColumn()
            Dim SolutionColumn As New DevExpress.XtraGrid.Columns.GridColumn()
            Dim StatusColumn As New DevExpress.XtraGrid.Columns.GridColumn() 'DL 24/07/2012 TO TEST

            AlarmsXtraGridView.Columns.AddRange(New DevExpress.XtraGrid.Columns.GridColumn() _
                        {TypeColumn, DateColumn, TimeColumn, _
                         NameColumn, DescriptionColumn, SolutionColumn, StatusColumn})

            AlarmsXtraGridView.OptionsView.AllowCellMerge = False
            AlarmsXtraGridView.OptionsView.GroupDrawMode = GroupDrawMode.Default
            AlarmsXtraGridView.OptionsView.ShowGroupedColumns = False
            AlarmsXtraGridView.OptionsView.ColumnAutoWidth = False
            AlarmsXtraGridView.OptionsView.RowAutoHeight = True
            AlarmsXtraGridView.OptionsView.ShowIndicator = False

            AlarmsXtraGridView.Appearance.Row.TextOptions.VAlignment = VertAlignment.Center
            'AlarmsXtraGridView.Appearance.FocusedRow.ForeColor = Color.White
            'AlarmsXtraGridView.Appearance.FocusedRow.BackColor = Color.LightSlateGray

            AlarmsXtraGridView.OptionsHint.ShowColumnHeaderHints = False

            AlarmsXtraGridView.OptionsBehavior.Editable = False
            AlarmsXtraGridView.OptionsBehavior.ReadOnly = True
            AlarmsXtraGridView.OptionsCustomization.AllowFilter = False
            AlarmsXtraGridView.OptionsCustomization.AllowSort = True

            'AlarmsXtraGridView.OptionsSelection.EnableAppearanceFocusedRow = True
            'AlarmsXtraGridView.OptionsSelection.MultiSelect = False

            AlarmsXtraGridView.ColumnPanelRowHeight = 30
            AlarmsXtraGridView.GroupCount = 0
            AlarmsXtraGridView.OptionsMenu.EnableColumnMenu = False

            'TypeColumn
            TypeColumn.Caption = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Type", LanguageID)
            TypeColumn.FieldName = "AlarmTypeImage"
            TypeColumn.Name = "Type"
            TypeColumn.Visible = True
            TypeColumn.Width = 50
            TypeColumn.OptionsColumn.AllowSize = True
            TypeColumn.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False
            TypeColumn.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False

            'Date Column
            DateColumn.Caption = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Date", LanguageID)
            DateColumn.FieldName = "AlarmDateTime"
            DateColumn.DisplayFormat.FormatType = FormatType.DateTime
            DateColumn.DisplayFormat.FormatString = SystemInfoManager.OSDateFormat
            DateColumn.Name = "Date"
            DateColumn.Visible = True
            DateColumn.Width = 75
            DateColumn.OptionsColumn.AllowSize = True
            DateColumn.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False
            DateColumn.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.True
            DateColumn.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center

            'Time column
            TimeColumn.Caption = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Time", LanguageID)
            TimeColumn.FieldName = "AlarmDateTime"
            TimeColumn.DisplayFormat.FormatType = FormatType.DateTime
            TimeColumn.DisplayFormat.FormatString = SystemInfoManager.OSLongTimeFormat
            TimeColumn.Name = "Time"
            TimeColumn.Visible = True
            TimeColumn.Width = 75
            TimeColumn.OptionsColumn.AllowSize = True
            TimeColumn.OptionsColumn.ShowCaption = True
            TimeColumn.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False
            TimeColumn.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.True
            TimeColumn.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center

            'Name Column
            NameColumn.Caption = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Name", LanguageID)
            NameColumn.FieldName = "Name"
            NameColumn.Name = "Name"
            NameColumn.Visible = True
            NameColumn.Width = 140
            NameColumn.OptionsColumn.AllowSize = True
            NameColumn.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False
            NameColumn.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.True

            'Description Column
            DescriptionColumn.Caption = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Description", LanguageID)
            DescriptionColumn.FieldName = "Description"
            DescriptionColumn.Name = "Description"
            DescriptionColumn.Visible = True
            DescriptionColumn.Width = 370
            DescriptionColumn.OptionsColumn.AllowSize = True
            DescriptionColumn.OptionsColumn.ShowCaption = True
            DescriptionColumn.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False
            DescriptionColumn.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.True

            'Solution Column
            SolutionColumn.Caption = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Solution", LanguageID)
            SolutionColumn.FieldName = "Solution"
            SolutionColumn.Name = "Solution"
            SolutionColumn.Visible = True
            SolutionColumn.Width = 370
            SolutionColumn.OptionsColumn.AllowSize = True
            SolutionColumn.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False
            SolutionColumn.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.True

            'DL 24/07/2012 TO TEST
            'StatusColumn Column
            StatusColumn.Caption = "AlarmStatus"
            StatusColumn.FieldName = "AlarmStatus"
            StatusColumn.Name = "AlarmStatus"
            StatusColumn.Visible = False
            StatusColumn.Width = 0
            'StatusColumn.OptionsColumn.AllowSize = True
            StatusColumn.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False
            StatusColumn.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False
            'DL 24/07/2012 TO TEST

            'For showing an image on cells
            Dim pictureEdit As DevExpress.XtraEditors.Repository.RepositoryItemPictureEdit = _
                    TryCast(AlarmsXtraGrid.RepositoryItems.Add("PictureEdit"),  _
                    DevExpress.XtraEditors.Repository.RepositoryItemPictureEdit)

            pictureEdit.SizeMode = PictureSizeMode.Clip
            pictureEdit.NullText = " "

            TypeColumn.ColumnEdit = pictureEdit

            'For wrapping text
            Dim largeTextEdit As DevExpress.XtraEditors.Repository.RepositoryItemMemoEdit = _
                    TryCast(AlarmsXtraGrid.RepositoryItems.Add("MemoEdit"),  _
                    DevExpress.XtraEditors.Repository.RepositoryItemMemoEdit)

            NameColumn.ColumnEdit = largeTextEdit
            DescriptionColumn.ColumnEdit = largeTextEdit
            SolutionColumn.ColumnEdit = largeTextEdit

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".InitializeAlarmsXtraGridView ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", MsgParent)
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
            Dim preloadedDataConfig As New PreloadedMasterDataDelegate
            Dim auxIconName As String = ""
            'Dim iconPath As String = MyBase.IconsPath

            'ERROR Image
            auxIconName = GetIconName("WARNINGSMALL")
            If Not String.IsNullOrEmpty(auxIconName) Then ERRORImage = preloadedDataConfig.GetIconImage("WARNINGSMALL")

            'WARNING Image
            auxIconName = GetIconName("STUS_WITHERRS")
            If Not String.IsNullOrEmpty(auxIconName) Then WARNINGImage = preloadedDataConfig.GetIconImage("STUS_WITHERRS")

            'SOLVED ALARM Image
            'TODO
            SOLVEDimage = NoImage
            auxIconName = GetIconName("SOLVEDSMALL")
            If Not String.IsNullOrEmpty(auxIconName) Then SOLVEDimage = preloadedDataConfig.GetIconImage("SOLVEDSMALL")

            'DL 25/07/2012
            'WARNING Disable Image 
            auxIconName = GetIconName("WRNGSMLDIS")
            If Not String.IsNullOrEmpty(auxIconName) Then SOLVEDErrorImage = preloadedDataConfig.GetIconImage("WRNGSMLDIS")

            'ALERT Disable Image 
            auxIconName = GetIconName("ALRTSMLDIS")
            If Not String.IsNullOrEmpty(auxIconName) Then SOLVEDWarningImage = preloadedDataConfig.GetIconImage("ALRTSMLDIS")
            'DL 25/07/2012

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".LoadAlarmsTabImages ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".LoadAlarmsTabImages ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", MsgParent)
        End Try
    End Sub

    ''' <summary>
    '''  Gets all label texts for Alarm Tab
    ''' </summary>
    ''' <remarks>
    ''' Created by:  RH 26/01/2012
    ''' Modified by: XB 04/02/2013 - Upper conversions redundants because the value is already in UpperCase must delete to avoid Regional Settings problems (Bugs tracking #1112)
    ''' </remarks>
    Private Sub GetAlarmsTabLabels()
        Try
            Dim myGlobalDataTO As New GlobalDataTO

            If (LanguageID Is Nothing) Then
                Dim currentLanguageGlobal As New GlobalBase
                LanguageID = currentLanguageGlobal.GetSessionInfo().ApplicationLanguage

                myMultiLangResourcesDelegate = New MultilanguageResourcesDelegate
            End If

            lblSampleClass = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_WSPrep_SampleClass", LanguageID)
            lblName = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Name", LanguageID)
            lblTest = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Test_Singular", LanguageID)
            lblReplicateNumber = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_CurveReplicate_Rep", LanguageID)
            lblRotorPosition = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_POSITION", LanguageID)
            lblReagent = myMultiLangResourcesDelegate.GetResourceText(Nothing, "PMD_TUBE_CONTENTS_REAGENT", LanguageID)
            lblSolution = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Solution", LanguageID)

            S_NO_VOLUME_Format = String.Format("- {0}: {1}, {2}: {3}, {4}: {5}", lblSampleClass, "{0}", lblName, "{1}", lblRotorPosition, "{2}")
            S_NO_VOLUME_BLANK_Format = String.Format("- {0}: {1}, {2}: {3}, {4}: {5}", lblSampleClass, "{0}", lblSolution, "{1}", lblRotorPosition, "{2}")

            'AG 18/06/2012 - Do not shown the replicate number
            PREP_LOCKED_Format = String.Format("- {0}, {1}, {2}: {3}", "{0}", "{1}", lblTest, "{2}")
            'AG 18/06/2012

            'DL 21/06/2012
            PREP_LOCKED_BLANK_Format = String.Format("- {0}, {1}: {2}, {3}: {4}", "{0}", lblSolution, "{1}", lblTest, "{2}")

            'AG 25/07/2012 - preparation clot alarms 
            PREP_WITH_CLOT_Format = String.Format("- {0}, {1}, {2}: {3}, {4}: {5}", "{0}", "{1}", lblTest, "{2}", lblReplicateNumber, "{3}")
            PREP_WITH_CLOT_BLANK_Format = String.Format("- {0}, {1}: {2}, {3}: {4}, {5}: {6}", "{0}", lblSolution, "{1}", lblTest, "{2}", lblReplicateNumber, "{3}")
            'AG 25/07/2012

            R_NO_VOLUME_Format = String.Format("- {0}: {1}, {2}: {3}", lblReagent, "{0}", lblRotorPosition, "{1}")

            'TR 01/10/2012 -Reagent bottle locked 
            BOTTLE_LOCKED_Format = (String.Format("- {0}: {1}, {2}: {3}", lblReagent, "{0}", lblRotorPosition, "{1}"))
            'TR 01/10/2012 -END.

            bsTitleLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_ALARMS_LIST", LanguageID)

            SampleClassDict("BLANK") = myMultiLangResourcesDelegate.GetResourceText(Nothing, "PMD_SAMPLE_CLASSES_BLANK", LanguageID)
            SampleClassDict("CALIB") = myMultiLangResourcesDelegate.GetResourceText(Nothing, "PMD_SAMPLE_CLASSES_CALIB", LanguageID)
            SampleClassDict("CTRL") = myMultiLangResourcesDelegate.GetResourceText(Nothing, "PMD_SAMPLE_CLASSES_CTRL", LanguageID)
            SampleClassDict("PATIENT") = myMultiLangResourcesDelegate.GetResourceText(Nothing, "PMD_SAMPLE_CLASSES_PATIENT", LanguageID)

            BlankModeDict("SALINESOL") = myMultiLangResourcesDelegate.GetResourceText(Nothing, "PMD_SPECIAL_SOLUTIONS_SALINESOL", LanguageID)
            BlankModeDict("DISTW") = myMultiLangResourcesDelegate.GetResourceText(Nothing, "PMD_SPECIAL_SOLUTIONS_DISTW", LanguageID)

            'AG 16/11/2012 - Add case blank only with reagent
            'OptionA: This line shows ... Solution: Blank only with reagent
            BlankModeDict("REAGENT") = myMultiLangResourcesDelegate.GetResourceText(Nothing, "PMD_BLANK_MODES_REAGENT", LanguageID) 'Blank only with reagent
            'OptionB: This line shows ... Solution: --
            'BlankModeDict("REAGENT") = "--"
            'AG 16/11/2012 - add case blank only with reagent

            'RH 26/04/2012
            ERRORLabel = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_ERROR", LanguageID)
            WARNINGLabel = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_WARNING", LanguageID)
            SOLVEDLabel = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SOLVED", LanguageID)

            'RH 17/05/2012
            lblWashingSolution = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_RotorPos_WashingSol", LanguageID)
            lblSpecialSolution = myMultiLangResourcesDelegate.GetResourceText(Nothing, "PMD_TUBE_CONTENTS_SPEC_SOL", LanguageID)

            WASH_SOL_Format = String.Format("- {0}: {1}, {2}: {3}", lblWashingSolution, "{0}", lblRotorPosition, "{1}")
            SPEC_SOL_Format = String.Format("- {0}: {1}, {2}: {3}", lblSpecialSolution, "{0}", lblRotorPosition, "{1}")

            ''RH 18/05/2012 NOTE: Update when new SolutionCodes appear!!!
            'SolutionCodeDict("SALINESOL") = myMultiLangResourcesDelegate.GetResourceText(Nothing, "PMD_SPECIAL_SOLUTIONS_SALINESOL", LanguageID)
            'SolutionCodeDict("WASHSOL1") = myMultiLangResourcesDelegate.GetResourceText(Nothing, "PMD_WASHING_SOLUTIONS_WASHSOL1", LanguageID)
            'SolutionCodeDict("WASHSOL2") = myMultiLangResourcesDelegate.GetResourceText(Nothing, "PMD_WASHING_SOLUTIONS_WASHSOL2", LanguageID)
            'SolutionCodeDict("WASHSOL3") = myMultiLangResourcesDelegate.GetResourceText(Nothing, "PMD_TUBE_CONTENTS_TUBE_WASH_SOL", LanguageID)
            'SolutionCodeDict("WASHSOL4") = myMultiLangResourcesDelegate.GetResourceText(Nothing, "PMD_WASHING_SOLUTIONS_WASHSOL3", LanguageID)

            'SGM 06/09/2012 - Automation of filling Solution Codes dictionary
            Dim myPreloadedMasterDataDelegate As New PreloadedMasterDataDelegate

            'Dilution Solution Codes
            myGlobalDataTO = myPreloadedMasterDataDelegate.GetList(Nothing, GlobalEnumerates.PreloadedMasterDataEnum.DIL_SOLUTIONS)
            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                Dim myDilutionCodesMasterDataDS As PreloadedMasterDataDS = DirectCast(myGlobalDataTO.SetDatos, PreloadedMasterDataDS)

                For Each S As PreloadedMasterDataDS.tfmwPreloadedMasterDataRow In myDilutionCodesMasterDataDS.tfmwPreloadedMasterData
                    If (Not SolutionCodeDict.ContainsKey(S.ItemID)) Then
                        SolutionCodeDict(S.ItemID) = S.FixedItemDesc
                    End If
                Next
            End If

            'Special Solution Codes
            myGlobalDataTO = myPreloadedMasterDataDelegate.GetList(Nothing, GlobalEnumerates.PreloadedMasterDataEnum.SPECIAL_SOLUTIONS)
            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                Dim mySpecialCodesMasterDataDS As PreloadedMasterDataDS = DirectCast(myGlobalDataTO.SetDatos, PreloadedMasterDataDS)

                For Each S As PreloadedMasterDataDS.tfmwPreloadedMasterDataRow In mySpecialCodesMasterDataDS.tfmwPreloadedMasterData
                    If (Not SolutionCodeDict.ContainsKey(S.ItemID)) Then
                        SolutionCodeDict(S.ItemID) = S.FixedItemDesc
                    End If
                Next
            End If

            'Washing Solution Codes
            myGlobalDataTO = myPreloadedMasterDataDelegate.GetList(Nothing, GlobalEnumerates.PreloadedMasterDataEnum.WASHING_SOLUTIONS)
            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                Dim myWashingCodesMasterDataDS As PreloadedMasterDataDS = DirectCast(myGlobalDataTO.SetDatos, PreloadedMasterDataDS)

                For Each S As PreloadedMasterDataDS.tfmwPreloadedMasterDataRow In myWashingCodesMasterDataDS.tfmwPreloadedMasterData
                    If (Not SolutionCodeDict.ContainsKey(S.ItemID)) Then
                        SolutionCodeDict(S.ItemID) = S.FixedItemDesc
                    End If
                Next
            End If
            'END SGM 06/09/2012
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".GetAlarmsTabLabels ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".GetAlarmsTabLabels", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", MsgParent)
        End Try
    End Sub

#End Region

#Region "Events"

    ''' <summary>
    ''' Processes the mouse over event over XtraGridView
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>
    ''' Created by: RH 26/04/2012
    ''' </remarks>
    Private Sub ToolTipController1_GetActiveObjectInfo(ByVal sender As System.Object, ByVal e As ToolTipControllerGetActiveObjectInfoEventArgs) Handles ToolTipController1.GetActiveObjectInfo
        Try
            If Not e.SelectedControl Is AlarmsXtraGrid Then Return

            'Get the view at the current mouse position
            Dim view As GridView = AlarmsXtraGrid.GetViewAt(e.ControlMousePosition)
            If view Is Nothing Then Return

            'Get the view's element information that resides at the current position
            Dim hi As GridHitInfo = view.CalcHitInfo(e.ControlMousePosition)

            If hi.InRowCell Then
                'Dim o As Object = hi.HitTest.ToString() + hi.RowHandle.ToString()
                'Dim text As String = "Row " + hi.RowHandle.ToString()
                'info = New ToolTipControlInfo(o, text)

                'This is how to get the DataRow behind the GridViewRow
                Dim CurrentRow As WSAnalyzerAlarmsDS.vwksAlarmsMonitorRow
                CurrentRow = CType(view.GetDataRow(hi.RowHandle), WSAnalyzerAlarmsDS.vwksAlarmsMonitorRow)

                'RH 23/05/2012
                Select Case hi.Column.Name
                    Case "Type"
                        If Not CurrentRow.AlarmStatus Then
                            e.Info = New ToolTipControlInfo(SOLVEDLabel, SOLVEDLabel)
                        Else
                            Select Case CurrentRow.AlarmType
                                Case "ERROR"
                                    e.Info = New ToolTipControlInfo(ERRORLabel, ERRORLabel)

                                Case "WARNING"
                                    e.Info = New ToolTipControlInfo(WARNINGLabel, WARNINGLabel)

                            End Select
                        End If

                End Select

            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ToolTipController1_GetActiveObjectInfo ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")

        End Try
    End Sub

#End Region

End Class