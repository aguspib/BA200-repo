Option Strict On
Option Explicit On

'Imports System.Configuration
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Global.GlobalEnumerates
Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.CommunicationsSwFw
Imports System.Windows.Forms
Imports System.Drawing
Imports Biosystems.Ax00.App

Public Class UiBarCodesConfig

#Region "Attributes"
    Private AnalyzerIDAttribute As String = ""
    Private WorkSessionIDAttribute As String = String.Empty
#End Region

#Region "Properties"
    Public WriteOnly Property ActiveAnalyzer() As String
        Set(ByVal value As String)
            AnalyzerIDAttribute = value
        End Set
    End Property

    Public WriteOnly Property WorkSessionID() As String
        Set(ByVal value As String)
            WorkSessionIDAttribute = value
        End Set
    End Property
#End Region

#Region "Declarations"

    'For current application Language
    Private LanguageID As String

    'Typed DS to get and update data
    Private BarCodesUserSettingDS As UserSettingDS
    Private SamplesBarCodesConfigurationDS As BarCodesDS
    Private LocalBarCodeSampleTypesMappingDS As BarCodeSampleTypesMappingDS  'RH 02/09/2011 - Remove unneeded and memory wasting "New" instruction.

    'To control the Min/Max limits for NumericUpDown controls used to configure the Samples Barcode
    Private BoundMinValue As Decimal
    Private BoundMaxValue As Decimal
    Private CanApplyBoundRules As Boolean = False

    'To control the screen cannot be moved
    Private myTop As Integer
    Private myLeft As Integer

    'For Analyzer management
    'Private mdiAnalyzerCopy As AnalyzerManager 'DL 09/09/2011
    Private entryValueForReagentsBarcodeDisable As Boolean 'AG 23/11/2011
    Private entryValueForSamplesBarcodeDisable As Boolean 'AG 23/11/2011

    'To control if values have been changed and have to be saved
    Private EditionMode As Boolean 'TR 14/02/2012
    Private ChangesMade As Boolean 'TR 14/02/2012
    Private BarcodeConfigChangesToSend As Boolean ' XBC 14/02/2012

    Private ResetNotInUseRotorPosition As Boolean 'TR 

    Private BarcodeMaxSize As Integer 'TR 07/05/2013

    Private IsSamplesBarcodeDisabledByUser As Boolean = False 'SGM 12/07/2013

#End Region

#Region "Public Events"
    Public Event StopCurrentOperationFinished(ByVal pAlarmType As ManagementAlarmTypes) 'SGM 19/10/2012
#End Region

