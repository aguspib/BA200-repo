Option Explicit On
Option Strict On

Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Global.TO
Imports Biosystems.Ax00.Global.GlobalEnumerates
Imports Biosystems.Ax00.FwScriptsManagement
Imports Biosystems.Ax00.BL
Imports System.Windows.Forms
Imports System.Drawing
Imports Biosystems.Ax00.CommunicationsSwFw
Imports System.IO
Imports Biosystems.Ax00.Controls.UserControls
Imports System.Globalization
Imports Biosystems.Ax00.InfoAnalyzer

'PENDING:
'Add Information XPS Docs for each Action
'The Reagents Pack Dallas Information MUST BE HIDE!!!!!!!!!! (DisplayISEInfo)
Public Class IISEResultsHistory

#Region "Events definitions"

#End Region

#Region " Structures "
    Public Structure ElectrodesFilter
        Public dateFrom As Date
        Public dateTo As Date
        Public electrodeNa As Boolean
        Public electrodeK As Boolean
        Public electrodeCl As Boolean
        Public electrodeLi As Boolean

        Public Function NeedSearchResults(ByVal oldFilter As ElectrodesFilter?) As Boolean
            Return Not oldFilter.HasValue OrElse _
                   oldFilter.Value.dateFrom <> Me.dateFrom OrElse _
                   oldFilter.Value.dateTo <> Me.dateTo
        End Function

        Public Function NeedHideColumns(ByVal oldFilter As ElectrodesFilter?) As Boolean
            Return Not oldFilter.HasValue OrElse _
                   oldFilter.Value.electrodeNa <> Me.electrodeNa OrElse _
                   oldFilter.Value.electrodeK <> Me.electrodeK OrElse _
                   oldFilter.Value.electrodeCl <> Me.electrodeCl OrElse _
                   oldFilter.Value.electrodeLi <> Me.electrodeLi
        End Function

        Public Function GetISEConditioningTypesArray() As ISEConditioningTypes()
            Return New ISEConditioningTypes() {ISEConditioningTypes.CALB}
        End Function
    End Structure

    Private Structure ConditioningFilter
        Public dateFrom As Date
        Public dateTo As Date
        Public conditioningPumps As Boolean
        Public conditioningBubbles As Boolean
        Public conditioningCleanings As Boolean

        Public Function NeedSearchResults(ByVal oldFilter As ConditioningFilter?) As Boolean
            Return Not oldFilter.HasValue OrElse _
                   oldFilter.Value.dateFrom <> Me.dateFrom OrElse _
                   oldFilter.Value.dateTo <> Me.dateTo OrElse _
                   oldFilter.Value.conditioningPumps <> Me.conditioningPumps OrElse _
                   oldFilter.Value.conditioningBubbles <> Me.conditioningBubbles OrElse _
                   oldFilter.Value.conditioningCleanings <> Me.conditioningCleanings
        End Function

        Public Function GetISEConditioningTypesArray() As ISEConditioningTypes()
            Dim types As ISEConditioningTypes() = {}
            If conditioningPumps Then
                Array.Resize(types, types.Length + 1)
                types(types.Length - 1) = ISEConditioningTypes.PMCL
            End If
            If conditioningBubbles Then
                Array.Resize(types, types.Length + 1)
                types(types.Length - 1) = ISEConditioningTypes.BBCL
            End If
            If conditioningCleanings Then
                Array.Resize(types, types.Length + 1)
                types(types.Length - 1) = ISEConditioningTypes.CLEN
            End If
            Return types
        End Function
    End Structure
#End Region

#Region "Attributes"
    Private AnalyzerIDAttribute As String = String.Empty
#End Region

#Region "Properties"
    Public WriteOnly Property AnalyzerID() As String
        Set(ByVal value As String)
            AnalyzerIDAttribute = value
        End Set
    End Property
#End Region

#Region "Declarations"
    Private myISEManager As ISEManager
    Private WithEvents mdiAnalyzerCopy As AnalyzerManager
    Private myCultureInfo As CultureInfo
    Private myISEHistory As New ISECalibHistoryDelegate

    ' Language
    Private currentLanguage As String

    Private mLastElectrodesFilter As ElectrodesFilter? = Nothing
    Private mLastConditioningFilter As ConditioningFilter? = Nothing

    Private mLastElectrodeDataTable As DataTable
    Private mLastConditioningDataTable As DataTable

    Private mImageDict As Dictionary(Of String, Image)
    Private mTextDict As Dictionary(Of String, String)
#End Region

#Region "Constructor"

    'JB 26/07/2012 myMDI not needed.
    'Public Sub New(ByVal pMDI As BSBaseForm)

    '    MyClass.myParentMDI = pMDI
    '    InitializeComponent()

    'End Sub
    Public Sub New()
        MyBase.New()
        InitializeComponent()

        myCultureInfo = My.Computer.Info.InstalledUICulture
        myISEHistory = New ISECalibHistoryDelegate
    End Sub
#End Region

#Region "Public Methods"

#End Region

