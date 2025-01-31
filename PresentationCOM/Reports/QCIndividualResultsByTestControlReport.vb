Option Strict On
Option Explicit On

Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Global.GlobalEnumerates
Imports DevExpress.XtraCharts
Imports System.Drawing
Imports DevExpress.Utils

Public Class QCIndividualResultsByTestControlReport
    Private mControlsRow As OpenQCResultsDS.tOpenResultsRow
    Private mLocalDecimalAllow As Integer
    Private mIncludeGraph As Boolean
    Private mLabelMEAN As String = ""
    Private mLabelSD As String = ""
    Private mRejectionCriteria As Single

    Public Sub SetControlsAndResultsDatasource(ByVal pControlsDS As OpenQCResultsDS, ByVal pResultsDS As QCResultsDS, ByVal pLocalDecimalAllow As Integer, _
                                               ByVal pGraphType As REPORT_QC_GRAPH_TYPE, ByVal pRejectionCriteria As Single)

        mControlsRow = (From ctrl As OpenQCResultsDS.tOpenResultsRow In pControlsDS.tOpenResults _
                       Where ctrl.QCControlLotID = CInt(ControlLotID.Value) _
                     AndAlso ctrl.Selected).FirstOrDefault

        mLocalDecimalAllow = pLocalDecimalAllow
        mIncludeGraph = (pGraphType = REPORT_QC_GRAPH_TYPE.LEVEY_JENNINGS_GRAPH) AndAlso (Not mControlsRow.IsMeanNull)
        mRejectionCriteria = pRejectionCriteria

        Me.DataSource = pResultsDS
    End Sub

    Private Sub QCIndividualResultsByTestControlReport_BeforePrint(ByVal sender As Object, ByVal e As System.Drawing.Printing.PrintEventArgs) Handles Me.BeforePrint
        If (Me.DesignMode) Then Exit Sub

        'Get all Multilanguage texts for the report
        'Dim currentLanguageGlobal As New GlobalBase
        Dim mCurrentLanguage As String = GlobalBase.GetSessionInfo().ApplicationLanguage
        Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

        XrLabelControlName.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Control_Name", mCurrentLanguage)
        XrLabelLotNumber.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_LotNumber", mCurrentLanguage)
        XrLabelMean.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Mean", mCurrentLanguage)
        XrLabelUnit.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Unit", mCurrentLanguage)
        XrLabelSD.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SD", mCurrentLanguage)
        XrLabelCV.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_CV", mCurrentLanguage)
        XrLabelRanges.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Ranges", mCurrentLanguage)
        XrLabelCalcRunNumber.Text = "n"
        XrLabelResultDateTime.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Date", mCurrentLanguage)
        XrLabelVisibleResultValue.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Result", mCurrentLanguage)
        XrLabelMeasureUnit.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Unit", mCurrentLanguage)
        XrLabelABSError.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_ABS_Error", mCurrentLanguage)
        XrLabelRELErrorPercent.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Rel_Error", mCurrentLanguage)
        XrLabelAlarmsList.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Alarms", mCurrentLanguage) 'JB 01/10/2012 - Resource string unification

        'Show/Hide the graph
        GroupFooter1.Visible = mIncludeGraph

        'Generic Control Data
        XrControlName.Text = mControlsRow.ControlName
        XrLotNumber.Text = mControlsRow.LotNumber
        XrUnit.Text = mControlsRow.MeasureUnit

        'BA-1608 - Verify if field Mean is Null before format it to avoid errors when Statistic Mode is used
        XrMean.Text = String.Empty
        If (Not mControlsRow.IsMeanNull) Then XrMean.Text = mControlsRow.Mean.ToString("F" & mLocalDecimalAllow.ToString())

        XrSD.Text = String.Empty
        If (Not mControlsRow.IsSDNull) Then XrSD.Text = mControlsRow.SD.ToString("F" & (mLocalDecimalAllow + 1).ToString())

        XrCV.Text = String.Empty
        If (Not mControlsRow.IsCVNull) Then XrCV.Text = mControlsRow.CV.ToString("F2")

        XrRanges.Text = mControlsRow.Ranges
        XrGraphHeaderLabel.Text = mControlsRow.ControlName & " (" & mControlsRow.LotNumber & ")"

        If (mIncludeGraph) Then PrepareLJGraph()
    End Sub

    Private Sub XrTableReportRow_BeforePrint(ByVal sender As System.Object, ByVal e As System.Drawing.Printing.PrintEventArgs) Handles XrTableReportRow.BeforePrint
        If (Me.DesignMode) Then Exit Sub

        XrCellResultDateTime.Text = DirectCast(GetCurrentColumnValue("ResultDateTime"), DateTime).ToString(SystemInfoManager.OSDateFormat).ToString() & " " & _
                                    DirectCast(GetCurrentColumnValue("ResultDateTime"), DateTime).ToString(SystemInfoManager.OSLongTimeFormat).ToString()

        If (DirectCast(GetCurrentColumnValue("IncludedInMean"), Boolean)) Then
            XrCellIncludedInMean.Text = "Xm"
        Else
            XrCellIncludedInMean.Text = String.Empty
        End If

        'BA-1608 - The Result Value has to be shown with the number of decimals defined for the Test/Sample Type
        XrCellVisibleResultValue.Text = CDbl(GetCurrentColumnValue("VisibleResultValue")).ToString("F" & mLocalDecimalAllow.ToString())

        XrCellABSError.Text = CDbl(GetCurrentColumnValue("ABSError")).ToString("F" & mLocalDecimalAllow.ToString())
        XrCellRELErrorPercent.Text = CDbl(GetCurrentColumnValue("RELErrorPercent")).ToString("F2")
    End Sub

