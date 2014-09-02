Option Explicit On
Option Strict On

Imports System.Drawing
Imports System.Windows.Forms
'Imports System.Text.RegularExpressions
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Types.AllowedTestsDS

Namespace Biosystems.Ax00.Controls.UserControls

    ''' <summary>
    ''' User Control that allows the creation and edition of the Formula 
    ''' needed for definition of a Calculated Test. It is specific for the 
    ''' screen of Calculated Tests in Module of Parameters Programming. 
    ''' </summary>
    ''' <remarks></remarks>
    Public Class BSFormula

#Region "Enumerates and Structures"
        ''' <summary>
        ''' Types defined for Formula members
        ''' </summary>
        Private Enum ValueTypes
            Brac   'Bracket
            Test   'Formula Test
            NInt   'Integer Number
            NDec   'Decimal Number
            Oper   'Operator +,-,/,*
            None
        End Enum
#End Region

#Region "Declarations"
        Private nPosition As Integer                    'Redundant assignment = 0
        Private imagePathControl As Hashtable
#End Region

#Region "Attibutes"
        'Attribute variables for all the Labels
        Private FormulaTitleAttribute As String = ""
        Private SampleTypeTitleAttribute As String = ""
        Private StandardTestsTitleAttribute As String = ""
        Private CalculatedTestsTitleAttribute As String = ""
        Private DeleteBtnToolTipAttribute As String = ""
        Private ClearBtnTooltipAttribute As String = ""

        'Attribute variables needed to fill the Formula Controls
        Private SampleTypesListAttribute As DataSet
        Private TestStandardListAttribute As DataSet
        Private TestCalculatedListAttribute As DataSet

        'Attribute variables needed when an unique SampleType is allowed
        Private SelectedSampleTypeAttribute As String = ""
        Private TestSTDSampleTypeListAttribute As DataSet
        Private TestCALCSampleTypeListAttribute As DataSet

        'Attribute variables needed when the Formula is defined for an exiting Calculated Test
        Private CalcTestIDAttribute As Integer ' = 0 Redundant field initialization
        Private FormulaValuesListAttribute As DataSet
        Private EditionModeAttribute As Boolean
        Private GenerateFormulaValueListAttribute As Boolean

        'Attribute variables needed to get the current Formula and its status
        Private FormulaStringAttribute As String = ""
        Private FormulaSintaxStatusAttribute As Boolean
        Private OriginalFormulaValueAttribute As String = ""
        Private EnableStatusAttribute As Boolean = True

        'Attribute variables needed for the path of the different Icons used in the control
        Private OKIconNameAttribute As String = ""
        Private WRONGIconNameAttribute As String = ""
        Private WARNINGIconNameAttribute As String = ""

        'TR 09/03/2011 -Message used to indicate the factory values
        Private FactoryValuesMessageAttribute As String = ""

        Private myunique As Boolean '= False Redundant field initialization

        'TR 28/07/2011 -Set variable to indicate the Decimal separator.
        Private DecimalSeparatorAttribute As String

#End Region

#Region "Properties"


        Public Property DecimalSeparator() As String
            Get
                Return DecimalSeparatorAttribute
            End Get
            Set(ByVal value As String)
                DecimalSeparatorAttribute = value
            End Set
        End Property

        ''' <summary>
        ''' Property use to recive the factory value message.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>CREATED BY: TR 09/03/2011</remarks>
        Public Property FactoryValueMessage() As String
            Get
                Return FactoryValuesMessageAttribute
            End Get
            Set(ByVal value As String)
                FactoryValuesMessageAttribute = value
            End Set
        End Property

        ''' <summary>
        ''' Title that must be shown for the Formula field
        ''' </summary>
        Public Property FormulaTitle() As String
            Get
                Return FormulaTitleAttribute
            End Get
            Set(ByVal value As String)
                FormulaTitleAttribute = value ' & ":"
                FormulaLabel.Text = FormulaTitleAttribute
            End Set
        End Property

        ''' <summary>
        ''' Title that must be shown for the ComboBox of Sample Types
        ''' </summary>
        Public Property SampleTypeTitle() As String
            Get
                Return SampleTypeTitleAttribute
            End Get
            Set(ByVal value As String)
                SampleTypeTitleAttribute = value ' & ":"
                SampleTypeLabel.Text = SampleTypeTitleAttribute
            End Set
        End Property

        ''' <summary>
        ''' Title that must be shown for the ListView of Standard Tests
        ''' </summary>
        ''' <remarks>
        ''' Created by: SA 06/07/2010
        ''' </remarks>
        Public Property StandardTestsTitle() As String
            Get
                Return StandardTestsTitleAttribute
            End Get
            Set(ByVal value As String)
                StandardTestsTitleAttribute = value ' & ":"
                bsStandardLabel.Text = StandardTestsTitleAttribute
            End Set
        End Property

        ''' <summary>
        ''' Title that must be shown for the ListView of Calculated Tests
        ''' </summary>
        ''' <remarks>
        ''' Created by: SA 06/07/2010
        ''' </remarks>
        Public Property CalculatedTestsTitle() As String
            Get
                Return CalculatedTestsTitleAttribute
            End Get
            Set(ByVal value As String)
                CalculatedTestsTitleAttribute = value ' & ":"
                bsCalculatedLabel.Text = CalculatedTestsTitleAttribute
            End Set
        End Property

        ''' <summary>
        ''' ToolTip for button Back (delete last Formula member) in the keyboard
        ''' </summary>
        ''' <remarks>
        ''' Created by: SA 06/07/2010
        ''' </remarks>
        Public Property DelFormulaMemberToolTip() As String
            Get
                Return DeleteBtnToolTipAttribute
            End Get
            Set(ByVal value As String)
                DeleteBtnToolTipAttribute = value
                bsFormulaToolTips.SetToolTip(BackButton, DeleteBtnToolTipAttribute)
            End Set
        End Property

        ''' <summary>
        ''' ToolTip for button Clear (clear Formula) in the keyboard
        ''' </summary>
        ''' <remarks>
        ''' Created by: SA 06/07/2010
        ''' </remarks>
        Public Property ClearFormulaToolTip() As String
            Get
                Return ClearBtnTooltipAttribute
            End Get
            Set(ByVal value As String)
                ClearBtnTooltipAttribute = value
                bsFormulaToolTips.SetToolTip(ClearButton, ClearBtnTooltipAttribute)
            End Set
        End Property

        ''' <summary>
        ''' To inform the Code+Description of all available Sample Types and fill
        ''' the correspondent ComboBox
        ''' </summary>
        Public WriteOnly Property SampleTypesList() As DataSet
            Set(ByVal value As DataSet)
                If (value Is Nothing) Then
                    PrepareTestList(SampleTypesListAttribute)
                Else
                    SampleTypesListAttribute = value
                    FillSampleTypesList(SampleTypesListAttribute)
                End If
            End Set
        End Property

        ''' <summary>
        ''' To fill the list containing Standard Tests
        ''' </summary>
        ''' <remarks>
        ''' Created by: DL 13/05/2010
        ''' </remarks>
        Public WriteOnly Property TestStandardList() As DataSet
            Set(ByVal value As DataSet)
                If value Is Nothing Then
                    PrepareTestList(TestStandardListAttribute)
                Else
                    TestStandardListAttribute = value
                End If
            End Set
        End Property

        ''' <summary>
        ''' To fill the list containing Calculated Tests
        ''' </summary>
        ''' <remarks>
        ''' Created by: DL 13/05/2010
        ''' </remarks>
        Public WriteOnly Property TestCalculatedList() As DataSet
            Set(ByVal value As DataSet)
                If value Is Nothing Then
                    PrepareTestList(TestCalculatedListAttribute)
                Else
                    TestCalculatedListAttribute = value
                End If
            End Set
        End Property

        ''' <summary>
        ''' When this property is informed, it means the Formula will work only for 
        ''' Tests (Standard and/or Calculated) defined for the specified SampleType.
        ''' Besides, the ComboBox of available SampleTypes will be disabled and 
        ''' it will show the informed value as the selected option
        ''' Modified by SG 30/07/2010 Add the Get
        ''' </summary>
        Public Property SelectedSampleType() As String
            Get
                Return SelectedSampleTypeAttribute
            End Get

            Set(ByVal value As String)
                SelectedSampleTypeAttribute = value
                If (Not SelectedSampleTypeAttribute Is Nothing) Then
                    SampleTypeListComboBox.SelectedValue = SelectedSampleTypeAttribute
                    FilterTestListBySampleType()
                End If
            End Set
        End Property

        ''' <summary>
        ''' To fill the list containing Standard Tests when a SampleType has been informed
        ''' </summary>
        ''' <remarks>
        ''' Created by: DL 13/05/2010
        ''' </remarks>
        Public WriteOnly Property TestStandardSampleTypeList() As DataSet
            Set(ByVal value As DataSet)
                If (value Is Nothing) Then
                    PrepareTestList(TestSTDSampleTypeListAttribute)
                Else
                    TestSTDSampleTypeListAttribute = value
                    FillStandardListView(TestSTDSampleTypeListAttribute)
                End If
            End Set
        End Property

        ''' <summary>
        ''' To fill the list containing Calculated Tests when a SampleType has been informed
        ''' </summary>
        ''' <remarks>
        ''' Created by: DL 13/05/2010
        ''' </remarks>
        Public WriteOnly Property TestCalculatedSampleTypeList() As DataSet
            Set(ByVal value As DataSet)
                If (value Is Nothing) Then
                    PrepareTestList(TestCALCSampleTypeListAttribute)
                Else
                    TestCALCSampleTypeListAttribute = value
                    FillCalculatedListView(TestCALCSampleTypeListAttribute)
                End If
            End Set
        End Property

        ''' <summary>
        ''' Identifier of the Calculated Test for which the Formula is defined, needed
        ''' to exclude it from the list of available Calculated Tests to include in the Formula
        ''' </summary>
        ''' <remarks>
        ''' Created by: DL 14/05/2010
        ''' </remarks>
        Public WriteOnly Property CalcTestID() As Integer
            Set(ByVal value As Integer)
                CalcTestIDAttribute = value
            End Set
        End Property

        ''' <summary>
        ''' To fill/return the list containing all the values included currently in the Formula
        ''' </summary>
        Public Property FormulaValuesList() As DataSet
            Get
                If (FormulaValuesListAttribute Is Nothing) Then
                    PrepareFormulaValuesList()
                End If
                Return FormulaValuesListAttribute
            End Get

            Set(ByVal value As DataSet)
                If (value Is Nothing) Then
                    PrepareFormulaValuesList()
                Else
                    FormulaValuesListAttribute = value
                End If
            End Set
        End Property

        ''' <summary>
        ''' To indicates if the control will recibe a Formula for Edition
        ''' </summary>
        Public WriteOnly Property EditionMode() As Boolean
            Set(ByVal value As Boolean)
                EditionModeAttribute = value
                If (EditionModeAttribute) Then
                    'Build the String Formula and validate the sintax is fine
                    RefreshFormula()
                    ValidateSintaxStatus()

                    'Refresh the Calculated Tests list to remove the Calculated Test being edited
                    If (TestCalculatedListAttribute IsNot Nothing) Then
                        'Get the list of Calculated Tests excepting the one in edition
                        Dim calculatedTestDataDS As New AllowedTestsDS
                        calculatedTestDataDS = DirectCast(TestCalculatedListAttribute, AllowedTestsDS)

                        Dim qSelectedTest As New List(Of tparAllowedTestsRow)
                        qSelectedTest = (From a In calculatedTestDataDS.tparAllowedTests _
                                        Where a.SampleTypeCode = SampleTypeListComboBox.SelectedValue.ToString _
                                          And a.TestCode <> CalcTestIDAttribute.ToString _
                                       Select a).ToList

                        'Add the obtained Calculated Tests to the DataSet to return
                        Dim tmpCalDataDS As New AllowedTestsDS
                        For Each calculatedRow As AllowedTestsDS.tparAllowedTestsRow In qSelectedTest
                            tmpCalDataDS.tparAllowedTests.ImportRow(calculatedRow)
                        Next calculatedRow

                        'Set value of filtered Calculated Test - Use the PROPERTY, not the ATTRIBUTE
                        'TestCALCSampleTypeListAttribute = tmpCalDataDS
                        TestCalculatedSampleTypeList = tmpCalDataDS
                    End If
                Else
                    FormulaTextBox.Clear()
                    FormulaStatusImage.ImageLocation = WRONGIconNameAttribute
                End If
            End Set
        End Property

        ''' <summary>
        '''Enable or disable controls in bsformula except labels
        ''' </summary>
        ''' <remarks>
        ''' Created by: DL 19/07/2011
        ''' </remarks>
        Public WriteOnly Property EnableDisableControls() As Boolean
            Set(ByVal value As Boolean)
                FormulaTextBox.Enabled = value

                If (value) Then
                    SampleTypeListComboBox.Enabled = myunique
                Else
                    SampleTypeListComboBox.Enabled = False
                End If

                bsStandardTestListView.Enabled = value
                bsCalculatedTestListView.Enabled = value
                AddSelectTestButton.Enabled = value
                NumbersPanel.Enabled = value
            End Set
        End Property

        ''' <summary>
        ''' To indicate if the Table FormulaValueList has to be generated
        ''' </summary>
        Public WriteOnly Property GenerateFormulaValueList() As Boolean
            Set(ByVal value As Boolean)
                GenerateFormulaValueListAttribute = value
            End Set
        End Property

        ''' <summary>
        ''' Return the current Formula as a String
        ''' Call this property after the control is filled with data
        ''' </summary>
        Public ReadOnly Property FormulaString() As String
            Get
                FormulaStringAttribute = FormulaTextBox.Text
                Return FormulaStringAttribute
            End Get
        End Property

        ''' <summary>
        ''' To fill the string Formula control
        ''' </summary>
        Public WriteOnly Property GenerateFormula() As String
            Set(ByVal value As String)
                FormulaStringAttribute = value
                Me.FormulaTextBox.Text = FormulaStringAttribute

                If (value = String.Empty) Then FormulaStatusImage.ImageLocation = WRONGIconNameAttribute
            End Set
        End Property

        ''' <summary>
        ''' Return if the Formula sintax is correct or if it has errors
        ''' </summary>
        Public ReadOnly Property FormulaSintaxStatus() As Boolean
            Get
                Return FormulaSintaxStatusAttribute
            End Get
        End Property

        ''' <summary>
        ''' Return the Formula loaded initially in the control
        ''' </summary>
        Public ReadOnly Property OriginalFormulaValue() As String
            Get
                OriginalFormulaValueAttribute = FormulaTextBox.Text
                Return OriginalFormulaValueAttribute
            End Get
        End Property

        ''' <summary>
        ''' To indicate if the Formula Control is enabled 
        ''' </summary>
        Public WriteOnly Property EnabledStatus() As Boolean
            Set(ByVal value As Boolean)
                EnableStatusAttribute = value
            End Set
        End Property


        ' DL 12/11/2010
        Public Property CheckImage() As String
            Get
                Return OKIconNameAttribute
            End Get
            Set(ByVal value As String)
                OKIconNameAttribute = value
                FormulaStatusImage.ImageLocation = OKIconNameAttribute
            End Set
        End Property

        ' DL 12/11/2010
        Public Property CancelImage() As String
            Get
                Return WRONGIconNameAttribute
            End Get

            Set(ByVal value As String)
                WRONGIconNameAttribute = value
                FormulaStatusImage.ImageLocation = WRONGIconNameAttribute
            End Set
        End Property

        ' DL 12/11/2010
        Public Property WarningImage() As String
            Get
                Return WARNINGIconNameAttribute
            End Get

            Set(ByVal value As String)
                WARNINGIconNameAttribute = value
                FormulaStatusImage.ImageLocation = WARNINGIconNameAttribute
            End Set
        End Property