#Region "Private Methods"
#Region " Common "
    ''' <summary>
    ''' Gets the image by its key
    ''' Only reads from file once for key
    ''' </summary>
    ''' <param name="pKey"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' Created by: JB 31/07/2012
    ''' </remarks>
    Private Function GetImage(ByVal pKey As String) As Image
        If Not mImageDict.ContainsKey(pKey) Then
            SetImageToDictionary(pKey)
        End If
        If Not mImageDict.ContainsKey(pKey) Then
            Return New Bitmap(16, 16)
        End If

        Return mImageDict.Item(pKey)
    End Function
    Private Sub SetImageToDictionary(ByVal pKey As String)
        Dim auxIconName As String = ""
        Dim iconPath As String = MyBase.IconsPath

        auxIconName = GetIconName(pKey)
        If Not String.IsNullOrEmpty(auxIconName) Then
            If mImageDict.ContainsKey(pKey) Then
                mImageDict.Item(pKey) = Image.FromFile(iconPath & auxIconName)
            Else
                mImageDict.Add(pKey, Image.FromFile(iconPath & auxIconName))
            End If
        End If

    End Sub

    ''' <summary>
    ''' Gets the multilanguage text by its key
    ''' Only reads from DataBase once for key
    ''' </summary>
    ''' <param name="pKey"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' Created by: JB 01/08/2012
    ''' </remarks>
    Private Function GetText(ByVal pKey As String) As String
        If Not mTextDict.ContainsKey(pKey) Then
            SetTextToDictionary(pKey)
        End If
        If Not mTextDict.ContainsKey(pKey) Then
            Return ""
        End If
        Return mTextDict.Item(pKey)
    End Function
    Private Sub SetTextToDictionary(ByVal pKey As String)
        Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate
        Dim text As String = myMultiLangResourcesDelegate.GetResourceText(Nothing, pKey, currentLanguage)
        If String.IsNullOrEmpty(text) Then text = "*" & pKey
        If mTextDict.ContainsKey(pKey) Then
            mTextDict.Item(pKey) = text
        Else
            mTextDict.Add(pKey, text)
        End If
    End Sub

    ''' <summary>
    ''' Gets the alarm description by its ID
    ''' </summary>
    ''' <param name="pAlarmId"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' Created by: JB 02/08/2012
    ''' </remarks>
    Private Function GetDescAlarm(ByVal pAlarmId As String) As String
        Try
            Dim myAlarmsDelegate As New AlarmsDelegate
            Dim myGlobal As GlobalDataTO = myAlarmsDelegate.Read(Nothing, pAlarmId)

            If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                Dim myAlarmsDS As AlarmsDS
                myAlarmsDS = CType(myGlobal.SetDatos, AlarmsDS)
                If myAlarmsDS.tfmwAlarms.Count > 0 Then
                    Return myAlarmsDS.tfmwAlarms(0).Description
                End If
            End If

            Return ""
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".GetDescAlarm ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".GetDescAlarm ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Function

    Private Function DecodeAffectedElectrodes(ByVal pError As ISEErrorTO) As String
        Dim strRes As String = ""

        If pError.Affected.Contains("Na") Then
            If Not String.IsNullOrEmpty(strRes) Then strRes &= ", "
            strRes &= GetText("LBL_Sodium")
        End If
        If pError.Affected.Contains("K") Then
            If Not String.IsNullOrEmpty(strRes) Then strRes &= ", "
            strRes &= GetText("LBL_Potassium")
        End If
        If pError.Affected.Contains("Cl") Then
            If Not String.IsNullOrEmpty(strRes) Then strRes &= ", "
            strRes &= GetText("LBL_Chlorine")
        End If
        If pError.Affected.Contains("Li") Then
            If Not String.IsNullOrEmpty(strRes) Then strRes &= ", "
            strRes &= GetText("LBL_Lithium")
        End If
        If Not String.IsNullOrEmpty(strRes) Then strRes &= ": "

        Return strRes
    End Function

    ''' <summary>
    ''' Decodes the "ERC" String to show in the grid
    ''' </summary>
    ''' <param name="pErrorStr">The ErrorString stored in DataBase (ERC  Errors)</param>
    ''' <returns></returns>
    ''' <remarks>
    ''' Created by: JB 02/08/2012
    ''' </remarks>
    Private Function DecodeERCErrors(ByVal pErrorStr As String) As String
        Try
            Dim myGlobal As New GlobalDataTO
            Dim myRecepcionDecoder As New ISEReception(mdiAnalyzerCopy)
            Dim myISEResultTO As ISEResultTO
            Dim strRes As String = ""

            'Processing the Errors ("<ERC " strings)
            If Not String.IsNullOrEmpty(pErrorStr) Then
                myISEResultTO = New ISEResultTO
                myISEResultTO.ReceivedResults = pErrorStr
                myISEResultTO.IsCancelError = True
                myISEResultTO.ISEResultType = ISEResultTO.ISEResultTypes.ERC
                myGlobal = myRecepcionDecoder.FillISEResultValues(myISEResultTO, ISEResultTO.ISEResultItemTypes.CancelError, pErrorStr)

                If Not myGlobal.HasError AndAlso myISEResultTO.Errors IsNot Nothing Then
                    For Each err As ISEErrorTO In myISEResultTO.Errors
                        If Not String.IsNullOrEmpty(strRes) Then strRes &= vbCrLf
                        strRes &= DecodeAffectedElectrodes(err) & GetDescAlarm(err.ErrorDesc) 'AlarmID
                    Next
                End If
            End If

            Return strRes
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".DecodeErrors ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".DecodeErrors ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Function

    ''' <summary>
    ''' Decodes the ErrorString and the ResultsString of Electrodes Calibration to show in the grid
    ''' </summary>
    ''' <param name="pResultStr"></param>
    ''' <param name="pErrorStr"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' Created by:  JB 02/08/2012
    ''' Modified by: JB 20/09/2012 - Added parameter LiEnabled 
    ''' </remarks>
    Private Function DecodeResultsAndErrorsForElectrodes(ByVal pResultStr As String, ByVal pErrorStr As String, ByVal pLiEnabled As Boolean) As String
        Try
            Dim myGlobal As New GlobalDataTO
            Dim myRecepcionDecoder As New ISEReception(mdiAnalyzerCopy)
            Dim myISEResultTO As ISEResultTO
            Dim strRes As String = DecodeERCErrors(pErrorStr)

            'Processing the Results (only for Electrode Calibration)
            Dim results As String() = pResultStr.Split("#"c)
            Dim bFirst As Boolean = True
            Dim bDecodedError As Boolean 'JB 20/09/2012 - To manage errors in message and in values
            'JB 20/09/2012 - Li Enabled in history
            Dim trLiEnabled As TriState
            If pLiEnabled Then trLiEnabled = TriState.True Else trLiEnabled = TriState.False

            For Each str As String In results
                If String.IsNullOrEmpty(str) Then Continue For
                myISEResultTO = New ISEResultTO
                myISEResultTO.ReceivedResults = str
                myISEResultTO.IsCancelError = False
                myISEResultTO.ISEResultType = ISEResultTO.ISEResultTypes.CAL

                'JB 20/09/2012 - Li Enabled in history
                If bFirst Then
                    myGlobal = myRecepcionDecoder.FillISEResultValues(myISEResultTO, ISEResultTO.ISEResultItemTypes.Calibration1, str, pForcedLiEnabledValue:=trLiEnabled)
                Else
                    myGlobal = myRecepcionDecoder.FillISEResultValues(myISEResultTO, ISEResultTO.ISEResultItemTypes.Calibration2, str, pForcedLiEnabledValue:=trLiEnabled)
                End If
                bFirst = False

                bDecodedError = False 'JB 20/09/2012 - No error decoded
                If Not myGlobal.HasError AndAlso myISEResultTO.Errors IsNot Nothing Then
                    For Each err As ISEErrorTO In myISEResultTO.Errors
                        If Not String.IsNullOrEmpty(strRes) Then strRes &= vbCrLf
                        strRes &= DecodeAffectedElectrodes(err) & GetDescAlarm(err.ErrorDesc) 'AlarmID
                        bDecodedError = True 'JB 20/09/2012 - Error decoded
                    Next
                End If
                'JB 20/09/2012 - Added validation only if no error in message
                If Not bDecodedError AndAlso str <> pErrorStr Then

                    myGlobal = myISEManager.ValidateElectrodesCalibration(str, trLiEnabled)
                    If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                        If Not DirectCast(myGlobal.SetDatos, Boolean) Then
                            If Not String.IsNullOrEmpty(strRes) Then strRes &= vbCrLf
                            strRes &= GetText("LBL_ISE_CALIB_ERROR")
                        End If
                    End If
                End If
            Next

            Return strRes
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".DecodeResultsAndErrorsForElectrodes ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".DecodeResultsAndErrorsForElectrodes ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Function

    ''' <summary>
    ''' Delete all selected calibrations in the grid
    ''' </summary>
    ''' <remarks>
    ''' Created by: JB 31/07/2012
    ''' </remarks>
    Private Sub DeleteSelectedRowsFromGrid(ByVal pGrid As BSDataGridView)
        Try
            If pGrid.SelectedRows.Count = 0 Then Exit Sub
            'Show delete confirmation message 
            If (ShowMessage(Name & ".DeleteSelectedRowsFromGrid ", GlobalEnumerates.Messages.DELETE_CONFIRMATION.ToString) <> Windows.Forms.DialogResult.Yes) Then Exit Sub

            Dim myGlobalDataTO As New GlobalDataTO
            For Each row As DataGridViewRow In pGrid.SelectedRows
                Dim idCalib As Integer
                If row.Cells("CalibrationID").Value IsNot Nothing Then
                    idCalib = CInt(row.Cells("CalibrationID").Value)
                    myGlobalDataTO = myISEHistory.DeleteCalibration(Nothing, idCalib)
                    If (myGlobalDataTO.HasError) Then
                        'Error deleting the current Calibration; show it
                        ShowMessage(Name & ".DeleteSelectedRowsFromGrid ", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage, Me)
                        Exit For
                    End If
                End If
            Next
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".DeleteSelectedRowsFromGrid ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".DeleteSelectedRowsFromGrid ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Actions to do when the Tab Selected changes
    ''' </summary>
    ''' <remarks>
    ''' Created by: JB 30/07/2012
    ''' </remarks>
    Protected Overridable Sub OnResultsTabSelectedIndexChanged()
        If bsResultsTabs.SelectedIndex = 0 Then
            GetElectrodeResults()
        ElseIf bsResultsTabs.SelectedIndex = 1 Then
            GetConditioningResults()
        End If
    End Sub
#End Region

