Option Strict On
Option Explicit On

Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Global.GlobalEnumerates
Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.Types

Public Class IWSManualRepetition
    Inherits Biosystems.Ax00.PresentationCOM.BSBaseForm

#Region "Declarations"
    Private RepetitionToAddExists As Boolean
    Private InitialPostDilutionType As PostDilutionTypes  'AG 28/07/2010 PostDilutionTypes
    Private CurrentPostDilutionType As PostDilutionTypes  'AG 28/07/2010 PostDilutionTypes
#End Region

#Region "Attributes"
    Private WorkSessionIDAttribute As String = ""
    Private AnalyzerIDAttribute As String = ""
    Private OrderTestIDAttribute As Integer = 0
    Private ManualRepetitionCriteriaAttribute As PostDilutionTypes = PostDilutionTypes.WITHOUT     'AG 28/07/2010
    Private TestTypeAttribute As String = "STD" 'AG 02/12/2010
    Private SampleClassAttribute As String = ""
#End Region

#Region "Properties"
    Public Property ActiveWorkSession() As String
        Get
            Return WorkSessionIDAttribute
        End Get
        Set(ByVal value As String)
            WorkSessionIDAttribute = value
        End Set
    End Property

    Public Property ActiveAnalyzer() As String
        Get
            Return AnalyzerIDAttribute
        End Get
        Set(ByVal value As String)
            AnalyzerIDAttribute = value
        End Set
    End Property

    Public Property ActiveOrderTestID() As Integer
        Get
            Return OrderTestIDAttribute
        End Get
        Set(ByVal value As Integer)
            OrderTestIDAttribute = value
        End Set
    End Property

    'AG 28/07/2010
    Public Property SelectedManualCriteria() As PostDilutionTypes
        Get
            Return (ManualRepetitionCriteriaAttribute)
        End Get
        Set(ByVal value As PostDilutionTypes)
            ManualRepetitionCriteriaAttribute = value
        End Set
    End Property
    'AG 28/07/2010

    'AG 02/12/2010
    Public Property TestType() As String
        Get
            Return TestTypeAttribute
        End Get
        Set(ByVal value As String)
            TestTypeAttribute = value
        End Set
    End Property
    'END AG 02/12/2010

    'AG 11/03/2011
    Public Property SampleClass() As String
        Get
            Return SampleClassAttribute
        End Get
        Set(ByVal value As String)
            SampleClassAttribute = value
        End Set
    End Property
    'END AG 11/03/2011

#End Region

