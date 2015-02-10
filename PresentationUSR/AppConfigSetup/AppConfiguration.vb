Option Explicit On
Option Strict On
Option Infer On

Imports Biosystems.Ax00.Global

Public Class AppConfiguration
    Inherits System.Windows.Forms.Form

#Region "Properties"

    ''' <summary>
    ''' Server name
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Property DataSource() As String
        Get
            Return Me.DataSourceTextBox.Text
        End Get
        Set(ByVal value As String)
            Me.DataSourceTextBox.Text = value
        End Set
    End Property

    ''' <summary>
    ''' Database Name
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Property InitialCaltalog() As String
        Get
            Return Me.DataBaseNameTextBox.Text
        End Get
        Set(ByVal value As String)
            Me.DataBaseNameTextBox.Text = value
        End Set
    End Property

    ''' <summary>
    ''' Integrated Security.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Property IntegratedSecurity() As Boolean
        Get
            Return Me.IntegratedSecurityCheckbox.Checked
        End Get
        Set(ByVal value As Boolean)
            Me.IntegratedSecurityCheckbox.Checked = value
        End Set
    End Property

    ''' <summary>
    ''' Database User ID
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Property UserID() As String
        Get
            Return Me.UserIDTextBox.Text
        End Get
        Set(ByVal value As String)
            Me.UserIDTextBox.Text = value
        End Set
    End Property

    ''' <summary>
    ''' Database User Password
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Property Password() As String
        Get
            Return Me.PasswordTextBox.Text
        End Get
        Set(ByVal value As String)
            Me.PasswordTextBox.Text = value
        End Set
    End Property

#End Region

#Region "Sub and Functions"

    ''' <summary>
    ''' Get the current Connection string from app.config, an fill the diferent components.
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub GetConnectionStringInfo()

        Try
            Dim myConnectionString As String = ""
            Dim mySecurity As New Security.Security
            'myConnectionString = mySecurity.Decryption(ConfigurationManager.ConnectionStrings("BiosystemsConn").ConnectionString)
            'TR 25/01/2011 -Replace by corresponding value on global base.
            myConnectionString = mySecurity.Decryption(GlobalBase.BioSystemsDBConn)

            If myConnectionString <> "" Then

                ConnectionStringTextBox.Text = myConnectionString

                For Each i As String In myConnectionString.Split(CChar(";"))
                    If i.Contains("Data Source") Then ' get the datasource value
                        DataSource = i
                    ElseIf i.Contains("Initial Catalog") Then ' get the initial catalog value
                        InitialCaltalog = i
                    End If
                    If i.Contains("Integrated Security") Then
                        If i.Contains("True") Then
                            IntegratedSecurity = True
                        Else
                            IntegratedSecurity = False
                        End If
                    End If

                    If i.Contains("User ID") Then
                        UserID = i
                    End If
                    If i.Contains("Password") Then
                        Password = i
                    End If
                Next
            Else
                'validate if connection string is empty to send an error
                'GlobalBase.CreateLogActivity("Error Reading the Connection String .", "DAOBase", EventLogEntryType.Error, False)
            End If

        Catch ex As Exception
            'GlobalBase.CreateLogActivity(ex.Message, "AppConfiguration.GetConnectionStringInfo", EventLogEntryType.Error, False)
        End Try
    End Sub

    ''' <summary>
    ''' Create a Connection string from the form input data
    ''' </summary>
    ''' <returns>New Conection String </returns>
    ''' <remarks></remarks>
    Private Function CreateConectionString() As String
        Dim newConnectionString As String = ""
        Try
            newConnectionString = DataSource & ";" & InitialCaltalog & ";Integrated Security=" & IntegratedSecurity & ";" & UserID & ";" & Password
        Catch ex As Exception
            'GlobalBase.CreateLogActivity(ex.Message, "AppConfiguration.CreateConectionString", EventLogEntryType.Error, False)
        End Try
        Return newConnectionString
    End Function

    ''' <summary>
    ''' Update the app.config file
    ''' </summary>
    ''' <param name="Value"></param>
    ''' <remarks></remarks>
    Private Sub UpdateAppConfigFile(ByVal Value As String)
        Try
            Dim myConnectionString As String = ""
            Dim mySecurity As New Security.Security
            myConnectionString = mySecurity.Encryption(Value) ' desencryt the connection string.
            AppConfigFileSetting.UpdateAppSettings("BiosystemsConn", myConnectionString)
            'Message after changes.
            Dim restartAppMessage As String = "In Order to apply changes, you need to restart the application."
            'show message indicating next step for new changes.
            MessageBox.Show(restartAppMessage, "Information", MessageBoxButtons.OK, MessageBoxIcon.Information)

        Catch ex As Exception
            'GlobalBase.CreateLogActivity(ex.Message, "AppConfiguration.UpdateAppConfigFile", EventLogEntryType.Error, False)
        End Try
    End Sub

#End Region

#Region "Events"

    Private Sub AppConfiguration_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        GetConnectionStringInfo()
    End Sub

    Private Sub ChangeAppConfigButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ChangeAppConfigButton.Click
        UpdateAppConfigFile(CreateConectionString())
    End Sub

    Private Sub CloseButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CloseButton.Click
        Me.Close()
    End Sub

    Private Sub GetApplicationButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles GetApplicationButton.Click
        Dim appInfo As New SystemInfoManager
        ' get all the computer, installed applications.
        IntalledApplicationGridView.DataSource = appInfo.GetAllInstallApplications().InstalleApplication
    End Sub

#End Region

    
End Class