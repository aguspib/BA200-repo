Option Strict On
Option Explicit On

Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global

Namespace Biosystems.Ax00.FwScriptsManagement

    Public Class CycleCountDelegate
        Inherits BaseFwScriptDelegate

#Region "Declarations"

        Public CurrentCyclesCountItemsDT As UIRefreshDS.CyclesValuesChangedDataTable

#End Region


#Region "Constructor"
        Public Sub New(ByVal pAnalyzerID As String, ByVal pFwScriptsDelegate As SendFwScriptsDelegate)
            MyBase.New(pAnalyzerID)
            myFwScriptDelegate = pFwScriptsDelegate
            Me.FwCompatibleAttr = True
        End Sub
        Public Sub New()
            MyBase.New()
        End Sub
#End Region

#Region "Attributes"
        Private AnalyzerIdAttr As String = ""

        Private FwCompatibleAttr As Boolean

        Private ReadFWDoneAttr As Boolean
        Private ReadFWCPUDoneAttr As Boolean
        Private ReadFWBM1DoneAttr As Boolean
        Private ReadFWBR1DoneAttr As Boolean
        Private ReadFWBR2DoneAttr As Boolean
        Private ReadFWAG1DoneAttr As Boolean
        Private ReadFWAG2DoneAttr As Boolean
        Private ReadFWDR1DoneAttr As Boolean
        Private ReadFWDR2DoneAttr As Boolean
        Private ReadFWDM1DoneAttr As Boolean
        Private ReadFWRR1DoneAttr As Boolean
        Private ReadFWRM1DoneAttr As Boolean
        Private ReadFWGLFDoneAttr As Boolean
        Private ReadFWSF1DoneAttr As Boolean
        Private ReadFWJE1DoneAttr As Boolean

        Private ReadAnalyzerInfoDoneAttr As Boolean
        Private ReadBM1InfoDoneAttr As Boolean
        Private ReadBR1InfoDoneAttr As Boolean
        Private ReadBR2InfoDoneAttr As Boolean
        Private ReadAG1InfoDoneAttr As Boolean
        Private ReadAG2InfoDoneAttr As Boolean
        Private ReadDR1InfoDoneAttr As Boolean
        Private ReadDR2InfoDoneAttr As Boolean
        Private ReadDM1InfoDoneAttr As Boolean
        Private ReadRR1InfoDoneAttr As Boolean
        Private ReadRM1InfoDoneAttr As Boolean
        Private ReadGLFInfoDoneAttr As Boolean
        Private ReadJEXInfoDoneAttr As Boolean
        Private ReadSFXInfoDoneAttr As Boolean
        Private ReadCPUInfoDoneAttr As Boolean

        Private myLocalRefreshDS As New UIRefreshDS

        Private MaxPOLLHWAttr As Integer = 14
        Private NumPOLLHWAttr As Integer
#End Region

