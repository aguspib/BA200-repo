Option Explicit On
Option Strict On

Imports System.Drawing

Imports DevExpress.XtraReports.UI

Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.BL
Imports System.Windows.Forms

Public Class XRMainForm

#Region "Declarations"

    Private LanguageID As String

#End Region

#Region "Fields"
    Private ReportField As XtraReport
    'JVV 01/10
    Private pagesPrinted As Integer = 0
    Private reportPages As Integer = 0
    'JVV 01/10
#End Region

#Region "Properties"

    Public Property Report() As XtraReport
        Get
            Return ReportField
        End Get
        Set(ByVal value As XtraReport)
            ReportField = value
        End Set
    End Property
    'JVV 01/10
    Public ReadOnly Property IsReportPrinted() As Boolean
        Get
            Return reportPages <= pagesPrinted
            'Més senzill per qualsevol cas: Return pagesPrinted > 0
        End Get
    End Property
    'JVV 01/10
#End Region

#Region "Events"

    ''' <summary>
    ''' When the screen is ESC key is pressed, code of Cancel Button is executed;
    ''' in other case, the screen is closed
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 14/05/2012
    ''' </remarks>
    Private Sub XRMainForm_KeyDown(ByVal sender As Object, ByVal e As KeyEventArgs) Handles Me.KeyDown
        Try
            If (e.KeyCode = Keys.Escape) Then
                ExitButton.PerformClick()
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".XRMainForm_KeyDown ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".XRMainForm_KeyDown", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub XRForm_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try
            'Get the current Language from the current Application Session
            Dim currentLanguageGlobal As New GlobalBase
            LanguageID = currentLanguageGlobal.GetSessionInfo().ApplicationLanguage

            PrepareButtons()

            GetScreenLabels()

            'JVV 1/10
            'AddHandler Me.ReportField.AfterPrint, AddressOf Me.AfterPrint 'No serveix ja que es genera l'event un cop s'ha fet el CreateDocument i no quan es prem el botó d'Imprimir!!
            AddHandler Me.ReportField.PrintProgress, AddressOf Me.PrintProgress
            'JVV 1/10

            If Not Report Is Nothing Then
                ' Bind the report's printing system to the print control. 

                PrintControl1.PrintingSystem = Report.PrintingSystem


                ' Generate the report's print document. 
                Report.CreateDocument()
                'JVV 2/10
                reportPages = Me.ReportField.Pages.Count
                'JVV 2/10

                'PrintControl1.PrintingSystem.Watermark.Text = "Biosystems"
                'PrintControl1.ShowPageMargins = False

                Me.PreviewBar3.Visible = False

                BsPanel3.Height = 642

                'ShowHideVisibleElements()

                If Report.Landscape Then PrintControl1.Zoom = 0.9
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".XRForm_Load ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")

        End Try
    End Sub

    'JVV 1/10
    'Private Sub AfterPrint(ByVal obj As Object, ByVal ev As EventArgs)
    '    MessageBox.Show("Tot imprès!!")
    'End Sub

    Private Sub PrintProgress(ByVal obj As Object, ByVal ev As DevExpress.XtraPrinting.PrintProgressEventArgs)
        pagesPrinted += 1
        'MessageBox.Show("Pag a imprimir: " & ev.PageIndex.ToString)
    End Sub
    'JVV 1/10


    Private Sub ShowHideVisibleElements()
        'Zoom
        ZoomBarEditItem1.Visibility = DevExpress.XtraBars.BarItemVisibility.Never

        'StopPageBuilding Stop
        PrintPreviewBarItem1.Visibility = DevExpress.XtraBars.BarItemVisibility.Never

        'DocumentMap Document Map
        PrintPreviewBarItem2.Visibility = DevExpress.XtraBars.BarItemVisibility.Never

        'Parameters Parameters
        PrintPreviewBarItem3.Visibility = DevExpress.XtraBars.BarItemVisibility.Never

        'Find Search
        PrintPreviewBarItem4.Visibility = DevExpress.XtraBars.BarItemVisibility.Never

        'Customize Customize
        PrintPreviewBarItem5.Visibility = DevExpress.XtraBars.BarItemVisibility.Never

        'Open Open
        PrintPreviewBarItem6.Visibility = DevExpress.XtraBars.BarItemVisibility.Never

        'Save Save
        PrintPreviewBarItem7.Visibility = DevExpress.XtraBars.BarItemVisibility.Never

        'Print &Print...
        PrintPreviewBarItem8.Visibility = DevExpress.XtraBars.BarItemVisibility.Always

        'PrintDirect Print
        PrintPreviewBarItem9.Visibility = DevExpress.XtraBars.BarItemVisibility.Never  'DevExpress.XtraBars.BarItemVisibility.Always DL 13/07/2012

        'PageSetup Page Set&up...
        PrintPreviewBarItem10.Visibility = DevExpress.XtraBars.BarItemVisibility.Never

        'EditPageHF Header And Footer
        PrintPreviewBarItem11.Visibility = DevExpress.XtraBars.BarItemVisibility.Never

        'Scale Scale
        PrintPreviewBarItem12.Visibility = DevExpress.XtraBars.BarItemVisibility.Never

        'HandTool Hand Tool
        PrintPreviewBarItem13.Visibility = DevExpress.XtraBars.BarItemVisibility.Never

        'Magnifier Magnifier
        PrintPreviewBarItem14.Visibility = DevExpress.XtraBars.BarItemVisibility.Never

        'ZoomOut Zoom Out
        PrintPreviewBarItem15.Visibility = DevExpress.XtraBars.BarItemVisibility.Never

        'ZoomIn Zoom In
        PrintPreviewBarItem16.Visibility = DevExpress.XtraBars.BarItemVisibility.Never

        'ShowFirstPage First Page
        PrintPreviewBarItem17.Visibility = DevExpress.XtraBars.BarItemVisibility.Never

        'ShowPrevPage Previous Page
        PrintPreviewBarItem18.Visibility = DevExpress.XtraBars.BarItemVisibility.Never

        'ShowNextPage Next Page
        PrintPreviewBarItem19.Visibility = DevExpress.XtraBars.BarItemVisibility.Never

        'ShowLastPage Last Page
        PrintPreviewBarItem20.Visibility = DevExpress.XtraBars.BarItemVisibility.Never

        'MultiplePages Multiple Pages
        PrintPreviewBarItem21.Visibility = DevExpress.XtraBars.BarItemVisibility.Never

        'FillBackground &Color...
        PrintPreviewBarItem22.Visibility = DevExpress.XtraBars.BarItemVisibility.Never

        'Watermark &Watermark...
        PrintPreviewBarItem23.Visibility = DevExpress.XtraBars.BarItemVisibility.Never

        'ExportFile Export Document...
        PrintPreviewBarItem24.Visibility = DevExpress.XtraBars.BarItemVisibility.Never

        'SendFile Send via E-Mail...
        PrintPreviewBarItem25.Visibility = DevExpress.XtraBars.BarItemVisibility.Never

        'ClosePreview E&xit
        PrintPreviewBarItem26.Visibility = DevExpress.XtraBars.BarItemVisibility.Never
    End Sub

    Private Sub PrintControl1_SelectedPageChanged(ByVal sender As System.Object, ByVal e As DevExpress.XtraPrinting.PageEventArgs) Handles PrintControl1.SelectedPageChanged
        PrintPreviewStaticItem1.Caption = String.Format("Página {0} de {1}", e.Page.Index + 1, PrintControl1.PrintingSystem.Pages.Count)
    End Sub

    Private Sub XRForm_FormClosed(ByVal sender As System.Object, ByVal e As FormClosedEventArgs) Handles MyBase.FormClosed
        Try
            'JVV 01/10
            'If Report.Pages.Count > pagesPrinted Then MessageBox.Show("Printed: " & pagesPrinted & " / " & Report.Pages.Count)
            'JVV 01/10
            PrintControl1.PrintingSystem.ClearContent()
            PrintControl1.PrintingSystem = Nothing
            If Not Report Is Nothing Then
                Report.ClosePreview()
                Report = Nothing
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".XRForm_FormClosed ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")

        End Try
    End Sub

    Private Sub ExitButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ExitButton.Click
        Try
            Me.Close()

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".bsExitButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")

        End Try
    End Sub

    ' ''' <summary>
    ' ''' To Export the report: Put a button in the toolbar and in its click event, perform the action below 
    ' ''' </summary>
    ' ''' <param name="sender"></param>
    ' ''' <param name="e"></param>
    ' ''' CREATED BY: JV 21/11/2013
    ' ''' <remarks></remarks>
    ''Private Sub BarButtonExport_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles BarButtonExport.ItemClick
    ''    Try
    ''        Report.ExportToPdf(GlobalBase.TemporalDirectory & "test.pdf")
    ''    Catch ex As Exception
    ''        CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".BarButtonExport_ItemClick ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
    ''        ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
    ''    End Try
    ''End Sub

