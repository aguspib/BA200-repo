Option Strict On
Option Explicit On
Option Infer On

Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Types

Public Class ApplicationLogView

    'Attribute and Property to receive the identifier of the active WorkSession
    Private WorkSessionIDAttribute As String
    Public WriteOnly Property WorkSessionID() As String
        Set(ByVal value As String)
            WorkSessionIDAttribute = value
        End Set
    End Property

    ''' <summary>
    ''' Load icons for graphical buttons
    ''' </summary>
    ''' <remarks>
    ''' Created by: TR 06/07/2012
    ''' </remarks>
    Private Sub PrepareButtons()
        Try
            Dim auxIconName As String = ""
            Dim iconPath As String = IconsPath

            'LOAD 
            auxIconName = GetIconName("OPEN")
            If (auxIconName <> "") Then
                OpenFileDialogButton.Image = Image.FromFile(iconPath & auxIconName)
            End If

            'EXIT Button
            auxIconName = GetIconName("ACCEPT1")
            If (auxIconName <> "") Then
                ExitButton.Image = Image.FromFile(iconPath & auxIconName)
            End If

            'Export 
            auxIconName = GetIconName("EXPORTSCRIPT")
            If (auxIconName <> "") Then
                GenerateXmlButton.Image = Image.FromFile(iconPath & auxIconName)
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".PrepareButtons ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".PrepareButtons ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Generate the XML Log File without Reset the active WorkSession (and without deleting the tfmwApplicationLog table)
    ''' </summary>
    ''' <remarks>
    ''' Created by: TR 06/07/2012
    ''' </remarks>
    Private Sub GenerateLogXml()
        Try
            Dim myGlobalDataTO As New GlobalDataTO
            'Dim MyApplicationLogManager As New ApplicationLogManager

            myGlobalDataTO = ApplicationLogManager.ExportLogToXml(WorkSessionIDAttribute, 90)

            If (myGlobalDataTO.HasError) Then
                ShowMessage(Me.Name, myGlobalDataTO.ErrorMessage)
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".GenerateLogXml ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".GenerateLogXml ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub OpenFileDialogButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OpenFileDialogButton.Click
        Try
            Dim myOpenFileDialog As New OpenFileDialog
            myOpenFileDialog.InitialDirectory = (Application.StartupPath & GlobalBase.XmlLogFilePath)
            If myOpenFileDialog.ShowDialog() = Windows.Forms.DialogResult.OK Then
                Dim myApplicationLogDS As New ApplicationLogDS
                myApplicationLogDS.ReadXml(myOpenFileDialog.FileName)
                ApplicationLogGridView.DataSource = myApplicationLogDS.tApplicationLog
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".OpenFileDialogButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".OpenFileDialogButton_Click ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub ApplicationLogView_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try
            PrepareButtons()
        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".ApplicationLogView_Load ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ApplicationLogView_Load ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try

    End Sub

    Private Sub ExitButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ExitButton.Click
        Close()
    End Sub

    Private Sub GenerateXmlButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles GenerateXmlButton.Click
        GenerateLogXml()
    End Sub
End Class