#Region "Properties"

        Public Property AnalyzerId() As String
            Get
                Return AnalyzerIdAttr
            End Get
            Set(ByVal value As String)
                AnalyzerIdAttr = value
            End Set
        End Property

        Public Property FwCompatible() As Boolean
            Get
                Return Me.FwCompatibleAttr
            End Get
            Set(ByVal value As Boolean)
                Me.FwCompatibleAttr = value
            End Set
        End Property

        Public Property ReadFWDone() As Boolean
            Get
                Return Me.ReadFWDoneAttr
            End Get
            Set(ByVal value As Boolean)
                Me.ReadFWDoneAttr = value

                If Not value Then
                    Me.ReadFWCPUDoneAttr = False
                    Me.ReadFWBM1DoneAttr = False
                    Me.ReadFWBR1DoneAttr = False
                    Me.ReadFWBR2DoneAttr = False
                    Me.ReadFWAG1DoneAttr = False
                    Me.ReadFWAG2DoneAttr = False
                    Me.ReadFWDR1DoneAttr = False
                    Me.ReadFWDR2DoneAttr = False
                    Me.ReadFWDM1DoneAttr = False
                    Me.ReadFWRR1DoneAttr = False
                    Me.ReadFWRM1DoneAttr = False
                    Me.ReadFWGLFDoneAttr = False
                    Me.ReadFWSF1DoneAttr = False
                    Me.ReadFWJE1DoneAttr = False
                End If
            End Set
        End Property

        Public Property ReadAnalyzerInfoDone() As Boolean
            Get
                Return Me.ReadAnalyzerInfoDoneAttr
            End Get
            Set(ByVal value As Boolean)
                Me.ReadAnalyzerInfoDoneAttr = value

                If Not value Then
                    Me.ReadBM1InfoDoneAttr = False
                    Me.ReadBR1InfoDoneAttr = False
                    Me.ReadBR2InfoDoneAttr = False
                    Me.ReadAG1InfoDoneAttr = False
                    Me.ReadAG2InfoDoneAttr = False
                    Me.ReadDR1InfoDoneAttr = False
                    Me.ReadDR2InfoDoneAttr = False
                    Me.ReadDM1InfoDoneAttr = False
                    Me.ReadRR1InfoDoneAttr = False
                    Me.ReadRM1InfoDoneAttr = False
                    Me.ReadGLFInfoDoneAttr = False
                    Me.ReadJEXInfoDoneAttr = False
                    Me.ReadSFXInfoDoneAttr = False
                    Me.ReadCPUInfoDoneAttr = False
                End If
            End Set
        End Property

        Public ReadOnly Property InfoFirmware(ByVal elementID As GlobalEnumerates.POLL_IDs) As UIRefreshDS.FirmwareValueChangedRow
            Get
                Dim returnValue As UIRefreshDS.FirmwareValueChangedRow = Nothing

                If Not myLocalRefreshDS Is Nothing Then
                    If myLocalRefreshDS.FirmwareValueChanged.Rows.Count > 0 Then
                        If myLocalRefreshDS.FirmwareValueChanged.Rows.Contains(elementID) Then
                            For Each myRow As UIRefreshDS.FirmwareValueChangedRow In myLocalRefreshDS.FirmwareValueChanged.Rows
                                If myRow.ElementID = elementID.ToString Then
                                    returnValue = myRow
                                    Exit For
                                End If
                            Next
                        End If
                    End If

                End If

                Return returnValue
            End Get
        End Property

        Public ReadOnly Property InfoCpu(ByVal elementID As GlobalEnumerates.CPU_ELEMENTS) As UIRefreshDS.CPUValueChangedRow
            Get
                Dim returnValue As UIRefreshDS.CPUValueChangedRow = Nothing

                If Not myLocalRefreshDS Is Nothing Then
                    If myLocalRefreshDS.CPUValueChanged.Rows.Count > 0 Then
                        If myLocalRefreshDS.CPUValueChanged.Rows.Contains(elementID) Then
                            For Each myRow As UIRefreshDS.CPUValueChangedRow In myLocalRefreshDS.CPUValueChanged.Rows
                                If myRow.ElementID = elementID.ToString Then
                                    returnValue = myRow
                                    Exit For
                                End If
                            Next
                        End If
                    End If

                End If

                Return returnValue
            End Get
        End Property

        Public ReadOnly Property InfoArm(ByVal elementID As GlobalEnumerates.POLL_IDs) As UIRefreshDS.ArmValueChangedRow
            Get
                Dim returnValue As UIRefreshDS.ArmValueChangedRow = Nothing

                If Not myLocalRefreshDS Is Nothing Then
                    If myLocalRefreshDS.ArmValueChanged.Rows.Count > 0 Then
                        If myLocalRefreshDS.ArmValueChanged.Rows.Contains(elementID) Then
                            For Each myRow As UIRefreshDS.ArmValueChangedRow In myLocalRefreshDS.ArmValueChanged.Rows
                                If myRow.ArmID = elementID.ToString Then
                                    returnValue = myRow
                                    Exit For
                                End If
                            Next
                        End If
                    End If

                End If

                Return returnValue
            End Get
        End Property

        Public ReadOnly Property InfoProbe(ByVal elementID As GlobalEnumerates.POLL_IDs) As UIRefreshDS.ProbeValueChangedRow
            Get
                Dim returnValue As UIRefreshDS.ProbeValueChangedRow = Nothing

                If Not myLocalRefreshDS Is Nothing Then
                    If myLocalRefreshDS.ProbeValueChanged.Rows.Count > 0 Then
                        If myLocalRefreshDS.ProbeValueChanged.Rows.Contains(elementID) Then
                            For Each myRow As UIRefreshDS.ProbeValueChangedRow In myLocalRefreshDS.ProbeValueChanged.Rows
                                If myRow.ProbeID = elementID.ToString Then
                                    returnValue = myRow
                                    Exit For
                                End If
                            Next
                        End If
                    End If

                End If

                Return returnValue
            End Get
        End Property

        Public ReadOnly Property InfoRotor(ByVal elementID As GlobalEnumerates.POLL_IDs) As UIRefreshDS.RotorValueChangedRow
            Get
                Dim returnValue As UIRefreshDS.RotorValueChangedRow = Nothing

                If Not myLocalRefreshDS Is Nothing Then
                    If myLocalRefreshDS.RotorValueChanged.Rows.Count > 0 Then
                        If myLocalRefreshDS.RotorValueChanged.Rows.Contains(elementID) Then
                            For Each myRow As UIRefreshDS.RotorValueChangedRow In myLocalRefreshDS.RotorValueChanged.Rows
                                If myRow.RotorID = elementID.ToString Then
                                    returnValue = myRow
                                    Exit For
                                End If
                            Next
                        End If
                    End If

                End If

                Return returnValue
            End Get
        End Property

        Public ReadOnly Property InfoPhotometrics(ByVal elementID As GlobalEnumerates.PHOTOMETRICS_ELEMENTS) As UIRefreshDS.PhotometricsValueChangedRow
            Get
                Dim returnValue As UIRefreshDS.PhotometricsValueChangedRow = Nothing

                If Not myLocalRefreshDS Is Nothing Then
                    If myLocalRefreshDS.PhotometricsValueChanged.Rows.Count > 0 Then
                        If myLocalRefreshDS.PhotometricsValueChanged.Rows.Contains(elementID) Then
                            For Each myRow As UIRefreshDS.PhotometricsValueChangedRow In myLocalRefreshDS.PhotometricsValueChanged.Rows
                                If myRow.ElementID = elementID.ToString Then
                                    returnValue = myRow
                                    Exit For
                                End If
                            Next
                        End If
                    End If

                End If

                Return returnValue
            End Get
        End Property

        Public ReadOnly Property InfoManifold(ByVal elementID As GlobalEnumerates.MANIFOLD_ELEMENTS) As UIRefreshDS.ManifoldValueChangedRow
            Get
                Dim returnValue As UIRefreshDS.ManifoldValueChangedRow = Nothing

                If Not myLocalRefreshDS Is Nothing Then
                    If myLocalRefreshDS.ManifoldValueChanged.Rows.Count > 0 Then
                        If myLocalRefreshDS.ManifoldValueChanged.Rows.Contains(elementID) Then
                            For Each myRow As UIRefreshDS.ManifoldValueChangedRow In myLocalRefreshDS.ManifoldValueChanged.Rows
                                If myRow.ElementID = elementID.ToString Then
                                    returnValue = myRow
                                    Exit For
                                End If
                            Next
                        End If
                    End If

                End If

                Return returnValue
            End Get
        End Property

        Public ReadOnly Property InfoFluidics(ByVal elementID As GlobalEnumerates.FLUIDICS_ELEMENTS) As UIRefreshDS.FluidicsValueChangedRow
            Get
                Dim returnValue As UIRefreshDS.FluidicsValueChangedRow = Nothing

                If Not myLocalRefreshDS Is Nothing Then
                    If myLocalRefreshDS.FluidicsValueChanged.Rows.Count > 0 Then
                        If myLocalRefreshDS.FluidicsValueChanged.Rows.Contains(elementID) Then
                            For Each myRow As UIRefreshDS.FluidicsValueChangedRow In myLocalRefreshDS.FluidicsValueChanged.Rows
                                If myRow.ElementID = elementID.ToString Then
                                    returnValue = myRow
                                    Exit For
                                End If
                            Next
                        End If
                    End If

                End If

                Return returnValue
            End Get
        End Property

        Public ReadOnly Property MaxPOLLHW() As Integer
            Get
                Return Me.MaxPOLLHWAttr
            End Get
        End Property

        Public Property NumPOLLHW() As Integer
            Get
                Return Me.NumPOLLHWAttr
            End Get
            Set(ByVal value As Integer)
                Me.NumPOLLHWAttr = value
            End Set
        End Property
