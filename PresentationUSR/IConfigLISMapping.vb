Option Explicit On
Option Strict Off

Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.CommunicationsSwFw

Public Class IConfigLISMapping
    Inherits Biosystems.Ax00.PresentationCOM.BSBaseForm

#Region "Declarations"
    Private currentLanguage As String                      'To store the current application language  


    Private ScreenIsLoading As Boolean = True 'flag for avoiding to handle events while screen is not loaded yet

    Private AllLISMappingDS As LISMappingsDS
    Private ConfigLISMappingDS As LISMappingsDS
    Private TestsLISMappingDS As AllTestsByTypeDS

    Private EditingConfigLISMappingDS As LISMappingsDS 'local copy for validation to preserve unique LIS value for each type
    Private EditingTestsLISMappingDS As AllTestsByTypeDS 'local copy for validation to preserve unique LIS value for all test types

    Private mdiAnalyzerCopy As AnalyzerManager

    Private IsRowEditing As Boolean = False ' a row in the datagrid is being edited

    Private mySampleTypesInWS As New List(Of String)

    Private mySavedOrdersFromLIMS As Boolean = False 'DL 24/04/2013

    Private previousStatusBarMessage As String = ""

    'SGM 09/05/2013
    Private SampleTypesText As String
    Private TestUnitsText As String
    Private STDTestText As String
    Private CALCTestText As String
    Private ISETestText As String
    Private OFFSTestText As String

#End Region

#Region "Private Properties"

    Private ReadOnly Property IsReadOnly As Boolean
        Get
            Dim res As Boolean = False

            'Dim myGlobalbase As New GlobalBase
            If GlobalBase.GetSessionInfo.UserLevel = "OPERATOR" Then
                res = True
            End If

            If Not res Then
                If Not mdiAnalyzerCopy Is Nothing Then
                    If mdiAnalyzerCopy.AnalyzerStatus = GlobalEnumerates.AnalyzerManagerStatus.RUNNING Then
                        res = True
                    End If
                End If
                If Not res Then
                    If mySavedOrdersFromLIMS Then
                        res = True
                    End If
                End If
            End If


            If res Then Me.LISMappingDataGridView.ReadOnly = True

            Return res

        End Get
    End Property

    Private IsEditionModeAttr As Boolean = False
    Private Property IsEditionMode As Boolean
        Get
            Return IsEditionModeAttr
        End Get
        Set(value As Boolean)
            If value <> IsEditionModeAttr Then
                If Not MyClass.IsReadOnly Then
                    IsEditionModeAttr = value
                    Me.EditButton.Enabled = Not value
                    Me.ButtonCancel.Enabled = value
                    Me.FilterComboBox.Enabled = Not value
                    Me.SaveButton.Enabled = value

                    Me.LISMappingDataGridView.Columns("LISValue").ReadOnly = Not value
                    For i As Integer = 0 To Me.LISMappingDataGridView.Rows.Count - 1
                        If value Then
                            Me.LISMappingDataGridView.Rows(i).Cells("LISValue").Style.BackColor = Color.White
                        Else
                            Me.LISMappingDataGridView.Rows(i).Cells("LISValue").Style.BackColor = Color.WhiteSmoke
                        End If
                    Next i

                    For Each dr As DataGridViewRow In Me.LISMappingDataGridView.Rows
                        MyClass.CheckGridRowIsLocked(dr)

                    Next

                End If
            End If
        End Set
    End Property

    Private IsAnyChangedAttr As Boolean = False
    Private Property IsAnyChanged As Boolean
        Get
            Return IsAnyChangedAttr
        End Get
        Set(value As Boolean)
            If value <> IsAnyChangedAttr Then
                If Not MyClass.IsReadOnly Then
                    IsAnyChangedAttr = value
                    If value Then
                        Me.SaveButton.Enabled = True
                    End If
                End If
            End If
        End Set
    End Property

    'Private IsAllValidatedOKAttr As Boolean = False
    Private ReadOnly Property IsAllValidatedOK As Boolean
        Get
            Dim isOK As Boolean = Not MyClass.ValidateAllGridRows
            MyClass.UpdateEditingConfigLISMappingDS()
            MyClass.UpdateEditingTestsLISMappingDS()
            Return isOK
        End Get
    End Property

    Private AnalyzerIDAttr As String = ""
    Private ReadOnly Property ActiveAnalyzer() As String
        Get
            Return mdiAnalyzerCopy.ActiveAnalyzer
        End Get
    End Property

    Public ReadOnly Property ActiveWorkSession() As String
        Get
            Return mdiAnalyzerCopy.ActiveWorkSession
        End Get
    End Property

#End Region

#Region "Enum"

    Private Enum LISItemTypes
        SAMPLE_TYPES
        TEST_UNITS
        STD
        CALC
        ISE
        OFFS
    End Enum
#End Region


