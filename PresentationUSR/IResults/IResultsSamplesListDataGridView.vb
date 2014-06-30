Option Explicit On
'Option Strict On

Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.BL.Framework
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Global.GlobalEnumerates
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.Controls.UserControls
Imports Biosystems.Ax00.Calculations 'AG 26/07/2010
Imports Biosystems.Ax00.CommunicationsSwFw

Imports System.Text
Imports System.ComponentModel
Imports DevExpress.XtraReports.UI
Imports DevExpress.XtraPrinting
Imports DevExpress.XtraPrintingLinks
Imports DevExpress.XtraEditors
Imports DevExpress.XtraGrid
Imports DevExpress.XtraGrid.Columns
Imports DevExpress.XtraGrid.Views.Base
Imports DevExpress.XtraGrid.Views.Grid
Imports DevExpress.XtraGrid.Views.Grid.ViewInfo
Imports DevExpress.XtraGrid.Repository
Imports DevExpress.XtraEditors.Controls
Imports DevExpress.Utils


Partial Class IResults

#Region "SamplesListDataGridView Methods"
    ''' <summary>
    ''' Initialize the Samples List DataGridView
    ''' </summary>
    ''' <remarks>
    ''' Modified by: PG 14/10/2010 - Add the LanguageID parameter
    '''              RH 18/10/2010 - Remove the LanguageID parameter. Now it is a class property.
    '''              RH 24/01/2011 - Added hidden column for the PatientID
    ''' </remarks>
    Private Sub InitializeSamplesListGrid()
        Try
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            bsSamplesListDataGridView.Columns.Clear()

            Dim STATColumn As New DataGridViewImageColumn
            With STATColumn
                .Name = "STAT"
                .HeaderText = ""
                .Width = 24
            End With
            bsSamplesListDataGridView.Columns.Add(STATColumn)

            'bsSamplesListDataGridView.Columns.Add("PatientName", myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Patient", LanguageID))
            'bsSamplesListDataGridView.Columns("PatientName").Width = 122
            'bsSamplesListDataGridView.Columns("PatientName").SortMode = DataGridViewColumnSortMode.Automatic

            'RH 08/05/2012 Change PatientName by PatienID
            bsSamplesListDataGridView.Columns.Add("PatientID", myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Patient", LanguageID))
            bsSamplesListDataGridView.Columns("PatientID").Width = 115 '122 SGM 16/04/2013
            'bsSamplesListDataGridView.Columns("PatientID").Width = 105
            bsSamplesListDataGridView.Columns("PatientID").SortMode = DataGridViewColumnSortMode.Automatic

            Dim PrintReportColumn As New DataGridViewImageColumn
            With PrintReportColumn
                .Name = "Print"
                .HeaderText = ""
                '.Width =  30 dl 16/03/2011
                .Visible = False ' dl 16/03/2011
            End With
            bsSamplesListDataGridView.Columns.Add(PrintReportColumn)

            Dim HISExportColumn As New DataGridViewImageColumn
            With HISExportColumn
                .Name = "HISExport"
                .HeaderText = ""
                '.Width = 30 dl 16/03/2011
                .Visible = False ' dl 16/03/2011
            End With
            bsSamplesListDataGridView.Columns.Add(HISExportColumn)

            'TR 30/09/2013 Solution for Bug #1299 create another checkbox column with the same 
            'charasteristic of order print with visible status = false
            Dim PrintReportColumnNew1 As New DataGridViewCheckBoxColumn
            With PrintReportColumnNew1
                .Name = "OrderToPrint1"
                '.HeaderText = "Print"
                .HeaderText = ""
                .Width = 30
                .Visible = False
            End With
            bsSamplesListDataGridView.Columns.Add(PrintReportColumnNew1)
            'TR 30/09/2013 -END

            ' Ini DL 16/03/2011
            Dim PrintReportColumnNew As New DataGridViewCheckBoxColumn
            With PrintReportColumnNew
                .Name = "OrderToPrint"
                '.HeaderText = "Print"
                .HeaderText = ""
                .Width = 30
            End With
            bsSamplesListDataGridView.Columns.Add(PrintReportColumnNew)

            Dim ExportColumn As New DataGridViewCheckBoxColumn
            With ExportColumn
                .Name = "OrderToExport"
                '.HeaderText = "OrderToExport"
                .HeaderText = MyClass.LISNameForColumnHeaders '"" SGM 12/04/2013
                .Width = 37 '30 SGM 16/04/2013
            End With
            bsSamplesListDataGridView.Columns.Add(ExportColumn)
            ' End DL 16/03/2011

            bsSamplesListDataGridView.Columns.Add("OrderID", "")
            bsSamplesListDataGridView.Columns("OrderID").Visible = False

            'bsSamplesListDataGridView.Columns.Add("PatientID", "")
            'bsSamplesListDataGridView.Columns("PatientID").Visible = False

            'RH 08/05/2012 Change PatienID by PatientName
            bsSamplesListDataGridView.Columns.Add("PatientName", "")
            bsSamplesListDataGridView.Columns("PatientName").Visible = False

            'TR 02/11/2013 -BT #1300 Column use to set the Patient id use to search.
            bsSamplesListDataGridView.Columns.Add("PatientIDToSearch", "")
            bsSamplesListDataGridView.Columns("PatientIDToSearch").Visible = False

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " InitializeSamplesGrid ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub


    ''' <summary>
    ''' Fills the SamplesListDataGridView with Sample Name associated data
    ''' If pCreateGrid -> clear grid and create all rows and icons
    ''' If not PCreateGrid -> do not clear grid and refresh icos for all rows (and create new rows if needed)
    ''' </summary>
    ''' <remarks>
    ''' Created by:  RH 22/07/2010
    ''' Modified by: RH 24/01/2011 - When verifying if a Patient is already included in the DataGrid, search 
    '''                              by PatientID instead of by Patient Name. Show the PatientID as ToolTip of
    '''                              field PatientName
    '''              AG 14/06/2013 - If Barcode is informed, show it as tooltip, else show the patient name
    '''              AG 29/08/2013 - If Barcode is informed, show it into the list and the patient ID as tooltip
    '''              SA 16/06/2014 - BT #1667 ==> Changes in the construction of ToolTips with Patient data:
    '''                                           ** If PatientID = PatientName, only PatientID is shown 
    '''                                           ** If First Character of PatientName is "-" and Last Character of PatientName is "-", remove both  
    '''                                           ** If PatientName is empty, only PatientID is shown
    ''' </remarks>
    Private Sub UpdateSamplesListDataGrid()
        Try
            If isClosingFlag Then Exit Sub ' XB 24/02/2014 - #1496 No refresh if screen is closing

            Dim startTime As DateTime = Now 'AG 21/06/2012 - time estimation
            Dim dgv As BSDataGridView = bsSamplesListDataGridView
            Dim RowIndex As Integer = -1
            Dim SamplesList As List(Of ExecutionsDS.vwksWSExecutionsResultsRow)
            Dim StatFlag() As Boolean = {True, False}
            Dim existsRow As Boolean = False

            ProcessEvent = False

            Dim hyphenIndex As Integer = -1
            Dim myPatientName As String = String.Empty

            Dim addedPatients As New List(Of String) 'AG 21/06/2012 - 
            For i As Integer = 0 To 1
                SamplesList = (From row In ExecutionsResultsDS.vwksWSExecutionsResults _
                               Where row.SampleClass = "PATIENT" _
                               AndAlso row.StatFlag = StatFlag(i) _
                               AndAlso row.RerunNumber = 1 _
                               Select row).ToList()

                For Each sampleRow As ExecutionsDS.vwksWSExecutionsResultsRow In SamplesList
                    existsRow = False
                    'AG 21/06/2012
                    'For k As Integer = 0 To RowIndex 'dgv.Rows.Count - 1
                    '    If dgv("PatientID", k).Value.Equals(sampleRow.PatientID) Then
                    '        existsRow = True
                    '        Exit For
                    '    End If
                    'Next
                    If addedPatients.Contains(sampleRow.PatientID) Then
                        existsRow = True
                    End If

                    'If not exist create row
                    If Not existsRow Then
                        RowIndex += 1
                        If RowIndex = dgv.Rows.Count Then dgv.Rows.Add()

                        dgv("STAT", RowIndex).Value = SampleIconList.Images(i)

                        'dgv("PatientName", RowIndex).Value = sampleRow.PatientName

                        'AG 29/08/2013 - Reverse order: if exists show barcode and then patientID as tooltip
                        'dgv("PatientID", RowIndex).Value = sampleRow.PatientID

                        ''AG 14/06/2013
                        ''dgv("PatientID", RowIndex).ToolTipText = sampleRow.PatientName
                        'If Not sampleRow.IsSpecimenIDListNull Then
                        '    dgv("PatientID", RowIndex).ToolTipText = sampleRow.SpecimenIDList
                        'Else
                        '    dgv("PatientID", RowIndex).ToolTipText = sampleRow.PatientName
                        'End If
                        'AG 14/06/2013

                        'dgv("PatientID", RowIndex).Tag = sampleRow.PatientID 'Inform the patient in the tag (used for load the results in grid)
                        'If sampleRow.IsSpecimenIDListNull Then
                        '    'Case NOT exists BARCODE
                        '    dgv("PatientID", RowIndex).Value = sampleRow.PatientID
                        '    dgv("PatientID", RowIndex).ToolTipText = sampleRow.PatientName
                        'Else
                        '    'Case exists BARCODE
                        '    dgv("PatientID", RowIndex).Value = sampleRow.SpecimenIDList
                        '    dgv("PatientID", RowIndex).ToolTipText = sampleRow.PatientID
                        'End If
                        ''AG 29/08/2013

                        'BT #1667 - Get value of Patient First and Last Name (if both of them have a hyphen as value, ignore these 
                        '           fields and shown only the PatientID
                        myPatientName = sampleRow.PatientName.Trim

                        If (sampleRow.PatientName.Trim.StartsWith("-") AndAlso sampleRow.PatientName.Trim.EndsWith("-")) Then
                            hyphenIndex = sampleRow.PatientName.Trim.IndexOf("-")
                            myPatientName = sampleRow.PatientName.Trim.Remove(hyphenIndex, 1)

                            hyphenIndex = myPatientName.Trim.LastIndexOf("-")
                            myPatientName = myPatientName.Trim.Remove(hyphenIndex, 1)
                        End If

                        'Inform the PatientID as Tag value for the row (it is used to load the Patient Results in the grid)
                        dgv("PatientID", RowIndex).Tag = sampleRow.PatientID

                        'BT #1667 - The TooolTip is built in the same way than in Monitor Screen and in Patients Grid shows in this screen but in Tests View
                        If (Not sampleRow.IsSpecimenIDListNull) Then
                            dgv("PatientID", RowIndex).Value = sampleRow.SpecimenIDList

                            'When the Barcode is informed, the ToolTip will be "BC (PatientID) - FirstName LastName" or, using the variables defined in  
                            'code: "SpecimenIDList (auxString) - PatientName", but with following exceptions (BT #1667):
                            '** If PatientName = "" OR auxString = PatientName, then the ToolTip will be "SpecimenIDList (auxString)"
                            If (sampleRow.PatientID.Trim <> myPatientName.Trim AndAlso myPatientName.Trim <> String.Empty) Then
                                dgv("PatientID", RowIndex).ToolTipText = String.Format("{0} ({1}) - {2}", sampleRow.SpecimenIDList.Trim, sampleRow.PatientID.Trim, myPatientName.Trim)
                            Else
                                dgv("PatientID", RowIndex).ToolTipText = String.Format("{0} ({1})", sampleRow.SpecimenIDList.Trim, sampleRow.PatientID.Trim)
                            End If
                        Else
                            dgv("PatientID", RowIndex).Value = sampleRow.PatientID

                            'When the Barcode is NOT informed, the ToolTip will be "PatientID - FirstName LastName" or, using the variables defined in  
                            'code: "auxString - PatientName", but with following exceptions (BT #1667):
                            '** If PatientName = "" OR auxString = PatientName, then the ToolTip will be "auxString"
                            If (sampleRow.PatientID.Trim <> myPatientName.Trim AndAlso myPatientName.Trim <> String.Empty) Then
                                dgv("PatientID", RowIndex).ToolTipText = String.Format("{0} - {1}", sampleRow.PatientID.Trim, myPatientName.Trim)
                            Else
                                dgv("PatientID", RowIndex).ToolTipText = sampleRow.PatientID
                            End If
                        End If

                        dgv("OrderID", RowIndex).Value = sampleRow.OrderID

                        'TR 02/12/2013 -BT #1300 Set the value to use it to search.
                        dgv("PatientIDToSearch", RowIndex).Value = sampleRow.PatientID

                        'DL 16/03/2011
                        dgv("OrderToPrint", RowIndex).Value = sampleRow.OrderToPrint
                        dgv("OrderToExport", RowIndex).Value = sampleRow.OrderToExport
                        'END DL 16/03/2011
                        existsRow = True

                        addedPatients.Add(sampleRow.PatientID)
                        If sampleRow.PatientID = SamplesListViewText Then dgv.Rows(RowIndex).Selected = True
                    End If

                    'Update Report Print available and HIS export icons
                    If sampleRow.OrderStatus = "CLOSED" AndAlso existsRow Then
                        'Print available
                        If sampleRow.PrintAvailable Then
                            'DL 09/11/2010
                            dgv("Print", RowIndex).Value = PrintImage
                            dgv("Print", RowIndex).ToolTipText = labelReportPrintAvailable
                        Else
                            dgv("Print", RowIndex).Value = NoImage
                            dgv("Print", RowIndex).ToolTipText = labelReportPrintNOTAvailable
                        End If

                        'HIS sent
                        If sampleRow.HIS_Sent Then
                            dgv("HISExport", RowIndex).Value = OKImage
                            dgv("HISExport", RowIndex).ToolTipText = labelHISSent '"HIS sent"
                        Else
                            dgv("HISExport", RowIndex).Value = NoImage
                            dgv("HISExport", RowIndex).ToolTipText = labelHISNOTSent '"HIS NOT sent"
                        End If
                    Else
                        dgv("Print", RowIndex).Value = NoImage
                        dgv("Print", RowIndex).ToolTipText = labelReportPrintNOTAvailable '"Report printing NOT available"
                        dgv("HISExport", RowIndex).Value = NoImage
                        dgv("HISExport", RowIndex).ToolTipText = labelHISNOTSent '"HIS NOT sent"
                    End If
                Next sampleRow
            Next i
            Debug.Print("IResults.UpdateSamplesListDataGrid (Update patient list): " & Now.Subtract(startTime).TotalMilliseconds.ToStringWithDecimals(0)) 'AG 21/06/2012 - time estimation

            Application.DoEvents()
            ProcessEvent = True
            bsSamplesListDataGridView_Click(Nothing, Nothing)

            'TR 30/09/2013
            If (Not isClosingFlag) Then
                bsSamplesListDataGridView.Refresh()
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.StackTrace + " - " + ex.HResult.ToString + "))", Name & " UpdateSamplesListDataGrid ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub


#End Region

End Class