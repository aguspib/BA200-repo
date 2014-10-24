Option Strict On
Option Explicit On


Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Global.GlobalEnumerates
Imports Biosystems.Ax00.DAL.DAO
Imports Biosystems.Ax00.CommunicationsSwFw
Imports System.Windows.Forms
Imports Biosystems.Ax00.Global.GlobalConstants
Imports Biosystems.Ax00.BL
Imports System.IO
Imports System.Threading
Imports Biosystems.Ax00.PresentationCOM
Imports Biosystems.Ax00.App

Public Class TestForm

    Private myMDI As Ax00ServiceMainMDI
    Public Sub New(ByRef pMDI As Ax00ServiceMainMDI)

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        myMDI = pMDI
    End Sub

    'Private Sub BsButton1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsButton1.Click
    '    Dim MyFormP As New FormPilar
    '    MyFormP.Show()
    'End Sub

    Private Sub BsButton2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsButton2.Click
        Dim MyFormP As New FormXavi
        MyFormP.Show()
    End Sub

    Private Sub BsButton3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsButton3.Click
        Dim MyFormP As New FormSergio(myMDI)
        MyFormP.Show()
    End Sub

    Private Sub BsButton4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsButton4.Click
        Dim MyFormP As New TestCrearScripts
        MyFormP.Show()
    End Sub

    Private Sub BsButton5_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsButton5.Click
        Dim MyFormP As New IPhotometryAdjustments
        MyFormP.Show()
    End Sub

    Private Sub BsButton7_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsButton7.Click
        Dim myTestPDF As New TestPDF
        myTestPDF.Show()
    End Sub

    Private Sub BsButton8_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsButton8.Click
        Dim myGlobal As New GlobalDataTO
        Dim myFWUtil As New FWUpdateRequestTO(GlobalEnumerates.FwUpdateActions.QueryNeeded)
        myGlobal = AnalyzerController.Instance.Analyzer.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.FW_UTIL, True, Nothing, myFWUtil)
    End Sub

End Class