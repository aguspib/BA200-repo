Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Global.GlobalEnumerates
Imports DevExpress.XtraReports.UI
Imports DevExpress.XtraCharts
Imports System.Drawing

Public Class QCCumulatedResultsByTestReport
    Private mTestSampleData As HistoryTestSamplesDS.tqcHistoryTestSamplesRow
    Private mDateRangeText As String
    Private mControlsDS As QCCumulatedSummaryDS
    Private mLabelMEAN As String = ""
    Private mLabelSD As String = ""
    Private mResultsDS As CumulatedResultsDS

    Public Sub SetControlsAndResultsDatasource(ByVal pTestSampleRow As HistoryTestSamplesDS.tqcHistoryTestSamplesRow, _
                                               ByVal pQCCumulatedSummaryDS As QCCumulatedSummaryDS, _
                                               ByVal pQCCummulatedResultsDS As CumulatedResultsDS, _
                                               ByVal pLocalDecimalAllow As Integer, _
                                               ByVal pDateRangeText As String)
        mTestSampleData = pTestSampleRow
        mDateRangeText = pDateRangeText
        mControlsDS = pQCCumulatedSummaryDS
        mResultsDS = pQCCummulatedResultsDS

        'Adding the SubReports
        For Each elem As QCCumulatedSummaryDS.QCCumulatedSummaryTableRow In From c In mControlsDS.QCCumulatedSummaryTable.Rows Where c.Selected
            Dim mQCRep As New QCCumulatedResultsByTestControlReport
            mQCRep.ControlLotID.Value = elem.QCControlLotID
            mQCRep.SetControlsAndResultsDatasource(mControlsDS, mResultsDS, pLocalDecimalAllow, pTestSampleRow.RejectionCriteria)

            Dim mSubReport As New XRSubreport()
            mSubReport.Name = "SubReport" & elem.QCControlLotID.ToString
            mSubReport.ReportSource = mQCRep
            Me.Detail1.Controls.Add(mSubReport)
            mSubReport.TopF = Me.Detail1.HeightF
            mSubReport.LeftF = 0
            Me.Detail1.HeightF += mSubReport.HeightF
        Next
    End Sub

    Private Sub QCCummulatedResultsByTestReport_BeforePrint(ByVal sender As Object, ByVal e As System.Drawing.Printing.PrintEventArgs) Handles Me.BeforePrint
        If Me.DesignMode Then Exit Sub

        'Multilanguage support
        Dim currentLanguageGlobal As New GlobalBase
        Dim CurrentLanguage As String = currentLanguageGlobal.GetSessionInfo().ApplicationLanguage
        Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

        'Multilanguage. Get texts from DB.
        XrHeaderLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "TITLE_CumulatedResults", CurrentLanguage)
        XrLabelTestName.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_TestName", CurrentLanguage)
        XrLabelSample.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SampleType", CurrentLanguage)
        XrLabelDateRange.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_DateRange", CurrentLanguage)
        XrLabelControls.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "TITLE_Controls_List", CurrentLanguage)

        'The TestSample data
        XrTestName.Text = mTestSampleData.TestName
        XrSample.Text = mTestSampleData.SampleType
        XrDateRange.Text = mDateRangeText
    End Sub

End Class