#Region "Private Methods"
    ''' <summary>
    ''' Fills LIS Mapping types combo from preloadmasterdata
    ''' </summary>
    ''' <remarks>
    ''' Created by XB 01/03/2013
    ''' Modified by SG 10/07/2013 - discard Remarks
    ''' </remarks>
    Private Sub FillFilterCombo()
        Try
            Dim myMLRD As New MultilanguageResourcesDelegate
            Dim myGlobalDataTO As New GlobalDataTO

            Me.FilterComboBox.Items.Clear()

            'all
            Me.FilterComboBox.Items.Add(myMLRD.GetResourceText(Nothing, "LBL_SRV_All", currentLanguage))

            'Get all Sample Types of the Tests included in the Formula defined for the Calculated Test
            'SG 10/07/2013 - discard Remarks
            Dim qValueTypes As List(Of String) = (From a In MyClass.AllLISMappingDS.vcfgLISMapping Where a.ValueType <> "REMARK" _
                                                 Select a.ValueTypeDesc).Distinct.ToList()
            'Dim qValueTypes As List(Of String) = (From a In MyClass.AllLISMappingDS.vcfgLISMapping _
            '                                     Select a.ValueTypeDesc).Distinct.ToList()

            For Each T As String In qValueTypes
                Me.FilterComboBox.Items.Add(T)
            Next

            If Me.FilterComboBox.Items.Count > 0 Then
                Me.FilterComboBox.SelectedIndex = 0
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".FillFilterCombo", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".FillFilterCombo", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Loads the data from the view of 
    ''' </summary>
    ''' <remarks>Created by SGM 08/04/2013</remarks>
    Private Sub LoadConfigLISMappingData()
        Try
            Dim resultData As New GlobalDataTO
            Dim myLISMasterMappingDelegate As New LISMappingsDelegate
            resultData = myLISMasterMappingDelegate.ReadAll(Nothing, MyClass.currentLanguage)
            If Not resultData.HasError AndAlso resultData.SetDatos IsNot Nothing Then
                MyClass.ConfigLISMappingDS = TryCast(resultData.SetDatos, LISMappingsDS)
            End If

            'initialize the editing ds
            MyClass.EditingConfigLISMappingDS = MyClass.ConfigLISMappingDS.Clone
            For Each dr As LISMappingsDS.vcfgLISMappingRow In MyClass.ConfigLISMappingDS.vcfgLISMapping.Rows
                Dim myRow As LISMappingsDS.vcfgLISMappingRow = MyClass.EditingConfigLISMappingDS.vcfgLISMapping.NewvcfgLISMappingRow
                MyClass.EditingConfigLISMappingDS.vcfgLISMapping.ImportRow(dr)
            Next

            MyClass.EditingConfigLISMappingDS.AcceptChanges()

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".LoadConfigLISMappingData", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".LoadConfigLISMappingData", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Loads all the data from the view vparAllTestsByType order by test name and assigns it to TestsLISMappingsDS
    ''' </summary>
    ''' <remarks>Created by SGM 08/04/2013</remarks>
    Private Sub LoadTestsLISMappingData()
        Try
            Dim resultData As New GlobalDataTO
            Dim myLISTestMappingDelegate As New AllTestByTypeDelegate
            resultData = myLISTestMappingDelegate.ReadAll(Nothing)
            If Not resultData.HasError AndAlso resultData.SetDatos IsNot Nothing Then
                MyClass.TestsLISMappingDS = TryCast(resultData.SetDatos, AllTestsByTypeDS)
            End If

            'initialize the editing ds
            MyClass.EditingTestsLISMappingDS = MyClass.TestsLISMappingDS.Clone
            For Each dr As AllTestsByTypeDS.vparAllTestsByTypeRow In MyClass.TestsLISMappingDS.vparAllTestsByType.Rows
                Dim myRow As AllTestsByTypeDS.vparAllTestsByTypeRow = MyClass.EditingTestsLISMappingDS.vparAllTestsByType.NewvparAllTestsByTypeRow
                MyClass.EditingTestsLISMappingDS.vparAllTestsByType.ImportRow(dr)
            Next
            MyClass.EditingTestsLISMappingDS.AcceptChanges()

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".LoadTestsLISMappingData", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".LoadTestsLISMappingData", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Unions the data loaded in ConfigLISMappingsDS and in TestsLISMappingsDS assigns it to SavedLISMappingsDS
    ''' </summary>
    ''' <remarks>Created by SGM 08/04/2013</remarks>
    Private Sub MergeLISMappingData()
        Try

            MyClass.AllLISMappingDS = New LISMappingsDS
            If MyClass.ConfigLISMappingDS IsNot Nothing Then
                For Each dr As LISMappingsDS.vcfgLISMappingRow In MyClass.ConfigLISMappingDS.vcfgLISMapping
                    Dim myRow As LISMappingsDS.vcfgLISMappingRow = MyClass.AllLISMappingDS.vcfgLISMapping.NewvcfgLISMappingRow
                    MyClass.AllLISMappingDS.vcfgLISMapping.ImportRow(dr)
                Next
                MyClass.AllLISMappingDS.AcceptChanges()
            End If

            If MyClass.TestsLISMappingDS IsNot Nothing Then
                For Each dr As AllTestsByTypeDS.vparAllTestsByTypeRow In MyClass.TestsLISMappingDS.vparAllTestsByType
                    Dim myRow As LISMappingsDS.vcfgLISMappingRow = MyClass.AllLISMappingDS.vcfgLISMapping.NewvcfgLISMappingRow
                    With myRow
                        .ValueType = dr.TestType
                        .ValueId = dr.TestID
                        .LongName = dr.TestName
                        .LISValue = dr.LISValue
                        .LanguageID = MyClass.currentLanguage
                        .InUse = dr.InUse
                        .UniqueSampleType = dr.UniqueSampleType
                    End With
                    MyClass.AllLISMappingDS.vcfgLISMapping.AddvcfgLISMappingRow(myRow)
                Next
                MyClass.AllLISMappingDS.AcceptChanges()
            End If

            For Each dr As LISMappingsDS.vcfgLISMappingRow In MyClass.AllLISMappingDS.vcfgLISMapping
                If Not dr.IsValueTypeNull Then
                    dr.BeginEdit()
                    Select Case dr.ValueType
                        Case LISItemTypes.SAMPLE_TYPES.ToString
                            dr.ValueTypeDesc = MyClass.SampleTypesText

                        Case LISItemTypes.TEST_UNITS.ToString
                            dr.ValueTypeDesc = MyClass.TestUnitsText

                        Case LISItemTypes.STD.ToString
                            dr.ValueTypeDesc = MyClass.STDTestText

                        Case LISItemTypes.CALC.ToString
                            dr.ValueTypeDesc = MyClass.CALCTestText

                        Case LISItemTypes.ISE.ToString
                            dr.ValueTypeDesc = MyClass.ISETestText

                        Case LISItemTypes.OFFS.ToString
                            dr.ValueTypeDesc = MyClass.OFFSTestText

                    End Select
                    dr.EndEdit()
                End If

            Next

            MyClass.AllLISMappingDS.vcfgLISMapping.AcceptChanges()

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".MergeLISMappingData", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".MergeLISMappingData", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub


    ''' <summary>
    ''' Gets the Sample Types currently being used
    ''' </summary>
    ''' <remarks>Created by SGM 08/04/2013</remarks>
    Private Sub GetSampleTypesInUse()
        Try
            Dim myOTDelegate As New WSOrderTestsDelegate
            Dim resultData As GlobalDataTO = myOTDelegate.GetSampleTypesInWS(Nothing)
            If Not resultData.HasError AndAlso resultData.SetDatos IsNot Nothing Then
                mySampleTypesInWS = TryCast(resultData.SetDatos, List(Of String))
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".GetSampleTypesInUse", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".GetSampleTypesInUse", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub


    ''' <summary>
    ''' Load all data needed for the screen
    ''' </summary>
    ''' <remarks>
    ''' Created by XB 01/03/2013
    ''' </remarks>
    Private Sub ScreenLoad()
        Try
            Cursor = Cursors.WaitCursor
            ScreenIsLoading = True

            'Get the current Language from the current Application Session
            'Dim currentLanguageGlobal As New GlobalBase
            currentLanguage = GlobalBase.GetSessionInfo().ApplicationLanguage.Trim.ToString

            If Not AppDomain.CurrentDomain.GetData("GlobalAnalyzerManager") Is Nothing Then
                mdiAnalyzerCopy = CType(AppDomain.CurrentDomain.GetData("GlobalAnalyzerManager"), AnalyzerManager) ' Use the same AnalyzerManager as the MDI
            End If


            'Load the multilanguage texts for all Screen Labels
            MyClass.GetScreenLabels()

            'Get Icons for graphical buttons
            MyClass.PrepareButtons()

            'gets the sample types in use
            MyClass.GetSampleTypesInUse()

            'loads all the data from vcfgLISMappings
            MyClass.LoadConfigLISMappingData()

            'loads all the data from vparAllTestsByType
            MyClass.LoadTestsLISMappingData()

            'merge data
            MyClass.MergeLISMappingData()

            'loads the filter criteria
            MyClass.FillFilterCombo()

            'DL 24/04/2013. BEGIN
            Dim myGlobalDataTO As New GlobalDataTO
            Dim mySavedWSDelegate As New SavedWSDelegate

            myGlobalDataTO = mySavedWSDelegate.GetAll(Nothing, True)
            If Not myGlobalDataTO.HasError Then
                If Not myGlobalDataTO.SetDatos Is Nothing AndAlso DirectCast(myGlobalDataTO.SetDatos, SavedWSDS).tparSavedWS.Count > 0 Then
                    mySavedOrdersFromLIMS = True
                    IAx00MainMDI.SetErrorStatusMessage(GetMessageText(GlobalEnumerates.Messages.LIS_MAPPING_WARNING.ToString)) 'SGM 06/05/2013 - show message in status bar "Locked because there are pending LIS orders"
                End If
            End If
            'DL 24/04/2013. END



        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ScreenLoad", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ScreenLoad", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        Finally
            Cursor = Cursors.Default
        End Try
    End Sub

    ''' <summary>
    ''' Get texts in the current application language for all screen controls
    ''' </summary>
    ''' <remarks>
    ''' '<param name="pLanguageID"> The current Language of Application </param>
    ''' Created by XB 01/03/2013
    ''' </remarks>
    Private Sub GetScreenLabels()
        Try
            'Get the current Language from the current Application Session
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            'For Labels.....
            bsLISMappingLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_LIS_Mapping", currentLanguage)
            BsLISMappingTypeLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_LIS_Mapping_Types", currentLanguage)

            ' Fot tooltips
            bsScreenToolTips.SetToolTip(CloseButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_CloseScreen", currentLanguage))
            bsScreenToolTips.SetToolTip(ButtonCancel, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Cancel", currentLanguage))
            bsScreenToolTips.SetToolTip(EditButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Edit", currentLanguage))
            bsScreenToolTips.SetToolTip(SaveButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Save", currentLanguage))

            'SGM 09/05/2013
            MyClass.SampleTypesText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SampleType", currentLanguage)
            MyClass.TestUnitsText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "MAD_MASTER_TEST_UNITS", currentLanguage)
            MyClass.STDTestText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_StandardTests", currentLanguage)
            MyClass.CALCTestText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_CalcTests_Long", currentLanguage)
            MyClass.ISETestText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_WSTests_ISETests", currentLanguage)
            MyClass.OFFSTestText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_WSTests_OffSystemTests", currentLanguage)


        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".GetScreenLabels ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".GetScreenLabels ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Method incharge to load the image for each graphical button 
    ''' </summary>
    ''' <remarks>
    ''' Created by XB 01/03/2013
    ''' </remarks>
    Private Sub PrepareButtons()
        Try
            Dim auxIconName As String = ""
            Dim iconPath As String = IconsPath

            ' Close Button
            auxIconName = GetIconName("CANCEL")
            If (auxIconName <> "") Then
                CloseButton.Image = Image.FromFile(iconPath & auxIconName)
            End If

            ' Edit EditButton
            auxIconName = GetIconName("EDIT")
            If (auxIconName <> "") Then
                EditButton.Image = Image.FromFile(iconPath & auxIconName)
            End If

            ' Save SaveButton
            auxIconName = GetIconName("SAVE")
            If (auxIconName <> "") Then
                SaveButton.Image = Image.FromFile(iconPath & auxIconName)
            End If

            ' Undo Button
            auxIconName = GetIconName("UNDO")
            If (auxIconName <> "") Then
                ButtonCancel.Image = Image.FromFile(iconPath & auxIconName)
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".PrepareButtons ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".PrepareButtons ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Configure the grid for details of settings data grid
    ''' </summary>
    ''' <remarks>
    ''' Created by XB 01/03/2013
    ''' Modified by SGM 05/03/2013
    ''' Modified by SGM 05/05/2013 - add InUse column
    ''' </remarks>
    Private Sub PrepareLISMappingsGrid()
        Try
            ' TODO : TEXTS !!!
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            LISMappingDataGridView.Rows.Clear()
            LISMappingDataGridView.Columns.Clear()

            LISMappingDataGridView.AutoGenerateColumns = False
            LISMappingDataGridView.AllowUserToAddRows = False
            LISMappingDataGridView.AllowUserToDeleteRows = False
            LISMappingDataGridView.EditMode = DataGridViewEditMode.EditOnEnter
            LISMappingDataGridView.MultiSelect = False
            LISMappingDataGridView.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter

            'Value ID (hidden column) 
            Dim ValueIDCol As New DataGridViewTextBoxColumn
            ValueIDCol.HeaderText = "*ValueID"
            ValueIDCol.Name = "ValueID"
            ValueIDCol.DataPropertyName = "ValueID"
            ValueIDCol.Visible = False
            LISMappingDataGridView.Columns.Add(ValueIDCol)

            'Value Type (hidden column)
            Dim ValueTypeCol As New DataGridViewTextBoxColumn
            ValueTypeCol.Name = "ValueType"
            ValueTypeCol.DataPropertyName = "ValueType"
            ValueTypeCol.Visible = False
            LISMappingDataGridView.Columns.Add(ValueTypeCol)


            'Value Type 
            Dim ValueTypeDescCol As New DataGridViewTextBoxColumn
            ValueTypeDescCol.Name = "ValueTypeDesc"
            ValueTypeDescCol.DataPropertyName = "ValueTypeDesc"
            ValueTypeDescCol.HeaderText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Type", currentLanguage)
            ValueTypeDescCol.ReadOnly = True
            ValueTypeDescCol.Visible = True
            LISMappingDataGridView.Columns.Add(ValueTypeDescCol)
            LISMappingDataGridView.Columns("ValueTypeDesc").Width = 290
            LISMappingDataGridView.Columns("ValueTypeDesc").SortMode = DataGridViewColumnSortMode.Automatic
            LISMappingDataGridView.Columns("ValueTypeDesc").HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight

            'LongName
            Dim LongNameCol As New DataGridViewTextBoxColumn
            LongNameCol.Name = "LongName"
            LongNameCol.DataPropertyName = "LongName"
            LongNameCol.HeaderText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Name", currentLanguage)
            LongNameCol.ReadOnly = True
            LongNameCol.Visible = True
            LISMappingDataGridView.Columns.Add(LongNameCol)
            LISMappingDataGridView.Columns("LongName").Width = 310
            LISMappingDataGridView.Columns("LongName").SortMode = DataGridViewColumnSortMode.NotSortable
            LISMappingDataGridView.Columns("LongName").HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight

            'LISValue
            Dim LISValueCol As New DataGridViewTextBoxColumn
            LISValueCol.Name = "LISValue"
            LISValueCol.DataPropertyName = "LISValue"
            LISValueCol.HeaderText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_LIS_Name", currentLanguage)
            LISValueCol.Visible = True
            LISMappingDataGridView.Columns.Add(LISValueCol)
            LISMappingDataGridView.Columns("LISValue").Width = 310
            LISMappingDataGridView.Columns("LISValue").SortMode = DataGridViewColumnSortMode.NotSortable
            LISMappingDataGridView.Columns("LISValue").HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight

            'IsTest (hidden column) 
            Dim IsTestCol As New DataGridViewCheckBoxColumn
            IsTestCol.Name = "IsTest"
            IsTestCol.DataPropertyName = "IsTest"
            IsTestCol.Visible = False
            LISMappingDataGridView.Columns.Add(IsTestCol)

            'IsCalcTest (hidden column) 
            Dim IsCalcTestCol As New DataGridViewCheckBoxColumn
            IsCalcTestCol.Name = "IsCalcTest"
            IsCalcTestCol.DataPropertyName = "IsCalcTest"
            IsCalcTestCol.Visible = False
            LISMappingDataGridView.Columns.Add(IsCalcTestCol)

            'Changed (hidden column) 
            Dim ChangedCol As New DataGridViewCheckBoxColumn
            ChangedCol.Name = "Changed"
            ChangedCol.DataPropertyName = "Changed"
            ChangedCol.Visible = False
            LISMappingDataGridView.Columns.Add(ChangedCol)

            'HasError (hidden column) 
            Dim HasErrorCol As New DataGridViewCheckBoxColumn
            HasErrorCol.Name = "HasError"
            HasErrorCol.DataPropertyName = "HasError"
            HasErrorCol.Visible = False
            LISMappingDataGridView.Columns.Add(HasErrorCol)

            'ReadOnly (is InUse) (hidden column) 
            Dim InUseCol As New DataGridViewCheckBoxColumn
            InUseCol.Name = "InUse"
            InUseCol.DataPropertyName = "InUse"
            InUseCol.Visible = False
            LISMappingDataGridView.Columns.Add(InUseCol)

            'ReadOnly (is MultiSampleType) (hidden column) 
            Dim UniqueSampleTypeCol As New DataGridViewCheckBoxColumn
            UniqueSampleTypeCol.Name = "UniqueSampleType"
            UniqueSampleTypeCol.DataPropertyName = "UniqueSampleType"
            UniqueSampleTypeCol.Visible = False
            LISMappingDataGridView.Columns.Add(UniqueSampleTypeCol)

            For Each dc As DataGridViewColumn In LISMappingDataGridView.Columns
                dc.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter
            Next


            '#If DEBUG Then
            '            ValueTypeCol.Visible = True
            '            ValueIDCol.Visible = True
            '            IsTestCol.Visible = True
            '            ChangedCol.Visible = True
            '            HasErrorCol.Visible = True
            '#End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".PrepareLISMappingsGrid " & Name, EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PrepareLISMappingsGrid ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Fills the grid with the data loaded
    ''' </summary>
    ''' <remarks>
    ''' Created by SGM 04/03/2013
    ''' Modified by SGM 10/07/2013 - discard Remarks
    ''' </remarks>
    Private Sub FillLISMappingsGrid()
        Try
            Me.LISMappingDataGridView.DataSource = Nothing
            Me.LISMappingDataGridView.Rows.Clear()

            Application.DoEvents()

            Dim viewData As DataView = New DataView
            viewData = MyClass.AllLISMappingDS.vcfgLISMapping.DefaultView

            Dim myMLRD As New MultilanguageResourcesDelegate
            Dim All As String = myMLRD.GetResourceText(Nothing, "LBL_SRV_All", currentLanguage)
            If Me.FilterComboBox.Items.Count > 0 Then
                If CStr(Me.FilterComboBox.SelectedItem) <> All Then
                    viewData.RowFilter = String.Format("ValueTypeDesc = '{0}'", CStr(Me.FilterComboBox.SelectedItem))
                    viewData.Sort = "LongName" '"ValueID" 'DL 24/04/2013
                Else
                    viewData.RowFilter = String.Format("ValueType <> '{0}'", "REMARK") 'SGM 10/07/2013 - discard Remarks
                    'viewData.RowFilter = ""
                    viewData.Sort = "ValueTypeDesc"
                End If
            End If
            Dim myBindingSource As BindingSource = New BindingSource
            myBindingSource.DataSource = viewData
            Me.LISMappingDataGridView.DataSource = myBindingSource

            Application.DoEvents()

            MyClass.PrepareRows()

            Me.LISMappingDataGridView.Focus()



        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".FillLISMappingsGrid " & Name, EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".FillLISMappingsGrid ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub


    ''' <summary>
    ''' Prepares the additional columns for each row
    ''' </summary>
    ''' <remarks>
    ''' Created by SGM 04/03/2012
    ''' Modified by SGM 05/04/2012 - set readonly the items in use
    ''' </remarks>
    Private Sub PrepareRows()
        Try
            For Each dr As DataGridViewRow In Me.LISMappingDataGridView.Rows

                dr.Cells("IsTest").Value = IsNumeric(CStr(dr.Cells("ValueID").Value))

                If CBool(dr.Cells("IsTest").Value) Then
                    dr.Cells("IsCalcTest").Value = CBool(CStr(dr.Cells("ValueType").Value = "CALC"))
                Else
                    dr.Cells("IsCalcTest").Value = False
                End If

                dr.Cells("Changed").Value = False
                dr.Cells("HasError").Value = False


                MyClass.CheckGridRowIsLocked(dr)


            Next
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".PrepareRows " & Name, EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PrepareRows ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub
    ''' <summary>
    ''' Set the screen controls to INITIAL MODE
    ''' </summary>
    ''' <remarks>
    ''' Created by XB 01/03/2013
    ''' </remarks> 
    Private Sub InitialModeScreenStatus(Optional ByVal pInitializeListView As Boolean = True)
        Try


            MyClass.IsAnyChanged = False
            MyClass.IsEditionMode = False

            ''DL 24/04/2013. BEGIN
            'If mySavedOrdersFromLIMS Then
            '    Me.EditButton.Enabled = False
            'Else
            '    Me.EditButton.Enabled = Not MyClass.IsReadOnly
            'End If
            ''DL 24/04/2013. END
            Me.EditButton.Enabled = Not MyClass.IsReadOnly

            Me.ButtonCancel.Enabled = False
            Me.SaveButton.Enabled = False
            Me.FilterComboBox.Enabled = True
            Me.LISMappingDataGridView.Columns("LISValue").ReadOnly = True

            For i As Integer = 0 To Me.LISMappingDataGridView.Rows.Count - 1
                Me.LISMappingDataGridView.Rows(i).Cells("LISValue").Style.BackColor = Color.WhiteSmoke
            Next i

            ScreenIsLoading = False



        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".InitialModeScreenStatus ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".InitialModeScreenStatus", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Save data of the updated Settings
    ''' </summary>
    ''' <remarks>
    ''' Created by XB 01/03/2013
    ''' </remarks>
    Private Function SaveChanges() As Boolean

        Dim resultData As New GlobalDataTO
        Dim isSavedOk As Boolean = False

        Try

            If MyClass.IsAllValidatedOK Then
                Dim myMasterDataToUpdate As New LISMappingsDS
                Dim myTestsDataToUpdate As New AllTestsByTypeDS

                For Each R As DataGridViewRow In LISMappingDataGridView.Rows
                    If R.Cells("Changed").Value Then
                        If Not R.Cells("HasError").Value Then
                            If IsDBNull(R.Cells("LISValue").Value) Then
                                R.Cells("LISValue").Value = ""
                            End If
                            If Not R.Cells("IsTest").Value Then
                                Dim myRow As LISMappingsDS.vcfgLISMappingRow = myMasterDataToUpdate.vcfgLISMapping.NewvcfgLISMappingRow
                                myRow.BeginEdit()
                                myRow.ValueType = CStr(R.Cells("ValueType").Value)
                                myRow.ValueId = CStr(R.Cells("ValueId").Value)
                                myRow.LISValue = CStr(R.Cells("LISValue").Value)
                                myRow.EndEdit()
                                myMasterDataToUpdate.vcfgLISMapping.AddvcfgLISMappingRow(myRow)
                            Else
                                Dim myRow As AllTestsByTypeDS.vparAllTestsByTypeRow = myTestsDataToUpdate.vparAllTestsByType.NewvparAllTestsByTypeRow
                                myRow.BeginEdit()
                                myRow.TestType = CStr(R.Cells("ValueType").Value)
                                myRow.TestID = CStr(R.Cells("ValueId").Value)
                                myRow.LISValue = CStr(R.Cells("LISValue").Value)
                                myRow.EndEdit()
                                myTestsDataToUpdate.vparAllTestsByType.AddvparAllTestsByTypeRow(myRow)
                            End If
                        End If
                    End If
                Next

                myMasterDataToUpdate.AcceptChanges()
                myTestsDataToUpdate.AcceptChanges()

                Dim isError As Boolean = False
                Dim savedItems As Integer = 0

                Dim myLISMasterMappingDelegate As New LISMappingsDelegate
                resultData = myLISMasterMappingDelegate.UpdateLISValues(Nothing, myMasterDataToUpdate)
                isError = resultData.HasError
                savedItems = resultData.AffectedRecords

                Dim myLISTestMappingDelegate As New AllTestByTypeDelegate
                resultData = myLISTestMappingDelegate.UpdateLISValues(Nothing, myTestsDataToUpdate)
                isError = isError Or resultData.HasError
                savedItems += resultData.AffectedRecords

                If Not isError AndAlso savedItems > 0 Then
                    isSavedOk = True

                    'clear all changed rows
                    For Each dr As DataGridViewRow In Me.LISMappingDataGridView.Rows
                        dr.Cells("Changed").Value = False
                    Next
                End If

            Else
                For Each dr As DataGridViewRow In Me.LISMappingDataGridView.Rows
                    If dr.Cells("HasError").Value Then
                        dr.Selected = True
                        dr.Cells("LISValue").Selected = True
                        Exit For
                    End If
                Next
                Me.LISMappingDataGridView.Focus()
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".SaveChanges", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".SaveChanges", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
        Return isSavedOk
    End Function

    ''' <summary>
    ''' Set the screen controls to EDIT MODE
    ''' </summary>
    ''' <remarks>
    ''' Created by XB 01/03/2013
    ''' </remarks>
    Private Sub EditModeScreenStatus()
        Try
            MyClass.IsEditionMode = True

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".EditModeScreenStatus", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".EditModeScreenStatus", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Execute the cancelling 
    ''' </summary>
    ''' <remarks>
    ''' Created by XB 01/03/2013
    '''          JCID 02/04/2013 No dialog result mustn't update changes flag, when user revert changes, must change changes flag
    ''' </remarks>
    Private Sub CancelEdition()
        Try
            If MyClass.IsAnyChanged Then
                If (ShowMessage(Me.Name, GlobalEnumerates.Messages.DISCARD_PENDING_CHANGES.ToString) = Windows.Forms.DialogResult.Yes) Then

                    MyClass.IsAnyChanged = False
                    MyClass.IsEditionMode = False
                    MyClass.MergeLISMappingData()
                    MyClass.FillLISMappingsGrid()

                    ' JCID 02/04/2013
                    'Else

                    'If MyClass.SaveChanges() Then
                    '    MyClass.IsAnyChanged = False
                    '    MyClass.IsEditionMode = False
                    '    MyClass.LoadConfigLISMappingData()
                    '    MyClass.LoadTestsLISMappingData()
                    '    MyClass.MergeLISMappingData()
                    '    MyClass.FillLISMappingsGrid()
                    'End If
                End If
            Else

                MyClass.IsEditionMode = False
                MyClass.MergeLISMappingData()
                MyClass.FillLISMappingsGrid()

            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".CancelControlEdition", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".CancelControlEdition", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' validates the informed row of the grid
    ''' </summary>
    ''' <param name="pCurrentRow"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' Created by SGM 05/03/2013
    ''' Modified by SGM 02/04/2013 - perform validation with the data from the grid and not from the loaded ds</remarks>
    Private Function ValidateGridRow(ByVal pCurrentRow As Integer) As Boolean
        Dim rowWithErrors As Boolean = False
        Try
            If MyClass.IsEditionMode Then

                Dim myRow As DataGridViewRow = Me.LISMappingDataGridView.Rows(pCurrentRow)
                MyClass.CheckGridRowIsLocked(myRow)

                If Not myRow.ReadOnly Then

                    'Clear previous errors...
                    myRow.Cells("LISValue").ErrorText = String.Empty
                    myRow.Cells("LISValue").Style.Alignment = DataGridViewContentAlignment.MiddleRight
                    Dim myLISValue As String = String.Empty
                    If Not IsDBNull(myRow.Cells("LISValue").Value) Then
                        myLISValue = CStr(myRow.Cells("LISValue").Value)
                    End If

                    If (Not rowWithErrors) Then

                        If myLISValue.Length = 0 Then
                            'Empty is a valid value
                        Else

                            Dim myValType As String = IIf(IsDBNull(myRow.Cells("ValueType").Value), String.Empty, CStr(myRow.Cells("ValueType").Value))

                            Dim isTest As Boolean = IIf(IsDBNull(myRow.Cells("IsTest").Value), False, myRow.Cells("IsTest").Value)
                            If Not isTest Then
                                'master data
                                'the new value entered must be UNIQUE for its own type
                                Dim myValID As String = IIf(IsDBNull(myRow.Cells("ValueId").Value), String.Empty, CStr(myRow.Cells("ValueId").Value))

                                If MyClass.EditingConfigLISMappingDS IsNot Nothing Then
                                    Dim myRows As List(Of LISMappingsDS.vcfgLISMappingRow)
                                    myRows = (From r In MyClass.EditingConfigLISMappingDS.vcfgLISMapping _
                                                 Where r.LISValue.ToUpperBS = myLISValue.ToUpperBS _
                                                 And (r.ValueType.ToUpperBS = myValType) _
                                                 And (r.ValueId.ToUpperBS <> myValID.ToUpperBS)
                                                 Select r).ToList()

                                    If myRows.Count > 0 Then
                                        rowWithErrors = True
                                        myRow.Cells("LISValue").ErrorText = GetMessageText(GlobalEnumerates.Messages.LIS_DUPLICATED_NAME.ToString) 'MSG_REPEATED_LIS_NAME
                                    End If
                                End If


                            Else

                                'validation to preserve unique LIS value for all test types
                                Dim myValID As String = IIf(IsNumeric(myRow.Cells("ValueId").Value), CStr(myRow.Cells("ValueId").Value), String.Empty)

                                If MyClass.EditingTestsLISMappingDS IsNot Nothing Then
                                    Dim myRows As List(Of AllTestsByTypeDS.vparAllTestsByTypeRow)
                                    myRows = (From r In MyClass.EditingTestsLISMappingDS.vparAllTestsByType _
                                                 Where r.LISValue.ToUpperBS = myLISValue.ToUpperBS _
                                                 And (((r.TestType.ToUpperBS = myValType) AndAlso (r.TestID.ToString <> myValID)) _
                                                 OrElse (r.TestType.ToUpperBS <> myValType)) _
                                                 Select r).ToList()

                                    If myRows.Count > 0 Then
                                        rowWithErrors = True
                                        myRow.Cells("LISValue").ErrorText = GetMessageText(GlobalEnumerates.Messages.LIS_DUPLICATED_NAME.ToString) 'MSG_REPEATED_LIS_NAME
                                    End If
                                End If

                            End If
                        End If
                    End If

                End If

                'If there are not errors in the grid row, clean all Error symbols
                If (Not rowWithErrors) Then
                    myRow.Cells("LISValue").ErrorText = String.Empty
                End If

                myRow.Cells("HasError").Value = rowWithErrors

                myRow.Cells("LISValue").Style.Alignment = DataGridViewContentAlignment.MiddleLeft

            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ValidateGridRow", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ValidateGridRow", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
        Return rowWithErrors
    End Function

    ''' <summary>
    ''' validates all the rows in the grid
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>Created by SGM 05/03/2013</remarks>
    Private Function ValidateAllGridRows() As Boolean
        Dim isError As Boolean = False
        Try
            For Each dr As DataGridViewRow In Me.LISMappingDataGridView.Rows
                If dr.Cells("HasError").Value Then
                    isError = True
                    Exit For
                End If
            Next
            'Me.SaveButton.Enabled = (IsEditionMode And Not IsRowEditing And Not isError)
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ValidateAllGridRows " & Name, EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ValidateAllGridRows ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
        Return isError
    End Function

    ''' <summary>
    ''' Encodes a string to a valid string for XML
    ''' </summary>
    ''' <param name="pString"></param>
    ''' <returns></returns>
    ''' <remarks>Created by SGM 05/03/2012</remarks>
    Private Function EncodeSpecialCharsForXML(ByVal pString As String) As String
        Dim res As String = ""
        Try
            Dim myChars() As Char = pString.ToCharArray
            For c As Integer = 0 To myChars.Length - 1 Step 1
                Select Case myChars(c)
                    Case "&" : res &= "&#x26;"
                    Case "<" : res &= "&#x60;"
                    Case ">" : res &= "&#x62;"
                    Case Else : res &= myChars(c)
                End Select
            Next

        Catch ex As Exception
            res = ""
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".EncodeSpecialCharsForXML", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".EncodeSpecialCharsForXML", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
        Return res
    End Function

    ''' <summary>
    ''' Decodes a string from a valid string for XML
    ''' </summary>
    ''' <param name="pString"></param>
    ''' <returns></returns>
    ''' <remarks>Created by SGM 05/03/2012</remarks>
    Private Function DecodeSpecialCharsForXML(ByVal pString As String) As String
        Dim res As String = pString
        Try

            If pString.Contains("&#x26;") Then
                res = res.Replace("&#x26;", "&")
            End If
            If pString.Contains("&#x60;") Then
                res = res.Replace("&#x60;", "<")
            End If
            If pString.Contains("&#x26;") Then
                res = res.Replace("&#x62;", ">")
            End If


        Catch ex As Exception
            res = ""
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".DecodeSpecialCharsForXML", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".DecodeSpecialCharsForXML", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
        Return res
    End Function

    ''' <summary>
    ''' Set the screen status by the User level.
    ''' </summary>
    ''' <remarks>
    ''' CREATED BY: TR 05/03/2013
    ''' </remarks>
    Private Sub ScreenStatusByUserLevel()
        Try
            Select Case CurrentUserLevel
                Case "SUPERVISOR"
                    Exit Select

                Case "OPERATOR"
                    Me.EditButton.Enabled = False
                    Me.ButtonCancel.Enabled = False
                    'Me.FilterComboBox.Enabled = False
                    Me.SaveButton.Enabled = False
                    Me.LISMappingDataGridView.ReadOnly = True
                    For i As Integer = 0 To Me.LISMappingDataGridView.Rows.Count - 1
                        Me.LISMappingDataGridView.Rows(i).Cells("LISValue").Style.BackColor = Color.WhiteSmoke
                    Next i
                    Exit Select
            End Select

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & "ScreenStatusByUserLevel ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & "ScreenStatusByUserLevel ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' updates the local copy for validation to preserve unique LIS value for each type
    ''' </summary>
    ''' <remarks>SGM 04/04/2013</remarks>
    Private Sub UpdateEditingConfigLISMappingDS()
        Try

            For Each r As DataGridViewRow In Me.LISMappingDataGridView.Rows
                If Not IsDBNull(r.Cells("IsTest").Value) AndAlso Not CBool(r.Cells("IsTest").Value) Then

                    Dim myValID As String = IIf(IsDBNull(r.Cells("ValueID").Value), String.Empty, r.Cells("ValueID").Value)
                    Dim myValType As String = IIf(IsDBNull(r.Cells("ValueType").Value), String.Empty, r.Cells("ValueType").Value)
                    Dim myLISValue As String = IIf(IsDBNull(r.Cells("LISValue").Value), String.Empty, r.Cells("LISValue").Value)

                    Dim myRows As List(Of LISMappingsDS.vcfgLISMappingRow)
                    myRows = (From dr In MyClass.EditingConfigLISMappingDS.vcfgLISMapping _
                                 Where dr.ValueType.ToUpperBS = myValType.ToUpperBS _
                                 And dr.ValueId.ToUpperBS = myValID.ToUpperBS _
                                 Select dr).ToList()

                    If myRows.Count > 0 Then
                        Dim myRow As LISMappingsDS.vcfgLISMappingRow = myRows(0)
                        With myRow
                            .BeginEdit()
                            .LISValue = myLISValue
                            .EndEdit()
                        End With
                    End If
                End If
            Next

            MyClass.EditingConfigLISMappingDS.AcceptChanges()

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".UpdateEditingConfigLISMappingDS", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".UpdateEditingConfigLISMappingDS", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' updates the local copy for validation to preserve unique LIS value for all test types
    ''' </summary>
    ''' <remarks>SGM 04/04/2013</remarks>
    Private Sub UpdateEditingTestsLISMappingDS()
        Try

            For Each r As DataGridViewRow In Me.LISMappingDataGridView.Rows
                If Not IsDBNull(r.Cells("IsTest").Value) AndAlso CBool(r.Cells("IsTest").Value) Then

                    Dim myValID As Integer = IIf(IsNumeric(r.Cells("ValueID").Value), CInt(r.Cells("ValueID").Value), 0)
                    Dim myTestName As String = IIf(IsDBNull(r.Cells("LongName").Value), String.Empty, r.Cells("LongName").Value)
                    Dim myLISValue As String = IIf(IsDBNull(r.Cells("LISValue").Value), String.Empty, r.Cells("LISValue").Value)

                    Dim myRows As List(Of AllTestsByTypeDS.vparAllTestsByTypeRow)
                    myRows = (From dr In MyClass.EditingTestsLISMappingDS.vparAllTestsByType _
                                 Where dr.TestName.ToUpperBS = myTestName.ToUpperBS _
                                 And dr.TestID = CInt(myValID) _
                                 Select dr).ToList()

                    If myRows.Count > 0 Then
                        Dim myRow As AllTestsByTypeDS.vparAllTestsByTypeRow = myRows(0)
                        With myRow
                            .BeginEdit()
                            .LISValue = myLISValue
                            .EndEdit()
                        End With
                    End If
                End If
            Next

            MyClass.EditingTestsLISMappingDS.AcceptChanges()

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".UpdateEditingTestsLISMappingDS", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".UpdateEditingTestsLISMappingDS", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Checks if the informed datarow is locked for edition
    ''' </summary>
    ''' <param name="pDataRow"></param>
    ''' <remarks></remarks>
    Private Sub CheckGridRowIsLocked(ByRef pDataRow As DataGridViewRow)
        Try

            Dim ReadOnlyRow As Boolean = False

            ReadOnlyRow = (MyClass.CheckGridRowInUse(pDataRow) Or MyClass.CheckCalcTestIsMultiSampleType(pDataRow))

            pDataRow.ReadOnly = ReadOnlyRow
            If ReadOnlyRow Then
                pDataRow.DefaultCellStyle.ForeColor = Color.DarkGray
            Else
                pDataRow.DefaultCellStyle.ForeColor = Color.Black
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".CheckGridRowIsLocked", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".CheckGridRowIsLocked", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Sets readonly/editable the informed datarow because the informed item is in use
    ''' </summary>
    ''' <remarks>SGM 05/04/2013</remarks>
    Private Function CheckGridRowInUse(ByRef pDataRow As DataGridViewRow) As Boolean
        Dim ReadOnlyRow As Boolean
        Try
            Dim resultData As New GlobalDataTO
            Dim IsInUse As Boolean = False
            If pDataRow IsNot Nothing Then

                Dim isTest As Boolean = IIf(IsDBNull(pDataRow.Cells("IsTest").Value), False, pDataRow.Cells("IsTest").Value)
                If isTest Then
                    IsInUse = IIf(IsDBNull(pDataRow.Cells("InUse").Value), False, CBool(pDataRow.Cells("InUse").Value))

                    'Pending to accept
                    ''SGM 09/05/2013 - Allow to Edit in case of Mapping not defined yet
                    'If IsInUse Then
                    '    If pDataRow.Cells("LISValue").Value Is String.Empty Then
                    '        IsInUse = False
                    '    End If
                    'End If
                    'Pending to accept
                Else
                    Dim myValType As String = IIf(IsDBNull(pDataRow.Cells("ValueType").Value), String.Empty, CStr(pDataRow.Cells("ValueType").Value))
                    If myValType.ToUpperBS = "SAMPLE_TYPES" Then
                        Dim myValID As String = IIf(IsDBNull(pDataRow.Cells("ValueID").Value), String.Empty, CStr(pDataRow.Cells("ValueID").Value))
                        IsInUse = MyClass.mySampleTypesInWS.Contains(myValID)
                    End If
                End If

                pDataRow.Cells("InUse").Value = IsInUse

                'update readonly
                ReadOnlyRow = IsInUse

            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".CheckGridRowInUse", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".CheckGridRowInUse", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try

        Return ReadOnlyRow
    End Function

    ''' <summary>
    ''' Checks if the user can edit some row in the grid. 
    ''' In case of the item refers to a Calculated Test that contains multiple SampleTypes the row is read only
    ''' </summary>
    ''' <param name="pDataRow"></param>
    ''' <remarks>
    ''' Created by SGM 02/05/2013
    ''' </remarks>
    Private Function CheckCalcTestIsMultiSampleType(ByRef pDataRow As DataGridViewRow)
        Dim ReadOnlyRow As Boolean
        Try
            Dim resultData As New GlobalDataTO
            Dim IsUniqueSampleType As Boolean = True
            If pDataRow IsNot Nothing Then

                Dim isTest As Boolean = IIf(IsDBNull(pDataRow.Cells("IsTest").Value), False, pDataRow.Cells("IsTest").Value)
                If isTest Then
                    Dim isCalcTest As Boolean = IIf(IsDBNull(pDataRow.Cells("IsCalcTest").Value), False, pDataRow.Cells("IsCalcTest").Value)
                    If isCalcTest Then
                        IsUniqueSampleType = IIf(IsDBNull(pDataRow.Cells("UniqueSampleType").Value), True, CBool(pDataRow.Cells("UniqueSampleType").Value))
                    End If
                End If

                pDataRow.Cells("UniqueSampleType").Value = IsUniqueSampleType

                'update readonly
                ReadOnlyRow = Not IsUniqueSampleType

            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".CheckCalcTestIsMultiSampleType", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".CheckCalcTestIsMultiSampleType", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
        Return ReadOnlyRow
    End Function

#End Region


#Region "Events Handlers"

    Private Sub IConfigLISMappingLoad(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            ScreenLoad()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".IConfigLISMappingLoad ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".IConfigLISMappingLoad", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub IConfigLISMapping_Shown(sender As Object, e As EventArgs) Handles Me.Shown
        Try
            'prepares the grid
            MyClass.PrepareLISMappingsGrid()

            'fills the data to the grid
            MyClass.FillLISMappingsGrid()

            MyClass.InitialModeScreenStatus()

            'Dim myGlobalbase As New GlobalBase
            CurrentUserLevel = GlobalBase.GetSessionInfo.UserLevel
            ScreenStatusByUserLevel()

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".Shown ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".Shown", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    'avoid to change selected item on mouse wheel
    Private Sub FilterComboBox_MouseWheel(sender As Object, e As MouseEventArgs) Handles FilterComboBox.MouseWheel
        Dim mwe As HandledMouseEventArgs = DirectCast(e, HandledMouseEventArgs)
        mwe.Handled = True
    End Sub

    Private Sub LISMappingTypeComboBox_SelectedIndexChanged(sender As Object, e As EventArgs) Handles FilterComboBox.SelectedIndexChanged
        Try

            If Not ScreenIsLoading Then
                If Not MyClass.IsEditionMode Then
                    Me.LISMappingDataGridView.DataSource = Nothing
                    MyClass.FillLISMappingsGrid()
                End If
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".LISMappingTypeComboBox.SelectedIndexChanged", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".LISMappingTypeComboBox.SelectedIndexChanged", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub


    Private Sub LISMappingDataGridView_CellBeginEdit(sender As Object, e As DataGridViewCellCancelEventArgs) Handles LISMappingDataGridView.CellBeginEdit
        Try
            Dim myDgv As DataGridView = TryCast(sender, DataGridView)
            Dim myRow As DataGridViewRow = myDgv.Rows(e.RowIndex)
            MyClass.CheckGridRowIsLocked(myRow)
            If myRow.ReadOnly Then
                e.Cancel = True
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".LISMappingDataGridView_CellBeginEdit", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".LISMappingDataGridView_CellBeginEdit", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub



    Private Sub LISMappingDataGridView_CellValueChanged(sender As Object, e As DataGridViewCellEventArgs) Handles LISMappingDataGridView.CellValueChanged
        Try
            If Not ScreenIsLoading And IsEditionMode Then
                Dim myDgv As DataGridView = TryCast(sender, DataGridView)
                For Each dr As DataGridViewRow In myDgv.Rows
                    If dr.Cells("LISValue").ColumnIndex = e.ColumnIndex Then
                        If dr.Index = e.RowIndex Then
                            dr.Cells("Changed").Value = True
                            IsAnyChanged = True
                            Exit For
                        End If
                    End If
                Next
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".LISMappingDataGridView_CellValueChanged", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".LISMappingDataGridView_CellValueChanged", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    Private Sub LISMappingDataGridView_RowValidating(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellCancelEventArgs) Handles LISMappingDataGridView.RowValidating
        Dim IsAllOK As Boolean
        Try
            If Not ScreenIsLoading And IsEditionMode Then
                ValidateGridRow(e.RowIndex)
                IsAllOK = IsAllValidatedOK
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".LISMappingDataGridView.RowValidating", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".LISMappingDataGridView.RowValidating", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try

        Me.SaveButton.Enabled = (IsEditionMode And IsAllOK)

    End Sub

    Private Sub LISMappingDataGridView_RowValidated(sender As Object, e As DataGridViewCellEventArgs) Handles LISMappingDataGridView.RowValidated
        Try
            If Not MyClass.IsAnyChanged Then
                MyClass.IsEditionMode = False
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".LISMappingDataGridView_RowValidated", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".LISMappingDataGridView_RowValidated", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    Private Sub LISMappingDataGridView_CellPainting(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellPaintingEventArgs) Handles LISMappingDataGridView.CellPainting
        Try
            If EditButton.Enabled Then

                If e.RowIndex > -1 Then
                    e.CellStyle.BackColor = Color.WhiteSmoke

                    For i As Integer = 0 To LISMappingDataGridView.Columns.Count - 1
                        LISMappingDataGridView.Columns(i).ReadOnly = True
                    Next i
                End If

            Else

                If e.RowIndex > -1 Then
                    LISMappingDataGridView.Rows(e.RowIndex).Cells("LISValue").ReadOnly = False

                    If (e.ColumnIndex = LISMappingDataGridView.Columns("ValueType").Index OrElse e.ColumnIndex = LISMappingDataGridView.Columns("LongName").Index) Then
                        e.CellStyle.BackColor = Color.WhiteSmoke
                    End If
                End If
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".LISMappingDataGridView.CellPainting", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".LISMappingDataGridView.CellPainting", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    Private Sub IConfigLISMapping_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyDown
        Try
            If (e.KeyCode = Keys.Escape) Then
                If (ButtonCancel.Enabled) Then
                    ButtonCancel.PerformClick()
                Else
                    CloseButton.PerformClick()
                End If
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".IConfigLISMapping_KeyDown", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".IConfigLISMapping_KeyDown", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Set the screen status to EDIT MODE 
    ''' </summary>
    ''' <remarks>
    ''' Created by XB 01/03/2013
    ''' </remarks>
    Private Sub EditButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles EditButton.Click
        Try
            EditModeScreenStatus()

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".EditButton_Click", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".EditButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Save changes (update) in the database
    ''' </summary>
    ''' <remarks>
    ''' Created by XB 01/03/2013
    ''' </remarks>   
    Private Sub SaveButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SaveButton.Click
        Try
            If MyClass.SaveChanges() Then
                MyClass.IsAnyChanged = False
                MyClass.IsEditionMode = False
                MyClass.LoadConfigLISMappingData()
                MyClass.LoadTestsLISMappingData()
                MyClass.MergeLISMappingData()
                MyClass.FillLISMappingsGrid()
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".SaveButton_Click", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".SaveButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Execute the cancelling of settings edition
    ''' </summary>
    ''' <remarks>
    ''' Created by XB 01/03/2013
    ''' </remarks>    
    Private Sub CancelButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButtonCancel.Click
        Try
            CancelEdition()

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".CancelButton_Click", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".CancelButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Close form
    ''' </summary>
    ''' <remarks>
    ''' Created by XB 01/03/2013
    ''' Modified by SGM 02/04/2013 - Do nothing in case of not discarding changes
    ''' </remarks>
    Private Sub CloseButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CloseButton.Click
        Try
            Dim screenClose As Boolean = False

            If MyClass.IsAnyChanged Then
                If (ShowMessage(Me.Name, GlobalEnumerates.Messages.DISCARD_PENDING_CHANGES.ToString) = DialogResult.Yes) Then
                    Me.Enabled = False ' Disable the form to avoid pressing buttons on closing
                    screenClose = True
                Else
                    'SGM 02/04/2013
                    'If MyClass.SaveChanges() Then
                    '    screenClose = True
                    'End If
                End If
            Else
                Me.Enabled = False ' Disable the form to avoid pressing buttons on closing
                screenClose = True
            End If

            If screenClose Then
                If Not Tag Is Nothing Then
                    'A PerformClick() method was executed
                    Close()
                Else
                    'Normal button click - Open the WS Monitor form and close this one
                    IAx00MainMDI.OpenMonitorForm(Me)
                End If
            End If


        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".CloseButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".CloseButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try

    End Sub

    Private Sub LISMappingDataGridView_CellDoubleClick(sender As Object, e As DataGridViewCellEventArgs) Handles LISMappingDataGridView.CellDoubleClick
        Try

            Dim dgv As DataGridView = TryCast(sender, DataGridView)
            If dgv IsNot Nothing Then
                If e.RowIndex >= 0 Then
                    If Not dgv.Rows(e.RowIndex).ReadOnly Then
                        EditModeScreenStatus()
                        If dgv.SelectedCells.Count > 0 Then
                            dgv.SelectedCells(dgv.Columns("LISValue").Index).Selected = True
                        End If
                    End If
                End If
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".LISMappingDataGridView_CellDoubleClick ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".LISMappingDataGridView_CellDoubleClick", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

#End Region



End Class