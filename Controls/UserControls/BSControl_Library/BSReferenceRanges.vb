Option Strict On
Option Explicit On

Imports System.Windows.Forms
Imports System.Drawing
'Imports Biosystems.Ax00
'Imports Biosystems.Ax00.Controls
Imports Biosystems.Ax00.Controls.UserControls
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Types.TestRefRangesDS

'User Control that allows the edition of Reference Ranges for Tests of whatever type: 
'Standard, Calculated, ISE and/or Off-System Tests
'Created by:  SG 01/09/2010
'Modified by: SA 15/12/2010 - New implementation


'RH 05/06/2012 Remove every Try/Catch
'It is a bad practice to catch an exception, do nothing with it and throw it again.
'Catch an exception only if you want to do something with it.
'"You should catch only those exceptions that you can recover from."
'"Catching exceptions that you cannot legitimately handle hides critical debugging information."
'"The purpose of a catch clause is to allow you to handle exceptions"
'On the other hand, "Do prefer using an empty throw (just "Throw") when catching and re-throwing an exception.
'This is the best way to preserve the exception call stack".

'Public Sub MethodWithBadCatch(ByVal anObject As Object)
'    Try
'        DoWork(anObject)

'    Catch e As ArgumentNullException
'        System.Diagnostics.Debug.Write(e.Message)
'        ' This is wrong.
'        Throw e
'        ' Should be this:
'        ' throw
'    End Try
'End Sub

'http://msdn.microsoft.com/en-us/library/ms229005(v=VS.90).aspx
'http://exceptionalcode.wordpress.com/2010/04/13/net-exceptions-throw-ex-is-evil-but-throw-is-not-that-innocent/

Public Class BSReferenceRanges

#Region "Attributes"

    'Test data
    Private TestTypeAttribute As String = "STD"
    Private TestIDAttribute As Integer = -1
    Private SampleTypeAttribute As String = String.Empty
    Private NumAllowedDecimalsAttribute As Integer = 0
    Private ActiveRangeTypeAttribute As String = String.Empty

    'SA + DL 16/10/2012. Begin
    'Private SelectedTestRefRangesDS As TestRefRangesDS = Nothing
    Private SelectedTestRefRangesDS As New TestRefRangesDS
    'SA + DL 16/10/2012. End

    'RH 05/06/2012 This solves the bug "Code generation for property  'DefinedTestRangesDS' failed.
    'Error was: 'Property accessor 'DefinedTestRangesDS' on object 'bsTestRefRanges'
    'threw thefollowing exception: 'Object reference not set to an instance of an object."


    'To set header text of visible columns in DataGridView of Detailed Reference Ranges
    Private GenderColumnTextAttribute As String = "Gender"
    Private AgeUnitColumnTextAttribute As String = "Age Unit"
    Private AgeFromColumnTextAttribute As String = "From"
    Private AgeToColumnTextAttribute As String = "To"
    Private LowerColumnTextAttribute As String = "Min. Value"
    Private UpperColumnTextAttribute As String = "Max. Value"
    Private user As String = String.Empty 'JV 23/01/2014 #1013

    'To control edition, errors and pending changes...
    Private EditionModeAttribute As Boolean = False
    Private ValidationErrorAttribute As Boolean = False
    Private ChangesMadeAttribute As Boolean
    Private ChangeSampleTypeDuringEditionAttribute As Boolean = False

#End Region

#Region "Public Properties"

    'To set the control height depending in which screen the User Control is shown:
    'for Standard Tests, there is less space available for Detailed Ranges, while for 
    'Calculated, ISE and Off-System Tests, more rows can be shown in the DataGridView
    'Created by:  SA 09/12/2010
    Public WriteOnly Property SmallLayout() As Boolean
        Set(ByVal value As Boolean)
            If (value) Then
                Me.Height = 252
                BsPanel.Height = 252
                bsRefDetailDataGridView.Height = 100
            Else
                Me.Height = 320
                BsPanel.Height = 320
                bsRefDetailDataGridView.Height = 127
            End If
        End Set
    End Property

    'To verify if there is at least a field marked with an error in the Control
    Public ReadOnly Property ValidationError() As Boolean
        Get
            Return ValidationErrorAttribute
        End Get
    End Property

    'To set the multilanguage text for Generic RadioButton
    'Created by:  SG 04/10/10
    Public WriteOnly Property TextForGenericRadioButton() As String
        Set(ByVal value As String)
            bsGenericCheckBox.Text = value
        End Set
    End Property

    'To set the multilanguage text for labels for Normality Range
    'Created by:  SG 04/10/10
    Public WriteOnly Property TextForNormalityLabel() As String
        Set(ByVal value As String)
            bsRefGenericNormalLabel.Text = value
        End Set
    End Property

    'To set the multilanguage text for the label for BorderLine Range
    'Created by:  SG 04/10/10
    Public WriteOnly Property TextForBorderLineLabel() As String
        Set(ByVal value As String)
            bsBorderLineLabel.Text = value
        End Set
    End Property

    'To set the multilanguage text for labels for controls used to enter the Min Range Value
    'Created by:  SG 04/10/10
    'Modify by: TR 09/09/2013 -Add the missing semicolon bug #930.
    Public WriteOnly Property TextForMinValueLabel() As String
        Set(ByVal value As String)
            bsRefNormalMinLabel.Text = value & ":"
            bsRefBorderMinLabel.Text = value & ":"
            LowerColumnTextAttribute = value
        End Set
    End Property

    'To set the multilanguage text for labels for controls used to enter the Max Range Value
    'Created by:  SG 04/10/10
    'Modify by: TR 09/09/2013 -Add the missing semicolon bug #930.
    Public WriteOnly Property TextForMaxValueLabel() As String
        Set(ByVal value As String)
            bsRefNormalMaxLabel.Text = value & ":"
            bsRefBorderMaxLabel.Text = value & ":"
            UpperColumnTextAttribute = value
        End Set
    End Property

    'To set the Measure Unit of the Test for which the Reference Ranges are defined
    Public WriteOnly Property MeasureUnit() As String
        Set(ByVal value As String)
            bsGenericNormalUnitLabel.Text = value
            bsBorderLineUnitLabel.Text = value
        End Set
    End Property

    'To set the multilanguage text for Detailed RadioButton
    'Created by:  SG 04/10/10
    Public WriteOnly Property TextForDetailedRadioButton() As String
        Set(ByVal value As String)
            bsDetailedCheckBox.Text = value
        End Set
    End Property

    'To set multilanguage texts of all visible columns in the DataGrid of Detailed Reference Ranges
    'Created by:  SG 04/10/10
    Public WriteOnly Property TextForGenderColumn() As String
        Set(ByVal value As String)
            GenderColumnTextAttribute = value
        End Set
    End Property

    Public WriteOnly Property TextForAgeColumn() As String
        Set(ByVal value As String)
            AgeUnitColumnTextAttribute = value
        End Set
    End Property

    Public WriteOnly Property TextForFromColumn() As String
        Set(ByVal value As String)
            AgeFromColumnTextAttribute = value
        End Set
    End Property

    Public WriteOnly Property TextForToColumn() As String
        Set(ByVal value As String)
            AgeToColumnTextAttribute = value
        End Set
    End Property

    'To set the Icon and ToolTip Text for Delete Detailed Range Button
    'Created by: DL 10/11/2010
    Public WriteOnly Property DeleteButtonImage() As Image
        Set(ByVal value As Image)
            bsRefDetailDeleteButton.Image = value
        End Set
    End Property

    Public WriteOnly Property ToolTipForDetailDeleteButton() As String
        Set(ByVal value As String)
            bsControlToolTips.SetToolTip(bsRefDetailDeleteButton, value)
        End Set
    End Property

    'To set the number of decimals allowed in min and max values (the number of decimals 
    'defined for the Test for which the Reference Ranges are defined)
    'Created by:  SG 04/10/10
    Public WriteOnly Property RefNumDecimals() As Integer
        Set(ByVal value As Integer)
            bsRefNormalLowerLimitUpDown.DecimalPlaces = value
            bsRefNormalUpperLimitUpDown.DecimalPlaces = value
            bsRefBorderLineLowerLimitUpDown.DecimalPlaces = value
            bsRefBorderLineUpperLimitUpDown.DecimalPlaces = value

            If (String.Compare(bsRefNormalLowerLimitUpDown.Text, String.Empty, False) <> 0) Then
                bsRefNormalLowerLimitUpDown.Value = CDec(bsRefNormalLowerLimitUpDown.Text)
            End If
            If (String.Compare(bsRefNormalUpperLimitUpDown.Text, String.Empty, False) <> 0) Then
                bsRefNormalUpperLimitUpDown.Value = CDec(bsRefNormalUpperLimitUpDown.Text)
            End If
            If (String.Compare(bsRefBorderLineLowerLimitUpDown.Text, String.Empty, False) <> 0) Then
                bsRefBorderLineLowerLimitUpDown.Value = CDec(bsRefBorderLineLowerLimitUpDown.Text)
            End If
            If (String.Compare(bsRefBorderLineUpperLimitUpDown.Text, String.Empty, False) <> 0) Then
                bsRefBorderLineUpperLimitUpDown.Value = CDec(bsRefBorderLineUpperLimitUpDown.Text)
            End If

            If (bsRefDetailDataGridView.ColumnCount > 0) Then
                'Set the new number of decimals for Normality values for Detailed Reference Ranges
                DirectCast(bsRefDetailDataGridView.Columns("NormalLowerLimit"), DataGridViewNumericUpDownColumn).DecimalPlaces = value
                DirectCast(bsRefDetailDataGridView.Columns("NormalUpperLimit"), DataGridViewNumericUpDownColumn).DecimalPlaces = value
            End If

            NumAllowedDecimalsAttribute = value
        End Set
    End Property

    'To set the type of Test for which the Reference Ranges control is being used
    Public WriteOnly Property TestType() As String
        Set(ByVal value As String)
            TestTypeAttribute = value

            Dim visibleBL As Boolean = (String.Compare(value, "STD", False) = 0)
            bsRefBorderMinLabel.Visible = visibleBL
            bsRefBorderMaxLabel.Visible = visibleBL
            bsBorderLineLabel.Visible = visibleBL
            bsBorderLineUnitLabel.Visible = visibleBL
            bsRefBorderLineLowerLimitUpDown.Visible = visibleBL
            bsRefBorderLineUpperLimitUpDown.Visible = visibleBL
        End Set
    End Property

    'To set the identifier of the Test for which the Reference Ranges will be added/updated
    Public WriteOnly Property TestID() As Integer
        Set(ByVal value As Integer)
            TestIDAttribute = value
        End Set
    End Property

    'To set the SampleType for which the Reference Ranges are defined for a specific Test
    Public WriteOnly Property SampleType() As String
        Set(ByVal value As String)
            SampleTypeAttribute = value
            ChangeSampleType()
        End Set
    End Property

    'To set the type of Range defined for a specific Test/SampleType
    Public Property ActiveRangeType() As String
        Get
            'Set value of ActiveRangeType according the informed values
            If (bsGenericCheckBox.Checked And (String.Compare(bsRefNormalLowerLimitUpDown.Text, String.Empty, False) <> 0 Or _
                                               String.Compare(bsRefNormalUpperLimitUpDown.Text, String.Empty, False) <> 0)) Then
                ActiveRangeTypeAttribute = "GENERIC"
            ElseIf (bsDetailedCheckBox.Checked And bsRefDetailDataGridView.Rows.Count > 1) Then
                ActiveRangeTypeAttribute = "DETAILED"
            Else
                ActiveRangeTypeAttribute = String.Empty

                bsGenericCheckBox.Checked = False
                bsDetailedCheckBox.Checked = False

                DisableControls("ALL", True)
            End If

            Return ActiveRangeTypeAttribute
        End Get
        Set(ByVal value As String)
            ActiveRangeTypeAttribute = value
        End Set
    End Property

    'To get/return the Reference Ranges currently defined for a specific Test/SampleType
    Public Property DefinedTestRangesDS() As Object
        Get
            If (Not SelectedTestRefRangesDS Is Nothing) Then
                'SelectedTestRefRangesDS.Clear()
                UpdateReferenceRanges()
                Return SelectedTestRefRangesDS
            Else
                Return New TestRefRangesDS
            End If
        End Get
        Set(ByVal value As Object)
            If (Not value Is Nothing) Then
                SelectedTestRefRangesDS = DirectCast(value, TestRefRangesDS)
            Else
                SelectedTestRefRangesDS = New TestRefRangesDS
            End If
        End Set
    End Property

    'To set the current state of the User Control: read-only (when false) or edition (when true)
    Public WriteOnly Property isEditing() As Boolean
        Set(ByVal value As Boolean)
            EditionModeAttribute = value
            CheckBoxManagement()
        End Set
    End Property

    'To manages when the Reference Ranges have been changed
    Public Property ChangesMade() As Boolean
        Get
            Return ChangesMadeAttribute
        End Get
        Set(ByVal value As Boolean)
            'ChangesMadeAttribute = value
            If Not ChangeSampleTypeDuringEditionAttribute Then
                ChangesMadeAttribute = value
            Else
                ChangesMadeAttribute = False
            End If
        End Set
    End Property


    Public Property ChangeSampleTypeDuringEdition() As Boolean
        Get
            Return ChangeSampleTypeDuringEditionAttribute
        End Get
        Set(ByVal value As Boolean)
            ChangeSampleTypeDuringEditionAttribute = value
        End Set
    End Property

    'JV 23/01/2014 #1013
    Public WriteOnly Property UserLevel() As String
        Set(ByVal value As String)
            user = value
        End Set
    End Property
    'JV 23/01/2014 #1013

#End Region

#Region "Enumerates"
    'Enumerate for the Preloaded lists containing available Genders and Age Units
    Private Enum RefPreloadedMasterDataEnum
        AGE_UNITS
        SEX_LIST
    End Enum

    'Enumerate for Limits that are set for screen controls
    Private Enum RefFieldLimitsEnum
        AGE_FROM_TO_DAYS
        AGE_FROM_TO_MONTHS
        AGE_FROM_TO_YEARS
        REF_RANGE_LIMITS
    End Enum

    'Enumerate for Messages than can be shown when validating the data entered in the control
    Private Enum RefMessages
        DETAIL_REF_OVERWRITE
        DELETE_CONFIRMATION
        DUPLICATED_RANGE_BY_GENDER
        MIN_MUST_BE_LOWER_THAN_MAX
        REQUIRED_VALUE
        SHOW_MESSAGE_TITLE_TEXT
        WRONG_RANGE_LIMITS
        WRONG_AGE_LIMITS
    End Enum
#End Region

#Region "Declarations"
    Public RangeType As String                           'Type of selected Range: Generic / Detailed
    Private DetailedRangeType As String = String.Empty   'Type of selected Detailed Range: By Gender / By Age / By Gender and Age

    'Framework Data
    Private OSDecimalSeparator As String
    Private AllFieldLimitsDS As FieldLimitsDS
    Private AllMessagesDS As MessagesDS
    Private GendersMasterDataDS As PreloadedMasterDataDS
    Private AgeUnitsMasterDataDS As PreloadedMasterDataDS
    Private DetailedRangesSubTypesDS As PreloadedMasterDataDS
    'Dim RowHeaderClicked As Boolean = False
#End Region

