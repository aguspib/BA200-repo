Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Global.GlobalEnumerates
Imports Biosystems.Ax00.Types
Imports DevExpress.XtraCharts
Imports Biosystems.Ax00.PresentationCOM

Public Class IQCGraphs

#Region "Declarations"
    Private LocalPoint As Point
    Private LabelSD As String = String.Empty
    Private LabelMEAN As String = String.Empty
    Private currentLanguage As String = String.Empty
    Private mDateFrom As DateTime
    Private mDateTo As DateTime
    Private mTestSampleId As Integer
    Private isLoading As Boolean     'To avoid double graph loading when the screen is opened
    Private myNewLocation As Point   'To avoid the screen movement
#End Region

#Region "Attribute"
    Private TestNameAttribute As String
    Private SampleTypeAttribute As String
    Private RejectionCriteriaAttribute As Single
    Private LocalDecimalAllowedAttribute As Integer
    Private OpenQCResultsDSAttribute As New OpenQCResultsDS
    Private QCResultsByControlDSAttribute As New QCResultsDS
#End Region

#Region "Properties"
    Public WriteOnly Property TestSampleID() As Integer
        Set(ByVal value As Integer)
            mTestSampleId = value
        End Set
    End Property

    Public WriteOnly Property DateFrom() As DateTime
        Set(ByVal value As DateTime)
            mDateFrom = value
        End Set
    End Property

    Public WriteOnly Property DateTo() As DateTime
        Set(ByVal value As DateTime)
            mDateTo = value
        End Set
    End Property

    Public WriteOnly Property TestName() As String
        Set(ByVal value As String)
            TestNameAttribute = value
        End Set
    End Property

    Public WriteOnly Property SampleType() As String
        Set(ByVal value As String)
            SampleTypeAttribute = value
        End Set
    End Property

    Public WriteOnly Property RejectionCriteria() As Single
        Set(ByVal value As Single)
            RejectionCriteriaAttribute = value
        End Set
    End Property

    Public WriteOnly Property DecimalAllowed() As Integer
        Set(ByVal value As Integer)
            LocalDecimalAllowedAttribute = value
        End Set
    End Property

    Public WriteOnly Property QCResultsByControlDS() As QCResultsDS
        Set(ByVal value As QCResultsDS)
            QCResultsByControlDSAttribute = value
        End Set
    End Property

    Public WriteOnly Property LocalOpenQCResultsDS() As OpenQCResultsDS
        Set(ByVal value As OpenQCResultsDS)
            OpenQCResultsDSAttribute = value
        End Set
    End Property
#End Region

