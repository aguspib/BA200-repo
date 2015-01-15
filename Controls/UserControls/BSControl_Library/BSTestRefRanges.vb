Option Strict On
Option Explicit On

Imports System.Windows.Forms
Imports System.Drawing
Imports Biosystems.Ax00.Controls.UserControls
Imports Biosystems.Ax00.Types

'User Control that allows the edition of Reference Ranges for Tests of whatever type: 
'Standard, Calculated, ISE and/or Off-System Tests
'Created by: SG 01/09/2010
Public Class BSTestRefRanges

#Region "Constructor"
    'This call is required by the Windows Form Designer
    Public Sub New()
        InitializeComponent()
    End Sub
#End Region

#Region "Enumerates"
    Private Enum TestTypes
        STANDARD
        CALCULATED
        OFFSYSTEM
        ISE
    End Enum

    Private Enum RefPreloadedMasterDataEnum
        AGE_UNITS
        SEX_LIST
    End Enum

    Private Enum RefFieldLimitsEnum
        AGE_FROM_TO_DAYS
        AGE_FROM_TO_MONTHS
        AGE_FROM_TO_YEARS
        NORMAL_REF_RANGE
        REF_RANGE_LIMITS
    End Enum

    Private Enum RefMessages
        DB_CONNECTION_ERROR
        DETAIL_REF_OVERWRITE
        DETGREATHERLIN
        DISCARD_PENDING_CHANGES
        MINGREATHERMAX
        MIN_MUST_BE_LOWER_THAN_MAX
        NOT_NULL_VALUE
        REQUIRED_VALUE
        SAVE_CHANGES
        SAVE_PENDING
        SHOW_MESSAGE_TITLE_TEXT
        SYSTEM_ERROR
        VALUE_OUT_RANGE
        WRONG_DATATYPE
        WRONG_RANGE_LIMITS
        ZERO_NOTALLOW
        WRONG_AGE_LIMITS
    End Enum
#End Region

#Region "Attributes"
    Private EditionModeAttribute As Boolean = False
    Private ChangesMadeAttribute As Boolean
#End Region

#Region "Public Properties"

    'To set the current state of the User Control: read-only (when false) or edition (when true)
    Public WriteOnly Property isEditing() As Boolean
        Set(ByVal value As Boolean)
            EditionModeAttribute = value
            RefRadioButtonsManagement()

            If (Not EditionModeAttribute) Then
                DisableControls("ALL")
            Else
                EnableControls("ALL")
            End If
            'EnableControls(EditionModeAttribute)
            RefDetailCheckBoxesManagement()
            RefRadioButtonsManagement()

            If (Not value) Then RefDetailDataGridUnselect()
        End Set
    End Property

    'To set the Icon for Add Detailed Range Button
    'Created by: DL 10/11/2010
    Public WriteOnly Property AddButtonImage() As Image
        Set(ByVal value As Image)
            bsRefDetailAddButton.Image = value
        End Set
    End Property

    'To set the Icon for Edit Detailed Range Button
    'Created by: DL 10/11/2010
    Public WriteOnly Property EditButtonImage() As Image
        Set(ByVal value As Image)
            bsRefDetailEditButton.Image = value
        End Set
    End Property
    'To set the Icon for Delete Detailed Range Button
    'Created by: DL 10/11/2010
    Public WriteOnly Property DeleteButtonImage() As Image
        Set(ByVal value As Image)
            bsRefDetailDeleteButton.Image = value
        End Set
    End Property

    'To set the Measure Unit of the Test for which the Reference Ranges are defined
    Public WriteOnly Property MeasureUnit() As String
        Set(ByVal value As String)
            bsGenericNormalUnitLabel.Text = value
            bsBorderLineUnitLabel.Text = value
            bsDetailedNormalUnitLabel.Text = value
        End Set
    End Property

    'To set the number of decimals allowed in min and max values (the number of decimals 
    'defined for the Test for which the Reference Ranges are defined
    'Created by:  SG 04/10/10
    Public WriteOnly Property RefNumDecimals() As Integer
        Set(ByVal value As Integer)
            bsRefNormalLowerLimitUpDown.DecimalPlaces = value
            bsRefNormalUpperLimitUpDown.DecimalPlaces = value
            bsRefBorderLineLowerLimitUpDown.DecimalPlaces = value
            bsRefBorderLineUpperLimitUpDown.DecimalPlaces = value
            bsRefDetailLowerUpDown.DecimalPlaces = value
            bsRefDetailUpperUpDown.DecimalPlaces = value

            If (bsRefNormalLowerLimitUpDown.Text <> "") Then
                bsRefNormalLowerLimitUpDown.Value = CDec(bsRefNormalLowerLimitUpDown.Text)
            End If
            If (bsRefNormalUpperLimitUpDown.Text <> "") Then
                bsRefNormalUpperLimitUpDown.Value = CDec(bsRefNormalUpperLimitUpDown.Text)
            End If
            If (bsRefBorderLineLowerLimitUpDown.Text <> "") Then
                bsRefBorderLineLowerLimitUpDown.Value = CDec(bsRefBorderLineLowerLimitUpDown.Text)
            End If
            If (bsRefBorderLineUpperLimitUpDown.Text <> "") Then
                bsRefBorderLineUpperLimitUpDown.Value = CDec(bsRefBorderLineUpperLimitUpDown.Text)
            End If
            If (bsRefDetailLowerUpDown.Text <> "") Then
                bsRefDetailLowerUpDown.Value = CDec(bsRefDetailLowerUpDown.Text)
            End If
            If (bsRefDetailUpperUpDown.Text <> "") Then
                bsRefDetailUpperUpDown.Value = CDec(bsRefDetailUpperUpDown.Text)
            End If
        End Set
    End Property

    'To set the multilanguage text for Generic RadioButton
    'Created by:  SG 04/10/10
    Public WriteOnly Property TextForGenericRadioButton() As String
        Set(ByVal value As String)
            bsRefGenericRadioButton.Text = value
        End Set
    End Property

    'To set the multilanguage text for Detailed RadioButton
    'Created by:  SG 04/10/10
    Public WriteOnly Property TextForDetailedRadioButton() As String
        Set(ByVal value As String)
            bsRefDetailedRadioButton.Text = value
        End Set
    End Property

    'To set the multilanguage text for labels for controls used to enter the Min Range Vale
    'Created by:  SG 04/10/10
    Public WriteOnly Property TextForMinValueLabel() As String
        Set(ByVal value As String)
            bsRefNormalMinLabel.Text = value
            bsRefDetailMinLabel.Text = value
        End Set
    End Property

    'To set the multilanguage text for labels for controls used to enter the Max Range Vale
    'Created by:  SG 04/10/10
    Public WriteOnly Property TextForMaxValueLabel() As String
        Set(ByVal value As String)
            bsRefNormalMaxLabel.Text = value
            bsRefDetailMaxLabel.Text = value
        End Set
    End Property

    'To set the multilanguage text for labels for Normality Range
    'Created by:  SG 04/10/10
    Public WriteOnly Property TextForNormalityLabel() As String
        Set(ByVal value As String)
            bsRefGenericNormalLabel.Text = value
            bsRefDetailNormalLabel.Text = value
        End Set
    End Property

    'To set the multilanguage text for the label for BorderLine Range
    'Created by:  SG 04/10/10
    Public WriteOnly Property TextForBorderLineLabel() As String
        Set(ByVal value As String)
            bsBorderLineLabel.Text = value
        End Set
    End Property

    'To set the multilanguage text for the label used for Gender ComboBox
    'Created by:  SG 04/10/10
    Public WriteOnly Property TextForGenderLabel() As String
        Set(ByVal value As String)
            bsGenderLabel.Text = value
        End Set
    End Property

    'To set the multilanguage text for the label used for Age Unit ComboBox
    'Created by:  SG 04/10/10
    Public WriteOnly Property TextForAgeUnitsLabel() As String
        Set(ByVal value As String)
            bsAgeUnitLabel.Text = value
        End Set
    End Property

    'To set the multilanguage text for the label of the control used to enter the Age From value
    'Created by:  SG 04/10/10
    Public WriteOnly Property TextForFromLabel() As String
        Set(ByVal value As String)
            bsFromLabel.Text = value
        End Set
    End Property

    'To set the multilanguage text for the label of the control used to enter the Age To value
    'Created by:  SG 04/10/10
    Public WriteOnly Property TextForToLabel() As String
        Set(ByVal value As String)
            bsToLabel.Text = value
        End Set
    End Property

    'To set multilanguage texts of all visible columns in the DataGrid of Detailed Reference Ranges
    'Created by:  SG 04/10/10
    Public WriteOnly Property TextForGenderColumn() As String
        Set(ByVal value As String)
            bsRefDetailDataGridView.Columns("GenderDesc").HeaderText = value
        End Set
    End Property

    Public WriteOnly Property TextForAgeColumn() As String
        Set(ByVal value As String)
            bsRefDetailDataGridView.Columns("AgeUnitDesc").HeaderText = value
        End Set
    End Property

    Public WriteOnly Property TextForFromColumn() As String
        Set(ByVal value As String)
            bsRefDetailDataGridView.Columns("AgeRangeFrom").HeaderText = value
        End Set
    End Property

    Public WriteOnly Property TextForToColumn() As String
        Set(ByVal value As String)
            bsRefDetailDataGridView.Columns("AgeRangeTo").HeaderText = value
        End Set
    End Property

    Public WriteOnly Property TextForMinColumn() As String
        Set(ByVal value As String)
            bsRefDetailDataGridView.Columns("NormalLowerLimit").HeaderText = value
        End Set
    End Property

    Public WriteOnly Property TextForMaxColumn() As String
        Set(ByVal value As String)
            bsRefDetailDataGridView.Columns("NormalUpperLimit").HeaderText = value
        End Set
    End Property

    'To set the multilanguage texts for ToolTips of all graphical buttons
    'Created by:  SG 04/10/10
    Public WriteOnly Property ToolTipForDetailEditButton() As String
        Set(ByVal value As String)
            bsControlToolTips.SetToolTip(bsRefDetailEditButton, value)
        End Set
    End Property

    Public WriteOnly Property ToolTipForDetailAddButton() As String
        Set(ByVal value As String)
            bsControlToolTips.SetToolTip(bsRefDetailAddButton, value)
        End Set
    End Property

    Public WriteOnly Property ToolTipForDetailDeleteButton() As String
        Set(ByVal value As String)
            bsControlToolTips.SetToolTip(bsRefDetailDeleteButton, value)
        End Set
    End Property
#End Region

#Region "Private Properties"
    'To manages when the Reference Ranges have been changed
    Private Property ChangesMade() As Boolean
        Get
            Return ChangesMadeAttribute
        End Get
        Set(ByVal value As Boolean)
            If (Not ChangesMadeAttribute) And (value) Then
                RaiseEvent ChangesMadeUp(Me, New System.EventArgs)
            End If
            ChangesMadeAttribute = value
        End Set
    End Property
#End Region

#Region "Declarations"
    Public RangeType As String
    Public ValidationError As Boolean = False
    Public ChangeSampleTypeDuringEdition As Boolean

    Private ISETestID As Integer
    Private OFFSTestID As Integer
    Private CalcTestID As Integer

    Private OSDecimalSeparator As String
    Private EditionMode As Boolean = False
    Private TestType As TestTypes = TestTypes.STANDARD

    Private ActiveRangeType As String
    Private SelectedSampleType As String

    'Datasets
    Private SelectedTestSamplesDS As TestSamplesDS
    Private SelectedTestRefRangesDS As TestRefRangesDS
    Private SelectedISETestSamplesDS As ISETestSamplesDS
    Private SelectedOffSystemTestSamplesDS As OffSystemTestSamplesDS

    'Datasets Framework
    Private AllFieldLimitsDS As FieldLimitsDS
    Private GendersMasterDataDS As PreloadedMasterDataDS
    Private AgeUnitsMasterDataDS As PreloadedMasterDataDS
    Private AllSampleTypesDS As MasterDataDS
    Private AllMessagesDS As MessagesDS

    'Auxiliary Collections
    Private qEditedGenericTestRefRanges As New List(Of TestRefRangesDS.tparTestRefRangesRow)
    Private qEditedRangeTypes As New List(Of TestSamplesDS.tparTestSamplesRow)
    Private RequiredDetailedRefControls As New List(Of Control)

    'Flags
    Private DeletedSampleType As String = ""
    Private SaveAndClose As Boolean = False
    Private CurrentSampleType As String
    Private IsDetailedRangeEditing As Boolean

    'Events
    Public Event ChangesMadeUp(ByVal sender As System.Object, ByVal e As System.EventArgs)
#End Region