#Region "Methods"
    ''' <summary>
    ''' Determines if the loaded repetition to add already exists
    ''' </summary>
    ''' <remarks>
    ''' Created by: SG 16/07/2010
    ''' </remarks>
    Private Function ExistsRepetitionToAdd() As GlobalDataTO
        Dim result As New GlobalDataTO

        Try
            Dim myWSRepetitionsToAdd As New WSRepetitionsToAddDS

            result = PrepareQueryForWSRepetitionToAdd(myWSRepetitionsToAdd)
            If (Not result.HasError) Then
                Dim myWSRepetitionsToAddDelegate As New WSRepetitionsToAddDelegate

                result = myWSRepetitionsToAddDelegate.Read(Nothing, myWSRepetitionsToAdd)
                If (Not result.HasError) Then
                    'AG 25/05/2012 - Do not use AffectedRecords
                    'RepetitionToAddExists = (result.AffectedRecords > 0)
                    RepetitionToAddExists = False
                    If CType(result.SetDatos, WSRepetitionsToAddDS).twksWSRepetitionsToAdd.Count > 0 Then
                        RepetitionToAddExists = True
                    End If
                    'AG 25/05/2012

                End If
            Else
                RepetitionToAddExists = False
                ShowMessage(Me.Name, result.ErrorCode, result.ErrorMessage)
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ExistsRepetitionToAdd ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name, Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")

            RepetitionToAddExists = False
            result.HasError = True
        End Try
        Return result
    End Function

    ''' <summary>
    ''' Gets the new data edited by user
    ''' </summary>
    ''' <remarks>
    ''' Created by: SG 16/07/2010
    ''' </remarks>
    Private Sub GetNewPostDilutionType() Handles BsNoRepeat.CheckedChanged, BsIncrease.CheckedChanged, BsEqual.CheckedChanged, BsDecrease.CheckedChanged
        Try
            If (BsEqual.Checked) Then
                CurrentPostDilutionType = PostDilutionTypes.NONE   'AG 28/07/2010 PostDilutionTypes.NONE
            ElseIf (BsIncrease.Checked) Then
                CurrentPostDilutionType = PostDilutionTypes.INC   'AG 28/07/2010 PostDilutionTypes.INC
            ElseIf (BsDecrease.Checked) Then
                CurrentPostDilutionType = PostDilutionTypes.RED   'AG 28/07/2010 PostDilutionTypes.RED
            ElseIf (BsNoRepeat.Checked) Then
                CurrentPostDilutionType = PostDilutionTypes.WITHOUT    'AG 28/07/2010 PostDilutionTypes.WITHOUT
            Else
                CurrentPostDilutionType = PostDilutionTypes.UNDEFINED    'AG 28/07/2010 PostDilutionTypes.WITHOUT
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".GetNewPostDilutionType ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name, Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Get texts in the current application language for all screen controls
    ''' </summary>
    ''' <param name="pLanguageID"> The current Language of Application </param>
    ''' <remarks>
    ''' Created by: PG 07/10/10
    ''' </remarks>
    Private Sub GetScreenLabels(ByVal pLanguageID As String)
        Try
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            'For Labels, CheckBox, RadioButtons.....
            BsNoRepeat.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_ManualRerun_None", pLanguageID)
            BsDecrease.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_ManualRerun_Decreasing", pLanguageID)
            BsEqual.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_ManualRerun_Equal", pLanguageID)
            BsIncrease.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_ManualRerun_Increasing", pLanguageID)
            bsManualRepetitionTitleLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "TITLE_ManualRerun", pLanguageID)

            'Tooltips
            bsScreenToolTips.SetToolTip(bsExitButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Save&Close", pLanguageID))
            bsScreenToolTips.SetToolTip(bsCancelButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Cancel&Close", pLanguageID))
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".GetScreenLabels ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name, Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Get icons for all graphical buttons in the screen
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub PrepareButtons()
        Try
            Dim auxIconName As String = String.Empty
            Dim iconPath As String = MyBase.IconsPath

            'ACCEPT Button
            auxIconName = GetIconName("ACCEPT1")
            If (auxIconName <> "") Then bsExitButton.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)

            'CANCEL Button
            auxIconName = GetIconName("CANCEL")
            If (auxIconName <> "") Then bsCancelButton.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".PrepareButtons", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name, Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Add all information needed to create the Manual Repetition in a typed DataSet WSRepetitionsToAddDS
    ''' </summary>
    ''' <param name="myWSRepetitionsToAdd"></param>
    ''' <param name="pPostDilutionType"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' Created by: SG 16/07/2010
    ''' </remarks>
    Private Function PrepareQueryForWSRepetitionToAdd(ByRef myWSRepetitionsToAdd As WSRepetitionsToAddDS, Optional ByVal pPostDilutionType As String = "") As GlobalDataTO
        Dim result As New GlobalDataTO

        Try
            Dim myWSRepetitionsToAddRow As WSRepetitionsToAddDS.twksWSRepetitionsToAddRow
            myWSRepetitionsToAddRow = myWSRepetitionsToAdd.twksWSRepetitionsToAdd.NewtwksWSRepetitionsToAddRow

            myWSRepetitionsToAddRow.BeginEdit()
            With myWSRepetitionsToAddRow
                .AnalyzerID = Me.AnalyzerIDAttribute
                .WorkSessionID = Me.WorkSessionIDAttribute
                If (Me.OrderTestIDAttribute > 0) Then .OrderTestID = Me.OrderTestIDAttribute
                If (pPostDilutionType <> "") Then .PostDilutionType = pPostDilutionType
            End With
            myWSRepetitionsToAddRow.EndEdit()
            myWSRepetitionsToAdd.twksWSRepetitionsToAdd.AddtwksWSRepetitionsToAddRow(myWSRepetitionsToAddRow)
            myWSRepetitionsToAdd.AcceptChanges()

            result.HasError = False
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".PrepareQueryForWSRepetitionToAdd ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name, Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")

            RepetitionToAddExists = False
            result.HasError = True
        End Try
        Return result
    End Function

    ''' <summary>
    ''' Save data of the requested Manual Repetition in the DB
    ''' </summary>
    ''' <remarks>
    ''' Created by: SG 16/07/2010
    ''' </remarks>
    Private Sub SaveChanges()
        Dim result As New GlobalDataTO

        Try
            Dim newPostDilutionValue As String = Me.CurrentPostDilutionType.ToString
            Dim myWSRepetitionsToAdd As New WSRepetitionsToAddDS
            result = PrepareQueryForWSRepetitionToAdd(myWSRepetitionsToAdd, newPostDilutionValue)

            If (Not result.HasError) Then
                Dim myWSRepetitionsToAddDelegate As New WSRepetitionsToAddDelegate

                'AG 25/05/2012
                'If (InitialPostDilutionType <> CurrentPostDilutionType) Then
                '    If (Me.CurrentPostDilutionType = PostDilutionTypes.WITHOUT) Then
                '        If (RepetitionToAddExists) Then
                '            result = myWSRepetitionsToAddDelegate.Delete(Nothing, myWSRepetitionsToAdd)
                '        End If
                '    Else
                '        If (RepetitionToAddExists) Then
                '            result = myWSRepetitionsToAddDelegate.Update(Nothing, myWSRepetitionsToAdd)
                '        Else
                '            result = myWSRepetitionsToAddDelegate.Add(Nothing, myWSRepetitionsToAdd)
                '        End If

                '    End If
                '    ManualRepetitionCriteriaAttribute = CurrentPostDilutionType 'AG 28/07/2010 - Inform the attribute

                'ElseIf (Not RepetitionToAddExists) Then
                '    result = myWSRepetitionsToAddDelegate.Add(Nothing, myWSRepetitionsToAdd)
                'End If

                If (Me.CurrentPostDilutionType = PostDilutionTypes.WITHOUT) Then
                    result = myWSRepetitionsToAddDelegate.Delete(Nothing, myWSRepetitionsToAdd)
                Else
                    If (RepetitionToAddExists) Then
                        result = myWSRepetitionsToAddDelegate.Update(Nothing, myWSRepetitionsToAdd)
                    Else
                        result = myWSRepetitionsToAddDelegate.Add(Nothing, myWSRepetitionsToAdd)
                    End If
                End If
                ManualRepetitionCriteriaAttribute = CurrentPostDilutionType 'AG 28/07/2010 - Inform the attribute
                'AG 25/05/2012

                If (result.HasError) Then
                    ShowMessage(Me.Name, result.ErrorCode, result.ErrorMessage)
                End If
            Else
                ShowMessage(Me.Name, result.ErrorCode, result.ErrorMessage)
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".SaveChanges ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name, Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Determines which option is selected when loading the form
    ''' </summary>
    ''' <remarks>
    ''' Created by: SG 16/07/2010
    ''' AG 25/05/2012 - Set NONE (repeat equal) as default value
    ''' </remarks>
    Private Sub SetUpPostDilutionTypes()
        Try
            Dim inputRepetitionToAdd As GlobalDataTO

            inputRepetitionToAdd = ExistsRepetitionToAdd()
            'AG 25/05/2012
            'If (inputRepetitionToAdd.AffectedRecords > 0 AndAlso Not inputRepetitionToAdd.HasError) Then
            If RepetitionToAddExists Then
                'AG 25/05/2012
                Dim type As String = DirectCast(inputRepetitionToAdd.SetDatos, WSRepetitionsToAddDS).twksWSRepetitionsToAdd.First.PostDilutionType.ToUpper
                Select Case (type)
                    Case "INC"
                        InitialPostDilutionType = PostDilutionTypes.INC   'AG 28/07/2010 PostDilutionTypes.INC
                        Me.BsEqual.Checked = False
                        Me.BsIncrease.Checked = True
                        Me.BsDecrease.Checked = False
                        Me.BsNoRepeat.Checked = False

                    Case "RED"
                        InitialPostDilutionType = PostDilutionTypes.RED   'AG 28/07/2010 PostDilutionTypes.RED
                        Me.BsEqual.Checked = False
                        Me.BsIncrease.Checked = False
                        Me.BsDecrease.Checked = True
                        Me.BsNoRepeat.Checked = False

                    Case "NONE"
                        InitialPostDilutionType = PostDilutionTypes.NONE   'AG 28/07/2010 PostDilutionTypes.NONE
                        Me.BsEqual.Checked = True
                        Me.BsIncrease.Checked = False
                        Me.BsDecrease.Checked = False
                        Me.BsNoRepeat.Checked = False

                    Case Else
                        'AG 25/05/2012 - Set NONE as default value
                        'InitialPostDilutionType = PostDilutionTypes.WITHOUT 'AG 02/12/2010
                        'Me.BsEqual.Checked = False
                        'Me.BsIncrease.Checked = False
                        'Me.BsDecrease.Checked = False
                        'Me.BsNoRepeat.Checked = True
                        InitialPostDilutionType = PostDilutionTypes.NONE   'AG 28/07/2010 PostDilutionTypes.NONE
                        Me.BsEqual.Checked = True
                        Me.BsIncrease.Checked = False
                        Me.BsDecrease.Checked = False
                        Me.BsNoRepeat.Checked = False

                End Select
            Else
                'AG 25/05/2012 - Set NONE as default value
                'InitialPostDilutionType = PostDilutionTypes.WITHOUT   'AG 28/0/2010 PostDilutionTypes.NONE
                'Me.BsEqual.Checked = False
                'Me.BsIncrease.Checked = False
                'Me.BsDecrease.Checked = False
                'Me.BsNoRepeat.Checked = True
                InitialPostDilutionType = PostDilutionTypes.NONE   'AG 28/07/2010 PostDilutionTypes.NONE
                Me.BsEqual.Checked = True
                Me.BsIncrease.Checked = False
                Me.BsDecrease.Checked = False
                Me.BsNoRepeat.Checked = False
                'AG 25/05/2012
            End If

            'AG 02/12/2010
            If (TestTypeAttribute <> "STD" OrElse SampleClassAttribute <> "PATIENT") Then 'Allow only repetition with equal criteria
                Me.BsIncrease.Checked = False
                Me.BsIncrease.Enabled = False
                Me.BsDecrease.Checked = False
                Me.BsDecrease.Enabled = False
            End If
            'END AG 02/12/2010

            ManualRepetitionCriteriaAttribute = InitialPostDilutionType 'AG 28/07/2010 - Inform the attribute
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".SetUpPostDilutionTypes ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name, Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub
#End Region