#Region "Public Methods"
    ''' <summary>
    ''' Loads all the needed framework data from the parent form
    ''' </summary>
    ''' <param name="pFieldLimitsDS">Typed DataSet FieldLimitsDS containing all Field Limits defined in BD</param>
    ''' <param name="pSexListDS">Typed DataSet PreloadedMasterDataDS containing the list of available Genders</param>
    ''' <param name="pAgeUnitsDS">Typed DataSet PreloadedMasterDataDS containing the list of available Age Units</param>
    ''' <param name="pDetailedSubTypesDS">Typed DataSet PreloadedMasterDataDS containing the list of SubTypes for Detailed Ranges</param>
    ''' <param name="pMessagesDS">Typed DataSet MessagesDS containing all Messages defined in BD</param>
    ''' <param name="pOSDecimalSeparator">Character used as decimal separator</param>
    Public Sub LoadFrameworkData(ByVal pFieldLimitsDS As FieldLimitsDS, ByVal pSexListDS As PreloadedMasterDataDS, _
                                 ByVal pAgeUnitsDS As PreloadedMasterDataDS, ByVal pDetailedSubTypesDS As PreloadedMasterDataDS, _
                                 ByVal pMessagesDS As MessagesDS, ByVal pOSDecimalSeparator As String)
        'Try
        AllFieldLimitsDS = pFieldLimitsDS
        GendersMasterDataDS = pSexListDS
        AgeUnitsMasterDataDS = pAgeUnitsDS
        AllMessagesDS = pMessagesDS
        OSDecimalSeparator = pOSDecimalSeparator

        If (Not pDetailedSubTypesDS Is Nothing) Then
            DetailedRangesSubTypesDS = pDetailedSubTypesDS

            Dim qSubTypes As List(Of PreloadedMasterDataDS.tfmwPreloadedMasterDataRow)
            qSubTypes = (From a In DetailedRangesSubTypesDS.tfmwPreloadedMasterData _
                     Order By a.Position _
                       Select a).ToList()

            bsDetailedSubTypesComboBox.DataSource = qSubTypes
            bsDetailedSubTypesComboBox.DisplayMember = "FixedItemDesc"
            bsDetailedSubTypesComboBox.ValueMember = "ItemID"
        End If

        PrepareControls()
        'Catch ex As Exception
        '    Throw ex
        'End Try
    End Sub

    ''' <summary>
    ''' Erases all the data loaded in the control and clears the controls
    ''' </summary>
    Public Sub ClearReferenceRanges()
        'Try
        'Generic Area...
        bsGenericCheckBox.Checked = False

        bsRefNormalLowerLimitUpDown.Value = bsRefNormalLowerLimitUpDown.Minimum
        bsRefNormalUpperLimitUpDown.Value = bsRefNormalUpperLimitUpDown.Minimum
        bsRefNormalLowerLimitUpDown.ResetText()
        bsRefNormalUpperLimitUpDown.ResetText()

        bsRefBorderLineLowerLimitUpDown.Value = bsRefBorderLineLowerLimitUpDown.Minimum
        bsRefBorderLineUpperLimitUpDown.Value = bsRefBorderLineUpperLimitUpDown.Minimum
        bsRefBorderLineLowerLimitUpDown.ResetText()
        bsRefBorderLineUpperLimitUpDown.ResetText()

        'Detailed Area...
        bsDetailedCheckBox.Checked = False

        DetailedRangeType = String.Empty
        bsDetailedSubTypesComboBox.SelectedIndex = -1
        bsRefDetailDataGridView.Rows.Clear()

        'Clean the Error Provider and disable all Controls
        ValidationErrorAttribute = False
        bsControlErrorProvider.Clear()
        DisableControls("ALL")
        'Catch ex As Exception
        '    Throw ex
        'End Try
    End Sub

    ''' <summary>
    ''' Load controls with the Ranges currently defined for the specified Test
    ''' </summary>
    Public Sub LoadReferenceRanges()
        'Try
        ClearReferenceRanges()
        bsGenericCheckBox.Checked = (String.Compare(ActiveRangeTypeAttribute, "GENERIC", False) = 0)
        bsDetailedCheckBox.Checked = (String.Compare(ActiveRangeTypeAttribute, "DETAILED", False) = 0)


        If Not SelectedTestRefRangesDS Is Nothing Then 'RH 05/06/2012
            If (SelectedTestRefRangesDS.tparTestRefRanges.Rows.Count > 0) Then
                For Each rangeRow As TestRefRangesDS.tparTestRefRangesRow In SelectedTestRefRangesDS.tparTestRefRanges.Rows
                    If (String.Compare(rangeRow.RangeType, "GENERIC", False) = 0) Then
                        'Normality Range
                        If (rangeRow.IsNormalLowerLimitNull) Then
                            bsRefNormalLowerLimitUpDown.Value = bsRefNormalLowerLimitUpDown.Minimum
                            bsRefNormalLowerLimitUpDown.ResetText()
                        Else
                            bsRefNormalLowerLimitUpDown.Text = CDec(rangeRow.NormalLowerLimit).ToString()
                        End If
                        If (rangeRow.IsNormalUpperLimitNull) Then
                            bsRefNormalUpperLimitUpDown.Value = bsRefNormalUpperLimitUpDown.Minimum
                            bsRefNormalUpperLimitUpDown.ResetText()
                        Else
                            bsRefNormalUpperLimitUpDown.Text = CDec(rangeRow.NormalUpperLimit).ToString()
                        End If

                        'Borderline Range
                        If (rangeRow.IsBorderLineLowerLimitNull) Then
                            bsRefBorderLineLowerLimitUpDown.Value = bsRefBorderLineLowerLimitUpDown.Minimum
                            bsRefBorderLineLowerLimitUpDown.ResetText()
                        Else
                            bsRefBorderLineLowerLimitUpDown.Text = CDec(rangeRow.BorderLineLowerLimit).ToString()
                        End If
                        If (rangeRow.IsBorderLineUpperLimitNull) Then
                            bsRefBorderLineUpperLimitUpDown.Value = bsRefBorderLineUpperLimitUpDown.Minimum
                            bsRefBorderLineUpperLimitUpDown.ResetText()
                        Else
                            bsRefBorderLineUpperLimitUpDown.Text = CDec(rangeRow.BorderLineUpperLimit).ToString()
                        End If
                    Else
                        'Detailed Reference Ranges
                        Dim r As Integer = bsRefDetailDataGridView.Rows.Add()

                        If (rangeRow.IsRangeIDNull) Then
                            bsRefDetailDataGridView.Rows(r).Cells("RangeID").Value = Nothing
                        Else
                            bsRefDetailDataGridView.Rows(r).Cells("RangeID").Value = rangeRow.RangeID
                        End If
                        If (rangeRow.IsGenderNull) Then
                            bsRefDetailDataGridView.Rows(r).Cells("GenderDesc").Value = "EMPTY"
                        Else
                            bsRefDetailDataGridView.Rows(r).Cells("GenderDesc").Value = rangeRow.Gender
                        End If
                        If (rangeRow.IsAgeUnitNull) Then
                            bsRefDetailDataGridView.Rows(r).Cells("AgeUnitDesc").Value = "EMPTY"
                        Else
                            bsRefDetailDataGridView.Rows(r).Cells("AgeUnitDesc").Value = rangeRow.AgeUnit
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

                        'Set the type of detailed Reference Range (just once; all detailed Ranges have the same pattern)
                        If (String.Compare(DetailedRangeType, String.Empty, False) = 0) Then
                            If (Not rangeRow.IsGenderNull And Not rangeRow.IsAgeUnitNull) Then
                                DetailedRangeType = "GENDER_AGE"
                            ElseIf (Not rangeRow.IsGenderNull) Then
                                DetailedRangeType = "GENDER"
                            ElseIf (Not rangeRow.IsAgeUnitNull) Then
                                DetailedRangeType = "AGE"
                            End If
                        End If
                    End If
                Next

                'Set value of the current Detailed Ranges SubType (if any)
                If (String.Compare(DetailedRangeType, String.Empty, False) = 0) Then
                    bsDetailedSubTypesComboBox.SelectedIndex = -1
                Else
                    bsDetailedSubTypesComboBox.SelectedValue = DetailedRangeType
                    ChangeDetailedSubType()
                End If
            End If
        End If

        If (Not EditionModeAttribute) Then DisableControls("ALL")
        UnselectDetailedRanges()

        ChangesMade = False
        'Catch ex As Exception
        '    Throw ex
        'End Try
    End Sub

    ''' <summary>
    ''' Validates data informed for Generic Reference Ranges
    ''' </summary>
    Public Sub ValidateRefRangesLimits(ByVal pMandatory As Boolean, Optional ByVal pSaving As Boolean = False)
        'Try
        If (bsGenericCheckBox.Checked Or pSaving) Then
            'Validation of values entered for Generic Ranges
            ValidateLimitsUpDown(bsRefNormalLowerLimitUpDown, bsRefNormalUpperLimitUpDown, pMandatory)

            If (Not ValidationErrorAttribute) Then
                If (String.Compare(bsRefNormalLowerLimitUpDown.Text, String.Empty, False) <> 0 And String.Compare(bsRefNormalUpperLimitUpDown.Text, String.Empty, False) <> 0) Then
                    If (String.Compare(TestTypeAttribute, "STD", False) = 0) Then
                        ValidateLimitsUpDown(bsRefBorderLineLowerLimitUpDown, bsRefBorderLineUpperLimitUpDown, False)
                        'TR 05/06/2012 -New implementation BorderLine Min < Normality Min < Normality Max < BorderLine Max.
                        If (String.Compare(bsRefBorderLineLowerLimitUpDown.Text, String.Empty, False) <> 0 And String.Compare(bsRefBorderLineUpperLimitUpDown.Text, String.Empty, False) <> 0) Then
                            If (bsRefNormalLowerLimitUpDown.Value <= bsRefBorderLineLowerLimitUpDown.Value) Then
                                bsControlErrorProvider.SetError(bsRefNormalLowerLimitUpDown, GetMessageText(RefMessages.WRONG_RANGE_LIMITS.ToString))
                                bsControlErrorProvider.SetError(bsRefBorderLineLowerLimitUpDown, GetMessageText(RefMessages.WRONG_RANGE_LIMITS.ToString))
                                ValidationErrorAttribute = True
                            End If
                            If (bsRefBorderLineLowerLimitUpDown.Value >= bsRefNormalUpperLimitUpDown.Value) Then
                                bsControlErrorProvider.SetError(bsRefNormalUpperLimitUpDown, GetMessageText(RefMessages.WRONG_RANGE_LIMITS.ToString))
                                bsControlErrorProvider.SetError(bsRefBorderLineLowerLimitUpDown, GetMessageText(RefMessages.WRONG_RANGE_LIMITS.ToString))
                                ValidationErrorAttribute = True
                            End If
                            If (bsRefBorderLineUpperLimitUpDown.Value <= bsRefNormalUpperLimitUpDown.Value) Then
                                bsControlErrorProvider.SetError(bsRefNormalUpperLimitUpDown, GetMessageText(RefMessages.WRONG_RANGE_LIMITS.ToString))
                                bsControlErrorProvider.SetError(bsRefBorderLineUpperLimitUpDown, GetMessageText(RefMessages.WRONG_RANGE_LIMITS.ToString))
                                ValidationErrorAttribute = True
                            End If
                        End If
                    End If
                End If

                If (Not ValidationErrorAttribute) Then bsControlErrorProvider.Clear()
            End If
        End If

        If (bsDetailedCheckBox.Checked Or pSaving) Then
            'Validation of values entered for Detailed Ranges (missing values and/or range limits)
            Dim rowWithErrors As Boolean = False
            For i As Integer = 0 To bsRefDetailDataGridView.Rows.Count - 2
                If (ValidateDetailedRow(i)) Then
                    'If at least one of the rows have errors, this variable is set to True
                    rowWithErrors = True
                End If
            Next
            If (Not ValidationErrorAttribute) Then ValidationErrorAttribute = rowWithErrors

            If (Not rowWithErrors) Then
                'Validation of not duplicated nor overlapped Detailed Ranges
                For i As Integer = 0 To bsRefDetailDataGridView.Rows.Count - 2
                    If (ValidateDetailedRanges(i)) Then
                        'If at least one of the rows have errors, this variable is set to True
                        rowWithErrors = True
                    End If
                Next
                If (Not ValidationErrorAttribute) Then ValidationErrorAttribute = rowWithErrors
            End If
        End If
        'Catch ex As Exception
        '    Throw ex
        'End Try
    End Sub

    ''' <summary>
    ''' If there are Reference Ranges defined for an OFF-SYSTEM Test and the Sample Type has been changed,
    ''' then the new SampleType is informed for all Reference Ranges
    ''' </summary>
    Public Sub ChangeSampleType()
        'Try
        If (String.Compare(TestTypeAttribute, "OFFS", False) = 0 AndAlso Not SelectedTestRefRangesDS Is Nothing) Then
            For Each row As TestRefRangesDS.tparTestRefRangesRow In SelectedTestRefRangesDS.tparTestRefRanges.Rows
                row.SampleType = SampleTypeAttribute
            Next
        End If
        'Catch ex As Exception
        '    Throw ex
        'End Try
    End Sub

    ''' <summary>
    ''' Erases all the data loaded in the control and clears the controls
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub ClearData()
        'Try
        'CalcTestID = 0
        RangeType = String.Empty
        MeasureUnit = String.Empty

        bsGenericCheckBox.Checked = False
        bsDetailedCheckBox.Checked = False

        If (Not EditionModeAttribute) Then DisableControls("ALL")
        ClearReferenceRangesControls()
        'Catch ex As Exception
        '    Throw ex
        'End Try
    End Sub

#End Region

