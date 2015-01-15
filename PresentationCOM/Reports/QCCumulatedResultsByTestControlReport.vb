Option Explicit On

Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Types
Imports DevExpress.XtraCharts
Imports System.Drawing

Public Class QCCumulatedResultsByTestControlReport
    Private mControlsRow As QCCumulatedSummaryDS.QCCumulatedSummaryTableRow
    Private mLocalDecimalAllow As Integer
    Private mLabelMEAN As String = ""
    Private mLabelSD As String = ""
    Private mLabelCV As String = ""
    Private mLabelUnit As String = ""
    Private mLabelRange As String = ""
    Private mRejectionCriteria As Single

    Public Sub SetControlsAndResultsDatasource(ByVal pControlsDS As QCCumulatedSummaryDS, _
                                               ByVal pResultsDS As CumulatedResultsDS, _
                                               ByVal pLocalDecimalAllow As Integer, _
                                               ByVal pRejectionCriteria As Single)
        mControlsRow = (From ctrl In pControlsDS.QCCumulatedSummaryTable Where ctrl.QCControlLotID = CInt(ControlLotID.Value) AndAlso ctrl.Selected).FirstOrDefault
        mLocalDecimalAllow = pLocalDecimalAllow
        mRejectionCriteria = pRejectionCriteria
        Me.DataSource = pResultsDS
    End Sub

    Private Sub QCIndividualResultsByTestControlReport_BeforePrint(ByVal sender As Object, ByVal e As System.Drawing.Printing.PrintEventArgs) Handles Me.BeforePrint
        If Me.DesignMode Then Exit Sub

        'Multilanguage support
        Dim currentLanguageGlobal As New GlobalBase
        Dim mCurrentLanguage As String = currentLanguageGlobal.GetSessionInfo().ApplicationLanguage
        Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

        mLabelMEAN = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Mean", mCurrentLanguage)
        mLabelSD = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SD", mCurrentLanguage)
        mLabelCV = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_CV", mCurrentLanguage)
        mLabelUnit = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Unit", mCurrentLanguage)
        mLabelRange = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Ranges", mCurrentLanguage)

        XrLabelControlName.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Control_Name", mCurrentLanguage)
        XrLabelLotNumber.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_LotNumber", mCurrentLanguage)
        XrLabelN.Text = "N"
        XrLabelMean.Text = mLabelMEAN
        XrLabelUnit.Text = mLabelUnit
        XrLabelSD.Text = mLabelSD
        XrLabelCV.Text = mLabelCV
        XrLabelRanges.Text = mLabelRange

        XrLabelCellCumDateTime.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Date", mCurrentLanguage)
        XrLabelCellTotalRuns.Text = "n"
        XrLabelCellMean.Text = mLabelMEAN
        XrLabelCellMeasureUnit.Text = mLabelUnit
        XrLabelCellSD.Text = mLabelSD
        XrLabelCellCV.Text = mLabelCV
        XrLabelCellRange.Text = mLabelRange

        'Generic Control Data
        XrControlName.Text = mControlsRow.ControlName
        XrLotNumber.Text = mControlsRow.LotNumber
        XrN.Text = mControlsRow.N.ToString
        If Not mControlsRow.IsMeanNull Then
            XrMean.Text = mControlsRow.Mean.ToString("F" & mLocalDecimalAllow.ToString())
            XrUnit.Text = mControlsRow.MeasureUnit
        End If
        If Not mControlsRow.IsSDNull Then XrSD.Text = mControlsRow.SD.ToString("F" & (mLocalDecimalAllow + 1).ToString())
        If Not mControlsRow.IsCVNull Then XrCV.Text = mControlsRow.CV.ToString("F2")
        XrRanges.Text = mControlsRow.Range

        XrGraphHeaderLabel.Text = mControlsRow.ControlName & " (" & mControlsRow.LotNumber & ")"

        'Hide or prepare the graph
        If (mControlsRow.IsMeanNull OrElse Not mControlsRow.Selected) Then
            GroupFooter1.Visible = False
        Else
            PrepareLJGraph()
        End If
    End Sub

    Private Sub XrTableReportRow_BeforePrint(ByVal sender As System.Object, ByVal e As System.Drawing.Printing.PrintEventArgs) Handles XrTableReportRow.BeforePrint
        If Me.DesignMode Then Exit Sub

        XrCellCumDateTime.Text = DirectCast(GetCurrentColumnValue("CumDateTime"), DateTime).ToString(SystemInfoManager.OSDateFormat).ToString() & " " & _
                                 DirectCast(GetCurrentColumnValue("CumDateTime"), DateTime).ToString(SystemInfoManager.OSLongTimeFormat).ToString()


        XrCellMean.Text = CDbl(GetCurrentColumnValue("Mean")).ToString("F" & mLocalDecimalAllow.ToString())
        XrCellSD.Text = CDbl(GetCurrentColumnValue("SD")).ToString("F" & (mLocalDecimalAllow + 1).ToString())
        XrCellCV.Text = CDbl(GetCurrentColumnValue("CV")).ToString("F2")
    End Sub