#Region "LJ Graph"
    ''' <summary>
    ''' Create a DataTable that will be used as DataSource of the Levey-Jennings graph 
    ''' </summary>
    ''' <param name="pXAxisValues">List of integer values to load in the column for Axis-X values ("Argument")</param>
    ''' <returns>DataTable containing a column for Axis-X values ("Argument") and a column for each one of the possible Series to add to the graphic</returns>
    ''' <remarks></remarks>
    Private Function CreateChartData(ByVal pXAxisValues As List(Of Integer)) As DataTable
        Dim myDataSourceTable As New DataTable("LeveyJenningsTable")

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
        Return myDataSourceTable
    End Function

    ''' <summary>
    ''' Draw a constant line in Y-Axis of a Levey-Jennings graph (for Mean and SD bars)
    ''' </summary>
    ''' <param name="pName">Title for the Constant line</param>
    ''' <param name="pDiagram">Diagram in which the Constant line will be added</param>
    ''' <param name="pValue">Value in Y-Axis in which the Constant line will be drawn</param>
    ''' <param name="pColor">Color for the Constant line</param>
    ''' <param name="pDashStyle">Dash Style for the Constant line</param>
    ''' <remarks></remarks>
    Private Sub CreateConstantLine(ByVal pName As String, ByVal pDiagram As XYDiagram, ByVal pValue As Double, ByVal pColor As Color, _
                                   ByVal pDashStyle As DashStyle)
        'Create the constant line
        Dim constantLine As New ConstantLine(pName)
        pDiagram.AxisY.ConstantLines.Add(constantLine)

        'Define its axis value
        constantLine.AxisValue = pValue

        'Customize the behavior of the constant line
        constantLine.Visible = True
        constantLine.ShowInLegend = False
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
    End Sub

    ''' <summary>
    ''' Drawn the Levey-Jenning graph for each one of the Control/Lots that have to be plotted
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 20/06/2014 - BT #1668 (rewritten based in the function used in screen IQCGraphs) 
    ''' Modified by: SA 25/09/2014 - BA-1608 ==> Added some changes required after activation of Option Strict On (convert to Integer runNumber("Argument")
    '''                                          when use it in a Linq 
    '''              SA 14/11/2014 - BA-1885 ==> When there are more than 50 QC Results to plot, the X-Axis should not show each RerunNumber (because 
    '''                                          so many numbers cannot be readable); property ArgumentScaleType has to be set to Numeric to avoid
    '''                                          overlapping of values in X-Axis
    ''' </remarks>
    Private Sub PrepareLJGraph()

        'Get all multilanguage labels
        'Dim currentLanguageGlobal As New GlobalBase
        Dim mCurrentLanguage As String = GlobalBase.GetSessionInfo().ApplicationLanguage
        Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

        mLabelSD = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SD", mCurrentLanguage)
        mLabelMEAN = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Mean", mCurrentLanguage)
        XrLabelWarning.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_WARNING", mCurrentLanguage)
        XrLabelError.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_ERROR", mCurrentLanguage)

        'Initialize the Graphic control
        XrLJGraph.Series.Clear()
        XrLJGraph.Legend.Visibility = DefaultBoolean.True
        XrLJGraph.SeriesTemplate.ValueScaleType = ScaleType.Numerical

        'Get RunNumbers to be used as Axis-X values for the Control to graph
        Dim myXRange As List(Of Integer) = (From a As QCResultsDS.tqcResultsRow In DirectCast(Me.DataSource, QCResultsDS).tqcResults _
                                           Where a.QCControlLotID = mControlsRow.QCControlLotID _
                                     AndAlso Not a.Excluded _
                                        Order By a.CalcRunNumber _
                                          Select a.CalcRunNumber).ToList

        'Create the table that will be used as DataSource for the Series that will be used for this Control
        Dim myDataSourceTable As DataTable = CreateChartData(myXRange)

        'Create the Graph Serie for the Control
        XrLJGraph.Series.Add(mControlsRow.ControlNameLotNum, ViewType.Line)
        XrLJGraph.Series(mControlsRow.ControlNameLotNum).ShowInLegend = False
        XrLJGraph.Series(mControlsRow.ControlNameLotNum).LabelsVisibility = DefaultBoolean.False
        XrLJGraph.Series(mControlsRow.ControlNameLotNum).Label.TextPattern = "{A}: {V}"

        'Set the Diagram properties
        Dim myDiagram As New XYDiagram
        myDiagram = CType(XrLJGraph.Diagram, XYDiagram)
        myDiagram.AxisY.ConstantLines.Clear()
        myDiagram.AxisX.ConstantLines.Clear()
        myDiagram.AxisX.WholeRange.Auto = True
        myDiagram.AxisX.VisualRange.Auto = True
        myDiagram.AxisY.GridLines.Visible = False
        myDiagram.AxisX.GridLines.Visible = False
        myDiagram.AxisX.Title.Visibility = DefaultBoolean.False
        myDiagram.AxisY.Title.Visibility = DefaultBoolean.False

        'In this Report, the LJ Graph is drawn individually for each Control
        Dim myUsedMean As Double = mControlsRow.Mean
        CreateConstantLine(mLabelMEAN, myDiagram, myUsedMean, Color.Black, DashStyle.Solid)

        'Create the Constant line for the Rejection Criteria
        If (mControlsRow.SD > 0) Then
            If (mRejectionCriteria = 1) Then
                CreateConstantLine("+1 " & mLabelSD, myDiagram, myUsedMean + (1 * mControlsRow.SD), Color.Black, DashStyle.Solid)
                CreateConstantLine("-1 " & mLabelSD, myDiagram, myUsedMean - (1 * mControlsRow.SD), Color.Black, DashStyle.Solid)
            Else
                CreateConstantLine("+1 " & mLabelSD, myDiagram, myUsedMean + (1 * mControlsRow.SD), Color.Black, DashStyle.Dash)
                CreateConstantLine("-1 " & mLabelSD, myDiagram, myUsedMean - (1 * mControlsRow.SD), Color.Black, DashStyle.Dash)
            End If

            If (mRejectionCriteria = 2) Then
                CreateConstantLine("+2 " & mLabelSD, myDiagram, myUsedMean + (2 * mControlsRow.SD), Color.Black, DashStyle.Solid)
                CreateConstantLine("-2 " & mLabelSD, myDiagram, myUsedMean - (2 * mControlsRow.SD), Color.Black, DashStyle.Solid)
            Else
                CreateConstantLine("+2 " & mLabelSD, myDiagram, myUsedMean + (2 * mControlsRow.SD), Color.Black, DashStyle.Dash)
                CreateConstantLine("-2 " & mLabelSD, myDiagram, myUsedMean - (2 * mControlsRow.SD), Color.Black, DashStyle.Dash)
            End If

            If (mRejectionCriteria = 3) Then
                CreateConstantLine("+3 " & mLabelSD, myDiagram, myUsedMean + (3 * mControlsRow.SD), Color.Black, DashStyle.Solid)
                CreateConstantLine("-3 " & mLabelSD, myDiagram, myUsedMean - (3 * mControlsRow.SD), Color.Black, DashStyle.Solid)
            Else
                CreateConstantLine("+3 " & mLabelSD, myDiagram, myUsedMean + (3 * mControlsRow.SD), Color.Black, DashStyle.Dash)
                CreateConstantLine("-3 " & mLabelSD, myDiagram, myUsedMean - (3 * mControlsRow.SD), Color.Black, DashStyle.Dash)
            End If

            If (CBool(mRejectionCriteria Mod 1) OrElse mRejectionCriteria >= 4) Then
                CreateConstantLine("+4 " & mLabelSD, myDiagram, myUsedMean + (mRejectionCriteria * mControlsRow.SD), Color.Black, DashStyle.Solid)
                CreateConstantLine("-4 " & mLabelSD, myDiagram, myUsedMean - (mRejectionCriteria * mControlsRow.SD), Color.Black, DashStyle.Solid)
            End If
        End If

        'Set the limits for Axis Y
        If (mRejectionCriteria < 1) Then
            Dim maxRelError As Double = (From a As QCResultsDS.tqcResultsRow In DirectCast(Me.DataSource, QCResultsDS).tqcResults _
                                        Where a.QCControlLotID = mControlsRow.QCControlLotID _
                                  AndAlso Not a.Excluded _
                                       Select a.RELError).Max

            If (Math.Round(myUsedMean + (maxRelError * mControlsRow.SD), 3) < Math.Round(myUsedMean + (mRejectionCriteria * mControlsRow.SD))) Then
                maxRelError = mRejectionCriteria
            End If

            myDiagram.AxisY.WholeRange.SetMinMaxValues(Math.Round(myUsedMean - (maxRelError * mControlsRow.SD), 3) - 10, _
                                                  Math.Round(myUsedMean + (maxRelError * mControlsRow.SD), 3) + 10)
            myDiagram.AxisY.VisualRange.SetMinMaxValues(Math.Round(myUsedMean - (maxRelError * mControlsRow.SD), 3) - 10, _
                                                  Math.Round(myUsedMean + (maxRelError * mControlsRow.SD), 3) + 10)
        Else
            If (mControlsRow.SD > 0) Then
                myDiagram.AxisY.WholeRange.SetMinMaxValues(Math.Round(myUsedMean - (4 * mControlsRow.SD), 3), _
                                                      Math.Round(myUsedMean + (4 * mControlsRow.SD), 3))
                myDiagram.AxisY.VisualRange.SetMinMaxValues(Math.Round(myUsedMean - (4 * mControlsRow.SD), 3), _
                                                      Math.Round(myUsedMean + (4 * mControlsRow.SD), 3))
            End If
        End If

        myDiagram.AxisY.VisualRange.SideMarginsValue = 0

        Dim minQCSeries = 50
        Dim validResultValues As List(Of QCResultsDS.tqcResultsRow)
        For Each runNumber As DataRow In myDataSourceTable.Rows
            'Search value of the Control/Lot for the Run Number 
            validResultValues = (From a As QCResultsDS.tqcResultsRow In DirectCast(Me.DataSource, QCResultsDS).tqcResults _
                                Where a.QCControlLotID = mControlsRow.QCControlLotID _
                              AndAlso a.CalcRunNumber = CInt(runNumber("Argument")) _
                             Order By a.CalcRunNumber _
                               Select a).ToList

            If (validResultValues.Count = 1) Then
                runNumber("Values") = validResultValues.First.VisibleResultValue
            End If
        Next
        myDataSourceTable.AcceptChanges()

        XrLJGraph.Series(mControlsRow.ControlNameLotNum).DataSource = myDataSourceTable
        XrLJGraph.Series(mControlsRow.ControlNameLotNum).ArgumentDataMember = "Argument"

        'ADDED THIS FOR CORRECT SCALING
        XrLJGraph.Series(mControlsRow.ControlNameLotNum).ArgumentScaleType = ScaleType.Qualitative

        'BA-1885 - When the number of results to plot is greater than 50, this property has to be used to avoid overlapping of values in X-Axis
        If (myDataSourceTable.Rows.Count > minQCSeries) Then XrLJGraph.Series(mControlsRow.ControlNameLotNum).ArgumentScaleType = ScaleType.Numerical

        XrLJGraph.Series(mControlsRow.ControlNameLotNum).ValueScaleType = ScaleType.Numerical
        XrLJGraph.Series(mControlsRow.ControlNameLotNum).ValueDataMembers.AddRange(New String() {"Values"})

        For i As Integer = 0 To XrLJGraph.Series(mControlsRow.ControlNameLotNum).Points.Count - 1
            XrLJGraph.Series(mControlsRow.ControlNameLotNum).Points(i).Tag = XrLJGraph.Series(mControlsRow.ControlNameLotNum).Points(i).Values(0).ToString
        Next

        'Set margins
        myDiagram.Margins.Right = 5

        'Set the Title for each axis
        myDiagram.AxisX.Title.Visibility = DefaultBoolean.True
        myDiagram.AxisX.Title.Antialiasing = False
        myDiagram.AxisX.Title.TextColor = Color.Black
        myDiagram.AxisX.Title.Alignment = StringAlignment.Center
        myDiagram.AxisX.Title.Font = New Font("Verdana", 8.25, FontStyle.Regular)
        myDiagram.AxisX.Title.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Serie", mCurrentLanguage)

        myDiagram.AxisY.Title.Visibility = DefaultBoolean.True
        myDiagram.AxisY.Title.Antialiasing = False
        myDiagram.AxisY.Title.TextColor = Color.Black
        myDiagram.AxisY.Title.Alignment = StringAlignment.Center
        myDiagram.AxisY.Title.Font = New Font("Verdana", 8.25, FontStyle.Regular)
        myDiagram.AxisY.Title.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Concentration_Long", mCurrentLanguage)

        'Validate if there are not series on the graph control to remove constant lines and axis titles
        If (XrLJGraph.Series.Count = 0) Then
            myDiagram = CType(XrLJGraph.Diagram, XYDiagram)

            If (Not myDiagram Is Nothing) Then
                'Remove all constant lines
                myDiagram.AxisY.ConstantLines.Clear()
                myDiagram.AxisX.ConstantLines.Clear()

                'Remove all axis titles
                myDiagram.AxisX.Title.Visibility = DefaultBoolean.False
                myDiagram.AxisY.Title.Visibility = DefaultBoolean.False
            End If
        End If
    End Sub

    Private Sub XrLJGraph_CustomDrawSeriesPoint(ByVal sender As System.Object, ByVal e As DevExpress.XtraCharts.CustomDrawSeriesPointEventArgs) Handles XrLJGraph.CustomDrawSeriesPoint
        Dim myArgumentValue As String = e.SeriesPoint.Argument.ToString
        CType(e.SeriesDrawOptions, LineDrawOptions).Marker.FillStyle.FillMode = FillMode.Solid

        'Filter by CalcRunNumber and ControlName to get the point values
        Dim resultsDT As QCResultsDS.tqcResultsDataTable = DirectCast(Me.DataSource, QCResultsDS).tqcResults
        Dim rowErr As List(Of QCResultsDS.tqcResultsRow) = (From a In resultsDT _
                                                           Where a.CalcRunNumber = CType(myArgumentValue, Integer) _
                                                         AndAlso a.ControlNameLotNum = mControlsRow.ControlNameLotNum _
                                                         AndAlso a.QCControlLotID = mControlsRow.QCControlLotID _
                                                          Select a).ToList()

        If (rowErr.Count > 0) Then
            With CType(e.SeriesDrawOptions, LineDrawOptions).Marker
                If (Not rowErr(0).IsValidationStatusNull AndAlso rowErr(0).ValidationStatus = "ERROR") Then
                    .Kind = MarkerKind.Diamond
                    .Color = Color.Black
                    .Size = 14
                ElseIf (Not rowErr(0).IsValidationStatusNull AndAlso rowErr(0).ValidationStatus = "WARNING") Then
                    .Kind = MarkerKind.Square
                    .Color = Color.DimGray
                    .Size = 14
                Else
                    .Kind = MarkerKind.Cross
                    .Color = Color.Black
                    .Size = 7
                End If
            End With
        End If
    End Sub
#End Region
End Class