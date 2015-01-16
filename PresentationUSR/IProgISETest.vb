Option Strict On
Option Explicit On

Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global.TO
Imports Biosystems.Ax00.Controls.UserControls
Imports Biosystems.Ax00.Global.GlobalEnumerates
'Imports Biosystems.Ax00.Types.AllowedTestsDS

Public Class IProgISETest


#Region "Attributes"
    Private SourceScreenAttribute As String = GlbSourceScreen.STANDARD.ToString
    Private WorkSessionIDAttribute As String
    Private AnalyzerIDAttribute As String
#End Region

#Region "Properties"

    Public Property AnalyzerID() As String
        Get
            Return AnalyzerIDAttribute
        End Get
        Set(ByVal value As String)
            AnalyzerIDAttribute = value
        End Set
    End Property

    Public Property WorkSessionID() As String
        Get
            Return WorkSessionIDAttribute
        End Get
        Set(ByVal value As String)
            WorkSessionIDAttribute = value
        End Set
    End Property
#End Region


#Region "Declarations"

    Private EditionMode As Boolean '= False
    Private SelectedISETestID As Integer '= 0         'To store the ID of the selected ISE Test
    Private SelectedSampleType As String '= ""        'To store the Code of the SampleType shown for the selected ISE Test
    Private OriginalISETestName As String '= ""       'To store the Name of the selected ISE Test and control Pending Changes
    Private OriginalSelectedIndex As Integer = -1    'To store the index of the selected ISE Test and control Pending Changes
    Private SelectedISETestSamplesDS As ISETestSamplesDS    'To manage the list of Sample Types defined for the selected ISE Test
    Private SelectedTestRefRangesDS As New TestRefRangesDS  'To manage the Reference Ranges for the selected ISE Test

    'TR 23/05/2011
    Private MinAllowedConcentration As Single              'To store the minimum allowed value for Min/Max Concentration fields
    Private MaxAllowedConcentration As Single              'To store the maximum allowed value for Min/Max Concentration fields
    'TR 23/05/2011

    'TR 07/04/2011
    Private ControlListDS As New ControlsDS
    Private SelectedTestControlDS As New TestControlsDS
    'Private ControlsListDS As New ControlsDS
    Private ControIDSel As String = ""
    'TR 07/04/2011 -END

    Private currentLanguage As String = "" 'TR 01/03/2011
    Private ValidationError As Boolean

    'TR 24/05/2011 
    Private LocalDeleteControlTOList As List(Of DeletedControlTO)
    'TR 24/05/2011 

    Private ChangesMade As Boolean
    Private SelectedTestSampleMultirulesDS As New TestSamplesMultirulesDS
    Private CUMULATE_QCRESULTS_Label As String
#End Region

#Region "Constructor"

    Public Sub New()
        'This call is required by the Windows Form Designer
        InitializeComponent()
    End Sub

#End Region

