Option Explicit On
Option Strict On

Imports System.Windows.Forms
Imports Biosystems.Ax00.Global

Public Class IMsgBox
    Inherits Biosystems.Ax00.PresentationCOM.BSBaseForm

    Private MessageAttr As String
    Private TitleAttr As String
    Private ButtonsAttr As MessageBoxButtons
    Private IconsAttr As MessageBoxIcon

    Private WithEvents myOKButton As Button
    Private WithEvents myYESButton As Button
    Private WithEvents myNOButton As Button
    Private WithEvents myCancelButton As Button

    Public Sub New()

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

    End Sub

    Public Function ShowMsg(ByVal pMessage As String, ByVal pTitle As String, Optional ByVal pButtons As MessageBoxButtons = MessageBoxButtons.OK, Optional ByVal pIcon As MessageBoxIcon = MessageBoxIcon.None) As DialogResult

        Dim res As DialogResult = Windows.Forms.DialogResult.None

        Try

            Me.Text = pTitle
            MyClass.SetMessageText(pMessage)
            MyClass.SetButtons(pButtons)
            MyClass.SetIconImage(pIcon)

            res = Me.ShowDialog()

        Catch ex As Exception
            Throw ex
        End Try
        Return res
    End Function

    Private Sub LoadIconImages()
        Dim iconPath As String = MyBase.IconsPath
        Try
            'from images to ImageList
        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".LoadIconImages", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
        End Try
    End Sub

    Private Sub SetMessageText(ByVal pText As String)
        Try
            Me.BsMessageLabel.Text = pText

            Dim text As String = Me.BsMessageLabel.Text
            Dim textfont As Font = Me.BsMessageLabel.Font

            Dim layoutsize As SizeF = New SizeF(Me.BsMessageLabel.Width, 5000.0)
            Dim g As Graphics = Graphics.FromHwnd(Me.BsMessageLabel.Handle)
            Dim StringSize As SizeF = g.MeasureString(text, textfont, layoutsize)

            g.Dispose()

            Me.BsMessageLabel.Height = CType(StringSize.Height, Integer)

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".SetMessageText", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
        End Try
    End Sub

    Private Sub SetButtons(ByVal pButtons As MessageBoxButtons)
        Try

            Select Case pButtons
                Case MessageBoxButtons.OK
                    Me.myOKButton = New Button
                    Me.myOKButton.Text = "OK"
                    Me.myOKButton.Visible = True
                    Me.ButtonsLayoutPanel.ColumnCount = 1
                    Me.ButtonsLayoutPanel.Controls.Add(Me.myOKButton, 0, 0)

                Case MessageBoxButtons.OKCancel
                    Me.myOKButton = New Button
                    Me.myOKButton.Text = "OK"
                    Me.myOKButton.Visible = True
                    Me.myCancelButton = New Button
                    Me.myCancelButton.Text = "Cancel"
                    Me.myCancelButton.Visible = True
                    Me.ButtonsLayoutPanel.ColumnCount = 2
                    Me.ButtonsLayoutPanel.Controls.Add(Me.myOKButton, 0, 0)
                    Me.ButtonsLayoutPanel.Controls.Add(Me.myCancelButton, 1, 0)

                Case MessageBoxButtons.YesNo
                    Me.myYESButton = New Button
                    Me.myYESButton.Text = "Yes"
                    Me.myYESButton.Visible = True
                    Me.myNOButton = New Button
                    Me.myNOButton.Text = "No"
                    Me.myNOButton.Visible = True
                    Me.ButtonsLayoutPanel.ColumnCount = 2
                    Me.ButtonsLayoutPanel.Controls.Add(Me.myYESButton, 0, 0)
                    Me.ButtonsLayoutPanel.Controls.Add(Me.myNOButton, 1, 0)

                Case MessageBoxButtons.YesNoCancel
                    Me.myYESButton = New Button
                    Me.myYESButton.Text = "Yes"
                    Me.myYESButton.Visible = True
                    Me.myNOButton = New Button
                    Me.myNOButton.Text = "No"
                    Me.myNOButton.Visible = True
                    Me.myCancelButton = New Button
                    Me.myCancelButton.Text = "Cancel"
                    Me.myCancelButton.Visible = True
                    Me.ButtonsLayoutPanel.ColumnCount = 3
                    Me.ButtonsLayoutPanel.Controls.Add(Me.myYESButton, 0, 0)
                    Me.ButtonsLayoutPanel.Controls.Add(Me.myNOButton, 1, 0)
                    Me.ButtonsLayoutPanel.Controls.Add(Me.myCancelButton, 2, 0)

            End Select

            Me.ButtonsLayoutPanel.Refresh()

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".SetButtons", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
        End Try
    End Sub


    Private Sub SetIconImage(ByVal pIconType As MessageBoxIcon)

        Dim myGlobal As New GlobalDataTO


        Dim myUtil As New Utilities

        Try


            'get form image list
            Select Case pIconType
                Case MessageBoxIcon.Error

                Case MessageBoxIcon.Exclamation

                Case MessageBoxIcon.Warning

                Case MessageBoxIcon.Stop

                Case MessageBoxIcon.Information

                Case MessageBoxIcon.Question

            End Select



        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".SetIconImage", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
        End Try
    End Sub

    Private Sub On_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles myOKButton.Click, myYESButton.Click, myNOButton.Click, myCancelButton.Click
        Try

            If sender Is Me.myOKButton Then
                Me.DialogResult = Windows.Forms.DialogResult.OK
            ElseIf sender Is Me.myCancelButton Then
                Me.DialogResult = Windows.Forms.DialogResult.Cancel
            ElseIf sender Is Me.myYESButton Then
                Me.DialogResult = Windows.Forms.DialogResult.Yes
            ElseIf sender Is Me.myNOButton Then
                Me.DialogResult = Windows.Forms.DialogResult.No
            End If

            Me.Close()

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".On_Button_Click", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
        End Try

    End Sub

    Private Sub BsMessageLabel_Resize(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsMessageLabel.Resize
        Try
            Me.Height = Me.BsMessageLabel.Height + 100
        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".BsMessageLabel_Resize", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
        End Try
    End Sub
End Class
