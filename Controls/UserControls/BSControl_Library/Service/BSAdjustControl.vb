Option Strict On
Option Explicit On

Imports System.Windows.Forms
Imports System.Drawing
'Imports Biosystems.Ax00
'Imports Biosystems.Ax00.Controls
'Imports Biosystems.Ax00.Controls.UserControls
'Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global

Imports System.Runtime.InteropServices 'WIN32 necessary for showing/hiding the caret (textbox cursor)

Namespace Biosystems.Ax00.Controls.UserControls
    ''' <summary>
    ''' User Control that allows the adjustment of any setting of the Analyzer. 
    ''' It allows to increase/decrease the desired value for the working setting between predefined limits 
    ''' and also by means of writing directly into the suited textbox. It also provides to the user some 
    ''' information about the current value saved and the limits range. This user control is specially designed 
    ''' to be used by keyboard. The values obtained from this control are not directly sent to the analyzer. 
    ''' It just provides the tool for adjusting them. Therefore the values obtained are received by the parent 
    ''' form by means of the corresponding event handlers in order to manage and send them to the Analyzer. 
    ''' </summary>
    ''' <remarks>Created by SG 17/12/10</remarks>
    Public Class BSAdjustControl


#Region "Constructor"

        Public Sub New()

            ' This call is required by the Windows Form Designer.
            InitializeComponent()

            ' Add any initialization after the InitializeComponent() call.
            StepValuesAttr.Clear()

            Dim mySteps As New List(Of Integer)
            mySteps.Add(1)
            mySteps.Add(10)
            mySteps.Add(100)

            MyClass.StepValues = mySteps

            MyClass.UnFocusedBackColor = MyClass.UnFocusedBackColorAttr
            MyClass.FocusedBackColor = MyClass.FocusedBackColorAttr
            MyClass.DisplayBackColor = MyClass.DisplayBackColorAttr
            MyClass.DisplayForeColor = MyClass.DisplayForeColorAttr
            MyClass.DisplayEditingForeColor = MyClass.DisplayEditingForeColorAttr
            MyClass.InfoBackColor = MyClass.InfoBackColorAttr
            MyClass.InfoTitlesForeColor = MyClass.InfoTitlesForeColorAttr
            MyClass.InfoValuesForeColor = MyClass.InfoValuesForeColorAttr


        End Sub

#End Region

