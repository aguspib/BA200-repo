Option Explicit On

Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Global.GlobalEnumerates
Imports Biosystems.Ax00.Types
Imports DevExpress.XtraCharts
Imports System.Drawing

Public Class QCIndividualResultsByTestControlReport
    Private mControlsRow As OpenQCResultsDS.tOpenResultsRow
    Private mLocalDecimalAllow As Integer
    Private mIncludeGraph As Boolean
    Private mLabelMEAN As String = ""
    Private mLabelSD As String = ""
    Private mRejectionCriteria As Single


    Public Sub SetControlsAndResultsDatasource(ByVal pControlsDS As OpenQCResultsDS, _
                                               ByVal pResultsDS As QCResultsDS, ByVal pLocalDecimalAllow As Integer, _
                                               ByVal pGraphType As REPORT_QC_GRAPH_TYPE, ByVal pRejectionCriteria As Single)
        mControlsRow = (From ctrl In pControlsDS.tOpenResults Where ctrl.QCControlLotID = CInt(ControlLotID.Value) AndAlso ctrl.Selected).FirstOrDefault
        mLocalDecimalAllow = pLocalDecimalAllow
        mIncludeGraph = (pGraphType = REPORT_QC_GRAPH_TYPE.LEVEY_JENNINGS_GRAPH)
        mRejectionCriteria = pRejectionCriteria
        Me.DataSource = pResultsDS
    End Sub

    Private Sub QCIndividualResultsByTestControlReport_BeforePrint(ByVal sender As Object, ByVal e As System.Drawing.Printing.PrintEventArgs) Handles Me.BeforePrint
        If Me.DesignMode Then Exit Sub

        'Multilanguage support
        Dim currentLanguageGlobal As New GlobalBase
        Dim mCurrentLanguage As String = currentLanguageGlobal.GetSessionInfo().ApplicationLanguage
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
        XrMean.Text = mControlsRow.Mean.ToString("F" & mLocalDecimalAllow.ToString())
        XrUnit.Text = mControlsRow.MeasureUnit
        'JC 12/11/2012
        XrSD.Text = ""
        XrCV.Text = ""

        If Not mControlsRow.IsSDNull Then XrSD.Text = mControlsRow.SD.ToString("F" & (mLocalDecimalAllow + 1).ToString())
        If Not mControlsRow.IsCVNull Then XrCV.Text = mControlsRow.CV.ToString("F2")
        XrRanges.Text = mControlsRow.Ranges
        XrGraphHeaderLabel.Text = mControlsRow.ControlName & " (" & mControlsRow.LotNumber & ")"

        If mIncludeGraph Then
            PrepareLJGraph()
        End If

    End Sub

    Private Sub XrTableReportRow_BeforePrint(ByVal sender As System.Object, ByVal e As System.Drawing.Printing.PrintEventArgs) Handles XrTableReportRow.BeforePrint
        If Me.DesignMode Then Exit Sub

        'If String.IsNullOrEmpty(GetCurrentColumnValue("AlarmsList").ToString) Then
        '    XrTableReportRow.Font = New System.Drawing.Font(XrTableReportRow.Font, Drawing.FontStyle.Regular)
        'Else
        '    XrTableReportRow.Font = New System.Drawing.Font(XrTableReportRow.Font, Drawing.FontStyle.Bold)
        'End If

        XrCellResultDateTime.Text = DirectCast(GetCurrentColumnValue("ResultDateTime"), DateTime).ToString(SystemInfoManager.OSDateFormat).ToString() & " " & _
                                    DirectCast(GetCurrentColumnValue("ResultDateTime"), DateTime).ToString(SystemInfoManager.OSLongTimeFormat).ToString()

        If DirectCast(GetCurrentColumnValue("IncludedInMean"), Boolean) Then
            XrCellIncludedInMean.Text = "Xm"
        Else
            XrCellIncludedInMean.Text = ""
        End If

        XrCellABSError.Text = CDbl(GetCurrentColumnValue("ABSError")).ToString("F" & mLocalDecimalAllow.ToString())
        XrCellRELErrorPercent.Text = CDbl(GetCurrentColumnValue("RELErrorPercent")).ToString("F2")
    End Sub

