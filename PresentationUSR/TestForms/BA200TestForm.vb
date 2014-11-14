Option Explicit On
Option Strict On

Imports System.Xml
Imports System.Windows.Forms

Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Global.TO
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.CommunicationsSwFw
Imports Biosystems.Ax00.Calculations
Imports Biosystems.Ax00.PresentationCOM
Imports Biosystems.Ax00.Controls.UserControls
Imports LIS.Biosystems.Ax00.LISCommunications
Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.DAL.DAO

Imports DevExpress.XtraReports.UI
Imports DevExpress.XtraPrinting
Imports DevExpress.XtraEditors
Imports Biosystems.Ax00.App
Imports Biosystems.Ax00.Global.GlobalEnumerates

Public Class BA200TestForm
    Inherits Biosystems.Ax00.PresentationCOM.BSBaseForm

    Private Sub btnFillRotor_Click(sender As Object, e As EventArgs) Handles btnFillRotor.Click
        AnalyzerController.Instance.Analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.DynamicBL_Fill) = "INI"
        AnalyzerController.Instance.Analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.DynamicBL_Read) = ""
        AnalyzerController.Instance.Analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.DynamicBL_Empty) = ""
        Dim myParams As New List(Of String)(New String() {CStr(Ax00FlightAction.FillRotor), "0"})
        AnalyzerController.Instance.Analyzer.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.ADJUST_FLIGHT, True, Nothing, Nothing, String.Empty, myParams)
    End Sub

    Private Sub btnPerformBL_Click(sender As Object, e As EventArgs) Handles btnPerformBL.Click
        AnalyzerController.Instance.Analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.DynamicBL_Fill) = "END"
        AnalyzerController.Instance.Analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.DynamicBL_Read) = "INI"
        AnalyzerController.Instance.Analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.DynamicBL_Empty) = ""
        Dim myParams As New List(Of String)(New String() {CStr(Ax00FlightAction.Perform), "0"})
        AnalyzerController.Instance.Analyzer.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.ADJUST_FLIGHT, True, Nothing, Nothing, String.Empty, myParams)
    End Sub

    Private Sub btnEmptyRotor_Click(sender As Object, e As EventArgs) Handles btnEmptyRotor.Click
        AnalyzerController.Instance.Analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.DynamicBL_Fill) = "END"
        AnalyzerController.Instance.Analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.DynamicBL_Read) = "END"
        AnalyzerController.Instance.Analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.DynamicBL_Empty) = "INI"
        Dim myParams As New List(Of String)(New String() {CStr(Ax00FlightAction.EmptyRotor), "0"})
        AnalyzerController.Instance.Analyzer.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.ADJUST_FLIGHT, True, Nothing, Nothing, String.Empty, myParams)
    End Sub
End Class