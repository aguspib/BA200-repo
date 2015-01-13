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

#Region "TestsListDataGridView Methods"
    ''' <summary>
    ''' Initialize the DataGridView containing the list of Tests with Results
    ''' </summary>
    ''' <remarks>
    ''' Modified by: PG 14/10/2010 - Add the LanguageID parameter
    '''              RH 18/10/2010 - Remove the LanguageID parameter. Now it is a class property.
    '''              SA 26/01/2011 - Added a hidden column to store the code of the TestType
    '''              RH 02/02/2011 - Removed the hidden column for storing the code of the TestType
    ''' </remarks>
    Private Sub InitializeTestsListGrid()
        Try
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            bsTestsListDataGridView.Columns.Clear()

            Dim TestTypeColumn As New DataGridViewImageColumn
            With TestTypeColumn
                .Name = "TestType"
                .HeaderText = ""
                .Width = 24
            End With
            bsTestsListDataGridView.Columns.Add(TestTypeColumn)

            bsTestsListDataGridView.Columns.Add("TestName", myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_TestNames", LanguageID))
            bsTestsListDataGridView.Columns("TestName").Width = bsTestsListDataGridView.Width - 2 - bsTestsListDataGridView.Columns("TestType").Width
            bsTestsListDataGridView.Columns("TestName").SortMode = DataGridViewColumnSortMode.Automatic
            bsTestsListDataGridView.Columns.Add("OrderID", "")
            bsTestsListDataGridView.Columns("OrderID").Visible = False

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & " InitializeTestsGrid ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Fills the TestsListDataGridView with Test Name associated data
    ''' </summary>
    ''' <remarks>
    ''' Created by:  RH 07/10/2010
    ''' Modified by: AG 01/12/2010 - Adapted for ISE and OFFS tests
    '''              SA 26/01/2011 - Besides the TestType Icon, inform also the Code in the correspondent hidden column
    '''              RH 02/02/2011 - Removed the hidden column for storing the code of the TestType
    '''              XB 28/11/2014 - Sort the CALC tests behind ISE anf OFFS too - BA-1867
    '''              WE 13/01/2015 - BA-2153: Note that the sequence of elements of array 'TestType' must always be in-sync
    '''                                       with the ImageList 'TestTypeIconList' in Sub PrepareButtons (Class IResults).
    ''' </remarks>
    Private Sub UpdateTestsListDataGrid()
        Try
            If isClosingFlag Then Exit Sub ' XB 24/02/2014 - #1496 No refresh if screen is closing

            Dim startTime As DateTime = Now 'AG 21/06/2012 - Time estimation
            Dim dgv As BSDataGridView = bsTestsListDataGridView
            Dim TestsList As List(Of ResultsDS.vwksResultsRow)
            Dim RowIndex As Integer = -1

            ' XB 28/11/2014 - BA-1867
            'Dim TestType() As String = {"STD", "CALC", "ISE", "OFFS"}
            Dim TestType() As String = {"STD", "ISE", "OFFS", "CALC"}
            ' XB 28/11/2014 - BA-1867

            ProcessEvent = False
            'TR 11/07/2012 -Declare Outside the for 
            Dim addedTests As New List(Of String) 'AG 21/06/2012
            Dim myTestType As String = String.Empty
            Dim existsRow As Boolean = False


            For i As Integer = 0 To 3
                'Dim addedTests As New List(Of String) 'AG 21/06/2012
                'Dim myTestType As String = TestType(i)
                myTestType = TestType(i)
                TestsList = (From row In AverageResultsDS.vwksResults _
                             Where String.Equals(row.TestType, myTestType) _
                             Select row).ToList()

                For Each testRow As ResultsDS.vwksResultsRow In TestsList
                    'Dim existsRow As Boolean = False
                    existsRow = False

                    'AG 21/06/2012
                    'For k As Integer = 0 To RowIndex 'dgv.Rows.Count - 1
                    '    If dgv("TestName", k).Value.Equals(testRow.TestName) Then
                    '        existsRow = True
                    '        Exit For
                    '    End If
                    'Next
                    If addedTests.Contains(testRow.TestName) Then existsRow = True

                    If Not existsRow Then
                        RowIndex += 1
                        If RowIndex = dgv.Rows.Count Then dgv.Rows.Add()

                        dgv("TestType", RowIndex).Value = TestTypeIconList.Images(i)
                        dgv("TestName", RowIndex).Value = testRow.TestName
                        dgv("OrderID", RowIndex).Value = testRow.OrderID

                        IsTestSTD(testRow.TestName) = (i = 0)
                        addedTests.Add(testRow.TestName) 'AG 21/06/2012
                        If String.Equals(testRow.TestName, TestsListViewText) Then dgv.Rows(RowIndex).Selected = True

                    End If
                Next testRow
            Next i
            Debug.Print("IResults.UpdateTestsListDataGrid (Update Test List): " & Now.Subtract(startTime).TotalMilliseconds.ToStringWithDecimals(0)) 'AG 21/06/2012 - time estimation

            ProcessEvent = True
            bsTestsListDataGridView_SelectionChanged(Nothing, Nothing)

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " UpdateTestsListDataGrid ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

#End Region

End Class