#End Region

#Region "Constructor"
        Public Sub New()
            InitializeComponent()
        End Sub

        Protected Overrides Sub Finalize()
            MyBase.Finalize()
        End Sub
#End Region

#Region "Public Events" 'SG 08/09/2010
        Public Event FormulaChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)
#End Region

#Region "Private Methods"
        ''' <summary>
        ''' Fill the ComboBox of SampleTypes
        ''' </summary>
        ''' <param name="pSampleTypesListDS">DataSet containing Code+Description of all available
        '''                                  Sample Types</param>
        Private Sub FillSampleTypesList(ByVal pSampleTypesListDS As DataSet)
            'Try
            Dim myDc As New DataColumn("CodDesc", System.Type.GetType("System.String"))

            pSampleTypesListDS.Tables(0).Columns.Add(myDc)
            'For Each myRow As DataRow In pSampleTypesListDS.Tables(0).Rows
            '    myRow.BeginEdit()
            '    myRow("CodDesc") = myRow("Code").ToString().TrimEnd() & "-" & myRow("Description").ToString().TrimEnd()
            '    myRow.EndEdit()
            'Next

            'Add data to the ComboBox
            SampleTypeListComboBox.DataSource = pSampleTypesListDS.Tables(0)
            SampleTypeListComboBox.DisplayMember = "Description"
            SampleTypeListComboBox.ValueMember = "Code"
            'Catch ex As Exception
            '    Throw ex
            'End Try

            'RH 26/05/2011 Remove Try/Catch because:

            '1)Do prefer using an empty throw (just "Throw") when catching and re-throwing an exception.
            'This is the best way to preserve the exception call stack.
            'http://msdn.microsoft.com/en-us/library/ms229005(v=VS.90).aspx
            'http://exceptionalcode.wordpress.com/2010/04/13/net-exceptions-throw-ex-is-evil-but-throw-is-not-that-innocent/

            '2)It is a bad practice to catch an exception, do nothing with it and throw it again.
            'Catch an exception only if you want to do something with it
            'See answers 335 and 115 on:
            'http://stackoverflow.com/questions/380819/common-programming-mistakes-for-net-developers-to-avoid
        End Sub

        ''' <summary>
        ''' Create a TestTable (Code and Description) and add it to the specified attribute DS
        ''' </summary>
        Private Sub PrepareTestList(ByRef pAttributeNameDS As DataSet)
            'Try
            Dim myTestTable As New DataTable

            myTestTable.Columns.Add("Code", System.Type.GetType("System.String"))
            myTestTable.Columns.Add("Description", System.Type.GetType("System.String"))
            myTestTable.TableName = "Test"

            pAttributeNameDS = New DataSet
            pAttributeNameDS.Tables.Add(myTestTable)
            'Catch ex As Exception
            '    Throw ex
            'End Try

            'RH 26/05/2011 Remove Try/Catch because:

            '1)Do prefer using an empty throw (just "Throw") when catching and re-throwing an exception.
            'This is the best way to preserve the exception call stack.
            'http://msdn.microsoft.com/en-us/library/ms229005(v=VS.90).aspx
            'http://exceptionalcode.wordpress.com/2010/04/13/net-exceptions-throw-ex-is-evil-but-throw-is-not-that-innocent/

            '2)It is a bad practice to catch an exception, do nothing with it and throw it again.
            'Catch an exception only if you want to do something with it
            'See answers 335 and 115 on:
            'http://stackoverflow.com/questions/380819/common-programming-mistakes-for-net-developers-to-avoid
        End Sub

        ''' <summary>
        ''' Fill the ListView of Standard Tests
        ''' </summary>
        ''' <param name="pTestsListDataSet">DataSet containing the Standard Tests to load</param>
        ''' <remarks>
        ''' Created by:  DL 13/05/2010
        ''' AG 02/09/2014 - BA-1869 get information about components availability
        ''' </remarks>
        Private Sub FillStandardListView(ByVal pTestsListDataSet As DataSet)
            'Try
            If (Not pTestsListDataSet Is Nothing) Then
                If (pTestsListDataSet.Tables.Count > 0) Then
                    'Apply filter in case there is one set
                    FilterTestList(pTestsListDataSet)

                    'Configure the ListView
                    InitializeStandardTestList()

                    'Assing the imageList
                    bsStandardTestListView.SmallImageList = FillTestImageList(pTestsListDataSet.Tables(0))

                    Dim imageIndex As Integer = 0
                    For Each myTest As DataRow In pTestsListDataSet.Tables(0).Rows
                        'Validate if the Image Path exists in the Image Path control
                        If (imagePathControl.ContainsKey(myTest("IconPath").ToString())) Then
                            'Get de Image index 
                            imageIndex = CInt(imagePathControl.Item(myTest("IconPath").ToString()).ToString())
                        Else
                            imageIndex = -1
                        End If

                        'Insert Standard Tests in the ListView
                        With bsStandardTestListView.Items.Add(myTest("TestName").ToString().TrimEnd(), imageIndex)
                            .SubItems.Add(myTest("TestTypeCode").ToString().TrimEnd())
                            .SubItems.Add(myTest("TestCode").ToString().TrimEnd())
                            .SubItems.Add(myTest("SampleTypeCode").ToString().TrimEnd())
                            'TR 09/03/2011 -Add the FactoryCalib item.
                            .SubItems.Add(myTest("FactoryCalib").ToString().TrimEnd())
                            'TR 09/03/2011 -END.
                            .SubItems.Add(myTest("Available").ToString().TrimEnd()) 'AG 02/09/2014 - BA-1869
                        End With
                    Next myTest
                End If
            End If
            'Catch ex As Exception
            '    Throw ex
            'End Try

            'RH 26/05/2011 Remove Try/Catch because:

            '1)Do prefer using an empty throw (just "Throw") when catching and re-throwing an exception.
            'This is the best way to preserve the exception call stack.
            'http://msdn.microsoft.com/en-us/library/ms229005(v=VS.90).aspx
            'http://exceptionalcode.wordpress.com/2010/04/13/net-exceptions-throw-ex-is-evil-but-throw-is-not-that-innocent/

            '2)It is a bad practice to catch an exception, do nothing with it and throw it again.
            'Catch an exception only if you want to do something with it
            'See answers 335 and 115 on:
            'http://stackoverflow.com/questions/380819/common-programming-mistakes-for-net-developers-to-avoid
        End Sub

        ''' <summary>
        ''' Fill the ListView of Calculated Tests
        ''' </summary>
        ''' <param name="pTestsListDataSet">DataSet containing the Calculated Tests to load</param>
        ''' <remarks>
        ''' Created by:  DL 13/05/2010
        ''' AG 02/09/2014 - BA-1869 get information about components availability
        ''' </remarks>
        Private Sub FillCalculatedListView(ByVal pTestsListDataSet As DataSet)
            'Try
            If (Not pTestsListDataSet Is Nothing) Then
                If (pTestsListDataSet.Tables.Count > 0) Then
                    'Apply filter in case there is one set
                    FilterTestList(pTestsListDataSet)

                    'Configure the ListView
                    InitializeCalculatedTestList()

                    'Assing the imageList
                    bsCalculatedTestListView.SmallImageList = FillTestImageList(pTestsListDataSet.Tables(0))

                    Dim imageIndex As Integer = 0
                    For Each myTest As DataRow In pTestsListDataSet.Tables(0).Rows
                        'Validate if the Image Path exists in the Image Path control
                        If (imagePathControl.ContainsKey(myTest("IconPath").ToString())) Then
                            'Get the Image Index
                            imageIndex = CInt(imagePathControl.Item(myTest("IconPath").ToString()).ToString())
                        Else
                            imageIndex = -1
                        End If

                        'Insert Calculated Tests in the ListView
                        With bsCalculatedTestListView.Items.Add(myTest("TestName").ToString().TrimEnd(), imageIndex)
                            .SubItems.Add(myTest("TestTypeCode").ToString().TrimEnd())
                            .SubItems.Add(myTest("TestCode").ToString().TrimEnd())
                            .SubItems.Add(myTest("SampleTypeCode").ToString().TrimEnd())
                            .SubItems.Add(myTest("Available").ToString().TrimEnd()) 'AG 02/09/2014 - BA-1869
                        End With
                    Next myTest
                End If
            End If
            'Catch ex As Exception
            '    Throw ex
            'End Try

            'RH 26/05/2011 Remove Try/Catch because:

            '1)Do prefer using an empty throw (just "Throw") when catching and re-throwing an exception.
            'This is the best way to preserve the exception call stack.
            'http://msdn.microsoft.com/en-us/library/ms229005(v=VS.90).aspx
            'http://exceptionalcode.wordpress.com/2010/04/13/net-exceptions-throw-ex-is-evil-but-throw-is-not-that-innocent/

            '2)It is a bad practice to catch an exception, do nothing with it and throw it again.
            'Catch an exception only if you want to do something with it
            'See answers 335 and 115 on:
            'http://stackoverflow.com/questions/380819/common-programming-mistakes-for-net-developers-to-avoid
        End Sub

        ''' <summary>
        ''' Create a FormulaValuesLisTable and add it to formulaValuesListAttribute DS
        ''' </summary>
        Private Sub PrepareFormulaValuesList()
            'Try
            Dim myFormulaValuesLisTable As New DataTable

            myFormulaValuesLisTable.Columns.Add("CalcTestID", System.Type.GetType("System.String"))
            myFormulaValuesLisTable.Columns.Add("Position", System.Type.GetType("System.Int32"))
            myFormulaValuesLisTable.Columns.Add("ValueType", System.Type.GetType("System.String"))
            myFormulaValuesLisTable.Columns.Add("Value", System.Type.GetType("System.String"))
            myFormulaValuesLisTable.Columns.Add("SampleType", System.Type.GetType("System.String"))
            myFormulaValuesLisTable.Columns.Add("TestType", System.Type.GetType("System.String"))
            myFormulaValuesLisTable.Columns.Add("TestName", System.Type.GetType("System.String"))
            myFormulaValuesLisTable.Columns.Add("Available", System.Type.GetType("System.String")) 'AG 02/09/2014 - BA-1869

            FormulaValuesListAttribute = New DataSet
            FormulaValuesListAttribute.Tables.Add(myFormulaValuesLisTable)

            nPosition = 0
            'Catch ex As Exception
            '    Throw ex
            'End Try

            'RH 26/05/2011 Remove Try/Catch because:

            '1)Do prefer using an empty throw (just "Throw") when catching and re-throwing an exception.
            'This is the best way to preserve the exception call stack.
            'http://msdn.microsoft.com/en-us/library/ms229005(v=VS.90).aspx
            'http://exceptionalcode.wordpress.com/2010/04/13/net-exceptions-throw-ex-is-evil-but-throw-is-not-that-innocent/

            '2)It is a bad practice to catch an exception, do nothing with it and throw it again.
            'Catch an exception only if you want to do something with it
            'See answers 335 and 115 on:
            'http://stackoverflow.com/questions/380819/common-programming-mistakes-for-net-developers-to-avoid
        End Sub

        ''' <summary>
        ''' Manages actions in Formula when a keyboard button is clicked
        ''' </summary>
        ''' <param name="myButton">Button clicked</param>
        Private Sub EnterFomulaValue(ByVal myButton As String)
            'Try
            'Store the formula length before adding a new member or doing anything 
            'Dim myFormulaSize As Integer = FormulaTextBox.Text.Length

            'Clear the Error Provider
            bsFormulaErrorProvider.Clear()

            'Validate the clicked Button
            Select Case myButton
                Case "BackButton"
                    EnterSelectedDelete()

                Case "ClearButton"
                    FormulaTextBox.Clear()
                    FormulaValuesListAttribute = Nothing
                    PrepareFormulaValuesList()

                Case "DotButton"
                    EnterSelectedNumber(".")

                Case "AddSelectTestButton"
                    EnterSelectedTest()

                Case Else
                    Dim mySymbol As String = ""

                    Select Case myButton
                        Case "ZeroButton"
                            mySymbol = "0"
                        Case "OneButton"
                            mySymbol = "1"
                        Case "TwoButton"
                            mySymbol = "2"
                        Case "ThreeButton"
                            mySymbol = "3"
                        Case "FourButton"
                            mySymbol = "4"
                        Case "FiveButton"
                            mySymbol = "5"
                        Case "SixButton"
                            mySymbol = "6"
                        Case "SevenButton"
                            mySymbol = "7"
                        Case "EightButton"
                            mySymbol = "8"
                        Case "NineButton"
                            mySymbol = "9"
                            '
                        Case "MinusButton"
                            mySymbol = "-"
                        Case "AddButton"
                            mySymbol = "+"
                        Case "DivisionButton"
                            mySymbol = "/"
                        Case "OpenParenthesisButton"
                            mySymbol = "("
                        Case "CloseParenthesisButton"
                            mySymbol = ")"
                        Case "MultButton"
                            mySymbol = "*"
                    End Select

                    If (IsNumeric(mySymbol)) Then
                        EnterSelectedNumber(mySymbol)
                    Else
                        EnterSelectedCommand(mySymbol)
                    End If
            End Select

            RefreshFormula()
            ValidateSintaxStatus()

            'Catch ex As Exception
            '    Throw ex
            'End Try

            'RH 26/05/2011 Remove Try/Catch because:

            '1)Do prefer using an empty throw (just "Throw") when catching and re-throwing an exception.
            'This is the best way to preserve the exception call stack.
            'http://msdn.microsoft.com/en-us/library/ms229005(v=VS.90).aspx
            'http://exceptionalcode.wordpress.com/2010/04/13/net-exceptions-throw-ex-is-evil-but-throw-is-not-that-innocent/

            '2)It is a bad practice to catch an exception, do nothing with it and throw it again.
            'Catch an exception only if you want to do something with it
            'See answers 335 and 115 on:
            'http://stackoverflow.com/questions/380819/common-programming-mistakes-for-net-developers-to-avoid
        End Sub

        ''' <summary>
        ''' Add the selected Test as last Formula member
        ''' </summary>
        ''' <remarks>
        ''' Created by:  DL 13/05/2010
        ''' AG 02/09/2014 - BA-1869 get information about components availability
        ''' </remarks>
        Private Sub EnterSelectedTest()
            'Try
            If FormulaValuesListAttribute Is Nothing Then PrepareFormulaValuesList()

            If (bsStandardTestListView.SelectedItems.Count > 0) Then
                For Each selectItem As ListViewItem In bsStandardTestListView.SelectedItems
                    Dim myRow As DataRow
                    myRow = FormulaValuesListAttribute.Tables(0).NewRow()

                    myRow("CalcTestID") = CalcTestIDAttribute
                    myRow("Position") = FormulaValuesListAttribute.Tables(0).Rows.Count + 1
                    myRow("ValueType") = "TEST"
                    myRow("Value") = selectItem.SubItems(2).Text
                    myRow("TestName") = selectItem.Text & " [" & selectItem.SubItems(3).Text & "]"
                    myRow("SampleType") = selectItem.SubItems(3).Text
                    myRow("TestType") = selectItem.SubItems(1).Text
                    myRow("Available") = CBool(selectItem.SubItems(5).Text) 'AG 02/09/2014 - BA-1869

                    FormulaValuesListAttribute.Tables(0).Rows.Add(myRow)
                    nPosition = FormulaValuesListAttribute.Tables(0).Rows.Count
                Next selectItem
            End If

            If (bsCalculatedTestListView.SelectedItems.Count > 0) Then
                For Each selectItem As ListViewItem In bsCalculatedTestListView.SelectedItems
                    Dim myRow As DataRow
                    myRow = FormulaValuesListAttribute.Tables(0).NewRow()

                    myRow("CalcTestID") = CalcTestIDAttribute
                    myRow("Position") = FormulaValuesListAttribute.Tables(0).Rows.Count + 1
                    myRow("ValueType") = "TEST"
                    myRow("Value") = selectItem.SubItems(2).Text
                    myRow("TestName") = selectItem.Text & " [" & selectItem.SubItems(3).Text & "]"
                    myRow("SampleType") = selectItem.SubItems(3).Text
                    myRow("TestType") = selectItem.SubItems(1).Text
                    myRow("Available") = CBool(selectItem.SubItems(4).Text) 'AG 02/09/2014 - BA-1869

                    FormulaValuesListAttribute.Tables(0).Rows.Add(myRow)
                    nPosition = FormulaValuesListAttribute.Tables(0).Rows.Count
                Next selectItem
            End If
            'Catch ex As Exception
            '    Throw ex
            'End Try

            'RH 26/05/2011 Remove Try/Catch because:

            '1)Do prefer using an empty throw (just "Throw") when catching and re-throwing an exception.
            'This is the best way to preserve the exception call stack.
            'http://msdn.microsoft.com/en-us/library/ms229005(v=VS.90).aspx
            'http://exceptionalcode.wordpress.com/2010/04/13/net-exceptions-throw-ex-is-evil-but-throw-is-not-that-innocent/

            '2)It is a bad practice to catch an exception, do nothing with it and throw it again.
            'Catch an exception only if you want to do something with it
            'See answers 335 and 115 on:
            'http://stackoverflow.com/questions/380819/common-programming-mistakes-for-net-developers-to-avoid
        End Sub

        ''' <summary>
        ''' Add the selected Bracket or Operator as last Formula member
        ''' </summary>
        ''' <remarks>
        ''' Created by:  DL 13/05/2010
        ''' AG 02/09/2014 - BA-1869 get information about components availability
        ''' </remarks>
        Private Sub EnterSelectedCommand(ByVal pCommand As String)
            'Try
            If FormulaValuesListAttribute Is Nothing Then PrepareFormulaValuesList()

            Dim myRow As DataRow = FormulaValuesListAttribute.Tables(0).NewRow()

            myRow("CalcTestID") = CalcTestIDAttribute
            myRow("Position") = FormulaValuesListAttribute.Tables(0).Rows.Count + 1

            If (pCommand = "(" Or pCommand = ")") Then
                myRow("ValueType") = "BRAC"
            Else
                myRow("ValueType") = "OPER"
            End If
            myRow("Value") = pCommand
            myRow("SampleType") = "NULL"
            myRow("TestType") = "NULL"
            myRow("TestName") = "NULL"
            myRow("Available") = True 'AG 02/09/2014 - BA-1869

            FormulaValuesListAttribute.Tables(0).Rows.Add(myRow)
            nPosition = FormulaValuesListAttribute.Tables(0).Rows.Count
            'Catch ex As Exception
            '    Throw ex
            'End Try

            'RH 26/05/2011 Remove Try/Catch because:

            '1)Do prefer using an empty throw (just "Throw") when catching and re-throwing an exception.
            'This is the best way to preserve the exception call stack.
            'http://msdn.microsoft.com/en-us/library/ms229005(v=VS.90).aspx
            'http://exceptionalcode.wordpress.com/2010/04/13/net-exceptions-throw-ex-is-evil-but-throw-is-not-that-innocent/

            '2)It is a bad practice to catch an exception, do nothing with it and throw it again.
            'Catch an exception only if you want to do something with it
            'See answers 335 and 115 on:
            'http://stackoverflow.com/questions/380819/common-programming-mistakes-for-net-developers-to-avoid
        End Sub

        ''' <summary>
        ''' Delete last Formula member (when Back key is pressed)
        ''' </summary>
        ''' <remarks>
        ''' Created by:  DL 13/05/2010
        ''' </remarks>
        Private Sub EnterSelectedDelete()
            'Try
            nPosition = FormulaValuesListAttribute.Tables(0).Rows.Count
            If (Not FormulaValuesListAttribute Is Nothing And nPosition > 0) Then
                If (FormulaValuesListAttribute.Tables(0).Rows(nPosition - 1).Item("ValueType").ToString = "NINT") Then
                    Dim myValue As String = FormulaValuesListAttribute.Tables(0).Rows(nPosition - 1).Item("Value").ToString
                    myValue = Mid(myValue, 1, Len(myValue) - 1)

                    If (myValue = "") Then
                        FormulaValuesListAttribute.Tables(0).Rows(nPosition - 1).Delete()
                        nPosition = nPosition - 1
                    Else
                        FormulaValuesListAttribute.Tables(0).Rows(nPosition - 1).Item("Value") = myValue
                    End If
                Else
                    If (FormulaValuesListAttribute.Tables(0).Rows(nPosition - 1).Item("ValueType").ToString = "TEST") Then
                        EnableStatusAttribute = False
                    End If

                    FormulaValuesListAttribute.Tables(0).Rows(nPosition - 1).Delete()
                    nPosition = nPosition - 1
                End If
                FormulaValuesListAttribute.AcceptChanges()
            End If
            'Catch ex As Exception
            '    Throw ex
            'End Try

            'RH 26/05/2011 Remove Try/Catch because:

            '1)Do prefer using an empty throw (just "Throw") when catching and re-throwing an exception.
            'This is the best way to preserve the exception call stack.
            'http://msdn.microsoft.com/en-us/library/ms229005(v=VS.90).aspx
            'http://exceptionalcode.wordpress.com/2010/04/13/net-exceptions-throw-ex-is-evil-but-throw-is-not-that-innocent/

            '2)It is a bad practice to catch an exception, do nothing with it and throw it again.
            'Catch an exception only if you want to do something with it
            'See answers 335 and 115 on:
            'http://stackoverflow.com/questions/380819/common-programming-mistakes-for-net-developers-to-avoid
        End Sub

        ''' <summary>
        ''' Add the selected Number as last Formula member
        ''' </summary>
        ''' <remarks>
        ''' Created by:  DL 13/05/2010
        ''' AG 02/09/2014 - BA-1869 get information about components availability
        ''' </remarks>
        Private Sub EnterSelectedNumber(ByVal pNumber As String)
            'Try
            Dim myIsNew As Boolean = True

            If FormulaValuesListAttribute Is Nothing Or nPosition = 0 Then PrepareFormulaValuesList()
            If (nPosition > 0 AndAlso FormulaValuesListAttribute.Tables(0).Rows.Count > 0) Then
                'Verify if the previous value is also a Number...
                If (FormulaValuesListAttribute.Tables(0).Rows(nPosition - 1).Item("ValueType").ToString = "NINT") Then
                    FormulaValuesListAttribute.Tables(0).Rows(nPosition - 1).Item("Value") = FormulaValuesListAttribute.Tables(0).Rows(nPosition - 1).Item("Value").ToString & _
                                                                                             pNumber
                    FormulaValuesListAttribute.AcceptChanges()

                    myIsNew = False
                End If
            End If

            If (myIsNew) Then
                Dim myRow As DataRow
                myRow = FormulaValuesListAttribute.Tables(0).NewRow()

                myRow("CalcTestID") = CalcTestIDAttribute
                myRow("Position") = FormulaValuesListAttribute.Tables(0).Rows.Count + 1
                myRow("ValueType") = "NINT"
                myRow("Value") = pNumber
                myRow("SampleType") = "NULL"
                myRow("TestType") = "NULL"
                myRow("TestName") = "NULL"
                myRow("Available") = True 'AG 02/09/2014 - BA-1869

                FormulaValuesListAttribute.Tables(0).Rows.Add(myRow)
                nPosition = FormulaValuesListAttribute.Tables(0).Rows.Count
            End If
            'Catch ex As Exception
            '    Throw ex
            'End Try

            'RH 26/05/2011 Remove Try/Catch because:

            '1)Do prefer using an empty throw (just "Throw") when catching and re-throwing an exception.
            'This is the best way to preserve the exception call stack.
            'http://msdn.microsoft.com/en-us/library/ms229005(v=VS.90).aspx
            'http://exceptionalcode.wordpress.com/2010/04/13/net-exceptions-throw-ex-is-evil-but-throw-is-not-that-innocent/

            '2)It is a bad practice to catch an exception, do nothing with it and throw it again.
            'Catch an exception only if you want to do something with it
            'See answers 335 and 115 on:
            'http://stackoverflow.com/questions/380819/common-programming-mistakes-for-net-developers-to-avoid
        End Sub

        ''' <summary>
        ''' Rebuilt the Formula string according values currently loaded in the FormulaValuesListAttribute DS
        ''' </summary>
        Private Sub RefreshFormula()
            'Try
            FormulaTextBox.Text = ""

            Dim myValue As String = ""
            Dim myValueType As String = ""
            For i As Integer = 0 To FormulaValuesListAttribute.Tables(0).Rows.Count - 1
                myValue = FormulaValuesListAttribute.Tables(0).Rows(i).Item("Value").ToString
                myValueType = FormulaValuesListAttribute.Tables(0).Rows(i).Item("ValueType").ToString

                If (myValueType = "NINT" Or myValueType = "NDEC") Then
                    FormulaTextBox.Text &= myValue.Replace(".", DecimalSeparator)
                ElseIf (myValueType = "TEST") Then
                    Dim myCol As Boolean = False
                    For j As Integer = 0 To FormulaValuesListAttribute.Tables(0).Columns.Count - 1
                        If (FormulaValuesListAttribute.Tables(0).Columns(j).ToString = "TestName") Then
                            myCol = True
                            Exit For
                        End If
                    Next j

                    If myCol Then
                        FormulaTextBox.Text &= FormulaValuesListAttribute.Tables(0).Rows(i).Item("TestName").ToString
                    Else
                        FormulaTextBox.Text &= myValue
                    End If

                Else
                    FormulaTextBox.Text &= myValue
                End If
            Next i

            'If the Formula is empty, then the Formula Sintax is wrong
            If (FormulaTextBox.Text.Length = 0) Then
                'FormulaStatusImage.Image = FormulaStatusImageList.Images("CANCELF")
                FormulaStatusImage.ImageLocation = WRONGIconNameAttribute
            End If

            'Catch ex As Exception
            '    Throw ex
            'End Try

            'RH 26/05/2011 Remove Try/Catch because:

            '1)Do prefer using an empty throw (just "Throw") when catching and re-throwing an exception.
            'This is the best way to preserve the exception call stack.
            'http://msdn.microsoft.com/en-us/library/ms229005(v=VS.90).aspx
            'http://exceptionalcode.wordpress.com/2010/04/13/net-exceptions-throw-ex-is-evil-but-throw-is-not-that-innocent/

            '2)It is a bad practice to catch an exception, do nothing with it and throw it again.
            'Catch an exception only if you want to do something with it
            'See answers 335 and 115 on:
            'http://stackoverflow.com/questions/380819/common-programming-mistakes-for-net-developers-to-avoid
        End Sub

        ''' <summary>
        ''' Create a ImageList with all the required images for the ListViews of Tests
        ''' </summary>
        ''' <param name="ImagePathTable">Table containig all the images path</param>
        ''' <returns>An image list control with all the icon images for the screen List Views and 
        '''          graphical Icons</returns>
        Private Function FillTestImageList(ByVal ImagePathTable As DataTable) As ImageList
            Dim myImageList As New ImageList  'Image list containing all the images
            imagePathControl = New Hashtable  'Initialize the imagepath Object

            'Try
            For Each ImagePathRow As DataRow In ImagePathTable.Rows
                'Validate if the data table contains rows
                If (Not ImagePathRow("IconPath") Is Nothing AndAlso ImagePathRow("IconPath").ToString() <> "") Then
                    If (System.IO.File.Exists(ImagePathRow("IconPath").ToString())) Then
                        'Validate if the image is in the ImageList control 
                        If (Not imagePathControl.ContainsKey(ImagePathRow("IconPath").ToString())) Then
                            'Add the Image to the ImageList control
                            myImageList.Images.Add(Image.FromFile(ImagePathRow("IconPath").ToString(), True))

                            'Add ImagePath to the HashTable control as key value 
                            imagePathControl.Add(ImagePathRow("IconPath").ToString(), myImageList.Images.Count - 1)
                        End If
                    End If
                End If
            Next ImagePathRow
            'Catch ex As Exception
            '    Throw ex
            'End Try

            'RH 26/05/2011 Remove Try/Catch because:

            '1)Do prefer using an empty throw (just "Throw") when catching and re-throwing an exception.
            'This is the best way to preserve the exception call stack.
            'http://msdn.microsoft.com/en-us/library/ms229005(v=VS.90).aspx
            'http://exceptionalcode.wordpress.com/2010/04/13/net-exceptions-throw-ex-is-evil-but-throw-is-not-that-innocent/

            '2)It is a bad practice to catch an exception, do nothing with it and throw it again.
            'Catch an exception only if you want to do something with it
            'See answers 335 and 115 on:
            'http://stackoverflow.com/questions/380819/common-programming-mistakes-for-net-developers-to-avoid
            Return myImageList
        End Function

        ''' <summary>
        ''' Reload the list of Standard and Calculated Tests when a new SampleType is selected in the
        ''' correspondent ComboBox
        ''' </summary>
        Private Sub FilterTestListBySampleType()
            'Try
            Dim tmpStdDataDS As New AllowedTestsDS
            Dim tmpCalDataDS As New AllowedTestsDS

            'Filter the list of Standard Tests
            If (TestStandardListAttribute IsNot Nothing AndAlso TestStandardListAttribute.Tables(0).Rows.Count > 0) Then
                Dim standardTestDataDS As New AllowedTestsDS
                standardTestDataDS = DirectCast(TestStandardListAttribute, AllowedTestsDS)

                Dim qSelectedTest As New List(Of tparAllowedTestsRow)
                qSelectedTest = (From a In standardTestDataDS.tparAllowedTests _
                                Where a.SampleTypeCode = SampleTypeListComboBox.SelectedValue.ToString _
                               Select a).ToList

                For Each standardRow As AllowedTestsDS.tparAllowedTestsRow In qSelectedTest
                    tmpStdDataDS.tparAllowedTests.ImportRow(standardRow)
                Next standardRow

                Me.TestStandardSampleTypeList = tmpStdDataDS
            End If

            'Filter the list of Calculated Tests
            If (TestCalculatedListAttribute IsNot Nothing AndAlso TestCalculatedListAttribute.Tables(0).Rows.Count > 0) Then
                Dim calculatedTestDataDS As New AllowedTestsDS
                calculatedTestDataDS = DirectCast(TestCalculatedListAttribute, AllowedTestsDS)

                Dim qSelectedTest As New List(Of tparAllowedTestsRow)
                qSelectedTest = (From a In calculatedTestDataDS.tparAllowedTests _
                                Where a.SampleTypeCode = SampleTypeListComboBox.SelectedValue.ToString _
                                  And a.TestCode <> CalcTestIDAttribute.ToString _
                               Select a).ToList

                'Add all selected Tests to the DataSet to return
                For Each calculatedRow As AllowedTestsDS.tparAllowedTestsRow In qSelectedTest
                    tmpCalDataDS.tparAllowedTests.ImportRow(calculatedRow)
                Next calculatedRow

                Me.TestCalculatedSampleTypeList = tmpCalDataDS
            End If
            'Catch ex As Exception
            '    Throw ex
            'End Try

            'RH 26/05/2011 Remove Try/Catch because:

            '1)Do prefer using an empty throw (just "Throw") when catching and re-throwing an exception.
            'This is the best way to preserve the exception call stack.
            'http://msdn.microsoft.com/en-us/library/ms229005(v=VS.90).aspx
            'http://exceptionalcode.wordpress.com/2010/04/13/net-exceptions-throw-ex-is-evil-but-throw-is-not-that-innocent/

            '2)It is a bad practice to catch an exception, do nothing with it and throw it again.
            'Catch an exception only if you want to do something with it
            'See answers 335 and 115 on:
            'http://stackoverflow.com/questions/380819/common-programming-mistakes-for-net-developers-to-avoid
        End Sub

        ''' <summary>
        ''' Receive a TestList by reference and create and apply a filter depending on the 
        ''' filtering options
        ''' </summary>
        ''' <param name="pTestsListDataSet">List of Tests to filter</param>
        Private Sub FilterTestList(ByRef pTestsListDataSet As DataSet)
            'Try
            Dim testFilterString As String = ""
            If (Not SampleTypeListComboBox.SelectedValue Is Nothing) Then
                testFilterString += " SampleTypeCode = '" & SampleTypeListComboBox.SelectedValue.ToString & "'"
            End If

            'Clean the row filter
            pTestsListDataSet.Tables(0).DefaultView.RowFilter = ""

            'Set the new row filter
            pTestsListDataSet.Tables(0).DefaultView.RowFilter = testFilterString
            'Catch ex As Exception
            '    Throw ex
            'End Try

            'RH 26/05/2011 Remove Try/Catch because:

            '1)Do prefer using an empty throw (just "Throw") when catching and re-throwing an exception.
            'This is the best way to preserve the exception call stack.
            'http://msdn.microsoft.com/en-us/library/ms229005(v=VS.90).aspx
            'http://exceptionalcode.wordpress.com/2010/04/13/net-exceptions-throw-ex-is-evil-but-throw-is-not-that-innocent/

            '2)It is a bad practice to catch an exception, do nothing with it and throw it again.
            'Catch an exception only if you want to do something with it
            'See answers 335 and 115 on:
            'http://stackoverflow.com/questions/380819/common-programming-mistakes-for-net-developers-to-avoid
        End Sub

        '''' <summary>
        '''' Evaluate the value to add to the Formula and return its Value Type
        '''' (Operator, Bracket, Decimal Number, Integer Number or Test)
        '''' </summary>
        '''' <param name="pValueToEval">Value to get its type</param>
        '''' <returns>Returns the type of the value</returns>
        'Private Function GetValueType(ByVal pValueToEval As String) As ValueTypes
        '    Dim myResultType As ValueTypes = ValueTypes.None
        '    'Try
        '    Select Case pValueToEval
        '        Case CChar("+"), CChar("-"), CChar("*"), CChar("/")  'OPERATORS
        '            myResultType = ValueTypes.Oper
        '        Case CChar("("), CChar(")")                          'BRACKETS
        '            myResultType = ValueTypes.Brac
        '        Case Else
        '            'Replace the ',' by '.' to evaluate Decimals 
        '            Dim myValueNum As String = pValueToEval.Replace(",", ".")

        '            'Validate if value is Numeric
        '            If (IsNumeric(myValueNum)) Then
        '                'Validate by regular expression if it is a Decimal
        '                If (myValueNum.Split(CChar(".")).Length > 1) Then
        '                    myResultType = ValueTypes.NDec         'DECIMAL NUMBER
        '                Else
        '                    myResultType = ValueTypes.NInt         'INTEGER NUMBER
        '                End If
        '            Else
        '                'Then the value is a TEST
        '                myResultType = ValueTypes.Test
        '            End If
        '    End Select
        '    'Catch ex As Exception
        '    '    Throw ex
        '    'End Try

        '    'RH 26/05/2011 Remove Try/Catch because:

        '    '1)Do prefer using an empty throw (just "Throw") when catching and re-throwing an exception.
        '    'This is the best way to preserve the exception call stack.
        '    'http://msdn.microsoft.com/en-us/library/ms229005(v=VS.90).aspx
        '    'http://exceptionalcode.wordpress.com/2010/04/13/net-exceptions-throw-ex-is-evil-but-throw-is-not-that-innocent/

        '    '2)It is a bad practice to catch an exception, do nothing with it and throw it again.
        '    'Catch an exception only if you want to do something with it
        '    'See answers 335 and 115 on:
        '    'http://stackoverflow.com/questions/380819/common-programming-mistakes-for-net-developers-to-avoid
        '    Return myResultType
        'End Function

        ''' <summary>
        ''' Method that execute all the mandatory Formula validations and set the proper icon depending
        ''' if the Formula Sintax is OK or has ERROR
        ''' </summary>
        ''' <remarks>
        ''' Modified by: DL 14/05/2010
        ''' </remarks>
        Private Function ValidateSintaxStatus() As Boolean
            Dim result As Boolean = ValidateFormula()

            'Try
            If result Then result = GlobalFormulaValidation()

            'If (FormulaStatusImageList.Images.Count > 0) Then
            If result Then
                FormulaStatusImage.ImageLocation = OKIconNameAttribute ' = FormulaStatusImageList.Images("ACCEPTF")
            Else
                FormulaStatusImage.ImageLocation = WRONGIconNameAttribute  'FormulaStatusImageList.Images("CANCELF")
            End If
            'End If

            FormulaSintaxStatusAttribute = result
            'Catch ex As Exception
            '    Throw ex
            'End Try

            'RH 26/05/2011 Remove Try/Catch because:

            '1)Do prefer using an empty throw (just "Throw") when catching and re-throwing an exception.
            'This is the best way to preserve the exception call stack.
            'http://msdn.microsoft.com/en-us/library/ms229005(v=VS.90).aspx
            'http://exceptionalcode.wordpress.com/2010/04/13/net-exceptions-throw-ex-is-evil-but-throw-is-not-that-innocent/

            '2)It is a bad practice to catch an exception, do nothing with it and throw it again.
            'Catch an exception only if you want to do something with it
            'See answers 335 and 115 on:
            'http://stackoverflow.com/questions/380819/common-programming-mistakes-for-net-developers-to-avoid
            Return result
        End Function

        ''' <summary>
        ''' Global validation for the Formula, validates the brakets, the final member and the first member
        ''' </summary>
        ''' <returns>True if Formula Sintax is OK; otherwise, False</returns>
        ''' <remarks>
        ''' Created by:  DL 17/05/2010
        ''' Modified by: SA 05/10/2011 - If the last formula character is an OPERATOR, then the formula sintax is wrong
        '''                              If the first formula character is an Operator different of MINUS, then the formula sintax is wrong
        ''' </remarks>
        Private Function GlobalFormulaValidation() As Boolean
            Dim result As Boolean = False
            'Try
            Dim numBracs As Integer = 0
            For i As Integer = 0 To FormulaValuesListAttribute.Tables(0).Rows.Count - 1
                If (FormulaValuesListAttribute.Tables(0).Rows(i).Item("ValueType").ToString = "BRAC") Then
                    If (FormulaValuesListAttribute.Tables(0).Rows(i).Item("Value").ToString = "(") Then numBracs += 1
                    If (FormulaValuesListAttribute.Tables(0).Rows(i).Item("Value").ToString = ")") Then numBracs -= 1
                End If
            Next i

            If (numBracs = 0) Then
                result = True

                'Validate the last formula character....
                Dim myNumRow As Integer = FormulaValuesListAttribute.Tables(0).Rows.Count - 1
                Dim myValueType As String = FormulaValuesListAttribute.Tables(0).Rows(myNumRow).Item("ValueType").ToString
                Dim myValue As String = FormulaValuesListAttribute.Tables(0).Rows(myNumRow).Item("Value").ToString

                'If (myValue = "(") Then
                '    result = True

                If (myValueType = "OPER") Then
                    'If last formula character is an OPERATOR, then the formula sintax is wrong
                    result = False
                ElseIf (myValueType = "NDEC" OrElse myValueType = "NINT") AndAlso _
                       (Mid(myValue, Len(myValue), 1) = "." OrElse Mid(myValue, Len(myValue), 1) = ",") Then
                    'If last formula character is the decimal separator, then the formula sintax is wrong
                    result = False
                End If

                If (result) Then
                    'Validate the first formula character....
                    myValueType = FormulaValuesListAttribute.Tables(0).Rows(0).Item("ValueType").ToString
                    myValue = FormulaValuesListAttribute.Tables(0).Rows(0).Item("Value").ToString

                    If (myValueType = "OPER" AndAlso myValue <> "-") Then
                        'If the first formula character is an Operator <> of MINUS, then the formula sintax is wrong
                        result = False
                    End If
                End If
            Else
                result = False
            End If
            'Catch ex As Exception
            '    Throw ex
            'End Try

            'RH 26/05/2011 Remove Try/Catch because:

            '1)Do prefer using an empty throw (just "Throw") when catching and re-throwing an exception.
            'This is the best way to preserve the exception call stack.
            'http://msdn.microsoft.com/en-us/library/ms229005(v=VS.90).aspx
            'http://exceptionalcode.wordpress.com/2010/04/13/net-exceptions-throw-ex-is-evil-but-throw-is-not-that-innocent/

            '2)It is a bad practice to catch an exception, do nothing with it and throw it again.
            'Catch an exception only if you want to do something with it
            'See answers 335 and 115 on:
            'http://stackoverflow.com/questions/380819/common-programming-mistakes-for-net-developers-to-avoid
            Return result
        End Function

        ''' <summary>
        ''' Function that validate each member added to the Formula
        ''' </summary>
        ''' <returns>True if Formula Sintax is OK; otherwise, False</returns>
        ''' <remarks>
        ''' Modified by: DL 17/05/2010
        ''' </remarks>
        Private Function ValidateFormula() As Boolean
            Dim result As Boolean = False

            'Try
            If (FormulaValuesListAttribute.Tables(0).Rows.Count > 0) Then    'count -1 
                For i As Integer = 0 To FormulaValuesListAttribute.Tables(0).Rows.Count - 1
                    Select Case (FormulaValuesListAttribute.Tables(0).Rows(i).Item("ValueType").ToString)
                        Case "OPER"
                            If (i > 0) Then
                                'DL 27/02/2012. begin
                                'If FormulaValuesListAttribute.Tables(0).Rows(i).Item("Value").ToString = "/" Then _
                                'OrElse FormulaValuesListAttribute.Tables(0).Rows(i).Item("Value").ToString = "*" Then

                                'If FormulaValuesListAttribute.Tables(0).Rows(i - 1).Item("ValueType").ToString = "TEST" Then _
                                '  OrElse FormulaValuesListAttribute.Tables(0).Rows(i - 1).Item("ValueType").ToString = "NINT" Then

                                '    If (i - 2) > 0 AndAlso FormulaValuesListAttribute.Tables(0).Rows(i - 2).Item("Value").ToString <> "(" Then
                                result = False
                                '         Exit For
                                '      End If

                                '   End If

                                'DL 27/02/2012. end
                                'Else

                                Dim myValue As String = FormulaValuesListAttribute.Tables(0).Rows(i - 1).Item("Value").ToString
                                Dim myValueType As String = FormulaValuesListAttribute.Tables(0).Rows(i - 1).Item("ValueType").ToString

                                If (myValueType = "OPER" OrElse myValueType = "TEST" OrElse (myValueType = "BRAC" AndAlso myValue = "(") OrElse _
                                   ((myValueType = "NINT" OrElse myValueType = "NDEC") AndAlso Mid(myValue, Len(myValue), 1) = ".")) Then

                                    result = False

                                Else
                                    result = True
                                End If
                                'End If

                            Else
                                result = False
                            End If

                        Case "NDEC", "NINT"
                                    If (i > 0) Then
                                        If FormulaValuesListAttribute.Tables(0).Rows(i - 1).Item("ValueType").ToString = "OPER" OrElse _
                                           (FormulaValuesListAttribute.Tables(0).Rows(i - 1).Item("ValueType").ToString = "BRAC" AndAlso _
                                            FormulaValuesListAttribute.Tables(0).Rows(i - 1).Item("Value").ToString = "(") Then
                                            'DL 08/02/2012. Not allow division by zero
                                            If FormulaValuesListAttribute.Tables(0).Rows(i - 1).Item("ValueType").ToString = "OPER" AndAlso _
                                               FormulaValuesListAttribute.Tables(0).Rows(i - 1).Item("Value").ToString = "/" AndAlso _
                                               FormulaValuesListAttribute.Tables(0).Rows(i).Item("Value").ToString = "0" Then

                                                result = False
                                            Else
                                                result = True
                                            End If
                                        Else
                                            result = False
                                        End If
                                    Else
                                        result = True
                                    End If

                        Case "TEST"
                                    If (i > 0) Then
                                        If (FormulaValuesListAttribute.Tables(0).Rows(i - 1).Item("ValueType").ToString = "OPER") OrElse _
                                           (FormulaValuesListAttribute.Tables(0).Rows(i - 1).Item("ValueType").ToString = "BRAC" AndAlso _
                                            FormulaValuesListAttribute.Tables(0).Rows(i - 1).Item("Value").ToString = "(") Then
                                            result = True
                                        Else
                                            result = False
                                        End If
                                    Else
                                        result = True
                                    End If

                        Case "BRAC"


                                    If (FormulaValuesListAttribute.Tables(0).Rows(i).Item("Value").ToString = "(") Then
                                        If (i > 0) Then
                                            If (FormulaValuesListAttribute.Tables(0).Rows(i - 1).Item("ValueType").ToString = "OPER") OrElse _
                                               (FormulaValuesListAttribute.Tables(0).Rows(i - 1).Item("Value").ToString = "(") Then
                                                result = True
                                            Else
                                                result = False
                                                Exit For
                                            End If
                                        Else
                                            result = True
                                        End If
                                    ElseIf (FormulaValuesListAttribute.Tables(0).Rows(i).Item("Value").ToString = ")") Then
                                        If (i > 0) Then
                                            Select Case (FormulaValuesListAttribute.Tables(0).Rows(i - 1).Item("ValueType").ToString)
                                                Case "OPER"
                                                    result = False
                                                Case "BRAC"
                                                    If (FormulaValuesListAttribute.Tables(0).Rows(i - 1).Item("Value").ToString = "(") Then
                                                        result = False
                                                    End If
                                                Case Else
                                                    'Check than exist any open brac
                                                    Dim myOpenBrac As Integer = 0
                                                    For j As Integer = 0 To i - 1
                                                        If (FormulaValuesListAttribute.Tables(0).Rows(j).Item("ValueType").ToString = "BRAC") Then
                                                            If (FormulaValuesListAttribute.Tables(0).Rows(j).Item("Value").ToString = "(") Then
                                                                myOpenBrac += 1
                                                            Else
                                                                myOpenBrac -= 1
                                                            End If
                                                        End If
                                                    Next j
                                                    If (myOpenBrac > 0) Then
                                                        result = True
                                                    Else
                                                        result = False
                                                    End If
                                            End Select
                                        Else
                                            result = False
                                        End If
                                    End If
                    End Select
                    'If Not result Then Exit For
                Next i
            Else
                result = False
            End If
            'Catch ex As Exception
            '    Throw ex
            'End Try

            'RH 26/05/2011 Remove Try/Catch because:

            '1)Do prefer using an empty throw (just "Throw") when catching and re-throwing an exception.
            'This is the best way to preserve the exception call stack.
            'http://msdn.microsoft.com/en-us/library/ms229005(v=VS.90).aspx
            'http://exceptionalcode.wordpress.com/2010/04/13/net-exceptions-throw-ex-is-evil-but-throw-is-not-that-innocent/

            '2)It is a bad practice to catch an exception, do nothing with it and throw it again.
            'Catch an exception only if you want to do something with it
            'See answers 335 and 115 on:
            'http://stackoverflow.com/questions/380819/common-programming-mistakes-for-net-developers-to-avoid
            Return result
        End Function

        '''' <summary>
        '''' Generate the Formula String from the DataSet containing the Formula values list
        '''' </summary>
        '''' <returns>Formula string</returns>
        '''' <remarks>
        '''' Modified by: DL 14/05/2010
        '''' </remarks>
        'Private Function GetFormulaFromFormulaValueList() As String
        '    Dim result As String = ""
        '    'Try
        '    If (Not FormulaValuesList Is Nothing) Then
        '        If (Not FormulaValuesListAttribute.Tables(0) Is Nothing AndAlso FormulaValuesListAttribute.Tables(0).Rows.Count > 0) Then
        '            For Each formulaValRow As DataRow In FormulaValuesListAttribute.Tables(0).Rows
        '                'Get the next Formula value and build the String
        '                result += formulaValRow("Value").ToString()
        '            Next
        '        End If
        '    End If
        '    'Catch ex As Exception
        '    '    Throw ex
        '    'End Try

        '    'RH 26/05/2011 Remove Try/Catch because:

        '    '1)Do prefer using an empty throw (just "Throw") when catching and re-throwing an exception.
        '    'This is the best way to preserve the exception call stack.
        '    'http://msdn.microsoft.com/en-us/library/ms229005(v=VS.90).aspx
        '    'http://exceptionalcode.wordpress.com/2010/04/13/net-exceptions-throw-ex-is-evil-but-throw-is-not-that-innocent/

        '    '2)It is a bad practice to catch an exception, do nothing with it and throw it again.
        '    'Catch an exception only if you want to do something with it
        '    'See answers 335 and 115 on:
        '    'http://stackoverflow.com/questions/380819/common-programming-mistakes-for-net-developers-to-avoid
        '    Return result
        'End Function
