Option Strict On
Option Explicit On

Imports System.Windows.Forms
Imports Biosystems.Ax00.Controls.UserControls
Imports Biosystems.Ax00.Global


''' <summary>
''' User Control for displaying the monitor sensors values during
''' all the execution of the Service Software Application
''' </summary>
''' <remarks>Created by SGM 15/04/2011</remarks>
Public Class BSMonitorPanel




#Region "declarations"

#End Region

    ' XBC 01/06/2012 - replace by Class
    '#Region "Private Structures"

    '    ''' <summary>
    '    ''' This data structure refers to each Sensor included in one of the Cells in 
    '    ''' the Monitor Panel. This structure is susable for getting or setting information 
    '    ''' about the Sensors included.
    '    ''' </summary>
    '    ''' <remarks>Created by SGM 15/04/2011</remarks>
    '    Private Structure ControlStruct
    '        Public MonitorControl As BSMonitorControlBase   'Control that represents the Sensor	BsMonitorControlBase
    '        Public ColumnIndex As Integer                   'Column index of the cell of the Sensor
    '        Public RowIndex As Integer                      'Row index of the cell off the Sensor
    '        Public SensorId As String                       'Identifier of the Sensor
    '        Public MinimumLimit As Single                   'Minimum limit of the sensor for notifications
    '        Public MaximumLimit As Single                   'Maximum limit of the sensor for notifications

    '        'constructor of the data sstructure
    '        Public Sub New(ByRef pControl As BSMonitorControlBase, _
    '                       ByVal pSensorId As String, _
    '                       ByVal pColIndex As Integer, _
    '                       ByVal pRowIndex As Integer, _
    '                       Optional ByVal pMin As Single = 0, _
    '                       Optional ByVal pMax As Single = 100)
    '            Try
    '                Me.MonitorControl = pControl
    '                Me.SensorId = pSensorId
    '                Me.ColumnIndex = pColIndex
    '                Me.RowIndex = pRowIndex
    '                Me.MinimumLimit = pMin
    '                Me.MaximumLimit = pMax

    '            Catch ex As Exception
    '                Throw ex
    '            End Try
    '        End Sub
    '    End Structure
    '#End Region

#Region "Public Properties"

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks>Created by SGM 15/04/2011</remarks>
    Public Property ColumnCount() As Integer
        Get
            Return ColumnCountAttr
        End Get
        Set(ByVal value As Integer)
            If value < 1 Then value = 1
            ColumnCountAttr = value
            Me.LayoutPanel.ColumnCount = value
            InitializePanel()
        End Set
    End Property

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks>Created by SGM 15/04/2011</remarks>
    Public Property RowCount() As Integer
        Get
            Return RowCountAttr
        End Get
        Set(ByVal value As Integer)
            If value < 1 Then value = 1
            RowCountAttr = value
            Me.LayoutPanel.RowCount = value
            InitializePanel()
        End Set
    End Property

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks>Created by SGM 15/04/2011</remarks>
    Public Property GridStyle() As TableLayoutPanelCellBorderStyle
        Get
            Return GridStyleAttr
        End Get
        Set(ByVal value As TableLayoutPanelCellBorderStyle)
            Me.LayoutPanel.CellBorderStyle = value
            GridStyleAttr = value
            InitializePanel()
        End Set
    End Property

#End Region

#Region "Private Properties"

    Private Property ControlList() As List(Of ControlStruct)
        Get
            Return ControlListAttr
        End Get
        Set(ByVal value As List(Of ControlStruct))
            ControlListAttr = value
        End Set
    End Property
#End Region

#Region "Attributes"
    Private ColumnCountAttr As Integer = 1
    Private RowCountAttr As Integer = 1
    Private GridStyleAttr As TableLayoutPanelCellBorderStyle = TableLayoutPanelCellBorderStyle.None

    Private ControlListAttr As New List(Of ControlStruct)
#End Region

#Region "Private Event Handlers"
    'Private Sub OnSensorDataRequested(ByVal pSensor As ServiceSensor) Handles SensorsListAttr.NewSensorDataRequested
    '    Try

    '        RaiseEvent NewSensorDataRequested(pSensor)

    '    Catch ex As Exception
    '        Throw ex
    '    End Try
    'End Sub

    'Private Sub OnSensorTimeout(ByVal pSensor As ServiceSensor) Handles SensorsListAttr.SensorTimeout
    '    Try

    '        RaiseEvent SensorDataRequestedTimeout(pSensor)

    '    Catch ex As Exception
    '        Throw ex
    '    End Try
    'End Sub
#End Region

