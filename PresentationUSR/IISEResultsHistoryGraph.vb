Option Strict On
Option Explicit On
Option Infer On
Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Global.GlobalEnumerates
Imports DevExpress.XtraCharts
Imports System.Globalization
Imports DevExpress.Utils

Public Class IISEResultsHistoryGraph

#Region " Declarations "
    ' Language
    Private currentLanguage As String
    Private myCultureInfo As CultureInfo
    Private myElectrodesFilter As IISEResultsHistory.ElectrodesFilter
    Private myDataSource As DataTable
    Private LocalPoint As Point

    Private loaded As Boolean = False
    Private mImageDict As Dictionary(Of String, Image)
    Private mTextDict As Dictionary(Of String, String)
#End Region

#Region " Attributes "
#End Region

#Region " Properties "

#End Region

#Region "Constructor"
#End Region

#Region " Public Methods "
    Public Sub SetData(ByVal pElectrodesFilter As IISEResultsHistory.ElectrodesFilter, ByVal pDatasource As DataTable)
        SetElectrodeFilter(pElectrodesFilter)
        myDataSource = pDatasource
        myCultureInfo = My.Computer.Info.InstalledUICulture
    End Sub
#End Region

#Region "Private Methods"
    Private Sub SetElectrodeFilter(ByVal filter As IISEResultsHistory.ElectrodesFilter)
        myElectrodesFilter = filter

        bsElectrodeNaCheck.Checked = filter.electrodeNa
        bsElectrodeKCheck.Checked = filter.electrodeK
        bsElectrodeClCheck.Checked = filter.electrodeCl
        bsElectrodeLiCheck.Checked = filter.electrodeLi
    End Sub