#End Region

#Region "Public Methods"
        ''' <summary>
        ''' Initialization of the Standard Tests ListView
        ''' </summary>
        Public Sub InitializeStandardTestList()
            'Try
            bsStandardTestListView.Items.Clear()
            bsStandardTestListView.Columns.Clear()
            bsStandardTestListView.View = View.Details
            'bsStandardTestListView.GridLines = True
            bsStandardTestListView.FullRowSelect = True
            bsStandardTestListView.Alignment = ListViewAlignment.Left
            bsStandardTestListView.HideSelection = False
            bsStandardTestListView.HeaderStyle = ColumnHeaderStyle.None
            bsStandardTestListView.MultiSelect = False
            bsStandardTestListView.Scrollable = True

            bsStandardTestListView.Columns.Add("TestName", 160, HorizontalAlignment.Left)
            bsStandardTestListView.Columns.Add("TestTypeCode", 0, HorizontalAlignment.Left)
            bsStandardTestListView.Columns.Add("TestCode", 0, HorizontalAlignment.Left)
            bsStandardTestListView.Columns.Add("SampleTypeCode", 0, HorizontalAlignment.Center)
            'Catch ex As Exception
            '    Throw ex
            'End Try

            'RH 26/05/2011 Remove Try/Catch because:

            '1)Do prefer using an empty throw (just "Throw") when catching and re-throwing an exception.
            'This is the best way to preserve the exception call stack.
            'http://msdn.microsoft.com/en-us/library/ms229005(v=VS.90).aspx
            'http://exceptionalcode.wordpress.com/2010/04/13/net-exceptions-throw-ex-is-evil-but-throw-is-not-that-innocent/

            '2)It is a bad practice to catch an exception, do nothing with it and throw it again.
            'Catch an exception only if you want to do something with it
            'See answers 335 and 115 on:
            'http://stackoverflow.com/questions/380819/common-programming-mistakes-for-net-developers-to-avoid
        End Sub

        ''' <summary>
        ''' Initialization of the Calculated Tests ListView   
        ''' </summary>
        Public Sub InitializeCalculatedTestList()
            'Try
            bsCalculatedTestListView.Items.Clear()
            bsCalculatedTestListView.Columns.Clear()
            bsCalculatedTestListView.View = View.Details
            'bsCalculatedTestListView.GridLines = True ' DL 19/07/2011
            bsCalculatedTestListView.FullRowSelect = True
            bsCalculatedTestListView.Alignment = ListViewAlignment.Left
            bsCalculatedTestListView.HideSelection = False
            bsCalculatedTestListView.HeaderStyle = ColumnHeaderStyle.None
            bsCalculatedTestListView.MultiSelect = False
            bsCalculatedTestListView.Scrollable = True

            bsCalculatedTestListView.Columns.Add("TestName", 160, HorizontalAlignment.Left)
            bsCalculatedTestListView.Columns.Add("TestTypeCode", 0, HorizontalAlignment.Left)
            bsCalculatedTestListView.Columns.Add("TestCode", 0, HorizontalAlignment.Left)
            bsCalculatedTestListView.Columns.Add("SampleTypeCode", 0, HorizontalAlignment.Center)
            'Catch ex As Exception
            '    Throw ex
            'End Try

            'RH 26/05/2011 Remove Try/Catch because:

            '1)Do prefer using an empty throw (just "Throw") when catching and re-throwing an exception.
            'This is the best way to preserve the exception call stack.
            'http://msdn.microsoft.com/en-us/library/ms229005(v=VS.90).aspx
            'http://exceptionalcode.wordpress.com/2010/04/13/net-exceptions-throw-ex-is-evil-but-throw-is-not-that-innocent/

            '2)It is a bad practice to catch an exception, do nothing with it and throw it again.
            'Catch an exception only if you want to do something with it
            'See answers 335 and 115 on:
            'http://stackoverflow.com/questions/380819/common-programming-mistakes-for-net-developers-to-avoid
        End Sub

        ''' <summary>
        ''' Set the Warning Icon for the Formula
        ''' </summary>
        ''' <remarks>
        ''' Created by: DL 14/05/2010
        ''' </remarks>
        Public Sub WarningSintaxStatus()
            'Dim result As Boolean = False
            'Try
            If (FormulaStatusImageList.Images.Count > 0) Then
                FormulaSintaxStatusAttribute = False
                FormulaStatusImage.ImageLocation = WARNINGIconNameAttribute  'FormulaStatusImageList.Images("WARNINGF") 12/11/2010
            End If
            'Catch ex As Exception
            '    Throw ex
            'End Try

            'RH 26/05/2011 Remove Try/Catch because:

            '1)Do prefer using an empty throw (just "Throw") when catching and re-throwing an exception.
            'This is the best way to preserve the exception call stack.
            'http://msdn.microsoft.com/en-us/library/ms229005(v=VS.90).aspx
            'http://exceptionalcode.wordpress.com/2010/04/13/net-exceptions-throw-ex-is-evil-but-throw-is-not-that-innocent/

            '2)It is a bad practice to catch an exception, do nothing with it and throw it again.
            'Catch an exception only if you want to do something with it
            'See answers 335 and 115 on:
            'http://stackoverflow.com/questions/380819/common-programming-mistakes-for-net-developers-to-avoid
            'Return result
        End Sub

        ''' <summary>
        ''' To enable/disable the ComboBox of Sample Types
        ''' </summary>
        Public Sub EnableSampleType(ByVal status As Boolean)
            SampleTypeListComboBox.Enabled = status
            If (SampleTypeListComboBox.Enabled) Then
                SampleTypeListComboBox.BackColor = Color.White
            Else
                SampleTypeListComboBox.BackColor = SystemColors.MenuBar
            End If

            myunique = status
        End Sub

        ''' <summary>
        ''' Allow to show the Error Provider when the Formula has not been informed
        ''' or the Formula Sintax is incorrect and the User clicks in Save button
        ''' </summary>
        ''' <param name="pMessageToShow">Text to shown in the Error Provider Control</param>
        ''' <remarks>
        ''' Created by:  SA 07/07/2010
        ''' </remarks>
        Public Sub ShowErrorProvider(ByVal pMessageToShow As String)
            bsFormulaErrorProvider.SetError(bsAuxForErrorLabel, pMessageToShow)
        End Sub
#End Region

#Region "Events"
        ''' <summary>
        ''' When click in a keyboard button (different of DEL or CLEAR), the correspondent Number, Decimal Point, Bracket
        ''' or Operator is added as last Formula member
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub AnyPadButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OneButton.Click, _
                                                                                                           TwoButton.Click, _
                                                                                                           ThreeButton.Click, _
                                                                                                           FourButton.Click, _
                                                                                                           DivisionButton.Click, _
                                                                                                           SixButton.Click, _
                                                                                                           SevenButton.Click, _
                                                                                                           OpenParenthesisButton.Click, _
                                                                                                           NineButton.Click, _
                                                                                                           MultButton.Click, _
                                                                                                           MinusButton.Click, _
                                                                                                           FiveButton.Click, _
                                                                                                           EightButton.Click, _
                                                                                                           CloseParenthesisButton.Click, _
                                                                                                           ClearButton.Click, _
                                                                                                           BackButton.Click, _
                                                                                                           AddButton.Click, _
                                                                                                           DotButton.Click, _
                                                                                                           ZeroButton.Click, _
                                                                                                           AddSelectTestButton.Click
            'Try
            'DL 08/02/2012. Begin
            'Dim myButton As New Button
            'myButton = CType(sender, Button)
            Dim myButtonName As String = DirectCast(sender, Button).Name
            EnterFomulaValue(myButtonName)
            'DL 08/02/2012. End

            'Catch ex As Exception
            '    Throw ex
            'End Try

            'RH 26/05/2011 Remove Try/Catch because:

            '1)Do prefer using an empty throw (just "Throw") when catching and re-throwing an exception.
            'This is the best way to preserve the exception call stack.
            'http://msdn.microsoft.com/en-us/library/ms229005(v=VS.90).aspx
            'http://exceptionalcode.wordpress.com/2010/04/13/net-exceptions-throw-ex-is-evil-but-throw-is-not-that-innocent/

            '2)It is a bad practice to catch an exception, do nothing with it and throw it again.
            'Catch an exception only if you want to do something with it
            'See answers 335 and 115 on:
            'http://stackoverflow.com/questions/380819/common-programming-mistakes-for-net-developers-to-avoid
        End Sub

        ''' <summary>
        ''' When double-clicking in a Standard Test, it is added as last Formula member
        ''' </summary>
        Private Sub bsStandardTestListView_DoubleClick(ByVal sender As Object, ByVal e As System.EventArgs) _
                                                       Handles bsStandardTestListView.DoubleClick
            'Try
            bsCalculatedTestListView.SelectedItems.Clear()
            EnterFomulaValue("AddSelectTestButton")

            'Catch ex As Exception
            '    Throw ex
            'End Try

            'RH 26/05/2011 Remove Try/Catch because:

            '1)Do prefer using an empty throw (just "Throw") when catching and re-throwing an exception.
            'This is the best way to preserve the exception call stack.
            'http://msdn.microsoft.com/en-us/library/ms229005(v=VS.90).aspx
            'http://exceptionalcode.wordpress.com/2010/04/13/net-exceptions-throw-ex-is-evil-but-throw-is-not-that-innocent/

            '2)It is a bad practice to catch an exception, do nothing with it and throw it again.
            'Catch an exception only if you want to do something with it
            'See answers 335 and 115 on:
            'http://stackoverflow.com/questions/380819/common-programming-mistakes-for-net-developers-to-avoid
        End Sub

        ''' <summary>
        ''' When double-clicking in a Calculated Test, it is added as last Formula member
        ''' </summary>
        Private Sub bsCalculatedTestListView_DoubleClick(ByVal sender As Object, ByVal e As System.EventArgs) _
                                                         Handles bsCalculatedTestListView.DoubleClick
            'Try
            bsStandardTestListView.SelectedItems.Clear()
            EnterFomulaValue("AddSelectTestButton")

            'Catch ex As Exception
            '    Throw ex
            'End Try

            'RH 26/05/2011 Remove Try/Catch because:

            '1)Do prefer using an empty throw (just "Throw") when catching and re-throwing an exception.
            'This is the best way to preserve the exception call stack.
            'http://msdn.microsoft.com/en-us/library/ms229005(v=VS.90).aspx
            'http://exceptionalcode.wordpress.com/2010/04/13/net-exceptions-throw-ex-is-evil-but-throw-is-not-that-innocent/

            '2)It is a bad practice to catch an exception, do nothing with it and throw it again.
            'Catch an exception only if you want to do something with it
            'See answers 335 and 115 on:
            'http://stackoverflow.com/questions/380819/common-programming-mistakes-for-net-developers-to-avoid
        End Sub

        ''' <summary>
        ''' Reload the list of Standard and Calculated Tests when a new SampleType is selected in the
        ''' correspondent ComboBox
        ''' </summary>
        Private Sub SampleTypeListComboBox_SelectedValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
                                                                Handles SampleTypeListComboBox.SelectedValueChanged
            'Try
            FilterTestListBySampleType()
            'Catch ex As Exception
            '    Throw ex
            'End Try

            'RH 26/05/2011 Remove Try/Catch because:

            '1)Do prefer using an empty throw (just "Throw") when catching and re-throwing an exception.
            'This is the best way to preserve the exception call stack.
            'http://msdn.microsoft.com/en-us/library/ms229005(v=VS.90).aspx
            'http://exceptionalcode.wordpress.com/2010/04/13/net-exceptions-throw-ex-is-evil-but-throw-is-not-that-innocent/

            '2)It is a bad practice to catch an exception, do nothing with it and throw it again.
            'Catch an exception only if you want to do something with it
            'See answers 335 and 115 on:
            'http://stackoverflow.com/questions/380819/common-programming-mistakes-for-net-developers-to-avoid
        End Sub

        ''' <summary>
        ''' Visibility of the ScrollBars according to the written lines
        ''' </summary>
        ''' <remarks>
        ''' Created by:  SG 09/07/2010
        ''' </remarks>
        Private Sub FormulaTextBox_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles FormulaTextBox.TextChanged
            'Try
            If FormulaTextBox.Lines.Count > 4 Then
                FormulaTextBox.ScrollBars = ScrollBars.Vertical
            Else
                FormulaTextBox.ScrollBars = ScrollBars.None
            End If

            If Me.EditionModeAttribute Then
                RaiseEvent FormulaChanged(Me, e)
            End If
            'Catch ex As Exception
            '    Throw ex
            'End Try

            'RH 26/05/2011 Remove Try/Catch because:

            '1)Do prefer using an empty throw (just "Throw") when catching and re-throwing an exception.
            'This is the best way to preserve the exception call stack.
            'http://msdn.microsoft.com/en-us/library/ms229005(v=VS.90).aspx
            'http://exceptionalcode.wordpress.com/2010/04/13/net-exceptions-throw-ex-is-evil-but-throw-is-not-that-innocent/

            '2)It is a bad practice to catch an exception, do nothing with it and throw it again.
            'Catch an exception only if you want to do something with it
            'See answers 335 and 115 on:
            'http://stackoverflow.com/questions/380819/common-programming-mistakes-for-net-developers-to-avoid
        End Sub

        ''' <summary>
        ''' Avoid put focus on the read only Formula TextBox
        ''' </summary>
        ''' <remarks>
        ''' Created by:  SG 09/07/2010
        ''' </remarks>
        Private Sub FormulaTextBox_GotFocus(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles FormulaTextBox.GotFocus
            'Try
            Dim c As Control = Me.GetNextControl(FormulaTextBox, True)
            c.Focus()
            'Catch ex As Exception
            '    Throw ex
            'End Try

            'RH 26/05/2011 Remove Try/Catch because:

            '1)Do prefer using an empty throw (just "Throw") when catching and re-throwing an exception.
            'This is the best way to preserve the exception call stack.
            'http://msdn.microsoft.com/en-us/library/ms229005(v=VS.90).aspx
            'http://exceptionalcode.wordpress.com/2010/04/13/net-exceptions-throw-ex-is-evil-but-throw-is-not-that-innocent/

            '2)It is a bad practice to catch an exception, do nothing with it and throw it again.
            'Catch an exception only if you want to do something with it
            'See answers 335 and 115 on:
            'http://stackoverflow.com/questions/380819/common-programming-mistakes-for-net-developers-to-avoid
        End Sub

        Private Sub bsStandardTestListView_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsStandardTestListView.SelectedIndexChanged
            'Try
            'TR 09/03/2011 -Validate if there is a selected item
            If bsStandardTestListView.SelectedItems.Count > 0 Then
                'Validate if has factory values.
                If CBool(bsStandardTestListView.SelectedItems(0).SubItems(4).Text) Then
                    MessageBox.Show(FactoryValuesMessageAttribute, "Warning")
                End If
            End If
            'Catch ex As Exception
            '    Throw ex
            'End Try

            'RH 26/05/2011 Remove Try/Catch because:

            '1)Do prefer using an empty throw (just "Throw") when catching and re-throwing an exception.
            'This is the best way to preserve the exception call stack.
            'http://msdn.microsoft.com/en-us/library/ms229005(v=VS.90).aspx
            'http://exceptionalcode.wordpress.com/2010/04/13/net-exceptions-throw-ex-is-evil-but-throw-is-not-that-innocent/

            '2)It is a bad practice to catch an exception, do nothing with it and throw it again.
            'Catch an exception only if you want to do something with it
            'See answers 335 and 115 on:
            'http://stackoverflow.com/questions/380819/common-programming-mistakes-for-net-developers-to-avoid
        End Sub
#End Region


    End Class

End Namespace
