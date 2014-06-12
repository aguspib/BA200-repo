'Imports System.ComponentModel.Design
Imports System.Windows.Forms
Imports System.Drawing
Imports System.IO
Imports DevExpress.XtraReports.UI
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.Types
'Imports DevExpress.LookAndFeel
Imports DevExpress.XtraReports.UserDesigner
Imports DevExpress.XtraPrinting
Imports System.Drawing.Design
Imports System.Drawing.Imaging

Public Class IBandTemplateReport
    Inherits Biosystems.Ax00.PresentationCOM.BSBaseForm

#Region "Declarations"
    Private currentLanguage As String
    Private selectedTemplateName As String
    Private originalSelectedIndex As Integer = -1
    Private EditionMode As Boolean '= False                 'To validate when the selected template is in Edition Mode
    Private ChangesMade As Boolean '= False                 'To validate if there are changes pending to save when cancelling 
    Private newTemplate As Boolean '= False
    Private tmpPreviewPictureBox As New PictureBox()
    Private labelPortrait As String = "PORTRAIT"
    Private labelLandScape As String = "LANDSCAPE"
    Private WithEvents DesignPanel As XRDesignPanel
    Private WithEvents DesignForm As XRDesignForm

    'Private WithEvents DesignMasterForm As XRDesignMasterForm 'RH 14/02/2012

    Private ReportChanged As Boolean '= False
    Private SaveAs As Boolean '= False

    'Private AuxTemplateName As String = "" 'DL 09/02/2012
    Private ReadOnly PathTemplates As String = GlobalBase.AppPath & GlobalBase.ReportPath

    Private goRef As Boolean = False

    Private myTemplatePlug As String
    Private myPortraitPlug As String
    Private myLandscapePlug As String
#End Region