#Region " Initializations "
    Private Function GetImage(ByVal pKey As String) As Image
        If Not mImageDict.ContainsKey(pKey) Then
            SetImageToDictionary(pKey)
        End If
        If Not mImageDict.ContainsKey(pKey) Then
            Return New Bitmap(16, 16)
        End If

        Return mImageDict.Item(pKey)
    End Function
    Private Sub SetImageToDictionary(ByVal pKey As String)
        Dim auxIconName As String = ""
        Dim iconPath As String = MyBase.IconsPath

        auxIconName = GetIconName(pKey)
        If Not String.IsNullOrEmpty(auxIconName) Then
            If mImageDict.ContainsKey(pKey) Then
                mImageDict.Item(pKey) = ImageUtilities.ImageFromFile(iconPath & auxIconName)
            Else
                mImageDict.Add(pKey, ImageUtilities.ImageFromFile(iconPath & auxIconName))
            End If
        End If

    End Sub

    Private Function GetText(ByVal pKey As String) As String
        If Not mTextDict.ContainsKey(pKey) Then
            SetTextToDictionary(pKey)
        End If
        If Not mTextDict.ContainsKey(pKey) Then
            Return ""
        End If
        Return mTextDict.Item(pKey)
    End Function
    Private Sub SetTextToDictionary(ByVal pKey As String)
        Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate
        Dim text As String = myMultiLangResourcesDelegate.GetResourceText(Nothing, pKey, currentLanguage)
        If String.IsNullOrEmpty(text) Then text = "*" & pKey
        If mTextDict.ContainsKey(pKey) Then
            mTextDict.Item(pKey) = text
        Else
            mTextDict.Add(pKey, text)
        End If
    End Sub

    ''' <summary>
    ''' Get texts in the current application language for all screen controls
    ''' </summary>
    ''' <remarks>
    ''' Created by:  JB 31/07/2012
    ''' Modified by: XB 05/09/2014 - Take the ISE test names from the Name field on tparISETests table  instead of a multilanguage label - BA-1902
    ''' </remarks>
    Private Sub GetScreenLabels()
        Try
            Me.bsTitleLabel.Text = GetText("TITLE_ISE_HistoricalCalibGraph")

            ' XB 05/09/2014 - BA-1902
            'bsElectrodeNaCheck.Text = GetText("LBL_Sodium")
            'bsElectrodeKCheck.Text = GetText("LBL_Potassium")
            'bsElectrodeClCheck.Text = GetText("LBL_Chlorine")
            'bsElectrodeLiCheck.Text = GetText("LBL_Lithium")

            'bsLegend.Text = GetText("LBL_Legend")
            'bsElectrodeNaLabel.Text = GetText("LBL_Sodium")
            'bsElectrodeKLabel.Text = GetText("LBL_Potassium")
            'bsElectrodeClLabel.Text = GetText("LBL_Chlorine")
            'bsElectrodeLiLabel.Text = GetText("LBL_Lithium")

            Dim ISETestList As New ISETestsDelegate
            bsElectrodeNaCheck.Text = ISETestList.GetName(Nothing, ISE_Tests.Na)
            bsElectrodeKCheck.Text = ISETestList.GetName(Nothing, ISE_Tests.K)
            bsElectrodeClCheck.Text = ISETestList.GetName(Nothing, ISE_Tests.Cl)
            bsElectrodeLiCheck.Text = ISETestList.GetName(Nothing, ISE_Tests.Li)

            bsLegend.Text = GetText("LBL_Legend")
            bsElectrodeNaLabel.Text = ISETestList.GetName(Nothing, ISE_Tests.Na)
            bsElectrodeKLabel.Text = ISETestList.GetName(Nothing, ISE_Tests.K)
            bsElectrodeClLabel.Text = ISETestList.GetName(Nothing, ISE_Tests.Cl)
            bsElectrodeLiLabel.Text = ISETestList.GetName(Nothing, ISE_Tests.Li)
            ' XB 05/09/2014 - BA-1902

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".GetScreenLabels", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".GetScreenLabels", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Loads the icons and tooltips used for painting the buttons
    ''' </summary>
    ''' <remarks>
    ''' Created by: JB 31/07/2012
    ''' </remarks>
    Private Sub PrepareButtons()
        Dim myToolTipsControl As New Windows.Forms.ToolTip
        Try
            'EXIT Button
            bsExitButton.Image = GetImage("CANCEL")
            myToolTipsControl.SetToolTip(bsExitButton, GetText("BTN_CloseScreen"))

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".PrepareButtons", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareButtons", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Adds a serie to the graph passed as parameter
    ''' </summary>
    ''' <param name="pGraph">Graph that will contain the new serie</param>
    ''' <param name="serieName">name of the serie to be added to the chart</param>
    ''' <param name="viewType">type of the view desired to this serie</param>
    ''' <param name="thisColor">color of the line of the serie</param>
    ''' <remarks></remarks>
    Private Sub AddSerieToGraph(ByVal pGraph As ChartControl, ByVal serieName As String, ByVal viewType As ViewType, ByVal thisColor As Color)
        pGraph.Series.Add(serieName, viewType)

        With pGraph.Series(serieName)
            .ShowInLegend = True
            .LabelsVisibility = DefaultBoolean.False
            .ArgumentScaleType = ScaleType.Qualitative
            .View.Color = thisColor
        End With

        CType(pGraph.Series(serieName).View, LineSeriesView).MarkerVisibility = DevExpress.Utils.DefaultBoolean.True
    End Sub


    ''' <summary>
    ''' Initializes the Graph
    ''' </summary>
    ''' <remarks>
    ''' Created by JB 31/07/2012
    ''' </remarks>
    Private Sub InitializeGraph(ByVal pGraph As ChartControl)
        Try
            pGraph.ClearCache()
            pGraph.Series.Clear()
            pGraph.Legend.Visibility = DefaultBoolean.False
            pGraph.SeriesTemplate.ValueScaleType = ScaleType.Numerical
            pGraph.BackColor = Color.White
            pGraph.AppearanceName = "Light"

            pGraph.CrosshairEnabled = DevExpress.Utils.DefaultBoolean.False
            pGraph.RuntimeHitTesting = True
            pGraph.SeriesTemplate.ArgumentScaleType = ScaleType.Qualitative

            'Na+
            AddSerieToGraph(pGraph, "MeanNa", ViewType.Line, Color.Green)
            'K+
            AddSerieToGraph(pGraph, "MeanK", ViewType.Line, Color.Blue)
            'Cl-
            AddSerieToGraph(pGraph, "MeanCl", ViewType.Line, Color.DarkOrange)
            'Li+
            AddSerieToGraph(pGraph, "MeanLi", ViewType.Line, Color.DarkViolet)

            Dim myDiagram As New XYDiagram

            myDiagram = CType(pGraph.Diagram, XYDiagram)

            myDiagram.AxisY.GridLines.Visible = True
            myDiagram.AxisX.GridLines.Visible = False

            myDiagram.AxisY.ConstantLines.Clear()
            myDiagram.AxisX.ConstantLines.Clear()

            'Set margins
            myDiagram.Margins.Right = 5

            'Set the Title for each axis
            myDiagram.AxisX.Title.Visibility = DefaultBoolean.True
            myDiagram.AxisX.Title.Antialiasing = False
            myDiagram.AxisX.Title.TextColor = Color.Black
            myDiagram.AxisX.Title.Alignment = StringAlignment.Center
            myDiagram.AxisX.Title.Font = New Font("Verdana", 8.25, FontStyle.Regular)
            myDiagram.AxisX.Title.Text = GetText("LBL_Date")

            myDiagram.AxisX.Label.Visible = False
            'myDiagram.AxisX.Label.Angle = -30
            'myDiagram.AxisX.Label.Antialiasing = True


            myDiagram.AxisY.Title.Visibility = DefaultBoolean.True
            myDiagram.AxisY.Title.Antialiasing = False
            myDiagram.AxisY.Title.TextColor = Color.Black
            myDiagram.AxisY.Title.Alignment = StringAlignment.Center
            myDiagram.AxisY.Title.Font = New Font("Verdana", 8.25, FontStyle.Regular)
            myDiagram.AxisY.Title.Text = GetText("LBL_Results")


        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".InitializeGraph ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".InitializeGraph ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Initialize all screen controls
    ''' </summary>
    ''' <remarks>
    ''' Created by: JB 31/07/2012
    ''' </remarks>
    Private Sub InitializeScreen()
        Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate
        Try
            mImageDict = New Dictionary(Of String, Image)()
            mTextDict = New Dictionary(Of String, String)()

            'Get the current Language from the current Application Session
            'Dim currentLanguageGlobal As New GlobalBase
            currentLanguage = GlobalBase.GetSessionInfo().ApplicationLanguage.Trim.ToString()

            GetScreenLabels()
            PrepareButtons()

            InitializeGraph(bsISEResultChartControl)
            SetDataToGraph(bsISEResultChartControl, myDataSource)

            ShowGraphForSelectedElectrodes(bsISEResultChartControl)

            Me.loaded = True
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".InitializeScreen ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".InitializeScreen ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub
#End Region

#Region " Graph "
    ''' <summary>
    ''' Sets data to Graph
    ''' </summary>
    ''' <param name="pGraph"></param>
    ''' <param name="pData"></param>
    ''' <remarks>
    ''' Created by:  JB 31/07/2012
    ''' Modified by: XB 05/09/2014 - Take the ISE test names from the Name field on tparISETests table  instead of a multilanguage label - BA-1902
    ''' </remarks>
    Private Sub SetDataToGraph(ByVal pGraph As ChartControl, ByVal pData As DataTable)
        Try
            'Hide series
            pGraph.Series("MeanNa").Visible = False
            pGraph.Series("MeanK").Visible = False
            pGraph.Series("MeanCl").Visible = False
            pGraph.Series("MeanLi").Visible = False

            'Clear series
            pGraph.Series("MeanNa").Points.Clear()
            pGraph.Series("MeanK").Points.Clear()
            pGraph.Series("MeanCl").Points.Clear()
            pGraph.Series("MeanLi").Points.Clear()

            'Sort the data to show
            Dim rows = From r In pData Order By CDate(r("CalibrationDate")) Ascending

            For Each row As DataRow In rows 'pData.Rows

                ' XB 05/09/2014 - BA-1902
                'Na+
                'pGraph.Series("MeanNa").Points.Add(New SeriesPoint(CDate(row("CalibrationDate")), row("MeanNa")))
                'pGraph.Series("MeanNa").Points(pGraph.Series("MeanNa").Points.Count - 1).Tag = GetText("LBL_Sodium") & ": " & _
                '                                                                               row("ResultsNa").ToString.Replace(vbCrLf, "; ") & vbCrLf & _
                '                                                                               CDate(row("CalibrationDate")).ToString(myCultureInfo.DateTimeFormat.ShortDatePattern)

                ''K+
                'pGraph.Series("MeanK").Points.Add(New SeriesPoint(CDate(row("CalibrationDate")), row("MeanK")))
                'pGraph.Series("MeanK").Points(pGraph.Series("MeanK").Points.Count - 1).Tag = GetText("LBL_Potassium") & ": " & _
                '                                                                             row("ResultsK").ToString.Replace(vbCrLf, "; ") & vbCrLf & _
                '                                                                             CDate(row("CalibrationDate")).ToString(myCultureInfo.DateTimeFormat.ShortDatePattern)

                ''Cl-
                'pGraph.Series("MeanCl").Points.Add(New SeriesPoint(CDate(row("CalibrationDate")), row("MeanCl")))
                'pGraph.Series("MeanCl").Points(pGraph.Series("MeanCl").Points.Count - 1).Tag = GetText("LBL_Chlorine") & ": " & _
                '                                                                               row("ResultsCl").ToString.Replace(vbCrLf, "; ") & vbCrLf & _
                '                                                                               CDate(row("CalibrationDate")).ToString(myCultureInfo.DateTimeFormat.ShortDatePattern)

                ''Li+
                'pGraph.Series("MeanLi").Points.Add(New SeriesPoint(CDate(row("CalibrationDate")), row("MeanLi")))
                'pGraph.Series("MeanLi").Points(pGraph.Series("MeanLi").Points.Count - 1).Tag = GetText("LBL_Lithium") & ": " & _
                '                                                                               row("ResultsLi").ToString.Replace(vbCrLf, "; ") & vbCrLf & _
                '                                                                               CDate(row("CalibrationDate")).ToString(myCultureInfo.DateTimeFormat.ShortDatePattern)

                Dim ISETestList As New ISETestsDelegate
                'Na+
                pGraph.Series("MeanNa").Points.Add(New SeriesPoint(CDate(row("CalibrationDate")), row("MeanNa")))
                pGraph.Series("MeanNa").Points(pGraph.Series("MeanNa").Points.Count - 1).Tag = ISETestList.GetName(Nothing, ISE_Tests.Na) & ": " & _
                                                                                               row("ResultsNa").ToString.Replace(vbCrLf, "; ") & vbCrLf & _
                                                                                               CDate(row("CalibrationDate")).ToString(myCultureInfo.DateTimeFormat.ShortDatePattern)

                'K+
                pGraph.Series("MeanK").Points.Add(New SeriesPoint(CDate(row("CalibrationDate")), row("MeanK")))
                pGraph.Series("MeanK").Points(pGraph.Series("MeanK").Points.Count - 1).Tag = ISETestList.GetName(Nothing, ISE_Tests.K) & ": " & _
                                                                                             row("ResultsK").ToString.Replace(vbCrLf, "; ") & vbCrLf & _
                                                                                             CDate(row("CalibrationDate")).ToString(myCultureInfo.DateTimeFormat.ShortDatePattern)

                'Cl-
                pGraph.Series("MeanCl").Points.Add(New SeriesPoint(CDate(row("CalibrationDate")), row("MeanCl")))
                pGraph.Series("MeanCl").Points(pGraph.Series("MeanCl").Points.Count - 1).Tag = ISETestList.GetName(Nothing, ISE_Tests.Cl) & ": " & _
                                                                                               row("ResultsCl").ToString.Replace(vbCrLf, "; ") & vbCrLf & _
                                                                                               CDate(row("CalibrationDate")).ToString(myCultureInfo.DateTimeFormat.ShortDatePattern)

                'Li+
                pGraph.Series("MeanLi").Points.Add(New SeriesPoint(CDate(row("CalibrationDate")), row("MeanLi")))
                pGraph.Series("MeanLi").Points(pGraph.Series("MeanLi").Points.Count - 1).Tag = ISETestList.GetName(Nothing, ISE_Tests.Li) & ": " & _
                                                                                               row("ResultsLi").ToString.Replace(vbCrLf, "; ") & vbCrLf & _
                                                                                               CDate(row("CalibrationDate")).ToString(myCultureInfo.DateTimeFormat.ShortDatePattern)
                ' XB 05/09/2014 - BA-1902

            Next
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".SetDataToGraph ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".SetDataToGraph ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub ShowGraphForSelectedElectrodes(ByVal pGraph As ChartControl)
        pGraph.Series("MeanNa").Visible = bsElectrodeNaCheck.Checked
        pGraph.Series("MeanK").Visible = bsElectrodeKCheck.Checked
        pGraph.Series("MeanCl").Visible = bsElectrodeClCheck.Checked
        pGraph.Series("MeanLi").Visible = bsElectrodeLiCheck.Checked

        AdaptAxisToVisibleData(pGraph)
    End Sub

    Private Sub AdaptAxisToVisibleData(ByVal pGraph As ChartControl)
        Try
            Const PERCENTAGE As Single = 0.15

            Dim myDiagram As New XYDiagram

            myDiagram = CType(pGraph.Diagram, XYDiagram)
            If myDiagram Is Nothing Then Exit Sub

            Dim minY As Single? = Nothing
            Dim maxY As Single? = Nothing
            Dim aux As Single
            For Each serie As Series In pGraph.Series
                If serie.Visible Then
                    aux = CSng((From p In serie.Points Select p.UserValues(0)).Min)
                    If Not minY.HasValue OrElse aux < minY Then minY = aux

                    aux = CSng((From p In serie.Points Select p.UserValues(0)).Max)
                    If Not maxY.HasValue OrElse aux > maxY Then maxY = aux
                End If
            Next

            'Set the Min and Max Range for Y
            Dim yExtra As Single = 1
            If maxY.HasValue AndAlso minY.HasValue Then
                yExtra += (maxY.Value - minY.Value) * PERCENTAGE
                myDiagram.AxisY.WholeRange.SetMinMaxValues(minY - yExtra, maxY + yExtra)
                myDiagram.AxisY.VisualRange.SetMinMaxValues(minY - yExtra, maxY + yExtra)
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".AdaptAxisToVisibleData ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".AdaptAxisToVisibleData ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub
#End Region