#Region "Private Methods"
    ''' <summary>
    ''' Manages the availability of the controls depending on which RadioButton is selected
    ''' and depending on if the control is in edition mode or not
    ''' </summary>
    Private Sub CheckBoxManagement()
        'Try
        If (EditionModeAttribute) Then
            If (bsGenericCheckBox.Checked) Then
                'Enable all controls used for Generic Reference Ranges 
                EnableControls("GENERIC")

                'Disable all controls used for Detailed Reference Ranges (excepting the 
                'correspondent option button)
                DisableControls("DETAILED")
                bsDetailedCheckBox.Enabled = True

            ElseIf (bsDetailedCheckBox.Checked) Then
                'Enable all controls used for Detailed Reference Ranges. 
                EnableControls("DETAILED")

                'Disable all controls used for Generic Reference Ranges (excepting the 
                'correspondent option button)
                DisableControls("GENERIC")
                bsGenericCheckBox.Enabled = True

            Else
                'Disable all controls excepting the option buttons that allow selecting the Range Type
                DisableControls("ALL", True)

                'bsGenericCheckBox.Enabled = True
                'bsDetailedCheckBox.Enabled = True
            End If
        Else
            'Disable all controls 
            DisableControls("ALL")
        End If

        'Set the enabled/disabled BackColor to all rows loaded in the grid of Detailed Reference Ranges
        UnselectDetailedRanges()
        'Catch ex As Exception
        '    Throw ex
        'End Try
    End Sub

    ''' <summary>
    ''' Unselect all rows in the DataGrid of Detailed Reference Ranges updating also the style of them
    ''' </summary>
    Private Sub UnselectDetailedRanges()
        'Try
        If (bsRefDetailDataGridView.Rows.Count > 0) Then
            For r As Integer = 0 To bsRefDetailDataGridView.Rows.Count - 1 Step 1
                bsRefDetailDataGridView.Rows(r).Selected = False

                If (bsRefDetailDataGridView.Enabled) Then
                    bsRefDetailDataGridView.Rows(r).DefaultCellStyle.BackColor = Color.White
                    bsRefDetailDataGridView.Rows(r).DefaultCellStyle.ForeColor = Color.Black
                Else
                    bsRefDetailDataGridView.Rows(r).DefaultCellStyle.BackColor = SystemColors.MenuBar
                    bsRefDetailDataGridView.Rows(r).DefaultCellStyle.ForeColor = Color.DarkGray
                End If
            Next
        End If
        'Catch ex As Exception
        '    Throw ex
        'End Try
    End Sub

    ''' <summary>
    ''' Change the grid layout when the SubType of Detailed Reference Ranges is changed
    ''' </summary>
    Private Sub ChangeDetailedSubType()
        'Try
        Dim fullLayout As Boolean = True

        If (bsDetailedSubTypesComboBox.SelectedIndex = -1) Then
            DetailedRangeType = String.Empty
        Else
            Dim changeSubType As Boolean = True
            If (String.Compare(DetailedRangeType, bsDetailedSubTypesComboBox.SelectedValue.ToString, False) <> 0) And _
               (String.Compare(DetailedRangeType, String.Empty, False) <> 0) And (bsRefDetailDataGridView.Rows.Count > 1) Then
                'Warning: detailed Reference Ranges currently loaded in the grid will be deleted
                If (ShowMessage(GetMessageText(RefMessages.SHOW_MESSAGE_TITLE_TEXT.ToString), _
                                RefMessages.DETAIL_REF_OVERWRITE.ToString) = Windows.Forms.DialogResult.Yes) Then
                    bsRefDetailDataGridView.Rows.Clear()

                    'Mark to delete all the previous Detailed Ranges
                    Dim qAllTestRefRanges As New List(Of TestRefRangesDS.tparTestRefRangesRow)
                    qAllTestRefRanges = (From a In SelectedTestRefRangesDS.tparTestRefRanges _
                                        Where String.Compare(a.RangeType, "DETAILED", False) = 0 _
                                          And String.Compare(a.SampleType, SampleTypeAttribute, False) = 0 _
                                       Select a).ToList()

                    For Each refRange As TestRefRangesDS.tparTestRefRangesRow In qAllTestRefRanges
                        refRange.IsDeleted = True
                    Next
                    SelectedTestRefRangesDS.tparTestRefRanges.AcceptChanges()
                Else
                    changeSubType = False
                    fullLayout = False

                    bsDetailedSubTypesComboBox.SelectedValue = DetailedRangeType
                End If
            End If

            If (changeSubType) Then
                DetailedRangeType = bsDetailedSubTypesComboBox.SelectedValue.ToString

                Select Case (bsDetailedSubTypesComboBox.SelectedValue.ToString)
                    Case "GENDER"
                        fullLayout = False
                        bsRefDetailDataGridView.Columns("GenderDesc").Width = 238
                        bsRefDetailDataGridView.Columns("GenderDesc").Visible = True

                        bsRefDetailDataGridView.Columns("AgeUnitDesc").Visible = False
                        bsRefDetailDataGridView.Columns("AgeRangeFrom").Visible = False
                        bsRefDetailDataGridView.Columns("AgeRangeTo").Visible = False

                        bsRefDetailDataGridView.Columns("NormalLowerLimit").Width = 145 'Dl 12/11/2012
                        bsRefDetailDataGridView.Columns("NormalUpperLimit").Width = 145 'Dl 12/11/2012

                    Case "AGE"
                        fullLayout = False
                        bsRefDetailDataGridView.Columns("GenderDesc").Visible = False

                        bsRefDetailDataGridView.Columns("AgeUnitDesc").Width = 130
                        bsRefDetailDataGridView.Columns("AgeRangeFrom").Width = 95
                        bsRefDetailDataGridView.Columns("AgeRangeTo").Width = 95

                        bsRefDetailDataGridView.Columns("AgeUnitDesc").Visible = True
                        bsRefDetailDataGridView.Columns("AgeRangeFrom").Visible = True
                        bsRefDetailDataGridView.Columns("AgeRangeTo").Visible = True

                        bsRefDetailDataGridView.Columns("NormalLowerLimit").Width = 105 'DL 12/11/2012
                        bsRefDetailDataGridView.Columns("NormalUpperLimit").Width = 105 'DL 12/11/2012
                End Select

                If (EditionModeAttribute) Then ChangesMade = True
            End If
        End If

        If (fullLayout) Then
            bsRefDetailDataGridView.Columns("GenderDesc").Width = 95 'dl 12/11/2012 100 '120 JB 09/11/2012
            bsRefDetailDataGridView.Columns("GenderDesc").Visible = True

            bsRefDetailDataGridView.Columns("AgeUnitDesc").Width = 95 'dl 12/11/2012 100 '110 JB 09/11/2012
            bsRefDetailDataGridView.Columns("AgeRangeFrom").Width = 64 '62 DL 12/11/2012 '70 JB 09/11/2012
            bsRefDetailDataGridView.Columns("AgeRangeTo").Width = 64 '62 DL 12/11/2012 '70 JB 09/11/2012

            bsRefDetailDataGridView.Columns("AgeUnitDesc").Visible = True
            bsRefDetailDataGridView.Columns("AgeRangeFrom").Visible = True
            bsRefDetailDataGridView.Columns("AgeRangeTo").Visible = True

            bsRefDetailDataGridView.Columns("NormalLowerLimit").Width = 106 '75 JB 09/11/2012
            bsRefDetailDataGridView.Columns("NormalUpperLimit").Width = 106 '75 JB 09/11/2012
        End If
        'Catch ex As Exception
        '    Throw ex
        'End Try
    End Sub

    ''' <summary>
    ''' Initialization of all controls
    ''' </summary>
    Private Sub PrepareControls()
        'Try
        'Area of generic Reference Ranges
        bsGenericCheckBox.Checked = False

        bsRefNormalLowerLimitUpDown.Value = bsRefNormalLowerLimitUpDown.Minimum
        bsRefNormalUpperLimitUpDown.Value = bsRefNormalUpperLimitUpDown.Minimum
        bsRefNormalLowerLimitUpDown.ResetText()
        bsRefNormalUpperLimitUpDown.ResetText()

        bsRefBorderLineLowerLimitUpDown.Value = bsRefBorderLineLowerLimitUpDown.Minimum
        bsRefBorderLineUpperLimitUpDown.Value = bsRefBorderLineUpperLimitUpDown.Minimum
        bsRefBorderLineLowerLimitUpDown.ResetText()
        bsRefBorderLineUpperLimitUpDown.ResetText()

        'Area of detailed Reference Ranges
        bsDetailedCheckBox.Checked = False

        bsRefDetailDataGridView.Rows.Clear()
        bsRefDetailDataGridView.Columns.Clear()
        bsRefDetailDataGridView.EditMode = DataGridViewEditMode.EditOnEnter

        Dim columnName As String
        Dim numLimits(4) As Integer

        'Gender Column (ComboBox)
        Dim comboBoxCol1 As New DataGridViewComboBoxColumn

        columnName = "GenderDesc"
        comboBoxCol1.DataPropertyName = columnName
        comboBoxCol1.Name = columnName
        comboBoxCol1.HeaderText = GenderColumnTextAttribute
        comboBoxCol1.DataSource = PrepareGenderList()
        comboBoxCol1.DisplayMember = "FixedItemDesc"
        comboBoxCol1.ValueMember = "ItemID"

        bsRefDetailDataGridView.Columns.Add(comboBoxCol1)
        bsRefDetailDataGridView.Columns(columnName).Width = 100 '120 JB - 09/11/2012
        bsRefDetailDataGridView.Columns(columnName).ReadOnly = False

        'Age Units Column (ComboBox)
        Dim comboBoxCol2 As New DataGridViewComboBoxColumn

        columnName = "AgeUnitDesc"
        comboBoxCol2.DataPropertyName = columnName
        comboBoxCol2.Name = columnName
        comboBoxCol2.HeaderText = AgeUnitColumnTextAttribute
        comboBoxCol2.DataSource = PrepareAgeUnitsList()
        comboBoxCol2.DisplayMember = "FixedItemDesc"
        comboBoxCol2.ValueMember = "ItemID"

        bsRefDetailDataGridView.Columns.Add(comboBoxCol2)
        bsRefDetailDataGridView.Columns(columnName).Width = 100 '110 JB - 09/11/2012
        bsRefDetailDataGridView.Columns(columnName).ReadOnly = False

        'Age From column (NumericUpDown)
        Dim ageFromNumUpDown As New DataGridViewNumericUpDownColumn
        numLimits = GetNumReplicatesLimits(RefFieldLimitsEnum.AGE_FROM_TO_YEARS.ToString)

        columnName = "AgeRangeFrom"
        ageFromNumUpDown.DataPropertyName = columnName
        ageFromNumUpDown.Name = columnName
        ageFromNumUpDown.HeaderText = AgeFromColumnTextAttribute
        ageFromNumUpDown.Minimum = numLimits(0)
        ageFromNumUpDown.Maximum = numLimits(1)
        ageFromNumUpDown.DecimalPlaces = numLimits(3)
        ageFromNumUpDown.Increment = numLimits(4)

        bsRefDetailDataGridView.Columns.Add(ageFromNumUpDown)
        bsRefDetailDataGridView.Columns(columnName).HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight
        bsRefDetailDataGridView.Columns(columnName).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
        bsRefDetailDataGridView.Columns(columnName).Width = 65 'dl 12/11/2012 '62 '75 JB - 09/11/2012
        bsRefDetailDataGridView.Columns(columnName).ReadOnly = False

        'Age To column (NumericUpDown)
        Dim ageToNumUpDown As New DataGridViewNumericUpDownColumn

        columnName = "AgeRangeTo"
        ageToNumUpDown.DataPropertyName = columnName
        ageToNumUpDown.Name = columnName
        ageToNumUpDown.HeaderText = AgeToColumnTextAttribute
        ageToNumUpDown.Minimum = numLimits(0)
        ageToNumUpDown.Maximum = numLimits(1)
        ageToNumUpDown.DecimalPlaces = numLimits(3)
        ageToNumUpDown.Increment = numLimits(4)

        bsRefDetailDataGridView.Columns.Add(ageToNumUpDown)
        bsRefDetailDataGridView.Columns(columnName).HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight
        bsRefDetailDataGridView.Columns(columnName).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
        bsRefDetailDataGridView.Columns(columnName).Width = 65 'dl 12/11/2012 ' 62 '75 JB - 09/11/2012
        bsRefDetailDataGridView.Columns(columnName).ReadOnly = False

        'Normal Lower Limit (NumericUpDown)
        Dim lowRangeNumUpDown As New DataGridViewNumericUpDownColumn
        numLimits = GetNumReplicatesLimits(RefFieldLimitsEnum.REF_RANGE_LIMITS.ToString)

        columnName = "NormalLowerLimit"
        lowRangeNumUpDown.DataPropertyName = columnName
        lowRangeNumUpDown.Name = columnName
        lowRangeNumUpDown.HeaderText = LowerColumnTextAttribute
        lowRangeNumUpDown.Minimum = numLimits(0)
        lowRangeNumUpDown.Maximum = numLimits(1)
        lowRangeNumUpDown.DecimalPlaces = NumAllowedDecimalsAttribute
        lowRangeNumUpDown.Increment = numLimits(4)

        bsRefDetailDataGridView.Columns.Add(lowRangeNumUpDown)
        bsRefDetailDataGridView.Columns(columnName).HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight
        bsRefDetailDataGridView.Columns(columnName).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
        bsRefDetailDataGridView.Columns(columnName).Width = 106 '79 JB - 09/11/2012
        bsRefDetailDataGridView.Columns(columnName).ReadOnly = False

        'Normal Upper Limit (NumericUpDown)
        Dim upperRangeNumUpDown As New DataGridViewNumericUpDownColumn

        columnName = "NormalUpperLimit"
        upperRangeNumUpDown.DataPropertyName = columnName
        upperRangeNumUpDown.Name = columnName
        upperRangeNumUpDown.HeaderText = UpperColumnTextAttribute
        upperRangeNumUpDown.Minimum = numLimits(0)
        upperRangeNumUpDown.Maximum = numLimits(1)
        upperRangeNumUpDown.DecimalPlaces = NumAllowedDecimalsAttribute
        upperRangeNumUpDown.Increment = numLimits(4)

        bsRefDetailDataGridView.Columns.Add(upperRangeNumUpDown)
        bsRefDetailDataGridView.Columns(columnName).HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight
        bsRefDetailDataGridView.Columns(columnName).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
        bsRefDetailDataGridView.Columns(columnName).Width = 106 '79 JB - 09/11/2012
        bsRefDetailDataGridView.Columns(columnName).ReadOnly = False

        'Hidden columns
        '...Sample Type 
        columnName = "SampleType"
        bsRefDetailDataGridView.Columns.Add(columnName, String.Empty)
        bsRefDetailDataGridView.Columns(columnName).Width = 0
        bsRefDetailDataGridView.Columns(columnName).Visible = False
        bsRefDetailDataGridView.Columns(columnName).DataPropertyName = columnName
        bsRefDetailDataGridView.Columns(columnName).ReadOnly = True

        '...Range Identifier
        columnName = "RangeID"
        bsRefDetailDataGridView.Columns.Add(columnName, String.Empty)
        bsRefDetailDataGridView.Columns(columnName).Width = 0
        bsRefDetailDataGridView.Columns(columnName).Visible = False
        bsRefDetailDataGridView.Columns(columnName).DataPropertyName = columnName
        bsRefDetailDataGridView.Columns(columnName).ReadOnly = True

        '...Added Range Flag
        columnName = "Added"
        bsRefDetailDataGridView.Columns.Add(columnName, String.Empty)
        bsRefDetailDataGridView.Columns(columnName).Width = 0
        bsRefDetailDataGridView.Columns(columnName).Visible = False
        bsRefDetailDataGridView.Columns(columnName).DataPropertyName = columnName
        bsRefDetailDataGridView.Columns(columnName).ReadOnly = True
        'Catch ex As Exception
        '    Throw ex
        'End Try
    End Sub

    ''' <summary>
    ''' Get all limit values defined for the specified Limit Identifier
    ''' </summary>
    ''' <param name="pLimitID">Limit Identifier</param>
    ''' <returns>Array of Integers containing each one of the values for the informed limit
    '''          (Min, Max, Decimals Allowed, Step and Default Value)</returns>
    Private Function GetNumReplicatesLimits(ByVal pLimitID As String) As Integer()
        Dim resultData(4) As Integer
        'Try
        Dim qValues As List(Of FieldLimitsDS.tfmwFieldLimitsRow)
        qValues = (From a In AllFieldLimitsDS.tfmwFieldLimits _
                  Where String.Compare(a.LimitID, pLimitID, False) = 0 _
                 Select a).ToList

        If (qValues.Count = 1) Then
            'Return the different limits values in each array position
            resultData(0) = CInt(qValues(0).MinValue)
            resultData(1) = CInt(qValues(0).MaxValue)

            If (Not qValues(0).IsDefaultValueNull) Then
                resultData(2) = CInt(qValues(0).DefaultValue)
            End If

            resultData(3) = CInt(qValues(0).DecimalsAllowed)

            If (Not qValues(0).IsStepValueNull) Then
                resultData(4) = CInt(qValues(0).StepValue)
            End If
        End If
        'Catch ex As Exception
        '    Throw ex
        'End Try
        Return resultData
    End Function

    ''' <summary>
    ''' Add an empty row to the list of available Genders
    ''' </summary>
    Private Function PrepareGenderList() As List(Of PreloadedMasterDataDS.tfmwPreloadedMasterDataRow)
        Dim qGender As List(Of PreloadedMasterDataDS.tfmwPreloadedMasterDataRow)
        'Try
        'Create a row in Genders DS containing an empty item
        Dim emptyRow As DataRow = GendersMasterDataDS.tfmwPreloadedMasterData.NewRow
        emptyRow(0) = RefPreloadedMasterDataEnum.SEX_LIST.ToString
        emptyRow(1) = "EMPTY"
        emptyRow(2) = String.Empty
        emptyRow(3) = String.Empty
        emptyRow(4) = 0
        emptyRow(5) = True
        emptyRow(6) = String.Empty
        GendersMasterDataDS.tfmwPreloadedMasterData.Rows.Add(emptyRow)

        'Sort the list of Genders by position and assign it as DataSource of the Gender column in the grid 
        qGender = (From a In GendersMasterDataDS.tfmwPreloadedMasterData _
               Order By a.Position _
                 Select a).ToList()
        'Catch ex As Exception
        '    Throw ex
        'End Try
        Return qGender
    End Function

    ''' <summary>
    ''' Add an empty row to the list of available Age Units
    ''' </summary>
    ''' <returns></returns>
    Private Function PrepareAgeUnitsList() As List(Of PreloadedMasterDataDS.tfmwPreloadedMasterDataRow)
        Dim qAgeUnits As List(Of PreloadedMasterDataDS.tfmwPreloadedMasterDataRow)
        'Try
        'Create a row in Age Units DS containing an empty item
        Dim emptyRow As DataRow = AgeUnitsMasterDataDS.tfmwPreloadedMasterData.NewRow
        emptyRow(0) = RefPreloadedMasterDataEnum.AGE_UNITS.ToString
        emptyRow(1) = "EMPTY"
        emptyRow(2) = String.Empty
        emptyRow(3) = String.Empty
        emptyRow(4) = 0
        emptyRow(5) = True
        emptyRow(6) = String.Empty
        AgeUnitsMasterDataDS.tfmwPreloadedMasterData.Rows.Add(emptyRow)

        'Sort the list of age Units by position and assign it as DataSource of the Age Unit column in the grid 
        qAgeUnits = (From a In AgeUnitsMasterDataDS.tfmwPreloadedMasterData _
                 Order By a.Position _
                   Select a).ToList()
        'Catch ex As Exception
        '    Throw ex
        'End Try
        Return qAgeUnits
    End Function

    ''' <summary>
    ''' Enables all Reference Range controls
    ''' </summary>
    Private Sub EnableControls(ByVal pArea As String)
        'Try
        If (String.Compare(pArea, "DETAILED", False) <> 0) Then
            bsGenericCheckBox.Enabled = True
            'JV 23/01/2014 #1013
            'bsRefNormalLowerLimitUpDown.Enabled = True
            'bsRefNormalUpperLimitUpDown.Enabled = True
            bsRefNormalLowerLimitUpDown.Enabled = user <> "OPERATOR"
            bsRefNormalUpperLimitUpDown.Enabled = user <> "OPERATOR"
            'JV 23/01/2014 #1013
            bsRefNormalLowerLimitUpDown.BackColor = Color.White
            bsRefNormalUpperLimitUpDown.BackColor = Color.White

            If (String.Compare(bsRefNormalLowerLimitUpDown.Text, String.Empty, False) <> 0) Then
                bsRefBorderLineLowerLimitUpDown.Enabled = True
                bsRefBorderLineUpperLimitUpDown.Enabled = True

                bsRefBorderLineLowerLimitUpDown.BackColor = Color.White
                bsRefBorderLineUpperLimitUpDown.BackColor = Color.White
            End If
        End If

        If (String.Compare(pArea, "GENERIC", False) <> 0) Then
            bsDetailedCheckBox.Enabled = True

            If (bsDetailedSubTypesComboBox.SelectedIndex = -1) Then bsDetailedSubTypesComboBox.SelectedIndex = 2
            bsDetailedSubTypesComboBox.BackColor = Color.White
            bsDetailedSubTypesComboBox.Enabled = True
            bsRefDetailDeleteButton.Enabled = True

            bsRefDetailDataGridView.Enabled = True
            bsRefDetailDataGridView.ReadOnly = False
            bsRefDetailDataGridView.AllowUserToAddRows = True
            'bsRefDetailDataGridView.AllowUserToDeleteRows = True

            For i As Integer = 0 To bsRefDetailDataGridView.Rows.Count - 2
                bsRefDetailDataGridView.Rows(i).ReadOnly = False
                bsRefDetailDataGridView.Rows(i).Cells("GenderDesc").ReadOnly = False

                bsRefDetailDataGridView.Rows(i).Cells("AgeUnitDesc").ReadOnly = False
                bsRefDetailDataGridView.Rows(i).Cells("AgeRangeFrom").ReadOnly = (bsRefDetailDataGridView.Rows(i).Cells("AgeUnitDesc").Value Is Nothing OrElse _
                                                                                  String.Compare(bsRefDetailDataGridView.Rows(i).Cells("AgeUnitDesc").Value.ToString, "EMPTY", False) = 0)
                bsRefDetailDataGridView.Rows(i).Cells("AgeRangeTo").ReadOnly = (bsRefDetailDataGridView.Rows(i).Cells("AgeUnitDesc").Value Is Nothing OrElse _
                                                                                String.Compare(bsRefDetailDataGridView.Rows(i).Cells("AgeUnitDesc").Value.ToString, "EMPTY", False) = 0)

                bsRefDetailDataGridView.Rows(i).Cells("NormalLowerLimit").ReadOnly = False
                bsRefDetailDataGridView.Rows(i).Cells("NormalUpperLimit").ReadOnly = False
            Next
        End If
        'Catch ex As Exception
        '    Throw ex
        'End Try
    End Sub

    ''' <summary>
    ''' Disables all Reference Range controls or the controls in the specified area (Generic/Detailed)
    ''' </summary>
    Private Sub DisableControls(ByVal pArea As String, Optional ByVal pCheckBoxEnable As Boolean = False)
        'Try
        If (String.Compare(pArea, "DETAILED", False) <> 0) Then
            bsGenericCheckBox.Enabled = pCheckBoxEnable

            bsRefNormalLowerLimitUpDown.Enabled = False
            bsRefNormalUpperLimitUpDown.Enabled = False
            bsRefBorderLineLowerLimitUpDown.Enabled = False
            bsRefBorderLineUpperLimitUpDown.Enabled = False

            bsRefNormalLowerLimitUpDown.BackColor = SystemColors.MenuBar
            bsRefNormalUpperLimitUpDown.BackColor = SystemColors.MenuBar
            bsRefBorderLineLowerLimitUpDown.BackColor = SystemColors.MenuBar
            bsRefBorderLineUpperLimitUpDown.BackColor = SystemColors.MenuBar
        End If

        If (String.Compare(pArea, "GENERIC", False) <> 0) Then
            bsDetailedCheckBox.Enabled = pCheckBoxEnable

            If (bsRefDetailDataGridView.RowCount = 0) Then bsDetailedSubTypesComboBox.SelectedIndex = -1
            bsDetailedSubTypesComboBox.BackColor = SystemColors.MenuBar
            bsDetailedSubTypesComboBox.Enabled = False
            bsRefDetailDeleteButton.Enabled = False

            bsRefDetailDataGridView.AllowUserToAddRows = False
            bsRefDetailDataGridView.AllowUserToDeleteRows = False
            bsRefDetailDataGridView.Enabled = False
            bsRefDetailDataGridView.ReadOnly = True
        End If
        'Catch ex As Exception
        '    Throw ex
        'End Try
    End Sub

    ''' <summary>
    ''' Auxiliary function used to shown Messages that have to be shown as a Dialog
    ''' </summary>
    ''' <remarks>
    ''' Modified by XB 04/02/2013 - Upper conversions redundants because the value is already in UpperCase must delete to avoid Regional Settings problems (Bugs tracking #1112)
    ''' </remarks>
    Public Function ShowMessage(ByVal pWindowTitle As String, ByVal pMessageID As String, Optional ByVal pSystemMessageText As String = "") As DialogResult
        Dim result As New DialogResult

        'Try
        result = Windows.Forms.DialogResult.No
        If (AllMessagesDS.tfmwMessages.Rows.Count > 0) Then
            Dim msgText As String = String.Empty
            Dim msgType As String = String.Empty

            Dim qMessages As New List(Of MessagesDS.tfmwMessagesRow)
            qMessages = (From a In AllMessagesDS.tfmwMessages _
                        Where String.Compare(a.MessageID, pMessageID, False) = 0 _
                       Select a).ToList()
            'Where String.Compare(a.MessageID.ToUpper, pMessageID.ToUpper, False) = 0 _

            If (qMessages.Count > 0) Then
                msgText = qMessages.First.MessageText
                msgType = qMessages.First.MessageType
            End If

            'Additional text to add to the Message Text
            If (String.Compare(pSystemMessageText, String.Empty, False) <> 0) Then msgText = msgText & " - " & pSystemMessageText

            'Show message with the proper icon according the Message Type
            If (String.Compare(msgType, "Error", False) = 0) Then
                'Error Message 
                result = MessageBox.Show(msgText, pWindowTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)

            ElseIf (String.Compare(msgType, "Information", False) = 0) Then
                'Information Message 
                result = MessageBox.Show(msgText, pWindowTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)

            ElseIf (String.Compare(msgType, "FailureAudit", False) = 0) Then
                'System Error Message - FailureAudit
                result = MessageBox.Show(msgText, pWindowTitle, MessageBoxButtons.OK, MessageBoxIcon.Stop)

            ElseIf (String.Compare(msgType, "Warning", False) = 0) Then
                'System Error Message - Warning
                result = MessageBox.Show(msgText, pWindowTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)

            Else
                'Question Message
                result = MessageBox.Show(msgText, pWindowTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question)
            End If
        End If
        'Catch ex As Exception
        '    Throw ex
        'End Try
        Return result
    End Function

    ''' <summary>
    ''' Auxiliary function that returns the multilanguage text for the specified Message Identifier
    ''' </summary>
    ''' <param name="pMessageId">Message Identifier</param>
    ''' <remarks>
    ''' Modified by XB 04/02/2013 - Upper conversions redundants because the value is already in UpperCase must delete to avoid Regional Settings problems (Bugs tracking #1112)
    ''' </remarks>
    Private Function GetMessageText(ByVal pMessageId As String) As String
        Dim textMessage As String = String.Empty
        'Try
        Dim qMessages As New List(Of MessagesDS.tfmwMessagesRow)
        qMessages = (From a In AllMessagesDS.tfmwMessages _
                    Where String.Compare(a.MessageID, pMessageId, False) = 0 _
                   Select a).ToList()
        'Where String.Compare(a.MessageID.ToUpper, pMessageId.ToUpper, False) = 0 _
        If (qMessages.Count > 0) Then textMessage = qMessages.First.MessageText
        'Catch ex As Exception
        '    Throw ex
        'End Try
        Return textMessage
    End Function

    ''' <summary>
    ''' Handle for NumericUpDown Columns that do not allow decimals nor negative values in the grid of detailed Reference Ranges
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 13/12/2010
    ''' </remarks>
    Private Sub editingGridUpDown_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs)
        'Try
        If (bsRefDetailDataGridView.CurrentCell.ColumnIndex = 2 OrElse _
            bsRefDetailDataGridView.CurrentCell.ColumnIndex = 3) Then
            If (String.Compare(e.KeyChar.ToString(), ".".ToString(), False) = 0 Or String.Compare(e.KeyChar.ToString(), ",".ToString(), False) = 0 Or String.Compare(e.KeyChar.ToString(), "'".ToString(), False) = 0 Or String.Compare(e.KeyChar.ToString(), "-".ToString(), False) = 0) Then
                e.Handled = True
            End If
            'TR 01/08/2011 -Validate columns that allow decimals to set the decimal separator.
        ElseIf bsRefDetailDataGridView.CurrentCell.ColumnIndex = 4 OrElse _
                                    bsRefDetailDataGridView.CurrentCell.ColumnIndex = 5 Then
            If (String.Compare(e.KeyChar.ToString(), ".".ToString(), False) = 0 Or String.Compare(e.KeyChar.ToString(), ",".ToString(), False) = 0 Or String.Compare(e.KeyChar.ToString(), "'".ToString(), False) = 0) Then
                e.KeyChar = CChar(OSDecimalSeparator)
                If (CType(sender, NumericUpDown).Text.Contains(".") Or CType(sender, NumericUpDown).Text.Contains(",")) Then
                    e.Handled = True
                End If
            End If
        End If

        'TR 28/07/2011 -Set the change made if is in edition mode
        If EditionModeAttribute And Not ChangesMade Then
            ChangesMade = True
        End If
        'TR 28/07/2011 -END.

        'Catch ex As Exception
        '    Throw ex
        'End Try
    End Sub

    ''' <summary>
    ''' Handle for ComboBox Columns in the grid of detailed Reference Ranges
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 13/12/2010
    ''' </remarks>
    Private Sub editingGridComboBox_SelectionChangeCommitted(ByVal sender As Object, ByVal e As System.EventArgs)
        'Try
        Dim myComboBoxCell As ComboBox = CType(sender, ComboBox)
        If (myComboBoxCell.SelectedIndex > -1) Then
            Dim currentRow As Integer = bsRefDetailDataGridView.CurrentCell.RowIndex
            Dim currentCol As Integer = bsRefDetailDataGridView.CurrentCell.ColumnIndex

            If (currentCol = 0) Then
                If (myComboBoxCell.SelectedIndex > 0) Then
                    bsRefDetailDataGridView.Rows(currentRow).Cells("GenderDesc").ErrorText = String.Empty
                End If

            ElseIf (currentCol = 1) Then
                If (myComboBoxCell.SelectedIndex = 0) Then
                    'If not AgeUnit is selected, then it is not possible to inform the Age range 
                    bsRefDetailDataGridView.Rows(currentRow).Cells("AgeRangeFrom").ErrorText = String.Empty
                    bsRefDetailDataGridView.Rows(currentRow).Cells("AgeRangeFrom").Value = Nothing
                    bsRefDetailDataGridView.Rows(currentRow).Cells("AgeRangeFrom").ReadOnly = True

                    bsRefDetailDataGridView.Rows(currentRow).Cells("AgeRangeTo").ErrorText = String.Empty
                    bsRefDetailDataGridView.Rows(currentRow).Cells("AgeRangeTo").Value = Nothing
                    bsRefDetailDataGridView.Rows(currentRow).Cells("AgeRangeTo").ReadOnly = True
                Else
                    bsRefDetailDataGridView.Rows(currentRow).Cells("AgeUnitDesc").ErrorText = String.Empty
                    bsRefDetailDataGridView.Rows(currentRow).Cells("AgeRangeFrom").ErrorText = String.Empty
                    bsRefDetailDataGridView.Rows(currentRow).Cells("AgeRangeTo").ErrorText = String.Empty

                    'Get the limit values for AgeRange fields depending on the selected AgeUnit 
                    Dim numLimits(4) As Integer
                    Select Case (myComboBoxCell.SelectedValue.ToString)
                        Case "Y"
                            numLimits = GetNumReplicatesLimits(RefFieldLimitsEnum.AGE_FROM_TO_YEARS.ToString)
                        Case "M"
                            numLimits = GetNumReplicatesLimits(RefFieldLimitsEnum.AGE_FROM_TO_MONTHS.ToString)
                        Case "D"
                            numLimits = GetNumReplicatesLimits(RefFieldLimitsEnum.AGE_FROM_TO_DAYS.ToString)
                    End Select

                    DirectCast(bsRefDetailDataGridView.Rows(currentRow).Cells("AgeRangeFrom"), DataGridViewNumericUpDownCell).Minimum = numLimits(0)
                    DirectCast(bsRefDetailDataGridView.Rows(currentRow).Cells("AgeRangeFrom"), DataGridViewNumericUpDownCell).Maximum = numLimits(1)
                    DirectCast(bsRefDetailDataGridView.Rows(currentRow).Cells("AgeRangeFrom"), DataGridViewNumericUpDownCell).DecimalPlaces = numLimits(3)
                    DirectCast(bsRefDetailDataGridView.Rows(currentRow).Cells("AgeRangeFrom"), DataGridViewNumericUpDownCell).Increment = numLimits(4)

                    DirectCast(bsRefDetailDataGridView.Rows(currentRow).Cells("AgeRangeTo"), DataGridViewNumericUpDownCell).Minimum = numLimits(0)
                    DirectCast(bsRefDetailDataGridView.Rows(currentRow).Cells("AgeRangeTo"), DataGridViewNumericUpDownCell).Maximum = numLimits(1)
                    DirectCast(bsRefDetailDataGridView.Rows(currentRow).Cells("AgeRangeTo"), DataGridViewNumericUpDownCell).DecimalPlaces = numLimits(3)
                    DirectCast(bsRefDetailDataGridView.Rows(currentRow).Cells("AgeRangeTo"), DataGridViewNumericUpDownCell).Increment = numLimits(4)
                End If
            End If

            'TR 28/07/2011 -Set the change made if is in edition mode
            If EditionModeAttribute And Not ChangesMade Then
                ChangesMade = True
            End If
            'TR 28/07/2011 -END.
        End If
        'Catch ex As Exception
        '    Throw ex
        'End Try
    End Sub

    Private Sub editingGridUpDown_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)
        'Try
        'TR 28/07/2011 -Set the change made if is in edition mode
        If EditionModeAttribute And Not ChangesMade Then
            ChangesMade = True
        End If
        'TR 28/07/2011 -END.
        'Catch ex As Exception
        '    Throw ex
        'End Try
    End Sub


    ''' <summary>
    ''' Verify the range values informed in the specified controls are correct
    ''' </summary>
    ''' <param name="pLimitUpdown1">Minimum limit BSNumericUpDown to evaluate</param>
    ''' <param name="pLimitUpdown2">Maximum limit BSNumericUpDown to evaluate</param>
    ''' <param name="pMandatory">Indicate if the specified controls are mandatory</param>
    Private Sub ValidateLimitsUpDown(ByRef pLimitUpdown1 As BSNumericUpDown, ByRef pLimitUpdown2 As BSNumericUpDown, ByVal pMandatory As Boolean)
        'Try
        If (pMandatory) Then
            If (String.Compare(pLimitUpdown1.Text, String.Empty, False) = 0 And pLimitUpdown2.Text = String.Empty) Then
                'Error --> Required  values not informed...
                bsControlErrorProvider.SetError(pLimitUpdown1, GetMessageText(RefMessages.REQUIRED_VALUE.ToString))
                bsControlErrorProvider.SetError(pLimitUpdown2, GetMessageText(RefMessages.REQUIRED_VALUE.ToString))
                ValidationErrorAttribute = True
            End If
        End If

        If (String.Compare(pLimitUpdown1.Text, String.Empty, False) <> 0 Or String.Compare(pLimitUpdown2.Text, String.Empty, False) <> 0) Then
            If (String.Compare(pLimitUpdown1.Text, String.Empty, False) = 0 And String.Compare(pLimitUpdown2.Text, String.Empty, False) <> 0) Then
                'Error --> Lower limit not informed
                bsControlErrorProvider.SetError(pLimitUpdown1, GetMessageText(RefMessages.REQUIRED_VALUE.ToString))
                ValidationErrorAttribute = True
            End If

            If (String.Compare(pLimitUpdown2.Text, String.Empty, False) = 0 And pLimitUpdown1.Text <> String.Empty) Then
                'Error --> Upper limit not informed
                bsControlErrorProvider.SetError(pLimitUpdown2, GetMessageText(RefMessages.REQUIRED_VALUE.ToString))
                ValidationErrorAttribute = True
            End If

            If (pLimitUpdown2.Text <> String.Empty And pLimitUpdown1.Text <> String.Empty) Then
                If (CDbl(pLimitUpdown1.Text) > CDbl(pLimitUpdown2.Text)) Then
                    'Error --> Lower limit greater than the Upper one
                    bsControlErrorProvider.SetError(pLimitUpdown1, GetMessageText(RefMessages.MIN_MUST_BE_LOWER_THAN_MAX.ToString))
                    ValidationErrorAttribute = True
                End If
                If (CDbl(pLimitUpdown1.Text) = CDbl(pLimitUpdown2.Text)) Then
                    'Error --> Lower limit is equal to Upper limit
                    bsControlErrorProvider.SetError(pLimitUpdown1, GetMessageText(RefMessages.MIN_MUST_BE_LOWER_THAN_MAX.ToString))
                    ValidationErrorAttribute = True
                End If
            End If
        End If
        'Catch ex As Exception
        '    Throw ex
        'End Try
    End Sub

    ''' <summary>
    ''' Update the DataSet to return (SelectedTestRefRangesDS) with all defined Reference Ranges
    ''' </summary>
    Private Sub UpdateReferenceRanges()
        'Try
        If (Not ValidationErrorAttribute) Then
            'If it is a new Test (TestID is not informed), then all Ranges are new
            If (TestIDAttribute = -1) Then
                'If (bsRefNormalLowerLimitUpDown.Text.ToString <> String.Empty And _
                '    bsRefNormalUpperLimitUpDown.Text.ToString <> String.Empty) Then
                If (String.Compare(bsRefNormalLowerLimitUpDown.Text, String.Empty, False) <> 0 AndAlso _
                    String.Compare(bsRefNormalUpperLimitUpDown.Text, String.Empty, False) <> 0) Then

                    'Add the GENERIC Range to the DS
                    Dim testRefRangesRow As TestRefRangesDS.tparTestRefRangesRow
                    testRefRangesRow = SelectedTestRefRangesDS.tparTestRefRanges.NewtparTestRefRangesRow()

                    If AddGenericRangeToDS_NEW(testRefRangesRow) Then
                        SelectedTestRefRangesDS.tparTestRefRanges.AddtparTestRefRangesRow(testRefRangesRow)
                    End If
                End If

                'Add to the final DataSet all detailed Ranges contained in the DataGridView
                For i As Integer = 0 To bsRefDetailDataGridView.Rows.Count - 2
                    Dim testRefRangesRow As TestRefRangesDS.tparTestRefRangesRow
                    testRefRangesRow = SelectedTestRefRangesDS.tparTestRefRanges.NewtparTestRefRangesRow()
                    'DL 16/10/2012. BEGIN 
                    If AddDetailedRangeToDS_NEW(testRefRangesRow, i) Then
                        SelectedTestRefRangesDS.tparTestRefRanges.AddtparTestRefRangesRow(testRefRangesRow)
                    End If
                    'DL 16/10/2012. END
                Next
            Else
                'Updating Reference Ranges of an existing Test
                'Search if there is a Generic Range in the DS 
                Dim qGenericRange As New List(Of TestRefRangesDS.tparTestRefRangesRow)
                qGenericRange = (From a In SelectedTestRefRangesDS.tparTestRefRanges _
                                Where String.Compare(a.RangeType, "GENERIC", False) = 0 _
                                  And String.Compare(a.SampleType, SampleTypeAttribute, False) = 0 _
                               Select a).ToList()

                If (qGenericRange.Count = 0) Then
                    If (String.Compare(bsRefNormalLowerLimitUpDown.Text, String.Empty, False) <> 0 And _
                        bsRefNormalUpperLimitUpDown.Text <> String.Empty) Then
                        'Add the GENERIC Range to the DS
                        Dim testRefRangesRow As TestRefRangesDS.tparTestRefRangesRow
                        testRefRangesRow = SelectedTestRefRangesDS.tparTestRefRanges.NewtparTestRefRangesRow()

                        'DL 16/10/2012. BEGIN
                        If AddGenericRangeToDS_NEW(testRefRangesRow) Then
                            SelectedTestRefRangesDS.tparTestRefRanges.AddtparTestRefRangesRow(testRefRangesRow)
                        End If
                        'DL 16/10/2012. END
                    End If
                Else
                    'A Generic Reference Range is in the DS. Update values or mark it as deleted
                    If (String.Compare(bsRefNormalLowerLimitUpDown.Text, String.Empty, False) <> 0 And _
                        bsRefNormalUpperLimitUpDown.Text <> String.Empty) Then
                        'Update values of the Normality Range
                        qGenericRange(0).NormalLowerLimit = CDec(bsRefNormalLowerLimitUpDown.Text)
                        qGenericRange(0).NormalUpperLimit = CDec(bsRefNormalUpperLimitUpDown.Text)

                        'Update values of the BorderLine Range if they are informed
                        If (TestTypeAttribute = "STD") Then
                            If (bsRefBorderLineLowerLimitUpDown.Text <> String.Empty And _
                                bsRefBorderLineUpperLimitUpDown.Text <> String.Empty) Then
                                qGenericRange(0).BorderLineLowerLimit = CDec(bsRefBorderLineLowerLimitUpDown.Text)
                                qGenericRange(0).BorderLineUpperLimit = CDec(bsRefBorderLineUpperLimitUpDown.Text)
                            Else
                                qGenericRange(0).SetBorderLineLowerLimitNull()
                                qGenericRange(0).SetBorderLineUpperLimitNull()
                            End If
                        End If
                    Else
                        'Mark the GENERIC Range as deleted
                        qGenericRange(0).IsDeleted = True
                    End If
                End If

                'Process detailed Reference Ranges
                Dim rowsToIgnore As Integer = 1
                If (bsDetailedCheckBox.Checked) Then rowsToIgnore = 2

                For i As Integer = 0 To bsRefDetailDataGridView.Rows.Count - rowsToIgnore
                    If (bsRefDetailDataGridView.Rows(i).Cells("RangeID").Value Is Nothing) Then
                        'New Range; add it to the DS
                        Dim testRefRangesRow As TestRefRangesDS.tparTestRefRangesRow
                        testRefRangesRow = SelectedTestRefRangesDS.tparTestRefRanges.NewtparTestRefRangesRow()

                        If AddDetailedRangeToDS_NEW(testRefRangesRow, i) Then
                            SelectedTestRefRangesDS.tparTestRefRanges.AddtparTestRefRangesRow(testRefRangesRow)
                        End If

                    Else
                        'Existing Range; search it in the DS to update the values
                        Dim qDetailedRange As New List(Of TestRefRangesDS.tparTestRefRangesRow)
                        qDetailedRange = (From a In SelectedTestRefRangesDS.tparTestRefRanges _
                                         Where String.Compare(a.RangeType, "DETAILED", False) = 0 _
                                           And Not a.IsRangeIDNull _
                                       AndAlso a.RangeID = CInt(bsRefDetailDataGridView.Rows(i).Cells("RangeID").Value) _
                                        Select a).ToList()

                        If (qDetailedRange.Count = 1) Then
                            If (bsRefDetailDataGridView.Rows(i).Cells("GenderDesc").Value Is Nothing OrElse _
                                bsRefDetailDataGridView.Rows(i).Cells("GenderDesc").Value.ToString = "EMPTY") Then
                                qDetailedRange(0).SetGenderNull()
                            Else
                                qDetailedRange(0).Gender = bsRefDetailDataGridView.Rows(i).Cells("GenderDesc").Value.ToString
                            End If

                            If (bsRefDetailDataGridView.Rows(i).Cells("AgeUnitDesc").Value Is Nothing OrElse _
                                bsRefDetailDataGridView.Rows(i).Cells("AgeUnitDesc").Value.ToString = "EMPTY") Then
                                qDetailedRange(0).SetAgeUnitNull()
                                qDetailedRange(0).SetAgeRangeFromNull()
                                qDetailedRange(0).SetAgeRangeToNull()
                            Else
                                qDetailedRange(0).AgeUnit = bsRefDetailDataGridView.Rows(i).Cells("AgeUnitDesc").Value.ToString
                                qDetailedRange(0).AgeRangeFrom = CInt(bsRefDetailDataGridView.Rows(i).Cells("AgeRangeFrom").FormattedValue)
                                qDetailedRange(0).AgeRangeTo = CInt(bsRefDetailDataGridView.Rows(i).Cells("AgeRangeTo").FormattedValue)
                            End If

                            qDetailedRange(0).NormalLowerLimit = CDec(bsRefDetailDataGridView.Rows(i).Cells("NormalLowerLimit").FormattedValue)
                            qDetailedRange(0).NormalUpperLimit = CDec(bsRefDetailDataGridView.Rows(i).Cells("NormalUpperLimit").FormattedValue)
                        End If
                    End If
                Next
            End If

            bsControlErrorProvider.Clear()
            ValidationErrorAttribute = False
        End If
        'Catch ex As Exception
        '    Throw ex
        'End Try
    End Sub

    ''' <summary>
    ''' Fill fields of a row of a typed Dataset TestRefRangesDS with data of controls for Generic Ranges
    ''' </summary>
    ''' <param name="pTestRefRangesRow">TestRefRangesDS row to fill</param>
    Private Sub AddGenericRangeToDS_old(ByRef pTestRefRangesRow As TestRefRangesDS.tparTestRefRangesRow)
        'Try
        If (TestIDAttribute = -1) Then
            pTestRefRangesRow.SetTestIDNull()
        Else
            pTestRefRangesRow.TestID = TestIDAttribute
        End If

        pTestRefRangesRow.SetRangeIDNull()
        pTestRefRangesRow.TestType = TestTypeAttribute
        pTestRefRangesRow.SampleType = SampleTypeAttribute
        pTestRefRangesRow.RangeType = "GENERIC"
        pTestRefRangesRow.NormalLowerLimit = CDec(bsRefNormalLowerLimitUpDown.Text)
        pTestRefRangesRow.NormalUpperLimit = CDec(bsRefNormalUpperLimitUpDown.Text)

        If (String.Compare(TestTypeAttribute, "STD", False) = 0) Then
            If (String.Compare(bsRefBorderLineLowerLimitUpDown.Text, String.Empty, False) <> 0 And _
                String.Compare(bsRefBorderLineUpperLimitUpDown.Text, String.Empty, False) <> 0) Then
                pTestRefRangesRow.BorderLineLowerLimit = CDec(bsRefBorderLineLowerLimitUpDown.Text)
                pTestRefRangesRow.BorderLineUpperLimit = CDec(bsRefBorderLineUpperLimitUpDown.Text)
            Else
                pTestRefRangesRow.SetBorderLineLowerLimitNull()
                pTestRefRangesRow.SetBorderLineUpperLimitNull()
            End If
        Else
            pTestRefRangesRow.SetBorderLineLowerLimitNull()
            pTestRefRangesRow.SetBorderLineUpperLimitNull()
        End If

        pTestRefRangesRow.SetGenderNull()
        pTestRefRangesRow.SetAgeUnitNull()
        pTestRefRangesRow.SetAgeRangeFromNull()
        pTestRefRangesRow.SetAgeRangeToNull()
        pTestRefRangesRow.IsNew = True
        pTestRefRangesRow.IsDeleted = False
        'Catch ex As Exception
        '    Throw ex
        'End Try
    End Sub

    ''' <summary>
    ''' Fill fields of a row of a typed Dataset TestRefRangesDS with data of controls for Generic Ranges
    ''' </summary>
    ''' <param name="pTestRefRangesRow">TestRefRangesDS row to fill</param>
    ''' <remarks>Solve the problem with duplicates asociated with the ds link with control</remarks> 
    Private Function AddGenericRangeToDS_NEW(ByRef pTestRefRangesRow As TestRefRangesDS.tparTestRefRangesRow) As Boolean
        Dim IsNew As Boolean = True

        'Try
        Dim LINQ As List(Of TestRefRangesDS.tparTestRefRangesRow)
        Dim numRows As Integer = 0 'DL 06/11/2012

        If bsRefBorderLineLowerLimitUpDown.Text = String.Empty AndAlso bsRefBorderLineUpperLimitUpDown.Text = String.Empty _
           AndAlso bsRefNormalLowerLimitUpDown.Text <> String.Empty AndAlso String.Compare(bsRefNormalUpperLimitUpDown.Text, String.Empty, False) <> 0 Then

            LINQ = (From A In SelectedTestRefRangesDS.tparTestRefRanges _
                    Where A.RangeType = "GENERIC" _
                    AndAlso A.SampleType = SampleTypeAttribute AndAlso A.TestType = TestTypeAttribute _
                    AndAlso A.NormalUpperLimit = CDec(bsRefNormalUpperLimitUpDown.Text) _
                    AndAlso A.NormalLowerLimit = CDec(bsRefNormalLowerLimitUpDown.Text) _
                    Select A).ToList

            numRows = LINQ.Count 'DL 06/11/2012

        ElseIf bsRefBorderLineLowerLimitUpDown.Text = String.Empty AndAlso bsRefBorderLineUpperLimitUpDown.Text = String.Empty _
           AndAlso bsRefNormalLowerLimitUpDown.Text <> String.Empty AndAlso bsRefNormalUpperLimitUpDown.Text <> String.Empty Then

            LINQ = (From A In SelectedTestRefRangesDS.tparTestRefRanges _
                    Where A.RangeType = "GENERIC" _
                    AndAlso A.SampleType = SampleTypeAttribute AndAlso A.TestType = TestTypeAttribute _
                    AndAlso A.NormalUpperLimit = CDec(bsRefNormalUpperLimitUpDown.Text) _
                    AndAlso A.NormalLowerLimit = CDec(bsRefNormalLowerLimitUpDown.Text) _
                    AndAlso A.BorderLineLowerLimit = CDec(bsRefBorderLineLowerLimitUpDown.Text) _
                    AndAlso A.BorderLineUpperLimit = CDec(bsRefBorderLineUpperLimitUpDown.Text) _
                    Select A).ToList

            numRows = LINQ.Count 'DL 06/11/2012
        End If

        If numRows > 0 Then    'DL 06/11/2012
            IsNew = False
        Else
            If (TestIDAttribute = -1) Then
                pTestRefRangesRow.SetTestIDNull()
            Else
                pTestRefRangesRow.TestID = TestIDAttribute
            End If

            pTestRefRangesRow.SetRangeIDNull()
            pTestRefRangesRow.TestType = TestTypeAttribute
            pTestRefRangesRow.SampleType = SampleTypeAttribute
            pTestRefRangesRow.RangeType = "GENERIC"
            pTestRefRangesRow.NormalLowerLimit = CDec(bsRefNormalLowerLimitUpDown.Text)
            pTestRefRangesRow.NormalUpperLimit = CDec(bsRefNormalUpperLimitUpDown.Text)

            If (String.Compare(TestTypeAttribute, "STD", False) = 0) Then 'TestTypeAttribute <> "STD"
                If (String.Compare(bsRefBorderLineLowerLimitUpDown.Text, String.Empty, False) <> 0 And _
                    String.Compare(bsRefBorderLineUpperLimitUpDown.Text, String.Empty, False) <> 0) Then
                    pTestRefRangesRow.BorderLineLowerLimit = CDec(bsRefBorderLineLowerLimitUpDown.Text)
                    pTestRefRangesRow.BorderLineUpperLimit = CDec(bsRefBorderLineUpperLimitUpDown.Text)
                Else
                    pTestRefRangesRow.SetBorderLineLowerLimitNull()
                    pTestRefRangesRow.SetBorderLineUpperLimitNull()
                End If
            Else
                pTestRefRangesRow.SetBorderLineLowerLimitNull()
                pTestRefRangesRow.SetBorderLineUpperLimitNull()
            End If

            pTestRefRangesRow.SetGenderNull()
            pTestRefRangesRow.SetAgeUnitNull()
            pTestRefRangesRow.SetAgeRangeFromNull()
            pTestRefRangesRow.SetAgeRangeToNull()
            pTestRefRangesRow.IsNew = True
            pTestRefRangesRow.IsDeleted = False
        End If

        'If SelectedTestRefRangesDS.tparTestRefRanges.Rows.Count = 1 Then IsNew = False

        Return IsNew
        'Catch ex As Exception
        '    Throw ex
        'End Try
    End Function

    ''' <summary>
    ''' Fill fields of a row of a typed Dataset TestRefRangesDS with data contained in an specific row of 
    ''' the DataGridView for DETAILED Ranges
    ''' </summary>
    ''' <param name="pTestRefRangesRow">TestRefRangesDS row to fill</param>
    ''' <param name="pGridRowIndex">Index of the DataGridView row from which the detailed Range is obtained</param>
    Private Sub AddDetailedRangeToDS_old(ByRef pTestRefRangesRow As TestRefRangesDS.tparTestRefRangesRow, ByVal pGridRowIndex As Integer)


        'Try
        If (TestIDAttribute = -1) Then
            pTestRefRangesRow.SetTestIDNull()
        Else
            pTestRefRangesRow.TestID = TestIDAttribute
        End If

        pTestRefRangesRow.SetRangeIDNull()
        pTestRefRangesRow.TestType = TestTypeAttribute
        pTestRefRangesRow.SampleType = SampleTypeAttribute
        pTestRefRangesRow.RangeType = "DETAILED"

        If (bsRefDetailDataGridView.Rows(pGridRowIndex).Cells("GenderDesc").Value Is Nothing OrElse _
            String.Compare(bsRefDetailDataGridView.Rows(pGridRowIndex).Cells("GenderDesc").Value.ToString, "EMPTY", False) = 0) Then
            pTestRefRangesRow.SetGenderNull()
        Else
            pTestRefRangesRow.Gender = bsRefDetailDataGridView.Rows(pGridRowIndex).Cells("GenderDesc").Value.ToString
        End If

        If (bsRefDetailDataGridView.Rows(pGridRowIndex).Cells("AgeUnitDesc").Value Is Nothing OrElse _
            String.Compare(bsRefDetailDataGridView.Rows(pGridRowIndex).Cells("AgeUnitDesc").Value.ToString, "EMPTY", False) = 0) Then
            pTestRefRangesRow.SetAgeUnitNull()
            pTestRefRangesRow.SetAgeRangeFromNull()
            pTestRefRangesRow.SetAgeRangeToNull()
        Else
            pTestRefRangesRow.AgeUnit = bsRefDetailDataGridView.Rows(pGridRowIndex).Cells("AgeUnitDesc").Value.ToString
            pTestRefRangesRow.AgeRangeFrom = CInt(bsRefDetailDataGridView.Rows(pGridRowIndex).Cells("AgeRangeFrom").FormattedValue)
            pTestRefRangesRow.AgeRangeTo = CInt(bsRefDetailDataGridView.Rows(pGridRowIndex).Cells("AgeRangeTo").FormattedValue)
        End If

        pTestRefRangesRow.NormalLowerLimit = CDec(bsRefDetailDataGridView.Rows(pGridRowIndex).Cells("NormalLowerLimit").FormattedValue)
        pTestRefRangesRow.NormalUpperLimit = CDec(bsRefDetailDataGridView.Rows(pGridRowIndex).Cells("NormalUpperLimit").FormattedValue)

        pTestRefRangesRow.SetBorderLineLowerLimitNull()
        pTestRefRangesRow.SetBorderLineUpperLimitNull()
        pTestRefRangesRow.IsNew = True
        pTestRefRangesRow.IsDeleted = False
        'Catch ex As Exception
        '    Throw ex
        'End Try
    End Sub


    ''' <summary>
    ''' Fill fields of a row of a typed Dataset TestRefRangesDS with data contained in an specific row of 
    ''' the DataGridView for DETAILED Ranges
    ''' </summary>
    ''' <param name="pTestRefRangesRow">TestRefRangesDS row to fill</param>
    ''' <param name="pGridRowIndex">Index of the DataGridView row from which the detailed Range is obtained</param>
    ''' <remarks>dl 16/10/2012 Solve the problem with duplicates asociated with the ds link with control</remarks> 
    Private Function AddDetailedRangeToDS_NEW(ByRef pTestRefRangesRow As TestRefRangesDS.tparTestRefRangesRow, ByVal pGridRowIndex As Integer) As Boolean
        Dim IsNew As Boolean = True

        'Try
        If (TestIDAttribute = -1) Then
            pTestRefRangesRow.SetTestIDNull()
        Else
            pTestRefRangesRow.TestID = TestIDAttribute
        End If

        pTestRefRangesRow.SetRangeIDNull()
        pTestRefRangesRow.TestType = TestTypeAttribute
        pTestRefRangesRow.SampleType = SampleTypeAttribute
        pTestRefRangesRow.RangeType = "DETAILED"

        If (bsRefDetailDataGridView.Rows(pGridRowIndex).Cells("GenderDesc").Value Is Nothing OrElse _
            String.Compare(bsRefDetailDataGridView.Rows(pGridRowIndex).Cells("GenderDesc").Value.ToString, "EMPTY", False) = 0) Then
            pTestRefRangesRow.SetGenderNull()
        Else
            pTestRefRangesRow.Gender = bsRefDetailDataGridView.Rows(pGridRowIndex).Cells("GenderDesc").Value.ToString
        End If

        If (bsRefDetailDataGridView.Rows(pGridRowIndex).Cells("AgeUnitDesc").Value Is Nothing OrElse _
            String.Compare(bsRefDetailDataGridView.Rows(pGridRowIndex).Cells("AgeUnitDesc").Value.ToString, "EMPTY", False) = 0) Then
            pTestRefRangesRow.SetAgeUnitNull()
            pTestRefRangesRow.SetAgeRangeFromNull()
            pTestRefRangesRow.SetAgeRangeToNull()
        Else
            pTestRefRangesRow.AgeUnit = bsRefDetailDataGridView.Rows(pGridRowIndex).Cells("AgeUnitDesc").Value.ToString
            pTestRefRangesRow.AgeRangeFrom = CInt(bsRefDetailDataGridView.Rows(pGridRowIndex).Cells("AgeRangeFrom").FormattedValue)
            pTestRefRangesRow.AgeRangeTo = CInt(bsRefDetailDataGridView.Rows(pGridRowIndex).Cells("AgeRangeTo").FormattedValue)
        End If

        pTestRefRangesRow.NormalLowerLimit = CDec(bsRefDetailDataGridView.Rows(pGridRowIndex).Cells("NormalLowerLimit").FormattedValue)
        pTestRefRangesRow.NormalUpperLimit = CDec(bsRefDetailDataGridView.Rows(pGridRowIndex).Cells("NormalUpperLimit").FormattedValue)

        pTestRefRangesRow.SetBorderLineLowerLimitNull()
        pTestRefRangesRow.SetBorderLineUpperLimitNull()
        pTestRefRangesRow.IsNew = True
        pTestRefRangesRow.IsDeleted = False


        'BETA DL 16/10/2012 DUPLICATE ROWS
        Dim LINQ As List(Of TestRefRangesDS.tparTestRefRangesRow)
        Dim IsDuplicated As Boolean = False

        Select Case DetailedRangeType
            Case "GENDER_AGE"

                ' AndAlso A.TestID = TestIDAttribute _
                LINQ = (From A In SelectedTestRefRangesDS.tparTestRefRanges _
                        Where A.RangeType = "DETAILED" _
                        AndAlso A.SampleType = SampleTypeAttribute AndAlso A.TestType = TestTypeAttribute _
                        AndAlso A.Gender = bsRefDetailDataGridView.Rows(pGridRowIndex).Cells("GenderDesc").Value.ToString _
                        AndAlso A.AgeUnit = bsRefDetailDataGridView.Rows(pGridRowIndex).Cells("AgeUnitDesc").Value.ToString _
                        AndAlso A.AgeRangeFrom = CInt(bsRefDetailDataGridView.Rows(pGridRowIndex).Cells("AgeRangeFrom").FormattedValue) _
                        AndAlso A.AgeRangeTo = CInt(bsRefDetailDataGridView.Rows(pGridRowIndex).Cells("AgeRangeTo").FormattedValue) _
                        AndAlso A.NormalUpperLimit = CDec(bsRefDetailDataGridView.Rows(pGridRowIndex).Cells("NormalUpperLimit").FormattedValue) _
                        AndAlso A.NormalLowerLimit = CDec(bsRefDetailDataGridView.Rows(pGridRowIndex).Cells("NormalLowerLimit").FormattedValue) _
                        Select A).ToList

                If LINQ.Count > 0 Then IsDuplicated = True

            Case "AGE"
                ' AndAlso A.TestID = TestIDAttribute _
                LINQ = (From A In SelectedTestRefRangesDS.tparTestRefRanges _
                        Where A.RangeType = "DETAILED" _
                        AndAlso A.SampleType = SampleTypeAttribute AndAlso A.TestType = TestTypeAttribute _
                        AndAlso A.AgeUnit = bsRefDetailDataGridView.Rows(pGridRowIndex).Cells("AgeUnitDesc").Value.ToString _
                        AndAlso A.AgeRangeFrom = CInt(bsRefDetailDataGridView.Rows(pGridRowIndex).Cells("AgeRangeFrom").FormattedValue) _
                        AndAlso A.AgeRangeTo = CInt(bsRefDetailDataGridView.Rows(pGridRowIndex).Cells("AgeRangeTo").FormattedValue) _
                        AndAlso A.NormalUpperLimit = CDec(bsRefDetailDataGridView.Rows(pGridRowIndex).Cells("NormalUpperLimit").FormattedValue) _
                        AndAlso A.NormalLowerLimit = CDec(bsRefDetailDataGridView.Rows(pGridRowIndex).Cells("NormalLowerLimit").FormattedValue) _
                        Select A).ToList

                If LINQ.Count > 0 Then IsDuplicated = True

            Case "GENDER"
                'AndAlso A.TestID = TestIDAttribute
                LINQ = (From A In SelectedTestRefRangesDS.tparTestRefRanges _
                        Where A.RangeType = "DETAILED" _
                        AndAlso A.SampleType = SampleTypeAttribute AndAlso A.TestType = TestTypeAttribute _
                        AndAlso A.Gender = bsRefDetailDataGridView.Rows(pGridRowIndex).Cells("GenderDesc").Value.ToString _
                        AndAlso A.NormalUpperLimit = CDec(bsRefDetailDataGridView.Rows(pGridRowIndex).Cells("NormalUpperLimit").FormattedValue) _
                        AndAlso A.NormalLowerLimit = CDec(bsRefDetailDataGridView.Rows(pGridRowIndex).Cells("NormalLowerLimit").FormattedValue) _
                        Select A).ToList

                If LINQ.Count > 0 Then IsDuplicated = True

        End Select

        If IsDuplicated Then IsNew = False

        Return IsNew
        'Catch ex As Exception
        '    Throw ex
        'End Try
    End Function

    ''' <summary>
    ''' Delete all detailed Reference Ranges cuurently selected in the grid
    ''' </summary>
    Private Sub DeleteSelectedDetailedRanges()
        'Try

        If (bsRefDetailDataGridView.SelectedRows.Count = 1 AndAlso Not bsRefDetailDataGridView.SelectedRows(0).IsNewRow) OrElse _
                                                                                      bsRefDetailDataGridView.SelectedRows.Count > 1 Then

            If (ShowMessage(GetMessageText(RefMessages.SHOW_MESSAGE_TITLE_TEXT.ToString), _
                            RefMessages.DELETE_CONFIRMATION.ToString) = Windows.Forms.DialogResult.Yes) Then
                For Each row As System.Windows.Forms.DataGridViewRow In bsRefDetailDataGridView.SelectedRows
                    If (row.Index <> bsRefDetailDataGridView.Rows.Count - 1) Then
                        If (Not row.Cells("RangeID").Value Is Nothing) Then
                            'It is an existing Range; mark it to delete 
                            Dim qTestRefRanges As New List(Of TestRefRangesDS.tparTestRefRangesRow)
                            qTestRefRanges = (From a In SelectedTestRefRangesDS.tparTestRefRanges _
                                                Where String.Compare(a.RangeType, "DETAILED", False) = 0 _
                                                  And String.Compare(a.SampleType, SampleTypeAttribute, False) = 0 _
                                                  And a.RangeID = CInt(row.Cells("RangeID").Value) _
                                               Select a).ToList()

                            If (qTestRefRanges.Count = 1) Then
                                qTestRefRanges(0).IsNew = False
                                qTestRefRanges(0).IsDeleted = True
                            End If
                            SelectedTestRefRangesDS.tparTestRefRanges.AcceptChanges()
                        End If

                        'Now delete the row in the grid
                        bsRefDetailDataGridView.Rows.Remove(row)
                        ChangesMade = True
                    End If
                Next

                'Validation of values entered for Detailed Ranges (missing values and/or range limits)
                Dim rowWithErrors As Boolean = False
                For i As Integer = 0 To bsRefDetailDataGridView.Rows.Count - 2
                    If (ValidateDetailedRow(i)) Then
                        'If at least one of the rows have errors, this variable is set to True
                        rowWithErrors = True
                    End If
                Next
                If (Not ValidationErrorAttribute) Then ValidationErrorAttribute = rowWithErrors

                If (Not rowWithErrors) Then
                    For i As Integer = 0 To bsRefDetailDataGridView.Rows.Count - 2
                        If (ValidateDetailedRanges(i)) Then
                            'If at least one of the rows have errors, this variable is set to True
                            rowWithErrors = True
                        End If
                    Next
                    If (Not ValidationErrorAttribute) Then ValidationErrorAttribute = rowWithErrors
                End If
            End If
        End If

        If (bsRefDetailDataGridView.SelectedRows.Count = 1 AndAlso bsRefDetailDataGridView.SelectedRows(0).IsNewRow) Then
            ValidationErrorAttribute = False
        End If

        'Catch ex As Exception
        '    Throw ex
        'End Try
    End Sub

    ''' <summary>
    ''' Validate all fields are correctly informed in a row of the grid for detailed Ranges
    ''' (missing values and/or range limits)
    ''' </summary>
    ''' <param name="pCurrentRow">Index of the grid row to validate</param>
    Private Function ValidateDetailedRow(ByVal pCurrentRow As Integer) As Boolean
        Dim rowWithErrors As Boolean = False
        'Try
        Dim errorFound As Boolean = False
        If (String.Compare(DetailedRangeType, "GENDER", False) = 0 Or String.Compare(DetailedRangeType, "GENDER_AGE", False) = 0) Then
            If (bsRefDetailDataGridView.Rows(pCurrentRow).Cells("GenderDesc").Value Is Nothing OrElse _
                String.Compare(bsRefDetailDataGridView.Rows(pCurrentRow).Cells("GenderDesc").Value.ToString, "EMPTY", False) = 0) Then
                rowWithErrors = True
                bsRefDetailDataGridView.Rows(pCurrentRow).Cells("GenderDesc").ErrorText = GetMessageText(RefMessages.REQUIRED_VALUE.ToString)
            End If
        End If

        If (String.Compare(DetailedRangeType, "AGE", False) = 0 Or String.Compare(DetailedRangeType, "GENDER_AGE", False) = 0) Then
            If (bsRefDetailDataGridView.Rows(pCurrentRow).Cells("AgeUnitDesc").Value Is Nothing OrElse _
                String.Compare(bsRefDetailDataGridView.Rows(pCurrentRow).Cells("AgeUnitDesc").Value.ToString, "EMPTY", False) = 0) Then
                rowWithErrors = True
                bsRefDetailDataGridView.Rows(pCurrentRow).Cells("AgeUnitDesc").ErrorText = GetMessageText(RefMessages.REQUIRED_VALUE.ToString)
            Else
                'Clear previous errors...
                bsRefDetailDataGridView.Rows(pCurrentRow).Cells("AgeRangeFrom").ErrorText = String.Empty
                bsRefDetailDataGridView.Rows(pCurrentRow).Cells("AgeRangeFrom").Style.Alignment = DataGridViewContentAlignment.MiddleRight

                bsRefDetailDataGridView.Rows(pCurrentRow).Cells("AgeRangeTo").ErrorText = String.Empty
                bsRefDetailDataGridView.Rows(pCurrentRow).Cells("AgeRangeTo").Style.Alignment = DataGridViewContentAlignment.MiddleRight

                'If an Age Unit was selected then the From/To values have to be informed and From < To
                If (bsRefDetailDataGridView.Rows(pCurrentRow).Cells("AgeRangeFrom").Value Is Nothing) Then
                    errorFound = True
                    rowWithErrors = True
                    bsRefDetailDataGridView.Rows(pCurrentRow).Cells("AgeRangeFrom").Style.Alignment = DataGridViewContentAlignment.MiddleCenter
                    bsRefDetailDataGridView.Rows(pCurrentRow).Cells("AgeRangeFrom").ErrorText = GetMessageText(RefMessages.REQUIRED_VALUE.ToString)
                End If
                If (bsRefDetailDataGridView.Rows(pCurrentRow).Cells("AgeRangeTo").Value Is Nothing) Then
                    errorFound = True
                    rowWithErrors = True
                    bsRefDetailDataGridView.Rows(pCurrentRow).Cells("AgeRangeTo").Style.Alignment = DataGridViewContentAlignment.MiddleCenter
                    bsRefDetailDataGridView.Rows(pCurrentRow).Cells("AgeRangeTo").ErrorText = GetMessageText(RefMessages.REQUIRED_VALUE.ToString)
                End If

                If (Not errorFound) Then
                    If (CInt(bsRefDetailDataGridView.Rows(pCurrentRow).Cells("AgeRangeFrom").Value.ToString) >= _
                        CInt(bsRefDetailDataGridView.Rows(pCurrentRow).Cells("AgeRangeTo").Value.ToString)) Then
                        errorFound = True
                        rowWithErrors = True
                        bsRefDetailDataGridView.Rows(pCurrentRow).Cells("AgeRangeFrom").Style.Alignment = DataGridViewContentAlignment.MiddleCenter
                        bsRefDetailDataGridView.Rows(pCurrentRow).Cells("AgeRangeFrom").ErrorText = GetMessageText(RefMessages.MIN_MUST_BE_LOWER_THAN_MAX.ToString)
                    End If
                End If
            End If
        End If

        'Clear previous errors...
        errorFound = False
        bsRefDetailDataGridView.Rows(pCurrentRow).Cells("NormalLowerLimit").ErrorText = String.Empty
        bsRefDetailDataGridView.Rows(pCurrentRow).Cells("NormalLowerLimit").Style.Alignment = DataGridViewContentAlignment.MiddleRight

        bsRefDetailDataGridView.Rows(pCurrentRow).Cells("NormalUpperLimit").ErrorText = String.Empty
        bsRefDetailDataGridView.Rows(pCurrentRow).Cells("NormalUpperLimit").Style.Alignment = DataGridViewContentAlignment.MiddleRight

        'Normal Limits have to be informed and LowerLimit < UpperLimit
        If (bsRefDetailDataGridView.Rows(pCurrentRow).Cells("NormalLowerLimit").Value Is Nothing) Then
            errorFound = True
            rowWithErrors = True
            bsRefDetailDataGridView.Rows(pCurrentRow).Cells("NormalLowerLimit").Style.Alignment = DataGridViewContentAlignment.MiddleCenter
            bsRefDetailDataGridView.Rows(pCurrentRow).Cells("NormalLowerLimit").ErrorText = GetMessageText(RefMessages.REQUIRED_VALUE.ToString)
        End If
        If (bsRefDetailDataGridView.Rows(pCurrentRow).Cells("NormalUpperLimit").Value Is Nothing) Then
            errorFound = True
            rowWithErrors = True
            bsRefDetailDataGridView.Rows(pCurrentRow).Cells("NormalUpperLimit").Style.Alignment = DataGridViewContentAlignment.MiddleCenter
            bsRefDetailDataGridView.Rows(pCurrentRow).Cells("NormalUpperLimit").ErrorText = GetMessageText(RefMessages.REQUIRED_VALUE.ToString)
        End If

        If (Not errorFound) Then
            If (CSng(bsRefDetailDataGridView.Rows(pCurrentRow).Cells("NormalLowerLimit").Value.ToString) >= _
                CSng(bsRefDetailDataGridView.Rows(pCurrentRow).Cells("NormalUpperLimit").Value.ToString)) Then
                errorFound = True
                rowWithErrors = True
                bsRefDetailDataGridView.Rows(pCurrentRow).Cells("NormalLowerLimit").Style.Alignment = DataGridViewContentAlignment.MiddleCenter
                bsRefDetailDataGridView.Rows(pCurrentRow).Cells("NormalLowerLimit").ErrorText = GetMessageText(RefMessages.MIN_MUST_BE_LOWER_THAN_MAX.ToString)
            End If
        End If

        If (Not rowWithErrors) Then
            'If there are not errors in the grid row, clean all Error symbols
            bsRefDetailDataGridView.Rows(pCurrentRow).Cells("GenderDesc").ErrorText = String.Empty
            bsRefDetailDataGridView.Rows(pCurrentRow).Cells("AgeUnitDesc").ErrorText = String.Empty

            bsRefDetailDataGridView.Rows(pCurrentRow).Cells("AgeRangeFrom").ErrorText = String.Empty
            bsRefDetailDataGridView.Rows(pCurrentRow).Cells("AgeRangeFrom").Style.Alignment = DataGridViewContentAlignment.MiddleRight

            bsRefDetailDataGridView.Rows(pCurrentRow).Cells("AgeRangeTo").ErrorText = String.Empty
            bsRefDetailDataGridView.Rows(pCurrentRow).Cells("AgeRangeTo").Style.Alignment = DataGridViewContentAlignment.MiddleRight

            bsRefDetailDataGridView.Rows(pCurrentRow).Cells("NormalLowerLimit").ErrorText = String.Empty
            bsRefDetailDataGridView.Rows(pCurrentRow).Cells("NormalLowerLimit").Style.Alignment = DataGridViewContentAlignment.MiddleRight

            bsRefDetailDataGridView.Rows(pCurrentRow).Cells("NormalUpperLimit").ErrorText = String.Empty
            bsRefDetailDataGridView.Rows(pCurrentRow).Cells("NormalUpperLimit").Style.Alignment = DataGridViewContentAlignment.MiddleRight
        End If
        'Catch ex As Exception
        '    Throw ex
        'End Try
        Return rowWithErrors
    End Function

    ''' <summary>
    ''' Validate ranges in the grid are not duplicated or overlapped
    ''' </summary>
    ''' <param name="pCurrentRow">Index of the grid row to validate</param>
    Private Function ValidateDetailedRanges(ByVal pCurrentRow As Integer) As Boolean
        Dim rowWithErrors As Boolean = False
        'Try
        rowWithErrors = (String.Compare(bsRefDetailDataGridView.Rows(pCurrentRow).Cells("GenderDesc").ErrorText, String.Empty, False) <> 0) Or _
                        (String.Compare(bsRefDetailDataGridView.Rows(pCurrentRow).Cells("AgeUnitDesc").ErrorText, String.Empty, False) <> 0) Or _
                        (String.Compare(bsRefDetailDataGridView.Rows(pCurrentRow).Cells("AgeRangeFrom").ErrorText, String.Empty, False) <> 0) Or _
                        (String.Compare(bsRefDetailDataGridView.Rows(pCurrentRow).Cells("AgeRangeTo").ErrorText, String.Empty, False) <> 0) Or _
                        (String.Compare(bsRefDetailDataGridView.Rows(pCurrentRow).Cells("NormalLowerLimit").ErrorText, String.Empty, False) <> 0) Or _
                        (String.Compare(bsRefDetailDataGridView.Rows(pCurrentRow).Cells("NormalUpperLimit").ErrorText, String.Empty, False) <> 0)

        If (Not rowWithErrors) Then
            If (String.Compare(DetailedRangeType, "GENDER", False) = 0) Then
                For i As Integer = 0 To bsRefDetailDataGridView.Rows.Count - 2
                    If (i <> pCurrentRow) Then
                        If (Not rowWithErrors) Then
                            If (bsRefDetailDataGridView.Rows(i).Cells("GenderDesc").ErrorText = String.Empty) Then
                                If (bsRefDetailDataGridView.Rows(i).Cells("GenderDesc").Value.ToString = _
                                    bsRefDetailDataGridView.Rows(pCurrentRow).Cells("GenderDesc").Value.ToString) Then
                                    rowWithErrors = True
                                    bsRefDetailDataGridView.Rows(pCurrentRow).Cells("GenderDesc").ErrorText = GetMessageText(RefMessages.DUPLICATED_RANGE_BY_GENDER.ToString)
                                    Exit For
                                End If
                            End If
                        End If
                    End If
                Next

            ElseIf (String.Compare(DetailedRangeType, "AGE", False) = 0 Or DetailedRangeType = "GENDER_AGE") Then
                Dim editedGender As String = String.Empty
                If (DetailedRangeType = "GENDER_AGE") Then editedGender = bsRefDetailDataGridView.Rows(pCurrentRow).Cells("GenderDesc").Value.ToString

                Dim editedAgeUnit As String = bsRefDetailDataGridView.Rows(pCurrentRow).Cells("AgeUnitDesc").Value.ToString
                Dim editedAgeFrom As Integer = CInt(bsRefDetailDataGridView.Rows(pCurrentRow).Cells("AgeRangeFrom").Value)
                Dim editedAgeTo As Integer = CInt(bsRefDetailDataGridView.Rows(pCurrentRow).Cells("AgeRangeTo").Value)

                For i As Integer = 0 To bsRefDetailDataGridView.Rows.Count - 2
                    If (i <> pCurrentRow) Then
                        If (Not rowWithErrors) Then
                            Dim readGender As String = String.Empty
                            If (DetailedRangeType = "GENDER_AGE") Then
                                If (bsRefDetailDataGridView.Rows(i).Cells("GenderDesc").ErrorText = String.Empty) Then
                                    readGender = bsRefDetailDataGridView.Rows(i).Cells("GenderDesc").Value.ToString
                                End If
                            End If

                            Dim readAgeUnit As String = String.Empty
                            If (bsRefDetailDataGridView.Rows(i).Cells("AgeUnitDesc").ErrorText = String.Empty) Then
                                readAgeUnit = bsRefDetailDataGridView.Rows(i).Cells("AgeUnitDesc").Value.ToString
                            End If

                            If (String.Compare(editedGender, readGender, False) = 0 And _
                                readAgeUnit = editedAgeUnit) Then
                                If (bsRefDetailDataGridView.Rows(i).Cells("AgeRangeFrom").ErrorText = String.Empty And _
                                    bsRefDetailDataGridView.Rows(i).Cells("AgeRangeTo").ErrorText = String.Empty) Then
                                    Dim readAgeFrom As Integer = CInt(bsRefDetailDataGridView.Rows(i).Cells("AgeRangeFrom").Value)
                                    Dim readAgeTo As Integer = CInt(bsRefDetailDataGridView.Rows(i).Cells("AgeRangeTo").Value)

                                    If (editedAgeFrom >= readAgeFrom And editedAgeFrom <= readAgeTo) Then
                                        rowWithErrors = True
                                        bsRefDetailDataGridView.Rows(pCurrentRow).Cells("AgeRangeFrom").Style.Alignment = DataGridViewContentAlignment.MiddleLeft
                                        bsRefDetailDataGridView.Rows(pCurrentRow).Cells("AgeRangeFrom").ErrorText = GetMessageText(RefMessages.WRONG_AGE_LIMITS.ToString)

                                    ElseIf (editedAgeTo >= readAgeFrom And editedAgeTo <= readAgeTo) Then
                                        rowWithErrors = True
                                        bsRefDetailDataGridView.Rows(pCurrentRow).Cells("AgeRangeTo").Style.Alignment = DataGridViewContentAlignment.MiddleLeft
                                        bsRefDetailDataGridView.Rows(pCurrentRow).Cells("AgeRangeTo").ErrorText = GetMessageText(RefMessages.WRONG_AGE_LIMITS.ToString)

                                    ElseIf (editedAgeFrom <= readAgeFrom And editedAgeTo >= readAgeTo) Then
                                        rowWithErrors = True
                                        bsRefDetailDataGridView.Rows(pCurrentRow).Cells("AgeRangeFrom").Style.Alignment = DataGridViewContentAlignment.MiddleLeft
                                        bsRefDetailDataGridView.Rows(pCurrentRow).Cells("AgeRangeTo").Style.Alignment = DataGridViewContentAlignment.MiddleLeft

                                        bsRefDetailDataGridView.Rows(pCurrentRow).Cells("AgeRangeFrom").ErrorText = GetMessageText(RefMessages.WRONG_AGE_LIMITS.ToString)
                                        bsRefDetailDataGridView.Rows(pCurrentRow).Cells("AgeRangeTo").ErrorText = GetMessageText(RefMessages.WRONG_AGE_LIMITS.ToString)
                                    End If
                                End If
                            End If
                        End If
                    End If
                Next
            End If
        End If

        ValidationErrorAttribute = rowWithErrors
        'Catch ex As Exception
        '    Throw ex
        'End Try
        Return rowWithErrors
    End Function

    ''' <summary>
    ''' Clears all Reference Range controls
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SG 01/09/2010
    ''' </remarks>
    Private Sub ClearReferenceRangesControls()
        'Try
        bsRefNormalLowerLimitUpDown.ResetText()
        bsRefNormalUpperLimitUpDown.ResetText()
        bsRefBorderLineLowerLimitUpDown.ResetText()
        bsRefBorderLineUpperLimitUpDown.ResetText()

        bsRefNormalLowerLimitUpDown.Value = bsRefNormalLowerLimitUpDown.Minimum
        bsRefNormalUpperLimitUpDown.Value = bsRefNormalUpperLimitUpDown.Minimum
        bsRefBorderLineLowerLimitUpDown.Value = bsRefBorderLineLowerLimitUpDown.Minimum
        bsRefBorderLineUpperLimitUpDown.Value = bsRefBorderLineUpperLimitUpDown.Minimum

        bsRefDetailDataGridView.Rows.Clear()
        bsControlErrorProvider.Clear()

        ValidationErrorAttribute = False
        'Catch ex As Exception
        '    Throw ex
        'End Try
    End Sub
#End Region

#Region "Events"

    '******************************************
    '*** EVENTS FOR GENERIC RANGES CONTROLS ***
    '******************************************
    '''' <summary>
    '''' Manages the selection/unselection of Generic Range as active Range Type
    '''' </summary>
    Private Sub bsGenericCheckBox_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles bsGenericCheckBox.Click
        'Try
        If (ValidationErrorAttribute) Then
            bsGenericCheckBox.Checked = Not bsGenericCheckBox.Checked
        Else
            Dim cancelChange As Boolean = False

            If (bsGenericCheckBox.Checked) Then
                If (bsDetailedCheckBox.Checked) Then
                    'Validation of values entered for Detailed Ranges (missing values and/or range limits)
                    Dim rowWithErrors As Boolean = False
                    For i As Integer = 0 To bsRefDetailDataGridView.Rows.Count - 2
                        If (ValidateDetailedRow(i)) Then
                            'If at least one of the rows have errors, this variable is set to True
                            rowWithErrors = True
                        End If
                    Next
                    If (Not ValidationErrorAttribute) Then ValidationErrorAttribute = rowWithErrors

                    If (Not rowWithErrors) Then
                        For i As Integer = 0 To bsRefDetailDataGridView.Rows.Count - 2
                            If (ValidateDetailedRanges(i)) Then
                                'If at least one of the rows have errors, this variable is set to True
                                rowWithErrors = True
                            End If
                        Next
                        If (Not ValidationErrorAttribute) Then ValidationErrorAttribute = rowWithErrors
                    End If
                End If
            End If
            cancelChange = ValidationErrorAttribute

            If (Not cancelChange) Then
                bsControlErrorProvider.Clear()
                ValidationErrorAttribute = False

                If (bsDetailedCheckBox.Checked) Then bsDetailedCheckBox.Checked = False
                If (EditionModeAttribute) Then ChangesMade = True
            Else
                bsGenericCheckBox.Checked = Not bsGenericCheckBox.Checked
            End If
        End If
        CheckBoxManagement()
        'Catch ex As Exception
        '    Throw ex
        'End Try
    End Sub

    ''' <summary>
    ''' Activate the Change made variable when some value of the corresponding control is change.
    ''' </summary>
    Private Sub ControlValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsRefNormalLowerLimitUpDown.ValueChanged, _
                                                                                                        bsRefBorderLineUpperLimitUpDown.ValueChanged, _
                                                                                                        bsRefNormalUpperLimitUpDown.ValueChanged, _
                                                                                                        bsRefBorderLineLowerLimitUpDown.ValueChanged
        'Try
        If (EditionModeAttribute) Then ChangesMade = True
        'Catch ex As Exception
        '    Throw ex
        'End Try
    End Sub

    ''' <summary>
    ''' Checks characters entered in Numeric UpDown controls are valid 
    ''' </summary>
    Private Sub NumericUpDown_KeyPress(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles bsRefNormalUpperLimitUpDown.KeyPress, _
                                                                                                                                 bsRefNormalLowerLimitUpDown.KeyPress, _
                                                                                                                                 bsRefBorderLineUpperLimitUpDown.KeyPress, _
                                                                                                                                 bsRefBorderLineLowerLimitUpDown.KeyPress
        'Try
        If (String.Compare(e.KeyChar.ToString(), ".".ToString(), False) = 0 Or String.Compare(e.KeyChar.ToString(), ",".ToString(), False) = 0 Or String.Compare(e.KeyChar.ToString(), "'".ToString(), False) = 0) Then
            e.KeyChar = CChar(OSDecimalSeparator)
            If (CType(sender, NumericUpDown).Text.Contains(".") Or CType(sender, NumericUpDown).Text.Contains(",")) Then
                e.Handled = True
            End If
        End If
        'Catch ex As Exception
        '    Throw ex
        'End Try
    End Sub

    ''' <summary>
    ''' When an Numeric UpDown control is empty, the defined minimum value is set as control value
    ''' </summary>
    Private Sub NumericUpDown_KeyUp(ByVal sender As System.Object, ByVal e As KeyEventArgs) Handles bsRefNormalUpperLimitUpDown.KeyUp, _
                                                                                                                         bsRefNormalLowerLimitUpDown.KeyUp, _
                                                                                                                         bsRefBorderLineUpperLimitUpDown.KeyUp, _
                                                                                                                         bsRefBorderLineLowerLimitUpDown.KeyUp
        'Try
        Dim miNumericUpDown As NumericUpDown = CType(sender, NumericUpDown)
        If (String.Compare(miNumericUpDown.Text, String.Empty, False) = 0) Then
            miNumericUpDown.Value = miNumericUpDown.Minimum
            miNumericUpDown.ResetText()
        End If

        If (EditionModeAttribute) Then ChangesMade = True
        'Catch ex As Exception
        '    Throw ex
        'End Try
    End Sub

    ''' <summary>
    ''' Generic event to clean the Error Provider when the value in the TextBox showing the error is changed
    ''' </summary>
    Private Sub NumericUpdDown_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles bsRefNormalLowerLimitUpDown.TextChanged, _
                                                                                                        bsRefNormalUpperLimitUpDown.TextChanged, _
                                                                                                        bsRefBorderLineLowerLimitUpDown.TextChanged, _
                                                                                                        bsRefBorderLineUpperLimitUpDown.TextChanged
        'Try
        Dim miNumericUpDown As NumericUpDown = CType(sender, NumericUpDown)
        If (String.Compare(miNumericUpDown.Text, String.Empty, False) = 0) Then
            If (String.Compare(bsControlErrorProvider.GetError(miNumericUpDown), String.Empty, False) <> 0) Then
                bsControlErrorProvider.SetError(miNumericUpDown, String.Empty)
            End If
        End If
        'Catch ex As Exception
        '    Throw ex
        'End Try
    End Sub

    ''' <summary>
    ''' Generic event to clean the Error Provider when the value in the NumericUpDown showing the error is changed
    ''' </summary>
    Private Sub NumericUpDown_ValueChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles bsRefNormalLowerLimitUpDown.ValueChanged, _
                                                                                                        bsRefNormalUpperLimitUpDown.ValueChanged, _
                                                                                                        bsRefBorderLineLowerLimitUpDown.ValueChanged, _
                                                                                                        bsRefBorderLineUpperLimitUpDown.ValueChanged
        'Try
        Dim myNumUpDown As New NumericUpDown
        myNumUpDown = CType(sender, NumericUpDown)

        If (myNumUpDown.Value > 0) Then
            If (String.Compare(bsControlErrorProvider.GetError(myNumUpDown), String.Empty, False) <> 0) Then
                bsControlErrorProvider.SetError(myNumUpDown, String.Empty)
            End If
        End If
        'Catch ex As Exception
        '    Throw ex
        'End Try
    End Sub

    ''' <summary>
    ''' Manages validating event of Numeric UpDown controls for Generic Normality Ranges
    ''' </summary>
    Private Sub RefNormalLimits_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles bsRefNormalLowerLimitUpDown.Validating, _
                                                                                                                                    bsRefNormalUpperLimitUpDown.Validating
        'Try
        ValidationErrorAttribute = False
        bsControlErrorProvider.Clear()

        'ValidateLimitsUpDown(bsRefNormalLowerLimitUpDown, bsRefNormalUpperLimitUpDown, False)'TR 12/04/2012 Commented.
        If (Not ValidationErrorAttribute) Then
            'ValidateRefRangesLimits(False)'TR 12/04/2012 Commented
            If (String.Compare(bsRefNormalLowerLimitUpDown.Text, String.Empty, False) <> 0 And String.Compare(bsRefNormalUpperLimitUpDown.Text, String.Empty, False) <> 0) Then
                bsRefBorderLineLowerLimitUpDown.Enabled = True
                bsRefBorderLineUpperLimitUpDown.Enabled = True

                bsRefBorderLineLowerLimitUpDown.BackColor = Color.White
                bsRefBorderLineUpperLimitUpDown.BackColor = Color.White
            Else
                If (String.Compare(bsRefNormalLowerLimitUpDown.Text, String.Empty, False) = 0) Then
                    bsRefNormalLowerLimitUpDown.ResetText()
                End If
                If (String.Compare(bsRefNormalUpperLimitUpDown.Text, String.Empty, False) = 0) Then
                    bsRefNormalUpperLimitUpDown.ResetText()
                End If

                bsRefBorderLineLowerLimitUpDown.Enabled = False
                bsRefBorderLineUpperLimitUpDown.Enabled = False

                bsRefBorderLineLowerLimitUpDown.BackColor = SystemColors.MenuBar
                bsRefBorderLineUpperLimitUpDown.BackColor = SystemColors.MenuBar

                bsRefBorderLineLowerLimitUpDown.Text = String.Empty
                bsRefBorderLineUpperLimitUpDown.Text = String.Empty
            End If
        End If
        'Catch ex As Exception
        '    Throw ex
        'End Try
    End Sub

    ''' <summary>
    ''' Manages validating event of Numeric UpDown controls for Generic BorderLine Ranges
    ''' </summary>
    Private Sub RefBorderLineLimits_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles bsRefBorderLineUpperLimitUpDown.Validating, _
                                                                                                                                        bsRefBorderLineLowerLimitUpDown.Validating
        'Try
        ValidationErrorAttribute = False
        bsControlErrorProvider.Clear()

        ValidateLimitsUpDown(bsRefBorderLineLowerLimitUpDown, bsRefBorderLineUpperLimitUpDown, False)
        ValidateRefRangesLimits(False)
        'Catch ex As Exception
        '    Throw ex
        'End Try
    End Sub

    '******************************************
    '*** EVENTS FOR DETAILED RANGES CONTROLS ***
    '******************************************
    ''' <summary>
    '''  Manages the selection/unselection of Detailed Ranges as active Range Type
    ''' </summary>
    Private Sub bsDetailedCheckBox_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles bsDetailedCheckBox.Click
        'Try
        If (ValidationErrorAttribute) Then
            bsDetailedCheckBox.Checked = Not bsDetailedCheckBox.Checked
        Else
            Dim cancelChange As Boolean = False

            If (bsDetailedCheckBox.Checked) Then
                cancelChange = (bsGenericCheckBox.Checked And ValidationErrorAttribute)
            Else
                cancelChange = ValidationErrorAttribute
            End If

            If (Not cancelChange) Then
                bsControlErrorProvider.Clear()
                ValidationErrorAttribute = False
                If (bsDetailedSubTypesComboBox.SelectedIndex = -1) Then
                    bsDetailedSubTypesComboBox.SelectedIndex = 2
                    ChangeDetailedSubType()
                End If

                If (bsGenericCheckBox.Checked) Then bsGenericCheckBox.Checked = False
                If (EditionModeAttribute) Then ChangesMade = True
            End If
        End If
        CheckBoxManagement()
        'Catch ex As Exception
        '    Throw ex
        'End Try
    End Sub

    ''' <summary>
    ''' Manages the change of SubType for Detailed Reference Ranges
    ''' </summary>
    Private Sub bsDetailedSubTypesComboBox_SelectionChangeCommitted(ByVal sender As Object, ByVal e As System.EventArgs) Handles bsDetailedSubTypesComboBox.SelectionChangeCommitted
        'Try
        ChangeDetailedSubType()
        'Catch ex As Exception
        '    Throw ex
        'End Try
    End Sub

    ''' <summary>
    ''' Manages the click in button for deleting the Detailed Reference Range selected in the DataGrid
    ''' </summary>
    Private Sub bsRefDetailDeleteButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsRefDetailDeleteButton.Click
        'Try
        DeleteSelectedDetailedRanges()
        'Catch ex As Exception
        '    Throw ex
        'End Try
    End Sub

    ''' <summary>
    ''' When the focus is set to one of the editable cells, the full current Row becomes editable
    ''' </summary>
    Private Sub bsRefDetailDataGridView_CellEnter(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles bsRefDetailDataGridView.CellEnter
        'Try
        If (e.RowIndex >= 0) Then bsRefDetailDataGridView.Rows(e.RowIndex).ReadOnly = False
        'Catch ex As Exception
        '    Throw ex
        'End Try
    End Sub

    ''' <summary>
    ''' When the ESC key is pressed in a NumericUpDown cell, ignore it
    ''' </summary>
    Private Sub bsRefDetailDataGridView_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles bsRefDetailDataGridView.KeyDown
        'Try
        If bsRefDetailDataGridView.CurrentCell IsNot Nothing AndAlso (bsRefDetailDataGridView.CurrentCell.ColumnIndex > 2) Then
            If (e.KeyCode = Keys.Escape) Then
                e.Handled = True
            End If
        End If
        'Catch ex As Exception
        '    Throw ex
        'End Try
    End Sub

    ''' <summary>
    ''' When the Row/Column Header is clicked, all Rows are selected and locked to allow deletion of them using the SUPR key
    ''' </summary>
    Private Sub bsRefDetailDataGridView_CellMouseClick(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellMouseEventArgs) Handles bsRefDetailDataGridView.CellMouseClick
        'Try
        If (e.RowIndex < 0 And e.ColumnIndex < -1) Then
            For i As Integer = 0 To bsRefDetailDataGridView.Rows.Count - 1
                For j As Integer = 0 To bsRefDetailDataGridView.Columns.Count - 1
                    bsRefDetailDataGridView.Rows(i).Cells(j).ReadOnly = True
                Next
            Next
        End If
        'Catch ex As Exception
        '    Throw ex
        'End Try
    End Sub

    ''' <summary>
    ''' When the row is selected by clicking in the RowHeader, the full Row becomes non-editable 
    ''' (to allow row deletion using the SUPR key)
    ''' </summary>
    Private Sub bsRefDetailDataGridView_RowHeaderMouseClick(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellMouseEventArgs) Handles bsRefDetailDataGridView.RowHeaderMouseClick
        'Try
        If (e.RowIndex >= 0) Then bsRefDetailDataGridView.Rows(e.RowIndex).ReadOnly = True
        'Catch ex As Exception
        '    Throw ex
        'End Try
    End Sub

    ''' <summary>
    ''' Delete the selected Detailed Reference Ranges using the SUPR key
    ''' </summary>
    Private Sub bsRefDetailDataGridView_PreviewKeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.PreviewKeyDownEventArgs) Handles bsRefDetailDataGridView.PreviewKeyDown
        'Try
        If (e.KeyCode = Keys.Delete) Then
            If (bsRefDetailDeleteButton.Enabled) Then DeleteSelectedDetailedRanges()
        End If
        'Catch ex As Exception
        '    Throw ex
        'End Try
    End Sub

    ''' <summary>
    ''' Manages changes when a new value is selected in whatever of the ComboBox columns in the DataGridView
    ''' for detailed  Reference Ranges
    ''' </summary>
    Private Sub bsRefDetailDataGridView_EditingControlShowing(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewEditingControlShowingEventArgs) Handles bsRefDetailDataGridView.EditingControlShowing
        'Try
        Dim currentRow As Integer = bsRefDetailDataGridView.CurrentCell.RowIndex
        Dim currentCol As Integer = bsRefDetailDataGridView.CurrentCell.ColumnIndex

        If (currentCol = 0 Or currentCol = 1) Then
            Dim editingComboBox As ComboBox = CType(e.Control, ComboBox)
            If (Not editingComboBox Is Nothing) Then
                RemoveHandler editingComboBox.SelectionChangeCommitted, AddressOf editingGridComboBox_SelectionChangeCommitted
                AddHandler editingComboBox.SelectionChangeCommitted, AddressOf editingGridComboBox_SelectionChangeCommitted
            End If
        ElseIf (currentCol = 2 Or currentCol = 3) Then
            Dim editingUpDownCell As NumericUpDown = CType(e.Control, NumericUpDown)
            If (Not editingUpDownCell Is Nothing) Then
                editingUpDownCell.ContextMenuStrip = New ContextMenuStrip

                RemoveHandler editingUpDownCell.KeyPress, AddressOf editingGridUpDown_KeyPress
                AddHandler editingUpDownCell.KeyPress, AddressOf editingGridUpDown_KeyPress

                RemoveHandler editingUpDownCell.ValueChanged, AddressOf editingGridUpDown_ValueChanged
                AddHandler editingUpDownCell.ValueChanged, AddressOf editingGridUpDown_ValueChanged
            End If
            'TR 28/07/2011 - Validate changes made on values colums on specials events
        ElseIf (currentCol = 4 Or currentCol = 5) Then
            Dim editingUpDownCell As NumericUpDown = CType(e.Control, NumericUpDown)
            If (Not editingUpDownCell Is Nothing) Then
                editingUpDownCell.ContextMenuStrip = New ContextMenuStrip

                RemoveHandler editingUpDownCell.KeyPress, AddressOf editingGridUpDown_KeyPress
                AddHandler editingUpDownCell.KeyPress, AddressOf editingGridUpDown_KeyPress

                RemoveHandler editingUpDownCell.ValueChanged, AddressOf editingGridUpDown_ValueChanged
                AddHandler editingUpDownCell.ValueChanged, AddressOf editingGridUpDown_ValueChanged

                RemoveHandler editingUpDownCell.MouseWheel, AddressOf editingGridUpDown_ValueChanged
                AddHandler editingUpDownCell.MouseWheel, AddressOf editingGridUpDown_ValueChanged
            End If
        End If
        'TR 28/07/2011 -Commented
        'ChangesMade = True
        'Catch ex As Exception
        '    Throw ex
        'End Try
    End Sub

    ''' <summary>
    ''' Validates missing values and/or range limits errors in the row being edited
    ''' </summary>
    Private Sub bsRefDetailDataGridView_RowValidating(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellCancelEventArgs) Handles bsRefDetailDataGridView.RowValidating
        'Try
        If (e.RowIndex <> bsRefDetailDataGridView.RowCount - 1) Then
            ValidationErrorAttribute = ValidateDetailedRow(e.RowIndex)
        End If
        'Catch ex As Exception
        '    Throw ex
        'End Try
    End Sub

    ''' <summary>
    ''' When all values entered in cells of the current row are correct, the duplicated and/or overlapped 
    ''' range definition is verified
    ''' </summary>
    Private Sub bsRefDetailDataGridView_RowValidated(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles bsRefDetailDataGridView.RowValidated
        'Try
        If (e.RowIndex <> bsRefDetailDataGridView.RowCount - 1) Then
            ValidationErrorAttribute = ValidateDetailedRanges(e.RowIndex)
        End If
        'Catch ex As Exception
        '    Throw ex
        'End Try
    End Sub

#End Region

    Public Sub New()

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
    End Sub
End Class