#End Region

#Region "Private Methods"

    ''' <summary>
    ''' Loads the images for the buttons.
    ''' </summary>
    ''' <remarks>
    ''' Created by:  RH 07/12/2011
    ''' </remarks>
    Private Sub PrepareButtons()
        Try
            Dim auxIconName As String = String.Empty
            Dim iconPath As String = MyBase.IconsPath

            auxIconName = GetIconName("CANCEL")
            If Not String.Equals(auxIconName, String.Empty) Then ExitButton.Image = Image.FromFile(iconPath & auxIconName)

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".PrepareButtons ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

#End Region

#Region "MultiLanguage"
    ''' <summary>
    ''' Gets texts in the current application language for all screen controls
    ''' </summary>
    ''' <remarks>
    ''' Created by:  RH 07/12/2011
    ''' </remarks>
    Private Sub GetScreenLabels()
        Try
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            'RH 05/01/2012 Text property needs to have a value different from String.Empty. Otherwise the form closes itself.
            Me.Text = " "

            ' For Tooltips...
            bsToolTips1.SetToolTip(ExitButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_CloseScreen", LanguageID))

            'RH 04/06/2012
            PrintPreviewBarItem8.Hint = myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Print", LanguageID)
            PrintPreviewBarItem8.Caption = PrintPreviewBarItem8.Hint & "..."

            PrintPreviewBarItem9.Caption = PrintPreviewBarItem8.Hint
            PrintPreviewBarItem9.Hint = myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_QuickPrint", LanguageID)

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".GetScreenLabels ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".GetScreenLabels ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", Me)

        End Try
    End Sub

#End Region

End Class