#Region " Initializations "
    ''' <summary>
    ''' Get texts in the current application language for all screen controls
    ''' </summary>
    ''' <remarks>
    ''' Created by: JB 27/07/2012
    ''' </remarks>
    Private Sub GetScreenLabels()
        Try
            Me.bsSubtitleLabel.Text = GetText("LBL_ISE_HistoricalResults")

            '
            'ElectrodesTab
            '
            ElectrodeResultsTab.Text = GetText("LBL_ISE_Electrodes")

            bsElectrodesLastResultGroup.Text = GetText("LBL_ISE_LastElectrodeCalibration")
            CALLastDateLabel.Text = GetText("LBL_Date") 'LBL_LastDate JB - 13/11/2012 - Same text than Monitor
            CALResultLabel.Text = GetText("LBL_Result")
            bsTitleHistoricLabel.Text = GetText("LBL_ISE_HistoricalCalibration")
            bsDateFromLabel.Text = GetText("LBL_Date_From") & ":"
            bsDateToLabel.Text = GetText("LBL_Date_To") & ":"
            bsElectrodesFilterGroup.Text = GetText("LBL_ISE_Electrodes")
            bsElectrodesFilterNaCheck.Text = GetText("LBL_Sodium")
            bsElectrodesFilterKCheck.Text = GetText("LBL_Potassium")
            bsElectrodesFilterClCheck.Text = GetText("LBL_Chlorine")
            bsElectrodesFilterLiCheck.Text = GetText("LBL_Lithium")

            '
            'Conditioning Tab
            '
            ConditioningResultsTab.Text = GetText("LBL_ISE_PUMPBUBLESCLEAN")
            bsConditioningsLastResultsGroup.Text = GetText("LBL_ISE_LastCalibrations")
            CALLastDateLabel2.Text = GetText("LBL_Date") 'LBL_LastDate JB - 13/11/2012 - Same text than Monitor
            CALResultLabel2.Text = GetText("LBL_Result")
            CALPumpsLabel.Text = GetText("LBL_ISE_Pumps") & ":"
            CALBubbleLabel.Text = GetText("LBL_ISE_Bubble") & ":"
            CALCleanLabel.Text = GetText("LBL_ISE_CLEAN") & ":"

            bsTitleHistoricLabel2.Text = GetText("LBL_ISE_HistoricalCalibration")
            bsDateFromLabel2.Text = GetText("LBL_Date_From") & ":"
            bsDateToLabel2.Text = GetText("LBL_Date_To") & ":"
            bsConditioningsFilterPumpCheck.Text = GetText("LBL_ISE_Pumps")
            bsConditioningsFilterBubbleCheck.Text = GetText("LBL_ISE_Bubble")
            bsConditioningsFilterCleanCheck.Text = GetText("LBL_ISE_CLEAN")

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".GetScreenLabels", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".GetScreenLabels", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Loads the icons and tooltips used for painting the buttons
    ''' </summary>
    ''' <remarks>
    ''' Created by: JB 27/07/2012
    ''' </remarks>
    Private Sub PrepareButtons()
        Dim myToolTipsControl As New ToolTip
        Try
            'EXIT Button
            bsExitButton.Image = GetImage("CANCEL")
            myToolTipsControl.SetToolTip(bsExitButton, GetText("BTN_CloseScreen"))

            'SEARCH Buttons
            bsElectrodesSearchButton.Image = GetImage("FIND")
            myToolTipsControl.SetToolTip(bsElectrodesSearchButton, GetText("BTN_Search"))

            bsConditioningSearchButton.Image = GetImage("FIND")
            myToolTipsControl.SetToolTip(bsConditioningSearchButton, GetText("BTN_Search"))

            'GRAPH Button
            bsElectrodesGraphButton.Image = GetImage("ABS_GRAPH")
            myToolTipsControl.SetToolTip(bsElectrodesGraphButton, GetText("LBL_Graph"))

            'DELETE Buttons
            bsElectrodesDeleteButton.Image = GetImage("REMOVE")
            myToolTipsControl.SetToolTip(bsElectrodesDeleteButton, GetText("BTN_Delete"))

            bsConditioningDeleteButton.Image = GetImage("REMOVE")
            myToolTipsControl.SetToolTip(bsConditioningDeleteButton, GetText("BTN_Delete"))


            'PRINT Buttons
            bsElectrodesPrintButton.Image = GetImage("PRINT")
            myToolTipsControl.SetToolTip(bsElectrodesPrintButton, GetText("BTN_Print"))
            'JB 30/08/2012 - Hide Print button
            bsElectrodesPrintButton.Visible = False


            bsConditioningPrintButton.Image = GetImage("PRINT")
            myToolTipsControl.SetToolTip(bsConditioningPrintButton, GetText("BTN_Print"))
            'JB 30/08/2012 - Hide Print button
            bsConditioningPrintButton.Visible = False

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".PrepareButtons", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareButtons", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Initializes the grid in the Electrodes Tab
    ''' </summary>
    ''' <remarks>
    ''' Created by: JB 27/07/2012
    ''' Modified by: JC 12/11/2012 - Modify column width.
    ''' </remarks>
    Private Sub PrepareElectrodeResultsGrid(ByVal pGrid As BSDataGridView)
        Try
            pGrid.Columns.Clear()

            'ICON
            Dim myDataGridViewImageColumn As New DataGridViewImageColumn With {.Name = "ICON", _
                                                                               .HeaderText = "", _
                                                                               .DataPropertyName = "ICON", _
                                                                               .Width = 20, _
                                                                               .ImageLayout = DataGridViewImageCellLayout.Normal}
            myDataGridViewImageColumn.DefaultCellStyle.NullValue = Nothing
            myDataGridViewImageColumn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            myDataGridViewImageColumn.HeaderCell.Style.Alignment = DataGridViewContentAlignment.TopCenter
            pGrid.Columns.Add(myDataGridViewImageColumn)
            With pGrid.Columns("ICON")
                .DefaultCellStyle.BackColor = Color.White
                .SortMode = DataGridViewColumnSortMode.NotSortable
            End With

            'AnalyzerID
            pGrid.Columns.Add("AnalyzerID", "AnalyzerID")
            With pGrid.Columns("AnalyzerID")
                .DataPropertyName = "AnalyzerID"
                .Width = 100
                .Resizable = DataGridViewTriState.True
                .ReadOnly = True
                .Visible = False
            End With

            'CalibrationID
            ' JC 12/11/2012
            pGrid.Columns.Add("CalibrationID", "CalibrationID")
            With pGrid.Columns("CalibrationID")
                .DataPropertyName = "CalibrationID"
                .Width = 100
                .Resizable = DataGridViewTriState.True
                .ReadOnly = True
                .Visible = False
            End With

            'CalibrationDate
            pGrid.Columns.Add("CalibrationDate", GetText("LBL_Date"))
            With pGrid.Columns("CalibrationDate")
                .DataPropertyName = "CalibrationDate"
                .Width = 150
                .DefaultCellStyle.NullValue = Nothing
                .SortMode = DataGridViewColumnSortMode.Automatic
                .HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft
                .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
                .Resizable = DataGridViewTriState.True
                .ReadOnly = True
                .Visible = True
            End With

            'ConditioningType
            pGrid.Columns.Add("ConditioningType", "ConditioningType")
            With pGrid.Columns("ConditioningType")
                .DataPropertyName = "ConditioningType"
                .Width = 100
                .Resizable = DataGridViewTriState.True
                .ReadOnly = True
                .Visible = False
            End With

            'ResultsString 
            pGrid.Columns.Add("ResultsString", GetText("LBL_Results"))
            With pGrid.Columns("ResultsString")
                .DataPropertyName = "ResultsString"
                .Width = 300
                .Resizable = DataGridViewTriState.True
                .ReadOnly = True
                .Visible = False
            End With

            'ResultsNa: Na+
            pGrid.Columns.Add("ResultsNa", GetText("LBL_Sodium"))
            With pGrid.Columns("ResultsNa")
                .DataPropertyName = "ResultsNa"
                .DefaultCellStyle.NullValue = Nothing
                .SortMode = DataGridViewColumnSortMode.NotSortable
                .HeaderCell.Style.WrapMode = DataGridViewTriState.True
                .HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight
                .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
                .DefaultCellStyle.WrapMode = DataGridViewTriState.True
                .Resizable = DataGridViewTriState.True
                .ReadOnly = True
                .Width = 50
                .Visible = True
            End With

            'MeanNa: Na+
            pGrid.Columns.Add("MeanNa", GetText("LBL_Sodium"))
            With pGrid.Columns("MeanNa")
                .DataPropertyName = "MeanNa"
                .DefaultCellStyle.NullValue = Nothing
                .SortMode = DataGridViewColumnSortMode.NotSortable
                .HeaderCell.Style.WrapMode = DataGridViewTriState.True
                .HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight
                .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
                .DefaultCellStyle.WrapMode = DataGridViewTriState.True
                .Resizable = DataGridViewTriState.True
                .ReadOnly = True
                .Width = 50
                .Visible = False
            End With

            'ResultsK: K+
            pGrid.Columns.Add("ResultsK", GetText("LBL_Potassium"))
            With pGrid.Columns("ResultsK")
                .DataPropertyName = "ResultsK"
                .DefaultCellStyle.NullValue = Nothing
                .SortMode = DataGridViewColumnSortMode.NotSortable
                .HeaderCell.Style.WrapMode = DataGridViewTriState.True
                .HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight
                .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
                .DefaultCellStyle.WrapMode = DataGridViewTriState.True
                .Resizable = DataGridViewTriState.True
                .ReadOnly = True
                .Width = 50
                .Visible = True
            End With

            'MeanK: K+
            pGrid.Columns.Add("MeanK", GetText("LBL_Potassium"))
            With pGrid.Columns("MeanK")
                .DataPropertyName = "MeanK"
                .DefaultCellStyle.NullValue = Nothing
                .SortMode = DataGridViewColumnSortMode.NotSortable
                .HeaderCell.Style.WrapMode = DataGridViewTriState.True
                .HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight
                .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
                .DefaultCellStyle.WrapMode = DataGridViewTriState.True
                .Resizable = DataGridViewTriState.True
                .ReadOnly = True
                .Width = 50
                .Visible = False
            End With

            'ResultsCl: Cl-
            pGrid.Columns.Add("ResultsCl", GetText("LBL_Chlorine"))
            With pGrid.Columns("ResultsCl")
                .DataPropertyName = "ResultsCl"
                .DefaultCellStyle.NullValue = Nothing
                .SortMode = DataGridViewColumnSortMode.NotSortable
                .HeaderCell.Style.WrapMode = DataGridViewTriState.True
                .HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight
                .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
                .DefaultCellStyle.WrapMode = DataGridViewTriState.True
                .Resizable = DataGridViewTriState.True
                .ReadOnly = True
                .Width = 50
                .Visible = True
            End With

            'MeanCl: Cl-
            pGrid.Columns.Add("MeanCl", GetText("LBL_Chlorine"))
            With pGrid.Columns("MeanCl")
                .DataPropertyName = "MeanCl"
                .DefaultCellStyle.NullValue = Nothing
                .SortMode = DataGridViewColumnSortMode.NotSortable
                .HeaderCell.Style.WrapMode = DataGridViewTriState.True
                .HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight
                .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
                .DefaultCellStyle.WrapMode = DataGridViewTriState.True
                .Resizable = DataGridViewTriState.True
                .ReadOnly = True
                .Width = 50
                .Visible = False
            End With

            'ResultsLi: Li+
            pGrid.Columns.Add("ResultsLi", GetText("LBL_Lithium"))
            With pGrid.Columns("ResultsLi")
                .DataPropertyName = "ResultsLi"
                .DefaultCellStyle.NullValue = Nothing
                .SortMode = DataGridViewColumnSortMode.NotSortable
                .HeaderCell.Style.WrapMode = DataGridViewTriState.True
                .HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight
                .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
                .DefaultCellStyle.WrapMode = DataGridViewTriState.True
                .Resizable = DataGridViewTriState.True
                .ReadOnly = True
                .Width = 50
                .Visible = True
            End With

            'MeanLi: Li+
            pGrid.Columns.Add("MeanLi", GetText("LBL_Lithium"))
            With pGrid.Columns("MeanLi")
                .DataPropertyName = "MeanLi"
                .DefaultCellStyle.NullValue = Nothing
                .SortMode = DataGridViewColumnSortMode.NotSortable
                .HeaderCell.Style.WrapMode = DataGridViewTriState.True
                .HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight
                .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
                .DefaultCellStyle.WrapMode = DataGridViewTriState.True
                .Resizable = DataGridViewTriState.True
                .ReadOnly = True
                .Width = 50
                .Visible = False
            End With

            'ErrorsString: Remarks
            pGrid.Columns.Add("ErrorsString", GetText("LBL_Remarks"))
            With pGrid.Columns("ErrorsString")
                .DataPropertyName = "ErrorsString"
                .AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
                .SortMode = DataGridViewColumnSortMode.NotSortable
                .HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft
                .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
                .Resizable = DataGridViewTriState.True
                .ReadOnly = True
                .Visible = True
            End With

            pGrid.MultiSelect = True
            pGrid.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells
            pGrid.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.EnableResizing
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".PrepareElectrodeResultsGrid ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Initializes de datatable for the Electrode Calibration Results
    ''' </summary>
    ''' <remarks>
    ''' Created by: JB 31/07/2012
    ''' </remarks>
    Private Sub InitializeLastElectrodeDataTable()
        mLastElectrodeDataTable = New DataTable("ElectrodeResults")
        With mLastElectrodeDataTable.Columns
            .Add("ICON")
            .Add("AnalyzerID")
            .Add("CalibrationID")
            .Add("CalibrationDate")
            .Add("ConditioningType")
            .Add("ResultsString")
            .Add("ResultsNa")
            .Add("MeanNa")
            .Add("ResultsK")
            .Add("MeanK")
            .Add("ResultsCl")
            .Add("MeanCl")
            .Add("ResultsLi")
            .Add("MeanLi")
            .Add("ErrorsString")
        End With
    End Sub

    ''' <summary>
    ''' Initializes the grid in the Conditionings Tab
    ''' </summary>
    ''' <remarks>
    ''' Created by JB 27/07/2012
    ''' </remarks>
    Private Sub PrepareConditioningResultsGrid(ByVal pGrid As BSDataGridView)
        Try
            pGrid.Columns.Clear()

            'ICON
            Dim myDataGridViewImageColumn As New DataGridViewImageColumn With {.Name = "ICON", _
                                                                               .HeaderText = "", _
                                                                               .DataPropertyName = "ICON", _
                                                                               .Width = 20, _
                                                                               .ImageLayout = DataGridViewImageCellLayout.Normal}
            myDataGridViewImageColumn.DefaultCellStyle.NullValue = Nothing
            myDataGridViewImageColumn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            myDataGridViewImageColumn.HeaderCell.Style.Alignment = DataGridViewContentAlignment.TopCenter
            pGrid.Columns.Add(myDataGridViewImageColumn)
            With pGrid.Columns("ICON")
                .DefaultCellStyle.BackColor = Color.White
                .SortMode = DataGridViewColumnSortMode.NotSortable
            End With

            'AnalyzerID
            pGrid.Columns.Add("AnalyzerID", "AnalyzerID")
            With pGrid.Columns("AnalyzerID")
                .DataPropertyName = "AnalyzerID"
                .Width = 100
                .Resizable = DataGridViewTriState.True
                .ReadOnly = True
                .Visible = False
            End With

            'CalibrationID
            pGrid.Columns.Add("CalibrationID", "CalibrationID")
            With pGrid.Columns("CalibrationID")
                .DataPropertyName = "CalibrationID"
                .Width = 100
                .Resizable = DataGridViewTriState.True
                .ReadOnly = True
                .Visible = False
            End With

            'CalibrationDate
            pGrid.Columns.Add("CalibrationDate", GetText("LBL_Date"))
            With pGrid.Columns("CalibrationDate")
                .DataPropertyName = "CalibrationDate"
                .Width = 150
                .DefaultCellStyle.NullValue = Nothing
                .SortMode = DataGridViewColumnSortMode.Automatic
                .HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft
                .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
                .Resizable = DataGridViewTriState.True
                .ReadOnly = True
                .Visible = True
            End With

            'ConditioningType
            pGrid.Columns.Add("ConditioningType", "ConditioningType")
            With pGrid.Columns("ConditioningType")
                .DataPropertyName = "ConditioningType"
                .Width = 100
                .Resizable = DataGridViewTriState.True
                .ReadOnly = True
                .Visible = False
            End With

            'ConditioningTypeName
            pGrid.Columns.Add("ConditioningTypeName", GetText("LBL_SRV_ConditioningOperation"))
            With pGrid.Columns("ConditioningTypeName")
                .DataPropertyName = "ConditioningTypeName"
                .Width = 175
                .Resizable = DataGridViewTriState.True
                .SortMode = DataGridViewColumnSortMode.Automatic
                '.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
                .HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft
                .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
                .ReadOnly = True
                .Visible = True
            End With

            'ResultsString 
            pGrid.Columns.Add("ResultsString", GetText("LBL_Results"))
            With pGrid.Columns("ResultsString")
                .DataPropertyName = "ResultsString"
                .Width = 300
                .Resizable = DataGridViewTriState.True
                .SortMode = DataGridViewColumnSortMode.NotSortable
                .HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft
                .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
                .ReadOnly = True
                .Visible = True
            End With

            'ErrorsString: Remarks
            pGrid.Columns.Add("ErrorsString", GetText("LBL_Remarks"))
            With pGrid.Columns("ErrorsString")
                .DataPropertyName = "ErrorsString"
                .AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
                .SortMode = DataGridViewColumnSortMode.NotSortable
                .HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft
                .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
                .Resizable = DataGridViewTriState.True
                .ReadOnly = True
                .Visible = True
            End With

            pGrid.MultiSelect = True
            pGrid.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells
            pGrid.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.EnableResizing
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".PrepareConditioningResultsGrid ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Initializes de datatable for the Conditioning Results
    ''' </summary>
    ''' <remarks>
    ''' Created by: JB 31/07/2012
    ''' </remarks>
    Private Sub InitializeLastConditioningDataTable()
        mLastConditioningDataTable = New DataTable("ConditioningResults")
        With mLastConditioningDataTable.Columns
            .Add("ICON")
            .Add("AnalyzerID")
            .Add("CalibrationID")
            .Add("CalibrationDate")
            .Add("ConditioningType")
            .Add("ConditioningTypeName")
            .Add("ResultsString")
            .Add("ErrorsString")
        End With
    End Sub

    ''' <summary>
    ''' Initializes the filter search in Electrode Tab
    ''' </summary>
    ''' <remarks>
    ''' Created by JB 27/07/2012
    ''' </remarks>
    Private Sub InitializeElectrodeFilterSearch()
        Try
            bsElectrodesDateFromDateTimePick.Value = Today.AddMonths(-2)
            bsElectrodesDateToDateTimePick.Value = Today

            bsElectrodesFilterNaCheck.Checked = True
            bsElectrodesFilterKCheck.Checked = True
            bsElectrodesFilterClCheck.Checked = True
            bsElectrodesFilterLiCheck.Checked = True
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".InitializeElectrodeFilterSearch ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Initializes the filter search in Conditioning Types Tab
    ''' </summary>
    ''' <remarks>
    ''' Created by JB 27/07/2012
    ''' </remarks>
    Private Sub InitializeConditioningFilterSearch()
        Try
            bsConditioningDateFromDateTimePick.Value = Today.AddMonths(-2)
            bsConditioningDateToDateTimePick.Value = Today

            bsConditioningsFilterPumpCheck.Checked = True
            bsConditioningsFilterBubbleCheck.Checked = True
            bsConditioningsFilterCleanCheck.Checked = True
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".InitializeConditioningFilterSearch ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Initialize all screen controls
    ''' </summary>
    ''' <remarks>
    ''' Created by: JB 27/07/2012
    ''' </remarks>
    Private Sub InitializeScreen()
        Try
            mImageDict = New Dictionary(Of String, Image)()
            mTextDict = New Dictionary(Of String, String)()

            'Get the current Language from the current Application Session
            Dim currentLanguageGlobal As New GlobalBase
            currentLanguage = currentLanguageGlobal.GetSessionInfo().ApplicationLanguage.Trim.ToString()

            'SGM 31/05/2013 - Get Level of the connected User
            Dim MyGlobalBase As New GlobalBase
            CurrentUserLevel = MyGlobalBase.GetSessionInfo().UserLevel
            ScreenStatusByUserLevel()

            GetScreenLabels()
            PrepareButtons()

            PrepareElectrodeResultsGrid(bsElectrodesGrid)
            PrepareConditioningResultsGrid(bsConditioningGrid)

            InitializeLastElectrodeDataTable()
            InitializeLastConditioningDataTable()

            FillLastResults()

            InitializeElectrodeFilterSearch()
            InitializeConditioningFilterSearch()

            OnResultsTabSelectedIndexChanged()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".InitializeScreen ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".InitializeScreen ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>
    ''' created by SG 31/05/2013
    ''' </remarks>
    Private Sub ScreenStatusByUserLevel()
        Try
            Select Case CurrentUserLevel    '.ToUpper()
                Case "SUPERVISOR"
                    Exit Select

                Case "OPERATOR"
                    bsElectrodesDeleteButton.Enabled = False
                    bsConditioningDeleteButton.Enabled = False
                    Exit Select
            End Select

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".ScreenStatusByUserLevel ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ScreenStatusByUserLevel ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

#End Region

#Region " Last Results "
    ''' <summary>
    ''' Cleans the Last Results Fields
    ''' </summary>
    ''' <remarks>
    ''' Created by: JB 27/07/2012
    ''' </remarks>
    Private Sub CleanLastResults()
        Try
            'Text initialization
            Me.CALElectrodesDateTextEdit.Text = ""
            Me.CALElectrodesResult1TextEdit.Text = ""
            Me.CALElectrodesResult2TextEdit.Text = ""

            Me.CALPumpsDateTextEdit.Text = ""
            Me.CALPumpsResultTextEdit.Text = ""

            Me.CALBubbleDateTextEdit.Text = ""
            Me.CALBubbleResultTextEdit.Text = ""

            Me.CALCleanDateTextEdit.Text = ""

            'Warning labels
            Me.CALElectrodesWarningLabel.Text = ""
            Me.CALPumpsWarningLabel.Text = ""
            Me.CALCleanWarningLabel.Text = ""
            Me.CALBubbleWarningLabel.Text = ""

            'Color initialization
            Dim myColor As Color = Color.Black

            Me.CALElectrodesDateTextEdit.ForeColor = myColor
            Me.CALElectrodesResult1TextEdit.ForeColor = myColor
            Me.CALElectrodesResult2TextEdit.ForeColor = myColor
            Me.CALElectrodesWarningLabel.ForeColor = myColor

            Me.CALPumpsDateTextEdit.ForeColor = myColor
            Me.CALPumpsResultTextEdit.ForeColor = myColor
            Me.CALPumpsWarningLabel.ForeColor = myColor

            Me.CALBubbleDateTextEdit.ForeColor = myColor
            Me.CALBubbleResultTextEdit.ForeColor = myColor
            Me.CALBubbleWarningLabel.ForeColor = myColor

            Me.CALCleanDateTextEdit.ForeColor = myColor
            Me.CALCleanWarningLabel.ForeColor = myColor

            'Icon initialization
            CALElectrodesIcon.BorderStyle = Windows.Forms.BorderStyle.None
            CALElectrodesIcon.BackgroundImage = Nothing
            CALElectrodesIcon.BackgroundImageLayout = ImageLayout.Center

            CALPumpsIcon.BorderStyle = Windows.Forms.BorderStyle.None
            CALPumpsIcon.BackgroundImage = Nothing
            CALPumpsIcon.BackgroundImageLayout = ImageLayout.Center

            CALBubbleIcon.BorderStyle = Windows.Forms.BorderStyle.None
            CALBubbleIcon.BackgroundImage = Nothing
            CALBubbleIcon.BackgroundImageLayout = ImageLayout.Center

            CALCleanIcon.BorderStyle = Windows.Forms.BorderStyle.None
            CALCleanIcon.BackgroundImage = Nothing
            CALCleanIcon.BackgroundImageLayout = ImageLayout.Center

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".CleanLastResults ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".CleanLastResults ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Fills the last ISE Results
    ''' </summary>
    ''' <remarks>
    ''' Created by: JB 27/07/2012 - Adapted from BSISEMonitorPanel.RefreshFieldsData
    ''' </remarks>
    Private Sub FillLastResults()
        Me.CleanLastResults()
        If myISEManager Is Nothing OrElse myISEManager.MonitorDataTO Is Nothing Then Exit Sub

        Try
            With myISEManager.MonitorDataTO

                ' ELECTRODES 
                Dim CheckRecommended As Boolean = False
                If .CAL_ElectrodesCalibDate <> Nothing Then
                    CALElectrodesDateTextEdit.Text = .CAL_ElectrodesCalibDate.ToString(myCultureInfo.DateTimeFormat.ShortDatePattern)
                    CALElectrodesResult1TextEdit.Text = .CAL_ElectrodesCalibResult1String
                    CALElectrodesResult2TextEdit.Text = .CAL_ElectrodesCalibResult2String
                    ' JBL 28/08/2012 - improvement : no display checksum and no display "<", ">"
                    If Not String.IsNullOrEmpty(Me.CALElectrodesResult1TextEdit.Text) AndAlso Me.CALElectrodesResult1TextEdit.Text.Length > 0 Then
                        Me.CALElectrodesResult1TextEdit.Text = Me.CALElectrodesResult1TextEdit.Text.Trim.Replace(">", "")
                        Me.CALElectrodesResult1TextEdit.Text = Me.CALElectrodesResult1TextEdit.Text.Trim.Replace("<", "")
                        Me.CALElectrodesResult1TextEdit.Text = Me.CALElectrodesResult1TextEdit.Text.Substring(0, Me.CALElectrodesResult1TextEdit.Text.Length - 1)
                    End If
                    If Not String.IsNullOrEmpty(Me.CALElectrodesResult2TextEdit.Text) AndAlso CALElectrodesResult2TextEdit.Text.Length > 0 Then
                        Me.CALElectrodesResult2TextEdit.Text = Me.CALElectrodesResult2TextEdit.Text.Trim.Replace(">", "")
                        Me.CALElectrodesResult2TextEdit.Text = Me.CALElectrodesResult2TextEdit.Text.Trim.Replace("<", "")
                        Me.CALElectrodesResult2TextEdit.Text = Me.CALElectrodesResult2TextEdit.Text.Substring(0, Me.CALElectrodesResult2TextEdit.Text.Length - 1)
                    End If
                    ' JBL 28/08/2012 

                    If .CAL_ElectrodesCalibResult1OK And .CAL_ElectrodesCalibResult2OK Then
                        'OK Image
                        CALElectrodesIcon.BackgroundImage = GetImage("STUS_FINISH")

                        CheckRecommended = False
                    Else
                        CALElectrodesWarningLabel.Text = GetText("LBL_ISE_CALIB_ERROR")

                        'Warning Image
                        CALElectrodesIcon.BackgroundImage = GetImage("STUS_WITHERRS")
                        CheckRecommended = True
                    End If
                Else
                    'Me.CALElectrodesIcon.BackgroundImage = MyClass.GetIconImage(IconImages.Warning)
                    CheckRecommended = True
                End If

                If .CAL_ElectrodesRecommended Or CheckRecommended Then  ' XBC 28/06/2012 - any of both mark the element as recommended !
                    If CALElectrodesWarningLabel.Text.Length > 0 Then CALElectrodesWarningLabel.Text &= ", "
                    CALElectrodesWarningLabel.Text &= GetText("LBL_ISE_CALIB_RECOMMENDED")

                    'Warning Image
                    CALElectrodesIcon.BackgroundImage = GetImage("STUS_WITHERRS")
                End If

                ' PUMPS
                CheckRecommended = False
                If .CAL_PumpsCalibDate <> Nothing Then
                    CALPumpsDateTextEdit.Text = .CAL_PumpsCalibDate.ToString(MyClass.myCultureInfo.DateTimeFormat.ShortDatePattern)
                    CALPumpsResultTextEdit.Text = .CAL_PumpsCalibResultString
                    ' JBL 28/08/2012 - improvement : no display "<", ">"
                    If Not String.IsNullOrEmpty(Me.CALPumpsResultTextEdit.Text) Then
                        Me.CALPumpsResultTextEdit.Text = Me.CALPumpsResultTextEdit.Text.Trim.Replace(">", "")
                        Me.CALPumpsResultTextEdit.Text = Me.CALPumpsResultTextEdit.Text.Trim.Replace("<", "")
                    End If
                    ' JBL 28/08/2012 

                    If .CAL_PumpsCalibResultOK Then
                        'OK Image
                        CALPumpsIcon.BackgroundImage = GetImage("STUS_FINISH")

                        CheckRecommended = False
                    Else
                        CALPumpsWarningLabel.Text = GetText("LBL_ISE_CALIB_ERROR")
                        'Warning Image
                        CALPumpsIcon.BackgroundImage = GetImage("STUS_WITHERRS")

                        CheckRecommended = True
                    End If
                Else
                    CALPumpsIcon.BackgroundImage = GetImage("STUS_WITHERRS")
                    CheckRecommended = True
                End If

                If .CAL_PumpsRecommended Or CheckRecommended Then
                    If CALPumpsWarningLabel.Text.Length > 0 Then CALPumpsWarningLabel.Text &= ", "
                    CALPumpsWarningLabel.Text &= GetText("LBL_ISE_CALIB_RECOMMENDED")

                    'Warning Image
                    CALPumpsIcon.BackgroundImage = GetImage("STUS_WITHERRS")
                End If

                ' BUBBLE
                CheckRecommended = False
                If .CAL_BubbleCalibDate <> Nothing Then
                    CALBubbleDateTextEdit.Text = .CAL_BubbleCalibDate.ToString(MyClass.myCultureInfo.DateTimeFormat.ShortDatePattern)
                    CALBubbleResultTextEdit.Text = .CAL_BubbleCalibResultString
                    ' JBL 28/08/2012 - improvement : no display "<", ">"
                    If Not String.IsNullOrEmpty(Me.CALBubbleResultTextEdit.Text) Then
                        Me.CALBubbleResultTextEdit.Text = Me.CALBubbleResultTextEdit.Text.Trim.Replace(">", "")
                        Me.CALBubbleResultTextEdit.Text = Me.CALBubbleResultTextEdit.Text.Trim.Replace("<", "")
                    End If
                    ' JBL 28/08/2012 
                    If .CAL_BubbleCalibResultOK Then
                        'OK Image
                        CALBubbleIcon.BackgroundImage = GetImage("STUS_FINISH")

                        CheckRecommended = False
                    Else
                        CALBubbleWarningLabel.Text = GetText("LBL_ISE_CALIB_ERROR")
                        'Warning Image
                        CALBubbleIcon.BackgroundImage = GetImage("STUS_WITHERRS")
                        CheckRecommended = True
                    End If
                Else
                    CheckRecommended = True
                End If


                If .CAL_BubbleRecommended Or CheckRecommended Then
                    If CALBubbleWarningLabel.Text.Length > 0 Then CALBubbleWarningLabel.Text &= ", "
                    CALBubbleWarningLabel.Text &= GetText("LBL_ISE_CALIB_RECOMMENDED")

                    'Warning Image
                    CALBubbleIcon.BackgroundImage = GetImage("STUS_WITHERRS")
                End If

                ' CLEAN
                If .CleanDate <> Nothing Then
                    CALCleanDateTextEdit.Text = .CleanDate.ToString(MyClass.myCultureInfo.DateTimeFormat.ShortDatePattern)
                End If

                If .CleanRecommended Then
                    CALCleanWarningLabel.Text = GetText("LBL_ISE_CLEAN_RECOMMENDED")
                    'Warning Image
                    CALCleanIcon.BackgroundImage = GetImage("STUS_WITHERRS")
                End If
            End With

            Application.DoEvents()
            Me.Refresh()

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".FindLastResults ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".FindLastResults ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub
#End Region

#Region " Electrode Results Search "
    ''' <summary>
    ''' Gets the current Search filter in Electrodes Tab
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>
    ''' Created by: JB 27/07/2012
    ''' </remarks>
    Private Function GetCurrentElectrodesFilter() As ElectrodesFilter
        Return New ElectrodesFilter With {.dateFrom = bsElectrodesDateFromDateTimePick.Value, _
                                          .dateTo = bsElectrodesDateToDateTimePick.Value, _
                                          .electrodeNa = bsElectrodesFilterNaCheck.Checked, _
                                          .electrodeK = bsElectrodesFilterKCheck.Checked, _
                                          .electrodeCl = bsElectrodesFilterClCheck.Checked, _
                                          .electrodeLi = bsElectrodesFilterLiCheck.Checked}
    End Function


    ''' <summary>
    ''' Sets the ISE Electrode Calibrations results to the grid
    ''' </summary>
    ''' <param name="pGrid"></param>
    ''' <param name="pThisCalibISE"></param>
    ''' <remarks>
    ''' Created by: JB 31/07/2012
    ''' </remarks>
    Private Sub SetElectrodeResultDatasource(ByVal pGrid As BSDataGridView, ByVal pThisCalibISE As HistoryISECalibrationsDS.thisCalibISEDataTable)
        Try
            mLastElectrodeDataTable.Clear()
            For Each iseRow As HistoryISECalibrationsDS.thisCalibISERow In pThisCalibISE
                Dim strRes As String() = iseRow.ResultsString.Split("#"c)
                Dim valueRes As ISEResultTO.LiNaKCl?

                Dim strNa As String = ""
                Dim strK As String = ""
                Dim strCl As String = ""
                Dim strLi As String = ""
                Dim numValues As Integer = 0
                Dim meanValues As New ISEResultTO.LiNaKCl(0, 0, 0, 0)
                For Each str As String In strRes
                    If String.IsNullOrEmpty(str) Then Continue For

                    valueRes = DecodeResultToLiNaKClValues(str)
                    If valueRes.HasValue AndAlso valueRes.Value.HasData Then
                        If strNa <> "" Then strNa &= vbCrLf
                        If strK <> "" Then strK &= vbCrLf
                        If strCl <> "" Then strCl &= vbCrLf
                        If strLi <> "" Then strLi &= vbCrLf

                        strNa &= valueRes.Value.Na.ToString
                        strK &= valueRes.Value.K.ToString
                        strCl &= valueRes.Value.Cl.ToString
                        strLi &= valueRes.Value.Li.ToString

                        numValues += 1
                        meanValues.Na += valueRes.Value.Na
                        meanValues.K += valueRes.Value.K
                        meanValues.Cl += valueRes.Value.Cl
                        meanValues.Li += valueRes.Value.Li
                    End If
                Next

                If numValues > 0 Then
                    meanValues.Na = meanValues.Na / numValues
                    meanValues.K = meanValues.K / numValues
                    meanValues.Cl = meanValues.Cl / numValues
                    meanValues.Li = meanValues.Li / numValues
                End If

                'JB 20/09/2012 - Added LiEnabled parameter
                Dim errStr As String = DecodeResultsAndErrorsForElectrodes(iseRow.ResultsString, iseRow.ErrorsString, iseRow.LiEnabled)

                Dim iconStr As String = ""
                If Not String.IsNullOrEmpty(errStr) Then iconStr = "STUS_WITHERRS"

                mLastElectrodeDataTable.Rows.Add(New Object() {iconStr, _
                                                               iseRow.AnalyzerID, _
                                                               iseRow.CalibrationID, _
                                                               iseRow.CalibrationDate, _
                                                               iseRow.ConditioningType, _
                                                               iseRow.ResultsString, _
                                                               strNa, _
                                                               meanValues.Na, _
                                                               strK, _
                                                               meanValues.K, _
                                                               strCl, _
                                                               meanValues.Cl, _
                                                               strLi, _
                                                               meanValues.Li, _
                                                               errStr})
            Next

            pGrid.DataSource = mLastElectrodeDataTable

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".SetElectrodeResultDatasource ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".SetElectrodeResultDatasource ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Finds in DB the Electrode Results by the filter
    ''' </summary>
    ''' <param name="filter"></param>
    ''' <remarks>
    ''' Created by: JB 27/07/2012
    ''' </remarks>
    Private Sub FindElectrodeResults(ByVal filter As ElectrodesFilter)
        Dim myGlobalDataTO As New GlobalDataTO
        Try
            myGlobalDataTO = myISEHistory.ReadByConditioningTypes(Nothing, AnalyzerIDAttribute, filter.GetISEConditioningTypesArray(), filter.dateFrom, filter.dateTo)
            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                Dim myLocalElectrodesResults As HistoryISECalibrationsDS = DirectCast(myGlobalDataTO.SetDatos, HistoryISECalibrationsDS)
                SetElectrodeResultDatasource(bsElectrodesGrid, myLocalElectrodesResults.thisCalibISE)
            ElseIf myGlobalDataTO.HasError Then
                ShowMessage(Me.Name & ".FindElectrodeResults", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage, Me)
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".FindElectrodeResults ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".FindElectrodeResults ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Hides/Shows the columns in the grid by the search filter
    ''' </summary>
    ''' <param name="filter"></param>
    ''' <remarks>
    ''' Created by: JB 27/07/2012
    ''' </remarks>
    Private Sub HideElectrodeResultsColumns(ByVal pGrid As BSDataGridView, ByVal filter As ElectrodesFilter)
        Try
            'Na+
            pGrid.Columns("ResultsNa").Visible = filter.electrodeNa
            'K+
            pGrid.Columns("ResultsK").Visible = filter.electrodeK
            'Cl-
            pGrid.Columns("ResultsCl").Visible = filter.electrodeCl
            'Li+
            pGrid.Columns("ResultsLi").Visible = filter.electrodeLi
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".HideElectrodeResultsColumns ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".HideElectrodeResultsColumns ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Manages the Electrode Results by the selected filter in screen
    ''' </summary>
    ''' <remarks>
    ''' Created by: JB 27/07/2012
    ''' </remarks>
    Private Sub GetElectrodeResults(Optional ByVal force As Boolean = False)
        Try
            Dim currentFilter As ElectrodesFilter = GetCurrentElectrodesFilter()
            'Need search data?
            If force OrElse currentFilter.NeedSearchResults(mLastElectrodesFilter) Then
                FindElectrodeResults(currentFilter)
            End If
            'Need hide columns?
            If force OrElse currentFilter.NeedHideColumns(mLastElectrodesFilter) Then
                HideElectrodeResultsColumns(bsElectrodesGrid, currentFilter)
            End If
            mLastElectrodesFilter = currentFilter

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".GetElectrodeResults ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".GetElectrodeResults ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Decodes the Result String to a LiNaKCl values structure
    ''' </summary>
    ''' <param name="pResultStr"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' Created by: JB 31/07/2012
    ''' </remarks>
    Private Function DecodeResultToLiNaKClValues(ByVal pResultStr As String) As ISEResultTO.LiNaKCl?
        Try
            Dim myGlobal As New GlobalDataTO
            Dim myReceptionDecoder As New ISEReception(mdiAnalyzerCopy)
            myGlobal = myReceptionDecoder.GetLiNaKClValues(pResultStr)

            If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                Return CType(myGlobal.SetDatos, ISEResultTO.LiNaKCl)
            Else
                Return Nothing
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".DecodeResultToLiNaKClValues ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".DecodeResultToLiNaKClValues ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Function
#End Region