#End Region

#Region "Public Methods"


        Public Function SendREAD_CYCLES(ByVal pQueryMode As GlobalEnumerates.Ax00Adjustsments) As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Try
                myResultData = myFwScriptDelegate.AnalyzerManager.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.READCYCLES, True, Nothing, pQueryMode)

            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "BaseFwScriptDelegate.SendREAD_CYCLES", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function


        Public Function SendWRITE_CYCLES(ByVal pQueryMode As GlobalEnumerates.Ax00Adjustsments) As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Try
                myResultData = myFwScriptDelegate.AnalyzerManager.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.WRITECYCLES, True, Nothing, pQueryMode)

            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "BaseFwScriptDelegate.SendWRITE_CYCLES", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function

        Public Function RefreshCyclesData(ByVal pRefreshEventType As List(Of GlobalEnumerates.UI_RefreshEvents), ByVal pRefreshDS As UIRefreshDS) As GlobalDataTO

            Dim myResultData As New GlobalDataTO

            Try

                If pRefreshEventType.Contains(GlobalEnumerates.UI_RefreshEvents.HWCYCLES_CHANGED) Then

                    'Generate UI_Refresh Cycles values dataset
                    Dim myNewCyclesValuesRow As UIRefreshDS.CyclesValuesChangedRow
                    myNewCyclesValuesRow = MyClass.CurrentCyclesCountItemsDT.NewCyclesValuesChangedRow
                    With myNewCyclesValuesRow
                        .BeginEdit()
                        .SubSystemID = pRefreshDS.CyclesValuesChanged.Rows(0).Item("SubSystemID").ToString
                        .ItemID = pRefreshDS.CyclesValuesChanged.Rows(0).Item("ItemID").ToString
                        .CycleUnits = pRefreshDS.CyclesValuesChanged.Rows(0).Item("CycleUnits").ToString
                        .CyclesCount = CLng(pRefreshDS.CyclesValuesChanged.Rows(0).Item("CyclesCount"))
                        .EndEdit()
                    End With
                    MyClass.CurrentCyclesCountItemsDT.AddCyclesValuesChangedRow(myNewCyclesValuesRow)

                    MyClass.CurrentCyclesCountItemsDT.AcceptChanges()

                End If

            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "InstrumentUpdateUtilDelegate.RefreshCyclesData", EventLogEntryType.Error, False)
            End Try

            Return myResultData

        End Function

#End Region

#Region "Private Methods"
        Private Function CheckFirmwareCompatibility() As Boolean
            Dim returnValue As Boolean
            Try

                ' PENDING : is necesary the compatibility check
                ' to implement when version package table's design has been created !

                ' to test : 
                returnValue = True

            Catch ex As Exception
                returnValue = False
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerInfoDelegate.CheckFirmwareCompatibility", EventLogEntryType.Error, False)
            End Try
            Return returnValue
        End Function
#End Region

    End Class

End Namespace