#Region "Methods"

    ''' <summary>
    ''' Updates the Barcodes Configuration values
    ''' </summary>
    ''' <remarks>
    ''' Created by:  RH 07/07/2011
    ''' Modified by: DL 22/07/2011 - Added updation of Analyzer Settings REAGENT_BARCODE_DISABLED and SAMPLE_BARCODE_DISABLED
    '''              AG 23/11/2011 - Sent the CONFIGURATION instruction with new values when: Analyzer is connected and ready and
    '''                              Analyzer Status is SLEEP or STANDBY
    '''             XBC 14/02/2012 - Sent the CONFIGURATION instruction only when changes have been made in Barcode configuration
    '''              SA 21/03/2012 - Do not sent the CONFIGURATION instruction if an error had happened in the setting updates
    '''              TR 07/03/2013 - Add the value for BARCODE_SAMPLEID_FLAG and save it.
    '''              TR 24/07/2013 - Set threading sleep for 250 after sending BARCODE REQUEST, wait for analyzer to answer.
    '''              XB+JC 09/10/2013 - Improve Usability #1273 Bugs tracking
    ''' </remarks>
    Private Sub AcceptSelection()
        Try
            Dim resultData As GlobalDataTO
            Dim myBarCodesDelegate As New BarCodeConfigDelegate()

            resultData = myBarCodesDelegate.UpdateSamplesBarCodesConfiguration(Nothing, SamplesBarCodesConfigurationDS)
            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                Dim myUserSettingsDelegate As New UserSettingsDelegate()

                For Each row As UserSettingDS.tcfgUserSettingsRow In BarCodesUserSettingDS.tcfgUserSettings

                    ' XB+JC 09/10/2013
                    'Select Case row.SettingID
                    '    Case UserSettingsEnum.BARCODE_EXTERNAL_END.ToString()
                    '        row.CurrentValue = bsExtIDEndNumericUpDown.Value.ToString()

                    '    Case UserSettingsEnum.BARCODE_EXTERNAL_INI.ToString()
                    '        row.CurrentValue = bsExtIDStartNumericUpDown.Value.ToString()

                    '    Case UserSettingsEnum.BARCODE_FULL_TOTAL.ToString()
                    '        'row.CurrentValue = bsFullTotalNumericUpDown.Value.ToString()
                    '        'TR 08/04/2013 -Replace control using a textBox instead a numeriUpDownBox.
                    '        'row.CurrentValue = bsFullTotalNumericUpDown.Text

                    '        row.CurrentValue = BarcodeMaxSize.ToString() 'TR 07/05/2013 -Use the variable instead the use of control.

                    '    Case UserSettingsEnum.BARCODE_SAMPLEID_FLAG.ToString()
                    '        row.CurrentValue = IIf(bsExternalIDCheckBox.Checked, 1, 0).ToString()

                    '    Case UserSettingsEnum.BARCODE_SAMPLETYPE_END.ToString()
                    '        row.CurrentValue = bsSampleTypeEndNumericUpDown.Value.ToString()

                    '    Case UserSettingsEnum.BARCODE_SAMPLETYPE_FLAG.ToString()
                    '        row.CurrentValue = IIf(bsSampleTypeCheckBox.Checked, 1, 0).ToString()

                    '    Case UserSettingsEnum.BARCODE_SAMPLETYPE_INI.ToString()
                    '        row.CurrentValue = bsSampleTypeStartNumericUpDown.Value.ToString()
                    'End Select
                    Select Case row.SettingID
                        Case UserSettingsEnum.BARCODE_EXTERNAL_END.ToString()
                            row.CurrentValue = bsExtIDEndNumericUpDown.Value.ToString()

                        Case UserSettingsEnum.BARCODE_EXTERNAL_INI.ToString()
                            row.CurrentValue = bsExtIDStartNumericUpDown.Value.ToString()

                        Case UserSettingsEnum.BARCODE_FULL_TOTAL.ToString()
                            'row.CurrentValue = bsFullTotalNumericUpDown.Value.ToString()
                            'TR 08/04/2013 -Replace control using a textBox instead a numeriUpDownBox.
                            'row.CurrentValue = bsFullTotalNumericUpDown.Text
                            row.CurrentValue = BarcodeMaxSize.ToString() 'TR 07/05/2013 -Use the variable instead the use of control.

                            ' JC 19/09/2013 Unified Settings.
                        Case UserSettingsEnum.BARCODE_SAMPLETYPE_FLAG.ToString(),
                             UserSettingsEnum.BARCODE_SAMPLEID_FLAG.ToString()
                            row.CurrentValue = IIf(bsExternalIDCheckBox.Checked, 1, 0).ToString()

                        Case UserSettingsEnum.BARCODE_SAMPLETYPE_END.ToString()
                            row.CurrentValue = bsSampleTypeEndNumericUpDown.Value.ToString()

                        Case UserSettingsEnum.BARCODE_SAMPLETYPE_INI.ToString()
                            row.CurrentValue = bsSampleTypeStartNumericUpDown.Value.ToString()
                    End Select
                    ' XB+JC 09/10/2013

                Next

                resultData = myUserSettingsDelegate.UpdateBarcodeSettings(Nothing, BarCodesUserSettingDS, LocalBarCodeSampleTypesMappingDS)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    Dim myAnalyzerSettingsDS As New AnalyzerSettingsDS
                    Dim myAnalyzerSettingsRow As AnalyzerSettingsDS.tcfgAnalyzerSettingsRow

                    'REAGENT_BARCODE_STATUS
                    myAnalyzerSettingsRow = myAnalyzerSettingsDS.tcfgAnalyzerSettings.NewtcfgAnalyzerSettingsRow
                    With myAnalyzerSettingsRow
                        .AnalyzerID = AnalyzerIDAttribute
                        .SettingID = GlobalEnumerates.AnalyzerSettingsEnum.REAGENT_BARCODE_DISABLED.ToString()
                        .CurrentValue = IIf(bsReagentsCheckBox.Checked, 1, 0).ToString()
                    End With
                    myAnalyzerSettingsDS.tcfgAnalyzerSettings.Rows.Add(myAnalyzerSettingsRow)

                    'SAMPLE_BARCODE_STATUS
                    myAnalyzerSettingsRow = myAnalyzerSettingsDS.tcfgAnalyzerSettings.NewtcfgAnalyzerSettingsRow
                    With myAnalyzerSettingsRow
                        .AnalyzerID = AnalyzerIDAttribute
                        .SettingID = GlobalEnumerates.AnalyzerSettingsEnum.SAMPLE_BARCODE_DISABLED.ToString()
                        .CurrentValue = IIf(bsSamplesCheckBox.Checked, 1, 0).ToString()
                    End With
                    myAnalyzerSettingsDS.tcfgAnalyzerSettings.Rows.Add(myAnalyzerSettingsRow)

                    Dim myAnalyzerSettings As New AnalyzerSettingsDelegate
                    resultData = myAnalyzerSettings.Save(Nothing, AnalyzerIDAttribute, myAnalyzerSettingsDS, Nothing)
                Else
                    'Error updating User Settings values
                    ShowMessage(Me.Name & ".AcceptSelection", resultData.ErrorCode, resultData.ErrorMessage, Me)
                End If
            Else
                'Error updating Barcodes Configuration values
                ShowMessage(Me.Name & ".AcceptSelection", resultData.ErrorCode, resultData.ErrorMessage, Me)
            End If

            'TR 26/04/2013 -In case changes on samples barcode then reset not in use positions on sample rotor.
            If ResetNotInUseRotorPosition Then
                Dim myBarcodeWSDelegate As New BarcodeWSDelegate
                resultData = myBarcodeWSDelegate.BarcodeConfigChangeActions(Nothing, AnalyzerIDAttribute, WorkSessionIDAttribute, GlobalEnumerates.Rotors.SAMPLES.ToString())
                ResetNotInUseRotorPosition = False
            End If

            'Send the CONFIGURATION instruction with new values when: Instrument is connecte and ready, and the Instrument Status is Sleep or StandBy 
            If (Not resultData.HasError) Then
                '#REFACTORING
                If (AnalyzerController.Instance.Analyzer.Connected AndAlso AnalyzerController.Instance.Analyzer.AnalyzerIsReady AndAlso _
                   (AnalyzerController.Instance.Analyzer.AnalyzerStatus = AnalyzerManagerStatus.SLEEPING OrElse AnalyzerController.Instance.Analyzer.AnalyzerStatus = AnalyzerManagerStatus.STANDBY)) Then
                    'Send the CONFIGURATION instruction if changes in Reagent or Sample Barcode activation have been made
                    If (entryValueForReagentsBarcodeDisable <> bsReagentsCheckBox.Checked OrElse entryValueForSamplesBarcodeDisable <> bsSamplesCheckBox.Checked) Then
                        resultData = AnalyzerController.Instance.Analyzer.ManageAnalyzer(AnalyzerManagerSwActionList.CONFIG, True, Nothing, Nothing)
                    End If

                    If (Me.BarcodeConfigChangesToSend) Then
                        Me.BarcodeConfigChangesToSend = False
                        Application.DoEvents()
                        System.Threading.Thread.Sleep(500)

                        Dim BarCodeDS As New AnalyzerManagerDS
                        Dim rowBarCode As AnalyzerManagerDS.barCodeRequestsRow
                        rowBarCode = BarCodeDS.barCodeRequests.NewbarCodeRequestsRow
                        With rowBarCode
                            .RotorType = "SAMPLES"
                            .Action = GlobalEnumerates.Ax00CodeBarAction.CONFIG
                            .Position = 0
                        End With
                        BarCodeDS.barCodeRequests.AddbarCodeRequestsRow(rowBarCode)
                        BarCodeDS.AcceptChanges()

                        resultData = AnalyzerController.Instance.Analyzer.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.BARCODE_REQUEST, True, Nothing, BarCodeDS) '#REFACTORING
                        'TR 24/07/2013 Bug #1245  When screen close does not become enable 
                        'start button due analyzer does not respond on time. wait before close 
                        System.Threading.Thread.Sleep(250)

                    End If

                    If (resultData.HasError) Then
                        ShowMessage(Me.Name & ".AcceptSelection", resultData.ErrorCode, resultData.ErrorMessage, Me)
                    End If
                End If

                Me.Enabled = False 'RH 13/04/2012 Disable the form to avoid pressing buttons on closing

                Me.Close()
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".AcceptSelection", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".AcceptSelection", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Applies bound rules to the NumericUpDown controls (Minimum and Maximum values)
    ''' </summary>
    ''' <remarks>
    ''' Created by:  RH 11/07/2011
    ''' Modified by: TR 08/04/2013 -Replace control bsFullTotalNumericUpDown using a textBox instead a numeriUpDownBox.
    '''              XB+JC 09/10/2013 - Improve Usability #1273 Bugs tracking
    ''' </remarks>
    Private Sub ApplyBoundRules()
        Try
            If (Not CanApplyBoundRules) Then Return

            ' XB+JC 09/10/2013 
            'If (bsSampleTypeCheckBox.Checked) Then
            If (bsExternalIDCheckBox.Checked) Then
                ' XB+JC 09/10/2013 

                bsExtIDStartNumericUpDown.Minimum = BoundMinValue
                bsExtIDStartNumericUpDown.Maximum = bsExtIDEndNumericUpDown.Value

                bsExtIDEndNumericUpDown.Minimum = bsExtIDStartNumericUpDown.Value
                'bsExtIDEndNumericUpDown.Maximum = bsFullTotalNumericUpDown.Value
                bsExtIDEndNumericUpDown.Maximum = BarcodeMaxSize


                bsSampleTypeStartNumericUpDown.Minimum = BoundMinValue

                bsSampleTypeEndNumericUpDown.Minimum = bsSampleTypeStartNumericUpDown.Value
                'bsSampleTypeEndNumericUpDown.Maximum = bsFullTotalNumericUpDown.Value
                bsSampleTypeEndNumericUpDown.Maximum = BarcodeMaxSize
                'bsSampleTypeStartNumericUpDown.Maximum = bsFullTotalNumericUpDown.Value 'TR 13/03/2013 -Set the maximum.
                bsSampleTypeStartNumericUpDown.Maximum = BarcodeMaxSize
            Else
                bsExtIDStartNumericUpDown.Minimum = BoundMinValue
                bsExtIDStartNumericUpDown.Maximum = bsExtIDEndNumericUpDown.Value

                bsExtIDEndNumericUpDown.Minimum = bsExtIDStartNumericUpDown.Value
                'bsExtIDEndNumericUpDown.Maximum = bsFullTotalNumericUpDown.Value
                bsExtIDEndNumericUpDown.Maximum = BarcodeMaxSize
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ApplyBoundRules", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ApplyBoundRules", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Verify if there are duplicated SampleType Laboratory code in the mapping grid
    ''' </summary>
    ''' <returns>True if there are duplicated codes; otherwise, False</returns>
    ''' <remarks>
    ''' Created by: TR 31/08/2011
    ''' </remarks>
    Private Function DuplicatedLaboratoryCode(ByVal pSampleType As String, ByVal pLaboratoryCode As String) As Boolean
        Dim myResult As Boolean = False
        Try
            For Each myRow As DataGridViewRow In bsSamplesTypesGridView.Rows
                If (myRow.Cells("SampleType").Value.ToString() <> pSampleType) Then
                    If (Not myRow.Cells("ExternalSampleType").Value.ToString() = String.Empty AndAlso _
                        myRow.Cells("ExternalSampleType").Value.ToString() = pLaboratoryCode) Then
                        myResult = True
                        Exit For
                    End If
                End If
            Next
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".DuplicatedLaboratoryCode", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".DuplicatedLaboratoryCode", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
        Return myResult
    End Function

    ''' <summary>
    ''' Gets texts in the current application language for all screen controls
    ''' </summary>
    ''' <remarks>
    ''' Created by:  RH 18/07/2011
    ''' Modified by: SA 21/03/2012 - Get and assign the multilanguage text for GroupBox bsBarcodeTypesGroupBox
    '''              XB+JC 09/10/2013 - Improve Usability #1273 Bugs tracking
    ''' </remarks>
    Private Sub GetScreenLabels()
        Try
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            'Labels in REAGENTS BARCODE area
            bsReagentsLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Barcodes_Reagents", LanguageID)
            bsReagentsCheckBox.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Barcodes_Reagents_CBox", LanguageID)

            'Labels in SAMPLES BARCODE area
            bsSamplesLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Barcodes_Samples", LanguageID)
            bsSamplesCheckBox.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Barcodes_Samples_CBox", LanguageID)
            bsBarcodeTypesGroupBox.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Barcode_Types", LanguageID)

            'bsBarCodeConfiGroupBox.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Barcode_Config", LanguageID)

            bsStartLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Barcodes_Start", LanguageID) & ":"
            bsEndLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Barcodes_End", LanguageID) & ":"
            bsTotalLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Total", LanguageID) & ":"

            'TR 08/04/2013 -comment label load because labels need validation.
            bsExternalIDCheckBox.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Barcode_Field_Activ", LanguageID)
            BarcodeTypesLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Barcode_Type", LanguageID)
            MaxBarcodeSizeLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Barcode_Size", LanguageID) & ":"

            bsExtenalIDLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_RotorPos_SampleID", LanguageID)

            'bsExternalIDCheckBox.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Barcodes_ExternalID", LanguageID)

            ' XB+JC 09/10/2013 
            'bsSampleTypeCheckBox.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SampleType", LanguageID)
            bsSampleTypeLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SampleType", LanguageID)
            ' XB+JC 09/10/2013 

            'Button Tooltips
            bsScreenToolTips.SetToolTip(bsAcceptButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Save&Close", LanguageID))
            bsScreenToolTips.SetToolTip(bsExitButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Cancel&Close", LanguageID))
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".GetScreenLabels", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".GetScreenLabels", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Initializes the DataGridView for Samples Barcodes Types
    ''' </summary>
    ''' <remarks>
    ''' Created by:  RH 07/07/2011
    ''' Modified by: TR 11/10/2011 - Set property SortMode = NotSortable for all grid columns
    '''              DL 25/11/2011 - Removed column "CheckValue" 
    '''                              (\\rdsoftware\RdSoftware\Development\Sw Ax00\USER Sw v1.0.0\DOC\Revision EF\Actividades DL\2- Cambio pantalla Configuracion Barcode)
    '''              SA 21/03/2012 - Changed label of column "Name" (from Barcode Type to Type)
    ''' </remarks>
    Private Sub InitializeBarCodeTypesGrid()
        Try
            Dim columnName As String
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            bsSamplesBarcodeTypesDataGridView.AutoGenerateColumns = False

            'Barcode Type Status column
            columnName = "Status"
            Dim checkBoxColumnStatus As New DataGridViewCheckBoxColumn
            checkBoxColumnStatus.Name = columnName
            checkBoxColumnStatus.HeaderText = String.Empty

            bsSamplesBarcodeTypesDataGridView.Columns.Add(checkBoxColumnStatus)
            bsSamplesBarcodeTypesDataGridView.Columns(columnName).Width = 50
            bsSamplesBarcodeTypesDataGridView.Columns(columnName).DataPropertyName = columnName
            bsSamplesBarcodeTypesDataGridView.Columns(columnName).ReadOnly = False
            bsSamplesBarcodeTypesDataGridView.Columns(columnName).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            bsSamplesBarcodeTypesDataGridView.Columns(columnName).SortMode = DataGridViewColumnSortMode.NotSortable

            'Barcode Type column
            columnName = "Name"
            bsSamplesBarcodeTypesDataGridView.Columns.Add(columnName, myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Type", LanguageID))
            bsSamplesBarcodeTypesDataGridView.Columns(columnName).DataPropertyName = columnName
            bsSamplesBarcodeTypesDataGridView.Columns(columnName).ReadOnly = True
            bsSamplesBarcodeTypesDataGridView.Columns(columnName).Width = 250
            bsSamplesBarcodeTypesDataGridView.Columns(columnName).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
            bsSamplesBarcodeTypesDataGridView.Columns(columnName).SortMode = DataGridViewColumnSortMode.NotSortable
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".InitializeBarCodeTypesGrid", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".InitializeBarCodeTypesGrid", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", Me)

        End Try
    End Sub

    ''' <summary>
    ''' Initialize the DataGridView for SampleTypes mapping 
    ''' </summary>
    ''' <remarks>
    ''' Created by:  TR 30/08/2011
    ''' Modified by: TR 11/10/2011 - Set property SortMode = NotSortable for all grid columns
    ''' </remarks>
    Private Sub InitializeSampleTypesGrid()
        Try
            Dim columnName As String
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            bsSamplesTypesGridView.Columns.Clear()
            bsSamplesTypesGridView.AutoGenerateColumns = False

            'SampleType Code + Description column
            columnName = "CodeDesc"
            bsSamplesTypesGridView.Columns.Add(columnName, myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SampleType", LanguageID))
            bsSamplesTypesGridView.Columns(columnName).DataPropertyName = columnName
            bsSamplesTypesGridView.Columns(columnName).ReadOnly = True
            bsSamplesTypesGridView.Columns(columnName).Width = 200
            bsSamplesTypesGridView.Columns(columnName).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
            bsSamplesTypesGridView.Columns(columnName).SortMode = DataGridViewColumnSortMode.NotSortable

            'SampleType Laboratory Code column
            columnName = "ExternalSampleType"
            Dim ExternalSampleType As New DataGridViewTextBoxColumn()
            ExternalSampleType.Name = columnName
            ExternalSampleType.MaxInputLength = CInt(bsSampleTypeTotalTextBox.Text) 'Set the maximum number of allowed characters
            ExternalSampleType.HeaderText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Laboratory_Code", LanguageID)

            bsSamplesTypesGridView.Columns.Add(ExternalSampleType)
            bsSamplesTypesGridView.Columns(columnName).DataPropertyName = columnName
            bsSamplesTypesGridView.Columns(columnName).ReadOnly = False
            bsSamplesTypesGridView.Columns(columnName).Width = 217
            bsSamplesTypesGridView.Columns(columnName).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
            bsSamplesTypesGridView.Columns(columnName).SortMode = DataGridViewColumnSortMode.NotSortable

            'SampleType Code column (hide)
            columnName = "SampleType"
            bsSamplesTypesGridView.Columns.Add(columnName, "")
            bsSamplesTypesGridView.Columns(columnName).DataPropertyName = columnName
            bsSamplesTypesGridView.Columns(columnName).ReadOnly = True
            bsSamplesTypesGridView.Columns(columnName).Width = 150
            bsSamplesTypesGridView.Columns(columnName).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
            bsSamplesTypesGridView.Columns(columnName).Visible = False
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".InitializeSampleTypesGrid", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".InitializeSampleTypesGrid", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Initializes the screen when loading
    ''' </summary>
    ''' <remarks>
    ''' Created by:  RH 06/07/2011
    ''' Modified by: DL 28/07/2011 - Added code to center the screen regarding the main MDI form
    '''              TR 30/08/2011 - Added functions to initialize and load the DataGridView containing the list of Sample Types mapping
    '''              DL 09/09/2011 - Added code to get the Analyzer Manager and disable all fields when the Analyzer status is RUNNING
    '''              AG 06/02/2012 - Before verify if the Analyzer status is RUNNING, verify if it is connected
    '''              TR 13/03/2012 - Added function SetLimits to set Min/Max allowed values for UpDown controls
    '''              SA 06/09/2012 - Added code to disable all screen fields and the Accept Button also when the Analyzer connection is
    '''                              in process
    '''              XB+JC 09/10/2013 - Improve Usability #1273 Bugs tracking
    '''              XB 06/11/2013 - Add protection against more performing operations (Starting Instrument, Shutting down, aborting WS) - BT #1150 + #1151
    '''              IT 23/10/2014 - REFACTORING (BA-2016)
    ''' </remarks>
    Private Sub InitializeScreen()
        Try
            'Center the screen
            Dim mySize As Size = Me.Parent.Size
            Dim myLocation As Point = Me.Parent.Location
            Me.Location = New Point(myLocation.X + CInt((mySize.Width - Me.Width) / 2), myLocation.Y + CInt((mySize.Height - Me.Height) / 2) - 60)

            'Get the current Language from the current Application Session
            'Dim currentLanguageGlobal As New GlobalBase
            LanguageID = GlobalBase.GetSessionInfo().ApplicationLanguage

            'Get Icons for Form Buttons
            PrepareButtons()

            'Set multilanguage texts for all form labels and tooltips...
            GetScreenLabels()

            'Get and set Min/Max allowed values for UpDown control for the total size of Samples Barcode
            SetLimits()

            'Configure and load the DataGridView of Barcode Types in area of Samples Barcode
            InitializeBarCodeTypesGrid()
            LoadBarCodesConfiguration()

            'Get and set Min/Max allowed values for controls Start/End for ExternalID and SampleType 
            LoadBarCodeBounds() 'Execute this method before LoadBarCodeUserSettings()
            LoadBarCodeUserSettings()

            'Configure and load the DataGridView of Sample Types mapping in area of Samples Barcode
            InitializeSampleTypesGrid()
            LoadBarCodeSampleTypesGrid()

            'TR 08/04/2013 -Set the backgound and ForeColor to control on readmode  status
            bsExtIDTotalTextBox.BackColor = SystemColors.MenuBar
            bsSampleTypeTotalTextBox.BackColor = SystemColors.MenuBar
            'bsFullTotalNumericUpDown.BackColor = SystemColors.MenuBar

            bsExtIDTotalTextBox.ForeColor = Color.DarkGray
            bsSampleTypeTotalTextBox.ForeColor = Color.DarkGray
            'bsFullTotalNumericUpDown.ForeColor = Color.DarkGray

            'bsSampleTypeCheckBox.Enabled = bsExternalIDCheckBox.Checked    XB+JC 09/10/2013 
            'TR 08/04/2013 -END.

            ScreenStatusByUserLevel() 'TR 20/04/2012

            'Verify if the screen has to be opened in READ-ONLY mode
            '#REFACTORING
            If (AnalyzerController.IsAnalyzerInstantiated) Then
                'If the connection process is in process, disable all screen fields (changes are not allowed) ORELSE
                'If the Analyzer is connected and its status is RUNNING, disable all screen fields (changes are not allowed)
                If (AnalyzerController.Instance.Analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.CONNECTprocess) = "INPROCESS") OrElse _
                   (AnalyzerController.Instance.Analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.ABORTprocess) = "INPROCESS") OrElse _
                   (AnalyzerController.Instance.Analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.SDOWNprocess) = "INPROCESS") OrElse _
                   Not AnalyzerController.Instance.Analyzer.AnalyzerIsReady OrElse _
                   (AnalyzerController.Instance.Analyzer.Connected AndAlso AnalyzerController.Instance.Analyzer.AnalyzerStatus = GlobalEnumerates.AnalyzerManagerStatus.RUNNING) Then
                    ' XB 06/11/2013 - WUPprocess, ABORTprocess and SDOWNprocess added 
                    bsReagentsGroupBox.Enabled = False
                    bsSamplesGroupBox.Enabled = False

                    bsAcceptButton.Enabled = False
                End If
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".InitializeScreen ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".InitializeScreen", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Loads the Samples Barcodes Bound Minimun and Maximum values
    ''' </summary>
    ''' <remarks>
    ''' Created by: RH 14/07/2011
    ''' AG 10/12/2014 BA-2168 Limit BARCODE_LIMIT is defined by model. So read it informing the model
    ''' </remarks>
    Private Sub LoadBarCodeBounds()
        Try
            Dim resultData As GlobalDataTO = Nothing
            Dim myFieldLimitsDelegate As New FieldLimitsDelegate

            'AG 10/12/2014 BA-2168
            'resultData = myFieldLimitsDelegate.GetList(Nothing, GlobalEnumerates.FieldLimitsEnum.BARCODE_LIMIT)
            resultData = myFieldLimitsDelegate.GetList(Nothing, GlobalEnumerates.FieldLimitsEnum.BARCODE_LIMIT, AnalyzerModel)

            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                Dim myFieldLimitsDS As FieldLimitsDS = DirectCast(resultData.SetDatos, FieldLimitsDS)

                If (myFieldLimitsDS.tfmwFieldLimits.Rows.Count = 1) Then
                    BoundMinValue = Convert.ToDecimal(myFieldLimitsDS.tfmwFieldLimits(0).MinValue)
                    BoundMaxValue = Convert.ToDecimal(myFieldLimitsDS.tfmwFieldLimits(0).MaxValue)
                Else
                    'Barcode Limit not found, show error Master Data Missing
                    ShowMessage(Me.Name & ".LoadBarCodeBounds ", GlobalEnumerates.Messages.MASTER_DATA_MISSING.ToString)
                End If
            Else
                'Error getting Barcodes Bound values
                ShowMessage(Me.Name & ".LoadBarCodeBounds", resultData.ErrorCode, resultData.ErrorMessage, Me)
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".LoadBarCodeBounds", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".LoadBarCodeBounds", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Load the Barcode Sample Types Grid
    ''' </summary>
    ''' <remarks>
    ''' Created by:  TR 30/08/2011
    ''' Modified by: RH 02/09/2011 - Remove unneeded and memory wasting "New" instructions
    ''' </remarks>
    Private Sub LoadBarCodeSampleTypesGrid()
        Try
            Dim myGlobalDataTO As GlobalDataTO
            Dim myBarCodeSampleTypesMappingDelegate As New BarCodeSampleTypesMappingDelegate

            myGlobalDataTO = myBarCodeSampleTypesMappingDelegate.ReadAll(Nothing)
            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                LocalBarCodeSampleTypesMappingDS = DirectCast(myGlobalDataTO.SetDatos, BarCodeSampleTypesMappingDS)
                bsSamplesTypesGridView.DataSource = LocalBarCodeSampleTypesMappingDS.tcfgBarCodeSampleTypesMapping
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".LoadBarCodeSampleTypesGrid", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".LoadBarCodeSampleTypesGrid", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Get current values of Reagents/Sample Barcode activation status and updates the CheckBoxes 
    ''' Get the list of available Barcode Types for Samples with their current activation status and 
    ''' load the corresponding DataGridView
    ''' </summary>
    ''' <remarks>
    ''' Created by:  RH 07/07/2011
    ''' Modified by: DL 22/07/2011 - Get current values of Analyzer Settings
    '''              SA 01/08/2011 - Analyzer Settings always exist in DB (if not it is an error); removed creation of them 
    '''              AG 23/11/2011 - Save values of Analyzer Settings currently saved in BD in global variables
    ''' </remarks>
    Private Sub LoadBarCodesConfiguration()
        Dim valuesOK As Boolean = True

        Try
            Dim resultData As GlobalDataTO = Nothing

            'Get Analyzer Settings: REAGENT_BARCODE_DISABLED and SAMPLE_BARCODE_DISABLED
            Dim myAnalyzerSettingsDS As New AnalyzerSettingsDS
            Dim myAnalyzerSettingsDelegate As New AnalyzerSettingsDelegate

            resultData = myAnalyzerSettingsDelegate.GetAnalyzerSetting(Nothing, AnalyzerIDAttribute, GlobalEnumerates.AnalyzerSettingsEnum.REAGENT_BARCODE_DISABLED.ToString())
            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                myAnalyzerSettingsDS = DirectCast(resultData.SetDatos, AnalyzerSettingsDS)

                If (myAnalyzerSettingsDS.tcfgAnalyzerSettings.Rows.Count = 1) Then
                    bsReagentsCheckBox.Checked = CType(myAnalyzerSettingsDS.tcfgAnalyzerSettings.First.CurrentValue, Boolean)
                Else
                    'The Analyzer Setting does not exist, shown error Master Data Missing 
                    valuesOK = False
                    ShowMessage(Me.Name & ".LoadBarCodesConfiguration ", GlobalEnumerates.Messages.MASTER_DATA_MISSING.ToString)
                End If
            Else
                'Error getting the Analyzer Setting value, show it 
                valuesOK = False
                ShowMessage(Me.Name & ".LoadBarCodesConfiguration ", resultData.ErrorCode, resultData.ErrorMessage)
            End If

            If (valuesOK) Then
                resultData = myAnalyzerSettingsDelegate.GetAnalyzerSetting(Nothing, AnalyzerIDAttribute, GlobalEnumerates.AnalyzerSettingsEnum.SAMPLE_BARCODE_DISABLED.ToString())
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    myAnalyzerSettingsDS = DirectCast(resultData.SetDatos, AnalyzerSettingsDS)

                    If (myAnalyzerSettingsDS.tcfgAnalyzerSettings.Rows.Count = 1) Then
                        bsSamplesCheckBox.Checked = CType(myAnalyzerSettingsDS.tcfgAnalyzerSettings.First.CurrentValue, Boolean)
                    Else
                        'The Analyzer Setting does not exist, shown error Master Data Missing 
                        valuesOK = False
                        ShowMessage(Me.Name & ".LoadBarCodesConfiguration ", GlobalEnumerates.Messages.MASTER_DATA_MISSING.ToString)
                    End If
                Else
                    'Error getting the Analyzer Setting value, show it 
                    valuesOK = False
                    ShowMessage(Me.Name & ".LoadBarCodesConfiguration ", resultData.ErrorCode, resultData.ErrorMessage)
                End If
            End If

            If (valuesOK) Then
                'Save values currently saved in BD in global variables
                entryValueForReagentsBarcodeDisable = bsReagentsCheckBox.Checked
                entryValueForSamplesBarcodeDisable = bsSamplesCheckBox.Checked

                'Get available BarCode Types for Samples and the current Status of each one of them (On/Off)
                Dim myBarCodesDelegate As New BarCodeConfigDelegate()

                resultData = myBarCodesDelegate.GetSamplesBarCodesConfiguration(Nothing)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    SamplesBarCodesConfigurationDS = DirectCast(resultData.SetDatos, BarCodesDS)

                    If (SamplesBarCodesConfigurationDS.vcfgSamplesBarCodesConfiguration.Rows.Count > 0) Then
                        bsSamplesBarcodeTypesDataGridView.DataSource = SamplesBarCodesConfigurationDS.vcfgSamplesBarCodesConfiguration
                    Else
                        'Barcode Types configuration was not found, shown error Master Data Missing 
                        ShowMessage(Me.Name & ".LoadBarCodesConfiguration ", GlobalEnumerates.Messages.MASTER_DATA_MISSING.ToString)
                    End If
                Else
                    'Error getting Barcode Types Configuration values, show it
                    ShowMessage(Me.Name & ".LoadBarCodesConfiguration", resultData.ErrorCode, resultData.ErrorMessage, Me)
                End If
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".LoadBarCodesConfiguration", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".LoadBarCodesConfiguration", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Get current values for Samples Barcode configuration and updates the corresponding screen controls
    ''' </summary>
    ''' <remarks>
    ''' Created by: RH 08/07/2011
    ''' Modified by: TR 07/03/2013 -Load the value for BARCODE_SAMPLEID_FLAG into bsExternalIDCheckBox
    '''              TR 08/04/2013 -Replace control bsFullTotalNumericUpDown using a textBox instead a numeriUpDownBox.
    '''              XB+JC 09/10/2013 - Improve Usability #1273 Bugs tracking
    ''' </remarks>
    Private Sub LoadBarCodeUserSettings()
        Try
            Dim resultData As GlobalDataTO = Nothing
            Dim myUserSettingsDelegate As New UserSettingsDelegate()

            resultData = myUserSettingsDelegate.ReadBarcodeSettings(Nothing)
            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                BarCodesUserSettingDS = DirectCast(resultData.SetDatos, UserSettingDS)

                CanApplyBoundRules = False
                For Each row As UserSettingDS.tcfgUserSettingsRow In BarCodesUserSettingDS.tcfgUserSettings
                    Dim CurrentValue As Decimal = Decimal.Parse(row.CurrentValue)

                    ' XB+JC 09/10/2013
                    'Select Case row.SettingID
                    '    Case UserSettingsEnum.BARCODE_FULL_TOTAL.ToString()
                    '        'TR 07/05/2013 -Change the object use the label instead a control.
                    '        MaxBarcodeSizeLabel.Text = MaxBarcodeSizeLabel.Text & " " & CurrentValue.ToString()
                    '        BarcodeMaxSize = CInt(CurrentValue) 'TR 07/05/2013 -Set the value to global variable for previous use.

                    '    Case UserSettingsEnum.BARCODE_EXTERNAL_INI.ToString()
                    '        bsExtIDStartNumericUpDown.Value = CurrentValue

                    '    Case UserSettingsEnum.BARCODE_EXTERNAL_END.ToString()
                    '        bsExtIDEndNumericUpDown.Value = CurrentValue

                    '    Case UserSettingsEnum.BARCODE_SAMPLEID_FLAG.ToString()
                    '        bsExternalIDCheckBox.Checked = (CurrentValue > 0)

                    '    Case UserSettingsEnum.BARCODE_SAMPLETYPE_FLAG.ToString()
                    '        bsSampleTypeCheckBox.Checked = (CurrentValue > 0)

                    '    Case UserSettingsEnum.BARCODE_SAMPLETYPE_INI.ToString()
                    '        bsSampleTypeStartNumericUpDown.Value = CurrentValue

                    '    Case UserSettingsEnum.BARCODE_SAMPLETYPE_END.ToString()
                    '        bsSampleTypeEndNumericUpDown.Value = CurrentValue

                    'End Select
                    Select Case row.SettingID
                        Case UserSettingsEnum.BARCODE_FULL_TOTAL.ToString()
                            'TR 07/05/2013 -Change the object use the label instead a control.
                            MaxBarcodeSizeLabel.Text = MaxBarcodeSizeLabel.Text & " " & CurrentValue.ToString()
                            BarcodeMaxSize = CInt(CurrentValue) 'TR 07/05/2013 -Set the value to global variable for previous use.

                        Case UserSettingsEnum.BARCODE_EXTERNAL_INI.ToString()
                            bsExtIDStartNumericUpDown.Value = CurrentValue

                        Case UserSettingsEnum.BARCODE_EXTERNAL_END.ToString()
                            bsExtIDEndNumericUpDown.Value = CurrentValue

                        Case UserSettingsEnum.BARCODE_SAMPLEID_FLAG.ToString()
                            'Case UserSettingsEnum.BARCODE_SAMPLETYPE_FLAG.ToString()
                            bsExternalIDCheckBox.Checked = (CurrentValue > 0)

                        Case UserSettingsEnum.BARCODE_SAMPLETYPE_INI.ToString()
                            bsSampleTypeStartNumericUpDown.Value = CurrentValue

                        Case UserSettingsEnum.BARCODE_SAMPLETYPE_END.ToString()
                            bsSampleTypeEndNumericUpDown.Value = CurrentValue


                    End Select
                    ' XB+JC 09/10/2013

                Next

                bsExtIDTotalTextBox.Text = (bsExtIDEndNumericUpDown.Value + 1 - bsExtIDStartNumericUpDown.Value).ToString()

                ' XB+JC 09/10/2013
                'If (bsSampleTypeCheckBox.Checked) Then
                If (bsExternalIDCheckBox.Checked) Then
                    ' XB+JC 09/10/2013

                    bsSampleTypeTotalTextBox.Text = (bsSampleTypeEndNumericUpDown.Value + 1 - bsSampleTypeStartNumericUpDown.Value).ToString()
                Else
                    bsSampleTypeTotalTextBox.Text = "0"
                End If

                CanApplyBoundRules = True
                ApplyBoundRules()
            Else
                'Error getting Barcodes Configuration values
                ShowMessage(Me.Name & ".LoadBarCodeUserSettings", resultData.ErrorCode, resultData.ErrorMessage, Me)
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".LoadBarCodeUserSettings", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".LoadBarCodeUserSettings", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Search Icons for all graphical screen buttons
    ''' </summary>
    ''' <remarks>
    ''' Created by:  RH 06/07/2011
    ''' </remarks>
    Private Sub PrepareButtons()
        Try
            Dim auxIconName As String = ""
            Dim iconPath As String = MyBase.IconsPath

            'ACCEPT Button
            auxIconName = GetIconName("ACCEPT1")
            If (auxIconName <> "") Then
                bsAcceptButton.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
            End If

            'CANCEL Button
            auxIconName = GetIconName("CANCEL")
            If (auxIconName <> "") Then
                bsExitButton.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " PrepareButtons ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & " PrepareButtons ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Get and set Min/Max allowed values for UpDown control for the total size of Samples Barcode
    ''' </summary>
    ''' <remarks>
    ''' Created by:  TR 13/03/2012
    ''' </remarks>
    Private Sub SetLimits()
        Try
            'TR 08/04/2013 -Replace control using a textBox instead a numeriUpDownBox.
            'Dim myGlobalDataTO As GlobalDataTO = Nothing
            'Dim myFieldLimitsDelegate As New FieldLimitsDelegate

            'myGlobalDataTO = myFieldLimitsDelegate.GetList(Nothing, FieldLimitsEnum.SAMPLE_BARCODE_SIZE_LIMIT)
            'If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
            '    Dim myFieldLimitsDS As FieldLimitsDS = DirectCast(myGlobalDataTO.SetDatos, FieldLimitsDS)

            '    If (myFieldLimitsDS.tfmwFieldLimits.Count > 0) Then
            '        'bsFullTotalNumericUpDown.Minimum = CType(myFieldLimitsDS.tfmwFieldLimits(0).MinValue, Decimal)
            '        'bsFullTotalNumericUpDown.Maximum = CType(myFieldLimitsDS.tfmwFieldLimits(0).MaxValue, Decimal)
            '        'If (Not myFieldLimitsDS.tfmwFieldLimits(0).IsStepValueNull) Then bsFullTotalNumericUpDown.Increment = CType(myFieldLimitsDS.tfmwFieldLimits(0).StepValue, Decimal)
            '    End If
            'Else
            '    ShowMessage(Me.Name & ".SetLimits ", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage, Me)
            'End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".SetLimits", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".SetLimits", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Validate if the length of the informed SampleType Laboratory Code is correct 
    ''' </summary>
    ''' <returns>True when the length is correct; otherwise, False</returns>
    ''' <remarks>
    ''' Created by: TR 31/08/2011
    ''' </remarks>
    Private Function ValidLaboratoryCodeLenght() As Boolean
        Dim myResult As Boolean = True
        Try
            'Validate if atleast there's one informed laboratory code
            Dim laboratoryCodeInformed As Boolean = False

            For Each myRow As DataGridViewRow In bsSamplesTypesGridView.Rows
                If (Not myRow.Cells("ExternalSampleType").Value.ToString = String.Empty AndAlso _
                    myRow.Cells("ExternalSampleType").Value.ToString().Length <> CInt(bsSampleTypeTotalTextBox.Text)) Then
                    'Length is not valid; shown the Error Message in the cell
                    myRow.Cells("ExternalSampleType").ErrorText = GetMessageText(GlobalEnumerates.Messages.WRONG_CODE_SIZE.ToString(), LanguageID)
                    myResult = False
                End If

                If (Not myRow.Cells("ExternalSampleType").Value.ToString = String.Empty) Then
                    'If at least one SampleType has been mapped, set value (there's one informed then set value = True 
                    If (Not laboratoryCodeInformed) Then laboratoryCodeInformed = True
                End If
            Next myRow

            'If there are not mapped SampleTypes, shown an Error Message: if SampleType is included in Samples Barcode, the Laboratory Code 
            'for at least one Sample Type has to be informed
            bsScreenErrorProvider.Clear()
            If (Not laboratoryCodeInformed AndAlso bsSamplesTypesGridView.Rows.Count > 0) Then
                bsScreenErrorProvider.SetError(bsSamplesTypesGridView, GetMessageText(GlobalEnumerates.Messages.ONE_LAB_CODE.ToString(), LanguageID))
                myResult = False
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".ValidLaboratoryCodeLenght", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ValidLaboratoryCodeLenght", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
        Return myResult
    End Function

    ''' <summary>
    ''' Not allow move form and mantain the center location in center parent
    ''' </summary>
    ''' <remarks>
    ''' Created by: DL 27/07/2011
    ''' </remarks>
    Protected Overrides Sub WndProc(ByRef m As Message)
        Try
            If (m.Msg = WM_WINDOWPOSCHANGING) Then
                Dim pos As WINDOWPOS = DirectCast(Runtime.InteropServices.Marshal.PtrToStructure(m.LParam, GetType(WINDOWPOS)), WINDOWPOS)
                Dim myLocation As Point = Me.Parent.Location
                Dim mySize As Size = Me.Parent.Size

                pos.x = myLocation.X + CInt((mySize.Width - Me.Width) / 2)
                pos.y = myLocation.Y + CInt((mySize.Height - Me.Height) / 2) - 60
                Runtime.InteropServices.Marshal.StructureToPtr(pos, m.LParam, True)
            End If

            MyBase.WndProc(m)
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WndProc " & Me.Name, EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & "WndProc", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Enable/disable functionality depending of level assigned to the connected User
    ''' </summary>
    ''' <remarks>
    ''' Created by: TR 23/04/2012
    ''' Modified by XB 04/02/2013 - Upper conversions redundants because the value is already in UpperCase must delete to avoid Regional Settings problems (Bugs tracking #1112)
    ''' </remarks>
    Private Sub ScreenStatusByUserLevel()
        Try
            Select Case CurrentUserLevel    '.ToUpper()
                Case "SUPERVISOR"
                    Exit Select

                Case "OPERATOR"
                    bsReagentsGroupBox.Enabled = False
                    bsSamplesGroupBox.Enabled = False
                    bsAcceptButton.Enabled = False
                    Exit Select
            End Select

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & "ScreenStatusByUserLevel ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & "ScreenStatusByUserLevel ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="pAlarmType"></param>
    ''' <remarks>Created by SGM 19/10/2012</remarks>
    Public Sub PrepareErrorMode(Optional ByVal pAlarmType As ManagementAlarmTypes = ManagementAlarmTypes.NONE)
        Try
            Me.bsAcceptButton.Enabled = False
            Me.bsExitButton.Enabled = True ' Just Exit button is enabled in error case

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".PrepareErrorMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareErrorMode ", Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        Finally
            Me.Cursor = Cursors.Default
        End Try
    End Sub

    ''' <summary>
    ''' It manages the Stop of the operation(s) that are currently being performed.  
    ''' </summary>
    ''' <param name="pAlarmType"></param>
    ''' <remarks>
    ''' Created by SGM 19/10/2012
    ''' Updated by XBC 22/10/2012 - Add business logic with every current operation
    ''' </remarks>
    Public Sub StopCurrentOperation(Optional ByVal pAlarmType As GlobalEnumerates.ManagementAlarmTypes = ManagementAlarmTypes.NONE)
        Try
            PrepareErrorMode()

            RaiseEvent StopCurrentOperationFinished(pAlarmType)

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".StopCurrentOperation ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".StopCurrentOperation ", Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        Finally
            Me.Cursor = Cursors.Default
        End Try
    End Sub

    ''' <summary>
    ''' Inform the user the elements not in use with barcode will be removed from sample rotor
    ''' </summary>
    ''' <remarks>CREATED BY: TR 07/04/2013</remarks>
    Private Sub InforNotInUseRemove()
        Try
            bsScreenErrorProvider.Clear()
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim myBarcodePositionsWithNoRequestsDelegate As New BarcodePositionsWithNoRequestsDelegate()
            myGlobalDataTO = myBarcodePositionsWithNoRequestsDelegate.GetScannedAndNotInUseElements(Nothing, AnalyzerIDAttribute, WorkSessionIDAttribute)

            If Not myGlobalDataTO.HasError Then
                'Validate if there are scanned elements and not in use
                If DirectCast(myGlobalDataTO.SetDatos, BarcodePositionsWithNoRequestsDS).twksWSBarcodePositionsWithNoRequests.Count > 0 Then
                    bsScreenErrorProvider.SetIconAlignment(bsBarCodeConfiGroupBox, ErrorIconAlignment.MiddleRight)
                    bsScreenErrorProvider.SetIconPadding(bsBarCodeConfiGroupBox, 20)

                    bsScreenErrorProvider.SetError(bsBarCodeConfiGroupBox, GetMessageText(GlobalEnumerates.Messages.REMOVE_NOTINUSE.ToString(), LanguageID))
                End If
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".InforNotInUseRemove", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".InforNotInUseRemove", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Checks if AutoLIS is enabled but the Samples Barcode is disabled. If so, a warning message is shown 
    ''' </summary>
    ''' <remarks>
    ''' Created by SGM 12/07/2013
    ''' </remarks>
    Public Function CheckAutoLISWhenDisabledSamplesBarcode() As Boolean
        Dim ret As Boolean = True
        Try
            Dim resultData As New GlobalDataTO
            If Me.bsSamplesCheckBox.Checked Then
                Dim myUserSettingsDlg As New UserSettingsDelegate
                resultData = myUserSettingsDlg.GetCurrentValueBySettingID(Nothing, UserSettingsEnum.AUTO_WS_WITH_LIS_MODE.ToString)
                If Not resultData.HasError AndAlso resultData.SetDatos IsNot Nothing Then
                    Dim isEnabled As Boolean = CBool(IIf(CInt(resultData.SetDatos) > 0, True, False))
                    If isEnabled Then
                        Dim res As DialogResult = MyBase.ShowMessage(Me.Name, Messages.BARCODE_AUTOLIS_WARNING.ToString)
                        If res = Windows.Forms.DialogResult.OK Then
                            'set AUTO_WS_WITH_LIS_MODE = 0
                            resultData = myUserSettingsDlg.Update(Nothing, UserSettingsEnum.AUTO_WS_WITH_LIS_MODE.ToString, "0")
                            If Not resultData.HasError Then
                                ret = True
                            End If
                        Else
                            ret = False
                        End If
                    Else
                        ret = True
                    End If
                    'Me.bsSamplesCheckBox.Checked = ret
                End If
            Else
                ret = True
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".CheckAutoLISWhenDisabledSamplesBarcode", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".CheckAutoLISWhenDisabledSamplesBarcode", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
        Return ret
    End Function
#End Region

#Region "Events"

    '*****************************'
    '* SCREEN AND GENERAL EVENTS *'
    '*****************************'
    ''' <summary>
    ''' When the screen ESC key is pressed, the screen is closed
    ''' </summary>
    ''' <remarks>
    ''' Created by:  RH 06/07/2011
    ''' Modified by: RH 04/07/2011 - Escape key should do exactly the same operations as bsExitButton_Click()
    ''' </remarks>
    Private Sub IBarCodesConfig_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyDown
        Try
            If (e.KeyCode = Keys.Escape) Then bsExitButton.PerformClick()

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".IBarCodesConfig_KeyDown ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".IBarCodesConfig_KeyDown", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Initializes the form
    ''' </summary>
    ''' <remarks>
    ''' Created by:  RH 06/07/2011
    ''' Modified by: XB+JC 09/10/2013 - Improve Usability #1273 Bugs tracking
    ''' </remarks>
    Private Sub IBarCodesConfig_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            myTop = Me.Top
            myLeft = Me.Left
            'TR 20/04/2012 get the current user level
            'Dim myGlobalbase As New GlobalBase
            CurrentUserLevel = GlobalBase.GetSessionInfo().UserLevel
            'TR 20/04/2012 -END.
            InitializeScreen()
            EditionMode = True
            If Not bsSamplesCheckBox.Checked Then
                bsSamplesCheckBox.Checked = True
                bsSamplesCheckBox.Checked = False
                ChangesMade = False
            End If

            ' XB+JC 09/10/2013
            'bsSamplesTypesGridView.Enabled = (bsSampleTypeCheckBox.Enabled AndAlso bsSampleTypeCheckBox.Checked) 'bsExternalIDCheckBox.Checked
            'bsExtenalIDLabel.Enabled = bsExternalIDCheckBox.Checked
            'bsSampleTypeCheckBox.Enabled = bsExternalIDCheckBox.Checked
            'bsExtIDStartNumericUpDown.Enabled = bsExternalIDCheckBox.Checked
            'bsExtIDEndNumericUpDown.Enabled = bsExternalIDCheckBox.Checked
            bsSamplesTypesGridView.Enabled = (bsExternalIDCheckBox.Enabled AndAlso bsExternalIDCheckBox.Checked) 'bsExternalIDCheckBox.Checked
            bsExtenalIDLabel.Enabled = bsExternalIDCheckBox.Checked
            bsExtIDStartNumericUpDown.Enabled = bsExternalIDCheckBox.Checked
            bsExtIDEndNumericUpDown.Enabled = bsExternalIDCheckBox.Checked
            bsSampleTypeStartNumericUpDown.Enabled = bsExternalIDCheckBox.Checked
            bsSampleTypeEndNumericUpDown.Enabled = bsExternalIDCheckBox.Checked
            ' XB+JC 09/10/2013

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".IBarCodesConfig_Load ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".IBarCodesConfig_Load", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Do not allow moving the screen
    ''' </summary>
    Private Sub IBarCodesConfig_Move(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Move
        Try
            Me.Top = myTop
            Me.Left = myLeft
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".IBarCodesConfig_Move ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".IBarCodesConfig_Move", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub


    ''' <summary>
    ''' When the CheckBoxes of Reagents/Samples Barcode Deactivation are changed, set flag ChangesMade to True
    ''' </summary>
    ''' <remarks>
    ''' Created by:  TR 14/02/2012
    ''' Modified by SGM 12/07/2013
    ''' </remarks>
    Private Sub ControlValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsReagentsCheckBox.CheckedChanged, _
                                                                                                        bsSamplesCheckBox.CheckedChanged
        Try
            'SGM 12/07/2013
            If Not (EditionMode) Then Exit Sub

            If CType(sender, Ax00.Controls.UserControls.BSCheckbox) Is bsSamplesCheckBox Then
                MyClass.IsSamplesBarcodeDisabledByUser = bsSamplesCheckBox.Checked
            End If
            ChangesMade = True

            'If (EditionMode) Then ChangesMade = True

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ControlValueChanged", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ControlValueChanged", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub


    '*********************************************************'
    '* EVENTS FOR CONTROLS IN SAMPLES BARCODE TYPES GROUPBOX *'
    '*********************************************************'
    Private Sub bsSamplesBarcodeTypesDataGridView_CellEndEdit(ByVal sender As System.Object, ByVal e As DataGridViewCellEventArgs) Handles bsSamplesBarcodeTypesDataGridView.CellEndEdit
        Try
            If (EditionMode) Then
                ChangesMade = True
                BarcodeConfigChangesToSend = True
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "bsSamplesBarcodeTypesDataGridView_CellEndEdit " & Me.Name, EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsSamplesBarcodeTypesDataGridView_CellEndEdit", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    '*****************************************************************'
    '* EVENTS FOR CONTROLS IN SAMPLES BARCODE CONFIGURATION GROUPBOX *'
    '*****************************************************************'
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>
    ''' Modified by: TR 08/04/2013 -Replace control bsFullTotalNumericUpDown using a textBox instead a numeriUpDownBox.
    '''              XB+JC 09/10/2013 - Improve Usability #1273 Bugs tracking
    ''' </remarks>
    Private Sub bsFullTotalNumericUpDown_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Try
            If (Not CanApplyBoundRules) Then Return

            bsScreenErrorProvider.Clear()

            ' XB+JC 09/10/2013
            'If (bsSampleTypeCheckBox.Checked) Then
            If (bsExternalIDCheckBox.Checked) Then
                ' XB+JC 09/10/2013

                'bsSampleTypeEndNumericUpDown.Maximum = bsFullTotalNumericUpDown.Value
                bsSampleTypeEndNumericUpDown.Maximum = BarcodeMaxSize
                'bsSampleTypeStartNumericUpDown.Maximum = bsFullTotalNumericUpDown.Value
                bsSampleTypeStartNumericUpDown.Maximum = BarcodeMaxSize
            End If

            'bsExtIDEndNumericUpDown.Maximum = bsFullTotalNumericUpDown.Value
            bsExtIDEndNumericUpDown.Maximum = BarcodeMaxSize

            If (EditionMode) Then
                ChangesMade = True
                BarcodeConfigChangesToSend = True
                InforNotInUseRemove()
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsFullTotalNumericUpDown_ValueChanged ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsFullTotalNumericUpDown_ValueChanged", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub bsExtIDStartNumericUpDown_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsExtIDStartNumericUpDown.ValueChanged
        Try
            If (Not CanApplyBoundRules) Then Return

            bsScreenErrorProvider.Clear()
            bsExtIDEndNumericUpDown.Minimum = bsExtIDStartNumericUpDown.Value
            bsExtIDTotalTextBox.Text = (bsExtIDEndNumericUpDown.Value + 1 - bsExtIDStartNumericUpDown.Value).ToString()
            If (EditionMode) Then
                ChangesMade = True
                ResetNotInUseRotorPosition = True 'TR 03/05/2013 -Reset pos not in use on samples rotor
                InforNotInUseRemove()
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsExtIDStartNumericUpDown_ValueChanged ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsExtIDStartNumericUpDown_ValueChanged", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub bsExtIDEndNumericUpDown_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsExtIDEndNumericUpDown.ValueChanged
        Try
            If (Not CanApplyBoundRules) Then Return

            bsScreenErrorProvider.Clear()
            bsExtIDStartNumericUpDown.Maximum = bsExtIDEndNumericUpDown.Value
            bsExtIDTotalTextBox.Text = (bsExtIDEndNumericUpDown.Value + 1 - bsExtIDStartNumericUpDown.Value).ToString()

            'If (bsSampleTypeCheckBox.Checked) Then
            '    bsFullTotalNumericUpDown.Minimum = Math.Max(bsSampleTypeEndNumericUpDown.Value, bsExtIDEndNumericUpDown.Value)
            'Else
            '    bsFullTotalNumericUpDown.Minimum = bsExtIDEndNumericUpDown.Value
            'End If

            If (EditionMode) Then
                ChangesMade = True
                ResetNotInUseRotorPosition = True 'TR 03/05/2013 -Reset pos not in use on samples rotor
                InforNotInUseRemove()
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsExtIDEndNumericUpDown_ValueChanged ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsExtIDEndNumericUpDown_ValueChanged", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ' XB+JC 09/10/2013 - Improve Usability #1273 Bugs tracking
    'Private Sub bsSampleTypeCheckBox_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsSampleTypeCheckBox.CheckedChanged
    '    Try
    '        bsScreenErrorProvider.Clear()
    '        bsSampleTypeStartNumericUpDown.Enabled = bsSampleTypeCheckBox.Checked
    '        bsSampleTypeEndNumericUpDown.Enabled = bsSampleTypeCheckBox.Checked

    '        bsSamplesTypesGridView.Enabled = (bsSampleTypeCheckBox.Enabled AndAlso bsSampleTypeCheckBox.Checked) 'bsSampleTypeCheckBox.Checked
    '        bsSamplesTypesGridView.Refresh()

    '        If (bsSampleTypeCheckBox.Checked) Then bsSampleTypeTotalTextBox.Text = (bsSampleTypeEndNumericUpDown.Value + 1 - bsSampleTypeStartNumericUpDown.Value).ToString()

    '        ApplyBoundRules()
    '        If (EditionMode) Then
    '            ChangesMade = True
    '            ResetNotInUseRotorPosition = True 'TR 03/05/2013 -Reset pos not in use on samples rotor
    '            InforNotInUseRemove()
    '        End If
    '    Catch ex As Exception
    '        GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsTypbsSampleTypeCheckBox_CheckedChangedeCheckBox_CheckedChanged ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
    '        ShowMessage(Me.Name & ".bsSampleTypeCheckBox_CheckedChanged", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", Me)
    '    End Try
    'End Sub
    ' XB+JC 09/10/2013 - Improve Usability #1273 Bugs tracking

    Private Sub bsSampleTypeStartNumericUpDown_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsSampleTypeStartNumericUpDown.ValueChanged
        Try
            If (Not CanApplyBoundRules) Then Return

            bsScreenErrorProvider.Clear()
            bsSampleTypeEndNumericUpDown.Minimum = bsSampleTypeStartNumericUpDown.Value
            bsSampleTypeTotalTextBox.Text = (bsSampleTypeEndNumericUpDown.Value + 1 - bsSampleTypeStartNumericUpDown.Value).ToString()
            If (EditionMode) Then
                ChangesMade = True
                ResetNotInUseRotorPosition = True 'TR 03/05/2013 -Reset pos not in use on samples rotor
                InforNotInUseRemove()
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsSampleTypeStartNumericUpDown_ValueChanged ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsSampleTypeStartNumericUpDown_ValueChanged", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub bsSampleTypeEndNumericUpDown_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsSampleTypeEndNumericUpDown.ValueChanged
        Try
            If (Not CanApplyBoundRules) Then Return

            bsScreenErrorProvider.Clear()
            bsSampleTypeTotalTextBox.Text = (bsSampleTypeEndNumericUpDown.Value + 1 - bsSampleTypeStartNumericUpDown.Value).ToString()
            'bsFullTotalNumericUpDown.Minimum = Math.Max(bsSampleTypeEndNumericUpDown.Value, bsExtIDEndNumericUpDown.Value)
            If (EditionMode) Then
                ChangesMade = True
                ResetNotInUseRotorPosition = True 'TR 03/05/2013 -Reset pos not in use on samples rotor.
                InforNotInUseRemove()
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsTypeEndNumericUpDown_ValueChanged ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsTypeEndNumericUpDown_ValueChanged", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub bsSampleTypeTotalTextBox_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsSampleTypeTotalTextBox.TextChanged
        Try
            If CInt(bsSampleTypeTotalTextBox.Text) >= 0 Then
                InitializeSampleTypesGrid()
                ValidLaboratoryCodeLenght()
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsSampleTypeTotalTextBox_TextChanged ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsSampleTypeTotalTextBox_TextChanged", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub bsSamplesTypesGridView_CellEndEdit(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles bsSamplesTypesGridView.CellEndEdit
        Try
            bsSamplesTypesGridView.Rows(e.RowIndex).Cells("ExternalSampleType").ErrorText = ""

            'Validate if the Laboratory Code is duplicated
            If (DuplicatedLaboratoryCode(bsSamplesTypesGridView.Rows(e.RowIndex).Cells("SampleType").Value.ToString(), _
                                         bsSamplesTypesGridView.Rows(e.RowIndex).Cells("ExternalSampleType").Value.ToString())) Then
                'Show Error Message in the grid and clean the informed value
                bsSamplesTypesGridView.Rows(e.RowIndex).Cells("ExternalSampleType").ErrorText = GetMessageText(GlobalEnumerates.Messages.DUPLICATE_CODE.ToString, LanguageID)
                bsSamplesTypesGridView.Rows(e.RowIndex).Cells("ExternalSampleType").Value = String.Empty
            End If

            If (EditionMode) Then
                ChangesMade = True
                BarcodeConfigChangesToSend = True
                ResetNotInUseRotorPosition = True 'TR 03/05/2013 -Reset pos not in use on samples rotor
                InforNotInUseRemove()
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsSamplesTypesGridView_CellEndEdit ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsSamplesTypesGridView_CellEndEdit", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub bsSamplesTypesGridView_EnabledChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsSamplesTypesGridView.EnabledChanged
        Try
            'Dim backColor As New Color
            'Dim letColor As New Color

            'If (Not (bsSampleTypeCheckBox.Enabled AndAlso bsSampleTypeCheckBox.Checked)) Then
            '    backColor = SystemColors.MenuBar
            '    letColor = Color.DarkGray
            'Else
            '    backColor = Color.White
            '    letColor = Color.Black
            'End If

            'For Each myRow As DataGridViewRow In bsSamplesTypesGridView.Rows
            '    myRow.DefaultCellStyle.BackColor = backColor
            '    myRow.DefaultCellStyle.ForeColor = letColor
            'Next myRow

            bsSamplesTypesGridView.Refresh()

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " bsSamplesTypesGridView_EnabledChanged ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & " bsSamplesTypesGridView_EnabledChanged ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub
    ''' <summary>
    ''' Apply the rules for external id check.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>
    ''' CREATED BY:  TR 07/03/2013
    ''' MODIFIED BY: TR 08/04/2013 -Set the bsFullTotalNumericUpDown read only.
    '''              XB+JC 09/10/2013 - Improve Usability #1273 Bugs tracking
    ''' </remarks>
    Private Sub bsExternalIDCheckBox_CheckedChanged(sender As Object, e As EventArgs) Handles bsExternalIDCheckBox.CheckedChanged
        Try
            bsScreenErrorProvider.Clear()

            ' XB+JC 09/10/2013
            'If Not bsExternalIDCheckBox.Checked Then
            '    bsSampleTypeCheckBox.Checked = False
            'End If

            'bsExtenalIDLabel.Enabled = bsExternalIDCheckBox.Checked
            ''bsFullTotalNumericUpDown.Enabled = bsExternalIDCheckBox.Checked
            'bsSampleTypeCheckBox.Enabled = bsExternalIDCheckBox.Checked
            'bsExtIDStartNumericUpDown.Enabled = bsExternalIDCheckBox.Checked
            'bsExtIDEndNumericUpDown.Enabled = bsExternalIDCheckBox.Checked

            'bsSamplesTypesGridView.Enabled = (bsSampleTypeCheckBox.Enabled AndAlso bsSampleTypeCheckBox.Checked) ' bsExternalIDCheckBox.Checked

            'If (EditionMode) Then
            '    ChangesMade = True
            '    BarcodeConfigChangesToSend = True
            '    ResetNotInUseRotorPosition = True 'TR 03/05/2013 -Reset pos not in use on samples rotor
            '    InforNotInUseRemove()
            'End If
            bsExtenalIDLabel.Enabled = bsExternalIDCheckBox.Checked
            bsExtIDStartNumericUpDown.Enabled = bsExternalIDCheckBox.Checked
            bsExtIDEndNumericUpDown.Enabled = bsExternalIDCheckBox.Checked

            bsSamplesTypesGridView.Enabled = (bsExternalIDCheckBox.Enabled AndAlso bsExternalIDCheckBox.Checked) ' bsExternalIDCheckBox.Checked

            If (EditionMode) Then
                ChangesMade = True
                BarcodeConfigChangesToSend = True
                ResetNotInUseRotorPosition = True 'TR 03/05/2013 -Reset pos not in use on samples rotor
                InforNotInUseRemove()
            End If

            'Logic code when deleted bsSampleTypeCheckBox was check changed:
            'bsSampleTypeCheckBox_CheckedChanged(sender, e) :
            bsScreenErrorProvider.Clear()
            bsSampleTypeStartNumericUpDown.Enabled = bsExternalIDCheckBox.Checked
            bsSampleTypeEndNumericUpDown.Enabled = bsExternalIDCheckBox.Checked

            bsSamplesTypesGridView.Enabled = (bsExternalIDCheckBox.Enabled AndAlso bsExternalIDCheckBox.Checked)
            bsSamplesTypesGridView.Refresh()

            If (bsExternalIDCheckBox.Checked) Then bsSampleTypeTotalTextBox.Text = (bsSampleTypeEndNumericUpDown.Value + 1 - bsSampleTypeStartNumericUpDown.Value).ToString()

            ApplyBoundRules()
            If (EditionMode) Then
                ChangesMade = True
                ResetNotInUseRotorPosition = True 'TR 03/05/2013 -Reset pos not in use on samples rotor
                InforNotInUseRemove()
            End If
            ' XB+JC 09/10/2013

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsExternalIDCheckBox_CheckedChanged ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsExternalIDCheckBox_CheckedChanged", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try

    End Sub

    '*****************************'
    '* EVENTS FOR SCREEN BUTTONS *'
    '*****************************'
    ''' <summary>
    ''' Accepts changes and closes the dialog
    ''' </summary>
    ''' <remarks>
    ''' Created by:  RH 06/07/2011
    ''' Modified by: DL 22/07/2011 - Changed the screen controls in which the ErrorProvider bullet is shown
    '''              TR 31/08/2011 - When the SampleType is included in the Barcode and the length is valid, verify also
    '''                              there are not duplicated SampleType Laboratory codes
    '''              SG 12/07/2013 - When disabling Sample Barcode check if it is Auto-LIS activated. 
    '''              XB 29/08/2013 - ExternalPID = barcode always (change in v2.1.1)
    '''              XB+JC 09/10/2013 - Improve Usability #1273 Bugs tracking
    ''' </remarks>
    Private Sub bsAcceptButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsAcceptButton.Click
        Try
            GlobalBase.CreateLogActivity("Btn Accept", Me.Name & ".bsAcceptButton_Click", EventLogEntryType.Information, False) 'JV #1360 24/10/2013
            'SG 12/07/2013 - When disabling Sample Barcode check if it is Auto-LIS activated. 
            If MyClass.IsSamplesBarcodeDisabledByUser Then
                If Not MyClass.CheckAutoLISWhenDisabledSamplesBarcode() Then
                    Exit Sub
                End If
            End If

            ' XB+JC 09/10/2013
            'If (bsSampleTypeCheckBox.Checked) Then
            If (bsExternalIDCheckBox.Checked) Then
                ' XB+JC 09/10/2013

                ' XB 29/08/2013 - ExternalPID = barcode always so always exists overlap
                'Dim ConditionOK As Boolean = (bsExtIDEndNumericUpDown.Value < bsSampleTypeStartNumericUpDown.Value) OrElse _
                '                             (bsSampleTypeEndNumericUpDown.Value < bsExtIDStartNumericUpDown.Value)

                'If (Not ConditionOK) Then
                '    bsScreenErrorProvider.SetError(bsSampleTypeStartNumericUpDown, GetMessageText(GlobalEnumerates.Messages.BARCODE_OVERLAP.ToString()))
                '    bsScreenErrorProvider.SetError(bsExtIDStartNumericUpDown, GetMessageText(GlobalEnumerates.Messages.BARCODE_OVERLAP.ToString()))
                'Else
                '    'Verify there are not duplicated SampleType Laboratory codes in the mapping grid
                '    If (ValidLaboratoryCodeLenght()) Then AcceptSelection()
                'End If

                'Verify there are not duplicated SampleType Laboratory codes in the mapping grid
                If (ValidLaboratoryCodeLenght()) Then AcceptSelection()
                ' XB 29/08/2013
            Else
                AcceptSelection()
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsAcceptButton_Click", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsAcceptButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Cancel changes and closes the dialog
    ''' </summary>
    ''' <remarks>
    ''' Created by:  RH 06/07/2011
    ''' Modified by: TR 14/02/2012 - Added management of Discard Pending Changes when some data has been changes
    '''             XBC 14/02/2012 - Do not open Monitor Screen; just close the screen
    ''' </remarks>
    Private Sub bsExitButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsExitButton.Click
        Try
            If (ChangesMade) Then
                If (ShowMessage(Name, GlobalEnumerates.Messages.DISCARD_PENDING_CHANGES.ToString) = Windows.Forms.DialogResult.Yes) Then
                    ChangesMade = False
                    Me.Enabled = False 'RH 13/04/2012 Disable the form to avoid pressing buttons on closing
                    Me.Close()
                    bsScreenErrorProvider.Clear()
                End If
            Else
                Me.Enabled = False 'RH 13/04/2012 Disable the form to avoid pressing buttons on closing
                Me.Close()
                bsScreenErrorProvider.Clear()
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "bsExitButton_Click " & Me.Name, EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsExitButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub


#End Region

#Region "Constructor"
    'DL 22/07/11
    'Public Sub New(ByRef myMDI As Form)
    Public Sub New()
        'RH 12/04/2012 myMDI not needed.
        'MyBase.New()
        'SetParentMDI(myMDI)
        InitializeComponent()

        EditionMode = False 'TR 14/02/2012
        ChangesMade = False 'TR 14/02/2012

        BarcodeConfigChangesToSend = False ' XBC 14/02/2012
        ResetNotInUseRotorPosition = False

    End Sub

#End Region

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>
    ''' Modified by: XB+JC 09/10/2013 - Improve Usability #1273 Bugs tracking
    ''' </remarks>
    Private Sub bsSamplesTypesGridView_CellPainting(sender As Object, e As DataGridViewCellPaintingEventArgs) Handles bsSamplesTypesGridView.CellPainting
        ' XB+JC 09/10/2013
        'If bsSamplesTypesGridView.Enabled AndAlso (bsSampleTypeCheckBox.Enabled AndAlso bsSampleTypeCheckBox.Checked) Then
        If bsSamplesTypesGridView.Enabled AndAlso (bsExternalIDCheckBox.Enabled AndAlso bsExternalIDCheckBox.Checked) Then
            ' XB+JC 09/10/2013

            e.CellStyle.BackColor = Color.White
            e.CellStyle.ForeColor = Color.Black
        Else
            e.CellStyle.BackColor = SystemColors.MenuBar
            e.CellStyle.ForeColor = Color.DarkGray
        End If

    End Sub


End Class