#Region " Electrode screens "
    Private Sub ShowElectrodeGraph()
        Try
            If mLastElectrodeDataTable.Rows.Count = 0 Then Exit Sub

            Using myISEGraph As New IISEResultsHistoryGraph()
                myISEGraph.SetData(mLastElectrodesFilter.Value, mLastElectrodeDataTable)
                myISEGraph.ShowDialog()

                bsElectrodesGrid.Refresh()
            End Using

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".ShowElectrodeGraph ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ShowElectrodeGraph ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub ShowElectrodeReport()
        Try
            'TODO: Show the ISE Electrodes Calibration Report
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".ShowElectrodeReport ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ShowElectrodeReport ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub
#End Region

#Region " Conditioning Results Search "
    ''' <summary>
    ''' Gets the current Search filter in Conditioining Tab
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>
    ''' Created by: JB 27/07/2012
    ''' </remarks>
    Private Function GetCurrentConditioningFilter() As ConditioningFilter
        Return New ConditioningFilter With {.dateFrom = bsConditioningDateFromDateTimePick.Value, _
                                            .dateTo = bsConditioningDateToDateTimePick.Value, _
                                            .conditioningPumps = bsConditioningsFilterPumpCheck.Checked, _
                                            .conditioningBubbles = bsConditioningsFilterBubbleCheck.Checked, _
                                            .conditioningCleanings = bsConditioningsFilterCleanCheck.Checked}
    End Function

    ''' <summary>
    ''' Sets the ISE Conditioning actions results to the grid
    ''' </summary>
    ''' <param name="pGrid"></param>
    ''' <param name="pThisCalibISE"></param>
    ''' <remarks>
    ''' Created by: JB 31/07/2012
    ''' </remarks>
    Private Sub SetConditioningResultDatasource(ByVal pGrid As BSDataGridView, ByVal pThisCalibISE As HistoryISECalibrationsDS.thisCalibISEDataTable)
        Try
            mLastConditioningDataTable.Clear()
            For Each iseRow As HistoryISECalibrationsDS.thisCalibISERow In pThisCalibISE
                Dim strConditioningName As String = ""
                Dim strResult As String = iseRow.ResultsString
                ' JBL 28/08/2012 - improvement : no display "<", ">"
                If Not String.IsNullOrEmpty(strResult) Then
                    strResult = strResult.Trim.Replace(">", "")
                    strResult = strResult.Trim.Replace("<", "")
                End If
                ' JBL 28/08/2012 
                If iseRow.ConditioningType = ISEConditioningTypes.PMCL.ToString Then
                    strConditioningName = GetText("LBL_ISE_Pumps")
                ElseIf iseRow.ConditioningType = ISEConditioningTypes.BBCL.ToString Then
                    strConditioningName = GetText("LBL_ISE_Bubble")
                ElseIf iseRow.ConditioningType = ISEConditioningTypes.CLEN.ToString Then
                    strConditioningName = GetText("LBL_ISE_CLEAN")
                End If

                Dim errStr As String = DecodeERCErrors(iseRow.ErrorsString)

                Dim iconStr As String = ""
                If Not String.IsNullOrEmpty(errStr) Then iconStr = "STUS_WITHERRS"

                mLastConditioningDataTable.Rows.Add(New Object() {iconStr, _
                                                                  iseRow.AnalyzerID, _
                                                                  iseRow.CalibrationID, _
                                                                  iseRow.CalibrationDate, _
                                                                  iseRow.ConditioningType, _
                                                                  strConditioningName, _
                                                                  strResult, _
                                                                  errStr})
            Next

            pGrid.DataSource = mLastConditioningDataTable

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".SetConditioningResultDatasource ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".SetConditioningResultDatasource ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Finds in DB the Conditioning Results by the filter
    ''' </summary>
    ''' <param name="filter"></param>
    ''' <remarks>
    ''' Created by: JB 27/07/2012
    ''' </remarks>
    Private Sub FindConditioningResults(ByVal filter As ConditioningFilter)
        Dim myGlobalDataTO As New GlobalDataTO
        Try
            myGlobalDataTO = myISEHistory.ReadByConditioningTypes(Nothing, AnalyzerIDAttribute, filter.GetISEConditioningTypesArray, filter.dateFrom, filter.dateTo)
            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                Dim myLocalConditioningResults As HistoryISECalibrationsDS = DirectCast(myGlobalDataTO.SetDatos, HistoryISECalibrationsDS)
                SetConditioningResultDatasource(bsConditioningGrid, myLocalConditioningResults.thisCalibISE)
            ElseIf myGlobalDataTO.HasError Then
                ShowMessage(Me.Name & ".FindConditioningResults", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage, Me)
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".FindConditioningResults ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".FindConditioningResults ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Manages the Conditioning Results by the selected filter in screen
    ''' </summary>
    ''' <remarks>
    ''' Created by: JB 27/07/2012
    ''' </remarks>
    Private Sub GetConditioningResults(Optional ByVal force As Boolean = False)
        Try
            Dim currentFilter As ConditioningFilter = GetCurrentConditioningFilter()
            'Need search data?
            If force OrElse currentFilter.NeedSearchResults(mLastConditioningFilter) Then
                FindConditioningResults(currentFilter)
            End If
            mLastConditioningFilter = currentFilter

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".GetConditioningResults ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".GetConditioningResults ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub
#End Region

#Region " Conditioning screens "
    Private Sub ShowConditioningReport()
        Try
            'TODO: Show The Conditioning Report
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".ShowConditioningReport ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ShowConditioningReport ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub
#End Region
#End Region

#Region "Events"
#Region " Screen Events "
    Private Sub IISEResultsHistory_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles MyBase.KeyDown
        Try
            If (e.KeyCode = Keys.Escape) Then
                'If (Not bsRejectionNumeric.Focused AndAlso Not bsNumberOfSeriesNumeric.Focused) Then
                '    bsExitButton.PerformClick()

                'ElseIf (bsRejectionNumeric.Focused AndAlso bsRejectionNumeric.Text = String.Empty) Then
                '    bsRejectionNumeric.Text = bsRejectionNumeric.Value.ToString()
                '    bsResultErrorProv.Clear()

                'ElseIf (bsNumberOfSeriesNumeric.Focused AndAlso bsNumberOfSeriesNumeric.Text = String.Empty) Then
                '    bsNumberOfSeriesNumeric.Text = bsNumberOfSeriesNumeric.Value.ToString()
                '    bsResultErrorProv.Clear()
                'Else
                bsExitButton.PerformClick()
                'End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".IISECalibHistory_KeyDown ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".IISECalibHistory_KeyDown ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub IISEResultsHistory_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try
            If Me.DesignMode Then Exit Sub

            'Get an instance of the ISE manager class
            If Not AppDomain.CurrentDomain.GetData("GlobalAnalyzerManager") Is Nothing Then
                mdiAnalyzerCopy = CType(AppDomain.CurrentDomain.GetData("GlobalAnalyzerManager"), AnalyzerManager) ' Use the same AnalyzerManager as the MDI
                myISEManager = mdiAnalyzerCopy.ISE_Manager
            End If

            InitializeScreen()

            ResetBorder()
            Application.DoEvents()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".IISECalibHistory_Load ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".IISECalibHistory_Load ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub IISEResultsHistory_Shown(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Shown
        Try
            'If (bsTestSampleListView.Items.Count > 0) Then
            '    If (QCTestSampleIDAttribute > 0) Then
            '        'Select the Test/SampleType on the ListView
            '        SelectQCTestSampleID(QCTestSampleIDAttribute)
            '    Else
            '        'Select the first Test/SampleType on the ListView
            '        bsTestSampleListView.Items(0).Selected = True
            '    End If

            '    'TR 20/04/2012 - Validate the user level to activate/desactivate functionalities
            '    ScreenStatusByUserLevel()
            'End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".IISECalibHistory_Shown ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".IISECalibHistory_Shown ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub
#End Region

#Region " Button Events "
    Private Sub ExitButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsExitButton.Click
        Try
            If (Not Tag Is Nothing) Then
                'A PerformClick() method was executed
                Close()
            Else
                'Normal button click - Open the WS Monitor form and close this one
                IAx00MainMDI.OpenMonitorForm(Me)
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".ExitButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ExitButton_Click ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

#Region " Buttons in Eletrodes Tab "
    Private Sub bsElectrodesSearchButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsElectrodesSearchButton.Click
        GetElectrodeResults()
    End Sub

    Private Sub bsElectrodesGraphButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsElectrodesGraphButton.Click
        ShowElectrodeGraph()
    End Sub

    Private Sub bsElectrodesDeleteButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsElectrodesDeleteButton.Click
        DeleteSelectedRowsFromGrid(bsElectrodesGrid)
        GetElectrodeResults(True)
    End Sub

    Private Sub bsElectrodesPrintButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsElectrodesPrintButton.Click
        ShowElectrodeReport()
    End Sub
#End Region

#Region " Buttons in Conditioning Types Tab "
    Private Sub bsConditioningsSearchButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsConditioningSearchButton.Click
        GetConditioningResults()
    End Sub

    Private Sub bsConditioningsDeleteButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsConditioningDeleteButton.Click
        DeleteSelectedRowsFromGrid(bsConditioningGrid)
        GetConditioningResults(True)
    End Sub

    Private Sub bsConditioningsPrintButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsConditioningPrintButton.Click
        ShowConditioningReport()
    End Sub
#End Region
#End Region

#Region " Tab Events "
    Private Sub bsResultsTabs_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsResultsTabs.SelectedIndexChanged
        OnResultsTabSelectedIndexChanged()
    End Sub
#End Region

#Region " Grid Events "
    Private Sub grids_CellFormatting(ByVal sender As System.Object, ByVal e As DataGridViewCellFormattingEventArgs) Handles bsElectrodesGrid.CellFormatting, _
                                                                                                                            bsConditioningGrid.CellFormatting
        Try
            Dim grid As BSDataGridView = DirectCast(sender, BSDataGridView)
            Dim colName As String = grid.Columns(e.ColumnIndex).Name
            If colName = "ResultsNa" OrElse _
               colName = "ResultsK" OrElse _
               colName = "ResultsCl" OrElse _
               colName = "ResultsLi" OrElse _
               colName = "ErrorsString" Then
                If e.Value Is Nothing Then e.Value = vbCrLf
                e.CellStyle.WrapMode = DataGridViewTriState.True
            End If
            If colName = "ICON" AndAlso e.Value IsNot Nothing Then
                Dim iconStr As String = e.Value.ToString()
                e.Value = GetImage(iconStr)
            End If
            If Not String.IsNullOrEmpty(grid.Rows(e.RowIndex).Cells("ErrorsString").Value.ToString) Then
                e.CellStyle.ForeColor = Color.Red
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".grids_CellFormatting ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".grids_CellFormatting ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub
#End Region

#Region " DateTimePicker Events "
    Private Sub bsElectrodesDateFromDateTimePick_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsElectrodesDateFromDateTimePick.ValueChanged
        bsElectrodesDateToDateTimePick.MinDate = bsElectrodesDateFromDateTimePick.Value
    End Sub

    Private Sub bsElectrodesDateToDateTimePick_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsElectrodesDateToDateTimePick.ValueChanged
        bsElectrodesDateFromDateTimePick.MaxDate = bsElectrodesDateToDateTimePick.Value
    End Sub

    Private Sub bsConditioningDateFromDateTimePick_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsConditioningDateFromDateTimePick.ValueChanged
        bsConditioningDateToDateTimePick.MinDate = bsConditioningDateFromDateTimePick.Value
    End Sub

    Private Sub bsConditioningDateToDateTimePick_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsConditioningDateToDateTimePick.ValueChanged
        bsConditioningDateFromDateTimePick.MaxDate = bsConditioningDateToDateTimePick.Value
    End Sub
#End Region
#End Region

End Class


