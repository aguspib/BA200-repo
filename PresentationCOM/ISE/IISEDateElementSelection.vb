Option Explicit On
Option Strict On

Imports Biosystems.Ax00.Global.GlobalEnumerates
Imports Biosystems.Ax00.BL
Imports System.Drawing
Imports Biosystems.Ax00.Global
Imports System.Windows.Forms

Public Class UiISEDateElementSelection
    Inherits Biosystems.Ax00.PresentationCOM.BSBaseForm

#Region "Constructor"

    Public Sub New()

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

    End Sub

    Public Sub New(ByVal pLiEnabled As Boolean, ByVal pLiInUse As Boolean)
        MyClass.InitializeComponent()
        MyClass.IsLiEnabledAttr = pLiEnabled
        MyClass.IsLithiumInUseAttr = pLiInUse
    End Sub

#End Region

#Region "Attributes"
    Private TitleLabelAttr As String

    Private IsLiEnabledAttr As Boolean = False

    Private Element1NameAttr As String
    Private Element1SelectedAttr As Boolean
    Private Element1DateTimeAttr As DateTime

    Private Element2NameAttr As String
    Private Element2SelectedAttr As Boolean
    Private Element2DateTimeAttr As DateTime

    Private Element3NameAttr As String
    Private Element3SelectedAttr As Boolean
    Private Element3DateTimeAttr As DateTime

    Private Element4NameAttr As String
    Private Element4SelectedAttr As Boolean
    Private Element4DateTimeAttr As DateTime

    Private Element5NameAttr As String
    Private Element5SelectedAttr As Boolean
    Private Element5DateTimeAttr As DateTime

    Private ShortModeAttr As Boolean

    Private IsLithiumInUseAttr As Boolean = False 'SGM 17/05/2012

#End Region

#Region "Properties"
    Public WriteOnly Property TitleLabel() As String
        Set(ByVal value As String)
            Me.TitleLabelAttr = value
        End Set
    End Property

    Public WriteOnly Property Element1Name() As String
        Set(ByVal value As String)
            Me.Element1NameAttr = value
        End Set
    End Property

    Public Property Element1Selected() As Boolean
        Get
            Return Me.Element1SelectedAttr
        End Get
        Set(ByVal value As Boolean)
            Me.Element1SelectedAttr = value
        End Set
    End Property

    Public Property Element1DateTime() As DateTime
        Get
            Return Me.Element1DateTimeAttr
        End Get
        Set(ByVal value As DateTime)
            Me.Element1DateTimeAttr = value
        End Set
    End Property

    Public WriteOnly Property Element2Name() As String
        Set(ByVal value As String)
            Me.Element2NameAttr = value
        End Set
    End Property

    Public Property Element2Selected() As Boolean
        Get
            Return Me.Element2SelectedAttr
        End Get
        Set(ByVal value As Boolean)
            Me.Element2SelectedAttr = value
        End Set
    End Property

    Public Property Element2DateTime() As DateTime
        Get
            Return Me.Element2DateTimeAttr
        End Get
        Set(ByVal value As DateTime)
            Me.Element2DateTimeAttr = value
        End Set
    End Property

    Public WriteOnly Property Element3Name() As String
        Set(ByVal value As String)
            Me.Element3NameAttr = value
        End Set
    End Property

    Public Property Element3Selected() As Boolean
        Get
            Return Me.Element3SelectedAttr
        End Get
        Set(ByVal value As Boolean)
            Me.Element3SelectedAttr = value
        End Set
    End Property

    Public Property Element3DateTime() As DateTime
        Get
            Return Me.Element3DateTimeAttr
        End Get
        Set(ByVal value As DateTime)
            Me.Element3DateTimeAttr = value
        End Set
    End Property

    Public WriteOnly Property Element4Name() As String
        Set(ByVal value As String)
            Me.Element4NameAttr = value
        End Set
    End Property

    Public Property Element4Selected() As Boolean
        Get
            Return Me.Element4SelectedAttr
        End Get
        Set(ByVal value As Boolean)
            Me.Element4SelectedAttr = value
        End Set
    End Property

    Public Property Element4DateTime() As DateTime
        Get
            Return Me.Element4DateTimeAttr
        End Get
        Set(ByVal value As DateTime)
            Me.Element4DateTimeAttr = value
        End Set
    End Property

    Public WriteOnly Property Element5Name() As String
        Set(ByVal value As String)
            Me.Element5NameAttr = value
        End Set
    End Property

    Public Property Element5Selected() As Boolean
        Get
            Return Me.Element5SelectedAttr
        End Get
        Set(ByVal value As Boolean)
            Me.Element5SelectedAttr = value
        End Set
    End Property

    Public Property Element5DateTime() As DateTime
        Get
            Return Me.Element5DateTimeAttr
        End Get
        Set(ByVal value As DateTime)
            Me.Element5DateTimeAttr = value
        End Set
    End Property

    Public WriteOnly Property ShortMode() As Boolean
        Set(ByVal value As Boolean)
            Me.ShortModeAttr = value
        End Set
    End Property

    'SGM 17/05/2012
    Public WriteOnly Property IsLithiumInUse() As Boolean
        Set(ByVal value As Boolean)
            MyClass.IsLithiumInUseAttr = value
        End Set
    End Property