#Region "Public Methods"

    ''' <summary>
    ''' Loads all the framework data from the parent form
    ''' </summary>
    ''' <param name="pFieldLimitsDS">FieldLimitsDS typed dataset</param>
    ''' <param name="pSampleTypesDS">MasterDataDS typed dataset</param>
    ''' <param name="pSexListDS">PreloadedMasterDataDS typed dataset</param>
    ''' <param name="pAgeUnitsDS">PreloadedMasterDataDS typed dataset</param>
    ''' <param name="pMessagesDS">MessagesDS typed dataset</param>
    ''' <param name="pOSDecimalSeparator">decimal separator</param>
    ''' <remarks>
    ''' Created by:  SG 01/09/2010
    ''' </remarks>
    Public Sub LoadDataSets_Framework(ByVal pFieldLimitsDS As FieldLimitsDS, ByVal pSampleTypesDS As MasterDataDS, _
                                      ByVal pSexListDS As PreloadedMasterDataDS, ByVal pAgeUnitsDS As PreloadedMasterDataDS, _
                                      ByVal pMessagesDS As MessagesDS, ByVal pOSDecimalSeparator As String)
        Try
            AllFieldLimitsDS = pFieldLimitsDS
            AllSampleTypesDS = pSampleTypesDS
            GendersMasterDataDS = pSexListDS
            AgeUnitsMasterDataDS = pAgeUnitsDS
            AllMessagesDS = pMessagesDS
            OSDecimalSeparator = pOSDecimalSeparator

            FillRefDetailedAreaCombos()
            PrepareControls()
        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    ''' <summary>
    ''' Loads the data necessary for editing Reference Ranges for an STANDARD Test
    ''' </summary>
    ''' <param name="pSelectedTestRefRangesDS">TestRefRangesDS typed dataset</param>
    ''' <param name="pSelectedTestSamplesDS">TestSamplesDS typed dataset</param>
    ''' <remarks>
    ''' Created by:  SG 01/09/2010
    ''' </remarks>
    Public Sub LoadDataStandard(ByRef pSelectedTestRefRangesDS As TestRefRangesDS, ByRef pSelectedTestSamplesDS As TestSamplesDS, _
                                ByVal pMeasureUnit As String, ByVal pSampleType As String)
        Try
            If (pSelectedTestSamplesDS.tparTestSamples.Rows.Count > 0) Then
                TestType = TestTypes.STANDARD
                MeasureUnit = pMeasureUnit
                SelectedTestSamplesDS = pSelectedTestSamplesDS
                SelectedTestRefRangesDS = pSelectedTestRefRangesDS

                Dim myRangeType As String = "NONE"
                Dim qTestSamples As New List(Of TestSamplesDS.tparTestSamplesRow)
                qTestSamples = (From a In SelectedTestSamplesDS.tparTestSamples _
                               Where a.SampleType = pSampleType _
                              Select a).ToList()

                If (qTestSamples.Count > 0) Then
                    Dim myTestID As Integer = qTestSamples.First.TestID
                    Dim mySampleType As String = pSampleType

                    If (Not qTestSamples.First.IsActiveRangeTypeNull) Then
                        myRangeType = qTestSamples.First.ActiveRangeType.Trim
                    End If
                    BindReferenceRangesControls(myTestID, myRangeType, mySampleType)

                    If (Not EditionModeAttribute) Then DisableControls("ALL")
                End If
                RefDetailDataGridUnselect()
            End If
        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    ''' <summary>
    ''' Creates the data needed for editing Reference Ranges for a STANDARD Test
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SG 01/09/2010
    ''' </remarks>
    Public Sub NewDataStandard(ByRef pSelectedTestRefRangesDS As TestRefRangesDS, ByVal pSampleType As String)
        Try
            SelectedTestRefRangesDS = pSelectedTestRefRangesDS

            TestType = TestTypes.STANDARD
            BindReferenceRangesControls(0, "NONE", pSampleType)

            If Not EditionModeAttribute Then DisableControls("ALL")
            RefDetailDataGridUnselect()
        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    ''' <summary>
    ''' Creates the data needed for editing Reference Ranges for a STANDARD Test
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 02/12/2010
    ''' </remarks>
    Public Sub NewDataOffSystem(ByRef pSelectedTestRefRangesDS As TestRefRangesDS, ByVal pSampleType As String)
        Try
            SelectedTestRefRangesDS = pSelectedTestRefRangesDS

            TestType = TestTypes.OFFSYSTEM
            BindReferenceRangesControls(0, "NONE", pSampleType)

            If Not EditionModeAttribute Then DisableControls("ALL")
            RefDetailDataGridUnselect()
        Catch ex As Exception
            Throw ex
        End Try
    End Sub


    ''' <summary>
    ''' Loads the data necessary for editing Reference Ranges for a CALCULATED Test
    ''' </summary>
    ''' <param name="pSelectedTestRefRangesDS">TestRefRangesDS typed dataset</param>
    ''' <param name="pSelectedCalculatedTestID">Calculated Test Identifier</param>
    ''' <param name="pActiveRangeType">Current Range Type: Generic or Detailed</param>
    ''' <param name="pMeasureUnit">Measure Unit of the Calculated Test</param>
    ''' <remarks>
    ''' Created by:  SG 01/09/2010
    ''' </remarks>
    Public Sub LoadDataCalculated(ByRef pSelectedTestRefRangesDS As TestRefRangesDS, ByVal pSelectedCalculatedTestID As Integer, _
                                  ByVal pActiveRangeType As String, ByVal pMeasureUnit As String)
        Try
            SelectedTestRefRangesDS = pSelectedTestRefRangesDS

            TestType = TestTypes.CALCULATED
            CalcTestID = pSelectedCalculatedTestID
            RangeType = pActiveRangeType
            MeasureUnit = pMeasureUnit

            'Border Line Generic Values are not available for Calculated Tests
            bsBorderLineLabel.Visible = False
            bsBorderLineUnitLabel.Visible = False
            bsRefBorderLineLowerLimitUpDown.Visible = False
            bsRefBorderLineUpperLimitUpDown.Visible = False

            BindReferenceRangesControls(CalcTestID, RangeType, "")

            If (Not EditionModeAttribute) Then DisableControls("ALL")
            RefDetailDataGridUnselect()
        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    ''' <summary>
    ''' Loads the data necessary for editing Reference Ranges for an ISE Test
    ''' </summary>
    ''' <param name="pSelectedTestRefRangesDS">TestRefRangesDS typed dataset</param>
    ''' <param name="pSelectedISETestSamplesDS">ISETestSamplesDS typed dataset</param>
    ''' <remarks>
    ''' Created by:  SG 01/09/2010
    ''' </remarks>
    Public Sub LoadDataISE(ByRef pSelectedTestRefRangesDS As TestRefRangesDS, ByRef pSelectedISETestSamplesDS As ISETestSamplesDS, _
                           ByVal pISEUnit As String, ByVal pISESampleType As String)
        Try
            If (pSelectedISETestSamplesDS.tparISETestSamples.Rows.Count > 0) Then
                SelectedTestRefRangesDS = pSelectedTestRefRangesDS

                TestType = TestTypes.ISE
                SelectedISETestSamplesDS = pSelectedISETestSamplesDS
                MeasureUnit = pISEUnit

                'Border Line Generic Values are not available for ISE Tests
                bsBorderLineLabel.Visible = False
                bsBorderLineUnitLabel.Visible = False
                bsRefBorderLineLowerLimitUpDown.Visible = False
                bsRefBorderLineUpperLimitUpDown.Visible = False

                Dim myRangeType As String = "NONE"

                Dim qISETestSamples As New List(Of ISETestSamplesDS.tparISETestSamplesRow)
                qISETestSamples = (From a In SelectedISETestSamplesDS.tparISETestSamples _
                                  Where a.SampleType = pISESampleType _
                                 Select a).ToList()

                If (qISETestSamples.Count > 0) Then
                    Dim myISETestID As Integer = qISETestSamples.First.ISETestID
                    Dim myISESampleType As String = pISESampleType

                    If (Not qISETestSamples.First.IsActiveRangeTypeNull) Then
                        myRangeType = qISETestSamples.First.ActiveRangeType.Trim
                    End If
                    BindReferenceRangesControls(myISETestID, myRangeType, myISESampleType)

                    If (Not EditionModeAttribute) Then DisableControls("ALL")
                End If
                RefDetailDataGridUnselect()
            End If
        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    ''' <summary>
    ''' Loads the data necessary for editing Reference Ranges for an OffSystem Test
    ''' </summary>
    ''' <param name="pSelectedTestRefRangesDS">TestRefRangesDS typed dataset</param>
    ''' <param name="pSelectedOffSystemTestSamplesDS">OffSystemTestSamplesDS typed dataset</param>
    ''' <param name="pOffSystemUnit">OffSystemUnit</param> 
    ''' <param name="pOffSystemSampleType">OffSystemSampleType</param>
    ''' <remarks>
    ''' Created by:  DL 25/11/2010
    ''' </remarks>
    Public Sub LoadDataOffSystem(ByRef pSelectedTestRefRangesDS As TestRefRangesDS, _
                                 ByRef pSelectedOffSystemTestSamplesDS As OffSystemTestSamplesDS, _
                                 ByVal pOffSystemUnit As String, _
                                 ByVal pOffSystemSampleType As String)
        Try

            If (pSelectedOffSystemTestSamplesDS IsNot Nothing AndAlso pSelectedOffSystemTestSamplesDS.tparOffSystemTestSamples.Rows.Count > 0) Then
                SelectedTestRefRangesDS = pSelectedTestRefRangesDS

                TestType = TestTypes.OFFSYSTEM
                SelectedOffSystemTestSamplesDS = pSelectedOffSystemTestSamplesDS
                MeasureUnit = pOffSystemUnit

                'Border Line Generic Values are not available for offsystem Tests
                bsBorderLineLabel.Visible = False
                bsBorderLineUnitLabel.Visible = False
                bsRefBorderLineLowerLimitUpDown.Visible = False
                bsRefBorderLineUpperLimitUpDown.Visible = False

                Dim myRangeType As String = "NONE"

                Dim qOffSystemTestSamples As New List(Of OffSystemTestSamplesDS.tparOffSystemTestSamplesRow)

                'AG 14/12/2010
                'qOffSystemTestSamples = (From a In SelectedOffSystemTestSamplesDS.tparOffSystemTestSamples _
                '         Where a.SampleType = pOffSystemSampleType _
                '         Select a).ToList()
                qOffSystemTestSamples = (From a In SelectedOffSystemTestSamplesDS.tparOffSystemTestSamples _
                                         Select a).ToList()

                If (qOffSystemTestSamples.Count > 0) Then
                    Dim myOffSystemTestID As Integer = qOffSystemTestSamples.First.OffSystemTestID
                    'Dim myOffSystemSampleType As String = pOffSystemSampleType

                    If (Not qOffSystemTestSamples.First.IsActiveRangeTypeNull) Then
                        myRangeType = qOffSystemTestSamples.First.ActiveRangeType.Trim
                    End If
                    BindReferenceRangesControls(myOffSystemTestID, myRangeType, "") 'myOffSystemSampleType)

                    If (Not EditionModeAttribute) Then DisableControls("ALL")
                End If
                RefDetailDataGridUnselect()
            End If


        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    ''' <summary>
    ''' Erases all the data loaded in the control and clears the controls
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub ClearData()
        Try
            CalcTestID = 0
            RangeType = ""
            MeasureUnit = ""

            bsRefGenericRadioButton.Checked = False
            bsRefDetailedRadioButton.Checked = False

            If (Not EditionModeAttribute) Then DisableControls("ALL")
            ClearReferenceRangesControls()
        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    ''' <summary>
    ''' Updates the editing data to the temporal dataset
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SG 01/09/2010
    ''' Modified by: SA 18/11/2010 - For Detailed Ranges, updates in the temporal DataSet also the description
    '''                              for fields Gender and Age Unit
    '''              XB 04/02/2013 - Upper conversions redundants because the value is already in UpperCase must delete to avoid Regional Settings problems (Bugs tracking #1112)
    ''' </remarks>
    Public Sub UpdateTestRefRanges(ByVal pTestId As Integer, ByVal pSampleType As String)
        Dim sampleType As String
        Dim testRefRangesRowGeneric As TestRefRangesDS.tparTestRefRangesRow = Nothing
        Dim testRefRangesRowDetailed As TestRefRangesDS.tparTestRefRangesRow

        Try
            Dim qTestSampleRow As List(Of TestSamplesDS.tparTestSamplesRow) = Nothing
            Dim qISETestSampleRow As List(Of ISETestSamplesDS.tparISETestSamplesRow)
            Dim qOFFSTestSampleRow As List(Of OffSystemTestSamplesDS.tparOffSystemTestSamplesRow)
            Dim myTestType As String = "STD"

            If (TestType = TestTypes.STANDARD) Then
                'STANDARD Test
                myTestType = "STD"
                qTestSampleRow = GetTestSampleListBySampleType(pTestId, pSampleType)

            ElseIf (TestType = TestTypes.ISE) Then
                'ISE Test
                myTestType = "ISE"
                qISETestSampleRow = GetISETestSampleListBySampleType(pTestId, pSampleType)

                If (qISETestSampleRow.Count > 0) Then
                    Dim myTestSamplesDS As New TestSamplesDS

                    For Each row As ISETestSamplesDS.tparISETestSamplesRow In qISETestSampleRow
                        Dim myTestSamplesRow As TestSamplesDS.tparTestSamplesRow = myTestSamplesDS.tparTestSamples.NewtparTestSamplesRow
                        With myTestSamplesRow
                            .TestID = ISETestID
                            .SampleType = row.SampleType
                        End With
                        myTestSamplesDS.tparTestSamples.AddtparTestSamplesRow(myTestSamplesRow)
                        myTestSamplesDS.AcceptChanges()

                        qTestSampleRow = New List(Of TestSamplesDS.tparTestSamplesRow)
                        qTestSampleRow.Add(myTestSamplesDS.tparTestSamples.First)
                    Next
                End If

            ElseIf (TestType = TestTypes.OFFSYSTEM) Then
                'OFFS Test
                myTestType = "OFFS"
                qOFFSTestSampleRow = GetOffSystemTestSampleListBySampleType(pTestId, pSampleType)

                If (qOFFSTestSampleRow.Count > 0) Then
                    Dim myTestSamplesDS As New TestSamplesDS

                    For Each row As OffSystemTestSamplesDS.tparOffSystemTestSamplesRow In qOFFSTestSampleRow
                        Dim myTestSamplesRow As TestSamplesDS.tparTestSamplesRow = myTestSamplesDS.tparTestSamples.NewtparTestSamplesRow
                        With myTestSamplesRow
                            .TestID = OFFSTestID
                            .SampleType = row.SampleType
                        End With
                        myTestSamplesDS.tparTestSamples.AddtparTestSamplesRow(myTestSamplesRow)
                        myTestSamplesDS.AcceptChanges()

                        qTestSampleRow = New List(Of TestSamplesDS.tparTestSamplesRow)
                        qTestSampleRow.Add(myTestSamplesDS.tparTestSamples.First)
                    Next

                Else
                    CreateNewTestRefRanges(pTestId, pSampleType) ' dl 13/12/2010
                End If


            ElseIf (TestType = TestTypes.CALCULATED) Then
                'CALCULATED Test
                myTestType = "CALC"

                Dim myTestSamplesDS As New TestSamplesDS
                Dim myTestSamplesRow As TestSamplesDS.tparTestSamplesRow = myTestSamplesDS.tparTestSamples.NewtparTestSamplesRow
                With myTestSamplesRow
                    .TestID = CalcTestID
                    .SampleType = pSampleType
                End With
                myTestSamplesDS.tparTestSamples.AddtparTestSamplesRow(myTestSamplesRow)
                myTestSamplesDS.AcceptChanges()

                qTestSampleRow = New List(Of TestSamplesDS.tparTestSamplesRow)
                qTestSampleRow.Add(myTestSamplesDS.tparTestSamples.First)
            End If

            If (qTestSampleRow IsNot Nothing) Then
                'If not found, then create a new one and add it to the list
                For item As Integer = 0 To qTestSampleRow.Count - 1
                    sampleType = qTestSampleRow(item).SampleType

                    If (pSampleType = sampleType) Then
                        RefCheckIfNewOrDeletedGenericBySampleType(sampleType)

                        '----------------'
                        ' GENERIC Ranges '
                        '----------------'
                        'Already locally saved
                        For Each r As TestRefRangesDS.tparTestRefRangesRow In SelectedTestRefRangesDS.tparTestRefRanges.Rows
                            testRefRangesRowGeneric = Nothing
                            If (r.RangeType = "GENERIC" And r.SampleType = sampleType) Then
                                testRefRangesRowGeneric = r
                                Exit For
                            End If
                        Next

                        'Deleted
                        If (qEditedGenericTestRefRanges.Count > 0) Then
                            If (qEditedGenericTestRefRanges.First.IsDeleted) Then
                                For Each e As TestRefRangesDS.tparTestRefRangesRow In qEditedGenericTestRefRanges
                                    testRefRangesRowGeneric = Nothing
                                    If (e.SampleType = sampleType) Then
                                        testRefRangesRowGeneric = e
                                        Exit For
                                    End If
                                Next
                            End If
                        End If

                        'Newly created
                        If (testRefRangesRowGeneric Is Nothing) Then
                            For Each e As TestRefRangesDS.tparTestRefRangesRow In qEditedGenericTestRefRanges
                                If (e.SampleType = sampleType) Then
                                    testRefRangesRowGeneric = e
                                    Exit For
                                End If
                            Next e

                            If (testRefRangesRowGeneric IsNot Nothing) Then
                                testRefRangesRowGeneric.IsNew = True
                                testRefRangesRowGeneric.SampleType = sampleType
                                testRefRangesRowGeneric.TS_DateTime = DateTime.Now
                            End If
                        End If

                        If (testRefRangesRowGeneric IsNot Nothing) Then
                            If (Not testRefRangesRowGeneric.IsDeleted) Then
                                'NormalLowerLimit
                                If (bsRefNormalLowerLimitUpDown.Text <> "") Then
                                    testRefRangesRowGeneric.NormalLowerLimit = CType(bsRefNormalLowerLimitUpDown.Text, Single)
                                Else
                                    testRefRangesRowGeneric.SetNormalLowerLimitNull()
                                End If

                                'NormalUpperLimit
                                If (bsRefNormalUpperLimitUpDown.Text <> "") Then
                                    testRefRangesRowGeneric.NormalUpperLimit = CType(bsRefNormalUpperLimitUpDown.Text, Single)
                                Else
                                    testRefRangesRowGeneric.SetNormalUpperLimitNull()
                                End If

                                'BorderLineLowerLimit
                                If (bsRefBorderLineLowerLimitUpDown.Text <> "") Then
                                    testRefRangesRowGeneric.BorderLineLowerLimit = CType(bsRefBorderLineLowerLimitUpDown.Text, Single)
                                Else
                                    testRefRangesRowGeneric.SetBorderLineLowerLimitNull()
                                End If

                                'BorderLineUpperLimit
                                If (bsRefBorderLineUpperLimitUpDown.Text <> "") Then
                                    testRefRangesRowGeneric.BorderLineUpperLimit = CType(bsRefBorderLineUpperLimitUpDown.Text, Single)
                                Else
                                    testRefRangesRowGeneric.SetBorderLineUpperLimitNull()
                                End If

                                testRefRangesRowGeneric.IsDeleted = False
                                testRefRangesRowGeneric.TestID = pTestId
                                testRefRangesRowGeneric.SampleType = sampleType
                                testRefRangesRowGeneric.RangeType = "GENERIC"

                                'Data for Detailed Ranges is set to null
                                testRefRangesRowGeneric.SetGenderNull()
                                testRefRangesRowGeneric.SetAgeUnitNull()
                                testRefRangesRowGeneric.SetAgeRangeFromNull()
                                testRefRangesRowGeneric.SetAgeRangeToNull()
                            End If

                            'Add the row
                            If (testRefRangesRowGeneric.RowState = DataRowState.Detached) Then
                                testRefRangesRowGeneric.TestType = myTestType 'AG 26/10/2010
                                SelectedTestRefRangesDS.tparTestRefRanges.AddtparTestRefRangesRow(testRefRangesRowGeneric)
                            End If
                            SelectedTestRefRangesDS.tparTestRefRanges.AcceptChanges()
                        End If
                        qEditedGenericTestRefRanges.Clear()

                        '-----------------'
                        ' DETAILED Ranges '
                        '-----------------'
                        'Update the dataset setting to delete the removed pre-existing ranges
                        RefRemoveDetailedRefRange(sampleType)

                        'First we have to see what type of detailed range has been edited
                        Dim CurrentDetailedType As String
                        CurrentDetailedType = RefGetGridDetailedType(sampleType)

                        If (CurrentDetailedType <> "NONE") Then
                            For Each dgr As DataGridViewRow In bsRefDetailDataGridView.Rows
                                testRefRangesRowDetailed = Nothing

                                'First we determine if there is any recent change in the grid
                                'If (dgr.Cells("SampleType").Value.ToString.ToUpper = sampleType.ToUpper And _
                                If (dgr.Cells("SampleType").Value.ToString = sampleType And _
                                    dgr.Cells("RangeID").Value IsNot Nothing) Then
                                    'Range recently added
                                    Dim rangeID As Integer = CInt(dgr.Cells("RangeID").Value)
                                    Dim isAdded As Boolean = CBool(dgr.Cells("Added").Value)
                                    dgr.Cells("Added").Value = False

                                    'Look for detailed ranges in local db
                                    For Each dr As TestRefRangesDS.tparTestRefRangesRow In SelectedTestRefRangesDS.tparTestRefRanges.Rows
                                        If (dr.RangeType = "DETAILED" And dr.SampleType = sampleType) Then
                                            If dr.RangeID = rangeID And Not isAdded Then 'exists. update
                                                testRefRangesRowDetailed = dr
                                                Exit For
                                            End If
                                        End If
                                    Next

                                    CurrentDetailedType = RefGetGridDetailedType(pSampleType)

                                    If testRefRangesRowDetailed Is Nothing Then 'don't exists. create new one
                                        testRefRangesRowDetailed = SelectedTestRefRangesDS.tparTestRefRanges.NewtparTestRefRangesRow
                                        testRefRangesRowDetailed.IsNew = True
                                        testRefRangesRowDetailed.IsDeleted = False
                                        testRefRangesRowDetailed.SampleType = sampleType
                                        'TestRefRangesRowDetailed.TS_User = "Admin" 'AG 25/10/2010
                                        testRefRangesRowDetailed.TS_DateTime = DateTime.Now
                                        testRefRangesRowDetailed.RangeID = rangeID 'assign the same id that in the grid
                                    End If

                                    If Not testRefRangesRowDetailed Is Nothing Then

                                        If Not testRefRangesRowDetailed.IsDeleted Then

                                            testRefRangesRowDetailed.TestID = pTestId
                                            testRefRangesRowDetailed.SampleType = sampleType
                                            testRefRangesRowDetailed.RangeType = "DETAILED"

                                            'DetailLowerLimit
                                            If dgr.Cells("NormalLowerLimit").Value IsNot Nothing Then
                                                testRefRangesRowDetailed.NormalLowerLimit = CType(dgr.Cells("NormalLowerLimit").Value, Single)
                                            Else
                                                testRefRangesRowDetailed.SetNormalLowerLimitNull()
                                            End If

                                            'DetailUpperLimit
                                            If dgr.Cells("NormalUpperLimit").Value IsNot Nothing Then
                                                testRefRangesRowDetailed.NormalUpperLimit = CType(dgr.Cells("NormalUpperLimit").Value, Single)
                                            Else
                                                testRefRangesRowDetailed.SetNormalUpperLimitNull()
                                            End If

                                            'Gender
                                            If CurrentDetailedType = "GENDER" Or CurrentDetailedType = "GENDER_AGE" Then
                                                If dgr.Cells("Gender").Value IsNot Nothing Then
                                                    testRefRangesRowDetailed.Gender = CType(dgr.Cells("Gender").Value, String)
                                                    testRefRangesRowDetailed.GenderDesc = CType(dgr.Cells("GenderDesc").Value, String)
                                                Else
                                                    testRefRangesRowDetailed.SetGenderNull()
                                                End If
                                            Else
                                                testRefRangesRowDetailed.SetGenderNull()
                                            End If

                                            'AgeUnit
                                            If CurrentDetailedType = "AGE" Or CurrentDetailedType = "GENDER_AGE" Then
                                                If dgr.Cells("AgeUnit").Value IsNot Nothing Then
                                                    testRefRangesRowDetailed.AgeUnit = CType(dgr.Cells("AgeUnit").Value, String)
                                                    testRefRangesRowDetailed.AgeUnitDesc = CType(dgr.Cells("AgeUnitDesc").Value, String)
                                                Else
                                                    testRefRangesRowDetailed.SetAgeUnitNull()
                                                End If

                                                'AgeFrom
                                                If dgr.Cells("AgeRangeFrom").Value IsNot Nothing Then
                                                    testRefRangesRowDetailed.AgeRangeFrom = CType(dgr.Cells("AgeRangeFrom").Value, Integer)
                                                Else
                                                    testRefRangesRowDetailed.SetAgeRangeFromNull()
                                                End If

                                                'AgeTo
                                                If dgr.Cells("AgeRangeTo").Value IsNot Nothing Then
                                                    testRefRangesRowDetailed.AgeRangeTo = CType(dgr.Cells("AgeRangeTo").Value, Integer)
                                                Else
                                                    testRefRangesRowDetailed.SetAgeRangeToNull()
                                                End If
                                            Else
                                                testRefRangesRowDetailed.SetAgeUnitNull()
                                                testRefRangesRowDetailed.SetAgeRangeFromNull()
                                                testRefRangesRowDetailed.SetAgeRangeToNull()
                                            End If

                                            'generic data set to null
                                            testRefRangesRowDetailed.SetBorderLineLowerLimitNull()
                                            testRefRangesRowDetailed.SetBorderLineUpperLimitNull()

                                        End If

                                        If testRefRangesRowDetailed.RowState = DataRowState.Detached Then 'new
                                            If isAdded Then
                                                testRefRangesRowDetailed.TestType = myTestType 'AG 26/10/2010
                                                SelectedTestRefRangesDS.tparTestRefRanges.AddtparTestRefRangesRow(testRefRangesRowDetailed)
                                            End If

                                        End If

                                        SelectedTestRefRangesDS.tparTestRefRanges.AcceptChanges()
                                    End If
                                End If
                            Next

                            bsControlErrorProvider.Clear()
                            ValidationError = False
                            RefDetailResetValues()
                        End If
                    End If
                Next
            End If
        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    ''' <summary>
    ''' Validates the editing data
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SG 01/09/2010
    ''' Modified by: AG 07/07/2010 - Get the multilanguage text of error messages to be shown in the Error Provider.
    '''                              Do not shown an error when both BorderLimit controls are empty
    ''' </remarks>
    Public Sub ValidateRefRangesLimits(ByVal pMandatory As Boolean, Optional ByVal pSaving As Boolean = False)
        Try
            If (bsRefGenericRadioButton.Checked Or pSaving) Then
                ValidateLimitsUpDown(bsRefNormalLowerLimitUpDown, bsRefNormalUpperLimitUpDown, pMandatory)

                If (Not ValidationError) Then
                    If (bsRefNormalLowerLimitUpDown.Text <> "" And bsRefNormalUpperLimitUpDown.Text <> "") Then
                        ValidateLimitsUpDown(bsRefBorderLineLowerLimitUpDown, bsRefBorderLineUpperLimitUpDown, False)

                        If (bsRefBorderLineLowerLimitUpDown.Text <> "" And bsRefBorderLineUpperLimitUpDown.Text <> "") Then
                            If (bsRefNormalLowerLimitUpDown.Value >= bsRefBorderLineLowerLimitUpDown.Value) Then
                                bsControlErrorProvider.SetError(bsRefNormalLowerLimitUpDown, GetMessageText(RefMessages.WRONG_RANGE_LIMITS.ToString))
                                bsControlErrorProvider.SetError(bsRefBorderLineLowerLimitUpDown, GetMessageText(RefMessages.WRONG_RANGE_LIMITS.ToString))
                                ValidationError = True
                            End If
                            If (bsRefBorderLineLowerLimitUpDown.Value >= bsRefNormalUpperLimitUpDown.Value) Then
                                bsControlErrorProvider.SetError(bsRefNormalUpperLimitUpDown, GetMessageText(RefMessages.WRONG_RANGE_LIMITS.ToString))
                                bsControlErrorProvider.SetError(bsRefBorderLineLowerLimitUpDown, GetMessageText(RefMessages.WRONG_RANGE_LIMITS.ToString))
                                ValidationError = True
                            End If
                            If (bsRefBorderLineUpperLimitUpDown.Value <= bsRefNormalUpperLimitUpDown.Value) Then
                                bsControlErrorProvider.SetError(bsRefNormalUpperLimitUpDown, GetMessageText(RefMessages.WRONG_RANGE_LIMITS.ToString))
                                bsControlErrorProvider.SetError(bsRefBorderLineUpperLimitUpDown, GetMessageText(RefMessages.WRONG_RANGE_LIMITS.ToString))
                                ValidationError = True
                            End If
                        End If
                    End If
                End If

            ElseIf (bsRefDetailedRadioButton.Checked And IsDetailedRangeEditing Or pSaving) Then
                For Each ctrl As Control In RequiredDetailedRefControls
                    If (TypeOf (ctrl) Is ComboBox) Then
                        Dim myCombo As ComboBox = CType(ctrl, ComboBox)
                        If (myCombo.SelectedIndex = -1) Then
                            bsControlErrorProvider.SetError(ctrl, GetMessageText(RefMessages.REQUIRED_VALUE.ToString))
                            ValidationError = True
                        End If
                    End If
                Next

                If (bsRefDetailAgeCheckBox.Checked) Then
                    ValidateLimitsUpDown(bsRefDetailAgeFromUpDown, bsRefDetailAgeToUpDown, pMandatory And True)
                End If
                ValidateLimitsUpDown(bsRefDetailLowerUpDown, bsRefDetailUpperUpDown, pMandatory)
            End If

            If (Not ValidationError) Then bsControlErrorProvider.Clear()
        Catch ex As Exception
            Throw ex
        End Try
    End Sub


    ''' <summary>
    ''' Checks if there is any Reference Range defined for the informed SampleType
    ''' </summary>
    ''' <param name="pSampleType">Sample Type</param>
    ''' <remarks>
    ''' Created by:  SG 01/09/2010
    ''' </remarks>
    Public Function IsReferenceRangesDefined(ByVal pSampleType As String) As Boolean
        Try
            Dim sampleType As String = pSampleType

            If (bsRefGenericRadioButton.Checked) Then
                RangeType = "GENERIC"
                If (bsRefNormalLowerLimitUpDown.Text <> "" Or bsRefNormalUpperLimitUpDown.Text <> "") Then
                    Return True
                Else
                    bsRefGenericRadioButton.Checked = False
                    Return IsReferenceRangesDefined(sampleType)
                End If

            ElseIf (bsRefDetailedRadioButton.Checked) Then
                RangeType = "DETAILED"
                If (RefCountDetailRowsBySampleType(sampleType) > 0) Then
                    Return True
                Else
                    bsRefDetailedRadioButton.Checked = False
                    Return IsReferenceRangesDefined(sampleType)
                End If

            Else
                RangeType = ""
                If (bsRefNormalLowerLimitUpDown.Text <> "") Then
                    bsRefGenericRadioButton.Checked = True
                    Return IsReferenceRangesDefined(sampleType)

                ElseIf (RefCountDetailRowsBySampleType(sampleType) > 0) Then
                    bsRefDetailedRadioButton.Checked = True
                    Return IsReferenceRangesDefined(sampleType)

                Else
                    Return False
                End If
            End If
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    ''' <summary>
    ''' Updates the DataGrid with the editing Detailed Reference Range
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SG 01/09/2010
    ''' </remarks>
    Public Sub RefUpdateDetailDataGridViewBySampleType(ByVal pTestID As Integer, ByVal pNewSampleType As String)
        Dim NewSampleType As String
        Dim FirstSampleType As String
        Dim NewDetailedRows As New List(Of DataGridViewRow)

        Try
            NewSampleType = pNewSampleType
            FirstSampleType = GetFirstSampleType(pTestID)

            Dim OrderedSampleTypes As New List(Of MasterDataDS.tcfgMasterDataRow)
            OrderedSampleTypes = GetSampleTypesByPosition()

            Dim qTestRefRanges As New List(Of TestRefRangesDS.tparTestRefRangesRow)
            For Each st As MasterDataDS.tcfgMasterDataRow In OrderedSampleTypes
                qTestRefRanges = (From a In SelectedTestRefRangesDS.tparTestRefRanges _
                                 Where a.SampleType = st.ItemID And a.RangeType = "DETAILED" _
                                Select a).ToList()

                If (qTestRefRanges.Count > 0) Then
                    bsRefDetailDataGridView.Rows.Clear()
                    For Each row As TestRefRangesDS.tparTestRefRangesRow In qTestRefRanges
                        Dim r As Integer = bsRefDetailDataGridView.Rows.Add

                        bsRefDetailDataGridView.Rows(r).Cells(1).Value = NewSampleType
                        bsRefDetailDataGridView.Rows(r).Cells("Added").Value = True

                        If (qTestRefRanges(r).IsRangeIDNull) Then
                            bsRefDetailDataGridView.Rows(r).Cells("RangeID").Value = Nothing
                        Else
                            bsRefDetailDataGridView.Rows(r).Cells("RangeID").Value = -(r + 1)
                        End If
                        If (qTestRefRanges(r).IsGenderNull) Then
                            bsRefDetailGenderCheckBox.Checked = False
                            bsRefDetailDataGridView.Rows(r).Cells("Gender").Value = Nothing
                        Else
                            bsRefDetailGenderCheckBox.Checked = True
                            bsRefDetailDataGridView.Rows(r).Cells("Gender").Value = qTestRefRanges(r).Gender
                        End If
                        If (qTestRefRanges(r).IsAgeUnitNull) Then
                            bsRefDetailAgeCheckBox.Checked = False
                            bsRefDetailDataGridView.Rows(r).Cells("AgeUnit").Value = Nothing
                        Else
                            bsRefDetailAgeCheckBox.Checked = True
                            bsRefDetailDataGridView.Rows(r).Cells("AgeUnit").Value = qTestRefRanges(r).AgeUnit
                        End If
                        If (qTestRefRanges(r).IsAgeRangeFromNull) Then
                            bsRefDetailDataGridView.Rows(r).Cells("AgeRangeFrom").Value = Nothing
                        Else
                            bsRefDetailDataGridView.Rows(r).Cells("AgeRangeFrom").Value = qTestRefRanges(r).AgeRangeFrom
                        End If
                        If (qTestRefRanges(r).IsAgeRangeToNull) Then
                            bsRefDetailDataGridView.Rows(r).Cells("AgeRangeTo").Value = Nothing
                        Else
                            bsRefDetailDataGridView.Rows(r).Cells("AgeRangeTo").Value = qTestRefRanges(r).AgeRangeTo
                        End If
                        If (qTestRefRanges(r).IsNormalLowerLimitNull) Then
                            bsRefDetailDataGridView.Rows(r).Cells("NormalLowerLimit").Value = Nothing
                        Else
                            bsRefDetailDataGridView.Rows(r).Cells("NormalLowerLimit").Value = qTestRefRanges(r).NormalLowerLimit
                        End If
                        If (qTestRefRanges(r).IsNormalUpperLimitNull) Then
                            bsRefDetailDataGridView.Rows(r).Cells("NormalUpperLimit").Value = Nothing
                        Else
                            bsRefDetailDataGridView.Rows(r).Cells("NormalUpperLimit").Value = qTestRefRanges(r).NormalUpperLimit
                        End If
                    Next

                    RefDetailDataGridUnselect()
                    Exit For
                End If
            Next
        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    ''' <summary>
    ''' Updates the editing Generic Reference Range
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SG 01/09/2010
    ''' </remarks>
    Public Sub RefUpdateGenericBySampleType(ByVal pTestID As Integer, ByVal pNewSampleType As String)
        Dim NewSampleType As String
        Dim FirstSampleType As String
        Dim NewDetailedRows As New List(Of DataGridViewRow)

        Try
            NewSampleType = pNewSampleType
            FirstSampleType = GetFirstSampleType(pTestID)

            Dim OrderedSampleTypes As New List(Of MasterDataDS.tcfgMasterDataRow)
            OrderedSampleTypes = GetSampleTypesByPosition()

            Dim qTestRefRanges As New List(Of TestRefRangesDS.tparTestRefRangesRow)
            For Each st As MasterDataDS.tcfgMasterDataRow In OrderedSampleTypes
                qTestRefRanges = (From a In SelectedTestRefRangesDS.tparTestRefRanges _
                                 Where a.SampleType = st.ItemID And a.RangeType = "GENERIC" _
                                Select a).ToList()

                If (qTestRefRanges.Count > 0) Then
                    If (qTestRefRanges.First.IsNormalLowerLimitNull) Then
                        bsRefNormalLowerLimitUpDown.Text = ""
                    Else
                        bsRefNormalLowerLimitUpDown.Text = qTestRefRanges.First.NormalLowerLimit.ToString
                    End If

                    If (qTestRefRanges.First.IsNormalUpperLimitNull) Then
                        bsRefNormalUpperLimitUpDown.Text = ""
                    Else
                        bsRefNormalUpperLimitUpDown.Text = qTestRefRanges.First.NormalUpperLimit.ToString
                    End If

                    If (qTestRefRanges.First.IsBorderLineLowerLimitNull) Then
                        bsRefBorderLineLowerLimitUpDown.Text = ""
                    Else
                        bsRefBorderLineLowerLimitUpDown.Text = qTestRefRanges.First.BorderLineLowerLimit.ToString
                    End If

                    If (qTestRefRanges.First.IsBorderLineUpperLimitNull) Then
                        bsRefBorderLineUpperLimitUpDown.Text = ""
                    Else
                        bsRefBorderLineUpperLimitUpDown.Text = qTestRefRanges.First.BorderLineUpperLimit.ToString
                    End If
                    Exit For
                End If
            Next
        Catch ex As Exception
            Throw ex
        End Try
    End Sub