#Region "Methods"
    ''' <summary>
    ''' Fill screen fields with data of the selected ISE Test
    ''' </summary>
    ''' <returns>True if the ISE Test is In Use; otherwise False</returns>
    ''' <remarks>
    ''' Modified by: SG 20/10/2010 
    ''' Modified by: SA 17/05/2012 - An ISE Test will be marked as in use when InUse flag is TRUE OR Enabled flag is FALSE
    '''                              CheckBox indicating if the ISE Test is available is visible (although disabled) only for Li+, 
    '''                              due to activation/deactivation of Li+ has to be done from ISE Utilities Screen
    ''' JB 26/09/2012: Reverted: Modified by: JB 26/09/2012 - Removed the activation/deactivation Li+ electrode verification
    ''' </remarks>
    Private Function BindISETestData() As Boolean
        Dim inUse As Boolean = False

        Try
            SelectedISETestID = CInt(bsISETestListView.SelectedItems(0).SubItems(1).Text)
            OriginalISETestName = bsISETestListView.SelectedItems(0).SubItems(0).Text
            OriginalSelectedIndex = bsISETestListView.SelectedIndices(0)

            'Fill screen controls with data of the selected ISE Test
            bsFullNameTextbox.Text = bsISETestListView.SelectedItems(0).SubItems(0).Text
            bsShortNameTextbox.Text = bsISETestListView.SelectedItems(0).SubItems(3).Text
            bsUnitComboBox.SelectedValue = bsISETestListView.SelectedItems(0).SubItems(4).Text

            If CType(bsISETestListView.SelectedItems(0).SubItems(6).Text, Boolean) Then
                bsAvailableISETestCheckBox.CheckState = CheckState.Checked
            Else
                bsAvailableISETestCheckBox.CheckState = CheckState.Unchecked
            End If
            bsAvailableISETestCheckBox.Enabled = False
            bsAvailableISETestCheckBox.Visible = (String.Compare(bsISETestListView.SelectedItems(0).SubItems(3).Text, "Li+", False) = 0)

            'JB 26/09/2012 - Reverted - JB 26/09/2012 - Remove the disabled electrode verification
            'inUse = CType(bsISETestListView.SelectedItems(0).SubItems(5).Text, Boolean) 
            'ISE Test is not available when it is InUse in the active WorkSession OR (do not change for OrElse!!)
            'it has been disabled in the ISE Utilities Screen (this is possible only for Li+)
            inUse = CType(bsISETestListView.SelectedItems(0).SubItems(5).Text, Boolean) Or _
                    Not CType(bsISETestListView.SelectedItems(0).SubItems(6).Text, Boolean)

            'Get the Sample Types linked to the selected ISE Test and load them in the ComboBox
            LoadSampleTypesList()

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".BindISETestData", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".BindISETestData", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
        Return inUse
    End Function



    ''' <summary>
    ''' Binds ISE Test Samples data to the corresponding screen fields.
    ''' </summary>
    ''' <remarks>
    ''' Created by: WE 30/07/2014 - #1865
    ''' </remarks>
    Private Sub BindISETestSamplesData(ByVal pTestID As Integer, ByVal pSampleType As String)

        Try
            'For the selected ISE TestID/SampleType get data of field TestLongName, Decimals, SlopeFactorA2 and SlopeFactorB2
            'from global DataSet SelectedISETestSamplesDS (loaded in function LoadSampleTypesList).
            Dim qTestSamples As List(Of ISETestSamplesDS.tparISETestSamplesRow)

            qTestSamples = (From a In SelectedISETestSamplesDS.tparISETestSamples _
                            Where String.Compare(a.SampleType, pSampleType, False) = 0 _
                            AndAlso a.ISETestID = pTestID _
                            Select a).ToList()

            If Not qTestSamples.First().IsTestLongNameNull Then
                bsReportNameTextBox.Text = qTestSamples.First().TestLongName
            Else
                bsReportNameTextBox.ResetText()
            End If

            bsDecimalsUpDown.Text = qTestSamples.First().Decimals.ToString()
            
            ' SlopeFactorA2 and SlopeFactorB2
            If qTestSamples.First().IsSlopeFactorA2Null Then
                bsSlopeA2UpDown.Value = 0
                bsSlopeA2UpDown.ResetText()
            Else
                bsSlopeA2UpDown.Text = CType(qTestSamples.First().SlopeFactorA2, Decimal).ToString()
            End If

            If qTestSamples.First().IsSlopeFactorB2Null Then
                bsSlopeB2UpDown.Value = 0
                bsSlopeB2UpDown.ResetText()
            Else
                bsSlopeB2UpDown.Text = CType(qTestSamples.First().SlopeFactorB2, Decimal).ToString()
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "BindISETestSamplesData " & Name, EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Execute the cancelling of a ISE Test edition
    ''' </summary>
    ''' <remarks>
    ''' Modified by: RH 05/06/2012
    ''' </remarks>
    Private Sub CancelISETestEdition()
        Try
            Dim setScreenToInitial As Boolean = False

            If (PendingChangesVerification()) Then
                If (ShowMessage(Me.Name, GlobalEnumerates.Messages.DISCARD_PENDING_CHANGES.ToString) = DialogResult.Yes) Then
                    SelectedTestRefRangesDS.Clear()
                    setScreenToInitial = True
                Else
                    If (OriginalSelectedIndex <> -1) Then
                        'Return focus to the ISE Test that has been edited
                        bsISETestListView.Items(OriginalSelectedIndex).Selected = True
                        bsISETestListView.Select()
                    End If
                End If
            Else
                setScreenToInitial = True
            End If

            If (setScreenToInitial) Then
                EditionMode = False
                ChangesMade = False

                If (OriginalSelectedIndex <> -1) Then
                    bsISETestListView.Items(OriginalSelectedIndex).Selected = True
                Else
                    'Set screen status to Initial Mode
                    InitialModeScreenStatus()
                    If (bsISETestListView.Items.Count > 0) Then
                        'Select the first ISE Test in the list
                        bsISETestListView.Items(0).Selected = True
                    End If
                End If

                If (bsISETestListView.SelectedItems.Count > 0) Then
                    'Load screen fields with all data of the selected ISE Test 
                    Dim inUseISETest As Boolean = BindISETestData()

                    BindISETestSamplesData(SelectedISETestID, SelectedSampleType)    ' WE 30/07/2014 - #1865

                    BindISETestQCData(SelectedISETestID, SelectedSampleType)

                    'Get the Reference Ranges defined for the ISE Test and shown them
                    GetRefRanges(SelectedISETestID)
                    LoadRefRangesData()

                    If (Not inUseISETest) Then
                        'Set screen status to Query Mode
                        QueryModeScreenStatus()
                    Else
                        'Set screen status to Read Only Mode 
                        ReadOnlyModeScreenStatus()
                    End If
                    bsISETestListView.Focus()
                End If
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".CancelISETestEdition", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".CancelISETestEdition", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Initialize the Global Variables used in the screen to store the selected ISE Test
    ''' and/or to control when there are changes pending to save
    ''' </summary>
    ''' <remarks>
    ''' Modified by: SG 20/10/2010
    ''' </remarks>
    Private Sub CleanGlobalValues()
        Try
            'Initialization of global variables
            SelectedISETestID = 0
            SelectedSampleType = ""
            OriginalSelectedIndex = -1
            OriginalISETestName = ""
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".CleanGlobalValues", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".CleanGlobalValues", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    '''' <summary>
    '''' When the Edit Button is clicked, set the screen status to EDIT MODE
    '''' </summary>
    '''' <remarks>
    '''' Modified by: SG 20/10/2010
    ''''              SA 12/01/2011 - Changes due to new implementation of Reference Ranges Control 
    '''' </remarks>
    'Private Sub EditISETestByButtonClick()
    '    Try
    '        EditionMode = True
    '        If (Not bsShortNameTextbox.Enabled) Then
    '            'Set screen status to Edit Mode
    '            EditModeScreenStatus()
    '        End If
    '    Catch ex As Exception
    '        CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".EditISETestByButtonClick", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
    '        ShowMessage(Me.Name & ".EditISETestByButtonClick", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
    '    End Try
    'End Sub

    '''' <summary>
    '''' When double clicking in a ISE Test in the ListView, verify if there are changes pending 
    '''' to save and set the screen status to EDIT MODE if it is not InUse, or to READ ONLY MODE if it 
    '''' is InUse in the active Work Session
    '''' </summary>
    '''' <remarks>
    '''' Modified by: RH 05/06/2012
    '''' </remarks>
    'Private Sub EditISETestByDoubleClick()
    '    Dim setScreenToEdition As Boolean = False

    '    Try
    '        EditionMode = True

    '        If Not CurrentUserLevel = "OPERATOR" Then ' TR 23/04/2012 -Validate if operator.
    '            If (bsISETestListView.SelectedIndices(0) <> OriginalSelectedIndex) Then
    '                If (PendingChangesVerification()) Then
    '                    If (ShowMessage(Me.Name, GlobalEnumerates.Messages.DISCARD_PENDING_CHANGES.ToString) = DialogResult.Yes) Then
    '                        SelectedTestRefRangesDS.Clear()
    '                        setScreenToEdition = True
    '                    Else
    '                        If (OriginalSelectedIndex <> -1) Then
    '                            'Return focus to the ISE Test that has been edited
    '                            bsISETestListView.Items(OriginalSelectedIndex).Selected = True
    '                            bsISETestListView.Select()
    '                        End If
    '                    End If
    '                Else
    '                    setScreenToEdition = True
    '                End If
    '            Else
    '                'If the screen was in Read Only Mode, it is changed to Edit Mode although the User
    '                'double-clicking in the same ISE Test 
    '                If (Not bsShortNameTextbox.Enabled) Then setScreenToEdition = True
    '            End If

    '            If (setScreenToEdition) Then
    '                'Load screen fields with all data of the selected ISE Test
    '                Dim inUseISETest As Boolean = BindISETestData()

    '                BindISETestQCData(SelectedISETestID, SelectedSampleType)

    '                'Get the Reference Ranges defined for the ISE Test and the selected SampleType and shown them
    '                GetRefRanges(SelectedISETestID)
    '                LoadRefRangesData()

    '                'Load screen fields with all data of the selected ISE Test
    '                If (Not inUseISETest) Then
    '                    'Set screen status to Edit Mode
    '                    EditModeScreenStatus()
    '                Else
    '                    'Set screen status to Read Only Mode
    '                    ReadOnlyModeScreenStatus()
    '                End If
    '            End If
    '        End If

    '    Catch ex As Exception
    '        CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".EditISETestByDoubleClick", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
    '        ShowMessage(Me.Name & ".EditISETestByDoubleClick", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
    '    End Try
    'End Sub

    ''' <summary>
    ''' When double clicking in a ISE Test in the ListView, verify if there are changes pending 
    ''' to save and set the screen status to EDIT MODE if it is not InUse, or to READ ONLY MODE if it 
    ''' is InUse in the active Work Session
    ''' </summary>
    ''' <remarks>
    ''' Modified by: RH 05/06/2012
    ''' </remarks>
    Private Sub EnableEdition()
        Dim setScreenToEdition As Boolean = False

        Try
            If Not String.Compare(CurrentUserLevel, "OPERATOR", False) = 0 Then ' TR 23/04/2012 -Validate if operator.
                If (bsISETestListView.SelectedIndices(0) <> OriginalSelectedIndex) Then
                    If (PendingChangesVerification()) Then
                        If (ShowMessage(Me.Name, GlobalEnumerates.Messages.DISCARD_PENDING_CHANGES.ToString) = DialogResult.Yes) Then
                            SelectedTestRefRangesDS.Clear()
                            setScreenToEdition = True
                        Else
                            If (OriginalSelectedIndex <> -1) Then
                                'Return focus to the ISE Test that has been edited
                                bsISETestListView.Items(OriginalSelectedIndex).Selected = True
                                bsISETestListView.Select()
                            End If
                        End If
                    Else
                        setScreenToEdition = True
                    End If
                Else
                    'If the screen was in Read Only Mode, it is changed to Edit Mode although the User
                    'double-clicking in the same ISE Test 
                    If (Not bsShortNameTextbox.Enabled) Then setScreenToEdition = True
                End If

                If (setScreenToEdition) Then
                    EditionMode = True
                    ChangesMade = False

                    'Reset LocalDeleteControlTOList
                    LocalDeleteControlTOList = New List(Of DeletedControlTO)

                    Dim SaveSelectedIndex As Integer = bsSampleTypeComboBox.SelectedIndex

                    'Load screen fields with all data of the selected ISE Test
                    Dim inUseISETest As Boolean = BindISETestData()

                    If (bsISETestListView.SelectedIndices(0) = OriginalSelectedIndex) AndAlso SaveSelectedIndex >= 0 Then
                        bsSampleTypeComboBox.SelectedIndex = SaveSelectedIndex
                        SelectedSampleType = bsSampleTypeComboBox.SelectedValue.ToString()
                    End If

                    BindISETestSamplesData(SelectedISETestID, SelectedSampleType)    ' WE 30/07/2014 - #1865

                    BindISETestQCData(SelectedISETestID, SelectedSampleType)

                    'Get the Reference Ranges defined for the ISE Test and the selected SampleType and shown them
                    GetRefRanges(SelectedISETestID)
                    LoadRefRangesData()

                    'Load screen fields with all data of the selected ISE Test
                    If (Not inUseISETest) Then
                        'Set screen status to Edit Mode
                        EditModeScreenStatus()
                    Else
                        'Set screen status to Read Only Mode
                        ReadOnlyModeScreenStatus()
                    End If
                End If
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".EnableEdition", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".EnableEdition", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Set the screen controls to EDIT MODE
    ''' </summary>
    ''' <remarks>
    ''' Modified by: RH 05/06/2012
    ''' </remarks>
    Private Sub EditModeScreenStatus()
        Try
            'Area of ISE Test List
            bsEditButton.Enabled = False
            bsCustomOrderButton.Enabled = False 'AG 05/09/2014 - BA-1869

            ' WE 29/07/2014 - #1865 
            bsFullNameTextbox.Enabled = True
            bsFullNameTextbox.BackColor = Color.White
            ' WE 29/07/2014 - #1865 - End

            bsShortNameTextbox.Enabled = True
            bsShortNameTextbox.BackColor = Color.White

            bsAvailableISETestCheckBox.Enabled = False

            ' WE 30/07/2014 - #1865 
            bsReportNameTextBox.Enabled = True
            bsReportNameTextBox.BackColor = Color.White

            bsDecimalsUpDown.Enabled = True
            bsDecimalsUpDown.BackColor = Color.White
            ' WE 30/07/2014 - #1865 - End

            bsSaveButton.Enabled = True
            bsCancelButton.Enabled = True

            bsTestRefRanges.isEditing = True

            bsQCPanel.Enabled = True

            ' WE 01/08/2014 - #1865
            bsSlopeA2UpDown.Enabled = True
            bsSlopeA2UpDown.BackColor = Color.White
            bsSlopeB2UpDown.Enabled = True
            bsSlopeB2UpDown.BackColor = Color.White
            ' WE 01/08/2014 - #1865 - End

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".EditModeScreenStatus", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".EditModeScreenStatus", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Close the screen, verifying first if there are changes pending to save
    ''' </summary>
    Private Sub ExitScreen()
        Dim screenClose As Boolean = True

        Try
            If EditionMode Then
                If PendingChangesVerification() Then
                    If ShowMessage(Me.Name, GlobalEnumerates.Messages.DISCARD_PENDING_CHANGES.ToString) = DialogResult.No Then
                        screenClose = False
                    End If
                End If
            End If

            If (screenClose) Then
                'TR 11/04/2012 -Disable form on close to avoid any button press.
                Me.Enabled = False

                If Not Me.Tag Is Nothing Then
                    'A PerformClick() method was executed
                    Me.Close()
                Else
                    'Normal button click
                    'Open the WS Monitor form and close this one
                    IAx00MainMDI.OpenMonitorForm(Me)
                End If
            Else
                If bsFullNameTextbox.Enabled Then
                    bsFullNameTextbox.Focus()
                ElseIf bsShortNameTextbox.Enabled Then
                    bsShortNameTextbox.Focus()
                End If
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ExitScreen", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ExitScreen", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Returns the list of AgeUnits
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>
    ''' Created by:  SG 20/10/2010
    ''' </remarks>
    Private Function GetAgeUnits() As PreloadedMasterDataDS
        Try
            Dim myGlobalDataTO As New GlobalDataTO
            Dim myPreloadedMasterDataDelegate As New PreloadedMasterDataDelegate()

            myGlobalDataTO = myPreloadedMasterDataDelegate.GetList(Nothing, GlobalEnumerates.PreloadedMasterDataEnum.AGE_UNITS)
            If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                Dim myPreloadedMasterDataDS As PreloadedMasterDataDS
                myPreloadedMasterDataDS = CType(myGlobalDataTO.SetDatos, PreloadedMasterDataDS)
                Return myPreloadedMasterDataDS
            Else
                ShowMessage(Me.Name & ".GetAgeUnits", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage)
                Return Nothing
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".GetAgeUnits", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".GetAgeUnits", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' Gets all defined SubTypes for Detailed Reference Ranges to pass to the Ref Ranges Control
    ''' </summary>
    ''' <remarks>
    ''' Created by: SA 14/12/2010 (as copy of GetGenders) - (copied from OFFSystem and adapted to ISE AG 10/01/2011)
    ''' </remarks>
    Private Function GetDetailedRangesSubTypes() As PreloadedMasterDataDS
        Try
            Dim myGlobalDataTO As New GlobalDataTO
            Dim myPreloadedMasterDataDelegate As New PreloadedMasterDataDelegate()

            myGlobalDataTO = myPreloadedMasterDataDelegate.GetList(Nothing, GlobalEnumerates.PreloadedMasterDataEnum.RANGE_SUBTYPES)
            If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                Dim myPreloadedMasterDataDS As PreloadedMasterDataDS

                myPreloadedMasterDataDS = DirectCast(myGlobalDataTO.SetDatos, PreloadedMasterDataDS)
                Return myPreloadedMasterDataDS
            Else
                'Error getting the list of available Detailed Reference Ranges Subtypes; shown it
                ShowMessage(Me.Name & ".GetDetailedRangesSubTypes", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage, Me)
                Return Nothing
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".GetDetailedRangesSubTypes ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".GetDetailedRangesSubTypes", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' Returns the lists of Genders
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>
    ''' Created by:  SG 20/10/2010
    ''' </remarks>
    Private Function GetGenders() As PreloadedMasterDataDS
        Try
            Dim myGlobalDataTO As New GlobalDataTO
            Dim myPreloadedMasterDataDelegate As New PreloadedMasterDataDelegate()
            myGlobalDataTO = myPreloadedMasterDataDelegate.GetList(Nothing, GlobalEnumerates.PreloadedMasterDataEnum.SEX_LIST)

            If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                Dim myPreloadedMasterDataDS As PreloadedMasterDataDS
                myPreloadedMasterDataDS = CType(myGlobalDataTO.SetDatos, PreloadedMasterDataDS)
                Return myPreloadedMasterDataDS
            Else
                ShowMessage(Me.Name & ".GetGenders", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage)
                Return Nothing
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".GetGenders", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".GetGenders", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' Gets all the field limits to pass to the Ref Ranges Control
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>
    ''' Created by SG 20/10/2010
    ''' </remarks>
    Private Function GetLimits() As FieldLimitsDS
        Dim myFieldLimitsDS As New FieldLimitsDS
        Try
            Dim myGlobalDataTO As New GlobalDataTO
            Dim myFieldLimitsDelegate As New FieldLimitsDelegate()
            myGlobalDataTO = myFieldLimitsDelegate.GetAllList(Nothing)

            If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                myFieldLimitsDS = CType(myGlobalDataTO.SetDatos, FieldLimitsDS)
            Else
                ShowMessage(Me.Name & ".GetLimits", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage)
                Return Nothing
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".GetLimits ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".GetLimits", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
        Return myFieldLimitsDS
    End Function

    ''' <summary>
    ''' Get all messages in the current application Language
    ''' </summary>
    ''' <returns>A String value containing the Message Text</returns>
    ''' <remarks>
    ''' Created by:  SG 20/10/2010
    ''' </remarks>
    Public Function GetMessages() As MessagesDS
        'Dim textMessage As String = String.Empty
        Try
            Dim myMessageDelegate As New MessageDelegate()
            Dim myGlobalDataTO As New GlobalDataTO()

            myGlobalDataTO = myMessageDelegate.GetAllMessageDescription(Nothing)
            If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                Dim myMessagesDS As New MessagesDS
                myMessagesDS = DirectCast(myGlobalDataTO.SetDatos, MessagesDS)
                Return myMessagesDS
            Else
                ShowMessage(Me.Name & ".GetMessages", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage)
                Return Nothing
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "GetMessages ", EventLogEntryType.Error, False)
            ShowMessage(Me.Name & ".GetMessages", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' Obtains value of field ActiveRangeType for the selected ISE TestID/Sample Type
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 12/01/2011
    ''' Modified by: XB 04/02/2013 - Upper conversions redundants because the value is already in UpperCase must delete to avoid Regional Settings problems (Bugs tracking #1112)
    ''' </remarks>
    Private Function GetRangeTypeForSampleType(ByVal pSampleType As String) As String
        Dim actRangeType As String = ""

        Try
            Dim qTestSamples As List(Of String)
            qTestSamples = (From a In Me.SelectedISETestSamplesDS.tparISETestSamples _
                            Where String.Equals(a.SampleType.Trim, pSampleType.Trim) _
                            And Not a.IsActiveRangeTypeNull _
                            Select a.ActiveRangeType).ToList()
            'Where String.Equals(a.SampleType.ToUpper.Trim, pSampleType.ToUpper.Trim) _

            If (qTestSamples.Count = 1) Then actRangeType = qTestSamples(0)

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".GetRangeTypeForSampleType", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".GetRangeTypeForSampleType", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
        Return actRangeType
    End Function

    ''' <summary>
    ''' Gets the Reference Ranges defined for the specified ISE test
    ''' </summary>
    ''' <param name="pISETestID">ISE Test Identifier</param>
    ''' <remarks>
    ''' Created by: SG 20/10/2010
    ''' </remarks>
    Private Sub GetRefRanges(ByVal pISETestID As Integer)
        Try
            Dim myGlobalDataTO As New GlobalDataTO
            Dim myTestRefRangesDelegate As New TestRefRangesDelegate

            myGlobalDataTO = myTestRefRangesDelegate.ReadByTestID(Nothing, pISETestID, SelectedSampleType, , "ISE") 'AG 10/01/2011 - add SelectedSampleType
            If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                SelectedTestRefRangesDS = CType(myGlobalDataTO.SetDatos, TestRefRangesDS)
            Else
                SelectedTestRefRangesDS = New TestRefRangesDS
                ShowMessage(Me.Name & ".GetRefRanges", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage)
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".GetRefRanges", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".GetRefRanges", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Get texts in the current application language for all screen controls
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SG 20/10/2010
    ''' Modified by: RH 05/06/2012 - Get labels for Quality Control Tab
    '''              WE 30/07/2014 - BA-1865 ==> Get labels for new controls ReportName and Decimals 
    '''              WE 01/08/2014 - BA-1865 ==> Get labels for Slope Function control in Options Tab
    '''              AG 05/09/2014 - BA-1869 ==> Added ToolTip for new button used to open the auxiliary screen that allow sort and set the  
    '''                                          availability of ISE Tests (Custom Order Button)
    '''              SA 17/11/2014 - BA-2125 ==> Added ToolTip for new button used to open the auxiliary screen that allow sort and set the  
    '''                                          availability of ISE Tests (Custom Order Button) - previous change was not really done; code 
    '''                                          was commented and the label was not the correct one. Commented code to get ToolTip for Print 
    '''                                          Button due to it is not visible.
    ''' </remarks>
    Private Sub GetScreenLabels()
        Try
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            'For Labels, CheckBox, RadioButtons.....
            bsISETestLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "TITLE_ISETests_Definition", currentLanguage)
            bsISETestListLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "TITLE_ISETests_List", currentLanguage)
            bsFullNameLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Name", currentLanguage) + ":"
            bsNameLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_ShortName", currentLanguage) + ":"
            bsUnitLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Unit", currentLanguage) + ":"
            bsSampleLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SampleType", currentLanguage) + ":"
            bsAvailableISETestCheckBox.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_AvailableISETest", currentLanguage)
            bsReportNameLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Tests_ReportName", currentLanguage) + ":"
            bsDecimalsLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Decimals", currentLanguage) + ":"

            'Details tab
            DetailsTabPage.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Tests_Options", currentLanguage)
            bsSlopeFunctionLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Tests_SlopeFunction", currentLanguage) + ":"
            bsReferenceRangesLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_ReferenceRanges_Long", currentLanguage) + ":"
            bsTestRefRanges.TextForGenericRadioButton = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Generic", currentLanguage)
            bsTestRefRanges.TextForNormalityLabel = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Normality", currentLanguage) & ":"
            bsTestRefRanges.TextForMinValueLabel = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_MinValue", currentLanguage)
            bsTestRefRanges.TextForMaxValueLabel = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_MaxValue", currentLanguage)
            bsTestRefRanges.TextForDetailedRadioButton = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_DetailedReferenceRange", currentLanguage)
            bsTestRefRanges.TextForGenderColumn = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Gender", currentLanguage)
            bsTestRefRanges.TextForAgeColumn = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Age", currentLanguage)
            bsTestRefRanges.TextForFromColumn = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_From", currentLanguage)
            bsTestRefRanges.TextForToColumn = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_To", currentLanguage)
            bsTestRefRanges.ToolTipForDetailDeleteButton = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_DelReferenceRange", currentLanguage)

            'Quality Control Tab
            QCTabPage.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_QualityControl", currentLanguage)
            QCValuesLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Quality_Control_Values", currentLanguage)
            QCActiveCheckBox.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_QCActive", currentLanguage)
            RulesToApplyGroupBox.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_RulesTo_Apply", currentLanguage)
            ControlValuesGroupBox.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Control_Values", currentLanguage)
            CalculationModeGroupBox.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_CALCULATION_Mode", currentLanguage)
            SixSigmaValuesGroupBox.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SixSigma_Values", currentLanguage)
            ControlReplicatesNumberLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Control_Replicates", currentLanguage) + ":"
            RejectionCriteriaLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Rejection_CriteriKSD", currentLanguage) + ":"
            StaticRadioButton.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Static", currentLanguage)
            ManualRadioButton.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Manual", currentLanguage)
            MinimumNumSeries.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_MinNum_Series", currentLanguage) + ":"
            ErrorAllowableLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Error_Allowable", currentLanguage) + ":"
            ControlsSelectionLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "TITLE_ControlSelection", currentLanguage)
            AddControlLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_CREATE_NEW_CONTROLS", currentLanguage)
            CUMULATE_QCRESULTS_Label = GetMessageText(GlobalEnumerates.Messages.CUMULATE_QCRESULTS.ToString(), currentLanguage)

            'For Tooltips
            bsScreenToolTips.SetToolTip(bsEditButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Edit", currentLanguage))
            bsScreenToolTips.SetToolTip(bsCustomOrderButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_TEST_SORTING_SELECTION", currentLanguage))
            'bsScreenToolTips.SetToolTip(bsPrintButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Print", currentLanguage))
            bsScreenToolTips.SetToolTip(bsSaveButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Save", currentLanguage))
            bsScreenToolTips.SetToolTip(bsCancelButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Cancel", currentLanguage))
            bsScreenToolTips.SetToolTip(bsExitButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_CloseScreen", currentLanguage))
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".GetScreenLabels", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".GetScreenLabels", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Configure and initialize the ListView of ISE Tests
    ''' </summary>
    ''' <remarks>
    ''' Modified by:  PG 08/10/2010 - Added the languageID parameter 
    '''               AG 22/10/2010 - Added column for Enabled field
    '''               RH 08/06/2012 - Remove the languageID parameter
    ''' </remarks>
    Private Sub InitializeISETestList()
        Try
            'Initialization of ISE Tests LListView
            bsISETestListView.Items.Clear()

            bsISETestListView.Alignment = ListViewAlignment.Left
            bsISETestListView.FullRowSelect = True
            bsISETestListView.MultiSelect = True
            bsISETestListView.Scrollable = True
            bsISETestListView.View = View.Details
            bsISETestListView.HideSelection = False
            bsISETestListView.HeaderStyle = ColumnHeaderStyle.Clickable

            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate
            'TR 20/03/2012 -Use the LBL_TestNames insted LBL_Name
            bsISETestListView.Columns.Add(myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_TestNames", currentLanguage), -2, HorizontalAlignment.Left)

            bsISETestListView.Columns.Add("ISETestID", 0, HorizontalAlignment.Left)
            bsISETestListView.Columns.Add("Name", 0, HorizontalAlignment.Left)
            bsISETestListView.Columns.Add("ShortName", 0, HorizontalAlignment.Left)
            bsISETestListView.Columns.Add("Units", 0, HorizontalAlignment.Left)
            bsISETestListView.Columns.Add("InUse", 0, HorizontalAlignment.Left)
            bsISETestListView.Columns.Add("Enabled", 0, HorizontalAlignment.Left)

            'Fill ListView with the list of existing ISE Tests
            LoadISETestList()

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".InitializeISETestList", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".InitializeISETestList", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Initializes the Reference Ranges User control, passing to it all DB data it neededs to work
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 16/12/2010 (copied from OFFSystem and adapted to ISE AG 10/01/2011)
    ''' </remarks>
    Private Sub InitializeReferenceRangesControl()
        Try
            'Control will be shown using the "big" layout and used for ISE Tests
            bsTestRefRanges.SmallLayout = False
            bsTestRefRanges.TestType = "ISE"

            'Load the necessary data to the Reference Ranges User Control
            Dim myAllFieldLimitsDS As FieldLimitsDS = GetLimits()
            Dim myGendersMasterDataDS As PreloadedMasterDataDS = GetGenders()
            Dim myAgeUnitsMasterDataDS As PreloadedMasterDataDS = GetAgeUnits()
            Dim myDetailedRangeSubTypesDS As PreloadedMasterDataDS = GetDetailedRangesSubTypes()
            Dim myAllMessagesDS As MessagesDS = GetMessages()

            bsTestRefRanges.LoadFrameworkData(myAllFieldLimitsDS, myGendersMasterDataDS, myAgeUnitsMasterDataDS, _
                                              myDetailedRangeSubTypesDS, myAllMessagesDS, SystemInfoManager.OSDecimalSeparator)
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".InitializeReferenceRangesControl", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".InitializeReferenceRangesControl", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Set the screen controls to INITIAL MODE
    ''' </summary>
    ''' <remarks>
    ''' Modified by: SG 20/10/2010 
    '''              SA 12/01/2010 - Changes due to new implementation of Reference Ranges Control 
    ''' </remarks>
    Private Sub InitialModeScreenStatus(Optional ByVal pInitializeListView As Boolean = True)
        Try
            If (pInitializeListView) Then
                'Area of ISE Test List
                If (bsISETestListView.Items.Count > 0) Then
                    bsISETestListView.Enabled = True
                    bsISETestListView.SelectedItems.Clear()
                End If

                bsEditButton.Enabled = False
                bsCustomOrderButton.Enabled = False 'AG 05/09/2014 - BA-1869
                'bsPrintButton.Enabled = False dl 11/05/2012
            End If

            'Area of ISE Test Definition
            bsFullNameTextbox.Text = ""
            bsFullNameTextbox.Enabled = False
            bsFullNameTextbox.BackColor = SystemColors.MenuBar

            bsShortNameTextbox.Text = ""
            bsShortNameTextbox.Enabled = False
            bsShortNameTextbox.BackColor = SystemColors.MenuBar

            bsSampleTypeComboBox.SelectedIndex = -1
            'bsSampleTypeComboBox.Enabled = False
            'bsSampleTypeComboBox.BackColor = SystemColors.MenuBar

            bsUnitComboBox.SelectedIndex = -1
            bsUnitComboBox.Enabled = False
            bsUnitComboBox.BackColor = SystemColors.MenuBar

            bsAvailableISETestCheckBox.CheckState = CheckState.Unchecked
            bsAvailableISETestCheckBox.Enabled = False

            ' WE 31/07/2014 - #1865
            bsReportNameTextBox.Text = ""
            bsReportNameTextBox.Enabled = False
            bsReportNameTextBox.BackColor = SystemColors.MenuBar

            bsDecimalsUpDown.Enabled = False
            bsDecimalsUpDown.BackColor = SystemColors.MenuBar

            SetISETestControlsLimits()
            ' WE 31/07/2014 - #1865 - End

            'Area of Reference Ranges
            InitializeReferenceRangesControl()
            bsTestRefRanges.isEditing = False

            'Buttons in details area
            bsSaveButton.Enabled = False
            bsCancelButton.Enabled = False

            'Initialize global variables and controls
            BsErrorProvider1.Clear()
            CleanGlobalValues()

            'RH 05/06/2012
            InitializeQCTab()

            ' WE 01/08/2014 - #1865
            InitializeOptionsTab()

            InitializeReferenceRangesControl()
            bsTestRefRanges.isEditing = False
            'RH 05/06/2012 END

            'Focus to button Edit
            If (pInitializeListView) Then bsEditButton.Focus()

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".InitialModeScreenStatus ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".InitialModeScreenStatus", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Load the ListView of ISE Tests with the list of existing ones
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SG 20/10/2010
    ''' Modified by: SA 17/05/2012 - An ISE Test will be marked as in use when InUse flag is TRUE OR Enabled flag is FALSE
    ''' </remarks>
    Private Sub LoadISETestList()
        Try
            Dim iconNameVar As String = ""
            Dim myIcons As New ImageList

            'Get the Icon defined for ISE Tests that are not in use in the current Work Session
            Dim notInUseIcon As String = GetIconName("TISE_SYS")
            If Not String.Equals(notInUseIcon, String.Empty) Then myIcons.Images.Add("TISE_SYS", Image.FromFile(MyBase.IconsPath & notInUseIcon, True))

            'Get the Icon defined for ISE Tests that are not in use in the current Work Session
            Dim inUseIcon As String = GetIconName("INUSETISE")
            If Not String.Equals(inUseIcon, String.Empty) Then myIcons.Images.Add("INUSETISE", Image.FromFile(MyBase.IconsPath & inUseIcon, True))

            'Assign the Icons to the ISE Tests List View
            bsISETestListView.Items.Clear()
            bsISETestListView.SmallImageList = myIcons

            'Get the list of existing ISE Tests
            Dim resultData As GlobalDataTO
            Dim ISETestList As New ISETestsDelegate

            resultData = ISETestList.GetList(Nothing)

            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                Dim myISETestDS As ISETestsDS

                myISETestDS = DirectCast(resultData.SetDatos, ISETestsDS)

                'Sort the returned ISE Tests
                Dim qISETests As List(Of ISETestsDS.tparISETestsRow)

                Select Case bsISETestListView.Sorting
                    Case SortOrder.Ascending
                        qISETests = (From a In myISETestDS.tparISETests _
                                     Select a Order By a.Name Ascending).ToList()

                    Case SortOrder.Descending
                        qISETests = (From a In myISETestDS.tparISETests _
                                     Select a Order By a.Name Descending).ToList()

                    Case SortOrder.None
                        qISETests = (From a In myISETestDS.tparISETests _
                                     Select a).ToList()

                    Case Else
                        qISETests = (From a In myISETestDS.tparISETests _
                                     Select a).ToList()

                End Select

                'Fill the List View with all existing ISE Tests
                Dim i As Integer = 0
                Dim inUseISETest As Boolean = False
                Dim rowToSelect As Integer = -1

                For Each iseTest As ISETestsDS.tparISETestsRow In qISETests
                    inUseISETest = (iseTest.InUse Or Not iseTest.Enabled)

                    iconNameVar = "TISE_SYS"
                    If (inUseISETest) Then iconNameVar = "INUSETISE"

                    bsISETestListView.Items.Add(iseTest.ISETestID.ToString, _
                                                iseTest.Name, _
                                                iconNameVar).Tag = inUseISETest

                    bsISETestListView.Items(i).SubItems.Add(iseTest.ISETestID.ToString)
                    bsISETestListView.Items(i).SubItems.Add(iseTest.Name)
                    bsISETestListView.Items(i).SubItems.Add(iseTest.ShortName)
                    bsISETestListView.Items(i).SubItems.Add(iseTest.Units)
                    bsISETestListView.Items(i).SubItems.Add(iseTest.InUse.ToString)
                    bsISETestListView.Items(i).SubItems.Add(iseTest.Enabled.ToString)

                    'If there is a selected ISE Test and it is still in the list, its position is stored to re-select 
                    'the same ISE Test once the list is loaded
                    If SelectedISETestID = iseTest.ISETestID Then rowToSelect = i
                    i += 1
                Next

                If (rowToSelect = -1) Then
                    'There was not a selected ISE Test or the selected one is not in the list; the global variables containing 
                    'information of the selected ISE Test is initializated
                    CleanGlobalValues()
                Else
                    'If there is a selected ISE Test, focus is put in the correspondent element in the Test ISE List
                    bsISETestListView.Items(rowToSelect).Selected = True
                    bsISETestListView.Select()

                    'The global variable containing the index of the selected ISE Test is updated
                    OriginalSelectedIndex = bsISETestListView.SelectedIndices(0)
                End If
            End If

            'An error has happened getting data from the Database
            If (resultData.HasError) Then
                ShowMessage(Me.Name & ".LoadISETestList", resultData.ErrorCode, resultData.ErrorMessage)

                CleanGlobalValues()
                bsISETestListView.Enabled = False
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".LoadISETestList ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".LoadISETestList", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Load the ComboBox of Measure Units with the list of existing ones
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub LoadMeasureUnits()
        Try
            'Get the list of existing Measure Units
            Dim myGlobalDataTo As GlobalDataTO
            Dim masterDataConfig As New MasterDataDelegate

            myGlobalDataTo = masterDataConfig.GetList(Nothing, MasterDataEnum.TEST_UNITS.ToString)

            If (Not myGlobalDataTo.HasError AndAlso Not myGlobalDataTo.SetDatos Is Nothing) Then
                Dim masterDataDS As MasterDataDS
                masterDataDS = DirectCast(myGlobalDataTo.SetDatos, MasterDataDS)

                If (masterDataDS.tcfgMasterData.Rows.Count > 0) Then
                    'Fill ComboBox of Measure Units in area of Calculated Test details
                    bsUnitComboBox.DataSource = masterDataDS.tcfgMasterData
                    bsUnitComboBox.DisplayMember = "FixedItemDesc"
                    bsUnitComboBox.ValueMember = "ItemID"
                End If
            Else
                'Error getting the list of defined Measure Units, show the error message
                ShowMessage(Me.Name & ".LoadMeasureUnits", myGlobalDataTo.ErrorCode, myGlobalDataTo.ErrorMessage)
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".LoadMeasureUnits", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".LoadMeasureUnits", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Shows the Reference Ranges defined for the selected ISE Test
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 24/12/2010 - (copied from OFFSystem and adapted to ISE AG 10/01/2011)
    ''' Modified by: XB 04/02/2013 - Upper conversions redundants because the value is already in UpperCase must delete to avoid Regional Settings problems (Bugs tracking #1112)
    ''' </remarks>
    Private Sub LoadRefRangesData()
        Try
            If (bsISETestListView.SelectedItems.Count = 1) Then
                Dim myTestID As Integer = CInt(bsISETestListView.SelectedItems(0).SubItems(1).Text)

                bsTestRefRanges.TestID = myTestID
                bsTestRefRanges.SampleType = SelectedSampleType

                'Search value of the ActiveRangeType for the selected ISE Test/SampleType
                bsTestRefRanges.ActiveRangeType = GetRangeTypeForSampleType(SelectedSampleType)

                bsTestRefRanges.MeasureUnit = bsUnitComboBox.Text.ToString
                bsTestRefRanges.DefinedTestRangesDS = SelectedTestRefRangesDS

                'DL 21/02/2012. Begin
                Dim decimalsNumber As Integer = 0

                Dim lstTestSamples As New List(Of ISETestSamplesDS.tparISETestSamplesRow)
                lstTestSamples = (From a In SelectedISETestSamplesDS.tparISETestSamples _
                                 Where a.ISETestID = SelectedISETestID _
                                   And String.Compare(a.SampleType.Trim, SelectedSampleType.Trim, False) = 0 _
                                Select a).ToList()
                'And String.Compare(a.SampleType.ToUpper.Trim, SelectedSampleType.ToUpper.Trim, False) = 0 _

                If lstTestSamples.Count > 0 Then decimalsNumber = lstTestSamples(0).Decimals

                'If (IsNumeric(bsDecimalsUpDown.Text)) Then decimalsNumber = Convert.ToInt32(bsDecimalsUpDown.Text)
                'DL 21/02/2012. End

                bsTestRefRanges.RefNumDecimals = decimalsNumber

                If (SelectedTestRefRangesDS IsNot Nothing) Then
                    If SelectedTestRefRangesDS.tparTestRefRanges.Rows.Count > 0 Then
                        bsTestRefRanges.LoadReferenceRanges()
                    Else
                        bsTestRefRanges.ClearReferenceRanges()
                    End If
                Else
                    bsTestRefRanges.ClearReferenceRanges()
                End If
                bsTestRefRanges.isEditing = bsShortNameTextbox.Enabled
            Else
                bsTestRefRanges.ClearData()
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".LoadRefRangesData", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".LoadRefRangesData", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Load the ComboBoxes of Sample Types with the list of the ones used for the selected ISE Test
    ''' </summary>
    ''' <remarks>
    ''' Modified by: SG 21/10/2010 
    '''              SA 19/11/2010 - When the selected ISE Tests have several SampleTypes, put visible the Plus Icon
    '''              SA 12/01/2011 - Changes due to new implementation of Reference Ranges Control
    ''' </remarks>
    Private Sub LoadSampleTypesList()
        Try
            'Dim myGlobalDataTo As New GlobalDataTO
            Dim myGlobalDataTo As GlobalDataTO
            Dim myISETestSampleDelegate As New ISETestSamplesDelegate

            myGlobalDataTo = myISETestSampleDelegate.GetListByISETestID(Nothing, SelectedISETestID)

            If (Not myGlobalDataTo.HasError And Not myGlobalDataTo.SetDatos Is Nothing) Then
                SelectedISETestSamplesDS = DirectCast(myGlobalDataTo.SetDatos, ISETestSamplesDS)

                If (SelectedISETestSamplesDS.tparISETestSamples.Rows.Count > 0) Then
                    'Load the SampleTypes in the ComboBox and select the first 
                    bsSampleTypeComboBox.DataSource = SelectedISETestSamplesDS.tparISETestSamples
                    bsSampleTypeComboBox.DisplayMember = "SampleTypeDesc"
                    bsSampleTypeComboBox.ValueMember = "SampleType"

                    bsSampleTypeComboBox.SelectedIndex = 0
                    SelectedSampleType = bsSampleTypeComboBox.SelectedValue.ToString
                End If

                If (bsSampleTypeComboBox.Items.Count > 1) Then
                    'Create the list with all Sample Types to shown them as ToolTip
                    Dim myISETestSampleList As String = ""
                    For Each myTestSampleRow As ISETestSamplesDS.tparISETestSamplesRow In SelectedISETestSamplesDS.tparISETestSamples.Rows
                        myISETestSampleList &= myTestSampleRow.SampleType & " " & vbCrLf
                    Next

                    bsSampleTypePlusPictureBox.Visible = True
                    bsScreenToolTips.SetToolTip(bsSampleTypePlusPictureBox, myISETestSampleList.TrimEnd())
                Else
                    bsSampleTypePlusPictureBox.Visible = False
                End If
            Else
                'Error getting the list of SampleTypes for the selected ISE Test; shown it
                ShowMessage(Me.Name & ".LoadSampleTypesList", myGlobalDataTo.ErrorCode, myGlobalDataTo.ErrorMessage)
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".LoadSampleTypesList", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".LoadSampleTypesList", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Verify if there is at least one User's change pending to save
    ''' </summary>
    ''' <returns>True if there are changes pending to save; otherwise, False</returns>
    ''' <remarks>
    ''' Modified by: SG 20/10/2010 
    '''              SA 12/01/2011 - Changes due to new implementation of Reference Ranges Control 
    ''' </remarks>
    Public Function PendingChangesVerification() As Boolean
        Dim pendingToSaveChanges As Boolean = False

        Try
            If (EditionMode) Then
                'When changes have been made in the Test Reference Ranges or there are errors still unsolved
                If (ChangesMade OrElse bsTestRefRanges.ChangesMade OrElse bsTestRefRanges.ValidationError) Then
                    pendingToSaveChanges = True
                    'SelectedTestRefRangesDS.Clear()
                    'SelectedTestRefRangesDS = bsTestRefRanges.DefinedTestRangesDS
                Else
                    If (bsISETestListView.SelectedItems.Count = 0) Then
                        'If there is an ISE Test in edition and a click is made out of the list of Tests,
                        'the Test in edition is selected to avoid errors if the screen is closed or the edition cancelled
                        If (OriginalSelectedIndex > 0) Then
                            'Return focus to the ISE Test that has been edited
                            bsISETestListView.Items(OriginalSelectedIndex).Selected = True
                            bsISETestListView.Select()
                        End If
                    End If

                    'In Edit Mode, loading values are compared against current values
                    ' WE 29/07/2014 - #1865 - Added <Name> field.
                    If (bsISETestListView.SelectedIndices(0) = OriginalSelectedIndex) Then
                        pendingToSaveChanges = (String.Compare(bsFullNameTextbox.Text.Trim, bsISETestListView.SelectedItems(0).SubItems(2).Text, False) <> 0) Or _
                                               (String.Compare(bsShortNameTextbox.Text.Trim, bsISETestListView.SelectedItems(0).SubItems(3).Text, False) <> 0) Or _
                                               (String.Compare(bsAvailableISETestCheckBox.Checked.ToString, bsISETestListView.SelectedItems(0).SubItems(6).Text, False) <> 0)
                    Else
                        pendingToSaveChanges = (String.Compare(bsFullNameTextbox.Text.Trim, bsISETestListView.Items(OriginalSelectedIndex).SubItems(2).Text, False) <> 0) Or _
                                               (String.Compare(bsShortNameTextbox.Text.Trim, bsISETestListView.Items(OriginalSelectedIndex).SubItems(3).Text, False) <> 0) Or _
                                               (bsAvailableISETestCheckBox.Checked.ToString <> bsISETestListView.Items(OriginalSelectedIndex).SubItems(6).Text)
                    End If
                End If

                If (Not pendingToSaveChanges) Then
                    pendingToSaveChanges = (Not ValidateSavingConditions())
                End If
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".PendingChangesVerification", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PendingChangesVerification", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try

        Return pendingToSaveChanges
    End Function

    ''' <summary>
    ''' Method incharge to load the image for each button
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 11/06/2010
    ''' Modified by: DL 03/11/2010 - Load the Icon in Image Property instead of in BackgroundImage Property
    '''              DL 12/11/2010 - Load Icons needed for graphical buttons in Reference Ranges User Control
    '''              AG 10/01/2011 - Changes due to new implementation of Reference Ranges Control 
    ''' </remarks>
    Private Sub PrepareButtons()
        Try
            Dim auxIconName As String = ""
            Dim iconPath As String = MyBase.IconsPath

            'Icon to indicate if the ISE Test uses several Sample Types
            auxIconName = GetIconName("PLUS")
            If Not String.Equals(auxIconName, String.Empty) Then bsSampleTypePlusPictureBox.ImageLocation = MyBase.IconsPath & GetIconName("PLUS")

            'EDIT Button
            auxIconName = GetIconName("EDIT")
            If Not String.Equals(auxIconName, String.Empty) Then bsEditButton.Image = Image.FromFile(iconPath & auxIconName, True)

            'PRINT Button
            auxIconName = GetIconName("PRINT")
            If Not String.Equals(auxIconName, String.Empty) Then bsPrintButton.Image = Image.FromFile(iconPath & auxIconName, True)
            'JB 30/08/2012 - Hide Print button
            bsPrintButton.Visible = False

            'CUSTOMSORT Button 'AG 05/09/2014 - BA-1869
            auxIconName = GetIconName("ORDER_TESTS")
            If Not String.Equals(auxIconName, String.Empty) Then bsCustomOrderButton.Image = Image.FromFile(iconPath & auxIconName, True)


            'SAVE Button
            auxIconName = GetIconName("SAVE")
            If Not String.Equals(auxIconName, String.Empty) Then bsSaveButton.Image = Image.FromFile(iconPath & auxIconName, True)

            'CANCEL Button
            auxIconName = GetIconName("UNDO")
            If Not String.Equals(auxIconName, String.Empty) Then bsCancelButton.Image = Image.FromFile(iconPath & auxIconName, True)

            'CLOSE Button
            auxIconName = GetIconName("CANCEL")
            If Not String.Equals(auxIconName, String.Empty) Then bsExitButton.Image = Image.FromFile(iconPath & auxIconName, True)

            'Reference Ranges Buttons
            'DELETE DETAILED RANGE Button
            auxIconName = GetIconName("REMOVE")
            If Not String.Equals(auxIconName, String.Empty) Then bsTestRefRanges.DeleteButtonImage = Image.FromFile(iconPath & auxIconName, True)

            'RH 05/06/2012
            auxIconName = GetIconName("ADD")
            If Not String.Equals(auxIconName, String.Empty) Then AddControls.Image = Image.FromFile(iconPath & auxIconName, True)

            auxIconName = GetIconName("REMOVE")
            If Not String.Equals(auxIconName, String.Empty) Then DeleteControlButton.Image = Image.FromFile(iconPath & auxIconName, True)

            'RH 05/06/2012 END

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".PrepareButtons", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PrepareButtons", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Get data of the selected ISE Test, fill the correspondent variables and controls and set the screen status 
    ''' to Read-only Mode
    ''' </summary>
    ''' <param name="pSampleTypeChanged" > TRUE when change sample type in edition mode, FALSE otherwise</param>
    ''' <remarks>
    ''' Modified by: RH 05/06/2012
    '''              WE 29/08/2014 - Bug fix that clears error indicator whenever user switches to other SampleType
    '''                              or ISE Test during edition of SlopeFactor fields.
    ''' </remarks>
    Private Sub QueryISETest(ByVal pSampleTypeChanged As Boolean)
        Dim setScreenToQuery As Boolean = False

        Try
            If (bsISETestListView.SelectedIndices(0) = OriginalSelectedIndex) AndAlso Not pSampleTypeChanged Then
                Return 'Nothing to do
            End If

            'If (bsISETestListView.SelectedIndices(0) <> OriginalSelectedIndex) Then
            If (PendingChangesVerification()) Then
                If (ShowMessage(Me.Name, GlobalEnumerates.Messages.DISCARD_PENDING_CHANGES.ToString) = DialogResult.Yes) Then
                    SelectedTestRefRangesDS.Clear()
                    setScreenToQuery = True
                    ' WE 29/08/2014 - Bug fix that clears error indicator whenever user switches to other SampleType
                    ' or ISE Test during edition of SlopeFactor fields.
                    BsErrorProvider1.Clear()
                Else
                    If (OriginalSelectedIndex <> -1) Then
                        bsISETestListView.SelectedItems.Clear() 'AG 26/10/2010 - Clear selection

                        'Return focus to the ISE Test that has been edited
                        bsISETestListView.Items(OriginalSelectedIndex).Selected = True
                        bsISETestListView.Select()
                    End If
                End If
            Else
                SelectedTestRefRangesDS.Clear()
                setScreenToQuery = True
            End If

            If (setScreenToQuery) Then
                EditionMode = False
                ChangesMade = False

                Dim SaveSelectedIndex As Integer = bsSampleTypeComboBox.SelectedIndex
                Dim SaveOriginalSelectedIndex As Integer = OriginalSelectedIndex

                'Load screen fields with all data of the selected ISE Test
                Dim inUseISETest As Boolean = BindISETestData()

                If (SaveOriginalSelectedIndex = OriginalSelectedIndex) AndAlso SaveSelectedIndex >= 0 Then
                    bsSampleTypeComboBox.SelectedIndex = SaveSelectedIndex
                    SelectedSampleType = bsSampleTypeComboBox.SelectedValue.ToString()
                End If

                BindISETestSamplesData(SelectedISETestID, SelectedSampleType)    ' WE 30/07/2014 - #1865

                BindISETestQCData(SelectedISETestID, SelectedSampleType)

                'Get the Reference Ranges defined for the ISE Test and the selected SampleType and shown them
                GetRefRanges(SelectedISETestID)
                LoadRefRangesData()

                If (Not inUseISETest) Then
                    'Set screen status to Query Mode
                    QueryModeScreenStatus()
                Else
                    'Set screen status to Read Only Mode 
                    ReadOnlyModeScreenStatus()
                End If
            End If

            'ElseIf (pSampleTypeChanged) Then
            '    SelectedSampleType = bsSampleTypeComboBox.SelectedValue.ToString()

            '    'Get the Reference Ranges defined for the ISE Test and the selected SampleType and shown them
            '    GetRefRanges(OriginalSelectedIndex)
            '    LoadRefRangesData()
            'End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".QueryISETest", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".QueryISETest", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Screen display in query mode when the KeyUp, KeyDown, PageDown or PageUp key is pressed
    ''' </summary>
    ''' <remarks>
    ''' Modified by: SG 20/10/2010 
    '''              SA 19/11/2010 - Include also PageDown and PageUp keys
    ''' </remarks>
    Private Sub QueryISETestByMoveUpDown(ByVal e As System.Windows.Forms.KeyEventArgs)
        Try
            'TR 13/10/2011 -Do not validate the press key and send the query
            QueryISETest(False)

            'Select Case e.KeyCode
            '    Case Keys.Up, Keys.Down, Keys.PageDown, Keys.PageUp
            '        QueryISETest(False)
            'End Select
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".QueryISETestByMoveUpDown", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".QueryISETestByMoveUpDown", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Set the screen controls to QUERY MODE
    ''' </summary>
    ''' <remarks>
    ''' Modified by: RH 05/06/2012
    ''' </remarks>
    Private Sub QueryModeScreenStatus()
        Try
            bsEditButton.Enabled = True
            bsCustomOrderButton.Enabled = True 'AG 05/09/2014 - BA-1869
            'bsPrintButton.Enabled = True dl 11/05/2012

            bsFullNameTextbox.Enabled = False
            bsFullNameTextbox.BackColor = SystemColors.MenuBar

            bsShortNameTextbox.Enabled = False
            bsShortNameTextbox.BackColor = SystemColors.MenuBar

            bsSampleTypeComboBox.Enabled = True
            bsSampleTypeComboBox.BackColor = Color.White

            bsUnitComboBox.Enabled = False
            bsUnitComboBox.BackColor = SystemColors.MenuBar

            bsAvailableISETestCheckBox.Enabled = False

            ' WE 30/07/2014 - #1865
            bsReportNameTextBox.Enabled = False
            bsReportNameTextBox.BackColor = SystemColors.MenuBar

            bsDecimalsUpDown.Enabled = False
            bsDecimalsUpDown.BackColor = SystemColors.MenuBar
            ' WE 30/07/2014 - #1865 - End

            bsVolumeUpDown.Enabled = False
            bsVolumeUpDown.BackColor = SystemColors.MenuBar

            bsDilutionUpDown.Enabled = False
            bsDilutionUpDown.BackColor = SystemColors.MenuBar

            bsSaveButton.Enabled = False
            bsCancelButton.Enabled = False

            bsTestRefRanges.isEditing = False

            bsQCPanel.Enabled = False

            ' WE 01/08/2014 - #1865
            bsSlopeA2UpDown.Enabled = False
            bsSlopeA2UpDown.BackColor = SystemColors.MenuBar
            bsSlopeB2UpDown.Enabled = False
            bsSlopeB2UpDown.BackColor = SystemColors.MenuBar
            ' WE 01/08/2014 - #1865 - End

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".QueryModeScreenStatus", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".QueryModeScreenStatus", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Set the screen controls to READ ONLY MODE (for InUse ISE Tests)
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SG 20/10/2010
    ''' </remarks>
    Private Sub ReadOnlyModeScreenStatus()
        Try
            'Set all controls to QUERY MODE
            QueryModeScreenStatus()

            'Disable all buttons that cannot be used in Read Only Mode
            bsEditButton.Enabled = False
            bsCustomOrderButton.Enabled = True 'AG 05/09/2014 - BA-1869

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ReadOnlyModeScreenStatus " & Me.Name, EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ReadOnlyModeScreenStatus", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Save data of the Updated ISE Test
    ''' </summary>
    ''' <param name="pSampleTypeChanged" >TRUE when user changes the sample type in edition mode.
    '''                                   FALSE when user press SAVE button</param>
    ''' <remarks>
    ''' Modified by: RH 13/06/2012
    '''              SA 21/06/2012 - When function ValidateSavingConditions returns FALSE, then this function returns
    '''                              value of global variable ValidationError
    ''' </remarks>
    Private Function SaveChanges(ByVal pSampleTypeChanged As Boolean) As Boolean
        Try
            'Verify if the current ISE Test/Sample Type can be saved
            If (ValidateSavingConditions()) Then
                BsErrorProvider1.Clear() 'RH 22/06/2012

                If SavePendingWarningMessage() = DialogResult.Cancel Then
                    'If current edition validation fails stop saving process
                    Return False
                End If

                If (SaveISETestChanges()) Then
                    'Refresh the ISE Tests List 
                    LoadISETestList()

                    'Show the Test in Query Mode only if the saving was not after a SampleType change
                    If (Not pSampleTypeChanged) Then
                        EditionMode = False
                        QueryModeScreenStatus()
                    End If

                    Return True
                End If
            Else
                Return ValidationError
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".SaveChanges", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".SaveChanges", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
            Return False
        End Try

        Return False
    End Function

    ''' <summary>
    ''' Save ISE Test changes (ISE, ISE SampleType and Reference Ranges)
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SG 20/10/2010
    ''' Modified by: SA 10/11/2010 - When SAVE function return an error, shown it. Change to function that
    '''                              return a Boolean value indicating if the Save was executed without errors 
    '''              SA 12/01/2011 - Changes due to new implementation of Reference Ranges Control 
    '''              SA 17/09/2014 - BA-1926 ==> When field ReportName is empty, set NULL value for it in the DataSet
    ''' </remarks>
    Private Function SaveISETestChanges() As Boolean
        Dim changesSaved As Boolean = False

        Try
            Dim continueSaving As Boolean = False

            'Create ISETests DataSets needed for the Save method
            'Dim newISETestDS As New ISETestsDS          'Add is not available in phase 1
            Dim updatedISETestsDS As New ISETestsDS

            'Process changes in the ISE Test definition
            'Dim myResult As New GlobalDataTO
            Dim myResult As GlobalDataTO
            Dim myISETestDelegate As New ISETestsDelegate

            myResult = myISETestDelegate.Read(Nothing, SelectedISETestID)

            If (Not myResult.HasError AndAlso Not myResult.SetDatos Is Nothing) Then
                updatedISETestsDS = CType(myResult.SetDatos, ISETestsDS)

                'Update DataSet fields with the screen control value when it has changed
                Dim testChanged As Boolean = False

                If (updatedISETestsDS.tparISETests.Rows.Count = 1) Then
                    continueSaving = True

                    With updatedISETestsDS.tparISETests(0)
                        .BeginEdit()
                        If Not String.Equals(.Name, bsFullNameTextbox.Text.Trim) Then
                            .Name = bsFullNameTextbox.Text.Trim
                            testChanged = True
                        End If

                        If Not String.Equals(.ShortName, bsShortNameTextbox.Text.Trim) Then
                            .ShortName = bsShortNameTextbox.Text.Trim
                            testChanged = True
                        End If

                        If Not String.Equals(.Units, bsUnitComboBox.SelectedValue.ToString) Then
                            .Units = bsUnitComboBox.SelectedValue.ToString
                            testChanged = True
                        End If

                        If (bsAvailableISETestCheckBox.CheckState = CheckState.Checked And Not .Enabled) Then
                            .Enabled = True
                            testChanged = True
                        ElseIf (bsAvailableISETestCheckBox.CheckState = CheckState.Unchecked And .Enabled) Then
                            .Enabled = False
                            testChanged = True
                        End If

                        .AcceptChanges()
                    End With

                    If (Not testChanged) Then updatedISETestsDS.Clear()
                End If
            Else
                'Error reading data of the selected ISE Test ID; shown it
                ShowMessage("", myResult.ErrorCode, myResult.ErrorMessage, Me)
            End If

            'Remove control row without ControlID 
            'Dim NumComtrol As Integer = SelectedTestControlDS.tparTestControls.Count() - 1
            'For i As Integer = NumComtrol To 0 Step -1
            '    If SelectedTestControlDS.tparTestControls(i).IsControlIDNull Then
            '        SelectedTestControlDS.tparTestControls(i).Delete()
            '    End If
            'Next

            'Create ISETestSamples DataSets needed for the Save method
            Dim newISETestsSamplesDS As New ISETestSamplesDS       'Add is not available in phase 1
            Dim updatedISETestsSamplesDS As New ISETestSamplesDS

            If (continueSaving) Then
                'Verify if value of ActiveRangeType has been changed for the Test/SampleType
                continueSaving = False
                Dim myISETestSampleDelegate As New ISETestSamplesDelegate

                myResult = myISETestSampleDelegate.GetListByISETestID(Nothing, SelectedISETestID, SelectedSampleType)

                If (Not myResult.HasError AndAlso Not myResult.SetDatos Is Nothing) Then
                    continueSaving = True
                    updatedISETestsSamplesDS = CType(myResult.SetDatos, ISETestSamplesDS)

                    'Update DataSet fields with the screen control value when it has changed
                    Dim testChanged As Boolean = False
                    Dim selectedRangeType As String = bsTestRefRanges.ActiveRangeType

                    If (updatedISETestsSamplesDS.tparISETestSamples.Rows.Count = 1) Then
                        With updatedISETestsSamplesDS.tparISETestSamples(0)
                            .BeginEdit()
                            .ISETestName = bsFullNameTextbox.Text.Trim
                            .ISETestShortName = bsShortNameTextbox.Text.Trim
                            .MeasureUnit = bsUnitComboBox.SelectedValue.ToString

                            If (Not .IsActiveRangeTypeNull) Then
                                If (selectedRangeType = String.Empty) Then
                                    testChanged = True
                                    .SetActiveRangeTypeNull()
                                ElseIf (String.Compare(selectedRangeType, .ActiveRangeType, False) <> 0) Then
                                    testChanged = True
                                    .ActiveRangeType = selectedRangeType
                                End If
                            Else
                                If (String.Compare(selectedRangeType, String.Empty, False) <> 0) Then
                                    testChanged = True
                                    .ActiveRangeType = selectedRangeType
                                End If
                            End If

                            'RH 11/06/2012
                            .QCActive = QCActiveCheckBox.Checked

                            ' WE 31/07/2014 - #1865
                            If (Not String.IsNullOrEmpty(bsReportNameTextBox.Text)) Then
                                .TestLongName = bsReportNameTextBox.Text.Trim
                            Else
                                .SetTestLongNameNull()
                            End If

                            .Decimals = CByte(bsDecimalsUpDown.Value)

                            'TR 29/03/2010 Add the slope factor to save.
                            If Not String.IsNullOrEmpty(bsSlopeA2UpDown.Text) Then
                                .SlopeFactorA2 = CType(bsSlopeA2UpDown.Value, Single)
                            Else
                                'TR 21/06/2010
                                .SetSlopeFactorA2Null()
                            End If

                            If Not String.IsNullOrEmpty(bsSlopeB2UpDown.Text) Then
                                .SlopeFactorB2 = CType(bsSlopeB2UpDown.Value, Single)
                            Else
                                'TR 21/06/2010
                                .SetSlopeFactorB2Null()
                            End If
                            ' WE 31/07/2014 - #1865 - End

                            If Not String.IsNullOrEmpty(QCReplicNumberNumeric.Text) Then
                                .ControlReplicates = CInt(QCReplicNumberNumeric.Value)
                            Else
                                .SetControlReplicatesNull()
                            End If

                            .NumberOfControls = CountActiveControl()

                            If Not String.IsNullOrEmpty(QCRejectionCriteria.Text) Then
                                .RejectionCriteria = QCRejectionCriteria.Value
                            Else
                                .SetRejectionCriteriaNull()
                            End If

                            If ManualRadioButton.Checked Then
                                .CalculationMode = "MANUAL"
                                .NumberOfSeries = 0
                            ElseIf StaticRadioButton.Checked Then
                                .CalculationMode = "STATISTIC"
                                'Set the minimun num of series if checked the static.
                                If Not String.IsNullOrEmpty(QCMinNumSeries.Text) Then
                                    .NumberOfSeries = CInt(QCMinNumSeries.Value)
                                Else
                                    .SetNumberOfSeriesNull()
                                End If
                            Else
                                .CalculationMode = String.Empty
                                .NumberOfSeries = 0
                            End If

                            If Not String.IsNullOrEmpty(QCErrorAllowable.Text) Then
                                .TotalAllowedError = QCErrorAllowable.Value
                            Else
                                .SetTotalAllowedErrorNull()
                            End If
                            'RH 11/06/2012 END

                            .AcceptChanges()
                        End With

                        'If (Not testChanged) Then updatedISETestsSamplesDS.Clear()
                    End If
                Else
                    'Error reading data of the selected ISE TestID and SampleType; shown it
                    ShowMessage("", myResult.ErrorCode, myResult.ErrorMessage, Me)
                End If
            End If

            'Create ReferenceRanges DataSets needed for the Save method
            Dim newRefRangesDS As New TestRefRangesDS
            Dim updatedRefRangesDS As New TestRefRangesDS
            Dim deletedRefRangesDS As New TestRefRangesDS

            If (continueSaving) Then
                'Get from the User Control of Reference Ranges the defined ones
                'SelectedTestRefRangesDS.Clear()
                SelectedTestRefRangesDS = DirectCast(bsTestRefRanges.DefinedTestRangesDS, TestRefRangesDS) 'bsTestRefRanges.DefinedTestRangesDS

                Dim myGlobalBase As New GlobalBase
                'Dim myTestRefRanges As New List(Of TestRefRangesDS.tparTestRefRangesRow)
                Dim myTestRefRanges As List(Of TestRefRangesDS.tparTestRefRangesRow)

                'CREATE: Get all added Reference Ranges
                myTestRefRanges = (From a In SelectedTestRefRangesDS.tparTestRefRanges _
                                   Where a.IsNew = True _
                                   Select a).ToList()

                For i As Integer = 0 To myTestRefRanges.Count - 1
                    myTestRefRanges(0).BeginEdit()
                    myTestRefRanges(0).TS_User = GlobalBase.GetSessionInfo.UserName
                    myTestRefRanges(0).TS_DateTime = Now
                    myTestRefRanges(0).EndEdit()

                    newRefRangesDS.tparTestRefRanges.ImportRow(myTestRefRanges(i))
                Next

                'UPDATE: Get all updated Reference Ranges
                myTestRefRanges = (From a In SelectedTestRefRangesDS.tparTestRefRanges _
                                   Where a.IsNew = False _
                                   And a.IsDeleted = False _
                                   Select a).ToList()

                For i As Integer = 0 To myTestRefRanges.Count - 1
                    myTestRefRanges(0).BeginEdit()
                    myTestRefRanges(0).TS_User = GlobalBase.GetSessionInfo.UserName
                    myTestRefRanges(0).TS_DateTime = Now
                    myTestRefRanges(0).EndEdit()

                    updatedRefRangesDS.tparTestRefRanges.ImportRow(myTestRefRanges(i))
                Next

                'DELETE: Get all Reference Ranges marked to delete
                myTestRefRanges = (From a In SelectedTestRefRangesDS.tparTestRefRanges _
                                   Where a.IsDeleted _
                                   Select a).ToList()

                For i As Integer = 0 To myTestRefRanges.Count - 1
                    deletedRefRangesDS.tparTestRefRanges.ImportRow(myTestRefRanges(i))
                Next

                UpdateTestSampleMultiRules(SelectedISETestID, SelectedSampleType)

                'Now save the ISE Test...
                myResult = myISETestDelegate.SaveISETestNEW(Nothing, updatedISETestsDS, updatedISETestsSamplesDS, _
                                                         newRefRangesDS, updatedRefRangesDS, deletedRefRangesDS, _
                                                         SelectedTestSampleMultirulesDS, SelectedTestControlDS, _
                                                         LocalDeleteControlTOList)

                If (Not myResult.HasError) Then
                    changesSaved = True
                    BindISETestQCData(SelectedISETestID, SelectedSampleType)
                Else
                    'Error saving the ISE Test data; shown it
                    ShowMessage(Me.Name & ".SaveISETestChanges", myResult.ErrorCode, myResult.ErrorMessage, Me)
                End If
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".SaveISETestChanges", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".SaveISETestChanges", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try

        Return changesSaved
    End Function

    ''' <summary>
    ''' Load all data needed for the screen
    ''' </summary>
    ''' <remarks>
    ''' Modified by: SG 20/10/2010 
    '''              PG 08/10/2010 - Multilanguage implementation
    '''              SA 19/11/2010 - Load the Icon indicating if the ISE Test uses several Sample Types
    '''              DL 09/12/2010 - Validate if the selected ISE Test uses several Sample Types to shown the Icon 
    '''              RH 15/12/2010 - Added call to new ResetBorder to avoid flickering when opening
    '''              AG 10/01/2011 - Changes due to new implementation of Reference Ranges Control 
    '''              SA 12/01/2011 - Changes due to new implementation of Reference Ranges Control 
    ''' </remarks>
    Private Sub ScreenLoad()
        Try
            'Get the current Language from the current Application Session
            Dim currentLanguageGlobal As New GlobalBase
            currentLanguage = GlobalBase.GetSessionInfo().ApplicationLanguage

            'Get Icons for graphical buttons
            PrepareButtons()

            'Load the list of available Measure Units 
            LoadMeasureUnits()
            'LoadDecimalsLimit()

            'Load the list of existing ISE Tests
            InitializeISETestList()

            'Load the multilanguage texts for all Screen Labels
            GetScreenLabels()

            'Set Screen Status to INITIAL MODE
            InitialModeScreenStatus()

            'changesMade = False 'AG 22/10/2010 - Initialize changes variables on load screen (No changes)
            If (bsISETestListView.Items.Count > 0) Then   'AG 21/06/2010
                'Select the first ISE Test in the list
                bsISETestListView.Items(0).Selected = True
                QueryISETest(False)
            End If

            'To avoid flickering when opening
            ResetBorder()

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ScreenLoad", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ScreenLoad", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Validates ref ranges
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>
    ''' Created by:  SG 20/10/2010
    ''' Modified by: AG 10/01/2011 - Following the scheme for new ref ranges controls (January 2011) - copied from OffSystem
    ''' </remarks>
    Private Function ValidateRefRanges() As Boolean
        Try
            If (String.Compare(bsTestRefRanges.ActiveRangeType, String.Empty, False) <> 0) Then
                bsTestRefRanges.ValidateRefRangesLimits(False, True)
                If (bsTestRefRanges.ValidationError) Then Return False
            End If
            Return True
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ValidateRefRanges", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ValidateRefRanges", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Validate that all mandatory fields have been informed with a correct value
    ''' </summary>
    ''' <returns>True if all fields have a correct value; otherwise, False</returns>
    ''' <remarks>
    ''' Modified by: SG 20/10/2010
    '''              SA 13/01/2010 - Verify also if the informed ShortName is unique
    '''              RH 08/06/2012 - Added validation of data in QCTab by calling function ValidateErrorOnQCTab
    '''              SA 21/06/2012 - Set global variable ValidationError = True when there is at least a wrong value in the screen fields
    '''              WE 29/07/2014 - Added validation for Name field.
    '''              WE 26/08/2014 - Introduction of Form-level validation for SlopeFactor A2/B2 likewise in IProgTest.
    '''             
    ''' </remarks>
    Private Function ValidateSavingConditions() As Boolean
        Dim fieldsOK As Boolean = True
        Try
            Dim setFocusTo As Integer = -1

            BsErrorProvider1.Clear()
            ' WE 29/07/2014 - #1865
            If (bsFullNameTextbox.TextLength = 0) Then
                'Validate the Long Name is not empty otherwise inform the missing data
                BsErrorProvider1.SetError(bsFullNameTextbox, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString))
                setFocusTo = 0
            End If
            ' WE 29/07/2014 - #1865 - End.

            If (bsShortNameTextbox.TextLength = 0) Then
                'Validate the Short Name is not empty otherwise inform the missing data
                BsErrorProvider1.SetError(bsShortNameTextbox, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString))
                If (setFocusTo = -1) Then setFocusTo = 1
            End If

            'If (bsUnitComboBox.Text.Trim.Length = 0) Then
            '    'Validate the Unit Combobox is not empty otherwise inform the missing data
            '    bsScreenErrorProvider.SetError(bsUnitComboBox, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString))
            '    If (setFocusTo = -1) Then setFocusTo = 2
            'End If

            If (Not ValidateRefRanges()) Then
                If (setFocusTo = -1) Then setFocusTo = 3
            End If

            'RH 08/06/2012
            If (ValidateErrorOnQCTab(True)) Then
                If (setFocusTo = -1) Then setFocusTo = 4
            End If

            ' WE 26/08/2014 - #1865 - Validation of SlopeFactors A2/B2.
            If Not bsSlopeA2UpDown.Text = "" AndAlso bsSlopeA2UpDown.Value = 0 Then
                BsErrorProvider1.SetError(bsSlopeA2UpDown, GetMessageText(GlobalEnumerates.Messages.ZERO_NOTALLOW.ToString)) 'AG 07/07/2010("ZERO_NOTALLOW"))
                If (setFocusTo = -1) Then setFocusTo = 5

            ElseIf Not bsSlopeA2UpDown.Text = "" AndAlso bsSlopeB2UpDown.Text = "" Then
                BsErrorProvider1.SetError(bsSlopeB2UpDown, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString)) 'AG 07/07/2010("REQUIRED_VALUE"))
                If (setFocusTo = -1) Then setFocusTo = 6

                'AG 07/07/2010 - If B informed then A is required
            ElseIf Not bsSlopeB2UpDown.Text = "" AndAlso bsSlopeA2UpDown.Text = "" Then
                BsErrorProvider1.SetError(bsSlopeA2UpDown, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString))
                If (setFocusTo = -1) Then setFocusTo = 5

            End If
            ' WE 26/08/2014 - #1865 - End.

            'Select the proper field to put the focus
            If (setFocusTo >= 0) Then
                fieldsOK = False

                'RH 08/06/2012
                Select Case setFocusTo
                    Case 0
                        bsFullNameTextbox.Focus()
                    Case 1
                        bsShortNameTextbox.Focus()
                    Case 2
                        bsUnitComboBox.Focus()
                    Case 3
                        bsISETestTabControl.SelectedTab = DetailsTabPage
                    Case 4
                        bsISETestTabControl.SelectedTab = QCTabPage
                    Case 5
                        bsISETestTabControl.SelectedTab = DetailsTabPage
                        bsSlopeA2UpDown.Focus()
                    Case 6
                        bsISETestTabControl.SelectedTab = DetailsTabPage
                        bsSlopeB2UpDown.Focus()
                End Select
            Else
                'All mandatory fields have been completed, verify that the Full Name is unique.
                ' WE 29/07/2014 - #1865
                Dim resultDataName As GlobalDataTO
                Dim myISETestDelegateName As New ISETestsDelegate

                resultDataName = myISETestDelegateName.ExistsISETestName(Nothing, bsFullNameTextbox.Text, "LNAME", SelectedISETestID)
                If (Not resultDataName.HasError AndAlso Not resultDataName.SetDatos Is Nothing) Then
                    Dim myISETestsDSname As ISETestsDS
                    myISETestsDSname = DirectCast(resultDataName.SetDatos, ISETestsDS)

                    If (myISETestsDSname.tparISETests.Rows.Count > 0) Then
                        fieldsOK = False

                        BsErrorProvider1.SetError(bsFullNameTextbox, GetMessageText(GlobalEnumerates.Messages.DUPLICATED_TEST_NAME.ToString))
                        bsFullNameTextbox.Focus()
                    End If
                End If
                ' WE 29/07/2014 - #1865 - End.

                'All mandatory fields are informed, verify the informed ShortName is unique
                Dim resultData As GlobalDataTO
                Dim myISETestDelegate As New ISETestsDelegate

                resultData = myISETestDelegate.ExistsISETestName(Nothing, bsShortNameTextbox.Text, "NAME", SelectedISETestID)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    Dim myISETestsDS As ISETestsDS
                    myISETestsDS = DirectCast(resultData.SetDatos, ISETestsDS)

                    If (myISETestsDS.tparISETests.Rows.Count > 0) Then
                        fieldsOK = False

                        BsErrorProvider1.SetError(bsShortNameTextbox, GetMessageText(GlobalEnumerates.Messages.DUPLICATED_TEST_SHORTNAME.ToString))
                        bsShortNameTextbox.Focus()
                    End If
                End If
            End If

            ValidationError = (Not fieldsOK)

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ValidateSavingConditions", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ValidateSavingConditions", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
        Return fieldsOK
    End Function

    ''' <summary>
    ''' Enable or disable functionallity by user level.
    ''' </summary>
    ''' <remarks>
    ''' CREATED BY: TR 23/04/2012
    ''' Modified by XB 04/02/2013 - Upper conversions redundants because the value is already in UpperCase must delete to avoid Regional Settings problems (Bugs tracking #1112)
    ''' </remarks>
    Private Sub ScreenStatusByUserLevel()
        Try
            Select Case CurrentUserLevel    '.ToUpper()
                Case "SUPERVISOR"
                    Exit Select

                Case "OPERATOR"
                    bsEditButton.Enabled = False
                    bsCustomOrderButton.Enabled = True 'AG 05/09/2014 - BA-1869
                    'bsPrintButton.Enabled = False dl 11/05/2012
                    Exit Select
            End Select

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & "ScreenStatusByUserLevel ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & "ScreenStatusByUserLevel ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub


#End Region

#Region "CONTROLS"

    ''' <summary>
    ''' SetUp QCTab controls.
    ''' </summary>
    ''' <remarks>
    ''' CREATE BY: RH 05/06/2012
    ''' </remarks>
    Private Sub InitializeQCTab()
        Try
            ReadQCLimits()

            PrepareISETestControlsGrid()

            UsedControlsGridView.DataSource = SelectedTestControlDS.tparTestControls

            QCActiveCheckBox.Checked = False

            QCReplicNumberNumeric.Value = QCReplicNumberNumeric.Minimum

            QCRejectionCriteria.Value = QCRejectionCriteria.Minimum

            'QCMinNumSeries.Value = QCMinNumSeries.Minimum

            ManualRadioButton.Checked = True 'Set Manual as default Calculation Mode
            'StaticRadioButton.Checked = False
            QCMinNumSeries.ResetText()
            'QCMinNumSeries.Enabled = False

            QCErrorAllowable.Value = QCErrorAllowable.Minimum

            'Uncheck all multirules excepting the 1-2s, which has to be checked and disabled in all screen modes
            's12CheckBox.Checked = False
            s13CheckBox.Checked = False
            x22CheckBox.Checked = False
            r4sCheckBox.Checked = False
            s41CheckBox.Checked = False
            x10CheckBox.Checked = False

            bsQCPanel.Enabled = False

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "PrepareQCTab " & Name, EventLogEntryType.Error, _
                              GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Setup the Used Controls Data grid view.
    ''' </summary>
    ''' <remarks>
    ''' CREATED BY: RH 05/06/2012
    ''' </remarks>
    Private Sub PrepareISETestControlsGrid()
        Try

            UsedControlsGridView.DataSource = Nothing
            UsedControlsGridView.Columns.Clear()
            UsedControlsGridView.Rows.Clear()
            UsedControlsGridView.AutoGenerateColumns = False
            UsedControlsGridView.AllowUserToDeleteRows = False
            UsedControlsGridView.AllowUserToResizeColumns = True

            'Set the fore color to black.
            UsedControlsGridView.RowsDefaultCellStyle.SelectionForeColor = Color.Black

            UsedControlsGridView.EditMode = DataGridViewEditMode.EditOnEnter

            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            Dim ActiveControlColChkBox As New DataGridViewCheckBoxColumn
            ActiveControlColChkBox.Width = 20
            ActiveControlColChkBox.Name = "ActiveControl"
            ActiveControlColChkBox.HeaderText = ""
            ActiveControlColChkBox.DataPropertyName = "ActiveControl"
            ActiveControlColChkBox.Resizable = DataGridViewTriState.False
            UsedControlsGridView.Columns.Add(ActiveControlColChkBox)


            Dim ControlNameComboBoxRow As New DataGridViewComboBoxColumn
            ControlNameComboBoxRow.HeaderText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Control_Name", currentLanguage)
            ControlNameComboBoxRow.Name = "ControlName"
            ControlNameComboBoxRow.DataPropertyName = "ControlID"
            ControlNameComboBoxRow.Width = 140
            ControlNameComboBoxRow.DisplayStyle = DataGridViewComboBoxDisplayStyle.ComboBox
            ControlNameComboBoxRow.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft

            'Get the data to show on combo
            ControlListDS = GetAllQCControls()
            ControlNameComboBoxRow.DataSource = ControlListDS.tparControls
            ControlNameComboBoxRow.DisplayMember = "ControlName"
            ControlNameComboBoxRow.ValueMember = "ControlID"
            ControlNameComboBoxRow.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
            UsedControlsGridView.Columns.Add(ControlNameComboBoxRow)

            Dim LotNumberColumn As New DataGridViewTextBoxColumn
            LotNumberColumn.HeaderText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_LotNumber", currentLanguage)
            LotNumberColumn.Name = "LotNumber"
            LotNumberColumn.DataPropertyName = "LotNumber"
            LotNumberColumn.ReadOnly = True
            LotNumberColumn.Width = 90
            LotNumberColumn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
            UsedControlsGridView.Columns.Add(LotNumberColumn)

            Dim ExpirationDateRow As New DataGridViewTextBoxColumn
            ExpirationDateRow.HeaderText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_ExpDate_Short", currentLanguage)
            ExpirationDateRow.Name = "ExpDate"
            ExpirationDateRow.DataPropertyName = "ExpirationDate"

            ExpirationDateRow.ReadOnly = True
            ExpirationDateRow.MaxInputLength = 10
            ExpirationDateRow.Width = 80
            UsedControlsGridView.Columns.Add(ExpirationDateRow)
            UsedControlsGridView.Columns("ExpDate").HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter
            UsedControlsGridView.Columns("ExpDate").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            UsedControlsGridView.Columns("ExpDate").DefaultCellStyle.Format = "dd'/'MM'/'yyyy"

            Dim MinRow As New DataGridViewTextBoxColumn
            MinRow.HeaderText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Min", currentLanguage)
            MinRow.MaxInputLength = 11
            MinRow.Name = "MinConcentration"
            MinRow.DataPropertyName = "MinConcentration"
            MinRow.Width = 62
            UsedControlsGridView.Columns.Add(MinRow)
            UsedControlsGridView.Columns("MinConcentration").HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight
            UsedControlsGridView.Columns("MinConcentration").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight

            Dim MaxRow As New DataGridViewTextBoxColumn
            MaxRow.HeaderText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Max", currentLanguage)
            MaxRow.Name = "MaxConcentration"
            MaxRow.MaxInputLength = 11
            MaxRow.DataPropertyName = "MaxConcentration"
            MaxRow.Width = 62
            UsedControlsGridView.Columns.Add(MaxRow)
            UsedControlsGridView.Columns("MaxConcentration").HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight
            UsedControlsGridView.Columns("MaxConcentration").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight

            Dim TargetMean As New DataGridViewTextBoxColumn
            TargetMean.HeaderText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_TargetMean", currentLanguage)
            TargetMean.Name = "TargetMean"
            TargetMean.DataPropertyName = "TargetMean"
            TargetMean.ReadOnly = True
            TargetMean.Width = 85
            TargetMean.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            TargetMean.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight
            UsedControlsGridView.Columns.Add(TargetMean)

            Dim TargetSD As New DataGridViewTextBoxColumn
            TargetSD.HeaderText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_TargetSD", currentLanguage)
            TargetSD.Name = "TargetSD"
            TargetSD.DataPropertyName = "TargetSD"
            TargetSD.ReadOnly = True
            TargetSD.Width = 85
            TargetSD.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            TargetSD.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight
            UsedControlsGridView.Columns.Add(TargetSD)

            Dim ControlID As New DataGridViewTextBoxColumn
            ControlID.Name = "ControlID"
            ControlID.Visible = False
            ControlID.DataPropertyName = "ControlID"
            UsedControlsGridView.Columns.Add(ControlID)

            Dim SampleTypeCol As New DataGridViewTextBoxColumn
            SampleTypeCol.Name = "SampleType"
            SampleTypeCol.Visible = False
            SampleTypeCol.DataPropertyName = "SampleType"
            UsedControlsGridView.Columns.Add(SampleTypeCol)

            Dim TestIDTypeCol As New DataGridViewTextBoxColumn
            TestIDTypeCol.Name = "TestID"
            TestIDTypeCol.Visible = False
            TestIDTypeCol.DataPropertyName = "TestID"
            UsedControlsGridView.Columns.Add(TestIDTypeCol)

            Dim TestTypeCol As New DataGridViewTextBoxColumn
            TestTypeCol.Name = "TestType"
            TestTypeCol.Visible = False
            TestTypeCol.DataPropertyName = "TestType"
            UsedControlsGridView.Columns.Add(TestTypeCol)

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", " PrepareUsedControlsGrid " & Name, EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Reads the limits to the QC Controls.
    ''' </summary>
    ''' <remarks>
    ''' CREATE BY: RH 05/06/2012
    ''' </remarks>
    Private Sub ReadQCLimits()
        Try
            'Prepare the limits for numeric controls.
            Dim myFieldLimitsDS As New FieldLimitsDS

            'Control Replicates.
            myFieldLimitsDS = GetControlsLimits(FieldLimitsEnum.CONTROL_REPLICATES)
            If myFieldLimitsDS.tfmwFieldLimits.Count > 0 Then
                QCReplicNumberNumeric.Minimum = CType(myFieldLimitsDS.tfmwFieldLimits(0).MinValue, Decimal)
                QCReplicNumberNumeric.Maximum = CType(myFieldLimitsDS.tfmwFieldLimits(0).MaxValue, Decimal)
                If Not myFieldLimitsDS.tfmwFieldLimits(0).IsStepValueNull Then
                    QCReplicNumberNumeric.Increment = CType(myFieldLimitsDS.tfmwFieldLimits(0).StepValue, Decimal)
                End If
            End If

            'Control Rejection.
            myFieldLimitsDS = GetControlsLimits(FieldLimitsEnum.CONTROL_REJECTION)
            If myFieldLimitsDS.tfmwFieldLimits.Count > 0 Then
                QCRejectionCriteria.Minimum = CType(myFieldLimitsDS.tfmwFieldLimits(0).MinValue, Decimal)
                QCRejectionCriteria.Maximum = CType(myFieldLimitsDS.tfmwFieldLimits(0).MaxValue, Decimal)
                If Not myFieldLimitsDS.tfmwFieldLimits(0).IsStepValueNull Then
                    QCRejectionCriteria.Increment = CType(myFieldLimitsDS.tfmwFieldLimits(0).StepValue, Decimal)
                End If
                If Not myFieldLimitsDS.tfmwFieldLimits(0).IsDecimalsAllowedNull Then
                    QCRejectionCriteria.DecimalPlaces = myFieldLimitsDS.tfmwFieldLimits(0).DecimalsAllowed
                End If
            End If

            'Control Min Num. Series.
            myFieldLimitsDS = GetControlsLimits(FieldLimitsEnum.CONTROL_MIN_NUM_SERIES)
            If myFieldLimitsDS.tfmwFieldLimits.Count > 0 Then
                QCMinNumSeries.Minimum = CType(myFieldLimitsDS.tfmwFieldLimits(0).MinValue, Decimal)
                QCMinNumSeries.Maximum = CType(myFieldLimitsDS.tfmwFieldLimits(0).MaxValue, Decimal)
                If Not myFieldLimitsDS.tfmwFieldLimits(0).IsStepValueNull Then
                    QCMinNumSeries.Increment = CType(myFieldLimitsDS.tfmwFieldLimits(0).StepValue, Decimal)
                End If
            End If

            'Get allowed limits for fields Min/Max Concentration
            myFieldLimitsDS = GetControlsLimits(FieldLimitsEnum.CONTROL_MIN_MAX_CONC)

            If (myFieldLimitsDS.tfmwFieldLimits.Rows.Count = 1) Then
                MinAllowedConcentration = myFieldLimitsDS.tfmwFieldLimits(0).MinValue
                MaxAllowedConcentration = myFieldLimitsDS.tfmwFieldLimits(0).MaxValue
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "SetQCLimits " & Name, EventLogEntryType.Error, _
                                                 GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Method in charge to get the controls limits value. 
    ''' </summary>
    ''' <param name="pLimitsID">Limit to get</param>
    ''' <returns></returns>
    ''' <remarks>CREATED BY: TR</remarks>
    Private Function GetControlsLimits(ByVal pLimitsID As FieldLimitsEnum, Optional ByVal pAnalyzerModel As String = "") As FieldLimitsDS
        Dim myFieldLimitsDS As New FieldLimitsDS

        Try
            Dim myGlobalDataTO As GlobalDataTO

            Dim myFieldLimitsDelegate As New FieldLimitsDelegate()
            'Load the time Cycles control
            myGlobalDataTO = myFieldLimitsDelegate.GetList(Nothing, pLimitsID, pAnalyzerModel)

            If Not myGlobalDataTO.HasError Then
                myFieldLimitsDS = CType(myGlobalDataTO.SetDatos, FieldLimitsDS)
            Else
                ShowMessage("Error", myGlobalDataTO.ErrorCode)
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " GetControlsLimits ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))") 'AG 07/07/2010  "SYSTEM_ERROR", ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try

        Return myFieldLimitsDS

    End Function

    ''' <summary>
    ''' Get all the Contron on database.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>
    ''' CREATED BY: TR 07/04/2011
    ''' </remarks>
    Private Function GetAllQCControls() As ControlsDS
        Dim myControlsDS As New ControlsDS

        Try
            Dim myGlobalDataTO As GlobalDataTO
            Dim myControlsDelegate As New ControlsDelegate

            'get control data 
            myGlobalDataTO = myControlsDelegate.GetAll(Nothing)

            If Not myGlobalDataTO.HasError Then
                'TR 06/10/2011 -INSERT EMPTY ROW
                Dim myControlRow As ControlsDS.tparControlsRow
                myControlRow = myControlsDS.tparControls.NewtparControlsRow
                'myControlRow.ControlID = 0
                myControlRow.ControlName = ""
                myControlsDS.tparControls.AddtparControlsRow(myControlRow)

                'myControlsDS = DirectCast(myGlobalDataTO.SetDatos, ControlsDS)
                For Each controlRow As ControlsDS.tparControlsRow In _
                                            DirectCast(myGlobalDataTO.SetDatos, ControlsDS).tparControls.Rows
                    myControlsDS.tparControls.ImportRow(controlRow)
                Next
                'TR 06/10/2011 -END
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", " GetAllQCControls " & Name, EventLogEntryType.Error, _
                                                      GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try

        Return myControlsDS
    End Function

    ''' <summary>
    ''' Get Information about specific control 
    ''' </summary>
    ''' <param name="pControlID">Control ID </param>
    ''' <returns></returns>
    ''' <remarks>
    ''' CREATED BY: TR 06/04/2011
    ''' </remarks>
    Private Function GetQCControlInfo(ByVal pControlID As Integer) As ControlsDS
        Dim myControlsDS As New ControlsDS
        Try
            Dim myGlobalDataTO As New GlobalDataTO
            Dim myControlsDelegate As New ControlsDelegate
            'get control data 
            myGlobalDataTO = myControlsDelegate.GetControlData(Nothing, pControlID)
            If Not myGlobalDataTO.HasError Then
                myControlsDS = DirectCast(myGlobalDataTO.SetDatos, ControlsDS)
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", " GetQCControlsInfo " & Name, EventLogEntryType.Error, _
                                                      GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
        Return myControlsDS
    End Function

    ''' <summary>
    ''' Bind al the QC controls on the QC Tab.
    ''' </summary>
    ''' <param name="pTestID"></param>
    ''' <remarks>
    ''' CREATED BY: RH 06/06/2012
    ''' </remarks>
    Private Sub BindISETestQCData(ByVal pTestID As Integer, ByVal pSampleType As String)
        Dim SaveEditionMode As Boolean = EditionMode

        Try
            Dim myGlobalDataTO As GlobalDataTO
            'Dim myTestSampleMultiDS As TestSamplesMultirulesDS
            Dim myTestSampleMultiDelegate As New TestSamplesMultirulesDelegate

            EditionMode = False

            'For the selected ISE TestID/SampleType, get data of fields ControlReplicates, RejectionCriteria,
            'CalculationMode, NumberOfSeries and TotalAllowedError from global DataSet SelectedISETestSamplesDS
            '(loaded in function LoadSampleTypesList)
            Dim qTestSamples As List(Of ISETestSamplesDS.tparISETestSamplesRow)

            qTestSamples = (From a In SelectedISETestSamplesDS.tparISETestSamples _
                            Where String.Compare(a.SampleType, pSampleType, False) = 0 _
                            AndAlso a.ISETestID = pTestID _
                            Select a).ToList()

            QCActiveCheckBox.Checked = qTestSamples.First().QCActive

            If Not qTestSamples.First().IsControlReplicatesNull Then
                QCReplicNumberNumeric.Text = qTestSamples.First().ControlReplicates.ToString()
            Else
                QCReplicNumberNumeric.ResetText()
            End If

            If Not qTestSamples.First().IsRejectionCriteriaNull Then
                QCRejectionCriteria.Value = CDec(qTestSamples.First().RejectionCriteria)
            Else
                QCRejectionCriteria.ResetText()
            End If

            'Validate the calculation mode to select the corresponding CheckBox.
            If Not qTestSamples.First().IsCalculationModeNull AndAlso _
                        qTestSamples.First().CalculationMode = "MANUAL" Then
                ManualRadioButton.Checked = True
                QCMinNumSeries.Enabled = False
            ElseIf Not qTestSamples.First().IsCalculationModeNull AndAlso _
                        qTestSamples.First().CalculationMode = "STATISTIC" Then
                StaticRadioButton.Checked = True
                QCMinNumSeries.Enabled = True
            Else
                ManualRadioButton.Checked = False
                StaticRadioButton.Checked = False
                QCMinNumSeries.Enabled = False
            End If

            If Not qTestSamples.First().IsNumberOfSeriesNull AndAlso _
                    Not qTestSamples.First().NumberOfSeries = 0 Then
                QCMinNumSeries.Text = qTestSamples.First().NumberOfSeries.ToString()
            Else
                QCMinNumSeries.ResetText()
            End If

            If Not qTestSamples.First().IsTotalAllowedErrorNull Then
                QCErrorAllowable.Value = CDec(qTestSamples.First().TotalAllowedError)
            Else
                QCErrorAllowable.ResetText()
            End If

            'Data For Rule To Apply
            myGlobalDataTO = myTestSampleMultiDelegate.GetByTestIDAndSampleTypeNEW(Nothing, "ISE", pTestID, pSampleType)

            If Not myGlobalDataTO.HasError Then
                SelectedTestSampleMultirulesDS = DirectCast(myGlobalDataTO.SetDatos, TestSamplesMultirulesDS)

                If SelectedTestSampleMultirulesDS.tparTestSamplesMultirules.Count > 0 Then
                    Dim qTestSampleMultiList As List(Of TestSamplesMultirulesDS.tparTestSamplesMultirulesRow)
                    qTestSampleMultiList = SelectedTestSampleMultirulesDS.tparTestSamplesMultirules.ToList()

                    's12CheckBox.Checked = _
                    '    qTestSampleMultiList.Where(Function(a) a.RuleID = "WESTGARD_1-2s").ToList().First().SelectedRule()

                    s13CheckBox.Checked = _
                        qTestSampleMultiList.Where(Function(a) a.RuleID = "WESTGARD_1-3s").First().SelectedRule

                    x22CheckBox.Checked = _
                        qTestSampleMultiList.Where(Function(a) a.RuleID = "WESTGARD_2-2s").First().SelectedRule

                    r4sCheckBox.Checked = _
                        qTestSampleMultiList.Where(Function(a) a.RuleID = "WESTGARD_R-4s").First().SelectedRule

                    s41CheckBox.Checked = _
                        qTestSampleMultiList.Where(Function(a) a.RuleID = "WESTGARD_4-1s").First().SelectedRule

                    x10CheckBox.Checked = _
                        qTestSampleMultiList.Where(Function(a) a.RuleID = "WESTGARD_10X").First().SelectedRule
                Else
                    's12CheckBox.Checked = False
                    s13CheckBox.Checked = False
                    x22CheckBox.Checked = False
                    r4sCheckBox.Checked = False
                    s41CheckBox.Checked = False
                    x10CheckBox.Checked = False
                End If

            Else
                ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, myGlobalDataTO.ErrorMessage)
            End If
            ' END Data For Rule To Apply

            '            Dim myControlsDS As New ControlsDS
            '            Dim myControlsDelegate As New ControlsDelegate
            Dim myTestControlDelegate As New TestControlsDelegate

            myGlobalDataTO = myTestControlDelegate.GetControlsNEW(Nothing, "ISE", pTestID, pSampleType)

            If Not myGlobalDataTO.HasError Then
                SelectedTestControlDS = DirectCast(myGlobalDataTO.SetDatos, TestControlsDS)
                ''Order by Active control so the check ones get the first positions
                'SelectedTestControlDS.tparTestControls.DefaultView.Sort = "ActiveControl DESC"
                PrepareISETestControlsGrid()
                UsedControlsGridView.DataSource = SelectedTestControlDS.tparTestControls
                UsedControlsGridView.ClearSelection()
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "BindISETestQCData " & Name, EventLogEntryType.Error, _
                                                            GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")

        Finally
            EditionMode = SaveEditionMode

        End Try
    End Sub

    ''' <summary>
    ''' Calculate the Target SD and the Target Mean on Used Controls Grid View.
    ''' </summary>
    ''' <param name="pRowIndex">Current Row index</param>
    ''' <param name="pColumnIndex">Current Column Index</param>
    ''' <remarks>
    ''' CREATED BY: RH 06/06/2012
    ''' </remarks>
    Private Sub CalculateTargetValues(ByVal pRowIndex As Integer, ByVal pColumnIndex As Integer)
        Try
            Dim dgv As BSDataGridView = UsedControlsGridView
            Dim ControlName As DataGridViewCell = dgv.Rows(pRowIndex).Cells("ControlName")
            Dim MinConcentration As DataGridViewCell = dgv.Rows(pRowIndex).Cells("MinConcentration")
            Dim MaxConcentration As DataGridViewCell = dgv.Rows(pRowIndex).Cells("MaxConcentration")
            Dim TargetMean As DataGridViewCell = dgv.Rows(pRowIndex).Cells("TargetMean")
            Dim TargetSD As DataGridViewCell = dgv.Rows(pRowIndex).Cells("TargetSD")

            'Clear all errors 
            ControlName.ErrorText = String.Empty
            MinConcentration.ErrorText = String.Empty
            MaxConcentration.ErrorText = String.Empty

            dgv.Rows(pRowIndex).Cells(pColumnIndex).Style.Alignment = DataGridViewContentAlignment.MiddleRight
            ControlName.Style.Alignment = DataGridViewContentAlignment.MiddleLeft
            MinConcentration.Style.Alignment = DataGridViewContentAlignment.MiddleRight
            MaxConcentration.Style.Alignment = DataGridViewContentAlignment.MiddleRight

            'Before calculation make sure Control is selected.
            If ControlName.Value Is DBNull.Value Then
                ControlName.ErrorText = GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString())
                Return
            End If

            'Validate there's not null values or zero.
            If Not MinConcentration.Value Is DBNull.Value AndAlso _
                Not MaxConcentration.Value Is DBNull.Value AndAlso _
                                Not CSng(MaxConcentration.Value) = 0 Then

                'Validate Before calculating
                If CSng(MinConcentration.Value) < CSng(MaxConcentration.Value) Then

                    'Calculate the Target Mean.
                    TargetMean.Value = ((CSng(MinConcentration.Value) + CSng(MaxConcentration.Value)) / 2)

                    'Calculate the Target SD.
                    TargetSD.Value = ((CSng(MaxConcentration.Value) - CSng(MinConcentration.Value)) / _
                                      (2 * QCRejectionCriteria.Value))
                Else
                    dgv.Rows(pRowIndex).Cells(pColumnIndex).Style.Alignment = DataGridViewContentAlignment.MiddleCenter
                    'Show Error 
                    dgv.Rows(pRowIndex).Cells(pColumnIndex).ErrorText = _
                                 GetMessageText(GlobalEnumerates.Messages.MIN_MUST_BE_LOWER_THAN_MAX.ToString())

                    TargetMean.Value = DBNull.Value
                    TargetSD.Value = DBNull.Value

                End If
            Else
                'Validate the related cell to show message.
                If MinConcentration.Value Is DBNull.Value Then
                    MinConcentration.Style.Alignment = DataGridViewContentAlignment.MiddleCenter
                    MinConcentration.ErrorText = GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString())

                ElseIf MaxConcentration.Value Is DBNull.Value OrElse CSng(MaxConcentration.Value) = 0 Then
                    MaxConcentration.Style.Alignment = DataGridViewContentAlignment.MiddleCenter
                    MaxConcentration.ErrorText = GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString())
                End If
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", " CalculatedTarget " & Name, EventLogEntryType.Error, _
                                                                           GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Activate/Desactivate the controls on QC TAB.
    ''' </summary>
    ''' <param name="pEnable"></param>
    ''' <remarks>
    ''' CREATED BY: TR 06/04/2011
    ''' </remarks>
    Private Sub ActivateQCControlsByQCActive(ByVal pEnable As Boolean)
        Try
            If Not EditionMode AndAlso pEnable Then
                pEnable = False
            End If

            AddControls.Enabled = pEnable
            DeleteControlButton.Enabled = pEnable

            'EnableDisableComboColumns(pEnable) ' dl 20/07/2011
            UsedControlsGridView.Enabled = pEnable

            RulesToApplyGroupBox.Enabled = pEnable

            'TR 25/01/2012 -Disable by defaul (s12CheckBox)
            s12CheckBox.Enabled = False

            ' dl 18/07/2011
            'ControlValuesGroupBox.Enabled = pEnable
            QCReplicNumberNumeric.Enabled = pEnable
            QCRejectionCriteria.Enabled = pEnable

            ' Six-Sigma Values Group Box is not used at this moment. Therefore its Visibility property is set to True at design-time.
            'SixSigmaValuesGroupBox.Enabled = pEnable
            QCErrorAllowable.Enabled = pEnable
            BsButton1.Enabled = pEnable


            'CalculationModeGroupBox.Enabled = pEnable
            ManualRadioButton.Enabled = pEnable
            StaticRadioButton.Enabled = pEnable
            QCMinNumSeries.Enabled = pEnable

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", " ActivateQCControlsByQCActive " & Name, EventLogEntryType.Error, _
                                                        GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Update the Test Sample Multirules controls.
    ''' </summary>
    ''' <param name="pTestID"></param>
    ''' <param name="pSampleType"></param>
    ''' <remarks>
    ''' CREATED BY: RH 08/06/2012
    ''' </remarks>
    Private Sub UpdateTestSampleMultiRules(ByVal pTestID As Integer, ByVal pSampleType As String)
        Try
            Dim qTestSampleMultiList As New List(Of TestSamplesMultirulesDS.tparTestSamplesMultirulesRow)

            qTestSampleMultiList = SelectedTestSampleMultirulesDS.tparTestSamplesMultirules.ToList()

            'WESTGARD_1-2s allways True
            qTestSampleMultiList.Where(Function(a) a.RuleID = _
                    "WESTGARD_1-2s").First().SelectedRule = True

            qTestSampleMultiList.Where(Function(a) a.RuleID = _
                    "WESTGARD_1-3s").First().SelectedRule = s13CheckBox.Checked

            qTestSampleMultiList.Where(Function(a) a.RuleID = _
                    "WESTGARD_2-2s").First().SelectedRule = x22CheckBox.Checked

            qTestSampleMultiList.Where(Function(a) a.RuleID = _
                    "WESTGARD_R-4s").First().SelectedRule = r4sCheckBox.Checked

            qTestSampleMultiList.Where(Function(a) a.RuleID = _
                    "WESTGARD_4-1s").First().SelectedRule = s41CheckBox.Checked

            qTestSampleMultiList.Where(Function(a) a.RuleID = _
                    "WESTGARD_10X").First().SelectedRule = x10CheckBox.Checked

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", " UpdateTestSampleMultiRules " & Name, EventLogEntryType.Error, _
                                                                    GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Validate the QC Tab
    ''' </summary>
    ''' <returns>
    ''' If error found return True.
    ''' Else False.
    ''' </returns>
    ''' <remarks>
    ''' CREATED BY: TR 07/04/2011
    ''' </remarks>
    Private Function ValidateErrorOnQCTab(Optional ByVal pSaveButtonClicked As Boolean = False) As Boolean
        Dim ErrorFound As Boolean = False

        Try
            'BsErrorProvider1.Clear() RH 22/06/2012 Don't remove previous error notifications

            'RH 02/07/2012 No error to check if QC is not active
            If Not QCActiveCheckBox.Checked Then
                Return False
            End If

            If String.Equals(QCReplicNumberNumeric.Text, String.Empty) Then
                BsErrorProvider1.SetError(QCReplicNumberNumeric, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString))
                ErrorFound = True
            End If

            If String.Equals(QCRejectionCriteria.Text, String.Empty) Then
                BsErrorProvider1.SetError(QCRejectionCriteria, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString))
                ErrorFound = True
            End If

            If StaticRadioButton.Checked AndAlso String.Equals(QCMinNumSeries.Text, String.Empty) Then
                BsErrorProvider1.SetError(QCMinNumSeries, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString))
                ErrorFound = True
            End If

            'Valida USedControls Grid 
            'ErrorFound = ValidateErrorOnQCUsedControls()
            If Not ErrorFound Then ErrorFound = ValidateErrorOnQCUsedControls() 'RH 22/06/2012 Keep the previous 'true' value, if exist

            ValidationError = ErrorFound

            If Not ValidationError Then
                'BsErrorProvider1.Clear() RH 22/06/2012 Don't remove previous error notifications
            Else
                'Go to wrong TAB only when saving over the database
                'If pSaveButtonClicked Then bsISETestTabControl.SelectTab("QCTabPage")
                If pSaveButtonClicked Then
                    bsISETestTabControl.SelectedTab = QCTabPage 'RH 05/06/2012
                End If
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", " ValidateErrorOnQCTab " & Name, EventLogEntryType.Error, _
                                                                    GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try

        Return ErrorFound
    End Function

    ''' <summary>
    ''' Validate all rows on the USedControls Gridview
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>CREATED BY: TR 13/04/2011</remarks>
    Private Function ValidateErrorOnQCUsedControls() As Boolean
        Dim ErrorFound As Boolean = False

        Try
            'BsErrorProvider1.Clear() RH 22/06/2012 Don't remove previous error notifications

            'Validate the UsedControlsGridView
            For Each usedControlRow As DataGridViewRow In Me.UsedControlsGridView.Rows
                'If Not usedControlRow.Cells("ControID").Value Is DBNull.Value _
                'OrElse Not usedControlRow.Cells("ControID").Value Is Nothing _
                'OrElse Not usedControlRow.Cells("ControID").Value.ToString = String.Empty Then
                usedControlRow.Cells("MinConcentration").Style.Alignment = _
                                                                        DataGridViewContentAlignment.MiddleRight
                'usedControlRow.Cells("ControlName").Style.Alignment = _
                '                                         DataGridViewContentAlignment.MiddleRight' TR 11/01/2011 -Commented
                usedControlRow.Cells("MaxConcentration").Style.Alignment = _
                                                         DataGridViewContentAlignment.MiddleRight

                If Not usedControlRow.IsNewRow Then
                    'Validate control Name = ID
                    If usedControlRow.Cells("ControlName").Value Is DBNull.Value Then
                        usedControlRow.Cells("ControlName").ErrorText = _
                           GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString())
                        ErrorFound = True
                        'usedControlRow.Cells("ControlName").Selected = True 'TR 07/10/2011 
                        'Exit For
                    End If

                    'Validate null values and Zero values
                    If Not usedControlRow.Cells("MinConcentration").Value Is DBNull.Value AndAlso _
                       Not usedControlRow.Cells("MaxConcentration").Value Is DBNull.Value AndAlso _
                       Not CSng(usedControlRow.Cells("MinConcentration").Value) < 0 AndAlso _
                       Not CSng(usedControlRow.Cells("MaxConcentration").Value) = 0 Then
                        'Validate if min is equal or greather than max
                        If CSng(usedControlRow.Cells("MinConcentration").Value) >= _
                                                        CSng(usedControlRow.Cells("MaxConcentration").Value) Then
                            usedControlRow.Cells("MinConcentration").Style.Alignment = _
                                                                                    DataGridViewContentAlignment.MiddleCenter
                            usedControlRow.Cells("MinConcentration").ErrorText = _
                                                GetMessageText(GlobalEnumerates.Messages.MIN_MUST_BE_LOWER_THAN_MAX.ToString)
                            ErrorFound = True
                        End If

                    ElseIf usedControlRow.Cells("MinConcentration").Value Is DBNull.Value OrElse _
                            CSng(usedControlRow.Cells("MinConcentration").Value) < 0 Then
                        usedControlRow.Cells("MinConcentration").Style.Alignment = _
                                                                                DataGridViewContentAlignment.MiddleCenter
                        usedControlRow.Cells("MinConcentration").ErrorText = _
                                                    GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString())
                        ErrorFound = True
                    ElseIf usedControlRow.Cells("MaxConcentration").Value Is DBNull.Value OrElse _
                                                    CSng(usedControlRow.Cells("MaxConcentration").Value) = 0 Then
                        usedControlRow.Cells("MaxConcentration").Style.Alignment = _
                                                                                DataGridViewContentAlignment.MiddleCenter
                        usedControlRow.Cells("MaxConcentration").ErrorText = _
                                                        GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString())
                        ErrorFound = True
                    End If
                    'TR 11/10/2011 -Commented
                    'If ErrorFound Then
                    '    Exit For
                    'End If
                    'TR 11/10/2011 -END.
                End If

                'End If
            Next

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", " ValidateQCUsedControls " & Name, EventLogEntryType.Error, _
                                                                   GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try

        Return ErrorFound
    End Function

    ''' <summary>
    ''' Validate if selected control exist on Test sample.
    ''' </summary>
    ''' <param name="pControlID">Control ID </param>
    ''' <returns></returns>
    ''' <remarks>
    ''' CREATED BY TR 07/04/2011
    ''' </remarks>
    Private Function ControlExist(ByVal pControlID As Integer) As Boolean
        Dim LocalControlExist As Boolean = False

        Try
            Dim qTestControlExist As List(Of TestControlsDS.tparTestControlsRow)
            qTestControlExist = (From a In SelectedTestControlDS.tparTestControls _
                                 Where Not a.IsControlIDNull AndAlso _
                                 a.ControlID = pControlID Select a).ToList
            If qTestControlExist.Count > 0 Then
                LocalControlExist = True
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", " ControlExist " & Name, EventLogEntryType.Error, _
                                                                    GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try

        Return LocalControlExist
    End Function

    ''' <summary>
    ''' Count the active control for selected TestSample
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>
    ''' CREATED BY: TR 07/04/2011
    ''' </remarks>
    Private Function CountActiveControl() As Integer
        Dim ActiveControlCount As Integer = 0

        Try
            UsedControlsGridView.Refresh()

            For Each TestControlRow As DataGridViewRow In UsedControlsGridView.Rows
                If CBool(TestControlRow.Cells("ActiveControl").Value) Then
                    ActiveControlCount += 1
                End If
            Next

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", " CountActiveControl " & Name, EventLogEntryType.Error, _
                                                                    GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try

        Return ActiveControlCount
    End Function

    ''' <summary>
    ''' Recalcualtes all the Target values on the UsedControlsGridView.
    ''' </summary>
    ''' <remarks>
    ''' CREATED BY: RH 07/06/2012
    ''' </remarks>
    Private Sub RecalculateAllTarget()
        Try
            Dim MinConColIndex As Integer = UsedControlsGridView.Columns("MinConcentration").Index
            Dim MaxConColIndex As Integer = UsedControlsGridView.Columns("MaxConcentration").Index

            'Recalculate the target For each control 
            For Each UsedControlRow As DataGridViewRow In UsedControlsGridView.Rows
                If Not UsedControlRow.Cells("MinConcentration").Value Is Nothing Then
                    For Each UsedControlCell As DataGridViewCell In UsedControlRow.Cells
                        If UsedControlCell.ColumnIndex = MinConColIndex OrElse UsedControlCell.ColumnIndex = MaxConColIndex Then
                            CalculateTargetValues(UsedControlCell.RowIndex, UsedControlCell.ColumnIndex)
                        End If
                    Next
                End If
            Next

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", " RecalculateAllTarget " & Name, EventLogEntryType.Error, _
                                                                   GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try

    End Sub

    ''' <summary>
    ''' Delete the selected control on the UsedControl Datagrid
    ''' </summary>
    ''' <remarks>
    ''' CREATED BY: RH 08/06/2012
    ''' </remarks>
    Private Sub DeleteSelectedControl()
        Try
            If UsedControlsGridView.SelectedRows.Count > 0 AndAlso _
                    ShowMessage("Warning", GlobalEnumerates.Messages.DELETE_CONFIRMATION.ToString) = _
                              DialogResult.Yes Then

                If UsedControlsGridView.SelectedRows.Count > 0 Then
                    Dim TestControlToRemove As New List(Of DataGridViewRow)

                    For Each TestControlRow As DataGridViewRow In UsedControlsGridView.SelectedRows
                        If Not TestControlRow.Cells("ControlID").Value Is Nothing Then
                            Dim TestIDCell As DataGridViewCell = TestControlRow.Cells("TestID")

                            If Not TestIDCell.Value Is DBNull.Value AndAlso Not TestIDCell.Value Is Nothing Then
                                Dim myDeleteControlTO As New DeletedControlTO

                                myDeleteControlTO.ControlID = CInt(TestControlRow.Cells("ControlID").Value)
                                myDeleteControlTO.TestID = CInt(TestIDCell.Value)
                                myDeleteControlTO.SampleType = TestControlRow.Cells("SampleType").Value.ToString()

                                LocalDeleteControlTOList.Add(myDeleteControlTO)
                                ChangesMade = True

                                TestControlToRemove.Add(TestControlRow)
                                'Else
                            End If
                        End If
                    Next



                    For Each row As DataGridViewRow In UsedControlsGridView.SelectedRows
                        'JB 26/09/2012 - Correction: Avoid exception removing not commited row
                        If row.Cells("ControlID").Value IsNot Nothing Then
                            UsedControlsGridView.Rows.Remove(row)
                        End If
                    Next

                    SelectedTestControlDS.AcceptChanges()
                End If

                If UsedControlsGridView.Rows.Count = 0 Then
                    UsedControlsGridView.AllowUserToAddRows = True
                End If
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", " DeleteSelectedControl " & Name, EventLogEntryType.Error, _
                                                                  GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Search changes in test - sample type that implicates delete old results for controls to reuse
    ''' Adapt from Ax5 (frmTecnicas.AnalizarSihayCambiosEnTecnica)
    ''' </summary>
    ''' <returns>
    ''' DialogResult
    ''' </returns>
    ''' <remarks>
    ''' Created by RH 13/06/2012
    ''' </remarks>
    Private Function SavePendingWarningMessage() As DialogResult
        Dim dialogResultToReturn As DialogResult = DialogResult.OK

        Try
            If ChangesMade Then
                Dim myPreloadeDelegate As New PreloadedMasterDataDelegate
                Dim imageBytes As Byte() = myPreloadeDelegate.GetIconImage("CTRL")

                Dim myQCResultsDelegate As New QCResultsDelegate
                Dim myHistoryQCInfoDS As HistoryQCInformationDS
                Dim myHistoryQCInfoList As List(Of HistoryQCInformationDS.HistoryQCInfoTableRow)

                Dim myDependenciesElementsDS As New DependenciesElementsDS

                'Validate if there are deleted controls.
                If LocalDeleteControlTOList.Count > 0 Then
                    Dim myGlobalDataTO As GlobalDataTO
                    Dim myDependenciesElementsRow As DependenciesElementsDS.DependenciesElementsRow

                    For Each myDelControTo As DeletedControlTO In LocalDeleteControlTOList
                        myGlobalDataTO = myQCResultsDelegate.SearchPendingResultsByTestIDSampleTypeNEW( _
                                            Nothing, "ISE", myDelControTo.TestID, myDelControTo.SampleType)

                        If Not myGlobalDataTO.HasError Then
                            myHistoryQCInfoDS = DirectCast(myGlobalDataTO.SetDatos, HistoryQCInformationDS)

                            myHistoryQCInfoList = (From a In myHistoryQCInfoDS.HistoryQCInfoTable _
                                                   Where a.ControlID = myDelControTo.ControlID _
                                                   Select a).ToList()

                            If myHistoryQCInfoList.Count > 0 Then
                                myDependenciesElementsRow = myDependenciesElementsDS.DependenciesElements.NewDependenciesElementsRow()
                                myDependenciesElementsRow.Type = imageBytes
                                myDependenciesElementsRow.Name = myHistoryQCInfoList.First().ControlName
                                myDependenciesElementsRow.FormProfileMember = myHistoryQCInfoList.First().TestName

                                myDependenciesElementsRow.FormProfileMember &= " " & CUMULATE_QCRESULTS_Label

                                myDependenciesElementsDS.DependenciesElements.AddDependenciesElementsRow(myDependenciesElementsRow)
                            End If
                        End If
                    Next
                End If

                If myDependenciesElementsDS.DependenciesElements.Count > 0 Then
                    Using myAffectedElementsWarning As New IWarningAfectedElements()
                        myAffectedElementsWarning.AffectedElements = myDependenciesElementsDS
                        myAffectedElementsWarning.ShowDialog()

                        dialogResultToReturn = myAffectedElementsWarning.DialogResult
                    End Using
                End If
            End If

        Catch ex As Exception
            'Write error SYSTEM_ERROR in the Application Log & show error message
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " SavePendingWarningMessage ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))") 'AG 07/07/2010  "SYSTEM_ERROR", ex.Message + " ((" + ex.HResult.ToString + "))")
            dialogResultToReturn = Windows.Forms.DialogResult.No
        End Try

        Return dialogResultToReturn
    End Function

    ''' <summary>
    ''' If the value informed is less than the allowed, set the minimum allowed as cell value
    ''' If the value informed is greater than the allowed, set the maximum allowed as cell value
    ''' Finally, apply format to numeric cells:
    ''' ** Min, Max and TargetMean are shown with the number of decimals defined for the Test
    ''' ** TargetSD is shown with the number of decimals defined for the Test + 1
    ''' </summary>
    ''' <param name="pRow">Current Row index</param>
    ''' <param name="pCol">Current Column Index</param>
    ''' <remarks>
    ''' Created by: SA 18/05/2011
    ''' Modified by: RH 18/06/2012
    ''' </remarks>
    Private Sub SetLimitValues(ByVal pRow As Integer, ByVal pCol As Integer)
        Try
            Dim myDecimalSeparator As String = SystemInfoManager.OSDecimalSeparator
            Dim numDecimals As Integer = GetDecimals()
            Dim Cell As DataGridViewCell = UsedControlsGridView.Rows(pRow).Cells(pCol)

            If Not Cell.Value Is Nothing Then
                Dim myValue As String = Cell.Value.ToString()

                If (Not Cell.Value Is DBNull.Value) AndAlso (String.Compare(myValue, "", False) <> 0) AndAlso (myValue <> myDecimalSeparator) Then
                    'TR 19/01/2012 -Set the validation to < instead <= because cero value is allow.
                    'If (CSng(myValue) <= MinAllowedConcentration) Then 
                    If (CSng(myValue) < MinAllowedConcentration) Then
                        myValue = (MinAllowedConcentration + 1).ToString()

                    ElseIf (CSng(myValue) > MaxAllowedConcentration) Then
                        myValue = MaxAllowedConcentration.ToString()

                    End If

                    'Format the value
                    If (UsedControlsGridView.Columns(pCol).Name = "TargetSD") Then
                        numDecimals += 1
                    End If

                    Cell.Value = CSng(myValue).ToStringWithDecimals(numDecimals)
                End If

            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".SetLimitValues", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".SetLimitValues", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Get Decimals allowed for the current SelectedISETestID and SelectedSampleType
    ''' </summary>
    ''' <returns>
    ''' Decimals allowed
    ''' </returns>
    ''' <remarks>
    ''' Created by RH 19/06/2012
    ''' </remarks>
    Private Function GetDecimals() As Integer
        Dim decimalsNumber As Integer = 0

        Try
            Dim lstTestSamples As List(Of ISETestSamplesDS.tparISETestSamplesRow)

            lstTestSamples = (From a In SelectedISETestSamplesDS.tparISETestSamples _
                              Where a.ISETestID = SelectedISETestID _
                              And String.Equals(a.SampleType, SelectedSampleType) _
                              Select a).ToList()

            If lstTestSamples.Count > 0 Then
                decimalsNumber = lstTestSamples(0).Decimals
            End If

            Return decimalsNumber

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " GetDecimals ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".GetDecimals", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try

        Return decimalsNumber
    End Function

    ''' <summary>
    ''' Set the limits and step increments for all Numeric UpDown controls on the main part of the form.
    ''' </summary>
    ''' <remarks>
    ''' Created by: WE 31/07/2014 - #1865
    ''' </remarks>
    Private Sub SetISETestControlsLimits()
        Try
            Dim myFieldLimitsDS As New FieldLimitsDS()

            '** FIELDS IN MAIN PART OF SCREEN (ABOVE TAB CONTROL PART) ** '
            'Decimals
            myFieldLimitsDS = GetControlsLimits(FieldLimitsEnum.CTEST_NUM_DECIMALS)
            If (myFieldLimitsDS.tfmwFieldLimits.Rows.Count > 0) Then
                bsDecimalsUpDown.Minimum = CType(myFieldLimitsDS.tfmwFieldLimits(0).MinValue, Decimal)
                bsDecimalsUpDown.Maximum = CType(myFieldLimitsDS.tfmwFieldLimits(0).MaxValue, Decimal)
                bsDecimalsUpDown.DecimalPlaces = myFieldLimitsDS.tfmwFieldLimits(0).DecimalsAllowed

                If (Not myFieldLimitsDS.tfmwFieldLimits(0).IsStepValueNull) Then
                    bsDecimalsUpDown.Increment = CType(myFieldLimitsDS.tfmwFieldLimits(0).StepValue, Decimal)
                End If
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " SetISETestControlsLimits ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Setup the limits and step increments of all Numeric UpDown controls on the Options tab page. 
    ''' </summary>
    ''' <remarks>
    ''' Created by: WE 01/08/2014 - #1865 
    ''' </remarks>
    Private Sub SetDetailsControlLimits()
        Try
            Dim myFieldLimitsDS As New FieldLimitsDS()

            '** FIELDS IN OPTIONS TAB ** '
            'Slope Factor A2
            myFieldLimitsDS = GetControlsLimits(FieldLimitsEnum.SLOPE_FACTOR_A2)
            If myFieldLimitsDS.tfmwFieldLimits.Rows.Count > 0 Then
                bsSlopeA2UpDown.Minimum = CType(myFieldLimitsDS.tfmwFieldLimits(0).MinValue, Decimal)
                bsSlopeA2UpDown.Maximum = CType(myFieldLimitsDS.tfmwFieldLimits(0).MaxValue, Decimal)
                bsSlopeA2UpDown.DecimalPlaces = myFieldLimitsDS.tfmwFieldLimits(0).DecimalsAllowed

                If Not myFieldLimitsDS.tfmwFieldLimits(0).IsStepValueNull Then
                    bsSlopeA2UpDown.Increment = CType(myFieldLimitsDS.tfmwFieldLimits(0).StepValue, Decimal)
                End If
            End If

            'Slope Factor B2
            myFieldLimitsDS = GetControlsLimits(FieldLimitsEnum.SLOPE_FACTOR_B2)
            If myFieldLimitsDS.tfmwFieldLimits.Rows.Count > 0 Then
                bsSlopeB2UpDown.Maximum = CType(myFieldLimitsDS.tfmwFieldLimits(0).MaxValue, Decimal)
                bsSlopeB2UpDown.Minimum = CType(myFieldLimitsDS.tfmwFieldLimits(0).MinValue, Decimal)
                bsSlopeB2UpDown.DecimalPlaces = myFieldLimitsDS.tfmwFieldLimits(0).DecimalsAllowed

                If Not myFieldLimitsDS.tfmwFieldLimits(0).IsStepValueNull Then
                    SlopeBUpDown.Increment = CType(myFieldLimitsDS.tfmwFieldLimits(0).StepValue, Decimal)
                End If
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " SetDetailsControlLimits ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Initializes Options tab controls.
    ''' </summary>
    ''' <remarks>
    ''' Created by: WE 01/08/2014 - #1865 
    ''' </remarks>
    Private Sub InitializeOptionsTab()

        Try
            bsSlopeA2UpDown.Enabled = False
            bsSlopeA2UpDown.BackColor = SystemColors.MenuBar
            bsSlopeB2UpDown.Enabled = False
            bsSlopeB2UpDown.BackColor = SystemColors.MenuBar

            SetDetailsControlLimits()

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "InitializeOptionsTab " & Name, EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Set up Decimals Number
    ''' </summary>
    ''' <param name="pDecimals"></param>
    ''' <remarks>Created by: WE 25/08/2014 - #1865
    ''' </remarks>
    Private Sub SetupDecimalsNumber(ByVal pDecimals As Integer)
        Try
            Me.UsedControlsGridView.Refresh()

            Me.bsTestRefRanges.RefNumDecimals = pDecimals

            RecalculateAllTarget()

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " SetupDecimalsNumber ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