#Region "Public Business Properties"

        ''' <summary>
        ''' Value displayed in the display
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by SG 17/12/10
        ''' Modified by XBC 28/01/2013 - change CSng function by Utilities.FormatToSingle method (Bugs tracking #1122)
        ''' </remarks>
        Public Property CurrentValue() As Single
            Get
                Return CurrentValueAttr
            End Get
            Set(ByVal value As Single)

                'value = CSng(FormatDisplayValue(value).Replace(",", "."))
                Dim myUtilities As New Utilities
                value = myUtilities.FormatToSingle(FormatDisplayValue(value).Replace(",", "."))

                'SG 21/01/11
                If value < Me.MinimumLimit Or value > Me.MaximumLimit Then
                    RaiseEvent SetPointOutOfRange(Me)
                    Exit Property
                End If

                If value <> CurrentValueAttr Then

                    If CurrentActionRequested = AdjustActions.NoAction Then
                        SetPointValue = value
                        LastSetPointValue = value
                    Else
                        LastSetPointValue = SetPointValue
                    End If

                    RaiseEvent CurrentValueChanged(Me, value)
                End If

                'only if not editing the value is updated
                If Not Me.EditionMode And CurrentActionRequested = AdjustActions.NoAction Then
                    If value = SetPointValue Then
                        BSDisplayTextBox.Text = FormatDisplayValue(value)
                    End If
                End If

                CurrentActionRequested = AdjustActions.NoAction
                Me.ButtonsAreaEnabled = True
                CurrentValueAttr = value

                If Not Me.Enabled Then
                    Me.SetPointValue = CurrentValueAttr
                Else
                    Me.BSDisplayTextBox.ForeColor = DisplayForeColor
                    Me.BSDisplayTextBox.Refresh()
                End If

            End Set
        End Property

        'SGM 08/04/11
        Public Property SimulationMode() As Boolean
            Get
                Return SimulationModeAttr
            End Get
            Set(ByVal value As Boolean)
                SimulationModeAttr = value
            End Set
        End Property

        ''' <summary>
        ''' Last value of the Analyzer's setting
        ''' </summary>
        ''' <value></value>
        ''' <remarks>Created by SG 17/12/10</remarks>
        Public WriteOnly Property LastValueSaved() As Single
            Set(ByVal value As Single)
                Me.BSLastValueLabel.Text = value.ToString
                LastValueSavedAttr = value
            End Set
        End Property

        ''' <summary>
        ''' Minimum value allowed
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>Created by SG 17/12/10</remarks>
        Public Property MinimumLimit() As Single
            Get
                Return MinimumLimitAttr
            End Get
            Set(ByVal value As Single)
                MinimumLimitAttr = value
                If MinimumLimitAttr > MaximumLimitAttr Then
                    MaximumLimit = MinimumLimitAttr + CurrentStepValue
                End If
                Dim min As String = Me.MinimumLimit.ToString.Replace(",", ".")
                Dim max As String = Me.MaximumLimit.ToString.Replace(",", ".")
                Me.BSRangeValueLabel.Text = min & "/" & max
            End Set
        End Property

        ''' <summary>
        ''' Maximum value allowed
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>Created by SG 17/12/10</remarks>
        Public Property MaximumLimit() As Single
            Get
                Return MaximumLimitAttr
            End Get
            Set(ByVal value As Single)
                MaximumLimitAttr = value
                If MaximumLimitAttr < MinimumLimitAttr Then
                    MinimumLimit = MaximumLimitAttr - CurrentStepValue
                End If
                Dim min As String = Me.MinimumLimit.ToString.Replace(",", ".")
                Dim max As String = Me.MaximumLimit.ToString.Replace(",", ".")
                Me.BSRangeValueLabel.Text = min & "/" & max
            End Set
        End Property

        ''' <summary>
        ''' Value for each increasing/decreasing step
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>Created by SG 17/12/10</remarks>
        Public Property CurrentStepValue() As Integer
            Get
                Return CurrentStepValueAttr
            End Get
            Set(ByVal value As Integer)
                If StepValues.Contains(value) Then
                    CurrentStepValueAttr = value
                    ' XBC 05/10/2011
                    'MyClass.BsStepLabel.Text = "x" & CurrentStepValueAttr.ToString
                    MyClass.BSStepButton.Text = CurrentStepValueAttr.ToString ' "x" & CurrentStepValueAttr.ToString
                End If
            End Set
        End Property

        Public Property StepValues() As List(Of Integer)
            Get
                Return StepValuesAttr
            End Get
            Set(ByVal value As List(Of Integer))

                'validate
                Dim stp As Integer = 0
                Dim err As Boolean = False
                For Each S As Integer In value
                    If S <= stp Then
                        err = True
                        Exit For
                    Else
                        stp = S
                    End If
                Next

                If value.Count > 0 And Not err Then
                    StepValuesAttr = value
                    ' XBC 05/10/2011
                    'MyClass.BsStepLabel.Text = "x" & StepValuesAttr(0).ToString
                    MyClass.BSStepButton.Text = StepValuesAttr(0).ToString ' "x" & StepValuesAttr(0).ToString
                End If

            End Set
        End Property


        ''' <summary>
        ''' Edition status of the control.
        ''' If True, the user can edit the textbox and all the key events are catched by the control
        ''' If False the user cannot edit the textbox. All the key events are catched by the parent form
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>Created by SG 17/12/10</remarks>
        Public Property EditionMode() As Boolean
            Get
                Return EditionModeAttr
            End Get
            Set(ByVal value As Boolean)

                If value <> EditionModeAttr Then

                    Me.BSDisplayTextBox.TabStop = value
                    Me.BSDisplayTextBox.ReadOnly = Not value

                    Me.BSHomeButton.Enabled = Not value
                    Me.BSIncreaseButton.Enabled = Not value
                    Me.BSDecreaseButton.Enabled = Not value
                    Me.BSEnterButton.Enabled = value

                    If value Then
                        ShowCaret(Me.BSDisplayTextBox.Handle)
                        Me.BSDisplayTextBox.ForeColor = DisplayEditingForeColor
                        Me.BSUnitsLabel.ForeColor = DisplayEditingForeColor
                    Else
                        HideCaret(Me.BSDisplayTextBox.Handle)
                        Me.BSDisplayTextBox.ForeColor = DisplayForeColor
                        Me.BSUnitsLabel.ForeColor = DisplayForeColor
                        'Me.BSDisplayTextBox.SelectedText = ""
                    End If

                    ' XBC 26/10/2011
                    'Me.BSDisplayTextBox.SelectionLength = 0 
                    Me.BSDisplayTextBox.SelectAll()
                    ' XBC 26/10/2011

                    EditionModeAttr = value

                    RaiseEvent EditionModeChanged(Me, value)

                End If
            End Set
        End Property

        Public Property IsFocused() As Boolean
            Get
                Return IsFocusedAttr
            End Get
            Set(ByVal value As Boolean)
                IsFocusedAttr = value
            End Set
        End Property

#End Region

#Region "Private Business Properties"

        ''' <summary>
        ''' Value aimed to be sent to the Analyzer
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by SG 17/12/10
        ''' Modified by XBC 28/01/2013 - change CSng function by Utilities.FormatToSingle method (Bugs tracking #1122)
        ''' </remarks>
        Private Property SetPointValue() As Single
            Get
                Return SetPointValueAttr
            End Get
            Set(ByVal value As Single)

                'value = CSng(FormatDisplayValue(value).Replace(",", "."))
                Dim myUtilities As New Utilities
                value = myUtilities.FormatToSingle(FormatDisplayValue(value).Replace(",", "."))

                If value <> SetPointValueAttr Then

                    SetPointValueAttr = value

                    If Me.Enabled Then
                        If CurrentActionRequested <> AdjustActions.NoAction Then

                            'Me.BSDisplayTextBox.Text = FormatDisplayValue(value)   ' XBC 12/12/2011

                            Me.ButtonsAreaEnabled = True

                            Select Case CurrentActionRequested
                                Case AdjustActions.Enter
                                    RaiseEvent AbsoluteSetPointReleased(Me, value)

                                Case AdjustActions.Increase
                                    RaiseEvent RelativeSetPointReleased(Me, MyClass.CurrentStepValue)

                                Case AdjustActions.Decrease
                                    RaiseEvent RelativeSetPointReleased(Me, -MyClass.CurrentStepValue)

                            End Select

                            If Not SimulationModeAttr Then
                                Me.ButtonsAreaEnabled = False
                            End If

                        End If
                    End If
                End If

            End Set
        End Property

        ''' <summary>
        ''' availability of the buttons
        ''' </summary>
        ''' <value></value>
        ''' <remarks>Created by SG 17/12/10</remarks>
        Private Property ButtonsAreaEnabled() As Boolean
            Get
                Return ButtonsAreaEnabledAttr
            End Get
            Set(ByVal value As Boolean)

                If value <> ButtonsAreaEnabledAttr Then
                    Me.BSHomeButton.Enabled = value And MyClass.HomingEnabled
                    Me.BSEnterButton.Enabled = value And Me.EditionMode

                    Me.BSIncreaseButton.Enabled = value
                    Me.BSDecreaseButton.Enabled = value

                    Me.BSDisplayTextBox.Enabled = value

                    ' XBC 05/10/2011
                    'Me.BsStepLabel.Enabled = value
                    Me.BSStepButton.Enabled = value

                    If Not value Then
                        Me.BSUnitsLabel.ForeColor = Color.Gray
                    Else
                        If Me.EditionMode Then
                            Me.BSUnitsLabel.ForeColor = DisplayEditingForeColor
                        Else
                            Me.BSUnitsLabel.ForeColor = DisplayForeColor
                        End If

                        RecoverButtonFocus()

                    End If

                    ButtonsAreaEnabledAttr = value

                    If value Then
                        Me.Cursor = Cursors.Default
                    Else
                        Me.Cursor = Cursors.WaitCursor
                    End If
                End If

            End Set
        End Property

        Private Property IsFocusing() As Boolean
            Get
                Return IsFocusingAttr
            End Get
            Set(ByVal value As Boolean)
                IsFocusingAttr = value
            End Set
        End Property

#End Region

#Region "Presentation Properties"

        ''' <summary>
        ''' Font used for the display
        ''' </summary>
        ''' <value></value>
        ''' <remarks>Created by SG 17/12/10</remarks>
        Public WriteOnly Property DisplayFont() As Font
            Set(ByVal value As Font)
                If value IsNot Nothing Then
                    Me.BSDisplayTextBox.Font = value
                    DisplayFontAttr = value
                End If
            End Set
        End Property


        ''' <summary>
        ''' Allows homing
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>Created by SG 17/12/10</remarks>
        Public Property HomingEnabled() As Boolean
            Get
                Return HomingEnabledAttr
            End Get
            Set(ByVal value As Boolean)
                Me.BSHomeButton.Enabled = value
                HomingEnabledAttr = value
            End Set
        End Property

        ''' <summary>
        ''' Allows adjusting
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>Created by SG 17/12/10</remarks>
        Public Property AdjustingEnabled() As Boolean
            Get
                Return AdjustingEnabledAttr
            End Get
            Set(ByVal value As Boolean)
                Me.BSAdjustButtonsPanel.Enabled = value
                AdjustingEnabledAttr = value
            End Set
        End Property

        ''' <summary>
        ''' Allows editing
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>Created by SG 17/12/10</remarks>
        Public Property EditingEnabled() As Boolean
            Get
                Return EditingEnabledAttr
            End Get
            Set(ByVal value As Boolean)
                'Me.BSDisplayTextBox.Enabled = value
                Me.BSEnterButton.Enabled = value
                EditingEnabledAttr = value
            End Set
        End Property

        ''' <summary>
        ''' Determines which mode the adjusting buttons will be displayed
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>Created by SG 17/12/10</remarks>
        Public Property AdjustButtonMode() As AdjustButtonModes
            Get
                Return AdjustButtonModeAttr
            End Get
            Set(ByVal value As AdjustButtonModes)
                
                AdjustButtonModeAttr = value
                MyClass.UpdateButtonImages(Me.BSIncreaseButton)
                MyClass.UpdateButtonImages(Me.BSDecreaseButton)

            End Set
        End Property

        ''' <summary>
        ''' Determines the directness of the increasement
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property IncreaseMode() As IncreaseModes
            Get
                Return IncreaseModeAttr
            End Get
            Set(ByVal value As IncreaseModes)
                IncreaseModeAttr = value
            End Set
        End Property

        ''' <summary>
        ''' units text for the adjusting value
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>Created by SG 17/12/10</remarks>
        Public Property UnitsCaption() As String
            Get
                Return UnitsCaptionAttr
            End Get
            Set(ByVal value As String)
                Me.BSUnitsLabel.Text = value
                UnitsCaptionAttr = value
            End Set
        End Property

        ''' <summary>
        ''' Control's backcolor when the control is not focused
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>Created by SG 17/12/10</remarks>
        Public Property UnFocusedBackColor() As Color
            Get
                Return UnFocusedBackColorAttr
            End Get
            Set(ByVal value As Color)
                UnFocusedBackColorAttr = value
            End Set
        End Property


        ''' <summary>
        ''' Control's backcolor when the control is focused
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>Created by SG 17/12/10</remarks>
        Public Property FocusedBackColor() As Color
            Get
                Return FocusedBackColorAttr
            End Get
            Set(ByVal value As Color)
                FocusedBackColorAttr = value
            End Set
        End Property

        ''' <summary>
        ''' Control's display's backcolor
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>Created by SG 17/12/10</remarks>
        Public Property DisplayBackColor() As Color
            Get
                Return DisplayBackColorAttr
            End Get
            Set(ByVal value As Color)
                Me.BSDisplayTextBox.BackColor = value
                Me.BSUnitsLabel.BackColor = value
                DisplayBackColorAttr = value
            End Set
        End Property

        ''' <summary>
        ''' Control's display's text's color while editing
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>Created by SG 17/12/10</remarks>
        Public Property DisplayEditingForeColor() As Color
            Get
                Return DisplayEditingForeColorAttr
            End Get
            Set(ByVal value As Color)
                DisplayEditingForeColorAttr = value
            End Set
        End Property

        ''' <summary>
        ''' Control's display's text's color
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>Created by SG 17/12/10</remarks>
        Public Property DisplayForeColor() As Color
            Get
                Return DisplayForeColorAttr
            End Get
            Set(ByVal value As Color)
                DisplayForeColorAttr = value
                Me.BSUnitsLabel.ForeColor = value
            End Set
        End Property

        ''' <summary>
        ''' Control's info area's backcolor
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>Created by SG 17/12/10</remarks>
        Public Property InfoBackColor() As Color
            Get
                Return InfoBackColorAttr
            End Get
            Set(ByVal value As Color)
                Me.BsRangePanel.BackColor = value
                Me.BSRangeValueLabel.BackColor = value
                Me.BSRangeValuesTitle.BackColor = value

                Me.BSLastValuePanel.BackColor = value
                Me.BSLastValueLabel.BackColor = value
                Me.BSLastValueTitle.BackColor = value

                InfoBackColorAttr = value
            End Set
        End Property

        ''' <summary>
        ''' Control's info area's titles' color
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>Created by SG 17/12/10</remarks>
        Public Property InfoTitlesForeColor() As Color
            Get
                Return InfoTitlesForeColorAttr
            End Get
            Set(ByVal value As Color)
                Me.BSLastValueTitle.ForeColor = value
                Me.BSRangeValuesTitle.ForeColor = value
                InfoTitlesForeColorAttr = value
            End Set
        End Property

        ''' <summary>
        ''' Control's info area's values' color
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>Created by SG 17/12/10</remarks>
        Public Property InfoValuesForeColor() As Color
            Get
                Return InfoValuesForeColorAttr
            End Get
            Set(ByVal value As Color)
                Me.BSLastValueLabel.ForeColor = value
                Me.BSRangeValueLabel.ForeColor = value
                InfoValuesForeColorAttr = value
            End Set
        End Property

        ''' <summary>
        ''' Maximum number of decimals allowed
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>Created by SG 17/12/10</remarks>
        Public Property MaxNumDecimals() As Integer
            Get
                Return MaxNumDecimalsAttr
            End Get
            Set(ByVal value As Integer)
                If value >= 0 Then
                    MaxNumDecimalsAttr = value
                    If Me.BSDisplayTextBox.Text.Length > 0 Then
                        Me.BSDisplayTextBox.Text = FormatDisplayValue(CDbl(Me.BSDisplayTextBox.Text.Replace(".", ",")))
                    End If
                End If
            End Set
        End Property

        ''' <summary>
        ''' Text for the Last Value Saved label
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>Created by SG 17/12/10</remarks>
        Public Property LastValueSavedTitle() As String
            Get
                Return LastValueSavedTitleAttr
            End Get
            Set(ByVal value As String)
                Me.BSLastValueTitle.Text = value
                LastValueSavedTitleAttr = value
            End Set
        End Property

        ''' <summary>
        ''' Text for the Range Limits label
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>Created by SG 17/12/10</remarks>
        Public Property RangeTitle() As String
            Get
                Return RangeTitleAttr
            End Get
            Set(ByVal value As String)
                Me.BSRangeValuesTitle.Text = value
                RangeTitleAttr = value
            End Set
        End Property

       
#End Region

#Region "Declarations"
        Private IsLoading As Boolean = True 'the control is being loaded
        Private CurrentActionRequested As AdjustActions = AdjustActions.NoAction 'currently requested action
        Private LastActionRequested As AdjustActions = AdjustActions.NoAction 'last requested action
        Private LastSetPointValue As Single 'last successful setpoint

#End Region

#Region "Attributes"
        Private LastValueSavedAttr As Single = 0
        Private LastSetPointValueAttr As Single = 0
        Private CurrentValueAttr As Single = 0
        Private SetPointValueAttr As Single = 0
        Private ButtonsAreaEnabledAttr As Boolean = True
        Private EditionModeAttr As Boolean = False
        Private ManageKeyEventsAttr As Boolean = False

        Private MaximumLimitAttr As Single = 100000
        Private MinimumLimitAttr As Single = 0

        Private DisplayFontAttr As Font

        Private HomingEnabledAttr As Boolean = True
        Private AdjustingEnabledAttr As Boolean = True
        Private EditingEnabledAttr As Boolean = True

        Private UnFocusedBackColorAttr As Color = Color.Gainsboro
        Private FocusedBackColorAttr As Color = Color.Blue
        Private DisplayBackColorAttr As Color = Color.Black
        Private DisplayForeColorAttr As Color = Color.LightGreen
        Private DisplayEditingForeColorAttr As Color = Color.White
        Private InfoBackColorAttr As Color = Color.LightSteelBlue
        Private InfoTitlesForeColorAttr As Color = Color.Black
        Private InfoValuesForeColorAttr As Color = Color.Black

        Private LastValueSavedTitleAttr As String = "Last:"
        Private RangeTitleAttr As String = "Range:"
        
        Private MaxNumDecimalsAttr As Integer = 3

        Private AdjustButtonModeAttr As AdjustButtonModes = AdjustButtonModes.LeftRight

        Private UnitsCaptionAttr As String = "units"

        Private CurrentStepValueAttr As Integer = 1
        Private StepValuesAttr As New List(Of Integer)

        Private IsFocusedAttr As Boolean = False
        Private IsFocusingAttr As Boolean = False

        Private SimulationModeAttr As Boolean = False

        Private IncreaseModeAttr As IncreaseModes = IncreaseModes.Direct

#End Region

#Region "Enumerates"
        Public Enum AdjustButtonModes
            UpDown
            LeftRight
        End Enum

        Private Enum AdjustActions
            NoAction
            Home
            Enter
            Increase
            Decrease
            Escape
            Force
        End Enum

        Private Enum ButtonImages
            IMG_HOME
            IMG_LEFT
            IMG_RIGHT
            IMG_UP
            IMG_DOWN
            IMG_ENTER
            IMG_HOME_DIS
            IMG_LEFT_DIS
            IMG_RIGHT_DIS
            IMG_UP_DIS
            IMG_DOWN_DIS
            IMG_ENTER_DIS
        End Enum

        Public Enum IncreaseModes
            Direct
            Inverse
        End Enum
#End Region

#Region "Public Events"

        ''' <summary>
        ''' New setpoint has been defined and confirmed so the parent form manages to send the corresponding firmware scripts
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="Value">setpoint value</param>
        ''' <remarks>Created by SG 17/12/10</remarks>
        Public Event AbsoluteSetPointReleased(ByVal sender As Object, ByVal Value As Single)

        ''' <summary>
        ''' New setpoint has been defined and confirmed so the parent form manages to send the corresponding firmware scripts
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="Value">setpoint value</param>
        ''' <remarks>Created by SG 17/12/10</remarks>
        Public Event RelativeSetPointReleased(ByVal sender As Object, ByVal Value As Single)

        ''' <summary>
        ''' New homing has been requested so the parent form manages to send the corresponding firmware scripts
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <remarks>Created by SG 17/12/10</remarks>
        Public Event HomeRequestReleased(ByVal sender As Object)


        ''' <summary>
        ''' The entered value is out of range
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <remarks>Created by SG 17/12/10</remarks>
        Public Event SetPointOutOfRange(ByVal sender As Object)


        ''' <summary>
        ''' The entered value has not correct syntax
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="Value">curent text value</param>
        ''' <remarks>Created by SG 17/12/10</remarks>
        Public Event ValidationError(ByVal sender As Object, ByVal Value As String)


        ''' <summary>
        ''' The edition mode has changed. That way the form can address the handling of the key events. 
        ''' If edition mode is activated the key events are catched by the control. Otherwise the form handles them.
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <remarks>Created by SG 17/12/10</remarks>
        Public Event EditionModeChanged(ByVal sender As Object, ByVal editionmode As Boolean)

        ''' <summary>
        ''' Focus received
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <remarks>Created by SG 03/01/11</remarks>
        Public Event FocusReceived(ByVal sender As Object)

        ''' <summary>
        ''' Focus lost
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <remarks>Created by SG 03/01/11</remarks>
        Public Event FocusLost(ByVal sender As Object)

        'Tab request
        Public Event TabRequest(ByVal sender As Object)

        'Back Tab request
        Public Event BackTabRequest(ByVal sender As Object)

        Public Event CurrentValueChanged(ByVal sender As Object, ByVal value As Single)

        ' XBC 10/10/2011
        Public Event GotoNextAdjustControl(ByVal myControl As String)

#End Region

#Region "Public Methods"

        ''' <summary>
        ''' Increases the setpoint one step. Disabled when editing the textbox.
        ''' </summary>
        ''' <remarks>Created by SG 17/12/10</remarks>
        Public Sub IncreaseRequest()
            Try
                If Me.Enabled And CurrentActionRequested = AdjustActions.NoAction Then
                    IncreaseStep()
                End If
            Catch ex As Exception
                Throw ex
            End Try
        End Sub

        ''' <summary>
        ''' Decreases the setpoint one step. Disabled when editing the textbox.
        ''' </summary>
        ''' <remarks>Created by SG 17/12/10</remarks>
        Public Sub DecreaseRequest()
            Try
                If Me.Enabled And CurrentActionRequested = AdjustActions.NoAction Then
                    DecreaseStep()
                End If
            Catch ex As Exception
                Throw ex
            End Try
        End Sub


        ''' <summary>
        ''' Updates the setpoint to the value written in the display textbox
        ''' </summary>
        ''' <remarks>Created by SG 17/12/10</remarks>
        Public Sub EnterRequest()
            Try
                If Me.Enabled And CurrentActionRequested = AdjustActions.NoAction Then
                    EnterValue()
                End If
            Catch ex As Exception
                Throw ex
            End Try
        End Sub

        ''' <summary>
        ''' Updates the setpoint to the value informed. Disabled when editing the textbox.
        ''' </summary>
        ''' <param name="pValue"></param>
        ''' <remarks>Created by SG 17/12/10</remarks>
        Public Sub ForceRequest(ByVal pValue As Single)
            Try
                If Me.Enabled And CurrentActionRequested = AdjustActions.NoAction Then
                    ForceValue(pValue)
                End If
            Catch ex As Exception
                Throw ex
            End Try
        End Sub

        ''' <summary>
        ''' Cancel the edition in the display textbox
        ''' </summary>
        ''' <remarks>Created by SG 17/12/10</remarks>
        Public Sub EscapeRequest()
            Try

                If Me.Enabled And CurrentActionRequested = AdjustActions.NoAction Or CurrentActionRequested = AdjustActions.Enter Then
                    Escape()
                End If

            Catch ex As Exception
                Throw ex
            End Try
        End Sub

        ''' <summary>
        ''' Releases a homing request. Disabled when editing the textbox.
        ''' </summary>
        ''' <remarks>Created by SG 17/12/10</remarks>
        Public Sub HomeRequest()
            Try

                If Me.Enabled And CurrentActionRequested = AdjustActions.NoAction Then
                    Home()

                    ' XBC 26/10/2011
                    BSDisplayTextBox.SelectionLength = 0
                    BSHomeButton.Focus()
                End If

            Catch ex As Exception
                Throw ex
            End Try
        End Sub

        ''' <summary>
        ''' Necessary operations when the control gets focus
        ''' </summary>
        ''' <remarks>Created by SG 17/12/10</remarks>
        Public Sub GetFocus()
            Try

                If Me.Enabled And CurrentActionRequested = AdjustActions.NoAction Then
                    If Not Me.IsFocusing Then
                        MyClass.IsFocusing = True
                        Me.BackColor = Me.FocusedBackColor
                        If EditionMode Then
                            Me.BSDisplayTextBox.Focus()
                            ShowCaret(Me.BSDisplayTextBox.Handle)
                            BSDisplayTextBox.Focus()
                        Else
                            HideCaret(Me.BSDisplayTextBox.Handle)
                            Me.Focus()
                        End If

                        ' XBC 26/10/2011
                        'BSDisplayTextBox.SelectionLength = 0

                        MyClass.IsFocused = True
                        MyClass.IsFocusing = False
                        RaiseEvent FocusReceived(Me)
                    End If
                End If
            Catch ex As Exception
                Throw ex
            End Try
        End Sub

        Public Sub ClearText()
            Try
                If Not Me.Enabled And CurrentActionRequested = AdjustActions.NoAction Then
                    BSDisplayTextBox.Text = ""
                End If
            Catch ex As Exception
                Throw ex
            End Try
        End Sub

        Public Sub ResetStepValue()
            Try
                If MyClass.StepValues.Count > 0 Then
                    MyClass.CurrentStepValue = MyClass.StepValues(0)
                    Me.BSStepButton.Text = MyClass.CurrentStepValue.ToString
                End If
            Catch ex As Exception
                Throw ex
            End Try
        End Sub
#End Region


#Region "Private Methods"





        ''' <summary>
        ''' Increases the setpoint one step. Disabled when editing the textbox.
        ''' </summary>
        ''' <remarks>
        ''' Created by SG 17/12/10
        ''' Modified by XBC 28/01/2013 - change CSng function by Utilities.FormatToSingle method (Bugs tracking #1122)
        ''' </remarks>
        Private Sub IncreaseStep()
            Try
                ' XBC 12/12/2011
                'If Not EditionMode Then
                If Not EditionMode And Me.ButtonsAreaEnabled Then
                    ' XBC 12/12/2011

                    If BSDisplayTextBox.Text.Length > 0 Then

                        'Dim myTextValue As Single = CSng(BSDisplayTextBox.Text.Replace(".", ","))
                        Dim myTextValue As Single
                        Dim myUtilities As New Utilities
                        myTextValue = myUtilities.FormatToSingle(BSDisplayTextBox.Text.Replace(".", ","))

                        Select Case MyClass.IncreaseMode
                            Case IncreaseModes.Direct
                                CurrentActionRequested = AdjustActions.Increase
                                UpdateNewSetPoint(myTextValue + Me.CurrentStepValue)
                                LastActionRequested = AdjustActions.Increase

                            Case IncreaseModes.Inverse
                                CurrentActionRequested = AdjustActions.Decrease
                                UpdateNewSetPoint(myTextValue - Me.CurrentStepValue)
                                LastActionRequested = AdjustActions.Decrease

                        End Select
                        
                        'Me.GetFocus()

                        ' XBC 26/10/2011
                        BSDisplayTextBox.SelectionLength = 0
                        BSIncreaseButton.Focus()

                    End If
                End If
            Catch ex As Exception
                Throw ex
            Finally
                CurrentActionRequested = AdjustActions.NoAction
            End Try
        End Sub

        ''' <summary>
        ''' Decreases the setpoint one step. Disabled when editing the textbox.
        ''' </summary>
        ''' <remarks>
        ''' Created by SG 17/12/10
        ''' Modified by XBC 28/01/2013 - change CSng function by Utilities.FormatToSingle method (Bugs tracking #1122)
        ''' </remarks>
        Private Sub DecreaseStep()
            Try
                ' XBC 12/12/2011
                'If Not EditionMode Then
                If Not EditionMode And Me.ButtonsAreaEnabled Then
                    ' XBC 12/12/2011

                    If BSDisplayTextBox.Text.Length > 0 Then

                        'Dim myTextValue As Single = CSng(BSDisplayTextBox.Text.Replace(".", ","))
                        Dim myTextValue As Single
                        Dim myUtilities As New Utilities
                        myTextValue = myUtilities.FormatToSingle(BSDisplayTextBox.Text.Replace(".", ","))

                        Select Case MyClass.IncreaseMode
                            Case IncreaseModes.Direct
                                CurrentActionRequested = AdjustActions.Decrease
                                UpdateNewSetPoint(myTextValue - Me.CurrentStepValue)
                                LastActionRequested = AdjustActions.Decrease

                            Case IncreaseModes.Inverse
                                CurrentActionRequested = AdjustActions.Increase
                                UpdateNewSetPoint(myTextValue + Me.CurrentStepValue)
                                LastActionRequested = AdjustActions.Increase

                        End Select
                        
                        'Me.GetFocus()

                        ' XBC 26/10/2011
                        BSDisplayTextBox.SelectionLength = 0
                        BSDecreaseButton.Focus()

                    End If
                End If
            Catch ex As Exception
                Throw ex
            Finally
                CurrentActionRequested = AdjustActions.NoAction
            End Try
        End Sub

        ''' <summary>
        ''' Updates the setpoint to the value written in the display textbox
        ''' </summary>
        ''' <remarks>
        ''' Created by SG 17/12/10
        ''' Modified by XBC 28/01/2013 - change CSng function by Utilities.FormatToSingle method (Bugs tracking #1122)
        ''' </remarks>
        Private Sub EnterValue()
            Try
                If BSDisplayTextBox.Text.Length > 0 Then

                    'Dim myTextValue As Single = CSng(BSDisplayTextBox.Text.Replace(".", ","))
                    Dim myTextValue As Single
                    Dim myUtilities As New Utilities
                    myTextValue = myUtilities.FormatToSingle(BSDisplayTextBox.Text.Replace(".", ","))

                    CurrentActionRequested = AdjustActions.Enter
                    UpdateNewSetPoint(myTextValue)
                    LastActionRequested = AdjustActions.Enter
                    Me.GetFocus()

                    ' XBC 26/10/2011
                    BSDisplayTextBox.SelectionLength = 0
                    BSEnterButton.Focus()

                End If

            Catch ex As Exception
                Throw ex
            Finally
                CurrentActionRequested = AdjustActions.NoAction
            End Try
        End Sub


        ''' <summary>
        ''' Updates the setpoint to the value informed. Disabled when editing the textbox.
        ''' </summary>
        ''' <param name="pValue"></param>
        ''' <remarks>Created by SG 17/12/10</remarks>
        Private Sub ForceValue(ByVal pValue As Single)
            Try
                If Not EditionMode Then
                    BSDisplayTextBox.Text = FormatDisplayValue(pValue)
                    CurrentActionRequested = AdjustActions.Force
                    UpdateNewSetPoint(pValue)
                    LastActionRequested = AdjustActions.Force
                    Me.GetFocus()
                End If
            Catch ex As Exception
                Throw ex
            Finally
                CurrentActionRequested = AdjustActions.NoAction
            End Try
        End Sub

        ''' <summary>
        ''' Releases a homing request. Disabled when editing the textbox.
        ''' </summary>
        ''' <remarks>Created by SG 17/12/10</remarks>
        Private Sub Home()
            Try
                If Not EditionMode Then
                    CurrentActionRequested = AdjustActions.Home

                    ' XBC 09/01/2012
                    UpdateNewSetPoint(0)
                    ' XBC 09/01/2012

                    RaiseEvent HomeRequestReleased(Me)
                    LastActionRequested = AdjustActions.Home
                    Me.GetFocus()
                End If
            Catch ex As Exception
                Throw ex
            Finally
                CurrentActionRequested = AdjustActions.NoAction
            End Try
        End Sub

        ''' <summary>
        ''' Cancel the edition in the display textbox
        ''' </summary>
        ''' <remarks>Created by SG 17/12/10</remarks>
        Private Sub Escape()
            Try
                If Me.EditionMode Then
                    CurrentActionRequested = AdjustActions.Escape
                    UpdateNewSetPoint(Me.LastSetPointValue)
                    Me.EditionMode = False
                    LastActionRequested = AdjustActions.Escape
                    CurrentActionRequested = AdjustActions.NoAction
                End If
            Catch ex As Exception
                Throw ex
            Finally
                CurrentActionRequested = AdjustActions.NoAction
            End Try
        End Sub

        ''' <summary>
        ''' Recovers the focus to the control after performing the requested action
        ''' </summary>
        ''' <remarks>Created by SG 17/12/10</remarks>
        Private Sub RecoverButtonFocus()
            Try
                Select Case LastActionRequested
                    Case AdjustActions.Increase
                        Me.BSIncreaseButton.Focus()

                    Case AdjustActions.Decrease
                        Me.BSDecreaseButton.Focus()

                    Case AdjustActions.Home
                        Me.BSHomeButton.Focus()

                    Case AdjustActions.Enter
                        Me.BSEnterButton.Focus()

                    Case AdjustActions.Escape
                        Me.Focus()

                End Select
            Catch ex As Exception
                Throw ex
            Finally
                CurrentActionRequested = AdjustActions.NoAction
                LastActionRequested = AdjustActions.NoAction
            End Try
        End Sub



        ''' <summary>
        ''' validates the entry against the predefined limits
        ''' </summary>
        ''' <param name="pValue"></param>
        ''' <returns></returns>
        ''' <remarks>Created by SG 09/12/10</remarks>
        Private Function ValidateLimits(ByVal pValue As Single, ByRef isOver As Boolean) As Single
            Try
                Dim myValue As Single = pValue

                If MyClass.IncreaseMode = IncreaseModes.Direct Then
                    Me.BSIncreaseButton.Enabled = (myValue < Me.MaximumLimit)
                    Me.BSDecreaseButton.Enabled = (myValue > Me.MinimumLimit)
                ElseIf MyClass.IncreaseMode = IncreaseModes.Inverse Then
                    Me.BSDecreaseButton.Enabled = (myValue < Me.MaximumLimit)
                    Me.BSIncreaseButton.Enabled = (myValue > Me.MinimumLimit)
                End If

                If myValue <= Me.MaximumLimit And myValue >= Me.MinimumLimit Then
                    isOver = False
                    Return myValue
                Else
                    isOver = True
                    Return Nothing
                End If

            Catch ex As Exception
                Throw ex
            End Try
        End Function


        ''' <summary>
        ''' Formats the new entry text according to the numeric format specifications
        ''' </summary>
        ''' <param name="pValue"></param>
        ''' <returns></returns>
        ''' <remarks>Created by SG 09/12/10</remarks>
        Private Function FormatDisplayValue(ByVal pValue As Double) As String
            Try
                Dim myFormat As String = "##0."
                For c As Integer = 1 To Me.MaxNumDecimalsAttr
                    myFormat = myFormat & "0"
                Next
                Dim myText As String = Format(pValue, myFormat).Replace(",", ".")
                If myText.EndsWith(".") Then
                    myText = myText.Substring(0, myText.Length - 1)
                End If

                Return myText

            Catch ex As Exception
                Throw ex
                RaiseEvent ValidationError(Me, Me.BSDisplayTextBox.Text)
            End Try
        End Function



        ''' <summary>
        ''' performs the new Setpoint releasement
        ''' </summary>
        ''' <param name="pValue"></param>
        ''' <remarks>Created by SG 17/12/10</remarks>
        Private Sub UpdateNewSetPoint(ByVal pValue As Single)
            Try
                If Not IsLoading Then

                    Dim isOut As Boolean = False
                    Dim myNewValue As Single = ValidateLimits(pValue, isOut)

                    If Not isOut Then
                        If CurrentActionRequested <> AdjustActions.NoAction And CurrentActionRequested <> AdjustActions.Escape Then
                            If Me.SetPointValue <> myNewValue Then
                                Me.SetPointValue = myNewValue
                            Else
                                Me.EscapeRequest()
                            End If

                        End If
                        'Me.BSDisplayTextBox.Text = FormatDisplayValue(myNewValue)      ' XBC 12/12/2011
                    Else
                        RaiseEvent SetPointOutOfRange(Me)
                        'Me.CurrentValue = Me.LastSetPointValue
                        RecoverButtonFocus()
                    End If

                    'Me.EditionMode = False

                End If

            Catch ex As Exception
                Throw ex
            End Try
        End Sub

        Private Sub UpdateButtonImages(ByVal pButton As Button)
            Try

                If Me.BsButtonsImageList.Images.Count = 0 Then Exit Sub

                If pButton IsNot Nothing Then
                    Dim isEnabled As Boolean = pButton.Enabled
                    If pButton Is Me.BSHomeButton Then
                        If isEnabled Then
                            pButton.BackgroundImage = Me.BsButtonsImageList.Images(ButtonImages.IMG_HOME.ToString)
                        Else
                            pButton.BackgroundImage = Me.BsButtonsImageList.Images(ButtonImages.IMG_HOME_DIS.ToString)
                        End If
                        'pButton.BackgroundImageLayout = ImageLayout.Stretch

                    ElseIf pButton Is Me.BSIncreaseButton Then
                        Select Case MyClass.AdjustButtonMode
                            Case AdjustButtonModes.LeftRight
                                If isEnabled Then
                                    pButton.BackgroundImage = Me.BsButtonsImageList.Images(ButtonImages.IMG_RIGHT.ToString)
                                Else
                                    pButton.BackgroundImage = Me.BsButtonsImageList.Images(ButtonImages.IMG_RIGHT_DIS.ToString)
                                End If

                            Case AdjustButtonModes.UpDown
                                If isEnabled Then
                                    pButton.BackgroundImage = Me.BsButtonsImageList.Images(ButtonImages.IMG_DOWN.ToString)
                                Else
                                    pButton.BackgroundImage = Me.BsButtonsImageList.Images(ButtonImages.IMG_DOWN_DIS.ToString)
                                End If

                        End Select
                        pButton.BackgroundImageLayout = ImageLayout.Center

                    ElseIf pButton Is Me.BSDecreaseButton Then
                        Select Case MyClass.AdjustButtonMode
                            Case AdjustButtonModes.LeftRight
                                If isEnabled Then
                                    pButton.BackgroundImage = Me.BsButtonsImageList.Images(ButtonImages.IMG_LEFT.ToString)
                                Else
                                    pButton.BackgroundImage = Me.BsButtonsImageList.Images(ButtonImages.IMG_LEFT_DIS.ToString)
                                End If

                            Case AdjustButtonModes.UpDown
                                If isEnabled Then
                                    pButton.BackgroundImage = Me.BsButtonsImageList.Images(ButtonImages.IMG_UP.ToString)
                                Else
                                    pButton.BackgroundImage = Me.BsButtonsImageList.Images(ButtonImages.IMG_UP_DIS.ToString)
                                End If

                        End Select
                        pButton.BackgroundImageLayout = ImageLayout.Center

                    ElseIf pButton Is Me.BSEnterButton Then
                        If isEnabled Then
                            pButton.BackgroundImage = Me.BsButtonsImageList.Images(ButtonImages.IMG_ENTER.ToString)
                        Else
                            pButton.BackgroundImage = Me.BsButtonsImageList.Images(ButtonImages.IMG_ENTER_DIS.ToString)
                        End If
                        pButton.BackgroundImageLayout = ImageLayout.Center
                    End If
                End If
            Catch ex As Exception
                Throw ex
            End Try
        End Sub

#End Region



#Region "Private Event Handlers"

        Private Sub BSDisplayTextBox_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles BSDisplayTextBox.KeyDown
            Try
                If EditionMode Then
                    Select Case e.KeyCode
                        Case Keys.Escape
                            If CurrentActionRequested = AdjustActions.NoAction Then
                                Me.EscapeRequest()
                            End If

                        Case Keys.Enter
                            If CurrentActionRequested = AdjustActions.NoAction Then
                                Me.EnterRequest()
                            End If

                        Case Else
                            e.Handled = False
                    End Select
                End If
            Catch ex As Exception
                Throw ex
            End Try
        End Sub

        ''THE TAB CANNOT BE CATCHED BY KEYDOWN EVENTS
        'Protected Overrides Function ProcessCmdKey(ByRef msg As Message, ByVal keyData As Keys) As Boolean
        '    Select Case keyData
        '        Case Keys.Shift And Keys.Tab
        '            BackTabPressed = True
        '            Return False

        '        Case Else
        '            Return True
        '    End Select

        'End Function

        Private Sub BSDisplayTextBox_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles BSDisplayTextBox.KeyUp
            Try
                Select Case e.KeyCode
                    Case Keys.Escape
                        Escape()
                        e.Handled = True

                    Case Keys.Enter
                        ' XBC 09/01/2012
                        'EnterValue()
                        ' XBC 09/01/2012

                        e.Handled = True

                    Case Keys.Tab
                        'If EditionMode Then
                        '    If EditionMode Then
                        '        Me.Escape()
                        '        RaiseEvent TabRequestWhileEditing(Me)
                        '    End If
                        'End If

                    Case Keys.Shift
                        'If EditionMode Then
                        '    If EditionMode Then
                        '        Me.Escape()
                        '        RaiseEvent BackTabRequestWhileEditing(Me)
                        '    End If
                        'End If

                End Select


            Catch ex As Exception
                Throw ex
            End Try
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        ''' Modified by XBC 28/01/2013 - change CSng function by Utilities.FormatToSingle method (Bugs tracking #1122)
        ''' Modified by XBC 28/01/2013 - disallow decimal inputs in case be configured without decimals (Bugs tracking #1122)
        ''' </remarks>
        Private Sub BSDisplayTextBox_KeyPress(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles BSDisplayTextBox.KeyPress
            Try

                If EditionMode Then

                    If CurrentActionRequested = AdjustActions.NoAction Then

                        Dim myAscii As Integer = Asc(e.KeyChar)

                        If Me.BSDisplayTextBox.Text.Length >= 6 Then
                            If myAscii <> 8 Then
                                e.Handled = True
                            End If
                        Else

                            Select Case myAscii
                                Case 8

                                Case 13
                                    If Me.BSDisplayTextBox.Text.Length > 0 Then
                                        Dim isOver As Boolean = False

                                        'Dim myTextValue As Single = CSng(BSDisplayTextBox.Text.Replace(".", ","))
                                        Dim myTextValue As Single
                                        Dim myUtilities As New Utilities
                                        myTextValue = myUtilities.FormatToSingle(BSDisplayTextBox.Text)

                                        Dim myValue As Single = ValidateLimits(myTextValue, isOver)
                                        If Not isOver Then
                                            EnterValue()
                                        End If
                                    Else
                                        e.Handled = True
                                    End If


                                Case Else
                                    If Not IsNumeric(e.KeyChar) Then
                                        ' XBC 28/01/2013
                                        'If (e.KeyChar = "." Or e.KeyChar = ",") And BSDisplayTextBox.Text.Contains(".") Then
                                        If MaxNumDecimalsAttr > 0 Then
                                            If (e.KeyChar = "." Or e.KeyChar = ",") And BSDisplayTextBox.Text.Contains(".") Then
                                                e.Handled = True
                                            End If
                                        Else
                                            If (e.KeyChar = "." Or e.KeyChar = ",") Then
                                                e.Handled = True
                                            End If
                                        End If
                                        ' XBC 28/01/2013
                                        If e.KeyChar = "-" And (BSDisplayTextBox.Text.Contains("-") Or BSDisplayTextBox.Text.Length > 0) Then
                                            e.Handled = True
                                        End If
                                        If e.KeyChar = "+" And (BSDisplayTextBox.Text.Contains("+") Or BSDisplayTextBox.Text.Length > 0) Then
                                            e.Handled = True
                                        End If
                                        If e.KeyChar <> "." And e.KeyChar <> "," And e.KeyChar <> "+" And e.KeyChar <> "-" Then
                                            e.Handled = True
                                        End If
                                    End If


                            End Select


                        End If


                    Else
                        e.Handled = True
                    End If

                    'SGM 27/09/2012 not to show message in case of not alphanumeric entered
                    'If e.Handled Then
                    '    RaiseEvent ValidationError(Me, Me.BSDisplayTextBox.Text)
                    'End If

                Else
                    e.Handled = True
                End If
            Catch ex As Exception
                Throw ex
                RaiseEvent ValidationError(Me, Me.BSDisplayTextBox.Text)
            End Try
        End Sub

        Private Sub BSAdjustControl_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Try
                IsLoading = False
            Catch ex As Exception
                Throw ex
            End Try

        End Sub

        Private Sub BSIncreaseButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BSIncreaseButton.Click
            Try
                If CurrentActionRequested = AdjustActions.NoAction Then
                    IncreaseStep()
                End If
            Catch ex As Exception
                Throw ex
            End Try
        End Sub

        Private Sub BSDecreaseButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BSDecreaseButton.Click
            Try
                If CurrentActionRequested = AdjustActions.NoAction Then
                    DecreaseStep()
                End If
            Catch ex As Exception
                Throw ex
            End Try
        End Sub

        Private Sub BSHomeButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BSHomeButton.Click
            Try
                If CurrentActionRequested = AdjustActions.NoAction Then
                    HomeRequest()
                End If
            Catch ex As Exception
                Throw ex
            End Try
        End Sub

        Private Sub BSEnterButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BSEnterButton.Click
            Try
                If CurrentActionRequested = AdjustActions.NoAction Then
                    EnterValue()
                End If
            Catch ex As Exception
                Throw ex
            End Try
        End Sub

        Private Sub BSAdjustControl_Leave(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Leave
            Try
                Me.EditionMode = False
                Me.BackColor = Me.UnFocusedBackColor
                Me.CurrentValue = LastSetPointValue
                Me.IsFocused = False

                RaiseEvent FocusLost(sender)

            Catch ex As Exception
                Throw ex
            End Try
        End Sub

        Private Sub Control_MouseUp(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.MouseUp, _
                                        BSAdjustButtonsPanel.MouseUp, BSInfoPanel.MouseUp, _
                                        BSUnitsLabel.MouseUp, BSDisplayTextBox.MouseUp, BSLastValueLabel.MouseUp, _
                                        BSLastValueTitle.MouseUp, BSRangeValueLabel.MouseUp, BSRangeValuesTitle.MouseUp, _
                                        BSHomeButton.MouseUp, BSIncreaseButton.MouseUp, BSDecreaseButton.MouseUp, _
                                         BSEnterButton.MouseUp, BSDisplayValuesPanel.MouseUp, _
                                         BsRangePanel.MouseUp, BSLastValuePanel.MouseUp

            Try
                If Not Me.IsFocused Then
                    Me.GetFocus()
                End If
            Catch ex As Exception
                Throw ex
            End Try
        End Sub

        Private Sub Control_GotFocus(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.GotFocus, _
                                        BSAdjustButtonsPanel.GotFocus, BSInfoPanel.GotFocus, BSDisplayPanel.GotFocus, _
                                        BSUnitsLabel.GotFocus, BSDisplayTextBox.GotFocus, BSLastValueLabel.GotFocus, _
                                        BSLastValueTitle.GotFocus, BSRangeValueLabel.GotFocus, BSRangeValuesTitle.GotFocus, _
                                        BSHomeButton.GotFocus, BSIncreaseButton.GotFocus, BSDecreaseButton.GotFocus, _
                                         BSEnterButton.GotFocus, BSDisplayValuesPanel.GotFocus, _
                                         BsRangePanel.GotFocus, BSLastValuePanel.GotFocus

            Try
                If Not Me.IsFocused Then
                    Me.GetFocus()
                End If
            Catch ex As Exception
                Throw ex
            End Try
        End Sub


        Private Sub BSDisplayTextBox_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BSDisplayTextBox.TextChanged
            Try
                BSDisplayTextBox.Text = BSDisplayTextBox.Text.Replace(",", ".")
                BSDisplayTextBox.Refresh()

            Catch ex As Exception
                Throw ex
            End Try
        End Sub

        Private Sub BSDisplayTextBox_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BSDisplayTextBox.Click
            Try
                If Me.EditingEnabled Then
                    If CurrentActionRequested = AdjustActions.NoAction Then
                        EditionMode = True
                        ShowCaret(BSDisplayTextBox.Handle)
                    End If

                    ' XBC 26/10/2011
                    'BSDisplayTextBox.SelectionLength = 0

                Else
                    HideCaret(BSDisplayTextBox.Handle)
                End If

            Catch ex As Exception
                Throw ex
            End Try
        End Sub

        Private Sub BSAdjustControl_EnabledChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.EnabledChanged
            Try
                Me.EditionMode = False

                'SGM 23/11/2011
                If MyBase.Enabled Then
                    MyClass.ResetStepValue()
                End If
                'SGM 23/11/2011

            Catch ex As Exception
                Throw ex
            End Try
        End Sub

        ' XBC 05/10/2011
        'Private Sub BsStepLabel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsStepLabel.Click
        Private Sub BsStepLabel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BSStepButton.Click
            Try
                If MyClass.StepValues.Count > 0 Then
                    Dim index As Integer = MyClass.StepValues.IndexOf(MyClass.CurrentStepValue)
                    If index < MyClass.StepValues.Count - 1 Then
                        MyClass.CurrentStepValue = MyClass.StepValues(index + 1)
                    Else
                        MyClass.CurrentStepValue = MyClass.StepValues(0)
                    End If
                End If
                ' XBC 05/10/2011
                'BsStepLabel.Text = "x" & MyClass.CurrentStepValue.ToString
                BSStepButton.Text = MyClass.CurrentStepValue.ToString ' "x" & MyClass.CurrentStepValue.ToString

                ' XBC 26/10/2011
                BSDisplayTextBox.SelectionLength = 0
            Catch ex As Exception
                Throw ex
            End Try
        End Sub

        '' XBC 05/10/2011
        ''Private Sub BsStepLabel_MouseEnter(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsStepLabel.MouseEnter
        'Private Sub BsStepLabel_MouseEnter(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BSStepButton.MouseEnter
        '    Try
        '        ' XBC 05/10/2011
        '        'BsStepLabel.ForeColor = Color.White
        '        BSStepButton.ForeColor = Color.White
        '        Me.Cursor = Cursors.Hand
        '    Catch ex As Exception
        '        Throw ex
        '    End Try
        'End Sub

        '' XBC 05/10/2011
        ''Private Sub BsStepLabel_MouseLeave(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsStepLabel.MouseLeave
        'Private Sub BsStepLabel_MouseLeave(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BSStepButton.MouseLeave
        '    Try
        '        ' XBC 05/10/2011
        '        'BsStepLabel.ForeColor = Color.LightGray
        '        BSStepButton.ForeColor = Color.LightGray
        '        Me.Cursor = Cursors.Default
        '    Catch ex As Exception
        '        Throw ex
        '    End Try
        'End Sub

        Private Sub BsButtons_MouseEnter(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BSHomeButton.MouseEnter, _
                                                                                                               BSDecreaseButton.MouseEnter, _
                                                                                                               BSIncreaseButton.MouseEnter, _
                                                                                                               BSEnterButton.MouseEnter, _
                                                                                                               BSStepButton.MouseEnter
            Try
                Me.Cursor = Cursors.Hand
            Catch ex As Exception
                Throw ex
            End Try
        End Sub

        Private Sub BsButtons_MouseLeave(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BSHomeButton.MouseLeave, _
                                                                                                                BSDecreaseButton.MouseLeave, _
                                                                                                                BSIncreaseButton.MouseLeave, _
                                                                                                                BSEnterButton.MouseLeave
            Try
                Me.Cursor = Cursors.Default
            Catch ex As Exception
                Throw ex
            End Try
        End Sub

        'buttons images change with enable/disable
        Private Sub BSButton_EnabledChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BSHomeButton.EnabledChanged, _
                                                                                                                BSIncreaseButton.EnabledChanged, _
                                                                                                                BSDecreaseButton.EnabledChanged, _
                                                                                                                BSEnterButton.EnabledChanged
            Try
                Dim myButton As Button = CType(sender, Button)
                UpdateButtonImages(myButton)
            Catch ex As Exception
                Throw ex
            End Try
        End Sub

        Private Sub BSAdjustControl_VisibleChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.VisibleChanged
            Try
                MyClass.ResetStepValue()
            Catch ex As Exception
                Throw ex
            End Try
        End Sub
#End Region

#Region "Win32"
        <DllImport("user32.dll")> _
        Private Shared Function HideCaret(ByVal hWnd As IntPtr) As Boolean
        End Function

        <DllImport("user32.dll")> _
        Private Shared Function ShowCaret(ByVal hWnd As IntPtr) As Boolean
        End Function

#End Region

#Region "Key Events Catching"

        Protected Overrides Function ProcessCmdKey(ByRef msg As Message, ByVal keyData As Keys) As Boolean
            Dim handled As Boolean = False

            Try
                If CurrentActionRequested = AdjustActions.NoAction Then
                    If Not MyClass.EditionMode And MyClass.ButtonsAreaEnabled Then

                        If keyData = 65545 Then 'shift and tab
                            RaiseEvent BackTabRequest(Me)
                            handled = True
                        End If

                        Select Case keyData
                            Case Keys.Back

                            Case Keys.Escape
                                EscapeRequest()
                                handled = True

                            Case Keys.Left
                                If MyClass.AdjustButtonMode = AdjustButtonModes.LeftRight Then
                                    DecreaseStep()
                                    handled = True
                                ElseIf MyClass.AdjustButtonMode = AdjustButtonModes.UpDown Then
                                    RaiseEvent GotoNextAdjustControl(Me.Name)   ' XBC 10/10/2011
                                    handled = True
                                End If


                            Case Keys.Right
                                If MyClass.AdjustButtonMode = AdjustButtonModes.LeftRight Then
                                    IncreaseStep()
                                    handled = True
                                ElseIf MyClass.AdjustButtonMode = AdjustButtonModes.UpDown Then
                                    RaiseEvent GotoNextAdjustControl(Me.Name)   ' XBC 10/10/2011
                                    handled = True
                                End If

                            Case Keys.Down
                                If MyClass.AdjustButtonMode = AdjustButtonModes.UpDown Then
                                    IncreaseStep()
                                    handled = True
                                ElseIf MyClass.AdjustButtonMode = AdjustButtonModes.LeftRight Then
                                    RaiseEvent GotoNextAdjustControl(Me.Name)   ' XBC 10/10/2011
                                    handled = True
                                End If

                            Case Keys.Up
                                If MyClass.AdjustButtonMode = AdjustButtonModes.UpDown Then
                                    DecreaseStep()
                                    handled = True
                                ElseIf MyClass.AdjustButtonMode = AdjustButtonModes.LeftRight Then
                                    RaiseEvent GotoNextAdjustControl(Me.Name)   ' XBC 10/10/2011
                                    handled = True
                                End If

                            Case Keys.Enter
                                EnterValue()
                                handled = True

                            Case Keys.Tab
                                RaiseEvent TabRequest(Me)
                                handled = True

                                ' XBC 10/01/2012
                            Case Keys.F9
                                If MyClass.BSHomeButton.Enabled Then
                                    MyClass.BSHomeButton.PerformClick()
                                End If

                                ' XBC 10/01/2012
                            Case Keys.F10
                                MyClass.BSStepButton.PerformClick()

                        End Select


                    Else

                        Select Case keyData
                            Case Keys.Enter
                                ' XBC 09/01/2012
                                'EnterValue()
                                MyClass.BSEnterButton.PerformClick()
                                ' XBC 09/01/2012

                                handled = True

                            Case Keys.Tab

                            Case Keys.Escape

                        End Select
                    End If
                End If

            Catch ex As Exception
                Throw ex
            End Try

            Return handled

        End Function

#End Region




       
    End Class

End Namespace