#Region "Private Methods"

    ''' <summary>
    ''' Edit Template
    ''' </summary>
    ''' <remarks>
    ''' Created by: DL 23/11/2011
    ''' </remarks>
    Private Sub EditTemplate()
        Try
            Cursor = Cursors.WaitCursor

            DeleteResidualFiles() 'IT 12/06/2014 #1661: Delete temporals reports before to edit another

            EditionMode = True
            bsEditButton.Enabled = False
            bsSaveButton.Enabled = True
            bsCancelButton.Enabled = True
            bsOrientationComboBox.Enabled = False

            'If master template then 
            If bsTemplatesListView.SelectedIndices.Count > 0 AndAlso CBool(bsTemplatesListView.Items(bsTemplatesListView.SelectedIndices(0)).SubItems(1).Text) Then
                bsTemplateTextBox.Enabled = False
                bsTemplateTextBox.BackColor = SystemColors.MenuBar

                bsOrientationComboBox.Enabled = False
                bsOrientationComboBox.BackColor = SystemColors.MenuBar

                bsDefaultCheckbox.Enabled = True

                bsEditReport.Enabled = False

            Else
                bsTemplateTextBox.Enabled = True
                bsTemplateTextBox.BackColor = Color.White
                '
                bsOrientationComboBox.Enabled = True
                bsOrientationComboBox.BackColor = Color.White  ' 03/01/2012

                bsDefaultCheckbox.Enabled = True
                bsEditReport.Enabled = True

            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".EditTemplate", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".EditTemplate", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")

        Finally
            Cursor = Cursors.Default

        End Try
    End Sub

    ''' <summary>
    ''' Show data of the selected templates in READ-ONLY MODE
    ''' </summary>
    ''' <remarks>
    ''' Created by: DL 23/11/2011
    ''' </remarks>
    Private Sub SelectTemplate(ByVal pRowSelect As Integer)
        Try
            Cursor = Cursors.WaitCursor

            Dim propiedadListView As System.Reflection.PropertyInfo

            propiedadListView = GetType(Form).GetProperty("DoubleBuffered", System.Reflection.BindingFlags.NonPublic Or System.Reflection.BindingFlags.Instance)
            propiedadListView.SetValue(bsTemplatesListView, True, Nothing)

            Me.Enabled = False 'Disable the screen

            ChangesMade = False
            newTemplate = False

            bsNewButton.Enabled = True
            bsSaveButton.Enabled = False
            bsCancelButton.Enabled = False
            bsTemplateTextBox.Enabled = False
            bsTemplateTextBox.BackColor = SystemColors.MenuBar
            bsOrientationComboBox.Enabled = False
            bsOrientationComboBox.BackColor = SystemColors.MenuBar
            bsDefaultCheckbox.Enabled = False
            bsEditReport.Enabled = False

            If CBool(bsTemplatesListView.Items(pRowSelect).SubItems(1).Text) Then
                bsDeleteButton.Enabled = False
                'bsEditButton.Enabled = False   'DL 20/02/2012
            Else
                bsDeleteButton.Enabled = True
                'bsEditButton.Enabled = True    'DL 20/02/2012
            End If

            bsEditButton.Enabled = True         'DL 20/02/2012

            selectedTemplateName = bsTemplatesListView.Items(pRowSelect).Text

            RemoveHandler bsTemplateTextBox.TextChanged, AddressOf bsTemplateTextBox_TextChanged
            RemoveHandler bsOrientationComboBox.TextChanged, AddressOf bsOrientationComboBox_TextChanged
            'RemoveHandler bsDefaultCheckbox.CheckedChanged, AddressOf bsDefaultCheckbox_CheckedChanged

            goRef = True

            bsTemplateTextBox.Text = bsTemplatesListView.Items(pRowSelect).Text

            If String.Equals(bsTemplatesListView.Items(pRowSelect).SubItems(2).Text, "PORTRAIT") Then
                bsOrientationComboBox.SelectedIndex = 0

            ElseIf String.Equals(bsTemplatesListView.Items(pRowSelect).SubItems(2).Text, "LANDSCAPE") Then
                bsOrientationComboBox.SelectedIndex = 1
            End If

            bsDefaultCheckbox.Checked = CBool(bsTemplatesListView.Items(pRowSelect).SubItems(4).Text)

            AddHandler bsTemplateTextBox.TextChanged, AddressOf bsTemplateTextBox_TextChanged
            AddHandler bsOrientationComboBox.TextChanged, AddressOf bsOrientationComboBox_TextChanged
            'AddHandler bsDefaultCheckbox.CheckedChanged, AddressOf bsDefaultCheckbox_CheckedChanged
            goRef = False

            'Add image
            Dim myPath As String = GlobalBase.AppPath & GlobalBase.ReportPath & "\" 'RH 15/12/2011

            tmpPreviewPictureBox = New PictureBox()
            'DL 09/05/2012
            'tmpPreviewPictureBox.ImageLocation = myPath & bsTemplatesListView.Items(pRowSelect).Text & ".gif"
            Dim myIndexTemplate As String = bsTemplatesListView.Items(pRowSelect).SubItems(3).Text.ToString
            Dim myLengthIndex As Integer = myIndexTemplate.ToString.Length - 5
            tmpPreviewPictureBox.ImageLocation = myPath & myIndexTemplate.Substring(0, myLengthIndex) & ".gif"
            'DL 09/05/2012

            tmpPreviewPictureBox.Dock = DockStyle.Fill
            tmpPreviewPictureBox.SizeMode = PictureBoxSizeMode.StretchImage

            For i As Integer = (bsPicturePanel.Controls.Count - 1) To 0 Step -1
                Dim ctrl As Control = bsPicturePanel.Controls(i)
                bsPicturePanel.Controls.Remove(ctrl)
                ctrl.Dispose()
            Next i

            bsPicturePanel.Controls.Add(tmpPreviewPictureBox)

            If String.Equals(bsTemplatesListView.Items(pRowSelect).SubItems(2).Text, "PORTRAIT") Then
                bsPicturePanel.Size = New Size(350, 450) '(280, 390)
            ElseIf String.Equals(bsTemplatesListView.Items(pRowSelect).SubItems(2).Text, "LANDSCAPE") Then
                bsPicturePanel.Size = New Size(580, 400)
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsTemplatesListView_Click", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsTemplatesListView_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")

        Finally
            Me.Enabled = True
            Me.Cursor = Cursors.Default
            bsTemplatesListView.Focus()

        End Try
    End Sub


    ''' <summary>
    ''' Add New template
    ''' </summary>
    ''' <remarks>
    ''' Created by: DL 23/11/2011
    ''' AG 12/06/2014 - #1661
    ''' </remarks>
    Private Function AddTemplate(ByVal pFileReport) As GlobalDataTO

        Dim resultData As New GlobalDataTO

        Try
            Dim myTemplateDS As New ReportTemplatesDS
            Dim newRow As ReportTemplatesDS.tcfgReportTemplatesRow
            Dim templateList As New ReportTemplatesDelegate

            resultData = templateList.Read(Nothing, bsTemplateTextBox.Text)

            If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                myTemplateDS = CType(resultData.SetDatos, ReportTemplatesDS)

                Dim myCurrentOrientation As String

                If String.Equals(bsOrientationComboBox.Text, labelPortrait) Then
                    myCurrentOrientation = "PORTRAIT"
                Else
                    myCurrentOrientation = "LANDSCAPE"
                End If

                If myTemplateDS.tcfgReportTemplates.Count > 0 Then
                    'AG 11/06/2014 #1661
                    'myTemplateDS.tcfgReportTemplates(0).TemplateOrientation = UCase(bsOrientationComboBox.Text.Trim)
                    myTemplateDS.tcfgReportTemplates(0).BeginEdit()
                    '1- Update name
                    If myTemplateDS.tcfgReportTemplates(0).TemplateName <> bsTemplateTextBox.Text Then
                        myTemplateDS.tcfgReportTemplates(0).TemplateName = bsTemplateTextBox.Text
                    End If

                    '2- Update orientation
                    If myTemplateDS.tcfgReportTemplates(0).TemplateOrientation <> myCurrentOrientation Then
                        myTemplateDS.tcfgReportTemplates(0).TemplateOrientation = myCurrentOrientation
                    End If

                    '3- Update default template
                    If myTemplateDS.tcfgReportTemplates(0).DefaultTemplate <> bsDefaultCheckbox.Checked Then
                        myTemplateDS.tcfgReportTemplates(0).DefaultTemplate = bsDefaultCheckbox.Checked
                    End If
                    myTemplateDS.tcfgReportTemplates(0).EndEdit()
                    'AG 11/06/2014 #1661
                Else
                    newRow = myTemplateDS.tcfgReportTemplates.NewtcfgReportTemplatesRow
                    newRow.DefaultTemplate = False
                    newRow.TemplateName = bsTemplateTextBox.Text
                    newRow.TemplateFileName = pFileReport
                    newRow.TemplateOrientation = myCurrentOrientation 'UCase(bsOrientationComboBox.Text.Trim)
                    newRow.DefaultTemplate = bsDefaultCheckbox.Checked
                    newRow.MasterTemplate = False

                    myTemplateDS.tcfgReportTemplates.AddtcfgReportTemplatesRow(newRow)

                    resultData = templateList.CreateReportTemplate(Nothing, myTemplateDS)

                    If (Not resultData.HasError) Then
                        resultData.SetDatos = myTemplateDS 'AG 12/06/2014 - #1661

                        bsTemplatesListView.Items.Add(bsTemplateTextBox.Text)
                        bsTemplatesListView.Items(bsTemplatesListView.Items.Count - 1).SubItems.Add(False)
                        bsTemplatesListView.Items(bsTemplatesListView.Items.Count - 1).SubItems.Add(myCurrentOrientation) 'UCase(bsOrientationComboBox.Text.Trim))
                        bsTemplatesListView.Items(bsTemplatesListView.Items.Count - 1).SubItems.Add(pFileReport)
                        bsTemplatesListView.Items(bsTemplatesListView.Items.Count - 1).SubItems.Add(bsDefaultCheckbox.Checked)
                    End If
                End If

            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".AddTemplate", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".AddTemplate", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try

        Return resultData

    End Function

#End Region

#Region "Events"

    ''' <summary>
    ''' Open End User design report
    ''' </summary>
    ''' <remarks>
    ''' Created by: DL 3/01/2012
    ''' </remarks>
    Private Sub bsEditReport_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsEditReport.Click
        Try
            OpenReport()

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsEditReport_Click", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsEditReport_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Closes Template design form
    ''' </summary>
    ''' <remarks>
    ''' Created by: DL 23/11/2011
    ''' Modified by: RH 15/12/2011
    ''' </remarks>
    Private Sub bsExitButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsExitButton.Click
        Try
            'RH 15/12/2011 New version. The old one does not work properly.
            If EditionMode AndAlso ChangesMade Then ' OrElse ReportDesignxUserControl.ReportChanged) Then
                If (ShowMessage(Me.Name, GlobalEnumerates.Messages.DISCARD_PENDING_CHANGES.ToString()) = DialogResult.Yes) Then
                    Me.Enabled = False 'RH 13/04/2012 Disable the form to avoid pressing buttons on closing
                    Close()
                End If
            Else
                Me.Enabled = False 'RH 13/04/2012 Disable the form to avoid pressing buttons on closing
                Close()
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsExitButton_Click", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsExitButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub


    ''' <summary>
    ''' Save (creates or updates) changes in templates
    ''' </summary>
    ''' <remarks>
    ''' AG 11/06/2014 #1661 allow the preloaded mastertemplate become default again (use variable templateName instead of control bsTemplateTextBox.Text)
    ''' </remarks>
    Private Sub SaveReport()
        Try
            Cursor = Cursors.WaitCursor 'RH 15/12/2011
            bsScreenErrorProvider.Clear()

            Dim SelectIndex As Integer

            If Not String.IsNullOrEmpty(bsTemplateTextBox.Text) AndAlso Not String.IsNullOrEmpty(bsOrientationComboBox.Text) Then

                Dim isDuplicated As Boolean = False

                'AG 11/06/2014 #1661 - In case we are editing a preloaded template we can not use the name in screen because is different from name in DB
                Dim templateName As String = bsTemplateTextBox.Text
                If bsTemplatesListView.SelectedIndices.Count > 0 AndAlso CBool(bsTemplatesListView.Items(bsTemplatesListView.SelectedIndices(0)).SubItems(1).Text) Then
                    templateName = bsTemplatesListView.Items(bsTemplatesListView.SelectedIndices(0)).SubItems(5).Text.ToString
                End If
                'AG 11/06/2014

                For i As Integer = 0 To bsTemplatesListView.Items.Count - 1

                    'Before save check for the template wont be duplicated!!!
                    If Not newTemplate Then
                        If Not String.Equals(bsTemplatesListView.SelectedItems(0).Text, templateName) AndAlso _
                           String.Equals(bsTemplatesListView.Items(i).Text, templateName) Then isDuplicated = True

                    Else
                        If String.Equals(bsTemplatesListView.Items(i).Text, templateName) Then isDuplicated = True

                    End If

                    If isDuplicated Then Exit For

                Next i

                'Deny permission is duplicated
                If isDuplicated Then
                    'bsScreenErrorProvider.SetError(bsTemplateTextBox, GetMessageText(GlobalEnumerates.Messages.REPEATED_NAME.ToString)) 'Duplicated test name
                    bsScreenErrorProvider.SetError(bsTemplateTextBox, GetMessageText(GlobalEnumerates.Messages.FILE_EXIST.ToString)) 'Duplicated name
                    bsTemplateTextBox.Focus()

                    'If not duplicate save changes (new or update)
                Else
                    Dim resultData As GlobalDataTO = Nothing
                    Dim templateList As New ReportTemplatesDelegate
                    Dim myFileReport As String

                    'Next code applies only for the USER templates, not for the preloaded (whose designer could not be edited)
                    If bsTemplatesListView.SelectedIndices.Count > 0 AndAlso Not CBool(bsTemplatesListView.Items(bsTemplatesListView.SelectedIndices(0)).SubItems(1).Text) Then
                        If File.Exists(PathTemplates & "\TEMP.REPX") AndAlso File.Exists(PathTemplates & "\" & templateName & ".REPX") Then
                            File.Delete(PathTemplates & "\" & templateName & ".REPX")
                            File.Delete(PathTemplates & "\" & templateName & ".GIF")

                            File.Copy(PathTemplates & "\TEMP.REPX", PathTemplates & "\" & templateName & ".REPX")
                            File.Copy(PathTemplates & "\TEMP.GIF", PathTemplates & "\" & templateName & ".GIF")

                            File.Delete(PathTemplates & "\TEMP.REPX")
                            File.Delete(PathTemplates & "\TEMP.GIF")


                        ElseIf Not newTemplate AndAlso Not String.Equals(bsTemplatesListView.SelectedItems(0).Text, templateName) Then

                            If File.Exists(PathTemplates & "\TEMP.REPX") AndAlso File.Exists(PathTemplates & "\" & bsTemplatesListView.SelectedItems(0).Text & ".REPX") Then
                                File.Delete(PathTemplates & bsTemplatesListView.SelectedItems(0).Text & ".REPX")
                                File.Delete(PathTemplates & bsTemplatesListView.SelectedItems(0).Text & ".GIF")

                                File.Copy(PathTemplates & "\TEMP.REPX", PathTemplates & "\" & templateName & ".REPX")
                                File.Copy(PathTemplates & "\TEMP.GIF", PathTemplates & "\" & templateName & ".GIF")

                                File.Delete(PathTemplates & "\TEMP.REPX")
                                File.Delete(PathTemplates & "\TEMP.GIF")

                            Else
                                File.Delete(PathTemplates & templateName & ".REPX")
                                File.Delete(PathTemplates & templateName & ".GIF")

                                File.Copy(PathTemplates & "\" & bsTemplatesListView.SelectedItems(0).Text & ".REPX", _
                                          PathTemplates & "\" & templateName & ".REPX")

                                File.Copy(PathTemplates & "\" & bsTemplatesListView.SelectedItems(0).Text & ".GIF", _
                                          PathTemplates & "\" & templateName & ".GIF")

                            End If

                        End If
                    End If
                    myFileReport = templateName & ".REPX"

                    Dim myCurrentOrientation As String
                    If String.Equals(bsOrientationComboBox.Text, labelPortrait) Then
                        myCurrentOrientation = "PORTRAIT"
                    Else
                        myCurrentOrientation = "LANDSCAPE"
                    End If

                    If Not newTemplate AndAlso bsTemplatesListView.SelectedIndices.Count > 0 Then

                        'AG 12/06/2014 - #1661 (change order of IF's: 1st when change orientation / else 2on when renamed / else 3rd if only change default)
                        If Not String.Equals(bsTemplatesListView.Items(bsTemplatesListView.SelectedIndices(0)).SubItems(2).Text, myCurrentOrientation) Then
                            '1) When Orientation changed ---> Create new report (independent if also have been renamed and change default fields)
                            '
                            resultData = AddTemplate(myFileReport)

                            Dim myTemplateDS As ReportTemplatesDS
                            If Not resultData.HasError Then
                                myTemplateDS = DirectCast(resultData.SetDatos, ReportTemplatesDS)
                                resultData = templateList.UpdateComplete(Nothing, myTemplateDS.tcfgReportTemplates(0))

                                If (Not resultData.HasError) Then bsTemplatesListView.Items(bsTemplatesListView.SelectedIndices(0)).SubItems(2).Text = bsOrientationComboBox.Text
                            End If

                        ElseIf Not String.Equals(bsTemplatesListView.Items(bsTemplatesListView.SelectedIndices(0)).Text, bsTemplateTextBox.Text) Then 'AG 11/06/2014 Do not replace bsTemplateTextBox.Text for templateName in this IF
                            '2) Report has been renamed (rename template name ID in database and also designer files)
                            '(independent if also changes default field)

                            'AG 11/06/2014 #1661 - In case we are editing a preloaded template we can not use the name in screen because is different from name in DB
                            'resultData = templateList.UpdateTemplateNameByOldName(Nothing, bsTemplateTextBox.Text, bsTemplatesListView.SelectedItems(0).Text)
                            resultData = templateList.UpdateRenamingTemplate(Nothing, templateName, bsTemplatesListView.SelectedItems(0).Text, myCurrentOrientation, bsDefaultCheckbox.Checked)

                        ElseIf Not String.Equals(bsTemplatesListView.Items(bsTemplatesListView.SelectedIndices(0)).SubItems(4).Text, bsDefaultCheckbox.Checked) Then
                            '3) Change DefaultTemplate field
                            'AG 11/06/2014 #1661 - In case we are editing a preloaded template we can not use the name in screen because is different from name in DB
                            'resultData = templateList.UpdateDefaultTemplateValueByTempltName(Nothing, bsTemplateTextBox.Text, bsDefaultCheckbox.Checked)
                            resultData = templateList.UpdateDefaultTemplateFieldByName(Nothing, templateName, bsDefaultCheckbox.Checked)

                        End If

                    ElseIf newTemplate Then
                        'Create new report
                        resultData = AddTemplate(myFileReport)
                    End If

                    If (resultData Is Nothing) OrElse (Not resultData.HasError) Then 'RH 19/12/2011
                        tmpPreviewPictureBox.Image = Nothing
                        'DL 03/01/2012
                        Dim sFile As String = PathTemplates & "\" & templateName
                        If newTemplate And SaveAs Then

                            Rename(PathTemplates & "\TEMP.GIF", sFile & ".GIF")
                            Rename(PathTemplates & "\TEMP.REPX", sFile & ".REPX")

                        ElseIf newTemplate And Not SaveAs Then

                            If String.Equals(myCurrentOrientation, "PORTRAIT") Then
                                File.Copy(PathTemplates & "\MASTERTEMPLATE.REPX", sFile & ".REPX")
                                File.Copy(PathTemplates & "\MASTERTEMPLATE.GIF", sFile & ".GIF")

                            ElseIf String.Equals(myCurrentOrientation, "LANDSCAPE") Then
                                File.Copy(PathTemplates & "\MASTERTEMPLATELS.REPX", sFile & ".REPX")
                                File.Copy(PathTemplates & "\MASTERTEMPLATELS.GIF", sFile & ".GIF")

                            End If

                        End If

                        'DL 03/01/2012
                        Dim auxTemplate As String = templateName
                        LoadTemplatesList()

                        For i As Integer = 0 To bsTemplatesListView.Items.Count - 1
                            If String.Equals(bsTemplatesListView.Items(i).Text, auxTemplate) Then
                                SelectIndex = i
                                Exit For
                            End If
                        Next i

                        EditionMode = False
                    End If

                    bsTemplateTextBox.Enabled = False
                    bsOrientationComboBox.Enabled = False
                    bsDefaultCheckbox.Enabled = False
                    bsEditButton.Enabled = True
                    bsTemplatesListView.Enabled = True
                    bsEditReport.Enabled = False
                    bsSaveButton.Enabled = False
                    bsCancelButton.Enabled = False

                    bsTemplatesListView.SelectedItems.Clear()

                    bsTemplatesListView.Items(SelectIndex).Selected = True
                    SelectTemplate(SelectIndex)

                    'RH Update Cached Default Template
                    If bsDefaultCheckbox.Checked Then
                        If String.Equals(myCurrentOrientation, "PORTRAIT") Then
                            'Load Reports Default Portrait Template
                            If Not XRManager.LoadDefaultPortraitTemplate() Then
                                'No Reports template has been loaded.
                                'Take the proper action!
                            End If
                        Else
                            'Load Reports Default Landscape Template
                            If Not XRManager.LoadDefaultLandscapeTemplate() Then
                                'No Reports template has been loaded.
                                'Take the proper action!
                            End If
                        End If
                    End If
                End If
            End If

            DeleteResidualFiles()

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".SaveReport", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".SaveReport", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")

        Finally
            Cursor = Cursors.Default 'RH 15/12/2011

        End Try

    End Sub

    ''' <summary>
    ''' Save Template
    ''' </summary>
    ''' <remarks>
    ''' Created by: DL 23/11/2011
    ''' </remarks>
    Private Sub bsSaveButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsSaveButton.Click

        Try
            SaveReport()

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsSaveButton_Click", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsSaveButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")

        End Try

    End Sub

    ''' <summary>
    ''' Delete template
    ''' </summary>
    ''' <remarks>
    ''' Created by: DL 23/11/2011
    ''' </remarks>
    Private Sub bsDeleteButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles bsDeleteButton.Click

        Try
            DeleteTemplate()

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsDeleteButton_Click", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsDeleteButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try

    End Sub

    ''' <summary>
    ''' Invoke event produce any change in the report 
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 24/11/2011 
    ''' </remarks>
    Private Sub DesignPanel_ReportStateChanged(ByVal sender As Object, ByVal e As ReportStateEventArgs) Handles DesignPanel.ReportStateChanged

        'Try
        If e.ReportState = ReportState.Changed Then
            ReportChanged = True
        Else
            ReportChanged = False
        End If

        'Catch ex As Exception
        '    Throw ex
        'End Try

    End Sub

    '''' <summary>
    '''' Export preview report to image
    '''' </summary>
    '''' <param name="nResolution">resolution</param>
    '''' <returns></returns>
    '''' <remarks></remarks>
    Public Sub ReportToFileBitmap(Optional ByVal nResolution As Integer = 96)

        Dim AuxFile As String = PathTemplates & "\AUXTEMP.REPX"

        File.Delete(AuxFile)
        File.Copy(PathTemplates & "\TEMP.REPX", AuxFile)

        ' Set Image-specific export options.
        Dim oImageFormat As New ImageExportOptions
        oImageFormat.ExportMode = ImageExportMode.SingleFilePageByPage
        oImageFormat.Format = ImageFormat.Gif
        oImageFormat.Resolution = nResolution
        oImageFormat.PageRange = 1
        oImageFormat.PageBorderWidth = 1
        oImageFormat.PageBorderColor = Color.Black

        Dim ExistObjects As Boolean = False 'DL 09/11/2012.

        If String.Equals(bsOrientationComboBox.Text, labelPortrait) Then
            'Need aux report to export as image
            Dim copyMasterTemplate As New MASTERTEMPLATE
            copyMasterTemplate.LoadLayout(AuxFile)
            copyMasterTemplate.CreateDocument()

            'DL 09/11/2012. Begin. When is empty produces a error to export to image
            'IT 11/06/2014 #1661 (begin)
            For Each band As Band In copyMasterTemplate.Controls
                If TypeOf band Is DevExpress.XtraReports.UI.PageFooterBand Then
                    If band.Controls.Count > 2 Then
                        ExistObjects = True
                        Exit For
                    End If
                Else
                    If band.Controls.Count > 0 Then
                        ExistObjects = True
                        Exit For
                    End If
                End If
            Next
            'IT 11/06/2014 #1661 (end)

            If Not ExistObjects Then
                File.Delete(PathTemplates & "\TEMP.GIF")
                File.Copy(PathTemplates & "\MASTERTEMPLATE_Empty.gif", PathTemplates & "\TEMP.GIF")
            Else
                copyMasterTemplate.ExportToImage(PathTemplates & "\TEMP.GIF", oImageFormat)
            End If
            'DL 09/11/2012. End

        Else
            'Need aux report to export as image
            Dim copyMasterTemplateLS As New MasterTemplateLS
            copyMasterTemplateLS.LoadLayout(AuxFile)
            copyMasterTemplateLS.CreateDocument()

            'DL 09/11/2012. Begin. When is empty produces a error to export to image
            'IT 11/06/2014 #1661 (begin)
            For Each band As Band In copyMasterTemplateLS.Controls
                If TypeOf band Is DevExpress.XtraReports.UI.PageFooterBand Then
                    If band.Controls.Count > 2 Then
                        ExistObjects = True
                        Exit For
                    End If
                Else
                    If band.Controls.Count > 0 Then
                        ExistObjects = True
                        Exit For
                    End If
                End If
            Next
            'IT 11/06/2014 #1661 (end)

            If Not ExistObjects Then
                File.Delete(PathTemplates & "\TEMP.GIF")
                File.Copy(PathTemplates & "\MASTERTEMPLATELS_Empty.gif", PathTemplates & "\TEMP.GIF")
            Else
                copyMasterTemplateLS.ExportToImage(PathTemplates & "\TEMP.GIF", oImageFormat)
            End If
            'DL 09/11/2012. End
        End If

        File.Delete(AuxFile)
    End Sub

    Private Sub DesignForm_KeyDown(ByVal sender As Object, ByVal e As KeyEventArgs) Handles DesignForm.KeyDown
        Try
            If (e.KeyCode = Keys.Escape) Then
                DesignForm.Close()
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".IBandTemplateReport_KeyDown", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".IBandTemplateReport_KeyDown", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Save report when close end user design form
    ''' </summary>
    ''' <remarks>
    ''' Created by: DL 03/01/2012
    '''</remarks>
    Private Sub DesignForm_Closing(ByVal sender As Object, ByVal e As FormClosingEventArgs) Handles DesignForm.FormClosing
        Try
            Cursor = Cursors.WaitCursor

            If (DesignForm.IsDisposed) Then Exit Sub

            If newTemplate OrElse ReportChanged Then

                'Save temporal repx
                DesignForm.ActiveDesignPanel.SaveReport(PathTemplates & "\TEMP.REPX")

                ReportToFileBitmap()

                tmpPreviewPictureBox = New PictureBox()
                tmpPreviewPictureBox.ImageLocation = PathTemplates & "\TEMP.GIF"
                tmpPreviewPictureBox.Dock = DockStyle.Fill
                tmpPreviewPictureBox.SizeMode = PictureBoxSizeMode.StretchImage

                For i As Integer = (bsPicturePanel.Controls.Count - 1) To 0 Step -1
                    Dim ctrl As Control = bsPicturePanel.Controls(i)
                    bsPicturePanel.Controls.Remove(ctrl)
                    ctrl.Dispose()
                Next i

                bsPicturePanel.Controls.Add(tmpPreviewPictureBox)
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".DesignForm_Closing", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".DesignForm_Closing", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")

        Finally
            Cursor = Cursors.Default

        End Try
    End Sub

    ''' <summary>
    ''' Invoke event when the DesignForm is Closed
    ''' </summary>
    ''' <remarks>
    ''' Created by: 'IT 11/06/2014 #1661
    '''</remarks>
    Private Sub DesignForm_Closed(ByVal sender As Object, ByVal e As FormClosedEventArgs) Handles DesignForm.FormClosed
        Try
            Cursor = Cursors.WaitCursor
            DesignForm.Dispose()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".DesignForm_Closed", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".DesignForm_Closed", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        Finally
            Cursor = Cursors.Default
        End Try
    End Sub

    ''' <summary>
    ''' initialize toolbar
    ''' </summary>
    ''' <remarks>
    ''' Created by: DL 03/01/2012
    '''</remarks>
    Private Sub DesignMdiController_DesignPanelLoaded(ByVal sender As Object, ByVal e As DesignerLoadedEventArgs)

        Try
            Dim panel As XRDesignPanel = TryCast(e.DesignerHost.GetService(GetType(XRDesignPanel)), XRDesignPanel)

            panel.SetCommandVisibility(ReportCommand.SaveFile, DevExpress.XtraReports.UserDesigner.CommandVisibility.None)
            panel.SetCommandVisibility(ReportCommand.SaveFileAs, DevExpress.XtraReports.UserDesigner.CommandVisibility.None)

            panel.SetCommandVisibility(ReportCommand.PropertiesWindow, DevExpress.XtraReports.UserDesigner.CommandVisibility.ContextMenu)
            panel.SetCommandVisibility(ReportCommand.PropertiesWindow, DevExpress.XtraReports.UserDesigner.CommandVisibility.Toolbar)
            panel.SetCommandVisibility(ReportCommand.PropertiesWindow, DevExpress.XtraReports.UserDesigner.CommandVisibility.Verb)
            panel.SetCommandVisibility(ReportCommand.PropertiesWindow, DevExpress.XtraReports.UserDesigner.CommandVisibility.All)
            panel.SetCommandVisibility(ReportCommand.PropertiesWindow, DevExpress.XtraReports.UserDesigner.CommandVisibility.None)
            '
            ' Hide HTML/Script View
            '
            panel.SetCommandVisibility(ReportCommand.ShowHTMLViewTab, DevExpress.XtraReports.UserDesigner.CommandVisibility.None)
            panel.SetCommandVisibility(ReportCommand.ShowScriptsTab, DevExpress.XtraReports.UserDesigner.CommandVisibility.None)
            panel.SetCommandVisibility(ReportCommand.ShowWindowInterface, DevExpress.XtraReports.UserDesigner.CommandVisibility.None)
            panel.SetCommandVisibility(ReportCommand.ShowTabbedInterface, DevExpress.XtraReports.UserDesigner.CommandVisibility.None)
            '
            ' Hide Verb in Edit text/band
            '
            panel.SetCommandVisibility(ReportCommand.VerbEditBands, DevExpress.XtraReports.UserDesigner.CommandVisibility.None)
            panel.SetCommandVisibility(ReportCommand.VerbEditText, DevExpress.XtraReports.UserDesigner.CommandVisibility.None)
            panel.SetCommandVisibility(ReportCommand.VerbPivotGridDesigner, DevExpress.XtraReports.UserDesigner.CommandVisibility.None)
            panel.SetCommandVisibility(ReportCommand.VerbReportWizard, DevExpress.XtraReports.UserDesigner.CommandVisibility.None)
            panel.SetCommandVisibility(ReportCommand.VerbRtfClear, DevExpress.XtraReports.UserDesigner.CommandVisibility.None)
            panel.SetCommandVisibility(ReportCommand.VerbRtfLoadFile, DevExpress.XtraReports.UserDesigner.CommandVisibility.None)
            panel.SetCommandVisibility(ReportCommand.InsertBottomMarginBand, DevExpress.XtraReports.UserDesigner.CommandVisibility.None)
            panel.SetCommandVisibility(ReportCommand.InsertDetailBand, DevExpress.XtraReports.UserDesigner.CommandVisibility.None)
            panel.SetCommandVisibility(ReportCommand.InsertDetailReport, DevExpress.XtraReports.UserDesigner.CommandVisibility.None)
            panel.SetCommandVisibility(ReportCommand.InsertGroupFooterBand, DevExpress.XtraReports.UserDesigner.CommandVisibility.None)
            panel.SetCommandVisibility(ReportCommand.InsertGroupHeaderBand, DevExpress.XtraReports.UserDesigner.CommandVisibility.None)
            panel.SetCommandVisibility(ReportCommand.InsertPageFooterBand, DevExpress.XtraReports.UserDesigner.CommandVisibility.None)
            panel.SetCommandVisibility(ReportCommand.InsertPageHeaderBand, DevExpress.XtraReports.UserDesigner.CommandVisibility.None)
            panel.SetCommandVisibility(ReportCommand.InsertReportFooterBand, DevExpress.XtraReports.UserDesigner.CommandVisibility.None)
            panel.SetCommandVisibility(ReportCommand.InsertReportHeaderBand, DevExpress.XtraReports.UserDesigner.CommandVisibility.None)
            panel.SetCommandVisibility(ReportCommand.InsertTopMarginBand, DevExpress.XtraReports.UserDesigner.CommandVisibility.None)
            panel.SetCommandVisibility(ReportCommand.BandMoveDown, DevExpress.XtraReports.UserDesigner.CommandVisibility.None)
            panel.SetCommandVisibility(ReportCommand.BandMoveUp, DevExpress.XtraReports.UserDesigner.CommandVisibility.None)
            panel.SetCommandVisibility(ReportCommand.MdiCascade, DevExpress.XtraReports.UserDesigner.CommandVisibility.None)
            panel.SetCommandVisibility(ReportCommand.MdiTileHorizontal, DevExpress.XtraReports.UserDesigner.CommandVisibility.None)
            panel.SetCommandVisibility(ReportCommand.MdiTileVertical, DevExpress.XtraReports.UserDesigner.CommandVisibility.None)
            panel.SetCommandVisibility(ReportCommand.Redo, DevExpress.XtraReports.UserDesigner.CommandVisibility.None)
            panel.SetCommandVisibility(ReportCommand.Exit, DevExpress.XtraReports.UserDesigner.CommandVisibility.None)

            RemoveHandler panel.ReportStateChanged, AddressOf DesignPanel_ReportStateChanged
            AddHandler panel.ReportStateChanged, AddressOf DesignPanel_ReportStateChanged

            Dim ts As IToolboxService = CType(e.DesignerHost.GetService(GetType(IToolboxService)), IToolboxService)
            Dim coll As ToolboxItemCollection = ts.GetToolboxItems()
            Dim myItemName As String

            ' Iterate through toolbox items.
            For Each item As ToolboxItem In coll
                ' Add the "Cool" prefix to all toolbox item names.
                myItemName = UCase(DirectCast(item.GetType(e.DesignerHost), System.Type).Name.ToString.Trim)

                If Not String.Equals(myItemName, "XRLABEL") AndAlso _
                   Not String.Equals(myItemName, "XRPICTUREBOX") AndAlso _
                   Not String.Equals(myItemName, "XRLINE") AndAlso _
                   Not String.Equals(myItemName, "XRSHAPE") AndAlso _
                   Not String.Equals(myItemName, "XRPAGEINFO") Then

                    ts.RemoveToolboxItem(item)

                End If

            Next item

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".DesignForm_Closing", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".DesignForm_Closing", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    '''' <summary>
    '''' Opens a Custom Report Designer (new version)
    '''' </summary>
    '''' <remarks>
    '''' Created by: RH 14/02/2012
    '''' </remarks>
    'Private Sub OpenReport_NEW()
    '    Try
    '        Using DesignMasterForm = New XRDesignMasterForm()
    '            DesignMasterForm.ShowDialog()
    '        End Using

    '    Catch ex As Exception
    '        CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".OpenReport", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
    '        ShowMessage(Me.Name & ".OpenReport", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")

    '    End Try
    'End Sub

    ''' <summary>
    ''' Configure and open report
    ''' </summary>
    ''' <remarks>
    ''' Created by: DL 23/11/2011
    ''' </remarks>
    Private Sub OpenReport()

        Try
            Cursor = Cursors.WaitCursor

            ' Create a design form.
            DesignForm = New XRDesignForm()

            ' hide toolbars
            DesignForm.DesignBarManager.Bars(0).Visible = False ' hide main menu
            DesignForm.DesignBarManager.Bars(1).Visible = False ' hide tool bar 
            DesignForm.DesignBarManager.Bars(3).Visible = False ' hide layout
            DesignForm.DesignBarManager.Bars(4).Visible = False ' hide status bar
            DesignForm.DesignBarManager.Bars(5).Visible = False ' hide zoom bar

            ' EDIT FORMATTING TOOLBARS
            DesignForm.DesignBarManager.AllowCustomization = False
            DesignForm.DesignBarManager.AllowMoveBarOnToolbar = False
            DesignForm.DesignBarManager.AllowQuickCustomization = False
            DesignForm.DesignBarManager.AllowShowToolbarsPopup = False

            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            ' Configure Formatting Toolbar
            With DesignForm.DesignBarManager.Bars(2)
                .CanDockStyle = DevExpress.XtraBars.BarCanDockStyle.Right
                .DockStyle = DevExpress.XtraBars.BarDockStyle.Top

                .Manager.AllowCustomization = False
                .Manager.AllowMoveBarOnToolbar = False
                .Manager.AllowQuickCustomization = False
                .Manager.AllowShowToolbarsPopup = False

                .OptionsBar.AllowDelete = False
                .OptionsBar.AllowQuickCustomization = False
                .OptionsBar.BarState = DevExpress.XtraBars.BarState.Expanded
                .OptionsBar.DisableClose = True
                .OptionsBar.DisableCustomization = True
                .OptionsBar.DrawDragBorder = False

                ' Multilanguage
                .ItemLinks(0).Caption = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Font", currentLanguage)             ' Font
                .ItemLinks(1).Caption = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Font_Size", currentLanguage)        ' Font Size 
                .ItemLinks(2).Caption = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Bold", currentLanguage)             ' Bold 
                .ItemLinks(3).Caption = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Italic", currentLanguage)           ' Italic 
                .ItemLinks(4).Caption = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Underline", currentLanguage)        ' Underline
                .ItemLinks(5).Caption = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Foreground_Color", currentLanguage) ' Foreground color
                .ItemLinks(6).Caption = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Background_Color", currentLanguage) ' Background color
                .ItemLinks(7).Caption = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Left", currentLanguage)             ' Left
                .ItemLinks(8).Caption = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Center", currentLanguage)           ' Center
                .ItemLinks(9).Caption = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Right", currentLanguage)            ' Right
                .ItemLinks(10).Caption = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Justify", currentLanguage)         ' Justify
            End With

            AddHandler DesignForm.DesignMdiController.DesignPanelLoaded, AddressOf DesignMdiController_DesignPanelLoaded

            Dim TemplateAux As New MASTERTEMPLATE
            'Dim myPath As String = GlobalBase.AppPath & GlobalBase.ReportPath & "\"
            Dim myFile As String = ""

            If newTemplate Then
                'AuxTemplateName = myPath & myFile
                myFile = PathTemplates & "\TEMP.REPX"
                'TemplateAux.LoadLayout(myFile)

                SaveAs = True

            Else
                'TemplateAux.LoadLayout(PathTemplates & "\" & bsTemplateTextBox.Text & ".REPX")

                If Not String.Equals(bsTemplatesListView.SelectedItems(0).Text, bsTemplateTextBox.Text) Then
                    myFile = PathTemplates & "\" & bsTemplatesListView.SelectedItems(0).Text & ".REPX"
                Else
                    myFile = PathTemplates & "\" & bsTemplateTextBox.Text & ".REPX"
                End If

                SaveAs = False
            End If

            TemplateAux.LoadLayout(myFile)
            DesignForm.OpenReport(TemplateAux)

            ' Configure Formatting Toolbar
            With DesignForm.DesignBarManager.Bars(6)
                .CanDockStyle = DevExpress.XtraBars.BarCanDockStyle.Left
                .DockStyle = DevExpress.XtraBars.BarDockStyle.Top
                .Manager.AllowCustomization = False
                .Manager.AllowMoveBarOnToolbar = False
                .Manager.AllowQuickCustomization = False
                .Manager.AllowShowToolbarsPopup = False

                .OptionsBar.AllowDelete = False
                .OptionsBar.AllowQuickCustomization = False
                .OptionsBar.BarState = DevExpress.XtraBars.BarState.Expanded
                .OptionsBar.DisableClose = True
                .OptionsBar.DisableCustomization = True
                .OptionsBar.DrawDragBorder = False
            End With

            ' Multilanguage
            Dim bar As DevExpress.XtraBars.Bar = DesignForm.DesignBarManager.Bars(6)
            bar.BeginUpdate()
            bar.ItemLinks(0).Caption = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Pointer", currentLanguage)             ' Pointer
            bar.ItemLinks(1).Caption = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Label", currentLanguage)               ' Label
            bar.ItemLinks(2).Caption = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_PictureBox", currentLanguage)          ' Picture Box
            bar.ItemLinks(3).Caption = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_LIMSErrors_LineNum", currentLanguage)  ' Line
            bar.ItemLinks(4).Caption = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Shape", currentLanguage)               ' Shape
            bar.ItemLinks(5).Caption = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_PageInfo", currentLanguage)               ' Shape
            bar.EndUpdate()

            TemplateAux = Nothing

            With DesignForm
                .DesignBarManager.Bars.Manager.AllowShowToolbarsPopup = False
                '.ActiveDesignPanel.Refresh()
                .ActiveDesignPanel.FileName = myFile ' new

                ' Hide the Field List and Property Grid dock panels.
                .SetWindowVisibility(DesignDockPanelType.ErrorList Or _
                                     DesignDockPanelType.FieldList Or _
                                     DesignDockPanelType.GroupAndSort Or _
                                     DesignDockPanelType.PropertyGrid Or _
                                     DesignDockPanelType.ToolBox Or _
                                     DesignDockPanelType.ReportExplorer, False)

                ' Hide Properties Windows tool bar

                .ActiveDesignPanel.SetCommandVisibility(ReportCommand.AddNewDataSource, DevExpress.XtraReports.UserDesigner.CommandVisibility.None)
                .ActiveDesignPanel.SetCommandVisibility(ReportCommand.AlignBottom, DevExpress.XtraReports.UserDesigner.CommandVisibility.None)
                .ActiveDesignPanel.SetCommandVisibility(ReportCommand.AlignHorizontalCenters, DevExpress.XtraReports.UserDesigner.CommandVisibility.None)
                .ActiveDesignPanel.SetCommandVisibility(ReportCommand.AlignToGrid, DevExpress.XtraReports.UserDesigner.CommandVisibility.None)
                .ActiveDesignPanel.SetCommandVisibility(ReportCommand.AlignTop, DevExpress.XtraReports.UserDesigner.CommandVisibility.None)
                .ActiveDesignPanel.SetCommandVisibility(ReportCommand.AlignVerticalCenters, DevExpress.XtraReports.UserDesigner.CommandVisibility.None)
                .ActiveDesignPanel.SetCommandVisibility(ReportCommand.Exit, DevExpress.XtraReports.UserDesigner.CommandVisibility.None)
                .ActiveDesignPanel.SetCommandVisibility(ReportCommand.ShowWindowInterface, DevExpress.XtraReports.UserDesigner.CommandVisibility.None)
                .ActiveDesignPanel.SetCommandVisibility(ReportCommand.VerbEditBands, DevExpress.XtraReports.UserDesigner.CommandVisibility.None)
                .ActiveDesignPanel.SetCommandVisibility(ReportCommand.Close, DevExpress.XtraReports.UserDesigner.CommandVisibility.None)
                .ActiveDesignPanel.SetCommandVisibility(ReportCommand.Closing, DevExpress.XtraReports.UserDesigner.CommandVisibility.None)
                .ActiveDesignPanel.SetCommandVisibility(ReportCommand.PropertiesWindow, DevExpress.XtraReports.UserDesigner.CommandVisibility.None)
                .ActiveDesignPanel.SetCommandVisibility(ReportCommand.Zoom, DevExpress.XtraReports.UserDesigner.CommandVisibility.None)
                .ActiveDesignPanel.Report.PrintingSystem.SetCommandVisibility(DevExpress.XtraPrinting.PrintingSystemCommand.Customize, DevExpress.XtraReports.UserDesigner.CommandVisibility.None)
                .ActiveDesignPanel.Report.PrintingSystem.SetCommandVisibility(DevExpress.XtraPrinting.PrintingSystemCommand.File, DevExpress.XtraReports.UserDesigner.CommandVisibility.None)
                .ActiveDesignPanel.Report.PrintingSystem.SetCommandVisibility(DevExpress.XtraPrinting.PrintingSystemCommand.Open, DevExpress.XtraReports.UserDesigner.CommandVisibility.None)
                .ActiveDesignPanel.Report.PrintingSystem.SetCommandVisibility(DevExpress.XtraPrinting.PrintingSystemCommand.PageOrientation, DevExpress.XtraReports.UserDesigner.CommandVisibility.None)
                .ActiveDesignPanel.Report.PrintingSystem.SetCommandVisibility(DevExpress.XtraPrinting.PrintingSystemCommand.Parameters, DevExpress.XtraReports.UserDesigner.CommandVisibility.None)
                .ActiveDesignPanel.Report.PrintingSystem.SetCommandVisibility(DevExpress.XtraPrinting.PrintingSystemCommand.Pointer, DevExpress.XtraReports.UserDesigner.CommandVisibility.None)
                .ActiveDesignPanel.Report.PrintingSystem.SetCommandVisibility(DevExpress.XtraPrinting.PrintingSystemCommand.PrintDirect, DevExpress.XtraReports.UserDesigner.CommandVisibility.None)
                .ActiveDesignPanel.Report.PrintingSystem.SetCommandVisibility(DevExpress.XtraPrinting.PrintingSystemCommand.ViewWholePage, DevExpress.XtraReports.UserDesigner.CommandVisibility.None)
                .ActiveDesignPanel.Report.PrintingSystem.SetCommandVisibility(DevExpress.XtraPrinting.PrintingSystemCommand.Save, DevExpress.XtraReports.UserDesigner.CommandVisibility.None)
                .ActiveDesignPanel.Report.PrintingSystem.SetCommandVisibility(DevExpress.XtraPrinting.PrintingSystemCommand.Find, DevExpress.XtraReports.UserDesigner.CommandVisibility.None)
                .ActiveDesignPanel.Report.PrintingSystem.SetCommandVisibility(DevExpress.XtraPrinting.PrintingSystemCommand.Magnifier, DevExpress.XtraReports.UserDesigner.CommandVisibility.None)
                .ActiveDesignPanel.Report.PrintingSystem.SetCommandVisibility(DevExpress.XtraPrinting.PrintingSystemCommand.PageSetup, DevExpress.XtraReports.UserDesigner.CommandVisibility.None)
                .ActiveDesignPanel.Report.PrintingSystem.SetCommandVisibility(DevExpress.XtraPrinting.PrintingSystemCommand.Watermark, DevExpress.XtraReports.UserDesigner.CommandVisibility.None)
                .ActiveDesignPanel.Report.PrintingSystem.SetCommandVisibility(DevExpress.XtraPrinting.PrintingSystemCommand.Background, DevExpress.XtraReports.UserDesigner.CommandVisibility.None)
                .ActiveDesignPanel.Report.PrintingSystem.SetCommandVisibility(DevExpress.XtraPrinting.PrintingSystemCommand.ExportCsv, DevExpress.XtraReports.UserDesigner.CommandVisibility.None)
                .ActiveDesignPanel.Report.PrintingSystem.SetCommandVisibility(DevExpress.XtraPrinting.PrintingSystemCommand.ExportFile, DevExpress.XtraReports.UserDesigner.CommandVisibility.None)
                .ActiveDesignPanel.Report.PrintingSystem.SetCommandVisibility(DevExpress.XtraPrinting.PrintingSystemCommand.SendFile, DevExpress.XtraReports.UserDesigner.CommandVisibility.None)
                .ActiveDesignPanel.Report.PrintingSystem.SetCommandVisibility(DevExpress.XtraPrinting.PrintingSystemCommand.Background, DevExpress.XtraPrinting.CommandVisibility.None)
                .ActiveDesignPanel.Report.PrintingSystem.SetCommandVisibility(DevExpress.XtraPrinting.PrintingSystemCommand.DocumentMap, DevExpress.XtraPrinting.CommandVisibility.None)
                .ActiveDesignPanel.Report.PrintingSystem.SetCommandVisibility(DevExpress.XtraPrinting.PrintingSystemCommand.EditPageHF, DevExpress.XtraPrinting.CommandVisibility.None)
                .ActiveDesignPanel.Report.PrintingSystem.SetCommandVisibility(DevExpress.XtraPrinting.PrintingSystemCommand.File, DevExpress.XtraPrinting.CommandVisibility.None)
                .ActiveDesignPanel.Report.PrintingSystem.SetCommandVisibility(DevExpress.XtraPrinting.PrintingSystemCommand.PageLayout, DevExpress.XtraPrinting.CommandVisibility.None)
                .ActiveDesignPanel.Report.PrintingSystem.SetCommandVisibility(DevExpress.XtraPrinting.PrintingSystemCommand.StopPageBuilding, DevExpress.XtraPrinting.CommandVisibility.None)
                .ActiveDesignPanel.Report.PrintingSystem.SetCommandVisibility(DevExpress.XtraPrinting.PrintingSystemCommand.SubmitParameters, DevExpress.XtraPrinting.CommandVisibility.None)
                .ActiveDesignPanel.Report.PrintingSystem.SetCommandVisibility(DevExpress.XtraPrinting.PrintingSystemCommand.View, DevExpress.XtraPrinting.CommandVisibility.None)
                .ActiveDesignPanel.Report.PrintingSystem.SetCommandVisibility(DevExpress.XtraPrinting.PrintingSystemCommand.ViewWholePage, DevExpress.XtraPrinting.CommandVisibility.None)
                .ActiveDesignPanel.Report.PrintingSystem.SetCommandVisibility(DevExpress.XtraPrinting.PrintingSystemCommand.Scale, DevExpress.XtraPrinting.CommandVisibility.None)
                .ActiveDesignPanel.Report.PrintingSystem.SetCommandVisibility(DevExpress.XtraPrinting.PrintingSystemCommand.Print, DevExpress.XtraPrinting.CommandVisibility.None)
                .ActiveDesignPanel.Report.PrintingSystem.SetCommandVisibility(DevExpress.XtraPrinting.PrintingSystemCommand.HandTool, DevExpress.XtraPrinting.CommandVisibility.None)
                .ActiveDesignPanel.Report.PrintingSystem.SetCommandVisibility(DevExpress.XtraPrinting.PrintingSystemCommand.ZoomOut, DevExpress.XtraPrinting.CommandVisibility.None)
                .ActiveDesignPanel.Report.PrintingSystem.SetCommandVisibility(DevExpress.XtraPrinting.PrintingSystemCommand.Zoom, DevExpress.XtraPrinting.CommandVisibility.None)
                .ActiveDesignPanel.Report.PrintingSystem.SetCommandVisibility(DevExpress.XtraPrinting.PrintingSystemCommand.ZoomIn, DevExpress.XtraPrinting.CommandVisibility.None)
                .ActiveDesignPanel.Report.PrintingSystem.SetCommandVisibility(DevExpress.XtraPrinting.PrintingSystemCommand.ClosePreview, DevExpress.XtraPrinting.CommandVisibility.None)
                .ActiveDesignPanel.Report.PrintingSystem.SetCommandVisibility(DevExpress.XtraPrinting.PrintingSystemCommand.Background, DevExpress.XtraPrinting.CommandVisibility.None)
                .ActiveDesignPanel.Report.PrintingSystem.SetCommandVisibility(DevExpress.XtraPrinting.PrintingSystemCommand.DocumentMap, DevExpress.XtraPrinting.CommandVisibility.None)
                .ActiveDesignPanel.Report.PrintingSystem.SetCommandVisibility(DevExpress.XtraPrinting.PrintingSystemCommand.MultiplePages, DevExpress.XtraPrinting.CommandVisibility.None)
                .ActiveDesignPanel.Report.PrintingSystem.SetCommandVisibility(DevExpress.XtraPrinting.PrintingSystemCommand.FillBackground, DevExpress.XtraPrinting.CommandVisibility.None)
                .ActiveDesignPanel.Report.PrintingSystem.SetCommandVisibility(DevExpress.XtraPrinting.PrintingSystemCommand.Magnifier, DevExpress.XtraPrinting.CommandVisibility.None)
                .ActiveDesignPanel.Report.PrintingSystem.SetCommandVisibility(DevExpress.XtraPrinting.PrintingSystemCommand.PageSetup, DevExpress.XtraPrinting.CommandVisibility.None)
                .ActiveDesignPanel.Report.PrintingSystem.SetCommandVisibility(DevExpress.XtraPrinting.PrintingSystemCommand.PrintDirect, DevExpress.XtraPrinting.CommandVisibility.None)
                .ActiveDesignPanel.Report.PrintingSystem.SetCommandVisibility(DevExpress.XtraPrinting.PrintingSystemCommand.ShowFirstPage, DevExpress.XtraPrinting.CommandVisibility.None)
                .ActiveDesignPanel.Report.PrintingSystem.SetCommandVisibility(DevExpress.XtraPrinting.PrintingSystemCommand.ShowLastPage, DevExpress.XtraPrinting.CommandVisibility.None)
                .ActiveDesignPanel.Report.PrintingSystem.SetCommandVisibility(DevExpress.XtraPrinting.PrintingSystemCommand.ShowNextPage, DevExpress.XtraPrinting.CommandVisibility.None)
                .ActiveDesignPanel.Report.PrintingSystem.SetCommandVisibility(DevExpress.XtraPrinting.PrintingSystemCommand.ShowPrevPage, DevExpress.XtraPrinting.CommandVisibility.None)

                .AllowDrop = True
                .AutoScaleMode = Windows.Forms.AutoScaleMode.Font
                .KeyPreview = True
                .FormBorderStyle = Windows.Forms.FormBorderStyle.FixedDialog
                .Size = New Size(Me.ParentForm.Width, Me.ParentForm.Height)

                Dim startPoint As Point
                startPoint = Me.ParentForm.PointToScreen(New Point(Me.ParentForm.Left, Me.Parent.Top))
                'DesignForm.Top = Me.ParentForm.Top '  startPoint.X
                'DesignForm.Left = Me.ParentForm.Location  'startPoint.Y
                'DesignForm.Location = Me.ParentForm.Location
                .PointToClient(startPoint)
                '.Location = Me.ParentForm.Location

                .MinimizeBox = False
                .MaximizeBox = False

                '.ActiveDesignPanel.Report.CreateDocument() ''
            End With

            Dim printTool As New ReportPrintTool(DesignForm.ActiveDesignPanel.Report)
            'printTool.PreviewForm.MdiParent =DesignForm.ActiveDesignPanel.Report.M  SystemParameter.MdiForm
            printTool.PreviewForm.PrintBarManager.MainMenu.Visible = False
            DirectCast(DirectCast(printTool.PreviewForm.PrintBarManager, DevExpress.XtraPrinting.Preview.PrintBarManager).PreviewBar, DevExpress.XtraBars.Bar).Visible = False

            DesignForm.Text = bsTemplateTextBox.Text
            DesignForm.ActiveDesignPanel.Report.Name = DesignForm.Text
            DesignForm.ActiveDesignPanel.Report.DisplayName = DesignForm.Text
            DesignForm.ActiveDesignPanel.Report.PaperKind = System.Drawing.Printing.PaperKind.A4

            'AddHandler DesignForm.DesignDockManager.ActivePanelChanged, AddressOf DesignDockManager_ActivePanelChanged

            'DesignForm.DesignDockManager

            ' Invoke the design form.
            DesignForm.ShowDialog()

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".OpenReport", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".OpenReport", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")

        Finally
            Cursor = Cursors.Default

        End Try
    End Sub


    'Private Sub DesignDockManager_ActivePanelChanged(ByVal sender As Object, ByVal e As DevExpress.XtraBars.Docking.ActivePanelChangedEventArgs)
    '   If e.Panel IsNot Nothing AndAlso TypeOf e.Panel Is FieldListDockPanel Then
    'DirectCast(e.Panel, FieldListDockPanel).ShowNodeToolTips = False
    '  End If
    'End Sub

    Private Sub AddImage()
        Try
            'Add image empty
            Dim myPath As String = GlobalBase.AppPath & GlobalBase.ReportPath & "\"
            Dim mySize As Size

            tmpPreviewPictureBox = New PictureBox()

            File.Delete(PathTemplates & "\TEMP.REPX")
            File.Delete(PathTemplates & "\TEMP.GIF")

            If String.Equals(bsOrientationComboBox.Text, labelPortrait) Then
                tmpPreviewPictureBox.ImageLocation = myPath & "MASTERTEMPLATE_Empty.gif"
                mySize = New Size(350, 450)
                File.Copy(PathTemplates & "\MASTERTEMPLATE.REPX", PathTemplates & "\TEMP.REPX")
                File.Copy(PathTemplates & "\MASTERTEMPLATE.GIF", PathTemplates & "\TEMP.GIF")
            Else
                tmpPreviewPictureBox.ImageLocation = myPath & "MASTERTEMPLATELS_Empty.gif"
                mySize = New Size(580, 400)
                File.Copy(PathTemplates & "\MASTERTEMPLATELS.REPX", PathTemplates & "\TEMP.REPX")
                File.Copy(PathTemplates & "\MASTERTEMPLATELS.GIF", PathTemplates & "\TEMP.GIF")
            End If

            tmpPreviewPictureBox.Dock = DockStyle.Fill
            tmpPreviewPictureBox.SizeMode = PictureBoxSizeMode.StretchImage

            For i As Integer = (bsPicturePanel.Controls.Count - 1) To 0 Step -1
                Dim ctrl As Control = bsPicturePanel.Controls(i)
                bsPicturePanel.Controls.Remove(ctrl)
                ctrl.Dispose()
            Next i

            bsPicturePanel.Controls.Add(tmpPreviewPictureBox)

            bsPicturePanel.Size = mySize
            bsPicturePanel.Refresh()

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".OpenReport", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".OpenReport", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Add new template based in master template (SAVE master template AS new template)
    ''' </summary>
    ''' <remarks>
    ''' Created by: DL 23/11/2011
    ''' AG 12/06/2014 #1661 - Control discard pending changes if clicks NEW button during edition mode
    ''' </remarks>
    Private Sub bsNewButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles bsNewButton.Click
        Try
            'AG 12/06/2014 #1661
            Dim setScreenToCreate As Boolean = True
            If EditionMode AndAlso (ChangesMade) Then 'Or ReportDesignxUserControl.ReportChanged) Then

                If (ShowMessage(Me.Name, GlobalEnumerates.Messages.DISCARD_PENDING_CHANGES.ToString) = Windows.Forms.DialogResult.Yes) Then
                    setScreenToCreate = True
                End If
            End If
            'AG 12/06/2014 #1661
            If setScreenToCreate Then
                AddImage()

                ' Refresh buttons status
                bsNewButton.Enabled = False
                bsEditButton.Enabled = False
                bsDeleteButton.Enabled = False
                bsSaveButton.Enabled = False
                bsEditReport.Enabled = False
                bsCancelButton.Enabled = True
                bsOrientationComboBox.Enabled = True
                '
                bsTemplatesListView.Enabled = False
                '
                EditionMode = True 'RH 15/12/2011
                ChangesMade = True 'RH 15/12/2011

                selectedTemplateName = ""
                newTemplate = True

                RemoveHandler bsTemplateTextBox.TextChanged, AddressOf bsTemplateTextBox_TextChanged
                RemoveHandler bsOrientationComboBox.TextChanged, AddressOf bsOrientationComboBox_TextChanged

                bsTemplateTextBox.Text = String.Empty
                bsOrientationComboBox.Text = ""

                bsTemplateTextBox.Enabled = True
                bsTemplateTextBox.BackColor = Color.White
                bsOrientationComboBox.Enabled = True
                bsOrientationComboBox.BackColor = Color.White
                bsDefaultCheckbox.Enabled = True
                bsDefaultCheckbox.Checked = False

                bsTemplateTextBox.BackColor = Color.Khaki

                AddHandler bsTemplateTextBox.TextChanged, AddressOf bsTemplateTextBox_TextChanged
                AddHandler bsOrientationComboBox.TextChanged, AddressOf bsOrientationComboBox_TextChanged

                '            AuxTemplateName = "" 'DL 09/02/2012

                bsTemplatesListView.SelectedItems.Clear()
                bsTemplateTextBox.Focus()
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsNewButton_Click", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsNewButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' default check box change
    ''' </summary>
    ''' <remarks>
    ''' Created by: DL 23/11/2011
    ''' </remarks>
    Private Sub bsDefaultCheckbox_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles bsDefaultCheckbox.CheckedChanged
        Try
            If Not goRef Then
                If bsTemplatesListView.SelectedIndices.Count > 0 AndAlso Not CBool(bsTemplatesListView.Items(bsTemplatesListView.SelectedIndices(0)).SubItems(1).Text) Then
                    If CBool(bsTemplatesListView.Items(bsTemplatesListView.SelectedIndices(0)).SubItems(4).Text) <> bsDefaultCheckbox.Checked Then
                        ChangesMade = True
                    End If
                End If
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & "bsDefaultCheckbox_CheckedChanged", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & "bsDefaultCheckbox_CheckedChanged", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' text box change
    ''' </summary>
    ''' <remarks>
    ''' Created by: DL 23/11/2011
    ''' </remarks>
    Private Sub bsTemplateTextBox_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles bsTemplateTextBox.TextChanged
        Try
            If String.Equals(bsTemplateTextBox.Text.ToString.Trim, String.Empty) Then
                bsTemplateTextBox.BackColor = Color.Khaki
                bsEditReport.Enabled = False
                bsSaveButton.Enabled = False

            Else
                bsTemplateTextBox.BackColor = Color.White
                bsEditReport.Enabled = True
                bsSaveButton.Enabled = True

            End If

            If bsTemplatesListView.SelectedIndices.Count > 0 Then
                If CBool(bsTemplatesListView.Items(bsTemplatesListView.SelectedIndices(0)).SubItems(1).Text) Then

                Else
                    If Not String.Equals(bsTemplatesListView.Items(bsTemplatesListView.SelectedIndices(0)).Text, bsTemplateTextBox.Text) Then
                        ChangesMade = True
                    End If
                End If
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & "bsTemplateTextBox_TextChanged", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & "bsTemplateTextBox_TextChanged", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try

    End Sub

    ''' <summary>
    ''' orientation change
    ''' </summary>
    ''' <remarks>
    ''' Created by: DL 23/11/2011
    ''' </remarks>
    Private Sub bsOrientationComboBox_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles bsOrientationComboBox.TextChanged

        Try
            If bsTemplatesListView.SelectedIndices.Count > 0 Then
                If Not CBool(bsTemplatesListView.Items(bsTemplatesListView.SelectedIndices(0)).SubItems(1).Text) Then
                    If Not String.Equals(bsTemplatesListView.Items(bsTemplatesListView.SelectedIndices(0)).SubItems(2).Text, bsOrientationComboBox.Text) Then
                        ChangesMade = True
                    End If

                End If


            Else
                ' is new
                If newTemplate Then

                    Dim myPath As String = GlobalBase.AppPath & GlobalBase.ReportPath & "\"
                    Dim mySize As Size

                    File.Delete(PathTemplates & "\TEMP.REPX")
                    File.Delete(PathTemplates & "\TEMP.GIF")

                    If String.Equals(bsOrientationComboBox.Text, labelPortrait) Then
                        tmpPreviewPictureBox.ImageLocation = myPath & "MASTERTEMPLATE_Empty.gif"
                        mySize = New Size(350, 450)
                        File.Copy(PathTemplates & "\MASTERTEMPLATE.REPX", PathTemplates & "\TEMP.REPX")
                        File.Copy(PathTemplates & "\MASTERTEMPLATE.GIF", PathTemplates & "\TEMP.GIF")
                    Else
                        tmpPreviewPictureBox.ImageLocation = myPath & "MASTERTEMPLATELS_Empty.gif"
                        mySize = New Size(580, 400)
                        File.Copy(PathTemplates & "\MASTERTEMPLATELS.REPX", PathTemplates & "\TEMP.REPX")
                        File.Copy(PathTemplates & "\MASTERTEMPLATELS.GIF", PathTemplates & "\TEMP.GIF")
                    End If

                    tmpPreviewPictureBox = New PictureBox()
                    tmpPreviewPictureBox.ImageLocation = myPath & selectedTemplateName & ".gif"
                    tmpPreviewPictureBox.Dock = DockStyle.Fill
                    tmpPreviewPictureBox.SizeMode = PictureBoxSizeMode.StretchImage

                    For i As Integer = (bsPicturePanel.Controls.Count - 1) To 0 Step -1
                        Dim ctrl As Control = bsPicturePanel.Controls(i)
                        bsPicturePanel.Controls.Remove(ctrl)
                        ctrl.Dispose()
                    Next i

                    bsPicturePanel.Controls.Add(tmpPreviewPictureBox)
                    bsPicturePanel.Size = mySize
                End If
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & "bsOrientationComboBox_TextChanged", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & "bsOrientationComboBox_TextChanged", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try

    End Sub


    ''' <summary>
    ''' Undo changes
    ''' </summary>
    ''' <remarks>
    ''' Created by: DL 23/11/2011
    ''' </remarks>
    Private Sub bsCancelButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsCancelButton.Click

        Try
            Dim setScreenToQuery As Boolean = False

            If EditionMode AndAlso ChangesMade Then ' Or ReportDesignxUserControl.ReportChanged) Then

                If (ShowMessage(Me.Name, GlobalEnumerates.Messages.DISCARD_PENDING_CHANGES.ToString) = Windows.Forms.DialogResult.Yes) Then
                    setScreenToQuery = True
                End If

            Else
                setScreenToQuery = True
            End If

            If setScreenToQuery Then
                bsTemplatesListView.SelectedItems.Clear()
                bsScreenErrorProvider.Clear()  'DL 31/07/2012

                SelectTemplate(0)

                bsNewButton.Enabled = True
                bsEditButton.Enabled = True
                bsDeleteButton.Enabled = True
                bsTemplatesListView.Enabled = True

                bsCancelButton.Enabled = False
                bsSaveButton.Enabled = False
                bsEditReport.Enabled = False

                EditionMode = False
            End If

            DeleteResidualFiles()

            bsNewButton.Select()

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & "bsCancelButton_Click", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & "bsCancelButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try

    End Sub

    ''' <summary>
    ''' Edit template
    ''' </summary>
    ''' <remarks>
    ''' Created by: DL 23/11/2011
    ''' </remarks>
    Private Sub bsEditButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles bsEditButton.Click

        Try
            EditTemplate()

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsEditButton_Click", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsEditButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Show data of the selected templates in READ-ONLY MODE
    ''' </summary>
    ''' <remarks>
    ''' Created by: DL 22/11/2011
    ''' </remarks>
    Private Sub bsTemplatesListView_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles bsTemplatesListView.Click
        Dim setScreenToQuery As Boolean = False

        Try
            Me.Cursor = Cursors.WaitCursor

            'If bsTemplatesListView.SelectedItems(0).Index <> originalSelectedIndex Then
            If bsTemplatesListView.SelectedItems.Count = 1 Then
                If EditionMode AndAlso (ChangesMade) Then 'Or ReportDesignxUserControl.ReportChanged) Then

                    If (ShowMessage(Me.Name, GlobalEnumerates.Messages.DISCARD_PENDING_CHANGES.ToString) = Windows.Forms.DialogResult.Yes) Then
                        setScreenToQuery = True
                    Else
                        If (originalSelectedIndex <> -1) Then
                            bsTemplatesListView.SelectedItems.Clear()

                            'Return focus to the TEMPLATE that has been edited
                            bsTemplatesListView.Items(originalSelectedIndex).Selected = True
                            bsTemplatesListView.Select()
                        End If
                    End If

                Else
                    setScreenToQuery = True
                End If


            ElseIf bsTemplatesListView.SelectedItems.Count > 1 Then
                bsTemplatesListView.SelectedItems.Clear()

                'Return focus to the TEMPLATE that has been edited
                bsTemplatesListView.Items(originalSelectedIndex).Selected = True
                bsTemplatesListView.Select()
            End If

            If setScreenToQuery Then
                EditionMode = False
                If (bsTemplatesListView.SelectedItems.Count = 1) Then
                    'If there is only a selected Control, show its data in details area
                    originalSelectedIndex = bsTemplatesListView.SelectedIndices(0)
                    SelectTemplate(bsTemplatesListView.SelectedIndices(0))

                Else
                    'If there are several Controls selected, verify if at least one of the selected Controls is InUse
                    Dim bEnabled As Boolean = True
                    For Each mySelectedItem As ListViewItem In bsTemplatesListView.SelectedItems
                        'If there is an item InUse
                        If (CBool(mySelectedItem.SubItems(7).Text)) Then
                            bEnabled = False
                            Exit For
                        End If
                    Next mySelectedItem

                    'Set screen status to the case when there are several selected Controls
                    'InitialModeScreenStatus(False)

                    bsEditButton.Enabled = False
                    bsDeleteButton.Enabled = bEnabled

                End If

                'Else

                '   MsgBox("")
            End If
            'Else
            'MsgBox("")
            'End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsTemplatesListView_Click", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsTemplatesListView_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")

        Finally
            Me.Cursor = Cursors.Default

        End Try
    End Sub

    ''' <summary>
    ''' When the screen is in ADD or EDIT Mode and the ESC Key is pressed, code of Cancel Button is executed;
    ''' in other case, the screen is closed when ESC Key is pressed
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 29/11/2011
    ''' </remarks>
    Private Sub IBandTemplateReport_KeyDown(ByVal sender As Object, ByVal e As KeyEventArgs) Handles Me.KeyDown
        Try
            If (e.KeyCode = Keys.Escape) Then
                If (bsCancelButton.Enabled) Then
                    'Escape key should do exactly the same operations as bsCancelButton_Click()
                    bsCancelButton.PerformClick()
                Else
                    'Escape key should do exactly the same operations as bsExitButton_Click()
                    bsExitButton.PerformClick()
                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".IBandTemplateReport_KeyDown", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".IBandTemplateReport_KeyDown", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Form initialization when loading: set the screen status to INITIAL MODE
    ''' </summary>
    ''' <remarks>
    ''' Created  by: DL 22/11/2011 
    ''' </remarks>
    Private Sub IBandTemplateReport_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            ScreenLoad()
            '            bsNewButton.Focus()
            bsNewButton.Select()

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ProgControls_Load", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ProgControls_Load", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    Private Sub bsTemplatesListView_DoubleClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles bsTemplatesListView.DoubleClick
        EditTemplate()
    End Sub

    ''' <summary>
    ''' Allow consultation of elements in the templates list using the keyboard, and also deletion using SUPR key
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 29/11/2011
    ''' </remarks>
    Private Sub bsTemplatesListView_KeyUp(ByVal sender As Object, ByVal e As KeyEventArgs) Handles bsTemplatesListView.KeyUp

        Try
            If (bsTemplatesListView.SelectedItems.Count = 1) Then
                SelectTemplate(bsTemplatesListView.SelectedIndices(0))
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsTemplatesListView_KeyUp", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsTemplatesListView_KeyUp", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    Private Sub bsTemplatesListView_PreviewKeyDown(ByVal sender As Object, ByVal e As PreviewKeyDownEventArgs) Handles bsTemplatesListView.PreviewKeyDown

        Try
            If e.KeyCode = Keys.Delete Then 'And Not EditionMode Then
                If bsDeleteButton.Enabled = True Then DeleteTemplate()
            ElseIf e.KeyCode = Keys.Enter Then 'And Not EditionMode Then
                EditTemplate()
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsTemplatesListView_PreviewKeyDown", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsTemplatesListView_PreviewKeyDown", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try

    End Sub

#End Region

#Region "Private Methods"

    ''' <summary>
    ''' Load all data needed for the screen
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 22/11/2011 
    ''' </remarks>
    Private Sub ScreenLoad()
        Try
            Cursor = Cursors.WaitCursor 'RH 15/12/2011

            'Get the current Language from the current Application Session
            Dim currentLanguageGlobal As New GlobalBase
            currentLanguage = currentLanguageGlobal.GetSessionInfo().ApplicationLanguage.Trim.ToString

            'Load the multilanguage texts for all Screen Labels
            GetScreenLabels()

            'Get Icons for graphical buttons
            PrepareButtons()

            'Load the list of available Sample Types
            LoadOrientation()

            'Configure and load the list of existing Controls
            InitializeTemplateList()

            If (bsTemplatesListView.Items.Count > 0) Then
                'Select the first template in the list
                bsTemplatesListView.Items(0).Selected = True

                DeleteResidualFiles()
                If bsTemplatesListView.SelectedIndices.Count = 1 Then
                    SelectTemplate(bsTemplatesListView.SelectedIndices(0))
                End If
            End If

            ResetBorder() 'RH 30/03/2012

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ScreenLoad", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ScreenLoad", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")

        Finally
            Cursor = Cursors.Default 'RH 15/12/2011

        End Try
    End Sub


    ''' <summary>
    ''' Configure and initialize the ListView of Templates
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 22/11/2011
    ''' </remarks>
    Private Sub InitializeTemplateList()
        Try
            'Initialization of template List
            bsTemplatesListView.Items.Clear()

            bsTemplatesListView.Alignment = ListViewAlignment.Left
            bsTemplatesListView.FullRowSelect = True
            bsTemplatesListView.MultiSelect = True
            bsTemplatesListView.Scrollable = True
            bsTemplatesListView.View = View.Details
            bsTemplatesListView.HideSelection = False
            bsTemplatesListView.HeaderStyle = ColumnHeaderStyle.None

            'List columns definition  --> only column containing the template Name will be visible
            bsTemplatesListView.Columns.Add("TemplateName", 177, HorizontalAlignment.Left)
            bsTemplatesListView.Columns.Add("MasterTemplate", 0, HorizontalAlignment.Left)
            bsTemplatesListView.Columns.Add("TemplateOrientation", 0, HorizontalAlignment.Left)
            bsTemplatesListView.Columns.Add("TemplateFileName", 0, HorizontalAlignment.Left)
            bsTemplatesListView.Columns.Add("DefaultTemplate", 0, HorizontalAlignment.Left)

            'Fill ListView with the list of existing Templates
            LoadTemplatesList()

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".InitializeTemplateList", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".InitializeTemplateList", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub



    ''' <summary>
    ''' Delete all residual file
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 09/02/2012
    ''' </remarks>
    Private Sub DeleteResidualFiles()
        Try
            'Get the list of existing Templates
            Dim resultData As New GlobalDataTO
            Dim templateList As New ReportTemplatesDelegate

            resultData = templateList.ReadAll(Nothing)
            If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                Dim myTemplateDS As ReportTemplatesDS = DirectCast(resultData.SetDatos, ReportTemplatesDS)

                Dim di As New IO.DirectoryInfo(PathTemplates)
                Dim aryFi As IO.FileInfo() = di.GetFiles("*.*")
                Dim fi As IO.FileInfo
                Dim mytemplateFileName As List(Of String)
                Dim myfilewithoutextension As String


                For Each fi In aryFi
                    myfilewithoutextension = fi.Name.Split(".")(0).ToString '& ".repx"
                    'myLength = Len(myfilewithoutextension)

                    If Not String.Equals(myfilewithoutextension, "MASTERTEMPLATE") AndAlso _
                       Not String.Equals(myfilewithoutextension, "MASTERTEMPLATELS") AndAlso _
                       Not String.Equals(myfilewithoutextension, "MASTERTEMPLATELS_Empty") AndAlso _
                       Not String.Equals(myfilewithoutextension, "MASTERTEMPLATE_Empty") Then

                        myfilewithoutextension &= ".REPX"

                        mytemplateFileName = (From row In myTemplateDS.tcfgReportTemplates _
                                              Where String.Equals(row.TemplateFileName, myfilewithoutextension) _
                                              Select row.TemplateFileName).ToList()

                        If mytemplateFileName.Count < 1 Then File.Delete(PathTemplates & "\" & fi.Name)
                    End If

                Next fi
                'end clear temps
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".DeleteResidualFiles ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".DeleteResidualFiles", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Load the ListView of templates with the list of existing ones
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 22/11/2011
    ''' </remarks>
    Private Sub LoadTemplatesList()
        Try
            bsTemplatesListView.Items.Clear()

            'Get the list of existing Templates
            Dim resultData As GlobalDataTO
            Dim templateList As New ReportTemplatesDelegate

            resultData = templateList.ReadAll(Nothing)

            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                Dim myTemplateDS As ReportTemplatesDS = DirectCast(resultData.SetDatos, ReportTemplatesDS)

                'Sort the returned templates
                Dim qTemplates As List(Of ReportTemplatesDS.tcfgReportTemplatesRow)
                qTemplates = (From row In myTemplateDS.tcfgReportTemplates _
                              Select row Order By row.MasterTemplate Descending, row.TemplateName).ToList()

                'Fill the List View with the list of sorted templates by master template
                Dim i As Integer = 0
                Dim rowToSelect As Integer = -1
                Dim myTestName As String = ""

                For Each templateRow As ReportTemplatesDS.tcfgReportTemplatesRow In qTemplates

                    Select Case templateRow.TemplateName

                        'For the default BioSystems MASTERTEMPLATEs change name for 'ReportTemplate (orientation)'
                        Case "MASTERTEMPLATE"
                            myTestName = myTemplatePlug & " (" & myPortraitPlug & ")"

                        Case "MASTERTEMPLATELS"
                            myTestName = myTemplatePlug & " (" & myLandscapePlug & ")"

                        Case Else
                            myTestName = templateRow.TemplateName

                    End Select
                    bsTemplatesListView.Items.Add(myTestName)
                    bsTemplatesListView.Items(i).SubItems.Add(templateRow.MasterTemplate.ToString)
                    bsTemplatesListView.Items(i).SubItems.Add(templateRow.TemplateOrientation)
                    bsTemplatesListView.Items(i).SubItems.Add(templateRow.TemplateFileName)
                    bsTemplatesListView.Items(i).SubItems.Add(templateRow.DefaultTemplate.ToString)
                    bsTemplatesListView.Items(i).SubItems.Add(templateRow.TemplateName) 'AG 11/06/2014 #1661 (the preloaded MasterTemplates change their names in screen, so we need to save the name into database in order to recover them as default is user wants) - subItems(5)

                    'If there is a selected template and it is still in the list, its position 
                    'is stored to re-select the same template once the list is loaded
                    If String.Equals(selectedTemplateName, templateRow.TemplateName) Then rowToSelect = i
                    i += 1
                Next templateRow

                selectedTemplateName = ""
                originalSelectedIndex = -1

            End If

            'An error has happened getting data from the Database
            If (resultData.HasError) Then
                ShowMessage(Me.Name & ".LoadTemplatesList", resultData.ErrorCode, resultData.ErrorMessage)

                selectedTemplateName = ""
                originalSelectedIndex = -1

            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".LoadTemplatesList ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".LoadTemplatesList", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' load orientation list
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 22/11/2011
    ''' </remarks>
    Private Sub LoadOrientation()
        Try
            'Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate
            bsOrientationComboBox.Items.Clear()
            bsOrientationComboBox.Items.Add(labelPortrait) ' PORTRAIT
            bsOrientationComboBox.Items.Add(labelLandScape)

            'For Labels, CheckBox, RadioButtons.....
            'bsControlsListLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "TITLE_Controls_List", currentLanguage)
            'bsControlDefLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "TITLE_Controls_Definition", currentLanguage)

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".GetScreenLabels", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".GetScreenLabels", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Get texts in the current application language for all screen controls
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 22/11/2011
    ''' </remarks>
    Private Sub GetScreenLabels()
        Try
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            bsNameLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Name", currentLanguage) + ":"
            bsOrientationLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Orientation", currentLanguage) + ":"
            bsDefaultCheckbox.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_DefaultTemplate", currentLanguage)
            bsTemplatesLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "TITLE_Template", currentLanguage)
            bsDetailsLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "TITLE_Details", currentLanguage)
            labelPortrait = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Summary_Vertical", currentLanguage)
            labelLandScape = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Summary_Horizontal", currentLanguage)

            bsScreenToolTips.SetToolTip(bsNewButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_AddNew", currentLanguage))
            bsScreenToolTips.SetToolTip(bsCancelButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Cancel", currentLanguage))
            bsScreenToolTips.SetToolTip(bsExitButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_CloseScreen", currentLanguage))
            bsScreenToolTips.SetToolTip(bsDeleteButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Delete", currentLanguage))
            bsScreenToolTips.SetToolTip(bsEditButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Edit", currentLanguage))
            bsScreenToolTips.SetToolTip(bsSaveButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Save", currentLanguage))

            myTemplatePlug = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_MASTER_TEMPLATE", currentLanguage)

            myPortraitPlug = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_PORTRAIT", currentLanguage)
            myLandscapePlug = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_LANDSCAPE", currentLanguage)

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".GetScreenLabels", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".GetScreenLabels", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub


    ''' <summary>
    ''' Method incharge of loading the image for each button
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 22/11/2011
    ''' </remarks>
    Private Sub PrepareButtons()
        Try
            Dim iconPath As String = MyBase.IconsPath
            Dim auxIconName As String = ""
            'Dim preloadedDataConfig As New PreloadedMasterDataDelegate

            'NEW Button
            auxIconName = GetIconName("ADD")
            If Not String.Equals(auxIconName, String.Empty) Then bsNewButton.Image = Image.FromFile(iconPath & auxIconName)

            'EDIT REPORT Button
            auxIconName = GetIconName("EDITREPORT")
            If Not String.Equals(auxIconName, String.Empty) Then bsEditReport.Image = Image.FromFile(iconPath & auxIconName)

            'EDIT Button
            auxIconName = GetIconName("EDIT")
            If Not String.Equals(auxIconName, String.Empty) Then bsEditButton.Image = Image.FromFile(iconPath & auxIconName)

            'DELETE Buttons
            auxIconName = GetIconName("REMOVE")
            If Not String.Equals(auxIconName, String.Empty) Then bsDeleteButton.Image = Image.FromFile(iconPath & auxIconName)

            'SAVE Button
            auxIconName = GetIconName("SAVE")
            If Not String.Equals(auxIconName, String.Empty) Then bsSaveButton.Image = Image.FromFile(iconPath & auxIconName)

            'CANCEL Button
            auxIconName = GetIconName("UNDO")
            If Not String.Equals(auxIconName, String.Empty) Then bsCancelButton.Image = Image.FromFile(iconPath & auxIconName)

            'CLOSE Button
            auxIconName = GetIconName("CANCEL")
            If Not String.Equals(auxIconName, String.Empty) Then bsExitButton.Image = Image.FromFile(iconPath & auxIconName)

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".PrepareButtons", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PrepareButtons", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub


    ''' <summary>
    ''' Delete template
    ''' </summary>
    ''' <remarks>
    ''' ??? - created
    ''' AG 12/06/2014 #1661 there is always 1 default template for each orientation
    ''' </remarks>
    Private Sub DeleteTemplate()
        Try
            If bsTemplatesListView.SelectedIndices.Count > 0 Then
                If (ShowMessage(Me.Name, GlobalEnumerates.Messages.DELETE_CONFIRMATION.ToString) = Windows.Forms.DialogResult.Yes) Then
                    'Dim resultData As New GlobalDataTO
                    Dim resultData As GlobalDataTO
                    Dim templateList As New ReportTemplatesDelegate
                    Dim iRow As Integer = bsTemplatesListView.SelectedIndices(0)

                    'AG 12/06/2014 - #1661 check if user wants delete the defaulttemplate .. in this case mark the mastertemplate as new default before delete
                    'resultData = templateList.Delete(Nothing, bsTemplatesListView.Items(iRow).Text)
                    Dim newDefault As Boolean = False
                    If bsTemplatesListView.Items(bsTemplatesListView.SelectedIndices(0)).SubItems(4).Text Then
                        'Get the current orientation
                        Dim myOrientation As String = bsTemplatesListView.Items(bsTemplatesListView.SelectedIndices(0)).SubItems(2).Text
                        resultData = templateList.SetDefaultTemplateStatus(Nothing, True, True, myOrientation)
                        newDefault = True
                    End If

                    resultData = templateList.Delete(Nothing, bsTemplatesListView.Items(iRow).Text)

                    If (Not resultData.HasError) Then
                        Dim fileImage As String = PathTemplates & "\" & bsTemplatesListView.Items(iRow).SubItems(3).Text

                        File.Delete(fileImage.Split(".")(0) & ".REPX")
                        File.Delete(fileImage.Split(".")(0) & ".GIF")

                        bsTemplatesListView.Items.Remove(bsTemplatesListView.Items(iRow))

                        'AG 12/06/2014 - #1661 - If new default has been programmed load the template list
                        If newDefault Then
                            LoadTemplatesList()
                        End If
                        'AG 12/06/2014 - #1661

                        If bsTemplatesListView.Items.Count > -1 Then
                            bsTemplatesListView.SelectedItems.Clear()
                            SelectTemplate(0)
                            bsTemplatesListView.Items(0).Selected = True
                            bsNewButton.Select()
                        End If

                    End If

                End If
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".DeleteTemplate", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".DeleteTemplate", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

#End Region

End Class