#Region "Methods"
    ''' <summary>
    ''' Validate if the value of the point drawn contains an Error or Warning alarms and in that case, change the icon 
    ''' </summary>
    ''' <remarks>
    ''' Created by:  TR 17/06/2011
    ''' Modified by: SA 26/01/2012 - Use field containing "ControlName (LotNumber)" as identifier of the plotted series
    ''' </remarks>
    Private Sub CustomDrawSeriesPoints(ByVal sender As Object, ByVal e As CustomDrawSeriesPointEventArgs)
        Try
            Dim myArgumentValue As String = e.SeriesPoint.Argument.ToString

            If (bsLeveyJenningsRB.Checked) Then
                'LJ Graph
                CType(e.SeriesDrawOptions, LineDrawOptions).Marker.FillStyle.FillMode = FillMode.Solid

                'Filter by CalcRunNumber and ControlName to get the point values
                Dim rowErr As List(Of QCResultsDS.tqcResultsRow) = (From a In QCResultsByControlDSAttribute.tqcResults _
                                                                   Where a.CalcRunNumber = CType(myArgumentValue, Integer) _
                                                                 AndAlso a.ControlNameLotNum = DirectCast(e.Series, DevExpress.XtraCharts.Series).Name _
                                                                  Select a).ToList()

                If (rowErr.Count > 0) Then
                    If (Not rowErr(0).IsValidationStatusNull AndAlso rowErr(0).ValidationStatus = "ERROR") Then
                        e.SeriesDrawOptions.Color = Color.Red
                        CType(e.SeriesDrawOptions, LineDrawOptions).Marker.Kind = MarkerKind.Diamond
                    ElseIf (Not rowErr(0).IsValidationStatusNull AndAlso rowErr(0).ValidationStatus = "WARNING") Then
                        e.SeriesDrawOptions.Color = Color.DarkOrange
                        CType(e.SeriesDrawOptions, LineDrawOptions).Marker.Kind = MarkerKind.Square
                    Else
                        CType(e.SeriesDrawOptions, LineDrawOptions).Marker.Kind = MarkerKind.Cross
                    End If
                End If
            Else
                'Youden Graph
                Dim myPoint As SeriesPoint = TryCast(e.SeriesPoint, SeriesPoint)
                If (Not myPoint Is Nothing AndAlso Not myPoint.Tag Is Nothing) Then
                    'Validate if the n on the tag property is the last to change the icon
                    If (e.Series.Points(e.Series.Points.Count - 1).Tag = DirectCast(myPoint.Tag, Integer)) Then
                        CType(e.SeriesDrawOptions, PointDrawOptions).Marker.FillStyle.FillMode = FillMode.Solid
                        CType(e.SeriesDrawOptions, PointDrawOptions).Marker.Kind = MarkerKind.Star
                        e.SeriesDrawOptions.Color = Color.CornflowerBlue
                    Else
                        CType(e.SeriesDrawOptions, PointDrawOptions).Marker.FillStyle.FillMode = FillMode.Solid
                        CType(e.SeriesDrawOptions, PointDrawOptions).Marker.Kind = MarkerKind.Cross
                        e.SeriesDrawOptions.Color = Color.Black
                    End If
                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".CustomDrawSeriesPoints ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".CustomDrawSeriesPoints ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Get the Legend labels for Levey-Jennings graph
    ''' </summary>
    ''' <param name="pLanguageID">Current application Language</param>
    ''' <remarks>
    ''' Created by:  TR 21/06/2011
    ''' </remarks>
    Private Sub GetLegendsLabels(ByVal pLanguageID As String)
        Try
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            bsFirstCtrlLotLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_FirstCtrlLot", pLanguageID)
            bsFirstCtrlLotLabel.Enabled = False
            bsFirstCtrlLotPictureBox.Enabled = False

            bsSecondCtrlLotLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SecondCtrlLot", pLanguageID)
            bsSecondCtrlLotLabel.Enabled = False
            bsSecondCtrlLotPictureBox.Enabled = False

            bsThirdCtrlLotLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_ThirdCtrlLot", pLanguageID)
            bsThirdCtrlLotLabel.Enabled = False
            bsThirdCtrlLotPictureBox.Enabled = False
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".GetScreenLabels ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".GetScreenLabels ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Get texts in the current application Language for all screen controls
    ''' </summary>
    ''' <param name="pLanguageID">Current application Language</param>
    ''' <remarks>
    ''' Created by:  TR 14/06/2011
    ''' Modified by: SA 26/01/2012 - Get also text for new Title label for area of Results in the Controls/Lots section
    '''              DL 24/02/2012 - Get also text for controls in the legend shown when the selected type of Graph is Youden
    ''' </remarks>
    Private Sub GetScreenLabels(ByVal pLanguageID As String)
        Try
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            bsGraphicalResultLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "TITLE_GraphicalResultRvw", pLanguageID)
            bsTestLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_TestName", pLanguageID) + ":"
            bsSampleTypeLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SampleType", pLanguageID) + ":"
            bsRejectionLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Rejection", pLanguageID) & ":"
            bsSDLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SD", pLanguageID)
            bsGraphTypeLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Graph_Type", pLanguageID) & ":"
            bsLeveyJenningsRB.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_LeveyJennings", pLanguageID)
            bsYoudenRB.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Youden", pLanguageID)

            bsResultByControlLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "TITLE_Controls_List", pLanguageID)
            bsControlLotResultsLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Results", pLanguageID)
            bsLegendGroupBox.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Legend", pLanguageID)
            bsWarningLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_RotorPos_OpenWarnings", pLanguageID)
            bsErrorLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_ERROR", pLanguageID) 'JB 01/10/2012 - Resource String unification
            bsLegendYoudenGB.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Legend", pLanguageID)
            bsLastPointLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_LAST_RUNPOINT", pLanguageID)

            LabelSD = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SD", pLanguageID)
            LabelMEAN = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Mean", pLanguageID)

            bs1SDLabel.Text = "1 " & LabelSD
            bs2SDLabel.Text = "2 " & LabelSD
            bs3SDLabel.Text = "3 " & LabelSD
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".GetScreenLabels ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".GetScreenLabels ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Screen initializacion when loading
    ''' </summary>
    ''' <remarks>
    ''' Created by:  TR 14/06/2011
    ''' Modified by: SA 03/12/2013 - Set value of boolean variable isLoading to avoid load twice the LJ graph when the screen is opened
    ''' </remarks>
    Private Sub InitializeScreen()
        Try
            isLoading = True

            'Get the current Language from the current Application Session
            Dim currentLanguageGlobal As New GlobalBase
            currentLanguage = currentLanguageGlobal.GetSessionInfo().ApplicationLanguage.Trim

            GetScreenLabels(currentLanguage)
            GetLegendsLabels(currentLanguage)

            PrepareButtons(currentLanguage)
            PrepareResultControlLotGrid(currentLanguage)

            'Inform controls for Test Name and Rejection Criteria with the value of the correspondent attributes
            bsTestNameTextBox.Text = TestNameAttribute
            bsRejectionTextBox.Text = RejectionCriteriaAttribute

            'Select LJ Graph by default
            If (Not bsLeveyJenningsRB.Checked) Then bsLeveyJenningsRB.Checked = True
            isLoading = False
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".InitializeScreen ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".InitializeScreen ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Load the data for the type of graph selected
    ''' </summary>
    ''' <remarks>
    ''' Created by:  TR 16/06/2011
    ''' </remarks>
    Private Sub LoadGraphic()
        Try
            If (bsLeveyJenningsRB.Checked) Then
                LoadLeveyJenningsGraph()
            ElseIf (bsYoudenRB.Checked) Then
                LoadYoudenGraph()
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".LoadGraphic ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".LoadGraphic ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Load the ToolTips to shown in the selected graph
    ''' </summary>
    ''' <remarks>
    ''' Created by:  TR 17/06/2011
    ''' Modified by: SA 25/04/2012 - Use field containing "ControlName (LotNumber)" to search the list of alarms to link to the tooltip
    ''' </remarks>
    Private Sub ObjectHotTracked(ByVal sender As Object, ByVal e As DevExpress.XtraCharts.HotTrackEventArgs)
        Try
            Dim myPoint As SeriesPoint = TryCast(e.AdditionalObject, SeriesPoint)
            If (Not myPoint Is Nothing) Then
                'Get the RunNumber
                Dim myArgumentValue As String = DirectCast(e.AdditionalObject, DevExpress.XtraCharts.SeriesPoint).Argument
                Dim mySerie As Series = TryCast(e.Object, DevExpress.XtraCharts.Series)
                Dim myToolTip As String = ""

                If (bsLeveyJenningsRB.Checked) Then
                    myToolTip = mySerie.Name & Environment.NewLine
                    myToolTip &= "n: "
                    myToolTip &= myPoint.NumericalArgument.ToString("F0") & " "

                    Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate
                    myToolTip &= " - " & myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Tests_Value", currentLanguage) & _
                                 ": " & myPoint.Values(0).ToString("F" & LocalDecimalAllowedAttribute)

                ElseIf (bsYoudenRB.Checked) Then
                    myToolTip &= " n: " & myPoint.Tag & Environment.NewLine
                    myToolTip &= " x: " & myPoint.NumericalArgument.ToString("F" & LocalDecimalAllowedAttribute) & Environment.NewLine
                    myToolTip &= " y: " & myPoint.Values(0).ToString("F" & LocalDecimalAllowedAttribute)
                End If

                'Filter by CalcRunNumber and ControlName to get the result value
                Dim rowErr As List(Of QCResultsDS.tqcResultsRow) = (From a In QCResultsByControlDSAttribute.tqcResults _
                                                                   Where a.CalcRunNumber = CType(myArgumentValue, Integer) _
                                                                 AndAlso a.ControlNameLotNum = mySerie.Name _
                                                                  Select a).ToList()

                'Validate if the result has alarms
                If (rowErr.Count > 0 AndAlso Not rowErr(0).IsAlarmsListNull AndAlso Not rowErr(0).AlarmsList = String.Empty) Then
                    myToolTip &= Environment.NewLine
                    myToolTip &= rowErr(0).AlarmsList
                End If

                bsToolTipController.ToolTipLocation = DevExpress.Utils.ToolTipLocation.LeftTop
                bsToolTipController.ShowHint(myToolTip, LocalPoint)
            Else
                bsToolTipController.HideHint()
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".ObjectHotTracked ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ObjectHotTracked ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Set Icon and ToolTip of all screen buttons. Get Icons for screen Legend 
    ''' </summary>
    ''' <param name="pLanguageID">Current application Language</param>
    ''' <remarks>
    ''' Created by:  TR 14/06/2011
    ''' Modified by: DL 24/02/2012 - Added icons for 1SD, 2SD and 3SD lines in Youden Legend
    ''' </remarks>
    Private Sub PrepareButtons(ByVal pLanguageID As String)
        Try
            Dim auxIconName As String = ""
            Dim iconPath As String = MyBase.IconsPath

            Dim myToolTipsControl As New ToolTip
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            'PRINT Button
            auxIconName = GetIconName("PRINT")
            If (auxIconName <> "") Then
                bsPrintButton.Image = Image.FromFile(iconPath & auxIconName)
                myToolTipsControl.SetToolTip(bsPrintButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Print", pLanguageID))
            End If

            'EXIT Button
            auxIconName = GetIconName("CANCEL")
            If (auxIconName <> "") Then
                bsExitButton.Image = Image.FromFile(iconPath & auxIconName)
                myToolTipsControl.SetToolTip(bsExitButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_CloseScreen", pLanguageID))
            End If

            'LEGEND Icons
            auxIconName = GetIconName("GREEN_CIRCLE")
            If (auxIconName <> "") Then bsFirstCtrlLotPictureBox.Image = Image.FromFile(iconPath & auxIconName)

            auxIconName = GetIconName("BLUE_CIRCLE")
            If (auxIconName <> "") Then bsSecondCtrlLotPictureBox.Image = Image.FromFile(iconPath & auxIconName)

            auxIconName = GetIconName("VIOLET_CIRCLE")
            If (auxIconName <> "") Then bsThirdCtrlLotPictureBox.Image = Image.FromFile(iconPath & auxIconName)

            auxIconName = GetIconName("ORANGE_CIRCLE")
            If (auxIconName <> "") Then bsWarningPictureBox.Image = Image.FromFile(iconPath & auxIconName)

            auxIconName = GetIconName("RED_DIAMOND")
            If (auxIconName <> "") Then bsErrorPictureBox.Image = Image.FromFile(iconPath & auxIconName)

            auxIconName = GetIconName("BLUE_START")
            If (auxIconName <> "") Then bsLastRunPintImage.Image = Image.FromFile(iconPath & auxIconName)

            auxIconName = GetIconName("1SD")
            If (auxIconName <> "") Then bs1SDPictureBox.Image = Image.FromFile(iconPath & auxIconName)

            auxIconName = GetIconName("2SD")
            If (auxIconName <> "") Then bs2SDPictureBox.Image = Image.FromFile(iconPath & auxIconName)

            auxIconName = GetIconName("3SD")
            If (auxIconName <> "") Then bs3SDPictureBox.Image = Image.FromFile(iconPath & auxIconName)
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".PrepareButtons ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".PrepareButtons ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Prepare the DataGridView of Results by Control/Lot
    ''' </summary>
    ''' <param name="pLanguageID">Current application Language</param>
    ''' <remarks>
    ''' Created by:  TR 14/06/2011
    ''' Modified by: SA 05/12/2011 - Added three new visible columns for Calculated values of Mean, SD and CV. Added
    '''                              a dummy column to separate the assigned values and the result values      
    '''              SA 25/01/2012 - Column LotNumber is deleted due to column ControlName will shown ControlName (LotNumber)     
    '''                              Changed labels used for columns containing calculated Mean, SD and CV      
    ''' </remarks>
    Private Sub PrepareResultControlLotGrid(ByVal pLanguageID As String)
        Try
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            bsResultControlLotGridView.AutoSize = False
            bsResultControlLotGridView.MultiSelect = False
            bsResultControlLotGridView.AutoGenerateColumns = False
            bsResultControlLotGridView.AllowUserToResizeColumns = False
            bsResultControlLotGridView.Columns.Clear()

            'CheckBox to Select/Unselect the Control/Lot to be included in the graph
            Dim ActiveControlColChkBox As New DataGridViewCheckBoxColumn
            ActiveControlColChkBox.Width = 20
            ActiveControlColChkBox.Name = "ActiveControl"
            ActiveControlColChkBox.HeaderText = ""
            ActiveControlColChkBox.DataPropertyName = "Selected"
            ActiveControlColChkBox.Resizable = DataGridViewTriState.False
            bsResultControlLotGridView.Columns.Add(ActiveControlColChkBox)
            bsResultControlLotGridView.Columns("ActiveControl").ReadOnly = False

            'Control Name
            bsResultControlLotGridView.Columns.Add("ControlName", myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Control_Name", pLanguageID))
            bsResultControlLotGridView.Columns("ControlName").Width = 180
            bsResultControlLotGridView.Columns("ControlName").DataPropertyName = "ControlNameLotNum"
            bsResultControlLotGridView.Columns("ControlName").HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft
            bsResultControlLotGridView.Columns("ControlName").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
            bsResultControlLotGridView.Columns("ControlName").ReadOnly = True

            'Assigned Mean for the Test/SampleType and Control/Lot
            bsResultControlLotGridView.Columns.Add("Mean", LabelMEAN)
            bsResultControlLotGridView.Columns("Mean").Width = 65
            bsResultControlLotGridView.Columns("Mean").DataPropertyName = "Mean"
            bsResultControlLotGridView.Columns("Mean").DefaultCellStyle.NullValue = Nothing
            bsResultControlLotGridView.Columns("Mean").SortMode = DataGridViewColumnSortMode.NotSortable
            bsResultControlLotGridView.Columns("Mean").HeaderCell.Style.WrapMode = DataGridViewTriState.True
            bsResultControlLotGridView.Columns("Mean").HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight
            bsResultControlLotGridView.Columns("Mean").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            bsResultControlLotGridView.Columns("Mean").ReadOnly = True

            'Test Measure Unit
            bsResultControlLotGridView.Columns.Add("Unit", myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Unit", pLanguageID).TrimEnd())
            bsResultControlLotGridView.Columns("Unit").Width = 65
            bsResultControlLotGridView.Columns("Unit").DataPropertyName = "MeasureUnit"
            bsResultControlLotGridView.Columns("Unit").SortMode = DataGridViewColumnSortMode.NotSortable
            bsResultControlLotGridView.Columns("Unit").HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter
            bsResultControlLotGridView.Columns("Unit").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            bsResultControlLotGridView.Columns("Unit").ReadOnly = True

            'Assigned Standard Deviation for the Test/SampleType and Control/Lot
            bsResultControlLotGridView.Columns.Add("SD", myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SD", pLanguageID))
            bsResultControlLotGridView.Columns("SD").Width = 65
            bsResultControlLotGridView.Columns("SD").DataPropertyName = "SD"
            bsResultControlLotGridView.Columns("SD").DefaultCellStyle.NullValue = Nothing
            bsResultControlLotGridView.Columns("SD").SortMode = DataGridViewColumnSortMode.NotSortable
            bsResultControlLotGridView.Columns("SD").HeaderCell.Style.WrapMode = DataGridViewTriState.True
            bsResultControlLotGridView.Columns("SD").HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight
            bsResultControlLotGridView.Columns("SD").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            bsResultControlLotGridView.Columns("SD").ReadOnly = True

            'Assigned Coefficient of Variation for the Test/SampleType and Control/Lot
            bsResultControlLotGridView.Columns.Add("CV", myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_CV", pLanguageID))
            bsResultControlLotGridView.Columns("CV").Width = 55
            bsResultControlLotGridView.Columns("CV").DataPropertyName = "CV"
            bsResultControlLotGridView.Columns("CV").DefaultCellStyle.NullValue = Nothing
            bsResultControlLotGridView.Columns("CV").SortMode = DataGridViewColumnSortMode.NotSortable
            bsResultControlLotGridView.Columns("CV").HeaderCell.Style.WrapMode = DataGridViewTriState.True
            bsResultControlLotGridView.Columns("CV").HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight
            bsResultControlLotGridView.Columns("CV").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            bsResultControlLotGridView.Columns("CV").ReadOnly = True

            'Assigned Min/Max Concentration values for the Test/SampleType and Control/Lot
            bsResultControlLotGridView.Columns.Add("Ranges", myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Ranges", pLanguageID).TrimEnd())
            bsResultControlLotGridView.Columns("Ranges").Width = 120
            bsResultControlLotGridView.Columns("Ranges").DataPropertyName = "Ranges"
            bsResultControlLotGridView.Columns("Ranges").SortMode = DataGridViewColumnSortMode.NotSortable
            bsResultControlLotGridView.Columns("Ranges").HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter
            bsResultControlLotGridView.Columns("Ranges").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            bsResultControlLotGridView.Columns("Ranges").ReadOnly = True

            'Separator
            bsResultControlLotGridView.Columns.Add("Dummy", "")
            bsResultControlLotGridView.Columns("Dummy").Width = 5
            bsResultControlLotGridView.Columns("Dummy").ReadOnly = True

            'Number of open QC Results (not included in the statistical calculation of the Mean) for the Test/SampleType and Control/Lot
            bsResultControlLotGridView.Columns.Add("n", "n")
            bsResultControlLotGridView.Columns("n").Width = 35
            bsResultControlLotGridView.Columns("n").DataPropertyName = "n"
            bsResultControlLotGridView.Columns("n").HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight
            bsResultControlLotGridView.Columns("n").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            bsResultControlLotGridView.Columns("n").ReadOnly = True

            'Additional columns to show Mean, SD and CV calculated from all obtained open QC Results
            'Calculated Mean of open QC Results for the Test/SampleType and Control/Lot
            bsResultControlLotGridView.Columns.Add("CalcMean", LabelMEAN)
            bsResultControlLotGridView.Columns("CalcMean").Width = 65
            bsResultControlLotGridView.Columns("CalcMean").DataPropertyName = "CalcMean"
            bsResultControlLotGridView.Columns("CalcMean").DefaultCellStyle.NullValue = Nothing
            bsResultControlLotGridView.Columns("CalcMean").SortMode = DataGridViewColumnSortMode.NotSortable
            bsResultControlLotGridView.Columns("CalcMean").HeaderCell.Style.WrapMode = DataGridViewTriState.True
            bsResultControlLotGridView.Columns("CalcMean").HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight
            bsResultControlLotGridView.Columns("CalcMean").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            bsResultControlLotGridView.Columns("CalcMean").ReadOnly = True

            'Calculated Standard Deviation for the Test/SampleType and Control/Lot
            bsResultControlLotGridView.Columns.Add("CalcSD", myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SD", pLanguageID))
            bsResultControlLotGridView.Columns("CalcSD").Width = 65
            bsResultControlLotGridView.Columns("CalcSD").DataPropertyName = "CalcSD"
            bsResultControlLotGridView.Columns("CalcSD").DefaultCellStyle.NullValue = Nothing
            bsResultControlLotGridView.Columns("CalcSD").SortMode = DataGridViewColumnSortMode.NotSortable
            bsResultControlLotGridView.Columns("CalcSD").HeaderCell.Style.WrapMode = DataGridViewTriState.True
            bsResultControlLotGridView.Columns("CalcSD").HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight
            bsResultControlLotGridView.Columns("CalcSD").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            bsResultControlLotGridView.Columns("CalcSD").ReadOnly = True

            'Calculated Coefficient of Variation for the Test/SampleType and Control/Lot
            bsResultControlLotGridView.Columns.Add("CalcCV", myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_CV", pLanguageID))
            bsResultControlLotGridView.Columns("CalcCV").Width = 55
            bsResultControlLotGridView.Columns("CalcCV").DataPropertyName = "CalcCV"
            bsResultControlLotGridView.Columns("CalcCV").DefaultCellStyle.NullValue = Nothing
            bsResultControlLotGridView.Columns("CalcCV").HeaderCell.Style.WrapMode = DataGridViewTriState.True
            bsResultControlLotGridView.Columns("CalcCV").SortMode = DataGridViewColumnSortMode.NotSortable
            bsResultControlLotGridView.Columns("CalcCV").HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight
            bsResultControlLotGridView.Columns("CalcCV").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            bsResultControlLotGridView.Columns("CalcCV").ReadOnly = True

            'Not visible columns...
            '** Identifier of the Control/Lot in QC Module
            bsResultControlLotGridView.Columns.Add("QCControlLotID", "QCControlLotID")
            bsResultControlLotGridView.Columns("QCControlLotID").DataPropertyName = "QCControlLotID"
            bsResultControlLotGridView.Columns("QCControlLotID").Visible = False

            '** Runs Group Number
            bsResultControlLotGridView.Columns.Add("RunsGroupNumber", "RunsGroupNumber")
            bsResultControlLotGridView.Columns("RunsGroupNumber").DataPropertyName = "RunsGroupNumber"
            bsResultControlLotGridView.Columns("RunsGroupNumber").Visible = False

            '** Number of the last Run included in the statistical calculation (only when Calculation Mode = STATISTICS)
            bsResultControlLotGridView.Columns.Add("LastMeanRunNumber", "LastMeanRunNumber")
            bsResultControlLotGridView.Columns("LastMeanRunNumber").DataPropertyName = "LastMeanRunNumber"
            bsResultControlLotGridView.Columns("LastMeanRunNumber").Visible = False
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".PrepareResultControlLotGrid ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".PrepareResultControlLotGrid ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Validate if the number of selected Controls is OK according the selected Graph type:
    ''' ** For LJ Graph: a maximum of three Controls can be selected
    ''' ** For Youden Graph: a maximum of two Controls can be selected
    ''' </summary>
    ''' <returns>True if the number of Controls selected for the correspondent Graph Type; otherwise, False</returns>
    ''' <remarks>
    ''' Created by:  TR 21/06/2011
    ''' </remarks>
    Private Function ValidateActiveControls() As Boolean
        Dim myResult As Boolean = False
        Try
            Dim maxNumOfControls As Integer = 0

            If (bsYoudenRB.Checked) Then
                maxNumOfControls = 3
            ElseIf (bsLeveyJenningsRB.Checked) Then
                maxNumOfControls = 4
            End If

            myResult = ((From a In OpenQCResultsDSAttribute.tOpenResults _
                    Where Not a.IsSelectedNull AndAlso a.Selected _
                       Select a).Count < maxNumOfControls)
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".ValidateActiveControls ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ValidateActiveControls ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
        Return myResult
    End Function

    Private Sub ReleaseElements()

        Try
            '--- Detach variable defined using WithEvents ---
            bsGraphicalResultsGroupBox = Nothing
            bsGraphicalResultLabel = Nothing
            bsSDLabel = Nothing
            bsRejectionLabel = Nothing
            bsSampleTypeLabel = Nothing
            bsTestLabel = Nothing
            bsGraphicalAreaGroupBox = Nothing
            bsYoudenRB = Nothing
            bsLeveyJenningsRB = Nothing
            bsResultByControlLabel = Nothing
            bsResultControlLotGridView = Nothing
            bsExitButton = Nothing
            bsPrintButton = Nothing
            bsQCResultChartControl = Nothing
            bsLegendGroupBox = Nothing
            bsToolTipController = Nothing
            bsTestNameTextBox = Nothing
            bsSampleTypeTextBox = Nothing
            bsRejectionTextBox = Nothing
            bsEventLog = Nothing
            bsSecondCtrlLotPictureBox = Nothing
            bsFirstCtrlLotLabel = Nothing
            bsSecondCtrlLotLabel = Nothing
            bsFirstCtrlLotPictureBox = Nothing
            bsThirdCtrlLotLabel = Nothing
            bsThirdCtrlLotPictureBox = Nothing
            bsWarningPictureBox = Nothing
            bsWarningLabel = Nothing
            bsErrorLabel = Nothing
            bsErrorPictureBox = Nothing
            bsGraphTypeLabel = Nothing
            bsLegendYoudenGB = Nothing
            bsLastPointLabel = Nothing
            bsLastRunPintImage = Nothing
            bsControlLotResultsLabel = Nothing
            bs3SDLabel = Nothing
            bs3SDPictureBox = Nothing
            bs2SDLabel = Nothing
            bs2SDPictureBox = Nothing
            bs1SDLabel = Nothing
            bs1SDPictureBox = Nothing
            '-----------------------------------------------
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ReleaseElements ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ReleaseElements ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try

    End Sub

#End Region

#Region "Methods for LEVEY-JENNINGS Graph"
    ''' <summary>
    ''' Draw a constant line in Y-Axis of a Levey-Jennings graph (for Mean and SD bars)
    ''' </summary>
    ''' <param name="pName">Title for the Constant line</param>
    ''' <param name="pDiagram">Diagram in which the Constant line will be added</param>
    ''' <param name="pValue">Value in Y-Axis in which the Constant line will be drawn</param>
    ''' <param name="pColor">Color for the Constant line</param>
    ''' <param name="pDashStyle">Dash Style for the Constant line</param>
    ''' <remarks>
    ''' Created by:  TR 17/06/2011
    ''' </remarks>
    Private Sub CreateConstantLine(ByVal pName As String, ByVal pDiagram As XYDiagram, ByVal pValue As Single, ByVal pColor As Color, _
                                   ByVal pDashStyle As DashStyle)
        Try
            'Create the constant line
            Dim constantLine As New ConstantLine(pName)
            pDiagram.AxisY.ConstantLines.Add(constantLine)

            'Define its axis value
            constantLine.AxisValue = pValue

            'Customize the behavior of the constant line
            constantLine.Visible = True
            constantLine.ShowInLegend = True
            constantLine.LegendText = pName
            constantLine.ShowBehind = False

            'Customize the constant line's title
            constantLine.Title.Visible = True
            constantLine.Title.Text = pName
            constantLine.Title.TextColor = pColor
            constantLine.Title.Antialiasing = False
            constantLine.Title.Font = New Font("Verdana", 8, FontStyle.Regular)
            constantLine.Title.ShowBelowLine = True
            constantLine.Title.Alignment = ConstantLineTitleAlignment.Far

            'Customize the appearance of the constant line
            constantLine.Color = pColor
            constantLine.LineStyle.Thickness = 1
            constantLine.LineStyle.DashStyle = pDashStyle
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".CreateConstantLine ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".CreateConstantLine ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Create a DataTable that will be used as DataSource of the Levey-Jennings graph 
    ''' </summary>
    ''' <param name="pXAxisValues">List of integer values to load in the column for Axis-X values ("Argument")</param>
    ''' <returns>DataTable containing a column for Axis-X values ("Argument") and a column for each one of the possible Series to add to the graphic</returns>
    ''' <remarks>
    ''' Created by:  SA 02/12/2013 - BT#1392 
    ''' </remarks>
    Private Function CreateChartData(ByVal pXAxisValues As List(Of Integer)) As DataTable
        Dim myDataSourceTable As New DataTable("LeveyJenningsTable")

        Try
            'Add Column for X-Axis Values
            myDataSourceTable.Columns.Add("Argument", GetType(Int32))

            'Add Column for Point Values of the Serie
            myDataSourceTable.Columns.Add("Values", GetType(Single))

            'Fill Argument Column with the list of values in the entry parameter pXAxisValues
            Dim newTableRow As DataRow = Nothing
            For Each numSerie As Integer In pXAxisValues
                newTableRow = myDataSourceTable.NewRow()
                newTableRow("Argument") = numSerie
                myDataSourceTable.Rows.Add(newTableRow)
            Next
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".CreateChartData ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".CreateChartData ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
        Return myDataSourceTable
    End Function


    ''' <summary>
    ''' Load the Levey Jennings Graph for the selected Controls
    ''' </summary>
    ''' <remarks>
    ''' Created by:  TR 16/06/2011
    ''' Modified by: SA 18/01/2012 - Changed LINQ used to calculate the num of Controls selected with results that can be plotted
    '''              SA 26/01/2012 - Use field containing "ControlName (LotNumber)" as identifier of each serie added to the plot
    '''              SA 02/05/2012 - Added labels for +/- SD and Mean below the dotted lines on the right side of the graphic for 
    '''                              both cases: one or more Controls plotted
    '''              DL 31/05/2012 - Set the X-Axis scale to numeric
    '''              SA 02/12/2013 - BT #1392 ==> Changes to draw correctly the Levey-Jennings graph when several Controls have been
    '''                                           selected (X-Axis values in the previous version were wrong and the graph has not sense)
    '''              SA 13/06/2014 - BT #1665 ==> Property AxisX.Range.Auto has to be initialized to TRUE (it is needed when LJ Graph is
    '''                                           reloaded after drawn the Youden Graph, which use a not automatic Range) 
    '''              SA 20/06/2014 - BT #1668 ==> When two Controls are drawn, value of property ArgumentScaleType has to be set to Qualitative
    '''                                           to prevent the values in X-Axis are scaled with decimals (Series are always integer)  
    '''              SA 25/09/2014 - BA-1608  ==> In the Linq used to get the list of selected Controls, condition a.n=0 is wrong; it should be
    '''                                           a.n > 0 (it is an old error, but its unique efect was that values in Y-Axis were normalized, 
    '''                                           although only one Control was plotted)
    ''' </remarks>
    Private Sub LoadLeveyJenningsGraph()
        Try
            'Get the Legend labels and show the frame
            GetLegendsLabels(currentLanguage)
            bsLegendGroupBox.Visible = True

            'Change the Graphic control size and set the location
            bsQCResultChartControl.Size = New Size(817, 374)
            bsQCResultChartControl.Location = New Drawing.Point(10, 155)

            'Initialize the Graphic control
            bsQCResultChartControl.ClearCache()
            bsQCResultChartControl.Series.Clear()
            bsQCResultChartControl.Legend.Visible = False
            bsQCResultChartControl.SeriesTemplate.ValueScaleType = ScaleType.Numerical
            bsQCResultChartControl.BackColor = Color.White
            bsQCResultChartControl.AppearanceName = "Light"

            'Get the list of selected Controls
            Dim mySelectedCtrlLots As List(Of OpenQCResultsDS.tOpenResultsRow) = (From a As OpenQCResultsDS.tOpenResultsRow In OpenQCResultsDSAttribute.tOpenResults _
                                                                                 Where (Not a.IsSelectedNull AndAlso a.Selected) _
                                                                               AndAlso (Not a.IsCalcMeanNull OrElse (Not a.IsMeanNull AndAlso a.n > 0)) _
                                                                                Select a).ToList
            Dim numSelectedWithMean As Integer = mySelectedCtrlLots.Count
            bsPrintButton.Enabled = (numSelectedWithMean > 0)

            Dim myDiagram As New XYDiagram
            Dim maxRelError As Double = 0
            Dim validResultValues As List(Of QCResultsDS.tqcResultsRow)
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            Dim myXRange As List(Of Integer)
            Dim myDataSourceTable As DataTable

            Dim mySeriesCount As Integer = 1
            For Each openQCResultRow As OpenQCResultsDS.tOpenResultsRow In mySelectedCtrlLots
                'Get RunNumbers to be used as Axis-X values for this Control
                myXRange = (From a As QCResultsDS.tqcResultsRow In QCResultsByControlDSAttribute.tqcResults _
                           Where a.QCControlLotID = openQCResultRow.QCControlLotID _
                     AndAlso Not a.Excluded _
                        Order By a.CalcRunNumber _
                          Select a.CalcRunNumber).ToList

                'Create the table that will be used as DataSource for the Serie that will be used for this Control
                myDataSourceTable = CreateChartData(myXRange)

                'Add a new Serie for the Selected Control
                bsQCResultChartControl.Series.Add(openQCResultRow.ControlNameLotNum, ViewType.Line)
                bsQCResultChartControl.Series(openQCResultRow.ControlNameLotNum).ShowInLegend = True
                bsQCResultChartControl.Series(openQCResultRow.ControlNameLotNum).Label.Visible = False
                bsQCResultChartControl.Series(openQCResultRow.ControlNameLotNum).PointOptions.PointView = PointView.ArgumentAndValues

                'Set the color for each Control/Lot to graph; inform the Legend text with the Control Name
                If (bsQCResultChartControl.Series.Count = 1) Then
                    bsQCResultChartControl.Series(openQCResultRow.ControlNameLotNum).View.Color = Color.Green
                    bsFirstCtrlLotLabel.Text = openQCResultRow.ControlName & Environment.NewLine
                    bsFirstCtrlLotLabel.Enabled = True

                ElseIf (bsQCResultChartControl.Series.Count = 2) Then
                    bsQCResultChartControl.Series(openQCResultRow.ControlNameLotNum).View.Color = Color.Blue
                    bsSecondCtrlLotLabel.Text = openQCResultRow.ControlName & Environment.NewLine
                    bsSecondCtrlLotLabel.Enabled = True

                ElseIf (bsQCResultChartControl.Series.Count = 3) Then
                    bsQCResultChartControl.Series(openQCResultRow.ControlNameLotNum).View.Color = Color.DarkViolet
                    bsThirdCtrlLotLabel.Text = openQCResultRow.ControlName & Environment.NewLine
                    bsThirdCtrlLotLabel.Enabled = True
                End If

                myDiagram = CType(bsQCResultChartControl.Diagram, XYDiagram)
                myDiagram.AxisY.ConstantLines.Clear()
                myDiagram.AxisX.ConstantLines.Clear()
                myDiagram.AxisX.Range.Auto = True
                myDiagram.AxisY.GridLines.Visible = False
                myDiagram.AxisX.GridLines.Visible = False
                myDiagram.AxisX.Title.Visible = False
                myDiagram.AxisY.Title.Visible = False

                If (numSelectedWithMean = 1) Then
                    'Only one Control is selected to be graph
                    CreateConstantLine(LabelMEAN, myDiagram, openQCResultRow.Mean, Color.Black, DashStyle.Solid)

                    If (openQCResultRow.SD > 0) Then
                        'Create the Constant line for the Rejection Criteria
                        If (RejectionCriteriaAttribute = 1) Then
                            CreateConstantLine("+1 " & LabelSD, myDiagram, openQCResultRow.Mean + (1 * openQCResultRow.SD), Color.Red, DashStyle.Solid)
                            CreateConstantLine("-1 " & LabelSD, myDiagram, openQCResultRow.Mean - (1 * openQCResultRow.SD), Color.Red, DashStyle.Solid)
                        Else
                            CreateConstantLine("+1 " & LabelSD, myDiagram, openQCResultRow.Mean + (1 * openQCResultRow.SD), Color.Black, DashStyle.Dash)
                            CreateConstantLine("-1 " & LabelSD, myDiagram, openQCResultRow.Mean - (1 * openQCResultRow.SD), Color.Black, DashStyle.Dash)
                        End If

                        If (RejectionCriteriaAttribute = 2) Then
                            CreateConstantLine("+2 " & LabelSD, myDiagram, openQCResultRow.Mean + (2 * openQCResultRow.SD), Color.Red, DashStyle.Solid)
                            CreateConstantLine("-2 " & LabelSD, myDiagram, openQCResultRow.Mean - (2 * openQCResultRow.SD), Color.Red, DashStyle.Solid)
                        Else
                            CreateConstantLine("+2 " & LabelSD, myDiagram, openQCResultRow.Mean + (2 * openQCResultRow.SD), Color.Black, DashStyle.Dash)
                            CreateConstantLine("-2 " & LabelSD, myDiagram, openQCResultRow.Mean - (2 * openQCResultRow.SD), Color.Black, DashStyle.Dash)
                        End If

                        If (RejectionCriteriaAttribute = 3) Then
                            CreateConstantLine("+3 " & LabelSD, myDiagram, openQCResultRow.Mean + (3 * openQCResultRow.SD), Color.Red, DashStyle.Solid)
                            CreateConstantLine("-3 " & LabelSD, myDiagram, openQCResultRow.Mean - (3 * openQCResultRow.SD), Color.Red, DashStyle.Solid)
                        Else
                            CreateConstantLine("+3 " & LabelSD, myDiagram, openQCResultRow.Mean + (3 * openQCResultRow.SD), Color.Black, DashStyle.Dash)
                            CreateConstantLine("-3 " & LabelSD, myDiagram, openQCResultRow.Mean - (3 * openQCResultRow.SD), Color.Black, DashStyle.Dash)
                        End If

                        If (RejectionCriteriaAttribute Mod 1 OrElse RejectionCriteriaAttribute >= 4) Then
                            CreateConstantLine("+4 " & LabelSD, myDiagram, openQCResultRow.Mean + (RejectionCriteriaAttribute * openQCResultRow.SD), Color.Red, DashStyle.Solid)
                            CreateConstantLine("-4 " & LabelSD, myDiagram, openQCResultRow.Mean - (RejectionCriteriaAttribute * openQCResultRow.SD), Color.Red, DashStyle.Solid)
                        End If
                    End If

                    'Set the limits for Axis Y
                    If (RejectionCriteriaAttribute < 1) Then
                        maxRelError = (From a In QCResultsByControlDSAttribute.tqcResults _
                                      Where a.QCControlLotID = openQCResultRow.QCControlLotID _
                                    AndAlso Not a.Excluded _
                                     Select a.RELError).Max

                        If (Math.Round(openQCResultRow.Mean + (maxRelError * openQCResultRow.SD), 3) < Math.Round(openQCResultRow.Mean + (RejectionCriteriaAttribute * openQCResultRow.SD))) Then
                            maxRelError = RejectionCriteriaAttribute
                        End If

                        myDiagram.AxisY.Range.SetMinMaxValues(Math.Round(openQCResultRow.Mean - (maxRelError * openQCResultRow.SD), 3) - 10, _
                                                              Math.Round(openQCResultRow.Mean + (maxRelError * openQCResultRow.SD), 3) + 10)
                    Else
                        If (openQCResultRow.SD > 0) Then
                            myDiagram.AxisY.Range.SetMinMaxValues(Math.Round(openQCResultRow.Mean - (4 * openQCResultRow.SD), 3), _
                                                                  Math.Round(openQCResultRow.Mean + (4 * openQCResultRow.SD), 3))
                        End If
                    End If

                    For Each runNumber As DataRow In myDataSourceTable.Rows
                        'Search value of the Control/Lot for the Run Number 
                        validResultValues = (From a As QCResultsDS.tqcResultsRow In QCResultsByControlDSAttribute.tqcResults _
                                            Where a.QCControlLotID = openQCResultRow.QCControlLotID _
                                          AndAlso a.CalcRunNumber = runNumber("Argument") _
                                         Order By a.CalcRunNumber _
                                           Select a).ToList

                        If (validResultValues.Count = 1) Then
                            runNumber("Values") = validResultValues.First.VisibleResultValue
                        End If
                    Next
                    myDataSourceTable.AcceptChanges()

                    bsQCResultChartControl.Series(openQCResultRow.ControlNameLotNum).DataSource = myDataSourceTable
                    bsQCResultChartControl.Series(openQCResultRow.ControlNameLotNum).ArgumentDataMember = "Argument"
                    'When only ONE Control is selected, this property is not used to allow that values of X-Axis shown exactly the 
                    'RunNumbers for which the Control has a not excluded value
                    'bsQCResultChartControl.Series(openQCResultRow.ControlNameLotNum).ArgumentScaleType = ScaleType.Numerical  
                    bsQCResultChartControl.Series(openQCResultRow.ControlNameLotNum).ValueScaleType = ScaleType.Numerical
                    bsQCResultChartControl.Series(openQCResultRow.ControlNameLotNum).ValueDataMembers.AddRange(New String() {"Values"})

                    For i As Integer = 0 To bsQCResultChartControl.Series(openQCResultRow.ControlNameLotNum).Points.Count - 1
                        bsQCResultChartControl.Series(openQCResultRow.ControlNameLotNum).Points(i).Tag = bsQCResultChartControl.Series(openQCResultRow.ControlNameLotNum).Points(i).Values(0).ToString
                    Next

                    'Set margins
                    myDiagram.Margins.Right = 5

                    'Set the Title for each axis
                    myDiagram.AxisX.Title.Visible = True
                    myDiagram.AxisX.Title.Antialiasing = False
                    myDiagram.AxisX.Title.TextColor = Color.Black
                    myDiagram.AxisX.Title.Alignment = StringAlignment.Center
                    myDiagram.AxisX.Title.Font = New Font("Verdana", 8.25, FontStyle.Regular)
                    myDiagram.AxisX.Title.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Serie", currentLanguage)

                    myDiagram.AxisY.Title.Visible = True
                    myDiagram.AxisY.Title.Antialiasing = False
                    myDiagram.AxisY.Title.TextColor = Color.Black
                    myDiagram.AxisY.Title.Alignment = StringAlignment.Center
                    myDiagram.AxisY.Title.Font = New Font("Verdana", 8.25, FontStyle.Regular)
                    myDiagram.AxisY.Title.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Concentration_Long", currentLanguage)
                Else
                    If (mySeriesCount = numSelectedWithMean) Then
                        'Several Controls are selected to be graph
                        CreateConstantLine(LabelMEAN, myDiagram, 0, Color.Black, DashStyle.Solid)

                        'Create the Constant line for the Rejection Criteria
                        If (RejectionCriteriaAttribute = 1) Then
                            CreateConstantLine("+1 " & LabelSD, myDiagram, 1, Color.Red, DashStyle.Solid)
                            CreateConstantLine("-1 " & LabelSD, myDiagram, -1, Color.Red, DashStyle.Solid)
                        Else
                            CreateConstantLine("+1 " & LabelSD, myDiagram, 1, Color.Black, DashStyle.Dash)
                            CreateConstantLine("-1 " & LabelSD, myDiagram, -1, Color.Black, DashStyle.Dash)
                        End If

                        If (RejectionCriteriaAttribute = 2) Then
                            CreateConstantLine("+2 " & LabelSD, myDiagram, 2, Color.Red, DashStyle.Solid)
                            CreateConstantLine("-2 " & LabelSD, myDiagram, -2, Color.Red, DashStyle.Solid)
                        Else
                            CreateConstantLine("+2 " & LabelSD, myDiagram, 2, Color.Black, DashStyle.Dash)
                            CreateConstantLine("-2 " & LabelSD, myDiagram, -2, Color.Black, DashStyle.Dash)
                        End If

                        If (RejectionCriteriaAttribute = 3) Then
                            CreateConstantLine("+3 " & LabelSD, myDiagram, 3, Color.Red, DashStyle.Solid)
                            CreateConstantLine("-3 " & LabelSD, myDiagram, -3, Color.Red, DashStyle.Solid)
                        Else
                            CreateConstantLine("+3 " & LabelSD, myDiagram, 3, Color.Black, DashStyle.Dash)
                            CreateConstantLine("-3 " & LabelSD, myDiagram, -3, Color.Black, DashStyle.Dash)
                        End If

                        If (RejectionCriteriaAttribute Mod 1 OrElse RejectionCriteriaAttribute >= 4) Then
                            CreateConstantLine("+" & RejectionCriteriaAttribute.ToString() & LabelSD, myDiagram, RejectionCriteriaAttribute, Color.Red, DashStyle.Solid)
                            CreateConstantLine("-" & RejectionCriteriaAttribute.ToString() & LabelSD, myDiagram, -RejectionCriteriaAttribute, Color.Red, DashStyle.Solid)
                        End If

                        'Set the limits for Axis Y
                        If (RejectionCriteriaAttribute < 1) Then
                            maxRelError = (From a In OpenQCResultsDSAttribute.tOpenResults Join b In QCResultsByControlDSAttribute.tqcResults _
                                                                                             On a.QCControlLotID Equals b.QCControlLotID _
                                      Where Not a.IsSelectedNull AndAlso a.Selected _
                                    AndAlso Not a.IsCalcMeanNull _
                                    AndAlso Not b.Excluded _
                                         Select b.RELError).Max

                            If (maxRelError < RejectionCriteriaAttribute) Then
                                maxRelError = RejectionCriteriaAttribute
                                myDiagram.AxisY.Range.SetMinMaxValues(-maxRelError - 0.1, maxRelError + 0.1)
                            Else
                                myDiagram.AxisY.Range.SetMinMaxValues(-maxRelError - 0.5, maxRelError + 0.5)
                            End If
                        Else
                            myDiagram.AxisY.Range.SetMinMaxValues(-4.5, 4.5)
                        End If
                    End If

                    For Each runNumber As DataRow In myDataSourceTable.Rows
                        'Search value of the Control/Lot for the Run Number 
                        validResultValues = (From a As QCResultsDS.tqcResultsRow In QCResultsByControlDSAttribute.tqcResults _
                                            Where a.QCControlLotID = openQCResultRow.QCControlLotID _
                                          AndAlso a.CalcRunNumber = runNumber("Argument") _
                                         Order By a.CalcRunNumber _
                                           Select a).ToList

                        If (validResultValues.Count = 1) Then
                            runNumber("Values") = validResultValues.First.RELError
                        End If
                    Next
                    myDataSourceTable.AcceptChanges()

                    bsQCResultChartControl.Series(openQCResultRow.ControlNameLotNum).DataSource = myDataSourceTable
                    bsQCResultChartControl.Series(openQCResultRow.ControlNameLotNum).ArgumentDataMember = "Argument"
                    'BT #1668 - When SEVERAL Controls have been selected, the ArgumentScaleType has to be set to QUALITATIVE 
                    bsQCResultChartControl.Series(openQCResultRow.ControlNameLotNum).ArgumentScaleType = ScaleType.Qualitative 'ScaleType.Numerical
                    bsQCResultChartControl.Series(openQCResultRow.ControlNameLotNum).ValueScaleType = ScaleType.Numerical
                    bsQCResultChartControl.Series(openQCResultRow.ControlNameLotNum).ValueDataMembers.AddRange(New String() {"Values"})

                    For i As Integer = 0 To bsQCResultChartControl.Series(openQCResultRow.ControlNameLotNum).Points.Count - 1
                        If (bsQCResultChartControl.Series(openQCResultRow.ControlNameLotNum).Points(i).Values(0) > 0) Then
                            bsQCResultChartControl.Series(openQCResultRow.ControlNameLotNum).Points(i).Tag = bsQCResultChartControl.Series(openQCResultRow.ControlNameLotNum).Points(i).Values(0).ToString
                        End If
                    Next

                    If (mySeriesCount = numSelectedWithMean) Then
                        'Set margins
                        myDiagram.Margins.Right = 5

                        'Set the Title for each axis
                        myDiagram.AxisX.Title.Visible = True
                        myDiagram.AxisX.Title.Antialiasing = False
                        myDiagram.AxisX.Title.TextColor = Color.Black
                        myDiagram.AxisX.Title.Alignment = StringAlignment.Center
                        myDiagram.AxisX.Title.Font = New Font("Verdana", 8.25, FontStyle.Regular)
                        myDiagram.AxisX.Title.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Serie", currentLanguage)

                        myDiagram.AxisY.Title.Visible = True
                        myDiagram.AxisY.Title.Antialiasing = False
                        myDiagram.AxisY.Title.Text = "  "
                    End If
                End If
                mySeriesCount += 1
            Next

            RemoveHandler bsQCResultChartControl.ObjectHotTracked, AddressOf ObjectHotTracked
            AddHandler bsQCResultChartControl.ObjectHotTracked, AddressOf ObjectHotTracked

            RemoveHandler bsQCResultChartControl.CustomDrawSeriesPoint, AddressOf CustomDrawSeriesPoints
            AddHandler bsQCResultChartControl.CustomDrawSeriesPoint, AddressOf CustomDrawSeriesPoints
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".LoadLeveyJenningsGraph ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".LoadLeveyJenningsGraph ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub
#End Region

#Region "Methods for YOUDEN Graph"
    ''' <summary>
    ''' Draw a constant line in X-Axis of a Youden Graph
    ''' </summary>
    ''' <param name="pName">Title for the Constant line</param>
    ''' <param name="pDiagram">Diagram in which the Constant line will be added</param>
    ''' <param name="pValue">Value in X-Axis in which the Constant line will be drawn</param>
    ''' <param name="pColor">Color for the Constant line</param>
    ''' <remarks>
    ''' Created by:  TR 17/06/2011
    ''' </remarks>
    Private Sub CreateConstantLineAxisX(ByVal pName As String, ByVal pDiagram As XYDiagram, ByVal pValue As Double, ByVal pColor As Color)
        Try
            'Create the constant line
            Dim constantLine As New ConstantLine(pName)
            pDiagram.AxisX.ConstantLines.Add(constantLine)

            'Define its axis value
            constantLine.AxisValue = pValue

            'Customize the behavior of the constant line
            constantLine.Visible = True
            constantLine.ShowInLegend = True
            constantLine.LegendText = pName
            constantLine.ShowBehind = False

            'Customize the constant line's title
            constantLine.Title.Visible = True
            constantLine.Title.Text = pName
            constantLine.Title.TextColor = pColor
            constantLine.Title.Antialiasing = False
            constantLine.Title.ShowBelowLine = False
            constantLine.Title.Alignment = ConstantLineTitleAlignment.Far
            constantLine.Title.Font = New Font("Verdana", 8, FontStyle.Bold)

            'Customize the appearance of the constant line.
            constantLine.Color = pColor
            constantLine.LineStyle.Thickness = 1
            constantLine.LineStyle.DashStyle = DashStyle.Solid
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".CreateConstantLineAxisX ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".CreateConstantLineAxisX ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Draw a constant line in Y-Axis of a Youden Graph
    ''' </summary>
    ''' <param name="pName">Title for the Constant line</param>
    ''' <param name="pDiagram">Diagram in which the Constant line will be added</param>
    ''' <param name="pValue">Value in Y-Axis in which the Constant line will be drawn</param>
    ''' <param name="pColor">Color for the Constant line</param>
    ''' <remarks>
    ''' Created by:  TR 17/06/2011
    ''' </remarks>
    Private Sub CreateConstantLineAxisY(ByVal pName As String, ByVal pDiagram As XYDiagram, ByVal pValue As Single, ByVal pColor As Color)
        Try
            'Create the constant line
            Dim constantLine As New ConstantLine(pName)
            pDiagram.AxisY.ConstantLines.Add(constantLine)

            'Define its axis value
            constantLine.AxisValue = pValue

            'Customize the behavior of the constant line
            constantLine.Visible = True
            constantLine.ShowInLegend = True
            constantLine.LegendText = pName
            constantLine.ShowBehind = False

            'Customize the constant line's title
            constantLine.Title.Visible = True
            constantLine.Title.Text = pName
            constantLine.Title.TextColor = pColor
            constantLine.Title.ShowBelowLine = False
            constantLine.Title.Antialiasing = False
            constantLine.Title.Alignment = ConstantLineTitleAlignment.Far
            constantLine.Title.Font = New Font("Verdana", 8, FontStyle.Bold)

            'Customize the appearance of the constant line.
            constantLine.Color = pColor
            constantLine.LineStyle.Thickness = 1
            constantLine.LineStyle.DashStyle = DashStyle.Solid
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".CreateConstantLineAxisY ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".CreateConstantLineAxisY ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Create the SD Squares in a Youden Graph
    ''' </summary>
    ''' <param name="pControl1Mean">Mean of the first selected Control</param>
    ''' <param name="pControl1SD">SD of the first selected Control</param>
    ''' <param name="pControl2Mean">Mean of the second selected Control</param>
    ''' <param name="pControl2SD">SD of the second selected Control</param>
    ''' <remarks>
    ''' Created by:  TR 17/06/2011
    ''' </remarks>
    Private Sub CreateSquares(ByVal pControl1Mean As Double, ByVal pControl1SD As Double, _
                              ByVal pControl2Mean As Double, ByVal pControl2SD As Double)
        Try
            Dim myLineSeriesView As LineSeriesView

            '****************'
            '*  Square SD1  *'
            '****************'
            Dim series4 As New Series("SD1L1", ViewType.Line)
            series4.Points.Add(New SeriesPoint((pControl1Mean + pControl1SD), (pControl2Mean + pControl2SD)))
            series4.Points.Add(New SeriesPoint((pControl1Mean - pControl1SD), (pControl2Mean + pControl2SD)))

            myLineSeriesView = CType(series4.View, LineSeriesView)
            myLineSeriesView.LineMarkerOptions.Visible = False
            myLineSeriesView.Color = Color.Fuchsia
            myLineSeriesView.LineStyle.DashStyle = DashStyle.Dash
            myLineSeriesView.LineStyle.Thickness = 2

            series4.PointOptions.PointView = PointView.Values
            series4.ArgumentScaleType = ScaleType.Numerical
            series4.ValueScaleType = ScaleType.Numerical
            series4.Label.Visible = False

            Dim series41 As New Series("SD1L2", ViewType.Line)
            series41.Points.Add(New SeriesPoint(pControl1Mean - (pControl1SD), pControl2Mean + (pControl2SD)))
            series41.Points.Add(New SeriesPoint(pControl1Mean - (pControl1SD), (pControl2Mean - (pControl2SD))))

            myLineSeriesView = CType(series41.View, LineSeriesView)
            myLineSeriesView.LineMarkerOptions.Visible = False
            myLineSeriesView.Color = Color.Fuchsia
            myLineSeriesView.LineStyle.DashStyle = DashStyle.Dash
            myLineSeriesView.LineStyle.Thickness = 2

            series41.PointOptions.PointView = PointView.Values
            series41.ArgumentScaleType = ScaleType.Numerical
            series41.ValueScaleType = ScaleType.Numerical
            series41.Label.Visible = False

            Dim series42 As New Series("SD1L3", ViewType.Line)
            series42.Points.Add(New SeriesPoint(pControl1Mean - (pControl1SD), pControl2Mean - (pControl2SD)))
            series42.Points.Add(New SeriesPoint(pControl1Mean + (pControl1SD), (pControl2Mean - (pControl2SD))))

            myLineSeriesView = CType(series42.View, LineSeriesView)
            myLineSeriesView.LineMarkerOptions.Visible = False
            myLineSeriesView.Color = Color.Fuchsia
            myLineSeriesView.LineStyle.DashStyle = DashStyle.Dash
            myLineSeriesView.LineStyle.Thickness = 2

            series42.PointOptions.PointView = PointView.Values
            series42.ArgumentScaleType = ScaleType.Numerical
            series42.ValueScaleType = ScaleType.Numerical
            series42.Label.Visible = False

            Dim series43 As New Series("SD1L4", ViewType.Line)
            series43.Points.Add(New SeriesPoint(pControl1Mean + (pControl1SD), pControl2Mean - (pControl2SD)))
            series43.Points.Add(New SeriesPoint(pControl1Mean + (pControl1SD), (pControl2Mean + (pControl2SD))))

            myLineSeriesView = CType(series43.View, LineSeriesView)
            myLineSeriesView.LineMarkerOptions.Visible = False
            myLineSeriesView.Color = Color.Fuchsia
            myLineSeriesView.LineStyle.DashStyle = DashStyle.Dash
            myLineSeriesView.LineStyle.Thickness = 2

            series43.PointOptions.PointView = PointView.Values
            series43.ArgumentScaleType = ScaleType.Numerical
            series43.ValueScaleType = ScaleType.Numerical
            series43.Label.Visible = False

            '****************'
            '*  Square SD2  *'
            '****************'
            Dim series40 As New Series("SD2L1", ViewType.Line)
            series40.Points.Add(New SeriesPoint(pControl1Mean + 2 * (pControl1SD), pControl2Mean + 2 * (pControl2SD)))
            series40.Points.Add(New SeriesPoint(pControl1Mean - 2 * (pControl1SD), (pControl2Mean + 2 * (pControl2SD))))

            myLineSeriesView = CType(series40.View, LineSeriesView)
            myLineSeriesView.LineMarkerOptions.Visible = False
            myLineSeriesView.Color = Color.Orange  'Color.Black
            myLineSeriesView.LineStyle.DashStyle = DashStyle.Dash
            myLineSeriesView.LineStyle.Thickness = 2

            series40.PointOptions.PointView = PointView.Values
            series40.ArgumentScaleType = ScaleType.Numerical
            series40.ValueScaleType = ScaleType.Numerical
            series40.Label.Visible = False

            Dim series410 As New Series("SD2L2", ViewType.Line)
            series410.Points.Add(New SeriesPoint(pControl1Mean - 2 * (pControl1SD), pControl2Mean + 2 * (pControl2SD)))
            series410.Points.Add(New SeriesPoint(pControl1Mean - 2 * (pControl1SD), (pControl2Mean - 2 * (pControl2SD))))

            myLineSeriesView = CType(series410.View, LineSeriesView)
            myLineSeriesView.LineMarkerOptions.Visible = False
            myLineSeriesView.Color = Color.Orange  'Color.Black
            myLineSeriesView.LineStyle.DashStyle = DashStyle.Dash
            myLineSeriesView.LineStyle.Thickness = 2

            series410.PointOptions.PointView = PointView.Values
            series410.ArgumentScaleType = ScaleType.Numerical
            series410.ValueScaleType = ScaleType.Numerical
            series410.Label.Visible = False

            Dim series420 As New Series("SD2L3", ViewType.Line)
            series420.Points.Add(New SeriesPoint(pControl1Mean - 2 * (pControl1SD), pControl2Mean - 2 * (pControl2SD)))
            series420.Points.Add(New SeriesPoint(pControl1Mean + 2 * (pControl1SD), (pControl2Mean - 2 * (pControl2SD))))

            myLineSeriesView = CType(series420.View, LineSeriesView)
            myLineSeriesView.LineMarkerOptions.Visible = False
            myLineSeriesView.Color = Color.Orange
            myLineSeriesView.LineStyle.DashStyle = DashStyle.Dash
            myLineSeriesView.LineStyle.Thickness = 2

            series420.PointOptions.PointView = PointView.Values
            series420.ArgumentScaleType = ScaleType.Numerical
            series420.ValueScaleType = ScaleType.Numerical
            series420.Label.Visible = False

            Dim series430 As New Series("SD2L4", ViewType.Line)
            series430.Points.Add(New SeriesPoint(pControl1Mean + 2 * (pControl1SD), pControl2Mean - 2 * (pControl2SD)))
            series430.Points.Add(New SeriesPoint(pControl1Mean + 2 * (pControl1SD), (pControl2Mean + 2 * (pControl2SD))))

            myLineSeriesView = CType(series430.View, LineSeriesView)
            myLineSeriesView.LineMarkerOptions.Visible = False
            myLineSeriesView.Color = Color.Orange
            myLineSeriesView.LineStyle.Thickness = 2
            myLineSeriesView.LineStyle.DashStyle = DashStyle.Dash

            series430.PointOptions.PointView = PointView.Values
            series430.ArgumentScaleType = ScaleType.Numerical
            series430.ValueScaleType = ScaleType.Numerical
            series430.Label.Visible = False

            '****************'
            '*  Square SD3  *'
            '****************'
            Dim series50 As New Series("SD3L1", ViewType.Line)
            series50.Points.Add(New SeriesPoint(pControl1Mean + 3 * (pControl1SD), pControl2Mean + 3 * (pControl2SD)))
            series50.Points.Add(New SeriesPoint(pControl1Mean - 3 * (pControl1SD), (pControl2Mean + 3 * (pControl2SD))))

            myLineSeriesView = CType(series50.View, LineSeriesView)
            myLineSeriesView.LineMarkerOptions.Visible = False
            myLineSeriesView.Color = Color.Green
            myLineSeriesView.LineStyle.Thickness = 2
            myLineSeriesView.LineStyle.DashStyle = DashStyle.Dash

            series50.PointOptions.PointView = PointView.Values
            series50.ArgumentScaleType = ScaleType.Numerical
            series50.ValueScaleType = ScaleType.Numerical
            series50.Label.Visible = False

            Dim series51 As New Series("SD3L2", ViewType.Line)
            series51.Points.Add(New SeriesPoint(pControl1Mean - 3 * (pControl1SD), pControl2Mean + 3 * (pControl2SD)))
            series51.Points.Add(New SeriesPoint(pControl1Mean - 3 * (pControl1SD), (pControl2Mean - 3 * (pControl2SD))))

            myLineSeriesView = CType(series51.View, LineSeriesView)
            myLineSeriesView.LineMarkerOptions.Visible = False
            myLineSeriesView.Color = Color.Green
            myLineSeriesView.LineStyle.Thickness = 2
            myLineSeriesView.LineStyle.DashStyle = DashStyle.Dash

            series51.PointOptions.PointView = PointView.Values
            series51.ArgumentScaleType = ScaleType.Numerical
            series51.ValueScaleType = ScaleType.Numerical
            series51.Label.Visible = False

            Dim series52 As New Series("SD3L3", ViewType.Line)
            series52.Points.Add(New SeriesPoint(pControl1Mean - 3 * (pControl1SD), pControl2Mean - 3 * (pControl2SD)))
            series52.Points.Add(New SeriesPoint(pControl1Mean + 3 * (pControl1SD), (pControl2Mean - 3 * (pControl2SD))))

            myLineSeriesView = CType(series52.View, LineSeriesView)
            myLineSeriesView.LineMarkerOptions.Visible = False
            myLineSeriesView.Color = Color.Green
            myLineSeriesView.LineStyle.Thickness = 2
            myLineSeriesView.LineStyle.DashStyle = DashStyle.Dash

            series52.PointOptions.PointView = PointView.Values
            series52.ArgumentScaleType = ScaleType.Numerical
            series52.ValueScaleType = ScaleType.Numerical
            series52.Label.Visible = False

            Dim series53 As New Series("SD3L4", ViewType.Line)
            series53.Points.Add(New SeriesPoint(pControl1Mean + 3 * (pControl1SD), pControl2Mean - 3 * (pControl2SD)))
            series53.Points.Add(New SeriesPoint(pControl1Mean + 3 * (pControl1SD), (pControl2Mean + 3 * (pControl2SD))))

            myLineSeriesView = CType(series53.View, LineSeriesView)
            myLineSeriesView.LineMarkerOptions.Visible = False
            myLineSeriesView.Color = Color.Green
            myLineSeriesView.LineStyle.Thickness = 2
            myLineSeriesView.LineStyle.DashStyle = DashStyle.Dash

            series53.PointOptions.PointView = PointView.Values
            series53.ArgumentScaleType = ScaleType.Numerical
            series53.ValueScaleType = ScaleType.Numerical
            series53.Label.Visible = False

            '*******************'
            '*  Diagonal Line  *'
            '*******************'
            Dim series60 As New Series("tan45º", ViewType.Line)
            series60.Points.Add(New SeriesPoint(pControl1Mean - 7 * (pControl1SD), pControl2Mean - 7 * (pControl2SD)))
            series60.Points.Add(New SeriesPoint(pControl1Mean + 7 * (pControl1SD), (pControl2Mean + 7 * (pControl2SD))))

            myLineSeriesView = CType(series60.View, LineSeriesView)
            myLineSeriesView.LineMarkerOptions.Visible = False
            myLineSeriesView.Color = Color.Tomato   'Color.Red
            myLineSeriesView.LineStyle.Thickness = 2

            series60.PointOptions.PointView = PointView.Values
            series60.ArgumentScaleType = ScaleType.Numerical
            series60.ValueScaleType = ScaleType.Numerical
            series60.Label.Visible = False


            bsQCResultChartControl.Series.AddRange(New Series() {series4, series41, series42, series43, series40, series410, _
                                                   series420, series430, series50, series51, series52, series53, series60})
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".CreateSquares ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".CreateSquares ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Load the Youden Graph for the selected Controls
    ''' </summary>
    ''' <remarks>
    ''' Created by:  TR 17/06/2011
    ''' Modified by: SA 23/12/2011 - If there are more than two Control/Lots selected, unselect the last one to allow drawing
    '''                              the plot
    '''              SA 26/01/2012 - Use field containing "ControlName (LotNumber)" as identifier of each serie added to the plot
    '''              SA 25/09/2014 - BA-1608 ==> Before drawn the graph, verify the selected Controls have at least a not exclude result
    ''' </remarks>
    Private Sub LoadYoudenGraph()
        Try
            bsLegendGroupBox.Visible = False

            'Change the Graphic control size and set the location
            bsQCResultChartControl.Size = New Size(485, 435) '(485, 440)
            bsQCResultChartControl.Location = New Drawing.Point(140, 155) 'New Drawing.Point(170, 155) '(170, 147)

            bsQCResultChartControl.ClearCache()
            bsQCResultChartControl.Series.Clear()
            bsQCResultChartControl.Legend.Visible = False

            'Get the list of selected Controls
            Dim mySelectecControlLotList As List(Of OpenQCResultsDS.tOpenResultsRow) = (From a As OpenQCResultsDS.tOpenResultsRow In OpenQCResultsDSAttribute.tOpenResults _
                                                                                   Where Not a.IsSelectedNull AndAlso a.Selected = True _
                                                                                      Select a).ToList()
            Dim numOfSelectedCtrls As Integer = mySelectecControlLotList.Count

            'BA-1608 - Verify for each selected Control that it has at least a not excluded result to plot and unselect Controls that not fulfill this conditions
            '          If the number of selected Controls changes, get again the selected Controls (the ones that remain selected)
            If (numOfSelectedCtrls > 0) Then
                Dim validResults As Boolean = False
                For Each selControl As OpenQCResultsDS.tOpenResultsRow In mySelectecControlLotList
                    'Verify if the Control has at least a not excluded result to plot; otherwise, set Selected = False for it
                    validResults = (QCResultsByControlDSAttribute.tqcResults.ToList.Where(Function(a) a.QCControlLotID = selControl.QCControlLotID AndAlso a.Excluded = False).Count > 0)
                    If (Not validResults) Then selControl.Selected = False
                Next


                If (OpenQCResultsDSAttribute.tOpenResults.ToList.Where(Function(b) Not b.IsSelectedNull AndAlso b.Selected = True).Count <> numOfSelectedCtrls) Then
                    'The number of selected Controls has changed, get the group of Controls that remains selected (if any) and count them
                    mySelectecControlLotList = (From a As OpenQCResultsDS.tOpenResultsRow In OpenQCResultsDSAttribute.tOpenResults _
                                           Where Not a.IsSelectedNull AndAlso a.Selected = True _
                                              Select a).ToList()
                    numOfSelectedCtrls = mySelectecControlLotList.Count
                End If
            End If

            If (numOfSelectedCtrls > 0) Then
                If (numOfSelectedCtrls > 2) Then
                    'If there are more than two Control/Lots selected, the last one is unselected
                    mySelectecControlLotList.Last.Selected = False
                    numOfSelectedCtrls -= 1
                End If

                bsQCResultChartControl.Series.Add(mySelectecControlLotList.First().ControlNameLotNum, ViewType.Point)
                bsQCResultChartControl.Series(mySelectecControlLotList.First().ControlNameLotNum).ShowInLegend = True
                bsQCResultChartControl.Series(mySelectecControlLotList.First().ControlNameLotNum).Label.Visible = False
                bsQCResultChartControl.Series(mySelectecControlLotList.First().ControlNameLotNum).PointOptions.PointView = PointView.Values
                bsQCResultChartControl.Series(mySelectecControlLotList.First().ControlNameLotNum).ArgumentScaleType = ScaleType.Numerical
                bsQCResultChartControl.Series(mySelectecControlLotList.First().ControlNameLotNum).ValueScaleType = ScaleType.Numerical
                bsQCResultChartControl.Series(mySelectecControlLotList.First().ControlNameLotNum).View.Color = Color.Black

                Dim myDiagram As XYDiagram = CType(bsQCResultChartControl.Diagram, XYDiagram)
                myDiagram.AxisY.ConstantLines.Clear()
                myDiagram.AxisX.ConstantLines.Clear()
                myDiagram.AxisY.Range.Auto = False
                myDiagram.AxisX.Range.Auto = False

                bsPrintButton.Enabled = True

                Dim XResultValues As New List(Of Single)
                If (numOfSelectedCtrls = 1) Then
                    If (mySelectecControlLotList.First().IsSDNull) Then
                        'There are not enough Results to drawn the graph
                        ShowMessage("Warning", GlobalEnumerates.Messages.STATISTICAL_LACK.ToString())

                        mySelectecControlLotList.First().Selected = False
                        bsResultControlLotGridView.Refresh()

                        bsQCResultChartControl.ClearCache()
                        bsQCResultChartControl.Series.Clear()
                    Else
                        'Drawn the Youden Graph for the selected Control...

                        'Set Margins
                        myDiagram.Margins.Right = 40

                        'Set values for X-Axis and Y-Axis
                        XResultValues = (From a In QCResultsByControlDSAttribute.tqcResults _
                                        Where a.QCControlLotID = mySelectecControlLotList.First().QCControlLotID _
                                      AndAlso a.ControlNameLotNum = mySelectecControlLotList.First().ControlNameLotNum _
                                  AndAlso Not a.Excluded _
                                       Select a.VisibleResultValue).ToList()

                        myDiagram.AxisX.Range.SetMinMaxValues(Math.Round(mySelectecControlLotList.First().Mean - (3 * mySelectecControlLotList.First().SD), 3) - 1, _
                                                              Math.Round(mySelectecControlLotList.First().Mean + (3 * mySelectecControlLotList.First().SD), 3) + 1)
                        myDiagram.AxisX.Title.Visible = True
                        myDiagram.AxisX.Title.Antialiasing = False
                        myDiagram.AxisX.Title.TextColor = Color.Black
                        myDiagram.AxisX.Title.Alignment = StringAlignment.Center
                        myDiagram.AxisX.Title.Font = New Font("Verdana", 8, FontStyle.Regular)
                        myDiagram.AxisX.Title.Text = mySelectecControlLotList.First().ControlName

                        myDiagram.AxisY.Range.SetMinMaxValues(Math.Round(mySelectecControlLotList.First().Mean - (3 * mySelectecControlLotList.First().SD), 3) - 1, _
                                                              Math.Round(mySelectecControlLotList.First().Mean + (3 * mySelectecControlLotList.First().SD), 3) + 1)
                        myDiagram.AxisY.Title.Visible = True
                        myDiagram.AxisY.Title.Antialiasing = False
                        myDiagram.AxisY.Title.TextColor = Color.Black
                        myDiagram.AxisY.Title.Alignment = StringAlignment.Center
                        myDiagram.AxisY.Title.Font = New Font("Verdana", 8, FontStyle.Regular)
                        myDiagram.AxisY.Title.Text = mySelectecControlLotList.Last().ControlName

                        'Create the graph squares
                        CreateSquares(mySelectecControlLotList.First().Mean, mySelectecControlLotList.First().SD, _
                                      mySelectecControlLotList.Last().Mean, mySelectecControlLotList.Last().SD)

                        'Drawn the points in the graph
                        For Each qcResultRow As QCResultsDS.tqcResultsRow In QCResultsByControlDSAttribute.tqcResults.Rows
                            If (Not qcResultRow.Excluded AndAlso qcResultRow.ControlNameLotNum = mySelectecControlLotList.First().ControlNameLotNum) Then
                                bsQCResultChartControl.Series(mySelectecControlLotList.First().ControlNameLotNum).Points.Add(New SeriesPoint(qcResultRow.VisibleResultValue, _
                                                                                                                                             qcResultRow.VisibleResultValue))
                                bsQCResultChartControl.Series(mySelectecControlLotList.First().ControlNameLotNum).Points(bsQCResultChartControl.Series(mySelectecControlLotList.First().ControlNameLotNum).Points.Count - 1).Tag = qcResultRow.CalcRunNumber
                            End If
                        Next

                        'Create cross lines with the Control Mean
                        CreateConstantLineAxisX(mySelectecControlLotList.First().Mean.ToString("F2"), myDiagram, mySelectecControlLotList.First().Mean, Color.Blue)
                        CreateConstantLineAxisY(mySelectecControlLotList.First().Mean.ToString("F2"), myDiagram, mySelectecControlLotList.First().Mean, Color.Blue)
                    End If

                ElseIf (numOfSelectedCtrls = 2) Then
                    If (mySelectecControlLotList.First().IsSDNull OrElse mySelectecControlLotList.Last().IsSDNull) Then
                        'There are not enough Results to drawn the graph...
                        ShowMessage("Warning", GlobalEnumerates.Messages.STATISTICAL_LACK.ToString())

                        mySelectecControlLotList.First().Selected = False
                        mySelectecControlLotList.Last().Selected = False
                        bsResultControlLotGridView.Refresh()

                        bsQCResultChartControl.ClearCache()
                        bsQCResultChartControl.Series.Clear()
                    Else
                        'Drawn the Youden Graph for the pair of selected Controls...

                        'Set Margins
                        myDiagram.Margins.Right = 40

                        'Set values to X-Axis
                        XResultValues = (From a In QCResultsByControlDSAttribute.tqcResults _
                                        Where a.QCControlLotID = mySelectecControlLotList.First().QCControlLotID _
                                      AndAlso a.ControlNameLotNum = mySelectecControlLotList.First().ControlNameLotNum _
                                  AndAlso Not a.Excluded _
                                       Select a.VisibleResultValue).ToList()

                        Dim MinValue As Single = Math.Round(mySelectecControlLotList.First().Mean - (3 * mySelectecControlLotList.First().SD), 3)
                        If (MinValue > XResultValues.Min) Then MinValue = XResultValues.Min

                        Dim MaxValue As Single = Math.Round(mySelectecControlLotList.First().Mean + (3 * mySelectecControlLotList.First().SD), 3)
                        If (MaxValue < XResultValues.Max) Then MaxValue = XResultValues.Max

                        myDiagram.AxisX.Range.SetMinMaxValues(MinValue - 1, MaxValue + 1)
                        myDiagram.AxisX.Title.Visible = True
                        myDiagram.AxisX.Title.Antialiasing = False
                        myDiagram.AxisX.Title.TextColor = Color.Black
                        myDiagram.AxisX.Title.Alignment = StringAlignment.Center
                        myDiagram.AxisX.Title.Font = New Font("Verdana", 8, FontStyle.Regular)
                        myDiagram.AxisX.Title.Text = mySelectecControlLotList.First().ControlName

                        'Set values to Y-Axis
                        Dim YResultValues As List(Of Single) = (From a In QCResultsByControlDSAttribute.tqcResults _
                                                               Where a.QCControlLotID = mySelectecControlLotList.Last().QCControlLotID _
                                                             AndAlso a.ControlName = mySelectecControlLotList.Last().ControlName _
                                                         AndAlso Not a.Excluded _
                                                              Select a.VisibleResultValue).ToList()

                        MinValue = Math.Round(mySelectecControlLotList.Last().Mean - (3 * mySelectecControlLotList.Last().SD), 3)
                        If (MinValue > YResultValues.Min) Then MinValue = YResultValues.Min

                        MaxValue = Math.Round(mySelectecControlLotList.Last().Mean + (3 * mySelectecControlLotList.Last().SD), 3)
                        If (MaxValue < YResultValues.Max) Then MaxValue = YResultValues.Max

                        myDiagram.AxisY.Range.SetMinMaxValues(MinValue - 1, MaxValue + 1)
                        myDiagram.AxisY.Title.Visible = True
                        myDiagram.AxisY.Title.Antialiasing = False
                        myDiagram.AxisY.Title.TextColor = Color.Black
                        myDiagram.AxisY.Title.Alignment = StringAlignment.Center
                        myDiagram.AxisY.Title.Font = New Font("Verdana", 8, FontStyle.Regular)
                        myDiagram.AxisY.Title.Text = mySelectecControlLotList.Last().ControlName

                        'Create cross lines with the Mean of selected Controls
                        CreateConstantLineAxisX(mySelectecControlLotList.First().Mean.ToString("F2"), myDiagram, mySelectecControlLotList.First().Mean, Color.Blue)
                        CreateConstantLineAxisY(mySelectecControlLotList.Last().Mean.ToString("F2"), myDiagram, mySelectecControlLotList.Last().Mean, Color.Blue)

                        'Create the graph squares
                        CreateSquares(mySelectecControlLotList.First().Mean, mySelectecControlLotList.First().SD, _
                                      mySelectecControlLotList.Last().Mean, mySelectecControlLotList.Last().SD)

                        'Drawn the points in the graph
                        Dim SecondControlValue As New List(Of QCResultsDS.tqcResultsRow)
                        For Each qcResultRow As QCResultsDS.tqcResultsRow In QCResultsByControlDSAttribute.tqcResults.Rows
                            If (Not qcResultRow.Excluded AndAlso qcResultRow.ControlNameLotNum = mySelectecControlLotList.First().ControlNameLotNum) Then
                                'Get result values for second control.
                                SecondControlValue = (From a In QCResultsByControlDSAttribute.tqcResults _
                                                     Where a.QCControlLotID = mySelectecControlLotList.Last().QCControlLotID _
                                                   AndAlso a.CalcRunNumber = qcResultRow.CalcRunNumber _
                                                    Select a).ToList()

                                If (SecondControlValue.Count > 0) Then
                                    bsQCResultChartControl.Series(mySelectecControlLotList.First().ControlNameLotNum).Points.Add(New SeriesPoint(qcResultRow.VisibleResultValue, _
                                                                                                                                                 SecondControlValue.First().VisibleResultValue))
                                    bsQCResultChartControl.Series(mySelectecControlLotList.First().ControlNameLotNum).Points(bsQCResultChartControl.Series(mySelectecControlLotList.First().ControlNameLotNum).Points.Count - 1).Tag = qcResultRow.CalcRunNumber
                                End If
                            End If
                        Next
                    End If
                Else
                    myDiagram = CType(bsQCResultChartControl.Diagram, XYDiagram)
                    myDiagram.AxisY.ConstantLines.Clear()
                    myDiagram.AxisX.ConstantLines.Clear()

                    myDiagram.AxisY.Title.Visible = False
                    myDiagram.AxisX.Title.Visible = False

                    bsPrintButton.Enabled = False
                End If
            Else
                bsPrintButton.Enabled = False
            End If

            RemoveHandler bsQCResultChartControl.CustomDrawSeriesPoint, AddressOf CustomDrawSeriesPoints
            AddHandler bsQCResultChartControl.CustomDrawSeriesPoint, AddressOf CustomDrawSeriesPoints

            RemoveHandler bsQCResultChartControl.ObjectHotTracked, AddressOf ObjectHotTracked
            AddHandler bsQCResultChartControl.ObjectHotTracked, AddressOf ObjectHotTracked
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".LoadYoudenGraph ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".LoadYoudenGraph ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub
#End Region

#Region "Events"
    '*******************
    '** SCREEN EVENTS **
    '*******************
    Private Sub IQCGraphs_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles MyBase.KeyDown
        Try
            If (e.KeyCode = Keys.Escape) Then bsExitButton.PerformClick()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".IQCGraphs_KeyDown ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".IQCGraphs_KeyDown ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub IQCGraphs_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try
            InitializeScreen()

            'Get description of the informed SampleType
            Dim myGlobalDataTO As New GlobalDataTO
            Dim myMasterDataDelegate As New MasterDataDelegate()

            myGlobalDataTO = myMasterDataDelegate.GetList(Nothing, GlobalEnumerates.MasterDataEnum.SAMPLE_TYPES.ToString)
            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                Dim qSampleType As List(Of MasterDataDS.tcfgMasterDataRow) = DirectCast(myGlobalDataTO.SetDatos, MasterDataDS).tcfgMasterData.ToList()
                If (qSampleType.Where(Function(a) a.ItemID = SampleTypeAttribute).Count > 0) Then
                    bsSampleTypeTextBox.Text = qSampleType.Where(Function(a) a.ItemID = SampleTypeAttribute).First().ItemIDDesc
                End If
            Else
                'Error getting the SampleType description; shown it
                ShowMessage(Name & ".IQCGraphs_Load ", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage, Me)
            End If
            Dim mySize As Size = IAx00MainMDI.Size
            Dim myLocation As Point = IAx00MainMDI.Location

            If (Not Me.MdiParent Is Nothing) Then
                mySize = Me.Parent.Size
                myLocation = Me.Parent.Location
            End If

            myNewLocation = New Point(myLocation.X + CInt((mySize.Width - Me.Width) / 2), myLocation.Y + CInt((mySize.Height - Me.Height) / 2))
            Me.Location = myNewLocation
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".IAddManualQCResultsAux_Load ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".IAddManualQCResultsAux_Load ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Not allow move form and mantain the center location in center parent
    ''' </summary>
    Protected Overrides Sub WndProc(ByRef m As Message)
        Try
            If (m.Msg = WM_WINDOWPOSCHANGING) Then
                Dim pos As WINDOWPOS = DirectCast(Runtime.InteropServices.Marshal.PtrToStructure(m.LParam, GetType(WINDOWPOS)), WINDOWPOS)

                Dim mySize As Size = IAx00MainMDI.Size
                Dim myLocation As Point = IAx00MainMDI.Location
                If (Not Me.MdiParent Is Nothing) Then
                    mySize = Me.Parent.Size
                    myLocation = Me.Parent.Location
                End If

                pos.x = myLocation.X + CInt((mySize.Width - Me.Width) / 2)
                pos.y = myLocation.Y + CInt((mySize.Height - Me.Height) / 2)
                Runtime.InteropServices.Marshal.StructureToPtr(pos, m.LParam, True)
            End If

            MyBase.WndProc(m)
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WndProc " & Me.Name, EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & "WndProc", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub IQCGraphs_Shown(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Shown
        Try
            If (OpenQCResultsDSAttribute.tOpenResults.Count > 0) Then
                bsResultControlLotGridView.DataSource = OpenQCResultsDSAttribute.tOpenResults

                Dim numSelectedWithMean As Integer = (From a In OpenQCResultsDSAttribute.tOpenResults _
                                                     Where (Not a.IsSelectedNull AndAlso a.Selected) _
                                                   AndAlso (Not a.IsCalcMeanNull OrElse (Not a.IsMeanNull AndAlso a.n = 0)) _
                                                    Select a).Count

                If (numSelectedWithMean > 0) Then
                    LoadGraphic()
                Else
                    ShowMessage("Warning", GlobalEnumerates.Messages.STATISTICAL_LACK.ToString())
                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".IQCGraphs_Shown ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".IQCGraphs_Shown ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    '****************************************************
    '** EVENTS FOR RESULTS BY CONTROL/LOT DATAGRIDVIEW **
    '****************************************************
    Private Sub ResultControlLotGridView_CellFormatting(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellFormattingEventArgs) Handles bsResultControlLotGridView.CellFormatting
        Try
            If (Not e.Value Is DBNull.Value) Then
                If (bsResultControlLotGridView.Columns(e.ColumnIndex).Name = "Mean" OrElse bsResultControlLotGridView.Columns(e.ColumnIndex).Name = "CalcMean") Then
                    e.Value = DirectCast(e.Value, Double).ToString("F" & LocalDecimalAllowedAttribute.ToString())

                ElseIf (bsResultControlLotGridView.Columns(e.ColumnIndex).Name = "SD" OrElse bsResultControlLotGridView.Columns(e.ColumnIndex).Name = "CalcSD") Then
                    e.Value = DirectCast(e.Value, Double).ToString("F" & (LocalDecimalAllowedAttribute + 1).ToString())

                ElseIf (bsResultControlLotGridView.Columns(e.ColumnIndex).Name = "CV" OrElse bsResultControlLotGridView.Columns(e.ColumnIndex).Name = "CalcCV") Then
                    e.Value = DirectCast(e.Value, Double).ToString("F2")

                ElseIf (bsResultControlLotGridView.Columns(e.ColumnIndex).Name = "Dummy") Then
                    e.CellStyle.BackColor = SystemColors.MenuBar
                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".ResultControlLotGridView_CellFormatting ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ResultControlLotGridView_CellFormatting ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Manages checking/unchecking of Control/Lots to plot
    ''' </summary>
    ''' <remarks>
    ''' Created by:  TR 
    ''' Modified by: SA 02/05/2012 - Changed validation to shown the warning message of Statistical Lack: shown it when column Mean is null,
    '''                              not when CalcMean is null; added the Else cases: accept selection and load the graphic in all possible cases
    '''                              Deleted the Event CellValueChanged due to the load of the graph is now executed in this event  
    ''' </remarks>
    Private Sub ResultControlLotGridView_CurrentCellDirtyStateChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsResultControlLotGridView.CurrentCellDirtyStateChanged
        Try
            If ((TypeOf bsResultControlLotGridView.CurrentCell Is DataGridViewCheckBoxCell)) Then
                bsResultControlLotGridView.CommitEdit(DataGridViewDataErrorContexts.Commit)

                'OwningColumn
                If (bsResultControlLotGridView.CurrentCell.OwningColumn.Index = 0) Then
                    If (Not ValidateActiveControls()) Then
                        'It is not possible to select a new Control/Lot, the maximum allowed is alreadt selected
                        'CheckBox value is set to False
                        bsResultControlLotGridView.CurrentCell.Value = False
                        bsResultControlLotGridView.CommitEdit(DataGridViewDataErrorContexts.Commit)
                    Else
                        If (bsResultControlLotGridView.CurrentCell.Value) Then
                            If (IsDBNull(bsResultControlLotGridView.CurrentRow.Cells("Mean").Value)) Then
                                'The CheckBox was checked by User and the action is allowed, but there is not enough data to plot. 
                                'A warning message is shown and the CheckBox value is set to False
                                ShowMessage("Warning", GlobalEnumerates.Messages.STATISTICAL_LACK.ToString())

                                bsResultControlLotGridView.CurrentCell.Value = False
                                bsResultControlLotGridView.CommitEdit(DataGridViewDataErrorContexts.Commit)
                            Else
                                'The CheckBox was checked by User and the action is possible: commit the action and load the graph
                                'plotting also values for the new selected Control/Lot
                                bsResultControlLotGridView.CommitEdit(DataGridViewDataErrorContexts.Commit)
                                LoadGraphic()
                            End If
                        Else
                            'The CheckBox was unchecked by User: commit the action and load the graph for the Control/Lots that
                            'remain selected (if any)
                            bsResultControlLotGridView.CommitEdit(DataGridViewDataErrorContexts.Commit)
                            LoadGraphic()
                        End If
                    End If
                End If
                bsResultControlLotGridView.RefreshEdit()
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".ResultControlLotGridView_CurrentCellDirtyStateChanged ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ResultControlLotGridView_CurrentCellDirtyStateChanged ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    '**************************************
    '** EVENTS FOR OTHER SCREEN CONTROLS **
    '**************************************
    Private Sub GraphicType_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsLeveyJenningsRB.CheckedChanged, bsYoudenRB.CheckedChanged
        Try
            If (isLoading) Then Return
            If (DirectCast(sender, RadioButton).Checked) Then
                LoadGraphic()
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".GraphicType_CheckedChanged ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".GraphicType_CheckedChanged ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub QCResultChartControl_MouseMove(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles bsQCResultChartControl.MouseMove
        Try
            LocalPoint = bsQCResultChartControl.PointToScreen(New Point(e.X, e.Y))
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".QCResultChartControl_MouseMove ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".QCResultChartControl_MouseMove ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    '*******************
    '** BUTTON EVENTS **
    '*******************
    Private Sub PrintButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsPrintButton.Click
        Try
            Cursor = Cursors.WaitCursor
            If (bsLeveyJenningsRB.Checked) Then
                'LJ Graph
                XRManager.ShowQCIndividualResultsByTestReport(mTestSampleId, mDateFrom, mDateTo, _
                                                              OpenQCResultsDSAttribute, QCResultsByControlDSAttribute, _
                                                              LocalDecimalAllowedAttribute, REPORT_QC_GRAPH_TYPE.LEVEY_JENNINGS_GRAPH)
            Else
                'Youden Graph
                XRManager.ShowQCIndividualResultsByTestReport(mTestSampleId, mDateFrom, mDateTo, _
                                                              OpenQCResultsDSAttribute, QCResultsByControlDSAttribute, _
                                                              LocalDecimalAllowedAttribute, REPORT_QC_GRAPH_TYPE.YOUDEN_GRAPH)
            End If
            Cursor = Cursors.Default
        Catch ex As Exception
            Cursor = Cursors.Default
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".PrintButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".PrintButton_Click ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub ExitButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsExitButton.Click
        Me.Close()
    End Sub
#End Region
End Class
