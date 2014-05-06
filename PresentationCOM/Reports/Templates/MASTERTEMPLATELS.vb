'Imports DevExpress.XtraReports.UserDesigner
Imports System.ComponentModel.Design
Imports DevExpress.XtraReports.UI

Public Class MasterTemplateLS


    Private Sub XtraReport1_DesignerLoaded(ByVal sender As Object, ByVal e As DevExpress.XtraReports.UserDesigner.DesignerLoadedEventArgs) Handles Me.DesignerLoaded
        Dim designer As ComponentDesigner = DirectCast(e.DesignerHost.GetDesigner(DirectCast(sender, XtraReport)), ComponentDesigner)

        designer.Verbs.RemoveAt(1)
        designer.Verbs.RemoveAt(0)

        designer.ActionLists.RemoveAt(4)
        designer.ActionLists.RemoveAt(3)
        designer.ActionLists.RemoveAt(2)
        designer.ActionLists.RemoveAt(1)
        designer.ActionLists.RemoveAt(0)

        ' Obtain the TopMargin band and remove it from the report. 
        'Dim band As Band = Bands.GetBandByType(GetType(TopMarginBand))
        'If Not (band Is Nothing) Then
        '    Bands.Remove(band)
        'End If

        '' Obtain the BottomMargin band and remove it from the report. 
        'band = Bands.GetBandByType(GetType(BottomMarginBand))
        'If Not (band Is Nothing) Then
        '    Bands.Remove(band)
        'End If

    End Sub



    Private Sub XtraReport1_FilterControlProperties(ByVal sender As Object, ByVal e As DevExpress.XtraReports.UserDesigner.FilterControlPropertiesEventArgs) Handles Me.FilterControlProperties
        'e.Properties.Remove("Angle")
        Try

            'Select Case UCase(DirectCast(e.Control.GetType(), System.Type).Name.ToString)
            '    Case "TOPMARGINBAND"
            '        If Not e.Properties("PrintOnEmptyDataSource") Is Nothing Then e.Properties.Remove("PrintOnEmptyDataSource")
            '        If Not e.Properties("FormattingRules") Is Nothing Then e.Properties.Remove("FormattingRules")

            '    Case "PAGEHEADERBAND"
            '        If Not e.Properties("PrintOnEmptyDataSource") Is Nothing Then e.Properties.Remove("PrintOnEmptyDataSource")
            '        If Not e.Properties("FormattingRules") Is Nothing Then e.Properties.Remove("FormattingRules")


            '    Case "PAGEFOOTERBAND"
            '        If Not e.Properties("PrintOnEmptyDataSource") Is Nothing Then e.Properties.Remove("PrintOnEmptyDataSource")
            '        If Not e.Properties("FormattingRules") Is Nothing Then e.Properties.Remove("FormattingRules")

            '    Case "BOTTOMMARGINBAND"
            '        If Not e.Properties("PrintOnEmptyDataSource") Is Nothing Then e.Properties.Remove("PrintOnEmptyDataSource")
            '        If Not e.Properties("FormattingRules") Is Nothing Then e.Properties.Remove("FormattingRules")

            '        'Case UCase("XRLabel")
            '        '    '        'e.Properties.Remove("DataBindings")
            '        '    '        'e.Properties.Remove("Summary")
            '        '    '        'e.Properties.Remove("FormattingRules")
            '        '    '        e.Properties.Remove("AnchorVertical")

            '        'Case UCase("XRPictureBox")
            '        '    '        ' e.Properties.Remove("DataBindings")
            '        '    '        ' e.Properties.Remove("FormattingRules")
            '        '    '        e.Properties.Remove("AnchorVertical")

            '        'Case UCase("XRLine")
            '        '    '        'e.Properties.Remove("DataBindings")
            '        '    '        'e.Properties.Remove("FormattingRules")
            '        '    '        e.Properties.Remove("AnchorVertical")

            '        'Case UCase("XRShape")
            '        '    '        'e.Properties.Remove("FormattingRules")
            '        '    '        e.Properties.Remove("AnchorVertical")

            '        'Case UCase("XRPageInfo")
            '        '    '        'e.Properties.Remove("FormattingRules")
            '        '    '        e.Properties.Remove("AnchorVertical")
            '        '    '        e.Properties.Remove("RunningBand")

            '    Case UCase("XtraReport1"), "DETAILBAND", "BOTTOMMARGINBAND"
            '        ' do nothing

            '    Case Else
            '        e.Properties.Remove("DataBindings")
            '        e.Properties.Remove("Summary")
            '        e.Properties.Remove("FormattingRules")
            '        e.Properties.Remove("AnchorVertical")
            '        e.Properties.Remove("RunningBand")
            'End Select

            If e.Properties.Count > 0 Then

                e.Properties.Remove("DataBindings")
                e.Properties.Remove("Summary")
                e.Properties.Remove("FormattingRules")
                e.Properties.Remove("AnchorVertical")
                e.Properties.Remove("RunningBand")
                e.Properties.Remove("FormattingRules")
                e.Properties.Remove("Tag")
                '    e.Properties.Remove("Band")
                '    e.Properties.Remove("MasterReport")
                '    e.Properties.Remove("GridSize")
                '    e.Properties.Remove("CanShrink")
                '    e.Properties.Remove("BorderWidth")
                '    e.Properties.Remove("SnapGridSize")
                '    e.Properties.Remove("CurrentRowIndex")
                '    e.Properties.Remove("LocationF")
                '    e.Properties.Remove("Dock")
                '    e.Properties.Remove("ScriptLanguage")
                '    e.Properties.Remove("EventStyleName")
                '    e.Properties.Remove("SnappingMode")
                '    e.Properties.Remove("AnchorVertical")
                '    e.Properties.Remove("DrawGrid")
                '    e.Properties.Remove("BorderDashStyle")
                '    e.Properties.Remove("Bottom")
                '    e.Properties.Remove("Borders")
                '    e.Properties.Remove("HasChildren")
                '    e.Properties.Remove("NavigateUrl")
                '    e.Properties.Remove("ScriptSecurityPermissions")
                '    e.Properties.Remove("Font")
                '    e.Properties.Remove("BorderDashStyle_")
                '    e.Properties.Remove("SizeF")
                '    e.Properties.Remove("Expanded")
                '    e.Properties.Remove("WidthF")
                '    e.Properties.Remove("ExportOptions")
                '    e.Properties.Remove("Band")
                '    e.Properties.Remove("SnapLinePadding")
                '    e.Properties.Remove("Watermark")
                '    e.Properties.Remove("CrossBandControls")
                '    e.Properties.Remove("PrinterName")
                '    e.Properties.Remove("Styles")
                '    e.Properties.Remove("Height")
                '    e.Properties.Remove("BackColor_")
                '    e.Properties.Remove("XlsxFormatString")
                '    e.Properties.Remove("DataMember")
                '    e.Properties.Remove("SnapToGrid")
                '    e.Properties.Remove("StyleSheet")
                '    e.Properties.Remove("LockedInuserDesigner")
                '    e.Properties.Remove("PaperName")
                '    e.Properties.Remove("ForeColor_")
                '    e.Properties.Remove("StylePriority")
                '    e.Properties.Remove("PrintOnEmptyDataSorce")
                '    e.Properties.Remove("ParentStyleUsing")
                '    e.Properties.Remove("RowCount")
                '    e.Properties.Remove("Target")
                '    e.Properties.Remove("Container")
                '    e.Properties.Remove("RightF")
                '    e.Properties.Remove("Location")
                '    e.Properties.Remove("Tag")
                '    e.Properties.Remove("ScriptReferencesString")
                '    e.Properties.Remove("CalculatedFields")
                '    e.Properties.Remove("Site")
                '    e.Properties.Remove("Name")
                '    e.Properties.Remove("ShowPrintStatusDialog")
                '    e.Properties.Remove("Report")
                '    e.Properties.Remove("PreviewRowCount")
                '    e.Properties.Remove("Visible")
                '    e.Properties.Remove("Size")
                '    e.Properties.Remove("Parent")
                '    e.Properties.Remove("Right")
                '    e.Properties.Remove("Pages")
                '    e.Properties.Remove("Dpi")
                '    e.Properties.Remove("BorderColor")
                '    e.Properties.Remove("ShowPrintingWarnings")
                '    e.Properties.Remove("ForeColor")
                '    e.Properties.Remove("Scripts")
                '    e.Properties.Remove("PageSize")
                '    e.Properties.Remove("Width")
                '    e.Properties.Remove("Index")
                '    e.Properties.Remove("Bands")
                '    e.Properties.Remove("BackColor")
                '    e.Properties.Remove("Left")
                '    e.Properties.Remove("LeftF")
                '    e.Properties.Remove("Top")
                '    e.Properties.Remove("DisplayName")
                '    e.Properties.Remove("WordWrap")
                '    e.Properties.Remove("RootReports")
                '    e.Properties.Remove("Controls")
                '    e.Properties.Remove("Version")
                '    e.Properties.Remove("TopF")
                '    e.Properties.Remove("OddStyleName")
                '    e.Properties.Remove("CanGrow")
                '    e.Properties.Remove("Borders_")
                '    e.Properties.Remove("Text")
                '    e.Properties.Remove("Font_")
                '    e.Properties.Remove("BottomF")
                '    e.Properties.Remove("TextAlignment")
            End If

        Catch ex As Exception
            MsgBox(ex.Message)
        End Try

    End Sub

End Class