#End Region

#Region "Constants"
    Private Const Window_Width_Short As Integer = 280
    Private Const Window_Height_Short As Integer = 194

    Private Const Window_Width_Large As Integer = 280
    Private Const Window_Height_Large As Integer = 382

    Private Const GroupBox_Width_Short As Integer = 250
    Private Const GroupBox_Height_Short As Integer = 108
    Private Const GroupBox_Location_X As Integer = 10
    Private Const GroupBox_Location_Y As Integer = 7
#End Region

#Region "Declarations"
    ' Language
    Private currentLanguage As String
#End Region

#Region "Private Methods"
    ''' <summary>
    ''' Search Icons for screen buttons
    ''' </summary>
    ''' <remarks>
    ''' Created by XBC 07/03/2012
    ''' </remarks>
    Private Sub PrepareButtons()
        Try
            Dim auxIconName As String = ""
            Dim iconPath As String = MyBase.IconsPath

            'ACCEPT Button
            auxIconName = GetIconName("ACCEPT1")
            If (auxIconName <> "") Then
                bsAcceptButton.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
            End If

            'CANCEL Button
            auxIconName = GetIconName("CANCEL")
            If (auxIconName <> "") Then
                bsExitButton.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".PrepareButtons", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PrepareButtons", Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
            Me.Close()
        End Try
    End Sub

    ''' <summary>
    ''' Get texts in the current application language for all screen controls
    ''' </summary>
    ''' <remarks>
    ''' Created by: XBC 07/03/2012
    ''' </remarks>
    Private Sub GetScreenLabels()
        Try
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            Me.bsDateSelectionLabel.Text = Me.TitleLabelAttr
            Me.bsLabel1.Text = Me.Element1NameAttr
            Me.bsLabel2.Text = Me.Element2NameAttr
            Me.bsLabel3.Text = Me.Element3NameAttr
            Me.bsLabel4.Text = Me.Element4NameAttr
            Me.BsLabel5.Text = Me.Element5NameAttr

            ' Checkboxes
            Me.bsRefCheckBox.Checked = Me.Element1SelectedAttr
            Me.BsNaCheckbox.Checked = Me.Element2SelectedAttr
            Me.BsKCheckbox.Checked = Me.Element3SelectedAttr
            Me.BsClCheckbox.Checked = Me.Element4SelectedAttr
            Me.BsLiCheckbox.Checked = Me.Element5SelectedAttr

            ' Tooltips
            GetScreenTooltip()

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".GetScreenLabels", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".GetScreenLabels", Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Get texts in the current application language for all screen controls
    ''' </summary>
    ''' <remarks>
    ''' Created by: XBC 07/03/2012
    ''' </remarks>
    Private Sub GetScreenTooltip()
        Try
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            ' For Tooltips...
            Me.bsScreenToolTips.SetToolTip(bsAcceptButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_AcceptSelection", currentLanguage))
            Me.bsScreenToolTips.SetToolTip(bsExitButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Cancel", currentLanguage))

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".GetScreenTooltip ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".GetScreenTooltip ", Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub
#End Region

#Region "Events"
    Private Sub IISEDateElementSelection_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try

            'Get the current Language from the current Application Session
            'Dim currentLanguageGlobal As New GlobalBase
            Me.currentLanguage = GlobalBase.GetSessionInfo().ApplicationLanguage.Trim.ToString

            'Load the multilanguage texts for all Screen Labels and get Icons for graphical Buttons
            Me.GetScreenLabels()
            Me.PrepareButtons()

            ' Configure Short/Large mode
            If Me.ShortModeAttr Then
                Me.Size = New System.Drawing.Size(Window_Width_Short, Window_Height_Short)
                Me.bsChangePwdGroupBox.Size = New System.Drawing.Size(GroupBox_Width_Short, GroupBox_Height_Short)
                Me.bsChangePwdGroupBox.Location = New Point(GroupBox_Location_X, GroupBox_Location_Y)

                Me.bsLabel2.Visible = False
                Me.bsLabel3.Visible = False
                Me.bsLabel4.Visible = False
                Me.BsLabel5.Visible = False

                Me.BsNaDateTimePicker.Visible = False
                Me.BsKDateTimePicker.Visible = False
                Me.BsClDateTimePicker.Visible = False
                Me.BsLiDateTimePicker.Visible = False

                ' XBC 29/06/2012
                Me.BsRefDateTimePicker.Value = Me.Element1DateTimeAttr
            Else
                ' XBC 29/06/2012
                Me.BsRefDateTimePicker.Value = Me.Element1DateTimeAttr
                Me.BsNaDateTimePicker.Value = Me.Element2DateTimeAttr
                Me.BsKDateTimePicker.Value = Me.Element3DateTimeAttr
                Me.BsClDateTimePicker.Value = Me.Element4DateTimeAttr
                Me.BsLiDateTimePicker.Value = Me.Element5DateTimeAttr
            End If

            Me.BsLiCheckbox.Enabled = Not MyClass.IsLithiumInUseAttr
            Me.BsLiCheckbox.Checked = MyClass.IsLiEnabledAttr

            ' XBC 29/06/2012
            Me.BsRefDateTimePicker.MaxDate = DateTime.Now
            'SGM 07/06/2012
            Me.BsNaDateTimePicker.MaxDate = DateTime.Now
            Me.BsKDateTimePicker.MaxDate = DateTime.Now
            Me.BsClDateTimePicker.MaxDate = DateTime.Now
            Me.BsLiDateTimePicker.MaxDate = DateTime.Now

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".Load ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".Load ", Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub bsAcceptButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsAcceptButton.Click
        Try
            Me.Element1DateTime = Me.BsRefDateTimePicker.Value
            Me.Element1SelectedAttr = Me.bsRefCheckBox.Checked

            Me.Element2DateTime = Me.BsNaDateTimePicker.Value
            Me.Element2SelectedAttr = Me.BsNaCheckbox.Checked

            Me.Element3DateTime = Me.BsKDateTimePicker.Value
            Me.Element3SelectedAttr = Me.BsKCheckbox.Checked

            Me.Element4DateTime = Me.BsClDateTimePicker.Value
            Me.Element4SelectedAttr = Me.BsClCheckbox.Checked

            Me.Element5DateTime = Me.BsLiDateTimePicker.Value
            Me.Element5SelectedAttr = Me.BsLiCheckbox.Checked

            'Close the screen
            Me.Close()

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".AcceptButton ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".AcceptButton ", Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub bsExitButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsExitButton.Click
        'Close the screen
        Me.Close()
    End Sub

    ''' <summary>
    ''' When the  ESC key is pressed, the screen is closed 
    ''' </summary>
    ''' <remarks>
    ''' Created by XBC 07/03/2012
    ''' </remarks>
    Private Sub ConfigUsers_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyDown
        Try
            If (e.KeyCode = Keys.Escape) Then
                BsExitButton.PerformClick()
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".KeyDown ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".KeyDown", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

#End Region


    Private Sub BsLiCheckbox_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsLiCheckbox.CheckedChanged
        Me.BsLiDateTimePicker.Enabled = BsLiCheckbox.Checked
    End Sub
End Class