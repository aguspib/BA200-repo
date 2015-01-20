Option Explicit On
Option Strict On

Imports System.Drawing

Imports DevExpress.XtraReports.UI

Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.BL

Public Class XRDesignMasterForm

#Region "Declarations"

    Private LanguageID As String

#End Region

#Region "Fields"

    Private ReportField As XtraReport = Nothing

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

#End Region

#Region "Events"

    Private Sub XRForm_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try
            'Get the current Language from the current Application Session
            Dim currentLanguageGlobal As New GlobalBase
            LanguageID = currentLanguageGlobal.GetSessionInfo().ApplicationLanguage

            PrepareButtons()

            GetScreenLabels()

            'If Not Report Is Nothing Then
            '    ' Bind the report's printing system to the print control. 
            '    PrintControl1.PrintingSystem = Report.PrintingSystem

            '    ' Generate the report's print document. 
            '    Report.CreateDocument()

            '    'PrintControl1.PrintingSystem.Watermark.Text = "Biosystems"

            '    'PrintControl1.ShowPageMargins = False

            '    'Me.PreviewBar1.Visible = False
            '    'Me.PreviewBar3.Visible = False

            '    'Configure Multilanguage Menu items. Read from BD.
            '    PrintPreviewSubItem1.Caption = "Archivo"
            '    PrintPreviewSubItem2.Caption = "Vista"
            '    PrintPreviewSubItem3.Caption = "Fondo"

            '    'Configure Multilanguage Buttoms items. Read from BD.
            '    PrintPreviewBarItem4.Caption = "Buscar"
            '    PrintPreviewBarItem4.Hint = "Buscar en el documento"

            '    PrintPreviewBarItem9.Caption = "Imprimir"
            '    PrintPreviewBarItem9.Hint = "Impresión rápida"

            '    'PrintPreviewStaticItem1.Caption = "Prueba"

            '    If Report.Landscape Then
            '        PrintControl1.Zoom = 0.9
            '    End If
            'End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".XRForm_Load ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")

        End Try
    End Sub

    'Private Sub PrintControl1_SelectedPageChanged(ByVal sender As System.Object, ByVal e As DevExpress.XtraPrinting.PageEventArgs) Handles PrintControl1.SelectedPageChanged
    '    PrintPreviewStaticItem1.Caption = String.Format("Página {0} de {1}", e.Page.Index + 1, PrintControl1.PrintingSystem.Pages.Count)
    'End Sub

    'Private Sub XRForm_FormClosed(ByVal sender As System.Object, ByVal e As System.Windows.Forms.FormClosedEventArgs) Handles MyBase.FormClosed
    '    Try
    '        PrintControl1.PrintingSystem.ClearContent()
    '        PrintControl1.PrintingSystem = Nothing
    '        If Not Report Is Nothing Then
    '            Report.ClosePreview()
    '            Report = Nothing
    '        End If

    '    Catch ex As Exception
    '        CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".XRForm_FormClosed ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
    '        ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")

    '    End Try
    'End Sub

    Private Sub ExitButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ExitButton.Click
        Try
            Me.Close()

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".bsExitButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")

        End Try
    End Sub

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
            If (auxIconName <> String.Empty) Then
                ExitButton.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
            End If

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

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".GetScreenLabels ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".GetScreenLabels ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", Me)

        End Try
    End Sub

#End Region

End Class
