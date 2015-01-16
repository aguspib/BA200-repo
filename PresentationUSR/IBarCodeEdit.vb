Option Strict On
Option Explicit On

Imports System

Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Global.GlobalEnumerates
Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.Types

Public Class IBarCodeEdit

#Region "Attributes"
    Dim BarCodeAttribute As String
    Dim RotorTypeAttribute As GlobalEnumerates.Rotors
    Dim TubeTypeAttribute As GlobalEnumerates.TubeTypes
    Private AnalyzerIDAttribute As String

    Private IsBarcodeSampleIdFlagAttr As Boolean = False 'SGM 12/03/2013
    Private SampleIdMaxLengthAttr As Integer = 30 'SGM 12/03/2013
    Private BarCodeLengthAttr As Integer = 0 'SGM 12/03/2013

    Private SampleTypeMaxLengthAttr As Integer = 0 'TR 09/04/2013 'variable used to indicate the sampletype character.
    Private IsSampleTypeFlagAttr As Boolean = False 'TR 09/04/2013 'Variable used to indicate if SampleType is enable.

#End Region

#Region "Properties"

    Public Property BarCode() As String
        Get
            Return BarCodeAttribute
        End Get
        Set(ByVal value As String)
            BarCodeAttribute = value
        End Set
    End Property

    Public Property RotorType() As GlobalEnumerates.Rotors
        Get
            Return RotorTypeAttribute
        End Get
        Set(ByVal value As GlobalEnumerates.Rotors)
            RotorTypeAttribute = value
        End Set
    End Property

    Public Property TubeType() As GlobalEnumerates.TubeTypes
        Get
            Return TubeTypeAttribute
        End Get
        Set(ByVal value As GlobalEnumerates.TubeTypes)
            TubeTypeAttribute = value
        End Set
    End Property

    Public Property AnalyzerID() As String
        Get
            Return AnalyzerIDAttribute
        End Get
        Set(ByVal value As String)
            AnalyzerIDAttribute = value
        End Set
    End Property

    'SGM 12/03/2013
    Private ReadOnly Property IsBarcodeSampleIdFlag As Boolean
        Get
            Return IsBarcodeSampleIdFlagAttr
        End Get
    End Property

    'SGM 12/03/2013
    Private ReadOnly Property SampleIdMaxLength As Integer
        Get
            Return SampleIdMaxLengthAttr
        End Get
    End Property

    'SGM 12/03/2013
    Private ReadOnly Property BarCodeLength As Integer
        Get
            Return BarCodeLengthAttr
        End Get
    End Property

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks>CREATED BY: TR 09/04/2013</remarks>
    Public Property SampleTypeMaxLength As Integer
        Get
            Return SampleTypeMaxLengthAttr
        End Get
        Set(ByVal value As Integer)
            SampleTypeMaxLengthAttr = value
        End Set
    End Property

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks>Create by: TR 09/04/2013</remarks>
    Private ReadOnly Property IsSampleTypeFlag As Boolean
        Get
            Return IsSampleTypeFlagAttr
        End Get
    End Property

#End Region

#Region "Declarations"
    Private LanguageID As String

#End Region

