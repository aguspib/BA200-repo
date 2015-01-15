Option Strict On
Option Explicit On

Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global

Imports DevExpress.XtraReports.UI

Public Class QCCumulatedResultsByTestReport
    Private mTestSampleData As HistoryTestSamplesDS.tqcHistoryTestSamplesRow
    Private mDateRangeText As String
    Private mControlsDS As QCCumulatedSummaryDS
    Private mLabelMEAN As String = ""
    Private mLabelSD As String = ""
    Private mResultsDS As CumulatedResultsDS

    ''' <summary></summary>
    ''' <remarks>
    ''' Created by: 
    ''' Modified by: SA 25/09/2014 - BA-1608 ==> ** Added some changes required after activation of Option Strict On
    '''                                          ** Changed from Sub to Function that returns a Boolean value: TRUE if the Report can be generated, 
    '''                                             and FALSE if the Report cannot be generated (if none of the linked Controls are Selected)
    ''' </remarks>
    Public Function SetControlsAndResultsDatasource(ByVal pTestSampleRow As HistoryTestSamplesDS.tqcHistoryTestSamplesRow, ByVal pQCCumulatedSummaryDS As QCCumulatedSummaryDS, _
                                                    ByVal pQCCummulatedResultsDS As CumulatedResultsDS, ByVal pLocalDecimalAllow As Integer, ByVal pDateRangeText As String) As Boolean
        Dim generateReport As Boolean = False

        'Get all selected Controls
        Dim lstSelectedControls As List(Of QCCumulatedSummaryDS.QCCumulatedSummaryTableRow) = (From c As QCCumulatedSummaryDS.QCCumulatedSummaryTableRow In pQCCumulatedSummaryDS.QCCumulatedSummaryTable _
                                                                                          Where Not c.IsSelectedNull AndAlso c.Selected = True _
                                                                                             Select c).ToList()
        If (lstSelectedControls.Count > 0) Then
            generateReport = True

            mTestSampleData = pTestSampleRow
            mDateRangeText = pDateRangeText
            mControlsDS = pQCCumulatedSummaryDS
            mResultsDS = pQCCummulatedResultsDS

            'Add the SubReports
            For Each elem As QCCumulatedSummaryDS.QCCumulatedSummaryTableRow In lstSelectedControls
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
        End If
        lstSelectedControls = Nothing

        Return generateReport
    End Function

    ''' <summary></summary>
    ''' <remarks>
    ''' Created by: 
    ''' Modified by: SA 23/09/2014 - BA-1608 ==> If ReportName is informed (field TestLongName is not Null nor empty), use it as Test Name in the report
    ''' </remarks>
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

        'BA-1608 - If ReportName is informed (field TestLongName is not Null nor empty), use it as Test Name in the report
        Dim myTestName As String = mTestSampleData.TestName
        If (Not mTestSampleData.IsTestLongNameNull AndAlso mTestSampleData.TestLongName <> String.Empty) Then myTestName = mTestSampleData.TestLongName
        XrTestName.Text = myTestName

        'Rest of the TestSample data
        XrSample.Text = mTestSampleData.SampleType
        XrDateRange.Text = mDateRangeText
    End Sub
End Class