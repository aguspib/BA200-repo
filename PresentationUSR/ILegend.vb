Option Explicit On


'Imports DevExpress.XtraGrid
'Imports DevExpress.Data
'Imports DevExpress.XtraGrid.Views.Grid
'Imports DevExpress.XtraGrid.Columns

Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.BL
'Imports System.IO



Public Class ILegend

#Region "Declarations"

    'Private ROUTINES As Byte() = Nothing
    'Private EQ_SENT_REP As Byte() = Nothing
    'Private PRINT As Byte() = Nothing
    'Private TEST As Byte() = Nothing
    'Private LEFT As Byte() = Nothing
    Private myLanguage As String
#End Region

#Region "Attributes"
    Private CaptionLabelAttribute As String
    Private SourceFormAttribute As String
    Private ParentMDIAttribute As Point
#End Region

#Region "Properties"
    Public WriteOnly Property CaptionLabel() As String
        Set(ByVal value As String)
            CaptionLabelAttribute = value
        End Set
    End Property

    Public WriteOnly Property SourceForm() As String
        Set(ByVal value As String)
            SourceFormAttribute = value
        End Set
    End Property

    Public WriteOnly Property ParentMDI() As Point
        Set(ByVal value As Point)
            ParentMDIAttribute = value
        End Set
    End Property

#End Region


    Private Sub LoadLegend()
        Try
            'Cursor = Cursors.WaitCursor

            Dim preloadedDataConfig As New PreloadedMasterDataDelegate
            Dim ResultData As GlobalDataTO

            ResultData = preloadedDataConfig.GetLegendList(Nothing, SourceFormAttribute)

            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate
            Dim myLegendDS As LegendDS
            myLegendDS = DirectCast(ResultData.SetDatos, LegendDS)

            For Each row As LegendDS.tfmwLegendRow In myLegendDS.tfmwLegend.Rows
                row.Picture = preloadedDataConfig.GetIconImage(row.FileName)
                row.Description = row.Description.ToString.Trim

                'DL 13/11/2012
                Select Case row.Group

                    Case "ACTIONS"
                        row.Group = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_Actions", myLanguage).ToUpper.ToString
                    Case "ICONS"
                        row.Group = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Icons", myLanguage).ToUpper.ToString
                    Case "STATUS"
                        row.Group = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_MainMDI_Status", myLanguage).ToUpper.ToString

                    Case "ACTIONS IN LIST AREA"
                        row.Group = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_ACTION_LISTAREA", myLanguage).ToUpper.ToString

                    Case "ACTIONS IN ROTOR AREA"
                        row.Group = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_ACTION_ROTORAREA", myLanguage).ToUpper.ToString

                    Case "REAGENTS STATUS"
                        row.Group = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LGD_RotBReagent", myLanguage).ToUpper.ToString

                    Case "SAMPLES STATUS"
                        row.Group = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SAMPLESSTATUS", myLanguage).ToUpper.ToString

                End Select


                'DL 13/11/2012

            Next row

            myLegendDS.AcceptChanges()

            bsSamplesListDataGridView.DataSource = myLegendDS.tfmwLegend

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".LoadLegend ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".LoadLegend ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")

            'Finally
            ' Cursor = Cursors.Default

        End Try
    End Sub

    Private Sub PrepareButtons()
        Try

            Dim auxIconName As String = String.Empty
            Dim iconPath As String = MyBase.IconsPath


            auxIconName = GetIconName("CANCEL")
            If (auxIconName <> "") Then
                ExitButton.Image = Image.FromFile(iconPath & auxIconName)
            End If

            'Dim imgStream As MemoryStream = New MemoryStream()
            'Dim img As Image

            'img = Image.FromFile("C:\David Luna\LeftClick.png")
            'img.Save(imgStream, System.Drawing.Imaging.ImageFormat.Png)

            'imgStream.Close()
            'LEFT = imgStream.ToArray()



        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".PrepareButtons ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", Me)

        End Try
    End Sub

    Private Sub ILegend_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyDown
        Try
            'Handle the escape key press depending the tab.
            If (e.KeyCode = Keys.Escape) Then
                'RH 04/07/2011 Escape key should do exactly the same operations as ExitButton_Click()
                ExitButton.PerformClick()
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " ILegend_KeyDown ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub



    Private Sub LegendForm_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        GetScreenLabels()
        PrepareButtons()
        LoadLegend()

        'DL 28/07/2011
        Dim myLocation As Point = IAx00MainMDI.Location ' ParentMDIAttribute '   IAx00MainMDI.Location
        Dim mySize As Size = IAx00MainMDI.Size

        Me.Location = New Point(myLocation.X + CInt((mySize.Width - Me.Width) / 2), myLocation.Y + CInt((mySize.Height - Me.Height) / 2))
        'END DL 28/07/2011
    End Sub

    ''' <summary>
    ''' Get texts in the current application language for all screen controls
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 12/03/2012
    ''' </remarks>
    Private Sub GetScreenLabels()
        Try
            'Get the current Language from the current Application Session
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate
            Dim currentLanguageGlobal As New GlobalBase

            myLanguage = GlobalBase.GetSessionInfo().ApplicationLanguage.Trim.ToString

            bsTitleLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "TITLE_LEGEND", myLanguage)

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".GetScreenLabels", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".GetScreenLabels", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
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

                Dim myLocation As Point = IAx00MainMDI.Location ' ParentMDIAttribute
                Dim mySize As Size = IAx00MainMDI.Size

                pos.x = myLocation.X + CInt((mySize.Width - Me.Width) / 2)
                pos.y = myLocation.Y + CInt((mySize.Height - Me.Height) / 2)
                Runtime.InteropServices.Marshal.StructureToPtr(pos, m.LParam, True)
            End If

            MyBase.WndProc(m)
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".WndProc ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".WndProc", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub ExitButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ExitButton.Click
        Me.Close()
    End Sub
End Class