#End Region

#Region " Events "
#Region " Screen Events "
    Private Sub IISEResultsHistoryGraph_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles MyBase.KeyDown
        Try
            If (e.KeyCode = Keys.Escape) Then bsExitButton.PerformClick()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".IISEResultsHistoryGraph_KeyDown ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".IISEResultsHistoryGraph_KeyDown ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub IISEResultsHistoryGraph_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try
            If Me.DesignMode Then Exit Sub

            InitializeScreen()

            'ResetBorder()
            'Application.DoEvents()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".IISEResultsHistoryGraph ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".IISEResultsHistoryGraph ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Not allow move form and mantain the center location in center parent
    ''' </summary>
    Protected Overrides Sub WndProc(ByRef m As Message)
        Try
            If (m.Msg = WM_WINDOWPOSCHANGING) Then
                Dim pos As WINDOWPOS = DirectCast(Runtime.InteropServices.Marshal.PtrToStructure(m.LParam, GetType(WINDOWPOS)), WINDOWPOS)

                'Dim mySize As Size = IAx00MainMDI.Size
                'Dim myLocation As Point = IAx00MainMDI.Location
                'If (Not Me.MdiParent Is Nothing) Then
                '    mySize = Me.Parent.Size
                '    myLocation = Me.Parent.Location
                'End If

                Dim myLocation As Point = IAx00MainMDI.Location
                Dim mySize As Size = IAx00MainMDI.Size

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
#End Region

#Region " Button Events "
    Private Sub ExitButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsExitButton.Click
        Me.Close()
    End Sub
#End Region

#Region " Check Events "
    Private Sub ElectrodeChecks_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsElectrodeNaCheck.CheckedChanged, _
                                                                                                                   bsElectrodeKCheck.CheckedChanged, _
                                                                                                                   bsElectrodeClCheck.CheckedChanged, _
                                                                                                                   bsElectrodeLiCheck.CheckedChanged
        If Not loaded Then Exit Sub
        ShowGraphForSelectedElectrodes(bsISEResultChartControl)
    End Sub
#End Region

#Region " Graph Events "
    Private Sub bsISEResultChartControl_ObjectHotTracked(ByVal sender As System.Object, ByVal e As DevExpress.XtraCharts.HotTrackEventArgs) Handles bsISEResultChartControl.ObjectHotTracked
        Try
            Dim myPoint As SeriesPoint = TryCast(e.AdditionalObject, SeriesPoint)
            If (Not myPoint Is Nothing) Then
                bsToolTipController.ToolTipLocation = DevExpress.Utils.ToolTipLocation.LeftTop
                bsToolTipController.ShowHint(myPoint.Tag.ToString, LocalPoint)
            Else
                bsToolTipController.HideHint()
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".bsISEResultChartControl_ObjectHotTracked ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsISEResultChartControl_ObjectHotTracked ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub bsISEResultChartControl_MouseMove(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles bsISEResultChartControl.MouseMove
        Try
            LocalPoint = bsISEResultChartControl.PointToScreen(New Point(e.X, e.Y))
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".bsISEResultChartControl_MouseMove ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsISEResultChartControl_MouseMove ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

#End Region
#End Region

End Class