#Region "Events"
    Private Sub IWSManualRepetition_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyDown
        Try
            If (e.KeyCode = Keys.Escape) Then bsCancelButton.PerformClick()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".IWSManualRepetition_KeyDown", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".IWSManualRepetition_KeyDown", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub IWSManualRepetition_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            'Dim currentLanguageGlobal As New GlobalBase
            Dim currentLanguage As String = GlobalBase.GetSessionInfo().ApplicationLanguage.Trim.ToString

            PrepareButtons()
            GetScreenLabels(currentLanguage)

            'If required attributes arent informed then disable all controls but Cancel button
            bsCancelButton.Enabled = True
            If (AnalyzerIDAttribute = "" OrElse WorkSessionIDAttribute = "" OrElse OrderTestIDAttribute = 0) Then
                bsLanguageSelectionGroupBox.Enabled = False
                bsExitButton.Enabled = False
            Else
                bsLanguageSelectionGroupBox.Enabled = True
                bsExitButton.Enabled = True

                SetUpPostDilutionTypes() 'SG 16/07/2010
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".IWSManualRepetition_Load", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name, Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    Private Sub bsCancelButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsCancelButton.Click
        Try
            ManualRepetitionCriteriaAttribute = PostDilutionTypes.UNDEFINED
            Me.Close()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsCancelButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name, Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    Private Sub bsExitButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsExitButton.Click
        Try
            SaveChanges()
            Me.Close()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsExitButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name, Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub
#End Region

#Region "TO REVIEW - DELETE?"
    'AG 28/07/2010 - Move to GlobalEnumerates class
    'Private Enum PostDilutionTypes
    '    INC
    '    RED
    '    NONE
    '    WITHOUT
    'End Enum
    'END AG 28/07/2010

    '''' <summary>
    '''' Gets the string that defines a spicified postdilutiontype
    '''' </summary>
    '''' <returns></returns>
    '''' <remarks>created by SG 16/07/2010</remarks>
    'Private Function GetPostDilutionDefinition(ByVal type As PostDilutionTypes) As String

    '    Dim myPreloadedMaster As New PreloadedMasterDataDelegate
    '    Dim PreloadedMasterDS As New PreloadedMasterDataDS

    '    Try
    '        PreloadedMasterDS = CType(myPreloadedMaster.GetSubTableItemByFixedItemDesc(Nothing, GlobalEnumerates.PreloadedMasterDataEnum.POSTDILUTION_TYPE, type.ToString).SetDatos, PreloadedMasterDataDS)
    '        If PreloadedMasterDS.tfmwPreloadedMasterData.Rows.Count > 0 Then
    '            Return PreloadedMasterDS.tfmwPreloadedMasterData(0).ItemID.ToString
    '        Else
    '            Return ""
    '        End If
    '    Catch ex As Exception
    '        CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".GetPostDilutionDefinition ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
    '        ShowMessage(Me.Name, GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
    '        Return ""
    '    End Try
    'End Function

    ''AG 16/06/2010 Adapted from WSTestSelectionAuxScreen
    ''ALL THESE FUNCTIONS MUST BE REMOVED AND REPLACED FOR THE FINAL ONES!!


    'Private Sub GetScreenLabels_ForTESTING(ByVal pControlCollection As System.Windows.Forms.Control.ControlCollection)
    '    Dim screenControl As New Control
    '    For Each screenControl In pControlCollection
    '        If (screenControl.GetType().ToString = "Biosystems.Ax00.Controls.UserControls.BSLabel") Or _
    '           (screenControl.GetType().ToString = "Biosystems.Ax00.Controls.UserControls.BSRadioButton") Or _
    '           (screenControl.GetType().ToString = "Biosystems.Ax00.Controls.UserControls.BSCheckbox") Then
    '            screenControl.Text = GetMultilanguageDescription_ForTESTING(screenControl.Name)
    '            If (screenControl.GetType().ToString = "Biosystems.Ax00.Controls.UserControls.BSLabel" AndAlso _
    '                screenControl.BackColor <> Color.LightSteelBlue) Then screenControl.Text += ":"

    '        ElseIf (screenControl.GetType().ToString = "Biosystems.Ax00.Controls.UserControls.BSButton") Then
    '            bsScreenToolTips.SetToolTip(screenControl, GetMultilanguageDescription_ForTESTING(screenControl.Name & "ToolTip"))

    '        ElseIf (screenControl.GetType().ToString = "Biosystems.Ax00.Controls.UserControls.BSGroupBox") Or _
    '               (screenControl.GetType().ToString = "Biosystems.Ax00.Controls.UserControls.BSTabControl") Or _
    '               (screenControl.GetType().ToString = "System.Windows.Forms.TabPage") Then
    '            screenControl.Text = GetMultilanguageDescription_ForTESTING(screenControl.Name)
    '            GetScreenLabels_ForTESTING(screenControl.Controls)
    '        End If
    '    Next
    'End Sub


    'Private Function GetMultilanguageDescription_ForTESTING(ByVal pControlName As String) As String
    '    Dim textToShow As String = ""
    '    Select Case (pControlName)
    '        Case "bsManualRepetitionTitleLabel"
    '            textToShow = "Select Repetition Criterion"
    '        Case "BsIncrease"
    '            textToShow = "Repeat Increasing"
    '        Case "BsEqual"
    '            textToShow = "Repeat Equal"
    '        Case "BsDecrease"
    '            textToShow = "Repeat Decreasing"
    '        Case "BsNoRepeat"
    '            textToShow = "Do not Repeat"
    '        Case "bsExitButtonToolTip"
    '            textToShow = "Accept changes and close"
    '        Case "bsCancelButtonToolTip"
    '            textToShow = "Discard changes and close"
    '    End Select
    '    Return textToShow
    'End Function
#End Region

End Class