#Region "Methods"
    ''' <summary>
    ''' Search Icons for screen buttons
    ''' </summary>
    ''' <remarks>
    ''' Created by:  RH 07/09/2011
    ''' </remarks>
    Private Sub PrepareButtons()
        Try
            Dim auxIconName As String = ""
            Dim iconPath As String = MyBase.IconsPath

            'ACCEPT Button
            auxIconName = GetIconName("ACCEPT1")
            If Not String.IsNullOrEmpty(auxIconName) Then
                bsAcceptButton.Image = Image.FromFile(iconPath & auxIconName)
            End If

            'CANCEL Button
            auxIconName = GetIconName("CANCEL")
            If Not String.IsNullOrEmpty(auxIconName) Then
                bsExitButton.Image = Image.FromFile(iconPath & auxIconName)
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " PrepareButtons ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & " PrepareButtons ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Screen loading and initialization
    ''' </summary>
    ''' <remarks>
    ''' Modified by: RH 07/09/2011
    '''              SA 17/10/2011 - Apply a numeric mask for Reagents and a Not ASCII characters mask for Samples 
    '''                              Removed control error when TubeType and/or Rotor Type are not informed
    ''' </remarks>
    Private Sub InitializeScreen()
        Try
            Dim mySize As Size = IAx00MainMDI.Size
            Dim myLocation As Point = IAx00MainMDI.Location

            If (Not Me.MdiParent Is Nothing) Then
                mySize = Me.Parent.Size
                myLocation = Me.Parent.Location
            End If
            Me.Location = New Point(myLocation.X + CInt((mySize.Width - Me.Width) / 2), myLocation.Y + CInt((mySize.Height - Me.Height) / 2))

            'Get the current Language from the current Application Session
            Dim currentLanguageGlobal As New GlobalBase()
            LanguageID = GlobalBase.GetSessionInfo().ApplicationLanguage

            'Get Icons for Screen Buttons
            PrepareButtons()

            'Set multilanguage texts for all screen labels and tooltips...
            GetScreenLabels()

            'Update Mask, that is, the exact number of characters the user should type in
            'Dim Mask As StringBuilder = New StringBuilder()
            Select Case RotorTypeAttribute
                Case GlobalEnumerates.Rotors.SAMPLES
                    'Not ASCII Characters are allowed for Samples
                    'SGM 12/03/2013 - the maximum length allowed depends on the limit for SampleId
                    MyClass.GetBarCodeSampleIdFlag()
                    MyClass.GetSampleIdMaxLength()
                    MyClass.GetSamplesBarCodeLength()
                    'End SGM 12/03/2013

                    'TR 09/04/2013
                    MyClass.GetSampleTypeMaxLength()
                    MyClass.GetSampleTypeFlag()
                    'TR 09/04/2013 -END

                    'Mask.Append("C"c, BarCodeLength)

                Case GlobalEnumerates.Rotors.REAGENTS
                    'Only Numeric Characters are allowed for Reagents
                    MyClass.GetReagentsBarCodeLength()
                    'Mask.Append("0"c, BarCodeLength)
                    bsElementTextBox.IsNumeric = True
            End Select

            'Me.bsElementTextBox.Mask = Mask.ToString()
            'TR 02/05/2013 -Set the lengh to avoid user enter values greater than the allowed.
            Me.bsElementTextBox.MaxLength = BarCodeLength
            Me.bsElementTextBox.Text = BarCodeAttribute

            'TR 09/04/213 -compleate the label on screen to show the limits to the user
            GetUserBarcodeLimits()

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "InitializeScreen " & Me.Name, EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & "InitializeScreen", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Returns the configuration flag for fixed Sample Id Barcode
    ''' </summary>
    ''' <remarks>
    ''' Created by: SGM 12/036/2013
    ''' </remarks>
    Private Sub GetBarCodeSampleIdFlag()
        Try
            Dim resultData As GlobalDataTO = Nothing
            Dim myUserSettingsDelegate As New UserSettingsDelegate()
            Dim BarCodeSampleIdFlagSetting As UserSettingsEnum

            BarCodeSampleIdFlagSetting = UserSettingsEnum.BARCODE_SAMPLEID_FLAG
            resultData = myUserSettingsDelegate.GetCurrentValueBySettingID(Nothing, BarCodeSampleIdFlagSetting.ToString())

            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                MyClass.IsBarcodeSampleIdFlagAttr = (CInt(resultData.SetDatos) > 0)
            Else
                ShowMessage(Name & ".GetBarCodeSampleIdFlag", resultData.ErrorCode, resultData.ErrorMessage, Me)
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".GetBarCodeSampleIdFlag", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".GetBarCodeSampleIdFlag", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Returns the SampleID maximum allowed value
    ''' </summary>
    ''' <remarks>
    ''' Created by: SG 12/03/2013
    ''' modified by: TR Get the maximun allowed by the Setting BARCODE_EXTERNAL_END, 
    ''' inidicate the last digit for the sample id
    ''' </remarks>
    Private Sub GetSampleIdMaxLength()
        Try
            Dim resultData As GlobalDataTO = Nothing
            Dim myUserSettingsDelegate As New UserSettingsDelegate()
            Dim BarCodeSampleIdFlagSetting As UserSettingsEnum

            BarCodeSampleIdFlagSetting = UserSettingsEnum.BARCODE_EXTERNAL_END
            resultData = myUserSettingsDelegate.GetCurrentValueBySettingID(Nothing, BarCodeSampleIdFlagSetting.ToString())

            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                MyClass.SampleIdMaxLengthAttr = CInt(resultData.SetDatos)
            Else
                ShowMessage(Name & ".GetSampleIdMaxLength", resultData.ErrorCode, resultData.ErrorMessage, Me)
            End If


            'Dim myFieldLimitsDelegate As New FieldLimitsDelegate()
            'Dim myFieldLimitsDS As FieldLimitsDS
            'resultData = myFieldLimitsDelegate.GetList(Nothing, FieldLimitsEnum.SAMPLE_BARCODE_SIZE_LIMIT)
            'If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
            '    myFieldLimitsDS = TryCast(resultData.SetDatos, FieldLimitsDS)
            '    If myFieldLimitsDS.tfmwFieldLimits.Rows.Count > 0 Then
            '        SampleIdMaxLengthAttr = Convert.ToInt16(myFieldLimitsDS.tfmwFieldLimits.First.MaxValue)
            '    Else
            '        ShowMessage(Name & ".GetSamplesBarCodeLength", resultData.ErrorCode, resultData.ErrorMessage, Me)
            '    End If
            'End If


        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".GetSampleIdMaxLength", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".GetSampleIdMaxLength", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Get the Sample type last character to set the SampleTypeMaxLengthAttr.
    ''' </summary>
    ''' <remarks>CREATED BY: TR 09/04/2013</remarks>
    Private Sub GetSampleTypeMaxLength()
        Try
            Dim resultData As GlobalDataTO = Nothing
            Dim myUserSettingsDelegate As New UserSettingsDelegate()
            Dim BarCodeSampleIdFlagSetting As UserSettingsEnum

            BarCodeSampleIdFlagSetting = UserSettingsEnum.BARCODE_SAMPLETYPE_END
            resultData = myUserSettingsDelegate.GetCurrentValueBySettingID(Nothing, BarCodeSampleIdFlagSetting.ToString())

            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                MyClass.SampleTypeMaxLengthAttr = CInt(resultData.SetDatos)
            Else
                ShowMessage(Name & ".GetSampleTypeMaxLength", resultData.ErrorCode, resultData.ErrorMessage, Me)
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".GetSampleTypeMaxLength", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".GetSampleTypeMaxLength", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>CREATED BY: TR 09/04/2013</remarks>
    Private Sub GetSampleTypeFlag()
        Try
            Dim resultData As GlobalDataTO = Nothing
            Dim myUserSettingsDelegate As New UserSettingsDelegate()
            Dim BarCodeSampleIdFlagSetting As UserSettingsEnum

            BarCodeSampleIdFlagSetting = UserSettingsEnum.BARCODE_SAMPLETYPE_FLAG
            resultData = myUserSettingsDelegate.GetCurrentValueBySettingID(Nothing, BarCodeSampleIdFlagSetting.ToString())

            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                MyClass.IsSampleTypeFlagAttr = (CInt(resultData.SetDatos) > 0)
            Else
                ShowMessage(Name & ".GetSampleTypeFlag", resultData.ErrorCode, resultData.ErrorMessage, Me)
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".GetSampleTypeFlag", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".GetSampleTypeFlag", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Returns the Samples BarCode Length value
    ''' </summary>
    ''' <remarks>
    ''' Created by: RH 07/09/2011
    ''' </remarks>
    Private Sub GetSamplesBarCodeLength()
        Try
            Dim resultData As GlobalDataTO = Nothing
            Dim myUserSettingsDelegate As New UserSettingsDelegate()
            Dim BarCodeLengthUserSetting As UserSettingsEnum

            BarCodeLengthUserSetting = UserSettingsEnum.BARCODE_FULL_TOTAL
            resultData = myUserSettingsDelegate.GetCurrentValueBySettingID(Nothing, BarCodeLengthUserSetting.ToString())

            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                MyClass.BarCodeLengthAttr = CInt(resultData.SetDatos)
            Else
                'Error getting BarCode Length value
                ShowMessage(Name & ".GetSamplesBarCodeLength", resultData.ErrorCode, resultData.ErrorMessage, Me)
            End If



        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".GetSamplesBarCodeLength", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".GetSamplesBarCodeLength", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Returns the Reagents BarCode Length value
    ''' </summary>
    ''' <remarks>
    ''' Created by: RH 08/09/2011
    ''' </remarks>
    Private Sub GetReagentsBarCodeLength()
        Dim BarCodeLength As Integer = 0

        Try
            Dim resultData As GlobalDataTO = Nothing
            Dim mySwParametersDelegate As New SwParametersDelegate
            'resultData = myFieldLimitsDelegate.GetList(Nothing, GlobalEnumerates.FieldLimitsEnum.BARCODE_CHKDIGIT_LIMIT) 'Commented 13/03/2012
            'TR 13/03/2012 -Get the barcode size from the real parameter
            resultData = mySwParametersDelegate.GetParameterByAnalyzer(Nothing, AnalyzerIDAttribute, _
                                                        GlobalEnumerates.SwParameters.REAGENT_BARCODE_SIZE.ToString(), True)

            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                Dim mySwParameters As ParametersDS = DirectCast(resultData.SetDatos, ParametersDS)

                If (mySwParameters.tfmwSwParameters.Rows.Count > 0) Then
                    MyClass.BarCodeLengthAttr = CInt(mySwParameters.tfmwSwParameters(0).ValueNumeric)
                End If
            Else
                'Error getting BarCode Length value
                ShowMessage(Name & ".GetReagentsBarCodeLength", resultData.ErrorCode, resultData.ErrorMessage, Me)
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".GetReagentsBarCodeLength", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".GetReagentsBarCodeLength", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Not allow move form and mantain the center location in center parent
    ''' </summary>
    ''' <remarks>
    ''' Created by: DL 27/07/2011
    ''' </remarks>
    Protected Overrides Sub WndProc(ByRef m As Message)
        Try
            If (m.Msg = WM_WINDOWPOSCHANGING) Then
                Dim pos As WINDOWPOS = DirectCast(Runtime.InteropServices.Marshal.PtrToStructure(m.LParam, GetType(WINDOWPOS)), WINDOWPOS)

                Dim mySize As Size = IAx00MainMDI.Size
                Dim myLocation As Point = IAx00MainMDI.Location
                If (Not Me.MdiParent Is Nothing) Then
                    mySize = Me.Parent.Size
                    myLocation = Me.Parent.Location
                End If

                pos.x = myLocation.X + CInt((mySize.Width - Me.Width) / 2)
                pos.y = myLocation.Y + CInt((mySize.Height - Me.Height) / 2)
                Runtime.InteropServices.Marshal.StructureToPtr(pos, m.LParam, True)
            End If

            MyBase.WndProc(m)
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WndProc " & Me.Name, EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & "WndProc", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Gets texts in the current application language for all screen controls
    ''' </summary>
    ''' <remarks>
    ''' Created by:  RH 07/09/2011
    ''' Modified by: TR 05/10/201 - Get resource text for title label and NameSelectionLabel
    ''' </remarks>
    Private Sub GetScreenLabels()
        Try
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate()

            bsTitleLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_ENTER_BARCODE", LanguageID)
            bsMinMaxValueLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_FLEX_BARCODE", LanguageID) & ":"

            'For Tooltips...
            bsScreenToolTips.SetToolTip(bsAcceptButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Save&Close", LanguageID))
            bsScreenToolTips.SetToolTip(bsExitButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_CloseScreen", LanguageID))
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".GetScreenLabels", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".GetScreenLabels", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Set the BackColor of the Barcode Masked TextBox. Used for events Leave, Enter and TextChange 
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 17/10/2011 
    ''' </remarks>
    Private Sub ValidateBarCodeTextBox()
        Try
            If (BarCodeAttribute = String.Empty) Then
                bsElementTextBox.BackColor = Color.White
            Else
                If (bsElementTextBox.Text <> String.Empty) Then
                    bsElementTextBox.BackColor = Color.White
                Else
                    bsElementTextBox.BackColor = Color.Khaki
                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ValidateBarCodeTextBox " & Me.Name, EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ValidateBarCodeTextBox", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Complete the label bsNameSelectionLabel indicating the barcode limits
    ''' min and max value. [min-max]
    ''' </summary>
    ''' <remarks>
    ''' CREATE BY: TR 09/04/2013
    ''' Modified by: TR 29/08/2013 -change the validation now the new funcionality 
    '''                             only alow the user to indicate the Sample Type position
    '''                             show only the value of sample type max length.
    ''' </remarks>
    Private Sub GetUserBarcodeLimits()
        Try
            If MyClass.IsBarcodeSampleIdFlag Then
                If MyClass.IsSampleTypeFlag Then
                    'bsAcceptButton.Enabled = (bsElementTextBox.Text.Length >= Math.Max(MyClass.SampleIdMaxLengthAttr, SampleTypeMaxLengthAttr))
                    bsMinMaxValueLabel.Text &= String.Format(" [{0}-{1}]", SampleTypeMaxLengthAttr, BarCodeLength) 'Math.Max(MyClass.SampleIdMaxLengthAttr, SampleTypeMaxLengthAttr), BarCodeLength)
                Else
                    bsMinMaxValueLabel.Text &= String.Format(" [{0}-{1}]", 1, BarCodeLength)
                End If
            Else
                bsMinMaxValueLabel.Text &= String.Format(" [{0}-{1}]", 1, BarCodeLength)
            End If
        Catch ex As Exception

        End Try
    End Sub