#End Region

#Region "Control Events"

    Private Sub UsedControlsGridView_RowEnter(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles UsedControlsGridView.RowEnter
        Try
            If e.RowIndex >= 0 Then
                Dim ControlNameValue As Object = UsedControlsGridView.Rows(e.RowIndex).Cells("ControlName").Value

                If EditionMode AndAlso Not ControlNameValue Is DBNull.Value AndAlso Not ControlNameValue Is Nothing Then
                    ControIDSel = ControlNameValue.ToString()
                Else
                    ControIDSel = String.Empty
                End If
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " UsedControlsGridView_RowEnter ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ValidateRefRangesSelection", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    'Private Sub ReadingSecUpDown_KeyPress(ByVal sender As System.Object, ByVal e As KeyPressEventArgs) Handles _
    '                QCReplicNumberNumeric.KeyPress, QCRejectionCriteria.KeyPress, QCMinNumSeries.KeyPress
    '    e.Handled = True
    'End Sub

    '''' <summary>
    '''' Activate the Change made variable when some value of 
    '''' the corresponding control is change.
    '''' </summary>
    '''' <param name="sender"></param>
    '''' <param name="e"></param>
    '''' <remarks>AG 18/03/2010 - complete control list and change function name ControlValueChanged</remarks>
    'Private Sub ControlValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _
    '                                QCReplicNumberNumeric.ValueChanged, QCRejectionCriteria, _
    '                                QCMinNumSeries.ValueChanged, QCErrorAllowable.ValueChanged

    '    If EditionMode Then
    '        'If ChangeSampleTypeDuringEdition Then
    '        '    ChangesMade = False 'AG 19/07/2010 (During edition user changes sampletype. all control are bind to new values but no changes are made!!)
    '        'Else
    '        '    ChangesMade = True
    '        'End If

    '        ChangesMade = Not ChangeSampleTypeDuringEdition
    '    End If

    'End Sub

    Private Sub editingGridComboBox_SelectionChangeCommitted(ByVal sender As Object, ByVal e As System.EventArgs)
        Try
            Dim myControlDS As ControlsDS
            Dim myComboBoxCell As ComboBox = CType(sender, ComboBox)
            'clear errors.
            UsedControlsGridView.CurrentRow.Cells("ControlName").ErrorText = String.Empty

            BsErrorProvider1.Clear()

            If EditionMode Then
                If Not myComboBoxCell.SelectedValue Is Nothing AndAlso myComboBoxCell.SelectedIndex >= 0 AndAlso _
                   Not String.Equals(ControIDSel, myComboBoxCell.SelectedValue.ToString()) Then

                    Dim CurrentRow As DataGridViewRow = UsedControlsGridView.CurrentRow

                    'Validate control do not exist.
                    If Not ControlExist(CInt(myComboBoxCell.SelectedValue)) Then
                        'Get Selected Control information.
                        myControlDS = GetQCControlInfo(CInt(myComboBoxCell.SelectedValue))

                        If myControlDS.tparControls.Count > 0 Then
                            UsedControlsGridView.BeginEdit(False)

                            CurrentRow.Cells("ControlName").Value = myComboBoxCell.SelectedValue

                            'Set the values for lot number and Exp. Date
                            CurrentRow.Cells("LotNumber").Value = myControlDS.tparControls(0).LotNumber
                            CurrentRow.Cells("ExpDate").Value = myControlDS.tparControls(0).ExpirationDate
                            CurrentRow.Cells("TestID").Value = SelectedISETestID 'set the test id

                            'Set the sample type
                            CurrentRow.Cells("SampleType").Value = bsSampleTypeComboBox.SelectedValue

                            'Set the test type
                            CurrentRow.Cells("TestType").Value = "ISE"

                            'Clear calculated values 
                            CurrentRow.Cells("MinConcentration").Value = DBNull.Value
                            CurrentRow.Cells("MaxConcentration").Value = DBNull.Value
                            CurrentRow.Cells("TargetMean").Value = DBNull.Value
                            CurrentRow.Cells("TargetSD").Value = DBNull.Value

                            UsedControlsGridView.EndEdit()

                            ValidationError = False
                            ChangesMade = True
                        End If
                    Else
                        CurrentRow.Cells("ControlName").Value = DBNull.Value
                        'Clear the values for lot number and Exp. Date
                        CurrentRow.Cells("LotNumber").Value = DBNull.Value
                        CurrentRow.Cells("ExpDate").Value = DBNull.Value
                        CurrentRow.Cells("SampleType").Value = DBNull.Value
                        CurrentRow.Cells("ControlID").Value = DBNull.Value
                        'Clear calculated values 
                        CurrentRow.Cells("MinConcentration").Value = DBNull.Value
                        CurrentRow.Cells("MaxConcentration").Value = DBNull.Value
                        CurrentRow.Cells("TargetMean").Value = DBNull.Value
                        CurrentRow.Cells("TargetSD").Value = DBNull.Value
                        'Show message indicating that control exist and set the cell value to null
                        CurrentRow.Cells("ControlName").ErrorText = GetMessageText(GlobalEnumerates.Messages.CONTROL_AREADYSEL.ToString())
                        ValidationError = True

                        UsedControlsGridView.CancelEdit()

                        UsedControlsGridView.CurrentRow.Selected = True
                    End If
                End If
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " editingGridComboBox_SelectionChangeCommitted ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub UsedControlsGridView_CellPainting(ByVal sender As Object, ByVal e As DataGridViewCellPaintingEventArgs) _
                    Handles UsedControlsGridView.CellPainting
        Try
            Dim ActiveControlColIndex As Integer = UsedControlsGridView.Columns("ActiveControl").Index
            Dim MinConColIndex As Integer = UsedControlsGridView.Columns("MinConcentration").Index
            Dim MaxConColIndex As Integer = UsedControlsGridView.Columns("MaxConcentration").Index
            Dim ControlNameColIndex As Integer = UsedControlsGridView.Columns("ControlName").Index
            Dim LotNumberColIndex As Integer = UsedControlsGridView.Columns("LotNumber").Index
            Dim ExpDateColIndex As Integer = UsedControlsGridView.Columns("ExpDate").Index
            Dim TargetMeanColIndex As Integer = UsedControlsGridView.Columns("TargetMean").Index
            Dim TargetSDColIndex As Integer = UsedControlsGridView.Columns("TargetSD").Index

            If e.RowIndex >= 0 AndAlso (e.ColumnIndex = LotNumberColIndex OrElse e.ColumnIndex = ExpDateColIndex OrElse _
                               e.ColumnIndex = TargetMeanColIndex OrElse e.ColumnIndex = TargetSDColIndex) Then

                e.CellStyle.BackColor = SystemColors.MenuBar
                e.CellStyle.ForeColor = Color.DarkGray

            ElseIf e.RowIndex >= 0 AndAlso (e.ColumnIndex = ActiveControlColIndex OrElse e.ColumnIndex = ControlNameColIndex OrElse _
                                   e.ColumnIndex = MinConColIndex OrElse e.ColumnIndex = MaxConColIndex) Then

                'DL 19/07/2011. Begin
                If e.ColumnIndex = ControlNameColIndex Then
                    Dim dgvCbo As DataGridViewComboBoxCell = TryCast(UsedControlsGridView(e.ColumnIndex, e.RowIndex), DataGridViewComboBoxCell)

                    If EditionMode AndAlso QCActiveCheckBox.Checked Then
                        dgvCbo.DisplayStyle = DataGridViewComboBoxDisplayStyle.DropDownButton
                        dgvCbo.ReadOnly = False
                        dgvCbo.Style.BackColor = Color.White
                        dgvCbo.Style.ForeColor = Color.Black
                    Else
                        dgvCbo.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
                        dgvCbo.ReadOnly = True
                        dgvCbo.Style.BackColor = SystemColors.MenuBar
                        dgvCbo.Style.ForeColor = Color.DarkGray
                    End If

                    'DL 19/07/2011. End
                Else
                    If EditionMode AndAlso QCActiveCheckBox.Checked Then
                        e.CellStyle.BackColor = Color.White
                        e.CellStyle.ForeColor = Color.Black
                        e.CellStyle.SelectionBackColor = Color.LightSlateGray
                    Else
                        e.CellStyle.BackColor = SystemColors.MenuBar
                        e.CellStyle.ForeColor = Color.DarkGray
                    End If
                End If

            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " UsedControlsGridView_CellPainting ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub UsedControlsGridView_EditingControlShowing(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewEditingControlShowingEventArgs) Handles UsedControlsGridView.EditingControlShowing
        Try
            Dim CurrentRow As Integer = UsedControlsGridView.CurrentCell.RowIndex
            Dim CurrentCol As Integer = UsedControlsGridView.CurrentCell.ColumnIndex
            Dim ControlNameColIndex As Integer = UsedControlsGridView.Columns("ControlName").Index
            Dim MinConColIndex As Integer = UsedControlsGridView.Columns("MinConcentration").Index
            Dim MaxConColIndex As Integer = UsedControlsGridView.Columns("MaxConcentration").Index

            If (CurrentCol = ControlNameColIndex) Then
                Dim editingComboBox As ComboBox = CType(e.Control, ComboBox)

                If (Not editingComboBox Is Nothing) Then
                    editingComboBox.DropDownStyle = ComboBoxStyle.DropDownList

                    RemoveHandler editingComboBox.SelectionChangeCommitted, AddressOf editingGridComboBox_SelectionChangeCommitted
                    AddHandler editingComboBox.SelectionChangeCommitted, AddressOf editingGridComboBox_SelectionChangeCommitted

                    RemoveHandler editingComboBox.KeyDown, AddressOf UsedControlsGridView_KeyDown
                    AddHandler editingComboBox.KeyDown, AddressOf UsedControlsGridView_KeyDown

                    'Disable the mouse wheel 
                    'RemoveHandler editingComboBox.MouseWheel, AddressOf EditingGridComboBox_MouseWeel
                    'AddHandler editingComboBox.MouseWheel, AddressOf EditingGridComboBox_MouseWeel
                End If
            End If

            'Validate columns 4 and 5
            If CurrentRow >= 0 AndAlso (CurrentCol = MinConColIndex OrElse CurrentCol = MaxConColIndex) Then
                'Dont allow to show copy/paste/Cut menu on cell
                'If e.Control.GetType().Name = "DataGridViewTextBoxEditingControl" Then
                If TypeOf (e.Control) Is DataGridViewTextBoxEditingControl Then
                    DirectCast(e.Control, DataGridViewTextBoxEditingControl).ShortcutsEnabled = False
                End If

                RemoveHandler e.Control.KeyPress, AddressOf CheckCell
                AddHandler e.Control.KeyPress, AddressOf CheckCell

            Else
                RemoveHandler e.Control.KeyPress, AddressOf CheckCell

            End If

        Catch ex As Exception
            'Write error SYSTEM_ERROR in the Application Log & show error message
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " UsedControlsGridView_EditingControlShowing ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Validate if the value enter is a numeric.
    ''' Dont allow string values only numeric values. 
    ''' and validate the decimal separator.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>Created by:  TR 05/05/2011</remarks>
    Private Sub CheckCell(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyPressEventArgs)
        Try
            Dim myDecimalSeparator As String = SystemInfoManager.OSDecimalSeparator
            'Validate the back key press.
            If String.Equals(e.KeyChar, CChar("")) OrElse String.Equals(e.KeyChar, ChrW(Keys.Back)) Then
                e.Handled = False
            Else
                'Add the Backspace key value (-Add the point)
                If String.Equals(e.KeyChar, myDecimalSeparator) OrElse String.Equals(e.KeyChar, CChar(".")) OrElse String.Equals(e.KeyChar, CChar(",")) Then

                    If String.Equals(e.KeyChar, CChar(".")) OrElse String.Equals(e.KeyChar, CChar(",")) Then e.KeyChar = CChar(myDecimalSeparator)

                    e.Handled = If(CType(sender, TextBox).Text.Contains(",") OrElse CType(sender, TextBox).Text.Contains("."), True, False)

                Else
                    If Not IsNumeric(e.KeyChar) Then e.Handled = True
                End If
            End If

            'Set the change made if is in edition mode
            If EditionMode AndAlso Not ChangesMade AndAlso Not e.KeyChar = ChrW(Keys.Tab) AndAlso Not e.KeyChar = ChrW(Keys.Back) Then
                ChangesMade = True
            End If

        Catch ex As Exception
            'Write error SYSTEM_ERROR in the Application Log & show error message
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " CheckCell ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try

    End Sub

    Private Sub UsedControlsGridView_CellLeave(ByVal sender As Object, ByVal e As DataGridViewCellEventArgs) Handles UsedControlsGridView.CellEndEdit
        Try
            If EditionMode Then
                Dim MinConColIndex As Integer = UsedControlsGridView.Columns("MinConcentration").Index
                Dim MaxConColIndex As Integer = UsedControlsGridView.Columns("MaxConcentration").Index
                Dim ControlNameValue As Object = UsedControlsGridView.Rows(e.RowIndex).Cells("ControlName").Value

                If Not ControlNameValue Is DBNull.Value AndAlso Not String.IsNullOrEmpty(Convert.ToString(ControlNameValue)) Then

                    If e.ColumnIndex = MinConColIndex OrElse e.ColumnIndex = MaxConColIndex Then
                        SetLimitValues(e.RowIndex, e.ColumnIndex)
                        'Calculate the values for Target (Mean/SD) if (Mean min or max change) SD (min or max or KSD change)
                        CalculateTargetValues(e.RowIndex, e.ColumnIndex)
                    End If
                End If
            End If

        Catch ex As Exception
            'Write error SYSTEM_ERROR in the Application Log & show error message
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " UsedControlsGridView_CellLeave ", _
                                            EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub UsedControlsGridView_RowValidating(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellCancelEventArgs) Handles UsedControlsGridView.RowValidating
        Try
            If EditionMode Then
                If ValidateErrorOnQCUsedControls() Then
                    'UsedControlsGridView.CancelEdit()

                    'TR 24/07/2013 BUG #987 Remove the e.cancel to avoid cell blocking.
                    'e.Cancel = True 'TR 19/04/2012
                End If
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " UsedControlsGridView_RowValidating ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try

    End Sub

    'RH 04/07/2012
    Private Sub UsedControlsGridView_CellContentClick(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles UsedControlsGridView.CellContentClick
        Try
            Dim ActiveControlColIndex As Integer = UsedControlsGridView.Columns("ActiveControl").Index
            Dim Cell As DataGridViewCell = UsedControlsGridView(e.ColumnIndex, e.RowIndex)

            If e.ColumnIndex = ActiveControlColIndex AndAlso EditionMode Then
                If CBool(Cell.EditedFormattedValue) AndAlso CountActiveControl() > 2 Then
                    Cell.ReadOnly = True 'Update the cell with the Edited Value (True)
                    Cell.ReadOnly = False
                    Cell.Value = False 'Uncheck the cell
                    Cell.ReadOnly = True 'Update the Edited Value to False. Actually uncheck the cell.
                    Cell.ReadOnly = False 'Update ReadOnly to False, so the cell remains editable.
                End If
            End If

        Catch ex As Exception
            'Write error SYSTEM_ERROR in the Application Log & show error message
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " UsedControlsGridView_CellContentClick ", _
                                            EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Format the cell values for Target values, Min And Max.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>
    ''' Modified by: WE 25/08/2014 - #1865
    ''' </remarks>
    Private Sub UsedControlsGridView_CellFormatting(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellFormattingEventArgs) Handles UsedControlsGridView.CellFormatting
        Try
            If e.ColumnIndex = 4 OrElse e.ColumnIndex = 5 OrElse e.ColumnIndex = 6 Then
                If Not e.Value Is DBNull.Value AndAlso Not e.Value Is Nothing Then
                    'e.Value = CSng(e.Value).ToStringWithDecimals(GetDecimals())
                    e.Value = CSng(e.Value).ToString("F" & bsDecimalsUpDown.Value)
                End If
            ElseIf e.ColumnIndex = 7 Then
                If Not e.Value Is DBNull.Value AndAlso Not e.Value Is Nothing Then
                    'e.Value = CSng(e.Value).ToStringWithDecimals(GetDecimals() + 1)
                    e.Value = CSng(e.Value).ToString("F" & (CInt(bsDecimalsUpDown.Value) + 1))
                End If
            End If

        Catch ex As Exception
            'Write error SYSTEM_ERROR in the Application Log & show error message
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " UsedControlsGridView_CellFormatting ", _
                                            EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try

        ' Example code from IProgTest
        'Try
        '    If e.ColumnIndex = 4 OrElse e.ColumnIndex = 5 OrElse e.ColumnIndex = 6 Then
        '        If Not e.Value Is DBNull.Value AndAlso Not e.Value Is Nothing Then
        '            e.Value = DirectCast(e.Value, Single).ToString("F" & DecimalsUpDown.Value)
        '        End If
        '    ElseIf e.ColumnIndex = 7 Then
        '        If Not e.Value Is DBNull.Value AndAlso Not e.Value Is Nothing Then
        '            'TR 10/05/2011  -Show the decimals allowed + 1 
        '            e.Value = DirectCast(e.Value, Single).ToString("F" & (CInt(DecimalsUpDown.Value) + 1))
        '        End If
        '    End If
        'Catch ex As Exception
        '    'Write error SYSTEM_ERROR in the Application Log & show error message
        '    CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " UsedControlsGridView_CellFormatting ", _
        '                                    EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
        '    ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        'End Try

    End Sub

    Private Sub QCActiveCheckBox_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles QCActiveCheckBox.CheckedChanged
        Try
            If EditionMode Then
                ActivateQCControlsByQCActive(QCActiveCheckBox.Checked)
                ChangesMade = True

                UsedControlsGridView.AllowUserToAddRows = QCActiveCheckBox.Checked
            Else
                ActivateQCControlsByQCActive(False)
                UsedControlsGridView.AllowUserToAddRows = False
            End If

            'Set Default Values for QC Area.
            If QCActiveCheckBox.Checked Then
                'Validate if has value else assigned
                If QCReplicNumberNumeric.Text = "" Then
                    QCReplicNumberNumeric.Text = "1"
                End If

                'TR 20/01/2012 -Validate if the statistic RB is checked 
                If Not StaticRadioButton.Checked Then
                    ManualRadioButton.Checked = True
                    QCMinNumSeries.Enabled = False
                End If
                'TR 20/01/2012 -END.

                If QCRejectionCriteria.Text = "" Then
                    QCRejectionCriteria.Text = "3"
                End If
            End If
            'TR 12/04/2011 -END

        Catch ex As Exception
            'Write error SYSTEM_ERROR in the Application Log & show error message
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " QCActiveCheckBox_CheckedChanged ", _
                                            EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)

            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try

    End Sub

    Private Sub ManualRadioButton_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ManualRadioButton.CheckedChanged
        Try
            If ManualRadioButton.Checked Then
                QCMinNumSeries.ResetText()
                QCMinNumSeries.Enabled = False
            Else
                QCMinNumSeries.Enabled = True
                QCMinNumSeries.Text = "10"
            End If

            If EditionMode Then
                ChangesMade = True
            End If

        Catch ex As Exception
            'Write error SYSTEM_ERROR in the Application Log & show error message
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " ManualRadioButton_CheckedChanged ", _
                                            EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try

    End Sub

    'Private Sub StaticRadioButton_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles StaticRadioButton.CheckedChanged
    '    Try
    '        QCMinNumSeries.Enabled = StaticRadioButton.Checked

    '        If EditionMode Then
    '            ChangesMade = True

    '            'Set default value.
    '            QCMinNumSeries.Value = QCMinNumSeries.Minimum
    '            'Set default value.
    '            'QCMinNumSeries.Text = "10"
    '        End If

    '    Catch ex As Exception
    '        'Write error SYSTEM_ERROR in the Application Log & show error message
    '        CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " StaticRadioButton_CheckedChanged ", _
    '                                        EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
    '        ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
    '    End Try
    'End Sub

    's12CheckBox.CheckedChanged
    Private Sub RulesToApplyCheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _
                    x22CheckBox.CheckedChanged, x10CheckBox.CheckedChanged, _
                    s41CheckBox.CheckedChanged, s13CheckBox.CheckedChanged, _
                    r4sCheckBox.CheckedChanged

        If EditionMode Then
            ChangesMade = True
        End If

    End Sub

    Private Sub QCRejectionCriteria_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles QCRejectionCriteria.ValueChanged
        Try
            If EditionMode Then
                ChangesMade = True
                'Recalculate targets
                RecalculateAllTarget()
            End If

        Catch ex As Exception
            'Write error SYSTEM_ERROR in the Application Log & show error message
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " QCRejectionCriteria_ValueChanged ", _
                                            EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub QCReplicNumberNumeric_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles QCReplicNumberNumeric.Validating
        Try

            'myTextBox = CType(sender, TextBox)

            'If (myTextBox.TextLength > 0) Then
            '    If (bsScreenErrorProvider.GetError(myTextBox) <> "") Then
            '        bsScreenErrorProvider.SetError(myTextBox, String.Empty)
            '    End If
            'End If

            ' WE 03/09/2014 - #1865
            If BsErrorProvider1.GetError(CType(sender, BSNumericUpDown)) = String.Empty Then
                If QCReplicNumberNumeric.Text = "" Then
                    BsErrorProvider1.SetError(QCReplicNumberNumeric, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString))
                End If
            Else
                If QCReplicNumberNumeric.Text <> "" Then
                    BsErrorProvider1.SetError(QCReplicNumberNumeric, String.Empty)
                End If
            End If
            ' WE 03/09/2014 - #1865 - End

            'BsErrorProvider1.Clear()
            'If QCReplicNumberNumeric.Text = "" Then
            '    BsErrorProvider1.SetError(QCReplicNumberNumeric, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString))
            '    'QCReplicNumberNumeric.Value = QCReplicNumberNumeric.Minimum
            '    'QCReplicNumberNumeric.Focus()
            'End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", " QCReplicNumberNumeric_Validating " & Name, EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    Private Sub QCRejectionCriteria_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles QCRejectionCriteria.Validating
        Try
            ' WE 03/09/2014 - #1865
            If BsErrorProvider1.GetError(CType(sender, BSNumericUpDown)) = String.Empty Then
                If QCRejectionCriteria.Text = "" Then
                    BsErrorProvider1.SetError(QCRejectionCriteria, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString))
                End If
            Else
                If QCRejectionCriteria.Text <> "" Then
                    BsErrorProvider1.SetError(QCRejectionCriteria, String.Empty)
                End If
            End If
            ' WE 03/09/2014 - #1865 - End

            'BsErrorProvider1.Clear()
            'If QCRejectionCriteria.Text = "" Then
            '    BsErrorProvider1.SetError(QCRejectionCriteria, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString))
            '    QCRejectionCriteria.Value = QCRejectionCriteria.Minimum
            '    QCRejectionCriteria.Focus()
            'End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", " QCRejectionCriteria_Validating " & Name, EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    Private Sub QCMinNumSeries_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles QCMinNumSeries.Validating
        Try
            ' WE 03/09/2014 - #1865
            If BsErrorProvider1.GetError(CType(sender, BSNumericUpDown)) = String.Empty Then
                If StaticRadioButton.Checked AndAlso QCMinNumSeries.Text = "" Then
                    BsErrorProvider1.SetError(QCMinNumSeries, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString))
                End If
            Else
                If StaticRadioButton.Checked AndAlso QCMinNumSeries.Text <> "" Then
                    BsErrorProvider1.SetError(QCMinNumSeries, String.Empty)
                End If
            End If
            ' WE 03/09/2014 - #1865 - End

            'BsErrorProvider1.Clear()
            'If StaticRadioButton.Checked AndAlso QCMinNumSeries.Text = "" Then
            '    BsErrorProvider1.SetError(QCMinNumSeries, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString))
            '    QCMinNumSeries.Value = QCMinNumSeries.Minimum
            '    QCMinNumSeries.Focus()
            'End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", " QCMinNumSeries_Validating " & Name, EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try

    End Sub

    Private Sub QCActiveCheckBox_EnabledChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles QCActiveCheckBox.EnabledChanged
        Try
            If QCActiveCheckBox.Enabled Then
                ActivateQCControlsByQCActive(QCActiveCheckBox.Checked)
                UsedControlsGridView.AllowUserToAddRows = QCActiveCheckBox.Checked
            Else
                ActivateQCControlsByQCActive(False)
                UsedControlsGridView.AllowUserToAddRows = QCActiveCheckBox.Enabled
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", " QCActiveCheckBox_EnabledChanged " & Name, EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    Private Sub DeleteControlButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DeleteControlButton.Click
        DeleteSelectedControl()
    End Sub

    'Private Sub StaticRadioButton_EnabledChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles StaticRadioButton.EnabledChanged
    '    Try
    '        If StaticRadioButton.Enabled AndAlso StaticRadioButton.Checked Then
    '            QCMinNumSeries.Enabled = StaticRadioButton.Checked
    '        Else
    '            QCMinNumSeries.Enabled = StaticRadioButton.Checked
    '        End If

    '    Catch ex As Exception
    '        CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", " StaticRadioButton_EnabledChanged " & Name, EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
    '        ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
    '    End Try
    'End Sub

    Private Sub AddControls_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AddControls.Click
        Try
            'Open controls Programming
            Using myProgControls As New IProgControls
                myProgControls.AnalyzerID = AnalyzerIDAttribute
                myProgControls.WorkSessionID = WorkSessionIDAttribute
                myProgControls.SourceScreen = GlbSourceScreen.TEST_QCTAB.ToString
                'myProgControls.FormBorderStyle = Windows.Forms.FormBorderStyle.FixedToolWindow

                'RH 07/06/2012 This is the right value
                'It is the used througout the application.
                'It enables the aplication to show it's icon when the user presses Alt + Tab. The other one not.
                myProgControls.FormBorderStyle = Windows.Forms.FormBorderStyle.FixedDialog

                myProgControls.Tag = "Put something here before showing, so the form will execute its normal Close()"
                myProgControls.ShowDialog()
            End Using

            'Reload the control list to get the new data.
            ControlListDS = GetAllQCControls()

            'Asigned the Datasource to the grid column to reload the data.
            DirectCast(UsedControlsGridView.Columns("ControlName"), DataGridViewComboBoxColumn).DataSource = ControlListDS.tparControls

        Catch ex As Exception
            'Write error SYSTEM_ERROR in the Application Log & show error message
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " AddControls_Click ", EventLogEntryType.Error, _
                                                        GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try

    End Sub

    Private Sub UsedControlsGridView_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles UsedControlsGridView.KeyDown
        Try
            'If sender.GetType.Name = "DataGridViewComboBoxEditingControl" Then
            If TypeOf (sender) Is DataGridViewComboBoxEditingControl Then
                '(BUG) implent this to avoid that selected control change to the first control on the list
                UsedControlsGridView.CancelEdit()
            End If

            If e.KeyCode = Keys.Delete Then
                DeleteSelectedControl()
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " UsedControlsGridView_KeyDown ", EventLogEntryType.Error, _
                                                       GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try

    End Sub

    'Private Sub UsedControlsGridView_CellValidating(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellValidatingEventArgs) Handles UsedControlsGridView.CellValidating
    '    Try
    '        If EditionMode Then
    '            If e.ColumnIndex = 4 OrElse e.ColumnIndex = 5 Then
    '                If Not e.FormattedValue Is DBNull.Value AndAlso Not String.IsNullOrEmpty(Convert.ToString(e.FormattedValue)) AndAlso _
    '                        Not IsNumeric(e.FormattedValue) Then
    '                    e.Cancel = True
    '                End If

    '            ElseIf e.ColumnIndex = 1 Then
    '                If Not e.FormattedValue Is DBNull.Value AndAlso Not String.IsNullOrEmpty(Convert.ToString(e.FormattedValue)) Then
    '                    UsedControlsGridView.CancelEdit()
    '                End If
    '            End If

    '        End If

    '    Catch ex As Exception
    '        'Write error SYSTEM_ERROR in the Application Log & show error message
    '        CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & " UsedControlsGridView_CellValidating ", EventLogEntryType.Error, _
    '                                                    GetApplicationInfoSession().ActivateSystemLog)
    '        ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
    '    End Try
    'End Sub




    ''' <summary>
    ''' Handler for NumericUpDown controls allowing numbers with decimals.
    ''' Only numbers and the decimal separator are allowed.
    ''' </summary>
    ''' <remarks>
    '''    Created by: WE 01/08/2014 - #1865. Same as used in IProgTest.
    '''    Modified:   SA 09/11/2012 - Implementation changed
    '''                TR 09/10/2013 - Bug #1315 allow too introduce  (-)minus on SlopeFactor A  B. 
    '''                                to allow negative values introduce by key press.
    ''' </remarks>
    Private Sub RealNumericUpDown_KeyPress(sender As Object, e As KeyPressEventArgs) Handles bsSlopeA2UpDown.KeyPress, bsSlopeB2UpDown.KeyPress

        Try
            If (e.KeyChar = CChar("") OrElse e.KeyChar = ChrW(Keys.Back)) Then
                e.Handled = False
            Else
                Dim myDecimalSeparator As String = SystemInfoManager.OSDecimalSeparator
                If (e.KeyChar = CChar(".") OrElse e.KeyChar = CChar(",")) Then
                    e.KeyChar = CChar(myDecimalSeparator)

                    If (CType(sender, BSNumericUpDown).Text.Contains(".") Or CType(sender, BSNumericUpDown).Text.Contains(",")) Then
                        e.Handled = True
                    Else
                        e.Handled = False
                    End If
                Else
                    'TR 09/10/2013 -BUG #1315 -Validate if slope controls to allow the -
                    If (CType(sender, BSNumericUpDown).Name = "bsSlopeA2UpDown" OrElse CType(sender, BSNumericUpDown).Name = "bsSlopeB2UpDown") Then
                        'Validate if it's a numeric value and not the - character used to indicate negative values.
                        If (Not IsNumeric(e.KeyChar) AndAlso Not e.KeyChar = "-") Then
                            e.Handled = True
                        ElseIf e.KeyChar = "-" Then
                            'Allow only one -Simbol on control
                            If CType(sender, BSNumericUpDown).Text.Contains("-") Then
                                e.Handled = True
                            End If
                        End If
                        'TR 09/10/2013 -BUG #1315 -END.
                    Else
                        If (Not IsNumeric(e.KeyChar)) Then
                            e.Handled = True
                        End If
                    End If
                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".RealNumericUpDown_KeyPress", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".RealNumericUpDown_KeyPress", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' To avoid entered following characters in the Numeric Up/Down allowing only integer numbers greater than zero:
    '''   ** Minus sign
    '''   ** Dot, Apostrophe or Comma as decimal point
    ''' </summary>
    ''' <remarks>
    ''' Created by:  WE 25/08/2014 - #1865. Same as used in IProgTest.
    ''' Modified by: SA 09/11/2012
    ''' </remarks>
    Private Sub IntegerNumericUpDown_KeyPress(sender As Object, e As KeyPressEventArgs) Handles bsDecimalsUpDown.KeyPress

        Try
            If (e.KeyChar = CChar("-") OrElse e.KeyChar = CChar(".") OrElse e.KeyChar = CChar(",") OrElse e.KeyChar = "'") Then e.Handled = True
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".IntegerNumericUpDown_KeyPress", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".IntegerNumericUpDown_KeyPress", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Generic event handler to capture the NumericUpDown content deletion
    ''' </summary>
    ''' <remarks>
    '''     Created by:  WE 01/08/2014 - #1865. Same as used in IProgTest.
    '''     Modified by: SG 18/06/2010
    ''' </remarks>
    Private Sub NumericUpDown_KeyUp(sender As Object, e As KeyEventArgs) Handles bsSlopeA2UpDown.KeyUp, bsSlopeB2UpDown.KeyUp

        Try
            Dim miNumericUpDown As NumericUpDown = CType(sender, NumericUpDown)

            If miNumericUpDown.Text <> "" Then

            Else
                miNumericUpDown.Value = miNumericUpDown.Minimum
                miNumericUpDown.ResetText()
            End If

            If EditionMode Then
                ChangesMade = True
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "NumericUpDown_KeyUp " & Name, EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".NumericUpDown_KeyUp", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>
    ''' Created by:  WE 25/08/2014 - #1865. Based on same concept as used in IProgTest.
    ''' Modified by: AG 09/07/2010
    '''              SA 09/11/2012 - Changes due to an error was raised when trying to convert to INT a "big" number that 
    '''                              was entered in the numeric UpDown
    ''' </remarks>
    Private Sub bsDecimalsUpDown_Validated(sender As Object, e As EventArgs) Handles bsDecimalsUpDown.Validated

        Try
            Dim decimalsNumber As Integer = 0
            Dim myDecimalNum As String = bsDecimalsUpDown.Text.ToString

            If (myDecimalNum = String.Empty) Then
                decimalsNumber = CInt(bsDecimalsUpDown.Minimum)
            Else
                If (myDecimalNum.Length = bsDecimalsUpDown.Maximum.ToString.Length) Then
                    decimalsNumber = CInt(bsDecimalsUpDown.Text)
                    decimalsNumber = CInt(IIf(decimalsNumber < bsDecimalsUpDown.Minimum, bsDecimalsUpDown.Minimum, decimalsNumber))
                    decimalsNumber = CInt(IIf(decimalsNumber > bsDecimalsUpDown.Maximum, bsDecimalsUpDown.Maximum, decimalsNumber))
                Else
                    decimalsNumber = CInt(bsDecimalsUpDown.Maximum)
                End If
            End If

            bsDecimalsUpDown.Text = decimalsNumber.ToString
            SetupDecimalsNumber(decimalsNumber)

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "bsDecimalsUpDown_Validated " & Name, EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsDecimalsUpDown_Validated", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try

    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>
    ''' Created by:  WE 26/08/2014 - #1865. Based on same concept as used in IProgTest. Extended with 3rd check as used by form-level validation and
    '''                              included control activation in first 2 cases. This event handler method may only be used in conjunction with field
    '''                              SlopeFactorA2 to avoid situation in which user will never be able to set both fields (SlopeFactorA2 and SlopeFactorB2)
    '''                              to empty value again.
    ''' </remarks>
    Private Sub SlopeUpDown_Validating(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles bsSlopeA2UpDown.Validating

        Try
            BsErrorProvider1.Clear()
            ValidationError = False
            If Not bsSlopeA2UpDown.Text = "" AndAlso bsSlopeA2UpDown.Value = 0 Then
                BsErrorProvider1.SetError(bsSlopeA2UpDown, GetMessageText(GlobalEnumerates.Messages.ZERO_NOTALLOW.ToString)) 'AG 07/07/2010("ZERO_NOTALLOW"))
                ValidationError = True
                bsSlopeA2UpDown.Select()
            ElseIf Not bsSlopeA2UpDown.Text = "" AndAlso bsSlopeB2UpDown.Text = "" Then
                BsErrorProvider1.SetError(bsSlopeB2UpDown, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString)) 'AG 07/07/2010("REQUIRED_VALUE"))
                ValidationError = True
                bsSlopeB2UpDown.Select()
            ElseIf Not bsSlopeB2UpDown.Text = "" AndAlso bsSlopeA2UpDown.Text = "" Then
                BsErrorProvider1.SetError(bsSlopeA2UpDown, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString))
                ValidationError = True
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "SlopeUpDown_Validating " & Name, EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".SlopeUpDown_Validating", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Activate the ChangesMade variable whenever the value of the corresponding control is changed.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>WE 27/08/2014 - #1865. Generic event handler method that triggers ChangesMade whenever the value of a control is changed.
    '''                          Only used for new fields for which ChangesMade can not be handled properly by PendingChangesVerification.</remarks>
    Private Sub ControlValueChanged(sender As Object, e As EventArgs) Handles bsReportNameTextBox.TextChanged,
                                                                              bsDecimalsUpDown.ValueChanged,
                                                                              bsSlopeB2UpDown.ValueChanged,
                                                                              bsSlopeA2UpDown.ValueChanged,
                                                                              QCReplicNumberNumeric.ValueChanged,
                                                                              QCMinNumSeries.ValueChanged
        If EditionMode Then
            ChangesMade = True
        End If
    End Sub


#End Region

#Region "Events"
    '*************************'
    '* EVENTS FOR THE SCREEN *
    '*************************'
    ''' <summary>
    ''' When the screen is in EDIT mode and ESC key is pressed, code of Cancel Button is executed;
    ''' in other case, the screen is closed
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 09/11/2010
    ''' </remarks>
    Private Sub ProgISETest_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyDown
        Try
            If (e.KeyCode = Keys.Escape) Then
                If (bsCancelButton.Enabled) Then
                    'CancelISETestEdition()

                    'RH 04/07/2011 Escape key should do exactly the same operations as bsCancelButton_Click()
                    bsCancelButton.PerformClick()
                Else
                    'ExitScreen()

                    'RH 04/07/2011 Escape key should do exactly the same operations as bsExitButton_Click()
                    bsExitButton.PerformClick()
                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ProgISETest_KeyDown ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ProgISETest_KeyDown", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Form initialization when loading: set the screen status to INITIAL MODE
    ''' </summary>
    ''' <remarks>
    ''' Modified by: SG 20/10/2010
    ''' </remarks>
    Private Sub ProgISETest_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            'TR 20/04/2012 get the current user level
            Dim MyGlobalBase As New GlobalBase
            CurrentUserLevel = GlobalBase.GetSessionInfo().UserLevel
            'TR 20/04/2012 -END.
            ScreenLoad()
            ScreenStatusByUserLevel() 'TR 23/04/2012
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ISETestForm_Load", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ISETestForm_Load", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    '************************************'
    '* EVENTS FOR LISTVIEW OF ISE TESTS *'
    '************************************'
    ''' <summary>
    ''' Show data of the selected ISE Test in READ-ONLY MODE
    ''' </summary>
    ''' <remarks>
    ''' Modified by:  SG 20/10/2010
    ''' </remarks>
    Private Sub bsISETestListView_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles bsISETestListView.Click
        Try
            If (bsISETestListView.SelectedItems.Count = 1) Then
                QueryISETest(False)
            Else
                InitialModeScreenStatus(False)

                bsEditButton.Enabled = False
                bsSampleTypeComboBox.Enabled = False
                bsSampleTypeComboBox.BackColor = SystemColors.MenuBar
                bsSampleTypePlusPictureBox.Visible = False
            End If
            ScreenStatusByUserLevel() 'TR 23/04/2012

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsISETestListView_Click", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsISETestListView_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Sort ascending/descending the list of ISE Tests when clicking in the list header
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SG 20/10/2010
    ''' </remarks>
    Private Sub bsISETestListView_ColumnClick(ByVal sender As Object, ByVal e As System.Windows.Forms.ColumnClickEventArgs) Handles bsISETestListView.ColumnClick
        Try
            Select Case bsISETestListView.Sorting
                Case SortOrder.None
                    bsISETestListView.Sorting = SortOrder.Ascending
                    Exit Select
                Case SortOrder.Ascending
                    bsISETestListView.Sorting = SortOrder.Descending
                    Exit Select
                Case SortOrder.Descending
                    bsISETestListView.Sorting = SortOrder.Ascending
                    Exit Select
                Case Else
                    Exit Select
            End Select
            bsISETestListView.Sort()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsISETestListView_ColumnClick", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsISETestListView_ColumnClick", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Don't allow Users to resize the visible ListView column
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SG 20/10/2010
    ''' </remarks>
    Private Sub bsISETestListView_ColumnWidthChanging(ByVal sender As Object, ByVal e As System.Windows.Forms.ColumnWidthChangingEventArgs) Handles bsISETestListView.ColumnWidthChanging
        Try
            e.Cancel = True
            'If e.ColumnIndex = 0 Then
            '    e.NewWidth = 210
            'Else
            '    e.NewWidth = 0
            'End If

            e.NewWidth = bsISETestListView.Columns(e.ColumnIndex).Width

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsISETestListView_ColumnWidthChanging", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsISETestListView_ColumnWidthChanging", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Load data of the selected ISE Test and set the screen status to Edit Mode
    ''' </summary>
    ''' <remarks>
    ''' Created by: RH 08/06/2012
    ''' </remarks>
    Private Sub bsISETestListView_DoubleClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles bsISETestListView.DoubleClick
        EnableEdition()
    End Sub

    ''' <summary>
    ''' Allow consultation of elements in ISE Test list using the keyboard
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SG 20/10/2010
    ''' </remarks>
    Private Sub bsISETestListView_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles bsISETestListView.KeyUp
        Try
            If (bsISETestListView.SelectedItems.Count = 1) Then
                QueryISETestByMoveUpDown(e)
            Else
                InitialModeScreenStatus(False)
                bsEditButton.Enabled = False
                bsCustomOrderButton.Enabled = True 'AG 05/09/2014 - BA-1869
            End If

            ScreenStatusByUserLevel() 'TR 23/04/2012

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsISETestListView_KeyUp", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsISETestListView_KeyUp", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Allow edition of the selected ISE Test when Enter Key is pressed
    ''' </summary>
    Private Sub bsISETestListView_PreviewKeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.PreviewKeyDownEventArgs) Handles bsISETestListView.PreviewKeyDown
        Try
            If (bsISETestListView.SelectedItems.Count > 0) Then
                If (e.KeyCode = Keys.Enter) Then
                    If (bsISETestListView.SelectedItems.Count = 1 AndAlso bsEditButton.Enabled) Then
                        EnableEdition()
                    End If
                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsISETestListView_PreviewKeyDown ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsISETestListView_PreviewKeyDown", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    '**********************'
    '* EVENTS FOR BUTTONS *'
    '**********************'
    ''' <summary>
    ''' Set the screen status to EDIT MODE for the selected ISE Test
    ''' </summary>
    ''' <remarks>
    ''' Modified by: RH 08/06/2012
    ''' </remarks>
    Private Sub bsEditButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles bsEditButton.Click
        EnableEdition()
    End Sub

    ''' <summary>
    ''' Saves changes to the database
    ''' </summary>
    ''' <remarks>
    ''' Created by: RH 13/06/2012
    ''' </remarks>    
    Private Sub bsSaveButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsSaveButton.Click
        If SaveChanges(False) Then
            '  CancelISETestEdition()
        End If
    End Sub

    ''' <summary>
    ''' Execute the cancelling of a ISE Test edition
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SG 20/10/2010
    ''' </remarks>    
    Private Sub bsCancelButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsCancelButton.Click
        Try
            CancelISETestEdition()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsCancelButton_Click", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsCancelButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Close the screen, verifying first if there are changes pending to save
    ''' </summary>
    Private Sub bsExitButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsExitButton.Click
        Try
            ExitScreen()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsExitButton_Click", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsExitButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    '*************************************'
    '* EVENTS FOR FIELDS IN DETAILS AREA *'
    '*************************************'
    ''' <summary>
    ''' Manages the changing of the current SampleType for an ISE Tests using several; data of the current
    ''' SampleType is saved locally and data of the selected SampleType is loaded
    ''' </summary>
    ''' <remarks>
    ''' Modified by: SA 12/01/2011 - Changes due to new implementation of Reference Ranges Control 
    ''' </remarks>
    Private Sub bsSampleTypeComboBox_SelectionChangeCommitted(ByVal sender As Object, ByVal e As System.EventArgs) Handles bsSampleTypeComboBox.SelectionChangeCommitted
        Try
            Dim cancelChange As Boolean = False

            If (bsSampleTypeComboBox.SelectedIndex >= 0) Then
                If (bsSampleTypeComboBox.SelectedValue.ToString() <> SelectedSampleType) Then
                    If (EditionMode) Then
                        'If there are changes pending to save, save them before load the Reference Ranges for the new selected Sample Type
                        If (PendingChangesVerification()) Then
                            If (Not SaveChanges(True)) Then
                                cancelChange = True
                            Else
                                'Update value of ActiveRangeType in the local DS containing all SampleTypes linked to the ISE Test
                                Dim lstTestSamples As List(Of ISETestSamplesDS.tparISETestSamplesRow)
                                lstTestSamples = (From a In SelectedISETestSamplesDS.tparISETestSamples _
                                                  Where a.ISETestID = SelectedISETestID _
                                                  AndAlso String.Equals(a.SampleType, SelectedSampleType) _
                                                  Select a).ToList()

                                If (lstTestSamples.Count = 1) Then
                                    lstTestSamples(0).BeginEdit()
                                    lstTestSamples(0).ActiveRangeType = bsTestRefRanges.ActiveRangeType
                                    lstTestSamples(0).EndEdit()
                                End If
                            End If

                        End If
                    End If

                    If (Not cancelChange) Then
                        ''Select the new SampleType and get and load the Reference Ranges defined for it
                        SelectedSampleType = bsSampleTypeComboBox.SelectedValue.ToString()
                        bsTestRefRanges.ChangesMade = False
                        bsTestRefRanges.ChangeSampleTypeDuringEdition = True
                        bsTestRefRanges.isEditing = False

                        'GetRefRanges(SelectedISETestID)
                        'LoadRefRangesData()

                        QueryISETest(True)

                        bsTestRefRanges.ChangeSampleTypeDuringEdition = False
                    Else
                        'Data changed for the previous SampleType could not be changed due to 
                        'error warnings; recovery the previous SampleType in the ComboBox
                        bsSampleTypeComboBox.SelectedValue = SelectedSampleType
                    End If
                End If
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsSampleTypeComboBox_SelectionChangeCommitted", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsSampleTypeComboBox_SelectionChangeCommitted", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Generic event to clean the Error Provider when the value in the TextBox showing the error is changed
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SG 20/10/2010
    ''' Modified by: SA SA 12/01/2011 - Changes due to new implementation of Reference Ranges Control 
    ''' </remarks>
    Private Sub bsTextbox_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles bsFullNameTextbox.TextChanged, _
                                                                                                   bsShortNameTextbox.TextChanged, ReportsNameTextBox.TextChanged
        ' WE 30/07/2014 - #1865 - Added ReportsNameTextBox.
        Try
            Dim myTextBox As New TextBox
            myTextBox = CType(sender, TextBox)

            If (myTextBox.TextLength > 0) Then
                If Not String.Equals(BsErrorProvider1.GetError(myTextBox), String.Empty) Then BsErrorProvider1.SetError(myTextBox, String.Empty)
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsTextbox_TextChanged", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsTextbox_TextChanged", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Manages the changing of the current Measure Unit for an ISE Test
    ''' </summary>
    Private Sub bsUnitComboBox_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles bsUnitComboBox.SelectedIndexChanged
        Try
            bsTestRefRanges.MeasureUnit = bsUnitComboBox.Text

            If EditionMode Then

            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsUnitComboBox_SelectedIndexChanged", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsUnitComboBox_SelectedIndexChanged", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 25/09/2012
    ''' </remarks>
    Private Sub UsedControlsGridView_DataError(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewDataErrorEventArgs) Handles UsedControlsGridView.DataError
        'Dim oDGVC As DataGridViewColumn = UsedControlsGridView.Columns(e.ColumnIndex)
        'Dim sTextoMensaje As String

        'sTextoMensaje = "Error en la columna: " & oDGVC.DataPropertyName & "" & e.Exception.Message

        'MessageBox.Show(sTextoMensaje, "Error de edición", MessageBoxButtons.OK)

        '// si después del mensaje quieres dejar la celda en su estado original
        '// realizas la siguiente asignación
        e.Cancel = False
    End Sub

    ''' <summary>
    ''' Open the customize order and availability for OFFS tests selection
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>AG 04/09/2014 - BA-1869</remarks>
    Private Sub BsCustomOrderButton_Click(sender As Object, e As EventArgs) Handles bsCustomOrderButton.Click
        Try
            'Shown the Positioning Warnings Screen
            Using AuxMe As New ISortingTestsAux()
                AuxMe.openMode = "TESTSELECTION"
                AuxMe.screenID = "ISE"
                AuxMe.ShowDialog()
            End Using

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".BsCustomOrderButton_Click", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".BsCustomOrderButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub
#End Region

End Class