#End Region

#Region "Private Methods"

    ''' <summary>
    ''' Control initialization
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SG 01/09/2010
    ''' </remarks>
    Private Sub PrepareControls()
        Try
            bsRefGenericRadioButton.Checked = False
            bsRefDetailedRadioButton.Checked = False

            bsRefNormalLowerLimitUpDown.Value = bsRefNormalLowerLimitUpDown.Minimum
            bsRefNormalUpperLimitUpDown.Value = bsRefNormalUpperLimitUpDown.Minimum
            bsRefNormalLowerLimitUpDown.ResetText()
            bsRefNormalUpperLimitUpDown.ResetText()

            bsRefBorderLineLowerLimitUpDown.Value = bsRefBorderLineLowerLimitUpDown.Minimum
            bsRefBorderLineUpperLimitUpDown.Value = bsRefBorderLineUpperLimitUpDown.Minimum
            bsRefBorderLineLowerLimitUpDown.ResetText()
            bsRefBorderLineUpperLimitUpDown.ResetText()

            bsRefDetailGenderCheckBox.Checked = False
            bsRefDetailGenderComboBox.SelectedIndex = -1

            bsRefDetailAgeCheckBox.Checked = False
            bsRefDetailAgeComboBox.SelectedIndex = -1
            bsRefDetailAgeFromUpDown.Value = bsRefDetailAgeFromUpDown.Minimum
            bsRefDetailAgeToUpDown.Value = bsRefDetailAgeToUpDown.Minimum
            bsRefDetailAgeFromUpDown.ResetText()
            bsRefDetailAgeToUpDown.ResetText()

            bsRefDetailLowerUpDown.Value = bsRefDetailLowerUpDown.Minimum
            bsRefDetailUpperUpDown.Value = bsRefDetailUpperUpDown.Minimum
            bsRefDetailLowerUpDown.ResetText()
            bsRefDetailUpperUpDown.ResetText()

            bsRefDetailDataGridView.Rows.Clear()
            RefRadioButtonsManagement()
        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    ''' <summary>
    ''' Enables all Reference Range controls
    ''' </summary>
    ''' <remarks>
    ''' Modified by: SA 18/11/2010 
    ''' </remarks>
    Private Sub EnableControls(ByVal pArea As String)
        Try
            If (pArea <> "DETAILED") Then
                bsRefGenericRadioButton.Enabled = True
                bsRefNormalLowerLimitUpDown.Enabled = True
                bsRefNormalUpperLimitUpDown.Enabled = True

                bsRefNormalLowerLimitUpDown.BackColor = Color.White
                bsRefNormalUpperLimitUpDown.BackColor = Color.White

                If (bsRefNormalLowerLimitUpDown.Text <> "") Then
                    bsRefBorderLineLowerLimitUpDown.Enabled = True
                    bsRefBorderLineUpperLimitUpDown.Enabled = True

                    bsRefBorderLineLowerLimitUpDown.BackColor = Color.White
                    bsRefBorderLineUpperLimitUpDown.BackColor = Color.White
                End If
            End If

            If (pArea <> "GENERIC") Then
                bsRefDetailedRadioButton.Enabled = True
                bsRefDetailGenderCheckBox.Enabled = True
                bsRefDetailGenderComboBox.Enabled = True
                bsRefDetailAgeCheckBox.Enabled = True
                bsRefDetailAgeComboBox.Enabled = True
                bsRefDetailAgeFromUpDown.Enabled = True
                bsRefDetailAgeToUpDown.Enabled = True

                bsRefDetailGenderComboBox.BackColor = Color.White
                bsRefDetailAgeComboBox.BackColor = Color.White
                bsRefDetailAgeFromUpDown.BackColor = Color.White
                bsRefDetailAgeToUpDown.BackColor = Color.White

                bsRefDetailLowerUpDown.Enabled = True
                bsRefDetailUpperUpDown.Enabled = True

                bsRefDetailLowerUpDown.BackColor = Color.White
                bsRefDetailUpperUpDown.BackColor = Color.White

                bsRefDetailAddButton.Enabled = True
                bsRefDetailEditButton.Enabled = True
                bsRefDetailDeleteButton.Enabled = True

                bsRefDetailDataGridView.Enabled = True
            End If

            'bsRefGenericRadioButton.Enabled = True
            'bsRefDetailedRadioButton.Enabled = True
            'bsRefNormalLowerLimitUpDown.Enabled = True
            'bsRefNormalUpperLimitUpDown.Enabled = True
            'bsRefBorderLineLowerLimitUpDown.Enabled = True
            'bsRefBorderLineUpperLimitUpDown.Enabled = True
            'bsRefDetailAgeCheckBox.Enabled = True
            'bsRefDetailGenderCheckBox.Enabled = True
            'bsRefDetailAgeComboBox.Enabled = True
            'bsRefDetailGenderComboBox.Enabled = True
            'bsRefDetailAgeFromUpDown.Enabled = True
            'bsRefDetailAgeToUpDown.Enabled = True
            'bsRefDetailLowerUpDown.Enabled = True
            'bsRefDetailUpperUpDown.Enabled = True
            'bsRefDetailDataGridView.Enabled = True
        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    ''' <summary>
    ''' Disables all Reference Range controls or the controls in the specified area (Generic/Detailed)
    ''' </summary>
    ''' <remarks>
    ''' Modified by: SA 18/11/2010 - Disable also the buttons in the Detailed Ref Ranges area; change the BackColor 
    '''                              to SystemsColor.MenuBar. Added parameter to indicate which area is enabled/disabled
    ''' </remarks>
    Private Sub DisableControls(ByVal pArea As String)
        Try
            If (pArea <> "DETAILED") Then
                bsRefGenericRadioButton.Enabled = False
                bsRefNormalLowerLimitUpDown.Enabled = False
                bsRefNormalUpperLimitUpDown.Enabled = False
                bsRefBorderLineLowerLimitUpDown.Enabled = False
                bsRefBorderLineUpperLimitUpDown.Enabled = False

                bsRefNormalLowerLimitUpDown.BackColor = SystemColors.MenuBar
                bsRefNormalUpperLimitUpDown.BackColor = SystemColors.MenuBar
                bsRefBorderLineLowerLimitUpDown.BackColor = SystemColors.MenuBar
                bsRefBorderLineUpperLimitUpDown.BackColor = SystemColors.MenuBar
            End If

            If (pArea <> "GENERIC") Then
                bsRefDetailedRadioButton.Enabled = False
                bsRefDetailGenderCheckBox.Enabled = False
                bsRefDetailGenderComboBox.Enabled = False
                bsRefDetailAgeCheckBox.Enabled = False
                bsRefDetailAgeComboBox.Enabled = False
                bsRefDetailAgeFromUpDown.Enabled = False
                bsRefDetailAgeToUpDown.Enabled = False

                bsRefDetailGenderComboBox.BackColor = SystemColors.MenuBar
                bsRefDetailAgeComboBox.BackColor = SystemColors.MenuBar
                bsRefDetailAgeFromUpDown.BackColor = SystemColors.MenuBar
                bsRefDetailAgeToUpDown.BackColor = SystemColors.MenuBar

                bsRefDetailLowerUpDown.Enabled = False
                bsRefDetailUpperUpDown.Enabled = False

                bsRefDetailLowerUpDown.BackColor = SystemColors.MenuBar
                bsRefDetailUpperUpDown.BackColor = SystemColors.MenuBar

                bsRefDetailAddButton.Enabled = False
                bsRefDetailEditButton.Enabled = False
                bsRefDetailDeleteButton.Enabled = False

                bsRefDetailDataGridView.Enabled = False
            End If
        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    ''' <summary>
    ''' Clears all Reference Range controls
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SG 01/09/2010
    ''' </remarks>
    Private Sub ClearReferenceRangesControls()
        Try
            bsRefNormalLowerLimitUpDown.ResetText()
            bsRefNormalUpperLimitUpDown.ResetText()
            bsRefBorderLineLowerLimitUpDown.ResetText()
            bsRefBorderLineUpperLimitUpDown.ResetText()

            If bsRefDetailGenderComboBox.Items.Count > 0 Then bsRefDetailGenderComboBox.SelectedIndex = 0
            If bsRefDetailAgeComboBox.Items.Count > 0 Then bsRefDetailAgeComboBox.SelectedIndex = 0

            bsRefDetailAgeFromUpDown.ResetText()
            bsRefDetailAgeToUpDown.ResetText()
            bsRefDetailLowerUpDown.ResetText()
            bsRefDetailUpperUpDown.ResetText()

            bsRefNormalLowerLimitUpDown.Value = bsRefNormalLowerLimitUpDown.Minimum
            bsRefNormalUpperLimitUpDown.Value = bsRefNormalUpperLimitUpDown.Minimum
            bsRefBorderLineLowerLimitUpDown.Value = bsRefBorderLineLowerLimitUpDown.Minimum
            bsRefBorderLineUpperLimitUpDown.Value = bsRefBorderLineUpperLimitUpDown.Minimum

            RefDetailResetValues()
            bsRefDetailGenderCheckBox.Checked = True
            bsRefDetailAgeCheckBox.Checked = False

            bsRefDetailDataGridView.Rows.Clear()
            BsRangeIDLabel.Text = ""

            bsControlErrorProvider.Clear()
            ValidationError = False
        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    ''' <summary>
    ''' Fills the Reference Range controls with the input data 
    ''' </summary>
    ''' <param name="pTestID">Test Identifier</param>
    ''' <param name="pRangeType">Range Type: Generic or Detailed</param>
    ''' <param name="pSampleType">Current Sample Type for the Test</param>
    ''' <remarks>
    ''' Created by:  SG 01/09/2010
    ''' Modified by: XB 04/02/2013 - Upper conversions redundants because the value is already in UpperCase must delete to avoid Regional Settings problems (Bugs tracking #1112)
    ''' </remarks>
    Private Sub BindReferenceRangesControls(ByVal pTestID As Integer, ByVal pRangeType As String, _
                                            Optional ByVal pSampleType As String = "")
        Try
            SelectedSampleType = pSampleType 'SG 02/09/2010
            CurrentSampleType = pSampleType

            ClearReferenceRangesControls()
            ChangesMade = False

            If (DeletedSampleType <> "") Then
                Dim qDeletedTestRefRanges As New List(Of TestRefRangesDS.tparTestRefRangesRow)
                qDeletedTestRefRanges = (From a In SelectedTestRefRangesDS.tparTestRefRanges _
                                        Where a.TestID = pTestID AndAlso a.SampleType = DeletedSampleType _
                                       Select a).ToList()

                If (qDeletedTestRefRanges.Count > 0) Then
                    For Each refRange As TestRefRangesDS.tparTestRefRangesRow In qDeletedTestRefRanges
                        refRange.IsDeleted = True
                        refRange.IsNew = False
                    Next
                    SelectedTestRefRangesDS.tparTestRefRanges.AcceptChanges()
                End If

                DeletedSampleType = ""
            End If

            'Generic Ranges...
            Dim qTestRefRanges As New List(Of TestRefRangesDS.tparTestRefRangesRow)
            qTestRefRanges = (From a In SelectedTestRefRangesDS.tparTestRefRanges _
                             Where a.RangeType = "GENERIC" _
                               And a.SampleType = pSampleType.Trim _
                           And Not a.IsDeleted _
                            Select a).ToList()
            'And a.SampleType.ToUpper = pSampleType.Trim.ToUpper _

            If (qTestRefRanges.Count > 0) Then
                For Each tparTestRefRangesRow As TestRefRangesDS.tparTestRefRangesRow In qTestRefRanges
                    If (Not tparTestRefRangesRow.IsDeleted) Then
                        'AG 19/07/2010
                        'If tparTestRefRangesRow.RangeType = "GENERIC" And tparTestRefRangesRow.SampleType.ToUpper = SelectedSampleTypeCombo.Text.ToUpper Then
                        'If (tparTestRefRangesRow.RangeType = "GENERIC" And tparTestRefRangesRow.SampleType.ToUpper = pSampleType.ToUpper.Trim) Then
                        If (tparTestRefRangesRow.RangeType = "GENERIC" And tparTestRefRangesRow.SampleType = pSampleType.Trim) Then
                            'If tparTestRefRangesRow.SampleType.ToUpper = SelectedSampleTypeCombo.Text.ToUpper Then
                            'If (tparTestRefRangesRow.SampleType.ToUpper = pSampleType.ToUpper.Trim) Then
                            If (tparTestRefRangesRow.SampleType = pSampleType.Trim) Then
                                'Normality Range
                                If (tparTestRefRangesRow.IsNormalLowerLimitNull) Then
                                    bsRefNormalLowerLimitUpDown.Value = bsRefNormalLowerLimitUpDown.Minimum
                                    bsRefNormalLowerLimitUpDown.ResetText()
                                Else
                                    bsRefNormalLowerLimitUpDown.Text = CType(tparTestRefRangesRow.NormalLowerLimit, Decimal).ToString()
                                End If
                                If (tparTestRefRangesRow.IsNormalUpperLimitNull) Then
                                    bsRefNormalUpperLimitUpDown.Value = bsRefNormalUpperLimitUpDown.Minimum
                                    bsRefNormalUpperLimitUpDown.ResetText()
                                Else
                                    bsRefNormalUpperLimitUpDown.Text = CType(tparTestRefRangesRow.NormalUpperLimit, Decimal).ToString()
                                End If

                                'Borderline Range
                                If (tparTestRefRangesRow.IsBorderLineLowerLimitNull) Then
                                    bsRefBorderLineLowerLimitUpDown.Value = bsRefBorderLineLowerLimitUpDown.Minimum
                                    bsRefBorderLineLowerLimitUpDown.ResetText()
                                Else
                                    bsRefBorderLineLowerLimitUpDown.Text = CType(tparTestRefRangesRow.BorderLineLowerLimit, Decimal).ToString()
                                End If
                                If (tparTestRefRangesRow.IsBorderLineUpperLimitNull) Then
                                    bsRefBorderLineUpperLimitUpDown.Value = bsRefBorderLineUpperLimitUpDown.Minimum
                                    bsRefBorderLineUpperLimitUpDown.ResetText()
                                Else
                                    bsRefBorderLineUpperLimitUpDown.Text = CType(tparTestRefRangesRow.BorderLineUpperLimit, Decimal).ToString()
                                End If
                                Exit For
                            Else
                                bsRefNormalLowerLimitUpDown.ResetText()
                                bsRefNormalUpperLimitUpDown.ResetText()
                                bsRefBorderLineLowerLimitUpDown.ResetText()
                                bsRefBorderLineUpperLimitUpDown.ResetText()
                                bsRefGenericRadioButton.Checked = False
                            End If
                            Exit For
                        End If
                    End If
                Next
            Else
                bsRefNormalLowerLimitUpDown.ResetText()
                bsRefNormalUpperLimitUpDown.ResetText()
                bsRefBorderLineLowerLimitUpDown.ResetText()
                bsRefBorderLineUpperLimitUpDown.ResetText()

                bsRefGenericRadioButton.Checked = False
                bsRefDetailedRadioButton.Checked = False
            End If

            ValidateLimitsUpDown(bsRefNormalLowerLimitUpDown, bsRefNormalUpperLimitUpDown, False)
            ValidateLimitsUpDown(bsRefBorderLineLowerLimitUpDown, bsRefBorderLineUpperLimitUpDown, False)

            bsRefDetailDataGridView.Rows.Clear()
            BindRefDetailDataGridView(pTestID, pSampleType) 'AG 19/07/2010 (add SampleType parameter) SG 05/07/2010

            'Select Case (pRangeType.Trim.ToUpper)
            Select Case (pRangeType.Trim)
                Case "GENERIC"
                    bsRefGenericRadioButton.Checked = True
                    bsRefDetailedRadioButton.Checked = False
                Case "DETAILED"
                    bsRefGenericRadioButton.Checked = False
                    bsRefDetailedRadioButton.Checked = True
                Case Else
                    bsRefGenericRadioButton.Checked = False
                    bsRefDetailedRadioButton.Checked = False
            End Select

            RefRadioButtonsManagement()            'SG 05/07/2010
            RefDetailUpdateCheckBoxes(pSampleType) 'SG 14/07/2010
            RefDetailCheckBoxesManagement()        'SG 14/07/2010
            RefRadioButtonsManagement()            'SG 05/07/2010
        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    ''' <summary>
    ''' Load the received detailed Reference Ranges in the DataGridView
    ''' </summary>
    ''' <param name="pTestID">Test Identifier</param>
    ''' <param name="pSampleType">Sample Type Code</param>
    ''' <remarks>
    ''' Created by:  SG 01/09/2010
    ''' Modified by: SA 17/11/2010 - For fields having multilanguage texts, load both fields Code and Description
    '''                              in the grid (Code is placed in a hidden column)
    '''              XB 04/02/2013 - Upper conversions redundants because the value is already in UpperCase must delete to avoid Regional Settings problems (Bugs tracking #1112)
    ''' </remarks>
    Private Sub BindRefDetailDataGridView(ByVal pTestID As Integer, ByVal pSampleType As String)
        Try
            bsRefDetailDataGridView.Rows.Clear()

            Dim qTestRefRanges As New List(Of TestRefRangesDS.tparTestRefRangesRow)
            qTestRefRanges = (From a In SelectedTestRefRangesDS.tparTestRefRanges _
                             Where a.TestID = pTestID _
                               And a.RangeType = "DETAILED" _
                               And a.SampleType = pSampleType _
                           And Not a.IsDeleted _
                            Select a).ToList()

            'Load the detailed Reference Ranges in the DataGrid
            If (qTestRefRanges.Count > 0) Then
                For Each rangeRow As TestRefRangesDS.tparTestRefRangesRow In qTestRefRanges
                    Dim r As Integer = bsRefDetailDataGridView.Rows.Add()
                    If (rangeRow.IsSampleTypeNull) Then
                        Exit For
                    Else
                        bsRefDetailDataGridView.Rows(r).Cells("SampleType").Value = rangeRow.SampleType
                        'If (rangeRow.SampleType.ToUpper <> pSampleType.Trim.ToUpper) Then
                        If (rangeRow.SampleType <> pSampleType.Trim) Then
                            bsRefDetailDataGridView.Rows(r).Visible = False
                        End If
                    End If
                    If (rangeRow.IsRangeIDNull) Then
                        bsRefDetailDataGridView.Rows(r).Cells("RangeID").Value = Nothing
                    Else
                        bsRefDetailDataGridView.Rows(r).Cells("RangeID").Value = rangeRow.RangeID
                    End If
                    If (rangeRow.IsGenderNull) Then
                        bsRefDetailGenderCheckBox.Checked = False
                        bsRefDetailDataGridView.Rows(r).Cells("Gender").Value = Nothing
                        bsRefDetailDataGridView.Rows(r).Cells("GenderDesc").Value = Nothing
                    Else
                        bsRefDetailGenderCheckBox.Checked = True
                        bsRefDetailDataGridView.Rows(r).Cells("Gender").Value = rangeRow.Gender
                        bsRefDetailDataGridView.Rows(r).Cells("GenderDesc").Value = rangeRow.GenderDesc
                    End If
                    If (rangeRow.IsAgeUnitNull) Then
                        bsRefDetailAgeCheckBox.Checked = False
                        bsRefDetailDataGridView.Rows(r).Cells("AgeUnit").Value = Nothing
                        bsRefDetailDataGridView.Rows(r).Cells("AgeUnitDesc").Value = Nothing
                    Else
                        bsRefDetailAgeCheckBox.Checked = True
                        bsRefDetailDataGridView.Rows(r).Cells("AgeUnit").Value = rangeRow.AgeUnit
                        bsRefDetailDataGridView.Rows(r).Cells("AgeUnitDesc").Value = rangeRow.AgeUnitDesc
                    End If
                    If (rangeRow.IsAgeRangeFromNull) Then
                        bsRefDetailDataGridView.Rows(r).Cells("AgeRangeFrom").Value = Nothing
                    Else
                        bsRefDetailDataGridView.Rows(r).Cells("AgeRangeFrom").Value = rangeRow.AgeRangeFrom
                    End If
                    If (rangeRow.IsAgeRangeToNull) Then
                        bsRefDetailDataGridView.Rows(r).Cells("AgeRangeTo").Value = Nothing
                    Else
                        bsRefDetailDataGridView.Rows(r).Cells("AgeRangeTo").Value = rangeRow.AgeRangeTo
                    End If
                    If (rangeRow.IsNormalLowerLimitNull) Then
                        bsRefDetailDataGridView.Rows(r).Cells("NormalLowerLimit").Value = Nothing
                    Else
                        bsRefDetailDataGridView.Rows(r).Cells("NormalLowerLimit").Value = rangeRow.NormalLowerLimit
                    End If
                    If (rangeRow.IsNormalUpperLimitNull) Then
                        bsRefDetailDataGridView.Rows(r).Cells("NormalUpperLimit").Value = Nothing
                    Else
                        bsRefDetailDataGridView.Rows(r).Cells("NormalUpperLimit").Value = rangeRow.NormalUpperLimit
                    End If
                    bsRefDetailDataGridView.Rows(r).Cells("Added").Value = False
                Next
            Else
                bsRefDetailedRadioButton.Checked = False
            End If

            'Unselect all reference range rows in the grid
            If (bsRefDetailDataGridView.SelectedRows.Count > 0) Then
                For r As Integer = 0 To bsRefDetailDataGridView.Rows.Count - 1 Step 1
                    bsRefDetailDataGridView.Rows(r).Selected = False
                Next
            End If
            RefDetailCheckBoxesManagement()
        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    ''' <summary>
    ''' Set the Min and Max allowed values for Age From/To according the selected Age Unit
    ''' </summary>
    ''' <remarks>
    ''' Modified by: SA 17/11/2010 - Use the Code of the selected AgeUnit, not the multilanguage description
    ''' </remarks>
    Private Sub RefDetailAgeLimitsSetUp()
        Dim myFieldLimits As FieldLimitsDS.tfmwFieldLimitsRow = Nothing

        Try
            If (bsRefDetailAgeComboBox.SelectedIndex > -1) Then
                Dim myPreloadedMasterData As PreloadedMasterDataDS.tfmwPreloadedMasterDataRow
                myPreloadedMasterData = CType(bsRefDetailAgeComboBox.SelectedItem, PreloadedMasterDataDS.tfmwPreloadedMasterDataRow)

                Select Case CStr(myPreloadedMasterData.ItemID).Trim
                    Case "Y"
                        myFieldLimits = GetControlsLimits(RefFieldLimitsEnum.AGE_FROM_TO_YEARS)
                    Case "M"
                        myFieldLimits = GetControlsLimits(RefFieldLimitsEnum.AGE_FROM_TO_MONTHS)
                    Case "D"
                        myFieldLimits = GetControlsLimits(RefFieldLimitsEnum.AGE_FROM_TO_DAYS)
                End Select

                If (myFieldLimits IsNot Nothing) Then
                    bsRefDetailAgeToUpDown.Minimum = CType(myFieldLimits.MinValue, Integer)
                    bsRefDetailAgeToUpDown.Maximum = CType(myFieldLimits.MaxValue, Integer)
                    bsRefDetailAgeToUpDown.DecimalPlaces = 0
                    bsRefDetailAgeToUpDown.Increment = 1

                    bsRefDetailAgeFromUpDown.Minimum = CType(myFieldLimits.MinValue, Integer)
                    bsRefDetailAgeFromUpDown.Maximum = CType(myFieldLimits.MaxValue, Integer)
                    bsRefDetailAgeFromUpDown.DecimalPlaces = 0
                    bsRefDetailAgeFromUpDown.Increment = 1
                Else
                    myFieldLimits = GetControlsLimits(RefFieldLimitsEnum.AGE_FROM_TO_YEARS)

                    bsRefDetailAgeToUpDown.Minimum = 1
                    bsRefDetailAgeToUpDown.Maximum = 110
                    bsRefDetailAgeToUpDown.DecimalPlaces = 0
                    bsRefDetailAgeToUpDown.Increment = 1

                    bsRefDetailAgeFromUpDown.Minimum = 1
                    bsRefDetailAgeFromUpDown.Maximum = 110
                    bsRefDetailAgeFromUpDown.DecimalPlaces = 0
                    bsRefDetailAgeFromUpDown.Increment = 1
                End If
            End If
        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    ''' <summary>
    ''' Auxiliary function that returns Min and Max Values for the specified Limit Identifier
    ''' </summary>
    ''' <param name="pLimitsID">Limit Identifier</param>
    ''' <returns></returns>
    ''' <remarks>
    ''' Created by:  SG 01/09/2010
    ''' </remarks>
    Private Function GetControlsLimits(ByVal pLimitsID As RefFieldLimitsEnum) As FieldLimitsDS.tfmwFieldLimitsRow
        Dim qFieldLimits As New List(Of FieldLimitsDS.tfmwFieldLimitsRow)

        Try
            qFieldLimits = (From a In AllFieldLimitsDS.tfmwFieldLimits _
                           Where a.LimitID = pLimitsID.ToString _
                          Select a).ToList()

            If (qFieldLimits.Count = 0) Then Return Nothing
        Catch ex As Exception
            Throw ex
        End Try
        Return qFieldLimits.First
    End Function

    ''' <summary>
    ''' Checks if there is there is a new or deleted Reference Range in the tenporal dataset
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SG 01/09/2010
    ''' Modified by: XB 04/02/2013 - Upper conversions redundants because the value is already in UpperCase must delete to avoid Regional Settings problems (Bugs tracking #1112)
    ''' </remarks>
    Private Sub RefCheckIfNewOrDeletedGenericBySampleType(ByVal pSampleType As String)
        Try
            qEditedGenericTestRefRanges = (From a In SelectedTestRefRangesDS.tparTestRefRanges _
                                          Where a.SampleType = pSampleType And a.RangeType = "GENERIC" _
                                         Select a).ToList()

            Dim hasdata As Boolean = False
            Dim newRow As TestRefRangesDS.tparTestRefRangesRow

            If (qEditedGenericTestRefRanges.Count = 0) Then
                newRow = SelectedTestRefRangesDS.tparTestRefRanges.NewtparTestRefRangesRow
                newRow.IsNew = True
                newRow.SampleType = pSampleType '.ToUpper
                newRow.RangeID = -999

                'NormalLowerLimit
                If (bsRefNormalLowerLimitUpDown.Text <> "") Then
                    newRow.NormalLowerLimit = CType(bsRefNormalLowerLimitUpDown.Text, Single)
                    hasdata = True
                Else
                    newRow.SetNormalLowerLimitNull()
                End If

                'NormalUpperLimit
                If (bsRefNormalUpperLimitUpDown.Text <> "") Then
                    newRow.NormalUpperLimit = CType(bsRefNormalUpperLimitUpDown.Text, Single)
                    hasdata = True
                Else
                    newRow.SetNormalUpperLimitNull()
                End If

                'BorderLineLowerLimit
                If (bsRefBorderLineLowerLimitUpDown.Text <> "") Then
                    newRow.BorderLineLowerLimit = CType(bsRefBorderLineLowerLimitUpDown.Text, Single)
                    hasdata = True
                Else
                    newRow.SetBorderLineLowerLimitNull()
                End If

                'BorderLineUpperLimit
                If (bsRefBorderLineUpperLimitUpDown.Text <> "") Then
                    newRow.BorderLineUpperLimit = CType(bsRefBorderLineUpperLimitUpDown.Text, Single)
                    hasdata = True
                Else
                    newRow.SetBorderLineUpperLimitNull()
                End If

                If (hasdata) Then
                    qEditedGenericTestRefRanges.Add(newRow)
                End If
            Else
                If (bsRefNormalLowerLimitUpDown.Text = "" And bsRefNormalUpperLimitUpDown.Text = "") Then
                    qEditedGenericTestRefRanges.First.IsDeleted = True
                Else
                    qEditedGenericTestRefRanges.First.IsDeleted = False
                End If
            End If
        Catch ex As Exception
            Throw ex
        End Try
    End Sub


    ''' <summary>
    ''' Validates that Detailed Reference Range by Age are not overlapped
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SG 01/09/2010
    ''' Modified by: SA 18/11/2010 - Compare codes, not multilanguage descriptions
    '''              XB 04/02/2013 - Upper conversions redundants because the value is already in UpperCase must delete to avoid Regional Settings problems (Bugs tracking #1112)
    ''' </remarks>
    Private Sub ValidateRefRangesNotOverlapped(Optional ByVal pOriginalRangeID As String = "")
        Try
            If (Not ValidationError) Then
                Dim gender As String = ""
                If (bsRefDetailGenderCheckBox.Checked) Then
                    gender = bsRefDetailGenderComboBox.SelectedValue.ToString.Trim
                End If

                Dim unit As String = ""
                If (bsRefDetailAgeCheckBox.Checked) Then
                    unit = bsRefDetailAgeComboBox.SelectedValue.ToString.Trim()
                End If

                Dim min As Integer
                If (bsRefDetailAgeFromUpDown.Text <> "") Then
                    min = CInt(bsRefDetailAgeFromUpDown.Text)
                Else
                    Exit Sub
                End If

                Dim max As Integer
                If (bsRefDetailAgeToUpDown.Text <> "") Then
                    max = CInt(bsRefDetailAgeToUpDown.Text)
                Else
                    Exit Sub
                End If

                For Each refRangeRow As DataGridViewRow In bsRefDetailDataGridView.Rows
                    If (refRangeRow.Visible) Then
                        If (refRangeRow.Cells("RangeID").Value.ToString <> pOriginalRangeID) Then
                            'If (refRangeRow.Cells("SampleType").Value.ToString.ToUpper = SelectedSampleType.ToUpper) Then
                            If (refRangeRow.Cells("SampleType").Value.ToString = SelectedSampleType) Then
                                Dim sex As String = ""
                                'If (gender <> "") Then sex = refRangeRow.Cells("Gender").Value.ToString.ToUpper.Trim
                                If (gender <> "") Then sex = refRangeRow.Cells("Gender").Value.ToString.Trim

                                'Dim ageUnits As String = refRangeRow.Cells("AgeUnit").Value.ToString.ToUpper.Trim
                                Dim ageUnits As String = refRangeRow.Cells("AgeUnit").Value.ToString.Trim
                                Dim ageFrom As Integer = CInt(refRangeRow.Cells("AgeRangeFrom").Value)
                                Dim ageTo As Integer = CInt(refRangeRow.Cells("AgeRangeTo").Value)

                                refRangeRow.DefaultCellStyle.ForeColor = Color.Black
                                If (unit = ageUnits And gender = sex) Then
                                    If (min >= ageFrom And min <= ageTo) Then
                                        ValidationError = True
                                        bsControlErrorProvider.SetError(bsRefDetailAgeFromUpDown, GetMessageText(RefMessages.WRONG_AGE_LIMITS.ToString))
                                    End If

                                    If (max >= ageFrom And max <= ageTo) Then
                                        ValidationError = True
                                        bsControlErrorProvider.SetError(bsRefDetailAgeToUpDown, GetMessageText(RefMessages.WRONG_AGE_LIMITS.ToString))
                                    End If

                                    If (min <= ageFrom And max >= ageTo) Then
                                        ValidationError = True
                                        bsControlErrorProvider.SetError(bsRefDetailAgeToUpDown, GetMessageText(RefMessages.WRONG_AGE_LIMITS.ToString))
                                    End If
                                End If

                                If (ValidationError) Then
                                    refRangeRow.DefaultCellStyle.ForeColor = Color.Red
                                    Exit Sub
                                End If
                            End If
                        End If
                    End If
                Next
            Else
                Exit Sub
            End If
        Catch ex As Exception
            ValidationError = True
            Throw ex
        End Try
    End Sub

    ''' <summary>
    ''' Manages the availability of the controls depending on which RadioButton is selected
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SG 01/09/2010
    ''' Modified by: SA 18/11/2010
    ''' </remarks>
    Private Sub RefRadioButtonsManagement()
        Try
            Dim generic As Boolean = bsRefGenericRadioButton.Checked
            Dim detailed As Boolean = bsRefDetailedRadioButton.Checked

            If (EditionModeAttribute) Then
                If (generic) Then
                    EnableControls("GENERIC")
                    'bsRefNormalLowerLimitUpDown.Enabled = True
                    'bsRefNormalUpperLimitUpDown.Enabled = True
                    'If (bsRefNormalLowerLimitUpDown.Text <> "") Then
                    '    bsRefBorderLineLowerLimitUpDown.Enabled = True
                    '    bsRefBorderLineUpperLimitUpDown.Enabled = True
                    'End If

                    'Disable all fields in the area of Detailed Reference Ranges
                    DisableControls("DETAILED")

                    bsRefDetailedRadioButton.Enabled = True
                    bsRefDetailGenderCheckBox.Checked = False
                    bsRefDetailAgeCheckBox.Checked = False

                    RefDetailResetValues()
                    RefDetailDataGridUnselect()

                    'bsRefDetailGenderComboBox.Enabled = False
                    'bsRefDetailAgeCheckBox.Enabled = False
                    'bsRefDetailLowerUpDown.Enabled = False
                    'bsRefDetailUpperUpDown.Enabled = False

                    'bsRefDetailGenderCheckBox.Enabled = False
                    'bsRefDetailAgeComboBox.Enabled = False
                    'bsRefDetailAgeFromUpDown.Enabled = False
                    'bsRefDetailAgeToUpDown.Enabled = False

                    'bsRefDetailAddButton.Enabled = False
                    'bsRefDetailEditButton.Enabled = False
                    'bsRefDetailDeleteButton.Enabled = False

                    'For Each dgr As DataGridViewRow In bsRefDetailDataGridView.SelectedRows
                    '    dgr.Selected = False
                    'Next
                    'bsRefDetailDataGridView.Enabled = False
                    'bsRefDetailedRadioButton.Checked = False

                ElseIf (detailed) Then
                    EnableControls("DETAILED")
                    RefDetailDataGridUnselect()

                    If (Not bsRefDetailAgeCheckBox.Checked) Then bsRefDetailGenderCheckBox.Checked = True
                    RefDetailCheckBoxesManagement()

                    'Disable all fields in the area of Generic Reference Ranges
                    DisableControls("GENERIC")
                    bsRefGenericRadioButton.Enabled = True

                    'bsRefDetailDataGridView.Enabled = True 'SG 27/06/2010
                    'bsRefNormalLowerLimitUpDown.Enabled = False
                    'bsRefNormalUpperLimitUpDown.Enabled = False
                    'bsRefBorderLineLowerLimitUpDown.Enabled = False
                    'bsRefBorderLineUpperLimitUpDown.Enabled = False

                    'bsRefDetailDataGridView.Enabled = True
                    'If (Not bsRefDetailAgeCheckBox.Checked) Then
                    '    bsRefDetailGenderCheckBox.Checked = True 'default
                    'End If

                    'RefDetailCheckBoxesManagement()
                    'bsRefGenericRadioButton.Checked = False

                ElseIf (Not IsReferenceRangesDefined(SelectedSampleType)) Then
                    DisableControls("ALL")
                    bsRefGenericRadioButton.Enabled = True
                    bsRefDetailedRadioButton.Enabled = True

                    'Disable all the controls
                    'bsRefNormalLowerLimitUpDown.Enabled = False
                    'bsRefNormalUpperLimitUpDown.Enabled = False
                    'bsRefBorderLineLowerLimitUpDown.Enabled = False
                    'bsRefBorderLineUpperLimitUpDown.Enabled = False

                    'bsRefDetailGenderComboBox.Enabled = False
                    'bsRefDetailAgeCheckBox.Enabled = False
                    'bsRefDetailLowerUpDown.Enabled = False
                    'bsRefDetailUpperUpDown.Enabled = False

                    'bsRefDetailGenderCheckBox.Enabled = False
                    'bsRefDetailAgeComboBox.Enabled = False
                    'bsRefDetailAgeFromUpDown.Enabled = False
                    'bsRefDetailAgeToUpDown.Enabled = False

                    'bsRefDetailAddButton.Enabled = False
                    'bsRefDetailEditButton.Enabled = False
                    'bsRefDetailDeleteButton.Enabled = False

                    'bsRefDetailDataGridView.Enabled = False
                End If
            End If
        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    ''' <summary>
    ''' Fills the combos needed for the Detailed Reference Ranges
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SG 01/09/2010
    ''' </remarks>
    Private Sub FillRefDetailedAreaCombos()
        Try
            'Age Units
            Dim qAgeUnits As List(Of PreloadedMasterDataDS.tfmwPreloadedMasterDataRow)
            qAgeUnits = (From a In AgeUnitsMasterDataDS.tfmwPreloadedMasterData _
                     Order By a.Position _
                       Select a).ToList()

            bsRefDetailAgeComboBox.DataSource = qAgeUnits
            bsRefDetailAgeComboBox.DisplayMember = "FixedItemDesc"
            bsRefDetailAgeComboBox.ValueMember = "ItemID"

            'Genders
            Dim qGender As List(Of PreloadedMasterDataDS.tfmwPreloadedMasterDataRow)
            qGender = (From a In GendersMasterDataDS.tfmwPreloadedMasterData _
                   Order By a.Position _
                     Select a).ToList()

            bsRefDetailGenderComboBox.DataSource = qGender
            bsRefDetailGenderComboBox.DisplayMember = "FixedItemDesc"
            bsRefDetailGenderComboBox.ValueMember = "ItemID"
        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    ''' <summary>
    ''' Manages the status of the checkboxes
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SG 01/09/2010
    ''' </remarks>
    Private Sub RefDetailUpdateCheckBoxes(ByVal pSampleType As String)
        Try
            Dim detailedType As String = RefGetGridDetailedType(pSampleType)
            Select Case detailedType
                Case "GENDER"
                    bsRefDetailGenderCheckBox.Checked = True
                    bsRefDetailAgeCheckBox.Checked = False

                Case "AGE"
                    bsRefDetailGenderCheckBox.Checked = False
                    bsRefDetailAgeCheckBox.Checked = True

                Case "GENDER_AGE"
                    bsRefDetailGenderCheckBox.Checked = True
                    bsRefDetailAgeCheckBox.Checked = True

                Case "NONE"
                    bsRefDetailGenderCheckBox.Checked = False
                    bsRefDetailAgeCheckBox.Checked = False

                Case Else
                    bsRefDetailGenderCheckBox.Checked = False
                    bsRefDetailAgeCheckBox.Checked = False
            End Select
        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    ''' <summary>
    ''' Manages the availability of the controls depending on which checkbox is checked
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SG 01/09/2010
    ''' Modified by: SA 18/11/2010 - When a CheckBox is unchecked, the ComboBox is shown empty, and when it is
    '''                              checked, the first element is selected in the related ComboBox
    ''' </remarks>
    Private Sub RefDetailCheckBoxesManagement()
        Dim none As Boolean = Not bsRefDetailGenderCheckBox.Checked And Not bsRefDetailAgeCheckBox.Checked
        Dim byGender As Boolean = bsRefDetailGenderCheckBox.Checked And Not bsRefDetailAgeCheckBox.Checked
        Dim byAge As Boolean = Not bsRefDetailGenderCheckBox.Checked And bsRefDetailAgeCheckBox.Checked
        Dim byBoth As Boolean = bsRefDetailGenderCheckBox.Checked And bsRefDetailAgeCheckBox.Checked

        Try
            If (EditionModeAttribute) Then
                'Default value is by Gender
                If (none) Then byGender = True

                bsRefDetailLowerUpDown.Enabled = True
                bsRefDetailUpperUpDown.Enabled = True

                If (byGender) Then
                    bsRefDetailGenderComboBox.Enabled = True
                    bsRefDetailGenderComboBox.SelectedIndex = 0
                    bsRefDetailGenderCheckBox.Enabled = False
                    bsRefDetailGenderComboBox.BackColor = Color.White

                    bsRefDetailAgeCheckBox.Enabled = True
                    bsRefDetailAgeComboBox.Enabled = False
                    bsRefDetailAgeComboBox.SelectedIndex = -1
                    bsRefDetailAgeFromUpDown.Enabled = False
                    bsRefDetailAgeToUpDown.Enabled = False

                    bsRefDetailAgeComboBox.BackColor = SystemColors.MenuBar
                    bsRefDetailAgeFromUpDown.BackColor = SystemColors.MenuBar
                    bsRefDetailAgeToUpDown.BackColor = SystemColors.MenuBar

                    'Clear the informed errors...
                    If (bsControlErrorProvider.GetError(bsRefDetailAgeFromUpDown) <> "") Then
                        bsControlErrorProvider.SetError(bsRefDetailAgeFromUpDown, String.Empty)
                    End If
                    If (bsControlErrorProvider.GetError(bsRefDetailAgeToUpDown) <> "") Then
                        bsControlErrorProvider.SetError(bsRefDetailAgeToUpDown, String.Empty)
                    End If
                    If (bsControlErrorProvider.GetError(bsRefDetailLowerUpDown) <> "") Then
                        bsControlErrorProvider.SetError(bsRefDetailLowerUpDown, String.Empty)
                    End If
                    If (bsControlErrorProvider.GetError(bsRefDetailUpperUpDown) <> "") Then
                        bsControlErrorProvider.SetError(bsRefDetailUpperUpDown, String.Empty)
                    End If

                    bsRefDetailAddButton.Enabled = True
                    bsRefDetailEditButton.Enabled = True
                    bsRefDetailDeleteButton.Enabled = True

                    RequiredDetailedRefControls.Clear()
                    RequiredDetailedRefControls.Add(bsRefDetailGenderComboBox)
                End If

                If (byAge) Then
                    bsRefDetailAgeComboBox.Enabled = True
                    bsRefDetailAgeComboBox.SelectedIndex = 0
                    bsRefDetailAgeFromUpDown.Enabled = True
                    bsRefDetailAgeToUpDown.Enabled = True
                    bsRefDetailAgeCheckBox.Enabled = False

                    bsRefDetailAgeComboBox.BackColor = Color.White
                    bsRefDetailAgeFromUpDown.BackColor = Color.White
                    bsRefDetailAgeToUpDown.BackColor = Color.White

                    bsRefDetailGenderCheckBox.Enabled = True
                    bsRefDetailGenderComboBox.Enabled = False
                    bsRefDetailGenderComboBox.SelectedIndex = -1
                    bsRefDetailGenderComboBox.BackColor = SystemColors.MenuBar

                    If (bsControlErrorProvider.GetError(bsRefDetailLowerUpDown) <> "") Then
                        bsControlErrorProvider.SetError(bsRefDetailLowerUpDown, String.Empty)
                    End If
                    If (bsControlErrorProvider.GetError(bsRefDetailUpperUpDown) <> "") Then
                        bsControlErrorProvider.SetError(bsRefDetailUpperUpDown, String.Empty)
                    End If

                    bsRefDetailAddButton.Enabled = True
                    bsRefDetailEditButton.Enabled = True
                    bsRefDetailDeleteButton.Enabled = True

                    RequiredDetailedRefControls.Clear()
                    RequiredDetailedRefControls.Add(bsRefDetailAgeComboBox)
                    RequiredDetailedRefControls.Add(bsRefDetailAgeFromUpDown)
                    RequiredDetailedRefControls.Add(bsRefDetailAgeToUpDown)
                End If

                If (byBoth) Then
                    bsRefDetailGenderComboBox.Enabled = True
                    bsRefDetailGenderComboBox.SelectedIndex = 0
                    bsRefDetailGenderCheckBox.Enabled = True
                    bsRefDetailGenderComboBox.BackColor = Color.White

                    bsRefDetailAgeComboBox.Enabled = True
                    bsRefDetailAgeComboBox.SelectedIndex = 0
                    bsRefDetailAgeFromUpDown.Enabled = True
                    bsRefDetailAgeToUpDown.Enabled = True
                    bsRefDetailAgeCheckBox.Enabled = True

                    bsRefDetailAgeComboBox.BackColor = Color.White
                    bsRefDetailAgeFromUpDown.BackColor = Color.White
                    bsRefDetailAgeToUpDown.BackColor = Color.White

                    bsRefDetailAddButton.Enabled = True
                    bsRefDetailEditButton.Enabled = True
                    bsRefDetailDeleteButton.Enabled = True

                    RequiredDetailedRefControls.Clear()
                    RequiredDetailedRefControls.Add(bsRefDetailGenderComboBox)
                    RequiredDetailedRefControls.Add(bsRefDetailAgeComboBox)
                    RequiredDetailedRefControls.Add(bsRefDetailAgeFromUpDown)
                    RequiredDetailedRefControls.Add(bsRefDetailAgeToUpDown)
                End If
            End If
        Catch ex As Exception
            Throw ex
        End Try
    End Sub


    ''' <summary>
    ''' Manages the updating of the DataGrid with data of the editing Detailed Range
    ''' </summary>
    ''' <param name="pSampleType">Sample Type for the Test</param>
    ''' <remarks>
    ''' Created by:  SG 01/09/2010
    ''' Modified by: XB 04/02/2013 - Upper conversions redundants because the value is already in UpperCase must delete to avoid Regional Settings problems (Bugs tracking #1112)
    ''' </remarks>
    Private Sub RefDetailUpdate(ByVal pSampleType As String)
        Dim selRow As Integer = -1
        Dim isNew As Boolean
        Dim isDifferent As Boolean
        Dim EqualRangeIndex As Integer

        Try
            ValidationError = False
            bsControlErrorProvider.Clear()
            ValidateRefRangesLimits(True)

            If (Not ValidationError) Then
                If (bsRefDetailLowerUpDown.Text <> "" And bsRefDetailUpperUpDown.Text <> "") Then
                    isDifferent = RefDetailRangePatternIsDifferent(pSampleType)
                    If (isDifferent) Then
                        Dim answer As DialogResult = ShowMessage(Text, RefMessages.DETAIL_REF_OVERWRITE.ToString) 'AG 07/07/2010 "DETAIL_REF_OVERWRITE")
                        If (answer = Windows.Forms.DialogResult.Yes) Then
                            'Mark to delete all the previous Detailed Ranges
                            Dim qAllTestRefRanges As New List(Of TestRefRangesDS.tparTestRefRangesRow)
                            qAllTestRefRanges = (From a In SelectedTestRefRangesDS.tparTestRefRanges _
                                                Where a.SampleType = pSampleType And a.RangeType = "DETAILED" _
                                               Select a).ToList()

                            For Each refRange As TestRefRangesDS.tparTestRefRangesRow In qAllTestRefRanges
                                refRange.IsDeleted = True
                            Next
                            SelectedTestRefRangesDS.tparTestRefRanges.AcceptChanges()

                            Dim indexesToRemove As New List(Of Integer)
                            For Each row As System.Windows.Forms.DataGridViewRow In bsRefDetailDataGridView.Rows
                                'If (CStr(row.Cells("SampleType").Value).ToUpper = pSampleType.ToUpper) Then
                                If (CStr(row.Cells("SampleType").Value) = pSampleType) Then
                                    indexesToRemove.Add(CInt(row.Cells("RangeID").Value))
                                End If
                            Next
                            For Each i As Integer In indexesToRemove
                                For Each dgr As System.Windows.Forms.DataGridViewRow In bsRefDetailDataGridView.Rows
                                    If (CInt(dgr.Cells("RangeID").Value) = i) Then
                                        bsRefDetailDataGridView.Rows.Remove(dgr)
                                        Exit For
                                    End If
                                Next
                            Next
                            EqualRangeIndex = -1
                            isNew = True
                        Else
                            RefDetailResetValues()
                            IsDetailedRangeEditing = False
                            Exit Sub
                        End If
                    Else
                        EqualRangeIndex = RefDetailRangeIsNew(pSampleType)
                        If (EqualRangeIndex = -1) Then
                            isNew = True
                        ElseIf (bsRefDetailDataGridView.SelectedRows.Count > 0) Then
                            If (CBool(bsRefDetailDataGridView.SelectedRows(0).Cells("Added").Value) = True) Then
                                isNew = True
                                selRow = EqualRangeIndex     'Exists but not still saved
                            Else
                                isNew = False
                                selRow = EqualRangeIndex
                            End If
                        Else
                            selRow = EqualRangeIndex
                        End If
                    End If

                    'SG 06/09/2010 if it's the same range it will manage as editing
                    If (bsRefDetailDataGridView.SelectedRows.Count > 0) AndAlso _
                       (BsRangeIDLabel.Text = bsRefDetailDataGridView.SelectedRows(0).Cells("RangeID").Value.ToString) Then
                        isNew = False

                        Dim myOriginalRangeID As String = BsRangeIDLabel.Text
                        ValidateRefRangesNotOverlapped(myOriginalRangeID)
                        If ValidationError Then Exit Sub

                        selRow = bsRefDetailDataGridView.SelectedRows(0).Index
                    End If

                    If (isNew) Then
                        ValidateRefRangesNotOverlapped()
                        If ValidationError Then Exit Sub

                        If (EqualRangeIndex = -1) Then 'AG 02/11/2010 - If exists but not saved (new) ... add row
                            selRow = bsRefDetailDataGridView.Rows.Add()
                            bsRefDetailDataGridView.Rows(selRow).Cells("RangeID").Value = -bsRefDetailDataGridView.Rows.Count
                            bsRefDetailDataGridView.Rows(selRow).Cells("Added").Value = True

                            If (bsRefDetailDataGridView.SelectedRows.Count > 0) Then
                                bsRefDetailDataGridView.SelectedRows(0).Selected = False
                            End If
                        End If
                    End If

                    If (bsRefDetailDataGridView.Rows.Count > 0) Then
                        bsRefDetailDataGridView.Rows(selRow).Selected = True
                    End If

                    If (bsRefDetailDataGridView.Rows.Count > 0 And EqualRangeIndex > -1) Then
                        selRow = bsRefDetailDataGridView.Rows(EqualRangeIndex).Index
                    End If

                    If (selRow >= 0) Then
                        'Cells always informed
                        bsRefDetailDataGridView.Rows(selRow).Cells("SampleType").Value = SelectedSampleType
                        bsRefDetailDataGridView.Rows(selRow).Cells("NormalLowerLimit").Value = CSng(bsRefDetailLowerUpDown.Text)
                        bsRefDetailDataGridView.Rows(selRow).Cells("NormalUpperLimit").Value = CSng(bsRefDetailUpperUpDown.Text)

                        If (bsRefDetailGenderCheckBox.Checked And bsRefDetailAgeCheckBox.Checked) Then
                            'Cells informed when Ranges are defined by Gender and Age
                            bsRefDetailDataGridView.Rows(selRow).Cells("Gender").Value = bsRefDetailGenderComboBox.SelectedValue.ToString
                            bsRefDetailDataGridView.Rows(selRow).Cells("GenderDesc").Value = bsRefDetailGenderComboBox.Text.ToString

                            bsRefDetailDataGridView.Rows(selRow).Cells("AgeUnit").Value = bsRefDetailAgeComboBox.SelectedValue.ToString
                            bsRefDetailDataGridView.Rows(selRow).Cells("AgeUnitDesc").Value = bsRefDetailAgeComboBox.Text   '.ToUpper
                            bsRefDetailDataGridView.Rows(selRow).Cells("AgeRangeFrom").Value = CInt(bsRefDetailAgeFromUpDown.Text)
                            bsRefDetailDataGridView.Rows(selRow).Cells("AgeRangeTo").Value = CInt(bsRefDetailAgeToUpDown.Text)

                        ElseIf (bsRefDetailGenderCheckBox.Checked And Not bsRefDetailAgeCheckBox.Checked) Then
                            'Cells informed when Ranges are defined only by Gender
                            bsRefDetailDataGridView.Rows(selRow).Cells("Gender").Value = bsRefDetailGenderComboBox.SelectedValue.ToString
                            bsRefDetailDataGridView.Rows(selRow).Cells("GenderDesc").Value = bsRefDetailGenderComboBox.Text.ToString

                            bsRefDetailDataGridView.Rows(selRow).Cells("AgeUnit").Value = Nothing
                            bsRefDetailDataGridView.Rows(selRow).Cells("AgeUnitDesc").Value = Nothing
                            bsRefDetailDataGridView.Rows(selRow).Cells("AgeRangeFrom").Value = Nothing
                            bsRefDetailDataGridView.Rows(selRow).Cells("AgeRangeTo").Value = Nothing

                        ElseIf (bsRefDetailAgeCheckBox.Checked And Not bsRefDetailGenderCheckBox.Checked) Then
                            'Cells informed when Ranges are defined only by Age
                            bsRefDetailDataGridView.Rows(selRow).Cells("AgeUnit").Value = bsRefDetailAgeComboBox.SelectedValue.ToString
                            bsRefDetailDataGridView.Rows(selRow).Cells("AgeUnitDesc").Value = bsRefDetailAgeComboBox.Text   '.ToUpper
                            bsRefDetailDataGridView.Rows(selRow).Cells("AgeRangeFrom").Value = CInt(bsRefDetailAgeFromUpDown.Text)
                            bsRefDetailDataGridView.Rows(selRow).Cells("AgeRangeTo").Value = CInt(bsRefDetailAgeToUpDown.Text)

                            bsRefDetailDataGridView.Rows(selRow).Cells("Gender").Value = Nothing
                            bsRefDetailDataGridView.Rows(selRow).Cells("GenderDesc").Value = Nothing
                        End If


                        bsRefDetailAgeFromUpDown.Value = bsRefDetailAgeFromUpDown.Minimum : bsRefDetailAgeFromUpDown.ResetText()
                        bsRefDetailAgeToUpDown.Value = bsRefDetailAgeToUpDown.Minimum : bsRefDetailAgeToUpDown.ResetText()

                        bsRefDetailLowerUpDown.Value = bsRefDetailLowerUpDown.Minimum : bsRefDetailLowerUpDown.ResetText()
                        bsRefDetailUpperUpDown.Value = bsRefDetailUpperUpDown.Minimum : bsRefDetailUpperUpDown.ResetText()
                    End If
                End If
                IsDetailedRangeEditing = False
            Else
                IsDetailedRangeEditing = True
            End If

            BsRangeIDLabel.Text = ""
            If (selRow < 0) Then
                RefDetailDataGridUnselect()
            Else
                If (Not ChangeSampleTypeDuringEdition) Then ChangesMade = True 'AG 27/10/2010
            End If
        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    ''' <summary>
    ''' Fills the Detailed Range controls with data of the Range selected in the DataGrid
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SG 01/09/2010
    ''' </remarks>
    Private Sub RefDetailEdit()
        Dim selRow As Integer = -1

        Try
            If (bsRefDetailDataGridView.SelectedRows.Count > 0) Then
                selRow = bsRefDetailDataGridView.SelectedRows(0).Index
            End If

            If (selRow > -1) Then
                bsRefDetailLowerUpDown.Text = bsRefDetailDataGridView.Rows(selRow).Cells("NormalLowerLimit").Value.ToString
                bsRefDetailUpperUpDown.Text = bsRefDetailDataGridView.Rows(selRow).Cells("NormalUpperLimit").Value.ToString

                If (bsRefDetailDataGridView.Rows(selRow).Cells("Gender").Value IsNot Nothing) And _
                   (bsRefDetailDataGridView.Rows(selRow).Cells("AgeUnit").Value IsNot Nothing) Then

                    'The Range is defined by Gender and Age
                    bsRefDetailGenderCheckBox.Enabled = True
                    bsRefDetailGenderComboBox.SelectedValue = bsRefDetailDataGridView.Rows(selRow).Cells("Gender").Value.ToString

                    bsRefDetailAgeCheckBox.Enabled = True
                    bsRefDetailAgeComboBox.SelectedValue = bsRefDetailDataGridView.Rows(selRow).Cells("AgeUnit").Value.ToString
                    bsRefDetailAgeFromUpDown.Text = bsRefDetailDataGridView.Rows(selRow).Cells("AgeRangeFrom").Value.ToString
                    bsRefDetailAgeToUpDown.Text = bsRefDetailDataGridView.Rows(selRow).Cells("AgeRangeTo").Value.ToString

                ElseIf (bsRefDetailDataGridView.Rows(selRow).Cells("Gender").Value IsNot Nothing) And _
                       (bsRefDetailDataGridView.Rows(selRow).Cells("AgeUnit").Value Is Nothing) Then
                    'The Range is defined only by Gender
                    bsRefDetailGenderCheckBox.Enabled = True
                    bsRefDetailGenderComboBox.SelectedValue = bsRefDetailDataGridView.Rows(selRow).Cells("Gender").Value.ToString

                    bsRefDetailAgeCheckBox.Enabled = False
                    bsRefDetailAgeComboBox.SelectedIndex = -1
                    bsRefDetailAgeFromUpDown.ResetText()
                    bsRefDetailAgeToUpDown.ResetText()

                ElseIf (bsRefDetailDataGridView.Rows(selRow).Cells("AgeUnit").Value IsNot Nothing) And _
                       (bsRefDetailDataGridView.Rows(selRow).Cells("Gender").Value Is Nothing) Then
                    'The Range is defined only by Age
                    bsRefDetailAgeCheckBox.Enabled = True
                    bsRefDetailAgeComboBox.SelectedValue = bsRefDetailDataGridView.Rows(selRow).Cells("AgeUnit").Value.ToString
                    bsRefDetailAgeFromUpDown.Text = bsRefDetailDataGridView.Rows(selRow).Cells("AgeRangeFrom").Value.ToString
                    bsRefDetailAgeToUpDown.Text = bsRefDetailDataGridView.Rows(selRow).Cells("AgeRangeTo").Value.ToString

                    bsRefDetailGenderCheckBox.Enabled = False
                    bsRefDetailGenderComboBox.SelectedIndex = -1
                End If

                BsRangeIDLabel.Text = bsRefDetailDataGridView.Rows(selRow).Cells("RangeID").Value.ToString

                RefDetailCheckBoxesManagement()
                RefDetailAgeLimitsSetUp()
                IsDetailedRangeEditing = True
            End If
        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    ''' <summary>
    ''' Deletes the Detailed Range selected on the DataGrid
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SG 01/09/2010
    ''' </remarks>
    Private Sub RefDetailDelete()
        Try
            For Each m_row As System.Windows.Forms.DataGridViewRow In bsRefDetailDataGridView.SelectedRows
                bsRefDetailDataGridView.Rows.Remove(m_row)
                If Not ChangeSampleTypeDuringEdition Then ChangesMade = True 'AG 27/10/2010
            Next
        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    ''' <summary>
    ''' Resets the data in the Detailed Range controls
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SG 01/09/2010
    ''' Modified by: SA 18/11/2010
    ''' </remarks>
    Private Sub RefDetailResetValues()
        Try
            If (bsRefDetailGenderCheckBox.Checked) Then
                bsRefDetailGenderComboBox.SelectedIndex = 0
            Else
                bsRefDetailGenderComboBox.SelectedIndex = -1
            End If

            If (bsRefDetailAgeCheckBox.Checked) Then
                bsRefDetailAgeComboBox.SelectedIndex = 0
            Else
                bsRefDetailAgeComboBox.SelectedIndex = -1
            End If
            'If (bsRefDetailGenderComboBox.Items.Count > 0) Then
            '    bsRefDetailGenderComboBox.SelectedIndex = 0
            'End If
            'If (bsRefDetailAgeComboBox.Items.Count > 0) Then
            '    bsRefDetailAgeComboBox.SelectedIndex = 0
            'End If
            bsRefDetailAgeFromUpDown.Value = bsRefDetailAgeFromUpDown.Minimum : bsRefDetailAgeFromUpDown.ResetText()
            bsRefDetailAgeToUpDown.Value = bsRefDetailAgeToUpDown.Minimum : bsRefDetailAgeToUpDown.ResetText()
            bsRefDetailLowerUpDown.Value = bsRefDetailLowerUpDown.Minimum : bsRefDetailLowerUpDown.ResetText()
            bsRefDetailUpperUpDown.Value = bsRefDetailUpperUpDown.Minimum : bsRefDetailUpperUpDown.ResetText()
        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    ''' <summary>
    ''' Unselect all rows in the DataGrid of Detailed Reference Ranges updating also the style of them
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SG 01/09/2010
    ''' Modified by: SA 18/11/2010 - When the Grid is not enabled, set BackColor to MenuBar
    ''' </remarks>
    Private Sub RefDetailDataGridUnselect()
        Try
            If (bsRefDetailDataGridView.Rows.Count > 0) Then
                For r As Integer = 0 To bsRefDetailDataGridView.Rows.Count - 1 Step 1
                    bsRefDetailDataGridView.Rows(r).Selected = False

                    If (bsRefDetailDataGridView.Enabled) Then
                        bsRefDetailDataGridView.Rows(r).DefaultCellStyle.BackColor = Color.White
                    Else
                        bsRefDetailDataGridView.Rows(r).DefaultCellStyle.BackColor = SystemColors.MenuBar
                    End If
                    bsRefDetailDataGridView.Rows(r).DefaultCellStyle.ForeColor = Color.Black
                Next
            End If
        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    ''' <summary>
    ''' Checks if the editing Detailed Range Type is different from the saved ones
    ''' </summary>
    ''' <returns>True/False</returns>
    ''' <remarks>
    ''' Created by:  SG 01/09/2010
    ''' </remarks>
    Private Function RefDetailRangePatternIsDifferent(ByVal pSampleType As String) As Boolean
        Try
            If (bsRefDetailDataGridView.Rows.Count > 0 AndAlso RefGetGridDetailedType(pSampleType) <> "NONE") Then
                If (bsRefDetailGenderCheckBox.Checked And bsRefDetailAgeCheckBox.Checked And _
                    RefGetGridDetailedType(pSampleType) <> "GENDER_AGE") Then
                    Return True

                ElseIf (bsRefDetailGenderCheckBox.Checked And Not bsRefDetailAgeCheckBox.Checked And _
                        RefGetGridDetailedType(pSampleType) <> "GENDER") Then
                    Return True

                ElseIf (Not bsRefDetailGenderCheckBox.Checked And bsRefDetailAgeCheckBox.Checked And _
                        RefGetGridDetailedType(pSampleType) <> "AGE") Then
                    Return True
                End If
            Else
                Return False
            End If
        Catch ex As Exception
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Checks if the editing Reference Range is already saved
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SG 01/09/2010
    ''' </remarks>
    Private Function RefDetailRangeIsNew(ByVal pSampleType As String) As Integer
        Try
            Dim index As Integer = RefCheckIfRangeExistsInGrid(pSampleType)
            If (index >= 0) Then
                If (bsRefDetailDataGridView.Rows.Count >= index) Then
                    Return index
                End If
            Else
                Return -1
            End If
        Catch ex As Exception
            Return -1
        End Try
    End Function

    ''' <summary>
    ''' Manages the availability of add, edit and delete buttons
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SG 01/09/2010
    ''' </remarks>
    Private Sub RefDetailAddEditEnabling()
        Try
            If (EditionModeAttribute And bsRefDetailedRadioButton.Checked) Then
                bsRefDetailAddButton.Enabled = Not (bsRefDetailDataGridView.SelectedRows.Count > 1)
                bsRefDetailEditButton.Enabled = Not (bsRefDetailDataGridView.SelectedRows.Count > 1)
            End If
        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    ''' <summary>
    ''' Gets the current Detailed Range Type defined in the DataGrid
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SG 01/09/2010
    ''' Modified by: XB 04/02/2013 - Upper conversions redundants because the value is already in UpperCase must delete to avoid Regional Settings problems (Bugs tracking #1112)
    ''' </remarks>
    Private Function RefGetGridDetailedType(ByVal pSampleType As String) As String
        Dim type As String = "NONE"

        Try
            If (bsRefDetailDataGridView.Rows.Count > 0) Then
                For Each dgr As DataGridViewRow In bsRefDetailDataGridView.Rows
                    If (dgr.Visible) Then
                        'If (dgr.Cells("SampleType").Value.ToString.ToUpper = pSampleType.ToUpper()) Then
                        If (dgr.Cells("SampleType").Value.ToString = pSampleType) Then
                            If (dgr.Cells("Gender").Value IsNot Nothing And dgr.Cells("AgeUnit").Value IsNot Nothing) Then
                                type = "GENDER_AGE"
                            ElseIf (dgr.Cells("Gender").Value IsNot Nothing) Then
                                type = "GENDER"
                            ElseIf (dgr.Cells("AgeUnit").Value IsNot Nothing) Then
                                type = "AGE"
                            Else
                                type = "NONE"
                            End If
                            Exit For
                        Else
                            type = "NONE"
                        End If
                    End If
                Next
            Else
                type = "NONE"
            End If
            Return type
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    ''' <summary>
    ''' Checks if there is any Detailed Reference Range for the specified SampleType loaded in the grid
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SG 01/09/2010
    ''' Modified by: XB 04/02/2013 - Upper conversions redundants because the value is already in UpperCase must delete to avoid Regional Settings problems (Bugs tracking #1112)
    ''' </remarks>
    Private Function RefCheckIfRangeExistsInGrid(ByVal pSampleType As String) As Integer
        Dim myGridType As String = ""
        Dim myGender As String = bsRefDetailGenderComboBox.Text
        Dim myAgeUnit As String = bsRefDetailAgeComboBox.Text
        Dim myAgeFrom As String = bsRefDetailAgeFromUpDown.Text
        Dim myAgeTo As String = bsRefDetailAgeToUpDown.Text
        Dim myLowerLimit As String = bsRefDetailLowerUpDown.Text
        Dim myUpperLimit As String = bsRefDetailUpperUpDown.Text

        Try
            If (bsRefDetailDataGridView.Rows.Count > 0) Then
                myGridType = RefGetGridDetailedType(pSampleType)

                Select Case (myGridType)
                    Case "GENDER"
                        For Each dgr As DataGridViewRow In bsRefDetailDataGridView.Rows
                            'If (dgr.Visible And RefGetDataFromCell(dgr, "SampleType").ToUpper = pSampleType.ToUpper) Then
                            If (dgr.Visible And RefGetDataFromCell(dgr, "SampleType") = pSampleType) Then
                                'If (myGender.ToUpper = RefGetDataFromCell(dgr, "Gender").ToUpper) Then
                                If (myGender = RefGetDataFromCell(dgr, "Gender")) Then
                                    Return dgr.Index
                                End If
                            End If
                        Next
                        Return -1

                    Case "AGE"
                        For Each dgr As DataGridViewRow In bsRefDetailDataGridView.Rows
                            'If (dgr.Visible And RefGetDataFromCell(dgr, "SampleType").ToUpper = pSampleType.ToUpper) Then
                            '    If (myAgeUnit.ToUpper = RefGetDataFromCell(dgr, "AgeUnit").ToUpper And _
                            '        myAgeFrom = RefGetDataFromCell(dgr, "AgeRangeFrom").ToUpper And _
                            '        myAgeTo = RefGetDataFromCell(dgr, "AgeRangeTo").ToUpper) Then
                            '        Return dgr.Index
                            '    End If
                            'End If
                            If (dgr.Visible And RefGetDataFromCell(dgr, "SampleType") = pSampleType) Then
                                If (myAgeUnit = RefGetDataFromCell(dgr, "AgeUnit") And _
                                    myAgeFrom = RefGetDataFromCell(dgr, "AgeRangeFrom") And _
                                    myAgeTo = RefGetDataFromCell(dgr, "AgeRangeTo")) Then
                                    Return dgr.Index
                                End If
                            End If
                        Next
                        Return -1

                    Case "GENDER_AGE"
                        For Each dgr As DataGridViewRow In bsRefDetailDataGridView.Rows
                            'If (dgr.Visible And RefGetDataFromCell(dgr, "SampleType").ToUpper = pSampleType.ToUpper) Then
                            '    If (myGender.ToUpper = dgr.Cells("Gender").Value.ToString.ToUpper And _
                            '        myAgeUnit.ToUpper = RefGetDataFromCell(dgr, "AgeUnit").ToUpper And _
                            '        myAgeFrom = RefGetDataFromCell(dgr, "AgeRangeFrom").ToUpper And _
                            '        myAgeTo = RefGetDataFromCell(dgr, "AgeRangeTo").ToUpper) Then
                            '        Return dgr.Index
                            '    End If
                            'End If
                            If (dgr.Visible And RefGetDataFromCell(dgr, "SampleType") = pSampleType) Then
                                If (myGender = dgr.Cells("Gender").Value.ToString And _
                                    myAgeUnit = RefGetDataFromCell(dgr, "AgeUnit") And _
                                    myAgeFrom = RefGetDataFromCell(dgr, "AgeRangeFrom") And _
                                    myAgeTo = RefGetDataFromCell(dgr, "AgeRangeTo")) Then
                                    Return dgr.Index
                                End If
                            End If
                        Next
                        Return -1

                    Case Else
                        Return -1
                End Select
            Else
                Return -1
            End If
        Catch ex As Exception
            Throw ex
            Return -1
        End Try
    End Function

    ''' <summary>
    ''' Counts the number of Detailed Reference Ranges of the specified Range Type
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SG 01/09/2010
    ''' Modified by: XB 04/02/2013 - Upper conversions redundants because the value is already in UpperCase must delete to avoid Regional Settings problems (Bugs tracking #1112)
    ''' </remarks>
    Private Function RefCountDetailRowsBySampleType(ByVal pSampleType As String) As Integer
        Dim count As Integer = 0
        Try
            For Each dgr As DataGridViewRow In bsRefDetailDataGridView.Rows
                'If (RefGetDataFromCell(dgr, "SampleType").ToUpper = pSampleType.ToUpper) Then
                If (RefGetDataFromCell(dgr, "SampleType") = pSampleType) Then
                    count = count + 1
                End If
            Next
            Return count
        Catch ex As Exception
            Throw ex
            Return -1
        End Try
    End Function

    ''' <summary>
    ''' Writes the ranges 'deleted' 
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SG 01/09/2010
    ''' </remarks>
    Private Function RefRemoveDetailedRefRange(ByVal pSampletype As String) As Boolean
        Try
            Dim qAllTestRefRanges As New List(Of TestRefRangesDS.tparTestRefRangesRow)
            qAllTestRefRanges = (From a In SelectedTestRefRangesDS.tparTestRefRanges _
                                Where a.SampleType = pSampletype And a.IsDeleted = False _
                                  And a.RangeType = "DETAILED" _
                               Select a).ToList()

            If (qAllTestRefRanges.Count > 0) Then
                For Each r As TestRefRangesDS.tparTestRefRangesRow In qAllTestRefRanges
                    Dim existsInGrid As Boolean = False
                    For Each dgr As DataGridViewRow In bsRefDetailDataGridView.Rows
                        Dim gridRangeID As Integer = CInt(RefGetDataFromCell(dgr, "RangeID"))
                        If (r.RangeType = "DETAILED") Then
                            If (r.RangeID = gridRangeID) Then
                                existsInGrid = True
                                Exit For
                            End If
                        End If
                    Next
                    If (Not existsInGrid) Then
                        r.IsNew = False
                        r.IsDeleted = True
                    End If
                Next
                Return False
            Else
                Return False
            End If
        Catch ex As Exception
            Throw ex
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Add all defined Reference Ranges to the final DataSet SelectedTestRefRangesDS 
    ''' </summary>
    ''' <remarks>
    ''' Modified by: SA 18/11/2010 - For Detailed Ranges, save in the temporal DataSet also the description
    '''                              for fields Gender and Age Unit
    ''' </remarks>
    Public Sub CreateNewTestRefRanges(ByVal pTestID As Integer, ByVal pSampleType As String)
        Try
            Dim isNew As Boolean = False
            Dim qTestRefRanges As New List(Of TestRefRangesDS.tparTestRefRangesRow)

            If (SelectedTestRefRangesDS Is Nothing) Then SelectedTestRefRangesDS = New TestRefRangesDS

            If (SelectedTestRefRangesDS.tparTestRefRanges.Rows.Count > 0) Then
                qTestRefRanges = (From a In SelectedTestRefRangesDS.tparTestRefRanges _
                                 Where a.TestID = pTestID And a.SampleType = pSampleType _
                                Select a).ToList()
                If (qTestRefRanges.Count = 0) Then
                    isNew = True
                End If
            Else
                isNew = True
            End If

            Dim myTestType As String = "STD"
            If (TestType = TestTypes.STANDARD) Then
                myTestType = "STD"
            ElseIf (TestType = TestTypes.ISE) Then
                myTestType = "ISE"
            ElseIf (TestType = TestTypes.CALCULATED) Then
                myTestType = "CALC"
            ElseIf (TestType = TestTypes.OFFSYSTEM) Then
                myTestType = "OFFS"
            End If

            If (isNew) Then
                If (IsReferenceRangesDefined(pSampleType)) Then
                    Dim testRefRangesRow As TestRefRangesDS.tparTestRefRangesRow
                    testRefRangesRow = SelectedTestRefRangesDS.tparTestRefRanges.NewtparTestRefRangesRow()
                    'TR 30/11/2010 -Validate the normar lower limits values before adding de generic range
                    If (bsRefNormalLowerLimitUpDown.Text <> "" AndAlso bsRefNormalUpperLimitUpDown.Text <> "") Then
                        qTestRefRanges.Add(testRefRangesRow)
                        qTestRefRanges.Last().TestID = pTestID
                        qTestRefRanges.Last().SampleType = pSampleType
                        qTestRefRanges.Last().IsNew = True
                        qTestRefRanges.Last().TS_DateTime = DateTime.Now
                        qTestRefRanges.Last().RangeType = "GENERIC"
                        qTestRefRanges.Last().RangeID = -999

                        'NormalLowerLimit
                        If (bsRefNormalLowerLimitUpDown.Text <> "") Then
                            qTestRefRanges.Last().NormalLowerLimit = CType(bsRefNormalLowerLimitUpDown.Text, Single)
                        Else
                            qTestRefRanges.Last().SetNormalLowerLimitNull()
                        End If

                        'NormalUpperLimit
                        If (bsRefNormalUpperLimitUpDown.Text <> "") Then
                            qTestRefRanges.Last().NormalUpperLimit = CType(bsRefNormalUpperLimitUpDown.Text, Single)
                        Else
                            qTestRefRanges.Last().SetNormalUpperLimitNull()
                        End If

                        'BorderLineLowerLimit
                        If (bsRefBorderLineLowerLimitUpDown.Text <> "") Then
                            qTestRefRanges.Last().BorderLineLowerLimit = CType(bsRefBorderLineLowerLimitUpDown.Text, Single)
                        Else
                            qTestRefRanges.Last().SetBorderLineLowerLimitNull()
                        End If

                        'BorderLineUpperLimit
                        If (bsRefBorderLineUpperLimitUpDown.Text <> "") Then
                            qTestRefRanges.Last().BorderLineUpperLimit = CType(bsRefBorderLineUpperLimitUpDown.Text, Single)
                        Else
                            qTestRefRanges.Last().SetBorderLineUpperLimitNull()
                        End If

                        'Fields for Detailed Reference Ranges are set to null
                        qTestRefRanges.Last().SetGenderNull()
                        qTestRefRanges.Last().SetAgeUnitNull()
                        qTestRefRanges.Last().SetAgeRangeFromNull()
                        qTestRefRanges.Last().SetAgeRangeToNull()

                        'Validate the row state to see if it's a new or modified row
                        If (qTestRefRanges.Last().RowState = DataRowState.Detached) Then
                            qTestRefRanges.Last.TestType = myTestType 'AG 26/10/2010
                            SelectedTestRefRangesDS.tparTestRefRanges.AddtparTestRefRangesRow(qTestRefRanges.Last())
                        End If
                        SelectedTestRefRangesDS.tparTestRefRanges.AcceptChanges()

                    End If

                    Dim CurrentDetailedType As String = RefGetGridDetailedType(pSampleType)
                    For r As Integer = 0 To bsRefDetailDataGridView.Rows.Count - 1 Step 1
                        testRefRangesRow = Nothing
                        testRefRangesRow = SelectedTestRefRangesDS.tparTestRefRanges.NewtparTestRefRangesRow()
                        qTestRefRanges.Add(testRefRangesRow)

                        qTestRefRanges.Last().TestID = pTestID
                        qTestRefRanges.Last().SampleType = pSampleType
                        qTestRefRanges.Last().IsNew = True
                        qTestRefRanges.Last().IsDeleted = False
                        qTestRefRanges.Last().TS_DateTime = DateTime.Now
                        qTestRefRanges.Last().RangeType = "DETAILED"
                        qTestRefRanges.Last().RangeID = -(r + 1)

                        'DetailLowerLimit
                        If (bsRefDetailDataGridView.Rows(r).Cells("NormalLowerLimit").Value IsNot Nothing) Then
                            qTestRefRanges.Last().NormalLowerLimit = CType(bsRefDetailDataGridView.Rows(r).Cells("NormalLowerLimit").Value, Single)
                        Else
                            qTestRefRanges.Last().SetNormalLowerLimitNull()
                        End If

                        'DetailUpperLimit
                        If (bsRefDetailDataGridView.Rows(r).Cells("NormalUpperLimit").Value IsNot Nothing) Then
                            qTestRefRanges.Last().NormalUpperLimit = CType(bsRefDetailDataGridView.Rows(r).Cells("NormalUpperLimit").Value, Single)
                        Else
                            qTestRefRanges.Last().SetNormalUpperLimitNull()
                        End If

                        'Gender
                        If (CurrentDetailedType = "GENDER" Or CurrentDetailedType = "GENDER_AGE") Then
                            If (bsRefDetailDataGridView.Rows(r).Cells("Gender").Value IsNot Nothing) Then
                                qTestRefRanges.Last().Gender = CType(bsRefDetailDataGridView.Rows(r).Cells("Gender").Value, String).First
                                qTestRefRanges.Last().GenderDesc = CType(bsRefDetailDataGridView.Rows(r).Cells("GenderDesc").Value, String).First
                            Else
                                qTestRefRanges.Last().SetGenderNull()
                            End If
                        Else
                            'Gender fields are set to null
                            qTestRefRanges.Last().SetGenderNull()
                        End If

                        'AgeUnit
                        If (CurrentDetailedType = "AGE" Or CurrentDetailedType = "GENDER_AGE") Then
                            If (bsRefDetailDataGridView.Rows(r).Cells("AgeUnit").Value IsNot Nothing) Then
                                qTestRefRanges.Last().AgeUnit = CType(bsRefDetailDataGridView.Rows(r).Cells("AgeUnit").Value, String)
                                qTestRefRanges.Last().AgeUnitDesc = CType(bsRefDetailDataGridView.Rows(r).Cells("AgeUnitDesc").Value, String)
                            Else
                                qTestRefRanges.Last().SetAgeUnitNull()
                            End If

                            'AgeFrom
                            If (bsRefDetailDataGridView.Rows(r).Cells("AgeRangeFrom").Value IsNot Nothing) Then
                                qTestRefRanges.Last().AgeRangeFrom = CType(bsRefDetailDataGridView.Rows(r).Cells("AgeRangeFrom").Value, Integer)
                            Else
                                qTestRefRanges.Last().SetAgeRangeFromNull()
                            End If

                            'AgeTo
                            If (bsRefDetailDataGridView.Rows(r).Cells("AgeRangeTo").Value IsNot Nothing) Then
                                qTestRefRanges.Last().AgeRangeTo = CType(bsRefDetailDataGridView.Rows(r).Cells("AgeRangeTo").Value, Integer)
                            Else
                                qTestRefRanges.Last().SetAgeRangeToNull()
                            End If
                        Else
                            'Age fields are set to null
                            qTestRefRanges.Last().SetAgeUnitNull()
                            qTestRefRanges.Last().SetAgeRangeFromNull()
                            qTestRefRanges.Last().SetAgeRangeToNull()
                        End If

                        bsRefDetailDataGridView.Rows(r).Cells("Added").Value = False

                        'Fields for Generic Reference Ranges are set to null
                        qTestRefRanges.Last().SetBorderLineLowerLimitNull()
                        qTestRefRanges.Last().SetBorderLineUpperLimitNull()

                        'Validate the row state to see if it's a new or modified row
                        If qTestRefRanges.Last().RowState = DataRowState.Detached Then
                            qTestRefRanges.Last.TestType = myTestType 'AG 26/10/2010
                            SelectedTestRefRangesDS.tparTestRefRanges.AddtparTestRefRangesRow(qTestRefRanges.Last())
                        End If

                        SelectedTestRefRangesDS.tparTestRefRanges.AcceptChanges()
                    Next r
                End If
            End If

        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    ''' <summary>
    ''' Verify the range values informed in the specified controls are correct
    ''' </summary>
    ''' <param name="pLimitUpdown1">Minimum limit BSNumericUpDown to evaluate</param>
    ''' <param name="pLimitUpdown2">Maximum limit BSNumericUpDown to evaluate</param>
    ''' <param name="pMandatory">Indicate if the specified controls are mandatory</param>
    ''' <remarks>
    ''' Created by:  SG 01/09/2010
    ''' </remarks>
    Private Sub ValidateLimitsUpDown(ByRef pLimitUpdown1 As BSNumericUpDown, ByRef pLimitUpdown2 As BSNumericUpDown, ByVal pMandatory As Boolean)

        Try
            If (pLimitUpdown1.Text = "0" And pLimitUpdown2.Text = "0") Then
                pLimitUpdown1.Text = ""
                pLimitUpdown2.Text = ""
            End If

            If (pMandatory) Then
                If (pLimitUpdown1.Text = "" And pLimitUpdown2.Text = "") Then
                    'Error --> Required  values not informed...
                    bsControlErrorProvider.SetError(pLimitUpdown1, GetMessageText(RefMessages.REQUIRED_VALUE.ToString)) 'AG 07/07/2010("REQUIRED_VALUE"))
                    bsControlErrorProvider.SetError(pLimitUpdown2, GetMessageText(RefMessages.REQUIRED_VALUE.ToString)) 'AG 07/07/2010("REQUIRED_VALUE"))
                    ValidationError = True
                End If
            End If

            If (pLimitUpdown1.Text <> "" Or pLimitUpdown2.Text <> "") Then
                If (pLimitUpdown1.Text = "" And pLimitUpdown2.Text <> "") Then
                    'Error --> Lower limit not informed
                    bsControlErrorProvider.SetError(pLimitUpdown1, GetMessageText(RefMessages.REQUIRED_VALUE.ToString)) 'AG 07/07/2010("REQUIRED_VALUE"))
                    ValidationError = True
                End If

                If (pLimitUpdown2.Text = "" And pLimitUpdown1.Text <> "") Then
                    'Error --> Upper limit not informed
                    bsControlErrorProvider.SetError(pLimitUpdown2, GetMessageText(RefMessages.REQUIRED_VALUE.ToString)) 'AG 07/07/2010("REQUIRED_VALUE"))
                    ValidationError = True
                End If

                If (pLimitUpdown2.Text <> "" And pLimitUpdown1.Text <> "") Then
                    If (CDbl(pLimitUpdown1.Text) > CDbl(pLimitUpdown2.Text)) Then
                        'Error --> Lower limit greater than the Upper one
                        bsControlErrorProvider.SetError(pLimitUpdown1, GetMessageText(RefMessages.MIN_MUST_BE_LOWER_THAN_MAX.ToString)) 'AG 07/07/2010("MIN_MUST_BE_LOWER_THAN_MAX"))
                        ValidationError = True
                    End If
                    If (CDbl(pLimitUpdown1.Text) = CDbl(pLimitUpdown2.Text)) Then
                        'Error --> Lower limit is equal to Upper limit
                        bsControlErrorProvider.SetError(pLimitUpdown1, GetMessageText(RefMessages.MIN_MUST_BE_LOWER_THAN_MAX.ToString)) 'AG 07/07/2010("MIN_MUST_BE_LOWER_THAN_MAX"))
                        ValidationError = True
                    End If
                End If
            End If
        Catch ex As Exception
            Throw ex
        End Try
    End Sub
#End Region

#Region "Auxiliary methods"

    ''' <summary>
    ''' Auxiliary function to get the data contained in the specified cell of the DataGrid
    ''' contained the Detailed Reference Ranges that have been defined
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SG 01/09/2010
    ''' </remarks>
    Private Function RefGetDataFromCell(ByVal dgr As DataGridViewRow, ByVal col As String) As String
        Try
            If dgr.Cells(col).Value IsNot Nothing Then
                Return dgr.Cells(col).Value.ToString
            Else
                Return ""
            End If
        Catch ex As Exception
            Throw ex
            Return ""
        End Try
    End Function

    ''' <summary>
    ''' Auxiliary function that returns the multilanguage text for the specified Message Identifier
    ''' </summary>
    ''' <param name="pMessageId">Message Identifier</param>
    ''' <remarks>
    ''' Created by:  SG 01/09/2010
    ''' Modified by: XB 04/02/2013 - Upper conversions redundants because the value is already in UpperCase must delete to avoid Regional Settings problems (Bugs tracking #1112)
    ''' </remarks>
    Private Function GetMessageText(ByVal pMessageId As String) As String
        Dim textMessage As String = String.Empty
        Try
            Dim qMessages As New List(Of MessagesDS.tfmwMessagesRow)
            qMessages = (From a In AllMessagesDS.tfmwMessages _
                        Where a.MessageID = pMessageId _
                       Select a).ToList()
            'Where a.MessageID.ToUpper = pMessageId.ToUpper _
            If (qMessages.Count > 0) Then textMessage = qMessages.First.MessageText
        Catch ex As Exception
            Throw ex
        End Try
        Return textMessage
    End Function

    ''' <summary>
    ''' Auxiliary function that returns the type (Error, Warning, Information) of the specified Message Identifier
    ''' </summary>
    ''' <param name="pMessageId">Message Identifier</param>
    ''' <remarks>
    ''' Created by:  SG 01/09/2010
    ''' Modified by: XB 04/02/2013 - Upper conversions redundants because the value is already in UpperCase must delete to avoid Regional Settings problems (Bugs tracking #1112)
    ''' </remarks>
    Private Function GetMessageType(ByVal pMessageId As String) As String
        Dim textMessage As String = String.Empty
        Try
            Dim qMessages As New List(Of MessagesDS.tfmwMessagesRow)
            qMessages = (From a In AllMessagesDS.tfmwMessages _
                        Where a.MessageID = pMessageId _
                       Select a).ToList()
            'Where a.MessageID.ToUpper = pMessageId.ToUpper _
            If (qMessages.Count > 0) Then textMessage = qMessages.First.MessageType
        Catch ex As Exception
            Throw ex
        End Try
        Return textMessage
    End Function

    ''' <summary>
    ''' Auxiliary function used to shown Messages that have to be shown as a Dialog
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SG 01/09/2010
    ''' </remarks>
    Public Function ShowMessage(ByVal pWindowTitle As String, ByVal pMessageID As String, Optional ByVal pSystemMessageText As String = "", _
                                Optional ByVal pOwnerWindow As System.Windows.Forms.IWin32Window = Nothing) As DialogResult
        Dim result As New DialogResult

        Try
            result = Windows.Forms.DialogResult.No
            If (AllMessagesDS.tfmwMessages.Rows.Count > 0) Then
                Dim msgText As String = GetMessageText(pMessageID)
                Dim msgType As String = GetMessageType(pMessageID)

                'Additional text to add to the Message Text
                If (pSystemMessageText <> "") Then msgText = msgText & " - " & pSystemMessageText

                'Window Owner...if it has not been informed, then the owner is the Parent Form
                'AG 28/07/2010
                If (pOwnerWindow Is Nothing) Then pOwnerWindow = ParentForm 'Ax00MainMDI

                'Show message with the proper icon according the Message Type
                If (msgType = "Error") Then
                    'Error Message 
                    result = MessageBox.Show(pOwnerWindow, msgText, pWindowTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)

                ElseIf (msgType = "Information") Then
                    'Information Message 
                    result = MessageBox.Show(pOwnerWindow, msgText, pWindowTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)

                ElseIf (msgType = "FailureAudit") Then
                    'System Error Message - FailureAudit
                    result = MessageBox.Show(pOwnerWindow, msgText, pWindowTitle, MessageBoxButtons.OK, MessageBoxIcon.Stop)

                ElseIf (msgType = "Warning") Then
                    'System Error Message - Warning
                    result = MessageBox.Show(pOwnerWindow, msgText, pWindowTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)

                Else
                    'Question Message
                    result = MessageBox.Show(pOwnerWindow, msgText, pWindowTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                End If
            End If
        Catch ex As Exception
            Throw ex
        End Try
        Return result
    End Function

    ''' <summary>
    ''' Auxiliary function for STANDARD Tests
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SG 01/09/2010
    ''' </remarks>
    Private Function GetFirstSampleType(ByVal pTestID As Integer) As String
        Try
            Dim qSampleTypes As New List(Of MasterDataDS.tcfgMasterDataRow)
            qSampleTypes = GetSampleTypesByPosition()

            If (qSampleTypes IsNot Nothing AndAlso qSampleTypes.Count > 0) Then
                Dim qTestSampleRows As New List(Of TestSamplesDS.tparTestSamplesRow)

                For Each pos As MasterDataDS.tcfgMasterDataRow In qSampleTypes
                    qTestSampleRows = GetTestSampleListBySampleType(pTestID, pos.ItemID.Trim)
                    If (qTestSampleRows IsNot Nothing AndAlso qTestSampleRows.Count > 0) Then
                        Return qTestSampleRows(0).SampleType
                    End If
                Next

                'It is newly edited
                Return "NEWLY_EDITED"
            Else
                Return ""
            End If
        Catch ex As Exception
            Throw ex
            Return ""
        End Try
    End Function

    ''' <summary>
    ''' Auxiliary function 
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SG 01/09/2010
    ''' </remarks>
    Private Function GetSampleTypesByPosition() As List(Of MasterDataDS.tcfgMasterDataRow)
        Try
            Dim qSampleTypes As New List(Of MasterDataDS.tcfgMasterDataRow)
            qSampleTypes = (From a In AllSampleTypesDS.tcfgMasterData _
                        Order By a.Position _
                          Select a).ToList()
            Return qSampleTypes
        Catch ex As Exception
            Throw ex
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' Auxiliary function
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SG 01/09/2010
    ''' </remarks>
    Private Function GetTestSampleListBySampleType(ByVal pTestId As Integer, ByVal pSampleType As String) As List(Of TestSamplesDS.tparTestSamplesRow)
        Try
            Dim qTestSampleRows As New List(Of TestSamplesDS.tparTestSamplesRow)

            If (SelectedTestSamplesDS IsNot Nothing) Then
                If (SelectedTestSamplesDS.tparTestSamples.Rows.Count > 0) Then
                    'Filter the SelectedTestSample by the TestID and SampleTye
                    qTestSampleRows = (From a In SelectedTestSamplesDS.tparTestSamples _
                                      Where a.TestID = pTestId And a.SampleType = pSampleType _
                                     Select a).ToList()
                End If
            End If
            Return qTestSampleRows
        Catch ex As Exception
            Throw ex
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' Auxiliary function for ISE Tests
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SG 10/10/2010
    ''' </remarks>
    Private Function GetISETestSampleListBySampleType(ByVal pISETestId As Integer, ByVal pISESampleType As String) As List(Of ISETestSamplesDS.tparISETestSamplesRow)
        Try
            Dim qISETestSampleRows As New List(Of ISETestSamplesDS.tparISETestSamplesRow)

            If (SelectedISETestSamplesDS IsNot Nothing) Then
                If (SelectedISETestSamplesDS.tparISETestSamples.Rows.Count > 0) Then
                    'Filter the SelectedTestSample by the ISETestID and SampleType
                    qISETestSampleRows = (From a In SelectedISETestSamplesDS.tparISETestSamples _
                                         Where a.ISETestID = pISETestId And a.SampleType = pISESampleType _
                                        Select a).ToList()
                End If
            End If
            Return qISETestSampleRows
        Catch ex As Exception
            Throw ex
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' Auxiliary function for OffSystem Tests
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 01/12/2010
    ''' </remarks>
    Private Function GetOffSystemTestSampleListBySampleType(ByVal pOFFSTestId As Integer, _
                                                            ByVal pOFFSSampleType As String) As List(Of OffSystemTestSamplesDS.tparOffSystemTestSamplesRow)
        Try
            Dim qOFFSTestSampleRows As New List(Of OffSystemTestSamplesDS.tparOffSystemTestSamplesRow)

            If (SelectedOffSystemTestSamplesDS IsNot Nothing) Then
                If (SelectedOffSystemTestSamplesDS.tparOffSystemTestSamples.Rows.Count > 0) Then
                    'Filter the SelectedTestSample by the OFFSTestID and SampleType
                    qOFFSTestSampleRows = (From a In SelectedOffSystemTestSamplesDS.tparOffSystemTestSamples _
                                           Where a.OffSystemTestID = pOFFSTestId And a.SampleType = pOFFSSampleType _
                                           Select a).ToList()
                End If
            End If
            Return qOFFSTestSampleRows
        Catch ex As Exception
            Throw ex
            Return Nothing
        End Try
    End Function

#End Region

#Region "Event Handlers"
    ''' <summary>
    ''' Activate the Change made variable when some value of the corresponding control is change.
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SG 01/09/2010
    ''' </remarks>
    Private Sub ControlValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsRefNormalLowerLimitUpDown.ValueChanged, _
                                                                                                        bsRefBorderLineUpperLimitUpDown.ValueChanged, _
                                                                                                        bsRefNormalUpperLimitUpDown.ValueChanged, _
                                                                                                        bsRefBorderLineLowerLimitUpDown.ValueChanged, _
                                                                                                        bsRefDetailDataGridView.CellValueChanged
        Try
            If (EditionModeAttribute) Then
                If (Not ChangeSampleTypeDuringEdition) Then ChangesMade = True 'AG 27/10/2010
            End If
        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    ''' <summary>
    ''' Checks characters entered in Numeric UpDown controls are valid 
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SG 01/09/2010</remarks>
    Private Sub NumericUpDown_KeyPress(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles bsRefNormalUpperLimitUpDown.KeyPress, _
                                                                                                                                 bsRefNormalLowerLimitUpDown.KeyPress, _
                                                                                                                                 bsRefBorderLineUpperLimitUpDown.KeyPress, _
                                                                                                                                 bsRefBorderLineLowerLimitUpDown.KeyPress, _
                                                                                                                                 bsRefDetailUpperUpDown.KeyPress, _
                                                                                                                                 bsRefDetailLowerUpDown.KeyPress, _
                                                                                                                                 bsRefDetailAgeToUpDown.KeyPress, _
                                                                                                                                 bsRefDetailAgeFromUpDown.KeyPress
        Try
            If (e.KeyChar = "." Or e.KeyChar = ",") Then
                e.KeyChar = CChar(OSDecimalSeparator)
                If (CType(sender, NumericUpDown).Text.Contains(".") Or CType(sender, NumericUpDown).Text.Contains(",")) Then
                    e.Handled = True
                End If
            End If
        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    ''' <summary>
    ''' When an Numeric UpDown control is empty, the defined minimum value is set as control value
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SG 01/09/2010
    ''' </remarks>
    Private Sub NumericUpDown_KeyUp(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles bsRefNormalUpperLimitUpDown.KeyUp, _
                                                                                                                         bsRefNormalLowerLimitUpDown.KeyUp, _
                                                                                                                         bsRefBorderLineUpperLimitUpDown.KeyUp, _
                                                                                                                         bsRefBorderLineLowerLimitUpDown.KeyUp, _
                                                                                                                         bsRefDetailUpperUpDown.KeyUp, _
                                                                                                                         bsRefDetailLowerUpDown.KeyUp, _
                                                                                                                         bsRefDetailAgeToUpDown.KeyUp, _
                                                                                                                         bsRefDetailAgeFromUpDown.KeyUp
        Try
            Dim miNumericUpDown As NumericUpDown = CType(sender, NumericUpDown)
            If (miNumericUpDown.Text = "") Then
                miNumericUpDown.Value = miNumericUpDown.Minimum
                miNumericUpDown.ResetText()
            End If

            If (EditionModeAttribute) Then
                If (Not ChangeSampleTypeDuringEdition) Then ChangesMade = True 'AG 27/10/2010
            End If
        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    ''' <summary>
    ''' Generic event to clean the Error Provider when the value in the NumericUpDown showing the error is changed
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 18/11/2010
    ''' </remarks>
    Private Sub NumericUpDown_ValueChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles bsRefNormalLowerLimitUpDown.ValueChanged, _
                                                                                                        bsRefNormalUpperLimitUpDown.ValueChanged, _
                                                                                                        bsRefBorderLineLowerLimitUpDown.ValueChanged, _
                                                                                                        bsRefBorderLineUpperLimitUpDown.ValueChanged, _
                                                                                                        bsRefDetailAgeFromUpDown.ValueChanged, _
                                                                                                        bsRefDetailAgeToUpDown.ValueChanged, _
                                                                                                        bsRefDetailLowerUpDown.ValueChanged, _
                                                                                                        bsRefDetailUpperUpDown.ValueChanged
        Try
            Dim myNumUpDown As New NumericUpDown
            myNumUpDown = CType(sender, NumericUpDown)

            If (myNumUpDown.Value > 0) Then
                If (bsControlErrorProvider.GetError(myNumUpDown) <> "") Then
                    bsControlErrorProvider.SetError(myNumUpDown, String.Empty)
                End If
            End If
        Catch ex As Exception
            Throw ex
        End Try
    End Sub
    ''' <summary>
    ''' Manages validating event of Numeric UpDown controls for Generic Normality Ranges
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SG 01/09/2010
    ''' Modified by: SA 18/11/2010 - When enabled/disabled the BorderLine controls, change also the BackColor
    ''' </remarks>
    Private Sub RefNormalLimits_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles bsRefNormalLowerLimitUpDown.Validating, _
                                                                                                                                    bsRefNormalUpperLimitUpDown.Validating
        Try
            ValidationError = False
            bsControlErrorProvider.Clear()

            ValidateLimitsUpDown(bsRefNormalLowerLimitUpDown, bsRefNormalUpperLimitUpDown, False)
            If (Not ValidationError) Then
                ValidateRefRangesLimits(False)
                If (bsRefNormalLowerLimitUpDown.Text <> "" And bsRefNormalUpperLimitUpDown.Text <> "") Then
                    bsRefBorderLineLowerLimitUpDown.Enabled = True
                    bsRefBorderLineUpperLimitUpDown.Enabled = True

                    bsRefBorderLineLowerLimitUpDown.BackColor = Color.White
                    bsRefBorderLineUpperLimitUpDown.BackColor = Color.White
                Else
                    If (bsRefNormalLowerLimitUpDown.Text = "") Then
                        bsRefNormalLowerLimitUpDown.ResetText()
                    End If
                    If (bsRefNormalUpperLimitUpDown.Text = "") Then
                        bsRefNormalUpperLimitUpDown.ResetText()
                    End If

                    bsRefBorderLineLowerLimitUpDown.Enabled = False
                    bsRefBorderLineUpperLimitUpDown.Enabled = False

                    bsRefBorderLineLowerLimitUpDown.BackColor = SystemColors.MenuBar
                    bsRefBorderLineUpperLimitUpDown.BackColor = SystemColors.MenuBar

                    bsRefBorderLineLowerLimitUpDown.Text = ""
                    bsRefBorderLineUpperLimitUpDown.Text = ""
                End If
            End If
        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    ''' <summary>
    ''' Manages validating event of Numeric UpDown controls for Generic BorderLine Ranges
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SG 01/09/2010
    ''' </remarks>
    Private Sub RefBorderLineLimits_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles bsRefBorderLineUpperLimitUpDown.Validating, _
                                                                                                                                        bsRefBorderLineLowerLimitUpDown.Validating
        Try
            ValidationError = False
            bsControlErrorProvider.Clear()

            ValidateLimitsUpDown(bsRefBorderLineLowerLimitUpDown, bsRefBorderLineUpperLimitUpDown, False)
            ValidateRefRangesLimits(False)
        Catch ex As Exception
            Throw ex
        End Try
    End Sub


    ''' <summary>
    ''' Manages validating event of Numeric UpDown controls for Detailed Normality Ranges and Age From/To
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SG 01/09/2010
    ''' </remarks>
    Private Sub RefDetailLimits_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles bsRefDetailUpperUpDown.Validating, _
                                                                                                                                    bsRefDetailLowerUpDown.Validating, _
                                                                                                                                    bsRefDetailAgeToUpDown.Validating, _
                                                                                                                                    bsRefDetailAgeFromUpDown.Validating
        Try
            ValidationError = False
            bsControlErrorProvider.Clear()

            If (bsRefDetailAgeCheckBox.Checked) Then
                ValidateLimitsUpDown(bsRefDetailAgeFromUpDown, bsRefDetailAgeToUpDown, True)
            End If

            ValidateLimitsUpDown(bsRefDetailLowerUpDown, bsRefDetailUpperUpDown, True)
            IsDetailedRangeEditing = True
        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    ''' <summary>
    ''' Manages Checked Changed event for RadioButtons used to specify the type of Reference Range to define
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SG 01/09/2010
    ''' </remarks>
    Private Sub RefRadioButtons_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsRefGenericRadioButton.CheckedChanged, _
                                                                                                                   bsRefDetailedRadioButton.CheckedChanged
        Try
            Static typelocked As String = ""
            If (Not ValidationError) Then
                typelocked = ""
                RefRadioButtonsManagement()

            ElseIf (bsRefGenericRadioButton.Checked) Then
                If (typelocked = "") Then
                    RefDetailResetValues()
                    bsControlErrorProvider.Clear()
                    ValidationError = False
                End If
                RefRadioButtonsManagement()

            ElseIf (bsRefDetailedRadioButton.Checked) Then
                If (typelocked = "") Then
                    typelocked = "gen"
                    RefDetailResetValues()
                    bsRefGenericRadioButton.Checked = Not bsRefGenericRadioButton.Checked
                Else
                    bsRefGenericRadioButton.Checked = (typelocked = "gen")
                End If
            End If

            If (EditionModeAttribute) Then
                If (Not ChangeSampleTypeDuringEdition) Then ChangesMade = True 'AG 27/10/2010
            End If
        Catch ex As Exception
            Throw ex
        End Try
    End Sub


    ''' <summary>
    ''' Manages Checked Changed event for By Gender and By Age Check Buttons for Detailed Reference Ranges
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SG 01/09/2010
    ''' </remarks>
    Private Sub RefDetail_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsRefDetailGenderCheckBox.CheckedChanged, bsRefDetailAgeCheckBox.CheckedChanged
        Try
            RefDetailCheckBoxesManagement()
        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    ''' <summary>
    ''' Manages the click in button for moving a Detailed Reference Range from controls to DataGrid
    ''' (for adding or updating)
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SG 01/09/2010
    ''' </remarks>
    Private Sub bsRefDetailAddButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsRefDetailAddButton.Click
        Try
            RefDetailUpdate(CurrentSampleType)
        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    ''' <summary>
    ''' Manages the click in button for moving a Detailed Reference Range from DataGrid to controls (for edition)
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SG 01/09/2010
    ''' </remarks>
    Private Sub bsRefDetailEditButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsRefDetailEditButton.Click
        Try
            RefDetailEdit()
        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    ''' <summary>
    ''' Manages the click in button for deleting the Detailed Reference Range selected in the DataGrid
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SG 01/09/2010
    ''' </remarks>
    Private Sub bsRefDetailDeleteButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsRefDetailDeleteButton.Click
        Try
            RefDetailDelete()
        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    ''' <summary>
    ''' Reload limits of fields for Age From/To when the selecting a different Age Unit
    ''' </summary>
    ''' <remarks>Created by:  SG 01/09/2010</remarks>
    Private Sub RefDetailAgeComboBox_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsRefDetailAgeComboBox.SelectedIndexChanged
        Try
            RefDetailAgeLimitsSetUp()
        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    ''' <summary>
    ''' Load the double-clicked Detailed Reference Range in the controls (for edition)
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SG 01/09/2010
    ''' </remarks>
    Private Sub RefDetailDataGridView_DoubleClick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsRefDetailDataGridView.DoubleClick
        Try
            RefDetailEdit()
        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    ''' <summary>
    ''' Delete the selected Detailed Reference Ranges using the SUPR key
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SG 01/09/2010
    ''' </remarks>
    Private Sub RefDetailDataGridView_PreviewKeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.PreviewKeyDownEventArgs) Handles bsRefDetailDataGridView.PreviewKeyDown
        Try
            If e.KeyCode = Keys.Delete Then
                If bsRefDetailDeleteButton.Enabled Then
                    RefDetailDelete()
                End If
            End If
        Catch ex As Exception
            Throw ex
        End Try
    End Sub



#End Region

#Region "QUITAR?"
    'QUITAR?????

    'Private Function RefGetEditingDetailedType() As String
    '    Try
    '        If RefDetailedRadioButton.Checked Then
    '            If RefDetailGenderCheckBox.Checked And RefDetailAgeCheckBox.Checked Then
    '                Return "GENDER_AGE"
    '            ElseIf RefDetailGenderCheckBox.Checked Then
    '                Return "GENDER"
    '            ElseIf RefDetailAgeCheckBox.Checked Then
    '                Return "AGE"
    '            Else
    '                Return "NONE"
    '            End If
    '        Else
    '            Return "NONE"
    '        End If

    '    Catch ex As Exception
    '        Throw ex
    '    End Try
    'End Function
    'Private Sub InitializeScreen()
    '    Try

    '        SetUpControlsLimits()

    '        FillRefDetailedAreaCombos()
    '        RefRadioButtonsManagement()
    '        RefDetailCheckBoxesManagement()

    '    Catch ex As Exception
    '        Throw ex
    '    End Try
    'End Sub

    'Public Sub ClearTestRefRanges()
    '    Try

    '        EditionModeAttribute = False
    '        RangeType = ""

    '        SelectedTestRefRangesDS.tparTestRefRanges.Clear()


    '        RefNormalUpperLimitUpDown.ResetText()
    '        RefNormalLowerLimitUpDown.ResetText()
    '        RefBorderLineUpperLimitUpDown.ResetText()
    '        RefBorderLineLowerLimitUpDown.ResetText()


    '    Catch ex As Exception
    '        Throw ex
    '    End Try
    'End Sub





    '''' <summary>
    '''' 
    '''' </summary>
    '''' <param name="pDecimals"></param>
    '''' <remarks></remarks>
    'Public Sub SetupDecimalsNumber(ByVal pDecimals As Integer)
    '    Try

    '        RefNormalLowerLimitUpDown.DecimalPlaces = pDecimals
    '        If RefNormalLowerLimitUpDown.Text <> "" Then
    '            RefNormalLowerLimitUpDown.Value = CDec(RefNormalLowerLimitUpDown.Text)
    '        End If

    '        RefNormalUpperLimitUpDown.DecimalPlaces = pDecimals
    '        If RefNormalUpperLimitUpDown.Text <> "" Then
    '            RefNormalUpperLimitUpDown.Value = CDec(RefNormalUpperLimitUpDown.Text)
    '        End If

    '        RefBorderLineLowerLimitUpDown.DecimalPlaces = pDecimals
    '        If RefBorderLineLowerLimitUpDown.Text <> "" Then
    '            RefBorderLineLowerLimitUpDown.Value = CDec(RefBorderLineLowerLimitUpDown.Text)
    '        End If

    '        RefBorderLineUpperLimitUpDown.DecimalPlaces = pDecimals
    '        If RefBorderLineUpperLimitUpDown.Text <> "" Then
    '            RefBorderLineUpperLimitUpDown.Value = CDec(RefBorderLineUpperLimitUpDown.Text)
    '        End If


    '    Catch ex As Exception
    '        Throw ex
    '    End Try
    'End Sub

    'Private Sub BindControls(ByVal pTestID As Integer, Optional ByVal pSampleType As String = "")
    '    Try
    '        Dim myTestID As Integer = pTestID
    '        Dim mySampleType As String = pSampleType
    '        'Get the data to connect the controls.

    '        Dim qTestSamples As New List(Of TestSamplesDS.tparTestSamplesRow)

    '        BindReferenceRangesControls(myTestID, ActiveRangeType, mySampleType) 'AG 19/07/2010 - add sampletype parameter 'SG 27/07/2010 add rangetype parameter


    '    Catch ex As Exception
    '        Throw ex
    '    End Try
    'End Sub

    'Private Function RefGetRangeType() As String
    '    Try
    '        If RefGenericRadioButton.Checked Then
    '            Return "GENERIC"
    '        ElseIf RefDetailedRadioButton.Checked Then
    '            Return "DETAILED"
    '        Else
    '            Return ""
    '        End If
    '    Catch ex As Exception
    '        Throw ex
    '    End Try
    'End Function

    '''' <summary>
    '''' gets the fixed item description of the specified gender item
    '''' </summary>
    '''' <param name="pItem"></param>
    '''' <returns></returns>
    '''' <remarks>Created by:  SG 01/09/2010</remarks>
    'Private Function GetGenderDescription(ByVal pItem As String) As String
    '    Try

    '        Dim qGenderList As New List(Of PreloadedMasterDataDS.tfmwPreloadedMasterDataRow)

    '        qGenderList = (From a In GendersMasterDataDS.tfmwPreloadedMasterData Where a.ItemID.ToUpper = pItem.ToUpper Select a).ToList()

    '        If qGenderList.Count > 0 Then
    '            Return qGenderList.First.FixedItemDesc
    '        Else
    '            Return Nothing
    '        End If

    '    Catch ex As Exception
    '        Throw ex
    '        Return Nothing
    '    End Try
    'End Function

    '''' <summary>
    '''' 
    '''' </summary>
    '''' <param name="sender"></param>
    '''' <param name="e"></param>
    '''' <remarks>Created by:  SG 01/09/2010</remarks>
    'Private Sub RefBorderLineLowerLimitUpDown_Validated(ByVal sender As System.Object, ByVal e As System.EventArgs)
    '    Try

    '        bsControlErrorProvider.Clear()
    '        If bsRefBorderLineLowerLimitUpDown.Text <> "" Then
    '            If RefBorderLineUpperLimitUpDown.Text <> "" Then
    '                If CType(bsRefBorderLineLowerLimitUpDown.Text, Decimal) > CType(RefBorderLineUpperLimitUpDown.Text, Decimal) Then
    '                    bsControlErrorProvider.SetError(bsRefBorderLineLowerLimitUpDown, GetMessageText(RefMessages.MINGREATHERMAX.ToString)) 'AG 07/07/2010("MINGREATHERMAX"))
    '                    ValidationError = True
    '                    bsRefBorderLineLowerLimitUpDown.Select()
    '                End If
    '            Else
    '                bsControlErrorProvider.SetError(RefBorderLineUpperLimitUpDown, GetMessageText(RefMessages.NOT_NULL_VALUE.ToString)) 'AG 07/07/2010("NOT_NULL_VALUE"))
    '                ValidationError = True
    '                RefBorderLineUpperLimitUpDown.Select()
    '            End If
    '        ElseIf RefBorderLineUpperLimitUpDown.Text <> "" Then
    '            bsControlErrorProvider.SetError(bsRefBorderLineLowerLimitUpDown, GetMessageText(RefMessages.NOT_NULL_VALUE.ToString)) 'AG 07/07/2010("NOT_NULL_VALUE"))
    '            ValidationError = True
    '            'RefBorderLineLowerLimitUpDown.Select()
    '        End If

    '    Catch ex As Exception
    '        Throw ex
    '    End Try
    'End Sub

    '''' <summary>
    '''' 
    '''' </summary>
    '''' <param name="sender"></param>
    '''' <param name="e"></param>
    '''' <remarks>Created by:  SG 01/09/2010</remarks>
    'Private Sub RefBorderLineUpperLimitUpDown_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs)

    '    Try

    '        bsControlErrorProvider.Clear()
    '        ValidationError = False
    '        If RefBorderLineUpperLimitUpDown.Text <> "" Then
    '            If bsRefBorderLineLowerLimitUpDown.Text <> "" Then
    '                If CType(bsRefBorderLineLowerLimitUpDown.Text, Decimal) > CType(RefBorderLineUpperLimitUpDown.Text, Decimal) Then
    '                    bsControlErrorProvider.SetError(bsRefBorderLineLowerLimitUpDown, GetMessageText(RefMessages.MINGREATHERMAX.ToString)) 'AG 07/07/2010("MINGREATHERMAX"))
    '                    ValidationError = True
    '                    bsRefBorderLineLowerLimitUpDown.Select()
    '                End If
    '            Else
    '                bsControlErrorProvider.SetError(bsRefBorderLineLowerLimitUpDown, GetMessageText(RefMessages.NOT_NULL_VALUE.ToString)) 'AG 07/07/2010("NOT_NULL_VALUE"))
    '                ValidationError = True
    '                bsRefBorderLineLowerLimitUpDown.Select()
    '            End If
    '        ElseIf bsRefBorderLineLowerLimitUpDown.Text <> "" Then
    '            bsControlErrorProvider.SetError(RefBorderLineUpperLimitUpDown, GetMessageText(RefMessages.NOT_NULL_VALUE.ToString)) 'AG 07/07/2010("NOT_NULL_VALUE"))
    '            ValidationError = True
    '            RefBorderLineUpperLimitUpDown.Select()
    '        End If

    '    Catch ex As Exception
    '        Throw ex
    '    End Try
    'End Sub

    '''' <summary>
    '''' Enable/disables add and edit buttons according to the multiselection
    '''' </summary>
    '''' <remarks>
    '''' Created by:  SG 01/09/2010
    '''' </remarks>
    'Private Sub RefDetailDataGridView_SelectionChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)
    '    Try
    '        RefDetailAddEditEnabling()
    '    Catch ex As Exception
    '        Throw ex
    '    End Try
    'End Sub

    '''' <summary>
    '''' Set limits for field Age From/To depending on the selected Age Unit
    '''' </summary>
    '''' <remarks>
    '''' Created by:  SG 01/09/2010
    '''' Modified by: SA 17/11/2010 - Use the Code of the selected AgeUnit, not the multilanguage description
    '''' </remarks>
    'Private Sub RefDetailAgeComboBox_SelectedValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)
    '    Try
    '        If (bsRefDetailAgeComboBox.SelectedIndex > -1) Then
    '            Dim item As PreloadedMasterDataDS.tfmwPreloadedMasterDataRow
    '            item = CType(bsRefDetailAgeComboBox.SelectedItem, PreloadedMasterDataDS.tfmwPreloadedMasterDataRow)

    '            Dim Criteria As RefFieldLimitsEnum
    '            Select Case (item.ItemID)
    '                Case "Y" : Criteria = RefFieldLimitsEnum.AGE_FROM_TO_YEARS
    '                Case "M" : Criteria = RefFieldLimitsEnum.AGE_FROM_TO_MONTHS
    '                Case "D" : Criteria = RefFieldLimitsEnum.AGE_FROM_TO_DAYS
    '            End Select
    '            SetRefDetailAgeLimits(Criteria)
    '        End If
    '    Catch ex As Exception
    '        Throw ex
    '    End Try
    'End Sub

    '''' <summary>
    '''' Validates values informed in Age From/To controls
    '''' </summary>
    '''' <remarks>
    '''' Created by:  SG 01/09/2010
    '''' </remarks>
    'Private Sub RefDetailAgeFromToUpDown_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs)
    '    Try
    '        bsControlErrorProvider.Clear()
    '        If (IsDetailedRangeEditing) Then ValidateLimitsUpDown(bsRefDetailAgeFromUpDown, bsRefDetailAgeToUpDown, True)
    '    Catch ex As Exception
    '        Throw ex
    '    End Try
    'End Sub

    '''' <summary>
    '''' 
    '''' </summary>
    '''' <param name="dgr"></param>
    '''' <param name="NewSampleType"></param>
    '''' <param name="hideOriginal"></param>
    '''' <returns></returns>
    '''' <remarks>Created by:  SG 01/09/2010</remarks>
    'Private Function RefDetailDataGridRowClone(ByVal dgr As DataGridViewRow, ByVal NewSampleType As String, ByVal hideOriginal As Boolean) As DataGridViewRow
    '    Dim newDgr As New DataGridViewRow
    '    Try
    '        Dim colSampleType As Integer = dgr.Cells("SampleType").ColumnIndex
    '        Dim colAdded As Integer = dgr.Cells("Added").ColumnIndex

    '        newDgr = CType(dgr.Clone, DataGridViewRow)
    '        If hideOriginal Then dgr.Visible = False


    '        For Each cell As DataGridViewCell In dgr.Cells
    '            Dim col As Integer = cell.ColumnIndex
    '            newDgr.Cells(col).Value = cell.Value
    '        Next

    '        newDgr.Cells(colAdded).Value = True
    '        newDgr.Cells(colSampleType).Value = NewSampleType

    '        Return newDgr

    '    Catch ex As Exception
    '        Throw ex
    '        Return Nothing
    '    End Try
    'End Function

    '''' <summary>
    '''' Sets the age limits depending on the age criteria
    '''' </summary>
    '''' <param name="pCriteria"></param>
    '''' <remarks>
    '''' Created by:  SG 01/09/2010
    '''' </remarks>
    'Private Sub SetRefDetailAgeLimits(ByVal pCriteria As RefFieldLimitsEnum)
    '    Try
    '        Dim myFieldLimits As FieldLimitsDS.tfmwFieldLimitsRow
    '        myFieldLimits = GetControlsLimits(pCriteria)
    '        If (myFieldLimits IsNot Nothing) Then
    '            bsRefDetailAgeToUpDown.Minimum = CType(myFieldLimits.MinValue, Integer)
    '            bsRefDetailAgeToUpDown.Maximum = CType(myFieldLimits.MaxValue, Integer)
    '            bsRefDetailAgeToUpDown.DecimalPlaces = 0
    '            bsRefDetailAgeToUpDown.Increment = 1

    '            bsRefDetailAgeFromUpDown.Minimum = CType(myFieldLimits.MinValue, Integer)
    '            bsRefDetailAgeFromUpDown.Maximum = CType(myFieldLimits.MaxValue, Integer)
    '            bsRefDetailAgeFromUpDown.DecimalPlaces = 0
    '            bsRefDetailAgeFromUpDown.Increment = 1
    '        End If
    '    Catch ex As Exception
    '        Throw ex
    '    End Try
    'End Sub

    '''' <summary>
    '''' sets the control limits
    '''' </summary>
    '''' <remarks>Created by:  SG 01/09/2010</remarks>
    'Private Sub SetUpControlsLimits()

    '    Try
    '        Dim myFieldLimits As FieldLimitsDS.tfmwFieldLimitsRow

    '        'Reference ranges
    '        'NormalLowerLimit, NormalUpperLimit
    '        myFieldLimits = GetControlsLimits(RefFieldLimitsEnum.REF_RANGE_LIMITS)
    '        If myFieldLimits IsNot Nothing Then
    '            bsRefNormalLowerLimitUpDown.Minimum = CType(myFieldLimits.MinValue, Decimal)
    '            bsRefNormalLowerLimitUpDown.Maximum = CType(myFieldLimits.MaxValue, Decimal)
    '            'RefNormalLowerLimitUpDown.DecimalPlaces = myFieldLimits.DecimalsAllowed
    '            If Not myFieldLimits.IsStepValueNull Then
    '                bsRefNormalLowerLimitUpDown.Increment = CType(myFieldLimits.StepValue, Decimal)
    '            End If

    '            bsRefNormalUpperLimitUpDown.Minimum = CType(myFieldLimits.MinValue, Decimal)
    '            bsRefNormalUpperLimitUpDown.Maximum = CType(myFieldLimits.MaxValue, Decimal)
    '            'RefNormalUpperLimitUpDown.DecimalPlaces = myFieldLimits.DecimalsAllowed
    '            If Not myFieldLimits.IsStepValueNull Then
    '                bsRefNormalUpperLimitUpDown.Increment = CType(myFieldLimits.StepValue, Decimal)
    '            End If
    '        End If

    '        'BorderLineLowerLimit, BorderLineUpperLimit
    '        myFieldLimits = GetControlsLimits(RefFieldLimitsEnum.REF_RANGE_LIMITS)
    '        If myFieldLimits IsNot Nothing Then
    '            bsRefBorderLineLowerLimitUpDown.Minimum = CType(myFieldLimits.MinValue, Decimal)
    '            bsRefBorderLineLowerLimitUpDown.Maximum = CType(myFieldLimits.MaxValue, Decimal)
    '            'RefBorderLineLowerLimitUpDown.DecimalPlaces = myFieldLimits.DecimalsAllowed
    '            If Not myFieldLimits.IsStepValueNull Then
    '                bsRefBorderLineLowerLimitUpDown.Increment = CType(myFieldLimits.StepValue, Decimal)
    '            End If

    '            bsRefBorderLineUpperLimitUpDown.Minimum = CType(myFieldLimits.MinValue, Decimal)
    '            bsRefBorderLineUpperLimitUpDown.Maximum = CType(myFieldLimits.MaxValue, Decimal)
    '            'RefBorderLineUpperLimitUpDown.DecimalPlaces = myFieldLimits.DecimalsAllowed
    '            If Not myFieldLimits.IsStepValueNull Then
    '                bsRefBorderLineUpperLimitUpDown.Increment = CType(myFieldLimits.StepValue, Decimal)
    '            End If
    '        End If

    '        'DetailedLowerLimit, DetailedUpperLimit
    '        myFieldLimits = GetControlsLimits(RefFieldLimitsEnum.REF_RANGE_LIMITS)
    '        If myFieldLimits IsNot Nothing Then
    '            bsRefDetailLowerUpDown.Minimum = CType(myFieldLimits.MinValue, Decimal)
    '            bsRefDetailLowerUpDown.Maximum = CType(myFieldLimits.MaxValue, Decimal)
    '            'RefDetailLowerUpDown.DecimalPlaces = myFieldLimits.DecimalsAllowed
    '            If Not myFieldLimits.IsStepValueNull Then
    '                bsRefDetailLowerUpDown.Increment = CType(myFieldLimits.StepValue, Decimal)
    '            End If

    '            bsRefDetailUpperUpDown.Minimum = CType(myFieldLimits.MinValue, Decimal)
    '            bsRefDetailUpperUpDown.Maximum = CType(myFieldLimits.MaxValue, Decimal)
    '            'RefDetailUpperUpDown.DecimalPlaces = myFieldLimits.DecimalsAllowed
    '            If Not myFieldLimits.IsStepValueNull Then
    '                bsRefDetailUpperUpDown.Increment = CType(myFieldLimits.StepValue, Decimal)
    '            End If
    '        End If


    '        'AgeFrom, AgeTo
    '        myFieldLimits = GetControlsLimits(RefFieldLimitsEnum.AGE_FROM_TO_YEARS)
    '        If myFieldLimits IsNot Nothing Then
    '            bsRefDetailAgeToUpDown.Minimum = CType(myFieldLimits.MinValue, Integer)
    '            bsRefDetailAgeToUpDown.Maximum = CType(myFieldLimits.MaxValue, Integer)
    '            bsRefDetailAgeToUpDown.DecimalPlaces = 0
    '            bsRefDetailAgeToUpDown.Increment = 1

    '            bsRefDetailAgeFromUpDown.Minimum = CType(myFieldLimits.MinValue, Integer)
    '            bsRefDetailAgeFromUpDown.Maximum = CType(myFieldLimits.MaxValue, Integer)
    '            bsRefDetailAgeFromUpDown.DecimalPlaces = 0
    '            bsRefDetailAgeFromUpDown.Increment = 1
    '        End If

    '        myFieldLimits = GetControlsLimits(RefFieldLimitsEnum.REF_RANGE_LIMITS)
    '        If myFieldLimits IsNot Nothing Then
    '            bsRefDetailLowerUpDown.Minimum = CType(myFieldLimits.MinValue, Decimal)
    '            bsRefDetailLowerUpDown.Maximum = CType(myFieldLimits.MaxValue, Decimal)
    '            bsRefDetailLowerUpDown.DecimalPlaces = myFieldLimits.DecimalsAllowed
    '            If Not myFieldLimits.IsStepValueNull Then
    '                bsRefDetailLowerUpDown.Increment = CType(myFieldLimits.StepValue, Decimal)
    '            End If

    '            bsRefDetailUpperUpDown.Minimum = CType(myFieldLimits.MinValue, Decimal)
    '            bsRefDetailUpperUpDown.Maximum = CType(myFieldLimits.MaxValue, Decimal)
    '            bsRefDetailUpperUpDown.DecimalPlaces = myFieldLimits.DecimalsAllowed
    '            If Not myFieldLimits.IsStepValueNull Then
    '                bsRefDetailUpperUpDown.Increment = CType(myFieldLimits.StepValue, Decimal)
    '            End If
    '        End If

    '    Catch ex As Exception
    '        Throw ex
    '    End Try

    'End Sub

#End Region
End Class
