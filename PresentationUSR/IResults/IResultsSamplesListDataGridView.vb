Option Explicit On
'Option Strict On

Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Controls.UserControls
'AG 26/07/2010

Imports Biosystems.Ax00.Global.TO 'AG 22/09/2014 - BA-1940

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
                .Width = 25 'IT 21/10/2014: BA-2036
            End With
            bsSamplesListDataGridView.Columns.Add(PrintReportColumnNew)

            Dim ExportColumn As New DataGridViewCheckBoxColumn
            With ExportColumn
                .Name = "OrderToExport"
                '.HeaderText = "OrderToExport"
                .HeaderText = "" 'IT 21/10/2014: BA-2036
                .Width = 25 'IT 21/10/2014: BA-2036
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
    '''              AG 22/09/2014 - BA-1940 1 patient with N specimenID will appear 1 row and all specimens separated by coma
    '''                                      (tooltip = all specimed separated by coma and finally patientID between brackets)
    ''' </remarks>
    Private Sub UpdateSamplesListDataGrid()
        Try

            'OLD CODE to remove
            '=====================
            'If isClosingFlag Then Exit Sub ' XB 24/02/2014 - #1496 No refresh if screen is closing

            'Dim startTime As DateTime = Now 'AG 21/06/2012 - time estimation
            'Dim dgv As BSDataGridView = bsSamplesListDataGridView
            'Dim RowIndex As Integer = -1
            'Dim SamplesList As List(Of ExecutionsDS.vwksWSExecutionsResultsRow)
            'Dim StatFlag() As Boolean = {True, False}
            'Dim existsRow As Boolean = False

            'ProcessEvent = False

            'Dim hyphenIndex As Integer = -1
            'Dim myPatientName As String = String.Empty

            'Dim addedPatients As New List(Of String) 'AG 21/06/2012 -
            'For i As Integer = 0 To 1
            '    SamplesList = (From row In ExecutionsResultsDS.vwksWSExecutionsResults _
            '                   Where row.SampleClass = "PATIENT" _
            '                   AndAlso row.StatFlag = StatFlag(i) _
            '                   AndAlso row.RerunNumber = 1 _
            '                   Select row).ToList()

            '    For Each sampleRow As ExecutionsDS.vwksWSExecutionsResultsRow In SamplesList
            '        existsRow = False

            '        If addedPatients.Contains(sampleRow.PatientID) Then
            '            existsRow = True
            '        End If

            '        'If not exist create row
            '        If Not existsRow Then
            '            RowIndex += 1
            '            If RowIndex = dgv.Rows.Count Then dgv.Rows.Add()

            '            dgv("STAT", RowIndex).Value = SampleIconList.Images(i)

            '            'BT #1667 - Get value of Patient First and Last Name (if both of them have a hyphen as value, ignore these 
            '            '           fields and shown only the PatientID
            '            myPatientName = sampleRow.PatientName.Trim

            '            If (sampleRow.PatientName.Trim.StartsWith("-") AndAlso sampleRow.PatientName.Trim.EndsWith("-")) Then
            '                hyphenIndex = sampleRow.PatientName.Trim.IndexOf("-")
            '                myPatientName = sampleRow.PatientName.Trim.Remove(hyphenIndex, 1)

            '                hyphenIndex = myPatientName.Trim.LastIndexOf("-")
            '                myPatientName = myPatientName.Trim.Remove(hyphenIndex, 1)
            '            End If

            '            'Inform the PatientID as Tag value for the row (it is used to load the Patient Results in the grid)
            '            dgv("PatientID", RowIndex).Tag = sampleRow.PatientID

            '            'BT #1667 - The TooolTip is built in the same way than in Monitor Screen and in Patients Grid shows in this screen but in Tests View
            '            If (Not sampleRow.IsSpecimenIDListNull) Then
            '                dgv("PatientID", RowIndex).Value = sampleRow.SpecimenIDList

            '                'When the Barcode is informed, the ToolTip will be "BC (PatientID) - FirstName LastName" or, using the variables defined in  
            '                'code: "SpecimenIDList (auxString) - PatientName", but with following exceptions (BT #1667):
            '                '** If PatientName = "" OR auxString = PatientName, then the ToolTip will be "SpecimenIDList (auxString)"
            '                If (sampleRow.PatientID.Trim <> myPatientName.Trim AndAlso myPatientName.Trim <> String.Empty) Then
            '                    dgv("PatientID", RowIndex).ToolTipText = String.Format("{0} ({1}) - {2}", sampleRow.SpecimenIDList.Trim, sampleRow.PatientID.Trim, myPatientName.Trim)
            '                Else
            '                    dgv("PatientID", RowIndex).ToolTipText = String.Format("{0} ({1})", sampleRow.SpecimenIDList.Trim, sampleRow.PatientID.Trim)
            '                End If

            '            Else
            '                dgv("PatientID", RowIndex).Value = sampleRow.PatientID

            '                'When the Barcode is NOT informed, the ToolTip will be "PatientID - FirstName LastName" or, using the variables defined in  
            '                'code: "auxString - PatientName", but with following exceptions (BT #1667):
            '                '** If PatientName = "" OR auxString = PatientName, then the ToolTip will be "auxString"
            '                If (sampleRow.PatientID.Trim <> myPatientName.Trim AndAlso myPatientName.Trim <> String.Empty) Then
            '                    dgv("PatientID", RowIndex).ToolTipText = String.Format("{0} - {1}", sampleRow.PatientID.Trim, myPatientName.Trim)
            '                Else
            '                    dgv("PatientID", RowIndex).ToolTipText = sampleRow.PatientID
            '                End If

            '            End If

            '            dgv("OrderID", RowIndex).Value = sampleRow.OrderID

            '            'TR 02/12/2013 -BT #1300 Set the value to use it to search.
            '            dgv("PatientIDToSearch", RowIndex).Value = sampleRow.PatientID

            '            'DL 16/03/2011
            '            dgv("OrderToPrint", RowIndex).Value = sampleRow.OrderToPrint
            '            dgv("OrderToExport", RowIndex).Value = sampleRow.OrderToExport
            '            'END DL 16/03/2011
            '            existsRow = True

            '            addedPatients.Add(sampleRow.PatientID)
            '            If sampleRow.PatientID = SamplesListViewText Then dgv.Rows(RowIndex).Selected = True
            '        End If

            '        'Update Report Print available and HIS export icons
            '        If sampleRow.OrderStatus = "CLOSED" AndAlso existsRow Then
            '            'Print available
            '            If sampleRow.PrintAvailable Then
            '                'DL 09/11/2010
            '                dgv("Print", RowIndex).Value = PrintImage
            '                dgv("Print", RowIndex).ToolTipText = labelReportPrintAvailable
            '            Else
            '                dgv("Print", RowIndex).Value = NoImage
            '                dgv("Print", RowIndex).ToolTipText = labelReportPrintNOTAvailable
            '            End If

            '            'HIS sent
            '            If sampleRow.HIS_Sent Then
            '                dgv("HISExport", RowIndex).Value = OKImage
            '                dgv("HISExport", RowIndex).ToolTipText = labelHISSent '"HIS sent"
            '            Else
            '                dgv("HISExport", RowIndex).Value = NoImage
            '                dgv("HISExport", RowIndex).ToolTipText = labelHISNOTSent '"HIS NOT sent"
            '            End If
            '        Else
            '            dgv("Print", RowIndex).Value = NoImage
            '            dgv("Print", RowIndex).ToolTipText = labelReportPrintNOTAvailable '"Report printing NOT available"
            '            dgv("HISExport", RowIndex).Value = NoImage
            '            dgv("HISExport", RowIndex).ToolTipText = labelHISNOTSent '"HIS NOT sent"
            '        End If
            '    Next sampleRow

            'Next i
            'Debug.Print("IResults.UpdateSamplesListDataGrid (Update patient list): " & Now.Subtract(startTime).TotalMilliseconds.ToStringWithDecimals(0)) 'AG 21/06/2012 - time estimation

            'Application.DoEvents()
            'ProcessEvent = True
            'bsSamplesListDataGridView_Click(Nothing, Nothing)

            ''TR 30/09/2013
            'If (Not isClosingFlag) Then
            '    bsSamplesListDataGridView.Refresh()
            'End If


            'NEW CODE to uncomment and validate
            '==================================
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

            'AG 22/09/2014 - BA-1940 declare new variables
            Dim patientIDSpecimensList As New List(Of PatientSpecimenTO)
            Dim linqRes As List(Of PatientSpecimenTO) = Nothing
            Dim auxPatientToolTipInfo As String = String.Empty
            Dim updateRow As Boolean = False 'Update an existing item in grid list
            'AG 22/09/2014 - BA-1940

            For i As Integer = 0 To 1
                'AG 22/09/2014 - BA-1940 clear lists and dictionary when changes the priority
                addedPatients.Clear()
                patientIDSpecimensList.Clear()
                'AG 22/09/2014 - BA-1940

                SamplesList = (From row In ExecutionsResultsDS.vwksWSExecutionsResults _
                               Where row.SampleClass = "PATIENT" _
                               AndAlso row.StatFlag = StatFlag(i) _
                               AndAlso row.RerunNumber = 1 _
                               Select row).ToList()

                For Each sampleRow As ExecutionsDS.vwksWSExecutionsResultsRow In SamplesList
                    existsRow = False
                    updateRow = False 'AG 16/09/2014 - BA-1940
                    auxPatientToolTipInfo = String.Empty 'AG 16/09/2014 - BA-1940

                    If addedPatients.Contains(sampleRow.PatientID) Then
                        existsRow = True

                        'AG 22/09/2014 - BA-1940 - Evaluate if is a new specimenID for the patienID and in this case update row
                        If Not sampleRow.IsSpecimenIDListNull Then
                            linqRes = (From a As PatientSpecimenTO In patientIDSpecimensList Where a.patientID = sampleRow.PatientID Select a).ToList
                            If linqRes.Count > 0 Then
                                If Not linqRes(0).Contains(sampleRow.SpecimenIDList) Then
                                    updateRow = True
                                End If
                            End If
                        End If
                        'AG 22/09/2014 - BA-1940
                    End If

                    'If not exist create row
                    If Not existsRow Then
                        RowIndex += 1
                        If RowIndex = dgv.Rows.Count Then dgv.Rows.Add()

                        dgv("STAT", RowIndex).Value = SampleIconList.Images(i)

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

                            'AG 22/09/2014 - BA-1940 - inform the tooltip without patienID / patientName, it will be added after loop completion
                            ''When the Barcode is informed, the ToolTip will be "BC (PatientID) - FirstName LastName" or, using the variables defined in  
                            ''code: "SpecimenIDList (auxString) - PatientName", but with following exceptions (BT #1667):
                            ''** If PatientName = "" OR auxString = PatientName, then the ToolTip will be "SpecimenIDList (auxString)"
                            'If (sampleRow.PatientID.Trim <> myPatientName.Trim AndAlso myPatientName.Trim <> String.Empty) Then
                            '    dgv("PatientID", RowIndex).ToolTipText = String.Format("{0} ({1}) - {2}", sampleRow.SpecimenIDList.Trim, sampleRow.PatientID.Trim, myPatientName.Trim)
                            'Else
                            '    dgv("PatientID", RowIndex).ToolTipText = String.Format("{0} ({1})", sampleRow.SpecimenIDList.Trim, sampleRow.PatientID.Trim)
                            'End If

                            dgv("PatientID", RowIndex).ToolTipText = sampleRow.SpecimenIDList.Trim
                            'AG 22/09/2014 - BA-1940
                        Else
                            dgv("PatientID", RowIndex).Value = sampleRow.PatientID

                            'AG 22/09/2014 - BA-1940 - in case PatientID do not inform here the tooltip, it will be informed after loop completion
                            'When the Barcode is NOT informed, the ToolTip will be "PatientID - FirstName LastName" or, using the variables defined in  
                            'code: "auxString - PatientName", but with following exceptions (BT #1667):
                            '** If PatientName = "" OR auxString = PatientName, then the ToolTip will be "auxString"
                            'If (sampleRow.PatientID.Trim <> myPatientName.Trim AndAlso myPatientName.Trim <> String.Empty) Then
                            '    dgv("PatientID", RowIndex).ToolTipText = String.Format("{0} - {1}", sampleRow.PatientID.Trim, myPatientName.Trim)
                            'Else
                            '    dgv("PatientID", RowIndex).ToolTipText = sampleRow.PatientID
                            'End If
                            dgv("PatientID", RowIndex).ToolTipText = String.Empty 'AG 23/09/2014 - BA-1940 - Initiate a clear value
                            'AG 22/09/2014 - BA-1940

                        End If

                        'AG 22/09/2014 - BA-1640 
                        'Create new entry in TO list with: patientID, 1st specimen, row index in grid, tool tip ending string
                        linqRes = (From a As PatientSpecimenTO In patientIDSpecimensList Where a.patientID = sampleRow.PatientID Select a).ToList
                        If linqRes.Count = 0 Then
                            Dim newItem As New PatientSpecimenTO
                            newItem.patientID = sampleRow.PatientID
                            newItem.UpdateSpecimenList(dgv("PatientID", RowIndex).Value)
                            newItem.RowIndex = RowIndex
                            If (sampleRow.PatientID.Trim <> myPatientName.Trim AndAlso myPatientName.Trim <> String.Empty) Then
                                auxPatientToolTipInfo = String.Format("({0}) - {1}", sampleRow.PatientID.Trim, myPatientName.Trim)
                            Else
                                auxPatientToolTipInfo = String.Format("({0})", sampleRow.PatientID.Trim)
                            End If
                            newItem.EndingToolTip = auxPatientToolTipInfo
                            patientIDSpecimensList.Add(newItem)
                        End If
                        'AG 22/09/2014 - BA-1640

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

                        'AG 22/09/2014 - BA-1940 update the specimenID list
                    ElseIf updateRow Then
                        'Get the row in grid
                        If linqRes.Count > 0 Then
                            dgv("PatientID", linqRes(0).RowIndex).Value &= ", " & sampleRow.SpecimenIDList
                            dgv("PatientID", linqRes(0).RowIndex).ToolTipText &= ", " & sampleRow.SpecimenIDList

                            linqRes(0).UpdateSpecimenList(sampleRow.SpecimenIDList) 'Add specimen to the TO list
                        End If
                        'AG 22/09/2014 - BA-1940

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

                'AG 22/09/2014 - BA-1940 finally complete the tooltip in list with (PatID) - pat name
                For Each item As PatientSpecimenTO In patientIDSpecimensList
                    dgv("PatientID", item.RowIndex).ToolTipText &= " " & item.EndingToolTip
                Next
                'AG 22/09/2014 - BA-1940

            Next i
            Debug.Print("IResults.UpdateSamplesListDataGrid (Update patient list): " & Now.Subtract(startTime).TotalMilliseconds.ToStringWithDecimals(0)) 'AG 21/06/2012 - time estimation

            'AG 22/09/2014 - BA-1940 - release memory
            addedPatients = Nothing
            patientIDSpecimensList = Nothing
            linqRes = Nothing
            'AG 22/09/2014 - BA-1940

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