#Region "LJ Graph"
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

    Private Sub PrepareLJGraph()
        'Multilanguage support
        Dim currentLanguageGlobal As New GlobalBase
        Dim mCurrentLanguage As String = currentLanguageGlobal.GetSessionInfo().ApplicationLanguage
        Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

        mLabelSD = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SD", mCurrentLanguage)
        mLabelMEAN = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Mean", mCurrentLanguage)
        XrLabelWarning.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_WARNING", mCurrentLanguage)
        XrLabelError.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_ERROR", mCurrentLanguage) 'JB 01/10/2012 - Resource String unification

        'The Graph
        XrLJGraph.Series.Clear()
        XrLJGraph.Legend.Visible = True
        XrLJGraph.SeriesTemplate.ValueScaleType = ScaleType.Numerical

        Dim myDiagram As New XYDiagram
        Dim myUsedMean As Double

        XrLJGraph.Series.Add(mControlsRow.ControlNameLotNum, ViewType.Line)
        XrLJGraph.Series(mControlsRow.ControlNameLotNum).ShowInLegend = False
        XrLJGraph.Series(mControlsRow.ControlNameLotNum).Label.Visible = False
        XrLJGraph.Series(mControlsRow.ControlNameLotNum).PointOptions.PointView = PointView.ArgumentAndValues

        myDiagram = CType(XrLJGraph.Diagram, XYDiagram)
        myDiagram.AxisY.GridLines.Visible = False
        myDiagram.AxisX.GridLines.Visible = False

        myDiagram.AxisY.ConstantLines.Clear()
        myDiagram.AxisX.ConstantLines.Clear()

        myDiagram.AxisX.Title.Visible = False
        myDiagram.AxisY.Title.Visible = False

        myUsedMean = mControlsRow.Mean
        'Only one Control is selected to be graph
        CreateConstantLine(mLabelMEAN, myDiagram, myUsedMean, Color.Black, DashStyle.Solid)
        If mControlsRow.SD > 0 Then
            'Create the Constant line for the Rejection Criteria

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

        Dim resultsDT As QCResultsDS.tqcResultsDataTable = DirectCast(Me.DataSource, QCResultsDS).tqcResults

        'Set the Controls limits
        If (mRejectionCriteria < 1) Then
            Dim MaxRelError As Double = 0

            'Get maximum REL Error
            MaxRelError = (From res In resultsDT Where res.QCControlLotID = mControlsRow.QCControlLotID AndAlso _
                                                       res.ControlNameLotNum = mControlsRow.ControlNameLotNum Select res.RELError).Max

            'Validate if calculation with max rel error is lower
            If (Math.Round(myUsedMean + (MaxRelError * mControlsRow.SD), 3)) < _
                          (myUsedMean + (mRejectionCriteria * mControlsRow.SD)) Then
                MaxRelError = mRejectionCriteria
            End If

            myDiagram.AxisY.Range.SetMinMaxValues(Math.Round(myUsedMean - (MaxRelError * mControlsRow.SD), 3) - 10 - 1, _
                                                  Math.Round(myUsedMean + (MaxRelError * mControlsRow.SD), 3) + 10 + 1)
        Else
            'TR 21/06/2012 -Validate if sd value is 0 (zero)
            If mControlsRow.SD > 0 Then
                'Set the Min and Max Range for Y
                myDiagram.AxisY.Range.SetMinMaxValues(Math.Round(myUsedMean - (4 * mControlsRow.SD), 3) - 1, _
                                                      Math.Round(myUsedMean + (4 * mControlsRow.SD), 3) + 1)
            End If

        End If

        'Set X Range limits
        Dim XRange As List(Of Integer) = (From a In resultsDT _
                                          Where Not a.Excluded _
                                                AndAlso a.ControlNameLotNum = mControlsRow.ControlNameLotNum _
                                                AndAlso a.QCControlLotID = mControlsRow.QCControlLotID _
                                          Select a.CalcRunNumber).ToList

        myDiagram.AxisX.Range.SetMinMaxValues(XRange.Min - 1, XRange.Max + 1)

        Dim lst = From item In resultsDT.Rows _
                  Where Not item.Excluded AndAlso _
                        item.QCControlLotID = mControlsRow.QCControlLotID AndAlso _
                        item.ControlNameLotNum = mControlsRow.ControlNameLotNum _
                  Order By item.CalcRunNumber Ascending

        For Each elem In lst
            XrLJGraph.Series(mControlsRow.ControlNameLotNum.ToString).Points.Add(New SeriesPoint(elem.CalcRunNumber, elem.VisibleResultValue))
        Next


        'Set margins
        myDiagram.Margins.Right = 5

        'Set the Title for each axis
        myDiagram.AxisX.Title.Visible = True
        myDiagram.AxisX.Title.Antialiasing = False
        myDiagram.AxisX.Title.TextColor = Color.Black
        myDiagram.AxisX.Title.Alignment = StringAlignment.Center
        myDiagram.AxisX.Title.Font = New Font("Verdana", 8.25, FontStyle.Regular)
        myDiagram.AxisX.Title.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Serie", mCurrentLanguage)

        myDiagram.AxisY.Title.Visible = True
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
                myDiagram.AxisX.Title.Visible = False
                myDiagram.AxisY.Title.Visible = False
            End If
        End If

    End Sub

    Private Sub XrLJGraph_CustomDrawSeriesPoint(ByVal sender As System.Object, ByVal e As DevExpress.XtraCharts.CustomDrawSeriesPointEventArgs) Handles XrLJGraph.CustomDrawSeriesPoint
        Dim myArgumentValue As String = e.SeriesPoint.Argument.ToString

        CType(e.SeriesDrawOptions, LineDrawOptions).Marker.FillStyle.FillMode = FillMode.Solid

        Dim resultsDT As QCResultsDS.tqcResultsDataTable = DirectCast(Me.DataSource, QCResultsDS).tqcResults

        'Filter by CalcRunNumber and ControlName to get the point values
        Dim rowErr As List(Of QCResultsDS.tqcResultsRow) = (From a In resultsDT _
                                                            Where a.CalcRunNumber = CType(myArgumentValue, Integer) AndAlso _
                                                                  a.ControlNameLotNum = mControlsRow.ControlNameLotNum AndAlso _
                                                                  a.QCControlLotID = mControlsRow.QCControlLotID _
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