#End Region

#Region "Events"
    ''' <summary>
    ''' Focuses on the TextBox on Activated event
    ''' </summary>
    Private Sub IBarCodeEdit_Activated(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Activated
        Me.bsElementTextBox.Focus()
    End Sub

    ''' <summary>
    ''' Screen load
    ''' </summary>
    Private Sub IBarCodeEdit_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        InitializeScreen()
    End Sub

    Private Sub bsElementTextBox_Enter(ByVal sender As Object, ByVal e As System.EventArgs) Handles bsElementTextBox.Enter
        ValidateBarCodeTextBox()
    End Sub

    Private Sub bsElementTextBox_Leave(ByVal sender As Object, ByVal e As System.EventArgs) Handles bsElementTextBox.Leave
        ValidateBarCodeTextBox()
    End Sub

    ''' <summary>
    ''' Validates the length of the entered BarCode. If succeed, enables the Accept button. Disable it otherwise.
    ''' </summary>
    ''' <remarks>
    ''' Created by:  RH 07/09/2011
    ''' Modified by: SG 12/03/2013
    '''              TR 29/08/2013 -change the validation now the new funcionality 
    '''                             only alow the user to indicate the Sample Type position
    '''                             Validate  only with the value of sample type max length.
    ''' </remarks>
    Private Sub bsElementTextBox_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsElementTextBox.TextChanged
        ValidateBarCodeTextBox()
        'SG 12/03/2013
        If MyClass.IsBarcodeSampleIdFlag Then
            If MyClass.IsSampleTypeFlag Then
                bsAcceptButton.Enabled = (bsElementTextBox.Text.Length >= SampleTypeMaxLengthAttr) 'Math.Max(MyClass.SampleIdMaxLengthAttr, SampleTypeMaxLengthAttr))
            Else
                bsAcceptButton.Enabled = (bsElementTextBox.Text.Length >= 1) 'MyClass.SampleIdMaxLengthAttr)
            End If
        Else
            bsAcceptButton.Enabled = True

        End If
        'end SG 12/03/2013

    End Sub

    ''' <summary>
    ''' Accepts selection and close the screen
    ''' </summary>
    ''' <remarks>
    ''' Created by:  RH 07/09/2011
    ''' </remarks>
    Private Sub bsAcceptButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsAcceptButton.Click
        'TR 02/05/2013 -Validate if the textbox is empty to indicate the element can't be removed.
        If (BarCodeAttribute <> String.Empty AndAlso bsElementTextBox.Text = String.Empty) Then
            'Warning: It is not possible to delete an informed Barcode; to do that, the position has to be deleted (content has to be downloaded)
            ShowMessage(Me.Name & ".bsExitButton_Click", GlobalEnumerates.Messages.REMOVEBARCODE.ToString())
        Else
            BarCodeAttribute = Me.bsElementTextBox.Text
        End If

        Close()
    End Sub

    ''' <summary>
    ''' Closes the form when Cancel button is clicked
    ''' </summary>
    ''' <remarks>
    ''' Created by:  RH 07/09/2011
    ''' Modified by: DL 11/10/2011 - Warning Message is shown when the Barcode is informed
    '''              SA 17/10/2011 - The Warning Message should be shown when a Barcode was informed but has been deleted 
    ''' </remarks>
    Private Sub bsExitButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsExitButton.Click
        Try
            If (BarCodeAttribute <> String.Empty AndAlso bsElementTextBox.Text = String.Empty) Then
                'Warning: It is not possible to delete an informed Barcode; to do that, the position has to be deleted (content has to be downloaded)
                ShowMessage(Me.Name & ".bsExitButton_Click", GlobalEnumerates.Messages.REMOVEBARCODE.ToString())
            End If

            BarCodeAttribute = String.Empty
            If (Not Me.MdiParent Is Nothing) Then
                If (Not Me.Tag Is Nothing) Then
                    'A PerformClick() method was executed
                    Me.Close()
                Else
                    'Normal button click
                    'Open the WS Monitor form and close this one
                    IAx00MainMDI.OpenMonitorForm(Me)
                End If
            Else
                Me.Close()
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "bsExitButton_Click " & Me.Name, EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsExitButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

#End Region


End Class