#Region "Public Methods"

    ''' <summary>
    ''' Removes all the Sensors from the panel
    ''' </summary>
    ''' <remarks>Created by SGM 15/04/2011</remarks>
    Public Sub Clear()
        Try
            Me.LayoutPanel.Controls.Clear()
            Me.ControlList.Clear()
        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    ''' <summary>
    ''' Sets the specified Control in the specified row and column
    ''' </summary>
    ''' <param name="pControl"></param>
    ''' <param name="pSensorId"></param>
    ''' <param name="pColIndex"></param>
    ''' <param name="pRowIndex"></param>
    ''' <param name="pVisible"></param>
    ''' <param name="pMin"></param>
    ''' <param name="pMax"></param>
    ''' <returns></returns>
    ''' <remarks>Created by SGM 15/04/2011</remarks>
    Public Function AddControl(ByRef pControl As BSMonitorControlBase, _
                               ByVal pSensorId As String, _
                          ByVal pColIndex As Integer, _
                          ByVal pRowIndex As Integer, _
                          Optional ByVal pVisible As Boolean = True, _
                       Optional ByVal pMin As Single = 0, _
                       Optional ByVal pMax As Single = 100) As GlobalDataTO

        Dim myGlobal As New GlobalDataTO

        Try

            If pControl IsNot Nothing Then

                If pControl.TitleText.Length = 0 Then
                    pControl.TitleHeight = 0
                End If

                pControl.Margin = New Padding(0, 0, 0, 0)
                pControl.Padding = New Padding(0, 0, 0, 0)

                If pColIndex >= 0 And pRowIndex >= 0 Then

                    If pColIndex < Me.LayoutPanel.ColumnCount And pRowIndex < Me.LayoutPanel.RowCount Then

                        pControl.Dock = DockStyle.Fill
                        For C As Integer = 0 To Me.LayoutPanel.ColumnCount - 1
                            Dim isAdded As Boolean = False
                            For R As Integer = 0 To Me.LayoutPanel.RowCount - 1
                                If C = pColIndex And R = pRowIndex Then
                                    Me.LayoutPanel.Controls.Add(pControl, C, R)
                                    isAdded = True
                                    Exit For
                                End If
                            Next
                            If isAdded Then
                                Dim myControlStruct As New ControlStruct(pControl, pSensorId, pColIndex, pRowIndex, pMin, pMax)
                                MyClass.ControlList.Add(myControlStruct)
                                pControl.RefreshControl()
                                pControl.Focus()
                                Exit For
                            End If
                        Next

                    End If

                Else
                    myGlobal.HasError = True
                End If


            End If

        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myGlobal.ErrorMessage = ex.Message
        End Try

        Return myGlobal

    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="pSensorId"></param>
    ''' <param name="pValue"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' Created by SGM 15/04/2011
    ''' Modified by XB 04/02/2013 - Upper conversions redundants because the value is already in UpperCase must delete to avoid Regional Settings problems (Bugs tracking #1112)
    ''' </remarks>
    Public Function RefreshSensorValue(ByVal pSensorId As String, _
                                       ByVal pValue As Single, _
                                       ByVal pInfoReaded As Boolean, _
                                       Optional ByVal pTitle As String = "") As GlobalDataTO

        Dim myGlobal As New GlobalDataTO

        Try
            For Each C As ControlStruct In MyClass.ControlList
                'If C.SensorId.ToUpper.Trim = pSensorId.ToUpper.Trim Then
                If C.SensorId.Trim = pSensorId.Trim Then
                    If TypeOf (C.MonitorControl) Is BSMonitorLED Then
                        Dim myControlLED As BSMonitorLED = CType(C.MonitorControl, BSMonitorLED)

                        ' XBC 07/02/2012 - 0 is ON ; 1 is OFF - excepting CONNECTED LED !
                        'If pValue > 0 Then

                        If pSensorId = GlobalEnumerates.AnalyzerSensors.CONNECTED.ToString Then
                            pValue -= 1
                        End If

                        If pValue = 0 Then
                            myControlLED.CurrentStatus = BSMonitorControlBase.Status._ON
                        Else
                            myControlLED.CurrentStatus = BSMonitorControlBase.Status._OFF
                        End If
                        ' XBC 07/02/2012 - 0 is ON ; 1 is OFF

                    ElseIf TypeOf (C.MonitorControl) Is BSMonitorDigitLabel Then
                        Dim myControlDigitLabel As BSMonitorDigitLabel = CType(C.MonitorControl, BSMonitorDigitLabel)
                        myControlDigitLabel.DigitValue = pValue

                        'limits
                        If pValue >= C.MinimumLimit And pValue <= C.MaximumLimit Then
                            myControlDigitLabel.CurrentStatus = BSMonitorControlBase.Status._ON
                        Else
                            myControlDigitLabel.CurrentStatus = BSMonitorControlBase.Status._OFF
                        End If

                    ElseIf TypeOf (C.MonitorControl) Is BSMonitorPercentBar Then
                        Dim myControlPercent As BSMonitorPercentBar = CType(C.MonitorControl, BSMonitorPercentBar)
                        myControlPercent.RealValue = pValue

                        'limits
                        If pValue >= C.MinimumLimit And pValue <= C.MaximumLimit Then
                            myControlPercent.CurrentStatus = BSMonitorControlBase.Status._ON
                        Else
                            myControlPercent.CurrentStatus = BSMonitorControlBase.Status._OFF
                        End If

                    End If

                    If pTitle.Length > 0 Then
                        C.MonitorControl.TitleText = pTitle
                    End If

                End If




            Next

            Application.DoEvents()

        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myGlobal.ErrorMessage = ex.Message
        End Try

        Return myGlobal

    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="pSensorId"></param>
    ''' <param name="pText"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' Created by SGM 15/04/2011
    ''' Modified by XB 04/02/2013 - Upper conversions redundants because the value is already in UpperCase must delete to avoid Regional Settings problems (Bugs tracking #1112)
    ''' </remarks>
    Public Function SetSensorText(ByVal pSensorId As String, ByVal pText As String) As GlobalDataTO

        Dim myGlobal As New GlobalDataTO

        Try
            For Each C As ControlStruct In MyClass.ControlList
                'If C.SensorId.ToUpper.Trim = pSensorId.ToUpper.Trim Then
                If C.SensorId.Trim = pSensorId.Trim Then
                    C.MonitorControl.TitleText = pText
                End If
            Next

        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myGlobal.ErrorMessage = ex.Message
        End Try

        Return myGlobal

    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>Created by SGM 15/04/2011</remarks>
    Public Sub DisableAllSensors()
        Try



            For Each C As ControlStruct In ControlList
                If C.MonitorControl IsNot Nothing Then
                    C.MonitorControl.Enabled = False
                    If C.SensorId <> GlobalEnumerates.AnalyzerSensors.CONNECTED.ToString Then
                        If TypeOf (C.MonitorControl) Is BSMonitorLED Then
                            Dim myControlLED As BSMonitorLED = CType(C.MonitorControl, BSMonitorLED)
                            myControlLED.CurrentStatus = BSMonitorControlBase.Status.DISABLED
                        ElseIf TypeOf (C.MonitorControl) Is BSMonitorDigitLabel Then
                            Dim myControlDigitLabel As BSMonitorDigitLabel = CType(C.MonitorControl, BSMonitorDigitLabel)
                            myControlDigitLabel.CurrentStatus = BSMonitorControlBase.Status.DISABLED

                        ElseIf TypeOf (C.MonitorControl) Is BSMonitorPercentBar Then
                            Dim myControlPercent As BSMonitorPercentBar = CType(C.MonitorControl, BSMonitorPercentBar)
                            myControlPercent.CurrentStatus = BSMonitorControlBase.Status.DISABLED
                        End If
                    End If

                End If
            Next


        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    Public Sub EnableAllSensors()
        Try
            For Each C As ControlStruct In ControlList
                If C.MonitorControl IsNot Nothing Then
                    C.MonitorControl.Enabled = True

                End If
            Next


        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="pSensorId"></param>
    ''' <param name="pMin"></param>
    ''' <param name="pMax"></param>
    ''' <remarks>
    ''' Created by SGM 31/05/2012
    ''' Modified by XB 04/02/2013 - Upper conversions redundants because the value is already in UpperCase must delete to avoid Regional Settings problems (Bugs tracking #1112)
    ''' </remarks>
    Public Sub SetSensorLimits(ByVal pSensorId As String, ByVal pMin As Single, ByVal pMax As Single)
        Try
            'Using Me

            '    Dim myOldControl As ControlStruct = Nothing
            '    Dim myNewControl As ControlStruct = Nothing

            For Each C As ControlStruct In ControlList
                'If C.SensorId.ToUpper = pSensorId.ToUpper Then
                If C.SensorId = pSensorId Then
                    'myNewControl = New ControlStruct(C.MonitorControl, C.SensorId, C.ColumnIndex, C.RowIndex, pMin, pMax)
                    'myOldControl = C

                    C.MinimumLimit = pMin
                    C.MaximumLimit = pMax

                    Exit For
                End If
            Next

            'If myOldControl.MonitorControl IsNot Nothing AndAlso myNewControl.MonitorControl IsNot Nothing Then
            '    ControlList.Remove(myOldControl)
            '    ControlList.Add(myNewControl)
            '    'myOldControl = Nothing
            'End If

            'End Using

        Catch ex As Exception
            Throw ex
        End Try
    End Sub



#End Region

#Region "Private Methods"

    ''' <summary>
    ''' Prepares the look of the Panel according to the contained elements
    ''' </summary>
    ''' <remarks>Created by SGM 15/04/2011</remarks>
    Private Sub InitializePanel()
        Try

            Me.LayoutPanel.ColumnStyles.Clear()
            For C As Integer = 0 To Me.LayoutPanel.ColumnCount - 1
                Me.LayoutPanel.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, CSng(100 / Me.LayoutPanel.ColumnCount)))
            Next
            Me.LayoutPanel.RowStyles.Clear()
            For C As Integer = 0 To Me.LayoutPanel.RowCount - 1
                Me.LayoutPanel.RowStyles.Add(New RowStyle(SizeType.Percent, CSng(100 / Me.LayoutPanel.RowCount)))
            Next

            Me.LayoutPanel.CellBorderStyle = Me.GridStyleAttr

        Catch ex As Exception
            Throw ex
        End Try
    End Sub


#End Region

#Region "Public Events"

#End Region

#Region "Private Event Handlers"


#End Region


End Class

