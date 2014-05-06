Option Strict On
Option Explicit On

Imports Biosystems.Ax00.Controls.UserControls

''' Created by XBC 01/06/2012
''' This data structure refers to each Sensor included in one of the Cells in 
''' the Monitor Panel. This structure is susable for getting or setting information 
''' about the Sensors included.

Public Class ControlStruct

#Region "Attributes"
    Private MonitorControlAttr As BSMonitorControlBase  'Control that represents the Sensor	BsMonitorControlBase
    Private ColumnIndexAttr As Integer                   'Column index of the cell of the Sensor
    Private RowIndexAttr As Integer                      'Row index of the cell off the Sensor
    Private SensorIdAttr As String                       'Identifier of the Sensor
    Private MinimumLimitAttr As Single                   'Minimum limit of the sensor for notifications
    Private MaximumLimitAttr As Single                   'Maximum limit of the sensor for notifications
#End Region

#Region "Properties"
    Public Property MonitorControl() As BSMonitorControlBase
        Get
            Return MonitorControlAttr
        End Get
        Set(ByVal value As BSMonitorControlBase)
            MonitorControlAttr = value
        End Set
    End Property
    Public Property ColumnIndex() As Integer
        Get
            Return ColumnIndexAttr
        End Get
        Set(ByVal value As Integer)
            ColumnIndexAttr = value
        End Set
    End Property
    Public Property RowIndex() As Integer
        Get
            Return RowIndexAttr
        End Get
        Set(ByVal value As Integer)
            RowIndexAttr = value
        End Set
    End Property
    Public Property SensorId() As String
        Get
            Return SensorIdAttr
        End Get
        Set(ByVal value As String)
            SensorIdAttr = value
        End Set
    End Property
    Public Property MinimumLimit() As Single
        Get
            Return MinimumLimitAttr
        End Get
        Set(ByVal value As Single)
            MinimumLimitAttr = value
        End Set
    End Property
    Public Property MaximumLimit() As Single
        Get
            Return MaximumLimitAttr
        End Get
        Set(ByVal value As Single)
            MaximumLimitAttr = value
        End Set
    End Property

#End Region

#Region "Constructor"

    'constructor of the data sstructure
    Public Sub New(ByRef pControl As BSMonitorControlBase, _
                   ByVal pSensorId As String, _
                   ByVal pColIndex As Integer, _
                   ByVal pRowIndex As Integer, _
                   Optional ByVal pMin As Single = 0, _
                   Optional ByVal pMax As Single = 100)
        Try
            Me.MonitorControl = pControl
            Me.SensorId = pSensorId
            Me.ColumnIndex = pColIndex
            Me.RowIndex = pRowIndex
            Me.MinimumLimit = pMin
            Me.MaximumLimit = pMax

        Catch ex As Exception
            Throw ex
        End Try
    End Sub
#End Region

End Class