#Region "LJ Graph"
    ''' <summary>
    ''' Load a constant line into the graphic
    ''' </summary>
    ''' <remarks>
    ''' Created by:  TR 28/06/2011
    ''' </remarks>
    Private Sub CreateConstantLine(ByVal pName As String, ByVal pDiagram As XYDiagram, ByVal pValue As Single, ByVal pColor As Color, _
                                   ByVal pDashStyle As DashStyle)
        Try
            'Create a constant line
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

            'Customize the appearance of the constant line.
            constantLine.Color = pColor
            constantLine.LineStyle.DashStyle = pDashStyle
            constantLine.LineStyle.Thickness = 1
        Catch ex As Exception
        End Try
    End Sub

    Private Sub PrepareLJGraph()
        Dim resultsDT As CumulatedResultsDS.tqcCumulatedResultsDataTable = DirectCast(Me.DataSource, CumulatedResultsDS).tqcCumulatedResults

        'Clear Graphics controls
        XrLJGraph.Series.Clear()
        XrLJGraph.Legend.Visible = False
        XrLJGraph.BackColor = Color.White
        XrLJGraph.AppearanceName = "Light"

        Dim myDiagram As New XYDiagram
        Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate


        'Set up the serie
        XrLJGraph.Series.Add(mControlsRow.QCControlLotID.ToString(), ViewType.Line)
        With XrLJGraph.Series(mControlsRow.QCControlLotID.ToString())
            .ShowInLegend = False
            .Label.Visible = False
            .PointOptions.PointView = PointView.ArgumentAndValues

            .View.Color = Color.Black
            .Tag = mControlsRow.ControlName & Environment.NewLine & mControlsRow.LotNumber
            .LegendText = mControlsRow.LotNumber

            'Create points
            For Each cumResultRow As CumulatedResultsDS.tqcCumulatedResultsRow In From r In resultsDT _
                                                                                  Where r.QCControlLotID = mControlsRow.QCControlLotID AndAlso _
                                                                                        Not r.IsMeanNull
                .Points.Add(New SeriesPoint(cumResultRow.VisibleCumResultNumber, cumResultRow.Mean))
            Next
        End With


        If (Not XrLJGraph.Diagram Is Nothing) Then
            myDiagram = CType(XrLJGraph.Diagram, XYDiagram)
            myDiagram.AxisY.GridLines.Visible = False
            myDiagram.AxisX.GridLines.Visible = True

            'Remove all Constant lines
            myDiagram.AxisY.ConstantLines.Clear()
            myDiagram.AxisX.ConstantLines.Clear()
            myDiagram.AxisX.Title.Visible = False
            myDiagram.AxisY.Title.Visible = False

            'Create constans lines
            Dim myMean As Single = 0
            Dim mySD As Single = 0

            If Not mControlsRow.IsMeanNull Then myMean = mControlsRow.Mean
            If Not mControlsRow.IsSDNull Then mySD = mControlsRow.SD

            'Draw contants lines 1,0,-1
            CreateConstantLine(mLabelMEAN, myDiagram, myMean, Color.Black, DashStyle.Solid)
            CreateConstantLine("+1 " & mLabelSD, myDiagram, myMean + (1 * mySD), Color.Black, DashStyle.Dash)
            CreateConstantLine("-1 " & mLabelSD, myDiagram, myMean - (1 * mySD), Color.Black, DashStyle.Dash)

            'Set limits for Axis Y
            myDiagram.AxisY.Range.SetMinMaxValues(myMean - (1 * mySD) - (myMean - (myMean - mySD)) / 2, _
                                                  myMean + (1 * mySD) + (myMean - (myMean - mySD)) / 2)
        End If
    End Sub

    Private Sub XrLJGraph_CustomDrawSeriesPoint(ByVal sender As System.Object, ByVal e As DevExpress.XtraCharts.CustomDrawSeriesPointEventArgs) Handles XrLJGraph.CustomDrawSeriesPoint
        With CType(e.SeriesDrawOptions, PointDrawOptions).Marker
            .FillStyle.FillMode = FillMode.Solid
            .Kind = MarkerKind.Cross
        End With
    End Sub
#End Region

End Class